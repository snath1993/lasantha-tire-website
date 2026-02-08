const sql = require('mssql');
const { config } = require('../sqlConfig');

async function run() {
    try {
        const pool = await sql.connect(config);
        console.log('Connected to DB');
        
        // Query columns of ReOrderHistory
        const result = await pool.request().query(`
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'ReOrderHistory' AND TABLE_CATALOG = 'WhatsAppAI'
        `);
        
        console.log('Columns in ReOrderHistory:');
        result.recordset.forEach(row => console.log(row.COLUMN_NAME));
        
        pool.close();
    } catch (err) {
        console.error('Error:', err);
    }
}

run();
