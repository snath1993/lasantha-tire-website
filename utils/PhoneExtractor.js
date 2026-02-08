// PhoneExtractor.js - Extract Sri Lankan phone numbers from text
// Supports: 077XXXXXXX, 947XXXXXXXX, +947XXXXXXXX (with spaces/dashes)

const CLEAN = /[\s\-().]/g;
const PATTERN = /(?:\+?94|0)\s*7[0-9](?:[\s\-().]*[0-9]){7}/g;

function extractPhones(text = '') {
  const matches = text.match(PATTERN) || [];
  const out = [];
  
  for (const m of matches) {
    const raw = m.replace(CLEAN, '');
    
    // Normalize to 94XXXXXXXXX format
    let num = raw;
    if (num.startsWith('+')) num = num.slice(1);
    if (num.startsWith('0')) num = '94' + num.slice(1);
    if (!num.startsWith('94')) num = '94' + num;
    
    // Validate: must be exactly 11 digits (94 + 9 digits)
    if (/^94[0-9]{9}$/.test(num)) {
      out.push(num);
    }
  }
  
  // De-duplicate
  return [...new Set(out)];
}

module.exports = { extractPhones };
