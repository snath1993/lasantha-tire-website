// Reusable social copy templates for high-converting Facebook posts
// Non-copyrighted, original structures (PAS, AIDA, FOMO, Testimonial, Listicle, Tips, WeekendOffer, NewArrival)

function getTemplates() {
  return [
    {
      id: 'AIDA',
      name: 'Attention-Interest-Desire-Action',
      instructions: `Use AIDA. 1) Big hook, 2) build interest, 3) convert desire with benefits, 4) clear CTA. Short lines, tasteful emojis, Singlish (Sinhala+English mix).`,
      skeleton: [
        '{HOOK}',
        '• Benefit 1',
        '• Benefit 2',
        '• Benefit 3',
        'Price: Rs. {todayPrice} (Reg. {regularPrice})',
  'Call: {phone}',
      ]
    },
    {
      id: 'PAS',
      name: 'Problem-Agitate-Solution',
      instructions: `Use PAS. Identify a real driver pain (fuel, grip, comfort), agitate briefly, then present tyre as solution. Keep under 140 words.`,
      skeleton: [
        '{PROBLEM_LINE}',
        '{AGITATE_LINE}',
        '{SOLUTION_LINE}',
        'Size: {size} • Brand: {brand}',
        'Today: Rs. {todayPrice}',
  '☎️ {phone}'
      ]
    },
    {
      id: 'FOMO',
      name: 'Limited Offer / FOMO',
      instructions: `Create urgency credibly (limited stock/limited time). Never lie. Short punchy lines, 2–4 emojis max.`,
      skeleton: [
        '🔥 Limited Stock Alert',
        '{brand} {product} {size}',
        'Now: Rs. {todayPrice} (Reg. {regularPrice})',
        'Only {stock}+ units left! ⏳',
  'Call now: {phone}'
      ]
    },
    {
      id: 'TESTIMONIAL',
      name: 'Happy Customer',
      instructions: `Write as a light testimonial vibe (generic, not fabricated). Keep ethical. Emphasize comfort/safety.`,
      skeleton: [
        'Happy Customer Review ⭐',
        '“Ride eka smooth. Grip eka superb!”',
        '{brand} {product} {size}',
        'Rs. {todayPrice}',
  '☎️ {phone}'
      ]
    },
    {
      id: 'LISTICLE',
      name: 'Top 3 Benefits',
      instructions: `List top 3 benefits for Sri Lankan roads (rain grip, fuel save, durability). Keep lines short.`,
      skeleton: [
        '{brand} {product} {size}',
        '1) Rain grip ✅',
        '2) Fuel savings ⛽',
        '3) Long life 💪',
        'Today: Rs. {todayPrice}',
  'Call: {phone}'
      ]
    },
    {
      id: 'TIPS',
      name: 'Care Tip + Offer',
      instructions: `Lead with a tyre care tip, then smoothly present the offer. Helpful first, then sell.`,
      skeleton: [
        '💡 Tip: Check air pressure monthly',
        'Better fuel • Longer life',
        '',
        '{brand} {product} {size}',
        'Offer: Rs. {todayPrice}',
  '☎️ {phone}'
      ]
    },
    {
      id: 'WEEKEND',
      name: 'Weekend Offer',
      instructions: `Weekend tone. Friendly + family vibe. Include store call-to-visit if relevant.`,
      skeleton: [
        '🎊 Weekend Special 🎊',
        '{brand} {product} {size}',
        'Today: Rs. {todayPrice}',
  'Visit or Call: {phone}'
      ]
    },
    {
      id: 'NEW',
      name: 'New Arrival',
      instructions: `Announce new arrival with excitement but remain honest. Keep under 120 words.`,
      skeleton: [
        '🆕 New Arrival',
        '{brand} {product} {size}',
        'Intro Price: Rs. {todayPrice}',
  '☎️ {phone}'
      ]
    }
  ];
}

module.exports = { getTemplates };
