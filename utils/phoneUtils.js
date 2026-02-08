/**
 * Phone Number Utilities
 * Shared phone number normalization and validation functions
 * to avoid duplicate code across jobs
 */

/**
 * Normalize Sri Lankan phone number to WhatsApp format
 * Converts various formats to 94XXXXXXXXX@c.us
 * 
 * @param {string} phone - Phone number in any format
 * @returns {string} Normalized phone number (without @c.us suffix)
 * 
 * @example
 * normalizePhone('0777078700') // returns '94777078700'
 * normalizePhone('+94777078700') // returns '94777078700'
 * normalizePhone('94777078700') // returns '94777078700'
 * normalizePhone('0094777078700') // returns '94777078700'
 */
function normalizePhone(phone) {
    if (!phone) return '';
    
    // Convert to string and remove all non-digit characters
    let cleaned = String(phone).replace(/\D/g, '');
    
    // Handle different formats
    if (cleaned.startsWith('0094')) {
        // International format with 00
        cleaned = cleaned.substring(2);
    } else if (cleaned.startsWith('94') && cleaned.length >= 11) {
        // Already in 94 format
        // Keep as is
    } else if (cleaned.startsWith('0') && cleaned.length === 10) {
        // Local format: 0771234567 -> 94771234567
        cleaned = '94' + cleaned.substring(1);
    } else if (cleaned.length === 9 && !cleaned.startsWith('0')) {
        // Just the number without prefix: 771234567 -> 94771234567
        cleaned = '94' + cleaned;
    }
    
    return cleaned;
}

/**
 * Convert phone to WhatsApp chat ID format
 * @param {string} phone - Phone number
 * @returns {string} WhatsApp chat ID (e.g., '94777078700@c.us')
 */
function toWhatsAppId(phone) {
    const normalized = normalizePhone(phone);
    if (!normalized) return '';
    return normalized + '@c.us';
}

/**
 * Validate Sri Lankan phone number
 * @param {string} phone - Phone number to validate
 * @returns {boolean} True if valid Sri Lankan number
 */
function isValidSLPhone(phone) {
    const normalized = normalizePhone(phone);
    // Sri Lankan numbers: 94 + 9 digits (total 11)
    // Mobile prefixes: 70, 71, 72, 74, 75, 76, 77, 78
    return /^94[7][0-8]\d{7}$/.test(normalized);
}

/**
 * Extract phone number from WhatsApp message author/from field
 * @param {string} waId - WhatsApp ID (e.g., '94777078700@c.us' or '94777078700:123@g.us')
 * @returns {string} Clean phone number
 */
function extractPhoneFromWaId(waId) {
    if (!waId) return '';
    // Remove @c.us, @s.whatsapp.net, @g.us suffixes
    // Handle group participant format: 94xxx:123@g.us
    return String(waId)
        .split('@')[0]
        .split(':')[0]
        .replace(/\D/g, '');
}

/**
 * Format phone for display (Sri Lankan format)
 * @param {string} phone - Phone number
 * @returns {string} Formatted display string (e.g., '077-7078700')
 */
function formatPhoneDisplay(phone) {
    const normalized = normalizePhone(phone);
    if (!normalized || normalized.length !== 11) return phone;
    
    // Convert 94771234567 to 077-1234567
    const local = '0' + normalized.substring(2);
    return local.substring(0, 3) + '-' + local.substring(3);
}

/**
 * Get admin numbers from environment or config
 * @param {string} envKey - Environment variable name (default: ADMIN_NUMBERS)
 * @param {string[]} fallback - Fallback numbers if env not set
 * @returns {string[]} Array of normalized admin phone numbers
 */
function getAdminNumbers(envKey = 'ADMIN_NUMBERS', fallback = []) {
    const envValue = process.env[envKey];
    if (envValue) {
        return envValue
            .split(',')
            .map(p => normalizePhone(p.trim()))
            .filter(p => p.length > 0);
    }
    return fallback.map(p => normalizePhone(p)).filter(p => p.length > 0);
}

module.exports = {
    normalizePhone,
    toWhatsAppId,
    isValidSLPhone,
    extractPhoneFromWaId,
    formatPhoneDisplay,
    getAdminNumbers
};
