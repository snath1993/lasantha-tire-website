const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const moment = require('moment');

async function generateMonthlyReportPDF(analytics, logAndSave = console.log) {
    try {
        logAndSave('[PDF] Starting PDF generation');
        
        // Input validation
        if (!analytics) {
            throw new Error('Analytics object is null or undefined');
        }
        if (!analytics.period) {
            throw new Error('Analytics missing period data');
        }
        if (!analytics.tyreSummary) {
            throw new Error('Analytics missing tyreSummary data');
        }
        
        logAndSave('[PDF] Input validation passed');
        
        // Initialize required structures with defaults
        const defaultAnalytics = {
            period: {
                start: new Date(),
                end: new Date()
            },
            tyreSummary: {
                totalQuantity: 0,
                totalAmount: 0,
                totalProfit: 0,
                totalInvoices: 0,
                profitMargin: 0,
                workingDays: 0,
                avgDailyRevenue: 0,
                avgDailyProfit: 0,
                avgDailyInvoices: 0
            },
            dateWiseAnalysis: {}
        };

        // Deep merge with defaults to ensure all properties exist
        analytics = {
            ...defaultAnalytics,
            ...analytics,
            tyreSummary: {
                ...defaultAnalytics.tyreSummary,
                ...(analytics.tyreSummary || {})
            },
            dateWiseAnalysis: {
                ...(analytics.dateWiseAnalysis || {})
            }
        };
        
        const { period, tyreSummary, dateWiseAnalysis = {} } = analytics;
        const monthName = moment(period.start).format('MMMM_YYYY');
        const outputDir = path.join(process.env.QUOTE_DIR || './quotations', 'monthly-reports');
        
        if (!fs.existsSync(outputDir)) {
            fs.mkdirSync(outputDir, { recursive: true });
        }
        
        const filename = `Monthly_Report_${monthName}.pdf`;
        const filePath = path.join(outputDir, filename);
        
        const doc = new PDFDocument();
        const stream = fs.createWriteStream(filePath);
        doc.pipe(stream);
        
        // Title
        doc.fontSize(20).text('Monthly Tyre Sales Report', 50, 50);
        doc.fontSize(16).text(monthName, 50, 80);
        
        let y = 120;
        
        // Summary
        doc.fontSize(14).text('SUMMARY', 50, y);
        y += 30;
        
        // Ensure all summary values have defaults and formatting
        const numberFormatter = new Intl.NumberFormat('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
        
        doc.fontSize(14)
           .text(`Total Revenue: Rs. ${numberFormatter.format(tyreSummary.totalAmount || 0)}`, 50, y);
        y += 20;
        doc.text(`Total Profit: Rs. ${numberFormatter.format(tyreSummary.totalProfit || 0)}`, 50, y);
        y += 20;
        doc.text(`Total Quantity: ${numberFormatter.format(tyreSummary.totalQuantity || 0)} tyres`, 50, y);
        y += 20;
        doc.text(`Total Invoices: ${numberFormatter.format(tyreSummary.totalInvoices || 0)}`, 50, y);
        y += 20;
        const profitMarginNum = tyreSummary && typeof tyreSummary.profitMargin === 'number' ? tyreSummary.profitMargin : 
                            tyreSummary && typeof tyreSummary.profitMargin === 'string' ? parseFloat(tyreSummary.profitMargin) : 0;
        doc.text(`Profit Margin: ${profitMarginNum.toFixed(1)}%`, 50, y);
        y += 20;
        doc.text(`Average Daily Revenue: Rs. ${numberFormatter.format(tyreSummary.avgDailyRevenue || 0)}`, 50, y);
        y += 20;
        doc.text(`Working Days: ${tyreSummary.workingDays || 0}`, 50, y);
        y += 20;
        doc.text(`Average Daily Profit: Rs. ${numberFormatter.format(tyreSummary.avgDailyProfit || 0)}`, 50, y);
        y += 20;
        doc.text(`Average Daily Invoices: ${(tyreSummary.avgDailyInvoices || 0).toFixed(1)}`, 50, y);
        y += 40;
        
        // Daily breakdown
        doc.fontSize(14).text('DAILY BREAKDOWN', 50, y);
        y += 30;

        // Default item structure
        const defaultDayData = {
            totalSales: 0,
            totalProfit: 0,
            totalQty: 0,
            invoices: 0,
            items: []
        };
        
        // Ensure dateWiseAnalysis exists and is an object
        const validDates = Object.keys(dateWiseAnalysis || {})
            .filter(date => dateWiseAnalysis[date] && Array.isArray(dateWiseAnalysis[date].items))
            .sort();

        validDates.forEach(date => {
            const rawData = dateWiseAnalysis[date] || {};
            const data = {
                ...defaultDayData,
                ...rawData,
                items: Array.isArray(rawData.items) ? rawData.items : []
            };
            
            // Skip if no items
            if (data.items.length === 0) return;
            
            if (y > 700) {
                doc.addPage();
                y = 50;
            }
            
            doc.fontSize(12).text(moment(date).format('YYYY-MM-DD'), 50, y, {underline: true});
            y += 20;
            
            // Header for details with adjusted spacing and alignment
            doc.fontSize(9)
               .text('Description', 50, y, { width: 240 })
               .text('Qty', 300, y, { width: 40, align: 'right' })
               .text('Unit Price', 350, y, { width: 70, align: 'right' })
               .text('Total', 430, y, { width: 70, align: 'right' })
               .text('Profit', 510, y, { width: 70, align: 'right' });
            y += 15;
            
            // Filter out invalid items and ensure all properties exist
            const validItems = (data.items || [])
                .filter(item => item && typeof item === 'object' && item.Description && Number(item.Qty || 0) > 0);
            
            validItems.forEach(rawItem => {
                if (y > 700) {
                    doc.addPage();
                    y = 50;
                }
                
                try {
                    // Debug: Log raw item values
                    console.log('\nProcessing item:', rawItem.Description);
                    console.log('Raw values:', {
                        Qty: rawItem.Qty,
                        UnitPrice: rawItem.UnitPrice,
                        EffectiveCostPrice: rawItem.EffectiveCostPrice,
                        LineTotal: rawItem.LineTotal,
                        Profit: rawItem.Profit
                    });

                    // Ensure each item has all required properties with proper defaults
                    const item = {
                        Description: String(rawItem.Description || '').trim(),
                        Qty: Math.max(0, Number(rawItem.Qty || 0)),
                        UnitPrice: Math.max(0, Number(rawItem.UnitPrice || 0)),
                        LineTotal: Math.max(0, Number(rawItem.LineTotal || 0)),
                        CostPrice: Math.max(0, Number(rawItem.EffectiveCostPrice || 0)),
                        // Use the profit directly from the SQL calculation
                        Profit: Math.max(0, Number(rawItem.Profit || 0))
                    };
                    
                    // Debug: Log calculated values
                    console.log('Calculated values:', {
                        Qty: item.Qty,
                        UnitPrice: item.UnitPrice,
                        CostPrice: item.CostPrice,
                        LineTotal: item.LineTotal,
                        Profit: item.Profit,
                        ExpectedProfit: (item.UnitPrice - item.CostPrice) * item.Qty
                    });
                    
                    doc.fontSize(8)
                       .text(item.Description, 50, y, { width: 240, ellipsis: true })
                       .text(item.Qty.toString(), 300, y, { width: 40, align: 'right' })
                       .text(`Rs. ${numberFormatter.format(item.UnitPrice)}`, 350, y, { width: 70, align: 'right' })
                       .text(`Rs. ${numberFormatter.format(item.LineTotal)}`, 430, y, { width: 70, align: 'right' })
                       .text(`Rs. ${numberFormatter.format(item.Profit)}`, 510, y, { width: 70, align: 'right' });
                    y += 12;
                } catch (itemError) {
                    logAndSave(`[WARNING] Error processing item: ${JSON.stringify(rawItem)}`);
                    logAndSave(itemError.message);
                }
            });
            
            // Add a separator line
            y += 5;
            doc.moveTo(50, y).lineTo(580, y).stroke();
            
            // Subtotal for the day with safe number handling
            y += 5;
            doc.fontSize(9)
               .text('Day Total:', 50, y)
               .text(`Quantity: ${numberFormatter.format(Number(data.totalQty || 0))}`, 300, y, { width: 40, align: 'right' })
               .text(`Sales: Rs. ${numberFormatter.format(Number(data.totalSales || 0))}`, 430, y, { width: 70, align: 'right' })
               .text(`Profit: Rs. ${numberFormatter.format(Number(data.totalProfit || 0))}`, 510, y, { width: 70, align: 'right' });
            
            y += 20;
        });
        
                doc.end();
        
        // Wait for the PDF to finish writing
        await new Promise((resolve, reject) => {
            stream.on('finish', () => {
                logAndSave('[PDF] Successfully generated PDF: ' + filename);
                resolve(filePath);
            });
            stream.on('error', reject);
        });
        
        return filePath;
        
    } catch (error) {
        logAndSave('[PDF ERROR] ' + error.message);
        if (error.stack) {
                logAndSave('[PDF ERROR STACK] ' + error.stack.split('\n').slice(0, 3).join(' | '));
        }
        throw error;
    }
}

module.exports = { generateMonthlyReportPDF };
