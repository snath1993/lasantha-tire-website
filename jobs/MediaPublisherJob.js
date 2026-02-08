// Media Publisher Job - Auto-publish images, videos, GIFs from post folder
// Supports: JPG, PNG, WEBP, GIF, MP4, MOV, AVI, MKV, WEBM
const fs = require('fs');
const path = require('path');
const axios = require('axios');
const { ConfigService } = require('../utils/ConfigService');

class MediaPublisherJob {
  constructor() {
    this.postFolder = path.join(__dirname, '..', 'post');
    this.processedFolder = path.join(__dirname, '..', 'post', 'processed');
    this.failedFolder = path.join(__dirname, '..', 'post', 'failed');
    this.pageId = process.env.FACEBOOK_PAGE_ID;
    this.accessToken = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
    this.publishMode = String(process.env.FB_PUBLISH_MODE || 'draft').toLowerCase();
    this.scanIntervalMs = Number(process.env.MEDIA_PUBLISHER_SCAN_INTERVAL_MS || 30000); // 30 seconds default
    this.isRunning = false;
    this.scanTimer = null;

    // Supported media formats
    this.supportedFormats = {
      image: ['.jpg', '.jpeg', '.png', '.gif', '.webp'],
      video: ['.mp4', '.mov', '.avi', '.mkv', '.webm', '.3gp', '.flv']
    };

    // Create required folders
    [this.postFolder, this.processedFolder, this.failedFolder].forEach(dir => {
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
      }
    });

    console.log('📁 Media Publisher initialized:');
    console.log(`   Watch folder: ${this.postFolder}`);
    console.log(`   Scan interval: ${this.scanIntervalMs / 1000}s`);
    console.log(`   Publish mode: ${this.publishMode}`);
  }

  /**
   * Start continuous monitoring
   */
  start() {
    if (this.isRunning) {
      console.log('⚠️  Media Publisher already running');
      return;
    }

    this.isRunning = true;
    console.log('🚀 Media Publisher started');

    // Initial scan
    this.scanAndPublish().catch(err => {
      console.error('❌ Initial scan error:', err.message);
    });

    // Schedule continuous scanning
    this.scanTimer = setInterval(() => {
      this.scanAndPublish().catch(err => {
        console.error('❌ Scan error:', err.message);
      });
    }, this.scanIntervalMs);
  }

  /**
   * Stop monitoring
   */
  stop() {
    if (this.scanTimer) {
      clearInterval(this.scanTimer);
      this.scanTimer = null;
    }
    this.isRunning = false;
    console.log('⏸️  Media Publisher stopped');
  }

  /**
   * Scan post folder and publish media files
   */
  async scanAndPublish() {
    try {
      const files = fs.readdirSync(this.postFolder, { withFileTypes: true });
      
      // Filter for media files only (not folders)
      const mediaFiles = files.filter(file => {
        if (!file.isFile()) return false;
        const ext = path.extname(file.name).toLowerCase();
        return this.isImageFile(ext) || this.isVideoFile(ext);
      });

      if (mediaFiles.length === 0) {
        console.log('📭 No media files found in post folder');
        return;
      }

      console.log(`📬 Found ${mediaFiles.length} media file(s) to publish`);

      // Process each file
      for (const file of mediaFiles) {
        const filePath = path.join(this.postFolder, file.name);
        await this.processMediaFile(filePath);
        
        // Small delay between posts to avoid rate limiting
        await this.delay(2000);
      }

    } catch (err) {
      console.error('❌ Scan and publish error:', err.message);
    }
  }

  /**
   * Process and publish a single media file
   */
  async processMediaFile(filePath) {
    const fileName = path.basename(filePath);
    const ext = path.extname(filePath).toLowerCase();

    try {
      console.log(`\n🔄 Processing: ${fileName}`);

      // Determine media type
      const mediaType = this.isImageFile(ext) ? 'image' : 'video';
      console.log(`   Type: ${mediaType}`);

      // Get caption (from caption.txt or generate default)
      const caption = await this.getCaptionForFile(filePath);
      console.log(`   Caption: ${caption.substring(0, 50)}...`);

      // Publish to Facebook
      const result = await this.publishToFacebook(filePath, caption, mediaType);

      // Move to processed folder
      const processedPath = path.join(this.processedFolder, fileName);
      fs.renameSync(filePath, processedPath);

      console.log(`✅ Published successfully: ${result.id}`);
      console.log(`   Moved to: ${processedPath}`);

    } catch (err) {
      console.error(`❌ Failed to process ${fileName}:`, err.message);
      
      // Move to failed folder
      try {
        const failedPath = path.join(this.failedFolder, fileName);
        fs.renameSync(filePath, failedPath);
        console.log(`   Moved to failed folder: ${failedPath}`);
      } catch (moveErr) {
        console.error(`   Could not move to failed folder:`, moveErr.message);
      }
    }
  }

  /**
   * Get caption for a media file
   * Looks for: <filename>-caption.txt or caption.txt in post folder
   */
  async getCaptionForFile(filePath) {
    const dir = path.dirname(filePath);
    const baseName = path.basename(filePath, path.extname(filePath));

    // Try specific caption file: <filename>-caption.txt
    const specificCaptionFile = path.join(dir, `${baseName}-caption.txt`);
    if (fs.existsSync(specificCaptionFile)) {
      return fs.readFileSync(specificCaptionFile, 'utf-8').trim();
    }

    // Try generic caption.txt
    const genericCaptionFile = path.join(dir, 'caption.txt');
    if (fs.existsSync(genericCaptionFile)) {
      return fs.readFileSync(genericCaptionFile, 'utf-8').trim();
    }

    // Default caption with emojis
    return this.generateDefaultCaption();
  }

  /**
   * Generate default Sinhala caption
   */
  generateDefaultCaption() {
    const captions = [
      '🚗💨 අපේ වාහනවල තත්ත්වය අද බලන්න!\n\n📞 0771222509\n📍 Thalawathugoda\n\n#LasanthaTyreTraders #QualityTyres #SriLanka',
      '🔥 විශේෂ දීමනාවක් සහිතව!\n\n📞 0771222509\n📍 Thalawathugoda\n\n#Tyres #BestPrice #LasanthaTyre',
      '✨ නව තොග පැමිණ ඇත!\n\n📞 0771222509\n📍 Thalawathugoda\n\n#NewStock #TyreShop #Quality'
    ];
    return captions[Math.floor(Math.random() * captions.length)];
  }

  /**
   * Publish media to Facebook
   */
  async publishToFacebook(mediaPath, caption, mediaType) {
    const FormData = require('form-data');
    const form = new FormData();
    
    const publishDirect = this.publishMode === 'publish';
    const pageId = ConfigService.get('FACEBOOK_PAGE_ID', this.pageId);
    const accessToken = ConfigService.get('FACEBOOK_PAGE_ACCESS_TOKEN', this.accessToken);

    form.append('message', caption);
    form.append('published', publishDirect ? 'true' : 'false');
    form.append('access_token', accessToken);

    // Read and append media file
    const mediaBuffer = fs.readFileSync(mediaPath);
    const fileName = path.basename(mediaPath);
    
    if (mediaType === 'image') {
      // Detect content type from extension
      const ext = path.extname(mediaPath).toLowerCase();
      const contentType = this.getImageContentType(ext);
      
      form.append('source', mediaBuffer, { 
        filename: fileName, 
        contentType: contentType 
      });

      const url = `https://graph.facebook.com/v18.0/${pageId}/photos`;
      const response = await axios.post(url, form, { 
        headers: form.getHeaders(), 
        maxContentLength: Infinity, 
        maxBodyLength: Infinity 
      });

      console.log(publishDirect ? '📸 Published photo to Facebook' : '📸 Saved photo as DRAFT');
      return response.data;

    } else {
      // Video upload
      const ext = path.extname(mediaPath).toLowerCase();
      const contentType = this.getVideoContentType(ext);
      
      form.append('source', mediaBuffer, { 
        filename: fileName, 
        contentType: contentType 
      });

      const url = `https://graph.facebook.com/v18.0/${pageId}/videos`;
      const response = await axios.post(url, form, { 
        headers: form.getHeaders(), 
        maxContentLength: Infinity, 
        maxBodyLength: Infinity,
        timeout: 300000 // 5 minutes for video upload
      });

      console.log(publishDirect ? '🎬 Published video to Facebook' : '🎬 Saved video as DRAFT');
      return response.data;
    }
  }

  /**
   * Check if file extension is an image
   */
  isImageFile(ext) {
    return this.supportedFormats.image.includes(ext.toLowerCase());
  }

  /**
   * Check if file extension is a video
   */
  isVideoFile(ext) {
    return this.supportedFormats.video.includes(ext.toLowerCase());
  }

  /**
   * Get image content type from extension
   */
  getImageContentType(ext) {
    const types = {
      '.jpg': 'image/jpeg',
      '.jpeg': 'image/jpeg',
      '.png': 'image/png',
      '.gif': 'image/gif',
      '.webp': 'image/webp'
    };
    return types[ext.toLowerCase()] || 'image/jpeg';
  }

  /**
   * Get video content type from extension
   */
  getVideoContentType(ext) {
    const types = {
      '.mp4': 'video/mp4',
      '.mov': 'video/quicktime',
      '.avi': 'video/x-msvideo',
      '.mkv': 'video/x-matroska',
      '.webm': 'video/webm',
      '.3gp': 'video/3gpp',
      '.flv': 'video/x-flv'
    };
    return types[ext.toLowerCase()] || 'video/mp4';
  }

  /**
   * Delay helper
   */
  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * Manual publish single file (for testing)
   */
  async publishSingleFile(fileName) {
    const filePath = path.join(this.postFolder, fileName);
    
    if (!fs.existsSync(filePath)) {
      throw new Error(`File not found: ${filePath}`);
    }

    await this.processMediaFile(filePath);
  }

  /**
   * Get current status
   */
  getStatus() {
    const files = fs.readdirSync(this.postFolder, { withFileTypes: true });
    const mediaFiles = files.filter(file => {
      if (!file.isFile()) return false;
      const ext = path.extname(file.name).toLowerCase();
      return this.isImageFile(ext) || this.isVideoFile(ext);
    });

    const processedFiles = fs.existsSync(this.processedFolder)
      ? fs.readdirSync(this.processedFolder).length
      : 0;

    const failedFiles = fs.existsSync(this.failedFolder)
      ? fs.readdirSync(this.failedFolder).length
      : 0;

    return {
      isRunning: this.isRunning,
      scanInterval: `${this.scanIntervalMs / 1000}s`,
      publishMode: this.publishMode,
      pendingFiles: mediaFiles.length,
      processedFiles: processedFiles,
      failedFiles: failedFiles,
      supportedFormats: this.supportedFormats
    };
  }
}

module.exports = MediaPublisherJob;

// Test if run directly
if (require.main === module) {
  require('dotenv').config();
  
  const job = new MediaPublisherJob();
  
  console.log('\n📊 Media Publisher Status:');
  console.log(JSON.stringify(job.getStatus(), null, 2));
  
  console.log('\n🔍 Starting one-time scan...\n');
  
  job.scanAndPublish()
    .then(() => {
      console.log('\n✅ Scan complete!');
      process.exit(0);
    })
    .catch(err => {
      console.error('\n❌ Scan failed:', err);
      process.exit(1);
    });
}
