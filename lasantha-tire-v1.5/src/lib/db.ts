import sql from 'mssql';

const sqlConfig: sql.config = {
  user: process.env.SQL_USER || process.env.DB_USER || '',
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
  database: process.env.SQL_DATABASE || process.env.DB_NAME || '',
  server: process.env.SQL_SERVER || process.env.DB_SERVER || '',
  port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000
  },
  options: {
    encrypt: false, // Set to true for Azure
    trustServerCertificate: true, // Change to false for production with valid certs
    enableArithAbort: true
  }
};

// Validate config
if (!sqlConfig.user || !sqlConfig.server || !sqlConfig.database) {
  console.error('❌ Database configuration missing. Please check .env file.');
}

// Singleton pool
let pool: sql.ConnectionPool | null = null;

export async function getPool(): Promise<sql.ConnectionPool> {
  if (pool) {
    if (pool.connected) {
      return pool;
    }
    // If pool exists but not connected, try to connect
    try {
      await pool.connect();
      return pool;
    } catch (err) {
      console.error('Failed to reconnect to existing pool:', err);
      pool = null; // Reset pool to try creating a new one
    }
  }

  try {
    pool = new sql.ConnectionPool(sqlConfig);
    await pool.connect();
    
    pool.on('error', (err) => {
      console.error('SQL Pool Error:', err);
    });

    return pool;
  } catch (err) {
    console.error('Failed to create SQL pool:', err);
    throw err;
  }
}

export { sql };
