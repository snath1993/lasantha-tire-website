const sql = require('mssql');
const { normalizeBrand } = require('../utils/brandUtils');
const { getAiRawPool } = require('../utils/aiDbConnection');

// Configuration
const TARGET_STOCK = 4;
const LOOKBACK_HOURS = 48; // Check sales from last 48 hours to catch missed runs

module.exports = async function ReOrderingJob(client, mainPool, aiPool, allowedContacts, logAndSave, options = {}) {
    logAndSave('🚀 Starting Advanced Re-Ordering Job...');

    const referenceDate = options.referenceDate ? new Date(options.referenceDate) : new Date();
    const referenceDateStr = referenceDate.toISOString().slice(0, 19).replace('T', ' ');

    // Find target group "Re Order"
    let targetGroup = null;
    try {
        if (client.getChats) {
            // Retry logic for fetching chats (wait for sync)
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
                // Log available groups to help debug
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
        // 1. Fetch Recent Sales from Main DB (LasanthaTyre)
        // We get sales from the last 48 hours relative to referenceDate
        // BUT we also enforce a hard cutoff date (2025-12-13) to ignore older data as per user request.
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

        // 2. Filter out already processed items from AI DB (WhatsAppAI)
        // We check ReOrderHistory table in WhatsAppAI database
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
        if (pendingItems.length > 0) {
            logAndSave(`📅 First Item Date: ${pendingItems[0].InvoiceDate}`);
            logAndSave(`📅 Last Item Date: ${pendingItems[pendingItems.length - 1].InvoiceDate}`);
        }

        // 3. Process Items & Apply Rules
        const ordersByBrand = {};
        const itemsToSave = [];

        for (const item of pendingItems) {
            const brand = normalizeBrand(item.Brand || 'OTHER');
            const isMotorbike = (item.Category && item.Category.toUpperCase().includes('MOTOR')) || false;
            
            let orderQty = 0;
            let shouldOrder = false;

            if (isMotorbike) {
                // Rule: Motorbike tyres -> Report Sold Qty directly
                orderQty = item.SoldQty;
                shouldOrder = true;
            } else {
                // Rule: General Tyres -> Target Stock 4
                // If Current Stock < 4, Order = 4 - Current Stock
                // Note: CurrentStock is the LIVE stock right now.
                if (item.CurrentStock < TARGET_STOCK) {
                    orderQty = TARGET_STOCK - item.CurrentStock;
                    shouldOrder = true;
                }
            }

            if (shouldOrder && orderQty > 0) {
                if (!ordersByBrand[brand]) {
                    ordersByBrand[brand] = {
                        general: [],
                        motorbike: []
                    };
                }

                const entry = {
                    ...item,
                    OrderQty: orderQty,
                    IsMotorbike: isMotorbike
                };

                if (isMotorbike) {
                    ordersByBrand[brand].motorbike.push(entry);
                } else {
                    ordersByBrand[brand].general.push(entry);
                }

                itemsToSave.push(entry);
            } else {
                // Even if we don't order (e.g. stock is sufficient), we should mark it as processed
                // so we don't check it again. But the user requirement implies we only care about re-ordering.
                // However, to be "Zero Miss", we should probably record it as processed with 0 order qty
                // if we want to avoid re-calculating. But for now, let's only save what we act on,
                // OR save everything with Status 'SKIPPED' if not ordered.
                // Let's save everything to avoid re-processing loop, but only alert on orders.
                itemsToSave.push({ ...item, OrderQty: 0, IsMotorbike: isMotorbike, Status: 'SKIPPED' });
            }
        }

        // 4. Generate & Send Messages
        // Use the Invoice Date from the data itself.
        // We pick the NEWEST date in the batch to represent the "Report Date".
        let dateStr = new Date().toISOString().split('T')[0]; // Default
        
        if (pendingItems.length > 0) {
            // Sort by date descending to find the newest
            const sortedByDate = [...pendingItems].sort((a, b) => new Date(b.InvoiceDate) - new Date(a.InvoiceDate));
            const newestItem = sortedByDate[0];
            
            // Format manually to avoid UTC shifts (keep local date)
            const d = new Date(newestItem.InvoiceDate);
            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            dateStr = `${year}-${month}-${day}`;
        }

        for (const [brand, data] of Object.entries(ordersByBrand)) {
            const generalOrders = data.general;
            const motorbikeOrders = data.motorbike;

            if (generalOrders.length === 0 && motorbikeOrders.length === 0) continue;

            let message = `*RE-ORDER REQUEST: ${brand}*\n`;
            message += `Date: ${dateStr}\n\n`;

            if (generalOrders.length > 0) {
                // General Tyres header removed as per request
                generalOrders.forEach((item, idx) => {
                    message += `${idx + 1}. ${item.ItemDescription}\n`;
                    message += `   Order: ${item.OrderQty} Pcs\n\n`;
                });
            }

            if (motorbikeOrders.length > 0) {
                message += `*Motorbike Tyres*\n`;
                motorbikeOrders.forEach((item, idx) => {
                    message += `${idx + 1}. ${item.ItemDescription}\n`;
                    message += `   Sold: ${item.SoldQty} Pcs\n\n`; // User asked to show "Sold" for motorbike
                });
            }

            // Send to "Re Order" group
            if (targetGroup) {
                try {
                    await client.sendMessage(targetGroup.id._serialized, message);
                    logAndSave(`📤 Sent ${brand} re-order alert to group "Re Order"`);
                } catch (err) {
                    logAndSave(`❌ Failed to send to group "Re Order": ${err.message}`);
                }
            } else {
                logAndSave(`⚠️ Skipping send for ${brand}: Target group not found.`);
            }
        }

        // 5. Save to Database (ReOrderHistory)
        if (itemsToSave.length > 0) {
            let aiRawPool = null;
            try {
                // Prefer raw mssql pool (required for sql.Transaction)
                aiRawPool = getAiRawPool();
            } catch (e) {
                // Fallback: if caller provided a real ConnectionPool, use it.
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
                    const request = new sql.Request(transaction);
                    request.input('InvoiceNo', sql.NVarChar(50), item.InvoiceNo);
                    request.input('InvoiceDate', sql.Date, item.InvoiceDate);
                    request.input('ItemCode', sql.NVarChar(50), item.ItemCode);
                    request.input('ItemDescription', sql.NVarChar(200), item.ItemDescription);
                    request.input('Brand', sql.NVarChar(50), item.Brand);
                    request.input('Category', sql.NVarChar(50), item.Category);
                    request.input('SoldQty', sql.Int, item.SoldQty);
                    request.input('StockAtOrder', sql.Int, item.CurrentStock);
                    request.input('OrderedQty', sql.Int, item.OrderQty);
                    request.input('Status', sql.NVarChar(20), item.Status || 'SENT');

                    // Use MERGE or IF NOT EXISTS to be safe against race conditions
                    await request.query(`
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
