
const { GoogleGenerativeAI } = require("@google/generative-ai");
require('dotenv').config({ path: __dirname + '/../.env' }); // Adjust path as needed

async function listModels() {
  console.log("Checking available models...");
  
  if (!process.env.GEMINI_API_KEY) {
    console.error("❌ GEMINI_API_KEY is missing in .env");
    return;
  }

  // We might need to use a raw fetch because the SDK simplifies this out usually,
  // but let's try to query the API directly to see what it says.
  const apiKey = process.env.GEMINI_API_KEY;
  const url = `https://generativelanguage.googleapis.com/v1beta/models?key=${apiKey}`;

  try {
    const fetch = (await import('node-fetch')).default;
    const response = await fetch(url);
    
    if (!response.ok) {
      console.error(`HTTP Error: ${response.status} ${response.statusText}`);
      const text = await response.text();
      console.error(text);
      return;
    }

    const data = await response.json();
    console.log("--- Available Models ---");
    if (data.models) {
        data.models.forEach(m => {
            console.log(`Name: ${m.name}`);
            console.log(`Supported Methods: ${JSON.stringify(m.supportedGenerationMethods)}`);
            console.log("---");
        });
    } else {
        console.log("No models returned.");
        console.log(JSON.stringify(data, null, 2));
    }
    
  } catch (error) {
    console.error("Error listing models:", error);
  }
}

listModels();
