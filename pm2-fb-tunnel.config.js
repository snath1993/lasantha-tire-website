module.exports = {
  apps: [{
    name: 'lasantha-fb-tunnel',
    script: 'C:\\whatsapp-sql-api\\cloudflared.exe',
    interpreter: 'none',
    args: 'tunnel run --token eyJhIjoiNjc2YThkYzVlZjE3NjJjMzVmZjNiOWI1NmQxYmZmNzgiLCJzIjoiTW1FNU9EQTVPR0l0TmpNeU5TMDBNbUV5TFdFellqUXRZVEF6T1dVMk4yRmtNRGMxIiwidCI6IjRlMTg3ZTA0LThhOGMtNGY4YS05NzE0LWM5MmU4ODkzODllOSJ9',
    cwd: 'C:\\whatsapp-sql-api',
    autorestart: true,
    watch: false,
    max_restarts: 10,
    restart_delay: 5000
  }]
};
