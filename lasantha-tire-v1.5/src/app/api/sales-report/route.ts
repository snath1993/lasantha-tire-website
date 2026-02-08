import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/lib/db';
import { validateSession } from '@/lib/session';

export const dynamic = 'force-dynamic';
export const revalidate = 0;

type RangeKey = 'today' | 'yesterday' | 'this_month' | 'last_month' | 'custom';

const noStoreHeaders = {
  'Cache-Control': 'no-store, max-age=0',
  'Pragma': 'no-cache',
  'Expires': '0',
  // Avoid accidentally sharing cached results across sessions
  'Vary': 'x-session-id'
} as const;

function startOfDayLocal(date: Date) {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  return d;
}

function endOfDayLocal(date: Date) {
  const d = new Date(date);
  d.setHours(23, 59, 59, 999);
  return d;
}

function parseDateOnly(dateStr: string): Date | null {
  // Supports YYYY-MM-DD
  const m = /^\d{4}-\d{2}-\d{2}$/.exec(dateStr);
  if (!m) return null;
  const d = new Date(`${dateStr}T00:00:00`);
  return Number.isNaN(d.getTime()) ? null : d;
}

function computeRange(range: RangeKey, startDate?: string | null, endDate?: string | null) {
  const now = new Date();
  let start = new Date(now);
  let end = new Date(now);

  switch (range) {
    case 'today':
      start = startOfDayLocal(now);
      end = endOfDayLocal(now);
      break;
    case 'yesterday': {
      const y = new Date(now);
      y.setDate(y.getDate() - 1);
      start = startOfDayLocal(y);
      end = endOfDayLocal(y);
      break;
    }
    case 'this_month': {
      const m = new Date(now.getFullYear(), now.getMonth(), 1);
      start = startOfDayLocal(m);
      end = endOfDayLocal(now);
      break;
    }
    case 'last_month': {
      const firstThisMonth = new Date(now.getFullYear(), now.getMonth(), 1);
      const lastDayPrevMonth = new Date(firstThisMonth);
      lastDayPrevMonth.setDate(0);
      const firstPrevMonth = new Date(lastDayPrevMonth.getFullYear(), lastDayPrevMonth.getMonth(), 1);
      start = startOfDayLocal(firstPrevMonth);
      end = endOfDayLocal(lastDayPrevMonth);
      break;
    }
    case 'custom': {
      const s = startDate ? parseDateOnly(startDate) : null;
      const e = endDate ? parseDateOnly(endDate) : null;
      if (!s || !e) {
        return { ok: false as const, error: 'Invalid custom date range. Use YYYY-MM-DD.' };
      }
      start = startOfDayLocal(s);
      end = endOfDayLocal(e);
      if (start.getTime() > end.getTime()) {
        return { ok: false as const, error: 'Start date must be before end date.' };
      }
      return { ok: true as const, start, end };
    }
  }

  return { ok: true as const, start, end };
}

function computePreviousRange(start: Date, end: Date) {
  // Previous period of same length ending just before current start
  const prevEnd = new Date(start.getTime() - 1);
  const duration = end.getTime() - start.getTime();
  const prevStart = new Date(prevEnd.getTime() - duration);
  return { prevStart, prevEnd };
}

export async function GET(request: NextRequest) {
  try {
    const serverTime = new Date().toISOString();

    // 🔒 Security: Validate Session
    const sessionId = request.headers.get('x-session-id');
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ success: false, error: 'Unauthorized' }, { status: 401, headers: noStoreHeaders });
    }

    const pool = await getPool();
    const sp = request.nextUrl.searchParams;

    const mode = (sp.get('mode') || 'data').toLowerCase();
    if (mode === 'meta') {
      const req = pool.request();
      const categories = await req.query(`
        SELECT DISTINCT s.Categoty AS Category
        FROM [View_Sales report whatsapp] s
        WHERE ISNULL(s.IsVoid, 0) = 0
          AND s.Categoty IS NOT NULL AND LTRIM(RTRIM(s.Categoty)) <> ''
        ORDER BY s.Categoty ASC
      `);

      const brands = await pool.request().query(`
        SELECT DISTINCT im.Custom3 AS Brand
        FROM [View_Item Master Whatsapp] im
        WHERE im.Custom3 IS NOT NULL AND LTRIM(RTRIM(im.Custom3)) <> ''
        ORDER BY im.Custom3 ASC
      `);

      return NextResponse.json({
        success: true,
        categories: categories.recordset.map(r => r.Category).filter(Boolean),
        brands: brands.recordset.map(r => r.Brand).filter(Boolean)
      }, { headers: noStoreHeaders });
    }

    const range = (sp.get('range') || 'this_month') as RangeKey;
    const startDate = sp.get('startDate');
    const endDate = sp.get('endDate');
    const category = sp.get('category');
    const brand = sp.get('brand');
    const search = sp.get('search');

    const computed = computeRange(range, startDate, endDate);
    if (!computed.ok) {
      return NextResponse.json({ success: false, error: computed.error }, { status: 400, headers: noStoreHeaders });
    }

    const { start, end } = computed;
    const { prevStart, prevEnd } = computePreviousRange(start, end);

    const filterCategory = category && category.trim() ? category.trim() : null;
    const filterBrand = brand && brand.trim() ? brand.trim() : null;
    const filterSearch = search && search.trim() ? `%${search.trim()}%` : null;

    const baseWhere = `
      s.InvoiceDate >= @startDate AND s.InvoiceDate <= @endDate
      AND ISNULL(s.IsVoid, 0) = 0
      AND (@category IS NULL OR s.Categoty = @category)
      AND (@brand IS NULL OR im.Custom3 = @brand)
      AND (@search IS NULL OR s.Description LIKE @search)
    `;

    const prevWhere = `
      s.InvoiceDate >= @prevStartDate AND s.InvoiceDate <= @prevEndDate
      AND ISNULL(s.IsVoid, 0) = 0
      AND (@category IS NULL OR s.Categoty = @category)
      AND (@brand IS NULL OR im.Custom3 = @brand)
      AND (@search IS NULL OR s.Description LIKE @search)
    `;

    const makeRequest = () => {
      const req = pool.request();
      req.input('startDate', sql.DateTime, start);
      req.input('endDate', sql.DateTime, end);
      req.input('prevStartDate', sql.DateTime, prevStart);
      req.input('prevEndDate', sql.DateTime, prevEnd);
      req.input('category', sql.NVarChar, filterCategory);
      req.input('brand', sql.NVarChar, filterBrand);
      req.input('search', sql.NVarChar, filterSearch);
      return req;
    };

    const summaryQuery = `
      SELECT
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS TotalSales,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0)) AS TotalQty,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0)) AS TotalCost,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS TotalProfit,
        COUNT(DISTINCT s.InvoiceNo) AS InvoiceCount
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
    `;

    const prevSummaryQuery = `
      SELECT
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS TotalSales,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0)) AS TotalQty,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0)) AS TotalCost,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS TotalProfit,
        COUNT(DISTINCT s.InvoiceNo) AS InvoiceCount
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${prevWhere}
    `;

    const categoryBreakdownQuery = `
      SELECT
        s.Categoty AS name,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS sales,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0)) AS quantity,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
      GROUP BY s.Categoty
      ORDER BY sales DESC
    `;

    const brandBreakdownQuery = `
      SELECT
        COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown') AS name,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS sales,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0)) AS quantity,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
      GROUP BY COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown')
      ORDER BY sales DESC
    `;

    const paymentBreakdownQuery = `
      SELECT
        COALESCE(NULLIF(LTRIM(RTRIM(s.PaymentM)), ''), 'Unknown') AS name,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS sales,
        COUNT(DISTINCT s.InvoiceNo) AS count
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
      GROUP BY COALESCE(NULLIF(LTRIM(RTRIM(s.PaymentM)), ''), 'Unknown')
      ORDER BY sales DESC
    `;

    const trendQuery = `
      SELECT
        CONVERT(varchar(10), CONVERT(date, s.InvoiceDate), 23) AS date,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS sales,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
      GROUP BY CONVERT(date, s.InvoiceDate)
      ORDER BY CONVERT(date, s.InvoiceDate) ASC
    `;

    const itemBreakdownQuery = `
      SELECT TOP 50
        s.ItemID AS itemId,
        s.Description AS name,
        s.Categoty AS category,
        COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown') AS brand,
        SUM(COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0)) AS quantity,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0)) AS sales,
        SUM(COALESCE(CAST(s.Amount AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Qty AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
      GROUP BY s.ItemID, s.Description, s.Categoty, COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown')
      ORDER BY sales DESC
    `;

    const req = makeRequest();

    const diagnosticsQuery = `
      SELECT
        MAX(s.InvoiceDate) AS maxInvoiceDate,
        COUNT(1) AS lineCount
      FROM [View_Sales report whatsapp] s
      LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.ItemID
      WHERE ${baseWhere}
    `;

    const [summary, prevSummary, categories, brands, payments, trend, items, diagnostics] = await Promise.all([
      req.query(summaryQuery),
      req.query(prevSummaryQuery),
      req.query(categoryBreakdownQuery),
      req.query(brandBreakdownQuery),
      req.query(paymentBreakdownQuery),
      req.query(trendQuery),
      req.query(itemBreakdownQuery),
      req.query(diagnosticsQuery)
    ]);

    const diagnosticsRow = diagnostics.recordset?.[0] || null;

    return NextResponse.json({
      success: true,
      diagnostics: {
        serverTime,
        maxInvoiceDate: diagnosticsRow?.maxInvoiceDate ? new Date(diagnosticsRow.maxInvoiceDate).toISOString() : null,
        lineCount: typeof diagnosticsRow?.lineCount === 'number' ? diagnosticsRow.lineCount : Number(diagnosticsRow?.lineCount ?? 0)
      },
      filters: {
        range,
        start: start.toISOString(),
        end: end.toISOString(),
        category: filterCategory,
        brand: filterBrand,
        search: filterSearch ? filterSearch.replace(/%/g, '') : null
      },
      comparison: {
        prevStart: prevStart.toISOString(),
        prevEnd: prevEnd.toISOString()
      },
      summary: summary.recordset[0] || null,
      prevSummary: prevSummary.recordset[0] || null,
      categoryBreakdown: categories.recordset || [],
      brandBreakdown: brands.recordset || [],
      paymentBreakdown: payments.recordset || [],
      trend: trend.recordset || [],
      topItems: items.recordset || []
    }, { headers: noStoreHeaders });
  } catch (error: any) {
    console.error('Sales Report API Error:', error);
    return NextResponse.json({ success: false, error: error?.message || String(error) }, { status: 500, headers: noStoreHeaders });
  }
}
