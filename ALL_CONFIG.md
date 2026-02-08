# Master Configuration & Documentation (ALL_CONFIG)

## 1. Project Overview
- **Project Name**: WhatsApp SQL API (Lasantha Tire Bot)
- **Root Directory**: `c:\whatsapp-sql-api`
- **Environment**: 
  - **OS**: Windows
  - **Host**: `Cashier-2` (Bot Context)
  - **Database Host**: `WIN-JIAVRTFMA0N` (SQL Server)

## 2. Database Configuration

### Connection Details
- **Server**: `WIN-JIAVRTFMA0N`
- **Databases**:
  - `LasanthaTire` (Main ERP Data)
  - `WhatsAppAI` (Bot Metadata & Logs)

### Key Database Views & Schemas
The bot relies heavily on SQL Views to abstract the ERP table structure.

#### A. `View_Sales report whatsapp`
Used for Daily Sales Reports. Note: Uses non-descriptive `Expr` columns which are mapped in the codebase (`ReOrderingJob.js`, `DailyTyreSalesReportJob.js`).

| Column Name | Mapped To (Logical Name) | Data Type | Notes |
| :--- | :--- | :--- | :--- |
| `Expr1` | **InvoiceDate** | Date | Transaction Date |
| `Expr2` | **InvoiceNo** | String | Invoice Number |
| `Expr3` | **ItemCode** | String | Item Code |
| `Expr4` | **Description** | String | Item Description |
| `Expr5` | **Qty** | Numeric | Sold Quantity |
| `Expr6` | **UnitPrice** | Currency | Unit Price |
| `Expr8` | **VehicleNo** | String | Vehicle Number |
| `Expr10` | **CustomerName** | String | Customer Name |
| `Expr12` | **Amount** | Currency | Line Total (Net) |
| `Expr13` | **LineDiscount** | Numeric | Discount Percentage |
| `Expr14` | **IsVoid** | Boolean/Int | Void Status (0/1) |

#### B. `View_Item Master Whatsapp`
Used for Item lookup, stock checking, and price inquiries.

| Column | Type | Description |
| :--- | :--- | :--- |
| `Code` | String | Unique Item Code |
| `Description` | String | Item Name / Specs |
| `Part No` | String | Alternate Part Number |
| `Selling Price` | Currency | Current Retail Price |
| `Cost Price` | Currency | Cost (Internal use) |
| `Location` | String | Warehouse/Bin Location |
| `Category` | String | Product Category (Tyre, Battery, etc.) |
| `Sub Category` | String | Sub-Category |

#### C. `View_Item Whse Whatsapp`
Used for checking stock levels per warehouse.

| Column | Type | Description |
| :--- | :--- | :--- |
| `ItemCode` | String | Links to Master `Code` |
| `WhseCode` | String | Warehouse Identifier (e.g., `01`, `02`) |
| `OnHand` | Numeric | Physical Stock Count |
| `IsActive` | Boolean | Active Status |

#### D. `View_Customer Master Whatsapp` (Verified Missing/Legacy)
*Note: Referenced in older code but currently returning "Invalid Object Name". Customer data is likely accessed via direct table joins or `View_Sales report whatsapp`.*

---

## 3. Job Registry & Scheduler
All automated tasks controlled by `scheduler.js` and `pm2`.

### Active Jobs

| Job Name | Schedule | Cron | Purpose | Target |
| :--- | :--- | :--- | :--- | :--- |
| **DailyTyreSalesReportJob** | Every 2 Hours | `0 */2 * * *` | Sales summary updates throughout the day. | Admin / Sales Group |
| **Sales Full Day Summary** | Daily 11:37 PM | `37 23 * * *` | Final sales total for the day. | Admin Group |
| **DatabaseBackupJob** | Daily 11:45 PM | `45 23 * * *` | **Double Safety Backup**. <br>1. SQL writes to `\\WIN-JIAVRTFMA0N\Sage\SQLBackups` <br>2. Bot copies to `c:\whatsapp-sql-api\backups\sql_archives` | Local Disk + Admin WhatsApp |
| **ReOrderingJob** | Daily 8:00 PM | `0 20 * * *` | Low stock alerts & Re-order suggestions. | Procurement / Admin |
| **ReOrderingJob (Fail-Safe)** | Daily 8:00 AM | `0 8 * * *` | Retry run if night job failed. | Procurement / Admin |
| **PeachtreeAgingReportJob** | Mondays 8:00 AM | `0 8 * * 1` | Weekly Debtors/Aging Report. | Admin |
| **DailyTyreSalesPdfSendJob** | Daily 7:00 AM | `0 7 * * *` | Sends yesterday's detailed PDF report. | Admin Group |
| **WatchedItemRealtimeJob** | Every 60s | `Interval` | Monitors high-priority items for stock changes. | Configured Listeners |
| **DailyFacebookPostJob** | Manual/Triggered | - | Auto-posts daily updates to Facebook Page. | Facebook Page |

### Disabled / Deprecated Jobs
- **DailySalesPDFReportJob** (9:00 PM slot - Disabled by user request)
- **FacebookCommentMonitorJob** (Replaced by Claude/AI responder)

---

## 4. File Structure & Organization

### Key Directories
- **`/jobs`**: Contains the logic for all scheduled tasks (e.g., `DatabaseBackupJob.js`, `ReOrderingJob.js`).
- **`/scripts`**: Utility scripts (e.g., `crystal/` for report extraction, `document_views.js` for schema docs).
- **`/backups`**: Local storage for database backup archives (`.bak` files).
- **`/apps`**: Sub-apps and microservices (e.g., `invoice-v2` for generating receipts).
- **`/utils`**: Shared helpers (`sqlPool.js` for DB connections, `waClientRegistry.js` for WhatsApp).

### Configuration Files
- `config.js` / `sqlConfig.js`: Main Database connection strings.
- `scheduler.js`: The central "brain" that initializes Cron jobs.
- `jobs-config.json`: Dynamic configuration for job tunables (retry times, recipients).
- `ecosystem.config.js`: PM2 process manager configuration.

---

## 5. Backup Strategy (Protocol: "Double Safety")
To prevent data loss in case of a server crash, the system uses a **Redundant Backup Strategy**:

1.  **Generation**: The Bot (Client) instructs the SQL Server (Remote) to generate a `.bak` file.
2.  **Remote Write**: SQL Server writes this file to its local disk (shared as `\\WIN-JIAVRTFMA0N\Sage\SQLBackups`).
3.  **Local Copy**: The Bot immediately copies this file to `C:\whatsapp-sql-api\backups\sql_archives`.
4.  **Retention**:
    *   **Remote File**: **KEPT** on the server (User specifically requested "server eke eka makanna epa").
    *   **Local File**: **KEPT** on the bot machine.
    *   **WhatsApp**: A copy is sent to the Admin's WhatsApp for immediate off-site access.

This ensures that if the Server crashes, we have a Local copy. If the Bot crashes, we have a Server copy.
