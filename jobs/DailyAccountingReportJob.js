const schedule = require('node-schedule');
const axios = require('axios');

// Global WhatsApp Client Instance
let whatsappClient = null;

// Managing Director contact (masked in code comments for security)
const MD_NUMBER = '94777311770@c.us'; // Managing Director WhatsApp number
const ADMIN_NUMBER = '94776197772@c.us'; // Admin number for testing

// Peachtree Bridge base URL (health lives at /health; API lives under /api/peachtree)
const PEACHTREE_BRIDGE_BASE = (process.env.PEACHTREE_BRIDGE_BASE || process.env.PEACHTREE_API_BASE || 'http://127.0.0.1:5001').replace(/\/$/, '');
const PEACHTREE_API = PEACHTREE_BRIDGE_BASE.endsWith('/api/peachtree')
    ? PEACHTREE_BRIDGE_BASE
    : `${PEACHTREE_BRIDGE_BASE}/api/peachtree`;

function describePeachtreeApiTarget() {
    try {
        const u = new URL(PEACHTREE_BRIDGE_BASE);
        const port = u.port ? u.port : (u.protocol === 'https:' ? '443' : '80');
        return `${u.hostname}:${port}`;
    } catch {
        return PEACHTREE_BRIDGE_BASE;
    }
}

/**
 * Format currency for Sri Lanka Rupees
 */
function formatCurrency(amount) {
    return `Rs. ${parseFloat(amount || 0).toLocaleString('en-US', { 
        minimumFractionDigits: 2, 
        maximumFractionDigits: 2 
    })}`;
}

/**
 * Format number with commas
 */
function formatNumber(num) {
    return parseInt(num || 0).toLocaleString('en-US');
}

/**
 * Generate fallback report when Peachtree API is unavailable
 */
function generateFallbackReport() {
    const now = new Date();
    const dateStr = now.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
    const timeStr = now.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true });
    
    let report = `*📊 DAILY ACCOUNTING REPORT*\n`;
    report += `_${dateStr} | ${timeStr}_\n\n`;
    
    report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
    report += `⚠️ *PEACHTREE CONNECTION UNAVAILABLE*\n`;
    report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n`;
    
    report += `The Peachtree Bridge service is currently offline or unreachable.\n\n`;
    
    report += `*Possible Causes:*\n`;
    report += `• Peachtree application not running\n`;
    report += `• Bridge service (${describePeachtreeApiTarget()}) not started\n`;
    report += `• Network connectivity issue\n`;
    report += `• Database locked or busy\n\n`;
    
    report += `*Action Required:*\n`;
    report += `1. Check if Peachtree is open\n`;
    report += `2. Verify Bridge service is running\n`;
    report += `3. Restart services if needed\n\n`;
    
    report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
    report += `_Report will resume once connection is restored_\n`;
    report += `_Lasantha Tire Company_`;
    
    return report;
}

/**
 * Generate comprehensive accounting report
 */
async function generateAccountingReport() {
    try {
        console.log('📊 Generating Daily Accounting Report...');
        
        // Check if Peachtree API is reachable first
        try {
            await axios.get(`${PEACHTREE_BRIDGE_BASE}/health`, { timeout: 5000 });
        } catch (healthError) {
            console.warn('⚠️  Peachtree API not reachable, generating fallback report');
            return generateFallbackReport();
        }
        
        // Fetch all data from Peachtree with timeout
        const [customersRes, vendorsRes, accountsRes] = await Promise.all([
            axios.get(`${PEACHTREE_API}/customers`, { timeout: 10000 }),
            axios.get(`${PEACHTREE_API}/vendors`, { timeout: 10000 }),
            axios.get(`${PEACHTREE_API}/chart-of-accounts`, { timeout: 10000 })
        ]);
        
        const customers = customersRes.data.data || [];
        const vendors = vendorsRes.data.data || [];
        const accounts = accountsRes.data.data || [];
        
        // Calculate key metrics
        const activeCustomers = customers.filter(c => c.CustomerIsInactive === 0 || c.Status === 'Active').length;
        const activeVendors = vendors.filter(v => v.VendorIsInactive === 0 || v.Status === 'Active').length;
        
        const totalAR = customers.reduce((sum, c) => sum + parseFloat(c.Balance || 0), 0);
        const totalAP = vendors.reduce((sum, v) => sum + parseFloat(v.Balance || 0), 0);
        const netPosition = totalAR - totalAP;
        
        // Top customers (highest balances)
        const topCustomers = customers
            .filter(c => parseFloat(c.Balance || 0) > 0)
            .sort((a, b) => parseFloat(b.Balance || 0) - parseFloat(a.Balance || 0))
            .slice(0, 5);
        
        // Top vendors (highest balances)
        const topVendors = vendors
            .filter(v => parseFloat(v.Balance || 0) > 0)
            .sort((a, b) => parseFloat(b.Balance || 0) - parseFloat(a.Balance || 0))
            .slice(0, 5);
        
        // Credit utilization warnings (>80%)
        const creditWarnings = customers
            .filter(c => {
                const balance = parseFloat(c.Balance || 0);
                const creditLimit = parseFloat(c.Terms_CreditLimit || c.CreditLimit || 0);
                return creditLimit > 0 && (balance / creditLimit) > 0.8;
            })
            .map(c => {
                const balance = parseFloat(c.Balance || 0);
                const creditLimit = parseFloat(c.Terms_CreditLimit || c.CreditLimit || 0);
                const utilization = ((balance / creditLimit) * 100).toFixed(1);
                return {
                    name: c.Customer_Bill_Name || c.CustomerName || c.CustomerID,
                    balance,
                    creditLimit,
                    utilization
                };
            });
        
        // Active accounts summary
        const activeAccounts = accounts.filter(a => a.Active === true || a.Active === 1).length;
        const totalAccountBalance = accounts.reduce((sum, a) => sum + parseFloat(a.Balance || 0), 0);
        
        // Generate report message
        const reportDate = new Date().toLocaleDateString('en-GB', {
            day: '2-digit',
            month: 'short',
            year: 'numeric'
        });
        
        const reportTime = new Date().toLocaleTimeString('en-US', {
            hour: '2-digit',
            minute: '2-digit',
            hour12: true
        });
        
        let report = `*📊 DAILY ACCOUNTING REPORT*\n`;
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `📅 Date: ${reportDate}\n`;
        report += `⏰ Time: ${reportTime}\n\n`;
        
        // Financial Summary
        report += `*💰 FINANCIAL SUMMARY*\n`;
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `📈 Accounts Receivable: ${formatCurrency(totalAR)}\n`;
        report += `📉 Accounts Payable: ${formatCurrency(totalAP)}\n`;
        report += `💵 Net Position: ${formatCurrency(netPosition)} ${netPosition >= 0 ? '✅' : '⚠️'}\n\n`;
        
        // Customer Summary
        report += `*👥 CUSTOMER SUMMARY*\n`;
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `Total Customers: ${formatNumber(customers.length)}\n`;
        report += `Active: ${formatNumber(activeCustomers)} | `;
        report += `Inactive: ${formatNumber(customers.length - activeCustomers)}\n`;
        report += `Outstanding Balance: ${formatCurrency(totalAR)}\n\n`;
        
        // Top 5 Customers
        if (topCustomers.length > 0) {
            report += `*🏆 TOP 5 CUSTOMERS (Outstanding)*\n`;
            report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
            topCustomers.forEach((c, i) => {
                const name = c.Customer_Bill_Name || c.CustomerName || c.CustomerID;
                const balance = parseFloat(c.Balance || 0);
                report += `${i + 1}. ${name}\n`;
                report += `   ${formatCurrency(balance)}\n`;
            });
            report += `\n`;
        }
        
        // Credit Warnings
        if (creditWarnings.length > 0) {
            report += `*⚠️ CREDIT LIMIT WARNINGS (>80%)*\n`;
            report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
            creditWarnings.forEach(w => {
                report += `• ${w.name}\n`;
                report += `  ${formatCurrency(w.balance)} / ${formatCurrency(w.creditLimit)}\n`;
                report += `  Utilization: ${w.utilization}%\n`;
            });
            report += `\n`;
        }
        
        // Vendor Summary
        report += `*🏢 VENDOR SUMMARY*\n`;
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `Total Vendors: ${formatNumber(vendors.length)}\n`;
        report += `Active: ${formatNumber(activeVendors)} | `;
        report += `Inactive: ${formatNumber(vendors.length - activeVendors)}\n`;
        report += `Outstanding Payables: ${formatCurrency(totalAP)}\n\n`;
        
        // Top 5 Vendors
        if (topVendors.length > 0) {
            report += `*🏆 TOP 5 VENDORS (Payables)*\n`;
            report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
            topVendors.forEach((v, i) => {
                const name = v.VendorName || v.VendorID;
                const balance = parseFloat(v.Balance || 0);
                report += `${i + 1}. ${name}\n`;
                report += `   ${formatCurrency(balance)}\n`;
            });
            report += `\n`;
        }
        
        // Chart of Accounts Summary
        report += `*📋 CHART OF ACCOUNTS*\n`;
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `Total Accounts: ${formatNumber(accounts.length)}\n`;
        report += `Active: ${formatNumber(activeAccounts)}\n`;
        report += `Total Balance: ${formatCurrency(totalAccountBalance)}\n\n`;
        
        // Footer
        report += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        report += `_Generated by Peachtree ERP Dashboard_\n`;
        report += `_Lasantha Tire Company_`;
        
        return report;
        
    } catch (error) {
        console.error('❌ Error generating accounting report:', error.message);
        console.error('Error details:', error.code, error.response?.status);
        
        // Return detailed error report
        const now = new Date();
        const dateStr = now.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
        
        let errorReport = `*❌ REPORT GENERATION FAILED*\n`;
        errorReport += `_${dateStr}_\n\n`;
        errorReport += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        errorReport += `*Error Details:*\n`;
        errorReport += `${error.message}\n\n`;
        
        if (error.code === 'ECONNREFUSED') {
            errorReport += `*Issue:* Peachtree Bridge not responding\n`;
            errorReport += `*Action:* Start the Bridge service on port 3001\n`;
        } else if (error.code === 'ETIMEDOUT') {
            errorReport += `*Issue:* Request timeout\n`;
            errorReport += `*Action:* Check Peachtree performance\n`;
        } else {
            errorReport += `*Issue:* Data retrieval error\n`;
            errorReport += `*Action:* Check logs and restart services\n`;
        }
        
        errorReport += `━━━━━━━━━━━━━━━━━━━━━━━━━━\n`;
        errorReport += `_Lasantha Tire Company_`;
        
        return errorReport;
    }
}

/**
 * Send WhatsApp message directly via Client or Fallback API
 */
async function sendWhatsAppMessage(to, message) {
    try {
        const number = to.replace('@c.us', '');
        const chatId = number.includes('@') ? number : `${number}@c.us`;

        // 1. Try using direct client instance (Most Reliable)
        if (whatsappClient) {
            await whatsappClient.sendMessage(chatId, message);
            return { success: true, method: 'direct' };
        }

        // 2. Fallback to API call (If client not initialized)
        console.warn('⚠️ WhatsApp client not set, falling back to local API');
        const botPort = process.env.BOT_API_PORT || 8585;
        const response = await axios.post(`http://localhost:${botPort}/api/whatsapp/send`, {
            number: number,
            message: message
        });
        
        return response.data;
    } catch (error) {
        console.error('❌ Error sending WhatsApp message:', error.message);
        throw error;
    }
}

/**
 * Send daily accounting report to Managing Director
 */
async function sendDailyReport() {
    console.log('\n🚀 Starting Daily Accounting Report Job...');
    console.log(`⏰ Time: ${new Date().toLocaleString()}`);
    
    try {
        // Generate report
        const report = await generateAccountingReport();
        
        // Send to Managing Director
        console.log(`📤 Sending report to Managing Director...`);
        await sendWhatsAppMessage(MD_NUMBER, report);
        
        console.log('✅ Daily accounting report sent successfully!');
        
    } catch (error) {
        console.error('❌ Failed to send daily report:', error.message);
        
        // Send error notification to admin
        try {
            await sendWhatsAppMessage(ADMIN_NUMBER, 
                `*⚠️ REPORT GENERATION ERROR*\n\n` +
                `Failed to generate/send daily accounting report.\n\n` +
                `Error: ${error.message}\n\n` +
                `Please check the system.`
            );
        } catch (notifyError) {
            console.error('❌ Failed to send error notification:', notifyError.message);
        }
    }
}

/**
 * Handle manual report request (test command)
 */
async function handleManualReportRequest(from) {
    console.log(`📱 Manual report request from: ${from}`);
    
    // Only allow admin to request manual reports
    if (from !== ADMIN_NUMBER) {
        console.log('⛔ Unauthorized request - ignoring');
        return;
    }
    
    try {
        // Generate and send report
        const report = await generateAccountingReport();
        await sendWhatsAppMessage(from, report);
        
        console.log('✅ Manual report sent to admin');
        
    } catch (error) {
        console.error('❌ Failed to send manual report:', error.message);
        await sendWhatsAppMessage(from, 
            `*❌ Error generating report*\n\n${error.message}`
        );
    }
}

/**
 * Schedule daily report at 9:00 AM
 */
function scheduleDailyReport() {
    // Schedule for 9:00 AM every day
    const job = schedule.scheduleJob('0 9 * * *', async () => {
        await sendDailyReport();
    });
    
    console.log('📅 Daily accounting report scheduled for 9:00 AM');
    console.log(`   Next run: ${job.nextInvocation()}`);
    
    return job;
}

/**
 * Initialize the daily accounting report job
 */
function initializeDailyAccountingReport(client) {
    console.log('\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
    console.log('📊 Daily Accounting Report System');
    console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
    
    // Set global client
    whatsappClient = client;

    // Schedule daily report
    const scheduledJob = scheduleDailyReport();
    
    // Listen for manual report requests from admin
    client.on('message', async (message) => {
        const text = message.body.toLowerCase().trim();
        const from = message.from;
        
        // Check for "acc report" command from admin
        if (text === 'acc report' && from === ADMIN_NUMBER) {
            console.log('\n📱 Admin requested manual accounting report');
            
            // Send "generating" message
            await message.reply('⏳ _Generating full accounting report..._');
            
            // Generate and send report
            await handleManualReportRequest(from);
        }
    });
    
    console.log('\n✅ Listeners registered:');
    console.log('   • Daily 9:00 AM report to MD');
    console.log('   • Manual "acc report" command (admin only)');
    console.log('\n💡 Test command: Send "acc report" from admin number');
    console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n');
    
    return {
        scheduledJob,
        sendManualReport: handleManualReportRequest,
        sendDailyReport
    };
}

module.exports = {
    initializeDailyAccountingReport,
    sendDailyReport,
    generateAccountingReport
};
