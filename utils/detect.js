// Centralized detection/helper functions

function extractTyreSizeFlexible(text) {
    if (!text || typeof text !== 'string') return null;
    // Normalize separators to single space (preserve dots so decimals like 10.50 remain)
    const cleaned = text.trim().replace(/[\s\/_\-]+/g, ' ');
    // Extract all numeric tokens (supports decimals in the middle token)
    const nums = cleaned.match(/\d+(?:\.\d+)?/g);
    if (!nums || nums.length === 0) return null;

    // If we have 3 numeric tokens, treat them as width/profile/rim
    if (nums.length >= 3) {
        // Keep token strings (preserve decimals if present) but also parse for validation
        const [wRaw, pRaw, rRaw] = nums.slice(0, 3).map(n => n.replace(/^0+/, ''));
        const width = parseFloat(wRaw);
        const second = parseFloat(pRaw);
        const third = parseFloat(rRaw);

        // Special-case: inputs like "155/12 8PR" -> collapse to 2-part size "155/12"
        // Heuristic: if second token is a plausible profile/rim (<=20) and third token looks like a ply/load marker (<10),
        // then ignore the third token. Ensure we don't break valid 3-part sizes like 31/10.50/15 (third = 15 >= 10).
        if (second <= 20 && third < 10) {
            if (width >= 20 && width <= 400 && second >= 5) {
                return `${wRaw}/${pRaw}`;
            }
        }

        // Basic sanity checks for tyre sizes (three-part)
        // Allow smaller widths for motorbike tyres (e.g., 31)
        if (width >= 20 && width <= 400 && second >= 5 && second <= 120 && third >= 8 && third <= 30) {
            return `${wRaw}/${pRaw}/${rRaw}`;
        }
    }

    // If only two numbers, treat as width/profile
    if (nums.length >= 2) {
        const [wRaw, pRaw] = nums.slice(0, 2).map(n => n.replace(/^0+/, ''));
        const width = parseFloat(wRaw);
        const second = parseFloat(pRaw);
        // Widen width range to support truck/bias formats like 700/16 and 750/16
        // Accept second token in a reasonable range (either profile-like 5..120 or rim-like 8..30)
        if (width >= 20 && width <= 1500 && ((second >= 5 && second <= 120) || (second >= 8 && second <= 30))) {
            return `${wRaw}/${pRaw}`;
        }
    }

    return null;
}

function extractVehicleNumber(text) {
    // Support: 2-3 letters or 1-3 digits, optional dash/space, then 3-4 digits
    // Examples: CBH-6483, CBH 6483, CBH6483, 64-6483, 64 6483, 646483
    const m = text.match(/\b([A-Z]{2,3}|\d{1,3})[-\s]?(\d{3,4})\b/i);
    if (!m) return null;
    const prefix = m[1].toUpperCase();
    const suffix = m[2].toUpperCase();
    if ((/^[A-Z]{2,3}$/.test(prefix) || /^\d{1,3}$/.test(prefix)) && /^\d{3,4}$/.test(suffix)) {
        // Normalize to DASHED uppercase format for consistent DB queries
        return `${prefix}-${suffix}`;
    }
    return null;
}

/**
 * Extract a small requested quantity (1-5) from text, avoiding tyre size numbers.
 * If a tyreSize is provided, it will be stripped before detection.
 */
function extractRequestedQty(text, tyreSize = null) {
    if (!text || typeof text !== 'string') return null;
    let t = text.toUpperCase();
    if (tyreSize) {
        const norm = String(tyreSize).toUpperCase().replace(/\s+/g, '');
        t = t.replace(new RegExp(norm.replace(/[-/]/g, '[\\-\\/]'), 'g'), ' ');
    }
    // Common tokens that imply quantity context (optional, for extra precision)
    // e.g., "tyre 2", "2 tyres", "pair", "දෙකක්", "1-5"
    const qtyMatch = t.match(/\b([1-5])\b(?!\s*R\d+)/);
    if (qtyMatch) return parseInt(qtyMatch[1], 10);
    // Sinhala words (basic): ekak(1), dekak(2), tunak(3), hatarak(4), pahak(5)
    const siMap = { 'EKAK':1, 'DEKAK':2, 'TUNAK':3, 'HATARAK':4, 'PAHAK':5 };
    for (const [k,v] of Object.entries(siMap)) {
        if (t.includes(k)) return v;
    }
    return null;
}

/**
 * Detect explicit detail requests: stock counts, brand, pattern, or general details.
 */
function extractDetailRequests(text) {
    if (!text || typeof text !== 'string') return { stock:false, brand:false, pattern:false, details:false };
    const t = text.toLowerCase();
    const stock = /(stock|qty|quantity|in stock|available|availability|තොග|තියෙනවද|තියෙනවාද)/i.test(t);
    const brand = /(brand|make|marka|මාර්කා|බ්‍රැන්ඩ්|බ්‍රෑන්ඩ්)/i.test(t);
    const pattern = /(pattern|tread|model|පැටන්|පැටර්න්)/i.test(t);
    const details = /(details|info|warranty|waranty|warenty|විස්තර|වගකීම)/i.test(t);
    return { stock, brand, pattern, details };
}

module.exports = {
    extractTyreSizeFlexible,
    extractVehicleNumber,
    extractRequestedQty,
    extractDetailRequests
};
