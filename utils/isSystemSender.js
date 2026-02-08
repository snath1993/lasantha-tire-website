// Detect system/broadcast or non-user senders
module.exports = function isSystemSender(from) {
    if (!from || typeof from !== 'string') return false;
    const lower = from.toLowerCase();
    // Common patterns: status@broadcast, broadcast, service numbers, group notifications
    if (lower.includes('broadcast') || lower.includes('status@') || lower.includes('notification@')) return true;
    return false;
};
