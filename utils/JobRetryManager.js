// utils/JobRetryManager.js
// Centralized job execution wrapper with automatic retry, logging, and WhatsApp failure alerts.
//
// Usage:
//   const { runWithRetry } = require('./utils/JobRetryManager');
//   await runWithRetry('ReOrderingJob', async () => { ... }, { maxRetries: 3, retryDelayMs: 60000 });
//
// Features:
// - Automatic retry on failure with configurable delay & back-off
// - Detailed execution log per job (job-execution-log.json)
// - WhatsApp alert to admin on final failure (all retries exhausted)
// - Tracks consecutive failures per job
// - Respects a global retry registry to avoid duplicate retry timers

const fs = require('fs');
const path = require('path');
const { updateJobStatus } = require('./jobStatus');

const LOG_FILE = path.join(__dirname, '..', 'job-execution-log.json');
const MAX_LOG_ENTRIES = 200; // per job, keep last N entries

// In-memory registry to avoid scheduling duplicate retries
const pendingRetries = new Map(); // jobName -> timeoutId
const consecutiveFailures = new Map(); // jobName -> count

// ─── Helpers ───────────────────────────────────────────────────────
function readLog() {
    try {
        if (fs.existsSync(LOG_FILE)) return JSON.parse(fs.readFileSync(LOG_FILE, 'utf8'));
    } catch { /* ignore corrupt file */ }
    return {};
}

function writeLog(data) {
    try {
        fs.writeFileSync(LOG_FILE, JSON.stringify(data, null, 2));
    } catch (e) {
        console.error('[JobRetryManager] Failed to write log:', e.message);
    }
}

function appendLogEntry(jobName, entry) {
    const log = readLog();
    if (!log[jobName]) log[jobName] = [];
    log[jobName].push(entry);
    // Keep only last N entries per job
    if (log[jobName].length > MAX_LOG_ENTRIES) {
        log[jobName] = log[jobName].slice(-MAX_LOG_ENTRIES);
    }
    writeLog(log);
}

/**
 * Send a WhatsApp alert to admin about a job failure.
 * Gracefully no-ops if WhatsApp client is unavailable.
 */
async function sendFailureAlert(jobName, error, attempt, maxRetries, logger) {
    try {
        const { sendWhatsAppMessage } = require('./schedulerUtils');
        const failures = consecutiveFailures.get(jobName) || 0;

        let alertMsg;
        if (attempt >= maxRetries) {
            // Final failure — all retries exhausted
            alertMsg =
                `🚨 *JOB FAILED — ALL RETRIES EXHAUSTED*\n\n` +
                `📛 *Job:* ${jobName}\n` +
                `❌ *Error:* ${error}\n` +
                `🔄 *Attempts:* ${attempt}/${maxRetries}\n` +
                `📊 *Consecutive failures:* ${failures}\n` +
                `⏰ *Time:* ${new Date().toLocaleString('en-LK', { timeZone: 'Asia/Colombo' })}\n\n` +
                `⚠️ This job will NOT run again until its next scheduled time.\n` +
                `Use _!job retry ${jobName}_ to manually retry.`;
        } else {
            // Retrying — informational alert (only send on 1st failure, not every retry)
            if (attempt > 1) return; // Don't spam on intermediate retries
            alertMsg =
                `⚠️ *JOB FAILED — RETRYING*\n\n` +
                `📛 *Job:* ${jobName}\n` +
                `❌ *Error:* ${error}\n` +
                `🔄 *Will retry:* attempt ${attempt + 1}/${maxRetries}\n` +
                `⏰ *Time:* ${new Date().toLocaleString('en-LK', { timeZone: 'Asia/Colombo' })}`;
        }

        // Send to admin (uses schedulerUtils which reads from config/env)
        await sendWhatsAppMessage(alertMsg);
        if (logger) logger(`[JobRetryManager] ⚠️ Failure alert sent for ${jobName}`);
    } catch (alertErr) {
        // Don't let alert failure crash the retry system
        if (logger) logger(`[JobRetryManager] Could not send failure alert: ${alertErr.message}`);
    }
}

// ─── Main: runWithRetry ────────────────────────────────────────────
/**
 * Execute a job function with automatic retry on failure.
 *
 * @param {string} jobName - Unique identifier for the job
 * @param {Function} jobFn - Async function to execute
 * @param {Object} [options]
 * @param {number} [options.maxRetries=3] - Max retry attempts (including first run = total attempts)
 * @param {number} [options.retryDelayMs=120000] - Delay between retries (2 min default)
 * @param {number} [options.backoffMultiplier=1.5] - Multiply delay on each subsequent retry
 * @param {number} [options.maxDelayMs=600000] - Maximum delay cap (10 min)
 * @param {boolean} [options.alertOnFailure=true] - Send WhatsApp alert on failure
 * @param {Function} [options.logger=console.log] - Logger function
 * @returns {Promise<{success: boolean, result?: any, error?: string, attempts: number}>}
 */
async function runWithRetry(jobName, jobFn, options = {}) {
    const {
        maxRetries = 3,
        retryDelayMs = 120000,       // 2 minutes
        backoffMultiplier = 1.5,
        maxDelayMs = 600000,          // 10 minutes
        alertOnFailure = true,
        logger = console.log,
    } = options;

    logger(`[JobRetryManager] ▶ ${jobName} starting...`);

    // Cancel any pending retry for this job (fresh run replaces old retry)
    if (pendingRetries.has(jobName)) {
        clearTimeout(pendingRetries.get(jobName));
        pendingRetries.delete(jobName);
    }

    let attempt = 0;
    let lastError = null;
    let currentDelay = retryDelayMs;

    while (attempt < maxRetries) {
        attempt++;
        const started = new Date();
        const isRetry = attempt > 1;

        if (isRetry) {
            logger(`[JobRetryManager] 🔄 Retrying ${jobName} (attempt ${attempt}/${maxRetries})...`);
        }

        try {
            const result = await jobFn();

            // ✅ Success
            const durationMs = Date.now() - started.getTime();
            consecutiveFailures.set(jobName, 0);

            const logEntry = {
                timestamp: started.toISOString(),
                status: 'success',
                attempt,
                durationMs,
                isRetry,
            };
            appendLogEntry(jobName, logEntry);
            logger(`[JobRetryManager] ✅ ${jobName} completed (${(durationMs / 1000).toFixed(1)}s)`);

            updateJobStatus(jobName, {
                lastRun: started.toISOString(),
                lastSuccess: true,
                lastError: null,
                lastDuration: durationMs,
                retryAttempt: isRetry ? attempt : 0,
                consecutiveFailures: 0,
            });

            if (isRetry) {
                logger(`[JobRetryManager] ✅ ${jobName} succeeded on retry attempt ${attempt}`);
                // Send recovery alert
                try {
                    const { sendWhatsAppMessage } = require('./schedulerUtils');
                    await sendWhatsAppMessage(
                        `✅ *JOB RECOVERED*\n\n` +
                        `📛 *Job:* ${jobName}\n` +
                        `🔄 *Succeeded on attempt:* ${attempt}/${maxRetries}\n` +
                        `⏱️ *Duration:* ${(durationMs / 1000).toFixed(1)}s\n` +
                        `⏰ *Time:* ${new Date().toLocaleString('en-LK', { timeZone: 'Asia/Colombo' })}`
                    );
                } catch { /* ignore */ }
            }

            return { success: true, result, attempts: attempt };

        } catch (err) {
            // ❌ Failed
            lastError = err.message || String(err);
            const durationMs = Date.now() - started.getTime();
            const failures = (consecutiveFailures.get(jobName) || 0) + 1;
            consecutiveFailures.set(jobName, failures);

            const logEntry = {
                timestamp: started.toISOString(),
                status: 'failed',
                attempt,
                maxRetries,
                durationMs,
                error: lastError,
                isRetry,
                consecutiveFailures: failures,
            };
            appendLogEntry(jobName, logEntry);

            updateJobStatus(jobName, {
                lastRun: started.toISOString(),
                lastSuccess: false,
                lastError,
                lastDuration: durationMs,
                retryAttempt: attempt,
                consecutiveFailures: failures,
            });

            logger(`[JobRetryManager] ❌ ${jobName} failed (attempt ${attempt}/${maxRetries}): ${lastError}`);

            // Send WhatsApp alert
            if (alertOnFailure) {
                await sendFailureAlert(jobName, lastError, attempt, maxRetries, logger);
            }

            // If more retries available, schedule one
            if (attempt < maxRetries) {
                logger(`[JobRetryManager] ⏳ Will retry ${jobName} in ${Math.round(currentDelay / 1000)}s...`);

                await new Promise((resolve) => {
                    const timerId = setTimeout(resolve, currentDelay);
                    pendingRetries.set(jobName, timerId);
                });
                pendingRetries.delete(jobName);

                // Increase delay with back-off
                currentDelay = Math.min(currentDelay * backoffMultiplier, maxDelayMs);
            }
        }
    }

    // All retries exhausted
    logger(`[JobRetryManager] 🚨 ${jobName} FAILED after ${maxRetries} attempts. Last error: ${lastError}`);
    return { success: false, error: lastError, attempts: attempt };
}

// ─── Utility: Get execution history for a job ──────────────────────
function getJobHistory(jobName, limit = 20) {
    const log = readLog();
    const entries = log[jobName] || [];
    return entries.slice(-limit);
}

// ─── Utility: Get failure summary across all jobs ──────────────────
function getFailureSummary() {
    const log = readLog();
    const summary = {};
    for (const [jobName, entries] of Object.entries(log)) {
        const recent = entries.slice(-50);
        const failures = recent.filter(e => e.status === 'failed');
        const successes = recent.filter(e => e.status === 'success');
        const lastEntry = recent[recent.length - 1];
        summary[jobName] = {
            totalRecent: recent.length,
            failures: failures.length,
            successes: successes.length,
            successRate: recent.length > 0 ? Math.round((successes.length / recent.length) * 100) : 0,
            lastStatus: lastEntry?.status || 'unknown',
            lastRun: lastEntry?.timestamp || null,
            consecutiveFailures: consecutiveFailures.get(jobName) || 0,
        };
    }
    return summary;
}

// ─── Utility: Cancel a pending retry ───────────────────────────────
function cancelRetry(jobName) {
    if (pendingRetries.has(jobName)) {
        clearTimeout(pendingRetries.get(jobName));
        pendingRetries.delete(jobName);
        return true;
    }
    return false;
}

// ─── Utility: Check if a retry is pending ──────────────────────────
function isRetryPending(jobName) {
    return pendingRetries.has(jobName);
}

module.exports = {
    runWithRetry,
    getJobHistory,
    getFailureSummary,
    cancelRetry,
    isRetryPending,
    consecutiveFailures,
};
