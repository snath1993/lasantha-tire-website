module.exports = {
  apps: [
    {
      name: 'lasantha-bot-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel --config C:\\whatsapp-sql-api\\bot-tunnel-config.yml run lasantha-bot-v2',
      autorestart: true,
      watch: false,
    },
    {
      name: 'lasantha-app-tunnel',
      script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
      args: 'tunnel run --protocol http2 --url http://127.0.0.1:3029 lasantha-app',
      autorestart: true,
      watch: false,
    }
  ]
};
