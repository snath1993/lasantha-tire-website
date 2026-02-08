# Lasantha Tire v2.0 - Advanced ERP Dashboard

Production-ready dashboard for Lasantha Tire with Peachtree ERP integration, mobile-first UI, and real-time business intelligence.

## Features

### Core Dashboard
- **Modern UI/UX**: Clean, responsive design with sidebar layout
- **Sales Intelligence Hub**: Advanced analytics with drill-down capabilities
- **Job Manager**: Manual control over system jobs
- **AI History**: View past AI bot conversations
- **Real-time Stats**: Key performance indicators

### Peachtree ERP Integration
- **ODBC Bridge**: Python 32-bit bridge for Peachtree ODBC
- **Real-time Data**: Customers, vendors, GL accounts, transactions
- **Business Status**: AR/AP aging, cash balances, top customers/vendors
- **Multi-port Detection**: Auto-detects bridge on port 5000 or 5001

### Mobile-First Features
- **Full-screen Overlays**: Customers, vendors, accounts, reports
- **Sales Force Automation**: Inventory, quotes, orders
- **Financial Command Center**: AR/AP aging, cash position, profit tracking
- **Search & Filter**: Quick access to all data

### Security
- **Session-based Auth**: x-session-id header authentication
- **WebAuthn Support**: Passkey login option
- **Protected API Routes**: All ERP endpoints require authentication

## Getting Started

1. **Install Dependencies**:
   ```bash
   npm install
   ```

2. **Build for Production**:
   ```bash
   npm run build
   ```

3. **Run Production Server**:
   ```bash
   npm start
   ```

   Dashboard available at `http://localhost:3029`

## PM2 Production Deployment

```bash
# Start via ecosystem config (from parent directory)
pm2 start ecosystem.config.js --only lasantha-tire-v2.0

# Or direct start
pm2 start node_modules/next/dist/bin/next --name lasantha-tire-v2.0 -- start -H 0.0.0.0 -p 3029
```

## Project Structure

```
src/
├── app/                    # Next.js App Router
│   ├── api/               # API routes
│   │   ├── erp/           # ERP endpoints (peachtree, inventory, sales)
│   │   ├── auth/          # Authentication endpoints
│   │   └── peachtree-bridge/  # Bridge management
│   └── dashboard/         # Dashboard pages
├── core/                  # Core utilities
│   ├── config/           # Peachtree schema definitions
│   ├── hooks/            # React hooks (useBridgeMonitor)
│   ├── lib/              # Auth, database utilities
│   └── utils/            # PDF/Excel exports
└── views/                 # UI components
    └── shared/
        ├── erp/          # PeachtreeDashboard.tsx
        └── ui/           # Skeletons, empty states
```

## API Endpoints

### Peachtree Bridge Management
- `GET /api/peachtree-bridge` - Bridge status
- `POST /api/peachtree-bridge` - Start/stop/status actions

### ERP Data
- `GET /api/erp/peachtree?endpoint=health` - Connection health
- `GET /api/erp/peachtree?endpoint=customers` - Customer list
- `GET /api/erp/peachtree?endpoint=vendors` - Vendor list
- `GET /api/erp/peachtree?endpoint=chart-of-accounts` - GL accounts
- `GET /api/erp/peachtree?endpoint=transactions` - Transactions
- `GET /api/erp/peachtree?endpoint=business-status/ar-aging` - AR aging
- `GET /api/erp/peachtree?endpoint=business-status/ap-aging` - AP aging
- `GET /api/erp/peachtree?endpoint=business-status/cash-balances` - Cash position

## Environment Variables

```env
# Server
PORT=3029
HOSTNAME=0.0.0.0
NODE_ENV=production

# Peachtree Bridge
PEACHTREE_BRIDGE_PORT=5000  # or 5001

# Backend API
BOT_API_URL=http://localhost:8585
```

## Cloudflare Tunnel

Production URL: `https://app.lasanthatyre.com`  
Tunnel Name: `lasantha-app`  
Local Target: `localhost:3029`

## Version History

- **v2.0** (Dec 2025): Peachtree ERP, mobile UI, Financial Command Center
- **v1.5** (Nov 2025): Stable production with basic features
- **v1.0** (Oct 2025): Initial dashboard
