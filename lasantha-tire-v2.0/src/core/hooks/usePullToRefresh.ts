import { useEffect, useRef, useState } from 'react';

interface UsePullToRefreshOptions {
  onRefresh: () => Promise<void>;
  threshold?: number; // pixels to pull before triggering
  enabled?: boolean;
}

export function usePullToRefresh({ onRefresh, threshold = 80, enabled = true }: UsePullToRefreshOptions) {
  const [isPulling, setIsPulling] = useState(false);
  const [pullDistance, setPullDistance] = useState(0);
  const [isRefreshing, setIsRefreshing] = useState(false);
  
  const startY = useRef<number>(0);
  const currentY = useRef<number>(0);

  useEffect(() => {
    if (!enabled) return;

    const handleTouchStart = (e: TouchEvent) => {
      // Only start if at top of page
      if (window.scrollY === 0) {
        startY.current = e.touches[0].clientY;
        setIsPulling(true);
      }
    };

    const handleTouchMove = (e: TouchEvent) => {
      if (!isPulling || isRefreshing) return;

      currentY.current = e.touches[0].clientY;
      const distance = currentY.current - startY.current;

      if (distance > 0 && window.scrollY === 0) {
        // Prevent default scrolling
        e.preventDefault();
        // Apply resistance - becomes harder to pull further
        const resistedDistance = Math.min(distance * 0.5, threshold * 1.5);
        setPullDistance(resistedDistance);
      }
    };

    const handleTouchEnd = async () => {
      if (!isPulling || isRefreshing) return;

      setIsPulling(false);

      if (pullDistance >= threshold) {
        setIsRefreshing(true);
        try {
          await onRefresh();
        } finally {
          setIsRefreshing(false);
          setPullDistance(0);
        }
      } else {
        setPullDistance(0);
      }
    };

    document.addEventListener('touchstart', handleTouchStart, { passive: true });
    document.addEventListener('touchmove', handleTouchMove, { passive: false });
    document.addEventListener('touchend', handleTouchEnd);

    return () => {
      document.removeEventListener('touchstart', handleTouchStart);
      document.removeEventListener('touchmove', handleTouchMove);
      document.removeEventListener('touchend', handleTouchEnd);
    };
  }, [enabled, isPulling, pullDistance, threshold, onRefresh, isRefreshing]);

  return {
    isPulling,
    pullDistance,
    isRefreshing,
    shouldTrigger: pullDistance >= threshold
  };
}
