// TyreQuotationPDFLibJob
// Generate and reply advanced quotation PDF using pdf-lib
const { generateQuotationPDFLib } = require('../utils/generateQuotationPDFLib');
const fs = require('fs');
const path = require('path');
const { extractTyreSizeFlexible } = require('../utils/detect');
const { parsePriceAdjustments } = require('../utils/priceAdjust');
const { parseAlignmentBalancing } = require('../utils/alignmentBalancing');
const { loadAllowedQuotationConfig, roundToStep } = require('../utils/quotationConfig');

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

module.exports = async function TyreQuotationPDFLibJob(msg, sql, sqlConfig, logAndSave) {
    const text = msg.body.trim();
    // Extract vehicle number first
    const { extractVehicleNumber } = require('../utils/detect');
    const detectedVehicleNo = extractVehicleNumber(text);
    // Pass vehicle number to adjustment parser to ignore its numbers
    const adjustments = parsePriceAdjustments(text, detectedVehicleNo ? [detectedVehicleNo] : []);
    const tyreSize = extractTyreSizeFlexible(text);
    // Detect flexible quote keywords
    const quoteRegex = /(quotation|quote|quot|qout|qoute|quotion|qoutation|කෝට්|ඉල්ලීම|கோட்|கோட்டேஷன்)/i;
    if (tyreSize && quoteRegex.test(text)) {
        const pricing = loadAllowedQuotationConfig();
        // Parse quantity, vehicle number, customer name, payment terms
        // Example: 185/65/15 quote 4/CBH-6483/Lasantha/credit
        // Improved quantity extraction: use number after 'quote' (or variations), else default to 1
        let requestedQty = 1;
        let vehicleNo = '';
        let customerName = '';
        let credit = '';
        let creditPeriod = '';
        // Find number after quote/quotation
        const quoteQtyMatch = text.match(/(?:quote|quot|qoute|qoutation|quotation|qout|quotion)[^\d]*(\d+)/i);
        if (quoteQtyMatch) {
            requestedQty = parseInt(quoteQtyMatch[1], 10);
        } // else keep requestedQty = 1

        // Vehicle number already extracted above
        if (detectedVehicleNo) {
            vehicleNo = detectedVehicleNo;
        }

        // Split by / and look for keywords
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
                // Only assign customerName if not already set as vehicleNo
                if (!vehicleNo || p.toUpperCase() !== vehicleNo) {
                    customerName = p;
                }
            }
        });
        if (!credit) credit = 'Cash';
        try {
            await sql.connect(sqlConfig);
            // Fetch both TYRES and WHEEL ALIGNMENT items for this size/quotation
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
            // Parse price adjustments from message (already done above)
            const isAllowedContact = true; // Quotation PDF is only for allowed contacts in your logic
            if (result.recordset.length > 0) {
                // Only include tyres with enough stock for requestedQty
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
                            let price = roundToStep(tyre.UnitCost, pricing.baseRoundStep) + pricing.allowedDefaultMarkup;
                            if (credit === 'Credit') price += pricing.creditMarkup;
                            if (isAllowedContact && adjustments.length > 0) {
                                // Always compare brand case-insensitively
                                const tyreBrand = brand.toUpperCase();
                                let adjustmentAttemptedForMaxxis = false;
                                adjustments.forEach(adj => {
                                    let brandMatch = true;
                                    if (adj.brand) {
                                        if (adj.exclude) {
                                            if (tyreBrand === normalizeBrand(adj.brand)) brandMatch = false;
                                        } else {
                                            if (tyreBrand !== normalizeBrand(adj.brand)) brandMatch = false;
                                        }
                                    }
                                    // If user tried to adjust MAXXIS/MAXXIES, note it for feedback
                                    if ((adj.brand && (normalizeBrand(adj.brand) === 'MAXXIS' || normalizeBrand(adj.brand) === 'MAXXIES')) && !adj.exclude) {
                                        adjustmentAttemptedForMaxxis = true;
                                    }
                                    if (brandMatch) {
                                        if (adj.type === 'percent') {
                                            price = Math.round(price * (1 + adj.value / 100));
                                        } else if (adj.type === 'fixed') {
                                            price = price + adj.value;
                                        }
                                    }
                                });
                                // If user tried to adjust MAXXIS/MAXXIES, send feedback
                                if (adjustmentAttemptedForMaxxis && (tyreBrand === 'MAXXIS' || tyreBrand === 'MAXXIES')) {
                                    msg.reply('Note: MAXXIS/MAXXIES prices are fixed and cannot be adjusted.');
                                }
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

                // Add alignment/balancing if requested
                const { alignment, balancing } = parseAlignmentBalancing(text);
                if (alignment) {
                    // Enhanced logic: add both normal and rear items if both keywords are present, never mix car and jeep items
                    let alignTypes = [];
                    const hasCar = /car/i.test(text);
                    const hasRearCar = /rear\s*car/i.test(text);
                    const hasJeep = /jeep/i.test(text);
                    const hasRearJeep = /rear\s*jeep/i.test(text);
                    // Only allow one vehicle type per quotation
                    if ((hasCar || hasRearCar) && !(hasJeep || hasRearJeep)) {
                        if (hasCar) alignTypes.push({ type: 'car|cars', rear: false });
                        if (hasRearCar) alignTypes.push({ type: 'car|cars', rear: true });
                    } else if ((hasJeep || hasRearJeep) && !(hasCar || hasRearCar)) {
                        if (hasJeep) alignTypes.push({ type: 'jeep|jeeps', rear: false });
                        if (hasRearJeep) alignTypes.push({ type: 'jeep|jeeps', rear: true });
                    } else {
                        // Default: use detected alignment type
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
                            // Find best match for alignment type
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
                    // Balancing price: 600 per tyre, qty = requestedQty
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
                // Logo path (update as needed)
                const logoPath = path.join(__dirname, '../Logo.png');
                const quotationNumber = getNextQuotationNumber();
                // Generate PDF buffer
                // Add warranty note to be included in the PDF
                const warrantyNote = 'Warranty: 4 years/40000km from billing date';
                const pdfBytes = await generateQuotationPDFLib({ tyres, tyreSize, qty: requestedQty, logoPath, vehicleNo, customerName, credit, creditPeriod, quotationNumber, warrantyNote });
                // Save PDF to file in quotations folder
                const quotationsDir = 'C:/QUOTE';
                if (!fs.existsSync(quotationsDir)) fs.mkdirSync(quotationsDir, { recursive: true });
                // Sanitize tyreSize for filename
                const safeTyreSize = tyreSize.replace(/[^a-zA-Z0-9\-_.]/g, '-');
                const fileName = path.join(quotationsDir, `quotation_${quotationNumber}_${safeTyreSize}_${Date.now()}.pdf`);
                fs.writeFileSync(fileName, pdfBytes);
                // Reply as media
                try {
                    // BAILEYS MIGRATION: Use wrapper
                    const { MessageMedia } = require('../utils/baileysWrapper');
                    const media = MessageMedia.fromFilePath(fileName);
                    await msg.reply(media);
                    logAndSave(`Advanced Quotation PDF sent for ${tyreSize}: ${fileName}`);
                } catch (e) {
                    logAndSave(`Advanced Quotation PDF created for ${tyreSize}: ${fileName} (media send error)`);
                }
                // Special: Send adjustment summary if any brand-specific or global adjustment was applied
                const adjustmentSummary = tyres
                    .map(tyre => {
                        const showBrand = tyre.brand || '';
                        if (!showBrand) return null;
                        const brandAdjs = adjustments.filter(a =>
                            (a.brand === showBrand || a.brand === null) && !a.exclude
                        );
                        if (brandAdjs.length > 0) {
                            const changes = brandAdjs.map(adj => {
                                if (adj.type === 'fixed') {
                                    return `${adj.value > 0 ? '+' : ''}${adj.value}`;
                                } else if (adj.type === 'percent') {
                                    return `${adj.value > 0 ? '+' : ''}${adj.value}%`;
                                }
                                return '';
                            }).join(', ');
                            return `${showBrand}: ${tyre.unitPrice}/= (adjusted by ${changes})`;
                        }
                        return null;
                    })
                    .filter(Boolean)
                    .join('\n');
                if (adjustmentSummary) {
                    await msg.reply(`*Special Price Adjustments Applied:*
${adjustmentSummary}`);
                }
            } else {
                // Only reply if no tyres found (error case)
                logAndSave(`No tyres for advanced quotation: ${tyreSize}`);
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

// Named export used by TyrePriceReplyJob when it already has filtered tyres
// Signature: (msg, tyres, tyreSize, senderNumber, requestedQty)
async function sendQuotationPDF(msg, tyres, tyreSize, senderNumber, requestedQty) {
    const logoPath = path.join(__dirname, '../Logo.png');
    const quotationNumber = getNextQuotationNumber();
    const vehicleNo = '';
    const customerName = '';
    const credit = 'Cash';
    const creditPeriod = '';
    const warrantyNote = 'Warranty: 4 years/40000km from billing date';
    const pdfBytes = await generateQuotationPDFLib({ tyres, tyreSize, qty: requestedQty || 1, logoPath, vehicleNo, customerName, credit, creditPeriod, quotationNumber, warrantyNote });

    const quotationsDir = 'C:/QUOTE';
    if (!fs.existsSync(quotationsDir)) fs.mkdirSync(quotationsDir, { recursive: true });
    const safeTyreSize = String(tyreSize).replace(/[^a-zA-Z0-9\-_.]/g, '-');
    const fileName = path.join(quotationsDir, `quotation_${quotationNumber}_${safeTyreSize}_${Date.now()}.pdf`);
    fs.writeFileSync(fileName, pdfBytes);

    try {
        // BAILEYS MIGRATION: Use wrapper
        const { MessageMedia } = require('../utils/baileysWrapper');
        const media = MessageMedia.fromFilePath(fileName);
        await msg.reply(media);
    } catch (e) {
        // Fallback: notify path
        try { await msg.reply(`Quotation PDF created for ${tyreSize}: ${fileName}`); } catch {}
    }
}

module.exports.sendQuotationPDF = sendQuotationPDF;
