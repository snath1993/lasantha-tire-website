// ClaudeClient.js - minimal wrapper around Anthropic Messages API via axios
const axios = require('axios');

const DEFAULT_MODEL = process.env.CLAUDE_MODEL || 'claude-haiku-4-5-20251001';

async function generate({ prompt, model = DEFAULT_MODEL, maxTokens = 600, temperature = 0.3, timeoutMs = 15000 }) {
  if (!process.env.ANTHROPIC_API_KEY) {
    throw new Error('ANTHROPIC_API_KEY missing');
  }
  const resp = await axios.post(
    'https://api.anthropic.com/v1/messages',
    {
      model,
      max_tokens: maxTokens,
      temperature,
      messages: [{ role: 'user', content: prompt }]
    },
    {
      headers: {
        'x-api-key': process.env.ANTHROPIC_API_KEY,
        'anthropic-version': '2023-06-01',
        'content-type': 'application/json'
      },
      timeout: timeoutMs
    }
  );
  const parts = resp.data?.content || [];
  const text = Array.isArray(parts) ? parts.map(p=>p?.text||'').join('\n').trim() : '';
  return text;
}

module.exports = { generate };
