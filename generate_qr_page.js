
const QRCode = require('qrcode');
const http = require('http');
const fs = require('fs');

// Path to the log file where the QR code might be (as a backup/fallback if needed)
const LOG_FILE = 'C:\\Users\\Cashi\\.pm2\\logs\\whatsapp-bot-out-0.log';

// We will try running a small script that listens to the PM2 stream or reads the log
// but for now, let's create a server that listens to the 'qr' event if we can import the bot? 
// No, the bot is a separate process. We need to hook into the existing bot.

// THE BOT ALREADY HAS AN ENDPOINT: /api/whatsapp/qr
// Let's verify if it actually WORKS.

const fetch = require('node-fetch'); // You might need to install this if not available, or use http

async function checkQREndpoint() {
    try {
        // Simple fetch using http native to avoid node-fetch dependency issues if any
        const http = require('http');
        
        return new Promise((resolve) => {
             http.get('http://localhost:8585/api/whatsapp/qr', (res) => {
                let data = '';
                res.on('data', (chunk) => data += chunk);
                res.on('end', async () => {
                    try {
                        const json = JSON.parse(data);
                        console.log('QR Code String Found:', json.qr ? json.qr.substring(0, 20) + '...' : 'NONE');
                        
                        if (json.qr) {
                            const qrImage = await QRCode.toDataURL(json.qr);
                            // Auto-refresh script added
                            const html = `
                            <html>
                                <head>
                                    <meta http-equiv="refresh" content="5">
                                    <title>WhatsApp Login</title>
                                </head>
                                <body style="display:flex;justify-content:center;align-items:center;height:100vh;background:#f0f0f0;font-family:sans-serif;">
                                    <div style="text-align:center;background:white;padding:40px;border-radius:10px;box-shadow:0 10px 25px rgba(0,0,0,0.1);">
                                        <h1 style="margin-bottom:20px;color:#075e54;">WhatsApp Login</h1>
                                        <div style="border: 4px solid #128c7e; padding: 10px; display:inline-block; border-radius: 8px;">
                                            <img src="${qrImage}" style="width:300px;height:300px;display:block;" />
                                        </div>
                                        <p style="margin-top:20px;color:#555;">Scan this with your WhatsApp (Linked Devices)</p>
                                        <p style="font-size:12px;color:#888;">Use 'Link a Device' in WhatsApp Settings</p>
                                        <div id="countdown" style="margin-top:10px;font-weight:bold;color:#128c7e;">Refreshing in 5s...</div>
                                    </div>
                                </body>
                            </html>
                            `;
                            fs.writeFileSync('c:\\whatsapp-sql-api\\login.html', html);
                            console.log('SUCCESS: Login file created at c:\\whatsapp-sql-api\\login.html');
                        } else {
                             console.log('API returned NO QR code (maybe already logged in? or initializing)');
                        }
                    } catch (e) {
                        console.log('Error parsing JSON:', e.message);
                    }
                    resolve();
                });
             }).on('error', (err) => {
                 console.log('Error calling API:', err.message);
                 resolve();
             });
        });

    } catch (e) {
        console.error('Failed to fetch QR from API:', e.message);
    }
}

checkQREndpoint();

// Since node-fetch isn't guaranteed, let's use standard http
const getQR = () => {
    http.get('http://localhost:8585/api/whatsapp/qr', (res) => {
        let data = '';
        res.on('data', (chunk) => data += chunk);
        res.on('end', async () => {
            try {
                const json = JSON.parse(data);
                if (json.qr) {
                    console.log("QR Code String Found:", json.qr.substring(0, 20) + "...");
                    const qrImage = await QRCode.toDataURL(json.qr);
                     const html = `
                        <html>
                            <head>
                                <meta http-equiv="refresh" content="5">
                                <title>Lasantha Tire Bot Login</title>
                            </head>
                            <body style="display:flex;justify-content:center;align-items:center;height:100vh;background:#25D366;font-family:sans-serif;">
                                <div style="text-align:center;background:white;padding:40px;border-radius:10px;box-shadow:0 10px 25px rgba(0,0,0,0.2);">
                                    <h1 style="color:#075E54;margin-bottom:20px;">Lasantha Tire Bot</h1>
                                    <div style="background:white;padding:10px;display:inline-block;">
                                        <img src="${qrImage}" style="width:300px;height:300px;display:block;" />
                                    </div>
                                    <p style="color:#555;margin-top:20px;">Open WhatsApp > Linked Devices > Link a Device</p>
                                    <p style="font-size:12px;color:#999;">Reloads every 5s</p>
                                </div>
                            </body>
                        </html>
                    `;
                    fs.writeFileSync('c:\\whatsapp-sql-api\\login.html', html);
                    console.log('SUCCESS: Login file created at c:\\whatsapp-sql-api\\login.html');
                } else {
                    console.log('API returned NO QR code (maybe already logged in? or initializing)');
                    // console.log(json);
                }
            } catch (e) {
                console.error('Error parsing JSON:', e.message);
            }
        });
    }).on('error', (err) => {
        console.error('Error connecting to bot API:', err.message);
    });
};

getQR();
