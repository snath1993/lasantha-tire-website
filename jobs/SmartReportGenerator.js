/**
 * ═══════════════════════════════════════════════════════════════════════════════
 * 🚀 SMART REPORT GENERATOR v2.1
 * ═══════════════════════════════════════════════════════════════════════════════
 * Professional Business Intelligence Report System for Lasantha Tyre Traders
 * 
 * Features:
 * - Zero AI dependency for standard reports (instant generation)
 * - Professional PDF reports with company branding
 * - Excel reports with formatted tables and charts
 * - Sinhala & English command support
 * - Category, Brand, Date filtering
 * - Month-wise breakdowns with totals
 * - Comparison reports (YoY, MoM)
 * - Interactive Menu System (titan report → number selection)
 * ═══════════════════════════════════════════════════════════════════════════════
 */

const fs = require('fs');
const path = require('path');
const PDFDocument = require('pdfkit');
const ExcelJS = require('exceljs');
const { executeSafely } = require('../utils/readOnlySafeDb');
const moment = require('moment');

// ═══════════════════════════════════════════════════════════════════════════════
// COMPANY BRANDING
// ═══════════════════════════════════════════════════════════════════════════════
const COMPANY = {
    name: 'Lasantha Tyre Traders',
    nameSinhala: 'ලසන්ත ටයර් ට්‍රේඩර්ස්',
    address: 'Panadura, Sri Lanka',
    phone: '038-2232922',
    mobile: '077-7078700',
    established: 1985,
    tagline: 'Quality Tyres Since 1985'
};

// ═══════════════════════════════════════════════════════════════════════════════
// INTERACTIVE REPORT MENU SYSTEM
// ═══════════════════════════════════════════════════════════════════════════════
const REPORT_MENU = {
    // Report definitions with numbers
    items: [
        { 
            id: 1, 
            key: 'yearlySales2025', 
            label: '2025 Sales Report (මේ අවුරුද්දේ විකුණුම්)', 
            emoji: '📊',
            reportType: 'yearlySales',
            params: { year: 2025, format: 'pdf' }
        },
        { 
            id: 2, 
            key: 'yearlySales2024', 
            label: '2024 Sales Report (පසුගිය අවුරුද්දේ විකුණුම්)', 
            emoji: '📈',
            reportType: 'yearlySales',
            params: { year: 2024, format: 'pdf' }
        },
        { 
            id: 3, 
            key: 'tyreSales', 
            label: 'Tyre Sales Report (ටයර් විකුණුම්)', 
            emoji: '🛞',
            reportType: 'tyreSales',
            params: { year: new Date().getFullYear(), format: 'pdf' }
        },
        { 
            id: 4, 
            key: 'categorySales', 
            label: 'Category Sales Report (කාණ්ඩ අනුව විකුණුම්)', 
            emoji: '📦',
            reportType: 'categorySales',
            params: { year: new Date().getFullYear(), format: 'pdf' }
        },
        { 
            id: 5, 
            key: 'brandSales', 
            label: 'Brand Sales Report (බ්‍රෑන්ඩ් අනුව විකුණුම්)', 
            emoji: '🏷️',
            reportType: 'brandSales',
            params: { year: new Date().getFullYear(), format: 'pdf' }
        },
        { 
            id: 6, 
            key: 'topSellers', 
            label: 'Top Selling Items (වැඩිපුරම විකුණෙන බඩු)', 
            emoji: '🏆',
            reportType: 'topSellers',
            params: { year: new Date().getFullYear(), format: 'pdf' }
        },
        { 
            id: 7, 
            key: 'lowStock', 
            label: 'Low Stock Alert (තොග අඩු බඩු)', 
            emoji: '⚠️',
            reportType: 'lowStock',
            params: { format: 'pdf' }
        },
        { 
            id: 8, 
            key: 'yearlySales2025Excel', 
            label: '2025 Sales Report EXCEL', 
            emoji: '📗',
            reportType: 'yearlySales',
            params: { year: 2025, format: 'excel' }
        },
        { 
            id: 9, 
            key: 'yearlySales2024Excel', 
            label: '2024 Sales Report EXCEL', 
            emoji: '📗',
            reportType: 'yearlySales',
            params: { year: 2024, format: 'excel' }
        },
        { 
            id: 10, 
            key: 'tyreSalesExcel', 
            label: 'Tyre Sales Report EXCEL', 
            emoji: '📗',
            reportType: 'tyreSales',
            params: { year: new Date().getFullYear(), format: 'excel' }
        }
    ],
    
    // Generate the menu message
    getMenuMessage: function() {
        const exampleYear = this.items.find(i => i.id === 1)?.params?.year || new Date().getFullYear();
        let msg = `📊 *TITAN REPORT MENU*\n`;
        msg += `━━━━━━━━━━━━━━━━━━━━━━━━━\n\n`;
        msg += `අවශ්‍ය report එකේ අංකය reply කරන්න:\n\n`;
        
        // PDF Reports section
        msg += `📄 *PDF Reports:*\n`;
        this.items.filter(i => i.params.format === 'pdf').forEach(item => {
            msg += `${item.id}️⃣ ${item.emoji} ${item.label}\n`;
        });
        
        msg += `\n📗 *Excel Reports:*\n`;
        this.items.filter(i => i.params.format === 'excel').forEach(item => {
            msg += `${item.id}️⃣ ${item.emoji} ${item.label}\n`;
        });
        
        msg += `\n━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        msg += `💡 _Example: Reply *1* for ${exampleYear} Sales PDF_`;
        
        return msg;
    },
    
    // Get report by number
    getReportById: function(id) {
        const numId = parseInt(id);
        return this.items.find(item => item.id === numId) || null;
    }
};

// User session storage for menu selections (expires after 5 minutes)
const userMenuSessions = new Map();
const MENU_SESSION_TIMEOUT = 5 * 60 * 1000; // 5 minutes

// Clean old sessions periodically
setInterval(() => {
    const now = Date.now();
    for (const [key, session] of userMenuSessions.entries()) {
        if (now - session.timestamp > MENU_SESSION_TIMEOUT) {
            userMenuSessions.delete(key);
        }
    }
}, 60 * 1000); // Check every minute

// ═══════════════════════════════════════════════════════════════════════════════
// REPORT OUTPUT DIRECTORY
// ═══════════════════════════════════════════════════════════════════════════════
const REPORTS_DIR = path.join(__dirname, '..', 'reports', 'generated');
if (!fs.existsSync(REPORTS_DIR)) {
    fs.mkdirSync(REPORTS_DIR, { recursive: true });
}

// ═══════════════════════════════════════════════════════════════════════════════
// COMMAND PATTERNS (Sinhala + English)
// ═══════════════════════════════════════════════════════════════════════════════
const REPORT_PATTERNS = {
    // Sales Reports
    dailySales: /^(daily\s*report|ada\s*sales|today\s*sales|අද\s*විකුණුම්)$/i,
    weeklySales: /^(weekly\s*report|sathi\s*sales|මේ\s*සතිය)$/i,
    monthlySales: /^(monthly\s*report|masika\s*report|මේ\s*මාසේ|this\s*month)$/i,
    yearlySales: /^(yearly\s*report|aurudde\s*report|මේ\s*අවුරුද්දේ|this\s*year)$/i,
    
    // Specific Year Reports
    yearReport: /(?:giya\s*aurudde|past\s*year|(\d{4})\s*sales|(\d{4})\s*report|(\d{4})\s*aurudde)/i,
    
    // Category Reports
    categoryReport: /(?:category|catagory|categoty)\s*(sales|report)?/i,
    tyreReport: /^(tyre\s*report|tyre\s*sales|ටයර්\s*විකුණුම්)$/i,
    
    // Stock Reports
    stockReport: /^(stock\s*report|stock\s*eka|තොග\s*වාර්තාව)$/i,
    lowStock: /^(low\s*stock|stock\s*adu|තොග\s*අඩු)$/i,
    
    // Brand Reports
    brandReport: /^(brand\s*report|brand\s*sales)/i,
    specificBrand: /(ceat|maxxis|dsi|goodride|continental|michelin|bridgestone|apollo|mrf|dunlop|yokohama|hankook|kumho)/i,
    
    // Top Performers
    topSellers: /^(top\s*sellers?|honda\s*yanne|best\s*selling|විකුණෙන\s*බඩු)$/i,
    topCustomers: /^(top\s*customers?|honda\s*customers?|ලොකු\s*කස්ටමර්)$/i,
    
    // Comparison
    compareYears: /compare\s*(\d{4})\s*(?:vs|and|with|saha)?\s*(\d{4})/i,
    compareMonths: /compare\s*months?|month\s*comparison|masa\s*compare/i,
};

// ═══════════════════════════════════════════════════════════════════════════════
// UTILITY FUNCTIONS
// ═══════════════════════════════════════════════════════════════════════════════

const formatCurrency = (amount) => {
    if (!amount || isNaN(amount)) return 'Rs. 0';
    return 'Rs. ' + parseFloat(amount).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

const formatNumber = (num) => {
    if (!num || isNaN(num)) return '0';
    return parseFloat(num).toLocaleString('en-US');
};

const getMonthName = (monthNum) => {
    const months = ['January', 'February', 'March', 'April', 'May', 'June', 
                    'July', 'August', 'September', 'October', 'November', 'December'];
    return months[monthNum - 1] || 'Unknown';
};

const getMonthNameSinhala = (monthNum) => {
    const months = ['ජනවාරි', 'පෙබරවාරි', 'මාර්තු', 'අප්‍රේල්', 'මැයි', 'ජූනි', 
                    'ජූලි', 'අගෝස්තු', 'සැප්තැම්බර්', 'ඔක්තෝබර්', 'නොවැම්බර්', 'දෙසැම්බර්'];
    return months[monthNum - 1] || 'Unknown';
};

// ═══════════════════════════════════════════════════════════════════════════════
// SQL QUERY TEMPLATES (Optimized for MSSQL)
// ═══════════════════════════════════════════════════════════════════════════════

const SQL_QUERIES = {
    // Yearly sales by month (with category filter option)
    yearlySalesByMonth: (year, category = null) => `
        SELECT 
            MONTH(Expr1) AS SaleMonth,
            COUNT(DISTINCT Expr2) AS InvoiceCount,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalAmount
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
            ${category ? `AND Categoty LIKE '%${category}%'` : ''}
        GROUP BY MONTH(Expr1)
        ORDER BY MONTH(Expr1)
    `,

    // Yearly sales by category
    yearlySalesByCategory: (year) => `
        SELECT 
            Categoty AS Category,
            COUNT(DISTINCT Expr2) AS InvoiceCount,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalAmount
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
        GROUP BY Categoty
        ORDER BY SUM(Expr12) DESC
    `,

    // Monthly category breakdown for a year
    yearlyMonthlyCategoryBreakdown: (year) => `
        SELECT 
            MONTH(Expr1) AS SaleMonth,
            Categoty AS Category,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalAmount
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
        GROUP BY MONTH(Expr1), Categoty
        ORDER BY MONTH(Expr1), SUM(Expr12) DESC
    `,

    // Daily sales summary
    dailySales: (date = 'GETDATE()') => `
        SELECT 
            COUNT(DISTINCT Expr2) AS InvoiceCount,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalAmount,
            COUNT(DISTINCT Expr10) AS UniqueCustomers
        FROM [View_Sales report whatsapp]
        WHERE CAST(Expr1 AS DATE) = CAST(${date} AS DATE)
            AND Expr14 = 0
    `,

    // Top selling items
    topSellingItems: (year, limit = 20) => `
        SELECT TOP ${limit}
            Expr4 AS ItemName,
            Categoty AS Category,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalRevenue,
            AVG(Expr6) AS AvgUnitPrice
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
        GROUP BY Expr4, Categoty
        ORDER BY SUM(Expr12) DESC
    `,

    // Brand-wise sales
    brandSales: (year) => `
        SELECT 
            CASE 
                WHEN Expr4 LIKE '%CEAT%' THEN 'CEAT'
                WHEN Expr4 LIKE '%MAXXIS%' THEN 'MAXXIS'
                WHEN Expr4 LIKE '%DSI%' THEN 'DSI'
                WHEN Expr4 LIKE '%GOODRIDE%' THEN 'GOODRIDE'
                WHEN Expr4 LIKE '%CONTINENTAL%' THEN 'CONTINENTAL'
                WHEN Expr4 LIKE '%MICHELIN%' THEN 'MICHELIN'
                WHEN Expr4 LIKE '%BRIDGESTONE%' THEN 'BRIDGESTONE'
                WHEN Expr4 LIKE '%APOLLO%' THEN 'APOLLO'
                WHEN Expr4 LIKE '%MRF%' THEN 'MRF'
                WHEN Expr4 LIKE '%DUNLOP%' THEN 'DUNLOP'
                WHEN Expr4 LIKE '%YOKOHAMA%' THEN 'YOKOHAMA'
                WHEN Expr4 LIKE '%HANKOOK%' THEN 'HANKOOK'
                WHEN Expr4 LIKE '%KUMHO%' THEN 'KUMHO'
                ELSE 'OTHER'
            END AS Brand,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalRevenue
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
            AND Categoty LIKE '%TYRE%'
        GROUP BY 
            CASE 
                WHEN Expr4 LIKE '%CEAT%' THEN 'CEAT'
                WHEN Expr4 LIKE '%MAXXIS%' THEN 'MAXXIS'
                WHEN Expr4 LIKE '%DSI%' THEN 'DSI'
                WHEN Expr4 LIKE '%GOODRIDE%' THEN 'GOODRIDE'
                WHEN Expr4 LIKE '%CONTINENTAL%' THEN 'CONTINENTAL'
                WHEN Expr4 LIKE '%MICHELIN%' THEN 'MICHELIN'
                WHEN Expr4 LIKE '%BRIDGESTONE%' THEN 'BRIDGESTONE'
                WHEN Expr4 LIKE '%APOLLO%' THEN 'APOLLO'
                WHEN Expr4 LIKE '%MRF%' THEN 'MRF'
                WHEN Expr4 LIKE '%DUNLOP%' THEN 'DUNLOP'
                WHEN Expr4 LIKE '%YOKOHAMA%' THEN 'YOKOHAMA'
                WHEN Expr4 LIKE '%HANKOOK%' THEN 'HANKOOK'
                WHEN Expr4 LIKE '%KUMHO%' THEN 'KUMHO'
                ELSE 'OTHER'
            END
        ORDER BY SUM(Expr12) DESC
    `,

    // Low stock items
    lowStock: (threshold = 4) => `
        SELECT 
            Description AS ItemName,
            Category,
            Quantity AS CurrentStock,
            SellingPrice
        FROM [View_ItemWhse]
        WHERE Quantity < ${threshold}
            AND Quantity > 0
        ORDER BY Quantity ASC
    `,

    // Year comparison
    yearComparison: (year1, year2) => `
        SELECT 
            y1.Category,
            y1.TotalAmount AS Year1Amount,
            y2.TotalAmount AS Year2Amount,
            y1.TotalQty AS Year1Qty,
            y2.TotalQty AS Year2Qty
        FROM (
            SELECT Categoty AS Category, SUM(Expr12) AS TotalAmount, SUM(Expr5) AS TotalQty
            FROM [View_Sales report whatsapp]
            WHERE YEAR(Expr1) = ${year1} AND Expr14 = 0
            GROUP BY Categoty
        ) y1
        FULL OUTER JOIN (
            SELECT Categoty AS Category, SUM(Expr12) AS TotalAmount, SUM(Expr5) AS TotalQty
            FROM [View_Sales report whatsapp]
            WHERE YEAR(Expr1) = ${year2} AND Expr14 = 0
            GROUP BY Categoty
        ) y2 ON y1.Category = y2.Category
        ORDER BY COALESCE(y1.TotalAmount, 0) + COALESCE(y2.TotalAmount, 0) DESC
    `,

    // Tyre-only sales by month
    tyreSalesByMonth: (year) => `
        SELECT 
            MONTH(Expr1) AS SaleMonth,
            SUM(Expr5) AS TotalQty,
            SUM(Expr12) AS TotalAmount
        FROM [View_Sales report whatsapp]
        WHERE YEAR(Expr1) = ${year}
            AND Expr14 = 0
            AND (Categoty LIKE '%TYRE%' OR Categoty LIKE '%KATTA%')
            AND Categoty NOT LIKE '%TUBE%'
        GROUP BY MONTH(Expr1)
        ORDER BY MONTH(Expr1)
    `,

    // Detailed yearly monthly summary with profit (joins to item master for UnitCost)
    yearlySalesMonthlyProfit: (year, category = null) => `
        SELECT
            MONTH(d.Expr1) AS SaleMonth,
            COUNT(DISTINCT d.Expr2) AS InvoiceCount,
            SUM(d.Expr5) AS TotalQty,
            SUM(d.Expr12) AS TotalRevenue,
            SUM(ISNULL(im.UnitCost, 0) * d.Expr5) AS TotalCost,
            SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) AS TotalProfit,
            CASE WHEN SUM(d.Expr12) > 0
                THEN (SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) * 100.0 / SUM(d.Expr12))
                ELSE 0
            END AS ProfitMargin
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            ${category ? `AND d.Categoty LIKE '%${category}%'` : ''}
        GROUP BY MONTH(d.Expr1)
        ORDER BY MONTH(d.Expr1)
    `,

    // Yearly totals by category with profit
    yearlySalesByCategoryWithProfit: (year, category = null) => `
        SELECT
            d.Categoty AS Category,
            COUNT(DISTINCT d.Expr2) AS InvoiceCount,
            SUM(d.Expr5) AS TotalQty,
            SUM(d.Expr12) AS TotalRevenue,
            SUM(ISNULL(im.UnitCost, 0) * d.Expr5) AS TotalCost,
            SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) AS TotalProfit,
            CASE WHEN SUM(d.Expr12) > 0
                THEN (SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) * 100.0 / SUM(d.Expr12))
                ELSE 0
            END AS ProfitMargin
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            ${category ? `AND d.Categoty LIKE '%${category}%'` : ''}
        GROUP BY d.Categoty
        ORDER BY SUM(d.Expr12) DESC
    `,

    // Month x Category breakdown with profit
    yearlyMonthlyCategoryWithProfit: (year, category = null) => `
        SELECT
            MONTH(d.Expr1) AS SaleMonth,
            d.Categoty AS Category,
            SUM(d.Expr5) AS TotalQty,
            SUM(d.Expr12) AS TotalRevenue,
            SUM(ISNULL(im.UnitCost, 0) * d.Expr5) AS TotalCost,
            SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) AS TotalProfit
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            ${category ? `AND d.Categoty LIKE '%${category}%'` : ''}
        GROUP BY MONTH(d.Expr1), d.Categoty
        ORDER BY MONTH(d.Expr1), SUM(d.Expr12) DESC
    `,

    // Month x Brand breakdown (uses Custom3 from item master; best for tyres)
    yearlyMonthlyBrandWithProfit: (year, categoryLike = '%TYRE%') => `
        SELECT
            MONTH(d.Expr1) AS SaleMonth,
            ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown') AS Brand,
            SUM(d.Expr5) AS TotalQty,
            SUM(d.Expr12) AS TotalRevenue,
            SUM(ISNULL(im.UnitCost, 0) * d.Expr5) AS TotalCost,
            SUM(d.Expr12 - (ISNULL(im.UnitCost, 0) * d.Expr5)) AS TotalProfit
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            AND d.Categoty LIKE '${categoryLike}'
        GROUP BY MONTH(d.Expr1), ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown')
        ORDER BY MONTH(d.Expr1), SUM(d.Expr12) DESC
    `,

    // Aggregated item list (month/category/brand/item) with profit
    yearlyItemAggregatesWithProfit: (year, category = null) => `
        SELECT
            MONTH(d.Expr1) AS SaleMonth,
            d.Categoty AS Category,
            ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown') AS Brand,
            d.Expr3 AS ItemCode,
            d.Expr4 AS ItemName,
            SUM(d.Expr5) AS TotalQty,
            AVG(d.Expr6) AS AvgUnitPrice,
            SUM(d.Expr12) AS TotalRevenue,
            SUM(ISNULL(d.UnitCost, ISNULL(im.UnitCost, 0)) * d.Expr5) AS TotalCost,
            SUM(d.Expr12 - (ISNULL(d.UnitCost, ISNULL(im.UnitCost, 0)) * d.Expr5)) AS TotalProfit,
            CASE WHEN SUM(d.Expr12) > 0
                THEN (SUM(d.Expr12 - (ISNULL(d.UnitCost, ISNULL(im.UnitCost, 0)) * d.Expr5)) * 100.0 / SUM(d.Expr12))
                ELSE 0
            END AS ProfitMargin
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            ${category ? `AND d.Categoty LIKE '%${category}%'` : ''}
        GROUP BY MONTH(d.Expr1), d.Categoty, ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown'), d.Expr3, d.Expr4
        ORDER BY MONTH(d.Expr1), SUM(d.Expr12) DESC
    `,

    // Detailed Sales Log (Transaction Level)
    yearlySalesLogWithProfit: (year, category = null) => `
        SELECT
            d.Expr1 AS SaleDate,
            MONTH(d.Expr1) AS SaleMonth,
            ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Others') AS Brand,
            d.Categoty AS Category,
            d.Expr4 AS ItemName,
            d.Expr5 AS Qty,
            d.Expr6 AS UnitPrice,
            d.Expr12 AS Revenue,
            (d.Expr12 - (ISNULL(d.UnitCost, ISNULL(im.UnitCost, 0)) * d.Expr5)) AS Profit,
            CASE WHEN d.Expr12 > 0 
                THEN ((d.Expr12 - (ISNULL(d.UnitCost, ISNULL(im.UnitCost, 0)) * d.Expr5)) * 100.0 / d.Expr12)
                ELSE 0 
            END AS Margin
        FROM [View_Sales report whatsapp] d
        LEFT JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
        WHERE YEAR(d.Expr1) = ${year}
            AND d.Expr14 = 0
            ${category ? `AND d.Categoty LIKE '%${category}%'` : ''}
        ORDER BY MONTH(d.Expr1), 
                 CASE WHEN d.Categoty = 'TYRES' THEN 1 ELSE 2 END, -- Prioritize 'TYRES'
                 ISNULL(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Others'), 
                 d.Expr1
    `,


};

// ═══════════════════════════════════════════════════════════════════════════════
// PDF REPORT GENERATOR
// ═══════════════════════════════════════════════════════════════════════════════

class PDFReportGenerator {
    constructor() {
        this.doc = null;
        this.yPosition = 0;
    }

    async generateYearlySalesReport(year, data, categoryData, options = {}) {
        const filename = `Sales_Report_DETAILED_${year}_${Date.now()}.pdf`;
        const filepath = path.join(REPORTS_DIR, filename);
        
        this.doc = new PDFDocument({ 
            size: 'A4', 
            margin: 50,
            info: {
                Title: `Sales Report ${year}`,
                Author: COMPANY.name,
                Subject: `Annual Sales Report for ${year}`
            }
        });
        
        const stream = fs.createWriteStream(filepath);
        this.doc.pipe(stream);
        
        const {
            monthlyCategoryData = [],
            monthlyBrandData = [],
            itemAggData = [],
            categoryFilter = null
        } = options || {};

        // Calculate Summary for the Cover Page
        const totalRevenue = data.reduce((sum, r) => sum + (Number(r.TotalRevenue || r.Revenue || 0)), 0);
        const totalProfit = data.reduce((sum, r) => sum + (Number(r.TotalProfit || r.Profit || 0)), 0);
        const totalQty = data.reduce((sum, r) => sum + (Number(r.TotalQty || r.Qty || 0)), 0);
        const margin = totalRevenue > 0 ? (totalProfit * 100.0 / totalRevenue) : 0;

        // ═══════════════════════════════════════════════════════════════════════════════
        // PAGE 1: EXECUTIVE SUMMARY (YEAR OVERVIEW)
        // ═══════════════════════════════════════════════════════════════════════════════
        this._drawHeader(`${year} ANNUAL PERFORMANCE SUMMARY`);
        
        this._drawSummaryBox({
            'Total Annual Revenue': formatCurrency(totalRevenue),
            'Total Annual Profit': formatCurrency(totalProfit),
            'Overall Margin': `${margin.toFixed(2)}%`,
            'Total Units Sold': formatNumber(totalQty)
        });

        this._drawSectionHeader('📅 MONTHLY PERFORMANCE');
        this._drawMonthlyProfitTable(monthlyCategoryData); // Using category breakdown for overview if available, or just month totals
        
        // ═══════════════════════════════════════════════════════════════════════════════
        // DETAILED MONTHLY BREAKDOWN
        // ═══════════════════════════════════════════════════════════════════════════════
        
        // Filter out Katta/Dag/Rebuild if not explicitly requested, or just categorize them separarely
        // The user specifically said "Not Katta Tyres".
        // Use the 'Category' field from the query to separate sections.

        const monthsWithSales = Array.from({ length: 12 }, (_, i) => i + 1)
            .filter(m => (data.find(r => r.SaleMonth === m) || itemAggData.find(r => r.SaleMonth === m)));

        for (const monthNum of monthsWithSales) {
            this.doc.addPage();
            this._drawHeader(`${getMonthName(monthNum)} ${year} - DETAILED BREAKDOWN`);

            // 1. Month Summary Box
            const monthItems = itemAggData.filter(r => r.SaleMonth === monthNum);
            const mRevenue = monthItems.reduce((sum, r) => sum + (Number(r.Revenue || 0)), 0);
            const mProfit = monthItems.reduce((sum, r) => sum + (Number(r.Profit || 0)), 0);
            const mMargin = mRevenue > 0 ? (mProfit * 100.0 / mRevenue) : 0;
            
            this._drawSummaryBox({
                'Month Revenue': formatCurrency(mRevenue),
                'Month Profit': formatCurrency(mProfit),
                'Margin %': `${mMargin.toFixed(1)}%`,
                'Items Sold': formatNumber(monthItems.length)
            });

            // 2. Separated Loops: "TYRES" vs "OTHERS"
            // Filter Logic based on user request: "Tyre Category details are important (not Katta)"
            
            const mainTyres = monthItems.filter(i => i.Category === 'TYRES');
            const otherItems = monthItems.filter(i => i.Category !== 'TYRES');

            // --- SECTION A: MAIN TYRES (The "Good" ones) ---
            if (mainTyres.length > 0) {
                 this._drawSectionHeader('🚗 MAIN TYRES (Brand New)');
                 this._renderGroupedItems(mainTyres);
            }

            // --- SECTION B: OTHERS (Katta, Services, Tubes, etc) ---
            if (otherItems.length > 0) {
                // Check page space
                if (this.yPosition > 650) this.doc.addPage();
                
                this._drawSectionHeader('🛠️ OTHER CATEGORIES (Services, Rebuilds, Accessories)');
                this._renderGroupedItems(otherItems);
            }
        }
        
        // Footer on all pages
        const pages = this.doc.bufferedPageRange();
        for (let i = 0; i < pages.count; i++) {
            // PDFKit uses 0-based page indexing internally but switchToPage expects page number
            // For footer, we just need the page number for display
            this._drawFooter(i + 1, pages.count);
        }
        
        this.doc.end();
        
        return new Promise((resolve, reject) => {
            stream.on('finish', () => resolve(filepath));
            stream.on('error', reject);
        });
    }

    _drawMiniBarChart(monthlyRows, { title, valueKey, color, yLabel }) {
        const chartLeft = 50;
        const chartTop = this.yPosition;
        const chartWidth = 495;
        const chartHeight = 80;

        this.doc.fill('#333333').fontSize(10).text(title, chartLeft, chartTop);

        const barsTop = chartTop + 15;
        const barsHeight = chartHeight - 20;
        const months = Array.from({ length: 12 }, (_, i) => i + 1);
        const values = months.map(m => {
            const row = monthlyRows.find(r => r.SaleMonth === m) || {};
            return Number(row[valueKey] || 0);
        });
        const maxVal = Math.max(...values, 1);
        const barGap = 3;
        const barWidth = Math.floor((chartWidth - (months.length - 1) * barGap) / months.length);

        let x = chartLeft;
        for (let i = 0; i < months.length; i++) {
            const v = values[i];
            const h = Math.round((v / maxVal) * barsHeight);
            const y = barsTop + (barsHeight - h);
            this.doc.rect(x, y, barWidth, h).fill(color);
            // Month label
            this.doc.fill('#666666').fontSize(7)
                .text(String(months[i]), x, barsTop + barsHeight + 2, { width: barWidth, align: 'center' });
            x += barWidth + barGap;
        }

        // Axis label (right)
        this.doc.fill('#888888').fontSize(8)
            .text(`${yLabel || ''} max ${formatCurrency(maxVal)}`, chartLeft, barsTop - 2, { width: chartWidth, align: 'right' });

        this.yPosition = chartTop + chartHeight + 12;
    }

    _drawMonthlyProfitTable(data) {
        const tableTop = this.yPosition;
        const colWidths = [85, 55, 65, 110, 100, 55, 70];
        const headers = ['Month', 'Inv', 'Qty', 'Revenue', 'Profit', 'Margin', 'MoM Rev'];

        this.doc.rect(50, tableTop, 495, 25).fill('#1a5f7a');
        let xPos = 55;
        headers.forEach((header, i) => {
            this.doc.fill('#ffffff').fontSize(9).text(header, xPos, tableTop + 8, { width: colWidths[i] - 5 });
            xPos += colWidths[i];
        });

        let yPos = tableTop + 25;
        const months = Array.from({ length: 12 }, (_, i) => i + 1);
        const monthRows = months.map(m => data.find(d => d.SaleMonth === m) || {
            SaleMonth: m,
            InvoiceCount: 0,
            TotalQty: 0,
            TotalRevenue: 0,
            TotalProfit: 0,
            ProfitMargin: 0
        });

        for (let i = 0; i < monthRows.length; i++) {
            const row = monthRows[i];
            const prev = i > 0 ? monthRows[i - 1] : null;
            const rev = Number(row.TotalRevenue || 0);
            const prevRev = prev ? Number(prev.TotalRevenue || 0) : 0;
            const mom = prev ? (prevRev > 0 ? ((rev - prevRev) * 100.0 / prevRev) : (rev > 0 ? 100 : 0)) : 0;

            const bgColor = i % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(50, yPos, 495, 22).fill(bgColor);

            xPos = 55;
            this.doc.fill('#333333').fontSize(8);
            this.doc.text(getMonthName(row.SaleMonth), xPos, yPos + 6, { width: colWidths[0] - 5 });
            xPos += colWidths[0];
            this.doc.text(formatNumber(row.InvoiceCount || 0), xPos, yPos + 6, { width: colWidths[1] - 5 });
            xPos += colWidths[1];
            this.doc.text(formatNumber(row.TotalQty || 0), xPos, yPos + 6, { width: colWidths[2] - 5 });
            xPos += colWidths[2];
            this.doc.text(formatCurrency(rev), xPos, yPos + 6, { width: colWidths[3] - 5 });
            xPos += colWidths[3];
            this.doc.text(formatCurrency(row.TotalProfit || 0), xPos, yPos + 6, { width: colWidths[4] - 5 });
            xPos += colWidths[4];
            this.doc.text(`${Number(row.ProfitMargin || 0).toFixed(1)}%`, xPos, yPos + 6, { width: colWidths[5] - 5 });
            xPos += colWidths[5];
            const momText = prev ? `${mom.toFixed(1)}%` : '-';
            this.doc.text(momText, xPos, yPos + 6, { width: colWidths[6] - 5 });

            yPos += 22;
        }

        const totals = monthRows.reduce((acc, r) => {
            acc.InvoiceCount += Number(r.InvoiceCount || 0);
            acc.TotalQty += Number(r.TotalQty || 0);
            acc.TotalRevenue += Number(r.TotalRevenue || 0);
            acc.TotalProfit += Number(r.TotalProfit || 0);
            return acc;
        }, { InvoiceCount: 0, TotalQty: 0, TotalRevenue: 0, TotalProfit: 0 });
        const totalMargin = totals.TotalRevenue > 0 ? (totals.TotalProfit * 100.0 / totals.TotalRevenue) : 0;

        this.doc.rect(50, yPos, 495, 25).fill('#1a5f7a');
        xPos = 55;
        this.doc.fill('#ffffff').fontSize(9);
        this.doc.text('TOTAL', xPos, yPos + 8, { width: colWidths[0] - 5 });
        xPos += colWidths[0];
        this.doc.text(formatNumber(totals.InvoiceCount), xPos, yPos + 8, { width: colWidths[1] - 5 });
        xPos += colWidths[1];
        this.doc.text(formatNumber(totals.TotalQty), xPos, yPos + 8, { width: colWidths[2] - 5 });
        xPos += colWidths[2];
        this.doc.text(formatCurrency(totals.TotalRevenue), xPos, yPos + 8, { width: colWidths[3] - 5 });
        xPos += colWidths[3];
        this.doc.text(formatCurrency(totals.TotalProfit), xPos, yPos + 8, { width: colWidths[4] - 5 });
        xPos += colWidths[4];
        this.doc.text(`${totalMargin.toFixed(1)}%`, xPos, yPos + 8, { width: colWidths[5] - 5 });

        this.yPosition = yPos + 40;
    }

    _drawCategoryProfitTable(data) {
        const tableTop = this.yPosition;
        const colWidths = [210, 60, 70, 95, 95, 55];
        const headers = ['Category', 'Inv', 'Qty', 'Revenue', 'Profit', 'Margin'];

        this.doc.rect(50, tableTop, 495, 25).fill('#1a5f7a');
        let xPos = 55;
        headers.forEach((header, i) => {
            this.doc.fill('#ffffff').fontSize(9).text(header, xPos, tableTop + 8, { width: colWidths[i] - 5 });
            xPos += colWidths[i];
        });

        let yPos = tableTop + 25;
        const rows = (data || []).slice(0, 40);
        rows.forEach((row, index) => {
            const bgColor = index % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(50, yPos, 495, 22).fill(bgColor);

            const rev = Number(row.TotalRevenue || row.TotalAmount || 0);
            const profit = Number(row.TotalProfit || 0);
            const margin = Number(row.ProfitMargin || (rev > 0 ? (profit * 100.0 / rev) : 0));

            xPos = 55;
            this.doc.fill('#333333').fontSize(8);
            this.doc.text(String(row.Category || 'Unknown').slice(0, 28), xPos, yPos + 6, { width: colWidths[0] - 5 });
            xPos += colWidths[0];
            this.doc.text(formatNumber(row.InvoiceCount || 0), xPos, yPos + 6, { width: colWidths[1] - 5 });
            xPos += colWidths[1];
            this.doc.text(formatNumber(row.TotalQty || 0), xPos, yPos + 6, { width: colWidths[2] - 5 });
            xPos += colWidths[2];
            this.doc.text(formatCurrency(rev), xPos, yPos + 6, { width: colWidths[3] - 5 });
            xPos += colWidths[3];
            this.doc.text(formatCurrency(profit), xPos, yPos + 6, { width: colWidths[4] - 5 });
            xPos += colWidths[4];
            this.doc.text(`${margin.toFixed(1)}%`, xPos, yPos + 6, { width: colWidths[5] - 5 });

            yPos += 22;
        });

        this.yPosition = yPos + 20;
    }

    _renderGroupedItems(itemList) {
        // Group by Brand
        const brandGroups = {};
        itemList.forEach(item => {
            const brand = item.Brand || 'Unknown';
            if (!brandGroups[brand]) brandGroups[brand] = [];
            brandGroups[brand].push(item);
        });

        // Iterate brands and print tables
        const sortedBrands = Object.keys(brandGroups).sort();
        
        for (const brand of sortedBrands) {
            const items = brandGroups[brand].sort((a, b) => {
                if (a.SaleDate && b.SaleDate) {
                    return new Date(a.SaleDate) - new Date(b.SaleDate);
                }
                const revA = Number(a.TotalRevenue || a.Revenue || 0);
                const revB = Number(b.TotalRevenue || b.Revenue || 0);
                return revB - revA;
            });
            
            // Check space
            if (this.yPosition > 700) {
                this.doc.addPage();
                this._drawHeader(`(Continued)`);
            }

            this.doc.fill('#1a5f7a').fontSize(10).font('Helvetica-Bold')
                .text(`🏷️ BRAND: ${brand} (${items.length} Items)`, 50, this.yPosition);
            this.yPosition += 18;

            this._drawDetailedItemTable(items);
            this.yPosition += 12;
        }
    }

    _drawDetailedItemTable(items) {
        // Updated columns to include Date
        const colWidths = [40, 160, 40, 70, 65, 80, 40]; // Total 495
        const headers = ['Date', 'Description', 'Qty', 'Unit Price', 'Profit', 'Revenue', '%'];
        const startX = 50;

        // Draw Header
        this.doc.rect(startX, this.yPosition, 495, 20).fill('#1a5f7a');
        let xPos = startX + 5;
        this.doc.fill('#ffffff').fontSize(8).font('Helvetica-Bold');
        headers.forEach((h, i) => {
            this.doc.text(h, xPos, this.yPosition + 6, { width: colWidths[i] - 5 });
            xPos += colWidths[i];
        });
        this.yPosition += 20;

        // Draw Rows
        this.doc.fill('#333333').fontSize(8).font('Helvetica');
        
        items.forEach((item, index) => {
            // Check page break
            if (this.yPosition > 760) {
                this.doc.addPage();
                this._drawHeader('(Continued)');
                this.yPosition += 10;
                
                // Redraw header
                this.doc.rect(startX, this.yPosition, 495, 20).fill('#1a5f7a');
                let x = startX + 5;
                this.doc.fill('#ffffff').fontSize(8).font('Helvetica-Bold');
                headers.forEach((h, i) => {
                    this.doc.text(h, x, this.yPosition + 6, { width: colWidths[i] - 5 });
                    x += colWidths[i];
                });
                this.yPosition += 20;
                this.doc.fill('#333333').fontSize(8).font('Helvetica');
            }

            const bgColor = index % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(startX, this.yPosition, 495, 18).fill(bgColor);

            // Handle both property sets (Aggregated vs Log)
            const mRev = Number(item.TotalRevenue || item.Revenue || 0);
            const mProfit = Number(item.TotalProfit || item.Profit || 0);
            const mQty = Number(item.TotalQty || item.Qty || 0);
            const mPrice = Number(item.AvgUnitPrice || item.UnitPrice || 0);
            const margin = mRev > 0 ? (mProfit * 100.0 / mRev) : 0;
            const saleDate = item.SaleDate ? moment(item.SaleDate).format('DD/MM') : '-';

            xPos = startX + 5;
            this.doc.fill('#333333');
            
            // Date
            this.doc.text(saleDate, xPos, this.yPosition + 5, { width: colWidths[0] - 5 });
            xPos += colWidths[0];

            // Description (truncate)
            this.doc.text(String(item.ItemName || '').slice(0, 35), xPos, this.yPosition + 5, { width: colWidths[1] - 5 });
            xPos += colWidths[1];
            
            // Qty
            this.doc.text(formatNumber(mQty), xPos, this.yPosition + 5, { width: colWidths[2] - 5 });
            xPos += colWidths[2];

            // Unit Price
            this.doc.text(formatNumber(mPrice), xPos, this.yPosition + 5, { width: colWidths[3] - 5 });
            xPos += colWidths[3];

            // Profit (Green if positive, Red if negative)
            if (mProfit < 0) this.doc.fill('#c0392b');
            this.doc.text(formatNumber(mProfit), xPos, this.yPosition + 5, { width: colWidths[4] - 5 });
            this.doc.fill('#333333');
            xPos += colWidths[4];

            // Revenue
            this.doc.text(formatNumber(mRev), xPos, this.yPosition + 5, { width: colWidths[5] - 5 });
            xPos += colWidths[5];

            // Margin %
            this.doc.text(`${margin.toFixed(0)}%`, xPos, this.yPosition + 5, { width: colWidths[6] - 5 });

            this.yPosition += 18;
        });
    }

    _drawSimpleTable(rows, columns) {
        const tableTop = this.yPosition;
        const totalWidth = columns.reduce((s, c) => s + c.width, 0);
        const width = Math.min(495, totalWidth);

        // Header
        this.doc.rect(50, tableTop, width, 22).fill('#1a5f7a');
        let xPos = 55;
        this.doc.fill('#ffffff').fontSize(9);
        for (const col of columns) {
            this.doc.text(col.label, xPos, tableTop + 6, { width: col.width - 5 });
            xPos += col.width;
        }

        let yPos = tableTop + 22;
        const maxRows = Math.min(rows.length, 18);
        for (let i = 0; i < maxRows; i++) {
            const r = rows[i];
            const bgColor = i % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(50, yPos, width, 20).fill(bgColor);
            xPos = 55;
            this.doc.fill('#333333').fontSize(8);
            for (const col of columns) {
                const raw = r[col.key];
                const val = col.fmt ? col.fmt(raw) : (raw == null ? '' : String(raw));
                this.doc.text(String(val), xPos, yPos + 5, { width: col.width - 5 });
                xPos += col.width;
            }
            yPos += 20;

            // Avoid running into footer
            if (yPos > 740) break;
        }
        this.yPosition = yPos + 16;
    }

    async generateTyreSalesReport(year, monthlyData, brandData, options = {}) {
        const filename = `Tyre_Sales_Report_${year}_${Date.now()}.pdf`;
        const filepath = path.join(REPORTS_DIR, filename);
        
        this.doc = new PDFDocument({ 
            size: 'A4', 
            margin: 50,
            info: {
                Title: `Tyre Sales Report ${year}`,
                Author: COMPANY.name
            }
        });
        
        const stream = fs.createWriteStream(filepath);
        this.doc.pipe(stream);
        
        // Header
        this._drawHeader(`TYRE SALES REPORT ${year}`);
        
        // Summary
        const totalAmount = monthlyData.reduce((sum, row) => sum + (row.TotalAmount || 0), 0);
        const totalQty = monthlyData.reduce((sum, row) => sum + (row.TotalQty || 0), 0);
        
        this._drawSummaryBox({
            'Total Tyre Revenue': formatCurrency(totalAmount),
            'Total Tyres Sold': formatNumber(totalQty),
            'Average/Month': formatCurrency(totalAmount / 12),
            'Avg Qty/Month': formatNumber(Math.round(totalQty / 12))
        });
        
        // Monthly Table
        this._drawSectionHeader('📅 MONTHLY TYRE SALES');
        this._drawMonthlyTable(monthlyData);
        
        // Brand Analysis (new page)
        if (brandData && brandData.length > 0) {
            this.doc.addPage();
            this._drawHeader(`BRAND ANALYSIS ${year}`);
            this._drawSectionHeader('🏷️ SALES BY BRAND');
            this._drawBrandTable(brandData, totalAmount);
        }
        
        this.doc.end();
        
        return new Promise((resolve, reject) => {
            stream.on('finish', () => resolve(filepath));
            stream.on('error', reject);
        });
    }

    _drawHeader(title) {
        // Minimal Header (No company branding as requested)
        this.doc.fill('#1a5f7a').fontSize(16).text(title, 50, 50, { align: 'left' });
        
        // Generated Date
        this.doc.fill('#888888').fontSize(9)
            .text(`Generated: ${moment().format('YYYY-MM-DD HH:mm')}`, 50, 70, { align: 'left' });
        
        // Single Divider
        this.doc.moveTo(50, 85).lineTo(545, 85).stroke('#1a5f7a');
        
        this.yPosition = 100;
    }

    _drawSummaryBox(data) {
        const boxY = this.yPosition;
        const boxWidth = 495;
        const boxHeight = 80;
        
        // Background
        this.doc.rect(50, boxY, boxWidth, boxHeight).fill('#f0f7fa');
        
        // Draw summary items
        const entries = Object.entries(data);
        const itemWidth = boxWidth / entries.length;
        
        entries.forEach(([label, value], index) => {
            const x = 50 + (index * itemWidth) + (itemWidth / 2);
            this.doc.fill('#1a5f7a').fontSize(10).text(label, x - 50, boxY + 15, { width: 100, align: 'center' });
            this.doc.fill('#333333').fontSize(14).text(value, x - 50, boxY + 35, { width: 100, align: 'center' });
        });
        
        this.yPosition = boxY + boxHeight + 20;
    }

    _drawSectionHeader(title) {
        this.doc.fill('#1a5f7a').fontSize(14).text(title, 50, this.yPosition);
        this.yPosition += 25;
    }

    _drawMonthlyTable(data) {
        const tableTop = this.yPosition;
        const colWidths = [100, 100, 100, 130];
        const headers = ['Month', 'Invoices', 'Quantity', 'Amount'];
        
        // Header row
        this.doc.rect(50, tableTop, 495, 25).fill('#1a5f7a');
        let xPos = 55;
        headers.forEach((header, i) => {
            this.doc.fill('#ffffff').fontSize(10).text(header, xPos, tableTop + 8, { width: colWidths[i] - 10 });
            xPos += colWidths[i];
        });
        
        // Data rows
        let yPos = tableTop + 25;
        const allMonths = Array.from({ length: 12 }, (_, i) => i + 1);
        
        allMonths.forEach((monthNum, index) => {
            const rowData = data.find(d => d.SaleMonth === monthNum) || { InvoiceCount: 0, TotalQty: 0, TotalAmount: 0 };
            const bgColor = index % 2 === 0 ? '#ffffff' : '#f5f5f5';
            
            this.doc.rect(50, yPos, 495, 22).fill(bgColor);
            
            xPos = 55;
            this.doc.fill('#333333').fontSize(9);
            this.doc.text(getMonthName(monthNum), xPos, yPos + 6, { width: colWidths[0] - 10 });
            xPos += colWidths[0];
            this.doc.text(formatNumber(rowData.InvoiceCount || 0), xPos, yPos + 6, { width: colWidths[1] - 10 });
            xPos += colWidths[1];
            this.doc.text(formatNumber(rowData.TotalQty || 0), xPos, yPos + 6, { width: colWidths[2] - 10 });
            xPos += colWidths[2];
            this.doc.text(formatCurrency(rowData.TotalAmount || 0), xPos, yPos + 6, { width: colWidths[3] - 10 });
            
            yPos += 22;
        });
        
        // Total row
        const totalAmount = data.reduce((sum, row) => sum + (row.TotalAmount || 0), 0);
        const totalQty = data.reduce((sum, row) => sum + (row.TotalQty || 0), 0);
        const totalInvoices = data.reduce((sum, row) => sum + (row.InvoiceCount || 0), 0);
        
        this.doc.rect(50, yPos, 495, 25).fill('#1a5f7a');
        xPos = 55;
        this.doc.fill('#ffffff').fontSize(10);
        this.doc.text('TOTAL', xPos, yPos + 8, { width: colWidths[0] - 10 });
        xPos += colWidths[0];
        this.doc.text(formatNumber(totalInvoices), xPos, yPos + 8, { width: colWidths[1] - 10 });
        xPos += colWidths[1];
        this.doc.text(formatNumber(totalQty), xPos, yPos + 8, { width: colWidths[2] - 10 });
        xPos += colWidths[2];
        this.doc.text(formatCurrency(totalAmount), xPos, yPos + 8, { width: colWidths[3] - 10 });
        
        this.yPosition = yPos + 40;
    }

    _drawCategoryTable(data) {
        const tableTop = this.yPosition;
        const colWidths = [180, 80, 80, 100];
        const headers = ['Category', 'Invoices', 'Quantity', 'Amount'];
        
        // Header row
        this.doc.rect(50, tableTop, 495, 25).fill('#1a5f7a');
        let xPos = 55;
        headers.forEach((header, i) => {
            this.doc.fill('#ffffff').fontSize(10).text(header, xPos, tableTop + 8, { width: colWidths[i] - 10 });
            xPos += colWidths[i];
        });
        
        // Data rows
        let yPos = tableTop + 25;
        data.forEach((row, index) => {
            const bgColor = index % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(50, yPos, 495, 22).fill(bgColor);
            
            xPos = 55;
            this.doc.fill('#333333').fontSize(9);
            this.doc.text(row.Category || 'Unknown', xPos, yPos + 6, { width: colWidths[0] - 10 });
            xPos += colWidths[0];
            this.doc.text(formatNumber(row.InvoiceCount || 0), xPos, yPos + 6, { width: colWidths[1] - 10 });
            xPos += colWidths[1];
            this.doc.text(formatNumber(row.TotalQty || 0), xPos, yPos + 6, { width: colWidths[2] - 10 });
            xPos += colWidths[2];
            this.doc.text(formatCurrency(row.TotalAmount || 0), xPos, yPos + 6, { width: colWidths[3] - 10 });
            
            yPos += 22;
        });
        
        this.yPosition = yPos + 20;
    }

    _drawBrandTable(data, totalRevenue) {
        const tableTop = this.yPosition;
        const colWidths = [150, 80, 120, 80];
        const headers = ['Brand', 'Quantity', 'Revenue', 'Share %'];
        
        this.doc.rect(50, tableTop, 495, 25).fill('#1a5f7a');
        let xPos = 55;
        headers.forEach((header, i) => {
            this.doc.fill('#ffffff').fontSize(10).text(header, xPos, tableTop + 8, { width: colWidths[i] - 10 });
            xPos += colWidths[i];
        });
        
        let yPos = tableTop + 25;
        data.forEach((row, index) => {
            const bgColor = index % 2 === 0 ? '#ffffff' : '#f5f5f5';
            this.doc.rect(50, yPos, 495, 22).fill(bgColor);
            
            const share = totalRevenue > 0 ? ((row.TotalRevenue / totalRevenue) * 100).toFixed(1) : 0;
            
            xPos = 55;
            this.doc.fill('#333333').fontSize(9);
            this.doc.text(row.Brand || 'Unknown', xPos, yPos + 6, { width: colWidths[0] - 10 });
            xPos += colWidths[0];
            this.doc.text(formatNumber(row.TotalQty || 0), xPos, yPos + 6, { width: colWidths[1] - 10 });
            xPos += colWidths[1];
            this.doc.text(formatCurrency(row.TotalRevenue || 0), xPos, yPos + 6, { width: colWidths[2] - 10 });
            xPos += colWidths[2];
            this.doc.text(`${share}%`, xPos, yPos + 6, { width: colWidths[3] - 10 });
            
            yPos += 22;
        });
        
        this.yPosition = yPos + 20;
    }

    _drawFooter(pageNum, totalPages) {
        const footerY = 780;
        this.doc.moveTo(50, footerY).lineTo(545, footerY).stroke('#cccccc');
        this.doc.fill('#888888').fontSize(8)
            .text(`${COMPANY.name} - Confidential`, 50, footerY + 10)
            .text(`Page ${pageNum} of ${totalPages}`, 450, footerY + 10);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// EXCEL REPORT GENERATOR
// ═══════════════════════════════════════════════════════════════════════════════

class ExcelReportGenerator {
    async generateYearlySalesReport(year, monthlyData, categoryData, options = {}) {
        const filename = `Sales_Report_DETAILED_${year}_${Date.now()}.xlsx`;
        const filepath = path.join(REPORTS_DIR, filename);

        const {
            monthlyCategoryData = [],
            monthlyBrandData = [],
            itemAggData = [],
            categoryFilter = null
        } = options || {};
        
        const workbook = new ExcelJS.Workbook();
        workbook.creator = COMPANY.name;
        workbook.created = new Date();
        
        // Summary Sheet
        const summarySheet = workbook.addWorksheet('Summary', {
            properties: { tabColor: { argb: '1a5f7a' } }
        });
        
        // Header
        summarySheet.mergeCells('A1:E1');
        summarySheet.getCell('A1').value = COMPANY.name;
        summarySheet.getCell('A1').font = { size: 18, bold: true, color: { argb: '1a5f7a' } };
        
        summarySheet.mergeCells('A2:E2');
        summarySheet.getCell('A2').value = `Annual Sales Report - ${year} (Detailed)`;
        summarySheet.getCell('A2').font = { size: 14, bold: true };
        
        summarySheet.mergeCells('A3:E3');
        summarySheet.getCell('A3').value = `Generated: ${moment().format('MMMM D, YYYY h:mm A')}`;
        summarySheet.getCell('A3').font = { size: 10, color: { argb: '888888' } };
        
        // Summary Stats
        const totalAmount = monthlyData.reduce((sum, row) => sum + (row.TotalRevenue || row.TotalAmount || 0), 0);
        const totalQty = monthlyData.reduce((sum, row) => sum + (row.TotalQty || 0), 0);
        const totalInvoices = monthlyData.reduce((sum, row) => sum + (row.InvoiceCount || 0), 0);
        const totalProfit = monthlyData.reduce((sum, row) => sum + (row.TotalProfit || 0), 0);
        const profitMargin = totalAmount > 0 ? (totalProfit / totalAmount) : 0;
        
        summarySheet.getCell('A5').value = 'Total Revenue:';
        summarySheet.getCell('B5').value = totalAmount;
        summarySheet.getCell('B5').numFmt = '"Rs. "#,##0.00';

        summarySheet.getCell('A6').value = 'Total Profit:';
        summarySheet.getCell('B6').value = totalProfit;
        summarySheet.getCell('B6').numFmt = '"Rs. "#,##0.00';

        summarySheet.getCell('A7').value = 'Profit Margin:';
        summarySheet.getCell('B7').value = profitMargin;
        summarySheet.getCell('B7').numFmt = '0.00%';
        
        summarySheet.getCell('A9').value = 'Total Quantity:';
        summarySheet.getCell('B9').value = totalQty;

        summarySheet.getCell('A10').value = 'Total Invoices:';
        summarySheet.getCell('B10').value = totalInvoices;

        if (categoryFilter) {
            summarySheet.getCell('A12').value = 'Category Filter:';
            summarySheet.getCell('B12').value = categoryFilter;
        }
        
        // Monthly Data Sheet
        const monthlySheet = workbook.addWorksheet('Monthly Sales', {
            properties: { tabColor: { argb: '2e86ab' } }
        });
        
        // Headers
        const monthlyHeaders = ['Month', 'Invoices', 'Quantity', 'Revenue', 'Cost', 'Profit', 'Margin', 'MoM Revenue'];
        monthlySheet.addRow(monthlyHeaders);
        monthlySheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };
        monthlySheet.getRow(1).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };
        
        const monthRows = [];
        for (let month = 1; month <= 12; month++) {
            const rowData = monthlyData.find(d => d.SaleMonth === month) || {
                SaleMonth: month,
                InvoiceCount: 0,
                TotalQty: 0,
                TotalRevenue: 0,
                TotalCost: 0,
                TotalProfit: 0,
                ProfitMargin: 0
            };
            monthRows.push(rowData);
        }

        for (let i = 0; i < monthRows.length; i++) {
            const rowData = monthRows[i];
            const prev = i > 0 ? monthRows[i - 1] : null;
            const rev = Number(rowData.TotalRevenue || 0);
            const prevRev = prev ? Number(prev.TotalRevenue || 0) : 0;
            const mom = i === 0 ? null : (prevRev > 0 ? (rev - prevRev) / prevRev : (rev > 0 ? 1 : 0));

            monthlySheet.addRow([
                getMonthName(i + 1),
                Number(rowData.InvoiceCount || 0),
                Number(rowData.TotalQty || 0),
                rev,
                Number(rowData.TotalCost || 0),
                Number(rowData.TotalProfit || 0),
                Number(rowData.ProfitMargin || 0) / 100.0,
                mom
            ]);
        }
        
        // Total row
        const totalCost = monthlyData.reduce((sum, row) => sum + (row.TotalCost || 0), 0);
        monthlySheet.addRow(['TOTAL', totalInvoices, totalQty, totalAmount, totalCost, totalProfit, profitMargin, null]);
        const lastRow = monthlySheet.lastRow;
        lastRow.font = { bold: true };
        lastRow.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'E8E8E8' } };
        
        // Formats
        monthlySheet.getColumn(4).numFmt = '"Rs. "#,##0.00';
        monthlySheet.getColumn(5).numFmt = '"Rs. "#,##0.00';
        monthlySheet.getColumn(6).numFmt = '"Rs. "#,##0.00';
        monthlySheet.getColumn(7).numFmt = '0.00%';
        monthlySheet.getColumn(8).numFmt = '0.00%';

        monthlySheet.getColumn(4).width = 18;
        monthlySheet.getColumn(5).width = 18;
        monthlySheet.getColumn(6).width = 18;
        monthlySheet.getColumn(7).width = 12;
        monthlySheet.getColumn(8).width = 14;
        monthlySheet.getColumn(1).width = 15;
        monthlySheet.getColumn(2).width = 12;
        monthlySheet.getColumn(3).width = 12;
        
        // Category Sheet
        if (categoryData && categoryData.length > 0) {
            const categorySheet = workbook.addWorksheet('By Category', {
                properties: { tabColor: { argb: 'f18f01' } }
            });
            
            const catHeaders = ['Category', 'Invoices', 'Quantity', 'Revenue', 'Cost', 'Profit', 'Margin', '% Share'];
            categorySheet.addRow(catHeaders);
            categorySheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };
            categorySheet.getRow(1).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };
            
            categoryData.forEach(row => {
                const revenue = Number(row.TotalRevenue || row.TotalAmount || 0);
                const profit = Number(row.TotalProfit || 0);
                const margin = Number(row.ProfitMargin || (revenue > 0 ? (profit / revenue) : 0));
                const share = totalAmount > 0 ? (revenue / totalAmount) : 0;
                categorySheet.addRow([
                    row.Category || 'Unknown',
                    row.InvoiceCount || 0,
                    row.TotalQty || 0,
                    revenue,
                    Number(row.TotalCost || 0),
                    profit,
                    (typeof margin === 'number' && margin > 1) ? (margin / 100.0) : margin,
                    share
                ]);
            });
            
            categorySheet.getColumn(4).numFmt = '"Rs. "#,##0.00';
            categorySheet.getColumn(5).numFmt = '"Rs. "#,##0.00';
            categorySheet.getColumn(6).numFmt = '"Rs. "#,##0.00';
            categorySheet.getColumn(7).numFmt = '0.00%';
            categorySheet.getColumn(8).numFmt = '0.00%';
            categorySheet.getColumn(1).width = 25;
            categorySheet.getColumn(4).width = 18;
            categorySheet.getColumn(6).width = 18;
        }

        // Month x Category Sheet (detailed)
        if (monthlyCategoryData && monthlyCategoryData.length > 0) {
            const mcSheet = workbook.addWorksheet('Month-Category', {
                properties: { tabColor: { argb: '7f8c8d' } }
            });
            mcSheet.addRow(['Month', 'Category', 'Qty', 'Revenue', 'Cost', 'Profit']);
            mcSheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };
            mcSheet.getRow(1).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };

            monthlyCategoryData.forEach(r => {
                mcSheet.addRow([
                    getMonthName(Number(r.SaleMonth || 0) || 0),
                    r.Category || 'Unknown',
                    Number(r.TotalQty || 0),
                    Number(r.TotalRevenue || 0),
                    Number(r.TotalCost || 0),
                    Number(r.TotalProfit || 0)
                ]);
            });
            mcSheet.getColumn(4).numFmt = '"Rs. "#,##0.00';
            mcSheet.getColumn(5).numFmt = '"Rs. "#,##0.00';
            mcSheet.getColumn(6).numFmt = '"Rs. "#,##0.00';
            mcSheet.getColumn(1).width = 14;
            mcSheet.getColumn(2).width = 26;
            mcSheet.getColumn(4).width = 18;
            mcSheet.getColumn(6).width = 18;
        }

        // Month x Brand Sheet (TYRES)
        if (monthlyBrandData && monthlyBrandData.length > 0) {
            const mbSheet = workbook.addWorksheet('Month-Brand (TYRES)', {
                properties: { tabColor: { argb: '16a085' } }
            });
            mbSheet.addRow(['Month', 'Brand', 'Qty', 'Revenue', 'Cost', 'Profit']);
            mbSheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };
            mbSheet.getRow(1).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };

            monthlyBrandData.forEach(r => {
                mbSheet.addRow([
                    getMonthName(Number(r.SaleMonth || 0) || 0),
                    r.Brand || 'Unknown',
                    Number(r.TotalQty || 0),
                    Number(r.TotalRevenue || 0),
                    Number(r.TotalCost || 0),
                    Number(r.TotalProfit || 0)
                ]);
            });
            mbSheet.getColumn(4).numFmt = '"Rs. "#,##0.00';
            mbSheet.getColumn(5).numFmt = '"Rs. "#,##0.00';
            mbSheet.getColumn(6).numFmt = '"Rs. "#,##0.00';
            mbSheet.getColumn(1).width = 14;
            mbSheet.getColumn(2).width = 18;
            mbSheet.getColumn(4).width = 18;
            mbSheet.getColumn(6).width = 18;
        }

        // Full Item Aggregates Sheet
        if (itemAggData && itemAggData.length > 0) {
            const itemsSheet = workbook.addWorksheet('Items (All)', {
                properties: { tabColor: { argb: '9b59b6' } }
            });
            itemsSheet.addRow(['Month', 'Category', 'Brand', 'ItemCode', 'ItemName', 'Qty', 'Avg Price', 'Revenue', 'Cost', 'Profit', 'Margin']);
            itemsSheet.getRow(1).font = { bold: true, color: { argb: 'FFFFFF' } };
            itemsSheet.getRow(1).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };

            itemAggData.forEach(r => {
                itemsSheet.addRow([
                    getMonthName(Number(r.SaleMonth || 0) || 0),
                    r.Category || 'Unknown',
                    r.Brand || 'Unknown',
                    r.ItemCode || '',
                    r.ItemName || '',
                    Number(r.TotalQty || 0),
                    Number(r.AvgUnitPrice || 0),
                    Number(r.TotalRevenue || 0),
                    Number(r.TotalCost || 0),
                    Number(r.TotalProfit || 0),
                    Number(r.ProfitMargin || 0) / 100.0
                ]);
            });

            itemsSheet.getColumn(7).numFmt = '"Rs. "#,##0.00';
            itemsSheet.getColumn(8).numFmt = '"Rs. "#,##0.00';
            itemsSheet.getColumn(9).numFmt = '"Rs. "#,##0.00';
            itemsSheet.getColumn(10).numFmt = '"Rs. "#,##0.00';
            itemsSheet.getColumn(11).numFmt = '0.00%';

            itemsSheet.getColumn(1).width = 14;
            itemsSheet.getColumn(2).width = 20;
            itemsSheet.getColumn(3).width = 14;
            itemsSheet.getColumn(4).width = 12;
            itemsSheet.getColumn(5).width = 45;
            itemsSheet.getColumn(8).width = 18;
            itemsSheet.getColumn(10).width = 18;
        }
        
        await workbook.xlsx.writeFile(filepath);
        return filepath;
    }

    async generateTyreSalesReport(year, monthlyData, brandData, options = {}) {
        const filename = `Tyre_Sales_Report_${year}_${Date.now()}.xlsx`;
        const filepath = path.join(REPORTS_DIR, filename);
        
        const workbook = new ExcelJS.Workbook();
        workbook.creator = COMPANY.name;
        
        // Monthly Tyre Sales Sheet
        const monthlySheet = workbook.addWorksheet('Monthly Tyre Sales');
        
        monthlySheet.mergeCells('A1:D1');
        monthlySheet.getCell('A1').value = `${COMPANY.name} - Tyre Sales Report ${year}`;
        monthlySheet.getCell('A1').font = { size: 16, bold: true, color: { argb: '1a5f7a' } };
        
        const headers = ['Month', 'Quantity Sold', 'Revenue'];
        monthlySheet.addRow([]);
        monthlySheet.addRow(headers);
        monthlySheet.getRow(3).font = { bold: true, color: { argb: 'FFFFFF' } };
        monthlySheet.getRow(3).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };
        
        let totalQty = 0, totalAmount = 0;
        for (let month = 1; month <= 12; month++) {
            const rowData = monthlyData.find(d => d.SaleMonth === month) || { TotalQty: 0, TotalAmount: 0 };
            monthlySheet.addRow([
                getMonthName(month),
                rowData.TotalQty || 0,
                rowData.TotalAmount || 0
            ]);
            totalQty += rowData.TotalQty || 0;
            totalAmount += rowData.TotalAmount || 0;
        }
        
        monthlySheet.addRow(['TOTAL', totalQty, totalAmount]);
        monthlySheet.lastRow.font = { bold: true };
        monthlySheet.lastRow.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'E8E8E8' } };
        
        monthlySheet.getColumn(3).numFmt = '"Rs. "#,##0.00';
        monthlySheet.getColumn(1).width = 15;
        monthlySheet.getColumn(2).width = 15;
        monthlySheet.getColumn(3).width = 20;
        
        // Brand Analysis Sheet
        if (brandData && brandData.length > 0) {
            const brandSheet = workbook.addWorksheet('Brand Analysis');
            
            brandSheet.mergeCells('A1:D1');
            brandSheet.getCell('A1').value = `Brand-wise Tyre Sales ${year}`;
            brandSheet.getCell('A1').font = { size: 14, bold: true };
            
            brandSheet.addRow([]);
            brandSheet.addRow(['Brand', 'Quantity', 'Revenue', 'Market Share']);
            brandSheet.getRow(3).font = { bold: true, color: { argb: 'FFFFFF' } };
            brandSheet.getRow(3).fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: '1a5f7a' } };
            
            const totalBrandRevenue = brandData.reduce((sum, b) => sum + (b.TotalRevenue || 0), 0);
            
            brandData.forEach(row => {
                const share = totalBrandRevenue > 0 ? row.TotalRevenue / totalBrandRevenue : 0;
                brandSheet.addRow([
                    row.Brand,
                    row.TotalQty || 0,
                    row.TotalRevenue || 0,
                    share
                ]);
            });
            
            brandSheet.getColumn(3).numFmt = '"Rs. "#,##0.00';
            brandSheet.getColumn(4).numFmt = '0.00%';
            brandSheet.getColumn(1).width = 18;
            brandSheet.getColumn(3).width = 18;
        }
        
        await workbook.xlsx.writeFile(filepath);
        return filepath;
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// MAIN SMART REPORT CLASS
// ═══════════════════════════════════════════════════════════════════════════════

class SmartReportGenerator {
    constructor() {
        this.pdfGenerator = new PDFReportGenerator();
        this.excelGenerator = new ExcelReportGenerator();
        console.log('📊 SmartReportGenerator v2.1 Initialized');
    }

    /**
     * Check if user is requesting the report menu
     */
    isMenuRequest(message) {
        const msg = message.trim().toLowerCase();
        // Broader regex to catch variations and ignore punctuation/spaces
        const patterns = [
            /titan\s*report/i,
            /report\s*menu/i,
            /titan\s*menu/i,
            /^reports?$/i,         // "report" or "reports" exactly
            /report\s*list/i,
            /show\s*reports/i,
            /report\s*eka/i,
            /sales\s*reports/i,
            /රිපෝට්/               // Sinhala
        ];
        
        return patterns.some(p => p.test(msg));
    }

    /**
     * Get the interactive report menu message
     */
    getReportMenu() {
        return REPORT_MENU.getMenuMessage();
    }

    /**
     * Set user session as waiting for menu selection
     */
    setMenuSession(phoneNumber) {
        const cleanPhone = phoneNumber.replace(/\D/g, '');
        userMenuSessions.set(cleanPhone, {
            waitingForSelection: true,
            timestamp: Date.now()
        });
        console.log(`📋 Menu session started for: ${cleanPhone}`);
    }

    /**
     * Check if user has an active menu session
     */
    hasMenuSession(phoneNumber) {
        const cleanPhone = phoneNumber.replace(/\D/g, '');
        const session = userMenuSessions.get(cleanPhone);
        if (session && Date.now() - session.timestamp < MENU_SESSION_TIMEOUT) {
            return true;
        }
        return false;
    }

    /**
     * Clear user's menu session
     */
    clearMenuSession(phoneNumber) {
        const cleanPhone = phoneNumber.replace(/\D/g, '');
        userMenuSessions.delete(cleanPhone);
        console.log(`📋 Menu session cleared for: ${cleanPhone}`);
    }

    /**
     * Handle menu selection (user replied with a number)
     * Returns the report config or null if invalid
     */
    handleMenuSelection(message) {
        const msg = message.trim();
        // Check if it's a number (1-10)
        const numMatch = msg.match(/^(\d{1,2})$/);
        if (numMatch) {
            const reportConfig = REPORT_MENU.getReportById(numMatch[1]);
            if (reportConfig) {
                return {
                    matched: true,
                    reportType: reportConfig.reportType,
                    params: reportConfig.params,
                    label: reportConfig.label,
                    emoji: reportConfig.emoji
                };
            }
        }
        return null;
    }

    /**
     * Check if a message matches any pre-defined report command
     * Returns { matched: boolean, reportType: string, params: object }
     */
    matchReportCommand(message, senderPhone = null) {
        const msg = message.trim().toLowerCase();
        
        // PRIORITY 1: Check for "titan report" menu request
        if (this.isMenuRequest(message)) {
            return {
                matched: true,
                isMenuRequest: true,
                reportType: 'menu',
                params: {}
            };
        }
        
        // PRIORITY 2: Check if user has active menu session and sent a number
        if (senderPhone && this.hasMenuSession(senderPhone)) {
            const selection = this.handleMenuSelection(message);
            if (selection) {
                this.clearMenuSession(senderPhone);
                return selection;
            }
            // If not a valid number, clear session and continue normal flow
            if (/^\d+$/.test(msg)) {
                this.clearMenuSession(senderPhone);
                return {
                    matched: true,
                    isInvalidSelection: true,
                    message: `❌ වලංගු අංකයක් නොවේ!\n\n📋 "titan report" කියලා යවන්න menu එක බලන්න.`
                };
            }
        }
        
        // Check for year-specific requests
        const yearMatch = msg.match(/(\d{4})/);
        const requestedYear = yearMatch ? parseInt(yearMatch[1]) : null;
        
        // Check for category mentions
        const categoryMatch = msg.match(/(tyre|tyres|wheel\s*alignment|service|battery|tube|katta)/i);
        const requestedCategory = categoryMatch ? categoryMatch[1].toUpperCase() : null;
        
        // Check for format preference
        const wantsPdf = /pdf/i.test(msg);
        const wantsExcel = /excel|xlsx/i.test(msg);
        const format = wantsExcel ? 'excel' : 'pdf'; // Default to PDF
        
        // Detect report type
        if (/giya\s*aurudde|past\s*year|last\s*year/i.test(msg) || 
            (requestedYear && requestedYear < new Date().getFullYear())) {
            return {
                matched: true,
                reportType: 'yearlySales',
                params: {
                    year: requestedYear || new Date().getFullYear() - 1,
                    category: requestedCategory,
                    format
                }
            };
        }
        
        if (/tyre\s*(sales|report)|ටයර්/i.test(msg)) {
            return {
                matched: true,
                reportType: 'tyreSales',
                params: {
                    year: requestedYear || new Date().getFullYear(),
                    format
                }
            };
        }
        
        if (/category|catagory|categoty/i.test(msg)) {
            return {
                matched: true,
                reportType: 'categorySales',
                params: {
                    year: requestedYear || new Date().getFullYear(),
                    format
                }
            };
        }
        
        if (/brand\s*(report|sales)/i.test(msg)) {
            return {
                matched: true,
                reportType: 'brandSales',
                params: {
                    year: requestedYear || new Date().getFullYear(),
                    format
                }
            };
        }
        
        if (/monthly|masa|masika/i.test(msg) && requestedYear) {
            return {
                matched: true,
                reportType: 'yearlySales',
                params: {
                    year: requestedYear,
                    category: requestedCategory,
                    format
                }
            };
        }
        
        if (/top\s*sell|best\s*sell|honda\s*yanne/i.test(msg)) {
            return {
                matched: true,
                reportType: 'topSellers',
                params: {
                    year: requestedYear || new Date().getFullYear(),
                    format
                }
            };
        }
        
        if (/stock\s*adu|low\s*stock/i.test(msg)) {
            return {
                matched: true,
                reportType: 'lowStock',
                params: { format }
            };
        }

        // No direct match - let AI handle it
        return { matched: false };
    }

    /**
     * Generate the requested report
     */
    async generateReport(reportType, params) {
        console.log(`📊 Generating ${reportType} report with params:`, params);
        
        try {
            switch (reportType) {
                case 'yearlySales':
                    return await this._generateYearlySalesReport(params);
                
                case 'tyreSales':
                    return await this._generateTyreSalesReport(params);
                
                case 'categorySales':
                    return await this._generateCategorySalesReport(params);
                
                case 'brandSales':
                    return await this._generateBrandSalesReport(params);
                
                case 'topSellers':
                    return await this._generateTopSellersReport(params);
                
                case 'lowStock':
                    return await this._generateLowStockReport(params);
                
                default:
                    throw new Error(`Unknown report type: ${reportType}`);
            }
        } catch (error) {
            console.error(`❌ Report generation error:`, error);
            throw error;
        }
    }

    async _generateYearlySalesReport(params) {
        const { year, category, format } = params;

        // Fetch shared datasets
        const [monthlyData, categoryData, monthlyCategoryData, monthlyBrandData] = await Promise.all([
            executeSafely(SQL_QUERIES.yearlySalesMonthlyProfit(year, category)),
            executeSafely(SQL_QUERIES.yearlySalesByCategoryWithProfit(year, category)),
            executeSafely(SQL_QUERIES.yearlyMonthlyCategoryWithProfit(year, category)),
            executeSafely(SQL_QUERIES.yearlyMonthlyBrandWithProfit(year))
        ]);
        
        let filepath;
        if (format === 'excel') {
            const itemAggData = await executeSafely(SQL_QUERIES.yearlyItemAggregatesWithProfit(year, category));
            filepath = await this.excelGenerator.generateYearlySalesReport(year, monthlyData, categoryData, {
                monthlyCategoryData,
                monthlyBrandData,
                itemAggData,
                categoryFilter: category || null
            });
        } else {
            // For PDF, use the detailed transaction log
            const salesLogData = await executeSafely(SQL_QUERIES.yearlySalesLogWithProfit(year, category));
            filepath = await this.pdfGenerator.generateYearlySalesReport(year, monthlyData, categoryData, {
                monthlyCategoryData,
                monthlyBrandData,
                itemAggData: salesLogData // Pass log data as itemAggData (variable name reuse)
            });
        }

        
        // Calculate summary for message
        const totalAmount = monthlyData.reduce((sum, row) => sum + (row.TotalRevenue || 0), 0);
        const totalQty = monthlyData.reduce((sum, row) => sum + (row.TotalQty || 0), 0);
        const totalProfit = monthlyData.reduce((sum, row) => sum + (row.TotalProfit || 0), 0);
        const profitMargin = totalAmount > 0 ? (totalProfit * 100.0 / totalAmount) : 0;
        
        return {
            filepath,
            filename: path.basename(filepath),
            summary: {
                year,
                totalRevenue: formatCurrency(totalAmount),
                totalProfit: formatCurrency(totalProfit),
                profitMargin: `${profitMargin.toFixed(1)}%`,
                totalQuantity: formatNumber(totalQty),
                category: category || 'All Categories'
            },
            message: `📊 *${year} Sales Report*\n\n` +
                     `💰 Total Revenue: ${formatCurrency(totalAmount)}\n` +
                     `📈 Total Profit: ${formatCurrency(totalProfit)}\n` +
                     `📌 Profit Margin: ${profitMargin.toFixed(1)}%\n` +
                     `📦 Total Quantity: ${formatNumber(totalQty)}\n` +
                     `📁 Category: ${category || 'All Categories'}\n\n` +
                     `✅ Report generated successfully!`
        };
    }

    async _generateTyreSalesReport(params) {
        const { year, format } = params;
        
        const monthlyData = await executeSafely(SQL_QUERIES.tyreSalesByMonth(year));
        const brandData = await executeSafely(SQL_QUERIES.brandSales(year));
        
        let filepath;
        if (format === 'excel') {
            filepath = await this.excelGenerator.generateTyreSalesReport(year, monthlyData, brandData);
        } else {
            filepath = await this.pdfGenerator.generateTyreSalesReport(year, monthlyData, brandData);
        }
        
        const totalAmount = monthlyData.reduce((sum, row) => sum + (row.TotalAmount || 0), 0);
        const totalQty = monthlyData.reduce((sum, row) => sum + (row.TotalQty || 0), 0);
        
        return {
            filepath,
            filename: path.basename(filepath),
            summary: {
                year,
                totalRevenue: formatCurrency(totalAmount),
                totalTyresSold: formatNumber(totalQty)
            },
            message: `🛞 *${year} Tyre Sales Report*\n\n` +
                     `💰 Total Tyre Revenue: ${formatCurrency(totalAmount)}\n` +
                     `📦 Total Tyres Sold: ${formatNumber(totalQty)}\n\n` +
                     `✅ Report generated successfully!`
        };
    }

    async _generateCategorySalesReport(params) {
        const { year, format } = params;
        
        const monthlyData = await executeSafely(SQL_QUERIES.yearlySalesByMonth(year));
        const categoryData = await executeSafely(SQL_QUERIES.yearlySalesByCategory(year));
        
        let filepath;
        if (format === 'excel') {
            filepath = await this.excelGenerator.generateYearlySalesReport(year, monthlyData, categoryData);
        } else {
            filepath = await this.pdfGenerator.generateYearlySalesReport(year, monthlyData, categoryData);
        }
        
        return {
            filepath,
            filename: path.basename(filepath),
            message: `📦 *${year} Category Sales Report*\n\n` +
                     `Top Categories:\n` +
                     categoryData.slice(0, 5).map((c, i) => 
                         `${i + 1}. ${c.Category}: ${formatCurrency(c.TotalAmount)}`
                     ).join('\n') +
                     `\n\n✅ Full report attached!`
        };
    }

    async _generateBrandSalesReport(params) {
        const { year, format } = params;
        
        const brandData = await executeSafely(SQL_QUERIES.brandSales(year));
        const monthlyData = await executeSafely(SQL_QUERIES.tyreSalesByMonth(year));
        
        let filepath;
        if (format === 'excel') {
            filepath = await this.excelGenerator.generateTyreSalesReport(year, monthlyData, brandData);
        } else {
            filepath = await this.pdfGenerator.generateTyreSalesReport(year, monthlyData, brandData);
        }
        
        return {
            filepath,
            filename: path.basename(filepath),
            message: `🏷️ *${year} Brand Sales Report*\n\n` +
                     `Top Brands:\n` +
                     brandData.slice(0, 5).map((b, i) => 
                         `${i + 1}. ${b.Brand}: ${formatCurrency(b.TotalRevenue)}`
                     ).join('\n') +
                     `\n\n✅ Full report attached!`
        };
    }

    async _generateTopSellersReport(params) {
        const { year, format } = params;
        
        const topItems = await executeSafely(SQL_QUERIES.topSellingItems(year, 50));
        
        // For now, return a text summary (can add PDF/Excel later)
        return {
            filepath: null,
            message: `🏆 *Top Selling Items ${year}*\n\n` +
                     topItems.slice(0, 10).map((item, i) => 
                         `${i + 1}. ${item.ItemName.substring(0, 30)}...\n` +
                         `   Qty: ${formatNumber(item.TotalQty)} | Rev: ${formatCurrency(item.TotalRevenue)}`
                     ).join('\n\n')
        };
    }

    async _generateLowStockReport(params) {
        const lowStockItems = await executeSafely(SQL_QUERIES.lowStock(4));
        
        return {
            filepath: null,
            message: `⚠️ *Low Stock Alert*\n\n` +
                     `Items with stock < 4:\n\n` +
                     lowStockItems.slice(0, 15).map(item => 
                         `• ${item.ItemName.substring(0, 35)}...\n` +
                         `  Stock: ${item.CurrentStock} | ${item.Category}`
                     ).join('\n\n') +
                     `\n\n📊 Total low stock items: ${lowStockItems.length}`
        };
    }
}

// Export singleton instance
module.exports = new SmartReportGenerator();
