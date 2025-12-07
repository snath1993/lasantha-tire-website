'use client';

import { useState, useEffect } from 'react';
import { 
  TrendingUp, TrendingDown, FileText, CheckCircle, 
  XCircle, Clock, DollarSign, Package, BarChart3,
  Calendar, RefreshCw
} from 'lucide-react';

const BOT_API_URL = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';

interface Stats {
  total: number;
  booked: number;
  expired: number;
  pending: number;
  conversionRate: number;
  totalValue: number;
  bookedValue: number;
  averageQuotation: number;
}

interface TrendData {
  date: string;
  quotations: number;
  booked: number;
}

interface SourceData {
  Source: string;
  count: number;
  booked: number;
}

interface PopularItem {
  description: string;
  brand: string;
  quotationCount: number;
  totalQuantity: number;
  averagePrice: number;
}

export default function AnalyticsDashboard() {
  const [stats, setStats] = useState<Stats | null>(null);
  const [trend, setTrend] = useState<TrendData[]>([]);
  const [sources, setSources] = useState<SourceData[]>([]);
  const [popularItems, setPopularItems] = useState<PopularItem[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchAnalytics = async () => {
    setLoading(true);
    try {
      // Fetch statistics
      const statsRes = await fetch(`${BOT_API_URL}/api/quotations/analytics/stats`);
      const statsData = await statsRes.json();
      
      if (statsData.ok) {
        setStats(statsData.stats);
        setTrend(statsData.trend);
        setSources(statsData.sources);
      }

      // Fetch popular items
      const itemsRes = await fetch(`${BOT_API_URL}/api/quotations/analytics/items`);
      const itemsData = await itemsRes.json();
      
      if (itemsData.ok) {
        setPopularItems(itemsData.items);
      }
    } catch (error) {
      console.error('Failed to fetch analytics:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAnalytics();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
        <div className="flex flex-col items-center gap-4">
          <RefreshCw className="w-12 h-12 text-blue-400 animate-spin" />
          <p className="text-slate-400">Loading analytics...</p>
        </div>
      </div>
    );
  }

  if (!stats) {
    return (
      <div className="p-4 text-center text-slate-400">
        Failed to load analytics
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 p-4 pb-24">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Analytics Dashboard</h1>
          <p className="text-sm text-slate-400">Last 30 days performance</p>
        </div>
        <button
          onClick={fetchAnalytics}
          className="p-3 bg-slate-800 hover:bg-slate-700 text-white rounded-xl transition-all active:scale-95"
        >
          <RefreshCw className="w-5 h-5" />
        </button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-2 gap-3 mb-6">
        <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/10 border border-blue-500/30 rounded-2xl p-4">
          <div className="flex items-center gap-2 mb-2">
            <FileText className="w-5 h-5 text-blue-400" />
            <span className="text-xs text-blue-300 font-semibold">Total Quotations</span>
          </div>
          <p className="text-3xl font-bold text-white">{stats.total}</p>
        </div>

        <div className="bg-gradient-to-br from-green-500/20 to-green-600/10 border border-green-500/30 rounded-2xl p-4">
          <div className="flex items-center gap-2 mb-2">
            <CheckCircle className="w-5 h-5 text-green-400" />
            <span className="text-xs text-green-300 font-semibold">Booked</span>
          </div>
          <p className="text-3xl font-bold text-white">{stats.booked}</p>
        </div>

        <div className="bg-gradient-to-br from-orange-500/20 to-orange-600/10 border border-orange-500/30 rounded-2xl p-4">
          <div className="flex items-center gap-2 mb-2">
            <Clock className="w-5 h-5 text-orange-400" />
            <span className="text-xs text-orange-300 font-semibold">Pending</span>
          </div>
          <p className="text-3xl font-bold text-white">{stats.pending}</p>
        </div>

        <div className="bg-gradient-to-br from-red-500/20 to-red-600/10 border border-red-500/30 rounded-2xl p-4">
          <div className="flex items-center gap-2 mb-2">
            <XCircle className="w-5 h-5 text-red-400" />
            <span className="text-xs text-red-300 font-semibold">Expired</span>
          </div>
          <p className="text-3xl font-bold text-white">{stats.expired}</p>
        </div>
      </div>

      {/* Conversion Rate */}
      <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 mb-6">
        <div className="flex items-center justify-between mb-3">
          <h3 className="text-lg font-bold text-white flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-blue-400" />
            Conversion Rate
          </h3>
          <span className="text-2xl font-bold text-blue-400">{stats.conversionRate}%</span>
        </div>
        <div className="w-full bg-slate-700 rounded-full h-3 overflow-hidden">
          <div 
            className="h-full bg-gradient-to-r from-blue-500 to-green-500 transition-all duration-500"
            style={{ width: `${stats.conversionRate}%` }}
          />
        </div>
      </div>

      {/* Revenue Stats */}
      <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 mb-6">
        <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
          <DollarSign className="w-5 h-5 text-green-400" />
          Revenue Overview
        </h3>
        <div className="space-y-3">
          <div className="flex justify-between items-center">
            <span className="text-slate-400">Total Value</span>
            <span className="text-xl font-bold text-white">Rs {stats.totalValue.toLocaleString()}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-slate-400">Booked Value</span>
            <span className="text-xl font-bold text-green-400">Rs {stats.bookedValue.toLocaleString()}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-slate-400">Average Quotation</span>
            <span className="text-xl font-bold text-blue-400">Rs {Math.round(stats.averageQuotation).toLocaleString()}</span>
          </div>
        </div>
      </div>

      {/* Weekly Trend */}
      {trend.length > 0 && (
        <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 mb-6">
          <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
            <Calendar className="w-5 h-5 text-purple-400" />
            Weekly Trend (Last 7 Days)
          </h3>
          <div className="space-y-2">
            {trend.map((day, idx) => (
              <div key={idx} className="flex items-center gap-3">
                <span className="text-xs text-slate-500 w-24">{new Date(day.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}</span>
                <div className="flex-1 flex gap-1">
                  <div 
                    className="bg-blue-500 h-8 rounded flex items-center justify-center text-xs text-white font-bold transition-all"
                    style={{ width: `${(day.quotations / Math.max(...trend.map(d => d.quotations))) * 100}%`, minWidth: '30px' }}
                  >
                    {day.quotations}
                  </div>
                  <div 
                    className="bg-green-500 h-8 rounded flex items-center justify-center text-xs text-white font-bold transition-all"
                    style={{ width: `${(day.booked / Math.max(...trend.map(d => d.quotations))) * 100}%`, minWidth: day.booked > 0 ? '30px' : '0' }}
                  >
                    {day.booked > 0 && day.booked}
                  </div>
                </div>
              </div>
            ))}
          </div>
          <div className="flex items-center gap-4 mt-4 text-xs">
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 bg-blue-500 rounded"></div>
              <span className="text-slate-400">Quotations</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 bg-green-500 rounded"></div>
              <span className="text-slate-400">Booked</span>
            </div>
          </div>
        </div>
      )}

      {/* Source Breakdown */}
      {sources.length > 0 && (
        <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 mb-6">
          <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
            <BarChart3 className="w-5 h-5 text-cyan-400" />
            Quotation Sources
          </h3>
          <div className="space-y-3">
            {sources.map((source, idx) => (
              <div key={idx}>
                <div className="flex justify-between items-center mb-1">
                  <span className="text-sm text-slate-300">{source.Source || 'Unknown'}</span>
                  <span className="text-sm text-slate-400">{source.count} ({source.booked} booked)</span>
                </div>
                <div className="w-full bg-slate-700 rounded-full h-2">
                  <div 
                    className="h-full bg-gradient-to-r from-cyan-500 to-blue-500 rounded-full"
                    style={{ width: `${(source.count / stats.total) * 100}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Popular Items */}
      {popularItems.length > 0 && (
        <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5">
          <h3 className="text-lg font-bold text-white flex items-center gap-2 mb-4">
            <Package className="w-5 h-5 text-yellow-400" />
            Top 10 Popular Items
          </h3>
          <div className="space-y-3">
            {popularItems.slice(0, 10).map((item, idx) => (
              <div key={idx} className="bg-slate-700/50 rounded-xl p-3">
                <div className="flex items-start justify-between mb-2">
                  <div className="flex-1">
                    <p className="text-sm font-semibold text-white">{item.description}</p>
                    <p className="text-xs text-slate-400">{item.brand}</p>
                  </div>
                  <span className="text-xs bg-yellow-500/20 text-yellow-400 px-2 py-1 rounded-full">
                    #{idx + 1}
                  </span>
                </div>
                <div className="flex items-center gap-4 text-xs text-slate-400">
                  <span>{item.quotationCount} quotations</span>
                  <span>•</span>
                  <span>{item.totalQuantity} units</span>
                  <span>•</span>
                  <span className="text-green-400">Rs {Math.round(item.averagePrice).toLocaleString()}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
