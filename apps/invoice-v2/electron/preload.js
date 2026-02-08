const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('invoiceV2', {
  crystalPrintInvoice: (payload) => ipcRenderer.invoke('crystal:printInvoice', payload),
});
