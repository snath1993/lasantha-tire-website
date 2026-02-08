const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const { spawn } = require('child_process');

let mainWindow;

function getStartUrl() {
  if (process.env.ELECTRON_START_URL) return process.env.ELECTRON_START_URL;
  // In production we typically load the Next.js server URL (or a static export).
  // For now, default to localhost dev port.
  return 'http://127.0.0.1:3070';
}

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 800,
    autoHideMenuBar: true,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  mainWindow.loadURL(getStartUrl());

  mainWindow.on('closed', function () {
    mainWindow = null;
  });
}

app.on('ready', createWindow);

app.on('window-all-closed', function () {
  if (process.platform !== 'darwin') app.quit();
});

app.on('activate', function () {
  if (mainWindow === null) createWindow();
});

// IPC: ask Windows-side Crystal bridge to print an invoice via legacy .rpt.
// This is a stub until the C# bridge executable is implemented.
ipcMain.handle('crystal:printInvoice', async (_event, payload) => {
  const invoiceNo = String(payload?.invoiceNo ?? '').trim();
  const reportPath = String(payload?.reportPath ?? '').trim();
  if (!invoiceNo) return { ok: false, error: 'Missing invoiceNo' };

  const exePath = path.join(__dirname, 'bridge', 'CrystalPrintBridge.exe');

  // If the bridge isn't present yet, return a clear message.
  // (We will generate this exe from a C# project later.)
  if (!require('fs').existsSync(exePath)) {
    return { ok: false, error: 'CrystalPrintBridge.exe not found', exePath };
  }

  return await new Promise((resolve) => {
    const args = [
      '--invoiceNo',
      invoiceNo,
    ];
    if (reportPath) {
      args.push('--reportPath', reportPath);
    }

    const child = spawn(exePath, args, {
      windowsHide: true,
    });

    let stdout = '';
    let stderr = '';
    child.stdout.on('data', (d) => (stdout += d.toString()));
    child.stderr.on('data', (d) => (stderr += d.toString()));
    child.on('close', (code) => {
      resolve({ ok: code === 0, code, stdout, stderr });
    });
  });
});
