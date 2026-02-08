
const axios = require('axios');
require('dotenv').config({ path: __dirname + '/../.env' });

async function listModels() {
  console.log("Checking available models...");
  
  if (!process.env.GEMINI_API_KEY) {
    console.error("❌ GEMINI_API_KEY is missing in .env");
    return;
  }

  const apiKey = process.env.GEMINI_API_KEY;
  // Endpoint to list models
  const url = `https://generativelanguage.googleapis.com/v1beta/models?key=${apiKey}`;

  try {
    const response = await axios.get(url);
    
    console.log("--- Available Models ---");
    if (response.data && response.data.models) {
        response.data.models.forEach(m => {
            // Filter only models that support generateContent
            if (m.supportedGenerationMethods && m.supportedGenerationMethods.includes('generateContent')) {
                 console.log(`Name: ${m.name}`);
                 console.log(`DisplayName: ${m.displayName}`);
                 console.log(`Methods: ${m.supportedGenerationMethods}`);
                 console.log("---");
            }
        });
    } else {
        console.log("No models returned or structure unknown.");
        console.log(JSON.stringify(response.data, null, 2));
    }
    
  } catch (error) {
    console.error("Error listing models:");
    if (error.response) {
        console.error(`Status: ${error.response.status}`);
        console.error(`Data: ${JSON.stringify(error.response.data)}`);
    } else {
        console.error(error.message);
    }
  }
}

listModels();
