// =================================================================
// AI Database Connection Utility (aiDbConnection.js)
// Connects specifically to the 'WhatsAppAI' database.
// =================================================================

const mssql = require('mssql');

// Load environment variables
require('dotenv').config();

// Configuration for the AI-specific database connection.
// It uses the main server credentials but targets the 'WhatsAppAI' database.
const AI_DB_ENABLED = String(process.env.ENABLE_AI_DB || process.env.ENABLE_AI_COPILOT || 'true').toLowerCase() === 'true';

const disableInstanceLookup = ['1', 'true', 'yes', 'on'].includes(String(process.env.SQL_DISABLE_INSTANCE_LOOKUP || '').toLowerCase());
const hasExplicitPort = process.env.SQL_PORT != null && String(process.env.SQL_PORT).trim() !== '';

const rawServer = process.env.SQL_SERVER;
const configuredPort = parseInt(process.env.SQL_PORT, 10) || 1433;

let serverHost = rawServer;
let instanceName = null;
if (rawServer && rawServer.includes('\\')) {
    const parts = rawServer.split('\\');
    serverHost = parts[0];
    instanceName = parts[1] || null;
}

const options = {
    encrypt: String(process.env.DB_ENCRYPT || 'false').toLowerCase() === 'true',
    trustServerCertificate: String(process.env.DB_TRUST_CERT || 'true').toLowerCase() === 'true',
    cryptoCredentialsDetails: {
        minVersion: 'TLSv1'
    }
};

// Match main SQL config behavior:
// - If server is HOST\\INSTANCE and no explicit SQL_PORT is provided, allow instanceName lookup.
// - If SQL_PORT is explicitly set (even 1433) or SQL_DISABLE_INSTANCE_LOOKUP=true, prefer host+port.
if (instanceName && !disableInstanceLookup && !hasExplicitPort) {
    options.instanceName = instanceName;
}

const aiDbConfig = {
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    server: serverHost,
    database: 'WhatsAppAI',
    port: configuredPort,
    options,
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    },
    connectionTimeout: parseInt(process.env.SQL_CONNECTION_TIMEOUT_MS || '30000', 10),
    requestTimeout: parseInt(process.env.SQL_REQUEST_TIMEOUT_MS || '30000', 10)
};

// Create a dedicated connection pool for the AI database with retry and error capture.
let _internalPool = new mssql.ConnectionPool(aiDbConfig);

// Export a stable wrapper so callers that destructure `aiPool` keep working even if we recreate the pool.
const aiPool = {
    get connected() { return _internalPool?.connected; },
    request() {
        if (!AI_DB_ENABLED) {
            throw new Error('AI DB disabled');
        }
        return _internalPool.request();
    },
    close() { return _internalPool.close(); },
    __setInternalPool(nextPool) { _internalPool = nextPool; }
};

let _aiPoolConnectPromise = null;
let aiDbRetryCount = 0;
let lastAiDbError = null;

// Getter to always return current connection promise (avoids race condition on import)
const aiPoolConnect = {
    then(resolve, reject) {
        return (_aiPoolConnectPromise || Promise.resolve(false)).then(resolve, reject);
    },
    catch(reject) {
        return (_aiPoolConnectPromise || Promise.resolve(false)).catch(reject);
    }
};

function connectAiDb(initial = false) {
    if (!AI_DB_ENABLED) {
        if (initial) console.log('[AI DB] Disabled (ENABLE_AI_DB/ENABLE_AI_COPILOT=false)');
        _aiPoolConnectPromise = Promise.resolve(false);
        return _aiPoolConnectPromise;
    }

    // Ensure error handler is attached for the current internal pool
    try {
        _internalPool.on('error', err => {
            console.error('[AI DB Error]', err);
        });
    } catch {}

    if (initial) {
        console.log('[AI DB] Connection pool configured for WhatsAppAI database.');
    }
    _aiPoolConnectPromise = _internalPool.connect()
        .then(() => {
            aiDbRetryCount = 0;
            lastAiDbError = null;
            console.log('[AI DB] ✅ Connected to WhatsAppAI');
            return true;
        })
        .catch(err => {
            lastAiDbError = err;
            aiDbRetryCount += 1;
            console.error(`[AI DB] ❌ Connect failed (${err.code || err.name || 'ERROR'}): ${err.message}`);
            const delay = Math.min(60000, 15000 * Math.max(1, aiDbRetryCount));
            setTimeout(() => {
                try {
                    const next = new mssql.ConnectionPool(aiDbConfig);
                    aiPool.__setInternalPool(next);
                } catch {}
                connectAiDb(false);
            }, delay);
            return false;
        });

    return _aiPoolConnectPromise;
}

connectAiDb(true);

function getAiRawPool() {
    if (!AI_DB_ENABLED) {
        throw new Error('AI DB disabled');
    }
    return _internalPool;
}

// Export the pool and a query function for easy use.
module.exports = {
    aiPool,
    aiPoolConnect,
    getAiRawPool,
    getAiDbStatus: () => ({ retryCount: aiDbRetryCount, lastError: lastAiDbError }),
    query: (text, params) => {
        if (!AI_DB_ENABLED || !aiPool) {
            return Promise.reject(new Error('AI DB disabled'));
        }
        return aiPool.request().query(text);
    }
};
