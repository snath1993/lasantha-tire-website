import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/lib/db';
import { validateSession } from '@/lib/session';

export async function GET(request: NextRequest) {
  try {
    // 🔒 Security: Validate Session
    const sessionId = request.headers.get('x-session-id');
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }

    const searchParams = request.nextUrl.searchParams;
    const type = searchParams.get('type') || 'summary';
    const days = parseInt(searchParams.get('days') || '30', 10);
    
    const pool = await getPool();
    let result;
    
    switch(type) {
      case 'summary':
        result = await pool.request()
          .input('days', sql.Int, days)
          .query(`
            SELECT 
              COUNT(DISTINCT InvoiceNo) as TotalInvoices,
              SUM(CAST(Amount as DECIMAL(18,2))) as TotalRevenue,
              AVG(CAST(Amount as DECIMAL(18,2))) as AverageInvoiceValue,
              COUNT(DISTINCT Description) as UniqueProductsSold
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND Amount IS NOT NULL
          `);
        break;
        
      case 'daily-trend':
        result = await pool.request()
          .input('days', sql.Int, days)
          .query(`
            SELECT 
              CAST(InvoiceDate as DATE) as Date,
              COUNT(DISTINCT InvoiceNo) as InvoiceCount,
              SUM(CAST(Amount as DECIMAL(18,2))) as Revenue,
              SUM(CAST(Qty as INT)) as TotalQuantity
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND Amount IS NOT NULL
            GROUP BY CAST(InvoiceDate as DATE)
            ORDER BY Date DESC
          `);
        break;
        
      case 'top-products':
        const limit = parseInt(searchParams.get('limit') || '20', 10);
        result = await pool.request()
          .input('days', sql.Int, days)
          .input('limit', sql.Int, limit)
          .query(`
            SELECT TOP (@limit)
              Description,
              COUNT(DISTINCT InvoiceNo) as InvoiceCount,
              SUM(CAST(Qty as INT)) as TotalQuantitySold,
              SUM(CAST(Amount as DECIMAL(18,2))) as TotalRevenue,
              AVG(CAST(UnitPrice as DECIMAL(18,2))) as AvgPrice
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND Amount IS NOT NULL
              AND Description IS NOT NULL
            GROUP BY Description
            ORDER BY SUM(CAST(Amount as DECIMAL(18,2))) DESC
          `);
        break;
        
      case 'by-brand':
        result = await pool.request()
          .input('days', sql.Int, days)
          .query(`
            SELECT TOP 15
              RTRIM(LTRIM(
                SUBSTRING(Description, 
                  CHARINDEX(' ', Description) + 1,
                  CASE 
                    WHEN CHARINDEX(' ', Description, CHARINDEX(' ', Description) + 1) > 0
                    THEN CHARINDEX(' ', Description, CHARINDEX(' ', Description) + 1) - CHARINDEX(' ', Description) - 1
                    ELSE 50
                  END
                )
              )) as Brand,
              COUNT(DISTINCT InvoiceNo) as InvoiceCount,
              SUM(CAST(Qty as INT)) as TotalQuantity,
              SUM(CAST(Amount as DECIMAL(18,2))) as TotalRevenue
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND Description LIKE '%/%'
              AND LEN(Description) > 10
              AND Amount IS NOT NULL
            GROUP BY RTRIM(LTRIM(
              SUBSTRING(Description, 
                CHARINDEX(' ', Description) + 1,
                CASE 
                  WHEN CHARINDEX(' ', Description, CHARINDEX(' ', Description) + 1) > 0
                  THEN CHARINDEX(' ', Description, CHARINDEX(' ', Description) + 1) - CHARINDEX(' ', Description) - 1
                  ELSE 50
                END
              )
            ))
            ORDER BY SUM(CAST(Amount as DECIMAL(18,2))) DESC
          `);
        break;
        
      case 'monthly':
        result = await pool.request().query(`
          SELECT 
            YEAR(InvoiceDate) as Year,
            MONTH(InvoiceDate) as Month,
            DATENAME(month, InvoiceDate) as MonthName,
            COUNT(DISTINCT InvoiceNo) as InvoiceCount,
            SUM(CAST(Amount as DECIMAL(18,2))) as Revenue,
            SUM(CAST(Qty as INT)) as Quantity
          FROM [View_Sales report whatsapp]
          WHERE InvoiceDate >= DATEADD(month, -12, GETDATE())
            AND Amount IS NOT NULL
          GROUP BY YEAR(InvoiceDate), MONTH(InvoiceDate), DATENAME(month, InvoiceDate)
          ORDER BY Year DESC, Month DESC
        `);
        break;
        
      case 'recent-invoices':
        const invoiceLimit = parseInt(searchParams.get('limit') || '50', 10);
        result = await pool.request()
          .input('limit', sql.Int, invoiceLimit)
          .query(`
            SELECT TOP (@limit)
              InvoiceNo,
              MAX(InvoiceDate) as InvoiceDate,
              MAX(CustomerName) as CustomerName,
              SUM(CAST(Amount as DECIMAL(18,2))) as TotalAmount,
              COUNT(*) as ItemCount
            FROM [View_Sales report whatsapp]
            WHERE Amount IS NOT NULL
            GROUP BY InvoiceNo
            ORDER BY MAX(InvoiceDate) DESC
          `);
        break;
        
      default:
        await pool.close();
        return NextResponse.json({ error: 'Invalid type parameter' }, { status: 400 });
    }

    await pool.close();
    
    return NextResponse.json({
      success: true,
      type,
      data: result.recordset,
      timestamp: new Date().toISOString()
    });
    
  } catch (error: any) {
    console.error('[ERP/Sales API] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to fetch sales data',
      timestamp: new Date().toISOString()
    }, { status: 500 });
  }
}
