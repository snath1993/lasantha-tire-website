/**
 * Lasantha Tyre — System Control Panel v2.0
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * Electron Main Process
 *
 * Complete desktop management for WhatsApp Bot, PM2 services,
 * scheduled jobs, database, configuration, bot swap (v1↔v2),
 * safe library updates with rollback, and QR code scanning.
 */

const { app, BrowserWindow, ipcMain, Tray, Menu, shell, nativeImage } = require('electron');
const path = require('path');
const fs = require('fs');
const { exec } = require('child_process');
const http = require('http');
const os = require('os');

// ── Constants ──
const PROJECT_ROOT = path.resolve(__dirname, '..', '..');
const SETTINGS_FILE = path.join(__dirname, 'settings.json');
const STATUS_FILE = path.join(PROJECT_ROOT, 'whatsapp-status.json');
const RUNTIME_PORT_FILE = path.join(PROJECT_ROOT, 'runtime-port.json');
const JOB_LOG_FILE = path.join(PROJECT_ROOT, 'job-execution-log.json');
const JOBS_CONFIG_FILE = path.join(PROJECT_ROOT, 'jobs-config.json');
const BACKUP_DIR = path.join(PROJECT_ROOT, 'backups', 'sql_archives');
const AUTH_DIR = path.join(PROJECT_ROOT, 'whatsapp-auth');
const LIB_BACKUP_DIR = path.join(PROJECT_ROOT, 'backups', 'library-update-backup');

const BOT_V1 = 'whatsapp-bot';
const BOT_V2 = 'whatsapp-bot-v2';

let mainWindow = null;
let tray = null;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  SETTINGS
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
function loadSettings() {
    try {
        if (fs.existsSync(SETTINGS_FILE)) return JSON.parse(fs.readFileSync(SETTINGS_FILE, 'utf8'));
    } catch {}
    return { botApiPort: 8585, refreshInterval: 5000, minimizeToTray: true, startMinimized: false, dashboardKey: '' };
}
function saveSettingsToDisk(s) { fs.writeFileSync(SETTINGS_FILE, JSON.stringify(s, null, 2), 'utf8'); }
let settings = loadSettings();

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  BOT API COMMUNICATION
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
function getBotApiPort() {
    try {
        if (fs.existsSync(RUNTIME_PORT_FILE)) {
            return JSON.parse(fs.readFileSync(RUNTIME_PORT_FILE, 'utf8')).port || settings.botApiPort;
        }
    } catch {}
    return settings.botApiPort;
}

function botApi(endpoint, method = 'GET', body = null) {
    return new Promise((resolve, reject) => {
        const port = getBotApiPort();
        const sep = endpoint.includes('?') ? '&' : '?';
        const keyParam = settings.dashboardKey ? `${sep}key=${encodeURIComponent(settings.dashboardKey)}` : '';
        const fullPath = endpoint + keyParam;

        function makeRequest(reqPath, redirectsLeft) {
            const options = {
                hostname: '127.0.0.1', port,
                path: reqPath, method,
                headers: { 'Content-Type': 'application/json' },
                timeout: 15000
            };
            const req = http.request(options, res => {
                // Follow HTTP redirects (301, 302, 307, 308)
                if ([301, 302, 307, 308].includes(res.statusCode) && res.headers.location && redirectsLeft > 0) {
                    res.resume(); // consume & discard redirect body
                    const loc = res.headers.location;
                    const redirectPath = loc.startsWith('http') ? new URL(loc).pathname + new URL(loc).search : loc;
                    return makeRequest(redirectPath, redirectsLeft - 1);
                }
                let data = '';
                res.on('data', c => data += c);
                res.on('end', () => { try { resolve(JSON.parse(data)); } catch { resolve({ raw: data }); } });
            });
            req.on('error', reject);
            req.on('timeout', () => { req.destroy(); reject(new Error('Timeout')); });
            if (body) req.write(JSON.stringify(body));
            req.end();
        }

        makeRequest(fullPath, 3);
    });
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  PM2 HELPERS
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
function pm2Exec(cmd) {
    return new Promise(resolve => {
        exec(`pm2 ${cmd}`, { windowsHide: true, timeout: 15000, cwd: PROJECT_ROOT }, (err, stdout, stderr) => {
            if (err) return resolve({ ok: false, error: err.message, stderr });
            resolve({ ok: true, output: stdout.trim() });
        });
    });
}

function pm2ListParsed() {
    return new Promise(resolve => {
        exec('pm2 jlist', { windowsHide: true, timeout: 10000 }, (err, stdout) => {
            if (err) return resolve([]);
            try {
                const raw = stdout.trim();
                if (!raw || !raw.startsWith('[')) return resolve([]);
                const data = JSON.parse(raw.replace(/"USERNAME":"[^"]*",/gi, '').replace(/"username":"[^"]*",/gi, ''));
                resolve(data.map(p => ({
                    name: p.name, pmId: p.pm_id,
                    status: p.pm2_env?.status || 'unknown',
                    cpu: p.monit?.cpu ?? 0, memory: p.monit?.memory ?? 0,
                    restarts: p.pm2_env?.restart_time ?? 0,
                    uptimeMs: p.pm2_env?.pm_uptime ? Date.now() - p.pm2_env.pm_uptime : 0,
                    pid: p.pid,
                })));
            } catch { resolve([]); }
        });
    });
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  FILE HELPERS
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
function readProjectFile(rel) {
    const full = path.join(PROJECT_ROOT, rel);
    return fs.existsSync(full) ? fs.readFileSync(full, 'utf8') : null;
}
function writeProjectFile(rel, content) {
    const full = path.join(PROJECT_ROOT, rel);
    if (fs.existsSync(full)) { try { fs.copyFileSync(full, full + '.bak'); } catch {} }
    fs.writeFileSync(full, content, 'utf8');
}

// Helper: send progress events to renderer
function sendProgress(step, message, pct) {
    mainWindow?.webContents.send('update-progress', { step, message, progress: pct });
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  WINDOW & TRAY
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
function createWindow() {
    mainWindow = new BrowserWindow({
        width: 1320, height: 880, minWidth: 1000, minHeight: 640,
        frame: false, backgroundColor: '#0c0c1d',
        icon: path.join(__dirname, 'assets', 'icon.png'),
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            contextIsolation: true, nodeIntegration: false, sandbox: false,
        },
        show: false,
    });
    mainWindow.loadFile(path.join(__dirname, 'renderer', 'index.html'));
    mainWindow.once('ready-to-show', () => { if (!settings.startMinimized) mainWindow.show(); });
    mainWindow.on('close', e => {
        if (!app.isQuitting && settings.minimizeToTray && tray) { e.preventDefault(); mainWindow.hide(); }
    });
    mainWindow.on('closed', () => { mainWindow = null; });
}

function createTray() {
    try {
        const iconPath = path.join(__dirname, 'assets', 'icon.png');
        tray = new Tray(fs.existsSync(iconPath) ? iconPath : nativeImage.createEmpty());
    } catch { return; }
    const contextMenu = Menu.buildFromTemplate([
        { label: 'Show Control Panel', click: () => { mainWindow?.show(); mainWindow?.focus(); } },
        { type: 'separator' },
        { label: 'Restart Bot', click: () => pm2Exec('restart whatsapp-bot-v2') },
        { label: 'Stop Bot', click: () => pm2Exec('stop whatsapp-bot-v2') },
        { type: 'separator' },
        { label: 'Quit', click: () => { app.isQuitting = true; app.quit(); } }
    ]);
    tray.setToolTip('Lasantha Tyre Control Panel');
    tray.setContextMenu(contextMenu);
    tray.on('double-click', () => { mainWindow?.show(); mainWindow?.focus(); });
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━
//  APP LIFECYCLE
// ━━━━━━━━━━━━━━━━━━━━━━━━━━
app.disableHardwareAcceleration();
app.whenReady().then(() => { createWindow(); createTray(); });
app.on('window-all-closed', () => { if (process.platform !== 'darwin') app.quit(); });
app.on('activate', () => { if (BrowserWindow.getAllWindows().length === 0) createWindow(); });
app.on('before-quit', () => { app.isQuitting = true; });

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  IPC HANDLERS
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

// ── Window Controls ──
ipcMain.handle('window-minimize', () => mainWindow?.minimize());
ipcMain.handle('window-maximize', () => {
    mainWindow?.isMaximized() ? mainWindow.unmaximize() : mainWindow?.maximize();
    return mainWindow?.isMaximized();
});
ipcMain.handle('window-close', () => mainWindow?.close());

// ── Overview ──
ipcMain.handle('get-overview', async () => {
    const sysInfo = {
        platform: os.platform(), hostname: os.hostname(),
        totalMemory: os.totalmem(), freeMemory: os.freemem(),
        cpus: os.cpus().length, uptime: os.uptime(), nodeVersion: process.version,
    };
    try {
        const overview = await botApi('/api/admin/overview');
        overview.system = sysInfo;
        return overview;
    } catch {
        const services = await pm2ListParsed();
        let waStatus = { isReady: false, state: 'UNKNOWN', qrAvailable: false };
        try {
            if (fs.existsSync(STATUS_FILE)) {
                const s = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
                waStatus = { isReady: !!s.ready, state: s.ready ? 'CONNECTED' : 'DISCONNECTED', qrAvailable: !!s.qr };
            }
        } catch {}
        return {
            ok: false, botApiDown: true, services, whatsapp: waStatus,
            database: { connected: false, error: 'Bot API unreachable' },
            jobs: { tracked: {}, failingCount: 0, retryingCount: 0 },
            system: sysInfo,
        };
    }
});

// ── Services ──
ipcMain.handle('get-services', () => pm2ListParsed());
ipcMain.handle('service-action', async (_, { name, action }) => {
    if (!/^[a-zA-Z0-9_.\-]+$/.test(name)) return { ok: false, error: 'Invalid name' };
    if (!['restart', 'stop', 'start'].includes(action)) return { ok: false, error: 'Invalid action' };
    return pm2Exec(`${action} ${name}`);
});

// ── WhatsApp ──
ipcMain.handle('get-whatsapp-status', async () => {
    try { return await botApi('/api/admin/overview').then(d => d.whatsapp); }
    catch {
        try {
            const s = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
            return { isReady: !!s.ready, state: s.ready ? 'CONNECTED' : 'DISCONNECTED', qrAvailable: !!s.qr };
        } catch { return { isReady: false, state: 'UNKNOWN' }; }
    }
});

ipcMain.handle('get-qr-code', async () => {
    try { return await botApi('/api/admin/whatsapp/qr'); }
    catch {
        try {
            const s = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
            if (s.qr) return { available: true, qr: s.qr, dataUrl: null };
        } catch {}
        return { available: false };
    }
});

ipcMain.handle('reconnect-whatsapp', async () => {
    try { return await botApi('/api/admin/whatsapp/reconnect', 'POST'); }
    catch (e) { return { ok: false, error: e.message }; }
});

// ── Hard Reset WhatsApp (clear session + restart) ──
ipcMain.handle('hard-reset-whatsapp', async () => {
    try {
        // 1. Stop the bot
        await pm2Exec('stop whatsapp-bot-v2');
        await new Promise(r => setTimeout(r, 3000));

        // 2. Clear auth/session directory
        if (fs.existsSync(AUTH_DIR)) {
            fs.rmSync(AUTH_DIR, { recursive: true, force: true });
            fs.mkdirSync(AUTH_DIR, { recursive: true });
        }

        // 3. Clear QR from status file
        try {
            if (fs.existsSync(STATUS_FILE)) {
                const s = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
                s.ready = false; s.qr = null;
                fs.writeFileSync(STATUS_FILE, JSON.stringify(s, null, 2));
            }
        } catch {}

        // 4. Restart
        await pm2Exec('restart whatsapp-bot-v2');
        return { ok: true, message: 'Session cleared. Bot restarting — scan QR code when it appears.' };
    } catch (e) {
        // Try to start anyway
        await pm2Exec('restart whatsapp-bot-v2');
        return { ok: false, error: e.message };
    }
});

// ── Send Test Message ──
ipcMain.handle('send-test-message', async (_, { number, message }) => {
    try { return await botApi('/api/titan/message', 'POST', { message, sender: number }); }
    catch (e) { return { ok: false, error: e.message }; }
});

// ── Jobs (FIXED: parse object-format jobs-config.json) ──
ipcMain.handle('get-jobs', async () => {
    const result = { config: [], history: {}, tracked: {} };
    try {
        if (fs.existsSync(JOBS_CONFIG_FILE)) {
            const cfg = JSON.parse(fs.readFileSync(JOBS_CONFIG_FILE, 'utf8'));
            // jobs-config.json is an object with job class names as keys
            result.config = Object.entries(cfg).map(([id, job]) => ({
                id, name: job.name || id,
                description: job.description || '',
                schedule: job.schedule || '',
                enabled: job.enabled !== false,
                icon: job.icon || '📋',
                type: job.type || 'daily',
            }));
        }
    } catch {}
    try {
        if (fs.existsSync(JOB_LOG_FILE)) result.history = JSON.parse(fs.readFileSync(JOB_LOG_FILE, 'utf8'));
    } catch {}
    try {
        const overview = await botApi('/api/admin/overview');
        result.tracked = overview.jobs?.tracked || {};
    } catch {}
    return result;
});

ipcMain.handle('trigger-job', async (_, jobName) => {
    try { return await botApi(`/api/admin/trigger/${encodeURIComponent(jobName)}`); }
    catch (e) { return { ok: false, error: e.message }; }
});

ipcMain.handle('get-job-history', async (_, jobName) => {
    try {
        if (!fs.existsSync(JOB_LOG_FILE)) return [];
        return JSON.parse(fs.readFileSync(JOB_LOG_FILE, 'utf8'))[jobName] || [];
    } catch { return []; }
});

// ── Database ──
ipcMain.handle('get-db-status', async () => {
    try {
        const health = await botApi('/api/admin/health');
        return { connected: health.database === true, status: health.status, name: 'LasanthaTire' };
    } catch (e) { return { connected: false, error: e.message }; }
});

ipcMain.handle('trigger-backup', async () => {
    try { return await botApi('/api/admin/trigger/db-backup'); }
    catch (e) { return { ok: false, error: e.message }; }
});

ipcMain.handle('get-backup-files', async () => {
    try {
        if (!fs.existsSync(BACKUP_DIR)) return [];
        return fs.readdirSync(BACKUP_DIR)
            .filter(f => f.endsWith('.zip') || f.endsWith('.bak'))
            .map(f => { const st = fs.statSync(path.join(BACKUP_DIR, f)); return { name: f, size: st.size, date: st.mtime.toISOString() }; })
            .sort((a, b) => new Date(b.date) - new Date(a.date));
    } catch { return []; }
});

// ── Logs ──
ipcMain.handle('get-logs', async (_, opts = {}) => {
    const service = opts.service || 'whatsapp-bot-v2';
    const lines = opts.lines || 200;
    return new Promise(resolve => {
        exec(`pm2 logs ${service} --nostream --lines ${lines} --raw`, {
            windowsHide: true, timeout: 10000, maxBuffer: 2 * 1024 * 1024
        }, (err, stdout, stderr) => {
            if (err) return resolve({ ok: false, logs: [`Error: ${err.message}`] });
            const all = ((stdout || '') + '\n' + (stderr || '')).split('\n').filter(Boolean).slice(-lines);
            resolve({ ok: true, logs: all });
        });
    });
});

ipcMain.handle('flush-pm2-logs', async () => pm2Exec('flush'));

// ── Configuration ──
ipcMain.handle('get-config-list', () => [
    { key: 'env', label: '.env', file: '.env' },
    { key: 'jobs-config', label: 'jobs-config.json', file: 'jobs-config.json' },
    { key: 'ecosystem', label: 'ecosystem.config.js', file: 'ecosystem.config.js' },
    { key: 'watched-items', label: 'watched-item-config.json', file: 'watched-item-config.json' },
    { key: 'jobs-status', label: 'job-status.json (read-only)', file: 'job-status.json' },
]);

ipcMain.handle('get-config-file', async (_, name) => {
    const allowed = { 'env': '.env', 'jobs-config': 'jobs-config.json', 'ecosystem': 'ecosystem.config.js', 'watched-items': 'watched-item-config.json', 'jobs-status': 'job-status.json' };
    const file = allowed[name];
    if (!file) return { ok: false, error: 'Unknown file' };
    return { ok: true, name: file, content: readProjectFile(file) || '' };
});

ipcMain.handle('save-config-file', async (_, { name, content }) => {
    const allowed = { 'env': '.env', 'jobs-config': 'jobs-config.json', 'ecosystem': 'ecosystem.config.js', 'watched-items': 'watched-item-config.json' };
    const file = allowed[name];
    if (!file) return { ok: false, error: 'Unknown or read-only file' };
    try { writeProjectFile(file, content); return { ok: true, message: `${file} saved (backup created)` }; }
    catch (e) { return { ok: false, error: e.message }; }
});

// ── System Info ──
ipcMain.handle('get-system-info', () => ({
    platform: os.platform(), hostname: os.hostname(), arch: os.arch(), release: os.release(),
    totalMemory: os.totalmem(), freeMemory: os.freemem(), cpus: os.cpus().length,
    cpuModel: os.cpus()[0]?.model || 'Unknown', uptime: os.uptime(),
    nodeVersion: process.version, electronVersion: process.versions.electron, projectRoot: PROJECT_ROOT,
}));

// ── Actions ──
ipcMain.handle('open-folder', async (_, folderPath) => {
    shell.openPath(path.isAbsolute(folderPath) ? folderPath : path.join(PROJECT_ROOT, folderPath));
    return { ok: true };
});

ipcMain.handle('open-dashboard-web', () => {
    shell.openExternal(`http://localhost:${getBotApiPort()}/admin`);
    return { ok: true };
});

ipcMain.handle('restart-all-tunnels', async () => {
    const tunnels = ['lasantha-bot-tunnel', 'lasantha-app-tunnel', 'lasantha-fb-tunnel', 'royal-booking-tunnel'];
    for (const t of tunnels) await pm2Exec(`restart ${t}`);
    return { ok: true };
});

// ── Settings ──
ipcMain.handle('get-settings', () => settings);
ipcMain.handle('save-settings', (_, s) => {
    settings = { ...settings, ...s };
    saveSettingsToDisk(settings);
    return { ok: true };
});

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  BOT SWAP (v1 ↔ v2)
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ipcMain.handle('get-active-bot', async () => {
    const all = await pm2ListParsed();
    const v1 = all.find(s => s.name === BOT_V1);
    const v2 = all.find(s => s.name === BOT_V2);
    return {
        active: v2?.status === 'online' ? BOT_V2 : (v1?.status === 'online' ? BOT_V1 : null),
        botV1: v1 || null,
        botV2: v2 || null,
    };
});

ipcMain.handle('bot-swap', async () => {
    const all = await pm2ListParsed();
    const v1Online = all.find(s => s.name === BOT_V1)?.status === 'online';
    const v2Online = all.find(s => s.name === BOT_V2)?.status === 'online';

    if (v2Online) {
        await pm2Exec(`stop ${BOT_V2}`);
        await new Promise(r => setTimeout(r, 2000));
        const res = await pm2Exec(`start ${BOT_V1}`);
        if (!res.ok) {
            await pm2Exec(`restart ${BOT_V2}`);
            return { ok: false, error: `Failed to start ${BOT_V1}: ${res.error}. V2 restarted.` };
        }
        return { ok: true, switchedTo: BOT_V1, message: 'Swapped to legacy bot (v1)' };
    } else if (v1Online) {
        await pm2Exec(`stop ${BOT_V1}`);
        await new Promise(r => setTimeout(r, 2000));
        const res = await pm2Exec(`start ${BOT_V2}`);
        if (!res.ok) {
            await pm2Exec(`restart ${BOT_V1}`);
            return { ok: false, error: `Failed to start ${BOT_V2}: ${res.error}. V1 restarted.` };
        }
        return { ok: true, switchedTo: BOT_V2, message: 'Swapped to new bot (v2)' };
    } else {
        const res = await pm2Exec(`start ${BOT_V2}`);
        return { ok: res.ok, switchedTo: BOT_V2, message: 'Neither bot was running — started v2' };
    }
});

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  SAFE LIBRARY UPDATE (with rollback)
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ipcMain.handle('get-library-info', async () => {
    const result = { current: null, latest: null, lastUpdate: null };
    // Current installed version
    try {
        const pkgPath = path.join(PROJECT_ROOT, 'node_modules', 'whatsapp-web.js', 'package.json');
        if (fs.existsSync(pkgPath)) result.current = JSON.parse(fs.readFileSync(pkgPath, 'utf8')).version;
    } catch {}
    // Latest available on npm
    try {
        result.latest = await new Promise((resolve) => {
            exec('npm view whatsapp-web.js version', { windowsHide: true, timeout: 15000, cwd: PROJECT_ROOT },
                (err, stdout) => resolve(err ? null : stdout.trim()));
        });
    } catch {}
    // Last update log
    try {
        const logPath = path.join(LIB_BACKUP_DIR, 'update-history.json');
        if (fs.existsSync(logPath)) {
            const history = JSON.parse(fs.readFileSync(logPath, 'utf8'));
            result.lastUpdate = history[history.length - 1] || null;
        }
    } catch {}
    return result;
});

ipcMain.handle('safe-library-update', async () => {
    const waModulePkg = path.join(PROJECT_ROOT, 'node_modules', 'whatsapp-web.js', 'package.json');
    const lockFile = path.join(PROJECT_ROOT, 'package-lock.json');

    try {
        // Step 1: Create backup
        sendProgress(1, 'Creating backup of current packages...', 5);
        if (!fs.existsSync(LIB_BACKUP_DIR)) fs.mkdirSync(LIB_BACKUP_DIR, { recursive: true });
        const lockBackup = path.join(LIB_BACKUP_DIR, 'package-lock.json.bak');
        if (fs.existsSync(lockFile)) fs.copyFileSync(lockFile, lockBackup);

        let oldVersion = 'unknown';
        try { oldVersion = JSON.parse(fs.readFileSync(waModulePkg, 'utf8')).version; } catch {}
        fs.writeFileSync(path.join(LIB_BACKUP_DIR, 'old-version.txt'), oldVersion);
        sendProgress(2, `Backup complete. Current: v${oldVersion}`, 15);

        // Step 2: Stop bot
        sendProgress(3, 'Stopping the bot...', 25);
        await pm2Exec(`stop ${BOT_V2}`);
        await new Promise(r => setTimeout(r, 2000));

        // Step 3: Install latest library
        sendProgress(4, 'Installing latest whatsapp-web.js...', 35);
        const installResult = await new Promise(resolve => {
            exec('npm install whatsapp-web.js@latest --save', {
                windowsHide: true, timeout: 180000, cwd: PROJECT_ROOT, maxBuffer: 5 * 1024 * 1024,
            }, (err, stdout, stderr) => {
                resolve(err ? { ok: false, error: err.message, stderr } : { ok: true, output: stdout });
            });
        });

        if (!installResult.ok) {
            sendProgress(5, 'Install FAILED. Rolling back...', 50);
            // Rollback
            if (fs.existsSync(lockBackup)) fs.copyFileSync(lockBackup, lockFile);
            await new Promise(r => {
                exec('npm install', { windowsHide: true, timeout: 180000, cwd: PROJECT_ROOT }, () => r());
            });
            sendProgress(6, 'Rollback complete. Starting bot...', 75);
            await pm2Exec(`restart ${BOT_V2}`);
            sendProgress(7, `Rolled back to v${oldVersion}`, 100);
            return { ok: false, error: 'npm install failed: ' + installResult.error, rolledBack: true, oldVersion };
        }

        let newVersion = 'unknown';
        try { newVersion = JSON.parse(fs.readFileSync(waModulePkg, 'utf8')).version; } catch {}
        sendProgress(5, `Installed v${newVersion}. Restarting bot...`, 55);

        // Step 4: Restart bot
        await pm2Exec(`restart ${BOT_V2}`);

        // Step 5: Health check (wait up to 25 seconds)
        sendProgress(6, 'Waiting for bot to start (health check)...', 70);
        let healthy = false;
        for (let attempt = 0; attempt < 3; attempt++) {
            await new Promise(r => setTimeout(r, 8000));
            try {
                const h = await botApi('/api/admin/health');
                if (h.status === 'healthy' || h.database === true) { healthy = true; break; }
            } catch {}
            sendProgress(6, `Health check attempt ${attempt + 2}...`, 70 + (attempt + 1) * 8);
        }

        if (!healthy) {
            sendProgress(7, 'Bot unhealthy after update! Rolling back...', 88);
            await pm2Exec(`stop ${BOT_V2}`);
            if (fs.existsSync(lockBackup)) fs.copyFileSync(lockBackup, lockFile);
            await new Promise(r => {
                exec('npm install', { windowsHide: true, timeout: 180000, cwd: PROJECT_ROOT }, () => r());
            });
            await pm2Exec(`restart ${BOT_V2}`);
            sendProgress(8, `Rolled back to v${oldVersion} (bot was unhealthy)`, 100);
            return { ok: false, error: 'Bot unhealthy after update', rolledBack: true, oldVersion, newVersion };
        }

        sendProgress(8, `Update successful! v${oldVersion} → v${newVersion}`, 100);

        // Log the update
        try {
            const historyPath = path.join(LIB_BACKUP_DIR, 'update-history.json');
            let history = [];
            try { history = JSON.parse(fs.readFileSync(historyPath, 'utf8')); } catch {}
            history.push({ date: new Date().toISOString(), from: oldVersion, to: newVersion, success: true });
            fs.writeFileSync(historyPath, JSON.stringify(history, null, 2));
        } catch {}

        return { ok: true, oldVersion, newVersion };
    } catch (e) {
        sendProgress(-1, 'Critical error: ' + e.message, 0);
        await pm2Exec(`restart ${BOT_V2}`);
        return { ok: false, error: e.message };
    }
});

// ── Manual Rollback ──
ipcMain.handle('rollback-library', async () => {
    const lockBackup = path.join(LIB_BACKUP_DIR, 'package-lock.json.bak');
    const lockFile = path.join(PROJECT_ROOT, 'package-lock.json');
    if (!fs.existsSync(lockBackup)) return { ok: false, error: 'No backup found to roll back to' };
    try {
        sendProgress(1, 'Stopping bot...', 10);
        await pm2Exec(`stop ${BOT_V2}`);
        sendProgress(2, 'Restoring package-lock.json...', 30);
        fs.copyFileSync(lockBackup, lockFile);
        sendProgress(3, 'Running npm install...', 50);
        await new Promise(r => {
            exec('npm install', { windowsHide: true, timeout: 180000, cwd: PROJECT_ROOT }, () => r());
        });
        sendProgress(4, 'Restarting bot...', 80);
        await pm2Exec(`restart ${BOT_V2}`);
        let ver = 'unknown';
        try { ver = JSON.parse(fs.readFileSync(path.join(PROJECT_ROOT, 'node_modules', 'whatsapp-web.js', 'package.json'), 'utf8')).version; } catch {}
        sendProgress(5, `Rollback complete. Now on v${ver}`, 100);
        return { ok: true, version: ver };
    } catch (e) {
        await pm2Exec(`restart ${BOT_V2}`);
        return { ok: false, error: e.message };
    }
});

// ── Disk Space ──
ipcMain.handle('get-disk-space', async () => {
    return new Promise(resolve => {
        exec('powershell -NoProfile -Command "Get-PSDrive -PSProvider FileSystem | Select-Object Name,@{N=\'UsedGB\';E={[math]::Round($_.Used/1GB,2)}},@{N=\'FreeGB\';E={[math]::Round($_.Free/1GB,2)}},@{N=\'TotalGB\';E={[math]::Round(($_.Used+$_.Free)/1GB,2)}} | ConvertTo-Json"',
        { windowsHide: true, timeout: 8000 }, (err, stdout) => {
            if (err) return resolve([]);
            try {
                let data = JSON.parse(stdout);
                if (!Array.isArray(data)) data = [data];
                resolve(data.filter(d => d.TotalGB > 0));
            } catch { resolve([]); }
        });
    });
});
