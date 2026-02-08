const fs = require('fs');
const path = require('path');

const SPECS_FILE = path.join(__dirname, '..', 'brand-pattern-specs.json');

function normalize(s) {
  return String(s || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
}

function loadPatternSpecs() {
  try {
    if (!fs.existsSync(SPECS_FILE)) return { brands: [] };
    const raw = fs.readFileSync(SPECS_FILE, 'utf-8');
    const data = JSON.parse(raw);
    if (!data || !Array.isArray(data.brands)) return { brands: [] };
    return data;
  } catch (e) {
    console.warn('PatternSpecs: failed to load specs:', e.message);
    return { brands: [] };
  }
}

function findPatternSpec(brand, pattern) {
  const data = loadPatternSpecs();
  const bn = normalize(brand);
  const pn = normalize(pattern);
  const brandEntry = data.brands.find(b => normalize(b.brand) === bn || normalize(b.brand).includes(bn) || bn.includes(normalize(b.brand)));
  if (!brandEntry || !Array.isArray(brandEntry.patterns)) return null;
  // exact/contains match
  const exact = brandEntry.patterns.find(p => normalize(p.pattern) === pn);
  if (exact) return { brand: brandEntry.brand, ...exact };
  const contains = brandEntry.patterns.find(p => pn && normalize(p.pattern).includes(pn));
  if (contains) return { brand: brandEntry.brand, ...contains };
  // fallback: first pattern marked default
  const def = brandEntry.patterns.find(p => p.default === true);
  return def ? { brand: brandEntry.brand, ...def } : null;
}

module.exports = { loadPatternSpecs, findPatternSpec };
