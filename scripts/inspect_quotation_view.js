/*
  Inspect quotation view for a given ref.

  Usage:
    node scripts/inspect_quotation_view.js 000864

  Env override:
    QUO_REF=000864 node scripts/inspect_quotation_view.js
*/

const { sql, config } = require('../sqlConfig');
const fs = require('fs');
const path = require('path');

function getRef() {
  const arg = process.argv[2];
  return (process.env.QUO_REF || arg || '').trim();
}

async function main() {
  const ref = getRef();
  if (!ref) {
    console.error('Missing ref. Usage: node scripts/inspect_quotation_view.js 000864');
    process.exit(2);
  }

  const viewDisplay = '[View_Quotation_WhatsApp]';
  const tableNameForInfoSchema = 'View_Quotation_WhatsApp';

  const outDir = path.join(__dirname, 'out', 'digital-invoice');
  fs.mkdirSync(outDir, { recursive: true });
  const outColsPath = path.join(outDir, `QUOTATION_VIEW_COLUMNS.json`);
  const outRowsPath = path.join(outDir, `QUOTATION_VIEW_${String(ref).replace(/[\\/\:*?"<>|]/g, '-')}.json`);

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

    console.log(`\nReading rows for SalesOrderNo = ${ref} ...`);
    const rows = await pool.request()
      .input('Ref', sql.VarChar, ref)
      .query(`SELECT * FROM ${viewDisplay} WHERE [SalesOrderNo] = @Ref`);

    console.log(`Row count: ${rows.recordset.length}`);

    // Print full JSON, but avoid exploding terminal if a column is huge
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
      const pick = (k) => (Object.prototype.hasOwnProperty.call(first, k) ? first[k] : undefined);
      const previewKeys = [
        'SalesOrderNo',
        'Date',
        'CusName',
        'ad1',
        'ad2',
        'VehicleNo',
        'SalesRep',
        'ItemID',
        'Description',
        'Quantity',
        'UnitPrice',
        'Amount',
        'Brand',
        'Country',
        'Warranty'
      ];
      const preview = {};
      for (const k of previewKeys) {
        const v = pick(k);
        if (v !== undefined) preview[k] = v;
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
