const titan = require('../jobs/GrandCentralMind');
require('dotenv').config();

// Mock WhatsApp Client
const mockClient = {
    sendMessage: async (to, msg) => {
        console.log('---------------------------------------------------');
        console.log(`📱 MOCK CLIENT: Sending to [${to}]`);
        console.log(`📝 MESSAGE:`);
        console.log(msg);
        console.log('---------------------------------------------------');
        return true;
    }
};

(async () => {
    console.log('🧠 Initializing Titan Business Brain...');
    
    // Test Case 1: Simple Inquiry
    // const prompt1 = "Analyze the stock of DURATURN 175/70/13. Is it low?";
    // console.log(`\n\n🔎 TEST 1: "${prompt1}"`);
    // const reply1 = await titan.processOrder(prompt1, mockClient);
    // console.log(`🤖 TITAN REPLY:\n${reply1}`);

    // Test Case 2: The User's specific request
    // "Send database details to a number"
    const prompt2 = "View_Sales report whatsapp eken pahu giya masa 3 Duraturn sales balala, eka summary ekak widihata hadala 94771234567 kiyana number ekata yawanna. Sinhala bhashawen yawanna.";
    console.log(`\n\n🔎 TEST 2: "${prompt2}"`);
    
    const result = await titan.processOrder(prompt2, mockClient);
    console.log(`\n\n🤖 TITAN FINAL RESPONSE:\n${result || '(No text response provided by model)'}`);
    
    console.log('\n✅ Test Complete');
})();
