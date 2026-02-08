module.exports = {
  apps: [
    {
      name: 'whatsapp-bot',
      script: './index.js',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      max_memory_restart: '500M',
      env: {
        NODE_ENV: 'production',
        PORT: 8585
      }
    },
    {
      name: 'lasantha-tire-dashboard',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -H 0.0.0.0 -p 3028',
      cwd: 'C:\\whatsapp-sql-api\\lasantha-tire-v1.5',
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      env: {
        NODE_ENV: 'production',
        PORT: 3028,
        HOSTNAME: '0.0.0.0'
      }
    },
    {
      name: 'peachtree-bridge',
      script: 'C:\\whatsapp-sql-api\\python32-portable\\python.exe',
      args: 'peachtree-odbc-bridge-32bit.py',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      autorestart: true,
      watch: false,
      env: {
        PEACHTREE_BRIDGE_PORT: 5001
      }
    },
    {
      name: 'royal-booking-v2',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -p 3099',
      cwd: 'C:\\whatsapp-sql-api\\apps\\royal-booking-v2',
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      env: {
        NODE_ENV: 'production',
        PORT: 3099
      }
    },
    {
      name: 'lasantha-tire-v2.0',
      script: 'node_modules/next/dist/bin/next',
      args: 'start -H 0.0.0.0 -p 3029',
      cwd: 'C:\\whatsapp-sql-api\\lasantha-tire-v2.0',
      instances: 1,
      exec_mode: 'fork',
      autorestart: true,
      watch: false,
      env: {
        NODE_ENV: 'production',
        PORT: 3029,
        HOSTNAME: '0.0.0.0'
      }
    },
    {
      name: 'lasantha-app-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:3029 lasantha-app',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      autorestart: true,
      watch: false
    },
    {
      name: 'lasantha-bot-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:8585 lasantha-bot-v2',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      autorestart: true,
      watch: false
    },
    {
      name: 'lasantha-fb-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:8585 lasantha-tyre-facebook',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      autorestart: true,
      watch: false
    },
    {
      name: 'royal-booking-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel --config C:\\whatsapp-sql-api\\cloudflared-booking.yml run',
      cwd: 'C:\\whatsapp-sql-api',
      instances: 1,
      autorestart: true,
      watch: false
    }
  ]
};
