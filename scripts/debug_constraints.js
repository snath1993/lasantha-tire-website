const sql = require('mssql');
require('dotenv').config();

async function checkTable() {
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

    try {
        await sql.connect(config);
        console.log('Connected to SQL');

        console.log('--- CHECKING CONSTRAINTS ---');
        try {
            const res = await sql.query("sp_helpconstraint 'dbo.tblSalesInvoices'");
            // sp_helpconstraint returns multiple resultsets usually, or formatted text. 
            // In tedious/node-mssql it might be recordsets.
            if(res.recordsets) {
                res.recordsets.forEach((rs, i) => {
                    console.log(`--- Resultset ${i} ---`);
                    console.table(rs);
                });
            }
        } catch (e) {
            console.log('sp_helpconstraint failed:', e.message);
        }

    } catch (err) {
        console.error('Error:', err);
    } finally {
        await sql.close();
    }
}

checkTable();
