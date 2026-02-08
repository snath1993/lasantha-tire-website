// utils/watchedItemConfig.js
// JSON-based configuration and state for watched item notifications.
const fs = require('fs');
const path = require('path');

const CONFIG_FILE = path.join(__dirname, '..', 'watched-item-config.json');
const STATE_FILE = path.join(__dirname, '..', 'watched-item-state.json');

function loadConfig() {
  let cfg;
  if (!fs.existsSync(CONFIG_FILE)) {
    cfg = { pattern: '', patterns: [], adminNumbers: [], minQty: 1 };

    // Recover patterns from state if config file is missing (prevents job from silently stopping)
    const recoverFromState = String(process.env.WATCHED_ITEM_RECOVER_FROM_STATE || 'true').toLowerCase() === 'true';
    if (recoverFromState && fs.existsSync(STATE_FILE)) {
      try {
        const st = JSON.parse(fs.readFileSync(STATE_FILE, 'utf8'));
        const keys = st && st.patterns ? Object.keys(st.patterns) : [];
        const recovered = keys.filter(k => k && k !== '__legacy__' && String(k).trim());
        if (recovered.length > 0) {
          cfg.patterns = recovered;
          // Best-effort: recreate config file so admins can edit it again
          try { fs.writeFileSync(CONFIG_FILE, JSON.stringify(cfg, null, 2)); } catch {}
        }
      } catch {
        // ignore
      }
    }
  } else {
    try {
      cfg = JSON.parse(fs.readFileSync(CONFIG_FILE,'utf8'));
    } catch (err) {
      console.error('Error reading config file:', err);
      cfg = { pattern: '', patterns: [], adminNumbers: [], minQty: 1 };
    }
  }
  
  // Ensure we have a valid configuration object
  cfg = cfg || {};
  
  // Handle legacy pattern field
  if (!cfg.patterns || !Array.isArray(cfg.patterns)) {
    const arr = [];
    if (cfg.pattern && typeof cfg.pattern === 'string' && cfg.pattern.trim()) {
      arr.push(cfg.pattern.trim());
    }
    cfg.patterns = arr;
  }

  // Filter out invalid patterns
  cfg.patterns = cfg.patterns.filter(p => p && typeof p === 'string' && p.trim());

  // Handle admin numbers
  cfg.adminNumbers = cfg.adminNumbers || [];
  if (!Array.isArray(cfg.adminNumbers)) {
    cfg.adminNumbers = [];
  }

  // Auto-fill admin numbers from environment if empty
  if ((!cfg.adminNumbers || cfg.adminNumbers.length === 0) && process.env.ADMIN_NUMBERS) {
    cfg.adminNumbers = process.env.ADMIN_NUMBERS.split(',')
      .map(s => s.trim())
      .filter(s => s && s.length > 0);
  }

  // Ensure we have minQty
  cfg.minQty = parseInt(cfg.minQty) || 1;

  return cfg;
}

function saveConfig(cfg) {
  if (!cfg.patterns || !Array.isArray(cfg.patterns)) {
    cfg.patterns = [];
    if (cfg.pattern && cfg.pattern.trim()) cfg.patterns.push(cfg.pattern.trim());
  }
  fs.writeFileSync(CONFIG_FILE, JSON.stringify(cfg, null, 2));
}

function loadState() {
  if (!fs.existsSync(STATE_FILE)) return { patterns: {} };
  try {
    const st = JSON.parse(fs.readFileSync(STATE_FILE,'utf8'));
    if (st.lastNotified) {
      st.patterns = st.patterns || {};
      st.patterns['__legacy__'] = { lastNotified: st.lastNotified, totalQtyToday: st.totalQtyToday || 0, lastUpdate: st.lastUpdate || null };
      delete st.lastNotified; delete st.totalQtyToday; delete st.lastUpdate;
    }
    return st;
  } catch {
    return { patterns: {} };
  }
}

function saveState(st) {
  fs.writeFileSync(STATE_FILE, JSON.stringify(st, null, 2));
}

module.exports = { loadConfig, saveConfig, loadState, saveState, CONFIG_FILE, STATE_FILE };
