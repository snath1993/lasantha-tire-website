// Unified Read-Only Patch with Metrics (backward compatible export)
// Keeps original behavior (blocks obvious write verbs) and now tracks metrics similar to dashboard readOnlyPolicy.
// Does NOT change caller expectations: module.exports is still the patch function.

const WRITE_REGEX = /\b(insert|update|delete|alter|drop|create|truncate|merge|exec|execute)\b/i;

const metrics = {
  totalSelects: 0,
  blockedWrites: 0,
  avgLatencyMs: 0,
  lastBlockedExample: undefined,
  lastQueryAt: undefined
};

function reconstruct(strings, values) {
  let out = '';
  for (let i = 0; i < strings.length; i++) { out += strings[i]; if (i < values.length) out += `@v${i}`; }
  return out;
}

function isWrite(text){
  const trimmed = (text||'').trim();
  if(!trimmed) return false;
  if(/^select\b/i.test(trimmed) || /^with\b/i.test(trimmed)) return false;
  return WRITE_REGEX.test(trimmed);
}

function recordLatency(startMs, sqlText){
  const dur = Date.now() - startMs;
  if(/^\s*(select|with)\b/i.test(sqlText||'')) {
    const a = 0.2;
    metrics.totalSelects += 1;
    metrics.avgLatencyMs = metrics.avgLatencyMs === 0 ? dur : (a * dur) + (1 - a) * metrics.avgLatencyMs;
    metrics.lastQueryAt = new Date().toISOString();
  }
}

function enforceReadOnly(sqlText){
  const flag = (process.env.MAIN_DB_READ_ONLY || process.env.READ_ONLY_ENFORCE || 'true').toLowerCase();
  if(flag === 'false') return; // disabled intentionally
  if(isWrite(sqlText)) {
    metrics.blockedWrites += 1;
    if(!metrics.lastBlockedExample) metrics.lastBlockedExample = sqlText.slice(0,200);
    const err = new Error('Write operation blocked by read-only policy');
    // @ts-ignore
    err.code = 'READ_ONLY_BLOCK';
    throw err;
  }
}

function getReadOnlyPolicyState(){
  return {
    mainDbReadOnly: (process.env.MAIN_DB_READ_ONLY || process.env.READ_ONLY_ENFORCE || 'true').toLowerCase() !== 'false',
    metrics: { ...metrics }
  };
}

function applyReadOnlyPatch(sql){
  if(!sql || typeof sql.query !== 'function') return;
  if(sql.__READ_ONLY_PATCHED__) return;
  const original = sql.query.bind(sql);
  sql.query = async function wrapped(first, ...rest){
    const start = Date.now();
    let text = '';
    try {
      if(Array.isArray(first) && Object.prototype.hasOwnProperty.call(first,'raw')) text = reconstruct(first, rest);
      else if(typeof first === 'string') text = first;
      // Enforce
      enforceReadOnly(text);
    } catch(e){
      // If enforcement threw, propagate (blocked write) preserving behavior
      return Promise.reject(e);
    }
    try {
      const result = await original(first, ...rest);
      recordLatency(start, text);
      return result;
    } catch(err){
      // Still record latency for failed SELECT attempts (optional)
      recordLatency(start, text);
      throw err;
    }
  };
  sql.__READ_ONLY_PATCHED__ = true;
}

// Backward compatible export (function) + attach helpers
applyReadOnlyPatch.metrics = metrics;
applyReadOnlyPatch.getReadOnlyPolicyState = getReadOnlyPolicyState;
applyReadOnlyPatch.enforceReadOnly = enforceReadOnly;

module.exports = applyReadOnlyPatch;