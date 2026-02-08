type SalesReportResponse = {
  success: boolean;
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
  paymentBreakdown?: Array<{ name: string; sales: number; count: number }>;
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

function safeNumber(val: any): number {
  const n = Number(val);
  return Number.isFinite(n) ? n : 0;
}

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-LK', {
    style: 'currency',
    currency: 'LKR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount);
}

function formatPercent(value: number): string {
  if (!Number.isFinite(value)) return '0%';
  return `${value.toFixed(1)}%`;
}

async function generateDoc(data: SalesReportResponse) {
  const jsPDF = (await import('jspdf')).default;
  const autoTable = (await import('jspdf-autotable')).default;

  const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

  const pageWidth = doc.internal.pageSize.getWidth();

  // Header
  doc.setFontSize(18);
  doc.setFont('helvetica', 'bold');
  doc.text('Lasantha Tyre Traders', 14, 16);

  doc.setFontSize(12);
  doc.setFont('helvetica', 'normal');
  doc.text('Sales Report', 14, 23);

  const start = new Date(data.filters.start);
  const end = new Date(data.filters.end);
  const generated = new Date();

  doc.setFontSize(9);
  doc.setTextColor(90);
  doc.text(`Range: ${start.toLocaleDateString('en-GB')} - ${end.toLocaleDateString('en-GB')}`, 14, 29);
  doc.text(`Generated: ${generated.toLocaleString('en-LK')}`, pageWidth - 14, 16, { align: 'right' });

  const filterLine = [
    data.filters.category ? `Category: ${data.filters.category}` : null,
    data.filters.brand ? `Brand: ${data.filters.brand}` : null,
    data.filters.search ? `Search: ${data.filters.search}` : null
  ].filter(Boolean).join(' | ');

  if (filterLine) {
    doc.setFontSize(8);
    doc.setTextColor(110);
    doc.text(filterLine, 14, 34);
  }

  doc.setDrawColor(220);
  doc.line(14, 37, pageWidth - 14, 37);

  const summary = data.summary || {
    TotalSales: 0,
    TotalQty: 0,
    TotalCost: 0,
    TotalProfit: 0,
    InvoiceCount: 0
  };

  const prev = data.prevSummary || {
    TotalSales: 0,
    TotalQty: 0,
    TotalCost: 0,
    TotalProfit: 0,
    InvoiceCount: 0
  };

  const totalSales = safeNumber(summary.TotalSales);
  const totalProfit = safeNumber(summary.TotalProfit);
  const totalQty = safeNumber(summary.TotalQty);
  const invoiceCount = safeNumber(summary.InvoiceCount);

  const prevSales = safeNumber(prev.TotalSales);
  const prevProfit = safeNumber(prev.TotalProfit);

  const margin = totalSales > 0 ? (totalProfit / totalSales) * 100 : 0;
  const salesChange = prevSales > 0 ? ((totalSales - prevSales) / prevSales) * 100 : 0;
  const profitChange = prevProfit > 0 ? ((totalProfit - prevProfit) / prevProfit) * 100 : 0;

  // Summary table
  autoTable(doc, {
    startY: 42,
    head: [['Metric', 'Value', 'Vs Previous Period']],
    body: [
      ['Total Revenue', formatCurrency(totalSales), formatPercent(salesChange)],
      ['Gross Profit', formatCurrency(totalProfit), formatPercent(profitChange)],
      ['Profit Margin', formatPercent(margin), ''],
      ['Units Sold', totalQty.toLocaleString('en-US'), ''],
      ['Invoice Count', invoiceCount.toLocaleString('en-US'), '']
    ],
    theme: 'grid',
    styles: { fontSize: 9 },
    headStyles: { fillColor: [124, 58, 237], textColor: [255, 255, 255], fontStyle: 'bold' },
    columnStyles: {
      1: { halign: 'right' },
      2: { halign: 'right' }
    }
  });

  let yPos = (doc as any).lastAutoTable?.finalY ? (doc as any).lastAutoTable.finalY + 8 : 120;

  // Category breakdown
  doc.setTextColor(0);
  doc.setFontSize(12);
  doc.setFont('helvetica', 'bold');
  doc.text('Category Breakdown', 14, yPos);
  yPos += 4;

  let catTotalQty = 0;
  let catTotalSales = 0;
  let catTotalProfit = 0;

  const categoryRows = (data.categoryBreakdown || []).slice(0, 30).map((r) => {
    const s = safeNumber(r.sales);
    const p = safeNumber(r.profit);
    const q = safeNumber(r.quantity);
    const m = s > 0 ? (p / s) * 100 : 0;

    catTotalQty += q;
    catTotalSales += s;
    catTotalProfit += p;

    return [r.name || 'Unknown', q.toLocaleString('en-US'), formatCurrency(s), formatCurrency(p), formatPercent(m)];
  });

  // Add Total Row for Category
  const catTotalMargin = catTotalSales > 0 ? (catTotalProfit / catTotalSales) * 100 : 0;
  categoryRows.push([
    'TOTAL', 
    catTotalQty.toLocaleString('en-US'), 
    formatCurrency(catTotalSales), 
    formatCurrency(catTotalProfit), 
    formatPercent(catTotalMargin)
  ]);

  autoTable(doc, {
    startY: yPos,
    head: [['Category', 'Qty', 'Sales', 'Profit', 'Margin']],
    body: categoryRows,
    theme: 'striped',
    styles: { fontSize: 8 },
    headStyles: { fillColor: [15, 23, 42], textColor: [255, 255, 255], fontStyle: 'bold' },
    columnStyles: {
      1: { halign: 'right' },
      2: { halign: 'right' },
      3: { halign: 'right' },
      4: { halign: 'right' }
    },
    didParseCell: (data) => {
      if (data.row.index === categoryRows.length - 1) {
        data.cell.styles.fontStyle = 'bold';
        data.cell.styles.fillColor = [240, 240, 240]; 
      }
    }
  });

  yPos = (doc as any).lastAutoTable.finalY + 8;

  // Brand breakdown
  if (yPos > doc.internal.pageSize.getHeight() - 80) {
    doc.addPage();
    yPos = 20;
  }

  doc.setFontSize(12);
  doc.setFont('helvetica', 'bold');
  doc.text('Brand Breakdown', 14, yPos);
  yPos += 4;

  let brandTotalQty = 0;
  let brandTotalSales = 0;
  let brandTotalProfit = 0;

  const brandRows = (data.brandBreakdown || []).slice(0, 30).map((r) => {
    const s = safeNumber(r.sales);
    const p = safeNumber(r.profit);
    const q = safeNumber(r.quantity);
    const m = s > 0 ? (p / s) * 100 : 0;

    brandTotalQty += q;
    brandTotalSales += s;
    brandTotalProfit += p;

    return [r.name || 'Unknown', q.toLocaleString('en-US'), formatCurrency(s), formatCurrency(p), formatPercent(m)];
  });

  // Add Total Row for Brand
  const brandTotalMargin = brandTotalSales > 0 ? (brandTotalProfit / brandTotalSales) * 100 : 0;
  brandRows.push([
    'TOTAL', 
    brandTotalQty.toLocaleString('en-US'), 
    formatCurrency(brandTotalSales), 
    formatCurrency(brandTotalProfit), 
    formatPercent(brandTotalMargin)
  ]);

  autoTable(doc, {
    startY: yPos,
    head: [['Brand', 'Qty', 'Sales', 'Profit', 'Margin']],
    body: brandRows,
    theme: 'striped',
    styles: { fontSize: 8 },
    headStyles: { fillColor: [30, 64, 175], textColor: [255, 255, 255], fontStyle: 'bold' },
    columnStyles: {
      1: { halign: 'right' },
      2: { halign: 'right' },
      3: { halign: 'right' },
      4: { halign: 'right' }
    },
    didParseCell: (data) => {
      if (data.row.index === brandRows.length - 1) {
        data.cell.styles.fontStyle = 'bold';
        data.cell.styles.fillColor = [240, 240, 240];
      }
    }
  });

  yPos = (doc as any).lastAutoTable.finalY + 8;

  // Payment Method breakdown
  if (yPos > doc.internal.pageSize.getHeight() - 80) {
    doc.addPage();
    yPos = 20;
  }

  doc.setFontSize(12);
  doc.setFont('helvetica', 'bold');
  doc.text('Payment Method Breakdown', 14, yPos);
  yPos += 4;

  let payTotalSales = 0;
  let payTotalCount = 0;

  const paymentRows = (data.paymentBreakdown || []).map((r) => {
    const s = safeNumber(r.sales);
    const c = safeNumber(r.count);
    
    payTotalSales += s;
    payTotalCount += c;

    return [r.name || 'Unknown', formatCurrency(s), c.toLocaleString('en-US')];
  });

  // Add Total Row for Payment Method
  paymentRows.push([
    'TOTAL',
    formatCurrency(payTotalSales),
    payTotalCount.toLocaleString('en-US')
  ]);

  autoTable(doc, {
    startY: yPos,
    head: [['Payment Method', 'Sales', 'Invoice Count']],
    body: paymentRows,
    theme: 'striped',
    styles: { fontSize: 8 },
    headStyles: { fillColor: [22, 163, 74], textColor: [255, 255, 255], fontStyle: 'bold' },
    columnStyles: {
      1: { halign: 'right' },
      2: { halign: 'right' }
    },
    didParseCell: (data) => {
      if (data.row.index === paymentRows.length - 1) {
        data.cell.styles.fontStyle = 'bold';
        data.cell.styles.fillColor = [240, 240, 240];
      }
    }
  });

  yPos = (doc as any).lastAutoTable.finalY + 8;

  // Top items
  if (yPos > doc.internal.pageSize.getHeight() - 90) {
    doc.addPage();
    yPos = 20;
  }

  doc.setFontSize(12);
  doc.setFont('helvetica', 'bold');
  doc.text('Top Items (By Sales)', 14, yPos);
  yPos += 4;

  const itemRows = (data.topItems || []).slice(0, 50).map((r, idx) => {
    const s = safeNumber(r.sales);
    const p = safeNumber(r.profit);
    return [
      String(idx + 1),
      (r.name || '').toString().slice(0, 55),
      r.brand || 'Unknown',
      r.category || 'Unknown',
      safeNumber(r.quantity).toLocaleString('en-US'),
      formatCurrency(s),
      formatCurrency(p)
    ];
  });

  autoTable(doc, {
    startY: yPos,
    head: [['#', 'Item', 'Brand', 'Category', 'Qty', 'Sales', 'Profit']],
    body: itemRows,
    theme: 'grid',
    styles: { fontSize: 7, cellPadding: 2 },
    headStyles: { fillColor: [124, 58, 237], textColor: [255, 255, 255], fontStyle: 'bold' },
    columnStyles: {
      0: { cellWidth: 8, halign: 'center' },
      4: { cellWidth: 14, halign: 'right' },
      5: { cellWidth: 22, halign: 'right' },
      6: { cellWidth: 22, halign: 'right' }
    }
  });

  // Footer page numbers
  const pageCount = (doc as any).internal.getNumberOfPages();
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(140);
    doc.text(`Page ${i} of ${pageCount}`, pageWidth / 2, doc.internal.pageSize.getHeight() - 8, { align: 'center' });
  }

  return doc;
}

export async function exportSalesReportToPDF(data: SalesReportResponse) {
  try {
    const doc = await generateDoc(data);
    const start = new Date(data.filters.start);
    const end = new Date(data.filters.end);
    const fileName = `sales_report_${start.toISOString().slice(0, 10)}_to_${end.toISOString().slice(0, 10)}.pdf`;

    // Try to open in new tab (mobile-friendly), fallback to save
    try {
      const blob = doc.output('blob');
      const blobUrl = URL.createObjectURL(blob);
      window.open(blobUrl, '_blank');
    } catch {
      doc.save(fileName);
    }

    return { success: true, fileName };
  } catch (error) {
    console.error('Sales Report PDF Export Error:', error);
    return { success: false, error: String(error) };
  }
}

export async function getSalesReportBase64(data: SalesReportResponse) {
  try {
    const doc = await generateDoc(data);
    const start = new Date(data.filters.start);
    const end = new Date(data.filters.end);
    const fileName = `sales_report_${start.toISOString().slice(0, 10)}_to_${end.toISOString().slice(0, 10)}.pdf`;
    
    const dataUri = doc.output('datauristring');
    const base64 = dataUri.split(',')[1];
    
    return { success: true, base64, fileName };
  } catch (error) {
    console.error('Sales Report PDF Base64 Error:', error);
    return { success: false, error: String(error) };
  }
}
