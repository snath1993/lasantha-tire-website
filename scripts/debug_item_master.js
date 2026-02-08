const { sql, config } = require('../sqlConfig');
const mssql = require('mssql');

(async () => {
    try {
        const pool = await mssql.connect(config);
        const result = await pool.request().query("SELECT TOP 1 * FROM [View_Item Master Whatsapp] WHERE ItemID IS NOT NULL");
        console.log(result.recordset[0]);
        process.exit(0);
    } catch (err) {
        console.error(err);
        process.exit(1);
    }
})();
