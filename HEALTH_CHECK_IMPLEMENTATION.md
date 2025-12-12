# Health Check Endpoints Implementation Guide

## Overview

This document provides implementation guidelines for adding health check endpoints to the Lasantha Tire WhatsApp Bot API server.

## Purpose

Health check endpoints are essential for:
- Monitoring service availability
- Kubernetes liveness/readiness probes
- Load balancer health checks
- Operational visibility
- Automated alerting

## Implementation

### 1. Basic Health Check

Add this endpoint to `index.js` (after Express app initialization):

```javascript
/**
 * Basic health check endpoint
 * Returns service status and uptime
 */
app.get('/health', (req, res) => {
    res.json({
        status: 'healthy',
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        version: '1.0.0',
        service: 'lasantha-tire-bot'
    });
});
```

**Response Example:**
```json
{
    "status": "healthy",
    "timestamp": "2025-12-12T07:36:25.702Z",
    "uptime": 3600.5,
    "version": "1.0.0",
    "service": "lasantha-tire-bot"
}
```

### 2. Detailed Health Check

Add this endpoint for comprehensive dependency status:

```javascript
/**
 * Detailed health check with dependency status
 * Checks database, WhatsApp, email, and Facebook connectivity
 */
app.get('/health/detailed', async (req, res) => {
    const health = {
        status: 'healthy',
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        version: '1.0.0',
        checks: {
            database: { status: 'unknown', message: '' },
            whatsapp: { status: 'unknown', message: '' },
            email: { status: 'unknown', message: '' },
            facebook: { status: 'unknown', message: '' }
        }
    };

    // Database health check
    try {
        const pool = await sql.connect(sqlConfig);
        await pool.request().query('SELECT 1 AS HealthCheck');
        await pool.close();
        health.checks.database.status = 'connected';
        health.checks.database.message = 'Database connection successful';
    } catch (error) {
        health.checks.database.status = 'error';
        health.checks.database.message = error.message;
        health.status = 'degraded';
    }

    // WhatsApp health check
    try {
        const { getWhatsAppClient } = require('./utils/waClientRegistry');
        const waClient = getWhatsAppClient();
        
        if (waClient && waClient.info) {
            health.checks.whatsapp.status = 'ready';
            health.checks.whatsapp.message = `Connected as ${waClient.info.wid.user}`;
        } else {
            health.checks.whatsapp.status = 'not_ready';
            health.checks.whatsapp.message = 'WhatsApp client not initialized';
            health.status = 'degraded';
        }
    } catch (error) {
        health.checks.whatsapp.status = 'error';
        health.checks.whatsapp.message = error.message;
        health.status = 'degraded';
    }

    // Email service health check
    try {
        const emailService = require('./services/emailService');
        if (emailService.initialized) {
            health.checks.email.status = 'ready';
            health.checks.email.message = `Email service initialized (${process.env.EMAIL_PROVIDER})`;
        } else {
            health.checks.email.status = 'not_configured';
            health.checks.email.message = 'Email service not configured';
        }
    } catch (error) {
        health.checks.email.status = 'error';
        health.checks.email.message = error.message;
    }

    // Facebook integration health check
    try {
        if (process.env.FACEBOOK_PAGE_ACCESS_TOKEN) {
            health.checks.facebook.status = 'configured';
            health.checks.facebook.message = 'Facebook integration configured';
        } else {
            health.checks.facebook.status = 'not_configured';
            health.checks.facebook.message = 'Facebook integration not configured';
        }
    } catch (error) {
        health.checks.facebook.status = 'error';
        health.checks.facebook.message = error.message;
    }

    // Set appropriate status code
    const statusCode = health.status === 'healthy' ? 200 : 503;
    res.status(statusCode).json(health);
});
```

**Response Example (Healthy):**
```json
{
    "status": "healthy",
    "timestamp": "2025-12-12T07:36:25.702Z",
    "uptime": 3600.5,
    "version": "1.0.0",
    "checks": {
        "database": {
            "status": "connected",
            "message": "Database connection successful"
        },
        "whatsapp": {
            "status": "ready",
            "message": "Connected as 947712225090"
        },
        "email": {
            "status": "ready",
            "message": "Email service initialized (gmail)"
        },
        "facebook": {
            "status": "configured",
            "message": "Facebook integration configured"
        }
    }
}
```

**Response Example (Degraded):**
```json
{
    "status": "degraded",
    "timestamp": "2025-12-12T07:36:25.702Z",
    "uptime": 3600.5,
    "version": "1.0.0",
    "checks": {
        "database": {
            "status": "error",
            "message": "Connection timeout"
        },
        "whatsapp": {
            "status": "ready",
            "message": "Connected as 947712225090"
        },
        "email": {
            "status": "not_configured",
            "message": "Email service not configured"
        },
        "facebook": {
            "status": "configured",
            "message": "Facebook integration configured"
        }
    }
}
```

### 3. Kubernetes Readiness Probe

Add this endpoint for Kubernetes readiness checks:

```javascript
/**
 * Kubernetes readiness probe
 * Returns 200 if service is ready to accept traffic
 * Returns 503 if service is not ready
 */
app.get('/ready', async (req, res) => {
    try {
        // Check critical dependencies
        const pool = await sql.connect(sqlConfig);
        await pool.request().query('SELECT 1');
        await pool.close();

        // Check WhatsApp client
        const { getWhatsAppClient } = require('./utils/waClientRegistry');
        const waClient = getWhatsAppClient();
        
        if (!waClient || !waClient.info) {
            throw new Error('WhatsApp client not ready');
        }

        res.status(200).send('OK');
    } catch (error) {
        console.error('[Health] Readiness check failed:', error.message);
        res.status(503).send('Service Unavailable');
    }
});
```

### 4. Kubernetes Liveness Probe

Add this endpoint for Kubernetes liveness checks:

```javascript
/**
 * Kubernetes liveness probe
 * Returns 200 if process is alive
 * Simple check without external dependencies
 */
app.get('/alive', (req, res) => {
    res.status(200).send('OK');
});
```

### 5. Metrics Endpoint (Optional)

Add this endpoint for basic metrics:

```javascript
/**
 * Metrics endpoint
 * Returns operational metrics
 */
app.get('/metrics', (req, res) => {
    const metrics = {
        timestamp: new Date().toISOString(),
        uptime: process.uptime(),
        memory: process.memoryUsage(),
        cpu: process.cpuUsage(),
        platform: process.platform,
        nodeVersion: process.version,
        pid: process.pid
    };

    res.json(metrics);
});
```

**Response Example:**
```json
{
    "timestamp": "2025-12-12T07:36:25.702Z",
    "uptime": 3600.5,
    "memory": {
        "rss": 123456789,
        "heapTotal": 67108864,
        "heapUsed": 45678901,
        "external": 1234567
    },
    "cpu": {
        "user": 1234567,
        "system": 234567
    },
    "platform": "linux",
    "nodeVersion": "v20.10.0",
    "pid": 12345
}
```

## Usage Examples

### 1. cURL Commands

```bash
# Basic health check
curl http://localhost:8585/health

# Detailed health check
curl http://localhost:8585/health/detailed

# Readiness check
curl http://localhost:8585/ready

# Liveness check
curl http://localhost:8585/alive

# Metrics
curl http://localhost:8585/metrics
```

### 2. Kubernetes Configuration

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: lasantha-tire-bot
spec:
  containers:
  - name: bot
    image: lasantha-tire-bot:latest
    ports:
    - containerPort: 8585
    livenessProbe:
      httpGet:
        path: /alive
        port: 8585
      initialDelaySeconds: 30
      periodSeconds: 10
      timeoutSeconds: 5
      failureThreshold: 3
    readinessProbe:
      httpGet:
        path: /ready
        port: 8585
      initialDelaySeconds: 10
      periodSeconds: 5
      timeoutSeconds: 3
      failureThreshold: 3
```

### 3. Docker Health Check

```dockerfile
FROM node:20-alpine

WORKDIR /app
COPY . .

RUN npm ci --only=production

EXPOSE 8585

HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
  CMD node healthcheck.js || exit 1

CMD ["node", "index.js"]
```

**healthcheck.js:**
```javascript
const http = require('http');

const options = {
  host: 'localhost',
  port: 8585,
  path: '/health',
  timeout: 2000
};

const request = http.request(options, (res) => {
  console.log(`Health check status: ${res.statusCode}`);
  if (res.statusCode === 200) {
    process.exit(0);
  } else {
    process.exit(1);
  }
});

request.on('error', (err) => {
  console.error('Health check failed:', err);
  process.exit(1);
});

request.end();
```

### 4. Load Balancer Configuration

**Nginx:**
```nginx
upstream lasantha_tire_bot {
    server localhost:8585;
    
    # Health check
    check interval=3000 rise=2 fall=3 timeout=1000 type=http;
    check_http_send "GET /health HTTP/1.0\r\n\r\n";
    check_http_expect_alive http_2xx http_3xx;
}

server {
    listen 80;
    server_name api.lasanthatyre.com;
    
    location / {
        proxy_pass http://lasantha_tire_bot;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

**HAProxy:**
```
backend lasantha_tire_bot
    mode http
    balance roundrobin
    option httpchk GET /health
    http-check expect status 200
    server bot1 localhost:8585 check inter 3000 rise 2 fall 3
```

### 5. Monitoring with Prometheus

Add Prometheus metrics endpoint:

```javascript
const prometheus = require('prom-client');

// Create a Registry
const register = new prometheus.Registry();

// Add default metrics
prometheus.collectDefaultMetrics({ register });

// Custom metrics
const httpRequestDuration = new prometheus.Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  labelNames: ['method', 'route', 'status_code'],
  buckets: [0.1, 0.5, 1, 2, 5]
});

register.registerMetric(httpRequestDuration);

// Metrics endpoint
app.get('/metrics', async (req, res) => {
  res.set('Content-Type', register.contentType);
  res.end(await register.metrics());
});
```

## Monitoring & Alerting

### 1. Uptime Monitoring

Use services like:
- **UptimeRobot** (Free tier: 50 monitors)
- **Pingdom** (Free trial)
- **StatusCake** (Free tier available)

Configuration:
```
URL: http://your-domain.com/health
Interval: 5 minutes
Alert if: Status code != 200
Alert channels: Email, SMS, Slack
```

### 2. Log Monitoring

Monitor health check logs:

```javascript
// Add logging to health checks
const logger = require('./utils/logger');

app.get('/health/detailed', async (req, res) => {
    const startTime = Date.now();
    
    try {
        // ... health checks ...
        
        const duration = Date.now() - startTime;
        logger.info('Health check completed', {
            status: health.status,
            duration,
            checks: health.checks
        });
        
        res.status(statusCode).json(health);
    } catch (error) {
        logger.error('Health check failed', { error: error.message });
        res.status(500).json({
            status: 'error',
            message: error.message
        });
    }
});
```

### 3. Alerting Rules

Example alerting configuration:

```yaml
# Prometheus Alert Rules
groups:
- name: lasantha_tire_bot
  rules:
  - alert: ServiceDown
    expr: up{job="lasantha-tire-bot"} == 0
    for: 5m
    labels:
      severity: critical
    annotations:
      summary: "Lasantha Tire Bot is down"
      
  - alert: HighErrorRate
    expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.05
    for: 5m
    labels:
      severity: warning
    annotations:
      summary: "High error rate detected"
      
  - alert: DatabaseDown
    expr: database_status{service="lasantha-tire-bot"} == 0
    for: 2m
    labels:
      severity: critical
    annotations:
      summary: "Database connection lost"
```

## Best Practices

1. **Response Time**: Keep health checks fast (<1 second)
2. **No Side Effects**: Health checks should not modify data
3. **Caching**: Cache expensive checks (e.g., database connection)
4. **Timeouts**: Set appropriate timeouts for external dependencies
5. **Status Codes**: Use proper HTTP status codes (200, 503)
6. **Logging**: Log health check failures for debugging
7. **Dependencies**: Check critical dependencies in readiness probe
8. **Graceful Degradation**: Return degraded status if non-critical services fail

## Security Considerations

1. **Authentication**: Consider adding authentication for detailed endpoints
2. **Rate Limiting**: Prevent abuse of health check endpoints
3. **Information Disclosure**: Don't expose sensitive information in error messages

```javascript
// Example: Protected health check
app.get('/health/detailed', authenticateToken, async (req, res) => {
    // ... health check logic ...
});

// Rate limiting
const rateLimit = require('express-rate-limit');

const healthCheckLimiter = rateLimit({
    windowMs: 1 * 60 * 1000, // 1 minute
    max: 60 // 60 requests per minute
});

app.get('/health', healthCheckLimiter, (req, res) => {
    // ... health check logic ...
});
```

## Testing

Test health check endpoints:

```javascript
// __tests__/health.test.js
const request = require('supertest');
const app = require('../index');

describe('Health Check Endpoints', () => {
    test('GET /health - returns 200', async () => {
        const response = await request(app)
            .get('/health')
            .expect(200);
        
        expect(response.body).toHaveProperty('status', 'healthy');
        expect(response.body).toHaveProperty('uptime');
    });

    test('GET /health/detailed - returns dependency status', async () => {
        const response = await request(app)
            .get('/health/detailed')
            .expect(200);
        
        expect(response.body.checks).toHaveProperty('database');
        expect(response.body.checks).toHaveProperty('whatsapp');
    });

    test('GET /ready - returns 200 when ready', async () => {
        await request(app)
            .get('/ready')
            .expect(200);
    });

    test('GET /alive - always returns 200', async () => {
        await request(app)
            .get('/alive')
            .expect(200);
    });
});
```

## Implementation Checklist

- [ ] Add basic `/health` endpoint
- [ ] Add detailed `/health/detailed` endpoint
- [ ] Add Kubernetes `/ready` probe
- [ ] Add Kubernetes `/alive` probe
- [ ] Add `/metrics` endpoint (optional)
- [ ] Configure load balancer health checks
- [ ] Setup uptime monitoring
- [ ] Configure alerting rules
- [ ] Add health check tests
- [ ] Document endpoints in API docs
- [ ] Update Kubernetes manifests
- [ ] Update Docker Compose files

---

**Document Version:** 1.0  
**Last Updated:** December 12, 2025  
**Related Documents:**
- PRODUCTION_READINESS_FINAL_REPORT.md
- IMPROVEMENT_RECOMMENDATIONS.md
