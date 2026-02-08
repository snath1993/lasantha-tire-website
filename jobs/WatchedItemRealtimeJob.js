// jobs/WatchedItemRealtimeJob.js
// Multi-pattern polling for watched item sales; sends WhatsApp alerts for new invoices.
const moment = require('moment');
const fs = require('fs');
const path = require('path');
const { loadConfig, loadState, saveState } = require('../utils/watchedItemConfig');
const { updateJobStatus } = require('../utils/jobStatus');

module.exports = async function WatchedItemRealtimeJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, options = {}) {
  // console.log('--- DEBUG: WatchedItemRealtimeJob VERSION 5.0 (DIAGNOSTIC) STARTING ---');
  
  const mainPool = options.mainPool;
  
  if (!mainPool && (!sqlConfig || !sqlConfig.server)) {
    const error = 'Invalid SQL configuration provided to WatchedItemRealtimeJob - mainPool or sqlConfig.server required';
    logAndSave(error);
    throw new Error(error);
  }

  const cfg = loadConfig();
  const patterns = (cfg.patterns || []).filter(p => p && p.trim());
  if (patterns.length === 0) {
      logAndSave('WatchedItemRealtimeJob: No patterns configured');
      return; 
  }
  
  const minQty = cfg.minQty || 1;
  const state = loadState();
  patterns.forEach(p => { if (!state.patterns[p]) state.patterns[p] = { lastNotified: [], totalQtyToday: 0, lastUpdate: null }; });
  
  let today = moment().format('YYYY-MM-DD');
  const started = new Date();
  let notifiedCount = 0;
  let pool = mainPool; 
  let poolCreatedHere = false;

  try {
    if (!pool) {
      if (!sql.connected) {
        try {
          pool = await sql.connect(sqlConfig);
          poolCreatedHere = true;
        } catch (connErr) {
          logAndSave(`SQL connection error: ${connErr.message}`);
          throw connErr;
        }
      } else {
        pool = sql; 
      }
    }

    // DIAGNOSTIC LOGGING - Disabled to reduce noise
    // if (pool.config) {
    //     console.log('--- POOL CONFIG ---');
    //     console.log('Server:', pool.config.server);
    //     console.log('Database:', pool.config.database);
    //     console.log('User:', pool.config.user);
    // } else {
    //     console.log('--- POOL CONFIG: UNDEFINED (Using global sql?) ---');
    // }

    // Date check
    try {
      const dateRequest = pool.request();
      const dateRes = await dateRequest.query('SELECT CONVERT(date, GETDATE()) AS ServerDate');
      if (dateRes && dateRes.recordset && dateRes.recordset.length > 0) {
        today = moment(dateRes.recordset[0].ServerDate).format('YYYY-MM-DD');
      }
    } catch (e) {
      logAndSave('WatchedItemRealtimeJob: failed to get server date, using local date: ' + (e && e.message));
    }

    const lookbackDate = moment(today).subtract(3, 'days').format('YYYY-MM-DD');
    // logAndSave(`WatchedItemRealtimeJob: checking sales since ${lookbackDate}`);

    // QUERY EXECUTION - Using parameterized query to prevent SQL injection
    const salesRequest = pool.request();
    salesRequest.input('lookbackDate', require('mssql').Date, new Date(lookbackDate));
    
    const result = await salesRequest.query(`
      SELECT 
        Expr2 AS InvoiceNo, 
        Expr4 AS Description, 
        Expr5 AS Qty, 
        Expr6 AS UnitPrice, 
        Expr1 AS InvoiceDate
      FROM [View_Sales report whatsapp]
      WHERE CAST(Expr1 AS DATE) >= @lookbackDate
    `);
    const allRows = result.recordset || [];

    for (const pattern of patterns) {
      const bucket = state.patterns[pattern];
      const matched = allRows.filter(r => (r.Description || '').toUpperCase().includes(pattern.toUpperCase()));
      const totalQty = matched.reduce((acc, r) => acc + (Number(r.Qty) || 0), 0);
      const newOnes = matched.filter(r => !bucket.lastNotified.includes(r.InvoiceNo) && (Number(r.Qty) || 0) >= minQty);
      
      if (newOnes.length > 0) {
        const successfullyNotified = [];
        for (const row of newOnes) {
          let unitPrice = Number(row.UnitPrice) || 0;
          const lineTotal = (row.Qty * unitPrice);
          const msg = `*Watched Item Sold*\nPattern: ${pattern}\nDescription: ${row.Description}\nInvoice: ${row.InvoiceNo}\nQty: ${row.Qty}\nUnit: ${unitPrice}/=\nLine Total: ${lineTotal}/=\nTotal Qty Today (${pattern}): ${totalQty}`;

          const admins = (Array.isArray(cfg.adminNumbers) && cfg.adminNumbers.length > 0)
            ? cfg.adminNumbers
            : (Array.isArray(cfg.contactNumbers) && cfg.contactNumbers.length > 0 ? cfg.contactNumbers : []);
          
          if (admins.length === 0) continue;

          let anySuccess = false;
          try {
            for (const admin of admins) {
                try {
                    await sendWhatsAppMessage(admin, msg);
                    anySuccess = true; 
                    // We don't break, send to all admins
                } catch (err) { }
            }
            
            if (anySuccess) {
              successfullyNotified.push(row.InvoiceNo);
              logAndSave(`WatchedItemRealtimeJob notified invoice ${row.InvoiceNo} pattern ${pattern}`);
              notifiedCount += 1;
            }
          } catch (sendErr) {
            logAndSave('WatchedItem msg send error: ' + sendErr.message);
          }

          // Queue Deletion Logic
          if (anySuccess) {
             try {
                const delaySeconds = parseInt(process.env.DELETION_DELAY_SECONDS || '2', 10);
                const scheduledTime = new Date(Date.now() + (delaySeconds * 1000));
                
                const queueRequest = pool.request();
                queueRequest.input('InvoiceNo', row.InvoiceNo);
                queueRequest.input('ItemID', (row.Description || '').substring(0, 500));
                queueRequest.input('ItemDescription', (row.Description || '').substring(0, 1000));
                // queueRequest.input('SoldDate', new Date()); // Assuming SoldDate is now, or parsing InvoiceDate if possible
                queueRequest.input('SoldDate', row.InvoiceDate ? new Date(row.InvoiceDate) : new Date());
                queueRequest.input('ScheduledTime', scheduledTime);
                queueRequest.input('Pattern', pattern);
                queueRequest.input('Qty', Number(row.Qty) || 0);
                
                await queueRequest.query(`
                    INSERT INTO WhatsAppAI.dbo.TblDeletionQueue 
                    (InvoiceNo, ItemID, ItemDescription, SoldDate, ScheduledDeletionTime, Pattern, Qty, Status)
                    VALUES 
                    (@InvoiceNo, @ItemID, @ItemDescription, @SoldDate, @ScheduledTime, @Pattern, @Qty, 'PENDING')
                `);
             } catch (qErr) { console.error('Queue Error', qErr); }
          }
        }

        if (successfullyNotified.length > 0) {
          bucket.lastNotified = bucket.lastNotified.concat(successfullyNotified);
        }
      }
      bucket.totalQtyToday = totalQty;
      bucket.lastUpdate = new Date().toISOString();
    }
    saveState(state);
    updateJobStatus('WatchedItemRealtimeJob', {
      lastRun: started.toISOString(),
      lastSuccess: true,
      lastError: null,
      patterns: patterns.length,
      notifiedCount,
      nextCheck: new Date(Date.now() + 60000).toISOString()
    });
  } catch (e) {
    console.error('WatchedItemRealtimeJob CRITICAL ERROR:', e);
    
    try {
        fs.writeFileSync(path.join(__dirname, '..', 'logs', `job_diagnostic_${Date.now()}.txt`), `Error: ${e.message}\nStack: ${e.stack}\nPoolConfig: ${JSON.stringify(pool ? pool.config : 'null')}`);
    } catch {}

    logAndSave('WatchedItemRealtimeJob error: ' + e.message);
    updateJobStatus('WatchedItemRealtimeJob', {
      lastRun: started.toISOString(),
      lastSuccess: false,
      lastError: e.message,
      nextCheck: new Date(Date.now() + 60000).toISOString()
    });
  } finally {
    try { 
      if (poolCreatedHere && pool && pool.connected) {
        await pool.close();
      } 
    } catch (closeError) { }
  }
};
