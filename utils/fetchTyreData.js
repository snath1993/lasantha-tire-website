/**
 * Fetch Tyre Data from Database
 */
const sql = require('mssql');

/**
 * Fetch tyre data from database
 * @param {object} pool - SQL connection pool
 * @param {string} tyreSize - Tyre size (e.g., "195/65R15")
 * @param {string|null} brand - Brand name (optional)
 * @param {string} senderNumber - Sender phone number
 * @param {Array} allowedContacts - List of allowed contacts
 * @param {string} text - Original message text
 * @returns {Promise<object>} - Tyre data
 */
async function fetchTyreData(pool, tyreSize, brand, senderNumber, allowedContacts, text) {
    try {
        const request = new sql.Request(pool);
        request.input('size', sql.VarChar(50), tyreSize);
        
        let query = `
            SELECT TOP 10
                im.BrandName as Brand,
                im.PatternName as Pattern,
                CONCAT(im.TyreWidth, '/', im.TyreAspectRatio, 'R', im.TyreDiameter) as Size,
                im.LoadIndex,
                im.SpeedRating,
                ISNULL(im.TotalQty, 0) as Qty,
                im.UnitCost as Price,
                im.ItemCode
            FROM [View_Item Master Whatsapp] im
            WHERE CONCAT(im.TyreWidth, '/', im.TyreAspectRatio, 'R', im.TyreDiameter) = @size
            AND im.Categoty LIKE '%Tyre%'
        `;
        
        if (brand) {
            request.input('brand', sql.VarChar(50), brand);
            query += ` AND im.BrandName = @brand`;
        }
        
        query += ` ORDER BY im.TotalQty DESC, im.UnitCost ASC`;
        
        const result = await request.query(query);
        
        return {
            count: result.recordset.length,
            tyres: result.recordset,
            tyreSize: tyreSize,
            brand: brand,
            formatted: null
        };
    } catch (error) {
        console.error('[FetchTyreData] Error:', error.message);
        return {
            count: 0,
            tyres: [],
            tyreSize: tyreSize,
            brand: brand,
            error: error.message
        };
    }
}

module.exports = {
    fetchTyreData
};
