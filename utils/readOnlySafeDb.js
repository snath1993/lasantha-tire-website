const sql = require('mssql');
require('dotenv').config();

// Ensure we have a pool explicitly for this safely
// const { poolPromise } = require('./db');

const dbConfig = {
    server: process.env.SQL_SERVER || 'WIN-JIAVRTFMA0N\\SQLEXPRESS',
    database: process.env.SQL_DATABASE || 'LasanthaTire',
    user: process.env.SQL_USER || 'sa',
    password: process.env.SQL_PASSWORD,
    port: parseInt(process.env.SQL_PORT || '1433'),
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

/**
 * Executes a SQL query ONLY if it is a read-only operation.
 * Blocks INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE, EXEC, MERGE, GRANT, REVOKE.
 * @param {string} query The SQL query to execute
 * @returns {Promise<any>} The recordset
 */
async function executeSafely(query) {
    // 1. Strict Keyword Analysis
    const forbiddenKeywords = [
        'INSERT', 'UPDATE', 'DELETE', 'DROP', 'ALTER', 'TRUNCATE', 
        'EXEC', 'EXECUTE', 'MERGE', 'GRANT', 'REVOKE', 'CREATE', 
        'BACKUP', 'RESTORE', 'DBCC'
    ];

    // Normalize query for checking (remove comments could be tricky, but basic check first)
    // We remove SQL comments -- and /* */ to avoid bypass
    const cleanQuery = query.replace(/--.*/g, '').replace(/\/\*[\s\S]*?\*\//g, '').toUpperCase();

    // Word boundary check to ensure we don't catch "UPDATE_DATE" column as "UPDATE" command
    // \b matches word boundaries
    const hasForbidden = forbiddenKeywords.some(keyword => {
        const regex = new RegExp(`\\b${keyword}\\b`);
        return regex.test(cleanQuery);
    });

    if (hasForbidden) {
        throw new Error(`🛡️ SECURITY ALERT: The AI attempted a forbidden database operation. 
        Operation blocked: ${query.substring(0, 50)}...
        Allowed mode: READ-ONLY (SELECT only).`);
    }

    // 2. Connect and Execute
    try {
        const pool = await new sql.ConnectionPool(dbConfig).connect();
        const result = await pool.request().query(query);
        await pool.close();
        return result.recordset;
    } catch (err) {
        throw new Error(`Database Query Error: ${err.message}`);
    }
}

module.exports = { executeSafely };
