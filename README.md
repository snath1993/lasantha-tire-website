# WhatsApp Tyre System

Unified WhatsApp-driven tyre quotation, pricing, vehicle invoice lookup, quotation PDF generation, watched item realtime monitoring, incremental sales reporting, and secured dashboard (username + 2FA) with IPC control.

## 🛡️ Security Status

**WhatsApp-web.js Version**: 1.34.2 (Latest - Updated Nov 8, 2025)  
**Security Posture**: Active Monitoring + Mitigation  
**Risk Level**: LOW (Private network deployment with monitoring)

📄 **See [SECURITY_CONFIGURATION.md](./SECURITY_CONFIGURATION.md)** for complete security details and firewall setup instructions.

## Components
- WhatsApp Bot (`index.js`): Parses tyre sizes & vehicle numbers, sends price/qty replies, builds quotation PDFs.
- Pricing Engine (`jobs/TyrePriceReplyJob.js` + `utils/pricingConfig.js`): Centralized markups & rounding.
- Daily Incremental Sales Report (`jobs/DailyTyreSalesReportJob.js` + `scheduler.js`): De-duplicates and persists invoices to SQLite.
- Watched Item Realtime Monitor (`jobs/WatchedItemRealtimeJob.js`): Multi-pattern polling & admin alerts.
- IPC Command Layer (`utils/ipcCommandWatcher.js`): Dashboard → Bot commands (sales report, scheduler restart, send 2FA code).
- 2FA & Auth (`dashboard/src/app/api/auth/2fa/*`, `lib/auth.ts`, `middleware.ts`): Username/password + WhatsApp-delivered one-time code.
- Job Status Tracking (`utils/jobStatus.js` + `job-status.json`): Status for daily report & watch job.
- Persistence: MSSQL (operational), SQLite (reported invoices), filesystem (PDFs), JSON state (watch state, sent reports, 2FA pending).
- **Appointment Booking System**:
  - Database: `Appointments` table in SQL Server.
  - API: `/api/appointments/book` (Bot) and `/api/appointments` (Next.js Proxy).
  - Features: Reference number generation, WhatsApp confirmations, Group notifications.

## Dashboard (Next.js)
Located in `dashboard/` with API routes that read logs, quotations, invoices (SQLite), job status, and send commands via IPC.

Key routes:
```
/api/stats
/api/invoices
/api/quotations
/api/logs
/api/job-status
/api/jobs (POST action=trigger_sales_report|restart_scheduler)
```

## Environment Configuration
Copy `.env.example` → `.env` and adjust.

| Variable | Purpose |
|----------|---------|
| SQL_USER / SQL_PASSWORD / SQL_SERVER / SQL_DATABASE | MSSQL connectivity |
| QUOTE_DIR | Folder for generated PDFs + SQLite DB |
| ADMIN_NUMBERS | Admin numbers (raw; code appends @c.us) |
| REPORT_NUMBERS or DAILY_REPORT_NUMBERS | Recipients of scheduled sales report |
| LOG_MAX_BYTES | Rotate bot log after exceeding bytes |
| WATCH_INTERVAL_SECONDS | Poll interval for watched item job |
| INVOICE_FILTER_TYRE_ONLY | 1 to filter invoice lines to tyre items only |
| COST_ADD_BASE / COST_ADD_CREDIT / MIN_TYRE_COST | Pricing adjustments |
| DASHBOARD_PROTECT | 1 enable auth, 0 disable (dev) |
| ADMIN_USER / ADMIN_PASS | Primary credential pair |
| TWO_FA_RATE_LIMIT_SECONDS | Min seconds between 2FA code generations |
| TWO_FA_EXP_MINUTES | 2FA code lifetime |
| TWO_FA_TARGET | Number receiving the 2FA code (WhatsApp) |
| DAILY_FULL_REPORT_TIME | HH:MM 24h time for full-day tyre summary (default 20:29) |
| DASHBOARD_KEY | Optional extra auth key (future use) |
| **ADVANCED PROFIT CALCULATION** | |
| REPORT_INCLUDE_PROFIT | 1 enable advanced profit calculation for daily reports |
| REPORT_PROFIT_DEBUG | 1 enable detailed profit calculation logging |
| REPORT_SHOW_COST | 1 show cost details in reports |
| MONTHLY_PROFIT_INCLUDE | 1 enable advanced profit calculation for monthly reports |
| MONTHLY_PROFIT_DEBUG | 1 enable monthly profit calculation debugging |
| MONTHLY_SHOW_COST | 1 show cost details in monthly reports |
| SHOP_MANAGEMENT_GROUP_ID | WhatsApp Group ID for management notifications (no bot replies) |
| WHATSAPP_BOT_URL | URL for the bot API (e.g., https://bot.lasanthatyre.com) |

## Run

Bot only:
```
npm run bot
```

Dashboard only:
```
npm run dashboard
```

Run both (Windows PowerShell – simple parallel):
```
npm run dev:all
```

Scan the QR presented in terminal to authenticate WhatsApp session.

Dashboard URL: http://localhost:3000

## Manual Sales Report Trigger
Admin WhatsApp: send `salesreport` (case/space insensitive).

Dashboard button → IPC command file → bot processes within ~5s.

## Key Files
| Path | Description |
|------|-------------|
| `index.js` | WhatsApp client + message routing |
| `scheduler.js` | Cron + watch interval starter |
| `jobs/` | All discrete jobs (pricing, qty, invoices, reports, watch) |
| `utils/ipcCommandWatcher.js` | Reads dashboard-commands.json and dispatches actions |
| `utils/jobStatus.js` | File-based status updates |
| `job-status.json` | Current job status snapshot |
| `sent-daily-reports.json` | De-dup ledger for daily report invoices |
| `watched-item-config.json` | Patterns & thresholds config |
| `watched-item-state.json` | Runtime pattern state (last notified, totals) |
| `2fa-pending.json` | Pending 2FA codes (auto-expired & rate-limited) |
| `utils/waClientRegistry.js` | Exposes WhatsApp client to IPC for 2FA send |

## Job Status JSON Example
```json
{
  "DailyTyreSalesReportJob": {
    "schedulerStartedAt": "2025-10-05T05:30:00.000Z",
    "lastRun": "2025-10-05T06:00:00.000Z",
    "lastSuccess": true,
    "nextRun": "2025-10-05T08:00:00.000Z",
    "updatedAt": "2025-10-05T06:00:01.100Z"
  }
}
```

## Extending
- Add new job: create file in `jobs/`, call from `scheduler.js` or trigger via IPC.
- Add dashboard action: extend `/dashboard/src/app/api/jobs/route.ts` and add case in IPC watcher.
- Configure watched items: use dashboard Watch tab (patterns, min qty, admin numbers). Backend polls every WATCH_INTERVAL_SECONDS.

## Authentication & 2FA
1. User submits username/password → server validates → generates 6-digit code (rate limited & expiring).
2. Code sent via IPC to bot → WhatsApp message to `TWO_FA_TARGET`.
3. User enters code → session cookie (HMAC) issued.
4. Attempts: max 5 wrong tries; expired codes auto-cleaned.
5. Rate limit enforced (`TWO_FA_RATE_LIMIT_SECONDS`).

## Watched Item Flow
1. Configure patterns in dashboard (Watch tab) – persisted to JSON.
2. Interval job polls invoices for today; matches patterns (case-insensitive substring).
3. New invoice lines meeting minQty trigger WhatsApp alerts to admin numbers.
4. State stored to avoid duplicate notifications.
5. Job status updated with notifiedCount & lastRun.

## Safety & Notes
- Core business logic (pricing formulas, parsing) intentionally untouched during ancillary feature additions.
- IPC polling interval ~5s (adjust in `ipcCommandWatcher`).
- Log rotation active (size based).
- Consider adding alerting for job failures (future enhancement).
 
Environment extras:
```
LOG_MAX_BYTES=2000000        # Rotate bot log when exceeding bytes
WATCH_INTERVAL_SECONDS=60    # Poll interval for watched item job
INVOICE_FILTER_TYRE_ONLY=1   # Filter invoice API to show only tyre product lines (set 0 to disable)
```

## Advanced Profit Calculation System

The system includes sophisticated profit calculation capabilities for enhanced business intelligence:

### Daily Reports (DailyTyreSalesReportJob.js)
- **Size Token Extraction**: Automatically extracts tyre sizes (195/65R15, 165/70/14, etc.) from item descriptions
- **Intelligent Cost Lookup**: Queries `tblItemMaster` table for accurate cost data using extracted size patterns
- **Smart Matching Algorithm**: 
  - +3 points for exact size matches
  - +2 points for brand/model word matches
  - Prioritizes higher cost items when scores are equal
- **Caching System**: Optimizes performance with `_COST_CACHE` to store lookup results
- **Fallback Strategy**: Uses SQL-based `CostPrrice` if advanced lookup fails

<!-- MonthlyTyreSalesReportJob documentation removed -->
- **Same Advanced Logic**: Implements identical profit calculation method as daily reports
- **Enhanced Analytics**: Provides both basic and advanced profit calculations for comparison
- **Date-wise Breakdown**: Shows profit calculations per day with advanced cost data
- **Performance Metrics**: Tracks cache usage and calculation method success rates

### Environment Control
```bash
# Enable advanced profit calculation
REPORT_INCLUDE_PROFIT=1          # Daily reports
MONTHLY_PROFIT_INCLUDE=1         # Monthly reports

# Enable debugging output
REPORT_PROFIT_DEBUG=1            # Daily debug logs
MONTHLY_PROFIT_DEBUG=1           # Monthly debug logs

# Show cost details in reports
REPORT_SHOW_COST=1               # Daily cost visibility
MONTHLY_SHOW_COST=1              # Monthly cost visibility
```

### Technical Implementation
- **Size Pattern Matching**: Multiple regex patterns for various tyre size formats
- **Brand Recognition**: Intelligent word extraction and matching
- **Cost Scoring**: Weighted algorithm prioritizing accuracy over speed
- **Memory Optimization**: Smart caching with automatic cleanup
- **Error Handling**: Graceful fallback to basic calculations

### Benefits
- **Higher Accuracy**: Uses real-time cost data from inventory master table
- **Better Intelligence**: Identifies actual vs. recorded cost discrepancies  
- **Performance Monitoring**: Tracks calculation success rates and cache efficiency
- **Flexible Configuration**: Easy enable/disable via environment variables
- **Debugging Support**: Detailed logging for troubleshooting cost lookups

## Future Enhancements (Optional)
- Structured JSON logging
- Alert on job failure via WhatsApp
- Historical aggregates (weekly/monthly) dashboard
- Rate limiting on verify endpoint + lockout window config

## License
MIT
 
## One-Click Launch (Windows)
Added helper scripts in project root:

| Script | Purpose |
|--------|---------|
| `run_dashboard.bat` | Start only the dashboard (Next.js dev) |
| `run_bot.bat` | Start only the WhatsApp bot (index.js) |
| `run_all.bat` | Open two PowerShell windows: bot + dashboard |
| `install-service.ps1` | Install Cloudflare Tunnel as a Windows Service |
| `fix-service.ps1` | Fix permissions and restart Cloudflare Service |

Create desktop shortcuts:
1. Right‑click the desired `.bat` file → Send to → Desktop (create shortcut).
2. (Optional) Change icon: Right‑click shortcut → Properties → Change Icon… (select a custom `.ico`).

VS Code Tasks (via `.vscode/tasks.json`):
Open Command Palette → Run Task → choose `Dashboard Dev`, `Bot`, or `Bot + Dashboard (PS)`.

Environment reminder: make sure Node.js is on PATH. If double‑clicking a script fails with "node not recognized", reopen a new terminal or install Node.

---

## 🌐 Cloudflare Tunnel Setup

The system uses Cloudflare Tunnel to expose the local Bot API securely to the public internet (for the production website).

**Configuration:**
- Config File: `cloudflared-config.yml`
- Credentials: `tunnel-cert.json`
- Service Name: `Cloudflared` (Windows Service)
- Public URL: `https://bot.lasanthatyre.com` -> `http://localhost:8585`

**Management:**
- **Install**: Run `install-service.ps1` as Administrator.
- **Fix/Restart**: Run `fix-service.ps1` as Administrator.
- **Logs**: Check Windows Event Viewer (Application logs source `cloudflared`).

## 🚀 Facebook Auto-Post System (Production Ready)

### New: DailyFacebookPostJob

Automated Facebook post generation with AI (Claude 4.5 Haiku), brand-centric creativity, and production-grade quality assurance.

**Location:** `jobs/DailyFacebookPostJob.js`  
**Runs:** 8:30 AM daily (Asia/Colombo timezone)

### Features

#### 1. 🧪 Caption QA Linter
- Validates emojis (3-8), hashtags (3-8), phone, prices
- Auto-fixes issues before publishing
- Enforces mandatory hashtags & store details
- **Result:** Zero human touch-ups

#### 2. 📊 A/B Draft Variants
- Generates 2 copy variants with different hooks/tones
- Saves each variant as separate JSON in `facebook-drafts/`
- Admin picks favorite before publishing
- **Result:** Data-driven copy selection

#### 3. 📈 Engagement Bias
- Tracks post performance (likes, comments, shares, reach)
- Ranks brands by engagement (past 7 days)
- Boosts high-performing brands 2x in future selections
- **Result:** Self-improving system

### Configuration

Add to `.env`:
```env
# === AI CAPTION GENERATION ===
# Primary AI provider (gemini=fastest, claude=most reliable)
FB_POST_AI_PROVIDER=gemini                    # default: gemini (fast), option: claude (reliable)

# AI API Keys (both recommended for triple-layer protection)
GEMINI_API_KEY=AIzaSyD...                     # Gemini 2.5 Flash (fastest, creative)
ANTHROPIC_API_KEY=sk-ant-api03-...            # Claude 4.5 Haiku (reliable fallback)

# Disable AI completely (use only premium fallback captions)
FB_POST_DISABLE_AI=false                      # true = skip AI, use fallback captions only

# === CAPTION FALLBACK SYSTEM ===
# Triple-layer protection: Gemini → Claude → 40+ Fallback Captions
# Fallback captions: professional, creative, ready-made (fallback-captions.json)
# System NEVER fails - always generates quality content!

# QA Linter
ENABLE_QA_LINTER=true
MIN_EMOJIS=3
MAX_EMOJIS=8
MIN_HASHTAGS=3
MAX_HASHTAGS=8

# A/B Variants
ENABLE_AB_DRAFTS=true
AB_VARIANT_COUNT=2

# Engagement Bias
ENABLE_ENGAGEMENT_BIAS=true
ENGAGEMENT_LOOKBACK_DAYS=7
```

### AI Caption Generation Strategy

**🎯 Triple-Layer Protection** (Never Fails):
```
1️⃣ Gemini 2.5 Flash (Primary)
   - Fastest (6-8 seconds)
   - Highly creative
   - Advanced marketing psychology
   - Auto-detects: rate limits, quota errors, timeouts
   ❌ If fails → Instant switch to Claude

2️⃣ Claude 4.5 Haiku (Fallback)
   - Most reliable (15-20 seconds)
   - Professional quality
   - 3 retry attempts with progressive backoff
   - Handles: rate limits, server errors, auth issues
   ❌ If fails → Guaranteed fallback captions

3️⃣ Premium Fallback Captions (Always Works)
   - 40+ professional ready-made captions
   - Natural Sinhala, attractive messaging
   - Random selection for variety
   - Advanced marketing psychology
   ✅ NEVER FAILS - Always produces quality content!
```

**Configuration Options:**

| Setting | Effect | Default |
|---------|--------|---------|
| `FB_POST_DISABLE_AI=true` | Skip all AI, use 70+ fallback captions only | **true** ✅ |
| `FB_POST_DISABLE_AI=false` | Enable AI caption generation | false |
| `FB_POST_AI_PROVIDER=gemini` | Use Gemini first (fastest) | gemini |
| `FB_POST_AI_PROVIDER=claude` | Use Claude first (most reliable) | gemini |
| Both API keys set + AI enabled | Full triple protection (Gemini→Claude→Fallback) | - |
| Only Gemini key + AI enabled | Gemini → Fallback (dual protection) | - |
| Only Claude key + AI enabled | Claude → Fallback (dual protection) | - |
| No API keys or AI disabled | Fallback captions only (always works) | **Default** ✅ |

**💡 Recommended Setup:**
- **Default (AI Disabled)**: Use 70+ professional fallback captions - guaranteed, instant, no API costs
- **Enable AI**: Set `FB_POST_DISABLE_AI=false` and add API keys for creative AI-generated captions
- **Dashboard Control**: Can be enabled/disabled via `config/settings.json` without restart

### Quick Test
```powershell
# Test all 3 features
node test-production-features.js

# Run actual job
node jobs/DailyFacebookPostJob.js
```

### Output
Saves to `facebook-drafts/`:
- `post_20251023_082345.jpg` — main image
- `post_20251023_082345.json` — main caption (QA passed)
- `ab_*_v1.json`, `ab_*_v2.json` — variant copies
- `ab_*_comparison.json` — side-by-side comparison

### Workflow
1. **8:30 AM:** Job generates post (caption validated, 2 variants created)
2. **WhatsApp Preview:** Admin receives image + main caption
3. **Admin Choice:** Pick main or variant copy to publish
4. **Record Engagement:** 1-2 days later, note likes/comments/shares
5. **Auto-Learn:** Next post automatically boosts winning brands

### Documentation

- **Quick Start:** See `QUICK_START_FEATURES.md`
- **Full Guide:** See `PRODUCTION_FEATURES_GUIDE.md`
- **Status:** See `PRODUCTION_READINESS.md`

### What's Included

| File | Purpose |
|------|---------|
| `utils/CaptionLinter.js` | QA validation & auto-fix |
| `utils/ABVariants.js` | A/B variant generation |
| `utils/EngagementBias.js` | Engagement tracking |
| `jobs/DailyFacebookPostJob.js` | Main post job (updated) |
| `test-production-features.js` | Feature validation |
| `QUICK_START_FEATURES.md` | Quick reference |
| `PRODUCTION_FEATURES_GUIDE.md` | Full documentation |
| `PRODUCTION_READINESS.md` | Status & checklist |

## 🤖 AI-Powered Facebook Automation (Claude-only)

### 1️⃣ **Comment Responder** (Public Comments)
Smart, safe replies to Facebook post comments with **strict no-price-in-public policy**.

**Location:** `jobs/FacebookCommentResponderJob.js`

**Features:**
- ✅ **Intent Classification** (PRICE_REQUEST, AVAILABILITY, VEHICLE_INQUIRY, SERVICE_REQUEST, COMPLAINT, PRAISE, GENERAL)
- ✅ **Entity Extraction** (tyre size, brand, vehicle number using Claude AI)
- ✅ **Smart Public Replies** (context-aware, no prices, natural language)
- ✅ **Private DMs** (detailed info with WhatsApp CTA)
- ✅ **Vehicle Number Support** (invoice history lookup)
- ✅ **Multi-language** (Sinhala/English auto-detection)
- ✅ **Business Rules Aware** (Store identity, hours, services)

### 2️⃣ **Messenger Responder** (Private Inbox)
Natural conversations in Facebook Messenger inbox with **price sharing allowed** (private conversations).

**Location:** `jobs/FacebookMessengerResponderJob.js`

**Features:**
- ✅ **Conversational AI** (multi-turn context awareness)
- ✅ **Price Sharing** (allowed in private DMs, not public)
- ✅ **Natural Language** (Sinhala/English mixing)
- ✅ **Service Information** (Wheel Alignment, Balancing, Nitrogen)
- ✅ **Vehicle History** (invoice lookup for vehicle numbers)
- ✅ **Fallback Support** (WhatsApp redirect if info unavailable)

### 3️⃣ **Post Generation** (Daily 8:30 AM)
Creative, brand-aware Facebook posts with WhatsApp approval flow.

**Location:** `jobs/DailyFacebookPostJob.js`

**Features:**
- ✅ **Creative Content** (varied hooks, CTAs, styles)
- ✅ **Brand Knowledge** (origin, tier, features, tagline)
- ✅ **A/B Variants** (2-3 different versions)
- ✅ **QA Validation** (emojis, hashtags, phone check)
- ✅ **Image Generation** (themed posters)
- ✅ **Admin Approval** (WhatsApp preview before posting)

### Poster Templates & Layouts (New)

This system renders modern poster images using JSON-driven templates (no direct background images). Templates define colors, typography sizes, element positions, masks, and abstract shapes.

- Template files: `assets/poster-templates/templates.json` (curated) and `assets/poster-templates/generated.templates.json` (auto-generated)
- JSON schema and examples: see `assets/poster-templates/README.md`
- Direct background images are not used in the poster render. The external folder (e.g., `C:\Users\Cashi\Music`) is used only to infer structure for auto-templates via the analyzer.

Rotation and selection:
- Job picks a templateId each run and saves it in the post meta JSON for history.
- Recent templateIds are avoided (last ~8) to minimize repetition.
- Curated templates are preferred over auto-generated ones when available.

Forcing a template (optional):
- Internally we pass `templateId` to `ImageGenerator.generateProductPoster(...)`. You can pin a specific template for testing by wiring that id into the job call.

Helper scripts and tasks:
- Analyze references → structured templates
  - VS Code Task: “Analyze Poster Templates” (uses `POSTER_TEMPLATE_DIR=C:/Users/Cashi/Music` from tasks)
  - Script: `node scripts/analyze-templates.js`
- Render all template samples (curated + generated) to review visually
  - Script: `node scripts/generate-template-samples.js`
  - Output: `facebook-drafts/samples/*.jpg`

Environment:
- `POSTER_TEMPLATE_DIR` is set at task-level for Bot and Combined runs in `.vscode/tasks.json`. It is used only by the analyzer script, not by the renderer.

Notes:
- NO PRICE policy applies to captions/posters.
- Brand/Pattern image selection prefers exact pattern, falls back to brand; otherwise item skipped.
- Image masks supported: `circle` (default), `rounded`, `square`. Curated templates use these for variety and readability.

---

## 📋 Setup & Configuration

Enable via `.env`:
```env
# Required for Facebook Graph API
FACEBOOK_PAGE_ID=194559142002903
FACEBOOK_PAGE_ACCESS_TOKEN=EAAG...

# Claude AI Provider (required for all features)
ANTHROPIC_API_KEY=sk-ant-...
CLAUDE_MODEL=claude-3-haiku-20240307

# Comment Responder (Public Comments)
ENABLE_FB_COMMENT_RESPONDER=true
FB_COMMENT_SCAN_INTERVAL_SEC=45
FB_REPLY_LANGUAGE=si           # si|en (auto-detect per comment)
FB_DM_WHATSAPP_FALLBACK=true   # Include WhatsApp link in DMs

# Messenger Responder (Private Inbox)
ENABLE_FB_MESSENGER_RESPONDER=true
FB_MESSENGER_SCAN_INTERVAL_SEC=60

# Store Info (DO NOT CHANGE)
STORE_NAME="Lasantha Tyre Traders"
STORE_LOCATION=Thalawathugoda
STORE_PHONE=0721222509
STORE_HOURS="06:30-21:00"
STORE_WHEEL_ALIGNMENT_HOURS="07:30-18:00"

# Admin WhatsApp for Post Approvals
ADMIN_WHATSAPP_NUMBER=0771222509
```

---

## 🎯 **Claude AI Usage Summary**

| **Feature** | **Claude AI Role** | **What It Does** |
|---|---|---|
| **Comment Classification** | ✅ Primary | Intent detection, entity extraction, language detection |
| **Comment Public Replies** | ✅ NEW | Context-aware, natural replies (no prices) |
| **Comment Private DMs** | ❌ Template | Fixed templates (with extracted info) |
| **Messenger Conversations** | ✅ NEW | Multi-turn conversations, price sharing (private) |
| **Post Generation** | ✅ Primary | Creative content, hooks, CTAs, hashtags |
| **A/B Variants** | ✅ Primary | Different copy versions |
| **Brand Knowledge** | ✅ Enhanced | Uses brand research in posts |
| **Style Matching** | ✅ Enhanced | Analyzes & follows existing post style |

---

## 🚀 **Business Rules (Claude AI එකට දැනුම් දෙන ඒවා)**

```javascript
// මෙම rules Claude AI එක automatically follow කරනවා:
1. මිල public comments වල mention නොකරන්න (STRICT)
2. Messenger DMs වල මිල දෙන්න පුළුවන් (Private conversations)
3. Store identity කවදාවත් වෙනස් නොකරන්න
4. Customer භාෂාවට match වෙන විදිහට reply කරන්න
5. Vehicle numbers හඳුනාගෙන invoice history check කරන්න කියන්න
6. Service requests වලට Wheel Alignment/Balancing/Nitrogen mention කරන්න
7. Complaints වලට professional විදිහට apologize කරලා resolve කරන්න
8. Uncertain නම් WhatsApp redirect කරන්න
```

---

## 📊 **Analytics & Tracking**

- **Comment History:** `data/comment-history.json` (intent, brand, size, vehicle, timestamps)
- **Messenger History:** `data/messenger-history.json` (conversations, analytics)
- **Post Metadata:** `facebook-drafts/post_TIMESTAMP.json` (QA results, A/B variants)

---

## 🔧 **Troubleshooting**

### Claude API Errors
- Check `ANTHROPIC_API_KEY` validity
- Verify rate limits not exceeded
- Fallback to templates if Claude unavailable

### Facebook Permissions
- Ensure `pages_messaging` permission for DMs
- Token must have `pages_read_engagement` for comments/messages

### Vehicle Number Detection
- Format: CBH-6483, WP-1234, etc.
- Detection: Claude AI + regex fallback
- Action: Offer invoice history lookup via DM

---

## 🎯 **Example Flows**

### Flow 1: Price Request Comment
```
Customer: "195/65R15 මිල කීයද?"
Claude: [Classifies as PRICE_REQUEST, extracts size]
Public Reply (AI): "විස්තර DM එකකට එවලා තිබේ ✅ WhatsApp: 0721222509"
Private DM (Template): "Brand/Size info + WhatsApp link"
```

### Flow 2: Vehicle Inquiry Comment
```
Customer: "CBH-6483 purchase history එක තියෙනවද?"
Claude: [Classifies as VEHICLE_INQUIRY, extracts vehicle number]
Public Reply (AI): "ඔබේ vehicle එකේ details check කරලා DM එකකින් දෙන්නම් ✅"
Private DM (Template): "Vehicle: CBH-6483. WhatsApp එකට message එකක් දාන්න..."
```

### Flow 3: Messenger Conversation
```
Customer (DM): "BRIDGESTONE 195/65R15 තියනවද?"
Claude: [Conversational, checks price]
Reply: "ඔව්, තියෙනවා! 🚗 
BRIDGESTONE 195/65R15
Pattern: ECOPIA EP150
Price: Rs. 21,500
Warranty: 3 years
Call/WhatsApp: 0721222509"
```

---

## 🛠️ **Legacy Code (Disabled)**

- `FacebookCommentMonitorJob.js` (Ollama-based) → Replaced by Claude
- Manual template replies → Now AI-generated

---

Notes:
- PriceProvider is stubbed today (`utils/PriceProvider.js`); DM will ask to continue on WhatsApp until SQL pricing is wired.
- Legacy Ollama-based monitor is disabled in the scheduler in favor of Claude.
- Requires pages_messaging permission for `private_replies` endpoint.
