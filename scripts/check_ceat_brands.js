const { getPool } = require('../utils/sqlPool');
async function run() {
    try {
        const pool = await getPool();
        const result = await pool.request().query(`
            SELECT DISTINCT Custom3 
            FROM [View_Item Master Whatsapp]
            WHERE Custom3 LIKE '%CEAT%'
        `);
        console.log(result.recordset);
    } finally { process.exit(); }
}
run();
