import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';

const createA4Doc = () =>
  new jsPDF({
    orientation: 'portrait',
    unit: 'mm',
    format: 'a4'
  });

// Company branding configuration
const COMPANY_CONFIG = {
  name: 'LASANTHA TYRE TRADERS',
  address: '1035 Pannipitiya Road, Kumaragewatta, Battaramulla',
  phone: '+94 77 313 1883',
  landline: '+94 11 277 3232',
  email: 'info@lasanthatyre.com',
  slogan: 'Go Smart with Lasantha Tyres',
  logo: null, // Can add base64 logo later
};

// Common PDF header
const addPDFHeader = (doc: jsPDF, title: string, subtitle?: string) => {
  // Company name
  doc.setFontSize(20);
  doc.setTextColor(30, 58, 138); // Blue
  doc.text(COMPANY_CONFIG.name, 14, 20);
  
  // Contact info
  doc.setFontSize(9);
  doc.setTextColor(100, 100, 100);
  doc.text(COMPANY_CONFIG.address, 14, 27);
  doc.text(`${COMPANY_CONFIG.phone} | ${COMPANY_CONFIG.email}`, 14, 32);
  
  // Report title
  doc.setFontSize(16);
  doc.setTextColor(0, 0, 0);
  doc.text(title, 14, 45);
  
  if (subtitle) {
    doc.setFontSize(11);
    doc.setTextColor(100, 100, 100);
    doc.text(subtitle, 14, 52);
  }
  
  // Date
  const today = new Date().toLocaleDateString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  });
  doc.setFontSize(9);
  doc.text(`Generated: ${today}`, doc.internal.pageSize.width - 14, 20, { align: 'right' });
  
  // Line separator
  doc.setDrawColor(200, 200, 200);
  doc.line(14, subtitle ? 57 : 50, doc.internal.pageSize.width - 14, subtitle ? 57 : 50);
  
  return subtitle ? 62 : 55;
};

// Format currency for PDF
const formatCurrency = (amount: number): string => {
  const safe = Number.isFinite(amount) ? amount : 0;
  return `Rs. ${safe.toLocaleString('en-LK', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
};

const parseAmount = (value: unknown): number => {
  if (typeof value === 'number') return Number.isFinite(value) ? value : 0;
  if (typeof value !== 'string') return 0;

  // Handles values like "Rs. 12,345.67" or "12,345.67"
  const normalized = value.replace(/[^0-9.+-]/g, '');
  const parsed = Number.parseFloat(normalized);
  return Number.isFinite(parsed) ? parsed : 0;
};

const getOutstanding = (row: any): number => {
  // Supports both Peachtree master lists (Balance) and business-status aging (TotalOutstanding)
  return parseAmount(row?.TotalOutstanding ?? row?.Balance ?? row?.Outstanding ?? 0);
};

const getPhone = (row: any): string => {
  const phone = row?.Phone ?? row?.Contact ?? row?.PhoneNumber ?? row?.Phone_Number;
  return typeof phone === 'string' && phone.trim().length > 0 ? phone.trim() : '-';
};

const getDaysOutstanding = (row: any): number => {
  const raw = row?.DaysOutstanding;
  const n = typeof raw === 'number' ? raw : Number.parseInt(String(raw ?? '0'), 10);
  return Number.isFinite(n) ? n : 0;
};

const getAgingBucketLabel = (daysOutstanding: number): string => {
  if (daysOutstanding <= 0) return 'Current';
  if (daysOutstanding <= 30) return '1-30';
  if (daysOutstanding <= 60) return '31-60';
  if (daysOutstanding <= 90) return '61-90';
  return '90+';
};

const addPDFFooter = (doc: jsPDF, generatedAt: Date = new Date()) => {
  const pageCount = (doc as any).internal.getNumberOfPages();
  const stamp = generatedAt.toLocaleString('en-GB');

  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(148, 163, 184);

    doc.text(
      `${COMPANY_CONFIG.name} • Generated ${stamp}`,
      14,
      doc.internal.pageSize.height - 8
    );
    doc.text(
      `Page ${i} of ${pageCount}`,
      doc.internal.pageSize.width - 14,
      doc.internal.pageSize.height - 8,
      { align: 'right' }
    );
  }
};

// Export Invoice Details PDF
export const exportInvoiceDetailsPDF = (
  invoices: any[],
  customerOrVendor: any,
  type: 'customer' | 'vendor'
) => {
  const doc = createA4Doc();
  const generatedAt = new Date();
  
  // 1. Professional Header
  const title = type === 'customer' 
    ? `Statement of Account`
    : `Vendor Statement`;
  
  const entityName = type === 'customer'
    ? customerOrVendor.CustomerName
    : customerOrVendor.VendorName;
    
  const entityId = customerOrVendor[type === 'customer' ? 'CustomerID' : 'VendorID'];
  
  const subtitle = `${entityName}`;
  
  let yPos = addPDFHeader(doc, title, subtitle);
  yPos += 5;
  
  // 2. Customer/Vendor Details Box (Left Side)
  doc.setFillColor(248, 250, 252); // Slate-50
  doc.setDrawColor(226, 232, 240); // Slate-200
  doc.roundedRect(14, yPos, 90, 35, 3, 3, 'FD');
  
  doc.setFontSize(10);
  doc.setTextColor(100, 116, 139); // Slate-500
  doc.text('Bill To:', 18, yPos + 8);
  
  doc.setFontSize(11);
  doc.setTextColor(15, 23, 42); // Slate-900
  doc.setFont('helvetica', 'bold');
  doc.text(entityName.substring(0, 35), 18, yPos + 14);
  
  doc.setFontSize(10);
  doc.setFont('helvetica', 'normal');
  doc.setTextColor(51, 65, 85); // Slate-700
  doc.text(`ID: ${entityId}`, 18, yPos + 20);
  doc.text(`Phone: ${customerOrVendor.Phone || 'N/A'}`, 18, yPos + 26);
  
  // 3. Summary Box (Right Side)
  const totalDebits = invoices
    .filter(inv => (inv.Amount || 0) > 0)
    .reduce((sum, inv) => sum + (inv.Amount || 0), 0);
    
  const totalCredits = Math.abs(invoices
    .filter(inv => (inv.Amount || 0) < 0)
    .reduce((sum, inv) => sum + (inv.Amount || 0), 0));
    
  const currentBalance = customerOrVendor.Balance || 0;
  
  doc.setFillColor(240, 253, 244); // Green-50
  if (currentBalance > 0) doc.setFillColor(254, 242, 242); // Red-50 if owing
  
  doc.roundedRect(110, yPos, doc.internal.pageSize.width - 124, 35, 3, 3, 'FD');
  
  doc.setFontSize(10);
  doc.setTextColor(100, 116, 139);
  doc.text('Account Summary', 115, yPos + 8);
  
  doc.setFontSize(16);
  doc.setFont('helvetica', 'bold');
  doc.setTextColor(currentBalance > 0 ? 220 : 22, currentBalance > 0 ? 38 : 163, currentBalance > 0 ? 38 : 74); // Red or Green
  doc.text(formatCurrency(currentBalance), 115, yPos + 18);
  
  doc.setFontSize(9);
  doc.setFont('helvetica', 'normal');
  doc.setTextColor(71, 85, 105);
  doc.text(`Total Invoiced: ${formatCurrency(totalDebits)}`, 115, yPos + 26);
  doc.text(`Total Paid: ${formatCurrency(totalCredits)}`, 115, yPos + 31);

  yPos += 45;
  
  // 4. Table with Quantity
  const tableData = invoices.map(inv => {
    const isDebit = (inv.Amount || 0) >= 0;
    return [
      inv.Date || '-',
      inv.InvoiceID || inv.TransactionID || '-',
      inv.Description || 'Transaction',
      inv.Quantity && parseFloat(inv.Quantity) !== 0 ? parseFloat(inv.Quantity) : '-', // Qty
      isDebit ? formatCurrency(Math.abs(inv.Amount)) : '-', // Debit
      !isDebit ? formatCurrency(Math.abs(inv.Amount)) : '-' // Credit
    ];
  });
  
  autoTable(doc, {
    startY: yPos,
    head: [['Date', 'Ref #', 'Description', 'Qty', 'Debit', 'Credit']],
    body: tableData,
    theme: 'grid',
    headStyles: {
      fillColor: [15, 23, 42], // Slate-900
      textColor: [255, 255, 255],
      fontStyle: 'bold',
      fontSize: 9,
      halign: 'center'
    },
    styles: {
      fontSize: 8,
      cellPadding: 3,
      textColor: [51, 65, 85]
    },
    columnStyles: {
      0: { cellWidth: 22 }, // Date
      1: { cellWidth: 25 }, // Ref
      2: { cellWidth: 'auto' }, // Desc
      3: { cellWidth: 15, halign: 'center' }, // Qty
      4: { cellWidth: 25, halign: 'right', textColor: [220, 38, 38] }, // Debit (Red)
      5: { cellWidth: 25, halign: 'right', textColor: [22, 163, 74] }  // Credit (Green)
    },
    alternateRowStyles: {
      fillColor: [248, 250, 252]
    }
  });
  
  // Footer
  addPDFFooter(doc, generatedAt);
  
  // 5. Mobile Open (Try to open in new tab/window which triggers PDF viewer)
  try {
    const blob = doc.output('blob');
    const blobUrl = URL.createObjectURL(blob);
    window.open(blobUrl, '_blank');
  } catch (e) {
    // Fallback to save if window.open fails (e.g. popup blocker)
    const fileName = `${type}_statement_${entityId}_${new Date().toISOString().split('T')[0]}.pdf`;
    doc.save(fileName);
  }
};

// Export AR/AP Aging Report PDF
export const exportArApAgingPDF = (
  customers: any[],
  vendors: any[],
  reportType: 'ar' | 'ap' | 'both',
  options?: {
    arSummary?: any;
    apSummary?: any;
  }
) => {
  const doc = createA4Doc();
  const generatedAt = new Date();
  
  const title = reportType === 'ar' ? 'Accounts Receivable Aging Report' :
                reportType === 'ap' ? 'Accounts Payable Aging Report' :
                'AR/AP Aging Report';
  
  let yPos = addPDFHeader(doc, title);
  yPos += 5;

  const DUST_THRESHOLD = 0.01;

  const arRows = (customers || []).filter(c => getOutstanding(c) > DUST_THRESHOLD);
  const apRows = (vendors || []).filter(v => getOutstanding(v) > DUST_THRESHOLD);

  const arTotal = options?.arSummary?.total != null ? parseAmount(options.arSummary.total) : arRows.reduce((s, c) => s + getOutstanding(c), 0);
  const apTotal = options?.apSummary?.total != null ? parseAmount(options.apSummary.total) : apRows.reduce((s, v) => s + getOutstanding(v), 0);
  
  // Summary boxes
  if (reportType === 'ar' || reportType === 'both') {
    const arCount = arRows.length;
    
    doc.setFillColor(220, 252, 231);
    doc.rect(14, yPos, (doc.internal.pageSize.width - 32) / 2 - 2, 20, 'F');
    doc.setFontSize(10);
    doc.setTextColor(0, 0, 0);
    doc.text('Accounts Receivable', 18, yPos + 7);
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text(formatCurrency(arTotal), 18, yPos + 15);
    doc.setFontSize(8);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(100, 100, 100);
    doc.text(`${arCount} customers`, 18, yPos + 19);
  }
  
  if (reportType === 'ap' || reportType === 'both') {
    const apCount = apRows.length;
    
    const xPos = reportType === 'both' ? (doc.internal.pageSize.width / 2) + 2 : 14;
    doc.setFillColor(254, 226, 226);
    doc.rect(xPos, yPos, (doc.internal.pageSize.width - 32) / 2 - 2, 20, 'F');
    doc.setFontSize(10);
    doc.setTextColor(0, 0, 0);
    doc.text('Accounts Payable', xPos + 4, yPos + 7);
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text(formatCurrency(apTotal), xPos + 4, yPos + 15);
    doc.setFontSize(8);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(100, 100, 100);
    doc.text(`${apCount} vendors`, xPos + 4, yPos + 19);
  }
  
  yPos += 25;

  // Aging bucket summary (if available)
  const renderBucketSummary = (label: string, summary: any) => {
    if (!summary) return;
    const current = parseAmount(summary.current);
    const d1_30 = parseAmount(summary.days_1_30);
    const d31_60 = parseAmount(summary.days_31_60);
    const d61_90 = parseAmount(summary.days_61_90);
    const over90 = parseAmount(summary.over_90);
    const total = parseAmount(summary.total);

    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text(`${label} Aging Summary`, 14, yPos);
    yPos += 4;

    autoTable(doc, {
      startY: yPos,
      head: [['Current', '1-30', '31-60', '61-90', '90+', 'Total']],
      body: [[
        formatCurrency(current),
        formatCurrency(d1_30),
        formatCurrency(d31_60),
        formatCurrency(d61_90),
        formatCurrency(over90),
        formatCurrency(total)
      ]],
      theme: 'grid',
      headStyles: {
        fillColor: [15, 23, 42],
        textColor: [255, 255, 255],
        fontStyle: 'bold',
        fontSize: 9,
        halign: 'center'
      },
      styles: {
        fontSize: 9,
        cellPadding: 2,
        halign: 'right'
      },
      columnStyles: {
        0: { halign: 'right' },
        1: { halign: 'right' },
        2: { halign: 'right' },
        3: { halign: 'right' },
        4: { halign: 'right' },
        5: { halign: 'right', fontStyle: 'bold' }
      }
    });
    yPos = (doc as any).lastAutoTable.finalY + 8;
  };

  if (reportType === 'ar' || reportType === 'both') {
    renderBucketSummary('Accounts Receivable', options?.arSummary);
  }
  if (reportType === 'ap' || reportType === 'both') {
    renderBucketSummary('Accounts Payable', options?.apSummary);
  }
  
  // AR Table
  if (reportType === 'ar' || reportType === 'both') {
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text('Receivables Details', 14, yPos);
    yPos += 5;
    
    const arData = arRows
      .map(c => {
        const outstanding = getOutstanding(c);
        const creditLimit = parseAmount(c?.CreditLimit ?? 0);
        const util = creditLimit > 0 ? `${((outstanding / creditLimit) * 100).toFixed(1)}%` : '-';
        const days = getDaysOutstanding(c);
        return [
          c.CustomerName ?? c.Customer_Bill_Name ?? '-',
          getPhone(c),
          String(days),
          getAgingBucketLabel(days),
          formatCurrency(outstanding),
          formatCurrency(creditLimit),
          util
        ];
      });
    
    autoTable(doc, {
      startY: yPos,
      head: [['Customer', 'Phone', 'Days', 'Bucket', 'Outstanding', 'Credit Limit', 'Utilization']],
      body: arData,
      theme: 'striped',
      headStyles: {
        fillColor: [34, 197, 94],
        textColor: [255, 255, 255],
        fontStyle: 'bold'
      },
      styles: { fontSize: 9, cellPadding: 2 },
      columnStyles: {
        2: { halign: 'right', cellWidth: 12 },
        3: { halign: 'center', cellWidth: 16 },
        4: { halign: 'right' },
        5: { halign: 'right' },
        6: { halign: 'center', cellWidth: 18 }
      }
    });
    
    yPos = (doc as any).lastAutoTable.finalY + 10;
  }
  
  // AP Table
  if (reportType === 'ap' || reportType === 'both') {
    if (yPos > doc.internal.pageSize.height - 60) {
      doc.addPage();
      yPos = 20;
    }
    
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text('Payables Details', 14, yPos);
    yPos += 5;
    
    const apData = apRows
      .map(v => {
        const outstanding = getOutstanding(v);
        const days = getDaysOutstanding(v);
        return [
          v.VendorName ?? '-',
          getPhone(v),
          String(days),
          getAgingBucketLabel(days),
          formatCurrency(outstanding),
          v.Status || 'Active'
        ];
      });
    
    autoTable(doc, {
      startY: yPos,
      head: [['Vendor', 'Phone', 'Days', 'Bucket', 'Outstanding', 'Status']],
      body: apData,
      theme: 'striped',
      headStyles: {
        fillColor: [220, 38, 38],
        textColor: [255, 255, 255],
        fontStyle: 'bold'
      },
      styles: { fontSize: 9, cellPadding: 2 },
      columnStyles: {
        2: { halign: 'right', cellWidth: 12 },
        3: { halign: 'center', cellWidth: 16 },
        4: { halign: 'right' },
        5: { halign: 'center', cellWidth: 18 }
      }
    });
  }
  
  addPDFFooter(doc, generatedAt);
  
  const fileName = `aging_report_${reportType}_${new Date().toISOString().split('T')[0]}.pdf`;
  doc.save(fileName);
};

// Export Financial Report PDF
export const exportFinancialReportPDF = (
  accounts: any[],
  totalRevenue: number,
  totalExpenses: number
) => {
  const doc = createA4Doc();
  const generatedAt = new Date();
  
  let yPos = addPDFHeader(doc, 'Financial Statement Report');
  yPos += 5;
  
  // Summary section
  const netPosition = totalRevenue - totalExpenses;
  
  doc.setFillColor(240, 245, 255);
  doc.rect(14, yPos, doc.internal.pageSize.width - 28, 35, 'F');
  
  doc.setFontSize(11);
  doc.setTextColor(0, 0, 0);
  doc.text('Financial Summary', 18, yPos + 8);
  
  doc.setFontSize(10);
  doc.text('Total Revenue:', 18, yPos + 16);
  doc.text(formatCurrency(totalRevenue), 70, yPos + 16);
  
  doc.text('Total Expenses:', 18, yPos + 23);
  doc.text(formatCurrency(totalExpenses), 70, yPos + 23);
  
  doc.setFont('helvetica', 'bold');
  doc.text('Net Position:', 18, yPos + 30);
  doc.setTextColor(netPosition >= 0 ? 34 : 220, netPosition >= 0 ? 197 : 38, netPosition >= 0 ? 94 : 38);
  doc.text(formatCurrency(netPosition), 70, yPos + 30);
  
  yPos += 40;
  
  // Chart of Accounts table
  doc.setFontSize(12);
  doc.setFont('helvetica', 'bold');
  doc.setTextColor(0, 0, 0);
  doc.text('Chart of Accounts', 14, yPos);
  yPos += 5;
  
  const tableData = accounts.map(acc => [
    acc.AccountID || acc.AccountName,
    acc.AccountType || 'N/A',
    formatCurrency(acc.Balance || 0),
    acc.Active ? 'Active' : 'Inactive'
  ]);
  
  autoTable(doc, {
    startY: yPos,
    head: [['Account', 'Type', 'Balance', 'Status']],
    body: tableData,
    theme: 'grid',
    headStyles: {
      fillColor: [30, 58, 138],
      textColor: [255, 255, 255],
      fontStyle: 'bold'
    },
    styles: { fontSize: 9 },
    columnStyles: {
      2: { halign: 'right' },
      3: { halign: 'center' }
    },
    didParseCell: (data) => {
      if (data.column.index === 3 && data.section === 'body') {
        const status = data.cell.raw as string;
        data.cell.styles.textColor = status === 'Active' ? [34, 197, 94] : [220, 38, 38];
      }
    }
  });
  
  addPDFFooter(doc, generatedAt);
  
  const fileName = `financial_report_${new Date().toISOString().split('T')[0]}.pdf`;
  doc.save(fileName);
};

// Export Quotation PDF (Professional A4 Format)
export const exportQuotationPDF = (
  quotationDetails: {
    vehicleNo: string;
    customerName: string;
    terms: string;
    date: string;
    quotationNo: string;
  },
  items: any[],
  options?: { 
      includeVat: boolean; 
      vatRate: number; 
      customerVatNo?: string;
      warrantyInfo?: { km: string; years: string };
  }
) => {
  const includeVat = options?.includeVat === true;
  const vatRate = Number(options?.vatRate ?? 0);
  const customerVatNo = options?.customerVatNo || '';
  const warrantyInfo = options?.warrantyInfo;

  const isTyreItem = (item: any): boolean => {
    const category = String(item?.Category ?? '').trim();
    if (!category) return false;
    return /tyre|tire/i.test(category);
  };

  const tyreCount = items.filter(isTyreItem).length;
  // If there are multiple tyre options, do not show any summed totals (misleading).
  const suppressTotals = tyreCount > 1;

  const getQty = (item: any): number => {
    const qty = Number(item?.Quantity ?? 1);
    return Number.isFinite(qty) && qty > 0 ? qty : 1;
  };

  const getUnitPrice = (item: any): number => {
    if (item?.isFOC) return 0;
    const unit = Number(item?.UnitPrice ?? item?.Price ?? 0);
    return Number.isFinite(unit) ? unit : 0;
  };

  const round2 = (n: number): number => Math.round((n + Number.EPSILON) * 100) / 100;
  // Initialize A4 Portrait
  const doc = new jsPDF({
    orientation: 'portrait',
    unit: 'mm',
    format: 'a4'
  });

  const pageWidth = doc.internal.pageSize.width;
  const pageHeight = doc.internal.pageSize.height;
  const margin = 15;
  
  // --- Header Section ---
  
  // Company Name (Left)
  doc.setFont('helvetica', 'bold');
  
  // Dynamic Company Name based on VAT status
  const companyName = includeVat ? 'LASANTHA TYRE TRADERS' : 'NEW LASANTHA TYRE TRADERS';
  
  // Auto-scale font size to prevent overlap with "QUOTATION" title
  // Available width is roughly 60% of page width to be safe
  const maxNameWidth = pageWidth * 0.6; 
  let nameFontSize = 22;
  doc.setFontSize(nameFontSize);
  
  while (doc.getTextWidth(companyName) > maxNameWidth && nameFontSize > 14) {
      nameFontSize -= 1;
      doc.setFontSize(nameFontSize);
  }
  
  doc.setTextColor(26, 54, 93); // Navy Blue
  doc.text(companyName, margin, 25);

  // Slogan
  doc.setFont('helvetica', 'italic');
  doc.setFontSize(10);
  doc.setTextColor(22, 163, 74); // Green-600
  doc.text(COMPANY_CONFIG.slogan, margin, 30);
  
  // Company Details (Left, below name)
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(9);
  doc.setTextColor(100, 116, 139); // Slate-500
  
  // Address (Split if too long)
  const addressLines = doc.splitTextToSize(COMPANY_CONFIG.address, 90);
  doc.text(addressLines, margin, 36);
  
  let detailsY = 36 + (addressLines.length * 4);
  
  // VAT Registration Number (Only for VAT Quotation)
  if (includeVat) {
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(26, 54, 93);
      doc.text('VAT Reg No: 743321219-7000', margin, detailsY);
      detailsY += 5;
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(100, 116, 139);
  }
  
  doc.text(`Hotline: ${COMPANY_CONFIG.phone} | Tel: ${COMPANY_CONFIG.landline}`, margin, detailsY);
  doc.text(`Email: ${COMPANY_CONFIG.email}`, margin, detailsY + 5);

  // "QUOTATION" Title (Right)
  doc.setFont('helvetica', 'bold');
  
  // Only show the large background text if it's a VAT quotation (shorter name)
  // For Non-VAT (longer name), we hide it to prevent clutter/overlap
  if (includeVat) {
      doc.setFontSize(32);
      doc.setTextColor(226, 232, 240); // Very light gray for background effect
      doc.text('QUOTATION', pageWidth - margin, 30, { align: 'right' });
  }
  
  doc.setFontSize(14);
  doc.setTextColor(26, 54, 93); // Navy Blue overlay
  doc.text('QUOTATION', pageWidth - margin, 30, { align: 'right' });

  // Quotation Meta (Right, below title)
  doc.setFontSize(10);
  doc.setTextColor(71, 85, 105); // Slate-600
  
  const metaX = pageWidth - margin - 40;
  const metaValX = pageWidth - margin;
  let metaY = 40;

  doc.text('Date:', metaX, metaY, { align: 'right' });
  doc.setFont('helvetica', 'bold');
  doc.text(quotationDetails.date, metaValX, metaY, { align: 'right' });
  
  metaY += 5;
  doc.setFont('helvetica', 'normal');
  doc.text('Quote #:', metaX, metaY, { align: 'right' });
  doc.setFont('helvetica', 'bold');
  doc.text(quotationDetails.quotationNo, metaValX, metaY, { align: 'right' });

  // Divider Line
  doc.setDrawColor(226, 232, 240);
  doc.setLineWidth(0.5);
  doc.line(margin, 50, pageWidth - margin, 50);

  // --- Customer & Vehicle Section ---
  let yPos = 65;

  // Box for Customer (Left)
  doc.setFillColor(248, 250, 252); // Slate-50
  doc.roundedRect(margin, 55, (pageWidth - margin * 3) / 2, 35, 2, 2, 'F');
  
  doc.setFontSize(8);
  doc.setTextColor(148, 163, 184); // Slate-400
  doc.setFont('helvetica', 'bold');
  doc.text('QUOTATION FOR', margin + 5, 62);
  
  doc.setFontSize(11);
  doc.setTextColor(15, 23, 42); // Slate-900
  doc.text(quotationDetails.customerName || 'Cash Customer', margin + 5, 70);
  
  let customerDetailsY = 76;

  if (quotationDetails.vehicleNo) {
    doc.setFontSize(9);
    doc.setTextColor(71, 85, 105);
    doc.setFont('helvetica', 'normal');
    doc.text(`Vehicle: ${quotationDetails.vehicleNo}`, margin + 5, customerDetailsY);
    customerDetailsY += 5;
  }

  // Customer VAT No (Only if provided and VAT is enabled)
  if (includeVat && customerVatNo) {
      doc.setFontSize(9);
      doc.setTextColor(71, 85, 105);
      doc.setFont('helvetica', 'normal');
      doc.text(`VAT No: ${customerVatNo}`, margin + 5, customerDetailsY);
  }

  // Box for Terms (Right)
  const rightBoxX = pageWidth / 2 + margin / 2;
  doc.setFillColor(248, 250, 252);
  doc.roundedRect(rightBoxX, 55, (pageWidth - margin * 3) / 2, 35, 2, 2, 'F');

  doc.setFontSize(8);
  doc.setTextColor(148, 163, 184);
  doc.setFont('helvetica', 'bold');
  doc.text('TERMS & VALIDITY', rightBoxX + 5, 62);

  doc.setFontSize(10);
  doc.setTextColor(15, 23, 42);
  doc.text(`Payment Terms: ${quotationDetails.terms || 'Cash'}`, rightBoxX + 5, 70);
  doc.setFont('helvetica', 'normal');
  doc.text('Valid until: 7 Days from date', rightBoxX + 5, 76);

  // --- Items Table ---
  yPos = 100;

  const tableData = items.map((item, index) => {
    const qty = getQty(item);
    const originalUnitPrice = getUnitPrice(item); // This is the VAT-inclusive price from the system
    
    const brand = item.Brand || '';
    const size = item.Size || '';
    const description = item.Description || item.ItemName || 'Item';
    const fullDescription = `${description}${brand || size ? `\n${brand} ${size}` : ''}`;

    if (!includeVat) {
      // Non-VAT case: Show original price as is
      const base = round2(qty * originalUnitPrice);
      return [
        index + 1,
        fullDescription,
        qty,
        formatCurrency(originalUnitPrice).replace('Rs. ', ''),
        formatCurrency(base).replace('Rs. ', '')
      ];
    }

    // VAT Case: Reverse Calculation
    // Formula: Base Price = Selling Price / (1 + VAT Rate/100)
    const vatMultiplier = 1 + (Number.isFinite(vatRate) ? vatRate : 0) / 100;
    const baseUnitPrice = round2(originalUnitPrice / vatMultiplier);
    
    // Calculate totals based on the derived base price
    const totalBaseAmount = round2(qty * baseUnitPrice);
    
    // Calculate VAT amount
    // Formula: VAT = Selling Price - Base Price
    // We calculate per unit first for precision, then multiply by qty
    const unitVatAmount = round2(originalUnitPrice - baseUnitPrice);
    const totalVatAmount = round2(qty * unitVatAmount);
    
    // Final Line Total should match the original selling price * qty
    // We use the original price to ensure no penny-rounding errors affect the final customer price
    const lineTotal = round2(qty * originalUnitPrice);

    return [
      index + 1,
      fullDescription,
      qty,
      formatCurrency(baseUnitPrice).replace('Rs. ', ''), // Show Excl. VAT Price
      formatCurrency(totalVatAmount).replace('Rs. ', ''), // Show Total VAT
      formatCurrency(lineTotal).replace('Rs. ', '') // Show Final Amount (Inclusive)
    ];
  });

  autoTable(doc, {
    startY: yPos,
    head: [
      includeVat
        ? ['#', 'DESCRIPTION', 'QTY', 'UNIT PRICE (Rs.)', `VAT (${vatRate}%)`, 'AMOUNT (Rs.)']
        : ['#', 'DESCRIPTION', 'QTY', 'UNIT PRICE (Rs.)', 'AMOUNT (Rs.)']
    ],
    body: tableData,
    theme: 'plain',
    headStyles: {
      fillColor: [241, 245, 249], // Slate-100
      textColor: [71, 85, 105], // Slate-600
      fontStyle: 'bold',
      fontSize: 8,
      halign: 'left',
      cellPadding: 3
    },
    styles: {
      fontSize: 9,
      cellPadding: 4,
      textColor: [51, 65, 85], // Slate-700
      lineColor: [226, 232, 240], // Slate-200
      lineWidth: 0.1,
    },
    columnStyles: includeVat
      ? {
          0: { cellWidth: 10, halign: 'center' },
          1: { cellWidth: 'auto' },
          2: { cellWidth: 12, halign: 'center' },
          3: { cellWidth: 26, halign: 'right' },
          4: { cellWidth: 22, halign: 'right' },
          5: { cellWidth: 28, halign: 'right', fontStyle: 'bold' }
        }
      : {
          0: { cellWidth: 10, halign: 'center' },
          1: { cellWidth: 'auto' }, // Description takes remaining space
          2: { cellWidth: 15, halign: 'center' },
          3: { cellWidth: 30, halign: 'right' },
          4: { cellWidth: 35, halign: 'right', fontStyle: 'bold' }
        },
    didDrawPage: (data) => {
      // Header is already drawn
    },
    margin: { left: margin, right: margin }
  });

  // --- Totals Section ---
  const finalY = (doc as any).lastAutoTable.finalY + 5;

  if (suppressTotals) {
    doc.setFontSize(10);
    doc.setFont('helvetica', 'italic');
    doc.setTextColor(100, 116, 139); // Slate-500
    doc.text('Note: Multiple tyre options included for selection.', pageWidth - margin, finalY + 5, { align: 'right' });
    doc.text('Totals are not summed. Please select your preferred option.', pageWidth - margin, finalY + 10, { align: 'right' });
  } else {
    // Calculate totals
    let subTotal = 0;
    let totalVat = 0;
    let grandTotal = 0;

    if (includeVat) {
        // For VAT mode, we sum up the reverse-calculated values
        const vatMultiplier = 1 + (Number.isFinite(vatRate) ? vatRate : 0) / 100;
        
        items.forEach(item => {
            const qty = getQty(item);
            const originalUnitPrice = getUnitPrice(item);
            
            const baseUnitPrice = round2(originalUnitPrice / vatMultiplier);
            const unitVatAmount = round2(originalUnitPrice - baseUnitPrice);
            
            subTotal += round2(qty * baseUnitPrice);
            totalVat += round2(qty * unitVatAmount);
            grandTotal += round2(qty * originalUnitPrice);
        });
    } else {
        // Non-VAT mode: Simple sum
        subTotal = round2(
            items.reduce((sum, item) => {
                const qty = getQty(item);
                const unitPrice = getUnitPrice(item);
                return sum + qty * unitPrice;
            }, 0)
        );
        grandTotal = subTotal;
    }

    let totalY = finalY;
    const rightMargin = pageWidth - margin;
    const labelX = rightMargin - 50;
    const valueX = rightMargin;

    // Subtotal
    doc.setFontSize(9);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(71, 85, 105);
    doc.text('Subtotal (Excl. VAT):', labelX, totalY, { align: 'right' });
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(15, 23, 42);
    doc.text(formatCurrency(subTotal), valueX, totalY, { align: 'right' });
    totalY += 6;

    // VAT + Total
    if (includeVat) {
      const safeRate = Number.isFinite(vatRate) ? vatRate : 0;
      
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(71, 85, 105);
      doc.text(`VAT (${safeRate}%):`, labelX, totalY, { align: 'right' });
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(15, 23, 42);
      doc.text(formatCurrency(totalVat), valueX, totalY, { align: 'right' });
      totalY += 6;
    }

    doc.setDrawColor(226, 232, 240);
    doc.line(labelX - 10, totalY - 2, valueX, totalY - 2);
    totalY += 5;

    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(26, 54, 93);
    doc.text('Total:', labelX, totalY, { align: 'right' });
    doc.text(formatCurrency(grandTotal), valueX, totalY, { align: 'right' });

    doc.setDrawColor(26, 54, 93);
    doc.setLineWidth(0.5);
    doc.line(labelX - 10, totalY + 2, valueX, totalY + 2);
    doc.line(labelX - 10, totalY + 3.5, valueX, totalY + 3.5);
  }

  // --- Warranty Section ---
  if (warrantyInfo) {
      const warrantyY = pageHeight - 45;
      // Increased height to 16mm to accommodate 2 lines comfortably
      doc.setFillColor(240, 253, 244); // Green-50
      doc.roundedRect(margin, warrantyY, pageWidth - (margin * 2), 16, 2, 2, 'F');
      
      doc.setFontSize(9);
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(22, 163, 74); // Green-600
      doc.text('WARRANTY TERMS:', margin + 5, warrantyY + 7);
      
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(21, 128, 61); // Green-700
      
      const warrantyText = `${Number(warrantyInfo.km).toLocaleString()} km or ${warrantyInfo.years} Year${warrantyInfo.years !== '1' ? 's' : ''} Warranty (Whichever comes first) against manufacturing defects. Standard warranty terms & conditions apply.`;
      
      doc.setFontSize(8);
      // Split text to fit within the box width (minus label width and margins)
      // Label width is approx 35mm, so we start at margin + 45
      const maxWidth = pageWidth - (margin * 2) - 50; 
      const splitText = doc.splitTextToSize(warrantyText, maxWidth);
      
      doc.text(splitText, margin + 45, warrantyY + 7);
  }

  // --- Footer Section ---
  const footerY = pageHeight - 30;
  
  // Thank you note
  doc.setFontSize(10);
  doc.setFont('helvetica', 'bold');
  doc.setTextColor(26, 54, 93);
  doc.text('Thank you for your business!', margin, footerY);
  
  doc.setFontSize(9);
  doc.setFont('helvetica', 'normal');
  doc.setTextColor(100, 116, 139);
  
  // Cheque Payment Instructions
  const chequePayableTo = includeVat ? 'Lasantha Tyre Traders' : 'New Lasantha Tyre Traders';
  doc.text(`Make cheques payable to: ${chequePayableTo}`, margin, footerY + 5);
  doc.text('If you have any questions about this quotation, please contact us.', margin, footerY + 10);

  // Bottom Line
  doc.setDrawColor(226, 232, 240);
  doc.line(margin, pageHeight - 15, pageWidth - margin, pageHeight - 15);

  // Page Number & Copyright & Disclaimer
  doc.setFontSize(8);
  doc.setTextColor(148, 163, 184);
  doc.text(
    `Generated by Lasantha Tire System • ${new Date().toLocaleString()}`,
    margin,
    pageHeight - 10
  );
  
  // Computer Generated Disclaimer (Right Aligned)
  doc.text(
      'This is a computer generated quotation, no signature required.',
      pageWidth - margin,
      pageHeight - 10,
      { align: 'right' }
  );
  
  // Open PDF
  try {
    const blob = doc.output('blob');
    const blobUrl = URL.createObjectURL(blob);
    window.open(blobUrl, '_blank');
    
    // Native Share (Mobile)
    if (navigator.share) {
      const file = new File([blob], `Quotation_${quotationDetails.quotationNo}.pdf`, { type: 'application/pdf' });
      if (navigator.canShare && navigator.canShare({ files: [file] })) {
        navigator.share({
          files: [file],
          title: `Quotation ${quotationDetails.quotationNo}`,
          text: `Here is the quotation for ${quotationDetails.customerName}`
        }).catch(console.error);
      }
    }
  } catch (e) {
    const fileName = `Quotation_${quotationDetails.quotationNo}.pdf`;
    doc.save(fileName);
  }
};
