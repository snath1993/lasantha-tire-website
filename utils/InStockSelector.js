// InStockSelector: Fetch in-stock brand/pattern combos from SQL first
// Falls back cleanly (returns []) if DB is unreachable/misconfigured
const sql = require('mssql');

function getSqlConfigFromEnv() {
  return {
    user: process.env.SQL_USER || process.env.DB_USER,
    password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
    database: process.env.SQL_DATABASE || process.env.DB_NAME,
    server: process.env.SQL_SERVER || process.env.DB_SERVER,
    port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
    pool: { max: 5, min: 0, idleTimeoutMillis: 30000 },
    options: { encrypt: false, trustServerCertificate: true }
  };
}

// Utility: determine if env has enough DB config
function hasDbConfig(cfg) {
  return cfg && cfg.user && cfg.password && cfg.database && cfg.server;
}

/**
 * getInStockBrandPatterns
 * Returns distinct brand/pattern combos that have stock > 0 from View_ItemWhse,
 * excluding motorcycle items and non-tyre items.
 * @param {Object} opts
 * @param {number} opts.limit - max rows to return
 * @param {boolean} opts.excludeMotorcycle - exclude motorcycle tyres
 * @param {number} opts.minQty - minimum total qty
 * @returns {Promise<Array<{brand:string, pattern:string, totalQty:number, itemId:string, itemDis:string}>>}
 */
async function getInStockBrandPatterns(opts = {}) {
  const { limit = 50, excludeMotorcycle = true, minQty = 1 } = opts;
  const cfg = getSqlConfigFromEnv();
  if (!hasDbConfig(cfg)) {
    console.warn('[InStockSelector] DB config missing; skipping DB-first selection');
    return [];
  }

  const pool = new sql.ConnectionPool(cfg);
  try {
    await pool.connect();
    const request = pool.request();
    request.input('limit', sql.Int, limit);
    request.input('minQty', sql.Int, minQty);

    // Query View_ItemWhse - parse brand/pattern from ItemDis
    // ItemDis format examples:
    // "215/55/16 MAXXIS HP5 TAIWAN"
    // "300/18 - R T/L TIMSUN TS628 CHINA"
    const query = `
      SELECT TOP (@limit)
        ItemId,
        ItemDis,
        SUM(QTY) AS TotalQty
      FROM View_ItemWhse
      WHERE QTY >= @minQty
        AND ItemDis IS NOT NULL
        AND ItemDis LIKE '%/%' 
        ${excludeMotorcycle ? "AND ItemDis NOT LIKE '%Motor%' AND ItemDis NOT LIKE '%/18%' AND ItemDis NOT LIKE '%/17%' AND ItemDis NOT LIKE '%/16 R%' AND ItemDis NOT LIKE '%/14 R%' AND ItemDis NOT LIKE '%TIMSUN%' AND ItemDis NOT LIKE '%TUBE%' AND ItemDis NOT LIKE '%KATTA%'" : ''}
      GROUP BY ItemId, ItemDis
      HAVING SUM(QTY) >= @minQty
      ORDER BY SUM(QTY) DESC
    `;

    const result = await request.query(query);

    if (!result || !Array.isArray(result.recordset)) {
      return [];
    }

    // Parse brand and pattern from ItemDis
    const rows = result.recordset.map(r => {
      const itemDis = String(r.ItemDis || '');
      
      // Extract brand (usually after size, before country)
      // Pattern: "215/55/16 MAXXIS HP5 TAIWAN" -> Brand: MAXXIS, Pattern: HP5
      const parts = itemDis.split(' ').filter(p => p.trim());
      
      let brand = '';
      let pattern = '';
      
      // Common brands (exact match only, minimum 2 characters)
      const knownBrands = ['MAXXIS', 'GT', 'BRIDGESTONE', 'MICHELIN', 'YOKOHAMA', 
                          'DURATURN', 'GITI', 'CONTINENTAL', 'PIRELLI', 'KUMHO',
                          'TOYO', 'FEDERAL', 'GOODRIDE', 'HIFLY', 'LANVIGATOR',
                          'MARSHAL', 'NEXEN', 'RUNWAY', 'TIMSUN', 'TRIANGLE',
                          'LEAO', 'LINGLONG', 'TRACMAX', 'MATRAX', 'RAPID',
                          'ROADSTONE', 'LENSO', 'ROYAL', 'BLACK', 'CEAT'];
      
      // Find brand in parts (skip size patterns like "215/55/16")
      for (let i = 0; i < parts.length; i++) {
        const part = parts[i].toUpperCase();
        
        // Skip size patterns
        if (part.match(/^\d+\/\d+/)) continue;
        
        // Skip single letters and common words
        if (part.length < 2) continue;
        if (['R', 'T/L', 'F/R', 'PR', 'LT', 'XL'].includes(part)) continue;
        
        // Match brand (exact match preferred)
        const exactMatch = knownBrands.find(kb => kb === part);
        if (exactMatch) {
          brand = exactMatch;
          // Pattern is usually the next part
          if (i + 1 < parts.length) {
            pattern = parts[i + 1];
            // If next part looks like country, try next one
            if (pattern.match(/^(CHINA|TAIWAN|THAILAND|JAPAN|KOREA|INDONESIA|SINGAPORE|USA|GERMANY|SRI)$/i)) {
              pattern = (i + 2 < parts.length) ? parts[i + 2] : '';
            }
          }
          break;
        }
        
        // Partial match (for compound names like "ROYAL BLACK")
        const partialMatch = knownBrands.find(kb => part.includes(kb) && part.length >= 3);
        if (partialMatch && !brand) {
          brand = partialMatch;
          // Check if next part is also part of brand name
          if (i + 1 < parts.length && knownBrands.includes(parts[i + 1].toUpperCase())) {
            brand = `${brand} ${parts[i + 1].toUpperCase()}`;
            i++; // Skip next part
          }
          // Pattern comes after brand
          if (i + 1 < parts.length) {
            pattern = parts[i + 1];
            if (pattern.match(/^(CHINA|TAIWAN|THAILAND|JAPAN|KOREA|INDONESIA|SINGAPORE|USA|GERMANY|SRI)$/i)) {
              pattern = (i + 2 < parts.length) ? parts[i + 2] : '';
            }
          }
          break;
        }
      }
      
      // If still no brand found, try to extract from ItemId
      if (!brand) {
        const itemIdParts = (r.ItemId || '').split('/');
        if (itemIdParts.length >= 3) {
          // Format: "215/55/16/MAX/HP5" -> Brand: MAX
          brand = (itemIdParts[3] || '').toUpperCase();
          pattern = (itemIdParts[4] || '').toUpperCase();
        }
      }

      return {
        brand: brand.trim(),
        pattern: pattern.trim(),
        totalQty: Number(r.TotalQty || 0),
        itemId: String(r.ItemId || ''),
        itemDis: itemDis
      };
    }).filter(r => r.brand && r.totalQty >= minQty);

    return rows;
  } catch (err) {
    console.warn('[InStockSelector] Query failed:', err.message);
    return [];
  } finally {
    try { pool.close(); } catch {}
  }
}

/**
 * getInStockBrandsOnly
 * Returns distinct brands that have stock > 0 (excludes motorcycle items by default)
 */
async function getInStockBrandsOnly(opts = {}) {
  const { limit = 50, excludeMotorcycle = true, minQty = 1 } = opts;
  const cfg = getSqlConfigFromEnv();
  if (!hasDbConfig(cfg)) {
    console.warn('[InStockSelector] DB config missing; skipping brand list');
    return [];
  }

  const pool = new sql.ConnectionPool(cfg);
  try {
    await pool.connect();
    const request = pool.request();
    request.input('minQty', sql.Int, minQty);

    // Query View_ItemWhse and extract brands from ItemDis
    const query = `
      SELECT ItemDis, SUM(QTY) AS TotalQty
      FROM View_ItemWhse
      WHERE QTY >= @minQty
        AND ItemDis IS NOT NULL
        AND ItemDis LIKE '%/%'
        ${excludeMotorcycle ? "AND ItemDis NOT LIKE '%Motor%' AND ItemDis NOT LIKE '%/18%' AND ItemDis NOT LIKE '%/17%' AND ItemDis NOT LIKE '%/16 R%' AND ItemDis NOT LIKE '%/14 R%' AND ItemDis NOT LIKE '%TIMSUN%' AND ItemDis NOT LIKE '%TUBE%' AND ItemDis NOT LIKE '%KATTA%'" : ''}
      GROUP BY ItemDis
      HAVING SUM(QTY) >= @minQty
    `;

    const result = await request.query(query);

    if (!result || !Array.isArray(result.recordset)) {
      return [];
    }

    // Parse brands and aggregate quantities
    const knownBrands = ['MAXXIS', 'GT', 'BRIDGESTONE', 'MICHELIN', 'YOKOHAMA', 
                        'DURATURN', 'GITI', 'CONTINENTAL', 'PIRELLI', 'KUMHO',
                        'TOYO', 'FEDERAL', 'GOODRIDE', 'HIFLY', 'LANVIGATOR',
                        'MARSHAL', 'NEXEN', 'RUNWAY', 'TIMSUN', 'TRIANGLE',
                        'LEAO', 'LINGLONG', 'TRACMAX', 'MATRAX', 'RAPID',
                        'ROADSTONE', 'LENSO', 'ROYAL', 'BLACK', 'CEAT'];

    const brandMap = new Map();

    result.recordset.forEach(r => {
      const itemDis = String(r.ItemDis || '');
      const parts = itemDis.split(' ').filter(p => p.trim());
      
      let foundBrand = null;
      for (let part of parts) {
        part = part.toUpperCase();
        
        // Skip size patterns and short words
        if (part.match(/^\d+\/\d+/) || part.length < 2) continue;
        if (['R', 'T/L', 'F/R', 'PR', 'LT', 'XL'].includes(part)) continue;
        
        // Exact match
        if (knownBrands.includes(part)) {
          foundBrand = part;
          break;
        }
        
        // Partial match (minimum 3 chars)
        const partialMatch = knownBrands.find(kb => part.includes(kb) && part.length >= 3);
        if (partialMatch) {
          foundBrand = partialMatch;
          break;
        }
      }
      
      if (foundBrand) {
        const existingQty = brandMap.get(foundBrand) || 0;
        brandMap.set(foundBrand, existingQty + Number(r.TotalQty || 0));
      }
    });

    // Convert to array and sort by quantity
    const rows = Array.from(brandMap.entries())
      .map(([brand, totalQty]) => ({ brand, totalQty }))
      .sort((a, b) => b.totalQty - a.totalQty)
      .slice(0, limit);

    return rows;
  } catch (err) {
    console.warn('[InStockSelector] Brand list query failed:', err.message);
    return [];
  } finally {
    try { pool.close(); } catch {}
  }
}

module.exports = { getInStockBrandPatterns, getInStockBrandsOnly };
