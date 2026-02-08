/*
  Generate VAT & NON-VAT sample PDFs

  - Picks 1 VAT + 1 NON-VAT quotation (from [View_Quotation_WhatsApp])
  - Picks 1 VAT + 1 NON-VAT invoice (from [View_Sales report whatsapp])
  - Uses digital-invoice/dbLayer AUTO fetch + pdfGenerator

  Usage:
    node scripts/generate_vat_nonvat_samples.js

  Optional env overrides:
    VAT_INVOICE_REF=... NONVAT_INVOICE_REF=... VAT_QUOTATION_REF=... NONVAT_QUOTATION_REF=... node scripts/generate_vat_nonvat_samples.js
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

function quoteCol(colName) {
  const s = String(colName || '').trim();
  if (!s) return s;
  if (s.startsWith('[') && s.endsWith(']')) return s;
  return `[${s.replace(/]/g, '')}]`;
}

async function pickCandidateRefs(pool, query) {
  const r = await pool.request().query(query);
  return (r.recordset || []).map((x) => String(x.RefNo || '')).filter(Boolean);
}

async function pickInvoiceRef(pool, { wantVat }) {
  const refCol = quoteCol(diConfig.DB.COLS.INVOICE.REF_NO);
  const dateCol = quoteCol(diConfig.DB.COLS.INVOICE.DATE);
  const vatCol = quoteCol(diConfig.DB.COLS.INVOICE.VAT_TOTAL);
  const taxNoCol = quoteCol(diConfig.DB.COLS.INVOICE.TAX_INVOICE_NO);

  const candidates = await pickCandidateRefs(
    pool,
    `
      SELECT TOP (30) RefNo
      FROM (
        SELECT
          ${refCol} AS RefNo,
          MAX(${dateCol}) AS MaxDate,
          MAX(CASE WHEN IsVATInvoice = 1 THEN 1 ELSE 0 END) AS IsVat,
          MAX(ISNULL(${vatCol}, 0)) AS VatTotal,
          MAX(CASE WHEN ISNULL(LTRIM(RTRIM(${taxNoCol})), '') <> '' THEN 1 ELSE 0 END) AS HasTaxNo
        FROM ${diConfig.DB.VIEW_INVOICE}
        GROUP BY ${refCol}
      ) t
      WHERE ${wantVat ? '(t.IsVat = 1 OR t.VatTotal > 0 OR t.HasTaxNo = 1)' : '(t.IsVat = 0 AND t.VatTotal <= 0 AND t.HasTaxNo = 0)'}
      ORDER BY t.MaxDate DESC
    `.trim()
  );

  return candidates;
}

async function pickQuotationRef(pool, { wantVat }) {
  const refCol = quoteCol(diConfig.DB.COLS.QUOTATION.REF_NO);
  const dateCol = quoteCol(diConfig.DB.COLS.QUOTATION.DATE);
  const vatCol = quoteCol(diConfig.DB.COLS.QUOTATION.VAT_TOTAL);

  const candidates = await pickCandidateRefs(
    pool,
    `
      SELECT TOP (30) RefNo
      FROM (
        SELECT
          ${refCol} AS RefNo,
          MAX(${dateCol}) AS MaxDate,
          MAX(ISNULL(${vatCol}, 0)) AS VatTotal
        FROM ${diConfig.DB.VIEW_QUOTATION}
        GROUP BY ${refCol}
      ) t
      WHERE ${wantVat ? '(t.VatTotal > 0)' : '(t.VatTotal <= 0)'}
      ORDER BY t.MaxDate DESC
    `.trim()
  );

  return candidates;
}

async function pickFirstMatching(db, candidates, predicate) {
  for (const ref of candidates) {
    try {
      const data = await db.fetchDocumentData('AUTO', ref);
      if (predicate(data)) return { ref, data };
    } catch {
      // keep trying
    }
  }
  return null;
}

async function generateOne(outDir, label, ref, data) {
  const jsonPath = path.join(outDir, `${label}_${safeFilePart(ref)}.json`);
  writeFileWithRetry(jsonPath, Buffer.from(JSON.stringify(data, null, 2)));

  const pdf = await pdfGenerator.generate(data);
  const pdfPath = path.join(outDir, `${label}_${safeFilePart(ref)}.pdf`);
  const written = writeFileWithRetry(pdfPath, pdf);

  return { jsonPath, pdfPath: written };
}

async function main() {
  const outDir = path.join(__dirname, 'out', 'digital-invoice', `samples_${timestampPart()}`);
  fs.mkdirSync(outDir, { recursive: true });

  console.log('Connecting to database...');
  const pool = await sql.connect(sqlConfig);

  try {
    const db = new DbLayer(pool);

    // Env overrides (if provided)
    const envVatInvoice = process.env.VAT_INVOICE_REF;
    const envNonVatInvoice = process.env.NONVAT_INVOICE_REF;
    const envVatQuotation = process.env.VAT_QUOTATION_REF;
    const envNonVatQuotation = process.env.NONVAT_QUOTATION_REF;

    // 1) VAT invoice
    const vatInvoice = envVatInvoice
      ? { ref: envVatInvoice, data: await db.fetchDocumentData('AUTO', envVatInvoice) }
      : await pickFirstMatching(db, await pickInvoiceRef(pool, { wantVat: true }), (d) => d?.metadata?.type === 'INVOICE' && d?.metadata?.isVatInvoice === true);

    // 2) NON-VAT invoice
    const nonVatInvoice = envNonVatInvoice
      ? { ref: envNonVatInvoice, data: await db.fetchDocumentData('AUTO', envNonVatInvoice) }
      : await pickFirstMatching(db, await pickInvoiceRef(pool, { wantVat: false }), (d) => d?.metadata?.type === 'INVOICE' && d?.metadata?.isVatInvoice !== true);

    // 3) VAT quotation (VATAmount > 0)
    const vatQuotation = envVatQuotation
      ? { ref: envVatQuotation, data: await db.fetchDocumentData('AUTO', envVatQuotation) }
      : await pickFirstMatching(db, await pickQuotationRef(pool, { wantVat: true }), (d) => d?.metadata?.type === 'QUOTATION' && (Number(String(d?.totals?.vatTotal || '').replace(/,/g, '')) > 0));

    // 4) NON-VAT quotation (VATAmount <= 0)
    const nonVatQuotation = envNonVatQuotation
      ? { ref: envNonVatQuotation, data: await db.fetchDocumentData('AUTO', envNonVatQuotation) }
      : await pickFirstMatching(db, await pickQuotationRef(pool, { wantVat: false }), (d) => d?.metadata?.type === 'QUOTATION' && !(Number(String(d?.totals?.vatTotal || '').replace(/,/g, '')) > 0));

    const missing = [];
    if (!vatInvoice) missing.push('VAT invoice');
    if (!nonVatInvoice) missing.push('NON-VAT invoice');
    if (!vatQuotation) missing.push('VAT quotation');
    if (!nonVatQuotation) missing.push('NON-VAT quotation');

    if (missing.length) {
      throw new Error(`Could not auto-pick: ${missing.join(', ')}. Try providing env overrides.`);
    }

    console.log('\nSelected refs:');
    console.log('  VAT Invoice     :', vatInvoice.ref);
    console.log('  NON-VAT Invoice :', nonVatInvoice.ref);
    console.log('  VAT Quotation   :', vatQuotation.ref);
    console.log('  NON-VAT Quotation:', nonVatQuotation.ref);

    console.log('\nGenerating PDFs...');
    const out1 = await generateOne(outDir, 'INVOICE_VAT', vatInvoice.ref, vatInvoice.data);
    const out2 = await generateOne(outDir, 'INVOICE_NONVAT', nonVatInvoice.ref, nonVatInvoice.data);
    const out3 = await generateOne(outDir, 'QUOTATION_VAT', vatQuotation.ref, vatQuotation.data);
    const out4 = await generateOne(outDir, 'QUOTATION_NONVAT', nonVatQuotation.ref, nonVatQuotation.data);

    console.log('\nWrote:');
    console.log('  ', out1.pdfPath);
    console.log('  ', out2.pdfPath);
    console.log('  ', out3.pdfPath);
    console.log('  ', out4.pdfPath);

    console.log(`\nOutput folder: ${outDir}`);
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
  console.error('Sample generation failed:', err && err.message ? err.message : err);
  process.exit(1);
});
