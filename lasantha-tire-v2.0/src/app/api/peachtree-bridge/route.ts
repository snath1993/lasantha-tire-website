import { NextRequest, NextResponse } from 'next/server';
import { spawn } from 'child_process';
import path from 'path';

// Track bridge process and logs
let bridgeProcess: any = null;
let bridgeLogs: string[] = [];
const MAX_LOG_LINES = 100; // Keep last 100 log lines

function getPortCandidates() {
  const candidates: number[] = [];

  const fromEnv = [
    process.env.PEACHTREE_BRIDGE_PORT,
    process.env.PORT,
  ].filter(Boolean) as string[];

  for (const value of fromEnv) {
    const parsed = Number.parseInt(value, 10);
    if (Number.isFinite(parsed)) candidates.push(parsed);
  }

  // 🆕 Check 5001 FIRST (zombie process may be on 5000 with old code)
  candidates.push(5001, 5000);

  // De-dupe while preserving order
  return Array.from(new Set(candidates));
}

async function checkHealthOnAnyPort(timeoutMs: number) {
  const ports = getPortCandidates();
  for (const port of ports) {
    try {
      const res = await fetch(`http://localhost:${port}/health`, {
        signal: AbortSignal.timeout(timeoutMs)
      });
      if (!res.ok) continue;
      const healthData = await res.json();
      return { ok: true as const, port, healthData };
    } catch {
      // try next
    }
  }
  return { ok: false as const, port: null as number | null, healthData: null as any };
}

function addLog(type: 'stdout' | 'stderr' | 'info' | 'error', message: string) {
  const timestamp = new Date().toISOString();
  const logEntry = `[${timestamp}] [${type.toUpperCase()}] ${message}`;
  bridgeLogs.push(logEntry);
  
  // Keep only last MAX_LOG_LINES
  if (bridgeLogs.length > MAX_LOG_LINES) {
    bridgeLogs = bridgeLogs.slice(-MAX_LOG_LINES);
  }
  
  // Also log to console
  if (type === 'error' || type === 'stderr') {
    console.error(logEntry);
  } else {
    console.log(logEntry);
  }
}

export async function POST(request: NextRequest) {
  try {
    const { action } = await request.json();

    if (action === 'start') {
      // 🔍 Check if actually running by testing health endpoint
      const existing = await checkHealthOnAnyPort(1000);
      if (existing.ok) {
        return NextResponse.json({
          success: true,
          message: 'Python bridge already running',
          status: 'running',
          pid: bridgeProcess?.pid || 'unknown',
          port: existing.port,
          health: existing.healthData
        });
      }

      // 🚀 Start Python bridge - Use 32-bit Python for ODBC compatibility
      // Assuming process.cwd() is the Next.js app root (lasantha-tire-v1.0)
      // and the python/bridge files are in the parent directory (whatsapp-sql-api)
      const projectRoot = path.join(process.cwd(), '..');
      const python32Path = path.join(projectRoot, 'python32-portable', 'python.exe');
      const bridgePath = path.join(projectRoot, 'peachtree-odbc-bridge-32bit.py');
      
      addLog('info', `Using Python: ${python32Path}`);
      addLog('info', `Starting bridge: ${bridgePath}`);
      
      bridgeProcess = spawn(python32Path, [bridgePath], {
        detached: true,
        stdio: ['ignore', 'pipe', 'pipe'], // Capture output for debugging
        cwd: projectRoot
      });

      // Log bridge output with enhanced logging
      bridgeProcess.stdout?.on('data', (data: Buffer) => {
        const output = data.toString().trim();
        addLog('stdout', output);
      });
      
      bridgeProcess.stderr?.on('data', (data: Buffer) => {
        const output = data.toString().trim();
        addLog('stderr', output);
      });

      bridgeProcess.on('error', (error: Error) => {
        addLog('error', `Process error: ${error.message}`);
        bridgeProcess = null;
      });

      bridgeProcess.on('exit', (code: number) => {
        addLog('info', `Process exited with code: ${code}`);
        bridgeProcess = null;
      });

      bridgeProcess.unref();

      // ⏳ Wait for bridge to start - verify with health check
      addLog('info', 'Waiting for startup (max 10 seconds)...');
      let attempts = 0;
      const maxAttempts = 20; // 10 seconds total
      
      while (attempts < maxAttempts) {
        await new Promise(resolve => setTimeout(resolve, 500));
        attempts++;
        
        const started = await checkHealthOnAnyPort(1000);
        if (started.ok) {
          addLog('info', `✅ Health check passed on port ${started.port}: ${JSON.stringify(started.healthData)}`);
          return NextResponse.json({
            success: true,
            message: 'Python bridge started successfully',
            status: 'running',
            pid: bridgeProcess.pid,
            port: started.port,
            health: started.healthData,
            logs: bridgeLogs.slice(-20) // Last 20 log lines
          });
        }
      }

      // If we get here, bridge didn't start in time
      addLog('error', '❌ Failed to start within 10 seconds');
      if (bridgeProcess) {
        bridgeProcess.kill();
        bridgeProcess = null;
      }
      
      return NextResponse.json({
        success: false,
        message: 'Python bridge failed to start - check logs below',
        status: 'stopped',
        logs: bridgeLogs.slice(-20), // Include logs in error response
        troubleshooting: [
          'Check if port 5000 is already in use',
          'Verify python32-portable\\python.exe exists',
          'Ensure Peachtree is closed (ODBC file locks)',
          'Check ODBC DSN "Peachtree" is configured'
        ]
      }, { status: 500 });
    }

    if (action === 'stop') {
      if (bridgeProcess && !bridgeProcess.killed) {
        bridgeProcess.kill();
        bridgeProcess = null;
        addLog('info', 'Bridge stopped by user');
        return NextResponse.json({
          success: true,
          message: 'Python bridge stopped',
          status: 'stopped',
          logs: bridgeLogs.slice(-10)
        });
      }

      return NextResponse.json({
        success: false,
        message: 'Bridge not running',
        status: 'stopped'
      });
    }

    if (action === 'status') {
      // 🔍 Real health check
      const status = await checkHealthOnAnyPort(2000);
      if (status.ok) {
        return NextResponse.json({
          success: true,
          status: 'running',
          pid: bridgeProcess?.pid || 'unknown',
          port: status.port,
          health: status.healthData,
          logs: bridgeLogs.slice(-10)
        });
      }

      return NextResponse.json({
        success: true,
        status: 'stopped',
        pid: null,
        logs: bridgeLogs.slice(-10)
      });
    }

    if (action === 'logs') {
      // New action to get all logs
      return NextResponse.json({
        success: true,
        logs: bridgeLogs,
        count: bridgeLogs.length
      });
    }

    return NextResponse.json({
      success: false,
      message: 'Invalid action'
    }, { status: 400 });

  } catch (error: any) {
    return NextResponse.json({
      success: false,
      message: error.message,
      status: 'error'
    }, { status: 500 });
  }
}

export async function GET(request: NextRequest) {
  // 🔍 Real health check - test actual connection
  const status = await checkHealthOnAnyPort(2000);
  if (status.ok) {
    return NextResponse.json({
      success: true,
      status: 'running',
      pid: bridgeProcess?.pid || 'unknown',
      port: status.port,
      health: status.healthData,
      logs: bridgeLogs.slice(-10) // Include recent logs
    });
  }

  return NextResponse.json({
    success: true,
    status: 'stopped',
    pid: null,
    logs: bridgeLogs.slice(-10) // Include logs even when stopped
  });
}
