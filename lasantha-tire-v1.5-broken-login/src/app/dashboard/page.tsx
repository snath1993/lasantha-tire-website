'use client';

import { useEffect, useState, Suspense } from 'react';
import { createPortal } from 'react-dom';
import { useRouter, useSearchParams } from 'next/navigation';
import { useModal } from '@/contexts/ModalContext';
import PeachtreeDashboard from '@/components/erp/PeachtreeDashboard';
import TireSearch from '@/components/mobile/TireSearch';
import StockView from '@/components/mobile/StockView';
import QuickQuote from '@/components/mobile/QuickQuote';
import QuotationView from '@/components/mobile/QuotationView';
import { 
  Database, 
  MessageSquare, 
  BarChart3, 
  Terminal, 
  Settings, 
  ArrowRight,
  Smartphone,
  Facebook,
  Briefcase,
  Package,
  Users
} from 'lucide-react';

type MobileMenuProps = {
  onOpenInventory: () => void;
  onOpenPeachtree: () => void;
  onOpenAnalytics: () => void;
  onOpenJobs: () => void;
  onOpenWhatsAppLive: () => void;
  onOpenSystemLogs: () => void;
};

function MobileMenu({
  onOpenInventory,
  onOpenPeachtree,
  onOpenAnalytics,
  onOpenJobs,
  onOpenWhatsAppLive,
  onOpenSystemLogs,
}: MobileMenuProps) {
  return (
    <div className="space-y-4 pb-24">
      <h2 className="text-xl font-bold text-zinc-900 mb-4">Menu</h2>

      <div className="grid grid-cols-2 gap-4">
        <button 
          onClick={onOpenInventory}
          className="p-4 bg-white rounded-2xl shadow-sm border border-zinc-100 flex flex-col items-center gap-2"
        >
          <div className="w-10 h-10 rounded-xl bg-indigo-100 text-indigo-600 flex items-center justify-center">
            <Database size={20} />
          </div>
          <span className="text-sm font-medium text-zinc-700">Inventory</span>
        </button>

        <button 
          onClick={onOpenPeachtree}
          className="p-4 bg-white rounded-2xl shadow-sm border border-zinc-100 flex flex-col items-center gap-2"
        >
          <div className="w-10 h-10 rounded-xl bg-emerald-100 text-emerald-600 flex items-center justify-center">
            <Briefcase size={20} />
          </div>
          <span className="text-sm font-medium text-zinc-700">Peachtree</span>
        </button>

        <button 
          onClick={onOpenAnalytics}
          className="p-4 bg-white rounded-2xl shadow-sm border border-zinc-100 flex flex-col items-center gap-2"
        >
          <div className="w-10 h-10 rounded-xl bg-violet-100 text-violet-600 flex items-center justify-center">
            <BarChart3 size={20} />
          </div>
          <span className="text-sm font-medium text-zinc-700">Analytics</span>
        </button>

        <button 
          onClick={onOpenJobs}
          className="p-4 bg-white rounded-2xl shadow-sm border border-zinc-100 flex flex-col items-center gap-2"
        >
          <div className="w-10 h-10 rounded-xl bg-orange-100 text-orange-600 flex items-center justify-center">
            <Settings size={20} />
          </div>
          <span className="text-sm font-medium text-zinc-700">Jobs</span>
        </button>
      </div>

      <div className="space-y-3 mt-6">
        <button 
          onClick={onOpenWhatsAppLive}
          className="w-full p-4 bg-[#25d366] text-white rounded-2xl shadow-lg shadow-emerald-500/20 flex items-center gap-3"
        >
          <MessageSquare size={20} />
          <span className="font-bold">WhatsApp Live</span>
          <ArrowRight size={16} className="ml-auto" />
        </button>

        <button 
          onClick={onOpenSystemLogs}
          className="w-full p-4 bg-slate-100 text-slate-900 border border-slate-200 rounded-2xl shadow-sm flex items-center gap-3"
        >
          <Terminal size={20} className="text-slate-600" />
          <span className="font-bold">System Logs</span>
          <ArrowRight size={16} className="ml-auto text-slate-400" />
        </button>
      </div>
    </div>
  );
}

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
        const sessionId = sessionStorage.getItem('sessionId') || localStorage.getItem('sessionId');
        if (!sessionId) {
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }

        // Add timeout to fetch
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 10000); // 10s timeout

        const res = await fetch('/api/auth/verify', { 
          cache: 'no-store',
          headers: { 'x-session-id': sessionId },
          signal: controller.signal
        });
        clearTimeout(timeoutId);
        
        const data = await res.json();
        if (!res.ok || !data.authenticated) {
          sessionStorage.removeItem('sessionId');
          localStorage.removeItem('sessionId');
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }
        setIsAuthenticated(true);
      } catch (error) {
        console.error("Auth check failed:", error);
        sessionStorage.removeItem('sessionId');
        localStorage.removeItem('sessionId');
        setIsAuthenticated(false);
        router.replace('/');
      }
    };
    checkAuth();
  }, [router, isMounted]);

  if (!isMounted) return null;
  
  if (isAuthenticated === null) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-900">
        <div className="flex flex-col items-center gap-4">
          <div className="w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
          <p className="text-white font-medium">Authenticating...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) return null;

  return (
    <div className="space-y-8">
      {/* Desktop Header - Hidden on Mobile */}
      <div className="hidden md:flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Dashboard Overview</h1>
          <p className="text-slate-500 mt-1">Welcome back, Admin. Here's what's happening today.</p>
        </div>
        <div className="flex items-center gap-3">
          <button 
            onClick={() => router.push('/analytics')}
            className="flex items-center gap-2 px-4 py-2 bg-violet-600 hover:bg-violet-700 text-white rounded-full shadow-lg shadow-violet-600/20 transition-all active:scale-95"
          >
            <BarChart3 size={18} />
            <span className="text-sm font-bold">Sales Report</span>
          </button>
          <span className="px-3 py-1 rounded-full bg-emerald-100 text-emerald-700 border border-emerald-200 text-xs font-bold uppercase tracking-wide">
            System Online
          </span>
          <span className="text-sm text-slate-500 font-medium">
            {new Date().toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}
          </span>
        </div>
      </div>

      {/* Mobile View - Tabbed Content */}
      <div className="md:hidden pb-24">
        {activeTab === 'home' && (
          <div className="space-y-4">
            <TireSearch onAddToQuote={addToQuote} quoteItems={quoteItems} />
          </div>
        )}

        {activeTab === 'inventory' && (
          <StockView />
        )}

        {activeTab === 'quotation' && (
          <QuotationView />
        )}

        {activeTab === 'peachtree' && (
          <div className="pb-24">
            <PeachtreeDashboard isModal={false} />
          </div>
        )}

        {activeTab === 'menu' && (
          <MobileMenu
            onOpenInventory={() => router.push('/dashboard/erp')}
            onOpenPeachtree={() => setShowPeachtree(true)}
            onOpenAnalytics={() => router.push('/dashboard/analytics')}
            onOpenJobs={() => router.push('/dashboard/jobs')}
            onOpenWhatsAppLive={() => router.push('/dashboard/whatsapp-live')}
            onOpenSystemLogs={() => router.push('/dashboard/terminal-logs')}
          />
        )}
      </div>

      {/* Desktop View - Grid Layout (Hidden on Mobile) */}
      <div className="hidden md:grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* ERP Section (Large Card) */}
        <div className="md:col-span-2 bg-white rounded-[2rem] p-8 shadow-sm border border-slate-200 relative overflow-hidden group">
          <div className="absolute top-0 right-0 w-64 h-64 bg-blue-50 rounded-full blur-3xl -mr-16 -mt-16 transition-all group-hover:bg-blue-100" />
          
          <div className="relative z-10">
            <div className="flex items-center gap-4 mb-6">
              <div className="w-12 h-12 bg-blue-600 rounded-2xl flex items-center justify-center text-white shadow-lg shadow-blue-600/20">
                <Database size={24} />
              </div>
              <div>
                <h2 className="text-xl font-bold text-slate-900">ERP Systems</h2>
                <p className="text-sm text-slate-500">Manage inventory & financial data</p>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <button 
                onClick={() => router.push('/dashboard/erp')}
                className="flex items-center gap-4 p-4 rounded-2xl bg-slate-50 hover:bg-slate-100 border border-slate-200 transition-all group/btn text-left"
              >
                <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center text-blue-600 shadow-sm border border-slate-200">
                  <Database size={20} />
                </div>
                <div>
                  <span className="block font-semibold text-slate-900">SQL ERP</span>
                  <span className="text-xs text-slate-500">Inventory & Sales</span>
                </div>
                <ArrowRight size={16} className="ml-auto text-slate-400 group-hover/btn:text-blue-600 transition-colors" />
              </button>

              <button 
                onClick={() => setShowPeachtree(true)}
                className="flex items-center gap-4 p-4 rounded-2xl bg-slate-50 hover:bg-slate-100 border border-slate-200 transition-all group/btn text-left"
              >
                <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center text-emerald-600 shadow-sm border border-slate-200">
                  <Briefcase size={20} />
                </div>
                <div>
                  <span className="block font-semibold text-slate-900">Peachtree</span>
                  <span className="text-xs text-slate-500">Accounting</span>
                </div>
                <ArrowRight size={16} className="ml-auto text-slate-400 group-hover/btn:text-emerald-600 transition-colors" />
              </button>
            </div>
          </div>
        </div>

        {/* Analytics Card */}
        <div 
          onClick={() => router.push('/dashboard/analytics')}
          className="bg-white rounded-[2rem] p-8 shadow-sm border border-slate-200 cursor-pointer hover:-translate-y-1 transition-all duration-300 group"
        >
          <div className="w-12 h-12 bg-violet-600 rounded-2xl flex items-center justify-center text-white shadow-lg shadow-violet-600/20 mb-6 group-hover:scale-110 transition-transform">
            <BarChart3 size={24} />
          </div>
          <h2 className="text-xl font-bold text-slate-900 mb-2">Analytics</h2>
          <p className="text-sm text-slate-500 mb-6">Real-time sales data and performance metrics.</p>
          <div className="flex items-center text-sm font-semibold text-violet-600">
            View Reports <ArrowRight size={16} className="ml-2 group-hover:translate-x-1 transition-transform" />
          </div>
        </div>

        {/* Job Management */}
        <div className="md:col-span-3 grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-white rounded-[2rem] p-8 shadow-sm border border-slate-200 relative overflow-hidden">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-lg font-bold text-slate-900">Automation Jobs</h2>
              <div className="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center">
                <Settings size={16} className="text-slate-500" />
              </div>
            </div>
            
            <div className="space-y-3">
              <button 
                onClick={() => router.push('/dashboard/jobs?category=whatsapp')}
                className="w-full flex items-center gap-3 p-3 rounded-xl hover:bg-slate-50 transition-colors text-left group"
              >
                <div className="w-8 h-8 rounded-lg bg-emerald-100 flex items-center justify-center text-emerald-600">
                  <Smartphone size={16} />
                </div>
                <span className="font-medium text-slate-700 group-hover:text-slate-900">WhatsApp Jobs</span>
                <span className="ml-auto text-xs font-bold bg-slate-100 px-2 py-1 rounded-md text-slate-600">16</span>
              </button>
              
              <button 
                onClick={() => router.push('/dashboard/jobs?category=facebook')}
                className="w-full flex items-center gap-3 p-3 rounded-xl hover:bg-slate-50 transition-colors text-left group"
              >
                <div className="w-8 h-8 rounded-lg bg-blue-100 flex items-center justify-center text-blue-600">
                  <Facebook size={16} />
                </div>
                <span className="font-medium text-slate-700 group-hover:text-slate-900">Facebook Jobs</span>
                <span className="ml-auto text-xs font-bold bg-slate-100 px-2 py-1 rounded-md text-slate-600">5</span>
              </button>
            </div>
          </div>

          {/* WhatsApp Live */}
          <div 
            onClick={() => router.push('/dashboard/whatsapp-live')}
            className="bg-[#25d366] rounded-[2rem] p-8 shadow-lg shadow-emerald-500/20 text-white cursor-pointer hover:-translate-y-1 transition-all relative overflow-hidden group"
          >
            <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full blur-2xl -mr-8 -mt-8" />
            <MessageSquare size={32} className="mb-6" />
            <h2 className="text-xl font-bold mb-2">WhatsApp Live</h2>
            <p className="text-emerald-50 text-sm mb-6 opacity-90">Direct access to WhatsApp Web interface.</p>
            <div className="flex items-center text-sm font-semibold bg-white/20 w-fit px-4 py-2 rounded-xl backdrop-blur-sm group-hover:bg-white/30 transition-colors">
              Open Chat <ArrowRight size={16} className="ml-2" />
            </div>
          </div>

          {/* Terminal Logs */}
          <div 
            onClick={() => router.push('/dashboard/terminal-logs')}
            className="bg-white rounded-[2rem] p-8 shadow-sm border border-slate-200 text-slate-900 cursor-pointer hover:-translate-y-1 transition-all relative overflow-hidden group"
          >
            <div className="absolute top-0 right-0 w-32 h-32 bg-slate-50 rounded-full blur-2xl -mr-8 -mt-8" />
            <Terminal size={32} className="mb-6 text-slate-400 group-hover:text-slate-600 transition-colors" />
            <h2 className="text-xl font-bold mb-2">System Logs</h2>
            <p className="text-slate-500 text-sm mb-6">Monitor server processes and errors.</p>
            <div className="flex items-center text-sm font-semibold text-slate-600 group-hover:text-slate-900 transition-colors">
              View Logs <ArrowRight size={16} className="ml-2" />
            </div>
          </div>
        </div>
      </div>

      {/* Peachtree Modal */}
      {isMounted && showPeachtree && createPortal(
        <div className="fixed inset-0 z-[100] overflow-y-auto bg-slate-900/20 backdrop-blur-sm animate-in fade-in duration-200">
          <PeachtreeDashboard isModal onClose={() => setShowPeachtree(false)} />
        </div>,
        document.body
      )}

      {/* Mobile Components - Hidden when results modal is open */}
      {!isResultsModalOpen && (
        <>
          <QuickQuote 
            items={quoteItems} 
            onRemoveItem={removeFromQuote} 
            onClear={() => setQuoteItems([])} 
          />
        </>
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
