const fs = require('fs');
const path = require('path');

function loadStyleGuide() {
  const file = path.join(__dirname, '..', 'facebook-style-guide.txt');
  try {
    if (fs.existsSync(file)) {
      return fs.readFileSync(file, 'utf8');
    }
  } catch (e) {
    // ignore
  }
  return '';
}

module.exports = { loadStyleGuide };
