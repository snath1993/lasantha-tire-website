const sql = require('mssql');
const { config } = require('../sqlConfig'); 
// Note: sqlConfig usually exports 'sqlConnectionPool' or just 'config'. 
// Let's check sqlConfig.js export again. 

// Based on read of sqlConfig.js, it seems to just set up variables but I missed the actual export.
// Let me quickly re-read the END of sqlConfig.js to see what it exports.
