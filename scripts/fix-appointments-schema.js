require('dotenv').config();
const sql = require('mssql');

async function run() {
    try {
        console.log('Connecting to DB...');
        const config = {
            user: process.env.SQL_USER || process.env.DB_USER,
            password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
            server: process.env.SQL_SERVER || process.env.DB_SERVER,
            database: 'WhatsAppAI', // Explicitly target WhatsAppAI
            options: {
                encrypt: false,
                trustServerCertificate: true
            }
        };

        await sql.connect(config);
        console.log('Connected.');

        console.log('Altering Appointments table...');
        await sql.query(`ALTER TABLE Appointments ALTER COLUMN ServiceType NVARCHAR(500)`);
        console.log('Column ServiceType altered to NVARCHAR(500).');

        process.exit(0);
    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
}

run();
