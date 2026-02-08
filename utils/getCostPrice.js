// Constants for cost price calculation
const MONTHLY_PROFIT_INCLUDE = process.env.MONTHLY_PROFIT_INCLUDE === '1' || process.env.REPORT_INCLUDE_PROFIT === '1';
const _MONTHLY_COST_CACHE = new Map();

async function getAdvancedCostPrice(sql, description) {
    if (!MONTHLY_PROFIT_INCLUDE || !description) return null;
    if (_MONTHLY_COST_CACHE.has(description)) return _MONTHLY_COST_CACHE.get(description);

    try {
        console.log('[MONTHLY-PROFIT] Processing:', description);

        // Clean and standardize the description
        const cleanDesc = description.toUpperCase().trim()
            .replace(/\s+/g, ' ')  // Normalize spaces
            .replace(/TYRES?$/, '') // Remove trailing TYRE/TYRES
            .trim();

        // Extract size and brand pattern
        const sizePattern = description.match(/\b\d{3}\/\d{2}[R]?\d{2}\b/i);
        if (!sizePattern) {
            console.log('[MONTHLY-PROFIT] No size pattern found');
            _MONTHLY_COST_CACHE.set(description, null);
            return null;
        }

        const sizeMatch = sizePattern[0].replace(/\s+/g, '');
        const brandPattern = cleanDesc.split(' ')[0];

        console.log('[MONTHLY-PROFIT] Extracted:', {
            size: sizeMatch,
            brand: brandPattern,
            clean: cleanDesc
        });

        // Try exact match first using the view
        const query1 = `
            SELECT TOP 5 
                m.ItemDescription, 
                m.UnitCost,
                m.Categoty as Category
            FROM [View_Item Master Whatsapp] m
            WHERE 
                m.Categoty = 'TYRES'
                AND m.UnitCost > 0 
                AND (
                    UPPER(LTRIM(RTRIM(m.ItemDescription))) = @cleanDesc 
                    OR (
                        UPPER(LTRIM(RTRIM(m.ItemDescription))) LIKE @sizeMatch 
                        AND UPPER(LTRIM(RTRIM(m.ItemDescription))) LIKE @brandMatch
                    )
                ) 
            ORDER BY 
                CASE WHEN UPPER(LTRIM(RTRIM(m.ItemDescription))) = @cleanDesc THEN 0 ELSE 1 END,
                m.UnitCost DESC
        `;

        const result = await sql.query(query1
            .replace(/@cleanDesc/g, `'${cleanDesc}'`)
            .replace(/@sizeMatch/g, `'%${sizeMatch}%'`)
            .replace(/@brandMatch/g, `'%${brandPattern}%'`)
        );

        console.log('[MONTHLY-PROFIT] Query1:', query1, {cleanDesc, sizeMatch, brandPattern});
        if (result.recordset?.length > 0) {
            console.log('[MONTHLY-PROFIT] Query1 matches:', result.recordset.map(r => ({desc: r.ItemDescription, cost: r.UnitCost})));
            const match = result.recordset[0];
            _MONTHLY_COST_CACHE.set(description, match.UnitCost);
            return match.UnitCost;
        } else {
            console.log('[MONTHLY-PROFIT] No match found for', description);
        }

        // Fallback: try matching by size only using the view
        const query2 = `
            SELECT TOP 5 
                m.ItemDescription, 
                m.UnitCost,
                m.Categoty as Category
            FROM [View_Item Master Whatsapp] m
            WHERE 
                m.Categoty = 'TYRES'
                AND m.UnitCost > 0 
                AND UPPER(LTRIM(RTRIM(m.ItemDescription))) LIKE @sizeMatch 
            ORDER BY m.UnitCost DESC
        `;

        const fallback = await sql.query(query2.replace(/@sizeMatch/g, `'%${sizeMatch}%'`));
        console.log('[MONTHLY-PROFIT] Query2:', query2, {sizeMatch});
        if (fallback.recordset?.length > 0) {
            console.log('[MONTHLY-PROFIT] Fallback size-only matches:', fallback.recordset.map(r => ({desc: r.ItemDescription, cost: r.UnitCost})));
            const match = fallback.recordset[0];
            _MONTHLY_COST_CACHE.set(description, match.UnitCost);
            return match.UnitCost;
        } else {
            console.log('[MONTHLY-PROFIT] No fallback size-only match for', description);
        }

        _MONTHLY_COST_CACHE.set(description, null);
        return null;

    } catch (error) {
        console.error('[MONTHLY-PROFIT] Error getting cost price:', error);
        _MONTHLY_COST_CACHE.set(description, null);
        return null;
    }
}

// Export the function
module.exports = {
    getAdvancedCostPrice
};