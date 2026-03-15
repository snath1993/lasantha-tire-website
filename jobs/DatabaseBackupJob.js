const fs = require('fs');
const path = require('path');
const os = require('os');
const { exec } = require('child_process');
const moment = require('moment');
const { sendMedia, send, getClient } = require('../utils/waClientRegistry');

// Maximum safe file size to inject into Chromium RAM via Puppeteer (12MB)
const MAX_WA_MEDIA_BYTES = 12 * 1024 * 1024;

// Helper to get local network IP
function getLocalIp() {
    const nets = os.networkInterfaces();
    for (const name of Object.keys(nets)) {
        for (const net of nets[name]) {
            if (net.family === 'IPv4' && !net.internal) return net.address;
        }
    }
    return '127.0.0.1';
}

/**
 * Backup Database and Send to WhatsApp
 * @param {object} options 
 * @param {string[]} options.databases - List of database names to backup
 * @param {string} options.adminNumber - WhatsApp number to send to
 * @param {object} options.mainPool - SQL Connection pool
 */
module.exports = async function DatabaseBackupJob(options = {}) {
    const { mainPool, databases = ['LasanthaTire'], adminNumber } = options;
    
    if (!adminNumber) {
        console.error('[DatabaseBackupJob] No Admin Number provided.');
        return;
    }

    if (!mainPool || !mainPool.connected) {
        console.error('[DatabaseBackupJob] Database pool not connected.');
        return;
    }

    // FIX: Check WhatsApp client readiness before attempting backup+send
    const client = getClient() || (global && global.whatsappClient);
    if (!client || !client.isReady) {
        console.warn('[DatabaseBackupJob] ⚠️ WhatsApp client not ready — skipping backup send. Will retry next cycle.');
        return;
    }

    console.log(`[DatabaseBackupJob] Starting backup for: ${databases.join(', ')}`);

    for (const dbName of databases) {
        try {
            await processDatabase(dbName, mainPool, adminNumber);
        } catch (err) {
            console.error(`[DatabaseBackupJob] Failed to backup ${dbName}:`, err);
            try {
                // Notify admin of failure
                const { send } = require('../utils/waClientRegistry');
                await send(adminNumber, `❌ Backup failed for ${dbName}: ${err.message}`);
            } catch (e) {}
        }
    }
};

async function processDatabase(dbName, pool, adminNumber) {
    const timestamp = moment().format('YYYY-MM-DD_HH-mm-ss');
    const fileName = `${dbName}_${timestamp}.bak`;
    // Use a robust folder path. C:\ProgramData is usually accessible by Service Accounts if we set permissions.
    // Use the Network Share 'Sage' on the SQL Server machine (WIN-JIAVRTFMA0N)
    const remoteBackupDir = process.env.BACKUP_REMOTE_DIR || '\\\\WIN-JIAVRTFMA0N\\Sage\\SQLBackups';
    // Use Local Archive Directory on this machine
    const localArchiveDir = process.env.BACKUP_LOCAL_DIR || path.join(__dirname, '..', 'backups', 'sql_archives');
    
    // Ensure dirs exist
    if (!fs.existsSync(localArchiveDir)) {
        try { fs.mkdirSync(localArchiveDir, { recursive: true }); } catch (e) {}
    }
    // Remote dir might not be reachable — don't crash if network share is offline
    let remoteAvailable = false;
    try {
        if (!fs.existsSync(remoteBackupDir)) {
            fs.mkdirSync(remoteBackupDir, { recursive: true });
        }
        remoteAvailable = true;
    } catch (e) {
        console.warn(`[DatabaseBackupJob] Remote share not reachable: ${e.message}. Using local path only.`);
    }

    // FIX: Use local temp path for SQL BACKUP, then copy to remote if available
    const localFilePath = path.join(localArchiveDir, fileName);
    const remoteFilePath = remoteAvailable ? path.join(remoteBackupDir, fileName) : null;
    
    // Backup to local first (more reliable) — SQL Server needs to write here
    const sqlBackupTarget = remoteFilePath || localFilePath;
    console.log(`[DatabaseBackupJob] Backing up ${dbName} to: ${sqlBackupTarget}...`);

    // 1. Execute Backup Command
    try {
        const request = pool.request();
        await request.query(`BACKUP DATABASE [${dbName}] TO DISK = '${sqlBackupTarget}' WITH FORMAT, COMPRESSION, INIT`);
    } catch (err) {
        console.warn(`[DatabaseBackupJob] Compression backup failed, trying normal backup... (${err.message})`);
        try {
            const request = pool.request();
            await request.query(`BACKUP DATABASE [${dbName}] TO DISK = '${sqlBackupTarget}' WITH FORMAT, INIT`);
        } catch (err2) {
            console.error(`[DatabaseBackupJob] Normal backup also failed: ${err2.message}`);
            throw err2;
        }
    }

    // 2. Copy file between local/remote for redundancy
    if (remoteFilePath && sqlBackupTarget === remoteFilePath) {
        // Backup was made to remote — copy to local archive
        console.log(`[DatabaseBackupJob] Copying backup to local archive: ${localFilePath}...`);
        try {
            fs.copyFileSync(remoteFilePath, localFilePath);
        } catch (e) {
            console.warn(`[DatabaseBackupJob] Failed to copy to local archive: ${e.message}. Will send from remote.`);
        }
    } else if (remoteFilePath && sqlBackupTarget === localFilePath) {
        // Backup was made locally — try to copy to remote for redundancy
        try {
            fs.copyFileSync(localFilePath, remoteFilePath);
            console.log(`[DatabaseBackupJob] Copied to remote share for redundancy.`);
        } catch (e) {
            console.warn(`[DatabaseBackupJob] Remote copy skipped: ${e.message}`);
        }
    }

    // Determine which file to send (prefer local)
    const fileToSendPath = fs.existsSync(localFilePath) ? localFilePath 
                         : (remoteFilePath && fs.existsSync(remoteFilePath) ? remoteFilePath : null);
    
    if (!fileToSendPath) {
        throw new Error(`Backup file not found at local (${localFilePath}) or remote paths`);
    }

    // Now work with fileToSendPath
    const stats = fs.statSync(fileToSendPath);
    const sizeMB = (stats.size / (1024 * 1024)).toFixed(2);
    console.log(`[DatabaseBackupJob] Backup ready: ${fileName} (${sizeMB} MB)`);
    
    let fileToSend = fileToSendPath;
    let cleanupFile = false; // Do not cleanup the Archive (User Request: "Crash protection")

    if (stats.size > MAX_WA_MEDIA_BYTES) {
        const zipPath = fileToSendPath + '.zip';
        console.log(`[DatabaseBackupJob] File too large (${sizeMB} MB), zipping...`);
        
        await zipFile(fileToSendPath, zipPath);
        
        const zipStats = fs.statSync(zipPath);
        const zipSizeMB = (zipStats.size / (1024 * 1024)).toFixed(2);
        
        if (zipStats.size > MAX_WA_MEDIA_BYTES) {
            console.error(`[DatabaseBackupJob] Even zipped file is too large: ${zipSizeMB} MB. Generating Web Link...`);
            
            // Generate a network download link by moving to Dashboard's public folder
            const dashboardPublicDir = path.join(__dirname, '..', 'lasantha-tire-v2.0', 'public', 'backups');
            if (!fs.existsSync(dashboardPublicDir)) fs.mkdirSync(dashboardPublicDir, { recursive: true });
            
            const destNext = path.join(dashboardPublicDir, path.basename(zipPath));
            fs.copyFileSync(zipPath, destNext);

            const localIp = getLocalIp();
            const downloadUrl = `http://${localIp}:3029/backups/${path.basename(zipPath)}`;
            
            const msg = `✅ Backup Successful: ${dbName} (${timestamp})\n📦 Size: ${zipSizeMB} MB\n\n⚠️ File is too large to send safely over WhatsApp.\n\n📥 Download it securely from the shop network here:\n${downloadUrl}`;
            
            const { send } = require('../utils/waClientRegistry');
            await send(adminNumber, msg);
            
            // Delete the huge BAK since we have the zip, to save space. Leave the zip in sql_archives
            try { fs.unlinkSync(fileToSendPath); } catch(e){} 
            return;
        }

        // Send Zip
        console.log(`[DatabaseBackupJob] Sending ${path.basename(zipPath)} (${zipSizeMB} MB)...`);
        const fileData = fs.readFileSync(zipPath);
        const mediaResult = await sendMedia(adminNumber, 'application/zip', fileData, path.basename(zipPath));
        
        if (mediaResult.ok) {
            console.log(`[DatabaseBackupJob] Sent successfully.`);
            const { send } = require('../utils/waClientRegistry');
            await send(adminNumber, `✅ Backup Successful: ${dbName} (${timestamp})\nSize: ${zipSizeMB} MB`);
            // Cleanup ZIP only — keep BAK in archive.
            try { fs.unlinkSync(fileToSendPath); } catch(e){} // Delete BAK
            // We LEAVE the .zip file in the archive folder.
        } else {
            console.error(`[DatabaseBackupJob] Failed to send media: ${mediaResult.error}`);
            throw new Error(mediaResult.error);
        }

    } else {
        // Size is okay, send original BAK
        console.log(`[DatabaseBackupJob] Sending ${fileName}...`);
        const fileData = fs.readFileSync(fileToSendPath);
        const mediaResult = await sendMedia(adminNumber, 'application/octet-stream', fileData, fileName);
        
        if (mediaResult.ok) {
            console.log(`[DatabaseBackupJob] Sent successfully.`);
            // We LEAVE the .bak file in the archive folder.
        } else {
            console.error(`[DatabaseBackupJob] Failed to send media: ${mediaResult.error}`);
            throw new Error(mediaResult.error);
        }
    }
}

function zipFile(source, destination) {
    return new Promise((resolve, reject) => {
        // Use PowerShell to zip
        const cmd = `powershell -NoProfile -Command "Compress-Archive -Path '${source}' -DestinationPath '${destination}' -Force"`;
        exec(cmd, { windowsHide: true }, (error, stdout, stderr) => {
            if (error) {
                reject(error);
            } else {
                resolve();
            }
        });
    });
}
