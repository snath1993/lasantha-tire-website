import { NextResponse } from 'next/server';
export async function POST() {
  try {
    const BOT_API_URL = process.env.BOT_API_URL || 'http://127.0.0.1:8585';
    const response = await fetch(`${BOT_API_URL}/api/bot/swap`, {
      method: 'POST'
    });
    const data = await response.json();
    return NextResponse.json(data);
  } catch (error: any) {
    return NextResponse.json({ error: error.message }, { status: 500 });
  }
}