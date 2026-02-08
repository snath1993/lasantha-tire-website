const sql = require('mssql');
const { config } = require('../sqlConfig');

(async () => {
    try {
        await sql.connect(config);
        const result = await sql.query`
            SELECT 
                d.name AS DatabaseName, 
                SUM(m.size) * 8 / 1024 AS SizeMB
            FROM sys.master_files m
            JOIN sys.databases d ON d.database_id = m.database_id
            GROUP BY d.name
            ORDER BY SizeMB DESC
        `;
        console.table(result.recordset);
        process.exit(0);
    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
})();