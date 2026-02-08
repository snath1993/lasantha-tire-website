const { getPool } = require('../utils/sqlPool');

async function run() {
    try {
        const pool = await getPool();
        const result = await pool.request().query(`
            SELECT TOP 1 * FROM [View_Item Master Whatsapp] WHERE Custom3 = 'CEAT'
        `);
        console.log(result.recordset[0]);
    } finally {
        process.exit();
    }
}
run();
