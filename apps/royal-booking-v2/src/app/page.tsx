import { BookingWizard } from "@/components/BookingWizard";
import { Suspense } from "react";

export default function Home() {
  return (
    <main className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Royal Booking</h1>
          <p className="mt-2 text-gray-600">Schedule your service with Lasantha Tire</p>
        </div>
        
        <Suspense fallback={<div className="text-center p-8">Loading booking system...</div>}>
          <BookingWizard />
        </Suspense>
      </div>
    </main>
  );
}
