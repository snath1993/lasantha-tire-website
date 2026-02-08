/*
  Digital Invoice/Quotation smoke test

  - Connects to SQL Server using sqlConfig.js
  - Finds a recent INVOICE ref and a recent QUOTATION ref
  - Runs AUTO doc-type detection via digital-invoice/dbLayer
  - Generates PDFs for both and writes them under scripts/out/digital-invoice/

  Usage:
    node scripts/digital_invoice_smoke.js

  Optional env overrides:
    INVOICE_REF=NLT00006299 QUOTATION_REF=SO000123 node scripts/digital_invoice_smoke.js
*/

const fs = require('fs');
const path = require('path');

const { sql, config: sqlConfig } = require('../sqlConfig');
const DbLayer = require('../digital-invoice/dbLayer');
const pdfGenerator = require('../digital-invoice/pdfGenerator');
const diConfig = require('../digital-invoice/config');

function safeFilePart(value) {
  return String(value || '')
    .trim()
    .replace(/[\\/\:*?"<>|]/g, '-')
    .replace(/\s+/g, '_')
    .slice(0, 120);
}

function timestampPart() {
  const d = new Date();
  const pad = (n) => String(n).padStart(2, '0');
  return (
    d.getFullYear() +
    pad(d.getMonth() + 1) +
    pad(d.getDate()) +
    '_' +
    pad(d.getHours()) +
    pad(d.getMinutes()) +
    pad(d.getSeconds())
  );
}

function writeFileWithRetry(filePath, buffer) {
  try {
    fs.writeFileSync(filePath, buffer);
    return filePath;
  } catch (e) {
    if (e && (e.code === 'EBUSY' || e.code === 'EPERM' || e.code === 'EACCES')) {
      const ext = path.extname(filePath);
      const base = filePath.slice(0, -ext.length);
      const alt = `${base}_${timestampPart()}${ext}`;
      fs.writeFileSync(alt, buffer);
      return alt;
    }
    throw e;
  }
}

async function pickLatestRef(pool, { viewName, refCol, dateCol }) {
  const refColQuoted = refCol.startsWith('[') ? refCol : `[${refCol.replace(/]/g, '')}]`;
  const dateColQuoted = dateCol.startsWith('[') ? dateCol : `[${dateCol.replace(/]/g, '')}]`;

  const r = await pool
    .request()
    .query(`SELECT TOP (1) ${refColQuoted} AS RefNo FROM ${viewName} ORDER BY ${dateColQuoted} DESC`);

  return (r.recordset && r.recordset[0] && r.recordset[0].RefNo) ? String(r.recordset[0].RefNo) : '';
}

async function main() {
  const outDir = path.join(__dirname, 'out', 'digital-invoice');
  fs.mkdirSync(outDir, { recursive: true });

  console.log('Connecting to database for smoke test...');
  const pool = await sql.connect(sqlConfig);

  try {
    const db = new DbLayer(pool);

    const invoiceRef = process.env.INVOICE_REF || await pickLatestRef(pool, {
      viewName: diConfig.DB.VIEW_INVOICE,
      refCol: diConfig.DB.COLS.INVOICE.REF_NO,
      dateCol: diConfig.DB.COLS.INVOICE.DATE
    });

    const quotationRef = process.env.QUOTATION_REF || await pickLatestRef(pool, {
      viewName: diConfig.DB.VIEW_QUOTATION,
      refCol: diConfig.DB.COLS.QUOTATION.REF_NO,
      dateCol: diConfig.DB.COLS.QUOTATION.DATE
    });

    if (!invoiceRef) {
      throw new Error('Could not find a recent invoice ref in invoice view. Set INVOICE_REF to test with a known ref.');
    }

    if (!quotationRef) {
      throw new Error('Could not find a recent quotation ref in quotation view. Set QUOTATION_REF to test with a known ref.');
    }

    console.log('Invoice Ref:', invoiceRef);
    console.log('Quotation Ref:', quotationRef);

    // 1) AUTO detect + data fetch
    console.log('\n--- Fetching (AUTO) invoice data ---');
    const invoiceData = await db.fetchDocumentData('AUTO', invoiceRef);
    console.log('Detected Type:', invoiceData && invoiceData.metadata ? invoiceData.metadata.type : '(unknown)');
    console.log('Customer:', invoiceData && invoiceData.invoice ? invoiceData.invoice.customerName : '(unknown)');
    console.log('Items:', Array.isArray(invoiceData.items) ? invoiceData.items.length : 0);

    console.log('\n--- Fetching (AUTO) quotation data ---');
    const quotationData = await db.fetchDocumentData('AUTO', quotationRef);
    console.log('Detected Type:', quotationData && quotationData.metadata ? quotationData.metadata.type : '(unknown)');
    console.log('Customer:', quotationData && quotationData.invoice ? quotationData.invoice.customerName : '(unknown)');
    console.log('Items:', Array.isArray(quotationData.items) ? quotationData.items.length : 0);

    // 2) Write JSON snapshots (handy for debugging template placeholders)
    const invoiceJsonPath = path.join(outDir, `INVOICE_${safeFilePart(invoiceRef)}.json`);
    const quotationJsonPath = path.join(outDir, `QUOTATION_${safeFilePart(quotationRef)}.json`);
    writeFileWithRetry(invoiceJsonPath, Buffer.from(JSON.stringify(invoiceData, null, 2)));
    writeFileWithRetry(quotationJsonPath, Buffer.from(JSON.stringify(quotationData, null, 2)));

    // 3) Generate PDFs
    console.log('\n--- Generating invoice PDF ---');
    const invoicePdf = await pdfGenerator.generate(invoiceData);
    const invoicePdfPath = path.join(outDir, `INVOICE_${safeFilePart(invoiceRef)}.pdf`);
    const invoicePdfWritten = writeFileWithRetry(invoicePdfPath, invoicePdf);

    console.log('Wrote:', invoicePdfWritten);

    console.log('\n--- Generating quotation PDF ---');
    const quotationPdf = await pdfGenerator.generate(quotationData);
    const quotationPdfPath = path.join(outDir, `QUOTATION_${safeFilePart(quotationRef)}.pdf`);
    const quotationPdfWritten = writeFileWithRetry(quotationPdfPath, quotationPdf);

    console.log('Wrote:', quotationPdfWritten);

    console.log('\n✅ Smoke test complete.');
  } finally {
    try { await pool.close(); } catch {}
    try {
      if (pdfGenerator && pdfGenerator.browser) {
        await pdfGenerator.browser.close();
        pdfGenerator.browser = null;
      }
    } catch {}
    try { await sql.close(); } catch {}
  }
}

main().catch((err) => {
  console.error('Smoke test failed:', err && err.message ? err.message : err);
  process.exit(1);
});
