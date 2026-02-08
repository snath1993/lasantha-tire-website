import { NextRequest, NextResponse } from 'next/server';
import { spawn } from 'child_process';
import path from 'path';

// Track bridge process and logs
let bridgeProcess: any = null;
let bridgeLogs: string[] = [];
const MAX_LOG_LINES = 100; // Keep last 100 log lines

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
      try {
        const healthCheck = await fetch('http://localhost:5000/health');
        if (healthCheck.ok) {
          return NextResponse.json({
            success: true,
            message: 'Python bridge already running',
            status: 'running',
            pid: bridgeProcess?.pid || 'unknown'
          });
        }
      } catch (e) {
        // Bridge not running, continue to start
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
        
        try {
          const healthCheck = await fetch('http://localhost:5000/health', {
            signal: AbortSignal.timeout(1000)
          });
          
          if (healthCheck.ok) {
            const healthData = await healthCheck.json();
            addLog('info', `✅ Health check passed: ${JSON.stringify(healthData)}`);
            
            return NextResponse.json({
              success: true,
              message: 'Python bridge started successfully',
              status: 'running',
              pid: bridgeProcess.pid,
              health: healthData,
              logs: bridgeLogs.slice(-20) // Last 20 log lines
            });
          }
        } catch (e) {
          // Still starting, continue waiting
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
      try {
        const healthCheck = await fetch('http://localhost:5000/health', {
          signal: AbortSignal.timeout(2000)
        });
        
        if (healthCheck.ok) {
          const healthData = await healthCheck.json();
          return NextResponse.json({
            success: true,
            status: 'running',
            pid: bridgeProcess?.pid || 'unknown',
            health: healthData,
            logs: bridgeLogs.slice(-10)
          });
        }
      } catch (error) {
        // Bridge not responding
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
  try {
    const healthCheck = await fetch('http://localhost:5000/health', {
      signal: AbortSignal.timeout(2000)
    });
    
    if (healthCheck.ok) {
      const healthData = await healthCheck.json();
      return NextResponse.json({
        success: true,
        status: 'running',
        pid: bridgeProcess?.pid || 'unknown',
        health: healthData,
        logs: bridgeLogs.slice(-10) // Include recent logs
      });
    }
  } catch (error) {
    // Bridge not responding
  }

  return NextResponse.json({
    success: true,
    status: 'stopped',
    pid: null,
    logs: bridgeLogs.slice(-10) // Include logs even when stopped
  });
}
