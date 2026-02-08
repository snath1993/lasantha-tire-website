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
  
  const title = type === 'customer' 
    ? `Customer Outstanding Report`
    : `Vendor Outstanding Report`;
  
  const entityName = type === 'customer'
    ? customerOrVendor.CustomerName
    : customerOrVendor.VendorName;
  
  const subtitle = `${entityName} (${customerOrVendor[type === 'customer' ? 'CustomerID' : 'VendorID']})`;
  
  let yPos = addPDFHeader(doc, title, subtitle);
  yPos += 5;
  
  // Entity details box
  doc.setFillColor(240, 245, 255);
  doc.rect(14, yPos, doc.internal.pageSize.width - 28, 25, 'F');
  
  doc.setFontSize(10);
  doc.setTextColor(0, 0, 0);
  doc.text(`Contact: ${customerOrVendor.Contact || 'N/A'}`, 18, yPos + 8);
  doc.text(`Phone: ${customerOrVendor.Phone || 'N/A'}`, 18, yPos + 15);
  
  const balance = customerOrVendor.Balance || 0;
  doc.setFontSize(11);
  doc.setFont('helvetica', 'bold');
  doc.text(`Total Outstanding: ${formatCurrency(balance)}`, 18, yPos + 22);
  
  if (type === 'customer' && customerOrVendor.CreditLimit) {
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(9);
    doc.text(`Credit Limit: ${formatCurrency(customerOrVendor.CreditLimit)}`, doc.internal.pageSize.width - 18, yPos + 8, { align: 'right' });
    
    const utilization = ((balance / customerOrVendor.CreditLimit) * 100).toFixed(1);
    const utilizationColor: [number, number, number] = parseFloat(utilization) > 90 ? [220, 38, 38] : 
                            parseFloat(utilization) > 70 ? [234, 179, 8] : [34, 197, 94];
    doc.setTextColor(utilizationColor[0], utilizationColor[1], utilizationColor[2]);
    doc.text(`Utilization: ${utilization}%`, doc.internal.pageSize.width - 18, yPos + 15, { align: 'right' });
  }
  
  yPos += 30;
  
  // Invoice/Transaction table
  const tableData = invoices.map(inv => [
    inv.InvoiceID || inv.TransactionID || 'N/A',
    inv.Date || 'N/A',
    inv.Type || 'Invoice',
    inv.Description || '-',
    formatCurrency(Math.abs(inv.Amount || 0)),
    inv.Status || 'Unpaid',
    inv.DueDate || '-'
  ]);
  
  autoTable(doc, {
    startY: yPos,
    head: [['Invoice #', 'Date', 'Type', 'Description', 'Amount', 'Status', 'Due Date']],
    body: tableData,
    theme: 'grid',
    headStyles: {
      fillColor: [30, 58, 138],
      textColor: [255, 255, 255],
      fontStyle: 'bold',
      fontSize: 10
    },
    styles: {
      fontSize: 9,
      cellPadding: 4
    },
    columnStyles: {
      0: { cellWidth: 25 },
      1: { cellWidth: 22 },
      2: { cellWidth: 20 },
      3: { cellWidth: 45 },
      4: { cellWidth: 25, halign: 'right' },
      5: { cellWidth: 20, halign: 'center' },
      6: { cellWidth: 22, halign: 'center' }
    },
    didParseCell: (data) => {
      if (data.column.index === 5 && data.section === 'body') {
        const status = data.cell.raw as string;
        if (status === 'Paid') {
          data.cell.styles.textColor = [34, 197, 94]; // Green
        } else if (status === 'Overdue') {
          data.cell.styles.textColor = [220, 38, 38]; // Red
        }
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
    doc.text(
      'Powered by Peachtree ERP Dashboard',
      doc.internal.pageSize.width - 14,
      doc.internal.pageSize.height - 10,
      { align: 'right' }
    );
  }
  
  // Save
  const fileName = `${type}_outstanding_${customerOrVendor[type === 'customer' ? 'CustomerID' : 'VendorID']}_${new Date().toISOString().split('T')[0]}.pdf`;
  doc.save(fileName);
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
