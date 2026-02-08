// utils/facebookConversationContext.js
// Store and retrieve conversation context for Facebook comment threads

const fs = require('fs');
const path = require('path');

const CONTEXT_FILE = path.join(__dirname, '..', 'facebook-conversation-context.json');
const CONTEXT_TTL = 24 * 60 * 60 * 1000; // 24 hours

// In-memory cache for faster access
let contextCache = {};

// Load contexts from file on startup
function loadContexts() {
    try {
        if (fs.existsSync(CONTEXT_FILE)) {
            const data = fs.readFileSync(CONTEXT_FILE, 'utf8');
            contextCache = JSON.parse(data);
            
            // Clean up expired contexts
            const now = Date.now();
            Object.keys(contextCache).forEach(key => {
                if (contextCache[key].timestamp && (now - contextCache[key].timestamp) > CONTEXT_TTL) {
                    delete contextCache[key];
                }
            });
        }
    } catch (error) {
        console.error('[FB Context] Error loading contexts:', error.message);
        contextCache = {};
    }
}

// Save contexts to file
function saveContexts() {
    try {
        fs.writeFileSync(CONTEXT_FILE, JSON.stringify(contextCache, null, 2));
    } catch (error) {
        console.error('[FB Context] Error saving contexts:', error.message);
    }
}

/**
 * Store conversation context for a comment
 * @param {string} commentId - The comment ID (bot's reply)
 * @param {object} context - Context object { tyreSize, brands, originalQuery, timestamp }
 */
function storeContext(commentId, context) {
    contextCache[commentId] = {
        ...context,
        timestamp: Date.now()
    };
    saveContexts();
    console.log(`[FB Context] Stored context for comment ${commentId}:`, context.tyreSize);
}

/**
 * Get conversation context for a parent comment
 * @param {string} parentCommentId - The parent comment ID
 * @returns {object|null} Context object or null if not found
 */
function getContext(parentCommentId) {
    const context = contextCache[parentCommentId];
    
    if (!context) {
        return null;
    }
    
    // Check if context is expired
    const now = Date.now();
    if (context.timestamp && (now - context.timestamp) > CONTEXT_TTL) {
        delete contextCache[parentCommentId];
        saveContexts();
        return null;
    }
    
    return context;
}

/**
 * Clear expired contexts (can be called periodically)
 */
function cleanupExpiredContexts() {
    const now = Date.now();
    let cleaned = 0;
    
    Object.keys(contextCache).forEach(key => {
        if (contextCache[key].timestamp && (now - contextCache[key].timestamp) > CONTEXT_TTL) {
            delete contextCache[key];
            cleaned++;
        }
    });
    
    if (cleaned > 0) {
        saveContexts();
        console.log(`[FB Context] Cleaned up ${cleaned} expired contexts`);
    }
}

// Load contexts on module initialization
loadContexts();

// Cleanup expired contexts every hour
setInterval(cleanupExpiredContexts, 60 * 60 * 1000);

module.exports = {
    storeContext,
    getContext,
    cleanupExpiredContexts
};
