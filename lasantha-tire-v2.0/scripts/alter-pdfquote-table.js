const sql = require('mssql');
require('dotenv').config({ path: '.env.local' });

const config = {
  user: process.env.SQL_USER || process.env.DB_USER,
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
  server: (process.env.SQL_SERVER || process.env.DB_SERVER || '').split('\\')[0],
  database: 'WhatsAppAI',
  options: {
    instanceName: (process.env.SQL_SERVER || process.env.DB_SERVER || '').split('\\')[1],
    encrypt: false,
    trustServerCertificate: true,
  },
};

async function run() {
  try {
    await sql.connect(config);

    const q = `
      IF COL_LENGTH('pdfquote', 'meta') IS NULL
      BEGIN
        ALTER TABLE pdfquote ADD meta NVARCHAR(MAX) NULL;
        PRINT 'Added column meta';
      END

      IF COL_LENGTH('pdfquote', 'updated_at') IS NULL
      BEGIN
        ALTER TABLE pdfquote ADD updated_at DATETIME NULL;
        PRINT 'Added column updated_at';
      END

      -- Backfill updated_at for existing rows (optional)
      UPDATE pdfquote
      SET updated_at = created_at
      WHERE updated_at IS NULL;
    `;

    await sql.query(q);
    console.log('pdfquote schema updated successfully.');
  } catch (err) {
    console.error('Schema update failed:', err);
    process.exitCode = 1;
  } finally {
    try { await sql.close(); } catch {}
  }
}

run();
