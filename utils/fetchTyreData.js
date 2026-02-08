// =================================================================
// Fetch Tyre Data Utility
// Reusable function to fetch tyre data using production logic
// =================================================================

const sql = require('mssql');
const { normalizeBrand } = require('./brandUtils');
const { loadPricingEnv, roundToStep } = require('./pricingConfig');
const { parsePriceAdjustments } = require('./priceAdjust');

const MIN_COST = parseInt(process.env.MIN_TYRE_COST || '3000', 10);
const ALLOWED_COST_CONTACTS = ['0777078700', '0777311770', '0771222509'];
const SPECIAL_MOTORBIKE_SIZES = ['100/90/17', '90/100/10', '140/60/17', '400/8'];

/**
 * Compute selling price with all business logic applied
 */
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

/**
 * Fetch tyre data with full production logic
 * @param {object} mainPool - SQL connection pool
 * @param {string} tyreSize - Tyre size (e.g., "195/65R15")
 * @param {string|null} brand - Brand filter (optional)
 * @param {string} senderNumber - User's phone number
 * @param {array} allowedContacts - List of allowed contact numbers
 * @param {string} rawText - Original message text for adjustments
 * @returns {Promise<object>} - { tyres: [...], formatted: "...", count: N }
 */
async function fetchTyreData(mainPool, tyreSize, brand = null, senderNumber = '', allowedContacts = [], rawText = '') {
    try {
        // Normalize sender number
        const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
        const isAllowedContact = allowedContacts.includes(senderNumber) || allowedContacts.includes(senderNormalizedTo0);
        const isAllowedCostContact = ALLOWED_COST_CONTACTS.includes(senderNumber) || ALLOWED_COST_CONTACTS.includes(senderNormalizedTo0);
        const isCostRequest = /cost/i.test(rawText);
        
        // Parse price adjustments (only for allowed contacts)
        const adjustments = isAllowedContact ? parsePriceAdjustments(rawText) : [];
        
        // Load pricing configuration
        const pricing = loadPricingEnv();

        // Build robust size variants: no-slash, with/without 'R'
        const normalized = String(tyreSize).replace(/\s+/g, '').toUpperCase();
        const noSlash = normalized.replace(/\//g, '');
        const withR = /R\d+$/.test(normalized) ? normalized : normalized.replace(/(\d+\/\d+)(\/)(\d+)$/, '$1R$3');
        const noR = normalized.replace(/R(?=\d+$)/, '');

        // Query database using production views
        let query = `
            SELECT 
                im.ItemDescription,
                im.UnitCost,
                im.Categoty,
                im.Custom3 as BrandName,
                im.Custom4 as PatternName,
                iw.QTY as Qty
            FROM [View_Item Master Whatsapp] im
            JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            WHERE im.Categoty = 'TYRES'
            AND iw.QTY > 0
            AND (
                im.ItemDescription LIKE @tyreSize
                OR im.ItemDescription LIKE @tyreSizeNoSlash
                OR im.ItemDescription LIKE @withR
                OR im.ItemDescription LIKE @noR
            )
            ${brand ? 'AND im.Custom3 LIKE @brand' : ''}
            ORDER BY im.UnitCost ASC
        `;

        let request = new sql.Request(mainPool)
            .input('tyreSize', sql.NVarChar(100), `%${normalized}%`)
            .input('tyreSizeNoSlash', sql.NVarChar(100), `%${noSlash}%`)
            .input('withR', sql.NVarChar(100), `%${withR}%`)
            .input('noR', sql.NVarChar(100), `%${noR}%`);

        if (brand) {
            request = request.input('brand', sql.NVarChar(50), `%${brand}%`);
        }

        let result = await request.query(query);

        // Filter by MIN_COST and size match
        let tyres = (result.recordset || []).filter(t => {
            const desc = (t.ItemDescription || '').toUpperCase().replace(/\s+/g, '');
            return (
                desc.includes(normalized) ||
                desc.includes(noSlash) ||
                desc.includes(withR) ||
                desc.includes(noR)
            ) && t.UnitCost > MIN_COST;
        });

        // If brand filter yielded zero results, fallback without brand
        if (tyres.length === 0 && brand) {
            const noBrandQuery = query.replace(/\s+AND im\.Custom3 LIKE @brand/, '');
            const req2 = new sql.Request(mainPool)
                .input('tyreSize', sql.NVarChar(100), `%${normalized}%`)
                .input('tyreSizeNoSlash', sql.NVarChar(100), `%${noSlash}%`)
                .input('withR', sql.NVarChar(100), `%${withR}%`)
                .input('noR', sql.NVarChar(100), `%${noR}%`);
            const res2 = await req2.query(noBrandQuery);
            tyres = (res2.recordset || []).filter(t => {
                const desc = (t.ItemDescription || '').toUpperCase().replace(/\s+/g, '');
                return (
                    desc.includes(normalized) ||
                    desc.includes(noSlash) ||
                    desc.includes(withR) ||
                    desc.includes(noR)
                ) && t.UnitCost > MIN_COST;
            });
        }

        if (tyres.length === 0) {
            return {
                tyres: [],
                formatted: `*${tyreSize}* is currently out of stock (QTY zero).`,
                count: 0,
                outOfStock: true
            };
        }

        // Format price list with computed prices
    let formattedList = `*Tyre Price List*\nTyre Size: *${normalized}*\n\n`;
        
        const processedTyres = tyres.map(t => {
            const description = (t.ItemDescription || '').trim();
            const normBrand = normalizeBrand(t.BrandName || t.Custom3 || '');
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

            formattedList += `${description} ${sellingPrice}/=\n\n`;

            return {
                ...t,
                description,
                sellingPrice,
                normBrand,
                isMotorbike
            };
        });

        formattedList += '-----------------------------\n💵 Cash Price Per Tyre 💵';
        
        if (!isAllowedContact) {
            formattedList += '\n\n📞 *This is an AI generated message. Call this number for negotiations: 0771222509*';
        }

        return {
            tyres: processedTyres,
            formatted: formattedList.trim(),
            count: processedTyres.length,
            outOfStock: false,
            isAllowedContact,
            pricing: {
                showCost: isAllowedCostContact,
                hasAdjustments: adjustments.length > 0
            }
        };

    } catch (error) {
        console.error('[fetchTyreData Error]', error);
        throw error;
    }
}

module.exports = {
    fetchTyreData,
    computeSellingPrice
};
