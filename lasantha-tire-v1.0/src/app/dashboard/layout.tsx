import { Sidebar } from "@/components/layout/Sidebar";
import { Header } from "@/components/layout/Header";

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex min-h-screen relative bg-[#f3f4f6] dark:bg-[#09090b]">
      <Sidebar />
      
      <div className="flex-1 md:pl-[328px] relative z-10 flex flex-col min-h-screen transition-all duration-300">
        <Header />
        <main className="flex-1 p-6 md:p-8 mt-24 animate-in fade-in slide-in-from-bottom-4 duration-500">
          <div className="max-w-7xl mx-auto">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
