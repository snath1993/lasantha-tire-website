const sql = require('mssql');
const { normalizeBrand } = require('../utils/brandUtils');
const { getAiRawPool } = require('../utils/aiDbConnection');

// Configuration
const TARGET_STOCK = 4;
const LOOKBACK_HOURS = 48;           // Check sales from last 48 hours to catch missed runs
const COOLDOWN_DAYS = 7;             // Suppress re-order for same ItemCode if already sent within 7 days
const FALLBACK_NUMBER = '94771222509@c.us'; // Admin fallback if group not found

module.exports = async function ReOrderingJob(client, mainPool, aiPool, allowedContacts, logAndSave, options = {}) {
    logAndSave('🚀 Starting Advanced Re-Ordering Job v2.0...');

    const referenceDate = options.referenceDate ? new Date(options.referenceDate) : new Date();
    const referenceDateStr = referenceDate.toISOString().slice(0, 19).replace('T', ' ');

    // ──────────────────────────────────────────────
    // 0. Find target group "Re Order"
    // ──────────────────────────────────────────────
    let targetGroup = null;
    try {
        if (client.getChats) {
            let chats = [];
            for (let i = 0; i < 3; i++) {
                chats = await client.getChats();
                if (chats.length > 0) break;
                logAndSave(`⏳ Waiting for chats to sync (Attempt ${i + 1}/3)...`);
                await new Promise(resolve => setTimeout(resolve, 5000));
            }

            targetGroup = chats.find(chat => chat.isGroup && chat.name.toLowerCase() === 're order');

            if (!targetGroup) {
                logAndSave('⚠️ Warning: WhatsApp Group "Re Order" not found.');
                const availableGroups = chats.filter(c => c.isGroup).map(c => c.name).join(', ');
                logAndSave(`📋 Available Groups: ${availableGroups || 'None'}`);
            } else {
                logAndSave(`✅ Target Group Found: ${targetGroup.name}`);
            }
        } else {
            logAndSave('⚠️ Client does not support getChats (Mock Mode?)');
        }
    } catch (error) {
        logAndSave(`❌ Error finding group: ${error.message}`);
    }

    try {
        // ──────────────────────────────────────────────
        // 1. Fetch Recent Sales from Main DB
        // ──────────────────────────────────────────────
        const HARD_CUTOFF_DATE = '2025-12-13T00:00:00';

        const salesQuery = `
            SELECT 
                d.Expr2 as InvoiceNo,
                d.Expr1 as InvoiceDate,
                d.Expr3 as ItemCode,
                d.Expr4 as ItemDescription,
                d.Expr5 as SoldQty,
                im.Categoty as Category,
                im.Custom3 as Brand,
                iw.QTY as CurrentStock
            FROM [View_Sales report whatsapp] d
            JOIN [View_Item Master Whatsapp] im ON d.Expr3 = im.ItemID
            JOIN [View_Item Whse Whatsapp] iw ON d.Expr3 = iw.ItemID
            WHERE d.Expr1 >= DATEADD(hour, -${LOOKBACK_HOURS}, @ReferenceDate)
            AND d.Expr1 <= @ReferenceDate
            AND d.Expr1 >= @HardCutoffDate
            AND im.Categoty IN ('TYRES', 'MOTORBIKE TYRES')
            ORDER BY d.Expr1 DESC
        `;

        const request = mainPool.request();
        request.input('ReferenceDate', sql.DateTime, referenceDate);
        request.input('HardCutoffDate', sql.DateTime, new Date(HARD_CUTOFF_DATE));
        const salesResult = await request.query(salesQuery);
        const allSales = salesResult.recordset;

        if (allSales.length === 0) {
            logAndSave('ℹ️ No sales found in the last 48 hours.');
            return;
        }

        // ──────────────────────────────────────────────
        // 2. Filter out already processed invoices
        // ──────────────────────────────────────────────
        const historyQuery = `
            SELECT InvoiceNo, ItemCode FROM [WhatsAppAI].[dbo].[ReOrderHistory]
            WHERE ProcessedAt >= DATEADD(hour, -${LOOKBACK_HOURS + 24}, @ReferenceDate)
        `;
        const historyRequest = aiPool.request();
        historyRequest.input('ReferenceDate', sql.DateTime, referenceDate);
        const historyResult = await historyRequest.query(historyQuery);
        const processedSet = new Set(historyResult.recordset.map(r => `${r.InvoiceNo}_${r.ItemCode}`));

        const pendingItems = allSales.filter(item => !processedSet.has(`${item.InvoiceNo}_${item.ItemCode}`));

        if (pendingItems.length === 0) {
            logAndSave('✅ All recent sales have already been processed.');
            return;
        }

        logAndSave(`🔍 Found ${pendingItems.length} pending items to process.`);

        // ──────────────────────────────────────────────
        // 2b. Cooldown Check — fetch items SENT in last 7 days (by ItemCode)
        //     Used to tag repeat requests + suppress spam
        // ──────────────────────────────────────────────
        let recentlySentMap = new Map(); // ItemCode -> { lastSentDate, sendCount }
        try {
            const cooldownQuery = `
                SELECT ItemCode, ItemDescription, 
                       MAX(ProcessedAt) as LastSent,
                       COUNT(*) as SendCount
                FROM [WhatsAppAI].[dbo].[ReOrderHistory]
                WHERE Status = 'SENT'
                AND ProcessedAt >= DATEADD(day, -${COOLDOWN_DAYS}, @ReferenceDate)
                GROUP BY ItemCode, ItemDescription
            `;
            const cooldownReq = aiPool.request();
            cooldownReq.input('ReferenceDate', sql.DateTime, referenceDate);
            const cooldownResult = await cooldownReq.query(cooldownQuery);
            for (const row of cooldownResult.recordset) {
                recentlySentMap.set(row.ItemCode, {
                    lastSent: row.LastSent,
                    sendCount: row.SendCount,
                    daysSinceLast: Math.floor((referenceDate - new Date(row.LastSent)) / (1000 * 60 * 60 * 24))
                });
            }
            logAndSave(`📊 Cooldown data loaded: ${recentlySentMap.size} items sent in last ${COOLDOWN_DAYS} days.`);
        } catch (e) {
            logAndSave(`⚠️ Cooldown query failed (proceeding without): ${e.message}`);
        }

        // ──────────────────────────────────────────────
        // 3. Process Items & Apply Rules
        // ──────────────────────────────────────────────
        const ordersByBrand = {};
        const itemsToSave = [];
        let cooldownSkipped = 0;
        
        // Aggregate items so the message doesn't repeat the same item if sold in multiple recent invoices
        const aggregatedForMessage = new Map();

        for (const item of pendingItems) {
            const brand = normalizeBrand(item.Brand || 'OTHER');
            const isMotorbike = (item.Category && item.Category.toUpperCase().includes('MOTOR')) || false;

            let orderQty = 0;
            let shouldOrder = false;

            if (isMotorbike) {
                orderQty = item.SoldQty;
                shouldOrder = true;
            } else {
                if (item.CurrentStock < TARGET_STOCK) {
                    orderQty = TARGET_STOCK - item.CurrentStock;
                    shouldOrder = true;
                }
            }

            // Check cooldown — if same ItemCode was SENT within COOLDOWN_DAYS, suppress
            const cooldownInfo = recentlySentMap.get(item.ItemCode);
            const isRepeat = !!cooldownInfo;
            const isCooldownActive = cooldownInfo && cooldownInfo.daysSinceLast < 2;

            if (shouldOrder && orderQty > 0) {
                if (isCooldownActive) {
                    // Still save as COOLDOWN_SKIPPED but don't send message
                    itemsToSave.push({ ...item, OrderQty: orderQty, IsMotorbike: isMotorbike, Status: 'COOLDOWN_SKIPPED' });
                    cooldownSkipped++;
                    continue;
                }

                // Prepare for DB Save
                const saveEntry = {
                    ...item,
                    OrderQty: orderQty,
                    IsMotorbike: isMotorbike,
                    Status: 'SENT'
                };
                itemsToSave.push(saveEntry);

                // Aggregate for the WhatsApp Message
                const aggKey = `${brand}_${item.ItemCode}`;
                if (!aggregatedForMessage.has(aggKey)) {
                    aggregatedForMessage.set(aggKey, {
                        brand: brand,
                        Category: item.Category,
                        ItemCode: item.ItemCode,
                        ItemDescription: item.ItemDescription,
                        CurrentStock: item.CurrentStock,
                        OrderQty: isMotorbike ? item.SoldQty : orderQty,
                        SoldQty: item.SoldQty,
                        IsMotorbike: isMotorbike,
                        _isRepeat: isRepeat,
                        _cooldownInfo: cooldownInfo
                    });
                } else {
                    // If it's a motorbike tire, sum up the sold and order amounts
                    const existing = aggregatedForMessage.get(aggKey);
                    existing.SoldQty += item.SoldQty;
                    if (isMotorbike) {
                        existing.OrderQty += item.SoldQty; // order more
                    }
                }
            } else {
                itemsToSave.push({ ...item, OrderQty: 0, IsMotorbike: isMotorbike, Status: 'SKIPPED' });
            }
        }
        
        // Push aggregated items to brand maps
        for (const aggItem of aggregatedForMessage.values()) {
            if (!ordersByBrand[aggItem.brand]) {
                ordersByBrand[aggItem.brand] = { general: [], motorbike: [] };
            }
            if (aggItem.IsMotorbike) {
                ordersByBrand[aggItem.brand].motorbike.push(aggItem);
            } else {
                ordersByBrand[aggItem.brand].general.push(aggItem);
            }
        }

        if (cooldownSkipped > 0) {
            logAndSave(`⏸️ Cooldown suppressed ${cooldownSkipped} items (already sent within 2 days).`);
        }

        // ──────────────────────────────────────────────
        // 4. Generate & Send Advanced Messages
        // ──────────────────────────────────────────────
        let dateStr = new Date().toISOString().split('T')[0];

        if (pendingItems.length > 0) {
            const sortedByDate = [...pendingItems].sort((a, b) => new Date(b.InvoiceDate) - new Date(a.InvoiceDate));
            const d = new Date(sortedByDate[0].InvoiceDate);
            dateStr = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
        }

        // Helper: format a single item line
        function formatItemLine(idx, item, isMotorbike) {
            const repeatBadge = item._isRepeat ? ` [Repeat: ${item._cooldownInfo.sendCount}x]` : '';

            let line = `${idx}. ${item.ItemDescription}${repeatBadge}\n`;
            if (isMotorbike) {
                line += `   > Sold: *${item.SoldQty}* | Stock: *${item.CurrentStock}*\n`;
            } else {
                line += `   > Order: *${item.OrderQty}* | Stock: *${item.CurrentStock}* / ${TARGET_STOCK}\n`;
            }
            return line;
        }

        let totalItemsSent = 0;
        let totalOrderQty = 0;

        for (const [brand, data] of Object.entries(ordersByBrand)) {
            const generalOrders = data.general;
            const motorbikeOrders = data.motorbike;

            if (generalOrders.length === 0 && motorbikeOrders.length === 0) continue;

            // Sort by priority: lowest stock first
            generalOrders.sort((a, b) => a.CurrentStock - b.CurrentStock);
            motorbikeOrders.sort((a, b) => a.CurrentStock - b.CurrentStock);

            const brandTotal = generalOrders.reduce((s, i) => s + i.OrderQty, 0) +
                               motorbikeOrders.reduce((s, i) => s + i.OrderQty, 0);
            const brandItemCount = generalOrders.length + motorbikeOrders.length;
            const repeatCount = [...generalOrders, ...motorbikeOrders].filter(i => i._isRepeat).length;

            // ── Build Classic Professional Message ──
            let message = '';
            message += `*RE-ORDER NOTIFICATION*\n`;
            message += `Brand: *${brand}*\n`;
            message += `Date: ${dateStr}\n`;
            message += `---------------------------------\n\n`;

            if (generalOrders.length > 0) {
                message += `*General Tyres* (${generalOrders.length})\n`;
                generalOrders.forEach((item, idx) => {
                    message += formatItemLine(idx + 1, item, false);
                });
                message += `\n`;
            }

            if (motorbikeOrders.length > 0) {
                message += `*Motorbike Tyres* (${motorbikeOrders.length})\n`;
                motorbikeOrders.forEach((item, idx) => {
                    message += formatItemLine(idx + 1, item, true);
                });
                message += `\n`;
            }

            // ── Summary Footer ──
            message += `---------------------------------\n`;
            message += `*Summary*\n`;
            message += `- Total Items: ${brandItemCount}\n`;
            message += `- Total Order Qty: ${brandTotal}\n`;
            if (repeatCount > 0) {
                message += `- Repeated Requests: ${repeatCount}\n`;
            }
            message += `\n`;

            totalItemsSent += brandItemCount;
            totalOrderQty += brandTotal;

            // ── Send Message ──
            const sendTarget = targetGroup ? targetGroup.id._serialized : FALLBACK_NUMBER;
            const sendLabel = targetGroup ? `group "Re Order"` : `fallback admin`;

            try {
                await client.sendMessage(sendTarget, message);
                logAndSave(`📤 Sent ${brand} re-order alert (${brandItemCount} items) to ${sendLabel}`);
            } catch (err) {
                logAndSave(`❌ Failed to send ${brand} to ${sendLabel}: ${err.message}`);
                // If group send failed, try fallback
                if (targetGroup) {
                    try {
                        await client.sendMessage(FALLBACK_NUMBER, message);
                        logAndSave(`📤 Sent ${brand} to fallback admin after group failure.`);
                    } catch (err2) {
                        logAndSave(`❌ Fallback send also failed: ${err2.message}`);
                    }
                }
            }

            // Small delay between brand messages to avoid rate limiting
            await new Promise(resolve => setTimeout(resolve, 2000));
        }

        // ── Overall Summary (if multiple brands) ──
        const brandCount = Object.keys(ordersByBrand).length;
        if (brandCount > 1 && totalItemsSent > 0) {
            const summaryMsg = `📦 *RE-ORDER COMPLETE*\n\n` +
                `Brands: ${brandCount}\n` +
                `Items: ${totalItemsSent}\n` +
                `Total Qty: ${totalOrderQty}\n` +
                `Cooldown Skipped: ${cooldownSkipped}\n` +
                `Date: ${dateStr}\n` +
                `\n_Automated by Lasantha Tyre Bot_`;

            const summaryTarget = targetGroup ? targetGroup.id._serialized : FALLBACK_NUMBER;
            try {
                await client.sendMessage(summaryTarget, summaryMsg);
                logAndSave(`📤 Sent overall re-order summary.`);
            } catch (e) {
                logAndSave(`⚠️ Failed to send overall summary: ${e.message}`);
            }
        }

        // ──────────────────────────────────────────────
        // 5. Save to Database (ReOrderHistory)
        // ──────────────────────────────────────────────
        if (itemsToSave.length > 0) {
            let aiRawPool = null;
            try {
                aiRawPool = getAiRawPool();
            } catch (e) {
                aiRawPool = aiPool;
            }

            if (!aiRawPool) {
                logAndSave('⚠️ WhatsAppAI pool not available; skipping ReOrderHistory save');
                return;
            }

            const transaction = new sql.Transaction(aiRawPool);
            await transaction.begin();

            try {
                for (const item of itemsToSave) {
                    const normalizedBrand = normalizeBrand(item.Brand || 'OTHER');
                    const req = new sql.Request(transaction);
                    req.input('InvoiceNo', sql.NVarChar(50), item.InvoiceNo);
                    req.input('InvoiceDate', sql.Date, item.InvoiceDate);
                    req.input('ItemCode', sql.NVarChar(50), item.ItemCode);
                    req.input('ItemDescription', sql.NVarChar(200), item.ItemDescription);
                    req.input('Brand', sql.NVarChar(50), normalizedBrand);  // Save NORMALIZED brand
                    req.input('Category', sql.NVarChar(50), item.Category);
                    req.input('SoldQty', sql.Int, item.SoldQty);
                    req.input('StockAtOrder', sql.Int, item.CurrentStock);
                    req.input('OrderedQty', sql.Int, item.OrderQty);
                    req.input('Status', sql.NVarChar(20), item.Status || 'SENT');

                    await req.query(`
                        IF NOT EXISTS (SELECT 1 FROM [WhatsAppAI].[dbo].[ReOrderHistory] WHERE InvoiceNo = @InvoiceNo AND ItemCode = @ItemCode)
                        BEGIN
                            INSERT INTO [WhatsAppAI].[dbo].[ReOrderHistory] (
                                InvoiceNo, InvoiceDate, ItemCode, ItemDescription, Brand, Category,
                                SoldQty, StockAtOrder, OrderedQty, Status
                            ) VALUES (
                                @InvoiceNo, @InvoiceDate, @ItemCode, @ItemDescription, @Brand, @Category,
                                @SoldQty, @StockAtOrder, @OrderedQty, @Status
                            )
                        END
                    `);
                }

                await transaction.commit();
                logAndSave(`💾 Saved ${itemsToSave.length} records to ReOrderHistory.`);
            } catch (err) {
                await transaction.rollback();
                logAndSave(`❌ Database save failed: ${err.message}`);
                throw err;
            }
        }

    } catch (error) {
        logAndSave(`❌ ReOrderingJob Error: ${error.message}`);
    }
};
