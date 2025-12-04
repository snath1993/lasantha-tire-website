import { NextResponse } from 'next/server';

export async function POST() {
  try {
    const response = await fetch('http://localhost:3100/api/whatsapp/login', {
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
