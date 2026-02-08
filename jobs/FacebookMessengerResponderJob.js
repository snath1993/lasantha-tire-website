// FacebookMessengerResponderJob.js - Claude AI for Facebook Messenger (Inbox) conversations
const axios = require('axios');
const path = require('path');
const fs = require('fs');
const { generate } = require('../utils/ClaudeClient');
const { extractTyreSizeFlexible, extractVehicleNumber } = require('../utils/detect');
const { getTyreOffer } = require('../utils/PriceProvider');

const PAGE_TOKEN = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
const INTERVAL_SEC = Number(process.env.FB_MESSENGER_SCAN_INTERVAL_SEC || 60);

const STORE = {
  name: process.env.STORE_NAME || 'Lasantha Tyre Traders',
  loc: process.env.STORE_LOCATION || 'Thalawathugoda',
  phone: process.env.STORE_PHONE || '0721222509',
  hours: process.env.STORE_HOURS || '06:30-21:00',
  align: process.env.STORE_WHEEL_ALIGNMENT_HOURS || '07:30-18:00'
};

// Business Rules for Claude AI
const BUSINESS_RULES = `
**LASANTHA TYRE TRADERS - MESSENGER CONVERSATION RULES**
කරුණාකර මෙම rules අනිවාර්යයෙන් follow කරන්න:

1. **මිල දැනුම් දීම (Pricing in Messenger)**
   - Messenger PRIVATE conversations වල මිල දෙන්න පුළුවන් ✅
   - විශ්වාස කළ හැකි customer කෙනෙකුට විතරක් මිල දෙන්න
   - තවමත් මිල system එකේ නැත්නම්, WhatsApp එකට direct කරන්න

2. **Store Identity (වෙනස් කරන්න බැහැ)**
   - Name: ${STORE.name}
   - Location: ${STORE.loc}
   - Phone: ${STORE.phone}
   - Hours: ${STORE.hours}
   - Wheel Alignment: ${STORE.align}

3. **Conversation Style**
   - Natural, friendly, professional
   - Customer සිංහල use කරොත් සිංහල reply
   - English use කරොත් English reply
   - Multi-turn conversation support කරන්න

4. **Feature Detection**
   - Tyre size: 195/65R15, 700/16, etc.
   - Brand: BRIDGESTONE, MICHELIN, etc.
   - Vehicle number: CBH-6483, WP-1234, etc.
   - Intent: price, availability, service, complaint, inquiry

5. **Response Guidelines**
   - Price requests: Offer details if available, else WhatsApp redirect
   - Availability: Check stock, suggest alternatives
   - Service: Mention Wheel Alignment, Balancing, Nitrogen
   - Vehicle history: Offer to check invoice/purchase records
   - Complaints: Apologize, resolve, escalate if needed

6. **Safety Rules**
   - කිසිම වරදක් හෝ false promise එකක් කියන්න එපා
   - Uncertain නම් "${STORE.phone} WhatsApp එකට call කරන්න" කියන්න
   - Customer abuse එකක් තියනවනම් politely terminate
`;

const STATE_PATH = path.join(__dirname, '..', 'data', 'messenger-history.json');
function loadState() {
  try { return JSON.parse(fs.readFileSync(STATE_PATH, 'utf8')); }
  catch { return { lastChecked: 0, conversations: {}, analytics: [] }; }
}
function saveState(s) {
  fs.mkdirSync(path.dirname(STATE_PATH), { recursive: true });
  fs.writeFileSync(STATE_PATH, JSON.stringify(s, null, 2));
}

async function fbGet(url, params) {
  const resp = await axios.get(url, { params: { ...(params||{}), access_token: PAGE_TOKEN } });
  return resp.data;
}

async function fbPost(url, body) {
  const resp = await axios.post(url, { ...(body||{}), access_token: PAGE_TOKEN });
  return resp.data;
}

async function listRecentMessages(sinceUnix) {
  // Get conversations with recent activity
  const conversations = await fbGet(`https://graph.facebook.com/v18.0/me/conversations`, {
    fields: 'id,updated_time,participants',
    limit: 20
  });
  
  const messages = [];
  for (const conv of conversations.data || []) {
    const convMessages = await fbGet(`https://graph.facebook.com/v18.0/${conv.id}/messages`, {
      fields: 'id,message,from,created_time',
      limit: 10,
      since: sinceUnix
    });
    
    for (const msg of convMessages.data || []) {
      // Filter out messages sent by page (only get customer messages)
      if (msg.from && !msg.from.id?.includes('page')) {
        messages.push({ conversationId: conv.id, ...msg });
      }
    }
  }
  
  return messages;
}

async function sendMessage(conversationId, text) {
  try {
    await fbPost(`https://graph.facebook.com/v18.0/${conversationId}/messages`, { message: text });
    return true;
  } catch (e) {
    console.warn('Messenger send failed:', e.response?.data || e.message);
    return false;
  }
}

async function generateSmartResponse(message, conversationHistory = []) {
  // Extract features
  const tyreSize = extractTyreSizeFlexible(message);
  const vehicleNo = extractVehicleNumber(message);
  const brand = (message.match(/\b(bridgestone|michelin|yokohama|dunlop|goodyear|continental|pirelli|kumho|nexen|giti|marshal|toyo|maxxis|gt)\b/i)?.[0] || '').toUpperCase() || null;
  
  // Check if price available
  let priceInfo = null;
  if (tyreSize || brand) {
    priceInfo = await getTyreOffer({ brand, size: tyreSize });
  }

  const historyContext = conversationHistory.length > 0 
    ? `\n**Previous Messages:**\n${conversationHistory.slice(-3).map(h => `${h.from}: ${h.message}`).join('\n')}\n`
    : '';

  const prompt = `${BUSINESS_RULES}

**TASK:** Generate a natural Messenger reply for a private conversation

${historyContext}
**Current Customer Message:** "${message}"

**Detected Info:**
- Tyre Size: ${tyreSize || 'Not detected'}
- Brand: ${brand || 'Not detected'}
- Vehicle No: ${vehicleNo || 'Not detected'}
- Price Available: ${priceInfo ? 'YES (Include in reply)' : 'NO (Redirect to WhatsApp)'}

${priceInfo ? `**Price Details (PRIVATE - okay to share in Messenger DM):**
Brand: ${priceInfo.brand}
Size: ${priceInfo.size}
Pattern: ${priceInfo.pattern}
Price: Rs. ${priceInfo.cashPrice?.toLocaleString('en-LK')}
Warranty: ${priceInfo.warranty}
Stock: ${priceInfo.stockHint}
` : ''}

**Response Guidelines:**
1. Natural conversational style (මිත්‍රශීලී)
2. If price info available, share it (Messenger private conversation වල okay)
3. If not available, guide to WhatsApp: ${STORE.phone}
4. If vehicle number, offer to check history
5. If service inquiry, mention Wheel Alignment (${STORE.align}), Balancing, etc.
6. Keep it SHORT (2-4 sentences max)

Generate reply in ${/[\u0D80-\u0DFF]/.test(message) ? 'Sinhala' : 'English'}:`;

  try {
    const reply = await generate({ prompt, maxTokens: 250, temperature: 0.5 });
    return reply.trim();
  } catch (e) {
    console.warn('Claude response generation failed:', e.message);
    return `ස්තුතියි message එකට! 🙏\n\nකරුණාකර WhatsApp එකට අමතන්න: ${STORE.phone}\n\nබොහොම ඉක්මනට reply කරන්නම්.`;
  }
}

async function handleMessage(msg, state) {
  const convId = msg.conversationId;
  
  // Get conversation history
  const history = state.conversations[convId] || [];
  
  // Generate smart response
  const response = await generateSmartResponse(msg.message, history);
  
  // Send reply
  const sent = await sendMessage(convId, response);
  
  // Track in history
  history.push({ from: 'customer', message: msg.message, ts: Date.now() });
  history.push({ from: 'bot', message: response, ts: Date.now() });
  if (history.length > 20) history.shift(); // Keep last 10 exchanges
  
  state.conversations[convId] = history;
  
  // Analytics
  state.analytics.push({
    type: 'messenger_reply',
    conversationId: convId,
    messageId: msg.id,
    sent,
    ts: Date.now()
  });
  if (state.analytics.length > 500) state.analytics = state.analytics.slice(-500);
  
  console.log(`[Messenger] Replied to ${convId}: ${response.slice(0, 50)}...`);
}

async function tick() {
  const state = loadState();
  const sinceUnix = state.lastChecked ? Math.floor(state.lastChecked/1000) : Math.floor((Date.now()-24*3600*1000)/1000);
  
  try {
    const messages = await listRecentMessages(sinceUnix);
    
    for (const msg of messages) {
      const key = `${msg.conversationId}_${msg.id}`;
      if (state.conversations[key]) continue; // Already handled
      
      await handleMessage(msg, state);
      state.conversations[key] = true; // Mark as handled
    }
    
    state.lastChecked = Date.now();
  } catch (e) {
    console.error('[MessengerResponder] tick error:', e.response?.data || e.message);
  } finally {
    saveState(state);
  }
}

function start() {
  if (String(process.env.ENABLE_FB_MESSENGER_RESPONDER||'false').toLowerCase() !== 'true') {
    console.log('[MessengerResponder] disabled via env');
    return;
  }
  if (!PAGE_TOKEN) {
    console.warn('[MessengerResponder] Missing Facebook access token');
    return;
  }
  console.log(`[MessengerResponder] Running (every ${INTERVAL_SEC}s)…`);
  tick();
  setInterval(tick, INTERVAL_SEC*1000);
}

if (require.main === module) {
  require('dotenv').config();
  start();
}

module.exports = { start };
