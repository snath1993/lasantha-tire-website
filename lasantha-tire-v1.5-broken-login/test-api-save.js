const fetch = require('node-fetch'); // You might need to install this or use built-in fetch in Node 18+

async function testApi() {
  try {
    const payload = {
      customerName: 'API Test Customer',
      vehicleNo: 'API-TEST-001',
      date: new Date().toISOString(),
      terms: 'Test Terms',
      items: [{ id: 1, name: 'Test Item', Quantity: 1, UnitPrice: 100 }],
      includeVat: true,
      totalAmount: 100
    };

    console.log('Sending request to http://localhost:3028/api/quotations/save');
    const res = await fetch('http://localhost:3028/api/quotations/save', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    const text = await res.text();
    console.log('Status:', res.status);
    console.log('Response:', text);

  } catch (err) {
    console.error('Error:', err);
  }
}

testApi();
