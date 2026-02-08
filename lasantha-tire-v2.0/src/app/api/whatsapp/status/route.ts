import { NextResponse } from 'next/server';

export async function GET() {
  try {
    const BOT_API_URL = process.env.BOT_API_URL || 'http://127.0.0.1:8585';
    // Fetch status from bot
    const statusResponse = await fetch(`${BOT_API_URL}/api/whatsapp/status`, { next: { revalidate: 0 } });
    const statusData = await statusResponse.json();
    
    // Also try to get QR code if not connected
    let qrCode = null;
    if (!statusData.isConnected) {
      try {
        // The bot might expose QR via /status endpoint which returns the full status object including QR
        const fullStatusResponse = await fetch(`${BOT_API_URL}/status`, { next: { revalidate: 0 } });
        const fullStatus = await fullStatusResponse.json();
        if (fullStatus.qr) {
          qrCode = fullStatus.qr;
        }
      } catch (e) {
        // Ignore error fetching QR
      }
    }
    
    // IMPORTANT: The Bot QR page expects { isConnected, phoneNumber, status }
    // Keep backward-compatible metadata fields too.
    return NextResponse.json({
      isConnected: !!statusData?.isConnected,
      phoneNumber: statusData?.phoneNumber ?? null,
      status: statusData?.status ?? (statusData?.isConnected ? 'connected' : 'offline'),
      isReady: !!statusData?.isConnected,
      hasQR: !!qrCode,
      qrCode,
      timestamp: new Date().toISOString(),
      details: statusData,
    });
  } catch (error: any) {
    // Return a safe, consistent shape so the UI doesn't mis-render.
    return NextResponse.json({
      isConnected: false,
      phoneNumber: null,
      status: 'offline',
      isReady: false,
      hasQR: false,
      qrCode: null,
      timestamp: new Date().toISOString(),
      error: error?.message || 'Failed to get status',
    });
  }
}
