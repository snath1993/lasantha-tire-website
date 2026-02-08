const { getPool } = require('../utils/sqlPool');
const sql = require('mssql');

async function run() {
    try {
        const pool = await getPool();
        const result = await pool.request().query(`
            SELECT TOP 20 ItemDescription, Categoty 
            FROM [View_Item Master Whatsapp] 
            WHERE ItemDescription LIKE '%MOTO%' 
               OR ItemDescription LIKE '%SCOOTER%'
               OR Categoty LIKE '%MOTO%'
        `);
        console.log('--- ITEMS MATCHING MOTO/SCOOTER ---');
        result.recordset.forEach(r => console.log(`${r.ItemDescription} -> ${r.Categoty}`));
    } catch (e) {
        console.error(e);
    } finally {
        process.exit();
    }
}
run();
