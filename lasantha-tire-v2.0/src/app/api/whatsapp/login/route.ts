import { NextResponse } from 'next/server';

export async function POST() {
  try {
    const BOT_API_URL = process.env.BOT_API_URL || 'http://127.0.0.1:8585';
    const response = await fetch(`${BOT_API_URL}/api/whatsapp/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' }
    });
    
    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.error || 'Failed to initialize WhatsApp');
    }
    
    return NextResponse.json(data);
  } catch (error: any) {
    return NextResponse.json(
      { error: error.message || 'Failed to initialize WhatsApp' },
      { status: 500 }
    );
  }
}
