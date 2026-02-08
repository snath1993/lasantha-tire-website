
const sql = require('mssql');
require('dotenv').config();

const config = {
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    server: process.env.SQL_SERVER,
    database: process.env.SQL_DATABASE,
    options: {
        encrypt: false,
        trustServerCertificate: true,
        instancename: 'SQLEXPRESS'
    }
};

async function checkVehicleFormat() {
    try {
        await sql.connect(config);
        console.log('Connected to DB');
        
        const result = await sql.query`
            sp_helptext 'View_Sales report whatsapp'
        `;
        
        console.log('View Definition:');
        result.recordset.forEach(row => process.stdout.write(row.Text));
        
        console.log('Sample Data from View:');
        console.table(result.recordset);
        
    } catch (err) {
        console.error('Error:', err);
    } finally {
        await sql.close();
    }
}

checkVehicleFormat();
