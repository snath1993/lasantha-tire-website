'use client';

import { useState, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import { Activity, MessageSquare, Phone, Power, RefreshCw, QrCode, CheckCircle, XCircle, Clock, TrendingUp, Users, DollarSign, Package, Home, Zap, Download, LogOut, ArrowUpRight } from 'lucide-react';
import Link from 'next/link';
import Image from 'next/image';

interface WhatsAppStatus {
  ready: boolean;
  qr?: string;
  phoneNumber?: string;
  reason?: string;
  auth_failure?: boolean;
  initializing?: boolean;
}

interface BotStats {
  totalMessages: number;
  todayMessages: number;
  activeChats: number;
  responseTime: number;
  uptime: number;
}

interface SalesData {
  today: number;
  week: number;
  month: number;
  topProducts: Array<{ name: string; count: number; revenue: number }>;
}

export default function AnalyticsPage() {
  const router = useRouter();
  const [isChecking, setIsChecking] = useState(true);
  const [whatsappStatus, setWhatsappStatus] = useState<WhatsAppStatus>({ ready: false });
  const [botStats, setBotStats] = useState<BotStats | null>(null);
  const [salesData, setSalesData] = useState<SalesData | null>(null);
  const [loading, setLoading] = useState(true);
  const [connecting, setConnecting] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);
  const [qrImage, setQrImage] = useState<string | null>(null);
  const [qrDataUrl, setQrDataUrl] = useState<string | null>(null);
  const eventSourceRef = useRef<EventSource | null>(null);

  useEffect(() => {
    const verifyAuth = async () => {
      const auth = await checkAuth();
      if (!auth.authenticated) {
        router.push('/');
        return;
      }
      setIsChecking(false);
    };
    verifyAuth();
  }, [router]);

  useEffect(() => {
    if (!isChecking) {
      loadInitialData();
      connectToSSE();
    }

    return () => {
      if (eventSourceRef.current) {
        eventSourceRef.current.close();
      }
    };
  }, [isChecking]);

  // Generate QR code image when qrImage changes
  useEffect(() => {
    if (qrImage && typeof window !== 'undefined') {
      (async () => {
        try {
          const mod: any = await import('qrcode');
          const QRCode = (mod && (mod.default || mod));
          if (QRCode && typeof QRCode.toDataURL === 'function') {
            const url = await QRCode.toDataURL(qrImage, {
              width: 512,
              margin: 2,
              errorCorrectionLevel: 'H',
              color: { dark: '#000000', light: '#FFFFFF' }
            });
            setQrDataUrl(url);
          } else {
            console.error('QR module missing toDataURL');
          }
        } catch (err) {
          console.error('QR generation error:', err);
        }
      })();
    } else {
      setQrDataUrl(null);
    }
  }, [qrImage]);

  const loadInitialData = async () => {
    try {
      // Only fetch in browser environment
      if (typeof window === 'undefined') {
        setLoading(false);
        return;
      }

      // Load WhatsApp status from bot API with retry
      try {
        const controller = new AbortController();
        const timer = setTimeout(() => controller.abort(), 5000);
        const statusRes = await fetch('http://localhost:3100/status', {
          method: 'GET',
          cache: 'no-cache',
          signal: controller.signal
        });
        clearTimeout(timer);
        if (statusRes.ok) {
          const status = await statusRes.json();
          setWhatsappStatus(status);
          if (status.qr) {
            setQrImage(status.qr);
          }
        }
      } catch (err) {
        // Silently handle - SSE will provide updates when bot is ready
        setWhatsappStatus({ ready: false });
      }

      // Load bot stats from dashboard API
      try {
        const statsRes = await fetch('/api/analytics/stats');
        if (statsRes.ok) {
          const stats = await statsRes.json();
          setBotStats(stats);
        }
      } catch (err) {
        // Silently handle - stats will load when available
      }

      // Load sales data from dashboard API
      try {
        const salesRes = await fetch('/api/analytics/sales');
        if (salesRes.ok) {
          const sales = await salesRes.json();
          setSalesData(sales);
        }
      } catch (err) {
        // Silently handle - sales will load when available
      }
    } catch (error) {
      // Silently handle initial load errors
    } finally {
      setLoading(false);
    }
  };

  const connectToSSE = () => {
    // Only connect SSE in browser
    if (typeof window === 'undefined') return;

    try {
      const eventSource = new EventSource('http://localhost:3100/sse');
      eventSourceRef.current = eventSource;

      eventSource.addEventListener('qr', (event) => {
        const data = JSON.parse(event.data);
        setWhatsappStatus({ ready: false, qr: data.qr, initializing: false });
        setQrImage(data.qr);
      });

      eventSource.addEventListener('ready', (event) => {
        setWhatsappStatus({ ready: true, initializing: false });
        setQrImage(null);
        showMessage('success', 'WhatsApp connected successfully!');
        loadInitialData();
      });

      eventSource.addEventListener('disconnected', (event) => {
        const data = JSON.parse(event.data);
        setWhatsappStatus({ ready: false, reason: data.reason, initializing: false });
        showMessage('error', 'WhatsApp disconnected: ' + data.reason);
      });

      eventSource.addEventListener('auth_failure', (event) => {
        setWhatsappStatus({ ready: false, auth_failure: true, initializing: false });
        showMessage('error', 'Authentication failed. Please scan QR code again.');
      });

      eventSource.addEventListener('initializing', () => {
        setWhatsappStatus({ ready: false, initializing: true });
      });

      eventSource.onerror = () => {
        // Silently handle - EventSource automatically reconnects
      };
    } catch (error) {
      // Silently handle SSE connection errors
    }
  };

  const handleConnect = async () => {
    setConnecting(true);
    try {
      const res = await fetch('http://localhost:3100/connect', { method: 'POST' });
      if (res.ok) {
        const data = await res.json();
        if (data.initializing) {
          setWhatsappStatus({ ready: false, initializing: true });
          showMessage('success', 'Initializing WhatsApp connection...');
        } else {
          showMessage('success', data.message || 'Connect request sent');
        }
      } else {
        showMessage('error', 'Failed to start connection');
      }
    } catch (error) {
      showMessage('error', 'Failed to connect to bot API');
    } finally {
      setConnecting(false);
    }
  };

  const handleDisconnect = async () => {
    if (!confirm('Are you sure you want to disconnect WhatsApp? This will log out the session.')) return;

    try {
      const res = await fetch('http://localhost:3100/logout', { method: 'POST' });
      const data = await res.json();
      
      if (res.ok || data.success) {
        setWhatsappStatus({ ready: false });
        setQrImage(null);
        showMessage('success', data.message || 'WhatsApp disconnected successfully');
        // Reload data after disconnect
        setTimeout(() => loadInitialData(), 1000);
      } else {
        showMessage('error', data.error || 'Failed to disconnect');
      }
    } catch (error) {
      console.error('Disconnect error:', error);
      showMessage('error', 'Failed to connect to bot API');
    }
  };

  const downloadQR = () => {
    if (!qrDataUrl) return;

    const link = document.createElement('a');
    link.download = `whatsapp-qr-${Date.now()}.png`;
    link.href = qrDataUrl;
    link.click();
  };

  const showMessage = (type: 'success' | 'error', text: string) => {
    setMessage({ type, text });
    setTimeout(() => setMessage(null), 5000);
  };

  const formatUptime = (seconds: number) => {
    const days = Math.floor(seconds / 86400);
    const hours = Math.floor((seconds % 86400) / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    return `${days}d ${hours}h ${minutes}m`;
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0
    }).format(amount);
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading Analytics...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-white tracking-tight">Analytics Dashboard</h1>
          <p className="text-slate-400 mt-1">Real-time insights & WhatsApp connection status</p>
        </div>
        <div className="flex items-center gap-3">
          <Link
            href="/dashboard"
            className="px-4 py-2 bg-slate-800/50 hover:bg-slate-700/50 text-white rounded-xl transition-colors border border-white/10 text-sm font-medium shadow-sm backdrop-blur-sm"
          >
            Back to Home
          </Link>
        </div>
      </div>

      {/* Message Banner */}
      {message && (
        <div className={`flex items-center gap-3 px-4 py-3 rounded-xl border ${
          message.type === 'success' 
            ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400' 
            : 'bg-rose-500/10 border-rose-500/20 text-rose-400'
        }`}>
          {message.type === 'success' ? <CheckCircle size={18} /> : <XCircle size={18} />}
          <span className="text-sm font-medium">{message.text}</span>
        </div>
      )}

      {/* WhatsApp Status Card */}
      <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-lg shadow-black/20 border border-white/10">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-8">
          <div className="flex items-center gap-4">
            <div className={`w-16 h-16 rounded-2xl flex items-center justify-center shadow-lg ${
              whatsappStatus.ready 
                ? 'bg-emerald-500 text-white shadow-emerald-500/20' 
                : 'bg-rose-500 text-white shadow-rose-500/20'
            }`}>
              <MessageSquare size={28} />
            </div>
            <div>
              <h2 className="text-xl font-bold text-white">WhatsApp Connection</h2>
              <div className="flex items-center gap-2 mt-1">
                <div className={`w-2 h-2 rounded-full ${whatsappStatus.ready ? 'bg-emerald-500' : 'bg-rose-500'}`} />
                <span className={`text-sm font-medium ${whatsappStatus.ready ? 'text-emerald-400' : 'text-rose-400'}`}>
                  {whatsappStatus.ready ? 'Connected' : (whatsappStatus.initializing ? 'Initializing...' : 'Disconnected')}
                </span>
                {whatsappStatus.phoneNumber && (
                  <span className="text-sm text-slate-500">• {whatsappStatus.phoneNumber}</span>
                )}
              </div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            {whatsappStatus.ready ? (
              <button
                onClick={handleDisconnect}
                className="flex items-center gap-2 px-5 py-2.5 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-xl font-medium transition-colors border border-rose-500/20"
              >
                <LogOut size={18} />
                Disconnect
              </button>
            ) : (
              <button
                onClick={handleConnect}
                disabled={connecting}
                className="flex items-center gap-2 px-5 py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl font-medium transition-colors shadow-lg shadow-emerald-600/20 disabled:opacity-70"
              >
                <Power size={18} />
                {connecting ? 'Connecting...' : 'Connect'}
              </button>
            )}
            <button
              onClick={loadInitialData}
              className="p-2.5 bg-slate-700/50 hover:bg-slate-600/50 text-white rounded-xl transition-colors border border-white/10"
            >
              <RefreshCw size={18} />
            </button>
          </div>
        </div>

        {/* QR Code Section */}
        {qrImage && !whatsappStatus.ready && !whatsappStatus.initializing && (
          <div className="mt-8 p-8 bg-slate-900/50 rounded-2xl border border-white/10 flex flex-col items-center text-center">
            <div className="w-12 h-12 bg-white/10 rounded-full flex items-center justify-center shadow-sm mb-4">
              <QrCode className="text-white" size={24} />
            </div>
            <h3 className="text-lg font-bold text-white mb-2">Scan QR Code</h3>
            <p className="text-slate-400 text-sm mb-6 max-w-md">
              Open WhatsApp on your phone, go to Settings → Linked Devices → Link a Device, and scan the code below.
            </p>
            
            {qrDataUrl ? (
              <div className="bg-white p-4 rounded-2xl shadow-sm border border-white/10 mb-6">
                <img src={qrDataUrl} alt="WhatsApp QR Code" className="w-64 h-64" />
              </div>
            ) : (
              <div className="w-64 h-64 bg-slate-800 rounded-2xl flex items-center justify-center mb-6 border border-white/5">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-slate-400"></div>
              </div>
            )}
            
            <button
              onClick={downloadQR}
              disabled={!qrDataUrl}
              className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-blue-400 hover:bg-blue-500/10 rounded-lg transition-colors"
            >
              <Download size={16} />
              Download QR Image
            </button>
          </div>
        )}

        {/* Initializing State */}
        {!whatsappStatus.ready && whatsappStatus.initializing && (
          <div className="mt-8 p-8 bg-blue-500/10 rounded-2xl border border-blue-500/20 flex items-center justify-center">
            <div className="flex flex-col items-center text-center">
              <div className="w-10 h-10 border-2 border-blue-500 border-t-transparent rounded-full animate-spin mb-4" />
              <p className="text-blue-400 font-medium">Starting WhatsApp session...</p>
              <p className="text-blue-400/70 text-sm mt-1">Please wait while we initialize the connection.</p>
            </div>
          </div>
        )}
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-slate-800/50 backdrop-blur-xl p-6 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-blue-500/10 rounded-2xl flex items-center justify-center text-blue-500">
              <MessageSquare size={24} />
            </div>
            <span className="text-xs font-bold bg-blue-500/10 text-blue-400 px-2 py-1 rounded-full">Today</span>
          </div>
          <h3 className="text-3xl font-bold text-white mb-1">{botStats?.todayMessages.toLocaleString() || '0'}</h3>
          <p className="text-sm text-slate-400">Messages Processed</p>
        </div>

        <div className="bg-slate-800/50 backdrop-blur-xl p-6 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-violet-500/10 rounded-2xl flex items-center justify-center text-violet-500">
              <Users size={24} />
            </div>
            <span className="text-xs font-bold bg-violet-500/10 text-violet-400 px-2 py-1 rounded-full">Active</span>
          </div>
          <h3 className="text-3xl font-bold text-white mb-1">{botStats?.activeChats || '0'}</h3>
          <p className="text-sm text-slate-400">Active Conversations</p>
        </div>

        <div className="bg-slate-800/50 backdrop-blur-xl p-6 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-emerald-500/10 rounded-2xl flex items-center justify-center text-emerald-500">
              <DollarSign size={24} />
            </div>
            <span className="text-xs font-bold bg-emerald-500/10 text-emerald-400 px-2 py-1 rounded-full">Revenue</span>
          </div>
          <h3 className="text-3xl font-bold text-white mb-1">{formatCurrency(salesData?.today || 0)}</h3>
          <p className="text-sm text-slate-400">Sales Today</p>
        </div>

        <div className="bg-slate-800/50 backdrop-blur-xl p-6 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-amber-500/10 rounded-2xl flex items-center justify-center text-amber-500">
              <Clock size={24} />
            </div>
            <span className="text-xs font-bold bg-amber-500/10 text-amber-400 px-2 py-1 rounded-full">Uptime</span>
          </div>
          <h3 className="text-3xl font-bold text-white mb-1">{botStats ? formatUptime(botStats.uptime) : '0d 0h'}</h3>
          <p className="text-sm text-slate-400">System Status</p>
        </div>
      </div>

      {/* Bottom Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Top Products */}
        <div className="bg-slate-800/50 backdrop-blur-xl p-8 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-8">
            <div>
              <h2 className="text-xl font-bold text-white">Top Products</h2>
              <p className="text-sm text-slate-400">Best selling items this week</p>
            </div>
            <div className="w-10 h-10 bg-slate-700/50 rounded-xl flex items-center justify-center text-slate-400">
              <Package size={20} />
            </div>
          </div>

          <div className="space-y-4">
            {salesData?.topProducts.slice(0, 5).map((product, idx) => (
              <div key={idx} className="flex items-center justify-between p-4 bg-slate-900/50 border border-white/5 rounded-2xl group hover:bg-blue-500/10 hover:border-blue-500/20 transition-colors">
                <div className="flex items-center gap-4">
                  <span className="text-sm font-bold text-slate-500 w-6">#{idx + 1}</span>
                  <div>
                    <p className="font-semibold text-white group-hover:text-blue-400">{product.name}</p>
                    <p className="text-xs text-slate-500 group-hover:text-blue-400/70">{product.count} units sold</p>
                  </div>
                </div>
                <span className="font-bold text-white">{formatCurrency(product.revenue)}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Sales Overview */}
        <div className="bg-slate-800/50 backdrop-blur-xl p-8 rounded-[2rem] shadow-lg shadow-black/20 border border-white/10">
          <div className="flex items-center justify-between mb-8">
            <div>
              <h2 className="text-xl font-bold text-white">Sales Overview</h2>
              <p className="text-sm text-slate-400">Performance metrics</p>
            </div>
            <div className="w-10 h-10 bg-slate-700/50 rounded-xl flex items-center justify-center text-slate-400">
              <TrendingUp size={20} />
            </div>
          </div>

          <div className="space-y-4">
            <div className="flex items-center justify-between p-4 border-b border-white/5">
              <span className="text-slate-400">Today</span>
              <span className="text-lg font-bold text-white">{formatCurrency(salesData?.today || 0)}</span>
            </div>
            <div className="flex items-center justify-between p-4 border-b border-white/5">
              <span className="text-slate-400">This Week</span>
              <span className="text-lg font-bold text-white">{formatCurrency(salesData?.week || 0)}</span>
            </div>
            <div className="flex items-center justify-between p-4 border-b border-white/5">
              <span className="text-slate-400">This Month</span>
              <span className="text-lg font-bold text-white">{formatCurrency(salesData?.month || 0)}</span>
            </div>
            
            <div className="mt-6 p-4 bg-emerald-500/10 rounded-2xl flex items-center gap-3 border border-emerald-500/20">
              <div className="w-8 h-8 bg-emerald-500/20 rounded-full flex items-center justify-center text-emerald-500">
                <ArrowUpRight size={16} />
              </div>
              <div>
                <p className="text-sm font-bold text-emerald-400">Positive Trend</p>
                <p className="text-xs text-emerald-500/70">Sales are up 15.3% from last month</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
