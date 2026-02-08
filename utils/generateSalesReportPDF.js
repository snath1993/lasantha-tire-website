// utils/generateSalesReportPDF.js
// Comprehensive PDF generation utility for sales reports
const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const moment = require('moment');

class SalesReportPDFGenerator {
    constructor() {
        this.doc = null;
        this.currentY = 0;
        this.pageMargin = 50;
        this.pageWidth = 595 - (this.pageMargin * 2); // A4 width minus margins
        
        // Colors
        this.colors = {
            primary: '#2563eb',      // Blue
            secondary: '#64748b',    // Gray
            success: '#059669',      // Green
            warning: '#d97706',      // Orange
            text: '#1f2937',         // Dark gray
            light: '#f8fafc'         // Light gray
        };
    }
    
    createDocument() {
        this.doc = new PDFDocument({
            size: 'A4',
            margin: this.pageMargin,
            info: {
                Title: 'Tyre Sales Report',
                Author: 'WhatsApp Tyre System',
                Subject: 'Sales Analytics Report',
                Creator: 'WhatsApp SQL API'
            }
        });
        
        this.currentY = this.pageMargin;
        return this.doc;
    }
    
    addHeader(title, period, reportType) {
        // Company header
        this.doc.fontSize(24)
            .fillColor(this.colors.primary)
            .text('TYRE SALES REPORT', this.pageMargin, this.currentY, { align: 'center' });
        
        this.currentY += 35;
        
        // Report type and period
        this.doc.fontSize(16)
            .fillColor(this.colors.text)
            .text(title, this.pageMargin, this.currentY, { align: 'center' });
        
        this.currentY += 25;
        
        let periodText = '';
        if (reportType === 'weekly') {
            const start = moment(period.start).format('MMM DD');
            const end = moment(period.end).format('MMM DD, YYYY');
            periodText = `Week: ${start} - ${end}`;
        } else if (reportType === 'monthly') {
            periodText = `Month: ${moment(period.start).format('MMMM YYYY')}`;
        }
        
        this.doc.fontSize(12)
            .fillColor(this.colors.secondary)
            .text(periodText, this.pageMargin, this.currentY, { align: 'center' });
        
        this.currentY += 20;
        
        // Generated timestamp
        this.doc.fontSize(10)
            .fillColor(this.colors.secondary)
            .text(`Generated: ${moment().format('MMMM DD, YYYY HH:mm')}`, this.pageMargin, this.currentY, { align: 'center' });
        
        this.currentY += 40;
        
        // Add separator line
        this.addLine();
    }
    
    addLine(color = this.colors.secondary) {
        this.doc.strokeColor(color)
            .lineWidth(1)
            .moveTo(this.pageMargin, this.currentY)
            .lineTo(this.pageMargin + this.pageWidth, this.currentY)
            .stroke();
        this.currentY += 20;
    }
    
    addSection(title, backgroundColor = this.colors.light) {
        // Check if we need a new page
        if (this.currentY > 700) {
            this.doc.addPage();
            this.currentY = this.pageMargin;
        }
        
        // Section header with background
        this.doc.rect(this.pageMargin, this.currentY - 5, this.pageWidth, 25)
            .fillColor(backgroundColor)
            .fill();
        
        this.doc.fontSize(14)
            .fillColor(this.colors.primary)
            .text(title, this.pageMargin + 10, this.currentY + 5);
        
        this.currentY += 35;
    }
    
    addSummaryBox(summary) {
        this.addSection('📊 EXECUTIVE SUMMARY');
        
        const boxHeight = 120;
        const boxY = this.currentY;
        
        // Summary box background
        this.doc.rect(this.pageMargin, boxY, this.pageWidth, boxHeight)
            .strokeColor(this.colors.primary)
            .lineWidth(2)
            .stroke();
        
        // Two column layout
        const leftX = this.pageMargin + 20;
        const rightX = this.pageMargin + (this.pageWidth / 2) + 10;
        let textY = boxY + 15;
        
        // Left column
        this.doc.fontSize(11).fillColor(this.colors.text);
        this.doc.text(`Total Invoices: ${summary.totalInvoices}`, leftX, textY);
        textY += 18;
        this.doc.text(`Total Revenue: Rs. ${summary.totalAmount.toLocaleString()}`, leftX, textY);
        textY += 18;
        this.doc.text(`Total Profit: Rs. ${summary.totalProfit.toLocaleString()}`, leftX, textY);
        textY += 18;
        this.doc.text(`Profit Margin: ${summary.profitMargin}%`, leftX, textY);
        
        // Right column
        textY = boxY + 15;
        this.doc.text(`Average Invoice: Rs. ${summary.averageInvoiceValue.toLocaleString()}`, rightX, textY);
        textY += 18;
        if (summary.totalQuantity) {
            this.doc.text(`Total Quantity: ${summary.totalQuantity} tyres`, rightX, textY);
            textY += 18;
        }
        if (summary.workingDays) {
            this.doc.text(`Working Days: ${summary.workingDays}`, rightX, textY);
            textY += 18;
            this.doc.text(`Daily Avg Revenue: Rs. ${summary.avgDailyRevenue.toLocaleString()}`, rightX, textY);
        }
        
        this.currentY += boxHeight + 30;
    }
    
    addTable(title, data, columns, options = {}) {
        this.addSection(title);
        
        if (!data || data.length === 0) {
            this.doc.fontSize(10)
                .fillColor(this.colors.secondary)
                .text('No data available', this.pageMargin + 20, this.currentY);
            this.currentY += 30;
            return;
        }
        
        const tableY = this.currentY;
        const rowHeight = 20;
        const colWidth = this.pageWidth / columns.length;
        
        // Table headers
        this.doc.rect(this.pageMargin, tableY, this.pageWidth, rowHeight)
            .fillColor(this.colors.primary)
            .fill();
        
        this.doc.fontSize(10)
            .fillColor('white');
        
        columns.forEach((col, i) => {
            const x = this.pageMargin + (i * colWidth) + 5;
            this.doc.text(col.header, x, tableY + 6, { width: colWidth - 10 });
        });
        
        this.currentY = tableY + rowHeight;
        
        // Table rows
        data.slice(0, 15).forEach((row, index) => {
            const rowY = this.currentY;
            let bgColor = index % 2 === 0 ? 'white' : this.colors.light;
            
            // Check for profit highlighting if profit analysis is enabled
            if (options.highlightProfit && row.profit !== undefined && row.unitPrice !== undefined) {
                const profitMargin = (row.profit / row.unitPrice) * 100;
                
                if (profitMargin > 10) {
                    // High profit - green highlight
                    bgColor = '#dcfce7'; // Light green
                } else if (profitMargin < 0) {
                    // Loss - red highlight
                    bgColor = '#fee2e2'; // Light red
                }
            }
            
            // Row background
            this.doc.rect(this.pageMargin, rowY, this.pageWidth, rowHeight)
                .fillColor(bgColor)
                .fill();
            
            // Row border
            this.doc.rect(this.pageMargin, rowY, this.pageWidth, rowHeight)
                .strokeColor(this.colors.secondary)
                .lineWidth(0.5)
                .stroke();
            
            this.doc.fontSize(9)
                .fillColor(this.colors.text);
            
            columns.forEach((col, i) => {
                const x = this.pageMargin + (i * colWidth) + 5;
                let value = row[col.key] || '';
                
                if (col.format === 'currency') {
                    value = `Rs. ${parseInt(value).toLocaleString()}`;
                } else if (col.format === 'number') {
                    value = parseInt(value).toLocaleString();
                } else if (col.format === 'percentage') {
                    value = `${parseFloat(value).toFixed(1)}%`;
                }
                
                this.doc.text(String(value), x, rowY + 6, { 
                    width: colWidth - 10,
                    ellipsis: true
                });
            });
            
            this.currentY += rowHeight;
        });
        
        this.currentY += 20;
    }
    
    addChart(title, data, type = 'bar') {
        this.addSection(title);
        
        if (!data || Object.keys(data).length === 0) {
            this.doc.fontSize(10)
                .fillColor(this.colors.secondary)
                .text('No data available for chart', this.pageMargin + 20, this.currentY);
            this.currentY += 30;
            return;
        }
        
        const chartY = this.currentY;
        const chartHeight = 150;
        const chartWidth = this.pageWidth - 40;
        const chartX = this.pageMargin + 20;
        
        // Chart background
        this.doc.rect(chartX, chartY, chartWidth, chartHeight)
            .strokeColor(this.colors.secondary)
            .lineWidth(1)
            .stroke();
        
        // Simple bar chart representation
        const entries = Object.entries(data).slice(0, 8);
        const maxValue = Math.max(...entries.map(([_, value]) => typeof value === 'object' ? value.amount || value.qty || 0 : value));
        const barWidth = chartWidth / entries.length;
        
        entries.forEach(([key, value], index) => {
            const barValue = typeof value === 'object' ? value.amount || value.qty || 0 : value;
            const barHeight = (barValue / maxValue) * (chartHeight - 40);
            const barX = chartX + (index * barWidth) + 10;
            const barY = chartY + chartHeight - barHeight - 20;
            
            // Draw bar
            this.doc.rect(barX, barY, barWidth - 20, barHeight)
                .fillColor(this.colors.primary)
                .fill();
            
            // Bar label
            this.doc.fontSize(8)
                .fillColor(this.colors.text)
                .text(key.substring(0, 8), barX, chartY + chartHeight - 15, {
                    width: barWidth - 20,
                    align: 'center'
                });
            
            // Bar value
            if (barHeight > 20) {
                this.doc.fontSize(7)
                    .fillColor('white')
                    .text(typeof value === 'object' ? 
                        (value.amount ? `Rs.${Math.round(value.amount/1000)}K` : value.qty.toString()) :
                        value.toString(), 
                        barX, barY + 5, {
                        width: barWidth - 20,
                        align: 'center'
                    });
            }
        });
        
        this.currentY += chartHeight + 30;
    }
    
    addFooter() {
        const footerY = 750;
        
        this.doc.fontSize(8)
            .fillColor(this.colors.secondary)
            .text('Generated by WhatsApp Tyre System', this.pageMargin, footerY, { align: 'center' });
        
        this.doc.text(`Page ${this.doc.bufferedPageRange().start + 1}`, this.pageMargin, footerY + 15, { align: 'center' });
    }
}

async function generateSalesReportPDF(reportData, outputPath, reportType = 'weekly') {
    return new Promise((resolve, reject) => {
        try {
            const generator = new SalesReportPDFGenerator();
            const doc = generator.createDocument();
            
            // Pipe the PDF to a file
            doc.pipe(fs.createWriteStream(outputPath));
            
            // Generate content based on report type
            if (reportType === 'weekly') {
                generateWeeklyPDFContent(generator, reportData);
            } else if (reportType === 'monthly') {
                generateMonthlyPDFContent(generator, reportData);
            }
            
            // Finalize the PDF
            doc.end();
            
            doc.on('end', () => {
                resolve(outputPath);
            });
            
            doc.on('error', (error) => {
                reject(error);
            });
            
        } catch (error) {
            reject(error);
        }
    });
}

function generateWeeklyPDFContent(generator, reportData) {
    const { period, summary, dailyBreakdown, brandAnalysis, topSellingItems } = reportData;
    
    // Header
    generator.addHeader('WEEKLY SALES REPORT', period, 'weekly');
    
    // Summary
    generator.addSummaryBox(summary);
    
    // Daily breakdown table
    const dailyData = Object.keys(dailyBreakdown).map(day => ({
        date: moment(day).format('ddd, MMM DD'),
        invoices: dailyBreakdown[day].invoices,
        amount: dailyBreakdown[day].amount,
        qty: dailyBreakdown[day].qty
    }));
    
    generator.addTable('📅 Daily Performance', dailyData, [
        { header: 'Date', key: 'date' },
        { header: 'Invoices', key: 'invoices', format: 'number' },
        { header: 'Revenue', key: 'amount', format: 'currency' },
        { header: 'Quantity', key: 'qty', format: 'number' }
    ]);
    
    // Brand analysis chart
    generator.addChart('🏷️ Brand Performance', brandAnalysis);
    
    // Top selling items
    generator.addTable('🏆 Top Selling Items', topSellingItems, [
        { header: 'Description', key: 'description' },
        { header: 'Quantity', key: 'qty', format: 'number' },
        { header: 'Revenue', key: 'revenue', format: 'currency' }
    ]);
    
    // Footer
    generator.addFooter();
}

function generateMonthlyPDFContent(generator, reportData) {
    const { period, summary, weeklyBreakdown, brandAnalysis, customerAnalysis, topItems } = reportData;
    
    // Header
    generator.addHeader('MONTHLY SALES REPORT', period, 'monthly');
    
    // Summary
    generator.addSummaryBox(summary);
    
    // Weekly breakdown
    const weeklyData = Object.keys(weeklyBreakdown).map(week => ({
        week: week,
        invoices: weeklyBreakdown[week].invoiceCount,
        amount: weeklyBreakdown[week].amount,
        qty: weeklyBreakdown[week].qty
    }));
    
    generator.addTable('📊 Weekly Breakdown', weeklyData, [
        { header: 'Week', key: 'week' },
        { header: 'Invoices', key: 'invoices', format: 'number' },
        { header: 'Revenue', key: 'amount', format: 'currency' },
        { header: 'Quantity', key: 'qty', format: 'number' }
    ]);
    
    // Start new page for charts
    generator.doc.addPage();
    generator.currentY = generator.pageMargin;
    
    // Brand analysis chart
    generator.addChart('🏷️ Brand Performance', brandAnalysis);
    
    // Customer analysis
    const topCustomers = Object.keys(customerAnalysis)
        .map(customer => ({
            customer: customer,
            revenue: customerAnalysis[customer].revenue,
            invoices: customerAnalysis[customer].invoiceCount,
            qty: customerAnalysis[customer].qty
        }))
        .sort((a, b) => b.revenue - a.revenue)
        .slice(0, 10);
    
    generator.addTable('👥 Top Customers', topCustomers, [
        { header: 'Customer', key: 'customer' },
        { header: 'Revenue', key: 'revenue', format: 'currency' },
        { header: 'Invoices', key: 'invoices', format: 'number' },
        { header: 'Quantity', key: 'qty', format: 'number' }
    ]);
    
    // Top selling items
    generator.addTable('🏆 Top Selling Items', topItems, [
        { header: 'Description', key: 'description' },
        { header: 'Quantity', key: 'qty', format: 'number' },
        { header: 'Revenue', key: 'revenue', format: 'currency' },
        { header: 'Profit', key: 'profit', format: 'currency' }
    ]);
    
    // Footer
    generator.addFooter();
}

module.exports = {
    generateSalesReportPDF,
    SalesReportPDFGenerator
};