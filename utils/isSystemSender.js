/**
 * Check if a message is from a system/broadcast sender
 * @param {string} from - WhatsApp sender ID
 * @returns {boolean} - True if system sender
 */
function isSystemSender(from) {
    if (!from) return true;
    
    // Filter out status broadcasts and groups announcements
    if (from === 'status@broadcast') return true;
    if (from.includes('broadcast')) return true;
    
    return false;
}

module.exports = isSystemSender;
