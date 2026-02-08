import { NextRequest, NextResponse } from 'next/server';
import { getSessionIdFromRequest, validateSession } from '@/core/lib/session';
import { exec } from 'child_process';
import { promisify } from 'util';

const execAsync = promisify(exec);

/**
 * System Restart API
 * Requires admin session
 * Restarts the PM2 process
 */
export async function POST(request: NextRequest) {
  try {
    const sessionId = getSessionIdFromRequest(request);
    const session = validateSession(sessionId);
    
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }
    
    if (session.role !== 'admin') {
      return NextResponse.json({ error: 'Forbidden - Admin only' }, { status: 403 });
    }

    const body = await request.json().catch(() => ({}));
    const { component = 'all' } = body; // 'all', 'dashboard', 'bot'

    console.log(`[System] Restart requested by ${session.username} for: ${component}`);

    // Build the restart command based on component
    let command = '';
    
    if (component === 'dashboard' || component === 'all') {
      command += 'pm2 restart lasantha-tire-v2.0';
    }
    
    if (component === 'bot' || component === 'all') {
      if (command) command += ' && ';
      command += 'pm2 restart whatsapp-bot';
    }

    if (!command) {
      return NextResponse.json({ error: 'Invalid component' }, { status: 400 });
    }

    // Execute the restart asynchronously (don't wait for it to complete)
    setTimeout(async () => {
      try {
        await execAsync(command);
        console.log(`[System] Restart completed: ${component}`);
      } catch (error) {
        console.error('[System] Restart error:', error);
      }
    }, 500); // 500ms delay to allow response to be sent

    return NextResponse.json({
      success: true,
      message: `Restarting ${component}...`,
      timestamp: new Date().toISOString()
    });

  } catch (error: any) {
    console.error('[System] Restart API error:', error);
    return NextResponse.json(
      { error: 'Failed to restart system' },
      { status: 500 }
    );
  }
}
