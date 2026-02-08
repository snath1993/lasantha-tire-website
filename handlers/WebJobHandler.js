const TyrePriceReplyJob = require('../jobs/TyrePriceReplyJob');
const sql = require('mssql');

class WebJobHandler {
    constructor(client, sqlConfig, allowedContacts, logger) {
        this.client = client;
        this.sqlConfig = sqlConfig;
        this.allowedContacts = allowedContacts;
        this.logger = logger || console.log;
    }

    async handlePriceRequest(data) {
        const { to, tireSize, customerName } = data;
        // Ensure 'to' is a valid WhatsApp ID
        const whatsappId = to.includes('@') ? to : `${to.replace(/\D/g, '')}@c.us`;

        // Mock Message Object
        // TyrePriceReplyJob expects:
        // msg.body (to extract size)
        // msg.from (to check allowed contacts)
        // msg.reply (via safeReply)
        const mockMsg = {
            from: whatsappId,
            body: tireSize, 
            _data: {
                notifyName: customerName || 'Website Customer'
            },
            // Mock reply function so safeReply works
            reply: async (text) => {
                if (this.client) {
                    return await this.client.sendMessage(whatsappId, text);
                }
            }
        };

        this.logger(`[WebJobHandler] Triggering TyrePriceReplyJob for ${whatsappId} (Size: ${tireSize})`);

        try {
            // TyrePriceReplyJob expects the global 'sql' object (mssql package) to manage its own connection lifecycle.
            // It calls sql.connect(sqlConfig) and sql.close().
            // We pass the 'sql' package required at the top of this file.
            await TyrePriceReplyJob(mockMsg, sql, this.sqlConfig, this.allowedContacts, this.logger);
            return { success: true };
        } catch (error) {
            this.logger(`[WebJobHandler] Job execution failed: ${error.message}`);
            throw error;
        }
    }
}

module.exports = WebJobHandler;
