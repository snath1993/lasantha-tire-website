// utils/sqlMonthlyTyreReport.js
// Pure SQL driven monthly tyre report generator to reduce JS-side heuristic filtering
// Exports: generateMonthlyTyreReportSQL(sql, { star  -- Fi  -- Get only overview data and daily analysis
SELECT * FROM overview;
SELECT Day as date, Qty as totalQty, Revenue as totalRevenue, Profit as totalProfit FROM daily ORDER BY Day;ult sets  
SELECT *, 
  CAST((SELECT * FROM brands FOR JSON AUTO) AS NVARCHAR(MAX)) AS BrandsJson,
  CAST((SELECT * FROM daily ORDER BY Day FOR JSON AUTO) AS NVARCHAR(MAX)) AS DailyJson,
  CAST((SELECT * FROM categories FOR JSON AUTO) AS NVARCHAR(MAX)) AS CategoriesJson,
  CAST((SELECT * FROM diagnostics FOR JSON AUTO) AS NVARCHAR(MAX)) AS DiagnosticsJson,
  CAST((SELECT ClassReason, COUNT(*) AS Count FROM base GROUP BY ClassReason FOR JSON AUTO) AS NVARCHAR(MAX)) AS ClassReasonJson
FROM overview;}, options)

const moment = require('moment');

// Service / accessory keyword lists for SQL pattern matching
const SERVICE_KEYWORDS = [
  'ALIGN','BALANC','CHANGE','FITTING','FIT ','ROTATION','ROTATE','CHECK','INSPECT','SERVICE','LABOUR','LABOR','MOUNT','DISMOUNT'
];
const ACCESSORY_KEYWORDS = [ 'TUBE','VALVE','PATCH','RIM','WHEEL','FLAP','LINER','WEIGHT','NUT','GLUE','CEMENT','NECK','STEM' ];
const REBUILD_KEYWORDS = [ 'REBUILD','RETREAD','RECAP','REMOULD','REMOLD','BANDAG','RE-MOULD','RE-TREAD' ];
const BRAND_KEYWORDS = [
  'BRIDGESTONE','MICHELIN','YOKOHAMA','CONTINENTAL','PIRELLI','DUNLOP','GOODYEAR','GOOD YEAR','HANKOOK','KUMHO','TOYO','MAXXIS','MAXXIES','BFGOODRICH','BFG','FALKEN','NITTO','COOPER','DURATURN','GOODRIDE','GT','NEXEN','CEAT','APOLLO','LINGLONG','HIFLY','KAPSEN'
];

function buildLikeDisjunction(alias, list) {
  return list.map(k => `UPPER(${alias}.Description) LIKE '%${k.toUpperCase()}%'`).join(' OR ');
}

// Basic size detection patterns (cannot use full regex in T-SQL easily)
// We'll approximate with numeric/ slash combos length heuristics.
const SIZE_PATTERNS = [
  '%/%/%',   // 185/65/15
  '%R1%',    // 65R15 etc (broad)
  '%-%-%'    // 6.00-16 or 6-90-15 simplified
];

function buildSizePattern(alias) {
  return '(' + SIZE_PATTERNS.map(p=> `${alias}.Description LIKE '${p}'`).join(' OR ') + ')';
}

async function generateMonthlyTyreReportSQL(sql, range, options = {}) {
  const { start, end } = range;
  const strict = options.strict || false; // if strict, require size OR category not service
  
  // Category filtering options
  const categoryFilterEnabled = process.env.MONTHLY_CATEGORY_FILTER === '1';
  const allowedCategories = process.env.MONTHLY_ALLOWED_CATEGORIES ? 
    process.env.MONTHLY_ALLOWED_CATEGORIES.split(',').map(s=>s.trim().toUpperCase()).filter(Boolean) : [];
  const excludedCategories = process.env.MONTHLY_EXCLUDED_CATEGORIES ? 
    process.env.MONTHLY_EXCLUDED_CATEGORIES.split(',').map(s=>s.trim().toUpperCase()).filter(Boolean) : [];

  // Build dynamic CASE logic
  const serviceCondition = '(' + buildLikeDisjunction('si', SERVICE_KEYWORDS) + ')';
  const accessoryCondition = '(' + buildLikeDisjunction('si', ACCESSORY_KEYWORDS) + ')';
  const rebuildCondition = '(' + buildLikeDisjunction('si', REBUILD_KEYWORDS) + ')';
  const sizeCond = buildSizePattern('si');
  const brandCond = '(' + BRAND_KEYWORDS.map(b => `UPPER(si.Description) LIKE '%${b}%'`).join(' OR ') + ')';

  const tyreWordCond = "(UPPER(si.Description) LIKE '%TYRE%' OR UPPER(si.Description) LIKE '%TIRE%')";

  // Revised isTyre classification with database category priority
  const isTyreExpr = `CASE
   -- Priority 1: Exclude clear accessories/services
   WHEN ${accessoryCondition} OR ${rebuildCondition} THEN 0
   -- Priority 2: Exclude KATTA, DAG, and REBUILD categories
   WHEN im.Categoty IN ('KATTA TYRES', 'DAG TYRES', 'REBUILD TYRES', 'DAG CHARGES', 'REBUILD CHARGES', 'RADIAL DAG TYRES', 'RADIAL DAG CHARGERS', 'SAVIYA TYRES', 'SAVIYA CHARGERS') THEN 0
   -- Priority 3: Main category-based inclusion (most reliable) - only new tyres
   WHEN im.Categoty = 'TYRES' THEN 1
   -- Priority 4: Exclude service categories
   WHEN im.Categoty IN ('SERVICES', 'TUBES', 'ALIGNMENT', 'WHEEL ALIGNMENT') THEN 0
   -- Priority 5: Brand-based inclusion (Custom3 has brand names)
   WHEN im.Custom3 IS NOT NULL AND im.Custom3 NOT IN ('-', '_', '') 
     AND im.Custom3 NOT LIKE '%TUBE%' AND im.Custom3 NOT LIKE '%VALVE%' 
     AND im.Custom3 NOT LIKE '%SERVICE%' AND im.Custom3 NOT LIKE '%REPAIR%' THEN 1
   -- Priority 6: Size + brand pattern (for items without proper category)
   WHEN (${sizeCond} AND ${brandCond}) THEN 1
   -- Priority 7: Exclude pure service lines
   WHEN ${serviceCondition} AND NOT (${sizeCond} OR ${brandCond} OR ${tyreWordCond}) THEN 0
   -- Priority 8: Fallback to tyre word or size
   WHEN ${tyreWordCond} THEN 1
   WHEN ${sizeCond} THEN 1
   ELSE 0 END`;

  const sizeRequiredExtra = strict ? ` AND (${sizeCond} OR ${tyreWordCond})` : '';
  
  // Category filtering SQL conditions
  let categoryFilterSQL = '';
  if (categoryFilterEnabled) {
    if (excludedCategories.length > 0) {
      categoryFilterSQL += ` AND NOT (${excludedCategories.map(cat => `UPPER(im.Custom3) LIKE '%${cat}%' OR UPPER(im.Categoty) LIKE '%${cat}%'`).join(' OR ')})`;
    }
    if (allowedCategories.length > 0) {
      categoryFilterSQL += ` AND (${allowedCategories.map(cat => `UPPER(im.Custom3) LIKE '%${cat}%' OR UPPER(im.Categoty) LIKE '%${cat}%'`).join(' OR ')})`;
    }
  }

  // Use CTE approach instead of temp table for better compatibility
  const brandCase = `CASE 
    ${BRAND_KEYWORDS.map(b=>`WHEN UPPER(Description) LIKE '%${b}%' THEN '${b.replace(/'/g,"''")}'`).join('\n    ')}
    ELSE 'Unknown' END`;
    
  // Single comprehensive query with multiple CTEs
  const query = `
WITH base AS (
SELECT 
  si.InvoiceNo,
  si.InvoiceDate,
  si.Description,
  si.Qty,
  si.UnitPrice,
  ISNULL(si.CostPrrice,0) AS CostPrrice,
  (si.Qty * si.UnitPrice) AS LineTotal,
  CASE WHEN ${serviceCondition} THEN 1 ELSE 0 END AS ServiceFlag,
  CASE WHEN ${accessoryCondition} THEN 1 ELSE 0 END AS AccessoryFlag,
  CASE WHEN ${rebuildCondition} THEN 1 ELSE 0 END AS RebuildFlag,
  CASE WHEN ${sizeCond} THEN 1 ELSE 0 END AS SizeFlag,
  CASE WHEN ${brandCond} THEN 1 ELSE 0 END AS BrandFlag,
  ${isTyreExpr} AS IsTyre,
  /* classification reason for diagnostics */
  CASE 
    WHEN (${accessoryCondition} OR ${rebuildCondition}) THEN 'excluded-accessory'
    WHEN im.Categoty IN ('KATTA TYRES', 'DAG TYRES', 'REBUILD TYRES', 'DAG CHARGES', 'REBUILD CHARGES', 'RADIAL DAG TYRES', 'RADIAL DAG CHARGERS', 'SAVIYA TYRES', 'SAVIYA CHARGERS') THEN 'excluded-katta-dag-rebuild'
    WHEN im.Categoty = 'TYRES' THEN 'main-category-tyres'
    WHEN im.Categoty IN ('SERVICES', 'TUBES', 'ALIGNMENT', 'WHEEL ALIGNMENT') THEN 'excluded-service-category'
    WHEN im.Custom3 IS NOT NULL AND im.Custom3 NOT IN ('-', '_', '') 
         AND im.Custom3 NOT LIKE '%TUBE%' AND im.Custom3 NOT LIKE '%VALVE%' 
         AND im.Custom3 NOT LIKE '%SERVICE%' AND im.Custom3 NOT LIKE '%REPAIR%' THEN 'brand-category'
    WHEN (${sizeCond} AND ${brandCond}) THEN 'size+brand'
    WHEN ${serviceCondition} AND NOT (${sizeCond} OR ${brandCond} OR ${tyreWordCond}) THEN 'excluded-service'
    WHEN ${tyreWordCond} THEN 'tyre-word'
    WHEN ${sizeCond} THEN 'size-only'
    ELSE 'other' END AS ClassReason,
  ISNULL(im.Custom3,'Unknown') AS TyreCategory,
  ISNULL(im.Categoty,'Unknown') AS MainCategory
FROM tblSalesInvoices si
LEFT JOIN tblItemMaster im ON si.Description = im.ItemDescription
  AND im.Categoty = 'TYRES'
  AND im.UnitCost > 0
WHERE si.InvoiceDate >= @start AND si.InvoiceDate <= @end
  AND si.Qty > 0
  ${sizeRequiredExtra}${categoryFilterSQL}
),
overview AS (
SELECT 
  COUNT(DISTINCT InvoiceNo) AS TotalInvoices,
  SUM(LineTotal) AS TotalRevenue,
  SUM(Qty) AS TotalQty,
  SUM((LineTotal - (Qty*CostPrrice))) AS DailyProfit,
  SUM(CASE WHEN CostPrrice>0 THEN (LineTotal - (Qty*CostPrrice)) ELSE 0 END) * 1.0 / NULLIF(SUM(LineTotal),0) * 100 AS BasicMargin
FROM base WHERE IsTyre=1
),
brands AS (
SELECT TOP 50 ${brandCase} AS BrandGuess, SUM(Qty) AS Qty, SUM(LineTotal) AS Revenue
FROM base WHERE IsTyre=1
GROUP BY ${brandCase}
),
daily AS (
SELECT CONVERT(date, InvoiceDate) AS Day, SUM(Qty) AS Qty, SUM(LineTotal) AS Revenue,
  SUM(CASE WHEN CostPrrice>0 THEN (LineTotal - (Qty*CostPrrice)) ELSE 0 END) AS Profit
FROM base WHERE IsTyre=1 GROUP BY CONVERT(date, InvoiceDate)
),
categories AS (
SELECT TyreCategory, SUM(Qty) AS Qty, SUM(LineTotal) AS Revenue,
  SUM(CASE WHEN CostPrrice>0 THEN (LineTotal - (Qty*CostPrrice)) ELSE 0 END) AS Profit
FROM base WHERE IsTyre=1 GROUP BY TyreCategory
),
diagnostics AS (
SELECT COUNT(*) AS BaseLines,
       SUM(CASE WHEN ServiceFlag=1 THEN 1 ELSE 0 END) AS ServiceLines,
       SUM(CASE WHEN AccessoryFlag=1 THEN 1 ELSE 0 END) AS AccessoryLines,
       SUM(CASE WHEN RebuildFlag=1 THEN 1 ELSE 0 END) AS RebuildLines,
       SUM(CASE WHEN SizeFlag=1 THEN 1 ELSE 0 END) AS SizeLines,
       SUM(CASE WHEN BrandFlag=1 THEN 1 ELSE 0 END) AS BrandLines,
  SUM(CASE WHEN IsTyre=1 THEN 1 ELSE 0 END) AS IncludedTyreLines,
  SUM(CASE WHEN IsTyre=1 AND CostPrrice>0 THEN 1 ELSE 0 END) AS CostCoveredLines,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='size+brand' THEN 1 ELSE 0 END) AS ReasonSizeBrand,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='size-only' THEN 1 ELSE 0 END) AS ReasonSizeOnly,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='brand-only' THEN 1 ELSE 0 END) AS ReasonBrandOnly,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='tyre-word' THEN 1 ELSE 0 END) AS ReasonTyreWord,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='main-category-tyres' THEN 1 ELSE 0 END) AS ReasonMainCategory,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='excluded-katta-dag-rebuild' THEN 1 ELSE 0 END) AS ReasonKattaExcluded,
  SUM(CASE WHEN IsTyre=1 AND ClassReason='brand-category' THEN 1 ELSE 0 END) AS ReasonBrandCategory
FROM base
)
-- Final result sets  
SELECT * FROM overview;
SELECT * FROM brands ORDER BY Qty DESC;
SELECT * FROM daily ORDER BY Day;
SELECT * FROM categories ORDER BY Qty DESC;
SELECT * FROM diagnostics;
SELECT ClassReason, COUNT(*) AS Count FROM base GROUP BY ClassReason;`;

  const request = new sql.Request();
  request.input('start', start);
  request.input('end', end);
  const result = await request.batch(query);

  // mssql batch returns recordsets array - updated for CTE structure
  const [aggRS, brandRS, dailyRS, catRS, diagRS, classRS] = result.recordsets;
  const agg = aggRS && aggRS[0] ? aggRS[0] : { TotalInvoices:0, TotalRevenue:0, TotalQty:0, BasicProfit:0, BasicMargin:0 };

  // Transform for compatibility with existing formatter
  const dateWiseAnalysis = {};
  dailyRS.forEach(r => {
    dateWiseAnalysis[moment(r.Day).format('YYYY-MM-DD')] = {
      totalQty: r.Qty || 0,
      totalRevenue: r.Revenue || 0,
      totalProfit: r.Profit || 0,
      totalAdvancedProfit: r.Profit || 0,
      invoiceCount: null, // unknown without extra query
      items: [] // can be populated if needed from tyresRS filtered by date
    };
  });

  const brandAnalysis = {};
  function normalizeBrand(name) {
    if (!name) return 'Unknown';
    const upper = name.toUpperCase();
    if (upper === 'GOOD YEAR') return 'GOODYEAR';
    if (upper === 'MAXXIES') return 'MAXXIS';
    return name;
  }
  brandRS.forEach(b => {
    const raw = (b.BrandGuess || 'Unknown') === '' ? 'Unknown' : (b.BrandGuess || 'Unknown');
    const brand = normalizeBrand(raw);
    if (!brandAnalysis[brand]) brandAnalysis[brand] = { qty: 0, revenue: 0, invoiceCount: null };
    brandAnalysis[brand].qty += b.Qty || 0;
    brandAnalysis[brand].revenue += b.Revenue || 0;
  });

  const categoryAnalysis = {};
  catRS.forEach(c => {
    const catName = c.TyreCategory || 'Unknown';
    categoryAnalysis[catName] = {
      qty: c.Qty || 0,
      revenue: c.Revenue || 0,
      profit: c.Profit || 0,
      itemCount: null,
      invoiceCount: null,
      avgPrice: c.Qty ? Math.round(c.Revenue / c.Qty) : 0,
      profitMargin: c.Revenue ? ((c.Profit / c.Revenue) * 100).toFixed(1) : '0.0'
    };
  });

  const workingDays = Object.keys(dateWiseAnalysis).length;

  const diag = diagRS && diagRS[0] ? diagRS[0] : {};
  const analytics = {
    period: { start, end },
    tyreSummary: {
      totalInvoices: agg.TotalInvoices || 0,
      totalAmount: Math.round(agg.TotalRevenue || 0),
      totalProfit: Math.round(agg.BasicProfit || 0),
      basicProfit: Math.round(agg.BasicProfit || 0),
      advancedProfit: undefined,
      totalQuantity: agg.TotalQty || 0,
      profitMargin: agg.BasicMargin ? agg.BasicMargin.toFixed ? agg.BasicMargin.toFixed(1) : Number(agg.BasicMargin).toFixed(1) : '0.0',
      basicProfitMargin: agg.BasicMargin ? agg.BasicMargin.toFixed ? agg.BasicMargin.toFixed(1) : Number(agg.BasicMargin).toFixed(1) : '0.0',
      averageInvoiceValue: (agg.TotalInvoices||0) > 0 ? Math.round((agg.TotalRevenue||0)/agg.TotalInvoices) : 0,
      workingDays,
      avgDailyRevenue: workingDays>0 ? Math.round((agg.TotalRevenue||0)/workingDays) : 0,
      avgDailyInvoices: workingDays>0 && (agg.TotalInvoices||0) ? Math.round(((agg.TotalInvoices||0)/workingDays)*10)/10 : 0,
      profitCalculationMethod: 'basic-sql'
    },
    dateWiseAnalysis,
    brandAnalysis,
    categoryAnalysis,
    topBrand: Object.keys(brandAnalysis).sort((a,b)=> (brandAnalysis[b].qty - brandAnalysis[a].qty))[0] || 'Unknown',
    topCategory: Object.keys(categoryAnalysis).sort((a,b)=> (categoryAnalysis[b].qty - categoryAnalysis[a].qty))[0] || 'Unknown',
    validTyreCount: (dailyRS || []).reduce((sum, day) => sum + (day.Qty || 0), 0),
    generatedAt: new Date().toISOString(),
    profitFeatures: {
      advancedProfitEnabled: false,
      cacheSize: 0,
      negativeProfitLines: 0,
      advancedGap: 0
    },
    metrics: {
      mode: 'sql',
      strict,
      rawLines: tyresRS ? tyresRS.length : 0,
      baseLines: diag.BaseLines || 0,
      serviceLines: diag.ServiceLines || 0,
      accessoryLines: diag.AccessoryLines || 0,
      rebuildLines: diag.RebuildLines || 0,
      sizeLines: diag.SizeLines || 0,
      brandLines: diag.BrandLines || 0,
      includedTyreLines: diag.IncludedTyreLines || 0,
      costCoveredLines: diag.CostCoveredLines || 0,
      reasonSizeBrand: diag.ReasonSizeBrand || 0,
      reasonSizeOnly: diag.ReasonSizeOnly || 0,
      reasonBrandOnly: diag.ReasonBrandOnly || 0,
      reasonTyreWord: diag.ReasonTyreWord || 0,
      reasonMainCategory: diag.ReasonMainCategory || 0,
      reasonKattaExcluded: diag.ReasonKattaExcluded || 0,
      reasonBrandCategory: diag.ReasonBrandCategory || 0,
      categoryFilterEnabled,
      allowedCategories: allowedCategories.join(','),
      excludedCategories: excludedCategories.join(','),
      classificationBreakdown: (classRS || []).reduce((acc, row) => {
        if (row.ClassReason) acc[row.ClassReason] = row.Count;
        return acc;
      }, {})
    },
    rawTyreLines: [] // Not included in CTE approach for performance
  };

  return analytics;
}

module.exports = { generateMonthlyTyreReportSQL };
