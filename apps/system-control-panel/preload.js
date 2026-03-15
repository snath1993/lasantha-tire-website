/**
 * Preload Script — Secure IPC bridge between Electron main and renderer
 * v2.0 — Includes Bot Swap + Library Update + Hard Reset APIs
 */
const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
    // ── Window controls ──
    minimize: () => ipcRenderer.invoke('window-minimize'),
    maximize: () => ipcRenderer.invoke('window-maximize'),
    close:    () => ipcRenderer.invoke('window-close'),

    // ── Overview ──
    getOverview: () => ipcRenderer.invoke('get-overview'),

    // ── Services ──
    getServices:   () => ipcRenderer.invoke('get-services'),
    serviceAction: (name, action) => ipcRenderer.invoke('service-action', { name, action }),

    // ── WhatsApp ──
    getWhatsAppStatus:  () => ipcRenderer.invoke('get-whatsapp-status'),
    getQRCode:          () => ipcRenderer.invoke('get-qr-code'),
    reconnectWhatsApp:  () => ipcRenderer.invoke('reconnect-whatsapp'),
    hardResetWhatsApp:  () => ipcRenderer.invoke('hard-reset-whatsapp'),
    sendTestMessage:    (number, message) => ipcRenderer.invoke('send-test-message', { number, message }),

    // ── Bot Swap ──
    getActiveBot: () => ipcRenderer.invoke('get-active-bot'),
    botSwap:      () => ipcRenderer.invoke('bot-swap'),

    // ── Jobs ──
    getJobs:       () => ipcRenderer.invoke('get-jobs'),
    triggerJob:    (name) => ipcRenderer.invoke('trigger-job', name),
    getJobHistory: (name) => ipcRenderer.invoke('get-job-history', name),

    // ── Database ──
    getDbStatus:    () => ipcRenderer.invoke('get-db-status'),
    triggerBackup:  () => ipcRenderer.invoke('trigger-backup'),
    getBackupFiles: () => ipcRenderer.invoke('get-backup-files'),

    // ── Logs ──
    getLogs:       (opts) => ipcRenderer.invoke('get-logs', opts),
    flushPm2Logs:  () => ipcRenderer.invoke('flush-pm2-logs'),

    // ── Configuration ──
    getConfigList: () => ipcRenderer.invoke('get-config-list'),
    getConfigFile: (name) => ipcRenderer.invoke('get-config-file', name),
    saveConfigFile: (name, content) => ipcRenderer.invoke('save-config-file', { name, content }),

    // ── Library Update ──
    getLibraryInfo:     () => ipcRenderer.invoke('get-library-info'),
    safeLibraryUpdate:  () => ipcRenderer.invoke('safe-library-update'),
    rollbackLibrary:    () => ipcRenderer.invoke('rollback-library'),

    // ── System ──
    getSystemInfo:       () => ipcRenderer.invoke('get-system-info'),
    getDiskSpace:        () => ipcRenderer.invoke('get-disk-space'),
    openFolder:          (p) => ipcRenderer.invoke('open-folder', p),
    openDashboardWeb:    () => ipcRenderer.invoke('open-dashboard-web'),
    restartAllTunnels:   () => ipcRenderer.invoke('restart-all-tunnels'),

    // ── Settings ──
    getSettings:  () => ipcRenderer.invoke('get-settings'),
    saveSettings: (s) => ipcRenderer.invoke('save-settings', s),

    // ── Progress events from main process ──
    onUpdateProgress: (callback) => {
        ipcRenderer.on('update-progress', (_, data) => callback(data));
    },
});
