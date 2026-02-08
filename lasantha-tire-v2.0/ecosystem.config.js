module.exports = {
  apps : [{
    name: "lasantha-tire-v2.0",
    script: "node_modules/next/dist/bin/next",
    args: "start -H 0.0.0.0 -p 3029",
    cwd: "c:\\whatsapp-sql-api\\lasantha-tire-v2.0",
    env: {
      NODE_ENV: "production",
      PORT: 3029
    }
  }]
}