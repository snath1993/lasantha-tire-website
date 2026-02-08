"use client";

import Link from "next/link";
import Image from "next/image";
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

export default function MenuPage() {
  const pathname = usePathname();

  return (
    <div className="fixed inset-0 z-50 flex items-end sm:items-center justify-center p-4 pb-24 sm:pb-4 bg-slate-900/90 backdrop-blur-md">
      
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
          <Link 
            href="/dashboard"
            className="p-2 text-slate-400 hover:text-white bg-white/5 rounded-full"
          >
            <X size={20} />
          </Link>
        </div>

        <div className="grid grid-cols-3 gap-3">
          {menuItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href;
            return (
              <Link
                key={item.href}
                href={item.href}
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
  );
}
