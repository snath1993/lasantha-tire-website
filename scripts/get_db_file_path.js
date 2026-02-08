require('dotenv').config();
const { config } = require('../sqlConfig');
const sql = require('mssql');

(async () => {
    try {
        const pool = await sql.connect(config);
        const result = await pool.request().query("SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('LasanthaTire') AND type = 0");
        console.log(result.recordset[0].physical_name);
        process.exit(0);
    } catch (err) {
        console.error(err);
        process.exit(1);
    }
})();