// TyrePhotoReplyJob
// Send tyre image(s) when user requests photos via text (e.g., "195/65R15 MAXXIS photo")

const sql = require('mssql');
const fs = require('fs');
// BAILEYS MIGRATION: Use wrapper instead of whatsapp-web.js
const { MessageMedia } = require('../utils/baileysWrapper');
const { extractTyreSizeFlexible } = require('../utils/detect');
const { extractConversationContext } = require('../utils/contextExtraction');
const { aiPool } = require('../utils/aiDbConnection');
const { fetchTyreImage, findAllTyreImages } = require('../utils/fetchTyreImage');
const { normalizeBrand, BRAND_NAME_MAPPING } = require('../utils/brandUtils');

// Brand detection regex (keep in sync with JobRegistry)
const BRAND_LIST = ['MAXXIS','DURATURN','BRIDGESTONE','MICHELIN','YOKOHAMA','CONTINENTAL','GOODYEAR','PIRELLI','DUNLOP','HANKOOK','CEAT','GITI','FEDERAL','KUMHO','NEXEN','TOYO'];
// Include common misspellings in regex
const BRAND_VARIANTS = [...BRAND_LIST, ...Object.keys(BRAND_NAME_MAPPING)];
const BRAND_REGEX = new RegExp(`\\b(${BRAND_VARIANTS.join('|')})\\b`, 'i');

// Extract a likely pattern name from ItemDescription or Custom3
// Also normalize any embedded brand names (e.g., MAXXIES -> MAXXIS)
function derivePattern(itemDescription, custom3) {
  // First check Custom3, but normalize it if it's a misspelled brand name
  if (custom3 && typeof custom3 === 'string' && custom3.trim().length >= 2) {
    const c3Trimmed = custom3.trim();
    const normalized = normalizeBrand(c3Trimmed);
    // If Custom3 is actually a brand name (typo like MAXXIES), skip it and extract from ItemDescription
    if (!BRAND_LIST.includes(normalized)) {
      return c3Trimmed;
    }
    // Otherwise, fall through to extract from ItemDescription
  }
  
  if (itemDescription) {
    // Split into tokens and filter out known brands
    const tokens = itemDescription.split(/\s+/).filter(t => t.length > 1);
    const patternTokens = [];
    
    for (const token of tokens) {
      // Skip size patterns (e.g., 195/65/15, 195/65R15)
      if (/^\d+[\/R]\d+/.test(token)) continue;
      
      // Normalize and check if it's a brand name
      const normalized = normalizeBrand(token);
      if (BRAND_LIST.includes(normalized)) continue;
      
      // Keep pattern-like tokens (uppercase with letters/numbers/dashes)
      if (/^[A-Z][A-Z0-9\-]*$/.test(token)) {
        patternTokens.push(token);
      }
    }
    
    if (patternTokens.length > 0) {
      // Prefer tokens with numbers or dashes (more likely to be pattern codes)
      const withNumberOrDash = patternTokens.filter(t => /[-0-9]/.test(t));
      if (withNumberOrDash.length > 0) {
        return withNumberOrDash.join(' ');
      }
      return patternTokens.join(' ');
    }
  }
  return null;
}

module.exports = async function TyrePhotoReplyJob(msg, deps) {
  const { sql: SQL, sqlConfig, allowedContacts, logAndSave, entities = {} } = deps || {};

  const rawText = (msg.body || '').trim();
  const senderNumber = msg.from.replace('@c.us', '');

  // Quick keyword gate: must mention photo-ish word
  const wantsPhoto = /(photo|image|පින්තූර|පින්තූරය|ඡායාරූප|pic|පට|එවන්න)/i.test(rawText);
  if (!wantsPhoto) return false; // Let others handle

  // Extract tyre size from entities or text
  let tyreSize = entities.tyreSize || extractTyreSizeFlexible(rawText);
  if (!tyreSize) {
    // No size → not our job
    return false;
  }

  // Extract brand from entities or text or conversation context
  let brand = entities.brand || (rawText.match(BRAND_REGEX)?.[1] || null);
  if (brand) {
    brand = normalizeBrand(brand.toUpperCase());
  }

  if (!brand) {
    try {
      // Look into recent conversation for brand context
      const historyResult = await aiPool.request()
        .input('phone', SQL.VarChar(50), senderNumber)
        .input('limit', SQL.Int, 10)
        .query('SELECT TOP (@limit) * FROM whatsapp_chat_history WHERE user_phone = @phone ORDER BY created_at DESC');

      const ctx = extractConversationContext(historyResult.recordset, rawText);
      if (ctx && ctx.brand) {
        brand = normalizeBrand(ctx.brand.toUpperCase());
        logAndSave && logAndSave(`[PhotoJob] Using brand from context: ${brand}`);
      }
    } catch (e) {
      // Non-fatal
      logAndSave && logAndSave(`[PhotoJob] Context lookup failed: ${e.message}`);
    }
  }

  try {
    await SQL.connect(sqlConfig);

    // Find candidate tyres for this size (and brand if given)
    const sizeVariant = tyreSize.replace(/\//g, '');
    const request = new SQL.Request();
    request.input('size', SQL.NVarChar, `%${tyreSize}%`);
    request.input('size2', SQL.NVarChar, `%${sizeVariant}%`);
    if (brand) {
      request.input('brand', SQL.NVarChar, `%${brand}%`);
    }

    let q = `
      SELECT TOP 10 im.ItemDescription, im.Custom3
      FROM [View_Item Master Whatsapp] im
      WHERE im.Categoty = 'TYRES'
        AND (
          im.ItemDescription LIKE @size
          OR REPLACE(im.ItemDescription, '/', '') LIKE @size2
        )
    `;
    if (brand) {
      q += ` AND (im.ItemDescription LIKE @brand OR im.Custom3 LIKE @brand)`;
    }

    const result = await request.query(q);
    const items = result.recordset || [];

    if (items.length === 0) {
      await msg.reply(`Sorry, couldn't find a matching item for *${tyreSize}*${brand ? ' ' + brand : ''}.`);
      logAndSave && logAndSave(`[PhotoJob] No items found for ${tyreSize} ${brand || ''}`);
      return true; // handled with message
    }

    // Decide brand if still missing: infer from first matching description token
    if (!brand) {
      const desc = items[0].ItemDescription || '';
      const b = desc.match(BRAND_REGEX)?.[1];
      if (b) {
        brand = normalizeBrand(b.toUpperCase());
        logAndSave && logAndSave(`[PhotoJob] Inferred brand from item: ${brand}`);
      }
    }

    if (!brand) {
      // Ask user to specify a brand if we truly cannot determine
      await msg.reply(`Please specify a brand for *${tyreSize}* (e.g., MAXXIS, DURATURN) to send the correct photo.`);
      return true;
    }

    // Try candidates until we find images
    let sent = false;
    for (const it of items) {
      const pattern = derivePattern(it.ItemDescription, it.Custom3);
      if (!pattern) continue;

      // Find ALL images for this pattern (supports multiple photos)
      const images = findAllTyreImages(brand, pattern);
      
      if (!images || images.length === 0) {
        logAndSave && logAndSave(`[PhotoJob] No images for ${brand} ${pattern}`);
        continue;
      }

      logAndSave && logAndSave(`[PhotoJob] Found ${images.length} image(s) for ${brand} ${pattern}`);

      // Send all images for this pattern
      for (let i = 0; i < images.length; i++) {
        const image = images[i];
        logAndSave && logAndSave(`[PhotoJob] Processing image ${i+1}/${images.length}: ${image.local_path}`);

        // Build media from local file
        let media;
        if (image.local_path) {
          try {
            if (!fs.existsSync(image.local_path)) {
              logAndSave && logAndSave(`[PhotoJob] File not found: ${image.local_path}`);
              continue;
            }
            const fileBuffer = fs.readFileSync(image.local_path);
            const base64 = fileBuffer.toString('base64');
            const ext = image.local_path.split('.').pop().toLowerCase();
            const mimeMap = { jpg: 'image/jpeg', jpeg: 'image/jpeg', png: 'image/png', webp: 'image/webp' };
            const mime = mimeMap[ext] || 'image/jpeg';
            media = new MessageMedia(mime, base64, `${brand}-${pattern}-${i+1}.${ext}`);
          } catch (fileErr) {
            logAndSave && logAndSave(`[PhotoJob] File read error: ${fileErr.message}`);
            continue;
          }
        }

        if (media) {
          // Build caption only for the first image
          let caption = '';
          if (i === 0) {
            const desc = it.ItemDescription || '';
            caption = `📷 *${brand} ${pattern}* - ${tyreSize}`;
            
            const liMatch = desc.match(/(\d{2,3})([A-Z])(?:\s|$)/);
            if (liMatch) {
              caption += `\nLoad: ${liMatch[1]} | Speed: ${liMatch[2]}`;
            }
            
            const origin = desc.match(/(TAIWAN|THAILAND|CHINA|INDONESIA|JAPAN|KOREA|MALAYSIA|INDIA)/i);
            if (origin) {
              caption += `\nOrigin: ${origin[1]}`;
            }
            
            if (it.Custom3 && it.Custom3.trim() && !Object.keys(BRAND_NAME_MAPPING).includes(it.Custom3.toUpperCase())) {
              caption += `\nVariant: ${it.Custom3.trim()}`;
            }
            
            // Add note if multiple images available
            if (images.length > 1) {
              caption += `\n\n📸 ${images.length} photos available`;
            }
          }
          
          try {
            await msg.reply(media);
            if (caption) {
              await msg.reply(caption);
            }
            logAndSave && logAndSave(`[PhotoJob] Sent image ${i+1}/${images.length} for ${brand} ${pattern}`);
            sent = true;
            
            // Small delay between multiple images to avoid rate limiting
            if (i < images.length - 1) {
              await new Promise(resolve => setTimeout(resolve, 500));
            }
          } catch (e) {
            logAndSave && logAndSave(`[PhotoJob] Send failed for image ${i+1}: ${e.message}`);
          }
        }
      }

      if (sent) break; // Found and sent images, stop searching
    }

    if (!sent) {
      await msg.reply(`Sorry, couldn't find a photo for *${tyreSize}* ${brand}.`);
      logAndSave && logAndSave(`[PhotoJob] Exhausted candidates without image for ${tyreSize} ${brand}`);
    }

    return true;
  } catch (err) {
    logAndSave && logAndSave(`[PhotoJob] Error: ${err.message}`);
    try { await SQL.close(); } catch {}
    await msg.reply('Sorry, an error occurred while retrieving the photo.');
    return true;
  } finally {
    try { await SQL.close(); } catch {}
  }
};
