"use client";

import Link from "next/link";
import Image from "next/image";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import { useModal } from "@/contexts/ModalContext";
import { 
  LayoutDashboard, 
  BarChart3, 
  Bot, 
  Settings, 
  History,
  PlayCircle,
  Database,
  LogOut,
  ChevronRight,
  X
} from "lucide-react";

const menuItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/dashboard" },
  { icon: BarChart3, label: "Sales Analytics", href: "/dashboard/analytics" },
  { icon: Database, label: "ERP System", href: "/dashboard/erp" },
  { icon: Bot, label: "WhatsApp Live", href: "/dashboard/whatsapp-live" },
  { icon: PlayCircle, label: "Job Manager", href: "/dashboard/jobs" },
  { icon: History, label: "AI History", href: "/dashboard/ai-history" },
  { icon: Settings, label: "Settings", href: "/dashboard/settings" },
];

export function Sidebar() {
  const pathname = usePathname();
  const { isMobileMenuOpen, setIsMobileMenuOpen } = useModal();

  return (
    <>
      {/* Mobile Popup Menu */}
      {isMobileMenuOpen && (
        <div className="fixed inset-0 z-50 md:hidden flex items-end sm:items-center justify-center p-4 pb-24 sm:pb-4">
          {/* Backdrop */}
          <div 
            className="absolute inset-0 bg-slate-900/90 backdrop-blur-md"
            onClick={() => setIsMobileMenuOpen(false)}
          />
          
          {/* Popup Content */}
          <div className="relative w-full max-w-sm bg-slate-900/95 border border-white/10 rounded-3xl p-6 shadow-2xl animate-in slide-in-from-bottom-10 zoom-in-95 duration-200">
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center overflow-hidden">
                  <Image 
                    src="/shop-logo.jpg" 
                    alt="Logo" 
                    width={40} 
                    height={40} 
                    className="w-full h-full object-cover"
                  />
                </div>
                <div>
                  <h2 className="text-lg font-bold text-white">Menu</h2>
                  <p className="text-xs text-slate-400">Lasantha Tire Service</p>
                </div>
              </div>
              <button 
                onClick={() => setIsMobileMenuOpen(false)}
                className="p-2 text-slate-400 hover:text-white bg-white/5 rounded-full"
              >
                <X size={20} />
              </button>
            </div>

            <div className="grid grid-cols-3 gap-3">
              {menuItems.map((item) => {
                const Icon = item.icon;
                const isActive = pathname === item.href;
                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    onClick={() => setIsMobileMenuOpen(false)}
                    className={cn(
                      "flex flex-col items-center gap-3 p-4 rounded-2xl border transition-all duration-200",
                      isActive 
                        ? "bg-blue-600 border-blue-500 text-white shadow-lg shadow-blue-600/20" 
                        : "bg-slate-800/50 border-white/5 text-slate-400 hover:bg-slate-800 hover:text-white hover:border-white/10"
                    )}
                  >
                    <Icon size={24} />
                    <span className="text-[10px] font-medium text-center leading-tight">{item.label}</span>
                  </Link>
                );
              })}
            </div>
          </div>
        </div>
      )}

      {/* Desktop Sidebar (Hidden on Mobile) */}
      <aside className="hidden md:flex fixed left-6 top-6 bottom-6 w-[280px] flex-col z-50">
        {/* Floating Dock Container */}
        <div className="flex-1 flex flex-col bg-slate-900/50 backdrop-blur-xl rounded-[2rem] shadow-2xl shadow-black/20 overflow-hidden border border-white/10 relative">
          
        {/* Decorative Top Gradient */}
        <div className="absolute top-0 left-0 right-0 h-32 bg-gradient-to-b from-blue-500/10 to-transparent pointer-events-none" />

        {/* Header */}
        <div className="p-8 relative z-10">
          <div className="flex items-center gap-4 mb-8">
            <div className="w-12 h-12 rounded-2xl bg-white flex items-center justify-center shadow-lg shadow-blue-500/25 overflow-hidden">
              <Image 
                src="/shop-logo.jpg" 
                alt="Lasantha Tire Service" 
                width={48} 
                height={48} 
                className="w-full h-full object-cover"
              />
            </div>
            <div>
              <h1 className="text-xl font-bold text-white tracking-tight">
                Lasantha
              </h1>
              <p className="text-xs text-slate-400 font-medium tracking-wide uppercase">
                Tire Service
              </p>
            </div>
          </div>

          <div className="h-px w-full bg-gradient-to-r from-transparent via-white/10 to-transparent" />
        </div>
        
        {/* Navigation */}
        <nav className="flex-1 px-4 space-y-2 overflow-y-auto custom-scrollbar relative z-10">
          {menuItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href;
            
            return (
              <Link
                key={item.href}
                href={item.href}
                className={cn(
                  "flex items-center gap-4 px-5 py-4 rounded-2xl transition-all duration-300 group relative",
                  isActive 
                    ? "bg-blue-600 text-white shadow-lg shadow-blue-600/20" 
                    : "text-slate-400 hover:text-white hover:bg-white/5"
                )}
              >
                <Icon size={22} className={cn("transition-colors", isActive ? "text-white" : "text-slate-500 group-hover:text-white")} />
                <span className="font-medium text-sm tracking-wide">{item.label}</span>
                
                {isActive && (
                  <ChevronRight size={16} className="ml-auto text-white" />
                )}
              </Link>
            );
          })}
        </nav>

        {/* Footer Status */}
        <div className="p-6 relative z-10">
          <div className="bg-slate-800/50 rounded-2xl p-4 border border-white/5 backdrop-blur-sm">
            <div className="flex items-center gap-3">
              <div className="relative">
                <div className="w-2 h-2 rounded-full bg-emerald-500 shadow-[0_0_10px_rgba(16,185,129,0.5)]" />
              </div>
              <div className="flex flex-col">
                <span className="text-xs font-medium text-white">System Operational</span>
                <span className="text-[10px] text-slate-400">Latency: 24ms</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </aside>
    </>
  );
}
