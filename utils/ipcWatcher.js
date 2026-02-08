// IPC Command Watcher (add require('./utils/ipcWatcher') in index.js)
// Non-invasive: triggers existing jobs without touching pricing logic.

const fs = require('fs');
const path = require('path');

const IPC_DIR = path.join(__dirname, '..', 'ipc');
const COMMAND_FILE = path.join(IPC_DIR, 'dashboard-commands.json');

if (!fs.existsSync(IPC_DIR)) {
  try { fs.mkdirSync(IPC_DIR, { recursive: true }); } catch(_){}
}

let lastId = '';
let handler = null; // to be set by main process

function setHandler(fn){ handler = fn; }

function tick(){
  try {
    if(!fs.existsSync(COMMAND_FILE)) return;
    const raw = fs.readFileSync(COMMAND_FILE,'utf8');
    const cmd = JSON.parse(raw);
    if(cmd.id && cmd.id === lastId) return; // already processed
    lastId = cmd.id;
    if(handler){
      Promise.resolve(handler(cmd)).catch(e=>{
        console.error('[IPC] Handler error:', e.message);
      });
    } else {
      console.warn('[IPC] No handler registered for command:', cmd.action);
    }
    // remove after process
    try { fs.unlinkSync(COMMAND_FILE); } catch(_){}
  } catch(e){
    console.error('[IPC] Failed to process command:', e.message);
  }
}

setInterval(tick, 5000);

module.exports = { setHandler };
