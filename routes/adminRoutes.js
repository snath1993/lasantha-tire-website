/**
 * Admin Dashboard API Routes
 * ━━━━━━━━━━━━━━━━━━━━━━━━━
 * Provides REST endpoints for the System Control Panel dashboard.
 * Mount via: require('./routes/adminRoutes')(app, { mainPool, ... })
 */

const { exec } = require('child_process');
const path = require('path');
const fs = require('fs');

module.exports = function mountAdminRoutes(app, deps = {}) {
    const { mainPool } = deps;

    // ── Auth middleware (optional dashboard key) ──
    function authCheck(req, res, next) {
        const dashKey = (process.env.DASHBOARD_KEY || '').trim();
        if (!dashKey) return next(); // no key configured = open access
        const provided = req.query.key || req.headers['x-dashboard-key'] || '';
        if (provided === dashKey) return next();
        return res.status(401).json({ error: 'Unauthorized — provide ?key=YOUR_DASHBOARD_KEY' });
    }

    // ── Serve dashboard static files ──
    const express = require('express');
    const adminDir = path.join(__dirname, '..', 'public', 'admin');
    app.use('/admin', authCheck, express.static(adminDir));

    // ═══════════════════════════════════
    //  GET /api/admin/overview
    //  Combined status for the dashboard
    // ═══════════════════════════════════
    app.get('/api/admin/overview', authCheck, async (req, res) => {
        try {
            const [services, whatsapp, database, jobs] = await Promise.all([
                getServiceStatus(),
                getWhatsAppStatus(),
                getDatabaseStatus(mainPool),
                getJobStatus()
            ]);
            res.json({ ok: true, services, whatsapp, database, jobs, serverTime: new Date().toISOString() });
        } catch (err) {
            res.json({ ok: false, error: err.message });
        }
    });

    // ═══════════════════════════════════
    //  GET /api/admin/health
    //  Simple health check endpoint
    // ═══════════════════════════════════
    app.get('/api/admin/health', authCheck, async (req, res) => {
        const [db, wa] = await Promise.all([
            getDatabaseStatus(mainPool),
            getWhatsAppStatus()
        ]);
        const healthy = db.connected && wa.isReady;
        res.status(healthy ? 200 : 503).json({
            status: healthy ? 'healthy' : 'degraded',
            database: db.connected,
            whatsapp: wa.isReady,
            timestamp: new Date().toISOString()
        });
    });

    // ═══════════════════════════════════
    //  GET /api/admin/logs
    //  Recent PM2 logs
    // ═══════════════════════════════════
    app.get('/api/admin/logs', authCheck, async (req, res) => {
        const lines = parseInt(req.query.lines || '100', 10);
        try {
            const logs = await getPm2Logs(lines);
            res.json({ ok: true, logs });
        } catch (err) {
            res.json({ ok: true, logs: [`Error reading logs: ${err.message}`] });
        }
    });

    // ═══════════════════════════════════
    //  POST /api/admin/services/:name/restart
    //  POST /api/admin/services/:name/stop
    //  POST /api/admin/services/:name/start
    // ═══════════════════════════════════
    app.post('/api/admin/services/:name/restart', authCheck, (req, res) => {
        pm2Command('restart', req.params.name, res);
    });
    app.post('/api/admin/services/:name/stop', authCheck, (req, res) => {
        pm2Command('stop', req.params.name, res);
    });
    app.post('/api/admin/services/:name/start', authCheck, (req, res) => {
        pm2Command('start', req.params.name, res);
    });

    // ═══════════════════════════════════
    //  POST /api/admin/whatsapp/reconnect
    // ═══════════════════════════════════
    app.post('/api/admin/whatsapp/reconnect', authCheck, async (req, res) => {
        try {
            // Access global reconnection handler
            if (typeof global.handleReconnection === 'function') {
                global.handleReconnection();
                res.json({ ok: true, message: 'Reconnect triggered' });
            } else if (typeof global.initializeClient === 'function') {
                global.initializeClient(true);
                res.json({ ok: true, message: 'Re-initialization triggered' });
            } else {
                res.json({ ok: false, error: 'Reconnect handler not available' });
            }
        } catch (err) {
            res.json({ ok: false, error: err.message });
        }
    });

    // ═══════════════════════════════════
    //  GET /api/admin/trigger/:job
    //  Manually trigger specific jobs
    // ═══════════════════════════════════
    app.get('/api/admin/trigger/:job', authCheck, async (req, res) => {
        const job = req.params.job;
        try {
            if (job === 'daily-sales-pdf') {
                // Delegate to existing trigger endpoint
                const portFile = path.join(__dirname, '..', 'runtime-port.json');
                res.redirect(`/api/jobs/daily-sales-pdf/trigger?key=${req.query.key || ''}`);
                return;
            }
            if (job === 'db-backup') {
                if (typeof global.triggerDatabaseBackup === 'function') {
                    global.triggerDatabaseBackup();
                    return res.json({ ok: true, message: 'Database backup triggered' });
                }
                return res.json({ ok: false, error: 'Backup function not available' });
            }
            if (job === 'daily-report') {
                if (typeof global.triggerDailyReport === 'function') {
                    global.triggerDailyReport();
                    return res.json({ ok: true, message: 'Daily report triggered' });
                }
                return res.json({ ok: false, error: 'Report function not available' });
            }
            res.json({ ok: false, error: `Unknown job: ${job}` });
        } catch (err) {
            res.json({ ok: false, error: err.message });
        }
    });

    // ═══════════════════════════════════
    //  GET /api/admin/whatsapp/qr
    //  Returns current QR code for scanning
    // ═══════════════════════════════════
    app.get('/api/admin/whatsapp/qr', authCheck, (req, res) => {
        const qr = global.lastQR || null;
        const dataUrl = global.currentQRCodeDataUrl || null;
        if (!qr) return res.json({ available: false });
        res.json({ available: true, qr, dataUrl });
    });

    // ═══════════════════════════════════
    //  GET /api/admin/config/:file
    //  Read configuration files
    // ═══════════════════════════════════
    app.get('/api/admin/config/:file', authCheck, (req, res) => {
        const allowed = {
            'env': '.env',
            'jobs-config': 'jobs-config.json',
            'ecosystem': 'ecosystem.config.js',
            'watched-items': 'watched-item-config.json',
        };
        const file = allowed[req.params.file];
        if (!file) return res.json({ ok: false, error: 'Unknown config file' });
        const filePath = path.join(__dirname, '..', file);
        try {
            const content = fs.existsSync(filePath) ? fs.readFileSync(filePath, 'utf8') : '';
            res.json({ ok: true, name: file, content });
        } catch (err) {
            res.json({ ok: false, error: err.message });
        }
    });

    // ═══════════════════════════════════
    //  POST /api/admin/config/:file
    //  Write configuration files
    // ═══════════════════════════════════
    app.post('/api/admin/config/:file', authCheck, express.json(), (req, res) => {
        const allowed = {
            'env': '.env',
            'jobs-config': 'jobs-config.json',
            'ecosystem': 'ecosystem.config.js',
            'watched-items': 'watched-item-config.json',
        };
        const file = allowed[req.params.file];
        if (!file) return res.json({ ok: false, error: 'Unknown config file' });
        const filePath = path.join(__dirname, '..', file);
        try {
            // Create a backup first
            if (fs.existsSync(filePath)) {
                const backupPath = filePath + '.bak';
                fs.copyFileSync(filePath, backupPath);
            }
            fs.writeFileSync(filePath, req.body.content || '', 'utf8');
            res.json({ ok: true, message: `${file} saved` });
        } catch (err) {
            res.json({ ok: false, error: err.message });
        }
    });

    console.log('[AdminRoutes] ✅ Admin dashboard routes mounted at /admin');
};

// ═══════════════════════════════════════
//  HELPER FUNCTIONS
// ═══════════════════════════════════════

/**
 * Get PM2 process list
 */
function getServiceStatus() {
    return new Promise((resolve) => {
        exec('pm2 jlist', { windowsHide: true, timeout: 10000 }, (err, stdout) => {
            if (err) return resolve([]);
            try {
                // PM2 jlist sometimes has duplicate keys — custom parse
                const raw = stdout.trim();
                if (!raw || !raw.startsWith('[')) return resolve([]);
                // Simple approach: use regex to extract objects
                const processes = [];
                const pm2Data = JSON.parse(raw.replace(/"USERNAME":"[^"]*",/gi, '').replace(/"username":"[^"]*",/gi, ''));
                for (const p of pm2Data) {
                    processes.push({
                        name: p.name,
                        pmId: p.pm_id,
                        status: p.pm2_env?.status || 'unknown',
                        cpu: p.monit?.cpu ?? null,
                        memory: p.monit?.memory ?? null,
                        restarts: p.pm2_env?.restart_time ?? 0,
                        uptimeMs: p.pm2_env?.pm_uptime ? Date.now() - p.pm2_env.pm_uptime : 0,
                        pid: p.pid,
                    });
                }
                resolve(processes);
            } catch (e) {
                // Fallback: parse pm2 list text output
                resolve([]);
            }
        });
    });
}

/**
 * Get WhatsApp connection status
 */
function getWhatsAppStatus() {
    return new Promise((resolve) => {
        try {
            const client = global.whatsappClient;
            const isReady = !!(client && (client.isReady || (client.info && client.info.wid)));
            const state = isReady ? 'CONNECTED' : (global.isInitializing ? 'INITIALIZING' : 'DISCONNECTED');

            // Check if QR is available
            const qrAvailable = !!(global.lastQR && !isReady);

            resolve({
                isReady,
                state,
                qrAvailable,
                engine: process.env.WHATSAPP_ENGINE || 'v1',
                reconnectAttempts: global.reconnectAttempts || 0,
            });
        } catch {
            resolve({ isReady: false, state: 'ERROR', qrAvailable: false, engine: 'v1' });
        }
    });
}

/**
 * Get database connection status
 */
async function getDatabaseStatus(pool) {
    try {
        if (!pool) return { connected: false, error: 'No pool available' };
        // Test with simple query
        const result = await pool.request().query('SELECT 1 AS ok');
        return {
            connected: true,
            name: pool.config?.database || process.env.SQL_DATABASE || 'Unknown',
        };
    } catch (err) {
        return { connected: false, error: err.message?.substring(0, 100) || 'Connection failed' };
    }
}

/**
 * Get job retry status
 */
function getJobStatus() {
    try {
        const { getFailureSummary } = require('../utils/JobRetryManager');
        const summary = getFailureSummary();
        // Load execution log for last execution details
        const logPath = path.join(__dirname, '..', 'job-execution-log.json');
        let tracked = {};
        if (fs.existsSync(logPath)) {
            const raw = JSON.parse(fs.readFileSync(logPath, 'utf8'));
            for (const [name, entries] of Object.entries(raw)) {
                const last = entries[entries.length - 1];
                tracked[name] = {
                    lastExecution: last || null,
                    consecutiveFailures: summary[name]?.consecutiveFailures || 0,
                    retryPending: summary[name]?.retryPending || false,
                };
            }
        }
        // Add jobs from summary that may not be in log yet
        for (const [name, s] of Object.entries(summary)) {
            if (!tracked[name]) {
                tracked[name] = {
                    lastExecution: null,
                    consecutiveFailures: s.consecutiveFailures || 0,
                    retryPending: s.retryPending || false,
                };
            }
        }
        const failingCount = Object.values(tracked).filter(t => t.consecutiveFailures > 0).length;
        const retryingCount = Object.values(tracked).filter(t => t.retryPending).length;
        return { tracked, failingCount, retryingCount };
    } catch {
        return { tracked: {}, failingCount: 0, retryingCount: 0 };
    }
}

/**
 * Get PM2 logs
 */
function getPm2Logs(lines = 100) {
    return new Promise((resolve) => {
        exec(`pm2 logs whatsapp-bot-v2 --nostream --lines ${lines} --raw`, {
            windowsHide: true,
            timeout: 10000,
            maxBuffer: 1024 * 1024
        }, (err, stdout) => {
            if (err) return resolve([`Error: ${err.message}`]);
            const all = (stdout || '').split('\n').filter(Boolean).slice(-lines);
            resolve(all);
        });
    });
}

/**
 * Execute PM2 command
 */
function pm2Command(action, name, res) {
    // Validate service name (prevent injection)
    if (!/^[a-zA-Z0-9_.-]+$/.test(name)) {
        return res.json({ ok: false, error: 'Invalid service name' });
    }
    exec(`pm2 ${action} ${name}`, { windowsHide: true, timeout: 15000 }, (err, stdout) => {
        if (err) return res.json({ ok: false, error: err.message });
        res.json({ ok: true, output: stdout?.trim()?.substring(0, 500) });
    });
}
