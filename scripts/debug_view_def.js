const sql = require('mssql');
require('dotenv').config();

async function checkView() {
    const config = {
        user: process.env.SQL_USER,
        password: process.env.SQL_PASSWORD,
        server: process.env.SQL_SERVER || 'WIN-JIAVRTFMA0N\\SQLEXPRESS',
        database: process.env.SQL_DATABASE || 'LasanthaTire',
        options: {
            encrypt: false,
            trustServerCertificate: true
        }
    };

    console.log('Config:', { ...config, password: '***' });

    try {
        await sql.connect(config);
        console.log('Connected to SQL');

        const result = await sql.query("sp_helptext '[View_Sales report whatsapp]'");
        console.log('--- VIEW DEFINITION ---');
        result.recordset.forEach(row => {
            console.log(Object.values(row)[0]);
        });

    } catch (err) {
        console.error('Error:', err);
    } finally {
        await sql.close();
    }
}

checkView();
