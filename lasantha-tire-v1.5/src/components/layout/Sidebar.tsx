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
  X,
  MessageSquare,
  Terminal,
  Briefcase,
  Activity
} from "lucide-react";

// Menu items grouped by category
const mainMenuItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/dashboard", description: "Overview & Stats" },
  { icon: BarChart3, label: "Analytics", href: "/dashboard/analytics", description: "Sales Reports" },
];

const systemMenuItems = [
  { icon: Database, label: "SQL ERP", href: "/dashboard/erp", description: "Inventory Data" },
  { icon: Briefcase, label: "Peachtree", href: "/dashboard/erp-peachtree", description: "Accounting" },
];

const automationMenuItems = [
  { icon: MessageSquare, label: "WhatsApp", href: "/dashboard/whatsapp-live", description: "Live Messages" },
  { icon: PlayCircle, label: "Jobs", href: "/dashboard/jobs", description: "Automation" },
  { icon: History, label: "AI History", href: "/dashboard/ai-history", description: "Chat Logs" },
];

const settingsMenuItems = [
  { icon: Terminal, label: "Logs", href: "/dashboard/terminal-logs", description: "System Logs" },
  { icon: Settings, label: "Settings", href: "/dashboard/settings", description: "Configuration" },
];

// For mobile menu (flat list)
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

  const renderMenuItem = (item: typeof mainMenuItems[0], isActive: boolean) => {
    const Icon = item.icon;
    return (
      <Link
        key={item.href}
        href={item.href}
        className={cn(
          "flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 group",
          isActive 
            ? "bg-gradient-to-r from-blue-600 to-blue-500 text-white shadow-lg shadow-blue-600/30" 
            : "text-slate-300 hover:text-white hover:bg-white/10"
        )}
      >
        <div className={cn(
          "w-9 h-9 rounded-lg flex items-center justify-center transition-all",
          isActive 
            ? "bg-white/20" 
            : "bg-slate-800/50 group-hover:bg-slate-700/50"
        )}>
          <Icon size={18} className={isActive ? "text-white" : "text-slate-400 group-hover:text-white"} />
        </div>
        <div className="flex-1 min-w-0">
          <span className="block font-semibold text-sm">{item.label}</span>
          <span className={cn(
            "block text-[10px] truncate",
            isActive ? "text-blue-100" : "text-slate-500 group-hover:text-slate-400"
          )}>{item.description}</span>
        </div>
        {isActive && (
          <ChevronRight size={14} className="text-white/70" />
        )}
      </Link>
    );
  };

  return (
    <>
      {/* Mobile Popup Menu - UNCHANGED */}
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

      {/* Desktop Sidebar - REDESIGNED */}
      <aside className="hidden md:flex fixed left-0 top-0 bottom-0 w-[260px] flex-col z-50 bg-slate-950 border-r border-slate-800/50">
        
        {/* Logo Header */}
        <div className="p-5 border-b border-slate-800/50">
          <div className="flex items-center gap-3">
            <div className="w-11 h-11 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-500/30 overflow-hidden">
              <Image 
                src="/shop-logo.jpg" 
                alt="Lasantha Tire Service" 
                width={44} 
                height={44} 
                className="w-full h-full object-cover"
              />
            </div>
            <div>
              <h1 className="text-lg font-bold text-white tracking-tight">Lasantha</h1>
              <p className="text-[10px] text-slate-500 font-medium tracking-widest uppercase">Tire Service</p>
            </div>
          </div>
        </div>
        
        {/* Navigation - Scrollable */}
        <nav className="flex-1 overflow-y-auto py-4 px-3 space-y-6 custom-scrollbar">
          
          {/* Main Section */}
          <div>
            <p className="px-4 mb-2 text-[10px] font-bold text-slate-600 uppercase tracking-widest">Main</p>
            <div className="space-y-1">
              {mainMenuItems.map((item) => renderMenuItem(item, pathname === item.href))}
            </div>
          </div>

          {/* Systems Section */}
          <div>
            <p className="px-4 mb-2 text-[10px] font-bold text-slate-600 uppercase tracking-widest">Systems</p>
            <div className="space-y-1">
              {systemMenuItems.map((item) => renderMenuItem(item, pathname === item.href))}
            </div>
          </div>

          {/* Automation Section */}
          <div>
            <p className="px-4 mb-2 text-[10px] font-bold text-slate-600 uppercase tracking-widest">Automation</p>
            <div className="space-y-1">
              {automationMenuItems.map((item) => renderMenuItem(item, pathname === item.href))}
            </div>
          </div>

          {/* Settings Section */}
          <div>
            <p className="px-4 mb-2 text-[10px] font-bold text-slate-600 uppercase tracking-widest">System</p>
            <div className="space-y-1">
              {settingsMenuItems.map((item) => renderMenuItem(item, pathname === item.href))}
            </div>
          </div>

        </nav>

        {/* Footer Status */}
        <div className="p-4 border-t border-slate-800/50">
          <div className="flex items-center gap-3 px-3 py-3 rounded-xl bg-slate-900/50">
            <div className="relative flex items-center justify-center">
              <div className="w-2.5 h-2.5 rounded-full bg-emerald-500 animate-pulse" />
              <div className="absolute w-2.5 h-2.5 rounded-full bg-emerald-500/50 animate-ping" />
            </div>
            <div className="flex-1">
              <p className="text-xs font-semibold text-white">System Online</p>
              <p className="text-[10px] text-slate-500">All services running</p>
            </div>
            <Activity size={14} className="text-emerald-500" />
          </div>
        </div>
      </aside>
    </>
  );
}
