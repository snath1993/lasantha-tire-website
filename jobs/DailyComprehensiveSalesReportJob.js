// Comprehensive Daily Sales Report by Category
// Sends after full day tyre sales report

const moment = require('moment');

function formatCurrency(num) {
    return num.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

module.exports = async function DailyComprehensiveSalesReportJob(sql, sqlConfig, sendWhatsApp, logAndSave, options = {}) {
    const { mainPool } = options;
    
    if (!mainPool) {
        console.error('[Comprehensive Report] No database pool provided');
        return;
    }

    try {
        // Get server date
        let serverDate = null;
        const dateRequest = mainPool.request();
        const dateResult = await dateRequest.query('SELECT CONVERT(date, GETDATE()) AS ServerDate');
        if (dateResult && dateResult.recordset && dateResult.recordset.length > 0) {
            serverDate = moment(dateResult.recordset[0].ServerDate).format('YYYY-MM-DD');
        }
        
        const today = options.date && String(options.date).trim() 
            ? moment(String(options.date).trim()).format('YYYY-MM-DD') 
            : (serverDate || moment().format('YYYY-MM-DD'));

        // Fetch all sales data
        const queryRequest = mainPool.request();
        queryRequest.input('today', sql.Date, new Date(today));
        const res = await queryRequest.query`
            SELECT 
                Expr2 AS InvoiceNo,
                Expr4 AS Description,
                Categoty,
                Expr5 AS Qty,
                Expr6 AS UnitPrice,
                Expr12 AS Amount,
                Expr1 AS InvoiceDate
            FROM [View_Sales report whatsapp]
            WHERE CONVERT(date, Expr1) = ${today}
                AND Expr6 > 0
                AND (Expr14 = 0 OR Expr14 IS NULL)
            ORDER BY Expr2, Categoty
        `;

        const rows = res.recordset || [];

        if (!rows.length) {
            console.log('[Comprehensive Report] No sales data for', today);
            return;
        }

        // Group by category
        const categoryTotals = {
            'TYRES': { qty: 0, amount: 0 },
            'KATTA TYRES': { qty: 0, amount: 0 },
            'DAG TYRES': { qty: 0, amount: 0 },
            'REBUILD TYRES': { qty: 0, amount: 0 },
            'TUBES': { qty: 0, amount: 0 },
            'TAPES/FLAPS': { qty: 0, amount: 0 },
            'ALLOYE WHEELS': { qty: 0, amount: 0 },
            'SPARE WHEEL': { qty: 0, amount: 0 },
            'TUBELESS VALVE': { qty: 0, amount: 0 },
            'WHEEL ALIGNMENT': { qty: 0, amount: 0, invoices: new Set() },
            'SERVICES': { qty: 0, amount: 0 },
            'ALIGNMENT': { qty: 0, amount: 0, items: new Map() },
            'Customize': { qty: 0, amount: 0, items: new Map() },
            'OTHERS': { qty: 0, amount: 0 }
        };

        let totalAmount = 0;
        let totalInvoices = new Set();

        for (const row of rows) {
            const cat = (row.Categoty || '').trim().toUpperCase();
            const amount = Number(row.Amount) || 0;
            const qty = Number(row.Qty) || 0;

            totalAmount += amount;
            totalInvoices.add(row.InvoiceNo);

            if (cat === 'TYRES') {
                categoryTotals['TYRES'].qty += qty;
                categoryTotals['TYRES'].amount += amount;
            } else if (cat === 'KATTA TYRES') {
                categoryTotals['KATTA TYRES'].qty += qty;
                categoryTotals['KATTA TYRES'].amount += amount;
            } else if (cat === 'DAG TYRES') {
                categoryTotals['DAG TYRES'].qty += qty;
                categoryTotals['DAG TYRES'].amount += amount;
            } else if (cat === 'REBUILD TYRES') {
                categoryTotals['REBUILD TYRES'].qty += qty;
                categoryTotals['REBUILD TYRES'].amount += amount;
            } else if (cat === 'TUBES') {
                categoryTotals['TUBES'].qty += qty;
                categoryTotals['TUBES'].amount += amount;
            } else if (cat === 'TAPES/FLAPS') {
                categoryTotals['TAPES/FLAPS'].qty += qty;
                categoryTotals['TAPES/FLAPS'].amount += amount;
            } else if (cat === 'ALLOYE WHEELS') {
                categoryTotals['ALLOYE WHEELS'].qty += qty;
                categoryTotals['ALLOYE WHEELS'].amount += amount;
            } else if (cat === 'SPARE WHEEL') {
                categoryTotals['SPARE WHEEL'].qty += qty;
                categoryTotals['SPARE WHEEL'].amount += amount;
            } else if (cat === 'TUBELESS VALVE') {
                categoryTotals['TUBELESS VALVE'].qty += qty;
                categoryTotals['TUBELESS VALVE'].amount += amount;
            } else if (cat === 'WHEEL ALIGNMENT') {
                categoryTotals['WHEEL ALIGNMENT'].invoices.add(row.InvoiceNo);
                categoryTotals['WHEEL ALIGNMENT'].amount += amount;
            } else if (cat === 'SERVICES') {
                categoryTotals['SERVICES'].qty += qty;
                categoryTotals['SERVICES'].amount += amount;
            } else if (cat === 'ALIGNMENT') {
                categoryTotals['ALIGNMENT'].qty += qty;
                categoryTotals['ALIGNMENT'].amount += amount;
                const key = row.Description;
                if (!categoryTotals['ALIGNMENT'].items.has(key)) {
                    categoryTotals['ALIGNMENT'].items.set(key, { qty: 0, amount: 0 });
                }
                const item = categoryTotals['ALIGNMENT'].items.get(key);
                item.qty += qty;
                item.amount += amount;
            } else if (cat === 'CUSTOMIZE') {
                categoryTotals['Customize'].qty += qty;
                categoryTotals['Customize'].amount += amount;
                const key = row.Description;
                if (!categoryTotals['Customize'].items.has(key)) {
                    categoryTotals['Customize'].items.set(key, { qty: 0, amount: 0 });
                }
                const item = categoryTotals['Customize'].items.get(key);
                item.qty += qty;
                item.amount += amount;
            } else {
                categoryTotals['OTHERS'].qty += qty;
                categoryTotals['OTHERS'].amount += amount;
            }
        }

        // Wheel Alignment qty = unique invoice count
        categoryTotals['WHEEL ALIGNMENT'].qty = categoryTotals['WHEEL ALIGNMENT'].invoices.size;

        // Build report
        let report = `📊 *DAILY SALES SUMMARY*\n`;
        report += `Date: ${moment(today).format('MMMM DD, YYYY')}\n`;
        report += `${'─'.repeat(35)}\n\n`;
        
        report += `📈 *OVERALL SUMMARY*\n`;
        report += `Total Invoices: ${totalInvoices.size}\n`;
        report += `Total Amount: Rs.${formatCurrency(totalAmount)}\n\n`;
        
        report += `${'─'.repeat(35)}\n\n`;
        report += `📦 *CATEGORY BREAKDOWN*\n\n`;

        // Add categories only if they have sales
        if (categoryTotals['KATTA TYRES'].amount > 0) {
            report += `🔸 *KATTA TYRES*\n`;
            report += `   Quantity: ${categoryTotals['KATTA TYRES'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['KATTA TYRES'].amount)}\n\n`;
        }

        if (categoryTotals['TYRES'].amount > 0) {
            report += `🔸 *TYRES*\n`;
            report += `   Quantity: ${categoryTotals['TYRES'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['TYRES'].amount)}\n\n`;
        }

        if (categoryTotals['DAG TYRES'].amount > 0) {
            report += `🔸 *DAG TYRES*\n`;
            report += `   Quantity: ${categoryTotals['DAG TYRES'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['DAG TYRES'].amount)}\n\n`;
        }

        if (categoryTotals['REBUILD TYRES'].amount > 0) {
            report += `🔸 *REBUILD TYRES*\n`;
            report += `   Quantity: ${categoryTotals['REBUILD TYRES'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['REBUILD TYRES'].amount)}\n\n`;
        }

        if (categoryTotals['TUBES'].amount > 0) {
            report += `🔸 *TUBES*\n`;
            report += `   Quantity: ${categoryTotals['TUBES'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['TUBES'].amount)}\n\n`;
        }

        if (categoryTotals['TAPES/FLAPS'].amount > 0) {
            report += `🔸 *TAPES/FLAPS*\n`;
            report += `   Quantity: ${categoryTotals['TAPES/FLAPS'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['TAPES/FLAPS'].amount)}\n\n`;
        }

        if (categoryTotals['ALLOYE WHEELS'].amount > 0) {
            report += `🔸 *ALLOYE WHEELS*\n`;
            report += `   Quantity: ${categoryTotals['ALLOYE WHEELS'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['ALLOYE WHEELS'].amount)}\n\n`;
        }

        if (categoryTotals['SPARE WHEEL'].amount > 0) {
            report += `🔸 *SPARE WHEEL*\n`;
            report += `   Quantity: ${categoryTotals['SPARE WHEEL'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['SPARE WHEEL'].amount)}\n\n`;
        }

        if (categoryTotals['TUBELESS VALVE'].amount > 0) {
            report += `🔸 *TUBELESS VALVE*\n`;
            report += `   Quantity: ${categoryTotals['TUBELESS VALVE'].qty}\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['TUBELESS VALVE'].amount)}\n\n`;
        }

        if (categoryTotals['WHEEL ALIGNMENT'].amount > 0) {
            report += `🔸 *WHEEL ALIGNMENT*\n`;
            report += `   Count: ${categoryTotals['WHEEL ALIGNMENT'].qty} invoices\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['WHEEL ALIGNMENT'].amount)}\n\n`;
        }

        if (categoryTotals['SERVICES'].amount > 0) {
            report += `🔸 *SERVICES*\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['SERVICES'].amount)}\n\n`;
        }

        if (categoryTotals['ALIGNMENT'].amount > 0) {
            report += `🔸 *ALIGNMENT*\n`;
            report += `   Items:\n`;
            for (const [desc, data] of categoryTotals['ALIGNMENT'].items) {
                report += `      • ${desc}\n`;
                report += `        Qty: ${data.qty}, Amount: Rs.${formatCurrency(data.amount)}\n`;
            }
            report += `   Total: Rs.${formatCurrency(categoryTotals['ALIGNMENT'].amount)}\n\n`;
        }

        if (categoryTotals['Customize'].amount > 0) {
            report += `🔸 *CUSTOMIZE*\n`;
            report += `   Items:\n`;
            for (const [desc, data] of categoryTotals['Customize'].items) {
                report += `      • ${desc}\n`;
                report += `        Qty: ${data.qty}, Amount: Rs.${formatCurrency(data.amount)}\n`;
            }
            report += `   Total: Rs.${formatCurrency(categoryTotals['Customize'].amount)}\n\n`;
        }

        if (categoryTotals['OTHERS'].amount > 0) {
            report += `🔸 *OTHER ITEMS*\n`;
            report += `   Total: Rs.${formatCurrency(categoryTotals['OTHERS'].amount)}\n\n`;
        }

        report += `${'─'.repeat(35)}\n`;
        report += `💰 *GRAND TOTAL: Rs.${formatCurrency(totalAmount)}*`;

        // Send report to configured numbers
        const recipients = options.recipients || [];
        for (const number of recipients) {
            await sendWhatsApp(number, report);
            if (logAndSave) logAndSave(`[Comprehensive Report] Sent to ${number}`);
        }

        console.log('[Comprehensive Report] ✅ Report sent successfully');

    } catch (error) {
        console.error('[Comprehensive Report] ❌ Error:', error.message);
        if (logAndSave) logAndSave(`[Comprehensive Report] Error: ${error.message}`);
    }
};
