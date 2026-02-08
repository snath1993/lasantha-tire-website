const { sql, config } = require('../sqlConfig');

async function queryViewStructure() {
    try {
        console.log('Connecting to database...');
        const pool = await sql.connect(config);
        
        console.log('\n=== [View_Sales report whatsapp] Column Structure ===\n');
        
        // Get column information
        const columnsResult = await pool.request().query(`
            SELECT 
                COLUMN_NAME,
                DATA_TYPE,
                CHARACTER_MAXIMUM_LENGTH,
                IS_NULLABLE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = 'View_Sales report whatsapp'
            ORDER BY ORDINAL_POSITION
        `);
        
        console.log('Column Name'.padEnd(30) + 'Data Type'.padEnd(20) + 'Max Length'.padEnd(15) + 'Nullable');
        console.log('-'.repeat(85));
        
        columnsResult.recordset.forEach(col => {
            console.log(
                col.COLUMN_NAME.padEnd(30) + 
                col.DATA_TYPE.padEnd(20) + 
                (col.CHARACTER_MAXIMUM_LENGTH ? String(col.CHARACTER_MAXIMUM_LENGTH).padEnd(15) : 'N/A'.padEnd(15)) + 
                col.IS_NULLABLE
            );
        });
        
        console.log('\n=== Sample Data (Top 3 Rows) ===\n');
        
        // Get sample data
        const sampleResult = await pool.request().query(`
            SELECT TOP 3 * 
            FROM [View_Sales report whatsapp]
        `);
        
        if (sampleResult.recordset.length > 0) {
            console.log(JSON.stringify(sampleResult.recordset, null, 2));
        } else {
            console.log('No data found in view.');
        }
        
        // Get row count
        const countResult = await pool.request().query(`
            SELECT COUNT(*) AS TotalRows
            FROM [View_Sales report whatsapp]
        `);
        
        console.log(`\n=== Total Rows: ${countResult.recordset[0].TotalRows} ===\n`);
        
        await pool.close();
        console.log('Query completed successfully.');
        
    } catch (error) {
        console.error('Error:', error.message);
        console.error(error);
    }
}

queryViewStructure();
