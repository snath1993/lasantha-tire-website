const fs = require('fs');
const path = require('path');
const { randomUUID } = require('crypto');

const queueFilePath = process.env.QUOTE_REQUEST_QUEUE_PATH
    ? path.resolve(process.env.QUOTE_REQUEST_QUEUE_PATH)
    : path.join(__dirname, '..', 'quote-request-queue.json');

async function ensureQueueFile() {
    try {
        await fs.promises.access(queueFilePath, fs.constants.F_OK);
    } catch {
        await fs.promises.writeFile(queueFilePath, '[]', 'utf8');
    }
}

async function readQueue() {
    await ensureQueueFile();
    const raw = await fs.promises.readFile(queueFilePath, 'utf8');
    try {
        const parsed = JSON.parse(raw);
        return Array.isArray(parsed) ? parsed : [];
    } catch {
        return [];
    }
}

async function writeQueue(entries) {
    const payload = JSON.stringify(entries, null, 2);
    await fs.promises.writeFile(queueFilePath, payload, 'utf8');
}

async function enqueueQuoteRequest(data) {
    const queue = await readQueue();
    const entry = {
        id: randomUUID(),
        number: data.number,
        message: data.message,
        source: data.source || 'unknown',
        meta: data.meta || {},
        createdAt: new Date().toISOString(),
        attempts: data.attempts || 0,
        lastAttemptAt: null,
        lastError: null
    };
    queue.push(entry);
    await writeQueue(queue);
    return entry;
}

async function replaceQueuedQuoteRequests(entries) {
    await writeQueue(entries);
    return entries;
}

async function upsertQuoteRequest(entry) {
    const queue = await readQueue();
    const idx = queue.findIndex((item) => item.id === entry.id);
    if (idx === -1) {
        queue.push(entry);
    } else {
        queue[idx] = entry;
    }
    await writeQueue(queue);
    return entry;
}

module.exports = {
    getQueueFilePath: () => queueFilePath,
    ensureQueueFile,
    getQueuedQuoteRequests: readQueue,
    enqueueQuoteRequest,
    replaceQueuedQuoteRequests,
    upsertQuoteRequest
};
