'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import { 
  Users, Building2, DollarSign, FileText, TrendingUp, 
  Package, Home, RefreshCw, Download, Search, Filter,
  Clock, BarChart3, PieChart, Calendar, ShoppingCart,
  CreditCard, Wallet, Receipt, AlertCircle, CheckCircle,
  XCircle, ArrowUpRight, ArrowDownRight, Activity, Database, X
} from 'lucide-react';
import Link from 'next/link';
import {
  LineChart, Line, BarChart, Bar, PieChart as RechartsPie, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  AreaChart, Area, RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar
} from 'recharts';
import { 
  exportInvoiceDetailsPDF, 
  exportArApAgingPDF, 
  exportFinancialReportPDF 
} from '@/utils/pdfExports';

// Types
interface PeachtreeTable {
  name: string;
  type: string;
}

interface Customer {
  CustomerID: string;
  CustomerName: string;
  Contact: string;
  Phone: string;
  Balance: number;
  CreditLimit: number;
  Status: string;
}

interface Vendor {
  VendorID: string;
  VendorName: string;
  Contact: string;
  Phone: string;
  Balance: number;
  Status: string;
  CreditPeriod?: string;
  Warehouse?: string;
}

interface GLAccount {
  AccountID: string;
  AccountName: string;
  AccountType: string;
  Balance: number;
  Active: boolean;
}

interface Transaction {
  TransactionID: string;
  Date: string;
  Type: string;
  Description: string;
  Amount: number;
  Status: string;
}

interface DashboardStats {
  totalCustomers: number;
  totalVendors: number;
  totalAccounts: number;
  totalTransactions: number;
  activeCustomers: number;
  activeVendors: number;
  totalRevenue: number;
  totalExpenses: number;
  dataCompleteness: {
    customers: number;
    vendors: number;
    accounts: number;
  };
}

export default function PeachtreeDashboard({ isModal, onClose }: { isModal?: boolean; onClose?: () => void }) {
  const router = useRouter();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [activeTab, setActiveTab] = useState<'dashboard' | 'customers' | 'vendors' | 'accounts' | 'transactions' | 'reports' | 'analytics' | 'financial' | 'ar-ap-aging' | 'inventory' | 'jobs' | 'payroll' | 'cashflow'>('dashboard');
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [connected, setConnected] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // 🆕 Peachtree Bridge Status
  const [bridgeStatus, setBridgeStatus] = useState<'running'|'stopped'|'checking'>('checking');
  const [bridgePid, setBridgePid] = useState<number|null>(null);
  const [bridgeLogs, setBridgeLogs] = useState<string[]>([]);
  const [showLogs, setShowLogs] = useState(false);
  const [startingBridge, setStartingBridge] = useState(false);
  
  // Filter states (Enhanced)
  const [hideZeroBalance, setHideZeroBalance] = useState(false); // 🔄 Changed to false - Show ALL customers/vendors
  const [hideInactive, setHideInactive] = useState(false);
  const [showBankAccountsOnly, setShowBankAccountsOnly] = useState(true); // 🆕 New filter for Bank Accounts (Default: true)
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [sortBy, setSortBy] = useState<'name' | 'balance' | 'date'>('name');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');
  
  // 🆕 Selected item for invoice details popup
  const [selectedCustomerForInvoice, setSelectedCustomerForInvoice] = useState<any>(null);
  const [selectedVendorForInvoice, setSelectedVendorForInvoice] = useState<any>(null);
  const [invoiceDetails, setInvoiceDetails] = useState<any>(null);
  const [loadingInvoices, setLoadingInvoices] = useState(false);
  
  // Business Status data
  const [arAging, setArAging] = useState<any>(null);
  const [apAging, setApAging] = useState<any>(null);
  const [cashBalances, setCashBalances] = useState<any>(null);
  const [topCustomers, setTopCustomers] = useState<any[]>([]);
  const [topVendors, setTopVendors] = useState<any[]>([]);
  
  // Data states
  const [tables, setTables] = useState<PeachtreeTable[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [vendors, setVendors] = useState<Vendor[]>([]);
  const [accounts, setAccounts] = useState<GLAccount[]>([]);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [stats, setStats] = useState<DashboardStats>({
    totalCustomers: 0,
    totalVendors: 0,
    totalAccounts: 0,
    totalTransactions: 0,
    activeCustomers: 0,
    activeVendors: 0,
    totalRevenue: 0,
    totalExpenses: 0,
    dataCompleteness: {
      customers: 100,
      vendors: 100,
      accounts: 100
    }
  });

  // Chart colors
  const CHART_COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899', '#06b6d4', '#84cc16'];

  useEffect(() => {
    const verifyAuth = async () => {
      try {
        const auth = await checkAuth();
        if (!auth.authenticated) {
          console.log('[ERP] Not authenticated, redirecting to login...');
          setIsAuthenticated(false);
          router.replace('/');
          return;
        }
        console.log('[ERP] Authenticated, loading data...');
        setIsAuthenticated(true);
      } catch (error) {
        console.error('[ERP] Auth check error:', error);
        setIsAuthenticated(false);
        router.replace('/');
      }
    };
    verifyAuth();
  }, [router]);

  // 🆕 Bridge Status Polling
  useEffect(() => {
    const checkBridge = async () => {
      try {
        const res = await fetch('/api/peachtree-bridge');
        if (!res.ok) throw new Error('Bridge API failed');
        const data = await res.json();
        setBridgeStatus(data.status);
        setBridgePid(data.pid || null);
      } catch (error) {
        console.error('[Bridge Status] Check failed:', error);
        setBridgeStatus('stopped');
        setBridgePid(null);
      }
    };
    
    checkBridge(); // Initial check
    const interval = setInterval(checkBridge, 5000); // Poll every 5 seconds
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (isAuthenticated) {
      loadAllData();
    }
  }, [isAuthenticated]);

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
  }, [autoRefresh]);

  const loadAllData = async () => {
    setLoading(true);
    setError(null);
    
    try {
      // 🔍 First check if bridge is actually running
      const bridgeStatusRes = await fetch('/api/peachtree-bridge');
      const bridgeStatus = await bridgeStatusRes.json();
      
      if (bridgeStatus.status !== 'running') {
        // Clear any previously loaded data to avoid showing stale/cached results
        setTables([]);
        setCustomers([]);
        setVendors([]);
        setAccounts([]);
        setTransactions([]);
        setTopCustomers([]);
        setTopVendors([]);
        setArAging(null);
        setApAging(null);
        setCashBalances(null);
        setConnected(false);
        setError('Peachtree bridge not running - Click "Connect Now" to start');
        setLoading(false);
        return;
      }

      // 🔍 Check ODBC connection health
      const healthRes = await fetch('/api/erp/peachtree?endpoint=health');
      if (!healthRes.ok) {
        throw new Error('Peachtree ODBC not connected');
      }
      const healthData = await healthRes.json();
      setConnected(healthData.status === 'ok' || healthData.success === true);

      // ✅ Load all data in parallel - FRESH from bridge
      await Promise.all([
        loadTables(),
        loadCustomers(),
        loadVendors(),
        loadAccounts(),
        loadTransactions(),
        loadBusinessStatusData()
      ]);

      calculateStats();
    } catch (error: any) {
      console.error('Error loading Peachtree data:', error);
      setError(error.message || 'Failed to connect to Peachtree');
      setConnected(false);
    } finally {
      setLoading(false);
    }
  };

  const loadTables = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=tables&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.tables) {
          setTables(data.tables.map((name: string) => ({
            name,
            type: 'TABLE'
          })));
        }
      }
    } catch (error) {
      console.error('Error loading tables:', error);
    }
  };

  const loadCustomers = async () => {
    try {
      // Always fetch fresh customer data from bridge (no-cache)
      const res = await fetch(`/api/erp/peachtree?endpoint=customers&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });

      
      if (res.ok) {
        const data = await res.json();
        console.log('🔍 [DEBUG] Customers data:', {
          success: data.success,
          count: data.count,
          dataLength: data.data?.length,
          firstRecord: data.data?.[0]
        });
        
        if (data.success && data.data) {
          // Transform to match our interface with actual Peachtree column names
          const transformedData = data.data.map((row: any, index: number) => {
            // Try multiple fields for customer name
            const customerName = row.Customer_Bill_Name?.trim() || 
                                row.CustomerName?.trim() || 
                                row.CompanyName?.trim() || 
                                row.CustomerID?.trim() || 
                                `Customer ${index + 1}`;
            
            return {
              CustomerID: row.CustomerID || `CUST-${index + 1}`,
              CustomerName: customerName,
              Contact: row.Contact?.trim() || row.ContactPerson?.trim() || '-',
              Phone: row.Phone_Number?.trim() || row.PhoneNumber2?.trim() || row.Phone1?.trim() || '-',
              Balance: parseFloat(row.Balance || 0),
              CreditLimit: parseFloat(row.Terms_CreditLimit || 0),
              // If CustomerIsInactive is not 0 but has balance, still show as Active
              Status: (row.CustomerIsInactive === 0 || parseFloat(row.Balance || 0) > 0) ? 'Active' : 'Inactive'
            };
          });
          


          setCustomers(transformedData);

        }
      }
    } catch (error) {
      console.error('❌ [DEBUG] Error loading customers:', error);
    }
  };

  const loadVendors = async () => {
    try {
      // Always fetch fresh vendor data from bridge (no-cache)
      const res = await fetch(`/api/erp/peachtree?endpoint=vendors&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });

      
      if (res.ok) {
        const data = await res.json();
        console.log('🔍 [DEBUG] Vendors data:', {
          success: data.success,
          count: data.count,
          dataLength: data.data?.length,
          firstRecord: data.data?.[0]
        });
        
        if (data.success && data.data) {
          const transformedData = data.data.map((row: any, index: number) => {
            // Try multiple fields for vendor name
            const vendorName = row.Vendor_Bill_Name?.trim() || 
                              row.VendorName?.trim() || 
                              row.CompanyName?.trim() || 
                              row.VendorID?.trim() || 
                              `Vendor ${index + 1}`;
            
            // Check both VendorInactive and VendorIsInactive field names
            const isInactive = row.VendorInactive || row.VendorIsInactive || 0;
            
            // Map credit periods based on vendor type (tire suppliers typically 30-60 days)
            const creditPeriod = row.Terms_CreditDays || row.CreditDays || '30 Days';
            
            // Map warehouse based on vendor ID or default
            const warehouse = row.Warehouse || row.DefaultWarehouse || 'Main Warehouse';
            
            return {
              VendorID: row.VendorID || `VEND-${index + 1}`,
              VendorName: vendorName,
              Contact: row.Contact?.trim() || row.ContactPerson?.trim() || '-',
              Phone: row.PhoneNumber?.trim() || row.PhoneNumber2?.trim() || row.Phone?.trim() || '-',
              Balance: parseFloat(row.Balance || 0),
              CreditPeriod: creditPeriod,
              Warehouse: warehouse,
              // If vendor has balance > 0, show as Active regardless of inactive flag
              Status: (isInactive === 0 || parseFloat(row.Balance || 0) > 0) ? 'Active' : 'Inactive'
            };
          });
          


          setVendors(transformedData);

        }
      }
    } catch (error) {
      console.error('❌ [DEBUG] Error loading vendors:', error);
    }
  };

  const loadAccounts = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=chart-of-accounts&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.data) {
          const transformedData = data.data.map((row: any, index: number) => {
            // Try multiple fields for account name/description
            const accountName = row.AccountDescription?.trim() ||
                               row.Description?.trim() || 
                               row.AccountName?.trim() || 
                               row.Account_Name?.trim() || 
                               row.AccountID?.trim() || 
                               `Account ${index + 1}`;
            
            // Better account type mapping
            const getAccountType = (typeNum: any) => {
              const types: Record<number, string> = {
                0: 'Cash',
                1: 'Accounts Receivable',
                2: 'Inventory',
                3: 'Other Assets',
                4: 'Fixed Assets',
                5: 'Accum. Depreciation',
                6: 'Other Current Assets',
                10: 'Accounts Payable',
                12: 'Other Current Liabilities',
                19: 'Equity-Retained Earnings',
                21: 'Income',
                23: 'Cost of Sales',
                24: 'Expenses'
              };
              const n = parseInt(typeNum);
              return types[n] || `Type ${typeNum}` || 'General';
            };
            
            return {
              AccountID: row.AccountID || `ACC-${index + 1}`,
              AccountName: accountName,
              AccountType: getAccountType(row.AccountType ?? row.Account_Type_Number),
              Balance: parseFloat(row.Balance0Net || row.Balance || 0),
              Active: (row.AccountIsInactive ?? row.Inactive) === 0
            };
          });
          setAccounts(transformedData);
        }
      }
    } catch (error) {
      console.error('Error loading accounts:', error);
    }
  };

  const loadTransactions = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=transactions&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.data) {
          const transformedData = data.data.map((row: any, index: number) => {
            // Try to extract transaction details from various possible fields
            const transactionID = row.TransactionID || row.ID || row.EntryNumber || row.JournalNumber || `TXN-${index + 1}`;
            const date = row.Date || row.TransactionDate || row.EntryDate || row.PostDate || '';
            const type = row.Type || row.TransactionType || row.JournalType || 'General';
            const description = row.Description?.trim() || row.Memo?.trim() || row.Reference?.trim() || 'Transaction';
            const amount = parseFloat(row.Amount || row.Debit || row.Credit || row.TotalAmount || 0);
            const status = row.Status || row.Posted === 1 ? 'Posted' : 'Draft';
            
            return {
              TransactionID: transactionID,
              Date: date,
              Type: type,
              Description: description,
              Amount: amount,
              Status: status
            };
          });
          setTransactions(transformedData);
        }
      }
    } catch (error) {
      console.error('Error loading transactions:', error);
    }
  };

  // 🆕 Business Status Data Loading Functions
  const loadBusinessStatusData = async () => {
    try {
      await Promise.all([
        loadARAging(),
        loadAPAging(),
        loadCashBalances(),
        loadTopCustomers(),
        loadTopVendors()
      ]);
    } catch (error) {
      console.error('Error loading business status data:', error);
    }
  };

  const loadARAging = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=business-status/ar-aging&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success) {
          setArAging(data);
        }
      }
    } catch (error) {
      console.error('Error loading AR aging:', error);
    }
  };

  const loadAPAging = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=business-status/ap-aging&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success) {
          setApAging(data);
        }
      }
    } catch (error) {
      console.error('Error loading AP aging:', error);
    }
  };

  const loadCashBalances = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=business-status/cash-balances&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success) {
          setCashBalances(data);
        }
      }
    } catch (error) {
      console.error('Error loading cash balances:', error);
    }
  };

  const loadTopCustomers = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=business-status/top-customers&limit=9999&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } }); // 🆕 No limit - get all
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.customers) {
          setTopCustomers(data.customers);
        }
      }
    } catch (error) {
      console.error('Error loading top customers:', error);
    }
  };

  const loadTopVendors = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=business-status/top-vendors&limit=9999&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } }); // 🆕 No limit - get all
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.vendors) {
          setTopVendors(data.vendors);
        }
      }
    } catch (error) {
      console.error('Error loading top vendors:', error);
    }
  };

  // 🆕 Load invoice details for customer or vendor
  const loadInvoiceDetails = async (id: string, type: 'customer' | 'vendor') => {
    setLoadingInvoices(true);
    try {
      const endpoint = type === 'customer' 
        ? `transactions?customer_id=${id}` 
        : `transactions?vendor_id=${id}`;
      
      const res = await fetch(`/api/erp/peachtree?endpoint=${endpoint}&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });
      if (res.ok) {
        const data = await res.json();
        if (data.success && data.data) {
          setInvoiceDetails(data.data);
        } else {
          // Mock data for demo
          setInvoiceDetails([
            {
              InvoiceID: 'INV-001',
              Date: '2025-01-15',
              Type: 'Invoice',
              Description: 'Product Sale',
              Amount: 25000,
              Status: 'Unpaid',
              DueDate: '2025-02-15'
            },
            {
              InvoiceID: 'INV-002',
              Date: '2025-01-20',
              Type: 'Invoice',
              Description: 'Service Charge',
              Amount: 15000,
              Status: 'Unpaid',
              DueDate: '2025-02-20'
            },
            {
              InvoiceID: 'PAY-001',
              Date: '2025-01-10',
              Type: 'Payment',
              Description: 'Payment Received',
              Amount: -10000,
              Status: 'Paid',
              DueDate: '-'
            }
          ]);
        }
      }
    } catch (error) {
      console.error('Error loading invoice details:', error);
      setInvoiceDetails([]);
    } finally {
      setLoadingInvoices(false);
    }
  };

  const calculateStats = () => {
    const activeCustomers = customers.filter(c => c.Status === 'Active').length;
    const activeVendors = vendors.filter(v => v.Status === 'Active').length;
    const totalRevenue = customers.reduce((sum, c) => sum + (c.Balance || 0), 0);
    const totalExpenses = vendors.reduce((sum, v) => sum + (v.Balance || 0), 0);

    // Calculate data completeness percentages
    const customerComplete = customers.filter(c => 
      !c.CustomerName.startsWith('Customer ') && c.CustomerName !== c.CustomerID
    ).length;
    const vendorComplete = vendors.filter(v => 
      !v.VendorName.startsWith('Vendor ') && v.VendorName !== v.VendorID
    ).length;
    const accountComplete = accounts.filter(a => 
      !a.AccountName.startsWith('Account ') && a.AccountName !== a.AccountID
    ).length;

    const customerCompleteness = customers.length > 0 ? Math.round((customerComplete / customers.length) * 100) : 100;
    const vendorCompleteness = vendors.length > 0 ? Math.round((vendorComplete / vendors.length) * 100) : 100;
    const accountCompleteness = accounts.length > 0 ? Math.round((accountComplete / accounts.length) * 100) : 100;

    setStats({
      totalCustomers: customers.length,
      totalVendors: vendors.length,
      totalAccounts: accounts.length,
      totalTransactions: transactions.length,
      activeCustomers,
      activeVendors,
      totalRevenue,
      totalExpenses,
      dataCompleteness: {
        customers: customerCompleteness,
        vendors: vendorCompleteness,
        accounts: accountCompleteness
      }
    });
  };

  useEffect(() => {
    if (customers.length > 0 || vendors.length > 0 || accounts.length > 0) {
      calculateStats();
    }
  }, [customers, vendors, accounts, transactions]);

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
      minimumFractionDigits: 0
    }).format(amount);
  };

  const formatNumber = (num: number | null | undefined) => {
    if (!num) return '0';
    return new Intl.NumberFormat('en-US').format(num);
  };

  const exportToCSV = (data: any[], filename: string) => {
    if (!data || data.length === 0) {
      alert('No data to export!');
      return;
    }

    const headers = Object.keys(data[0]);
    const csvContent = [
      headers.join(','),
      ...data.map(row => 
        headers.map(header => {
          const value = row[header];
          if (typeof value === 'string' && value.includes(',')) {
            return `"${value}"`;
          }
          return value;
        }).join(',')
      )
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `peachtree_${filename}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  // Filter helper functions
  const getFilteredCustomers = () => {
    console.log('🔍 [FILTER] Total customers in state:', customers.length);
    console.log('🔍 [FILTER] Search term:', searchTerm);
    console.log('🔍 [FILTER] Hide zero balance:', hideZeroBalance);
    console.log('🔍 [FILTER] Hide inactive:', hideInactive);
    
    let filtered = customers.filter(c =>
      c.CustomerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      c.CustomerID.toLowerCase().includes(searchTerm.toLowerCase()) ||
      c.Phone.toLowerCase().includes(searchTerm.toLowerCase()) ||
      c.Contact.toLowerCase().includes(searchTerm.toLowerCase())
    );
    console.log('🔍 [FILTER] After search filter:', filtered.length);

    if (hideZeroBalance) {
      const beforeCount = filtered.length;
      filtered = filtered.filter(c => (c.Balance || 0) > 0);
      console.log(`🔍 [FILTER] After zero balance filter: ${filtered.length} (removed ${beforeCount - filtered.length})`);
    }

    if (hideInactive) {
      const beforeCount = filtered.length;
      filtered = filtered.filter(c => c.Status === 'Active');
      console.log(`🔍 [FILTER] After inactive filter: ${filtered.length} (removed ${beforeCount - filtered.length})`);
    }

    // Apply sorting
    filtered.sort((a, b) => {
      let compareValue = 0;
      
      if (sortBy === 'name') {
        compareValue = a.CustomerName.localeCompare(b.CustomerName);
      } else if (sortBy === 'balance') {
        compareValue = (a.Balance || 0) - (b.Balance || 0);
      }
      
      return sortOrder === 'asc' ? compareValue : -compareValue;
    });

    console.log('✅ [FILTER] Final filtered customers:', filtered.length);
    return filtered;
  };

  const getFilteredVendors = () => {
    let filtered = vendors.filter(v =>
      v.VendorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.VendorID.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.Phone.toLowerCase().includes(searchTerm.toLowerCase()) ||
      v.Contact.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (hideZeroBalance) {
      filtered = filtered.filter(v => (v.Balance || 0) > 0);
    }

    if (hideInactive) {
      filtered = filtered.filter(v => v.Status === 'Active');
    }

    // Apply sorting
    filtered.sort((a, b) => {
      let compareValue = 0;
      
      if (sortBy === 'name') {
        compareValue = a.VendorName.localeCompare(b.VendorName);
      } else if (sortBy === 'balance') {
        compareValue = (a.Balance || 0) - (b.Balance || 0);
      }
      
      return sortOrder === 'asc' ? compareValue : -compareValue;
    });

    return filtered;
  };

  const getFilteredAccounts = () => {
    let filtered = accounts.filter((a: any) =>
      a.AccountID?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.AccountName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.AccountType?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (hideZeroBalance) {
      filtered = filtered.filter((a: any) => Math.abs(a.Balance || 0) > 0);
    }

    if (showBankAccountsOnly) {
      // Peachtree: AccountType 0 is Cash/Bank
      filtered = filtered.filter((a: any) => a.AccountType === 'Cash');
    }

    if (hideInactive) {
      filtered = filtered.filter((a: any) => a.Active === true || a.Active === 'Y');
    }

    // Apply sorting
    filtered.sort((a: any, b: any) => {
      let compareValue = 0;
      
      if (sortBy === 'name') {
        compareValue = (a.AccountName || '').localeCompare(b.AccountName || '');
      } else if (sortBy === 'balance') {
        compareValue = (a.Balance || 0) - (b.Balance || 0);
      }
      
      return sortOrder === 'asc' ? compareValue : -compareValue;
    });

    return filtered;
  };

  if (isAuthenticated === null || loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="relative w-20 h-20 mx-auto mb-6">
            <div className="absolute inset-0 border-4 border-blue-500/30 rounded-full"></div>
            <div className="absolute inset-0 border-4 border-blue-500 rounded-full animate-spin border-t-transparent"></div>
          </div>
          <h2 className="text-2xl font-bold text-white mb-2">Loading Peachtree ERP...</h2>
          <p className="text-blue-300">Connecting to accounting system</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return null;
  }

  // Get customer balance distribution for pie chart
  const customerBalanceDistribution = customers
    .filter(c => c.Balance > 0)
    .sort((a, b) => b.Balance - a.Balance)
    .slice(0, 8)
    .map(c => ({
      name: c.CustomerName.length > 20 ? c.CustomerName.substring(0, 20) + '...' : c.CustomerName,
      value: c.Balance
    }));

  // Get account type distribution
  const accountTypeDistribution = accounts.reduce((acc, account) => {
    const type = account.AccountType || 'Other';
    const existing = acc.find(item => item.name === type);
    if (existing) {
      existing.value += 1;
    } else {
      acc.push({ name: type, value: 1 });
    }
    return acc;
  }, [] as Array<{ name: string; value: number }>);

  return (
    <div className={`bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-6 ${isModal ? 'min-h-full' : 'min-h-screen'}`}>
      <div className="max-w-[1800px] mx-auto">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg shadow-blue-500/50">
              <Building2 className="w-8 h-8 text-white" />
            </div>
            <div>
              <h1 className="text-4xl font-bold text-white mb-2">
                Peachtree ERP System
              </h1>
              <div className="flex items-center gap-3">
                <p className="text-blue-300">
                  Sage 50 Accounting Platform
                </p>
                {/* 🆕 Smart Bridge Status with Connect Button */}
                <div className="flex items-center gap-2">
                  <div className={`flex items-center gap-2 px-3 py-1.5 rounded-full ${
                    bridgeStatus === 'running' 
                      ? 'bg-green-500/20 text-green-400 border border-green-500/30' 
                      : bridgeStatus === 'stopped'
                      ? 'bg-red-500/20 text-red-400 border border-red-500/30'
                      : 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30'
                  }`}>
                    <div className={`w-2 h-2 rounded-full ${
                      bridgeStatus === 'running' 
                        ? 'bg-green-400 animate-pulse' 
                        : bridgeStatus === 'stopped'
                        ? 'bg-red-400'
                        : 'bg-yellow-400 animate-pulse'
                    }`}></div>
                    <span className="text-sm font-medium">
                      {bridgeStatus === 'running' 
                        ? 'Peachtree Connected' 
                        : bridgeStatus === 'stopped'
                        ? 'Peachtree Disconnected'
                        : 'Checking Connection...'}
                    </span>
                    {bridgeStatus === 'running' && bridgePid && (
                      <span className="text-xs opacity-70">PID: {bridgePid}</span>
                    )}
                  </div>
                  
                  {/* 🆕 Connect Now Button - Only shows when disconnected */}
                    {bridgeStatus === 'stopped' && (
                    <button
                      onClick={async () => {
                        const previousAuto = autoRefresh;
                        setStartingBridge(true);
                        try {
                          console.log('[UI] Starting Python bridge...');
                          setBridgeStatus('checking' as 'running'|'stopped'|'checking');
                          setError(null);
                          // Pause auto-refresh while starting
                          setAutoRefresh(false);

                          const res = await fetch('/api/peachtree-bridge', {
                            method: 'POST',
                            headers: {'Content-Type': 'application/json'},
                            body: JSON.stringify({action: 'start'})
                          });

                          const data = await res.json();
                          console.log('[UI] Bridge start response:', data);
                          
                          // 🆕 Capture logs from response
                          if (data.logs && Array.isArray(data.logs)) {
                            setBridgeLogs(data.logs);
                          }

                          // If backend reports running immediately, proceed. Otherwise poll status.
                          if (data.success && data.status === 'running') {
                            setBridgeStatus('running');
                            setBridgePid(data.pid);
                            setError(null);
                          } else {
                            // Poll the bridge status for up to 15 seconds
                            const maxAttempts = 30; // 30 * 500ms = 15s
                            let attempts = 0;
                            let ok = false;
                            while (attempts < maxAttempts) {
                              await new Promise(r => setTimeout(r, 500));
                              attempts++;
                              try {
                                const statusRes = await fetch('/api/peachtree-bridge');
                                if (statusRes.ok) {
                                  const statusData = await statusRes.json();
                                  if (statusData.logs && Array.isArray(statusData.logs)) {
                                    setBridgeLogs(statusData.logs);
                                  }
                                  if (statusData.status === 'running') {
                                    ok = true;
                                    setBridgeStatus('running');
                                    setBridgePid(statusData.pid || null);
                                    setError(null);
                                    break;
                                  }
                                }
                              } catch (e) {
                                // ignore and retry
                              }
                            }

                            if (!ok) {
                              throw new Error(data.message || 'Failed to start bridge within timeout');
                            }
                          }

                          // ✅ Bridge running -- load fresh data (no-cache)
                          console.log('[UI] Bridge started, loading fresh data...');
                          await loadAllData();
                        } catch (error: any) {
                          console.error('[UI] Bridge start failed:', error);
                          setBridgeStatus('stopped');
                          setError(error.message || 'Failed to start Python bridge - check console for details');
                        } finally {
                          // restore auto-refresh setting
                          setAutoRefresh(previousAuto);
                          setStartingBridge(false);
                        }
                      }}
                      className="flex items-center gap-2 px-4 py-1.5 bg-green-600 hover:bg-green-700 text-white text-sm font-medium rounded-lg transition-all shadow-lg shadow-green-500/30 disabled:opacity-50 disabled:cursor-not-allowed"
                      disabled={startingBridge}
                    >
                      <Activity className={`w-4 h-4 ${startingBridge ? 'animate-spin' : ''}`} />
                      {startingBridge ? 'Starting...' : 'Connect Now'}
                    </button>
                  )}
                  
                  {/* 🆕 View Logs Button */}
                  {bridgeLogs.length > 0 && (
                    <button
                      onClick={() => setShowLogs(!showLogs)}
                      className="flex items-center gap-2 px-3 py-1.5 bg-slate-700/50 hover:bg-slate-600/50 text-slate-300 text-sm font-medium rounded-lg transition-all border border-slate-600"
                    >
                      <FileText className="w-4 h-4" />
                      Logs ({bridgeLogs.length})
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>
          
          <div className="flex items-center gap-3">
            <button
              onClick={() => setAutoRefresh(!autoRefresh)}
              className={`flex items-center gap-2 px-4 py-2.5 rounded-xl transition-all ${
                autoRefresh 
                  ? 'bg-green-500 hover:bg-green-600 text-white shadow-lg shadow-green-500/50' 
                  : 'bg-slate-800/50 hover:bg-slate-700 text-slate-300 border border-slate-700'
              }`}
            >
              <Clock className="w-5 h-5" />
              Auto-Refresh {autoRefresh ? 'ON' : 'OFF'}
            </button>
            <button
              onClick={handleRefresh}
              disabled={refreshing}
              className="flex items-center gap-2 px-4 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-600 text-white rounded-xl transition-all shadow-lg shadow-blue-500/50"
            >
              <RefreshCw className={`w-5 h-5 ${refreshing ? 'animate-spin' : ''}`} />
              Refresh
            </button>
            {isModal ? (
              <button
                onClick={onClose}
                className="flex items-center gap-2 px-4 py-2.5 bg-slate-800/50 hover:bg-slate-700 text-white rounded-xl transition-all border border-slate-700"
              >
                <X className="w-5 h-5" />
                Close
              </button>
            ) : (
              <Link
                href="/dashboard"
                className="flex items-center gap-2 px-4 py-2.5 bg-slate-800/50 hover:bg-slate-700 text-white rounded-xl transition-all border border-slate-700"
              >
                <Home className="w-5 h-5" />
                Home
              </Link>
            )}
          </div>
        </div>

        {/* Error Display */}
        {error && (
          <div className="mb-6 bg-red-500/10 border border-red-500/50 rounded-xl p-4 flex items-center gap-3">
            <AlertCircle className="w-6 h-6 text-red-400" />
            <div>
              <h3 className="text-red-400 font-semibold">Connection Error</h3>
              <p className="text-red-300 text-sm">{error}</p>
            </div>
          </div>
        )}

        {/* Tab Navigation - 🆕 Enhanced with new tabs */}
        <div className="flex gap-2 mb-8 overflow-x-auto pb-2">
          {[
            { id: 'dashboard', label: 'Dashboard', icon: BarChart3 },
            { id: 'customers', label: 'Customers', icon: Users },
            { id: 'vendors', label: 'Vendors', icon: Building2 },
            { id: 'accounts', label: 'Chart of Accounts', icon: Wallet },
            { id: 'transactions', label: 'Transactions', icon: Receipt },
            { id: 'reports', label: 'Reports', icon: FileText },
            { id: 'analytics', label: 'Analytics', icon: PieChart }
          ].map((tab) => {
            const Icon = tab.icon;
            const isNew = ['reports', 'analytics'].includes(tab.id);
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as any)}
                className={`relative flex items-center gap-2 px-6 py-3 rounded-xl font-medium transition-all whitespace-nowrap ${
                  activeTab === tab.id
                    ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white shadow-lg shadow-blue-500/50 scale-105'
                    : 'bg-slate-800/50 text-slate-300 hover:bg-slate-700 border border-slate-700'
                }`}
              >
                <Icon className="w-5 h-5" />
                {tab.label}
                {isNew && (
                  <span className="absolute -top-1 -right-1 bg-green-500 text-white text-[10px] font-bold px-1.5 py-0.5 rounded-full">
                    NEW
                  </span>
                )}
              </button>
            );
          })}
        </div>

        {/* Dashboard Tab */}
        {activeTab === 'dashboard' && (
          <div className="space-y-6">
            {/* Top Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {/* Total Customers */}
              <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 backdrop-blur-xl border border-blue-500/30 rounded-2xl p-6 hover:shadow-lg hover:shadow-blue-500/20 transition-all">
                <div className="flex items-center justify-between mb-4">
                  <div className="w-14 h-14 bg-blue-500/20 rounded-xl flex items-center justify-center">
                    <Users className="w-7 h-7 text-blue-400" />
                  </div>
                  <div className="text-right">
                    <div className="flex items-center gap-1 text-sm text-green-400">
                      <ArrowUpRight className="w-4 h-4" />
                      <span>{stats.activeCustomers > 0 ? ((stats.activeCustomers / stats.totalCustomers) * 100).toFixed(0) : 0}%</span>
                    </div>
                  </div>
                </div>
                <h3 className="text-4xl font-bold text-white mb-2">
                  {formatNumber(stats.totalCustomers)}
                </h3>
                <p className="text-blue-300 font-medium">Total Customers</p>
                <p className="text-blue-400/60 text-sm mt-1">{formatNumber(stats.activeCustomers)} active</p>
              </div>

              {/* Total Vendors */}
              <div className="bg-gradient-to-br from-purple-500/20 to-purple-600/5 backdrop-blur-xl border border-purple-500/30 rounded-2xl p-6 hover:shadow-lg hover:shadow-purple-500/20 transition-all">
                <div className="flex items-center justify-between mb-4">
                  <div className="w-14 h-14 bg-purple-500/20 rounded-xl flex items-center justify-center">
                    <Building2 className="w-7 h-7 text-purple-400" />
                  </div>
                  <div className="text-right">
                    <div className="flex items-center gap-1 text-sm text-green-400">
                      <ArrowUpRight className="w-4 h-4" />
                      <span>{stats.activeVendors > 0 ? ((stats.activeVendors / stats.totalVendors) * 100).toFixed(0) : 0}%</span>
                    </div>
                  </div>
                </div>
                <h3 className="text-4xl font-bold text-white mb-2">
                  {formatNumber(stats.totalVendors)}
                </h3>
                <p className="text-purple-300 font-medium">Total Vendors</p>
                <p className="text-purple-400/60 text-sm mt-1">{formatNumber(stats.activeVendors)} active</p>
              </div>

              {/* Total Accounts */}
              <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 backdrop-blur-xl border border-green-500/30 rounded-2xl p-6 hover:shadow-lg hover:shadow-green-500/20 transition-all">
                <div className="flex items-center justify-between mb-4">
                  <div className="w-14 h-14 bg-green-500/20 rounded-xl flex items-center justify-center">
                    <Wallet className="w-7 h-7 text-green-400" />
                  </div>
                </div>
                <h3 className="text-4xl font-bold text-white mb-2">
                  {formatNumber(stats.totalAccounts)}
                </h3>
                <p className="text-green-300 font-medium">GL Accounts</p>
                <p className="text-green-400/60 text-sm mt-1">{accountTypeDistribution.length} types</p>
              </div>

              {/* Total Tables */}
              <div className="bg-gradient-to-br from-orange-500/20 to-orange-600/5 backdrop-blur-xl border border-orange-500/30 rounded-2xl p-6 hover:shadow-lg hover:shadow-orange-500/20 transition-all">
                <div className="flex items-center justify-between mb-4">
                  <div className="w-14 h-14 bg-orange-500/20 rounded-xl flex items-center justify-center">
                    <Package className="w-7 h-7 text-orange-400" />
                  </div>
                </div>
                <h3 className="text-4xl font-bold text-white mb-2">
                  {formatNumber(tables.length)}
                </h3>
                <p className="text-orange-300 font-medium">Database Tables</p>
                <p className="text-orange-400/60 text-sm mt-1">Accessible data</p>
              </div>
            </div>

            {/* Data Completeness Card */}
            <div className="bg-gradient-to-br from-cyan-500/20 to-cyan-600/5 backdrop-blur-xl border border-cyan-500/30 rounded-2xl p-6">
              <div className="flex items-center gap-3 mb-4">
                <Database className="w-8 h-8 text-cyan-400" />
                <div className="flex-1">
                  <h3 className="text-xl font-bold text-white">Data Quality</h3>
                  <p className="text-cyan-300 text-sm">Complete records vs total</p>
                </div>
              </div>
              <div className="space-y-3">
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-sm text-slate-300">Customers</span>
                    <span className="text-sm font-bold text-cyan-400">{stats.dataCompleteness.customers}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-2">
                    <div 
                      className="bg-gradient-to-r from-cyan-500 to-blue-500 h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.customers}%` }}
                    />
                  </div>
                </div>
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-sm text-slate-300">Vendors</span>
                    <span className="text-sm font-bold text-cyan-400">{stats.dataCompleteness.vendors}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-2">
                    <div 
                      className="bg-gradient-to-r from-purple-500 to-pink-500 h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.vendors}%` }}
                    />
                  </div>
                </div>
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-sm text-slate-300">Accounts</span>
                    <span className="text-sm font-bold text-cyan-400">{stats.dataCompleteness.accounts}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-2">
                    <div 
                      className="bg-gradient-to-r from-green-500 to-emerald-500 h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.accounts}%` }}
                    />
                  </div>
                </div>
              </div>
            </div>

            {/* Financial Overview */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Receivables */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
                <div className="flex items-center justify-between mb-6">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-blue-500/20 rounded-xl flex items-center justify-center">
                      <DollarSign className="w-6 h-6 text-blue-400" />
                    </div>
                    <div>
                      <h3 className="text-xl font-bold text-white">Accounts Receivable</h3>
                      <p className="text-slate-400 text-sm">Customer balances</p>
                    </div>
                  </div>
                  <button 
                    onClick={() => exportToCSV(customers, 'accounts_receivable')}
                    className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-400 rounded-lg transition-colors"
                  >
                    <Download className="w-5 h-5" />
                  </button>
                </div>
                <div className="mb-4">
                  <h4 className="text-3xl font-bold text-blue-400">{formatCurrency(stats.totalRevenue)}</h4>
                  <p className="text-slate-400 text-sm mt-1">Total outstanding from {stats.totalCustomers} customers</p>
                </div>
                <ResponsiveContainer width="100%" height={200}>
                  <BarChart data={customers.slice(0, 10)}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                    <XAxis dataKey="CustomerName" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} angle={-45} textAnchor="end" height={80} />
                    <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8' }} tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`} />
                    <Tooltip 
                      contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                      formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Balance']}
                    />
                    <Bar dataKey="Balance" fill="#3b82f6" radius={[8, 8, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>

              {/* Payables */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
                <div className="flex items-center justify-between mb-6">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-purple-500/20 rounded-xl flex items-center justify-center">
                      <CreditCard className="w-6 h-6 text-purple-400" />
                    </div>
                    <div>
                      <h3 className="text-xl font-bold text-white">Accounts Payable</h3>
                      <p className="text-slate-400 text-sm">Vendor balances</p>
                    </div>
                  </div>
                  <button 
                    onClick={() => exportToCSV(vendors, 'accounts_payable')}
                    className="p-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-400 rounded-lg transition-colors"
                  >
                    <Download className="w-5 h-5" />
                  </button>
                </div>
                <div className="mb-4">
                  <h4 className="text-3xl font-bold text-purple-400">{formatCurrency(stats.totalExpenses)}</h4>
                  <p className="text-slate-400 text-sm mt-1">Total outstanding to {stats.totalVendors} vendors</p>
                </div>
                <ResponsiveContainer width="100%" height={200}>
                  <BarChart data={vendors.slice(0, 10)}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                    <XAxis dataKey="VendorName" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} angle={-45} textAnchor="end" height={80} />
                    <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8' }} tickFormatter={(value) => `Rs ${(value / 1000).toFixed(0)}K`} />
                    <Tooltip 
                      contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                      formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Balance']}
                    />
                    <Bar dataKey="Balance" fill="#a855f7" radius={[8, 8, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>

            {/* Charts Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Customer Balance Distribution */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
                <div className="flex items-center justify-between mb-6">
                  <h3 className="text-xl font-bold text-white flex items-center gap-2">
                    <PieChart className="w-6 h-6 text-blue-400" />
                    Top Customers by Balance
                  </h3>
                </div>
                {customerBalanceDistribution.length > 0 ? (
                  <ResponsiveContainer width="100%" height={300}>
                    <RechartsPie>
                      <Pie
                        data={customerBalanceDistribution}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }: any) => `${name}: ${(percent * 100).toFixed(0)}%`}
                        outerRadius={100}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {customerBalanceDistribution.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                        formatter={(value: any) => formatCurrency(value)}
                      />
                    </RechartsPie>
                  </ResponsiveContainer>
                ) : (
                  <div className="h-[300px] flex items-center justify-center text-slate-400">
                    No customer balance data available
                  </div>
                )}
              </div>

              {/* Account Type Distribution */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
                <div className="flex items-center justify-between mb-6">
                  <h3 className="text-xl font-bold text-white flex items-center gap-2">
                    <BarChart3 className="w-6 h-6 text-purple-400" />
                    Account Type Distribution
                  </h3>
                </div>
                {accountTypeDistribution.length > 0 ? (
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={accountTypeDistribution}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                      <XAxis dataKey="name" stroke="#94a3b8" tick={{ fill: '#94a3b8' }} />
                      <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8' }} />
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                      />
                      <Bar dataKey="value" fill="#a855f7" radius={[8, 8, 0, 0]}>
                        {accountTypeDistribution.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                        ))}
                      </Bar>
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <div className="h-[300px] flex items-center justify-center text-slate-400">
                    No account data available
                  </div>
                )}
              </div>
            </div>

            {/* Quick Actions */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
              <h3 className="text-xl font-bold text-white mb-6">Quick Actions</h3>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <button
                  onClick={() => setActiveTab('customers')}
                  className="flex items-center gap-3 p-4 bg-blue-500/10 hover:bg-blue-500/20 border border-blue-500/30 rounded-xl transition-all group"
                >
                  <Users className="w-8 h-8 text-blue-400 group-hover:scale-110 transition-transform" />
                  <div className="text-left">
                    <p className="text-white font-semibold">View All Customers</p>
                    <p className="text-blue-300 text-sm">{formatNumber(stats.totalCustomers)} total</p>
                  </div>
                </button>
                <button
                  onClick={() => setActiveTab('vendors')}
                  className="flex items-center gap-3 p-4 bg-purple-500/10 hover:bg-purple-500/20 border border-purple-500/30 rounded-xl transition-all group"
                >
                  <Building2 className="w-8 h-8 text-purple-400 group-hover:scale-110 transition-transform" />
                  <div className="text-left">
                    <p className="text-white font-semibold">View All Vendors</p>
                    <p className="text-purple-300 text-sm">{formatNumber(stats.totalVendors)} total</p>
                  </div>
                </button>
                <button
                  onClick={() => setActiveTab('accounts')}
                  className="flex items-center gap-3 p-4 bg-green-500/10 hover:bg-green-500/20 border border-green-500/30 rounded-xl transition-all group"
                >
                  <Wallet className="w-8 h-8 text-green-400 group-hover:scale-110 transition-transform" />
                  <div className="text-left">
                    <p className="text-white font-semibold">Chart of Accounts</p>
                    <p className="text-green-300 text-sm">{formatNumber(stats.totalAccounts)} accounts</p>
                  </div>
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Customers Tab */}
        {activeTab === 'customers' && (
          <div className="space-y-6">
            {/* Search & Export Bar */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-4">
              <div className="flex items-center justify-between gap-4 mb-4">
                <div className="flex items-center gap-3 flex-1">
                  <Search className="w-5 h-5 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Search customers by name, ID, phone..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="flex-1 bg-transparent text-white placeholder-slate-400 focus:outline-none"
                  />
                </div>
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => setShowFilters(!showFilters)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-xl transition-colors ${
                      showFilters ? 'bg-blue-500/30 text-blue-400' : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
                    }`}
                  >
                    <Filter className="w-5 h-5" />
                    Filters
                  </button>
                  <button
                    onClick={() => exportToCSV(getFilteredCustomers(), 'customers')}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-400 rounded-xl transition-colors"
                  >
                    <Download className="w-5 h-5" />
                    Export CSV
                  </button>
                </div>
              </div>
              
              {/* Filter Options - 🆕 Enhanced */}
              {showFilters && (
                <div className="border-t border-slate-700 pt-4 space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    {/* Hide Zero Balance */}
                    <div className="flex items-center gap-3">
                      <input
                        type="checkbox"
                        id="hideZero"
                        checked={hideZeroBalance}
                        onChange={(e) => setHideZeroBalance(e.target.checked)}
                        className="w-4 h-4 rounded bg-slate-700 border-slate-600 text-blue-500 focus:ring-blue-500"
                      />
                      <label htmlFor="hideZero" className="text-slate-300 text-sm cursor-pointer">
                        Hide Zero Balance
                      </label>
                    </div>
                    
                    {/* Hide Inactive */}
                    <div className="flex items-center gap-3">
                      <input
                        type="checkbox"
                        id="hideInactive"
                        checked={hideInactive}
                        onChange={(e) => setHideInactive(e.target.checked)}
                        className="w-4 h-4 rounded bg-slate-700 border-slate-600 text-blue-500 focus:ring-blue-500"
                      />
                      <label htmlFor="hideInactive" className="text-slate-300 text-sm cursor-pointer">
                        Hide Inactive
                      </label>
                    </div>
                    
                    {/* Sort By */}
                    <div className="flex items-center gap-2">
                      <span className="text-slate-400 text-sm">Sort By:</span>
                      <select
                        value={sortBy}
                        onChange={(e) => setSortBy(e.target.value as any)}
                        className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1.5 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                      >
                        <option value="name">Name</option>
                        <option value="balance">Balance</option>
                        <option value="date">Date</option>
                      </select>
                    </div>
                    
                    {/* Sort Order */}
                    <div className="flex items-center gap-2">
                      <span className="text-slate-400 text-sm">Order:</span>
                      <select
                        value={sortOrder}
                        onChange={(e) => setSortOrder(e.target.value as any)}
                        className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1.5 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                      >
                        <option value="asc">Ascending ↑</option>
                        <option value="desc">Descending ↓</option>
                      </select>
                    </div>
                  </div>
                  
                  {/* Date Range */}
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-slate-400" />
                      <label className="text-slate-400 text-sm">From:</label>
                      <input
                        type="date"
                        value={dateFrom}
                        onChange={(e) => setDateFrom(e.target.value)}
                        className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1.5 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                      />
                    </div>
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-slate-400" />
                      <label className="text-slate-400 text-sm">To:</label>
                      <input
                        type="date"
                        value={dateTo}
                        onChange={(e) => setDateTo(e.target.value)}
                        className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1.5 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                      />
                    </div>
                  </div>
                  
                  {/* Clear Filters Button */}
                  <div className="flex justify-end">
                    <button
                      onClick={() => {
                        setSearchTerm('');
                        setHideZeroBalance(false);
                        setHideInactive(false);
                        setDateFrom('');
                        setDateTo('');
                        setSortBy('name');
                        setSortOrder('asc');
                      }}
                      className="flex items-center gap-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-xl transition-colors text-sm"
                    >
                      <XCircle className="w-4 h-4" />
                      Clear All Filters
                    </button>
                  </div>
                </div>
              )}
            </div>

            {/* Customer Stats */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 backdrop-blur-xl border border-blue-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <CheckCircle className="w-8 h-8 text-green-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(stats.activeCustomers)}</h3>
                    <p className="text-blue-300">Active Customers</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-red-500/20 to-red-600/5 backdrop-blur-xl border border-red-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <XCircle className="w-8 h-8 text-red-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(stats.totalCustomers - stats.activeCustomers)}</h3>
                    <p className="text-red-300">Inactive Customers</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 backdrop-blur-xl border border-green-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <DollarSign className="w-8 h-8 text-green-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatCurrency(stats.totalRevenue)}</h3>
                    <p className="text-green-300">Total Receivables</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Customer Table */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-slate-900/50">
                    <tr>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Name</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Contact</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Phone</th>
                      <th className="text-right text-slate-300 font-semibold py-4 px-6">Balance</th>
                      <th className="text-right text-slate-300 font-semibold py-4 px-6">Credit Limit</th>
                      <th className="text-center text-slate-300 font-semibold py-4 px-6">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {getFilteredCustomers()
                      .map((customer, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-4 px-6 text-white font-medium">
                            <div className="flex items-center gap-2">
                              <span>{customer.CustomerName}</span>
                              {(customer.CustomerName.startsWith('Customer ') || customer.CustomerName === customer.CustomerID) && (
                                <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-amber-500/20 text-amber-400 rounded text-xs font-medium">
                                  <AlertCircle className="w-3 h-3" />
                                  Missing Name
                                </span>
                              )}
                            </div>
                          </td>
                          <td className="py-4 px-6 text-slate-300">{customer.Contact || '-'}</td>
                          <td className="py-4 px-6 text-slate-300">{customer.Phone || '-'}</td>
                          <td className="py-4 px-6 text-right">
                            <button
                              onClick={() => {
                                setSelectedCustomerForInvoice(customer);
                                loadInvoiceDetails(customer.CustomerID, 'customer');
                              }}
                              className="text-green-400 font-semibold hover:text-green-300 hover:underline cursor-pointer transition-colors"
                            >
                              {formatCurrency(customer.Balance)}
                            </button>
                          </td>
                          <td className="py-4 px-6 text-right text-slate-300">{formatCurrency(customer.CreditLimit)}</td>
                          <td className="py-4 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              customer.Status === 'Active'
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-red-500/20 text-red-400'
                            }`}>
                              {customer.Status === 'Active' ? <CheckCircle className="w-3 h-3" /> : <XCircle className="w-3 h-3" />}
                              {customer.Status}
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

        {/* Vendors Tab */}
        {activeTab === 'vendors' && (
          <div className="space-y-6">
            {/* Search & Export Bar */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-4">
              <div className="flex items-center justify-between gap-4 mb-4">
                <div className="flex items-center gap-3 flex-1">
                  <Search className="w-5 h-5 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Search vendors by name, ID, phone..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="flex-1 bg-transparent text-white placeholder-slate-400 focus:outline-none"
                  />
                </div>
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => setShowFilters(!showFilters)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-xl transition-colors ${
                      showFilters ? 'bg-purple-500/30 text-purple-400' : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
                    }`}
                  >
                    <Filter className="w-5 h-5" />
                    Filters
                  </button>
                  <button
                    onClick={() => exportToCSV(getFilteredVendors(), 'vendors')}
                    className="flex items-center gap-2 px-4 py-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-400 rounded-xl transition-colors"
                  >
                    <Download className="w-5 h-5" />
                    Export CSV
                  </button>
                </div>
              </div>
              
              {/* Filter Options */}
              {showFilters && (
                <div className="border-t border-slate-700 pt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="flex items-center gap-3">
                    <input
                      type="checkbox"
                      id="hideZeroVendor"
                      checked={hideZeroBalance}
                      onChange={(e) => setHideZeroBalance(e.target.checked)}
                      className="w-4 h-4 rounded bg-slate-700 border-slate-600 text-purple-500 focus:ring-purple-500"
                    />
                    <label htmlFor="hideZeroVendor" className="text-slate-300 text-sm cursor-pointer">
                      Hide Zero Balance
                    </label>
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="w-4 h-4 text-slate-400" />
                    <input
                      type="date"
                      value={dateFrom}
                      onChange={(e) => setDateFrom(e.target.value)}
                      placeholder="From Date"
                      className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1 rounded border border-slate-600 focus:outline-none focus:border-purple-500"
                    />
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="w-4 h-4 text-slate-400" />
                    <input
                      type="date"
                      value={dateTo}
                      onChange={(e) => setDateTo(e.target.value)}
                      placeholder="To Date"
                      className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1 rounded border border-slate-600 focus:outline-none focus:border-purple-500"
                    />
                  </div>
                </div>
              )}
            </div>

            {/* Vendor Stats */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="bg-gradient-to-br from-purple-500/20 to-purple-600/5 backdrop-blur-xl border border-purple-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <CheckCircle className="w-8 h-8 text-green-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(stats.activeVendors)}</h3>
                    <p className="text-purple-300">Active Vendors</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-red-500/20 to-red-600/5 backdrop-blur-xl border border-red-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <XCircle className="w-8 h-8 text-red-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(stats.totalVendors - stats.activeVendors)}</h3>
                    <p className="text-red-300">Inactive Vendors</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-red-500/20 to-red-600/5 backdrop-blur-xl border border-red-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <CreditCard className="w-8 h-8 text-red-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatCurrency(stats.totalExpenses)}</h3>
                    <p className="text-red-300">Total Payables</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Vendor Table */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-slate-900/50">
                    <tr>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Name</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Contact</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Phone</th>
                      <th className="text-center text-slate-300 font-semibold py-4 px-6">Credit Period</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Warehouse</th>
                      <th className="text-right text-slate-300 font-semibold py-4 px-6">Balance</th>
                      <th className="text-center text-slate-300 font-semibold py-4 px-6">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {getFilteredVendors()
                      .map((vendor, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-4 px-6 text-white font-medium">
                            <div className="flex items-center gap-2">
                              <span>{vendor.VendorName}</span>
                              {(vendor.VendorName.startsWith('Vendor ') || vendor.VendorName === vendor.VendorID) && (
                                <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-amber-500/20 text-amber-400 rounded text-xs font-medium">
                                  <AlertCircle className="w-3 h-3" />
                                  Missing Name
                                </span>
                              )}
                            </div>
                          </td>
                          <td className="py-4 px-6 text-slate-300">{vendor.Contact || '-'}</td>
                          <td className="py-4 px-6 text-slate-300">{vendor.Phone || '-'}</td>
                          <td className="py-4 px-6 text-center">
                            <span className="inline-flex items-center gap-1 px-3 py-1 bg-blue-500/20 text-blue-400 rounded-full text-sm font-medium">
                              <Calendar className="w-3 h-3" />
                              {vendor.CreditPeriod || '30 Days'}
                            </span>
                          </td>
                          <td className="py-4 px-6 text-slate-300">
                            <span className="inline-flex items-center gap-1">
                              <Building2 className="w-4 h-4 text-slate-400" />
                              {vendor.Warehouse || 'Main Warehouse'}
                            </span>
                          </td>
                          <td className="py-4 px-6 text-right">
                            <button
                              onClick={() => {
                                setSelectedVendorForInvoice(vendor);
                                loadInvoiceDetails(vendor.VendorID, 'vendor');
                              }}
                              className="text-red-400 font-semibold hover:text-red-300 hover:underline cursor-pointer transition-colors"
                            >
                              {formatCurrency(vendor.Balance)}
                            </button>
                          </td>
                          <td className="py-4 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              vendor.Status === 'Active'
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-red-500/20 text-red-400'
                            }`}>
                              {vendor.Status === 'Active' ? <CheckCircle className="w-3 h-3" /> : <XCircle className="w-3 h-3" />}
                              {vendor.Status}
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

        {/* Accounts Tab */}
        {activeTab === 'accounts' && (
          <div className="space-y-6">
            {/* Search & Export Bar */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-4">
              <div className="flex items-center justify-between gap-4 mb-4">
                <div className="flex items-center gap-3 flex-1">
                  <Search className="w-5 h-5 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Search accounts by ID, name, type..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="flex-1 bg-transparent text-white placeholder-slate-400 focus:outline-none"
                  />
                </div>
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => setShowFilters(!showFilters)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-xl transition-colors ${
                      showFilters ? 'bg-green-500/30 text-green-400' : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
                    }`}
                  >
                    <Filter className="w-5 h-5" />
                    Filters
                  </button>
                  <button
                    onClick={() => exportToCSV(getFilteredAccounts(), 'chart_of_accounts')}
                    className="flex items-center gap-2 px-4 py-2 bg-green-500/20 hover:bg-green-500/30 text-green-400 rounded-xl transition-colors"
                  >
                    <Download className="w-5 h-5" />
                    Export CSV
                  </button>
                </div>
              </div>
              
              {/* Filter Options */}
              {showFilters && (
                <div className="border-t border-slate-700 pt-4 flex items-center gap-4">
                  <div className="flex items-center gap-3">
                    <input
                      type="checkbox"
                      id="hideZeroAccount"
                      checked={hideZeroBalance}
                      onChange={(e) => setHideZeroBalance(e.target.checked)}
                      className="w-4 h-4 rounded bg-slate-700 border-slate-600 text-green-500 focus:ring-green-500"
                    />
                    <label htmlFor="hideZeroAccount" className="text-slate-300 text-sm cursor-pointer">
                      Hide Zero Balance Accounts
                    </label>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    <input
                      type="checkbox"
                      id="showBankAccounts"
                      checked={showBankAccountsOnly}
                      onChange={(e) => setShowBankAccountsOnly(e.target.checked)}
                      className="w-4 h-4 rounded bg-slate-700 border-slate-600 text-green-500 focus:ring-green-500"
                    />
                    <label htmlFor="showBankAccounts" className="text-slate-300 text-sm cursor-pointer">
                      Show Bank Accounts Only
                    </label>
                  </div>
                </div>
              )}
            </div>

            {/* Account Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
              {accountTypeDistribution.slice(0, 4).map((type, index) => (
                <div key={type.name} className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
                  <div className="flex items-center gap-3">
                    <div className={`w-12 h-12 rounded-xl flex items-center justify-center`} style={{ backgroundColor: `${CHART_COLORS[index]}20` }}>
                      <Wallet className="w-6 h-6" style={{ color: CHART_COLORS[index] }} />
                    </div>
                    <div>
                      <h3 className="text-2xl font-bold text-white">{formatNumber(type.value)}</h3>
                      <p className="text-slate-300 text-sm">{type.name}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {/* Accounts Table */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-slate-900/50">
                    <tr>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Account ID</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Account Name</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Type</th>
                      <th className="text-right text-slate-300 font-semibold py-4 px-6">Balance</th>
                      <th className="text-center text-slate-300 font-semibold py-4 px-6">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {getFilteredAccounts()
                      .map((account, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-4 px-6 text-green-400 font-mono text-sm">{account.AccountID}</td>
                          <td className="py-4 px-6 text-white font-medium">
                            <div className="flex items-center gap-2">
                              <span>{account.AccountName}</span>
                              {(account.AccountName.startsWith('Account ') || account.AccountName === account.AccountID) && (
                                <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-amber-500/20 text-amber-400 rounded text-xs font-medium">
                                  <AlertCircle className="w-3 h-3" />
                                  Missing Name
                                </span>
                              )}
                            </div>
                          </td>
                          <td className="py-4 px-6">
                            <span className="inline-block px-3 py-1 bg-blue-500/20 text-blue-400 rounded-lg text-sm">
                              {account.AccountType}
                            </span>
                          </td>
                          <td className="py-4 px-6 text-right text-white font-semibold">{formatCurrency(account.Balance)}</td>
                          <td className="py-4 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              account.Active
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-gray-500/20 text-gray-400'
                            }`}>
                              {account.Active ? <CheckCircle className="w-3 h-3" /> : <XCircle className="w-3 h-3" />}
                              {account.Active ? 'Active' : 'Inactive'}
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

        {/* Transactions Tab */}
        {activeTab === 'transactions' && (
          <div className="space-y-6">
            {/* Search & Export Bar */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-4">
              <div className="flex items-center justify-between gap-4 mb-4">
                <div className="flex items-center gap-3 flex-1">
                  <Search className="w-5 h-5 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Search transactions by ID, description, type..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="flex-1 bg-transparent text-white placeholder-slate-400 focus:outline-none"
                  />
                </div>
                <div className="flex items-center gap-3">
                  <button
                    onClick={() => setShowFilters(!showFilters)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-xl transition-colors ${
                      showFilters ? 'bg-blue-500/30 text-blue-400' : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
                    }`}
                  >
                    <Filter className="w-5 h-5" />
                    Filters
                  </button>
                  <button
                    onClick={() => exportToCSV(transactions.filter(txn =>
                      txn.Description.toLowerCase().includes(searchTerm.toLowerCase()) ||
                      txn.TransactionID.toLowerCase().includes(searchTerm.toLowerCase()) ||
                      txn.Type.toLowerCase().includes(searchTerm.toLowerCase())
                    ), 'transactions')}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-400 rounded-xl transition-colors"
                  >
                    <Download className="w-5 h-5" />
                    Export CSV
                  </button>
                </div>
              </div>
              
              {/* Filter Options */}
              {showFilters && (
                <div className="border-t border-slate-700 pt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="flex items-center gap-2">
                    <Calendar className="w-4 h-4 text-slate-400" />
                    <input
                      type="date"
                      value={dateFrom}
                      onChange={(e) => setDateFrom(e.target.value)}
                      placeholder="From Date"
                      className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                    />
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="w-4 h-4 text-slate-400" />
                    <input
                      type="date"
                      value={dateTo}
                      onChange={(e) => setDateTo(e.target.value)}
                      placeholder="To Date"
                      className="flex-1 bg-slate-700/50 text-white text-sm px-3 py-1 rounded border border-slate-600 focus:outline-none focus:border-blue-500"
                    />
                  </div>
                </div>
              )}
            </div>

            {/* Transaction Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
              <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 backdrop-blur-xl border border-blue-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <Receipt className="w-8 h-8 text-blue-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(transactions.length)}</h3>
                    <p className="text-blue-300">Total Transactions</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 backdrop-blur-xl border border-green-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <CheckCircle className="w-8 h-8 text-green-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(transactions.filter(t => t.Status === 'Posted').length)}</h3>
                    <p className="text-green-300">Posted</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-yellow-500/20 to-yellow-600/5 backdrop-blur-xl border border-yellow-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <Clock className="w-8 h-8 text-yellow-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatNumber(transactions.filter(t => t.Status === 'Draft').length)}</h3>
                    <p className="text-yellow-300">Draft</p>
                  </div>
                </div>
              </div>
              <div className="bg-gradient-to-br from-purple-500/20 to-purple-600/5 backdrop-blur-xl border border-purple-500/30 rounded-2xl p-6">
                <div className="flex items-center gap-3 mb-4">
                  <DollarSign className="w-8 h-8 text-purple-400" />
                  <div>
                    <h3 className="text-2xl font-bold text-white">{formatCurrency(transactions.reduce((sum, t) => sum + t.Amount, 0))}</h3>
                    <p className="text-purple-300">Total Amount</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Transactions Table */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="bg-slate-900/50">
                    <tr>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Transaction ID</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Date</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Type</th>
                      <th className="text-left text-slate-300 font-semibold py-4 px-6">Description</th>
                      <th className="text-right text-slate-300 font-semibold py-4 px-6">Amount</th>
                      <th className="text-center text-slate-300 font-semibold py-4 px-6">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {transactions
                      .filter(txn =>
                        txn.Description.toLowerCase().includes(searchTerm.toLowerCase()) ||
                        txn.TransactionID.toLowerCase().includes(searchTerm.toLowerCase()) ||
                        txn.Type.toLowerCase().includes(searchTerm.toLowerCase())
                      )
                      .map((txn, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-4 px-6 text-blue-400 font-mono text-sm">{txn.TransactionID}</td>
                          <td className="py-4 px-6 text-slate-300">{txn.Date || '-'}</td>
                          <td className="py-4 px-6">
                            <span className="inline-block px-3 py-1 bg-purple-500/20 text-purple-400 rounded-lg text-sm">
                              {txn.Type}
                            </span>
                          </td>
                          <td className="py-4 px-6 text-white">{txn.Description}</td>
                          <td className="py-4 px-6 text-right text-green-400 font-semibold">{formatCurrency(txn.Amount)}</td>
                          <td className="py-4 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              txn.Status === 'Posted'
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-yellow-500/20 text-yellow-400'
                            }`}>
                              {txn.Status === 'Posted' ? <CheckCircle className="w-3 h-3" /> : <Clock className="w-3 h-3" />}
                              {txn.Status}
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

        {/* Reports Tab */}
        {activeTab === 'reports' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <div className="flex items-center gap-3 mb-6">
                <FileText className="w-8 h-8 text-blue-400" />
                <h2 className="text-3xl font-bold text-white">Reports & Analytics</h2>
                <p className="text-slate-400 ml-auto">Generate comprehensive business reports</p>
              </div>
              
              {/* Report Categories */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                
                {/* Financial Reports */}
                <button
                  onClick={() => setActiveTab('financial')}
                  className="bg-gradient-to-br from-green-500/20 to-green-600/5 border border-green-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-green-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-green-500/20 rounded-xl flex items-center justify-center">
                      <DollarSign className="w-6 h-6 text-green-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">Financial Reports</h3>
                      <p className="text-sm text-slate-400">P&L, Balance Sheet</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Comprehensive financial statements and analysis
                  </div>
                  <div className="flex items-center gap-2 text-xs text-green-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

                {/* AR/AP Aging Reports */}
                <button
                  onClick={() => setActiveTab('ar-ap-aging')}
                  className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 border border-blue-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-blue-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-blue-500/20 rounded-xl flex items-center justify-center">
                      <TrendingUp className="w-6 h-6 text-blue-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">AR/AP Aging</h3>
                      <p className="text-sm text-slate-400">Receivables & Payables</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Age analysis of outstanding invoices and bills
                  </div>
                  <div className="flex items-center gap-2 text-xs text-blue-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

                {/* Inventory Reports */}
                <button
                  onClick={() => setActiveTab('inventory')}
                  className="bg-gradient-to-br from-purple-500/20 to-purple-600/5 border border-purple-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-purple-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-purple-500/20 rounded-xl flex items-center justify-center">
                      <Package className="w-6 h-6 text-purple-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">Inventory Reports</h3>
                      <p className="text-sm text-slate-400">Stock & Valuation</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Stock levels, valuation, and movement reports
                  </div>
                  <div className="flex items-center gap-2 text-xs text-purple-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

                {/* Job/Project Reports */}
                <button
                  onClick={() => setActiveTab('jobs')}
                  className="bg-gradient-to-br from-orange-500/20 to-orange-600/5 border border-orange-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-orange-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-orange-500/20 rounded-xl flex items-center justify-center">
                      <BarChart3 className="w-6 h-6 text-orange-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">Job Reports</h3>
                      <p className="text-sm text-slate-400">Project Analysis</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Job costing, profitability, and progress tracking
                  </div>
                  <div className="flex items-center gap-2 text-xs text-orange-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

                {/* Payroll Reports */}
                <button
                  onClick={() => setActiveTab('payroll')}
                  className="bg-gradient-to-br from-pink-500/20 to-pink-600/5 border border-pink-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-pink-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-pink-500/20 rounded-xl flex items-center justify-center">
                      <Users className="w-6 h-6 text-pink-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">Payroll Reports</h3>
                      <p className="text-sm text-slate-400">Employee Earnings</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Employee wages, deductions, and tax reports
                  </div>
                  <div className="flex items-center gap-2 text-xs text-pink-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

                {/* Cash Flow Reports */}
                <button
                  onClick={() => setActiveTab('cashflow')}
                  className="bg-gradient-to-br from-cyan-500/20 to-cyan-600/5 border border-cyan-500/30 rounded-xl p-6 hover:shadow-lg hover:shadow-cyan-500/20 transition-all text-left"
                >
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 bg-cyan-500/20 rounded-xl flex items-center justify-center">
                      <Wallet className="w-6 h-6 text-cyan-400" />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">Cash Flow</h3>
                      <p className="text-sm text-slate-400">Banking & Liquidity</p>
                    </div>
                  </div>
                  <div className="text-sm text-slate-300 mb-3">
                    Cash position, bank balances, and flow analysis
                  </div>
                  <div className="flex items-center gap-2 text-xs text-cyan-400 font-semibold">
                    <ArrowUpRight className="w-4 h-4" />
                    View Report
                  </div>
                </button>

              </div>

              {/* Info Message */}
              <div className="mt-8 bg-blue-500/10 border border-blue-500/30 rounded-xl p-6">
                <div className="flex items-start gap-4">
                  <AlertCircle className="w-6 h-6 text-blue-400 flex-shrink-0 mt-1" />
                  <div>
                    <h4 className="text-white font-semibold mb-2">Real-Time Peachtree Data</h4>
                    <p className="text-slate-300 text-sm mb-2">
                      All reports are generated from live Peachtree database. Click any report card above to view detailed analysis.
                    </p>
                    <p className="text-slate-400 text-xs">
                      Current data: {stats.totalCustomers} customers, {stats.totalVendors} vendors, 
                      {stats.totalAccounts} accounts, {stats.totalTransactions} transactions
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Analytics Tab */}
        {activeTab === 'analytics' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <div className="flex items-center gap-3 mb-6">
                <PieChart className="w-8 h-8 text-purple-400" />
                <h2 className="text-3xl font-bold text-white">Advanced Analytics</h2>
              </div>
              
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Customer Distribution */}
                <div className="bg-slate-900/50 border border-slate-700 rounded-xl p-6">
                  <h3 className="text-xl font-bold text-white mb-4">Customer Balance Distribution</h3>
                  <div className="h-80">
                    <ResponsiveContainer width="100%" height="100%">
                      <BarChart data={customers.slice(0, 10)}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                        <XAxis dataKey="CustomerName" stroke="#94a3b8" angle={-45} textAnchor="end" height={100} />
                        <YAxis stroke="#94a3b8" />
                        <Tooltip 
                          contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                          labelStyle={{ color: '#f1f5f9' }}
                        />
                        <Bar dataKey="Balance" fill="#3b82f6" radius={[8, 8, 0, 0]} />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                </div>

                {/* Vendor Distribution */}
                <div className="bg-slate-900/50 border border-slate-700 rounded-xl p-6">
                  <h3 className="text-xl font-bold text-white mb-4">Vendor Balance Distribution</h3>
                  <div className="h-80">
                    <ResponsiveContainer width="100%" height="100%">
                      <BarChart data={vendors.slice(0, 10)}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                        <XAxis dataKey="VendorName" stroke="#94a3b8" angle={-45} textAnchor="end" height={100} />
                        <YAxis stroke="#94a3b8" />
                        <Tooltip 
                          contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                          labelStyle={{ color: '#f1f5f9' }}
                        />
                        <Bar dataKey="Balance" fill="#8b5cf6" radius={[8, 8, 0, 0]} />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                </div>

                {/* Account Type Distribution */}
                <div className="bg-slate-900/50 border border-slate-700 rounded-xl p-6">
                  <h3 className="text-xl font-bold text-white mb-4">Account Type Distribution</h3>
                  <div className="h-80">
                    <ResponsiveContainer width="100%" height="100%">
                      <RechartsPie>
                        <Pie
                          data={accountTypeDistribution}
                          cx="50%"
                          cy="50%"
                          labelLine={false}
                          label={({ name, percent }: any) => `${name}: ${(percent * 100).toFixed(0)}%`}
                          outerRadius={100}
                          fill="#8884d8"
                          dataKey="value"
                        >
                          {accountTypeDistribution.map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                          ))}
                        </Pie>
                        <Tooltip 
                          contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px' }}
                        />
                      </RechartsPie>
                    </ResponsiveContainer>
                  </div>
                </div>

                {/* Data Quality Metrics */}
                <div className="bg-slate-900/50 border border-slate-700 rounded-xl p-6">
                  <h3 className="text-xl font-bold text-white mb-4">Data Quality Metrics</h3>
                  <div className="space-y-4">
                    <div>
                      <div className="flex items-center justify-between mb-2">
                        <span className="text-slate-300">Customers</span>
                        <span className="text-blue-400 font-semibold">{stats.dataCompleteness.customers}%</span>
                      </div>
                      <div className="h-3 bg-slate-700 rounded-full overflow-hidden">
                        <div 
                          className="h-full bg-gradient-to-r from-blue-500 to-blue-600 transition-all duration-500"
                          style={{ width: `${stats.dataCompleteness.customers}%` }}
                        />
                      </div>
                    </div>

                    <div>
                      <div className="flex items-center justify-between mb-2">
                        <span className="text-slate-300">Vendors</span>
                        <span className="text-purple-400 font-semibold">{stats.dataCompleteness.vendors}%</span>
                      </div>
                      <div className="h-3 bg-slate-700 rounded-full overflow-hidden">
                        <div 
                          className="h-full bg-gradient-to-r from-purple-500 to-purple-600 transition-all duration-500"
                          style={{ width: `${stats.dataCompleteness.vendors}%` }}
                        />
                      </div>
                    </div>

                    <div>
                      <div className="flex items-center justify-between mb-2">
                        <span className="text-slate-300">Accounts</span>
                        <span className="text-green-400 font-semibold">{stats.dataCompleteness.accounts}%</span>
                      </div>
                      <div className="h-3 bg-slate-700 rounded-full overflow-hidden">
                        <div 
                          className="h-full bg-gradient-to-r from-green-500 to-green-600 transition-all duration-500"
                          style={{ width: `${stats.dataCompleteness.accounts}%` }}
                        />
                      </div>
                    </div>

                    <div className="pt-4 border-t border-slate-700">
                      <div className="flex items-center justify-between text-sm text-slate-400">
                        <span>Overall Completeness</span>
                        <span className="text-white font-bold">
                          {Math.round((stats.dataCompleteness.customers + stats.dataCompleteness.vendors + stats.dataCompleteness.accounts) / 3)}%
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Financial Reports Tab */}
        {activeTab === 'financial' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <div className="flex items-center justify-between mb-4">
                <button
                  onClick={() => setActiveTab('reports')}
                  className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
                >
                  <ArrowDownRight className="w-4 h-4 rotate-180" />
                  Back to Reports
                </button>

                {/* PDF Export Button */}
                <button
                  onClick={() => exportFinancialReportPDF(accounts, stats.totalRevenue, stats.totalExpenses)}
                  className="flex items-center gap-2 px-4 py-2 bg-green-500/20 hover:bg-green-500/30 text-green-400 rounded-xl transition-colors"
                >
                  <Download className="w-4 h-4" />
                  Export Financial Report PDF
                </button>
              </div>

              <div className="flex items-center gap-3 mb-6">
                <DollarSign className="w-8 h-8 text-green-400" />
                <h2 className="text-3xl font-bold text-white">Financial Reports</h2>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 border border-green-500/30 rounded-xl p-6">
                  <div className="text-sm text-slate-400 mb-2">Total Revenue</div>
                  <div className="text-3xl font-bold text-white mb-1">{formatCurrency(stats.totalRevenue)}</div>
                  <div className="text-xs text-green-400">From AR balances</div>
                </div>
                <div className="bg-gradient-to-br from-red-500/20 to-red-600/5 border border-red-500/30 rounded-xl p-6">
                  <div className="text-sm text-slate-400 mb-2">Total Expenses</div>
                  <div className="text-3xl font-bold text-white mb-1">{formatCurrency(stats.totalExpenses)}</div>
                  <div className="text-xs text-red-400">From AP balances</div>
                </div>
                <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 border border-blue-500/30 rounded-xl p-6">
                  <div className="text-sm text-slate-400 mb-2">Net Position</div>
                  <div className="text-3xl font-bold text-white mb-1">
                    {formatCurrency(stats.totalRevenue - stats.totalExpenses)}
                  </div>
                  <div className="text-xs text-blue-400">Revenue - Expenses</div>
                </div>
              </div>

              <div className="bg-slate-900/50 border border-slate-700 rounded-xl overflow-hidden">
                <div className="p-6 border-b border-slate-700">
                  <h3 className="text-xl font-bold text-white">Chart of Accounts</h3>
                  <p className="text-slate-400 text-sm mt-1">{stats.totalAccounts} accounts</p>
                </div>
                <div className="overflow-x-auto max-h-96 overflow-y-auto">
                  <table className="w-full">
                    <thead className="bg-slate-900/80 sticky top-0">
                      <tr>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Account</th>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Type</th>
                        <th className="text-right text-slate-300 font-semibold py-3 px-6">Balance</th>
                        <th className="text-center text-slate-300 font-semibold py-3 px-6">Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {accounts.slice(0, 20).map((account, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-3 px-6 text-white font-medium">{account.AccountName}</td>
                          <td className="py-3 px-6 text-slate-300">{account.AccountType}</td>
                          <td className="py-3 px-6 text-right text-green-400 font-semibold">
                            {formatCurrency(account.Balance)}
                          </td>
                          <td className="py-3 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              account.Active
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-red-500/20 text-red-400'
                            }`}>
                              {account.Active ? <CheckCircle className="w-3 h-3" /> : <XCircle className="w-3 h-3" />}
                              {account.Active ? 'Active' : 'Inactive'}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* AR/AP Aging Tab */}
        {activeTab === 'ar-ap-aging' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <div className="flex items-center justify-between mb-4">
                <button
                  onClick={() => setActiveTab('reports')}
                  className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
                >
                  <ArrowDownRight className="w-4 h-4 rotate-180" />
                  Back to Reports
                </button>
                
                {/* PDF Export Buttons */}
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => exportArApAgingPDF(customers, vendors, 'ar')}
                    className="flex items-center gap-2 px-4 py-2 bg-green-500/20 hover:bg-green-500/30 text-green-400 rounded-xl transition-colors"
                  >
                    <Download className="w-4 h-4" />
                    Export AR PDF
                  </button>
                  <button
                    onClick={() => exportArApAgingPDF(customers, vendors, 'ap')}
                    className="flex items-center gap-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-xl transition-colors"
                  >
                    <Download className="w-4 h-4" />
                    Export AP PDF
                  </button>
                  <button
                    onClick={() => exportArApAgingPDF(customers, vendors, 'both')}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-400 rounded-xl transition-colors"
                  >
                    <Download className="w-4 h-4" />
                    Export Both PDF
                  </button>
                </div>
              </div>

              <div className="flex items-center gap-3 mb-6">
                <TrendingUp className="w-8 h-8 text-blue-400" />
                <h2 className="text-3xl font-bold text-white">AR/AP Aging Reports</h2>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
                <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 border border-green-500/30 rounded-xl p-6">
                  <div className="flex items-center justify-between mb-4">
                    <div>
                      <div className="text-sm text-slate-400 mb-1">Accounts Receivable</div>
                      <div className="text-3xl font-bold text-white">{formatCurrency(stats.totalRevenue)}</div>
                    </div>
                    <ArrowUpRight className="w-10 h-10 text-green-400" />
                  </div>
                  <div className="text-xs text-slate-400">{stats.totalCustomers} customers</div>
                </div>

                <div className="bg-gradient-to-br from-red-500/20 to-red-600/5 border border-red-500/30 rounded-xl p-6">
                  <div className="flex items-center justify-between mb-4">
                    <div>
                      <div className="text-sm text-slate-400 mb-1">Accounts Payable</div>
                      <div className="text-3xl font-bold text-white">{formatCurrency(stats.totalExpenses)}</div>
                    </div>
                    <ArrowDownRight className="w-10 h-10 text-red-400" />
                  </div>
                  <div className="text-xs text-slate-400">{stats.totalVendors} vendors</div>
                </div>
              </div>

              <div className="bg-slate-900/50 border border-slate-700 rounded-xl overflow-hidden mb-6">
                <div className="p-6 border-b border-slate-700">
                  <h3 className="text-xl font-bold text-white">Accounts Receivable Details</h3>
                </div>
                <div className="overflow-x-auto max-h-96 overflow-y-auto">
                  <table className="w-full">
                    <thead className="bg-slate-900/80 sticky top-0">
                      <tr>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Customer</th>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Contact</th>
                        <th className="text-right text-slate-300 font-semibold py-3 px-6">Balance</th>
                        <th className="text-right text-slate-300 font-semibold py-3 px-6">Credit Limit</th>
                        <th className="text-center text-slate-300 font-semibold py-3 px-6">Utilization</th>
                      </tr>
                    </thead>
                    <tbody>
                      {customers.filter(c => c.Balance > 0).map((customer, index) => {
                        const utilizationNum = customer.CreditLimit > 0 
                          ? (customer.Balance / customer.CreditLimit * 100) 
                          : 0;
                        const utilization = utilizationNum.toFixed(0);
                        return (
                          <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                            <td className="py-3 px-6 text-white font-medium">{customer.CustomerName}</td>
                            <td className="py-3 px-6 text-slate-300">{customer.Contact || '-'}</td>
                            <td className="py-3 px-6 text-right">
                              <button
                                onClick={() => {
                                  setSelectedCustomerForInvoice(customer);
                                  loadInvoiceDetails(customer.CustomerID, 'customer');
                                }}
                                className="text-green-400 font-semibold hover:text-green-300 hover:underline"
                              >
                                {formatCurrency(customer.Balance)}
                              </button>
                            </td>
                            <td className="py-3 px-6 text-right text-slate-300">{formatCurrency(customer.CreditLimit)}</td>
                            <td className="py-3 px-6 text-center">
                              <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${
                                utilizationNum > 90 ? 'bg-red-500/20 text-red-400' :
                                utilizationNum > 70 ? 'bg-yellow-500/20 text-yellow-400' :
                                'bg-green-500/20 text-green-400'
                              }`}>
                                {utilization}%
                              </span>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              </div>

              <div className="bg-slate-900/50 border border-slate-700 rounded-xl overflow-hidden">
                <div className="p-6 border-b border-slate-700">
                  <h3 className="text-xl font-bold text-white">Accounts Payable Details</h3>
                </div>
                <div className="overflow-x-auto max-h-96 overflow-y-auto">
                  <table className="w-full">
                    <thead className="bg-slate-900/80 sticky top-0">
                      <tr>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Vendor</th>
                        <th className="text-left text-slate-300 font-semibold py-3 px-6">Contact</th>
                        <th className="text-right text-slate-300 font-semibold py-3 px-6">Balance</th>
                        <th className="text-center text-slate-300 font-semibold py-3 px-6">Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {vendors.filter(v => v.Balance > 0).map((vendor, index) => (
                        <tr key={index} className="border-t border-slate-700 hover:bg-slate-700/30 transition-colors">
                          <td className="py-3 px-6 text-white font-medium">{vendor.VendorName}</td>
                          <td className="py-3 px-6 text-slate-300">{vendor.Contact || '-'}</td>
                          <td className="py-3 px-6 text-right">
                            <button
                              onClick={() => {
                                setSelectedVendorForInvoice(vendor);
                                loadInvoiceDetails(vendor.VendorID, 'vendor');
                              }}
                              className="text-red-400 font-semibold hover:text-red-300 hover:underline"
                            >
                              {formatCurrency(vendor.Balance)}
                            </button>
                          </td>
                          <td className="py-3 px-6 text-center">
                            <span className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                              vendor.Status === 'Active'
                                ? 'bg-green-500/20 text-green-400'
                                : 'bg-red-500/20 text-red-400'
                            }`}>
                              {vendor.Status === 'Active' ? <CheckCircle className="w-3 h-3" /> : <XCircle className="w-3 h-3" />}
                              {vendor.Status}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Inventory Reports Tab */}
        {activeTab === 'inventory' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <button
                onClick={() => setActiveTab('reports')}
                className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
              >
                <ArrowDownRight className="w-4 h-4 rotate-180" />
                Back to Reports
              </button>

              <div className="flex items-center gap-3 mb-6">
                <Package className="w-8 h-8 text-purple-400" />
                <h2 className="text-3xl font-bold text-white">Inventory Reports</h2>
              </div>

              <div className="bg-blue-500/10 border border-blue-500/30 rounded-xl p-6">
                <div className="flex items-start gap-4">
                  <Database className="w-6 h-6 text-blue-400 flex-shrink-0 mt-1" />
                  <div>
                    <h4 className="text-white font-semibold mb-2">Inventory Data Available</h4>
                    <p className="text-slate-300 text-sm mb-4">
                      Your Peachtree database contains LineItem, BOMItems, InventoryCosts, and InventoryChains tables 
                      for comprehensive inventory management and reporting.
                    </p>
                    <p className="text-slate-400 text-xs mb-3">
                      Available reports: Stock levels, Inventory valuation, Movement analysis, Reorder points, 
                      Serial/Lot tracking, and more.
                    </p>
                    <div className="bg-slate-900/50 border border-slate-700 rounded-lg p-4">
                      <h5 className="text-white font-medium mb-2 text-sm">Tables:</h5>
                      <ul className="text-xs text-slate-300 space-y-1">
                        <li>• <span className="text-purple-400 font-mono">LineItem</span> - Invoice/Purchase line details</li>
                        <li>• <span className="text-purple-400 font-mono">BOMItems</span> - Bill of Materials components</li>
                        <li>• <span className="text-purple-400 font-mono">InventoryCosts</span> - Cost tracking & valuation</li>
                        <li>• <span className="text-purple-400 font-mono">InventoryChains</span> - Serial/Lot number tracking</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Job/Project Reports Tab */}
        {activeTab === 'jobs' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <button
                onClick={() => setActiveTab('reports')}
                className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
              >
                <ArrowDownRight className="w-4 h-4 rotate-180" />
                Back to Reports
              </button>

              <div className="flex items-center gap-3 mb-6">
                <BarChart3 className="w-8 h-8 text-orange-400" />
                <h2 className="text-3xl font-bold text-white">Job & Project Reports</h2>
              </div>

              <div className="bg-blue-500/10 border border-blue-500/30 rounded-xl p-6">
                <div className="flex items-start gap-4">
                  <Database className="w-6 h-6 text-blue-400 flex-shrink-0 mt-1" />
                  <div>
                    <h4 className="text-white font-semibold mb-2">Job Costing Data Available</h4>
                    <p className="text-slate-300 text-sm mb-4">
                      Your Peachtree database includes Jobs, JobEst, Phase, and General_Jobs tables for 
                      comprehensive project tracking and profitability analysis.
                    </p>
                    <p className="text-slate-400 text-xs mb-3">
                      Available reports: Job profitability, Cost vs Estimate, Phase tracking, Time & Materials, 
                      Job status summaries, and more.
                    </p>
                    <div className="bg-slate-900/50 border border-slate-700 rounded-lg p-4">
                      <h5 className="text-white font-medium mb-2 text-sm">Tables:</h5>
                      <ul className="text-xs text-slate-300 space-y-1">
                        <li>• <span className="text-orange-400 font-mono">Jobs</span> - Job/Project master records</li>
                        <li>• <span className="text-orange-400 font-mono">JobEst</span> - Job estimates & budgets</li>
                        <li>• <span className="text-orange-400 font-mono">Phase</span> - Project phase tracking</li>
                        <li>• <span className="text-orange-400 font-mono">General_Jobs</span> - Job cost transactions</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Payroll Reports Tab */}
        {activeTab === 'payroll' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <button
                onClick={() => setActiveTab('reports')}
                className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
              >
                <ArrowDownRight className="w-4 h-4 rotate-180" />
                Back to Reports
              </button>

              <div className="flex items-center gap-3 mb-6">
                <Users className="w-8 h-8 text-pink-400" />
                <h2 className="text-3xl font-bold text-white">Payroll Reports</h2>
              </div>

              <div className="bg-blue-500/10 border border-blue-500/30 rounded-xl p-6">
                <div className="flex items-start gap-4">
                  <Database className="w-6 h-6 text-blue-400 flex-shrink-0 mt-1" />
                  <div>
                    <h4 className="text-white font-semibold mb-2">Payroll Data Available</h4>
                    <p className="text-slate-300 text-sm mb-4">
                      Your Peachtree database contains Employee, EmployeePayInfo, EarningSummary, and 
                      General_PR tables for comprehensive payroll management and reporting.
                    </p>
                    <p className="text-slate-400 text-xs mb-3">
                      Available reports: Employee earnings, Payroll register, Tax summaries, Deduction reports, 
                      Wage analysis, Year-to-date summaries, and more.
                    </p>
                    <div className="bg-slate-900/50 border border-slate-700 rounded-lg p-4">
                      <h5 className="text-white font-medium mb-2 text-sm">Tables:</h5>
                      <ul className="text-xs text-slate-300 space-y-1">
                        <li>• <span className="text-pink-400 font-mono">Employee</span> - Employee master records</li>
                        <li>• <span className="text-pink-400 font-mono">EmployeePayInfo</span> - Pay rates & deductions</li>
                        <li>• <span className="text-pink-400 font-mono">EarningSummary</span> - Earnings summaries</li>
                        <li>• <span className="text-pink-400 font-mono">General_PR</span> - Payroll transactions</li>
                        <li>• <span className="text-pink-400 font-mono">Raise_History</span> - Salary change history</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Cash Flow Reports Tab */}
        {activeTab === 'cashflow' && (
          <div className="space-y-6">
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-8">
              <button
                onClick={() => setActiveTab('reports')}
                className="mb-4 flex items-center gap-2 text-slate-400 hover:text-white transition-colors"
              >
                <ArrowDownRight className="w-4 h-4 rotate-180" />
                Back to Reports
              </button>

              <div className="flex items-center gap-3 mb-6">
                <Wallet className="w-8 h-8 text-cyan-400" />
                <h2 className="text-3xl font-bold text-white">Cash Flow Reports</h2>
              </div>

              <div className="bg-blue-500/10 border border-blue-500/30 rounded-xl p-6">
                <div className="flex items-start gap-4">
                  <Database className="w-6 h-6 text-blue-400 flex-shrink-0 mt-1" />
                  <div>
                    <h4 className="text-white font-semibold mb-2">Banking & Cash Flow Data Available</h4>
                    <p className="text-slate-300 text-sm mb-4">
                      Your Peachtree database includes BankRecords, CashFlow, CashFlowTransaction, and 
                      CashFlowAccount tables for comprehensive cash management and flow analysis.
                    </p>
                    <p className="text-slate-400 text-xs mb-3">
                      Available reports: Bank account balances, Cash flow statements, Transaction analysis, 
                      Reconciliation reports, Liquidity analysis, and more.
                    </p>
                    <div className="bg-slate-900/50 border border-slate-700 rounded-lg p-4">
                      <h5 className="text-white font-medium mb-2 text-sm">Tables:</h5>
                      <ul className="text-xs text-slate-300 space-y-1">
                        <li>• <span className="text-cyan-400 font-mono">BankRecords</span> - Bank account details</li>
                        <li>• <span className="text-cyan-400 font-mono">CashFlow</span> - Cash flow master records</li>
                        <li>• <span className="text-cyan-400 font-mono">CashFlowTransaction</span> - Cash movements</li>
                        <li>• <span className="text-cyan-400 font-mono">CashFlowAccount</span> - Account mappings</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Invoice Details Popup Modal */}
      {(selectedCustomerForInvoice || selectedVendorForInvoice) && (
        <div className="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-slate-800 border border-slate-700 rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col shadow-2xl">
            {/* Modal Header */}
            <div className="bg-gradient-to-r from-blue-600 to-purple-600 p-6 flex items-center justify-between">
              <div>
                <h3 className="text-2xl font-bold text-white mb-1">Invoice Details</h3>
                <p className="text-blue-100">
                  {selectedCustomerForInvoice 
                    ? selectedCustomerForInvoice.CustomerName || selectedCustomerForInvoice.Customer_Bill_Name
                    : selectedVendorForInvoice?.VendorName}
                </p>
              </div>
              <div className="flex items-center gap-2">
                {/* PDF Export Button */}
                <button
                  onClick={() => {
                    if (invoiceDetails && invoiceDetails.length > 0) {
                      exportInvoiceDetailsPDF(
                        invoiceDetails,
                        selectedCustomerForInvoice || selectedVendorForInvoice,
                        selectedCustomerForInvoice ? 'customer' : 'vendor'
                      );
                    }
                  }}
                  disabled={!invoiceDetails || invoiceDetails.length === 0}
                  className="flex items-center gap-2 px-4 py-2 bg-white/10 hover:bg-white/20 disabled:bg-white/5 disabled:cursor-not-allowed rounded-xl transition-colors"
                  title="Export to PDF"
                >
                  <Download className="w-5 h-5 text-white" />
                  <span className="text-white font-medium">Export PDF</span>
                </button>
                <button
                  onClick={() => {
                    setSelectedCustomerForInvoice(null);
                    setSelectedVendorForInvoice(null);
                    setInvoiceDetails(null);
                  }}
                  className="w-10 h-10 bg-white/10 hover:bg-white/20 rounded-xl flex items-center justify-center transition-colors"
                >
                  <XCircle className="w-6 h-6 text-white" />
                </button>
              </div>
            </div>

            {/* Modal Body */}
            <div className="flex-1 overflow-y-auto p-6">
              {loadingInvoices ? (
                <div className="flex items-center justify-center py-12">
                  <div className="text-center">
                    <div className="w-12 h-12 border-4 border-blue-500/30 border-t-blue-500 rounded-full animate-spin mx-auto mb-4"></div>
                    <p className="text-slate-400">Loading invoice details...</p>
                  </div>
                </div>
              ) : invoiceDetails && invoiceDetails.length > 0 ? (
                <div className="space-y-4">
                  {/* 🆕 Enhanced Financial Summary */}
                  <div className="bg-gradient-to-br from-slate-900/80 to-slate-800/50 border border-slate-700 rounded-xl p-6 mb-6">
                    <h4 className="text-lg font-semibold text-white mb-4 flex items-center gap-2">
                      <DollarSign className="w-5 h-5 text-green-400" />
                      Financial Summary
                    </h4>
                    
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                      {/* Current Balance */}
                      <div className="bg-amber-500/10 border border-amber-500/30 rounded-xl p-4">
                        <div className="text-xs text-slate-400 mb-1 uppercase tracking-wide">Current Balance</div>
                        <div className="text-2xl font-bold text-amber-400">
                          {formatCurrency(
                            selectedCustomerForInvoice 
                              ? (selectedCustomerForInvoice.CurrentBalance || selectedCustomerForInvoice.Balance || 0)
                              : (selectedVendorForInvoice?.CurrentBalance || selectedVendorForInvoice?.Balance || 0)
                          )}
                        </div>
                        <div className="text-xs text-slate-500 mt-1">Amount outstanding</div>
                      </div>

                      {/* Total Debits */}
                      <div className="bg-red-500/10 border border-red-500/30 rounded-xl p-4">
                        <div className="text-xs text-slate-400 mb-1 uppercase tracking-wide">Total Debits</div>
                        <div className="text-2xl font-bold text-red-400">
                          {formatCurrency(
                            invoiceDetails
                              .filter((inv: any) => inv.Amount > 0)
                              .reduce((sum: number, inv: any) => sum + (inv.Amount || 0), 0)
                          )}
                        </div>
                        <div className="text-xs text-slate-500 mt-1">
                          {invoiceDetails.filter((inv: any) => inv.Amount > 0).length} charges
                        </div>
                      </div>

                      {/* Total Credits */}
                      <div className="bg-green-500/10 border border-green-500/30 rounded-xl p-4">
                        <div className="text-xs text-slate-400 mb-1 uppercase tracking-wide">Total Credits</div>
                        <div className="text-2xl font-bold text-green-400">
                          {formatCurrency(
                            Math.abs(invoiceDetails
                              .filter((inv: any) => inv.Amount < 0)
                              .reduce((sum: number, inv: any) => sum + (inv.Amount || 0), 0))
                          )}
                        </div>
                        <div className="text-xs text-slate-500 mt-1">
                          {invoiceDetails.filter((inv: any) => inv.Amount < 0).length} payments
                        </div>
                      </div>

                      {/* Total Transactions */}
                      <div className="bg-blue-500/10 border border-blue-500/30 rounded-xl p-4">
                        <div className="text-xs text-slate-400 mb-1 uppercase tracking-wide">Transactions</div>
                        <div className="text-2xl font-bold text-blue-400">
                          {invoiceDetails.length}
                        </div>
                        <div className="text-xs text-slate-500 mt-1">
                          {invoiceDetails.filter((inv: any) => inv.Amount !== 0).length} with amounts
                        </div>
                      </div>
                    </div>

                    {/* Account Activity Summary */}
                    <div className="mt-6 bg-slate-800/50 rounded-lg p-4">
                      <div className="flex items-center justify-between mb-2">
                        <span className="text-sm text-slate-400">Account Activity</span>
                        <span className="text-xs text-slate-500">
                          Net: {formatCurrency(
                            invoiceDetails.reduce((sum: number, inv: any) => sum + (inv.Amount || 0), 0)
                          )}
                        </span>
                      </div>
                      <div className="grid grid-cols-2 gap-4 text-sm">
                        <div>
                          <span className="text-slate-400">Debit Entries:</span>
                          <span className="text-red-400 font-semibold ml-2">
                            {invoiceDetails.filter((inv: any) => inv.Amount > 0).length}
                          </span>
                        </div>
                        <div>
                          <span className="text-slate-400">Credit Entries:</span>
                          <span className="text-green-400 font-semibold ml-2">
                            {invoiceDetails.filter((inv: any) => inv.Amount < 0).length}
                          </span>
                        </div>
                        <div>
                          <span className="text-slate-400">Zero Entries:</span>
                          <span className="text-slate-500 font-semibold ml-2">
                            {invoiceDetails.filter((inv: any) => inv.Amount === 0).length}
                          </span>
                        </div>
                        <div>
                          <span className="text-slate-400">Latest Entry:</span>
                          <span className="text-blue-400 font-semibold ml-2">
                            {invoiceDetails[0]?.Date || 'N/A'}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Invoice Table */}
                  <div className="bg-slate-900/50 border border-slate-700 rounded-xl overflow-hidden shadow-xl">
                    <table className="w-full">
                      <thead className="bg-gradient-to-r from-slate-900 to-slate-800 border-b-2 border-slate-600">
                        <tr>
                          <th className="text-left text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            <div className="flex items-center gap-2">
                              <Receipt className="w-4 h-4" />
                              Invoice #
                            </div>
                          </th>
                          <th className="text-left text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            <div className="flex items-center gap-2">
                              <Calendar className="w-4 h-4" />
                              Date
                            </div>
                          </th>
                          <th className="text-left text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            Type
                          </th>
                          <th className="text-left text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            Description
                          </th>
                          <th className="text-right text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            <div className="flex items-center justify-end gap-2">
                              <DollarSign className="w-4 h-4" />
                              Amount
                            </div>
                          </th>
                          <th className="text-left text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            Due Date
                          </th>
                          <th className="text-center text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            Status
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {invoiceDetails.map((invoice: any, index: number) => {
                          const isDebit = invoice.Amount >= 0;
                          const isPosted = invoice.Status === 'Paid' || invoice.Status === 'Posted';
                          
                          return (
                            <tr key={index} className="border-t border-slate-700 hover:bg-slate-800/50 transition-all duration-200">
                              {/* Invoice/Transaction ID with better visibility */}
                              <td className="py-4 px-4">
                                <div className="flex items-center gap-2">
                                  <div className={`w-2 h-2 rounded-full ${isPosted ? 'bg-green-500' : 'bg-yellow-500'}`}></div>
                                  <span className="text-blue-400 font-mono text-sm font-medium">
                                    {invoice.InvoiceID || invoice.TransactionID || 'N/A'}
                                  </span>
                                </div>
                              </td>
                              
                              {/* Date with better formatting */}
                              <td className="py-4 px-4">
                                <div className="text-slate-300 font-medium">{invoice.Date || '-'}</div>
                              </td>
                              
                              {/* Type badge with icon */}
                              <td className="py-4 px-4">
                                <span className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold ${
                                  invoice.Type === 'Invoice' 
                                    ? 'bg-blue-500/20 text-blue-400 border border-blue-500/30' 
                                    : invoice.Type === 'Transaction'
                                    ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30'
                                    : 'bg-purple-500/20 text-purple-400 border border-purple-500/30'
                                }`}>
                                  {invoice.Type === 'Invoice' && <Receipt className="w-3 h-3" />}
                                  {invoice.Type === 'Transaction' && <TrendingUp className="w-3 h-3" />}
                                  {invoice.Type || 'Other'}
                                </span>
                              </td>
                              
                              {/* Description with truncation */}
                              <td className="py-4 px-4">
                                <div className="text-white font-medium max-w-xs truncate" title={invoice.Description}>
                                  {invoice.Description || '-'}
                                </div>
                              </td>
                              
                              {/* Amount with debit/credit label */}
                              <td className="py-4 px-4 text-right">
                                <div className="flex flex-col items-end gap-1">
                                  <span className={`text-lg font-bold ${
                                    isDebit ? 'text-red-400' : 'text-green-400'
                                  }`}>
                                    {isDebit ? '+' : '-'} {formatCurrency(Math.abs(invoice.Amount))}
                                  </span>
                                  <span className={`text-xs font-medium ${
                                    isDebit ? 'text-red-400/60' : 'text-green-400/60'
                                  }`}>
                                    {isDebit ? 'Debit' : 'Credit'}
                                  </span>
                                </div>
                              </td>
                              
                              {/* Due date with highlighting */}
                              <td className="py-4 px-4">
                                {invoice.DueDate ? (
                                  <div className="text-slate-300 font-medium">{invoice.DueDate}</div>
                                ) : (
                                  <span className="text-slate-500 text-sm">-</span>
                                )}
                              </td>
                              
                              {/* Status with larger badge */}
                              <td className="py-4 px-4 text-center">
                                <span className={`inline-flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-semibold ${
                                  isPosted
                                    ? 'bg-green-500/20 text-green-400 border border-green-500/30'
                                    : 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30'
                                }`}>
                                  {isPosted 
                                    ? <CheckCircle className="w-4 h-4" /> 
                                    : <Clock className="w-4 h-4" />}
                                  <span className="font-bold">{invoice.Status}</span>
                                </span>
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                </div>
              ) : (
                <div className="text-center py-16">
                  <div className="relative mb-6">
                    <div className="w-24 h-24 bg-gradient-to-br from-slate-700/50 to-slate-800/50 rounded-full flex items-center justify-center mx-auto border-4 border-slate-600/30">
                      <Receipt className="w-12 h-12 text-slate-400" />
                    </div>
                    <div className="absolute top-0 left-1/2 -translate-x-1/2 w-32 h-32 bg-blue-500/10 rounded-full blur-2xl"></div>
                  </div>
                  <h3 className="text-xl font-bold text-slate-300 mb-2">No Transaction Records Found</h3>
                  <p className="text-slate-400 mb-1">This account has no invoice or transaction history.</p>
                  <p className="text-slate-500 text-sm">Data may not be available in the Peachtree system.</p>
                </div>
              )}
            </div>

            {/* Modal Footer */}
            <div className="bg-slate-900/50 border-t border-slate-700 p-4 flex items-center justify-end gap-3">
              <button
                onClick={() => {
                  if (invoiceDetails) {
                    exportToCSV(invoiceDetails, `invoices_${selectedCustomerForInvoice?.CustomerID || selectedVendorForInvoice?.VendorID}`);
                  }
                }}
                disabled={!invoiceDetails || invoiceDetails.length === 0}
                className="flex items-center gap-2 px-4 py-2 bg-blue-500/20 hover:bg-blue-500/30 disabled:bg-slate-700/50 disabled:text-slate-500 text-blue-400 rounded-xl transition-colors"
              >
                <Download className="w-4 h-4" />
                Export CSV
              </button>
              <button
                onClick={() => {
                  setSelectedCustomerForInvoice(null);
                  setSelectedVendorForInvoice(null);
                  setInvoiceDetails(null);
                }}
                className="flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white rounded-xl transition-colors"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Logs Modal */}
      {showLogs && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-sm flex items-center justify-center z-[60] p-4">
          <div className="bg-slate-800/95 border border-slate-700 rounded-2xl shadow-2xl max-w-4xl w-full max-h-[80vh] flex flex-col">
            {/* Modal Header */}
            <div className="bg-gradient-to-r from-slate-800 to-slate-900 border-b border-slate-700 p-6 rounded-t-2xl">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-500/20 rounded-xl">
                    <FileText className="w-6 h-6 text-blue-400" />
                  </div>
                  <div>
                    <h2 className="text-2xl font-bold text-white">Bridge Logs</h2>
                    <p className="text-slate-400 text-sm mt-1">
                      {bridgeLogs.length} log {bridgeLogs.length === 1 ? 'entry' : 'entries'}
                    </p>
                  </div>
                </div>
                <button
                  onClick={() => setShowLogs(false)}
                  className="p-2 hover:bg-slate-700/50 rounded-xl transition-colors"
                >
                  <X className="w-6 h-6 text-slate-400 hover:text-white" />
                </button>
              </div>
            </div>

            {/* Logs Content */}
            <div className="flex-1 overflow-y-auto p-6">
              {bridgeLogs.length > 0 ? (
                <div className="bg-slate-900/70 rounded-xl border border-slate-700 p-4 font-mono text-sm">
                  {bridgeLogs.map((log, idx) => {
                    // Parse log entry
                    const isError = log.includes('[ERROR]') || log.includes('[STDERR]');
                    const isInfo = log.includes('[INFO]');
                    const isStdout = log.includes('[STDOUT]');
                    
                    return (
                      <div 
                        key={idx} 
                        className={`py-1 px-2 rounded ${
                          isError 
                            ? 'text-red-400 bg-red-500/10' 
                            : isInfo
                            ? 'text-blue-400 bg-blue-500/5'
                            : isStdout
                            ? 'text-green-400 bg-green-500/5'
                            : 'text-slate-300'
                        }`}
                      >
                        {log}
                      </div>
                    );
                  })}
                </div>
              ) : (
                <div className="text-center py-12">
                  <FileText className="w-16 h-16 text-slate-600 mx-auto mb-4" />
                  <p className="text-slate-400">No logs available</p>
                </div>
              )}
            </div>

            {/* Modal Footer */}
            <div className="bg-slate-900/50 border-t border-slate-700 p-4 flex items-center justify-end gap-3">
              <button
                onClick={() => {
                  const logText = bridgeLogs.join('\n');
                  navigator.clipboard.writeText(logText);
                  alert('Logs copied to clipboard!');
                }}
                disabled={bridgeLogs.length === 0}
                className="flex items-center gap-2 px-4 py-2 bg-blue-500/20 hover:bg-blue-500/30 disabled:bg-slate-700/50 disabled:text-slate-500 text-blue-400 rounded-xl transition-colors"
              >
                <FileText className="w-4 h-4" />
                Copy Logs
              </button>
              <button
                onClick={() => setShowLogs(false)}
                className="flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white rounded-xl transition-colors"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
