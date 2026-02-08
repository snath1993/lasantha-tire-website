const moment = require('moment-timezone');
const fs = require('fs');
const path = require('path');
const sqlite3 = require('sqlite3').verbose();

// Constants
const SENT_FILE = path.join(__dirname, '..', 'sent_invoices.txt');
const QUOTE_DIR = process.env.QUOTE_DIR || 'C:/QUOTE';
const SENT_DB_PATH = path.join(QUOTE_DIR, 'invoice_reports.db');

// Initialize SQLite tracking database
function initTrackingDb() {
    return new Promise((resolve, reject) => {
        // Ensure the directory exists
        fs.mkdirSync(path.dirname(SENT_DB_PATH), { recursive: true });
        const db = new sqlite3.Database(SENT_DB_PATH);
        db.serialize(() => {
            db.run(`CREATE TABLE IF NOT EXISTS sent_invoices (
                InvoiceNo TEXT PRIMARY KEY,
                SentAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                SentType TEXT CHECK(SentType IN ('AUTO', 'MANUAL'))
            )`, (err) => {
                if (err) return reject(err);
            });
            // New table for AI attributed sales
            db.run(`CREATE TABLE IF NOT EXISTS ai_attributed_sales (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                invoiceNo TEXT,
                amount REAL,
                date TEXT,
                recorded_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(invoiceNo) REFERENCES sent_invoices(InvoiceNo)
            )`, (err) => {
                if (err) return reject(err);
            });
            db.close((err) => {
                if (err) reject(err);
                else resolve();
            });
        });
    });
}

// Get stock quantity from View_ItemWhse
async function getStockQty(sql, itemDescription) {
    try {
        const result = await sql.query`
            SELECT SUM(QTY) as TotalQty
            FROM View_ItemWhse 
            WHERE ItemDis LIKE ${`%${itemDescription}%`}
        `;
        return result.recordset?.[0]?.TotalQty || 0;
    } catch (err) {
        console.error('Stock check error:', err.message);
        return 0;
    }
}

// Get previously sent invoice numbers from text file
function getSentInvoices() {
    try {
        if (!fs.existsSync(SENT_FILE)) {
            return new Set();
        }
        const content = fs.readFileSync(SENT_FILE, 'utf8');
        // Normalize to uppercase trimmed invoice numbers for consistent comparisons
        return new Set(content.split('\n')
            .map(l => l.trim())
            .filter(l => l && !l.startsWith('#'))
            .map(l => l.toUpperCase())
        );
    } catch (err) {
        console.error('Error reading sent invoices:', err);
        return new Set();
    }
}

// Save sent invoice numbers and AI sales data
async function saveSentInvoices(invoiceNos, type = 'AUTO', aiSales = []) {
    try {
        // --- Text file saving logic (for legacy compatibility) ---
        let newList = [];
        if (!invoiceNos) newList = [];
        else if (Array.isArray(invoiceNos)) newList = invoiceNos.map(i => String(i).trim()).filter(Boolean);
        else if (typeof invoiceNos === 'string') newList = invoiceNos.split(/\r?\n/).map(i => String(i).trim()).filter(Boolean);

        const BACKUP_DIR = path.join(__dirname, '..', 'backup');
        if (!fs.existsSync(BACKUP_DIR)) fs.mkdirSync(BACKUP_DIR, { recursive: true });
        if (!fs.existsSync(SENT_FILE)) fs.writeFileSync(SENT_FILE, '# Daily Sales Report - Sent Invoice Numbers\n', 'utf8');

        const existing = getSentInvoices();
        const merged = new Set(Array.from(existing).map(v => v.toUpperCase()));
        newList.forEach(v => merged.add(v.toUpperCase()));

        const ts = moment().format('YYYYMMDD_HHmmss');
        const backupPath = path.join(BACKUP_DIR, `sent_invoices_${ts}.txt`);
        if (fs.existsSync(SENT_FILE)) fs.copyFileSync(SENT_FILE, backupPath);

        const today = moment().format('YYYY-MM-DD');
        const header = '# Daily Sales Report - Sent Invoice Numbers\n';
        const comment = `\n# Sales for ${today} (${type})\n`;
        const lines = Array.from(merged).sort();
        const newContent = header + comment + (lines.length ? lines.join('\n') + '\n' : '\n');

        const tmpPath = SENT_FILE + '.tmp';
        fs.writeFileSync(tmpPath, newContent, 'utf8');
        if (fs.existsSync(SENT_FILE)) fs.unlinkSync(SENT_FILE);
        fs.renameSync(tmpPath, SENT_FILE);
        // --- End of text file logic ---

        // --- SQLite saving logic ---
        await initTrackingDb(); // Ensure tables are created
        const db = new sqlite3.Database(SENT_DB_PATH);

        return new Promise((resolve, reject) => {
            db.serialize(() => {
                const stmtInvoices = db.prepare("INSERT OR IGNORE INTO sent_invoices (InvoiceNo, SentType) VALUES (?, ?)");
                const stmtAiSales = db.prepare("INSERT INTO ai_attributed_sales (invoiceNo, amount, date) VALUES (?, ?, ?)");

                db.run('BEGIN TRANSACTION');

                for (const invNo of newList) {
                    stmtInvoices.run(invNo, type);
                }

                if (aiSales && aiSales.length > 0) {
                    for (const sale of aiSales) {
                        stmtAiSales.run(sale.invoiceNo, sale.amount, sale.date);
                    }
                }

                db.run('COMMIT', (err) => {
                    stmtInvoices.finalize();
                    stmtAiSales.finalize();
                    db.close();
                    if (err) {
                        console.error('Error saving to SQLite:', err.message);
                        reject({ ok: false, error: err.message });
                    } else {
                        resolve({ ok: true, count: lines.length });
                    }
                });
            });
        });

    } catch (err) {
        console.error('Error saving sent invoices:', err && err.message ? err.message : err);
        return { ok: false, error: err && err.message ? err.message : String(err) };
    }
}

/**
 * Handle daily sales report requests from admin
 */
async function handleDailySalesReport(msg, sql, sqlConfig, client, logAndSave, options = {}) {
    const senderNumber = msg.from.replace('@c.us', '');
    
    // Force fullDay mode for testing
    options.fullDay = true;
    
    // Only allow admin numbers
    if (senderNumber !== '0771222509' && senderNumber !== '94771222509') {
        return false;
    }

    const text = msg.body.trim().toLowerCase();
    const normalizedText = text.replace(/\s+/g, '');
    // Match exactly "salesreport" or "sales report"
    if (normalizedText !== 'salesreport') {
        return false;
    }

    try {
        // Initialize tracking file if needed
        if (!fs.existsSync(SENT_FILE)) {
            fs.writeFileSync(SENT_FILE, '# Daily Sales Report - Sent Invoice Numbers\n', 'utf8');
        }

        // Connect to SQL Server
        let pool = await sql.connect(sqlConfig);
        
        // Get today's date range in Sri Lanka time (Asia/Colombo)
        const today = moment().tz('Asia/Colombo').format('YYYY-MM-DD');
        const startTime = moment().tz('Asia/Colombo').startOf('day').format('YYYY-MM-DD HH:mm:ss');
        const endTime = moment().tz('Asia/Colombo').endOf('day').format('YYYY-MM-DD HH:mm:ss');
        
        console.log('Date debug:', {
            raw: new Date(),
            today,
            startTime,
            endTime,
            momentNow: moment().format(),
            momentTz: moment().tz('Asia/Colombo').format()
        });

        // Get sent invoice numbers (for AUTO reports only)
        const sentInvoices = options.fullDay ? new Set() : await getSentInvoices();
        console.log('Sent invoices:', Array.from(sentInvoices));

        // Query to get today's tyre sales
        console.log('Querying sales for:', { startTime, endTime });
        const salesResult = await pool.request()
            .input('StartTime', sql.DateTime, startTime)
            .input('EndTime', sql.DateTime, endTime)
            .query(`
                SELECT 
                    InvoiceNo,
                    Description,
                    Qty,
                    UnitPrice,
                    Amount,
                    Categoty,
                    UnitCost
                FROM [View_Sales report whatsapp]
                WHERE InvoiceDate BETWEEN @StartTime AND @EndTime
                AND Categoty = 'TYRES'
            `);
        console.log('Query returned:', salesResult.recordset.length, 'rows');
        if (salesResult.recordset.length > 0) {
            console.log('First record:', salesResult.recordset[0]);
            console.log('Invoice numbers:', salesResult.recordset.map(r => r.InvoiceNo));
        }

    // Filter out already sent invoices (compare uppercase)
    const newSales = salesResult.recordset.filter(sale => !sentInvoices.has(String(sale.InvoiceNo).trim().toUpperCase()));

        // Format the response
        let response = `*Daily Sales Report*\nDate: ${today}\n──────────\n`;

        if (newSales.length > 0) {
            // Group by invoice
            const invoiceMap = new Map();
            for (const sale of newSales) {
                if (!invoiceMap.has(sale.InvoiceNo)) {
                    invoiceMap.set(sale.InvoiceNo, []);
                }
                invoiceMap.get(sale.InvoiceNo).push(sale);
            }

            // Calculate totals and prepare detailed report
            let totalQty = 0;
            let totalAmount = 0;
            let totalProfit = 0;
            let itemDetails = '';

            // Process each invoice
            for (const [invNo, items] of invoiceMap) {
                // Add invoice header
                itemDetails += `\n📋 *Invoice: ${invNo}*`;
                
                // Process items in this invoice
                for (const item of items) {
                    const qty = Number(item.Qty) || 0;
                    const unitPrice = Number(item.UnitPrice) || 0;
                    const unitCost = Number(item.UnitCost) || 0;
                    const amount = Number(item.Amount) || (qty * unitPrice);
                    const profit = amount - (qty * unitCost);

                    // Get current stock quantity
                    const stockQty = await getStockQty(sql, item.Description);

                    // Add item details with proper formatting
                    itemDetails += `\n🔸 ${item.Description}\n`;
                    itemDetails += `   Quantity Sold: ${qty}\n`;
                    itemDetails += `   Stock Available: ${stockQty}\n`;
                    itemDetails += `   Unit Price: Rs.${unitPrice.toLocaleString()}\n`;
                    itemDetails += `   Total: Rs.${amount.toLocaleString()}\n`;
                    itemDetails += `   Profit: Rs.${profit.toLocaleString()}\n`;
                    itemDetails += `   ──────────\n`;

                    totalQty += qty;
                    totalAmount += amount;
                    totalProfit += profit;
                }
            }

            // Add detailed report first
            response += `📝 *Detailed Report*`;
            response += itemDetails;

            // Add summary at the bottom with a divider
            response += `\n\n📊 *Summary*\n`;
            response += `───────────\n`;
            response += `Total Sales Records: ${newSales.length}\n`;
            response += `Total Invoices: ${invoiceMap.size}\n`;
            response += `Total Quantity: ${totalQty}\n`;
            response += `Total Amount: LKR ${totalAmount.toLocaleString()}\n`;
            response += `Total Profit: LKR ${totalProfit.toLocaleString()}`;

            // Save the sent invoice numbers
            await saveSentInvoices(
                Array.from(invoiceMap.keys()), 
                options.fullDay ? 'MANUAL' : 'AUTO'
            );
        } else {
            response += 'No new tyre sales found for today.';
        }

        // Send the response
        await msg.reply(response);
        if (logAndSave) logAndSave(`Daily sales report sent to ${senderNumber}`);
        
        return true;
    } catch (err) {
        console.error('Error in handleDailySalesReport:', err);
        if (logAndSave) logAndSave(`Error in handleDailySalesReport: ${err.message}`);
        await msg.reply('Error generating sales report. Please try again later.');
        return true;
    }
}

module.exports = {
    handleDailySalesReport,
    initTrackingDb,
    // expose helpers for other modules (jobs)
    getSentInvoices,
    saveSentInvoices
};
