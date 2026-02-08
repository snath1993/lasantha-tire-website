import { NextResponse } from 'next/server';

export async function GET() {
  try {
    // Fetch status from the separate Bot process
    // The bot runs on port 3100 by default
    const botResponse = await fetch('http://localhost:8585/health', { 
      next: { revalidate: 0 },
      signal: AbortSignal.timeout(5000) // 5s timeout
    });
    
    if (!botResponse.ok) {
      throw new Error(`Bot returned ${botResponse.status}`);
    }

    const botData = await botResponse.json();
    
    const isReady = botData.checks?.whatsapp?.status === 'connected';
    
    return NextResponse.json({
      ok: true,
      whatsapp: {
        isReady,
        hasQR: !isReady && botData.status !== 'ok', // Rough heuristic
        details: botData.checks?.whatsapp
      },
      timestamp: new Date().toISOString(),
    });
  } catch (error: any) {
    console.error('Health check failed:', error.message);
    return NextResponse.json(
      { 
        ok: false, 
        error: 'Health check failed. Bot might be offline.',
        whatsapp: { isReady: false, hasQR: false }
      },
      { status: 503 }
    );
  }
}
