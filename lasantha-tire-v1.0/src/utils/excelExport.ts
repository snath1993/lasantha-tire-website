// Excel export utility with chart support using xlsx library
// Requires: npm install xlsx --save

import * as XLSX from 'xlsx';

export interface ExcelChartData {
  title: string;
  data: any[];
  xKey: string;
  yKey: string;
  type: 'bar' | 'line' | 'pie';
}

export interface ExcelExportOptions {
  filename: string;
  sheetName?: string;
  data: any[];
  charts?: ExcelChartData[];
  headers?: string[];
  columnWidths?: number[];
  includeTimestamp?: boolean;
}

/**
 * Format currency values for Excel
 */
function formatCurrency(value: number): string {
  return `$${value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')}`;
}

/**
 * Format date values for Excel
 */
function formatDate(date: Date | string): string {
  if (!date) return '';
  const d = typeof date === 'string' ? new Date(date) : date;
  return d.toLocaleDateString();
}

/**
 * Export data to Excel with formatting
 */
export async function exportToExcel({
  filename,
  sheetName = 'Sheet1',
  data,
  headers,
  columnWidths,
  includeTimestamp = true
}: ExcelExportOptions): Promise<void> {
  try {
    // Create workbook
    const workbook = XLSX.utils.book_new();

    // Prepare data with headers
    const worksheetData = headers 
      ? [headers, ...data.map(row => 
          headers.map(header => row[header] ?? '')
        )]
      : data;

    // Create worksheet
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);

    // Apply column widths
    if (columnWidths) {
      worksheet['!cols'] = columnWidths.map(width => ({ wch: width }));
    }

    // Add worksheet to workbook
    XLSX.utils.book_append_sheet(workbook, worksheet, sheetName);

    // Generate filename with timestamp if requested
    const timestamp = includeTimestamp 
      ? `_${new Date().toISOString().split('T')[0]}`
      : '';
    const finalFilename = `${filename}${timestamp}.xlsx`;

    // Write file
    XLSX.writeFile(workbook, finalFilename);

    return Promise.resolve();
  } catch (error) {
    console.error('[Excel Export] Error:', error);
    return Promise.reject(error);
  }
}

/**
 * Export customers to Excel
 */
export async function exportCustomersToExcel(customers: any[]): Promise<void> {
  const data = customers.map(c => ({
    'Customer ID': c.CustomerID || c.customer_id || '',
    'Name': c.CustomerName || c.customer_name || '',
    'Contact': c.Contact || c.contact || '',
    'Phone': c.Phone || c.phone || '',
    'Balance': formatCurrency(c.Balance || c.balance || 0),
    'Credit Limit': formatCurrency(c.CreditLimit || c.credit_limit || 0),
    'Status': c.Status || c.status || 'Active'
  }));

  return exportToExcel({
    filename: 'Peachtree_Customers',
    sheetName: 'Customers',
    data,
    headers: ['Customer ID', 'Name', 'Contact', 'Phone', 'Balance', 'Credit Limit', 'Status'],
    columnWidths: [15, 25, 20, 15, 15, 15, 12]
  });
}

/**
 * Export vendors to Excel
 */
export async function exportVendorsToExcel(vendors: any[]): Promise<void> {
  const data = vendors.map(v => ({
    'Vendor ID': v.VendorID || v.vendor_id || '',
    'Name': v.VendorName || v.vendor_name || '',
    'Contact': v.Contact || v.contact || '',
    'Phone': v.Phone || v.phone || '',
    'Balance': formatCurrency(v.Balance || v.balance || 0),
    'Status': v.Status || v.status || 'Active'
  }));

  return exportToExcel({
    filename: 'Peachtree_Vendors',
    sheetName: 'Vendors',
    data,
    headers: ['Vendor ID', 'Name', 'Contact', 'Phone', 'Balance', 'Status'],
    columnWidths: [15, 25, 20, 15, 15, 12]
  });
}

/**
 * Export GL accounts to Excel
 */
export async function exportAccountsToExcel(accounts: any[]): Promise<void> {
  const data = accounts.map(a => ({
    'Account ID': a.AccountID || a.account_id || '',
    'Name': a.AccountName || a.account_name || '',
    'Type': a.AccountType || a.account_type || '',
    'Balance': formatCurrency(a.Balance || a.balance || 0),
    'Active': a.Active ? 'Yes' : 'No'
  }));

  return exportToExcel({
    filename: 'Peachtree_Accounts',
    sheetName: 'Chart of Accounts',
    data,
    headers: ['Account ID', 'Name', 'Type', 'Balance', 'Active'],
    columnWidths: [15, 30, 15, 15, 10]
  });
}

/**
 * Export transactions to Excel
 */
export async function exportTransactionsToExcel(transactions: any[]): Promise<void> {
  const data = transactions.map(t => ({
    'Transaction ID': t.TransactionID || t.transaction_id || '',
    'Date': formatDate(t.Date || t.date || ''),
    'Type': t.Type || t.type || '',
    'Description': t.Description || t.description || '',
    'Amount': formatCurrency(t.Amount || t.amount || 0),
    'Status': t.Status || t.status || ''
  }));

  return exportToExcel({
    filename: 'Peachtree_Transactions',
    sheetName: 'Transactions',
    data,
    headers: ['Transaction ID', 'Date', 'Type', 'Description', 'Amount', 'Status'],
    columnWidths: [18, 12, 12, 35, 15, 12]
  });
}

/**
 * Export dashboard summary with multiple sheets
 */
export async function exportDashboardToExcel({
  customers,
  vendors,
  accounts,
  transactions,
  stats
}: {
  customers: any[];
  vendors: any[];
  accounts: any[];
  transactions: any[];
  stats: any;
}): Promise<void> {
  try {
    const workbook = XLSX.utils.book_new();

    // Summary sheet
    const summaryData = [
      ['Peachtree Dashboard Summary'],
      ['Generated:', new Date().toLocaleString()],
      [''],
      ['Metric', 'Value'],
      ['Total Customers', stats.totalCustomers || 0],
      ['Active Customers', stats.activeCustomers || 0],
      ['Total Vendors', stats.totalVendors || 0],
      ['Active Vendors', stats.activeVendors || 0],
      ['Total Accounts', stats.totalAccounts || 0],
      ['Total Transactions', stats.totalTransactions || 0],
      ['Total Revenue', formatCurrency(stats.totalRevenue || 0)],
      ['Total Expenses', formatCurrency(stats.totalExpenses || 0)]
    ];
    const summarySheet = XLSX.utils.aoa_to_sheet(summaryData);
    summarySheet['!cols'] = [{ wch: 25 }, { wch: 20 }];
    XLSX.utils.book_append_sheet(workbook, summarySheet, 'Summary');

    // Customers sheet
    if (customers.length > 0) {
      const customerData = customers.map(c => [
        c.CustomerID || c.customer_id || '',
        c.CustomerName || c.customer_name || '',
        c.Contact || c.contact || '',
        c.Phone || c.phone || '',
        c.Balance || c.balance || 0,
        c.CreditLimit || c.credit_limit || 0,
        c.Status || c.status || 'Active'
      ]);
      const customerSheet = XLSX.utils.aoa_to_sheet([
        ['Customer ID', 'Name', 'Contact', 'Phone', 'Balance', 'Credit Limit', 'Status'],
        ...customerData
      ]);
      customerSheet['!cols'] = [
        { wch: 15 }, { wch: 25 }, { wch: 20 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 12 }
      ];
      XLSX.utils.book_append_sheet(workbook, customerSheet, 'Customers');
    }

    // Vendors sheet
    if (vendors.length > 0) {
      const vendorData = vendors.map(v => [
        v.VendorID || v.vendor_id || '',
        v.VendorName || v.vendor_name || '',
        v.Contact || v.contact || '',
        v.Phone || v.phone || '',
        v.Balance || v.balance || 0,
        v.Status || v.status || 'Active'
      ]);
      const vendorSheet = XLSX.utils.aoa_to_sheet([
        ['Vendor ID', 'Name', 'Contact', 'Phone', 'Balance', 'Status'],
        ...vendorData
      ]);
      vendorSheet['!cols'] = [
        { wch: 15 }, { wch: 25 }, { wch: 20 }, { wch: 15 }, { wch: 15 }, { wch: 12 }
      ];
      XLSX.utils.book_append_sheet(workbook, vendorSheet, 'Vendors');
    }

    // Accounts sheet
    if (accounts.length > 0) {
      const accountData = accounts.map(a => [
        a.AccountID || a.account_id || '',
        a.AccountName || a.account_name || '',
        a.AccountType || a.account_type || '',
        a.Balance || a.balance || 0,
        a.Active ? 'Yes' : 'No'
      ]);
      const accountSheet = XLSX.utils.aoa_to_sheet([
        ['Account ID', 'Name', 'Type', 'Balance', 'Active'],
        ...accountData
      ]);
      accountSheet['!cols'] = [
        { wch: 15 }, { wch: 30 }, { wch: 15 }, { wch: 15 }, { wch: 10 }
      ];
      XLSX.utils.book_append_sheet(workbook, accountSheet, 'Accounts');
    }

    // Transactions sheet
    if (transactions.length > 0) {
      const transactionData = transactions.map(t => [
        t.TransactionID || t.transaction_id || '',
        formatDate(t.Date || t.date || ''),
        t.Type || t.type || '',
        t.Description || t.description || '',
        t.Amount || t.amount || 0,
        t.Status || t.status || ''
      ]);
      const transactionSheet = XLSX.utils.aoa_to_sheet([
        ['Transaction ID', 'Date', 'Type', 'Description', 'Amount', 'Status'],
        ...transactionData
      ]);
      transactionSheet['!cols'] = [
        { wch: 18 }, { wch: 12 }, { wch: 12 }, { wch: 35 }, { wch: 15 }, { wch: 12 }
      ];
      XLSX.utils.book_append_sheet(workbook, transactionSheet, 'Transactions');
    }

    // Write file
    const filename = `Peachtree_Dashboard_${new Date().toISOString().split('T')[0]}.xlsx`;
    XLSX.writeFile(workbook, filename);

    return Promise.resolve();
  } catch (error) {
    console.error('[Excel Export] Dashboard export error:', error);
    return Promise.reject(error);
  }
}
