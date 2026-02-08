const http = require('http');

const data = JSON.stringify({
    tyreSize: '205/55R16',
    items: [
        {
            itemId: 'ITEM-001',
            description: 'Dunlop 205/55R16 SP Sport',
            brand: 'Dunlop',
            price: 25000,
            quantity: 4,
            isService: false
        },
        {
            itemId: 'SERV-001',
            description: 'Computerized Wheel Alignment',
            brand: 'Service',
            price: 2500,
            quantity: 1,
            isService: true
        }
    ],
    customerPhone: '0771234567',
    customerName: 'Test User',
    vehicleNumber: 'CAB-1234',
    totalAmount: 102500,
    messageContent: 'Test Quotation for Mobile View',
    source: 'Test Script',
    vatRate: 18,
    includeVat: true
});

const options = {
    hostname: '127.0.0.1',
    port: 8585,
    path: '/api/quotations',
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Content-Length': data.length
    }
};

const req = http.request(options, (res) => {
    let responseBody = '';

    res.on('data', (chunk) => {
        responseBody += chunk;
    });

    res.on('end', () => {
        try {
            const parsed = JSON.parse(responseBody);
            if (parsed.ok) {
                console.log('✅ Quotation Created Successfully!');
                console.log('RefCode:', parsed.refCode);
                console.log('Booking URL:', parsed.bookingUrl);
                
                // Construct local URL for testing
                const localUrl = `http://localhost:3099/?ref=${parsed.refCode}`;
                console.log('Local Test URL:', localUrl);
            } else {
                console.error('❌ Failed to create quotation:', parsed.error);
            }
        } catch (e) {
            console.error('❌ Error parsing response:', e);
            console.log('Raw Response:', responseBody);
        }
    });
});

req.on('error', (error) => {
    console.error('❌ Request Error:', error);
});

req.write(data);
req.end();
