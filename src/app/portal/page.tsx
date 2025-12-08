'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Phone, Lock, ArrowRight, Loader2, ShieldCheck, Car, FileText, Calendar, Clock, CheckCircle, XCircle, LogOut, History, AlertCircle } from 'lucide-react';
import Link from 'next/link';
import { getBotApiUrl } from '@/utils/getBotApiUrl';

// Bot API URL - Use environment variable or default to localhost
const BOT_API_URL = getBotApiUrl();

interface Quotation {
  QuotationNumber: string;
  RefCode: string;
  TotalAmount: number;
  CreatedAt: string;
  ExpiryDate: string;
  Status: string;
  Items: string; // JSON string
  VehicleNumber: string;
  IsBooked: boolean;
  IsExpired: boolean;
}

interface Vehicle {
  VehicleNumber: string;
  VehicleType: string;
  LastServiceDate: string;
  TotalQuotations: number;
}

export default function CustomerPortal() {
  // Auth State
  const [step, setStep] = useState<'phone' | 'otp' | 'dashboard'>('phone');
  const [phone, setPhone] = useState('');
  const [otp, setOtp] = useState('');
  const [sessionToken, setSessionToken] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Data State
  const [quotations, setQuotations] = useState<Quotation[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [activeTab, setActiveTab] = useState<'quotations' | 'vehicles'>('quotations');

  // Check for existing session on mount
  useEffect(() => {
    const savedToken = localStorage.getItem('portal_token');
    const savedPhone = localStorage.getItem('portal_phone');
    if (savedToken && savedPhone) {
      setSessionToken(savedToken);
      setPhone(savedPhone);
      setStep('dashboard');
      fetchCustomerData(savedPhone, savedToken);
    }
  }, []);

  const handleSendOTP = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Basic validation
      const cleanPhone = phone.replace(/\D/g, '');
      if (cleanPhone.length < 9) {
        throw new Error('Please enter a valid phone number');
      }

      const response = await fetch(`${BOT_API_URL}/api/auth/send-otp`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ phone: cleanPhone })
      });

      const data = await response.json();

      if (data.ok) {
        setStep('otp');
      } else {
        throw new Error(data.error || 'Failed to send OTP');
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOTP = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const cleanPhone = phone.replace(/\D/g, '');
      const response = await fetch(`${BOT_API_URL}/api/auth/verify-otp`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ phone: cleanPhone, otp })
      });

      const data = await response.json();

      if (data.ok) {
        const token = data.sessionToken;
        setSessionToken(token);
        localStorage.setItem('portal_token', token);
        localStorage.setItem('portal_phone', cleanPhone);
        setStep('dashboard');
        fetchCustomerData(cleanPhone, token);
      } else {
        throw new Error(data.error || 'Invalid OTP');
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchCustomerData = async (phoneNumber: string, token: string) => {
    try {
      // Fetch Quotations
      const quotesRes = await fetch(`${BOT_API_URL}/api/quotations/customer/${phoneNumber}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const quotesData = await quotesRes.json();
      if (quotesData.ok) {
        setQuotations(quotesData.quotations);
      }

      // Fetch Vehicles
      const vehiclesRes = await fetch(`${BOT_API_URL}/api/customers/vehicles/${phoneNumber}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const vehiclesData = await vehiclesRes.json();
      if (vehiclesData.ok) {
        setVehicles(vehiclesData.vehicles);
      }
    } catch (err) {
      console.error('Failed to fetch data', err);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('portal_token');
    localStorage.removeItem('portal_phone');
    setSessionToken('');
    setPhone('');
    setOtp('');
    setQuotations([]);
    setVehicles([]);
    setStep('phone');
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0
    }).format(amount);
  };

  return (
    <div className="min-h-screen bg-slate-50 pt-24 pb-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-4xl mx-auto">
        
        {/* Header */}
        <div className="text-center mb-12">
          <h1 className="text-3xl font-bold text-slate-900 sm:text-4xl mb-4">
            Customer Portal
          </h1>
          <p className="text-lg text-slate-600">
            Manage your quotations, vehicles, and bookings in one place.
          </p>
        </div>

        {/* Auth Container */}
        <div className="bg-white rounded-2xl shadow-xl overflow-hidden border border-slate-100">
          
          {/* Step 1: Phone Input */}
          {step === 'phone' && (
            <motion.div 
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              className="p-8 sm:p-12 max-w-md mx-auto"
            >
              <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-6">
                <Phone className="w-8 h-8 text-blue-600" />
              </div>
              <h2 className="text-2xl font-bold text-center text-slate-900 mb-2">Welcome Back</h2>
              <p className="text-center text-slate-500 mb-8">Enter your phone number to access your account via WhatsApp OTP.</p>

              <form onSubmit={handleSendOTP} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-2">Phone Number</label>
                  <div className="relative">
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <span className="text-slate-500 font-medium">+94</span>
                    </div>
                    <input
                      type="tel"
                      value={phone}
                      onChange={(e) => setPhone(e.target.value)}
                      className="block w-full pl-12 pr-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                      placeholder="77 123 4567"
                      required
                    />
                  </div>
                </div>

                {error && (
                  <div className="p-3 bg-red-50 text-red-600 text-sm rounded-lg flex items-center gap-2">
                    <AlertCircle className="w-4 h-4" />
                    {error}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={loading}
                  className="w-full flex items-center justify-center gap-2 py-3 px-4 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : <ArrowRight className="w-5 h-5" />}
                  Send OTP via WhatsApp
                </button>
              </form>
            </motion.div>
          )}

          {/* Step 2: OTP Input */}
          {step === 'otp' && (
            <motion.div 
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              className="p-8 sm:p-12 max-w-md mx-auto"
            >
              <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
                <Lock className="w-8 h-8 text-green-600" />
              </div>
              <h2 className="text-2xl font-bold text-center text-slate-900 mb-2">Verify OTP</h2>
              <p className="text-center text-slate-500 mb-8">
                Enter the 6-digit code sent to your WhatsApp number ending in 
                <span className="font-bold text-slate-900"> {phone.slice(-4)}</span>
              </p>

              <form onSubmit={handleVerifyOTP} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-2">One-Time Password</label>
                  <input
                    type="text"
                    value={otp}
                    onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
                    className="block w-full text-center text-2xl tracking-widest py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors font-mono"
                    placeholder="000000"
                    required
                  />
                </div>

                {error && (
                  <div className="p-3 bg-red-50 text-red-600 text-sm rounded-lg flex items-center gap-2">
                    <AlertCircle className="w-4 h-4" />
                    {error}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={loading || otp.length !== 6}
                  className="w-full flex items-center justify-center gap-2 py-3 px-4 bg-green-600 hover:bg-green-700 text-white font-bold rounded-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : <ShieldCheck className="w-5 h-5" />}
                  Verify & Login
                </button>

                <button
                  type="button"
                  onClick={() => setStep('phone')}
                  className="w-full text-sm text-slate-500 hover:text-slate-700 font-medium"
                >
                  Change Phone Number
                </button>
              </form>
            </motion.div>
          )}

          {/* Step 3: Dashboard */}
          {step === 'dashboard' && (
            <motion.div 
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              className="flex flex-col min-h-[600px]"
            >
              {/* Dashboard Header */}
              <div className="bg-slate-900 text-white p-6 sm:p-8 flex flex-col sm:flex-row justify-between items-center gap-4">
                <div>
                  <h2 className="text-2xl font-bold mb-1">My Account</h2>
                  <p className="text-slate-400 flex items-center gap-2">
                    <Phone className="w-4 h-4" /> +94 {phone}
                  </p>
                </div>
                <button
                  onClick={handleLogout}
                  className="flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 rounded-lg transition-colors text-sm font-medium"
                >
                  <LogOut className="w-4 h-4" />
                  Sign Out
                </button>
              </div>

              {/* Navigation Tabs */}
              <div className="flex border-b border-slate-200">
                <button
                  onClick={() => setActiveTab('quotations')}
                  className={`flex-1 py-4 text-sm font-bold text-center border-b-2 transition-colors flex items-center justify-center gap-2 ${
                    activeTab === 'quotations'
                      ? 'border-blue-600 text-blue-600'
                      : 'border-transparent text-slate-500 hover:text-slate-700'
                  }`}
                >
                  <FileText className="w-4 h-4" />
                  Quotations
                </button>
                <button
                  onClick={() => setActiveTab('vehicles')}
                  className={`flex-1 py-4 text-sm font-bold text-center border-b-2 transition-colors flex items-center justify-center gap-2 ${
                    activeTab === 'vehicles'
                      ? 'border-blue-600 text-blue-600'
                      : 'border-transparent text-slate-500 hover:text-slate-700'
                  }`}
                >
                  <Car className="w-4 h-4" />
                  My Vehicles
                </button>
              </div>

              {/* Content Area */}
              <div className="p-6 sm:p-8 bg-slate-50 flex-1">
                
                {/* Quotations Tab */}
                {activeTab === 'quotations' && (
                  <div className="space-y-4">
                    {quotations.length === 0 ? (
                      <div className="text-center py-12">
                        <div className="w-16 h-16 bg-slate-200 rounded-full flex items-center justify-center mx-auto mb-4">
                          <History className="w-8 h-8 text-slate-400" />
                        </div>
                        <h3 className="text-lg font-bold text-slate-900">No Quotations Found</h3>
                        <p className="text-slate-500">You haven't requested any quotations yet.</p>
                      </div>
                    ) : (
                      quotations.map((quote) => (
                        <div key={quote.QuotationNumber} className="bg-white rounded-xl p-6 shadow-sm border border-slate-200 hover:shadow-md transition-shadow">
                          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-4">
                            <div>
                              <div className="flex items-center gap-3 mb-1">
                                <h3 className="text-lg font-bold text-slate-900">{quote.QuotationNumber}</h3>
                                <span className={`px-2.5 py-0.5 rounded-full text-xs font-bold ${
                                  quote.IsBooked ? 'bg-green-100 text-green-700' :
                                  quote.IsExpired ? 'bg-red-100 text-red-700' :
                                  'bg-blue-100 text-blue-700'
                                }`}>
                                  {quote.IsBooked ? 'BOOKED' : quote.IsExpired ? 'EXPIRED' : 'ACTIVE'}
                                </span>
                              </div>
                              <p className="text-sm text-slate-500 flex items-center gap-2">
                                <Calendar className="w-3 h-3" />
                                {formatDate(quote.CreatedAt)}
                              </p>
                            </div>
                            <div className="text-right">
                              <p className="text-2xl font-bold text-slate-900">{formatCurrency(quote.TotalAmount)}</p>
                              <p className="text-xs text-slate-500">Total Amount</p>
                            </div>
                          </div>

                          <div className="border-t border-slate-100 pt-4 mt-4">
                            <div className="flex items-center justify-between">
                              <div className="flex items-center gap-2 text-sm text-slate-600">
                                <Car className="w-4 h-4" />
                                {quote.VehicleNumber || 'N/A'}
                              </div>
                              {!quote.IsBooked && !quote.IsExpired && (
                                <Link 
                                  href={`/book?ref=${quote.QuotationNumber}`}
                                  className="text-sm font-bold text-blue-600 hover:text-blue-700 flex items-center gap-1"
                                >
                                  Book Now <ArrowRight className="w-4 h-4" />
                                </Link>
                              )}
                            </div>
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                )}

                {/* Vehicles Tab */}
                {activeTab === 'vehicles' && (
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    {vehicles.length === 0 ? (
                      <div className="col-span-full text-center py-12">
                        <div className="w-16 h-16 bg-slate-200 rounded-full flex items-center justify-center mx-auto mb-4">
                          <Car className="w-8 h-8 text-slate-400" />
                        </div>
                        <h3 className="text-lg font-bold text-slate-900">No Vehicles Found</h3>
                        <p className="text-slate-500">Your vehicle history will appear here.</p>
                      </div>
                    ) : (
                      vehicles.map((vehicle) => (
                        <div key={vehicle.VehicleNumber} className="bg-white rounded-xl p-6 shadow-sm border border-slate-200">
                          <div className="flex items-center gap-4 mb-4">
                            <div className="w-12 h-12 bg-blue-50 rounded-lg flex items-center justify-center">
                              <Car className="w-6 h-6 text-blue-600" />
                            </div>
                            <div>
                              <h3 className="text-lg font-bold text-slate-900">{vehicle.VehicleNumber}</h3>
                              <p className="text-sm text-slate-500">{vehicle.VehicleType || 'Unknown Type'}</p>
                            </div>
                          </div>
                          <div className="space-y-2 text-sm">
                            <div className="flex justify-between text-slate-600">
                              <span>Total Quotations</span>
                              <span className="font-bold text-slate-900">{vehicle.TotalQuotations}</span>
                            </div>
                            <div className="flex justify-between text-slate-600">
                              <span>Last Service</span>
                              <span className="font-bold text-slate-900">
                                {vehicle.LastServiceDate ? formatDate(vehicle.LastServiceDate) : 'Never'}
                              </span>
                            </div>
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                )}

              </div>
            </motion.div>
          )}
        </div>
      </div>
    </div>
  );
}
