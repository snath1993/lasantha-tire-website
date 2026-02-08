import { Bell, Search, User, Menu, Command } from "lucide-react";
import { Button } from "@/components/ui/button";

export function Header() {
  return (
    <header className="fixed top-6 right-6 left-6 md:left-[328px] z-40 flex items-center justify-between pointer-events-none">
      {/* Left Side - Search (Floating Pill) */}
      <div className="pointer-events-auto flex items-center gap-4 w-full max-w-xl">
        
        <div className="relative w-full max-w-md hidden md:block group">
          <div className="absolute inset-0 bg-white rounded-2xl shadow-lg shadow-black/5 transition-shadow duration-300 group-hover:shadow-xl group-hover:shadow-indigo-500/10" />
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-zinc-400 group-focus-within:text-indigo-600 transition-colors" size={20} />
          <input 
            type="text" 
            placeholder="Search..." 
            className="relative w-full pl-12 pr-12 py-3.5 rounded-2xl bg-transparent border-none focus:ring-0 text-sm font-medium text-zinc-700 placeholder:text-zinc-400"
          />
          <div className="absolute right-4 top-1/2 -translate-y-1/2 flex items-center gap-1 px-1.5 py-0.5 rounded border border-zinc-200 bg-zinc-50">
            <Command size={10} className="text-zinc-400" />
            <span className="text-[10px] font-bold text-zinc-400">K</span>
          </div>
        </div>
      </div>

      {/* Right Side - Actions (Floating Pill) */}
      <div className="pointer-events-auto flex items-center gap-3 bg-white p-2 pr-4 rounded-2xl shadow-lg shadow-black/5 border border-zinc-100">
        <Button variant="ghost" size="icon" className="relative text-zinc-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-xl w-10 h-10 transition-colors">
          <Bell size={20} />
          <span className="absolute top-2.5 right-2.5 w-2 h-2 bg-rose-500 rounded-full ring-2 ring-white" />
        </Button>
        
        <div className="h-8 w-px bg-zinc-100 mx-1" />
        
        <div className="flex items-center gap-3 cursor-pointer group">
          <div className="text-right hidden md:block">
            <p className="text-sm font-bold text-zinc-800 group-hover:text-indigo-600 transition-colors">Admin</p>
            <p className="text-[10px] font-medium text-zinc-400">Super User</p>
          </div>
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center text-white shadow-md shadow-indigo-500/20 group-hover:scale-105 transition-transform">
            <User size={20} />
          </div>
        </div>
      </div>
    </header>
  );
}
