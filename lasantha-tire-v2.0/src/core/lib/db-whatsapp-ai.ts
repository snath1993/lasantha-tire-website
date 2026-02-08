import sql from 'mssql';

const sqlConfig: sql.config = {
  user: process.env.SQL_USER || process.env.DB_USER || '',
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
  database: 'WhatsAppAI', // Explicitly using WhatsAppAI
  server: process.env.SQL_SERVER || process.env.DB_SERVER || '',
  port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000
  },
  options: {
    encrypt: false,
    trustServerCertificate: true,
    enableArithAbort: true
  }
};

let pool: sql.ConnectionPool | null = null;

export async function getWhatsAppAIPool(): Promise<sql.ConnectionPool> {
  if (pool) {
    if (pool.connected) {
      return pool;
    }
    try {
      await pool.connect();
      return pool;
    } catch (err) {
      console.error('Failed to reconnect to WhatsAppAI pool:', err);
      pool = null;
    }
  }

  try {
    pool = new sql.ConnectionPool(sqlConfig);
    await pool.connect();
    return pool;
  } catch (err) {
    console.error('Failed to connect to WhatsAppAI database:', err);
    throw err;
  }
}
