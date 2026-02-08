import { NextRequest, NextResponse } from 'next/server';
import { validateSession, getSessionIdFromRequest } from '@/core/lib/session';

export async function GET(req: NextRequest) {
  try {
    // Get session ID from request
    const sessionId = getSessionIdFromRequest(req);

    if (!sessionId) {
      return NextResponse.json(
        { authenticated: false },
        { status: 401 }
      );
    }

    // Validate server-side session
    const session = validateSession(sessionId);

    if (!session) {
      return NextResponse.json(
        { authenticated: false, error: 'Invalid or expired session' },
        { status: 401 }
      );
    }

    return NextResponse.json({
      authenticated: true,
      user: {
        userId: session.userId,
        username: session.username,
        role: session.role,
      },
    });
  } catch (e: any) {
    console.error('Verify error:', e);
    return NextResponse.json(
      { authenticated: false },
      { status: 401 }
    );
  }
}
