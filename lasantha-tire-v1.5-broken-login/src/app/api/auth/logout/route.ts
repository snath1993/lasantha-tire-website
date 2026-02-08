import { NextRequest, NextResponse } from 'next/server';
import { deleteSession, getSessionIdFromRequest } from '@/lib/session';

export async function POST(req: NextRequest) {
  try {
    // Get session ID from request
    const sessionId = getSessionIdFromRequest(req);

    if (!sessionId) {
      return NextResponse.json(
        { success: false, error: 'No active session' },
        { status: 401 }
      );
    }

    // Delete server-side session
    const deleted = deleteSession(sessionId);

    if (deleted) {
      console.log(`[Logout] Session deleted: ${sessionId}`);
      return NextResponse.json({ 
        success: true, 
        message: 'Logged out successfully' 
      });
    }

    return NextResponse.json(
      { success: false, error: 'Session not found' },
      { status: 404 }
    );
  } catch (e: any) {
    console.error('Logout error:', e);
    return NextResponse.json(
      { success: false, error: 'An error occurred during logout' },
      { status: 500 }
    );
  }
}
