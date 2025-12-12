# V3 Deployment Guide

## 🚀 Production Deployment Checklist

### Pre-Deployment Steps

#### 1. Environment Configuration
Create production `.env` file:
```env
# Required
WHATSAPP_BOT_URL=https://your-production-bot-url.com
NODE_ENV=production
NEXT_PUBLIC_SITE_URL=https://lasanthatyre.com
NEXT_PUBLIC_SITE_NAME=Lasantha Tyre Traders

# Optional but Recommended
NEXT_PUBLIC_GA_ID=G-XXXXXXXXXX
SENTRY_DSN=your-sentry-dsn
```

#### 2. PWA Assets
Create the following icon files in `/public`:
- `icon-192x192.png` - 192x192 app icon
- `icon-384x384.png` - 384x384 app icon
- `icon-512x512.png` - 512x512 app icon
- `apple-touch-icon.png` - 180x180 Apple icon
- `favicon.ico` - 32x32 favicon

#### 3. Security Verification
```bash
# Check for security vulnerabilities
npm audit

# Fix if any found
npm audit fix

# Type checking
npm run type-check

# Linting
npm run lint
```

#### 4. Build Testing
```bash
# Create production build
npm run build

# Test production server locally
npm start

# Verify at http://localhost:3025
```

## 🌐 Deployment Options

### Option 1: Vercel (Recommended)

1. **Install Vercel CLI**
```bash
npm i -g vercel
```

2. **Deploy**
```bash
cd apps/lasantha-site-v2
vercel --prod
```

3. **Environment Variables**
Add in Vercel Dashboard:
- Settings → Environment Variables
- Add all variables from `.env.example`

4. **Domain Configuration**
- Add custom domain in Vercel Dashboard
- Update DNS records as instructed

### Option 2: Docker

1. **Create Dockerfile**
```dockerfile
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
EXPOSE 3025
CMD ["node", "server.js"]
```

2. **Build & Run**
```bash
docker build -t lasantha-tire-v3 .
docker run -p 3025:3025 -e WHATSAPP_BOT_URL=https://bot.url lasantha-tire-v3
```

### Option 3: Traditional VPS

1. **Install Dependencies**
```bash
# Install Node.js 20+
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# Install PM2
sudo npm install -g pm2
```

2. **Deploy Application**
```bash
cd /var/www/lasantha-tire-website/apps/lasantha-site-v2

# Install dependencies
npm ci --production

# Build
npm run build

# Start with PM2
pm2 start npm --name "lasantha-v3" -- start
pm2 save
pm2 startup
```

3. **Configure Nginx**
```nginx
server {
    listen 80;
    server_name lasanthatyre.com www.lasanthatyre.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name lasanthatyre.com www.lasanthatyre.com;

    ssl_certificate /etc/letsencrypt/live/lasanthatyre.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/lasanthatyre.com/privkey.pem;

    # Security Headers (Next.js also sets these, but good to have backup)
    add_header Strict-Transport-Security "max-age=63072000; includeSubDomains; preload" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    location / {
        proxy_pass http://localhost:3025;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

## 📊 Post-Deployment Verification

### 1. Health Check
```bash
curl https://lasanthatyre.com/api/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-12-12T13:30:00.000Z",
  "version": "3.0.0",
  "checks": {
    "server": "ok",
    "memory": { ... },
    "responseTime": 5
  }
}
```

### 2. Security Headers Check
```bash
curl -I https://lasanthatyre.com
```

Verify these headers exist:
- `Strict-Transport-Security`
- `X-Frame-Options`
- `X-Content-Type-Options`
- `X-XSS-Protection`
- `Referrer-Policy`

### 3. Performance Testing

#### Lighthouse Audit
1. Open Chrome DevTools
2. Go to Lighthouse tab
3. Run audit for:
   - Performance (Target: 95+)
   - Accessibility (Target: 95+)
   - Best Practices (Target: 95+)
   - SEO (Target: 95+)

#### WebPageTest
Visit: https://www.webpagetest.org/
- Test from multiple locations
- Verify First Contentful Paint < 1.5s
- Verify Time to Interactive < 3.0s

### 4. PWA Verification
1. Open site in Chrome
2. Look for install prompt
3. Install as app
4. Test offline functionality (when implemented)

### 5. SEO Verification

#### Google Search Console
1. Add property
2. Verify ownership
3. Submit sitemap: `https://lasanthatyre.com/sitemap.xml`
4. Monitor indexing status

#### Rich Snippets Test
Visit: https://search.google.com/test/rich-results
- Enter: `https://lasanthatyre.com`
- Verify Business schema is detected

## 🔍 Monitoring Setup

### 1. Google Analytics 4
```typescript
// Add to layout.tsx
import { Analytics } from '@vercel/analytics/react'

export default function RootLayout({ children }) {
  return (
    <html>
      <body>
        {children}
        <Analytics />
      </body>
    </html>
  )
}
```

### 2. Error Tracking (Sentry)
```bash
npm install @sentry/nextjs
npx @sentry/wizard@latest -i nextjs
```

### 3. Uptime Monitoring
Options:
- **UptimeRobot** (Free)
- **Pingdom**
- **StatusCake**

Monitor endpoints:
- `https://lasanthatyre.com/` (Main site)
- `https://lasanthatyre.com/api/health` (Health check)

### 4. Performance Monitoring
Use Vercel Analytics or Google Analytics 4 Core Web Vitals

## 🔄 Update Process

### Rolling Updates
```bash
# Pull latest changes
git pull origin main

# Install dependencies
npm ci

# Build
npm run build

# Restart (PM2)
pm2 restart lasantha-v3

# Or (Docker)
docker-compose up -d --build
```

### Zero-Downtime Updates (Advanced)
Use load balancer with multiple instances:
1. Update instance 1, remove from load balancer
2. Deploy new version
3. Add back to load balancer
4. Repeat for instance 2

## 📱 SSL Certificate Setup

### Let's Encrypt (Free)
```bash
# Install Certbot
sudo apt-get install certbot python3-certbot-nginx

# Get certificate
sudo certbot --nginx -d lasanthatyre.com -d www.lasanthatyre.com

# Auto-renewal (already configured by Certbot)
sudo certbot renew --dry-run
```

## 🎯 Performance Optimization Tips

### 1. Enable Caching
- Static assets: 1 year
- API responses: Appropriate TTL
- CDN for static files

### 2. Image Optimization
- Use WebP/AVIF formats
- Responsive images
- Lazy loading enabled

### 3. Code Splitting
- Automatic with Next.js
- Dynamic imports where needed

### 4. Database Optimization
- Connection pooling
- Query optimization
- Caching layer (Redis)

## 🐛 Troubleshooting

### Build Fails
```bash
# Clear cache
rm -rf .next node_modules
npm install
npm run build
```

### Memory Issues
```bash
# Increase Node memory
NODE_OPTIONS="--max-old-space-size=4096" npm run build
```

### Port Already in Use
```bash
# Kill process on port 3025
lsof -ti:3025 | xargs kill -9
```

## 📞 Support Contacts

- **Technical Issues**: development@lasanthatyre.com
- **Security Issues**: security@lasanthatyre.com
- **Emergency**: +94 77 313 1883

## 📝 Change Log

Track all production deployments:
- Version 3.0.0 - Initial V3 deployment
- Date: 2025-12-12
- Changes: [List major changes]

---

**Last Updated**: December 12, 2025  
**Next Review**: January 12, 2026
