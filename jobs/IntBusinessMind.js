const Groq = require('groq-sdk');
const { executeSafely } = require('../utils/readOnlySafeDb');
const fs = require('fs');
const path = require('path');

// ============================================================================
// INT - ADVANCED BUSINESS INTELLIGENCE SYSTEM (The Strategic Brain)
// ============================================================================

const JOB_NAME = 'IntBusinessMind';
// Flexible Admin List (normalized during check)
const ALLOWED_ADMINS = [
    '94777078700', '0777078700',
    '94771222509', '0771222509',
    '947771222509', '07771222509', // Added missing admin number
    '94777311770', '0777311770',
    '94777440230', '0777440230',
    '94776235674', '0776235674'
];

// Model Configuration - Priority: Groq Llama 3 (Free & Fast)
const GROQ_API_KEY = process.env.GROQ_API_KEY;
// UPDATED MODELS: Removed decommissioned llama3-70b-8192. Added Llama 3.1 70B & 8B.
const MODEL_PRIORITY = ['llama-3.3-70b-versatile', 'llama-3.1-70b-versatile', 'llama-3.1-8b-instant'];

function toNumber(v) {
    const n = Number(v);
    return Number.isFinite(n) ? n : 0;
}

function toYmd(v) {
    if (!v) return null;
    const d = (v instanceof Date) ? v : new Date(v);
    if (Number.isNaN(d.getTime())) return null;
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
}

function summarizeSalesRows(rows, { topItems = 15, topCustomers = 10 } = {}) {
    const totals = { revenue: 0, qty: 0, profit: 0 };
    const byDay = new Map();
    const byItem = new Map();
    const byCustomer = new Map();

    for (const r of rows || []) {
        const dateKey = toYmd(r.Expr1 || r.InvoiceDate || r.date);
        const itemName = String(r.Expr4 || r.Description || r.ItemName || r.item || '').trim();
        const customerName = String(r.Expr10 || r.CustomerName || r.customer || '').trim();
        const qty = toNumber(r.Expr5 ?? r.Qty ?? r.qty);
        const amount = toNumber(r.Expr12 ?? r.Amount ?? r.amount);
        const profit = toNumber(r.Profit ?? r.profit);

        totals.revenue += amount;
        totals.qty += qty;
        totals.profit += profit;

        if (dateKey) {
            const cur = byDay.get(dateKey) || { date: dateKey, revenue: 0, qty: 0, profit: 0 };
            cur.revenue += amount;
            cur.qty += qty;
            cur.profit += profit;
            byDay.set(dateKey, cur);
        }

        if (itemName) {
            const cur = byItem.get(itemName) || { item: itemName, revenue: 0, qty: 0, profit: 0 };
            cur.revenue += amount;
            cur.qty += qty;
            cur.profit += profit;
            byItem.set(itemName, cur);
        }

        if (customerName) {
            const cur = byCustomer.get(customerName) || { customer: customerName, revenue: 0, qty: 0 };
            cur.revenue += amount;
            cur.qty += qty;
            byCustomer.set(customerName, cur);
        }
    }

    const days = Array.from(byDay.values()).sort((a, b) => a.date.localeCompare(b.date));
    const items = Array.from(byItem.values()).sort((a, b) => b.revenue - a.revenue).slice(0, topItems);
    const customers = Array.from(byCustomer.values()).sort((a, b) => b.revenue - a.revenue).slice(0, topCustomers);

    return {
        totals: {
            revenue: Math.round(totals.revenue * 100) / 100,
            qty: Math.round(totals.qty * 100) / 100,
            profit: Math.round(totals.profit * 100) / 100,
        },
        dailyTotals: days,
        topItems: items,
        topCustomers: customers,
    };
}

function formatMoneyLkr(amount) {
    const n = toNumber(amount);
    try {
        return `Rs. ${new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 }).format(Math.round(n))}`;
    } catch {
        return `Rs. ${Math.round(n)}`;
    }
}

function formatPlainNumber(n) {
    const x = toNumber(n);
    try {
        return new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 }).format(Math.round(x));
    } catch {
        return String(Math.round(x));
    }
}

function buildDeterministicSummaryReport({ userQuery, sqlUsed, summary, wantsSinhala }) {
    const days = Array.isArray(summary?.dailyTotals) ? summary.dailyTotals : [];
    const topItems = Array.isArray(summary?.topItems) ? summary.topItems : [];
    const topCustomers = Array.isArray(summary?.topCustomers) ? summary.topCustomers : [];

    const fromDate = days[0]?.date || null;
    const toDate = days.length ? days[days.length - 1]?.date : null;
    const totalRevenue = summary?.totals?.revenue ?? 0;
    const totalQty = summary?.totals?.qty ?? 0;
    const totalProfit = summary?.totals?.profit ?? 0;

    const avgDaily = days.length ? (toNumber(totalRevenue) / days.length) : 0;
    let bestDay = null;
    let worstDay = null;
    for (const d of days) {
        if (!bestDay || toNumber(d.revenue) > toNumber(bestDay.revenue)) bestDay = d;
        if (!worstDay || toNumber(d.revenue) < toNumber(worstDay.revenue)) worstDay = d;
    }

    const top5 = topItems.slice(0, 5);
    const top3Cust = topCustomers.slice(0, 3);

    if (wantsSinhala) {
        const header = `🧠 **Int මාසික වාර්තාව (Auto Summary)**`;
        const rangeLine = (fromDate && toDate) ? `📅 කාල සීමාව: ${fromDate} සිට ${toDate} දක්වා` : `📅 කාල සීමාව: (දත්ත අනුව)`;

        const exec = [
            `1) **📊 සාරාංශය**`,
            `- එකතුව විකිණීම් (Revenue): **${formatMoneyLkr(totalRevenue)}**`,
            `- එකතුව Qty: **${formatPlainNumber(totalQty)}**`,
            totalProfit ? `- එකතුව ලාභය (Profit): **${formatMoneyLkr(totalProfit)}**` : `- ලාභය: (මෙම දත්තයෙන් නිරවද්‍ය ලෙස ගණනය කිරීමට තවමත් column නොමැති විය හැක.)`,
            `- සාමාන්‍ය දෛනික විකිණීම්: **${formatMoneyLkr(avgDaily)}**`,
        ].join('\n');

        const trend = [
            `2) **🧐 දෛනික රටාව/ප්‍රවණතාවය**`,
            bestDay ? `- වැඩිම දවස: **${bestDay.date}** (${formatMoneyLkr(bestDay.revenue)})` : `- වැඩිම දවස: N/A`,
            worstDay ? `- අඩුම දවස: **${worstDay.date}** (${formatMoneyLkr(worstDay.revenue)})` : `- අඩුම දවස: N/A`,
        ].join('\n');

        const top = [
            `3) **🏆 Top Items (Revenue අනුව)**`,
            ...(top5.length
                ? top5.map((x, i) => `- ${i + 1}. ${x.item} — ${formatMoneyLkr(x.revenue)} | Qty ${formatPlainNumber(x.qty)}`)
                : [`- (Top items දත්ත නැත)`]),
        ].join('\n');

        const cust = [
            `4) **👥 Top Customers (Revenue අනුව)**`,
            ...(top3Cust.length
                ? top3Cust.map((x, i) => `- ${i + 1}. ${x.customer} — ${formatMoneyLkr(x.revenue)} | Qty ${formatPlainNumber(x.qty)}`)
                : [`- (Customer දත්ත නැත)`]),
        ].join('\n');

        const rec = [
            `5) **🚀 නිර්දේශ**`,
            `- Top items සඳහා stock levels පරීක්ෂා කර **ඉක්මනින් reorder** කරගන්න.`,
            `- අඩු විකිණීම් ඇති දින (low days) වල promotions/WhatsApp reminders එකක් plan කරන්න.`,
            `- ඔබට අවශ්‍ය නම් මේ වාර්තාව **Brand අනුව / Category අනුව**ත් කඩලා දෙන්න පුළුවන්.`,
        ].join('\n');

        const footer = `\n—\n(මෙය AI වලට දත්ත ගොඩක් වැඩි වීම නිසා Auto Summary එකක්. අවශ්‍ය නම් සතියෙන්-සතියට breakdown එකක් දෙන්න පුළුවන්.)`;
        return [header, rangeLine, '', exec, '', trend, '', top, '', cust, '', rec, footer].join('\n');
    }

    const header = `🧠 **Int Monthly Report (Auto Summary)**`;
    const rangeLine = (fromDate && toDate) ? `📅 Range: ${fromDate} to ${toDate}` : `📅 Range: (derived from data)`;
    const exec = [
        `1) **📊 Executive Summary**`,
        `- Total Revenue: **${formatMoneyLkr(totalRevenue)}**`,
        `- Total Qty: **${formatPlainNumber(totalQty)}**`,
        totalProfit ? `- Total Profit: **${formatMoneyLkr(totalProfit)}**` : `- Profit: (not fully available from current columns)`,
        `- Avg Daily Revenue: **${formatMoneyLkr(avgDaily)}**`,
    ].join('\n');
    const trend = [
        `2) **🧐 Trend**`,
        bestDay ? `- Best day: **${bestDay.date}** (${formatMoneyLkr(bestDay.revenue)})` : `- Best day: N/A`,
        worstDay ? `- Worst day: **${worstDay.date}** (${formatMoneyLkr(worstDay.revenue)})` : `- Worst day: N/A`,
    ].join('\n');
    const top = [
        `3) **🏆 Top Items (by revenue)**`,
        ...(top5.length
            ? top5.map((x, i) => `- ${i + 1}. ${x.item} — ${formatMoneyLkr(x.revenue)} | Qty ${formatPlainNumber(x.qty)}`)
            : [`- (No top item data)`]),
    ].join('\n');
    const rec = [
        `4) **🚀 Recommendations**`,
        `- Check stock and reorder for top-selling items.`,
        `- Consider promotions on low-sales days.`,
        `- If you want, I can break this down by brand/category.`
    ].join('\n');

    return [header, rangeLine, '', exec, '', trend, '', top, '', rec].join('\n');
}

function startOfLocalDay(d) {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
}

function addDays(d, days) {
    const x = new Date(d);
    x.setDate(x.getDate() + days);
    return x;
}

function formatYmd(d) {
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
}

function normalizeUserQuery(raw) {
    return String(raw || '')
        .replace(/^\s*(int|report|forecast|analyze)\b\s*/i, '')
        .trim();
}

function inferDateRangeFromQuery(raw) {
    const q = normalizeUserQuery(raw).toLowerCase();
    const now = new Date();
    const today0 = startOfLocalDay(now);

    // Sinhala/English quick intents
    const hasToday = /\b(ada|today|අද)\b/i.test(q);
    const hasYesterday = /\b(iye|yesterday|ඊයේ)\b/i.test(q);
    const hasThisMonth = /(me\s*masa|this\s*month|මෙම\s*මාස)/i.test(q);
    const hasLastMonth = /(pasu\s*giya\s*masa|last\s*month|පසුගිය\s*මාස)/i.test(q);
    const hasThisWeek = /(me\s*sathiya|this\s*week|මෙම\s*සතිය)/i.test(q);
    const hasLastWeek = /(pasu\s*giya\s*sathiya|last\s*week|පසුගිය\s*සතිය)/i.test(q);

    // Explicit date: YYYY-MM-DD
    const isoDate = q.match(/\b(\d{4})-(\d{2})-(\d{2})\b/);
    if (isoDate) {
        const [_, y, m, d] = isoDate;
        const dt = new Date(Number(y), Number(m) - 1, Number(d));
        const start = startOfLocalDay(dt);
        return { start, endExclusive: addDays(start, 1), reason: 'explicit_date' };
    }

    // Explicit date: DD/MM/YYYY or DD-MM-YYYY (Sri Lanka style)
    const slDate = q.match(/\b(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{4})\b/);
    if (slDate) {
        const [_, dd, mm, yyyy] = slDate;
        const dt = new Date(Number(yyyy), Number(mm) - 1, Number(dd));
        const start = startOfLocalDay(dt);
        return { start, endExclusive: addDays(start, 1), reason: 'explicit_date' };
    }

    if (hasYesterday) {
        const start = addDays(today0, -1);
        return { start, endExclusive: today0, reason: 'yesterday' };
    }
    if (hasToday) {
        return { start: today0, endExclusive: addDays(today0, 1), reason: 'today' };
    }
    if (hasLastMonth) {
        const start = new Date(today0.getFullYear(), today0.getMonth() - 1, 1);
        const endExclusive = new Date(today0.getFullYear(), today0.getMonth(), 1);
        return { start: startOfLocalDay(start), endExclusive: startOfLocalDay(endExclusive), reason: 'last_month' };
    }
    if (hasThisMonth) {
        const start = new Date(today0.getFullYear(), today0.getMonth(), 1);
        return { start: startOfLocalDay(start), endExclusive: addDays(today0, 1), reason: 'this_month' };
    }
    if (hasLastWeek || hasThisWeek) {
        // Monday as start-of-week (Sri Lanka business norm)
        const day = (today0.getDay() + 6) % 7; // Monday=0
        const startThisWeek = addDays(today0, -day);
        if (hasLastWeek) {
            const start = addDays(startThisWeek, -7);
            const endExclusive = startThisWeek;
            return { start, endExclusive, reason: 'last_week' };
        }
        return { start: startThisWeek, endExclusive: addDays(today0, 1), reason: 'this_week' };
    }

    return null;
}

function containsSinhala(text) {
    return /[\u0D80-\u0DFF]/.test(String(text || ''));
}

function sinhalaCharRatio(text) {
    const s = String(text || '');
    if (!s) return 0;
    const chars = [...s];
    const total = chars.length || 1;
    const si = chars.filter(ch => /[\u0D80-\u0DFF]/.test(ch)).length;
    return si / total;
}

function shouldTranslateToSinhala(text) {
    // Avoid double-translation if the output is already mostly Sinhala.
    return sinhalaCharRatio(text) < 0.08;
}

class IntBusinessMind {
    constructor() {
        this.name = JOB_NAME;
        this.description = 'Advanced Business Intelligence & Predictive Analytics';
        this.priority = 'HIGH';

        // Lazy-detected sales view column mode
        // 'expr' => Expr1/Expr2/... style
        // 'named' => InvoiceDate/InvoiceNo/... style
        this._salesViewMode = null;
        
        // This job handles messages that start with "Int" or "Report" from admins
        this.patterns = [
            /^\s*Int\b/i,
            /^\s*Report\b/i,
            /^\s*Forecast\b/i,
            /^\s*Analyze\b/i
        ];
        
        console.log(`🧠 [Int] Business Intelligence System Loaded.`);
    }

    async detectSalesViewMode() {
        if (this._salesViewMode) return this._salesViewMode;
        try {
            const rows = await executeSafely('SELECT TOP 1 * FROM [View_Sales report whatsapp]');
            const first = rows && rows[0] ? rows[0] : null;
            const keys = first ? Object.keys(first) : [];
            if (keys.some(k => /^Expr1$/i.test(k))) {
                this._salesViewMode = 'expr';
            } else if (keys.some(k => /^InvoiceDate$/i.test(k))) {
                this._salesViewMode = 'named';
            } else {
                // Default to expr (most strict) if uncertain
                this._salesViewMode = 'expr';
            }
        } catch (e) {
            // If we cannot probe (e.g. DB temporary issue), stay on safest default
            this._salesViewMode = 'expr';
        }
        return this._salesViewMode;
    }

    getSalesViewColumns(mode) {
        if (mode === 'named') {
            return {
                invoiceDate: 'InvoiceDate',
                invoiceNo: 'InvoiceNo',
                description: 'Description',
                qty: 'Qty',
                unitPrice: 'UnitPrice',
                amount: 'Amount',
                unitCost: 'UnitCost',
                isVoid: 'IsVoid',
                category: 'Categoty',
                customerName: 'CustomerName',
                vehicleNo: 'VehicleNo'
            };
        }
        return {
            invoiceDate: 'Expr1',
            invoiceNo: 'Expr2',
            description: 'Expr4',
            qty: 'Expr5',
            unitPrice: 'Expr6',
            amount: 'Expr12',
            unitCost: 'UnitCost',
            isVoid: 'Expr14',
            category: 'Categoty',
            customerName: 'Expr10',
            vehicleNo: 'Expr8'
        };
    }

    rewriteSalesSqlColumns(sql, cols) {
        // If model used named columns but view is expr (or vice versa), rewrite conservatively.
        // Applies only when the sales view is referenced.
        let out = String(sql || '');
        if (!/\[View_Sales report whatsapp\]/i.test(out)) return out;

        const map = {
            InvoiceDate: cols.invoiceDate,
            InvoiceNo: cols.invoiceNo,
            Description: cols.description,
            Qty: cols.qty,
            UnitPrice: cols.unitPrice,
            Amount: cols.amount,
            IsVoid: cols.isVoid,
            CustomerName: cols.customerName,
            VehicleNo: cols.vehicleNo
        };

        // Whole-word replacement; keeps casing irrelevant for SQL Server.
        for (const [from, to] of Object.entries(map)) {
            const re = new RegExp(`\\b${from}\\b`, 'gi');
            out = out.replace(re, to);
        }
        return out;
    }

    enforceSalesFilters(sql, cols, dateRange) {
        let out = String(sql || '').trim().replace(/;\s*$/, '');
        if (!/\[View_Sales report whatsapp\]/i.test(out)) return out;

        const clauseBreak = /\b(group\s+by|order\s+by|having|union)\b/i;
        const match = out.match(clauseBreak);
        const splitAt = match && typeof match.index === 'number' ? match.index : -1;

        const head = splitAt >= 0 ? out.slice(0, splitAt).trimEnd() : out;
        const tail = splitAt >= 0 ? out.slice(splitAt) : '';

        const requiredParts = [];

        // Category filter (typo column name in DB: Categoty)
        if (!/\bCategoty\b\s*=\s*'TYRES'/i.test(out)) {
            requiredParts.push("Categoty = 'TYRES'");
        }

        // Void filter
        const isVoidCol = cols.isVoid;
        const voidRegex = new RegExp(`\\b${isVoidCol}\\b\\s*=\\s*0`, 'i');
        const voidNullRegex = new RegExp(`\\b${isVoidCol}\\b\\s+IS\\s+NULL`, 'i');
        if (!voidRegex.test(out) && !voidNullRegex.test(out)) {
            requiredParts.push(`(${isVoidCol} = 0 OR ${isVoidCol} IS NULL)`);
        }

        // Date filter (only if we can infer one)
        if (dateRange) {
            const dcol = cols.invoiceDate;
            const hasWhere = /\bwhere\b/i.test(head);
            const whereSection = hasWhere ? head.slice(head.toLowerCase().indexOf('where')) : '';
            const hasDateInWhere = new RegExp(`\\b${dcol}\\b`, 'i').test(whereSection);
            if (!hasDateInWhere) {
                const startStr = formatYmd(dateRange.start);
                const endStr = formatYmd(dateRange.endExclusive);
                requiredParts.push(`${dcol} >= '${startStr}' AND ${dcol} < '${endStr}'`);
            }
        }

        if (!requiredParts.length) return out;

        const hasWhere = /\bwhere\b/i.test(head);
        const injected = hasWhere
            ? `${head} AND ${requiredParts.join(' AND ')} ${tail}`
            : `${head} WHERE ${requiredParts.join(' AND ')} ${tail}`;

        return injected.trim();
    }

    // Main Entry Point
    async handle(messageObj, client) {
        const { from, body } = messageObj;

        let wantsSinhala = containsSinhala(body);
        
        // Normalize sender (remove @c.us, @s.whatsapp.net, remove spaces)
        let sender = (from || '').replace('@c.us', '').replace('@s.whatsapp.net', '').replace(/\s+/g, '');

        // Check if sender is in ALLOWED_ADMINS (check both 9477.. and 077..)
        const isAllowed = ALLOWED_ADMINS.includes(sender) || 
                          ALLOWED_ADMINS.some(admin => sender.endsWith(admin) || admin.endsWith(sender));

        if (!isAllowed) {
            // Only log if it LOOKS like an Int trigger to avoid spam logs
            if (this.patterns.some(p => p.test(body))) {
                console.log(`⚠️ [Int] Access denied for ${sender} (Not in Admin List)`);
            }
            return false;
        }

        // 2. Identify Trigger
        const triggerMatch = this.patterns.some(p => p.test(body));
        if (!triggerMatch) return false;

        console.log(`🧠 [Int] Processing request from ${sender}: "${body}"`);
        
        // Indicate thinking...
        if (typeof client.sendTyping === 'function') await client.sendTyping(from);

        try {
            // 3. The "Int" Brain Execution Flow
            //    Step A: Understand Intent & Plan SQL
            let planningResult;
            try {
                planningResult = await this.planAndGenerateSql(body);

                // Check language detection
                if (planningResult.detected_language === 'singlish' || planningResult.detected_language === 'sinhala') {
                    console.log(`🧠 [Int] AI detected language: ${planningResult.detected_language}, forcing Sinhala output.`);
                    wantsSinhala = true;
                }
            } catch (planErr) {
                console.error('❌ [Int] Plan Error:', planErr);

                // Fallback: for date-range sales questions, run a safe default sales query.
                const dateRange = inferDateRangeFromQuery(body);
                const looksLikeSales = /\b(sale|sales|report|revenue|income|turnover)\b/i.test(body) || /\b(මාස|විකිණු|විකුණු|රෙපෝට්|වාර්තාව)\b/i.test(body);
                if (dateRange && looksLikeSales) {
                    planningResult = {
                        type: 'sql_query',
                        sql: `SELECT InvoiceDate, InvoiceNo, Description, Qty, UnitPrice, Amount, CustomerName, IsVoid, UnitCost FROM [View_Sales report whatsapp]`,
                        response: ''
                    };
                } else {
                    // As a last resort, return a conversational error response.
                    planningResult = {
                        type: 'conversational',
                        sql: '',
                        response: `🧠 **Int**: මට මේ request එක plan කරගන්න අමාරු වුණා.\n\nකරුණාකර මෙහෙම කියලා try කරන්න:\n- "Int 2026-01-01 to 2026-01-31 sales summary"\n- "Int අද sales කීයද?"`
                    };
                }
            }
            
            if (planningResult.type === 'conversational') {
                // Just a chat/consultation without DB
                let replyText = planningResult.response;
                if (wantsSinhala && shouldTranslateToSinhala(replyText)) {
                    replyText = await this.translateToSinhala(replyText);
                }
                await client.sendMessage(from, replyText);
                return true;
            }

            //    Step B: Execute SQL (Read-Only)
            let dbData = null;
            if (planningResult.sql) {
                const dateRange = inferDateRangeFromQuery(body);
                const mode = await this.detectSalesViewMode();
                const cols = this.getSalesViewColumns(mode);

                let sqlToRun = planningResult.sql;
                sqlToRun = this.rewriteSalesSqlColumns(sqlToRun, cols);
                sqlToRun = this.enforceSalesFilters(sqlToRun, cols, dateRange);

                console.log(`🧠 [Int] Executing SQL: ${sqlToRun}`);
                dbData = await executeSafely(sqlToRun);

                // Keep sqlUsed aligned with what actually ran
                planningResult.sql = sqlToRun;
            }

            //    Step C: Analyze Data & Formulate Strategic Advice
            let finalReport = await this.analyzeAndReport(body, planningResult.sql, dbData);
            console.log(`🧠 [Int] Report Generated (Length: ${finalReport ? finalReport.length : 0})`);

            if (wantsSinhala && shouldTranslateToSinhala(finalReport)) {
                console.log(`🇱🇰 [Int] Translating report to Sinhala...`);
                const translated = await this.translateToSinhala(finalReport);
                if (translated && translated.trim().length > 0) {
                    finalReport = translated;
                    console.log(`✅ [Int] Translation success (Length: ${finalReport.length})`);
                } else {
                    console.warn(`⚠️ [Int] Translation returned empty! Falling back to English.`);
                    finalReport = "> *[Translation Failed - Showing Original]*\n\n" + finalReport;
                }
            }
            
            //    Step D: Send Response
            if (!finalReport || finalReport.trim().length === 0) {
                console.error('❌ [Int] Final report is empty! Aborting send.');
                await client.sendMessage(from, "🧠 **Int Error:** The report was generated but came out empty. Please try again.");
            } else {
                console.log(`📤 [Int] Sending response to ${from}...`);
                await client.sendMessage(from, finalReport);
                console.log(`✅ [Int] Message sent.`);
            }
            return true;

        } catch (error) {
            console.error('❌ [Int] Error:', error);
            await client.sendMessage(from, `🧠 **Int System Error**\nI encountered an issue analyzing that.\n\nError: ${error.message}`);
            return true;
        }
    }

    // ------------------------------------------------------------------------
    // AI CORE METHODS
    // ------------------------------------------------------------------------

    // Robust Generation with Fallback Strategy (Groq/Llama 3)
    async generateWithFallback(systemInstruction, userMessage, options = {}) {
        let lastError = null;

        const temperature = typeof options.temperature === 'number' ? options.temperature : 0.1;
        const max_tokens = typeof options.max_tokens === 'number' ? options.max_tokens : 4096;

        // Initialize Groq Client
        const groq = new Groq({ apiKey: GROQ_API_KEY });

        for (const modelName of MODEL_PRIORITY) {
            try {
                // console.log(`🧠 [Int] Trying Groq model: ${modelName}`);
                
                const completion = await groq.chat.completions.create({
                    messages: [
                        { role: "system", content: systemInstruction },
                        { role: "user", content: userMessage }
                    ],
                    model: modelName,
                    temperature,
                    max_tokens,
                });

                const text = completion.choices[0]?.message?.content;
                if (text) return text;
                
            } catch (e) {
                console.warn(`⚠️ [Int] Groq Model ${modelName} failed: ${e.message ? e.message.split(' ')[0] : 'Unknown Error'}`);
                lastError = e;
                // Wait briefly before retry to avoid instant loops on network glitches
                await new Promise(r => setTimeout(r, 1000));
            }
        }

        // If all failed
        throw new Error(`All AI models failed. Last error: ${lastError ? lastError.message : 'Unknown'}`);
    }

    async translateToSinhala(text) {
        const input = String(text || '').trim();
        if (!input) return input;

        const systemPrompt = `You are a native Sri Lankan business professional.
Translate the message into clear, formal, and authoritative Sinhala (Sri Lankan).

Rules:
1. **Style:** Use professional "office Sinhala" (Formal/Written style). Avoid "Google Translate" or robotic phrasing.
   - Bad: "මෙය කලබලය ජනකයි" (Robotic) -> Good: "මෙම තත්වය ගැටළුකාරී විය හැක."
   - Bad: "විකිණීම් පහල වැටී ඇත" -> Good: "විකුණුම් වල සැලකිය යුතු පහත වැටීමක් පෙන්නුම් කරයි."
2. **Numbers & Entities:** Keep ALL numbers, dates, currency (LKR), English technical terms (Profit Margin, Stockout, Invoice) exactly as is.
3. **Structure:** Keep newlines and bullet points exactly as in the original.
4. **Output:** ONLY the translated Sinhala text. No explanations.`;

        return await this.generateWithFallback(systemPrompt, input, { temperature: 0.2, max_tokens: 4096 });
    }

    async planAndGenerateSql(userQuery) {

        const dateRange = inferDateRangeFromQuery(userQuery);
        const dateContext = dateRange
            ? `\n**Date Range (already inferred from the user's wording):**\n- start (inclusive): ${formatYmd(dateRange.start)}\n- end (exclusive): ${formatYmd(dateRange.endExclusive)}\n**IMPORTANT:** If you query sales, you MUST filter by the date column using this range.\n`
            : `\n**Date Range:** Not explicitly provided / not inferred. If the question is time-based, ask a clarifying question instead of guessing.\n`;
        
        const systemPrompt = `
You are **Int**, the highly advanced Business Intelligence Mind for 'Lasantha Tyre Traders'.
Your role is to act as a **Strategic Business Consultant** and **Data Scientist**.

**Your Capabilities:**
1.  **Read-Only DB Access:** You can generate MSSQL queries to fetch data.
2.  **Strategic Insight:** You don't just show numbers; you explain *why* and suggest *next steps*.
3.  **Predictive Thinking:** You look for trends to forecast future performance.

**Database Schema (COMPREHENSIVE VIEW LIST):**
You have access to the following Views. Use them intelligently. 

**1. SALES & PROFIT (Primary Source - MATCHING DAILY REPORT LOGIC)**
- **[View_Sales report whatsapp]**: The OFFICIAL source for Daily Sales Reports.
    - **CRITICAL:** This view has obscured column names. You MUST use these aliases for queries to work.
    - **Columns Mapping:** 
        - \`Expr1\` = InvoiceDate (DateTime)
        - \`Expr2\` = InvoiceNo
        - \`Expr4\` = Description
        - \`Expr5\` = Qty
        - \`Expr6\` = UnitPrice
        - \`Expr12\` = Amount (SubTotal)
        - \`UnitCost\` = UnitCost
        - \`Expr14\` = IsVoid (0 = Valid, 1 = Void)
        - \`Categoty\` = Category ('TYRES')
        - \`Expr10\` = CustomerName
        - \`Expr8\` = VehicleNo
    - **IMPORTANT (NULL-SAFE):** Always wrap numeric fields with \`ISNULL(x,0)\` (or \`COALESCE(x,0)\`).
    - **Profit Formula (NULL-SAFE):** \`SUM(ISNULL(Expr12,0) - (ISNULL(Expr5,0) * ISNULL(UnitCost,0)))\`
    - **Total Sales (NULL-SAFE):** \`SUM(ISNULL(Expr12,0))\`
    - **Total Units (NULL-SAFE):** \`SUM(ISNULL(Expr5,0))\`
    - **IMPORTANT FILTERS:** Always apply: \`WHERE Categoty = 'TYRES' AND (Expr14 = 0 OR Expr14 IS NULL)\`

**2. INVENTORY & STOCK**
- **[View_Item Whse Whatsapp]**: Real-time Stock Levels.
    - **Use this for:** Stock checks, Inventory value.
    - **Cols:** WhseName, ItemId, ItemDis, QTY (OnHand), UnitCost, TotalCost, OPBQtry.

**3. CUSTOMERS & OUTSTANDING**
- **[view_inv]**: Customer details & Invoice Headers.
    - **Cols:** InvoiceNo, CustomerName, Address1, Address2.
- **[View_LoadCustomer]**: Customer Registry.
    - **Cols:** Name, Address1, ContactNo, NIC.

**4. SUPPLIERS & PURCHASES**
- **[View_DirectSupplierInvoicesWhatsapp]**: Supplier Invoices.
    - **Cols:** InvoiceDate, Description, ItemID, Qty.
- **[View_LastPurchaseDate]**: Last buying date per item.
    - **Cols:** ItemID, Description, LastPurDate.

**5. QUOTATIONS**
- **[View_Quotation_WhatsApp]**: Sent Quotations.
    - **Cols:** Date, SalesOrderNo, CustomerID, ItemID, Description, UnitPrice, Amount, VehicleNo.

**Instructions:**
Analyze the user's request ("${userQuery}").
Select the best View(s) to answer the question.
${dateContext}

- **For SALES/PROFIT/GROWTH (Standard):**
  - Use: **[View_Sales report whatsapp]**
  - Query: 
    \`\`\`sql
    SELECT 
        Expr1 as InvoiceDate, 
        Expr2 as InvoiceNo, 
        Expr4 as Description, 
        Expr12 as Amount, 
        (Expr12 - (Expr5 * UnitCost)) as Profit
    FROM [View_Sales report whatsapp] 
    WHERE Categoty = 'TYRES' 
      AND (Expr14 = 0 OR Expr14 IS NULL)
      AND Expr1 >= '...'
    \`\`\`

- **For STOCK:** Use [View_Item Whse Whatsapp].

- **Date Filtering:** ALWAYS filter by \`Expr1\` (Date) when using sales view.

**Output Format:**
Return a **valid JSON object** ONLY. No markdown.
{
    "type": "sql_query" | "conversational",
    "sql": "SELECT TOP 10 ...", 
    "response": "...",
    "detected_language": "english" | "sinhala" | "singlish"
}
`;

        const userTask = `Analyze this request: "${userQuery}".
Select the best View(s) to answer the question.
    ${dateContext}

- **For SALES/PROFIT/GROWTH (Standard):**
  - Use: **[View_Sales report whatsapp]**
  - Query: 
    \`\`\`sql
    SELECT 
        Expr1 as InvoiceDate, 
        Expr2 as InvoiceNo, 
        Expr4 as Description, 
        Expr12 as Amount, 
        (Expr12 - (Expr5 * UnitCost)) as Profit
    FROM [View_Sales report whatsapp] 
    WHERE Categoty = 'TYRES' 
      AND (Expr14 = 0 OR Expr14 IS NULL)
      AND Expr1 >= '...'
    \`\`\`

- **For STOCK:** Use [View_Item Whse Whatsapp].`;

        let textRaw = '';
        try {
            textRaw = await this.generateWithFallback(systemPrompt, userTask);
            // Robust JSON extraction for Llama 3
            let jsonString = textRaw.trim();
            const firstOpen = jsonString.indexOf('{');
            const lastClose = jsonString.lastIndexOf('}');
            if (firstOpen !== -1 && lastClose !== -1) {
                jsonString = jsonString.substring(firstOpen, lastClose + 1);
            }
            try {
                return JSON.parse(jsonString);
            } catch (parseErr) {
                // Self-heal: ask model to convert to strict JSON only.
                const fixerSystem = `You are a strict JSON formatter.
Convert the user's text into ONE valid JSON object exactly matching this schema:
{
  "type": "sql_query" | "conversational",
  "sql": string | "",
  "response": string,
  "detected_language": "english" | "sinhala" | "singlish"
}

Rules:
- Output ONLY the JSON object. No markdown.
- If the input contains a SQL query, put it in "sql".
- If no SQL is appropriate, set type="conversational" and sql="".
- Always include a helpful "response".
- Detect language: "singlish" if Sinhala words in English letters.`;

                const fixed = await this.generateWithFallback(fixerSystem, `TEXT:\n${textRaw}`, { temperature: 0.0, max_tokens: 1200 });
                let fixedJson = String(fixed || '').trim();
                const fo = fixedJson.indexOf('{');
                const lc = fixedJson.lastIndexOf('}');
                if (fo !== -1 && lc !== -1) fixedJson = fixedJson.substring(fo, lc + 1);
                return JSON.parse(fixedJson);
            }
        } catch (e) {
            console.error('Plan Error:', e);
            console.error('Raw Response:', textRaw);
            throw new Error("I couldn't plan the data analysis. Please try again.");
        }
    }

    async analyzeAndReport(userQuery, sqlUsed, data) {
        const rows = Array.isArray(data) ? data : [];
        const wantsSinhala = typeof containsSinhala === 'function' ? containsSinhala(userQuery) : false;

        const buildPrompts = (payload, { summarized } = {}) => {
            const systemPrompt = `
You are **Int**, the Supreme Business Intelligence Brain.
You have successfully executed a database query based on the user's request.

**User Request:** "${userQuery}"
**SQL Used:** ${sqlUsed}
**Data Retrieved:** ${JSON.stringify(payload).substring(0, 15000)} (Truncated if too long)

**Your Task:**
Generate a **Professional, Strategic Business Report**.

**Report Structure:**
1.  **📊 Executive Summary:** Direct answer to the question with key numbers.
2.  **🧐 Deep Analysis:**
    - Identify trends (Upward/Downward).
    - Compare with expectations.
    - Explain *possible reasons* for performance.
3.  **🔮 Predictive Insight (The "Int" Speciality):**
    - Based on this data, what will likely happen next week/month?
    - Is there a risk (e.g., Stockout)?
4.  **🚀 Strategic Recommendations:**
    - What should the admin do? (e.g., "Discount this item", "Order more of Brand X", "Call these lapsed customers").

**Tone:**
- Highly Professional, Intelligent, Confident.
- Use "We" when referring to the business intelligence.
- Be concise but deep.

**Language Rule:**
- If the user request is in Sinhala, write the report in Sinhala (Sri Lankan).
- Otherwise, write in English.

**Formatting:**
- Use emojis effectively.
- Use Bold for emphasis.
- If data is empty, analyze *why* (e.g., "No sales found for this period. This is concerning...").
`;

            const userTask = summarized
                ? `Generate the report using the provided summary tables (daily totals + top items/customers).`
                : `Generate the report using the provided row-level data.`;

            return { systemPrompt, userTask };
        };

        const summarizePayload = () => ({
            _note: `Summarized from ${rows.length} rows to avoid AI context limits.`,
            summary: summarizeSalesRows(rows),
        });

        // Heuristic: month/date-range queries often produce big row sets.
        // Use a low threshold, and also keep a retry path if the first attempt fails.
        const initialShouldSummarize = rows.length > 200;
        const initialPayload = initialShouldSummarize ? summarizePayload() : data;

        try {
             const { systemPrompt, userTask } = buildPrompts(initialPayload, { summarized: initialShouldSummarize });
             return await this.generateWithFallback(systemPrompt, userTask);
        } catch (e) {
             console.error('Analysis Error (attempt 1):', e);

             // If we didn't summarize yet, retry once with a summarized payload.
             if (!initialShouldSummarize && rows.length) {
                 try {
                     console.warn('⚠️ [Int] Retrying analysis with summarized payload...');
                     const summarizedPayload = summarizePayload();
                     const { systemPrompt, userTask } = buildPrompts(summarizedPayload, { summarized: true });
                     return await this.generateWithFallback(systemPrompt, userTask);
                 } catch (e2) {
                     console.error('Analysis Error (attempt 2 summarized):', e2);
                 }
             }

             // Final fallback: generate a useful report without AI.
             if (rows.length) {
                 try {
                     const summary = summarizeSalesRows(rows);
                     return buildDeterministicSummaryReport({ userQuery, sqlUsed, summary, wantsSinhala });
                 } catch (fallbackErr) {
                     console.error('Deterministic fallback failed:', fallbackErr);
                 }
             }

             return "🧠 **Int Analysis Failed:** I could not process the amount of data retrieved. Try a narrower date range.";
        }
    }
}

module.exports = new IntBusinessMind();
