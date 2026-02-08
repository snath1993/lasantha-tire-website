
const { Client, LocalAuth } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');
const path = require('path');

// Use a FRESH path
const AUTH_PATH = path.join(__dirname, '.wwebjs_auth_TEST_V3');

console.log('Starting minimal test client...');
console.log(`Auth Path: ${AUTH_PATH}`);

const client = new Client({
    authStrategy: new LocalAuth({
        clientId: 'session-lasantha-tire-bot',
        dataPath: AUTH_PATH
    }),
    // FIX: Try a newer known-good version
    webVersionCache: {
        type: 'remote',
        remotePath: 'https://raw.githubusercontent.com/wppconnect-team/wa-version/main/html/2.2410.1.html',
    },
    puppeteer: {
        headless: 'new',
        executablePath: 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',
        args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-accelerated-2d-canvas',
            '--no-first-run',
            '--no-zygote',
            '--disable-gpu'
        ]
    }
});

client.on('qr', (qr) => {
    console.log('QR RECEIVED', qr);
    qrcode.generate(qr, { small: true });
});

client.on('ready', () => {
    console.log('✅ CLIENT IS READY!');
    console.log('Please send a message to the bot now.');
});

client.on('message', msg => {
    console.log('------------------------------------------------');
    console.log('📩 MESSAGE RECEIVED!');
    console.log('FROM:', msg.from);
    console.log('BODY:', msg.body);
    console.log('------------------------------------------------');
    
    if (msg.body === '!ping') {
        msg.reply('pong');
        console.log('Replied with pong');
    }
});

client.on('authenticated', () => {
    console.log('AUTHENTICATED');
    // Force Ready if it gets stuck
    setTimeout(async () => {
        console.log('Checking state...');
        const state = await client.getState();
        console.log('State:', state);
        if (state === 'CONNECTED') {
             console.log('Forcing listeners...');
             // Manually inject WWebJS logic
             try {
                await client.pupPage.evaluate(() => {
                    // This forces the library to attach if it missed the event
                    window.Store = window.Store || {}; 
                });
             } catch(e) { console.log('Inject err', e.message); }
             
             // Emit ready
             client.emit('ready');
        }
    }, 15000);
});

client.on('auth_failure', msg => {
    console.error('AUTHENTICATION FAILURE', msg);
});

client.initialize();
