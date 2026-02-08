const sql = require('mssql');
require('dotenv').config({ path: 'lasantha-tire-v1.0/.env.local' });

const config = {
    user: process.env.SQL_USER || 'sa',
    password: process.env.SQL_PASSWORD,
    server: process.env.SQL_SERVER || 'localhost',
    database: process.env.SQL_DATABASE || 'LasanthaTire',
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

console.log('Testing connection with:', {
    server: config.server,
    database: config.database,
    user: config.user
});

async function testConnection() {
    try {
        await sql.connect(config);
        console.log('✅ Connection successful!');
        
        const result = await sql.query`SELECT TOP 1 * FROM View_ItemWhse`;
        console.log('✅ Query successful! Found item:', result.recordset[0]);
        
        process.exit(0);
    } catch (err) {
        console.error('❌ Connection failed:', err);
        process.exit(1);
    }
}

testConnection();
