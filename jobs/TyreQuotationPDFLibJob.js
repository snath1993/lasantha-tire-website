/**
 * Tyre Quotation PDF Job
 * Generates PDF quotations for tyre inquiries
 */

const { extractTyreSizeFlexible } = require('../utils/detect');

/**
 * Handle quotation PDF generation
 * @param {object} msg - WhatsApp message
 * @param {object} sql - SQL module
 * @param {object} sqlConfig - SQL configuration
 * @param {Function} logAndSave - Logging function
 * @returns {Promise<boolean>} - True if handled
 */
async function TyreQuotationPDFLibJob(msg, sql, sqlConfig, logAndSave) {
    try {
        const text = msg.body ? msg.body.trim() : '';
        const tyreSize = extractTyreSizeFlexible(text);
        
        if (!tyreSize) {
            return false; // Not a tyre size query
        }
        
        // Check if it's a quotation request
        const lowerText = text.toLowerCase();
        const isQuotationRequest = lowerText.includes('quotation') || lowerText.includes('pdf') || 
                                   lowerText.includes('quote');
        
        if (!isQuotationRequest) {
            return false; // Not a quotation request
        }
        
        if (logAndSave) {
            logAndSave(`[TyreQuotationPDFLibJob] Processing quotation request for ${tyreSize}`);
        }
        
        // This job is now handled by the AI workflow in the main handler
        // Return false to let the AI workflow handle it
        return false;
        
    } catch (error) {
        if (logAndSave) {
            logAndSave(`[TyreQuotationPDFLibJob] Error: ${error.message}`);
        }
        return false;
    }
}

module.exports = TyreQuotationPDFLibJob;
