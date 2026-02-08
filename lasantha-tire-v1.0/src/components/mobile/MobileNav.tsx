'use client';

import { Home, FileText, Package, Users, Menu } from 'lucide-react';
import Link from 'next/link';
import { useSearchParams } from 'next/navigation';

export default function MobileNav() {
  const searchParams = useSearchParams();
  const activeTab = searchParams.get('tab') || 'home';

  const isActive = (tab: string) => activeTab === tab;

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-zinc-100 pb-safe z-40 md:hidden shadow-[0_-10px_40px_-15px_rgba(0,0,0,0.1)]">
      <div className="flex justify-around items-center h-16">
        <Link 
          href="/dashboard" 
          className={`flex flex-col items-center gap-1 w-full h-full justify-center transition-colors ${isActive('home') ? 'text-indigo-600' : 'text-zinc-400 hover:text-zinc-600'}`}
        >
          <Home className="w-6 h-6" />
          <span className="text-[10px] font-medium">Home</span>
        </Link>
        
        <Link 
          href="/dashboard?tab=inventory" 
          className={`flex flex-col items-center gap-1 w-full h-full justify-center transition-colors ${isActive('inventory') ? 'text-indigo-600' : 'text-zinc-400 hover:text-zinc-600'}`}
        >
          <Package className="w-6 h-6" />
          <span className="text-[10px] font-medium">Stock</span>
        </Link>

        {/* Center Action Button Placeholder - Quote is handled by floating button */}
        <div className="w-full h-full flex justify-center items-center pointer-events-none">
           {/* Space for floating action button */}
        </div>

        <Link 
          href="/dashboard?tab=customers" 
          className={`flex flex-col items-center gap-1 w-full h-full justify-center transition-colors ${isActive('customers') ? 'text-indigo-600' : 'text-zinc-400 hover:text-zinc-600'}`}
        >
          <Users className="w-6 h-6" />
          <span className="text-[10px] font-medium">Customers</span>
        </Link>

        <Link 
          href="/dashboard?tab=menu"
          className={`flex flex-col items-center gap-1 w-full h-full justify-center transition-colors ${isActive('menu') ? 'text-indigo-600' : 'text-zinc-400 hover:text-zinc-600'}`}
        >
          <Menu className="w-6 h-6" />
          <span className="text-[10px] font-medium">Menu</span>
        </Link>
      </div>
    </div>
  );
}
