const sql = require('mssql');
const { config } = require('../sqlConfig');

async function run() {
    try {
        const pool = await sql.connect(config);
        console.log('Connected to DB');
        
        // Try querying the table directly to see if it errors
        try {
            const result = await pool.request().query(`
                SELECT COLUMN_NAME 
                FROM [WhatsAppAI].INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'ReOrderHistory'
            `);
            console.log('Columns in [WhatsAppAI] ReOrderHistory:');
            if (result.recordset.length === 0) {
                console.log('(No columns found - table might not exist)');
            } else {
                result.recordset.forEach(row => console.log(row.COLUMN_NAME));
            }
        } catch (e) {
            console.log('Error querying schema:', e.message);
        }

        pool.close();
    } catch (err) {
        console.error('Error:', err);
    }
}

run();
