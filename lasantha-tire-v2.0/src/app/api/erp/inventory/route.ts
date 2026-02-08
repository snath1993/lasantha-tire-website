import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/core/lib/db';
import { validateSession } from '@/core/lib/session';

// Pricing Constants (Default values matching utils/pricingConfig.js)
const PRICING = {
  BASE_ROUND_STEP: 50,
  PUBLIC_BASE_MARKUP: 1500,
  PUBLIC_EXTRA_BUFFER: 500,
  ALLOWED_DEFAULT_MARKUP: 1500,
  MOTORBIKE_EXTRA: 1000,
  MAXXIS_PUBLIC_MARKUP: 500
};

const SPECIAL_MOTORBIKE_SIZES = ['100/90/17', '90/100/10', '140/60/17', '400/8'];

function normalizeBrand(brand: string): string {
  if (!brand) return '';
  return brand.toUpperCase().trim().replace(/[^A-Z0-9]/g, '');
}

function roundToStep(cost: number, step: number): number {
  return Math.round(cost / step) * step;
}

function computeSellingPrice(tyre: any, tyreSize: string) {
  const pricing = {
    baseRoundStep: parseInt(process.env.PRICE_ROUND_STEP || String(PRICING.BASE_ROUND_STEP), 10),
    publicBaseMarkup: parseInt(process.env.PUBLIC_BASE_MARKUP || String(PRICING.PUBLIC_BASE_MARKUP), 10),
    publicExtraBuffer: parseInt(process.env.PUBLIC_EXTRA_BUFFER || String(PRICING.PUBLIC_EXTRA_BUFFER), 10),
    allowedDefaultMarkup: parseInt(process.env.ALLOWED_DEFAULT_MARKUP || String(PRICING.ALLOWED_DEFAULT_MARKUP), 10),
    motorbikeExtra: parseInt(process.env.MOTORBIKE_EXTRA || String(PRICING.MOTORBIKE_EXTRA), 10),
    maxxisPublicMarkup: parseInt(process.env.MAXXIS_PUBLIC_MARKUP || String(PRICING.MAXXIS_PUBLIC_MARKUP), 10)
  };

  const rounded = roundToStep(tyre.UnitCost, pricing.baseRoundStep);
  const normBrand = normalizeBrand(tyre.Custom3 || '');
  const isMotorbike = (tyre.Categoty && tyre.Categoty.toUpperCase().includes('MOTOR')) || false;

  // Motorbike special uplift (only selected sizes)
  if (isMotorbike && SPECIAL_MOTORBIKE_SIZES.includes(tyreSize)) {
      return rounded + pricing.motorbikeExtra;
  }

  // Public pricing (non-allowed)
  if (normBrand === 'MAXXIS' || normBrand === 'MAXXIES') {
      return rounded + pricing.maxxisPublicMarkup;
  }
  return rounded + pricing.publicBaseMarkup + pricing.publicExtraBuffer;
}

export async function GET(request: NextRequest) {
  try {
    // 🔒 Security: Validate Session
    const sessionId = request.headers.get('x-session-id');
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }

    const pool = await getPool();
    const searchParams = request.nextUrl.searchParams;
    const type = searchParams.get('type') || 'summary';
    
    let result;
    
    if (type === 'search') {
        const query = searchParams.get('query') || '';
        const width = searchParams.get('width');
        const profile = searchParams.get('profile');
        const rim = searchParams.get('rim');
        
        // Construct search term from components if query is empty
        let searchTerm = query;
        if (!searchTerm && width && profile && rim) {
            searchTerm = `${width}/${profile}/${rim}`; // e.g. 195/65/15
        } else if (!searchTerm && width && rim) {
             searchTerm = `${width}/${rim}`; // e.g. 195/15 (unlikely but possible)
        }

        if (!searchTerm) {
             return NextResponse.json({ error: 'Search query or tyre dimensions required' }, { status: 400 });
        }

        // Use the exact logic from TyrePriceReplyJob.js
        // SELECT im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3
        // FROM [View_Item Master Whatsapp] im
        // JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
        // WHERE im.Categoty = 'TYRES' ...

        const request = pool.request();
        request.input('search', sql.NVarChar, `%${searchTerm}%`);
        request.input('searchNoSlash', sql.NVarChar, `%${searchTerm.replace(/\//g, '')}%`);

        const sqlQuery = `
            SELECT im.ItemID, im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3, iw.QTY,
                   GRN1.InvReferenceNo AS GRN1No, GRN1.InvoiceDate AS GRN1Date, GRN1.Qty AS GRN1Qty,
                   GRN2.InvReferenceNo AS GRN2No, GRN2.InvoiceDate AS GRN2Date, GRN2.Qty AS GRN2Qty,
                   GRN3.InvReferenceNo AS GRN3No, GRN3.InvoiceDate AS GRN3Date, GRN3.Qty AS GRN3Qty,
                   GRN4.InvReferenceNo AS GRN4No, GRN4.InvoiceDate AS GRN4Date, GRN4.Qty AS GRN4Qty,
                   GRN5.InvReferenceNo AS GRN5No, GRN5.InvoiceDate AS GRN5Date, GRN5.Qty AS GRN5Qty
            FROM [View_Item Master Whatsapp] im
            JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            OUTER APPLY (SELECT TOP 1 InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC) GRN1
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) GRN2
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY) GRN3
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY) GRN4
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY) GRN5
            WHERE im.Categoty = 'TYRES'
              AND iw.QTY > 0
              AND (
                  im.ItemDescription LIKE @search
                  OR im.ItemDescription LIKE @searchNoSlash
              )
        `;
        
        result = await request.query(sqlQuery);
        
        // Process results to add calculated selling price
        const processedData = result.recordset.map(item => {
            const grnHistory = [];
            if (item.GRN1No) grnHistory.push({ InvReferenceNo: item.GRN1No, InvoiceDate: item.GRN1Date, Qty: item.GRN1Qty });
            if (item.GRN2No) grnHistory.push({ InvReferenceNo: item.GRN2No, InvoiceDate: item.GRN2Date, Qty: item.GRN2Qty });
            if (item.GRN3No) grnHistory.push({ InvReferenceNo: item.GRN3No, InvoiceDate: item.GRN3Date, Qty: item.GRN3Qty });
            if (item.GRN4No) grnHistory.push({ InvReferenceNo: item.GRN4No, InvoiceDate: item.GRN4Date, Qty: item.GRN4Qty });
            if (item.GRN5No) grnHistory.push({ InvReferenceNo: item.GRN5No, InvoiceDate: item.GRN5Date, Qty: item.GRN5Qty });

            const lastGRN = grnHistory.length > 0 ? {
                No: grnHistory[0].InvReferenceNo,
                Date: grnHistory[0].InvoiceDate,
                Qty: grnHistory[0].Qty,
                History: grnHistory
            } : null;

            return {
                ItemId: item.ItemID,
                Description: item.ItemDescription,
                Quantity: item.QTY,
                UnitCost: item.UnitCost,
                SellingPrice: computeSellingPrice(item, searchTerm),
                Brand: item.Custom3 || 'Unknown',
                Category: item.Categoty,
                LastGRN: lastGRN
            };
        });

        return NextResponse.json({
            success: true,
            type,
            data: processedData,
            timestamp: new Date().toISOString()
        });
    } else if (type === 'brands') {
        const request = pool.request();
        const sqlQuery = `
            SELECT im.Custom3 as Brand, SUM(iw.QTY) as TotalQty
            FROM [View_Item Master Whatsapp] im
            JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            WHERE im.Categoty = 'TYRES' AND im.Custom3 IS NOT NULL AND im.Custom3 <> '' AND iw.QTY > 0
            GROUP BY im.Custom3
            ORDER BY TotalQty DESC
        `;
        result = await request.query(sqlQuery);
        // Return array of objects { Brand, TotalQty }
        return NextResponse.json({ success: true, data: result.recordset });
    } else if (type === 'list') {
        const page = parseInt(searchParams.get('page') || '1');
        const limit = parseInt(searchParams.get('limit') || '50');
        const brand = searchParams.get('brand');
        const offset = (page - 1) * limit;
        
        const request = pool.request();
        request.input('offset', sql.Int, offset);
        request.input('limit', sql.Int, limit);

        let whereClause = "WHERE im.Categoty = 'TYRES' AND iw.QTY > 0";
        
        if (brand) {
            request.input('brand', sql.NVarChar, brand);
            whereClause += " AND im.Custom3 = @brand";
        }

        const sqlQuery = `
            SELECT im.ItemID, im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3, iw.QTY,
                   GRN1.InvReferenceNo AS GRN1No, GRN1.InvoiceDate AS GRN1Date, GRN1.Qty AS GRN1Qty,
                   GRN2.InvReferenceNo AS GRN2No, GRN2.InvoiceDate AS GRN2Date, GRN2.Qty AS GRN2Qty,
                   GRN3.InvReferenceNo AS GRN3No, GRN3.InvoiceDate AS GRN3Date, GRN3.Qty AS GRN3Qty,
                   GRN4.InvReferenceNo AS GRN4No, GRN4.InvoiceDate AS GRN4Date, GRN4.Qty AS GRN4Qty,
                   GRN5.InvReferenceNo AS GRN5No, GRN5.InvoiceDate AS GRN5Date, GRN5.Qty AS GRN5Qty
            FROM [View_Item Master Whatsapp] im
            JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            OUTER APPLY (SELECT TOP 1 InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC) GRN1
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) GRN2
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY) GRN3
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY) GRN4
            OUTER APPLY (SELECT InvReferenceNo, InvoiceDate, Qty FROM View_DirectSupplierInvoicesWhatsapp grn WHERE grn.ItemID = im.ItemID ORDER BY grn.InvoiceDate DESC OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY) GRN5
            ${whereClause}
            ORDER BY im.ItemDescription
            OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
        `;
        
        result = await request.query(sqlQuery);
        
        const processedData = result.recordset.map(item => {
            const grnHistory = [];
            if (item.GRN1No) grnHistory.push({ InvReferenceNo: item.GRN1No, InvoiceDate: item.GRN1Date, Qty: item.GRN1Qty });
            if (item.GRN2No) grnHistory.push({ InvReferenceNo: item.GRN2No, InvoiceDate: item.GRN2Date, Qty: item.GRN2Qty });
            if (item.GRN3No) grnHistory.push({ InvReferenceNo: item.GRN3No, InvoiceDate: item.GRN3Date, Qty: item.GRN3Qty });
            if (item.GRN4No) grnHistory.push({ InvReferenceNo: item.GRN4No, InvoiceDate: item.GRN4Date, Qty: item.GRN4Qty });
            if (item.GRN5No) grnHistory.push({ InvReferenceNo: item.GRN5No, InvoiceDate: item.GRN5Date, Qty: item.GRN5Qty });

            const lastGRN = grnHistory.length > 0 ? {
                No: grnHistory[0].InvReferenceNo,
                Date: grnHistory[0].InvoiceDate,
                Qty: grnHistory[0].Qty,
                History: grnHistory // Pass full history to frontend
            } : null;

            return {
                ItemId: item.ItemID,
                Description: item.ItemDescription,
                Quantity: item.QTY,
                UnitCost: item.UnitCost,
                SellingPrice: computeSellingPrice(item, ''),
                Brand: item.Custom3 || 'Unknown',
                Category: item.Categoty,
                LastGRN: lastGRN
            };
        });

        return NextResponse.json({
            success: true,
            type,
            data: processedData,
            page,
            limit,
            timestamp: new Date().toISOString()
        });
    } else if (type === 'services') {
        const category = searchParams.get('category');
        const query = searchParams.get('query') || '';
        const itemId = searchParams.get('itemId');
        
        const request = pool.request();
        let sqlQuery = `
            SELECT im.ItemID, im.ItemDescription, im.UnitCost, im.PriceLevel1, im.Categoty, im.Custom3, iw.QTY
            FROM [View_Item Master Whatsapp] im
            JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            WHERE im.Categoty NOT IN ('TYRES', 'DAG TYRES', 'REBUILD TYRES', 'SAVIYA TYRES', 'KATTA TYRES', 'RADIAL DAG TYRES')
        `;

        if (itemId) {
            request.input('itemId', sql.NVarChar, itemId);
            sqlQuery += " AND im.ItemID = @itemId";
        } else {
            if (category) {
                request.input('category', sql.NVarChar, category);
                sqlQuery += " AND im.Categoty = @category";
            }

            if (query) {
                request.input('search', sql.NVarChar, `%${query}%`);
                sqlQuery += " AND im.ItemDescription LIKE @search";
            }
        }

        sqlQuery += " ORDER BY im.Categoty, im.ItemDescription";
        
        result = await request.query(sqlQuery);
        
        const processedData = result.recordset.map(item => {
            return {
                ItemId: item.ItemID,
                Description: item.ItemDescription,
                Quantity: item.QTY,
                UnitCost: item.UnitCost,
                SellingPrice: item.PriceLevel1, // Use PriceLevel1 for services
                Brand: item.Custom3 || '-',
                Category: item.Categoty
            };
        });

        return NextResponse.json({
            success: true,
            type,
            data: processedData,
            timestamp: new Date().toISOString()
        });
    } else if (type === 'service_categories') {
        const request = pool.request();
        const sqlQuery = `
            SELECT DISTINCT Categoty 
            FROM [View_Item Master Whatsapp] 
            WHERE Categoty NOT IN ('TYRES', 'DAG TYRES', 'REBUILD TYRES', 'SAVIYA TYRES', 'KATTA TYRES', 'RADIAL DAG TYRES')
            AND Categoty IS NOT NULL AND Categoty <> ''
            ORDER BY Categoty
        `;
        result = await request.query(sqlQuery);
        return NextResponse.json({ success: true, data: result.recordset.map(r => r.Categoty) });
    } else {
        // Fallback for other types if needed, or just return error
        // For now, we only implement the 'search' logic as requested
         return NextResponse.json({ error: 'Only search type is supported with this logic' }, { status: 400 });
    }
    
  } catch (error: any) {
    console.error('[ERP/Inventory API] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to fetch inventory data',
      timestamp: new Date().toISOString()
    }, { status: 500 });
  }
}
