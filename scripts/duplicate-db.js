require('dotenv').config();
const sql = require('mssql');
const path = require('path');

async function run() {
    try {
        const config = {
            user: process.env.SQL_USER || process.env.DB_USER,
            password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
            server: process.env.SQL_SERVER || process.env.DB_SERVER,
            database: 'master', // Connect to master to perform backup/restore
            options: {
                encrypt: false,
                trustServerCertificate: true
            }
        };

        console.log('Connecting to SQL Server (master)...');
        const pool = await sql.connect(config);
        console.log('Connected.');

        const sourceDb = 'LasanthaTire';
        const targetDb = 'Test';
        const backupFile = `C:\\Program Files\\Microsoft SQL Server\\MSSQL12.SQLEXPRESS\\MSSQL\\Backup\\${sourceDb}_copy.bak`;

        // 1. Get Logical File Names and Physical Paths
        console.log(`Getting file details for ${sourceDb}...`);
        const fileResult = await pool.request().query(`
            SELECT name, physical_name, type_desc 
            FROM sys.master_files 
            WHERE database_id = DB_ID('${sourceDb}')
        `);

        if (fileResult.recordset.length === 0) {
            throw new Error(`Database ${sourceDb} not found.`);
        }

        const dataFile = fileResult.recordset.find(f => f.type_desc === 'ROWS');
        const logFile = fileResult.recordset.find(f => f.type_desc === 'LOG');

        console.log('Data File:', dataFile.name, '->', dataFile.physical_name);
        console.log('Log File:', logFile.name, '->', logFile.physical_name);

        // Construct new paths
        const newDataPath = dataFile.physical_name.replace(`${sourceDb}.mdf`, `${targetDb}.mdf`).replace(`${sourceDb}.MDF`, `${targetDb}.MDF`);
        const newLogPath = logFile.physical_name.replace(`${sourceDb}_log.ldf`, `${targetDb}_log.ldf`).replace(`${sourceDb}_log.LDF`, `${targetDb}_log.LDF`);

        // If paths are identical (e.g. if source didn't have db name in filename), append _Test
        const finalDataPath = newDataPath === dataFile.physical_name ? newDataPath.replace('.mdf', '_Test.mdf') : newDataPath;
        const finalLogPath = newLogPath === logFile.physical_name ? newLogPath.replace('.ldf', '_Test_log.ldf') : newLogPath;

        console.log('Target Data Path:', finalDataPath);
        console.log('Target Log Path:', finalLogPath);

        // 2. Backup Source Database
        console.log(`Backing up ${sourceDb} to ${backupFile}...`);
        await pool.request().query(`
            BACKUP DATABASE [${sourceDb}] 
            TO DISK = '${backupFile}' 
            WITH COPY_ONLY, FORMAT, INIT, STATS = 10
        `);
        console.log('Backup completed.');

        // 3. Restore as New Database
        console.log(`Restoring as ${targetDb}...`);
        // Check if target exists and drop if necessary (Optional, maybe dangerous? Let's fail if exists for safety)
        const checkTarget = await pool.request().query(`SELECT dbid FROM sys.sysdatabases WHERE name = '${targetDb}'`);
        if (checkTarget.recordset.length > 0) {
            console.log(`Target database ${targetDb} already exists. Dropping it...`);
             await pool.request().query(`
                ALTER DATABASE [${targetDb}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [${targetDb}];
            `);
        }

        const restoreQuery = `
            RESTORE DATABASE [${targetDb}] 
            FROM DISK = '${backupFile}' 
            WITH 
            MOVE '${dataFile.name}' TO '${finalDataPath}',
            MOVE '${logFile.name}' TO '${finalLogPath}',
            STATS = 10
        `;
        
        await pool.request().query(restoreQuery);
        console.log(`Database duplicated successfully! New DB: ${targetDb}`);

        process.exit(0);

    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
}

run();
