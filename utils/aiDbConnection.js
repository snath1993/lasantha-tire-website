/**
 * AI Database Connection Pool
 * Manages connection to WhatsAppAI database
 */
const sql = require('mssql');

// AI Database configuration
const aiConfig = {
    server: process.env.SQL_SERVER || process.env.DB_SERVER || 'localhost',
    user: process.env.SQL_USER || process.env.DB_USER || 'sa',
    password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
    database: 'WhatsAppAI',
    options: {
        encrypt: false,
        trustServerCertificate: true,
        enableArithAbort: true,
        connectTimeout: 30000,
        requestTimeout: 30000
    },
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

let aiPool = null;
let aiPoolConnect = null;

/**
 * Initialize AI database connection pool
 */
async function initAiPool() {
    if (aiPool) {
        return aiPool;
    }
    
    try {
        aiPool = new sql.ConnectionPool(aiConfig);
        aiPoolConnect = aiPool.connect();
        await aiPoolConnect;
        
        console.log('[AI DB] ✅ Connected to WhatsAppAI database');
        
        // Handle errors
        aiPool.on('error', err => {
            console.error('[AI DB] ❌ Connection error:', err);
        });
        
        return aiPool;
    } catch (error) {
        console.error('[AI DB] ❌ Failed to connect:', error.message);
        // Return a mock pool that doesn't crash the app
        const mockRequest = () => ({
            input: function() { return this; },
            query: async () => ({ recordset: [] })
        });
        return {
            request: mockRequest,
            query: async () => ({ recordset: [] })
        };
    }
}

// Initialize on require
initAiPool();

// Fallback mock for exports
const mockRequest = () => ({
    input: function() { return this; },
    query: async () => ({ recordset: [] })
});

module.exports = {
    aiPool: aiPool || { request: mockRequest, query: async () => ({ recordset: [] }) },
    aiPoolConnect: aiPoolConnect || Promise.resolve()
};
