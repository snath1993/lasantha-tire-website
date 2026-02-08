require('dotenv').config();
const { config } = require('../sqlConfig');
const sql = require('mssql');
const DatabaseBackupJob = require('../jobs/DatabaseBackupJob');

(async () => {
    try {
        const pool = await sql.connect(config);
        const adminNum = process.env.ADMIN_NUMBER || '0777311770';
        if (!adminNum) {
            console.error('ADMIN_NUMBER not set in .env');
            process.exit(1);
        }
        console.log('Running Backup Job for Admin:', adminNum);
        
        // Mock sendMedia if running disconnected from real WA client (for testing logic)
        // But here we want to test real sending if possible.
        // If the bot is running, we can't easily hook into its client instance from a separate process.
        // Setup: This script won't work perfectly for *sending* if the client exists only in the main process.
        // However, DatabaseBackupJob uses require('../utils/waClientRegistry').
        
        // Since waClientRegistry holds the client in a global/module var, a separate process won't see the authenticated client.
        // WE CANNOT RUN THIS SEPARATELY TO SEND MESSAGE.
        
        console.log('NOTE: This script creates the backup but cannot send WhatsApp message because the WhatsApp Client is in the main Bot process.');
        console.log('To test fully, please wait for the schedule or trigger via IPC if available.');
        console.log('Per user request, I will just proceed to show it works up to backup creation.');

        // For this test script, we will just create the backup and skip sending or mock sending.
        // Actually, let's just create the backup to verify SQL permissions.
        
        // We override sendMedia to just log
        const waRegistry = require('../utils/waClientRegistry');
        waRegistry.sendMedia = async (num, mime, buf, name) => {
            console.log(`[MOCK SEND] Sending ${name} (${buf.length} bytes) to ${num}`);
            return { ok: true };
        };

        // Also override send
        waRegistry.send = async (num, msg) => {
             console.log(`[MOCK SEND] Msg to ${num}: ${msg}`);
             return { ok: true };
        };

        await DatabaseBackupJob({ 
            mainPool: pool, 
            databases: ['LasanthaTire', 'LasanthaTest'], 
            adminNumber: adminNum 
        });

        process.exit(0);
    } catch (err) {
        console.error(err);
        process.exit(1);
    }
})();