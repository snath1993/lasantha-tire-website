// DailyTyreSalesReportJob.js
// Scheduled job: send daily tyre sales report to specified WhatsApp numbers
const moment = require('moment');
const salesCtrl = require('../controllers/salesReportController');
const { aiPoolConnect, aiPool } = require('../utils/aiDbConnection'); // AI DB connection
// mainPool is passed as a parameter from scheduler/index.js
const sql = require('mssql'); // For type definitions

// === Feature flags ===
const REPORT_INCLUDE_PROFIT = process.env.REPORT_INCLUDE_PROFIT === '1';

// === Constants ===
const TYRE_CATEGORY = 'TYRES';
const MAX_DISCOUNT_PERCENTAGE = 100;
const WHATSAPP_MESSAGE_DELAY_MS = 1000; // Delay between messages to prevent spam flagging
const DAILY_REPORT_NUMBERS = (process.env.DAILY_REPORT_NUMBERS || '0777311770,0777078700').split(',').map(n => n.trim());

/**
 * DISABLED: Checks for AI-attributed sales by cross-referencing with the ai_conversations table.
 * Currently disabled because CustomerPhone column is not available in [View_Sales report whatsapp]
 * @param {Array<string>} customerPhones - An array of customer phone numbers from the invoices.
 * @returns {Promise<Set<string>>} - A Set containing phone numbers that had recent AI interactions.
 */
/*
async function getAiAttributedPhones(customerPhones) {
    if (!customerPhones || customerPhones.length === 0) {
        return new Set();
    }

    try {
        await aiPoolConnect;
        const request = new sql.Request(aiPool);
        
        // Normalize phone numbers to handle variations like 9477... vs 077...
        const normalizedPhones = customerPhones.map(p => {
            if (!p) return null;
            let phone = String(p);
            if (phone.startsWith('94')) return phone.substring(2);
            if (phone.startsWith('+94')) return phone.substring(3);
            return phone;
        }).filter(Boolean);

        if (normalizedPhones.length === 0) return new Set();

        // Create a temporary table to hold the phone numbers for an efficient join
        const phoneTable = new sql.Table();
        phoneTable.columns.add('phone', sql.VarChar(25));
        new Set(normalizedPhones).forEach(p => phoneTable.rows.add(p));

        const result = await request
            .input('phoneNumbers', phoneTable)
            .query(`
                SELECT DISTINCT ac.user_phone
                FROM ai_conversations ac
                JOIN @phoneNumbers p ON (
                    ac.user_phone = p.phone OR
                    '0' + ac.user_phone = p.phone OR
                    ac.user_phone = '0' + p.phone
                )
                WHERE ac.last_ai_interaction >= DATEADD(hour, -48, GETDATE())
            `);

        const attributedPhones = new Set(result.recordset.map(r => r.user_phone));
        if (attributedPhones.size > 0) {
            console.log(`[AI Attribution] Found ${attributedPhones.size} attributed sales.`);
        }
        return attributedPhones;

    } catch (err) {
        console.error('[AI Attribution Error]', err.message);
        return new Set(); // Return an empty set on error
    }
}
*/


function formatCurrency(amount) {
    return Number(amount || 0).toLocaleString('en-LK', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

// Get available quantity for an item from View_ItemWhse (SQL Server)
async function getAvailableQuantity(sqlPool, itemDescription) {
    try {
        if (!sqlPool || !sqlPool.connected) return 0;
        const request = sqlPool.request();
        request.input('itemDescription', sql.VarChar, itemDescription);
        const res = await request.query`
            SELECT TOP 1 QTY
            FROM View_ItemWhse
            WHERE ItemDis = ${itemDescription}
            ORDER BY QTY DESC
        `;
        if (res.recordset && res.recordset.length) return Number(res.recordset[0].QTY) || 0;
        return 0;
    } catch (e) {
        console.warn('getAvailableQuantity error:', e && e.message ? e.message : e);
        return 0;
    }
}

// Batch fetch available quantities for multiple items (Performance optimization)
async function batchGetAvailableQuantities(sqlPool, itemDescriptions) {
    try {
        if (!sqlPool) {
            console.warn('batchGetAvailableQuantities: sqlPool is null or undefined');
            return new Map();
        }
        
        if (!sqlPool.connected) {
            console.warn('batchGetAvailableQuantities: sqlPool is not connected');
            return new Map();
        }
        
        if (!itemDescriptions || itemDescriptions.length === 0) {
            return new Map();
        }
        
        // Remove duplicates
        const uniqueItems = [...new Set(itemDescriptions)];
        
        if (uniqueItems.length === 0) return new Map();
        
        // Build WHERE IN clause safely
        const request = sqlPool.request();
        let whereClause = '';
        uniqueItems.forEach((item, idx) => {
            const paramName = `item${idx}`;
            request.input(paramName, sql.VarChar, item);
            whereClause += (idx > 0 ? ',' : '') + `@${paramName}`;
        });
        
        const query = `
            SELECT ItemDis, MAX(QTY) AS QTY
            FROM View_ItemWhse
            WHERE ItemDis IN (${whereClause})
            GROUP BY ItemDis
        `;
        
        const res = await request.query(query);
        
        const stockMap = new Map();
        if (res.recordset) {
            for (const row of res.recordset) {
                stockMap.set(row.ItemDis, Number(row.QTY) || 0);
            }
        }
        
        // Fill in missing items with 0
        for (const item of uniqueItems) {
            if (!stockMap.has(item)) {
                stockMap.set(item, 0);
            }
        }
        
        return stockMap;
    } catch (e) {
        console.error('batchGetAvailableQuantities error:', e && e.message ? e.message : e);
        console.error('Stack:', e && e.stack ? e.stack : 'No stack trace');
        return new Map();
    }
}


// Normalize or safely stringify invoice dates
function normalizeInvoiceDate(raw) {
    if (!raw) return null;
    try {
        if (raw instanceof Date) return raw.toISOString();
        const s = String(raw).trim();
        if (!s) return null;
        const d = new Date(s);
        if (!isNaN(d.getTime())) return d.toISOString();
        const d2 = new Date(s + 'Z');
        if (!isNaN(d2.getTime())) return d2.toISOString();
        return null;
    } catch (e) {
        return null;
    }
}

module.exports = async function DailyTyreSalesReportJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, options = {}) {
    let sqlOpened = false;
    
    // ========================================
    // CRITICAL: Extract mainPool from options
    // This is used for database queries throughout the function
    // ========================================
    const mainPool = options.mainPool || sql;
    
    // Safe wrapper for WhatsApp sends: supports two modes
    // - safeSend(number, message) -> forwards to sendWhatsAppMessage
    // - safeSend(message) -> sends message to configured contactNumbers for this job
    const fs = require('fs');
    const path = require('path');
        async function safeSend(a, b) {
            try {
                if (b === undefined) {
                    // a is the message: send to job-configured recipients
                    const cfgPath = path.join(__dirname, '..', 'jobs-config.json');
                    let recipients = [];
                    try {
                        if (fs.existsSync(cfgPath)) {
                            const cfg = JSON.parse(fs.readFileSync(cfgPath, 'utf8'));
                            recipients = (cfg && cfg['DailyTyreSalesReportJob'] && cfg['DailyTyreSalesReportJob'].contactNumbers) ? cfg['DailyTyreSalesReportJob'].contactNumbers : [];
                        }
                    } catch (e) { recipients = []; }
                    if (!recipients || recipients.length === 0) {
                        console.error('[WA] No recipients configured for DailyTyreSalesReportJob');
                        return { ok: false, error: 'no-recipients' };
                    }
                    let lastResult = { ok: true };
                    for (let i = 0; i < recipients.length; i++) {
                        const num = recipients[i];
                        
                        // Add delay between messages to prevent spam flagging
                        if (i > 0) {
                            await new Promise(resolve => setTimeout(resolve, WHATSAPP_MESSAGE_DELAY_MS));
                        }
                        
                        const r = await sendWhatsAppMessage(num, a);
                        // Some send overrides (debug/testing) may not return a structured result; treat undefined as success
                        if (r === undefined) {
                          // assume success for debug senders that only log
                          continue;
                        }
                        if (!r || !r.ok) {
                            console.error('[WA] send to', num, 'failed:', r && r.error);
                            lastResult = { ok: false, error: r && r.error };
                        }
                    }
                    return lastResult;
                }
                // two-arg form: (number, message)
                return await sendWhatsAppMessage(a, b);
            } catch (err) {
                const m = err && err.message ? err.message : String(err);
                if (/unregistered-number/.test(m) || /getNumberId error/.test(m) || /Evaluation failed/.test(m)) {
                    console.warn('[WA] Non-fatal send error:', m);
                    return { ok: false, error: m };
                }
                throw err;
            }
        }

    // prefer replying in-chat if options.replyToMsg is provided; otherwise use safeSend
    const deliver = (options && options.replyToMsg && typeof options.replyToMsg.reply === 'function')
        ? (async (text) => options.replyToMsg.reply(text))
        : safeSend;

    try {
        // ========================================
        // CRITICAL FIX: Use mainPool for all database operations
        // mainPool is the actual connection pool passed from scheduler
        // ========================================
        
        // Ensure mainPool is connected
        if (!mainPool || !mainPool.connected) {
            throw new Error('Database connection pool not available or not connected');
        }

        // Get date from SQL Server instead of local PC
        let serverDate = null;
        const dateRequest = mainPool.request();
        const dateResult = await dateRequest.query('SELECT CONVERT(date, GETDATE()) AS ServerDate');
        if (dateResult && dateResult.recordset && dateResult.recordset.length > 0) {
            serverDate = moment(dateResult.recordset[0].ServerDate).format('YYYY-MM-DD');
        }
        
        const today = options.date && String(options.date).trim() 
            ? moment(String(options.date).trim()).format('YYYY-MM-DD') 
            : (serverDate || moment().format('YYYY-MM-DD')); // fallback to local date if server date fails

        // Fetch day rows from SQL Server view (Fixed: Use CONVERT for proper date comparison)
        // Mapped columns from View: Expr1=InvoiceDate, Expr2=InvoiceNo, Expr4=Description, Expr5=Qty, Expr6=UnitPrice, Expr8=VehicleNo, Expr10=CustomerName, Expr12=Amount, Expr13=LineDiscountPercentage, Expr14=IsVoid
        const queryRequest = mainPool.request();
        queryRequest.input('today', sql.Date, new Date(today));
        const res = await queryRequest.query`
                        SELECT 
                                Expr2 AS InvoiceNo,
                                Expr4 AS Description,
                                Expr5 AS Qty,
                                Expr6 AS UnitPrice,
                                UnitCost,
                                Expr1 AS InvoiceDate,
                                Expr12 AS Amount,
                                Categoty,
                                Expr10 AS CustomerName,
                                Expr8 AS VehicleNo,
                                Expr13 AS LineDiscountPercentage
                        FROM [View_Sales report whatsapp]
                        WHERE CONVERT(date, Expr1) = ${today}
                            AND Expr6 > 0
                            AND (Expr14 = 0 OR Expr14 IS NULL) -- IsVoid check enabled using Expr14
                        ORDER BY Expr1, Expr2
                `;

        let rows = res.recordset || [];

        // Keep only tyre category rows (using constant)
        rows = rows.filter(r => (r.Categoty || '').toString().trim().toUpperCase() === TYRE_CATEGORY);

        if (!rows.length) {
            const emptyMsg = `╔════════════════════════╗\n║   Daily Tyre Sales     ║\n╚════════════════════════╝\n\nDate: ${moment(today).format('MMMM DD, YYYY')}\nStatus: No tyre sales recorded for the day.`;
            // For interactive/manual (replyToMsg) calls, still reply
            if (options && options.replyToMsg && typeof options.replyToMsg.reply === 'function') {
                await deliver(emptyMsg);
                return;
            }
            // For automated scheduled runs, skip sending an empty report
            console.log('No tyre sales rows for', today, '- skipping automated send');
            if (logAndSave) logAndSave(`No tyre sales rows for ${today}`);
            return;
        }

        // Group rows by InvoiceNo
        const invMap = new Map();
        for (const r of rows) {
            if (!invMap.has(r.InvoiceNo)) invMap.set(r.InvoiceNo, []);
            invMap.get(r.InvoiceNo).push(r);
        }

        // Totals
        let totalLineItems = 0;
        let totalUnits = 0;
        let totalAmount = 0;
        let totalCost = 0;
        let totalProfit = 0;
        let totalAiAttributedAmount = 0; // New total for AI sales

        // Determine already-sent invoices (when not full-day)
        const isFullDayReport = options.fullDay || options.date;
        const sentInvoices = isFullDayReport ? new Set() : salesCtrl.getSentInvoices();

        // Note: AI Attribution disabled - CustomerPhone not available in view
        // const customerPhones = Array.from(invMap.values()).flat().map(item => item.CustomerPhone);
        // const aiAttributedPhones = await getAiAttributedPhones(customerPhones);

        // Batch fetch stock quantities for all items (Performance optimization)
        const allItemDescriptions = rows.map(r => r.Description);
        const stockMap = await batchGetAvailableQuantities(mainPool, allItemDescriptions);

        const invoices = [];
        for (const [invNo, items] of invMap) {
            // skip invoices that were already recorded as sent
            if (sentInvoices && sentInvoices.has && sentInvoices.has(invNo)) continue;
            let invUnits = 0, invAmt = 0, invCost = 0, invProfit = 0;
            // const isAiAttributed = aiAttributedPhones.has(items[0].CustomerPhone?.substring(2)) || aiAttributedPhones.has(items[0].CustomerPhone?.substring(3));
            const isAiAttributed = false; // Disabled until CustomerPhone available
            
            const itemLines = [];
            for (const it of items) {
                                const qty = Number(it.Qty) || 0;
                                let unitPrice = Number(it.UnitPrice) || 0;
                                
                                // Apply discount with validation
                                let discountPercentage = Number(it.LineDiscountPercentage) || 0;
                                if (discountPercentage > MAX_DISCOUNT_PERCENTAGE) {
                                    console.warn(`[Discount Warning] Invoice ${invNo}: Discount ${discountPercentage}% exceeds maximum ${MAX_DISCOUNT_PERCENTAGE}%, capping at ${MAX_DISCOUNT_PERCENTAGE}%`);
                                    discountPercentage = MAX_DISCOUNT_PERCENTAGE;
                                }
                                if (discountPercentage > 0) {
                                    unitPrice = unitPrice * (1 - discountPercentage / 100);
                                }
                                
                                const amount = qty * unitPrice;
                                const unitCost = (it.UnitCost !== undefined && it.UnitCost !== null) ? Number(it.UnitCost) : 0;
                                const cost = unitCost * qty;
                                const profit = amount - cost;

                                invUnits += qty; invAmt += amount; invCost += cost; invProfit += profit;
                                totalLineItems++; totalUnits += qty; totalAmount += amount; totalCost += cost; totalProfit += profit;
                                if (isAiAttributed) {
                                    totalAiAttributedAmount += amount;
                                }

                                // Use batch-fetched stock quantity
                                const stock = stockMap.get(it.Description) || 0;
                                itemLines.push({ 
                                    desc: it.Description, 
                                    qty, 
                                    unitPrice,
                                    unitCost,
                                    amount, 
                                    cost: REPORT_INCLUDE_PROFIT ? cost : undefined, 
                                    profit: REPORT_INCLUDE_PROFIT ? profit : undefined, 
                                    stock, 
                                    discount: discountPercentage 
                                });
            }
            invoices.push({ invNo, date: normalizeInvoiceDate(items[0].InvoiceDate) || moment(items[0].InvoiceDate).format('YYYY-MM-DD HH:mm'), units: invUnits, amount: invAmt, profit: invProfit, lines: itemLines, isAiAttributed });
        }

        // If after filtering there are no invoices to report, do not send an empty "0 tyres" report
        if (!invoices || invoices.length === 0) {
            // For interactive/manual (replyToMsg) calls, still reply with a friendly no-sales message
            if (options && options.replyToMsg && typeof options.replyToMsg.reply === 'function') {
                const emptyMsg = `╔════════════════════════╗\n║   Daily Tyre Sales     ║\n╚════════════════════════╝\n\nDate: ${moment(today).format('MMMM DD, YYYY')}\nStatus: No tyre sales recorded for the day.`;
                await deliver(emptyMsg);
                return;
            }
            // For automated runs, just log and return quietly
            console.log('No new tyre invoices to report for', today, '- skipping automated send');
            if (logAndSave) logAndSave(`No new tyre invoices to report for ${today}`);
            return;
        }

        // Build report with emoji and classic layout
        const header = `${isFullDayReport ? '📊 *FULL DAY SALES REPORT*\nDate: ' + moment(today).format('MMMM DD, YYYY') + '\n───────────────────\n\n' : '📊 '}Summary\n`;
        const summary = `Total Sales Records: ${totalLineItems}\nTotal Invoices: ${invoices.length}\nTotal Quantity: ${totalUnits}\nTotal Amount: LKR ${formatCurrency(totalAmount)}\nTotal Profit: LKR ${formatCurrency(totalProfit)}${REPORT_INCLUDE_PROFIT ? `\nMargin Rate: ${totalAmount ? ((totalProfit/totalAmount)*100).toFixed(1) : '0.0'}%` : ''}\n\n🤖 AI Attributed Sales: LKR ${formatCurrency(totalAiAttributedAmount)}\n\n📝 Detailed Report\n`;

        let body = '';
        for (const inv of invoices) {
            body += `\n📋 Invoice: ${inv.invNo} ${inv.isAiAttributed ? '🤖' : ''}\n`;
            for (const L of inv.lines) {
                body += `🔸 ${L.desc}\n`;
                body += `Quantity: ${L.qty}\n`;
                body += `Unit Price: Rs.${formatCurrency(L.unitPrice)}\n`;
                body += `Unit Cost: Rs.${formatCurrency(L.unitCost)}\n`;
                body += `Total: Rs.${formatCurrency(L.amount)}\n`;
                if (REPORT_INCLUDE_PROFIT) {
                    const margin = L.amount > 0 ? ((L.profit / L.amount) * 100).toFixed(1) : '0.0';
                    body += `💰 Profit: Rs.${formatCurrency(L.profit)} (Margin: ${margin}%)\n`;
                }
                body += `📦 Stock Available: [*${L.stock}*]\n`;
            }
            body += `──────────\n`;
        }
        const sendResult = await deliver(header + summary + body);

        // If send succeeded (ok true or undefined for debug senders), persist sent invoice numbers
        if (sendResult === undefined || (sendResult && sendResult.ok)) {
            try {
                const invNos = invoices.map(i => i.invNo);
                if (invNos.length) {
                    // Also save AI attributed sales to SQLite
                    const aiSales = invoices.filter(i => i.isAiAttributed).map(i => ({
                        invoiceNo: i.invNo,
                        amount: i.amount,
                        date: i.date,
                    }));
                    await salesCtrl.saveSentInvoices(invNos, isFullDayReport ? 'MANUAL' : 'AUTO', aiSales);
                }
            } catch (e) {
                console.warn('Could not persist sent invoice numbers:', e && e.message ? e.message : e);
            }

            // Send comprehensive sales summary after full day tyre report
            if (isFullDayReport && !options.replyToMsg) {
                try {
                    const ComprehensiveSalesReport = require('./DailyComprehensiveSalesReportJob');
                    await ComprehensiveSalesReport(sql, sqlConfig, sendWhatsAppMessage, logAndSave, {
                        mainPool,
                        date: today,
                        recipients: DAILY_REPORT_NUMBERS
                    });
                } catch (compErr) {
                    console.error('[Comprehensive Report] Error:', compErr.message);
                }
            }
        }

    } catch (err) {
        console.error('DailyTyreSalesReportJob error:', err.stack);
        if (logAndSave) logAndSave('DailyTyreSalesReportJob error: ' + (err.stack || err.message));
        // For interactive/manual calls, still reply with error
        if (options && options.replyToMsg && typeof options.replyToMsg.reply === 'function') {
            await deliver('An error occurred while generating the report: ' + err.message);
        }
    } finally {
        if (sqlOpened && sql.connected) {
            try {
                await sql.close();
            } catch (e) {
                console.warn('Could not close SQL connection:', e.message);
            }
        }
    }
};