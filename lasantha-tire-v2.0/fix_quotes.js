const fs = require('fs');
const path = require('path');

const srcDir = path.join(__dirname, 'src');

function walk(dir) {
    const files = fs.readdirSync(dir);
    files.forEach(file => {
        const filePath = path.join(dir, file);
        const stat = fs.statSync(filePath);
        if (stat.isDirectory()) {
            walk(filePath);
        } else if (file.endsWith('.ts') || file.endsWith('.tsx') || file.endsWith('.js')) {
            fixFile(filePath);
        }
    });
}

function fixFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    let originalContent = content;

    // Fix mixed quotes: 'path" -> 'path'
    content = content.replace(/'(@\/[^"']*)["']/g, "'$1'");
    
    // Also fix if I accidentally created "path' -> 'path' (though my script used ' replacement)
    
    if (content !== originalContent) {
        console.log(`Fixing quotes in ${filePath}`);
        fs.writeFileSync(filePath, content);
    }
}

walk(srcDir);
