/**
 * Test Gemini API keys individually.
 *
 * Usage:
 *   cd c:\whatsapp-sql-api
 *   node scripts/ai/test_gemini_keys.js
 *
 * It loads keys from:
 *   GEMINI_API_KEY, GEMINI_API_KEY_2 ... GEMINI_API_KEY_10
 */

require('dotenv').config();

const { GoogleGenerativeAI } = require('@google/generative-ai');

const PROMPT = 'Reply with exactly: OK';

const MODEL_CANDIDATES = [
  // Keep list short; we only need to see which ones work in this environment.
  'gemini-2.0-flash',
  'gemini-2.0-flash-lite',
  'gemini-flash-lite-latest',
];

function loadKeys() {
  const keys = [];
  if (process.env.GEMINI_API_KEY) keys.push(process.env.GEMINI_API_KEY);
  for (let i = 2; i <= 10; i++) {
    const k = process.env[`GEMINI_API_KEY_${i}`];
    if (k) keys.push(k);
  }
  return keys;
}

function briefError(err) {
  const status = err?.status ?? err?.response?.status ?? null;
  const msg = (err && err.message) ? String(err.message) : String(err);

  // Extract useful quota hints if present.
  const quotaHint = msg.includes('Quota') || msg.toLowerCase().includes('quota') || msg.includes('429')
    ? ' (quota/rate-limit)'
    : '';

  return {
    status,
    message: msg.slice(0, 300) + (msg.length > 300 ? '…' : ''),
    quotaHint,
  };
}

async function testKeyOnModel(apiKey, modelName) {
  const genAI = new GoogleGenerativeAI(apiKey);
  const model = genAI.getGenerativeModel({
    model: modelName,
    generationConfig: {
      maxOutputTokens: 10,
      temperature: 0,
    },
  });

  const result = await model.generateContent(PROMPT);
  const response = await result.response;
  const text = response.text();
  return String(text || '').trim();
}

async function main() {
  const keys = loadKeys();

  if (!keys.length) {
    console.error('No keys found. Set GEMINI_API_KEY (and/or GEMINI_API_KEY_2..10) in .env');
    process.exit(1);
  }

  console.log(`Loaded ${keys.length} key(s). Testing models: ${MODEL_CANDIDATES.join(', ')}`);
  console.log('---');

  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    const keyLabel = `Key ${i + 1}/${keys.length} (...${String(key).slice(-4)})`;

    console.log(keyLabel);

    let anySuccess = false;
    for (const modelName of MODEL_CANDIDATES) {
      try {
        const text = await testKeyOnModel(key, modelName);
        anySuccess = true;
        console.log(`  ✅ ${modelName}: ${JSON.stringify(text)}`);
        // Stop after first success for this key.
        break;
      } catch (err) {
        const info = briefError(err);
        console.log(`  ❌ ${modelName}: status=${info.status ?? 'n/a'}${info.quotaHint} msg=${JSON.stringify(info.message)}`);
      }
    }

    if (!anySuccess) {
      console.log('  -> No working model for this key (in current environment).');
    }

    console.log('---');
  }
}

main().catch((e) => {
  console.error('Fatal:', e);
  process.exit(1);
});
