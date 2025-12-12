import { NextResponse } from 'next/server';

export async function GET(
  _request: Request,
  { params }: { params: Promise<{ refId: string }> }
) {
  const { refId } = await params;
  // Use the public Cloudflare Tunnel URL by default if env var is not set
  const botUrl = process.env.WHATSAPP_BOT_URL || 'https://bot.lasanthatyre.com';

  try {
    const res = await fetch(`${botUrl}/api/quotations/${refId}`, {
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
