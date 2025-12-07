/**
 * QuotationExpiryJob - Automatically marks expired quotations
 * Runs every hour to check and update expired quotations
 */

const sql = require('mssql');
require('dotenv').config();

class QuotationExpiryJob {
    constructor() {
        this.name = 'QuotationExpiryJob';
        this.description = 'Marks quotations as expired after their expiry date';
        this.schedule = '0 * * * *'; // Every hour
        this.enabled = true;
    }

    async execute() {
        const startTime = Date.now();
        let expiredCount = 0;

        try {
            console.log(`[${this.name}] Starting expiry check...`);

            const config = {
                user: process.env.SQL_USER,
                password: process.env.SQL_PASSWORD,
                server: process.env.SQL_SERVER,
                database: 'WhatsAppAI',
                options: {
                    encrypt: process.env.SQL_ENCRYPT === 'true',
                    trustServerCertificate: process.env.SQL_TRUST_CERT === 'true',
                    enableArithAbort: true,
                    connectTimeout: 30000,
                    requestTimeout: 30000
                }
            };

            const pool = await sql.connect(config);
            const request = pool.request();

            // Update expired quotations
            const result = await request.query(`
                UPDATE Quotations
                SET IsExpired = 1, Status = 'Expired'
                WHERE IsExpired = 0 
                  AND IsBooked = 0
                  AND ExpiryDate IS NOT NULL
                  AND ExpiryDate < GETDATE()
            `);

            expiredCount = result.rowsAffected[0];

            await pool.close();

            const duration = Date.now() - startTime;
            console.log(`[${this.name}] Completed in ${duration}ms - Marked ${expiredCount} quotations as expired`);

            return {
                success: true,
                expiredCount,
                duration
            };

        } catch (error) {
            console.error(`[${this.name}] Error:`, error);
            return {
                success: false,
                error: error.message
            };
        }
    }

    // Manual trigger method
    async run() {
        return await this.execute();
    }
}

module.exports = QuotationExpiryJob;
