const sql = require('mssql');

const sqlConfig = {
  user: process.env.SQL_USER || 'sa',
  password: process.env.SQL_PASSWORD || '123456', // Trying common default or empty
  database: process.env.SQL_DATABASE || 'LasanthaTire',
  server: process.env.SQL_SERVER || 'localhost',
  port: parseInt(process.env.SQL_PORT || '1433', 10),
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function run() {
  try {
    await sql.connect(sqlConfig);
    const result = await sql.query`SELECT DISTINCT Categoty FROM [View_Item Master Whatsapp] ORDER BY Categoty`;
    console.log('Categories:', result.recordset);
  } catch (err) {
    console.error('Error:', err);
  } finally {
    await sql.close();
  }
}

run();
