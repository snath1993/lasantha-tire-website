const fs = require('fs');
const path = require('path');
const os = require('os');
const { exec } = require('child_process');
const moment = require('moment');
const { sendMedia } = require('../utils/waClientRegistry');

// Maximum file size for WhatsApp (approx 95MB to be safe)
const MAX_WA_SIZE_BYTES = 95 * 1024 * 1024;

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
    const remoteBackupDir = '\\\\WIN-JIAVRTFMA0N\\Sage\\SQLBackups';
    // Use Local Archive Directory on this machine (Cashier-2)
    const localArchiveDir = 'C:\\whatsapp-sql-api\\backups\\sql_archives';
    
    // Ensure dirs exist
    if (!fs.existsSync(remoteBackupDir)) {
        try { fs.mkdirSync(remoteBackupDir, { recursive: true }); } catch (e) {}
    }
    if (!fs.existsSync(localArchiveDir)) {
        try { fs.mkdirSync(localArchiveDir, { recursive: true }); } catch (e) {}
    }

    const remoteFilePath = path.join(remoteBackupDir, fileName);
    const localFilePath = path.join(localArchiveDir, fileName);
    
    console.log(`[DatabaseBackupJob] Backing up ${dbName} to Remote Share: ${remoteFilePath}...`);

    // 1. Execute Backup Command to Remote Share
    try {
        const request = pool.request();
        await request.query(`BACKUP DATABASE [${dbName}] TO DISK = '${remoteFilePath}' WITH FORMAT, COMPRESSION, INIT`);
    } catch (err) {
        console.warn(`[DatabaseBackupJob] Compression backup failed, trying normal backup... (${err.message})`);
        const request = pool.request();
        await request.query(`BACKUP DATABASE [${dbName}] TO DISK = '${remoteFilePath}' WITH FORMAT, INIT`);
    }

    // 2. Copy file to Local Archive (Safety Copy on Cashier-2)
    // User Request: Keep copy on Server AND Local Machine
    console.log(`[DatabaseBackupJob] Copying backup to local archive: ${localFilePath}...`);
    try {
        fs.copyFileSync(remoteFilePath, localFilePath);
        // fs.unlinkSync(remoteFilePath); // User asked to KEEP the file on the Server too.
    } catch (e) {
        console.error('[DatabaseBackupJob] Failed to copy file to local archive:', e);
        // If move failed, we might still have it on remote, so let's continue with remote if local doesn't exist?
        // But for simplicity, lets throw or assume localFilePath is target now.
        // If copy failed, we can't send.
        throw e;
    }

    // Now work with localFilePath
    const stats = fs.statSync(localFilePath);
    const sizeMB = (stats.size / (1024 * 1024)).toFixed(2);
    console.log(`[DatabaseBackupJob] Local copy secured: ${fileName} (${sizeMB} MB)`);
    
    let fileToSend = localFilePath;
    let cleanupFile = false; // Do not cleanup the Archive (User Request: "Crash protection")

    if (stats.size > MAX_WA_SIZE_BYTES) {
        const zipPath = localFilePath + '.zip';
        console.log(`[DatabaseBackupJob] File too large (${sizeMB} MB), zipping locally...`);
        
        await zipFile(localFilePath, zipPath);
        
        const zipStats = fs.statSync(zipPath);
        const zipSizeMB = (zipStats.size / (1024 * 1024)).toFixed(2);
        
        // We send the Zip, but we keep the original BAK or ZIP in archive?
        // Let's keep the ZIP if we made one, and delete the huge BAK to save space?
        // Or keep header.
        
        fileToSend = zipPath;
        
        if (zipStats.size > MAX_WA_SIZE_BYTES) {
            console.error(`[DatabaseBackupJob] Even zipped file is too large: ${zipSizeMB} MB`);
            const { send } = require('../utils/waClientRegistry');
            await send(adminNumber, `⚠️ Backup for ${dbName} is too large to send via WhatsApp (${zipSizeMB} MB). Archived locally at: ${localFilePath}`);
            // We keep the local file.
            try { fs.unlinkSync(zipPath); } catch(e){} // Delete the failed zip wrapper
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
            // Cleanup ZIP only (Keep BAK?) or Keep ZIP and delete BAK?
            // To save space, let's keep ZIP and delete BAK.
            try { fs.unlinkSync(localFilePath); } catch(e){} // Delete BAK
            // We LEAVE the .zip file in the archive folder.
        } else {
            console.error(`[DatabaseBackupJob] Failed to send media: ${mediaResult.error}`);
            throw new Error(mediaResult.error);
        }

    } else {
        // Size is okay, send original BAK
        console.log(`[DatabaseBackupJob] Sending ${fileName}...`);
        const fileData = fs.readFileSync(localFilePath);
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
        exec(cmd, (error, stdout, stderr) => {
            if (error) {
                reject(error);
            } else {
                resolve();
            }
        });
    });
}
