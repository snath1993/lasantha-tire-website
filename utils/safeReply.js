/**
 * Safely send a reply to a WhatsApp message with error handling
 * @param {object} msg - WhatsApp message object
 * @param {object} client - WhatsApp client instance
 * @param {string} to - Recipient ID
 * @param {string} text - Message text to send
 * @returns {Promise<boolean>} - True if sent successfully
 */
async function safeReply(msg, client, to, text) {
    try {
        if (!msg || !text) {
            console.error('[SafeReply] Invalid parameters');
            return false;
        }
        
        // Try to reply to the message
        await msg.reply(text);
        console.log(`[SafeReply] ✅ Reply sent to ${to}`);
        return true;
    } catch (error) {
        console.error(`[SafeReply] ❌ Failed to send reply: ${error.message}`);
        
        // Fallback: try to send directly using client
        try {
            if (client && to) {
                await client.sendMessage(to, text);
                console.log(`[SafeReply] ✅ Fallback send successful to ${to}`);
                return true;
            }
        } catch (fallbackError) {
            console.error(`[SafeReply] ❌ Fallback also failed: ${fallbackError.message}`);
        }
        
        return false;
    }
}

module.exports = safeReply;
