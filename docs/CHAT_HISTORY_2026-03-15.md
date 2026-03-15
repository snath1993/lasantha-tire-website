# Chat History — System Control Panel Development
## Date: March 15, 2026
## Project: Lasantha Tyre — WhatsApp Bot System
## Branch: copilot/improve-booking-page-functionality

---

## SESSION OVERVIEW

මේ chat sessions වලදී Electron desktop Control Panel app එකක් මුල සිට නිර්මාණය කර, debug කර, production-ready තත්වයට ගෙනෙන ලදී. App එකෙන් WhatsApp bot, PM2 services, scheduled jobs, database, configuration, library updates සියල්ල manage කරන්න පුළුවන්.

---

## PHASE 1: Initial Control Panel Creation
**Commit:** `dd109ab`

- Electron desktop app එක `apps/system-control-panel/` folder එකේ නිර්මාණය කරන ලදී
- Files 4ක් create කරන ලදී: `main.js`, `preload.js`, `renderer/index.html`, `renderer/app.js`
- package.json with electron 28 + electron-builder configured

---

## PHASE 2: Full v2.0 Rebuild
**Commit:** `48934aa`

පළමු version එකේ buttons ගොඩක් වැඩ කළේ නැත. පරිශීලකයාගේ ඉල්ලීම මත සියලුම files 4ම මකා නැවත ලියන ලදී:

### main.js (652 lines):
- Bot API communication with HTTP redirect support
- PM2 helpers (exec, list parsed)
- File read/write with automatic .bak backup
- Progress event system for renderer
- 26 IPC handlers covering all features
- Bot Swap (v1 ↔ v2) with safe failover
- Safe Library Update with automatic rollback
- Disk space monitoring

### preload.js (80 lines):
- Secure IPC bridge with contextBridge
- 30+ API methods exposed to renderer
- onUpdateProgress event listener

### renderer/index.html (651 lines):
- Premium dark glassmorphism UI
- 11 navigation pages (Overview, WhatsApp, Bot Swap, Services, Jobs, Database, Logs, Configuration, Library Update, Quick Actions, Settings)
- Custom title bar with window controls
- Status bar with real-time indicators
- Progress modal overlay
- Toast notification system
- QR code scanner display
- Config file editor
- Log viewer with syntax highlighting

### renderer/app.js (730 lines):
- Navigation system with page-enter handlers
- Auto-refresh system (configurable interval)
- 49 interactive buttons/handlers
- Service management (restart/stop/start individual + all)
- Job triggering and monitoring
- Database backup management
- Log viewing with search/filter
- Configuration editing with save + backup
- Library version checking and safe update
- Settings management with persistence
- Status bar real-time updates

---

## PHASE 3: Critical Bug Fixes
**Commit:** `fe98ab0`

### Bug 1: CSP Blocking ALL Inline Handlers
- **Root Cause:** Content-Security-Policy had `script-src 'self'` which blocked every `onclick` and `onchange` attribute in HTML
- **Impact:** 35+ buttons were completely dead (silently blocked)
- **Fix:** Added `'unsafe-inline'` to `script-src` directive
- **Only navigation worked** because it used `addEventListener` in app.js

### Bug 2: HTTP Redirect Not Followed
- **Root Cause:** `botApi()` used Node's `http.request` which doesn't follow redirects
- **Impact:** "Send PDF Now" button failed — server's `/api/admin/trigger/daily-sales-pdf` returns `res.redirect()` 
- **Fix:** Added redirect following (301/302/307/308) with up to 3 levels of recursion
- Also increased timeout from 10s to 15s

---

## PHASE 4: Production-Critical Fixes
**Commit:** `d6cb267`

### Bug 3: App Could Never Quit
- **Root Cause:** `mainWindow.on('close')` handler always called `e.preventDefault()` when `minimizeToTray` is on (default: ON), without checking `app.isQuitting`
- **Impact:** User clicks Quit from tray → app.quit() fires → close event fires → preventDefault blocks it → app never exits. Required Task Manager to kill.
- **Fix:** Added `!app.isQuitting` check before preventing close

### Bug 4: Missing Tray Icon
- **Root Cause:** `assets/` folder was empty — no `icon.png` file
- **Impact:** Tray icon was invisible on Windows → user couldn't right-click tray → couldn't access Quit/Show menu
- **Fix:** Generated proper 64x64 PNG icon with app color scheme (purple/blue/green concentric design)

### Bug 5: Progress Modal No Dismiss
- **Root Cause:** If library update IPC hangs (npm stuck, network down), user is trapped with undismissable overlay
- **Fix:** Added dismiss button that appears after 30 seconds as safety net, and when progress reaches 100%

### Bug 6: Silent Config Loading Failures
- **Root Cause:** Config page swallowed errors in empty try/catch → user sees blank editor
- **Fix:** Added "Loading..." indicator, proper error display, and caught error details shown to user

---

## DEPLOYMENT / MIGRATION DISCUSSION

### Question: Can this be built as a standalone exe?
**Answer:** Yes, `npx electron-builder --win` will create an NSIS installer (~80-120MB). But it's a Control Panel only — needs bot server, PM2, SQL Server running separately.

### Question: Can this entire project be moved to another PC?
**Answer:** Yes, with conditions:
- Same network: Just change `SQL_SERVER` IP in `.env`
- Same machine as DB: No `.env` changes needed at all
- Need to install: Node.js, PM2 (`npm i -g pm2`)
- Need to copy: `C:\Users\{user}\.cloudflared\` folder (tunnel credentials)
- WhatsApp session: Usually transfers, may need QR re-scan

### Question: Database server PC එකේම install කරනවා නම්?
**Answer:** Easiest option:
1. Node.js install
2. `npm i -g pm2`
3. Copy `C:\whatsapp-sql-api` folder
4. Copy `.cloudflared` from user profile
5. `pm2 start ecosystem.config.js`
6. total time ~20 minutes, `.env` changes 0

### Question: How to deliver updates?
**Answer:** Best method — Git:
- Dev PC: `git push`
- Server PC: `git pull` + `pm2 restart all`

---

## ARCHITECTURE OVERVIEW

```
C:\whatsapp-sql-api\
├── index.js                    — Main WhatsApp bot server (5000+ lines)
├── .env                        — All configuration (DB, API keys, settings)
├── ecosystem.config.js         — PM2 process manager config (10 services)
├── sqlConfig.js                — SQL Server connection setup
├── package.json                — Dependencies
│
├── apps/
│   ├── system-control-panel/   — Electron desktop app (THIS PROJECT)
│   │   ├── main.js             — Electron main process (652 lines)
│   │   ├── preload.js          — IPC bridge (80 lines)
│   │   ├── renderer/
│   │   │   ├── index.html      — UI (651 lines)
│   │   │   └── app.js          — Renderer logic (730 lines)
│   │   ├── assets/icon.png     — Tray icon
│   │   └── package.json        — Electron dependencies
│   ├── royal-booking-v2/       — Royal Booking web app
│   └── publish/                — Facebook publisher app
│
├── jobs/                       — Scheduled job classes
├── routes/                     — Express API routes (adminRoutes.js)
├── services/                   — Service modules
├── digital-invoice/            — Invoice PDF generation
├── lasantha-tire-v1.5/         — Dashboard (Next.js)
├── lasantha-tire-v2.0/         — Dashboard v2 (Next.js)
├── cloudflared.exe             — Cloudflare tunnel binary
├── python32-portable/          — Python 3.2 for Peachtree ODBC bridge
│
├── .wwebjs_auth_TEST_V3/       — WhatsApp session data
├── backups/sql_archives/       — Database backups
└── logs/                       — Application logs
```

## PM2 SERVICES (10 total):
1. `whatsapp-bot-v2` — Main bot (port 8585)
2. `lasantha-tire-dashboard` — Dashboard v1.5 (port 3028)
3. `lasantha-tire-v2.0` — Dashboard v2 (port 3029)
4. `peachtree-bridge` — Peachtree ODBC bridge (port 5001)
5. `royal-booking-v2` — Royal Booking (port 3099)
6. `lasantha-app-tunnel` — Cloudflare tunnel for app
7. `lasantha-bot-tunnel` — Cloudflare tunnel for bot
8. `lasantha-fb-tunnel` — Cloudflare tunnel for Facebook
9. `royal-booking-tunnel` — Cloudflare tunnel for booking
10. `whatsapp-bot` — Legacy bot v1 (stopped)

## KEY API ENDPOINTS:
- `GET /api/admin/overview` — Full system status
- `GET /api/admin/health` — Health check
- `GET /api/admin/whatsapp/qr` — QR code for scanning
- `POST /api/admin/whatsapp/reconnect` — Reconnect WhatsApp
- `GET /api/admin/trigger/:job` — Trigger scheduled jobs
- `GET /api/admin/config/:file` — Read config files
- `POST /api/admin/config/:file` — Write config files
- `POST /api/titan/message` — Send WhatsApp message

## COMMIT HISTORY:
```
d6cb267 fix: production-critical bugs — app quit, tray icon, progress modal, loading states
fe98ab0 fix: CSP blocking all inline handlers + botApi redirect support
48934aa feat: Control Panel v2.0 — full rebuild with Bot Swap, Safe Library Update, premium UI
dd109ab Add System Control Panel desktop application (Electron)
65c8099 Production upgrade: Admin dashboard, process monitoring, QR notifications
```

---

## CONTROL PANEL PAGES (11):

### 1. Overview
- System health badge (HEALTHY/DEGRADED/BOT API DOWN)
- 8 stat cards: WhatsApp, Database, Services, Jobs, Uptime, Memory, Restarts, Disk
- Recent job execution table
- Auto-refresh every 5 seconds

### 2. WhatsApp Management
- Connection status, engine version, reconnect count
- QR code scanner (auto-detects when QR is needed)
- Reconnect button, Hard Reset (clears session + restart)
- Send test message

### 3. Bot Swap (v1 ↔ v2)
- Visual side-by-side bot status cards
- One-click swap with safe failover
- Auto-rollback if target bot fails to start

### 4. Services (PM2)
- All 10 services in table with status, CPU, memory, restarts, uptime, PID
- Individual restart/stop/start per service
- Restart All / Stop All buttons

### 5. Scheduled Jobs
- Job execution status table (last run, status, duration, failures)
- Job configuration table (schedule, enabled/disabled)
- Quick trigger buttons: Send PDF Now, Backup Now, Daily Report

### 6. Database
- Connection status
- Backup Now button
- Backup file archive table (name, size, date)
- Open backup folder

### 7. Log Viewer
- Service selector dropdown
- Line count control
- Search/filter
- Syntax-highlighted log output (errors red, warnings yellow, success green)
- Copy to clipboard, Flush PM2 logs

### 8. Configuration Editor
- Dropdown: .env, jobs-config.json, ecosystem.config.js, watched-item-config.json, job-status.json
- Full text editor with save
- Auto-backup (.bak) before saving
- Save & Restart bot option

### 9. Library Update
- Current vs latest version display
- One-click safe update with progress modal
- Auto backup → stop bot → npm install → restart → health check → rollback if unhealthy
- Manual rollback option
- Update history log

### 10. Quick Actions
- Bot Control (restart/stop/start)
- Tunnels (restart all, individual)
- Jobs (trigger individually)
- Folders (open project root, backups, logs, WhatsApp session)
- Web Dashboard (open in browser)
- Diagnostics (health check, view logs)

### 11. Settings
- Bot API Port
- Dashboard Key (authentication)
- Refresh Interval (2s/5s/10s/30s/1min)
- Minimize to tray toggle
- Start minimized toggle
- System info display (Electron version, Node.js, platform, CPU, RAM)

---
*Document generated: March 15, 2026*
*By: GitHub Copilot (Claude Opus 4.6)*
