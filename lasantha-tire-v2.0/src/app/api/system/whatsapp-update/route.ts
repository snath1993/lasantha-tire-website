import { NextRequest, NextResponse } from 'next/server';
import { getSessionIdFromRequest, validateSession } from '@/core/lib/session';
import { exec } from 'child_process';
import { promisify } from 'util';
import path from 'path';
import fs from 'fs';

const execAsync = promisify(exec);

/**
 * WhatsApp Engine Update API
 * Requires admin session
 * Runs the PowerShell updater script asynchronously.
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

    // This Next.js app runs from the lasantha-tire-v2.0 folder.
    // The updater script lives in the repo root under scripts/.
    const repoRoot = path.resolve(process.cwd(), '..');
    const scriptPath = path.resolve(repoRoot, 'scripts', 'update_wa_engine.ps1');

    if (!fs.existsSync(scriptPath)) {
      return NextResponse.json(
        { error: 'Updater script not found' },
        { status: 500 }
      );
    }

    console.log(`[System] WhatsApp update requested by ${session.username}`);

    const command = `powershell -NoProfile -ExecutionPolicy Bypass -File "${scriptPath}"`;

    // Fire-and-forget so the UI gets an immediate response.
    setTimeout(async () => {
      try {
        await execAsync(command, {
          cwd: repoRoot,
          windowsHide: true,
          maxBuffer: 10 * 1024 * 1024,
        });
        console.log('[System] WhatsApp update completed');
      } catch (error) {
        console.error('[System] WhatsApp update error:', error);
      }
    }, 500);

    return NextResponse.json({
      success: true,
      message: 'WhatsApp engine update started. The bot will restart when complete.',
      timestamp: new Date().toISOString(),
    });
  } catch (error: any) {
    console.error('[System] WhatsApp update API error:', error);
    return NextResponse.json(
      { error: 'Failed to start WhatsApp update' },
      { status: 500 }
    );
  }
}
