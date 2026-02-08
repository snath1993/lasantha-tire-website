// MonthlySalesPDFReportJob.js
// Generates professional PDF monthly sales reports with categories and charts
const moment = require('moment');
const PDFDocument = require('pdfkit');
const fs = require('fs');
const path = require('path');
const Chart = require('chart.js/auto');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');
const { MessageMedia } = require('whatsapp-web.js');

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

// Define the table structure and styling
const pageConfig = {
    margin: 50,
    width: 495,  // A4 width (595) - 2*margin
    maxY: 750    // A4 height (842) - margin for footer
};

const tableConfig = {
    headers: ['Date', 'Item', 'Invoice #', 'Quantity', 'Unit Price', 'Total', 'Profit'],
    columnWidths: [70, 140, 60, 50, 60, 60, 55], // Total = 495 (page width)
    headerColor: '#4472C4',
    rowColors: ['#FFFFFF', '#E9EEF6'],
    textColors: {
        header: '#FFFFFF',
        regular: '#000000',
        total: '#000000'
    }
};

/**
 * Generate a chart for the sales data
 */
async function generateSalesChart(salesData) {
    console.log('Generating sales chart...');
    try {
        // Process data for chart
        const categoryData = salesData.reduce((acc, sale) => {
            const category = sale.category || 'Uncategorized';
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
                        backgroundColor: '#4472C4',
                        borderColor: '#2F528F',
                        borderWidth: 1
                    },
                    {
                        label: 'Profit',
                        data: sortedCategories.map(([_, data]) => data.profit),
                        backgroundColor: '#70AD47',
                        borderColor: '#507E32',
                        borderWidth: 1
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
                            text: 'Amount (Rs.)'
                        },
                        ticks: {
                            callback: value => 'Rs. ' + value.toLocaleString()
                        }
                    }
                },
                plugins: {
                    title: {
                        display: true,
                        text: 'Sales by Category',
                        font: {
                            size: 16,
                            weight: 'bold'
                        }
                    },
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            font: {
                                size: 12
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
            InvoiceDate as date,
            Description as item,
            InvoiceNo as invoiceNumber,
            Qty as quantity,
            UnitPrice as unitPrice,
            UnitCost as costPrice,
            Categoty as category
        FROM [View_Sales report whatsapp]
        WHERE YEAR(InvoiceDate) = @year
        AND MONTH(InvoiceDate) = @month
        AND IsConfirm = 1
        ORDER BY InvoiceDate, Categoty, Description
    `;

    try {
        // Ensure we have a connection
        if (!sql.connected) {
            await sql.connect(sqlConfig);
        }
        const result = await sql.query(query.replace('@year', monthYear.year).replace('@month', monthYear.month));
        return result.recordset.map(row => ({
            date: moment(row.date).format('YYYY-MM-DD'),
            item: row.item,
            invoiceNumber: row.invoiceNumber,
            quantity: Number(row.quantity) || 0,
            unitPrice: Number(row.unitPrice) || 0,
            totalAmount: (Number(row.quantity) || 0) * (Number(row.unitPrice) || 0),
            costPrice: Number(row.costPrice) || 0,
            profit: (Number(row.quantity) || 0) * ((Number(row.unitPrice) || 0) - (Number(row.costPrice) || 0)),
            category: row.category
        }));
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
        try {
            doc = new PDFDocument({
                size: 'A4',
                margins: { top: 50, bottom: 50, left: 50, right: 50 },
                bufferPages: true,
                autoFirstPage: true
            });

            // Set up error handling
            doc.on('error', err => {
                console.error('PDF Document error:', err);
                reject(err);
            });

            // Create write stream with error handling
            const stream = fs.createWriteStream(outputPath);
            stream.on('error', err => {
                console.error('Stream error:', err);
                reject(err);
            });
            stream.on('finish', () => {
                console.log('PDF stream finished writing');
                resolve(outputPath);
            });

            // Pipe output to file
            doc.pipe(stream);

            // Add header
            doc.font('Helvetica-Bold')
               .fontSize(24)
               .text('Monthly Sales Report', { align: 'center' });
            
            doc.moveDown()
               .fontSize(14)
               .text(`${moment().month(monthYear.month - 1).format('MMMM')} ${monthYear.year}`, { align: 'center' });

            // Generate and add chart
            try {
                const chartBuffer = await generateSalesChart(salesData);
                doc.moveDown()
                   .image(chartBuffer, {
                       fit: [500, 250],
                       align: 'center',
                       valign: 'center'
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

            // Group data by category
            const categoryGroups = salesData.reduce((acc, sale) => {
                const category = sale.category || 'Uncategorized';
                if (!acc[category]) acc[category] = [];
                acc[category].push(sale);
                return acc;
            }, {});

            // Add tables for each category
            let yPos = doc.y + 30;
            
            function drawTableHeaders(y) {
                const startX = pageConfig.margin;
                doc.font('Helvetica-Bold')
                   .fontSize(10)
                   .fillColor(tableConfig.headerColor);

                // Draw header background
                doc.rect(startX, y, pageConfig.width, 20)
                   .fill();

                // Draw header text
                doc.fillColor(tableConfig.textColors.header);
                let currentX = startX;
                tableConfig.headers.forEach((header, i) => {
                    doc.text(header, 
                        currentX,
                        y + 5,
                        {
                            width: tableConfig.columnWidths[i],
                            align: i > 2 ? 'right' : 'left'
                        }
                    );
                    currentX += tableConfig.columnWidths[i];
                });
                return y + 25;
            }

            for (const [category, items] of Object.entries(categoryGroups)) {
                // Check if we need a new page
                if (yPos > pageConfig.maxY) {
                    doc.addPage();
                    yPos = pageConfig.margin;
                }

                // Category header
                doc.font('Helvetica-Bold')
                   .fontSize(14)
                   .fillColor(tableConfig.textColors.regular)
                   .text(category, pageConfig.margin, yPos);
                yPos += 25;

                // Table headers
                yPos = drawTableHeaders(yPos);

                // Table rows
                let totalQty = 0, totalAmount = 0, totalProfit = 0;
                items.forEach((sale, index) => {
                    if (yPos > pageConfig.maxY) {
                        doc.addPage();
                        yPos = pageConfig.margin;
                        yPos = drawTableHeaders(yPos);
                    }

                    // Alternate row background
                    if (index % 2 === 1) {
                        doc.rect(pageConfig.margin, yPos, pageConfig.width, 15)
                           .fill('#f8f9fa');
                    }

                    doc.font('Helvetica')
                       .fontSize(9)
                       .fillColor(tableConfig.textColors.regular);

                    // Row data
                    let currentX = pageConfig.margin;
                    const rowData = [
                        moment(sale.date).format('DD/MM/YY'),
                        sale.item,
                        sale.invoiceNumber,
                        sale.quantity.toLocaleString(),
                        `Rs. ${sale.unitPrice.toLocaleString()}`,
                        `Rs. ${sale.totalAmount.toLocaleString()}`,
                        `Rs. ${sale.profit.toLocaleString()}`
                    ];

                    rowData.forEach((text, i) => {
                        doc.text(text,
                            currentX,
                            yPos + 3,
                            {
                                width: tableConfig.columnWidths[i],
                                align: i > 2 ? 'right' : 'left',
                                ellipsis: true
                            }
                        );
                        currentX += tableConfig.columnWidths[i];
                    });

                    // Update totals
                    totalQty += sale.quantity;
                    totalAmount += sale.totalAmount;
                    totalProfit += sale.profit;
                    yPos += 15;
                });

                // Category totals
                doc.font('Helvetica-Bold')
                   .fontSize(10)
                   .fillColor(tableConfig.textColors.total);

                // Draw total row background
                const totalsY = yPos + 5;
                doc.rect(pageConfig.margin, totalsY, pageConfig.width, 20)
                   .fillAndStroke('#E9EEF6', '#4472C4');

                let currentX = pageConfig.margin;
                doc.text('Category Total:', currentX + 5, totalsY + 5);

                const totalsData = [
                    '',
                    '',
                    '',
                    totalQty.toLocaleString(),
                    '',
                    `Rs. ${totalAmount.toLocaleString()}`,
                    `Rs. ${totalProfit.toLocaleString()}`
                ];

                totalsData.forEach((text, i) => {
                    if (text) {
                        currentX = pageConfig.margin + 
                            tableConfig.columnWidths.slice(0, i).reduce((a, b) => a + b, 0);
                        doc.text(text, currentX, totalsY + 5, {
                            width: tableConfig.columnWidths[i],
                            align: 'right'
                        });
                    }
                });

                yPos = totalsY + 30;
            }

            // Grand totals
            const grandTotal = salesData.reduce((acc, sale) => ({
                quantity: acc.quantity + sale.quantity,
                amount: acc.amount + sale.totalAmount,
                profit: acc.profit + sale.profit
            }), { quantity: 0, amount: 0, profit: 0 });

            // Monthly Summary page with box design
            doc.addPage();
            
            const summaryBox = {
                x: 100,
                y: 200,
                width: 400,
                height: 200,
                padding: 20
            };

            // Draw a styled box for the summary
            doc.rect(summaryBox.x, summaryBox.y, summaryBox.width, summaryBox.height)
               .fillAndStroke('#F8F9FA', '#4472C4');

            // Title
            doc.font('Helvetica-Bold')
               .fontSize(24)
               .fillColor('#2F528F')
               .text('Monthly Summary', summaryBox.x, summaryBox.y - 40, { 
                   width: summaryBox.width,
                   align: 'center' 
               });

            // Summary items with formatted numbers
            const summaryY = summaryBox.y + summaryBox.padding;
            doc.fontSize(14)
               .fillColor('#000000');

            const summaryItems = [
                { label: 'Total Items Sold:', value: grandTotal.quantity.toLocaleString() },
                { label: 'Total Revenue:', value: `Rs. ${grandTotal.amount.toLocaleString()}` },
                { label: 'Total Profit:', value: `Rs. ${grandTotal.profit.toLocaleString()}` }
            ];

            summaryItems.forEach((item, index) => {
                const y = summaryY + (index * 40);
                
                // Label (left-aligned)
                doc.font('Helvetica-Bold')
                   .text(item.label, 
                        summaryBox.x + summaryBox.padding,
                        y,
                        { width: 200 });
                
                // Value (right-aligned)
                doc.font('Helvetica')
                   .text(item.value,
                        summaryBox.x + summaryBox.width - 200 - summaryBox.padding,
                        y,
                        { width: 200, align: 'right' });
            });

            // Add page numbers
            const pages = doc.bufferedPageRange();
            for (let i = 0; i < pages.count; i++) {
                doc.switchToPage(i);
                doc.fontSize(8)
                   .fillColor('#666666')
                   .text(
                       `Page ${i + 1} of ${pages.count}`,
                       pageConfig.margin,
                       doc.page.height - 20,
                       {
                           align: 'center',
                           width: pageConfig.width
                       }
                   );
            }

            // Finalize PDF
            doc.end();

        } catch (err) {
            console.error('Error creating PDF:', err);
            if (doc) {
                try {
                    doc.end();
                } catch (endErr) {
                    console.error('Error ending PDF document:', endErr);
                }
            }
            reject(err);
        }
    });
}

/**
 * Handle the monthly sales PDF report request
 */
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
    handleMonthlySalesPDFReport
};