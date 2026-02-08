const sql = require('mssql');
const moment = require('moment');

async function generateMonthlyTyreReportSQL(sqlConnection, monthRange, options = {}) {
  const { start, end } = monthRange;
  const { strict = false } = options;

  // Base CTE for reuse
  // Debug query first
  const debugQuery = `
  SELECT TOP 10
    si.Description as SalesDesc,
    im.ItemDescription as MasterDesc,
    im.UnitCost,
    UPPER(SUBSTRING(si.Description, 1, 8)) as SalesPrefix,
    UPPER(SUBSTRING(im.ItemDescription, 1, 8)) as MasterPrefix,
    CASE WHEN UPPER(SUBSTRING(si.Description, 1, 8)) = UPPER(SUBSTRING(im.ItemDescription, 1, 8)) THEN 'YES' ELSE 'NO' END as PrefixMatch
  FROM tblSalesInvoices si
  CROSS APPLY (
    SELECT TOP 5 ItemDescription, UnitCost
    FROM tblItemMaster
    WHERE Categoty = 'TYRES'
      AND UnitCost > 0
      AND (
        UPPER(SUBSTRING(ItemDescription, 1, 8)) = UPPER(SUBSTRING(si.Description, 1, 8))
        OR UPPER(ItemDescription) LIKE '%' + UPPER(SUBSTRING(si.Description, 1, 8)) + '%'
      )
    ORDER BY UnitCost DESC
  ) im
  WHERE si.InvoiceDate BETWEEN @start AND @end
    AND si.Description LIKE '%[0-9][0-9][0-9]/[0-9][0-9]/[0-9][0-9]%'
  ORDER BY si.InvoiceDate DESC`;

  const baseCTE = `
  WITH TyreBase AS (
    SELECT 
      si.InvoiceNo,
      si.InvoiceDate,
      si.Description,
      si.Qty,
      si.UnitPrice,
      ISNULL((
        SELECT TOP 1 UnitCost
        FROM tblItemMaster WITH (NOLOCK)
        WHERE ItemDescription = si.Description
          AND Categoty = 'TYRES'
          AND UnitCost > 0
        ORDER BY UnitCost DESC
      ), 0) as EffectiveCostPrice,
      (si.UnitPrice * si.Qty) AS LineTotal,
      ((si.UnitPrice - ISNULL((
        SELECT TOP 1 UnitCost
        FROM tblItemMaster WITH (NOLOCK)
        WHERE ItemDescription = si.Description
          AND Categoty = 'TYRES'
          AND UnitCost > 0
        ORDER BY UnitCost DESC
      ), 0)) * si.Qty) as Profit,
      CASE 
        WHEN UPPER(si.Description) LIKE '%MAXXIS%' THEN 'MAXXIS'
        WHEN UPPER(si.Description) LIKE '%BRIDGESTONE%' THEN 'BRIDGESTONE'
        WHEN UPPER(si.Description) LIKE '%GOODYEAR%' THEN 'GOODYEAR'
        WHEN UPPER(si.Description) LIKE '%GT%' THEN 'GT'
        WHEN UPPER(si.Description) LIKE '%DURATURN%' THEN 'DURATURN'
        ELSE 'Unknown'
      END AS BrandGuess,
      0 as WageAmount
    FROM tblSalesInvoices si WITH (NOLOCK)
    LEFT JOIN tblItemMaster im WITH (NOLOCK) ON si.Description = im.ItemDescription
      AND im.Categoty = 'TYRES'
      AND im.UnitCost > 0
    WHERE si.InvoiceDate BETWEEN @start AND @end
      AND si.Qty > 0
      AND (
        si.Description LIKE '%[0-9][0-9][0-9]/[0-9][0-9]/[0-9][0-9]%' 
        OR si.Description LIKE '%[0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]%'
      )
      AND si.Description NOT LIKE '%REPAIR%'
      AND si.Description NOT LIKE '%SERVICE%'
      AND si.Description NOT LIKE '%FITTING%'
      AND si.Description NOT LIKE '%BALANCE%'
      AND si.Description NOT LIKE '%ALIGNMENT%'
  )`;

  const overviewQuery = baseCTE + `
  SELECT 
    COUNT(DISTINCT InvoiceNo) AS TotalInvoices,
    SUM(LineTotal) AS TotalRevenue,
    SUM(Qty) AS TotalQty,
    SUM(Profit) AS TotalProfit,
    0 as TotalWages,
    CASE 
      WHEN SUM(LineTotal) > 0 THEN (SUM(Profit) * 100.0 / SUM(LineTotal))
      ELSE 0 
    END AS ProfitMargin
  FROM TyreBase`;

  const dailyQuery = baseCTE + `
  SELECT 
    CONVERT(date, InvoiceDate) AS Day,
    SUM(Qty) AS DailyQty,
    SUM(LineTotal) AS DailyRevenue,
    SUM(Profit) AS DailyProfit,
    0 as DailyWages,
    COUNT(DISTINCT InvoiceNo) as DailyInvoices
  FROM TyreBase
  GROUP BY CONVERT(date, InvoiceDate)`;

  const detailsQuery = baseCTE + `
  SELECT
    InvoiceNo,
    CONVERT(date, InvoiceDate) AS Day,
    Description,
    Qty,
    UnitPrice,
    LineTotal,
    Profit AS ItemProfit,
    0 as WageAmount,
    BrandGuess
  FROM TyreBase
  ORDER BY InvoiceDate DESC, LineTotal DESC`;

  try {
    // Create request with connection
    if (!sqlConnection || !sqlConnection.connected) {
      throw new Error('SQL connection is not established');
    }

    const request = new sql.Request(sqlConnection);
    request.input('start', sql.Date, start);
    request.input('end', sql.Date, end);

    // Run queries in parallel
    const [overview, daily, details] = await Promise.all([
      request.query(overviewQuery),
      request.query(dailyQuery),
      request.query(detailsQuery)
    ]);

    const overviewData = overview.recordset[0];
    const dailyData = daily.recordset;
    const detailData = details.recordset;

    // Calculate working days and validate data
    if (!overviewData) {
        throw new Error('No overview data returned from SQL query');
    }

    const workingDays = dailyData.length || 1;
    const totalRevenue = Number(overviewData.TotalRevenue) || 0;
    const totalQty = Number(overviewData.TotalQty) || 0;
    const totalProfit = Number(overviewData.TotalProfit) || 0;
    const totalInvoices = Number(overviewData.TotalInvoices) || 0;
    const profitMargin = Number(overviewData.ProfitMargin) || 0;

    // Build analytics structure with validated data
    const analytics = {
      period: { 
          start: moment(start).startOf('day').toDate(),
          end: moment(end).endOf('day').toDate()
      },
      tyreSummary: {
        totalQuantity: Math.max(0, totalQty),
        totalAmount: Math.max(0, totalRevenue),
        totalProfit: totalProfit,
        totalInvoices: Math.max(0, totalInvoices),
        profitMargin: Math.max(0, Math.min(100, profitMargin)),
        workingDays: Math.max(1, workingDays),
        avgDailyRevenue: Math.max(0, totalRevenue / Math.max(1, workingDays)),
        avgDailyProfit: totalProfit / Math.max(1, workingDays),
        avgDailyInvoices: Math.max(0, totalInvoices / Math.max(1, workingDays))
      },
      dateWiseAnalysis: {},
      diagnostics: {
        mode: 'sql-enhanced',
        includedLines: detailData.length,
        classificationBreakdown: { TYRES: detailData.length }
      }
    };

    // Process daily data
    dailyData.forEach(day => {
      const dateStr = moment(day.Day).format('YYYY-MM-DD');
      const dayDetails = detailData.filter(d => 
        moment(d.Day).format('YYYY-MM-DD') === dateStr
      );

      analytics.dateWiseAnalysis[dateStr] = {
        totalSales: Number(day.DailyRevenue || 0),
        totalProfit: Number(day.DailyProfit || 0),
        totalQty: Number(day.DailyQty || 0),
        invoices: Number(day.DailyInvoices || 0),
        items: dayDetails.map(item => ({
          Description: String(item.Description || ''),
          Category: String(item.BrandGuess || 'Unknown'),
          Qty: Number(item.Qty || 0),
          UnitPrice: Number(item.UnitPrice || 0),
          LineTotal: Number(item.LineTotal || 0),
          EffectiveCostPrice: Number(item.EffectiveCostPrice || 0),
          Profit: Number(item.Profit || 0),
          CostPrice: Number(item.EffectiveCostPrice || 0)
        })).filter(item => item.Description && item.Qty > 0)
      };
    });

    return analytics;

  } catch (error) {
    console.error('Failed to generate monthly tyre report:', error);
    throw error;
  }
}

module.exports = { generateMonthlyTyreReportSQL };