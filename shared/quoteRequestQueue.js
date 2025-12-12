/**
 * Quote Request Queue
 * Manages quote requests in a file-based queue
 */

const fs = require('fs');
const path = require('path');

const QUEUE_FILE = path.join(__dirname, '..', 'quote-requests.json');

/**
 * Ensure queue file exists
 */
function ensureQueueFile() {
    if (!fs.existsSync(QUEUE_FILE)) {
        fs.writeFileSync(QUEUE_FILE, JSON.stringify([]));
    }
}

/**
 * Get queue file path
 */
function getQueueFilePath() {
    return QUEUE_FILE;
}

/**
 * Enqueue a quote request
 * @param {object} request - Quote request data
 */
function enqueueQuoteRequest(request) {
    try {
        ensureQueueFile();
        const queue = JSON.parse(fs.readFileSync(QUEUE_FILE, 'utf8'));
        queue.push({
            ...request,
            timestamp: new Date().toISOString()
        });
        fs.writeFileSync(QUEUE_FILE, JSON.stringify(queue, null, 2));
        console.log('[QuoteQueue] Request enqueued');
    } catch (error) {
        console.error('[QuoteQueue] Error enqueueing:', error.message);
    }
}

/**
 * Get queued quote requests
 * @returns {Array} - List of queued requests
 */
function getQueuedQuoteRequests() {
    try {
        ensureQueueFile();
        return JSON.parse(fs.readFileSync(QUEUE_FILE, 'utf8'));
    } catch (error) {
        console.error('[QuoteQueue] Error reading queue:', error.message);
        return [];
    }
}

/**
 * Replace queued quote requests
 * @param {Array} requests - New queue
 */
function replaceQueuedQuoteRequests(requests) {
    try {
        fs.writeFileSync(QUEUE_FILE, JSON.stringify(requests, null, 2));
        console.log('[QuoteQueue] Queue replaced');
    } catch (error) {
        console.error('[QuoteQueue] Error replacing queue:', error.message);
    }
}

module.exports = {
    enqueueQuoteRequest,
    getQueuedQuoteRequests,
    replaceQueuedQuoteRequests,
    ensureQueueFile,
    getQueueFilePath
};
