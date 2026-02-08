const { GoogleGenerativeAI } = require('@google/generative-ai');
require('dotenv').config({ path: 'c:\\whatsapp-sql-api\\.env' });

(async () => {
    console.log("Checking API Key:", process.env.GEMINI_API_KEY ? "Present" : "Missing");
    const genAI = new GoogleGenerativeAI(process.env.GEMINI_API_KEY);
    
    // There isn't a direct "listModels" on the main class in some versions, 
    // but let's try to infer or just try the most basic model "gemini-1.0-pro"
    
    const modelsToTry = [
        "gemini-1.5-flash",
        "gemini-1.5-flash-latest",
        "gemini-1.5-pro",
        "gemini-1.5-pro-latest",
        "gemini-1.0-pro",
        "gemini-pro"
    ];

    for (const m of modelsToTry) {
        console.log(`\nTesting model: ${m}`);
        try {
            const model = genAI.getGenerativeModel({ model: m });
            const result = await model.generateContent("Hi");
            console.log(`✅ SUCCESS with ${m}`);
            console.log(result.response.text());
            break; // Found one that works!
        } catch (e) {
            console.log(`❌ FAILED ${m}: ${e.message}`);
        }
    }
})();