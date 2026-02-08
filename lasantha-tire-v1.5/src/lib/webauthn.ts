import { NextRequest } from 'next/server';

export const rpName = 'Lasantha Tire Dashboard';

export function getWebAuthnConfig(req: NextRequest) {
  const host = req.headers.get('host') || 'localhost:3026';
  // If using Cloudflare tunnel, protocol is https. Localhost is http.
  // We can infer protocol from referer or origin header if available, or default to https for non-localhost.
  const referer = req.headers.get('referer');
  const originHeader = req.headers.get('origin');
  
  let protocol = 'https';
  if (host.includes('localhost') || host.includes('127.0.0.1')) {
    protocol = 'http';
  }
  
  // If origin header is present, use it as origin
  let origin = originHeader || `${protocol}://${host}`;
  
  // rpID should be the hostname without port
  const rpID = host.split(':')[0];

  return { rpID, origin, rpName };
}

export type UserAuthenticator = {
  id: number;
  credentialID: string;
  credentialPublicKey: string;
  counter: number;
  transports?: string;
  username: string;
};
