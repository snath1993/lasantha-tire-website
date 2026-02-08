const DbLayer = require('./dbLayer');
const pdfGenerator = require('./pdfGenerator');
const config = require('./config');
// BAILEYS MIGRATION: Use wrapper instead of whatsapp-web.js
const { MessageMedia } = require('../utils/baileysWrapper');
const fs = require('fs');
const path = require('path');

/**
 * 🔄 QUEUE PROCESSOR: Watches tblDigitalInvoiceQueue and processes pending items
 * Uses file-based approach to avoid Base64 corruption issues
 */
class QueueProcessor {
    constructor(sqlPool, waClient, emailService) {
        this.pool = sqlPool;
        this.db = new DbLayer(sqlPool); 
        this.waClient = waClient;
        this.emailService = emailService;
        this.isProcessing = false;
        this.pollInterval = config.POLLING_INTERVAL || 3000;
        
        // Temp folder for PDFs
        this.tempDir = path.join(__dirname, 'temp');
        if (!fs.existsSync(this.tempDir)) {
            fs.mkdirSync(this.tempDir, { recursive: true });
        }
    }

    start() {
        console.log(`📋 [QueueProcessor] Started. Polling every ${this.pollInterval}ms...`);
        this.intervalId = setInterval(() => this.processQueue(), this.pollInterval);
    }

    stop() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            console.log('📋 [QueueProcessor] Stopped.');
        }
    }

    async processQueue() {
        if (this.isProcessing) return;
        this.isProcessing = true;

        try {
            const job = await this.db.fetchNextJob();
            
            if (job) {
                const hintedType = (job.DocType === null || job.DocType === undefined || String(job.DocType).trim() === '') ? 'AUTO' : String(job.DocType).trim();
                console.log(`⚡ [QueueProcessor] Processing Job #${job.ID}: ${hintedType} ${job.RefNumber}`);
                await this.handleJob(job);
            }

        } catch (error) {
            console.error('⚠️ [QueueProcessor] Loop Error:', error.message);
        } finally {
            this.isProcessing = false;
        }
    }

    async handleJob(job) {
        const logs = [];
        let finalStatus = 'SENT';
        let tempFilePath = null;
        
        const hintedType = (job.DocType === null || job.DocType === undefined || String(job.DocType).trim() === '') ? 'AUTO' : String(job.DocType).trim();
        logs.push(`Started processing ${hintedType} #${job.RefNumber}`);

        try {
            // 1. Fetch Data
            logs.push(`Fetching data...`);
            const invoiceData = await this.db.fetchDocumentData(job.DocType, job.RefNumber);
            try {
                if (invoiceData && invoiceData.metadata && invoiceData.metadata.type) {
                    logs.push(`Detected doc type: ${invoiceData.metadata.type}`);
                }
            } catch {}
            // Enrich for template (invoice view doesn't always contain phone)
            try {
                if (invoiceData && invoiceData.invoice && job.ContactNumber) {
                    invoiceData.invoice.customerMobile = job.ContactNumber;
                }
            } catch {}
            logs.push('Data fetched.');

            // 2. Generate PDF and SAVE TO FILE (avoid Base64 in-memory issues)
            logs.push('Generating PDF...');
            const pdfBuffer = await pdfGenerator.generate(invoiceData);
            
            const fileName = `${job.RefNumber.replace(/[\/\\:]/g, '-')}.pdf`;
            tempFilePath = path.join(this.tempDir, fileName);
            fs.writeFileSync(tempFilePath, pdfBuffer);
            logs.push(`PDF saved: ${fileName}`);

            // 3. Send via WhatsApp using MessageMedia.fromFilePath (more reliable)
            if (job.ContactNumber && this.waClient) {
                try {
                    // Normalize number
                    let inputNumber = job.ContactNumber.replace(/\D/g, '');
                    if (inputNumber.startsWith('0')) inputNumber = '94' + inputNumber.substring(1);
                    if (inputNumber.length === 9) inputNumber = '94' + inputNumber;
                    
                    const chatId = `${inputNumber}@c.us`;
                    logs.push(`Sending to: ${chatId}`);

                    const caption = `📄 *${invoiceData.metadata.type}* from ${config.COMPANY.NAME}\nRef: ${job.RefNumber}\nDate: ${invoiceData.invoice.date}`;
                    
                    // Load from FILE instead of raw buffer (cleaner Base64)
                    const media = MessageMedia.fromFilePath(tempFilePath);
                    
                    // Avoid WA sendSeen() crash path
                    await this.waClient.sendMessage(chatId, media, { caption, sendSeen: false });
                    
                    logs.push(`WhatsApp sent ✅`);

                } catch (waError) {
                    console.error('WA Send Error:', waError.message);
                    logs.push(`WhatsApp Error: ${waError.message}`);
                    
                    // Try text-only fallback
                    try {
                        logs.push('Trying text-only fallback...');
                        let inputNumber = job.ContactNumber.replace(/\D/g, '');
                        if (inputNumber.startsWith('0')) inputNumber = '94' + inputNumber.substring(1);
                        const chatId = `${inputNumber}@c.us`;
                        
                        const textMsg = `📄 *${invoiceData.metadata.type}* from ${config.COMPANY.NAME}\n\nRef: ${job.RefNumber}\nDate: ${invoiceData.invoice.date}\nCustomer: ${invoiceData.invoice.customerName}\n\n⚠️ PDF attachment failed. Please contact us for the document.`;
                        
                        await this.waClient.sendMessage(chatId, textMsg, { sendSeen: false });
                        logs.push('Text fallback sent ✅');
                        finalStatus = 'PARTIAL'; // Partial success
                    } catch (textErr) {
                        logs.push(`Text fallback failed: ${textErr.message} ❌`);
                        finalStatus = 'FAILED';
                    }
                }
            } else {
                logs.push('No WhatsApp number provided.');
                if (!job.EmailAddress) finalStatus = 'FAILED';
            }

            // 4. Send via Email (Zoho) with PDF attachment
            if (job.EmailAddress && this.emailService && this.emailService.initialized) {
                try {
                    logs.push(`Sending email to: ${job.EmailAddress}`);
                    
                    const emailResult = await this.emailService.sendInvoiceEmail({
                        customerEmail: job.EmailAddress,
                        customerName: invoiceData.invoice.customerName || 'Customer',
                        invoiceNumber: job.RefNumber,
                        docType: invoiceData.metadata.type,
                        pdfPath: tempFilePath,
                        totalAmount: invoiceData.totals.grandTotal,
                        items: invoiceData.items || []
                    });
                    
                    if (emailResult.success) {
                        logs.push(`Email sent successfully ✅ (${emailResult.messageId})`);
                    } else {
                        logs.push(`Email failed: ${emailResult.error} ⚠️`);
                        // Don't fail the whole job if only email fails (WhatsApp may have succeeded)
                        if (finalStatus === 'SENT') {
                            finalStatus = 'PARTIAL'; // WhatsApp sent, email failed
                        }
                    }
                } catch (emailError) {
                    console.error('Email Send Error:', emailError.message);
                    logs.push(`Email exception: ${emailError.message} ❌`);
                    if (finalStatus === 'SENT') {
                        finalStatus = 'PARTIAL';
                    }
                }
            } else if (job.EmailAddress) {
                logs.push('Email service not configured - skipped.');
            }

        } catch (error) {
            console.error(`❌ Job #${job.ID} Failed:`, error);
            finalStatus = 'FAILED';
            logs.push(`CRITICAL ERROR: ${error.message}`);
        } finally {
            // Cleanup temp file
            if (tempFilePath && fs.existsSync(tempFilePath)) {
                try { fs.unlinkSync(tempFilePath); } catch {}
            }
        }

        await this.db.updateStatus(job.ID, finalStatus, logs.join(' | '));
        console.log(`✅ [QueueProcessor] Job #${job.ID} Finished: ${finalStatus}`);
    }
}

module.exports = QueueProcessor;
