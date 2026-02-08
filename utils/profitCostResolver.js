// utils/profitCostResolver.js
// Centralized advanced cost lookup with caching & scoring

const _cache = new Map();

function extractSizeToken(desc) {
  if (!desc) return null;
  const patterns = [
    /\b\d{3}\/\d{2}\/\d{2}\b/,
    /\b\d{3}\/\d{2}R\d{2}\b/i,
    /\b\d{3}-\d{2}-\d{2}\b/,
    /\b\d{3}\s+\d{2}\s+\d{2}\b/,
    /\b\d{3}\/\d{2,3}\/\d{2}\b/,
  ];
  for (const p of patterns) {
    const m = desc.match(p); if (m) return m[0].replace(/\s+/g,'').replace(/-/g,'/').toUpperCase();
  }
  return null;
}

function median(nums){ if(!nums.length) return null; const s=[...nums].sort((a,b)=>a-b); const m=Math.floor(s.length/2); return s.length%2? s[m] : (s[m-1]+s[m])/2; }

async function lookupCostForDescription(sql, description, { debug = false, pricingContext } = {}) {
  if (!description) return null;
  if (_cache.has(description)) return _cache.get(description);
  const sizeTok = extractSizeToken(description);
  if (!sizeTok) { _cache.set(description, null); return null; }
  try {
    const query = await sql.query`
      SELECT TOP 25 ItemDescription, UnitCost
      FROM tblItemMaster
      WHERE ItemDescription LIKE ${'%' + sizeTok + '%'}
        AND UnitCost IS NOT NULL AND UnitCost > 0
      ORDER BY UnitCost ASC
    `;
    const rows = query.recordset || [];
    if (rows.length === 0) { _cache.set(description,null); return null; }
    const rawTokens = description.toUpperCase().replace(sizeTok,'').split(/[\s\-]+/).filter(w => w && w.length>2 && !/^(CHINA|SRI|LANKA|JAPAN|TYRE|TIRE|PLUS|T\/L|TL|T)$/.test(w));
    let best=null; const candidateCosts=[];
    for (const r of rows) {
      const u = (r.ItemDescription||'').toUpperCase();
      let score = 0;
      for (const t of rawTokens) if (u.includes(t)) score += 2;
      if (u.includes(sizeTok)) score += 3;
      const unitCost = Number(r.UnitCost)||0;
      candidateCosts.push(unitCost);
      if (!best || score>best.score || (score===best.score && unitCost>best.unitCost)) {
        best = { score, unitCost };
      }
    }
  let chosen = best ? best.unitCost : null;
    const med = median(candidateCosts);
    // Sanity: if we have pricing context (minUnit, avgUnit) enforce guard rails
    if (pricingContext && chosen != null) {
      const { minUnit, avgUnit } = pricingContext;
      if (minUnit && chosen > minUnit * 1.15) {
        if (debug) console.log('[COST] Rejecting cost above 115% of min unit', chosen,'min',minUnit);
        chosen = med && med < minUnit * 1.15 ? med : null;
      }
      if (avgUnit && chosen && chosen > avgUnit * 1.10) {
        if (debug) console.log('[COST] Adjusting cost above 110% of avg unit', chosen,'avg',avgUnit);
        chosen = med && med < avgUnit * 1.10 ? med : avgUnit * 1.05;
      }
    }
  if (debug) console.log('[COST] pre-cache chosen', description, '=>', chosen, 'score', best && best.score);
  _cache.set(description, chosen);
    return chosen;
  } catch (e) {
    if (debug) console.log('[COST] error', e.message);
    _cache.set(description,null); return null;
  }
}

async function batchLookupCosts(sql, descriptions, opts={}) {
  const uniq = Array.from(new Set(descriptions.filter(Boolean)));
  // sequential might be fine for small sets; could parallelize with Promise.all but beware connection pooling
  for (const d of uniq) {
    if (!_cache.has(d)) await lookupCostForDescription(sql, d, opts);
  }
  return uniq.reduce((acc,d)=>{ acc[d]=_cache.get(d)||null; return acc; },{});
}

function getCachedCost(description) { return _cache.get(description) || null; }
function cacheSize() { return _cache.size; }
function resetCache() { _cache.clear(); }

module.exports = {
  extractSizeToken,
  lookupCostForDescription,
  batchLookupCosts,
  getCachedCost,
  cacheSize,
  resetCache
};
