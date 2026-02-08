import { NextRequest, NextResponse } from 'next/server';
import {
  authenticateUser,
  checkRateLimit,
  getClientIP,
} from '@/lib/auth';
import { createSession, startSessionCleanup } from '@/lib/session';

// Start session cleanup on server start
startSessionCleanup();

export async function POST(req: NextRequest) {
  try {
    // Get client IP for rate limiting
    const clientIP = getClientIP(req);

    // Rate limiting: 5 attempts per 15 minutes
    if (!checkRateLimit(`login:${clientIP}`, 5, 15 * 60 * 1000)) {
      return NextResponse.json(
        { 
          success: false, 
          error: 'Too many login attempts. Please try again later.' 
        },
        { status: 429 }
      );
    }

    const { username, password } = await req.json();

    // Validate input
    if (!username || !password) {
      return NextResponse.json(
        { success: false, error: 'Username and password are required' },
        { status: 400 }
      );
    }

    // Authenticate user
    const user = await authenticateUser(username, password);

    if (!user) {
      return NextResponse.json(
        { success: false, error: 'Invalid username or password' },
        { status: 401 }
      );
    }

    // Get user agent for session tracking
    const userAgent = req.headers.get('user-agent') || 'unknown';

    // Create server-side session (NO cookies, NO JWT)
    const sessionId = createSession(
      user.userId,
      user.username,
      user.role,
      clientIP,
      userAgent
    );

    // Return session ID in response body (client stores in memory)
    const response = NextResponse.json({ 
      success: true,
      sessionId, // ✅ Client will send this in headers
      user: {
        username: user.username,
        role: user.role,
      },
    });

    console.log(`[Login] Success: ${username} from ${clientIP}`);

    return response;
  } catch (e: any) {
    console.error('Login error:', e);
    return NextResponse.json(
      { success: false, error: 'An error occurred during login' },
      { status: 500 }
    );
  }
}
