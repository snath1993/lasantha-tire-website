import { verifyRegistrationResponse } from '@simplewebauthn/server';
import { NextRequest, NextResponse } from 'next/server';
import { getWebAuthnConfig } from '@/core/lib/webauthn';
import { validateSession } from '@/core/lib/session';
import { getPool, sql } from '@/core/lib/db';

export async function POST(req: NextRequest) {
  const sessionId = req.headers.get('x-session-id');
  const session = validateSession(sessionId);
  
  if (!session) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const body = await req.json();
  const challenge = req.cookies.get('webauthn_challenge')?.value;

  if (!challenge) {
    return NextResponse.json({ error: 'Challenge not found' }, { status: 400 });
  }

  const { rpID, origin } = getWebAuthnConfig(req);

  let verification;
  try {
    verification = await verifyRegistrationResponse({
      response: body,
      expectedChallenge: challenge,
      expectedOrigin: origin,
      expectedRPID: rpID,
    });
  } catch (error: any) {
    console.error(error);
    return NextResponse.json({ error: error.message }, { status: 400 });
  }

  const { verified, registrationInfo } = verification;

  if (verified && registrationInfo) {
    const { credential } = registrationInfo;
    const { id, publicKey, counter, transports } = credential;

    if (!id || !publicKey) {
      console.error('Missing credential info in registrationInfo:', registrationInfo);
      return NextResponse.json({ error: 'Registration failed: Missing credential info' }, { status: 500 });
    }

    // Save to DB
    const pool = await getPool();
    await pool.request()
      .input('credentialID', sql.VarChar, Buffer.from(id, 'base64url').toString('base64'))
      .input('credentialPublicKey', sql.VarChar, Buffer.from(publicKey).toString('base64'))
      .input('counter', sql.BigInt, counter)
      .input('transports', sql.VarChar, transports?.join(',') || '')
      .input('username', sql.VarChar, session.username)
      .query(`
        INSERT INTO UserAuthenticators (credentialID, credentialPublicKey, counter, transports, username)
        VALUES (@credentialID, @credentialPublicKey, @counter, @transports, @username)
      `);

    const response = NextResponse.json({ verified: true });
    response.cookies.delete('webauthn_challenge');
    return response;
  }

  return NextResponse.json({ verified: false }, { status: 400 });
}
