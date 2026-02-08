// Server-side session management (No cookies, No JWT)
// Sessions stored in memory (production: use Redis/Database)

import { v4 as uuidv4 } from 'uuid';
import { NextRequest } from 'next/server';

interface Session {
  sessionId: string;
  userId: string;
  username: string;
  role: string;
  createdAt: number;
  lastAccess: number;
  ipAddress: string;
  userAgent: string;
}

// Use global to persist sessions across hot reloads in development
const globalForSessions = global as typeof globalThis & {
  sessions: Map<string, Session>;
};

// In-memory session store (production: use Redis)
const sessions = globalForSessions.sessions || new Map<string, Session>();

// Persist sessions across hot reloads
if (process.env.NODE_ENV !== 'production') {
  globalForSessions.sessions = sessions;
}

// Session configuration
const SESSION_TIMEOUT = 30 * 60 * 1000; // 30 minutes of inactivity
const SESSION_MAX_AGE = 8 * 60 * 60 * 1000; // 8 hours maximum

// Create new session
export function createSession(
  userId: string,
  username: string,
  role: string,
  ipAddress: string,
  userAgent: string
): string {
  const sessionId = uuidv4();
  const now = Date.now();

  const session: Session = {
    sessionId,
    userId,
    username,
    role,
    createdAt: now,
    lastAccess: now,
    ipAddress,
    userAgent,
  };

  sessions.set(sessionId, session);
  
  console.log(`[Session] Created: ${sessionId} for user: ${username}`);
  
  return sessionId;
}

// Get session by ID
export function getSession(sessionId: string): Session | null {
  const session = sessions.get(sessionId);
  
  if (!session) {
    return null;
  }

  const now = Date.now();
  
  // Check if session expired (inactivity timeout)
  if (now - session.lastAccess > SESSION_TIMEOUT) {
    console.log(`[Session] Expired (inactivity): ${sessionId}`);
    sessions.delete(sessionId);
    return null;
  }

  // Check if session expired (maximum age)
  if (now - session.createdAt > SESSION_MAX_AGE) {
    console.log(`[Session] Expired (max age): ${sessionId}`);
    sessions.delete(sessionId);
    return null;
  }

  // Update last access time
  session.lastAccess = now;
  sessions.set(sessionId, session);

  return session;
}

// Validate session
export function validateSession(sessionId: string | null): Session | null {
  if (!sessionId) {
    return null;
  }

  return getSession(sessionId);
}

// Delete session (logout)
export function deleteSession(sessionId: string): boolean {
  const existed = sessions.has(sessionId);
  sessions.delete(sessionId);
  
  if (existed) {
    console.log(`[Session] Deleted: ${sessionId}`);
  }
  
  return existed;
}

// Clean up expired sessions (call periodically)
export function cleanupExpiredSessions(): number {
  const now = Date.now();
  let cleaned = 0;

  for (const [sessionId, session] of sessions.entries()) {
    if (
      now - session.lastAccess > SESSION_TIMEOUT ||
      now - session.createdAt > SESSION_MAX_AGE
    ) {
      sessions.delete(sessionId);
      cleaned++;
    }
  }

  if (cleaned > 0) {
    console.log(`[Session] Cleaned up ${cleaned} expired sessions`);
  }

  return cleaned;
}

// Get session ID from request headers (custom header)
export function getSessionIdFromRequest(request: NextRequest): string | null {
  // Try custom header first (most secure)
  const headerSessionId = request.headers.get('x-session-id');
  if (headerSessionId) {
    return headerSessionId;
  }

  // Try URL parameter (for GET requests, less secure)
  const url = new URL(request.url);
  const paramSessionId = url.searchParams.get('sessionId');
  if (paramSessionId) {
    return paramSessionId;
  }

  return null;
}

// Get active session count
export function getActiveSessionCount(): number {
  return sessions.size;
}

// Get all sessions for a user
export function getUserSessions(userId: string): Session[] {
  const userSessions: Session[] = [];
  
  for (const session of sessions.values()) {
    if (session.userId === userId) {
      userSessions.push(session);
    }
  }
  
  return userSessions;
}

// Delete all sessions for a user
export function deleteUserSessions(userId: string): number {
  let deleted = 0;
  
  for (const [sessionId, session] of sessions.entries()) {
    if (session.userId === userId) {
      sessions.delete(sessionId);
      deleted++;
    }
  }
  
  if (deleted > 0) {
    console.log(`[Session] Deleted ${deleted} sessions for user: ${userId}`);
  }
  
  return deleted;
}

// Start cleanup interval (call once on server start)
let cleanupInterval: NodeJS.Timeout | null = null;

export function startSessionCleanup(): void {
  if (cleanupInterval) {
    return; // Already started
  }

  // Run cleanup every 5 minutes
  cleanupInterval = setInterval(() => {
    cleanupExpiredSessions();
  }, 5 * 60 * 1000);

  console.log('[Session] Cleanup interval started (every 5 minutes)');
}

// Stop cleanup interval
export function stopSessionCleanup(): void {
  if (cleanupInterval) {
    clearInterval(cleanupInterval);
    cleanupInterval = null;
    console.log('[Session] Cleanup interval stopped');
  }
}
