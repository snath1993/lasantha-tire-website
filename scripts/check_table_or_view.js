// Check if tblSalesInvoices is a table or view
const { sql, config: sqlConfig } = require('../sqlConfig');

async function checkTableType() {
    try {
        const pool = await sql.connect(sqlConfig);
        
        // Check if it's a view
        const viewCheck = await pool.request().query(`
            SELECT 
                OBJECT_NAME(object_id) as Name,
                type_desc as Type
            FROM sys.objects
            WHERE OBJECT_NAME(object_id) = 'tblSalesInvoices'
        `);
        
        console.log('Object Info:', viewCheck.recordset);
        
        // Check for triggers
        const triggerCheck = await pool.request().query(`
            SELECT 
                t.name as TriggerName,
                OBJECT_DEFINITION(t.object_id) as Definition
            FROM sys.triggers t
            WHERE parent_id = OBJECT_ID('dbo.tblSalesInvoices')
        `);
        
        if (triggerCheck.recordset.length > 0) {
            console.log('\n=== TRIGGERS ON tblSalesInvoices ===');
            triggerCheck.recordset.forEach(t => {
                console.log(`\nTrigger: ${t.TriggerName}`);
                console.log(t.Definition);
            });
        } else {
            console.log('\nNo triggers found on tblSalesInvoices');
        }
        
        // Check the actual columns
        const columnCheck = await pool.request().query(`
            SELECT COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = 'tblSalesInvoices'
            AND TABLE_SCHEMA = 'dbo'
            ORDER BY ORDINAL_POSITION
        `);
        
        console.log('\n=== COLUMNS IN tblSalesInvoices ===');
        columnCheck.recordset.forEach(c => {
            console.log(`${c.COLUMN_NAME}: ${c.DATA_TYPE}`);
        });
        
        await pool.close();
    } catch (e) {
        console.error('Error:', e.message);
        console.error('Stack:', e.stack);
    }
}

checkTableType();
