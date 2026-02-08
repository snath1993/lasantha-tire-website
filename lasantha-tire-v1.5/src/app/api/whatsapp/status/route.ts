import { NextResponse } from 'next/server';

export async function GET() {
  try {
    // Fetch status from bot
    const statusResponse = await fetch('http://localhost:3100/api/whatsapp/status', { next: { revalidate: 0 } });
    const statusData = await statusResponse.json();
    
    // Also try to get QR code if not connected
    let qrCode = null;
    if (!statusData.isConnected) {
      try {
        // The bot might expose QR via /status endpoint which returns the full status object including QR
        const fullStatusResponse = await fetch('http://localhost:3100/status', { next: { revalidate: 0 } });
        const fullStatus = await fullStatusResponse.json();
        if (fullStatus.qr) {
          qrCode = fullStatus.qr;
        }
      } catch (e) {
        // Ignore error fetching QR
      }
    }
    
    return NextResponse.json({
      isReady: statusData.isConnected,
      hasQR: !!qrCode,
      qrCode,
      timestamp: new Date().toISOString(),
      details: statusData
    });
  } catch (error: any) {
    return NextResponse.json(
      { error: error.message || 'Failed to get status' },
      { status: 500 }
    );
  }
}
