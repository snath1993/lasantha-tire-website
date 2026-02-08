// TyreQuotationPDFJob
// Create and reply quotation PDF for given tyre size
const PDFDocument = require('pdfkit');
const fs = require('fs');
const { extractTyreSizeFlexible } = require('../utils/detect');
const { parsePriceAdjustments } = require('../utils/priceAdjust');
const { loadBasicQuotationConfig, roundToStep } = require('../utils/quotationConfig');
module.exports = async function TyreQuotationPDFJob(msg, sql, sqlConfig, logAndSave) {
    const text = msg.body.trim();
    const tyreSize = extractTyreSizeFlexible(text);
    // Detect flexible quote keywords
    const quoteRegex = /(quotation|quote|quot|qout|qoute)/i;
    if (tyreSize && quoteRegex.test(text)) {
        const pricing = loadBasicQuotationConfig();
        try {
            await sql.connect(sqlConfig);
            const result = await sql.query`
                SELECT im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3
                FROM [View_Item Master Whatsapp] im
                JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
                WHERE im.Categoty = 'TYRES'
                  AND iw.QTY > 0
                  AND (
                      im.ItemDescription LIKE ${'%' + tyreSize + '%'}
                      OR im.ItemDescription LIKE ${'%' + tyreSize.replace(/\//g, '') + '%'}
                  )
            `;
            const matches = result.recordset.filter(tyre => tyre.UnitCost > pricing.minimumUnitCost);
            if (matches.length > 0) {
                const doc = new PDFDocument();
                const safeTyreSize = String(tyreSize).replace(/[\/\\]/g, '-');
                const fileName = `quotation_${safeTyreSize}_${Date.now()}.pdf`;
                doc.pipe(fs.createWriteStream(fileName));
                doc.fontSize(18).text(`Tyre Quotation for ${tyreSize}`, { align: 'center' });
                doc.moveDown();
                // Parse price adjustments from message
                const isAllowedContact = true; // Quotation PDF is only for allowed contacts in your logic
                const adjustments = parsePriceAdjustments(text);
                matches.forEach(tyre => {
                    const brand = tyre.Custom3 ? tyre.Custom3.trim().toUpperCase() : '';
                    let price = roundToStep(tyre.UnitCost, pricing.baseRoundStep) + pricing.allowedDefaultMarkup;
                    // Apply price adjustments for allowed contacts
                    if (isAllowedContact && adjustments.length > 0) {
                        adjustments.forEach(adj => {
                            let brandMatch = true;
                            if (adj.brand) {
                                if (adj.exclude) {
                                    if (brand === adj.brand) brandMatch = false;
                                } else {
                                    if (brand !== adj.brand) brandMatch = false;
                                }
                            }
                            if (brandMatch) {
                                if (adj.type === 'percent') {
                                    price = Math.round(price * (1 + adj.value / 100));
                                } else if (adj.type === 'fixed') {
                                    price = price + adj.value;
                                }
                            }
                        });
                    }
                    doc.fontSize(12).text(`Description: ${tyre.ItemDescription}`);
                    doc.text(`Brand: ${tyre.Custom3}`);
                    doc.text(`Unit Price: ${price}/=`);
                    doc.moveDown();
                });
                doc.end();
                // Wait for file to finish writing
                doc.on('finish', async () => {
                    try {
                        // BAILEYS MIGRATION: Use wrapper
                        const { MessageMedia } = require('../utils/baileysWrapper');
                        const media = MessageMedia.fromFilePath(fileName);
                        await msg.reply(media);
                        logAndSave(`Quotation PDF sent for ${tyreSize}: ${fileName}`);
                    } catch (e) {
                        msg.reply(`Quotation PDF created for ${tyreSize}. (Error sending PDF as media)`);
                        logAndSave(`Quotation PDF created for ${tyreSize}: ${fileName} (media send error)`);
                    }
                });
                logAndSave(`Quotation PDF created for ${tyreSize}: ${fileName}`);
            } else {
                msg.reply(`No tyres found for size: ${tyreSize}`);
                logAndSave(`No tyres for quotation: ${tyreSize}`);
            }
        } catch (err) {
            msg.reply('Error connecting to SQL Server.');
            logAndSave(`SQL error: ${err.message}`);
        } finally {
            await sql.close();
        }
        return true;
    }
    return false;
}
