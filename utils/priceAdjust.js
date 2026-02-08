// Utility to extract price adjustment instructions from message text
// Supports: +10%, -5%, +1000/=, -500, brand, without brand, etc.

const { normalizeBrand } = require('./brandUtils');
function parsePriceAdjustments(text, vehicleNumbers = []) {
    // Remove / separators before parsing adjustments
    const cleanedText = text.replace(/\s*\/\s*/g, ' ');
    const adjustments = [];
    // Collect all numbers from vehicle numbers to ignore as adjustments
    const ignoreNumbers = [];
    for (const v of vehicleNumbers) {
        if (typeof v === 'string') {
            const nums = v.match(/\d{3,4}/g);
            if (nums) ignoreNumbers.push(...nums);
        }
    }

    // Percentage adjustments (global, but force to 'without MAXXIS/MAXXIES')
    const percentRegex = /([+-]\d{1,3})%/g;
    let match;
    while ((match = percentRegex.exec(cleanedText)) !== null) {
        if (!ignoreNumbers.includes(Math.abs(parseInt(match[1], 10)).toString())) {
            adjustments.push({
                type: 'percent',
                value: parseFloat(match[1]),
                brand: normalizeBrand('MAXXIS'),
                exclude: true // always exclude MAXXIS for global
            });
            adjustments.push({
                type: 'percent',
                value: parseFloat(match[1]),
                brand: normalizeBrand('MAXXIES'),
                exclude: true // always exclude MAXXIES for global
            });
        }
    }
    // Fixed value adjustments (global, but force to 'without MAXXIS/MAXXIES')
    const fixedRegex = /([+-]\d{1,7})\s*(?:[=/]|rs|rupees)?/gi;
    while ((match = fixedRegex.exec(cleanedText)) !== null) {
        if (!/%/.test(match[0])) {
            if (!ignoreNumbers.includes(Math.abs(parseInt(match[1], 10)).toString())) {
                adjustments.push({
                    type: 'fixed',
                    value: parseFloat(match[1]),
                    brand: normalizeBrand('MAXXIS'),
                    exclude: true // always exclude MAXXIS for global
                });
                adjustments.push({
                    type: 'fixed',
                    value: parseFloat(match[1]),
                    brand: normalizeBrand('MAXXIES'),
                    exclude: true // always exclude MAXXIES for global
                });
            }
        }
    }
    // Brand specific: e.g. yokohama +10%, 145/80/12 yokohama +10%
    const brandPercentRegex = /(\b[a-zA-Z0-9]+\b)\s*([+-]\d{1,3})%/g;
    while ((match = brandPercentRegex.exec(cleanedText)) !== null) {
        const brand = normalizeBrand(match[1]);
        if (brand === normalizeBrand('MAXXIS') || brand === normalizeBrand('MAXXIES')) continue; // ignore any adjustment for MAXXIS/MAXXIES
        if (!ignoreNumbers.includes(Math.abs(parseInt(match[2], 10)).toString())) {
            adjustments.push({
                type: 'percent',
                value: parseFloat(match[2]),
                brand: brand,
                exclude: false
            });
        }
    }
    // Brand exclude: without maxxis +10%
    const withoutBrandRegex = /without\s+(\b[a-zA-Z0-9]+\b)\s*([+-]\d{1,3})%/gi;
    while ((match = withoutBrandRegex.exec(cleanedText)) !== null) {
        if (!ignoreNumbers.includes(Math.abs(parseInt(match[2], 10)).toString())) {
            adjustments.push({
                type: 'percent',
                value: parseFloat(match[2]),
                brand: match[1].toUpperCase(),
                exclude: true
            });
        }
    }
    // Brand only (for fixed): yokohama +1000
    const brandFixedRegex = /(\b[a-zA-Z0-9]+\b)\s*([+-]\d{1,7})\s*(?:[=/]|rs|rupees)?/gi;
    while ((match = brandFixedRegex.exec(cleanedText)) !== null) {
        const brand = normalizeBrand(match[1]);
        if (brand === normalizeBrand('MAXXIS') || brand === normalizeBrand('MAXXIES')) continue; // ignore any adjustment for MAXXIS/MAXXIES
        if (!/%/.test(match[0])) {
            if (!ignoreNumbers.includes(Math.abs(parseInt(match[2], 10)).toString())) {
                adjustments.push({
                    type: 'fixed',
                    value: parseFloat(match[2]),
                    brand: brand,
                    exclude: false
                });
            }
        }
    }
    // Brand exclude (for fixed): without maxxis +1000
    const withoutBrandFixedRegex = /without\s+(\b[a-zA-Z0-9]+\b)\s*([+-]\d{1,7})\s*(?:[=/]|rs|rupees)?/gi;
    while ((match = withoutBrandFixedRegex.exec(cleanedText)) !== null) {
        if (!ignoreNumbers.includes(Math.abs(parseInt(match[2], 10)).toString())) {
            adjustments.push({
                type: 'fixed',
                value: parseFloat(match[2]),
                brand: match[1].toUpperCase(),
                exclude: true
            });
        }
    }
    return adjustments;
}

module.exports = { parsePriceAdjustments };