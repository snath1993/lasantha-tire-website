const sql = require('mssql');
const fs = require('fs');
const path = require('path');
const { normalizeBrand } = require('./brandUtils');
require('dotenv').config();

/**
 * Fetch tyre image from database (supports binary data and file path)
 * @param {string} brand - Tyre brand name
 * @param {string} pattern - Tyre pattern name
 * @returns {Promise<object|null>} Image data or null if not found
 */
// In-memory cache for local metadata to avoid disk reads per call
let metadataCache = null;
let metadataMtimeMs = 0;
let loggedMissingTable = false;

function normalizePattern(pat) {
  if (!pat) return '';
  return pat.toUpperCase().trim().replace(/\s+/g, ' ');
}

function loosePatternKey(pat) {
  return normalizePattern(pat).replace(/[\s\-_.]/g, '');
}

function loadLocalMetadata() {
  try {
    const metaPath = path.join(process.cwd(), 'tyre-images', 'metadata.json');
    if (!fs.existsSync(metaPath)) return null;
    const stat = fs.statSync(metaPath);
    if (!metadataCache || stat.mtimeMs !== metadataMtimeMs) {
      const raw = fs.readFileSync(metaPath, 'utf8');
      const arr = JSON.parse(raw);
      // Build lookup maps for fast match
      const byBrand = new Map();
      const byKey = new Map(); // brand|patternLoose -> entry
      for (const e of arr) {
        const b = normalizeBrand(e.brand || '');
        const p = normalizePattern(e.pattern || '');
        const k = `${b}|${loosePatternKey(p)}`;
        byKey.set(k, e);
        if (!byBrand.has(b)) byBrand.set(b, []);
        byBrand.get(b).push(e);
      }
      metadataCache = { arr, byBrand, byKey };
      metadataMtimeMs = stat.mtimeMs;
    }
    return metadataCache;
  } catch (e) {
    // Silent fail – no local metadata
    return null;
  }
}

function resolveLocalPath(p) {
  if (!p) return null;
  if (path.isAbsolute(p)) return fs.existsSync(p) ? p : null;
  const abs = path.join(process.cwd(), p.replace(/^\.\//, ''));
  return fs.existsSync(abs) ? abs : null;
}

function tryFilesystemFallback(brand, pattern) {
  const b = normalizeBrand(brand);
  const p = normalizePattern(pattern);
  
  // SCAN FILESYSTEM FIRST - ignore metadata cache with outdated .jpg paths
  // Prefer PNG > JPG > WEBP, and skip files <5KB (corrupted)
  try {
    const dir = path.join(process.cwd(), 'tyre-images');
    if (!fs.existsSync(dir)) return null;
    const files = fs.readdirSync(dir);
    const candidates = [
      `${b}_${p}`,
      `${b}_${p.replace(/\s+/g, '_')}`,
      `${b}_${p.replace(/\s+/g, '')}`,
      `${b}-${p}`,
      `${b}${p}`,
    ];
    // Prefer PNG (usually better quality), then jpg/jpeg, then webp
    const exts = ['.png', '.jpg', '.jpeg', '.webp'];
    for (const base of candidates) {
      for (const ext of exts) {
        const match = files.find(f => f.toUpperCase() === `${base}${ext}`.toUpperCase());
        if (match) {
          const abs = path.join(dir, match);
          // Check file size - skip if too small (likely corrupted)
          const stats = fs.statSync(abs);
          if (stats.size < 5000) {
            // Skip files smaller than 5KB (likely corrupted)
            console.log(`[fetchTyreImage] Skipping small file ${match} (${stats.size} bytes)`);
            continue;
          }
          // Found good file
          return { brand: b, pattern: p, local_path: abs };
        }
      }
    }
  } catch (err) {
    console.error('[fetchTyreImage] Filesystem scan error:', err.message);
  }
  
  // Only use metadata as last resort if filesystem scan failed
  const cache = loadLocalMetadata();
  if (cache && cache.byKey) {
    const hit = cache.byKey.get(`${b}|${loosePatternKey(p)}`) || cache.byKey.get(`${b}|${loosePatternKey(p.replace(/-/g, ''))}`);
    if (hit) {
      const local = resolveLocalPath(hit.local_path || hit.localPath);
      if (local && fs.existsSync(local)) {
        const stats = fs.statSync(local);
        if (stats.size >= 5000) {
          return {
            brand: b,
            pattern: p,
            local_path: local,
            image_url: hit.imageUrl || hit.image_url || null,
            description: hit.description || null,
            category: hit.category || null,
            view_type: hit.viewType || hit.view_type || null
          };
        } else {
          console.log(`[fetchTyreImage] Skipping small metadata file (${stats.size} bytes)`);
        }
      }
    }
  }
  
  return null;
}

/**
 * Find ALL images matching a brand and pattern (supports multiple photos per pattern)
 * @param {string} brand - Tyre brand name
 * @param {string} pattern - Tyre pattern name
 * @returns {Array<object>} Array of image data objects (empty if none found)
 */
function findAllTyreImages(brand, pattern) {
  const b = normalizeBrand(brand);
  const p = normalizePattern(pattern);
  const results = [];

  try {
    // Search in brand-specific subfolder first
    const brandDir = path.join(process.cwd(), 'tyre-images', b);
    if (fs.existsSync(brandDir)) {
      const files = fs.readdirSync(brandDir);
      const exts = ['.png', '.jpg', '.jpeg', '.webp'];
      
      // Find all files matching pattern (case-insensitive)
      for (const file of files) {
        const fileUpper = file.toUpperCase();
        const patternClean = p.replace(/\s+/g, '').replace(/[\-_]/g, '');
        const fileClean = fileUpper.replace(/\s+/g, '').replace(/[\-_]/g, '');
        
        // Check if filename contains the pattern
        if (fileClean.includes(patternClean) || fileClean.includes(p.replace(/\s+/g, '-').toUpperCase())) {
          const abs = path.join(brandDir, file);
          const stats = fs.statSync(abs);
          if (stats.size >= 5000) { // Skip corrupted files
            results.push({ brand: b, pattern: p, local_path: abs, filename: file });
          }
        }
      }
    }

    // Fallback to root tyre-images folder if nothing found
    if (results.length === 0) {
      const dir = path.join(process.cwd(), 'tyre-images');
      if (fs.existsSync(dir)) {
        const files = fs.readdirSync(dir);
        const candidates = [
          `${b}_${p}`,
          `${b}_${p.replace(/\s+/g, '_')}`,
          `${b}_${p.replace(/\s+/g, '')}`,
          `${b}-${p}`,
        ];
        const exts = ['.png', '.jpg', '.jpeg', '.webp'];
        
        for (const base of candidates) {
          for (const ext of exts) {
            const match = files.find(f => f.toUpperCase() === `${base}${ext}`.toUpperCase());
            if (match) {
              const abs = path.join(dir, match);
              const stats = fs.statSync(abs);
              if (stats.size >= 5000) {
                results.push({ brand: b, pattern: p, local_path: abs, filename: match });
              }
            }
          }
        }
      }
    }
  } catch (err) {
    console.error('[findAllTyreImages] Error:', err.message);
  }

  return results;
}

async function fetchTyreImage(brand, pattern) {
  // Normalize inputs
  const normalizedBrand = normalizeBrand(brand);
  const normalizedPattern = normalizePattern(pattern);

  // SKIP DATABASE - directly use local filesystem
  // Database images may be corrupted, so we rely entirely on local files
  
  // Try local filesystem metadata or direct file match
  const local = tryFilesystemFallback(normalizedBrand, normalizedPattern);
  if (local) return local;

  // Nothing found
  return null;
}

module.exports = { fetchTyreImage, findAllTyreImages };
