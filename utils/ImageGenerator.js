const sharp = require('sharp');
const fs = require('fs').promises;
const path = require('path');

// Helper to pick a random item from an array
const pickRandom = (arr) => arr[Math.floor(Math.random() * arr.length)];
const escapeXML = (str) => String(str).replace(/[<>&'"]/g, c => ({'<':'&lt;','>':'&gt;','&':'&amp;','\'':'&apos;','"':'&quot;'}[c]));

class ImageGenerator {
    constructor() {
        this.libraries = {};
        this.imagePathCache = new Map(); // Cache for resolved image paths
        this.fontsLoaded = false;
    }

    async loadLibraries() {
        if (this.fontsLoaded) return; // Avoid reloading

        const libDir = path.join(__dirname, '..', 'assets', 'poster-elements');
        const libraryFiles = {
            shapes: 'shape-library.json',
            layouts: 'layout-library.json',
            palettes: 'color-palette-library.json',
            taglines: 'tagline-library.json',
            brands: 'brand-list.json',
        };

        try {
            for (const [key, filename] of Object.entries(libraryFiles)) {
                const filePath = path.join(libDir, filename);
                const fileContent = await fs.readFile(filePath, 'utf-8');
                this.libraries[key] = JSON.parse(fileContent)[key];
            }
            this.fontsLoaded = true;
            console.log('🎨 Dynamic Design libraries loaded successfully.');
        } catch (e) {
            console.error(`❌ CRITICAL: Could not load design library '${e.path || ''}'.`, e.message);
            throw new Error(`Failed to load design libraries.`);
        }
    }

    async findTyreImage(product) {
        if (!product || !product.brand) return null;
        const cacheKey = `${product.brand}-${product.name}`;
        if (this.imagePathCache.has(cacheKey)) {
            return this.imagePathCache.get(cacheKey);
        }

        const rootDir = path.join(__dirname, '..', 'tyre-images');
        const exts = ['.jpg', '.jpeg', '.png', '.webp'];
        const norm = (s) => String(s || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
        const brandFolder = String(product.brand || '').trim();
        const patternN = norm(product.name);

        try {
            const brandDir = path.join(rootDir, brandFolder);
            const entries = await fs.readdir(brandDir, { withFileTypes: true });
            let candidates = [];
            for (const ent of entries) {
                const full = path.join(brandDir, ent.name);
                if (ent.isDirectory()) {
                    const sub = await fs.readdir(full, { withFileTypes: true });
                    for (const f of sub) {
                        const ffull = path.join(full, f.name);
                        if (f.isFile() && exts.includes(path.extname(ffull).toLowerCase())) {
                            candidates.push(ffull);
                        }
                    }
                } else if (exts.includes(path.extname(full).toLowerCase())) {
                    candidates.push(full);
                }
            }
            const found = candidates.find(fp => norm(path.basename(fp)).includes(patternN));
            if (found) {
                this.imagePathCache.set(cacheKey, found);
                return found;
            }
            // Strict rule: no image if exact pattern not found
            this.imagePathCache.set(cacheKey, null);
            return null;

        } catch (e) {
            if (e.code !== 'ENOENT') console.warn('Image scan error:', e.message);
            return null;
        }
    }

    renderBackground(palette, shapes) {
        // Optional gradient support with SOLID base layer to prevent bleed-through
        let backgroundSvg = '';
        
        // ALWAYS render a solid base layer first (user: "wisthara photo eken pita yanawa")
        backgroundSvg += `<rect width="100%" height="100%" fill="${palette.background}" opacity="1.0" />`;
        
        const grad = palette.gradient;
        if (grad) {
            const safeId = String(palette.id || 'palette').replace(/[^a-z0-9]/gi, '_').toLowerCase();
            const gradId = `bgGradient_${safeId}`;
            if (grad.type === 'radial') {
                const stops = Array.isArray(grad.stops) && grad.stops.length
                    ? grad.stops
                    : [
                        { offset: '0%', color: grad.from || palette.background },
                        { offset: '100%', color: grad.to || palette.primary }
                      ];
                backgroundSvg += `
                    <defs>
                        <radialGradient id="${gradId}" cx="50%" cy="50%" r="75%">
                            ${stops.map(s => `<stop offset="${s.offset}" stop-color="${s.color}" />`).join('')}
                        </radialGradient>
                    </defs>
                    <rect width="100%" height="100%" fill="url(#${gradId})" opacity="0.85" />`;
            } else {
                // linear (default)
                const angle = Number.isFinite(grad.angle) ? grad.angle : 45;
                const stops = Array.isArray(grad.stops) && grad.stops.length
                    ? grad.stops
                    : [
                        { offset: '0%', color: grad.from || palette.background },
                        { offset: '100%', color: grad.to || palette.primary }
                      ];
                backgroundSvg += `
                    <defs>
                        <linearGradient id="${gradId}" x1="0%" y1="0%" x2="100%" y2="0%" gradientTransform="rotate(${angle})">
                            ${stops.map(s => `<stop offset="${s.offset}" stop-color="${s.color}" />`).join('')}
                        </linearGradient>
                    </defs>
                    <rect width="100%" height="100%" fill="url(#${gradId})" opacity="0.85" />`;
            }
        }

        // Add 3 to 5 background shapes for production-level graphics (user: "graphics disign part eka meeta wada improve wenna ona")
        const shapeCount = Math.floor(Math.random() * 3) + 3; // Increased from 2-4 to 3-5
        const backgroundShapes = shapes.filter(s => s.type === 'background');

        for (let i = 0; i < shapeCount; i++) {
            if (backgroundShapes.length === 0) break;
            const shape = pickRandom(backgroundShapes);
            // Replace placeholder colors and opacities with slightly more prominent values
            const paletteChoices = [palette.primary, palette.secondary, palette.accent].filter(Boolean);
            const shapeSvg = shape.svg
                .replace(/fill='rgba\([^)]+\)'/g, `fill='${pickRandom(paletteChoices)}' opacity='${(Math.random() * 0.22 + 0.08).toFixed(2)}'`) // Increased opacity
                .replace(/stroke='rgba\([^)]+\)'/g, `stroke='${pickRandom(paletteChoices)}' opacity='${(Math.random() * 0.30 + 0.12).toFixed(2)}'`); // Increased opacity
            backgroundSvg += shapeSvg;
        }
        return backgroundSvg;
    }

    renderTextElements(layout, palette, product) {
        let textSvg = '';
        const { brand_name, tagline, prestige_info } = layout;

        // Helper functions for fitting/wrapping
        const measure = (str, size) => String(str).length * (size * 0.6);
        const fitSingleLine = (text, base, min, allowed) => {
            let size = base;
            while (size > min && measure(text, size) > allowed) size -= 2;
            return Math.max(size, min);
        };
        const splitTwoLines = (text) => {
            const mid = Math.floor(text.length / 2);
            let split = text.lastIndexOf(' ', mid);
            if (split < 0) split = text.indexOf(' ', mid);
            if (split <= 0) return [text];
            return [text.slice(0, split), text.slice(split + 1)];
        };

        // If this layout is the classic-sample style, render using those fields
        if (layout.store_name || layout.brand_title || layout.model || layout.size || layout.auth_badge) {
            const canvasWidth = 1000;
            const margin = 40;

            // Store name top-left - INCREASED SIZE
            if (layout.store_name) {
                const storeName = String(process.env.STORE_NAME || 'Lasantha Tyre Traders');
                const allowed = (layout.store_name.text_anchor === 'start')
                    ? Math.max(200, canvasWidth - Number(layout.store_name.x) - margin)
                    : (layout.store_name.text_anchor === 'end')
                        ? Math.max(200, Number(layout.store_name.x) - margin)
                        : Math.floor(canvasWidth * 0.8);
                let size = fitSingleLine(storeName, 58, 32, allowed); // Increased from 42/22 to 58/32
                textSvg += `<text x="${layout.store_name.x}" y="${layout.store_name.y}" font-family="Roboto, sans-serif" font-size="${size}" font-weight="700" fill="${palette.text_primary}" text-anchor="${layout.store_name.text_anchor}">${escapeXML(storeName)}</text>`;
            }

            // Authorised Dealer badge top-right - REMOVED per user request
            // User: "caption eke authrized dealer kilala ona na"
            // if (layout.auth_badge) {
            //     const badge = process.env.AUTH_BADGE_TEXT || 'Authorised Dealer';
            //     const allowed = (layout.auth_badge.text_anchor === 'end')
            //         ? Math.max(200, Number(layout.auth_badge.x) - margin)
            //         : Math.floor(canvasWidth * 0.45);
            //     const size = fitSingleLine(badge, 30, 18, allowed);
            //     textSvg += `<text x="${layout.auth_badge.x}" y="${layout.auth_badge.y}" font-family="Roboto, sans-serif" font-size="${size}" font-weight="600" fill="${palette.secondary}" text-anchor="${layout.auth_badge.text_anchor}">${escapeXML(badge)}</text>`;
            // }

            // Big brand title - IMPROVED SIZE AND SPACING
            if (layout.brand_title) {
                const title = String(product.brand || 'PREMIUM').toUpperCase();
                const allowed = (layout.brand_title.text_anchor === 'start')
                    ? Math.max(220, canvasWidth - Number(layout.brand_title.x) - margin - 540) // Account for tyre image width
                    : Math.floor(canvasWidth * 0.9);
                let size = fitSingleLine(title, 86, 48, allowed); // Reduced max to avoid overlap with image
                // Try wrap into two lines if still too long
                let lines = [title];
                if (measure(title, size) > allowed) {
                    lines = splitTwoLines(title);
                    while (size > 50 && Math.max(measure(lines[0], size), measure(lines[1] || '', size)) > allowed) size -= 2;
                }
                lines.forEach((ln, idx) => {
                    textSvg += `<text x="${layout.brand_title.x}" y="${layout.brand_title.y + idx * (size + 12)}" font-family="Roboto, sans-serif" font-size="${size}" font-weight="900" fill="${palette.text_primary}" text-anchor="${layout.brand_title.text_anchor}">${escapeXML(ln)}</text>`;
                });
            }

            // Brand origin/country just beneath brand title
            if (layout.prestige_info) {
                const origin = product.brandKnowledge?.origin || ({
                    BRIDGESTONE: 'Japan',
                    YOKOHAMA: 'Japan',
                    MAXXIS: 'Taiwan',
                    KUMHO: 'South Korea',
                    GT: 'Singapore',
                    GITI: 'Singapore',
                    DURATURN: 'Thailand',
                    MARSHAL: 'South Korea'
                })[String(product.brand || '').toUpperCase()] || '';
                if (origin) {
                    const text = `${String(product.brand).toUpperCase()} • ${origin.toUpperCase()}`;
                    const allowed = Math.max(200, canvasWidth - Number(layout.prestige_info.x) - margin);
                    const fs = fitSingleLine(text, 34, 20, allowed);
                    textSvg += `<text x="${layout.prestige_info.x}" y="${layout.prestige_info.y}" font-family="Roboto, sans-serif" font-size="${fs}" font-weight="700" fill="${palette.secondary}" text-anchor="${layout.prestige_info.text_anchor}">${escapeXML(text)}</text>`;
                }
            }

            // Model line (primary color highlight) - IMPROVED SPACING
            if (layout.model) {
                const raw = String(product.name || '').replace(/\s+/g, ' ').trim();
                let model = raw;
                if (product.brand) {
                    const reBrand = new RegExp(product.brand, 'i');
                    model = model.replace(reBrand, '').trim();
                }
                // Remove size tokens like 195/65R15
                model = model.replace(/\b\d{3}\/\d{2}R\d{2}\b/i, '').trim();
                if (!model) model = raw || 'Ecopia EP150';
                const allowed = Math.max(220, canvasWidth - Number(layout.model.x) - margin - 540); // Account for tyre image
                const size = fitSingleLine(model, 58, 30, allowed); // Adjusted for better fit
                textSvg += `<text x="${layout.model.x}" y="${layout.model.y}" font-family="Roboto, sans-serif" font-size="${size}" font-weight="800" fill="${palette.primary}" text-anchor="${layout.model.text_anchor}">${escapeXML(model)}</text>`;
            }

            // Size line - INCREASED SIZE (user: "poster ekata tyre size add wenna ba")
            if (layout.size) {
                let sizeMatch = '';
                const src = `${product.size || ''} ${product.name || ''}`;
                const m = src.match(/\b\d{3}\/\d{2}R\d{2}\b/i);
                if (m) sizeMatch = m[0].toUpperCase();
                const text = sizeMatch || (product.size ? String(product.size) : '').toUpperCase() || '';
                if (text) {
                    const allowed = Math.max(200, canvasWidth - Number(layout.size.x) - margin);
                    const fs = fitSingleLine(text, 54, 30, allowed); // Increased from 42/22 to 54/30
                    textSvg += `<text x="${layout.size.x}" y="${layout.size.y}" font-family="Roboto, sans-serif" font-size="${fs}" font-weight="700" fill="${palette.text_primary}" text-anchor="${layout.size.text_anchor}">${escapeXML(text)}</text>`;
                }
            }

            // Taglines (Sinhala + English) under size line if defined - INCREASED SIZE & MULTI-LINE SUPPORT
            if (layout.tagline) {
                let si = process.env.TAGLINE_SI || '';
                let en = process.env.TAGLINE_EN || '';
                if (!si && this.libraries.taglines && this.libraries.taglines.length > 0) {
                    const siList = this.libraries.taglines.filter(t => t.lang === 'si');
                    if (siList.length) {
                        const item = pickRandom(siList);
                        si = item.text;
                        const idx = this.libraries.taglines.indexOf(item);
                        if (!en && idx + 1 < this.libraries.taglines.length && this.libraries.taglines[idx + 1].lang === 'en') {
                            en = this.libraries.taglines[idx + 1].text;
                        }
                    }
                }
                if (!si) si = 'සෑම කිලෝමීටරයකටම වටිනාකමක්';
                if (!en) en = 'Value in Every Kilometer';

                const allowed = Math.max(380, canvasWidth - Number(layout.tagline.x) - margin - 20);
                
                // Multi-line support for long Sinhala captions
                const wrapText = (text, maxWidth, fontSize) => {
                    const words = text.split(' ');
                    const lines = [];
                    let currentLine = words[0];
                    
                    for (let i = 1; i < words.length; i++) {
                        const testLine = currentLine + ' ' + words[i];
                        if (measure(testLine, fontSize) <= maxWidth) {
                            currentLine = testLine;
                        } else {
                            lines.push(currentLine);
                            currentLine = words[i];
                        }
                    }
                    lines.push(currentLine);
                    return lines;
                };
                
                const siSize = fitSingleLine(si, 36, 22, allowed); // Adjusted for better readability
                const siLines = wrapText(si, allowed, siSize);
                let currentY = layout.tagline.y;
                
                siLines.forEach((line, idx) => {
                    textSvg += `<text x="${layout.tagline.x}" y="${currentY}" font-family="'Noto Sans Sinhala', sans-serif" font-style="italic" font-size="${siSize}" font-weight="700" fill="${palette.primary}" text-anchor="${layout.tagline.text_anchor}">${idx === 0 ? '"' : ''}${escapeXML(line)}${idx === siLines.length - 1 ? '"' : ''}</text>`;
                    currentY += siSize + 10;
                });
                
                const enSize = fitSingleLine(en, 26, 16, allowed);
                textSvg += `<text x="${layout.tagline.x}" y="${currentY + 16}" font-family="Roboto, sans-serif" font-style="italic" font-size="${enSize}" font-weight="500" fill="${palette.text_secondary}" text-anchor="${layout.tagline.text_anchor}">"${escapeXML(en)}"</text>`;
            }

            // Skip dual-language taglines for classic layout
            return textSvg;
        }

        // Store Name (requested to appear at the top instead of the big brand word)
        const storeNameRaw = process.env.STORE_NAME || 'Lasantha Tyre Traders';
        const titleBrand = String(storeNameRaw).toUpperCase();
    const canvasWidthBrand = 1000;
        const marginBrand = 40;
        const allowedBrandWidth = (brand_name.text_anchor === 'start')
            ? Math.max(200, canvasWidthBrand - Number(brand_name.x) - marginBrand)
            : (brand_name.text_anchor === 'end')
                ? Math.max(200, Number(brand_name.x) - marginBrand)
                : Math.floor(canvasWidthBrand * 0.9);

        const approxWidth = (str, size) => str.length * (size * 0.6);

        // Try one-line fit first - INCREASED BASE SIZE (user: "akuru podi wadi")
        let brandSize = 180; // Increased from 150
        while (brandSize > 36 && approxWidth(titleBrand, brandSize) > allowedBrandWidth) { // Increased min from 28 to 36
            brandSize -= 2;
        }

        let brandLines = [titleBrand];
        if (approxWidth(titleBrand, brandSize) > allowedBrandWidth) {
            // Try two-line wrap at nearest space to middle
            const mid = Math.floor(titleBrand.length / 2);
            let split = titleBrand.lastIndexOf(' ', mid);
            if (split < 0) split = titleBrand.indexOf(' ', mid);
            if (split > 0) {
                brandLines = [titleBrand.slice(0, split), titleBrand.slice(split + 1)];
                // Fit based on longest line
                brandSize = Math.min(brandSize, 140); // Increased from 120
                while (brandSize > 34 && Math.max( // Increased min from 26 to 34
                    approxWidth(brandLines[0], brandSize),
                    approxWidth(brandLines[1], brandSize)
                ) > allowedBrandWidth) {
                    brandSize -= 2;
                }
            }
        }

        const brandFill = palette.text_primary;
        const brandFont = "Roboto, sans-serif";
        const brandWeight = "bold";
        const brandX = brand_name.x;
        let brandY = brand_name.y;
        brandLines.forEach((line, idx) => {
            const safe = escapeXML(line);
            textSvg += `<text x="${brandX}" y="${brandY + idx * (brandSize + 10)}" font-family="${brandFont}" font-size="${brandSize}" font-weight="${brandWeight}" fill="${brandFill}" text-anchor="${brand_name.text_anchor}">${safe}</text>`;
        });

        // Prestige Info (Tier, Origin) - SAFE CONSTRUCTION
        let prestigeParts = [];
        if (product.brandKnowledge) {
            if (product.brandKnowledge.tier) {
                prestigeParts.push(escapeXML(product.brandKnowledge.tier.toUpperCase()));
            }
            if (product.brandKnowledge.origin) {
                prestigeParts.push(escapeXML(product.brandKnowledge.origin.toUpperCase()));
            }
        }
        const prestigeText = prestigeParts.join(' | ');
        if (prestigeText) {
            textSvg += `<text x="${prestige_info.x}" y="${prestige_info.y}" font-family="Roboto, sans-serif" font-size="32" font-weight="bold" fill="${palette.primary}" text-anchor="${prestige_info.text_anchor}">${prestigeText}</text>`;
        }

        // Tagline - allow overrides then fallback to library pairs
        let sinhalaTagline = process.env.TAGLINE_SI || '';
        let englishTagline = process.env.TAGLINE_EN || '';

        if (!sinhalaTagline && this.libraries.taglines && this.libraries.taglines.length > 0) {
            const siTaglines = this.libraries.taglines.filter(t => t.lang === 'si');
            if (siTaglines.length > 0) {
                const randomSiTagline = pickRandom(siTaglines);
                sinhalaTagline = randomSiTagline.text;
                const siIndex = this.libraries.taglines.indexOf(randomSiTagline);
                if (!englishTagline && siIndex + 1 < this.libraries.taglines.length && this.libraries.taglines[siIndex + 1].lang === 'en') {
                    englishTagline = this.libraries.taglines[siIndex + 1].text;
                }
            }
        }

        // Fallback defaults (use the requested value statement in Sinhala as default)
        if (!sinhalaTagline) {
            sinhalaTagline = 'සෑම කිලෝමීටරයකටම වටිනාකමක්';
        }
        if (!englishTagline) {
            englishTagline = 'Value in Every Kilometer';
        }

        // Helper to fit text within available width by reducing font size
        const fitFontSize = (str, base, min, maxWidthPx) => {
            let size = base;
            const k = 0.6; // approximate width factor per char
            while (size > min && (str.length * size * k) > maxWidthPx) {
                size -= 1;
            }
            return size;
        };

        const canvasWidth = 1000; // matches generateProductPoster
        const margin = 40;
        const allowedWidth = (tagline.text_anchor === 'start')
            ? Math.max(200, canvasWidth - Number(tagline.x) - margin)
            : (tagline.text_anchor === 'end')
                ? Math.max(200, Number(tagline.x) - margin)
                : Math.floor(canvasWidth * 0.86); // centered: allow wider

        if (sinhalaTagline) {
            const siSize = fitFontSize(sinhalaTagline, 42, 24, allowedWidth); // Increased from 32/18 to 42/24
            textSvg += `<text x="${tagline.x}" y="${tagline.y}" font-family="'Noto Sans Sinhala', sans-serif" font-style="italic" font-size="${siSize}" font-weight="bold" fill="${palette.primary}" text-anchor="${tagline.text_anchor}">"${escapeXML(sinhalaTagline)}"</text>`;
        }
        if (englishTagline) {
            // Position the English tagline slightly below the Sinhala one
            const englishY = parseInt(tagline.y) + 48; // Increased spacing from 40 to 48
            const enSize = fitFontSize(englishTagline, 30, 20, allowedWidth); // Increased from 24/16 to 30/20
            textSvg += `<text x="${tagline.x}" y="${englishY}" font-family="Roboto, sans-serif" font-style="italic" font-size="${enSize}" font-weight="normal" fill="${palette.text_secondary}" text-anchor="${tagline.text_anchor}">"${escapeXML(englishTagline)}"</text>`;
        }
        
        return textSvg;
    }

    renderLogoOnly(layout, palette) {
        let svg = '';
        // Render only the logo (for caption-only mode)
        if (layout.logo) {
            const brandDir = path.join(__dirname, '..', 'assets', 'branding');
            const candidates = [];
            if (process.env.LOGO_PATH) candidates.push(process.env.LOGO_PATH);
            candidates.push(
                path.join(brandDir, 'lasantha-logo.png'),
                path.join(brandDir, 'lasantha-logo.jpg'),
                path.join(brandDir, 'logo.png')
            );
            const fsSync = require('fs');
            const found = candidates.find(p => {
                try { return fsSync.existsSync(p) && fsSync.statSync(p).isFile(); } catch { return false; }
            });
            if (found) {
                try {
                    const logoBuf = fsSync.readFileSync(found);
                    const ext = (path.extname(found).slice(1) || 'png').toLowerCase();
                    const b64 = logoBuf.toString('base64');
                    svg += `\n<image href="data:image/${ext};base64,${b64}" x="${layout.logo.x}" y="${layout.logo.y}" width="${layout.logo.width}" height="${layout.logo.height}" preserveAspectRatio="xMidYMid meet" />`;
                } catch (e) {
                    console.warn('⚠️ Logo unreadable:', e.message);
                }
            }
        }
        return svg;
    }

    renderCaptionCentered(caption, palette, width = 1000, height = 1000) {
        // Inline caption rendering for centered layout (caption-only mode)
        const escapeXML = (str) => String(str).replace(/[<>&'"]/g, c => ({'<':'&lt;','>':'&gt;','&':'&amp;','\'':'&apos;','"':'&quot;'}[c]));
        
        // Dynamic font size based on caption length (auto-scaling)
        const baseSize = 40;
        const minSize = 22;
        let fontSize = baseSize;
        
        // Simple word wrapping
        const words = String(caption || '').split(' ');
        const maxLineWidth = Math.floor(width * 0.85);
        const lines = [];
        let currentLine = '';
        
        for (const word of words) {
            const testLine = currentLine ? `${currentLine} ${word}` : word;
            const estimatedWidth = testLine.length * (fontSize * 0.6);
            
            if (estimatedWidth > maxLineWidth && currentLine) {
                lines.push(currentLine);
                currentLine = word;
            } else {
                currentLine = testLine;
            }
        }
        if (currentLine) lines.push(currentLine);
        
        // Adjust font size if too many lines
        if (lines.length > 8) {
            fontSize = Math.max(minSize, baseSize - Math.floor((lines.length - 8) * 2));
        }
        
        // Center vertically
        const lineHeight = fontSize * 1.5;
        const totalHeight = lines.length * lineHeight;
        let startY = (height - totalHeight) / 2 + fontSize;
        
        // Background rectangle for readability
        const bgPadding = 40;
        const bgY = startY - fontSize - bgPadding;
        const bgHeight = totalHeight + (bgPadding * 2);
        
        let svg = `<rect x="${width * 0.075}" y="${bgY}" width="${width * 0.85}" height="${bgHeight}" fill="${palette.background}" opacity="0.95" rx="20" />`;
        
        // Render text lines
        lines.forEach((line, idx) => {
            const y = startY + (idx * lineHeight);
            svg += `<text x="50%" y="${y}" font-family="'Noto Sans Sinhala', sans-serif" font-size="${fontSize}" font-weight="700" fill="${palette.text_primary}" text-anchor="middle">${escapeXML(line)}</text>`;
        });
        
        return svg;
    }

    renderCaptionOverlay(caption, layout, palette, width = 1000, height = 1000) {
        // Inline caption rendering for overlay layout (bottom overlay)
        const escapeXML = (str) => String(str).replace(/[<>&'"]/g, c => ({'<':'&lt;','>':'&gt;','&':'&amp;','\'':'&apos;','"':'&quot;'}[c]));
        
        // Smaller overlay caption
        const fontSize = 28;
        const words = String(caption || '').split(' ');
        const maxLineWidth = Math.floor(width * 0.9);
        const lines = [];
        let currentLine = '';
        
        for (const word of words) {
            const testLine = currentLine ? `${currentLine} ${word}` : word;
            const estimatedWidth = testLine.length * (fontSize * 0.6);
            
            if (estimatedWidth > maxLineWidth && currentLine) {
                lines.push(currentLine);
                currentLine = word;
            } else {
                currentLine = testLine;
            }
        }
        if (currentLine) lines.push(currentLine);
        
        // Position at bottom (above footer)
        const lineHeight = fontSize * 1.4;
        const totalHeight = lines.length * lineHeight;
        const bgPadding = 20;
        const footerHeight = 100;
        const bottomMargin = 20;
        const bgY = height - footerHeight - totalHeight - (bgPadding * 2) - bottomMargin;
        const bgHeight = totalHeight + (bgPadding * 2);
        
        let svg = `<rect x="${width * 0.05}" y="${bgY}" width="${width * 0.9}" height="${bgHeight}" fill="${palette.background}" opacity="0.9" rx="15" />`;
        
        // Render text lines
        let startY = bgY + bgPadding + fontSize;
        lines.forEach((line, idx) => {
            const y = startY + (idx * lineHeight);
            svg += `<text x="50%" y="${y}" font-family="'Noto Sans Sinhala', sans-serif" font-size="${fontSize}" font-weight="600" fill="${palette.text_primary}" text-anchor="middle">${escapeXML(line)}</text>`;
        });
        
        return svg;
    }

    renderLogoAndBrands(layout, palette, width = 1000, height = 1000, footerY = 900) {
        let svg = '';
        // --- Logo ---
        if (layout.logo) {
            // Try env path, then common defaults in assets/branding
            const brandDir = path.join(__dirname, '..', 'assets', 'branding');
            const candidates = [];
            if (process.env.LOGO_PATH) candidates.push(process.env.LOGO_PATH);
            candidates.push(
                path.join(brandDir, 'lasantha-logo.png'),
                path.join(brandDir, 'lasantha-logo.jpg'),
                path.join(brandDir, 'lasantha-logo.jpeg'),
                path.join(brandDir, 'lasantha-logo.webp'),
                path.join(brandDir, 'logo.png'),
                path.join(brandDir, 'logo.jpg'),
                path.join(brandDir, 'logo.webp')
            );
            const fsSync = require('fs');
            const found = candidates.find(p => {
                try { return fsSync.existsSync(p) && fsSync.statSync(p).isFile(); } catch { return false; }
            });
            if (found) {
                try {
                    const logoBuf = fsSync.readFileSync(found);
                    const ext = (path.extname(found).slice(1) || 'png').toLowerCase();
                    const b64 = logoBuf.toString('base64');
                    svg += `\n<image href="data:image/${ext};base64,${b64}" x="${layout.logo.x}" y="${layout.logo.y}" width="${layout.logo.width}" height="${layout.logo.height}" preserveAspectRatio="xMidYMid meet" />`;
                } catch (e) {
                    console.warn('⚠️ Logo unreadable:', e.message);
                }
            } else {
                console.warn('⚠️ Logo not found. Tried paths:', candidates.join(' | '));
            }
        }

        // --- Brand chips grid ---
        if (layout.brand_grid && Array.isArray(this.libraries.brands)) {
            const grid = layout.brand_grid;
            const names = this.libraries.brands.slice(0, grid.columns * 3); // up to rows we can fit

            // Compute rows and total grid dimensions
            const rows = Math.ceil(names.length / grid.columns);
            let pillW = grid.pill_w;
            let pillH = grid.pill_h;
            const totalW = (pillW * grid.columns) + (grid.gap_x * (grid.columns - 1));
            const availableW = Math.max(200, width - (grid.start_x * 2));
            if (totalW > availableW) {
                const scale = availableW / totalW;
                pillW = Math.floor(pillW * scale);
            }

            let fontSize = grid.font_size;
            // If scaled down a lot, reduce font size slightly
            if (pillW < grid.pill_w) {
                const scaleF = pillW / grid.pill_w;
                fontSize = Math.max(18, Math.floor(fontSize * (0.95 * scaleF + 0.05)));
            }

            const gridHeight = (rows * pillH) + ((rows - 1) * grid.gap_y);
            let startY = grid.start_y;
            const maxBottom = footerY - 16; // keep above footer
            if ((startY + gridHeight) > maxBottom) {
                startY = Math.max(60, maxBottom - gridHeight);
            }

            let x = grid.start_x;
            let y = startY;
            let col = 0;
            names.forEach((name, idx) => {
                const label = escapeXML(String(name));
                // pill background
                svg += `\n<rect x="${x}" y="${y}" rx="${grid.radius}" ry="${grid.radius}" width="${pillW}" height="${pillH}" fill="${palette.secondary}" opacity="0.9" />`;
                // text centered in pill
                const cx = x + pillW / 2;
                const cy = y + pillH / 2 + (fontSize * 0.35);
                svg += `\n<text x="${cx}" y="${cy}" font-family="Roboto, sans-serif" font-size="${fontSize}" font-weight="700" fill="${palette.text_primary}" text-anchor="middle">${label}</text>`;

                col++;
                if (col >= grid.columns) {
                    col = 0;
                    x = grid.start_x;
                    y += pillH + grid.gap_y;
                } else {
                    x += pillW + grid.gap_x;
                }
            });
        }
        return svg;
    }

    async generateProductPoster(product, options = {}) {
        try {
            await this.loadLibraries();

            const width = 1000, height = 1000;
            
            // Extract caption from options if provided
            const caption = options.caption || null;
            
            // Caption-only mode: skip product-specific elements (tyre image, brand, model, size)
            const captionOnlyMode = options.captionOnly || false;

            // --- 1. Dynamic Design Element Selection ---
            // Optional deterministic overrides via env (FORCE_LAYOUT / FORCE_PALETTE)
            const forceLayoutId = (process.env.FORCE_LAYOUT || '').trim();
            const forcePaletteId = (process.env.FORCE_PALETTE || '').trim();

            let layout = null;
            if (forceLayoutId) {
                layout = (this.libraries.layouts || []).find(l => String(l.id).toLowerCase() === forceLayoutId.toLowerCase()) || null;
                if (!layout) {
                    console.warn(`⚠️ FORCE_LAYOUT '${forceLayoutId}' not found. Falling back to random.`);
                }
            }
            if (!layout) layout = pickRandom(this.libraries.layouts);

            let palette = null;
            if (forcePaletteId) {
                palette = (this.libraries.palettes || []).find(p => String(p.id).toLowerCase() === forcePaletteId.toLowerCase()) || null;
                if (!palette) {
                    console.warn(`⚠️ FORCE_PALETTE '${forcePaletteId}' not found. Falling back to random.`);
                }
            }
            if (!palette) palette = pickRandom(this.libraries.palettes);
            const shapes = this.libraries.shapes;
            console.log(`🎨 Dynamically selected - Layout: ${layout.id}, Palette: ${palette.id}`);

            let svgElements = [];

                        // --- 2. SVG Defs and Fonts ---
                        // Use CDATA to avoid XML parsing issues with '&' in Google Fonts URL params
                        svgElements.push(`
                                <defs>
                                    <style><![CDATA[
                                        @import url('https://fonts.googleapis.com/css2?family=Noto+Sans+Sinhala:wght@400;700&family=Roboto:wght@700;900&display=swap');
                                    ]]></style>
                                </defs>
                        `);

            // If no forced layout, prefer classic-sample-green as default
            if (!forceLayoutId) {
                const classic = (this.libraries.layouts || []).find(l => String(l.id) === 'classic-sample-green');
                if (classic) layout = classic;
            }

            // If no forced palette, prefer fresh-mint-green as default
            if (!forcePaletteId) {
                const mint = (this.libraries.palettes || []).find(p => String(p.id) === 'fresh-mint-green');
                if (mint) palette = mint;
            }

            // --- 3. Render Background & Shapes ---
            svgElements.push(this.renderBackground(palette, shapes));

            // --- 4. Render Text (skip in caption-only mode) ---
            if (!captionOnlyMode) {
                svgElements.push(this.renderTextElements(layout, palette, product));
            }

            // --- 4b. Render Logo (always show) and Multi-Brand Grid (skip in caption-only mode) ---
            if (!captionOnlyMode) {
                svgElements.push(this.renderLogoAndBrands(layout, palette, width, height, height - 100));
            } else {
                // Caption-only mode: Show only logo
                svgElements.push(this.renderLogoOnly(layout, palette));
            }
            
            // --- 4c. Render Caption (if provided) ---
            if (caption) {
                if (captionOnlyMode) {
                    // Larger caption area for caption-only mode
                    svgElements.push(this.renderCaptionCentered(caption, palette, width, height));
                } else {
                    // Original overlay mode
                    svgElements.push(this.renderCaptionOverlay(caption, layout, palette, width, height));
                }
            }

            // --- 5. Image Processing and Embedding ---
            // Skip tyre image in caption-only mode (user: "tyre photos add karana eka nawatthanna")
            if (!captionOnlyMode) {
                const tyreImagePath = await this.findTyreImage(product);
                if (tyreImagePath) {
                    try {
                        const imgConfig = layout.tyre_image;
                        const imageBuffer = await fs.readFile(tyreImagePath);
                        const base64Image = imageBuffer.toString('base64');
                        const ext = path.extname(tyreImagePath).substring(1);

                        svgElements.push(`
                            <defs>
                              <clipPath id="imageClip">
                                <rect x="${imgConfig.x}" y="${imgConfig.y}" width="${imgConfig.width}" height="${imgConfig.height}" rx="30" />
                              </clipPath>
                            </defs>
                            <image href="data:image/${ext};base64,${base64Image}" x="${imgConfig.x}" y="${imgConfig.y}" width="${imgConfig.width}" height="${imgConfig.height}" clip-path="url(#imageClip)" preserveAspectRatio="xMidYMid slice" />
                        `);
                    } catch (imgErr) {
                        console.warn(`⚠️ Could not load or process image ${tyreImagePath}: ${imgErr.message}`);
                    }
                }
            }

            // --- 6. Footer --- Updated with user's contact: 0771222509, location: Thalawathugoda
            const footerY = height - 100;
            svgElements.push(`<rect x="0" y="${footerY}" width="${width}" height="100" fill="${palette.primary}" />`);
            const phone = escapeXML(process.env.STORE_PHONE || '0771222509'); // Updated default
            const location = escapeXML(process.env.STORE_LOCATION || 'Thalawathugoda'); // Updated default
            const footerText = `📞 ${phone} | 📍 ${location}`;
            svgElements.push(`<text x="50%" y="${footerY + 65}" text-anchor="middle" font-family="Roboto, sans-serif" font-size="44" font-weight="bold" fill="${palette.background}">${footerText}</text>`); // Increased from 40 to 44

            // --- 7. Assemble and Render SVG ---
            const finalSvg = `<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" xmlns="http://www.w3.org/2000/svg">${svgElements.join('')}</svg>`;
            
            // For debugging
            // await fs.writeFile(path.join(__dirname, '../facebook-drafts/debug-dynamic.svg'), finalSvg);

            return await sharp(Buffer.from(finalSvg))
                .jpeg({ quality: 90, progressive: true })
                .toBuffer();

        } catch (err) {
            console.error('❌ Dynamic SVG Product poster generation error:', err.message);
            console.error('❌ Stack trace:', err.stack);
            return Buffer.from(''); // Return empty buffer on failure
        }
    }
    
    // Backward compatibility methods
    async generateProductImage(productData) {
        return await this.generateProductPoster(productData);
    }
    async generateGenericPromotionalImage() {
        console.warn('⚠️ generateGenericPromotionalImage is deprecated. Returning empty image.');
        return Buffer.from('');
    }
}

module.exports = ImageGenerator;
