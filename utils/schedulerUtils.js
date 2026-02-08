// utils/schedulerUtils.js
// Centralized send + logging utilities for scheduled jobs (daily sales report, watch job, etc.)
const fs = require('fs');
const path = require('path');
const { send, normalizeNumber, getClient } = require('./waClientRegistry');

// Flexible send: supports two signatures:
// - sendWhatsAppMessage(number, message)  -> single recipient
// - sendWhatsAppMessage(message)          -> send to report recipients (DailyTyreSalesReportJob.contactNumbers or env)
// - sendWhatsAppMessage('report', message) or ('admin', message) -> keyword mapped recipients
async function sendWhatsAppMessage(numberOrMessage, maybeMessage) {
    const started = Date.now();
    let recipients = [];
    let message = maybeMessage;
    let singleRecipient = null;

    // Determine mode
    if (maybeMessage === undefined) {
        // Called with single arg: treat as message to report recipients
        message = String(numberOrMessage || '');
        // Load recipients from jobs-config for DailyTyreSalesReportJob
        try {
            const cfgPath = path.join(__dirname, '..', 'jobs-config.json');
            if (fs.existsSync(cfgPath)) {
                const cfg = JSON.parse(fs.readFileSync(cfgPath, 'utf8'));
                const job = cfg && cfg['DailyTyreSalesReportJob'];
                if (job && Array.isArray(job.contactNumbers) && job.contactNumbers.length) recipients = job.contactNumbers.slice();
            }
        } catch (e) { recipients = []; }
        // Fallback to env var REPORT_RECIPIENTS (comma separated) or ADMIN_NUMBER
        if (!recipients || recipients.length === 0) {
            const envList = (process.env.REPORT_RECIPIENTS || '').split(',').map(s=>s.trim()).filter(Boolean);
            if (envList.length) recipients = envList;
            else if (process.env.ADMIN_NUMBER) recipients = [process.env.ADMIN_NUMBER];
        }
    } else {
        // two-arg form: number provided
        const key = String(numberOrMessage || '').toLowerCase();
        if (['report','reports'].includes(key)) {
            // same as single-arg report recipients
            try {
                const cfgPath = path.join(__dirname, '..', 'jobs-config.json');
                if (fs.existsSync(cfgPath)) {
                    const cfg = JSON.parse(fs.readFileSync(cfgPath, 'utf8'));
                    const job = cfg && cfg['DailyTyreSalesReportJob'];
                    if (job && Array.isArray(job.contactNumbers) && job.contactNumbers.length) recipients = job.contactNumbers.slice();
                }
            } catch (e) { recipients = []; }
            if (!recipients || recipients.length === 0) {
                const envList = (process.env.REPORT_RECIPIENTS || '').split(',').map(s=>s.trim()).filter(Boolean);
                if (envList.length) recipients = envList;
                else if (process.env.ADMIN_NUMBER) recipients = [process.env.ADMIN_NUMBER];
            }
            message = String(maybeMessage || '');
        } else if (['admin','admins'].includes(key)) {
            // admin recipients
            const admins = (process.env.ADMIN_NUMBER ? [process.env.ADMIN_NUMBER] : []).concat((process.env.ADMIN_NUMBERS||'').split(',').map(s=>s.trim()).filter(Boolean));
            recipients = admins.filter(Boolean);
            message = String(maybeMessage || '');
        } else {
            singleRecipient = String(numberOrMessage || '');
            message = String(maybeMessage || '');
        }
    }

    // If singleRecipient is set, send to that one
    if (singleRecipient) {
        const norm = normalizeNumber(singleRecipient);
        if (!norm) {
            console.error(`[SEND FAIL] invalid number provided: ${singleRecipient}`);
            return { number: null, ok: false, error: 'invalid-number' };
        }
        try {
            const result = await send(norm, message);
            if (!result.ok) {
                console.error(`[SEND FAIL] ${norm} :: ${result.error}`);
                return { number: norm, ok: false, error: result.error };
            }
            if (result.queued) {
                console.log(`[SEND QUEUED] ${norm} (client not ready) chars=${message.length}`);
                return { number: norm, queued: true, ok: true };
            }
            console.log(`[SEND OK] ${norm} chars=${message.length} in ${Date.now()-started}ms`);
            return { number: norm, queued: false, ok: true };
        } catch (e) {
            console.error(`[SEND FAIL] ${norm} :: ${e && e.message ? e.message : e}`);
            return { number: norm, ok: false, error: e && e.message ? e.message : String(e) };
        }
    }

    // Otherwise send to recipients array
    if (!recipients || recipients.length === 0) {
        console.error('[SEND FAIL] no recipients configured');
        return { ok: false, error: 'no-recipients' };
    }
    const results = [];
    for (const r of recipients) {
        const norm = normalizeNumber(r);
        if (!norm) {
            console.error(`[SEND FAIL] invalid number provided in recipients: ${r}`);
            results.push({ recipient: r, ok: false, error: 'invalid-number' });
            continue;
        }
        try {
            const res = await send(norm, message);
            results.push({ recipient: norm, ok: !!res.ok, queued: !!res.queued, error: res.error || null });
            if (res.queued) console.log(`[SEND QUEUED] ${norm} (client not ready) chars=${message.length}`);
            else if (res.ok) console.log(`[SEND OK] ${norm} chars=${message.length} in ${Date.now()-started}ms`);
            else console.error(`[SEND FAIL] ${norm} :: ${res.error}`);
        } catch (e) {
            console.error(`[SEND FAIL] ${norm} :: ${e && e.message ? e.message : e}`);
            results.push({ recipient: norm, ok: false, error: e && e.message ? e.message : String(e) });
        }
    }
    const failed = results.filter(x => !x.ok).length;
    return { ok: failed === 0, results };
}

function logAndSave(msg) {
    const logFile = 'whatsapp-bot.log';
    try {
        const line = `[${new Date().toISOString()}] ${msg}\n`;
        fs.appendFileSync(logFile, line);
    } catch (e) {
        console.error('log write error:', e.message);
    }
    console.log(msg);
}

module.exports = { sendWhatsAppMessage, logAndSave };
