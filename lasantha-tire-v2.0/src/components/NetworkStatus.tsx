'use client';

import { useState, useEffect } from 'react';
import { WifiOff, Wifi } from 'lucide-react';

/**
 * Global network status banner — shows only when offline.
 * Auto-hides briefly after reconnection.
 */
export default function NetworkStatus() {
  const [isOnline, setIsOnline] = useState(true);
  const [showRecovered, setShowRecovered] = useState(false);

  useEffect(() => {
    // Set initial
    setIsOnline(navigator.onLine);

    const goOnline = () => {
      setIsOnline(true);
      setShowRecovered(true);
      setTimeout(() => setShowRecovered(false), 3000);
    };
    const goOffline = () => {
      setIsOnline(false);
      setShowRecovered(false);
    };

    window.addEventListener('online', goOnline);
    window.addEventListener('offline', goOffline);
    return () => {
      window.removeEventListener('online', goOnline);
      window.removeEventListener('offline', goOffline);
    };
  }, []);

  if (isOnline && !showRecovered) return null;

  return (
    <div
      className={`fixed top-0 left-0 right-0 z-[9998] flex items-center justify-center gap-2 py-2 px-4 text-xs font-semibold transition-all duration-300 ${
        isOnline
          ? 'bg-emerald-600 text-white'
          : 'bg-red-600 text-white'
      }`}
      role="alert"
      aria-live="assertive"
    >
      {isOnline ? (
        <>
          <Wifi className="w-3.5 h-3.5" />
          Back online
        </>
      ) : (
        <>
          <WifiOff className="w-3.5 h-3.5" />
          No internet connection — changes won&apos;t be saved
        </>
      )}
    </div>
  );
}
