'use client';

import { useState } from 'react';
import { 
  Phone, FileText, CheckCircle, XCircle, 
  Clock, Car, Calendar, ExternalLink, Loader2, Shield, Lock
} from 'lucide-react';

const BOT_API_URL = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';

interface Quotation {
  Id: number;
  RefCode: string;
  QuotationNumber: string;
  CustomerName: string;
  VehicleNumber: string;
  TyreSize: string;
  TotalAmount: number;
  CreatedAt: string;
  ExpiryDate: string;
  IsBooked: boolean;
  IsExpired: boolean;
  BookingReference: string;
  Source: string;
}

interface Vehicle {
  VehicleNumber: string;
  VehicleType: string;
  LastServiceDate: string;
  TotalQuotations: number;
}

export default function CustomerPortal() {
  // Auth State
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [authStep, setAuthStep] = useState<'phone' | 'otp'>('phone');
  const [phone, setPhone] = useState('');
  const [otp, setOtp] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [sessionToken, setSessionToken] = useState('');
  
  // Data State
  const [quotations, setQuotations] = useState<Quotation[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  
  // UI State
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const sendOTP = async () => {
    if (!phone || phone.length < 9) {
      setError('Please enter a valid phone number');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${BOT_API_URL}/api/auth/send-otp`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ phone })
      });

      const data = await response.json();

      if (data.ok) {
        setOtpSent(true);
        setAuthStep('otp');
      } else {
        setError(data.error || 'Failed to send OTP');
      }
    } catch (error) {
      console.error('Send OTP error:', error);
      setError('Connection error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const verifyOTP = async () => {
    if (!otp || otp.length !== 6) {
      setError('Please enter the 6-digit OTP');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${BOT_API_URL}/api/auth/verify-otp`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ phone, otp })
      });

      const data = await response.json();

      if (data.ok) {
        setSessionToken(data.sessionToken);
        setIsAuthenticated(true);
        fetchCustomerData(data.phone);
      } else {
        setError(data.error || 'Invalid OTP');
      }
    } catch (error) {
      console.error('Verify OTP error:', error);
      setError('Connection error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const fetchCustomerData = async (customerPhone: string) => {
    setLoading(true);
    try {
      // Fetch quotations
      const quotationsRes = await fetch(`${BOT_API_URL}/api/quotations/customer/${customerPhone}`);
      const quotationsData = await quotationsRes.json();
      
      if (quotationsData.ok) {
        setQuotations(quotationsData.quotations);
      }

      // Fetch vehicles
      const vehiclesRes = await fetch(`${BOT_API_URL}/api/customers/vehicles/${customerPhone}`);
      const vehiclesData = await vehiclesRes.json();
      
      if (vehiclesData.ok) {
        setVehicles(vehiclesData.vehicles);
      }
    } catch (error) {
      console.error('Failed to fetch customer data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    setAuthStep('phone');
    setPhone('');
    setOtp('');
    setOtpSent(false);
    setSessionToken('');
    setQuotations([]);
    setVehicles([]);
    setError('');
  };

  const getStatusBadge = (quotation: Quotation) => {
    if (quotation.IsBooked) {
      return (
        <span className="flex items-center gap-1 text-xs bg-green-500/20 text-green-400 px-2 py-1 rounded-full">
          <CheckCircle className="w-3 h-3" />
          Booked
        </span>
      );
    }
    if (quotation.IsExpired) {
      return (
        <span className="flex items-center gap-1 text-xs bg-red-500/20 text-red-400 px-2 py-1 rounded-full">
          <XCircle className="w-3 h-3" />
          Expired
        </span>
      );
    }
    return (
      <span className="flex items-center gap-1 text-xs bg-orange-500/20 text-orange-400 px-2 py-1 rounded-full">
        <Clock className="w-3 h-3" />
        Pending
      </span>
    );
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-4 pb-24">
      {/* Header */}
      <div className="mb-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-white mb-2">Customer Portal</h1>
            <p className="text-sm text-slate-400">Secure access to your quotations</p>
          </div>
          {isAuthenticated && (
            <button
              onClick={handleLogout}
              className="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white text-sm rounded-lg transition-all"
            >
              Logout
            </button>
          )}
        </div>
      </div>

      {/* Authentication Flow */}
      {!isAuthenticated && (
        <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6 mb-6">
          <div className="flex items-center gap-3 mb-6">
            <div className="p-3 bg-blue-500/20 rounded-xl">
              <Shield className="w-6 h-6 text-blue-400" />
            </div>
            <div>
              <h3 className="text-lg font-bold text-white">Secure Login</h3>
              <p className="text-xs text-slate-400">Verify via WhatsApp OTP</p>
            </div>
          </div>

          {/* Phone Number Step */}
          {authStep === 'phone' && (
            <div className="space-y-4">
              <div>
                <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
                  Phone Number
                </label>
                <div className="relative">
                  <Phone className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
                  <input
                    type="tel"
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && sendOTP()}
                    placeholder="07XXXXXXXX"
                    className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 pl-10 pr-4 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none"
                  />
                </div>
              </div>

              {error && (
                <div className="bg-red-500/10 border border-red-500/30 rounded-lg p-3">
                  <p className="text-sm text-red-400">{error}</p>
                </div>
              )}

              <button
                onClick={sendOTP}
                disabled={loading || !phone}
                className="w-full py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-700 text-white rounded-xl font-bold flex items-center justify-center gap-2 transition-all active:scale-95 disabled:scale-100"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Sending OTP...
                  </>
                ) : (
                  <>
                    <Phone className="w-5 h-5" />
                    Send OTP via WhatsApp
                  </>
                )}
              </button>

              <div className="flex items-center gap-2 text-xs text-slate-500">
                <Lock className="w-4 h-4" />
                <span>Your data is secure and encrypted</span>
              </div>
            </div>
          )}

          {/* OTP Verification Step */}
          {authStep === 'otp' && (
            <div className="space-y-4">
              <div className="bg-green-500/10 border border-green-500/30 rounded-lg p-3 mb-4">
                <p className="text-sm text-green-400">
                  📱 OTP sent to your WhatsApp: {phone}
                </p>
              </div>

              <div>
                <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
                  Enter 6-Digit OTP
                </label>
                <input
                  type="text"
                  value={otp}
                  onChange={(e) => setOtp(e.target.value.replace(/\D/g, '').slice(0, 6))}
                  onKeyPress={(e) => e.key === 'Enter' && verifyOTP()}
                  placeholder="000000"
                  className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-4 px-4 text-white text-center text-2xl font-mono placeholder:text-slate-600 focus:border-blue-500 focus:outline-none tracking-widest"
                  maxLength={6}
                />
              </div>

              {error && (
                <div className="bg-red-500/10 border border-red-500/30 rounded-lg p-3">
                  <p className="text-sm text-red-400">{error}</p>
                </div>
              )}

              <button
                onClick={verifyOTP}
                disabled={loading || otp.length !== 6}
                className="w-full py-3 bg-green-600 hover:bg-green-700 disabled:bg-slate-700 text-white rounded-xl font-bold flex items-center justify-center gap-2 transition-all active:scale-95 disabled:scale-100"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Verifying...
                  </>
                ) : (
                  <>
                    <CheckCircle className="w-5 h-5" />
                    Verify & Login
                  </>
                )}
              </button>

              <button
                onClick={() => {
                  setAuthStep('phone');
                  setOtp('');
                  setError('');
                }}
                className="w-full py-2 text-sm text-slate-400 hover:text-white transition-all"
              >
                ← Change Phone Number
              </button>

              <button
                onClick={sendOTP}
                disabled={loading}
                className="w-full py-2 text-sm text-blue-400 hover:text-blue-300 transition-all"
              >
                Resend OTP
              </button>
            </div>
          )}
        </div>
      )}

      {/* Customer Data - Only show when authenticated */}
      {isAuthenticated && !loading && (
        <>
          {/* Vehicles Section */}
          {vehicles.length > 0 && (
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 mb-6">
              <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
                <Car className="w-5 h-5 text-blue-400" />
                Your Vehicles ({vehicles.length})
              </h3>
              <div className="space-y-3">
                {vehicles.map((vehicle, idx) => (
                  <div key={idx} className="bg-slate-700/50 rounded-xl p-4">
                    <div className="flex items-center justify-between mb-2">
                      <span className="text-lg font-bold text-white font-mono">{vehicle.VehicleNumber}</span>
                      <span className="text-xs bg-blue-500/20 text-blue-400 px-2 py-1 rounded-full">
                        {vehicle.VehicleType}
                      </span>
                    </div>
                    <div className="flex items-center gap-4 text-xs text-slate-400">
                      <span>{vehicle.TotalQuotations} quotations</span>
                      <span>•</span>
                      <span>Last service: {new Date(vehicle.LastServiceDate).toLocaleDateString()}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Quotations Section */}
          {quotations.length > 0 ? (
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5">
              <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
                <FileText className="w-5 h-5 text-green-400" />
                Your Quotations ({quotations.length})
              </h3>
              <div className="space-y-3">
                {quotations.map((quotation) => (
                  <div key={quotation.Id} className="bg-slate-700/50 rounded-xl p-4">
                    <div className="flex items-start justify-between mb-3">
                      <div>
                        <p className="text-sm font-mono font-bold text-blue-400">
                          {quotation.QuotationNumber || quotation.RefCode}
                        </p>
                        <p className="text-xs text-slate-500 mt-1">
                          {new Date(quotation.CreatedAt).toLocaleDateString('en-US', { 
                            month: 'short', 
                            day: 'numeric', 
                            year: 'numeric' 
                          })}
                        </p>
                      </div>
                      {getStatusBadge(quotation)}
                    </div>

                    <div className="space-y-2 mb-3">
                      {quotation.VehicleNumber && (
                        <div className="flex items-center gap-2 text-sm text-slate-300">
                          <Car className="w-4 h-4 text-slate-500" />
                          <span>{quotation.VehicleNumber}</span>
                        </div>
                      )}
                      {quotation.TyreSize && (
                        <div className="flex items-center gap-2 text-sm text-slate-300">
                          <span className="text-slate-500">Size:</span>
                          <span>{quotation.TyreSize}</span>
                        </div>
                      )}
                      <div className="flex items-center gap-2 text-sm">
                        <span className="text-slate-500">Amount:</span>
                        <span className="text-lg font-bold text-white">Rs {quotation.TotalAmount.toLocaleString()}</span>
                      </div>
                    </div>

                    {quotation.IsBooked && quotation.BookingReference && (
                      <div className="bg-green-500/10 border border-green-500/30 rounded-lg p-3 mb-3">
                        <p className="text-xs text-green-400 mb-1">Booking Reference</p>
                        <p className="text-sm font-mono font-bold text-green-300">{quotation.BookingReference}</p>
                      </div>
                    )}

                    {!quotation.IsBooked && !quotation.IsExpired && (
                      <div className="flex gap-2">
                        <a
                          href={`https://lasanthatyre.com/book?ref=${quotation.QuotationNumber || quotation.RefCode}`}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="flex-1 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold rounded-lg flex items-center justify-center gap-2 transition-all"
                        >
                          <Calendar className="w-4 h-4" />
                          Book Appointment
                        </a>
                        <button className="px-4 py-2 bg-slate-600 hover:bg-slate-500 text-white text-sm font-semibold rounded-lg flex items-center gap-2 transition-all">
                          <ExternalLink className="w-4 h-4" />
                          View
                        </button>
                      </div>
                    )}

                    {quotation.ExpiryDate && !quotation.IsExpired && (
                      <p className="text-xs text-orange-400 mt-3">
                        ⏰ Valid until {new Date(quotation.ExpiryDate).toLocaleDateString()}
                      </p>
                    )}
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8 text-center">
              <FileText className="w-16 h-16 text-slate-600 mx-auto mb-4" />
              <h3 className="text-lg font-bold text-white mb-2">No Quotations Found</h3>
              <p className="text-sm text-slate-400">
                No quotations found for this phone number
              </p>
            </div>
          )}
        </>
      )}

      {/* Loading State */}
      {isAuthenticated && loading && (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="w-12 h-12 text-blue-400 animate-spin" />
        </div>
      )}
    </div>
  );
}
