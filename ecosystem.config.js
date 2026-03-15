const path = require('path');
const ROOT = __dirname;
const cf = path.join(ROOT, process.env.CLOUDFLARED_PATH || 'cloudflared.exe');

module.exports = {
  apps: [
    {
      name: 'whatsapp-bot-v2',
      script: './index.js',
      cwd: ROOT,
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      windowsHide: true,
      kill_timeout: 8000,         // Give process 8s to clean up before SIGKILL
      max_memory_restart: '500M',
      restart_delay: 5000,        // Wait 5s before auto-restart on crash
      max_restarts: 50,           // Allow up to 50 restarts before PM2 gives up
      min_uptime: 10000,          // Process must run at least 10s to count as "started"
      exp_backoff_restart_delay: 1000, // Exponential backoff for rapid crash loops (1s, 2s, 4s...)
      env: {
        NODE_ENV: 'production',
        PORT: 8585
      }
    },
    {
      name: 'lasantha-tire-dashboard',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -H 0.0.0.0 -p 3028',
      cwd: path.join(ROOT, 'lasantha-tire-v1.5'),
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      windowsHide: true,
      env: {
        NODE_ENV: 'production',
        PORT: 3028,
        HOSTNAME: '0.0.0.0'
      }
    },
    {
      name: 'peachtree-bridge',
      script: path.join(ROOT, 'python32-portable', 'python.exe'),
      args: 'peachtree-odbc-bridge-32bit.py',
      cwd: ROOT,
      instances: 1,
      autorestart: true,
      watch: false,
      windowsHide: true,
      env: {
        PEACHTREE_BRIDGE_PORT: 5001
      }
    },
    {
      name: 'royal-booking-v2',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -p 3099',
      cwd: path.join(ROOT, 'apps', 'royal-booking-v2'),
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      windowsHide: true,
      env: {
        NODE_ENV: 'production',
        PORT: 3099
      }
    },
    {
      name: 'lasantha-tire-v2.0',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -H 0.0.0.0 -p 3029',
      cwd: path.join(ROOT, 'lasantha-tire-v2.0'),
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      windowsHide: true,
      env: {
        NODE_ENV: 'production',
        PORT: 3029,
        HOSTNAME: '0.0.0.0'
      }
    },
    {
      name: 'lasantha-app-tunnel',
      script: cf,
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:3029 lasantha-app',
      cwd: ROOT,
      instances: 1,
      autorestart: true,
      watch: false,
      windowsHide: true
    },
    {
      name: 'lasantha-bot-tunnel',
      script: cf,
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:8585 lasantha-bot-v2',
      cwd: ROOT,
      instances: 1,
      autorestart: true,
      watch: false,
      windowsHide: true
    },
    {
      name: 'lasantha-fb-tunnel',
      script: cf,
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:8585 lasantha-tyre-facebook',
      cwd: ROOT,
      instances: 1,
      autorestart: true,
      watch: false,
      windowsHide: true
    },
    {
      name: 'royal-booking-tunnel',
      script: cf,
      args: `tunnel --config ${path.join(ROOT, 'cloudflared-booking.yml')} run`,
      cwd: ROOT,
      instances: 1,
      autorestart: true,
      watch: false,
      windowsHide: true
    }
  ]
};
