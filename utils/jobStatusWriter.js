// Periodically persist in-memory job status snapshot to a JSON file for the dashboard
const fs = require('fs');
const path = require('path');
const { snapshot } = require('./jobStatus');

const SNAP_FILE = path.join(__dirname, '../job-status.json');
const INTERVAL_MS = 15000; // 15 seconds

function atomicWrite(targetPath, data, cb){
  try {
    const tmp = targetPath + '.tmp';
    fs.writeFile(tmp, data, (err)=>{
      if(err) return cb && cb(err);
      fs.rename(tmp, targetPath, ()=> cb && cb());
    });
  } catch(e){
    cb && cb(e);
  }
}

function write() {
  try {
    const data = JSON.stringify(snapshot(), null, 2);
  atomicWrite(SNAP_FILE, data, ()=>{});
  } catch (e) {
    // Silent fail – dashboard can handle absence
  }
}

setInterval(write, INTERVAL_MS);
write();

module.exports = {};