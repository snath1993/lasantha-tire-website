// Enhanced context extraction from chat history
// Extracts tyre size, brand, and other entities from recent conversation
const { extractTyreSizeFlexible } = require('./detect');

/**
 * Extract contextual information from chat history
 * @param {Array} chatHistory - Recent chat messages (newest first)
 * @param {string} currentMessage - Current user message
 * @returns {Object} - Extracted context { tyreSize, brand, lastIntent, entities }
 */
function extractConversationContext(chatHistory, currentMessage) {
    const context = {
        tyreSize: null,
        brand: null,
        lastIntent: null,
        lastBotResponse: null,
        entities: {},
        conversationFlow: []
    };

    if (!Array.isArray(chatHistory) || chatHistory.length === 0) {
        return context;
    }

    // NOTE: Do NOT use naive regexes here; they accidentally match phone numbers
    // like 077 731 1770 → "731/17/70". Instead, use validated extractor.

    // Brand patterns - expanded list with variations
    const brandPatterns = [
        /\b(MAXXIS|MAXXIES|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|GOOD\s*YEAR|CONTINENTAL|PIRELLI|HANKOOK|KUMHO|TOYO|NEXEN|FEDERAL|GITI|CEAT)\b/i
    ];

    // Iterate through history (newest to oldest)
    for (let i = 0; i < Math.min(chatHistory.length, 10); i++) {
        const msg = chatHistory[i];
        const text = msg.message_text || '';
        const isBot = (msg.sender_type || '').toLowerCase() === 'bot';
        const isUser = (msg.sender_type || '').toLowerCase() === 'user';

        // Track conversation flow
        context.conversationFlow.push({
            type: msg.sender_type,
            text: text.substring(0, 100),
            timestamp: msg.created_at
        });

        // Extract tyre size if not already found (validated to avoid phone numbers)
        if (!context.tyreSize && text) {
            const detected = extractTyreSizeFlexible(text);
            if (detected) {
                context.tyreSize = detected; // can be "width/profile" or "width/profile/rim"
            }
        }

        // Extract brand if not already found
        if (!context.brand && text) {
            const brandMatch = text.match(brandPatterns[0]);
            if (brandMatch) {
                context.brand = brandMatch[1].toUpperCase();
            }
        }

        // Detect last intent from bot response
        if (isBot && !context.lastIntent) {
            if (text.includes('ගාන') || text.includes('price') || text.includes('Rs.') || text.includes('රු.')) {
                context.lastIntent = 'price_inquiry';
            } else if (text.includes('තොග') || text.includes('stock') || text.includes('qty') || text.includes('available')) {
                context.lastIntent = 'stock_inquiry';
            } else if (text.includes('photo') || text.includes('පින්තූර') || text.includes('image')) {
                context.lastIntent = 'photo_request';
            } else if (text.includes('quotation') || text.includes('estimate') || text.includes('කෝට්ටේෂන්')) {
                context.lastIntent = 'quotation_request';
            } else if (text.includes('warranty') || text.includes('වොරන්ටි')) {
                context.lastIntent = 'warranty_inquiry';
            } else if (text.includes('තව') || text.includes('more') || text.includes('වෙනත්') || text.includes('other')) {
                context.lastIntent = 'asking_for_alternatives';
            }
            context.lastBotResponse = text.substring(0, 200);
        }

        // Stop if we have all we need
        if (context.tyreSize && context.brand && context.lastIntent) {
            break;
        }
    }

    return context;
}

/**
 * Enhance current message analysis with conversation context
 * @param {string} currentMessage - Current user message
 * @param {Object} context - Extracted conversation context
 * @returns {Object} - Enhanced entities
 */
function enhanceWithContext(currentMessage, context) {
    const enhanced = {
        tyre_size: null,
        brand: null,
        contextAware: false,
        source: 'current'
    };

    // Try to extract from current message first (validated extractor)
    const detectedCurrent = extractTyreSizeFlexible(currentMessage || '');
    if (detectedCurrent) {
        enhanced.tyre_size = detectedCurrent; // safe, avoids phone numbers
        enhanced.source = 'current';
    }

    const brandPattern = /\b(MAXXIS|MAXXIES|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|GOOD\s*YEAR|CONTINENTAL|PIRELLI|HANKOOK|KUMHO|TOYO|NEXEN|FEDERAL|GITI|CEAT)\b/i;
    const brandMatch = currentMessage.match(brandPattern);
    if (brandMatch) {
        // Normalize brand variations
        let normalizedBrand = brandMatch[1].toUpperCase().replace(/\s+/g, '');
        if (normalizedBrand === 'MAXXIES') normalizedBrand = 'MAXXIS';
        if (normalizedBrand === 'GOODYEAR') normalizedBrand = 'GOODYEAR';
        enhanced.brand = normalizedBrand;
        enhanced.source = 'current';
    }

    // If current message has only brand but no size, use context
    if (enhanced.brand && !enhanced.tyre_size && context.tyreSize) {
        enhanced.tyre_size = context.tyreSize;
        enhanced.contextAware = true;
        enhanced.source = 'context+current';
    }

    // If current message has only size but no brand, use context brand intelligently
    if (enhanced.tyre_size && !enhanced.brand && context.brand) {
        // Use context brand unless user is explicitly asking about OTHER brands
        const askingAboutOtherBrands = /වෙනත්|තව\s*(brands?|එකක්)|more\s*brands?|other\s*brands?|alternatives/i.test(currentMessage);
        if (!askingAboutOtherBrands) {
            enhanced.brand = context.brand;
            enhanced.contextAware = true;
            enhanced.source = 'context+current';
        }
    }

    // If current message is very short (follow-up question), try to use full context
    if (!enhanced.tyre_size && context.tyreSize) {
        const isShortFollowUp = currentMessage.trim().split(/\s+/).length <= 4;
        const isFollowUpKeyword = /ගාන|price|තොග|stock|photo|පින්තූර|warranty|වොරන්ටි|available|තිබෙනව/i.test(currentMessage);
        
        if (isShortFollowUp && isFollowUpKeyword) {
            enhanced.tyre_size = context.tyreSize;
            enhanced.brand = context.brand; // Use brand too if available
            enhanced.contextAware = true;
            enhanced.source = 'context_followup';
        }
    }

    return enhanced;
}

module.exports = {
    extractConversationContext,
    enhanceWithContext
};
