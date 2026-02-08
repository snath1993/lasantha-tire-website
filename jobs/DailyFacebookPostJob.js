// Daily Facebook Post Job - Claude AI Only
// Isolated from other jobs - unique post IDs prevent conflicts
const cron = require('node-cron');
const axios = require('axios');
const fs = require('fs');
const path = require('path');
const { createCanvas } = require('canvas');
const ImageGenerator = require('../utils/ImageGenerator');
const { ConfigService } = require('../utils/ConfigService');
const { getTemplates } = require('../utils/SocialTemplates');
const { fetchTrends } = require('../utils/WebEnricher');
const { loadStyleGuide } = require('../utils/socialStyle');
const { loadPostStyleGuide } = require('../utils/postStyleLoader');
const { getCaptionLinter } = require('../utils/CaptionLinter');
const { getABVariantGenerator } = require('../utils/ABVariants');
const { getEngagementBias } = require('../utils/EngagementBias');

class DailyFacebookPostJob {
  constructor(whatsappClient = null) {
    this.tasks = [];
    this.imageGen = new ImageGenerator();
    this.whatsapp = whatsappClient;
    this.pageId = process.env.FACEBOOK_PAGE_ID;
    this.accessToken = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
    this.adminWhatsAppNumber = process.env.ADMIN_WHATSAPP_NUMBER;
    
    // 🔄 Poster Cycling System - Tracks which posters have been published
    this.postFolder = path.join(__dirname, '..', 'post');
    this.processedFolder = path.join(this.postFolder, 'processed');
    this.historyFile = path.join(__dirname, '..', 'publish-history.json');
    this.posterHistory = this.loadPosterHistory();
    
    // 📝 Caption Cycling System - Tracks which captions have been used
    this.captionHistoryFile = path.join(__dirname, '..', 'caption-usage-history.json');
    this.fallbackCaptionsFile = path.join(__dirname, '..', 'fallback-captions.json');
    this.seasonalCaptionsFile = path.join(__dirname, '..', 'seasonal-captions.json');
    this.captionHistory = this.loadCaptionHistory();
    this.fallbackCaptions = this.loadFallbackCaptions();
    this.seasonalCaptions = this.loadSeasonalCaptions();
    
    // Supported poster formats
    this.supportedFormats = {
      image: ['.jpg', '.jpeg', '.png', '.gif', '.webp'],
      video: ['.mp4', '.mov', '.avi', '.mkv', '.webm', '.3gp', '.flv']
    };
    
    // Create folders if needed
    [this.postFolder, this.processedFolder].forEach(dir => {
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
      }
    });
    
    // AI Provider Selection - Gemini (fast) → Claude (reliable) → Fallback (guaranteed)
    // Default is 'gemini' for speed, but system auto-falls back to Claude if Gemini fails
    this.aiProvider = process.env.FB_POST_AI_PROVIDER || 'gemini'; // 'gemini' (default) or 'claude'
    
    // Initialize both AI providers if keys available
    // Gemini 2.5 Flash (Primary - Fastest)
    if (process.env.GEMINI_API_KEY) {
      try {
        const { GoogleGenerativeAI } = require('@google/generative-ai');
        this.genAI = new GoogleGenerativeAI(process.env.GEMINI_API_KEY);
        console.log('✅ Gemini 2.5 Flash ready (primary AI)');
      } catch (e) {
        console.warn('⚠️  Gemini initialization failed:', e.message);
      }
    } else {
      console.warn('⚠️  GEMINI_API_KEY not set - will use Claude or fallback captions');
    }
    
    // Claude 4.5 Haiku (Fallback - Most Reliable)
    if (process.env.ANTHROPIC_API_KEY) {
      console.log('✅ Claude 4.5 Haiku ready (fallback AI)');
    } else {
      console.warn('⚠️  ANTHROPIC_API_KEY not set - will use Gemini or fallback captions');
    }
    
    // Log caption generation strategy
    const hasGemini = !!process.env.GEMINI_API_KEY;
    const hasClaude = !!process.env.ANTHROPIC_API_KEY;
    if (hasGemini && hasClaude) {
      console.log('🎯 Caption Strategy: Gemini → Claude → Fallback (Triple Protection)');
    } else if (hasGemini) {
      console.log('🎯 Caption Strategy: Gemini → Fallback (Dual Protection)');
    } else if (hasClaude) {
      console.log('🎯 Caption Strategy: Claude → Fallback (Dual Protection)');
    } else {
      console.log('🎯 Caption Strategy: Fallback Captions Only (Always Works)');
    }
    
    this.pendingPosts = new Map();
    this.draftsDir = path.join(__dirname, '../facebook-drafts');
    if (!fs.existsSync(this.draftsDir)) { fs.mkdirSync(this.draftsDir, { recursive: true }); }

    // Publish mode: 'draft' (default) or 'publish'
    this.publishMode = String(process.env.FB_PUBLISH_MODE || 'draft').toLowerCase();
    // Approval mode: 'auto' (post immediately) is now the default. 'whatsapp' for manual approval.
    this.approvalMode = String(process.env.FB_APPROVAL_MODE || 'auto').toLowerCase();
  }
  
  setWhatsAppClient(client) { this.whatsapp = client; }
  
  // 🔄 Load poster publish history
  loadPosterHistory() {
    try {
      if (fs.existsSync(this.historyFile)) {
        const data = fs.readFileSync(this.historyFile, 'utf-8');
        return JSON.parse(data);
      }
    } catch (err) {
      console.warn('⚠️  Could not load poster history:', err.message);
    }
    return {
      published: [],
      lastPublish: null,
      totalPublished: 0,
      failedAttempts: []
    };
  }
  
  // 📝 Get caption for poster file
  async getCaptionForPoster(filename) {
    const captionsFolder = path.join(this.postFolder, 'captions');
    
    // 1️⃣ Try image-specific caption first
    const baseName = path.parse(filename).name;
    const captionCandidates = [
      `${filename}.txt`,           // exact match: "1.jpg.txt"
      `${baseName}.txt`            // base name: "1.txt"
    ];

    for (const candidate of captionCandidates) {
      const captionPath = path.join(captionsFolder, candidate);
      try {
        if (fs.existsSync(captionPath)) {
          const content = fs.readFileSync(captionPath, 'utf-8');
          const trimmed = content.trim();
          if (trimmed) {
            console.log(`📄 Using image-specific caption: ${candidate}`);
            return trimmed;
          }
        }
      } catch (err) {
        // File not found or error, try next
      }
    }

    // 2️⃣ STRICT MODE: Use Fallback Captions (fallback-captions.json)
    // We skip seasonal-captions.json and default.txt to ensure only approved captions are used.
    const rotatingCaption = this.getNextFallbackCaption();
    if (rotatingCaption) {
      console.log('🔄 Using rotating fallback caption (Strict Mode)');
      return rotatingCaption;
    }

    // 3️⃣ Generate basic caption as last resort (Review Only)
    const defaultCaption = this.generateDefaultPosterCaption();
    console.log('📄 Using auto-generated caption');
    return defaultCaption;
  }
  
  // 📋 Load caption usage history
  loadCaptionHistory() {
    try {
      if (fs.existsSync(this.captionHistoryFile)) {
        const data = fs.readFileSync(this.captionHistoryFile, 'utf-8');
        return JSON.parse(data);
      }
    } catch (err) {
      console.log('⚠️ Error loading caption history:', err.message);
    }
    return { usedIndices: [], lastResetDate: new Date().toISOString() };
  }

  // 💾 Save caption usage history
  saveCaptionHistory() {
    try {
      fs.writeFileSync(
        this.captionHistoryFile,
        JSON.stringify(this.captionHistory, null, 2),
        'utf-8'
      );
    } catch (err) {
      console.log('⚠️ Error saving caption history:', err.message);
    }
  }

  // 📚 Load fallback captions from JSON
  loadFallbackCaptions() {
    try {
      if (fs.existsSync(this.fallbackCaptionsFile)) {
        const data = fs.readFileSync(this.fallbackCaptionsFile, 'utf-8');
        const json = JSON.parse(data);
        return json.captions || [];
      }
    } catch (err) {
      console.log('⚠️ Error loading fallback captions:', err.message);
    }
    return [];
  }

  // 🌦️ Load seasonal captions from JSON
  loadSeasonalCaptions() {
    try {
      if (fs.existsSync(this.seasonalCaptionsFile)) {
        const data = fs.readFileSync(this.seasonalCaptionsFile, 'utf-8');
        return JSON.parse(data);
      }
    } catch (err) {
      console.log('⚠️ Error loading seasonal captions:', err.message);
    }
    return {};
  }

  // 🌦️ Get seasonal caption based on current date
  getSeasonalCaption() {
    if (!this.seasonalCaptions || Object.keys(this.seasonalCaptions).length === 0) {
      return null;
    }

    const now = new Date();
    const month = now.getMonth() + 1; // 1-12
    const day = now.getDate();

    // Check each seasonal category
    for (const [category, data] of Object.entries(this.seasonalCaptions)) {
      if (!data.date_range || !data.captions || data.captions.length === 0) {
        continue;
      }

      const { start, end } = data.date_range;
      const [startMonth, startDay] = start.split('-').map(Number);
      const [endMonth, endDay] = end.split('-').map(Number);

      let isInRange = false;

      // Handle date ranges (including year wrap-around)
      if (startMonth < endMonth || (startMonth === endMonth && startDay <= endDay)) {
        // Normal range (e.g., 03-15 to 05-20)
        isInRange = (month > startMonth || (month === startMonth && day >= startDay)) &&
                    (month < endMonth || (month === endMonth && day <= endDay));
      } else {
        // Wrap-around range (e.g., 12-15 to 01-15)
        isInRange = (month > startMonth || (month === startMonth && day >= startDay)) ||
                    (month < endMonth || (month === endMonth && day <= endDay));
      }

      if (isInRange) {
        console.log(`🌦️ Seasonal match: ${category} (${start} to ${end})`);
        // Return random caption from this category
        const randomIndex = Math.floor(Math.random() * data.captions.length);
        return data.captions[randomIndex];
      }
    }

    return null; // No seasonal match
  }

  // 🔄 Get next fallback caption (rotating, no repeats)
  getNextFallbackCaption() {
    if (!this.fallbackCaptions || this.fallbackCaptions.length === 0) {
      return null;
    }

    // Reset if all captions have been used
    if (this.captionHistory.usedIndices.length >= this.fallbackCaptions.length) {
      console.log('🔄 All captions used! Resetting rotation...');
      this.captionHistory.usedIndices = [];
      this.captionHistory.lastResetDate = new Date().toISOString();
      this.saveCaptionHistory();
    }

    // Find unused caption indices
    const unusedIndices = [];
    for (let i = 0; i < this.fallbackCaptions.length; i++) {
      if (!this.captionHistory.usedIndices.includes(i)) {
        unusedIndices.push(i);
      }
    }

    if (unusedIndices.length === 0) {
      return null; // Shouldn't happen after reset
    }

    // Pick random unused caption
    const randomIndex = unusedIndices[Math.floor(Math.random() * unusedIndices.length)];
    const caption = this.fallbackCaptions[randomIndex];

    // Mark as used
    this.captionHistory.usedIndices.push(randomIndex);
    this.saveCaptionHistory();

    console.log(`📝 Using fallback caption ${randomIndex + 1}/${this.fallbackCaptions.length} (${this.captionHistory.usedIndices.length} used so far)`);
    return caption;
  }

  // 🎨 Generate default caption for poster
  generateDefaultPosterCaption() {
    const storePhone = process.env.STORE_PHONE || '0771222509';
    const storeLocation = process.env.STORE_LOCATION || 'Thalawathugoda';
    const whatsappLink = process.env.STORE_WHATSAPP_LINK || 'https://wa.me/94771222509';
    
    const captions = [
      `🚗💨 අපේ නවතම දැන්වීම!\n\n📞 ${storePhone}\n📍 ${storeLocation}\n\n💬 WhatsApp: ${whatsappLink}`,
      `🔥 විශේෂ දීමනාවක් සහිතව!\n\n📞 ${storePhone}\n📍 ${storeLocation}\n\n💬 WhatsApp: ${whatsappLink}`,
      `✨ ගුණාත්මක Tyres අඩු මිලකට!\n\n📞 ${storePhone}\n📍 ${storeLocation}\n\n💬 WhatsApp: ${whatsappLink}`,
      `🎉 අද වැඩිම විශ්වාසය දිනාගත් Tyre Shop!\n\n📞 ${storePhone}\n📍 ${storeLocation}\n\n💬 WhatsApp: ${whatsappLink}`
    ];
    
    // Return random caption
    return captions[Math.floor(Math.random() * captions.length)];
  }
  
  // ⚠️ Send warning to admin when no posters available
  async sendNoPosterWarningToAdmin() {
    if (!this.whatsapp || !this.adminWhatsAppNumber) {
      console.log('⚠️  WhatsApp not configured - cannot send admin warning');
      return;
    }

    try {
      let chatId = this.adminWhatsAppNumber.replace(/\D/g, '');
      if (!chatId.startsWith('94')) {
        chatId = '94' + chatId.substring(1);
      }
      chatId += '@c.us';

      const now = new Date().toLocaleString('en-US', { timeZone: 'Asia/Colombo' });
      const message = 
        `⚠️ *No Posters Available for Publishing*\n\n` +
        `📭 The post/ folder is empty - no posters found to publish.\n\n` +
        `🤖 AI post generation is disabled by configuration.\n\n` +
        `📅 Scheduled Time: ${now}\n\n` +
        `📁 Please add poster files to:\n` +
        `   C:\\whatsapp-sql-api\\post\\\n\n` +
        `✅ Supported formats: JPG, PNG, GIF, WEBP, MP4\n\n` +
        `💡 *Action Required:*\n` +
        `   1. Add poster images to post/ folder\n` +
        `   2. Optionally add captions to post/captions/\n` +
        `   3. Next scheduled post will use them automatically\n\n` +
        `📊 *Stats:*\n` +
        `   Total Published: ${this.posterHistory.totalPublished}\n` +
        `   Failed Attempts: ${this.posterHistory.failedAttempts.length}`;
      
      await this.whatsapp.sendMessage(chatId, message);
      console.log('📱 No-poster warning sent to admin WhatsApp');

    } catch (err) {
      console.error('❌ Error sending no-poster warning:', err.message);
    }
  }
  
  // 💾 Save poster publish history
  savePosterHistory() {
    try {
      fs.writeFileSync(this.historyFile, JSON.stringify(this.posterHistory, null, 2));
    } catch (err) {
      console.error('❌ Could not save poster history:', err.message);
    }
  }
  
  // 🎯 Find next unpublished poster (with intelligent cycling)
  findNextPoster() {
    try {
      const files = fs.readdirSync(this.postFolder);
      
      // Get all poster files (alphabetically sorted for consistency)
      const allPosters = files.filter(file => {
        const ext = path.extname(file).toLowerCase();
        const isMedia = [...this.supportedFormats.image, ...this.supportedFormats.video].includes(ext);
        const filePath = path.join(this.postFolder, file);
        const isFile = fs.statSync(filePath).isFile();
        return isMedia && isFile;
      }).sort(); // Alphabetical order: 1, 2, 3, etc.

      if (allPosters.length === 0) {
        console.log('📭 No poster files found in post/ folder');
        return null;
      }

      // Get files that have been published
      const publishedFiles = this.posterHistory.published.map(p => p.file);

      // Find unpublished posters first
      const unpublishedPosters = allPosters.filter(file => !publishedFiles.includes(file));

      if (unpublishedPosters.length > 0) {
        console.log(`📂 Found ${unpublishedPosters.length} unpublished poster(s)`);
        console.log(`🎯 Selecting: ${unpublishedPosters[0]}`);
        return unpublishedPosters[0];
      }

      // 🔄 ALL POSTERS PUBLISHED -> RETURN NULL TO TRIGGER BACKGROUND GEN
      console.log('🏁 All posters have been published. Switching to Text-Only/Background mode.');
      return null;

    } catch (err) {
      console.error('❌ Error finding next poster:', err.message);
      return null;
    }
  }
  
  start() {
    // Read schedule from jobs-config.json
    const jobConfig = ConfigService.getJobConfig('DailyFacebookPostJob') || {};
    const schedule = jobConfig.schedule || '35 10 */2 * *'; // Default: Every 2 days at 10:35 AM
    
    const dailyPost = cron.schedule(schedule, () => this.generateAndPublishPost(), { 
      timezone: 'Asia/Colombo',
      recoverMissedExecutions: true
    });
    this.tasks.push(dailyPost);
    console.log(`✅ Facebook post job started (${schedule})`);
    console.log(`📁 Poster folder: ${this.postFolder}`);
    console.log(`🔄 Cycling enabled - posters repeat after all published`);
  }
  
  stop() { this.tasks.forEach(task => task.stop()); }
  
  // 🎨 Generate attractive background image with caption
  async generateCaptionImage(captionText) {
    try {
      const width = 1080;
      const height = 1080;
      const canvas = createCanvas(width, height);
      const ctx = canvas.getContext('2d');

      // 1. Background (Gradient)
      const colors = [
        ['#1a2a6c', '#b21f1f', '#fdbb2d'], // Deep Blue to Red to Orange
        ['#000000', '#434343'],            // Black to Dark Grey
        ['#000046', '#1CB5E0'],            // Deep Blue to Light Blue
        ['#0f0c29', '#302b63', '#24243e'], // Dark Purple/Blue
        ['#8E2DE2', '#4A00E0'],            // Purple to Blue
        ['#C31432', '#240b36']             // Red to Dark
      ];
      const palette = colors[Math.floor(Math.random() * colors.length)];
      
      const gradient = ctx.createLinearGradient(0, 0, width, height);
      palette.forEach((color, index) => {
        gradient.addColorStop(index / (palette.length - 1), color);
      });
      
      ctx.fillStyle = gradient;
      ctx.fillRect(0, 0, width, height);

      // 2. Texture
      ctx.fillStyle = 'rgba(255, 255, 255, 0.03)';
      for (let i = 0; i < 10; i++) {
        ctx.beginPath();
        ctx.arc(Math.random() * width, Math.random() * height, Math.random() * 400, 0, Math.PI * 2);
        ctx.fill();
      }

      // 3. Text Configuration
      ctx.fillStyle = '#FFFFFF';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const fontSize = 55; 
      // Try to use a system font that supports Sinhala/English
      ctx.font = `bold ${fontSize}px "Segoe UI", "Arial", "Iskoola Pota", "Nirmala UI", sans-serif`;

      // 4. Wrap & Draw Text
      const words = captionText.split(/\s+/);
      let line = '';
      const lines = [];
      const maxWidth = width - 160; 
      
      for (const word of words) {
        const testLine = line + word + ' ';
        const metrics = ctx.measureText(testLine);
        if (metrics.width > maxWidth && line.length > 0) {
          lines.push(line);
          line = word + ' ';
        } else {
          line = testLine;
        }
      }
      lines.push(line);

      const lineHeight = fontSize * 1.5;
      const totalHeight = lines.length * lineHeight;
      let startY = (height - totalHeight) / 2;

      // Draw shadow
      ctx.shadowColor = "rgba(0,0,0,0.5)";
      ctx.shadowBlur = 10;
      ctx.shadowOffsetX = 3;
      ctx.shadowOffsetY = 3;

      lines.forEach((l, i) => {
        ctx.fillText(l.trim(), width / 2, startY + (i * lineHeight));
      });
      
      // Reset shadow
      ctx.shadowColor = "transparent";

      // 5. Branding Footer
      ctx.font = '24px "Segoe UI", "Arial", sans-serif';
      ctx.fillStyle = 'rgba(255, 255, 255, 0.6)';
      ctx.fillText('Lasantha Tyre Service • Thalawathugoda • 077 122 2509', width / 2, height - 60);

      const fileName = `bg_gen_${Date.now()}.png`;
      const outputPath = path.join(this.postFolder, fileName);
      const buffer = canvas.toBuffer('image/png');
      fs.writeFileSync(outputPath, buffer);
      
      return outputPath;
    } catch (err) {
      console.error('❌ Error generating caption image:', err);
      return null;
    }
  }

  async generateAndPublishPost() {
    let post;
    let result;
    let error;
    let usingPoster = false;
    let posterFile = null;

    try {
      // 🔄 STEP 1: Check if there are posters in post/ folder
      posterFile = this.findNextPoster();
      
      if (posterFile) {
        // Use poster from post/ folder
        usingPoster = true;
        const posterPath = path.join(this.postFolder, posterFile);
        console.log('\n' + '='.repeat(60));
        console.log(`🎨 Using poster from post/ folder: ${posterFile}`);
        console.log('='.repeat(60) + '\n');
        
        // Get caption for poster (from captions folder or generate)
        const captionText = await this.getCaptionForPoster(posterFile);
        
        post = {
          content: captionText,
          imagePath: posterPath,
          isPoster: true,
          posterFile: posterFile
        };
      } else {
        // 🔄 No posters available? -> Switch to Attractive Background Mode
        console.log('\n' + '='.repeat(60));
        console.log('📭 No posters in post/ folder. Switching to Generated Background Mode...');
        console.log('='.repeat(60) + '\n');
        
        // 1. Get Caption STRICTLY from fallback file
        const captionText = this.getNextFallbackCaption();
        
        if (!captionText) {
          console.error('❌ No fallback captions available either! Cannot publish.');
          return;
        }

        // 2. Generate Image
        const genImagePath = await this.generateCaptionImage(captionText);
        
        if (!genImagePath) {
           console.error('❌ Failed to generate background image.');
           return;
        }

        console.log(`🎨 Generated attractive background image: ${genImagePath}`);

        post = {
          content: captionText,
          imagePath: genImagePath,
          isPoster: false, 
          posterFile: null
        };
      }

      // 2. Post directly to Facebook
      const localOnly = ConfigService.getBoolean('LOCAL_PREVIEW_ONLY', false);
      if (localOnly) {
        console.log('📝 LOCAL_PREVIEW_ONLY=true -> Skipping live post. Sending local preview to admin.');
        result = { id: 'local-preview-only', post_id: 'local-preview-only' };
      } else {
        console.log('🚀 Publishing post directly to Facebook...');
        result = await this.postToFacebook(post.content, post.imagePath);
        console.log(`✅ Post successful. ID: ${result.id || 'N/A'}`);
        
        // 🔄 If poster was used, move to processed and update history
        if (usingPoster && posterFile) {
          this.posterHistory.published.push({
            file: posterFile,
            caption: post.content,
            publishDate: new Date().toISOString(),
            facebookId: result.id || result.post_id,
            status: 'published'
          });
          this.posterHistory.lastPublish = new Date().toISOString();
          this.posterHistory.totalPublished++;
          this.savePosterHistory();
          
          // Move poster to processed folder
          const processedPath = path.join(this.processedFolder, posterFile);
          if (fs.existsSync(post.imagePath)) {
            fs.renameSync(post.imagePath, processedPath);
            console.log(`📦 Moved poster to processed: ${processedPath}`);
          }
        }
      }
    } catch (e) {
      error = e;
      console.error('❌ CRITICAL: Failed to generate or publish post:', e.message);
      
      // Record failed poster attempt
      if (usingPoster && posterFile) {
        this.posterHistory.failedAttempts.push({
          file: posterFile,
          error: e.message,
          date: new Date().toISOString()
        });
        this.savePosterHistory();
      }
    }

    // 3. Send status update and a copy of the post to the admin
    try {
      if (error) {
        await this.sendPublicationReportToAdmin(post, { status: 'error', error });
      } else {
        await this.sendPublicationReportToAdmin(post, { status: 'success', result, usingPoster });
      }
    } catch (adminErr) {
      console.error('❌ Failed to send admin WhatsApp update:', adminErr.message);
    }

    // 4. Clean up the local image file (ONLY for AI-generated images, NOT posters)
    if (post && post.imagePath && fs.existsSync(post.imagePath) && !usingPoster) {
      try {
        fs.unlinkSync(post.imagePath);
        console.log(`🗑️ Cleaned up AI-generated image: ${post.imagePath}`);
      } catch (cleanupErr) {
        console.warn(`⚠️  Could not delete image file ${post.imagePath}:`, cleanupErr.message);
      }
    }
  }
  
  async generateAndSendForApproval() {
    let post;
    try {
      post = await this.generatePost();
      const localOnly = ConfigService.getBoolean('LOCAL_PREVIEW_ONLY', false);
      if (localOnly) {
        console.log('📝 LOCAL_PREVIEW_ONLY=true -> Skipping live post (preview only).');
        await this.sendPublicationReportToAdmin(post, { status: 'local_preview' });
        return;
      }

      console.log('🚀 Auto-publishing to Facebook...');
      let result;
      if (ConfigService.getBoolean('PUBLISH_VIA_QUEUE', true)) {
        try {
          const { PostPublisherQueue } = require('../utils/PostPublisherQueue');
          const q = PostPublisherQueue.getInstance();
          result = await q.enqueue({ 
            caption: post.content, 
            imagePath: post.imagePath, 
            publishMode: ConfigService.get('FB_PUBLISH_MODE', this.publishMode) 
          });
        } catch (e) {
          console.warn('⚠️  Queue publish failed, falling back to direct:', e.message);
          result = await this.postToFacebook(post.content, post.imagePath);
        }
      } else {
        result = await this.postToFacebook(post.content, post.imagePath);
      }
      
      console.log(`✅ Post successful: ${result.id || 'ok'}`);
      await this.sendPublicationReportToAdmin(post, { status: 'success', result });

    } catch (err) {
      console.error('❌ Error in generateAndSendForApproval:', err.message);
      await this.sendPublicationReportToAdmin(post, { status: 'error', error: err });
    } finally {
      // Clean up local file after posting to avoid clutter
      if (post && post.imagePath && fs.existsSync(post.imagePath)) {
        try { 
          fs.unlinkSync(post.imagePath);
          console.log(`🗑️  Cleaned up temporary image: ${post.imagePath}`);
        } catch (e) {
          console.warn(`⚠️ Could not delete temp image: ${e.message}`);
        }
      }
    }
  }

  async generatePost() {
    // 1) Load utilities
  const { getProductCatalog } = require('../utils/ProductCatalog');
    const { getBrandKnowledge } = require('../utils/BrandKnowledge');
  const { findPatternSpec } = require('../utils/PatternSpecs');
    const { getInStockBrandPatterns } = require('../utils/InStockSelector');
    const { getPostHistory } = require('../utils/PostHistory');
    const { getContentStrategy } = require('../utils/ContentStrategies');
    const { analyzeExistingPosts } = require('../scripts/analyzeFacebookPosts');
    
    const catalog = getProductCatalog();
    const brandKnowledge = getBrandKnowledge();
    const history = getPostHistory();
    
    // 2) Get next content type for variety (balances usage)
    const contentType = await history.getNextContentType();
    console.log(`🎯 Content strategy: ${contentType}`);
    
    // 3) Get creative strategy for this post
    const strategy = getContentStrategy(contentType);
    
  // 4) Try to get unique product (avoid repetition)
  let product;
  let lockedBrand = null; // freeze the brand selected so later steps cannot change it
    let attempts = 0;
    const maxAttempts = 10;
    
    // 4a) DB-first: prefer an in-stock brand/pattern from SQL (exclude motorcycle)
    // ENHANCED: Restrict to allowed brands and prioritize within that set
    // Use brand's any tyre image if specific pattern image not found
    try {
      // Load allowed brands from brand-list.json (single source of truth)
      let allowedBrands = ['MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO'];
      try {
        const brandListPath = path.join(__dirname, '..', 'assets', 'poster-elements', 'brand-list.json');
        if (fs.existsSync(brandListPath)) {
          const bl = JSON.parse(fs.readFileSync(brandListPath, 'utf-8'));
          if (Array.isArray(bl.brands) && bl.brands.length) {
            allowedBrands = bl.brands.map(b => String(b).toUpperCase());
          }
        }
      } catch {}

      const dbPreferred = await getInStockBrandPatterns({ limit: 50, excludeMotorcycle: true, minQty: 1 });
      // Filter to only allowed brands
      const filtered = Array.isArray(dbPreferred) ? dbPreferred.filter(r => allowedBrands.includes(String(r.brand || '').toUpperCase())) : [];
      
      // Priority order (can reflect business preference within allowed)
      const priorityBrands = ['MAXXIS','DURATURN','GITI','MARSHAL','GT','YOKOHAMA','KUMHO'];
      
      // Sort products: priority brands first, then by stock quantity
      const sortedProducts = filtered.sort((a, b) => {
        const aIsPriority = priorityBrands.includes(a.brand.toUpperCase());
        const bIsPriority = priorityBrands.includes(b.brand.toUpperCase());
        
        if (aIsPriority && !bIsPriority) return -1;
        if (!aIsPriority && bIsPriority) return 1;
        
        // Both priority or both non-priority, sort by quantity
        return b.totalQty - a.totalQty;
      });
      
  if (Array.isArray(sortedProducts) && sortedProducts.length) {
        // Helper function to check if tyre image exists
        // Returns: { found: boolean, hasExactMatch: boolean, path: string|null }
        const checkTyreImage = (brand, pattern) => {
          const fs = require('fs');
          const root = path.join(__dirname, '..', 'tyre-images');
          if (!fs.existsSync(root)) return { found: false, hasExactMatch: false, path: null };
          const brandDir = path.join(root, String(brand || '').trim());
          if (!fs.existsSync(brandDir)) return { found: false, hasExactMatch: false, path: null };
          const exts = ['.jpg', '.jpeg', '.png', '.webp', '.gif'];
          const norm = (s) => String(s || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
          const patternN = norm(pattern);
          const entries = fs.readdirSync(brandDir, { withFileTypes: true });
          let files = [];
          for (const ent of entries) {
            const full = path.join(brandDir, ent.name);
            if (ent.isDirectory()) {
              try {
                const sub = fs.readdirSync(full, { withFileTypes: true });
                for (const f of sub) {
                  const ffull = path.join(full, f.name);
                  if (f.isFile() && exts.includes(path.extname(ffull).toLowerCase())) files.push(ffull);
                }
              } catch {}
            } else if (exts.includes(path.extname(full).toLowerCase())) {
              files.push(full);
            }
          }
          if (!files.length) return { found: false, hasExactMatch: false, path: null };
          const exact = files.find(fp => norm(path.basename(fp)).includes(patternN));
          if (exact) return { found: true, hasExactMatch: true, path: exact };
          return { found: false, hasExactMatch: false, path: null };
        };
        
        // pick the first one that has an exact image and is not too similar to recent posts
        for (const row of sortedProducts) {
          const patternName = row.pattern;
          
          // Check if image exists for this brand/pattern
          const imageCheck = checkTyreImage(row.brand, patternName);
          if (!imageCheck.found || !imageCheck.hasExactMatch) {
            console.log(`⏭️  Skipping ${row.brand} ${patternName} - no matching image`);
            continue; // Skip this product, try next one
          }
          
          console.log(`✅ Image found for ${row.brand} ${patternName} (exact pattern match)`);
          
          const candidate = {
            brand: row.brand,
            name: patternName,
            size: null,
            description: null,
            regularPrice: null,
            salePrice: null,
            inStock: true,
            additionalProducts: [],
            contentType
          };
          const isSimilar = await history.isTooSimilar({
            brand: candidate.brand,
            size: candidate.size,
            content: `${candidate.brand} ${candidate.name}`
          });
          if (!isSimilar) {
            product = candidate;
            lockedBrand = String(candidate.brand || '').toUpperCase();
            const isPriority = priorityBrands.includes(row.brand.toUpperCase()) ? '⭐ PRIORITY' : '';
            console.log(`📦 DB-first pick: ${candidate.brand} ${candidate.name} (qty>=${row.totalQty}) ${isPriority}`);
            break;
          } else {
            console.log(`⏭️  Skipping ${row.brand} ${patternName} - too similar to recent posts`);
          }
        }
        // if all were similar, fallback to first one WITH EXACT IMAGE (prefer priority brands)
        if (!product) {
          for (const row of sortedProducts) {
            const imageCheck = checkTyreImage(row.brand, row.pattern);
            if (imageCheck.found && imageCheck.hasExactMatch) {
              product = {
                brand: row.brand,
                name: row.pattern,
                size: null,
                description: null,
                regularPrice: null,
                salePrice: null,
                inStock: true,
                additionalProducts: [],
                contentType
              };
              lockedBrand = String(row.brand || '').toUpperCase();
              const isPriority = priorityBrands.includes(lockedBrand) ? '⭐ PRIORITY' : '';
              console.log(`📦 DB-first (fallback with image): ${product.brand} ${product.name} ${isPriority}`);
              break;
            }
          }
        }
      }
    } catch (e) {
      console.warn('⚠️  DB-first selection skipped:', e.message);
    }

    // If we still don't have a product, try catalog; otherwise skip to enrichment
    if (!product) {
      if (catalog.isAvailable()) {
        // Try to find non-repetitive product
        while (attempts < maxAttempts) {
  const catalogData = catalog.getCreativePostData(contentType);
  // Prefer items with stock > 0 or inStock true
  // Restrict catalog pool to allowed brands as well
  let allowedBrands2 = ['MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO'];
  try {
    const brandListPath = path.join(__dirname, '..', 'assets', 'poster-elements', 'brand-list.json');
    if (fs.existsSync(brandListPath)) {
      const bl = JSON.parse(fs.readFileSync(brandListPath, 'utf-8'));
      if (Array.isArray(bl.brands) && bl.brands.length) {
        allowedBrands2 = bl.brands.map(b => String(b).toUpperCase());
      }
    }
  } catch {}
  const poolAll = Array.isArray(catalogData.products) ? catalogData.products : [];
  const pool = poolAll.filter(p => allowedBrands2.includes(String(p?.brand || '').toUpperCase()));
  const mainProduct = pool.find(p => (p?.inStock === true) || (Number(p?.stockQty || p?.qty || 0) > 0)) || pool[0];
        
        const patternName = mainProduct.pattern || mainProduct.model || mainProduct.displayName;
        const tempProduct = {
          brand: mainProduct.brand,
          name: patternName,
          size: mainProduct.size,
          description: mainProduct.description,
          regularPrice: null,
          salePrice: null,
          inStock: null,
          additionalProducts: catalogData.products.slice(1),
          contentType: contentType
        };
        
        // Check if too similar to recent posts
        const isSimilar = await history.isTooSimilar({
          brand: tempProduct.brand,
          size: tempProduct.size,
          content: `${tempProduct.brand} ${tempProduct.size} ${tempProduct.description}`
        });
        
        if (!isSimilar) {
    product = tempProduct;
    lockedBrand = String(tempProduct.brand || '').toUpperCase();
          break;
        }
        
        attempts++;
        }
        
        // If all attempts failed, use last one anyway
        if (!product) {
          console.warn('⚠️  Could not find unique product after 10 attempts, proceeding anyway');
          const catalogData = catalog.getCreativePostData(contentType);
          const pool = Array.isArray(catalogData.products) ? catalogData.products : [];
          const mainProduct = pool[0] || {};
          const patternName = mainProduct.pattern || mainProduct.model || mainProduct.displayName;
          product = {
            brand: mainProduct.brand,
            name: patternName,
            size: mainProduct.size,
            description: mainProduct.description,
            regularPrice: null,
            salePrice: null,
            inStock: null,
            additionalProducts: pool.slice(1),
            contentType: contentType
          };
        }
        lockedBrand = String(product.brand || '').toUpperCase();
        
        // Enhance with brand knowledge
        if (brandKnowledge.isAvailable()) {
          const brandInfo = brandKnowledge.getBrandInfo(product.brand);
          if (brandInfo) {
            product.brandKnowledge = {
              origin: brandInfo.origin,
              tier: brandInfo.tier,
              tagline: brandInfo.tagline,
              description: brandInfo.description,
              features: brandInfo.features.slice(0, 4),
              strengths: brandInfo.strengths,
              popularModels: brandInfo.popularModels.slice(0, 3),
              idealFor: brandInfo.idealFor
            };
            console.log(`📦 Product: ${product.brand} ${product.size} [${brandInfo.tier} - ${brandInfo.origin}] (${attempts} attempts)`);
          } else {
            console.log(`📦 Product: ${product.brand} ${product.size} (${attempts} attempts)`);
          }
        } else {
          console.log(`📦 Product: ${product.brand} ${product.size} (${attempts} attempts)`);
        }

        // Attach pattern-level specs from local DB if available
        const spec = findPatternSpec(product.brand, product.name);
        if (spec) {
          product.patternSpec = spec;
        }
      } else {
        // Fallback to dummy data (only if we still don't have a product)
        console.warn('⚠️  Product catalog not available. Run: node scripts/extractProductCatalog.js');
        const ContentGenerator = require('../utils/ContentGenerator');
        const cg = new ContentGenerator();
        product = await cg.getRandomProduct();
        product.contentType = contentType;
        lockedBrand = String(product.brand || '').toUpperCase();
      }
    } else {
      // We already have a product from DB-first; optionally enrich with brand knowledge
      if (brandKnowledge.isAvailable()) {
        const brandInfo = brandKnowledge.getBrandInfo(product.brand);
        if (brandInfo) {
          product.brandKnowledge = {
            origin: brandInfo.origin,
            tier: brandInfo.tier,
            tagline: brandInfo.tagline,
            description: brandInfo.description,
            features: brandInfo.features.slice(0, 4),
            strengths: brandInfo.strengths,
            popularModels: brandInfo.popularModels.slice(0, 3),
            idealFor: brandInfo.idealFor
          };
        }
      }
      lockedBrand = String(product.brand || '').toUpperCase();
      // Attach pattern-level specs from local DB if available
      const spec = findPatternSpec(product.brand, product.name);
      if (spec) product.patternSpec = spec;
    }

  // 2) Optionally auto re-analyze post style if new images were added
    try {
      const autoStyle = String(process.env.AUTO_REANALYZE_POST_STYLE || '').toLowerCase() === 'true';
      if (autoStyle) {
        const stylePath = path.join(__dirname, '../facebook-post-style-guide.json');
        const postDir = path.join(__dirname, '../post');
        let needAnalyze = false;
        let styleMTime = 0;
        if (fs.existsSync(stylePath)) {
          styleMTime = fs.statSync(stylePath).mtimeMs;
        } else {
          needAnalyze = true;
        }
        if (fs.existsSync(postDir)) {
          const files = fs.readdirSync(postDir).filter(f => /\.(jpg|jpeg|png)$/i.test(f));
          if (files.length) {
            const latest = files
              .map(f => fs.statSync(path.join(postDir, f)).mtimeMs)
              .reduce((a,b) => Math.max(a,b), 0);
            if (latest > styleMTime) needAnalyze = true;
          }
        }
        if (needAnalyze) {
          console.log('🔁 Auto re-analyzing Facebook post style...');
          await analyzeExistingPosts();
        }
      }
    } catch (e) {
      console.warn('⚠️  Auto style analysis skipped:', e.message);
    }

    // 3) Load ANALYZED post style guide from existing Facebook posts
    const postStyleGuide = loadPostStyleGuide();
    // Fallback to old style guide if analysis not available
    const styleGuide = postStyleGuide || loadStyleGuide();

    // 4) Prepare base template/content as fallback
    const storePhone = (process.env.STORE_PHONE || '0771222509');
    const whatsappLink = `https://wa.me/94${storePhone.substring(1)}`;

    const store = {
      name: (require('../utils/ConfigService').ConfigService.get('STORE_NAME', process.env.STORE_NAME || 'Lasantha Tyre Traders')),
      phone: storePhone,
      whatsappLink: whatsappLink,
      location: process.env.STORE_LOCATION || 'Thalawathugoda',
      hours: process.env.STORE_HOURS || '06:30-21:00',
      alignHours: process.env.STORE_WHEEL_ALIGNMENT_HOURS || '07:30-18:00',
    };
    const baseTemplate = { type: 'promo', theme: 'blue', content: `${product.brand || ''} ${product.name} ${product.size}\n\nCall: ${store.phone}` };
    let content = baseTemplate.content;
    let providerUsed = 'fallback';

  // 5) Choose a copy template and optional trend bullets
    const templates = getTemplates();
    const template = templates[Math.floor(Math.random() * templates.length)];
    let trends = [];
    try { trends = await fetchTrends(); } catch {}

  // 6) Build prompt - V5 (Production-Level Creative AI Caption Generation)
  // ULTRA-CREATIVE GENERAL MESSAGING - Focus on business reputation & customer psychology
  
  // Check if AI is disabled via dashboard config or environment variable
  // Priority: Dashboard config > Environment variable
  const jobConfig = ConfigService.getJobConfig('DailyFacebookPostJob') || {};
  const jobSettings = jobConfig.settings || {};
  
  // Check multiple sources for AI disable flag (dashboard config takes priority)
  let aiDisabled = false;
  if (jobSettings.useFallbackOnly === true) {
    aiDisabled = true;
    console.log('🎯 [Dashboard Config] AI disabled - using fallback captions only');
  } else if (jobSettings.useAI === false) {
    aiDisabled = true;
    console.log('🎯 [Dashboard Config] AI disabled via useAI=false - using fallback captions only');
  } else if ((ConfigService.get('FB_POST_DISABLE_AI') || process.env.FB_POST_DISABLE_AI || 'false').toLowerCase() === 'true') {
    aiDisabled = true;
    console.log('🎯 [Environment Variable] AI disabled - using fallback captions only');
  }
  
  if (aiDisabled) {
    console.log('⏭️  Skipping all AI providers - direct fallback caption mode');
    // Skip AI generation, go straight to fallback captions
    providerUsed = 'fallback-captions-only';
  }
  
  const prompt = `
You are a WORLD-CLASS marketing copywriter and creative storyteller specializing in automotive products and Sri Lankan market psychology. Your captions have generated millions in revenue through emotional connection and persuasive storytelling.

Create a UNIQUE, CREATIVE, and IRRESISTIBLY ATTRACTIVE Facebook post for ${store.name} - the most trusted premium tyre dealer in ${store.location}.

**🎯 MASTER MARKETING STRATEGY: PSYCHOLOGICAL CUSTOMER ATTRACTION**

Your mission: Create content so compelling that customers feel they MUST visit us. Build deep emotional connection, trust, and desire. Every word should trigger positive emotions and buying intent.

**CORE PRINCIPLES:**
1. **Emotional Resonance** - Touch hearts before minds
2. **Trust Building** - Establish credibility and reliability
3. **Value Perception** - Show worth beyond price
4. **FOMO Creation** - Gentle urgency without pressure
5. **Brand Loyalty** - Make them remember us forever

**🎨 CREATIVE STRUCTURE:**

1. **ATTENTION-GRABBING HOOK (2-4 lines):**
   - Start with a POWERFUL, CREATIVE opening that STOPS THE SCROLL
   - Use RANDOM creative angles (MUST ROTATE - be unpredictable):
     
     **Storytelling Hooks:**
     * "සිංහල නව අවුරුද්දට ගමට යද්දි අපේ වෑන් එකේ ටයර් පුපුරලා අනතුරක් වෙන්න හිටියා... ඒ දවස මට තේරුණා හොඳ ටයර් එකක වටිනාකම 😰🙏"
     * "පොඩි දුව පාසලට යන අතරමග road එකේ... හදිස්සිම brake එකක්... ඔන්න එහෙමයි අපිට තේරෙන්නේ ටයර් එකක grip එක කොච්චර වැදගත්ද කියලා 🚗💨"
     
     **Emotional Safety Appeals:**
     * "ඔයාගේ පවුලේ ආරක්ෂාව... එක ටයර් එකකින් තීරණය වෙනවා දැන සිටියාද? 🚗💙"
     * "හැම උදේම school van එකේ යන ඔයාගේ දරුවෝ... ඔවුන්ගේ ආරක්ෂාවම රඳා පවතින්නේ හොඳ ටයර් මත! ‍‍👧‍👦✨"
     
     **Question Hooks (Curiosity):**
     * "ඔයාගේ වාහනයට හොඳම ටයර් සොයනවද? හරියට තෝරාගන්න ක්‍රමය දන්නවද? 🤔"
     * "මාසෙකට දෙපාරක් petrol station එකට යනවද? හොඳ ටයර් එකකින් fuel save කරන්න පුළුවන් දැනගෙන හිටියාද? ⛽"
     
     **Value Investment Hooks:**
     * "සතියකට එක පාරක් හෝටලේ කන එක Rs.3000. හැබැයි අවුරුදු 4ක් ආරක්ෂිතව ගමන් කරන්න හොඳ ටයර් set එකක් ගන්න හිතන්නේ නැහැ! 💭💡"
     * "Phone එකට cover එකක් දාගන්නවා. හැබැයි ජීවිතේ carry කරන වාහනයට හොඳම ටයර් ගන්න අමතක වෙනවා! 🤔💪"
     
     **Trust & Experience:**
     * "1997 සිට මේ business එකේ... අපි දැකලා තියෙනවා ඔය තීරණ ගැන පසුතැවෙන customers ලාත්, සතුටින් ඉන්න customers ලාත් 🏆"
     * "වසර 25ක් වෙනවා අපි මේ industry එකේ... දන්නවා කුමන තීරණ customers වගේ ඔබට life-changing වෙයිද කියලා 📚✨"
     
     **Problem-Solution:**
     * "හැම වරක්ම wheel alignment වෙන්න යන්න ලේසි නෑ... හැබැයි හොඳ ටයර් set එකකින් ඒ පොරක්කුව අඩු කරන්න පුළුවන්! 🔧"
     * "Tyre එක කැඩෙන එක හදිසි අනතුරක් නෙමෙයි... එය පුරෝකථනය කළ හැකි අනතුරක්! නිවැරදි තෝරා ගැනීමෙන් වැළකෙන්න පුළුවන් "
   
   - Use NATURAL Sinhala - like talking to a friend at a kopi kade
   - Create IMMEDIATE EMOTIONAL CONNECTION
   - Make them FEEL before they THINK

2. **BUSINESS VALUE & TRUST BUILDING (5-8 lines):**
   - Focus on OUR BUSINESS EXCELLENCE & CUSTOMER CARE (NOT specific brands):
   
   **Trust & Reputation:**
   * "Lasantha Tyre Traders - ${store.location} හි විශ්වාසදායීම නම. වසර ගණනාවක් දහස් ගණන් පවුල් අපව තෝරාගෙන තියෙනවා ✅"
   * "අපි තනිකරම ටයර් විකුණන්නේ නෙමෙයි... ඔයාගේ පවුලේ ආරක්ෂාව protect කරන්න උපකාර කරනවා 🛡️"
   * "සෑම customer කෙනෙක්ම අපගේ පවුලේ අයෙක් - ඒ නිසා අපි කවදාවත් compromise කරන්නේ නැහැ 💙"
   
   **Quality Assurance:**
   * "100% Original Products Only - අපි guarantee කරනවා හැම ටයර් එකක්ම genuine බව ✨"
   * "ගුණාත්මක තත්ත්වය අපගේ reputation එක... මිල අපගේ competitive edge එක 🏆"
   * "ලෝක ප්‍රමිතියේ ටයර් Sri Lankan prices වලට - විශේෂඥ උපදෙස් නොමිලේ 🌟"
   
   **Expert Service:**
   * "පළපුරුදු තාක්ෂණික විශේෂඥයන් ඔයාගේ vehicle type, driving style, budget එකට perfect ටයර් තෝරා දෙනවා 👨‍🔧"
   * "හැම customer කෙනෙකුටම personalized service - කාටවත් වගේ treat කරන්නේ නැහැ, ඔයා special! ⭐"
   * "නොමිලේ technical consultation - load index, speed rating, tread pattern හැමදේම explain කරනවා 📚"
   
   **Free Value-Added Services:**
   * "✨ නොමිලේ N2 Nitrogen Filling - ටයර් life එක වැඩි කරන්න"
   * "🔧 නොමිලේ Tyre Fixing Service - කුඩා punctures වහාම fix කරනවා"
   * "🎯 නොමිලේ Tyre Pressure Check & Expert Advice"
   
   **Long-term Partnership:**
   * "එක පාරටම හොඳ තීරණයක් ගන්න - අවුරුදු ගාණක් ආරක්ෂිතව, පහසුවෙන් ගමන් කරන්න 🛣️"
   * "අපගේ after-sales support එක lifetime - ඔයා මිලදී ගත්තට පස්සේත් අපි ඔයා එක්ක ✅"
   
   - Use 5-8 relevant emojis strategically (🚗 ✨ 💪 🛣️ ✅ 📞 🏆 ⭐)
   - Short, punchy sentences - easy to read on mobile
   - Add personality and warmth - avoid corporate/stiff language

3. **CALL TO ACTION (Must include this block):**
   - Use this EXACT format.
   - "වැඩි විස්තර සහ නොමිලේ උපදෙස් සඳහා දැන්ම අපට WhatsApp කරන්න!"
   - "👉 ${store.whatsappLink}"
   - "📞 Call: ${store.phone}"
   - "📍 Visit Us: ${store.location}"

**🏷️ HASHTAGS (Choose 5-7 relevant tags from these categories):**

*   **Core Business:** #LasanthaTyreTraders #TyresSriLanka #${store.location.replace(/\s+/g,'')} #PremiumTyres #TrustedQuality #QualityTyres #SafetyFirst
*   **Services:** #WheelAlignment #NitrogenFilling #TyreFixing #TyreShop #AutoCareSL
*   **Brands (use only the one you are promoting):** #Maxxis #Duraturn #Yokohama #Giti #Marshal #GT #Kumho
*   **Vehicle Types:** #CarTyres #VanTyres #SUVTyres #LorryTyres #SriLankaCars
*   **Concepts:** #RoadSafety #FamilySafety #TireCare #FuelEfficiency #BestDealsSL

**🎯 SUCCESS CRITERIA:**
Your post should make the reader think: "This is the TRUSTED tyre business I want to work with - I must message them NOW on WhatsApp to experience their excellent service and get expert advice for my vehicle!"

**🎨 CREATIVITY MANDATE:**
- Every caption must be UNIQUE and DIFFERENT from previous ones
- Rotate creative angles: safety focus → value investment → expert service → customer trust → quality assurance → family protection
- Use varied emotional triggers each time
- Be sensitive to customer needs and market psychology
- Show genuine care for customer wellbeing, not just selling

Now create the post:
`;


    // 7) AI Generation - INTELLIGENT FALLBACK CHAIN WITH DISABLE OPTION
    // Priority: Gemini (fast) → Claude (reliable) → Fallback Captions (always works)
    // Can be completely disabled: FB_POST_DISABLE_AI=true
    
    let geminiAttempted = false;
    let claudeAttempted = false;
    
    // ========================================
    // CHECK IF AI IS DISABLED
    // ========================================
    if (aiDisabled) {
      console.log('⏭️  AI caption generation disabled - using premium fallback captions');
      // Skip all AI attempts, go directly to fallback captions
      content = null; // Force fallback caption selection
    }
    
    // ========================================
    // STEP 1: TRY GEMINI 2.5 FLASH FIRST (FASTEST)
    // ========================================
    const geminiKey = ConfigService.get('GEMINI_API_KEY') || process.env.GEMINI_API_KEY;
    if (!aiDisabled && geminiKey) {
      console.log('🚀 [1/3] Trying Gemini 2.5 Flash (fastest option)...');
      geminiAttempted = true;
      
      try {
        if (!this.genAI) {
          const { GoogleGenerativeAI } = require('@google/generative-ai');
          this.genAI = new GoogleGenerativeAI(geminiKey);
        }
        const model = this.genAI.getGenerativeModel({ 
          model: 'gemini-2.5-flash',
          generationConfig: {
            temperature: 0.85, // High creativity for attractive captions
            topP: 0.95, // More diverse vocabulary
            topK: 40, // Better word selection
            maxOutputTokens: 2048, // Optimized for quality captions
            candidateCount: 1
          }
        });
        
        const result = await model.generateContent(prompt);
        
        // Extract text from Gemini response
        let text = '';
        if (result.response && result.response.text) {
          if (typeof result.response.text === 'function') {
            text = result.response.text().trim();
          } else {
            text = result.response.text.trim();
          }
        } else if (result.response && result.response.candidates) {
          const firstCandidate = result.response.candidates[0];
          if (firstCandidate && firstCandidate.content && firstCandidate.content.parts) {
            text = firstCandidate.content.parts.map(part => part.text || '').join('').trim();
          }
        }
        
        if (text && text.length > 100) {
          content = text;
          providerUsed = 'gemini-2.5-flash';
          console.log('✅ Gemini SUCCESS! Post generated (' + text.length + ' chars)');
        } else {
          throw new Error(`Gemini response too short: ${text.length} chars`);
        }
      } catch (geminiError) {
        // Detailed error handling for different Gemini failure scenarios
        const errorMsg = geminiError.message || String(geminiError);
        
        if (errorMsg.includes('429') || errorMsg.includes('quota') || errorMsg.includes('RESOURCE_EXHAUSTED')) {
          console.warn('⚠️  Gemini quota exceeded or rate limited');
        } else if (errorMsg.includes('503') || errorMsg.includes('unavailable')) {
          console.warn('⚠️  Gemini service temporarily unavailable');
        } else if (errorMsg.includes('401') || errorMsg.includes('API key')) {
          console.warn('⚠️  Gemini API key invalid or expired');
        } else if (errorMsg.includes('timeout')) {
          console.warn('⚠️  Gemini request timed out');
        } else {
          console.warn('⚠️  Gemini failed:', errorMsg.substring(0, 100));
        }
        
        console.log('🔄 [2/3] Instantly switching to Claude 4.5 Haiku...');
      }
    } else {
      console.log('⏭️  Gemini API key not configured, skipping to Claude...');
    }
    
    // ========================================
    // STEP 2: TRY CLAUDE 4.5 HAIKU IF GEMINI FAILED
    // ========================================
    if (!aiDisabled && !content) {
      const anthropicKey = ConfigService.get('ANTHROPIC_API_KEY') || process.env.ANTHROPIC_API_KEY;
      if (anthropicKey) {
        console.log('🤖 [2/3] Trying Claude 4.5 Haiku (reliable fallback)...');
        claudeAttempted = true;
        
        // Claude call with intelligent retries
        const callClaude = async () => {
          return axios.post(
            'https://api.anthropic.com/v1/messages',
            {
              model: 'claude-haiku-4-5-20251001',
              max_tokens: 800, // Increased for more creative content
              temperature: 0.85, // High creativity matching Gemini
              top_p: 0.95, // More diverse responses
              messages: [{ role: 'user', content: prompt }]
            },
            {
              headers: {
                'x-api-key': anthropicKey,
                'anthropic-version': '2023-06-01',
                'content-type': 'application/json'
              },
              timeout: 20000 // Increased timeout for quality responses
            }
          );
        };

        const sleep = (ms) => new Promise(res => setTimeout(res, ms));
        let claudeError = null;
        
        for (let attempt = 1; attempt <= 3; attempt++) {
          try {
            const resp = await callClaude();
            const parts = resp.data?.content || [];
            const text = Array.isArray(parts) ? parts.map(p=>p?.text||'').join('\n').trim() : '';
            
            if (text && text.length > 100) {
              content = text;
              providerUsed = 'claude-4.5-haiku';
              console.log(`✅ Claude SUCCESS! Professional caption generated (attempt ${attempt}, ${text.length} chars)`);
              claudeError = null;
              break;
            }
            throw new Error('Claude returned empty response');
            
          } catch (e) {
            claudeError = e;
            const status = e?.response?.status;
            const errorData = e?.response?.data?.error?.message || e?.message || String(e);
            
            // Intelligent error handling
            if (status === 429) {
              console.warn(`⚠️  Claude rate limit (attempt ${attempt}) - retrying with backoff...`);
            } else if (status >= 500) {
              console.warn(`⚠️  Claude server error ${status} (attempt ${attempt}) - retrying...`);
            } else if (status === 401) {
              console.warn(`⚠️  Claude API key invalid - skipping retries`);
              break; // Don't retry on auth errors
            } else {
              console.warn(`⚠️  Claude attempt ${attempt} failed: ${errorData.substring(0, 100)}`);
            }
            
            const transient = !status || status >= 500 || status === 429;
            if (attempt < 3 && transient) {
              const backoffDelay = 1000 * attempt; // Progressive backoff: 1s, 2s, 3s
              await sleep(backoffDelay);
              continue;
            }
            break;
          }
        }

        if (claudeError) {
          console.warn('⚠️  Claude failed after all retry attempts');
          console.log('🔄 [3/3] Switching to ready-made professional captions (guaranteed success)...');
        }
      } else {
        console.log('⏭️  Claude API key not configured, skipping to fallback captions...');
      }
    }
    
    // ========================================
    // STEP 3: USE FALLBACK CAPTIONS IF BOTH AI FAILED
    // ========================================
    if (!content) {
      console.log('🎯 [3/3] Loading seasonal/fallback captions...');
      try {
        // Use Sri Lankan calendar-aware seasonal caption system
        const { getSeasonalCaption, getCurrentEventInfo } = require('../utils/sriLankanCalendar');
        
        // Get appropriate caption (seasonal or regular fallback)
        const selectedCaption = getSeasonalCaption();
        const currentEvent = getCurrentEventInfo();
        
        if (currentEvent) {
          console.log(`🎉 SEASONAL CAPTION ACTIVE: ${currentEvent.name} (${currentEvent.dateRange.start} to ${currentEvent.dateRange.end})`);
          providerUsed = `seasonal-${currentEvent.key}`;
        } else {
          console.log('ℹ️  No seasonal event - using regular fallback captions');
          providerUsed = 'fallback-captions';
        }
        
        // Replace store phone number in caption
        content = selectedCaption.replace(/අමතන්න:/g, `අමතන්න: ${store.phone}`);
        content = content.replace(/Call:/g, `Call: ${store.phone}`);
        content = content.replace(/📞 [\d\s-]+/g, `📞 ${store.phone}`);
        content = content.replace(/0771222509/g, store.phone);
        
        console.log('✅ FALLBACK SUCCESS! Using calendar-aware caption system');
        console.log('📝 Caption preview:', content.substring(0, 80) + '...');
      } catch (fallbackError) {
        console.error('❌ CRITICAL: All caption generation methods failed!', fallbackError.message);
        
        // Last resort: Ultra-basic caption
        content = `🚗 ගුණාත්මක ටයර් - විශ්වාසදායී සේවාව!\n\nලසන්ත ටයර් ට්‍රේඩර්ස් ඔබට හොඳම තත්ත්වයේ ටයර් සහ විශේෂඥ සේවාව ලබා දෙයි.\n\n✨ N2 පිරවීම නොමිලේ\n✨ Tyre Fixing නොමිලේ\n\n📞 අමතන්න: ${store.phone}\n📍 ${store.location}\n\n#LasanthaTyreTraders #QualityTyres #TrustedService`;
        providerUsed = 'emergency-fallback';
        console.log('⚠️  Using emergency ultra-basic caption');
      }
    }
    
    // ========================================
    // REPORT WHICH METHOD WORKED
    // ========================================
    console.log('');
    console.log('═══════════════════════════════════════');
    if (providerUsed.includes('gemini')) {
      console.log('✅ CAPTION GENERATED BY: Gemini 2.5 Flash (fast & creative)');
    } else if (providerUsed.includes('claude')) {
      console.log('✅ CAPTION GENERATED BY: Claude 4.5 Haiku (reliable fallback)');
    } else if (providerUsed.startsWith('seasonal-')) {
      const eventName = providerUsed.replace('seasonal-', '').replace(/_/g, ' ').toUpperCase();
      console.log(`✅ CAPTION GENERATED BY: Seasonal Event - ${eventName} 🎉`);
    } else if (providerUsed === 'fallback-captions') {
      console.log('✅ CAPTION GENERATED BY: Professional Fallback Library (AI unavailable)');
    } else {
      console.log('⚠️  CAPTION GENERATED BY: Emergency Basic Template');
    }
    console.log('═══════════════════════════════════════');
    console.log('');

    // Ensure exact phone present
    const normalizeDigits = (s) => String(s||'').replace(/\D/g, '');
    if (!normalizeDigits(content).includes(normalizeDigits(store.phone))) {
      content += `\n\nCall: ${store.phone}`;
    }

    // Sanitize brand mentions: enforce allowed brand set and replace disallowed brands with selected product.brand
    try {
      let allowedBrands3 = ['MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO'];
      try {
        const brandListPath = path.join(__dirname, '..', 'assets', 'poster-elements', 'brand-list.json');
        if (fs.existsSync(brandListPath)) {
          const bl = JSON.parse(fs.readFileSync(brandListPath, 'utf-8'));
          if (Array.isArray(bl.brands) && bl.brands.length) allowedBrands3 = bl.brands.map(b => String(b).toUpperCase());
        }
      } catch {}
      const allBrandsSeen = new Set([ 'BRIDGESTONE','MICHELIN','PIRELLI','HANKOOK','APOLLO','CEAT','CONTINENTAL','TOYO','NEXEN','MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO' ]);
      const selected = String(product.brand || '').toUpperCase();
      const disallowed = [...allBrandsSeen].filter(b => b !== selected && !allowedBrands3.includes(b));
      disallowed.forEach(b => {
        // Replace tokens that are not part of a hashtag (no lookbehind for broad Node compatibility)
        const rx = new RegExp(`(^|[^#])\\b${b}\\b`, 'gi');
        content = content.replace(rx, (m, p1) => `${p1}${selected}`);
      });

      // Replace disallowed brand hashtags with selected brand hashtag
      const tagRx = /#([A-Za-z][A-Za-z0-9]+)/g;
      content = content.replace(tagRx, (m, p1) => {
        const up = p1.toUpperCase();
        if (!allowedBrands3.includes(up) && up !== selected) return `#${selected}`;
        return m;
      });
    } catch {}

    // Enforce no prices by sanitizing any currency mentions
    const pricePattern = /(Rs\.?|රු\.?|LKR|₨)\s*\d[\d,\.]*/gi;
    if (pricePattern.test(content)) {
      content = content.replace(pricePattern, '').replace(/\n\s*\n\s*\n+/g, '\n\n');
    }

    // Enforce no single-size mentions to keep brand/pattern focus
    const sizePatterns = [
      /\b\d{3}\s*\/\s*\d{2}\s*(R\s*\d{2})?\b/gi,      // 195/65 or 195/65R15
      /\b\d{2,3}\s*\/\s*\d{2}\s*R?\s*\d{2}\b/gi,       // 245/45R19 variants
      /\b\d{1,2}\.\d{2}\s*\/\s*\d{2}\b/gi,             // 6.50/16 style
      /\b\d{2,3}\s*\/\s*\d{2}\b/gi,                      // 165/13
      /\b\d{2,3}\s*PR\b/gi                                 // 8PR
    ];
    let cleaned = content;
    sizePatterns.forEach(rx => { cleaned = cleaned.replace(rx, 'many sizes'); });
    if (cleaned !== content) {
      content = cleaned.trim();
      if (!/many sizes available\.?$/i.test(content)) {
        content += "\n\nSizes: many options available.";
      }
    }

    // Remove hallucinated or nonsensical phrases (e.g., "SIM AJUSTMENT line")
    const badPhrases = [ /sim\s*ajus?tment\s*line/ig, /ajus?tment\s*line/ig ];
    badPhrases.forEach(rx => { content = content.replace(rx, '').replace(/\n\s*\n\s*\n+/g, '\n\n'); });

    // Ensure hashtags include brand and location, and meet minimum count
    const existingTags = (content.match(/#\w+/g) || []).map(t => t.toLowerCase());
    const brandTag = '#' + String(product.brand || '').replace(/\s+/g, '');
    const mustHave = [brandTag, '#Thalawathugoda', '#LasanthaTyreTraders'];
    const missing = mustHave.filter(t => !existingTags.includes(t.toLowerCase()));
    const generic = ['#Tyres','#TyreShop','#SriLanka','#WheelAlignment','#CarCare','#AutoCare','#Safety'];
    // Append missing mandatory tags
    if (missing.length) content += `\n${missing.join(' ')}`;
    // Ensure at least 3 hashtags
    let totalTags = (content.match(/#\w+/g) || []).length;
    let gi = 0;
    while (totalTags < 3 && gi < generic.length) {
      const t = generic[gi++];
      if (!content.toLowerCase().includes(t.toLowerCase())) {
        content += (totalTags === 0 ? '\n' : ' ') + t;
        totalTags++;
      }
    }

    // Provider trace (for observability)
    console.log('📊 Post generator provider:', providerUsed);

    // 8) QA Linter: Validate and auto-fix caption
    const linter = getCaptionLinter();
    const lintResult = linter.lint(content);
    let finalContent = lintResult.caption;

    // Final brand sanitization pass (post-lint) to guarantee compliance
    try {
      let allowedBrands4 = ['MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO'];
      const brandListPath = path.join(__dirname, '..', 'assets', 'poster-elements', 'brand-list.json');
      if (fs.existsSync(brandListPath)) {
        const bl = JSON.parse(fs.readFileSync(brandListPath, 'utf-8'));
        if (Array.isArray(bl.brands) && bl.brands.length) allowedBrands4 = bl.brands.map(b => String(b).toUpperCase());
      }
      const selectedFinal = String((lockedBrand || product.brand) || '').toUpperCase();
      const allSeenFinal = new Set(['BRIDGESTONE','MICHELIN','PIRELLI','HANKOOK','APOLLO','CEAT','CONTINENTAL','TOYO','NEXEN','MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO']);
      const disallowedFinal = [...allSeenFinal].filter(b => b !== selectedFinal && !allowedBrands4.includes(b));
      disallowedFinal.forEach(b => {
        const rx = new RegExp(`(^|[^#])\\b${b}\\b`, 'gi');
        finalContent = finalContent.replace(rx, (m, p1) => `${p1}${selectedFinal}`);
      });
      const tagRx = /#([A-Za-z][A-Za-z0-9]+)/g;
      finalContent = finalContent.replace(tagRx, (m, p1) => {
        const up = p1.toUpperCase();
        if (!allowedBrands4.includes(up) && up !== selectedFinal) return `#${selectedFinal}`;
        return m;
      });
      // Collapse duplicate brand hashtags like "#BRAND #BRAND #BRAND" -> "#BRAND"
      const brandTag = `#${selectedFinal}`;
      const dupRx = new RegExp(`(${brandTag})(?:\\s+\\1)+`, 'g');
      finalContent = finalContent.replace(dupRx, '$1');
    } catch {}
    linter.printReport(lintResult);

    // Before variants: enforce locked brand
    if (lockedBrand) {
      product.brand = lockedBrand; // normalize brand to locked selection
    }

    // 9) Generate A/B variants if enabled
    let abVariants = [];
    const abGen = getABVariantGenerator();
    if (abGen.enableAB) {
      console.log(`\n🔀 Generating ${abGen.variantCount} A/B variants...`);
      abVariants = await abGen.generateVariants(
        product,
        store,
        styleGuide || 'Premium tyre brand (style guide unavailable)',
        product.brandKnowledge ? `${product.brandKnowledge.origin} | ${product.brandKnowledge.tier}` : null
      );
      // Sanitize disallowed brand mentions inside variants as well
      try {
        let allowedAB = ['MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO'];
        const brandListPath = path.join(__dirname, '..', 'assets', 'poster-elements', 'brand-list.json');
        if (fs.existsSync(brandListPath)) {
          const bl = JSON.parse(fs.readFileSync(brandListPath, 'utf-8'));
          if (Array.isArray(bl.brands) && bl.brands.length) allowedAB = bl.brands.map(b => String(b).toUpperCase());
        }
        const selectedB = String(product.brand || '').toUpperCase();
        const allSeen = new Set(['BRIDGESTONE','MICHELIN','PIRELLI','HANKOOK','APOLLO','CEAT','CONTINENTAL','TOYO','NEXEN','MAXXIS','DURATURN','YOKOHAMA','GITI','MARSHAL','GT','KUMHO']);
        const disallowedB = [...allSeen].filter(b => b !== selectedB && !allowedAB.includes(b));
        const sanitize = (txt) => {
          let out = String(txt || '');
          disallowedB.forEach(b => {
            const rx = new RegExp(`(^|[^#])\\b${b}\\b`, 'gi');
            out = out.replace(rx, (m, p1) => `${p1}${selectedB}`);
          });
          // Sanitize hashtags as well
          const tagRx = /#([A-Za-z][A-ZaZ0-9]+)/g;
          out = out.replace(tagRx, (m, p1) => {
            const up = p1.toUpperCase();
            if (!allowedAB.includes(up) && up !== selectedB) return `#${selectedB}`;
            return m;
          });
          return out;
        };
        abVariants = abVariants.map(v => ({ ...v, caption: sanitize(v.caption || ''), title: sanitize(v.title || '') }));
      } catch {}
      if (abVariants.length > 0) {
        abGen.printVariants(abVariants);
      }
    }

    // 10) Resolve image according to mode: generate | external | none
  const imageMode = String((require('../utils/ConfigService').ConfigService.get('POST_IMAGE_MODE', process.env.POST_IMAGE_MODE || 'generate'))).toLowerCase();
    let imagePath = null;
    let templateId = null;
    const ts = Date.now();
    const stamp = new Date(ts).toISOString().replace(/[-:T.Z]/g, '').slice(0, 14);

    // Helper to select external image (sequential or newest)
    const resolveExternalImage = () => {
      try {
  const envDir = (require('../utils/ConfigService').ConfigService.get('EXTERNAL_IMAGE_DIR')) && String(require('../utils/ConfigService').ConfigService.get('EXTERNAL_IMAGE_DIR')).trim();
        const defaultDir = path.join(__dirname, '..', 'post');
        const dir = envDir || defaultDir;
        if (!fs.existsSync(dir)) {
          if (!envDir) {
            // Auto-create default folder when using fallback
            try { fs.mkdirSync(dir, { recursive: true }); } catch {}
            console.warn(`⚠️  External poster folder not found. Created default: ${dir}`);
          } else {
            console.warn('⚠️  EXTERNAL_IMAGE_DIR not set or does not exist.');
            return null;
          }
        }
        const entries = fs.readdirSync(dir, { withFileTypes: true });
        const exts = new Set(['.jpg','.jpeg','.png','.webp']);
        let files = entries
          .filter(e => e.isFile())
          .map(e => path.join(dir, e.name))
          .filter(fp => exts.has(path.extname(fp).toLowerCase()));
        if (!files.length) {
          console.warn('⚠️  No images found in EXTERNAL_IMAGE_DIR.');
          return null;
        }
  const strategy = String(require('../utils/ConfigService').ConfigService.get('POSTER_PICK_STRATEGY', 'sequential')).toLowerCase();
        if (strategy === 'newest') {
          const newest = files
            .map(f => ({ f, t: fs.statSync(f).mtimeMs }))
            .sort((a, b) => b.t - a.t)[0].f;
          return newest;
        }
        // sequential non-repeating until all used
        files = files.sort((a, b) => a.localeCompare(b, undefined, { sensitivity: 'base' }));
        // Build signatures to detect updated files with same name
        const fileInfo = files.map(f => {
          const st = fs.statSync(f);
          const base = path.basename(f);
          const sig = `${base}:${st.size}:${st.mtimeMs}`;
          return { f, base, sig, t: st.mtimeMs };
        });
        const statePath = path.join(this.draftsDir, '.external-image-state.json');
        let state = { dir, order: 'name', used: {}, last: null };
        try {
          if (fs.existsSync(statePath)) {
            const loaded = JSON.parse(fs.readFileSync(statePath, 'utf-8'));
            if (loaded && loaded.dir === dir) state = { ...state, ...loaded };
          }
        } catch {}
        // purge used entries that no longer exist
        try {
          const usedKeys = Object.keys(state.used || {});
          for (const k of usedKeys) {
            const full = path.join(dir, k);
            if (!fs.existsSync(full)) delete state.used[k];
          }
        } catch {}

        const basenames = fileInfo.map(o => o.base);
        const lastIdx = state.last ? basenames.indexOf(state.last) : -1;
        const n = fileInfo.length;
        let pickObj = null;
        // scan forward circularly for first not-used file (by sig)
        for (let i = 1; i <= n; i++) {
          const idx = (lastIdx + i) % n;
          const obj = fileInfo[idx];
          const usedSig = state.used[obj.base];
          if (usedSig !== obj.sig) { pickObj = obj; break; }
        }
        if (!pickObj) {
          // all used; reset and start from first in order
          state.used = {};
          pickObj = fileInfo[0];
        }
        state.used[pickObj.base] = pickObj.sig;
        state.last = pickObj.base;
        try { fs.writeFileSync(statePath, JSON.stringify(state, null, 2)); } catch {}
        return pickObj.f;
      } catch (e) {
        console.warn('⚠️  External image selection failed:', e.message);
        return null;
      }
    };

    if (imageMode === 'none') {
      console.log('📝 Skipping image generation (POST_IMAGE_MODE=none).');
      imagePath = null;
    } else if (imageMode === 'external') {
      const src = resolveExternalImage();
      if (src && fs.existsSync(src)) {
        const destExt = path.extname(src).toLowerCase() || '.jpg';
        const dest = path.join(this.draftsDir, `post_${stamp}${destExt}`);
        try {
          fs.copyFileSync(src, dest);
          imagePath = dest;
          console.log(`📷 Using external image: ${src}`);
          console.log(`✅ Copied to drafts: ${dest}`);
          // Also refresh samples/latest for quick viewing
          try {
            const samplesDir = path.join(this.draftsDir, 'samples');
            if (!fs.existsSync(samplesDir)) fs.mkdirSync(samplesDir, { recursive: true });
            const samplePath = path.join(samplesDir, `sample_${stamp}${destExt}`);
            fs.copyFileSync(dest, samplePath);
            try { fs.copyFileSync(dest, path.join(samplesDir, 'latest.jpg')); } catch {}
          } catch (copyErr) {
            console.warn('⚠️  Could not copy external image to samples dir:', copyErr.message);
          }
        } catch (e) {
          console.warn('⚠️  Failed to copy external image, proceeding without image:', e.message);
          imagePath = null;
        }
      } else {
        console.warn('⚠️  No usable external image found. Proceeding without image.');
        imagePath = null;
      }
    } else {
      // Default: generate poster image (product-specific) with theme variety
      const themes = ['blue', 'green', 'orange'];
      const theme = themes[Math.floor(Math.random() * themes.length)];
      // Select a curated JSON template ID that avoids recent repetition
      const selectTemplateId = () => {
        try {
          const baseDir = path.join(__dirname, '..', 'assets', 'poster-templates');
          const primary = path.join(baseDir, 'templates.json');
          const generated = path.join(baseDir, 'generated.templates.json');
          let defs = [];
          if (fs.existsSync(primary)) defs = defs.concat(JSON.parse(fs.readFileSync(primary, 'utf-8')) || []);
          if (fs.existsSync(generated)) defs = defs.concat(JSON.parse(fs.readFileSync(generated, 'utf-8')) || []);
          if (!Array.isArray(defs) || defs.length === 0) return null;
          const forcedTemplateId = process.env.FORCE_TEMPLATE_ID && String(process.env.FORCE_TEMPLATE_ID).trim();
          if (forcedTemplateId) {
            const forced = defs.find(d => d && d.id === forcedTemplateId);
            if (forced) {
              console.log(`⚙️  FORCE_TEMPLATE_ID matched: ${forcedTemplateId}`);
              return forcedTemplateId;
            }
            console.warn(`⚠️  FORCE_TEMPLATE_ID '${forcedTemplateId}' not found. Falling back to rotation logic.`);
          }

          const curated = defs.filter(d => d && typeof d.id === 'string' && !d.id.startsWith('auto-'));
          const pool = curated.length > 0 ? curated : defs;
          // Read recent templateIds from draft meta files
          const recentIds = new Set();
          try {
            const files = fs.readdirSync(this.draftsDir)
              .filter(f => f.startsWith('post_') && f.endsWith('.json'))
              .map(f => ({ f, t: fs.statSync(path.join(this.draftsDir, f)).mtimeMs }))
              .sort((a, b) => b.t - a.t)
              .slice(0, 8);
            for (const { f } of files) {
              try {
                const meta = JSON.parse(fs.readFileSync(path.join(this.draftsDir, f), 'utf-8'));
                if (meta && meta.templateId) recentIds.add(meta.templateId);
              } catch {}
            }
          } catch {}
          const candidates = pool.filter(d => !recentIds.has(d.id));
          const pickFrom = candidates.length > 0 ? candidates : pool;
          const pick = pickFrom[Math.floor(Math.random() * pickFrom.length)];
          return pick?.id || null;
        } catch (e) {
          console.warn('Template select failed:', e.message);
          return null;
        }
      };
      templateId = selectTemplateId();
      if (templateId) console.log(`🎯 Template chosen: ${templateId}`);
      // Use JSON-driven templates (no direct background images)
      const imageBuffer = await this.imageGen.generateProductPoster(product, theme, {
        templateMode: 'none', // avoid using raw poster images as backgrounds
        templateId
      });
      if (!imageBuffer || imageBuffer.length === 0) {
        throw new Error('Image generation failed - empty buffer returned');
      }
      const dest = path.join(this.draftsDir, `post_${stamp}.jpg`);
      try {
        fs.writeFileSync(dest, imageBuffer);
        console.log(`✅ Image saved: ${dest} (${(imageBuffer.length / 1024).toFixed(2)} KB)`);
        if (!fs.existsSync(dest)) throw new Error(`File was not created: ${dest}`);
        // Also save a copy into facebook-drafts/samples for quick access to themes
        try {
          const samplesDir = path.join(this.draftsDir, 'samples');
          if (!fs.existsSync(samplesDir)) fs.mkdirSync(samplesDir, { recursive: true });
          const samplePath = path.join(samplesDir, `sample_${stamp}.jpg`);
          fs.copyFileSync(dest, samplePath);
          try { fs.copyFileSync(dest, path.join(samplesDir, 'latest.jpg')); } catch {}
        } catch (copyErr) {
          console.warn('⚠️  Could not copy image to samples dir:', copyErr.message);
        }
      } catch (writeErr) {
        console.error(`❌ Failed to write image file: ${writeErr.message}`);
        throw writeErr;
      }
      imagePath = dest;
    }

    // 11) Save draft metadata JSON for traceability
    const meta = {
      brand: product.brand,
      size: product.size,
      contentType: product.contentType,
      provider: providerUsed,
      imageMode,
      theme: (typeof theme !== 'undefined') ? theme : null,
      templateId,
      imagePath,
      timestamp: new Date().toISOString(),
      qaLinter: {
        passed: !lintResult.needsFix,
        issues: lintResult.issues,
        warnings: lintResult.warnings
      },
      abVariants: abVariants.length
    };
    const metaPath = path.join(this.draftsDir, `post_${stamp}.json`);
    fs.writeFileSync(metaPath, JSON.stringify({ ...meta, content: finalContent }, null, 2));

    // 12) Save A/B variants if generated
    if (abVariants.length > 0) {
      await abGen.saveDrafts(abVariants, product.brand, null); // image shared, not duplicated
    }

    // 13) Add to history to prevent repetition
    await history.addPost({
      brand: product.brand,
      size: product.size,
      contentType: product.contentType,
      content: finalContent,
      imagePath: imagePath
    });
    
    return { content: finalContent, imagePath, type: 'promo', contentType: product.contentType };
  }
  
  async sendPublicationReportToAdmin(post, report) {
    if (!this.whatsapp || !this.adminWhatsAppNumber) {
      console.warn('⚠️  WhatsApp client or admin number not set. Skipping publication report.');
      return;
    }
    
    try {
      let chatId = this.adminWhatsAppNumber.replace(/\D/g, '');
      if (!chatId.startsWith('94')) chatId = '94' + chatId.substring(1);
      chatId += '@c.us';

      // BAILEYS MIGRATION: Use wrapper
      const { MessageMedia } = require('../utils/baileysWrapper');
      let statusMessage = '';

      switch (report.status) {
        case 'success':
          const postId = report.result?.id || 'N/A';
          const postUrl = postId.includes('_') ? `https://www.facebook.com/${postId}` : `https://www.facebook.com/posts/${postId}`;
          statusMessage = `✅ *Successfully Published*\n\nPost ID: ${postId}\nLink: ${postUrl}`;
          break;
        case 'error':
          const errorMessage = report.error?.response?.data?.error?.message || report.error?.message || 'Unknown error';
          statusMessage = `❌ *Failed to Publish*\n\nError: ${errorMessage.substring(0, 200)}`;
          break;
        case 'local_preview':
          statusMessage = `📝 *Local Preview Only*\n\nThis post was generated but not published to Facebook because LOCAL_PREVIEW_ONLY is enabled.`;
          break;
        default:
          statusMessage = `ℹ️ *Publication Report*\n\nStatus: ${report.status}`;
      }

      // Send the image and caption first, if available
      if (post && post.imagePath && fs.existsSync(post.imagePath)) {
        const imageData = fs.readFileSync(post.imagePath, { encoding: 'base64' });
        await this.whatsapp.sendMessage(chatId, new MessageMedia('image/jpeg', imageData, 'published-post.jpg'));
      }
      if (post && post.content) {
        await this.whatsapp.sendMessage(chatId, post.content);
      }
      
      // Send the final status message
      await this.whatsapp.sendMessage(chatId, statusMessage);
      console.log(`✅ Publication report sent to admin for status: ${report.status}`);

    } catch (err) {
      console.error('❌ Failed to send publication report to admin:', err.message);
    }
  }
  
  async handleApproval(postId, approved = true) {
    // This function is now mostly obsolete for the primary auto-publish flow,
    // but we keep it in case FB_APPROVAL_MODE is manually set to 'whatsapp'.
    const post = this.pendingPosts.get(postId);
    if (!post) return false;

    this.pendingPosts.delete(postId);
    
    if (!approved) {
      console.log(`❌ Admin rejected post ID: ${postId}. Deleting draft.`);
      if (fs.existsSync(post.imagePath)) fs.unlinkSync(post.imagePath);
      return true;
    }

    try {
      console.log(`✅ Admin approved post ID: ${postId}. Publishing...`);
      const result = await this.postToFacebook(post.content, post.imagePath);
      
      if (this.whatsapp && this.adminWhatsAppNumber) {
        let chatId = this.adminWhatsAppNumber.replace(/\D/g, '');
        if (!chatId.startsWith('94')) chatId = '94' + chatId.substring(1);
        chatId += '@c.us';
        
        const publishMode = String(ConfigService.get('FB_PUBLISH_MODE', this.publishMode)).toLowerCase();
        let adminMessage = '';
        if (publishMode === 'publish') {
          const postUrl = result.id.includes('_') ? `https://www.facebook.com/${result.id}` : `https://www.facebook.com/posts/${result.id}`;
          adminMessage = `✅ *Post Published!*\n\nPost ID: ${result.id}\nLink: ${postUrl}`;
        } else {
          adminMessage = `📝 *Draft Saved!*\n\nPost ID: ${result.id}\n\nDraft has been saved to the Facebook page. You can publish it manually from Publishing Tools > Drafts.`;
        }
        await this.whatsapp.sendMessage(chatId, adminMessage);
      }
    } catch (error) {
        console.error(`❌ Failed to publish approved post ${postId}:`, error.message);
        // Optionally, notify admin of the failure
        if (this.whatsapp && this.adminWhatsAppNumber) {
            let chatId = this.adminWhatsAppNumber.replace(/\D/g, '');
            if (!chatId.startsWith('94')) chatId = '94' + chatId.substring(1);
            chatId += '@c.us';
            await this.whatsapp.sendMessage(chatId, `❌ *Approval Failed!*\n\nCould not publish the post you approved (ID: ${postId}).\nError: ${error.message}`);
        }
    } finally {
        if (fs.existsSync(post.imagePath)) fs.unlinkSync(post.imagePath);
    }
    
    return true;
  }
  
  async postToFacebook(content, imagePath) {
    const FormData = require('form-data');
    const form = new FormData();
    const publishDirect = String((require('../utils/ConfigService').ConfigService.get('FB_PUBLISH_MODE', this.publishMode))).toLowerCase() === 'publish';
    form.append('message', content);
    form.append('published', publishDirect ? 'true' : 'false');
    const cfg = require('../utils/ConfigService').ConfigService;
    const pageId = cfg.get('FACEBOOK_PAGE_ID', this.pageId);
    const accessToken = cfg.get('FACEBOOK_PAGE_ACCESS_TOKEN', this.accessToken);
    form.append('access_token', accessToken);
    if (imagePath && fs.existsSync(imagePath)) {
      form.append('source', fs.readFileSync(imagePath), { filename: 'post.jpg', contentType: 'image/jpeg' });
      const response = await axios.post(`https://graph.facebook.com/v18.0/${pageId}/photos`, form, { headers: form.getHeaders(), maxContentLength: Infinity, maxBodyLength: Infinity });
      console.log(publishDirect ? ' Published photo to Facebook.' : ' Saved as DRAFT (unpublished). Admin can publish from Facebook.');
      return response.data;
    }
    const response = await axios.post(`https://graph.facebook.com/v18.0/${pageId}/feed`, { message: content, published: publishDirect, access_token: accessToken });
    console.log(publishDirect ? ' Published text post to Facebook.' : ' Saved as DRAFT (unpublished). Admin can publish from Facebook.');
    return response.data;
  }
  
  async enhanceWithGemini(template) {
    // Legacy method - no longer used (Claude-only setup)
    return template;
  }
  
  async testWithSimulatedApproval() {
    const post = await this.generatePost();
    console.log('\n Post:\n' + post.content + (post.imagePath ? ('\n ' + post.imagePath) : '\n (text-only)'));

  const localOnly = (require('../utils/ConfigService').ConfigService.getBoolean('LOCAL_PREVIEW_ONLY', false));
    if (localOnly) {
      if (post.imagePath) {
        console.log(`🖼️ Local preview generated only. Not posting to Facebook. Image kept at: ${post.imagePath}`);
      } else {
        console.log('📝 Local preview (text-only). Not posting to Facebook.');
      }
      return { id: 'local-preview', imagePath: post.imagePath || null };
    }

    const result = await this.postToFacebook(post.content, post.imagePath);
    // Clean up local file after posting to avoid clutter
    try {
      if (fs.existsSync(post.imagePath)) fs.unlinkSync(post.imagePath);
    } catch (e) {
      console.warn('⚠️ Could not delete local image after posting:', e.message);
    }
    console.log(' Published: ' + result.id);
    return result;
  }
}

module.exports = DailyFacebookPostJob;

if (require.main === module) {
  require('dotenv').config();
  new DailyFacebookPostJob().testWithSimulatedApproval().then(() => process.exit(0)).catch(err => { console.error(err); process.exit(1); });
}
