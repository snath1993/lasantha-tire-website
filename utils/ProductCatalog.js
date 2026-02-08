/**
 * Product Catalog Loader
 * Loads product data from extracted catalog file
 * Provides random product selection for post generation
 */

const fs = require('fs');
const path = require('path');
const { normalizeBrand } = require('./BrandAliases');
const preferredBrands = (() => {
    try {
        const p = path.join(__dirname, '..', 'preferred-brands.json');
        if (fs.existsSync(p)) {
            return JSON.parse(fs.readFileSync(p, 'utf8'));
        }
    } catch {}
    return [];
})();

class ProductCatalog {
    constructor() {
        this.catalog = null;
        this.catalogPath = path.join(__dirname, '..', 'product-catalog.json');
        this.loadCatalog();
    }

    /**
     * Load catalog from file
     */
    loadCatalog() {
        try {
            if (fs.existsSync(this.catalogPath)) {
                const data = fs.readFileSync(this.catalogPath, 'utf8');
                const rawCatalog = JSON.parse(data);
                
                // Transform new format to expected format
                // New format has: { totalProducts, products: [...], categories: {...} }
                const allProducts = rawCatalog.products || [];
                
                // Extract brands and sizes from products (normalized)
                const brands = new Set();
                const sizes = new Set();
                const byBrand = {};
                
                allProducts.forEach(product => {
                    // Extract brand from ItemDescription or Custom3
                    const rawBrand = product.Custom3 || this.extractBrandFromDescription(product.ItemDescription);
                    const brand = normalizeBrand(rawBrand);
                    if (brand) {
                        brands.add(brand);
                        if (!byBrand[brand]) byBrand[brand] = [];
                        byBrand[brand].push({
                            code: product.ItemID,
                            brand: brand,
                            size: product.Custom1 || this.extractSizeFromDescription(product.ItemDescription),
                            description: product.ItemDescription,
                            fullText: product.ItemDescription,
                            category: product.Categoty,
                            itemClass: product.ItemClass
                        });
                    }
                    
                    // Extract size
                    const size = product.Custom1 || this.extractSizeFromDescription(product.ItemDescription);
                    if (size) sizes.add(size);
                });
                
                // Remove invalid placeholder brands if any remained
                const brandList = Array.from(brands).filter(b => b && b !== '-' && b !== '_');

                this.catalog = {
                    totalProducts: allProducts.length,
                    uniqueBrands: brandList.length,
                    uniqueSizes: sizes.size,
                    extractedAt: rawCatalog.extractedAt,
                    brands: brandList,
                    sizes: Array.from(sizes),
                    byBrand: byBrand,
                    products: allProducts.map(p => ({
                        code: p.ItemID,
                        brand: normalizeBrand(p.Custom3 || this.extractBrandFromDescription(p.ItemDescription)),
                        size: p.Custom1 || this.extractSizeFromDescription(p.ItemDescription),
                        description: p.ItemDescription,
                        fullText: p.ItemDescription,
                        category: p.Categoty,
                        itemClass: p.ItemClass
                    }))
                };
                
                console.log(`✅ Loaded product catalog: ${this.catalog.totalProducts} products, ${this.catalog.uniqueBrands} brands`);
            } else {
                console.warn('⚠️  Product catalog not found. Run: node scripts/extractProductCatalog.js');
                this.catalog = null;
            }
        } catch (error) {
            console.error('❌ Error loading product catalog:', error.message);
            this.catalog = null;
        }
    }
    
    /**
     * Extract brand from description text
     */
    extractBrandFromDescription(description) {
        if (!description) return null;
        // Match brand names at start: "BRIDGESTONE ...", "MICHELIN ...", etc
        const brandMatch = description.match(/^([A-Z][A-Z\s&]+?)(?:\s+\d|$)/);
        return brandMatch ? brandMatch[1].trim() : null;
    }
    
    /**
     * Extract size from description text
     */
    extractSizeFromDescription(description) {
        if (!description) return null;
        // Match tyre sizes: 195/65R15, 205/55R16, etc
        const sizeMatch = description.match(/(\d{3}\/\d{2}R?\d{2}\.?\d?)/i) || description.match(/(\d{2,3}-\d{2})/);
        return sizeMatch ? sizeMatch[1] : null;
    }

    /**
     * Check if catalog is available
     */
    isAvailable() {
        return this.catalog !== null && this.catalog.products.length > 0;
    }

    /**
     * Return preferred brands that actually exist in catalog (preserve order from config)
     */
    getAvailablePreferredBrands() {
        if (!this.isAvailable() || !preferredBrands.length) return [];
        const set = new Set(this.catalog.brands);
        return preferredBrands
            .map(b => normalizeBrand(b))
            .filter(b => b && set.has(b));
    }

    /**
     * Pick a random element from an array
     */
    pick(arr) {
        return arr[Math.floor(Math.random() * arr.length)];
    }

    /**
     * Get random product
     */
    getRandomProduct() {
        if (!this.isAvailable()) {
            return null;
        }

        // 70% bias to preferred brands (if available)
        const availPreferred = this.getAvailablePreferredBrands();
        const usePreferred = availPreferred.length > 0 && Math.random() < 0.7;
        if (usePreferred) {
            const brand = this.pick(availPreferred);
            const byBrand = this.getProductsByBrand(brand);
            if (byBrand.length) return this.pick(byBrand);
        }
        // Fallback to any product
        return this.pick(this.catalog.products);
    }

    /**
     * Get random products (multiple)
     */
    getRandomProducts(count = 3) {
        if (!this.isAvailable()) {
            return [];
        }

        const selected = [];
        const usedCodes = new Set();
        const availPreferred = this.getAvailablePreferredBrands();
        const byBrand = this.catalog.byBrand;

        // Try to pull at least half from preferred brands when possible
        const targetPreferred = Math.min(
            Math.ceil(count / 2),
            availPreferred.reduce((sum, b) => sum + (byBrand[b]?.length || 0), 0) > 0 ? Math.ceil(count / 2) : 0
        );

        let addedPreferred = 0;
        while (addedPreferred < targetPreferred && availPreferred.length) {
            const brand = this.pick(availPreferred);
            const pool = (byBrand[brand] || []).filter(p => !usedCodes.has(p.code));
            if (pool.length === 0) break;
            const p = this.pick(pool);
            selected.push(p);
            usedCodes.add(p.code);
            addedPreferred++;
        }

        // Fill remaining with any products
        const poolAny = this.catalog.products.filter(p => !usedCodes.has(p.code));
        while (selected.length < Math.min(count, this.catalog.products.length) && poolAny.length) {
            const p = this.pick(poolAny);
            selected.push(p);
            usedCodes.add(p.code);
        }

        return selected;
    }

    /**
     * Get random brand
     */
    getRandomBrand() {
        if (!this.isAvailable() || this.catalog.brands.length === 0) {
            return null;
        }

        const availPreferred = this.getAvailablePreferredBrands();
        if (availPreferred.length && Math.random() < 0.75) {
            return this.pick(availPreferred);
        }
        return this.pick(this.catalog.brands);
    }

    /**
     * Get products by brand
     */
    getProductsByBrand(brand) {
        if (!this.isAvailable() || !this.catalog.byBrand[brand]) {
            return [];
        }

        return this.catalog.byBrand[brand];
    }

    /**
     * Get random product from specific brand
     */
    getRandomProductFromBrand(brand) {
        const products = this.getProductsByBrand(brand);
        if (products.length === 0) {
            return null;
        }

        return products[Math.floor(Math.random() * products.length)];
    }

    /**
     * Get all brands
     */
    getAllBrands() {
        if (!this.isAvailable()) {
            return [];
        }

        return this.catalog.brands;
    }

    /**
     * Get all sizes
     */
    getAllSizes() {
        if (!this.isAvailable()) {
            return [];
        }

        return this.catalog.sizes;
    }

    /**
     * Get random size
     */
    getRandomSize() {
        if (!this.isAvailable() || this.catalog.sizes.length === 0) {
            return null;
        }

        const sizes = this.catalog.sizes;
        return sizes[Math.floor(Math.random() * sizes.length)];
    }

    /**
     * Get products by size
     */
    getProductsBySize(size) {
        if (!this.isAvailable()) {
            return [];
        }

        return this.catalog.products.filter(p => p.size === size);
    }

    /**
     * Get catalog stats
     */
    getStats() {
        if (!this.isAvailable()) {
            return null;
        }

        return {
            totalProducts: this.catalog.totalProducts,
            uniqueBrands: this.catalog.uniqueBrands,
            uniqueSizes: this.catalog.uniqueSizes,
            extractedAt: this.catalog.extractedAt,
            brands: this.catalog.brands,
            topBrands: this.catalog.brands.slice(0, 10),
            commonSizes: this.catalog.sizes.slice(0, 10)
        };
    }

    /**
     * Format product for post (NO prices)
     */
    formatProductForPost(product) {
        if (!product) {
            return null;
        }

        return {
            brand: product.brand || 'Premium Brand',
            size: product.size || 'Various Sizes',
            description: product.fullText || product.description,
            code: product.code,
            // NO price, NO quantity - just brand, size, description
            displayName: product.brand && product.size 
                ? `${product.brand} ${product.size}`
                : product.description
        };
    }

    /**
     * Get creative post data (multiple products, mix brands/sizes)
     */
    getCreativePostData(type = 'single') {
        if (!this.isAvailable()) {
            return null;
        }

        switch (type) {
            case 'single':
                // Single product spotlight
                const product = this.getRandomProduct();
                return {
                    type: 'single',
                    products: [this.formatProductForPost(product)]
                };

            case 'brand-showcase':
                // Multiple products from same brand
                const brand = this.getRandomBrand();
                const brandProducts = this.getProductsByBrand(brand)
                    .slice(0, 3)
                    .map(p => this.formatProductForPost(p));
                return {
                    type: 'brand-showcase',
                    brand: brand,
                    products: brandProducts
                };

            case 'size-comparison':
                // Multiple products with same size, different brands
                const size = this.getRandomSize();
                const sizeProducts = this.getProductsBySize(size)
                    .slice(0, 3)
                    .map(p => this.formatProductForPost(p));
                return {
                    type: 'size-comparison',
                    size: size,
                    products: sizeProducts
                };

            case 'variety':
                // Mix of different brands and sizes
                const varietyProducts = this.getRandomProducts(3)
                    .map(p => this.formatProductForPost(p));
                return {
                    type: 'variety',
                    products: varietyProducts
                };

            default:
                return this.getCreativePostData('single');
        }
    }
}

// Singleton instance
let instance = null;

function getProductCatalog() {
    if (!instance) {
        instance = new ProductCatalog();
    }
    return instance;
}

module.exports = {
    ProductCatalog,
    getProductCatalog
};
