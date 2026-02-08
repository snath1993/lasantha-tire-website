// utils/redactSensitive.js
// Redacts common sensitive values from logs without changing runtime behavior.

function maskKeepLast(value, keepLast = 2) {
  const str = String(value);
  if (str.length <= keepLast) return '*'.repeat(str.length);
  return '*'.repeat(str.length - keepLast) + str.slice(-keepLast);
}

function redactSriLankaPhoneNumbers(text) {
  // Handles formats like: 9477xxxxxxx, +9477xxxxxxx, 077xxxxxxx
  return String(text)
    // +94XXXXXXXXX or 94XXXXXXXXX (11-12 digits including country code)
    .replace(/\b\+?94(\d{9})\b/g, (_, tail9) => `94${maskKeepLast(tail9, 2)}`)
    // 0XXXXXXXXX (10 digits)
    .replace(/\b0(\d{9})\b/g, (_, tail9) => `0${maskKeepLast(tail9, 2)}`);
}

function redactAccessTokens(text) {
  // access_token=... in URLs / logs
  return String(text)
    .replace(/(access_token=)([^\s&]+)/gi, '$1***')
    .replace(/(FACEBOOK_PAGE_ACCESS_TOKEN\s*[:=]\s*)([^\s]+)/gi, '$1***');
}

function redactApiKeys(text) {
  // Generic patterns for common key names appearing in logs
  return String(text)
    .replace(/\b(GEMINI_API_KEY|ANTHROPIC_API_KEY|OPENAI_API_KEY|SQL_PASSWORD|DB_PASSWORD)\b\s*[:=]\s*([^\s]+)/gi, '$1=***');
}

function redactSensitive(text) {
  let out = String(text ?? '');
  out = redactAccessTokens(out);
  out = redactApiKeys(out);
  out = redactSriLankaPhoneNumbers(out);
  return out;
}

module.exports = { redactSensitive };
