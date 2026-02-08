/**
 * Gemini Multi-Key AI Service with Auto-Rotation
 * Rotates between multiple API keys when quota exceeded
 * Fallback: Sinhala/English templates (no errors shown)
 */

const { GoogleGenerativeAI } = require('@google/generative-ai');
const Groq = require('groq-sdk');

class MultiAIService {
    constructor() {
        // Groq Initialization (Primary High-Speed)
        this.groq = new Groq({ apiKey: process.env.GROQ_API_KEY });
        this.groqModel = 'llama-3.3-70b-versatile';

        // Initialize Multiple Gemini Keys - rotate when quota exceeded!
        this.geminiKeys = this._loadGeminiKeys();
        this.currentKeyIndex = 0;
        this.keyFailCounts = {}; // Track failures per key
        
        // Initialize first Gemini instance
        this.gemini = this.geminiKeys.length > 0 
            ? new GoogleGenerativeAI(this.geminiKeys[0]) 
            : null;
        
        // Stats tracking
        this.stats = {
            gemini: { success: 0, fail: 0 },
            template: { used: 0 },
            keyRotations: 0
        };
        
        // Rate limiting - be conservative
        this.lastGeminiCall = 0;
        this.geminiMinInterval = 3000; // 3 seconds between calls
        
        // Cooldown - only when ALL keys exhausted
        this.allKeysExhausted = false;
        this.exhaustedTime = 0;
        this.exhaustedCooldown = 60000; // 1 minute cooldown
        
        console.log('🤖 [MultiAI] Initialized with Key Rotation:', {
            totalKeys: this.geminiKeys.length,
            status: this.geminiKeys.length > 0 ? '✅ Ready' : '❌ No API Keys'
        });
    }

    /**
     * Load all Gemini API keys from environment
     * Supports: GEMINI_API_KEY, GEMINI_API_KEY_2, GEMINI_API_KEY_3, etc.
     */
    _loadGeminiKeys() {
        const keys = [];
        
        // Primary key
        if (process.env.GEMINI_API_KEY) {
            keys.push(process.env.GEMINI_API_KEY);
        }
        
        // Additional keys (GEMINI_API_KEY_2, GEMINI_API_KEY_3, etc.)
        for (let i = 2; i <= 10; i++) {
            const key = process.env[`GEMINI_API_KEY_${i}`];
            if (key) {
                keys.push(key);
            }
        }
        
        console.log(`🔑 [MultiAI] Loaded ${keys.length} Gemini API key(s)`);
        return keys;
    }

    /**
     * Rotate to next available key
     */
    _rotateKey() {
        if (this.geminiKeys.length <= 1) return false;
        
        const previousIndex = this.currentKeyIndex;
        this.currentKeyIndex = (this.currentKeyIndex + 1) % this.geminiKeys.length;
        
        // Check if we've tried all keys
        if (this.currentKeyIndex === 0) {
            // Full rotation completed - all keys might be exhausted
            const allFailed = Object.keys(this.keyFailCounts).length >= this.geminiKeys.length;
            if (allFailed) {
                this.allKeysExhausted = true;
                this.exhaustedTime = Date.now();
                console.log('⚠️ [MultiAI] All keys exhausted, using templates for 1 minute');
                return false;
            }
        }
        
        // Switch to new key
        this.gemini = new GoogleGenerativeAI(this.geminiKeys[this.currentKeyIndex]);
        this.stats.keyRotations++;
        
        console.log(`🔄 [MultiAI] Rotated to key ${this.currentKeyIndex + 1}/${this.geminiKeys.length}`);
        return true;
    }

    /**
     * Reset key failure tracking (call periodically)
     */
    _resetKeyFailures() {
        this.keyFailCounts = {};
        this.allKeysExhausted = false;
    }

    /**
     * Get AI response with automatic fallback
     * @param {string} prompt - The prompt to send
     * @param {object} options - Options like language, context
     * @returns {Promise<{text: string, provider: string, cached: boolean}>}
     */
    async chat(prompt, options = {}) {
        const { 
            language = 'auto',
            context = '',
            systemPrompt = '',
            maxTokens = 500,
            temperature = 0.7
        } = options;

        // Check if all keys exhausted and in cooldown
        if (this.allKeysExhausted) {
            if ((Date.now() - this.exhaustedTime) > this.exhaustedCooldown) {
                // Cooldown over, reset and try again
                this._resetKeyFailures();
            } else {
                this.stats.template.used++;
                return { 
                    text: this._getTemplateResponse(prompt, options), 
                    provider: 'template', 
                    cached: false 
                };
            }
        }

        // Build full prompt with Sinhala emphasis
        // Change: If Sinhala, force English for Logic, then Translate
        const originalLanguage = language;
        const processLanguage = (language === 'sinhala') ? 'english' : language; // Force English for generation
        
        let finalGeneratedText = null;
        let providerUsed = null;

        // 1. Try Groq (Primary - Free & Fast)
        try {
            // console.log(`🤖 [MultiAI] Trying Groq (Llama 3)...`);
            const groqPrompt = this._buildPrompt(prompt, { language: processLanguage, context, systemPrompt: '' }); // System prompt passed separately if needed, but here we bake it in or pass to method
            
            // For Groq, we can pass systemPrompt distinct from user prompt if we want, but _buildPrompt combines them. 
            // larger context window allows combined.
            finalGeneratedText = await this._tryGroq(prompt, { maxTokens, temperature, systemPrompt: systemPrompt + (context ? `\nContext: ${context}` : '') });
            providerUsed = 'groq-llama3';
        } catch (groqErr) {
            console.warn(`⚠️ [MultiAI] Groq failed: ${groqErr.message}. Falling back to Gemini.`);
        }

        // 2. Try Gemini (Fallback) if Groq failed
        if (!finalGeneratedText && this.gemini) {
            const fullPrompt = this._buildPrompt(prompt, { language: processLanguage, context, systemPrompt });

            // Try current key and rotate on failure
            for (let attempt = 0; attempt < this.geminiKeys.length; attempt++) {
                try {
                    console.log(`🤖 [MultiAI] Trying Key ${this.currentKeyIndex + 1}/${this.geminiKeys.length} (${this.geminiKeys[this.currentKeyIndex].slice(-5)})`);
                    const result = await this._tryGemini(fullPrompt, { maxTokens, temperature });
                    finalGeneratedText = result;
                    providerUsed = `gemini-key${this.currentKeyIndex + 1}`;
                    
                    // Clear failure count for successful key
                    delete this.keyFailCounts[this.currentKeyIndex];
                    break; 
                } catch (err) {
                    this.stats.gemini.fail++;
                    console.error(`❌ [MultiAI] Key ${this.currentKeyIndex + 1} Error: ${err.message.slice(0, 200)}`);

                    const msg = (err && err.message) ? err.message : '';
                    const shouldRotate = msg.includes('429') || msg.includes('403') || msg.includes('404') || msg.toLowerCase().includes('quota');

                    if (shouldRotate) {
                        this.keyFailCounts[this.currentKeyIndex] = true;
                        console.log(`⏸️ [MultiAI] Key ${this.currentKeyIndex + 1} failed (rotating...)`);

                        if (!this._rotateKey()) {
                            break; // All keys exhausted
                        }

                        console.log(`⏳ [MultiAI] Waiting 5s before next key attempt...`);
                        await new Promise(resolve => setTimeout(resolve, 5000));
                    } else {
                        break;
                    }
                }
            }
        }

        // Translation Layer (applied to whichever provider succeeded)
        if (finalGeneratedText) {
            if (originalLanguage === 'sinhala' || originalLanguage === 'mixed') {
                // console.log('🇱🇰 [MultiAI] Translating response to Sinhala...');
                finalGeneratedText = await this._translateToSinhala(finalGeneratedText);
            }
            return { text: finalGeneratedText, provider: providerUsed, cached: false };
        }

        // Fallback to template (silent, no errors)
        this.stats.template.used++;
        return { 
            text: this._getTemplateResponse(prompt, options), 
            provider: 'template', 
            cached: false 
        };
    }

    /**
     * Check if in cooldown period (deprecated - now using allKeysExhausted)
     */
    _isInCooldown() {
        return this.allKeysExhausted && (Date.now() - this.exhaustedTime) < this.exhaustedCooldown;
    }

    /**
     * Tyre-specific query with database context
     */
    async tyreQuery(userMessage, tyreData = [], options = {}) {
        const systemPrompt = `ඔබ ලංකාවේ ලසන්ත ටයර් හවුස් හි උපකාරී සහායකයෙක්.
You are a helpful assistant for Lasantha Tyre House in Sri Lanka.

ගැනුම්කරුට ඔවුන්ගේ භාෂාවෙන්ම (සිංහල හෝ English) පිළිතුරු දෙන්න.
Be friendly, professional, and respond in the customer's language.

මිල ගැන අහනවා නම්, මිල Rs. වලින් පෙන්වන්න.
If asked about prices, show in Sri Lankan Rupees (Rs.).

Current tyre inventory:
${JSON.stringify(tyreData.slice(0, 10), null, 2)}`;

        return this.chat(userMessage, {
            ...options,
            systemPrompt,
            context: 'tyre_shop'
        });
    }

    /**
     * General conversation
     */
    async conversation(userMessage, chatHistory = [], options = {}) {
        const historyContext = chatHistory
            .slice(-5)
            .map(m => `${m.role}: ${m.content}`)
            .join('\n');

        const systemPrompt = `ඔබ ලසන්ත ටයර් හවුස් හි උපකාරී සහායකයෙක්.
You are a helpful assistant for Lasantha Tyre House.

ගැනුම්කරුගේ භාෂාවෙන්ම පිළිතුරු දෙන්න (සිංහල හෝ English).
Respond naturally in the customer's language.

Previous conversation:
${historyContext}`;

        return this.chat(userMessage, {
            ...options,
            systemPrompt
        });
    }

    // ============ Private Methods ============

    async _tryGroq(prompt, { maxTokens, temperature, systemPrompt }) {
        try {
            const completion = await this.groq.chat.completions.create({
                messages: [
                    ...(systemPrompt ? [{ role: "system", content: systemPrompt }] : []),
                    { role: "user", content: prompt }
                ],
                model: this.groqModel,
                temperature: temperature || 0.7,
                max_tokens: maxTokens || 1024,
            });
            return completion.choices[0]?.message?.content || '';
        } catch (e) {
            throw new Error(`Groq API Error: ${e.message}`);
        }
    }

    async _tryGemini(prompt, { maxTokens, temperature }) {
        // Rate limiting
        const now = Date.now();
        const timeSinceLastCall = now - this.lastGeminiCall;
        if (timeSinceLastCall < this.geminiMinInterval) {
            await this._sleep(this.geminiMinInterval - timeSinceLastCall);
        }
        this.lastGeminiCall = Date.now();

        const model = this.gemini.getGenerativeModel({ 
            model: 'gemini-flash-lite-latest',
            generationConfig: {
                maxOutputTokens: maxTokens,
                temperature
            }
        });

        const result = await model.generateContent(prompt);
        const response = await result.response;
        return response.text();
    }

    async _translateToSinhala(text) {
        try {
            const prompt = `You are a native Sri Lankan business professional.
Translate the message into clear, formal, and authoritative Sinhala (Sri Lankan).

Rules:
1. **Style:** Use professional "office Sinhala" (Formal/Written style). Avoid "Google Translate" or robotic phrasing.
2. **Numbers & Entities:** Keep ALL numbers, dates, currency (LKR), brand names, and technical terms exactly as is.
3. **Structure:** Keep newlines and bullet points exactly as in the original.
4. **Output:** ONLY the translated Sinhala text. No explanations.

Text: "${text}"`;

            // Use Groq for translation (High Quality, Fast)
            return await this._tryGroq(prompt, { maxTokens: 4096, temperature: 0.1 });
        } catch (e) {
            console.warn('Groq Translation failed, falling back to Gemini:', e.message);
            // Fallback to Gemini if Groq fails
            try {
                const model = this.gemini.getGenerativeModel({ model: 'gemini-flash-lite-latest' });
                const result = await model.generateContent(`Translate to Sinhala: ${text}`);
                return (await result.response).text().trim();
            } catch (geminiErr) {
                console.error('All translations failed:', geminiErr);
                return text;
            }
        }
    }

    _buildPrompt(prompt, { language, context, systemPrompt }) {
        let fullPrompt = '';
        
        if (systemPrompt) {
            fullPrompt += systemPrompt + '\n\n';
        }
        
        // Add language instruction
        if (language === 'sinhala') {
            fullPrompt += 'කරුණාකර සිංහලෙන් පිළිතුරු දෙන්න.\n';
        } else if (language === 'english') {
            fullPrompt += 'Please respond in English.\n';
        } else {
            fullPrompt += 'Respond in the same language as the user (Sinhala or English).\n';
        }
        
        fullPrompt += `User: ${prompt}`;
        
        return fullPrompt;
    }

    _getTemplateResponse(prompt, options = {}) {
        // Detect language from prompt
        const isSinhala = /[\u0D80-\u0DFF]/.test(prompt);
        const lowerPrompt = prompt.toLowerCase();
        
        // Greeting detection
        if (lowerPrompt.includes('hi') || lowerPrompt.includes('hello') || 
            lowerPrompt.includes('ayubowan') || prompt.includes('ආයුබෝවන්') ||
            lowerPrompt.includes('kohomada') || prompt.includes('කොහොමද')) {
            
            if (isSinhala) {
                return `👋 ආයුබෝවන්! ලසන්ත ටයර් හවුස් වෙත සාදරයෙන් පිළිගනිමු!

අපට ඔබට කොහොමද උදව් කරන්න පුළුවන්?

🛞 Tyre size එක එවන්න - මිල දැනගන්න (උදා: 185/65/15)
📞 Hot line: 077-1222509

ස්තුතියි! 🙏`;
            }
            return `👋 Hello! Welcome to Lasantha Tyre House!

How can we help you today?

🛞 Send tyre size for prices (e.g., 185/65/15)
📞 Hotline: 077-1222509

Thank you! 🙏`;
        }
        
        // Price inquiry
        if (lowerPrompt.includes('price') || lowerPrompt.includes('ගාන') || 
            lowerPrompt.includes('මිල') || lowerPrompt.includes('කීයද')) {
            
            if (isSinhala) {
                return `🛞 ටයර් මිල විමසීම

ඔබගේ විමසුමට ස්තුතියි! 

කරුණාකර tyre size එක එවන්න (උදා: 185/65/15)
අපි මිල කියන්නම්.

📞 Hot line: 077-1222509`;
            }
            return `🛞 Tyre Price Inquiry

Thank you for your inquiry!

Please send the tyre size (e.g., 185/65/15) and we'll provide current prices.

📞 Hotline: 077-1222509`;
        }
        
        // Stock inquiry
        if (lowerPrompt.includes('stock') || lowerPrompt.includes('available') || 
            lowerPrompt.includes('තියෙනවද') || lowerPrompt.includes('ඇත')) {
            
            if (isSinhala) {
                return `📦 Stock විමසීම

අපි ළඟ tyres වර්ග ගොඩක් තියෙනවා!

ඔබට ඕනේ size එක එවන්න - අපි check කරලා කියන්නම්.

📞 Hot line: 077-1222509`;
            }
            return `📦 Stock Inquiry

We have a wide range of tyres available!

Send the size you need and we'll check availability.

📞 Hotline: 077-1222509`;
        }

        // Thanks
        if (lowerPrompt.includes('thank') || lowerPrompt.includes('ස්තුති') || 
            lowerPrompt.includes('thanks')) {
            
            if (isSinhala) {
                return `🙏 ඔබටත් ස්තුතියි!

අපේ service එක ගැන සතුටු වෙනවා නම් අපිට ඉතාමත් සතුටුයි.
ඕනෑම වෙලාවක එන්න!

📞 077-1222509`;
            }
            return `🙏 Thank you!

We're glad to help! Visit us anytime.

📞 077-1222509`;
        }
        
        // Default response - bilingual
        if (isSinhala) {
            return `👋 ලසන්ත ටයර් හවුස් වෙත ස්වාගතයි!

ඔබට උදව් කරන්න අපි ready!

🛞 Tyre size එවන්න - මිල දැනගන්න
📦 Stock ගැන දැනගන්න
📞 Call: 077-1222509

ස්තුතියි! 🙏`;
        }
        
        return `👋 Welcome to Lasantha Tyre House!

We're here to help!

🛞 Send tyre size for prices (e.g., 185/65/15)
📦 Check stock availability  
📞 Call: 077-1222509

Thank you! 🙏`;
    }

    _sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    /**
     * Get service statistics
     */
    getStats() {
        return {
            ...this.stats,
            inCooldown: this._isInCooldown(),
            cooldownRemaining: this._isInCooldown() 
                ? Math.ceil((this.exhaustedCooldown - (Date.now() - this.exhaustedTime)) / 1000) 
                : 0
        };
    }

    /**
     * Health check
     */
    async healthCheck() {
        const results = { gemini: false };

        if (this.gemini && !this._isInCooldown()) {
            try {
                await this._tryGemini('Say "OK"', { maxTokens: 10, temperature: 0 });
                results.gemini = true;
            } catch {}
        }

        return results;
    }

}

let instance = null;

function getMultiAI() {
    if (!instance) {
        instance = new MultiAIService();
    }
    return instance;
}

module.exports = {
    MultiAIService,
    getMultiAI
};
