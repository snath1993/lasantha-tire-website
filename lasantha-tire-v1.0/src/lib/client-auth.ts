/**
 * Client-side authentication utilities for session-based auth
 * Uses sessionStorage and x-session-id header
 */

export interface AuthUser {
  userId: string;
  username: string;
  role: string;
}

export interface AuthCheckResult {
  authenticated: boolean;
  user?: AuthUser;
}

/**
 * Check if user is authenticated using server-side session
 * @returns Promise with authentication status and user info
 */
export async function checkAuth(): Promise<AuthCheckResult> {
  try {
    // Get session ID from sessionStorage
    const sessionId = sessionStorage.getItem('sessionId');
    
    if (!sessionId) {
      console.log('[Auth] No session ID found');
      return { authenticated: false };
    }

    // Verify session with server
    const res = await fetch('/api/auth/verify', {
      cache: 'no-store',
      headers: {
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
        'x-session-id': sessionId,
      },
    });

    const data = await res.json();

    if (!res.ok || !data.authenticated) {
      console.log('[Auth] Invalid session, clearing');
      sessionStorage.removeItem('sessionId');
      return { authenticated: false };
    }

    console.log('[Auth] Authenticated as:', data.user?.username);
    return {
      authenticated: true,
      user: data.user,
    };
  } catch (error) {
    console.error('[Auth] Check failed:', error);
    sessionStorage.removeItem('sessionId');
    return { authenticated: false };
  }
}

/**
 * Get session ID from sessionStorage
 * @returns Session ID or null
 */
export function getSessionId(): string | null {
  return sessionStorage.getItem('sessionId');
}

/**
 * Clear session (logout)
 */
export async function clearSession(): Promise<void> {
  try {
    const sessionId = getSessionId();
    
    if (sessionId) {
      // Tell server to delete session
      await fetch('/api/auth/logout', {
        method: 'POST',
        headers: {
          'x-session-id': sessionId,
        },
      });
    }
  } catch (error) {
    console.error('[Auth] Logout error:', error);
  } finally {
    // Always clear local session
    sessionStorage.removeItem('sessionId');
  }
}

/**
 * Get headers with session ID for authenticated requests
 * @returns Headers object with x-session-id
 */
export function getAuthHeaders(): HeadersInit {
  const sessionId = getSessionId();
  return sessionId ? { 'x-session-id': sessionId } : {};
}
