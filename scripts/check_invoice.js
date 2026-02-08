require('dotenv').config();
const sql = require('mssql');

const erpSqlConfig = {
    server: process.env.SQL_SERVER,
    database: process.env.SQL_DATABASE, // LasanthaTire
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

async function run() {
    try {
        await sql.connect(erpSqlConfig);
        const result = await sql.query`SELECT * FROM tblSalesInvoices WHERE InvoiceNo = 'NLT00006513'`;
        console.log(JSON.stringify(result.recordset[0], null, 2));
        await sql.close();
    } catch (err) {
        console.error(err);
    }
}
run();