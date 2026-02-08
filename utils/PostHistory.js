/**
 * Post History Manager
 * Tracks generated posts to prevent repetition
 * Ensures variety and freshness in content
 */

const fs = require('fs').promises;
const path = require('path');
const crypto = require('crypto');

class PostHistory {
  constructor() {
    this.historyPath = path.join(__dirname, '..', 'post-generation-history.json');
    this.history = null;
  }
  
  async load() {
    try {
      const data = await fs.readFile(this.historyPath, 'utf8');
      this.history = JSON.parse(data);
    } catch (error) {
      // File doesn't exist or error reading - create new
      this.history = {
        posts: [],
        brands: {},
        sizes: {},
        contentTypes: {},
        lastGenerated: null
      };
    }
  }
  
  async save() {
    await fs.writeFile(this.historyPath, JSON.stringify(this.history, null, 2), 'utf8');
  }
  
  /**
   * Generate content hash for similarity detection
   */
  generateHash(content) {
    return crypto.createHash('md5').update(content.toLowerCase().replace(/\s+/g, '')).digest('hex');
  }
  
  /**
   * Check if post is too similar to recent posts
   * @param {Object} post - Post data (brand, size, content)
   * @returns {boolean} True if too similar
   */
  async isTooSimilar(post) {
    if (!this.history) await this.load();
    
    const recentPosts = this.history.posts.slice(-30); // Check last 30 posts
    
    // Brand cooldown by days
    const cooldownDays = Number(process.env.MIN_DAYS_BETWEEN_BRAND || 3);
    if (post.brand && this.history.brands[post.brand]?.lastUsed && cooldownDays > 0) {
      const last = new Date(this.history.brands[post.brand].lastUsed);
      const now = new Date();
      const diffDays = (now - last) / (1000 * 60 * 60 * 24);
      if (diffDays < cooldownDays) {
        console.log(`⚠️  Brand on cooldown: ${post.brand} used ${diffDays.toFixed(1)}d ago (< ${cooldownDays}d)`);
        return true;
      }
    }
    
    // Check exact brand + size combination in last 10 posts
    const last10 = recentPosts.slice(-10);
    const sameProduct = last10.some(p => 
      p.brand === post.brand && p.size === post.size
    );
    
    if (sameProduct) {
      console.log(`⚠️  Recently used: ${post.brand} ${post.size} (within last 10 posts)`);
      return true;
    }
    
    // Check content hash for near-duplicates
    const postHash = this.generateHash(post.content);
    const similarContent = recentPosts.some(p => {
      if (!p.contentHash) return false;
      const similarity = this.calculateSimilarity(postHash, p.contentHash);
      return similarity > 0.85; // 85% similar = too similar
    });
    
    if (similarContent) {
      console.log(`⚠️  Similar content detected`);
      return true;
    }
    
    return false;
  }
  
  /**
   * Calculate similarity between two hashes (simple approach)
   */
  calculateSimilarity(hash1, hash2) {
    if (hash1 === hash2) return 1.0;
    
    let matches = 0;
    const length = Math.min(hash1.length, hash2.length);
    for (let i = 0; i < length; i++) {
      if (hash1[i] === hash2[i]) matches++;
    }
    return matches / length;
  }
  
  /**
   * Add post to history
   */
  async addPost(post) {
    if (!this.history) await this.load();
    
    const historyEntry = {
      brand: post.brand,
      size: post.size,
      contentType: post.contentType || 'single',
      contentHash: this.generateHash(post.content),
      length: post.content.length,
      timestamp: new Date().toISOString(),
      imagePath: post.imagePath
    };
    
    this.history.posts.push(historyEntry);
    
    // Update brand usage stats
    if (!this.history.brands[post.brand]) {
      this.history.brands[post.brand] = { count: 0, lastUsed: null };
    }
    this.history.brands[post.brand].count++;
    this.history.brands[post.brand].lastUsed = historyEntry.timestamp;
    
    // Update size usage stats
    if (post.size) {
      if (!this.history.sizes[post.size]) {
        this.history.sizes[post.size] = { count: 0, lastUsed: null };
      }
      this.history.sizes[post.size].count++;
      this.history.sizes[post.size].lastUsed = historyEntry.timestamp;
    }
    
    // Update content type stats
    const type = post.contentType || 'single';
    if (!this.history.contentTypes[type]) {
      this.history.contentTypes[type] = 0;
    }
    this.history.contentTypes[type]++;
    
    this.history.lastGenerated = historyEntry.timestamp;
    
    // Keep only last 100 posts to prevent file bloat
    if (this.history.posts.length > 100) {
      this.history.posts = this.history.posts.slice(-100);
    }
    
    await this.save();
    console.log(`✅ Post added to history: ${post.brand} ${post.size}`);
  }
  
  /**
   * Get least used brands (for variety)
   */
  async getLeastUsedBrands(count = 5) {
    if (!this.history) await this.load();
    
    const brandStats = Object.entries(this.history.brands)
      .map(([brand, stats]) => ({ brand, ...stats }))
      .sort((a, b) => a.count - b.count);
    
    return brandStats.slice(0, count).map(b => b.brand);
  }
  
  /**
   * Get content type to use next (for balance)
   */
  async getNextContentType() {
    if (!this.history) await this.load();
    
  const types = ['single', 'brand-showcase', 'variety', 'size-comparison', 'educational'];
    const counts = types.map(type => ({
      type,
      count: this.history.contentTypes[type] || 0
    }));
    
    // Sort by least used
    counts.sort((a, b) => a.count - b.count);
    
    // Return one of the 3 least used (random for variety)
    const leastUsed = counts.slice(0, 3);
    return leastUsed[Math.floor(Math.random() * leastUsed.length)].type;
  }
  
  /**
   * Get statistics
   */
  async getStats() {
    if (!this.history) await this.load();
    
    return {
      totalPosts: this.history.posts.length,
      uniqueBrands: Object.keys(this.history.brands).length,
      uniqueSizes: Object.keys(this.history.sizes).length,
      contentTypes: this.history.contentTypes,
      lastGenerated: this.history.lastGenerated,
      recentBrands: this.history.posts.slice(-10).map(p => p.brand)
    };
  }
}

// Singleton
let instance = null;

function getPostHistory() {
  if (!instance) {
    instance = new PostHistory();
  }
  return instance;
}

module.exports = { getPostHistory, PostHistory };
