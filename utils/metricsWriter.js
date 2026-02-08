// Periodic runtime metrics writer combining read-only policy + job status + log sizes.
// Require this in index.js AFTER applying readOnlyPatch & loading jobStatusWriter.

const fs = require('fs');
const path = require('path');
const applyReadOnlyPatch = require('./readOnlyPatch');
const { snapshot } = require('./jobStatus');

const METRICS_FILE = path.join(__dirname, '..', 'runtime-metrics.json');
const LOG_FILE = path.join(__dirname, '..', 'whatsapp-bot.log');
const STRUCTURED_LOG_DIR = path.join(__dirname, '..', 'logs');

function collect(){
  const roState = applyReadOnlyPatch.getReadOnlyPolicyState ? applyReadOnlyPatch.getReadOnlyPolicyState() : {};
  let plainLogSize = 0; let structuredLogSize = 0;
  try { if(fs.existsSync(LOG_FILE)) plainLogSize = fs.statSync(LOG_FILE).size; } catch(_){}
  try {
    if(fs.existsSync(STRUCTURED_LOG_DIR)) {
      const files = fs.readdirSync(STRUCTURED_LOG_DIR).filter(f=>f.startsWith('structured.log'));
      structuredLogSize = files.reduce((acc,f)=>{ try { return acc + fs.statSync(path.join(STRUCTURED_LOG_DIR,f)).size; } catch(_) { return acc; } },0);
    }
  } catch(_){}
  return {
    ts: new Date().toISOString(),
    readOnly: roState,
    jobs: snapshot(),
    logs: { plainBytes: plainLogSize, structuredBytes: structuredLogSize }
  };
}

function writeOnce(){
  try {
    const data = JSON.stringify(collect(), null, 2);
    const tmp = METRICS_FILE + '.tmp';
    fs.writeFileSync(tmp, data);
    fs.renameSync(tmp, METRICS_FILE);
  } catch(e){
    // ignore
  }
}

setInterval(writeOnce, 20000);
writeOnce();

module.exports = { METRICS_FILE };
