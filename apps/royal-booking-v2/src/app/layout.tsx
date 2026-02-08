import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Royal Booking V2.0",
  description: "Advanced Booking System for Lasantha Tire",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className="antialiased bg-gray-50 text-gray-900">
        {children}
      </body>
    </html>
  );
}
