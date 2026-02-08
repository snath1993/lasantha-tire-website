const { getPool } = require('../utils/sqlPool');
const sql = require('mssql');

async function run() {
    try {
        const pool = await getPool();
        
        // Calculate date 3 months ago
        const date = new Date();
        date.setMonth(date.getMonth() - 3);
        const dateStr = date.toISOString().slice(0, 10);

        console.log(`Checking Sales Since: ${dateStr}\n`);

        const result = await pool.request().query(`
            SELECT DISTINCT im.Custom3 as Brand
            FROM [View_Sales report whatsapp] s
            JOIN [View_Item Master Whatsapp] im ON s.Expr3 = im.ItemID
            WHERE s.Expr1 >= DATEADD(month, -3, GETDATE())
            AND im.Custom3 IS NOT NULL 
            AND im.Custom3 <> ''
            ORDER BY im.Custom3
        `);

        console.log('--- ACTIVE BRANDS (LAST 3 MONTHS) ---');
        result.recordset.forEach((row, index) => {
            console.log(`${index + 1}. ${row.Brand}`);
        });
        console.log('-------------------------------------');
        console.log(`Total Active Brands: ${result.recordset.length}`);

    } catch (err) {
        console.error('Error:', err);
    } finally {
        process.exit();
    }
}

run();
