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
    await sql.connect(sqlConfig);
    
    // Fetch IDs for specific alignment items
    const alignments = await sql.query`
      SELECT ItemID, ItemDescription, Categoty, UnitCost, Custom3 
      FROM [View_Item Master Whatsapp] 
      WHERE Categoty = 'WHEEL ALIGNMENT' OR ItemDescription LIKE '%ALIGNMENT%'
    `;
    console.log('--- Alignment Items ---');
    console.table(alignments.recordset);

    // Fetch IDs for Balancing and Valves (Neck)
    const others = await sql.query`
      SELECT ItemID, ItemDescription, Categoty, UnitCost 
      FROM [View_Item Master Whatsapp] 
      WHERE ItemDescription LIKE '%BALANCING%' OR ItemDescription LIKE '%VALVE%' OR ItemDescription LIKE '%NECK%'
    `;
    console.log('--- Other Common Services ---');
    console.table(others.recordset);

  } catch (err) {
    console.error('Error:', err);
  } finally {
    await sql.close();
  }
}

run();
