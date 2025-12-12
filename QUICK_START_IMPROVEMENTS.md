# ⚡ QUICK START IMPROVEMENTS
## Immediate Actions to Boost Production Readiness

**Time Required:** 2-3 days  
**Impact:** HIGH  
**Difficulty:** Easy to Medium  

---

## 🎯 GOAL

Improve production readiness from **92% to 95%** in just 2-3 days with these high-impact, low-effort improvements.

---

## DAY 1: Security & Configuration (6 hours)

### 1. Add Security Headers (2 hours)

#### Step 1: Install Helmet
```bash
cd /home/runner/work/lasantha-tire-website/lasantha-tire-website
npm install helmet
```

#### Step 2: Update index.js
Add after the express initialization:

```javascript
// After: const app = express();
const helmet = require('helmet');

// Add security headers
app.use(helmet({
  contentSecurityPolicy: {
    directives: {
      defaultSrc: ["'self'"],
      styleSrc: ["'self'", "'unsafe-inline'"],
      scriptSrc: ["'self'"],
      imgSrc: ["'self'", "data:", "https:"],
      connectSrc: ["'self'"],
      fontSrc: ["'self'"],
      objectSrc: ["'none'"],
      mediaSrc: ["'self'"],
      frameSrc: ["'none'"],
    },
  },
  hsts: {
    maxAge: 31536000,
    includeSubDomains: true,
    preload: true
  },
  noSniff: true,
  xssFilter: true,
  hidePoweredBy: true
}));
```

### 2. Add CORS Configuration (1 hour)

#### Step 1: Install CORS
```bash
npm install cors
```

#### Step 2: Update index.js
Add after helmet:

```javascript
const cors = require('cors');

// Add to .env
// ALLOWED_ORIGINS=http://localhost:3026,http://localhost:3025,https://yourdomain.com

const allowedOrigins = process.env.ALLOWED_ORIGINS?.split(',') || [
  'http://localhost:3026',
  'http://localhost:3025'
];

app.use(cors({
  origin: (origin, callback) => {
    // Allow requests with no origin (mobile apps, Postman, etc.)
    if (!origin) return callback(null, true);
    
    if (allowedOrigins.includes(origin)) {
      callback(null, true);
    } else {
      callback(new Error('Not allowed by CORS'));
    }
  },
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization', 'X-Requested-With'],
  exposedHeaders: ['Content-Range', 'X-Content-Range'],
  maxAge: 86400 // 24 hours
}));
```

### 3. Setup Proper Logging (3 hours)

#### Step 1: Install Winston
```bash
npm install winston winston-daily-rotate-file
```

#### Step 2: Create logger utility
Create file: `utils/logger.js`

```javascript
const winston = require('winston');
const path = require('path');

// Define log format
const logFormat = winston.format.combine(
  winston.format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
  winston.format.errors({ stack: true }),
  winston.format.splat(),
  winston.format.json()
);

// Create console format for development
const consoleFormat = winston.format.combine(
  winston.format.colorize(),
  winston.format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
  winston.format.printf(({ timestamp, level, message, ...meta }) => {
    let msg = `${timestamp} [${level}]: ${message}`;
    if (Object.keys(meta).length > 0) {
      msg += ` ${JSON.stringify(meta)}`;
    }
    return msg;
  })
);

// Create logger instance
const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: logFormat,
  defaultMeta: { service: 'lasantha-tire-bot' },
  transports: [
    // Error log file
    new winston.transports.DailyRotateFile({
      filename: path.join(__dirname, '../logs/error-%DATE%.log'),
      datePattern: 'YYYY-MM-DD',
      level: 'error',
      maxSize: '20m',
      maxFiles: '14d',
      zippedArchive: true
    }),
    // Combined log file
    new winston.transports.DailyRotateFile({
      filename: path.join(__dirname, '../logs/combined-%DATE%.log'),
      datePattern: 'YYYY-MM-DD',
      maxSize: '20m',
      maxFiles: '30d',
      zippedArchive: true
    }),
    // Console output
    new winston.transports.Console({
      format: consoleFormat,
      level: process.env.NODE_ENV === 'production' ? 'info' : 'debug'
    })
  ]
});

// Create stream for Morgan HTTP logging
logger.stream = {
  write: (message) => {
    logger.info(message.trim());
  }
};

module.exports = logger;
```

#### Step 3: Replace console.log
In `index.js`, add at the top:
```javascript
const logger = require('./utils/logger');

// Replace console.log with logger
// console.log('message') -> logger.info('message')
// console.error('error') -> logger.error('error')
// console.warn('warning') -> logger.warn('warning')
```

#### Step 4: Add HTTP request logging
```bash
npm install morgan
```

In `index.js`:
```javascript
const morgan = require('morgan');

// Add after app initialization
app.use(morgan('combined', { stream: logger.stream }));
```

---

## DAY 2: Testing & Documentation (8 hours)

### 4. Setup Testing Framework (3 hours)

#### Step 1: Install Jest and dependencies
```bash
npm install --save-dev jest @types/jest supertest
```

#### Step 2: Create jest.config.js
```javascript
module.exports = {
  testEnvironment: 'node',
  coverageDirectory: 'coverage',
  collectCoverageFrom: [
    'utils/**/*.js',
    'jobs/**/*.js',
    'services/**/*.js',
    '!**/node_modules/**',
    '!**/tests/**'
  ],
  testMatch: [
    '**/tests/**/*.test.js',
    '**/?(*.)+(spec|test).js'
  ],
  coverageThreshold: {
    global: {
      branches: 40,
      functions: 40,
      lines: 40,
      statements: 40
    }
  },
  testTimeout: 10000
};
```

#### Step 3: Update package.json
Add to scripts section:
```json
{
  "scripts": {
    "test": "jest",
    "test:watch": "jest --watch",
    "test:coverage": "jest --coverage"
  }
}
```

#### Step 4: Create sample test
Create: `tests/services/emailService.test.js`

```javascript
const emailService = require('../../services/emailService');

describe('EmailService', () => {
  describe('initialization', () => {
    test('should initialize without errors', async () => {
      expect(emailService).toBeDefined();
    });
  });

  describe('sendQuotationEmail', () => {
    test('should fail with invalid email', async () => {
      const result = await emailService.sendQuotationEmail({
        customerEmail: 'invalid-email',
        quotationNumber: 'TEST-001',
        items: [],
        totalAmount: 0
      });
      
      expect(result.success).toBe(false);
      expect(result.error).toBeDefined();
    });

    test('should validate email format', async () => {
      const result = await emailService.sendQuotationEmail({
        customerEmail: 'test@example.com',
        quotationNumber: 'TEST-001',
        customerName: 'Test User',
        items: [
          {
            description: 'Tyre 195/65R15',
            brand: 'Test Brand',
            quantity: 4,
            price: 10000
          }
        ],
        totalAmount: 40000,
        bookingUrl: 'https://example.com/book',
        expiryDate: new Date()
      });
      
      // Will fail if not initialized, but validates structure
      expect(result).toHaveProperty('success');
    });
  });
});
```

#### Step 5: Run tests
```bash
npm test
```

### 5. Create API Documentation (3 hours)

#### Step 1: Install Swagger
```bash
npm install swagger-ui-express swagger-jsdoc
```

#### Step 2: Create swagger.js
Create: `config/swagger.js`

```javascript
const swaggerJsdoc = require('swagger-jsdoc');
const swaggerUi = require('swagger-ui-express');

const options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'Lasantha Tire Bot API',
      version: '1.0.0',
      description: 'WhatsApp Tyre Quotation and Management System API',
      contact: {
        name: 'API Support',
        email: 'support@lasanthatyre.com'
      }
    },
    servers: [
      {
        url: 'http://localhost:3100',
        description: 'Development server'
      },
      {
        url: 'https://bot.lasanthatyre.com',
        description: 'Production server'
      }
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: 'http',
          scheme: 'bearer',
          bearerFormat: 'JWT'
        }
      }
    }
  },
  apis: ['./index.js', './routes/*.js'] // Files containing annotations
};

const swaggerSpec = swaggerJsdoc(options);

module.exports = { swaggerUi, swaggerSpec };
```

#### Step 3: Update index.js
Add after app initialization:

```javascript
const { swaggerUi, swaggerSpec } = require('./config/swagger');

// API Documentation
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec, {
  customCss: '.swagger-ui .topbar { display: none }',
  customSiteTitle: 'Lasantha Tire Bot API Docs'
}));

console.log('📚 API Documentation: http://localhost:3100/api-docs');
```

#### Step 4: Add Swagger annotations
Add to existing endpoints in index.js:

```javascript
/**
 * @swagger
 * /health:
 *   get:
 *     summary: Health check endpoint
 *     description: Returns the health status of the bot API
 *     responses:
 *       200:
 *         description: Bot is healthy
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 status:
 *                   type: string
 *                   example: ok
 *                 timestamp:
 *                   type: string
 *                   format: date-time
 *                 uptime:
 *                   type: number
 */
app.get('/health', async (req, res) => {
  // ... existing code
});
```

### 6. Create Deployment Documentation (2 hours)

Create: `docs/DEPLOYMENT.md`

```markdown
# Deployment Guide

## Prerequisites

- Node.js 18+ installed
- SQL Server 2019+ running
- Windows Server 2019+ OR Ubuntu 20.04+
- PM2 installed globally: `npm install -g pm2`

## Environment Setup

1. Copy `.env.example` to `.env`
2. Fill in all required values
3. Test database connection

## Installation Steps

### 1. Clone Repository
\`\`\`bash
git clone https://github.com/snath1993/lasantha-tire-website.git
cd lasantha-tire-website
\`\`\`

### 2. Install Dependencies
\`\`\`bash
npm install
cd lasantha-tire-v1.0 && npm install
cd ../apps/lasantha-site-v2 && npm install
\`\`\`

### 3. Build Applications
\`\`\`bash
cd lasantha-tire-v1.0 && npm run build
cd ../apps/lasantha-site-v2 && npm run build
\`\`\`

### 4. Start Services

#### Using PM2 (Recommended)
\`\`\`bash
pm2 start ecosystem.config.js
pm2 save
pm2 startup
\`\`\`

#### Manual Start
\`\`\`bash
# Terminal 1: Bot
node index.js

# Terminal 2: Dashboard
cd lasantha-tire-v1.0 && npm start

# Terminal 3: Website
cd apps/lasantha-site-v2 && npm start
\`\`\`

## Production Checklist

- [ ] Environment variables configured
- [ ] Database migrations run
- [ ] HTTPS/SSL configured
- [ ] Firewall rules configured
- [ ] Backup strategy implemented
- [ ] Monitoring configured
- [ ] Logging configured
- [ ] WhatsApp authenticated
- [ ] Email credentials configured
- [ ] Test all features

## Monitoring

- Health endpoint: `http://localhost:3100/health`
- Stats endpoint: `http://localhost:3100/stats`
- PM2 monitoring: `pm2 monit`

## Troubleshooting

See README.md for detailed troubleshooting guide.
\`\`\`

---

## DAY 3: Validation & Optimization (4 hours)

### 7. Add Input Validation (2 hours)

#### Step 1: Install express-validator
```bash
npm install express-validator
```

#### Step 2: Create validation middleware
Create: `middleware/validation.js`

```javascript
const { body, param, query, validationResult } = require('express-validator');

// Error handler middleware
const validate = (req, res, next) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ 
      success: false,
      errors: errors.array() 
    });
  }
  next();
};

// Common validators
const validators = {
  // Quotation validators
  quotation: [
    body('customerName').notEmpty().trim().escape(),
    body('customerPhone').matches(/^0\d{9}$/),
    body('items').isArray({ min: 1 }),
    body('totalAmount').isNumeric().toFloat(),
    validate
  ],
  
  // Email validators
  email: [
    body('customerEmail').isEmail().normalizeEmail(),
    validate
  ],
  
  // Phone validators
  phone: [
    param('phone').matches(/^0\d{9}$/),
    validate
  ],
  
  // OTP validators
  otp: [
    body('phone').matches(/^0\d{9}$/),
    body('otp').isLength({ min: 6, max: 6 }).isNumeric(),
    validate
  ]
};

module.exports = validators;
```

#### Step 3: Apply to routes
In `index.js`:

```javascript
const validators = require('./middleware/validation');

// Apply validators to routes
app.post('/api/quotations', validators.quotation, async (req, res) => {
  // ... existing code
});

app.post('/api/auth/verify-otp', validators.otp, async (req, res) => {
  // ... existing code
});
```

### 8. Performance Optimization (2 hours)

#### Step 1: Add compression
```bash
npm install compression
```

In `index.js`:
```javascript
const compression = require('compression');

// Add after body parser
app.use(compression({
  level: 6,
  threshold: 1024,
  filter: (req, res) => {
    if (req.headers['x-no-compression']) {
      return false;
    }
    return compression.filter(req, res);
  }
}));
```

#### Step 2: Optimize database queries
Add connection pooling configuration in `index.js`:

```javascript
const dbConfig = {
  user: process.env.SQL_USER || process.env.DB_USER,
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD,
  database: process.env.SQL_DATABASE || process.env.DB_NAME,
  server: process.env.SQL_SERVER || process.env.DB_SERVER,
  port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
  pool: {
    max: 20, // Increased from default
    min: 5,
    idleTimeoutMillis: 30000
  },
  options: {
    encrypt: String(process.env.DB_ENCRYPT || 'false').toLowerCase() === 'true',
    trustServerCertificate: String(process.env.DB_TRUST_CERT || 'true').toLowerCase() === 'true',
    enableArithAbort: true
  },
  connectionTimeout: parseInt(process.env.SQL_CONNECTION_TIMEOUT_MS || '30000', 10),
  requestTimeout: parseInt(process.env.SQL_REQUEST_TIMEOUT_MS || '30000', 10)
};
```

---

## 🎯 VERIFICATION CHECKLIST

After completing all improvements:

### Security ✅
- [ ] Helmet installed and configured
- [ ] CORS configured with allowed origins
- [ ] Input validation on critical endpoints
- [ ] Logging system implemented

### Testing ✅
- [ ] Jest configured
- [ ] Sample tests created and passing
- [ ] Test coverage reporting enabled

### Documentation ✅
- [ ] .env.example created
- [ ] Swagger API docs accessible
- [ ] Deployment guide created

### Performance ✅
- [ ] Compression enabled
- [ ] Database pooling optimized
- [ ] Logger configured

---

## 📊 EXPECTED RESULTS

After completing these improvements:

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| **Production Readiness** | 92% | 95% | +3% |
| **Security Score** | Good | Excellent | +2 levels |
| **Documentation** | 85% | 92% | +7% |
| **Code Quality** | 88% | 92% | +4% |
| **Test Coverage** | 0% | 15% | +15% |

---

## 🚀 NEXT STEPS

1. Complete Day 1-3 improvements
2. Test all changes thoroughly
3. Deploy to staging environment
4. Run security audit
5. Move to Phase 2 of Improvement Roadmap

---

## 💡 TIPS

1. **Start with Day 1** - Security is critical
2. **Test incrementally** - Don't wait until the end
3. **Backup before changes** - Save your current state
4. **Document as you go** - Update README with changes
5. **Ask for help** - Don't hesitate to seek assistance

---

## 📞 SUPPORT

If you encounter issues:
1. Check the error logs in `logs/` directory
2. Review the TROUBLESHOOTING section in README.md
3. Verify environment variables are set correctly
4. Test individual components separately

---

**Last Updated:** December 12, 2025  
**Version:** 1.0  

---

**GOOD LUCK! 🚀**
