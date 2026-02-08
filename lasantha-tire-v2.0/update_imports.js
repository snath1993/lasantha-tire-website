const fs = require('fs');
const path = require('path');

const srcDir = path.join(__dirname, 'src');

const replacements = [
    { from: /from ['"]@\/config/g, to: "from '@/core/config" },
    { from: /from ['"]@\/contexts/g, to: "from '@/core/contexts" },
    { from: /from ['"]@\/hooks/g, to: "from '@/core/hooks" },
    { from: /from ['"]@\/lib/g, to: "from '@/core/lib" },
    { from: /from ['"]@\/utils/g, to: "from '@/core/utils" },
    { from: /from ['"]@\/components\/mobile/g, to: "from '@/views/mobile" },
    { from: /from ['"]@\/components\/erp/g, to: "from '@/views/shared/erp" },
    { from: /from ['"]@\/components\/ui/g, to: "from '@/views/shared/ui" },
    { from: /from ['"]@\/components\/modals/g, to: "from '@/views/shared/modals" },
    { from: /from ['"]@\/components\/layout/g, to: "from '@/views/shared/layout" },
    { from: /from ['"]@\/components\/filters/g, to: "from '@/views/shared/filters" },
    { from: /from ['"]@\/components\/BusinessStatusReports/g, to: "from '@/views/desktop/BusinessStatusReports" },
    { from: /from ['"]@\/components\/ConnectionSettings/g, to: "from '@/views/desktop/ConnectionSettings" },
    { from: /from ['"]@\/components\/ERPDashboard/g, to: "from '@/views/desktop/ERPDashboard" },
    // Handle imports that might not use 'from' (like require, though unlikely in TS)
    // Handle import { ... } from '@/config/...'
    { from: /import .* from ['"]@\/config/g, to: (match) => match.replace('@/config', '@/core/config') },
    // Actually the regex above with 'from' covers most cases.
    // Let's add a more generic replacement for the path part only if it follows 'from' or 'import'
];

function walk(dir) {
    const files = fs.readdirSync(dir);
    files.forEach(file => {
        const filePath = path.join(dir, file);
        const stat = fs.statSync(filePath);
        if (stat.isDirectory()) {
            walk(filePath);
        } else if (file.endsWith('.ts') || file.endsWith('.tsx') || file.endsWith('.js')) {
            updateFile(filePath);
        }
    });
}

function updateFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    let originalContent = content;

    // Simple string replacements for the paths
    content = content.replace(/['"]@\/config/g, "'@/core/config");
    content = content.replace(/['"]@\/contexts/g, "'@/core/contexts");
    content = content.replace(/['"]@\/hooks/g, "'@/core/hooks");
    content = content.replace(/['"]@\/lib/g, "'@/core/lib");
    content = content.replace(/['"]@\/utils/g, "'@/core/utils");
    content = content.replace(/['"]@\/components\/mobile/g, "'@/views/mobile");
    content = content.replace(/['"]@\/components\/erp/g, "'@/views/shared/erp");
    content = content.replace(/['"]@\/components\/ui/g, "'@/views/shared/ui");
    content = content.replace(/['"]@\/components\/modals/g, "'@/views/shared/modals");
    content = content.replace(/['"]@\/components\/layout/g, "'@/views/shared/layout");
    content = content.replace(/['"]@\/components\/filters/g, "'@/views/shared/filters");
    content = content.replace(/['"]@\/components\/BusinessStatusReports/g, "'@/views/desktop/BusinessStatusReports");
    content = content.replace(/['"]@\/components\/ConnectionSettings/g, "'@/views/desktop/ConnectionSettings");
    content = content.replace(/['"]@\/components\/ERPDashboard/g, "'@/views/desktop/ERPDashboard");

    if (content !== originalContent) {
        console.log(`Updating ${filePath}`);
        fs.writeFileSync(filePath, content);
    }
}

walk(srcDir);
