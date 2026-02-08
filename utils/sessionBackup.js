/**
 * WhatsApp Session Backup Utility
 * 
 * මෙම utility එක නිතරම session data backup කර ගන්නවා.
 * Session එක corrupt වුණාම, backup එකෙන් restore කරන්න පුළුවන්.
 */

const fs = require('fs');
const path = require('path');

const AUTH_PATH = path.join(__dirname, '..', '.wwebjs_auth');
const BACKUP_PATH = path.join(__dirname, '..', 'whatsapp-auth-backup');
const BACKUP_INTERVAL = 3600000; // Backup every 1 hour (3600000ms)
const MAX_BACKUPS = 5; // Keep last 5 backups only

/**
 * Create a backup of the WhatsApp session data
 */
async function createSessionBackup() {
    try {
        // Check if auth folder exists
        if (!fs.existsSync(AUTH_PATH)) {
            console.log('[Session Backup] No session data to backup');
            return false;
        }

        // Create backup directory if it doesn't exist
        if (!fs.existsSync(BACKUP_PATH)) {
            fs.mkdirSync(BACKUP_PATH, { recursive: true });
        }

        // Create timestamped backup folder
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
        const backupFolder = path.join(BACKUP_PATH, `session-backup-${timestamp}`);

        // Copy entire auth folder to backup
        await copyDir(AUTH_PATH, backupFolder);

        console.log(`[Session Backup] ✅ Backup created: ${backupFolder}`);

        // Clean up old backups (keep only last MAX_BACKUPS)
        cleanupOldBackups();

        return true;
    } catch (err) {
        console.error(`[Session Backup] ❌ Backup failed: ${err.message}`);
        return false;
    }
}

/**
 * Restore session from the most recent backup
 */
async function restoreSessionFromBackup() {
    try {
        if (!fs.existsSync(BACKUP_PATH)) {
            console.log('[Session Backup] No backups available');
            return false;
        }

        // Get all backup folders
        const backups = fs.readdirSync(BACKUP_PATH)
            .filter(f => f.startsWith('session-backup-'))
            .map(f => ({
                name: f,
                path: path.join(BACKUP_PATH, f),
                time: fs.statSync(path.join(BACKUP_PATH, f)).mtime.getTime()
            }))
            .sort((a, b) => b.time - a.time); // Sort by newest first

        if (backups.length === 0) {
            console.log('[Session Backup] No valid backups found');
            return false;
        }

        const latestBackup = backups[0];
        console.log(`[Session Backup] Restoring from: ${latestBackup.name}`);

        // Remove current auth folder if it exists
        if (fs.existsSync(AUTH_PATH)) {
            fs.rmSync(AUTH_PATH, { recursive: true, force: true });
        }

        // Copy backup to auth folder
        await copyDir(latestBackup.path, AUTH_PATH);

        console.log('[Session Backup] ✅ Session restored successfully');
        return true;
    } catch (err) {
        console.error(`[Session Backup] ❌ Restore failed: ${err.message}`);
        return false;
    }
}

/**
 * Delete old backups (keep only last MAX_BACKUPS)
 */
function cleanupOldBackups() {
    try {
        if (!fs.existsSync(BACKUP_PATH)) return;

        const backups = fs.readdirSync(BACKUP_PATH)
            .filter(f => f.startsWith('session-backup-'))
            .map(f => ({
                name: f,
                path: path.join(BACKUP_PATH, f),
                time: fs.statSync(path.join(BACKUP_PATH, f)).mtime.getTime()
            }))
            .sort((a, b) => b.time - a.time); // Sort by newest first

        // Delete backups beyond MAX_BACKUPS
        if (backups.length > MAX_BACKUPS) {
            const toDelete = backups.slice(MAX_BACKUPS);
            toDelete.forEach(backup => {
                try {
                    fs.rmSync(backup.path, { recursive: true, force: true });
                    console.log(`[Session Backup] Deleted old backup: ${backup.name}`);
                } catch (err) {
                    console.error(`[Session Backup] Failed to delete ${backup.name}: ${err.message}`);
                }
            });
        }
    } catch (err) {
        console.error(`[Session Backup] Cleanup error: ${err.message}`);
    }
}

/**
 * Recursively copy directory
 */
async function copyDir(src, dest) {
    try {
        // Create destination directory
        if (!fs.existsSync(dest)) {
            fs.mkdirSync(dest, { recursive: true });
        }

        const entries = fs.readdirSync(src, { withFileTypes: true });

        for (const entry of entries) {
            const srcPath = path.join(src, entry.name);
            const destPath = path.join(dest, entry.name);

            try {
                // Skip SQLite temporary/lock files which are often locked (EBUSY)
                if (entry.name.endsWith('-wal') || entry.name.endsWith('-shm')) {
                     continue;
                }

                if (entry.isDirectory()) {
                    // Create subdirectory first
                    if (!fs.existsSync(destPath)) {
                        fs.mkdirSync(destPath, { recursive: true });
                    }
                    await copyDir(srcPath, destPath);
                } else {
                    // Ensure parent directory exists before copying file
                    const destDir = path.dirname(destPath);
                    if (!fs.existsSync(destDir)) {
                        fs.mkdirSync(destDir, { recursive: true });
                    }
                    fs.copyFileSync(srcPath, destPath);
                }
            } catch (fileErr) {
                if (fileErr && fileErr.code === 'ENOENT') {
                    // File disappeared during copy (common with LevelDB temp files) - safe to ignore
                    continue;
                }
                if (fileErr && fileErr.code === 'EBUSY') {
                    // File is locked by the process - skip silently or with debug log to avoid clutter
                    console.warn(`[Session Backup] Skip (Locked): ${entry.name}`);
                    continue;
                }
                console.warn(`[Session Backup] Skip: ${entry.name} (${fileErr.code || fileErr.message})`);
            }
        }
    } catch (err) {
        console.error(`[Session Backup] Copy error for ${src}: ${err.message}`);
        throw err;
    }
}

/**
 * Start automatic session backup
 */
function startSessionBackup(logFn = console.log) {
    logFn('[Session Backup] Starting automatic backup system...');
    
    // Create initial backup after 5 minutes (give time for session to establish)
    setTimeout(() => {
        createSessionBackup();
    }, 300000); // 5 minutes

    // Then backup every hour
    setInterval(() => {
        createSessionBackup();
    }, BACKUP_INTERVAL);

    logFn(`[Session Backup] Automatic backup enabled (every ${BACKUP_INTERVAL / 60000} minutes)`);
}

module.exports = {
    createSessionBackup,
    restoreSessionFromBackup,
    startSessionBackup
};
