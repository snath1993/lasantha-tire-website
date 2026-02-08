// safeReply(msg, client, to, text)
// NOTE: WhatsApp Web has been intermittently failing inside sendSeen() with:
//   "Cannot read properties of undefined (reading 'markedUnread')"
// To keep replies reliable, we prefer client.sendMessage with sendSeen=false.
module.exports = async function safeReply(msg, client, to, text) {
    const payload = (text === undefined || text === null) ? '' : String(text);

    const shouldRetryAfterReady = (err) => {
        const m = (err && err.message) ? String(err.message) : String(err || '');
        return (
            m.includes('Client not ready') ||
            m.includes('Connection Closed') ||
            m.includes('Precondition Required') ||
            m.includes('statusCode: 428')
        );
    };

    // 1) client.sendMessage (preferred)
    try {
        if (client && typeof client.sendMessage === 'function') {
            await client.sendMessage(to, payload, { sendSeen: false });
            return { ok: true, via: 'client.sendMessage' };
        }
    } catch (e) {
        console.error('[safeReply] client.sendMessage failed:', e && e.message ? e.message : e);

        // Transient reconnect case (Baileys): wait for ready and retry once.
        try {
            if (client && typeof client.waitUntilReady === 'function' && shouldRetryAfterReady(e)) {
                await client.waitUntilReady(15000);
                await client.sendMessage(to, payload, { sendSeen: false });
                return { ok: true, via: 'client.sendMessage(retry-after-ready)' };
            }
        } catch (retryErr) {
            console.error('[safeReply] retry-after-ready failed:', retryErr && retryErr.message ? retryErr.message : retryErr);
        }
    }

    // 2) chat.sendMessage (fallback for linked-device @lid cases)
    try {
        if (msg && typeof msg.getChat === 'function') {
            const chat = await msg.getChat();
            if (chat && typeof chat.sendMessage === 'function') {
                await chat.sendMessage(payload, { sendSeen: false });
                return { ok: true, via: 'chat.sendMessage' };
            }
        }
    } catch (e) {
        console.error('[safeReply] chat.sendMessage failed:', e && e.message ? e.message : e);
    }

    // 3) msg.reply (last resort)
    try {
        if (msg && typeof msg.reply === 'function') {
            await msg.reply(payload);
            return { ok: true, via: 'msg.reply' };
        }
    } catch (e) {
        console.error('[safeReply] msg.reply failed:', e && e.message ? e.message : e);
        return { ok: false, error: e.message || e };
    }

    console.warn('[safeReply] No valid send method available. to=', to);
    return { ok: false, error: 'no-send-method' };
};
