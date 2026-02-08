
require('dotenv').config({ path: 'c:\\whatsapp-sql-api\\.env' });
const titan = require('../jobs/GrandCentralMind');

// Mock WhatsApp Client
const mockClient = {
    sendMessage: async (to, msg) => {
        console.log(`[MOCK WA] Sending to ${to}:
---------------------------------------------------
${msg}
---------------------------------------------------`);
        return true;
    }
};

(async () => {
    try {
        console.log('--- STARTING TITAN TEST ---');
        
        // Mock Message
        const mockMsg = {
            from: '9477078700@c.us',
            body: 'analyze sales for last month and give me a summary', 
            notifyName: 'DebugUser',
            reply: async (text) => {
                console.log(`[MOCK WA] Reply:
...................................................
${text}
...................................................`);
            }
        };

        console.log(`Sending message: "${mockMsg.body}"`);
        // Correct method signature: processOrder(instruction, client, sender)
        await titan.processOrder(mockMsg.body, mockClient, mockMsg.from);

        console.log('--- TEST COMPLETE ---');

    } catch (error) {
        console.error('CRITICAL ERROR:', error);
    }
})();
