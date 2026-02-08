import { NextRequest, NextResponse } from 'next/server';
import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';

const USERS_FILE = path.join(process.cwd(), 'config', 'users.json');
const OTPS_FILE = path.join(process.cwd(), 'config', 'otps.json');

function readUsers() {
  if (!fs.existsSync(USERS_FILE)) return { users: [] };
  return JSON.parse(fs.readFileSync(USERS_FILE, 'utf-8'));
}

function writeUsers(data: any) {
  data.lastModified = new Date().toISOString();
  fs.writeFileSync(USERS_FILE, JSON.stringify(data, null, 2));
}

function verifyOTP(username: string, otp: string) {
  if (!fs.existsSync(OTPS_FILE)) return false;
  
  try {
    const otps = JSON.parse(fs.readFileSync(OTPS_FILE, 'utf-8'));
    const record = otps[username];
    
    if (!record) return false;
    if (record.expiresAt < Date.now()) return false;
    if (record.otp !== otp) return false;
    
    // Valid OTP, delete it
    delete otps[username];
    fs.writeFileSync(OTPS_FILE, JSON.stringify(otps, null, 2));
    return true;
  } catch (e) {
    return false;
  }
}

export async function POST(request: NextRequest) {
  try {
    const { username, otp, newPassword } = await request.json();

    if (!username || !otp || !newPassword) {
      return NextResponse.json({ error: 'All fields are required' }, { status: 400 });
    }

    if (newPassword.length < 8) {
      return NextResponse.json({ error: 'Password must be at least 8 characters' }, { status: 400 });
    }

    if (!verifyOTP(username, otp)) {
      return NextResponse.json({ error: 'Invalid or expired OTP' }, { status: 400 });
    }

    const data = readUsers();
    const userIndex = data.users.findIndex((u: any) => u.username === username);

    if (userIndex === -1) {
      return NextResponse.json({ error: 'User not found' }, { status: 404 });
    }

    // Update password (SHA-256 hex hash)
    data.users[userIndex].password = crypto.createHash('sha256').update(newPassword).digest('hex');
    writeUsers(data);

    return NextResponse.json({ success: true, message: 'Password reset successfully' });

  } catch (error: any) {
    console.error('Reset password error:', error);
    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
