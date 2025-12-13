import { Sidebar } from "@/components/layout/Sidebar";
import { Header } from "@/components/layout/Header";
import { ModalProvider } from "@/contexts/ModalContext";
import { ToastProvider } from "@/contexts/ToastContext";
import { ThemeProvider } from "@/contexts/ThemeContext";
import SmartBookingPopup from "@/components/mobile/SmartBookingPopup";

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
        
        <div className="flex-1 md:pl-[328px] relative z-10 flex flex-col min-h-screen transition-all duration-300">
          <Header />
          <main className="flex-1 p-6 md:p-8 mt-24 animate-in fade-in slide-in-from-bottom-4 duration-500">
            <div className="max-w-7xl mx-auto">
              {children}
            </div>
          </main>
        </div>
        <SmartBookingPopup />
      </div>
        </ModalProvider>
      </ToastProvider>
    </ThemeProvider>
  );
}
