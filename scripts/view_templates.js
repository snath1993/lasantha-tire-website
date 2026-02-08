require('dotenv').config();
const sql = require('mssql');

const config = {
    server: process.env.SQL_SERVER,
    database: 'WhatsAppAI',
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

async function run() {
    try {
        await sql.connect(config);
        const result = await sql.query`SELECT * FROM TblInvoiceTemplates`;
        console.log(JSON.stringify(result.recordset, null, 2));
        await sql.close();
    } catch (err) {
        console.error(err);
    }
}
run();