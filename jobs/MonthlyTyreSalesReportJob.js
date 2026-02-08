// MonthlyTyreSalesReportJob.js
// Monthly tyre sales report with tyre-only filtering (excluding services, katta, rebuilt tyres)
const moment = require('moment');
const sqlConfig = require('../sqlConfig');
const { getAdvancedCostPrice } = require('../utils/getCostPrice');

async function getMonthlyTyreSalesReport(sql, monthRange = null) {
    if (!monthRange) {
        // Default to last month
        const now = moment();
        monthRange = {
            start: now.clone().subtract(1, 'months').startOf('month').toDate(),
            end: now.clone().subtract(1, 'months').endOf('month').toDate()
        };
    }

    let pool = null;
    try {
        // Create a new connection using the config
        pool = await new sql.ConnectionPool(sqlConfig).connect();
        
        // Get sales data using the view
        const salesQuery = `
            SELECT 
                si.Description,
                SUM(si.Qty) as TotalQty,
                SUM(si.Qty * (si.UnitPrice * (1 - ISNULL(si.LineDiscountPercentage,0)/100))) as TotalAmount,
                m.UnitCost as CurrentCost,
                m.Categoty as Category,
                ISNULL(w.QTY, 0) as CurrentStock
            FROM [View_Sales report whatsapp] si
            LEFT JOIN [View_Item Master Whatsapp] m ON m.ItemDescription = si.Description
            LEFT JOIN [View_Item Whse Whatsapp] w ON w.ItemID = m.ItemID
            WHERE 
                si.InvoiceDate >= @start 
                AND si.InvoiceDate <= @end
                AND (si.Categoty = 'TYRES' OR m.Categoty = 'TYRES')
            GROUP BY 
                si.Description,
                m.UnitCost,
                m.Categoty,
                ISNULL(w.QTY, 0)
            ORDER BY TotalAmount DESC
        `;

        const request = pool.request();
        request.input('start', sql.DateTime, monthRange.start);
        request.input('end', sql.DateTime, monthRange.end);
        const result = await request.query(salesQuery
        );

        // Process results and add profit calculations
        let processedResults = [];
        let totalRevenue = 0;
        let totalProfit = 0;
        let totalQuantity = 0;

        const MONTHLY_PROFIT_INCLUDE = process.env.MONTHLY_PROFIT_INCLUDE === '1' || process.env.REPORT_INCLUDE_PROFIT === '1';

        for (const row of result.recordset) {
            const costPrice = MONTHLY_PROFIT_INCLUDE ? await getAdvancedCostPrice(sql, row.Description) : null;
            const profit = costPrice ? (row.TotalAmount - (costPrice * row.TotalQty)) : null;

            processedResults.push({
                description: row.Description,
                quantity: row.TotalQty,
                revenue: row.TotalAmount,
                costPrice: costPrice,
                profit: profit,
                category: row.Category || 'Unknown',
                subCategory: row.SubCategory || 'Unknown',
                currentStock: row.CurrentStock || 0
            });

            totalRevenue += row.TotalAmount;
            totalQuantity += row.TotalQty;
            if (profit) totalProfit += profit;
        }

        return {
            sales: processedResults,
            summary: {
                totalRevenue,
                totalProfit: MONTHLY_PROFIT_INCLUDE ? totalProfit : null,
                totalQuantity,
                period: {
                    start: monthRange.start,
                    end: monthRange.end
                }
            }
        };
    } finally {
        if (pool) {
            try {
                await pool.close();
            } catch (err) {
                console.error('Error closing SQL pool:', err);
            }
        }
    }
}

// Category analysis
async function analyzeCategoryData(salesData) {
    const categories = {};
    const MONTHLY_PROFIT_INCLUDE = process.env.MONTHLY_PROFIT_INCLUDE === '1' || process.env.REPORT_INCLUDE_PROFIT === '1';
    
    // Process by category
    for (const sale of salesData) {
        const category = sale.category || 'Unknown';
        if (!categories[category]) {
            categories[category] = {
                quantity: 0,
                revenue: 0,
                profit: 0,
                items: new Set()
            };
        }
        
        categories[category].quantity += sale.quantity;
        categories[category].revenue += sale.revenue;
        if (sale.profit) categories[category].profit += sale.profit;
        categories[category].items.add(sale.description);
    }

    // Convert to array and sort by revenue
    return Object.entries(categories)
        .map(([name, data]) => ({
            name,
            quantity: data.quantity,
            revenue: data.revenue,
            profit: MONTHLY_PROFIT_INCLUDE ? data.profit : null,
            uniqueItems: data.items.size
        }))
        .sort((a, b) => b.revenue - a.revenue);
}

// Message handler for manual report generation
async function handleMonthlyReportMessage(msg, client, sql, logAndSave) {
    const text = msg.body.toLowerCase();
    const isOldFormat = text === '!monthly' || text.startsWith('!monthly ');
    const isNewFormat = text.match(/^sales report \d{4}\/\d{2}$/);
    
    if (!isOldFormat && !isNewFormat) {
        return false;
    }

    try {
        let monthRange = null;
        let dateStr = '';
        
        if (isNewFormat) {
            // Parse "sales report YYYY/MM" format
            dateStr = text.split('sales report ')[1]; // Gets "YYYY/MM"
        } else if (text.startsWith('!monthly ')) {
            // Parse old "!monthly YYYY-MM" or "!monthly YYYY/MM" format
            dateStr = text.split(' ')[1];
        }
        
        if (dateStr) {
            // Support both / and - separators
            const date = moment(dateStr.replace('/', '-'), 'YYYY-MM');
            if (!date.isValid()) {
                await client.sendMessage(msg.from, 'Invalid date format. Use YYYY/MM (e.g., sales report 2025/09)');
                return true;
            }
            monthRange = {
                start: date.startOf('month').toDate(),
                end: date.endOf('month').toDate()
            };
        }

        // Run the report job
        const result = await mainJob(sql, 
            async (type, content) => await client.sendMessage(msg.from, content),
            logAndSave,
            { monthRange }
        );

        if (!result.success) {
            await client.sendMessage(msg.from, `Error generating report: ${result.error}`);
        }

        return true;
    } catch (error) {
        console.error('Error in monthly report handler:', error);
        await client.sendMessage(msg.from, 'Error generating monthly report. Please try again later.');
        return true;
    }
}

// Main job function
const mainJob = async function MonthlyTyreSalesReportJob(sql, sendWhatsAppMessage, logAndSave, options = {}) {
    // Handle undefined logAndSave
    const logFn = typeof logAndSave === 'function' ? logAndSave : console.log;

    // Defensive check for SQL config
    if (!sqlConfig || typeof sqlConfig.server !== 'string' || !sqlConfig.server) {
        const errMsg = 'SQL config is missing required "server" property. Please check your .env or sqlConfig.js.';
        logFn(errMsg);
        return { success: false, error: errMsg };
    }

    try {
        // Generate report
        const monthRange = options.monthRange || null;
        const report = await getMonthlyTyreSalesReport(sql, monthRange);

        // Environment variables for report configuration
        const MONTHLY_PROFIT_INCLUDE = process.env.MONTHLY_PROFIT_INCLUDE === '1' || process.env.REPORT_INCLUDE_PROFIT === '1';
        const MONTHLY_SHOW_COST = process.env.MONTHLY_SHOW_COST === '1' || process.env.REPORT_SHOW_COST === '1';
        
        // Analyze categories
        const categoryAnalysis = await analyzeCategoryData(report.sales);

        // Format message
        let message = ` *MONTHLY TYRE SALES REPORT*\n`;
        message += ` ${moment(report.summary.period.start).format('MMMM YYYY')}\n\n`;
        
        // Summary section
        message += ` *SUMMARY*\n`;
        message += ` Total Sales: ${report.summary.totalQuantity} tyres\n`;
        message += ` Revenue: Rs. ${Math.round(report.summary.totalRevenue).toLocaleString()}\n`;
        if (MONTHLY_PROFIT_INCLUDE) {
            message += ` Profit: Rs. ${Math.round(report.summary.totalProfit).toLocaleString()}\n`;
        }
        message += `\n`;

        // Top selling items
        message += ` *TOP SELLING ITEMS*\n`;
        const topItems = report.sales
            .sort((a, b) => b.quantity - a.quantity)
            .slice(0, 5);

        topItems.forEach((item, index) => {
            message += `${index + 1}. ${item.description}\n`;
            message += `   Qty: ${item.quantity} | Rs. ${Math.round(item.revenue).toLocaleString()}\n`;
            if (MONTHLY_SHOW_COST && item.costPrice) {
                message += `   Cost: Rs. ${Math.round(item.costPrice).toLocaleString()}\n`;
            }
            message += `   Stock: ${item.currentStock}\n`;
        });
        message += `\n`;

        // Category analysis
        message += ` *CATEGORY BREAKDOWN*\n`;
        categoryAnalysis.forEach(cat => {
            message += ` ${cat.name}\n`;
            message += `   Items: ${cat.uniqueItems} | Qty: ${cat.quantity}\n`;
            message += `   Revenue: Rs. ${Math.round(cat.revenue).toLocaleString()}\n`;
            if (MONTHLY_PROFIT_INCLUDE) {
                message += `   Profit: Rs. ${Math.round(cat.profit).toLocaleString()}\n`;
            }
        });

        // Send report
        await sendWhatsAppMessage('report', message);
        logFn('Monthly tyre sales report sent successfully');

        return {
            success: true,
            message: 'Report sent successfully'
        };

    } catch (error) {
        console.error('Error in MonthlyTyreSalesReportJob:', error);
        logFn('Error generating monthly tyre sales report: ' + error.message);
        return {
            success: false,
            error: error.message
        };
    }
}

module.exports = Object.assign(mainJob, { handleMonthlyReportMessage });
