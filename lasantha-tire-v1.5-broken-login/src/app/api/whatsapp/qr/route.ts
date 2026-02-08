import { NextResponse } from 'next/server';

export async function GET() {
  try {
    const BOT_API_URL = process.env.BOT_API_URL || 'http://127.0.0.1:3100';
    const response = await fetch(`${BOT_API_URL}/api/whatsapp/qr`, { next: { revalidate: 0 } });
    const data = await response.json();

    return NextResponse.json({
      ok: data.ok !== false,
      hasQR: !!data.qr,
      qr: data.qr || null,
      qrCode: data.qr || null,
      timestamp: new Date().toISOString(),
    });
  } catch (error: any) {
    return NextResponse.json(
      { ok: false, error: error.message || 'Failed to fetch QR' },
      { status: 500 }
    );
  }
}
