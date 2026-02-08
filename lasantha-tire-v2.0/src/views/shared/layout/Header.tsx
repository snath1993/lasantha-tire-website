'use client';

import { Bell, Search, User, Menu, Command } from "lucide-react";
import { Button } from '@/views/shared/ui/button';
import { useModal } from '@/core/contexts/ModalContext';

export function Header() {
  const { isResultsModalOpen } = useModal();
  
  if (isResultsModalOpen) return null;
  
  return (
    <header className="fixed top-6 right-6 left-6 z-40 flex items-center justify-between pointer-events-none">
      {/* Left Side - Search (Floating Pill) */}
      <div className="pointer-events-auto flex items-center gap-4 w-full max-w-xl">
        
        <div className="relative w-full max-w-md hidden md:block group">
          <div className="absolute inset-0 bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl shadow-lg shadow-black/20 transition-all duration-300 group-hover:shadow-xl group-hover:shadow-blue-500/10" />
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-blue-400 transition-colors" size={20} />
          <input 
            type="text" 
            placeholder="Search..." 
            className="relative w-full pl-12 pr-12 py-3.5 rounded-2xl bg-transparent border-none focus:ring-0 text-sm font-medium text-white placeholder:text-slate-500"
            suppressHydrationWarning={true}
          />
          <div className="absolute right-4 top-1/2 -translate-y-1/2 flex items-center gap-1 px-1.5 py-0.5 rounded border border-slate-600 bg-slate-700/50">
            <Command size={10} className="text-slate-400" />
            <span className="text-[10px] font-bold text-slate-400">K</span>
          </div>
        </div>
      </div>

      {/* Right Side - Actions (Floating Pill) */}
      {/* Removed Bell and User icons as requested */}
      {/* <div className="pointer-events-auto flex items-center gap-3 bg-slate-800/50 backdrop-blur-xl p-2 pr-4 rounded-2xl shadow-lg shadow-black/20 border border-slate-700">
        <Button variant="ghost" size="icon" className="relative text-slate-400 hover:text-blue-400 hover:bg-slate-700/50 rounded-xl w-10 h-10 transition-colors">
          <Bell size={20} />
          <span className="absolute top-2.5 right-2.5 w-2 h-2 bg-rose-500 rounded-full ring-2 ring-slate-800" />
        </Button>
        
        <div className="h-8 w-px bg-slate-700 mx-1" />
        
        <div className="flex items-center gap-3 cursor-pointer group">
          <div className="text-right hidden md:block">
            <p className="text-sm font-bold text-white group-hover:text-blue-400 transition-colors">Admin</p>
            <p className="text-[10px] font-medium text-slate-400">Super User</p>
          </div>
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-600 to-purple-600 flex items-center justify-center text-white shadow-md shadow-blue-500/20 group-hover:scale-105 transition-transform">
            <User size={20} />
          </div>
        </div>
      </div> */}
    </header>
  );
}
