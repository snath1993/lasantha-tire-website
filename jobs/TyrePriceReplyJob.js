// TyrePriceReplyJob (refactored for readability & maintainability)
// Responsibilities:
// 1. Detect tyre size in message (ignoring an included vehicle number if present)
// 2. If tyre size found, fetch available tyre records from DB
// 3. Decide whether to send a quotation PDF or a priced list
// 4. Apply pricing rules (brand adjustments, cost requests, motorbike rules)
// 5. Fallback to vehicle invoice handling if only vehicle no present

const { extractTyreSizeFlexible, extractVehicleNumber } = require('../utils/detect');
const { parsePriceAdjustments } = require('../utils/priceAdjust');
const { normalizeBrand } = require('../utils/brandUtils');

// Constants & helpers
const QUOTE_REGEX = /(quote|quot|qoute|qoutation|quotation|qout|quotion)/i; // tolerance for misspellings
// MIN_COST moved inside function for hot-reload
const ALLOWED_COST_CONTACTS = ['0777078700', '0777311770', '0771222509'];
const SPECIAL_MOTORBIKE_SIZES = ['100/90/17', '90/100/10', '140/60/17', '400/8'];
const { loadPricingEnv, roundToStep } = require('../utils/pricingConfig');
const safeReply = require('../utils/safeReply');

function computeSellingPrice({ tyre, tyreSize, normBrand, isMotorbike, isCostReq, isAllowedContact, isAllowedCostContact, adjustments }, pricing) {
    const rounded = roundToStep(tyre.UnitCost, pricing.baseRoundStep);

    // Cost-only (no markup) for privileged contacts
    if (isCostReq && isAllowedCostContact) return tyre.UnitCost;

    // Motorbike special uplift (only selected sizes)
    if (isMotorbike && SPECIAL_MOTORBIKE_SIZES.includes(tyreSize)) {
        return rounded + pricing.motorbikeExtra;
    }

    // Cost request by allowed contact (just rounded cost)
    if (isCostReq && isAllowedContact) return rounded;

    // Allowed contact brand-specific adjustments / discounts
    if (isAllowedContact) {
        if (normBrand === 'MAXXIS' || normBrand === 'MAXXIES') {
            const maxxisAdj = adjustments.find(a => a.brand && normalizeBrand(a.brand) === 'MAXXIS' && a.type === 'fixed' && a.value < 0);
            if (maxxisAdj) return rounded + maxxisAdj.value; // negative discount allowed
            return rounded; // base only
        }
        const brandAdj = adjustments.find(a => a.brand && normalizeBrand(a.brand) === normBrand && a.type === 'fixed');
        if (brandAdj) return rounded + brandAdj.value;
        return rounded + pricing.allowedDefaultMarkup;
    }

    // Public pricing (non-allowed)
    if (normBrand === 'MAXXIS' || normBrand === 'MAXXIES') {
        return rounded + pricing.maxxisPublicMarkup;
    }
    return rounded + pricing.publicBaseMarkup + pricing.publicExtraBuffer;
}

module.exports = async function TyrePriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    // Ignore WhatsApp system/broadcast/status messages (they are not user commands)
    // Examples include: 'status@broadcast', messages from broadcast lists, or other system senders
    if (typeof msg.from === 'string' && /broadcast|status@/i.test(msg.from)) {
        // Log and silently ignore
        try {
            logAndSave && logAndSave(`Ignored system/broadcast message from ${msg.from}: ${msg.body || '<no body>'}`);
        } catch (error) {
            console.warn('TyrePriceReplyJob: failed to record broadcast ignore', error);
        }
        return false;
    }

    const rawText = msg.body.trim();
    const senderNumber = msg.from.replace('@c.us', '');
    const vehicleNo = extractVehicleNumber(rawText);
    if (vehicleNo) {
        logAndSave(`Detected vehicle number: ${vehicleNo} in message from ${senderNumber}`);
    }
    // Remove vehicle no (if present) for tyre detection context
    const textForTyre = vehicleNo ? rawText.replace(vehicleNo, '') : rawText;
    const tyreSize = extractTyreSizeFlexible(textForTyre);
    // Accept both local (0...) and international (94...) formats when checking allowed lists
    const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
    const isAllowedContact = allowedContacts.includes(senderNumber) || allowedContacts.includes(senderNormalizedTo0);
    const isAllowedCostContact = ALLOWED_COST_CONTACTS.includes(senderNumber) || ALLOWED_COST_CONTACTS.includes(senderNormalizedTo0);

    // If tyre size not present but vehicle number is, delegate to invoice job
    if (!tyreSize) {
        if (vehicleNo) {
            const VehicleInvoiceReplyJob = require('./VehicleInvoiceReplyJob');
            await VehicleInvoiceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
            return true;
        }
        return false; // let other jobs attempt
    }

    // Handle quotation-only (avoid price list) or qty queries early
    const wantsQtyOnly = /(qty|quantity)/i.test(rawText);
    const wantsQuotation = QUOTE_REGEX.test(rawText);
    const isCostRequest = /cost/i.test(rawText);

    // If this is a cost request, delegate to CostPriceReplyJob (independent job)
    if (isCostRequest) {
        const CostPriceReplyJob = require('./CostPriceReplyJob');
        const handled = await CostPriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
        if (handled) return true;
        // if not handled (e.g., not allowed), continue to normal flow
    }

    try {
        const pricing = loadPricingEnv();
        const MIN_COST = parseInt(process.env.MIN_TYRE_COST || '3000', 10);
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

        const tyres = (result.recordset || []).filter(t =>
            (t.ItemDescription.includes(tyreSize) || t.ItemDescription.replace(/\//g, '').includes(tyreSize.replace(/\//g, '')))
            && t.UnitCost > MIN_COST
        );

        if (tyres.length === 0) {
            await safeReply(msg, undefined, msg.from, `*${tyreSize}* is currently out of stock (QTY zero).`);
            logAndSave(`Out of stock reply for size ${tyreSize} to ${senderNumber}`);
            return true;
        }

        // Quotation PDF path
        if (wantsQuotation) {
            // Extract requested quantity (digits before the quote keyword)
            const qtyMatch = rawText.match(/(\d+)\s*(?=(quote|quot|qoute|qoutation|quotation|qout|quotion))/i);
            const requestedQty = qtyMatch ? parseInt(qtyMatch[1], 10) : null;
            const { sendQuotationPDF } = require('./TyreQuotationPDFLibJob');
            if (isAllowedContact && typeof sendQuotationPDF === 'function') {
                await sendQuotationPDF(msg, tyres, tyreSize, senderNumber, requestedQty);
                logAndSave(`Advanced Quotation PDF sent for ${tyreSize} to ${senderNumber} qty ${requestedQty || '-'} `);
            } else {
                // Fallback to public quotation flow for non-allowed contacts
                try {
                    const TyreQuotationPDFLibJobPublic = require('./TyreQuotationPDFLibJobPublic');
                    const handled = await TyreQuotationPDFLibJobPublic(msg, sql, sqlConfig, logAndSave);
                    if (handled) return true;
                } catch (error) {
                    logAndSave(`TyrePriceReplyJob: public quotation flow failed - ${error.message}`);
                }
            }
            return true; // Do not send price list as well
        }

        if (wantsQtyOnly) {
            // Another job should have handled quantity separately; just acknowledge handled
            return true;
        }

        // Prepare adjustments (only allowed contacts can send them)
        const adjustments = isAllowedContact ? parsePriceAdjustments(rawText) : [];

        // Default: prepare and send price list reply
        {
            let reply = `*Tyre Price List*\nTyre Size: *${tyreSize}*\n\n`;
            // adjustments already parsed above
            tyres.forEach(t => {
                const description = (t.ItemDescription || '').replace(new RegExp(tyreSize, 'gi'), '').trim();
                const normBrand = normalizeBrand(t.Custom3 || '');
                const isMotorbike = (t.Categoty && t.Categoty.toUpperCase().includes('MOTOR')) || false;
                const sellingPrice = computeSellingPrice({
                    tyre: t,
                    tyreSize,
                    normBrand,
                    isMotorbike,
                    isCostReq: isCostRequest,
                    isAllowedContact,
                    isAllowedCostContact,
                    adjustments
                }, pricing);
                reply += `${description} ${sellingPrice}/=\n\n`;
            });
            reply += '-----------------------------\n💵 Cash Price Per Tyre 💵';
            if (!isAllowedContact) {
                reply += '\n\n📞 *This is an AI generated message. Call this number for negotiations: 0771222509*';
            }
                await safeReply(msg, undefined, msg.from, reply.trim());
                logAndSave(`Reply sent for size ${tyreSize} to ${senderNumber}: ${reply.trim().slice(0,200)}...`);
            return true;
        }
    } catch (err) {
        try {
            await safeReply(msg, undefined, msg.from, 'Error connecting to SQL Server.');
        } catch (replyError) {
            logAndSave(`TyrePriceReplyJob: failed to send SQL error reply - ${replyError.message}`);
        }
        logAndSave(`SQL error: ${err.message}`);
        return true; // message handled (error case)
    } finally {
        if (sql.connected) {
            try {
                await sql.close();
            } catch (closeError) {
                logAndSave(`TyrePriceReplyJob: failed to close SQL connection - ${closeError.message}`);
            }
        }
    }
};
