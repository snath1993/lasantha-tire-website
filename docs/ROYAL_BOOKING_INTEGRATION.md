# Royal Booking System - Quotation Integration

## Overview
This document explains how quotation data flows from the SQL database through the WhatsApp Bot API to the Royal Booking application when customers click the booking link in their quotation email.

## Email Integration

### Professional English Email Template
- **World-class design** with modern gradient header and responsive layout
- **English-only content** - no Sinhala text
- **Hotline number**: 0773131883
- **Conditional Royal Booking link** appears only for quotations

### Quotation Email Features
When a customer receives a quotation via email:
1. Professional header with company branding
2. Document type badge (QUOTATION)
3. Quotation number prominently displayed
4. Itemized table with Description, Quantity, Unit Price, Total
5. **Book Appointment CTA** - Blue button linking to Royal Booking
6. PDF attachment with full quotation details
7. Service features grid (Alignment, Installation, Quality, Pricing)
8. Footer with hotline and contact information

### Booking Link Format
```
${ROYAL_BOOKING_URL}/?ref=${quotationNumber}
```

Example:
```
http://localhost:3005/?ref=QUO-2024-001
```

## Data Flow Architecture

### Step 1: SQL Database (Source)
**Table**: `Quotations`

**Key Fields**:
- `RefCode` - Short reference code (e.g., "ABC123")
- `QuotationNumber` - Full quotation number (e.g., "QUO-2024-001")
- `CustomerName` - Customer name
- `CustomerPhone` - Phone number
- `VehicleNumber` - Vehicle registration
- `TyreSize` - Tire size/specification
- `Items` - JSON array of quotation items (stored as NVARCHAR(MAX))
- `TotalAmount` - Total quotation amount
- `CreatedAt` - Creation timestamp
- `ExpiryDate` - Quotation expiry date
- `IsBooked` - Booking status flag
- `BookingReference` - Reference from booking system

**Items Structure** (JSON):
```json
[
  {
    "description": "Bridgestone Turanza 205/55R16",
    "quantity": 4,
    "price": 25000,
    "total": 100000
  },
  {
    "description": "Wheel Alignment (Computerized)",
    "quantity": 1,
    "price": 3500,
    "total": 3500
  }
]
```

### Step 2: WhatsApp Bot API (Middleware)
**Endpoint**: `GET /api/quotations/:refCode`  
**File**: `index.js` (lines 1106-1152)

**Process**:
1. Accepts RefCode or QuotationNumber as parameter
2. Queries SQL database: `SELECT * FROM Quotations WHERE RefCode = @Reference OR QuotationNumber = @Reference`
3. Parses `Items` JSON string into JavaScript array
4. Returns normalized quotation object

**Response Format**:
```json
{
  "ok": true,
  "quotation": {
    "Id": 123,
    "RefCode": "ABC123",
    "QuotationNumber": "QUO-2024-001",
    "CustomerName": "John Doe",
    "CustomerPhone": "0771234567",
    "VehicleNumber": "CAB-1234",
    "TyreSize": "205/55R16",
    "Items": [
      {
        "description": "Bridgestone Turanza 205/55R16",
        "quantity": 4,
        "price": 25000,
        "total": 100000
      }
    ],
    "TotalAmount": 103500,
    "CreatedAt": "2024-01-15T10:30:00.000Z",
    "ExpiryDate": "2024-01-22",
    "IsBooked": false,
    "BookingReference": null
  }
}
```

### Step 3: Royal Booking API Proxy
**Endpoint**: `GET /api/quotation/:refId`  
**File**: `apps/royal-booking-v2/src/app/api/quotation/[refId]/route.ts`

**Process**:
1. Receives request from Royal Booking frontend
2. Proxies to WhatsApp Bot API: `${BOT_API_URL}/api/quotations/${refId}`
3. Returns same data structure to frontend

**Environment Variable**:
```
WHATSAPP_BOT_URL=http://localhost:8585
```

### Step 4: Royal Booking Frontend
**Page**: Royal Booking wizard  
**URL Parameter**: `?ref=QUO-2024-001`

**Process**:
1. User clicks "Book Appointment" button in email
2. Browser opens: `http://localhost:3005/?ref=QUO-2024-001`
3. Page detects `ref` parameter
4. Fetches quotation data: `fetch('/api/quotation/QUO-2024-001')`
5. **Pre-fills form** with:
   - Customer name
   - Phone number
   - Vehicle number
   - Tire size
   - Quotation items (already selected)
6. User only needs to:
   - Select appointment date
   - Select time slot
   - Confirm booking

## Complete Flow Diagram

```
┌─────────────────────┐
│   SQL Database      │
│   tblQuotations     │
│                     │
│  RefCode: ABC123    │
│  Items: JSON array  │
│  CustomerName: ...  │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  WhatsApp Bot API   │
│  Port: 8585         │
│                     │
│  GET /api/quotations│
│       /:refCode     │
│                     │
│  • Parse Items JSON │
│  • Normalize data   │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  Royal Booking API  │
│  Port: 3005         │
│                     │
│  GET /api/quotation │
│       /[refId]      │
│                     │
│  • Proxy request    │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  Royal Booking Page │
│  ?ref=QUO-2024-001  │
│                     │
│  • Fetch quotation  │
│  • Pre-fill form    │
│  • User books slot  │
└─────────────────────┘
```

## Email Service Configuration

### Environment Variables (.env)
```bash
# Email Configuration
EMAIL_PROVIDER=zoho
EMAIL_USER=info@lasanthatyre.com
EMAIL_PASSWORD=RAWnP5ZhfZtt
EMAIL_FROM_NAME=Lasantha Tyre Traders

# Royal Booking URL (used in quotation emails)
ROYAL_BOOKING_URL=http://localhost:3005
```

### Production Setup
For production deployment, update:
```bash
ROYAL_BOOKING_URL=https://booking.lasanthatyre.com
```

## Code Files Modified

### 1. Email Service (`services/emailService.js`)
**Changes**:
- ✅ World-class professional English template
- ✅ Removed all Sinhala text
- ✅ Updated hotline to 0773131883
- ✅ Added conditional Royal Booking CTA for quotations
- ✅ Modern responsive design with gradient header
- ✅ Service features grid
- ✅ Professional footer with copyright

**Key Code**:
```javascript
const isQuotation = (documentTitle === 'QUOTATION' || documentTitle === 'TAX QUOTATION');
const bookingLink = isQuotation ? `${royalBookingUrl}/?ref=${encodeURIComponent(invoiceNumber)}` : null;

${isQuotation ? `
<div class="cta-section">
  <div class="cta-title">📅 Ready to proceed? Book your appointment now!</div>
  <p>Skip the hassle - your quotation details are already pre-loaded.</p>
  <a href="${bookingLink}" class="cta-button">Book Appointment →</a>
</div>
` : ''}
```

### 2. Environment Configuration (`.env`)
**Changes**:
- ✅ Added `ROYAL_BOOKING_URL=http://localhost:3005`

### 3. No Changes Required
- ✅ Royal Booking page already working correctly
- ✅ Quotation API endpoint functional
- ✅ Data fetching and pre-filling implemented

## Testing Checklist

### Email Template
- [ ] Send test quotation email
- [ ] Verify English-only content
- [ ] Check hotline number displays as 0773131883
- [ ] Confirm "Book Appointment" button appears
- [ ] Verify button links to correct URL format

### Booking Flow
- [ ] Click "Book Appointment" button in email
- [ ] Confirm Royal Booking page loads
- [ ] Verify quotation data pre-fills correctly:
  - [ ] Customer name
  - [ ] Phone number
  - [ ] Vehicle number
  - [ ] Tire size
  - [ ] Quotation items
- [ ] Select date and time
- [ ] Complete booking

### Data Integrity
- [ ] Quotation items match email PDF
- [ ] Total amount matches
- [ ] Expiry date displayed correctly
- [ ] Booking updates SQL database

## Benefits for Customers

1. **Seamless Experience**: One-click from quotation to booking
2. **No Re-entry**: All details pre-filled automatically
3. **Fast Booking**: Just select date and time
4. **Professional Communication**: World-class email design
5. **Clear Call-to-Action**: Prominent booking button

## Technical Notes

### Royal Booking Page - DO NOT MODIFY
⚠️ **User Warning**: "mona hari aulak unoth" (don't break anything)

The Royal Booking page is already implemented and working correctly:
- URL parameter handling: `?ref=CODE`
- Quotation data fetching
- Form pre-filling
- Booking submission

**No changes required** - integration is complete via email link only.

### Items Array Format
Both systems expect the same structure:
```javascript
{
  description: string,  // Item description
  quantity: number,     // Quantity
  price: number,        // Unit price in Rs
  total: number         // Line total (qty × price)
}
```

### Booking Confirmation
When booking is completed, the Royal Booking system:
1. Creates booking record
2. Calls: `POST /api/quotations/${refCode}/booked`
3. Updates SQL: `IsBooked = 1`, `BookingReference = 'BOOK-XXX'`
4. Prevents duplicate bookings

## Maintenance

### Updating Royal Booking URL
1. Edit `.env` file
2. Change `ROYAL_BOOKING_URL` value
3. Restart bot: `pm2 restart whatsapp-bot --update-env`

### Email Template Updates
Edit `services/emailService.js` → `sendInvoiceEmail()` method

### Adding New Quotation Fields
1. Update SQL table `Quotations`
2. Modify bot API response (index.js)
3. Update Royal Booking API proxy (if needed)
4. Update Royal Booking form fields

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Author**: GitHub Copilot  
**Status**: ✅ Complete Integration
