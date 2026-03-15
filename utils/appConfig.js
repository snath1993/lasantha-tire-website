/**
 * Centralized Application Configuration
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * Single source of truth for all configuration values.
 * All modules should import from here instead of reading process.env directly.
 *
 * Usage:
 *   const config = require('./utils/appConfig');
 *   console.log(config.store.name);     // "Lasantha Tyre Traders"
 *   console.log(config.admin.numbers);  // ["0771222509"]
 */

require('dotenv').config();
const path = require('path');

// ── Helpers ──
function env(key, fallback = '') { return (process.env[key] || fallback).trim(); }
function envBool(key, fallback = false) {
    const v = (process.env[key] || '').trim().toLowerCase();
    if (!v) return fallback;
    return ['1', 'true', 'yes', 'on'].includes(v);
}
function envInt(key, fallback) {
    const v = parseInt(process.env[key] || '', 10);
    return isNaN(v) ? fallback : v;
}
function envList(key, fallback = '') {
    return (process.env[key] || fallback).split(',').map(s => s.trim()).filter(Boolean);
}

// ── App Root ──
const APP_ROOT = env('APP_ROOT') || path.resolve(__dirname, '..');

const config = {
    // ── Application ──
    appRoot: APP_ROOT,
    nodeEnv: env('NODE_ENV', 'development'),
    isProd: env('NODE_ENV', '').toLowerCase() === 'production',

    // ── Bot Core ──
    bot: {
        port: envInt('BOT_API_PORT', 8585),
        portMaxTries: envInt('BOT_PORT_MAX_TRIES', 10),
        engine: env('WHATSAPP_ENGINE', 'v1'),
        chromiumPath: env('CHROMIUM_PATH') || env('CHROME_PATH') || '',
        headless: env('WA_HEADLESS', 'true'),
        quoteDir: env('QUOTE_DIR', path.join(APP_ROOT, 'quotations')),
        quoteQueueIntervalMs: envInt('QUOTE_QUEUE_INTERVAL_MS', 60000),
        apiRateLimitMax: envInt('API_RATE_LIMIT_MAX', 300),
        logLevel: env('LOG_LEVEL', 'info'),
        logMaxBytes: envInt('LOG_MAX_BYTES', 2000000),
        schedulerEnabled: envBool('SCHEDULER_ENABLED', true),
    },

    // ── WhatsApp Session ──
    session: {
        clearOnAuthFailure: envBool('CLEAR_SESSION_ON_AUTH_FAILURE'),
        clearOnInitError: envBool('CLEAR_SESSION_ON_INIT_ERROR'),
        ignoreReadyTimeoutConnected: envBool('WA_IGNORE_READY_TIMEOUT_CONNECTED'),
        disableHealthMonitor: envBool('WA_DISABLE_HEALTH_MONITOR'),
        maxQueueAttempts: envInt('WA_MAX_QUEUE_ATTEMPTS', 3),
        queueFlushDelayMs: envInt('WA_QUEUE_FLUSH_DELAY_MS', 1500),
        mediaSendMaxTries: envInt('WA_MEDIA_SEND_MAX_TRIES', 4),
        defaultCountryCode: env('DEFAULT_COUNTRY_CODE', '94'),
    },

    // ── Admin & Contacts ──
    admin: {
        numbers: envList('ADMIN_NUMBERS', '0771222509'),
        primaryNumber: env('ADMIN_NUMBER', '0777311770'),
        whatsappNumber: env('ADMIN_WHATSAPP_NUMBER', ''),
        superAdmins: envList('SUPER_ADMIN_NUMBERS', '94777311770,94777078700,94771222509,94777440230'),
        jobAdmins: envList('JOB_ADMIN_NUMBERS', '94777311770,94771222509'),
        costPriceAllowed: envList('COST_PRICE_ALLOWED', '0777078700,0777311770,0771222509'),
        dailyReportNumbers: envList('DAILY_REPORT_NUMBERS', '0777311770,0777078700'),
        reportRecipients: envList('REPORT_RECIPIENTS'),
        allowedNumbers: envList('ALLOWED_NUMBERS'),
    },

    // ── Groups ──
    groups: {
        shopManagementId: env('SHOP_MANAGEMENT_GROUP_ID'),
        allowedIds: envList('ALLOWED_GROUP_IDS'),
        reOrderId: env('RE_ORDER_GROUP_ID'),
        allowlistOnly: envBool('WA_GROUP_ALLOWLIST_ONLY'),
    },

    // ── Store Branding ──
    store: {
        name: env('STORE_NAME', 'Lasantha Tyre Traders'),
        phone: env('STORE_PHONE', '0771222509'),
        whatsapp: env('STORE_WHATSAPP', '0721222509'),
        location: env('STORE_LOCATION', 'Thalawathugoda'),
        address: env('STORE_ADDRESS', '1035, Pannipitiya Road, Kumaragewattha, Battaramulla'),
        landline: env('STORE_LANDLINE', '0112773232'),
        landline2: env('STORE_LANDLINE_2', '0112773231'),
        hotline: env('STORE_HOTLINE', '0773131883'),
        email: env('STORE_EMAIL', 'lasanthatyretraders@gmail.com'),
        infoEmail: env('STORE_INFO_EMAIL', 'info@lasanthatyre.com'),
        website: env('STORE_WEBSITE', 'www.lasanthatyre.lk'),
        whatsappLink: env('STORE_WHATSAPP_LINK', 'https://wa.me/94771222509'),
        googleMaps: env('STORE_GOOGLE_MAPS', 'https://maps.app.goo.gl/frJHWEo4oRYYiJqw9'),
        vatNo: env('STORE_VAT_NO', '743321219-7000'),
        hours: env('STORE_HOURS', 'Monday-Saturday 8:00 AM - 6:00 PM'),
        wheelAlignmentHours: env('STORE_WHEEL_ALIGNMENT_HOURS', '07:30-18:00'),
    },

    // ── Pricing ──
    pricing: {
        costAddBase: envInt('COST_ADD_BASE', 1500),
        costAddCredit: envInt('COST_ADD_CREDIT', 500),
        minTyreCost: envInt('MIN_TYRE_COST', 3000),
        roundStep: envInt('PRICE_ROUND_STEP', 100),
    },

    // ── Reports ──
    reports: {
        dailyFullReportTime: env('DAILY_FULL_REPORT_TIME', '20:29'),
        includeProfit: envBool('REPORT_INCLUDE_PROFIT'),
        showCost: envBool('REPORT_SHOW_COST'),
        invoiceFilterTyreOnly: envBool('INVOICE_FILTER_TYRE_ONLY', true),
    },

    // ── AI ──
    ai: {
        enabled: envBool('ENABLE_AI_COPILOT', true),
        dbEnabled: envBool('ENABLE_AI_DB'),
        contextMessages: envInt('AI_CONTEXT_MESSAGES', 5),
        geminiKey: env('GEMINI_API_KEY'),
        anthropicKey: env('ANTHROPIC_API_KEY'),
        groqKey: env('GROQ_API_KEY'),
        claudeModel: env('CLAUDE_MODEL', 'claude-haiku-4-5-20251001'),
        ollamaHost: env('OLLAMA_HOST', 'http://localhost:11434'),
    },

    // ── Facebook ──
    facebook: {
        pageId: env('FACEBOOK_PAGE_ID'),
        pageAccessToken: env('FACEBOOK_PAGE_ACCESS_TOKEN'),
        appId: env('FACEBOOK_APP_ID'),
        appSecret: env('FACEBOOK_APP_SECRET'),
        verifyToken: env('FACEBOOK_VERIFY_TOKEN'),
        postAiProvider: env('FB_POST_AI_PROVIDER', 'gemini'),
        disableAi: envBool('FB_POST_DISABLE_AI', true),
        publishMode: env('FB_PUBLISH_MODE', 'draft'),
        approvalMode: env('FB_APPROVAL_MODE', 'whatsapp'),
        localPreviewOnly: envBool('LOCAL_PREVIEW_ONLY'),
    },

    // ── Email ──
    email: {
        provider: env('EMAIL_PROVIDER', 'gmail'),
        user: env('EMAIL_USER'),
        password: env('EMAIL_PASSWORD'),
        fromName: env('EMAIL_FROM_NAME', 'Lasantha Tyre Traders'),
    },

    // ── Paths ──
    paths: {
        cloudflared: env('CLOUDFLARED_PATH') || path.join(APP_ROOT, 'cloudflared.exe'),
        backupRemoteDir: env('BACKUP_REMOTE_DIR', ''),
        backupLocalDir: env('BACKUP_LOCAL_DIR') ? path.resolve(APP_ROOT, env('BACKUP_LOCAL_DIR')) : path.join(APP_ROOT, 'backups', 'sql_archives'),
        pm2LogPath: env('PM2_LOG_PATH', ''),
        postDir: path.join(APP_ROOT, 'post'),
    },

    // ── Dashboard ──
    dashboard: {
        enabled: envBool('DASHBOARD_ENABLED', true),
        protect: envBool('DASHBOARD_PROTECT'),
        key: env('DASHBOARD_KEY'),
        adminUser: env('ADMIN_USER', 'admin'),
        adminPass: env('ADMIN_PASS'),
    },

    // ── Peachtree ──
    peachtree: {
        bridgeBase: env('PEACHTREE_BRIDGE_BASE', 'http://127.0.0.1:5001'),
        bridgePort: envInt('PEACHTREE_BRIDGE_PORT', 5001),
    },

    // ── URLs ──
    urls: {
        royalBooking: env('ROYAL_BOOKING_URL', 'https://book.lasanthatyre.com'),
        botUrl: env('WHATSAPP_BOT_URL', ''),
    },

    // ── QR Notification ──
    qrNotify: {
        email: env('QR_NOTIFY_EMAIL'),
        intervalMinutes: envInt('QR_NOTIFY_INTERVAL_MINUTES', 15),
    },
};

module.exports = config;
