// handlers/BrandStockCheckHandler.js
const sql = require('mssql');
const { getPool } = require('../utils/sqlPool');
const { logAndSave } = require('../utils/schedulerUtils');
const { normalizeBrand } = require('../utils/brandUtils');

/**
 * Advanced Brand Stock Analyzer
 * 1. Checks if the message is a Brand Name (from Item Master).
 * 2. If it is a Brand, fetches all items for that Brand.
 * 3. Applies "Active Item" filter:
 *    - Current Stock > 0
 *    - OR Sold within last 3 months
 * 4. Generates a smart report.
 */
class BrandStockCheckHandler {
    constructor() {
        this.cacheTime = 0;
        this.cachedBrands = [];
    }

    /**
     * Entry point to handle a message.
     * @param {object} message - The WhatsApp message object
     * @param {string} rawText - The raw message body
     * @returns {boolean} - True if handled, False otherwise
     */
    async handle(message, rawText) {
        if (!rawText) return false;
        
        const searchText = rawText.trim().toUpperCase();
        
        // Quick check: is this likely a brand?
        // Load brands if cache expired (every hour)
        if (Date.now() - this.cacheTime > 3600000) {
            await this.refreshBrandCache();
        }

        // Fuzzy match brand using normalizeBrand logic or direct includes
        // We look for a known brand in our cache that matches the input.
        // Or if the input *IS* the brand.
        const matchedBrand = this.findMatchingBrand(searchText);

        if (!matchedBrand) return false; // Not a brand request

        logAndSave(`🔍 Brand Query Detected: "${searchText}" -> Matched: "${matchedBrand}"`);
        
        try {
            await this.generateAndReply(message, matchedBrand, searchText);
            return true;
        } catch (e) {
            logAndSave(`❌ BrandStockCheckHandler Error: ${e.message}`);
            return false;
        }
    }

    async refreshBrandCache() {
        try {
            const pool = await getPool();
            const result = await pool.request().query(`
                SELECT DISTINCT Custom3 as Brand 
                FROM [View_Item Master Whatsapp] 
                WHERE Custom3 IS NOT NULL AND Custom3 <> ''
            `);
            this.cachedBrands = result.recordset.map(r => r.Brand.toUpperCase().trim());
            this.cacheTime = Date.now();
            logAndSave(`✅ Brand Cache Refreshed: ${this.cachedBrands.length} brands loaded.`);
        } catch (e) {
            console.error('Failed to refresh brand cache:', e);
        }
    }

    findMatchingBrand(text) {
        // 1. Exact match
        if (this.cachedBrands.includes(text)) return text;

        // 2. Fuzzy Match (Strict - Distance 1) BEFORE Prefix Match
        // This solves "MAXXIS" input matching "MAXXIES" (Dist 1) instead of "MAXXIS MOTOR BIKE" (Prefix).
        // If the user made a tiny typo on a MAIN brand, we should catch that before assuming they typed a prefix of a sub-brand.
        let strictFuzzyMatch = null;
        let minStrictDist = 999;
        
        for (const brand of this.cachedBrands) {
            const dist = this.levenshtein(text, brand);
            // Very strict: Only allow 1 char difference
            if (dist === 1 && dist < minStrictDist) {
                minStrictDist = dist;
                strictFuzzyMatch = brand;
            }
        }
        if (strictFuzzyMatch) return strictFuzzyMatch;

        // 3. Check if BRAND starts with Input (Autocomplete style, e.g. Input: "CEAT MO" -> Brand: "CEAT MOTORBIKE")
        // We prioritize this over prefix match to capture specific sub-brands like "CEAT MOTORBIKE"
        const potentialMatches = this.cachedBrands.filter(b => b.startsWith(text));
        if (potentialMatches.length > 0) {
            // Return the shortest match (e.g. if Input is "CEAT", prefer "CEAT" over "CEAT MOTORBIKE")
            // BUT if Input is "CEAT MO", "CEAT" won't be here, only "CEAT MOTORBIKE".
            return potentialMatches.sort((a, b) => a.length - b.length)[0];
        }

        // 4. Check if INPUT starts with a Brand (e.g. Input: "CEAT 100/90", Brand: "CEAT")
        // We match the LONGEST brand that text starts with.
        const prefixMatches = this.cachedBrands.filter(b => text.startsWith(b));
        if (prefixMatches.length > 0) {
            // Return longest match (e.g. if we have "CEAT" and "CEAT PREMIUM", and input is "CEAT PREMIUM...", pick longer)
            return prefixMatches.sort((a, b) => b.length - a.length)[0];
        }

        // 5. Fuzzy Match (Loose - Distance 2+)
        // Allow slightly more tolerance for longer words
        let bestMatch = null;
        let minDist = 999;

        for (const brand of this.cachedBrands) {
            const dist = this.levenshtein(text, brand);
            const tolerance = brand.length > 5 ? 2 : 1; // Allow 2 errors for long words, 1 for short

            if (dist <= tolerance && dist < minDist) {
                minDist = dist;
                bestMatch = brand;
            }
        }
        
        return bestMatch;
    }

    // Standard Levenshtein Distance Algorithm
    levenshtein(a, b) {
        if (a.length === 0) return b.length;
        if (b.length === 0) return a.length;

        const matrix = [];

        // increment along the first column of each row
        var i;
        for (i = 0; i <= b.length; i++) {
            matrix[i] = [i];
        }

        // increment each column in the first row
        var j;
        for (j = 0; j <= a.length; j++) {
            matrix[0][j] = j;
        }

        // Fill in the rest of the matrix
        for (i = 1; i <= b.length; i++) {
            for (j = 1; j <= a.length; j++) {
                if (b.charAt(i - 1) == a.charAt(j - 1)) {
                    matrix[i][j] = matrix[i - 1][j - 1];
                } else {
                    matrix[i][j] = Math.min(
                        matrix[i - 1][j - 1] + 1, // substitution
                        Math.min(
                            matrix[i][j - 1] + 1, // insertion
                            matrix[i - 1][j] + 1  // deletion
                        )
                    );
                }
            }
        }

        return matrix[b.length][a.length];
    }

    async generateAndReply(message, brand, rawText) {
        const pool = await getPool();
        const request = pool.request();
        
        // 1. Get All Items for Brand with Stock
        // We join Master and Whse Views
        const inventoryQuery = `
            SELECT 
                im.ItemID as ItemCode,
                im.ItemDescription as Description,
                im.Categoty, 
                ISNULL(iw.QTY, 0) as OnHand
            FROM [View_Item Master Whatsapp] im
            LEFT JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID
            WHERE im.Custom3 = @Brand
        `;
        
        request.input('Brand', sql.NVarChar, brand);
        const invResult = await request.query(inventoryQuery);
        let allItems = invResult.recordset;

        if (allItems.length === 0) {
            await message.reply(`❌ No items found for brand: ${brand}`);
            return;
        }

        // 2. Get Recent Sales (Last 1 Month) for this Brand
        // We only care about ItemIDs that have been sold.
        const cutoffDate = new Date();
        cutoffDate.setMonth(cutoffDate.getMonth() - 1); // Changed from 3 months to 1 month
        
        const salesRequest = pool.request();
        salesRequest.input('CutoffDate', sql.DateTime, cutoffDate);
        salesRequest.input('Brand', sql.NVarChar, brand);
        
        const salesQuery = `
            SELECT DISTINCT Expr3 as ItemCode
            FROM [View_Sales report whatsapp] d
            JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
            WHERE d.Expr1 >= @CutoffDate
            AND im.Custom3 = @Brand
        `;
        
        const salesResult = await salesRequest.query(salesQuery);
        const activeItemSet = new Set(salesResult.recordset.map(r => r.ItemCode));

        // 3. Process Logic
        // Active Item = (Stock > 0) OR (Sold in last 1 month)
        
        const criticalItems = []; // Out of Stock (Stock 0 but Active)
        const lowItems = [];      // Low Stock (0 < Stock < 4)
        const healthyItems = [];  // Healthy (Stock >= 4)
        
        // Also capture "Inactive with Stock" (Dead stock?) - Maybe just classify as Healthy/Low based on stock
        // The user said: "re stock nokaranna thiranaya karala... mathakadi sell una ewa ekka compare karanna"
        // So if Stock is 0 AND NOT Sold recently -> It's Dead/Discontinued. Don't show.
        
        for (const item of allItems) {
            const isSoldRecently = activeItemSet.has(item.ItemCode);
            const stock = item.OnHand;

            if (stock === 0) {
                if (isSoldRecently) {
                    // Critical: No stock, but demand exists
                    criticalItems.push(item);
                } else {
                    // Dead stock: No stock, No demand. Ignore.
                }
            } else if (stock < 4) {
                // Has stock, but low. It IS active because it has stock.
                lowItems.push(item);
            } else {
                // Healthy stock
                healthyItems.push(item);
            }
        }

        // 4. Build Report
        if (criticalItems.length === 0 && lowItems.length === 0 && healthyItems.length === 0) {
            await message.reply(`ℹ️ No active stock or sales found for *${brand}* in last 3 months.`);
            return;
        }

        let response = `📊 *SMART STOCK: ${brand}*\n`;
        response += `_(Active Items Only - Last 3 Months)_ \n\n`;

        if (criticalItems.length > 0) {
            response += `🔴 *OUT OF STOCK (Order Now)*\n`;
            criticalItems.forEach(i => {
                response += `- ${i.Description}\n`;
                response += `  Target: 4 Pcs\n`;
            });
            response += `\n`;
        }

        if (lowItems.length > 0) {
            response += `🟡 *LOW STOCK (Running Out)*\n`;
            lowItems.forEach(i => {
                const need = 4 - i.OnHand;
                response += `- ${i.Description}\n`;
                response += `  Stock: ${i.OnHand} | Need: ${need}\n`;
            });
            response += `\n`;
        }

        if (healthyItems.length > 0) {
            response += `🟢 *HEALTHY (Stock >= 4)*\n`;
            response += `  ${healthyItems.length} items hidden.\n`;
        }

        response += `\n_Generated by WhatsApp Bot 🤖_`;

        await message.reply(response);
    }
}

module.exports = new BrandStockCheckHandler();
