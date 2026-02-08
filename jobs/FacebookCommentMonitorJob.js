// Facebook Comment Monitor Job (Auto-reply with Ollama AI)
const cron = require('node-cron');
const axios = require('axios');
const OllamaService = require('../utils/OllamaService');

class FacebookCommentMonitorJob {
  constructor() {
    this.task = null;
    this.ollama = new OllamaService();
    
    this.pageId = process.env.FACEBOOK_PAGE_ID;
    this.accessToken = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
    
    // Track replied comments to avoid duplicates
    this.repliedComments = new Set();
    
    // Keywords that trigger auto-reply (Sinhala + English)
    this.keywords = [
      'මිල', 'price', 'ගාන', 'කීයද',
      'තියෙනවද', 'stock', 'available',
      'කොහෙද', 'location', 'address',
      'අංකය', 'contact', 'phone', 'number'
    ];
    
    if (!this.pageId || !this.accessToken) {
      console.error('❌ Facebook credentials missing');
    }
  }
  
  start() {
    console.log('👁️ Starting FacebookCommentMonitorJob...');
    
    // Check comments every 30 minutes
    this.task = cron.schedule('*/30 * * * *', () => {
      this.checkAndReplyToComments();
    }, {
      timezone: 'Asia/Colombo',
      recoverMissedExecutions: true
    });
    
    console.log('✅ Comment monitoring: Every 30 minutes');
    
    // Run once immediately on startup
    this.checkAndReplyToComments();
  }
  
  stop() {
    if (this.task) {
      this.task.stop();
      console.log('⏹️ FacebookCommentMonitorJob stopped');
    }
  }
  
  async checkAndReplyToComments() {
    try {
      console.log('\n🔍 Checking Facebook comments...');
      
      // Get recent posts (last 5)
      const postsUrl = `https://graph.facebook.com/v18.0/${this.pageId}/posts`;
      const postsResponse = await axios.get(postsUrl, {
        params: {
          access_token: this.accessToken,
          limit: 5,
          fields: 'id,message,created_time'
        }
      });
      
      const posts = postsResponse.data.data || [];
      console.log(`📄 Found ${posts.length} recent posts`);
      
      let totalReplied = 0;
      
      // Check comments on each post
      for (const post of posts) {
        const comments = await this.getCommentsForPost(post.id);
        
        for (const comment of comments) {
          // Skip if already replied
          if (this.repliedComments.has(comment.id)) {
            continue;
          }
          
          // Check if comment matches keywords
          if (this.shouldReply(comment.message)) {
            await this.replyToComment(comment);
            this.repliedComments.add(comment.id);
            totalReplied++;
          }
        }
      }
      
      console.log(`✅ Replied to ${totalReplied} new comments\n`);
      
      // Cleanup old replied IDs (keep last 100)
      if (this.repliedComments.size > 100) {
        const arr = Array.from(this.repliedComments);
        this.repliedComments = new Set(arr.slice(-100));
      }
      
    } catch (err) {
      console.error('❌ Comment monitoring error:', err.message);
      if (err.response && err.response.data) {
        console.error('   ↳ Response data:', err.response.data);
      }
    }
  }
  
  async getCommentsForPost(postId) {
    try {
      const url = `https://graph.facebook.com/v18.0/${postId}/comments`;
      const response = await axios.get(url, {
        params: {
          access_token: this.accessToken,
          fields: 'id,message,from,created_time',
          limit: 20
        }
      });
      
      return response.data.data || [];
    } catch (err) {
      console.error(`Error getting comments for ${postId}:`, err.message);
      return [];
    }
  }
  
  shouldReply(message) {
    const lower = message.toLowerCase();
    
    // Check if any keyword is present
    return this.keywords.some(keyword => 
      lower.includes(keyword.toLowerCase())
    );
  }
  
  async replyToComment(comment) {
    try {
      console.log(`💬 Replying to: "${comment.message.substring(0, 50)}..."`);
      
      // Analyze sentiment with Ollama
      const sentiment = await this.ollama.analyzeSentiment(comment.message);
      console.log(`   Sentiment: ${sentiment}`);
      
      // Generate reply with Ollama
      let reply = await this.ollama.generateReply(comment.message, sentiment);
      
      // Post reply
      const url = `https://graph.facebook.com/v18.0/${comment.id}/comments`;
      await axios.post(url, {
        message: reply,
        access_token: this.accessToken
      });
      
      console.log(`✅ Reply posted to comment ${comment.id}`);
      
      // If negative sentiment, alert admin (could extend to WhatsApp alert)
      if (sentiment === 'NEGATIVE') {
        console.log(`⚠️ ALERT: Negative comment detected from ${comment.from.name}`);
        // TODO: Send WhatsApp alert to admin
      }
      
      return true;
      
    } catch (err) {
      console.error(`❌ Failed to reply to comment ${comment.id}:`, err.message);
      return false;
    }
  }
  
  // Manual test
  async testMonitoring() {
    console.log('🧪 Testing comment monitoring...');
    await this.checkAndReplyToComments();
  }
}

module.exports = FacebookCommentMonitorJob;

// Test if run directly: node jobs/FacebookCommentMonitorJob.js
if (require.main === module) {
  require('dotenv').config();
  const job = new FacebookCommentMonitorJob();
  job.testMonitoring()
    .then(() => {
      console.log('✅ Test complete!');
      process.exit(0);
    })
    .catch(err => {
      console.error('❌ Test failed:', err);
      process.exit(1);
    });
}
