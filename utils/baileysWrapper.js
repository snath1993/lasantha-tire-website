/**
 * Baileys Wrapper - whatsapp-web.js compatible API
 * This wrapper provides the same interface as whatsapp-web.js but uses Baileys internally
 * Updated for Baileys 7.x
 */

const { makeWASocket, useMultiFileAuthState, DisconnectReason, fetchLatestBaileysVersion, makeCacheableSignalKeyStore, downloadMediaMessage } = require('@whiskeysockets/baileys');
const { Boom } = require('@hapi/boom');
const pino = require('pino');
const fs = require('fs');
const path = require('path');
const EventEmitter = require('events');

// Suppress Baileys internal logs
const logger = pino({ level: 'silent' });

// Simple in-memory store (Baileys 7.x doesn't export makeInMemoryStore)
class SimpleStore {
    constructor() {
        this.chats = new Map();
        this.contacts = {};
        this.messages = new Map();
    }
    
    bind(ev) {
        ev.on('chats.upsert', (chats) => {
            for (const chat of chats) {
                this.chats.set(chat.id, chat);
            }
        });
        ev.on('contacts.upsert', (contacts) => {
            for (const contact of contacts) {
                this.contacts[contact.id] = contact;
            }
        });
        ev.on('messages.upsert', ({ messages }) => {
            for (const msg of messages) {
                const key = msg.key.remoteJid;
                if (!this.messages.has(key)) {
                    this.messages.set(key, []);
                }
                this.messages.get(key).push(msg);
            }
        });
    }
    
    all() {
        return Array.from(this.chats.values());
    }
    
    get(id) {
        return this.chats.get(id);
    }
}

const store = new SimpleStore();

class BaileysClient extends EventEmitter {
    constructor(options = {}) {
        super();
        this.options = options;
        this.sock = null;
        this.info = null;
        this.isReady = false;
        this.authPath = options.authPath || path.join(process.cwd(), '.baileys_auth');
        this.saveCreds = null;
        this._qrRetryCount = 0;
        this._maxQrRetries = options.qrMaxRetries || 0; // 0 = unlimited

        // This client often has many feature modules attaching listeners.
        // Increase limit to avoid noisy MaxListenersExceededWarning.
        this.setMaxListeners(Math.max(this.getMaxListeners(), 50));
    }

    /**
     * Wait until the client is connected/ready.
     * Useful for retrying sends during transient reconnects.
     */
    waitUntilReady(timeoutMs = 15000) {
        if (this.isReady) return Promise.resolve(true);

        return new Promise((resolve, reject) => {
            const timeout = setTimeout(() => {
                cleanup();
                reject(new Error(`Client not ready after ${timeoutMs}ms`));
            }, timeoutMs);

            const onReady = () => {
                cleanup();
                resolve(true);
            };
            const onAuthFailure = (msg) => {
                cleanup();
                reject(new Error(`Auth failure: ${msg || 'unknown'}`));
            };
            const onDisconnected = (reason) => {
                // Don't immediately reject for transient disconnects; reconnect may still happen.
                // But if we know we're logged out, fail fast.
                if (String(reason).toUpperCase().includes('LOGOUT')) {
                    cleanup();
                    reject(new Error('Logged out'));
                }
            };

            const cleanup = () => {
                clearTimeout(timeout);
                this.off('ready', onReady);
                this.off('auth_failure', onAuthFailure);
                this.off('disconnected', onDisconnected);
            };

            this.on('ready', onReady);
            this.on('auth_failure', onAuthFailure);
            this.on('disconnected', onDisconnected);
        });
    }

    async initialize() {
        try {
            // Ensure auth directory exists
            if (!fs.existsSync(this.authPath)) {
                fs.mkdirSync(this.authPath, { recursive: true });
            }

            // Load authentication state
            const { state, saveCreds } = await useMultiFileAuthState(this.authPath);
            this.saveCreds = saveCreds;

            // Fetch latest Baileys version
            const { version, isLatest } = await fetchLatestBaileysVersion();
            console.log(`[Baileys] Using WA v${version.join('.')}, isLatest: ${isLatest}`);

            // Create socket
            this.sock = makeWASocket({
                version,
                logger,
                printQRInTerminal: false, // We'll handle QR ourselves
                auth: {
                    creds: state.creds,
                    keys: makeCacheableSignalKeyStore(state.keys, logger),
                },
                browser: ['Lasantha Tire Bot', 'Chrome', '120.0.0'],
                generateHighQualityLinkPreview: true,
                syncFullHistory: false,
                markOnlineOnConnect: false,
            });

            // Bind store to socket events
            store.bind(this.sock.ev);

            // Setup event handlers
            this._setupEventHandlers();

            return this;
        } catch (error) {
            console.error('[Baileys] Initialize error:', error);
            this.emit('auth_failure', error.message);
            throw error;
        }
    }

    _setupEventHandlers() {
        // Connection updates (QR, connected, disconnected)
        this.sock.ev.on('connection.update', async (update) => {
            const { connection, lastDisconnect, qr } = update;

            if (qr) {
                this._qrRetryCount++;
                if (this._maxQrRetries > 0 && this._qrRetryCount > this._maxQrRetries) {
                    console.log('[Baileys] Max QR retries reached');
                    this.emit('disconnected', 'Max qrcode retries reached');
                    return;
                }
                // Emit QR in whatsapp-web.js format
                this.emit('qr', qr);
            }

            if (connection === 'close') {
                const statusCode = (lastDisconnect?.error instanceof Boom) 
                    ? lastDisconnect.error.output?.statusCode 
                    : null;
                const shouldReconnect = statusCode !== DisconnectReason.loggedOut;
                
                console.log(`[Baileys] Connection closed. Status: ${statusCode}, Reconnect: ${shouldReconnect}`);
                
                this.isReady = false;
                this.info = null;

                if (statusCode === DisconnectReason.loggedOut) {
                    this.emit('auth_failure', 'Session logged out');
                    this.emit('disconnected', 'LOGOUT');
                } else if (shouldReconnect) {
                    this.emit('disconnected', 'NAVIGATION');
                    // Auto-reconnect
                    setTimeout(() => this.initialize(), 3000);
                } else {
                    this.emit('disconnected', lastDisconnect?.error?.message || 'Unknown');
                }
            }

            if (connection === 'connecting') {
                this.emit('loading_screen', 50, 'Connecting to WhatsApp...');
            }

            if (connection === 'open') {
                this._qrRetryCount = 0;
                this.isReady = true;
                
                // Get user info
                const user = this.sock.user;
                if (user) {
                    this.info = {
                        wid: {
                            user: user.id.split(':')[0].split('@')[0],
                            _serialized: user.id
                        },
                        pushname: user.name || 'Lasantha Tire Bot',
                        platform: 'smbi'
                    };
                }

                this.emit('authenticated');
                this.emit('ready');
            }
        });

        // Credentials update
        this.sock.ev.on('creds.update', async () => {
            if (this.saveCreds) {
                await this.saveCreds();
            }
        });

        // Messages received
        this.sock.ev.on('messages.upsert', async ({ messages, type }) => {
            if (type !== 'notify') return;

            for (const msg of messages) {
                // Convert to whatsapp-web.js format
                const convertedMsg = this._convertMessage(msg);
                
                if (msg.key.fromMe) {
                    this.emit('message_create', convertedMsg);
                } else {
                    this.emit('message', convertedMsg);
                }
            }
        });

        // Message status updates (sent, delivered, read)
        this.sock.ev.on('messages.update', async (updates) => {
            for (const update of updates) {
                if (update.update.status) {
                    const msg = this._convertMessageUpdate(update);
                    this.emit('message_ack', msg, update.update.status);
                }
            }
        });
    }

    _convertMessage(msg) {
        const jid = msg.key.remoteJid;
        const isGroup = jid.endsWith('@g.us');
        const from = jid;
        const body = msg.message?.conversation 
            || msg.message?.extendedTextMessage?.text 
            || msg.message?.imageMessage?.caption
            || msg.message?.videoMessage?.caption
            || msg.message?.documentMessage?.caption
            || '';
        
        const hasMedia = !!(
            msg.message?.imageMessage ||
            msg.message?.videoMessage ||
            msg.message?.audioMessage ||
            msg.message?.documentMessage ||
            msg.message?.stickerMessage
        );

        let type = 'chat';
        if (msg.message?.imageMessage) type = 'image';
        else if (msg.message?.videoMessage) type = 'video';
        else if (msg.message?.audioMessage) type = 'audio';
        else if (msg.message?.documentMessage) type = 'document';
        else if (msg.message?.stickerMessage) type = 'sticker';

        const converted = {
            id: {
                fromMe: msg.key.fromMe,
                remote: jid,
                id: msg.key.id,
                _serialized: `${msg.key.fromMe}_${jid}_${msg.key.id}`
            },
            body,
            from,
            to: msg.key.fromMe ? jid : this.info?.wid?._serialized,
            fromMe: msg.key.fromMe,
            hasMedia,
            type,
            timestamp: msg.messageTimestamp,
            author: isGroup ? (msg.key.participant || msg.key.remoteJid) : undefined,
            isGroup,
            ack: msg.status || 0,
            // Store original message for media download
            _data: msg,

            // Methods (whatsapp-web.js compatible)
            reply: async (content, chatId, options = {}) => {
                return this.sendMessage(from, content, { ...options, quoted: msg });
            },
            getChat: async () => {
                return this.getChatById(from);
            },
            getContact: async () => {
                return this.getContactById(from);
            },
            downloadMedia: async () => {
                return this._downloadMedia(msg);
            }
        };

        return converted;
    }

    _convertMessageUpdate(update) {
        return {
            id: {
                fromMe: update.key.fromMe,
                remote: update.key.remoteJid,
                id: update.key.id,
                _serialized: `${update.key.fromMe}_${update.key.remoteJid}_${update.key.id}`
            },
            ack: update.update.status
        };
    }

    async _downloadMedia(msg) {
        try {
            const buffer = await downloadMediaMessage(
                msg,
                'buffer',
                {},
                { 
                    logger,
                    reuploadRequest: this.sock.updateMediaMessage
                }
            );

            let mimetype = 'application/octet-stream';
            let filename = 'file';

            if (msg.message?.imageMessage) {
                mimetype = msg.message.imageMessage.mimetype || 'image/jpeg';
                filename = 'image.jpg';
            } else if (msg.message?.videoMessage) {
                mimetype = msg.message.videoMessage.mimetype || 'video/mp4';
                filename = 'video.mp4';
            } else if (msg.message?.audioMessage) {
                mimetype = msg.message.audioMessage.mimetype || 'audio/ogg';
                filename = 'audio.ogg';
            } else if (msg.message?.documentMessage) {
                mimetype = msg.message.documentMessage.mimetype || 'application/octet-stream';
                filename = msg.message.documentMessage.fileName || 'document';
            }

            return {
                data: buffer.toString('base64'),
                mimetype,
                filename
            };
        } catch (error) {
            console.error('[Baileys] Download media error:', error);
            return null;
        }
    }

    // ============================================
    // whatsapp-web.js Compatible Methods
    // ============================================

    async sendMessage(chatId, content, options = {}) {
        if (!this.sock || !this.isReady) {
            throw new Error('Client not ready');
        }

        const jid = this._formatJid(chatId);
        let messageContent = {};

        // Handle different content types
        if (typeof content === 'string') {
            messageContent = { text: content };
        } else if (content && content.mimetype) {
            // MessageMedia object
            const buffer = Buffer.from(content.data, 'base64');
            
            if (content.mimetype.startsWith('image/')) {
                messageContent = {
                    image: buffer,
                    caption: options.caption || '',
                    mimetype: content.mimetype
                };
            } else if (content.mimetype.startsWith('video/')) {
                messageContent = {
                    video: buffer,
                    caption: options.caption || '',
                    mimetype: content.mimetype
                };
            } else if (content.mimetype.startsWith('audio/')) {
                messageContent = {
                    audio: buffer,
                    mimetype: content.mimetype,
                    ptt: options.ptt || false
                };
            } else {
                // Document
                messageContent = {
                    document: buffer,
                    mimetype: content.mimetype,
                    fileName: content.filename || 'file',
                    caption: options.caption || ''
                };
            }
        }

        // Add quoted message if replying
        if (options.quoted && options.quoted._data) {
            messageContent.quoted = options.quoted._data;
        }

        try {
            const result = await this.sock.sendMessage(jid, messageContent);
            return this._convertMessage({ ...result, key: result.key, message: result.message });
        } catch (error) {
            console.error('[Baileys] Send message error:', error);
            throw error;
        }
    }
    
    /**
     * Send typing indicator (composing presence)
     * @param {string} chatId - The JID of the chat
     */
    async sendTyping(chatId) {
        if (!this.sock || !this.isReady) return;
        try {
            const jid = this._formatJid(chatId);
            await this.sock.sendPresenceUpdate('composing', jid);
        } catch (err) {
            console.error('[Baileys] Error sending presence:', err.message);
        }
    }

    async getChats() {
        const chats = store.all();
        return chats.map(chat => ({
            id: { _serialized: chat.id },
            name: chat.name || chat.id,
            isGroup: chat.id.endsWith('@g.us'),
            unreadCount: chat.unreadCount || 0,
            timestamp: chat.conversationTimestamp,
        }));
    }

    async getChatById(chatId) {
        const jid = this._formatJid(chatId);
        const chat = store.get(jid);
        
        return {
            id: { _serialized: jid },
            name: chat?.name || jid,
            isGroup: jid.endsWith('@g.us'),
            unreadCount: chat?.unreadCount || 0,
            sendMessage: async (content, options) => this.sendMessage(jid, content, options),
            clearState: async () => { /* no-op for compatibility */ }
        };
    }

    async getContacts() {
        const contacts = store.contacts;
        return Object.values(contacts).map(contact => ({
            id: { _serialized: contact.id },
            name: contact.name || contact.notify || contact.id,
            pushname: contact.notify,
            isMyContact: true,
            isUser: !contact.id.endsWith('@g.us'),
            isGroup: contact.id.endsWith('@g.us'),
        }));
    }

    async getContactById(contactId) {
        const jid = this._formatJid(contactId);
        const contact = store.contacts[jid];
        
        return {
            id: { _serialized: jid },
            name: contact?.name || contact?.notify || jid,
            pushname: contact?.notify,
            isMyContact: !!contact,
            isUser: !jid.endsWith('@g.us'),
            isGroup: jid.endsWith('@g.us'),
        };
    }

    async getState() {
        if (this.sock && this.isReady) {
            return 'CONNECTED';
        }
        return 'DISCONNECTED';
    }

    async logout() {
        try {
            if (this.sock) {
                await this.sock.logout();
            }
            // Clear auth folder
            if (fs.existsSync(this.authPath)) {
                fs.rmSync(this.authPath, { recursive: true, force: true });
            }
            this.isReady = false;
            this.info = null;
        } catch (error) {
            console.error('[Baileys] Logout error:', error);
        }
    }

    async destroy() {
        try {
            // Remove all listeners first to prevent memory leaks
            this.removeAllListeners();
            
            if (this.sock) {
                // Remove socket event listeners before ending
                if (this.sock.ev) {
                    this.sock.ev.removeAllListeners();
                }
                this.sock.end();
                this.sock = null;
            }
            this.isReady = false;
            this.info = null;
        } catch (error) {
            console.error('[Baileys] Destroy error:', error);
        }
    }

    // Inject method for compatibility (no-op in Baileys)
    async inject() {
        // Baileys doesn't need injection - events are already set up
        return true;
    }

    // Helper to format JID
    _formatJid(id) {
        if (!id) return id;
        // Remove any existing suffix and add proper one
        const cleaned = id.replace(/@[cgs]\.us$/, '').replace(/@s\.whatsapp\.net$/, '');
        // Check if it's a group
        if (id.includes('@g.us') || id.includes('-')) {
            return cleaned.includes('@') ? cleaned : `${cleaned}@g.us`;
        }
        return `${cleaned}@s.whatsapp.net`;
    }
}

// MessageMedia compatible class
class MessageMedia {
    constructor(mimetype, data, filename = null) {
        this.mimetype = mimetype;
        this.data = data;
        this.filename = filename;
    }

    static fromFilePath(filePath) {
        const fs = require('fs');
        const path = require('path');
        const mime = require('mime-types');

        const data = fs.readFileSync(filePath).toString('base64');
        const mimetype = mime.lookup(filePath) || 'application/octet-stream';
        const filename = path.basename(filePath);

        return new MessageMedia(mimetype, data, filename);
    }

    static fromUrl(url, options = {}) {
        // For compatibility - would need to implement URL fetching
        throw new Error('MessageMedia.fromUrl not yet implemented in Baileys wrapper');
    }
}

// LocalAuth compatible class (maps to Baileys auth)
class LocalAuth {
    constructor(options = {}) {
        this.clientId = options.clientId || 'default';
        this.dataPath = options.dataPath || path.join(process.cwd(), '.baileys_auth');
    }

    getAuthPath() {
        return path.join(this.dataPath, `session-${this.clientId}`);
    }
}

module.exports = {
    Client: BaileysClient,
    MessageMedia,
    LocalAuth,
    // Export store for advanced usage
    store
};
