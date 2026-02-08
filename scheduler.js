// scheduler.js
// Scheduler to run DailyTyreSalesReportJob every 2 hours
const cron = require('node-cron');
const sql = require('mssql');
const { sql: sqlModule, config: sqlConfig } = require('./sqlConfig'); // Import sql and config from sqlConfig
const { getPool } = require('./utils/sqlPool'); // Import mainPool helper
const { getClient } = require('./utils/waClientRegistry');
const DailyTyreSalesReportJob = require('./jobs/DailyTyreSalesReportJob');
const DailyTyreSalesPdfSendJob = require('./jobs/DailyTyreSalesPdfSendJob');
const moment = require('moment');
// const WeeklyTyreSalesReportJob = require('./jobs/WeeklyTyreSalesReportJob');
const WatchedItemRealtimeJob = require('./jobs/WatchedItemRealtimeJob');
const DeletionQueueProcessorJob = require('./jobs/DeletionQueueProcessorJob');
const DatabaseBackupJob = require('./jobs/DatabaseBackupJob'); // New Backup Job

// Facebook automation jobs (Claude-only)
const DailyFacebookPostJob = require('./jobs/DailyFacebookPostJob');
// const FacebookCommentMonitorJob = require('./jobs/FacebookCommentMonitorJob'); // Deprecated: Ollama-based
const TokenRefreshJob = require('./jobs/TokenRefreshJob');
const { start: startFbCommentResponder } = require('./jobs/FacebookCommentResponderJob');
// Messenger responder removed - not needed
const { loadConfig: loadWatchedCfg } = require('./utils/watchedItemConfig');
const { sendWhatsAppMessage, logAndSave } = require('./utils/schedulerUtils');
const { updateJobStatus, computeNextRun } = require('./utils/jobStatus');
const fs = require('fs');
const path = require('path');

// Incremental run every 2 hours at minute 0 (e.g., 00:00, 02:00, 04:00, ...)
const CRON_SPEC = '0 */2 * * *';
cron.schedule(CRON_SPEC, async () => {
    const started = new Date();
    logAndSave('Running scheduled DailyTyreSalesReportJob...');
    try {
        // Get mainPool connection
        const mainPool = await getPool();
        // In scheduled runs prefer reading from local SQLite (sales_sync.db) which is kept in sync
        await DailyTyreSalesReportJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, { useSQLite: true, mainPool });
        updateJobStatus('DailyTyreSalesReportJob', {
            lastRun: started.toISOString(),
            lastSuccess: true,
            lastError: null,
            nextRun: computeNextRun(CRON_SPEC)
        });
    } catch (e) {
        updateJobStatus('DailyTyreSalesReportJob', {
            lastRun: started.toISOString(),
            lastSuccess: false,
            lastError: e.message,
            nextRun: computeNextRun(CRON_SPEC)
        });
    }
}, {
    recoverMissedExecutions: true
});

updateJobStatus('DailyTyreSalesReportJob', { schedulerStartedAt: new Date().toISOString(), nextRun: computeNextRun(CRON_SPEC) });
console.log('Scheduler started: DailyTyreSalesReportJob will run every 2 hours.');

// Next-day PDF send (7:00 AM): send yesterday's PDF report (separate from the night summary message)
const PDF_CRON_SPEC = '0 7 * * *';
cron.schedule(PDF_CRON_SPEC, async () => {
    const started = new Date();
    const reportDate = moment().subtract(1, 'day').format('YYYY-MM-DD');
    logAndSave(`[Daily Sales PDF] Running scheduled PDF send for ${reportDate}...`);
    try {
        const mainPool = await getPool();
        await DailyTyreSalesPdfSendJob(logAndSave, { date: reportDate, mainPool });
        updateJobStatus('DailyTyreSalesPdfSendJob', { lastRun: started.toISOString(), lastSuccess: true, lastError: null, date: reportDate });
    } catch (e) {
        logAndSave(`[Daily Sales PDF] Failed: ${e.message}`);
        updateJobStatus('DailyTyreSalesPdfSendJob', { lastRun: started.toISOString(), lastSuccess: false, lastError: e.message, date: reportDate });
    }
}, {
    recoverMissedExecutions: true
});

updateJobStatus('DailyTyreSalesPdfSendJob', { schedulerStartedAt: new Date().toISOString(), nextRun: computeNextRun(PDF_CRON_SPEC) });
logAndSave(`[Daily Sales PDF] Scheduled: ${PDF_CRON_SPEC} (sends yesterday's report at 7:00 AM)`);

// Database Backup Job (11:45 PM)
// Backs up LasanthaTire and sends to Admin
const BACKUP_CRON_SPEC = '45 23 * * *';
cron.schedule(BACKUP_CRON_SPEC, async () => {
    const started = new Date();
    logAndSave(`[Database Backup] Starting scheduled backup...`);
    try {
        const mainPool = await getPool();
        // Collect all admin numbers from .env and jobs-config
        const adminNumbers = new Set();
        
        // Add from .env
        if (process.env.ADMIN_WHATSAPP_NUMBER) {
            adminNumbers.add(process.env.ADMIN_WHATSAPP_NUMBER);
        }
        if (process.env.ADMIN_NUMBER) {
            adminNumbers.add(process.env.ADMIN_NUMBER);
        }
        
        // Add from jobs-config.json
        try {
            const jc = JSON.parse(fs.readFileSync(path.join(__dirname, 'jobs-config.json'), 'utf8'));
            if (jc.DailyTyreSalesReportJob && jc.DailyTyreSalesReportJob.contactNumbers) {
                jc.DailyTyreSalesReportJob.contactNumbers.forEach(num => adminNumbers.add(num));
            }
        } catch (e) {}
        
        const adminNumbersArray = Array.from(adminNumbers);
        logAndSave(`[Database Backup] Will send backups to: ${adminNumbersArray.join(', ')}`);
        
        // Backup LasanthaTire and WhatsAppAI and send to all admin numbers
        for (const adminNum of adminNumbersArray) {
            await DatabaseBackupJob({ 
                mainPool, 
                databases: ['LasanthaTire', 'WhatsAppAI'], 
                adminNumber: adminNum 
            });
        }
        
        updateJobStatus('DatabaseBackupJob', { lastRun: started.toISOString(), lastSuccess: true, lastError: null });
    } catch (e) {
        logAndSave(`[Database Backup] Failed: ${e.message}`);
        updateJobStatus('DatabaseBackupJob', { lastRun: started.toISOString(), lastSuccess: false, lastError: e.message });
    }
}, {
    recoverMissedExecutions: false 
});
updateJobStatus('DatabaseBackupJob', { schedulerStartedAt: new Date().toISOString(), nextRun: computeNextRun(BACKUP_CRON_SPEC) });
logAndSave(`[Database Backup] Scheduled: ${BACKUP_CRON_SPEC}`);

// Full-day summary run (default 23:37 local time)
// Selection precedence: env DAILY_FULL_REPORT_TIME -> jobs-config.json DailyTyreSalesReportJob.fullDayReportTime -> jobs-defaults.json -> fallback
const JOBS_CONFIG_PATH = path.join(__dirname, 'jobs-config.json');
const JOBS_DEFAULTS_PATH = path.join(__dirname, 'jobs-defaults.json');
function hhmmToCron(hhmm) {
    const m = String(hhmm || '').trim().match(/^(\d{1,2}):(\d{2})$/);
    if (!m) return null;
    const hour = parseInt(m[1], 10);
    const minute = parseInt(m[2], 10);
    if (isNaN(hour) || isNaN(minute) || hour < 0 || hour > 23 || minute < 0 || minute > 59) return null;
    return `${minute} ${hour} * * *`;
}
function getFullDayCron() {
    // 1) env var DAILY_FULL_REPORT_TIME as HH:MM or as cron
    try {
        const envVal = process.env.DAILY_FULL_REPORT_TIME;
        if (envVal) {
            const asCron = envVal.includes(' ') ? envVal.trim() : hhmmToCron(envVal.trim());
            if (asCron && require('node-cron').validate(asCron)) {
                console.log('Full-day schedule source: ENV DAILY_FULL_REPORT_TIME ->', asCron);
                return asCron;
            } else {
                console.warn('DAILY_FULL_REPORT_TIME present but invalid format:', envVal);
            }
        }
    } catch (e) { /* ignore */ }

    // 2) jobs-config.json -> fullDayReportTime
    try {
        const jobsCfg = JSON.parse(fs.readFileSync(JOBS_CONFIG_PATH, 'utf8'));
        const job = jobsCfg['DailyTyreSalesReportJob'];
        if (job && job.fullDayReportTime) {
            const v = String(job.fullDayReportTime).trim();
            const asCron = v.includes(' ') ? v : hhmmToCron(v);
            if (asCron && require('node-cron').validate(asCron)) {
                console.log('Full-day schedule source: jobs-config.json fullDayReportTime ->', asCron);
                return asCron;
            } else {
                console.warn('jobs-config.json fullDayReportTime invalid for DailyTyreSalesReportJob:', v);
            }
        }
    } catch (e) { /* ignore read errors */ }

    // 3) jobs-defaults.json -> fullDayReportTime
    try {
        const defCfg = JSON.parse(fs.readFileSync(JOBS_DEFAULTS_PATH, 'utf8'));
        const defJob = defCfg['DailyTyreSalesReportJob'];
        if (defJob && defJob.fullDayReportTime) {
            const v = String(defJob.fullDayReportTime).trim();
            const asCron = v.includes(' ') ? v : hhmmToCron(v);
            if (asCron && require('node-cron').validate(asCron)) {
                console.log('Full-day schedule source: jobs-defaults.json fullDayReportTime ->', asCron);
                return asCron;
            }
        }
    } catch (e) { /* ignore */ }

    // 4) fallback default
    const fallback = '37 23 * * *';
    console.log('Full-day schedule source: fallback ->', fallback);
    return fallback;
}



// --- Real-time rescheduling when jobs-config.json changes ---
let fullDayTask = null;
function rescheduleFullDayJob() {
    if (fullDayTask) {
        fullDayTask.stop();
        fullDayTask = null;
    }
    const cronSpec = getFullDayCron();
    fullDayTask = cron.schedule(cronSpec, async () => {
        const started = new Date();
        logAndSave('Running full-day DailyTyreSalesReportJob summary...');
        try {
            // Get mainPool connection
            const mainPool = await getPool();
            await DailyTyreSalesReportJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, { fullDay: true, useSQLite: true, mainPool });
            updateJobStatus('DailyTyreSalesReportJob', { lastFullSummary: started.toISOString() });
        } catch (e) {
            logAndSave('Full-day summary error: ' + e.message);
        }
    }, {
        recoverMissedExecutions: true
    });
    logAndSave(`Full-day summary rescheduled at ${cronSpec} (jobs-config.json)`);
}

const jobsConfigPath = path.join(__dirname, 'jobs-config.json');
let jobsConfigWatcher = null;
let jobsConfigDebounceTimer = null;

function scheduleRescheduleFullDayJob() {
    if (jobsConfigDebounceTimer) clearTimeout(jobsConfigDebounceTimer);
    jobsConfigDebounceTimer = setTimeout(() => {
        try { rescheduleFullDayJob(); } catch (e) {
            logAndSave('Full-day reschedule error: ' + e.message);
        }
    }, 500);
}

function startJobsConfigWatcher() {
    // Ensure we don't leave dangling watchers
    try { if (jobsConfigWatcher) jobsConfigWatcher.close(); } catch (e) { /* ignore */ }
    jobsConfigWatcher = null;

    const retryMs = parseInt(process.env.JOBS_CONFIG_WATCH_RETRY_MS || '10000', 10);
    const scheduleRetry = (reason) => {
        logAndSave(`jobs-config.json watcher disabled (${reason}); retrying in ${Math.round(retryMs / 1000)}s`);
        setTimeout(() => startJobsConfigWatcher(), retryMs);
    };

    if (!fs.existsSync(jobsConfigPath)) {
        return scheduleRetry('file not found');
    }

    try {
        jobsConfigWatcher = fs.watch(jobsConfigPath, (eventType) => {
            // On Windows, "rename" can fire when the file is replaced atomically.
            if (eventType === 'change' || eventType === 'rename') {
                scheduleRescheduleFullDayJob();
            }
        });
        jobsConfigWatcher.on('error', (err) => {
            scheduleRetry(err.code || err.message || 'watch error');
        });
        logAndSave('Watching jobs-config.json for schedule changes');
    } catch (err) {
        scheduleRetry(err.code || err.message || 'watch init error');
    }
}

// Initial schedule + start watcher
rescheduleFullDayJob();
startJobsConfigWatcher();

// Lightweight interval watcher for real-time item monitoring (decoupled from WhatsApp client lifecycle)
const watchIntervalMs = (parseInt(process.env.WATCH_INTERVAL_SECONDS || '60', 10)) * 1000;
let isWatchedItemJobRunning = false;
setInterval(async () => {
    if (isWatchedItemJobRunning) {
        logAndSave('WatchedItemRealtimeJob skipped - previous run still in progress');
        return;
    }
    isWatchedItemJobRunning = true;
    try {
        const cfg = loadWatchedCfg();
        if ((!cfg.pattern || !cfg.pattern.trim()) && (!cfg.patterns || cfg.patterns.length === 0)) return; // nothing configured
        const mainPool = await getPool();
        await WatchedItemRealtimeJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, { mainPool });
    } catch (e) {
        logAndSave('WatchedItemRealtimeJob interval error: ' + e.message);
    } finally {
        isWatchedItemJobRunning = false;
    }
}, watchIntervalMs);
logAndSave(`WatchedItemRealtimeJob interval started (${watchIntervalMs/1000}s)`);

// Deletion Queue Processor - runs every 1 minute
const queueProcessorIntervalMs = 60000; // 1 minute
setInterval(async () => {
    try {
        const mainPool = await getPool();
        await DeletionQueueProcessorJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, { mainPool });
    } catch (e) {
        logAndSave('DeletionQueueProcessorJob interval error: ' + e.message);
    }
}, queueProcessorIntervalMs);
logAndSave(`DeletionQueueProcessorJob interval started (${queueProcessorIntervalMs/1000}s)`);
updateJobStatus('DeletionQueueProcessorJob', { 
    schedulerStartedAt: new Date().toISOString(),
    intervalSeconds: queueProcessorIntervalMs / 1000
});





// Start Facebook automation jobs
let facebookPostJob = null;
let facebookCommentJob = null; // legacy (kept for compatibility, not started)
let tokenRefreshJob = null;
let mediaPublisherJob = null; // Media publisher for post folder
let scheduledMediaPublisherJob = null; // Scheduled daily publisher (5:15 PM)
let whatsappClientForFacebook = null; // Store WhatsApp client reference

function startFacebookJobs(whatsappClient) {
  if (!whatsappClient) {
    console.warn('⚠️ WhatsApp client not available for Facebook jobs');
    return;
  }

    try {
        // Facebook post job (WhatsApp approval mode)
        if (String(process.env.ENABLE_DAILY_FB_POST_JOB || 'true').toLowerCase() === 'true') {
            facebookPostJob = new DailyFacebookPostJob();
            facebookPostJob.setWhatsAppClient(whatsappClient);
            facebookPostJob.start();
            console.log('✅ Daily Facebook Post Job started');
        } else {
            console.log('ℹ️ Daily Facebook Post Job disabled via ENABLE_DAILY_FB_POST_JOB');
        }

        // Scheduled Media Publisher (දිනපතා 5:15 PM)
        if (String(process.env.ENABLE_SCHEDULED_MEDIA_PUBLISHER || 'true').toLowerCase() === 'true') {
            const ScheduledMediaPublisherJob = require('./jobs/ScheduledMediaPublisherJob');
            scheduledMediaPublisherJob = new ScheduledMediaPublisherJob(whatsappClient);
            scheduledMediaPublisherJob.start();
            console.log('✅ Scheduled Media Publisher started (daily 5:15 PM)');
        } else {
            console.log('ℹ️ Scheduled Media Publisher disabled via ENABLE_SCHEDULED_MEDIA_PUBLISHER');
        }

        // Continuous Media Publisher job (optional - for immediate processing)
        if (String(process.env.ENABLE_MEDIA_PUBLISHER || 'false').toLowerCase() === 'true') {
            const MediaPublisherJob = require('./jobs/MediaPublisherJob');
            mediaPublisherJob = new MediaPublisherJob();
            mediaPublisherJob.start();
            console.log('✅ Continuous Media Publisher started (watching post folder)');
        } else {
            console.log('ℹ️ Continuous Media Publisher disabled via ENABLE_MEDIA_PUBLISHER');
        }

        // Claude-based comment responder (no public prices). Guarded by env flag.
        if (String(process.env.ENABLE_FB_COMMENT_RESPONDER || 'false').toLowerCase() === 'true') {
            startFbCommentResponder();
            console.log('✅ Facebook Comment Responder (Claude) started');
        } else {
            console.log('ℹ️ Facebook Comment Responder disabled via ENABLE_FB_COMMENT_RESPONDER');
        }

        console.log('✅ Facebook automation jobs started (Claude-only)');
        console.log('✅ WhatsApp approval system enabled');
    } catch (err) {
    console.error('❌ Facebook jobs error:', err.message);
    console.error('📋 Stack trace:', err.stack);
    console.error('🔍 Error details:', {
      name: err.name,
      code: err.code,
      errno: err.errno
    });
  }
}

const ReOrderingJob = require('./jobs/ReOrderingJob');
const DailySalesPDFReportJob = require('./jobs/DailySalesPDFReportJob');
const PeachtreeAgingReportJob = require('./jobs/PeachtreeAgingReportJob');
const { aiPoolConnect, aiPool } = require('./utils/aiDbConnection');

// Re-Ordering Job Schedules
// 1. Primary Run: 8:00 PM Daily
cron.schedule('0 20 * * *', async () => {
    logAndSave('Running scheduled ReOrderingJob (Primary 8 PM)...');
    try {
        const mainPool = await getPool();
        await aiPoolConnect;
        // Use configured contacts or defaults
        const contacts = ['0777311770', '0771222509']; 
        await ReOrderingJob(global.whatsappClient, mainPool, aiPool, contacts, logAndSave);
        updateJobStatus('ReOrderingJob', { lastRun: new Date().toISOString(), lastSuccess: true });
    } catch (e) {
        logAndSave(`❌ ReOrderingJob failed: ${e.message}`);
        updateJobStatus('ReOrderingJob', { lastRun: new Date().toISOString(), lastSuccess: false, lastError: e.message });
    }
});

// 2. Fail-Safe Run: 8:00 AM Daily (Catches missed runs from previous night)
cron.schedule('0 8 * * *', async () => {
    logAndSave('Running scheduled ReOrderingJob (Fail-Safe 8 AM)...');
    try {
        const mainPool = await getPool();
        await aiPoolConnect;
        const contacts = ['0777311770', '0771222509'];
        await ReOrderingJob(global.whatsappClient, mainPool, aiPool, contacts, logAndSave);
        updateJobStatus('ReOrderingJob', { lastRun: new Date().toISOString(), lastSuccess: true });
    } catch (e) {
        logAndSave(`❌ ReOrderingJob (Fail-Safe) failed: ${e.message}`);
    }
});

// 3. Daily Sales PDF Report: 9:00 PM Daily (DISABLED by user request)
/*
cron.schedule('0 21 * * *', async () => {
    logAndSave('Running scheduled DailySalesPDFReportJob (9 PM)...');
    try {
        const mainPool = await getPool();
        await aiPoolConnect;
        const contacts = ['0777311770', '0771222509'];
        await DailySalesPDFReportJob(global.whatsappClient, mainPool, aiPool, contacts, logAndSave);
        updateJobStatus('DailySalesPDFReportJob', { lastRun: new Date().toISOString(), lastSuccess: true });
    } catch (e) {
        logAndSave(`❌ DailySalesPDFReportJob failed: ${e.message}`);
        updateJobStatus('DailySalesPDFReportJob', { lastRun: new Date().toISOString(), lastSuccess: false, lastError: e.message });
    }
});
*/

// 4. Weekly Aging Report: Monday 8:00 AM
cron.schedule('0 8 * * 1', async () => {
    logAndSave('Running scheduled PeachtreeAgingReportJob (Weekly)...');
    try {
        const mainPool = await getPool();
        await aiPoolConnect;
        const contacts = ['0777311770', '0771222509'];
        await PeachtreeAgingReportJob(global.whatsappClient, mainPool, aiPool, contacts, logAndSave);
        updateJobStatus('PeachtreeAgingReportJob', { lastRun: new Date().toISOString(), lastSuccess: true });
    } catch (e) {
        logAndSave(`❌ PeachtreeAgingReportJob failed: ${e.message}`);
        updateJobStatus('PeachtreeAgingReportJob', { lastRun: new Date().toISOString(), lastSuccess: false, lastError: e.message });
    }
});

// --- ONE-TIME TRIGGER FOR TESTING ---
/*
setTimeout(async () => {
    logAndSave('🚀 Triggering Manual Daily Sales PDF for 2026-01-29 (Recovery - Attempt 5 - Split Caption)...');
    try {
        const client = getClient(); 
        const finalClient = client || global.whatsappClient;

        if (finalClient) {
             const target = '94777311770@c.us';
             // Diagnostic
             try {
                if (finalClient.getChatById) await finalClient.getChatById(target);
             } catch (e) {}

             // Execute Job
             await DailyTyreSalesPdfSendJob(logAndSave, { date: '2026-01-29' });
             logAndSave('✅ Manual Recovery Executed.');
        } else {
            logAndSave('⚠️ Client not ready for recovery.');
        }
    } catch (e) {
        logAndSave(`❌ Manual Recovery Failed: ${e.message}`);
    }
}, 90000); // 90 seconds
*/




/*
setTimeout(async () => {
    logAndSave('🚀 Triggering One-Time Peachtree Aging Report for Verification...');
    try {
        const mainPool = await getPool();
        await aiPoolConnect;
        const contacts = ['0777311770', '0771222509'];
        // Wait for client to be ready
        if (global.whatsappClient) {
             await PeachtreeAgingReportJob(global.whatsappClient, mainPool, aiPool, contacts, logAndSave);
        } else {
            logAndSave('⚠️ Client not ready for one-time trigger');
        }
    } catch (e) {
        logAndSave(`❌ One-Time Trigger Failed: ${e.message}`);
    }
}, 30000); // Run 30 seconds after startup
*/


// Export function to get Facebook post job (for message handling)
function getFacebookPostJob() {
    return facebookPostJob;
}

// Export function to get Media Publisher job
function getMediaPublisherJob() {
    return mediaPublisherJob;
}

// Export function to get Scheduled Media Publisher job
function getScheduledMediaPublisherJob() {
    return scheduledMediaPublisherJob;
}

// Export initialization function
function initializeScheduler(whatsappClient = null) {
    // Update job statuses for all jobs
    // Only DailyTyreSalesReportJob is scheduled
    updateJobStatus('DailyTyreSalesReportJob', {
        schedulerStartedAt: new Date().toISOString(),
        nextRun: computeNextRun(CRON_SPEC)
    });
    logAndSave('Scheduler loaded: DailyTyreSalesReportJob will run every 2 hours.');
    
    // Start Facebook automation with WhatsApp client
    startFacebookJobs(whatsappClient);

    // Run ReOrderingJob on startup (Fail-Safe for restarts) - DISABLED by user request
    /*
    if (whatsappClient) {
        setTimeout(async () => {
            logAndSave('🔄 Running ReOrderingJob on startup (Fail-Safe)...');
            try {
                const mainPool = await getPool();
                await aiPoolConnect;
                const contacts = ['0777311770', '0771222509'];
                await ReOrderingJob(whatsappClient, mainPool, aiPool, contacts, logAndSave);
            } catch (e) {
                logAndSave(`❌ Startup ReOrderingJob failed: ${e.message}`);
            }
        }, 60000); // Wait 1 minute for connections to stabilize
    }
    */
    
    return true;
}

module.exports = {
    initializeScheduler,
    getFacebookPostJob,
    getMediaPublisherJob,
    getScheduledMediaPublisherJob
};