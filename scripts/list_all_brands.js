const { getPool } = require('../utils/sqlPool');
const sql = require('mssql');

async function run() {
    try {
        const pool = await getPool();
        console.log('Connected to DB. Fetching All Brands...');
        
        const result = await pool.request().query(`
            SELECT DISTINCT Custom3 as Brand 
            FROM [View_Item Master Whatsapp] 
            WHERE Custom3 IS NOT NULL AND Custom3 <> ''
            ORDER BY Custom3
        `);

        console.log('\n--- ALL AVAILABLE BRANDS ---');
        result.recordset.forEach(row => {
            console.log(row.Brand);
        });
        console.log(`\nTotal Brands: ${result.recordset.length}`);
        console.log('----------------------------\n');

    } catch (err) {
        console.error('Error:', err);
    } finally {
        process.exit();
    }
}

run();
