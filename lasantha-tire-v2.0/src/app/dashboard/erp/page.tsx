'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/core/lib/client-auth';
import { 
  Package, TrendingUp, DollarSign, Users, AlertTriangle, 
  ShoppingCart, FileText, Home, RefreshCw, Calendar,
  BarChart3, PieChart, TrendingDown, Clock, Download,
  Search, Filter, ArrowUpRight, ArrowDownRight
} from 'lucide-react';
import Link from 'next/link';
import {
  LineChart, Line, BarChart, Bar, PieChart as RechartsPie, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  AreaChart, Area
} from 'recharts';

interface InventorySummary {
  TotalProducts: number;
  TotalBrands: number;
  TotalQuantity: number;
  LowStockItems: number;
  OutOfStockItems: number;
}

interface SalesSummary {
  TotalInvoices: number;
  TotalRevenue: number;
  AverageInvoiceValue: number;
  UniqueProductsSold: number;
}

interface FinancialSummary {
  CurrentMonthRevenue: number;
  LastMonthRevenue: number;
  YearToDateRevenue: number;
  ActiveCustomersThisMonth: number;
}

interface BrandData {
  Brand: string;
  ProductCount: number;
  TotalQuantity: number;
  AvgQuantity: number;
  LowStockCount: number;
}

interface TopProduct {
  ItemId: string;
  Description: string;
  InvoiceCount: number;
  TotalQuantitySold: number;
  TotalRevenue: number;
  AvgRevenue: number;
}

interface DailyTrend {
  Date: string;
  TotalRevenue: number;
  InvoiceCount: number;
}

interface MonthlyData {
  Month: string;
  TotalRevenue: number;
  InvoiceCount: number;
}

export default function ERPDashboard() {
  const router = useRouter();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [activeTab, setActiveTab] = useState<'inventory' | 'sales' | 'finance' | 'overview'>('overview');
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Data states
  const [inventorySummary, setInventorySummary] = useState<InventorySummary | null>(null);
  const [salesSummary, setSalesSummary] = useState<SalesSummary | null>(null);
  const [financialSummary, setFinancialSummary] = useState<FinancialSummary | null>(null);
  const [brandData, setBrandData] = useState<BrandData[]>([]);
  const [topProducts, setTopProducts] = useState<TopProduct[]>([]);
  const [dailyTrends, setDailyTrends] = useState<DailyTrend[]>([]);
  const [monthlyData, setMonthlyData] = useState<MonthlyData[]>([]);
  const [dateRange, setDateRange] = useState(30);

  useEffect(() => {
    const verifyAuth = async () => {
      const auth = await checkAuth();
      if (!auth.authenticated) {
        setIsAuthenticated(false);
        router.replace('/');
        return;
      }
      setIsAuthenticated(true);
    };
    verifyAuth();
  }, [router]);

  useEffect(() => {
    if (isAuthenticated) {
      loadAllData();
    }
  }, [dateRange, isAuthenticated]);

  // Auto-refresh effect
  useEffect(() => {
    let interval: NodeJS.Timeout;
    if (autoRefresh) {
      interval = setInterval(() => {
        loadAllData();
      }, 30000); // Refresh every 30 seconds
    }
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [autoRefresh, dateRange]);

  const loadAllData = async () => {
    setLoading(true);
    try {
      await Promise.all([
        loadInventoryData(),
        loadSalesData(),
        loadFinancialData()
      ]);
    } catch (error) {
      console.error('Error loading ERP data:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadInventoryData = async () => {
    try {
      // Summary
      const summaryRes = await fetch('/api/erp/inventory?type=summary');
      if (summaryRes.ok) {
        const data = await summaryRes.json();
        if (data.success && data.data[0]) {
          setInventorySummary(data.data[0]);
        }
      }

      // By brand
      const brandsRes = await fetch('/api/erp/inventory?type=by-brand');
      if (brandsRes.ok) {
        const data = await brandsRes.json();
        if (data.success) {
          setBrandData(data.data);
        }
      }
    } catch (error) {
      // Silent handling
    }
  };

  const loadSalesData = async () => {
    try {
      // Summary
      const summaryRes = await fetch(`/api/erp/sales?type=summary&days=${dateRange}`);
      if (summaryRes.ok) {
        const data = await summaryRes.json();
        if (data.success && data.data[0]) {
          setSalesSummary(data.data[0]);
        }
      }

      // Top products
      const productsRes = await fetch(`/api/erp/sales?type=top-products&days=${dateRange}&limit=10`);
      if (productsRes.ok) {
        const data = await productsRes.json();
        if (data.success) {
          setTopProducts(data.data);
        }
      }

      // Daily trends
      const trendsRes = await fetch(`/api/erp/sales?type=daily-trend&days=${dateRange}`);
      if (trendsRes.ok) {
        const data = await trendsRes.json();
        if (data.success) {
          setDailyTrends(data.data);
        }
      }

      // Monthly data
      const monthlyRes = await fetch(`/api/erp/sales?type=monthly`);
      if (monthlyRes.ok) {
        const data = await monthlyRes.json();
        if (data.success) {
          setMonthlyData(data.data);
        }
      }
    } catch (error) {
      // Silent handling
    }
  };

  const loadFinancialData = async () => {
    try {
      const res = await fetch('/api/erp/finance?type=summary');
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.data[0]) {
          setFinancialSummary(data.data[0]);
        }
      }
    } catch (error) {
      // Silent handling
    }
  };

  const handleRefresh = async () => {
    setRefreshing(true);
    await loadAllData();
    setRefreshing(false);
  };

  const formatCurrency = (amount: number | null | undefined) => {
    if (!amount) return 'LKR 0';
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(amount);
  };

  const formatNumber = (num: number | null | undefined) => {
    if (!num) return '0';
    return new Intl.NumberFormat('en-US').format(num);
  };

  // Export to CSV function
  const exportToCSV = (data: any[], filename: string) => {
    if (!data || data.length === 0) {
      alert('No data to export!');
      return;
    }

    // Get headers from first object
    const headers = Object.keys(data[0]);
    
    // Create CSV content
    const csvContent = [
      headers.join(','), // Header row
      ...data.map(row => 
        headers.map(header => {
          const value = row[header];
          // Handle values with commas by wrapping in quotes
          if (typeof value === 'string' && value.includes(',')) {
            return `"${value}"`;
          }
          return value;
        }).join(',')
      )
    ].join('\n');

    // Create blob and download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `${filename}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  // Export handlers for different data types
  const exportInventoryData = () => {
    exportToCSV(brandData, 'inventory_by_brand');
  };

  const exportSalesData = () => {
    exportToCSV(topProducts, 'top_selling_products');
  };

  const exportDailyTrends = () => {
    exportToCSV(dailyTrends, 'daily_sales_trends');
  };

  const exportMonthlyData = () => {
    exportToCSV(monthlyData, 'monthly_revenue_data');
  };

  const calculateGrowth = (current: number | null | undefined, previous: number | null | undefined) => {
    if (!current || !previous) return 0;
    return ((current - previous) / previous) * 100;
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading ERP Dashboard...</p>
          <p className="text-slate-500 text-xs mt-1">Fetching data from SQL Server</p>
        </div>
      </div>
    );
  }

  const revenueGrowth = calculateGrowth(
    financialSummary?.CurrentMonthRevenue,
    financialSummary?.LastMonthRevenue
  );

  return (
    <div className="space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-white tracking-tight">
            ERP Analytics
          </h1>
          <p className="text-slate-400 mt-1">
            Real-time business intelligence from SQL Server
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <div className="relative">
            <select
              value={dateRange}
              onChange={(e) => setDateRange(Number(e.target.value))}
              className="appearance-none pl-4 pr-10 py-2.5 bg-slate-800 border border-white/10 rounded-xl text-sm font-medium text-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 shadow-xl cursor-pointer hover:bg-slate-700 transition-colors"
            >
              <option value={7}>Last 7 Days</option>
              <option value={30}>Last 30 Days</option>
              <option value={90}>Last 90 Days</option>
              <option value={365}>Last Year</option>
            </select>
            <Filter className="w-4 h-4 text-slate-400 absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none" />
          </div>

          <button
            onClick={() => setAutoRefresh(!autoRefresh)}
            className={`flex items-center gap-2 px-4 py-2.5 rounded-xl text-sm font-medium transition-all shadow-sm border ${
              autoRefresh 
                ? 'bg-emerald-500/20 text-emerald-400 border-emerald-500/30 hover:bg-emerald-500/30' 
                : 'bg-slate-800 text-slate-300 border-white/10 hover:bg-slate-700'
            }`}
          >
            <Clock className={`w-4 h-4 ${autoRefresh ? 'text-emerald-400' : 'text-slate-400'}`} />
            Auto-Refresh {autoRefresh ? 'ON' : 'OFF'}
          </button>

          <button
            onClick={handleRefresh}
            disabled={refreshing}
            className="flex items-center gap-2 px-4 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white rounded-xl transition-all shadow-lg shadow-blue-600/20 text-sm font-medium"
          >
            <RefreshCw className={`w-4 h-4 ${refreshing ? 'animate-spin' : ''}`} />
            Refresh
          </button>
          
          <Link
            href="/dashboard"
            className="px-4 py-2.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl transition-colors border border-white/10 text-sm font-medium shadow-sm"
          >
            Back to Home
          </Link>
        </div>
      </div>

      {/* Tab Navigation */}
      <div className="flex gap-2 p-1 bg-slate-800/50 backdrop-blur-xl border border-white/10 rounded-2xl w-fit overflow-x-auto">
        {[
          { id: 'overview', label: 'Overview', icon: BarChart3 },
          { id: 'inventory', label: 'Inventory', icon: Package },
          { id: 'sales', label: 'Sales', icon: TrendingUp },
          { id: 'finance', label: 'Finance', icon: DollarSign }
        ].map((tab) => {
          const Icon = tab.icon;
          const isActive = activeTab === tab.id;
          return (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all whitespace-nowrap ${
                isActive
                  ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20'
                  : 'text-slate-400 hover:text-white hover:bg-white/5'
              }`}
            >
              <Icon className={`w-4 h-4 ${isActive ? 'text-white' : 'text-slate-400'}`} />
              {tab.label}
            </button>
          );
        })}
      </div>

      {/* Overview Tab */}
      {activeTab === 'overview' && (
        <div className="space-y-6">
          {/* Top Stats Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {/* Total Revenue */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-4">
                <div className="w-12 h-12 bg-blue-500/20 rounded-2xl flex items-center justify-center border border-blue-500/20">
                  <DollarSign className="w-6 h-6 text-blue-400" />
                </div>
                <div className={`flex items-center gap-1 text-xs font-bold px-2 py-1 rounded-full ${
                  revenueGrowth >= 0 ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/20' : 'bg-rose-500/20 text-rose-400 border border-rose-500/20'
                }`}>
                  {revenueGrowth >= 0 ? <ArrowUpRight className="w-3 h-3" /> : <ArrowDownRight className="w-3 h-3" />}
                  {Math.abs(revenueGrowth).toFixed(1)}%
                </div>
              </div>
              <h3 className="text-3xl font-bold text-white mb-1 tracking-tight">
                {formatCurrency(financialSummary?.CurrentMonthRevenue)}
              </h3>
              <p className="text-slate-400 text-sm font-medium">This Month Revenue</p>
              <p className="text-slate-500 text-xs mt-2">
                Last month: {formatCurrency(financialSummary?.LastMonthRevenue)}
              </p>
            </div>

            {/* Total Products */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-4">
                <div className="w-12 h-12 bg-violet-500/20 rounded-2xl flex items-center justify-center border border-violet-500/20">
                  <Package className="w-6 h-6 text-violet-400" />
                </div>
              </div>
              <h3 className="text-3xl font-bold text-white mb-1 tracking-tight">
                {formatNumber(inventorySummary?.TotalProducts)}
              </h3>
              <p className="text-slate-400 text-sm font-medium">Total Products</p>
              <p className="text-slate-500 text-xs mt-2">
                {formatNumber(inventorySummary?.TotalQuantity)} units in stock
              </p>
            </div>

            {/* Total Invoices */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-4">
                <div className="w-12 h-12 bg-emerald-500/20 rounded-2xl flex items-center justify-center border border-emerald-500/20">
                  <FileText className="w-6 h-6 text-emerald-400" />
                </div>
              </div>
              <h3 className="text-3xl font-bold text-white mb-1 tracking-tight">
                {formatNumber(salesSummary?.TotalInvoices)}
              </h3>
              <p className="text-slate-400 text-sm font-medium">Total Invoices</p>
              <p className="text-slate-500 text-xs mt-2">
                Last {dateRange} days
              </p>
            </div>

            {/* Active Customers */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-4">
                <div className="w-12 h-12 bg-amber-500/20 rounded-2xl flex items-center justify-center border border-amber-500/20">
                  <Users className="w-6 h-6 text-amber-400" />
                </div>
              </div>
              <h3 className="text-3xl font-bold text-white mb-1 tracking-tight">
                {formatNumber(financialSummary?.ActiveCustomersThisMonth)}
              </h3>
              <p className="text-slate-400 text-sm font-medium">Active Customers</p>
              <p className="text-slate-500 text-xs mt-2">
                This month
              </p>
            </div>
          </div>

          {/* Alerts Row */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {/* Low Stock Alert */}
            <div className="bg-amber-500/10 border border-amber-500/20 rounded-[2rem] p-6">
              <div className="flex items-center gap-4 mb-4">
                <div className="w-10 h-10 bg-amber-500/20 rounded-xl flex items-center justify-center shadow-sm border border-amber-500/20">
                  <AlertTriangle className="w-5 h-5 text-amber-400" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-amber-400">
                    {formatNumber(inventorySummary?.LowStockItems)}
                  </h3>
                  <p className="text-amber-300 text-sm font-medium">Low Stock Items</p>
                </div>
              </div>
              <button 
                onClick={() => setActiveTab('inventory')}
                className="text-amber-400 hover:text-amber-300 text-sm font-bold flex items-center gap-1 transition-colors"
              >
                View Details <ArrowUpRight className="w-3 h-3" />
              </button>
            </div>

            {/* Out of Stock Alert */}
            <div className="bg-rose-500/10 border border-rose-500/20 rounded-[2rem] p-6">
              <div className="flex items-center gap-4 mb-4">
                <div className="w-10 h-10 bg-rose-500/20 rounded-xl flex items-center justify-center shadow-sm border border-rose-500/20">
                  <AlertTriangle className="w-5 h-5 text-rose-400" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-rose-400">
                    {formatNumber(inventorySummary?.OutOfStockItems)}
                  </h3>
                  <p className="text-rose-300 text-sm font-medium">Out of Stock</p>
                </div>
              </div>
              <button 
                onClick={() => setActiveTab('inventory')}
                className="text-rose-400 hover:text-rose-300 text-sm font-bold flex items-center gap-1 transition-colors"
              >
                View Details <ArrowUpRight className="w-3 h-3" />
              </button>
            </div>

            {/* Average Invoice */}
            <div className="bg-blue-500/10 border border-blue-500/20 rounded-[2rem] p-6">
              <div className="flex items-center gap-4 mb-4">
                <div className="w-10 h-10 bg-blue-500/20 rounded-xl flex items-center justify-center shadow-sm border border-blue-500/20">
                  <ShoppingCart className="w-5 h-5 text-blue-400" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-blue-400">
                    {formatCurrency(salesSummary?.AverageInvoiceValue)}
                  </h3>
                  <p className="text-blue-300 text-sm font-medium">Avg Invoice Value</p>
                </div>
              </div>
              <button 
                onClick={() => setActiveTab('sales')}
                className="text-blue-400 hover:text-blue-300 text-sm font-bold flex items-center gap-1 transition-colors"
              >
                View Details <ArrowUpRight className="w-3 h-3" />
              </button>
            </div>
          </div>

          {/* Charts Section */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Revenue Trend Chart */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-bold text-white flex items-center gap-2">
                  <div className="w-1 h-5 bg-blue-500 rounded-full"></div>
                  Revenue Trend ({dateRange} Days)
                </h3>
              </div>
              <ResponsiveContainer width="100%" height={300}>
                <AreaChart data={dailyTrends}>
                  <defs>
                    <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="#3b82f6" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" vertical={false} />
                  <XAxis 
                    dataKey="Date" 
                    stroke="#94a3b8"
                    tick={{ fill: '#94a3b8', fontSize: 12 }}
                    tickLine={false}
                    axisLine={false}
                    dy={10}
                  />
                  <YAxis 
                    stroke="#94a3b8"
                    tick={{ fill: '#94a3b8', fontSize: 12 }}
                    tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`}
                    tickLine={false}
                    axisLine={false}
                    dx={-10}
                  />
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: '#1e293b', 
                      border: '1px solid rgba(255,255,255,0.1)',
                      borderRadius: '12px',
                      boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                      color: '#fff'
                    }}
                    formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Revenue']}
                    labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                  />
                  <Area 
                    type="monotone" 
                    dataKey="TotalRevenue" 
                    stroke="#3b82f6" 
                    strokeWidth={3}
                    fillOpacity={1} 
                    fill="url(#colorRevenue)" 
                  />
                </AreaChart>
              </ResponsiveContainer>
            </div>

            {/* Top Brands Bar Chart */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-bold text-white flex items-center gap-2">
                  <div className="w-1 h-5 bg-violet-500 rounded-full"></div>
                  Top 8 Brands by Stock
                </h3>
              </div>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={brandData.slice(0, 8)}>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" vertical={false} />
                  <XAxis 
                    dataKey="Brand" 
                    stroke="#94a3b8"
                    tick={{ fill: '#94a3b8', fontSize: 11 }}
                    angle={-45}
                    textAnchor="end"
                    height={80}
                    tickLine={false}
                    axisLine={false}
                  />
                  <YAxis 
                    stroke="#94a3b8"
                    tick={{ fill: '#94a3b8', fontSize: 12 }}
                    tickLine={false}
                    axisLine={false}
                    dx={-10}
                  />
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: '#1e293b', 
                      border: '1px solid rgba(255,255,255,0.1)',
                      borderRadius: '12px',
                      boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                      color: '#fff'
                    }}
                    labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                    cursor={{ fill: 'rgba(255,255,255,0.05)' }}
                  />
                  <Bar dataKey="TotalQuantity" fill="#8b5cf6" radius={[6, 6, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Monthly Comparison Chart */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <div className="w-1 h-5 bg-emerald-500 rounded-full"></div>
                12-Month Revenue Comparison
              </h3>
              <button className="flex items-center gap-2 px-4 py-2 bg-slate-700/50 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors border border-white/5">
                <Download className="w-4 h-4" />
                Export
              </button>
            </div>
            <ResponsiveContainer width="100%" height={350}>
              <LineChart data={monthlyData}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" vertical={false} />
                <XAxis 
                  dataKey="Month" 
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickLine={false}
                  axisLine={false}
                  dy={10}
                />
                <YAxis 
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`}
                  tickLine={false}
                  axisLine={false}
                  dx={-10}
                />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: '#1e293b', 
                    border: '1px solid rgba(255,255,255,0.1)',
                    borderRadius: '12px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                    color: '#fff'
                  }}
                  formatter={(value: any, name: any) => {
                    if (name === 'TotalRevenue') return [`Rs ${formatCurrency(value)}`, 'Revenue'];
                    return [value, 'Invoices'];
                  }}
                  labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                />
                <Legend 
                  wrapperStyle={{ paddingTop: '20px', color: '#fff' }}
                />
                <Line 
                  type="monotone" 
                  dataKey="TotalRevenue" 
                  stroke="#10b981" 
                  strokeWidth={3}
                  dot={{ fill: '#1e293b', stroke: '#10b981', strokeWidth: 2, r: 4 }}
                  activeDot={{ r: 6, fill: '#10b981' }}
                  name="Revenue"
                />
                <Line 
                  type="monotone" 
                  dataKey="InvoiceCount" 
                  stroke="#f59e0b" 
                  strokeWidth={3}
                  dot={{ fill: '#1e293b', stroke: '#f59e0b', strokeWidth: 2, r: 4 }}
                  activeDot={{ r: 6, fill: '#f59e0b' }}
                  name="Invoices"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* Top Products & Top Brands */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Top Selling Products */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
              <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
                <div className="w-1 h-5 bg-emerald-500 rounded-full"></div>
                Top Selling Products
              </h3>
              <div className="space-y-4">
                {topProducts.slice(0, 5).map((product, index) => (
                  <div key={product.ItemId} className="flex items-center justify-between p-4 bg-slate-700/30 rounded-2xl border border-white/5 hover:border-white/10 transition-colors">
                    <div className="flex items-center gap-4 flex-1 min-w-0">
                      <div className="flex-shrink-0 w-10 h-10 bg-slate-800 rounded-xl flex items-center justify-center shadow-sm border border-white/10">
                        <span className="text-white font-bold text-sm">{index + 1}</span>
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-white font-medium text-sm truncate">{product.Description}</p>
                        <p className="text-slate-400 text-xs mt-0.5">{formatNumber(product.TotalQuantitySold)} units sold</p>
                      </div>
                    </div>
                    <div className="text-right flex-shrink-0 ml-4">
                      <p className="text-emerald-400 font-bold text-sm">{formatCurrency(product.TotalRevenue)}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Top Brands by Stock */}
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
              <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
                <div className="w-1 h-5 bg-violet-500 rounded-full"></div>
                Top Brands by Stock
              </h3>
              <div className="space-y-4">
                {brandData.slice(0, 5).map((brand, index) => (
                  <div key={brand.Brand} className="flex items-center justify-between p-4 bg-slate-700/30 rounded-2xl border border-white/5 hover:border-white/10 transition-colors">
                    <div className="flex items-center gap-4 flex-1">
                      <div className="flex-shrink-0 w-10 h-10 bg-slate-800 rounded-xl flex items-center justify-center shadow-sm border border-white/10">
                        <span className="text-white font-bold text-sm">{index + 1}</span>
                      </div>
                      <div>
                        <p className="text-white font-medium text-sm">{brand.Brand}</p>
                        <p className="text-slate-400 text-xs mt-0.5">{formatNumber(brand.ProductCount)} products</p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="text-violet-400 font-bold text-sm">{formatNumber(brand.TotalQuantity)}</p>
                      <p className="text-slate-400 text-xs">units</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Inventory Tab */}
      {activeTab === 'inventory' && (
        <div className="space-y-6">
          {/* Inventory Stats */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Total Products</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(inventorySummary?.TotalProducts)}</h3>
                </div>
                <div className="w-12 h-12 bg-violet-500/20 rounded-2xl flex items-center justify-center border border-violet-500/20">
                  <Package className="w-6 h-6 text-violet-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Total Brands</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(inventorySummary?.TotalBrands)}</h3>
                </div>
                <div className="w-12 h-12 bg-blue-500/20 rounded-2xl flex items-center justify-center border border-blue-500/20">
                  <BarChart3 className="w-6 h-6 text-blue-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Low Stock</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(inventorySummary?.LowStockItems)}</h3>
                </div>
                <div className="w-12 h-12 bg-amber-500/20 rounded-2xl flex items-center justify-center border border-amber-500/20">
                  <AlertTriangle className="w-6 h-6 text-amber-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Out of Stock</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(inventorySummary?.OutOfStockItems)}</h3>
                </div>
                <div className="w-12 h-12 bg-rose-500/20 rounded-2xl flex items-center justify-center border border-rose-500/20">
                  <TrendingDown className="w-6 h-6 text-rose-400" />
                </div>
              </div>
            </div>
          </div>

          {/* Brand Distribution Chart */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <div className="w-1 h-5 bg-blue-500 rounded-full"></div>
                Top 10 Brands Distribution
              </h3>
              <button 
                onClick={exportInventoryData}
                className="flex items-center gap-2 px-4 py-2 bg-slate-700/50 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors border border-white/5"
              >
                <Download className="w-4 h-4" />
                Export CSV
              </button>
            </div>
            <ResponsiveContainer width="100%" height={400}>
              <BarChart data={brandData.slice(0, 10)} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" horizontal={false} />
                <XAxis type="number" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 12 }} tickLine={false} axisLine={false} />
                <YAxis dataKey="Brand" type="category" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 12 }} width={100} tickLine={false} axisLine={false} />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: '#1e293b', 
                    border: '1px solid rgba(255,255,255,0.1)',
                    borderRadius: '12px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                    color: '#fff'
                  }}
                  labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                  cursor={{ fill: 'rgba(255,255,255,0.05)' }}
                />
                <Bar dataKey="TotalQuantity" fill="#8b5cf6" radius={[0, 6, 6, 0]}>
                  {brandData.slice(0, 10).map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={`hsl(${260 - index * 10}, 70%, ${60 - index * 2}%)`} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* Detailed Brand Table */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white">Brand Details</h3>
              <div className="relative">
                <input
                  type="text"
                  placeholder="Search brands..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10 pr-4 py-2.5 bg-slate-700/50 border border-white/10 rounded-xl text-sm text-white focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 w-64 transition-all placeholder-slate-400"
                />
                <Search className="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
              </div>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-white/10">
                    <th className="text-left text-slate-400 font-medium text-sm py-4 px-4">Brand</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Products</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Total Qty</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Avg Qty</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Low Stock</th>
                  </tr>
                </thead>
                <tbody>
                  {brandData
                    .filter(brand => 
                      brand.Brand.toLowerCase().includes(searchTerm.toLowerCase())
                    )
                    .map((brand) => (
                    <tr key={brand.Brand} className="border-b border-white/5 hover:bg-white/5 transition-colors">
                      <td className="py-4 px-4 text-white font-medium text-sm">{brand.Brand}</td>
                      <td className="py-4 px-4 text-right text-slate-300 text-sm">{formatNumber(brand.ProductCount)}</td>
                      <td className="py-4 px-4 text-right text-indigo-400 font-medium text-sm">{formatNumber(brand.TotalQuantity)}</td>
                      <td className="py-4 px-4 text-right text-slate-300 text-sm">{formatNumber(brand.AvgQuantity)}</td>
                      <td className="py-4 px-4 text-right">
                        <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${
                          brand.LowStockCount > 0 ? 'bg-rose-500/20 text-rose-400' : 'bg-emerald-500/20 text-emerald-400'
                        }`}>
                          {formatNumber(brand.LowStockCount)}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {/* Sales Tab */}
      {activeTab === 'sales' && (
        <div className="space-y-6">
          {/* Sales Stats */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Total Revenue</p>
                  <h3 className="text-2xl font-bold text-white">{formatCurrency(salesSummary?.TotalRevenue)}</h3>
                </div>
                <div className="w-12 h-12 bg-emerald-500/20 rounded-2xl flex items-center justify-center border border-emerald-500/20">
                  <DollarSign className="w-6 h-6 text-emerald-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Total Invoices</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(salesSummary?.TotalInvoices)}</h3>
                </div>
                <div className="w-12 h-12 bg-blue-500/20 rounded-2xl flex items-center justify-center border border-blue-500/20">
                  <FileText className="w-6 h-6 text-blue-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Avg Invoice</p>
                  <h3 className="text-2xl font-bold text-white">{formatCurrency(salesSummary?.AverageInvoiceValue)}</h3>
                </div>
                <div className="w-12 h-12 bg-violet-500/20 rounded-2xl flex items-center justify-center border border-violet-500/20">
                  <ShoppingCart className="w-6 h-6 text-violet-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Unique Products</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(salesSummary?.UniqueProductsSold)}</h3>
                </div>
                <div className="w-12 h-12 bg-amber-500/20 rounded-2xl flex items-center justify-center border border-amber-500/20">
                  <Package className="w-6 h-6 text-amber-400" />
                </div>
              </div>
            </div>
          </div>

          {/* Daily Trend Line Chart */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <div className="w-1 h-5 bg-emerald-500 rounded-full"></div>
                Daily Sales Trend (Last {dateRange} Days)
              </h3>
              <button 
                onClick={exportDailyTrends}
                className="flex items-center gap-2 px-4 py-2 bg-slate-700/50 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors border border-white/5"
              >
                <Download className="w-4 h-4" />
                Export CSV
              </button>
            </div>
            <ResponsiveContainer width="100%" height={350}>
              <LineChart data={dailyTrends}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" vertical={false} />
                <XAxis 
                  dataKey="Date" 
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickLine={false}
                  axisLine={false}
                  dy={10}
                />
                <YAxis 
                  yAxisId="left"
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`}
                  tickLine={false}
                  axisLine={false}
                  dx={-10}
                />
                <YAxis 
                  yAxisId="right"
                  orientation="right"
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickLine={false}
                  axisLine={false}
                  dx={10}
                />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: '#1e293b', 
                    border: '1px solid rgba(255,255,255,0.1)',
                    borderRadius: '12px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                    color: '#fff'
                  }}
                  formatter={(value: any, name: any) => {
                    if (name === 'Revenue') return [`Rs ${formatCurrency(value)}`, 'Revenue'];
                    return [value, 'Invoices'];
                  }}
                  labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                />
                <Legend wrapperStyle={{ paddingTop: '20px', color: '#fff' }} />
                <Line 
                  yAxisId="left"
                  type="monotone" 
                  dataKey="TotalRevenue" 
                  stroke="#10b981" 
                  strokeWidth={3}
                  dot={{ fill: '#1e293b', stroke: '#10b981', strokeWidth: 2, r: 4 }}
                  activeDot={{ r: 6, fill: '#10b981' }}
                  name="Revenue"
                />
                <Line 
                  yAxisId="right"
                  type="monotone" 
                  dataKey="InvoiceCount" 
                  stroke="#3b82f6" 
                  strokeWidth={3}
                  dot={{ fill: '#1e293b', stroke: '#3b82f6', strokeWidth: 2, r: 4 }}
                  activeDot={{ r: 6, fill: '#3b82f6' }}
                  name="Invoices"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* Top Products Table */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white">Top Selling Products</h3>
              <div className="flex items-center gap-3">
                <div className="relative">
                  <input
                    type="text"
                    placeholder="Search products..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10 pr-4 py-2.5 bg-slate-700/50 border border-white/10 rounded-xl text-sm text-white focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 w-64 transition-all placeholder-slate-400"
                  />
                  <Search className="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
                </div>
                <button 
                  onClick={exportSalesData}
                  className="flex items-center gap-2 px-4 py-2.5 bg-slate-700/50 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors border border-white/5"
                >
                  <Download className="w-4 h-4" />
                  Export
                </button>
              </div>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-white/10">
                    <th className="text-left text-slate-400 font-medium text-sm py-4 px-4">Rank</th>
                    <th className="text-left text-slate-400 font-medium text-sm py-4 px-4">Product</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Invoices</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Qty Sold</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Total Revenue</th>
                    <th className="text-right text-slate-400 font-medium text-sm py-4 px-4">Avg Revenue</th>
                  </tr>
                </thead>
                <tbody>
                  {topProducts
                    .filter(product => 
                      product.Description.toLowerCase().includes(searchTerm.toLowerCase())
                    )
                    .map((product, index) => (
                    <tr key={product.ItemId} className="border-b border-white/5 hover:bg-white/5 transition-colors">
                      <td className="py-4 px-4">
                        <div className="w-8 h-8 bg-indigo-500/20 rounded-lg flex items-center justify-center border border-indigo-500/20">
                          <span className="text-indigo-400 font-bold text-sm">{index + 1}</span>
                        </div>
                      </td>
                      <td className="py-4 px-4 text-white font-medium text-sm max-w-md truncate">{product.Description}</td>
                      <td className="py-4 px-4 text-right text-slate-300 text-sm">{formatNumber(product.InvoiceCount)}</td>
                      <td className="py-4 px-4 text-right text-indigo-400 font-medium text-sm">{formatNumber(product.TotalQuantitySold)}</td>
                      <td className="py-4 px-4 text-right text-emerald-400 font-bold text-sm">{formatCurrency(product.TotalRevenue)}</td>
                      <td className="py-4 px-4 text-right text-slate-300 text-sm">{formatCurrency(product.AvgRevenue)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {/* Finance Tab */}
      {activeTab === 'finance' && (
        <div className="space-y-6">
          {/* Financial Stats */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between mb-4">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Current Month</p>
                  <h3 className="text-2xl font-bold text-white">{formatCurrency(financialSummary?.CurrentMonthRevenue)}</h3>
                </div>
                <div className="w-12 h-12 bg-emerald-500/20 rounded-2xl flex items-center justify-center border border-emerald-500/20">
                  <TrendingUp className="w-6 h-6 text-emerald-400" />
                </div>
              </div>
              <div className="flex items-center gap-2 text-sm px-3 py-1.5 bg-slate-700/50 rounded-lg w-fit border border-white/5">
                {financialSummary && financialSummary.CurrentMonthRevenue > financialSummary.LastMonthRevenue ? (
                  <>
                    <TrendingUp className="w-4 h-4 text-emerald-400" />
                    <span className="text-emerald-400 font-medium">
                      {((((financialSummary.CurrentMonthRevenue - financialSummary.LastMonthRevenue) / financialSummary.LastMonthRevenue) * 100) || 0).toFixed(1)}% from last month
                    </span>
                  </>
                ) : (
                  <>
                    <TrendingDown className="w-4 h-4 text-rose-400" />
                    <span className="text-rose-400 font-medium">
                      {((((financialSummary?.LastMonthRevenue || 0) - (financialSummary?.CurrentMonthRevenue || 0)) / (financialSummary?.LastMonthRevenue || 1) * 100) || 0).toFixed(1)}% from last month
                    </span>
                  </>
                )}
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Year to Date</p>
                  <h3 className="text-2xl font-bold text-white">{formatCurrency(financialSummary?.YearToDateRevenue)}</h3>
                </div>
                <div className="w-12 h-12 bg-blue-500/20 rounded-2xl flex items-center justify-center border border-blue-500/20">
                  <Calendar className="w-6 h-6 text-blue-400" />
                </div>
              </div>
            </div>
            <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-xl border border-white/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-slate-400 text-sm font-medium mb-1">Active Customers</p>
                  <h3 className="text-3xl font-bold text-white">{formatNumber(financialSummary?.ActiveCustomersThisMonth)}</h3>
                </div>
                <div className="w-12 h-12 bg-violet-500/20 rounded-2xl flex items-center justify-center border border-violet-500/20">
                  <Users className="w-6 h-6 text-violet-400" />
                </div>
              </div>
            </div>
          </div>

          {/* 12-Month Revenue Trend */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <div className="w-1 h-5 bg-blue-500 rounded-full"></div>
                12-Month Revenue Analysis
              </h3>
              <button 
                onClick={exportMonthlyData}
                className="flex items-center gap-2 px-4 py-2 bg-slate-700/50 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors border border-white/5"
              >
                <Download className="w-4 h-4" />
                Export Report
              </button>
            </div>
            <ResponsiveContainer width="100%" height={400}>
              <BarChart data={monthlyData}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" vertical={false} />
                <XAxis 
                  dataKey="Month" 
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickLine={false}
                  axisLine={false}
                  dy={10}
                />
                <YAxis 
                  stroke="#94a3b8"
                  tick={{ fill: '#94a3b8', fontSize: 12 }}
                  tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`}
                  tickLine={false}
                  axisLine={false}
                  dx={-10}
                />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: '#1e293b', 
                    border: '1px solid rgba(255,255,255,0.1)',
                    borderRadius: '12px',
                    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                    color: '#fff'
                  }}
                  formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Revenue']}
                  labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                  cursor={{ fill: 'rgba(255,255,255,0.05)' }}
                />
                <Bar dataKey="TotalRevenue" radius={[6, 6, 0, 0]}>
                  {monthlyData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={`hsl(${220 + index * 10}, 70%, 55%)`} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* Month Comparison */}
          <div className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-8 shadow-xl border border-white/10">
            <h3 className="text-lg font-bold text-white mb-6 flex items-center gap-2">
              <div className="w-1 h-5 bg-indigo-500 rounded-full"></div>
              Monthly Comparison
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="bg-slate-700/30 rounded-2xl p-6 border border-white/5">
                <div className="flex items-center justify-between mb-4">
                  <h4 className="text-base font-bold text-white">Current Month</h4>
                  <div className="w-10 h-10 bg-emerald-500/20 rounded-xl flex items-center justify-center border border-emerald-500/20">
                    <Calendar className="w-5 h-5 text-emerald-400" />
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <p className="text-slate-400 text-sm font-medium">Total Revenue</p>
                    <p className="text-2xl font-bold text-emerald-400">{formatCurrency(financialSummary?.CurrentMonthRevenue)}</p>
                  </div>
                  <div>
                    <p className="text-slate-400 text-sm font-medium">Active Customers</p>
                    <p className="text-xl font-bold text-white">{formatNumber(financialSummary?.ActiveCustomersThisMonth)}</p>
                  </div>
                </div>
              </div>
              <div className="bg-slate-700/30 rounded-2xl p-6 border border-white/5">
                <div className="flex items-center justify-between mb-4">
                  <h4 className="text-base font-bold text-white">Last Month</h4>
                  <div className="w-10 h-10 bg-blue-500/20 rounded-xl flex items-center justify-center border border-blue-500/20">
                    <Calendar className="w-5 h-5 text-blue-400" />
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <p className="text-slate-400 text-sm font-medium">Total Revenue</p>
                    <p className="text-2xl font-bold text-blue-400">{formatCurrency(financialSummary?.LastMonthRevenue)}</p>
                  </div>
                  <div>
                    <p className="text-slate-400 text-sm font-medium">Growth Rate</p>
                    <div className="flex items-center gap-2">
                      {financialSummary && financialSummary.CurrentMonthRevenue > financialSummary.LastMonthRevenue ? (
                        <>
                          <TrendingUp className="w-5 h-5 text-emerald-400" />
                          <p className="text-xl font-bold text-emerald-400">
                            +{((((financialSummary.CurrentMonthRevenue - financialSummary.LastMonthRevenue) / financialSummary.LastMonthRevenue) * 100) || 0).toFixed(1)}%
                          </p>
                        </>
                      ) : (
                        <>
                          <TrendingDown className="w-5 h-5 text-rose-400" />
                          <p className="text-xl font-bold text-rose-400">
                            -{((((financialSummary?.LastMonthRevenue || 0) - (financialSummary?.CurrentMonthRevenue || 0)) / (financialSummary?.LastMonthRevenue || 1) * 100) || 0).toFixed(1)}%
                          </p>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
