/**
 * Caption Quality Assurance Linter
 * Auto-validates and fixes generated posts to ensure production-ready output
 * Checks: emoji count, hashtag count, phone, prices, spacing, formatting
 */

class CaptionLinter {
  constructor(config = {}) {
    this.minEmojis = Number(process.env.MIN_EMOJIS || 3);
    this.maxEmojis = Number(process.env.MAX_EMOJIS || 8);
    this.minHashtags = Number(process.env.MIN_HASHTAGS || 3);
    this.maxHashtags = Number(process.env.MAX_HASHTAGS || 8);
    this.storePhone = process.env.STORE_PHONE || '0721222509';
    this.enableQALinter = String(process.env.ENABLE_QA_LINTER || 'true').toLowerCase() === 'true';
  }

  /**
   * Count emojis in text
   */
  countEmojis(text) {
    if (!text) return 0;
    // Unicode emoji ranges
    const emojiPattern = /[\u{1F300}-\u{1F9FF}]|[\u{2600}-\u{26FF}]|[\u{2700}-\u{27BF}]|[\u{1F600}-\u{1F64F}]|[\u{1F900}-\u{1F9FF}]/gu;
    const matches = text.match(emojiPattern);
    return matches ? matches.length : 0;
  }

  /**
   * Count hashtags
   */
  countHashtags(text) {
    if (!text) return 0;
    const hashtags = text.match(/#\w+/g);
    return hashtags ? hashtags.length : 0;
  }

  /**
   * Get all hashtags
   */
  getHashtags(text) {
    if (!text) return [];
    const hashtags = text.match(/#\w+/g);
    return hashtags || [];
  }

  /**
   * Check if phone number is present
   */
  hasPhoneNumber(text) {
    if (!text) return false;
    // Normalize and check for phone
    const normalized = text.replace(/\D/g, '');
    const phoneNormalized = this.storePhone.replace(/\D/g, '');
    return normalized.includes(phoneNormalized);
  }

  /**
   * Check for price mentions (Rs., රු., LKR, ₨, etc.)
   */
  hasPriceMentions(text) {
    if (!text) return false;
    const pricePattern = /(Rs\.?|රු\.?|LKR|₨)\s*\d[\d,\.]*|Rs\s*\d+|රු\s*\d+/gi;
    return pricePattern.test(text);
  }

  /**
   * Check for excessive spacing or formatting issues
   */
  hasSpacingIssues(text) {
    if (!text) return false;
    // Multiple blank lines
    if (/\n\n\n+/.test(text)) return true;
    // Trailing/leading spaces on lines
    const lines = text.split('\n');
    return lines.some(line => /^\s+|\s+$/.test(line));
  }

  /**
   * Validate and return issues
   */
  validate(caption) {
    if (!this.enableQALinter) {
      return { valid: true, issues: [], warnings: [] };
    }

    const issues = [];
    const warnings = [];

    // 1. Emoji count
    const emojiCount = this.countEmojis(caption);
    if (emojiCount < this.minEmojis) {
      issues.push(`⚠️  Emoji count: ${emojiCount} (min ${this.minEmojis})`);
    }
    if (emojiCount > this.maxEmojis) {
      warnings.push(`⚠️  Emoji count: ${emojiCount} (max ${this.maxEmojis})`);
    }

    // 2. Hashtag count
    const hashtagCount = this.countHashtags(caption);
    if (hashtagCount < this.minHashtags) {
      issues.push(`⚠️  Hashtags: ${hashtagCount} (min ${this.minHashtags})`);
    }
    if (hashtagCount > this.maxHashtags) {
      warnings.push(`⚠️  Hashtags: ${hashtagCount} (max ${this.maxHashtags})`);
    }

    // 3. Phone number
    if (!this.hasPhoneNumber(caption)) {
      issues.push(`❌ Phone number missing: ${this.storePhone}`);
    }

    // 4. Price mentions
    if (this.hasPriceMentions(caption)) {
      issues.push(`❌ Price mentions detected (must be removed)`);
    }

    // 5. Spacing issues
    if (this.hasSpacingIssues(caption)) {
      warnings.push(`⚠️  Spacing issues detected (excessive blank lines or indentation)`);
    }

    // 6. Mandatory hashtags
    const hashtags = this.getHashtags(caption).map(h => h.toLowerCase());
    const mandatoryTags = ['#thalawathugoda', '#lasanthatyretraders'];
    mandatoryTags.forEach(tag => {
      if (!hashtags.includes(tag)) {
        issues.push(`❌ Missing mandatory hashtag: ${tag}`);
      }
    });

    const valid = issues.length === 0;
    return { valid, issues, warnings, emojiCount, hashtagCount };
  }

  /**
   * Auto-fix issues where possible
   */
  autoFix(caption) {
    if (!this.enableQALinter) return caption;

    let fixed = caption;

    // 1. Remove price mentions
    fixed = fixed.replace(/(Rs\.?|රු\.?|LKR|₨)\s*\d[\d,\.]*/gi, '');
    fixed = fixed.replace(/Rs\s+\d+/gi, '');
    fixed = fixed.replace(/රු\s+\d+/gi, '');

    // 2. Clean up extra blank lines
    fixed = fixed.replace(/\n\n\n+/g, '\n\n');

    // 3. Trim trailing/leading spaces on each line
    fixed = fixed
      .split('\n')
      .map(line => line.trim())
      .join('\n');

    // 4. Ensure emoji count is at least minimum
    const emojiCount = this.countEmojis(fixed);
    if (emojiCount < this.minEmojis) {
      const emojis = ['🛞', '🚗', '🏆', '⭐', '✅'];
      const needed = this.minEmojis - emojiCount;
      for (let i = 0; i < needed; i++) {
        const emoji = emojis[i % emojis.length];
        fixed += ` ${emoji}`;
      }
    }

    // 5. Ensure hashtags at minimum (add generic ones if needed)
    const hashtagCount = this.countHashtags(fixed);
    if (hashtagCount < this.minHashtags) {
      const generic = ['#Tyres', '#TyreShop', '#SriLanka', '#CarCare', '#Safety'];
      const needed = this.minHashtags - hashtagCount;
      let added = 0;
      for (const tag of generic) {
        if (added >= needed) break;
        if (!fixed.toLowerCase().includes(tag.toLowerCase())) {
          fixed += ` ${tag}`;
          added++;
        }
      }
    }

    return fixed;
  }

  /**
   * Full validation + auto-fix report
   */
  lint(caption) {
    const validation = this.validate(caption);
    const fixed = this.autoFix(caption);
    const revalidation = this.validate(fixed);

    return {
      original: {
        ...validation,
        length: caption.length
      },
      fixed: {
        ...revalidation,
        length: fixed.length
      },
      caption: fixed,
      issues: validation.issues,
      warnings: validation.warnings,
      needsFix: validation.issues.length > 0
    };
  }

  /**
   * Print linter report
   */
  printReport(lintResult) {
    console.log('\n📋 Caption QA Linter Report:');
    console.log('─'.repeat(50));

    if (lintResult.issues.length === 0 && lintResult.warnings.length === 0) {
      console.log('✅ PASS - All checks passed!');
    } else {
      if (lintResult.issues.length > 0) {
        console.log('❌ ISSUES (must fix):');
        lintResult.issues.forEach(i => console.log(`  ${i}`));
      }
      if (lintResult.warnings.length > 0) {
        console.log('⚠️  WARNINGS (review):');
        lintResult.warnings.forEach(w => console.log(`  ${w}`));
      }
    }

    if (lintResult.needsFix) {
      console.log('\n🔧 Auto-fixed version applied.');
      console.log(`Length: ${lintResult.original.length} → ${lintResult.fixed.length}`);
    }

    console.log('─'.repeat(50));
  }
}

function getCaptionLinter() {
  return new CaptionLinter();
}

module.exports = { CaptionLinter, getCaptionLinter };
