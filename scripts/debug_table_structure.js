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

        const query = `
          SELECT TOP 1
            InvoiceNo, 
            Description, 
            Qty, 
            UnitPrice, 
            InvoiceDate
          FROM dbo.tblSalesInvoices
        `;
        
        console.log('Running query:', query);
        const res = await sql.query(query);
        console.log('Success:', res.recordset);

    } catch (err) {
        console.error('QUERY FAILED:', err.message);
    } finally {
        await sql.close();
    }
}

checkTable();
