/**
 * Tyre Price Reply Job
 * Handles tyre price inquiries
 */

const { extractTyreSizeFlexible } = require('../utils/detect');
const safeReply = require('../utils/safeReply');

/**
 * Handle tyre price inquiry
 * @param {object} msg - WhatsApp message
 * @param {object} sql - SQL module
 * @param {object} sqlConfig - SQL configuration
 * @param {Array} allowedContacts - Allowed contacts
 * @param {Function} logAndSave - Logging function
 * @returns {Promise<boolean>} - True if handled
 */
async function TyrePriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    try {
        const text = msg.body ? msg.body.trim() : '';
        const tyreSize = extractTyreSizeFlexible(text);
        
        if (!tyreSize) {
            return false; // Not a tyre size query
        }
        
        // Check if it's a simple price inquiry
        const lowerText = text.toLowerCase();
        const isPriceQuery = lowerText.includes('price') || lowerText.includes('මිල') || lowerText.includes('කියද');
        
        if (!isPriceQuery) {
            return false; // Not a price query
        }
        
        logAndSave(`[TyrePriceReplyJob] Processing price inquiry for ${tyreSize}`);
        
        // This job is now handled by the AI workflow in the main handler
        // Return false to let the AI workflow handle it
        return false;
        
    } catch (error) {
        if (logAndSave) {
            logAndSave(`[TyrePriceReplyJob] Error: ${error.message}`);
        }
        return false;
    }
}

module.exports = TyrePriceReplyJob;
