/**
 * Multilingual Natural Language Understanding (NLU)
 * Supports: Sinhala, Tamil, English
 * 
 * Features:
 * - Intent detection across 3 languages
 * - Entity extraction (tyre size, brand, quantity)
 * - Sentiment analysis
 * - Language detection
 * - Typo tolerance
 */

// Language-specific intent patterns
const INTENT_PATTERNS = {
    photo: {
        english: [
            'photo', 'image', 'picture', 'pic', 'show', 'send me', 'can you send',
            'display', 'view', 'look', 'see', 'visual', 'appearance', 'looks like'
        ],
        sinhala: [
            'පින්තූර', 'පින්තූරය', 'ඡායාරූප', 'පින්න', 'පට', 'එවන්න', 'දාන්න', 'යවන්න', 'පෙන්නන්න',
            'පෙනුම', 'විදිහ', 'වගේ', 'බලන්න', 'පෙන්වන්න', 'පෙන්වන්න', 'පෙන්නන', 'පෙනෙනකොට'
        ],
        tamil: [
            'படம்', 'புகைப்படம்', 'அனுப்பு', 'காட்டு', 'போட்டோ', 'பார்க்க', 'அனுப்பவும்'
        ],
        variations: [
            'poto', 'foto', 'phto', 'pict', 'ewanna', 'yawanna', 'pennanna', 'dannana',
            'පනතර', 'පනතරය', 'එවන', 'යවන', 'පනන', 'send photo', 'photo eka', 'පින්තූරයක්'
        ]
    },
    
    price: {
        english: [
            'price', 'cost', 'how much', 'rate', 'amount', 'total', 'value',
            'charge', 'fee', 'expensive', 'cheap', 'pay', 'payment', 'money',
            'affordable', 'budget', 'spend', 'worth'
        ],
        sinhala: [
            'ගාන', 'ගානේ', 'ගානය', 'කීයද', 'කියද', 'කියනවද', 'මිල', 'වගේ', 'වගෙ', 'කීයක්ද',
            'ගණන', 'මුදල', 'ගෙවන්න', 'කීය', 'කීයද', 'ගානක්', 'මිලයක්', 'වියදම',
            'අගය', 'කොච්චර', 'කොච්චරද', 'ගාන කියපන්', 'මුදලක්', 'ගාන කියන්න'
        ],
        tamil: [
            'விலை', 'எவ்வளவு', 'கூறு', 'தொகை', 'செலுத்த', 'பணம்', 'விலை என்ன'
        ],
        variations: [
            'kiyada', 'keeyada', 'gaana', 'mil', 'wage', 'gana', 'කීය', 'ගන',
            'how much this', 'මිල කියන්න', 'ගාන කියපන', 'price eka'
        ]
    },
    
    stock: {
        english: [
            'stock', 'available', 'in stock', 'have', 'got', 'quantity', 'qty',
            'availability', 'can get', 'can buy', 'ready', 'supply', 'inventory',
            'do you have', 'is it available', 'get', 'obtain'
        ],
        sinhala: [
            'තියෙනවද', 'තියෙනවා', 'තියයනවද', 'තියනවද', 'ඇත්තද', 'තියෙද', 'ස්ටොක්', 'ගණන',
            'ගන්න පුළුවන්ද', 'ලබාගන්න', 'ගන්න', 'තියෙනවා', 'තිබෙනවද', 'තිබෙනවා',
            'තියයනවද', 'තියෙද', 'තිබෙද', 'තිබෙනවද', 'තියයනා', 'තිබේද', 'අපේ ගාව'
        ],
        tamil: [
            'இருப்பு', 'கிடைக்கும்', 'உள்ளதா', 'அளவு', 'வாங்க', 'கிடைக்கும்'
        ],
        variations: [
            'thiyenawada', 'athdhada', 'stock', 'qty', 'තියද', 'තියන',
            'have stock', 'තිබෙනවද', 'ගන්න පුළුවන්ද', 'available da'
        ]
    },
    
    quotation: {
        english: [
            'quotation', 'quote', 'estimate', 'bill', 'invoice', 'receipt',
            'estimation', 'cost breakdown', 'detailed price', 'full price'
        ],
        sinhala: [
            'ඇස්තමේන්තුව', 'ඇස්තමේන්තු', 'කෝටේෂන්', 'බිල', 'ඉස්ටිමේට්',
            'ඇස්තමේන්තුවක්', 'බිල්පත', 'ඇස්තමේන්තු එකක්'
        ],
        tamil: [
            'மதிப்பீடு', 'கோட்', 'பில்', 'மதிப்பீடு வேண்டும்'
        ],
        variations: [
            'asthamenthuwa', 'quotation', 'estimate', 'බිලක්', 'ඇස්තමේන්තු eka'
        ]
    },
    
    greeting: {
        english: [
            'hi', 'hello', 'hey', 'good morning', 'good evening', 'good afternoon',
            'greetings', 'howdy', 'sup', "what's up", 'yo', 'hiya'
        ],
        sinhala: [
            'හලෝ', 'හායි', 'ආයුබෝවන්', 'සුබ උදෑසනක්', 'කොහොමද', 'සුබ දවසක්',
            'සුබ සන්ධ්‍යාවක්', 'කොහොමද ඉන්නේ', 'හායි', 'හෙලෝ'
        ],
        tamil: [
            'வணக்கம்', 'ஹலோ', 'நல்ல காலை', 'நல்ல மாலை', 'எப்படி இருக்கிறீர்கள்'
        ],
        variations: [
            'halo', 'helo', 'hii', 'hiii', 'හලො', 'හයි', 'kohomada'
        ]
    },
    
    thanks: {
        english: [
            'thank', 'thanks', 'thank you', 'appreciate', 'grateful', 'thx',
            'ty', 'tq', 'much appreciated', 'thanks a lot', 'many thanks'
        ],
        sinhala: [
            'ස්තූතියි', 'ස්තූති', 'බොහොම', 'ස්තුතියි', 'බොහොමයි', 'ස්තූතියි බොහොම',
            'තැන්ක්ස්', 'ස්තුති', 'බොහොම ස්තූතියි', 'ස්තූති බොහොම', 'බොහොම පින්'
        ],
        tamil: [
            'நன்றி', 'மிக்க நன்றி', 'நன்றி நன்றி', 'நன்றிகள்'
        ],
        variations: [
            'sthuthi', 'stuthy', 'thanks', 'thanku', 'thank u', 'ස්තුති', 'බොහොම'
        ]
    },
    
    warranty: {
        english: [
            'warranty', 'guarantee', 'defect', 'problem', 'issue', 'fault',
            'coverage', 'warrantee', 'protection', 'assurance', 'broken'
        ],
        sinhala: [
            'වොරන්ටි', 'වරන්ටි', 'ගැරන්ටි', 'දෝෂ', 'ගැටළු', 'ප්‍රශ්න', 'කැඩුනා',
            'වොරන්ටි කාලය', 'ගැරන්ටි එක', 'වරන්ටි එක', 'ආරක්ෂාව'
        ],
        tamil: [
            'உத்தரவாதம்', 'பிரச்சினை', 'உத்தரவாத காலம்', 'குறை'
        ],
        variations: [
            'warranty', 'waranty', 'guarantee', 'වරන්ටි', 'වොරන්ටි එක'
        ]
    }
};

// Brand name variations (multi-language + typos)
const BRAND_PATTERNS = {
    'MAXXIS': ['maxxis', 'maxxies', 'මැක්සිස්', 'மாக்சிஸ்', 'maxsis', 'maksis'],
    'DURATURN': ['duraturn', 'dura turn', 'ඩුරටර්න්', 'டுரடர்ன்'],
    'YOKOHAMA': ['yokohama', 'යොකොහාමා', 'யோகோஹாமா', 'yokohamma'],
    'BRIDGESTONE': ['bridgestone', 'bridge stone', 'බ්‍රිජ්ස්ටෝන්', 'பிரிட்ஜ்ஸ்டோன்'],
    'CONTINENTAL': ['continental', 'කොන්ටිනෙන්ටල්', 'காண்டினென்டல்'],
    'MICHELIN': ['michelin', 'මිශලින්', 'மைக்லின்'],
    'GOODYEAR': ['goodyear', 'good year', 'ගුඩ්ඊයර්', 'குட்இயர்'],
    'CEAT': ['ceat', 'සීට්', 'சீட்'],
    'GITI': ['giti', 'ගිටි', 'கிட்டி'],
    'FEDERAL': ['federal', 'ෆෙඩරල්', 'பெடரல்'],
    'KUMHO': ['kumho', 'කුම්හෝ', 'கும்ஹோ'],
    'TOYO': ['toyo', 'ටෝයෝ', 'டோயோ'],
    'NEXEN': ['nexen', 'නෙක්සන්', 'நெக்ஸன்'],
    'PIRELLI': ['pirelli', 'පිරලී', 'பிரெல்லி'],
    'DUNLOP': ['dunlop', 'ඩන්ලොප්', 'டன்லாப்'],
    'HANKOOK': ['hankook', 'හැන්කුක්', 'ஹான்கூக்']
};

/**
 * Detect language of text
 * @param {string} text 
 * @returns {string} 'sinhala' | 'tamil' | 'english' | 'mixed'
 */
function detectLanguage(text) {
    const sinhalaChars = (text.match(/[\u0D80-\u0DFF]/g) || []).length;
    const tamilChars = (text.match(/[\u0B80-\u0BFF]/g) || []).length;
    const englishWords = (text.match(/\b[a-zA-Z]+\b/g) || []).length;
    
    const total = sinhalaChars + tamilChars + englishWords;
    if (total === 0) return 'unknown';
    
    const sinhalaPercent = (sinhalaChars / total) * 100;
    const tamilPercent = (tamilChars / total) * 100;
    const englishPercent = (englishWords / total) * 100;
    
    if (sinhalaPercent > 40) return sinhalaPercent > 70 ? 'sinhala' : 'mixed';
    if (tamilPercent > 40) return tamilPercent > 70 ? 'tamil' : 'mixed';
    if (englishPercent > 40) return 'english';
    
    return 'mixed';
}

/**
 * Calculate semantic similarity between two strings
 * Uses character n-grams for fuzzy matching
 * @param {string} str1 
 * @param {string} str2 
 * @returns {number} Similarity score (0-1)
 */
function calculateSimilarity(str1, str2) {
    const s1 = str1.toLowerCase();
    const s2 = str2.toLowerCase();
    
    // Exact match
    if (s1 === s2) return 1.0;
    
    // Contains check
    if (s1.includes(s2) || s2.includes(s1)) return 0.8;
    
    // Character n-gram similarity (trigrams)
    const getNGrams = (str, n = 3) => {
        const ngrams = [];
        for (let i = 0; i <= str.length - n; i++) {
            ngrams.push(str.substring(i, i + n));
        }
        return ngrams;
    };
    
    const ngrams1 = getNGrams(s1);
    const ngrams2 = getNGrams(s2);
    
    if (ngrams1.length === 0 || ngrams2.length === 0) {
        // Fallback to character overlap
        const chars1 = new Set(s1.split(''));
        const chars2 = new Set(s2.split(''));
        const intersection = [...chars1].filter(c => chars2.has(c)).length;
        const union = new Set([...chars1, ...chars2]).size;
        return union > 0 ? intersection / union : 0;
    }
    
    const set1 = new Set(ngrams1);
    const set2 = new Set(ngrams2);
    const intersection = [...set1].filter(ng => set2.has(ng)).length;
    const union = set1.size + set2.size - intersection;
    
    return union > 0 ? intersection / union : 0;
}

/**
 * Extract intent from text (multi-language aware with semantic matching)
 * @param {string} text 
 * @returns {Object} { intent: string, confidence: number, language: string }
 */
function extractIntent(text) {
    const normalizedText = text.toLowerCase().trim();
    const language = detectLanguage(text);
    
    const scores = {};
    
    // Check each intent
    for (const [intentName, patterns] of Object.entries(INTENT_PATTERNS)) {
        let score = 0;
        
        // Check all language patterns
        const allPatterns = [
            ...patterns.english,
            ...patterns.sinhala,
            ...patterns.tamil,
            ...(patterns.variations || [])
        ];
        
        for (const pattern of allPatterns) {
            const patternLower = pattern.toLowerCase();
            
            // Exact or substring match
            if (normalizedText.includes(patternLower)) {
                // Boost score if pattern matches detected language
                const boost = (
                    (language === 'english' && patterns.english.includes(pattern)) ||
                    (language === 'sinhala' && patterns.sinhala.includes(pattern)) ||
                    (language === 'tamil' && patterns.tamil.includes(pattern))
                ) ? 2 : 1;
                
                score += boost;
            } else {
                // Fuzzy matching for typos and similar words
                const words = normalizedText.split(/\s+/);
                for (const word of words) {
                    const similarity = calculateSimilarity(word, patternLower);
                    if (similarity >= 0.7) { // 70% similar = likely match
                        score += similarity;
                    }
                }
            }
        }
        
        if (score > 0) {
            scores[intentName] = score;
        }
    }
    
    // Find highest scoring intent
    if (Object.keys(scores).length === 0) {
        return { intent: null, confidence: 0, language };
    }
    
    const sortedIntents = Object.entries(scores)
        .sort((a, b) => b[1] - a[1]);
    
    const [topIntent, topScore] = sortedIntents[0];
    const totalScore = Object.values(scores).reduce((a, b) => a + b, 0);
    const confidence = Math.min(100, (topScore / totalScore) * 100);
    
    return {
        intent: topIntent,
        confidence: Math.round(confidence),
        language,
        allScores: scores
    };
}

/**
 * Extract brand from text (multi-language + typo tolerant + semantic matching)
 * @param {string} text 
 * @returns {string|null} Normalized brand name
 */
function extractBrand(text) {
    const normalizedText = text.toLowerCase();
    
    for (const [brand, variations] of Object.entries(BRAND_PATTERNS)) {
        for (const variation of variations) {
            if (normalizedText.includes(variation)) {
                return brand;
            }
        }
    }
    
    return null;
}

/**
 * Extract quantity from text (multi-language)
 * @param {string} text 
 * @returns {number|null}
 */
function extractQuantity(text) {
    // English: "4 tyres", "2 pcs"
    const engMatch = text.match(/(\d+)\s*(tyre|tire|piece|pcs|unit)/i);
    if (engMatch) return parseInt(engMatch[1]);
    
    // Sinhala: "ටයර් 4", "4 ක්"
    const sinMatch = text.match(/(\d+)\s*(ටයර්|ක්|ගාන|ගානක්)/i);
    if (sinMatch) return parseInt(sinMatch[1]);
    
    // Tamil: "4 டயர்"
    const tamMatch = text.match(/(\d+)\s*டயர்/i);
    if (tamMatch) return parseInt(tamMatch[1]);
    
    // Just a number near tyre size
    const numMatch = text.match(/\b(\d+)\b/);
    if (numMatch && parseInt(numMatch[1]) <= 10) {
        return parseInt(numMatch[1]);
    }
    
    return null;
}

/**
 * Analyze sentiment (positive, neutral, negative)
 * @param {string} text 
 * @returns {Object} { sentiment: string, score: number }
 */
function analyzeSentiment(text) {
    const positive = {
        english: ['good', 'great', 'excellent', 'best', 'nice', 'perfect', 'love'],
        sinhala: ['හොඳ', 'සුපිරි', 'ලස්සන', 'වඩා', 'පට්ට'],
        tamil: ['நல்ல', 'சிறந்த', 'அருமை']
    };
    
    const negative = {
        english: ['bad', 'poor', 'worst', 'terrible', 'problem', 'issue', 'defect'],
        sinhala: ['නරක', 'වැරදි', 'ගැටළු', 'දෝෂ', 'කැඩුනා'],
        tamil: ['மோசம்', 'பிரச்சினை', 'குறை']
    };
    
    const normalizedText = text.toLowerCase();
    let score = 0;
    
    // Check positive words
    Object.values(positive).flat().forEach(word => {
        if (normalizedText.includes(word.toLowerCase())) score += 1;
    });
    
    // Check negative words
    Object.values(negative).flat().forEach(word => {
        if (normalizedText.includes(word.toLowerCase())) score -= 1;
    });
    
    const sentiment = score > 0 ? 'positive' : score < 0 ? 'negative' : 'neutral';
    
    return { sentiment, score };
}

/**
 * Complete NLU analysis
 * @param {string} text 
 * @returns {Object} Full analysis result
 */
function analyzeMessage(text) {
    const language = detectLanguage(text);
    const intentResult = extractIntent(text);
    const sentimentResult = analyzeSentiment(text);
    
    const result = {
        originalText: text,
        language: language,
        intent: intentResult.intent,
        confidence: intentResult.confidence,
        entities: {
            brand: extractBrand(text),
            quantity: extractQuantity(text)
        },
        sentiment: sentimentResult.sentiment,
        sentimentScore: sentimentResult.score,
        allIntentScores: intentResult.allScores || {},
        timestamp: new Date().toISOString()
    };
    
    return result;
}

module.exports = {
    detectLanguage,
    extractIntent,
    extractBrand,
    extractQuantity,
    analyzeSentiment,
    analyzeMessage,
    INTENT_PATTERNS,
    BRAND_PATTERNS
};
