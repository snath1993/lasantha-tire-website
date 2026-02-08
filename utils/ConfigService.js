const fs = require('fs');
const path = require('path');
const EventEmitter = require('events');
const dotenv = require('dotenv');

// Centralized configuration with live reload and process.env fallback
class ConfigService extends EventEmitter {
  constructor() {
    super();
    this.configDir = path.join(__dirname, '..', 'config');
    this.configPath = path.join(this.configDir, 'settings.json');
    this.secretsDir = this._getSecretsDir();
    this.secretsConfigPath = path.join(this.secretsDir, 'settings.secrets.json');
    this.envPath = path.join(this.secretsDir, '.env');
    this.legacyEnvPath = path.join(__dirname, '..', '.env');
    this.data = {};
    this.envData = {};
    this.secretsData = {};
    this._ensureFile();
    this._load();
    this._loadSecrets();
    this._loadEnv();
    this._watch();
  }

  _getSecretsDir() {
    // Prefer explicit override
    const explicit = (process.env.WHATSAPP_SQL_API_SECRETS_DIR || process.env.SECRETS_DIR || '').trim();
    if (explicit) return explicit;

    // Windows default
    const appData = (process.env.APPDATA || '').trim();
    if (appData) return path.join(appData, 'whatsapp-sql-api');

    // Cross-platform fallback
    return path.join(__dirname, '..', '.secrets');
  }

  _ensureFile() {
    try { if (!fs.existsSync(this.configDir)) fs.mkdirSync(this.configDir, { recursive: true }); } catch {}
    try { if (!fs.existsSync(this.secretsDir)) fs.mkdirSync(this.secretsDir, { recursive: true }); } catch {}
    if (!fs.existsSync(this.configPath)) {
      const defaults = {
        // Core tokens/IDs (empty by default)
        FACEBOOK_PAGE_ID: process.env.FACEBOOK_PAGE_ID || '',
        FACEBOOK_PAGE_ACCESS_TOKEN: process.env.FACEBOOK_PAGE_ACCESS_TOKEN || '',
        FACEBOOK_APP_SECRET: process.env.FACEBOOK_APP_SECRET || '',
        FACEBOOK_VERIFY_TOKEN: process.env.FACEBOOK_VERIFY_TOKEN || 'lasantha_tyre_secure_2025_webhook_token',
        // AI providers
        FB_POST_AI_PROVIDER: process.env.FB_POST_AI_PROVIDER || 'gemini',
        FB_POST_DISABLE_AI: process.env.FB_POST_DISABLE_AI || 'true', // Default: AI disabled, use fallback captions
        ANTHROPIC_API_KEY: process.env.ANTHROPIC_API_KEY || '',
        GEMINI_API_KEY: process.env.GEMINI_API_KEY || '',
        // Modes and flags
        FB_PUBLISH_MODE: process.env.FB_PUBLISH_MODE || 'draft',
        FB_APPROVAL_MODE: process.env.FB_APPROVAL_MODE || 'whatsapp',
        LOCAL_PREVIEW_ONLY: process.env.LOCAL_PREVIEW_ONLY || 'false',
        POST_IMAGE_MODE: process.env.POST_IMAGE_MODE || 'generate',
        EXTERNAL_IMAGE_DIR: process.env.EXTERNAL_IMAGE_DIR || '',
        POSTER_PICK_STRATEGY: process.env.POSTER_PICK_STRATEGY || 'sequential',
        STORE_NAME: process.env.STORE_NAME || 'Lasantha Tyre Traders',
        PUBLISH_VIA_QUEUE: process.env.PUBLISH_VIA_QUEUE || 'true',
        WA_POSTER_BATCH_WINDOW_SECONDS: process.env.WA_POSTER_BATCH_WINDOW_SECONDS || '60',
        WA_POSTER_PUBLISH_DELAY_MS: process.env.WA_POSTER_PUBLISH_DELAY_MS || '2500'
      };
      try { fs.writeFileSync(this.configPath, JSON.stringify(defaults, null, 2)); } catch {}
    }
  }

  _load() {
    try {
      const raw = fs.readFileSync(this.configPath, 'utf-8');
      const json = JSON.parse(raw || '{}');
      this.data = json || {};
    } catch (e) {
      // keep previous
    }
  }

  _loadSecrets() {
    try {
      if (!fs.existsSync(this.secretsConfigPath)) {
        this.secretsData = {};
        return;
      }
      const raw = fs.readFileSync(this.secretsConfigPath, 'utf-8');
      const json = JSON.parse(raw || '{}');
      this.secretsData = json || {};
    } catch (e) {
      // keep previous
    }
  }

  _loadEnv() {
    try {
      if (fs.existsSync(this.envPath)) {
        const envConfig = dotenv.parse(fs.readFileSync(this.envPath));
        this.envData = envConfig;
        // Also update process.env for compatibility
        for (const k in envConfig) {
          process.env[k] = envConfig[k];
        }
        return;
      }

      // Backward compatibility: legacy repo-root .env
      if (fs.existsSync(this.legacyEnvPath)) {
        const envConfig = dotenv.parse(fs.readFileSync(this.legacyEnvPath));
        this.envData = envConfig;
        for (const k in envConfig) {
          process.env[k] = envConfig[k];
        }
      }
    } catch (e) {
      console.warn('Failed to reload .env:', e.message);
    }
  }

  _watch() {
    try {
      fs.watchFile(this.configPath, { interval: 1000 }, () => {
        const before = JSON.stringify(this.data);
        this._load();
        const after = JSON.stringify(this.data);
        if (before !== after) {
          this.emit('changed', this.data);
        }
      });

      fs.watchFile(this.secretsConfigPath, { interval: 1000 }, () => {
        const before = JSON.stringify(this.secretsData);
        this._loadSecrets();
        const after = JSON.stringify(this.secretsData);
        if (before !== after) {
          this.emit('secrets_changed', this.secretsData);
        }
      });

      fs.watchFile(this.envPath, { interval: 1000 }, () => {
        console.log('🔄 .env file changed, reloading config...');
        this._loadEnv();
        this.emit('env_changed', this.envData);
      });

      // Watch legacy env too (compat)
      fs.watchFile(this.legacyEnvPath, { interval: 1000 }, () => {
        console.log('🔄 Legacy .env file changed, reloading config...');
        this._loadEnv();
        this.emit('env_changed', this.envData);
      });
    } catch {}
  }

  // Basic getters
  get(key, def = undefined) {
    // Priority:
    // 1) process.env (best for production secret injection)
    // 2) secrets file (settings.secrets.json)
    // 3) secrets .env (loaded into envData + process.env for compat)
    // 4) public settings.json
    // 5) default

    const fromEnv = process.env[key];
    if (fromEnv !== undefined && fromEnv !== '') {
      return fromEnv;
    }

    const secretVal = this.secretsData?.[key];
    if (secretVal !== undefined && secretVal !== null && secretVal !== '') {
      return secretVal;
    }

    const val = this.data?.[key];
    if (val !== undefined && val !== null && val !== '') {
      return val;
    }
    
    const envVal = this.envData?.[key];
    if (envVal !== undefined && envVal !== '') {
      return envVal;
    }

    return def;
  }

  getBoolean(key, def = false) {
    const v = this.get(key, def);
    if (typeof v === 'boolean') return v;
    const s = String(v).toLowerCase();
    return s === '1' || s === 'true' || s === 'yes' || s === 'on';
  }

  getNumber(key, def = 0) {
    const v = this.get(key, def);
    const n = Number(v);
    return Number.isFinite(n) ? n : def;
  }

  all() {
    // merge process.env as fallback (but do not leak entire env)
    return { ...this.data };
  }

  set(key, value) {
    this.data[key] = value;
    this._save();
    this.emit('changed', this.data);
  }

  _save() {
    try { fs.writeFileSync(this.configPath, JSON.stringify(this.data, null, 2)); } catch {}
  }

  // Job configuration from jobs-config.json
  getJobConfig(jobName) {
    try {
      const jobsConfigPath = path.join(__dirname, '..', 'config', 'jobs.json');
      if (!fs.existsSync(jobsConfigPath)) return null;
      const raw = fs.readFileSync(jobsConfigPath, 'utf-8');
      const config = JSON.parse(raw);
      return config[jobName] || null;
    } catch (e) {
      console.warn(`⚠️  Failed to load job config for ${jobName}:`, e.message);
      return null;
    }
  }

  // Update job settings in jobs.json
  setJobSettings(jobName, settings) {
    try {
      const jobsConfigPath = path.join(__dirname, '..', 'config', 'jobs.json');
      if (!fs.existsSync(jobsConfigPath)) return false;
      
      const raw = fs.readFileSync(jobsConfigPath, 'utf-8');
      const config = JSON.parse(raw);
      
      if (!config[jobName]) {
        console.warn(`⚠️  Job ${jobName} not found in jobs.json`);
        return false;
      }
      
      config[jobName].settings = { ...(config[jobName].settings || {}), ...settings };
      config[jobName].updatedAt = new Date().toISOString();
      
      fs.writeFileSync(jobsConfigPath, JSON.stringify(config, null, 2));
      console.log(`✅ Job settings updated for ${jobName}`);
      return true;
    } catch (e) {
      console.error(`❌ Failed to update job settings for ${jobName}:`, e.message);
      return false;
    }
  }
}

let singleton;
function getConfigService() {
  if (!singleton) singleton = new ConfigService();
  return singleton;
}

module.exports = { ConfigService: getConfigService() };
