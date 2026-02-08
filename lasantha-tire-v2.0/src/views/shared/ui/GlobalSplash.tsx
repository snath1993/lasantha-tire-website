'use client';

import { useState, useEffect } from 'react';
import { AnimatePresence } from 'framer-motion';
import SplashScreen from './SplashScreen';
import { usePathname } from 'next/navigation';

export default function GlobalSplash() {
  const [isVisible, setIsVisible] = useState(true);
  const pathname = usePathname();

  useEffect(() => {
    // Check if we've already shown the splash in this session (optional)
    // const hasShown = sessionStorage.getItem('splashShown');
    // if (hasShown) {
    //   setIsVisible(false);
    //   return;
    // }

    const timer = setTimeout(() => {
      setIsVisible(false);
      // sessionStorage.setItem('splashShown', 'true');
    }, 2500);

    return () => clearTimeout(timer);
  }, []);

  return (
    <AnimatePresence mode="wait">
      {isVisible && <SplashScreen />}
    </AnimatePresence>
  );
}
