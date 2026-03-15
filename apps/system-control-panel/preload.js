/**
 * Preload Script — Secure bridge between Electron main and renderer
 */
const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
    // Window controls
    minimize: () => ipcRenderer.invoke('window-minimize'),
    maximize: () => ipcRenderer.invoke('window-maximize'),
    close: () => ipcRenderer.invoke('window-close'),

    // Overview
    getOverview: () => ipcRenderer.invoke('get-overview'),

    // Services
    getServices: () => ipcRenderer.invoke('get-services'),
    serviceAction: (name, action) => ipcRenderer.invoke('service-action', { name, action }),

    // WhatsApp
    getWhatsAppStatus: () => ipcRenderer.invoke('get-whatsapp-status'),
    getQRCode: () => ipcRenderer.invoke('get-qr-code'),
    reconnectWhatsApp: () => ipcRenderer.invoke('reconnect-whatsapp'),

    // Jobs
    getJobs: () => ipcRenderer.invoke('get-jobs'),
    triggerJob: (jobName) => ipcRenderer.invoke('trigger-job', jobName),
    getJobHistory: (jobName) => ipcRenderer.invoke('get-job-history', jobName),

    // Database
    getDbStatus: () => ipcRenderer.invoke('get-db-status'),
    triggerBackup: () => ipcRenderer.invoke('trigger-backup'),
    getBackupFiles: () => ipcRenderer.invoke('get-backup-files'),

    // Logs
    getLogs: (opts) => ipcRenderer.invoke('get-logs', opts),

    // Configuration
    getConfigList: () => ipcRenderer.invoke('get-config-list'),
    getConfigFile: (name) => ipcRenderer.invoke('get-config-file', name),
    saveConfigFile: (name, content) => ipcRenderer.invoke('save-config-file', { name, content }),

    // System
    getSystemInfo: () => ipcRenderer.invoke('get-system-info'),
    openFolder: (p) => ipcRenderer.invoke('open-folder', p),
    openDashboardWeb: () => ipcRenderer.invoke('open-dashboard-web'),
    restartAllTunnels: () => ipcRenderer.invoke('restart-all-tunnels'),
    sendTestMessage: (number, message) => ipcRenderer.invoke('send-test-message', { number, message }),

    // Settings
    getSettings: () => ipcRenderer.invoke('get-settings'),
    saveSettings: (s) => ipcRenderer.invoke('save-settings', s),
});
