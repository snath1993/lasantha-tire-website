import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/core/lib/db';
import { validateSession } from '@/core/lib/session';

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
    
    const pool = await getPool();
    let result;
    
    switch(type) {
      case 'summary':
        result = await pool.request().query(`
          SELECT 
            (SELECT SUM(CAST(Amount as DECIMAL(18,2)))
             FROM [View_Sales report whatsapp]
             WHERE InvoiceDate >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
               AND InvoiceDate < DATEADD(month, DATEDIFF(month, 0, GETDATE()) + 1, 0)
               AND Amount IS NOT NULL
            ) as CurrentMonthRevenue,
            (SELECT SUM(CAST(Amount as DECIMAL(18,2)))
             FROM [View_Sales report whatsapp]
             WHERE InvoiceDate >= DATEADD(month, DATEDIFF(month, 0, GETDATE()) - 1, 0)
               AND InvoiceDate < DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
               AND Amount IS NOT NULL
            ) as LastMonthRevenue,
            (SELECT SUM(CAST(Amount as DECIMAL(18,2)))
             FROM [View_Sales report whatsapp]
             WHERE YEAR(InvoiceDate) = YEAR(GETDATE())
               AND Amount IS NOT NULL
            ) as YearToDateRevenue,
            (SELECT COUNT(DISTINCT CustomerName)
             FROM [View_Sales report whatsapp]
             WHERE InvoiceDate >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
               AND CustomerName IS NOT NULL
            ) as ActiveCustomersThisMonth
        `);
        break;
        
      case 'revenue-comparison':
        result = await pool.request().query(`
          SELECT 
            YEAR(InvoiceDate) as Year,
            MONTH(InvoiceDate) as Month,
            DATENAME(month, InvoiceDate) as MonthName,
            SUM(CAST(Amount as DECIMAL(18,2))) as Revenue,
            COUNT(DISTINCT InvoiceNo) as InvoiceCount,
            COUNT(DISTINCT CustomerName) as UniqueCustomers
          FROM [View_Sales report whatsapp]
          WHERE InvoiceDate >= DATEADD(month, -12, GETDATE())
            AND Amount IS NOT NULL
          GROUP BY YEAR(InvoiceDate), MONTH(InvoiceDate), DATENAME(month, InvoiceDate)
          ORDER BY Year DESC, Month DESC
        `);
        break;
        
      case 'top-customers':
        const limit = parseInt(searchParams.get('limit') || '20', 10);
        const days = parseInt(searchParams.get('days') || '90', 10);
        result = await pool.request()
          .input('limit', sql.Int, limit)
          .input('days', sql.Int, days)
          .query(`
            SELECT TOP (@limit)
              CustomerName,
              COUNT(DISTINCT InvoiceNo) as InvoiceCount,
              SUM(CAST(Amount as DECIMAL(18,2))) as TotalRevenue,
              AVG(CAST(Amount as DECIMAL(18,2))) as AvgInvoiceValue,
              MAX(InvoiceDate) as LastPurchaseDate
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND CustomerName IS NOT NULL
              AND Amount IS NOT NULL
            GROUP BY CustomerName
            ORDER BY SUM(CAST(Amount as DECIMAL(18,2))) DESC
          `);
        break;
        
      case 'payment-status':
        result = await pool.request().query(`
          SELECT 
            CASE 
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 30 THEN 'Current (0-30 days)'
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 60 THEN '31-60 days'
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 90 THEN '61-90 days'
              ELSE 'Over 90 days'
            END as AgingBucket,
            COUNT(DISTINCT InvoiceNo) as InvoiceCount,
            SUM(CAST(Amount as DECIMAL(18,2))) as TotalAmount
          FROM [View_Sales report whatsapp]
          WHERE Amount IS NOT NULL
            AND InvoiceDate >= DATEADD(month, -6, GETDATE())
          GROUP BY 
            CASE 
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 30 THEN 'Current (0-30 days)'
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 60 THEN '31-60 days'
              WHEN DATEDIFF(day, InvoiceDate, GETDATE()) <= 90 THEN '61-90 days'
              ELSE 'Over 90 days'
            END
          ORDER BY MIN(DATEDIFF(day, InvoiceDate, GETDATE()))
        `);
        break;
        
      case 'profit-analysis':
        result = await pool.request()
          .input('days', sql.Int, parseInt(searchParams.get('days') || '30', 10))
          .query(`
            SELECT 
              CAST(InvoiceDate as DATE) as Date,
              SUM(CAST(Amount as DECIMAL(18,2))) as Revenue,
              SUM(CAST(Qty as INT)) as UnitsSold,
              COUNT(DISTINCT InvoiceNo) as InvoiceCount
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= DATEADD(day, -@days, GETDATE())
              AND Amount IS NOT NULL
            GROUP BY CAST(InvoiceDate as DATE)
            ORDER BY Date DESC
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
    console.error('[ERP/Finance API] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to fetch financial data',
      timestamp: new Date().toISOString()
    }, { status: 500 });
  }
}
