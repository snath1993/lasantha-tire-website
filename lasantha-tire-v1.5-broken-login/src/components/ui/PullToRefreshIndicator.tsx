'use client';

import { RefreshCw } from 'lucide-react';

interface PullToRefreshIndicatorProps {
  pullDistance: number;
  isRefreshing: boolean;
  threshold: number;
}

export function PullToRefreshIndicator({ pullDistance, isRefreshing, threshold }: PullToRefreshIndicatorProps) {
  if (pullDistance === 0 && !isRefreshing) return null;

  const progress = Math.min((pullDistance / threshold) * 100, 100);
  const rotation = (pullDistance / threshold) * 360;

  return (
    <div
      className="fixed top-0 left-0 right-0 z-50 flex items-center justify-center transition-all duration-200 pointer-events-none"
      style={{
        transform: `translateY(${isRefreshing ? '60px' : `${Math.min(pullDistance, threshold + 20)}px`})`,
        opacity: pullDistance > 10 ? 1 : 0
      }}
    >
      <div className="bg-slate-800/90 backdrop-blur-xl border border-slate-700 rounded-full p-3 shadow-lg">
        <RefreshCw
          className={`w-6 h-6 ${isRefreshing ? 'animate-spin' : ''} ${
            progress >= 100 ? 'text-green-400' : 'text-blue-400'
          }`}
          style={{
            transform: isRefreshing ? 'none' : `rotate(${rotation}deg)`,
            transition: isRefreshing ? 'none' : 'transform 0.1s ease-out'
          }}
        />
      </div>
      {!isRefreshing && (
        <div className="absolute -bottom-8 text-xs font-medium text-slate-400">
          {progress >= 100 ? 'Release to refresh' : 'Pull to refresh'}
        </div>
      )}
    </div>
  );
}
