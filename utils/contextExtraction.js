/**
 * Context Extraction Utilities
 * Extract conversation context from chat history
 */

const { extractTyreSizeFlexible } = require('./detect');

/**
 * Extract conversation context from chat history
 * @param {Array} history - Chat history records
 * @param {string} currentMessage - Current message text
 * @returns {object} - Context object
 */
function extractConversationContext(history, currentMessage) {
    const context = {
        tyreSize: null,
        brand: null,
        lastIntent: null,
        previousMessages: []
    };
    
    if (!Array.isArray(history) || history.length === 0) {
        return context;
    }
    
    // Look for tyre size and brand in recent messages
    for (const record of history) {
        const text = record.message_content || record.message || '';
        
        // Extract tyre size
        if (!context.tyreSize) {
            const size = extractTyreSizeFlexible(text);
            if (size) {
                context.tyreSize = size;
            }
        }
        
        // Extract brand
        if (!context.brand) {
            const brandMatch = text.match(/(MAXXIS|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|CONTINENTAL|GITI|KUMHO|HANKOOK)/i);
            if (brandMatch) {
                context.brand = brandMatch[1].toUpperCase();
            }
        }
        
        // Extract intent
        if (!context.lastIntent && record.sender_type === 'bot') {
            if (text.includes('price') || text.includes('මිල')) {
                context.lastIntent = 'price_inquiry';
            } else if (text.includes('stock') || text.includes('available')) {
                context.lastIntent = 'stock_check';
            } else if (text.includes('quotation') || text.includes('PDF')) {
                context.lastIntent = 'quotation_request';
            }
        }
        
        context.previousMessages.push(text);
    }
    
    return context;
}

/**
 * Enhance entities with conversation context
 * @param {string} currentMessage - Current message
 * @param {object} context - Conversation context
 * @returns {object} - Enhanced entities
 */
function enhanceWithContext(currentMessage, context) {
    const enhanced = {
        contextAware: false,
        tyre_size: null,
        brand: null,
        source: 'direct'
    };
    
    // First try to extract from current message
    const directSize = extractTyreSizeFlexible(currentMessage);
    const directBrand = currentMessage.match(/(MAXXIS|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|CONTINENTAL|GITI|KUMHO|HANKOOK)/i);
    
    if (directSize) {
        enhanced.tyre_size = directSize;
        enhanced.source = 'direct';
    } else if (context.tyreSize) {
        enhanced.tyre_size = context.tyreSize;
        enhanced.source = 'context';
        enhanced.contextAware = true;
    }
    
    if (directBrand) {
        enhanced.brand = directBrand[1].toUpperCase();
        enhanced.source = 'direct';
    } else if (context.brand) {
        enhanced.brand = context.brand;
        enhanced.source = enhanced.source === 'context' ? 'context' : 'mixed';
        enhanced.contextAware = true;
    }
    
    return enhanced;
}

module.exports = {
    extractConversationContext,
    enhanceWithContext
};
