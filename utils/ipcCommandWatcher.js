// utils/ipcCommandWatcher.js
// Simple file-based IPC so external apps (Dashboard/WPF) can instruct the bot.
// Commands are JSON files written to ./ipc-commands/*.json
// Supported commands:
// - { type: "send", number: "9477...", message: "..." }
// - { type: "logout" }

const fs = require('fs');
const path = require('path');
const { send, getClient } = require('./waClientRegistry');
const moment = require('moment');

// Helps verify which version is running under PM2
console.log('[IPC] ipcCommandWatcher loaded (supports: send, logout, daily-sales-pdf, accounting-report)');

const CMD_DIR = path.join(__dirname, '..', 'ipc-commands');
const RESP_DIR = path.join(__dirname, '..', 'ipc-responses');

function ensureDir(p) {
  try { if (!fs.existsSync(p)) fs.mkdirSync(p, { recursive: true }); } catch {}
}

function safeReadJSON(file) {
  try {
    const raw = fs.readFileSync(file, 'utf8');
    const cleaned = String(raw || '').replace(/^\uFEFF/, '').trim();
    return JSON.parse(cleaned);
  } catch {
    return null;
  }
}

function writeResponse(baseName, data) {
  try {
    ensureDir(RESP_DIR);
    fs.writeFileSync(path.join(RESP_DIR, baseName.replace(/\.json$/, '') + '.json'), JSON.stringify({ ...data, at: new Date().toISOString() }), 'utf8');
  } catch {}
}

async function processCommandFile(filePath, logCb) {
  const base = path.basename(filePath);
  const cmd = safeReadJSON(filePath);
  if (!cmd || typeof cmd !== 'object') {
    writeResponse(base, { ok: false, error: 'invalid-command' });
    try { fs.unlinkSync(filePath); } catch {}
    return;
  }
  try {
    if (cmd.type === 'send') {
      const res = await send(cmd.number, cmd.message || '');
      writeResponse(base, { ok: !!res.ok, queued: !!res.queued, retry: !!res.retry });
      logCb && logCb(`[IPC] send -> ${cmd.number} ok=${res.ok} queued=${!!res.queued}`);
    } else if (cmd.type === 'daily-sales-pdf') {
      // Triggers DailyTyreSalesPdfSendJob inside the running bot process.
      // Command example:
      // { "type": "daily-sales-pdf", "date": "YYYY-MM-DD", "recipients": ["077...."] }
      const client = getClient();
      if (!client) {
        writeResponse(base, { ok: false, error: 'client-not-ready' });
        logCb && logCb('[IPC] daily-sales-pdf requested but client not ready');
        return;
      }

      // eslint-disable-next-line global-require
      const DailyTyreSalesPdfSendJob = require('../jobs/DailyTyreSalesPdfSendJob');
      const dateISO = cmd.date
        ? moment(String(cmd.date).trim()).format('YYYY-MM-DD')
        : moment().subtract(1, 'day').format('YYYY-MM-DD');
      const recipients = Array.isArray(cmd.recipients) ? cmd.recipients : undefined;

      logCb && logCb(`[IPC] daily-sales-pdf -> date=${dateISO} recipients=${recipients ? recipients.length : 'default'}`);
      const result = await DailyTyreSalesPdfSendJob(logCb || console.log, { date: dateISO, recipients });
      writeResponse(base, { ok: !!(result && result.ok), result });
    } else if (cmd.type === 'accounting-report') {
      const client = getClient();
      if (!client) {
         writeResponse(base, { ok: false, error: 'client-not-ready' });
         logCb && logCb('[IPC] accounting-report requested but client not ready');
         return;
      }
      
      try {
        logCb && logCb('[IPC] Triggering Manual Accounting Report');
        // Retrieve the module - ensuring we use the same instance if cached properly (or new one with client passed effectively)
        // Note: initializeDailyAccountingReport stores the client in a module-level variable in that file.
        // Since we are in the same process, requiring it again *should* give the same module instance which has 'whatsappClient' set.
        const { sendDailyReport } = require('../jobs/DailyAccountingReportJob');
        await sendDailyReport();
        writeResponse(base, { ok: true });
        logCb && logCb('[IPC] Manual Accounting Report sent successfully');
      } catch (err) {
        logCb && logCb('[IPC] Error sending accounting report: ' + err.message);
        writeResponse(base, { ok: false, error: err.message });
      }
    } else if (cmd.type === 'logout') {
      const client = getClient();
      if (client && typeof client.logout === 'function') {
        await client.logout();
        writeResponse(base, { ok: true });
        logCb && logCb('[IPC] logout requested');
      } else {
        writeResponse(base, { ok: false, error: 'client-not-ready' });
      }
    } else {
      writeResponse(base, {
        ok: false,
        error: 'unknown-type',
        receivedType: cmd.type,
        keys: Object.keys(cmd || {})
      });
    }
  } catch (e) {
    writeResponse(base, { ok: false, error: (e && e.message) || String(e) });
  } finally {
    try { fs.unlinkSync(filePath); } catch {}
  }
}

function startWatcher(logCb) {
  try {
    ensureDir(CMD_DIR);
    ensureDir(RESP_DIR);
    // Process any pending files on startup
    for (const f of fs.readdirSync(CMD_DIR)) {
      if (f.endsWith('.json')) processCommandFile(path.join(CMD_DIR, f), logCb);
    }
    // Watch for new files
    fs.watch(CMD_DIR, { persistent: false }, (event, filename) => {
      if (!filename || !filename.endsWith('.json')) return;
      const full = path.join(CMD_DIR, filename);
      // slight delay to avoid partial writes
      setTimeout(() => {
        if (fs.existsSync(full)) processCommandFile(full, logCb);
      }, 100);
    });
    logCb && logCb('[IPC] Command watcher active (types: send, logout, daily-sales-pdf)');
  } catch (e) {
    logCb && logCb('[IPC] Failed to start watcher: ' + (e && e.message));
  }
}

module.exports = { startWatcher };
