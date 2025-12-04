import { useEffect, useRef, useState, useCallback } from 'react';

interface UseBridgeMonitorOptions {
  checkInterval?: number; // milliseconds
  maxRetries?: number;
  retryDelay?: number; // milliseconds
  onStatusChange?: (status: 'running' | 'stopped' | 'restarting') => void;
  autoRestart?: boolean;
}

interface BridgeStatus {
  status: 'running' | 'stopped' | 'restarting' | 'checking';
  pid: number | null;
  lastCheck: Date | null;
  consecutiveFailures: number;
}

export function useBridgeMonitor({
  checkInterval = 10000,
  maxRetries = 3,
  retryDelay = 5000,
  onStatusChange,
  autoRestart = true
}: UseBridgeMonitorOptions = {}) {
  const [bridgeStatus, setBridgeStatus] = useState<BridgeStatus>({
    status: 'checking',
    pid: null,
    lastCheck: null,
    consecutiveFailures: 0
  });

  const checkIntervalRef = useRef<NodeJS.Timeout | null>(null);
  const isRestartingRef = useRef(false);
  const prevStatusRef = useRef<BridgeStatus['status']>(bridgeStatus.status);

  // Handle status changes with a side effect instead of during render
  useEffect(() => {
    if (prevStatusRef.current !== bridgeStatus.status) {
      onStatusChange?.(bridgeStatus.status);
      prevStatusRef.current = bridgeStatus.status;
    }
  }, [bridgeStatus.status, onStatusChange]);

  const checkBridgeStatus = useCallback(async () => {
    try {
      const res = await fetch('/api/peachtree-bridge', {
        method: 'GET',
        cache: 'no-store'
      });

      if (!res.ok) throw new Error('Bridge API not responding');

      const data = await res.json();
      const isRunning = data.status === 'running';

      setBridgeStatus(prev => {
        const newStatus: BridgeStatus = {
          status: isRunning ? 'running' : 'stopped',
          pid: data.pid || null,
          lastCheck: new Date(),
          consecutiveFailures: isRunning ? 0 : prev.consecutiveFailures + 1
        };
        return newStatus;
      });

      return isRunning;
    } catch (error) {
      console.error('[Bridge Monitor] Check failed:', error);
      
      setBridgeStatus(prev => ({
        ...prev,
        status: 'stopped',
        lastCheck: new Date(),
        consecutiveFailures: prev.consecutiveFailures + 1
      }));

      return false;
    }
  }, []);

  const restartBridge = useCallback(async () => {
    if (isRestartingRef.current) {
      console.log('[Bridge Monitor] Restart already in progress');
      return false;
    }

    try {
      isRestartingRef.current = true;
      
      setBridgeStatus(prev => ({
        ...prev,
        status: 'restarting'
      }));

      console.log('[Bridge Monitor] Attempting to restart bridge...');

      const res = await fetch('/api/peachtree-bridge', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action: 'start' })
      });

      if (!res.ok) throw new Error('Restart request failed');

      const data = await res.json();

      if (data.success) {
        console.log('[Bridge Monitor] Bridge restarted successfully');
        
        // Wait a bit and verify it's actually running
        await new Promise(resolve => setTimeout(resolve, 2000));
        const isRunning = await checkBridgeStatus();

        if (isRunning) {
          setBridgeStatus(prev => ({
            ...prev,
            status: 'running',
            pid: data.pid || null,
            consecutiveFailures: 0
          }));
          return true;
        }
      }

      throw new Error('Bridge failed to start');
    } catch (error) {
      console.error('[Bridge Monitor] Restart failed:', error);
      setBridgeStatus(prev => ({
        ...prev,
        status: 'stopped'
      }));
      return false;
    } finally {
      isRestartingRef.current = false;
    }
  }, [checkBridgeStatus]);

  // Auto-restart logic
  useEffect(() => {
    if (
      autoRestart &&
      bridgeStatus.status === 'stopped' &&
      bridgeStatus.consecutiveFailures > 0 &&
      bridgeStatus.consecutiveFailures <= maxRetries &&
      !isRestartingRef.current
    ) {
      console.log(
        `[Bridge Monitor] Auto-restart attempt ${bridgeStatus.consecutiveFailures}/${maxRetries}`
      );
      
      const timeout = setTimeout(() => {
        restartBridge();
      }, retryDelay);

      return () => clearTimeout(timeout);
    }
  }, [
    bridgeStatus.status,
    bridgeStatus.consecutiveFailures,
    autoRestart,
    maxRetries,
    retryDelay,
    restartBridge
  ]);

  // Periodic health checks
  useEffect(() => {
    checkBridgeStatus(); // Initial check

    checkIntervalRef.current = setInterval(() => {
      checkBridgeStatus();
    }, checkInterval);

    return () => {
      if (checkIntervalRef.current) {
        clearInterval(checkIntervalRef.current);
      }
    };
  }, [checkInterval, checkBridgeStatus]);

  return {
    status: bridgeStatus.status,
    pid: bridgeStatus.pid,
    lastCheck: bridgeStatus.lastCheck,
    consecutiveFailures: bridgeStatus.consecutiveFailures,
    isHealthy: bridgeStatus.status === 'running',
    checkStatus: checkBridgeStatus,
    restart: restartBridge
  };
}
