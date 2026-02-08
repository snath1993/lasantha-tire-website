const http = require('http');
const fs = require('fs');
const path = require('path');

// Try to read port from runtime-port.json
let port = 8585;
try {
    const runtimeConfig = JSON.parse(fs.readFileSync(path.join(__dirname, '..', 'runtime-port.json'), 'utf8'));
    if (runtimeConfig.port) port = runtimeConfig.port;
} catch (e) {
    console.log('Could not read runtime-port.json, using default port 8585');
}

const url = `http://127.0.0.1:${port}/api/jobs/daily-sales-pdf/trigger`;

console.log(`Sending GET request to ${url}...`);

http.get(url, (res) => {
    let data = '';
    
    res.on('data', (chunk) => {
        data += chunk;
    });
    
    res.on('end', () => {
        console.log(`Status Code: ${res.statusCode}`);
        try {
            const json = JSON.parse(data);
            console.log('Response:', JSON.stringify(json, null, 2));
        } catch {
            console.log('Response:', data);
        }
    });

}).on('error', (err) => {
    console.error('Error connecting to bot API:', err.message);
});
