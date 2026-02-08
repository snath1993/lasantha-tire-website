// BrandAliases.js
// Canonical brand name normalization and aliases

const ALIASES = {
  'GOOD YEAR': 'GOODYEAR',
  'GOODYEAR': 'GOODYEAR',
  'YOKOHAMA T/H': 'YOKOHAMA',
  'YOKOHAMA TOY': 'YOKOHAMA',
  'YOKOHAMA ': 'YOKOHAMA',
  'MAXXIS MOTOR BIKE': 'MAXXIS',
  'MICHELIN MOTOR BIKE': 'MICHELIN',
  'DUNLOP MOTORBIKE': 'DUNLOP',
  'GT CHINA': 'GT',
  'GT ': 'GT',
  'D/STAR ': 'D STAR',
  'D/KING': 'D KING',
  'A PLUS': 'A PLUS',
  'POWERTRAC': 'POWERTRAC',
  'ARMSTRONG': 'ARMSTRONG',
  'LAUFENN': 'LAUFENN',
  'MARSHAL': 'MARSHAL',
  'NEXEN': 'NEXEN',
  'KUMHO': 'KUMHO',
  'BRIDGESTONE': 'BRIDGESTONE',
  'CONTINENTAL': 'CONTINENTAL',
  'PIRELLI': 'PIRELLI',
  'TOYO': 'TOYO',
  'MAXXIS': 'MAXXIS',
  'GITI DGS': 'GITI',
  'GITI': 'GITI'
};

function normalizeBrand(name) {
  if (!name) return null;
  const raw = String(name).toUpperCase().trim();
  if (raw === '-' || raw === '_' || raw === 'UNKNOWN') return null;
  // Exact alias match
  if (ALIASES[raw]) return ALIASES[raw];
  // Try strip multiple spaces
  const collapsed = raw.replace(/\s+/g, ' ');
  if (ALIASES[collapsed]) return ALIASES[collapsed];
  // Remove trailing punctuation
  const cleaned = collapsed.replace(/[^A-Z0-9\s]/g, '').trim();
  if (ALIASES[cleaned]) return ALIASES[cleaned];
  return cleaned || null;
}

module.exports = { normalizeBrand, ALIASES };
