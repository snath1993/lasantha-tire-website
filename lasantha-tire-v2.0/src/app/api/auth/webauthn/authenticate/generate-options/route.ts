import { generateAuthenticationOptions } from '@simplewebauthn/server';
import { NextRequest, NextResponse } from 'next/server';
import { getWebAuthnConfig } from '@/core/lib/webauthn';
import { getPool, sql } from '@/core/lib/db';

export async function GET(req: NextRequest) {
  // Check if username is provided (optional)
  const url = new URL(req.url);
  const username = url.searchParams.get('username');
  const { rpID } = getWebAuthnConfig(req);

  let allowCredentials;

  if (username) {
    const pool = await getPool();
    const result = await pool.request()
      .input('username', sql.VarChar, username)
      .query('SELECT credentialID, transports FROM UserAuthenticators WHERE username = @username');

    allowCredentials = result.recordset.map(record => ({
      id: record.credentialID,
      type: 'public-key' as const,
      transports: record.transports ? record.transports.split(',') : undefined,
    }));
  }

  const options = await generateAuthenticationOptions({
    rpID,
    allowCredentials,
    userVerification: 'preferred',
  });

  const response = NextResponse.json(options);
  response.cookies.set('webauthn_challenge', options.challenge, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    path: '/',
    maxAge: 60 * 5,
  });

  return response;
}
