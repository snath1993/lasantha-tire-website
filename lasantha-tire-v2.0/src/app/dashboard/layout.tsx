import { Sidebar } from '@/views/shared/layout/Sidebar';
import { Header } from '@/views/shared/layout/Header';
import MobileNav from '@/views/mobile/MobileNav';
import { ModalProvider } from '@/core/contexts/ModalContext';
import { Suspense } from "react";
import { ToastProvider } from '@/core/contexts/ToastContext';
import { ThemeProvider } from '@/core/contexts/ThemeContext';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ThemeProvider>
      <ToastProvider>
        <ModalProvider>
          <div className="flex min-h-screen relative bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 dark:from-slate-950 dark:via-slate-900 dark:to-slate-950">
        <Sidebar />
        
        <div className="flex-1 relative z-10 flex flex-col min-h-screen transition-all duration-300">
          <Header />
          <main className="flex-1 p-3 md:p-8 mt-20 md:mt-24 animate-in fade-in slide-in-from-bottom-4 duration-500 max-w-[100vw] overflow-x-hidden">
            <div className="max-w-7xl mx-auto w-full">
              {children}
            </div>
          </main>
        </div>
        <Suspense fallback={null}>
          <MobileNav />
        </Suspense>
      </div>
        </ModalProvider>
      </ToastProvider>
    </ThemeProvider>
  );
}
