const fs = require('fs');
const path = require('path');
const moment = require('moment');

const SENT_FILE = path.join(__dirname, '..', 'sent_invoices.txt');
const BACKUP_DIR = path.join(__dirname, '..', 'backup');

function ensureFiles() {
    if (!fs.existsSync(SENT_FILE)) fs.writeFileSync(SENT_FILE, '# Daily Sales Report - Sent Invoice Numbers\n', 'utf8');
    if (!fs.existsSync(BACKUP_DIR)) fs.mkdirSync(BACKUP_DIR, { recursive: true });
}

function listSentInvoices() {
    try {
        ensureFiles();
        const content = fs.readFileSync(SENT_FILE, 'utf8');
        const invoices = content.split('\n').map(l => l.trim()).filter(l => l && !l.startsWith('#'));
        return invoices;
    } catch (err) {
        console.error('listSentInvoices error:', err.message);
        return [];
    }
}

function backupSentFile() {
    try {
        ensureFiles();
        const ts = moment().format('YYYYMMDD_HHmmss');
        const backupPath = path.join(BACKUP_DIR, `sent_invoices_${ts}.txt`);
        fs.copyFileSync(SENT_FILE, backupPath);
        return backupPath;
    } catch (err) {
        console.error('backupSentFile error:', err.message);
        return null;
    }
}

function removeInvoice(invoiceNo) {
    try {
        ensureFiles();
        const invoices = listSentInvoices();
        const normalized = invoiceNo && String(invoiceNo).trim();
        if (!normalized) return { ok: false, message: 'Invalid invoice number' };
        const idx = invoices.findIndex(i => i.toUpperCase() === normalized.toUpperCase());
        if (idx === -1) return { ok: false, message: 'Invoice not found' };
        // backup before modifying
        const backup = backupSentFile();
        invoices.splice(idx, 1);
        const header = '# Daily Sales Report - Sent Invoice Numbers\n';
        fs.writeFileSync(SENT_FILE, header + '\n' + invoices.join('\n') + '\n', 'utf8');
        return { ok: true, backup };
    } catch (err) {
        console.error('removeInvoice error:', err.message);
        return { ok: false, message: err.message };
    }
}

function clearSentInvoices() {
    try {
        ensureFiles();
        // create backup
        const backup = backupSentFile();
        // write header only
        fs.writeFileSync(SENT_FILE, '# Daily Sales Report - Sent Invoice Numbers\n', 'utf8');
        return { ok: true, backup };
    } catch (err) {
        console.error('clearSentInvoices error:', err.message);
        return { ok: false, message: err.message };
    }
}

module.exports = {
    listSentInvoices,
    removeInvoice,
    clearSentInvoices,
    backupSentFile
};
