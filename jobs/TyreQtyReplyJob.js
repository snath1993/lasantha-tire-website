/**
 * Tyre Quantity Reply Job
 * Handles tyre stock/quantity inquiries
 */

const { extractTyreSizeFlexible } = require('../utils/detect');

/**
 * Handle tyre quantity inquiry
 * @param {object} msg - WhatsApp message
 * @param {object} sql - SQL module
 * @param {object} sqlConfig - SQL configuration
 * @param {Array} allowedContacts - Allowed contacts
 * @param {Function} logAndSave - Logging function
 * @returns {Promise<boolean>} - True if handled
 */
async function TyreQtyReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    try {
        const text = msg.body ? msg.body.trim() : '';
        const tyreSize = extractTyreSizeFlexible(text);
        
        if (!tyreSize) {
            return false; // Not a tyre size query
        }
        
        // Check if it's a stock/quantity inquiry
        const lowerText = text.toLowerCase();
        const isQtyQuery = lowerText.includes('stock') || lowerText.includes('available') || 
                           lowerText.includes('තියෙනවද') || lowerText.includes('qty');
        
        if (!isQtyQuery) {
            return false; // Not a quantity query
        }
        
        if (logAndSave) {
            logAndSave(`[TyreQtyReplyJob] Processing quantity inquiry for ${tyreSize}`);
        }
        
        // This job is now handled by the AI workflow in the main handler
        // Return false to let the AI workflow handle it
        return false;
        
    } catch (error) {
        if (logAndSave) {
            logAndSave(`[TyreQtyReplyJob] Error: ${error.message}`);
        }
        return false;
    }
}

module.exports = TyreQtyReplyJob;
