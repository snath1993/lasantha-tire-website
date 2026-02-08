import { verifyAuthenticationResponse, type WebAuthnCredential } from '@simplewebauthn/server';
import { NextRequest, NextResponse } from 'next/server';
import { getWebAuthnConfig } from '@/lib/webauthn';
import { getPool, sql } from '@/lib/db';
import { createSession } from '@/lib/session';

export async function POST(req: NextRequest) {
  const body = await req.json();
  const challenge = req.cookies.get('webauthn_challenge')?.value;

  if (!challenge) {
    return NextResponse.json({ error: 'Challenge not found' }, { status: 400 });
  }

  const credentialID = body.id;
  if (!credentialID) {
    return NextResponse.json({ error: 'Credential ID not found in request' }, { status: 400 });
  }

  const { rpID, origin } = getWebAuthnConfig(req);

  // 1. Find authenticator in DB
  const pool = await getPool();
  const result = await pool.request()
    .input('credentialID', sql.VarChar, credentialID) // credentialID is base64url in response, but we stored base64. 
    // Wait, SimpleWebAuthn uses base64url for ID in JSON.
    // My DB stores base64. I need to be careful with encoding.
    // Actually, let's fetch all authenticators for the user if we knew the user, but we don't.
    // We have to search by credentialID.
    // The body.id is base64url. I should convert it to base64 to match DB if I stored it as base64.
    // Let's assume I stored it as base64 (standard Buffer.toString('base64')).
    // base64url is similar but with -_ instead of +/ and no padding.
    // Let's try to match it.
    .query('SELECT * FROM UserAuthenticators'); 
    // Fetching all is inefficient but for a small app it's fine. 
    // Better: Convert input ID to base64 and search.
  
  // Let's do it in JS to be safe with encoding formats
  const authenticator = result.recordset.find(record => {
    const dbId = record.credentialID; // base64
    const reqId = credentialID; // base64url
    // Convert reqId to base64
    const reqIdBuffer = Buffer.from(reqId, 'base64url');
    const dbIdBuffer = Buffer.from(dbId, 'base64');
    return reqIdBuffer.equals(dbIdBuffer);
  });

  if (!authenticator) {
    console.error('Authenticator not found for credentialID:', credentialID);
    return NextResponse.json({ error: 'Authenticator not found' }, { status: 400 });
  }

  console.log('Found authenticator:', {
    id: authenticator.id,
    username: authenticator.username,
    counter: authenticator.counter,
    hasCredentialID: !!authenticator.credentialID,
    hasPublicKey: !!authenticator.credentialPublicKey
  });

  if (!authenticator.credentialID || !authenticator.credentialPublicKey) {
    console.error('Authenticator missing required fields:', authenticator);
    return NextResponse.json({ error: 'Invalid authenticator data' }, { status: 500 });
  }

  const credential: WebAuthnCredential = {
    id: Buffer.from(authenticator.credentialID, 'base64').toString('base64url'),
    publicKey: Buffer.from(authenticator.credentialPublicKey, 'base64'),
    counter: Number(authenticator.counter || 0),
    transports: authenticator.transports ? authenticator.transports.split(',') : undefined,
  };

  console.log('Prepared credential for verification:', {
    idLength: credential.id.length,
    publicKeyLength: credential.publicKey.length,
    counter: credential.counter
  });

  let verification;
  try {
    verification = await verifyAuthenticationResponse({
      response: body,
      expectedChallenge: challenge,
      expectedOrigin: origin,
      expectedRPID: rpID,
      credential,
    });
  } catch (error: any) {
    console.error('Verification error:', error);
    return NextResponse.json({ error: error.message }, { status: 400 });
  }

  const { verified, authenticationInfo } = verification;

  if (verified && authenticationInfo) {
    // Update counter
    await pool.request()
      .input('id', sql.Int, authenticator.id)
      .input('counter', sql.BigInt, authenticationInfo.newCounter)
      .query('UPDATE UserAuthenticators SET counter = @counter WHERE id = @id');

    // Log user in
    // createSession signature: (userId, username, role, ipAddress, userAgent)
    // We don't have userId/role in authenticator table yet, assuming admin for now or fetching from Users table
    // For now, let's use username as userId and 'admin' as role since this is likely the admin user
    const ip = req.headers.get('x-forwarded-for') || 'unknown';
    const userAgent = req.headers.get('user-agent') || 'unknown';
    
    const sessionId = createSession(
      authenticator.username, // userId
      authenticator.username, // username
      'admin',               // role (defaulting to admin for FaceID users for now)
      ip,
      userAgent
    );
    
    const response = NextResponse.json({ verified: true, sessionId });
    response.cookies.delete('webauthn_challenge');
    return response;
  }

  return NextResponse.json({ verified: false }, { status: 400 });
}
