// utils/ipcManager.js
// Unified IPC manager for handling all inter-process communication
const fs = require('fs');
const path = require('path');
const { updateJobStatus } = require('./jobStatus');
const sql = require('mssql');
const { config: sqlConfig } = require('../sqlConfig');
const DailyTyreSalesReportJob = require('../jobs/DailyTyreSalesReportJob');
const { getClient, send } = require('./waClientRegistry');
const { sendWhatsAppMessage: schedulerSend } = require('./schedulerUtils');

// Constants
const IPC_DIR = path.join(__dirname, '..', 'ipc');
const COMMAND_FILE = path.join(IPC_DIR, 'dashboard-commands.json');
const TEMP_FILE = path.join(IPC_DIR, 'dashboard-commands.tmp');

// Ensure IPC directory exists
if (!fs.existsSync(IPC_DIR)) fs.mkdirSync(IPC_DIR, { recursive: true });

// State management
let lastCommandId = null;
let lastContentHash = null;
let parseErrorCount = 0;
let lastParseErrorMsg = '';

// Error retry configuration
const MAX_RETRY_ATTEMPTS = 3;
const RETRY_DELAY_MS = 1000;

// Command handlers
const handlers = {
  async trigger_sales_report(log, params) {
    try {
      await DailyTyreSalesReportJob(sql, sqlConfig, schedulerSend, log, { fullDay: true });
      updateJobStatus('DailyTyreSalesReportJob', { lastManualFullDayTrigger: new Date().toISOString() });
    } catch (e) {
      log('IPC trigger_sales_report error: ' + e.message);
      throw e;
    }
  },

  async send_2fa_code(log, { number, code }) {
    if (!number || !code) {
      log('IPC 2FA missing number/code');
      throw new Error('Missing required parameters');
    }
    const result = await send(number, `🔐 2FA Code: *${code}*\nThis code expires in 5 minutes.`);
    if (!result.ok) throw new Error(result.error || 'Send failed');
    return result;
  },

  async restart_scheduler(log) {
    try {
      const schedPath = require.resolve('../scheduler');
      delete require.cache[schedPath];
      require('../scheduler');
      updateJobStatus('DailyTyreSalesReportJob', { schedulerRestartedAt: new Date().toISOString() });
    } catch (e) {
      log('IPC restart_scheduler error: ' + e.message);
      throw e;
    }
  },

  async toggle_job(log, { jobId, enabled }) {
    const configPath = path.join(__dirname, '..', 'jobs-config.json');
    try {
      const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));
      if (!config[jobId]) throw new Error('Invalid job ID');
      config[jobId].enabled = enabled;
      fs.writeFileSync(configPath, JSON.stringify(config, null, 2));
      log(`IPC: Job ${jobId} ${enabled ? 'enabled' : 'disabled'}`);
    } catch (e) {
      log('IPC toggle_job error: ' + e.message);
      throw e;
    }
  }
};

// Utility functions
function safeParse(jsonText) {
  const trimmed = jsonText.replace(/\uFEFF/g, '').trim();
  try {
    return JSON.parse(trimmed);
  } catch (e) {
    if (
      /Unexpected end of JSON input/i.test(e.message) ||
      /Expected property name or '}'/i.test(e.message) ||
      (!trimmed.endsWith('}') && trimmed.length < 50)
    ) {
      return null;
    }
    throw e;
  }
}

function hash(str) {
  let h = 0;
  for (let i = 0; i < str.length; i++) {
    h = ((h << 5) - h) + str.charCodeAt(i);
    h |= 0;
  }
  return h.toString();
}

// Command writer
function writeCommandAtomic(cmd) {
  const payload = JSON.stringify(cmd, null, 2);
  fs.writeFileSync(TEMP_FILE, payload);
  fs.renameSync(TEMP_FILE, COMMAND_FILE);
}

// Command processor
async function processCommand(cmd, log) {
  if (!cmd || !cmd.action) {
    throw new Error('Invalid command format');
  }

  const handler = handlers[cmd.action];
  if (!handler) {
    log('IPC: Unknown action ' + cmd.action);
    return;
  }

  try {
    await handler(log, cmd);
    log(`IPC: Successfully processed ${cmd.action}`);
  } catch (error) {
    log(`IPC: Error processing ${cmd.action}: ${error.message}`);
    throw error;
  }
}

// Start watcher
function startWatcher(log) {
  log('IPC Manager: Starting command watcher...');

  async function tick() {
    try {
      if (!fs.existsSync(COMMAND_FILE)) return;
      
      const content = fs.readFileSync(COMMAND_FILE, 'utf8');
      const contentHash = hash(content);
      
      // Skip if content hasn't changed
      if (contentHash === lastContentHash) return;
      lastContentHash = contentHash;

      const cmd = safeParse(content);
      if (!cmd) return;

      // Skip if already processed
      if (cmd.id === lastCommandId) return;
      lastCommandId = cmd.id;

      // Process command with retries
      let lastError = null;
      for (let attempt = 1; attempt <= MAX_RETRY_ATTEMPTS; attempt++) {
        try {
          await processCommand(cmd, log);
          parseErrorCount = 0;
          lastParseErrorMsg = '';
          return;
        } catch (e) {
          lastError = e;
          if (attempt < MAX_RETRY_ATTEMPTS) {
            await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS));
          }
        }
      }

      // If all retries failed
      log(`IPC: Command failed after ${MAX_RETRY_ATTEMPTS} attempts: ${lastError.message}`);
      parseErrorCount++;
      lastParseErrorMsg = lastError.message;

    } catch (e) {
      log('IPC: Watcher error: ' + e.message);
      parseErrorCount++;
      lastParseErrorMsg = e.message;
    }
  }

  // Check every 5 seconds
  setInterval(tick, 5000);
  log('IPC Manager: Command watcher started');
}

module.exports = {
  startWatcher,
  writeCommandAtomic,
  processCommand
};