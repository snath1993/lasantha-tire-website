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
    console.log('Connecting to:', sqlConfig.server, 'Instance:', sqlConfig.options.instanceName);
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
