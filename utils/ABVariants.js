/**
 * A/B Draft Variants Generator
 * Generates multiple copy variants for A/B testing
 * Saves all variants as separate drafts for manual selection
 */

const axios = require('axios');
const fs = require('fs');
const path = require('path');

class ABVariantGenerator {
  constructor() {
    this.enableAB = String(process.env.ENABLE_AB_DRAFTS || 'true').toLowerCase() === 'true';
    this.variantCount = Number(process.env.AB_VARIANT_COUNT || 2);
    this.draftDir = path.join(__dirname, '../facebook-drafts');
  }

  /**
   * Generate multiple copy variants using Claude
   * @param {Object} product - Product data
   * @param {Object} store - Store info
   * @param {string} styleGuide - Style guide text
   * @param {string} brandInfo - Brand knowledge
   * @returns {Array} Array of variant objects {hook, content, cta}
   */
  async generateVariants(product, store, styleGuide, brandInfo) {
    if (!this.enableAB || this.variantCount < 2) {
      return []; // Disabled or invalid count
    }

    if (!process.env.ANTHROPIC_API_KEY) {
      console.warn('⚠️  ANTHROPIC_API_KEY not set. Cannot generate A/B variants.');
      return [];
    }

    const variants = [];
    const hooks = [
      '🔥 HOT DEAL ALERT!',
      '⭐ PREMIUM CHOICE!',
      '💡 SMART CHOICE!',
      '🏆 BRAND SPOTLIGHT!',
      '✨ NEW STOCK ALERT!',
      '🛡️ SAFETY FIRST!'
    ];

    const ctas = [
      'Call now for instant service',
      'Visit us today and save',
      'Limited availability - secure yours now',
      'Order online or visit our store',
      'Get expert advice - contact us'
    ];

    try {
      // Generate 2 or more variants (respecting variantCount)
      for (let i = 0; i < Math.min(this.variantCount, 3); i++) {
        const hook = hooks[i % hooks.length];
        const cta = ctas[i % ctas.length];

        const variantPrompt = `
You are a social media copywriter for Lasantha Tyre Traders (${store.location}, Sri Lanka).

Generate a UNIQUE Facebook post variant ${i + 1} of ${this.variantCount}. This is variant ${i + 1} so use a DIFFERENT approach from any previous variants.

PRODUCT: ${product.brand} ${product.size} - ${product.description}

VARIANT ${i + 1} APPROACH:
- Hook: ${hook}
- CTA style: ${cta}
- Tone: ${['Bold & Direct', 'Educational & Friendly', 'Premium & Exclusive'][i % 3]}
- Focus: ${['Quality & Reliability', 'Customer Value', 'Safety & Trust'][i % 3]}

STYLE GUIDE (follow core style):
${styleGuide}

BRAND INFO:
${brandInfo ? `Origin: ${brandInfo.origin}\nTier: ${brandInfo.tier}\nTagline: "${brandInfo.tagline}"` : 'Premium tyre brand'}

CONSTRAINTS:
- NO prices or quantities
- Always include phone: ${store.phone}
- 3-8 hashtags (include #Thalawathugoda #LasanthaTyreTraders #${(product.brand || '').replace(/\s+/g, '')})
- Use varied emoji patterns (not same as other variants)
- 150-250 characters (concise for engagement)
- Keep Sinhala/English natural mix

GENERATE ONLY THE CAPTION (no explanations or meta-text):
`;

        try {
          const resp = await axios.post(
            'https://api.anthropic.com/v1/messages',
            {
              model: 'claude-haiku-4-5-20251001',
              max_tokens: 400,
              temperature: 0.8,
              messages: [{ role: 'user', content: variantPrompt }]
            },
            {
              headers: {
                'x-api-key': process.env.ANTHROPIC_API_KEY,
                'anthropic-version': '2023-06-01',
                'content-type': 'application/json'
              },
              timeout: 10000
            }
          );

          const content = resp.data?.content?.[0]?.text?.trim() || '';
          if (content) {
            variants.push({
              variantId: i + 1,
              hook,
              cta,
              tone: ['Bold & Direct', 'Educational & Friendly', 'Premium & Exclusive'][i % 3],
              focus: ['Quality & Reliability', 'Customer Value', 'Safety & Trust'][i % 3],
              content,
              generatedAt: new Date().toISOString()
            });
            console.log(`✅ Variant ${i + 1}/${this.variantCount} generated`);
          }
        } catch (e) {
          console.warn(`⚠️  Failed to generate variant ${i + 1}:`, e.message);
        }
      }

      return variants;
    } catch (err) {
      console.error('❌ Error in generateVariants:', err.message);
      return [];
    }
  }

  /**
   * Save variants as separate draft files
   * @param {Array} variants - Array of variant objects
   * @param {string} productBrand - Product brand
   * @param {Object} imageBuffer - Image buffer to save
   * @returns {Array} Array of file paths saved
   */
  async saveDrafts(variants, productBrand, imageBuffer = null) {
    if (!variants.length) {
      console.warn('⚠️  No variants to save');
      return [];
    }

    const timestamp = new Date().toISOString().replace(/[-:T]/g, '').slice(0, 15);
    const baseFilename = `ab_${productBrand}_${timestamp}`;
    const savedPaths = [];

    try {
      // Ensure drafts directory exists
      if (!fs.existsSync(this.draftDir)) {
        fs.mkdirSync(this.draftDir, { recursive: true });
      }

      // Save each variant as a separate JSON
      variants.forEach((variant, idx) => {
        const variantFilename = `${baseFilename}_v${variant.variantId}.json`;
        const filePath = path.join(this.draftDir, variantFilename);

        const draftData = {
          variantId: variant.variantId,
          productBrand,
          timestamp: new Date().toISOString(),
          hook: variant.hook,
          tone: variant.tone,
          focus: variant.focus,
          content: variant.content,
          cta: variant.cta,
          metadata: {
            status: 'draft',
            type: 'ab-variant',
            readyForPublish: true
          }
        };

        fs.writeFileSync(filePath, JSON.stringify(draftData, null, 2));
        savedPaths.push(filePath);
        console.log(`💾 Saved: ${variantFilename}`);
      });

      // Save a comparison JSON to help with manual selection
      const comparisonPath = path.join(this.draftDir, `${baseFilename}_comparison.json`);
      const comparison = {
        productBrand,
        variantCount: variants.length,
        timestamp: new Date().toISOString(),
        variants: variants.map(v => ({
          id: v.variantId,
          tone: v.tone,
          focus: v.focus,
          preview: v.content.substring(0, 100) + '...'
        })),
        instructions: 'Pick your favorite variant and publish from facebook-drafts. Update comparison.json with winner.'
      };
      fs.writeFileSync(comparisonPath, JSON.stringify(comparison, null, 2));
      savedPaths.push(comparisonPath);
      console.log(`📊 Comparison saved: ${comparisonPath.split(path.sep).pop()}`);

      // Save image if provided (used by both variants)
      if (imageBuffer) {
        const imagePath = path.join(this.draftDir, `${baseFilename}.jpg`);
        fs.writeFileSync(imagePath, imageBuffer);
        savedPaths.push(imagePath);
        console.log(`🖼️  Image saved: ${imagePath.split(path.sep).pop()}`);
      }

      return savedPaths;
    } catch (err) {
      console.error('❌ Error saving drafts:', err.message);
      return [];
    }
  }

  /**
   * Print variant comparison
   */
  printVariants(variants) {
    if (!variants.length) {
      console.log('⚠️  No variants to display');
      return;
    }

    console.log('\n📊 A/B Variant Comparison:');
    console.log('═'.repeat(70));

    variants.forEach((v, idx) => {
      console.log(`\n📌 Variant ${v.variantId}:`);
      console.log(`   Tone: ${v.tone}`);
      console.log(`   Focus: ${v.focus}`);
      console.log(`   Hook: ${v.hook}`);
      console.log(`   Preview: ${v.content.substring(0, 80)}...`);
      console.log('─'.repeat(70));
    });

    console.log('\n💡 TIP: Choose the variant that resonates best with your audience.');
    console.log('   Publish one, then track engagement to inform future picks.\n');
  }
}

function getABVariantGenerator() {
  return new ABVariantGenerator();
}

module.exports = { ABVariantGenerator, getABVariantGenerator };
