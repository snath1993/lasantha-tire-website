const { getPool } = require('../utils/sqlPool');
async function run() {
    try {
        const pool = await getPool();
        const result = await pool.request().query(`
            SELECT TOP 10 ItemDescription, Categoty, Custom3 
            FROM [View_Item Master Whatsapp] 
            WHERE Custom3 = 'MAXXIES'
        `);
        console.log('--- MAXXIES ITEMS ---');
        result.recordset.forEach(r => console.log(`${r.ItemDescription} -> ${r.Categoty}`));
    } catch (e) { console.error(e); } finally { process.exit(); }
}
run();
