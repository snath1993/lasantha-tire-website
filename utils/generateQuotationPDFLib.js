const { PDFDocument, rgb, StandardFonts } = require('pdf-lib');
const fs = require('fs');
const path = require('path');


async function generateQuotationPDFLib({ tyres, tyreSize, qty, logoPath, vehicleNo, customerName, credit, creditPeriod, quotationNumber }) {
    const pdfDoc = await PDFDocument.create();
    const page = pdfDoc.addPage([595.28, 841.89]); // A4 size

    // Fonts
    const fontNormal = await pdfDoc.embedFont(StandardFonts.Helvetica);
    const fontBold = await pdfDoc.embedFont(StandardFonts.HelveticaBold);

    // Quotation number and date at topmost right (smaller font, wrap if long, right-aligned)
    const now = new Date();
    const dateStr = now.toISOString().slice(0, 10);
    let shortQtn = quotationNumber ? quotationNumber.replace(/QTN-\d{4}-(\d{4})/, 'QTN-$1') : '-';
    if (shortQtn.length > 10) shortQtn = shortQtn.slice(0, 10) + '\n' + shortQtn.slice(10);
    const qtnFontSize = 10;
    const qtnLines = shortQtn.split('\n');
    // Calculate rightmost X for alignment
    const qtnMaxWidth = Math.max(...qtnLines.map(line => fontBold.widthOfTextAtSize(line, qtnFontSize)));
    const qtnX = page.getWidth() - qtnMaxWidth - 30;
    const qtnY = page.getHeight() - 30;
    qtnLines.forEach((line, idx) => {
        page.drawText(line, {
            x: qtnX,
            y: qtnY - idx * (qtnFontSize + 2),
            size: qtnFontSize,
            font: fontBold,
            color: rgb(0.1, 0.1, 0.5)
        });
    });
    // Date right-aligned with quotation number
    const dateWidth = fontNormal.widthOfTextAtSize(`Date: ${dateStr}`, 9);
    page.drawText(`Date: ${dateStr}`, {
        x: page.getWidth() - dateWidth - 30,
        y: qtnY - qtnLines.length * (qtnFontSize + 2) - 2,
        size: 9,
        font: fontNormal,
        color: rgb(0.2, 0.2, 0.2)
    });

    // Shop name, slogan, address, contact
    page.drawText('LASANTHA TYRE TRADERS', {
        x: 50,
        y: page.getHeight() - 50,
        size: 28,
        font: fontBold,
        color: rgb(0.1, 0.1, 0.5)
    });
    page.drawText('GO SMART WITH LASANTHA TYRE', {
        x: 50,
        y: page.getHeight() - 85,
        size: 18,
        font: fontBold,
        color: rgb(0.98, 0.58, 0.13),
    });
    page.drawText('1035, Pannipitiya Road, Kumaragewattha, Battaramulla, Sri Lanka', {
        x: 50,
        y: page.getHeight() - 110,
        size: 13,
        font: fontNormal,
        color: rgb(0.2, 0.2, 0.2),
    });
    page.drawText('Phone: 0112 773 232 | Mobile: 0771 222 509 | Email: lasanthatyretraders@gmail.com', {
        x: 50,
        y: page.getHeight() - 130,
        size: 13,
        font: fontNormal,
        color: rgb(0.2, 0.2, 0.2),
    });

    // Add extra space between address and payment terms rows
    let infoY = page.getHeight() - 150;
    page.drawText(`Payment Terms - ${credit ? credit : 'Cash'}    Vehicle Number - ${vehicleNo || '-'}`, {
        x: 50,
        y: infoY,
        size: 13,
        font: fontNormal
    });
    infoY -= 18;
    // Customer Name row below
    page.drawText(`Customer Name - ${customerName || '-'}`, {
        x: 50,
        y: infoY,
        size: 13,
        font: fontNormal
    });
    infoY -= 18;
    // Credit period row if applicable
    if (credit && credit.toLowerCase() === 'credit' && creditPeriod) {
        page.drawText(`Credit Period: ${creditPeriod}`, {
            x: 50,
            y: infoY,
            size: 13,
            font: fontNormal,
            color: rgb(0.98, 0.58, 0.13)
        });
        infoY -= 18;
    }


    // Draw watermark first (background, extra large, faded)
    try {
        const logoBuffer = fs.readFileSync(logoPath);
        let logoImage;
        if (logoPath.toLowerCase().endsWith('.png')) {
            logoImage = await pdfDoc.embedPng(logoBuffer);
        } else {
            logoImage = await pdfDoc.embedJpg(logoBuffer);
        }
        const wmWidth = page.getWidth() * 1.1;
        const wmHeight = wmWidth * (logoImage.height / logoImage.width);
        page.drawImage(logoImage, {
            x: (page.getWidth() - wmWidth) / 2,
            y: (page.getHeight() - wmHeight) / 2,
            width: wmWidth,
            height: wmHeight,
            opacity: 0.13,
        });
    } catch (err) {
        console.error('Logo watermark error:', err.message);
    }

    // Price chart columns
    const marginLeft = 40;
    const chartStartY = infoY - 30; // Move chart further down
    const rowHeight = 22;
    // Columns: No, Description, QTY, Unit Price, Total
    // Shift QTY, Unit Price, Total columns further right
    const colX = [marginLeft, marginLeft + 40, marginLeft + 260, marginLeft + 370, marginLeft + 470, marginLeft + 560];
    // Column headers
    page.drawText('No', { x: colX[0] + 8, y: chartStartY, size: 13, font: fontBold });
    page.drawText('Description', { x: colX[1] + 8, y: chartStartY, size: 13, font: fontBold });
    // Center QTY header above numbers
    const qtyHeader = 'QTY';
    const qtyHeaderWidth = fontBold.widthOfTextAtSize(qtyHeader, 13);
    const qtyHeaderCenter = colX[2] + ((colX[3] - colX[2]) / 2) - (qtyHeaderWidth / 2);
    page.drawText(qtyHeader, { x: qtyHeaderCenter, y: chartStartY, size: 13, font: fontBold });
    page.drawText('Unit Price', { x: colX[3] + 8, y: chartStartY, size: 13, font: fontBold });
    page.drawText('Total', { x: colX[4] + 8, y: chartStartY, size: 13, font: fontBold });
    let y = chartStartY - rowHeight;
    tyres.forEach((tyre, i) => {
        page.drawText(String(i + 1), { x: colX[0] + 8, y, size: 11, font: fontNormal });
        // Wrap description to max 4 words per line
        const descWords = (tyre.description || '').split(' ');
        let descLines = [];
        for (let j = 0; j < descWords.length; j += 4) {
            descLines.push(descWords.slice(j, j + 4).join(' '));
        }
        descLines.forEach((line, idx) => {
            page.drawText(line, {
                x: colX[1] + 8,
                y: y - idx * 14,
                size: 11,
                font: fontNormal
            });
        });
        // Print QTY, Unit Price and Total
        const qtyStr = `${tyre.qty || '-'} `;
        const unitPriceStr = `${tyre.unitPrice || '-'} /=`;
        const totalStr = `${tyre.total || '-'} /=`;
        // QTY column centered
        const qtyWidth = fontNormal.widthOfTextAtSize(qtyStr, 11);
        const qtyColCenter = colX[2] + ((colX[3] - colX[2]) / 2) - (qtyWidth / 2);
        page.drawText(qtyStr, { x: qtyColCenter, y, size: 11, font: fontNormal });
        // Unit Price and Total as before
        page.drawText(unitPriceStr, { x: colX[3] + 8, y, size: 11, font: fontNormal });
        page.drawText(totalStr, { x: colX[4] + 8, y, size: 11, font: fontNormal });
        // Draw horizontal line under the last line of the item
        const lastLineY = y - (descLines.length - 1) * 14;
        page.drawLine({
            start: { x: marginLeft, y: lastLineY - 4 },
            end: { x: colX[4] + 60, y: lastLineY - 4 },
            thickness: 0.5,
            color: rgb(0.8, 0.8, 0.8)
        });
        y -= 22 + (descLines.length - 1) * 12; // Increased space between items
    });

    // Computer generated note below price chart (only once)
    page.drawText('Computer generated quotation, no signature required.', {
        x: 50,
        y: y - 20,
        size: 12,
        font: fontNormal,
        color: rgb(0.4, 0.4, 0.4)
    });

    // Quotation validity and thank you note
    const validityY = y - 38;
    page.drawText('This quotation is valid for 14 days from the date of issue.', {
        x: 50,
        y: validityY,
        size: 11,
        font: fontNormal,
        color: rgb(0.1, 0.1, 0.5)
    });
    page.drawText('Thank you for considering Lasantha Tyre Traders.', {
        x: 50,
        y: validityY - 16,
        size: 11,
        font: fontNormal,
        color: rgb(0.1, 0.1, 0.5)
    });

    // Warranty/Terms section (placed dynamically below chart)
        // Warranty/Terms section aligned to the bottom of the page
        const bottomMargin = 40;
        const warrantyFontSize = 9;
        const warrantyLineHeight = 14;
        const terms = [
            '• Warranty is provided by the manufacturer or importer and covers manufacturing defects only.',
            '• No warranty for used, repaired, punctured, cut, or burst tires, or for visible flaws caused by wheel alignment or technical issues.',
            '• Wheel alignment is recommended every 5,000 km for better mileage.',
            '• Damage from improper use, overloading, incorrect air pressure, poor maintenance, or third-party installations is not covered.',
            '• Customers are encouraged to inspect tires upon purchase and raise any concerns immediately.',
            '• Returns or exchanges are available for unused tires with the receipt.',
            '• Road hazard damage, such as potholes or debris, is excluded.',
            '• We look forward to serving you and ensuring your satisfaction with every purchase.'
        ];
        // Calculate total height needed for all terms and heading
        const totalTermsHeight = (terms.length * warrantyLineHeight) + 22;
        const warrantyY = bottomMargin + totalTermsHeight;
        // Draw heading
        page.drawText('Company Warranty & Terms', {
            x: 50,
            y: warrantyY,
            size: 15,
            font: fontBold,
            color: rgb(0.98, 0.58, 0.13),
        });
        // Draw terms
        let termY = warrantyY - 22;
        terms.forEach(term => {
            page.drawText(term, {
                x: 55,
                y: termY,
                size: warrantyFontSize,
                font: fontNormal,
                color: rgb(0.2, 0.2, 0.2),
            });
            termY -= warrantyLineHeight;
        });

    // Return PDF buffer
    const pdfBytes = await pdfDoc.save();
    return pdfBytes;
}

module.exports = { generateQuotationPDFLib };
