const DbLayer = require('./dbLayer');
const pdfGenerator = require('./pdfGenerator');
const config = require('./config');
// BAILEYS MIGRATION: Use wrapper instead of whatsapp-web.js
const { MessageMedia } = require('../utils/baileysWrapper');

/**
 * 🧠 THE MANAGER: Digital Invoice Orchestrator
 * Controls the flow: DB Watch -> PDF Gen -> WhatsApp/Email -> DB Update
 */
class DigitalInvoiceManager {
    constructor(sqlPool, waClient, emailService) {
        this.db = new DbLayer(sqlPool);
        this.waClient = waClient;
        this.emailService = emailService;
        this.isProcessing = false;
        
        console.log('🚀 [DigitalInvoice] Manager Initialized. Waiting to start...');
    }

    start() {
        console.log(`⏱️ [DigitalInvoice] Started. Polling every ${config.POLLING_INTERVAL}ms...`);
        // Start the continuous loop
        setInterval(() => this.processQueue(), config.POLLING_INTERVAL);
    }

    async processQueue() {
        if (this.isProcessing) return; // Prevent Overlap
        this.isProcessing = true;

        try {
            // 1. ATOMIC FETCH (The Magic Step)
            const job = await this.db.fetchNextJob();
            
            if (job) {
                console.log(`⚡ [DigitalInvoice] Processing Job #${job.ID} (${job.DocType} - ${job.RefNumber})`);
                await this.handleJob(job);
            }

        } catch (error) {
            console.error('⚠️ [DigitalInvoice] Loop Error:', error);
        } finally {
            this.isProcessing = false;
        }
    }

    async handleJob(job) {
        let log = [];
        let status = 'SUCCESS';

        try {
            // 2. Data Gathering (Real DB)
            log.push('Fetching Data...');
            const header = await this.db.getInvoiceHeader(job.RefNumber);
            const items = await this.db.getInvoiceItems(job.RefNumber);

            if (!header) {
                throw new Error(`Invoice '${job.RefNumber}' not found in View.`);
            }

            // Helper: Currency Formatter
            const formatMoney = (val) => {
                const n = Number(val);
                return isNaN(n) ? '0.00' : n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            };

            // Calculate Totals
            const totalAmount = items.reduce((acc, item) => acc + (Number(item.Amount) || 0), 0);
            
            // Map Data for PDF Generator
            const pdfData = {
                docType: job.DocType,
                invoiceNo: header.InvoiceNo,
                date: header.InvoiceDate ? new Date(header.InvoiceDate).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
                customerName: header.CustomerName || 'Cash Customer',
                customerMobile: job.ContactNumber,
                vehicleNo: header.VehicleNo || '-',
                items: items.map(i => ({
                    description: i.Description,
                    brand: i.Categoty || '', // The View has a typo: 'Categoty'
                    size: '', 
                    quantity: Number(i.Qty) || 0,
                    price: formatMoney(i.UnitPrice),
                    total: formatMoney(i.Amount)
                })),
                subtotal: formatMoney(totalAmount),
                discount: '0.00',
                total: formatMoney(totalAmount),
                qrCode: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=='
            };

            // 3. Generate PDF (High-End Artist)
            log.push('Generating PDF...');
            const pdfBuffer = await pdfGenerator.generate(pdfData);
            const pdfBase64 = pdfBuffer.toString('base64');
            log.push('PDF Generated.');

            // 4. Parallel Sending (The Turbo Boost)
            const tasks = [];

            // Task A: WhatsApp
            if (job.ContactNumber && this.waClient) {
                tasks.push((async () => {
                    try {
                        const chatId = job.ContactNumber.includes('@') ? job.ContactNumber : `${job.ContactNumber.replace(/\D/g,'')}@c.us`;
                        const media = new MessageMedia('application/pdf', pdfBase64, `${job.DocType}_${job.RefNumber}.pdf`);
                        await this.waClient.sendMessage(chatId, media, { caption: `Dear Customer, here is your ${job.DocType} from Lasantha Tyre.` });
                        log.push('WA: Sent ✅');
                        return true;
                    } catch (e) {
                        log.push(`WA: Failed ❌ (${e.message})`);
                        return false;
                    }
                })());
            }

            // Task B: Email (Zoho)
            if (job.EmailAddress && this.emailService) {
                // TODO: Implement proper email with attachment using emailService
                log.push('Email: Skipped (Not Configured)');
            }

            // Wait for all to finish (Non-blocking)
            await Promise.all(tasks);

        } catch (error) {
            console.error('❌ Job Failed:', error);
            status = 'FAILED';
            log.push(`Critical Error: ${error.message}`);
        }

        // 5. Final Status Update
        await this.db.updateStatus(job.ID, status, log.join(' | '));
        console.log(`✅ [DigitalInvoice] Job #${job.ID} Finished: ${status}`);
    }
}

module.exports = DigitalInvoiceManager;
