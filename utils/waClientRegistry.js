// utils/waClientRegistry.js
// WhatsApp client registry + queued send support so 2FA codes are not lost if client not ready.
let _client = null;
const _queue = []; // { number, message, attempts }
const LOG_FILE = 'whatsapp-bot.log';
const QUEUE_FILE = 'wa-queued.json';
let _queueLoaded = false;
const MAX_ATTEMPTS = parseInt(process.env.WA_MAX_QUEUE_ATTEMPTS || '3', 10);
const fs = require('fs');

function appendLog(line) {
  try {
    fs.appendFileSync(LOG_FILE, `[${new Date().toISOString()}] ${line}\n`);
  } catch {}
}

function normalizeNumber(raw) {
  if (!raw) return raw;
  let n = ('' + raw).replace(/[^0-9+]/g, '');
  // Validate length - reject if too long (max 15 digits as per E.164 standard)
  if (n.length > 15) return null;
  // Remove leading +
  if (n.startsWith('+')) n = n.slice(1);
  // If starts with 0 and length 10 (e.g. Sri Lanka), replace leading 0 with country code
  const cc = process.env.DEFAULT_COUNTRY_CODE || '94';
  if (n.startsWith('0') && n.length === 10) n = cc + n.slice(1);
  // If still short (<=11) and doesn't start with cc, attempt prefix
  if (!n.startsWith(cc) && n.length <= 11) n = cc + n;
  // Final validation - must be between 10 and 15 digits
  return (n.length >= 10 && n.length <= 15) ? n : null;
}

function loadQueueFromDisk() {
  try {
    if (_queueLoaded) return; // only load once per process
    _queueLoaded = true;
    if (!fs.existsSync(QUEUE_FILE)) return;
    const raw = fs.readFileSync(QUEUE_FILE, 'utf8');
    const arr = JSON.parse(raw);
    if (Array.isArray(arr)) {
      // merge into in-memory queue without duplicating same number+message
      let added = 0;
      for (const it of arr) {
        const exists = _queue.find(q => q.number === it.number && q.message === it.message);
        if (!exists) {
          _queue.push(it);
          added++;
        }
      }
      appendLog('[WA] Loaded queued messages from disk: ' + added + ' (total in-memory: ' + _queue.length + ')');
    }
  } catch (e) {
    appendLog('[WA] Failed to load queue from disk: ' + (e && e.message));
  }
}

// Load existing queued messages on startup and clean invalid entries
function cleanQueueInMemory() {
  let removed = 0;
  for (let i = _queue.length - 1; i >= 0; i--) {
    const it = _queue[i];
    if (!it || !it.number || !normalizeNumber(it.number)) {
      _queue.splice(i, 1);
      removed++;
    }
  }
  if (removed > 0) appendLog('[WA] Cleaned invalid queued items on startup: ' + removed);
}

loadQueueFromDisk();
cleanQueueInMemory();
saveQueueToDisk();

function setClient(c) {
  _client = c;
  // Delay flush slightly to ensure session fully initialized
  if (_client && _queue.length > 0) {
    const delay = parseInt(process.env.WA_QUEUE_FLUSH_DELAY_MS || '1500', 10);
    setTimeout(async () => {
      // take a snapshot of items to process, clear in-memory queue and attempt sending
      const toProcess = _queue.splice(0, _queue.length);
      for (const item of toProcess) {
        try {
          // skip invalid/null numbers
          if (!item || !item.number) {
            const skip = '[WA] Skipping invalid queued item (missing number)';
            console.warn(skip);
            appendLog(skip);
            continue;
          }
          const norm = normalizeNumber(item.number);
          if (!norm) {
            const skip = '[WA] Skipping queued item with invalid normalized number: ' + item.number;
            console.warn(skip);
            appendLog(skip);
            continue;
          }
          await internalSend(norm, item.message);
          const msg = '[WA] Flushed queued message to ' + item.number;
          console.log(msg);
          appendLog(msg);
        } catch (e) {
          // requeue with attempt count, up to MAX_ATTEMPTS
          item.attempts = (item.attempts || 0) + 1;
          if (item.attempts < MAX_ATTEMPTS) {
            // avoid duplicate requeue
            const exists = _queue.find(q => q.number === item.number && q.message === item.message);
            if (!exists) _queue.push(item);
            const warn = '[WA] Requeued flushed item ' + item.number + ' attempt=' + item.attempts;
            console.warn(warn);
            appendLog(warn);
          } else {
            const errLine = '[WA] Dropping queued item after max attempts: ' + item.number + ' attempts=' + item.attempts + ' err=' + (e && e.message);
            console.error(errLine);
            appendLog(errLine);
          }
        }
      }
      // persist any remaining items (or remove file when empty)
      saveQueueToDisk();
    }, delay);
  }
}
function getClient() { return _client; }

async function internalSend(norm, message) {
  if (!_client) throw new Error('client-not-ready');
  if (!norm) throw new Error('invalid-number');
  const numId = await safeGetNumberId(norm);
  if (!numId) throw new Error('unregistered-number:' + norm);
  const jid = (numId && numId._serialized) || (String(norm).endsWith('@c.us') ? norm : `${norm}@c.us`);
  return _client.sendMessage(jid, message);
}

async function safeGetNumberId(norm) {
  try {
    if (!_client) return null;
    if (!norm) return null;
    if (typeof _client.getNumberId === 'function') {
      const plain = String(norm).includes('@c.us') ? String(norm).replace('@c.us','') : String(norm);
      return await _client.getNumberId(plain);
    }
    return { _serialized: String(norm).endsWith('@c.us') ? String(norm) : `${String(norm)}@c.us` };
  } catch (e) {
    console.error('[WA] getNumberId error:', e.message);
    return null;
  }
}

function saveQueueToDisk() {
  try {
    if (_queue.length === 0) {
      // remove file if exists
      try {
        if (fs.existsSync(QUEUE_FILE)) fs.unlinkSync(QUEUE_FILE);
      } catch (e) {
        appendLog('[WA] Failed to remove empty queue file: ' + (e && e.message));
      }
      return;
    }
    fs.writeFileSync(QUEUE_FILE, JSON.stringify(_queue), 'utf8');
  } catch (e) {
    appendLog('[WA] Failed to save queue to disk: ' + (e && e.message));
  }
}

async function sendRaw(number, message) {
  const norm = normalizeNumber(number);
  // don't allow invalid numbers to be queued or sent
  if (!norm) {
    const err = '[WA] sendRaw called with invalid/empty number: ' + String(number);
    console.error(err);
    appendLog(err);
    throw new Error('invalid-number');
  }
  if (!_client) {
    _queue.push({ number: norm, message, attempts: 0 });
    try { saveQueueToDisk(); } catch (e) { /* best-effort */ }
    return { queued: true };
  }
  try {
    await internalSend(norm, message);
    return { queued: false };
  } catch (e) {
    // If evaluation / readiness issue, queue one retry
    if ((/Evaluation failed/i.test(e.message) || /client-not-ready/.test(e.message)) && !_queue.find(q => q.number === norm)) {
      _queue.push({ number: norm, message, attempts: 1 });
      try { saveQueueToDisk(); } catch (e) {}
      return { queued: true, retry: true };
    }
    console.error('[WA] sendRaw error:', e.message);
    throw e;
  }
}

// Safe wrapper exported for callers: validates input and returns user-friendly results
async function send(number, message) {
  try {
    const norm = normalizeNumber(number);
    if (!norm) return { ok: false, error: 'invalid-number' };
    const res = await sendRaw(norm, message);
    return { ok: true, queued: !!res.queued, retry: !!res.retry };
  } catch (e) {
    // normalize known errors to structured objects
    const msg = (e && e.message) ? e.message : String(e);
    appendLog('[WA] send wrapper error: ' + msg);
    return { ok: false, error: msg };
  }
}

// Media sending (PDF, images, etc.)
// Note: we intentionally do NOT queue media to disk (can be large). If client is not ready, caller should retry later.
async function sendMedia(number, mimeType, buffer, filename, options = {}) {
  try {
    const norm = normalizeNumber(number);
    if (!norm) return { ok: false, error: 'invalid-number' };

    const client = _client || (global && global.whatsappClient);
    if (!client) return { ok: false, error: 'client-not-ready' };

    let MessageMedia;
    try {
      // BAILEYS MIGRATION: Use wrapper
      ({ MessageMedia } = require('./baileysWrapper'));
    } catch (e) {
      return { ok: false, error: 'baileys-wrapper-not-available' };
    }

    // Resolve JID
    let jid = String(norm).endsWith('@c.us') ? String(norm) : `${String(norm)}@c.us`;
    try {
      if (typeof client.getNumberId === 'function') {
        const plain = String(norm).includes('@c.us') ? String(norm).replace('@c.us', '') : String(norm);
        const numId = await client.getNumberId(plain);
        if (numId && numId._serialized) jid = numId._serialized;
      }
    } catch (e) {
      // If numberId lookup fails, attempt sending using the derived jid.
      appendLog('[WA] sendMedia getNumberId error: ' + (e && e.message ? e.message : String(e)));
    }

    const base64 = Buffer.isBuffer(buffer) ? buffer.toString('base64') : Buffer.from(buffer || '').toString('base64');
    const media = new MessageMedia(mimeType, base64, filename);

    // WhatsApp Web store sometimes isn't fully ready immediately after boot.
    // Wait for Store.Chat + WidFactory, then retry on evaluation errors.
    const ready = await waitForWhatsAppWebStore(client);
    if (!ready) {
      throw new Error('wa-store-not-ready');
    }

    // Attempt to pre-load chat to avoid 'markedUnread' undefined error
    try {
        if (typeof client.getChatById === 'function') {
           await client.getChatById(jid);
        }
    } catch (ignore) {}

    const maxTries = parseInt(process.env.WA_MEDIA_SEND_MAX_TRIES || '4', 10);
    let lastErr = null;
    for (let attempt = 1; attempt <= maxTries; attempt++) {
      try {
        await client.sendMessage(jid, media, options);
        return { ok: true };
      } catch (e) {
        lastErr = e;
        const msg = (e && e.message) ? e.message : String(e);
        const isReadinessError = /Evaluation failed/i.test(msg)
          || /Cannot read properties of undefined \(reading 'getChat'\)/i.test(msg)
          || /Cannot read properties of undefined \(reading 'WidFactory'\)/i.test(msg)
          || /Cannot read properties of undefined \(reading 'markedUnread'\)/i.test(msg)
          || /getChat/i.test(msg)
          || /WidFactory/i.test(msg)
          || /wa-store-not-ready/i.test(msg);
        if (!isReadinessError || attempt === maxTries) throw e;
        
        const warnMsg = `[WA] sendMedia retry ${attempt}/${maxTries} due to readiness error: ${msg}`;
        console.warn(warnMsg);
        appendLog(warnMsg);
        
        await new Promise(r => setTimeout(r, 3000 * attempt));
        await waitForWhatsAppWebStore(client);
      }
    }
    if (lastErr) throw lastErr;
    return { ok: true };
  } catch (e) {
    const msg = (e && e.message) ? e.message : String(e);
    appendLog('[WA] sendMedia error: ' + msg);
    return { ok: false, error: msg };
  }
}

async function waitForWhatsAppWebStore(client) {
  try {
    if (!client) return true;
    // BAILEYS MIGRATION: Baileys doesn't use Puppeteer, so no Store to wait for
    // Just check if client is ready
    if (client.isReady) {
      return true;
    }
    // Fallback: small delay so early sends don't race init.
    await new Promise(r => setTimeout(r, 2500));
    return true;
  } catch {
    // best-effort wait only
    return false;
  }
}

module.exports = { setClient, getClient, sendRaw, send, normalizeNumber, sendMedia };
