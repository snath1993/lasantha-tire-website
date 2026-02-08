// MonthlySalesPDFReportJob.js
// Generates professional PDF monthly sales reports with categories and charts
const moment = require('moment');
const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const Chart = require('chart.js/auto');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');
// BAILEYS MIGRATION: Use wrapper instead of whatsapp-web.js
const { MessageMedia } = require('../utils/baileysWrapper');

// Chart configuration
const chartCallback = (ChartJS) => {
    ChartJS.defaults.responsive = true;
    ChartJS.defaults.maintainAspectRatio = false;
};

const chartJSNodeCanvas = new ChartJSNodeCanvas({
    width: 600,
    height: 300,
    chartCallback
});

// Define the page configuration
const pageConfig = {
    margin: 50,
    width: 495,  // A4 width (595) - 2*margin
    maxY: 750,   // A4 height (842) - margin for footer
    fonts: {
        regular: 'Helvetica',
        bold: 'Helvetica-Bold',
        italic: 'Helvetica-Oblique'
    },
    colors: {
        primary: '#2F528F',     // Dark blue
        secondary: '#4472C4',   // Medium blue
        accent: '#70AD47',      // Green
        text: '#333333',        // Dark gray
        subtext: '#666666',     // Medium gray
        background: '#FFFFFF',   // White
        altBackground: '#F8F9FA' // Light gray
    }
};

// Define the table configuration
const tableConfig = {
    headers: ['Item Description', 'Quantity', 'Unit Price', 'Total', 'Profit'],
    columnWidths: [245, 60, 65, 65, 60], // Total = 495 (page width)
    rowHeight: 25,
    fontSize: {
        header: 10,
        data: 9,
        total: 10,
        summary: 11,
        date: 10
    },
    colors: {
        header: pageConfig.colors.secondary,
        headerText: pageConfig.colors.background,
        row: pageConfig.colors.background,
        altRow: pageConfig.colors.altBackground,
        date: pageConfig.colors.primary,
        total: pageConfig.colors.primary,
        text: pageConfig.colors.text,
        border: pageConfig.colors.secondary,
        totalBg: pageConfig.colors.altBackground,
        totalBorder: pageConfig.colors.secondary
    },
    alignment: {
        left: ['Item Description'],
        right: ['Quantity', 'Unit Price', 'Total', 'Profit']
    }
};

/**
 * Generate a chart for the sales data
 */
async function generateSalesChart(salesData) {
    console.log('Generating sales chart...');
    try {
        // Process data for chart - combine data from all days
        const categoryData = salesData.reduce((acc, dayGroup) => {
            dayGroup.items.forEach(sale => {
                const category = sale.category || 'TYRES';
                if (!acc[category]) {
                    acc[category] = {
                        quantity: 0,
                        revenue: 0,
                        profit: 0
                    };
                }
                acc[category].quantity += sale.quantity;
                acc[category].revenue += sale.totalAmount;
                acc[category].profit += sale.profit;
            });
            return acc;
        }, {});

        console.log('Chart data by category:', Object.keys(categoryData));

        // Sort categories by revenue for better visualization
        const sortedCategories = Object.entries(categoryData)
            .sort((a, b) => b[1].revenue - a[1].revenue);

        const chartConfig = {
            type: 'bar',
            data: {
                labels: sortedCategories.map(([cat]) => cat),
                datasets: [
                    {
                        label: 'Revenue',
                        data: sortedCategories.map(([_, data]) => data.revenue),
                        backgroundColor: 'rgba(68, 114, 196, 0.8)',
                        borderColor: '#2F528F',
                        borderWidth: 2,
                        borderRadius: 4,
                        barThickness: 32
                    },
                    {
                        label: 'Profit',
                        data: sortedCategories.map(([_, data]) => data.profit),
                        backgroundColor: 'rgba(112, 173, 71, 0.8)',
                        borderColor: '#507E32',
                        borderWidth: 2,
                        borderRadius: 4,
                        barThickness: 32
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Amount (Rs.)',
                            font: {
                                size: 12,
                                weight: 'bold'
                            }
                        },
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)',
                            drawBorder: false
                        },
                        ticks: {
                            callback: value => 'Rs. ' + value.toLocaleString(),
                            font: {
                                size: 11
                            }
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            }
                        }
                    }
                },
                plugins: {
                    title: {
                        display: false  // We'll add the title manually in PDF
                    },
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            font: {
                                size: 12,
                                weight: 'bold'
                            },
                            usePointStyle: true,
                            pointStyle: 'circle'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(255, 255, 255, 0.9)',
                        titleColor: '#333',
                        titleFont: {
                            size: 13,
                            weight: 'bold'
                        },
                        bodyColor: '#666',
                        bodyFont: {
                            size: 12
                        },
                        borderColor: '#ddd',
                        borderWidth: 1,
                        padding: 12,
                        displayColors: true,
                        callbacks: {
                            label: (context) => {
                                let label = context.dataset.label || '';
                                let value = context.raw || 0;
                                return `${label}: Rs. ${value.toLocaleString()}`;
                            }
                        }
                    }
                }
            }
        };

        const buffer = await chartJSNodeCanvas.renderToBuffer(chartConfig);
        console.log('Chart generation successful, buffer size:', buffer.length);
        return buffer;
    } catch (error) {
        console.error('Error generating chart:', error);
        throw error;
    }
}

/**
 * Fetch monthly sales data from the database
 */
async function getMonthlySalesData(sql, sqlConfig, monthYear) {
        const query = `
        SELECT 
            CAST(InvoiceDate AS DATE) as date,
            Description as item,
            SUM(Qty) as quantity,
            -- Apply discount per line and round to 2 decimals at line-level before summing
            SUM(ROUND(Qty * (UnitPrice * (1 - ISNULL(LineDiscountPercentage,0)/100.0)), 2)) as totalRevenueDisc,
            SUM(Qty * ISNULL(UnitCost, 0)) as totalCost,
            'TYRES' as category
        FROM [View_Sales report whatsapp]
        WHERE YEAR(InvoiceDate) = @year
        AND MONTH(InvoiceDate) = @month
        AND Categoty = 'TYRES'
        GROUP BY CAST(InvoiceDate AS DATE), Description
        ORDER BY CAST(InvoiceDate AS DATE), Description
    `;

    try {
        // Ensure we have a connection
        if (!sql.connected) {
            await sql.connect(sqlConfig);
        }
        const result = await sql.query(query.replace('@year', monthYear.year).replace('@month', monthYear.month));
        
        // Map and group the data by date
        const salesByDate = {};
        result.recordset.forEach(row => {
            const date = moment(row.date);
            const dateKey = date.format('YYYY-MM-DD');
            
            if (!salesByDate[dateKey]) {
                salesByDate[dateKey] = {
                    date: date,
                    items: []
                };
            }

            salesByDate[dateKey].items.push({
                item: row.item,
                quantity: Number(row.quantity) || 0,
                // Derive discount-aware average unit price from totals
                unitPrice: (Number(row.quantity) ? (Number(row.totalRevenueDisc) / Number(row.quantity)) : 0) || 0,
                totalAmount: Number(row.totalRevenueDisc) || 0,
                // Derive average cost price from totals
                costPrice: (Number(row.quantity) ? (Number(row.totalCost) / Number(row.quantity)) : 0) || 0,
                profit: (Number(row.totalRevenueDisc) || 0) - (Number(row.totalCost) || 0)
            });
        });

        // Convert to sorted array
        return Object.values(salesByDate)
            .sort((a, b) => a.date.valueOf() - b.date.valueOf());
    } catch (error) {
        console.error('Error fetching sales data:', error);
        throw error;
    }
}

/**
 * Create the PDF report with sales data
 */
async function createPDFReport(salesData, monthYear, outputPath) {
    console.log('Creating PDF report with', salesData.length, 'items');
    
    return new Promise(async (resolve, reject) => {
        let doc;
        let tempPath = null;
        try {
            // Create PDF generator class for better state management
            class PDFGenerator {
                constructor(doc, isCountingOnly = false) {
                    this.doc = doc;
                    this.currentPage = 1;
                    this.totalPages = null;
                    this.isCountingOnly = isCountingOnly;
                }

                setTotalPages(total) {
                    this.totalPages = total;
                }

                isNearPageEnd(y, requiredSpace = 150) {
                    return y > pageConfig.maxY - requiredSpace;
                }



                startNewPage() {
                    this.doc.addPage({ size: 'A4' });
                    // Do not draw table headers on new pages
                    return pageConfig.margin;
                }

                drawTableHeaders(y) {
                    const startX = pageConfig.margin;
                    
                    // Draw header background
                    this.doc.rect(startX, y, pageConfig.width, tableConfig.rowHeight)
                        .fill(tableConfig.colors.header);

                    // Draw header text
                    this.doc.font(pageConfig.fonts.bold)
                        .fontSize(tableConfig.fontSize.header)
                        .fillColor(tableConfig.colors.headerText);

                    let currentX = startX;
                    tableConfig.headers.forEach((header, i) => {
                        const align = tableConfig.alignment.right.includes(header) ? 'right' : 'left';
                        this.doc.text(header,
                            currentX + (align === 'right' ? -5 : 5),
                            y + (tableConfig.rowHeight - tableConfig.fontSize.header) / 2,
                            {
                                width: tableConfig.columnWidths[i],
                                align: align
                            }
                        );
                        currentX += tableConfig.columnWidths[i];
                    });

                    return y + tableConfig.rowHeight + 5;
                }
            }

            // First pass to count pages
            let pageCount = await new Promise((resolveCount) => {
                const tempDoc = new PDFDocument({
                    size: 'A4',
                    margins: { top: 50, bottom: 50, left: 50, right: 50 },
                    autoFirstPage: false
                });
                
                const tempGen = new PDFGenerator(tempDoc, true);
                let pages = 1;
                tempDoc.on('pageAdded', () => pages++);
                
                const tempPath = path.join(path.dirname(outputPath), '_temp_' + path.basename(outputPath));
                const tempStream = fs.createWriteStream(tempPath);
                tempDoc.pipe(tempStream);
                
                // Generate content without page numbers to count pages
                generatePDFContent(tempGen, salesData, monthYear)
                    .then(() => {
                        tempDoc.end();
                        resolveCount(pages);
                    });
            });

            // Initialize actual PDF
            doc = new PDFDocument({
                size: 'A4',
                margins: { top: 50, bottom: 50, left: 50, right: 50 },
                autoFirstPage: false
            });

            // Create PDF generator and store it in the document context
            const pdfGen = new PDFGenerator(doc);
            doc.pdfGen = pdfGen;  // Attach to doc for access in other functions
            pdfGen.setTotalPages(pageCount);

            async function generatePDFContent(pdfGen, salesData, monthYear) {
                const doc = pdfGen.doc;
                
                     // Company header removed per request. Continue with the rest of the content generation.
                     // (You'll need to update all doc references to use pdfGen methods)
            }

            // Create first page explicitly (to avoid empty page when autoFirstPage is false)
            doc.addPage({ size: 'A4' });

            // Set up error handling
            doc.on('error', err => {
                console.error('PDF Document error:', err);
                reject(err);
            });

            // Create write stream with error handling
            const stream = fs.createWriteStream(outputPath);
            stream.on('error', err => {
                console.error('Stream error:', err);
                cleanupAndReject(err);
            });
            stream.on('finish', () => {
                console.log('PDF stream finished writing');
                cleanupAndResolve();
            });

            // Cleanup helper functions
            const cleanupAndResolve = () => {
                try {
                    if (tempPath && fs.existsSync(tempPath)) {
                        fs.unlinkSync(tempPath);
                    }
                    resolve(outputPath);
                } catch (err) {
                    console.error('Cleanup error:', err);
                    resolve(outputPath);
                }
            };

            const cleanupAndReject = (err) => {
                try {
                    if (tempPath && fs.existsSync(tempPath)) {
                        fs.unlinkSync(tempPath);
                    }
                } catch (cleanupErr) {
                    console.error('Cleanup error:', cleanupErr);
                }
                reject(err);
            };

            // Pipe output to file
            doc.pipe(stream);

                    // Draw header at top of first page
                    let y = pageConfig.margin;
                    y = drawReportHeader(doc, monthYear, y);
                    // Add spacing after header before table header
                    y += 10;
                    // Table header removed from top of first page

// Helper to draw the report header (title + period) at top of each page
function drawReportHeader(doc, monthYear, y) {
    doc.font(pageConfig.fonts.bold)
       .fontSize(20)
       .fillColor(pageConfig.colors.text)
       .text('Monthly Sales Report', pageConfig.margin, y, {
           width: pageConfig.width,
           align: 'center'
       });
    y += 28;
    doc.font(pageConfig.fonts.regular)
       .fontSize(13)
       .fillColor(pageConfig.colors.subtext)
       .text(`${moment().month(monthYear.month - 1).format('MMMM')} ${monthYear.year}`, pageConfig.margin, y, {
           width: pageConfig.width,
           align: 'center'
       });
    y += 22;
    // Decorative line
    doc.moveTo(pageConfig.margin, y)
       .lineTo(pageConfig.margin + pageConfig.width, y)
       .lineWidth(1)
       .stroke(pageConfig.colors.secondary);
    y += 10;
    return y;
}
            
            // Add decorative line
            const lineY = doc.y + 10;
            doc.moveTo(pageConfig.margin, lineY)
               .lineTo(pageConfig.margin + pageConfig.width, lineY)
               .lineWidth(1)
               .stroke(pageConfig.colors.secondary);

            // Add chart and analysis section
            try {
                // Add spacing before the section
                doc.moveDown(2);
                
                // Create a section header box
                const headerHeight = 40;
                const headerY = doc.y;
                
                // Draw background for header
                doc.rect(pageConfig.margin, headerY, pageConfig.width, headerHeight)
                   .fill('#f5f7fa');
                
                // Add left accent border
                doc.rect(pageConfig.margin, headerY, 4, headerHeight)
                   .fill(pageConfig.colors.primary);
                
                // Add title text with proper vertical centering
                doc.font(pageConfig.fonts.bold)
                   .fontSize(18)
                   .fillColor(pageConfig.colors.primary)
                   .text('Sales Performance Analysis', 
                         pageConfig.margin + 15, 
                         headerY + (headerHeight - 18) / 2, {
                       width: pageConfig.width - 20,
                       align: 'left'
                   });
                
                // Move down after header
                doc.y = headerY + headerHeight + 15;

                // Process data for analysis
                const categoryData = {};
                salesData.forEach(dayGroup => {
                    dayGroup.items.forEach(item => {
                        const category = item.category || 'TYRES';
                        if (!categoryData[category]) {
                            categoryData[category] = {
                                quantity: 0,
                                revenue: 0,
                                profit: 0
                            };
                        }
                        categoryData[category].quantity += item.quantity;
                        categoryData[category].revenue += item.totalAmount;
                        categoryData[category].profit += item.profit;
                    });
                });

                const sortedCategories = Object.entries(categoryData)
                    .sort((a, b) => b[1].revenue - a[1].revenue);

                // Calculate key metrics
                const totalRevenue = sortedCategories.reduce((sum, [_, data]) => sum + data.revenue, 0);
                const totalProfit = sortedCategories.reduce((sum, [_, data]) => sum + data.profit, 0);
                const profitMargin = (totalProfit / totalRevenue * 100).toFixed(1);
                const topCategory = sortedCategories[0];
                const avgProfitPerDay = totalProfit / salesData.length;
                
                // Add analysis text with enhanced formatting
                doc.moveDown(1);
                doc.font(pageConfig.fonts.bold)
                   .fontSize(12)
                   .fillColor(pageConfig.colors.text)
                   .text('Key Insights:', pageConfig.margin, doc.y);
                
                doc.font(pageConfig.fonts.regular)
                   .fontSize(11)
                   .fillColor(pageConfig.colors.text)
                   .moveDown(0.5);

                // Format metrics nicely
                const insights = [
                    `• Top Category: ${topCategory[0]} (Revenue: Rs. ${topCategory[1].revenue.toLocaleString()})`,
                    `• Overall Profit Margin: ${profitMargin}%`,
                    `• Average Daily Profit: Rs. ${avgProfitPerDay.toLocaleString()}`,
                    `• Total Items Sold: ${sortedCategories.reduce((sum, [_, data]) => sum + data.quantity, 0).toLocaleString()}`
                ];

                insights.forEach(insight => {
                    doc.text(insight, {
                        width: pageConfig.width,
                        align: 'left'
                    });
                });

                // Generate and add chart
                doc.moveDown(1);
                const chartBuffer = await generateSalesChart(salesData);
                doc.image(chartBuffer, {
                    fit: [520, 300],
                    align: 'center'
                });
            } catch (err) {
                console.error('Error adding chart to PDF:', err);
                doc.moveDown()
                   .font('Helvetica')
                   .fontSize(10)
                   .fillColor('red')
                   .text('Chart generation failed. Continuing with report...', {
                       align: 'center'
                   });
            }

            // Start table after chart
            let yPos = doc.y + 30;
            let grandTotalQty = 0, grandTotalAmount = 0, grandTotalProfit = 0;
            
            // Page management functions
            function isNearPageEnd(y, requiredSpace = 150) {
                return y > pageConfig.maxY - requiredSpace;
            }

            function startNewPage() {
                // Get the current PDFGenerator instance
                const pdfGen = this.pdfGen || {};
                
                // Add page number if we have a PDFGenerator
                if (pdfGen.addPageNumber) {
                    pdfGen.addPageNumber();
                }
                
                // Create new page (explicit A4)
                doc.addPage({ size: 'A4' });
                
                // Update page count if we have a PDFGenerator
                if (pdfGen.currentPage) {
                    pdfGen.currentPage++;
                }
                
                const y = pageConfig.margin;
                return pdfGen.drawTableHeaders ? pdfGen.drawTableHeaders(y) : y;
            }

            function drawTableHeaders(y) {
                const startX = pageConfig.margin;
                
                // Draw header background
                doc.rect(startX, y, pageConfig.width, tableConfig.rowHeight)
                   .fill(tableConfig.colors.header);

                // Draw header text
                doc.font(pageConfig.fonts.bold)
                   .fontSize(tableConfig.fontSize.header)
                   .fillColor(tableConfig.colors.headerText);

                let currentX = startX;
                tableConfig.headers.forEach((header, i) => {
                    // Determine text alignment for column
                    const align = tableConfig.alignment.right.includes(header) ? 'right' : 'left';
                    
                    doc.text(header, 
                        currentX + (align === 'right' ? -5 : 5),
                        y + (tableConfig.rowHeight - tableConfig.fontSize.header) / 2,
                        {
                            width: tableConfig.columnWidths[i],
                            align: align
                        }
                    );
                    currentX += tableConfig.columnWidths[i];
                });

                return y + tableConfig.rowHeight + 5;
            }

            // Draw table headers first
            yPos = drawTableHeaders(yPos);

            // Process each date group
            for (const dateGroup of salesData) {
                // Check if we need a new page
                if (isNearPageEnd(yPos, tableConfig.rowHeight + 30)) {
                    yPos = pdfGen.startNewPage();
                    // Do NOT draw table header again
                }

                // Add spacing before date group
                yPos += 5;

                // Date header with gradient-like effect
                doc.rect(pageConfig.margin, yPos, pageConfig.width, tableConfig.rowHeight)
                   .fill(pageConfig.colors.altBackground);
                doc.rect(pageConfig.margin, yPos, 5, tableConfig.rowHeight)
                   .fill(pageConfig.colors.primary);
                
                doc.font(pageConfig.fonts.bold)
                   .fontSize(tableConfig.fontSize.header)
                   .fillColor(tableConfig.colors.date)
                   .text(dateGroup.date.format('dddd, DD MMM YYYY'), 
                         pageConfig.margin + 15, 
                         yPos + (tableConfig.rowHeight - tableConfig.fontSize.header) / 2);
                
                yPos += tableConfig.rowHeight + 5;

                // Process items for this date
                let dateTotalQty = 0, dateTotalAmount = 0, dateTotalProfit = 0;

                dateGroup.items.forEach((item, index) => {
                    if (yPos > pageConfig.maxY - tableConfig.rowHeight) {
                        doc.addPage({ size: 'A4' });
                        yPos = drawReportHeader(doc, monthYear, pageConfig.margin);
                        // Do NOT draw table header again
                    }

                    // Alternate row background with subtle borders
                    doc.rect(pageConfig.margin, yPos, pageConfig.width, tableConfig.rowHeight)
                       .fill(index % 2 === 0 ? tableConfig.colors.row : tableConfig.colors.altRow);
                    
                    // Add subtle left border
                    doc.rect(pageConfig.margin, yPos, 2, tableConfig.rowHeight)
                       .fill(pageConfig.colors.border);

                    doc.font(pageConfig.fonts.regular)
                       .fontSize(tableConfig.fontSize.data)
                       .fillColor(tableConfig.colors.text);

                    // Row data
                    let currentX = pageConfig.margin;
                    const rowData = [
                        { text: item.item, align: 'left' },
                        { text: item.quantity.toLocaleString(), align: 'right' },
                        { text: item.unitPrice.toLocaleString(), align: 'right' },
                        { text: item.totalAmount.toLocaleString(), align: 'right' },
                        { text: item.profit.toLocaleString(), align: 'right' }
                    ];

                    rowData.forEach((col, i) => {
                        doc.text(
                            col.text,
                            currentX + (col.align === 'right' ? -5 : 5),
                            yPos + (tableConfig.rowHeight - tableConfig.fontSize.data) / 2,
                            {
                                width: tableConfig.columnWidths[i],
                                align: col.align,
                                ellipsis: true
                            }
                        );
                        currentX += tableConfig.columnWidths[i];
                    });

                    // Update totals
                    dateTotalQty += item.quantity;
                    dateTotalAmount += item.totalAmount;
                    dateTotalProfit += item.profit;
                    grandTotalQty += item.quantity;
                    grandTotalAmount += item.totalAmount;
                    grandTotalProfit += item.profit;
                    yPos += 15;
                });

                // Date totals
                doc.font(pageConfig.fonts.bold)
                   .fontSize(tableConfig.fontSize.total)
                   .fillColor(tableConfig.colors.text);

                // Draw total row background
                const dateTotalsY = yPos + 5;
                doc.rect(pageConfig.margin, dateTotalsY, pageConfig.width, tableConfig.rowHeight)
                   .fillAndStroke(pageConfig.colors.altBackground, tableConfig.colors.header);

                let currentX = pageConfig.margin;
                doc.text('Day Total:', currentX + 5, dateTotalsY + (tableConfig.rowHeight - tableConfig.fontSize.total) / 2);

                const dateTotalsData = [
                    '',
                    dateTotalQty.toLocaleString(),
                    '',
                    dateTotalAmount.toLocaleString(),
                    dateTotalProfit.toLocaleString()
                ];

                dateTotalsData.forEach((text, i) => {
                    if (text) {
                        currentX = pageConfig.margin + 
                            tableConfig.columnWidths.slice(0, i).reduce((a, b) => a + b, 0);
                        doc.font(pageConfig.fonts.bold)
                           .fontSize(tableConfig.fontSize.total)
                           .fillColor(tableConfig.colors.text)
                           .text(text, currentX - 5, dateTotalsY + (tableConfig.rowHeight - tableConfig.fontSize.total) / 2, {
                                width: tableConfig.columnWidths[i],
                                align: 'right'
                           });
                    }
                });

                yPos = dateTotalsY + 30;
            }

            // Add final summary section
            if (isNearPageEnd(yPos, 150)) {
                yPos = startNewPage();
            }
            
            yPos += 20;

            // Calculate profitability metrics (guard against division by zero)
            const profitMargin = grandTotalAmount ? (grandTotalProfit / grandTotalAmount * 100).toFixed(1) : '0.0';
            const avgProfitPerItem = grandTotalQty ? (grandTotalProfit / grandTotalQty).toFixed(0) : '0';

            // Draw summary box
            const summaryBoxY = yPos;
            const summaryBoxHeight = 100;
            
            // Draw main summary box with accent border
            doc.rect(pageConfig.margin, summaryBoxY, pageConfig.width, summaryBoxHeight)
               .fillAndStroke(pageConfig.colors.background, pageConfig.colors.primary);
            
            // Add accent top border
            doc.rect(pageConfig.margin, summaryBoxY, pageConfig.width, 3)
               .fill(pageConfig.colors.primary);

            // Grid layout for summary
            const gridStartY = summaryBoxY + 15;
            const colWidth = pageConfig.width / 2;
            const rowHeight = 30;
            
            // Function to draw a metric
            function drawMetric(label, value, col, row, isHighlight = false) {
                const x = pageConfig.margin + (col * colWidth);
                const y = gridStartY + (row * rowHeight);
                
                // Label
                doc.font(pageConfig.fonts.regular)
                   .fontSize(10)
                   .fillColor(pageConfig.colors.text)
                   .text(label, x + 15, y);
                
                // Value
                doc.font(pageConfig.fonts.bold)
                   .fontSize(isHighlight ? 12 : 11)
                   .fillColor(isHighlight ? pageConfig.colors.primary : pageConfig.colors.text)
                   .text(value, x + (colWidth / 2) - 20, y, { 
                       width: (colWidth / 2),
                       align: 'right' 
                   });
            }

            // Draw metrics grid
            drawMetric('Total Items:', grandTotalQty.toLocaleString(), 0, 0);
            drawMetric('Total Revenue:', grandTotalAmount.toLocaleString(), 1, 0);
            drawMetric('Total Profit:', grandTotalProfit.toLocaleString(), 0, 1, true);
            drawMetric('Profit Margin:', profitMargin + '%', 1, 1, true);            // Generate actual content with page numbers
            await generatePDFContent(pdfGen, salesData, monthYear);
            

            // Monthly Summary page removed (summary already displayed earlier in the report)


            // Add page number to the current page (bottom center)
            doc.fontSize(8)
               .fillColor('#666666')
               .text(
                   `Page ${doc.page.number}`,
                   pageConfig.margin,
                   doc.page.height - 20,
                   {
                       align: 'center',
                       width: pageConfig.width
                   }
               );

        } catch (err) {
            console.error('Error creating PDF:', err);
            reject(err);
        }
        // Finalize PDF (only once, after all writing is done)
        if (doc) {
            try {
                doc.end();
            } catch (err) {
                console.error('Error creating PDF:', err);
                reject(err);
            }
        }
    });
}

async function handleMonthlySalesPDFReport(msg, sql, sqlConfig, client) {
    const text = msg.body.toLowerCase();
    const match = text.match(/^sales report (\d{4})\/(\d{2})$/);
    
    if (!match) return false;

    try {
        const monthYear = {
            year: parseInt(match[1], 10),
            month: parseInt(match[2], 10)
        };

        if (monthYear.month < 1 || monthYear.month > 12) {
            await client.sendMessage(msg.from, 'Invalid month. Please use format: sales report YYYY/MM (e.g., sales report 2025/09)');
            return true;
        }

        // Send acknowledgment
        await client.sendMessage(msg.from, 'Generating monthly sales report PDF, please wait...');

        // Fetch data and generate report
        const salesData = await getMonthlySalesData(sql, sqlConfig, monthYear);
        if (!salesData.length) {
            await client.sendMessage(msg.from, 'No sales data found for the specified month.');
            return true;
        }

        // Create output directory if it doesn't exist
        const outputDir = path.join(__dirname, '../reports');
        console.log('Creating output directory:', outputDir);
        if (!fs.existsSync(outputDir)) {
            fs.mkdirSync(outputDir, { recursive: true });
        }

        // Generate PDF
        const filename = `sales_report_${monthYear.year}_${monthYear.month.toString().padStart(2, '0')}.pdf`;
        const outputPath = path.join(outputDir, filename);
        console.log('Generating PDF at:', outputPath);
        console.log('Data items count:', salesData.length);

        await createPDFReport(salesData, monthYear, outputPath);
        console.log('PDF generation completed');

        // Verify the file was created
        const fileStats = fs.statSync(outputPath);
        console.log('PDF file size:', fileStats.size, 'bytes');

        // Send PDF
        const media = MessageMedia.fromFilePath(outputPath);
        await client.sendMessage(msg.from, media, {
            caption: `Sales Report for ${moment().month(monthYear.month - 1).format('MMMM')} ${monthYear.year}`
        });

        return true;
    } catch (error) {
        console.error('Error generating sales report:', error);
        await client.sendMessage(msg.from, 'Error generating sales report. Please try again later.');
        return true;
    }
}

module.exports = {
    handleMonthlySalesPDFReport,
    createPDFReport,
    getMonthlySalesData
};
