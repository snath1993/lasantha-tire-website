const InvoiceDeletionJob = require('../jobs/invoiceDeletionJob');

async function run() {
    console.log('--- STARTING MANUAL RE-VOID TEST ---');
    const job = new InvoiceDeletionJob();
    
    try {
        await job.initialize();
        console.log('Job initialized.');
        
        const invoiceNo = 'NLT00006513';
        console.log(`Processing invoice: ${invoiceNo}`);
        
        // Using 'force' or similar isn't needed, the logic just does Backup -> Delete -> Insert
        const result = await job.processInvoiceDeletion(
            invoiceNo, 
            'MANUAL_TEST', 
            'Manual re-void to fix missing columns'
        );
        
        console.log('Result:', JSON.stringify(result, null, 2));
        
    } catch (err) {
        console.error('ERROR:', err);
    } finally {
        process.exit(0);
    }
}

run();