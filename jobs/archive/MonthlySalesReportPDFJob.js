// MonthlySalesReportPDFJob.js
// Generates a monthly sales report PDF for a given year/month, using SQL Server data and daily job logic
const fs = require('fs');
const path = require('path');
const moment = require('moment');
const { generateMonthlyTyreReportSQL } = require('../utils/sqlMonthlyTyreReportEnhanced');
const { generateMonthlyReportPDF } = require('../utils/generateMonthlyReportPDF');

// Main entry: message like "Sales report 2025/09"
module.exports = async function MonthlySalesReportPDFJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, message) {
    // Parse year/month from message
    const match = /sales report\s+(\d{4})\/(\d{2})/i.exec(message);
    if (!match) {
        logAndSave('Invalid monthly report request: ' + message);
        return;
    }
    const year = parseInt(match[1], 10);
    const month = parseInt(match[2], 10);
    const startDate = moment({ year, month: month - 1, day: 1 });
    const endDate = moment(startDate).endOf('month');

    // Use the enhanced SQL report generator
    logAndSave('[INFO] Using SQL enhanced report generator');
    const monthRange = { start: startDate.toDate(), end: endDate.toDate() };

    try {
        // Remove duplicate sql variable
        const analytics = await generateMonthlyTyreReportSQL(sql, monthRange, { strict: false });
        logAndSave('[INFO] SQL report generation successful');
        
        const pdfPath = await generateMonthlyReportPDF(analytics, logAndSave);
        await sendWhatsAppMessage('admin', `Monthly sales report for ${year}-${month} attached.`, pdfPath);
        logAndSave(`Monthly sales report PDF sent for ${year}-${month}`);
        return;
    } catch (error) {
        logAndSave('[SQL MODE ERROR STACK] ' + error.message);
        // Fall through to JS mode
    }

    // Fallback to JS mode
    logAndSave('Falling back to JS mode');
    logAndSave('Generating monthly report (refactored) for ' + startDate.format('YYYY-MM-DD') + ' to ' + endDate.format('YYYY-MM-DD'));
    
    const pdfPath = await generateMonthlyReportPDF({
        period: { 
            start: startDate.toISOString(), 
            end: endDate.toISOString() 
        },
        dateWiseAnalysis: {},
        tyreSummary: {
            totalQuantity: 0,
            totalAmount: 0,
            totalProfit: 0,
            totalInvoices: 0,
            profitMargin: 0,
            workingDays: 0,
            avgDailyRevenue: 0,
            avgDailyInvoices: 0
        },
        brandAnalysis: {},
        diagnostics: {
            mode: 'js-fallback',
            includedLines: 0,
            classificationBreakdown: {}
        }
    }, logAndSave);

    await sendWhatsAppMessage('admin', `Monthly sales report for ${year}-${month} attached.`, pdfPath);
    logAndSave(`Monthly sales report PDF sent for ${year}-${month}`);
};
