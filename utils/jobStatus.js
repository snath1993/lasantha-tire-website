// utils/jobStatus.js
// Unified job status tracking (file-based + in-memory helpers) without modifying core business logic.
const fs = require('fs');
const path = require('path');

const STATUS_FILE = path.join(__dirname, '..', 'job-status.json');

function readStatus() {
  if (!fs.existsSync(STATUS_FILE)) return {};
  try { return JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8')); } catch { return {}; }
}

function writeStatus(status) {
  fs.writeFileSync(STATUS_FILE, JSON.stringify(status, null, 2));
}

function updateJobStatus(jobName, partial) {
  const status = readStatus();
  if (!status[jobName]) status[jobName] = {};
  status[jobName] = { ...status[jobName], ...partial, updatedAt: new Date().toISOString() };
  writeStatus(status);
}

function computeNextRun(cronSpec) {
  if (cronSpec === '0 */2 * * *') {
    const d = new Date();
    d.setMinutes(0,0,0);
    const nextHourBlock = d.getHours() % 2 === 0 ? d.getHours() + 2 : d.getHours() + (2 - (d.getHours() % 2));
    d.setHours(nextHourBlock);
    return d.toISOString();
  }
  return null;
}

// Optional in-memory tracker (not currently used by scheduler, but exposed)
const memoryStatus = {};
function start(name) {
  if (!memoryStatus[name]) memoryStatus[name] = {};
  memoryStatus[name].lastStart = new Date().toISOString();
  memoryStatus[name].running = true;
  memoryStatus[name].runs = (memoryStatus[name].runs || 0) + 1;
}
function end(name, ok = true, errMsg = null) {
  if (!memoryStatus[name]) memoryStatus[name] = {};
  memoryStatus[name].lastEnd = new Date().toISOString();
  memoryStatus[name].running = false;
  if (ok) memoryStatus[name].lastSuccess = memoryStatus[name].lastEnd; else memoryStatus[name].lastError = errMsg;
}
function snapshot() { return memoryStatus; }

module.exports = { updateJobStatus, computeNextRun, start, end, snapshot };
