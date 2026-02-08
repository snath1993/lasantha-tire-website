// Pattern name normalization utility
// Use this to standardize pattern names before saving to database

/**
 * Normalize pattern name for consistent matching
 * @param {string} pattern - Raw pattern name
 * @returns {string} - Normalized pattern name
 */
function normalizePatternName(pattern) {
    if (!pattern) return '';
    
    let normalized = String(pattern).trim();
    
    // Remove common prefixes/suffixes that cause duplicates
    const removePhrases = [
        ' BRAVO',
        'Bravo Series ',
        'RAZR MT ',
        'Wormdrive ',
        'Premitra 5 ',
        'Mecotra '
    ];
    
    removePhrases.forEach(phrase => {
        if (normalized.includes(phrase)) {
            normalized = normalized.replace(phrase, '');
        }
    });
    
    // Normalize dashes and spaces
    normalized = normalized.replace(/\s*-\s*/g, ''); // Remove dashes with spaces
    normalized = normalized.replace(/\s+/g, ''); // Remove all spaces
    
    // Uppercase for consistency
    normalized = normalized.toUpperCase();
    
    return normalized;
}

/**
 * Check if two pattern names match after normalization
 * @param {string} pattern1 
 * @param {string} pattern2 
 * @returns {boolean}
 */
function patternsMatch(pattern1, pattern2) {
    return normalizePatternName(pattern1) === normalizePatternName(pattern2);
}

/**
 * Find best matching pattern from a list
 * @param {string} searchPattern - Pattern to search for
 * @param {Array<string>} patternList - List of available patterns
 * @returns {string|null} - Best match or null
 */
function findBestMatch(searchPattern, patternList) {
    const normalized = normalizePatternName(searchPattern);
    
    // Exact match first
    for (const pattern of patternList) {
        if (normalizePatternName(pattern) === normalized) {
            return pattern;
        }
    }
    
    // Partial match
    for (const pattern of patternList) {
        const normPattern = normalizePatternName(pattern);
        if (normPattern.includes(normalized) || normalized.includes(normPattern)) {
            return pattern;
        }
    }
    
    return null;
}

module.exports = {
    normalizePatternName,
    patternsMatch,
    findBestMatch
};
