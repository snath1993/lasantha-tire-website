// utils/brandUtils.js
// Utility for brand name normalization

// Brand name mapping to handle spelling variations, trailing spaces, and unify brand names
const BRAND_NAME_MAPPING = {
    'MAXXIES': 'MAXXIS',
    'GOOD YEAR': 'GOODYEAR',
    'GT CHINA': 'GT',
    'GT ': 'GT',             // GT with trailing space
    'CEAT MOTOR BIKE': 'CEAT MOTORBIKE',
    'MAXXIES ': 'MAXXIS',
};

function normalizeBrand(brand) {
    if (!brand) return '';
    const normalized = brand.trim().toUpperCase();
    // Apply brand name mapping if exists
    return BRAND_NAME_MAPPING[normalized] || normalized;
}

module.exports = { normalizeBrand, BRAND_NAME_MAPPING };