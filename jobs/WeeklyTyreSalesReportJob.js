// jobs/WeeklyTyreSalesReportJob.js
// Weekly tyre sales report with PDF generation capability
const path = require('path');
const fs = require('fs');
const moment = require('moment');
const { generateSalesReportPDF } = require('../utils/generateSalesReportPDF');

function initDb() {
    const quoteDir = process.env.QUOTE_DIR || './quotations';
    const dbPath = path.join(quoteDir, 'sales-reports.db');
    
    if (!fs.existsSync(quoteDir)) {
        fs.mkdirSync(quoteDir, { recursive: true });
    }
    
    const Database = require('sqlite3').Database;
    const db = new Database(dbPath);
    
    // Create weekly reports table
    db.serialize(() => {
        db.run(`CREATE TABLE IF NOT EXISTS weekly_reports (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            week_start DATE NOT NULL,
            week_end DATE NOT NULL,
            total_invoices INTEGER DEFAULT 0,
            total_amount DECIMAL(15,2) DEFAULT 0,
            total_profit DECIMAL(15,2) DEFAULT 0,
            top_brand TEXT,
            report_data TEXT,
            pdf_path TEXT,
            generated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            UNIQUE(week_start, week_end)
        )`);
    });
    
    return db;
}

function getWeekRange(date = new Date()) {
    const start = moment(date).startOf('week').format('YYYY-MM-DD');
    const end = moment(date).endOf('week').format('YYYY-MM-DD');
    return { start, end };
}

function getPreviousWeek() {
    const lastWeek = moment().subtract(1, 'week');
    return getWeekRange(lastWeek.toDate());
}

async function generateWeeklyReport(sql, sqlConfig, weekRange, logAndSave, generatePDF = false) {
    try {
        await sql.connect(sqlConfig);
        
        const { start, end } = weekRange;
        logAndSave(`Generating weekly report for ${start} to ${end}`);
        
        // Main sales query with enhanced details
        const query = `
            SELECT 
                InvoiceNo,
                InvoiceDate,
                CustomerName,
                Description,
                Qty,
                UnitPrice,
                LineDiscountPercentage,
                (Qty * (UnitPrice * (1 - ISNULL(LineDiscountPercentage,0)/100))) as LineTotal,
                UnitCost,
                ((Qty * (UnitPrice * (1 - ISNULL(LineDiscountPercentage,0)/100))) - (Qty * ISNULL(UnitCost, 0))) as LineProfit
            FROM [View_Sales report whatsapp]
            WHERE InvoiceDate >= '${start}' 
                AND InvoiceDate <= '${end}'
                AND Categoty = 'TYRES'
            ORDER BY InvoiceDate DESC, InvoiceNo
        `;
        
        const result = await sql.query(query);
        const invoices = result.recordset;
        
        // Calculate summary statistics
        const totalInvoices = new Set(invoices.map(inv => inv.InvoiceNo)).size;
        const totalAmount = invoices.reduce((sum, inv) => sum + (inv.LineTotal || 0), 0);
        const totalProfit = invoices.reduce((sum, inv) => sum + (inv.LineProfit || 0), 0);
        
        // Brand analysis
        const brandCounts = {};
        invoices.forEach(inv => {
            const desc = inv.Description || '';
            const brand = extractBrandFromDescription(desc);
            brandCounts[brand] = (brandCounts[brand] || 0) + inv.Qty;
        });
        
        const topBrand = Object.keys(brandCounts).reduce((a, b) => 
            brandCounts[a] > brandCounts[b] ? a : b, 'Unknown');
        
        // Daily breakdown
        const dailyBreakdown = {};
        invoices.forEach(inv => {
            const day = moment(inv.InvoiceDate).format('YYYY-MM-DD');
            if (!dailyBreakdown[day]) {
                dailyBreakdown[day] = { invoices: 0, amount: 0, qty: 0 };
            }
            dailyBreakdown[day].amount += inv.LineTotal || 0;
            dailyBreakdown[day].qty += inv.Qty || 0;
        });
        
        // Count unique invoices per day
        const invoicesByDay = {};
        invoices.forEach(inv => {
            const day = moment(inv.InvoiceDate).format('YYYY-MM-DD');
            if (!invoicesByDay[day]) invoicesByDay[day] = new Set();
            invoicesByDay[day].add(inv.InvoiceNo);
        });
        
        Object.keys(invoicesByDay).forEach(day => {
            if (dailyBreakdown[day]) {
                dailyBreakdown[day].invoices = invoicesByDay[day].size;
            }
        });
        
        const reportData = {
            period: { start, end },
            summary: {
                totalInvoices,
                totalAmount: Math.round(totalAmount),
                totalProfit: Math.round(totalProfit),
                profitMargin: totalAmount > 0 ? ((totalProfit / totalAmount) * 100).toFixed(1) : 0,
                topBrand,
                averageInvoiceValue: totalInvoices > 0 ? Math.round(totalAmount / totalInvoices) : 0
            },
            dailyBreakdown,
            brandAnalysis: brandCounts,
            topSellingItems: getTopSellingItems(invoices),
            generatedAt: new Date().toISOString()
        };
        
        // Save to database
        const db = initDb();
        await new Promise((resolve, reject) => {
            db.run(`INSERT OR REPLACE INTO weekly_reports 
                (week_start, week_end, total_invoices, total_amount, total_profit, top_brand, report_data, pdf_path) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
                [start, end, totalInvoices, totalAmount, totalProfit, topBrand, JSON.stringify(reportData), null],
                function(err) {
                    if (err) reject(err);
                    else resolve(this.lastID);
                }
            );
        });
        db.close();
        
        // Generate PDF if requested
        let pdfPath = null;
        if (generatePDF) {
            pdfPath = await generateWeeklyReportPDF(reportData, logAndSave);
        }
        
        return { reportData, pdfPath };
        
    } catch (error) {
        logAndSave(`Weekly report generation error: ${error.message}`);
        throw error;
    } finally {
        await sql.close();
    }
}

function extractBrandFromDescription(desc) {
    const brands = ['Bridgestone', 'Michelin', 'Yokohama', 'Continental', 'Pirelli', 'Dunlop', 'Goodyear'];
    const upperDesc = desc.toUpperCase();
    
    for (const brand of brands) {
        if (upperDesc.includes(brand.toUpperCase())) {
            return brand;
        }
    }
    return 'Other';
}

function getTopSellingItems(invoices) {
    const itemCounts = {};
    
    invoices.forEach(inv => {
        const key = inv.Description || 'Unknown';
        if (!itemCounts[key]) {
            itemCounts[key] = { description: key, qty: 0, revenue: 0 };
        }
        itemCounts[key].qty += inv.Qty || 0;
        itemCounts[key].revenue += inv.LineTotal || 0;
    });
    
    return Object.values(itemCounts)
        .sort((a, b) => b.qty - a.qty)
        .slice(0, 10);
}

async function generateWeeklyReportPDF(reportData, logAndSave) {
    try {
        const quoteDir = process.env.QUOTE_DIR || './quotations';
        const reportsDir = path.join(quoteDir, 'weekly-reports');
        
        if (!fs.existsSync(reportsDir)) {
            fs.mkdirSync(reportsDir, { recursive: true });
        }
        
        const { start, end } = reportData.period;
        const filename = `Weekly_Report_${start}_to_${end}.pdf`;
        const pdfPath = path.join(reportsDir, filename);
        
        // Use the PDF generation utility
        await generateSalesReportPDF(reportData, pdfPath, 'weekly');
        
        logAndSave(`Weekly report PDF generated: ${filename}`);
        return pdfPath;
        
    } catch (error) {
        logAndSave(`Weekly PDF generation error: ${error.message}`);
        throw error;
    }
}

// Main export function
module.exports = async function WeeklyTyreSalesReportJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, options = {}) {
    try {
        const { generatePDF = false, weekRange = null } = options;
        const targetWeek = weekRange || getPreviousWeek();
        
        logAndSave(`Starting weekly report job for week ${targetWeek.start} to ${targetWeek.end}`);
        
        const result = await generateWeeklyReport(sql, sqlConfig, targetWeek, logAndSave, generatePDF);
        const { reportData, pdfPath } = result;
        
        // Format WhatsApp message
        const message = formatWeeklyReportMessage(reportData);
        
        // Send to report numbers
        const reportNumbers = getReportNumbers();
        
        for (const number of reportNumbers) {
            try {
                await sendWhatsAppMessage(number, message);
                
                // Send PDF if generated
                if (pdfPath && fs.existsSync(pdfPath)) {
                    // Note: WhatsApp file sending would need additional implementation
                    logAndSave(`PDF ready for ${number}: ${path.basename(pdfPath)}`);
                }
            } catch (sendError) {
                logAndSave(`Failed to send weekly report to ${number}: ${sendError.message}`);
            }
        }
        
        logAndSave(`Weekly report job completed successfully`);
        return true;
        
    } catch (error) {
        logAndSave(`Weekly report job failed: ${error.message}`);
        throw error;
    }
};

function getReportNumbers() {
    try {
        const { getJobContactNumbers } = require('../utils/jobsConfigReader');
        const configNumbers = getJobContactNumbers('WeeklyTyreSalesReportJob');
        if (configNumbers && configNumbers.length > 0) {
            return configNumbers;
        }
    } catch (error) {
        console.warn('Could not load job config, falling back to environment:', error.message);
    }
    
    const numbers = process.env.REPORT_NUMBERS || process.env.DAILY_REPORT_NUMBERS || '';
    return numbers.split(',').map(n => n.trim()).filter(Boolean);
}

function formatWeeklyReportMessage(reportData) {
    const { period, summary, dailyBreakdown } = reportData;
    const weekStart = moment(period.start).format('MMM DD');
    const weekEnd = moment(period.end).format('MMM DD, YYYY');
    
    let message = `📊 *WEEKLY TYRE SALES REPORT*\n`;
    message += `📅 Week: ${weekStart} - ${weekEnd}\n`;
    message += `━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n`;
    
    message += `💰 *SUMMARY*\n`;
    message += `• Total Invoices: ${summary.totalInvoices}\n`;
    message += `• Total Revenue: Rs. ${summary.totalAmount.toLocaleString()}\n`;
    message += `• Total Profit: Rs. ${summary.totalProfit.toLocaleString()}\n`;
    message += `• Profit Margin: ${summary.profitMargin}%\n`;
    message += `• Avg Invoice: Rs. ${summary.averageInvoiceValue.toLocaleString()}\n`;
    message += `• Top Brand: ${summary.topBrand}\n\n`;
    
    message += `📈 *DAILY BREAKDOWN*\n`;
    Object.keys(dailyBreakdown)
        .sort()
        .forEach(day => {
            const dayData = dailyBreakdown[day];
            const dayName = moment(day).format('ddd');
            message += `${dayName} ${moment(day).format('MM/DD')}: ${dayData.invoices} invoices, Rs. ${Math.round(dayData.amount).toLocaleString()}\n`;
        });
    
    message += `\n🕐 Generated: ${moment().format('MMM DD, YYYY HH:mm')}`;
    
    return message;
}

// Export additional functions for external use
module.exports.generateWeeklyReport = generateWeeklyReport;
module.exports.generateWeeklyReportPDF = generateWeeklyReportPDF;
module.exports.getWeekRange = getWeekRange;
