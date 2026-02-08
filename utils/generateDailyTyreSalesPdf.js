// utils/generateDailyTyreSalesPdf.js
// Generates an A4 professional detailed PDF for full-day TYRE sales.
const PDFDocument = require('pdfkit');
const moment = require('moment');
const sql = require('mssql');

function fmtMoney(n) {
  const v = Number(n || 0);
  return v.toLocaleString('en-LK', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function fmtInt(n) {
  const v = Number(n || 0);
  return v.toLocaleString('en-LK');
}

function safeText(v) {
  const s = (v === null || v === undefined) ? '' : String(v);
  return s.replace(/\s+/g, ' ').trim();
}

function ensurePageSpace(doc, needed = 80) {
  const bottom = doc.page.height - doc.page.margins.bottom;
  if (doc.y + needed > bottom) doc.addPage();
}

function drawSectionTitle(doc, title) {
  ensurePageSpace(doc, 40);
  doc.moveDown(0.6);
  doc.font('Helvetica-Bold').fontSize(12).fillColor('#111827').text(title);
  doc.moveDown(0.2);
  doc.strokeColor('#e5e7eb').lineWidth(1).moveTo(doc.page.margins.left, doc.y).lineTo(doc.page.width - doc.page.margins.right, doc.y).stroke();
  doc.moveDown(0.6);
}

function drawKeyValueGrid(doc, items) {
  // items: [{ k, v }, ...] rendered in two columns
  ensurePageSpace(doc, 80);
  const left = doc.page.margins.left;
  const right = doc.page.width - doc.page.margins.right;
  const mid = left + (right - left) / 2;
  const rowH = 14;
  const startY = doc.y;

  doc.fontSize(9).fillColor('#111827');
  for (let i = 0; i < items.length; i++) {
    const col = i % 2;
    const row = Math.floor(i / 2);
    const x = col === 0 ? left : mid;
    const y = startY + row * rowH;
    const kv = items[i];
    doc.font('Helvetica-Bold').text(safeText(kv.k) + ':', x, y, { width: (right - left) / 2 - 10 });
    doc.font('Helvetica').text(safeText(kv.v), x + 90, y, { width: (right - left) / 2 - 100 });
  }
  doc.y = startY + Math.ceil(items.length / 2) * rowH + 6;
}

function drawTable(doc, headers, rows, colWidths, opts = {}) {
  // headers: string[], rows: string[][], colWidths: number[] in points (sum <= printable width)
  const left = doc.page.margins.left;
  const right = doc.page.width - doc.page.margins.right;
  const width = right - left;
  const rowH = opts.rowHeight || 16;
  const headerH = opts.headerHeight || 18;
  const fontSize = opts.fontSize || 8.5;
  const wrap = Array.isArray(opts.wrap) ? opts.wrap : [];
  const padX = opts.padX || 4;
  const padY = opts.padY || 4;
  const minRowH = opts.minRowHeight || rowH;

  const totalColsWidth = colWidths.reduce((a, b) => a + b, 0);
  const scale = totalColsWidth > width ? (width / totalColsWidth) : 1;
  const scaled = colWidths.map(w => w * scale);

  function drawHeader() {
    ensurePageSpace(doc, headerH + rowH);
    const y = doc.y;
    doc.rect(left, y, width, headerH).fillColor('#2563eb').fill();
    doc.fillColor('#ffffff').font('Helvetica-Bold').fontSize(fontSize);
    let x = left;
    for (let i = 0; i < headers.length; i++) {
      doc.text(headers[i], x + 4, y + 5, { width: scaled[i] - 8, ellipsis: true });
      x += scaled[i];
    }
    doc.y = y + headerH;
  }

  drawHeader();
  doc.font('Helvetica').fontSize(fontSize).fillColor('#111827');

  for (let r = 0; r < rows.length; r++) {
    // Compute dynamic row height when wrapping is enabled.
    let computedRowH = minRowH;
    let xForMeasure = left;
    for (let c = 0; c < headers.length; c++) {
      const cell = rows[r][c] === undefined || rows[r][c] === null ? '' : String(rows[r][c]);
      const cellWrap = !!wrap[c];
      if (cellWrap) {
        const measureW = Math.max(10, scaled[c] - (padX * 2));
        // Use current font settings (Helvetica, fontSize) for accurate height.
        const h = doc.heightOfString(cell, { width: measureW, align: (opts.align && opts.align[c]) ? opts.align[c] : 'left' });
        computedRowH = Math.max(computedRowH, h + (padY * 2));
      }
      xForMeasure += scaled[c];
    }

    ensurePageSpace(doc, computedRowH + 10);
    if (doc.y > doc.page.height - doc.page.margins.bottom - computedRowH) {
      doc.addPage();
      drawHeader();
      doc.font('Helvetica').fontSize(fontSize).fillColor('#111827');
    }
    const y = doc.y;
    const isAlt = r % 2 === 1;
    doc.rect(left, y, width, computedRowH).fillColor(isAlt ? '#f8fafc' : '#ffffff').fill();
    doc.strokeColor('#e5e7eb').lineWidth(0.5).rect(left, y, width, computedRowH).stroke();

    let x = left;
    for (let c = 0; c < headers.length; c++) {
      const cell = rows[r][c] === undefined || rows[r][c] === null ? '' : String(rows[r][c]);
      const align = (opts.align && opts.align[c]) ? opts.align[c] : 'left';
      const cellWrap = !!wrap[c];
      doc.fillColor('#111827').text(cell, x + padX, y + padY, {
        width: Math.max(10, scaled[c] - (padX * 2)),
        align,
        ellipsis: !cellWrap,
        lineBreak: cellWrap
      });
      x += scaled[c];
    }
    doc.y = y + computedRowH;
  }
  doc.moveDown(0.6);
}

async function batchGetAvailableQuantities(mainPool, itemDescriptions) {
  try {
    if (!mainPool || !mainPool.connected) return new Map();
    const unique = [...new Set((itemDescriptions || []).map(s => safeText(s)).filter(Boolean))];
    if (unique.length === 0) return new Map();
    const req = mainPool.request();
    let whereClause = '';
    unique.forEach((item, idx) => {
      const p = `item${idx}`;
      req.input(p, sql.VarChar, item);
      whereClause += (idx ? ',' : '') + `@${p}`;
    });
    const q = `SELECT ItemDis, MAX(QTY) AS QTY FROM View_ItemWhse WHERE ItemDis IN (${whereClause}) GROUP BY ItemDis`;
    const res = await req.query(q);
    const map = new Map();
    for (const row of (res.recordset || [])) map.set(row.ItemDis, Number(row.QTY) || 0);
    for (const item of unique) if (!map.has(item)) map.set(item, 0);
    return map;
  } catch {
    return new Map();
  }
}

module.exports = async function generateDailyTyreSalesPdf(mainPool, dayISO, options = {}) {
  if (!mainPool || !mainPool.connected) throw new Error('mainPool not connected');

  const today = moment(String(dayISO).trim()).format('YYYY-MM-DD');
  const generatedAt = moment().format('YYYY-MM-DD HH:mm');

  // Pull full-day sales with payment method and invoice metadata (all categories)
  const req = mainPool.request();
  // Using Expr aliases as per View definition
  const res = await req.query`
    SELECT
      Expr2 AS InvoiceNo,
      Expr1 AS InvoiceDate,
      Expr10 AS CustomerName,
      Expr8 AS VehicleNo,
      Expr15 AS PaymentM,
      Expr4 AS Description,
      Expr5 AS Qty,
      Expr6 AS UnitPrice,
      UnitCost,
      Expr13 AS LineDiscountPercentage,
      Expr12 AS Amount,
      Categoty,
      Expr14 AS IsVoid
    FROM [View_Sales report whatsapp]
    WHERE CONVERT(date, Expr1) = ${today}
      AND Expr6 > 0
      AND (Expr14 = 0 OR Expr14 IS NULL)
    ORDER BY Expr1, Expr2
  `;

  const rows = (res.recordset || []).map(r => ({
    InvoiceNo: safeText(r.InvoiceNo),
    InvoiceDate: r.InvoiceDate,
    CustomerName: safeText(r.CustomerName),
    VehicleNo: safeText(r.VehicleNo),
    PaymentM: safeText(r.PaymentM) || 'Unknown',
    Description: safeText(r.Description),
    Category: safeText(r.Categoty) || 'Unknown',
    Qty: Number(r.Qty) || 0,
    UnitPrice: Number(r.UnitPrice) || 0,
    UnitCost: (r.UnitCost === null || r.UnitCost === undefined) ? 0 : (Number(r.UnitCost) || 0),
    LineDiscountPercentage: Number(r.LineDiscountPercentage) || 0,
    Amount: (r.Amount === null || r.Amount === undefined) ? null : (Number(r.Amount) || 0)
  })).filter(r => r.InvoiceNo && r.Description);

  // Group by invoice
  const invoicesMap = new Map();
  for (const r of rows) {
    if (!invoicesMap.has(r.InvoiceNo)) invoicesMap.set(r.InvoiceNo, []);
    invoicesMap.get(r.InvoiceNo).push(r);
  }

  // Stock is only meaningful for TYRE item summary (kept scoped to TYRES to reduce query size)
  const tyreItems = rows
    .filter(r => String(r.Category).trim().toUpperCase() === 'TYRES')
    .map(r => r.Description);
  const stockMap = await batchGetAvailableQuantities(mainPool, tyreItems);

  // Compute summaries
  let totalInvoices = invoicesMap.size;
  let totalQty = 0;
  let totalSales = 0;
  let totalCost = 0;
  let totalProfit = 0;

  const paymentAgg = new Map(); // method -> { count, amount }
  const tyreItemAgg = new Map(); // desc -> { qty, sales, cost, profit, stock }
  const categoryAgg = new Map(); // category -> { qty, sales, cost, profit }

  const invoices = [];
  for (const [invNo, lines] of invoicesMap) {
    const invDate = lines[0].InvoiceDate ? moment(lines[0].InvoiceDate).format('YYYY-MM-DD HH:mm') : '';
    const customer = lines[0].CustomerName;
    const vehicle = lines[0].VehicleNo;
    const payment = lines[0].PaymentM || 'Unknown';

    let invQty = 0, invSales = 0, invCost = 0, invProfit = 0;
    const lineModels = [];

    for (const it of lines) {
      const qty = Number(it.Qty) || 0;
      const unitPrice = Number(it.UnitPrice) || 0;
      let disc = Number(it.LineDiscountPercentage) || 0;
      if (disc < 0) disc = 0;
      if (disc > 100) disc = 100;
      const netUnit = unitPrice * (1 - disc / 100);
      const amount = qty * netUnit;
      const unitCost = Number(it.UnitCost) || 0;
      const cost = unitCost * qty;
      const profit = amount - cost;
      const normalizedCategory = safeText(it.Category) || 'Unknown';
      const stock = String(normalizedCategory).trim().toUpperCase() === 'TYRES' ? (stockMap.get(it.Description) || 0) : 0;

      invQty += qty;
      invSales += amount;
      invCost += cost;
      invProfit += profit;

      totalQty += qty;
      totalSales += amount;
      totalCost += cost;
      totalProfit += profit;

      // payment aggregation
      if (!paymentAgg.has(payment)) paymentAgg.set(payment, { count: 0, amount: 0 });
      // count invoices per payment only once

      // category aggregation (all categories)
      if (!categoryAgg.has(normalizedCategory)) categoryAgg.set(normalizedCategory, { qty: 0, sales: 0, cost: 0, profit: 0 });
      const ca = categoryAgg.get(normalizedCategory);
      ca.qty += qty;
      ca.sales += amount;
      ca.cost += cost;
      ca.profit += profit;

      // TYRE item aggregation (for the dedicated TYRES item summary section)
      if (String(normalizedCategory).trim().toUpperCase() === 'TYRES') {
        if (!tyreItemAgg.has(it.Description)) tyreItemAgg.set(it.Description, { qty: 0, sales: 0, cost: 0, profit: 0, stock });
        const ia = tyreItemAgg.get(it.Description);
        ia.qty += qty;
        ia.sales += amount;
        ia.cost += cost;
        ia.profit += profit;
        ia.stock = stock;
      }

      lineModels.push({
        category: normalizedCategory,
        desc: it.Description,
        qty,
        unitPrice,
        disc,
        netUnit,
        amount,
        unitCost,
        cost,
        profit,
        stock
      });
    }

    // payment aggregation: count invoice once, add total invoice sales
    const pa = paymentAgg.get(payment) || { count: 0, amount: 0 };
    pa.count += 1;
    pa.amount += invSales;
    paymentAgg.set(payment, pa);

    invoices.push({ invNo, invDate, customer, vehicle, payment, qty: invQty, sales: invSales, cost: invCost, profit: invProfit, lines: lineModels });
  }

  const margin = totalSales > 0 ? ((totalProfit / totalSales) * 100) : 0;
  const avgInvoice = totalInvoices > 0 ? (totalSales / totalInvoices) : 0;

  // Sort for readability
  invoices.sort((a, b) => String(a.invNo).localeCompare(String(b.invNo)));
  const paymentRows = Array.from(paymentAgg.entries()).sort((a, b) => (b[1].amount || 0) - (a[1].amount || 0));
  const tyreItemRows = Array.from(tyreItemAgg.entries()).sort((a, b) => (b[1].sales || 0) - (a[1].sales || 0));
  const categoryRows = Array.from(categoryAgg.entries()).sort((a, b) => (b[1].sales || 0) - (a[1].sales || 0));

  const tyresTotals = { qty: 0, sales: 0, cost: 0, profit: 0 };
  for (const [, it] of tyreItemAgg.entries()) {
    tyresTotals.qty += Number(it.qty) || 0;
    tyresTotals.sales += Number(it.sales) || 0;
    tyresTotals.cost += Number(it.cost) || 0;
    tyresTotals.profit += Number(it.profit) || 0;
  }

  // PDF
  const doc = new PDFDocument({ size: 'A4', margin: 42, bufferPages: true, compress: true });
  const chunks = [];
  doc.on('data', c => chunks.push(c));

  // Title
  doc.font('Helvetica-Bold').fontSize(18).fillColor('#111827').text('Daily Sales Report', { align: 'center' });
  doc.moveDown(0.2);
  doc.font('Helvetica').fontSize(10).fillColor('#6b7280').text(`Report Date: ${moment(today).format('MMMM DD, YYYY')}   |   Generated: ${generatedAt}`, { align: 'center' });
  doc.moveDown(0.1);
  doc.font('Helvetica').fontSize(9).fillColor('#6b7280').text('Includes all invoices/items for the day + dedicated TYRES item summary.', { align: 'center' });
  doc.moveDown(0.6);
  doc.strokeColor('#e5e7eb').lineWidth(1).moveTo(doc.page.margins.left, doc.y).lineTo(doc.page.width - doc.page.margins.right, doc.y).stroke();
  doc.moveDown(0.8);

  // Executive summary
  drawSectionTitle(doc, 'Executive Summary');
  drawKeyValueGrid(doc, [
    { k: 'Total Invoices', v: fmtInt(totalInvoices) },
    { k: 'Total Quantity (All)', v: fmtInt(totalQty) },
    { k: 'Total Sales', v: `LKR ${fmtMoney(totalSales)}` },
    { k: 'Total Cost', v: `LKR ${fmtMoney(totalCost)}` },
    { k: 'Total Profit', v: `LKR ${fmtMoney(totalProfit)}` },
    { k: 'Profit Margin', v: `${margin.toFixed(1)}%` },
    { k: 'Average Invoice Value', v: `LKR ${fmtMoney(avgInvoice)}` },
    { k: 'Category Focus', v: 'TYRES' }
  ]);

  // Payment methods
  drawSectionTitle(doc, 'Payment Methods');
  drawTable(
    doc,
    ['Method', 'Invoices', 'Amount (LKR)'],
    paymentRows.map(([name, p]) => [
      name || 'Unknown',
      fmtInt(p.count),
      fmtMoney(p.amount)
    ]),
    [260, 80, 140],
    { align: ['left', 'right', 'right'] }
  );

  // Quantity summary by category
  drawSectionTitle(doc, 'Quantity Summary (By Category)');
  drawTable(
    doc,
    ['Category', 'Qty', 'Sales', 'Profit', 'Margin'],
    (function buildCategorySummaryRows() {
      let tq = 0, ts = 0, tp = 0, tc = 0;
      const lines = categoryRows.map(([cat, it]) => {
        tq += Number(it.qty) || 0;
        ts += Number(it.sales) || 0;
        tc += Number(it.cost) || 0;
        tp += Number(it.profit) || 0;
        const m = it.sales > 0 ? ((it.profit / it.sales) * 100) : 0;
        return [
          cat || 'Unknown',
          fmtInt(it.qty),
          fmtMoney(it.sales),
          fmtMoney(it.profit),
          `${m.toFixed(1)}%`
        ];
      });
      const totalMargin = ts > 0 ? ((tp / ts) * 100) : 0;
      lines.push(['TOTAL', fmtInt(tq), fmtMoney(ts), fmtMoney(tp), `${totalMargin.toFixed(1)}%`]);
      return lines;
    })(),
    [210, 55, 95, 95, 70],
    { align: ['left', 'right', 'right', 'right', 'right'], fontSize: 8.4, wrap: [true, false, false, false, false], minRowHeight: 16 }
  );

  // Item summary (TYRES)
  drawSectionTitle(doc, 'Item Summary (TYRES)');
  drawTable(
    doc,
    ['Item', 'Qty', 'Sales', 'Cost', 'Profit', 'Margin', 'Stock'],
    (function buildTyreItemSummaryRows() {
      const lines = tyreItemRows.map(([desc, it]) => {
        const m = it.sales > 0 ? ((it.profit / it.sales) * 100) : 0;
        return [
          desc,
          fmtInt(it.qty),
          fmtMoney(it.sales),
          fmtMoney(it.cost),
          fmtMoney(it.profit),
          `${m.toFixed(1)}%`,
          fmtInt(it.stock)
        ];
      });
      const totalMargin = tyresTotals.sales > 0 ? ((tyresTotals.profit / tyresTotals.sales) * 100) : 0;
      lines.push([
        'TOTAL',
        fmtInt(tyresTotals.qty),
        fmtMoney(tyresTotals.sales),
        fmtMoney(tyresTotals.cost),
        fmtMoney(tyresTotals.profit),
        `${totalMargin.toFixed(1)}%`,
        ''
      ]);
      return lines;
    })(),
    [210, 45, 70, 70, 70, 55, 50],
    {
      align: ['left', 'right', 'right', 'right', 'right', 'right', 'right'],
      fontSize: 8.2,
      wrap: [true, false, false, false, false, false, false],
      minRowHeight: 18
    }
  );

  // Invoice details
  doc.addPage();
  drawSectionTitle(doc, 'Invoice Details');
  doc.font('Helvetica').fontSize(9).fillColor('#6b7280').text('Each invoice is listed with item lines, totals, and payment method.', { align: 'left' });
  doc.moveDown(0.6);

  for (const inv of invoices) {
    ensurePageSpace(doc, 160);
    doc.font('Helvetica-Bold').fontSize(11).fillColor('#111827').text(`Invoice: ${inv.invNo}`);
    doc.moveDown(0.2);
    doc.font('Helvetica').fontSize(9).fillColor('#374151');
    drawKeyValueGrid(doc, [
      { k: 'Invoice Date/Time', v: inv.invDate || '-' },
      { k: 'Payment Method', v: inv.payment || 'Unknown' },
      { k: 'Customer', v: inv.customer || '-' },
      { k: 'Vehicle No', v: inv.vehicle || '-' },
      { k: 'Invoice Total', v: `LKR ${fmtMoney(inv.sales)}` },
      { k: 'Invoice Profit', v: `LKR ${fmtMoney(inv.profit)}` }
    ]);

    drawTable(
      doc,
      ['Category', 'Item', 'Qty', 'Unit', 'Disc%', 'Net Unit', 'Amount', 'Profit'],
      inv.lines.map(L => {
        const m = L.amount > 0 ? ((L.profit / L.amount) * 100) : 0;
        return [
          L.category || 'Unknown',
          L.desc,
          fmtInt(L.qty),
          fmtMoney(L.unitPrice),
          `${Number(L.disc || 0).toFixed(0)}%`,
          fmtMoney(L.netUnit),
          fmtMoney(L.amount),
          `${fmtMoney(L.profit)} (${m.toFixed(1)}%)`
        ];
      }),
      [80, 170, 35, 50, 40, 55, 55, 75],
      {
        align: ['left', 'left', 'right', 'right', 'right', 'right', 'right', 'right'],
        fontSize: 7.8,
        minRowHeight: 18,
        wrap: [false, true, false, false, false, false, false, false]
      }
    );

    doc.moveDown(0.2);
    doc.strokeColor('#e5e7eb').lineWidth(1).moveTo(doc.page.margins.left, doc.y).lineTo(doc.page.width - doc.page.margins.right, doc.y).stroke();
    doc.moveDown(0.6);
  }

  // Footer page numbers
  const range = doc.bufferedPageRange();
  for (let i = 0; i < range.count; i++) {
    doc.switchToPage(i);
    doc.font('Helvetica').fontSize(8).fillColor('#6b7280');
    const pageNo = i + 1;
    doc.text(`Page ${pageNo} of ${range.count}`, 0, doc.page.height - doc.page.margins.bottom + 10, { align: 'center' });
  }

  doc.end();

  const pdfBuffer = await new Promise((resolve, reject) => {
    doc.on('end', () => resolve(Buffer.concat(chunks)));
    doc.on('error', reject);
  });

  const fileName = `Daily_Tyre_Sales_Report_${today}.pdf`;
  return { buffer: pdfBuffer, fileName };
};
