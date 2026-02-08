// Non-invasive working directory guard.
// Call require('./utils/workingDirGuard') early in index.js (top) to warn if bot started from a subfolder.
// Does NOT throw or exit unless a clearly wrong path pattern is detected repeatedly.

const path = require('path');
let warned = false;

(function guard(){
  try {
    const cwd = process.cwd();
    // Expect root folder to contain 'whatsapp-sql-api' (adjust if renamed)
    const base = path.basename(cwd).toLowerCase();
    // If user accidentally started inside jobs/Ui or any deeper path
    if (/(^ui$)|(^jobs$)/.test(base) || /\\jobs\\ui$/i.test(cwd) || /\/jobs\/ui$/i.test(cwd)) {
      if (!warned) {
        warned = true;
        console.warn('[RUNTIME GUARD] Detected launch from a sub-directory (', cwd, ').');
        console.warn('[RUNTIME GUARD] Please cd to the project root (e.g. C:/whatsapp-sql-api) then run: node index.js');
      }
    }
  } catch(e){
    // Silent; guard is best-effort
  }
})();

module.exports = {};