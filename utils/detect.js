/**
 * Detection utilities for extracting information from messages
 */

/**
 * Extract tyre size from text (flexible patterns)
 * @param {string} text - Text to search
 * @returns {string|null} - Tyre size or null
 */
function extractTyreSizeFlexible(text) {
    if (!text) return null;
    
    // Common tyre size patterns: 195/65R15, 195/65/15, 195-65-15, etc.
    const patterns = [
        /(\d{3})[\s\/\-](\d{2})[\s\/\-]?[rR]?[\s\/\-]?(\d{2})/,  // 195/65R15 or 195/65/15
        /(\d{3})[\s\/\-](\d{2})[\s\/\-](\d{2})/,                   // 195-65-15
    ];
    
    for (const pattern of patterns) {
        const match = text.match(pattern);
        if (match) {
            return `${match[1]}/${match[2]}R${match[3]}`;
        }
    }
    
    return null;
}

/**
 * Extract vehicle number from text
 * @param {string} text - Text to search
 * @returns {string|null} - Vehicle number or null
 */
function extractVehicleNumber(text) {
    if (!text) return null;
    
    // Sri Lankan vehicle number patterns: ABC-1234, ABC1234, WP ABC-1234
    const patterns = [
        /([A-Z]{2,3}[\s\-]?\d{4})/i,           // ABC-1234 or ABC1234
        /([A-Z]{2}\s+[A-Z]{2,3}[\s\-]?\d{4})/i // WP ABC-1234
    ];
    
    for (const pattern of patterns) {
        const match = text.match(pattern);
        if (match) {
            return match[1].toUpperCase();
        }
    }
    
    return null;
}

/**
 * Extract requested quantity from text
 * @param {string} text - Text to search
 * @param {string} tyreSize - Tyre size for context
 * @returns {number|null} - Quantity or null
 */
function extractRequestedQty(text, tyreSize) {
    if (!text) return null;
    
    // Look for numbers that might be quantities
    const qtyPatterns = [
        /(\d+)\s*(pcs|pieces|tyres?|tires?|units?)/i,  // "4 tyres"
        /need\s+(\d+)/i,                                  // "need 4"
        /(\d+)\s*ක/,                                      // "4 ක" (Sinhala)
    ];
    
    for (const pattern of qtyPatterns) {
        const match = text.match(pattern);
        if (match) {
            const qty = parseInt(match[1], 10);
            if (qty > 0 && qty <= 100) {
                return qty;
            }
        }
    }
    
    return null;
}

/**
 * Extract detail requests from text
 * @param {string} text - Text to search
 * @returns {object} - Object with detail flags
 */
function extractDetailRequests(text) {
    if (!text) return {};
    
    const lowerText = text.toLowerCase();
    
    return {
        wantsStock: lowerText.includes('stock') || lowerText.includes('available') || lowerText.includes('තියෙනවද'),
        wantsPrice: lowerText.includes('price') || lowerText.includes('cost') || lowerText.includes('මිල'),
        wantsSpecs: lowerText.includes('spec') || lowerText.includes('detail') || lowerText.includes('විස්තර'),
    };
}

module.exports = {
    extractTyreSizeFlexible,
    extractVehicleNumber,
    extractRequestedQty,
    extractDetailRequests
};
