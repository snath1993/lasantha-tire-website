'use client';

import { useEffect, useState, Suspense } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { Loader2, AlertCircle, CheckCircle } from 'lucide-react';
import { BookingProvider, useBooking } from '@/contexts/BookingContext';
import SmartBookingPopup from '@/components/mobile/SmartBookingPopup';

function BookingPageContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const { openBookingPopup } = useBooking();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const refCode = searchParams.get('ref');
    
    if (!refCode) {
      setError('No reference code provided');
      setLoading(false);
      return;
    }

    // Fetch quotation details
    const fetchQuotation = async () => {
      try {
        const botApiUrl = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';
        const response = await fetch(`${botApiUrl}/api/quotations/${refCode}`);
        
        if (!response.ok) {
          throw new Error('Quotation not found');
        }

        const data = await response.json();
        
        if (data.ok && data.quotation) {
          const quotation = data.quotation;
          
          // Parse items from JSON string if needed
          let items = [];
          try {
            items = typeof quotation.Items === 'string' 
              ? JSON.parse(quotation.Items) 
              : quotation.Items || [];
          } catch (e) {
            console.error('Failed to parse items:', e);
            items = [];
          }

          // Map items to the expected format
          const bookingItems = items.map((item: any) => ({
            ItemId: item.itemId || '',
            Description: item.description || item.Description || '',
            Brand: item.brand || item.Brand || '',
            Price: item.price || item.Price || 0,
            SellingPrice: item.price || item.SellingPrice || 0,
            Quantity: item.quantity || item.Quantity || 1,
          }));

          const customerInfo = {
            name: quotation.CustomerName || '',
            phone: quotation.CustomerPhone || '',
            vehicleNumber: quotation.VehicleNumber || '',
          };

          // Open booking popup with the quotation data
          openBookingPopup(bookingItems, refCode, customerInfo);
          setLoading(false);
        } else {
          throw new Error('Invalid quotation data');
        }
      } catch (err: any) {
        console.error('Error fetching quotation:', err);
        setError(err.message || 'Failed to load quotation');
        setLoading(false);
      }
    };

    fetchQuotation();
  }, [searchParams, openBookingPopup]);

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-blue-500 animate-spin mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-white mb-2">Loading your booking...</h2>
          <p className="text-slate-400">Please wait a moment</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 flex items-center justify-center p-4">
        <div className="bg-slate-900/80 backdrop-blur-xl rounded-2xl p-8 max-w-md w-full border border-slate-700">
          <div className="text-center">
            <div className="w-16 h-16 bg-red-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
              <AlertCircle className="w-10 h-10 text-red-400" />
            </div>
            <h2 className="text-2xl font-bold text-white mb-2">Booking Error</h2>
            <p className="text-slate-400 mb-6">{error}</p>
            <div className="space-y-3">
              <button
                onClick={() => router.push('/')}
                className="w-full px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold transition-colors"
              >
                Go to Home
              </button>
              <a
                href="https://wa.me/94721222509"
                target="_blank"
                rel="noopener noreferrer"
                className="block w-full px-6 py-3 bg-green-600 hover:bg-green-700 text-white rounded-xl font-bold transition-colors text-center"
              >
                Contact via WhatsApp
              </a>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return <SmartBookingPopup />;
}

export default function BookingPage() {
  return (
    <BookingProvider>
      <Suspense fallback={
        <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 flex items-center justify-center">
          <Loader2 className="w-16 h-16 text-blue-500 animate-spin" />
        </div>
      }>
        <BookingPageContent />
      </Suspense>
    </BookingProvider>
  );
}
