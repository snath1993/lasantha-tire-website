# Lasantha Tire Center - Modern Website v2.0

Modern, animated website for Lasantha Tire Center built with Next.js 15, React 19, and Framer Motion.

## 🚀 Features

- ✨ **Modern Animations** - Smooth transitions and effects using Framer Motion
- 📱 **Fully Responsive** - Optimized for all devices
- 🎨 **Beautiful UI** - Gradient backgrounds, animated elements
- 💬 **WhatsApp Integration** - Direct price requests via WhatsApp bot
- 🔍 **Smart Price System** - Integrates with existing TyrePriceReplyJob
- ⚡ **Fast Performance** - Built on Next.js 15 with App Router

## 🛠️ Tech Stack

- **Framework**: Next.js 15.0.3
- **UI Library**: React 19.0.0
- **Animations**: Framer Motion 11.11.17
- **Icons**: Lucide React 0.454.0
- **Styling**: Tailwind CSS 3.4.14
- **Language**: TypeScript 5.6.3

## 📁 Project Structure

```
lasantha-site-v2/
├── src/
│   ├── app/
│   │   ├── api/
│   │   │   └── quote/
│   │   │       └── route.ts          # API endpoint for quote requests
│   │   ├── layout.tsx                # Root layout
│   │   ├── page.tsx                  # Homepage
│   │   └── globals.css               # Global styles
│   ├── components/
│   │   ├── Header.tsx                # Navigation header
│   │   ├── Hero.tsx                  # Hero section with animations
│   │   ├── Services.tsx              # Services showcase
│   │   ├── Brands.tsx                # Brand logos
│   │   ├── QuoteForm.tsx             # WhatsApp quote form
│   │   └── Footer.tsx                # Footer section
│   └── lib/                          # Utilities (if needed)
├── public/
│   └── images/                       # Static assets
├── .env.local                        # Environment variables
├── tailwind.config.ts                # Tailwind configuration
├── tsconfig.json                     # TypeScript configuration
├── next.config.mjs                   # Next.js configuration
└── package.json                      # Dependencies
```

## 🔧 Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Configure environment variables**:
   Create `.env.local` file:
   ```env
   WHATSAPP_BOT_URL=http://localhost:3100
   ```

3. **Run development server**:
   ```bash
   npm run dev
   ```

4. **Open browser**:
   Visit [http://localhost:3001](http://localhost:3001)

## 📡 API Integration

### Quote Request Flow

1. Customer fills quote form (name, phone, tire size, quantity)
2. Form submits to `/api/quote` endpoint
3. API checks if WhatsApp number is registered
4. If registered, creates entry in `WebsiteTirePriceRequests` table
5. Triggers `TyrePriceReplyJob` to send price via WhatsApp
6. Customer receives price directly on WhatsApp

### Database Setup

Run the SQL script to create the required table:

```bash
# From main project directory
sqlcmd -S YOUR_SERVER -d YOUR_DATABASE -i create-website-price-requests-table.sql
```

Or execute in SSMS:
```sql
-- See: create-website-price-requests-table.sql
```

### Bot API Endpoints

The website uses these bot API endpoints:

- `POST /api/check-whatsapp` - Verify WhatsApp registration
- `POST /api/send-price-request` - Send price request to customer

## 🎨 Components

### Hero Section
- Animated gradient background
- Rotating tire visual
- Floating statistics
- Smooth scroll indicator

### Services Grid
- 6 service cards with hover effects
- Individual animations on scroll
- Icon animations on hover

### Quote Form
- Real-time validation
- WhatsApp number verification
- Optional vehicle model field
- Quantity selector (1-6 tires)
- Success/error notifications
- Loading states

### Brands Showcase
- Premium tire brand logos
- Hover scale animations
- Grid layout (responsive)

## 🔑 Key Features

### WhatsApp Integration

- **Number Verification**: Checks if customer's number is registered on WhatsApp
- **Direct Messaging**: Sends price directly to customer (no redirect)
- **Error Handling**: Shows friendly messages for unregistered numbers
- **Fallback Mode**: Works even if bot is offline (queues requests)

### Optional Fields

- **Vehicle Model**: Checkbox to include vehicle information
- **Quantity**: Dropdown selection (1-6 tires, or omit)
- Both fields are optional for faster quotes

### Notifications

- ✅ **Success**: Green notification when request sent
- ⚠️ **Warning**: Yellow notification for unregistered WhatsApp numbers
- ❌ **Error**: Red notification for failures

## 🚀 Deployment

### Production Build

```bash
npm run build
npm start
```

### Environment Variables (Production)

```env
WHATSAPP_BOT_URL=https://your-bot-domain.com
```

## 📊 Performance

- **Lighthouse Score**: 95+ (target)
- **First Contentful Paint**: < 1.5s
- **Time to Interactive**: < 3.0s
- **Bundle Size**: Optimized with Next.js

## 🎯 Future Enhancements

- [ ] Add tire search functionality
- [ ] Customer account system
- [ ] Order tracking
- [ ] Live chat integration
- [ ] Multi-language support (Sinhala/Tamil)
- [ ] Admin panel for quotes

## 📝 Notes

- Port 3001 (website) must be different from 3100 (bot)
- Bot must be running for WhatsApp features to work
- Database connection required for price requests
- Original website at `apps/lasantha-site` remains unchanged

## 🐛 Troubleshooting

### Website not loading
- Check if port 3001 is available
- Verify Node.js version (v18+)
- Clear `.next` cache: `rm -rf .next`

### WhatsApp not working
- Ensure bot is running on port 3100
- Check `WHATSAPP_BOT_URL` in `.env.local`
- Verify bot endpoints are accessible

### Form not submitting
- Check browser console for errors
- Verify API route is accessible
- Check network tab for failed requests

## 📞 Support

For issues or questions, contact the development team.

## 📄 License

Private - Lasantha Tire Center

---

**Version**: 2.0.0  
**Last Updated**: November 18, 2025  
**Built with** ❤️ **using Next.js**
