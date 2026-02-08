// Structured JSONL logger (parallel, non-invasive)
// Usage: const log = require('./utils/structuredLogger'); log.info('event_name',{detail:'x'})

const fs = require('fs');
const path = require('path');

const LOG_DIR = path.join(__dirname, '..', 'logs');
if(!fs.existsSync(LOG_DIR)) { try { fs.mkdirSync(LOG_DIR, { recursive: true }); } catch(_){} }
const FILE = path.join(LOG_DIR, 'structured.log.jsonl');
const MAX_SIZE = 5 * 1024 * 1024; // 5MB rotation threshold

function rotateIfNeeded(){
  try {
    if(fs.existsSync(FILE)) {
      const stat = fs.statSync(FILE);
      if(stat.size >= MAX_SIZE){
        const stamp = new Date().toISOString().replace(/[:.]/g,'-');
        fs.renameSync(FILE, FILE + '.' + stamp + '.bak');
      }
    }
  } catch(_){}
}

function write(level, event, data){
  try {
    rotateIfNeeded();
    const line = JSON.stringify({ ts:new Date().toISOString(), level, event, ...data }) + '\n';
    fs.appendFile(FILE, line, ()=>{});
  } catch(_){}
}

module.exports = {
  info: (event, data={}) => write('info', event, data),
  warn: (event, data={}) => write('warn', event, data),
  error: (event, data={}) => write('error', event, data),
  file: FILE
};
