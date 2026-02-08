import { NextRequest, NextResponse } from 'next/server';
import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';

const USERS_FILE = path.join(process.cwd(), 'config', 'users.json');
const OTPS_FILE = path.join(process.cwd(), 'config', 'otps.json');
const BOT_API_URL = process.env.BOT_API_URL || 'http://localhost:8585';

function readUsers() {
  if (!fs.existsSync(USERS_FILE)) return { users: [] };
  return JSON.parse(fs.readFileSync(USERS_FILE, 'utf-8'));
}

function saveOTP(username: string, otp: string) {
  let otps: any = {};
  if (fs.existsSync(OTPS_FILE)) {
    try {
      otps = JSON.parse(fs.readFileSync(OTPS_FILE, 'utf-8'));
    } catch (e) {}
  }
  
  // Clean up expired OTPs
  const now = Date.now();
  Object.keys(otps).forEach(key => {
    if (otps[key].expiresAt < now) {
      delete otps[key];
    }
  });

  otps[username] = {
    otp,
    expiresAt: now + 5 * 60 * 1000 // 5 minutes
  };

  fs.writeFileSync(OTPS_FILE, JSON.stringify(otps, null, 2));
}

export async function POST(request: NextRequest) {
  try {
    const { username } = await request.json();

    if (!username) {
      return NextResponse.json({ error: 'Username is required' }, { status: 400 });
    }

    const data = readUsers();
    const user = data.users.find((u: any) => u.username === username && u.active);

    if (!user) {
      // Don't reveal user existence
      return NextResponse.json({ success: false, message: 'If user exists, OTP sent.' });
    }

    if (!user.mobile) {
      return NextResponse.json({ error: 'No mobile number linked to this account. Please contact admin.' }, { status: 400 });
    }

    // Generate 6 digit OTP
    const otp = Math.floor(100000 + Math.random() * 900000).toString();
    saveOTP(username, otp);

    // Send via WhatsApp Bot
    const message = `🔐 *Lasantha Tyre Dashboard*\n\nYour password reset code is: *${otp}*\n\nThis code expires in 5 minutes.`;
    
    try {
      const botRes = await fetch(`${BOT_API_URL}/api/whatsapp/send`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          number: user.mobile,
          message: message
        })
      });

      if (!botRes.ok) {
        console.error('Failed to send OTP via bot:', await botRes.text());
        return NextResponse.json({ error: 'Failed to send WhatsApp message. Check bot status.' }, { status: 500 });
      }
    } catch (err) {
      console.error('Bot connection error:', err);
      return NextResponse.json({ error: 'Could not connect to WhatsApp Bot.' }, { status: 500 });
    }

    return NextResponse.json({ success: true, message: 'OTP sent to your WhatsApp number.' });

  } catch (error: any) {
    console.error('Forgot password error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
