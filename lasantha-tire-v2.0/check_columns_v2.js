const sql = require('mssql');

require('dotenv').config();

async function run() {
  try {
    const config = {
      user: process.env.SQL_USER || process.env.DB_USER || 'sa',
      password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
      server: process.env.SQL_SERVER || process.env.DB_SERVER || 'localhost',
      database: process.env.SQL_DATABASE || process.env.DB_NAME || 'LasanthaTire',
      options: {
        encrypt: false,
        trustServerCertificate: true
      }
    };

    if (!config.password) {
      console.error('Missing SQL_PASSWORD/DB_PASSWORD in environment');
      process.exit(1);
    }

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
