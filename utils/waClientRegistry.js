/**
 * WhatsApp Client Registry
 * Maintains a global reference to the WhatsApp client
 */

let globalClient = null;

/**
 * Set the global WhatsApp client
 * @param {object} client - WhatsApp client instance
 */
function setClient(client) {
    globalClient = client;
    console.log('[Client Registry] WhatsApp client registered');
}

/**
 * Get the global WhatsApp client
 * @returns {object|null} - WhatsApp client or null
 */
function getClient() {
    return globalClient;
}

module.exports = {
    setClient,
    getClient
};
