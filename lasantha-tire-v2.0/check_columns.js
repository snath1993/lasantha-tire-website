const sql = require('mssql');

async function run() {
  try {
    const config = {
      user: process.env.DB_USER || 'sa',
      password: process.env.DB_PASSWORD || 'Lasantha@123',
      server: process.env.DB_SERVER || 'localhost',
      database: process.env.DB_NAME || 'Lasantha_Tire_Main_DB',
      options: {
        encrypt: false,
        trustServerCertificate: true
      }
    };

    await sql.connect(config);
    
    console.log('Checking columns for [View_Sales report whatsapp]...');
    const viewCols = await sql.query(`
      SELECT COLUMN_NAME 
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME = 'View_Sales report whatsapp'
    `);
    console.log(viewCols.recordset.map(r => r.COLUMN_NAME));

    console.log('Checking columns for [SalesInvoice]...');
    const tableCols = await sql.query(`
      SELECT COLUMN_NAME 
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME = 'SalesInvoice'
    `);
    console.log(tableCols.recordset.map(r => r.COLUMN_NAME));

    await sql.close();
  } catch (err) {
    console.error(err);
  }
}

run();
