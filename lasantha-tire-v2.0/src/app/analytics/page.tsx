'use client';

import { useEffect, useMemo, useState } from 'react';
import { useRouter } from 'next/navigation';
import { 
  BarChart3, TrendingUp, DollarSign,
  Package, ArrowLeft, RefreshCw, Download, FilterX,
  ChevronLeft, ChevronRight, Send
} from 'lucide-react';
import { 
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell, Legend
} from 'recharts';
// Removed DayPicker import
import { 
  format, addMonths, subMonths, startOfMonth, endOfMonth, 
  startOfWeek, endOfWeek, eachDayOfInterval, isSameMonth, isSameDay,
  isAfter, startOfToday
} from 'date-fns';

import { authenticatedFetch, checkAuth } from '@/core/lib/client-auth';
import { exportSalesReportToPDF, getSalesReportBase64 } from '@/core/lib/salesReportExport';

type DateRange = {
  from: Date | undefined;
  to?: Date | undefined;
};

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8', '#82ca9d'];

type SalesReportResponse = {
  success: boolean;
  diagnostics?: {
    serverTime?: string;
    maxInvoiceDate?: string | null;
    lineCount?: number;
  };
  filters: {
    range: string;
    start: string;
    end: string;
    category: string | null;
    brand: string | null;
    search: string | null;
  };
  comparison: {
    prevStart: string;
    prevEnd: string;
  };
  summary: {
    TotalSales: number | null;
    TotalQty: number | null;
    TotalCost: number | null;
    TotalProfit: number | null;
    InvoiceCount: number | null;
  } | null;
  prevSummary: {
    TotalSales: number | null;
    TotalQty: number | null;
    TotalCost: number | null;
    TotalProfit: number | null;
    InvoiceCount: number | null;
  } | null;
  categoryBreakdown: Array<{ name: string; sales: number; profit: number; quantity: number }>;
  brandBreakdown: Array<{ name: string; sales: number; profit: number; quantity: number }>;
  paymentBreakdown: Array<{ name: string; sales: number; count: number }>;
  trend: Array<{ date: string; sales: number; profit: number }>;
  topItems: Array<{
    itemId: string;
    name: string;
    category: string;
    brand: string;
    quantity: number;
    sales: number;
    profit: number;
  }>;
};

type SalesReportMeta = {
  success: boolean;
  categories: string[];
  brands: string[];
};

function safeNumber(val: any): number {
  const n = Number(val);
  return Number.isFinite(n) ? n : 0;
}

export default function AnalyticsPage() {
  const router = useRouter();
  const [isCheckingAuth, setIsCheckingAuth] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const [range, setRange] = useState<'today' | 'yesterday' | 'this_month' | 'last_month' | 'custom'>('this_month');
  const [customStartDate, setCustomStartDate] = useState('');
  const [customEndDate, setCustomEndDate] = useState('');

  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const [pendingRange, setPendingRange] = useState<DateRange | undefined>(undefined);
  const [calendarTarget, setCalendarTarget] = useState<'from' | 'to'>('from');
  const [pendingSingleDate, setPendingSingleDate] = useState<Date | undefined>(undefined);

  const [category, setCategory] = useState('');
  const [brand, setBrand] = useState('');
  
  // Applied filters (for manual trigger)
  const [appliedCategory, setAppliedCategory] = useState('');
  const [appliedBrand, setAppliedBrand] = useState('');

  const [meta, setMeta] = useState<SalesReportMeta | null>(null);
  const [loading, setLoading] = useState(true);
  const [exportingPdf, setExportingPdf] = useState(false);
  const [sendingWhatsApp, setSendingWhatsApp] = useState(false);
  const [data, setData] = useState<SalesReportResponse | null>(null);
  const [lastFetchedAt, setLastFetchedAt] = useState<Date | null>(null);
  
  // Custom Calendar State
  const [viewDate, setViewDate] = useState(new Date());

  const calendarDays = useMemo(() => {
    if (!isCalendarOpen) return [];
    const start = startOfWeek(startOfMonth(viewDate));
    const end = endOfWeek(endOfMonth(viewDate));
    return eachDayOfInterval({ start, end });
  }, [viewDate, isCalendarOpen]);

  const filtersKey = useMemo(() => {
    return [range, customStartDate, customEndDate, appliedCategory, appliedBrand].join('|');
  }, [range, customStartDate, customEndDate, appliedCategory, appliedBrand]);

  const formatCurrency = (val: number) => {
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(val);
  };

  const formatPct = (val: number) => {
    if (!Number.isFinite(val)) return '0.0%';
    return `${val.toFixed(1)}%`;
  };

  const formatDateTime = (iso: string) => {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return iso;
    return d.toLocaleString('en-LK', {
      year: 'numeric',
      month: 'short',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const buildUrl = () => {
    const params = new URLSearchParams();
    params.set('range', range);
    if (range === 'custom') {
      if (customStartDate) params.set('startDate', customStartDate);
      if (customEndDate) params.set('endDate', customEndDate);
    }
    if (appliedCategory) params.set('category', appliedCategory);
    if (appliedBrand) params.set('brand', appliedBrand);
    return `/api/sales-report?${params.toString()}`;
  };

  const openCalendarFor = (target: 'from' | 'to') => {
    setRange('custom');
    setCalendarTarget(target);

    const from = customStartDate ? new Date(`${customStartDate}T00:00:00`) : undefined;
    const to = customEndDate ? new Date(`${customEndDate}T00:00:00`) : undefined;
    const nextRange = from || to ? { from, to } : undefined;
    setPendingRange(nextRange);

    const selected = target === 'from' ? from : to;
    setPendingSingleDate(selected);
    setViewDate(selected || new Date());
    setIsCalendarOpen(true);
  };

  const applyCalendar = () => {
    if (pendingRange?.from) setCustomStartDate(format(pendingRange.from, 'yyyy-MM-dd'));
    else setCustomStartDate('');

    if (pendingRange?.to) setCustomEndDate(format(pendingRange.to, 'yyyy-MM-dd'));
    else setCustomEndDate('');

    setIsCalendarOpen(false);
  };

  const fetchMeta = async () => {
    try {
      const res = await authenticatedFetch('/api/sales-report?mode=meta', { cache: 'no-store' });
      if (!res.ok) return;
      const json = (await res.json()) as SalesReportMeta;
      if (json.success) setMeta(json);
    } catch (e) {
      console.error('Failed to fetch sales report meta:', e);
    }
  };

  const fetchData = async () => {
    setLoading(true);
    try {
      const url = buildUrl();
      const res = await authenticatedFetch(url, { cache: 'no-store' });
      const json = (await res.json()) as SalesReportResponse;
      if (res.ok && json?.success) {
        setData(json);
        setLastFetchedAt(new Date());
      }
    } catch (error) {
      console.error('Failed to fetch sales report:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const verify = async () => {
      try {
        const auth = await checkAuth();
        if (!auth.authenticated) {
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }
        setIsAuthenticated(true);
      } finally {
        setIsCheckingAuth(false);
      }
    };
    verify();
  }, [router]);

  useEffect(() => {
    if (!isCalendarOpen) return;
    // Prevent background page scrolling/touch bleed while modal is open (mobile accuracy)
    const prevOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = prevOverflow;
    };
  }, [isCalendarOpen]);

  useEffect(() => {
    if (!isAuthenticated) return;
    fetchMeta();
  }, [isAuthenticated]);

  useEffect(() => {
    if (!isAuthenticated) return;
    // For custom range, don't auto-fetch until both dates are provided
    if (range === 'custom' && (!customStartDate || !customEndDate)) {
      return;
    }

    const timer = setTimeout(() => {
      fetchData();
    }, 250);
    return () => clearTimeout(timer);
  }, [isAuthenticated, filtersKey]);

  useEffect(() => {
    if (!isAuthenticated) return;
    if (range === 'custom' && (!customStartDate || !customEndDate)) return;

    const pollMs = 30000;
    const tick = () => {
      if (document.visibilityState !== 'visible') return;
      fetchData();
    };

    const interval = window.setInterval(tick, pollMs);
    const onFocus = () => tick();
    window.addEventListener('focus', onFocus);

    return () => {
      window.clearInterval(interval);
      window.removeEventListener('focus', onFocus);
    };
  }, [isAuthenticated, range, customStartDate, customEndDate, filtersKey]);

  const kpis = useMemo(() => {
    const summary = data?.summary;
    const prev = data?.prevSummary;
    const totalSales = safeNumber(summary?.TotalSales);
    const totalProfit = safeNumber(summary?.TotalProfit);
    const totalQty = safeNumber(summary?.TotalQty);
    const invoiceCount = safeNumber(summary?.InvoiceCount);
    const prevSales = safeNumber(prev?.TotalSales);
    const prevProfit = safeNumber(prev?.TotalProfit);

    const margin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
    const salesChange = prevSales > 0 ? ((totalSales - prevSales) / prevSales) * 100 : 0;
    const profitChange = prevProfit > 0 ? ((totalProfit - prevProfit) / prevProfit) * 100 : 0;

    const avgInvoice = invoiceCount > 0 ? totalSales / invoiceCount : 0;

    return {
      totalSales,
      totalProfit,
      totalQty,
      invoiceCount,
      avgInvoice,
      margin,
      salesChange,
      profitChange
    };
  }, [data]);

  const topCategory = useMemo(() => {
    const first = data?.categoryBreakdown?.[0];
    if (!first) return null;
    return {
      name: first.name,
      sales: safeNumber(first.sales)
    };
  }, [data]);

  const handleDownloadPdf = async () => {
    if (!data) return;
    setExportingPdf(true);
    try {
      await exportSalesReportToPDF(data);
    } finally {
      setExportingPdf(false);
    }
  };

  const handleSendWhatsApp = async () => {
    if (!data) return;
    const phone = window.prompt('Enter WhatsApp number (e.g., 94771234567):');
    if (!phone) return;

    setSendingWhatsApp(true);
    try {
      const pdfResult = await getSalesReportBase64(data);
      if (!pdfResult.success || !pdfResult.base64) {
        alert('Failed to generate PDF');
        return;
      }

      const res = await authenticatedFetch('/api/bot-proxy/send-media', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          phone,
          caption: `Sales Report (${data.filters.start} - ${data.filters.end})`,
          mediaData: pdfResult.base64,
          mimeType: 'application/pdf',
          filename: pdfResult.fileName
        })
      });

      const json = await res.json();
      if (json.ok) {
        alert('Sent successfully!');
      } else {
        alert('Failed to send: ' + (json.error || 'Unknown error'));
      }
    } catch (e) {
      console.error(e);
      alert('Error sending message');
    } finally {
      setSendingWhatsApp(false);
    }
  };

  const clearFilters = () => {
    setRange('this_month');
    setCustomStartDate('');
    setCustomEndDate('');
    setCategory('');
    setBrand('');
    setAppliedCategory('');
    setAppliedBrand('');
  };

  if (isCheckingAuth) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50">
        <div className="w-8 h-8 border-4 border-violet-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!isAuthenticated) return null;

  return (
    <div className="min-h-screen bg-slate-50 pb-20">
      {/* Header */}
      <div className="bg-white border-b border-slate-200 sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 py-3 sm:py-0 sm:h-16">
            <div className="flex items-start sm:items-center gap-3 sm:gap-4 min-w-0">
              <button 
                onClick={() => router.back()}
                className="p-2 hover:bg-slate-100 rounded-full transition-colors"
              >
                <ArrowLeft className="w-6 h-6 text-slate-600" />
              </button>
              <div>
                <h1 className="text-xl font-bold text-slate-900 flex items-center gap-2">
                  <BarChart3 className="w-6 h-6 text-violet-600" />
                  Sales Intelligence Hub
                </h1>
                <div className="text-xs text-slate-500 mt-0.5 break-words">
                  {data?.filters?.start && data?.filters?.end
                    ? `Range: ${formatDateTime(data.filters.start)} → ${formatDateTime(data.filters.end)}`
                    : 'Range: —'}
                  {lastFetchedAt
                    ? ` • Refreshed: ${lastFetchedAt.toLocaleTimeString('en-LK', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}`
                    : ' • Refreshed: —'}
                  {data?.diagnostics?.maxInvoiceDate
                    ? ` • DB latest invoice: ${formatDateTime(data.diagnostics.maxInvoiceDate)}`
                    : ''}
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2 self-end sm:self-auto">
              <button 
                onClick={fetchData}
                className="p-2 hover:bg-slate-100 rounded-full transition-colors"
                title="Refresh Data"
              >
                <RefreshCw className={`w-5 h-5 text-slate-600 ${loading ? 'animate-spin' : ''}`} />
              </button>
              <button
                onClick={handleDownloadPdf}
                disabled={!data || exportingPdf}
                className="flex items-center gap-2 px-3 py-2 rounded-full bg-violet-600 hover:bg-violet-700 text-white text-sm font-bold shadow-lg shadow-violet-600/20 transition-all disabled:opacity-60 disabled:cursor-not-allowed"
                title="Download PDF"
              >
                <Download className={`w-4 h-4 ${exportingPdf ? 'animate-pulse' : ''}`} />
                PDF
              </button>
              <button
                onClick={handleSendWhatsApp}
                disabled={!data || sendingWhatsApp}
                className="flex items-center gap-2 px-3 py-2 rounded-full bg-green-600 hover:bg-green-700 text-white text-sm font-bold shadow-lg shadow-green-600/20 transition-all disabled:opacity-60 disabled:cursor-not-allowed"
                title="Send via WhatsApp"
              >
                <Send className={`w-4 h-4 ${sendingWhatsApp ? 'animate-pulse' : ''}`} />
                WhatsApp
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Controls */}
        <div className="space-y-4 mb-8">
          <div className="flex flex-wrap gap-2">
            {(['today', 'yesterday', 'this_month', 'last_month', 'custom'] as const).map((r) => (
            <button
              key={r}
              onClick={() => setRange(r)}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition-all ${
                range === r 
                  ? 'bg-violet-600 text-white shadow-lg shadow-violet-600/20' 
                  : 'bg-white text-slate-600 hover:bg-slate-50 border border-slate-200'
              }`}
            >
              {r === 'custom' ? 'From-To' : r.replace('_', ' ').replace(/\b\w/g, l => l.toUpperCase())}
            </button>
            ))}
            <button
              onClick={clearFilters}
              className="w-full sm:w-auto sm:ml-auto flex items-center justify-center gap-2 px-4 py-2 rounded-lg text-sm font-bold bg-white text-slate-700 hover:bg-slate-50 border border-slate-200"
              title="Clear Filters"
            >
              <FilterX className="w-4 h-4" />
              Clear
            </button>
          </div>

          {range === 'custom' && (
            <div className="flex flex-col sm:flex-row flex-wrap gap-3 items-stretch sm:items-end">
              <div className="w-full sm:w-auto">
                <label className="block text-xs font-bold text-slate-600 mb-1">From</label>
                <button
                  type="button"
                  onClick={() => openCalendarFor('from')}
                  className="w-full sm:w-auto px-3 py-2 rounded-lg bg-white border border-slate-200 text-sm text-slate-900 text-left hover:bg-slate-50"
                >
                  {customStartDate || 'Select start'}
                </button>
              </div>
              <div className="w-full sm:w-auto">
                <label className="block text-xs font-bold text-slate-600 mb-1">To</label>
                <button
                  type="button"
                  onClick={() => openCalendarFor('to')}
                  className="w-full sm:w-auto px-3 py-2 rounded-lg bg-white border border-slate-200 text-sm text-slate-900 text-left hover:bg-slate-50"
                >
                  {customEndDate || 'Select end'}
                </button>
              </div>
              <div className="text-xs text-slate-500">
                Select From and To dates.
              </div>
            </div>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
            <div>
              <label className="block text-xs font-bold text-slate-600 mb-1">Category</label>
              <select
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                className="w-full px-3 py-2 rounded-lg bg-white border border-slate-200 text-sm text-slate-900"
              >
                <option value="">All Categories</option>
                {(meta?.categories || []).map((c) => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-xs font-bold text-slate-600 mb-1">Brand</label>
              <select
                value={brand}
                onChange={(e) => setBrand(e.target.value)}
                className="w-full px-3 py-2 rounded-lg bg-white border border-slate-200 text-sm text-slate-900"
              >
                <option value="">All Brands</option>
                {(meta?.brands || []).map((b) => (
                  <option key={b} value={b}>{b}</option>
                ))}
              </select>
            </div>
            <div className="flex items-end">
              <button
                onClick={() => {
                  setAppliedCategory(category);
                  setAppliedBrand(brand);
                }}
                className="w-full px-4 py-2 rounded-lg text-sm font-bold bg-violet-600 hover:bg-violet-700 text-white shadow-lg shadow-violet-600/20 transition-all"
              >
                Apply Filters
              </button>
            </div>
          </div>
        </div>

        {isCalendarOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
            <button
              className="absolute inset-0 bg-black/60 backdrop-blur-sm"
              onClick={() => setIsCalendarOpen(false)}
              aria-label="Close calendar"
            />
            <div className="relative w-full max-w-md bg-white rounded-2xl border border-slate-200 shadow-xl p-4 touch-manipulation">
              <div className="flex items-center justify-between mb-3">
                <div>
                  <div className="text-sm font-bold text-slate-900">Select {calendarTarget === 'from' ? 'From' : 'To'} Date</div>
                  <div className="text-xs text-slate-500">
                    {pendingRange?.from ? format(pendingRange.from, 'yyyy-MM-dd') : '—'}
                    {'  →  '}
                    {pendingRange?.to ? format(pendingRange.to, 'yyyy-MM-dd') : '—'}
                  </div>
                </div>
                <button
                  type="button"
                  onClick={() => setIsCalendarOpen(false)}
                  className="px-3 py-1.5 rounded-lg text-sm font-bold bg-white text-slate-700 hover:bg-slate-50 border border-slate-200"
                >
                  Close
                </button>
              </div>

              <div className="w-full px-2">
                {/* Custom Calendar Header */}
                <div className="flex items-center justify-between mb-4">
                  <button 
                    onClick={() => setViewDate(subMonths(viewDate, 1))}
                    className="p-2 hover:bg-slate-100 rounded-full transition-colors"
                  >
                    <ChevronLeft className="w-6 h-6 text-slate-600" />
                  </button>
                  <h2 className="text-lg font-bold text-slate-800">
                    {format(viewDate, 'MMMM yyyy')}
                  </h2>
                  <button 
                    onClick={() => setViewDate(addMonths(viewDate, 1))}
                    className="p-2 hover:bg-slate-100 rounded-full transition-colors"
                  >
                    <ChevronRight className="w-6 h-6 text-slate-600" />
                  </button>
                </div>

                {/* Days Header */}
                <div className="grid grid-cols-7 mb-2">
                  {['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'].map(d => (
                    <div key={d} className="text-center text-sm font-bold text-slate-400 py-1">
                      {d}
                    </div>
                  ))}
                </div>

                {/* Calendar Grid */}
                <div className="grid grid-cols-7 gap-1">
                  {calendarDays.map((date, i) => {
                    const isSelected = pendingSingleDate && isSameDay(date, pendingSingleDate);
                    const isCurrentMonth = isSameMonth(date, viewDate);
                    const isToday = isSameDay(date, new Date());
                    const isFuture = isAfter(date, startOfToday());

                    return (
                      <button
                        key={i}
                        disabled={isFuture}
                        onClick={() => {
                          if (isFuture) return;
                          setPendingSingleDate(date);
                          if (calendarTarget === 'from') {
                            const currentTo = pendingRange?.to;
                            const nextTo = currentTo && date.getTime() > currentTo.getTime() ? undefined : currentTo;
                            setPendingRange({ from: date, to: nextTo });
                          } else {
                            const currentFrom = pendingRange?.from;
                            const nextFrom = currentFrom && date.getTime() < currentFrom.getTime() ? undefined : currentFrom;
                            setPendingRange({ from: nextFrom, to: date });
                          }
                        }}
                        className={`
                          aspect-square flex items-center justify-center rounded-lg text-lg transition-all
                          ${!isCurrentMonth ? 'text-slate-300' : 'text-slate-700 font-medium'}
                          ${isSelected 
                            ? '!bg-emerald-600 !text-white shadow-lg shadow-emerald-200 scale-105 font-bold ring-2 ring-emerald-100' 
                            : 'hover:bg-slate-100'}
                          ${isToday && !isSelected ? 'border-2 border-emerald-500 text-emerald-600 font-bold' : ''}
                          ${isFuture ? 'opacity-20 cursor-not-allowed hover:bg-transparent' : ''}
                        `}
                      >
                        {format(date, 'd')}
                      </button>
                    );
                  })}
                </div>
              </div>

              <div className="mt-4 flex gap-2">
                <button
                  type="button"
                  className="flex-1 px-4 py-2 rounded-lg text-sm font-bold bg-white text-slate-700 hover:bg-slate-50 border border-slate-200"
                  onClick={() => {
                    if (calendarTarget === 'from') {
                      setPendingRange({ from: undefined, to: pendingRange?.to });
                      setPendingSingleDate(undefined);
                      return;
                    }
                    setPendingRange({ from: pendingRange?.from, to: undefined });
                    setPendingSingleDate(undefined);
                  }}
                >
                  Clear
                </button>
                <button
                  type="button"
                  disabled={calendarTarget === 'from' ? !pendingRange?.from : !pendingRange?.to}
                  className="flex-1 px-4 py-2 rounded-lg text-sm font-bold bg-violet-600 hover:bg-violet-700 text-white disabled:opacity-60 disabled:cursor-not-allowed"
                  onClick={applyCalendar}
                >
                  Apply
                </button>
              </div>
            </div>
          </div>
        )}

        {loading && !data ? (
          <div className="flex items-center justify-center h-64">
            <div className="w-8 h-8 border-4 border-violet-600 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : data ? (
          <div className="space-y-6">
            {/* KPI Cards */}
            <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4 sm:gap-6">
              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-emerald-100 text-emerald-600 rounded-xl">
                    <DollarSign className="w-6 h-6" />
                  </div>
                  <span className="text-xs font-bold text-emerald-600 bg-emerald-50 px-2 py-1 rounded-full">
                    Total Revenue
                  </span>
                </div>
                <h3 className="text-2xl sm:text-3xl font-bold text-slate-900 mb-1 break-words">
                  {formatCurrency(kpis.totalSales)}
                </h3>
                <p className="text-sm text-slate-500">
                  Change: {formatPct(kpis.salesChange)}
                </p>
              </div>

              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-blue-100 text-blue-600 rounded-xl">
                    <TrendingUp className="w-6 h-6" />
                  </div>
                  <span className="text-xs font-bold text-blue-600 bg-blue-50 px-2 py-1 rounded-full">
                    Gross Profit
                  </span>
                </div>
                <h3 className="text-2xl sm:text-3xl font-bold text-slate-900 mb-1 break-words">
                  {formatCurrency(kpis.totalProfit)}
                </h3>
                <p className="text-sm text-slate-500">
                  Change: {formatPct(kpis.profitChange)}
                </p>
              </div>

              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-slate-100 text-slate-700 rounded-xl">
                    <BarChart3 className="w-6 h-6" />
                  </div>
                  <span className="text-xs font-bold text-slate-700 bg-slate-50 px-2 py-1 rounded-full">
                    Margin & Units
                  </span>
                </div>
                <h3 className="text-2xl sm:text-3xl font-bold text-slate-900 mb-1">
                  {formatPct(kpis.margin)}
                </h3>
                <p className="text-sm text-slate-500">
                  Units Sold: {kpis.totalQty.toLocaleString('en-US')}
                </p>
              </div>

              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-violet-100 text-violet-600 rounded-xl">
                    <Package className="w-6 h-6" />
                  </div>
                  <span className="text-xs font-bold text-violet-600 bg-violet-50 px-2 py-1 rounded-full">
                    Top Category
                  </span>
                </div>
                <h3 className="text-2xl font-bold text-slate-900 mb-1 truncate">
                  {topCategory?.name || 'N/A'}
                </h3>
                <p className="text-sm text-slate-500">
                  {formatCurrency(topCategory?.sales || 0)} Sales
                </p>
              </div>
            </div>

            {/* Sales Trend */}
            <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
              <h3 className="text-base sm:text-lg font-bold text-slate-900 mb-4 sm:mb-6">Sales Trend</h3>
              <div className="h-64 sm:h-80">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={data.trend}>
                    <defs>
                      <linearGradient id="colorSales" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#8b5cf6" stopOpacity={0.8}/>
                        <stop offset="95%" stopColor="#8b5cf6" stopOpacity={0}/>
                      </linearGradient>
                    </defs>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} />
                    <XAxis 
                      dataKey="date" 
                      tick={{fontSize: 12}} 
                      tickFormatter={(val) => val.split('-').slice(1).join('/')}
                    />
                    <YAxis 
                      tick={{fontSize: 12}}
                      tickFormatter={(val) => `${val/1000}k`}
                    />
                    <Tooltip 
                      formatter={(value: any) => formatCurrency(Number(value) || 0)}
                      labelFormatter={(label) => new Date(label).toLocaleDateString()}
                    />
                    <Area 
                      type="monotone" 
                      dataKey="sales" 
                      stroke="#8b5cf6" 
                      fillOpacity={1} 
                      fill="url(#colorSales)" 
                    />
                  </AreaChart>
                </ResponsiveContainer>
              </div>
            </div>

            {/* Pie Charts Row */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Category Distribution */}
              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <h3 className="text-base sm:text-lg font-bold text-slate-900 mb-4 sm:mb-6">Category Distribution</h3>
                <div className="h-64 sm:h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={data.categoryBreakdown}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={100}
                        fill="#8884d8"
                        paddingAngle={5}
                        dataKey="sales"
                      >
                        {data.categoryBreakdown.map((entry: any, index: number) => (
                          <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip formatter={(value: any) => formatCurrency(Number(value) || 0)} />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>

              {/* Payment Method Distribution */}
              <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm border border-slate-200">
                <h3 className="text-base sm:text-lg font-bold text-slate-900 mb-4 sm:mb-6">Payment Methods</h3>
                <div className="h-64 sm:h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={data.paymentBreakdown}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={100}
                        fill="#82ca9d"
                        paddingAngle={5}
                        dataKey="sales"
                      >
                        {data.paymentBreakdown.map((entry: any, index: number) => (
                          <Cell key={`cell-${index}`} fill={COLORS[(index + 2) % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip formatter={(value: any) => formatCurrency(Number(value) || 0)} />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>

            {/* Breakdowns + Drilldown */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="p-4 sm:p-6 border-b border-slate-100">
                  <h3 className="text-lg font-bold text-slate-900">Category Breakdown</h3>
                  <p className="text-sm text-slate-500 mt-1">Tip: click a category to filter.</p>
                </div>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm text-left">
                    <thead className="text-xs text-slate-500 uppercase bg-slate-50">
                      <tr>
                        <th className="px-4 sm:px-6 py-3">Category</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Qty</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Sales</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Profit</th>
                        <th className="hidden sm:table-cell px-4 sm:px-6 py-3 text-right">Margin</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.categoryBreakdown.map((row, idx) => {
                        const s = safeNumber(row.sales);
                        const p = safeNumber(row.profit);
                        const q = safeNumber(row.quantity);
                        const m = s > 0 ? (p / s) * 100 : 0;
                        return (
                          <tr
                            key={idx}
                            className="border-b border-slate-100 hover:bg-slate-50 cursor-pointer"
                            onClick={() => setCategory(row.name)}
                            title="Click to filter"
                          >
                            <td className="px-4 sm:px-6 py-4 font-medium text-slate-900">{row.name || 'Unknown'}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">{q.toLocaleString('en-US')}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">{formatCurrency(s)}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-emerald-700 font-medium">{formatCurrency(p)}</td>
                            <td className="hidden sm:table-cell px-4 sm:px-6 py-4 text-right">{formatPct(m)}</td>
                          </tr>
                        );
                      })}
                    </tbody>
                    <tfoot className="bg-slate-50 font-bold border-t-2 border-slate-200">
                      <tr>
                        <td className="px-4 sm:px-6 py-4 text-slate-900">TOTAL</td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900">
                          {data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.quantity), 0).toLocaleString('en-US')}
                        </td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900">
                          {formatCurrency(data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0))}
                        </td>
                        <td className="px-4 sm:px-6 py-4 text-right text-emerald-700">
                          {formatCurrency(data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.profit), 0))}
                        </td>
                        <td className="hidden sm:table-cell px-4 sm:px-6 py-4 text-right text-slate-900">
                          {formatPct(
                            data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0) > 0
                              ? (data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.profit), 0) /
                                  data.categoryBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0)) *
                                100
                              : 0
                          )}
                        </td>
                      </tr>
                    </tfoot>
                  </table>
                </div>
              </div>

              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="p-4 sm:p-6 border-b border-slate-100">
                  <h3 className="text-lg font-bold text-slate-900">Brand Breakdown</h3>
                  <p className="text-sm text-slate-500 mt-1">Tip: click a brand to filter.</p>
                </div>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm text-left">
                    <thead className="text-xs text-slate-500 uppercase bg-slate-50">
                      <tr>
                        <th className="px-4 sm:px-6 py-3">Brand</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Qty</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Sales</th>
                        <th className="px-4 sm:px-6 py-3 text-right">Profit</th>
                        <th className="hidden sm:table-cell px-4 sm:px-6 py-3 text-right">Margin</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.brandBreakdown.slice(0, 30).map((row, idx) => {
                        const s = safeNumber(row.sales);
                        const p = safeNumber(row.profit);
                        const q = safeNumber(row.quantity);
                        const m = s > 0 ? (p / s) * 100 : 0;
                        return (
                          <tr
                            key={idx}
                            className="border-b border-slate-100 hover:bg-slate-50 cursor-pointer"
                            onClick={() => setBrand(row.name)}
                            title="Click to filter"
                          >
                            <td className="px-4 sm:px-6 py-4 font-medium text-slate-900">{row.name || 'Unknown'}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">{q.toLocaleString('en-US')}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">{formatCurrency(s)}</td>
                            <td className="px-4 sm:px-6 py-4 text-right text-emerald-700 font-medium">{formatCurrency(p)}</td>
                            <td className="hidden sm:table-cell px-4 sm:px-6 py-4 text-right">{formatPct(m)}</td>
                          </tr>
                        );
                      })}
                    </tbody>
                    <tfoot className="bg-slate-50 font-bold border-t-2 border-slate-200">
                      <tr>
                        <td className="px-4 sm:px-6 py-4 text-slate-900">TOTAL</td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900">
                          {data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.quantity), 0).toLocaleString('en-US')}
                        </td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900">
                          {formatCurrency(data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0))}
                        </td>
                        <td className="px-4 sm:px-6 py-4 text-right text-emerald-700">
                          {formatCurrency(data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.profit), 0))}
                        </td>
                        <td className="hidden sm:table-cell px-4 sm:px-6 py-4 text-right text-slate-900">
                          {formatPct(
                            data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0) > 0
                              ? (data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.profit), 0) /
                                  data.brandBreakdown.reduce((acc, r) => acc + safeNumber(r.sales), 0)) *
                                100
                              : 0
                          )}
                        </td>
                      </tr>
                    </tfoot>
                  </table>
                </div>
              </div>
            </div>

            {/* Top Items Table */}
            <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
              <div className="p-4 sm:p-6 border-b border-slate-100">
                <h3 className="text-lg font-bold text-slate-900">Top Items</h3>
                <p className="text-sm text-slate-500 mt-1">Sorted by Sales (top 50).</p>
              </div>
              <div className="overflow-x-auto">
                <table className="w-full text-sm text-left">
                  <thead className="text-xs text-slate-500 uppercase bg-slate-50">
                    <tr>
                      <th className="px-4 sm:px-6 py-3">Item</th>
                      <th className="hidden md:table-cell px-4 sm:px-6 py-3">Brand</th>
                      <th className="hidden md:table-cell px-4 sm:px-6 py-3">Category</th>
                      <th className="px-4 sm:px-6 py-3 text-right">Qty</th>
                      <th className="px-4 sm:px-6 py-3 text-right">Sales</th>
                      <th className="hidden md:table-cell px-4 sm:px-6 py-3 text-right">Profit</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.topItems.map((item, index) => (
                      <tr key={`${item.itemId}-${index}`} className="border-b border-slate-100 hover:bg-slate-50">
                        <td className="px-4 sm:px-6 py-4 font-medium text-slate-900">
                          <div className="leading-snug">{item.name}</div>
                          <div className="text-xs text-slate-500 mt-1 md:hidden">
                            {(item.brand || 'Unknown') + ' • ' + (item.category || 'Unknown')}
                          </div>
                        </td>
                        <td className="hidden md:table-cell px-4 sm:px-6 py-4">{item.brand || 'Unknown'}</td>
                        <td className="hidden md:table-cell px-4 sm:px-6 py-4">{item.category || 'Unknown'}</td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">{safeNumber(item.quantity).toLocaleString('en-US')}</td>
                        <td className="px-4 sm:px-6 py-4 text-right text-slate-900 font-medium">
                          <div>{formatCurrency(safeNumber(item.sales))}</div>
                          <div className="text-xs text-emerald-700 font-semibold md:hidden">Profit: {formatCurrency(safeNumber(item.profit))}</div>
                        </td>
                        <td className="hidden md:table-cell px-4 sm:px-6 py-4 text-right text-emerald-700 font-medium">{formatCurrency(safeNumber(item.profit))}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}
