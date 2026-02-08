const { sql, config } = require('../sqlConfig');
const mssql = require('mssql');

(async () => {
    try {
        const pool = await mssql.connect(config);
        
        // 1. Check unique Categories
        console.log('\n--- UNIQUE CATEGORIES ---');
        const r1 = await pool.request().query("SELECT DISTINCT Categoty FROM [View_Sales report whatsapp] ORDER BY Categoty");
        r1.recordset.forEach(r => console.log(r.Categoty));
        
        // 2. Search for 'Katta' or 'Rebuild' or 'Retread' or 'Dag' in Item Names of TYRE category
        console.log('\n--- POTENTIAL "KATTA" ITEMS ---');
        const r2 = await pool.request().query(`
            SELECT TOP 20 Expr4 as ItemName, Categoty 
            FROM [View_Sales report whatsapp] 
            WHERE (Expr4 LIKE '%Katta%' OR Expr4 LIKE '%Retread%' OR Expr4 LIKE '%Rebuild%' OR Expr4 LIKE '%Dag%')
        `);
        r2.recordset.forEach(r => console.log(`${r.ItemName} (${r.Categoty})`));

        process.exit(0);
    } catch (err) {
        console.error(err);
        process.exit(1);
    }
})();
