'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

export default function WakeUpProvider({ children }: { children: React.ReactNode }) {
  const router = useRouter();

  useEffect(() => {
    const handleWakeUp = () => {
      if (document.visibilityState === 'visible') {
        console.log('App woke up! Refreshing data...');
        
        // 1. Refresh Server Components
        router.refresh();
        
        // 2. Dispatch event for Client Components
        window.dispatchEvent(new CustomEvent('app-wake-up'));
      }
    };

    // Listen for visibility change (tab switch, app background/foreground)
    document.addEventListener('visibilitychange', handleWakeUp);
    
    // Listen for window focus (browser window focus)
    window.addEventListener('focus', handleWakeUp);

    return () => {
      document.removeEventListener('visibilitychange', handleWakeUp);
      window.removeEventListener('focus', handleWakeUp);
    };
  }, [router]);

  return <>{children}</>;
}
