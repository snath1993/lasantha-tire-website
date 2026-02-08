import { NextRequest, NextResponse } from 'next/server';
import { getSessionIdFromRequest, validateSession } from '@/core/lib/session';
import { exec } from 'child_process';
import { promisify } from 'util';
import os from 'os';

const execAsync = promisify(exec);

interface ProcessInfo {
  name: string;
  status: string;
  uptime: string;
  cpu: string;
  memory: string;
  restarts: number;
}

interface SystemHealth {
  timestamp: string;
  system: {
    hostname: string;
    platform: string;
    uptime: number;
    loadAverage: number[];
    cpuUsage: number;
    totalMemory: number;
    freeMemory: number;
    usedMemory: number;
  };
  processes: ProcessInfo[];
  database: {
    connected: boolean;
    responseTime?: number;
  };
}

/**
 * System Health Monitoring API
 * Returns real-time system status
 */
export async function GET(request: NextRequest) {
  try {
    const sessionId = getSessionIdFromRequest(request);
    const session = validateSession(sessionId);
    
    if (!session) {
      return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
    }

    // Gather system metrics
    const totalMemory = os.totalmem();
    const freeMemory = os.freemem();
    const usedMemory = totalMemory - freeMemory;
    const cpuCount = os.cpus().length;
    const loadAvg = os.loadavg();

    // Get PM2 process list
    let processes: ProcessInfo[] = [];
    try {
      const { stdout } = await execAsync('pm2 jlist');
      const pm2List = JSON.parse(stdout);
      
      processes = pm2List
        .filter((p: any) => ['lasantha-tire-v2.0', 'whatsapp-bot', 'peachtree-bridge'].includes(p.name))
        .map((p: any) => ({
          name: p.name,
          status: p.pm2_env.status,
          uptime: formatUptime(p.pm2_env.pm_uptime),
          cpu: `${p.monit.cpu}%`,
          memory: formatBytes(p.monit.memory),
          restarts: p.pm2_env.restart_time
        }));
    } catch (error) {
      console.error('[Health] Failed to get PM2 list:', error);
    }

    // Check database connectivity (simple check)
    let dbConnected = false;
    try {
      const dbCheckTimeout = new Promise((_, reject) => 
        setTimeout(() => reject(new Error('timeout')), 2000)
      );
      
      // If SQL API exists, try to ping it
      const dbCheck = fetch('http://localhost:8585/api/health', { 
        signal: AbortSignal.timeout(2000) 
      }).then(r => r.ok).catch(() => false);
      
      dbConnected = await Promise.race([dbCheck, dbCheckTimeout]) as boolean;
    } catch {
      dbConnected = false;
    }

    const health: SystemHealth = {
      timestamp: new Date().toISOString(),
      system: {
        hostname: os.hostname(),
        platform: `${os.platform()} ${os.arch()}`,
        uptime: os.uptime(),
        loadAverage: loadAvg,
        cpuUsage: ((loadAvg[0] / cpuCount) * 100),
        totalMemory,
        freeMemory,
        usedMemory
      },
      processes,
      database: {
        connected: dbConnected
      }
    };

    return NextResponse.json(health);

  } catch (error: any) {
    console.error('[Health] API error:', error);
    return NextResponse.json(
      { error: 'Failed to get system health' },
      { status: 500 }
    );
  }
}

function formatUptime(startTime: number): string {
  const uptime = Date.now() - startTime;
  const seconds = Math.floor(uptime / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);

  if (days > 0) return `${days}d ${hours % 24}h`;
  if (hours > 0) return `${hours}h ${minutes % 60}m`;
  if (minutes > 0) return `${minutes}m`;
  return `${seconds}s`;
}

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return `${Math.round(bytes / Math.pow(k, i))} ${sizes[i]}`;
}
