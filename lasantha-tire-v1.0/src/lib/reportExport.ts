/**
 * Report Export Utilities
 * PDF, Excel, CSV export functions for Peachtree reports
 */

// PDF Export using jsPDF
export async function exportToPDF(reportData: any, reportType: string) {
  try {
    // Dynamic import to avoid SSR issues
    const jsPDF = (await import('jspdf')).default;
    const autoTable = (await import('jspdf-autotable')).default;
    
    const doc = new jsPDF();
    const pageWidth = doc.internal.pageSize.getWidth();
    
    // Company header
    doc.setFontSize(18);
    doc.setFont('helvetica', 'bold');
    doc.text('Lasantha Tyre Service', pageWidth / 2, 15, { align: 'center' });
    
    doc.setFontSize(12);
    doc.setFont('helvetica', 'normal');
    doc.text('Business Status Report', pageWidth / 2, 22, { align: 'center' });
    
    doc.setFontSize(10);
    doc.text(`Generated: ${new Date().toLocaleString('en-LK')}`, pageWidth / 2, 28, { align: 'center' });
    
    let yPos = 35;

    if (reportType === 'ar-aging' && reportData.arAging) {
      // AR Aging Report
      doc.setFontSize(14);
      doc.setFont('helvetica', 'bold');
      doc.text('Accounts Receivable Aging Report', 14, yPos);
      yPos += 10;
      
      const arData = [
        ['Category', 'Amount (LKR)'],
        ['Current', formatCurrency(reportData.arAging.current)],
        ['1-30 days', formatCurrency(reportData.arAging.days_1_30)],
        ['31-60 days', formatCurrency(reportData.arAging.days_31_60)],
        ['61-90 days', formatCurrency(reportData.arAging.days_61_90)],
        ['Over 90 days', formatCurrency(reportData.arAging.over_90)],
        ['Total Outstanding', formatCurrency(reportData.arAging.total)]
      ];
      
      autoTable(doc, {
        startY: yPos,
        head: [arData[0]],
        body: arData.slice(1),
        theme: 'grid',
        headStyles: { fillColor: [34, 197, 94] }, // Green
        styles: { fontSize: 10 }
      });
      
      yPos = (doc as any).lastAutoTable.finalY + 10;
    }

    if (reportType === 'ap-aging' && reportData.apAging) {
      // AP Aging Report
      doc.setFontSize(14);
      doc.setFont('helvetica', 'bold');
      doc.text('Accounts Payable Aging Report', 14, yPos);
      yPos += 10;
      
      const apData = [
        ['Category', 'Amount (LKR)'],
        ['Current', formatCurrency(reportData.apAging.current)],
        ['1-30 days', formatCurrency(reportData.apAging.days_1_30)],
        ['31-60 days', formatCurrency(reportData.apAging.days_31_60)],
        ['61-90 days', formatCurrency(reportData.apAging.days_61_90)],
        ['Over 90 days', formatCurrency(reportData.apAging.over_90)],
        ['Total Outstanding', formatCurrency(reportData.apAging.total)]
      ];
      
      autoTable(doc, {
        startY: yPos,
        head: [apData[0]],
        body: apData.slice(1),
        theme: 'grid',
        headStyles: { fillColor: [239, 68, 68] }, // Red
        styles: { fontSize: 10 }
      });
      
      yPos = (doc as any).lastAutoTable.finalY + 10;
    }

    if (reportType === 'cash-balances' && reportData.cashBalances) {
      // Cash Balances
      doc.setFontSize(14);
      doc.setFont('helvetica', 'bold');
      doc.text('Cash & Bank Balances', 14, yPos);
      yPos += 10;
      
      const cashData = [
        ['Account ID', 'Account Name', 'Balance (LKR)'],
        ...reportData.cashBalances.map((acc: any) => [
          acc.AccountID,
          acc.AccountName,
          formatCurrency(acc.Balance)
        ]),
        ['', 'Total', formatCurrency(reportData.totalCash)]
      ];
      
      autoTable(doc, {
        startY: yPos,
        head: [cashData[0]],
        body: cashData.slice(1),
        theme: 'grid',
        headStyles: { fillColor: [59, 130, 246] }, // Blue
        styles: { fontSize: 9 }
      });
    }

    if (reportType === 'outstanding' && reportData.outstandingReport) {
      // Outstanding Report
      doc.addPage();
      yPos = 20;
      
      doc.setFontSize(16);
      doc.setFont('helvetica', 'bold');
      doc.text('Outstanding Report', 14, yPos);
      yPos += 10;
      
      // AR Section
      if (reportData.outstandingReport.ar) {
        doc.setFontSize(12);
        doc.text('Accounts Receivable', 14, yPos);
        yPos += 7;
        
        const arCustomers = [
          ['Customer ID', 'Customer Name', 'Outstanding (LKR)'],
          ...reportData.outstandingReport.ar.customers.slice(0, 30).map((c: any) => [
            c.CustomerID,
            c.CustomerName,
            formatCurrency(c.Outstanding)
          ])
        ];
        
        autoTable(doc, {
          startY: yPos,
          head: [arCustomers[0]],
          body: arCustomers.slice(1),
          theme: 'striped',
          headStyles: { fillColor: [34, 197, 94] },
          styles: { fontSize: 8 }
        });
        
        yPos = (doc as any).lastAutoTable.finalY + 10;
        
        doc.setFontSize(10);
        doc.setFont('helvetica', 'bold');
        doc.text(`Total AR: ${formatCurrency(reportData.outstandingReport.ar.total)}`, 14, yPos);
        yPos += 10;
      }
      
      // AP Section
      if (reportData.outstandingReport.ap) {
        if (yPos > 250) {
          doc.addPage();
          yPos = 20;
        }
        
        doc.setFontSize(12);
        doc.text('Accounts Payable', 14, yPos);
        yPos += 7;
        
        const apVendors = [
          ['Vendor ID', 'Vendor Name', 'Outstanding (LKR)'],
          ...reportData.outstandingReport.ap.vendors.slice(0, 30).map((v: any) => [
            v.VendorID,
            v.VendorName,
            formatCurrency(v.Outstanding)
          ])
        ];
        
        autoTable(doc, {
          startY: yPos,
          head: [apVendors[0]],
          body: apVendors.slice(1),
          theme: 'striped',
          headStyles: { fillColor: [239, 68, 68] },
          styles: { fontSize: 8 }
        });
        
        yPos = (doc as any).lastAutoTable.finalY + 10;
        
        doc.setFontSize(10);
        doc.setFont('helvetica', 'bold');
        doc.text(`Total AP: ${formatCurrency(reportData.outstandingReport.ap.total)}`, 14, yPos);
      }
    }

    if (reportType === 'overview') {
      // Complete overview
      doc.setFontSize(14);
      doc.setFont('helvetica', 'bold');
      doc.text('Business Status Overview', 14, yPos);
      yPos += 10;
      
      const summaryData = [
        ['Metric', 'Amount (LKR)'],
        ['Total AR', formatCurrency(reportData.arAging?.total || 0)],
        ['Total AP', formatCurrency(reportData.apAging?.total || 0)],
        ['Net Position', formatCurrency((reportData.arAging?.total || 0) - (reportData.apAging?.total || 0))],
        ['Total Cash', formatCurrency(reportData.totalCash || 0)]
      ];
      
      autoTable(doc, {
        startY: yPos,
        head: [summaryData[0]],
        body: summaryData.slice(1),
        theme: 'grid',
        headStyles: { fillColor: [124, 58, 237] }, // Purple
        styles: { fontSize: 11, fontStyle: 'bold' }
      });
    }
    
    // Save PDF
    const fileName = `${reportType}_report_${Date.now()}.pdf`;
    doc.save(fileName);
    
    return { success: true, fileName };
    
  } catch (error) {
    console.error('PDF Export Error:', error);
    return { success: false, error: String(error) };
  }
}

// Excel Export using xlsx
export async function exportToExcel(reportData: any, reportType: string) {
  try {
    const XLSX = await import('xlsx');
    
    const workbook = XLSX.utils.book_new();
    
    // AR Aging Sheet
    if (reportData.arAging) {
      const arData = [
        ['Accounts Receivable Aging Report'],
        ['Generated:', new Date().toLocaleString('en-LK')],
        [],
        ['Category', 'Amount (LKR)'],
        ['Current', reportData.arAging.current],
        ['1-30 days', reportData.arAging.days_1_30],
        ['31-60 days', reportData.arAging.days_31_60],
        ['61-90 days', reportData.arAging.days_61_90],
        ['Over 90 days', reportData.arAging.over_90],
        ['Total', reportData.arAging.total]
      ];
      
      const arSheet = XLSX.utils.aoa_to_sheet(arData);
      XLSX.utils.book_append_sheet(workbook, arSheet, 'AR Aging');
    }
    
    // AP Aging Sheet
    if (reportData.apAging) {
      const apData = [
        ['Accounts Payable Aging Report'],
        ['Generated:', new Date().toLocaleString('en-LK')],
        [],
        ['Category', 'Amount (LKR)'],
        ['Current', reportData.apAging.current],
        ['1-30 days', reportData.apAging.days_1_30],
        ['31-60 days', reportData.apAging.days_31_60],
        ['61-90 days', reportData.apAging.days_61_90],
        ['Over 90 days', reportData.apAging.over_90],
        ['Total', reportData.apAging.total]
      ];
      
      const apSheet = XLSX.utils.aoa_to_sheet(apData);
      XLSX.utils.book_append_sheet(workbook, apSheet, 'AP Aging');
    }
    
    // Cash Balances Sheet
    if (reportData.cashBalances && reportData.cashBalances.length > 0) {
      const cashData = [
        ['Cash & Bank Balances'],
        ['Generated:', new Date().toLocaleString('en-LK')],
        [],
        ['Account ID', 'Account Name', 'Account Type', 'Balance (LKR)'],
        ...reportData.cashBalances.map((acc: any) => [
          acc.AccountID,
          acc.AccountName,
          acc.AccountType,
          acc.Balance
        ]),
        [],
        ['Total', '', '', reportData.totalCash]
      ];
      
      const cashSheet = XLSX.utils.aoa_to_sheet(cashData);
      XLSX.utils.book_append_sheet(workbook, cashSheet, 'Cash Balances');
    }
    
    // Top Customers Sheet
    if (reportData.topCustomers && reportData.topCustomers.length > 0) {
      const customerData = [
        ['Top Customers'],
        ['Generated:', new Date().toLocaleString('en-LK')],
        [],
        ['Rank', 'Customer ID', 'Customer Name', 'Balance (LKR)', 'Credit Limit'],
        ...reportData.topCustomers.map((c: any, i: number) => [
          i + 1,
          c.CustomerID,
          c.CustomerName,
          c.CurrentBalance,
          c.CreditLimit
        ])
      ];
      
      const customerSheet = XLSX.utils.aoa_to_sheet(customerData);
      XLSX.utils.book_append_sheet(workbook, customerSheet, 'Top Customers');
    }
    
    // Outstanding Report Sheet
    if (reportData.outstandingReport) {
      const outData = [
        ['Outstanding Report'],
        ['Generated:', new Date().toLocaleString('en-LK')],
        [],
        ['Summary'],
        ['Total AR:', reportData.outstandingReport.ar?.total || 0],
        ['Total AP:', reportData.outstandingReport.ap?.total || 0],
        ['Net Position:', (reportData.outstandingReport.ar?.total || 0) - (reportData.outstandingReport.ap?.total || 0)],
        [],
        ['AR Details - Customers with Outstanding Balance'],
        ['Customer ID', 'Customer Name', 'Outstanding (LKR)'],
        ...(reportData.outstandingReport.ar?.customers || []).map((c: any) => [
          c.CustomerID,
          c.CustomerName,
          c.Outstanding
        ]),
        [],
        ['AP Details - Vendors with Outstanding Balance'],
        ['Vendor ID', 'Vendor Name', 'Outstanding (LKR)'],
        ...(reportData.outstandingReport.ap?.vendors || []).map((v: any) => [
          v.VendorID,
          v.VendorName,
          v.Outstanding
        ])
      ];
      
      const outSheet = XLSX.utils.aoa_to_sheet(outData);
      XLSX.utils.book_append_sheet(workbook, outSheet, 'Outstanding');
    }
    
    // Write file
    const fileName = `${reportType}_report_${Date.now()}.xlsx`;
    XLSX.writeFile(workbook, fileName);
    
    return { success: true, fileName };
    
  } catch (error) {
    console.error('Excel Export Error:', error);
    return { success: false, error: String(error) };
  }
}

// CSV Export (simple)
export function exportToCSV(data: any[], filename: string) {
  try {
    if (!data || data.length === 0) {
      throw new Error('No data to export');
    }
    
    const headers = Object.keys(data[0]);
    const csvRows = [];
    
    // Add headers
    csvRows.push(headers.join(','));
    
    // Add data rows
    for (const row of data) {
      const values = headers.map(header => {
        const val = row[header];
        // Escape commas and quotes
        const escaped = String(val).replace(/"/g, '""');
        return `"${escaped}"`;
      });
      csvRows.push(values.join(','));
    }
    
    const csvString = csvRows.join('\n');
    const blob = new Blob([csvString], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${filename}_${Date.now()}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
    
    return { success: true, filename: a.download };
    
  } catch (error) {
    console.error('CSV Export Error:', error);
    return { success: false, error: String(error) };
  }
}

// Helper function
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-LK', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount || 0);
}

// Email report (requires backend API)
export async function emailReport(reportData: any, reportType: string, recipientEmail: string) {
  try {
    // First generate PDF
    const pdfResult = await exportToPDF(reportData, reportType);
    
    if (!pdfResult.success) {
      throw new Error('Failed to generate PDF');
    }
    
    // Send via API (you'll need to implement this endpoint)
    const response = await fetch('/api/email-report', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        reportType,
        recipientEmail,
        fileName: pdfResult.fileName,
        reportData
      })
    });
    
    if (!response.ok) {
      throw new Error('Failed to send email');
    }
    
    return { success: true, message: 'Report emailed successfully' };
    
  } catch (error) {
    console.error('Email Report Error:', error);
    return { success: false, error: String(error) };
  }
}
