// Scheduled Media Publisher with WhatsApp Reporting & History Tracking
// දිනපතා 5:15 PM එකට image + caption publish කරන system
const cron = require('node-cron');
const fs = require('fs');
const path = require('path');
const axios = require('axios');
const { ConfigService } = require('../utils/ConfigService');

class ScheduledMediaPublisherJob {
  constructor(whatsappClient = null) {
    this.whatsapp = whatsappClient;
    this.postFolder = path.join(__dirname, '..', 'post');
    this.captionsFolder = path.join(this.postFolder, 'captions');
    this.processedFolder = path.join(this.postFolder, 'processed');
    this.historyFile = path.join(__dirname, '..', 'publish-history.json');
    this.pageId = process.env.FACEBOOK_PAGE_ID;
    this.accessToken = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
    this.adminNumber = process.env.ADMIN_WHATSAPP_NUMBER;
    this.publishMode = String(process.env.FB_PUBLISH_MODE || 'draft').toLowerCase();
    this.cronTask = null;
    this.whatsappLink = process.env.STORE_WHATSAPP_LINK || 'https://wa.me/94771222509';

    // Supported formats
    this.supportedFormats = {
      image: ['.jpg', '.jpeg', '.png', '.gif', '.webp'],
      video: ['.mp4', '.mov', '.avi', '.mkv', '.webm', '.3gp', '.flv']
    };

    // Initialize folders
    this.initializeFolders();
    
    // Load publish history
    this.history = this.loadHistory();
  }

  initializeFolders() {
    [this.postFolder, this.captionsFolder, this.processedFolder].forEach(dir => {
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
      }
    });
  }

  setWhatsAppClient(client) {
    this.whatsapp = client;
  }

  loadHistory() {
    try {
      if (fs.existsSync(this.historyFile)) {
        const data = fs.readFileSync(this.historyFile, 'utf-8');
        return JSON.parse(data);
      }
    } catch (err) {
      console.warn('⚠️  Could not load history:', err.message);
    }
    return {
      published: [],
      lastPublish: null,
      totalPublished: 0,
      failedAttempts: []
    };
  }

  saveHistory() {
    try {
      fs.writeFileSync(this.historyFile, JSON.stringify(this.history, null, 2));
    } catch (err) {
      console.error('❌ Could not save history:', err.message);
    }
  }

  start() {
    // Schedule for 08:45 AM daily
    this.cronTask = cron.schedule('45 8 * * *', () => {
      this.executePublish();
    }, {
      timezone: 'Asia/Colombo',
      recoverMissedExecutions: true
    });

    console.log('📅 Scheduled Media Publisher started');
    console.log('⏰ Will publish daily at 08:45 AM');
  }

  stop() {
    if (this.cronTask) {
      this.cronTask.stop();
      console.log('⏹️  Scheduled Media Publisher stopped');
    }
  }

  async executePublish() {
    const startTime = new Date();
    console.log('\n' + '='.repeat(60));
    console.log(`🚀 Daily Media Publish Started - ${startTime.toLocaleString('en-US', { timeZone: 'Asia/Colombo' })}`);
    console.log('='.repeat(60) + '\n');

    try {
      // Find unpublished media
      const mediaFile = await this.findNextUnpublishedMedia();
      
      if (!mediaFile) {
        const message = '📭 No unpublished media files found. All captions have been published!\n\n🔄 To continue, add new media files to the post folder.';
        console.log(message);
        await this.sendAdminReport('INFO', 'No Unpublished Media', message);
        return;
      }

      console.log(`📄 Selected file: ${mediaFile}`);

      // Get caption
      const caption = await this.getCaptionForFile(mediaFile);
      const captionWithLink = this.addWhatsAppLink(caption);

      console.log(`📝 Caption preview: ${captionWithLink.substring(0, 100)}...`);

      // Publish to Facebook
      const filePath = path.join(this.postFolder, mediaFile);
      const result = await this.publishToFacebook(captionWithLink, filePath, mediaFile);

      if (result.success) {
        // Record success
        this.history.published.push({
          file: mediaFile,
          caption: caption,
          publishDate: new Date().toISOString(),
          facebookId: result.facebookId,
          status: 'published'
        });
        this.history.lastPublish = new Date().toISOString();
        this.history.totalPublished++;
        this.saveHistory();

        // Move to processed folder (ensure folder exists first)
        if (!fs.existsSync(this.processedFolder)) {
          fs.mkdirSync(this.processedFolder, { recursive: true });
        }
        const processedPath = path.join(this.processedFolder, mediaFile);
        fs.renameSync(filePath, processedPath);

        // Send success report to admin
        await this.sendAdminReport('SUCCESS', 'Published Successfully', 
          `✅ *Post Published Successfully!*\n\n` +
          `📁 File: ${mediaFile}\n` +
          `📋 Facebook ID: ${result.facebookId}\n` +
          `📝 Caption: ${caption.substring(0, 50)}...\n` +
          `⏰ Time: ${new Date().toLocaleTimeString('en-US', { timeZone: 'Asia/Colombo' })}\n` +
          `📊 Total Published: ${this.history.totalPublished}\n\n` +
          `🎉 Post is now ${this.publishMode === 'publish' ? 'LIVE' : 'saved as DRAFT'} on Facebook!`
        );

        console.log(`✅ Published successfully: ${result.facebookId}`);
      } else {
        // Record failure
        this.history.failedAttempts.push({
          file: mediaFile,
          error: result.error,
          date: new Date().toISOString()
        });
        this.saveHistory();

        // Send failure report to admin
        await this.sendAdminReport('FAILURE', 'Publish Failed', 
          `❌ *Publishing Failed*\n\n` +
          `📁 File: ${mediaFile}\n` +
          `⚠️ Error: ${result.error}\n` +
          `⏰ Time: ${new Date().toLocaleTimeString('en-US', { timeZone: 'Asia/Colombo' })}\n\n` +
          `Please check the bot logs for details.`
        );

        console.error(`❌ Publish failed: ${result.error}`);
      }

    } catch (err) {
      console.error('❌ Execute publish error:', err.message);
      await this.sendAdminReport('ERROR', 'System Error', 
        `❌ *System Error*\n\n` +
        `Error: ${err.message}\n` +
        `⏰ Time: ${new Date().toLocaleTimeString('en-US', { timeZone: 'Asia/Colombo' })}`
      );
    }

    const endTime = new Date();
    const duration = ((endTime - startTime) / 1000).toFixed(2);
    console.log('\n' + '='.repeat(60));
    console.log(`✅ Daily Publish Complete - Duration: ${duration}s`);
    console.log('='.repeat(60) + '\n');
  }

  async findNextUnpublishedMedia() {
    try {
      const files = fs.readdirSync(this.postFolder);
      
      // Get all media files in the folder (alphabetically sorted)
      const allMediaFiles = files.filter(file => {
        const ext = path.extname(file).toLowerCase();
        return [...this.supportedFormats.image, ...this.supportedFormats.video].includes(ext);
      }).sort(); // Sort alphabetically for consistent order

      if (allMediaFiles.length === 0) {
        console.log('📭 No media files found in post folder');
        return null;
      }

      // Get files that have been published
      const publishedFiles = this.history.published.map(p => p.file);

      // Find unpublished media first
      const unpublishedMedia = allMediaFiles.filter(file => !publishedFiles.includes(file));

      if (unpublishedMedia.length > 0) {
        console.log(`📂 Found ${unpublishedMedia.length} unpublished file(s), selecting first one`);
        return unpublishedMedia[0];
      }

      // 🔄 ALL FILES HAVE BEEN PUBLISHED - STOP POSTING
      console.log('✅ All files have been published! No more posts to publish.');
      console.log(`📊 Total published: ${this.history.totalPublished} posts`);
      console.log('💡 Add new files to post/ folder to publish more content');
      
      // Don't reset history - wait for new files instead
      // this.history.published = [];
      // this.saveHistory();

      // Return null to stop posting
      return null;

    } catch (err) {
      console.error('❌ Error finding unpublished media:', err.message);
      return null;
    }
  }

  async getCaptionForFile(filename) {
    // Caption matching strategies (same as MediaPublisherJob)
    const baseName = path.parse(filename).name;
    const captionCandidates = [
      `${filename}.txt`,
      `${baseName}.txt`,
      `${baseName.replace(/\D/g, '')}.txt`,
      'default.txt'
    ];

    for (const candidate of captionCandidates) {
      const captionPath = path.join(this.captionsFolder, candidate);
      try {
        const content = fs.readFileSync(captionPath, 'utf-8');
        const trimmed = content.trim();
        if (trimmed) {
          console.log(`📄 Using caption file: ${candidate}`);
          return trimmed;
        }
      } catch (err) {
        // File not found, try next
      }
    }

    // Default caption if no file found
    return `🎉 අපගේ නවතම දැන්වීම!\n\n📞 ${process.env.STORE_PHONE || '0771222509'}\n📍 ${process.env.STORE_LOCATION || 'Thalawathugoda'}`;
  }

  addWhatsAppLink(caption) {
    // Add WhatsApp link at the end of caption
    const separator = '\n\n━━━━━━━━━━━━━━━\n\n';
    const whatsappText = `💬 WhatsApp මගින් අමතන්න:\n${this.whatsappLink}`;
    return `${caption}${separator}${whatsappText}`;
  }

  async publishToFacebook(caption, mediaPath, filename) {
    try {
      const FormData = require('form-data');
      const form = new FormData();
      const publishDirect = this.publishMode === 'publish';
      
      form.append('message', caption);
      form.append('published', publishDirect ? 'true' : 'false');
      
      const pageId = ConfigService.get('FACEBOOK_PAGE_ID', this.pageId);
      const accessToken = ConfigService.get('FACEBOOK_PAGE_ACCESS_TOKEN', this.accessToken);
      form.append('access_token', accessToken);

      if (fs.existsSync(mediaPath)) {
        const mediaBuffer = fs.readFileSync(mediaPath);
        const ext = path.extname(mediaPath).substring(1).toLowerCase();
        const mediaType = this.supportedFormats.image.includes('.' + ext) ? 'image' : 'video';
        
        if (mediaType === 'image') {
          form.append('source', mediaBuffer, {
            filename: `media.${ext}`,
            contentType: this.getContentType(ext, 'image')
          });
          
          const endpoint = `https://graph.facebook.com/v18.0/${pageId}/photos`;
          const response = await axios.post(endpoint, form, {
            headers: form.getHeaders(),
            maxContentLength: Infinity,
            maxBodyLength: Infinity,
            timeout: 120000
          });
          
          return {
            success: true,
            facebookId: response.data.id || response.data.post_id,
            type: 'image'
          };
          
        } else if (mediaType === 'video') {
          form.append('source', mediaBuffer, {
            filename: `video.${ext}`,
            contentType: this.getContentType(ext, 'video')
          });
          
          const endpoint = `https://graph.facebook.com/v18.0/${pageId}/videos`;
          const response = await axios.post(endpoint, form, {
            headers: form.getHeaders(),
            maxContentLength: Infinity,
            maxBodyLength: Infinity,
            timeout: 300000 // 5 minutes for videos
          });
          
          return {
            success: true,
            facebookId: response.data.id,
            type: 'video'
          };
        }
      }

      throw new Error('Invalid media file');

    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.error?.message || err.message
      };
    }
  }

  getContentType(ext, mediaType) {
    const types = {
      jpg: 'image/jpeg',
      jpeg: 'image/jpeg',
      png: 'image/png',
      gif: 'image/gif',
      webp: 'image/webp',
      mp4: 'video/mp4',
      mov: 'video/quicktime',
      avi: 'video/x-msvideo',
      mkv: 'video/x-matroska',
      webm: 'video/webm',
      flv: 'video/x-flv',
      wmv: 'video/x-ms-wmv',
      '3gp': 'video/3gpp'
    };
    
    return types[ext.toLowerCase()] || (mediaType === 'video' ? 'video/mp4' : 'image/jpeg');
  }

  async sendAdminReport(type, title, message) {
    if (!this.whatsapp || !this.adminNumber) {
      console.log('⚠️  WhatsApp not configured for admin reports');
      return;
    }

    try {
      let chatId = this.adminNumber.replace(/\D/g, '');
      if (!chatId.startsWith('94')) {
        chatId = '94' + chatId.substring(1);
      }
      chatId += '@c.us';

      const emoji = {
        'SUCCESS': '✅',
        'FAILURE': '❌',
        'ERROR': '⚠️',
        'INFO': 'ℹ️'
      }[type] || '📢';

      const fullMessage = `${emoji} *${title}*\n\n${message}`;
      
      await this.whatsapp.sendMessage(chatId, fullMessage);
      console.log(`📱 Admin report sent: ${type}`);

    } catch (err) {
      console.error('❌ Error sending admin report:', err.message);
    }
  }

  getPublishStats() {
    const totalMedia = fs.readdirSync(this.postFolder)
      .filter(f => {
        const ext = path.extname(f).toLowerCase();
        return [...this.supportedFormats.image, ...this.supportedFormats.video].includes(ext);
      }).length;

    return {
      totalPublished: this.history.totalPublished,
      remainingMedia: totalMedia,
      lastPublish: this.history.lastPublish,
      failedAttempts: this.history.failedAttempts.length,
      publishedFiles: this.history.published.map(p => p.file)
    };
  }

  async testPublish() {
    console.log('🧪 Running test publish...\n');
    await this.executePublish();
  }
}

module.exports = ScheduledMediaPublisherJob;

// Test if run directly
if (require.main === module) {
  require('dotenv').config();
  const job = new ScheduledMediaPublisherJob();
  
  console.log('📊 Current Stats:');
  const stats = job.getPublishStats();
  console.log(`   Total Published: ${stats.totalPublished}`);
  console.log(`   Remaining Media: ${stats.remainingMedia}`);
  console.log(`   Last Publish: ${stats.lastPublish || 'Never'}`);
  console.log(`   Failed Attempts: ${stats.failedAttempts}\n`);
  
  job.testPublish()
    .then(() => {
      console.log('\n✅ Test complete!');
      process.exit(0);
    })
    .catch(err => {
      console.error('\n❌ Test failed:', err);
      process.exit(1);
    });
}
