// CommentClassifier.js
// Uses Claude to classify Facebook comments and extract entities, with robust fallback
const { generate } = require('./ClaudeClient');
const { extractTyreSizeFlexible, extractVehicleNumber } = require('./detect');

const BRAND_REGEX = /\b(bridgestone|michelin|yokohama|dunlop|goodyear|continental|pirelli|kumho|nexen|giti|marshal|toyo|maxxis|gt)\b/i;
const PRICE_WORDS = /(price|prise|prize|rate|cost|මිල|මිලා|මිලේ|harga|rs\.?|රු\.?)/i;

function fallbackClassify(text) {
  const isPrice = PRICE_WORDS.test(text || '');
  const size = extractTyreSizeFlexible(text) || null;
  const brand = (text.match(BRAND_REGEX)?.[0] || '').toUpperCase() || null;
  const vehicleNo = extractVehicleNumber(text) || null;
  const language = /[\u0D80-\u0DFF]/.test(text) ? 'si' : (/([A-Za-z].*[\u0D80-\u0DFF]|[\u0D80-\u0DFF].*[A-Za-z])/.test(text) ? 'mix' : 'en');
  return {
    intent: isPrice ? 'PRICE_REQUEST' : 'GENERAL',
    size,
    brand,
    vehicleNo,
    language,
    confidence: 0.45
  };
}

async function classifyComment(text) {
  try {
    const prompt = `You are a smart assistant for Lasantha Tyre Traders Facebook page.

**CRITICAL BUSINESS RULES (MUST FOLLOW):**
1. NEVER mention prices publicly - මිල public එකේ කිව්වොත් වරදියි
2. If price requested, classify as PRICE_REQUEST (we'll DM privately)
3. If vehicle number detected (e.g., CBH-6483), classify as VEHICLE_INQUIRY
4. Detect Sinhala (සිංහල), English, or Mixed language
5. Extract: tyre size (195/65R15), brand (BRIDGESTONE), vehicle number (WP-1234)

**INTENT TYPES:**
- PRICE_REQUEST: මිල/price/කීයද mentions
- AVAILABILITY: තියනවද/available/stock
- VEHICLE_INQUIRY: Vehicle number found (පොලිස් එකෙන්/invoice/purchase history)
- SERVICE_REQUEST: Wheel alignment/balancing/nitrogen
- COMPLAINT: Problem/issue/කැත/වැරදි
- PRAISE: Thanks/හොඳයි/excellent
- GENERAL: Other

Return STRICT JSON:
{
  "intent": "PRICE_REQUEST|AVAILABILITY|VEHICLE_INQUIRY|SERVICE_REQUEST|COMPLAINT|PRAISE|GENERAL",
  "size": "195/65R15 or null",
  "brand": "BRIDGESTONE or null",
  "vehicleNo": "CBH-6483 or null",
  "language": "si|en|mix",
  "confidence": 0.0-1.0,
  "reasoning": "Brief explanation in Sinhala or English"
}

Comment: "${text}"`;
    const raw = await generate({ prompt, maxTokens: 500, temperature: 0.1 });
    let json;
    try {
      const match = raw.match(/\{[\s\S]*\}/);
      json = JSON.parse(match ? match[0] : raw);
    } catch {
      return fallbackClassify(text);
    }
    // Enrich with local detectors if missing
    if (!json.size) json.size = extractTyreSizeFlexible(text);
    if (!json.vehicleNo) json.vehicleNo = extractVehicleNumber(text);
    if (!json.brand) {
      const b = (text.match(BRAND_REGEX)?.[0] || '').toUpperCase() || null;
      json.brand = b;
    }
    if (!json.language) json.language = /[\u0D80-\u0DFF]/.test(text) ? 'si' : 'en';
    if (typeof json.confidence !== 'number') json.confidence = 0.6;
    return json;
  } catch (e) {
    return fallbackClassify(text);
  }
}

module.exports = { classifyComment };
