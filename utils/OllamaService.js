// Ollama AI Service (100% FREE - runs locally!)
const { Ollama } = require('ollama');

class OllamaService {
  constructor() {
    const host = process.env.OLLAMA_HOST || 'http://localhost:11434';
    this.ollama = new Ollama({ host });
    
    // Use available model (check what's installed)
    this.defaultModel = 'phi:latest'; // User has phi installed
    this.isAvailable = false;
    
    this.init();
  }
  
  async init() {
    try {
      const models = await this.ollama.list();
      if (models && models.models && models.models.length > 0) {
        this.isAvailable = true;
        // Use first available model
        this.defaultModel = models.models[0].name;
        console.log(`✅ Ollama available with model: ${this.defaultModel}`);
      }
    } catch (err) {
      console.log('⚠️ Ollama not available, using templates');
      this.isAvailable = false;
    }
  }
  
  async enhanceContent(templateContent, productData) {
    if (!this.isAvailable) {
      return templateContent; // Fallback to template
    }
    
    try {
      const prompt = `Improve this Facebook post for a tyre business in Sri Lanka. Keep it engaging with emojis, use Sinhala/English mix (Singlish), maintain the price and contact info exactly as given. Make it more creative and catchy but don't change the core message:

Original post:
${templateContent}

Product: ${productData.brand} ${productData.name} ${productData.size}

Enhanced version (keep under 250 words):`;

      const response = await this.ollama.generate({
        model: this.defaultModel,
        prompt: prompt,
        stream: false,
        options: {
          temperature: 0.7,
          num_predict: 400
        }
      });
      
      return response.response || templateContent;
      
    } catch (err) {
      console.error('Ollama enhance error:', err.message);
      return templateContent; // Fallback
    }
  }
  
  async analyzeSentiment(text) {
    if (!this.isAvailable) {
      return this.keywordSentiment(text);
    }
    
    try {
      const prompt = `Analyze the sentiment of this customer message. Respond with ONLY ONE WORD: POSITIVE, NEGATIVE, or NEUTRAL.

Message: "${text}"

Sentiment:`;

      const response = await this.ollama.generate({
        model: this.defaultModel,
        prompt: prompt,
        stream: false,
        options: {
          temperature: 0.1,
          num_predict: 10
        }
      });
      
      const sentiment = response.response.trim().toUpperCase();
      
      if (sentiment.includes('POSITIVE')) return 'POSITIVE';
      if (sentiment.includes('NEGATIVE')) return 'NEGATIVE';
      return 'NEUTRAL';
      
    } catch (err) {
      console.error('Sentiment analysis error:', err.message);
      return this.keywordSentiment(text);
    }
  }

  // Generic text generation helper used by analysis scripts
  async generateText(prompt) {
    if (!this.isAvailable) {
      // Fallback: return the prompt tail to avoid failing the pipeline
      return 'AI analysis is currently unavailable. Please start Ollama or set OLLAMA_HOST.\n\nPrompt summary:\n' + (String(prompt).slice(-800) || '');
    }
    try {
      const response = await this.ollama.generate({
        model: this.defaultModel,
        prompt,
        stream: false,
        options: {
          temperature: 0.6,
          num_predict: 800
        }
      });
      return response.response || '';
    } catch (err) {
      console.error('Ollama generateText error:', err.message);
      return '';
    }
  }
  
  keywordSentiment(text) {
    const lower = text.toLowerCase();
    const positive = ['good', 'great', 'excellent', 'හොඳ', 'මරු', 'ස්තූති', 'thank'];
    const negative = ['bad', 'worst', 'poor', 'නරක', 'කුණු', 'අපහසු', 'problem'];
    
    const hasPos = positive.some(w => lower.includes(w));
    const hasNeg = negative.some(w => lower.includes(w));
    
    if (hasPos && !hasNeg) return 'POSITIVE';
    if (hasNeg && !hasPos) return 'NEGATIVE';
    return 'NEUTRAL';
  }
  
  async generateReply(comment, sentiment) {
    if (!this.isAvailable) {
      return this.templateReply(sentiment);
    }
    
    try {
      const prompt = `Generate a friendly, helpful reply to this customer comment on Facebook. Use Sinhala/English mix (Singlish). Include contact number 077-777-7777. Keep it short (under 100 words).

Customer comment: "${comment}"
Sentiment: ${sentiment}

Your reply:`;

      const response = await this.ollama.generate({
        model: this.defaultModel,
        prompt: prompt,
        stream: false,
        options: {
          temperature: 0.6,
          num_predict: 200
        }
      });
      
      return response.response || this.templateReply(sentiment);
      
    } catch (err) {
      console.error('Reply generation error:', err.message);
      return this.templateReply(sentiment);
    }
  }
  
  templateReply(sentiment) {
    if (sentiment === 'NEGATIVE') {
      return `🙏 අපි සමාවෙනවා මෙම අපහසුතාවය ගැන!\n\nඅපි වහාම මේක විසඳනවා. කරුණාකර අමතන්න:\n📞 077-777-7777\n\nඅපි priority එකක් විදිහට handle කරනවා!\n\n- Lasantha Tyre Management`;
    }
    
    if (sentiment === 'POSITIVE') {
      return `❤️ ස්තූතියි! ඔබේ අදහස අපට ශක්තියක්! 🙏\n\nඅපේ සේවාව ඔබට කැමති වීම සතුටක්!\n\n☎️ 077-777-7777\n#LasanthaTyre`;
    }
    
    return `හායි! 👋 ස්තූතියි comment එකට!\n\nවැඩි විස්තර සඳහා අමතන්න:\n📞 077-777-7777\n\n#LasanthaTyre`;
  }
}

module.exports = OllamaService;
