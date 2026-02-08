import { NextResponse } from 'next/server';

export async function POST(req: Request) {
  try {
    const body = await req.json();
    
    // Forward to the local Bot API running on port 8585
    const response = await fetch('http://127.0.0.1:8585/api/titan/message', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorText = await response.text();
      return NextResponse.json({ error: `Bot API error: ${errorText}` }, { status: response.status });
    }

    const data = await response.json();
    return NextResponse.json(data);
  } catch (error: any) {
    console.error('Titan Proxy Error:', error);
    return NextResponse.json({ error: error.message }, { status: 500 });
  }
}
