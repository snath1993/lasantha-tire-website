const moment = require('moment');
const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const axios = require('axios');

// Configuration
const BRIDGE_URL = 'http://localhost:5001';

module.exports = async function PeachtreeAgingReportJob(client, mainPool, aiPool, allowedContacts, logAndSave, options = {}) {
    logAndSave('🚀 Starting Peachtree Aging Report Job...');

    // 1. Find Target Group "Re Order"
    let targetGroup = null;
    try {
        if (client.getChats) {
            let chats = await client.getChats();
            targetGroup = chats.find(chat => chat.isGroup && chat.name.toLowerCase() === 're order');
            
            if (!targetGroup) {
                logAndSave('⚠️ Warning: WhatsApp Group "Re Order" not found.');
                return;
            }
        } else {
            logAndSave('⚠️ Client does not support getChats (Mock Mode?)');
            return;
        }
    } catch (error) {
        logAndSave(`❌ Error finding group: ${error.message}`);
        return;
    }

    try {
        // 2. Fetch Data from Python Bridge
        logAndSave('📡 Fetching AR Aging data from Peachtree Bridge...');
        const arRes = await axios.get(`${BRIDGE_URL}/api/peachtree/business-status/ar-aging`);
        const arData = arRes.data;

        logAndSave('📡 Fetching AP Aging data from Peachtree Bridge...');
        const apRes = await axios.get(`${BRIDGE_URL}/api/peachtree/business-status/ap-aging`);
        const apData = apRes.data;

        if (!arData.success || !apData.success) {
            throw new Error('Failed to fetch aging data from bridge');
        }

        // 3. Generate Professional A4 PDF
        const doc = new PDFDocument({ size: 'A4', margin: 50, bufferPages: true });
        const buffers = [];
        doc.on('data', buffers.push.bind(buffers));

        // --- PDF STYLING ---
        const colors = {
            primary: '#1e3a8a', // Dark Blue
            secondary: '#64748b', // Slate Gray
            accent: '#f59e0b', // Amber
            text: '#1f2937', // Gray 800
            lightBg: '#f3f4f6', // Gray 100
            border: '#e5e7eb' // Gray 200
        };

        // Header
        doc.fontSize(24).fillColor(colors.primary).text('LASANTHA TYRE TRADERS', { align: 'center' });
        doc.fontSize(10).fillColor(colors.secondary).text('1035 Pannipitiya Road, Kumaragewatta, Battaramulla', { align: 'center' });
        doc.text('+94 77 313 1883 | info@lasanthatyre.com', { align: 'center' });
        doc.moveDown(2);

        // Title
        doc.fontSize(18).fillColor(colors.text).text('Weekly Aging Report', { align: 'center' });
        doc.fontSize(12).fillColor(colors.secondary).text(`Generated: ${moment().format('MMMM Do YYYY, h:mm a')}`, { align: 'center' });
        doc.moveDown(2);

        // Helper: Draw Summary Box
        const drawSummaryBox = (title, data, y) => {
            const boxHeight = 100;
            doc.rect(50, y, 495, boxHeight).fillAndStroke(colors.lightBg, colors.border);
            
            doc.fillColor(colors.primary).fontSize(14).text(title, 70, y + 15);
            
            const labels = ['Current', '1-30 Days', '31-60 Days', '61-90 Days', 'Over 90 Days'];
            const keys = ['current', 'days_1_30', 'days_31_60', 'days_61_90', 'over_90'];
            
            let currentY = y + 40;
            doc.fontSize(10).fillColor(colors.text);
            
            labels.forEach((label, i) => {
                const key = keys[i];
                const amount = parseFloat(data.summary[key] || 0) || 0;
                const total = parseFloat(data.summary.total || 0) || 0;
                const percent = total > 0 ? (amount / total) * 100 : 0;
                
                doc.text(label, 70, currentY);
                doc.text(`Rs. ${amount.toLocaleString('en-US', { minimumFractionDigits: 2 })}`, 200, currentY, { align: 'right', width: 100 });
                doc.text(`${percent.toFixed(1)}%`, 320, currentY);
                
                // Bar
                const barWidth = percent * 1.5; // Scale factor
                doc.rect(380, currentY, barWidth, 8).fill(colors.primary);
                
                currentY += 12;
            });

            // Total
            doc.font('Helvetica-Bold').text('Total Outstanding:', 70, currentY + 5);
            const totalOutstanding = parseFloat(data.summary.total || 0) || 0;
            doc.text(`Rs. ${totalOutstanding.toLocaleString('en-US', { minimumFractionDigits: 2 })}`, 200, currentY + 5, { align: 'right', width: 100 });
            doc.font('Helvetica');
        };

        // AR Summary (Receivables)
        drawSummaryBox('Accounts Receivable (Money In)', arData, doc.y);
        doc.moveDown(9);

        // AP Summary (Payables)
        drawSummaryBox('Accounts Payable (Money Out)', apData, doc.y);
        doc.moveDown(2);

        // Page Break for Details
        doc.addPage();

        // Helper: Draw Table
        const drawTable = (title, headers, rows, colWidths, totalLabel = 'Total', overrideTotal = null) => {
            doc.fontSize(14).fillColor(colors.primary).text(title);
            doc.moveDown(0.5);
            
            const startX = 50;
            let currentY = doc.y;
            
            // Header
            doc.fontSize(9).font('Helvetica-Bold').fillColor(colors.text);
            let currentX = startX;
            headers.forEach((h, i) => {
                doc.text(h, currentX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                currentX += colWidths[i];
            });
            
            currentY += 15;
            doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke(colors.border);
            currentY += 5;

            // Rows
            doc.font('Helvetica').fillColor(colors.text);
            let totalAmount = 0;

            rows.forEach((row, idx) => {
                if (overrideTotal === null) {
                    // Calculate total from last column (assuming it's the amount)
                    const rawAmountCell = row[row.length - 1];
                    const amount = (typeof rawAmountCell === 'number')
                        ? rawAmountCell
                        : parseFloat(String(rawAmountCell).replace(/[^0-9.-]/g, ''));
                    if (!isNaN(amount)) {
                        totalAmount += amount;
                    }
                }

                if (currentY > 750) {
                    doc.addPage();
                    currentY = 50;
                    // Redraw header
                    doc.fontSize(9).font('Helvetica-Bold');
                    let hX = startX;
                    headers.forEach((h, i) => {
                        doc.text(h, hX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                        hX += colWidths[i];
                    });
                    currentY += 15;
                    doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke(colors.border);
                    currentY += 5;
                    doc.font('Helvetica');
                }
                
                // Zebra striping
                if (idx % 2 === 0) {
                    doc.rect(startX, currentY - 2, 495, 12).fill(colors.lightBg);
                    doc.fillColor(colors.text);
                }

                currentX = startX;
                row.forEach((cell, i) => {
                    doc.text(cell, currentX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                    currentX += colWidths[i];
                });
                currentY += 12;
            });

            // Draw Total Row
            currentY += 5;
            doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke(colors.primary);
            currentY += 5;
            
            doc.font('Helvetica-Bold').fillColor(colors.primary);
            doc.text(totalLabel, startX, currentY, { width: colWidths[0] + colWidths[1] + colWidths[2], align: 'right' });
            
            // Align total with the last column
            const finalTotal = overrideTotal !== null ? overrideTotal : totalAmount;
            let lastColX = startX + colWidths[0] + colWidths[1] + colWidths[2];
            doc.text(`Rs. ${finalTotal.toLocaleString('en-US', { minimumFractionDigits: 2 })}`, lastColX, currentY, { width: colWidths[3], align: 'right' });
            
            doc.y = currentY + 25;
        };

        // Helper: Format Currency (LKR)
        const formatLKR = (amount) => {
            const val = parseFloat(amount || 0);
            // Format: Rs. 1,234.56 (remove .00 if integer? User said "unnecessary 0")
            // Let's keep 2 decimals for consistency but use Rs. prefix
            return `Rs. ${val.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        };

        // All Customers Table (Filtered > 0)
        const customerRows = arData.customers
            .filter(c => parseFloat(c.TotalOutstanding || 0) > 0.01) // Filter > 0.01 to avoid dust
            .map(c => [
                c.Customer_Bill_Name || c.CustomerID,
                c.Phone || '-',
                (c.DaysOutstanding || 0).toString(),
                formatLKR(c.TotalOutstanding)
            ]);
        // Adjusted widths: Name(245), Phone(90), Days(40), Balance(120) = 495
        drawTable('All Outstanding Customers', ['Customer', 'Phone', 'Days', 'Balance (LKR)'], customerRows, [245, 90, 40, 120], 'Total Receivables:', arData.summary.total);

        // All Vendors Table (Filtered > 0)
        const vendorRows = apData.vendors
            .filter(v => parseFloat(v.TotalOutstanding || 0) > 0.01)
            .map(v => [
                v.VendorName || v.VendorID,
                v.Phone || '-',
                (v.DaysOutstanding || 0).toString(),
                formatLKR(v.TotalOutstanding)
            ]);
        drawTable('All Outstanding Vendors', ['Vendor', 'Phone', 'Days', 'Balance (LKR)'], vendorRows, [245, 90, 40, 120], 'Total Payables:', apData.summary.total);

        // Net Position Summary
        if (doc.y > 650) doc.addPage();
        doc.moveDown(2);
        
        const totalAR = parseFloat(arData.summary.total || 0);
        const totalAP = parseFloat(apData.summary.total || 0);
        const netPosition = totalAR - totalAP;
        
        doc.rect(50, doc.y, 495, 120).fillAndStroke('#f8fafc', colors.primary);
        let summaryY = doc.y + 20;
        
        doc.fillColor(colors.primary).fontSize(16).text('Net Financial Position', 0, summaryY, { align: 'center' });
        summaryY += 30;
        
        doc.fontSize(12).fillColor(colors.text);
        doc.text('Total Receivables (In):', 100, summaryY);
        doc.text(formatLKR(totalAR), 300, summaryY, { align: 'right', width: 150 });
        
        summaryY += 20;
        doc.text('Total Payables (Out):', 100, summaryY);
        doc.text(formatLKR(totalAP), 300, summaryY, { align: 'right', width: 150 });
        
        summaryY += 25;
        doc.moveTo(100, summaryY - 5).lineTo(450, summaryY - 5).stroke(colors.border);
        
        doc.font('Helvetica-Bold').fontSize(14);
        doc.text('Net Cash Position:', 100, summaryY);
        
        const netColor = netPosition >= 0 ? '#10b981' : '#ef4444'; // Green or Red
        doc.fillColor(netColor).text(formatLKR(netPosition), 300, summaryY, { align: 'right', width: 150 });

        // Footer
        const pageCount = doc.bufferedPageRange().count;
        for (let i = 0; i < pageCount; i++) {
            doc.switchToPage(i);
            doc.fontSize(8).fillColor(colors.secondary).text(
                `Page ${i + 1} of ${pageCount} | Lasantha Tyre Traders | Confidential`,
                50,
                doc.page.height - 30,
                { align: 'center', width: 495 }
            );
        }

        doc.end();

        // Wait for PDF
        const pdfBuffer = await new Promise((resolve) => {
            doc.on('end', () => {
                resolve(Buffer.concat(buffers));
            });
        });

        // 4. Send PDF
        // BAILEYS MIGRATION: Use wrapper
        const { MessageMedia } = require('../utils/baileysWrapper');
        const media = new MessageMedia('application/pdf', pdfBuffer.toString('base64'), `Aging_Report_${moment().format('YYYY-MM-DD')}.pdf`);
        
        await client.sendMessage(targetGroup.id._serialized, media, { caption: `📊 Weekly Aging Report - ${moment().format('YYYY-MM-DD')}` });
        logAndSave(`✅ Sent Aging Report to group "Re Order"`);

    } catch (err) {
        logAndSave(`❌ Aging Report Job Failed: ${err.message}`);
        console.error(err);
    }
};
