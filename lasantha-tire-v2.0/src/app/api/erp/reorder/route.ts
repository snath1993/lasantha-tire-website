import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/core/lib/db';
import { validateSession } from '@/core/lib/session';

// ─── Constants ────────────────────────────────────────────────────────
const TARGET_STOCK_GENERAL = 4;
const TARGET_STOCK_MOTORBIKE = 2;
const DEAD_STOCK_DAYS = 180; // 6 months no sale = dead stock

export async function GET(request: NextRequest) {
  try {
    const sessionId = request.headers.get('x-session-id');
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }

    const pool = await getPool();
    const searchParams = request.nextUrl.searchParams;
    const brand = searchParams.get('brand'); // optional filter
    const analysisMonths = parseInt(searchParams.get('months') || '3', 10);

    // ──────────────────────────────────────────────
    // 1. All tyre items with current stock (including 0 stock)
    // ──────────────────────────────────────────────
    const stockReq = pool.request();
    let stockWhere = "WHERE im.Categoty IN ('TYRES', 'MOTORBIKE TYRES') AND im.Custom3 IS NOT NULL AND im.Custom3 <> ''";
    if (brand) {
      stockReq.input('brand', sql.NVarChar, brand);
      stockWhere += " AND im.Custom3 = @brand";
    }

    const stockQuery = `
      SELECT 
        im.ItemID, im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3 as Brand,
        iw.QTY as CurrentStock,
        GRN1.InvReferenceNo AS LastGRNNo, GRN1.InvoiceDate AS LastGRNDate, GRN1.Qty AS LastGRNQty
      FROM [View_Item Master Whatsapp] im
      JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
      OUTER APPLY (
        SELECT TOP 1 InvReferenceNo, InvoiceDate, Qty 
        FROM View_DirectSupplierInvoicesWhatsapp grn 
        WHERE grn.ItemID = im.ItemID 
        ORDER BY grn.InvoiceDate DESC
      ) GRN1
      ${stockWhere}
      ORDER BY iw.QTY ASC, im.ItemDescription
    `;
    const stockResult = await stockReq.query(stockQuery);

    // ──────────────────────────────────────────────
    // 2. Sales data for last N months (sell velocity)
    // ──────────────────────────────────────────────
    const salesReq = pool.request();
    salesReq.input('months', sql.Int, analysisMonths);
    let salesWhere = '';
    if (brand) {
      salesReq.input('salesBrand', sql.NVarChar, brand);
      salesWhere = "AND im.Custom3 = @salesBrand";
    }

    const salesQuery = `
      SELECT 
        d.Expr3 as ItemCode,
        CAST(d.Expr1 AS DATE) as SaleDate,
        SUM(CAST(d.Expr5 AS INT)) as DayQty
      FROM [View_Sales report whatsapp] d
      JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
      WHERE d.Expr1 >= DATEADD(month, -@months, GETDATE())
        AND d.Expr1 <= GETDATE()
        AND im.Categoty IN ('TYRES', 'MOTORBIKE TYRES')
        ${salesWhere}
      GROUP BY d.Expr3, CAST(d.Expr1 AS DATE)
      ORDER BY d.Expr3
    `;
    const salesResult = await salesReq.query(salesQuery);

    // ──────────────────────────────────────────────
    // 3. Aggregate sales per item
    // ──────────────────────────────────────────────
    const salesMap = new Map<string, { total: number; last7: number; last30: number; last90: number; lastSaleDate: Date | null; dailySales: { date: string; qty: number }[] }>();
    const now = new Date();

    for (const row of salesResult.recordset) {
      const code = row.ItemCode;
      const saleDate = new Date(row.SaleDate);
      const daysDiff = Math.floor((now.getTime() - saleDate.getTime()) / (1000 * 60 * 60 * 24));
      const qty = row.DayQty || 0;

      if (!salesMap.has(code)) {
        salesMap.set(code, { total: 0, last7: 0, last30: 0, last90: 0, lastSaleDate: null, dailySales: [] });
      }
      const entry = salesMap.get(code)!;
      entry.total += qty;
      if (daysDiff <= 7) entry.last7 += qty;
      if (daysDiff <= 30) entry.last30 += qty;
      if (daysDiff <= 90) entry.last90 += qty;
      if (!entry.lastSaleDate || saleDate > entry.lastSaleDate) {
        entry.lastSaleDate = saleDate;
      }
      entry.dailySales.push({ date: row.SaleDate, qty });
    }

    // ──────────────────────────────────────────────
    // 4. Check pending orders from ReOrderHistory (cross-DB)
    // ──────────────────────────────────────────────
    let pendingOrders = new Map<string, { orderDate: Date; orderedQty: number; status: string }>();
    try {
      const pendingReq = pool.request();
      const pendingQuery = `
        SELECT ItemCode, MAX(ProcessedAt) as LastOrderDate, SUM(OrderedQty) as TotalOrdered, Status
        FROM [WhatsAppAI].[dbo].[ReOrderHistory]
        WHERE Status IN ('SENT', 'PENDING')
          AND ProcessedAt >= DATEADD(day, -14, GETDATE())
        GROUP BY ItemCode, Status
      `;
      const pendingResult = await pendingReq.query(pendingQuery);
      for (const row of pendingResult.recordset) {
        pendingOrders.set(row.ItemCode, {
          orderDate: new Date(row.LastOrderDate),
          orderedQty: row.TotalOrdered,
          status: row.Status
        });
      }
    } catch {
      // Cross-DB query might fail — continue without pending data
    }

    // ──────────────────────────────────────────────
    // 5. Build reorder items with intelligence
    // ──────────────────────────────────────────────
    const reorderItems: ReorderItem[] = [];

    for (const item of stockResult.recordset) {
      const isMotorbike = item.Categoty?.toUpperCase().includes('MOTOR') || false;
      const targetStock = isMotorbike ? TARGET_STOCK_MOTORBIKE : TARGET_STOCK_GENERAL;
      const sales = salesMap.get(item.ItemID);
      const pending = pendingOrders.get(item.ItemID);

      // Sell velocity
      const monthlyRate = sales ? Math.round((sales.last30 / 30) * 30) : 0;
      const weeklyRate = sales ? sales.last7 : 0;
      const avg30 = sales ? sales.last30 / 30 : 0;
      const avg7 = sales ? sales.last7 / 7 : 0;

      // Velocity classification
      let velocity: 'hot' | 'rising' | 'steady' | 'slowing' | 'dead' = 'steady';
      if (!sales || sales.total === 0) {
        velocity = 'dead';
      } else if (avg7 > avg30 * 2 && sales.last7 > 0) {
        velocity = 'hot';
      } else if (avg7 > avg30 * 1.2) {
        velocity = 'rising';
      } else if (avg30 > 0 && avg7 < avg30 * 0.5) {
        velocity = 'slowing';
      }

      // Dead stock check
      const daysSinceLastSale = sales?.lastSaleDate
        ? Math.floor((now.getTime() - sales.lastSaleDate.getTime()) / (1000 * 60 * 60 * 24))
        : 999;
      const isDead = daysSinceLastSale > DEAD_STOCK_DAYS && item.CurrentStock > 0;

      // Stock status
      let stockStatus: 'out' | 'critical' | 'low' | 'ok' | 'dead' = 'ok';
      if (isDead) stockStatus = 'dead';
      else if (item.CurrentStock === 0) stockStatus = 'out';
      else if (item.CurrentStock <= 1) stockStatus = 'critical';
      else if (item.CurrentStock < targetStock) stockStatus = 'low';

      // Suggested order qty
      let suggestedQty = 0;
      if (stockStatus === 'out' || stockStatus === 'critical' || stockStatus === 'low') {
        if (isMotorbike) {
          suggestedQty = Math.max(weeklyRate, targetStock - item.CurrentStock);
        } else {
          suggestedQty = targetStock - item.CurrentStock;
          // If velocity is hot, add buffer
          if (velocity === 'hot') suggestedQty = Math.max(suggestedQty, weeklyRate * 2);
        }
      }
      if (suggestedQty < 0) suggestedQty = 0;

      // Skip items that are OK and don't need ordering
      if (stockStatus === 'ok' && !isDead) continue;

      // Last GRN info
      const lastGRN = item.LastGRNNo ? {
        no: item.LastGRNNo,
        date: item.LastGRNDate,
        qty: item.LastGRNQty
      } : null;

      // GRN age in days
      const grnAgeDays = lastGRN ? Math.floor((now.getTime() - new Date(lastGRN.date).getTime()) / (1000 * 60 * 60 * 24)) : null;

      reorderItems.push({
        itemId: item.ItemID,
        description: item.ItemDescription,
        brand: item.Brand,
        category: item.Categoty,
        currentStock: item.CurrentStock,
        targetStock,
        unitCost: item.UnitCost || 0,
        isMotorbike,
        stockStatus,
        velocity,
        suggestedQty,
        sales: {
          last7: sales?.last7 || 0,
          last30: sales?.last30 || 0,
          last90: sales?.last90 || 0,
          monthlyRate,
          weeklyRate,
          lastSaleDate: sales?.lastSaleDate?.toISOString() || null,
          daysSinceLastSale: daysSinceLastSale === 999 ? null : daysSinceLastSale,
        },
        lastGRN,
        grnAgeDays,
        pendingOrder: pending ? {
          orderDate: pending.orderDate.toISOString(),
          orderedQty: pending.orderedQty,
          status: pending.status,
        } : null,
      });
    }

    // ──────────────────────────────────────────────
    // 6. Summary stats
    // ──────────────────────────────────────────────
    const outOfStock = reorderItems.filter(i => i.stockStatus === 'out').length;
    const critical = reorderItems.filter(i => i.stockStatus === 'critical').length;
    const lowStock = reorderItems.filter(i => i.stockStatus === 'low').length;
    const deadStock = reorderItems.filter(i => i.stockStatus === 'dead').length;
    const motorbike = reorderItems.filter(i => i.isMotorbike && i.stockStatus !== 'dead').length;
    const pendingCount = reorderItems.filter(i => i.pendingOrder).length;
    const totalSuggestedQty = reorderItems.reduce((s, i) => s + i.suggestedQty, 0);
    const estCost = reorderItems.reduce((s, i) => s + (i.suggestedQty * i.unitCost), 0);

    // Brands list for filter
    const brands = [...new Set(reorderItems.map(i => i.brand))].sort();

    return NextResponse.json({
      success: true,
      summary: {
        total: reorderItems.length,
        outOfStock,
        critical,
        lowStock,
        deadStock,
        motorbike,
        pendingOrders: pendingCount,
        totalSuggestedQty,
        estimatedCost: Math.round(estCost),
      },
      brands,
      items: reorderItems,
      analysisMonths,
      timestamp: new Date().toISOString(),
    });
  } catch (error: any) {
    console.error('[Reorder API] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to generate reorder data',
    }, { status: 500 });
  }
}

// ─── Types ────────────────────────────────────────────────────────────
interface ReorderItem {
  itemId: string;
  description: string;
  brand: string;
  category: string;
  currentStock: number;
  targetStock: number;
  unitCost: number;
  isMotorbike: boolean;
  stockStatus: 'out' | 'critical' | 'low' | 'ok' | 'dead';
  velocity: 'hot' | 'rising' | 'steady' | 'slowing' | 'dead';
  suggestedQty: number;
  sales: {
    last7: number;
    last30: number;
    last90: number;
    monthlyRate: number;
    weeklyRate: number;
    lastSaleDate: string | null;
    daysSinceLastSale: number | null;
  };
  lastGRN: { no: string; date: string; qty: number } | null;
  grnAgeDays: number | null;
  pendingOrder: { orderDate: string; orderedQty: number; status: string } | null;
}
