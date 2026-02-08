import { NextResponse } from 'next/server';
import { getPool } from '@/core/lib/db';
import sql from 'mssql';

export async function GET(request: Request) {
  try {
    const { searchParams } = new URL(request.url);
    const range = searchParams.get('range') || 'this_month';
    const customStart = searchParams.get('startDate');
    const customEnd = searchParams.get('endDate');
    
    let startDate = new Date();
    let endDate = new Date();
    
    // Set time to end of day for endDate
    endDate.setHours(23, 59, 59, 999);

    switch (range) {
      case 'today':
        startDate.setHours(0, 0, 0, 0);
        break;
      case 'yesterday':
        startDate.setDate(startDate.getDate() - 1);
        startDate.setHours(0, 0, 0, 0);
        endDate.setDate(endDate.getDate() - 1);
        endDate.setHours(23, 59, 59, 999);
        break;
      case 'this_month':
        startDate.setDate(1);
        startDate.setHours(0, 0, 0, 0);
        break;
      case 'last_month':
        startDate.setMonth(startDate.getMonth() - 1);
        startDate.setDate(1);
        startDate.setHours(0, 0, 0, 0);
        endDate.setDate(0); // Last day of previous month
        endDate.setHours(23, 59, 59, 999);
        break;
      case 'custom':
        if (customStart) startDate = new Date(customStart);
        if (customEnd) endDate = new Date(customEnd);
        break;
    }

    const pool = await getPool();

    // 1. Total Sales & Profit
    const summaryQuery = `
      SELECT 
        SUM(Amount) as TotalSales,
        SUM((Qty * UnitCost)) as TotalCost,
        SUM(Amount - (Qty * UnitCost)) as TotalProfit,
        COUNT(DISTINCT InvoiceNo) as InvoiceCount
      FROM [View_Sales report whatsapp]
      WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate
      AND IsVoid = 0
    `;

    // 2. Sales by Category
    const categoryQuery = `
      SELECT 
        Categoty as name,
        SUM(Amount) as value,
        SUM(Amount - (Qty * UnitCost)) as profit
      FROM [View_Sales report whatsapp]
      WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate
      AND IsVoid = 0
      GROUP BY Categoty
      ORDER BY value DESC
    `;

    // 3. Daily Trend
    const trendQuery = `
      SELECT 
        FORMAT(InvoiceDate, 'yyyy-MM-dd') as date,
        SUM(Amount) as sales,
        SUM(Amount - (Qty * UnitCost)) as profit
      FROM [View_Sales report whatsapp]
      WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate
      AND IsVoid = 0
      GROUP BY FORMAT(InvoiceDate, 'yyyy-MM-dd')
      ORDER BY date ASC
    `;

    // 4. Top Items
    const itemsQuery = `
      SELECT TOP 10
        Description as name,
        SUM(Qty) as quantity,
        SUM(Amount) as sales,
        SUM(Amount - (Qty * UnitCost)) as profit
      FROM [View_Sales report whatsapp]
      WHERE InvoiceDate >= @startDate AND InvoiceDate <= @endDate
      AND IsVoid = 0
      GROUP BY Description
      ORDER BY sales DESC
    `;

    const sqlRequest = pool.request();
    sqlRequest.input('startDate', sql.DateTime, startDate);
    sqlRequest.input('endDate', sql.DateTime, endDate);

    const [summary, categories, trend, items] = await Promise.all([
      sqlRequest.query(summaryQuery),
      sqlRequest.query(categoryQuery),
      sqlRequest.query(trendQuery),
      sqlRequest.query(itemsQuery)
    ]);

    return NextResponse.json({
      success: true,
      range: { start: startDate, end: endDate },
      summary: summary.recordset[0],
      categories: categories.recordset,
      trend: trend.recordset,
      topItems: items.recordset
    });

  } catch (error: any) {
    console.error('Analytics API Error:', error);
    return NextResponse.json(
      { success: false, error: error.message },
      { status: 500 }
    );
  }
}
