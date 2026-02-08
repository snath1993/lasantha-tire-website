const sql = require('mssql');
require('dotenv').config();

const config = {
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    server: process.env.SQL_SERVER.split('\\')[0], // Handle instance name if needed, but usually server is enough or server\instance
    database: 'WhatsAppAI', // Explicitly target the AI database where quotations are stored
    options: {
        encrypt: process.env.DB_ENCRYPT === 'true',
        trustServerCertificate: process.env.DB_TRUST_CERT === 'true',
        instanceName: process.env.SQL_SERVER.includes('\\') ? process.env.SQL_SERVER.split('\\')[1] : undefined
    }
};

async function getLatestQuotation() {
    try {
        await sql.connect(config);
        // Get the last 5 quotations to give a few options
        const result = await sql.query`SELECT TOP 5 RefCode, QuotationNumber, CustomerName, CreatedAt, Items FROM Quotations ORDER BY Id DESC`;
        
        if (result.recordset.length > 0) {
            console.log('✅ Found Real Quotations in WhatsAppAI DB:');
            result.recordset.forEach(q => {
                console.log('------------------------------------------------');
                console.log(`RefCode: ${q.RefCode}`);
                console.log(`Customer: ${q.CustomerName}`);
                console.log(`Date: ${q.CreatedAt}`);
                console.log(`Items: ${q.Items ? q.Items.substring(0, 50) + '...' : 'N/A'}`);
                console.log(`🔗 Link: http://localhost:3099/?ref=${q.RefCode}`);
            });
        } else {
            console.log('❌ No quotations found in WhatsAppAI database.');
        }
    } catch (err) {
        console.error('❌ Database Error:', err);
    } finally {
        await sql.close();
    }
}

getLatestQuotation();