/**
 * Lasantha Tyre — System Control Panel
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * Electron Main Process
 *
 * Desktop application for complete management of the
 * WhatsApp Bot, PM2 services, scheduled jobs, database,
 * configuration, and QR code scanning.
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

let mainWindow = null;
let tray = null;

// ── Settings Management ──
function loadSettings() {
    try {
        if (fs.existsSync(SETTINGS_FILE)) {
            return JSON.parse(fs.readFileSync(SETTINGS_FILE, 'utf8'));
        }
    } catch {}
    return {
        botApiPort: 8585,
        refreshInterval: 5000,
        minimizeToTray: true,
        startMinimized: false,
        dashboardKey: '',
    };
}
function saveSettingsToDisk(settings) {
    fs.writeFileSync(SETTINGS_FILE, JSON.stringify(settings, null, 2), 'utf8');
}

let settings = loadSettings();

// ── Bot API Communication ──
function getBotApiPort() {
    try {
        if (fs.existsSync(RUNTIME_PORT_FILE)) {
            const data = JSON.parse(fs.readFileSync(RUNTIME_PORT_FILE, 'utf8'));
            return data.port || settings.botApiPort;
        }
    } catch {}
    return settings.botApiPort;
}

function botApi(endpoint, method = 'GET', body = null) {
    return new Promise((resolve, reject) => {
        const port = getBotApiPort();
        const keyParam = settings.dashboardKey ? `?key=${encodeURIComponent(settings.dashboardKey)}` : '';
        const urlPath = endpoint.includes('?') ? `${endpoint}&key=${encodeURIComponent(settings.dashboardKey)}` : `${endpoint}${keyParam}`;
        const options = {
            hostname: '127.0.0.1', port,
            path: urlPath, method,
            headers: { 'Content-Type': 'application/json' },
            timeout: 10000
        };
        const req = http.request(options, (res) => {
            let data = '';
            res.on('data', chunk => data += chunk);
            res.on('end', () => {
                try { resolve(JSON.parse(data)); }
                catch { resolve({ raw: data }); }
            });
        });
        req.on('error', err => reject(err));
        req.on('timeout', () => { req.destroy(); reject(new Error('Timeout')); });
        if (body) req.write(JSON.stringify(body));
        req.end();
    });
}

// ── PM2 Commands ──
function pm2Exec(cmd) {
    return new Promise((resolve) => {
        exec(`pm2 ${cmd}`, { windowsHide: true, timeout: 15000, cwd: PROJECT_ROOT }, (err, stdout, stderr) => {
            if (err) return resolve({ ok: false, error: err.message, stderr });
            resolve({ ok: true, output: stdout.trim() });
        });
    });
}

function pm2ListParsed() {
    return new Promise((resolve) => {
        exec('pm2 jlist', { windowsHide: true, timeout: 10000 }, (err, stdout) => {
            if (err) return resolve([]);
            try {
                const raw = stdout.trim();
                if (!raw || !raw.startsWith('[')) return resolve([]);
                const data = JSON.parse(raw.replace(/"USERNAME":"[^"]*",/gi, '').replace(/"username":"[^"]*",/gi, ''));
                resolve(data.map(p => ({
                    name: p.name,
                    pmId: p.pm_id,
                    status: p.pm2_env?.status || 'unknown',
                    cpu: p.monit?.cpu ?? 0,
                    memory: p.monit?.memory ?? 0,
                    restarts: p.pm2_env?.restart_time ?? 0,
                    uptimeMs: p.pm2_env?.pm_uptime ? Date.now() - p.pm2_env.pm_uptime : 0,
                    pid: p.pid,
                })));
            } catch { resolve([]); }
        });
    });
}

// ── File Helpers ──
function readProjectFile(relativePath) {
    const full = path.join(PROJECT_ROOT, relativePath);
    if (!fs.existsSync(full)) return null;
    return fs.readFileSync(full, 'utf8');
}

function writeProjectFile(relativePath, content) {
    const full = path.join(PROJECT_ROOT, relativePath);
    // Backup first
    if (fs.existsSync(full)) {
        try { fs.copyFileSync(full, full + '.bak'); } catch {}
    }
    fs.writeFileSync(full, content, 'utf8');
}

// ── Window Management ──
function createWindow() {
    mainWindow = new BrowserWindow({
        width: 1280,
        height: 860,
        minWidth: 960,
        minHeight: 640,
        frame: false,
        backgroundColor: '#0f0f1a',
        icon: path.join(__dirname, 'assets', 'icon.png'),
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            contextIsolation: true,
            nodeIntegration: false,
            sandbox: false,
        },
        show: false,
    });

    mainWindow.loadFile(path.join(__dirname, 'renderer', 'index.html'));

    mainWindow.once('ready-to-show', () => {
        if (!settings.startMinimized) mainWindow.show();
    });

    mainWindow.on('close', (e) => {
        if (settings.minimizeToTray && tray) {
            e.preventDefault();
            mainWindow.hide();
        }
    });

    mainWindow.on('closed', () => { mainWindow = null; });
}

function createTray() {
    // Create a simple 16x16 icon using nativeImage (no external file needed)
    const icon = nativeImage.createFromBuffer(Buffer.alloc(0)).resize({ width: 16, height: 16 });
    try {
        const iconPath = path.join(__dirname, 'assets', 'icon.png');
        if (fs.existsSync(iconPath)) {
            tray = new Tray(iconPath);
        } else {
            // Create a minimal tray icon if no icon file exists
            tray = new Tray(nativeImage.createEmpty());
        }
    } catch {
        return; // Skip tray if icon creation fails
    }

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

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  APP LIFECYCLE
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━

app.disableHardwareAcceleration(); // Prevent GPU issues on some Windows machines

app.whenReady().then(() => {
    createWindow();
    createTray();
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit();
});

app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
});

app.on('before-quit', () => { app.isQuitting = true; });

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  IPC HANDLERS — All communication with renderer
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

// ── Window Controls ──
ipcMain.handle('window-minimize', () => mainWindow?.minimize());
ipcMain.handle('window-maximize', () => {
    if (mainWindow?.isMaximized()) mainWindow.unmaximize();
    else mainWindow?.maximize();
    return mainWindow?.isMaximized();
});
ipcMain.handle('window-close', () => mainWindow?.close());

// ── Overview (combined status) ──
ipcMain.handle('get-overview', async () => {
    try {
        // Try bot API first for accurate live data
        const overview = await botApi('/api/admin/overview');
        // Augment with system info
        overview.system = {
            platform: os.platform(),
            hostname: os.hostname(),
            totalMemory: os.totalmem(),
            freeMemory: os.freemem(),
            cpus: os.cpus().length,
            uptime: os.uptime(),
            nodeVersion: process.version,
        };
        return overview;
    } catch (err) {
        // Fallback: direct PM2 listing + file reads
        const services = await pm2ListParsed();
        let waStatus = { isReady: false, state: 'UNKNOWN', qrAvailable: false };
        try {
            if (fs.existsSync(STATUS_FILE)) {
                const s = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
                waStatus = { isReady: !!s.ready, state: s.ready ? 'CONNECTED' : 'DISCONNECTED', qrAvailable: !!s.qr };
            }
        } catch {}
        return {
            ok: false, botApiDown: true,
            services,
            whatsapp: waStatus,
            database: { connected: false, error: 'Bot API unreachable' },
            jobs: { tracked: {}, failingCount: 0, retryingCount: 0 },
            system: {
                platform: os.platform(), hostname: os.hostname(),
                totalMemory: os.totalmem(), freeMemory: os.freemem(),
                cpus: os.cpus().length, uptime: os.uptime(),
            },
        };
    }
});

// ── Services ──
ipcMain.handle('get-services', async () => pm2ListParsed());
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
    try {
        const result = await botApi('/api/admin/whatsapp/qr');
        return result;
    } catch {
        // Fallback: read from status file
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

// ── Jobs ──
ipcMain.handle('get-jobs', async () => {
    const result = { config: [], history: {} };
    try {
        if (fs.existsSync(JOBS_CONFIG_FILE)) {
            const cfg = JSON.parse(fs.readFileSync(JOBS_CONFIG_FILE, 'utf8'));
            result.config = cfg.jobs || [];
        }
    } catch {}
    try {
        if (fs.existsSync(JOB_LOG_FILE)) {
            result.history = JSON.parse(fs.readFileSync(JOB_LOG_FILE, 'utf8'));
        }
    } catch {}
    // Also get retry status from bot API
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
        const all = JSON.parse(fs.readFileSync(JOB_LOG_FILE, 'utf8'));
        return all[jobName] || [];
    } catch { return []; }
});

// ── Database ──
ipcMain.handle('get-db-status', async () => {
    try {
        const health = await botApi('/api/admin/health');
        return { connected: health.database === true, status: health.status };
    } catch (e) {
        return { connected: false, error: e.message };
    }
});

ipcMain.handle('trigger-backup', async () => {
    try { return await botApi('/api/admin/trigger/db-backup'); }
    catch (e) { return { ok: false, error: e.message }; }
});

ipcMain.handle('get-backup-files', async () => {
    try {
        if (!fs.existsSync(BACKUP_DIR)) return [];
        const files = fs.readdirSync(BACKUP_DIR)
            .filter(f => f.endsWith('.zip') || f.endsWith('.bak'))
            .map(f => {
                const stat = fs.statSync(path.join(BACKUP_DIR, f));
                return { name: f, size: stat.size, date: stat.mtime.toISOString() };
            })
            .sort((a, b) => new Date(b.date) - new Date(a.date));
        return files;
    } catch { return []; }
});

// ── Logs ──
ipcMain.handle('get-logs', async (_, opts = {}) => {
    const service = opts.service || 'whatsapp-bot-v2';
    const lines = opts.lines || 200;
    return new Promise((resolve) => {
        exec(`pm2 logs ${service} --nostream --lines ${lines} --raw`, {
            windowsHide: true, timeout: 10000, maxBuffer: 2 * 1024 * 1024
        }, (err, stdout, stderr) => {
            if (err) return resolve({ ok: false, logs: [`Error: ${err.message}`] });
            const all = ((stdout || '') + '\n' + (stderr || '')).split('\n').filter(Boolean).slice(-lines);
            resolve({ ok: true, logs: all });
        });
    });
});

// ── Configuration ──
ipcMain.handle('get-config-list', () => {
    return [
        { key: 'env', label: '.env', file: '.env' },
        { key: 'jobs-config', label: 'jobs-config.json', file: 'jobs-config.json' },
        { key: 'ecosystem', label: 'ecosystem.config.js', file: 'ecosystem.config.js' },
        { key: 'watched-items', label: 'watched-item-config.json', file: 'watched-item-config.json' },
        { key: 'jobs-status', label: 'job-status.json (read-only)', file: 'job-status.json' },
    ];
});

ipcMain.handle('get-config-file', async (_, name) => {
    const allowed = {
        'env': '.env',
        'jobs-config': 'jobs-config.json',
        'ecosystem': 'ecosystem.config.js',
        'watched-items': 'watched-item-config.json',
        'jobs-status': 'job-status.json',
    };
    const file = allowed[name];
    if (!file) return { ok: false, error: 'Unknown file' };
    const content = readProjectFile(file);
    return { ok: true, name: file, content: content || '' };
});

ipcMain.handle('save-config-file', async (_, { name, content }) => {
    const allowed = {
        'env': '.env',
        'jobs-config': 'jobs-config.json',
        'ecosystem': 'ecosystem.config.js',
        'watched-items': 'watched-item-config.json',
    };
    const file = allowed[name];
    if (!file) return { ok: false, error: 'Unknown or read-only file' };
    try {
        writeProjectFile(file, content);
        return { ok: true, message: `${file} saved (backup created)` };
    } catch (e) {
        return { ok: false, error: e.message };
    }
});

// ── System Info ──
ipcMain.handle('get-system-info', () => ({
    platform: os.platform(),
    hostname: os.hostname(),
    arch: os.arch(),
    release: os.release(),
    totalMemory: os.totalmem(),
    freeMemory: os.freemem(),
    cpus: os.cpus().length,
    cpuModel: os.cpus()[0]?.model || 'Unknown',
    uptime: os.uptime(),
    nodeVersion: process.version,
    electronVersion: process.versions.electron,
    projectRoot: PROJECT_ROOT,
}));

// ── Actions ──
ipcMain.handle('open-folder', async (_, folderPath) => {
    const full = path.isAbsolute(folderPath) ? folderPath : path.join(PROJECT_ROOT, folderPath);
    shell.openPath(full);
    return { ok: true };
});

ipcMain.handle('open-dashboard-web', () => {
    const port = getBotApiPort();
    shell.openExternal(`http://localhost:${port}/admin`);
    return { ok: true };
});

ipcMain.handle('restart-all-tunnels', async () => {
    const tunnels = ['lasantha-bot-tunnel', 'lasantha-app-tunnel', 'lasantha-fb-tunnel', 'royal-booking-tunnel'];
    const results = [];
    for (const t of tunnels) {
        results.push(await pm2Exec(`restart ${t}`));
    }
    return { ok: true, results };
});

ipcMain.handle('send-test-message', async (_, { number, message }) => {
    try {
        return await botApi('/api/titan/message', 'POST', { message, sender: number });
    } catch (e) {
        return { ok: false, error: e.message };
    }
});

// ── Settings ──
ipcMain.handle('get-settings', () => settings);
ipcMain.handle('save-settings', (_, newSettings) => {
    settings = { ...settings, ...newSettings };
    saveSettingsToDisk(settings);
    return { ok: true };
});
