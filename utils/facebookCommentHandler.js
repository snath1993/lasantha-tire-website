const { replyToComment } = require('./facebookApi');
const { extractTyreSizeFlexible } = require('./detect');
const { fetchTyreData } = require('./fetchTyreData');
const { GoogleGenerativeAI } = require('@google/generative-ai');
const { storeContext, getContext } = require('./facebookConversationContext');
const fs = require('fs');
const path = require('path');

// Use public-facing phone number from environment (falls back to hardcoded if not set)
const WHATSAPP_NUMBER = process.env.STORE_PHONE || process.env.WHATSAPP_PRIMARY_NUMBER || '0721222509';
const genAI = new GoogleGenerativeAI(process.env.GEMINI_API_KEY);

// Load brand categories from config
let brandCategoriesConfig = null;
try {
    const candidatePaths = [
        path.resolve(__dirname, '..', 'brand-categories.json'),
        path.resolve(process.cwd(), 'brand-categories.json')
    ];
    let loadedFrom = null;
    for (const p of candidatePaths) {
        if (fs.existsSync(p)) {
            const raw = String(fs.readFileSync(p, 'utf8') || '').trim();
            if (!raw) continue;
            brandCategoriesConfig = JSON.parse(raw);
            loadedFrom = p;
            break;
        }
    }
    if (brandCategoriesConfig) {
        console.log('[FB Handler] Loaded brand categories config from', loadedFrom);
    } else {
        throw new Error('brand-categories.json not found or empty');
    }
} catch (error) {
    console.warn('[FB Handler] Failed to load brand-categories.json, using defaults:', error.message);
}

// ========================================
// DUPLICATE REPLY GUARD
// ========================================
const processedComments = new Map(); // Map<comment_id, timestamp>
const PROCESSED_TTL = 60 * 60 * 1000; // 1 hour

function isAlreadyProcessed(commentId) {
    const now = Date.now();
    
    // Cleanup expired entries
    for (const [id, timestamp] of processedComments.entries()) {
        if (now - timestamp > PROCESSED_TTL) {
            processedComments.delete(id);
        }
    }
    
    // Check if already processed
    if (processedComments.has(commentId)) {
        const processedAt = processedComments.get(commentId);
        const ageMinutes = Math.round((now - processedAt) / 1000 / 60);
        console.log(`[FB Handler] ⏭️  Comment ${commentId} already processed ${ageMinutes}m ago - skipping`);
        return true;
    }
    
    return false;
}

function markAsProcessed(commentId) {
    processedComments.set(commentId, Date.now());
}

// ========================================
// INTELLIGENT SCENARIO DETECTION
// ========================================
function detectCommentScenario(message, tyreSize, language) {
    const msgLower = message.toLowerCase();
    
    // Pattern detection
    const hasPrice = /price|මිල|කීයද|cost|කොච්චර|how much|ගාන/i.test(msgLower);
    const hasComparison = /better|best|compare|හොඳම|වඩා|difference|vs|versus/i.test(msgLower);
    const hasQuality = /quality|යටත්වගුව|warranty|guarantee|durable|හොඳද/i.test(msgLower);
    const hasAvailability = /available|තියෙනවද|තිබෙනවද|stock|ඇද්ද|have/i.test(msgLower);
    const isGreeting = /^(hello|hi|හෙලෝ|ආයුබෝවන්)/i.test(msgLower);
    const isThanks = /thank|ස්තූති|thanks/i.test(msgLower);
    
    // Count multiple brands mentioned
    const brands = ['maxxis', 'dunlop', 'bridgestone', 'michelin', 'giti', 'gt', 'duraturn'];
    const mentionedBrands = brands.filter(b => msgLower.includes(b));
    
    // Decision logic
    if (isGreeting) {
        return { type: 'greeting', useAI: true, queryDB: false, confidence: 95 };
    }
    
    if (isThanks) {
        return { type: 'thanks', useAI: true, queryDB: false, confidence: 95 };
    }
    
    if (hasPrice) {
        // Don't use AI for price - direct to WhatsApp (no public pricing)
        return { type: 'price_inquiry', useAI: false, queryDB: false, confidence: 85 };
    }
    
    if (hasComparison || mentionedBrands.length >= 2) {
        return { type: 'brand_comparison', useAI: true, queryDB: !!tyreSize, confidence: 80 };
    }
    
    if (hasQuality) {
        // AI with strict warranty guidelines
        return { type: 'quality_check', useAI: true, queryDB: false, confidence: 75 };
    }
    
    if (tyreSize && hasAvailability && !hasPrice && !hasComparison) {
        return { type: 'simple_lookup', useAI: false, queryDB: true, confidence: 90 };
    }
    
    if (hasAvailability && !tyreSize) {
        return { type: 'general_availability', useAI: true, queryDB: false, confidence: 70 };
    }
    
    if (tyreSize) {
        return { type: 'size_with_context', useAI: true, queryDB: true, confidence: 65 };
    }
    
    return { type: 'general', useAI: true, queryDB: false, confidence: 50 };
}

// ========================================
// FIXED RESPONSES (NO AI)
// ========================================
function generateFixedPriceResponse(lang) {
    if (lang === 'si') {
        return `🙏 ස්තූතියි ඔබේ විමසීම සඳහා.\n\nමිල ගණන් විස්තර WhatsApp ඔස්සේ දෙන්නම්:\n📱 wa.me/94721222509\n📞 0721222509\n\n(Lasantha Tyre Traders)`;
    } else {
        return `🙏 Thank you for your inquiry.\n\nFor pricing details, please WhatsApp us:\n📱 wa.me/94721222509\n📞 0721222509\n\n(Lasantha Tyre Traders)`;
    }
}

async function handleFacebookComment(commentEvent, mainPool) {
    try {
        const { comment_id, message, from, verb, parent_id } = commentEvent;
        const FB_PAGE_ID = process.env.FACEBOOK_PAGE_ID || '194559142002903';
        
        // Check if already processed (duplicate event)
        if (isAlreadyProcessed(comment_id)) {
            return;
        }
        
        if (verb === 'remove' || verb === 'edit') return;
        if (from.id === FB_PAGE_ID) return;
        if (!message || message.trim().length === 0) return;
        
        // Mark as being processed
        markAsProcessed(comment_id);
        
        console.log(`[FB] Comment from ${from.name}: ${message}`);
        console.log(`[FB] Parent ID: ${parent_id || 'NONE'}`);
        
        const language = detectLanguage(message);
        
        // Check if this is a reply to our previous comment (continuation)
        const parentContext = parent_id ? getContext(parent_id) : null;
        
        if (parentContext) {
            console.log(`[FB Handler] 🔄 Found context! Previous size: ${parentContext.tyreSize}, Brands: ${parentContext.brands?.join(', ')}`);
            
            // This is a follow-up question - handle contextually
            const response = await handleContextualReply(message, parentContext, language, mainPool);
            await replyToComment(comment_id, response);
            
            // Store context for this new reply too (for further continuation)
            storeContext(comment_id, {
                tyreSize: parentContext.tyreSize,
                brands: parentContext.brands,
                originalQuery: parentContext.originalQuery,
                followUp: message
            });
            
            console.log('[FB] Contextual response sent');
            return;
        }
        
        // No parent context - this is a new query
        const tyreSize = extractTyreSizeFlexible(message);
        
        console.log(`[FB Handler] 🔍 Detected language: ${language}`);
        console.log(`[FB Handler] 🔍 Detected tyre size: ${tyreSize || 'NONE'}`);
        
        // Detect scenario
        const scenario = detectCommentScenario(message, tyreSize, language);
        console.log(`[FB Handler] 🎯 Scenario: ${scenario.type} (${scenario.confidence}% confidence)`);
        console.log(`[FB Handler] 📋 AI: ${scenario.useAI}, DB: ${scenario.queryDB}`);
        
        let response = '';
        let brandsAvailable = [];
        
        // Handle based on scenario
        if (scenario.queryDB && tyreSize) {
            console.log(`[FB Handler] ✅ Querying database for: ${tyreSize}`);
            try {
                await mainPool;
                const result = await fetchTyreData(mainPool, tyreSize);
                console.log(`[FB Handler] 📦 Database returned ${result?.tyres?.length || 0} tyres`);
                
                if (result && result.tyres && result.tyres.length > 0) {
                    brandsAvailable = [...new Set(result.tyres.map(t => t.BrandName || t.normBrand || t.brand))].filter(b => b);
                    
                    if (scenario.useAI) {
                        console.log(`[FB Handler] 🤖 Using AI to enhance response`);
                        response = await generateAI(message, scenario.type, tyreSize, language, {
                            brands: brandsAvailable,
                            itemCount: result.tyres.length
                        });
                    } else {
                        response = buildWhatsAppLink(tyreSize, result.tyres, language);
                        console.log(`[FB Handler] ✅ Built standard brand list`);
                    }
                } else {
                    console.log(`[FB Handler] ⚠️ No stock`);
                    response = await generateAI(message, 'no_stock', tyreSize, language);
                }
            } catch (error) {
                console.error(`[FB Handler] ❌ Database error:`, error.message);
                response = await generateAI(message, 'error', tyreSize, language);
            }
        } else if (scenario.useAI) {
            console.log(`[FB Handler] 🤖 AI handling: ${scenario.type}`);
            response = await generateAI(message, scenario.type, tyreSize, language);
        } else if (scenario.type === 'price_inquiry') {
            console.log(`[FB Handler] 💰 Price inquiry - fixed response (no public pricing)`);
            response = generateFixedPriceResponse(language);
        } else {
            console.log(`[FB Handler] ℹ️ Fallback to general AI`);
            response = await generateAI(message, 'general', null, language);
        }
        
        await replyToComment(comment_id, response);
        
        // Store context for potential follow-up questions
        if (tyreSize && brandsAvailable.length > 0) {
            storeContext(comment_id, {
                tyreSize: tyreSize,
                brands: brandsAvailable,
                originalQuery: message
            });
            console.log(`[FB Handler] 💾 Stored context for comment ${comment_id}`);
        }
        
        console.log('[FB] Response sent');
    } catch (error) {
        console.error('[FB] Error:', error.message);
    }
}

function buildWhatsAppLink(size, data, lang) {
    // Helper function to remove tyre size from description
    const removeSizeFromDescription = (description, tyreSize) => {
        if (!description) return '';
        const normalized = tyreSize.replace(/[\/\-\s]/g, '');
        const patterns = [
            tyreSize,
            tyreSize.replace(/R/gi, ''),
            tyreSize.replace(/R/gi, '/'),
            tyreSize.replace(/[\/\-]/g, ' '),
            normalized
        ];
        
        let cleaned = description;
        patterns.forEach(pattern => {
            const regex = new RegExp(pattern.replace(/[\/\-\s]/g, '[\\s\\/\\-]*'), 'gi');
            cleaned = cleaned.replace(regex, '');
        });
        
        return cleaned.replace(/\s+/g, ' ').trim();
    };
    
    const categories = brandCategoriesConfig?.categories || {
        performance: { brands: ['GT', 'MAXXIS', 'MICHELIN', 'BRIDGESTONE', 'GITI'] },
        valueDurability: { brands: ['DURATURN', 'LANVIGATOR', 'WINDFORCE'] }
    };
    
    const rules = brandCategoriesConfig?.rules || {
        maxItemsBeforeTruncation: 15,
        truncationMessage_si: '• (තවත් විකල්ප WhatsApp link එකෙන් බලන්න)',
        truncationMessage_en: '• (See remaining options via WhatsApp link)'
    };
    
    const allItems = data.map(t => ({
        brand: (t.BrandName || t.normBrand || t.brand || '').toUpperCase(),
        description: t.ItemDescription || t.Description || ''
    }));
    
    const uniqueItems = Array.from(
        new Map(allItems.map(item => [item.description, item])).values()
    );
    
    const performanceBrands = categories.performance.brands.map(b => b.toUpperCase());
    const valueDurabilityBrands = categories.valueDurability.brands.map(b => b.toUpperCase());
    
    const performanceItems = uniqueItems.filter(item => performanceBrands.includes(item.brand));
    const valueDurabilityItems = uniqueItems.filter(item => valueDurabilityBrands.includes(item.brand));
    const otherItems = uniqueItems.filter(item => 
        !performanceBrands.includes(item.brand) && !valueDurabilityBrands.includes(item.brand)
    );

    if (lang === 'si') {
        const link = `https://wa.me/94${WHATSAPP_NUMBER.substring(1)}?text=${encodeURIComponent(`Expert advice for ${size}`)}`;
        let reply = `✅ ස්තූතියි. ඔබගේ විමසීම පරිදි, ${size} ප්‍රමාණයේ ටයර් සඳහා විකල්ප කිහිපයක් අප සතුව ඇත.\n\n🌟 **අපේ Expert's Choice:**\n\n`;
        
        let itemCount = 0;
        const maxItems = rules.maxItemsBeforeTruncation;
        let wasTruncated = false;
        
        if (performanceItems.length > 0) {
            reply += `**Performance:**\n`;
            for (let i = 0; i < performanceItems.length && itemCount < maxItems; i++, itemCount++) {
                reply += `  • ${removeSizeFromDescription(performanceItems[i].description, size)}\n`;
            }
            reply += `\n`;
        }
        
        if (itemCount < maxItems && valueDurabilityItems.length > 0) {
            reply += `**Value & Durability:**\n`;
            for (let i = 0; i < valueDurabilityItems.length && itemCount < maxItems; i++, itemCount++) {
                reply += `  • ${removeSizeFromDescription(valueDurabilityItems[i].description, size)}\n`;
            }
            reply += `\n`;
        }
        
        if (itemCount < maxItems && otherItems.length > 0) {
            reply += `**තවත් විකල්ප:**\n`;
            for (let i = 0; i < otherItems.length && itemCount < maxItems; i++, itemCount++) {
                reply += `  • ${removeSizeFromDescription(otherItems[i].description, size)}\n`;
            }
            reply += `\n`;
        }
        
        if (itemCount >= maxItems && (performanceItems.length + valueDurabilityItems.length + otherItems.length) > maxItems) {
            reply += `${rules.truncationMessage_si}\n\n`;
        }
        
        reply += `මිල ගණන් සහ ඔබට අවශ්‍ය ටයරය පිළිබඳ සම්පූර්ණ විස්තර ලබා ගැනීමට, පහත link එක හරහා WhatsApp ඔස්සේ අප සමඟ සම්බන්ධ වන්න.\n\n`;
        reply += `📲 **Get Expert Advice & Pricing:**\n👉 ${link}\n\n📞 **Call Us:** ${WHATSAPP_NUMBER}\n(Lasantha Tyre Traders - Your Trusted Tyre Partner)`;
        
        return reply;
    }

    const link = `https://wa.me/94${WHATSAPP_NUMBER.substring(1)}?text=${encodeURIComponent(`Requesting quote for ${size}`)}`;
    let reply = `Thank you for your inquiry. We confirm that size ${size} is currently available.\n\n**Available Options:**\n\n`;
    
    let itemCount = 0;
    const maxItems = rules.maxItemsBeforeTruncation;
    
    if (performanceItems.length > 0) {
        reply += `**Performance:**\n`;
        for (let i = 0; i < performanceItems.length && itemCount < maxItems; i++, itemCount++) {
            reply += `  • ${removeSizeFromDescription(performanceItems[i].description, size)}\n`;
        }
        reply += `\n`;
    }
    
    if (itemCount < maxItems && valueDurabilityItems.length > 0) {
        reply += `**Value & Durability:**\n`;
        for (let i = 0; i < valueDurabilityItems.length && itemCount < maxItems; i++, itemCount++) {
            reply += `  • ${removeSizeFromDescription(valueDurabilityItems[i].description, size)}\n`;
        }
        reply += `\n`;
    }
    
    if (itemCount < maxItems && otherItems.length > 0) {
        reply += `**Other Options:**\n`;
        for (let i = 0; i < otherItems.length && itemCount < maxItems; i++, itemCount++) {
            reply += `  • ${removeSizeFromDescription(otherItems[i].description, size)}\n`;
        }
        reply += `\n`;
    }
    
    if (itemCount >= maxItems && (performanceItems.length + valueDurabilityItems.length + otherItems.length) > maxItems) {
        reply += `${rules.truncationMessage_en}\n\n`;
    }
    
    reply += `For complete pricing details and expert advice, please contact us via WhatsApp.\n\n`;
    reply += `**Contact for Quotation:**\n${link}\n\n**Direct Line:** ${WHATSAPP_NUMBER}`;
    
    return reply;
}

async function generateAI(msg, scenario, size, lang, context = null) {
    try {
        // Use Gemini 2.5 Flash for better performance
        const model = genAI.getGenerativeModel({ model: 'gemini-2.5-flash' });
        
        let systemPrompt = `You are Lasantha Tyre Traders AI assistant. Contact: wa.me/94721222509 | 0721222509
CRITICAL: Keep ALL responses SHORT (2-3 lines maximum)`;

        let userPrompt = '';
        
        if (scenario === 'greeting') {
            userPrompt = lang === 'si'
                ? `Greeting: "${msg}"\nස්වාභාවික reply (1-2 lines), ටයර් services mention කරන්න`
                : `Greeting: "${msg}"\nNatural reply (1-2 lines), mention tyre services`;
        }
        else if (scenario === 'thanks') {
            userPrompt = lang === 'si'
                ? `Thanks message: "${msg}"\nඅත්පත් reply (1 line)`
                : `Thanks message: "${msg}"\nGracious reply (1 line)`;
        }
        else if (scenario === 'price_inquiry') {
            const brands = context?.brands || [];
            userPrompt = lang === 'si'
                ? `මිල විමසීම: "${msg}"${size ? `\nSize ${size} available. Brands: ${brands.slice(0,3).join(', ')}` : ''}
Reply (2 lines): 1) මිල brand එකට අනුව වෙනස් 2) WhatsApp link`
                : `Price inquiry: "${msg}"${size ? `\nSize ${size} available. Brands: ${brands.slice(0,3).join(', ')}` : ''}
Reply (2 lines): 1) Prices vary by brand 2) WhatsApp link`;
        }
        else if (scenario === 'brand_comparison') {
            userPrompt = lang === 'si'
                ? `Brand comparison: "${msg}"${size ? `\nSize: ${size}` : ''}
Reply (2 lines): Expert advice needed, WhatsApp කරන්න`
                : `Brand comparison: "${msg}"${size ? `\nSize: ${size}` : ''}
Reply (2 lines): Expert advice needed, contact via WhatsApp`;
        }
        else if (scenario === 'quality_check') {
            userPrompt = lang === 'si'
                ? `Quality/warranty විමසීම: "${msg}"

⚠️ CRITICAL WARRANTY CONDITIONS (MUST MENTION):
- Warranty: 40,000 KM හෝ 4 Years (පළමුව සම්පූර්ණ වන එක)
- Coverage: නිෂ්පාදන දෝෂ විතරයි (Manufacturing defects only)
- NOT covered: ගැලපෙන්න පුළුවන් ගැටළු, වැරදි භාවිතය, හානි

Reply (2-3 lines): 
1) Warranty conditions කෙටියෙන් mention කරන්න
2) විස්තර WhatsApp කරන්න invite කරන්න`
                : `Quality/warranty inquiry: "${msg}"

⚠️ CRITICAL WARRANTY CONDITIONS (MUST MENTION):
- Warranty: 40,000 KM or 4 Years (whichever comes first)
- Coverage: Manufacturing defects ONLY
- NOT covered: Wear & tear, misuse, damage

Reply (2-3 lines):
1) Briefly mention warranty terms
2) Invite to WhatsApp for details`;
        }
        else if (scenario === 'no_stock') {
            userPrompt = lang === 'si'
                ? `Out of stock size ${size}. "${msg}"
Reply (2 lines): දැනට නැත, alternatives check කරන්න offer`
                : `Out of stock size ${size}. "${msg}"
Reply (2 lines): Currently unavailable, offer alternatives`;
        }
        else if (scenario === 'error') {
            userPrompt = lang === 'si'
                ? `System error. "${msg}"
Reply (2 lines): සමාවෙන්න, WhatsApp කරන්න`
                : `System error. "${msg}"
Reply (2 lines): Apologies, contact WhatsApp`;
        }
        else {
            userPrompt = lang === 'si'
                ? `විමසීම: "${msg}"\nකෙටි helpful reply (2 lines)`
                : `Inquiry: "${msg}"\nShort helpful reply (2 lines)`;
        }
        
        const fullPrompt = `${systemPrompt}\n\n${userPrompt}`;
        const result = await model.generateContent(fullPrompt);
        let response = result.response.text().trim();
        
        if (response.length > 400) {
            response = response.substring(0, 397) + '...';
        }
        
        return response;
        
    } catch (error) {
        console.error('[FB AI] Error:', error.message);
        return lang === 'si' 
            ? `🙏 විස්තර:\n📱 wa.me/94721222509`
            : `🙏 Details:\n📱 wa.me/94721222509`;
    }
}

function detectLanguage(text) {
    const lowerText = text.toLowerCase();
    
    if (/[඀-෿]/.test(text)) return 'si';
    
    const sinhalaKeywords = [
        'tyre', 'tire', 'thiyenawada', 'තයර්', 'tayar', 'kiyada', 'කීයද', 'මිල', 'mila',
        'mokadda', 'thiyeda', 'තියෙද', 'awada', 'enavada', 'ganna', 'ගන්න', 'දෙන්න'
    ];
    
    const matchCount = sinhalaKeywords.filter(keyword => lowerText.includes(keyword)).length;
    
    if (matchCount >= 2) return 'si';
    
    const englishPatterns = [
        /\b(how much|price|cost|available|stock|have|need|want)\b/i
    ];
    
    if (englishPatterns.some(pattern => pattern.test(text)) && matchCount === 0) {
        return 'en';
    }
    
    return matchCount >= 1 ? 'si' : 'en';
}

async function handleContextualReply(message, context, lang, mainPool) {
    const { tyreSize, brands } = context;
    const msgLower = message.toLowerCase();
    
    console.log(`[FB Handler] 🤔 Contextual question: "${message}"`);
    
    const brandMentioned = brands.find(b => msgLower.includes(b.toLowerCase()));
    
    if (brandMentioned) {
        if (lang === 'si') {
            return `ඔව්, ${brandMentioned.toUpperCase()} brand එක ${tyreSize} size එකේ තියෙනවා! ✅\n\nමිල සහ තොගය:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}?text=${encodeURIComponent(`${brandMentioned} ${tyreSize} price`)}\n\n📞 ${WHATSAPP_NUMBER}`;
        } else {
            return `Yes, we have ${brandMentioned.toUpperCase()} in size ${tyreSize}! ✅\n\nFor pricing:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}?text=${encodeURIComponent(`${brandMentioned} ${tyreSize} price`)}\n\n📞 ${WHATSAPP_NUMBER}`;
        }
    }
    
    if (msgLower.includes('brand') || msgLower.includes('බ්‍රෑන්ඩ්')) {
        const brandList = brands.slice(0, 6).join(', ').toUpperCase();
        if (lang === 'si') {
            return `${tyreSize} brands:\n\n${brandList}${brands.length > 6 ? ` සහ තවත් ${brands.length - 6}` : ''}\n\nවැඩි විස්තර:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}\n📞 ${WHATSAPP_NUMBER}`;
        } else {
            return `Brands for ${tyreSize}:\n\n${brandList}${brands.length > 6 ? ` and ${brands.length - 6} more` : ''}\n\nFor details:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}\n📞 ${WHATSAPP_NUMBER}`;
        }
    }
    
    if (msgLower.includes('price') || msgLower.includes('මිල') || msgLower.includes('කීයද')) {
        if (lang === 'si') {
            return `${tyreSize} මිල brand එකට අනුව වෙනස්.\n\nසියලු මිල:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}\n📞 ${WHATSAPP_NUMBER}`;
        } else {
            return `Prices for ${tyreSize} vary by brand.\n\nFor pricing:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}\n📞 ${WHATSAPP_NUMBER}`;
        }
    }
    
    try {
        const model = genAI.getGenerativeModel({ model: 'gemini-2.5-flash' });
        const prompt = lang === 'si'
            ? `Context: ${tyreSize}, Brands: ${brands.slice(0,3).join(', ')}. Follow-up: "${message}". Short reply (2 lines), WhatsApp: wa.me/94721222509`
            : `Context: ${tyreSize}, Brands: ${brands.slice(0,3).join(', ')}. Follow-up: "${message}". Short reply (2 lines), WhatsApp: wa.me/94721222509`;
        
        const result = await model.generateContent(prompt);
        return result.response.text();
    } catch (error) {
        return lang === 'si'
            ? `${tyreSize} විස්තර:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}`
            : `Details for ${tyreSize}:\n👉 https://wa.me/94${WHATSAPP_NUMBER.substring(1)}`;
    }
}

module.exports = { handleFacebookComment };
