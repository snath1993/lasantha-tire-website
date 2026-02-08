/*
  Inspect invoice view for a given invoice no.

  Usage:
    node scripts/inspect_invoice_view.js NLT00006314

  Env override:
    INV_REF=NLT00006314 node scripts/inspect_invoice_view.js
*/

const { sql, config } = require('../sqlConfig');
const fs = require('fs');
const path = require('path');

function getRef() {
  const arg = process.argv[2];
  return (process.env.INV_REF || arg || '').trim();
}

async function main() {
  const ref = getRef();
  if (!ref) {
    console.error('Missing ref. Usage: node scripts/inspect_invoice_view.js NLT00006314');
    process.exit(2);
  }

  const viewDisplay = '[View_Sales report whatsapp]';
  const tableNameForInfoSchema = 'View_Sales report whatsapp';

  const outDir = path.join(__dirname, 'out', 'digital-invoice');
  fs.mkdirSync(outDir, { recursive: true });
  const outColsPath = path.join(outDir, `INVOICE_VIEW_COLUMNS.json`);
  const outRowsPath = path.join(outDir, `INVOICE_VIEW_${String(ref).replace(/[\\/\:*?"<>|]/g, '-')}.json`);

  console.log('Connecting...');
  const pool = await sql.connect(config);
  try {
    console.log('\nReading view columns...');
    const cols = await pool.request().query(
      `SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE\n` +
      `FROM INFORMATION_SCHEMA.COLUMNS\n` +
      `WHERE TABLE_NAME = '${tableNameForInfoSchema}'\n` +
      `ORDER BY ORDINAL_POSITION`
    );
    fs.writeFileSync(outColsPath, JSON.stringify(cols.recordset, null, 2));
    console.log(`Columns: ${cols.recordset.length} (saved: ${outColsPath})`);

    // Auto-detect which column contains this invoice reference.
    let refColumn = null;
    const candidates = cols.recordset
      .filter((c) => String(c.DATA_TYPE || '').toLowerCase().includes('char'))
      .map((c) => c.COLUMN_NAME);

    for (const col of candidates) {
      try {
        const r = await pool.request()
          .input('Ref', sql.VarChar, ref)
          .query(`SELECT TOP (1) 1 AS Found FROM ${viewDisplay} WHERE [${String(col).replace(/]/g, '')}] = @Ref`);
        if (r.recordset && r.recordset.length) {
          refColumn = col;
          break;
        }
      } catch {
        // ignore and continue
      }
    }

    if (!refColumn) {
      console.log(`\nCould not find a column that equals '${ref}'. Writing TOP 5 rows for manual inspection...`);
      const top = await pool.request().query(`SELECT TOP 5 * FROM ${viewDisplay}`);
      fs.writeFileSync(outRowsPath, JSON.stringify(top.recordset, null, 2));
      console.log(`Saved rows: ${outRowsPath}`);
      process.exit(3);
    }

    console.log(`\nDetected ref column: ${refColumn}`);
    console.log(`Reading rows where [${refColumn}] = ${ref} ...`);
    const rows = await pool.request()
      .input('Ref', sql.VarChar, ref)
      .query(`SELECT * FROM ${viewDisplay} WHERE [${String(refColumn).replace(/]/g, '')}] = @Ref`);

    console.log(`Row count: ${rows.recordset.length}`);

    const sanitized = rows.recordset.map((r) => {
      const out = {};
      for (const [k, v] of Object.entries(r)) {
        if (typeof v === 'string' && v.length > 500) out[k] = v.slice(0, 500) + '...<truncated>';
        else out[k] = v;
      }
      return out;
    });

    fs.writeFileSync(outRowsPath, JSON.stringify(sanitized, null, 2));
    console.log(`Saved rows: ${outRowsPath}`);

    if (sanitized.length) {
      const first = sanitized[0];
      const previewKeys = [
        'InvoiceNo',
        'InvoiceDate',
        'CustomerName',
        'VehicleNo',
        'Mileage',
        'SalesRep',
        'PaymentM',
        'ItemID',
        'Description',
        'Qty',
        'UnitPrice',
        'Amount'
      ];
      const preview = {};
      for (const k of previewKeys) {
        if (Object.prototype.hasOwnProperty.call(first, k)) preview[k] = first[k];
      }
      console.log('\nPreview (first row selected fields):');
      console.log(JSON.stringify(preview, null, 2));
    }
  } finally {
    try { await pool.close(); } catch {}
    try { await sql.close(); } catch {}
  }
}

main().catch((err) => {
  console.error('Failed:', err && err.message ? err.message : err);
  process.exit(1);
});
