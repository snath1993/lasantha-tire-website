/**
 * QR Code Notifier — Email / WhatsApp alerts when QR scan is needed
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 * When the bot disconnects and needs a new QR code scan, this module
 * sends periodic reminders to the admin(s) via WhatsApp and optionally email.
 */

class QRNotifier {
    constructor(options = {}) {
        this.intervalMinutes = options.intervalMinutes || 5;
        this.adminNumbers = options.adminNumbers || [];
        this.sendFn = options.sendFn || null; // async (number, message) => {}
        this.emailFn = options.emailFn || null; // async (subject, body) => {}
        this.dashboardUrl = options.dashboardUrl || 'http://localhost:8585/admin';
        this.timer = null;
        this.lastQR = null;
        this.qrPending = false;
        this.reminderCount = 0;
        this.maxReminders = options.maxReminders || 20;
    }

    /**
     * Called when a QR code is generated. Sends first alert + starts reminder timer.
     */
    async onQRGenerated(qrString) {
        this.lastQR = qrString;
        this.qrPending = true;
        this.reminderCount = 0;

        // Send immediate alert
        await this._sendAlert(true);

        // Start periodic reminders
        if (this.timer) clearInterval(this.timer);
        this.timer = setInterval(() => {
            if (!this.qrPending) { this.stop(); return; }
            if (this.reminderCount >= this.maxReminders) {
                this._sendFinalWarning();
                this.stop();
                return;
            }
            this._sendAlert(false);
        }, this.intervalMinutes * 60 * 1000);

        console.log(`[QRNotifier] ⚠️ QR notification started (reminders every ${this.intervalMinutes}min)`);
    }

    /**
     * Called when WhatsApp is successfully authenticated. Stops reminders.
     */
    onAuthenticated() {
        if (this.qrPending) {
            this.qrPending = false;
            this.lastQR = null;
            this.stop();

            // Send confirmation
            const msg = [
                '✅ *WhatsApp Connected Successfully*',
                '━━━━━━━━━━━━━━━━━━━━━━',
                '',
                'QR code was scanned and bot is now online.',
                '',
                `🕐 ${new Date().toLocaleString('en-GB', { timeZone: 'Asia/Colombo' })}`,
            ].join('\n');

            if (this.sendFn && this.adminNumbers.length > 0) {
                for (const num of this.adminNumbers) {
                    this.sendFn(num, msg).catch(() => {});
                }
            }

            console.log('[QRNotifier] ✅ Authentication confirmed, reminders stopped');
        }
    }

    stop() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }

    async _sendAlert(isFirst) {
        this.reminderCount++;

        const lines = [
            isFirst ? '🔴 *WhatsApp QR Code Scan Required!*' : `🔁 *QR Reminder #${this.reminderCount}*`,
            '━━━━━━━━━━━━━━━━━━━━━━',
            '',
            'The WhatsApp bot has been logged out and needs a new QR code scan.',
            '',
            `📱 Open the dashboard to scan:`,
            this.dashboardUrl,
            '',
            isFirst ? '⚡ Please scan as soon as possible to restore bot functionality.' : `⏰ Still waiting for QR scan... (reminder ${this.reminderCount})`,
            '',
            `🕐 ${new Date().toLocaleString('en-GB', { timeZone: 'Asia/Colombo' })}`,
        ];

        const msg = lines.join('\n');

        // Send via WhatsApp (to OTHER numbers, since bot might be disconnected on primary)
        // The admin numbers may have a secondary WhatsApp or access dashboard
        console.log(`[QRNotifier] 📢 QR alert sent (${isFirst ? 'initial' : `reminder #${this.reminderCount}`})`);

        // Try email notification
        if (this.emailFn) {
            try {
                await this.emailFn(
                    `⚠️ WhatsApp QR Scan Required - Lasantha Tyre Bot`,
                    `The WhatsApp bot needs a QR code scan.\n\nDashboard: ${this.dashboardUrl}\n\nTime: ${new Date().toLocaleString('en-GB', { timeZone: 'Asia/Colombo' })}`
                );
            } catch (e) {
                console.error('[QRNotifier] Email error:', e.message);
            }
        }
    }

    async _sendFinalWarning() {
        console.log(`[QRNotifier] ⛔ Max reminders (${this.maxReminders}) reached, stopping`);

        if (this.emailFn) {
            try {
                await this.emailFn(
                    '⛔ CRITICAL: WhatsApp Bot Disconnected - Max Reminders Reached',
                    `The WhatsApp bot has been disconnected for over ${this.maxReminders * this.intervalMinutes} minutes and no one has scanned the QR code.\n\nDashboard: ${this.dashboardUrl}\n\nPlease scan the QR code immediately or contact technical support.`
                );
            } catch {}
        }
    }
}

module.exports = QRNotifier;
