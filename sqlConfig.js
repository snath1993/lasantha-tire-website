// SQL Server configuration
const sql = require('mssql');
require('dotenv').config();

// Basic SQL Server configuration
const isProd = String(process.env.NODE_ENV || '').toLowerCase() === 'production';

const disableInstanceLookup = ['1', 'true', 'yes', 'on'].includes(String(process.env.SQL_DISABLE_INSTANCE_LOOKUP || '').toLowerCase());
const hasExplicitPort = process.env.SQL_PORT != null && String(process.env.SQL_PORT).trim() !== '';

const rawServer = process.env.SQL_SERVER || (isProd ? '' : 'WIN-JIAVRTFMA0N\\SQLEXPRESS');
const configuredPort = parseInt(process.env.SQL_PORT || '1433', 10);

let serverHost = rawServer;
let instanceName = null;
if (rawServer.includes('\\')) {
    const parts = rawServer.split('\\');
    serverHost = parts[0];
    instanceName = parts[1] || null;
}

const options = {
    encrypt: false,
    trustServerCertificate: true,
    cryptoCredentialsDetails: {
        minVersion: 'TLSv1'
    }
};

// SQL Browser / instance lookup behavior:
// - Default: if server is HOST\\INSTANCE and no explicit SQL_PORT is provided, allow instanceName so the driver can locate the instance.
// - If SQL_PORT is explicitly set (even 1433), or SQL_DISABLE_INSTANCE_LOOKUP=true, prefer host+port to avoid EINSTLOOKUP/UDP 1434 dependency.
if (instanceName && !disableInstanceLookup && !hasExplicitPort) {
    options.instanceName = instanceName;
}

if (isProd) {
    const missing = [];
    ['SQL_SERVER', 'SQL_USER', 'SQL_PASSWORD', 'SQL_DATABASE'].forEach((k) => {
        if (!process.env[k] || String(process.env[k]).trim() === '') missing.push(k);
    });
    if (missing.length) {
        throw new Error(`Missing required SQL env vars in production: ${missing.join(', ')}`);
    }
} else {
    // Non-production: allow legacy defaults but warn loudly.
    if (!process.env.SQL_USER || !process.env.SQL_PASSWORD) {
        console.warn('[SQL Config] ⚠️ Using legacy fallback SQL credentials (non-production). Set SQL_USER/SQL_PASSWORD to secure values.');
    }
}

const config = {
    user: process.env.SQL_USER || '',
    password: process.env.SQL_PASSWORD || '',
    server: serverHost,
    database: process.env.SQL_DATABASE || '',
    port: configuredPort,
    options,
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

// For debugging
console.log('SQL Server config:', {
    server: instanceName ? `${serverHost}\\${instanceName}` : config.server,
    database: config.database,
    user: config.user,
    port: config.port
    // password omitted for security
});

module.exports = { sql, config };
// Also export config as default for backward compatibility
module.exports.default = config;