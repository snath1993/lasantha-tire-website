"use client";

import Link from "next/link";
import Image from "next/image";
import { useRouter, usePathname } from "next/navigation";
import { cn } from '@/core/lib/utils';
import { 
  LayoutDashboard, 
  BarChart3, 
  Bot, 
  Settings, 
  History,
  PlayCircle,
  Database,
  X,
  LogOut,
  QrCode,
  Terminal,
  Activity,
  FileText,
  Package,
  Briefcase,
  ChevronRight
} from "lucide-react";

interface MenuItem {
  icon: any;
  label: string;
  href: string;
  color?: string;
  description?: string;
}

const menuSections: { title: string; items: MenuItem[] }[] = [
  {
    title: "Core Modules",
    items: [
      { icon: LayoutDashboard, label: "Dashboard Home", href: "/dashboard", color: "text-blue-400", description: "Main overview" },
      { icon: PlayCircle, label: "Job Manager", href: "/dashboard/jobs", color: "text-green-400", description: "Manage active jobs" },
      { icon: BarChart3, label: "Sales Analytics", href: "/dashboard/analytics", color: "text-purple-400", description: "Reports & trends" },
    ]
  },
  {
    title: "ERP & Finance",
    items: [
      { icon: Database, label: "Peachtree ERP", href: "/dashboard/erp-peachtree", color: "text-orange-400", description: "Customers, Vendors, GL" },
      { icon: Activity, label: "Business Status", href: "/dashboard/business-status", color: "text-emerald-400", description: "Live financial health" },
      { icon: FileText, label: "Quotations", href: "/dashboard?tab=quotation", color: "text-yellow-400", description: "Create & manage quotes" },
      { icon: Package, label: "Inventory", href: "/dashboard?tab=inventory", color: "text-cyan-400", description: "Stock levels" },
    ]
  },
  {
    title: "Communication",
    items: [
      { icon: Bot, label: "WhatsApp Live", href: "/dashboard/whatsapp-live", color: "text-green-500", description: "Live chat & status" },
      { icon: QrCode, label: "Bot Connection", href: "/dashboard/bot-qr", color: "text-white", description: "Scan QR code" },
    ]
  },
  {
    title: "System",
    items: [
      { icon: History, label: "AI History", href: "/dashboard/ai-history", color: "text-indigo-400", description: "Past conversations" },
      { icon: Terminal, label: "System Logs", href: "/dashboard/terminal-logs", color: "text-slate-400", description: "Server logs" },
      { icon: Settings, label: "Settings", href: "/dashboard/settings", color: "text-slate-200", description: "App configuration" },
    ]
  }
];

export default function MenuPage() {
  const pathname = usePathname();
  const router = useRouter();

  const handleLogout = async () => {
    try {
      await fetch('/api/auth/logout', { method: 'POST' });
      sessionStorage.removeItem('sessionId');
      router.replace('/');
    } catch (error) {
      console.error('Logout failed', error);
      router.replace('/');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex flex-col bg-slate-950 text-white overflow-hidden">
      
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b border-white/10 bg-slate-900/50 backdrop-blur-md">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center overflow-hidden shadow-lg shadow-blue-900/20">
            <Image 
              src="/shop-logo.jpg" 
              alt="Logo" 
              width={40} 
              height={40} 
              className="w-full h-full object-cover"
            />
          </div>
          <div>
            <h2 className="text-lg font-bold text-white leading-none">Menu</h2>
            <p className="text-xs text-slate-400 mt-1">Lasantha Tire Service v2.0</p>
          </div>
        </div>
        <Link 
          href="/dashboard"
          className="p-2 text-slate-400 hover:text-white bg-white/5 hover:bg-white/10 rounded-full transition-colors"
        >
          <X size={24} />
        </Link>
      </div>

      {/* Scrollable Content */}
      <div className="flex-1 overflow-y-auto p-4 pb-24 space-y-6">
        {menuSections.map((section, idx) => (
          <div key={idx} className="space-y-3">
            <h3 className="text-xs font-semibold text-slate-500 uppercase tracking-wider px-1">
              {section.title}
            </h3>
            <div className="grid grid-cols-1 gap-2">
              {section.items.map((item) => {
                const Icon = item.icon;
                const isActive = pathname === item.href;
                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    className={cn(
                      "flex items-center gap-4 p-3 rounded-xl border transition-all duration-200 group",
                      isActive 
                        ? "bg-blue-600/10 border-blue-500/50" 
                        : "bg-slate-900/50 border-white/5 hover:bg-slate-800 hover:border-white/10"
                    )}
                  >
                    <div className={cn(
                      "p-2.5 rounded-lg bg-slate-950 border border-white/5 group-hover:scale-105 transition-transform",
                      item.color
                    )}>
                      <Icon size={20} />
                    </div>
                    <div className="flex-1">
                      <div className={cn("font-medium text-sm", isActive ? "text-blue-400" : "text-slate-200")}>
                        {item.label}
                      </div>
                      {item.description && (
                        <div className="text-[11px] text-slate-500 mt-0.5">
                          {item.description}
                        </div>
                      )}
                    </div>
                    <ChevronRight size={16} className="text-slate-600 group-hover:text-slate-400 group-hover:translate-x-0.5 transition-all" />
                  </Link>
                );
              })}
            </div>
          </div>
        ))}

        {/* Logout Section */}
        <div className="pt-4 border-t border-white/10">
          <button
            onClick={handleLogout}
            className="w-full flex items-center justify-center gap-2 p-4 rounded-xl bg-red-500/10 border border-red-500/20 text-red-400 hover:bg-red-500/20 hover:text-red-300 transition-all"
          >
            <LogOut size={20} />
            <span className="font-medium">Sign Out</span>
          </button>
          <p className="text-center text-[10px] text-slate-600 mt-4">
            System Version 2.0.0 (Build 2025.12.26)
          </p>
        </div>
      </div>
    </div>
  );
}
