import { generateRegistrationOptions } from '@simplewebauthn/server';
import { NextRequest, NextResponse } from 'next/server';
import { getWebAuthnConfig } from '@/lib/webauthn';
import { validateSession } from '@/lib/session';
import { getPool, sql } from '@/lib/db';

export async function GET(req: NextRequest) {
  // 1. Ensure user is logged in via Session ID
  const sessionId = req.headers.get('x-session-id');
  const session = validateSession(sessionId);
  
  if (!session) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const { rpID, rpName } = getWebAuthnConfig(req);
  const username = session.username;

  // 2. Get user's existing authenticators to prevent re-registering
  const pool = await getPool();
  const result = await pool.request()
    .input('username', sql.VarChar, username)
    .query('SELECT credentialID, transports FROM UserAuthenticators WHERE username = @username');

  const userAuthenticators = result.recordset.map(record => ({
    credentialID: Buffer.from(record.credentialID, 'base64').toString('base64url'),
    transports: record.transports ? record.transports.split(',') : undefined,
  }));

  // 3. Generate registration options
  const options = await generateRegistrationOptions({
    rpName,
    rpID,
    userID: new TextEncoder().encode(username),
    userName: username,
    attestationType: 'none',
    excludeCredentials: userAuthenticators.map(authenticator => ({
      id: authenticator.credentialID,
      type: 'public-key',
      transports: authenticator.transports as any[],
    })),
    authenticatorSelection: {
      residentKey: 'preferred',
      userVerification: 'preferred',
      authenticatorAttachment: 'platform', // Forces FaceID/TouchID
    },
  });

  // 4. Save challenge to cookie (signed/httpOnly)
  const response = NextResponse.json(options);
  response.cookies.set('webauthn_challenge', options.challenge, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    path: '/',
    maxAge: 60 * 5, // 5 minutes
  });

  return response;
}
