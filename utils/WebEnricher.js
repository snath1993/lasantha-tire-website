// Optional web enrichment for trend bullets (safe, minimal)
// Controlled by env: ALLOW_WEB_SCRAPE=1

const axios = require('axios');

const DEFAULT_SOURCES = [
  'https://www.tyrepress.com/category/news/',
  'https://www.autoblog.com/',
  'https://www.caranddriver.com/news/'
];

function isEnabled() {
  return String(process.env.ALLOW_WEB_SCRAPE || '0') === '1';
}

async function fetchTrends(sources = DEFAULT_SOURCES, timeoutMs = 6000) {
  if (!isEnabled()) return [];
  const bullets = [];
  for (const url of sources) {
    try {
      const res = await axios.get(url, { timeout: timeoutMs });
      const text = String(res.data || '').replace(/\s+/g, ' ').slice(0, 20000);
      // crude keyword picks
      const picks = [];
      const pairs = [
        [/EV|electric vehicle/i, 'EV tyre insights'],
        [/rain|monsoon|wet/i, 'Wet-grip focus'],
        [/fuel|economy|efficiency/i, 'Fuel-saving trend'],
        [/durability|warranty|mileage/i, 'Durability matters'],
        [/all[- ]terrain|AT tyre/i, 'All-terrain demand'],
      ];
      for (const [re, label] of pairs) {
        if (re.test(text)) picks.push(label);
      }
      // take up to 2 unique labels per source
      for (const p of picks.slice(0, 2)) {
        if (!bullets.includes(p)) bullets.push(p);
      }
      if (bullets.length >= 4) break;
    } catch (_) {
      // ignore
    }
  }
  return bullets.slice(0, 4);
}

module.exports = { fetchTrends, isEnabled };
