// jobs/DailyTyreSalesPdfSendJob.js
// Sends the previous day's (or specified day's) Daily Sales PDF (with TYRE summary) via WhatsApp.
const moment = require('moment');
const { getPool } = require('../utils/sqlPool');
const { sendMedia, send } = require('../utils/waClientRegistry');

function readRecipientsFromJobsConfig() {
  try {
    // eslint-disable-next-line global-require
    const path = require('path');

    // eslint-disable-next-line global-require
    const fs = require('fs');
    const cfgPath = path.join(__dirname, '..', 'jobs-config.json');
    if (!fs.existsSync(cfgPath)) return [];
    const cfg = JSON.parse(fs.readFileSync(cfgPath, 'utf8'));
    const job = cfg && cfg.DailyTyreSalesReportJob;
    const nums = job && Array.isArray(job.contactNumbers) ? job.contactNumbers : [];
    return nums.map(n => String(n).trim()).filter(Boolean);
  } catch {
    return [];
  }
}

function readRecipientsFromEnvAdmins() {
  try {
    const raw = (((process.env.ADMIN_NUMBERS || '') + ',' + (process.env.ADMIN_WHATSAPP_NUMBER || ''))
      .split(',')
      .map(s => String(s || '').trim())
      .filter(Boolean));
    // de-dup
    return Array.from(new Set(raw));
  } catch {
    return [];
  }
}

module.exports = async function DailyTyreSalesPdfSendJob(logAndSave = console.log, options = {}) {
  const dayISO = options.date
    ? moment(String(options.date).trim()).format('YYYY-MM-DD')
    : moment().subtract(1, 'day').format('YYYY-MM-DD');

  const recipients = (options.recipients && options.recipients.length)
    ? options.recipients
    : (() => {
        const fromCfg = readRecipientsFromJobsConfig();
        if (fromCfg && fromCfg.length) return fromCfg;
        return readRecipientsFromEnvAdmins();
      })();

  if (!recipients || recipients.length === 0) {
    logAndSave('[Daily Sales PDF] No recipients configured (DailyTyreSalesReportJob.contactNumbers or ADMIN_NUMBERS)');
    return { ok: false, error: 'no-recipients', date: dayISO };
  }

  const mainPool = options.mainPool || await getPool();
  // eslint-disable-next-line global-require
  const generatePdf = require('../utils/generateDailyTyreSalesPdf');

  const { buffer, fileName } = await generatePdf(mainPool, dayISO);
  const caption = `📄 Daily Sales PDF - ${moment(dayISO).format('MMMM DD, YYYY')} (sent ${moment().format('MMMM DD, YYYY HH:mm')})`;

  let okCount = 0;
  let failCount = 0;

  for (const num of recipients) {
    // WORKAROUND: Send media WITHOUT caption first to avoid 'markedUnread' error in WWebJS.
    // Then send caption as a separate text message.
    // eslint-disable-next-line no-await-in-loop
    // const res = await sendMedia(num, 'application/pdf', buffer, fileName, { caption });
    
    // 1. Send PDF (No Caption)
    // eslint-disable-next-line no-await-in-loop
    const res = await sendMedia(num, 'application/pdf', buffer, fileName);

    // 2. If success, send Caption
    if (res && res.ok) {
        // eslint-disable-next-line no-await-in-loop
        await send(num, caption);
        okCount++;
        logAndSave(`[Daily Sales PDF] sent to ${num}`);
    } else {
        failCount++;
        logAndSave(`[Daily Sales PDF] send failed to ${num} :: ${res && res.error ? res.error : 'unknown-error'}`);
    }

    // eslint-disable-next-line no-await-in-loop
    await new Promise(resolve => setTimeout(resolve, Number(process.env.WHATSAPP_MESSAGE_DELAY_MS || 2500)));
  }

  logAndSave(`[Daily Sales PDF] Completed: ${fileName} ok=${okCount}/${recipients.length} fail=${failCount}`);
  return { ok: okCount > 0, date: dayISO, fileName, okCount, failCount, total: recipients.length };
};
