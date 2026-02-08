const { getPool } = require('../utils/sqlPool');
const sql = require('mssql');

async function run() {
    try {
        const pool = await getPool();
        console.log('Connected to DB. Fetching Categories...');
        
        const result = await pool.request().query(`
            SELECT DISTINCT Categoty
            FROM [View_Item Master Whatsapp]
            ORDER BY Categoty
        `);

        console.log('\n--- BRAND CATEGORIES FOUND ---');
        result.recordset.forEach(row => {
            console.log(row.Categoty);
        });
        console.log('------------------------------\n');

    } catch (err) {
        console.error('Error:', err);
    } finally {
        process.exit();
    }
}

run();
