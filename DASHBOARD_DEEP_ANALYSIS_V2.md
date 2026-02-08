# Dashboard Deep Analysis: Lasantha Tire ERP & Bot Manager

## 1. Overview
**Project:** `lasantha-tire` (Next.js 16)
**Status:** 🟢 Advanced & Production Ready
**Scope:** ERP (Sales/Inventory) + Bot Management + WhatsApp Web Clone.

You correctly identified that I initially analyzed the wrong dashboard. This report focuses on the main `lasantha-tire` application.

## 2. Current Capabilities
This dashboard is extremely powerful and well-integrated.

| Feature | Status | Details |
|---------|--------|---------|
| **Job Configuration** | ✅ Working | Edits `jobs-config.json`. The Bot watches this file and reloads automatically. |
| **WhatsApp Live** | ✅ Working | Full WhatsApp Web clone. Connects to Bot via SSE and API. |
| **ERP Analytics** | ✅ Working | Real-time Sales, Inventory, and Finance dashboards connected to SQL Server. |
| **Job Execution** | ❌ **Missing** | You can *configure* jobs, but you cannot *run* them manually from the UI. |
| **Live Logs** | ⚠️ Partial | `terminal-logs` page exists but needs verification of SSE connection. |

## 3. The "Missing Link": Manual Job Control
While you can change *when* a job runs (Schedule), you cannot say "Run Backup Now".
- **Backend Support:** The Bot (`index.js`) has `POST /api/jobs/run` to trigger jobs immediately.
- **Frontend Gap:** The `JobsPage` (`src/app/jobs/page.tsx`) lacks a "Run Now" button.
- **Service Gap:** `src/services/job-manager.ts` contains empty `TODO` methods and is not connected to the Bot's API.

## 4. Recommended Development Roadmap

### Phase 1: Enable Manual Job Execution (High Priority)
*Goal: Run "Daily Sales Report" or "Backup" immediately from the UI.*
1.  **Add "Run Now" Button:**
    - Update `src/app/jobs/page.tsx` to add a Play button next to each job.
    - Create a new API route `src/app/api/jobs/run/route.ts` that proxies the request to `http://localhost:8585/api/jobs/run`.
2.  **Show Last Run Status:**
    - Fetch real-time job status from `/api/job-execution-status` on the Bot.
    - Display "Last Run: 5 mins ago (Success)" on the job card.

### Phase 2: Enhanced Observability
1.  **Live Job Logs:**
    - When a job is running, show its specific logs in a modal (using the `job_status` SSE event).
2.  **System Health:**
    - Add a "Bot Health" widget to the header showing CPU/RAM and Uptime (from `/api/stats`).

### Phase 3: AI Chat History
1.  **AI Conversations:**
    - Create a page to view `whatsapp_chat_history` from the SQL database.
    - See how the AI replied to customers vs. how a human would.

## 5. Immediate Action Plan
I recommend we start by adding the **"Run Now"** functionality.
1.  Create `src/app/api/jobs/run/route.ts`.
2.  Update `src/app/jobs/page.tsx` to call this API when a "Run" button is clicked.
