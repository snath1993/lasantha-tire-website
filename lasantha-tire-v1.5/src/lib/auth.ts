// Authentication utilities for secure session management

import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import { NextRequest, NextResponse } from 'next/server';

function getJwtSecret(): string {
  const secret = process.env.JWT_SECRET?.trim();
  if (secret) return secret;

  if (process.env.NODE_ENV === 'production') {
    throw new Error('JWT_SECRET is required in production');
  }

  return 'dev-jwt-secret-not-for-production';
}

const JWT_EXPIRES_IN = '24h';
const COOKIE_NAME = 'auth_token';

export interface JWTPayload {
  userId: string;
  username: string;
  role?: string;
  iat?: number;
  exp?: number;
}

// Hash password
export async function hashPassword(password: string): Promise<string> {
  const salt = await bcrypt.genSalt(12);
  return await bcrypt.hash(password, salt);
}

// Verify password
export async function verifyPassword(password: string, hashedPassword: string): Promise<boolean> {
  return await bcrypt.compare(password, hashedPassword);
}

// Generate JWT token
export function generateToken(payload: Omit<JWTPayload, 'iat' | 'exp'>): string {
  return jwt.sign(payload, getJwtSecret(), { expiresIn: JWT_EXPIRES_IN });
}

// Verify JWT token
export function verifyToken(token: string): JWTPayload | null {
  try {
    return jwt.verify(token, getJwtSecret()) as JWTPayload;
  } catch (error) {
    return null;
  }
}

// Set HTTP-only cookie with JWT
export function setAuthCookie(response: NextResponse, token: string): NextResponse {
  // Use 'lax' for development to support both localhost and network IP
  // In production with HTTPS, use 'none' with secure: true for cross-origin support
  const isProduction = process.env.NODE_ENV === 'production';
  
  response.cookies.set({
    name: COOKIE_NAME,
    value: token,
    httpOnly: true,
    // Development: false (HTTP), Production: true (HTTPS required)
    secure: isProduction,
    // Development: 'lax' works with localhost + network IPs
    // Production: 'none' for cross-origin (requires secure: true)
    sameSite: isProduction ? 'none' : 'lax',
    // CRITICAL FIX: Remove maxAge to create a SESSION cookie
    // Session cookies are automatically deleted when browser closes
    // maxAge: 60 * 60 * 24, // ❌ This made cookie persist 24 hours
    path: '/',
  });
  return response;
}

// Remove auth cookie
export function clearAuthCookie(response: NextResponse): NextResponse {
  const isProduction = process.env.NODE_ENV === 'production';
  
  response.cookies.set({
    name: COOKIE_NAME,
    value: '',
    httpOnly: true,
    secure: isProduction,
    sameSite: isProduction ? 'none' : 'lax',
    maxAge: 0,
    path: '/',
  });
  return response;
}

// Get token from request cookies
export function getTokenFromRequest(request: NextRequest): string | null {
  return request.cookies.get(COOKIE_NAME)?.value || null;
}

// Verify authentication from request
export function verifyAuth(request: NextRequest): JWTPayload | null {
  const token = getTokenFromRequest(request);
  if (!token) return null;
  return verifyToken(token);
}

// Rate limiting map (in production, use Redis)
const rateLimitMap = new Map<string, { count: number; resetTime: number }>();

// Simple rate limiter
export function checkRateLimit(identifier: string, maxAttempts = 5, windowMs = 15 * 60 * 1000): boolean {
  const now = Date.now();
  const record = rateLimitMap.get(identifier);

  if (!record || now > record.resetTime) {
    rateLimitMap.set(identifier, { count: 1, resetTime: now + windowMs });
    return true;
  }

  if (record.count >= maxAttempts) {
    return false;
  }

  record.count++;
  return true;
}

// Get client IP from request
export function getClientIP(request: NextRequest): string {
  const forwarded = request.headers.get('x-forwarded-for');
  const realIP = request.headers.get('x-real-ip');
  
  if (forwarded) {
    return forwarded.split(',')[0].trim();
  }
  
  if (realIP) {
    return realIP.trim();
  }
  
  return 'unknown';
}

// User credentials (in production, store in database)
export interface UserCredentials {
  userId: string;
  username: string;
  passwordHash: string;
  role: string;
}

const defaultAdminPassword = process.env.DEFAULT_ADMIN_PASSWORD?.trim();

export const defaultUsers: UserCredentials[] = defaultAdminPassword
  ? [
      {
        userId: '1',
        username: 'admin',
        passwordHash: bcrypt.hashSync(defaultAdminPassword, 12),
        role: 'admin',
      },
    ]
  : [];

// Authenticate user
export async function authenticateUser(username: string, password: string): Promise<UserCredentials | null> {
  if (!defaultUsers.length) {
    console.error('[Auth] No users configured. Set DEFAULT_ADMIN_PASSWORD in environment.');
    return null;
  }
  const user = defaultUsers.find((u) => u.username === username);
  if (!user) return null;

  const isValid = await verifyPassword(password, user.passwordHash);
  if (!isValid) return null;

  return user;
}
