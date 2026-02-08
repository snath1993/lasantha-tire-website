module.exports = {
  apps: [{
    name: 'lasantha-bot-tunnel',
    script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
    interpreter: 'none',
    args: 'tunnel run --protocol http2 --url http://127.0.0.1:3029 lasantha-bot-v2',
    cwd: 'C:\\whatsapp-sql-api',
    autorestart: true,
    watch: false,
    max_restarts: 10,
    restart_delay: 5000
  }]
};
