const sql = require('mssql');
require('dotenv').config({ path: '.env.local' }); // Load env from the nextjs app

const sqlConfig = {
  user: process.env.DB_USER,
  password: process.env.DB_PASSWORD,
  database: 'WhatsAppAI',
  server: process.env.DB_SERVER,
  port: parseInt(process.env.DB_PORT || '1433', 10),
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function testSave() {
  try {
    console.log('Connecting to database...');
    const pool = await sql.connect(sqlConfig);
    console.log('Connected.');

    const prefix = 'QT-V-';
    
    console.log('Checking for existing records...');
    const result = await pool.request()
        .input('prefix', sql.VarChar, prefix + '%')
        .query(`
          SELECT TOP 1 quotation_no 
          FROM pdfquote 
          WHERE quotation_no LIKE @prefix 
          ORDER BY id DESC
        `);
    
    console.log('Last record:', result.recordset[0]);

    let nextNum = 1;
    if (result.recordset.length > 0) {
        const lastNo = result.recordset[0].quotation_no;
        const parts = lastNo.split('-');
        const lastNumStr = parts[parts.length - 1];
        const parsed = parseInt(lastNumStr, 10);
        if (!isNaN(parsed)) {
          nextNum = parsed + 1;
        }
    }
    const quotationNo = `${prefix}${nextNum.toString().padStart(4, '0')}`;
    console.log('Generated Quotation No:', quotationNo);

    console.log('Attempting insert...');
    await pool.request()
        .input('quotation_no', sql.VarChar, quotationNo)
        .input('customer_name', sql.NVarChar, 'Test Customer')
        .input('vehicle_no', sql.NVarChar, 'TEST-123')
        .input('date', sql.Date, new Date())
        .input('terms', sql.NVarChar, 'Test Terms')
        .input('total_amount', sql.Decimal(18, 2), 100.00)
        .input('items', sql.NVarChar(sql.MAX), JSON.stringify([{id: 1, name: 'Test Item'}]))
        .input('type', sql.VarChar, 'VAT')
        .query(`
          INSERT INTO pdfquote (quotation_no, customer_name, vehicle_no, date, terms, total_amount, items, type)
          VALUES (@quotation_no, @customer_name, @vehicle_no, @date, @terms, @total_amount, @items, @type)
        `);
    
    console.log('Insert successful.');
    
    await pool.close();
  } catch (err) {
    console.error('Error:', err);
  }
}

testSave();
