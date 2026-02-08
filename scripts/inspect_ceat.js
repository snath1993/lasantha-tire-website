const { getPool } = require('../utils/sqlPool');
const sql = require('mssql');

async function run() {
    try {
        const pool = await getPool();
        
        const result = await pool.request().query(`
            SELECT TOP 50 ItemDescription, Categoty, Custom3 as Brand
            FROM [View_Item Master Whatsapp]
            WHERE Custom3 = 'CEAT'
        `);

        console.log('\n--- SAMPLE CEAT ITEMS ---');
        result.recordset.forEach(row => {
            console.log(`[${row.Brand}] ${row.ItemDescription} -> ${row.Categoty}`);
        });
        console.log('-------------------------\n');

    } catch (err) {
        console.error('Error:', err);
    } finally {
        process.exit();
    }
}

run();
