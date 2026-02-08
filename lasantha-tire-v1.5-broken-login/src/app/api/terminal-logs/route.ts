import { NextResponse } from 'next/server';
import { exec } from 'child_process';
import { promisify } from 'util';
import * as fs from 'fs/promises';
import * as path from 'path';

const execAsync = promisify(exec);

interface LogEntry {
  timestamp: string;
  level: 'info' | 'error' | 'warn';
  message: string;
  source: string;
}

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url);
  const logType = searchParams.get('type') || 'all'; // 'out', 'error', 'all', 'pm2'
  const lines = parseInt(searchParams.get('lines') || '100');

  try {
    const logs: LogEntry[] = [];

    // PM2 logs from log files
    const logsDir = path.join(process.cwd(), '..', 'logs');
    
    if (logType === 'all' || logType === 'out') {
      try {
        const outLogPath = path.join(logsDir, 'pm2-out.log');
        const outContent = await fs.readFile(outLogPath, 'utf-8');
        const outLines = outContent.split('\n').filter(line => line.trim());
        
        outLines.slice(-lines).forEach(line => {
          const match = line.match(/^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}): (.+)$/);
          if (match) {
            logs.push({
              timestamp: match[1],
              level: 'info',
              message: match[2],
              source: 'output'
            });
          } else {
            logs.push({
              timestamp: new Date().toISOString(),
              level: 'info',
              message: line,
              source: 'output'
            });
          }
        });
      } catch (err) {
        console.error('Error reading pm2-out.log:', err);
      }
    }

    if (logType === 'all' || logType === 'error') {
      try {
        const errLogPath = path.join(logsDir, 'pm2-error.log');
        const errContent = await fs.readFile(errLogPath, 'utf-8');
        const errLines = errContent.split('\n').filter(line => line.trim());
        
        errLines.slice(-lines).forEach(line => {
          const match = line.match(/^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}): (.+)$/);
          if (match) {
            logs.push({
              timestamp: match[1],
              level: 'error',
              message: match[2],
              source: 'error'
            });
          } else {
            logs.push({
              timestamp: new Date().toISOString(),
              level: 'error',
              message: line,
              source: 'error'
            });
          }
        });
      } catch (err) {
        console.error('Error reading pm2-error.log:', err);
      }
    }

    // Get PM2 status
    if (logType === 'all' || logType === 'pm2') {
      try {
        const { stdout } = await execAsync('pm2 jlist', { 
          cwd: path.join(process.cwd(), '..'),
          windowsHide: true 
        });
        
        const pm2Status = JSON.parse(stdout);
        if (pm2Status && pm2Status.length > 0) {
          const botProcess = pm2Status.find((p: any) => p.name === 'whatsapp-bot');
          if (botProcess) {
            logs.push({
              timestamp: new Date().toISOString(),
              level: 'info',
              message: `PM2 Status: ${botProcess.pm2_env.status} | Memory: ${Math.round(botProcess.monit.memory / 1024 / 1024)}MB | Uptime: ${Math.round(botProcess.pm2_env.pm_uptime ? (Date.now() - botProcess.pm2_env.pm_uptime) / 1000 : 0)}s | Restarts: ${botProcess.pm2_env.restart_time}`,
              source: 'pm2'
            });
          }
        }
      } catch (err) {
        console.error('Error getting PM2 status:', err);
      }
    }

    // Sort by timestamp
    logs.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());

    return NextResponse.json({
      success: true,
      logs: logs.slice(-lines),
      total: logs.length,
      type: logType
    });

  } catch (error: any) {
    console.error('Error fetching terminal logs:', error);
    return NextResponse.json(
      { 
        success: false, 
        error: 'Failed to fetch logs',
        details: error.message 
      },
      { status: 500 }
    );
  }
}
