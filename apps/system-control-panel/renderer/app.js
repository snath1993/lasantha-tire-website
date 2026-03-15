/**
 * Lasantha Tyre — System Control Panel
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * Renderer Application Logic
 */

// ── Global State ──
let refreshTimer = null;
let currentPage = 'overview';
let appSettings = {};

const App = {};

// ━━━━━━━━━━━━━━━━━━━━
//  NAVIGATION
// ━━━━━━━━━━━━━━━━━━━━
document.querySelectorAll('.nav-item').forEach(item => {
    item.addEventListener('click', () => {
        const page = item.dataset.page;
        if (!page) return;
        document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
        item.classList.add('active');
        document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
        const el = document.getElementById('page-' + page);
        if (el) el.classList.add('active');
        currentPage = page;
        onPageEnter(page);
    });
});

function onPageEnter(page) {
    if (page === 'overview') App.refreshOverview();
    if (page === 'whatsapp') { App.refreshWA(); App.refreshQR(); }
    if (page === 'services') App.refreshServices();
    if (page === 'jobs') App.refreshJobs();
    if (page === 'database') { App.refreshDbStatus(); App.refreshBackups(); }
    if (page === 'logs') App.refreshLogs();
    if (page === 'config') App.initConfigPage();
    if (page === 'settings') App.loadAppSettings();
}

// ━━━━━━━━━━━━━━━━━━━━
//  HELPERS
// ━━━━━━━━━━━━━━━━━━━━
function $(id) { return document.getElementById(id); }
function formatBytes(b) { if (!b) return '0 B'; if (b < 1024) return b + ' B'; if (b < 1048576) return (b / 1024).toFixed(0) + ' KB'; return (b / 1048576).toFixed(1) + ' MB'; }
function formatUptime(ms) {
    if (!ms || ms <= 0) return '—';
    const s = Math.floor(ms / 1000); const m = Math.floor(s / 60); const h = Math.floor(m / 60); const d = Math.floor(h / 24);
    if (d > 0) return d + 'd ' + (h % 24) + 'h';
    if (h > 0) return h + 'h ' + (m % 60) + 'm';
    if (m > 0) return m + 'm';
    return s + 's';
}
function formatTime(iso) {
    if (!iso) return '—';
    const d = new Date(iso);
    return d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit' }) + ' ' +
           d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short' });
}
function statusBadge(status) {
    const cls = { online: 'online', stopped: 'stopped', errored: 'error', launching: 'warning' }[status] || 'warning';
    return `<span class="status ${cls}">${status}</span>`;
}

function toast(msg, type = 'info') {
    const el = document.createElement('div');
    el.className = 'toast ' + type;
    el.textContent = msg;
    $('toast-container').appendChild(el);
    setTimeout(() => { el.style.opacity = '0'; setTimeout(() => el.remove(), 300); }, 4000);
}

// ━━━━━━━━━━━━━━━━━━━━
//  OVERVIEW
// ━━━━━━━━━━━━━━━━━━━━
App.refreshOverview = async () => {
    try {
        const data = await api.getOverview();
        // Health badge
        const health = $('ov-health');
        if (data.botApiDown) {
            health.className = 'badge status error'; health.textContent = 'BOT API DOWN';
        } else if (data.whatsapp?.isReady && data.database?.connected) {
            health.className = 'badge status healthy'; health.textContent = 'HEALTHY';
        } else {
            health.className = 'badge status warning'; health.textContent = 'DEGRADED';
        }

        // WhatsApp card
        $('ov-wa').innerHTML = data.whatsapp?.isReady
            ? '<span style="color:var(--success)">Connected</span>'
            : '<span style="color:var(--error)">Offline</span>';
        $('ov-wa-sub').textContent = data.whatsapp?.engine ? 'Engine: ' + data.whatsapp.engine : '';

        // Database card
        $('ov-db').innerHTML = data.database?.connected
            ? '<span style="color:var(--success)">Connected</span>'
            : '<span style="color:var(--error)">Error</span>';
        $('ov-db-sub').textContent = data.database?.name || data.database?.error || '';

        // Services
        const svcs = data.services || [];
        const onlineCount = svcs.filter(s => s.status === 'online').length;
        $('ov-svc').textContent = `${onlineCount}/${svcs.length}`;
        $('ov-svc-sub').textContent = onlineCount === svcs.length ? 'All online' : (svcs.length - onlineCount) + ' stopped';

        // Jobs
        const jobData = data.jobs || {};
        $('ov-jobs').textContent = jobData.failingCount > 0 ? `${jobData.failingCount} failing` : '0 issues';
        $('ov-jobs').style.color = jobData.failingCount > 0 ? 'var(--error)' : 'var(--success)';
        $('ov-jobs-sub').textContent = jobData.retryingCount > 0 ? `${jobData.retryingCount} retrying` : 'All healthy';

        // Bot uptime
        const bot = svcs.find(s => s.name === 'whatsapp-bot-v2');
        $('ov-uptime').textContent = bot ? formatUptime(bot.uptimeMs) : '—';
        $('ov-uptime-sub').textContent = bot ? `PID: ${bot.pid}` : '';

        // Memory
        $('ov-mem').textContent = bot ? formatBytes(bot.memory) : '—';
        if (data.system) {
            const usedPct = ((1 - data.system.freeMemory / data.system.totalMemory) * 100).toFixed(0);
            $('ov-mem-sub').textContent = `System: ${usedPct}% used`;
        }

        // Restarts
        const totalRestarts = svcs.reduce((s, p) => s + (p.restarts || 0), 0);
        $('ov-restarts').textContent = totalRestarts;
        $('ov-restarts-sub').textContent = bot ? `Bot: ${bot.restarts}` : '';

        // System
        if (data.system) {
            $('ov-sys').textContent = data.system.hostname || '—';
            $('ov-sys-sub').textContent = `${data.system.cpus} CPU · ${formatBytes(data.system.totalMemory)} RAM`;
        }

        // Job execution table
        renderJobTable(jobData.tracked);

        // Status bar
        updateStatusBar(data);
    } catch (e) {
        $('ov-health').className = 'badge status error';
        $('ov-health').textContent = 'ERROR';
    }
};

function renderJobTable(tracked) {
    const tbody = $('ov-job-table');
    if (!tracked || !Object.keys(tracked).length) {
        tbody.innerHTML = '<tr><td colspan="4" style="color:var(--text-muted)">No job data available</td></tr>';
        return;
    }
    let rows = '';
    for (const [name, info] of Object.entries(tracked)) {
        const last = info.lastExecution;
        const statusCls = last?.status === 'success' ? 'success' : (last?.status === 'failed' ? 'failed' : 'warning');
        rows += `<tr>
            <td>${name}</td>
            <td>${formatTime(last?.timestamp)}</td>
            <td>${statusBadge(last?.status || 'unknown')}</td>
            <td>${last?.durationMs ? (last.durationMs / 1000).toFixed(1) + 's' : (last?.duration ? (last.duration / 1000).toFixed(1) + 's' : '—')}</td>
        </tr>`;
    }
    tbody.innerHTML = rows;
}

// ━━━━━━━━━━━━━━━━━━━━
//  WHATSAPP
// ━━━━━━━━━━━━━━━━━━━━
App.refreshWA = async () => {
    try {
        const wa = await api.getWhatsAppStatus();
        const el = $('wa-status');
        if (wa.isReady) { el.className = 'status connected'; el.textContent = 'Connected'; }
        else if (wa.state === 'INITIALIZING') { el.className = 'status warning'; el.textContent = 'Initializing'; }
        else { el.className = 'status stopped'; el.textContent = 'Disconnected'; }
        $('wa-engine').textContent = (wa.engine || 'v1').toUpperCase();
        $('wa-reconnects').textContent = wa.reconnectAttempts || 0;
    } catch {}
};

App.refreshQR = async () => {
    try {
        const qr = await api.getQRCode();
        const img = $('qr-image');
        const placeholder = $('qr-placeholder');
        const label = $('qr-label');
        if (qr.available && qr.dataUrl) {
            img.src = qr.dataUrl;
            img.style.display = 'block';
            placeholder.style.display = 'none';
            label.textContent = '📱 Scan this QR code with WhatsApp on your phone';
            label.style.color = 'var(--warning)';
        } else if (qr.available && qr.qr) {
            // QR string available but no data URL
            placeholder.textContent = 'QR available — open web dashboard to scan';
            placeholder.style.display = 'flex';
            img.style.display = 'none';
            label.textContent = '⚠️ QR code needs scanning — use web dashboard';
            label.style.color = 'var(--warning)';
        } else {
            img.style.display = 'none';
            placeholder.textContent = 'No QR code needed';
            placeholder.style.display = 'flex';
            label.textContent = '✅ WhatsApp is connected — no QR needed';
            label.style.color = 'var(--success)';
        }
    } catch {}
};

App.reconnectWA = async () => {
    toast('Reconnecting WhatsApp...', 'info');
    const res = await api.reconnectWhatsApp();
    toast(res.ok ? 'Reconnect triggered' : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
};

App.hardResetWA = async () => {
    if (!confirm('This will stop the bot, clear the session, and restart. Continue?')) return;
    toast('Hard resetting WhatsApp...', 'info');
    await api.serviceAction('whatsapp-bot-v2', 'restart');
    toast('Bot restarted — check QR in a few seconds', 'success');
    setTimeout(() => App.refreshQR(), 8000);
};

App.sendTestMsg = async () => {
    const num = $('wa-test-number').value.trim();
    const msg = $('wa-test-msg').value.trim();
    if (!num || !msg) return toast('Enter number and message', 'error');
    toast('Sending...', 'info');
    const res = await api.sendTestMessage(num, msg);
    toast(res.error ? `Failed: ${res.error}` : 'Message sent', res.error ? 'error' : 'success');
};

// ━━━━━━━━━━━━━━━━━━━━
//  SERVICES
// ━━━━━━━━━━━━━━━━━━━━
App.refreshServices = async () => {
    try {
        const svcs = await api.getServices();
        const tbody = $('svc-table');
        let rows = '';
        for (const s of svcs) {
            rows += `<tr>
                <td><strong>${s.name}</strong></td>
                <td>${statusBadge(s.status)}</td>
                <td>${s.cpu}%</td>
                <td>${formatBytes(s.memory)}</td>
                <td>${s.restarts}</td>
                <td>${formatUptime(s.uptimeMs)}</td>
                <td class="mono text-muted">${s.pid || '—'}</td>
                <td>
                    <button class="btn btn-ghost btn-sm" onclick="App.svcAction('${s.name}','restart')">🔄</button>
                    <button class="btn btn-ghost btn-sm" onclick="App.svcAction('${s.name}','${s.status === 'online' ? 'stop' : 'start'}')">${s.status === 'online' ? '⏸' : '▶'}</button>
                </td>
            </tr>`;
        }
        tbody.innerHTML = rows || '<tr><td colspan="8" class="text-muted">No services found</td></tr>';
    } catch (e) {
        toast('Failed to load services', 'error');
    }
};

App.svcAction = async (name, action) => {
    toast(`${action === 'restart' ? 'Restarting' : action === 'stop' ? 'Stopping' : 'Starting'} ${name}...`, 'info');
    const res = await api.serviceAction(name, action);
    toast(res.ok ? `${name} ${action}ed` : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
    setTimeout(() => App.refreshServices(), 2000);
};

App.serviceActionAll = async (action) => {
    if (!confirm(`Are you sure you want to ${action} ALL services?`)) return;
    const svcs = await api.getServices();
    for (const s of svcs) {
        if (s.name !== 'whatsapp-bot' /* skip old stopped one */) {
            await api.serviceAction(s.name, action);
        }
    }
    toast(`All services ${action}ed`, 'success');
    setTimeout(() => App.refreshServices(), 3000);
};

// ━━━━━━━━━━━━━━━━━━━━
//  JOBS
// ━━━━━━━━━━━━━━━━━━━━
App.refreshJobs = async () => {
    try {
        const data = await api.getJobs();

        // Status table (tracked jobs with execution history)
        const tracked = data.tracked || {};
        const history = data.history || {};
        const tbody = $('jobs-table');
        let allJobs = { ...tracked };
        // Also include jobs from history
        for (const name of Object.keys(history)) {
            if (!allJobs[name]) {
                const entries = history[name];
                const last = entries[entries.length - 1];
                allJobs[name] = { lastExecution: last, consecutiveFailures: 0, retryPending: false };
            }
        }
        let rows = '';
        for (const [name, info] of Object.entries(allJobs)) {
            const last = info.lastExecution;
            rows += `<tr>
                <td>${name}</td>
                <td>${formatTime(last?.timestamp)}</td>
                <td>${statusBadge(last?.status || 'unknown')}</td>
                <td>${last?.durationMs ? (last.durationMs / 1000).toFixed(1) + 's' : '—'}</td>
                <td style="color:${info.consecutiveFailures > 0 ? 'var(--error)' : 'var(--success)'}">${info.consecutiveFailures || 0}</td>
            </tr>`;
        }
        tbody.innerHTML = rows || '<tr><td colspan="5" class="text-muted">No job data</td></tr>';

        // Config table
        const cfgTbody = $('jobs-config-table');
        const cfgJobs = data.config || [];
        let cfgRows = '';
        for (const j of cfgJobs) {
            cfgRows += `<tr>
                <td>${j.id || j.name || '—'}</td>
                <td class="mono">${j.schedule || j.cron || '—'}</td>
                <td>${j.enabled !== false ? '<span style="color:var(--success)">✅</span>' : '<span style="color:var(--error)">❌</span>'}</td>
            </tr>`;
        }
        cfgTbody.innerHTML = cfgRows || '<tr><td colspan="3" class="text-muted">No job configuration</td></tr>';
    } catch (e) {
        toast('Failed to load jobs', 'error');
    }
};

App.triggerJob = async (jobName) => {
    toast(`Triggering ${jobName}...`, 'info');
    const res = await api.triggerJob(jobName);
    toast(res.ok || res.message ? `${jobName} triggered` : `Failed: ${res.error || 'Unknown'}`, res.ok || res.message ? 'success' : 'error');
};

// ━━━━━━━━━━━━━━━━━━━━
//  DATABASE
// ━━━━━━━━━━━━━━━━━━━━
App.refreshDbStatus = async () => {
    try {
        const db = await api.getDbStatus();
        const el = $('db-status');
        if (db.connected) { el.className = 'status connected'; el.textContent = 'Connected'; }
        else { el.className = 'status error'; el.textContent = 'Error'; }
        $('db-name').textContent = db.name || '—';
    } catch {}
};

App.refreshBackups = async () => {
    try {
        const files = await api.getBackupFiles();
        const tbody = $('backup-table');
        let rows = '';
        for (const f of files) {
            rows += `<tr>
                <td class="mono">${f.name}</td>
                <td>${formatBytes(f.size)}</td>
                <td>${formatTime(f.date)}</td>
            </tr>`;
        }
        tbody.innerHTML = rows || '<tr><td colspan="3" class="text-muted">No backup files found</td></tr>';
    } catch {}
};

App.triggerBackup = async () => {
    toast('Triggering database backup...', 'info');
    const res = await api.triggerBackup();
    toast(res.ok ? 'Backup triggered' : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
};

App.openFolder = (p) => api.openFolder(p);

// ━━━━━━━━━━━━━━━━━━━━
//  LOGS
// ━━━━━━━━━━━━━━━━━━━━
App.refreshLogs = async () => {
    try {
        const service = $('log-service').value;
        const lines = parseInt($('log-lines').value) || 200;
        const search = $('log-search').value.toLowerCase();
        const data = await api.getLogs({ service, lines });
        const output = $('log-output');
        let logs = data.logs || [];
        if (search) logs = logs.filter(l => l.toLowerCase().includes(search));
        output.innerHTML = logs.map(colorLogLine).join('\n');
        output.scrollTop = output.scrollHeight;
    } catch (e) {
        $('log-output').textContent = 'Error loading logs: ' + e.message;
    }
};

function colorLogLine(line) {
    const escaped = line.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    if (/error|ERR|❌|💥|CRITICAL/i.test(line)) return `<span class="log-line-err">${escaped}</span>`;
    if (/warn|⚠️/i.test(line)) return `<span class="log-line-warn">${escaped}</span>`;
    if (/✅|success|connected|ready/i.test(line)) return `<span class="log-line-ok">${escaped}</span>`;
    if (/\[.*\]|info|📊|📡|🌐/i.test(line)) return `<span class="log-line-info">${escaped}</span>`;
    return escaped;
}

App.copyLogs = () => {
    const text = $('log-output').innerText;
    navigator.clipboard.writeText(text).then(() => toast('Logs copied', 'success'));
};

// ━━━━━━━━━━━━━━━━━━━━
//  CONFIGURATION
// ━━━━━━━━━━━━━━━━━━━━
App.initConfigPage = async () => {
    try {
        const select = $('config-select');
        if (select.children.length === 0) {
            const list = await api.getConfigList();
            for (const item of list) {
                const opt = document.createElement('option');
                opt.value = item.key;
                opt.textContent = item.label;
                select.appendChild(opt);
            }
        }
        App.loadConfigFile();
    } catch {}
};

App.loadConfigFile = async () => {
    const key = $('config-select').value;
    if (!key) return;
    try {
        const data = await api.getConfigFile(key);
        $('config-editor').value = data.content || '';
    } catch (e) {
        $('config-editor').value = 'Error loading file: ' + e.message;
    }
};

App.saveConfig = async () => {
    const key = $('config-select').value;
    const content = $('config-editor').value;
    toast('Saving...', 'info');
    const res = await api.saveConfigFile(key, content);
    toast(res.ok ? res.message || 'Saved' : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
};

App.saveConfigAndRestart = async () => {
    await App.saveConfig();
    setTimeout(async () => {
        toast('Restarting bot...', 'info');
        await api.serviceAction('whatsapp-bot-v2', 'restart');
        toast('Bot restarted', 'success');
    }, 1000);
};

// ━━━━━━━━━━━━━━━━━━━━
//  ACTIONS
// ━━━━━━━━━━━━━━━━━━━━
App.restartAllTunnels = async () => {
    toast('Restarting all tunnels...', 'info');
    const res = await api.restartAllTunnels();
    toast(res.ok ? 'All tunnels restarted' : 'Failed', res.ok ? 'success' : 'error');
};

// ━━━━━━━━━━━━━━━━━━━━
//  SETTINGS
// ━━━━━━━━━━━━━━━━━━━━
App.loadAppSettings = async () => {
    try {
        appSettings = await api.getSettings();
        $('set-port').value = appSettings.botApiPort || 8585;
        $('set-key').value = appSettings.dashboardKey || '';
        $('set-refresh').value = String(appSettings.refreshInterval || 5000);
        $('set-tray').checked = appSettings.minimizeToTray !== false;
        $('set-startmin').checked = !!appSettings.startMinimized;

        // About info
        const sys = await api.getSystemInfo();
        $('about-info').innerHTML = `
            <div>App Version: <strong>1.0.0</strong></div>
            <div>Electron: <strong>${sys.electronVersion || '—'}</strong></div>
            <div>Node.js: <strong>${sys.nodeVersion || '—'}</strong></div>
            <div>Platform: <strong>${sys.platform} ${sys.arch}</strong></div>
            <div>Hostname: <strong>${sys.hostname}</strong></div>
            <div>CPU: <strong>${sys.cpuModel}</strong></div>
            <div>RAM: <strong>${formatBytes(sys.totalMemory)}</strong></div>
            <div>Project: <strong>${sys.projectRoot || '—'}</strong></div>
        `;
    } catch {}
};

App.saveAppSettings = async () => {
    const newSettings = {
        botApiPort: parseInt($('set-port').value) || 8585,
        dashboardKey: $('set-key').value.trim(),
        refreshInterval: parseInt($('set-refresh').value) || 5000,
        minimizeToTray: $('set-tray').checked,
        startMinimized: $('set-startmin').checked,
    };
    const res = await api.saveSettings(newSettings);
    appSettings = newSettings;
    // Restart auto-refresh with new interval
    startAutoRefresh();
    toast(res.ok ? 'Settings saved' : 'Failed', res.ok ? 'success' : 'error');
};

// ━━━━━━━━━━━━━━━━━━━━
//  STATUS BAR
// ━━━━━━━━━━━━━━━━━━━━
function updateStatusBar(data) {
    const waDot = $('sb-wa-dot');
    const dbDot = $('sb-db-dot');
    const botDot = $('sb-bot-dot');
    const svcCount = $('sb-svc-count');

    if (data.whatsapp?.isReady) { waDot.className = 'sb-dot on'; }
    else { waDot.className = 'sb-dot off'; }

    if (data.database?.connected) { dbDot.className = 'sb-dot on'; }
    else { dbDot.className = 'sb-dot off'; }

    const svcs = data.services || [];
    const bot = svcs.find(s => s.name === 'whatsapp-bot-v2');
    if (bot?.status === 'online') { botDot.className = 'sb-dot on'; }
    else { botDot.className = 'sb-dot off'; }

    const onlineCount = svcs.filter(s => s.status === 'online').length;
    svcCount.textContent = `Services: ${onlineCount}/${svcs.length}`;

    // Clock
    $('sb-time').textContent = new Date().toLocaleTimeString('en-GB');
}

// Clock ticker
setInterval(() => { $('sb-time').textContent = new Date().toLocaleTimeString('en-GB'); }, 1000);

// ━━━━━━━━━━━━━━━━━━━━
//  AUTO REFRESH
// ━━━━━━━━━━━━━━━━━━━━
function startAutoRefresh() {
    if (refreshTimer) clearInterval(refreshTimer);
    const interval = appSettings.refreshInterval || 5000;
    refreshTimer = setInterval(() => {
        // Only auto-refresh the active page
        if (currentPage === 'overview') App.refreshOverview();
        else if (currentPage === 'services') App.refreshServices();
        else if (currentPage === 'whatsapp') { App.refreshWA(); App.refreshQR(); }
    }, interval);
}

// ━━━━━━━━━━━━━━━━━━━━
//  INIT
// ━━━━━━━━━━━━━━━━━━━━
(async function init() {
    try {
        appSettings = await api.getSettings();
    } catch {}
    App.refreshOverview();
    startAutoRefresh();
})();
