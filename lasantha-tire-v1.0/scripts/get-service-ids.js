const sql = require('mssql');

const sqlConfig = {
  user: 'sa',
  password: 'Admin1234',
  database: 'LasanthaTire',
  server: 'WIN-JIAVRTFMA0N', 
  options: {
    encrypt: false,
    trustServerCertificate: true,
    instanceName: 'SQLEXPRESS'
  }
};

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
