"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import { 
  LayoutDashboard, 
  BarChart3, 
  Bot, 
  Settings, 
  History,
  PlayCircle,
  Database,
  LogOut,
  ChevronRight
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

  return (
    <aside className="fixed left-6 top-6 bottom-6 w-[280px] hidden md:flex flex-col z-50">
      {/* Floating Dock Container */}
      <div className="flex-1 flex flex-col bg-[#18181b] dark:bg-black rounded-[2rem] shadow-2xl shadow-black/10 overflow-hidden border border-white/5 relative">
        
        {/* Decorative Top Gradient */}
        <div className="absolute top-0 left-0 right-0 h-32 bg-gradient-to-b from-indigo-500/10 to-transparent pointer-events-none" />

        {/* Header */}
        <div className="p-8 relative z-10">
          <div className="flex items-center gap-4 mb-8">
            <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-lg shadow-indigo-500/25 text-white">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                <path d="M2 17L12 22L22 17" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                <path d="M2 12L12 17L22 12" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
              </svg>
            </div>
            <div>
              <h1 className="text-xl font-bold text-white tracking-tight">
                Lasantha
              </h1>
              <p className="text-xs text-zinc-400 font-medium tracking-wide uppercase">
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
                    ? "bg-white text-black shadow-xl shadow-white/10" 
                    : "text-zinc-400 hover:text-white hover:bg-white/5"
                )}
              >
                <Icon size={22} className={cn("transition-colors", isActive ? "text-indigo-600" : "text-zinc-500 group-hover:text-white")} />
                <span className="font-medium text-sm tracking-wide">{item.label}</span>
                
                {isActive && (
                  <ChevronRight size={16} className="ml-auto text-indigo-600" />
                )}
              </Link>
            );
          })}
        </nav>

        {/* Footer Status */}
        <div className="p-6 relative z-10">
          <div className="bg-white/5 rounded-2xl p-4 border border-white/5 backdrop-blur-sm">
            <div className="flex items-center gap-3">
              <div className="relative">
                <div className="w-2 h-2 rounded-full bg-emerald-500 shadow-[0_0_10px_rgba(16,185,129,0.5)]" />
              </div>
              <div className="flex flex-col">
                <span className="text-xs font-medium text-white">System Operational</span>
                <span className="text-[10px] text-zinc-500">Latency: 24ms</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </aside>
  );
}
