'use client';

import { Home, Package, Menu, Briefcase, FileText } from 'lucide-react';
import Link from 'next/link';
import { useSearchParams } from 'next/navigation';
import { cn } from '@/lib/utils';

export default function MobileNav() {
  const searchParams = useSearchParams();
  const activeTab = searchParams.get('tab') || 'home';

  const isActive = (tab: string) => activeTab === tab;

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-slate-900/90 backdrop-blur-xl border-t border-white/10 pb-safe z-50 md:hidden shadow-[0_-10px_40px_-15px_rgba(0,0,0,0.5)]">
      <div className="flex justify-around items-center h-20 relative px-2">
        
        <Link 
          href="/dashboard" 
          className={cn(
            "flex flex-col items-center gap-1.5 w-full h-full justify-center transition-all duration-300 group",
            isActive('home') ? "text-blue-400" : "text-slate-500 hover:text-slate-300"
          )}
        >
          <div className={cn("p-1.5 rounded-xl transition-all", isActive('home') && "bg-blue-500/10")}>
            <Home className={cn("w-6 h-6", isActive('home') && "fill-blue-500/20")} />
          </div>
          <span className="text-[10px] font-medium">Home</span>
        </Link>
        
        <Link 
          href="/dashboard?tab=inventory" 
          className={cn(
            "flex flex-col items-center gap-1.5 w-full h-full justify-center transition-all duration-300 group",
            isActive('inventory') ? "text-blue-400" : "text-slate-500 hover:text-slate-300"
          )}
        >
          <div className={cn("p-1.5 rounded-xl transition-all", isActive('inventory') && "bg-blue-500/10")}>
            <Package className={cn("w-6 h-6", isActive('inventory') && "fill-blue-500/20")} />
          </div>
          <span className="text-[10px] font-medium">Stock</span>
        </Link>

        <Link 
          href="/dashboard?tab=quotation" 
          className={cn(
            "flex flex-col items-center gap-1.5 w-full h-full justify-center transition-all duration-300 group",
            isActive('quotation') ? "text-blue-400" : "text-slate-500 hover:text-slate-300"
          )}
        >
          <div className={cn("p-1.5 rounded-xl transition-all", isActive('quotation') && "bg-blue-500/10")}>
            <FileText className={cn("w-6 h-6", isActive('quotation') && "fill-blue-500/20")} />
          </div>
          <span className="text-[10px] font-medium">Quote</span>
        </Link>

        <Link 
          href="/dashboard?tab=peachtree" 
          className={cn(
            "flex flex-col items-center gap-1.5 w-full h-full justify-center transition-all duration-300 group",
            isActive('peachtree') ? "text-blue-400" : "text-slate-500 hover:text-slate-300"
          )}
        >
          <div className={cn("p-1.5 rounded-xl transition-all", isActive('peachtree') && "bg-blue-500/10")}>
            <Briefcase className={cn("w-6 h-6", isActive('peachtree') && "fill-blue-500/20")} />
          </div>
          <span className="text-[10px] font-medium">Peachtree</span>
        </Link>

        <Link 
          href="/dashboard?tab=menu"
          className={cn(
            "flex flex-col items-center gap-1.5 w-full h-full justify-center transition-all duration-300 group",
            isActive('menu') ? "text-blue-400" : "text-slate-500 hover:text-slate-300"
          )}
        >
          <div className={cn("p-1.5 rounded-xl transition-all", isActive('menu') && "bg-blue-500/10")}>
            <Menu className={cn("w-6 h-6", isActive('menu') && "fill-blue-500/20")} />
          </div>
          <span className="text-[10px] font-medium">Menu</span>
        </Link>
      </div>
    </div>
  );
}
