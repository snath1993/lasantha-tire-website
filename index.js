require('dotenv').config();
const { ConfigService } = require('./utils/ConfigService');
const { Client, LocalAuth, MessageMedia } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');
const sql = require('mssql');
const fs = require('fs');
const path = require('path');
const http = require('http');
const net = require('net');
const express = require('express');
const bodyParser = require('body-parser');
const crypto = require('crypto');
const rateLimit = require('express-rate-limit');
const {
    enqueueQuoteRequest,
    getQueuedQuoteRequests,
    replaceQueuedQuoteRequests,
    ensureQueueFile,
    getQueueFilePath
} = require('./shared/quoteRequestQueue');

// ========================================
// ENVIRONMENT VALIDATION (fail-fast for production safety)
// ========================================
const REQUIRED_ENV_GROUPS = [
    ['SQL_SERVER', 'DB_SERVER'],
    ['SQL_USER', 'DB_USER'],
    ['SQL_PASSWORD', 'DB_PASSWORD'],
    ['SQL_DATABASE', 'DB_NAME']
];

const REQUIRED_ENV_VARS = ['FACEBOOK_PAGE_ID', 'FACEBOOK_PAGE_ACCESS_TOKEN'];

const missingEnvVars = [];

REQUIRED_ENV_GROUPS.forEach(([primary, fallback]) => {
    const primaryValue = process.env[primary]?.trim();
    const fallbackValue = fallback ? process.env[fallback]?.trim() : undefined;
    if (!primaryValue && !fallbackValue) {
        missingEnvVars.push(primary);
    }
});

REQUIRED_ENV_VARS.forEach((envName) => {
    if (!process.env[envName] || process.env[envName].trim() === '') {
        missingEnvVars.push(envName);
    }
});

if (missingEnvVars.length > 0) {
    console.error('❌ Critical configuration error: Missing required environment variables.');
    console.error('   Provide values for:', missingEnvVars.join(', '));
    console.error('   Refer to .env.example for guidance. Exiting to prevent partial startup.');
    process.exit(1);
}

// --- AI & Dual-DB Imports ---
const { aiPoolConnect, aiPool } = require('./utils/aiDbConnection');
const gemini = require('./utils/geminiApiClient');
const { extractTyreSizeFlexible, extractVehicleNumber, extractRequestedQty, extractDetailRequests } = require('./utils/detect');
const { fetchTyreData } = require('./utils/fetchTyreData');
const { fetchTyreSpecs } = require('./utils/fetchTyreSpecs');
const { fetchTyreImage } = require('./utils/fetchTyreImage');
const { extractConversationContext, enhanceWithContext } = require('./utils/contextExtraction');

// --- Legacy & System Imports ---
const isSystemSender = require('./utils/isSystemSender');
const safeReply = require('./utils/safeReply');
const TyrePriceReplyJob = require('./jobs/TyrePriceReplyJob');
const TyreQtyReplyJob = require('./jobs/TyreQtyReplyJob');
const TyreQuotationPDFLibJob = require('./jobs/TyreQuotationPDFLibJob');
const VehicleInvoiceReplyJob = require('./jobs/VehicleInvoiceReplyJob');
const { setClient } = require('./utils/waClientRegistry');
const { initializeScheduler } = require('./scheduler');

// --- Advanced Job System ---
const { registry } = require('./utils/JobRegistry');
const { orchestrator } = require('./utils/JobOrchestrator');
const { registerAllJobs } = require('./utils/registerJobs');

// --- Daily Accounting Report Job ---
const { initializeDailyAccountingReport } = require('./jobs/DailyAccountingReportJob');

// --- Scheduled Media Publisher Job ---
const ScheduledMediaPublisherJob = require('./jobs/ScheduledMediaPublisherJob');

// --- Session Backup System ---
const { startSessionBackup, restoreSessionFromBackup } = require('./utils/sessionBackup');
const WebJobHandler = require('./handlers/WebJobHandler');

// ========================================
// GLOBAL ERROR HANDLERS
// ========================================
// Handle unhandled promise rejections (like WhatsApp Web cache errors)
process.on('unhandledRejection', (reason, promise) => {
    console.error('💥 Unhandled Rejection at:', promise);
    console.error('   Reason:', reason);
    
    // Don't crash on WhatsApp Web.js cache errors - they're usually non-fatal
    if (reason && reason.message && reason.message.includes('LocalWebCache')) {
        console.warn('⚠️  WhatsApp Web cache error detected - continuing execution');
        return;
    }
    
    // Log to file for debugging
    const errorLog = `[${new Date().toISOString()}] Unhandled Rejection: ${reason}\n`;
    try {
        fs.appendFileSync('error.log', errorLog);
    } catch {}
});

// Handle uncaught exceptions
process.on('uncaughtException', (error) => {
    console.error('💥 Uncaught Exception:', error);
    
    // Don't crash on WhatsApp Web.js cache errors
    if (error && error.message && error.message.includes('LocalWebCache')) {
        console.warn('⚠️  WhatsApp Web cache error detected - continuing execution');
        return;
    }
    
    // Log to file
    const errorLog = `[${new Date().toISOString()}] Uncaught Exception: ${error.stack}\n`;
    try {
        fs.appendFileSync('error.log', errorLog);
    } catch {}
    
    // Exit on critical errors
    console.error('💀 Critical error - shutting down gracefully...');
    process.exit(1);
});

// --- Constants ---
const LOG_FILE = 'whatsapp-bot.log';
const STATUS_FILE = path.join(__dirname, 'whatsapp-status.json');
// Snapshot current session folder structure for debugging
function snapshotSession() {
    const snap = {
        timestamp: new Date().toISOString(),
        authPath: AUTH_PATH,
        exists: false,
        sessionFolders: [],
        totalFiles: 0,
        keyArtifacts: {
            indexedDB: false,
            localStorage: false,
            serviceWorker: false,
            cookiesFile: false
        }
    };
    try {
        if (fs.existsSync(AUTH_PATH)) {
            snap.exists = true;
            const entries = fs.readdirSync(AUTH_PATH, { withFileTypes: true });
            for (const ent of entries) {
                if (ent.isDirectory() && ent.name.startsWith('session-')) {
                    const sessionDir = path.join(AUTH_PATH, ent.name);
                    const stat = fs.statSync(sessionDir);
                    const details = { name: ent.name, mtime: stat.mtime, fileCount: 0, sizeBytes: 0 };
                    const walk = (dir) => {
                        try {
                            const items = fs.readdirSync(dir, { withFileTypes: true });
                            for (const it of items) {
                                const full = path.join(dir, it.name);
                                try {
                                    if (it.isDirectory()) walk(full); else {
                                        details.fileCount++; const s = fs.statSync(full); details.sizeBytes += s.size;
                                    }
                                } catch {}
                            }
                        } catch {}
                    };
                    walk(sessionDir);
                    snap.sessionFolders.push(details);
                }
            }
            // Detect key artifacts in primary session folder (clientId specific)
            const primary = path.join(AUTH_PATH, 'session-lasantha-tire-bot');
            snap.keyArtifacts.indexedDB = fs.existsSync(path.join(primary, 'Default', 'IndexedDB'));
            snap.keyArtifacts.localStorage = fs.existsSync(path.join(primary, 'Default', 'Local Storage'));
            snap.keyArtifacts.serviceWorker = fs.existsSync(path.join(primary, 'Default', 'Service Worker'));
            // Common cookie store
            const cookiesPattern = path.join(primary, 'Default', 'Cookies');
            snap.keyArtifacts.cookiesFile = fs.existsSync(cookiesPattern);
            snap.totalFiles = snap.sessionFolders.reduce((a, f) => a + f.fileCount, 0);
        }
    } catch (e) {
        snap.error = e.message;
    }
    try { fs.writeFileSync(path.join(__dirname, 'session-debug.json'), JSON.stringify(snap, null, 2)); } catch {}
    return snap;
}
// Use absolute path for LocalAuth to avoid CWD differences on restart
const AUTH_PATH = path.join(__dirname, '.wwebjs_auth');
const BOT_API_PORT = parseInt(process.env.BOT_API_PORT || '3100', 10);
// Admin numbers can be provided as ADMIN_NUMBERS (comma-separated) or a single ADMIN_WHATSAPP_NUMBER
const ADMIN_NUMBERS_RAW = (((process.env.ADMIN_NUMBERS || '') + ',' + (process.env.ADMIN_WHATSAPP_NUMBER || ''))
    .split(',')
    .map(n => n.trim())
    .filter(Boolean));

// Helper function to normalize phone numbers for comparison (handles 0771222509 vs 94771222509)
function normalizePhoneNumber(phone) {
    const digits = phone.replace(/\D/g, ''); // Remove non-digits
    // Convert 0771222509 → 94771222509
    if (digits.startsWith('0') && digits.length === 10) {
        return '94' + digits.substring(1);
    }
    return digits;
}

// Check if a sender number (from WhatsApp) is an admin
function isAdminNumber(senderFrom) {
    // senderFrom is like "94771222509@c.us" or "0771222509"
    const senderDigits = normalizePhoneNumber(senderFrom);
    return ADMIN_NUMBERS_RAW.some(adminNum => {
        const adminDigits = normalizePhoneNumber(adminNum);
        return senderDigits === adminDigits;
    });
}

const ADMIN_NUMBERS = ADMIN_NUMBERS_RAW; // Keep for backward compatibility
const AI_ENABLED = String(process.env.ENABLE_AI_COPILOT || 'true').toLowerCase() === 'true';
const AI_CONTEXT_MESSAGES = parseInt(process.env.AI_CONTEXT_MESSAGES, 10) || 5;
// Session persistence control flags (set in .env if you want aggressive cleanup)
const CLEAR_SESSION_ON_AUTH_FAILURE = String(process.env.CLEAR_SESSION_ON_AUTH_FAILURE || 'false').toLowerCase() === 'true';
const CLEAR_SESSION_ON_INIT_ERROR = String(process.env.CLEAR_SESSION_ON_INIT_ERROR || 'false').toLowerCase() === 'true';

// WhatsApp QR Code storage for dashboard
let currentQRCode = null;
let qrCodeTimestamp = null;

// --- Express App Setup ---
const app = express();
app.set('trust proxy', 1);

// For signature verification, we need access to raw body
// Apply raw body parser ONLY to Facebook webhook
app.use('/facebook/webhook', express.raw({ type: 'application/json' }));

// For all other routes, use JSON parser
app.use((req, res, next) => {
    if (!req.path.startsWith('/facebook/webhook')) {
        express.json()(req, res, next);
    } else {
        next();
    }
});

app.use(bodyParser.json());

// Apply rate limiting to API endpoints (protects against abuse / brute-force)
const apiRateLimitMax = parseInt(process.env.API_RATE_LIMIT_MAX || '300', 10);
const apiRateLimiter = rateLimit({
    windowMs: 15 * 60 * 1000, // 15 minutes
    max: apiRateLimitMax,
    standardHeaders: true,
    legacyHeaders: false,
    message: {
        status: 429,
        error: 'Too many requests from this IP, please try again later.'
    }
});
app.use('/api', apiRateLimiter);

// ========================================
// CORS MIDDLEWARE FOR DASHBOARD
// ========================================
app.use((req, res, next) => {
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization');
    res.setHeader('Access-Control-Allow-Credentials', 'true');
    
    // Handle preflight
    if (req.method === 'OPTIONS') {
        return res.sendStatus(200);
    }
    
    next();
});

// ========================================
// HEALTH CHECK & MONITORING ENDPOINTS
// ========================================

// Health check endpoint
app.get('/health', async (req, res) => {
    const health = {
        status: 'ok',
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        service: 'WhatsApp Bot API',
        version: '1.0.0',
        checks: {},
        db: {
            main: {
                configuredDatabase: sqlConfig.database,
                server: sqlConfig.server,
                retryCount: typeof mainDbRetryCount !== 'undefined' ? mainDbRetryCount : 0,
                lastError: (typeof lastMainDbError !== 'undefined' && lastMainDbError) ? (lastMainDbError.message || String(lastMainDbError)) : null
            }
        }
    };

    try {
        // Check WhatsApp client status
        if (global.whatsappClient) {
            const waInfo = global.whatsappClient.info;
            health.checks.whatsapp = {
                status: waInfo ? 'connected' : 'disconnected',
                phone: waInfo?.wid?.user || 'unknown',
                platform: waInfo?.platform || 'unknown'
            };
        } else {
            health.checks.whatsapp = { status: 'not_initialized' };
        }

        // Check Main DB (LasanthaTire) without hanging the endpoint
        try {
            if (mainPool && mainPool.connected) {
                // light probe, but guard errors
                try {
                    await mainPool.request().query('SELECT 1 as test');
                    health.checks.mainDatabase = {
                        status: 'connected',
                        database: sqlConfig.database,
                        server: sqlConfig.server
                    };
                } catch (probeErr) {
                    health.checks.mainDatabase = {
                        status: 'connected_but_probe_failed',
                        error: probeErr.message,
                        database: sqlConfig.database,
                        server: sqlConfig.server
                    };
                    health.status = 'degraded';
                }
            } else {
                health.checks.mainDatabase = {
                    status: 'disconnected',
                    error: (lastMainDbError && lastMainDbError.message) || 'Not connected'
                };
                health.status = 'degraded';
            }
        } catch (err) {
            health.checks.mainDatabase = {
                status: 'error',
                error: err.message
            };
            health.status = 'degraded';
        }

        // Check AI DB (WhatsAppAI)
        try {
            await aiPoolConnect;
            const result = await aiPool.request().query('SELECT 1 as test');
            health.checks.aiDatabase = {
                status: 'connected',
                database: 'WhatsAppAI'
            };
        } catch (err) {
            health.checks.aiDatabase = {
                status: 'error',
                error: err.message
            };
            health.status = 'degraded';
        }

        // Check file system
        health.checks.filesystem = {
            auth_dir: fs.existsSync(AUTH_PATH),
            tyre_images: fs.existsSync('./tyre-images'),
            results_dir: fs.existsSync('./results')
        };

        // Memory usage
        const mem = process.memoryUsage();
        health.memory = {
            rss_mb: (mem.rss / 1024 / 1024).toFixed(2),
            heapUsed_mb: (mem.heapUsed / 1024 / 1024).toFixed(2),
            heapTotal_mb: (mem.heapTotal / 1024 / 1024).toFixed(2)
        };

        const httpCode = health.status === 'ok' ? 200 : 503;
        res.status(httpCode).json(health);

    } catch (error) {
        health.status = 'error';
        health.error = error.message;
        res.status(503).json(health);
    }
});

// Stats endpoint
app.get('/stats', async (req, res) => {
    try {
        await aiPoolConnect;
        
        // Get customer stats
        const customerStats = await aiPool.request().query(`
            SELECT 
                COUNT(*) as total_customers,
                SUM(CASE WHEN whatsapp_chat_active = 1 THEN 1 ELSE 0 END) as whatsapp_active,
                SUM(CASE WHEN ai_interaction_count > 0 THEN 1 ELSE 0 END) as ai_users,
                SUM(CASE WHEN total_purchases > 0 THEN 1 ELSE 0 END) as paying_customers,
                AVG(CAST(ai_interaction_count as FLOAT)) as avg_ai_interactions,
                SUM(ai_interaction_count) as total_ai_interactions
            FROM customer_details
        `);

        // Get recent AI conversations
        const recentAI = await aiPool.request().query(`
            SELECT COUNT(*) as count
            FROM ai_conversations
            WHERE last_ai_interaction >= DATEADD(hour, -24, GETDATE())
        `);

        res.json({
            timestamp: new Date().toISOString(),
            customers: customerStats.recordset[0],
            recent_ai_chats_24h: recentAI.recordset[0].count
        });

    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ========================================
// CONFIGURATION & TOKENS API (for Desktop App)
// ========================================

// Get full configuration (backed by config/settings.json)
app.get('/api/config', (req, res) => {
    try {
        const data = ConfigService.all();
        return res.json({ ok: true, data });
    } catch (error) {
        return res.status(500).json({ ok: false, error: error.message });
    }
});

// Replace configuration (upsert keys)
app.put('/api/config', (req, res) => {
    try {
        const payload = req.body || {};
        if (typeof payload !== 'object' || Array.isArray(payload)) {
            return res.status(400).json({ ok: false, error: 'Invalid payload' });
        }
        Object.entries(payload).forEach(([k, v]) => ConfigService.set(k, v));
        const data = ConfigService.all();
        return res.json({ ok: true, data });
    } catch (error) {
        return res.status(500).json({ ok: false, error: error.message });
    }
});

// Token status summary for UI badges
app.get('/api/token-status', (req, res) => {
    try {
        const cfg = ConfigService.all();
        const keys = [
            'FACEBOOK_PAGE_ID',
            'FACEBOOK_PAGE_ACCESS_TOKEN',
            'FACEBOOK_APP_SECRET',
            'FACEBOOK_VERIFY_TOKEN',
            'ANTHROPIC_API_KEY',
            'GEMINI_API_KEY'
        ];
        const tokens = {};
        const now = new Date().toISOString();
        keys.forEach(k => {
            const val = (cfg[k] ?? '').toString();
            tokens[k] = { active: !!val && val.length > 0, length: val.length || 0, updatedAt: now };
        });

        const facebook = {
            configured: !!(cfg['FACEBOOK_PAGE_ACCESS_TOKEN']),
            lastCheck: now,
            lastRefresh: null,
            expiresAt: null,
            refreshCount: null,
            recentErrors: []
        };

        return res.json({ ok: true, data: { tokens, facebook, configUpdatedAt: now } });
    } catch (error) {
        return res.status(500).json({ ok: false, error: error.message });
    }
});

// Lightweight test endpoints (no external calls)
app.post('/api/test/facebook', (req, res) => {
    const { pageId, accessToken } = req.body || {};
    if (!pageId || !accessToken) {
        return res.status(400).json({ ok: false, error: 'Missing pageId or accessToken' });
    }
    // Basic sanity validation only
    const ok = typeof accessToken === 'string' && accessToken.length > 20;
    return res.json({ ok, data: ok ? { data: { name: 'Facebook Page (stub)' } } : null, error: ok ? null : 'Token looks invalid' });
});

// ========================================
// APPOINTMENT BOOKING API
// ========================================
app.post('/api/appointments/book', async (req, res) => {
    const { customerName, phoneNumber, vehicleNumber, serviceType, appointmentDate, timeSlot, notes } = req.body;

    if (!customerName || !phoneNumber || !serviceType || !appointmentDate || !timeSlot) {
        return res.status(400).json({ ok: false, error: 'Missing required fields' });
    }

    try {
        await aiPoolConnect;

        // Generate Reference Number (APT-YYYYMMDD-XXXX)
        const dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        const randomSuffix = Math.floor(1000 + Math.random() * 9000);
        const referenceNo = `APT-${dateStr}-${randomSuffix}`;

        // Insert into Database
        const request = new sql.Request(aiPool);
        request.input('ReferenceNo', sql.NVarChar(50), referenceNo);
        request.input('CustomerName', sql.NVarChar(100), customerName);
        request.input('PhoneNumber', sql.NVarChar(20), phoneNumber);
        request.input('VehicleNumber', sql.NVarChar(20), vehicleNumber || null);
        request.input('ServiceType', sql.NVarChar(50), serviceType);
        request.input('AppointmentDate', sql.Date, appointmentDate);
        request.input('TimeSlot', sql.NVarChar(20), timeSlot);
        request.input('Notes', sql.NVarChar(sql.MAX), notes || null);

        await request.query(`
            INSERT INTO Appointments (ReferenceNo, CustomerName, PhoneNumber, VehicleNumber, ServiceType, AppointmentDate, TimeSlot, Status, Notes)
            VALUES (@ReferenceNo, @CustomerName, @PhoneNumber, @VehicleNumber, @ServiceType, @AppointmentDate, @TimeSlot, 'Pending', @Notes)
        `);

        // Send WhatsApp Confirmation
        if (global.whatsappClient) {
            const formattedPhone = normalizePhoneNumber(phoneNumber);
            const chatId = `${formattedPhone}@c.us`;
            const message = `🚗 *Appointment Confirmed!* 🚗\n\n` +
                            `Dear ${customerName},\n` +
                            `Your appointment for *${serviceType}* has been booked successfully.\n\n` +
                            `📅 Date: ${appointmentDate}\n` +
                            `⏰ Time: ${timeSlot}\n` +
                            `🔢 Ref No: *${referenceNo}*\n\n` +
                            `Thank you for choosing Lasantha Tyre!`;
            
            try {
                await global.whatsappClient.sendMessage(chatId, message);
            } catch (waError) {
                console.error('Failed to send WhatsApp confirmation:', waError);
                // Don't fail the request if WhatsApp fails, just log it
            }

            // Send Notification to Shop Management Group
            const SHOP_MANAGEMENT_GROUP_ID = process.env.SHOP_MANAGEMENT_GROUP_ID;
            if (SHOP_MANAGEMENT_GROUP_ID) {
                const adminMsg = `🆕 *New Appointment Alert* 🆕\n\n` +
                                 `👤 Customer: ${customerName}\n` +
                                 `📞 Phone: ${phoneNumber}\n` +
                                 `🚗 Vehicle: ${vehicleNumber || 'N/A'}\n` +
                                 `🔧 Service: ${serviceType}\n` +
                                 `📅 Date: ${appointmentDate}\n` +
                                 `⏰ Time: ${timeSlot}\n` +
                                 `📝 Notes: ${notes || 'None'}\n` +
                                 `🔢 Ref: ${referenceNo}`;
                try {
                    await global.whatsappClient.sendMessage(SHOP_MANAGEMENT_GROUP_ID, adminMsg);
                    console.log(`Notification sent to management group: ${SHOP_MANAGEMENT_GROUP_ID}`);
                } catch (groupErr) {
                    console.error('Failed to send group notification:', groupErr);
                }
            }
        }

        res.json({ ok: true, referenceNo, message: 'Appointment booked successfully' });

    } catch (error) {
        console.error('Appointment booking error:', error);
        res.status(500).json({ ok: false, error: error.message });
    }
});

// ========================================
// QUOTATION LINK API
// ========================================
app.get('/api/quotations/:refId', async (req, res) => {
    const { refId } = req.params;

    if (!refId) {
        return res.status(400).json({ ok: false, error: 'Missing Ref ID' });
    }

    try {
        const pool = mainPool && mainPool.connected ? mainPool : await sql.connect(sqlConfig);

        const result = await pool.request()
            .input('RefID', sql.Int, parseInt(refId, 10))
            .query(`
                SELECT RefID, ItemsJson, CustomerName, CustomerPhone, CreatedAt
                FROM QuotationLinks
                WHERE RefID = @RefID
            `);

        if (result.recordset.length === 0) {
            return res.status(404).json({ ok: false, error: 'Quotation not found' });
        }

        const quote = result.recordset[0];
        // Parse ItemsJson if it's a string
        try {
            if (typeof quote.ItemsJson === 'string') {
                quote.ItemsJson = JSON.parse(quote.ItemsJson);
            }
        } catch (e) {
            console.error('Error parsing ItemsJson:', e);
        }

        res.json({ ok: true, data: quote });

    } catch (error) {
        console.error('Error fetching quotation:', error);
        res.status(500).json({ ok: false, error: error.message });
    }
});

app.post('/api/test/anthropic', (req, res) => {
    const { apiKey } = req.body || {};
    const ok = typeof apiKey === 'string' && apiKey.trim().length > 10;
    return res.json({ ok, data: ok ? { provider: 'anthropic' } : null, error: ok ? null : 'Missing/short API key' });
});

app.post('/api/test/gemini', (req, res) => {
    const { apiKey } = req.body || {};
    const ok = typeof apiKey === 'string' && apiKey.trim().length > 10;
    return res.json({ ok, data: ok ? { provider: 'gemini' } : null, error: ok ? null : 'Missing/short API key' });
});

// Advanced Job System Statistics
app.get('/api/jobs/stats', (req, res) => {
    try {
        const jobStats = registry.getStats();
        const workflowStats = orchestrator.getStats();
        
        res.json({
            timestamp: new Date().toISOString(),
            jobSystem: jobStats,
            workflows: workflowStats
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Trigger a job manually
app.post('/api/jobs/run', async (req, res) => {
    try {
        let { jobId, text = '' } = req.body || {};
        if (!jobId) return res.status(400).json({ ok: false, error: 'jobId required' });

        // Handle "Job" suffix mismatch (e.g. UI sends TyrePriceReplyJob, registry has TyrePriceReply)
        if (!registry.jobs.has(jobId) && jobId.endsWith('Job')) {
            const strippedId = jobId.slice(0, -3);
            if (registry.jobs.has(strippedId)) {
                jobId = strippedId;
            }
        }

        if (!registry.jobs.has(jobId)) return res.status(404).json({ ok: false, error: 'Job not found' });
        const startedAt = Date.now();
        // Broadcast job start over SSE for desktop UI
        sseBroadcast('job_status', {
            jobId,
            status: 'started',
            isRunning: true,
            lastRun: null
        });

        const dummyMsg = { body: text, from: 'desktop@app' };
        const context = {
            logAndSave: (m) => console.log('[ManualJob]', m)
        };
    await registry.execute(jobId, dummyMsg, { sql: mainPool, sqlConfig, allowedContacts, context });

        const tookMs = Date.now() - startedAt;
        // Broadcast job completion
        sseBroadcast('job_status', {
            jobId,
            status: 'completed',
            isRunning: false,
            lastRun: new Date().toISOString(),
            durationMs: tookMs
        });
        return res.json({ ok: true, tookMs });
    } catch (error) {
        try {
            // Broadcast failure over SSE so UI can surface it
            const jobId = (req.body && req.body.jobId) || 'unknown';
            sseBroadcast('job_status', {
                jobId,
                status: 'failed',
                isRunning: false,
                error: String(error && error.message ? error.message : error)
            });
        } catch {}
        return res.status(500).json({ ok: false, error: error.message });
    }
});

// ========================================
// WHATSAPP CONTROL API (minimal wiring for Desktop App)
// ========================================

app.get('/api/whatsapp/status', (req, res) => {
    try {
        if (global.whatsappClient && global.whatsappClient.info) {
            const phone = global.whatsappClient.info?.wid?.user || null;
            return res.json({ isConnected: true, phoneNumber: phone, status: 'connected' });
        }
        return res.json({ isConnected: false, phoneNumber: null, status: 'offline' });
    } catch (error) {
        return res.status(500).json({ isConnected: false, error: error.message });
    }
});

// Job Execution Manager Status (Advanced Conflict Prevention System)
app.get('/api/job-execution-status', (req, res) => {
    try {
        const { getInstance } = require('./utils/JobExecutionManager');
        const jobManager = getInstance();
        
        const status = jobManager.getStatus();
        const statistics = jobManager.getStatistics();
        
        res.json({
            timestamp: new Date().toISOString(),
            status,
            statistics,
            systemHealth: {
                activeJobs: status.runningJobs.length,
                queuedJobs: status.queuedJobs.length,
                lockedResources: status.resourceLocks.length,
                successRate: statistics.totalExecutions > 0 
                    ? Math.round((statistics.successCount / statistics.totalExecutions) * 100) 
                    : 100
            }
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.post('/api/whatsapp/login', async (req, res) => {
    try {
        console.log('[Login] Login request received');
        
        // Check if already connected
        if (global.whatsappClient && global.whatsappClient.info) {
            const phone = global.whatsappClient.info?.wid?.user || null;
            console.log('[Login] Already connected as:', phone);
            return res.json({ success: true, QRCode: null, phoneNumber: phone, session: 'active' });
        }
        
        // If no client or logged out, initialize fresh client
        if (!global.whatsappClient) {
            console.log('[Login] Initializing fresh WhatsApp client for QR login...');
            
            // Initialize client (this will trigger QR generation via SSE)
            // Use force=true to bypass reconnection cooldowns
            setTimeout(() => initializeClient(true), 100);
            
            return res.json({ 
                success: true, 
                QRCode: 'generating', 
                phoneNumber: null, 
                session: 'initializing',
                message: 'QR code will be broadcast via SSE stream'
            });
        }
        
        // Client exists but not connected yet
        return res.json({ 
            success: true, 
            QRCode: 'pending', 
            phoneNumber: null, 
            session: 'waiting',
            message: 'Waiting for QR scan'
        });
    } catch (error) {
        console.error('[Login] Error:', error);
        return res.status(500).json({ success: false, error: error.message });
    }
});

app.post('/api/whatsapp/logout', async (req, res) => {
    try {
        console.log('[Logout] Starting logout process...');
        
        // Logout WhatsApp client
        if (global.whatsappClient) {
            try { 
                await global.whatsappClient.logout();
                console.log('[Logout] WhatsApp client logged out');
            } catch (err) {
                console.error('[Logout] Error logging out client:', err);
            }
        }
        
        // Destroy client instance
        if (global.whatsappClient) {
            try {
                await global.whatsappClient.destroy();
                console.log('[Logout] WhatsApp client destroyed');
            } catch (err) {
                console.error('[Logout] Error destroying client:', err);
            }
        }
        
        global.whatsappClient = null;
        
        // Delete authentication folder to force QR scan on next login
        const authPath = path.join(__dirname, '.wwebjs_auth');
        if (fs.existsSync(authPath)) {
            try {
                fs.rmSync(authPath, { recursive: true, force: true });
                console.log('[Logout] ✅ Authentication folder deleted - QR scan required for next login');
            } catch (err) {
                console.error('[Logout] Error deleting auth folder:', err);
            }
        }
        
        // Update status file
        fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false, logout: true }, null, 2));
        
        // Broadcast disconnect event via SSE
        sseBroadcast('disconnected', { reason: 'User logged out', timestamp: Date.now() });
        
        console.log('[Logout] ✅ Logout complete - ready for fresh QR login');
        return res.json({ success: true, message: 'Logged out successfully. QR scan required for next login.' });
    } catch (error) {
        console.error('[Logout] Fatal error:', error);
        return res.status(500).json({ success: false, error: error.message });
    }
});

app.post('/api/whatsapp/send', async (req, res) => {
    const context = 'Send Message API';
    try {
        const { number, message, allowQueue = false, source = 'api', meta = {} } = req.body || {};
        const normalizedNumber = typeof number === 'string' ? number.replace(/\D/g, '') : '';
        if (!normalizedNumber || !message) {
            return res.status(400).json({ success: false, error: 'number and message required' });
        }

        if (!isWhatsAppClientReady()) {
            if (allowQueue) {
                try {
                    const entry = await enqueueQuoteRequest({
                        number: normalizedNumber,
                        message,
                        source,
                        meta
                    });
                    logAndSave(`[QuoteQueue] Queued request ${entry.id} from ${source} (${normalizedNumber})`);
                    return res.status(202).json({
                        success: false,
                        queued: true,
                        queueId: entry.id,
                        message: 'WhatsApp bot offline. Request queued and will send once connected.'
                    });
                } catch (queueError) {
                    logAndSave(`[QuoteQueue] Failed to queue request: ${queueError.message}`);
                }
            }
            return respondClientUnavailable(res, context);
        }

        const chatId = `${normalizedNumber}@c.us`;
        await global.whatsappClient.sendMessage(chatId, message);

        // Attempt to flush any queued requests now that the client is reachable
        processQueuedQuoteRequests('api_send').catch(() => {});

        return res.json({ success: true });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Dashboard-friendly endpoints for WhatsApp management
app.get('/status', (req, res) => {
    try {
        if (fs.existsSync(STATUS_FILE)) {
            const status = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
            return res.json(status);
        }
        return res.json({ ready: false });
    } catch (error) {
        return res.status(500).json({ ready: false, error: error.message });
    }
});

app.post('/connect', async (req, res) => {
    try {
        // Check if already connected
        if (global.whatsappClient && global.whatsappClient.info) {
            logAndSave('[Connect API] Already connected');
            return res.json({ success: true, message: 'Already connected', ready: true });
        }
        
        // Reset reconnection attempts if user manually triggers connect
        reconnectAttempts = 0;
        
        // Trigger initialization (broadcast initializing state)
        if (!isInitializing && !isInitialized) {
            logAndSave('[Connect API] Starting initialization...');
            try {
                fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false, initializing: true }), 'utf8');
            } catch {}
            try { sseBroadcast('initializing', { initializing: true }); } catch {}
            
            // Start initialization asynchronously
            initializeClient(true).catch(e => {
                logAndSave(`[Connect API] Init error: ${e.message}`);
            });
            
            return res.json({ success: true, message: 'Initialization started', initializing: true });
        }
        
        if (isInitializing) {
            logAndSave('[Connect API] Already initializing');
            return res.json({ success: true, message: 'Already initializing', initializing: true });
        }
        
        // Fallback - shouldn't reach here normally
        logAndSave('[Connect API] Unexpected state, forcing re-init');
        initializeClient(true).catch(e => {
            logAndSave(`[Connect API] Force init error: ${e.message}`);
        });
        return res.json({ success: true, message: 'Initialization triggered', initializing: true });
    } catch (error) {
        logAndSave(`[Connect API] Error: ${error.message}`);
        return res.status(500).json({ success: false, error: error.message });
    }
});

app.post('/logout', async (req, res) => {
    try {
        let loggedOut = false;
        
        // Try to logout from WhatsApp client if it exists and is initialized
        if (global.whatsappClient && global.whatsappClient.pupPage) {
            try {
                // Only try to logout if client has an active page (initialized)
                await global.whatsappClient.logout();
                loggedOut = true;
                console.log('[Logout] WhatsApp client logged out');
            } catch (clientError) {
                // Client might not be fully initialized or already logged out
                console.log('[Logout] Client logout skipped:', clientError.message);
            }
        }
        
        // Always clear auth data regardless of client state
        const authPath = AUTH_PATH;
        if (fs.existsSync(authPath)) {
            try {
                fs.rmSync(authPath, { recursive: true, force: true });
                console.log('[Logout] Auth directory cleared');
            } catch (fsError) {
                console.error('[Logout] Failed to clear auth directory:', fsError.message);
            }
        }
        
        // Update status file
        try {
            fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false }), 'utf8');
        } catch (e) {

        // Debug endpoint to inspect session snapshot
        app.get('/debug/session', (req, res) => {
            try {
                return res.json(snapshotSession());
            } catch (e) {
                return res.status(500).json({ error: e.message });
            }
        });
            console.error('[Logout] Failed to update status file:', e.message);
        }
        
        // Broadcast disconnect event via SSE
        try {
            sseBroadcast('disconnected', { reason: 'User logged out' });
        } catch (e) {
            console.error('[Logout] Failed to broadcast SSE:', e.message);
        }
        
        return res.json({ 
            success: true, 
            message: loggedOut ? 'Logged out successfully' : 'Session cleared (client was not connected)'
        });
    } catch (error) {
        console.error('[Logout] Error:', error);
        // Even if there's an error, try to clear auth
        try {
            if (fs.existsSync(AUTH_PATH)) {
                fs.rmSync(AUTH_PATH, { recursive: true, force: true });
            }
            fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false }), 'utf8');
        } catch (e) {}
        return res.json({ success: true, message: 'Session cleared' });
    }
});

// ========================================
// WHATSAPP MANAGEMENT ENDPOINTS
// ========================================

const SESSION_ERROR_PATTERNS = [
    'Execution context was destroyed',
    'Session closed',
    'Target closed',
    'Protocol error',
    'clientInstance is not defined',
    'Cannot read properties of undefined',
    'Evaluation failed'
];

function isWhatsAppClientReady() {
    return Boolean(global.whatsappClient && global.whatsappClient.pupPage && isInitialized);
}

function respondClientUnavailable(res, context) {
    if (global.whatsappClient) {
        triggerSessionRecovery(context, 'client unavailable via API');
    }
    return res.status(503).json({
        error: 'WhatsApp client not ready',
        status: isInitializing ? 'initializing' : 'offline',
        context
    });
}

function handleWhatsappApiError(context, error, res) {
    const message = (error && error.message) ? error.message : String(error);
    console.error(`[${context}] Error:`, error);
    if (SESSION_ERROR_PATTERNS.some(pattern => message.includes(pattern))) {
        triggerSessionRecovery(context, message);
        if (res && !res.headersSent) {
            res.status(503).json({
                error: 'WhatsApp session restarting, please retry shortly.',
                detail: message,
                context
            });
        }
        return true;
    }
    if (res && !res.headersSent) {
        res.status(500).json({ error: message, context });
    }
    return false;
}

// Get all chats
app.get('/whatsapp/chats', async (req, res) => {
    const context = 'Chats API';
    if (!isWhatsAppClientReady()) {
        return respondClientUnavailable(res, context);
    }

    try {
        const chats = await global.whatsappClient.getChats();
        const chatData = await Promise.all(chats.map(async (chat) => {
            try {
                // Get profile picture URL
                let profilePicUrl = null;
                try {
                    profilePicUrl = await chat.getProfilePicUrl();
                } catch (picErr) {
                    // Profile pic not available, use default
                    profilePicUrl = null;
                }

                return {
                    id: chat.id._serialized,
                    name: chat.name || 'Unknown',
                    isGroup: chat.isGroup,
                    unreadCount: chat.unreadCount || 0,
                    lastMessage: chat.lastMessage ? chat.lastMessage.body : '',
                    timestamp: chat.timestamp || Date.now(),
                    isPinned: chat.pinned || false,
                    isArchived: chat.archived || false,
                    profilePicUrl: profilePicUrl,
                    participantCount: chat.isGroup ? (chat.participants ? chat.participants.length : 0) : undefined
                };
            } catch (err) {
                console.error('[Chats API] Error processing chat:', err);
                return null;
            }
        }));

        // Return chats array directly (not wrapped in object)
        res.json(chatData.filter(c => c !== null));
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Get all contacts
app.get('/whatsapp/contacts', async (req, res) => {
    const context = 'Contacts API';
    if (!isWhatsAppClientReady()) {
        return respondClientUnavailable(res, context);
    }

    try {
        const contacts = await global.whatsappClient.getContacts();
        const contactData = contacts
            .filter(contact => !contact.isMe && contact.number && !contact.isGroup)
            .slice(0, 100) // Limit to 100 contacts for performance
            .map(contact => ({
                id: contact.id._serialized,
                name: contact.name || contact.pushname || contact.number,
                number: contact.number,
                isBlocked: contact.isBlocked || false,
                profilePicUrl: contact.profilePicUrl || null
            }));

        res.json({ contacts: contactData });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Get linked devices
app.get('/whatsapp/devices', async (req, res) => {
    const context = 'Devices API';
    if (!isWhatsAppClientReady()) {
        return respondClientUnavailable(res, context);
    }

    try {
        const info = global.whatsappClient.info || {};
        const devices = [
            {
                id: 'primary',
                name: info.pushname || 'Primary Device',
                type: 'primary',
                lastSeen: 'Active now',
                active: true
            }
        ];

        res.json({ devices });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Get profile info
app.get('/whatsapp/profile', async (req, res) => {
    const context = 'Profile API';
    if (!isWhatsAppClientReady()) {
        return respondClientUnavailable(res, context);
    }

    try {
        const info = global.whatsappClient.info || {};
        const state = await global.whatsappClient.getState();
        
        res.json({
            pushname: info.pushname || 'Unknown',
            number: info.wid ? info.wid.user : 'Unknown',
            platform: info.platform || 'unknown',
            status: 'Connected',
            state: state
        });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Check if WhatsApp number is registered
app.post('/api/check-whatsapp', async (req, res) => {
    const context = 'Check WhatsApp Registration';
    try {
        const { phone } = req.body;
        if (!phone) {
            return res.status(400).json({ error: 'Phone number required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        // Format phone number for WhatsApp ID
        const cleanPhone = phone.replace(/\D/g, '');
        const whatsappId = `${cleanPhone}@c.us`;

        try {
            const isRegistered = await global.whatsappClient.isRegisteredUser(whatsappId);
            res.json({ 
                registered: isRegistered,
                phone: cleanPhone,
                whatsappId: isRegistered ? whatsappId : null
            });
        } catch (checkError) {
            console.error('[Check WhatsApp] Error:', checkError);
            res.json({ 
                registered: false,
                phone: cleanPhone,
                error: 'Could not verify WhatsApp registration'
            });
        }
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Send tire price request via WhatsApp
app.post('/api/send-price-request', async (req, res) => {
    const context = 'Send Price Request';
    try {
        const { to, tireSize, quantity, customerName, vehicle } = req.body;
        
        if (!to || !tireSize) {
            return res.status(400).json({ error: 'Phone number and tire size required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        // Format phone number
        const cleanPhone = to.replace(/\D/g, '');
        const whatsappId = `${cleanPhone}@c.us`;

        // Check if registered
        const isRegistered = await global.whatsappClient.isRegisteredUser(whatsappId);
        if (!isRegistered) {
            return res.status(400).json({ 
                error: 'Phone number not registered on WhatsApp',
                registered: false
            });
        }

        // Store request in database
        let requestId = null;
        try {
            // Use aiPool for WhatsAppAI database as requested
            await aiPoolConnect; // Ensure connection is ready
            const result = await aiPool.request()
                .input('CustomerName', sql.NVarChar, customerName || 'Website Customer')
                .input('PhoneNumber', sql.NVarChar, cleanPhone)
                .input('TireSize', sql.NVarChar, tireSize)
                .input('Quantity', sql.Int, parseInt(quantity) || 1)
                .input('Vehicle', sql.NVarChar, vehicle || null)
                .input('Source', sql.NVarChar, 'Website')
                .input('Status', sql.NVarChar, 'Pending')
                .input('RequestDate', sql.DateTime, new Date())
                .query(`
                    INSERT INTO WebsiteTirePriceRequests 
                    (CustomerName, PhoneNumber, TireSize, Quantity, Vehicle, Source, Status, RequestDate)
                    VALUES 
                    (@CustomerName, @PhoneNumber, @TireSize, @Quantity, @Vehicle, @Source, @Status, @RequestDate);
                    SELECT SCOPE_IDENTITY() AS RequestId;
                `);
            if (result.recordset && result.recordset[0]) {
                requestId = result.recordset[0].RequestId;
            }
        } catch (dbError) {
            console.error('[Send Price Request] DB Log Error (Non-fatal):', dbError.message);
        }

        // Use WebJobHandler to execute the job
        try {
            const handler = new WebJobHandler(global.whatsappClient, sqlConfig, allowedContacts, logAndSave);
            await handler.handlePriceRequest({
                to: whatsappId,
                tireSize: tireSize,
                customerName: customerName
            });

            res.json({
                success: true,
                message: 'Price request processed via WebJobHandler',
                requestId: requestId,
                to: whatsappId
            });
        } catch (jobError) {
            console.error('[Send Price Request] Job Error:', jobError);
            
            // Fallback: Send direct message if job fails
            const message = `🔍 *Tire Price Request*\n\n`;
            const details = `📏 Size: ${tireSize}\n📦 Quantity: ${quantity || 1}${vehicle ? `\n🚗 Vehicle: ${vehicle}` : ''}`;
            await global.whatsappClient.sendMessage(whatsappId, message + details);
            
            res.json({
                success: true,
                message: 'Price request sent (fallback mode)',
                to: whatsappId,
                error: jobError.message
            });
        }

    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Block contact
app.post('/whatsapp/contacts/block', async (req, res) => {
    const context = 'Block Contact API';
    try {
        const { contactId } = req.body;
        if (!contactId) {
            return res.status(400).json({ error: 'contactId required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const contact = await global.whatsappClient.getContactById(contactId);
        await contact.block();
        
        res.json({ success: true, message: 'Contact blocked' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Unblock contact
app.post('/whatsapp/contacts/unblock', async (req, res) => {
    const context = 'Unblock Contact API';
    try {
        const { contactId } = req.body;
        if (!contactId) {
            return res.status(400).json({ error: 'contactId required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const contact = await global.whatsappClient.getContactById(contactId);
        await contact.unblock();
        
        res.json({ success: true, message: 'Contact unblocked' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Archive chat
app.post('/whatsapp/chats/archive', async (req, res) => {
    const context = 'Archive Chat API';
    try {
        const { chatId } = req.body;
        if (!chatId) {
            return res.status(400).json({ error: 'chatId required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const chat = await global.whatsappClient.getChatById(chatId);
        await chat.archive();
        
        res.json({ success: true, message: 'Chat archived' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Pin chat
app.post('/whatsapp/chats/pin', async (req, res) => {
    const context = 'Pin Chat API';
    try {
        const { chatId } = req.body;
        if (!chatId) {
            return res.status(400).json({ error: 'chatId required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const chat = await global.whatsappClient.getChatById(chatId);
        await chat.pin();
        
        res.json({ success: true, message: 'Chat pinned' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Delete chat
app.post('/whatsapp/chats/delete', async (req, res) => {
    const context = 'Delete Chat API';
    try {
        const { chatId } = req.body;
        if (!chatId) {
            return res.status(400).json({ error: 'chatId required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const chat = await global.whatsappClient.getChatById(chatId);
        await chat.delete();
        
        res.json({ success: true, message: 'Chat deleted' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Update profile
app.post('/whatsapp/profile/update', async (req, res) => {
    const context = 'Update Profile API';
    try {
        const { pushname, status } = req.body;
        
        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        if (pushname) {
            await global.whatsappClient.setDisplayName(pushname);
        }
        
        if (status) {
            await global.whatsappClient.setStatus(status);
        }
        
        res.json({ success: true, message: 'Profile updated' });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Export session
app.get('/whatsapp/session/export', async (req, res) => {
    try {
        const authPath = AUTH_PATH;
        if (!fs.existsSync(authPath)) {
            return res.status(404).json({ error: 'No session found' });
        }

        // Create a simple zip-like export (just tar or copy folder)
        // For now, just return session info
        res.json({ 
            success: true, 
            message: 'Session export not yet implemented',
            sessionPath: authPath,
            tip: 'You can manually backup the .wwebjs_auth folder'
        });
    } catch (error) {
        console.error('[Export Session API] Error:', error);
        res.status(500).json({ error: error.message });
    }
});

// Get messages from a specific chat
app.get('/whatsapp/messages/:chatId', async (req, res) => {
    const context = 'Messages API';
    if (!isWhatsAppClientReady()) {
        return respondClientUnavailable(res, context);
    }

    try {
        const { chatId } = req.params;
        const limit = parseInt(req.query.limit) || 50;

        const chat = await global.whatsappClient.getChatById(chatId);
        const messages = await chat.fetchMessages({ limit });

        const messageData = messages.map(msg => ({
            id: msg.id._serialized,
            body: msg.body,
            timestamp: msg.timestamp,
            from: msg.from,
            to: msg.to,
            fromMe: msg.fromMe,
            hasMedia: msg.hasMedia,
            type: msg.type,
            author: msg.author,
            ack: msg.ack,
            isForwarded: msg.isForwarded,
            hasQuotedMsg: msg.hasQuotedMsg
        }));

        res.json(messageData);
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

// Send message to a chat
app.post('/whatsapp/messages/send', async (req, res) => {
    const context = 'Send Chat Message API';
    try {
        const { chatId, message } = req.body;
        if (!chatId || !message) {
            return res.status(400).json({ error: 'chatId and message required' });
        }

        if (!isWhatsAppClientReady()) {
            return respondClientUnavailable(res, context);
        }

        const sentMessage = await global.whatsappClient.sendMessage(chatId, message);

        res.json({ 
            success: true,
            messageId: sentMessage.id._serialized,
            timestamp: sentMessage.timestamp
        });
    } catch (error) {
        handleWhatsappApiError(context, error, res);
    }
});

app.get('/sse', (req, res) => {
    // SSE headers
    res.setHeader('Content-Type', 'text/event-stream');
    res.setHeader('Cache-Control', 'no-cache');
    res.setHeader('Connection', 'keep-alive');
    res.setHeader('Access-Control-Allow-Origin', '*');

    // Send initial connection message
    res.write(`: connected ${Date.now()}\n\n`);
    
    // Add client to set
    sseClients.add(res);
    logAndSave(`[SSE] Client connected (total: ${sseClients.size})`);

    // Send current status immediately if available
    try {
        if (fs.existsSync(STATUS_FILE)) {
            const status = JSON.parse(fs.readFileSync(STATUS_FILE, 'utf8'));
            if (status.qr) {
                res.write(`event: qr\ndata: ${JSON.stringify({ qr: status.qr })}\n\n`);
            } else if (status.ready) {
                res.write(`event: ready\ndata: ${JSON.stringify({ ready: true, phoneNumber: status.phoneNumber })}\n\n`);
            }
        }
    } catch (e) {
        console.error('[SSE] Initial status send error:', e.message);
    }

    // Keep-alive ping every 25s
    const ping = setInterval(() => {
        try { res.write(`: ping ${Date.now()}\n\n`); } catch {}
    }, 25000);

    req.on('close', () => {
        clearInterval(ping);
        sseClients.delete(res);
        try { res.end(); } catch {}
        logAndSave(`[SSE] Client disconnected (total: ${sseClients.size})`);
    });
});


// Job Details
app.get('/api/jobs/:jobName', (req, res) => {
    try {
        const jobName = req.params.jobName;
        const job = registry.jobs.get(jobName);
        
        if (!job) {
            return res.status(404).json({ error: 'Job not found' });
        }

        res.json({
            name: job.name,
            description: job.description,
            priority: job.priority,
            patterns: job.patterns.map(p => p.toString()),
            requiresEntities: job.requiresEntities,
            canHandlePartial: job.canHandlePartial,
            estimatedResponseTime: job.estimatedResponseTime + 'ms',
            stats: {
                executions: job.executions,
                successes: job.successes,
                failures: job.failures,
                successRate: job.executions > 0 ? ((job.successes / job.executions) * 100).toFixed(2) + '%' : 'N/A',
                avgResponseTime: job.successes > 0 ? Math.round(job.totalTime / job.successes) + 'ms' : 'N/A'
            },
            registered: job.registered
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ========================================
// FACEBOOK WEBHOOK ENDPOINTS
// ========================================

// Facebook Webhook Verification (GET)
app.get('/facebook/webhook', (req, res) => {
    const VERIFY_TOKEN = ConfigService.get('FACEBOOK_VERIFY_TOKEN', 'lasantha_tyre_secure_2025_webhook_token');
    
    const mode = req.query['hub.mode'];
    const token = req.query['hub.verify_token'];
    const challenge = req.query['hub.challenge'];
    
    console.log('[Facebook Webhook] Verification request received');
    console.log('  Mode:', mode);
    console.log('  Token:', token ? '***' + token.slice(-4) : 'none');
    console.log('  Challenge:', challenge ? 'present' : 'none');
    
    if (mode === 'subscribe' && token === VERIFY_TOKEN) {
        console.log('✅ [Facebook Webhook] Verification successful');
        res.status(200).send(challenge);
    } else {
        console.error('❌ [Facebook Webhook] Verification failed');
        res.sendStatus(403);
    }
});

// Import Facebook comment handler
const { handleFacebookComment } = require('./utils/facebookCommentHandler');

// Facebook Webhook Events (POST) - with raw body capture for signature verification
app.post('/facebook/webhook', async (req, res) => {
    // ========================================
    // SECURITY: Verify Facebook signature
    // ========================================
    const signature = req.headers['x-hub-signature-256'];
    const appSecret = ConfigService.get('FACEBOOK_APP_SECRET');
    
    if (appSecret && signature) {
        // req.body should be a Buffer from express.raw() middleware
        const bodyBuffer = Buffer.isBuffer(req.body) ? req.body : Buffer.from(JSON.stringify(req.body));
        
        const expectedSignature = 'sha256=' + crypto
            .createHmac('sha256', appSecret)
            .update(bodyBuffer)
            .digest('hex');
        
        if (signature !== expectedSignature) {
            console.error('❌ [Facebook Webhook] Invalid signature - rejecting request');
            console.error('  Expected:', expectedSignature);
            console.error('  Received:', signature);
            console.error('  Body type:', typeof req.body);
            console.error('  Is Buffer:', Buffer.isBuffer(req.body));
            return res.status(403).send('INVALID_SIGNATURE');
        }
        console.log('✅ [Facebook Webhook] Signature verified');
    } else if (appSecret && !signature) {
        console.warn('⚠️  [Facebook Webhook] No signature provided but FACEBOOK_APP_SECRET is set - rejecting');
        return res.status(403).send('NO_SIGNATURE');
    }
    
    // Parse JSON body if it's a buffer
    const body = Buffer.isBuffer(req.body) ? JSON.parse(req.body.toString('utf8')) : req.body;
    
    console.log('[Facebook Webhook] Event received:', JSON.stringify(body, null, 2));
    
    // Always respond to Facebook immediately to avoid timeouts
    res.status(200).send('EVENT_RECEIVED');
    
    try {
        
        // Verify it's a page subscription
        if (body.object === 'page') {
            // Process entries asynchronously (don't block webhook response)
            body.entry.forEach(entry => {
                console.log('[Facebook Webhook] Processing entry:', JSON.stringify(entry, null, 2));
                
                // Handle messaging events (DMs)
                if (entry.messaging && Array.isArray(entry.messaging)) {
                    entry.messaging.forEach(event => {
                        console.log('[Facebook Webhook] Message event:', JSON.stringify(event, null, 2));
                        if (event.message && event.message.text) {
                            console.log('✅ [Facebook Webhook] Message received:', event.message.text);
                            // TODO: Handle direct messages if needed
                        }
                    });
                }
                
                // Handle feed changes (comments, posts, etc.)
                if (entry.changes && Array.isArray(entry.changes)) {
                    entry.changes.forEach(change => {
                        console.log('[Facebook Webhook] Change event:', JSON.stringify(change, null, 2));
                        
                        if (change.field === 'feed' && change.value) {
                            const value = change.value;
                            
                            // Handle comments
                            if (value.item === 'comment') {
                                console.log('✅ [Facebook Webhook] Comment detected!');
                                console.log('  - Comment ID:', value.comment_id);
                                console.log('  - Post ID:', value.post_id);
                                console.log('  - Message:', value.message);
                                console.log('  - From:', value.from);
                                console.log('  - Verb:', value.verb); // add, edit, remove
                                
                                // Process comment asynchronously
                                handleFacebookComment(value, mainPool).catch(error => {
                                    console.error('[Facebook Webhook] Error in comment handler:', error);
                                });
                            }
                            
                            // Handle posts
                            if (value.item === 'post' || value.item === 'status') {
                                console.log('✅ [Facebook Webhook] Post detected!');
                                console.log('  - Post ID:', value.post_id);
                                console.log('  - Message:', value.message);
                            }
                        }
                    });
                }
            });
        }
    } catch (error) {
        console.error('[Facebook Webhook] Error processing event:', error);
    }
});

// Shared config/utility
const sqlConfig = {
    user: process.env.SQL_USER || process.env.DB_USER,
    password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
    database: process.env.SQL_DATABASE || process.env.DB_NAME,
    server: process.env.SQL_SERVER || process.env.DB_SERVER,
    port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
    pool: { max: 10, min: 0, idleTimeoutMillis: 30000 },
    options: {
        encrypt: String(process.env.DB_ENCRYPT || 'false').toLowerCase() === 'true',
        trustServerCertificate: String(process.env.DB_TRUST_CERT || 'true').toLowerCase() === 'true'
    },
    connectionTimeout: parseInt(process.env.SQL_CONNECTION_TIMEOUT_MS || '30000', 10),
    requestTimeout: parseInt(process.env.SQL_REQUEST_TIMEOUT_MS || '30000', 10)
};

// Main production database connection pool with basic retry and error capture
let mainPool = new sql.ConnectionPool(sqlConfig);
let mainPoolConnect = null;
let mainDbRetryCount = 0;
let lastMainDbError = null;

function connectMainDb(initial = false) {
    try {
        if (initial) {
            console.log(`[Main DB] Connection pool configured for ${sqlConfig.database} database.`);
        }
        mainPool.on('error', err => console.error('[Main DB Error]', err));
    } catch {}

    mainPoolConnect = mainPool.connect()
        .then(() => {
            mainDbRetryCount = 0;
            lastMainDbError = null;
            console.log(`[Main DB] ✅ Connected to ${sqlConfig.database}`);
            return true;
        })
        .catch(err => {
            lastMainDbError = err;
            mainDbRetryCount += 1;
            console.error(`[Main DB] ❌ Connect failed (${err.code || err.name || 'ERROR'}): ${err.message}`);
            // Schedule a retry with backoff (15s, 30s, 60s ... capped at 60s)
            const delay = Math.min(60000, 15000 * Math.max(1, mainDbRetryCount));
            setTimeout(() => {
                try {
                    // Recreate pool to avoid stale internal state
                    mainPool = new sql.ConnectionPool(sqlConfig);
                } catch {}
                connectMainDb(false);
            }, delay);
            // Swallow rejection so it doesn't trigger unhandledRejection
            return false;
        });
}

connectMainDb(true);



// Load allowed contacts from jobs-config.json (root-level allowedContacts or union of job contactNumbers)
let allowedContactsConfig = [
    '94777311770',
    '94777078700',
    '94771222509',
    '94777440230',
    '94773508814',
    '94776235674'
];
// Alias used throughout legacy jobs
let allowedContacts = allowedContactsConfig;

function loadAllowedContactsFromConfig() {
    try {
        const cfgPath = require('path').join(__dirname, 'jobs-config.json');
        if (!fs.existsSync(cfgPath)) {
            logAndSave('jobs-config.json not found; using built-in allowed contacts');
            return;
        }
        const raw = fs.readFileSync(cfgPath, 'utf8');
        const json = JSON.parse(raw);
        // Prefer explicit root-level allowedContacts if present
        let next = [];
        if (Array.isArray(json.allowedContacts) && json.allowedContacts.length) {
            next = json.allowedContacts;
        } else if (Array.isArray(json.AllowedContacts) && json.AllowedContacts.length) {
            next = json.AllowedContacts;
        } else {
            // Fallback: union all job contactNumbers arrays
            const keys = Object.keys(json).filter(k => k !== 'savedAt');
            keys.forEach(k => {
                const job = json[k];
                if (job && Array.isArray(job.contactNumbers)) {
                    next.push(...job.contactNumbers);
                }
            });
        }
        // If config didn't provide anything, keep current (default) list
        if (next.length) {
            // Deduplicate and normalize simple formatting (trim, remove spaces)
            const uniq = new Set(next.map(n => String(n).replace(/\s+/g, '').replace(/^\+/, '')));
            allowedContactsConfig = Array.from(uniq);
            // keep exported alias in sync
            allowedContacts = allowedContactsConfig;
            logAndSave(`Loaded ${allowedContactsConfig.length} allowed contacts from jobs-config.json`);
        } else {
            logAndSave('No allowed contacts found in jobs-config.json; using built-in defaults');
        }
    } catch (e) {
        logAndSave(`Failed to load allowed contacts from config: ${e.message}`);
    }
}

// Initial load and watch for changes to keep in sync with UI edits
loadAllowedContactsFromConfig();
try {
    const cfgPath = require('path').join(__dirname, 'jobs-config.json');
    if (fs.existsSync(cfgPath)) {
        fs.watchFile(cfgPath, { interval: 1500 }, () => {
            logAndSave('Detected change in jobs-config.json; reloading allowed contacts...');
            loadAllowedContactsFromConfig();
        });
    }
} catch { /* ignore watcher issues */ }


// ...existing code...

function logAndSave(message) {
    try {
        // Simple log rotation
        const maxBytes = parseInt(process.env.LOG_MAX_BYTES || '2000000', 10); // 2MB default
        if (fs.existsSync(LOG_FILE)) {
            const stats = fs.statSync(LOG_FILE);
            if (stats.size > maxBytes) {
                const rotated = `${LOG_FILE.replace(/\.log$/, '')}-${Date.now()}.log`;
                fs.renameSync(LOG_FILE, rotated);
            }
        }
        const logLine = `[${new Date().toISOString()}] ${message}\n`;
        fs.appendFileSync(LOG_FILE, logLine);
    } catch (e) {
        console.error('Log write error:', e.message);
    }
    console.log(message);
}

// ========================================
// Quote Request Queue Processing
// ========================================
const QUOTE_QUEUE_INTERVAL_MS = parseInt(process.env.QUOTE_QUEUE_INTERVAL_MS || '60000', 10);
let isProcessingQuoteQueue = false;

async function processQueuedQuoteRequests(trigger = 'manual') {
    if (isProcessingQuoteQueue) return;
    if (!isWhatsAppClientReady()) return;

    isProcessingQuoteQueue = true;
    try {
        const queue = await getQueuedQuoteRequests();
        if (!Array.isArray(queue) || queue.length === 0) {
            return;
        }

        let changed = false;
        const remaining = [];
        for (const entry of queue) {
            const digits = (entry?.number || '').replace(/\D/g, '');
            if (!digits || !entry?.message) {
                changed = true;
                logAndSave(`[QuoteQueue] Dropping invalid entry ${entry?.id || 'unknown'}`);
                continue;
            }

            try {
                const chatId = `${digits}@c.us`;
                await global.whatsappClient.sendMessage(chatId, entry.message);
                changed = true;
                logAndSave(`[QuoteQueue] Delivered queued request ${entry.id} (source=${entry.source || 'unknown'}, trigger=${trigger})`);
            } catch (error) {
                const message = error && error.message ? error.message : String(error);
                changed = true;
                remaining.push({
                    ...entry,
                    attempts: (entry.attempts || 0) + 1,
                    lastAttemptAt: new Date().toISOString(),
                    lastError: message
                });
                logAndSave(`[QuoteQueue] Delivery failed for ${entry.id}: ${message}`);
            }
        }

        if (changed) {
            if (remaining.length) {
                await replaceQueuedQuoteRequests(remaining);
            } else {
                await replaceQueuedQuoteRequests([]);
            }
        }
    } catch (error) {
        logAndSave(`[QuoteQueue] Processing error (${trigger}): ${error.message}`);
    } finally {
        isProcessingQuoteQueue = false;
    }
}

ensureQueueFile()
    .then(() => logAndSave(`[QuoteQueue] Queue ready at ${getQueueFilePath()}`))
    .catch((error) => logAndSave(`[QuoteQueue] Failed to initialize queue file: ${error.message}`));

try {
    fs.watchFile(getQueueFilePath(), { interval: 5000 }, () => {
        processQueuedQuoteRequests('file_watch').catch(() => {});
    });
} catch (error) {
    logAndSave(`[QuoteQueue] File watch unavailable: ${error.message}`);
}

setInterval(() => {
    processQueuedQuoteRequests('interval').catch(() => {});
}, QUOTE_QUEUE_INTERVAL_MS);

// --- Minimal SSE server & broadcaster ---
const sseClients = new Set();
app.get('/events', (req, res) => {
    // Standard SSE headers
    res.setHeader('Content-Type', 'text/event-stream');
    res.setHeader('Cache-Control', 'no-cache');
    res.setHeader('Connection', 'keep-alive');
    // CORS for local desktop app
    res.setHeader('Access-Control-Allow-Origin', '*');

    // Send initial comment to open stream
    res.write(`: connected ${Date.now()}\n\n`);
    sseClients.add(res);
    logAndSave(`[SSE] Client connected (total: ${sseClients.size})`);

    // Keep-alive ping every 25s
    const ping = setInterval(() => {
        try { res.write(`: ping ${Date.now()}\n\n`); } catch {}
    }, 25000);

    req.on('close', () => {
        clearInterval(ping);
        sseClients.delete(res);
        try { res.end(); } catch {}
        logAndSave(`[SSE] Client disconnected (total: ${sseClients.size})`);
    });
});

function sseBroadcast(event, data) {
    try {
        const payload = typeof data === 'string' ? data : JSON.stringify(data);
        const line = `event: ${event}\ndata: ${payload}\n\n`;
        for (const res of sseClients) {
            try { res.write(line); } catch {}
        }
    } catch (e) {
        console.error('[SSE] Broadcast error:', e.message);
    }
}

// --- Status helper (write file + optional SSE) ---
function updateWhatsAppStatus(status, extra = {}) {
    try {
        const payload = { status, ...extra };
        try {
            fs.writeFileSync(STATUS_FILE, JSON.stringify(payload), 'utf8');
        } catch {}
        try {
            sseBroadcast('status', payload);
        } catch {}
    } catch (e) {
        console.error('[Status] Update error:', e.message);
    }
}

// --- Duplicate message guard (simple in-memory TTL cache) ---
const recentMessages = new Set();
function isDuplicateMessage(msg) {
    try {
        const key = (msg && msg.id && msg.id._serialized) 
            || `${msg.timestamp}-${msg.from}-${(msg.body||'').slice(0,50)}`;
        if (!key) return false;
        if (recentMessages.has(key)) return true;
        recentMessages.add(key);
        setTimeout(() => recentMessages.delete(key), 60 * 1000);
        return false;
    } catch { return false; }
}

// --- IPC purge no-op (actual implementation can replace this) ---
function purgeIpcQueues() { /* no-op fallback */ }

// --- WhatsApp Client Initialization ---
// Build Puppeteer options with a safe executablePath fallback
const defaultChromePath = 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe';
const desiredChromePath = process.env.CHROMIUM_PATH || defaultChromePath;

function getPuppeteerOptions() {
    const opts = {
        // ✅ FIX 5: Use 'new' headless mode for better session persistence
        // Old headless mode had issues with LocalStorage/IndexedDB persistence
        headless: 'new',  // Changed from true to 'new' (Puppeteer 21+)
        args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-gpu',
            // ✅ FIX 6: Add session persistence flags
            '--disable-web-security',
            '--disable-features=IsolateOrigins,site-per-process',
            '--allow-running-insecure-content',
            '--disable-blink-features=AutomationControlled',
            '--no-first-run',
            '--no-default-browser-check',
            '--disable-extensions',
            // ✅ FIX 7: Ensure localStorage/IndexedDB persistence
            '--enable-features=NetworkService,NetworkServiceInProcess'
        ],
        timeout: 120000,  // Increased to 120s for QR scan time
        handleSIGINT: false,
        handleSIGTERM: false,
        handleSIGHUP: false
        // ❌ REMOVED: userDataDir conflicts with LocalAuth
        // LocalAuth manages its own browser data in .wwebjs_auth/session-{clientId}
        // userDataDir: path.join(AUTH_PATH, 'browser-data'),  // CONFLICT!
    };
    
    // Don't use external Chrome - let Puppeteer use bundled Chromium
    // try {
    //     if (fs.existsSync(desiredChromePath)) {
    //         opts.executablePath = desiredChromePath;
    //     }
    // } catch (e) {
    //     logAndSave(`[Puppeteer] Chrome path check failed: ${e.message}`);
    // }
    
    return opts;
}

function getClientOptions() {
    return {
        authStrategy: new LocalAuth({ 
            dataPath: AUTH_PATH,
            clientId: 'lasantha-tire-bot'
        }),
        puppeteer: getPuppeteerOptions(),
        // ✅ FIX 1: Use local cache to persist WhatsApp Web version (prevents re-authentication)
        // Remote cache causes session invalidation when WhatsApp updates
        webVersionCache: { 
            type: 'local'
        },
        authTimeoutMs: 120000,  // Increased to 120s for QR scan time
        qrMaxRetries: 5,
        restartOnAuthFail: false,  // ✅ FIX 2: Don't restart on auth fail - preserve session
        takeoverOnConflict: true,
        takeoverTimeoutMs: 10000,
        // ✅ FIX 3: Add session backup options
        authTimeoutMs: 180000,  // 3 minutes for QR scan
        qrTimeoutMs: 120000,    // 2 minutes QR validity
        // ✅ FIX 4: Preserve session across restarts
        bypassCSP: true,        // Required for WhatsApp Web compatibility
        userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
    };
}

// Create initial client instance
let client = new Client(getClientOptions());
global.whatsappClient = client;

function setupClientEventHandlers(clientInstance) {
    clientInstance.on('qr', (qr) => {
        logAndSave('[QR] QR Code received');
        // ✅ FIX 9: Log session status when QR is requested
        const sessionPath = path.join(AUTH_PATH, 'session-lasantha-tire-bot');
        const sessionExists = fs.existsSync(sessionPath);
        logAndSave(`[QR] Session folder exists: ${sessionExists}`);
        if (sessionExists) {
            logAndSave('[QR] ⚠️ Session exists but QR requested - possible session corruption');
        }
        const snap = snapshotSession();
        logAndSave(`[QR] Snapshot: files=${snap.totalFiles} indexedDB=${snap.keyArtifacts.indexedDB} localStorage=${snap.keyArtifacts.localStorage}`);
        qrcode.generate(qr, { small: true });
        
        // Store QR code for dashboard API
        currentQRCode = qr;
        qrCodeTimestamp = Date.now();
        
        try {
            fs.writeFileSync(STATUS_FILE, JSON.stringify({ qr, ready: false }), 'utf8');
        } catch {}
        sseBroadcast('qr', { qr });
    });

    clientInstance.on('loading_screen', (percent, message) => {
        logAndSave(`[Loading] ${percent}% - ${message}`);
    });
    
    // ✅ FIX 10: Add authenticated event to confirm session loaded
    clientInstance.on('authenticated', () => {
        logAndSave('[Auth] ✅ Session authenticated successfully - credentials saved');
        sseBroadcast('authenticated', { status: 'Session loaded from disk' });
    });

    // ✅ CRITICAL FIX: Attach message handler to the client instance
    // This ensures messages are received after reconnections
    setupMessageHandler(clientInstance);
    logAndSave('[Setup] Message handler attached to client');
}

function setupClientLifecycleHandlers(clientInstance) {
    clientInstance.on('disconnected', async (reason) => {
        logAndSave(`[Disconnect] Client disconnected: ${reason}`);
        isInitialized = false;
        isInitializing = false;
        try { fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false, reason }), 'utf8'); } catch {}
        sseBroadcast('disconnected', { reason });
        
        // Only auto-reconnect for certain disconnect reasons
        const autoReconnectReasons = ['CONFLICT', 'UNPAIRED', 'TIMEOUT'];
        if (autoReconnectReasons.some(r => String(reason).includes(r))) {
            logAndSave('[Disconnect] Auto-reconnect triggered');
            handleReconnection();
        } else {
            logAndSave('[Disconnect] Manual reconnect required (use Connect button)');
        }
    });

    clientInstance.on('auth_failure', async (msg) => {
        logAndSave(`[Auth] ❌ Authentication failed: ${msg}`);
        isInitialized = false;
        isInitializing = false;
        try { fs.writeFileSync(STATUS_FILE, JSON.stringify({ ready: false, auth_failure: true }), 'utf8'); } catch {}
        
        // Try to restore from backup first (session එක corrupt වෙලා නම්)
        logAndSave('[Auth] Attempting to restore session from backup...');
        const restored = await restoreSessionFromBackup();
        if (restored) {
            // IMPORTANT: Do NOT delete restored session folder here
            logAndSave('[Auth] ✅ Session restored from backup, will attempt reconnection without deleting data');
            setTimeout(() => { handleReconnection(); }, 3000);
        } else {
            if (CLEAR_SESSION_ON_AUTH_FAILURE) {
                try {
                    if (fs.existsSync(AUTH_PATH)) {
                        fs.rmSync(AUTH_PATH, { recursive: true, force: true });
                        logAndSave('[Auth] Cleared authentication data (env flag enabled)');
                    }
                } catch (err) {
                    logAndSave(`[Auth] Failed to clear auth data: ${err.message}`);
                }
            } else {
                logAndSave('[Auth] Skipping auth data deletion (CLEAR_SESSION_ON_AUTH_FAILURE=false)');
            }
            sseBroadcast('auth_failure', { message: msg });
            logAndSave('[Auth] ⚠️ Please use Connect button to scan new QR code if session not usable');
        }
    });

    clientInstance.on('ready', () => {
        logAndSave('[Ready] ✅ WhatsApp client is ready!');
        isInitialized = true;
        isInitializing = false;
        reconnectAttempts = 0;
        
        if (reconnectTimer) {
            clearTimeout(reconnectTimer);
            reconnectTimer = null;
        }
        
        setClient(clientInstance);
        
        try {
            const phoneNumber = (clientInstance && clientInstance.info && clientInstance.info.wid && clientInstance.info.wid.user) ? clientInstance.info.wid.user : undefined;
            const status = { ready: true };
            if (phoneNumber) {
                status.phoneNumber = phoneNumber;
                logAndSave(`[Ready] Connected as: ${phoneNumber}`);
            }
            fs.writeFileSync(STATUS_FILE, JSON.stringify(status), 'utf8');
        } catch (e) {
            logAndSave(`[Ready] Status file write error: ${e.message}`);
        }
        
        sseBroadcast('ready', { ready: true });

        processQueuedQuoteRequests('whatsapp_ready').catch(() => {});

        // Initialize all components
        try {
            if (!initializeScheduler(clientInstance)) {
                throw new Error('Scheduler initialization failed');
            }

            try {
                initializeDailyAccountingReport(clientInstance);
                logAndSave('Daily Accounting Report System initialized');
            } catch (reportErr) {
                logAndSave(`Warning: Daily Accounting Report initialization failed: ${reportErr.message}`);
                console.error('Daily Accounting Report init error:', reportErr);
            }

            // Scheduled Media Publisher (conditional start)
            if (String(process.env.ENABLE_SCHEDULED_MEDIA_PUBLISHER || 'true').toLowerCase() === 'true') {
                try {
                    const mediaPublisher = new ScheduledMediaPublisherJob(clientInstance);
                    mediaPublisher.start();
                    logAndSave('📅 Scheduled Media Publisher started (10:05 PM daily)');
                } catch (publishErr) {
                    logAndSave(`Warning: Scheduled Media Publisher initialization failed: ${publishErr.message}`);
                    console.error('Scheduled Media Publisher init error:', publishErr);
                }
            } else {
                logAndSave('ℹ️  Scheduled Media Publisher disabled (ENABLE_SCHEDULED_MEDIA_PUBLISHER=false)');
            }

            if (!ipcWatcherStarted) {
                purgeIpcQueues();
                const { startWatcher } = require('./utils/ipcCommandWatcher');
                startWatcher((m) => logAndSave(m));
                ipcWatcherStarted = true;
                logAndSave('IPC Command watcher started');
            }
        } catch (e) {
            logAndSave(`Initialization error: ${e.message}`);
            console.error('Initialization failed:', e);
        }
        // Start backup system only after first successful ready (avoid backing up partial QR state)
        try {
            if (!global.__backupStarted) {
                startSessionBackup(logAndSave);
                global.__backupStarted = true;
                logAndSave('[Ready] Session backup system started');
            }
        } catch (backupErr) {
            logAndSave(`[Ready] Session backup failed to start: ${backupErr.message}`);
        }
    });
}

// Enhanced error handling and recovery
let isInitialized = false;
let isInitializing = false;
let reconnectAttempts = 0;
let ipcWatcherStarted = false;
let reconnectTimer = null;
const MAX_RECONNECT_ATTEMPTS = 3; // Reduced from 5 to avoid infinite loops
const BASE_RECONNECT_INTERVAL = 15000; // 15 seconds base
const MAX_RECONNECT_INTERVAL = 60000; // 1 minute max

// Cleanup function to destroy client properly
async function cleanupClient() {
    if (global.whatsappClient && global.whatsappClient !== null) {
        try {
            // Try to destroy the client gracefully
            if (typeof global.whatsappClient.destroy === 'function') {
                await global.whatsappClient.destroy();
                logAndSave('[Cleanup] Client destroyed');
            }
        } catch (e) {
            // Only log if it's not a null reference error
            if (!e.message.includes('null')) {
                logAndSave(`[Cleanup] Error destroying client: ${e.message}`);
            }
        }
    }
    global.whatsappClient = null;
    isInitialized = false;
    isInitializing = false;
}

async function initializeClient(force = false) {
    if (isInitializing) {
        logAndSave('[Init] Already initializing, skipping...');
        return;
    }
    if (!force && isInitialized) {
        logAndSave('[Init] Already initialized, skipping...');
        return;
    }

    // Clear any pending reconnect timer
    if (reconnectTimer) {
        clearTimeout(reconnectTimer);
        reconnectTimer = null;
    }

    isInitializing = true;
    try {
        logAndSave('[Init] Starting WhatsApp client initialization...');
        const preSnap = snapshotSession();
        logAndSave(`[Init] Session snapshot before init: exists=${preSnap.exists} folders=${preSnap.sessionFolders.length} files=${preSnap.totalFiles}`);
        
        // ✅ FIX 11: Validate existing session before starting
        const sessionPath = path.join(AUTH_PATH, 'session-lasantha-tire-bot');
        const sessionExists = fs.existsSync(sessionPath);
        
        if (sessionExists) {
            // Check if session has critical files
            const indexedDBPath = path.join(sessionPath, 'Default', 'IndexedDB');
            const localStoragePath = path.join(sessionPath, 'Default', 'Local Storage');
            const hasIndexedDB = fs.existsSync(indexedDBPath);
            const hasLocalStorage = fs.existsSync(localStoragePath);
            
            logAndSave(`[Init] 📂 Found existing session:`);
            logAndSave(`       - IndexedDB: ${hasIndexedDB ? '✅' : '❌'}`);
            logAndSave(`       - LocalStorage: ${hasLocalStorage ? '✅' : '❌'}`);
            
            if (hasIndexedDB && hasLocalStorage) {
                logAndSave('[Init] ✅ Valid session detected - will attempt to restore without QR');
            } else {
                logAndSave('[Init] ⚠️  Incomplete session - may require QR scan');
            }
        } else {
            logAndSave('[Init] 📱 No existing session - QR scan will be required');
        }
        
        // Cleanup any existing client first
        await cleanupClient();
        
        // Give it a moment to fully clean up
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Create a FRESH client instance (whatsapp-web.js doesn't support re-initializing same instance)
        logAndSave('[Init] Creating new client instance...');
        client = new Client(getClientOptions());
        global.whatsappClient = client;
        
        // Set up event handlers for the new client
        setupClientEventHandlers(client);
        setupClientLifecycleHandlers(client);
        
        // Now initialize the new client
        logAndSave('[Init] Calling client.initialize()...');
        await client.initialize();
        reconnectAttempts = 0;
        logAndSave('[Init] ✅ Initialization completed successfully');
    } catch (error) {
        logAndSave(`[Init] ❌ Initialization error: ${error.message}`);
        console.error('[Init] Full error:', error);
        
        // Check if error is due to session corruption
        const isSessionError = error.message && (
            error.message.includes('Execution context was destroyed') ||
            error.message.includes('Target closed') ||
            error.message.includes('Protocol error') ||
            error.message.includes('Session closed')
        );
        
        if (isSessionError && reconnectAttempts === 0) {
            logAndSave('[Init] 🔧 Session corruption detected - attempting restore from backup...');
            try {
                // Try to restore from backup
                const restored = await restoreSessionFromBackup();
                if (restored) {
                    // Do NOT delete restored session; it is now the good copy
                    logAndSave('[Init] ✅ Session restored from backup - retrying initialization WITHOUT deleting restored data...');
                    setTimeout(() => {
                        isInitializing = false;
                        initializeClient(true);
                    }, 3000);
                    return;
                } else {
                    logAndSave('[Init] ⚠️  No backup available - will require fresh QR scan');
                    if (CLEAR_SESSION_ON_INIT_ERROR) {
                        if (fs.existsSync(AUTH_PATH)) {
                            fs.rmSync(AUTH_PATH, { recursive: true, force: true });
                            logAndSave('[Init] Deleted corrupted session (env flag CLEAR_SESSION_ON_INIT_ERROR=true)');
                        }
                    } else {
                        logAndSave('[Init] Keeping corrupted session for manual inspection (CLEAR_SESSION_ON_INIT_ERROR=false)');
                    }
                }
            } catch (restoreErr) {
                logAndSave(`[Init] ❌ Restore failed: ${restoreErr.message}`);
            }
        }
        
        isInitializing = false;
        handleReconnection();
    }
}

function handleReconnection() {
    isInitialized = false;
    
    if (reconnectTimer) {
        logAndSave('[Reconnect] Attempt already scheduled, skipping duplicate trigger');
        return;
    }
    
    if (reconnectAttempts >= MAX_RECONNECT_ATTEMPTS) {
        logAndSave(`[Reconnect] ⚠️  Max reconnection attempts (${MAX_RECONNECT_ATTEMPTS}) reached.`);
        logAndSave('[Reconnect] Manual restart required. Use dashboard Connect button or restart bot.');
        
        // Reset attempts after 5 minutes to allow retry
        setTimeout(() => {
            logAndSave('[Reconnect] Reconnection attempts reset after cooldown');
            reconnectAttempts = 0;
        }, 300000); // 5 minutes
        return;
    }
    
    reconnectAttempts++;
    
    // Exponential backoff: 15s, 30s, 60s
    const delay = Math.min(
        BASE_RECONNECT_INTERVAL * Math.pow(2, reconnectAttempts - 1),
        MAX_RECONNECT_INTERVAL
    );
    
    logAndSave(`[Reconnect] Attempt ${reconnectAttempts}/${MAX_RECONNECT_ATTEMPTS} in ${delay/1000}s...`);
    
    reconnectTimer = setTimeout(() => {
        reconnectTimer = null;
        initializeClient(true);
    }, delay);
}

function triggerSessionRecovery(context, message) {
    const detail = message || 'unknown';
    logAndSave(`[${context}] ⚠️ Session issue detected: ${detail}`);
    isInitialized = false;

    if (isInitializing) {
        logAndSave(`[${context}] Initialization already in progress; waiting for completion`);
        return;
    }

    if (reconnectTimer) {
        logAndSave(`[${context}] Reconnect already scheduled; skipping duplicate trigger`);
        return;
    }

    handleReconnection();
}

// Set up event handlers for initial client instance
setupClientEventHandlers(client);
setupClientLifecycleHandlers(client);

// Start the client
initializeClient();


// Session health monitor - checks every 5 minutes if session is still valid
setInterval(async () => {
    if (!isInitialized || !global.whatsappClient) {
        return; // Skip if not initialized
    }
    
    try {
        // Try to get client state to check if session is alive
        const state = await global.whatsappClient.getState();
        
        if (state !== 'CONNECTED') {
            logAndSave(`[Health] ⚠️ Session state is ${state}, attempting reconnection...`);
            isInitialized = false;
            handleReconnection();
        } else {
            // Session is healthy, log occasionally
            if (Math.random() < 0.1) { // 10% chance to log (avoid log spam)
                logAndSave('[Health] ✅ Session is healthy (CONNECTED)');
            }
        }
    } catch (err) {
        logAndSave(`[Health] ⚠️ Failed to check session state: ${err.message}`);
        // If we can't check state, the session might be dead
        if (isInitialized) {
            isInitialized = false;
            handleReconnection();
        }
    }
}, 300000); // Check every 5 minutes (300000ms)

// --- AI Helper Functions ---

/**
 * Logs a message to the AI chat history database.
 * @param {string} phone - The user's phone number.
 * @param {string} type - 'user' or 'bot'.
 * @param {string} message - The message content.
 */
async function logToAiChatHistory(phone, type, message) {
    try {
        await aiPoolConnect;
        const request = new sql.Request(aiPool);
        request.input('user_phone', sql.VarChar(25), phone);
        request.input('sender_type', sql.VarChar(10), type);
        request.input('message_text', sql.NVarChar(sql.MAX), message);
        await request.query('INSERT INTO whatsapp_chat_history (user_phone, sender_type, message_text) VALUES (@user_phone, @sender_type, @message_text)');
    } catch (err) {
        logAndSave(`[AI DB Error] Failed to log chat history: ${err.message}`);
    }
}

/**
 * Logs a failed AI interaction to the failure queue.
 * @param {string} phone - The user's phone number.
 * @param {string} message - The failed message.
 * @param {string} errorType - The type of error (e.g., 'API_TIMEOUT').
 */
async function logAiFailure(phone, message, errorDetails) {
    try {
        await aiPoolConnect;
        const request = new sql.Request(aiPool);
        request.input('user_phone', sql.VarChar(25), phone);
        request.input('failed_message', sql.NVarChar(sql.MAX), message);
        request.input('error_type', sql.VarChar(50), errorDetails.substring(0, 50));
        await request.query('INSERT INTO ai_failure_queue (user_phone, failed_message, error_type) VALUES (@user_phone, @failed_message, @error_type)');
        logAndSave(`[AI Failure] Logged to queue: ${errorDetails.substring(0, 50)}...`);
    } catch (err) {
        logAndSave(`[AI DB Error] Failed to log AI failure: ${err.message}`);
    }
}

/**
 * Checks if a message is a simple, regex-based request.
 * @param {string} text - The message body.
 * @returns {boolean}
 */
function isSimpleRequest(text) {
    // A simple request is one that contains a tyre size and nothing else, or very few other words.
    const tyreSize = extractTyreSizeFlexible(text);
    if (!tyreSize) return false;

    const otherWords = text.replace(tyreSize, '').replace(/price|mil|kiyada|ගාන/ig, '').trim();
    return otherWords.length < 10; // Allow for a few extra words like "price?"
}

// ========================================
// MESSAGE HANDLER SETUP (FOR CLIENT RECONNECTIONS)
// ========================================
function setupMessageHandler(clientInstance) {
    // Remove any existing message listeners to prevent duplicates
    clientInstance.removeAllListeners('message');
    
    clientInstance.on('message', async msg => {
    // Log ALL incoming messages for debugging
    console.log(`[MSG] From: ${msg.from}, Body: "${msg.body}", isSystem: ${isSystemSender(msg.from)}, isDupe: ${isDuplicateMessage(msg)}`);
    
    // Ignore system/broadcast messages and duplicates
    if (isSystemSender(msg.from) || isDuplicateMessage(msg)) {
        return;
    }

    // ========================================
    // GROUP ID DISCOVERY COMMAND
    // ========================================
    // Allow discovering group IDs for configuration purposes
    const msgText = msg.body ? msg.body.trim().toLowerCase() : '';
    console.log(`[Debug] Received message: "${msgText}" from ${msg.from} isGroup=${msg.from.endsWith('@g.us')}`);
    
    if (msgText === '!groupid' && msg.from.endsWith('@g.us')) {
        console.log(`[GroupID] Processing !groupid command from: ${msg.from}`);
        try {
            await msg.reply(`📋 *Group ID:*\n\`\`\`${msg.from}\`\`\`\n\n_Use this ID to configure appointment notifications in .env file_`);
            console.log(`[GroupID] Sent group ID to: ${msg.from}`);
        } catch (err) {
            console.error(`[GroupID] Error sending reply:`, err);
        }
        return;
    }

    // ========================================
    // RESTRICTED GROUPS (NO REPLIES)
    // ========================================
    const SHOP_MANAGEMENT_GROUP_ID = process.env.SHOP_MANAGEMENT_GROUP_ID;
    if (SHOP_MANAGEMENT_GROUP_ID && msg.from === SHOP_MANAGEMENT_GROUP_ID) {
        logAndSave(`[Restricted] Ignoring message from Shop Management Group: ${msg.from}`);
        return;
    }

    const senderNumber = msg.from.replace('@c.us', '');
    const text = msg.body.trim();

    // Prepare typing indicator
    const chat = await msg.getChat();
    try { await chat.sendStateTyping(); } catch {}

    try {
        // Log user message to AI chat history
        await logToAiChatHistory(senderNumber, 'user', text);

        // ========================================
        // STEP 0: ADMIN COMMANDS (HIGHEST PRIORITY)
        // ========================================
        // Admin Co-pilot slash commands must run BEFORE job system
        // to prevent job matches on command keywords like "testpost"
        // Accept either slash-prefixed commands or plain keywords from admin (e.g., "testpost")
        if (isAdminNumber(msg.from)) {
            logAndSave(`[Admin Check] ✅ Admin detected: ${msg.from}`);
            const normalized = text.trim().toLowerCase();
            const maybeCommand = normalized.startsWith('/') ? normalized.substring(1) : normalized.split(/\s+/)[0];
            const [command, ...args] = maybeCommand.split(' ');
            
            // --- Admin inbound poster flow (photos/captions) ---
            try {
                                const POST_POOL = ConfigService.get('EXTERNAL_IMAGE_DIR') && String(ConfigService.get('EXTERNAL_IMAGE_DIR')).trim()
                  ? path.resolve(process.env.EXTERNAL_IMAGE_DIR)
                  : path.join(__dirname, 'post');
                const INBOX_DIR = path.join(__dirname, 'post-inbox');
                const PUBLISHED_DIR = path.join(__dirname, 'post-published');
                try { if (!fs.existsSync(POST_POOL)) fs.mkdirSync(POST_POOL, { recursive: true }); } catch {}
                try { if (!fs.existsSync(INBOX_DIR)) fs.mkdirSync(INBOX_DIR, { recursive: true }); } catch {}
                try { if (!fs.existsSync(PUBLISHED_DIR)) fs.mkdirSync(PUBLISHED_DIR, { recursive: true }); } catch {}

                const nowStamp = () => new Date().toISOString().replace(/[-:T.Z]/g, '').slice(0, 14);
                const senderKey = senderNumber.replace(/\D/g, '');

                const saveMediaTo = async (mediaObj, baseDir, prefix) => {
                    const ext = (() => {
                        const t = (mediaObj.mimetype || '').toLowerCase();
                        if (t.includes('png')) return '.png';
                        if (t.includes('webp')) return '.webp';
                        if (t.includes('jpeg')) return '.jpg';
                        if (t.includes('jpg')) return '.jpg';
                        return '.jpg';
                    })();
                    const name = `${prefix}_${nowStamp()}_${Math.random().toString(36).slice(2,8)}${ext}`;
                    const full = path.join(baseDir, name);
                    try {
                        const buf = Buffer.from(mediaObj.data, 'base64');
                        fs.writeFileSync(full, buf);
                        return full;
                    } catch (e) {
                        logAndSave(`[Admin Poster] ❌ Failed to save media: ${e.message}`);
                        return null;
                    }
                };

                const findOldestPending = () => {
                    try {
                        const files = fs.readdirSync(INBOX_DIR).filter(f => f.startsWith(senderKey + '_'));
                        if (!files.length) return null;
                        const withTime = files.map(f => ({ f, t: fs.statSync(path.join(INBOX_DIR, f)).mtimeMs }))
                                              .sort((a,b) => a.t - b.t);
                        return path.join(INBOX_DIR, withTime[0].f);
                    } catch { return null; }
                };

                const publishNow = async (caption, imagePath) => {
                    try {
                        const localOnly = ConfigService.getBoolean('LOCAL_PREVIEW_ONLY', false);
                        if (localOnly) {
                            const draftsDir = path.join(__dirname, 'facebook-drafts');
                            try { if (!fs.existsSync(draftsDir)) fs.mkdirSync(draftsDir, { recursive: true }); } catch {}
                            const dest = path.join(draftsDir, `wa_instant_${nowStamp()}` + path.extname(imagePath || '.jpg'));
                            try { fs.copyFileSync(imagePath, dest); } catch {}
                            await safeReply(msg, client, msg.from, `📝 Preview-only. Saved:
${dest}\n\n${caption}`);
                            return { id: 'local-preview', path: dest };
                        }
                        // Use serialized publish queue to avoid conflicts with other jobs
                        const { PostPublisherQueue } = require('./utils/PostPublisherQueue');
                        const q = PostPublisherQueue.getInstance();
                        const res = await q.enqueue({ caption, imagePath, publishMode: ConfigService.get('FB_PUBLISH_MODE', 'draft') });
                        return res;
                    } catch (e) {
                        logAndSave(`[Admin Poster] ❌ Publish failed: ${e.message}`);
                        await safeReply(msg, client, msg.from, `❌ Publish failed: ${e.message}`);
                        return null;
                    }
                };

                if (msg.hasMedia) {
                    const media = await msg.downloadMedia();
                    if (!media) {
                        await safeReply(msg, client, msg.from, `❌ Could not download media.`);
                            // Instrument Puppeteer page events for deeper diagnostics
                            try {
                                client.pupPage?.on('console', msg => {
                                    const txt = msg.text();
                                    if (/errors?|failed|timeout/i.test(txt)) {
                                        logAndSave(`[BrowserConsole] ${txt}`);
                                    }
                                });
                                client.pupPage?.on('requestfailed', req => {
                                    logAndSave(`[RequestFailed] ${req.url()} - ${req.failure()?.errorText}`);
                                });
                                client.pupPage?.on('pageerror', err => {
                                    logAndSave(`[PageError] ${err.message}`);
                                });
                            } catch (instrumentErr) {
                                logAndSave(`[Init] Instrumentation error: ${instrumentErr.message}`);
                            }

                        return;
                    }
                    const bodyLower = (text || '').trim().toLowerCase();
                    if (text && bodyLower !== 'poster') {
                        // Media + caption => immediate publish
                        const tmpPath = await saveMediaTo(media, path.join(__dirname, 'facebook-drafts'), `wa_pub_${senderKey}`);
                        if (!tmpPath) return;
                            // Enhanced diagnostics on failure
                            try {
                                if (client && client.pupPage) {
                                    const currentUrl = client.pupPage.url();
                                    logAndSave(`[Diag] Current page URL: ${currentUrl}`);
                                    // Attempt to capture HTML snippet
                                    const html = await client.pupPage.content();
                                    const snippet = html.slice(0, 500).replace(/\s+/g, ' ');
                                    logAndSave(`[Diag] HTML snippet: ${snippet}`);
                                    // Screenshot for visual debugging
                                    const screenshotPath = path.join(__dirname, 'last-init-failure.png');
                                    try {
                                        await client.pupPage.screenshot({ path: screenshotPath });
                                        logAndSave(`[Diag] Screenshot saved: ${screenshotPath}`);
                                    } catch (shotErr) {
                                        logAndSave(`[Diag] Screenshot failed: ${shotErr.message}`);
                                    }
                                }
                            } catch (diagErr) {
                                logAndSave(`[Diag] Diagnostics error: ${diagErr.message}`);
                            }
                        const res = await publishNow(text, tmpPath);
                        if (res && res.id) {
                            // In preview mode, don't consume/move the original; keep it in place
                            const localOnlyMoveGuard = String(process.env.LOCAL_PREVIEW_ONLY || '').toLowerCase() === 'true';
                            if (!localOnlyMoveGuard) {
                                try { fs.renameSync(tmpPath, path.join(PUBLISHED_DIR, path.basename(tmpPath))); } catch {}
                            }
                            await safeReply(msg, client, msg.from, `✅ Published: ${res.id}`);
                        }
                        return;
                    }
                    if (bodyLower === 'poster') {
                        const saved = await saveMediaTo(media, POST_POOL, `wa_poster_${senderKey}`);
                        if (saved) await safeReply(msg, client, msg.from, `✅ Added to poster pool:\n${saved}`);
                        return;
                    }
                    // No caption -> queue to inbox
                    const queued = await saveMediaTo(media, INBOX_DIR, `${senderKey}`);
                    if (queued) await safeReply(msg, client, msg.from, `📥 Saved to inbox. Reply with a caption to publish.\n${queued}`);
                    return;
                }

                // Caption only -> publish photos from inbox within a time window as separate posts
                if (!msg.hasMedia && text && text.length > 0) {
                    const WINDOW_SEC = Math.max(5, ConfigService.getNumber('WA_POSTER_BATCH_WINDOW_SECONDS', 60));
                    const cutoff = Date.now() - WINDOW_SEC * 1000;
                    let publishedAny = false;
                    try {
                        const files = fs.readdirSync(INBOX_DIR)
                          .filter(f => f.startsWith(senderKey + '_'))
                          .map(f => ({ f, full: path.join(INBOX_DIR, f), t: fs.statSync(path.join(INBOX_DIR, f)).mtimeMs }))
                          .sort((a,b) => a.t - b.t);
                        const inWindow = files.filter(x => x.t >= cutoff);
                        const targets = inWindow.length ? inWindow : (files.length ? [files[0]] : []);
                        for (const x of targets) {
                            const res = await publishNow(text, x.full);
                            if (res && res.id) {
                                // In preview mode, don't consume inbox items
                                const localOnlyMoveGuard = String(process.env.LOCAL_PREVIEW_ONLY || '').toLowerCase() === 'true';
                                if (!localOnlyMoveGuard) {
                                    try { fs.renameSync(x.full, path.join(PUBLISHED_DIR, path.basename(x.full))); } catch {}
                                }
                                publishedAny = true;
                            }
                        }
                    } catch (e) { logAndSave(`[Admin Poster] ⚠️ Batch publish error: ${e.message}`); }
                    if (publishedAny) {
                        await safeReply(msg, client, msg.from, `✅ Published ${'posts'} for your caption.`);
                        return;
                    }
                }
            } catch (adminPosterErr) {
                logAndSave(`[Admin Poster] ⚠️ Flow error: ${adminPosterErr.message}`);
                // continue to other admin commands
            }
            
            if (command === 'testpost' || command === 'fbpost') {
                try {
                    await safeReply(msg, client, msg.from, `🔄 Testing Facebook Post Generation...\n\nUsing: ${process.env.FB_POST_AI_PROVIDER || 'claude'}\n\nPlease wait...`);
                    
                    const { getFacebookPostJob } = require('./scheduler');
                    const fbJob = getFacebookPostJob();
                    
                    if (!fbJob) {
                        return await safeReply(msg, client, msg.from, `❌ Facebook Post Job not initialized. Please restart bot.`);
                    }
                    
                    logAndSave(`[Admin Command] ${senderNumber} triggered manual FB post test`);
                    
                    // Trigger the job - it will send preview to admin WhatsApp
                    await fbJob.generateAndSendForApproval();
                    
                    // Note: The reply will come from the job itself to admin WhatsApp
                    logAndSave(`[Admin Command] FB post test completed`);
                    
                } catch (e) {
                    logAndSave(`[Admin Command Error] /testpost failed: ${e.message}`);
                    await safeReply(msg, client, msg.from, `❌ Error: ${e.message}`);
                }
                return;
            }
            
            if (command === 'price') {
                try {
                    const size = extractTyreSizeFlexible(args.join(' '));
                    const brand = args.filter(arg => !extractTyreSizeFlexible(arg)).join(' ') || null;
                    if (!size) {
                        return await safeReply(msg, client, msg.from, "Usage: /price [size] [brand]");
                    }
                    
                    // Use the production fetchTyreData utility
                    const tyreData = await fetchTyreData(
                        mainPool,
                        size,
                        brand,
                        senderNumber,
                        allowedContacts,
                        text
                    );
                    
                    // For admin, send the formatted price list directly
                    if (tyreData.formatted) {
                        await safeReply(msg, client, msg.from, tyreData.formatted);
                    } else {
                        // Fallback: use AI to format
                        const adminReply = await gemini.generateAdminResponse(tyreData);
                        await safeReply(msg, client, msg.from, adminReply);
                    }
                } catch (e) {
                    logAndSave(`[Admin Command Error] /price failed: ${e.message}`);
                    await safeReply(msg, client, msg.from, `Error: ${e.message}`);
                }
                return;
            }
        }

        // ========================================
        // STEP 1: TRY ADVANCED JOB SYSTEM
        // ========================================
        const jobResult = await registry.tryHandle(msg, {
            sql,
            sqlConfig: {
                ...sqlConfig,
                database: 'LasanthaTire'
            },
            allowedContacts,
            logAndSave,
            context: {
                senderNumber,
                text,
                isAdmin: isAdminNumber(msg.from),
                logAndSave  // Pass logAndSave to context for JobRegistry debugging
            }
        });

        if (jobResult.handled) {
            logAndSave(`[JobSystem] ✅ Handled by ${jobResult.jobName} (score: ${jobResult.score})`);
            
            // Log to AI chat history
            await logToAiChatHistory(senderNumber, 'bot', `[Job: ${jobResult.jobName}] Handled successfully`);
            
            return; // Job handled successfully, exit
        }

        logAndSave(`[JobSystem] ⏭️  No job matched (reason: ${jobResult.reason}). Continuing to legacy flow...`);

        // ========================================
        // STEP 2: CHECK FOR WORKFLOWS
        // ========================================
        const workflowName = orchestrator.matchWorkflow(text, { senderNumber });
        if (workflowName) {
            logAndSave(`[Workflow] 🔄 Matched workflow: ${workflowName}`);
            
            const workflowResult = await orchestrator.executeWorkflow(workflowName, msg, {
                sql,
                sqlConfig: {
                    ...sqlConfig,
                    database: 'LasanthaTire'
                },
                allowedContacts,
                logAndSave,
                context: {
                    senderNumber,
                    text,
                    isAdmin: isAdminNumber(msg.from)
                }
            });

            if (workflowResult.success) {
                logAndSave(`[Workflow] ✅ Workflow ${workflowName} completed in ${workflowResult.duration}ms`);
                await logToAiChatHistory(senderNumber, 'bot', `[Workflow: ${workflowName}] Completed`);
                return; // Workflow handled successfully, exit
            }

            logAndSave(`[Workflow] ⚠️  Workflow ${workflowName} failed: ${workflowResult.error}`);
        }

        // ========================================
        // STEP 3: LEGACY FLOW (Photo, AI, etc.)
        // ========================================
        logAndSave(`[Legacy] 📜 Entering legacy flow for message: ${text.substring(0, 50)}...`);

        // --- PHOTO HANDLING: Check if message has photo with size + brand ---
        if (msg.hasMedia) {
            try {
                logAndSave(`[Photo] Received media from ${senderNumber}. Caption: ${text}`);
                
                // Extract size and brand from caption text
                let tyreSize = extractTyreSizeFlexible(text);
                let brandMatch = text.match(/(MAXXIS|DURATURN|BRIDGESTONE|MICHELIN|YOKOHAMA|DUNLOP|GOODYEAR|CONTINENTAL)/i);
                let brand = brandMatch ? brandMatch[1].toUpperCase() : null;
                
                // If caption doesn't have size/brand, try to get from conversation context
                if (!tyreSize || !brand) {
                    logAndSave(`[Photo] Caption incomplete. Checking conversation context...`);
                    
                    // Get recent chat history
                    const historyResult = await new sql.Request(aiPool)
                        .input('phone', sql.VarChar(25), senderNumber)
                        .input('limit', sql.Int, 10)
                        .query('SELECT TOP (@limit) * FROM whatsapp_chat_history WHERE user_phone = @phone ORDER BY created_at DESC');
                    
                    // Extract context
                    const conversationContext = extractConversationContext(historyResult.recordset, text);
                    
                    // Use context if available
                    if (!tyreSize && conversationContext.tyreSize) {
                        tyreSize = conversationContext.tyreSize;
                        logAndSave(`[Photo] Using size from context: ${tyreSize}`);
                    }
                    if (!brand && conversationContext.brand) {
                        brand = conversationContext.brand;
                        logAndSave(`[Photo] Using brand from context: ${brand}`);
                    }
                }
                
                if (tyreSize && brand) {
                    logAndSave(`[Photo] Final detection - Size: ${tyreSize}, Brand: ${brand}`);
                    
                    // Query database for this size + brand combination using VIEW
                    await mainPoolConnect;
                    const request = new sql.Request(mainPool);
                    request.input('size', sql.VarChar(50), tyreSize);
                    request.input('brand', sql.VarChar(50), brand);
                    
                    const result = await request.query(`
                        SELECT TOP 1 
                            im.PatternName,
                            im.TyreWidth as Width, 
                            im.TyreAspectRatio as AspectRatio, 
                            im.TyreDiameter as Diameter,
                            im.LoadIndex, 
                            im.SpeedRating,
                            ISNULL(im.TotalQty, 0) as TotalStock,
                            im.UnitCost as MinSalesPrice,
                            im.ItemCode
                        FROM [View_Item Master Whatsapp] im
                        WHERE CONCAT(im.TyreWidth, '/', im.TyreAspectRatio, 'R', im.TyreDiameter) = @size
                        AND im.BrandName = @brand
                        AND im.Categoty LIKE '%Tyre%'
                        ORDER BY im.TotalQty DESC
                    `);
                    
                    if (result.recordset.length > 0) {
                        const item = result.recordset[0];
                        const hasStock = item.TotalStock > 0;
                        
                        // Check if sender is admin or allowed contact
                        const isAdminSender = ADMIN_NUMBERS.includes(senderNumber);
                        const allowedNumbers = ['0777311770']; // Add more allowed numbers here
                        const normalizedSender = senderNumber.replace(/^\+?94/, '0');
                        const isAllowedContact = allowedNumbers.includes(normalizedSender) || isAdminSender;
                        
                        // Try to fetch tyre image from database
                        const imageData = await fetchTyreImage(brand, item.PatternName);
                        
                        // Prepare response message
                        let responseMsg = `📸 *${brand} ${item.PatternName}*\n\n`;
                        responseMsg += `🔧 *විස්තර:*\n`;
                        responseMsg += `Size: ${tyreSize}\n`;
                        if (item.LoadIndex) responseMsg += `Load Index: ${item.LoadIndex}\n`;
                        if (item.SpeedRating) responseMsg += `Speed Rating: ${item.SpeedRating}\n`;
                        responseMsg += `\n💰 *මිල:* Rs. ${parseFloat(item.MinSalesPrice).toLocaleString()}\n\n`;
                        
                        if (hasStock) {
                            responseMsg += `✅ *දැන් තොගයේ ඇත!*\n`;
                            // Only show stock quantity to admin/allowed contacts
                            if (isAllowedContact) {
                                responseMsg += `Stock: ${item.TotalStock} units\n`;
                                logAndSave(`[Photo] Stock quantity shown to allowed contact: ${normalizedSender}`);
                            }
                        } else {
                            responseMsg += `⚠️ *දැන් තොගයේ නැත*\n`;
                            responseMsg += `කරුණාකර අමතන්න: 0777311770\n`;
                        }
                        
                        // Send response with image if available
                        if (imageData && imageData.image_data) {
                            const base64Data = Buffer.from(imageData.image_data).toString('base64');
                            const media = new MessageMedia(imageData.image_type, base64Data);
                            await msg.reply(media, undefined, { caption: responseMsg });
                            logAndSave(`[Photo] ✅ Sent ${brand} ${item.PatternName} image + specs (${hasStock ? 'IN STOCK' : 'OUT OF STOCK'})`);
                        } else {
                            // No image available - send text only
                            await msg.reply(responseMsg);
                            logAndSave(`[Photo] ✅ Sent ${brand} ${item.PatternName} specs (no image available)`);
                        }
                        
                        // Log to chat history
                        await logToAiChatHistory(senderNumber, 'bot', responseMsg);
                        return; // Done - exit message handler
                        
                    } else {
                        // No matching pattern found
                        logAndSave(`[Photo] ⚠️ No pattern found for ${brand} ${tyreSize}`);
                        await msg.reply(`ක්ෂමාවෙන්න, ${brand} ${tyreSize} size එක අපේ database එකේ නැහැ. කරුණාකර 0777311770 අමතන්න.`);
                        return;
                    }
                    
                } else {
                    // Photo sent but no size/brand detected
                    logAndSave(`[Photo] Caption missing size/brand. Text: ${text}`);
                    await msg.reply(`කරුණාකර photo එකේ caption එකේ tyre size එක සහ brand එක දන්වන්න.\n\nඋදාහරණය: 195/65R15 MAXXIS`);
                    return;
                }
                
            } catch (photoError) {
                logAndSave(`[Photo Error] ${photoError.message}`);
                // Continue to normal AI handling if photo processing fails
            }
        }

        // --- Intelligent Triage ---
        if (AI_ENABLED && !isSimpleRequest(text)) {
        // --- Complex Request: AI Workflow ---
        try {
            logAndSave(`[AI] Complex request from ${senderNumber}. Starting AI workflow.`);

            // Check for known query (skip for now - feature coming soon)
            let analysis = null;
            const knownQueryResult = { recordset: [] };

            if (knownQueryResult.recordset.length > 0) {
                // --- Self-Learning: Found in known_queries ---
                const knownQuery = knownQueryResult.recordset[0];
                logAndSave(`[AI] Found in known_queries. Skipping AI call.`);
                analysis = {
                    intent: knownQuery.correct_intent,
                    entities: JSON.parse(knownQuery.correct_entities_json)
                };
                // (Continue to data fetching with this human-verified analysis)
            } else {
                 // --- Step 1: Analyze (Gemini) with Context Enhancement ---
                const historyResult = await new sql.Request(aiPool)
                    .input('phone', sql.VarChar(25), senderNumber)
                    .input('limit', sql.Int, AI_CONTEXT_MESSAGES)
                    .query('SELECT TOP (@limit) * FROM whatsapp_chat_history WHERE user_phone = @phone ORDER BY created_at DESC');
                
                // Extract conversation context from history
                const conversationContext = extractConversationContext(historyResult.recordset, text);
                logAndSave(`[AI Context] Size: ${conversationContext.tyreSize || 'none'}, Brand: ${conversationContext.brand || 'none'}, LastIntent: ${conversationContext.lastIntent || 'none'}`);
                
                analysis = await gemini.analyzeMessage(text, historyResult.recordset.reverse());

                // Enhance analysis with conversation context
                const enhancedEntities = enhanceWithContext(text, conversationContext);
                if (enhancedEntities.contextAware) {
                    logAndSave(`[AI Enhanced] Using context: ${enhancedEntities.tyre_size} ${enhancedEntities.brand || ''} (source: ${enhancedEntities.source})`);
                    // Override AI analysis with context-aware entities
                    analysis.entities.tyre_size = enhancedEntities.tyre_size || analysis.entities.tyre_size;
                    analysis.entities.brand = enhancedEntities.brand || analysis.entities.brand;
                    analysis.contextEnhanced = true;
                }

                // Derive conversation meta
                const hadAnyBotReply = Array.isArray(historyResult.recordset) && historyResult.recordset.some(r => (r.sender_type||'').toLowerCase() === 'bot');
                var isFirstReply = !hadAnyBotReply;
            }

            // --- Step 2: Act & Generate (Gemini) ---
            // If AI detected a quotation request, directly invoke the PDF job (keeps legacy logic but triggered via AI as well)
            if (analysis.intent === 'quotation_request' && analysis.entities.tyre_size) {
                try {
                    logAndSave(`[AI] Quotation request detected for ${analysis.entities.tyre_size}. Generating PDF...`);
                    const handled = await TyreQuotationPDFLibJob(msg, sql, sqlConfig, logAndSave);
                    if (handled) {
                        await logToAiChatHistory(senderNumber, 'bot', '[AI] Quotation PDF sent');
                        return; // Done. PDF job replied with media.
                    }
                } catch (e) {
                    logAndSave(`[AI→PDF Error] ${e.message}`);
                    // fallthrough to normal handling below
                }
            }

            let dataForGeneration = {};
            let tyreData = null; // Declare at outer scope for image sending later
            if (analysis.intent === 'price_inquiry' && analysis.entities.tyre_size) {
                // Fetch tyre data using PRODUCTION LOGIC (same as TyrePriceReplyJob)
                try {
                    const tyreSize = analysis.entities.tyre_size;
                    const brand = analysis.entities.brand || null;
                    const requestedQty = extractRequestedQty(text, tyreSize);
                    const detailRequests = extractDetailRequests(text);
                    const isAdminSender = ADMIN_NUMBERS.includes(senderNumber);
                    const specialNumbersAllowDetails = ['0777311770'];
                    const isSpecial = specialNumbersAllowDetails.includes(senderNumber.replace(/^\+?94/, '0'));
                    // For admin & special numbers: allow stock/DB details; others: suppress by default
                    const allowStockDetails = isAdminSender || isSpecial;
                    // Always require explicit ask to show details/stock counts (your policy)
                    const requireExplicitDetails = true;
                    const suppressQtyByDefault = !allowStockDetails; // regular customers suppressed by default
                    
                    // Use the production fetchTyreData utility with full business logic
                    tyreData = await fetchTyreData(
                        mainPool,
                        tyreSize,
                        brand,
                        senderNumber,
                        allowedContacts,
                        text
                    );

                    // Check if it's a returning customer
                    const customerHistory = await new sql.Request(aiPool)
                        .input('phone', sql.VarChar(25), senderNumber)
                        .query('SELECT TOP 1 1 FROM ai_conversations WHERE user_phone = @phone AND conversation_count > 1');
                    const isReturningCustomer = customerHistory.recordset.length > 0;

                    // Check for bulk inquiry (4 tyres)
                    const isBulkInquiry = (text.includes('4') && text.toLowerCase().includes('tyre')) || requestedQty === 4;
                    
                    // Compute total stock across matched items
                    const totalStock = Array.isArray(tyreData.tyres) ? tyreData.tyres.reduce((sum, t) => sum + (Number(t.Qty)||0), 0) : 0;

                    // Fetch tyre specifications from WhatsAppAI database
                    let tyreSpecs = null;
                    if (tyreData.tyres && tyreData.tyres.length > 0) {
                        const firstTyre = tyreData.tyres[0];
                        const tyreBrand = firstTyre.Brand || brand;
                        const tyrePattern = firstTyre.Pattern || null;
                        
                        if (tyreBrand) {
                            try {
                                tyreSpecs = await fetchTyreSpecs(tyreBrand, tyrePattern);
                                if (tyreSpecs) {
                                    logAndSave(`[AI] Found specs for ${tyreBrand} ${tyrePattern || ''} (Confidence: ${tyreSpecs.dataQuality.confidence})`);
                                }
                            } catch (specsError) {
                                logAndSave(`[AI Specs Error] ${specsError.message}`);
                            }
                        }
                    }

                    dataForGeneration = {
                        ...tyreData,
                        tyreSize: tyreSize,
                        brand: brand,
                        tyreSpecs: tyreSpecs, // Add specs to data
                        meta: {
                            isFirstReply,
                            isAdminSender,
                            suppressQtyByDefault,
                            allowStockDetails,
                            requireExplicitDetails,
                            detailRequests,
                            requestedQty,
                            totalStock,
                            isReturningCustomer, // Added for marketing
                            isBulkInquiry,       // Added for marketing
                        }
                    };
                    
                    logAndSave(`[AI] Fetched ${tyreData.count} tyres for ${tyreSize}${brand ? ` (${brand})` : ''}. Returning: ${isReturningCustomer}, Bulk: ${isBulkInquiry}`);
                    
                } catch (dbError) {
                    logAndSave(`[AI DB Error] Failed to fetch tyre data: ${dbError.message}`);
                    dataForGeneration = { error: 'Database error', tyreSize: analysis.entities.tyre_size };
                }
            }
            // (Add more data fetching logic for other intents like stock_check, vehicle_lookup here)

            const aiReply = await gemini.generateResponse(text, analysis, dataForGeneration);
            
            await safeReply(msg, client, msg.from, aiReply);
            await logToAiChatHistory(senderNumber, 'bot', aiReply);

            // --- Send Tyre Image if available ---
            if (analysis.intent === 'price_inquiry' && tyreData && tyreData.tyres && tyreData.tyres.length > 0) {
                try {
                    const firstTyre = tyreData.tyres[0];
                    const tyreBrand = firstTyre.Brand || analysis.entities.brand;
                    const tyrePattern = firstTyre.Pattern;
                    
                    if (tyreBrand && tyrePattern) {
                        logAndSave(`[Image] Checking for ${tyreBrand} ${tyrePattern} image...`);
                        const imageData = await fetchTyreImage(tyreBrand, tyrePattern);
                        
                        if (imageData) {
                            let media;
                            
                            // Prefer binary data, fallback to file path
                            if (imageData.image_data && imageData.image_type) {
                                // Create MessageMedia from binary data
                                const base64Data = Buffer.from(imageData.image_data).toString('base64');
                                media = new MessageMedia(imageData.image_type, base64Data);
                                logAndSave(`[Image] Using binary data from database (${Math.round(imageData.image_size/1024)}KB)`);
                            } else if (imageData.local_path) {
                                // Fallback to file system
                                const imagePath = path.resolve(imageData.local_path);
                                if (fs.existsSync(imagePath)) {
                                    media = MessageMedia.fromFilePath(imagePath);
                                    logAndSave(`[Image] Using file from ${imagePath}`);
                                } else {
                                    logAndSave(`[Image] ⚠️  File not found: ${imagePath}`);
                                }
                            }
                            
                            if (media) {
                                const caption = `📸 ${imageData.brand} ${imageData.pattern}\n` +
                                               `${imageData.description || ''}`;
                                
                                await client.sendMessage(msg.from, media, { caption });
                                logAndSave(`[Image] ✅ Sent image for ${tyreBrand} ${tyrePattern}`);
                            }
                        } else {
                            logAndSave(`[Image] No image available for ${tyreBrand} ${tyrePattern}`);
                        }
                    }
                } catch (imageError) {
                    logAndSave(`[Image Error] ${imageError.message}`);
                    // Continue without image - don't break the flow
                }
            }

            // Log to AI conversations for ROI tracking
            await new sql.Request(aiPool)
                .input('phone', sql.VarChar(25), senderNumber)
                .query(`
                    IF EXISTS (SELECT 1 FROM ai_conversations WHERE user_phone = @phone)
                    UPDATE ai_conversations SET last_ai_interaction = GETDATE(), conversation_count = conversation_count + 1 WHERE user_phone = @phone
                    ELSE
                    INSERT INTO ai_conversations (user_phone, last_ai_interaction) VALUES (@phone, GETDATE())
                `);

            return; // AI handled the message

        } catch (error) {
            // --- Graceful Fallback ---
            logAndSave(`[AI Error] Workflow failed: ${error.message}`);
            await logAiFailure(senderNumber, text, error.message);
            const fallbackMessage = "Temporary system error. Please call 0771222509. Ref: AI_FALLBACK";
            await safeReply(msg, client, msg.from, fallbackMessage);
            await logToAiChatHistory(senderNumber, 'bot', fallbackMessage);
            return;
        }
        }

        // --- Simple Request or AI Disabled: Legacy Workflow ---
        logAndSave(`[Legacy] Simple request from ${senderNumber}. Using legacy jobs.`);
        
        // If message is ONLY a vehicle number, trigger combined invoice job
        const vehicleNumber = extractVehicleNumber(text);
        if (vehicleNumber && !extractTyreSizeFlexible(text) && text.replace(vehicleNumber, '').replace(/\W/g, '').length === 0) {
            const handled = await VehicleInvoiceReplyJob(msg, mainPool, allowedContacts, logAndSave);
            if (!handled) {
                await safeReply(msg, client, msg.from, `No invoice found for vehicle number: ${vehicleNumber}`);
            }
            return;
        }

        // Run legacy jobs
        const jobPromises = [
            // Pass correct params expected by legacy jobs: (msg, sql, sqlConfig, allowedContacts?, logAndSave)
            TyreQtyReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave),
            TyrePriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave),
            TyreQuotationPDFLibJob(msg, sql, sqlConfig, logAndSave),
        ];

        const jobResults = await Promise.all(jobPromises.map(p => p.catch(e => {
            logAndSave(`[Legacy Job Error] ${e.message}`);
            return false;
        })));

        if (!jobResults.some(Boolean)) {
            const fallbackReply = `Please type a valid command or tyre size in this format: 195/65/15`;
            await safeReply(msg, clientInstance, msg.from, fallbackReply);
            await logToAiChatHistory(senderNumber, 'bot', fallbackReply);
        }
    } finally {
        try { await chat.clearState(); } catch {}
    }
    });
}

// Call setupMessageHandler for initial client (this will be replaced on reconnection)
setupMessageHandler(client);

// ========================================
// GRACEFUL SHUTDOWN HANDLING
// ========================================

let isShuttingDown = false;

async function gracefulShutdown(signal) {
    if (isShuttingDown) {
        console.log(`\n⚠️  Shutdown already in progress...`);
        return;
    }
    
    isShuttingDown = true;
    console.log(`\n🛑 Received ${signal}. Starting graceful shutdown...`);
    
    const shutdownTimeout = setTimeout(() => {
        console.error('❌ Graceful shutdown timeout! Forcing exit...');
        process.exit(1);
    }, 30000); // 30 seconds timeout

    try {
        // 1. Create session backup before shutdown
        console.log('💾 Creating session backup before shutdown...');
        try {
            const { createSessionBackup } = require('./utils/sessionBackup');
            await createSessionBackup();
            console.log('✅ Session backup created');
        } catch (backupErr) {
            console.warn('⚠️  Session backup failed (non-critical):', backupErr.message);
        }
        
        // 2. Stop accepting new messages (DO NOT logout; preserves session for next run)
        if (global.whatsappClient) {
            console.log('📱 Closing WhatsApp client (preserving session)...');
            try {
                if (typeof global.whatsappClient.destroy === 'function') {
                    await global.whatsappClient.destroy();
                }
                console.log('✅ WhatsApp client closed');
            } catch (err) {
                console.log('⚠️  WhatsApp client close error (non-critical):', err.message);
            }
        }

        // 3. Close database connections
        console.log('💾 Closing database connections...');
        try {
            if (mainPool && mainPool.connected) {
                await mainPool.close();
                console.log('✅ Main database pool closed');
            }
            if (aiPool && aiPool.connected) {
                await aiPool.close();
                console.log('✅ AI database pool closed');
            }
        } catch (err) {
            console.error('⚠️  Database close error:', err.message);
        }

        // 4. Close Express server
        if (global.httpServer) {
            console.log('🌐 Closing HTTP server...');
            await new Promise((resolve) => {
                global.httpServer.close((err) => {
                    if (err) console.error('⚠️  HTTP server close error:', err.message);
                    else console.log('✅ HTTP server closed');
                    resolve();
                });
            });
        }

        // 5. Save final status
        console.log('💾 Saving final status...');
        updateWhatsAppStatus('shutdown', { timestamp: new Date().toISOString() });

        clearTimeout(shutdownTimeout);
        console.log('✅ Graceful shutdown complete');
        process.exit(0);

    } catch (error) {
        console.error('❌ Error during graceful shutdown:', error);
        clearTimeout(shutdownTimeout);
        process.exit(1);
    }
}

// Register shutdown handlers
process.on('SIGTERM', () => gracefulShutdown('SIGTERM'));
process.on('SIGINT', () => gracefulShutdown('SIGINT'));
process.on('SIGUSR2', () => gracefulShutdown('SIGUSR2')); // nodemon restart

// Handle uncaught errors
process.on('uncaughtException', (error) => {
    console.error('💥 Uncaught Exception:', error);
    if (!isShuttingDown) {
        gracefulShutdown('uncaughtException');
    }
});

process.on('unhandledRejection', (reason, promise) => {
    console.error('💥 Unhandled Rejection at:', promise, 'reason:', reason);
    // Don't shutdown on unhandled rejection, just log it
});

console.log('✅ Graceful shutdown handlers registered');

// ========================================
// START EXPRESS SERVER (with automatic port probe)
// ========================================

async function isPortFree(port) {
    return new Promise((resolve) => {
        const srv = net.createServer()
            .once('error', () => resolve(false))
            .once('listening', () => srv.close(() => resolve(true)))
            .listen(port, '0.0.0.0');
    });
}

async function findAvailablePort(startPort, maxAttempts = 10) {
    let port = startPort;
    for (let i = 0; i < maxAttempts; i++, port++) {
        // eslint-disable-next-line no-await-in-loop
        const free = await isPortFree(port);
        if (free) return port;
    }
    return null;
}

(async function startServer() {
    const preferred = BOT_API_PORT;
    const maxTries = parseInt(process.env.BOT_PORT_MAX_TRIES || '10', 10);
    console.log(`🌐 Starting Express server. Preferred port ${preferred} (max tries: ${maxTries})...`);

    const selected = await findAvailablePort(preferred, maxTries);
    if (!selected) {
        console.error(`❌ No free port found in range ${preferred}-${preferred + maxTries - 1}.`);
        process.exit(1);
        return;
    }

    if (selected !== preferred) {
        console.warn(`⚠️  Port ${preferred} in use. Switching to available port ${selected}.`);
        try {
            fs.writeFileSync(path.join(__dirname, 'runtime-port.json'), JSON.stringify({ port: selected, preferred, timestamp: new Date().toISOString() }), 'utf8');
        } catch {}
        try { sseBroadcast('server_status', { event: 'port_changed', port: selected, previous: preferred }); } catch {}
    }

    // Keep process env consistent for any downstream code using it
    process.env.BOT_API_PORT = String(selected);

    global.httpServer = app.listen(selected, () => {
        console.log(`✅ Express server listening on http://localhost:${selected}`);
        console.log(`📡 Health endpoint: http://localhost:${selected}/health`);
        console.log(`📊 Stats endpoint: http://localhost:${selected}/stats`);
        console.log(`🎣 Facebook webhook: http://localhost:${selected}/facebook/webhook`);
    });
    global.httpServer.on('error', (err) => {
        if (err && err.code === 'EADDRINUSE') {
            console.error(`❌ Port ${selected} became busy during start. Try increasing BOT_PORT_MAX_TRIES or freeing the port.`);
            console.error('   Tip (PowerShell):');
            console.error('   netstat -ano | findstr :' + selected);
            console.error('   taskkill /PID <pid> /F');
            process.exitCode = 1;
            return;
        }
        console.error('💥 HTTP server error:', err);
    });
})().catch((e) => {
    console.error('❌ Failed to start server:', e);
    process.exit(1);
});

// ========================================
// ADVANCED JOB SYSTEM INITIALIZATION
// ========================================
console.log('🔧 Initializing Advanced Job System...');
try {
    registerAllJobs();
    console.log('✅ Advanced Job System initialized');
    console.log(`📊 Registered ${registry.jobs.size} jobs`);
    console.log(`🔄 Registered ${orchestrator.workflows.size} workflows`);
} catch (err) {
    console.error('❌ Failed to initialize Advanced Job System:', err.message);
}

