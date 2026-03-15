/**
 * Lasantha Tyre — System Control Panel v2.0
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
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
        navigateToPage(page);
    });
});

function navigateToPage(page) {
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    const navItem = document.querySelector(`.nav-item[data-page="${page}"]`);
    if (navItem) navItem.classList.add('active');
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    const el = document.getElementById('page-' + page);
    if (el) el.classList.add('active');
    currentPage = page;
    onPageEnter(page);
}

App.navigateTo = navigateToPage;

function onPageEnter(page) {
    const handlers = {
        overview: () => App.refreshOverview(),
        whatsapp: () => { App.refreshWA(); App.refreshQR(); },
        botswap: () => App.refreshBotSwap(),
        services: () => App.refreshServices(),
        jobs: () => App.refreshJobs(),
        database: () => { App.refreshDbStatus(); App.refreshBackups(); },
        logs: () => App.refreshLogs(),
        config: () => App.initConfigPage(),
        updates: () => App.checkLibraryVersions(),
        settings: () => App.loadAppSettings(),
    };
    if (handlers[page]) handlers[page]();
}

// ━━━━━━━━━━━━━━━━━━━━
//  HELPERS
// ━━━━━━━━━━━━━━━━━━━━
function $(id) { return document.getElementById(id); }
function formatBytes(b) {
    if (!b) return '0 B';
    if (b < 1024) return b + ' B';
    if (b < 1048576) return (b / 1024).toFixed(0) + ' KB';
    if (b < 1073741824) return (b / 1048576).toFixed(1) + ' MB';
    return (b / 1073741824).toFixed(2) + ' GB';
}
function formatUptime(ms) {
    if (!ms || ms <= 0) return '—';
    const s = Math.floor(ms / 1000), m = Math.floor(s / 60), h = Math.floor(m / 60), d = Math.floor(h / 24);
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
    const cls = { online: 'online', stopped: 'stopped', errored: 'error', launching: 'warning', connected: 'connected', success: 'success', failed: 'failed' }[status] || 'warning';
    return `<span class="status ${cls}">${status}</span>`;
}

function toast(msg, type = 'info') {
    const icons = { success: '✅', error: '❌', info: 'ℹ️', warning: '⚠️' };
    const el = document.createElement('div');
    el.className = 'toast ' + type;
    el.innerHTML = `<span>${icons[type] || ''}</span><span>${msg}</span>`;
    $('toast-container').appendChild(el);
    setTimeout(() => { el.style.opacity = '0'; el.style.transition = 'opacity .3s'; setTimeout(() => el.remove(), 300); }, 4500);
}

// ━━━━━━━━━━━━━━━━━━━━
//  PROGRESS MODAL
// ━━━━━━━━━━━━━━━━━━━━
let progressDismissTimer = null;
function showProgress(title) {
    $('progress-title').textContent = title;
    $('progress-msg').textContent = 'Initializing...';
    $('progress-bar').style.width = '0%';
    $('progress-pct').textContent = '0%';
    $('progress-dismiss-btn').style.display = 'none';
    $('progress-overlay').classList.add('show');
    // Show dismiss button after 30s as safety net
    if (progressDismissTimer) clearTimeout(progressDismissTimer);
    progressDismissTimer = setTimeout(() => { $('progress-dismiss-btn').style.display = 'inline-flex'; }, 30000);
}
function hideProgress() {
    $('progress-overlay').classList.remove('show');
    if (progressDismissTimer) { clearTimeout(progressDismissTimer); progressDismissTimer = null; }
}
function updateProgress(data) {
    $('progress-msg').textContent = data.message || '';
    $('progress-bar').style.width = (data.progress || 0) + '%';
    $('progress-pct').textContent = (data.progress || 0) + '%';
    if ((data.progress || 0) >= 100) {
        $('progress-dismiss-btn').style.display = 'inline-flex';
    }
}

// Listen for progress events from main process
api.onUpdateProgress(updateProgress);

// ━━━━━━━━━━━━━━━━━━━━
//  OVERVIEW
// ━━━━━━━━━━━━━━━━━━━━
App.refreshOverview = async () => {
    try {
        const data = await api.getOverview();

        // Health badge
        const health = $('ov-health');
        if (data.botApiDown) { health.className = 'badge status error'; health.textContent = 'BOT API DOWN'; }
        else if (data.whatsapp?.isReady && data.database?.connected) { health.className = 'badge status healthy'; health.textContent = 'HEALTHY'; }
        else { health.className = 'badge status warning'; health.textContent = 'DEGRADED'; }

        // WhatsApp
        $('ov-wa').innerHTML = data.whatsapp?.isReady
            ? '<span style="color:var(--success)">Connected</span>'
            : '<span style="color:var(--error)">Offline</span>';
        $('ov-wa-sub').textContent = data.whatsapp?.engine ? 'Engine: ' + data.whatsapp.engine : '';

        // Database
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
        const bot = svcs.find(s => s.name === 'whatsapp-bot-v2') || svcs.find(s => s.name === 'whatsapp-bot' && s.status === 'online');
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

        // Disk space
        try {
            const drives = await api.getDiskSpace();
            if (drives && drives.length > 0) {
                const main = drives.find(d => d.Name === 'C') || drives[0];
                const pct = main.TotalGB > 0 ? ((main.UsedGB / main.TotalGB) * 100).toFixed(0) : 0;
                $('ov-disk').textContent = `${main.FreeGB} GB free`;
                $('ov-disk-sub').textContent = `${main.Name}: ${main.UsedGB}/${main.TotalGB} GB`;
                const bar = $('ov-disk-bar');
                bar.style.width = pct + '%';
                bar.style.background = pct > 90 ? 'var(--error)' : pct > 75 ? 'var(--warning)' : 'var(--gradient)';
            }
        } catch {}

        // Job execution table
        renderJobTable(jobData.tracked);
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
            img.src = qr.dataUrl; img.style.display = 'block'; placeholder.style.display = 'none';
            label.textContent = '📱 Scan this QR code with WhatsApp on your phone';
            label.style.color = 'var(--warning)';
        } else if (qr.available && qr.qr) {
            placeholder.textContent = 'QR available — open web dashboard to scan';
            placeholder.style.display = 'flex'; img.style.display = 'none';
            label.textContent = '⚠️ QR code needs scanning — use web dashboard';
            label.style.color = 'var(--warning)';
        } else {
            img.style.display = 'none'; placeholder.textContent = 'No QR code needed';
            placeholder.style.display = 'flex';
            label.textContent = '✅ WhatsApp is connected — no QR needed';
            label.style.color = 'var(--success)';
        }
    } catch {}
};

App.reconnectWA = async () => {
    toast('Reconnecting WhatsApp...', 'info');
    try {
        const res = await api.reconnectWhatsApp();
        toast(res.ok ? 'Reconnect triggered' : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
    } catch (e) { toast('Reconnect failed: ' + e.message, 'error'); }
};

App.hardResetWA = async () => {
    if (!confirm('⚠️ HARD RESET\n\nThis will:\n• Stop the bot\n• DELETE the WhatsApp session data\n• Restart the bot\n\nYou will need to scan a NEW QR code.\n\nContinue?')) return;
    toast('Hard resetting WhatsApp session...', 'warning');
    try {
        const res = await api.hardResetWhatsApp();
        toast(res.ok ? res.message : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
        if (res.ok) setTimeout(() => { App.refreshWA(); App.refreshQR(); }, 10000);
    } catch (e) { toast('Reset failed: ' + e.message, 'error'); }
};

App.sendTestMsg = async () => {
    const num = $('wa-test-number')?.value?.trim();
    const msg = $('wa-test-msg')?.value?.trim();
    if (!num || !msg) return toast('Enter phone number and message first', 'warning');
    toast('Sending test message...', 'info');
    try {
        const res = await api.sendTestMessage(num, msg);
        toast(res.error ? `Failed: ${res.error}` : 'Message sent!', res.error ? 'error' : 'success');
    } catch (e) { toast('Send failed: ' + e.message, 'error'); }
};

// ━━━━━━━━━━━━━━━━━━━━
//  BOT SWAP
// ━━━━━━━━━━━━━━━━━━━━
App.refreshBotSwap = async () => {
    try {
        const data = await api.getActiveBot();

        // V1 card
        const v1Card = $('swap-v1');
        const v1 = data.botV1;
        if (v1 && v1.status === 'online') {
            v1Card.classList.add('active-bot');
            $('swap-v1-status').innerHTML = statusBadge('online');
            $('swap-v1-icon').textContent = '🟢';
            $('swap-v1-info').textContent = `PID: ${v1.pid} | Uptime: ${formatUptime(v1.uptimeMs)} | Mem: ${formatBytes(v1.memory)}`;
        } else {
            v1Card.classList.remove('active-bot');
            $('swap-v1-status').innerHTML = statusBadge(v1?.status || 'stopped');
            $('swap-v1-icon').textContent = '🔴';
            $('swap-v1-info').textContent = v1 ? `Restarts: ${v1.restarts}` : 'Not registered in PM2';
        }

        // V2 card
        const v2Card = $('swap-v2');
        const v2 = data.botV2;
        if (v2 && v2.status === 'online') {
            v2Card.classList.add('active-bot');
            $('swap-v2-status').innerHTML = statusBadge('online');
            $('swap-v2-icon').textContent = '🟢';
            $('swap-v2-info').textContent = `PID: ${v2.pid} | Uptime: ${formatUptime(v2.uptimeMs)} | Mem: ${formatBytes(v2.memory)}`;
        } else {
            v2Card.classList.remove('active-bot');
            $('swap-v2-status').innerHTML = statusBadge(v2?.status || 'stopped');
            $('swap-v2-icon').textContent = '🔴';
            $('swap-v2-info').textContent = v2 ? `Restarts: ${v2.restarts}` : 'Not registered in PM2';
        }

        // Update swap button text
        const btn = $('swap-btn');
        if (data.active === 'whatsapp-bot-v2') {
            btn.textContent = '🔄 Swap to Legacy Bot (v1)';
        } else if (data.active === 'whatsapp-bot') {
            btn.textContent = '🔄 Swap to New Bot (v2)';
        } else {
            btn.textContent = '▶ Start Bot v2';
        }
    } catch (e) {
        toast('Failed to check bot status', 'error');
    }
};

App.performSwap = async () => {
    const data = await api.getActiveBot();
    const target = data.active === 'whatsapp-bot-v2' ? 'legacy bot (v1)' : 'new bot (v2)';
    if (!confirm(`Are you sure you want to swap to the ${target}?\n\nThe current bot will be stopped.`)) return;

    toast('Swapping bot versions...', 'warning');
    $('swap-btn').disabled = true;
    try {
        const res = await api.botSwap();
        toast(res.ok ? `✅ ${res.message}` : `❌ ${res.error}`, res.ok ? 'success' : 'error');
        setTimeout(() => App.refreshBotSwap(), 3000);
    } catch (e) {
        toast('Swap failed: ' + e.message, 'error');
    }
    $('swap-btn').disabled = false;
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
    } catch { toast('Failed to load services', 'error'); }
};

App.svcAction = async (name, action) => {
    const verb = { restart: 'Restarting', stop: 'Stopping', start: 'Starting' }[action] || action;
    toast(`${verb} ${name}...`, 'info');
    try {
        const res = await api.serviceAction(name, action);
        toast(res.ok ? `${name} — ${action} done` : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
        setTimeout(() => { if (currentPage === 'services') App.refreshServices(); }, 2000);
    } catch (e) { toast(`${action} failed: ${e.message}`, 'error'); }
};

App.serviceActionAll = async (action) => {
    if (!confirm(`Are you sure you want to ${action} ALL services?`)) return;
    toast(`${action}ing all services...`, 'warning');
    const svcs = await api.getServices();
    for (const s of svcs) await api.serviceAction(s.name, action);
    toast(`All services ${action}ed`, 'success');
    setTimeout(() => App.refreshServices(), 3000);
};

// ━━━━━━━━━━━━━━━━━━━━
//  JOBS
// ━━━━━━━━━━━━━━━━━━━━
App.refreshJobs = async () => {
    try {
        const data = await api.getJobs();

        // Status table (tracked + history)
        const tracked = data.tracked || {};
        const history = data.history || {};
        const allJobs = { ...tracked };
        for (const name of Object.keys(history)) {
            if (!allJobs[name]) {
                const entries = history[name];
                const last = entries[entries.length - 1];
                allJobs[name] = { lastExecution: last, consecutiveFailures: 0 };
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
        $('jobs-table').innerHTML = rows || '<tr><td colspan="5" class="text-muted">No job data</td></tr>';

        // Config table (FIXED: now receives properly parsed object data)
        const cfgJobs = data.config || [];
        let cfgRows = '';
        for (const j of cfgJobs) {
            cfgRows += `<tr>
                <td>${j.icon || '📋'} <strong>${j.name || j.id}</strong></td>
                <td class="text-sm" style="color:var(--text-secondary)">${j.description || '—'}</td>
                <td class="mono">${j.schedule || '—'}</td>
                <td>${j.enabled ? '<span style="color:var(--success)">✅ Enabled</span>' : '<span style="color:var(--error)">❌ Disabled</span>'}</td>
            </tr>`;
        }
        $('jobs-config-table').innerHTML = cfgRows || '<tr><td colspan="4" class="text-muted">No job configuration found</td></tr>';
    } catch { toast('Failed to load jobs', 'error'); }
};

App.triggerJob = async (jobName) => {
    toast(`Triggering ${jobName}...`, 'info');
    try {
        const res = await api.triggerJob(jobName);
        toast(res.ok || res.message ? `${jobName} triggered` : `Failed: ${res.error || 'Unknown'}`, res.ok || res.message ? 'success' : 'error');
    } catch (e) { toast(`Trigger failed: ${e.message}`, 'error'); }
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
        let rows = '';
        for (const f of files) {
            rows += `<tr>
                <td class="mono">${f.name}</td>
                <td>${formatBytes(f.size)}</td>
                <td>${formatTime(f.date)}</td>
            </tr>`;
        }
        $('backup-table').innerHTML = rows || '<tr><td colspan="3" class="text-muted">No backup files found</td></tr>';
    } catch {}
};

App.triggerBackup = async () => {
    toast('Triggering database backup...', 'info');
    try {
        const res = await api.triggerBackup();
        toast(res.ok ? 'Backup triggered' : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
    } catch (e) { toast('Backup trigger failed: ' + e.message, 'error'); }
};

App.openFolder = (p) => api.openFolder(p);

// ━━━━━━━━━━━━━━━━━━━━
//  LOGS
// ━━━━━━━━━━━━━━━━━━━━
App.refreshLogs = async () => {
    try {
        $('log-output').innerHTML = '<span class="text-muted">Loading logs...</span>';
        const service = $('log-service').value;
        const lines = parseInt($('log-lines').value) || 200;
        const search = $('log-search').value.toLowerCase();
        const data = await api.getLogs({ service, lines });
        let logs = data.logs || [];
        if (search) logs = logs.filter(l => l.toLowerCase().includes(search));
        $('log-output').innerHTML = logs.length > 0 ? logs.map(colorLogLine).join('\n') : '<span class="text-muted">No log entries found</span>';
        $('log-output').scrollTop = $('log-output').scrollHeight;
    } catch (e) {
        $('log-output').textContent = 'Error loading logs: ' + e.message;
    }
};

function colorLogLine(line) {
    const esc = line.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    if (/error|ERR|❌|💥|CRITICAL/i.test(line)) return `<span class="log-line-err">${esc}</span>`;
    if (/warn|⚠️/i.test(line)) return `<span class="log-line-warn">${esc}</span>`;
    if (/✅|success|connected|ready/i.test(line)) return `<span class="log-line-ok">${esc}</span>`;
    if (/\[.*\]|info|📊|📡|🌐/i.test(line)) return `<span class="log-line-info">${esc}</span>`;
    return esc;
}

App.copyLogs = () => {
    navigator.clipboard.writeText($('log-output').innerText).then(() => toast('Logs copied to clipboard', 'success'));
};

App.flushLogs = async () => {
    if (!confirm('Flush all PM2 log files? This action cannot be undone.')) return;
    toast('Flushing PM2 logs...', 'info');
    try {
        await api.flushPm2Logs();
        toast('PM2 logs flushed', 'success');
    } catch (e) { toast('Flush failed: ' + e.message, 'error'); }
};

// ━━━━━━━━━━━━━━━━━━━━
//  CONFIGURATION
// ━━━━━━━━━━━━━━━━━━━━
App.initConfigPage = async () => {
    try {
        const select = $('config-select');
        if (select.children.length === 0) {
            $('config-editor').value = 'Loading file list...';
            const list = await api.getConfigList();
            for (const item of list) {
                const opt = document.createElement('option');
                opt.value = item.key; opt.textContent = item.label;
                select.appendChild(opt);
            }
        }
        App.loadConfigFile();
    } catch (e) { $('config-editor').value = 'Error loading config list: ' + (e.message || e); }
};

App.loadConfigFile = async () => {
    const key = $('config-select').value;
    if (!key) return;
    $('config-editor').value = 'Loading...';
    try {
        const data = await api.getConfigFile(key);
        $('config-editor').value = data.ok === false ? `Error: ${data.error}` : (data.content || '(empty file)');
    } catch (e) { $('config-editor').value = 'Error loading file: ' + e.message; }
};

App.saveConfig = async () => {
    const key = $('config-select').value;
    const content = $('config-editor').value;
    toast('Saving...', 'info');
    try {
        const res = await api.saveConfigFile(key, content);
        toast(res.ok ? (res.message || 'Saved') : `Failed: ${res.error}`, res.ok ? 'success' : 'error');
    } catch (e) { toast('Save failed: ' + e.message, 'error'); }
};

App.saveConfigAndRestart = async () => {
    await App.saveConfig();
    setTimeout(async () => {
        toast('Restarting bot...', 'info');
        try {
            await api.serviceAction('whatsapp-bot-v2', 'restart');
            toast('Bot restarted', 'success');
        } catch (e) { toast('Restart failed: ' + e.message, 'error'); }
    }, 1000);
};

// ━━━━━━━━━━━━━━━━━━━━
//  LIBRARY UPDATE
// ━━━━━━━━━━━━━━━━━━━━
App.checkLibraryVersions = async () => {
    try {
        const info = await api.getLibraryInfo();
        $('lib-current').textContent = info.current || 'Not found';
        $('lib-latest').textContent = info.latest || 'Check failed';

        // Highlight if update available
        if (info.current && info.latest && info.current !== info.latest) {
            $('lib-latest').style.color = 'var(--warning)';
            $('lib-update-btn').disabled = false;
        } else if (info.current === info.latest) {
            $('lib-latest').style.color = 'var(--success)';
        }

        // Show last update info
        if (info.lastUpdate) {
            $('lib-update-log').style.display = 'block';
            const u = info.lastUpdate;
            $('lib-last-update').innerHTML = `
                <div><strong>Date:</strong> ${formatTime(u.date)}</div>
                <div><strong>From:</strong> v${u.from} → <strong>To:</strong> v${u.to}</div>
                <div><strong>Result:</strong> ${u.success ? '<span style="color:var(--success)">✅ Success</span>' : '<span style="color:var(--error)">❌ Failed</span>'}</div>
            `;
        }
    } catch (e) { toast('Version check failed: ' + e.message, 'error'); }
};

App.startLibraryUpdate = async () => {
    if (!confirm('⚠️ LIBRARY UPDATE\n\nThis will:\n• Backup current packages\n• Install latest whatsapp-web.js\n• Restart the bot\n• Auto-rollback if unhealthy\n\nThe bot will be STOPPED during the update.\n\nContinue?')) return;

    showProgress('📦 Updating WhatsApp Library');
    $('lib-update-btn').disabled = true;
    try {
        const res = await api.safeLibraryUpdate();
        hideProgress();
        if (res.ok) {
            toast(`✅ Update successful! v${res.oldVersion} → v${res.newVersion}`, 'success');
        } else if (res.rolledBack) {
            toast(`⚠️ Update rolled back: ${res.error}`, 'warning');
        } else {
            toast(`❌ Update failed: ${res.error}`, 'error');
        }
        setTimeout(() => App.checkLibraryVersions(), 2000);
    } catch (e) {
        hideProgress();
        toast('Update error: ' + e.message, 'error');
    }
    $('lib-update-btn').disabled = false;
};

App.rollbackLibrary = async () => {
    if (!confirm('⚠️ MANUAL ROLLBACK\n\nRollback to the previously backed-up package versions?\nThe bot will be stopped and restarted.\n\nContinue?')) return;

    showProgress('↩ Rolling Back Library');
    try {
        const res = await api.rollbackLibrary();
        hideProgress();
        toast(res.ok ? `✅ Rolled back to v${res.version}` : `❌ Rollback failed: ${res.error}`, res.ok ? 'success' : 'error');
        setTimeout(() => App.checkLibraryVersions(), 2000);
    } catch (e) {
        hideProgress();
        toast('Rollback error: ' + e.message, 'error');
    }
};

// ━━━━━━━━━━━━━━━━━━━━
//  ACTIONS
// ━━━━━━━━━━━━━━━━━━━━
App.restartAllTunnels = async () => {
    toast('Restarting all tunnels...', 'info');
    try {
        const res = await api.restartAllTunnels();
        toast(res.ok ? 'All tunnels restarted' : 'Failed', res.ok ? 'success' : 'error');
    } catch (e) { toast('Failed: ' + e.message, 'error'); }
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

        const sys = await api.getSystemInfo();
        $('about-info').innerHTML = `
            <div>App Version: <strong>2.0.0</strong></div>
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
    try {
        const res = await api.saveSettings(newSettings);
        appSettings = newSettings;
        startAutoRefresh();
        toast(res.ok ? 'Settings saved' : 'Failed to save', res.ok ? 'success' : 'error');
    } catch (e) { toast('Save failed: ' + e.message, 'error'); }
};

// ━━━━━━━━━━━━━━━━━━━━
//  STATUS BAR
// ━━━━━━━━━━━━━━━━━━━━
function updateStatusBar(data) {
    $('sb-wa-dot').className = 'sb-dot ' + (data.whatsapp?.isReady ? 'on' : 'off');
    $('sb-db-dot').className = 'sb-dot ' + (data.database?.connected ? 'on' : 'off');
    const svcs = data.services || [];
    const bot = svcs.find(s => s.name === 'whatsapp-bot-v2') || svcs.find(s => s.name === 'whatsapp-bot' && s.status === 'online');
    $('sb-bot-dot').className = 'sb-dot ' + (bot?.status === 'online' ? 'on' : 'off');
    const onlineCount = svcs.filter(s => s.status === 'online').length;
    $('sb-svc-count').textContent = `Services: ${onlineCount}/${svcs.length}`;
    $('sb-time').textContent = new Date().toLocaleTimeString('en-GB');
}

// Clock
setInterval(() => { $('sb-time').textContent = new Date().toLocaleTimeString('en-GB'); }, 1000);

// ━━━━━━━━━━━━━━━━━━━━
//  AUTO REFRESH
// ━━━━━━━━━━━━━━━━━━━━
function startAutoRefresh() {
    if (refreshTimer) clearInterval(refreshTimer);
    const interval = appSettings.refreshInterval || 5000;
    refreshTimer = setInterval(() => {
        if (currentPage === 'overview') App.refreshOverview();
        else if (currentPage === 'services') App.refreshServices();
        else if (currentPage === 'whatsapp') { App.refreshWA(); App.refreshQR(); }
        else if (currentPage === 'botswap') App.refreshBotSwap();
    }, interval);
}

// ━━━━━━━━━━━━━━━━━━━━
//  INIT
// ━━━━━━━━━━━━━━━━━━━━
(async function init() {
    try { appSettings = await api.getSettings(); } catch {}
    App.refreshOverview();
    startAutoRefresh();
})();
