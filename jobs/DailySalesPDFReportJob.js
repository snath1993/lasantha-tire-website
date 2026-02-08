const moment = require('moment');
const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const sql = require('mssql');

module.exports = async function DailySalesPDFReportJob(client, mainPool, aiPool, allowedContacts, logAndSave, options = {}) {
    logAndSave('🚀 Starting Daily Sales PDF Report Job...');

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
        // 2. Fetch Data
        const today = moment().format('YYYY-MM-DD');
        const start = `${today} 00:00:00`;
        const end = `${today} 23:59:59`;

        const baseWhere = `
            CAST(s.Expr1 AS DATE) = CAST(@startDate AS DATE)
            AND (s.Expr14 = 0 OR s.Expr14 IS NULL)
        `;

        const req = mainPool.request();
        req.input('startDate', sql.DateTime, start);
        req.input('endDate', sql.DateTime, end);

        // Summary Query 
        // Using [View_Sales report whatsapp] with Expr columns
        // Expr1: InvoiceDate, Expr12: Amount, Expr5: Qty, UnitCost: UnitCost
        const summaryQuery = `
            SELECT
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0)) AS TotalSales,
                SUM(COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0)) AS TotalQty,
                SUM(COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0)) AS TotalCost,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS TotalProfit,
                COUNT(DISTINCT s.Expr2) AS InvoiceCount
            FROM [View_Sales report whatsapp] s
            LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.Expr3
            WHERE ${baseWhere}
        `;
        const summaryRes = await req.query(summaryQuery);
        const summary = summaryRes.recordset[0];

        // Category Breakdown
        // Using Expr columns: Categoty is correct (typo in DB), Amount=Expr12, Qty=Expr5, UnitCost=UnitCost
        const categoryQuery = `
            SELECT
                s.Categoty AS name,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0)) AS sales,
                SUM(COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0)) AS quantity,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
            FROM [View_Sales report whatsapp] s
            -- No need to join Item Master if Category is in Sales View, but keeping join for safety if needed
            -- Actually Categoty is in [View_Sales report whatsapp]
            WHERE ${baseWhere}
            GROUP BY s.Categoty
            ORDER BY sales DESC
        `;
        const categoryRes = await req.query(categoryQuery);
        const categories = categoryRes.recordset;

        // Brand Breakdown
        // Custom3 in ItemMaster
        // Join Condition: s.ItemID (Expr3) = im.ItemID
        const brandQuery = `
            SELECT
                COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown') AS name,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0)) AS sales,
                SUM(COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0)) AS quantity,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0) - (COALESCE(CAST(s.Expr5 AS DECIMAL(18,2)), 0) * COALESCE(CAST(s.UnitCost AS DECIMAL(18,2)), 0))) AS profit
            FROM [View_Sales report whatsapp] s
            LEFT JOIN [View_Item Master Whatsapp] im ON im.ItemID = s.Expr3
            WHERE ${baseWhere}
            GROUP BY COALESCE(NULLIF(LTRIM(RTRIM(im.Custom3)), ''), 'Unknown')
            ORDER BY sales DESC
        `;
        const brandRes = await req.query(brandQuery);
        const brands = brandRes.recordset;

        // Payment Breakdown
        // PaymentM = Expr15, InvoiceNo = Expr2, Amount = Expr12
        const paymentQuery = `
            SELECT
                COALESCE(NULLIF(LTRIM(RTRIM(s.Expr15)), ''), 'Unknown') AS name,
                SUM(COALESCE(CAST(s.Expr12 AS DECIMAL(18,2)), 0)) AS sales,
                COUNT(DISTINCT s.Expr2) AS count
            FROM [View_Sales report whatsapp] s
            WHERE ${baseWhere}
            GROUP BY COALESCE(NULLIF(LTRIM(RTRIM(s.Expr15)), ''), 'Unknown')
            ORDER BY sales DESC
        `;
        const paymentRes = await req.query(paymentQuery);
        const payments = paymentRes.recordset;

        // 3. Generate PDF
        const doc = new PDFDocument({ margin: 50 });
        const buffers = [];
        doc.on('data', buffers.push.bind(buffers));
        
        // Helper to format currency
        const fmt = (n) => (n || 0).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        const fmtInt = (n) => (n || 0).toLocaleString('en-US');

        // Title
        doc.fontSize(20).text(`Daily Sales Report`, { align: 'center' });
        doc.fontSize(12).text(moment().format('MMMM Do YYYY'), { align: 'center' });
        doc.moveDown();

        // Summary Box
        doc.rect(50, doc.y, 495, 80).stroke();
        const startY = doc.y + 10;
        
        doc.fontSize(10);
        doc.text('Total Sales:', 70, startY);
        doc.text(fmt(summary.TotalSales), 150, startY);
        
        doc.text('Total Profit:', 300, startY);
        doc.text(fmt(summary.TotalProfit), 380, startY);

        doc.text('Total Qty:', 70, startY + 20);
        doc.text(fmtInt(summary.TotalQty), 150, startY + 20);

        doc.text('Invoices:', 300, startY + 20);
        doc.text(fmtInt(summary.InvoiceCount), 380, startY + 20);
        
        doc.moveDown(4);

        // Helper to draw table
        function drawTable(title, headers, rows, colWidths) {
            doc.moveDown();
            doc.fontSize(14).font('Helvetica-Bold').text(title);
            doc.moveDown(0.5);
            
            const startX = 50;
            let currentY = doc.y;
            
            // Header
            doc.fontSize(10).font('Helvetica-Bold');
            let currentX = startX;
            headers.forEach((h, i) => {
                doc.text(h, currentX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                currentX += colWidths[i];
            });
            
            currentY += 15;
            doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke();
            currentY += 5;

            // Rows
            doc.font('Helvetica');
            rows.forEach((row, idx) => {
                if (currentY > 700) {
                    doc.addPage();
                    currentY = 50;
                    // Redraw headers on new page
                    doc.fontSize(10).font('Helvetica-Bold');
                    let hX = startX;
                    headers.forEach((h, i) => {
                        doc.text(h, hX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                        hX += colWidths[i];
                    });
                    currentY += 15;
                    doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke();
                    currentY += 5;
                    doc.font('Helvetica');
                }
                
                currentX = startX;
                row.forEach((cell, i) => {
                    doc.text(cell, currentX, currentY, { width: colWidths[i], align: i === 0 ? 'left' : 'right' });
                    currentX += colWidths[i];
                });
                currentY += 15;
            });
            
            // Total Row (if applicable)
            if (rows.length > 0 && headers.length > 1) {
                 // Check if we need a new page for the total row
                 if (currentY > 700) {
                    doc.addPage();
                    currentY = 50;
                 }
                 currentY += 5;
                 doc.moveTo(startX, currentY).lineTo(startX + 495, currentY).stroke();
                 currentY += 5;
                 doc.font('Helvetica-Bold');
                 // We don't print "Total" text here because it's already in the last row of data passed to this function
            }
            
            doc.y = currentY + 20;
        }

        // Category Table
        const catRows = categories.map(c => [
            c.name || 'Unknown',
            fmtInt(c.quantity),
            fmt(c.sales),
            fmt(c.profit)
        ]);
        // Add Total Row
        catRows.push([
            'TOTAL',
            fmtInt(categories.reduce((a, b) => a + (b.quantity || 0), 0)),
            fmt(categories.reduce((a, b) => a + (b.sales || 0), 0)),
            fmt(categories.reduce((a, b) => a + (b.profit || 0), 0))
        ]);

        drawTable('Category Breakdown', ['Category', 'Qty', 'Sales', 'Profit'], catRows, [195, 60, 120, 120]);

        // Brand Table
        const brandRows = brands.map(b => [
            b.name || 'Unknown',
            fmtInt(b.quantity),
            fmt(b.sales),
            fmt(b.profit)
        ]);
        // Add Total Row
        brandRows.push([
            'TOTAL',
            fmtInt(brands.reduce((a, b) => a + (b.quantity || 0), 0)),
            fmt(brands.reduce((a, b) => a + (b.sales || 0), 0)),
            fmt(brands.reduce((a, b) => a + (b.profit || 0), 0))
        ]);

        drawTable('Brand Breakdown', ['Brand', 'Qty', 'Sales', 'Profit'], brandRows, [195, 60, 120, 120]);

        // Payment Table
        const payRows = payments.map(p => [
            p.name || 'Unknown',
            fmtInt(p.count),
            fmt(p.sales)
        ]);
        // Add Total Row
        payRows.push([
            'TOTAL',
            fmtInt(payments.reduce((a, b) => a + (b.count || 0), 0)),
            fmt(payments.reduce((a, b) => a + (b.sales || 0), 0))
        ]);

        drawTable('Payment Methods', ['Method', 'Count', 'Amount'], payRows, [195, 100, 200]);

        doc.end();

        // Wait for PDF to finish
        const pdfBuffer = await new Promise((resolve) => {
            doc.on('end', () => {
                resolve(Buffer.concat(buffers));
            });
        });

        // 4. Send PDF
        // BAILEYS MIGRATION: Use wrapper
        const { MessageMedia } = require('../utils/baileysWrapper');
        const media = new MessageMedia('application/pdf', pdfBuffer.toString('base64'), `Sales_Report_${today}.pdf`);
        
        await client.sendMessage(targetGroup.id._serialized, media, { caption: `📊 Daily Sales Report - ${today}` });
        logAndSave(`✅ Sent Daily Sales Report to group "Re Order"`);

    } catch (err) {
        logAndSave(`❌ Daily Sales Report Job Failed: ${err.message}`);
        console.error(err);
    }
};
