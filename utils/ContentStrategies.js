/**
 * Creative Content Strategies
 * Multiple content types and variations for diverse posts
 */

const CONTENT_STRATEGIES = {
  // 1. Single Product Spotlight
  single: {
    name: 'Single Product Spotlight',
    templates: [
      {
        hook: 'NEW ARRIVAL! 🛞',
        structure: 'Hook → Brand Story → Product → Features → CTA',
        focus: 'Brand heritage and quality'
      },
      {
        hook: '🔥 HOT DEAL ALERT!',
        structure: 'Hook → Problem → Solution → Features → Urgency → CTA',
        focus: 'Problem-solving approach'
      },
      {
        hook: '⭐ PREMIUM CHOICE!',
        structure: 'Hook → Why Premium → Product → Benefits → Trust → CTA',
        focus: 'Premium positioning'
      },
      {
        hook: '💡 SMART CHOICE!',
        structure: 'Hook → Smart Tip → Product → Value Prop → CTA',
        focus: 'Educational + value'
      }
    ]
  },

  // 2. Brand Showcase
  'brand-showcase': {
    name: 'Brand Showcase',
    templates: [
      {
        hook: '🏆 PREMIUM BRAND SPOTLIGHT!',
        structure: 'Hook → Brand History → Product Range → Why Choose → CTA',
        focus: 'Brand authority'
      },
      {
        hook: '✨ DISCOVER {BRAND}!',
        structure: 'Hook → Brand Values → Technology → Product Options → CTA',
        focus: 'Brand innovation'
      },
      {
        hook: '🌟 TRUSTED GLOBALLY!',
        structure: 'Hook → Global Recognition → Local Availability → Products → CTA',
        focus: 'Trust and availability'
      }
    ]
  },

  // 3. Product Variety/Comparison
  variety: {
    name: 'Product Variety',
    templates: [
      {
        hook: '🎯 MULTIPLE OPTIONS AVAILABLE!',
        structure: 'Hook → Different Needs → Product Options → Match Your Need → CTA',
        focus: 'Choice and versatility'
      },
      {
        hook: '📋 COMPLETE RANGE!',
        structure: 'Hook → Product List → Features Overview → Stock Status → CTA',
        focus: 'Comprehensive inventory'
      },
      {
        hook: '💪 WE HAVE IT ALL!',
        structure: 'Hook → Customer Types → Matching Products → Why Us → CTA',
        focus: 'Customer-centric'
      }
    ]
  },

  // 4. Comparison Posts
  comparison: {
    name: 'Smart Comparison',
    templates: [
      {
        hook: '🤔 WHICH ONE TO CHOOSE?',
        structure: 'Hook → Options → Use Cases → Recommendations → CTA',
        focus: 'Helping decision'
      },
      {
        hook: '⚖️ COMPARE & DECIDE!',
        structure: 'Hook → Feature Comparison → Best For Each → Expert Advice → CTA',
        focus: 'Expert guidance'
      }
    ]
  },

  // 5. Educational Content
  educational: {
    name: 'Educational Post',
    templates: [
      {
        hook: '📚 TYRE KNOWLEDGE!',
        structure: 'Hook → Did You Know → Product Example → Tips → CTA',
        focus: 'Education + product'
      },
      {
        hook: '💡 PRO TIP!',
        structure: 'Hook → Expert Advice → Why It Matters → Our Solution → CTA',
        focus: 'Expert positioning'
      },
      {
        hook: '🔧 MAINTENANCE TIPS!',
        structure: 'Hook → Care Tips → Product Quality → Longevity → CTA',
        focus: 'Customer care'
      }
    ]
  },

  // 6. Seasonal/Timely
  seasonal: {
    name: 'Seasonal Content',
    templates: [
      {
        hook: '🌧️ MONSOON READY!',
        structure: 'Hook → Weather Challenge → Wet Grip Solution → Products → CTA',
        focus: 'Seasonal relevance'
      },
      {
        hook: '🎉 FESTIVAL SPECIAL!',
        structure: 'Hook → Festival Greetings → Travel Safety → Products → CTA',
        focus: 'Festival tie-in'
      },
      {
        hook: '🚗 ROAD TRIP SEASON!',
        structure: 'Hook → Journey Safety → Tyre Check → Products → CTA',
        focus: 'Travel safety'
      }
    ]
  },

  // 7. Customer-Centric
  'customer-story': {
    name: 'Customer-Centric',
    templates: [
      {
        hook: '👥 JOIN THOUSANDS OF HAPPY CUSTOMERS!',
        structure: 'Hook → Customer Trust → Why They Choose Us → Products → CTA',
        focus: 'Social proof'
      },
      {
        hook: '⭐ 5-STAR SERVICE!',
        structure: 'Hook → Service Quality → Expert Installation → Products → CTA',
        focus: 'Service excellence'
      }
    ]
  },

  // 8. Urgency/Scarcity
  urgency: {
    name: 'Urgency-Driven',
    templates: [
      {
        hook: '⏰ LIMITED TIME!',
        structure: 'Hook → Urgency → Product → Benefits → Act Now → CTA',
        focus: 'Time-sensitive'
      },
      {
        hook: '🔥 STOCK RUNNING LOW!',
        structure: 'Hook → Popularity → Stock Status → Secure Yours → CTA',
        focus: 'Scarcity'
      },
      {
        hook: '🎯 LAST CHANCE!',
        structure: 'Hook → Opportunity → Product → Don\'t Miss → CTA',
        focus: 'FOMO'
      }
    ]
  }
};

// Emoji variations for visual diversity
const EMOJI_SETS = {
  tyres: ['🛞', '🚗', '🚙', '🚕', '🏎️', '🚐'],
  attention: ['🔥', '⭐', '✨', '💫', '🌟', '💥'],
  check: ['✅', '✔️', '☑️', '🎯'],
  money: ['💰', '💵', '💸', '🤑'],
  safety: ['🛡️', '🔒', '✅', '💯'],
  quality: ['⭐', '🏆', '👑', '💎', '🥇'],
  time: ['⏰', '⏳', '🕐', '⌚'],
  phone: ['📞', '☎️', '📱', '📲'],
  location: ['📍', '🗺️', '📌', '🏪'],
  point: ['👉', '➡️', '▶️', '🔸']
};

// Opening hooks variety
const HOOKS = [
  // New arrivals
  'NEW STOCK ARRIVED! 🛞',
  '🎉 FRESH INVENTORY JUST IN!',
  '✨ LATEST ARRIVALS!',
  '🚨 NEW COLLECTION ALERT!',
  
  // Quality focus
  '🏆 PREMIUM QUALITY TYRES!',
  '⭐ TOP-TIER BRANDS AVAILABLE!',
  '💎 QUALITY YOU CAN TRUST!',
  '👑 THE BEST JUST ARRIVED!',
  
  // Safety focus
  '🛡️ SAFETY FIRST!',
  '✅ ROAD-TESTED QUALITY!',
  '💯 CERTIFIED SAFE!',
  '🔒 SECURE YOUR JOURNEY!',
  
  // Value focus
  '💰 BEST VALUE IN TOWN!',
  '🎯 SMART CHOICE, GREAT PRICE!',
  '💵 AFFORDABLE EXCELLENCE!',
  '🤝 UNBEATABLE DEALS!',
  
  // Urgency
  '⚡ LIMITED STOCK ALERT!',
  '🔥 HOT DEAL - ACT FAST!',
  '⏰ DON\'T MISS OUT!',
  '🚨 HURRY - SELLING FAST!',
  
  // Seasonal
  '🌧️ MONSOON-READY TYRES!',
  '☀️ SUMMER DRIVING ESSENTIALS!',
  '🛣️ ROAD TRIP READY!',
  '🎊 FESTIVAL SEASON SPECIALS!'
];

// Closing CTAs variety
const CTAS = [
  '📞 CALL NOW: {phone}\n🏪 VISIT: {location}\n⏰ HOURS: {hours}',
  '🚗 DRIVE IN TODAY!\n📞 CALL: {phone}\n📍 LOCATION: {location}',
  '💬 CONTACT US NOW!\n☎️ {phone}\n🗺️ {location}\n🕐 {hours}',
  '🎯 GET YOURS TODAY!\n📱 CALL: {phone}\n🏪 {location}\n⌚ OPEN: {hours}',
  '✅ BOOK YOUR TYRES!\n📞 PHONE: {phone}\n📌 AT: {location}\n⏰ TIMING: {hours}'
];

// Feature presentation styles
const FEATURE_STYLES = [
  'checkmarks', // ✅ Feature 1\n✅ Feature 2
  'bullets',    // • Feature 1\n• Feature 2
  'numbered',   // 1️⃣ Feature 1\n2️⃣ Feature 2
  'stars',      // ⭐ Feature 1\n⭐ Feature 2
  'arrows'      // ➡️ Feature 1\n➡️ Feature 2
];

// Educational tips (rotate these)
const EDUCATIONAL_TIPS = [
  '💡 TIP: Check tyre pressure monthly for better fuel efficiency!',
  '💡 DID YOU KNOW? Proper wheel alignment extends tyre life by 40%!',
  '💡 PRO TIP: Rotate tyres every 10,000 km for even wear!',
  '💡 SAFETY TIP: Replace tyres when tread depth is below 3mm!',
  '💡 EXPERT ADVICE: Check tyres before long journeys!',
  '💡 FUN FACT: Quality tyres improve fuel economy by up to 15%!',
  '💡 MAINTENANCE TIP: Avoid sudden braking to extend tyre life!',
  '💡 WISE CHOICE: Invest in quality tyres for family safety!'
];

function getRandomElement(array) {
  return array[Math.floor(Math.random() * array.length)];
}

function getRandomEmoji(category) {
  const emojis = EMOJI_SETS[category] || EMOJI_SETS.tyres;
  return getRandomElement(emojis);
}

function getContentStrategy(strategyType) {
  // Map aliases
  const key = strategyType === 'size-comparison' ? 'comparison' : strategyType;
  const strategy = CONTENT_STRATEGIES[key] || CONTENT_STRATEGIES.single;
  const template = getRandomElement(strategy.templates);
  
  return {
    ...strategy,
    template,
    hook: getRandomElement(HOOKS),
    cta: getRandomElement(CTAS),
    featureStyle: getRandomElement(FEATURE_STYLES),
    tip: getRandomElement(EDUCATIONAL_TIPS),
    emojis: {
      tyre: getRandomEmoji('tyres'),
      attention: getRandomEmoji('attention'),
      check: getRandomEmoji('check'),
      money: getRandomEmoji('money'),
      safety: getRandomEmoji('safety'),
      quality: getRandomEmoji('quality'),
      phone: getRandomEmoji('phone'),
      location: getRandomEmoji('location')
    }
  };
}

module.exports = {
  CONTENT_STRATEGIES,
  HOOKS,
  CTAS,
  EDUCATIONAL_TIPS,
  EMOJI_SETS,
  FEATURE_STYLES,
  getContentStrategy,
  getRandomElement,
  getRandomEmoji
};
