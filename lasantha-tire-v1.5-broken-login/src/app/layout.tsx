import type { Metadata, Viewport } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import GlobalSplash from "@/components/ui/GlobalSplash";

const inter = Inter({ subsets: ["latin"] });

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 1,
  userScalable: false,
};

export const metadata: Metadata = {
  title: "Lasantha Tire ERP",
  description: "Advanced ERP & Bot Management System",
  manifest: "/manifest.json",
  appleWebApp: {
    capable: true,
    statusBarStyle: "black-translucent",
    title: "Lasantha Tire",
  },
  formatDetection: {
    telephone: false,
  },
  other: {
    "mobile-web-app-capable": "yes",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${inter.className} overflow-x-hidden w-full bg-slate-50`}>
        <GlobalSplash />
        {children}
      </body>
    </html>
  );
}
