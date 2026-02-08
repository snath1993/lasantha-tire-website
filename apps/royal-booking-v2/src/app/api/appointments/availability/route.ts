import { NextResponse } from 'next/server';

const BOT_API_URL = process.env.WHATSAPP_BOT_URL || 'http://localhost:8585';

// Compatibility endpoint for older booking UIs.
// Returns a simple list of booked slot times (e.g. ["08:00", "08:30"]) for a given date.
export async function GET(request: Request) {
  const { searchParams } = new URL(request.url);
  const date = searchParams.get('date');

  if (!date) {
    return NextResponse.json({ ok: false, error: 'Date is required' }, { status: 400 });
  }

  try {
    const res = await fetch(`${BOT_API_URL}/api/appointments/booked-slots?date=${date}` as string, {
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

    const data = (await res.json()) as { ok?: boolean; slotCounts?: Record<string, number> };

    const bookedSlots = Object.entries(data.slotCounts || {})
      .filter(([, count]) => Number(count) > 0)
      .map(([time]) => time);

    return NextResponse.json({ ok: true, bookedSlots });
  } catch (error) {
    console.error('Error proxying to bot:', error);
    return NextResponse.json(
      { ok: false, error: 'Internal Server Error' },
      { status: 500 }
    );
  }
}
