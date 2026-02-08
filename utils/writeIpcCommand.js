// Helper script: node utils/writeIpcCommand.js action [key=value ...]
// Example: node utils/writeIpcCommand.js trigger_sales_report
//          node utils/writeIpcCommand.js send_2fa_code number=0771222509 code=123456
const { writeCommandAtomic } = require('./ipcCommandWatcher');

function parseArgs() {
  const [, , action, ...rest] = process.argv;
  if (!action) {
    console.error('Usage: node utils/writeIpcCommand.js <action> [key=value]');
    process.exit(1);
  }
  const extra = {};
  rest.forEach(p => {
    const eq = p.indexOf('=');
    if (eq > 0) extra[p.slice(0, eq)] = p.slice(eq + 1);
  });
  return { action, extra };
}

function main() {
  const { action, extra } = parseArgs();
  const cmd = {
    id: action + '-' + Date.now() + '-' + Math.random().toString(36).slice(2),
    action,
    timestamp: new Date().toISOString(),
    ...extra
  };
  try {
    writeCommandAtomic(cmd);
    console.log('IPC command written:', cmd);
  } catch (e) {
    console.error('Failed to write command:', e.message);
    process.exit(2);
  }
}

main();