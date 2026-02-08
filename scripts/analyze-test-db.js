require('dotenv').config();
const sql = require('mssql');

async function run() {
    try {
        const config = {
            user: process.env.SQL_USER || process.env.DB_USER,
            password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
            server: process.env.SQL_SERVER || process.env.DB_SERVER,
            database: 'Test', // Explicitly target the Test database
            options: {
                encrypt: false,
                trustServerCertificate: true
            }
        };

        console.log('Connecting to Test Database...');
        const pool = await sql.connect(config);
        console.log('Connected.');

        // 1. Get Tables and Row Counts
        console.log('\n--- TABLES & ROW COUNTS ---');
        const tablesResult = await pool.request().query(`
            SELECT 
                t.name AS TableName,
                p.rows AS RowCount_
            FROM sys.tables t
            INNER JOIN sys.indexes i ON t.object_id = i.object_id
            INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
            WHERE t.is_ms_shipped = 0 AND i.index_id < 2
            ORDER BY p.rows DESC
        `);
        
        const tables = tablesResult.recordset;
        console.table(tables);

        // 2. Get Views
        console.log('\n--- VIEWS ---');
        const viewsResult = await pool.request().query(`
            SELECT name AS ViewName FROM sys.views ORDER BY name
        `);
        console.table(viewsResult.recordset);

        // 3. Analyze Columns of Top 5 Largest Tables to understand structure
        console.log('\n--- STRUCTURE OF TOP 5 TABLES ---');
        const topTables = tables.slice(0, 5);
        
        for (const table of topTables) {
            const colResult = await pool.request().query(`
                SELECT COLUMN_NAME, DATA_TYPE 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = '${table.TableName}'
            `);
            console.log(`\nTable: ${table.TableName}`);
            console.log(colResult.recordset.map(c => `${c.COLUMN_NAME} (${c.DATA_TYPE})`).join(', '));
        }

        process.exit(0);

    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
}

run();
