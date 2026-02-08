// =================================================================
// Gemini API Client (geminiApiClient.js)
// Manages all API calls to Google's Gemini 2.5 Flash model.
// =================================================================

const { GoogleGenerativeAI, HarmCategory, HarmBlockThreshold } = require('@google/generative-ai');

// Load environment variables
require('dotenv').config();

const API_TIMEOUT_MS = 45000; // 45-second timeout for API calls
const MAX_RETRIES = 5; // Retry failed requests 5 times for better reliability

// --- Multi-Key System ---
const geminiKeys = [];
if (process.env.GEMINI_API_KEY) geminiKeys.push(process.env.GEMINI_API_KEY);
for (let i = 2; i <= 10; i++) {
    const key = process.env[`GEMINI_API_KEY_${i}`];
    if (key) geminiKeys.push(key);
}

let currentKeyIndex = 0;
let genAI = new GoogleGenerativeAI(geminiKeys[0] || 'MISSING_KEY');

function rotateKey() {
    if (geminiKeys.length <= 1) return false;
    currentKeyIndex = (currentKeyIndex + 1) % geminiKeys.length;
    genAI = new GoogleGenerativeAI(geminiKeys[currentKeyIndex]);
    console.log(`🔄 [Gemini Client] Rotated to Key ${currentKeyIndex + 1}/${geminiKeys.length} (${geminiKeys[currentKeyIndex].slice(-4)})`);
    return true;
}

const MODEL_NAME = 'gemini-flash-lite-latest'; // RESTORED WORKING MODEL (2.0 Quota exceeded)

// Helper to get model with current key
function getModel(config = {}) {
    return genAI.getGenerativeModel({
        model: MODEL_NAME,
        ...config,
        safetySettings: [
            { category: HarmCategory.HARM_CATEGORY_HARASSMENT, threshold: HarmBlockThreshold.BLOCK_ONLY_HIGH },
            { category: HarmCategory.HARM_CATEGORY_HATE_SPEECH, threshold: HarmBlockThreshold.BLOCK_ONLY_HIGH },
        ],
    });
}
// ------------------------

// The master prompt defining the AI's behavior
const MASTER_PROMPT = `
# Master System Prompt

**1. Your Identity:**
You are a friendly, expert, and very polite sales assistant for 'Lasantha Tyre Traders', located in Thalawathugoda (Phone: 0721222509, Admin: 0771222509).

**2. Core Rules:**
* **Language:** The user's language (Sinhala, English, or mixed) *must* be auto-detected. You *must* reply *only* in that same language. Your Sinhala must be natural and human-like, not robotic.
* **Tone:** Your tone must always be respectful, helpful, and professional ('lassanata wadagath widihata').
* **Pricing:** This is a private WhatsApp chat. You are *fully authorized* to share prices, stock levels, and warranty information. This rule is different from the Facebook *public comment* rules.

**3. Your Goal:**
Your primary goal is to understand the user's need (tyre size, brand, vehicle number), provide them with accurate price and stock information (which will be given to you by the system), and guide them towards a successful sale.

**4. Business Knowledge:**
* **Services:** You are aware of services like Wheel Alignment (Available ONLY between 07:30 AM - 06:00 PM), Balancing, and Nitrogen.
* **Location:** 1035 Pannipitiya Road, Battaramulla 10230, Sri Lanka (Google Maps: https://maps.app.goo.gl/frJHWEo4oRYYiJqw9). Landmarks: Opposite Glomark Supermarket (Thalawathugoda branch), Near Hiru TV Studio.
* **Business Hours:** Open EVERY DAY from 07:00 AM to 09:00 PM.
* **Contact:** Phone: 0112 773 232, Mobile: 077 731 1770.
* **Rating:** 4.3 stars with 589+ positive customer reviews.
* **Vehicle History:** You can look up vehicle history if a number is provided (the system will provide this data).
* **Warranty Policy (CRITICAL - Follow exactly):** 
  - **Coverage Duration:** Warranty is valid for EITHER 40,000 km OR 50,000 km (depending on brand/pattern) OR 4 years from billing date - **whichever milestone is reached FIRST**.
  - **STRICT RULE:** NEVER mention warranty details unless the customer SPECIFICALLY asks about "warranty", "guarantee", or "durability".
  - **Brand-Specific Mileage:** 
    * Premium brands (Bridgestone, Michelin, Continental, Yokohama): Usually 50,000 km
    * Mid-range brands (Maxxis, Dunlop, Hankook, Kumho): Usually 45,000 km
    * Economy brands (Duraturn, Federal, Giti, Ceat): Usually 40,000 km
  - **ABSOLUTE HARD LIMIT:** Do NOT promise warranty exceeding 50,000 km under ANY circumstances. If asked for more, politely decline and explain 50,000 km is our maximum.
  - **Who Handles Claims:** Tyre importer/manufacturer (not Lasantha Tyre Traders directly). We assist with the claim process.
  - **What IS Covered (Manufacturing Defects Only):**
    * Tread separation (rubber peeling from casing)
    * Material defects (air bubbles, uneven rubber density)
    * Workmanship issues (improper curing, mold defects)
  - **What is NOT Covered (Customer Responsibility):**
    * Road hazards: Potholes, curbs, sharp objects, nails
    * Punctures, cuts, or impact damage
    * Accidents or collisions
    * Improper use: Overloading, under-inflation, over-inflation
    * Normal wear and tear (tread worn to legal limit)
    * Uneven wear due to poor wheel alignment or suspension issues
  - **Mandatory for Claims:** Original invoice/bill with date and tyre details. NO invoice = NO warranty claim.
  - **How to Maximize Tyre Life:**
    * Check air pressure weekly (when tyres are cold)
    * Rotate tyres every 5k km (front-to-back, cross pattern)
    * Get wheel alignment checked every 15k km or if steering feels off
    * Avoid overloading (check vehicle's max load capacity)
    * Drive smoothly (avoid harsh braking, sharp turns, potholes)
  - **CRITICAL RESPONSE STYLE - Keep warranty explanations SHORT & SWEET:**
    * **STRICT RULE:** NEVER mention warranty details unless the customer SPECIFICALLY asks about "warranty", "guarantee", or "durability".
    * If the user asks for price/stock, DO NOT include warranty info in the reply.
    * Keep it concise - customers don't like long explanations
    * Focus on the essentials: Coverage km/years, what's covered, keep invoice
    * Use abbreviations: "5k km" not "5,000 km", "15k" not "15,000"
  - **When discussing warranty in Sinhala, use this CONCISE format:**
    * "🛡️ **Warranty:** [Brand අනුව 40k-50k km] හෝ 4 years - මුලින්ම එන එක. Manufacturing defects විතරයි. Road damage/accidents වලට නෑ. බිල්පත තියාගන්න."
    * Only add maintenance tips IF customer seems interested or asks follow-up questions
  - **When discussing warranty in English, use this CONCISE format:**
    * "🛡️ **Warranty:** [40k-50k km based on brand] OR 4 years - whichever first. Manufacturing defects only. No road damage/accidents. Keep your invoice."
    * Only add maintenance tips IF customer seems interested or asks follow-up questions

**5. Stock Availability Responses (CRITICAL - Keep it Simple):**
* When providing stock information, ALWAYS use English term "Stock available"
* DO NOT say in Sinhala: "ඔබ 1 ටයරයක් ඉල්ලුවා – ඔව්, 1 මාගෙන තියනවා" ❌
* DO NOT repeat what customer asked ❌
* CORRECT responses:
  - "Stock available" ✅ (preferred - clear and professional)
  - "තියෙනවා" ✅ (acceptable if customer used pure Sinhala)
* Keep it short - one or two words maximum
* Match the customer's language style naturally

**6. Greeting & Discount Policy (CRITICAL - Follow Exactly):**
* **Greeting for returning customers (OPTIONAL - use sparingly):**
  - You MAY use a brief greeting like: "නැවතත් ඔබව දැකීම සතුටක්!" 
  - This is OPTIONAL - not required for every message
  - Keep it short and natural
* **Discount Policy:**
  - **IF offering discount:** ONLY mention 1% discount maximum
  - Format: "අපගේ නැවත පැමිණෙන පාරිභෝගිකයෙකු ලෙස, ඔබට අද මිලදී ගැනීම සඳහා 1% ක වට්ටමක් ලැබෙනවා."
  - Show calculation: "1% ක වට්ටමකින් පසුව රු. [calculated_price]/="
  - **NEVER** offer more than 1% discount
  - **ONLY** offer discount if system data suggests customer is returning/valuable
  - If unsure, DO NOT offer any discount - just show regular price
* **If customer asks for discount:**
  - Redirect to phone: "මිල ගැන වැඩිදුර සාකච්ඡා කිරීමට කරුණාකර 077 731 1770 අමතන්න"

**7. Location Mentions (Keep it Simple):**
* When mentioning location/showroom, use this SHORT format:
  - "තලවතුගොඩ පිහිටි අපගේ ශාඛාව වෙත පැමිණෙන්න" ✅
  - DO NOT say: "Glomark Supermarket (Thalawathugoda ශාඛාව) ඉදිරිපිට පිහිටි අපගේ ප්‍රදර්ශනාගාරයට පැමිණෙන්න" ❌
* Only mention landmarks if customer asks for directions specifically
* Keep location references brief and natural

**8. Fallback Rule:**
If you are uncertain about a request, *do not guess*. Politely ask the user to clarify or redirect them to a human admin by saying, "For this specific query, please contact our senior staff at 0771222509 for the best assistance."
`;

// ---------------------------------------------------------
// NEW: Dedicated Translation Function
// Supports the "Separate Translator" Concept
// ---------------------------------------------------------
async function translateToSinhala(text) {
    try {
        const model = getModel({ generationConfig: { temperature: 0.1 } });
        const prompt = `Translate the following English text to natural, conversational Sinhala (appropriate for a business chat). 
Do not translate brand names (like 'Lasantha Tyre', 'Dunlop', 'Toyota') or technical terms (like 'Alignment', 'Warranty'). Keep them in English.
Make it sound polite and helpful.

Text to translate:
"${text}"

Sinhala Translation:`;

        const result = await model.generateContent(prompt);
        return (await result.response).text().trim();
    } catch (error) {
        console.error("Translation failed:", error);
        return text; // Fallback to original
    }
}

/**
 * Creates a promise that rejects after a specified timeout.
 * @param {number} timeout - The timeout in milliseconds.
 * @returns {Promise<never>}
 */
const createTimeout = (timeout) => {
    return new Promise((_, reject) => {
        setTimeout(() => {
            reject(new Error(`API call timed out after ${timeout}ms`));
        }, timeout);
    });
};

/**
 * Retry wrapper for API calls with exponential backoff
 * @param {Function} apiCall - The async function to retry
 * @param {number} maxRetries - Maximum number of retries
 * @param {number} baseDelay - Base delay in ms (will be multiplied for exponential backoff)
 * @returns {Promise} - Result of the API call
 */
async function withRetry(apiCall, maxRetries = MAX_RETRIES, baseDelay = 2000) {
    let lastError;
    
    for (let attempt = 0; attempt <= maxRetries; attempt++) {
        try {
            return await apiCall();
        } catch (error) {
            lastError = error;
            const errMsg = error.message || '';

            // Check for Quota Exceeded (429) -> ROTATE KEY
            if (errMsg.includes('429') || errMsg.includes('Quota') || errMsg.includes('quota')) {
                console.log(`⚠️ [Gemini Client] Quota exceeded on Key ${currentKeyIndex + 1}. Rotating...`);
                if (rotateKey()) {
                    // Start retries fresh or just continue loop? 
                    // Let's continue immediately with new key
                    // We don't increment 'attempt' significantly to allow full retries on new key
                    // But to avoid infinite loops, we still respect maxRetries limit roughly
                    await new Promise(resolve => setTimeout(resolve, 1000)); // Short delay for switch
                    continue; 
                } else {
                    console.log(`[Gemini Client] No more keys to rotate to.`);
                }
            }
            
            // Check if error is 503 (overload) - use longer delays
            const is503 = error.status === 503 || (errMsg.includes('overloaded'));
            
            // Don't retry on the last attempt
            if (attempt < maxRetries) {
                // Exponential backoff: 2s, 4s, 8s, 16s, 32s (or faster for non-503 errors)
                const delay = is503 
                    ? baseDelay * Math.pow(2, attempt) * 1.5 // Longer delays for overload: 3s, 6s, 12s, 24s, 48s
                    : baseDelay * Math.pow(2, attempt); // Normal: 2s, 4s, 8s, 16s, 32s
                console.log(`[Gemini Retry] Attempt ${attempt + 1}/${maxRetries} failed (${is503 ? '503 Overload' : 'Error'}), retrying in ${Math.round(delay/1000)}s...`);
                await new Promise(resolve => setTimeout(resolve, delay));
            }
        }
    }
    
    // If all retries failed, throw the last error
    throw lastError;
}

/**
 * Step 1: Analyzes the user's message to extract intent and entities.
 * @param {string} userMessage - The user's latest message.
 * @param {Array} chatHistory - An array of previous messages in the conversation.
 * @returns {Promise<object>} - A JSON object with the analysis.
 */
async function analyzeMessage(userMessage, chatHistory = []) {
    const historyText = chatHistory.map(msg => `${msg.sender_type}: ${msg.message_text}`).join('\n');

    const prompt = `
        ${MASTER_PROMPT}

        **Analysis Task:**
        Based on the chat history and the new user message, analyze the user's request.
        
        **CRITICAL CONTEXT RULES:**
        1. If the user previously mentioned a tyre size (e.g., "195/65/15") and now mentions only a brand (e.g., "MAXXIS"), combine them together. The user wants that brand in that size.
        2. If the user previously mentioned a brand and now mentions a different size, they are asking about a NEW inquiry.
        3. If the user says words like "වෙනත්" (other), "තව" (more), "අනිත්" (another), they want MORE OPTIONS from the SAME size mentioned earlier.
        4. Always look at the last 3-5 messages to understand the full context.
        5. If size was mentioned in history but not in current message, check if current message is related (brand, asking for options, etc.) and include the size from history.

        **Chat History:**
        ${historyText || 'No history.'}

        **New User Message:** "${userMessage}"

        **Your Output MUST be a single, valid JSON object with the following structure:**
        {
            "language": "si | en | ta | singlish | mixed",
            "intent": "price_inquiry | stock_check | vehicle_lookup | quotation_request | greeting | general_question | unclear",
            "entities": {
                "tyre_size": "e.g., 195/65R15 or null (MUST extract from history if relevant)",
                "brand": "e.g., Maxxis or null",
                "vehicle_number": "e.g., ABC-1234 or null"
            },
            "confidence": "A score from 0.0 to 1.0",
            "contextUsed": "Explain if you used context from history (yes/no and why)"
        }
        
        **Examples:**
        - History: "user: 195/65/15", Current: "MAXXIS" → tyre_size: "195/65/15", brand: "MAXXIS" (used context)
        - History: "user: 195/65/15", "bot: Here are prices...", Current: "වෙනත් පැටර්න්" → tyre_size: "195/65/15", brand: null (asking for other brands in same size)
        - History: "user: MAXXIS", Current: "195/65/15" → tyre_size: "195/65/15", brand: "MAXXIS" (used context)
    `;

    try {
        // Use retry wrapper with timeout and reduced retries for faster fallback
        const result = await withRetry(async () => {
             // Get FRESH model instance on every retry to pick up key rotations
             const model = getModel({
                generationConfig: { responseMimeType: "application/json" }
             });
             return await Promise.race([
                model.generateContent(prompt),
                createTimeout(API_TIMEOUT_MS)
            ]);
        }, 2, 1000); // Only 2 retries (faster fallback to Claude)
        
        const response = result.response;
        let jsonText = response.text();

        // Robust JSON Extraction
        const firstOpen = jsonText.indexOf('{');
        const lastClose = jsonText.lastIndexOf('}');
        if (firstOpen !== -1 && lastClose !== -1) {
            jsonText = jsonText.substring(firstOpen, lastClose + 1);
        } else {
            // Fallback cleanup
            jsonText = jsonText.replace(/```json/g, '').replace(/```/g, '').trim();
        }

        try {
            return JSON.parse(jsonText);
        } catch (e) {
            console.error(`[Gemini JSON Parse Error] Raw text: ${response.text()}`);
            throw e;
        }
    } catch (error) {
        console.error('[Gemini Analyze Error]', error.message || error);
        
        // Check if Claude fallback is available
        if (process.env.ANTHROPIC_API_KEY) {
            console.log('[Gemini→Claude] Switching to Claude fallback for analysis...');
            try {
                const claudeClient = require('./claudeClient');
                return await claudeClient.analyzeMessage(userMessage, chatHistory);
            } catch (claudeError) {
                console.error('[Claude Fallback Error]', claudeError.message);
            }
        }
        
        throw new Error('API_ERROR');
    }
}

/**
 * Step 2: Generates a natural language response based on provided data.
 * @param {string} userMessage - The user's message.
 * @param {object} analysis - The result from analyzeMessage.
 * @param {object} data - The data fetched from the main database (e.g., price, stock).
 * @returns {Promise<string>} - The AI-generated text response.
 */
async function generateResponse(userMessage, analysis, data) {
    // textModel removed, use getModel inside retry

        const policies = (data && data.meta) ? data.meta : {};
        const prompt = `
        ${MASTER_PROMPT}

        **Response Generation Task:**
        Generate a natural, human-like response based on the user's message and the data provided by the system.

        **User's Message:** "${userMessage}"
        **Detected Language:** ${analysis.language}
        **Detected Intent:** ${analysis.intent}

        **System-Provided Data:**
        ${JSON.stringify(data, null, 2)}

        **Tyre Specifications (if available):**
        ${data.tyreSpecs ? `
        This tyre has the following detailed specifications:
        - Brand: ${data.tyreSpecs.brand}
        - Pattern: ${data.tyreSpecs.pattern || 'Not specified'}
        - EU Label: Fuel Efficiency: ${data.tyreSpecs.euLabel.fuelEfficiency || 'N/A'}, Wet Grip: ${data.tyreSpecs.euLabel.wetGrip || 'N/A'}, Noise: ${data.tyreSpecs.euLabel.noiseLevel || 'N/A'} dB (Class ${data.tyreSpecs.euLabel.noiseClass || 'N/A'})
        - Performance: Dry Grip: ${data.tyreSpecs.performance.dryGrip || 'N/A'}/10, Comfort: ${data.tyreSpecs.performance.comfort || 'N/A'}/10, Durability: ${data.tyreSpecs.performance.durability || 'N/A'}/10, Handling: ${data.tyreSpecs.performance.handling || 'N/A'}/10
        - Category: ${data.tyreSpecs.classification.category || 'N/A'}
        - Terrain: ${data.tyreSpecs.classification.terrain || 'N/A'}
        - Season: ${data.tyreSpecs.classification.season || 'N/A'}
        - Speed Rating: ${data.tyreSpecs.technical.speedRating || 'N/A'}
        - Load Index: ${data.tyreSpecs.technical.loadIndex || 'N/A'}
        - Max Speed: ${data.tyreSpecs.technical.maxSpeed || 'N/A'} km/h
        - Best For: ${data.tyreSpecs.features.bestFor || 'N/A'}
        - Warranty (INTERNAL - DO NOT SHARE unless explicitly asked): ${
            data.tyreSpecs.classification.category === 'Motorbike' 
                ? 'Not applicable for motorcycle tyres' 
                : (data.tyreSpecs.features.warranty && data.tyreSpecs.features.warranty <= 40000 
                    ? (data.tyreSpecs.features.warranty / 1000).toFixed(0) + ' km (conditions apply)' 
                    : 'N/A')
        }
        - Special Technology: ${data.tyreSpecs.technology.special || 'None'}
        - Run Flat: ${data.tyreSpecs.technology.runFlat ? 'Yes' : 'No'}
        - Reinforced: ${data.tyreSpecs.technology.reinforced ? 'Yes' : 'No'}
        
        **IMPORTANT:** Use these specifications to provide expert advice. Mention the most relevant specs based on the user's needs (e.g., if they ask about fuel efficiency, highlight the EU Label fuel rating; if they ask about durability, mention the durability rating and warranty).
        ` : 'No detailed specifications available for this tyre.'}

                **Policies & Context (apply strictly):**
        - isFirstReply: ${policies.isFirstReply ? 'true' : 'false'}
        - allowStockDetails: ${policies.allowStockDetails ? 'true' : 'false'}
        - requireExplicitDetails: ${policies.requireExplicitDetails ? 'true' : 'false'}
        - suppressQtyByDefault (for non-admin): ${policies.suppressQtyByDefault ? 'true' : 'false'}
                - requestedQty (only if user asked): ${Number.isInteger(policies.requestedQty) ? policies.requestedQty : 'null'}
                - totalStockForSize: ${typeof policies.totalStock === 'number' ? policies.totalStock : 'null'}
        - detailRequests: ${JSON.stringify(policies.detailRequests || {})}

        **Instructions:**
        - Reply in **ENGLISH** first (for internal accuracy).
        - Use the provided data to answer the user's query accurately.
        - If data indicates an error (e.g., "Item not found"), inform the user politely.
        - Maintain the persona defined in the Master Prompt.
        - Keep the response concise and clear.
        
        Generate the English response now.
        
        **Marketing & Sales Strategy (apply where appropriate):**
        - **Expert Advisor:** When providing tyre details, add a short, valuable, one-sentence tip. Examples: "This tyre is excellent for fuel efficiency." or "For best performance, we recommend rotating your tyres every 5,000 km."
        - **Personalized Offers:**
            - If data.meta.isReturningCustomer is true, add a warm welcome: "It's great to see you again! As a returning customer, you get a special 5% discount on your purchase today."
            - If data.meta.isBulkInquiry is true (e.g., user asks for 4 tyres), present this special offer: "Since you're looking for a set of 4 tyres, we have a special package for you: FREE N2 filling, a FREE complete suspension inspection, and FREE tyre fixing."
        - **Trust & Transparency:**
            - If an item is out of stock, don't just say it's unavailable. Say: "We're currently out of that specific tyre, but we expect a new shipment within 2-3 days. Would you like us to reserve it for you?"
            - Always mention that prices are all-inclusive to build trust.
        - **Convenience:**
            - Where appropriate, mention delivery options: "We can also arrange for delivery within the Colombo area."

                - Style guidelines: short paragraphs or bullet points, clear call-to-action, friendly but professional tone, a touch of warmth.
                - Greeting policy:
                    - If isFirstReply is true, start with "Dear Sir/Madam," (only once at the beginning of the conversation). Otherwise, DO NOT include any such salutation.
                                - Stock & details policy:
                                    - For ALL users, only share specific details if the user explicitly asked (requireExplicitDetails=true) AND the corresponding flag in detailRequests is true.
                                    - If allowStockDetails is true (admin or approved number), you may include stock counts and DB details ONLY when the user explicitly asked for them (detailRequests.stock/brand/pattern/details).
                                    - If allowStockDetails is false (regular customers), DO NOT include stock counts or internal DB fields by default; show only description and price unless the user explicitly requested those details. Even then, be concise and share only what was asked.
                        - Quantity policy (very important):
                            - If suppressQtyByDefault is true (regular customers), DO NOT mention any stock quantities by default.
                            - ONLY if requestedQty is present AND 1 <= requestedQty <= 5, add ONE short confirmation line at the end:
                        - If totalStockForSize >= requestedQty: in English: "You asked for ${'${requestedQty}'} tyres — yes, ${'${requestedQty}'} in stock." In Sinhala: keep the same meaning (e.g., "ඔබ ${'${requestedQty}'} ටයර් ඉල්ලුවා – ඔව්, ${'${requestedQty}'} මාගෙන තියනවා").
                        - If totalStockForSize < requestedQty: English: "You asked for ${'${requestedQty}'} tyres — currently ${'${totalStockForSize}'} in stock." Sinhala: provide the same meaning.
                    - Otherwise, never expose stock counts.
                - Do not repeat "Dear Sir/Madam" on follow-up messages.
                - At the end, if appropriate, add a subtle CTA with our number 0771222509 and location Thalawathugoda.
                - **Your output must be only the text of the reply, with no extra code fences.**
    `;

    try {
        // Use retry wrapper with timeout and reduced retries for faster fallback
        const result = await withRetry(async () => {
             const textModel = getModel();
             return await Promise.race([
                textModel.generateContent(prompt),
                createTimeout(API_TIMEOUT_MS)
            ]);
        }, 2, 1000); // Only 2 retries
        
        let finalText = result.response.text().trim();

        // ------------------------------------------------
        // NEW: Apply Translation if User's Language is Sinhala
        // ------------------------------------------------
        if (analysis.language === 'sinhala' || analysis.language === 'mixed' || analysis.language === 'singlish') {
            console.log(`🇱🇰 [Gemini Client] Translating response to Sinhala (detected: ${analysis.language})...`);
            finalText = await translateToSinhala(finalText);
        }

        return finalText;
    } catch (error) {
        console.error('[Gemini Generate Error]', error.message || error);
        
        // Check if Claude fallback is available
        if (process.env.ANTHROPIC_API_KEY) {
            console.log('[Gemini→Claude] Switching to Claude fallback for response generation...');
            try {
                const claudeClient = require('./claudeClient');
                return await claudeClient.generateResponse(userMessage, analysis, data);
            } catch (claudeError) {
                console.error('[Claude Fallback Error]', claudeError.message);
            }
        }
        
        throw new Error('API_ERROR');
    }
}

/**
 * Generates a customer-ready message for an Admin Co-pilot command.
 * @param {object} data - The data fetched from the main database.
 * @returns {Promise<string>} - The AI-generated text response for the admin.
 */
async function generateAdminResponse(data) {
    // textModel removed, use getModel inside retry
    const prompt = `
        ${MASTER_PROMPT}

        **Admin Co-pilot Task:**
        You are assisting an admin. An admin used the '/price' command.
        Format the following system data into a polite, professional, customer-ready message.

        **System-Provided Data:**
        ${JSON.stringify(data, null, 2)}

        **Instructions:**
        - The message should be ready to be forwarded to a customer.
        - Use a friendly and helpful tone.
        - Include all relevant details: price, stock, brand, pattern, warranty.
        - Add the store's contact number (0721222509) for follow-up.
        - **Your output must be only the text of the reply.**
    `;
     try {
        // Use retry wrapper with timeout
        const result = await withRetry(async () => {
            const textModel = getModel();
            return await Promise.race([
                textModel.generateContent(prompt),
                createTimeout(API_TIMEOUT_MS)
            ]);
        });
        
        return result.response.text().trim();
    } catch (error) {
        console.error('[Gemini Admin Error]', error);
        throw new Error('API_ERROR');
    }
}


module.exports = {
    analyzeMessage,
    generateResponse,
    generateAdminResponse,
};
