import { NextRequest, NextResponse } from 'next/server';

const BOT_API_URL = process.env.BOT_API_URL || 'http://localhost:8585';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const response = await fetch(`${BOT_API_URL}/api/send-media`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      return NextResponse.json(
        { ok: false, error: errorData.error || 'Bot API request failed' },
        { status: response.status }
      );
    }

    const data = await response.json();
    return NextResponse.json(data);
  } catch (error) {
    console.error('Bot proxy error:', error);
    return NextResponse.json(
      { ok: false, error: 'Internal server error' },
      { status: 500 }
    );
  }
}
