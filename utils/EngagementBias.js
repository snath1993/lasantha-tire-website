/**
 * Engagement-Driven Brand Selection
 * Tracks engagement metrics and biases brand selection toward recent high-performers
 */

const fs = require('fs');
const path = require('path');

class EngagementBias {
  constructor() {
    this.enableEngagementBias = String(process.env.ENABLE_ENGAGEMENT_BIAS || 'true').toLowerCase() === 'true';
    this.lookbackDays = Number(process.env.ENGAGEMENT_LOOKBACK_DAYS || 7);
    this.engagementPath = path.join(__dirname, '../engagement-history.json');
    this.engagement = null;
  }

  /**
   * Load engagement history
   */
  async load() {
    try {
      if (fs.existsSync(this.engagementPath)) {
        this.engagement = JSON.parse(fs.readFileSync(this.engagementPath, 'utf8'));
      } else {
        this.engagement = { entries: [], brandScores: {} };
      }
    } catch (err) {
      console.warn('⚠️  Could not load engagement history:', err.message);
      this.engagement = { entries: [], brandScores: {} };
    }
  }

  /**
   * Save engagement history
   */
  async save() {
    try {
      fs.writeFileSync(this.engagementPath, JSON.stringify(this.engagement, null, 2));
    } catch (err) {
      console.error('❌ Error saving engagement history:', err.message);
    }
  }

  /**
   * Record engagement for a published post
   * @param {Object} post - { brand, size, contentType, likes, comments, shares, reach }
   */
  async recordEngagement(post) {
    if (!this.enableEngagementBias) return;
    if (!this.engagement) await this.load();

    const entry = {
      brand: post.brand,
      size: post.size,
      contentType: post.contentType,
      engagement: {
        likes: post.likes || 0,
        comments: post.comments || 0,
        shares: post.shares || 0,
        reach: post.reach || 0
      },
      engagementScore: this.calculateScore(post),
      recordedAt: new Date().toISOString()
    };

    this.engagement.entries.push(entry);

    // Update brand scores
    if (!this.engagement.brandScores[post.brand]) {
      this.engagement.brandScores[post.brand] = [];
    }
    this.engagement.brandScores[post.brand].push(entry.engagementScore);

    // Keep only recent entries to prevent bloat
    if (this.engagement.entries.length > 200) {
      this.engagement.entries = this.engagement.entries.slice(-200);
    }

    await this.save();
    console.log(`📊 Recorded engagement for ${post.brand}: score ${entry.engagementScore.toFixed(1)}`);
  }

  /**
   * Calculate engagement score (simple weighted sum)
   * Weights: likes(1) + comments(3) + shares(5) + reach(0.1)
   */
  calculateScore(post) {
    const likes = post.likes || 0;
    const comments = post.comments || 0;
    const shares = post.shares || 0;
    const reach = post.reach || 0;

    return likes * 1 + comments * 3 + shares * 5 + reach * 0.1;
  }

  /**
   * Get brands sorted by recent engagement (past N days)
   */
  async getTopBrandsByEngagement(days = null) {
    if (!this.enableEngagementBias) return [];
    if (!this.engagement) await this.load();

    const lookback = days || this.lookbackDays;
    const cutoffTime = new Date(Date.now() - lookback * 24 * 60 * 60 * 1000).toISOString();

    // Filter recent entries
    const recentEntries = this.engagement.entries.filter(e => e.recordedAt >= cutoffTime);

    if (recentEntries.length === 0) {
      return [];
    }

    // Group by brand and calculate average score
    const brandStats = {};
    recentEntries.forEach(entry => {
      if (!brandStats[entry.brand]) {
        brandStats[entry.brand] = { scores: [], count: 0 };
      }
      brandStats[entry.brand].scores.push(entry.engagementScore);
      brandStats[entry.brand].count++;
    });

    // Calculate averages and sort
    const ranked = Object.entries(brandStats)
      .map(([brand, stats]) => ({
        brand,
        avgScore: stats.scores.reduce((a, b) => a + b, 0) / stats.scores.length,
        postCount: stats.count,
        totalScore: stats.scores.reduce((a, b) => a + b, 0)
      }))
      .sort((a, b) => b.avgScore - a.avgScore);

    return ranked;
  }

  /**
   * Get engagement boost factor for a brand
   * Returns multiplier to apply when selecting this brand (1.0 = normal, 2.0 = 2x boost)
   */
  async getBoostFactor(brand) {
    if (!this.enableEngagementBias) return 1.0;
    if (!this.engagement) await this.load();

    const topBrands = await this.getTopBrandsByEngagement();
    if (topBrands.length === 0) return 1.0;

    const rank = topBrands.findIndex(b => b.brand === brand);
    if (rank === -1) return 0.5; // Unknown brands get a small penalty

    // Boost top 3 brands more, rest less
    if (rank === 0) return 2.0; // 1st place: 2x boost
    if (rank === 1) return 1.5; // 2nd place: 1.5x boost
    if (rank === 2) return 1.2; // 3rd place: 1.2x boost
    return 1.0; // Others: normal
  }

  /**
   * Use engagement bias to pick a brand from a list
   * Applies boost factors and weighted random selection
   */
  async pickBrandByEngagement(brandList) {
    if (!this.enableEngagementBias || !brandList.length) {
      return brandList[Math.floor(Math.random() * brandList.length)];
    }

    if (!this.engagement) await this.load();

    // Calculate weights
    const weights = await Promise.all(
      brandList.map(async (brand) => ({
        brand,
        weight: await this.getBoostFactor(brand)
      }))
    );

    // Weighted random selection
    const totalWeight = weights.reduce((sum, w) => sum + w.weight, 0);
    let random = Math.random() * totalWeight;

    for (const w of weights) {
      random -= w.weight;
      if (random <= 0) {
        console.log(`✅ Engagement bias selected: ${w.brand} (boost: ${w.weight}x)`);
        return w.brand;
      }
    }

    // Fallback
    return brandList[0];
  }

  /**
   * Print engagement report
   */
  async printReport(days = null) {
    if (!this.engagement) await this.load();

    const topBrands = await this.getTopBrandsByEngagement(days || this.lookbackDays);

    if (topBrands.length === 0) {
      console.log('📊 No engagement data available yet.');
      return;
    }

    console.log(`\n📊 Engagement Report (past ${days || this.lookbackDays} days):`);
    console.log('═'.repeat(60));

    topBrands.slice(0, 10).forEach((b, idx) => {
      console.log(
        `${idx + 1}. ${b.brand.padEnd(15)} | Avg Score: ${b.avgScore.toFixed(1).padEnd(6)} | Posts: ${b.postCount}`
      );
    });

    console.log('═'.repeat(60));
    console.log(`Total entries tracked: ${this.engagement.entries.length}\n`);
  }
}

function getEngagementBias() {
  return new EngagementBias();
}

module.exports = { EngagementBias, getEngagementBias };
