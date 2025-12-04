import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';

// Company branding configuration
const COMPANY_CONFIG = {
  name: 'Lasantha Tire Company',
  address: 'Colombo, Sri Lanka',
  phone: '+94 11 234 5678',
  email: 'info@lasanthatire.lk',
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
  return `Rs. ${amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
};

// Export Invoice Details PDF
export const exportInvoiceDetailsPDF = (
  invoices: any[],
  customerOrVendor: any,
  type: 'customer' | 'vendor'
) => {
  const doc = new jsPDF();
  
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
  const pageCount = (doc as any).internal.getNumberOfPages();
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(148, 163, 184);
    doc.text(
      `Page ${i} of ${pageCount} - Generated on ${new Date().toLocaleString()}`,
      doc.internal.pageSize.width / 2,
      doc.internal.pageSize.height - 10,
      { align: 'center' }
    );
  }
  
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
  reportType: 'ar' | 'ap' | 'both'
) => {
  const doc = new jsPDF();
  
  const title = reportType === 'ar' ? 'Accounts Receivable Aging Report' :
                reportType === 'ap' ? 'Accounts Payable Aging Report' :
                'AR/AP Aging Report';
  
  let yPos = addPDFHeader(doc, title);
  yPos += 5;
  
  // Summary boxes
  if (reportType === 'ar' || reportType === 'both') {
    const arTotal = customers.reduce((sum, c) => sum + (c.Balance || 0), 0);
    const arCount = customers.filter(c => (c.Balance || 0) > 0).length;
    
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
    const apTotal = vendors.reduce((sum, v) => sum + (v.Balance || 0), 0);
    const apCount = vendors.filter(v => (v.Balance || 0) > 0).length;
    
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
  
  // AR Table
  if (reportType === 'ar' || reportType === 'both') {
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text('Receivables Details', 14, yPos);
    yPos += 5;
    
    const arData = customers
      .filter(c => (c.Balance || 0) > 0)
      .map(c => [
        c.CustomerName,
        c.Contact || '-',
        formatCurrency(c.Balance || 0),
        formatCurrency(c.CreditLimit || 0),
        c.CreditLimit > 0 ? `${((c.Balance / c.CreditLimit) * 100).toFixed(1)}%` : '-'
      ]);
    
    autoTable(doc, {
      startY: yPos,
      head: [['Customer', 'Contact', 'Balance', 'Credit Limit', 'Utilization']],
      body: arData,
      theme: 'striped',
      headStyles: {
        fillColor: [34, 197, 94],
        textColor: [255, 255, 255],
        fontStyle: 'bold'
      },
      styles: { fontSize: 9 },
      columnStyles: {
        2: { halign: 'right' },
        3: { halign: 'right' },
        4: { halign: 'center' }
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
    
    const apData = vendors
      .filter(v => (v.Balance || 0) > 0)
      .map(v => [
        v.VendorName,
        v.Contact || '-',
        formatCurrency(v.Balance || 0),
        v.Status || 'Active'
      ]);
    
    autoTable(doc, {
      startY: yPos,
      head: [['Vendor', 'Contact', 'Balance', 'Status']],
      body: apData,
      theme: 'striped',
      headStyles: {
        fillColor: [220, 38, 38],
        textColor: [255, 255, 255],
        fontStyle: 'bold'
      },
      styles: { fontSize: 9 },
      columnStyles: {
        2: { halign: 'right' },
        3: { halign: 'center' }
      }
    });
  }
  
  // Footer
  const pageCount = (doc as any).internal.getNumberOfPages();
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(150, 150, 150);
    doc.text(
      `Page ${i} of ${pageCount}`,
      doc.internal.pageSize.width / 2,
      doc.internal.pageSize.height - 10,
      { align: 'center' }
    );
  }
  
  const fileName = `aging_report_${reportType}_${new Date().toISOString().split('T')[0]}.pdf`;
  doc.save(fileName);
};

// Export Financial Report PDF
export const exportFinancialReportPDF = (
  accounts: any[],
  totalRevenue: number,
  totalExpenses: number
) => {
  const doc = new jsPDF();
  
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
  
  // Footer
  const pageCount = (doc as any).internal.getNumberOfPages();
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(150, 150, 150);
    doc.text(
      `Page ${i} of ${pageCount}`,
      doc.internal.pageSize.width / 2,
      doc.internal.pageSize.height - 10,
      { align: 'center' }
    );
  }
  
  const fileName = `financial_report_${new Date().toISOString().split('T')[0]}.pdf`;
  doc.save(fileName);
};

// Export Quotation PDF
export const exportQuotationPDF = (
  quotationDetails: {
    vehicleNo: string;
    customerName: string;
    terms: string;
    date: string;
    quotationNo: string;
  },
  items: any[]
) => {
  const doc = new jsPDF();
  
  // 1. Header
  let yPos = addPDFHeader(doc, 'Quotation', `Ref: ${quotationDetails.quotationNo}`);
  yPos += 5;

  // 2. Customer & Vehicle Details Box
  doc.setFillColor(248, 250, 252); // Slate-50
  doc.setDrawColor(226, 232, 240); // Slate-200
  doc.roundedRect(14, yPos, doc.internal.pageSize.width - 28, 35, 3, 3, 'FD');

  // Left Side: Customer
  doc.setFontSize(10);
  doc.setTextColor(100, 116, 139); // Slate-500
  doc.text('Customer Details:', 18, yPos + 8);
  
  doc.setFontSize(11);
  doc.setTextColor(15, 23, 42); // Slate-900
  doc.setFont('helvetica', 'bold');
  doc.text(quotationDetails.customerName || 'Cash Customer', 18, yPos + 15);
  
  // Right Side: Vehicle & Terms
  doc.setFontSize(10);
  doc.setFont('helvetica', 'normal');
  doc.setTextColor(100, 116, 139);
  doc.text('Vehicle No:', 110, yPos + 8);
  doc.text('Terms:', 110, yPos + 15);
  doc.text('Date:', 110, yPos + 22);

  doc.setTextColor(15, 23, 42);
  doc.setFont('helvetica', 'bold');
  doc.text(quotationDetails.vehicleNo || '-', 140, yPos + 8);
  doc.text(quotationDetails.terms || 'Cash', 140, yPos + 15);
  doc.text(quotationDetails.date, 140, yPos + 22);

  yPos += 45;

  // 3. Items Table
  const tableData = items.map((item, index) => [
    index + 1,
    item.Description || item.ItemName || 'Item',
    item.Size || '-',
    item.Brand || '-',
    item.Quantity || 1,
    formatCurrency(item.UnitPrice || item.Price || 0),
    formatCurrency((item.Quantity || 1) * (item.UnitPrice || item.Price || 0))
  ]);

  autoTable(doc, {
    startY: yPos,
    head: [['#', 'Description', 'Size', 'Brand', 'Qty', 'Unit Price', 'Amount']],
    body: tableData,
    theme: 'grid',
    headStyles: {
      fillColor: [30, 58, 138], // Blue-900
      textColor: [255, 255, 255],
      fontStyle: 'bold',
      halign: 'center'
    },
    styles: {
      fontSize: 9,
      cellPadding: 4,
      textColor: [51, 65, 85]
    },
    columnStyles: {
      0: { cellWidth: 10, halign: 'center' },
      4: { cellWidth: 15, halign: 'center' },
      5: { cellWidth: 30, halign: 'right' },
      6: { cellWidth: 35, halign: 'right', fontStyle: 'bold' }
    },
    alternateRowStyles: {
      fillColor: [248, 250, 252]
    }
  });

  // 4. Totals
  const totalAmount = items.reduce((sum, item) => sum + ((item.Quantity || 1) * (item.UnitPrice || item.Price || 0)), 0);
  yPos = (doc as any).lastAutoTable.finalY + 10;

  // Draw Total Box
  const boxWidth = 80;
  const boxX = doc.internal.pageSize.width - 14 - boxWidth;
  
  doc.setFillColor(240, 253, 244); // Green-50
  doc.roundedRect(boxX, yPos, boxWidth, 25, 3, 3, 'F');
  
  doc.setFontSize(12);
  doc.setTextColor(15, 23, 42);
  doc.setFont('helvetica', 'bold');
  doc.text('Total Amount:', boxX + 5, yPos + 16);
  
  doc.setFontSize(14);
  doc.setTextColor(22, 163, 74); // Green-600
  doc.text(formatCurrency(totalAmount), doc.internal.pageSize.width - 20, yPos + 16, { align: 'right' });

  // 5. Footer / Terms
  yPos += 40;
  doc.setFontSize(8);
  doc.setTextColor(100, 116, 139);
  doc.setFont('helvetica', 'normal');
  doc.text('Terms & Conditions:', 14, yPos);
  doc.text('1. This quotation is valid for 7 days.', 14, yPos + 5);
  doc.text('2. Goods once sold cannot be returned.', 14, yPos + 10);
  doc.text('3. Warranty applicable as per manufacturer policy.', 14, yPos + 15);

  // 6. Open/Share
  try {
    const blob = doc.output('blob');
    const blobUrl = URL.createObjectURL(blob);
    window.open(blobUrl, '_blank');
    
    // Also try to trigger native share if available (mobile)
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
