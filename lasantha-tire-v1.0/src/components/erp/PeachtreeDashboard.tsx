'use client';

import { useState, useEffect, useMemo } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import { 
  Users, Building2, DollarSign, FileText, TrendingUp, 
  Package, Home, RefreshCw, Download, Search, Filter,
  Clock, BarChart3, PieChart, Calendar, ShoppingCart,
  CreditCard, Wallet, Receipt, AlertCircle, CheckCircle,
  XCircle, ArrowUpRight, ArrowDownRight, Activity, Database, X,
  ArrowLeft, Bell, Menu, Plus, Share2, Truck, HelpCircle, AlertTriangle, Settings
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
import { 
  getPeachtreeSchema,
  listModules,
  listTablesByModule,
  PeachtreeTableDefinition
} from '@/config/peachtreeSchema';
import { useBridgeMonitor } from '@/hooks/useBridgeMonitor';
import { useToast } from '@/contexts/ToastContext';
import { 
  TableLoadingSkeleton, 
  CardLoadingSkeleton, 
  StatLoadingSkeleton,
  ChartLoadingSkeleton,
  ListLoadingSkeleton,
  MobileCardLoadingSkeleton,
  DashboardLoadingSkeleton
} from '@/components/ui/LoadingSkeletons';
import {
  NoCustomersState,
  NoVendorsState,
  NoAccountsState,
  NoTransactionsState,
  SearchNoResultsState,
  FilterNoResultsState
} from '@/components/ui/EmptyStates';
import {
  exportCustomersToExcel,
  exportVendorsToExcel,
  exportAccountsToExcel,
  exportTransactionsToExcel,
  exportDashboardToExcel
} from '@/utils/excelExport';

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
  const { showToast } = useToast();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [activeTab, setActiveTab] = useState<'dashboard' | 'customers' | 'vendors' | 'accounts' | 'transactions' | 'reports' | 'analytics' | 'financial' | 'ar-ap-aging' | 'inventory' | 'jobs' | 'payroll' | 'cashflow'>('dashboard');
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [connected, setConnected] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // 🆕 Bridge monitoring with auto-restart
  const bridgeMonitor = useBridgeMonitor({
    checkInterval: 10000,
    maxRetries: 3,
    retryDelay: 5000,
    autoRestart: true,
    onStatusChange: (status) => {
      console.log(`[Bridge Monitor] Status changed: ${status}`);
      
      if (status === 'restarting') {
        showToast('Attempting to restart Peachtree Bridge...', 'warning');
      } else if (status === 'running') {
        showToast('Peachtree Bridge is running', 'success');
      } else if (status === 'stopped') {
        showToast('Peachtree Bridge is not running', 'error');
      }
    }
  });

  const [bridgeLogs, setBridgeLogs] = useState<string[]>([]);
  const [showLogs, setShowLogs] = useState(false);
  const [startingBridge, setStartingBridge] = useState(false);

  const schema = useMemo(() => getPeachtreeSchema(), []);
  const schemaModules = useMemo(() => listModules(), []);
  const schemaModuleOptions = useMemo(() => ['All Modules', ...schemaModules], [schemaModules]);
  const schemaFallbackTables = useMemo(() => (
    schema.tables.map((table) => ({ name: table.name, type: 'TABLE' }))
  ), [schema]);
  const [selectedSchemaModule, setSelectedSchemaModule] = useState<string>(() => schemaModuleOptions[0] || 'All Modules');
  const catalogTables = useMemo<PeachtreeTableDefinition[]>(() => (
    selectedSchemaModule === 'All Modules' || !selectedSchemaModule
      ? schema.tables
      : listTablesByModule(selectedSchemaModule)
  ), [selectedSchemaModule, schema]);
  
  // Filter states (Enhanced)
  const [hideZeroBalance, setHideZeroBalance] = useState(false); // 🔄 Changed to false - Show ALL customers/vendors
  const [hideInactive, setHideInactive] = useState(false);
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

  // 🆕 Mobile View State
  const [mobileView, setMobileView] = useState<'home' | 'customers' | 'vendors' | 'accounts' | 'reports' | 'search' | 'alerts' | 'menu' | 'quick-actions'>('home');
  const [mobileSearchTerm, setMobileSearchTerm] = useState('');
  const [mobileCustomerFilter, setMobileCustomerFilter] = useState<'all' | 'active' | 'balance'>('all');
  const [mobileVendorFilter, setMobileVendorFilter] = useState<'all' | 'active' | 'balance'>('all');
  const [mobileAccountFilter, setMobileAccountFilter] = useState<'all' | 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense'>('all');
  const [mobileTransactionFilter, setMobileTransactionFilter] = useState<'all' | 'Posted' | 'Draft'>('all');
  const [mobileSelectedReport, setMobileSelectedReport] = useState<string | null>(null);
  const [mobileLoading, setMobileLoading] = useState(false);
  const [mobileError, setMobileError] = useState<string | null>(null);

  // 🆕 Mobile Report Generation State
  const [mobileReportType, setMobileReportType] = useState<string | null>(null);
  const [mobileReportSubtype, setMobileReportSubtype] = useState<string>('summary');
  const [mobileReportDateRange, setMobileReportDateRange] = useState<'thisMonth' | 'lastMonth' | 'ytd' | 'custom'>('thisMonth');
  const [mobileReportGenerating, setMobileReportGenerating] = useState(false);
  const [mobileReportResult, setMobileReportResult] = useState<any>(null);

  // 🆕 Advanced Mobile Reports State
  const [mobileReportStep, setMobileReportStep] = useState<'menu' | 'config' | 'view'>('menu');
  const [selectedReportCategory, setSelectedReportCategory] = useState<'financial' | 'sales' | 'customers' | null>(null);
  const [reportDateRange, setReportDateRange] = useState<'thisMonth' | 'lastMonth' | 'ytd' | 'all'>('thisMonth');

  
  // Business Status data
  const [arAging, setArAging] = useState<any>(null);
  const [apAging, setApAging] = useState<any>(null);
  const [cashBalances, setCashBalances] = useState<any>(null);
  const [topCustomers, setTopCustomers] = useState<any[]>([]);
  const [topVendors, setTopVendors] = useState<any[]>([]);
  
  // Data states
  const [tables, setTables] = useState<PeachtreeTable[]>(schemaFallbackTables);
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

  // 🆕 Bridge Status Polling - Replaced with useBridgeMonitor hook above

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
      console.log('🚀 [LOAD] Starting data load...');
      
      // 🔍 First check if bridge is actually running
      const bridgeStatusRes = await fetch('/api/peachtree-bridge');
      const bridgeStatus = await bridgeStatusRes.json();
      
      console.log('🔍 [BRIDGE] Bridge status:', bridgeStatus);
      
      if (bridgeStatus.status !== 'running') {
        console.warn('⚠️ [BRIDGE] Bridge not running, clearing data');
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

      console.log('✅ [BRIDGE] Bridge is running, checking health...');

      // 🔍 Check ODBC connection health
      try {
        const healthRes = await fetch('/api/erp/peachtree?endpoint=health');
        const healthData = await healthRes.json();
        
        console.log('✅ [HEALTH] Health check response:', healthData);
        
        // Bridge is connected if status is 'ok' OR connected field is true
        const isConnected = healthData.status === 'ok' || healthData.connected === true;
        setConnected(isConnected);
        
        if (!isConnected) {
          console.warn('⚠️ [HEALTH] Not connected, but will try to load data anyway');
        } else {
          console.log('✅ [HEALTH] Connected successfully!');
        }
      } catch (healthError: any) {
        console.error('⚠️ [HEALTH] Health check failed (continuing anyway):', healthError.message);
        setConnected(false);
      }

      console.log('📊 [DATA] Loading all data...');

      // ✅ Load all data in parallel - FRESH from bridge
      await Promise.all([
        loadTables(),
        loadCustomers(),
        loadVendors(),
        loadAccounts(),
        loadTransactions(),
        loadBusinessStatusData()
      ]);

      console.log('✅ [DATA] All data loaded successfully');
      calculateStats();
      
      // Clear any previous errors
      setError(null);
      
    } catch (error: any) {
      console.error('❌ [ERROR] Failed to load Peachtree data:', error);
      setError(error.message || 'Failed to connect to Peachtree');
      setConnected(false);
    } finally {
      setLoading(false);
      console.log('🏁 [LOAD] Load complete');
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
          return;
        }
      }
      setTables(schemaFallbackTables);
    } catch (error) {
      console.error('Error loading tables:', error);
      setTables(schemaFallbackTables);
    }
  };

  const loadCustomers = async () => {
    try {
      setMobileError(null);
      // Always fetch fresh customer data from bridge (no-cache)
      const res = await fetch(`/api/erp/peachtree?endpoint=customers&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });

      
      if (!res.ok) {
        const errorText = await res.text();
        console.error('❌ [DEBUG] Customers request failed:', res.status, errorText);
        setCustomers([]);
        setMobileError(`Server error ${res.status} while loading customers`);
        return;
      }

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
          setMobileError(null);

        }
        else {
          console.warn('⚠️ [DEBUG] Customers response missing data');
          setCustomers([]);
          setMobileError('Peachtree returned no customers');
        }
      }
    } catch (error) {
      console.error('❌ [DEBUG] Error loading customers:', error);
      setCustomers([]);
      setMobileError('Failed to load customers from Peachtree');
    }
  };

  const loadVendors = async () => {
    try {
      setMobileError(null);
      // Always fetch fresh vendor data from bridge (no-cache)
      const res = await fetch(`/api/erp/peachtree?endpoint=vendors&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });

      
      if (!res.ok) {
        const errorText = await res.text();
        console.error('❌ [DEBUG] Vendors request failed:', res.status, errorText);
        setVendors([]);
        setMobileError(`Server error ${res.status} while loading vendors`);
        return;
      }

      if (res.ok) {
        const data = await res.json();
        console.log('🔍 [DEBUG] Vendors data:', {
          success: data.success,
          count: data.count,
          dataLength: data.data?.length,
          firstRecord: data.data?.[0]
        });
        
        if (data.success && data.data) {
                  setMobileError(null);
          const transformedData = data.data.map((row: any, index: number) => {
            // Try multiple fields for vendor name
            const vendorName = row.Vendor_Bill_Name?.trim() || 
                              row.VendorName?.trim() || 
                              row.CompanyName?.trim() || 
              setMobileError('Failed to load vendors from Peachtree');
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
      if (!res.ok) {
        console.warn('⚠️ [ACCOUNTS] Response not OK:', res.status);
        setAccounts([]);
        return;
      }

      if (res.ok) {
        const data = await res.json();
        if (data.success && data.data) {
          const transformedData = data.data.map((row: any, index: number) => {
            // Try multiple fields for account name/description
            const accountName = row.Description?.trim() || 
                               row.AccountName?.trim() || 
                               row.Account_Name?.trim() || 
                               row.AccountID?.trim() || 
                               `Account ${index + 1}`;
            
            // Better account type mapping
            const getAccountType = (typeNum: any) => {
              const types: Record<number, string> = {
                1: 'Asset',
                2: 'Liability',
                3: 'Equity',
                4: 'Revenue',
                5: 'Expense',
                6: 'Cost of Sales'
              };
              return types[parseInt(typeNum)] || `Type ${typeNum}` || 'General';
            };
            
            return {
              AccountID: row.AccountID || `ACC-${index + 1}`,
              AccountName: accountName,
              AccountType: getAccountType(row.Account_Type_Number),
              Balance: parseFloat(row.Balance || 0),
              Active: row.Inactive === 0
            };
          });
          setAccounts(transformedData);
        } else {
          console.warn('⚠️ [ACCOUNTS] Peachtree returned no account data');
          setAccounts([]);
        }
      }
    } catch (error) {
      console.error('Error loading accounts:', error);
      setAccounts([]);
    }
  };

  const loadTransactions = async () => {
    try {
      const res = await fetch(`/api/erp/peachtree?endpoint=transactions&_=${Date.now()}`, { cache: 'no-store', headers: { 'Cache-Control': 'no-store' } });

      if (!res.ok) {
        console.warn('⚠️ [TXNS] Response not OK:', res.status);
        setTransactions([]);
        return;
      }

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
        } else {
          console.warn('⚠️ [TXNS] Peachtree returned no transaction data');
          setTransactions([]);
        }
      }
    } catch (error) {
      console.error('Error loading transactions:', error);
      setTransactions([]);
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
          // No data found
          setInvoiceDetails([]);
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
      a.Description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.AccountType?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (hideZeroBalance) {
      filtered = filtered.filter((a: any) => Math.abs(a.Balance || 0) > 0);
    }

    if (hideInactive) {
      filtered = filtered.filter((a: any) => a.Active === true || a.Active === 'Y');
    }

    // Apply sorting
    filtered.sort((a: any, b: any) => {
      let compareValue = 0;
      
      if (sortBy === 'name') {
        compareValue = (a.Description || '').localeCompare(b.Description || '');
      } else if (sortBy === 'balance') {
        compareValue = (a.Balance || 0) - (b.Balance || 0);
      }
      
      return sortOrder === 'asc' ? compareValue : -compareValue;
    });

    return filtered;
  };

  if (isAuthenticated === null || loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center relative overflow-hidden">
        {/* Background Effects */}
        <div className="absolute inset-0 overflow-hidden pointer-events-none">
          <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-blue-600/20 rounded-full blur-[120px] animate-pulse"></div>
          <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-purple-600/20 rounded-full blur-[120px] animate-pulse delay-1000"></div>
        </div>

        <div className="relative z-10 bg-slate-900/50 backdrop-blur-2xl border border-white/10 p-12 rounded-3xl shadow-2xl max-w-md w-full mx-4 text-center">
          {/* Logo/Icon Animation */}
          <div className="relative w-24 h-24 mx-auto mb-8">
            <div className="absolute inset-0 bg-blue-500/20 rounded-full animate-ping"></div>
            <div className="absolute inset-0 bg-gradient-to-tr from-blue-600 to-purple-600 rounded-full flex items-center justify-center shadow-lg shadow-blue-500/30 animate-pulse">
              <Database className="w-10 h-10 text-white" />
            </div>
            {/* Orbiting Dots */}
            <div className="absolute inset-0 animate-spin duration-[3s]">
              <div className="absolute top-0 left-1/2 -translate-x-1/2 -translate-y-2 w-3 h-3 bg-blue-400 rounded-full shadow-[0_0_10px_rgba(96,165,250,0.8)]"></div>
            </div>
            <div className="absolute inset-0 animate-spin duration-[5s] reverse">
              <div className="absolute bottom-0 left-1/2 -translate-x-1/2 translate-y-2 w-2 h-2 bg-purple-400 rounded-full shadow-[0_0_10px_rgba(192,132,252,0.8)]"></div>
            </div>
          </div>

          <h2 className="text-3xl font-bold text-white mb-3 tracking-tight">
            Lasantha <span className="text-blue-400">ERP</span>
          </h2>
          
          <div className="space-y-1 mb-8">
            <p className="text-slate-300 font-medium">Establishing Secure Connection</p>
            <p className="text-slate-500 text-sm">Syncing with Peachtree Accounting...</p>
          </div>

          {/* Progress Bar */}
          <div className="relative h-1.5 bg-slate-800 rounded-full overflow-hidden mb-4">
            <div className="absolute inset-y-0 left-0 bg-gradient-to-r from-blue-500 via-purple-500 to-blue-500 w-[50%] animate-[shimmer_2s_infinite_linear] rounded-full"></div>
          </div>

          <div className="flex justify-center gap-2">
            <span className="w-1.5 h-1.5 bg-blue-500 rounded-full animate-bounce delay-0"></span>
            <span className="w-1.5 h-1.5 bg-blue-500 rounded-full animate-bounce delay-150"></span>
            <span className="w-1.5 h-1.5 bg-blue-500 rounded-full animate-bounce delay-300"></span>
          </div>
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
    <div className={`bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-0 md:p-6 ${isModal ? 'min-h-full' : 'min-h-screen'}`}>
      <div className="hidden md:block max-w-[1800px] mx-auto">
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
                    bridgeMonitor.status === 'running' 
                      ? 'bg-green-500/20 text-green-400 border border-green-500/30' 
                      : bridgeMonitor.status === 'stopped'
                      ? 'bg-red-500/20 text-red-400 border border-red-500/30'
                      : 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30'
                  }`}>
                    <div className={`w-2 h-2 rounded-full ${
                      bridgeMonitor.status === 'running' 
                        ? 'bg-green-400 animate-pulse' 
                        : bridgeMonitor.status === 'stopped'
                        ? 'bg-red-400'
                        : 'bg-yellow-400 animate-pulse'
                    }`}></div>
                    <span className="text-sm font-medium">
                      {bridgeMonitor.status === 'running' 
                        ? 'Peachtree Connected' 
                        : bridgeMonitor.status === 'stopped'
                        ? 'Peachtree Disconnected'
                        : bridgeMonitor.status === 'restarting'
                        ? 'Restarting...'
                        : 'Checking Connection...'}
                    </span>
                    {bridgeMonitor.status === 'running' && bridgeMonitor.pid && (
                      <span className="text-xs opacity-70">PID: {bridgeMonitor.pid}</span>
                    )}
                    {bridgeMonitor.consecutiveFailures > 0 && bridgeMonitor.status === 'stopped' && (
                      <span className="text-xs opacity-70">Retry {bridgeMonitor.consecutiveFailures}/3</span>
                    )}
                  </div>
                  
                  {/* 🆕 Connect Now Button - Only shows when disconnected */}
                    {bridgeMonitor.status === 'stopped' && (
                    <button
                      onClick={async () => {
                        const previousAuto = autoRefresh;
                        setStartingBridge(true);
                        try {
                          console.log('[UI] Attempting to restart bridge via monitor...');
                          setError(null);
                          // Pause auto-refresh while starting
                          setAutoRefresh(false);

                          const success = await bridgeMonitor.restart();
                          
                          if (success) {
                            showToast('Bridge restarted successfully!', 'success');
                            // ✅ Bridge running -- load fresh data (no-cache)
                            console.log('[UI] Bridge started, loading fresh data...');
                            await loadAllData();
                          } else {
                            showToast('Failed to restart bridge', 'error');
                            setError('Failed to restart Python bridge - check console for details');
                          }
                        } catch (error: any) {
                          console.error('[UI] Bridge restart failed:', error);
                          showToast(error.message || 'Bridge restart error', 'error');
                          setError(error.message || 'Failed to start Python bridge - check console for details');
                        } finally {
                          // restore auto-refresh setting
                          setAutoRefresh(previousAuto);
                          setStartingBridge(false);
                        }
                      }}
                      className="flex items-center gap-2 px-4 py-1.5 bg-green-600 hover:bg-green-700 text-white text-sm font-medium rounded-lg transition-all shadow-lg shadow-green-500/30 disabled:opacity-50 disabled:cursor-not-allowed"
                      disabled={startingBridge || bridgeMonitor.status === 'restarting'}
                    >
                      <Activity className={`w-4 h-4 ${startingBridge || bridgeMonitor.status === 'restarting' ? 'animate-spin' : ''}`} />
                      {startingBridge || bridgeMonitor.status === 'restarting' ? 'Starting...' : 'Connect Now'}
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

        {/* Tab Navigation - Mobile Optimized */}
        <div className="flex gap-2 mb-6 overflow-x-auto pb-2 scrollbar-hide">
          {[
            { id: 'dashboard', label: 'Dashboard', shortLabel: 'Home', icon: BarChart3 },
            { id: 'customers', label: 'Customers', shortLabel: 'Customers', icon: Users },
            { id: 'vendors', label: 'Vendors', shortLabel: 'Vendors', icon: Building2 },
            { id: 'accounts', label: 'Chart of Accounts', shortLabel: 'Accounts', icon: Wallet },
            { id: 'transactions', label: 'Transactions', shortLabel: 'Trans', icon: Receipt },
            { id: 'reports', label: 'Reports', shortLabel: 'Reports', icon: FileText },
            { id: 'analytics', label: 'Analytics', shortLabel: 'Stats', icon: PieChart }
          ].map((tab) => {
            const Icon = tab.icon;
            const isNew = ['reports', 'analytics'].includes(tab.id);
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as any)}
                className={`relative flex items-center gap-1.5 px-3 md:px-6 py-2 md:py-3 rounded-lg md:rounded-xl font-medium transition-all whitespace-nowrap text-sm md:text-base ${
                  activeTab === tab.id
                    ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white shadow-lg shadow-blue-500/50'
                    : 'bg-slate-800/50 text-slate-300 hover:bg-slate-700 border border-slate-700'
                }`}
              >
                <Icon className="w-4 h-4 md:w-5 md:h-5" />
                <span className="hidden sm:inline">{tab.label}</span>
                <span className="sm:hidden">{tab.shortLabel}</span>
                {isNew && (
                  <span className="absolute -top-1 -right-1 bg-green-500 text-white text-[8px] md:text-[10px] font-bold px-1 md:px-1.5 py-0.5 rounded-full">
                    NEW
                  </span>
                )}
              </button>
            );
          })}
        </div>

        {/* Dashboard Tab */}
        {activeTab === 'dashboard' && (
          <div className="space-y-4 md:space-y-6">
            {/* Top Stats Grid - Mobile Optimized */}
            <div className="grid grid-cols-2 md:grid-cols-2 lg:grid-cols-4 gap-3 md:gap-6">
              {/* Total Customers */}
              <div className="bg-gradient-to-br from-blue-500/20 to-blue-600/5 backdrop-blur-xl border border-blue-500/30 rounded-xl md:rounded-2xl p-4 md:p-6 hover:shadow-lg hover:shadow-blue-500/20 transition-all">
                <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-2 md:mb-4">
                  <div className="w-10 h-10 md:w-14 md:h-14 bg-blue-500/20 rounded-lg md:rounded-xl flex items-center justify-center mb-2 md:mb-0">
                    <Users className="w-5 h-5 md:w-7 md:h-7 text-blue-400" />
                  </div>
                  <div className="hidden md:block text-right">
                    <div className="flex items-center gap-1 text-sm text-green-400">
                      <ArrowUpRight className="w-4 h-4" />
                      <span>{stats.activeCustomers > 0 ? ((stats.activeCustomers / stats.totalCustomers) * 100).toFixed(0) : 0}%</span>
                    </div>
                  </div>
                </div>
                <h3 className="text-2xl md:text-4xl font-bold text-white mb-1 md:mb-2">
                  {formatNumber(stats.totalCustomers)}
                </h3>
                <p className="text-blue-300 font-medium text-xs md:text-base">Customers</p>
                <p className="text-blue-400/60 text-[10px] md:text-sm mt-0.5 md:mt-1">{formatNumber(stats.activeCustomers)} active</p>
              </div>

              {/* Total Vendors */}
              <div className="bg-gradient-to-br from-purple-500/20 to-purple-600/5 backdrop-blur-xl border border-purple-500/30 rounded-xl md:rounded-2xl p-4 md:p-6 hover:shadow-lg hover:shadow-purple-500/20 transition-all">
                <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-2 md:mb-4">
                  <div className="w-10 h-10 md:w-14 md:h-14 bg-purple-500/20 rounded-lg md:rounded-xl flex items-center justify-center mb-2 md:mb-0">
                    <Building2 className="w-5 h-5 md:w-7 md:h-7 text-purple-400" />
                  </div>
                  <div className="hidden md:block text-right">
                    <div className="flex items-center gap-1 text-sm text-green-400">
                      <ArrowUpRight className="w-4 h-4" />
                      <span>{stats.activeVendors > 0 ? ((stats.activeVendors / stats.totalVendors) * 100).toFixed(0) : 0}%</span>
                    </div>
                  </div>
                </div>
                <h3 className="text-2xl md:text-4xl font-bold text-white mb-1 md:mb-2">
                  {formatNumber(stats.totalVendors)}
                </h3>
                <p className="text-purple-300 font-medium text-xs md:text-base">Vendors</p>
                <p className="text-purple-400/60 text-[10px] md:text-sm mt-0.5 md:mt-1">{formatNumber(stats.activeVendors)} active</p>
              </div>

              {/* Total Accounts */}
              <div className="bg-gradient-to-br from-green-500/20 to-green-600/5 backdrop-blur-xl border border-green-500/30 rounded-xl md:rounded-2xl p-4 md:p-6 hover:shadow-lg hover:shadow-green-500/20 transition-all">
                <div className="flex flex-col mb-2 md:mb-4">
                  <div className="w-10 h-10 md:w-14 md:h-14 bg-green-500/20 rounded-lg md:rounded-xl flex items-center justify-center mb-2 md:mb-0">
                    <Wallet className="w-5 h-5 md:w-7 md:h-7 text-green-400" />
                  </div>
                </div>
                <h3 className="text-2xl md:text-4xl font-bold text-white mb-1 md:mb-2">
                  {formatNumber(stats.totalAccounts)}
                </h3>
                <p className="text-green-300 font-medium text-xs md:text-base">Accounts</p>
                <p className="text-green-400/60 text-[10px] md:text-sm mt-0.5 md:mt-1">{accountTypeDistribution.length} types</p>
              </div>

              {/* Total Tables */}
              <div className="bg-gradient-to-br from-orange-500/20 to-orange-600/5 backdrop-blur-xl border border-orange-500/30 rounded-xl md:rounded-2xl p-4 md:p-6 hover:shadow-lg hover:shadow-orange-500/20 transition-all">
                <div className="flex flex-col mb-2 md:mb-4">
                  <div className="w-10 h-10 md:w-14 md:h-14 bg-orange-500/20 rounded-lg md:rounded-xl flex items-center justify-center mb-2 md:mb-0">
                    <Package className="w-5 h-5 md:w-7 md:h-7 text-orange-400" />
                  </div>
                </div>
                <h3 className="text-2xl md:text-4xl font-bold text-white mb-1 md:mb-2">
                  {formatNumber(tables.length)}
                </h3>
                <p className="text-orange-300 font-medium text-xs md:text-base">DB Tables</p>
                <p className="text-orange-400/60 text-[10px] md:text-sm mt-0.5 md:mt-1">Accessible data</p>
              </div>
            </div>

            {/* Data Completeness Card - Mobile Optimized */}
            <div className="bg-gradient-to-br from-cyan-500/20 to-cyan-600/5 backdrop-blur-xl border border-cyan-500/30 rounded-xl md:rounded-2xl p-4 md:p-6">
              <div className="flex items-center gap-2 md:gap-3 mb-3 md:mb-4">
                <Database className="w-6 h-6 md:w-8 md:h-8 text-cyan-400" />
                <div className="flex-1">
                  <h3 className="text-base md:text-xl font-bold text-white">Data Quality</h3>
                  <p className="text-cyan-300 text-xs md:text-sm">Complete records vs total</p>
                </div>
              </div>
              <div className="space-y-2 md:space-y-3">
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-xs md:text-sm text-slate-300">Customers</span>
                    <span className="text-xs md:text-sm font-bold text-cyan-400">{stats.dataCompleteness.customers}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-1.5 md:h-2">
                    <div 
                      className="bg-gradient-to-r from-cyan-500 to-blue-500 h-1.5 md:h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.customers}%` }}
                    />
                  </div>
                </div>
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-xs md:text-sm text-slate-300">Vendors</span>
                    <span className="text-xs md:text-sm font-bold text-cyan-400">{stats.dataCompleteness.vendors}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-1.5 md:h-2">
                    <div 
                      className="bg-gradient-to-r from-purple-500 to-pink-500 h-1.5 md:h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.vendors}%` }}
                    />
                  </div>
                </div>
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-xs md:text-sm text-slate-300">Accounts</span>
                    <span className="text-xs md:text-sm font-bold text-cyan-400">{stats.dataCompleteness.accounts}%</span>
                  </div>
                  <div className="w-full bg-slate-700/50 rounded-full h-1.5 md:h-2">
                    <div 
                      className="bg-gradient-to-r from-green-500 to-emerald-500 h-1.5 md:h-2 rounded-full transition-all duration-500"
                      style={{ width: `${stats.dataCompleteness.accounts}%` }}
                    />
                  </div>
                </div>
              </div>
            </div>

            {/* Schema Catalog */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-2 md:gap-4 mb-4">
                <div>
                  <h3 className="text-base md:text-xl font-bold text-white">Peachtree Data Catalog</h3>
                  <p className="text-slate-400 text-xs md:text-sm">
                    {schema.tables.length} tracked tables • v{schema.version}
                  </p>
                </div>
                <span className="text-xs md:text-sm text-slate-400">{schema.source}</span>
              </div>

              <div className="flex gap-2 flex-wrap overflow-x-auto pb-2">
                {schemaModuleOptions.map((module) => (
                  <button
                    key={module}
                    onClick={() => setSelectedSchemaModule(module)}
                    className={`px-3 py-1.5 rounded-full text-xs font-semibold transition-colors border ${
                      selectedSchemaModule === module
                        ? 'bg-blue-500 text-white border-blue-400 shadow-lg shadow-blue-500/30'
                        : 'bg-slate-900/60 text-slate-300 border-slate-700 hover:text-white'
                    }`}
                  >
                    {module === 'All Modules' ? 'All' : module}
                  </button>
                ))}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 mt-4">
                {catalogTables.slice(0, 4).map((table) => (
                  <div key={table.name} className="p-4 rounded-xl border border-slate-700 bg-slate-900/40">
                    <div className="flex items-center justify-between mb-2">
                      <p className="text-white font-semibold text-sm md:text-base">{table.name}</p>
                      <span className="text-[10px] uppercase tracking-wide text-slate-400">{table.module}</span>
                    </div>
                    <p className="text-slate-400 text-sm min-h-[48px]">{table.description}</p>
                    <div className="mt-3 text-[11px] text-slate-400 flex flex-wrap gap-2">
                      <span className="px-2 py-0.5 rounded-full bg-slate-800 border border-slate-700">
                        PK: {table.primaryKey}
                      </span>
                      {table.businessKey && (
                        <span className="px-2 py-0.5 rounded-full bg-slate-800 border border-slate-700">
                          BK: {table.businessKey}
                        </span>
                      )}
                      <span className="px-2 py-0.5 rounded-full bg-slate-800 border border-slate-700">
                        {table.fields.length} fields
                      </span>
                    </div>
                  </div>
                ))}
                {catalogTables.length > 4 && (
                  <div className="p-4 rounded-xl border border-dashed border-slate-700 bg-slate-900/20 flex items-center justify-between text-sm text-slate-400">
                    <span>
                      {catalogTables.length - 4} more in {selectedSchemaModule === 'All Modules' ? 'catalog' : selectedSchemaModule}
                    </span>
                    <span className="text-white font-semibold">{schema.tables.length} total</span>
                  </div>
                )}
              </div>

              <div className="mt-4 text-xs text-slate-400 space-y-1">
                <p className="font-semibold text-slate-300">Schema Notes</p>
                <ul className="list-disc list-inside space-y-1">
                  {schema.notes.slice(0, 2).map((note, idx) => (
                    <li key={`${idx}-${note}`}>{note}</li>
                  ))}
                </ul>
              </div>
            </div>

            {/* Financial Overview - Mobile Optimized */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 md:gap-6">
              {/* Receivables */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
                <div className="flex items-center justify-between mb-4 md:mb-6">
                  <div className="flex items-center gap-2 md:gap-3">
                    <div className="w-10 h-10 md:w-12 md:h-12 bg-blue-500/20 rounded-lg md:rounded-xl flex items-center justify-center">
                      <DollarSign className="w-5 h-5 md:w-6 md:h-6 text-blue-400" />
                    </div>
                    <div>
                      <h3 className="text-base md:text-xl font-bold text-white">Receivable</h3>
                      <p className="text-slate-400 text-xs md:text-sm">Customer balances</p>
                    </div>
                  </div>
                  <button 
                    onClick={() => exportToCSV(customers, 'accounts_receivable')}
                    className="p-1.5 md:p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-400 rounded-lg transition-colors"
                  >
                    <Download className="w-4 h-4 md:w-5 md:h-5" />
                  </button>
                </div>
                <div className="mb-3 md:mb-4">
                  <h4 className="text-2xl md:text-3xl font-bold text-blue-400">{formatCurrency(stats.totalRevenue)}</h4>
                  <p className="text-slate-400 text-xs md:text-sm mt-1">Outstanding from {stats.totalCustomers} customers</p>
                </div>
                <ResponsiveContainer width="100%" height={180}>
                  <BarChart data={customers.slice(0, 10)}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                    <XAxis dataKey="CustomerName" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 8 }} angle={-45} textAnchor="end" height={60} />
                    <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`} />
                    <Tooltip 
                      contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                      formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Balance']}
                    />
                    <Bar dataKey="Balance" fill="#3b82f6" radius={[6, 6, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>

              {/* Payables */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
                <div className="flex items-center justify-between mb-4 md:mb-6">
                  <div className="flex items-center gap-2 md:gap-3">
                    <div className="w-10 h-10 md:w-12 md:h-12 bg-purple-500/20 rounded-lg md:rounded-xl flex items-center justify-center">
                      <CreditCard className="w-5 h-5 md:w-6 md:h-6 text-purple-400" />
                    </div>
                    <div>
                      <h3 className="text-base md:text-xl font-bold text-white">Payable</h3>
                      <p className="text-slate-400 text-xs md:text-sm">Vendor balances</p>
                    </div>
                  </div>
                  <button 
                    onClick={() => exportToCSV(vendors, 'accounts_payable')}
                    className="p-1.5 md:p-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-400 rounded-lg transition-colors"
                  >
                    <Download className="w-4 h-4 md:w-5 md:h-5" />
                  </button>
                </div>
                <div className="mb-3 md:mb-4">
                  <h4 className="text-2xl md:text-3xl font-bold text-purple-400">{formatCurrency(stats.totalExpenses)}</h4>
                  <p className="text-slate-400 text-xs md:text-sm mt-1">Outstanding to {stats.totalVendors} vendors</p>
                </div>
                <ResponsiveContainer width="100%" height={180}>
                  <BarChart data={vendors.slice(0, 10)}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                    <XAxis dataKey="VendorName" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 8 }} angle={-45} textAnchor="end" height={60} />
                    <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`} />
                    <Tooltip 
                      contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                      formatter={(value: any) => [`Rs ${formatCurrency(value)}`, 'Balance']}
                    />
                    <Bar dataKey="Balance" fill="#a855f7" radius={[6, 6, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>

            {/* Charts Grid - Mobile Optimized */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 md:gap-6">
              {/* Customer Balance Distribution */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
                <div className="flex items-center justify-between mb-4 md:mb-6">
                  <h3 className="text-base md:text-xl font-bold text-white flex items-center gap-2">
                    <PieChart className="w-5 h-5 md:w-6 md:h-6 text-blue-400" />
                    <span className="text-sm md:text-xl">Top Customers</span>
                  </h3>
                </div>
                {customerBalanceDistribution.length > 0 ? (
                  <ResponsiveContainer width="100%" height={250}>
                    <RechartsPie>
                      <Pie
                        data={customerBalanceDistribution}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }: any) => `${(percent * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {customerBalanceDistribution.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                        formatter={(value: any) => formatCurrency(value)}
                      />
                    </RechartsPie>
                  </ResponsiveContainer>
                ) : (
                  <div className="h-[250px] flex items-center justify-center text-slate-400 text-sm">
                    No customer data
                  </div>
                )}
              </div>

              {/* Account Type Distribution */}
              <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
                <div className="flex items-center justify-between mb-4 md:mb-6">
                  <h3 className="text-base md:text-xl font-bold text-white flex items-center gap-2">
                    <BarChart3 className="w-5 h-5 md:w-6 md:h-6 text-purple-400" />
                    <span className="text-sm md:text-xl">Account Types</span>
                  </h3>
                </div>
                {accountTypeDistribution.length > 0 ? (
                  <ResponsiveContainer width="100%" height={250}>
                    <BarChart data={accountTypeDistribution}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                      <XAxis dataKey="name" stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} angle={-20} textAnchor="end" height={60} />
                      <YAxis stroke="#94a3b8" tick={{ fill: '#94a3b8', fontSize: 10 }} />
                      <Tooltip 
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                      />
                      <Bar dataKey="value" fill="#a855f7" radius={[6, 6, 0, 0]}>
                        {accountTypeDistribution.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                        ))}
                      </Bar>
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <div className="h-[250px] flex items-center justify-center text-slate-400 text-sm">
                    No account data
                  </div>
                )}
              </div>
            </div>

            {/* Quick Actions - Mobile Optimized */}
            <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl md:rounded-2xl p-4 md:p-6">
              <h3 className="text-base md:text-xl font-bold text-white mb-4 md:mb-6">Quick Actions</h3>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-3 md:gap-4">
                <button
                  onClick={() => setActiveTab('customers')}
                  className="flex items-center gap-3 p-3 md:p-4 bg-blue-500/10 hover:bg-blue-500/20 border border-blue-500/30 rounded-lg md:rounded-xl transition-all group"
                >
                  <Users className="w-6 h-6 md:w-8 md:h-8 text-blue-400 group-hover:scale-110 transition-transform" />
                  <div className="text-left">
                    <p className="text-white font-semibold text-sm md:text-base">View Customers</p>
                    <p className="text-blue-300 text-xs md:text-sm">{formatNumber(stats.totalCustomers)} total</p>
                  </div>
                </button>
                <button
                  onClick={() => setActiveTab('vendors')}
                  className="flex items-center gap-3 p-3 md:p-4 bg-purple-500/10 hover:bg-purple-500/20 border border-purple-500/30 rounded-lg md:rounded-xl transition-all group"
                >
                  <Building2 className="w-6 h-6 md:w-8 md:h-8 text-purple-400 group-hover:scale-110 transition-transform" />
                  <div className="text-left">
                    <p className="text-white font-semibold text-sm md:text-base">View Vendors</p>
                    <p className="text-purple-300 text-xs md:text-sm">{formatNumber(stats.totalVendors)} total</p>
                  </div>
                </button>
                <button
                  onClick={() => setActiveTab('accounts')}
                  className="flex items-center gap-3 p-3 md:p-4 bg-green-500/10 hover:bg-green-500/20 border border-green-500/30 rounded-lg md:rounded-xl transition-all group"
                >
                  <Wallet className="w-6 h-6 md:w-8 md:h-8 text-green-400 group-hover:scale-110 transition-transform" />
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

            {/* Customer Table - Desktop */}
            <div className="hidden md:block bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
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

            {/* Customer List - Mobile Card View */}
            <div className="md:hidden space-y-4">
              {getFilteredCustomers().map((customer, index) => (
                <div key={index} className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl p-4 space-y-3 shadow-lg">
                  <div className="flex justify-between items-start">
                    <div className="flex-1 mr-2">
                      <h3 className="text-white font-bold text-lg leading-tight">{customer.CustomerName}</h3>
                      <div className="flex items-center gap-2 mt-1">
                        <span className="text-slate-400 text-xs font-mono bg-slate-900/50 px-1.5 py-0.5 rounded">{customer.CustomerID}</span>
                        {(customer.CustomerName.startsWith('Customer ') || customer.CustomerName === customer.CustomerID) && (
                          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-amber-500/20 text-amber-400 rounded text-[10px] font-medium">
                            <AlertCircle className="w-3 h-3" />
                            Missing Name
                          </span>
                        )}
                      </div>
                    </div>
                    <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-bold ${
                      customer.Status === 'Active' ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                    }`}>
                      {customer.Status}
                    </span>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-3 text-sm bg-slate-900/30 p-3 rounded-lg">
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Contact</p>
                      <p className="text-slate-300 font-medium truncate">{customer.Contact || '-'}</p>
                    </div>
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Phone</p>
                      <p className="text-slate-300 font-medium truncate">{customer.Phone || '-'}</p>
                    </div>
                  </div>

                  <div className="flex justify-between items-end pt-1">
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Credit Limit</p>
                      <p className="text-slate-300 font-medium">{formatCurrency(customer.CreditLimit)}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Balance</p>
                      <button
                        onClick={() => {
                          setSelectedCustomerForInvoice(customer);
                          loadInvoiceDetails(customer.CustomerID, 'customer');
                        }}
                        className="text-green-400 font-bold text-xl hover:underline flex items-center justify-end gap-1"
                      >
                        {formatCurrency(customer.Balance)}
                        <ArrowUpRight className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
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

            {/* Vendor Table - Desktop */}
            <div className="hidden md:block bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
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

            {/* Vendor List - Mobile Card View */}
            <div className="md:hidden space-y-4">
              {getFilteredVendors().map((vendor, index) => (
                <div key={index} className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl p-4 space-y-3 shadow-lg">
                  <div className="flex justify-between items-start">
                    <div className="flex-1 mr-2">
                      <h3 className="text-white font-bold text-lg leading-tight">{vendor.VendorName}</h3>
                      <div className="flex items-center gap-2 mt-1">
                        <span className="text-slate-400 text-xs font-mono bg-slate-900/50 px-1.5 py-0.5 rounded">{vendor.VendorID}</span>
                        {(vendor.VendorName.startsWith('Vendor ') || vendor.VendorName === vendor.VendorID) && (
                          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-amber-500/20 text-amber-400 rounded text-[10px] font-medium">
                            <AlertCircle className="w-3 h-3" />
                            Missing Name
                          </span>
                        )}
                      </div>
                    </div>
                    <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-bold ${
                      vendor.Status === 'Active' ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
                    }`}>
                      {vendor.Status}
                    </span>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-3 text-sm bg-slate-900/30 p-3 rounded-lg">
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Contact</p>
                      <p className="text-slate-300 font-medium truncate">{vendor.Contact || '-'}</p>
                    </div>
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Phone</p>
                      <p className="text-slate-300 font-medium truncate">{vendor.Phone || '-'}</p>
                    </div>
                  </div>

                  <div className="flex justify-between items-end pt-1">
                    <div>
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Credit Period</p>
                      <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-blue-500/20 text-blue-400 rounded text-xs font-medium">
                        <Calendar className="w-3 h-3" />
                        {vendor.CreditPeriod || '30 Days'}
                      </span>
                    </div>
                    <div className="text-right">
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Balance</p>
                      <button
                        onClick={() => {
                          setSelectedVendorForInvoice(vendor);
                          loadInvoiceDetails(vendor.VendorID, 'vendor');
                        }}
                        className="text-red-400 font-bold text-xl hover:underline flex items-center justify-end gap-1"
                      >
                        {formatCurrency(vendor.Balance)}
                        <ArrowUpRight className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
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

            {/* Accounts Table - Desktop */}
            <div className="hidden md:block bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
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

            {/* Accounts List - Mobile Card View */}
            <div className="md:hidden space-y-4">
              {getFilteredAccounts().map((account, index) => (
                <div key={index} className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl p-4 space-y-3 shadow-lg">
                  <div className="flex justify-between items-start">
                    <div className="flex-1 mr-2">
                      <h3 className="text-white font-bold text-lg leading-tight">{account.AccountName}</h3>
                      <div className="flex items-center gap-2 mt-1">
                        <span className="text-green-400 text-xs font-mono bg-green-500/10 px-1.5 py-0.5 rounded border border-green-500/20">{account.AccountID}</span>
                        {(account.AccountName.startsWith('Account ') || account.AccountName === account.AccountID) && (
                          <span className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-amber-500/20 text-amber-400 rounded text-[10px] font-medium">
                            <AlertCircle className="w-3 h-3" />
                            Missing Name
                          </span>
                        )}
                      </div>
                    </div>
                    <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-bold ${
                      account.Active ? 'bg-green-500/20 text-green-400' : 'bg-gray-500/20 text-gray-400'
                    }`}>
                      {account.Active ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                  
                  <div className="flex justify-between items-center pt-2 border-t border-slate-700/50">
                    <span className="inline-block px-2 py-1 bg-blue-500/20 text-blue-400 rounded-lg text-xs font-medium">
                      {account.AccountType}
                    </span>
                    <div className="text-right">
                      <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Balance</p>
                      <p className="text-white font-bold text-lg">{formatCurrency(account.Balance)}</p>
                    </div>
                  </div>
                </div>
              ))}
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

            {/* Transactions Table - Desktop */}
            <div className="hidden md:block bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl overflow-hidden">
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

            {/* Transactions List - Mobile Card View */}
            <div className="md:hidden space-y-4">
              {transactions
                .filter(txn =>
                  txn.Description.toLowerCase().includes(searchTerm.toLowerCase()) ||
                  txn.TransactionID.toLowerCase().includes(searchTerm.toLowerCase()) ||
                  txn.Type.toLowerCase().includes(searchTerm.toLowerCase())
                )
                .map((txn, index) => (
                  <div key={index} className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-xl p-4 space-y-3 shadow-lg">
                    <div className="flex justify-between items-start">
                      <div className="flex-1 mr-2">
                        <div className="flex items-center gap-2 mb-1">
                          <span className="text-blue-400 text-xs font-mono bg-blue-500/10 px-1.5 py-0.5 rounded border border-blue-500/20">{txn.TransactionID}</span>
                          <span className="text-slate-400 text-xs">{txn.Date || '-'}</span>
                        </div>
                        <p className="text-white font-medium leading-tight line-clamp-2">{txn.Description}</p>
                      </div>
                      <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-bold ${
                        txn.Status === 'Posted' ? 'bg-green-500/20 text-green-400' : 'bg-yellow-500/20 text-yellow-400'
                      }`}>
                        {txn.Status}
                      </span>
                    </div>
                    
                    <div className="flex justify-between items-center pt-2 border-t border-slate-700/50">
                      <span className="inline-block px-2 py-1 bg-purple-500/20 text-purple-400 rounded-lg text-xs font-medium">
                        {txn.Type}
                      </span>
                      <div className="text-right">
                        <p className="text-slate-500 text-xs uppercase tracking-wider mb-0.5">Amount</p>
                        <p className="text-green-400 font-bold text-lg">{formatCurrency(txn.Amount)}</p>
                      </div>
                    </div>
                  </div>
                ))}
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

      {/* 📱 Mobile View - Advanced App Structure */}
      <div className="md:hidden fixed inset-0 z-[100] bg-slate-900 overflow-y-auto pb-20">
        {/* Mobile Header */}
        <div className="bg-slate-900/95 backdrop-blur-xl border-b border-slate-800 p-4 sticky top-0 z-30 safe-top">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-purple-600 rounded-xl flex items-center justify-center shadow-lg shadow-blue-500/20">
                <Building2 className="w-5 h-5 text-white" />
              </div>
              <div>
                <h1 className="text-lg font-bold text-white leading-none">Peachtree</h1>
                <div className="flex items-center gap-1.5 mt-1">
                  <div className={`w-2 h-2 rounded-full ${bridgeMonitor.status === 'running' ? 'bg-green-400 animate-pulse' : 'bg-red-400'}`} />
                  <span className="text-xs text-slate-400">{bridgeMonitor.status === 'running' ? 'Connected' : 'Offline'}</span>
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <button className="w-10 h-10 bg-slate-800 rounded-full flex items-center justify-center text-slate-400">
                <Search className="w-5 h-5" />
              </button>
              <button 
                onClick={() => {
                  if (isModal && onClose) {
                    onClose();
                  } else {
                    router.push('/dashboard');
                  }
                }}
                className="w-10 h-10 bg-red-500/10 border border-red-500/20 rounded-full flex items-center justify-center text-red-400 active:scale-95 transition-transform"
              >
                <X className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Content Area */}
        <div className="p-4 space-y-6">
          
          {/* 1. Hero Stats Carousel (Horizontal Scroll) */}
          <div className="flex gap-4 overflow-x-auto pb-4 -mx-4 px-4 scrollbar-hide snap-x">
            {/* Revenue Card */}
            <div className="min-w-[85vw] bg-gradient-to-br from-blue-600 to-blue-800 rounded-2xl p-5 shadow-xl shadow-blue-900/20 snap-center relative overflow-hidden">
              <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full -mr-10 -mt-10 blur-2xl"></div>
              <div className="relative z-10">
                <div className="flex items-center gap-2 mb-3">
                  <div className="p-2 bg-white/20 rounded-lg backdrop-blur-sm">
                    <DollarSign className="w-5 h-5 text-white" />
                  </div>
                  <span className="text-blue-100 font-medium">Total Receivables</span>
                </div>
                <h3 className="text-3xl font-bold text-white mb-1">{formatCurrency(stats.totalRevenue)}</h3>
                <p className="text-blue-200 text-sm">From {stats.activeCustomers} active customers</p>
              </div>
            </div>

            {/* Payables Card */}
            <div className="min-w-[85vw] bg-gradient-to-br from-purple-600 to-purple-800 rounded-2xl p-5 shadow-xl shadow-purple-900/20 snap-center relative overflow-hidden">
              <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full -mr-10 -mt-10 blur-2xl"></div>
              <div className="relative z-10">
                <div className="flex items-center gap-2 mb-3">
                  <div className="p-2 bg-white/20 rounded-lg backdrop-blur-sm">
                    <CreditCard className="w-5 h-5 text-white" />
                  </div>
                  <span className="text-purple-100 font-medium">Total Payables</span>
                </div>
                <h3 className="text-3xl font-bold text-white mb-1">{formatCurrency(stats.totalExpenses)}</h3>
                <p className="text-purple-200 text-sm">To {stats.activeVendors} active vendors</p>
              </div>
            </div>
          </div>

          {/* 2. App Grid (The "OS" Feel) */}
          <div>
            <h3 className="text-slate-400 text-sm font-semibold uppercase tracking-wider mb-3">Applications</h3>
            <div className="grid grid-cols-3 gap-3">
              {[
                { id: 'customers', label: 'Customers', icon: Users, color: 'bg-blue-500', count: stats.totalCustomers },
                { id: 'vendors', label: 'Vendors', icon: Building2, color: 'bg-purple-500', count: stats.totalVendors },
                { id: 'accounts', label: 'Accounts', icon: Wallet, color: 'bg-green-500', count: stats.totalAccounts },
                { id: 'transactions', label: 'History', icon: Receipt, color: 'bg-orange-500', count: stats.totalTransactions },
                { id: 'reports', label: 'Reports', icon: FileText, color: 'bg-pink-500', count: 4 },
                { id: 'analytics', label: 'Analytics', icon: PieChart, color: 'bg-cyan-500', count: 2 },
              ].map((app) => (
                <button
                  key={app.id}
                  onClick={async () => {
                    console.log(`📱 [MOBILE] Opening ${app.id} app...`);
                    
                    // Set mobile view first to show the overlay
                    setMobileView(app.id as any);
                    
                    // Load data if needed
                    if (app.id === 'customers' && customers.length === 0) {
                      console.log('📱 [MOBILE] Loading customers...');
                      setMobileLoading(true);
                      try {
                        await loadCustomers();
                      } finally {
                        setMobileLoading(false);
                      }
                      console.log('📱 [MOBILE] Customers loaded:', customers.length);
                    } else if (app.id === 'vendors' && vendors.length === 0) {
                      console.log('📱 [MOBILE] Loading vendors...');
                      setMobileLoading(true);
                      try {
                        await loadVendors();
                      } finally {
                        setMobileLoading(false);
                      }
                      console.log('📱 [MOBILE] Vendors loaded:', vendors.length);
                    } else if (app.id === 'accounts' && accounts.length === 0) {
                      console.log('📱 [MOBILE] Loading accounts...');
                      setMobileLoading(true);
                      try {
                        await loadAccounts();
                      } finally {
                        setMobileLoading(false);
                      }
                    } else if (app.id === 'transactions' && transactions.length === 0) {
                      console.log('📱 [MOBILE] Loading transactions...');
                      setMobileLoading(true);
                      try {
                        await loadTransactions();
                      } finally {
                        setMobileLoading(false);
                      }
                    }
                  }}
                  className="flex flex-col items-center gap-2 p-3 bg-slate-800/50 border border-slate-700/50 rounded-2xl active:scale-95 transition-transform"
                >
                  <div className={`w-12 h-12 ${app.color} rounded-2xl flex items-center justify-center shadow-lg`}>
                    <app.icon className="w-6 h-6 text-white" />
                  </div>
                  <span className="text-slate-300 text-xs font-medium">{app.label}</span>
                </button>
              ))}
            </div>
          </div>

              {/* 3. Recent Activity List */}
          <div>
            <h3 className="text-slate-400 text-sm font-semibold uppercase tracking-wider mb-3">Recent Activity</h3>
            <div className="bg-slate-800/50 border border-slate-700/50 rounded-2xl overflow-hidden">
              {transactions.length > 0 ? (
                transactions.slice(0, 5).map((tx, i) => (
                  <div key={i} className="p-4 border-b border-slate-700/50 last:border-0 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 bg-slate-700 rounded-full flex items-center justify-center">
                        <Receipt className="w-5 h-5 text-slate-400" />
                      </div>
                      <div>
                        <p className="text-white font-medium text-sm truncate max-w-[150px]">{tx.Description}</p>
                        <p className="text-slate-500 text-xs">{tx.Date}</p>
                      </div>
                    </div>
                    <span className="text-white font-bold text-sm">{formatCurrency(tx.Amount)}</span>
                  </div>
                ))
              ) : (
                <div className="p-6 text-center text-slate-500 text-sm">
                  No recent transactions found
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Mobile Bottom Nav */}
        <div className="fixed bottom-0 left-0 right-0 bg-slate-900/90 backdrop-blur-xl border-t border-slate-800 p-2 z-40">
          <div className="flex justify-around items-center">
            <button 
              onClick={() => setMobileView('home')} 
              className={`p-2 rounded-xl flex flex-col items-center gap-1 ${mobileView === 'home' ? 'text-blue-400' : 'text-slate-500'}`}
            >
              <Home className="w-6 h-6" />
              <span className="text-[10px] font-medium">Home</span>
            </button>
            
            <button 
              onClick={() => setMobileView('search')}
              className={`p-2 rounded-xl flex flex-col items-center gap-1 ${mobileView === 'search' ? 'text-blue-400' : 'text-slate-500'}`}
            >
              <Search className="w-6 h-6" />
              <span className="text-[10px] font-medium">Search</span>
            </button>
            
            <button 
              onClick={() => setMobileView('quick-actions')}
              className="w-12 h-12 bg-blue-600 rounded-full flex items-center justify-center -mt-6 shadow-lg shadow-blue-600/40 border-4 border-slate-900 active:scale-95 transition-transform"
            >
              <Plus className="w-6 h-6 text-white" />
            </button>
            
            <button 
              onClick={() => setMobileView('alerts')}
              className={`p-2 rounded-xl flex flex-col items-center gap-1 ${mobileView === 'alerts' ? 'text-blue-400' : 'text-slate-500'}`}
            >
              <Bell className="w-6 h-6" />
              <span className="text-[10px] font-medium">Alerts</span>
            </button>
            
            <button 
              onClick={() => setMobileView('menu')}
              className={`p-2 rounded-xl flex flex-col items-center gap-1 ${mobileView === 'menu' ? 'text-blue-400' : 'text-slate-500'}`}
            >
              <Menu className="w-6 h-6" />
              <span className="text-[10px] font-medium">Menu</span>
            </button>
          </div>
        </div>

        {/* 📱 Full Screen Overlays (The "Popup" Pages) */}
        
        {/* Customers Overlay */}
        {mobileView === 'customers' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            {/* Header */}
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3">
              <button onClick={() => setMobileView('home')} className="p-2 hover:bg-slate-800 rounded-full">
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <h2 className="text-lg font-bold text-white flex-1">Customers</h2>
              <button 
                onClick={async () => {
                  try {
                    await exportCustomersToExcel(customers);
                    showToast('Customers exported to Excel', 'success');
                  } catch (error) {
                    showToast('Export failed', 'error');
                  }
                }}
                className="p-2 hover:bg-slate-800 rounded-full"
                title="Export to Excel"
              >
                <Download className="w-5 h-5 text-blue-400" />
              </button>
            </div>
            
            {/* Search & Filter */}
            <div className="p-4 bg-slate-900/50 border-b border-slate-800 space-y-3">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  type="text" 
                  placeholder="Search customers..." 
                  className="w-full bg-slate-800 text-white pl-10 pr-4 py-3 rounded-xl border border-slate-700 focus:border-blue-500 focus:outline-none"
                  value={mobileSearchTerm}
                  onChange={(e) => setMobileSearchTerm(e.target.value)}
                />
              </div>
              <div className="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
                {[
                  { id: 'all', label: 'All' },
                  { id: 'active', label: 'Active' },
                  { id: 'balance', label: 'With Balance' }
                ].map(filter => (
                  <button
                    key={filter.id}
                    onClick={() => setMobileCustomerFilter(filter.id as typeof mobileCustomerFilter)}
                    className={`px-4 py-1.5 text-sm font-medium rounded-full whitespace-nowrap border transition-colors ${
                      mobileCustomerFilter === filter.id
                        ? 'bg-blue-600 text-white border-blue-500'
                        : 'bg-slate-800 text-slate-300 border-slate-700'
                    }`}
                  >
                    {filter.label}
                  </button>
                ))}
              </div>
            </div>

            {/* List */}
            <div className="flex-1 overflow-y-auto p-4 space-y-3">
              {(loading || mobileLoading) ? (
                <MobileCardLoadingSkeleton count={5} />
              ) : customers.length === 0 ? (
                <NoCustomersState 
                  onReload={async () => {
                    setMobileLoading(true);
                    try {
                      await loadCustomers();
                    } finally {
                      setMobileLoading(false);
                    }
                  }}
                  loading={mobileLoading}
                />
              ) : (
                customers
                  .filter(c => c.CustomerName.toLowerCase().includes(mobileSearchTerm.toLowerCase()))
                  .filter(c => {
                    if (mobileCustomerFilter === 'active') return c.Status === 'Active';
                    if (mobileCustomerFilter === 'balance') return c.Balance !== 0;
                    return true;
                  })
                  .map((customer, i) => (
                  <div 
                    key={i} 
                    onClick={() => {
                      setSelectedCustomerForInvoice(customer);
                      loadInvoiceDetails(customer.CustomerID, 'customer');
                    }}
                    className="bg-slate-800/50 border border-slate-700 rounded-xl p-4 active:bg-slate-800 transition-colors relative overflow-hidden"
                  >
                    {/* Status Indicator Strip */}
                    <div className={`absolute left-0 top-0 bottom-0 w-1 ${customer.Status === 'Active' ? 'bg-blue-500' : 'bg-slate-600'}`}></div>
                    
                    <div className="pl-2">
                      <div className="flex justify-between items-start mb-2">
                        <div>
                          <h3 className="text-white font-bold text-lg leading-tight">{customer.CustomerName}</h3>
                          <div className="flex items-center gap-2 mt-1">
                            <span className="text-slate-400 text-xs font-mono bg-slate-900/50 px-1.5 py-0.5 rounded border border-slate-700">{customer.CustomerID}</span>
                            {customer.Status !== 'Active' && (
                              <span className="text-xs text-red-400 bg-red-500/10 px-1.5 py-0.5 rounded">Inactive</span>
                            )}
                          </div>
                        </div>
                        <div className="text-right">
                          <p className="text-xs text-slate-500 uppercase mb-0.5">Balance</p>
                          <p className={`text-xl font-bold ${customer.Balance > 0 ? 'text-blue-400' : 'text-slate-300'}`}>
                            {formatCurrency(customer.Balance)}
                          </p>
                        </div>
                      </div>
                      
                      <div className="grid grid-cols-2 gap-4 mt-3 pt-3 border-t border-slate-700/50">
                        <div>
                          <p className="text-slate-500 text-xs uppercase mb-0.5">Contact</p>
                          <p className="text-slate-300 text-sm truncate">{customer.Contact || '-'}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-slate-500 text-xs uppercase mb-0.5">Phone</p>
                          <p className="text-slate-300 text-sm font-mono">{customer.Phone || '-'}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        )}

        {/* Vendors Overlay */}
        {mobileView === 'vendors' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3">
              <button onClick={() => setMobileView('home')} className="p-2 hover:bg-slate-800 rounded-full">
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <h2 className="text-lg font-bold text-white flex-1">Vendors</h2>
              <button 
                onClick={async () => {
                  try {
                    await exportVendorsToExcel(vendors);
                    showToast('Vendors exported to Excel', 'success');
                  } catch (error) {
                    showToast('Export failed', 'error');
                  }
                }}
                className="p-2 hover:bg-slate-800 rounded-full"
                title="Export to Excel"
              >
                <Download className="w-5 h-5 text-purple-400" />
              </button>
            </div>
            <div className="p-4 bg-slate-900/50 border-b border-slate-800 space-y-3">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  type="text" 
                  placeholder="Search vendors..." 
                  className="w-full bg-slate-800 text-white pl-10 pr-4 py-3 rounded-xl border border-slate-700 focus:border-purple-500 focus:outline-none"
                  value={mobileSearchTerm}
                  onChange={(e) => setMobileSearchTerm(e.target.value)}
                />
              </div>
              <div className="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
                {[
                  { id: 'all', label: 'All' },
                  { id: 'active', label: 'Active' },
                  { id: 'balance', label: 'With Balance' }
                ].map(filter => (
                  <button
                    key={filter.id}
                    onClick={() => setMobileVendorFilter(filter.id as typeof mobileVendorFilter)}
                    className={`px-4 py-1.5 text-sm font-medium rounded-full whitespace-nowrap border transition-colors ${
                      mobileVendorFilter === filter.id
                        ? 'bg-purple-600 text-white border-purple-500'
                        : 'bg-slate-800 text-slate-300 border-slate-700'
                    }`}
                  >
                    {filter.label}
                  </button>
                ))}
              </div>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-3">
              {(loading || mobileLoading) ? (
                <MobileCardLoadingSkeleton count={5} />
              ) : vendors.length === 0 ? (
                <NoVendorsState 
                  onReload={async () => {
                    setMobileLoading(true);
                    try {
                      await loadVendors();
                    } finally {
                      setMobileLoading(false);
                    }
                  }}
                  loading={mobileLoading}
                />
              ) : (
                vendors
                  .filter(v => v.VendorName.toLowerCase().includes(mobileSearchTerm.toLowerCase()))
                  .filter(v => {
                    if (mobileVendorFilter === 'active') return v.Status === 'Active';
                    if (mobileVendorFilter === 'balance') return v.Balance !== 0;
                    return true;
                  })
                  .map((vendor, i) => (
                  <div 
                    key={i} 
                    onClick={() => {
                      setSelectedVendorForInvoice(vendor);
                      loadInvoiceDetails(vendor.VendorID, 'vendor');
                    }}
                    className="bg-slate-800/50 border border-slate-700 rounded-xl p-4 active:bg-slate-800 transition-colors relative overflow-hidden"
                  >
                    {/* Status Indicator Strip */}
                    <div className={`absolute left-0 top-0 bottom-0 w-1 ${vendor.Status === 'Active' ? 'bg-purple-500' : 'bg-slate-600'}`}></div>
                    
                    <div className="pl-2">
                      <div className="flex justify-between items-start mb-2">
                        <div>
                          <h3 className="text-white font-bold text-lg leading-tight">{vendor.VendorName}</h3>
                          <div className="flex items-center gap-2 mt-1">
                            <span className="text-slate-400 text-xs font-mono bg-slate-900/50 px-1.5 py-0.5 rounded border border-slate-700">{vendor.VendorID}</span>
                            {vendor.Status !== 'Active' && (
                              <span className="text-xs text-red-400 bg-red-500/10 px-1.5 py-0.5 rounded">Inactive</span>
                            )}
                          </div>
                        </div>
                        <div className="text-right">
                          <p className="text-xs text-slate-500 uppercase mb-0.5">Balance</p>
                          <p className={`text-xl font-bold ${vendor.Balance > 0 ? 'text-purple-400' : 'text-slate-300'}`}>
                            {formatCurrency(vendor.Balance)}
                          </p>
                        </div>
                      </div>
                      
                      <div className="grid grid-cols-2 gap-4 mt-3 pt-3 border-t border-slate-700/50">
                        <div>
                          <p className="text-slate-500 text-xs uppercase mb-0.5">Contact</p>
                          <p className="text-slate-300 text-sm truncate">{vendor.Contact || '-'}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-slate-500 text-xs uppercase mb-0.5">Phone</p>
                          <p className="text-slate-300 text-sm font-mono">{vendor.Phone || '-'}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        )}

        {/* Accounts Overlay - Advanced Mobile */}
        {mobileView === 'accounts' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            
            {/* Header */}
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3 shadow-lg z-10">
              <button onClick={() => setMobileView('home')} className="p-2 hover:bg-slate-800 rounded-full transition-colors">
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <div className="flex-1">
                <h2 className="text-lg font-bold text-white">Chart of Accounts</h2>
                <p className="text-xs text-slate-400">{accounts.length} accounts synced</p>
              </div>
              <button 
                onClick={async () => {
                  try {
                    await exportAccountsToExcel(accounts);
                    showToast('Accounts exported to Excel', 'success');
                  } catch (error) {
                    showToast('Export failed', 'error');
                  }
                }}
                className="p-2 hover:bg-slate-800 rounded-full"
                title="Export to Excel"
              >
                <Download className="w-5 h-5 text-green-400" />
              </button>
            </div>

            {/* Search & Filter */}
            <div className="p-4 bg-slate-900/50 border-b border-slate-800 space-y-3 backdrop-blur-sm sticky top-0 z-20">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  type="text" 
                  placeholder="Search accounts..." 
                  className="w-full bg-slate-800 text-white pl-10 pr-4 py-3 rounded-xl border border-slate-700 focus:border-green-500 focus:outline-none shadow-inner"
                  value={mobileSearchTerm}
                  onChange={(e) => setMobileSearchTerm(e.target.value)}
                />
              </div>
              <div className="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
                {[
                  { id: 'all', label: 'All' },
                  { id: 'Asset', label: 'Assets' },
                  { id: 'Liability', label: 'Liabilities' },
                  { id: 'Equity', label: 'Equity' },
                  { id: 'Revenue', label: 'Income' },
                  { id: 'Expense', label: 'Expenses' }
                ].map(filter => (
                  <button
                    key={filter.id}
                    onClick={() => setMobileAccountFilter(filter.id as any)}
                    className={`px-4 py-1.5 text-sm font-medium rounded-full whitespace-nowrap border transition-all active:scale-95 ${
                      mobileAccountFilter === filter.id
                        ? 'bg-green-600 text-white border-green-500 shadow-lg shadow-green-900/20'
                        : 'bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700'
                    }`}
                  >
                    {filter.label}
                  </button>
                ))}
              </div>
            </div>

            {/* List Content */}
            <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-slate-950">
              {(loading || mobileLoading) ? (
                <MobileCardLoadingSkeleton count={6} />
              ) : accounts.length === 0 ? (
                <NoAccountsState 
                  onReload={loadAccounts}
                  loading={mobileLoading}
                />
              ) : (
                (() => {
                  const filteredAccounts = accounts
                    .filter(a => 
                      a.AccountName.toLowerCase().includes(mobileSearchTerm.toLowerCase()) ||
                      a.AccountID.toLowerCase().includes(mobileSearchTerm.toLowerCase())
                    )
                    .filter(a => mobileAccountFilter === 'all' || a.AccountType === mobileAccountFilter);
                  
                  const maxBalance = Math.max(...filteredAccounts.map(a => Math.abs(a.Balance)), 1);

                  return filteredAccounts.map((account, i) => (
                    <div 
                      key={i} 
                      className="group bg-slate-800/50 border border-slate-700 rounded-2xl p-4 relative overflow-hidden active:bg-slate-800 transition-all"
                    >
                      {/* Type Indicator Bar */}
                      <div className={`absolute left-0 top-0 bottom-0 w-1.5 ${
                        account.AccountType === 'Asset' ? 'bg-blue-500' :
                        account.AccountType === 'Liability' ? 'bg-red-500' :
                        account.AccountType === 'Equity' ? 'bg-purple-500' :
                        account.AccountType === 'Revenue' ? 'bg-green-500' :
                        'bg-orange-500'
                      }`}></div>

                      <div className="pl-3">
                        <div className="flex justify-between items-start mb-2">
                          <div>
                            <h3 className="text-white font-bold text-lg leading-tight">{account.AccountName}</h3>
                            <div className="flex items-center gap-2 mt-1">
                              <span className="text-xs font-mono text-slate-400 bg-slate-900/50 px-1.5 py-0.5 rounded border border-slate-700">
                                {account.AccountID}
                              </span>
                              {!account.Active && (
                                <span className="text-[10px] text-red-400 bg-red-500/10 px-1.5 py-0.5 rounded border border-red-500/20">
                                  Inactive
                                </span>
                              )}
                            </div>
                          </div>
                          <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
                            account.AccountType === 'Asset' ? 'bg-blue-500/10 text-blue-400' :
                            account.AccountType === 'Liability' ? 'bg-red-500/10 text-red-400' :
                            account.AccountType === 'Equity' ? 'bg-purple-500/10 text-purple-400' :
                            account.AccountType === 'Revenue' ? 'bg-green-500/10 text-green-400' :
                            'bg-orange-500/10 text-orange-400'
                          }`}>
                            {account.AccountType === 'Asset' && <Wallet className="w-4 h-4" />}
                            {account.AccountType === 'Liability' && <CreditCard className="w-4 h-4" />}
                            {account.AccountType === 'Equity' && <PieChart className="w-4 h-4" />}
                            {account.AccountType === 'Revenue' && <TrendingUp className="w-4 h-4" />}
                            {account.AccountType === 'Expense' && <Receipt className="w-4 h-4" />}
                          </div>
                        </div>

                        <div className="flex justify-between items-end pt-3 border-t border-slate-700/50 mt-3">
                          <span className={`inline-block px-2.5 py-1 rounded-lg text-xs font-bold uppercase tracking-wide ${
                            account.AccountType === 'Asset' ? 'bg-blue-500/10 text-blue-400' :
                            account.AccountType === 'Liability' ? 'bg-red-500/10 text-red-400' :
                            account.AccountType === 'Equity' ? 'bg-purple-500/10 text-purple-400' :
                            account.AccountType === 'Revenue' ? 'bg-green-500/10 text-green-400' :
                            'bg-orange-500/10 text-orange-400'
                          }`}>
                            {account.AccountType}
                          </span>
                          <div className="text-right">
                            <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider mb-0.5">Current Balance</p>
                            <p className={`text-xl font-bold ${account.Balance < 0 ? 'text-red-400' : 'text-white'}`}>
                              {formatCurrency(account.Balance)}
                            </p>
                          </div>
                        </div>
                      </div>
                    </div>
                  ));
                })()
              )}
            </div>
          </div>
        )}

        {/* Transactions Overlay - Advanced Mobile */}
        {mobileView === 'transactions' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            
            {/* Header */}
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3 shadow-lg z-10">
              <button onClick={() => setMobileView('home')} className="p-2 hover:bg-slate-800 rounded-full transition-colors">
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <div>
                <h2 className="text-lg font-bold text-white">Transaction History</h2>
                <p className="text-xs text-slate-400">{transactions.length} records found</p>
              </div>
            </div>

            {/* Search & Filter */}
            <div className="p-4 bg-slate-900/50 border-b border-slate-800 space-y-3 backdrop-blur-sm sticky top-0 z-20">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  type="text" 
                  placeholder="Search transactions..." 
                  className="w-full bg-slate-800 text-white pl-10 pr-4 py-3 rounded-xl border border-slate-700 focus:border-orange-500 focus:outline-none shadow-inner"
                  value={mobileSearchTerm}
                  onChange={(e) => setMobileSearchTerm(e.target.value)}
                />
              </div>
              <div className="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
                {[
                  { id: 'all', label: 'All' },
                  { id: 'Posted', label: 'Posted' },
                  { id: 'Draft', label: 'Draft' }
                ].map(filter => (
                  <button
                    key={filter.id}
                    onClick={() => setMobileTransactionFilter(filter.id as any)}
                    className={`px-4 py-1.5 text-sm font-medium rounded-full whitespace-nowrap border transition-all active:scale-95 ${
                      mobileTransactionFilter === filter.id
                        ? 'bg-orange-600 text-white border-orange-500 shadow-lg shadow-orange-900/20'
                        : 'bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700'
                    }`}
                  >
                    {filter.label}
                  </button>
                ))}
              </div>
            </div>

            {/* List Content */}
            <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-slate-950">
              {(loading || mobileLoading) ? (
                <ListLoadingSkeleton rows={8} />
              ) : transactions.length === 0 ? (
                <NoTransactionsState 
                  onReload={loadTransactions}
                  loading={mobileLoading}
                />
              ) : (
                transactions
                  .filter(t => 
                    t.Description.toLowerCase().includes(mobileSearchTerm.toLowerCase()) ||
                    t.TransactionID.toLowerCase().includes(mobileSearchTerm.toLowerCase())
                  )
                  .filter(t => mobileTransactionFilter === 'all' || t.Status === mobileTransactionFilter)
                  .map((txn, i) => (
                  <div 
                    key={i} 
                    className="group bg-slate-800/50 border border-slate-700 rounded-2xl p-4 relative overflow-hidden active:bg-slate-800 transition-all"
                  >
                    {/* Status Strip */}
                    <div className={`absolute left-0 top-0 bottom-0 w-1.5 ${txn.Amount >= 0 ? 'bg-green-500' : 'bg-red-500'}`}></div>
                    
                    <div className="pl-3">
                      <div className="flex justify-between items-start mb-2">
                        <div className="flex-1 mr-2">
                          <div className="flex items-center gap-2 mb-1">
                            <span className="text-[10px] font-mono text-blue-400 bg-blue-500/10 px-1.5 py-0.5 rounded border border-blue-500/20">
                              {txn.TransactionID}
                            </span>
                            <span className="text-[10px] text-slate-500 flex items-center gap-1">
                              <Calendar className="w-3 h-3" />
                              {txn.Date}
                            </span>
                          </div>
                          <p className="text-white font-bold text-sm leading-tight line-clamp-2">{txn.Description}</p>
                        </div>
                        <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
                          txn.Amount >= 0 ? 'bg-green-500/10 text-green-400' : 'bg-red-500/10 text-red-400'
                        }`}>
                          {txn.Amount >= 0 ? <ArrowDownLeft className="w-4 h-4" /> : <ArrowUpRight className="w-4 h-4" />}
                        </div>
                      </div>

                      <div className="flex justify-between items-center pt-3 border-t border-slate-700/50 mt-3">
                        <div className="flex items-center gap-2">
                          <span className="inline-block px-2 py-1 bg-slate-700/50 text-slate-300 rounded-lg text-[10px] font-bold uppercase tracking-wide border border-slate-600/50">
                            {txn.Type}
                          </span>
                          <span className={`text-[10px] font-bold uppercase ${
                            txn.Status === 'Posted' ? 'text-green-500' : 'text-yellow-500'
                          }`}>
                            {txn.Status}
                          </span>
                        </div>
                        <p className={`font-bold text-lg tracking-tight ${txn.Amount >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                          {txn.Amount >= 0 ? '+' : ''}{formatCurrency(txn.Amount)}
                        </p>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        )}

        {/* Reports Overlay - Advanced Mobile Generation */}
        {mobileView === 'reports' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            
            {/* Header */}
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3 shadow-lg z-10">
              <button 
                onClick={() => {
                  if (mobileReportResult) {
                    setMobileReportResult(null);
                  } else if (mobileReportType) {
                    setMobileReportType(null);
                  } else {
                    setMobileView('home');
                  }
                }} 
                className="p-2 hover:bg-slate-800 rounded-full transition-colors"
              >
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <div>
                <h2 className="text-lg font-bold text-white">
                  {mobileReportResult ? 'Report Results' : mobileReportType ? 'Configure Report' : 'Reports Center'}
                </h2>
                {mobileReportType && !mobileReportResult && (
                  <p className="text-xs text-blue-400">{mobileReportType}</p>
                )}
              </div>
            </div>

            {/* Content Area */}
            <div className="flex-1 overflow-y-auto bg-slate-950">
              
              {/* 1. Report Selection (Home) */}
              {!mobileReportType && (
                <div className="p-4 space-y-6">
                  {/* Featured: Financial Health */}
                  <button 
                    onClick={() => setMobileReportType('Financial Overview')}
                    className="w-full text-left group relative overflow-hidden rounded-3xl bg-gradient-to-br from-blue-600 to-indigo-600 p-6 shadow-2xl shadow-blue-900/20 transition-all active:scale-95"
                  >
                    <div className="absolute top-0 right-0 -mt-8 -mr-8 h-32 w-32 rounded-full bg-white/10 blur-2xl"></div>
                    <div className="relative z-10">
                      <div className="mb-4 inline-flex rounded-xl bg-white/20 p-3 backdrop-blur-md">
                        <DollarSign className="h-6 w-6 text-white" />
                      </div>
                      <h3 className="mb-2 text-2xl font-bold text-white">Financial Health</h3>
                      <p className="text-blue-100 text-sm">Generate P&L, Balance Sheet, and Cash Flow summaries instantly.</p>
                      <div className="mt-4 flex items-center gap-2 text-sm font-bold text-white">
                        <span>Start Analysis</span>
                        <ArrowUpRight className="h-4 w-4 transition-transform group-hover:translate-x-1 group-hover:-translate-y-1" />
                      </div>
                    </div>
                  </button>

                  {/* Categories Grid */}
                  <div>
                    <h3 className="mb-4 text-xs font-bold uppercase tracking-wider text-slate-500">Report Categories</h3>
                    <div className="grid grid-cols-2 gap-3">
                      {[
                        { id: 'Sales', icon: TrendingUp, color: 'text-emerald-400', bg: 'bg-emerald-500/10', border: 'border-emerald-500/20' },
                        { id: 'Expenses', icon: CreditCard, color: 'text-rose-400', bg: 'bg-rose-500/10', border: 'border-rose-500/20' },
                        { id: 'Inventory', icon: Package, color: 'text-amber-400', bg: 'bg-amber-500/10', border: 'border-amber-500/20' },
                        { id: 'Customers', icon: Users, color: 'text-blue-400', bg: 'bg-blue-500/10', border: 'border-blue-500/20' },
                        { id: 'Vendors', icon: Building2, color: 'text-purple-400', bg: 'bg-purple-500/10', border: 'border-purple-500/20' },
                        { id: 'Tax', icon: FileText, color: 'text-slate-400', bg: 'bg-slate-500/10', border: 'border-slate-500/20' },
                      ].map((cat) => (
                        <button
                          key={cat.id}
                          onClick={() => setMobileReportType(cat.id)}
                          className={`${cat.bg} ${cat.border} border flex flex-col items-center justify-center gap-3 rounded-2xl p-4 transition-all active:scale-95`}
                        >
                          <div className="rounded-full bg-slate-900/50 p-3">
                            <cat.icon className={`h-6 w-6 ${cat.color}`} />
                          </div>
                          <span className="font-medium text-white">{cat.id}</span>
                        </button>
                      ))}
                    </div>
                  </div>
                </div>
              )}

              {/* 2. Configuration Screen */}
              {mobileReportType && !mobileReportResult && (
                <div className="p-4 space-y-6 animate-in slide-in-from-right duration-200">
                  
                  {/* Date Range Selector */}
                  <div className="space-y-3">
                    <label className="text-sm font-medium text-slate-400">Time Period</label>
                    <div className="grid grid-cols-2 gap-2">
                      {['thisMonth', 'lastMonth', 'ytd', 'custom'].map((range) => (
                        <button
                          key={range}
                          onClick={() => setMobileReportDateRange(range as any)}
                          className={`px-4 py-3 rounded-xl text-sm font-medium border transition-all ${
                            mobileReportDateRange === range
                              ? 'bg-blue-600 border-blue-500 text-white shadow-lg shadow-blue-900/20'
                              : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                          }`}
                        >
                          {range === 'thisMonth' ? 'This Month' : 
                           range === 'lastMonth' ? 'Last Month' : 
                           range === 'ytd' ? 'Year to Date' : 'Custom Range'}
                        </button>
                      ))}
                    </div>
                  </div>

                  {/* Report Subtype (Dynamic based on Type) */}
                  <div className="space-y-3">
                    <label className="text-sm font-medium text-slate-400">Report Type</label>
                    <div className="space-y-2">
                      {(mobileReportType === 'Financial Overview' ? ['Profit & Loss', 'Balance Sheet', 'Cash Flow'] :
                        mobileReportType === 'Sales' ? ['Sales by Customer', 'Sales by Item', 'Aged Receivables'] :
                        mobileReportType === 'Expenses' ? ['Expenses by Vendor', 'Expense Accounts', 'Aged Payables'] :
                        ['Summary Report', 'Detailed Report', 'Exception Report']
                      ).map((subtype) => (
                        <button
                          key={subtype}
                          onClick={() => setMobileReportSubtype(subtype)}
                          className={`w-full flex items-center justify-between px-4 py-3 rounded-xl border transition-all ${
                            mobileReportSubtype === subtype
                              ? 'bg-slate-800 border-blue-500 text-white ring-1 ring-blue-500'
                              : 'bg-slate-800/50 border-slate-700 text-slate-400'
                          }`}
                        >
                          <span>{subtype}</span>
                          {mobileReportSubtype === subtype && <CheckCircle className="w-5 h-5 text-blue-500" />}
                        </button>
                      ))}
                    </div>
                  </div>

                  {/* Generate Button */}
                  <div className="pt-4">
                    <button
                      onClick={() => {
                        setMobileReportGenerating(true);
                        
                        // Use a small timeout to allow UI to update
                        setTimeout(() => {
                          try {
                            let resultData: any = {
                              generatedAt: new Date().toLocaleString(),
                              total: 0,
                              items: [],
                              type: mobileReportType
                            };

                            // Logic to fetch REAL data based on report type
                            if (mobileReportType === 'Financial Overview') {
                              resultData.total = stats.totalRevenue - stats.totalExpenses;
                              resultData.items = accounts; 
                              resultData.revenue = stats.totalRevenue;
                              resultData.expenses = stats.totalExpenses;
                            } 
                            else if (mobileReportType === 'Sales') {
                              resultData.total = stats.totalRevenue;
                              // Filter customers with balance > 0 for sales report
                              resultData.items = customers.filter(c => c.Balance > 0);
                            } 
                            else if (mobileReportType === 'Expenses') {
                              resultData.total = stats.totalExpenses;
                              // Filter vendors with balance > 0 for expenses report
                              resultData.items = vendors.filter(v => v.Balance > 0);
                            } 
                            else if (mobileReportType === 'Customers') {
                              resultData.total = customers.length;
                              resultData.items = customers;
                            } 
                            else if (mobileReportType === 'Vendors') {
                              resultData.total = vendors.length;
                              resultData.items = vendors;
                            }
                            else if (mobileReportType === 'Inventory') {
                              // Placeholder for inventory - use tables count or similar if no item data
                              resultData.total = tables.length; 
                              resultData.items = tables;
                            }
                            else {
                              // Default fallback
                              resultData.total = 0;
                              resultData.items = [];
                            }

                            setMobileReportResult(resultData);
                          } catch (e) {
                            console.error('Report generation failed:', e);
                            setMobileError('Failed to generate report');
                          } finally {
                            setMobileReportGenerating(false);
                          }
                        }, 1000);
                      }}
                      disabled={mobileReportGenerating}
                      className="w-full py-4 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-xl text-white font-bold text-lg shadow-xl shadow-blue-900/20 active:scale-95 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                    >
                      {mobileReportGenerating ? (
                        <>
                          <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                          Generating...
                        </>
                      ) : (
                        <>
                          <FileText className="w-5 h-5" />
                          Generate Report
                        </>
                      )}
                    </button>
                  </div>
                </div>
              )}

              {/* 3. Results Screen */}
              {mobileReportResult && (
                <div className="p-4 space-y-6 animate-in slide-in-from-bottom duration-300">
                  
                  {/* Summary Card */}
                  <div className="bg-slate-800 border border-slate-700 rounded-2xl p-6 text-center">
                    <p className="text-slate-400 text-sm uppercase tracking-wider mb-1">{mobileReportSubtype}</p>
                    <h3 className="text-3xl font-bold text-white mb-2">{formatCurrency(mobileReportResult.total)}</h3>
                    <p className="text-xs text-slate-500">Generated: {mobileReportResult.generatedAt}</p>
                  </div>

                  {/* Chart Placeholder */}
                  <div className="h-48 bg-slate-800/50 border border-slate-700 rounded-2xl p-4 flex items-center justify-center">
                    <div className="text-center">
                      <BarChart3 className="w-8 h-8 text-slate-600 mx-auto mb-2" />
                      <p className="text-slate-500 text-sm">Visual Analysis</p>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="grid grid-cols-2 gap-3">
                    <button 
                      onClick={() => {
                        if (!mobileReportResult) return;
                        
                        try {
                          if (mobileReportType === 'Financial Overview') {
                            exportFinancialReportPDF(accounts, stats.totalRevenue, stats.totalExpenses);
                          } else if (mobileReportType === 'Sales' || mobileReportType === 'Customers') {
                            exportArApAgingPDF(customers, vendors, 'ar');
                          } else if (mobileReportType === 'Expenses' || mobileReportType === 'Vendors') {
                            exportArApAgingPDF(customers, vendors, 'ap');
                          } else {
                            // Default or other types
                            alert('PDF export for this report type is coming soon!');
                          }
                        } catch (e) {
                          console.error('PDF export failed:', e);
                          alert('Failed to generate PDF. Please try again.');
                        }
                      }}
                      className="flex items-center justify-center gap-2 py-3 bg-slate-800 border border-slate-700 rounded-xl text-white font-medium active:bg-slate-700 hover:bg-slate-700 transition-colors"
                    >
                      <Download className="w-5 h-5" />
                      PDF
                    </button>
                    <button 
                      onClick={() => {
                        if (navigator.share && mobileReportResult) {
                          navigator.share({
                            title: `${mobileReportType} Report`,
                            text: `Generated on ${mobileReportResult.generatedAt}. Total: ${formatCurrency(mobileReportResult.total)}`,
                          }).catch(console.error);
                        } else {
                          alert('Sharing is not supported on this device/browser');
                        }
                      }}
                      className="flex items-center justify-center gap-2 py-3 bg-slate-800 border border-slate-700 rounded-xl text-white font-medium active:bg-slate-700 hover:bg-slate-700 transition-colors"
                    >
                      <Share2 className="w-5 h-5" />
                      Share
                    </button>
                  </div>
                </div>
              )}

            </div>
          </div>
        )}

        {/* Analytics Overlay */}
        {mobileView === 'analytics' && (
          <div className="fixed inset-0 bg-slate-950 z-50 flex flex-col animate-in slide-in-from-right duration-200">
            <div className="bg-slate-900 border-b border-slate-800 p-4 flex items-center gap-3">
              <button onClick={() => setMobileView('home')} className="p-2 hover:bg-slate-800 rounded-full">
                <ArrowLeft className="w-6 h-6 text-white" />
              </button>
              <h2 className="text-lg font-bold text-white">Business Analytics</h2>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-6">
              
              {/* Key Metrics Row */}
              <div className="grid grid-cols-2 gap-3">
                <div className="bg-slate-800/50 border border-slate-700 rounded-xl p-4">
                  <p className="text-slate-400 text-xs uppercase mb-1">Revenue</p>
                  <p className="text-xl font-bold text-green-400">{formatCurrency(stats.totalRevenue)}</p>
                  <div className="flex items-center gap-1 mt-1 text-xs text-green-500">
                    <ArrowUpRight className="w-3 h-3" />
                    <span>+12.5%</span>
                  </div>
                </div>
                <div className="bg-slate-800/50 border border-slate-700 rounded-xl p-4">
                  <p className="text-slate-400 text-xs uppercase mb-1">Expenses</p>
                  <p className="text-xl font-bold text-red-400">{formatCurrency(stats.totalExpenses)}</p>
                  <div className="flex items-center gap-1 mt-1 text-xs text-red-500">
                    <ArrowUpRight className="w-3 h-3" />
                    <span>+5.2%</span>
                  </div>
                </div>
              </div>

              {/* Customer Distribution */}
              <div className="bg-slate-800/50 border border-slate-700 rounded-2xl p-5">
                <div className="flex items-center justify-between mb-6">
                  <h3 className="text-white font-bold">Top Customers</h3>
                  <button className="text-blue-400 text-xs font-medium">View All</button>
                </div>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={customers.slice(0, 5)} layout="vertical" margin={{ left: 0, right: 0, top: 0, bottom: 0 }}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#334155" horizontal={false} />
                      <XAxis type="number" hide />
                      <YAxis dataKey="CustomerName" type="category" width={100} tick={{ fill: '#94a3b8', fontSize: 10 }} />
                      <Tooltip 
                        cursor={{ fill: '#334155', opacity: 0.2 }}
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                      />
                      <Bar dataKey="Balance" fill="#3b82f6" radius={[0, 4, 4, 0]} barSize={20} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              {/* Vendor Distribution */}
              <div className="bg-slate-800/50 border border-slate-700 rounded-2xl p-5">
                <div className="flex items-center justify-between mb-6">
                  <h3 className="text-white font-bold">Top Vendors</h3>
                  <button className="text-purple-400 text-xs font-medium">View All</button>
                </div>
                <div className="h-64 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={vendors.slice(0, 5)} layout="vertical" margin={{ left: 0, right: 0, top: 0, bottom: 0 }}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#334155" horizontal={false} />
                      <XAxis type="number" hide />
                      <YAxis dataKey="VendorName" type="category" width={100} tick={{ fill: '#94a3b8', fontSize: 10 }} />
                      <Tooltip 
                        cursor={{ fill: '#334155', opacity: 0.2 }}
                        contentStyle={{ backgroundColor: '#1e293b', border: '1px solid #334155', borderRadius: '8px', fontSize: '12px' }}
                      />
                      <Bar dataKey="Balance" fill="#8b5cf6" radius={[0, 4, 4, 0]} barSize={20} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              {/* Data Quality */}
              <div className="bg-slate-800/50 border border-slate-700 rounded-2xl p-5">
                <h3 className="text-white font-bold mb-4">System Health</h3>
                <div className="space-y-4">
                  {[
                    { label: 'Customer Data', value: stats.dataCompleteness.customers, color: 'bg-blue-500' },
                    { label: 'Vendor Data', value: stats.dataCompleteness.vendors, color: 'bg-purple-500' },
                    { label: 'Account Data', value: stats.dataCompleteness.accounts, color: 'bg-green-500' }
                  ].map((item, i) => (
                    <div key={i}>
                      <div className="flex justify-between text-sm mb-1.5">
                        <span className="text-slate-400">{item.label}</span>
                        <span className="text-white font-medium">{item.value}%</span>
                      </div>
                      <div className="h-2 bg-slate-700 rounded-full overflow-hidden">
                        <div className={`h-full ${item.color} transition-all duration-1000`} style={{ width: `${item.value}%` }}></div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        )}

      </div>

      {/* Invoice Details Popup Modal */}
      {(selectedCustomerForInvoice || selectedVendorForInvoice) && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-md flex items-center justify-center z-[200] p-4 animate-in fade-in duration-200">
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

                  {/* Invoice Table - Desktop */}
                  <div className="hidden md:block bg-slate-900/50 border border-slate-700 rounded-xl overflow-hidden shadow-xl">
                    <div className="overflow-x-auto">
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
                          <th className="text-center text-slate-200 font-bold py-4 px-4 uppercase text-xs tracking-wider">
                            Qty
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

                              {/* Quantity */}
                              <td className="py-4 px-4 text-center">
                                {invoice.Quantity && parseFloat(invoice.Quantity) !== 0 ? (
                                  <span className="text-slate-300 font-mono text-sm">
                                    {parseFloat(invoice.Quantity)}
                                  </span>
                                ) : (
                                  <span className="text-slate-600 text-xs">-</span>
                                )}
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

                  {/* Invoice List - Mobile Card View (Advanced) */}
                  <div className="md:hidden space-y-4 pb-20">
                    {(() => {
                      // Group invoices by InvoiceID to show one card per invoice
                      const groupedInvoices = invoiceDetails.reduce((acc: any, curr: any) => {
                        // Use InvoiceID as key, fallback to TransactionID if N/A
                        const key = (curr.InvoiceID && curr.InvoiceID !== 'N/A') 
                          ? curr.InvoiceID 
                          : (curr.TransactionID || `unknown-${Math.random()}`);
                        
                        if (!acc[key]) {
                          acc[key] = { 
                            ...curr, 
                            TotalAmount: 0,
                            Count: 0,
                            Items: []
                          };
                        }
                        
                        // Only add to total if it's a line item (RowType 0, RowNumber > 0)
                        // OR if it's a simple transaction without row details
                        const isLineItem = (curr.RowType === 0 || curr.RowType === undefined) && 
                                         (curr.RowNumber > 0 || curr.RowNumber === undefined) &&
                                         Math.abs(curr.Amount) > 0;

                        if (isLineItem) {
                          acc[key].TotalAmount += Math.abs(curr.Amount || 0);
                          acc[key].Count += 1;
                          acc[key].Items.push(curr);
                        }
                        
                        // If this is the header row (RowNumber 0), update the main description/date if needed
                        if (curr.RowNumber === 0) {
                           acc[key].Date = curr.Date || acc[key].Date;
                           acc[key].DueDate = curr.DueDate || acc[key].DueDate;
                           // Don't overwrite description if we already have one, unless it's generic
                           if (!acc[key].Description || acc[key].Description === 'Transaction') {
                             acc[key].Description = curr.Description;
                           }
                        }

                        return acc;
                      }, {});

                      return Object.values(groupedInvoices).map((invoice: any, index: number) => {
                        // Skip if no items and no amount (empty/header only invoice)
                        if (invoice.Items.length === 0 && invoice.TotalAmount === 0) return null;

                        const isDebit = invoice.TotalAmount >= 0; // Use calculated total
                        const isPosted = invoice.Status === 'Paid' || invoice.Status === 'Posted';
                        
                        return (
                          <div 
                            key={index} 
                            className={`relative bg-gradient-to-br from-slate-800 to-slate-900 border border-slate-700 rounded-2xl p-5 shadow-lg overflow-hidden ${
                              // Determine color based on transaction type (Invoice vs Payment)
                              // If items are mostly negative (Sales), it's an Invoice (Green/Blue)
                              // If items are positive (Receipts), it's a Payment (Red/Orange)
                              // But here we use TotalAmount which is abs sum.
                              // Let's check the first item's sign to guess type
                              (invoice.Items[0]?.Amount || 0) < 0 
                                ? 'border-l-4 border-l-green-500' // Sales/Credit
                                : 'border-l-4 border-l-red-500'   // Payment/Debit
                            }`}
                          >
                            {/* Background Decoration */}
                            <div className="absolute top-0 right-0 w-24 h-24 bg-white/5 rounded-full -mr-8 -mt-8 blur-2xl pointer-events-none"></div>
                            
                            {/* Header Row */}
                            <div className="flex justify-between items-start mb-3 relative z-10">
                              <div className="flex flex-col">
                                <span className="text-[10px] text-slate-400 uppercase tracking-wider font-bold mb-0.5">
                                  {invoice.Type || 'Transaction'}
                                </span>
                                <div className="flex items-center gap-2">
                                  <span className="text-white font-mono font-bold text-lg tracking-tight">
                                    {invoice.InvoiceID || invoice.TransactionID || 'N/A'}
                                  </span>
                                  {isPosted && <CheckCircle className="w-4 h-4 text-green-500" />}
                                  {invoice.Count > 1 && (
                                    <span className="text-[10px] bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded-full">
                                      {invoice.Count} items
                                    </span>
                                  )}
                                </div>
                              </div>
                              <span className={`px-3 py-1 rounded-full text-xs font-bold shadow-sm ${
                                isPosted
                                  ? 'bg-green-500 text-white shadow-green-500/20'
                                  : 'bg-yellow-500 text-white shadow-yellow-500/20'
                              }`}>
                                {invoice.Status}
                              </span>
                            </div>

                            {/* Line Items List */}
                            <div className="mb-4 relative z-10">
                              <div className="bg-slate-950/30 rounded-xl p-3 space-y-2 max-h-[200px] overflow-y-auto scrollbar-thin scrollbar-thumb-slate-700">
                                {invoice.Items.map((item: any, idx: number) => (
                                  <div key={idx} className="flex justify-between items-start gap-3 text-sm border-b border-white/5 last:border-0 pb-2 last:pb-0">
                                    <div className="flex flex-col flex-1">
                                      <span className="text-slate-300 leading-snug text-xs md:text-sm">
                                        {item.Description || 'Item Details'}
                                      </span>
                                      {item.Quantity && parseFloat(item.Quantity) !== 0 && (
                                        <span className="text-slate-500 text-[10px] font-mono mt-0.5">
                                          Qty: {parseFloat(item.Quantity)}
                                        </span>
                                      )}
                                    </div>
                                    <span className={`font-mono text-xs font-medium whitespace-nowrap ${
                                      item.Amount >= 0 ? 'text-red-300' : 'text-green-300'
                                    }`}>
                                      {formatCurrency(Math.abs(item.Amount))}
                                    </span>
                                  </div>
                                ))}
                              </div>
                            </div>

                            {/* Details Grid */}
                            <div className="grid grid-cols-2 gap-4 mb-4 relative z-10">
                              <div className="bg-slate-950/30 rounded-lg p-2">
                                <div className="flex items-center gap-1.5 text-slate-500 text-xs mb-1">
                                  <Calendar className="w-3 h-3" />
                                  <span>Date</span>
                                </div>
                                <div className="text-slate-200 font-medium text-sm">{invoice.Date}</div>
                              </div>
                              <div className="bg-slate-950/30 rounded-lg p-2">
                                <div className="flex items-center gap-1.5 text-slate-500 text-xs mb-1">
                                  <Clock className="w-3 h-3" />
                                  <span>Due Date</span>
                                </div>
                                <div className="text-slate-200 font-medium text-sm">{invoice.DueDate || '-'}</div>
                              </div>
                            </div>

                            {/* Footer / Amount */}
                            <div className="flex justify-between items-center pt-3 border-t border-slate-700/50 relative z-10">
                              <div className="flex items-center gap-2">
                                <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
                                  (invoice.Items[0]?.Amount || 0) >= 0 ? 'bg-red-500/10 text-red-400' : 'bg-green-500/10 text-green-400'
                                }`}>
                                  {(invoice.Items[0]?.Amount || 0) >= 0 ? <ArrowUpRight className="w-4 h-4" /> : <ArrowDownRight className="w-4 h-4" />}
                                </div>
                                <span className="text-xs text-slate-400 font-medium">
                                  Total Amount
                                </span>
                              </div>
                              <div className={`text-2xl font-bold tracking-tight ${
                                (invoice.Items[0]?.Amount || 0) >= 0 ? 'text-red-400' : 'text-green-400'
                              }`}>
                                {formatCurrency(invoice.TotalAmount)}
                              </div>
                            </div>
                          </div>
                        );
                      });
                    })()}
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

      {/* Mobile Search Overlay */}
      {mobileView === 'search' && (
        <div className="fixed inset-0 bg-slate-900 z-50 flex flex-col animate-in slide-in-from-bottom-10 duration-200">
          <div className="p-4 border-b border-slate-800 flex items-center gap-3">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
              <input
                type="text"
                placeholder="Search invoices, customers..."
                className="w-full bg-slate-800 border-none rounded-xl py-3 pl-10 pr-4 text-white placeholder:text-slate-500 focus:ring-2 focus:ring-blue-500"
                autoFocus
              />
            </div>
            <button 
              onClick={() => setMobileView('home')}
              className="p-2 text-slate-400 hover:text-white"
            >
              Cancel
            </button>
          </div>
          <div className="flex-1 p-4">
            <h3 className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-3">Quick Links</h3>
            <div className="space-y-2">
              <button onClick={() => setMobileView('customers')} className="w-full flex items-center gap-3 p-3 bg-slate-800/50 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Users className="w-5 h-5 text-blue-400" />
                <span>Search Customers</span>
              </button>
              <button onClick={() => setMobileView('vendors')} className="w-full flex items-center gap-3 p-3 bg-slate-800/50 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Truck className="w-5 h-5 text-purple-400" />
                <span>Search Vendors</span>
              </button>
              <button onClick={() => setMobileView('reports')} className="w-full flex items-center gap-3 p-3 bg-slate-800/50 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <FileText className="w-5 h-5 text-emerald-400" />
                <span>Find Invoices</span>
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Mobile Quick Actions Overlay */}
      {mobileView === 'quick-actions' && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-end sm:items-center justify-center p-4 animate-in fade-in duration-200">
          <div className="bg-slate-800 w-full max-w-sm rounded-2xl overflow-hidden shadow-2xl animate-in slide-in-from-bottom-10 duration-300">
            <div className="p-4 border-b border-slate-700 flex items-center justify-between">
              <h3 className="font-bold text-white">Quick Actions</h3>
              <button onClick={() => setMobileView('home')} className="p-1 text-slate-400 hover:text-white">
                <X className="w-5 h-5" />
              </button>
            </div>
            <div className="grid grid-cols-3 gap-4 p-6">
              <button className="flex flex-col items-center gap-2 group">
                <div className="w-12 h-12 rounded-xl bg-blue-500/20 flex items-center justify-center group-hover:bg-blue-500/30 transition-colors">
                  <FileText className="w-6 h-6 text-blue-400" />
                </div>
                <span className="text-xs text-slate-300 text-center">New Invoice</span>
              </button>
              <button className="flex flex-col items-center gap-2 group">
                <div className="w-12 h-12 rounded-xl bg-purple-500/20 flex items-center justify-center group-hover:bg-purple-500/30 transition-colors">
                  <Users className="w-6 h-6 text-purple-400" />
                </div>
                <span className="text-xs text-slate-300 text-center">Add Customer</span>
              </button>
              <button className="flex flex-col items-center gap-2 group">
                <div className="w-12 h-12 rounded-xl bg-emerald-500/20 flex items-center justify-center group-hover:bg-emerald-500/30 transition-colors">
                  <RefreshCw className="w-6 h-6 text-emerald-400" />
                </div>
                <span className="text-xs text-slate-300 text-center">Sync Data</span>
              </button>
              <button className="flex flex-col items-center gap-2 group">
                <div className="w-12 h-12 rounded-xl bg-amber-500/20 flex items-center justify-center group-hover:bg-amber-500/30 transition-colors">
                  <AlertTriangle className="w-6 h-6 text-amber-400" />
                </div>
                <span className="text-xs text-slate-300 text-center">Report Issue</span>
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Mobile Alerts Overlay */}
      {mobileView === 'alerts' && (
        <div className="fixed inset-0 bg-slate-900 z-50 flex flex-col animate-in slide-in-from-bottom-10 duration-200">
          <div className="p-4 border-b border-slate-800 flex items-center justify-between">
            <h2 className="text-lg font-bold text-white">Notifications</h2>
            <button 
              onClick={() => setMobileView('home')}
              className="p-2 text-slate-400 hover:text-white bg-slate-800 rounded-lg"
            >
              <X className="w-5 h-5" />
            </button>
          </div>
          <div className="flex-1 p-4 flex flex-col items-center justify-center text-slate-500">
            <div className="w-16 h-16 bg-slate-800 rounded-full flex items-center justify-center mb-4">
              <Bell className="w-8 h-8 text-slate-600" />
            </div>
            <p>No new notifications</p>
          </div>
        </div>
      )}

      {/* Mobile Menu Overlay */}
      {mobileView === 'menu' && (
        <div className="fixed inset-0 bg-slate-900 z-50 flex flex-col animate-in slide-in-from-right duration-200">
          <div className="p-6 bg-gradient-to-br from-slate-800 to-slate-900 border-b border-slate-700">
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-blue-600 rounded-xl flex items-center justify-center text-white font-bold text-xl shadow-lg shadow-blue-900/20">
                  LT
                </div>
                <div>
                  <h2 className="text-lg font-bold text-white">Lasantha Tire</h2>
                  <p className="text-slate-400 text-sm">Admin Dashboard</p>
                </div>
              </div>
              <button 
                onClick={() => setMobileView('home')}
                className="p-2 text-slate-400 hover:text-white bg-slate-800/50 rounded-lg"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
          </div>
          
          <div className="flex-1 overflow-y-auto p-4">
            <div className="space-y-1">
              <button onClick={() => setMobileView('home')} className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Home className="w-6 h-6 text-blue-400" />
                <span className="font-medium">Dashboard Home</span>
              </button>
              <button onClick={() => setMobileView('customers')} className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Users className="w-6 h-6 text-purple-400" />
                <span className="font-medium">Customers</span>
              </button>
              <button onClick={() => setMobileView('vendors')} className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Truck className="w-6 h-6 text-emerald-400" />
                <span className="font-medium">Vendors</span>
              </button>
              <button onClick={() => setMobileView('accounts')} className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <Wallet className="w-6 h-6 text-amber-400" />
                <span className="font-medium">Accounts</span>
              </button>
              <button onClick={() => setMobileView('reports')} className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-300 hover:bg-slate-800 hover:text-white transition-colors">
                <FileText className="w-6 h-6 text-rose-400" />
                <span className="font-medium">Reports</span>
              </button>
            </div>

            <div className="my-6 border-t border-slate-800"></div>

            <div className="space-y-1">
              <button className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-400 hover:bg-slate-800 hover:text-white transition-colors">
                <Settings className="w-6 h-6" />
                <span className="font-medium">Settings</span>
              </button>
              <button className="w-full flex items-center gap-4 p-4 rounded-xl text-slate-400 hover:bg-slate-800 hover:text-white transition-colors">
                <HelpCircle className="w-6 h-6" />
                <span className="font-medium">Help & Support</span>
              </button>
            </div>
          </div>
          
          <div className="p-4 border-t border-slate-800 text-center text-slate-600 text-xs">
            v1.0.0 • Peachtree Bridge Connected
          </div>
        </div>
      )}
    </div>
  );
}
