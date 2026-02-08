import { NextRequest, NextResponse } from 'next/server';
import { validateSession, getSessionIdFromRequest } from '@/lib/session';
import * as fs from 'fs';
import * as path from 'path';
import * as crypto from 'crypto';

const USERS_FILE = path.join(process.cwd(), 'config', 'users.json');

interface User {
  id: string;
  username: string;
  password: string; // hashed
  role: 'admin' | 'user' | 'viewer';
  createdAt: string;
  lastLogin?: string;
  active: boolean;
}

interface UsersData {
  users: User[];
  lastModified: string;
}

function ensureUsersFile() {
  const configDir = path.dirname(USERS_FILE);
  if (!fs.existsSync(configDir)) {
    fs.mkdirSync(configDir, { recursive: true });
  }

  if (!fs.existsSync(USERS_FILE)) {
    // Create default admin user
    const envPassword = process.env.DEFAULT_ADMIN_PASSWORD?.trim();
    const isProduction = process.env.NODE_ENV === 'production';

    let defaultAdminSecret = envPassword;
    if (!defaultAdminSecret) {
      if (isProduction) {
        throw new Error('DEFAULT_ADMIN_PASSWORD is required to initialize users in production');
      }
      defaultAdminSecret = crypto.randomBytes(16).toString('hex');
      console.warn('[Users] DEFAULT_ADMIN_PASSWORD not set. Generated a dev-only admin password:', defaultAdminSecret);
    }
    const defaultData: UsersData = {
      users: [
        {
          id: crypto.randomUUID(),
          username: 'admin',
          password: hashPassword(defaultAdminSecret),
          role: 'admin',
          createdAt: new Date().toISOString(),
          active: true
        }
      ],
      lastModified: new Date().toISOString()
    };
    fs.writeFileSync(USERS_FILE, JSON.stringify(defaultData, null, 2));
  }
}

function hashPassword(password: string): string {
  return crypto.createHash('sha256').update(password).digest('hex');
}

function readUsers(): UsersData {
  ensureUsersFile();
  const data = fs.readFileSync(USERS_FILE, 'utf-8');
  return JSON.parse(data);
}

function writeUsers(data: UsersData) {
  data.lastModified = new Date().toISOString();
  fs.writeFileSync(USERS_FILE, JSON.stringify(data, null, 2));
}

// GET - List all users
export async function GET(request: NextRequest) {
  try {
    const sessionId = getSessionIdFromRequest(request);
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }
    if (session.role !== 'admin') {
      return NextResponse.json({ error: 'Forbidden' }, { status: 403 });
    }

    const data = readUsers();
    
    // Don't send passwords to client
    const safeUsers = data.users.map(u => ({
      id: u.id,
      username: u.username,
      role: u.role,
      createdAt: u.createdAt,
      lastLogin: u.lastLogin,
      active: u.active
    }));

    return NextResponse.json({ 
      users: safeUsers,
      lastModified: data.lastModified
    });
  } catch (error: any) {
    console.error('Error reading users:', error);
    return NextResponse.json({ error: 'Failed to read users' }, { status: 500 });
  }
}

// POST - Create or update user
export async function POST(request: NextRequest) {
  try {
    const sessionId = getSessionIdFromRequest(request);
    const session = validateSession(sessionId);
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }
    if (session.role !== 'admin') {
      return NextResponse.json({ error: 'Forbidden' }, { status: 403 });
    }

    const body = await request.json();
    const { action, user } = body;

    const data = readUsers();

    if (action === 'create') {
      // Check if username exists
      if (data.users.some(u => u.username === user.username)) {
        return NextResponse.json({ error: 'Username already exists' }, { status: 400 });
      }

      const newUser: User = {
        id: crypto.randomUUID(),
        username: user.username,
        password: hashPassword(user.password),
        role: user.role || 'user',
        createdAt: new Date().toISOString(),
        active: true
      };

      data.users.push(newUser);
      writeUsers(data);

      return NextResponse.json({ 
        success: true, 
        message: 'User created successfully',
        user: {
          id: newUser.id,
          username: newUser.username,
          role: newUser.role,
          createdAt: newUser.createdAt,
          active: newUser.active
        }
      });
    }

    if (action === 'update') {
      const index = data.users.findIndex(u => u.id === user.id);
      if (index === -1) {
        return NextResponse.json({ error: 'User not found' }, { status: 404 });
      }

      // Update user
      const existingUser = data.users[index];
      
      // If username changed, check for duplicates
      if (user.username && user.username !== existingUser.username) {
        if (data.users.some(u => u.username === user.username && u.id !== user.id)) {
          return NextResponse.json({ error: 'Username already exists' }, { status: 400 });
        }
        existingUser.username = user.username;
      }

      // If password provided, update it
      if (user.password) {
        existingUser['password'] = hashPassword(user.password);
      }

      // Update role if provided
      if (user.role) {
        existingUser.role = user.role;
      }

      // Update active status if provided
      if (typeof user.active !== 'undefined') {
        existingUser.active = user.active;
      }

      writeUsers(data);

      return NextResponse.json({ 
        success: true, 
        message: 'User updated successfully',
        user: {
          id: existingUser.id,
          username: existingUser.username,
          role: existingUser.role,
          createdAt: existingUser.createdAt,
          active: existingUser.active
        }
      });
    }

    if (action === 'delete') {
      const index = data.users.findIndex(u => u.id === user.id);
      if (index === -1) {
        return NextResponse.json({ error: 'User not found' }, { status: 404 });
      }

      // Prevent deleting the last admin
      const remainingAdmins = data.users.filter(u => u.role === 'admin' && u.id !== user.id);
      if (remainingAdmins.length === 0) {
        return NextResponse.json({ error: 'Cannot delete the last admin user' }, { status: 400 });
      }

      data.users.splice(index, 1);
      writeUsers(data);

      return NextResponse.json({ 
        success: true, 
        message: 'User deleted successfully'
      });
    }

    return NextResponse.json({ error: 'Invalid action' }, { status: 400 });
  } catch (error: any) {
    console.error('Error managing user:', error);
    return NextResponse.json({ error: 'Failed to manage user' }, { status: 500 });
  }
}
