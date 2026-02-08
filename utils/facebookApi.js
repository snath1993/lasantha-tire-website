/**
 * Facebook Graph API Helper Functions
 * Handles comment replies and private messaging with retry logic
 */

const axios = require('axios');
const fs = require('fs');
const path = require('path');

const FB_PAGE_ACCESS_TOKEN = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
const FB_API_VERSION = 'v21.0';
const FB_GRAPH_URL = `https://graph.facebook.com/${FB_API_VERSION}`;
const ERROR_LOG_FILE = path.join(__dirname, '..', 'facebook-api-errors.log');

// Retry configuration
const RETRY_CONFIG = {
    maxAttempts: 3,
    initialDelayMs: 1000,
    maxDelayMs: 10000,
    backoffMultiplier: 2
};

/**
 * Log detailed error to file
 */
function logError(context, error) {
    try {
        const timestamp = new Date().toISOString();
        const errorDetails = {
            timestamp,
            context,
            status: error.response?.status,
            statusText: error.response?.statusText,
            data: error.response?.data,
            message: error.message
        };
        
        const logLine = `${JSON.stringify(errorDetails)}\n`;
        fs.appendFileSync(ERROR_LOG_FILE, logLine);
    } catch (logErr) {
        console.error('[FB API] Failed to log error:', logErr.message);
    }
}

/**
 * Retry with exponential backoff
 */
async function retryWithBackoff(fn, context) {
    let lastError;
    
    for (let attempt = 1; attempt <= RETRY_CONFIG.maxAttempts; attempt++) {
        try {
            return await fn();
        } catch (error) {
            lastError = error;
            const status = error.response?.status;
            
            // Don't retry on client errors (4xx except 429)
            if (status && status >= 400 && status < 500 && status !== 429) {
                console.error(`[FB API] Client error ${status} - not retrying`);
                logError(context, error);
                throw error;
            }
            
            // Retry on 429 (rate limit) or 5xx (server errors)
            if (attempt < RETRY_CONFIG.maxAttempts && (status === 429 || (status >= 500))) {
                const delay = Math.min(
                    RETRY_CONFIG.initialDelayMs * Math.pow(RETRY_CONFIG.backoffMultiplier, attempt - 1),
                    RETRY_CONFIG.maxDelayMs
                );
                
                console.warn(`[FB API] Attempt ${attempt}/${RETRY_CONFIG.maxAttempts} failed (${status}). Retrying in ${delay}ms...`);
                await new Promise(resolve => setTimeout(resolve, delay));
            }
        }
    }
    
    // All attempts failed
    console.error(`[FB API] All ${RETRY_CONFIG.maxAttempts} attempts failed for ${context}`);
    logError(context, lastError);
    throw lastError;
}

/**
 * Reply to a Facebook comment (public)
 * @param {string} commentId - The ID of the comment to reply to
 * @param {string} message - The reply message
 * @returns {Promise<object>} Response from Facebook API
 */
async function replyToComment(commentId, message) {
    return retryWithBackoff(async () => {
        console.log(`[FB API] Replying to comment ${commentId}`);
        
        const url = `${FB_GRAPH_URL}/${commentId}/comments`;
        const response = await axios.post(url, {
            message: message,
            access_token: FB_PAGE_ACCESS_TOKEN
        });
        
        console.log(`✅ [FB API] Comment reply sent successfully`);
        return response.data;
    }, `replyToComment(${commentId})`);
}

/**
 * Send a private message to a user via Facebook Messenger
 * @param {string} userId - The Facebook user ID (PSID)
 * @param {string} message - The message to send
 * @returns {Promise<object>} Response from Facebook API
 */
async function sendPrivateMessage(userId, message) {
    return retryWithBackoff(async () => {
        console.log(`[FB API] Sending private message to user ${userId}`);
        
        const url = `${FB_GRAPH_URL}/me/messages`;
        const response = await axios.post(url, {
            recipient: { id: userId },
            message: { text: message },
            access_token: FB_PAGE_ACCESS_TOKEN
        });
        
        console.log(`✅ [FB API] Private message sent successfully`);
        return response.data;
    }, `sendPrivateMessage(${userId})`);
}

/**
 * Get comment details including message text
 * @param {string} commentId - The ID of the comment
 * @returns {Promise<object>} Comment details
 */
async function getCommentDetails(commentId) {
    return retryWithBackoff(async () => {
        console.log(`[FB API] Fetching comment details for ${commentId}`);
        
        const url = `${FB_GRAPH_URL}/${commentId}`;
        const response = await axios.get(url, {
            params: {
                fields: 'id,message,from,created_time,parent,permalink_url',
                access_token: FB_PAGE_ACCESS_TOKEN
            }
        });
        
        console.log(`✅ [FB API] Comment details fetched`);
        return response.data;
    }, `getCommentDetails(${commentId})`);
}

/**
 * Generate polite acknowledgment message for public comment reply
 * @param {string} language - 'si' for Sinhala, 'en' for English
 * @returns {string} Acknowledgment message
 */
function getAcknowledgmentMessage(language = 'si') {
    const messages = {
        si: '🙏 ස්තූතියි! මොහොතකින් details එක reply කරන්නම්...',
        en: '🙏 Thank you! We will reply with details in a moment...'
    };
    
    return messages[language] || messages.si;
}

/**
 * Format tyre details message for private messaging
 * @param {object} tyreData - Tyre data from database
 * @param {string} language - 'si' or 'en'
 * @returns {string} Formatted message
 */
function formatTyreDetailsMessage(tyreData, language = 'si') {
    if (!tyreData || tyreData.length === 0) {
        if (language === 'si') {
            return '😔 සමාවෙන්න, දැනට මෙම tyre size එක stock එකේ නැහැ. වැඩි විස්තර සඳහා අමතන්න: 0721222509';
        } else {
            return '😔 Sorry, this tyre size is currently out of stock. For more information, please call: 0721222509';
        }
    }
    
    let message = language === 'si' 
        ? '🚗 *Tyre Details / ටයර් විස්තර*\n\n'
        : '🚗 *Tyre Details*\n\n';
    
    tyreData.forEach((tyre, index) => {
        message += `${index + 1}. *${tyre.brand}* ${tyre.size}\n`;
        message += `   💰 Price: Rs. ${Number(tyre.price).toLocaleString()}\n`;
        message += `   📦 Stock: ${tyre.qty} units\n`;
        if (tyre.notes) {
            message += `   📝 ${tyre.notes}\n`;
        }
        message += '\n';
    });
    
    message += language === 'si'
        ? '\n📞 *විමසීම් සඳහා:* 0721222509\n📍 *ස්ථානය:* Thalawathugoda\n⏰ *වේලාව:* 6:30 AM - 9:00 PM'
        : '\n📞 *Contact:* 0721222509\n📍 *Location:* Thalawathugoda\n⏰ *Hours:* 6:30 AM - 9:00 PM';
    
    return message;
}

module.exports = {
    replyToComment,
    sendPrivateMessage,
    getCommentDetails,
    getAcknowledgmentMessage,
    formatTyreDetailsMessage
};
