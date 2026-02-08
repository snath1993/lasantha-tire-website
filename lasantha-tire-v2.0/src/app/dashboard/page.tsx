'use client';

import { useEffect, useState, Suspense } from 'react';
import { createPortal } from 'react-dom';
import { useRouter, useSearchParams } from 'next/navigation';
import { useModal } from '@/core/contexts/ModalContext';
import PeachtreeDashboard from '@/views/shared/erp/PeachtreeDashboard';
import MobileDashboard from '@/views/mobile/MobileDashboard';
import DesktopDashboard from '@/views/desktop/DesktopDashboard';

function DashboardContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const activeTab = searchParams.get('tab') || 'home';
  
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [isMounted, setIsMounted] = useState(false);
  const [showPeachtree, setShowPeachtree] = useState(false);
  
  // Mobile Quote State
  const [quoteItems, setQuoteItems] = useState<any[]>([]);
  const { isResultsModalOpen, setIsResultsModalOpen } = useModal();

  const addToQuote = (item: any) => {
    setQuoteItems(prev => {
      // 1. Toggle Logic: If item exists, remove it
      const exists = prev.find(i => i.ItemId === item.ItemId);
      if (exists) {
        return prev.filter(i => i.ItemId !== item.ItemId);
      }

      // 2. Single Alignment Logic
      const ALIGNMENT_IDS = ['120', '121', '161', '144'];
      if (ALIGNMENT_IDS.includes(item.ItemId)) {
        // Remove any existing alignment items
        const filtered = prev.filter(i => !ALIGNMENT_IDS.includes(i.ItemId));
        return [...filtered, { ...item, Quantity: 1 }];
      }

      // 3. Balancing Logic
      if (item.ItemId === '122') { // Wheel Balancing
        // Set price to 500, default qty 1
        return [...prev, { ...item, Quantity: 1, SellingPrice: 500 }];
      }

      return [...prev, { ...item, Quantity: 1 }];
    });
  };

  const removeFromQuote = (index: number) => {
    setQuoteItems(prev => prev.filter((_, i) => i !== index));
  };

  useEffect(() => {
    setIsMounted(true);
  }, []);

  useEffect(() => {
    if (!isMounted) return;
    
    const checkAuth = async () => {
      try {
        const sessionId = sessionStorage.getItem('sessionId');
        if (!sessionId) {
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }

        const res = await fetch('/api/auth/verify', { 
          cache: 'no-store',
          headers: { 'x-session-id': sessionId }
        });
        
        const data = await res.json();
        if (!res.ok || !data.authenticated) {
          sessionStorage.removeItem('sessionId');
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }
        setIsAuthenticated(true);
      } catch {
        sessionStorage.removeItem('sessionId');
        setIsAuthenticated(false);
        router.replace('/');
      }
    };
    checkAuth();
  }, [router, isMounted]);

  if (!isMounted || isAuthenticated === null) return null;
  if (!isAuthenticated) return null;

  return (
    <div className="space-y-8">
      <MobileDashboard 
        activeTab={activeTab}
        quoteItems={quoteItems}
        addToQuote={addToQuote}
        removeFromQuote={removeFromQuote}
        setQuoteItems={setQuoteItems}
        setShowPeachtree={setShowPeachtree}
      />

      <DesktopDashboard 
        setShowPeachtree={setShowPeachtree}
      />

      {/* Peachtree Modal */}
      {isMounted && showPeachtree && createPortal(
        <div className="fixed inset-0 z-[100] overflow-y-auto bg-slate-900/20 backdrop-blur-sm animate-in fade-in duration-200">
          <PeachtreeDashboard isModal onClose={() => setShowPeachtree(false)} />
        </div>,
        document.body
      )}
    </div>
  );
}

export default function DashboardHome() {
  return (
    <Suspense fallback={<div className="min-h-screen flex items-center justify-center"><div className="w-8 h-8 border-2 border-indigo-600 border-t-transparent rounded-full animate-spin" /></div>}>
      <DashboardContent />
    </Suspense>
  );
}
