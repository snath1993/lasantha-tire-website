const sql = require('mssql');
require('dotenv').config();

const config = {
    server: 'WIN-JIAVRTFMA0N\\SQLEXPRESS',
    database: 'LasanthaTire',
    user: 'sa',
    password: process.env.SQL_PASSWORD || 'password',
    port: 1433,
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

(async () => {
    try {
        const pool = await sql.connect(config);
        
        // Get data from last 4 months
        const fourMonthsAgo = new Date();
        fourMonthsAgo.setMonth(fourMonthsAgo.getMonth() - 4);
        const dateString = fourMonthsAgo.toISOString().split('T')[0];
        
        const result = await pool.request().query(`
            SELECT 
                Expr1 AS InvoiceDate,
                Expr2 AS InvoiceNo,
                Expr3 AS ItemCode,
                Expr4 AS ItemName,
                Expr5 AS Qty,
                Expr6 AS Price,
                Expr12 AS Amount,
                Categoty,
                WHID
            FROM [View_Sales report whatsapp]
            WHERE Expr1 >= '${dateString}'
                AND Expr4 LIKE '%DURATURN%'
                AND Categoty = 'TYRES'
            ORDER BY Expr1 DESC
        `);
        
        console.log('📊 DURATURN BRAND SALES ANALYSIS (Last 4 Months)');
        console.log('='.repeat(60));
        console.log(`Total Records: ${result.recordset.length}`);
        console.log(`Period: ${dateString} to ${new Date().toISOString().split('T')[0]}`);
        console.log('');
        
        if (result.recordset.length === 0) {
            console.log('❌ No Duraturn sales found in the last 4 months');
            await pool.close();
            return;
        }
        
        // Aggregate by tyre size
        const sizeAnalysis = {};
        let totalQty = 0;
        let totalRevenue = 0;
        
        result.recordset.forEach(record => {
            const itemName = record.ItemName || '';
            const qty = parseInt(record.Qty) || 0;
            const amount = parseFloat(record.Amount) || 0;
            
            // Extract size (e.g., "165/70/14" from "165/70/14 DURATURN...")
            const sizeMatch = itemName.match(/^(\d+\/\d+[\/R]\d+)/);
            const size = sizeMatch ? sizeMatch[1] : itemName.split(' ')[0];
            
            if (!sizeAnalysis[size]) {
                sizeAnalysis[size] = {
                    size: size,
                    totalQty: 0,
                    totalRevenue: 0,
                    invoiceCount: 0,
                    avgPrice: 0,
                    sampleItems: []
                };
            }
            
            sizeAnalysis[size].totalQty += qty;
            sizeAnalysis[size].totalRevenue += amount;
            sizeAnalysis[size].invoiceCount += 1;
            
            if (sizeAnalysis[size].sampleItems.length < 2) {
                sizeAnalysis[size].sampleItems.push(itemName);
            }
            
            totalQty += qty;
            totalRevenue += amount;
        });
        
        // Calculate average prices
        Object.keys(sizeAnalysis).forEach(size => {
            const data = sizeAnalysis[size];
            data.avgPrice = data.totalRevenue / data.totalQty;
        });
        
        // Sort by total quantity sold
        const sortedSizes = Object.values(sizeAnalysis).sort((a, b) => b.totalQty - a.totalQty);
        
        console.log('📈 SALES BY TYRE SIZE (Ranked by Quantity Sold)');
        console.log('='.repeat(60));
        console.log('');
        
        sortedSizes.forEach((data, index) => {
            const marketShare = ((data.totalQty / totalQty) * 100).toFixed(1);
            const revenueShare = ((data.totalRevenue / totalRevenue) * 100).toFixed(1);
            
            console.log(`${index + 1}. ${data.size}`);
            console.log(`   Quantity Sold: ${data.totalQty} units (${marketShare}% of Duraturn sales)`);
            console.log(`   Revenue: Rs. ${data.totalRevenue.toLocaleString()} (${revenueShare}%)`);
            console.log(`   Avg Price: Rs. ${Math.round(data.avgPrice).toLocaleString()} per unit`);
            console.log(`   Invoices: ${data.invoiceCount}`);
            console.log(`   Sample: ${data.sampleItems[0]}`);
            console.log('');
        });
        
        console.log('='.repeat(60));
        console.log('💰 OVERALL SUMMARY');
        console.log(`Total Quantity: ${totalQty} units`);
        console.log(`Total Revenue: Rs. ${totalRevenue.toLocaleString()}`);
        console.log(`Average Sale Value: Rs. ${Math.round(totalRevenue / result.recordset.length).toLocaleString()}`);
        console.log('');
        
        console.log('='.repeat(60));
        console.log('✅ RECOMMENDED SIZES TO ORDER (Top 10)');
        console.log('='.repeat(60));
        
        const topSizes = sortedSizes.slice(0, 10);
        topSizes.forEach((data, index) => {
            const monthlyAvg = (data.totalQty / 4).toFixed(1);
            console.log(`${index + 1}. ${data.size}`);
            console.log(`   Monthly Average: ${monthlyAvg} units`);
            console.log(`   Suggested Order Qty: ${Math.ceil(monthlyAvg * 2)} units (2 months stock)`);
            console.log(`   Avg Selling Price: Rs. ${Math.round(data.avgPrice).toLocaleString()}`);
            console.log('');
        });
        
        // Now get current stock from View_ItemWhse
        console.log('='.repeat(60));
        console.log('📦 CURRENT STOCK ANALYSIS');
        console.log('='.repeat(60));
        console.log('Checking current inventory levels...\n');
        
        const stockResult = await pool.request().query(`
            SELECT 
                ItemId,
                ItemDis,
                WhseId,
                QTY,
                UnitCost
            FROM [View_ItemWhse]
            WHERE ItemDis LIKE '%DURATURN%'
                AND QTY > 0
            ORDER BY ItemDis
        `);
        
        console.log(`Total Duraturn items in stock: ${stockResult.recordset.length}\n`);
        
        // Group stock by size
        const stockBySize = {};
        stockResult.recordset.forEach(item => {
            const itemName = item.ItemDis || '';
            const sizeMatch = itemName.match(/^(\d+\/\d+[\/R]\d+)/);
            const size = sizeMatch ? sizeMatch[1] : itemName.split(' ')[0];
            
            if (!stockBySize[size]) {
                stockBySize[size] = {
                    totalQty: 0,
                    items: []
                };
            }
            
            stockBySize[size].totalQty += parseInt(item.QTY) || 0;
            stockBySize[size].items.push({
                itemName: item.ItemDis,
                whid: item.WhseId,
                qty: parseInt(item.QTY) || 0,
                cost: parseFloat(item.UnitCost) || 0
            });
        });
        
        // Compare with top 10 sizes
        console.log('='.repeat(60));
        console.log('🎯 ORDER RECOMMENDATIONS (Top 10 Sizes)');
        console.log('='.repeat(60));
        console.log('');
        
        let totalToOrder = 0;
        const orderList = [];
        
        topSizes.forEach((data, index) => {
            const monthlyAvg = data.totalQty / 4;
            const recommendedStock = Math.ceil(monthlyAvg * 2); // 2 months stock
            const currentStock = stockBySize[data.size] ? stockBySize[data.size].totalQty : 0;
            const toOrder = Math.max(0, recommendedStock - currentStock);
            
            console.log(`${index + 1}. ${data.size}`);
            console.log(`   Monthly Sales: ${monthlyAvg.toFixed(1)} units/month`);
            console.log(`   Recommended Stock: ${recommendedStock} units (2 months)`);
            console.log(`   Current Stock: ${currentStock} units`);
            
            if (toOrder > 0) {
                console.log(`   ⚠️  ORDER NEEDED: ${toOrder} units`);
                totalToOrder += toOrder;
                orderList.push({
                    size: data.size,
                    toOrder: toOrder,
                    currentStock: currentStock,
                    avgPrice: data.avgPrice,
                    sample: data.sampleItems[0]
                });
            } else {
                const surplus = currentStock - recommendedStock;
                console.log(`   ✅ SUFFICIENT (surplus: ${surplus} units)`);
            }
            
            if (stockBySize[data.size] && stockBySize[data.size].items.length > 0) {
                console.log(`   Stock Details:`);
                stockBySize[data.size].items.forEach(item => {
                    console.log(`     - ${item.itemName}: ${item.qty} @ ${item.whid}`);
                });
            }
            console.log('');
        });
        
        console.log('='.repeat(60));
        console.log('📋 FINAL ORDER SUMMARY');
        console.log('='.repeat(60));
        console.log('');
        
        if (orderList.length === 0) {
            console.log('✅ All top 10 sizes have sufficient stock!');
        } else {
            console.log(`Total items to order: ${orderList.length} sizes`);
            console.log(`Total quantity to order: ${totalToOrder} units\n`);
            
            let estimatedCost = 0;
            orderList.forEach((item, idx) => {
                const estimatedItemCost = item.toOrder * item.avgPrice * 0.7; // Assume 70% of selling price as cost
                estimatedCost += estimatedItemCost;
                
                console.log(`${idx + 1}. ${item.size}`);
                console.log(`   Order Qty: ${item.toOrder} units`);
                console.log(`   Current: ${item.currentStock} units`);
                console.log(`   Est. Cost: Rs. ${Math.round(estimatedItemCost).toLocaleString()}`);
                console.log(`   Sample: ${item.sample}`);
                console.log('');
            });
            
            console.log('='.repeat(60));
            console.log(`💰 ESTIMATED TOTAL ORDER VALUE: Rs. ${Math.round(estimatedCost).toLocaleString()}`);
            console.log('(Estimated at 70% of average selling price)');
        }
        
        await pool.close();
        
    } catch (err) {
        console.error('❌ Error:', err.message);
        process.exit(1);
    }
})();
