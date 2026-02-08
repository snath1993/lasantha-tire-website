const fs = require('fs');
const path = require('path');

/**
 * 🔍 WhatsApp Session Health Check Utility
 * 
 * මේ utility එක session data හරියට save වෙලා තියෙනවාද කියලා check කරනවා
 * Bot restart කරද්දී credentials load වෙන්නේ නැත්තං මේක use කරලා problem detect කරන්න පුළුවන්
 */

const AUTH_PATH = './.wwebjs_auth';
const SESSION_ID = 'lasantha-tire-bot';

/**
 * Check if session exists and is valid
 * @returns {Object} { valid: boolean, reason: string, details: object }
 */
function checkSessionHealth() {
    const result = {
        valid: false,
        reason: '',
        details: {}
    };

    const sessionPath = path.join(AUTH_PATH, `session-${SESSION_ID}`);
    
    // Check 1: Session folder exists
    if (!fs.existsSync(sessionPath)) {
        result.reason = 'Session folder not found';
        return result;
    }
    result.details.sessionFolder = '✅ Exists';

    // Check 2: Default profile exists
    const defaultPath = path.join(sessionPath, 'Default');
    if (!fs.existsSync(defaultPath)) {
        result.reason = 'Default profile not found';
        return result;
    }
    result.details.defaultProfile = '✅ Exists';

    // Check 3: IndexedDB exists (critical for WhatsApp Web)
    const indexedDBPath = path.join(defaultPath, 'IndexedDB');
    if (!fs.existsSync(indexedDBPath)) {
        result.reason = 'IndexedDB not found - session not authenticated';
        return result;
    }
    
    // Check IndexedDB has data
    const indexedDBFiles = fs.readdirSync(path.join(indexedDBPath, 'https_web.whatsapp.com_0.indexeddb.leveldb'), { withFileTypes: true })
        .filter(f => f.isFile() && f.name.endsWith('.ldb'));
    
    if (indexedDBFiles.length === 0) {
        result.reason = 'IndexedDB empty - no authentication data';
        return result;
    }
    result.details.indexedDB = `✅ ${indexedDBFiles.length} data files`;

    // Check 4: Local Storage exists
    const localStoragePath = path.join(defaultPath, 'Local Storage');
    if (!fs.existsSync(localStoragePath)) {
        result.reason = 'Local Storage not found';
        return result;
    }
    
    // Check Local Storage leveldb
    const leveldbPath = path.join(localStoragePath, 'leveldb');
    if (!fs.existsSync(leveldbPath)) {
        result.reason = 'Local Storage leveldb not found';
        return result;
    }
    
    const leveldbFiles = fs.readdirSync(leveldbPath, { withFileTypes: true })
        .filter(f => f.isFile() && (f.name.endsWith('.log') || f.name.endsWith('.ldb')));
    
    if (leveldbFiles.length === 0) {
        result.reason = 'Local Storage empty';
        return result;
    }
    result.details.localStorage = `✅ ${leveldbFiles.length} data files`;

    // Check 5: Session Storage exists
    const sessionStoragePath = path.join(defaultPath, 'Session Storage');
    if (!fs.existsSync(sessionStoragePath)) {
        result.reason = 'Session Storage not found';
        return result;
    }
    result.details.sessionStorage = '✅ Exists';

    // Check 6: Check last modification time (should be recent)
    const stats = fs.statSync(leveldbPath);
    const lastModified = stats.mtime;
    const ageHours = (Date.now() - lastModified.getTime()) / (1000 * 60 * 60);
    result.details.lastModified = `${Math.round(ageHours)} hours ago`;
    
    if (ageHours > 72) {
        result.reason = `Session data old (${Math.round(ageHours)} hours) - may need refresh`;
        result.valid = false;
        return result;
    }

    // All checks passed
    result.valid = true;
    result.reason = 'Session healthy - should connect without QR';
    
    return result;
}

/**
 * Get session file sizes for diagnostics
 */
function getSessionStats() {
    const sessionPath = path.join(AUTH_PATH, `session-${SESSION_ID}`);
    
    if (!fs.existsSync(sessionPath)) {
        return { error: 'Session not found' };
    }

    const stats = {
        totalFiles: 0,
        totalSize: 0,
        folders: {}
    };

    function scanDirectory(dir, name) {
        const files = fs.readdirSync(dir, { withFileTypes: true });
        let folderSize = 0;
        let fileCount = 0;

        for (const file of files) {
            const fullPath = path.join(dir, file.name);
            
            if (file.isDirectory()) {
                const subStats = scanDirectory(fullPath, file.name);
                folderSize += subStats.size;
                fileCount += subStats.count;
            } else {
                const stat = fs.statSync(fullPath);
                folderSize += stat.size;
                fileCount++;
            }
        }

        stats.totalFiles += fileCount;
        stats.totalSize += folderSize;
        stats.folders[name] = {
            files: fileCount,
            size: `${(folderSize / 1024).toFixed(2)} KB`
        };

        return { size: folderSize, count: fileCount };
    }

    try {
        scanDirectory(sessionPath, 'root');
        stats.totalSizeFormatted = `${(stats.totalSize / (1024 * 1024)).toFixed(2)} MB`;
        return stats;
    } catch (err) {
        return { error: err.message };
    }
}

/**
 * Delete corrupted session
 */
function deleteCorruptedSession() {
    const sessionPath = path.join(AUTH_PATH, `session-${SESSION_ID}`);
    
    if (!fs.existsSync(sessionPath)) {
        return { success: false, reason: 'Session not found' };
    }

    try {
        fs.rmSync(sessionPath, { recursive: true, force: true });
        return { success: true, message: 'Session deleted - fresh QR scan required' };
    } catch (err) {
        return { success: false, reason: err.message };
    }
}

module.exports = {
    checkSessionHealth,
    getSessionStats,
    deleteCorruptedSession
};
