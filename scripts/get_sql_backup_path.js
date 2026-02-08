const sql = require('mssql');
const { config } = require('../sqlConfig');

(async () => {
    try {
        await sql.connect(config);
        const result = await sql.query("SELECT SERVERPROPERTY('InstanceDefaultBackupPath') AS BackupPath");
        let path = result.recordset[0].BackupPath;
        if (!path) {
            // Fallback for older versions or if not configured
             const res2 = await sql.query("EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'BackupDirectory'");
             path = res2.recordset[0].Data;
        }
        console.log('Default Backup Path:', path);
        process.exit(0);
    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
})();