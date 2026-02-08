const { config } = require('../sqlConfig');
const mssql = require('mssql');

(async () => {
    try {
        const pool = await mssql.connect(config);
        
        const views = [
            'View_Item Master Whatsapp',
            'View_Item Whse Whatsapp',
            'View_Customer Master Whatsapp',
            'View_Sales report whatsapp' // Re-confirming
        ];

        for (const view of views) {
            console.log(`\n--- Structure for ${view} ---`);
            try {
                // Get one row to infer structure (easiest way without querying sys tables complexly)
                const result = await pool.request().query(`SELECT TOP 1 * FROM [${view}]`);
                if (result.recordset.length > 0) {
                    console.log(Object.keys(result.recordset[0]));
                } else {
                    // If empty, query sys.columns
                    const colResult = await pool.request().query(`
                        SELECT name FROM sys.columns 
                        WHERE object_id = OBJECT_ID('${view}')
                    `);
                    console.log(colResult.recordset.map(r => r.name));
                }
            } catch (e) {
                console.log(`Error reading ${view}: ${e.message}`);
            }
        }
        process.exit(0);
    } catch (err) {
        console.error(err);
        process.exit(1);
    }
})();