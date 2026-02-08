// TyreQuotationPDFLibJobPublic.js
// Public version: Generate and reply quotation PDF for non-allowed contacts
const { generateQuotationPDFLib } = require('../utils/generateQuotationPDFLib');
const fs = require('fs');
const path = require('path');
const { extractTyreSizeFlexible } = require('../utils/detect');
const { parseAlignmentBalancing } = require('../utils/alignmentBalancing');
const { loadPublicQuotationConfig, roundToStep } = require('../utils/quotationConfig');

const seqFile = path.join(__dirname, '../quotation-seq.json');
function getNextQuotationNumber() {
    let seq = { lastNumber: 0 };
    try {
        if (fs.existsSync(seqFile)) {
            seq = JSON.parse(fs.readFileSync(seqFile, 'utf8'));
        }
    } catch (e) {}
    seq.lastNumber++;
    fs.writeFileSync(seqFile, JSON.stringify(seq));
    const year = new Date().getFullYear();
    return `QTN-${year}-${String(seq.lastNumber).padStart(4, '0')}`;
}

module.exports = async function TyreQuotationPDFLibJobPublic(msg, sql, sqlConfig, logAndSave) {
    const text = msg.body.trim();
    const { extractVehicleNumber } = require('../utils/detect');
    const detectedVehicleNo = extractVehicleNumber(text);
        const tyreSize = extractTyreSizeFlexible(text); // Extract tyre size from the message
    const quoteRegex = /(quotation|quote|quot|qout|qoute|quotion|qoutation|කෝට්|ඉල්ලීම|கோட்|கோட்டேஷன்)/i;
    if (tyreSize && quoteRegex.test(text)) {
        const pricing = loadPublicQuotationConfig();
        let requestedQty = 1;
        let vehicleNo = '';
        let customerName = '';
        let credit = '';
        let creditPeriod = '';
        const quoteQtyMatch = text.match(/(?:quote|quot|qoute|qoutation|quotation|qout|quotion)[^\d]*(\d+)/i);
        if (quoteQtyMatch) {
            requestedQty = parseInt(quoteQtyMatch[1], 10);
        }
        if (detectedVehicleNo) {
            vehicleNo = detectedVehicleNo;
        }
        const parts = text.split(/[\/]/);
        parts.forEach(part => {
            const p = part.trim();
            if (/^(credit)$/i.test(p)) {
                credit = 'Credit';
                creditPeriod = '30 days';
            } else if (/^(cash)$/i.test(p)) {
                credit = 'Cash';
            } else if (
                p.length > 2 &&
                !quoteRegex.test(p) &&
                !/^\d+$/.test(p) &&
                !/ali[g]?n?m?e?n?t?/i.test(p) &&
                !/balanc(ing)?/i.test(p)
            ) {
                if (!vehicleNo || p.toUpperCase() !== vehicleNo) {
                    customerName = p;
                }
            }
        });
        if (!credit) credit = 'Cash';
        try {
            await sql.connect(sqlConfig);
            const result = await sql.query`
                SELECT im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3, iw.QTY
                FROM [View_Item Master Whatsapp] im
                JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
                WHERE (im.Categoty = 'TYRES' OR im.Categoty = 'WHEEL ALIGNMENT')
                  AND iw.QTY > 0
                  AND (
                      im.ItemDescription LIKE ${'%' + tyreSize + '%'}
                      OR im.ItemDescription LIKE ${'%' + tyreSize.replace(/\//g, '') + '%'}
                  )
            `;
            logAndSave('DEBUG: SQL result for ' + tyreSize + ': ' + JSON.stringify(result.recordset));
            if (result.recordset.length > 0) {
                let tyres = result.recordset
                    .filter(tyre => (tyre.QTY >= requestedQty || tyre.Categoty === 'WHEEL ALIGNMENT') && tyre.UnitCost > pricing.minimumUnitCost)
                    .map((tyre, i) => {
                        if (tyre.Categoty === 'WHEEL ALIGNMENT') {
                            const price = roundToStep((tyre.PriceLevel1 || tyre.UnitCost), pricing.alignmentRoundStep);
                            return {
                                description: tyre.ItemDescription,
                                brand: '',
                                qty: 1,
                                availableQty: tyre.QTY,
                                unitPrice: price,
                                total: price
                            };
                        } else {
                            const { normalizeBrand } = require('../utils/brandUtils');
                            const brand = normalizeBrand(tyre.Custom3);
                            // Use the same logic as TyrePriceReplyJob for public (non-allowed) contacts
                            let price;
                            if (brand === normalizeBrand('MAXXIS') || brand === normalizeBrand('MAXXIES')) {
                                price = roundToStep(tyre.UnitCost, pricing.baseRoundStep) + pricing.maxxisPublicMarkup;
                            } else {
                                price = roundToStep(tyre.UnitCost, pricing.baseRoundStep) + pricing.publicBaseMarkup + pricing.publicExtraBuffer;
                            }
                            return {
                                description: tyre.ItemDescription,
                                brand: brand,
                                qty: requestedQty,
                                availableQty: tyre.QTY,
                                unitPrice: price,
                                total: price * requestedQty
                            };
                        }
                    });
                const { alignment, balancing } = parseAlignmentBalancing(text);
                if (alignment) {
                    let alignTypes = [];
                    const hasCar = /car/i.test(text);
                    const hasRearCar = /rear\s*car/i.test(text);
                    const hasJeep = /jeep/i.test(text);
                    const hasRearJeep = /rear\s*jeep/i.test(text);
                    if ((hasCar || hasRearCar) && !(hasJeep || hasRearJeep)) {
                        if (hasCar) alignTypes.push({ type: 'car|cars', rear: false });
                        if (hasRearCar) alignTypes.push({ type: 'car|cars', rear: true });
                    } else if ((hasJeep || hasRearJeep) && !(hasCar || hasRearCar)) {
                        if (hasJeep) alignTypes.push({ type: 'jeep|jeeps', rear: false });
                        if (hasRearJeep) alignTypes.push({ type: 'jeep|jeeps', rear: true });
                    } else {
                        let alignType = alignment;
                        if (alignType === 'van') alignType = 'van|vans';
                        if (alignType === 'car') alignType = 'car|cars';
                        if (alignType === 'jeep') alignType = 'jeep|jeeps';
                        if (alignType === 'lorry') alignType = 'lorry|lorries';
                        if (alignType === 'bus') alignType = 'bus|buses|tipper';
                        if (alignType === 'prime') alignType = 'prime';
                        alignTypes.push({ type: alignType, rear: /rear/i.test(text) });
                    }
                    try {
                        await sql.connect(sqlConfig);
                        const alignResult = await sql.query`
                            SELECT im.PriceLevel1, im.ItemDescription
                            FROM [View_Item Master Whatsapp] im
                            WHERE im.Categoty = 'WHEEL ALIGNMENT'
                        `;
                        logAndSave('DEBUG: Alignment SQL result: ' + JSON.stringify(alignResult.recordset));
                        for (const alignObj of alignTypes) {
                            let filteredRows;
                            if (alignObj.rear) {
                                filteredRows = alignResult.recordset.filter(row => /rear/i.test(row.ItemDescription));
                                logAndSave('DEBUG: Filtering for REAR alignment items only');
                            } else {
                                filteredRows = alignResult.recordset.filter(row => !/rear/i.test(row.ItemDescription));
                                logAndSave('DEBUG: Filtering for NON-REAR alignment items only');
                            }
                            if (filteredRows.length > 0) {
                                let found = null;
                                for (const row of filteredRows) {
                                    logAndSave('DEBUG: Checking alignment row: ' + row.ItemDescription + ' vs ' + alignObj.type);
                                    if (new RegExp(alignObj.type, 'i').test(row.ItemDescription)) {
                                        found = row;
                                        logAndSave('DEBUG: Alignment MATCH: ' + JSON.stringify(row));
                                        break;
                                    }
                                }
                                if (found) {
                                    const alignPrice = Math.round(found.PriceLevel1 / 50) * 50;
                                    const alignDesc = found.ItemDescription;
                                    tyres.push({
                                        description: alignDesc || `Wheel Alignment (${alignObj.type})`,
                                        qty: 1,
                                        availableQty: 1,
                                        unitPrice: alignPrice,
                                        total: alignPrice
                                    });
                                } else {
                                    logAndSave('DEBUG: No alignment match found for type: ' + alignObj.type);
                                }
                            } else {
                                logAndSave('DEBUG: No alignment items found in DB after rear filtering');
                            }
                        }
                    } catch (e) { logAndSave('DEBUG: Alignment SQL error: ' + e.message); }
                }
                if (balancing) {
                    const balQty = requestedQty;
                    const balPrice = pricing.balancingPrice;
                    tyres.push({
                        description: `Wheel Balancing`,
                        qty: balQty,
                        availableQty: balQty,
                        unitPrice: balPrice,
                        total: balPrice * balQty
                    });
                }
                logAndSave('DEBUG: Filtered tyres for QTY >= ' + requestedQty + ': ' + JSON.stringify(tyres));
                if (tyres.length === 0) {
                    msg.reply(`No tyres available with requested quantity (${requestedQty}) for size ${tyreSize}.`);
                    logAndSave(`No tyres with enough stock for ${tyreSize} qty ${requestedQty}`);
                    return true;
                }
                const logoPath = path.join(__dirname, '../Logo.png');
                const quotationNumber = getNextQuotationNumber();
                // Add warranty note to be included in the PDF
                const warrantyNote = 'Warranty: 4 years/40000km from billing date';
                const pdfBytes = await generateQuotationPDFLib({ tyres, tyreSize, qty: requestedQty, logoPath, vehicleNo, customerName, credit, creditPeriod, quotationNumber, warrantyNote });
                const quotationsDir = 'C:/QUOTE';
                if (!fs.existsSync(quotationsDir)) fs.mkdirSync(quotationsDir, { recursive: true });
                const safeTyreSize = tyreSize.replace(/[^a-zA-Z0-9\-_.]/g, '-');
                const fileName = path.join(quotationsDir, `quotation_${quotationNumber}_${safeTyreSize}_${Date.now()}.pdf`);
                fs.writeFileSync(fileName, pdfBytes);
                try {
                    // BAILEYS MIGRATION: Use wrapper
                    const { MessageMedia } = require('../utils/baileysWrapper');
                    const media = MessageMedia.fromFilePath(fileName);
                    await msg.reply(media);
                    logAndSave(`Public Quotation PDF sent for ${tyreSize}: ${fileName}`);
                } catch (e) {
                    logAndSave(`Public Quotation PDF created for ${tyreSize}: ${fileName} (media send error)`);
                }
                // Add watermark or warning for public users
                await msg.reply('This is a public quotation. For best prices and more details, contact us directly.');
            } else {
                logAndSave(`No tyres for public quotation: ${tyreSize}`);
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
};
