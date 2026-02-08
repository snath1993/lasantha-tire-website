'use client';

import { Bell, Search, User, Menu, Command, Calendar, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useModal } from "@/contexts/ModalContext";
import { usePathname } from "next/navigation";

// Page titles mapping
const pageTitles: Record<string, { title: string; subtitle: string }> = {
  '/dashboard': { title: 'Dashboard', subtitle: 'System overview and quick actions' },
  '/dashboard/analytics': { title: 'Analytics', subtitle: 'Sales reports and statistics' },
  '/dashboard/erp': { title: 'SQL ERP', subtitle: 'Inventory and sales data' },
  '/dashboard/erp-peachtree': { title: 'Peachtree', subtitle: 'Accounting and finance' },
  '/dashboard/whatsapp-live': { title: 'WhatsApp Live', subtitle: 'Real-time messages' },
  '/dashboard/jobs': { title: 'Job Manager', subtitle: 'Automation and scheduled tasks' },
  '/dashboard/ai-history': { title: 'AI History', subtitle: 'Conversation logs' },
  '/dashboard/terminal-logs': { title: 'System Logs', subtitle: 'Server processes and errors' },
  '/dashboard/settings': { title: 'Settings', subtitle: 'System configuration' },
};

export function Header() {
  const { isResultsModalOpen } = useModal();
  const pathname = usePathname();
  
  if (isResultsModalOpen) return null;

  const pageInfo = pageTitles[pathname] || { title: 'Dashboard', subtitle: 'Welcome' };
  const today = new Date().toLocaleDateString('en-US', { 
    weekday: 'short', 
    month: 'short', 
    day: 'numeric' 
  });
  
  return (
    <header className="hidden md:block sticky top-0 z-40 bg-slate-900/80 backdrop-blur-xl border-b border-slate-800/50">
      <div className="px-8 py-4">
        <div className="flex items-center justify-between">
          {/* Left Side - Page Title */}
          <div>
            <h1 className="text-xl font-bold text-white">{pageInfo.title}</h1>
            <p className="text-sm text-slate-500">{pageInfo.subtitle}</p>
          </div>

          {/* Right Side - Search & Actions */}
          <div className="flex items-center gap-4">
            {/* Search */}
            <div className="relative group">
              <div className="absolute inset-0 bg-slate-800/80 rounded-xl border border-slate-700/50 transition-all group-hover:border-slate-600" />
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500 group-focus-within:text-blue-400 transition-colors" size={16} />
              <input 
                type="text" 
                placeholder="Search..." 
                className="relative w-56 pl-10 pr-16 py-2.5 rounded-xl bg-transparent border-none focus:ring-0 text-sm text-white placeholder:text-slate-500"
                suppressHydrationWarning={true}
              />
              <div className="absolute right-3 top-1/2 -translate-y-1/2 flex items-center gap-1 px-1.5 py-0.5 rounded bg-slate-700/50 border border-slate-600/50">
                <Command size={10} className="text-slate-400" />
                <span className="text-[10px] font-bold text-slate-400">K</span>
              </div>
            </div>

            {/* Date */}
            <div className="flex items-center gap-2 px-3 py-2 rounded-xl bg-slate-800/50 border border-slate-700/50">
              <Calendar size={14} className="text-slate-500" />
              <span className="text-xs font-medium text-slate-400">{today}</span>
            </div>

            {/* Refresh Button */}
            <button className="p-2.5 rounded-xl bg-slate-800/50 border border-slate-700/50 text-slate-400 hover:text-white hover:bg-slate-700/50 transition-all">
              <RefreshCw size={16} />
            </button>

            {/* User Avatar */}
            <div className="flex items-center gap-3 pl-3 border-l border-slate-700/50">
              <div className="text-right">
                <p className="text-sm font-semibold text-white">Admin</p>
                <p className="text-[10px] text-slate-500">Super User</p>
              </div>
              <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-white shadow-lg shadow-blue-500/20">
                <User size={16} />
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
}
