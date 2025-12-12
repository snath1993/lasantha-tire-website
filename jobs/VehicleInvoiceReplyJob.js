/**
 * Vehicle Invoice Reply Job
 * Handles vehicle invoice lookup
 */

const { extractVehicleNumber } = require('../utils/detect');

/**
 * Handle vehicle invoice lookup
 * @param {object} msg - WhatsApp message
 * @param {object} pool - SQL connection pool
 * @param {Array} allowedContacts - Allowed contacts
 * @param {Function} logAndSave - Logging function
 * @returns {Promise<boolean>} - True if handled
 */
async function VehicleInvoiceReplyJob(msg, pool, allowedContacts, logAndSave) {
    try {
        const text = msg.body ? msg.body.trim() : '';
        const vehicleNumber = extractVehicleNumber(text);
        
        if (!vehicleNumber) {
            return false; // Not a vehicle number
        }
        
        if (logAndSave) {
            logAndSave(`[VehicleInvoiceReplyJob] Processing invoice lookup for ${vehicleNumber}`);
        }
        
        // Mock response - in production this would query the database
        await msg.reply(`Looking up invoices for vehicle: ${vehicleNumber}...\n\nPlease contact 0777311770 for invoice details.`);
        
        return true;
        
    } catch (error) {
        if (logAndSave) {
            logAndSave(`[VehicleInvoiceReplyJob] Error: ${error.message}`);
        }
        return false;
    }
}

module.exports = VehicleInvoiceReplyJob;
