# Royal Booking V2

A modern, mobile-first booking application for Lasantha Tyre, built with Next.js 15.

## 📱 Features

- **Mobile-First Design**: Optimized for touch interfaces with sticky headers and footers.
- **Real-Time Availability**: Checks database for booked slots to prevent double bookings.
- **Multiple Service Selection**: Customers can select multiple services (e.g., Tyre Installation + Wheel Alignment) in a single booking.
- **Quotation Integration**: Seamlessly loads quotation details from the `WhatsAppAI` database using a Reference Code (`?ref=CODE`).
- **Smart Constraints**:
  - Wheel Alignment + Suspension are restricted to 7:30 AM - 5:30 PM.
  - Prevents booking past time slots for the current day.
- **WhatsApp Integration**: Sends immediate booking confirmation via WhatsApp Bot.

## 🛠️ Tech Stack

- **Framework**: Next.js 15 (App Router)
- **Styling**: Tailwind CSS
- **Animations**: Framer Motion
- **Icons**: Lucide React
- **State Management**: React Hooks (useState, useEffect)

## 🚀 Getting Started

### Prerequisites

- Node.js 18+
- Access to the main `WhatsAppAI` MSSQL database.
- The main WhatsApp Bot running on port `8585` (for API proxying).

### Installation

```bash
cd apps/royal-booking-v2
npm install
```

### Running Locally

```bash
npm run dev
```
The app will start on **http://localhost:3099**.

### Building for Production

```bash
npm run build
npm start
```

## 📦 Deployment

This application is managed via PM2 in the root `ecosystem.config.js`.

```bash
# Start/Restart via PM2
pm2 start ecosystem.config.js --only royal-booking-v2
```

## 🌐 External Access

The application is exposed to the internet via a Cloudflare Tunnel.
- **Public URL**: https://book.lasanthatyre.com
- **Tunnel Config**: `c:\whatsapp-sql-api\cloudflared-booking.yml`
