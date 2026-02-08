'use client';

import React, { useState, useEffect } from 'react';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell, LineChart, Line
} from 'recharts';
import { 
  Download, RefreshCw, FileText, FileSpreadsheet, Mail, 
  TrendingUp, TrendingDown, DollarSign, Users, CreditCard,
  AlertCircle, CheckCircle, Calendar
} from 'lucide-react';
import { exportToPDF, exportToExcel } from '@/lib/reportExport';

// Types
interface AgingBucket {
  current: number;
  days_1_30: number;
  days_31_60: number;
  days_61_90: number;
  over_90: number;
  total: number;
}

interface CashAccount {
  AccountID: string;
  AccountName: string;
  AccountType: string;
  Balance: number;
}

interface Customer {
  CustomerID: string;
  CustomerName: string;
  CurrentBalance: number;
  CreditLimit: number;
}

interface BusinessData {
  arAging: AgingBucket;
  apAging: AgingBucket;
  cashBalances: CashAccount[];
  topCustomers: Customer[];
  totalCash: number;
  lastUpdated: string;
}

export default function BusinessStatusReports() {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<BusinessData | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState('overview');
  const [exporting, setExporting] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      // In a real app, this would be an API call
      // const response = await fetch('/api/reports/business-status');
      // const result = await response.json();
      
      // Simulating API call to Peachtree Bridge
      const bridgeUrl = process.env.NEXT_PUBLIC_PEACHTREE_BRIDGE_URL || 'http://localhost:3001';
      
      // For demo purposes, we'll use mock data if bridge is not available
      // or if we're in development mode without the bridge running
      
      // Mock data structure matching Peachtree output
      const mockData: BusinessData = {
        arAging: {
          current: 1250000,
          days_1_30: 850000,
          days_31_60: 450000,
          days_61_90: 120000,
          over_90: 350000,
          total: 3020000
        },
        apAging: {
          current: 950000,
          days_1_30: 650000,
          days_31_60: 250000,
          days_61_90: 50000,
          over_90: 0,
          total: 1900000
        },
        cashBalances: [
          { AccountID: '10100', AccountName: 'Petty Cash', AccountType: 'Cash', Balance: 45000 },
          { AccountID: '10200', AccountName: 'Sampath Bank', AccountType: 'Bank', Balance: 1250000 },
          { AccountID: '10300', AccountName: 'Commercial Bank', AccountType: 'Bank', Balance: 850000 },
          { AccountID: '10400', AccountName: 'HNB', AccountType: 'Bank', Balance: 320000 }
        ],
        topCustomers: [
          { CustomerID: 'C001', CustomerName: 'Lanka Logistics', CurrentBalance: 450000, CreditLimit: 500000 },
          { CustomerID: 'C005', CustomerName: 'City Transport', CurrentBalance: 320000, CreditLimit: 400000 },
          { CustomerID: 'C012', CustomerName: 'ABC Construction', CurrentBalance: 280000, CreditLimit: 300000 },
          { CustomerID: 'C008', CustomerName: 'Metro Cabs', CurrentBalance: 150000, CreditLimit: 200000 },
          { CustomerID: 'C022', CustomerName: 'Road Runners', CurrentBalance: 120000, CreditLimit: 150000 }
        ],
        totalCash: 2465000,
        lastUpdated: new Date().toISOString()
      };

      // Simulate network delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setData(mockData);
    } catch (err) {
      console.error('Error fetching business status:', err);
      setError('Failed to load business status data. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleExport = async (type: 'pdf' | 'excel') => {
    if (!data) return;
    
    setExporting(true);
    try {
      if (type === 'pdf') {
        await exportToPDF(data, activeTab);
      } else {
        await exportToExcel(data, activeTab);
      }
    } catch (err) {
      console.error('Export failed:', err);
      alert('Export failed. See console for details.');
    } finally {
      setExporting(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(amount);
  };

  // Chart Data Preparation
  const getAgingChartData = () => {
    if (!data) return [];
    return [
      { name: 'Current', AR: data.arAging.current, AP: data.apAging.current },
      { name: '1-30 Days', AR: data.arAging.days_1_30, AP: data.apAging.days_1_30 },
      { name: '31-60 Days', AR: data.arAging.days_31_60, AP: data.apAging.days_31_60 },
      { name: '61-90 Days', AR: data.arAging.days_61_90, AP: data.apAging.days_61_90 },
      { name: '> 90 Days', AR: data.arAging.over_90, AP: data.apAging.over_90 },
    ];
  };

  const getCashChartData = () => {
    if (!data) return [];
    return data.cashBalances.map(acc => ({
      name: acc.AccountName,
      value: acc.Balance
    }));
  };

  const COLORS = ['#6366f1', '#8b5cf6', '#10b981', '#f59e0b', '#ec4899'];

  if (loading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border-l-4 border-red-500 p-4 m-4 rounded-r-xl">
        <div className="flex">
          <div className="flex-shrink-0">
            <AlertCircle className="h-5 w-5 text-red-500" />
          </div>
          <div className="ml-3">
            <p className="text-sm text-red-700">{error}</p>
            <button 
              onClick={fetchData}
              className="mt-2 text-sm font-medium text-red-700 hover:text-red-600 underline"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (!data) return null;

  return (
    <div className="space-y-6">
      {/* Header Controls */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-white p-6 rounded-[2rem] shadow-sm border border-zinc-100">
        <div>
          <h2 className="text-lg font-semibold text-zinc-900">Financial Overview</h2>
          <p className="text-sm text-zinc-500">Last updated: {new Date(data.lastUpdated).toLocaleString()}</p>
        </div>
        <div className="flex gap-2">
          <button 
            onClick={fetchData}
            className="flex items-center gap-2 px-4 py-2 bg-zinc-50 hover:bg-zinc-100 text-zinc-700 rounded-xl text-sm transition-colors font-medium"
          >
            <RefreshCw size={16} />
            Refresh
          </button>
          <div className="relative group">
            <button 
              className="flex items-center gap-2 px-4 py-2 bg-indigo-50 hover:bg-indigo-100 text-indigo-700 rounded-xl text-sm transition-colors font-medium"
            >
              <Download size={16} />
              Export
            </button>
            <div className="absolute right-0 mt-2 w-48 bg-white rounded-xl shadow-lg border border-zinc-100 hidden group-hover:block z-10 overflow-hidden">
              <div className="py-1">
                <button 
                  onClick={() => handleExport('pdf')}
                  disabled={exporting}
                  className="flex items-center gap-2 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50 w-full text-left"
                >
                  <FileText size={16} className="text-red-500" />
                  Export as PDF
                </button>
                <button 
                  onClick={() => handleExport('excel')}
                  disabled={exporting}
                  className="flex items-center gap-2 px-4 py-2 text-sm text-zinc-700 hover:bg-zinc-50 w-full text-left"
                >
                  <FileSpreadsheet size={16} className="text-emerald-500" />
                  Export as Excel
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Key Metrics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white p-6 rounded-[2rem] shadow-sm border border-zinc-100">
          <div className="flex justify-between items-start">
            <div>
              <p className="text-sm font-medium text-zinc-500">Total Receivables</p>
              <h3 className="text-2xl font-bold text-zinc-900 mt-1">{formatCurrency(data.arAging.total)}</h3>
            </div>
            <div className="p-3 bg-emerald-50 rounded-xl">
              <TrendingUp className="h-5 w-5 text-emerald-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-red-500 font-medium">{formatCurrency(data.arAging.over_90)}</span>
            <span className="text-zinc-500 ml-2">overdue &gt; 90 days</span>
          </div>
        </div>

        <div className="bg-white p-6 rounded-[2rem] shadow-sm border border-zinc-100">
          <div className="flex justify-between items-start">
            <div>
              <p className="text-sm font-medium text-zinc-500">Total Payables</p>
              <h3 className="text-2xl font-bold text-zinc-900 mt-1">{formatCurrency(data.apAging.total)}</h3>
            </div>
            <div className="p-3 bg-red-50 rounded-xl">
              <TrendingDown className="h-5 w-5 text-red-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-emerald-500 font-medium">Net Position:</span>
            <span className={`ml-2 font-bold ${data.arAging.total - data.apAging.total >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>
              {formatCurrency(data.arAging.total - data.apAging.total)}
            </span>
          </div>
        </div>

        <div className="bg-white p-6 rounded-[2rem] shadow-sm border border-zinc-100">
          <div className="flex justify-between items-start">
            <div>
              <p className="text-sm font-medium text-zinc-500">Cash Position</p>
              <h3 className="text-2xl font-bold text-zinc-900 mt-1">{formatCurrency(data.totalCash)}</h3>
            </div>
            <div className="p-3 bg-indigo-50 rounded-xl">
              <DollarSign className="h-5 w-5 text-indigo-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-zinc-500">{data.cashBalances.length} active accounts</span>
          </div>
        </div>

        <div className="bg-white p-6 rounded-[2rem] shadow-sm border border-zinc-100">
          <div className="flex justify-between items-start">
            <div>
              <p className="text-sm font-medium text-zinc-500">Top Customer Exposure</p>
              <h3 className="text-2xl font-bold text-zinc-900 mt-1">
                {data.topCustomers.length > 0 ? formatCurrency(data.topCustomers[0].CurrentBalance) : '0'}
              </h3>
            </div>
            <div className="p-3 bg-violet-50 rounded-xl">
              <Users className="h-5 w-5 text-violet-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center text-sm">
            <span className="text-zinc-500 truncate max-w-[150px]">
              {data.topCustomers.length > 0 ? data.topCustomers[0].CustomerName : 'No Data'}
            </span>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-zinc-200">
        <nav className="-mb-px flex space-x-8">
          {['overview', 'ar-aging', 'ap-aging', 'cash-balances'].map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`
                whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm capitalize transition-colors
                ${activeTab === tab
                  ? 'border-indigo-500 text-indigo-600'
                  : 'border-transparent text-zinc-500 hover:text-zinc-700 hover:border-zinc-300'}
              `}
            >
              {tab.replace('-', ' ')}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab Content */}
      <div className="bg-white p-8 rounded-[2rem] shadow-sm border border-zinc-100 min-h-[400px]">
        {activeTab === 'overview' && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div>
              <h3 className="text-lg font-medium text-zinc-900 mb-4">AR vs AP Aging Comparison</h3>
              <div className="h-80">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={getAgingChartData()}
                    margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#f3f4f6" />
                    <XAxis dataKey="name" stroke="#71717a" fontSize={12} tickLine={false} axisLine={false} />
                    <YAxis stroke="#71717a" fontSize={12} tickLine={false} axisLine={false} />
                    <Tooltip 
                      formatter={(value) => formatCurrency(value as number)}
                      contentStyle={{ borderRadius: '1rem', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                    />
                    <Legend />
                    <Bar dataKey="AR" fill="#10b981" name="Receivables" radius={[4, 4, 0, 0]} />
                    <Bar dataKey="AP" fill="#ef4444" name="Payables" radius={[4, 4, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>
            <div>
              <h3 className="text-lg font-medium text-zinc-900 mb-4">Cash Distribution</h3>
              <div className="h-80">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={getCashChartData()}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }: { name?: string, percent?: number }) => `${name || ''} ${((percent || 0) * 100).toFixed(0)}%`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {getCashChartData().map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip 
                      formatter={(value) => formatCurrency(value as number)}
                      contentStyle={{ borderRadius: '1rem', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'ar-aging' && (
          <div>
            <h3 className="text-lg font-medium text-zinc-900 mb-4">Accounts Receivable Aging</h3>
            <div className="overflow-hidden rounded-xl border border-zinc-100">
              <table className="min-w-full divide-y divide-zinc-200">
                <thead className="bg-zinc-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-zinc-500 uppercase tracking-wider">Category</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">Amount</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">% of Total</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-zinc-200">
                  {[
                    { label: 'Current', value: data.arAging.current },
                    { label: '1-30 Days', value: data.arAging.days_1_30 },
                    { label: '31-60 Days', value: data.arAging.days_31_60 },
                    { label: '61-90 Days', value: data.arAging.days_61_90 },
                    { label: 'Over 90 Days', value: data.arAging.over_90 },
                  ].map((row, idx) => (
                    <tr key={idx} className={idx % 2 === 0 ? 'bg-white' : 'bg-zinc-50/50'}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-zinc-900">{row.label}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-500">{formatCurrency(row.value)}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-500">
                        {((row.value / data.arAging.total) * 100).toFixed(1)}%
                      </td>
                    </tr>
                  ))}
                  <tr className="bg-zinc-100 font-bold">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-zinc-900">Total</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-900">{formatCurrency(data.arAging.total)}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-900">100.0%</td>
                  </tr>
                </tbody>
              </table>
            </div>
            
            <div className="mt-8">
              <h3 className="text-lg font-medium text-zinc-900 mb-4">Top Customers by Outstanding Balance</h3>
              <div className="overflow-hidden rounded-xl border border-zinc-100">
                <table className="min-w-full divide-y divide-zinc-200">
                  <thead className="bg-zinc-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-zinc-500 uppercase tracking-wider">Customer ID</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-zinc-500 uppercase tracking-wider">Name</th>
                      <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">Balance</th>
                      <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">Credit Limit</th>
                      <th className="px-6 py-3 text-center text-xs font-medium text-zinc-500 uppercase tracking-wider">Status</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-zinc-200">
                    {data.topCustomers.map((customer) => (
                      <tr key={customer.CustomerID}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-zinc-500">{customer.CustomerID}</td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-zinc-900">{customer.CustomerName}</td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-900">{formatCurrency(customer.CurrentBalance)}</td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-500">{formatCurrency(customer.CreditLimit)}</td>
                        <td className="px-6 py-4 whitespace-nowrap text-center">
                          {customer.CurrentBalance > customer.CreditLimit ? (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800">
                              Over Limit
                            </span>
                          ) : (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-emerald-100 text-emerald-800">
                              Good
                            </span>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'ap-aging' && (
          <div>
            <h3 className="text-lg font-medium text-zinc-900 mb-4">Accounts Payable Aging</h3>
            <div className="overflow-hidden rounded-xl border border-zinc-100">
              <table className="min-w-full divide-y divide-zinc-200">
                <thead className="bg-zinc-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-zinc-500 uppercase tracking-wider">Category</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">Amount</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-zinc-500 uppercase tracking-wider">% of Total</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-zinc-200">
                  {[
                    { label: 'Current', value: data.apAging.current },
                    { label: '1-30 Days', value: data.apAging.days_1_30 },
                    { label: '31-60 Days', value: data.apAging.days_31_60 },
                    { label: '61-90 Days', value: data.apAging.days_61_90 },
                    { label: 'Over 90 Days', value: data.apAging.over_90 },
                  ].map((row, idx) => (
                    <tr key={idx} className={idx % 2 === 0 ? 'bg-white' : 'bg-zinc-50/50'}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-zinc-900">{row.label}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-500">{formatCurrency(row.value)}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-500">
                        {((row.value / data.apAging.total) * 100).toFixed(1)}%
                      </td>
                    </tr>
                  ))}
                  <tr className="bg-zinc-100 font-bold">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-zinc-900">Total</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-900">{formatCurrency(data.apAging.total)}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right text-zinc-900">100.0%</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        )}

        {activeTab === 'cash-balances' && (
          <div>
            <h3 className="text-lg font-medium text-zinc-900 mb-4">Cash & Bank Balances</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
              {data.cashBalances.map((account) => (
                <div key={account.AccountID} className="bg-white border border-zinc-100 rounded-2xl p-6 shadow-sm flex items-center justify-between">
                  <div className="flex items-center gap-4">
                    <div className={`p-3 rounded-xl ${account.AccountType === 'Cash' ? 'bg-emerald-100 text-emerald-600' : 'bg-indigo-100 text-indigo-600'}`}>
                      {account.AccountType === 'Cash' ? <DollarSign size={24} /> : <CreditCard size={24} />}
                    </div>
                    <div>
                      <p className="text-sm text-zinc-500">{account.AccountID} - {account.AccountType}</p>
                      <h4 className="text-lg font-bold text-zinc-900">{account.AccountName}</h4>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-xl font-bold text-zinc-900">{formatCurrency(account.Balance)}</p>
                  </div>
                </div>
              ))}
            </div>
            
            <div className="bg-indigo-50 p-6 rounded-2xl border border-indigo-100">
              <div className="flex justify-between items-center">
                <span className="text-lg font-medium text-indigo-900">Total Liquid Assets</span>
                <span className="text-2xl font-bold text-indigo-900">{formatCurrency(data.totalCash)}</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
