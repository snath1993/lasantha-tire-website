import os

file_path = r'c:\whatsapp-sql-api\index.js'

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Add Import
import_marker = "require('dotenv').config();"
import_code = "\nconst QueueProcessor = require('./digital-invoice/queueProcessor');"

if import_code.strip() not in content:
    content = content.replace(import_marker, import_marker + import_code)
    print("Added import.")

# 2. Add Initialization Code
init_code = """
// ========================================
// DIGITAL INVOICE SYSTEM LINK
// ========================================
console.log('🔗 Wiring up Digital Invoice System...');
setTimeout(() => {
    // Only start if we have the client and DB
    const checkAndStart = () => {
        if (global.whatsappClient && mainPool) {
            // Initialize the Queue Processor
            // Note: Email service is passed as null for now
            const invoiceProcessor = new QueueProcessor(mainPool, global.whatsappClient, null);
            invoiceProcessor.start();
            
            // Expose to global for debugging if needed
            global.invoiceProcessor = invoiceProcessor;
            
            console.log('✅ Digital Invoice Processor Linked & Started');
            return true;
        }
        return false;
    };

    if (!checkAndStart()) {
        console.warn('⚠️  Digital Invoice Processor: Dependencies not ready. Retrying in 10s...');
        setTimeout(() => {
             if (!checkAndStart()) {
                 console.error('❌ Digital Invoice Processor Start Failed: Dependencies still missing after retry.');
             }
        }, 10000);
    }
}, 5000); // Wait 5s for main systems to settle
"""

if "DIGITAL INVOICE SYSTEM LINK" not in content:
    content += init_code
    print("Added initialization code.")

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)
