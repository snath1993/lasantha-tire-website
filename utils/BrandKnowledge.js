/**
 * Brand Knowledge Loader
 * Loads brand information, tread patterns, features for post generation
 */

const fs = require('fs');
const path = require('path');

class BrandKnowledge {
  constructor() {
    this.knowledgePath = path.join(__dirname, '..', 'brand-knowledge-base.json');
    this.knowledge = null;
  }
  
  load() {
    try {
      if (!fs.existsSync(this.knowledgePath)) {
        console.warn('⚠️  brand-knowledge-base.json not found. Run: node scripts/generateBrandKnowledge.js');
        return false;
      }
      
      const data = fs.readFileSync(this.knowledgePath, 'utf8');
      this.knowledge = JSON.parse(data);
      console.log(`✅ Loaded brand knowledge: ${this.knowledge.totalBrands} brands`);
      return true;
    } catch (error) {
      console.error('❌ Failed to load brand knowledge:', error.message);
      return false;
    }
  }
  
  isAvailable() {
    if (!this.knowledge) {
      return this.load();
    }
    return true;
  }
  
  /**
   * Get brand information
   * @param {string} brandName - Brand name (e.g., "BRIDGESTONE", "MICHELIN")
   * @returns {Object|null} Brand data
   */
  getBrandInfo(brandName) {
    if (!this.isAvailable()) {
      return null;
    }
    
    // Normalize brand name
    const normalized = brandName.toUpperCase().trim();
    
    // Direct match
    if (this.knowledge.brands[normalized]) {
      return this.knowledge.brands[normalized];
    }
    
    // Fuzzy match (partial)
    const brandKeys = Object.keys(this.knowledge.brands);
    const match = brandKeys.find(key => 
      key.includes(normalized) || normalized.includes(key)
    );
    
    return match ? this.knowledge.brands[match] : null;
  }
  
  /**
   * Get brand features for post
   * @param {string} brandName 
   * @returns {Array} Features array
   */
  getBrandFeatures(brandName) {
    const brand = this.getBrandInfo(brandName);
    return brand ? brand.features : [];
  }
  
  /**
   * Get brand tagline
   * @param {string} brandName 
   * @returns {string} Tagline
   */
  getBrandTagline(brandName) {
    const brand = this.getBrandInfo(brandName);
    return brand ? brand.tagline : '';
  }
  
  /**
   * Get popular models
   * @param {string} brandName 
   * @returns {Array} Models array
   */
  getPopularModels(brandName) {
    const brand = this.getBrandInfo(brandName);
    return brand ? brand.popularModels : [];
  }
  
  /**
   * Get brand tier (Premium, Mid-Premium, etc)
   * @param {string} brandName 
   * @returns {string} Tier
   */
  getBrandTier(brandName) {
    const brand = this.getBrandInfo(brandName);
    return brand ? brand.tier : 'Standard';
  }
  
  /**
   * Get random marketing point by category
   * @param {string} category - safety, comfort, performance, durability, ecoFriendly, value
   * @returns {string} Marketing point
   */
  getMarketingPoint(category = 'safety') {
    if (!this.isAvailable()) {
      return '';
    }
    
    const points = this.knowledge.marketingPoints[category] || [];
    return points.length > 0 ? points[Math.floor(Math.random() * points.length)] : '';
  }
  
  /**
   * Get tread pattern description
   * @param {string} patternType - asymmetric, directional, symmetric, allTerrain, touring
   * @returns {string} Description
   */
  getTreadPatternDescription(patternType) {
    if (!this.isAvailable()) {
      return '';
    }
    
    return this.knowledge.treadPatterns[patternType] || '';
  }
  
  /**
   * Get enhanced product data with brand knowledge
   * @param {Object} product - Product from catalog
   * @returns {Object} Enhanced product data
   */
  enhanceProductWithBrandKnowledge(product) {
    if (!product || !product.brand) {
      return product;
    }
    
    const brandInfo = this.getBrandInfo(product.brand);
    
    if (!brandInfo) {
      return product;
    }
    
    return {
      ...product,
      brandKnowledge: {
        origin: brandInfo.origin,
        tier: brandInfo.tier,
        tagline: brandInfo.tagline,
        description: brandInfo.description,
        features: brandInfo.features.slice(0, 3), // Top 3 features
        strengths: brandInfo.strengths,
        popularModels: brandInfo.popularModels.map(m => m.model),
        idealFor: brandInfo.idealFor
      }
    };
  }
  
  /**
   * Get brand categories
   * @returns {Object} Categories
   */
  getCategories() {
    if (!this.isAvailable()) {
      return {};
    }
    return this.knowledge.categories;
  }
  
  /**
   * Check if brand is premium
   * @param {string} brandName 
   * @returns {boolean}
   */
  isPremiumBrand(brandName) {
    if (!this.isAvailable()) {
      return false;
    }
    
    const normalized = brandName.toUpperCase().trim();
    const categories = this.knowledge.categories;
    
    return categories.ultraPremium.includes(normalized) || 
           categories.premium.includes(normalized);
  }
}

// Singleton
let instance = null;

function getBrandKnowledge() {
  if (!instance) {
    instance = new BrandKnowledge();
  }
  return instance;
}

module.exports = { getBrandKnowledge, BrandKnowledge };
