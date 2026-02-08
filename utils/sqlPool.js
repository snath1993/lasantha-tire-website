// Shared MSSQL pool helper to avoid frequent connect/close cycles.
// Usage in index.js or jobs: const { getPool } = require('./utils/sqlPool');
// const pool = await getPool(); const result = await pool.request().query('SELECT 1');

const { sql, config: baseConfig } = require('../sqlConfig');
let poolPromise = null;
let lastInitError = null;

async function getPool(){
  if(poolPromise) return poolPromise;
  poolPromise = new Promise(async (resolve, reject) => {
    try {
      const p = new sql.ConnectionPool(baseConfig);
      const closeListener = () => { poolPromise = null; };
      p.on('close', closeListener);
      await p.connect();
      resolve(p);
    } catch(e){
      lastInitError = e;
      poolPromise = null;
      reject(e);
    }
  });
  return poolPromise;
}

function getLastInitError(){ return lastInitError; }

module.exports = { getPool, getLastInitError };
