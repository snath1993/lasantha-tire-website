// utils/tyreFilter.js
// Unified tyre filtering & classification logic shared by daily & monthly reports

const DEFAULT_EXCLUDE_KEYWORDS = [
  'REBUILD','REBUILT','REBUIL','REBUILED','REBULD','RE-BUILD','RETREAD','RE-TREAD','RE TREAD',
  'RECAP','BANDAG','BANDAGE','DAG','REMOULD','RE-MOULD','RE MOULD','REMOLD',
  'USED','SECOND','SERVICE','LABOUR','LABOR','ALIGN','BALANC','CHANGE','CHANGING','FITTING','FIT ',
  'PUNCT','REPAIR','VALVE','NUT','WASH','CLEAN','AIR ',
  'KATTA','KATTA TYRE','KATTA TYRES','KOTTA'
];

// Additional explicit service / non-tyre tokens
const SERVICE_KEYWORDS = [
  'CHECK','CHECKING','INSPECT','INSPECTION','MOUNT','MOUNTING','DISMOUNT','DISMOUNTING','ROTATION','ROTATE',
  'SWAP','REMOVE','INSTALL','CONSULTATION','ADVICE','QUOTE','QUOTATION'
];

// Hard non-tyre parts/accessories
const NON_TYRE_ITEM_KEYWORDS = [
  'TUBE','PATCH','VALVE','RIM','WHEEL','FLAP','LINER','KIT','GLUE','CEMENT','WEIGHT'
];

// Size patterns
const SIZE_REGEXES = [
  /\b\d{3}\s*[\/|-]\s*\d{2,3}\s*[\/|-]\s*\d{2}\b/,        // 195/65/15 or 145-80-13
  /\b\d{3}\s*\/\s*\d{2,3}\s*R\s*\d{2}\b/i,                // 195/65R15
  /\b\d{2,3}\s*X\s*\d{2,3}\s*\/\s*\d{2}\b/i,              // 12.5 x 80 / 18
  /\b\d{2,3}\s*\/\s*\d{2}\s*-\s*\d{2}\b/                  // 6/90-15
];

// Brand / indicator tokens used to allow tyre lines without explicit size
const TYRE_INDICATORS = [
  'BRIDGESTONE','MICHELIN','YOKOHAMA','CONTINENTAL','PIRELLI','DUNLOP','GOODYEAR','HANKOOK','KUMHO','TOYO','MAXXIS',
  'RADIAL','BIAS','STEEL','BELTED','PLY','ADVAN','CINTURATO','ENERGY','ECOPIA','SCORPION'
];

function hasSize(desc) {
  return SIZE_REGEXES.some(rx => rx.test(desc));
}

function normalizeDescription(desc='') { return desc.trim(); }

function isValidTyreLineDetailed(descRaw, options = {}) {
  const { requireSize = true, debug = false } = options;
  if (!descRaw) return { valid:false, reason:'empty' };
  const desc = normalizeDescription(descRaw);
  const upper = desc.toUpperCase();
  const tyreWord = upper.includes('TYRE') || upper.includes('TIRE');
  const sizePresent = hasSize(desc);

  const keywordLists = {
    keyword: DEFAULT_EXCLUDE_KEYWORDS,
    service: SERVICE_KEYWORDS,
    accessory: NON_TYRE_ITEM_KEYWORDS
  };
  for (const type in keywordLists) {
    const hit = keywordLists[type].find(k => upper.includes(k));
    if (hit) {
      const hardService = ['ALIGN','BALANC','CHECK','INSPECT'];
      if (sizePresent && !hardService.some(h => upper.includes(h))) {
        if (debug) console.log('[TYRE-FILTER] SIZE OVERRIDES exclusion', hit, '::', desc);
        continue; // allow this line
      }
      if (debug) console.log('[TYRE-FILTER] EXCLUDE', type, hit, '::', desc);
      return { valid:false, reason:type };
    }
  }
  if (!tyreWord && !sizePresent) return { valid:false, reason:'no-tyre-word-size' };
  if (requireSize && !sizePresent) {
    const indicator = TYRE_INDICATORS.find(b => upper.includes(b));
    if (!indicator) return { valid:false, reason:'missing-size' };
  }
  return { valid:true, reason:'ok' };
}

function isValidTyreLine(descRaw, options) { return isValidTyreLineDetailed(descRaw, options).valid; }

function classifyCategory(custom3Value) {
  if (!custom3Value) return 'Unknown';
  const c = String(custom3Value).trim();
  if (!c) return 'Unknown';
  const up = c.toUpperCase();
  const nonTyre = [...NON_TYRE_ITEM_KEYWORDS,'SERVICE','INSPECTION','CHECK','MOUNT','ROTATION','ACCESSORY','ACCESSORIES','TOOL','TOOLS'];
  if (nonTyre.some(k => up.includes(k))) return 'Service/Non-Tyre Items';
  return c; // keep original formatting
}

module.exports = {
  isValidTyreLine,
  isValidTyreLineDetailed,
  classifyCategory,
  SIZE_REGEXES,
  DEFAULT_EXCLUDE_KEYWORDS,
  SERVICE_KEYWORDS,
  NON_TYRE_ITEM_KEYWORDS,
  TYRE_INDICATORS,
  normalizeDescription,
  hasSize
};
