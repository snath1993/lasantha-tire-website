const sql = require('mssql');

require('dotenv').config();

const sqlConfig = {
  user: process.env.SQL_USER || process.env.DB_USER || 'sa',
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
  database: process.env.SQL_DATABASE || process.env.DB_NAME || 'LasanthaTire',
  server: process.env.SQL_SERVER || process.env.DB_SERVER || 'localhost',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

if (!sqlConfig.password) {
  console.error('Missing SQL_PASSWORD/DB_PASSWORD in environment');
  process.exit(1);
}

async function run() {
  try {
    console.log('Connecting to:', sqlConfig.server);
    await sql.connect(sqlConfig);
    console.log('Connected!');
    
    // Get distinct categories
    const result = await sql.query`SELECT DISTINCT Categoty FROM [View_Item Master Whatsapp] ORDER BY Categoty`;
    console.log('Categories:', result.recordset);
    
    // Check QTY for services
    const qtyCheck = await sql.query`SELECT TOP 5 ItemDescription, Categoty, iw.QTY FROM [View_Item Master Whatsapp] im JOIN [View_Item Whse Whatsapp] iw ON im.ItemID = iw.ItemID WHERE Categoty = 'WHEEL ALIGNMENT'`;
    console.log('Service QTY:', qtyCheck.recordset);

  } catch (err) {
    console.error('Error:', err);
  } finally {
    await sql.close();
  }
}

run();
