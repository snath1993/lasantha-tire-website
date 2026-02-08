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
  const [jobCounts, setJobCounts] = useState({ whatsapp: 0, facebook: 0 });
  
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

  useEffect(() => {
    if (isAuthenticated) {
      fetch('/api/jobs/config')
        .then(res => res.json())
        .then(data => {
          // Count enabled jobs from the config
          const count = Object.values(data).filter((j: any) => j.enabled).length;
          setJobCounts(prev => ({ ...prev, whatsapp: count }));
        })
        .catch(err => console.error('Failed to load job config:', err));
    }
  }, [isAuthenticated]);

  if (!isMounted || isAuthenticated === null) return null;
  if (!isAuthenticated) return null;

  return (
    <div className="space-y-6">
      {/* Mobile View - Tabbed Content - UNCHANGED */}
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

      {/* Desktop View - REDESIGNED Grid Layout (Hidden on Mobile) */}
      <div className="hidden md:block space-y-6">
        
        {/* Welcome Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-white">Welcome back, Admin</h1>
            <p className="text-slate-400 text-sm mt-1">Here's what's happening with your business today.</p>
          </div>
          <div className="flex items-center gap-3">
            <span className="flex items-center gap-2 px-4 py-2 rounded-xl bg-emerald-500/10 border border-emerald-500/20">
              <div className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
              <span className="text-sm font-medium text-emerald-400">All Systems Online</span>
            </span>
          </div>
        </div>

        {/* Quick Stats Row */}
        <div className="grid grid-cols-4 gap-4">
          <div className="bg-slate-800/50 rounded-2xl p-5 border border-slate-700/50">
            <div className="flex items-center justify-between mb-3">
              <span className="text-slate-400 text-sm">Today's Sales</span>
              <BarChart3 size={18} className="text-blue-400" />
            </div>
            <p className="text-2xl font-bold text-white">Rs. 125,400</p>
            <p className="text-xs text-emerald-400 mt-1">↑ 12% from yesterday</p>
          </div>
          <div className="bg-slate-800/50 rounded-2xl p-5 border border-slate-700/50">
            <div className="flex items-center justify-between mb-3">
              <span className="text-slate-400 text-sm">Active Jobs</span>
              <Settings size={18} className="text-violet-400" />
            </div>
            <p className="text-2xl font-bold text-white">{jobCounts.whatsapp + jobCounts.facebook}</p>
            <p className="text-xs text-slate-500 mt-1">Running smoothly</p>
          </div>
          <div className="bg-slate-800/50 rounded-2xl p-5 border border-slate-700/50">
            <div className="flex items-center justify-between mb-3">
              <span className="text-slate-400 text-sm">WhatsApp Messages</span>
              <MessageSquare size={18} className="text-emerald-400" />
            </div>
            <p className="text-2xl font-bold text-white">47</p>
            <p className="text-xs text-slate-500 mt-1">Today's conversations</p>
          </div>
          <div className="bg-slate-800/50 rounded-2xl p-5 border border-slate-700/50">
            <div className="flex items-center justify-between mb-3">
              <span className="text-slate-400 text-sm">Low Stock Items</span>
              <Package size={18} className="text-amber-400" />
            </div>
            <p className="text-2xl font-bold text-white">8</p>
            <p className="text-xs text-amber-400 mt-1">Needs attention</p>
          </div>
        </div>

        {/* Main Content Grid */}
        <div className="grid grid-cols-3 gap-6">
          
          {/* ERP Systems Card */}
          <div className="col-span-2 bg-slate-800/50 rounded-2xl p-6 border border-slate-700/50">
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-blue-500/20 flex items-center justify-center">
                  <Database size={20} className="text-blue-400" />
                </div>
                <div>
                  <h2 className="text-lg font-bold text-white">ERP Systems</h2>
                  <p className="text-xs text-slate-500">Manage inventory & financial data</p>
                </div>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <button 
                onClick={() => router.push('/dashboard/erp')}
                className="flex items-center gap-4 p-4 rounded-xl bg-slate-900/50 hover:bg-slate-900 border border-slate-700/50 hover:border-blue-500/30 transition-all group text-left"
              >
                <div className="w-12 h-12 rounded-xl bg-blue-500/10 flex items-center justify-center group-hover:bg-blue-500/20 transition-colors">
                  <Database size={22} className="text-blue-400" />
                </div>
                <div className="flex-1">
                  <span className="block font-semibold text-white">SQL ERP</span>
                  <span className="text-xs text-slate-500">Inventory & Sales</span>
                </div>
                <ArrowRight size={16} className="text-slate-600 group-hover:text-blue-400 transition-colors" />
              </button>

              <button 
                onClick={() => setShowPeachtree(true)}
                className="flex items-center gap-4 p-4 rounded-xl bg-slate-900/50 hover:bg-slate-900 border border-slate-700/50 hover:border-emerald-500/30 transition-all group text-left"
              >
                <div className="w-12 h-12 rounded-xl bg-emerald-500/10 flex items-center justify-center group-hover:bg-emerald-500/20 transition-colors">
                  <Briefcase size={22} className="text-emerald-400" />
                </div>
                <div className="flex-1">
                  <span className="block font-semibold text-white">Peachtree</span>
                  <span className="text-xs text-slate-500">Accounting</span>
                </div>
                <ArrowRight size={16} className="text-slate-600 group-hover:text-emerald-400 transition-colors" />
              </button>
            </div>
          </div>

          {/* Analytics Quick Card */}
          <div 
            onClick={() => router.push('/dashboard/analytics')}
            className="bg-gradient-to-br from-violet-600 to-violet-700 rounded-2xl p-6 cursor-pointer hover:from-violet-500 hover:to-violet-600 transition-all group"
          >
            <BarChart3 size={28} className="text-white/90 mb-4" />
            <h2 className="text-xl font-bold text-white mb-2">Analytics</h2>
            <p className="text-violet-200 text-sm mb-6">View detailed sales reports and performance metrics.</p>
            <div className="flex items-center text-sm font-semibold text-white/90 group-hover:text-white">
              View Reports 
              <ArrowRight size={16} className="ml-2 group-hover:translate-x-1 transition-transform" />
            </div>
          </div>
        </div>

        {/* Second Row */}
        <div className="grid grid-cols-3 gap-6">
          
          {/* Automation Jobs */}
          <div className="bg-slate-800/50 rounded-2xl p-6 border border-slate-700/50">
            <div className="flex items-center justify-between mb-5">
              <h2 className="text-lg font-bold text-white">Automation</h2>
              <button 
                onClick={() => router.push('/dashboard/jobs')}
                className="text-xs text-blue-400 hover:text-blue-300 font-medium"
              >
                View All →
              </button>
            </div>
            
            <div className="space-y-3">
              <button 
                onClick={() => router.push('/dashboard/jobs?category=whatsapp')}
                className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-900/50 hover:bg-slate-900 border border-slate-700/50 transition-all text-left group"
              >
                <div className="w-9 h-9 rounded-lg bg-emerald-500/10 flex items-center justify-center">
                  <Smartphone size={16} className="text-emerald-400" />
                </div>
                <span className="flex-1 font-medium text-slate-300 group-hover:text-white text-sm">WhatsApp Jobs</span>
                <span className="text-xs font-bold bg-slate-800 px-2 py-1 rounded-md text-slate-400">{jobCounts.whatsapp}</span>
              </button>
              
              <button 
                onClick={() => router.push('/dashboard/jobs?category=facebook')}
                className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-900/50 hover:bg-slate-900 border border-slate-700/50 transition-all text-left group"
              >
                <div className="w-9 h-9 rounded-lg bg-blue-500/10 flex items-center justify-center">
                  <Facebook size={16} className="text-blue-400" />
                </div>
                <span className="flex-1 font-medium text-slate-300 group-hover:text-white text-sm">Facebook Jobs</span>
                <span className="text-xs font-bold bg-slate-800 px-2 py-1 rounded-md text-slate-400">{jobCounts.facebook}</span>
              </button>
            </div>
          </div>

          {/* WhatsApp Live */}
          <div 
            onClick={() => router.push('/dashboard/whatsapp-live')}
            className="bg-gradient-to-br from-emerald-600 to-emerald-700 rounded-2xl p-6 cursor-pointer hover:from-emerald-500 hover:to-emerald-600 transition-all group"
          >
            <MessageSquare size={28} className="text-white/90 mb-4" />
            <h2 className="text-xl font-bold text-white mb-2">WhatsApp Live</h2>
            <p className="text-emerald-100 text-sm mb-6">Direct access to WhatsApp Web interface.</p>
            <div className="flex items-center text-sm font-semibold text-white/90 group-hover:text-white">
              Open Chat 
              <ArrowRight size={16} className="ml-2 group-hover:translate-x-1 transition-transform" />
            </div>
          </div>

          {/* System Logs */}
          <div 
            onClick={() => router.push('/dashboard/terminal-logs')}
            className="bg-slate-800/50 rounded-2xl p-6 border border-slate-700/50 cursor-pointer hover:bg-slate-800 transition-all group"
          >
            <Terminal size={28} className="text-slate-400 group-hover:text-slate-300 mb-4 transition-colors" />
            <h2 className="text-xl font-bold text-white mb-2">System Logs</h2>
            <p className="text-slate-500 text-sm mb-6">Monitor server processes and errors.</p>
            <div className="flex items-center text-sm font-medium text-slate-400 group-hover:text-white transition-colors">
              View Logs 
              <ArrowRight size={16} className="ml-2 group-hover:translate-x-1 transition-transform" />
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
