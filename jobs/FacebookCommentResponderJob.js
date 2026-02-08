// FacebookCommentResponderJob.js - Template-based smart responder (AI disabled)
const axios = require('axios');
const path = require('path');
const fs = require('fs');
const { classifyComment } = require('../utils/CommentClassifier');
const { getTyreOffer } = require('../utils/PriceProvider');
const { extractPhones } = require('../utils/PhoneExtractor');
const { getClient } = require('../utils/waClientRegistry');

const PAGE_ID = process.env.FACEBOOK_PAGE_ID;
const PAGE_TOKEN = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
const INTERVAL_SEC = Number(process.env.FB_COMMENT_SCAN_INTERVAL_SEC || 45);
const LANG = (process.env.FB_REPLY_LANGUAGE || 'si').toLowerCase();
const DM_WA = String(process.env.FB_DM_WHATSAPP_FALLBACK || 'true').toLowerCase() === 'true';

const STORE = {
  name: process.env.STORE_NAME || 'Lasantha Tyre Traders',
  loc: process.env.STORE_LOCATION || 'Thalawathugoda',
  phone: process.env.STORE_PHONE || '0721222509',
  hours: process.env.STORE_HOURS || '06:30-21:00',
  align: process.env.STORE_WHEEL_ALIGNMENT_HOURS || '07:30-18:00'
};

// Business Rules for Claude AI (මෙම rules Claude AI එකට දැනුම් දීම)
const BUSINESS_RULES = `
**LASANTHA TYRE TRADERS - BUSINESS RULES**
කරුණාකර මෙම rules අනිවාර්යයෙන් follow කරන්න:

1. **මිල ප්‍රසිද්ධ කිරීම තහනම්** (NO PUBLIC PRICES)
   - Public comments වල මිල mention කරන්න එපා
   - "විස්තර DM එකකට එවලා තිබේ" කියලා direct කරන්න
   
2. **Store Identity (වෙනස් කරන්න බැහැ)**
   - Name: ${STORE.name}
   - Location: ${STORE.loc}
   - Phone: ${STORE.phone}
   - Hours: ${STORE.hours}
   - Wheel Alignment: ${STORE.align}

3. **භාෂාව (Language)**
   - Customer සිංහල use කරොත් සිංහල reply
   - English use කරොත් English reply
   - Mixed භාෂාවත් natural විදිහට use කරන්න පුළුවන්

4. **Tone & Style**
   - Friendly & Professional
   - Customer කෙලින්ම වරදි කියන්න එපා
   - Solution-focused replies

5. **Vehicle Number Handling**
   - Vehicle number තියනවනම් invoice/history check කරන්න කියන්න
   - "ඔබේ vehicle එකේ details check කරලා DM එකකින් දෙන්නම්" style

6. **Emergency Contacts**
   - WhatsApp: ${STORE.phone}
   - Location: ${STORE.loc}
`;


const STATE_PATH = path.join(__dirname, '..', 'data', 'comment-history.json');
function loadState() { try { return JSON.parse(fs.readFileSync(STATE_PATH, 'utf8')); } catch { return { lastChecked: 0, handled: {}, analytics: [] }; } }
function saveState(s) { fs.mkdirSync(path.dirname(STATE_PATH), { recursive: true }); fs.writeFileSync(STATE_PATH, JSON.stringify(s, null, 2)); }

function t(key, ctx) {
  const text = {
    si: {
      publicNoPrice: `විස්තර DM එකකට එවලා තිබේ ✅ (Public එකේ මිල publish කරන්නේ නැහැ). WhatsApp: ${STORE.phone}`,
      dmBase: `Hello! 👋 ${STORE.name} (${STORE.loc})\nPublic එකේ මිල publish නොකරන නිසා DM එකට විස්තර දාලා යවන්නෙමු.`,
      dmOffer: (o) => `\n• Brand: ${o.brand}\n• Size: ${o.size}\n• Pattern: ${o.pattern}\n• Warranty: ${o.warranty || 'Standard'}\n• Availability: ${o.stockHint || 'In stock'}\n• Price (LKR): ${o.cashPrice?.toLocaleString('en-LK')}\n\nOrder/Info: ${STORE.phone}`,
      dmNoOffer: (c) => `\n• Brand/Size: ${[c.brand, c.size].filter(Boolean).join(' ')}\nමේ ටික confirm කරලා ඉක්මනින් price දෙන්නම්. WhatsApp එකට message එකක් දාන්න: ${STORE.phone}`,
      praise: `ස්තුතියි! 🙏 අවශ්‍ය කරුණු DM එකකට දාන්න.`,
      complaint: `කණගාටුයි 😔 කරුණාකර DM එකකට විස්තර එවන්න. ඉක්මනින් විසඳන්නම්. WhatsApp: ${STORE.phone}`,
      service: `අපි කරන සේවාවන්: Wheel Alignment (${STORE.align}), Balancing, Nitrogen, Rotation. DM/WhatsApp: ${STORE.phone}`,
      general: `ඔබගේ comment එකට ස්තුතියි! 😊 අවශ්‍ය brand/size එක DM කරන්න. WhatsApp: ${STORE.phone}`
    },
    en: {
      publicNoPrice: `Sent you a DM with details ✅ (We avoid posting prices publicly). WhatsApp: ${STORE.phone}`,
      dmBase: `Hello from ${STORE.name} (${STORE.loc}) 👋 We avoid posting prices publicly. Details below:`,
      dmOffer: (o) => `\n• Brand: ${o.brand}\n• Size: ${o.size}\n• Pattern: ${o.pattern}\n• Warranty: ${o.warranty || 'Standard'}\n• Availability: ${o.stockHint || 'In stock'}\n• Price (LKR): ${o.cashPrice?.toLocaleString('en-LK')}\n\nOrder/Info: ${STORE.phone}`,
      dmNoOffer: (c) => `\n• Brand/Size: ${[c.brand, c.size].filter(Boolean).join(' ')}\nWe’ll confirm stock & best price. Please WhatsApp ${STORE.phone} for a quick quote.`,
      praise: `Thank you! 🙏 DM us if you need any help.`,
      complaint: `Sorry about this 😔 Please DM your details—we’ll fix it ASAP. WhatsApp: ${STORE.phone}`,
      service: `We offer Wheel Alignment (${STORE.align}), Balancing, Nitrogen, Rotation. DM/WhatsApp: ${STORE.phone}`,
      general: `Thanks for your comment! 😊 DM us the tyre brand/size you need. WhatsApp: ${STORE.phone}`
    }
  };
  return text[LANG][key] || text.si[key];
}

// Advanced Template Reply System with WhatsApp Links
function getAdvancedTemplate(intent, ctx, language) {
  const waLink = `https://wa.me/94${STORE.phone.replace(/^0/, '')}`;
  const lang = language || 'si';
  
  const templates = {
    si: {
      PRICE_REQUEST: ctx.brand && ctx.size 
        ? `${ctx.brand} ${ctx.size} ගැන විස්තර WhatsApp එකෙන් කතා කරමු ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `විස්තර WhatsApp එකෙන් කතා කරමු ✅\n(Public එකේ මිල publish කරන්නේ නැහැ)\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      AVAILABILITY: ctx.brand && ctx.size
        ? `${ctx.brand} ${ctx.size} stock තියෙනවද කියලා WhatsApp එකෙන් check කරමු ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `Stock availability WhatsApp එකෙන් check කරමු ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      VEHICLE_INQUIRY: ctx.vehicleNo
        ? `${ctx.vehicleNo} vehicle එකේ details WhatsApp එකට message කරන්න ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `Vehicle details WhatsApp එකෙන් කතා කරමු ✅\n\nWhatsApp: ${waLink}`,
      
      SERVICE_REQUEST: `අපේ සේවාවන්:\n🔧 Wheel Alignment (${STORE.align})\n⚖️ Balancing\n💨 Nitrogen\n🔄 Rotation\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      COMPLAINT: `කණගාටුයි 😔\nකරුණාකර WhatsApp එකෙන් විස්තර කියන්න.\nඉක්මනින් විසඳන්නම්.\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      PRAISE: `බොහොම ස්තූතියි! 🙏\nඅනාගතයේ අවශ්‍ය වුණොත් WhatsApp එකෙන් කතා කරන්න.\n\nWhatsApp: ${waLink}`,
      
      GENERAL: `Comment එකට ස්තූතියි! 😊\nඅවශ්‍ය tyre brand/size එක WhatsApp එකෙන් කියන්න.\n\nWhatsApp: ${waLink}`
    },
    en: {
      PRICE_REQUEST: ctx.brand && ctx.size
        ? `Let's discuss ${ctx.brand} ${ctx.size} details via WhatsApp ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `Let's discuss details via WhatsApp ✅\n(We avoid public pricing)\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      AVAILABILITY: ctx.brand && ctx.size
        ? `Let me check ${ctx.brand} ${ctx.size} stock via WhatsApp ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `Let's check stock availability via WhatsApp ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      VEHICLE_INQUIRY: ctx.vehicleNo
        ? `Message us on WhatsApp for ${ctx.vehicleNo} details ✅\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`
        : `Let's discuss vehicle details via WhatsApp ✅\n\nWhatsApp: ${waLink}`,
      
      SERVICE_REQUEST: `Our Services:\n🔧 Wheel Alignment (${STORE.align})\n⚖️ Balancing\n💨 Nitrogen\n🔄 Rotation\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      COMPLAINT: `Sorry about this 😔\nPlease WhatsApp us the details - we'll fix it ASAP.\n\nWhatsApp: ${waLink}\nCall: ${STORE.phone}`,
      
      PRAISE: `Thank you so much! 🙏\nFeel free to reach out on WhatsApp anytime.\n\nWhatsApp: ${waLink}`,
      
      GENERAL: `Thanks for your comment! 😊\nWhatsApp us the tyre brand/size you need.\n\nWhatsApp: ${waLink}`
    }
  };
  
  return templates[lang]?.[intent] || templates.si[intent] || templates.si.GENERAL;
}

// Job Handler Logic: Decide AI vs Template
function shouldUseAI(intent, ctx, language) {
  // Simple queries with brand+size → Template (fast & professional)
  if ((intent === 'PRICE_REQUEST' || intent === 'AVAILABILITY') && ctx.brand && ctx.size) {
    console.log('[FB Comment] 🎯 Simple query (brand+size present) → Template reply');
    return false;
  }
  
  // Vehicle inquiry with number → Template
  if (intent === 'VEHICLE_INQUIRY' && ctx.vehicleNo) {
    console.log('[FB Comment] 🚗 Vehicle inquiry (number present) → Template reply');
    return false;
  }
  
  // Service requests → Template
  if (intent === 'SERVICE_REQUEST') {
    console.log('[FB Comment] 🔧 Service request → Template reply');
    return false;
  }
  
  // Praise → Template
  if (intent === 'PRAISE') {
    console.log('[FB Comment] 🙏 Praise → Template reply');
    return false;
  }
  
  // Complex/ambiguous/complaints → AI
  const useAI = intent === 'COMPLAINT' || intent === 'GENERAL' || 
                (!ctx.brand && !ctx.size && !ctx.vehicleNo);
  
  if (useAI) {
    const aiEngine = language === 'si' ? 'Gemini' : 'Claude';
    console.log(`[FB Comment] 🤖 Complex query → AI reply (${aiEngine} for ${language})`);
  }
  
  return useAI;
}

async function fbGet(url, params) {
  const resp = await axios.get(url, { params: { ...(params||{}), access_token: PAGE_TOKEN } });
  return resp.data;
}
async function fbPost(url, body) {
  const resp = await axios.post(url, { ...(body||{}), access_token: PAGE_TOKEN });
  return resp.data;
}

async function listRecentComments(sinceUnix) {
  // Use feed endpoint instead of posts endpoint for better compatibility
  const feed = await fbGet(`https://graph.facebook.com/v21.0/${PAGE_ID}/feed`, { 
    fields: 'id,message,created_time,comments.limit(50){id,message,from,created_time}', 
    limit: 10, 
    since: sinceUnix 
  });
  const out = [];
  for (const p of feed.data || []) {
    if (p.comments && p.comments.data) {
      for (const c of p.comments.data) {
        out.push({ postId: p.id, ...c });
      }
    }
  }
  return out;
}

async function sendPrivateReply(commentId, text) {
  try {
    await fbPost(`https://graph.facebook.com/v18.0/${commentId}/private_replies`, { message: text });
    console.log(`[FB DM] Private reply sent to comment ${commentId}`);
    return true;
  } catch (e) {
    const errMsg = e.response?.data?.error?.message || e.message;
    const errCode = e.response?.data?.error?.code;
    
    // If permission error, just log warning - don't crash
    if (errCode === 200 || /permission|pages_messaging/i.test(errMsg)) {
      console.log('[FB DM] Private reply skipped (no permission) - continuing with public reply only');
      return false;
    }
    
    console.warn('[FB DM] Private reply failed:', errMsg);
    return false;
  }
}

async function replyPublic(commentId, text) {
  try {
    await fbPost(`https://graph.facebook.com/v18.0/${commentId}/comments`, { message: text });
    console.log(`[FB Public] Reply posted to comment ${commentId}`);
  } catch (e) {
    const errMsg = e.response?.data?.error?.message || e.message;
    console.error('[FB Public] Reply failed:', errMsg);
  }
}

function trackIntent(state, ev) {
  state.analytics.push(ev);
  if (state.analytics.length > 500) state.analytics = state.analytics.slice(-500);
}

async function sendWhatsAppMessage(to94, text) {
  try {
    const waClient = getClient();
    if (!waClient) {
      console.warn('[WA] Client not ready, cannot send to', to94);
      return false;
    }
    
    // Convert 94XXXXXXXXX to 94XXXXXXXXX@c.us format
    const chatId = to94.endsWith('@c.us') ? to94 : `${to94}@c.us`;
    await waClient.sendMessage(chatId, text);
    console.log(`[WA] Sent tyre details to ${to94}`);
    return true;
  } catch (e) {
    console.warn('[WA] Send failed:', e.message);
    return false;
  }
}

function buildWhatsAppMessage(ctx, offer) {
  const head = `සුභ දවසක්! 👋 ${STORE.name} (${STORE.loc})\n\n`;
  
  const inquiry = (ctx.brand || ctx.size)
    ? `ඔබ විමසූ විස්තර:\n${[ctx.brand, ctx.size].filter(Boolean).join(' ')}\n\n`
    : '';
  
  const details = offer
    ? `📦 **විස්තර:**\n` +
      `• Brand: ${offer.brand}\n` +
      `• Size: ${offer.size}\n` +
      `• Pattern: ${offer.pattern}\n` +
      `• Warranty: ${offer.warranty || 'Standard'}\n` +
      `• Availability: ${offer.stockHint || 'In stock'}\n` +
      `• Price (LKR): Rs. ${offer.cashPrice?.toLocaleString('en-LK')}\n\n`
    : `අපි ඔබගේ inquiry එක check කරලා හොඳම price එක confirm කරන්නම්.\n\n`;
  
  const footer = 
    `📞 Call/WhatsApp: ${STORE.phone}\n` +
    `🕐 Hours: ${STORE.hours}\n` +
    `🔧 Wheel Alignment: ${STORE.align}\n` +
    `📍 Location: ${STORE.loc}`;
  
  return head + inquiry + details + footer;
}


async function generateSmartReply(intent, ctx, language) {
  const waLink = `https://wa.me/94${STORE.phone.replace(/^0/, '')}`;
  
  // Use advanced template system directly - NO AI
  console.log('[FB Comment] 📝 Using template reply (AI disabled)');
  return null; // Will use getAdvancedTemplate() fallback
}

async function handleComment(c, state) {
  const startTime = Date.now();
  console.log(`\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`);
  console.log(`[FB Comment] 💬 New comment ID: ${c.id}`);
  console.log(`[FB Comment] 📝 Message: "${c.message?.substring(0, 100)}${c.message?.length > 100 ? '...' : ''}"`);
  console.log(`[FB Comment] 👤 From: ${c.from?.name || c.from?.id || 'unknown'}`);
  
  const cls = await classifyComment(c.message || '');
  const ctx = { brand: cls.brand || null, size: cls.size || null, vehicleNo: cls.vehicleNo || null };

  // Extract phone numbers from comment
  const phones = extractPhones(c.message || '');
  console.log(`[FB Comment] 🤖 Classification:`);
  console.log(`  • Intent: ${cls.intent}`);
  console.log(`  • Language: ${cls.language || 'unknown'}`);
  console.log(`  • Brand: ${ctx.brand || 'none'}`);
  console.log(`  • Size: ${ctx.size || 'none'}`);
  console.log(`  • Vehicle: ${ctx.vehicleNo || 'none'}`);
  console.log(`  • Phones: ${phones.length > 0 ? phones.join(', ') : 'none'}`);

  // Handle VEHICLE_INQUIRY
  if (cls.intent === 'VEHICLE_INQUIRY' && ctx.vehicleNo) {
    console.log(`[FB Comment] 🚗 Processing VEHICLE_INQUIRY for ${ctx.vehicleNo}`);
    
    // Decide: AI or Template
    const useAI = shouldUseAI(cls.intent, ctx, cls.language);
    let publicReply;
    
    if (useAI) {
      const smartReply = await generateSmartReply('VEHICLE_INQUIRY', ctx, cls.language);
      publicReply = smartReply || getAdvancedTemplate('VEHICLE_INQUIRY', ctx, cls.language);
    } else {
      publicReply = getAdvancedTemplate('VEHICLE_INQUIRY', ctx, cls.language);
    }
    
    await replyPublic(c.id, publicReply);
    
    const waLink = `https://wa.me/94${STORE.phone.replace(/^0/, '')}`;
    const dmText = `Hello! 👋 ${STORE.name}\n\nVehicle: ${ctx.vehicleNo}\nඔබගේ vehicle එකේ purchase history check කරලා details දෙන්නම්.\n\nකරුණාකර WhatsApp එකට message එකක් දාන්න:\n${waLink}\nCall: ${STORE.phone}`;
    await sendPrivateReply(c.id, dmText);
    
    trackIntent(state, { type: 'VEHICLE_INQUIRY', vehicleNo: ctx.vehicleNo, postId: c.postId, commentId: c.id, ai: useAI, ts: Date.now() });
    console.log(`[FB Comment] ✅ VEHICLE_INQUIRY handled in ${Date.now() - startTime}ms`);
    console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n`);
    return;
  }

  if (cls.intent === 'PRICE_REQUEST' || cls.intent === 'AVAILABILITY') {
    console.log(`[FB Comment] 💰 Processing ${cls.intent}`);
    const offer = await getTyreOffer({ brand: ctx.brand, size: ctx.size });
    
    if (offer) {
      console.log(`[FB Comment] 📦 Offer found: ${offer.brand} ${offer.size} - Rs. ${offer.cashPrice?.toLocaleString('en-LK')}`);
    } else {
      console.log(`[FB Comment] ⚠️  No exact offer found for ${ctx.brand} ${ctx.size}`);
    }
    
    // If phone number found in comment, send WhatsApp directly
    if (phones.length > 0) {
      console.log(`[FB Comment] 📱 Phone number(s) detected, sending WhatsApp message...`);
      let waSent = false;
      const waMessage = buildWhatsAppMessage(ctx, offer);
      
      for (const phone of phones) {
        try {
          console.log(`  → Sending to ${phone}...`);
          const success = await sendWhatsAppMessage(phone, waMessage);
          if (success) {
            console.log(`  ✅ WhatsApp sent to ${phone}`);
            waSent = true;
          } else {
            console.log(`  ❌ WhatsApp failed for ${phone}`);
          }
        } catch (e) {
          console.warn(`  ❌ WhatsApp error for ${phone}:`, e.message);
        }
      }
      
      // Public confirmation (no price!)
      const publicReply = waSent
        ? `විස්තර WhatsApp message එකක් ලෙස යවා තියෙනවා ✅\n\nතවත් ප්‍රශ්න ඇතිනම් ${STORE.phone} අමතන්න.`
        : `WhatsApp message එක යවන්න බැරි වුනා 😔\n\nකරුණාකර ${STORE.phone} අමතන්න හෝ WhatsApp කරන්න.`;
      
      await replyPublic(c.id, publicReply);
      
      trackIntent(state, {
        type: 'PRICE_REQUEST_WA',
        brand: ctx.brand,
        size: ctx.size,
        phones,
        waSent,
        postId: c.postId,
        commentId: c.id,
        ts: Date.now()
      });
      
      console.log(`[FB Comment] ✅ WhatsApp flow completed in ${Date.now() - startTime}ms`);
      console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n`);
      return;
    }
    
    // No phone in comment - try Facebook DM (if enabled) and guide to WhatsApp
    if (DM_WA) {
      console.log(`[FB Comment] 💌 No phone number, trying Facebook DM...`);
      const dmText = [ t('dmBase', ctx), offer ? t('dmOffer', offer) : t('dmNoOffer', ctx), `\nWhatsApp direct: https://wa.me/94${STORE.phone.replace(/^0/, '')}` ].join('\n');
      
      try {
        await sendPrivateReply(c.id, dmText);
        console.log(`[FB Comment] ✓ Facebook DM sent`);
      } catch (e) {
        console.log('[FB Comment] ⚠️  Facebook DM failed (pages_messaging permission not available) - using public reply only');
      }
    } else {
      console.log(`[FB Comment] 📝 DM disabled (FB_DM_WHATSAPP_FALLBACK=false) - using public reply only`);
    }
    
    // Decide: AI or Advanced Template
    const useAI = shouldUseAI(cls.intent, ctx, cls.language);
    let publicReply;
    
    if (useAI) {
      const smartReply = await generateSmartReply(cls.intent, ctx, cls.language);
      publicReply = smartReply || getAdvancedTemplate(cls.intent, ctx, cls.language);
      
      if (smartReply) {
        console.log(`[FB Comment] ✓ AI reply: "${smartReply.substring(0, 80)}..."`);
      } else {
        console.log(`[FB Comment] ⚠️  AI failed, using advanced template`);
      }
    } else {
      publicReply = getAdvancedTemplate(cls.intent, ctx, cls.language);
    }
    
    await replyPublic(c.id, publicReply);

    trackIntent(state, { type: cls.intent, brand: ctx.brand, size: ctx.size, postId: c.postId, commentId: c.id, dm: true, ai: useAI, ts: Date.now() });
    console.log(`[FB Comment] ✅ ${cls.intent} handled in ${Date.now() - startTime}ms`);
    console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n`);
    return;
  }

  // Handle other intents (SERVICE, COMPLAINT, PRAISE, GENERAL)
  console.log(`[FB Comment] 🔔 Processing ${cls.intent}`);
  
  // Decide: AI or Advanced Template
  const useAI = shouldUseAI(cls.intent, ctx, cls.language);
  let finalReply;
  
  if (useAI) {
    const smartReply = await generateSmartReply(cls.intent, ctx, cls.language);
    finalReply = smartReply || getAdvancedTemplate(cls.intent, ctx, cls.language);
    
    if (smartReply) {
      console.log(`[FB Comment] ✓ AI reply: "${smartReply.substring(0, 80)}..."`);
    } else {
      console.log(`[FB Comment] ⚠️  AI failed, using advanced template`);
    }
  } else {
    finalReply = getAdvancedTemplate(cls.intent, ctx, cls.language);
  }
  
  if (smartReply) {
    console.log(`[FB Comment] ✓ Claude reply: "${smartReply.substring(0, 80)}..."`);
  } else {
    console.log(`[FB Comment] ⚠️  Claude failed, using template`);
  }
  
  await replyPublic(c.id, finalReply);
  
  trackIntent(state, { type: cls.intent, brand: ctx.brand, size: ctx.size, vehicleNo: ctx.vehicleNo, postId: c.postId, commentId: c.id, ai: useAI, ts: Date.now() });
  console.log(`[FB Comment] ✅ ${cls.intent} handled in ${Date.now() - startTime}ms`);
  console.log(`━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n`);
}

async function tick() {
  const state = loadState();
  const sinceUnix = state.lastChecked ? Math.floor(state.lastChecked/1000) : Math.floor((Date.now()-24*3600*1000)/1000);
  try {
    const comments = await listRecentComments(sinceUnix);
    if (comments.length > 0) {
      if (comments.length > 0) {
      console.log(`[CommentResponder] Found ${comments.length} new comments to process`);
    }
    }
    
    for (const c of comments) {
      if (state.handled[c.id]) continue;
      
      try {
        await handleComment(c, state);
        state.handled[c.id] = 1;
      } catch (e) {
        console.error(`[CommentResponder] Error handling comment ${c.id}:`, e.message);
        // Mark as handled even if error, to avoid retry loops
        state.handled[c.id] = 1;
      }
    }
    state.lastChecked = Date.now();
  } catch (e) {
    const errMsg = e.response?.data?.error?.message || e.message;
    console.error('[CommentResponder] Tick error:', errMsg);
  } finally {
    saveState(state);
  }
}

let intervalId = null;

function start() {
  if (intervalId) {
    console.log('[CommentResponder] Already running, skipping start');
    return;
  }

  if (String(process.env.ENABLE_FB_COMMENT_RESPONDER||'false').toLowerCase() !== 'true') {
    console.log('[CommentResponder] disabled via env');
    return;
  }
  if (!PAGE_ID || !PAGE_TOKEN) {
    console.warn('[CommentResponder] Missing Facebook credentials');
    return;
  }
  console.log(`[CommentResponder] Running (every ${INTERVAL_SEC}s)…`);
  tick();
  intervalId = setInterval(tick, INTERVAL_SEC*1000);
}

async function fetchCommentDetails(commentId) {
  try {
    const url = `https://graph.facebook.com/v18.0/${commentId}?fields=id,message,from,created_time,parent&access_token=${PAGE_TOKEN}`;
    const resp = await axios.get(url);
    return resp.data;
  } catch (e) {
    console.warn(`[Webhook] Failed to fetch comment ${commentId}:`, e.message);
    return null;
  }
}

async function handleCommentWebhook(change) {
  const startTime = Date.now();
  
  // Handle Page feed change payloads for real-time comments
  if (!change || change.field !== 'feed') {
    console.log('[Webhook] Ignoring non-feed change');
    return false;
  }
  
  const v = change.value || {};
  if (v.item !== 'comment' || (v.verb !== 'add' && v.verb !== 'edited')) {
    console.log(`[Webhook] Ignoring feed change: item=${v.item}, verb=${v.verb}`);
    return false;
  }

  const commentId = v.comment_id;
  const postId = v.post_id;
  let message = v.message;
  let from = v.from || {};
  let created_time = v.created_time;

  console.log(`[Webhook] 📥 New ${v.verb} event: comment=${commentId}, post=${postId}`);

  // If message is missing, fetch full comment details
  if (!message || !from.id) {
    console.log('[Webhook] Message/from missing, fetching full comment details...');
    const details = await fetchCommentDetails(commentId);
    if (details) {
      message = details.message || message;
      from = details.from || from;
      created_time = details.created_time || created_time;
      console.log(`[Webhook] ✓ Fetched details: ${message?.substring(0, 50)}...`);
    } else {
      console.warn('[Webhook] Failed to fetch comment details, using webhook payload');
    }
  }

  if (!commentId || !postId) {
    console.warn('[Webhook] Missing commentId or postId, skipping');
    return false;
  }

  const c = {
    id: commentId,
    postId,
    message: message || '',
    from,
    created_time: created_time || new Date().toISOString(),
    comment_count: 0
  };

  const state = loadState();
  
  // Check if already handled
  if (state.handled[commentId]) {
    console.log(`[Webhook] ⏭️  Comment ${commentId} already handled, skipping`);
    return false;
  }

  try {
    console.log(`[Webhook] 🤖 Processing: "${c.message?.substring(0, 80)}..."`);
    await handleComment(c, state);
    state.handled[commentId] = 1;
    state.lastChecked = Date.now();
    saveState(state);
    
    const elapsed = Date.now() - startTime;
    console.log(`[Webhook] ✅ Comment ${commentId} handled successfully in ${elapsed}ms`);
    return true;
  } catch (e) {
    console.error('[Webhook] ❌ handleComment error:', e?.message || e);
    console.error('[Webhook] Stack:', e?.stack);
    state.handled[commentId] = 1; // Mark handled to avoid loops
    saveState(state);
    return false;
  }
}

if (require.main === module) {
  require('dotenv').config();
  start();
}

module.exports = { start, handleCommentWebhook };
