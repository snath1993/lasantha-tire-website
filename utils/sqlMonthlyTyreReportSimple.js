// Simplified SQL monthly report with KATTA/DAG/REBUILD exclusions
const sql = require('mssql');
const moment = require('moment');

async function generateMonthlyTyreReportSQL(sqlConnection, monthRange, options = {}) {
  const { start, end } = monthRange;
  const { strict = false } = options;

  // Simple single query approach
  const query = `
WITH TyreBase AS (
  SELECT 
    si.InvoiceNo,
    si.InvoiceDate,
    si.Description,
    si.Qty,
    si.UnitPrice,
    ISNULL(im.UnitCost,0) AS CostPrice,
    (si.Qty * si.UnitPrice) AS LineTotal,
    si.Description AS ItemDescription,
    CASE WHEN im.Categoty = 'TYRES' THEN 1 ELSE 0 END AS IsTyre,
    CASE 
      WHEN im.Categoty = 'TYRES' THEN 'main-category-tyres'
      WHEN im.Categoty IN ('KATTA TYRES', 'DAG TYRES', 'REBUILD TYRES') THEN 'excluded-katta-dag-rebuild'
      ELSE 'other'
    END AS ClassReason,
    ISNULL(im.Categoty,'Unknown') AS MainCategory,
    CASE 
      WHEN UPPER(si.Description) LIKE '%MAXXIS%' THEN 'MAXXIS'
      WHEN UPPER(si.Description) LIKE '%BRIDGESTONE%' THEN 'BRIDGESTONE'
      WHEN UPPER(si.Description) LIKE '%GOODYEAR%' THEN 'GOODYEAR'
      WHEN UPPER(si.Description) LIKE '%GT%' THEN 'GT'
      WHEN UPPER(si.Description) LIKE '%DURATURN%' THEN 'DURATURN'
      ELSE 'Unknown'
    END AS BrandGuess
  FROM tblSalesInvoices si
  LEFT JOIN tblItemMaster im ON si.Description = im.ItemDescription
  WHERE si.InvoiceDate >= @start AND si.InvoiceDate <= @end
    AND si.Qty > 0
)
SELECT 
  'OVERVIEW' as ResultType,
  COUNT(DISTINCT CASE WHEN IsTyre=1 THEN InvoiceNo END) AS TotalInvoices,
  SUM(CASE WHEN IsTyre=1 THEN LineTotal ELSE 0 END) AS TotalRevenue,
  SUM(CASE WHEN IsTyre=1 THEN Qty ELSE 0 END) AS TotalQty,
  SUM(CASE WHEN IsTyre=1 THEN (LineTotal - (Qty*CostPrice)) ELSE 0 END) AS BasicProfit,
  CASE WHEN SUM(CASE WHEN IsTyre=1 THEN LineTotal ELSE 0 END) > 0 
    THEN SUM(CASE WHEN IsTyre=1 THEN (LineTotal - (Qty*CostPrice)) ELSE 0 END) * 100.0 / SUM(CASE WHEN IsTyre=1 THEN LineTotal ELSE 0 END)
    ELSE 0 END AS BasicMargin,
  NULL as BrandGuess, NULL as Day, NULL as ClassReason, NULL as Count
FROM TyreBase

UNION ALL

SELECT 
  'BRANDS' as ResultType,
  NULL, NULL, 
  SUM(Qty) AS TotalQty,
  SUM(LineTotal) AS TotalRevenue,
  NULL,
  BrandGuess, NULL, NULL, NULL
FROM TyreBase 
WHERE IsTyre=1
GROUP BY BrandGuess

UNION ALL

SELECT 
  'DAILY' as ResultType,
  NULL, NULL,
  SUM(Qty) AS TotalQty,
  SUM(LineTotal) AS TotalRevenue,
  SUM(CASE WHEN CostPrrice>0 THEN (LineTotal - (Qty*CostPrrice)) ELSE 0 END) AS BasicProfit,
  NULL,
  CONVERT(date, InvoiceDate) AS Day,
  NULL, NULL
FROM TyreBase 
WHERE IsTyre=1
GROUP BY CONVERT(date, InvoiceDate)

UNION ALL

SELECT 
  'DETAILS' as ResultType,
  NULL, NULL,
  Qty AS TotalQty,
  LineTotal AS TotalRevenue,
  CASE WHEN CostPrrice>0 THEN (LineTotal - (Qty*CostPrrice)) ELSE 0 END AS BasicProfit,
  BrandGuess,
  CONVERT(date, InvoiceDate) AS Day,
  MainCategory AS ClassReason,
  NULL
FROM TyreBase 
WHERE IsTyre=1

UNION ALL

SELECT 
  'CLASSIFICATION' as ResultType,
  NULL, NULL, NULL, NULL, NULL, NULL, NULL,
  ClassReason,
  COUNT(*) AS Count
FROM TyreBase
GROUP BY ClassReason

ORDER BY ResultType, Day, TotalQty DESC;`;

  const request = new sql.Request();
  request.input('start', start);
  request.input('end', end);
  
  const result = await request.query(query);
  
  // Parse results by type
  const rows = result.recordset;
  const overview = rows.find(r => r.ResultType === 'OVERVIEW') || {};
  const brands = rows.filter(r => r.ResultType === 'BRANDS');
  const daily = rows.filter(r => r.ResultType === 'DAILY');
  const classification = rows.filter(r => r.ResultType === 'CLASSIFICATION');
  
  // Build analytics object
  const analytics = {
    period: { start, end }, // Add period for PDF generation
    overview: {
      totalInvoices: overview.TotalInvoices || 0,
      totalRevenue: overview.TotalRevenue || 0,
      totalQty: overview.TotalQty || 0,
      basicProfit: overview.BasicProfit || 0,
      basicMargin: overview.BasicMargin || 0
    },
    tyreSummary: {
      totalInvoices: overview.TotalInvoices || 0,
      totalAmount: Math.round(overview.TotalRevenue || 0),
      totalProfit: Math.round(overview.BasicProfit || 0),
      basicProfit: Math.round(overview.BasicProfit || 0),
      totalQuantity: overview.TotalQty || 0,
      profitMargin: (overview.BasicMargin || 0).toFixed(1),
      basicProfitMargin: (overview.BasicMargin || 0).toFixed(1),
      averageInvoiceValue: (overview.TotalInvoices||0) > 0 ? Math.round((overview.TotalRevenue||0)/overview.TotalInvoices) : 0,
      profitCalculationMethod: 'basic-sql'
    },
    dateWiseAnalysis: {},
    brandAnalysis: {},
    topBrand: brands.length > 0 ? brands[0].BrandGuess : 'Unknown',
    validTyreCount: overview.TotalQty || 0,
    diagnostics: {
      mode: 'sql-simplified',
      includedLines: overview.TotalQty || 0,
      classificationBreakdown: {}
    }
  };
  
  // Process daily data and item details
  const details = rows.filter(r => r.ResultType === 'DETAILS');
  daily.forEach(row => {
    if (row.Day) {
      const dateStr = moment(row.Day).format('YYYY-MM-DD');
      const dayItems = details.filter(d => moment(d.Day).format('YYYY-MM-DD') === dateStr);
      
      analytics.dateWiseAnalysis[dateStr] = {
        totalQty: row.TotalQty || 0,
        totalAmount: row.TotalRevenue || 0,
        totalProfit: row.BasicProfit || 0,
        items: dayItems.map(item => ({
          Description: item.Description || '',
          Qty: item.TotalQty || 0,
          UnitPrice: item.TotalQty ? (item.TotalRevenue / item.TotalQty) : 0,
          LineTotal: item.TotalRevenue || 0,
          TyreCategory: item.ClassReason || 'Unknown',
          BrandGuess: item.BrandGuess || 'Unknown'
        }))
      };
    }
  });
  
  // Calculate working days
  const workingDays = Object.keys(analytics.dateWiseAnalysis).length;
  analytics.tyreSummary.workingDays = workingDays;
  analytics.tyreSummary.avgDailyRevenue = workingDays > 0 ? Math.round((overview.TotalRevenue||0)/workingDays) : 0;
  analytics.tyreSummary.avgDailyInvoices = workingDays > 0 && (overview.TotalInvoices||0) ? Math.round(((overview.TotalInvoices||0)/workingDays)*10)/10 : 0;
  
  // Process brand data
  brands.forEach(row => {
    if (row.BrandGuess) {
      analytics.brandAnalysis[row.BrandGuess] = {
        qty: row.TotalQty || 0,
        revenue: row.TotalRevenue || 0,
        profit: 0
      };
    }
  });
  
  // Process classification
  classification.forEach(row => {
    if (row.ClassReason) {
      analytics.diagnostics.classificationBreakdown[row.ClassReason] = row.Count || 0;
    }
  });
  
  return analytics;
}

module.exports = { generateMonthlyTyreReportSQL };