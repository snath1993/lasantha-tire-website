// Load analyzed Facebook post style guide
const fs = require('fs');
const path = require('path');

function loadPostStyleGuide() {
  try {
    const stylePath = path.join(__dirname, '../facebook-post-style-guide.json');
    if (!fs.existsSync(stylePath)) {
      console.warn('⚠️  facebook-post-style-guide.json not found. Using default style.');
      return null;
    }
    const data = JSON.parse(fs.readFileSync(stylePath, 'utf8'));
    return data.rawAnalysis || JSON.stringify(data);
  } catch (err) {
    console.warn('⚠️  Could not load post style guide:', err.message);
    return null;
  }
}

module.exports = { loadPostStyleGuide };
