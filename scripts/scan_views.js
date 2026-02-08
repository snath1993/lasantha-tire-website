const { sql, config } = require('../sqlConfig');

async function scanViews() {
    try {
        await sql.connect(config);
        console.log("Connected to database. Scanning views...");

        const result = await sql.query(`
            SELECT 
                v.TABLE_NAME as ViewName,
                c.COLUMN_NAME as ColumnName,
                c.DATA_TYPE as DataType
            FROM 
                INFORMATION_SCHEMA.VIEWS v
            JOIN 
                INFORMATION_SCHEMA.COLUMNS c ON v.TABLE_NAME = c.TABLE_NAME
            ORDER BY 
                v.TABLE_NAME, c.ORDINAL_POSITION
        `);

        if (result.recordset.length === 0) {
            console.log("No views found or permission denied.");
            return;
        }

        const views = {};

        result.recordset.forEach(row => {
            if (!views[row.ViewName]) {
                views[row.ViewName] = [];
            }
            views[row.ViewName].push(`${row.ColumnName} (${row.DataType})`);
        });

        console.log("\n--- VIEW SCHEMA DEFINITIONS ---\n");
        for (const [viewName, columns] of Object.entries(views)) {
            console.log(`- **[${viewName}]**`);
            console.log(`    - Columns: ${columns.join(', ')}.`);
        }
        console.log("\n-------------------------------\n");

    } catch (err) {
        console.error("Error scanning views:", err);
    } finally {
        await sql.close();
    }
}

scanViews();
