/*
  Secrets migration script

  Goal:
  - Move sensitive keys/tokens/passwords out of repo-visible paths into an OS-local secrets directory.
  - Preserve behavior: app still loads secrets via ConfigService (process.env -> secrets file -> legacy files).

  This script:
  - Creates a secrets directory (default: %APPDATA%\whatsapp-sql-api on Windows)
  - Copies (does NOT print) secrets from:
      1) repo-root .env (legacy)
      2) config/settings.json
    into:
      - <secretsDir>/.env
      - <secretsDir>/settings.secrets.json
  - Scrubs secret fields in config/settings.json (sets to empty string) so it can be safely committed if desired.

  Run:
    node scripts/migrate-secrets.js
*/

const fs = require('fs');
const path = require('path');
const dotenv = require('dotenv');

function getSecretsDir() {
  const explicit = (process.env.WHATSAPP_SQL_API_SECRETS_DIR || process.env.SECRETS_DIR || '').trim();
  if (explicit) return explicit;
  const appData = (process.env.APPDATA || '').trim();
  if (appData) return path.join(appData, 'whatsapp-sql-api');
  return path.join(__dirname, '..', '.secrets');
}

const projectRoot = path.join(__dirname, '..');
const legacyEnvPath = path.join(projectRoot, '.env');
const publicConfigPath = path.join(projectRoot, 'config', 'settings.json');

const secretsDir = getSecretsDir();
const secretsEnvPath = path.join(secretsDir, '.env');
const secretsConfigPath = path.join(secretsDir, 'settings.secrets.json');

const SECRET_KEYS = [
  'SQL_SERVER',
  'SQL_USER',
  'SQL_PASSWORD',
  'SQL_DATABASE',
  'SQL_PORT',
  'FACEBOOK_PAGE_ID',
  'FACEBOOK_PAGE_ACCESS_TOKEN',
  'FACEBOOK_APP_SECRET',
  'FACEBOOK_VERIFY_TOKEN',
  'ANTHROPIC_API_KEY',
  'GEMINI_API_KEY'
];

function ensureDir(dir) {
  try {
    if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
  } catch (e) {
    console.error('❌ Failed to create secrets directory:', dir);
    console.error(e.message);
    process.exit(1);
  }
}

function safeReadJson(p) {
  try {
    if (!fs.existsSync(p)) return null;
    const raw = fs.readFileSync(p, 'utf8');
    return JSON.parse(raw || '{}');
  } catch {
    return null;
  }
}

function safeWriteJson(p, obj) {
  fs.writeFileSync(p, JSON.stringify(obj, null, 2), 'utf8');
}

function parseEnvFile(p) {
  try {
    if (!fs.existsSync(p)) return {};
    return dotenv.parse(fs.readFileSync(p, 'utf8'));
  } catch {
    return {};
  }
}

function renderEnv(envObj) {
  const keys = Object.keys(envObj).sort();
  return keys.map(k => `${k}=${envObj[k]}`).join('\n') + '\n';
}

function main() {
  ensureDir(secretsDir);

  const legacyEnv = parseEnvFile(legacyEnvPath);
  const publicCfg = safeReadJson(publicConfigPath) || {};
  const existingSecretsCfg = safeReadJson(secretsConfigPath) || {};

  // Merge secrets: precedence -> existing secrets file > legacy env > public cfg
  const mergedSecrets = { ...existingSecretsCfg };

  for (const k of SECRET_KEYS) {
    if (mergedSecrets[k] !== undefined && mergedSecrets[k] !== '') continue;
    if (legacyEnv[k] !== undefined && legacyEnv[k] !== '') {
      mergedSecrets[k] = legacyEnv[k];
      continue;
    }
    if (publicCfg[k] !== undefined && publicCfg[k] !== '') {
      mergedSecrets[k] = String(publicCfg[k]);
    }
  }

  // Write secrets JSON
  safeWriteJson(secretsConfigPath, mergedSecrets);

  // Write secrets .env (helps legacy code paths that depend on dotenv style)
  const envForSecrets = {};
  for (const k of SECRET_KEYS) {
    if (mergedSecrets[k] !== undefined && mergedSecrets[k] !== '') envForSecrets[k] = mergedSecrets[k];
  }
  fs.writeFileSync(secretsEnvPath, renderEnv(envForSecrets), 'utf8');

  // Scrub secrets from public config (keep keys but blank them)
  const scrubbedCfg = { ...publicCfg };
  let scrubbedCount = 0;
  for (const k of SECRET_KEYS) {
    if (scrubbedCfg[k] !== undefined && scrubbedCfg[k] !== '') {
      scrubbedCfg[k] = '';
      scrubbedCount += 1;
    }
  }

  // Ensure public config dir exists
  ensureDir(path.dirname(publicConfigPath));
  safeWriteJson(publicConfigPath, scrubbedCfg);

  console.log('✅ Secrets migration complete.');
  console.log('   Secrets directory:', secretsDir);
  console.log('   Wrote:', secretsEnvPath);
  console.log('   Wrote:', secretsConfigPath);
  console.log('   Scrubbed secret fields in:', publicConfigPath, `(fields blanked: ${scrubbedCount})`);
  console.log('');
  console.log('Next steps:');
  console.log(' - Ensure your secrets directory is backed up securely.');
  console.log(' - Rotate any tokens that were previously committed/shared.');
}

if (require.main === module) {
  main();
}
