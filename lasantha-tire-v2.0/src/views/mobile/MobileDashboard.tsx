'use client';

import { useRouter } from 'next/navigation';
import { useModal } from '@/core/contexts/ModalContext';
import PeachtreeDashboard from '@/views/shared/erp/PeachtreeDashboard';
import TireSearch from '@/views/mobile/TireSearch';
import StockView from '@/views/mobile/StockView';
import QuickQuote from '@/views/mobile/QuickQuote';
import QuotationView from '@/views/mobile/QuotationView';
import TitanView from '@/views/mobile/TitanView';
import { 
  Database, 
  MessageSquare, 
  BarChart3, 
  Terminal, 
  Settings, 
  ArrowRight,
  Bot
} from 'lucide-react';

type MobileMenuProps = {
  onOpenInventory: () => void;
  onOpenPeachtree: () => void;
  onOpenAnalytics: () => void;
  onOpenJobs: () => void;
  onOpenWhatsAppLive: () => void;
  onOpenSystemLogs: () => void;
  onOpenTitan: () => void;
};

function MobileMenu({
  onOpenInventory,
  onOpenPeachtree,
  onOpenAnalytics,
  onOpenJobs,
  onOpenWhatsAppLive,
  onOpenSystemLogs,
  onOpenTitan,
}: MobileMenuProps) {
  return (
    <div className="space-y-4 pb-24">
      <h2 className="text-xl font-bold text-zinc-900 mb-4">Menu</h2>

      <div className="bg-indigo-600 rounded-2xl p-4 text-white mb-6 shadow-lg shadow-indigo-200">
         <div className="flex items-center justify-between mb-2">
            <h3 className="font-bold text-lg">Titan AI Manager</h3>
            <div className="bg-white/20 p-2 rounded-lg">
              <Bot size={24} className="text-white" />
            </div>
         </div>
         <p className="text-indigo-100 text-sm mb-4">Control system jobs and analyze data via AI chat.</p>
         <button 
           onClick={onOpenTitan}
           className="w-full bg-white text-indigo-600 font-bold py-2 rounded-xl flex items-center justify-center gap-2 text-sm"
         >
           Open Chat <ArrowRight size={16} />
         </button>
      </div>

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

// Need to import Briefcase locally or pass it
import { Briefcase } from 'lucide-react';

interface MobileDashboardProps {
  activeTab: string;
  quoteItems: any[];
  addToQuote: (item: any) => void;
  removeFromQuote: (index: number) => void;
  setQuoteItems: (items: any[]) => void;
  setShowPeachtree: (show: boolean) => void;
}

export default function MobileDashboard({
  activeTab,
  quoteItems,
  addToQuote,
  removeFromQuote,
  setQuoteItems,
  setShowPeachtree
}: MobileDashboardProps) {
  const router = useRouter();
  const { isResultsModalOpen } = useModal();

  return (
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

      {activeTab === 'titan' && (
        <TitanView />
      )}

      {activeTab === 'menu' && (
        <MobileMenu
          onOpenInventory={() => router.push('/dashboard/erp')}
          onOpenPeachtree={() => setShowPeachtree(true)}
          onOpenAnalytics={() => router.push('/dashboard/analytics')}
          onOpenJobs={() => router.push('/dashboard/jobs')}
          onOpenWhatsAppLive={() => router.push('/dashboard/whatsapp-live')}
          onOpenSystemLogs={() => router.push('/dashboard/terminal-logs')}
          onOpenTitan={() => router.push('/dashboard?tab=titan')}
        />
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
