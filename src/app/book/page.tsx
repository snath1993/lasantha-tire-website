'use client';

import { useState, useEffect, Suspense } from 'react';
import { useSearchParams } from 'next/navigation';
import { motion, AnimatePresence } from 'framer-motion';
import { Clock, User, Phone, Check, AlertCircle, ChevronDown, ChevronRight, Loader2 } from 'lucide-react';

// Types
interface QuotationItem {
  description: string;
  brand?: string;
  size?: string;
  price?: number;
  unitPrice?: number;
  quantity: number;
  total?: number;
  category?: string;
}

interface QuotationData {
  RefID: number;
  ItemsJson: QuotationItem[];
  CustomerName: string;
  CustomerPhone: string;
}

function BookingContent() {
  const searchParams = useSearchParams();
  const refId = searchParams.get('ref');
  
  const [loading, setLoading] = useState(true);
  const [quotation, setQuotation] = useState<QuotationData | null>(null);
  const [error, setError] = useState('');
  
  // Form State
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [date, setDate] = useState('');
  const [time, setTime] = useState('');
  const [selectedItemIndex, setSelectedItemIndex] = useState<number | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  // Fetch Quotation Data
  useEffect(() => {
    if (!refId) {
      setLoading(false);
      return;
    }

    const fetchQuotation = async () => {
      try {
        // In a real scenario, this would call your API
        // For now, we'll simulate fetching from the Bot API we just set up
        // Since this is client-side, we need to call the Next.js API route which proxies to the Bot
        // But wait, we haven't set up the Next.js proxy yet. 
        // Let's assume we can fetch directly or mock it for this step.
        
        // Mocking for UI development first (Royal Experience)
        // In production, replace with: const res = await fetch(`/api/quotations/${refId}`);
        
        // Simulating API call
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Mock Data based on Ref ID
        // This will be replaced by real API call in next step
        /*
        setQuotation({
          RefID: parseInt(refId),
          ItemsJson: [
            { description: '175/70R13 Dunlop SP Touring', brand: 'Dunlop', size: '175/70R13', price: 15000, quantity: 2 },
            { description: '175/70R13 CEAT Milaze', brand: 'CEAT', size: '175/70R13', price: 12000, quantity: 2 }
          ],
          CustomerName: '',
          CustomerPhone: ''
        });
        */
       
       // Real Fetch attempt via Next.js Proxy
       const res = await fetch(`/api/quotation/${refId}`);
       if (res.ok) {
         const data = await res.json();
         if (data.ok) {
            setQuotation(data.data);
            if (data.data.CustomerName) setName(data.data.CustomerName);
            if (data.data.CustomerPhone) setPhone(data.data.CustomerPhone);
            // Auto-select first item if only one exists
            if (data.data.ItemsJson.length === 1) setSelectedItemIndex(0);
         } else {
            setError('Quotation not found');
         }
       } else {
         // Fallback for demo if API not reachable yet
         setError('Unable to load quotation details. Please fill manually.');
       }

      } catch (err) {
        console.error(err);
        setError('Failed to load quotation');
      } finally {
        setLoading(false);
      }
    };

    fetchQuotation();
  }, [refId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validation
    if (!name || !phone || !date || !time) {
      setError('Please fill all required fields');
      return;
    }
    
    if (quotation && quotation.ItemsJson.length > 1 && selectedItemIndex === null) {
      setError('Please select an item from the quotation');
      return;
    }
    
    setIsSubmitting(true);
    setError('');

    try {
      const response = await fetch('/api/appointments/book', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          refId: quotation?.RefID || refId,
          selectedItemIndex,
          name,
          phone,
          date,
          time,
          notes: ''
        })
      });

      const data = await response.json();
      
      if (data.success) {
        setIsSuccess(true);
      } else {
        setError(data.error || 'Failed to book appointment');
        setIsSubmitting(false);
      }
    } catch (err) {
      console.error('Booking error:', err);
      setError('Network error. Please try again.');
      setIsSubmitting(false);
    }
  };

  // Time Slots Logic
  const timeSlots = [
    '07:00 AM', '07:30 AM', '08:00 AM', '08:30 AM', '09:00 AM', '09:30 AM',
    '10:00 AM', '10:30 AM', '11:00 AM', '11:30 AM', '12:00 PM', '12:30 PM',
    '01:00 PM', '01:30 PM', '02:00 PM', '02:30 PM', '03:00 PM', '03:30 PM',
    '04:00 PM', '04:30 PM', '05:00 PM', '05:30 PM', '06:00 PM', '06:30 PM',
    '07:00 PM', '07:30 PM', '08:00 PM'
  ];

  const isAlignmentTime = (t: string) => {
    // Parse time string (e.g., "07:30 AM")
    const [timePart, modifier] = t.split(' ');
    const [hoursRaw, minutes] = timePart.split(':').map(Number);
    let hours = hoursRaw;
    
    if (hours === 12) {
      hours = modifier === 'PM' ? 12 : 0;
    } else if (modifier === 'PM') {
      hours += 12;
    }

    const totalMinutes = hours * 60 + minutes;
    const startMinutes = 7 * 60 + 30; // 7:30 AM
    const endMinutes = 17 * 60 + 30;  // 5:30 PM

    return totalMinutes >= startMinutes && totalMinutes <= endMinutes;
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <div className="text-center space-y-4">
          <Loader2 className="w-12 h-12 text-blue-500 animate-spin mx-auto" />
          <p className="text-slate-400 animate-pulse">Loading your booking details...</p>
        </div>
      </div>
    );
  }

  if (isSuccess) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center p-4">
        <motion.div 
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="bg-slate-900 border border-slate-800 rounded-3xl p-8 max-w-md w-full text-center space-y-6 shadow-2xl shadow-blue-900/20"
        >
          <div className="w-20 h-20 bg-green-500/20 rounded-full flex items-center justify-center mx-auto mb-6">
            <Check className="w-10 h-10 text-green-500" />
          </div>
          <h1 className="text-3xl font-bold text-white">Booking Confirmed!</h1>
          <p className="text-slate-400">
            Thank you, <span className="text-white font-semibold">{name}</span>. 
            We have reserved your slot for <span className="text-blue-400">{date} at {time}</span>.
          </p>
          <div className="bg-slate-800 rounded-xl p-4 border border-slate-700">
            <p className="text-sm text-slate-500 mb-1">Reference Number</p>
            <p className="text-xl font-mono font-bold text-white">#{refId || 'NEW-BOOKING'}</p>
          </div>
          <p className="text-xs text-slate-500">
            You will receive a WhatsApp confirmation shortly.
          </p>
        </motion.div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white font-sans selection:bg-blue-500/30">
      {/* Header Image / Branding */}
      <div className="h-48 bg-gradient-to-b from-blue-900/40 to-slate-950 relative overflow-hidden">
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-20"></div>
        <div className="absolute bottom-0 left-0 right-0 h-24 bg-gradient-to-t from-slate-950 to-transparent"></div>
        <div className="container mx-auto px-6 h-full flex flex-col justify-center relative z-10">
          <h1 className="text-3xl md:text-4xl font-bold text-white mb-2">Lasantha Tyre Traders</h1>
          <p className="text-blue-400 font-medium">Premium Service Booking</p>
        </div>
      </div>

      <div className="container mx-auto px-4 -mt-10 relative z-20 pb-20">
        <motion.div 
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          className="max-w-2xl mx-auto bg-slate-900/80 backdrop-blur-xl border border-slate-800 rounded-3xl shadow-2xl overflow-hidden"
        >
          {/* Quotation Summary Card */}
          {quotation && (
            <div className="bg-slate-800/50 p-6 border-b border-slate-700">
              <div className="flex items-center justify-between mb-4">
                <span className="bg-blue-500/10 text-blue-400 px-3 py-1 rounded-full text-xs font-mono border border-blue-500/20">
                  Ref: #{quotation.RefID}
                </span>
                <span className="text-slate-400 text-sm">Valid Quotation</span>
              </div>
              
              <h2 className="text-xl font-bold text-white mb-4">Select Your Package</h2>
              
              <div className="space-y-3">
                {quotation.ItemsJson.map((item, idx) => {
                  const rawPrice = item.price ?? item.unitPrice ?? item.total ?? 0;
                  const priceValue = typeof rawPrice === 'number' ? rawPrice : Number(rawPrice) || 0;
                  const subtitleParts = [item.brand, item.size].filter(Boolean);
                  const quantityLabel = item.quantity ?? 1;
                  const subtitle = subtitleParts.length
                    ? subtitleParts.join(' • ')
                    : `Qty: ${quantityLabel}`;

                  return (
                    <div 
                      key={idx}
                      onClick={() => setSelectedItemIndex(idx)}
                      className={`p-4 rounded-xl border transition-all cursor-pointer relative group ${
                        selectedItemIndex === idx 
                          ? 'bg-blue-600/10 border-blue-500 shadow-lg shadow-blue-900/10' 
                          : 'bg-slate-900 border-slate-700 hover:border-slate-600'
                      }`}
                    >
                      <div className="flex justify-between items-start">
                        <div>
                          <h3 className="font-bold text-white">{item.description}</h3>
                          <p className="text-sm text-slate-400 mt-1">{subtitle}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-lg font-bold text-emerald-400">Rs. {priceValue.toLocaleString()}</p>
                          {selectedItemIndex === idx && (
                            <span className="text-xs text-blue-400 font-medium flex items-center justify-end gap-1 mt-1">
                              <Check className="w-3 h-3" /> Selected
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* Booking Form */}
          <form onSubmit={handleSubmit} className="p-6 space-y-6">
            
            {/* Personal Details */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium text-slate-400 ml-1">Your Name</label>
                <div className="relative">
                  <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
                  <input 
                    type="text" 
                    required
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Enter your name"
                    className="w-full bg-slate-950 border border-slate-800 rounded-xl py-3 pl-12 pr-4 text-white focus:border-blue-500 focus:ring-1 focus:ring-blue-500 outline-none transition-all"
                  />
                </div>
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium text-slate-400 ml-1">Phone Number</label>
                <div className="relative">
                  <Phone className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
                  <input 
                    type="tel" 
                    required
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="077 123 4567"
                    className="w-full bg-slate-950 border border-slate-800 rounded-xl py-3 pl-12 pr-4 text-white focus:border-blue-500 focus:ring-1 focus:ring-blue-500 outline-none transition-all"
                  />
                </div>
              </div>
            </div>

            {/* Date & Time */}
            <div className="space-y-4 pt-2">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <Clock className="w-5 h-5 text-blue-400" />
                Select Time Slot
              </h3>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Date</label>
                  <input 
                    type="date" 
                    required
                    value={date}
                    onChange={(e) => setDate(e.target.value)}
                    className="w-full bg-slate-950 border border-slate-800 rounded-xl py-3 px-4 text-white focus:border-blue-500 outline-none [color-scheme:dark]"
                  />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Time</label>
                  <div className="relative">
                    <select 
                      required
                      value={time}
                      onChange={(e) => setTime(e.target.value)}
                      className="w-full bg-slate-950 border border-slate-800 rounded-xl py-3 px-4 text-white focus:border-blue-500 outline-none appearance-none cursor-pointer"
                    >
                      <option value="">Select a time</option>
                      {timeSlots.map(t => (
                        <option key={t} value={t}>{t}</option>
                      ))}
                    </select>
                    <ChevronDown className="absolute right-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500 pointer-events-none" />
                  </div>
                </div>
              </div>

              {/* Smart Notification for Alignment */}
              <AnimatePresence>
                {time && !isAlignmentTime(time) && (
                  <motion.div 
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: 'auto', opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    className="overflow-hidden"
                  >
                    <div className="bg-amber-900/20 border border-amber-500/30 rounded-xl p-4 flex gap-3 items-start mt-4">
                      <AlertCircle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
                      <div className="text-sm text-slate-300">
                        <p className="font-semibold text-amber-400 mb-1">Wheel Alignment Hours</p>
                        <p>Our Wheel Alignment service is available between <span className="text-white font-bold">7:30 AM</span> and <span className="text-white font-bold">5:30 PM</span>. You selected {time}.</p>
                        <p className="mt-1 text-xs text-slate-400">You can still book for other services, or please choose a different time for alignment.</p>
                      </div>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>

            {/* Submit Button */}
            {/* Validation Error Display */}
            {error && !loading && (
              <div className="bg-red-900/20 border border-red-500/30 rounded-xl p-4 flex gap-3 items-start">
                <AlertCircle className="w-5 h-5 text-red-400 shrink-0 mt-0.5" />
                <p className="text-sm text-red-300">{error}</p>
              </div>
            )}

            <button 
              type="submit" 
              disabled={isSubmitting || !name || !phone || !date || !time}
              className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 text-white font-bold py-4 rounded-xl shadow-lg shadow-blue-900/20 active:scale-[0.98] transition-all flex items-center justify-center gap-2 disabled:opacity-70 disabled:cursor-not-allowed"
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin" />
                  Confirming...
                </>
              ) : (
                <>
                  Confirm Booking
                  <ChevronRight className="w-5 h-5" />
                </>
              )}
            </button>

            <p className="text-center text-xs text-slate-500">
              By booking, you agree to our service terms. No payment required now.
            </p>

          </form>
        </motion.div>
      </div>
    </div>
  );
}

export default function BookingPage() {
  return (
    <Suspense fallback={
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <Loader2 className="w-12 h-12 text-blue-500 animate-spin" />
      </div>
    }>
      <BookingContent />
    </Suspense>
  );
}
