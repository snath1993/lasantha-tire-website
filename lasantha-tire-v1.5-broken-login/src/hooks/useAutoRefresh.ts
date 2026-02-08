import { useEffect, useRef, useCallback } from 'react';

interface UseAutoRefreshOptions {
  enabled: boolean;
  interval?: number; // milliseconds
  onRefresh: () => void | Promise<void>;
}

export function useAutoRefresh({ enabled, interval = 30000, onRefresh }: UseAutoRefreshOptions) {
  const timerRef = useRef<NodeJS.Timeout | null>(null);
  const isRefreshingRef = useRef(false);

  const refresh = useCallback(async () => {
    if (isRefreshingRef.current) return;
    
    try {
      isRefreshingRef.current = true;
      await onRefresh();
    } finally {
      isRefreshingRef.current = false;
    }
  }, [onRefresh]);

  useEffect(() => {
    if (!enabled) {
      if (timerRef.current) {
        clearInterval(timerRef.current);
        timerRef.current = null;
      }
      return;
    }

    timerRef.current = setInterval(refresh, interval);

    return () => {
      if (timerRef.current) {
        clearInterval(timerRef.current);
        timerRef.current = null;
      }
    };
  }, [enabled, interval, refresh]);

  return { refresh };
}
