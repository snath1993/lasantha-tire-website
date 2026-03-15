/**
 * Process Monitor — PM2 Crash Detection & WhatsApp Alerts
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * Monitors PM2 processes for restarts/crashes and sends WhatsApp alerts.
 * Also provides a daily health summary.
 */

const { exec } = require('child_process');
const path = require('path');
const fs = require('fs');

class ProcessMonitor {
    constructor(options = {}) {
        this.checkIntervalMs = options.checkIntervalMs || 30000; // 30s
        this.alertCooldownMs = options.alertCooldownMs || 300000; // 5min between alerts for same service
        this.adminNumbers = options.adminNumbers || [];
        this.sendFn = options.sendFn || null; // async (number, message) => {}
        this.lastRestartCounts = {};
        this.lastAlertTime = {};
        this.timer = null;
        this.startupTime = Date.now();
        this.dailyTimer = null;
    }

    start() {
        if (this.timer) return;
        console.log('[ProcessMonitor] ✅ Started (check every ' + (this.checkIntervalMs / 1000) + 's)');

        // Initial snapshot (don't alert on first run)
        this._snapshot().then(() => {
            this.timer = setInterval(() => this._check(), this.checkIntervalMs);
        });

        // Schedule daily health summary at 6:00 AM
        this._scheduleDailySummary();
    }

    stop() {
        if (this.timer) { clearInterval(this.timer); this.timer = null; }
        if (this.dailyTimer) { clearTimeout(this.dailyTimer); this.dailyTimer = null; }
    }

    async _snapshot() {
        const services = await this._getProcesses();
        for (const s of services) {
            this.lastRestartCounts[s.name] = s.restarts;
        }
    }

    async _check() {
        try {
            const services = await this._getProcesses();
            for (const s of services) {
                const prev = this.lastRestartCounts[s.name];

                // Detect restart
                if (prev !== undefined && s.restarts > prev) {
                    const diff = s.restarts - prev;
                    await this._alert(s.name, `🔄 *${s.name}* restarted ${diff} time(s)\nTotal restarts: ${s.restarts}\nStatus: ${s.status}\nMemory: ${this._formatBytes(s.memory)}`);
                }

                // Detect stopped
                if (prev !== undefined && s.status === 'stopped' && this.lastRestartCounts[s.name + '_status'] !== 'stopped') {
                    await this._alert(s.name, `⛔ *${s.name}* has STOPPED\nLast known restarts: ${s.restarts}`);
                }

                this.lastRestartCounts[s.name] = s.restarts;
                this.lastRestartCounts[s.name + '_status'] = s.status;
            }
        } catch (err) {
            console.error('[ProcessMonitor] Check error:', err.message);
        }
    }

    async _alert(serviceName, message) {
        const now = Date.now();
        const lastAlert = this.lastAlertTime[serviceName] || 0;
        if (now - lastAlert < this.alertCooldownMs) return; // Cooldown
        this.lastAlertTime[serviceName] = now;

        const fullMsg = `⚠️ *Process Monitor Alert*\n━━━━━━━━━━━━━━━\n${message}\n━━━━━━━━━━━━━━━\n🕐 ${new Date().toLocaleString('en-GB', { timeZone: 'Asia/Colombo' })}`;

        console.log(`[ProcessMonitor] 🚨 Alert: ${serviceName}`);

        if (this.sendFn && this.adminNumbers.length > 0) {
            for (const num of this.adminNumbers) {
                try {
                    await this.sendFn(num, fullMsg);
                } catch (e) {
                    console.error(`[ProcessMonitor] Failed to send alert to ${num}:`, e.message);
                }
            }
        }
    }

    async sendDailyHealthSummary() {
        try {
            const services = await this._getProcesses();
            const online = services.filter(s => s.status === 'online');
            const stopped = services.filter(s => s.status !== 'online');

            // Check database
            let dbOk = false;
            try {
                const pool = global.mainPool;
                if (pool) {
                    await pool.request().query('SELECT 1');
                    dbOk = true;
                }
            } catch {}

            // Check WhatsApp
            const waOk = !!(global.whatsappClient && (global.whatsappClient.isReady || global.whatsappClient.info?.wid));

            const uptimeHours = Math.floor((Date.now() - this.startupTime) / 3600000);
            const totalRestarts = services.reduce((sum, s) => sum + (s.restarts || 0), 0);

            const lines = [
                `📊 *Daily System Health Report*`,
                `━━━━━━━━━━━━━━━━━━━━━━`,
                ``,
                `📱 WhatsApp: ${waOk ? '✅ Connected' : '❌ Disconnected'}`,
                `🗄️ Database: ${dbOk ? '✅ Connected' : '❌ Error'}`,
                `⚙️ Services: ${online.length}/${services.length} online`,
                ``,
            ];

            if (stopped.length > 0) {
                lines.push(`⛔ Stopped: ${stopped.map(s => s.name).join(', ')}`);
                lines.push('');
            }

            lines.push(`⏱️ Bot Uptime: ${uptimeHours}h`);
            lines.push(`🔄 Total Restarts: ${totalRestarts}`);

            // Get bot memory
            const bot = services.find(s => s.name === 'whatsapp-bot-v2');
            if (bot) {
                lines.push(`💾 Bot Memory: ${this._formatBytes(bot.memory)}`);
            }

            lines.push('');
            lines.push(`🕐 ${new Date().toLocaleString('en-GB', { timeZone: 'Asia/Colombo' })}`);

            const msg = lines.join('\n');

            if (this.sendFn && this.adminNumbers.length > 0) {
                for (const num of this.adminNumbers) {
                    try { await this.sendFn(num, msg); } catch {}
                }
            }

            console.log('[ProcessMonitor] 📊 Daily health summary sent');
        } catch (err) {
            console.error('[ProcessMonitor] Daily summary error:', err.message);
        }
    }

    _scheduleDailySummary() {
        const now = new Date();
        const target = new Date();
        target.setHours(6, 0, 0, 0); // 6:00 AM
        if (target <= now) target.setDate(target.getDate() + 1);
        const ms = target.getTime() - now.getTime();

        this.dailyTimer = setTimeout(() => {
            this.sendDailyHealthSummary();
            // Reschedule for next day
            setInterval(() => this.sendDailyHealthSummary(), 24 * 60 * 60 * 1000);
        }, ms);

        console.log(`[ProcessMonitor] 📊 Daily health summary scheduled for 6:00 AM (in ${Math.floor(ms / 60000)}min)`);
    }

    _getProcesses() {
        return new Promise((resolve) => {
            exec('pm2 jlist', { windowsHide: true, timeout: 10000 }, (err, stdout) => {
                if (err) return resolve([]);
                try {
                    const raw = stdout.trim();
                    if (!raw || !raw.startsWith('[')) return resolve([]);
                    const data = JSON.parse(raw.replace(/"USERNAME":"[^"]*",/gi, '').replace(/"username":"[^"]*",/gi, ''));
                    resolve(data.map(p => ({
                        name: p.name,
                        status: p.pm2_env?.status || 'unknown',
                        restarts: p.pm2_env?.restart_time || 0,
                        memory: p.monit?.memory || 0,
                        cpu: p.monit?.cpu || 0,
                    })));
                } catch { resolve([]); }
            });
        });
    }

    _formatBytes(b) {
        if (!b) return '0B';
        if (b < 1048576) return (b / 1024).toFixed(0) + 'KB';
        return (b / 1048576).toFixed(1) + 'MB';
    }
}

module.exports = ProcessMonitor;
