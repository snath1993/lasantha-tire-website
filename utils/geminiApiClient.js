/**
 * Gemini API Client
 * Wrapper for AI interactions
 */

/**
 * Analyze incoming message and extract intent and entities
 * @param {string} text - Message text
 * @param {Array} history - Chat history
 * @returns {Promise<object>} - Analysis result
 */
async function analyzeMessage(text, history = []) {
    // Simple rule-based analysis for now
    const analysis = {
        intent: 'unknown',
        entities: {},
        confidence: 0.8
    };
    
    const lowerText = text.toLowerCase();
    
    // Detect intent
    if (lowerText.includes('price') || lowerText.includes('මිල') || lowerText.includes('කියද') || lowerText.includes('cost')) {
        analysis.intent = 'price_inquiry';
    } else if (lowerText.includes('quotation') || lowerText.includes('pdf') || lowerText.includes('quote')) {
        analysis.intent = 'quotation_request';
    } else if (lowerText.includes('stock') || lowerText.includes('available') || lowerText.includes('තියෙනවද')) {
        analysis.intent = 'stock_check';
    } else if (lowerText.includes('invoice') || lowerText.includes('bill')) {
        analysis.intent = 'invoice_lookup';
    }
    
    // Extract entities from text
    const { extractTyreSizeFlexible } = require('./detect');
    const tyreSize = extractTyreSizeFlexible(text);
    if (tyreSize) {
        analysis.entities.tyre_size = tyreSize;
    }
    
    const brandMatch = text.match(/(MAXXIS|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|CONTINENTAL|GITI|KUMHO|HANKOOK)/i);
    if (brandMatch) {
        analysis.entities.brand = brandMatch[1].toUpperCase();
    }
    
    return analysis;
}

/**
 * Generate response based on analysis and data
 * @param {string} text - Original message
 * @param {object} analysis - Message analysis
 * @param {object} data - Data to include in response
 * @returns {Promise<string>} - Generated response
 */
async function generateResponse(text, analysis, data) {
    try {
        // Build response based on intent and data
        if (analysis.intent === 'price_inquiry' && data.tyres && data.tyres.length > 0) {
            return formatPriceResponse(data);
        } else if (analysis.intent === 'stock_check' && data.tyres) {
            return formatStockResponse(data);
        } else if (data.error) {
            return `Sorry, I encountered an error: ${data.error}. Please contact 0777311770 for assistance.`;
        } else if (!data.tyres || data.tyres.length === 0) {
            return `Sorry, I couldn't find any tyres matching your request. Please contact 0777311770 for assistance.`;
        }
        
        return `Thank you for your message. Please contact 0777311770 for assistance.`;
    } catch (error) {
        console.error('[Gemini] Error generating response:', error.message);
        return `Sorry, I encountered an error. Please contact 0777311770 for assistance.`;
    }
}

/**
 * Format price response
 * @param {object} data - Tyre data
 * @returns {string} - Formatted response
 */
function formatPriceResponse(data) {
    let response = `🔍 *Available Tyres*\n\n`;
    
    const displayTyres = data.tyres.slice(0, 5); // Show top 5
    
    for (const tyre of displayTyres) {
        response += `📦 *${tyre.Brand || 'Unknown'} ${tyre.Pattern || ''}*\n`;
        response += `Size: ${data.tyreSize || 'N/A'}\n`;
        response += `💰 Price: Rs. ${parseFloat(tyre.Price || 0).toLocaleString()}\n`;
        
        if (data.meta && data.meta.allowStockDetails) {
            response += `📊 Stock: ${tyre.Qty || 0} units\n`;
        }
        
        response += `\n`;
    }
    
    response += `\n📞 For orders, call: 0777311770\n`;
    response += `📍 Lasantha Tire Centre`;
    
    return response;
}

/**
 * Format stock response
 * @param {object} data - Tyre data
 * @returns {string} - Formatted response
 */
function formatStockResponse(data) {
    if (!data.tyres || data.tyres.length === 0) {
        return `Sorry, we don't have ${data.tyreSize || 'that size'} in stock right now. Please contact 0777311770 for alternatives.`;
    }
    
    const inStock = data.tyres.filter(t => t.Qty > 0);
    
    if (inStock.length === 0) {
        return `Sorry, ${data.tyreSize || 'that size'} is currently out of stock. Please contact 0777311770 for expected availability.`;
    }
    
    let response = `✅ *In Stock*\n\n`;
    response += `We have ${inStock.length} option(s) for ${data.tyreSize || 'your size'}:\n\n`;
    
    for (const tyre of inStock.slice(0, 3)) {
        response += `• ${tyre.Brand || 'Unknown'} ${tyre.Pattern || ''}\n`;
    }
    
    response += `\n📞 Call for details: 0777311770`;
    
    return response;
}

/**
 * Generate admin response (with more details)
 * @param {object} data - Tyre data
 * @returns {Promise<string>} - Admin response
 */
async function generateAdminResponse(data) {
    if (data.formatted) {
        return data.formatted;
    }
    
    return formatPriceResponse(data);
}

module.exports = {
    analyzeMessage,
    generateResponse,
    generateAdminResponse
};
