import { Sidebar } from "@/components/layout/Sidebar";
import { Header } from "@/components/layout/Header";
import MobileNav from "@/components/mobile/MobileNav";
import { ModalProvider } from "@/contexts/ModalContext";
import { Suspense } from "react";
import { ToastProvider } from "@/contexts/ToastContext";
import { ThemeProvider } from "@/contexts/ThemeContext";

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ThemeProvider>
      <ToastProvider>
        <ModalProvider>
          <div className="flex min-h-screen relative bg-slate-900">
        <Sidebar />
        
        {/* Main Content - Adjusted for new sidebar */}
        <div className="flex-1 relative z-10 flex flex-col min-h-screen transition-all duration-300 md:ml-[260px]">
          <Header />
          <main className="flex-1 p-4 md:p-8 mt-20 md:mt-6 animate-in fade-in slide-in-from-bottom-4 duration-500 max-w-[100vw] overflow-x-hidden">
            <div className="max-w-6xl mx-auto w-full">
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
