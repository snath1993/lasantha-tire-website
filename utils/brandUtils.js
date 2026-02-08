// utils/brandUtils.js
// Utility for brand name normalization

// Brand name mapping to handle spelling variations and unify brand names
const BRAND_NAME_MAPPING = {
    'MAXXIES': 'MAXXIS',
    'GOOD YEAR': 'GOODYEAR'
};

function normalizeBrand(brand) {
    if (!brand) return '';
    const normalized = brand.trim().toUpperCase();
    // Apply brand name mapping if exists
    return BRAND_NAME_MAPPING[normalized] || normalized;
}

module.exports = { normalizeBrand, BRAND_NAME_MAPPING };