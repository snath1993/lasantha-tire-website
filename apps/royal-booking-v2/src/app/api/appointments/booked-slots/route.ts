import { NextResponse } from 'next/server';

const BOT_API_URL = process.env.WHATSAPP_BOT_URL || 'http://localhost:8585';

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url);
  const date = searchParams.get('date');

  if (!date) {
    return NextResponse.json({ ok: false, error: 'Date is required' }, { status: 400 });
  }

  try {
    const res = await fetch(`${BOT_API_URL}/api/appointments/booked-slots?date=${date}`, {
      cache: 'no-store',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!res.ok) {
      return NextResponse.json(
        { ok: false, error: 'Failed to fetch from bot' },
        { status: res.status }
      );
    }

    const data = await res.json();
    return NextResponse.json(data);
  } catch (error) {
    console.error('Error proxying to bot:', error);
    return NextResponse.json(
      { ok: false, error: 'Internal Server Error' },
      { status: 500 }
    );
  }
}
