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
    // Get session ID from sessionStorage or localStorage
    const sessionId = getSessionId();
    
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
      localStorage.removeItem('sessionId');
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
    localStorage.removeItem('sessionId');
    return { authenticated: false };
  }
}

/**
 * Get session ID from sessionStorage
 * @returns Session ID or null
 */
export function getSessionId(): string | null {
  if (typeof window === 'undefined') return null;
  return sessionStorage.getItem('sessionId') || localStorage.getItem('sessionId');
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
    localStorage.removeItem('sessionId');
  }
}

/**
 * Get headers with session ID for authenticated requests
 * @returns Headers object with x-session-id
 */
export function getAuthHeaders(): Record<string, string> {
  const sessionId = getSessionId();
  // Ensure session ID is a valid string without control characters
  if (sessionId && typeof sessionId === 'string') {
    // Remove any non-printable characters or unsafe header characters
    const cleanSessionId = sessionId.replace(/[^a-zA-Z0-9\-\._~]/g, '');
    if (cleanSessionId) {
      return { 'x-session-id': cleanSessionId };
    }
  }
  return {};
}

/**
 * Wrapper for fetch that automatically adds the session ID header
 */
export async function authenticatedFetch(url: string, options: RequestInit = {}): Promise<Response> {
  try {
    const authHeaders = getAuthHeaders();
    const requestHeaders = options.headers || {};
    
    // Merge headers safely
    const headers: Record<string, string> = {
      ...authHeaders,
    };

    // Handle different types of headers in options
    if (requestHeaders instanceof Headers) {
      requestHeaders.forEach((value, key) => {
        headers[key] = value;
      });
    } else if (Array.isArray(requestHeaders)) {
      requestHeaders.forEach(([key, value]) => {
        headers[key] = value;
      });
    } else {
      Object.assign(headers, requestHeaders);
    }

    return await fetch(url, {
      ...options,
      headers
    });
  } catch (error) {
    console.error('[Auth] Fetch error:', error);
    throw error;
  }
}
