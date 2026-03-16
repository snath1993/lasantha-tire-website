/**
 * Professional Quotation PDF Template Builder (v2.0)
 * 
 * Generates premium HTML layout matching the digital-invoice system
 * for client-side PDF generation via html2pdf.js.
 * 
 * Supports: VAT / Non-VAT modes, warranty info, customer VAT no,
 * multiple tyre comparison, service items with FREE badges.
 */

// ─── Helpers ────────────────────────────────────────────────────────────────
const esc = (s: string | number | null | undefined): string =>
  String(s ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');

const money = (n: number): string => {
  if (!Number.isFinite(n)) return '';
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(n);
};

const round2 = (n: number): number => Math.round((n + Number.EPSILON) * 100) / 100;

// ─── Company Config (matches digital-invoice/config.js) ─────────────────────
const COMPANY = {
  NAME: 'Lasantha Tyre Traders',
  NAME_NONVAT: 'New Lasantha Tyre Traders',
  ADDRESS: '1035, Pannipitiya Road, Kumaragewattha, Battaramulla',
  PHONE: '0112 773 232',
  FAX: '0112 773 231',
  MOBILE: '0771 222 509',
  EMAIL: 'lasanthatyretraders@gmail.com',
  WEB: 'www.lasanthatyre.lk',
  VAT_NO: '743321219-7000',
  COLOR_PRIMARY: '#e67e22',
  COLOR_SECONDARY: '#2c3e50',
};

// ─── Types ──────────────────────────────────────────────────────────────────
export interface QuotationDetails {
  vehicleNo: string;
  customerName: string;
  terms: string;
  date: string;
  quotationNo: string;
  expiryDate?: string;
}

export interface QuotationItem {
  Description?: string;
  ItemName?: string;
  Brand?: string;
  Size?: string;
  Quantity?: number;
  UnitPrice?: number;
  Price?: number;
  Category?: string;
  isFOC?: boolean;
  DiscountPercent?: number;
  Country?: string;
  Warranty?: string;
}

export interface QuotationPdfOptions {
  includeVat?: boolean;
  vatRate?: number;
  customerVatNo?: string;
  warrantyInfo?: { km: string; years: string };
}

// ─── Item Classification ────────────────────────────────────────────────────
function isServiceItem(item: QuotationItem): boolean {
  const desc = (item.Description || item.ItemName || '').toLowerCase();
  return desc.includes('alignment') ||
    desc.includes('balancing') ||
    desc.includes('nitrogen') ||
    desc.includes('tubeless neck') ||
    (item.Category || '').toLowerCase() === 'service';
}

function isTyreItem(item: QuotationItem): boolean {
  const category = String(item?.Category ?? '').trim();
  if (!category) return false;
  return /tyre|tire/i.test(category);
}

// ─── Build Items Table HTML ─────────────────────────────────────────────────
function buildItemsTableHtml(items: QuotationItem[], options: QuotationPdfOptions): string {
  if (items.length === 0) return '<p style="text-align:center; color:#999;">No items</p>';

  const tyreItems = items.filter(it => !isServiceItem(it));
  const serviceItems = items.filter(it => isServiceItem(it));
  const includeVat = options.includeVat === true;
  const vatRate = Number(options.vatRate ?? 0);

  let html = '';

  if (tyreItems.length > 0) {
    const showAsComparison = tyreItems.length > 1;

    if (showAsComparison) {
      html += buildComparisonTable(tyreItems, includeVat, vatRate);
    } else {
      html += buildSingleItemTable(tyreItems[0], includeVat, vatRate);
    }
  }

  if (serviceItems.length > 0) {
    html += buildServicesTable(serviceItems);
  }

  return html;
}

// ── Comparison Table (multiple tyre options) ──
function buildComparisonTable(items: QuotationItem[], includeVat: boolean, vatRate: number): string {
  const optCount = items.length;
  const totalCols = optCount + 1;

  let optHeaders = '';
  let colTags = '';
  for (let i = 0; i < optCount; i++) {
    colTags += `<col>`;
    optHeaders += `<th class="opt-head"><span class="opt-pill">${i + 1}</span> Option ${i + 1}</th>`;
  }

  const descRow = buildRow('Description', items, it => esc(it.Description || it.ItemName || '-'), true);
  const brandRow = buildRow('Brand', items, it => esc(it.Brand || '-'));
  const sizeRow = buildRow('Size', items, it => esc(it.Size || '-'));
  const qtyRow = buildRow('Quantity', items, it => String(it.Quantity || 1));

  const countryRow = items.some(it => it.Country) ? buildRow('Country', items, it => esc(it.Country || '-')) : '';
  const warrantyRow = items.some(it => it.Warranty) ? buildRow('Warranty', items, it => esc(it.Warranty || '-')) : '';

  // Pricing section depends on VAT mode
  let pricingRows = '';
  let grandCells = '';

  if (includeVat && vatRate > 0) {
    const vatMultiplier = 1 + vatRate / 100;

    // Base price (Excl. VAT)
    pricingRows += buildCalcRow('Unit Price (Excl. VAT)', items, it => {
      const sellPrice = it.UnitPrice || it.Price || 0;
      return round2(sellPrice / vatMultiplier);
    });
    // VAT per unit
    pricingRows += buildCalcRow(`VAT (${vatRate}%)`, items, it => {
      const sellPrice = it.UnitPrice || it.Price || 0;
      const base = round2(sellPrice / vatMultiplier);
      return round2(sellPrice - base);
    });
    // Unit Price (Incl. VAT)
    pricingRows += buildCalcRow('Unit Price (Incl. VAT)', items, it => it.UnitPrice || it.Price || 0);
    // Line total
    pricingRows += buildCalcRow('Line Total', items, it => {
      const qty = it.Quantity || 1;
      return round2(qty * (it.UnitPrice || it.Price || 0));
    }, 'subtotal');

    // Grand total row cells (including VAT) — not summed because multiple options
    for (const item of items) {
      const total = round2((item.Quantity || 1) * (item.UnitPrice || item.Price || 0));
      grandCells += `<td class="prc">${money(total)}</td>`;
    }
  } else {
    // Non-VAT: simple pricing
    pricingRows += buildCalcRow('Unit Price', items, it => it.UnitPrice || it.Price || 0);
    pricingRows += buildCalcRow('Total', items, it => {
      return round2((it.Quantity || 1) * (it.UnitPrice || it.Price || 0));
    }, 'subtotal');

    for (const item of items) {
      const total = round2((item.Quantity || 1) * (item.UnitPrice || item.Price || 0));
      grandCells += `<td class="prc">${money(total)}</td>`;
    }
  }

  // Note for multiple tyre options
  const tyreCount = items.filter(isTyreItem).length;
  const suppressNote = tyreCount > 1
    ? `<tr><td colspan="${totalCols}" style="text-align:center; font-size:7px; color:#888; padding:4px; font-style:italic;">
        * Multiple tyre options included for selection. Please select your preferred option.
      </td></tr>`
    : '';

  return `
    <table class="cmp-table">
      <colgroup><col class="col-label">${colTags}</colgroup>
      <thead>
        <tr>
          <th class="corner">Specification</th>
          ${optHeaders}
        </tr>
      </thead>
      <tbody>
        <tr class="sec-div"><td colspan="${totalCols}">&#9654; Product Details</td></tr>
        ${descRow}${brandRow}${sizeRow}${countryRow}${warrantyRow}${qtyRow}
        <tr class="sec-div"><td colspan="${totalCols}">&#9654; Pricing${includeVat ? ' (VAT Inclusive)' : ''}</td></tr>
        ${pricingRows}
        <tr class="grand">
          <td class="lbl">GRAND TOTAL${includeVat ? ' (Incl. VAT)' : ''}</td>
          ${grandCells}
        </tr>
        ${suppressNote}
      </tbody>
    </table>`;
}

function buildRow(label: string, items: QuotationItem[], getValue: (it: QuotationItem) => string, isDesc = false): string {
  const trClass = isDesc ? ' class="desc-row"' : '';
  let cells = '';
  for (const item of items) {
    cells += `<td class="val">${getValue(item)}</td>`;
  }
  return `<tr${trClass}><td class="lbl">${esc(label)}</td>${cells}</tr>\n`;
}

function buildCalcRow(label: string, items: QuotationItem[], getValue: (it: QuotationItem) => number, extraClass = ''): string {
  const trClass = extraClass ? ` class="${extraClass}"` : '';
  let cells = '';
  for (const item of items) {
    const val = getValue(item);
    cells += `<td class="prc">${val > 0 ? money(val) : '-'}</td>`;
  }
  return `<tr${trClass}><td class="lbl">${esc(label)}</td>${cells}</tr>\n`;
}

// ── Single Item Table ──
function buildSingleItemTable(item: QuotationItem, includeVat: boolean, vatRate: number): string {
  const qty = item.Quantity || 1;
  const sellPrice = item.UnitPrice || item.Price || 0;

  let pricingRows = '';

  if (includeVat && vatRate > 0) {
    const vatMultiplier = 1 + vatRate / 100;
    const basePrice = round2(sellPrice / vatMultiplier);
    const vatAmount = round2(sellPrice - basePrice);
    const totalBase = round2(qty * basePrice);
    const totalVat = round2(qty * vatAmount);
    const grandTotal = round2(qty * sellPrice);

    pricingRows = `
        <tr><td class="lbl">Unit Price (Excl. VAT)</td><td class="prc">${money(basePrice)}</td></tr>
        <tr><td class="lbl">VAT (${vatRate}%)</td><td class="prc">${money(vatAmount)}</td></tr>
        <tr><td class="lbl">Unit Price (Incl. VAT)</td><td class="prc">${money(sellPrice)}</td></tr>
        <tr class="subtotal"><td class="lbl">Sub Total (Excl. VAT)</td><td class="prc">${money(totalBase)}</td></tr>
        <tr><td class="lbl">Total VAT</td><td class="prc">${money(totalVat)}</td></tr>
        <tr class="grand"><td class="lbl">GRAND TOTAL (Incl. VAT)</td><td class="prc">${money(grandTotal)}</td></tr>`;
  } else {
    const total = round2(qty * sellPrice);
    pricingRows = `
        <tr><td class="lbl">Unit Price</td><td class="prc">${money(sellPrice)}</td></tr>
        <tr class="subtotal"><td class="lbl">Total</td><td class="prc">${money(total)}</td></tr>
        <tr class="grand"><td class="lbl">GRAND TOTAL</td><td class="prc">${money(total)}</td></tr>`;
  }

  return `
    <table class="cmp-table">
      <colgroup><col class="col-label"><col></colgroup>
      <thead>
        <tr>
          <th class="corner">Specification</th>
          <th class="opt-head">Details</th>
        </tr>
      </thead>
      <tbody>
        <tr class="sec-div"><td colspan="2">&#9654; Product Details</td></tr>
        <tr class="desc-row"><td class="lbl">Description</td><td class="val">${esc(item.Description || item.ItemName || '-')}</td></tr>
        <tr><td class="lbl">Brand</td><td class="val">${esc(item.Brand || '-')}</td></tr>
        <tr><td class="lbl">Size</td><td class="val">${esc(item.Size || '-')}</td></tr>
        ${item.Country ? `<tr><td class="lbl">Country</td><td class="val">${esc(item.Country)}</td></tr>` : ''}
        ${item.Warranty ? `<tr><td class="lbl">Warranty</td><td class="val">${esc(item.Warranty)}</td></tr>` : ''}
        <tr><td class="lbl">Quantity</td><td class="val">${qty}</td></tr>
        <tr class="sec-div"><td colspan="2">&#9654; Pricing${includeVat ? ' (VAT Inclusive)' : ''}</td></tr>
        ${pricingRows}
      </tbody>
    </table>`;
}

// ── Services Table ──
function buildServicesTable(items: QuotationItem[]): string {
  let rows = '';
  let svcTotal = 0;

  items.forEach((svc, i) => {
    const qty = svc.Quantity || 1;
    const price = svc.UnitPrice || svc.Price || 0;
    const isFree = svc.isFOC || price === 0;
    const lineTotal = isFree ? 0 : qty * price;
    svcTotal += lineTotal;

    const freeBadge = '<span style="background:#27ae60;color:#fff;padding:2px 8px;border-radius:3px;font-weight:700;font-size:9px;">FREE</span>';

    rows += `<tr>
      <td class="center">${i + 1}</td>
      <td>${esc(svc.Description || svc.ItemName || '')}</td>
      <td class="center">${qty}</td>
      <td class="right">${isFree ? freeBadge : money(price)}</td>
      <td class="right">${isFree ? freeBadge : money(lineTotal)}</td>
    </tr>`;
  });

  return `
    <div class="svc-section-title">Additional Services</div>
    <table class="svc-table">
      <thead>
        <tr>
          <th style="width:40px">#</th>
          <th>Service Description</th>
          <th class="center" style="width:60px">Qty</th>
          <th class="right" style="width:100px">Unit Price</th>
          <th class="right" style="width:100px">Amount</th>
        </tr>
      </thead>
      <tbody>${rows}</tbody>
      <tfoot>
        <tr>
          <td colspan="4">Services Total</td>
          <td class="right">${money(svcTotal)}</td>
        </tr>
      </tfoot>
    </table>`;
}

// ─── Build Full HTML Document ───────────────────────────────────────────────
export function buildQuotationHtml(
  details: QuotationDetails,
  items: QuotationItem[],
  options: QuotationPdfOptions = {}
): string {
  const includeVat = options.includeVat === true;
  const vatRate = Number(options.vatRate ?? 0);
  const customerVatNo = options.customerVatNo || '';
  const warrantyInfo = options.warrantyInfo;

  const companyName = includeVat ? COMPANY.NAME : COMPANY.NAME_NONVAT;
  const today = details.date || new Date().toLocaleDateString('en-GB');

  // Valid until (7 days)
  const validDate = new Date();
  validDate.setDate(validDate.getDate() + 7);
  const validUntil = details.expiryDate || validDate.toLocaleDateString('en-GB');

  // Build items HTML
  const itemsHtml = buildItemsTableHtml(items, options);

  // Warranty terms
  const defaultWarrantyBullets = [
    'Company warranty only.',
    'No warranty for used, repaired, punctured, cut, or burst tires.',
    'Alignment is recommended every 5,000 km for better mileage.',
    'Damage from improper use, overloading, or incorrect air pressure is not covered.',
    'Customers must inspect tires upon purchase and raise concerns immediately.',
    'Returns or exchanges are only for unused tires with the receipt.',
  ];

  // Custom warranty box (if warrantyInfo provided)
  const customWarrantyHtml = warrantyInfo
    ? `<div class="custom-warranty">
        <span class="cw-icon">&#128737;</span>
        <strong>Warranty:</strong> ${esc(Number(warrantyInfo.km).toLocaleString())} km or ${esc(warrantyInfo.years)} Year${warrantyInfo.years !== '1' ? 's' : ''}
        (whichever comes first) against manufacturing defects.
      </div>`
    : '';

  // Cheque payable name
  const chequePayableTo = includeVat ? COMPANY.NAME : COMPANY.NAME_NONVAT;

  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>QUOTATION - ${esc(details.quotationNo)}</title>
  <style>
    :root {
      --primary: ${COMPANY.COLOR_PRIMARY};
      --primary-dark: #d35400;
      --secondary: ${COMPANY.COLOR_SECONDARY};
      --secondary-light: #34495e;
      --grey-bg: #f8f9fa;
      --border: #e9ecef;
      --text: #333;
      --text-light: #666;
      --text-muted: #999;
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: 'Segoe UI', 'Roboto', Arial, sans-serif;
      background: #fff;
      color: var(--text);
      -webkit-print-color-adjust: exact;
      print-color-adjust: exact;
    }
    .page {
      width: 794px;
      min-height: 1123px;
      margin: 0 auto;
      padding: 12px 16px 10px;
      position: relative;
    }
    .top-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 3px;
    }
    .top-bar h1 {
      color: var(--secondary);
      font-size: 20px;
      font-weight: 800;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      line-height: 1;
    }
    .top-bar .slogan {
      font-size: 8px;
      color: var(--primary);
      font-weight: 600;
      letter-spacing: 0.5px;
      margin-top: 1px;
    }
    .doc-badge {
      display: inline-block;
      color: var(--primary);
      border: 2px solid var(--primary);
      border-radius: 999px;
      padding: 3px 12px;
      font-size: 13px;
      font-weight: 800;
      letter-spacing: 1px;
      text-transform: uppercase;
      background: rgba(230,126,34,0.06);
    }

    .info-strip {
      display: flex;
      gap: 0;
      border: 1px solid #dce1e6;
      border-radius: 4px;
      overflow: hidden;
      margin-bottom: 6px;
    }
    .info-strip .info-panel {
      flex: 1;
      padding: 6px 8px;
      font-size: 8px;
      color: var(--text-light);
      line-height: 1.45;
    }
    .info-strip .info-panel:first-child {
      background: #fff;
      border-right: 2px solid var(--primary);
    }
    .info-strip .info-panel:last-child {
      background: var(--grey-bg);
    }
    .info-strip .panel-title {
      font-size: 7px;
      color: var(--text-muted);
      text-transform: uppercase;
      letter-spacing: 1.5px;
      font-weight: 700;
      margin-bottom: 3px;
    }
    .info-strip .panel-name {
      font-size: 11px;
      font-weight: 700;
      color: var(--secondary);
      margin-bottom: 2px;
    }
    .info-strip .info-row {
      display: flex;
      gap: 10px;
      flex-wrap: wrap;
      margin-top: 2px;
    }
    .doc-meta {
      display: flex;
      justify-content: flex-end;
      gap: 16px;
      font-size: 8.5px;
      color: var(--text-light);
      margin: -2px 0 5px;
      padding-bottom: 4px;
      border-bottom: 2px solid var(--primary);
    }
    .doc-meta strong { color: var(--text); }

    /* COMPARISON TABLE */
    .cmp-table { width: 100%; border-collapse: collapse; margin-bottom: 6px; border: 1px solid #dce1e6; }
    .cmp-table col.col-label { width: 140px; }
    .cmp-table thead th { padding: 6px 4px; font-size: 7.5px; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 700; }
    .cmp-table thead th.corner {
      background: linear-gradient(135deg, var(--secondary) 0%, var(--secondary-light) 100%);
      color: #fff; text-align: left; padding-left: 8px; font-size: 8px; letter-spacing: 1px;
    }
    .cmp-table thead th.opt-head {
      background: linear-gradient(135deg, var(--secondary) 0%, var(--secondary-light) 100%);
      color: #fff; text-align: center; border-left: 1px solid rgba(255,255,255,0.08);
    }
    .opt-pill {
      display: inline-flex; align-items: center; justify-content: center;
      background: var(--primary); color: #fff; width: 14px; height: 14px;
      border-radius: 50%; font-size: 7px; font-weight: 800; margin-right: 2px; line-height: 1;
    }
    .cmp-table tr.sec-div td {
      background: linear-gradient(135deg, var(--primary) 0%, var(--primary-dark) 100%);
      color: #fff; font-weight: 700; font-size: 6.5px; text-transform: uppercase;
      letter-spacing: 2px; padding: 3px 8px; border: none;
    }
    .cmp-table td.lbl {
      font-weight: 600; font-size: 7px; color: var(--secondary); padding: 4px 6px;
      background: #f5f7f9; border-right: 1px solid var(--border); border-bottom: 1px solid #eef0f2;
      text-transform: uppercase; letter-spacing: 0.3px; white-space: nowrap;
    }
    .cmp-table td.val {
      text-align: center; padding: 4px 3px; font-size: 7.5px;
      border-bottom: 1px solid #eef0f2; border-left: 1px solid #f3f4f6; color: var(--text); vertical-align: middle;
    }
    .cmp-table td.prc {
      text-align: right; padding: 4px 6px 4px 3px; font-size: 7.5px;
      border-bottom: 1px solid #eef0f2; border-left: 1px solid #f3f4f6;
      font-variant-numeric: tabular-nums; color: var(--text); vertical-align: middle;
    }
    .cmp-table td:nth-child(even) { background-color: rgba(44,62,80,0.018); }
    .cmp-table tr.sec-div td:nth-child(even), .cmp-table tr.grand td:nth-child(even) { background-color: transparent; }
    .cmp-table tr.desc-row td.val { font-size: 7px; font-weight: 500; line-height: 1.2; vertical-align: top; }
    .cmp-table tr.subtotal td { border-top: 1.5px solid #dce1e6; font-weight: 600; }
    .cmp-table tr.grand td {
      background: var(--secondary) !important; color: #fff !important;
      font-weight: 700; font-size: 8.5px; padding: 5px 6px;
      border-top: 3px solid var(--primary); border-bottom: none;
    }
    .cmp-table tr.grand td.lbl { background: var(--secondary) !important; color: #fff !important; font-size: 7.5px; }
    .cmp-table tr.grand td.prc { color: var(--primary) !important; font-weight: 800; font-size: 9px; }

    /* SERVICES TABLE */
    .svc-section-title {
      font-size: 7.5px; font-weight: 700; color: var(--secondary);
      text-transform: uppercase; letter-spacing: 1.5px; margin-bottom: 3px; padding-left: 2px;
    }
    .svc-table { width: 100%; border-collapse: collapse; margin-bottom: 8px; border: 1px solid #dce1e6; }
    .svc-table thead th {
      background: var(--secondary-light); color: #fff; padding: 4px 6px;
      font-size: 7px; text-transform: uppercase; letter-spacing: 0.5px; font-weight: 700; text-align: left;
    }
    .svc-table thead th.right { text-align: right; }
    .svc-table thead th.center { text-align: center; }
    .svc-table tbody td { padding: 4px 6px; font-size: 7.5px; border-bottom: 1px solid #eef0f2; color: var(--text); }
    .svc-table tbody td.center { text-align: center; }
    .svc-table tbody td.right { text-align: right; font-variant-numeric: tabular-nums; }
    .svc-table tbody tr:nth-child(even) { background: rgba(44,62,80,0.015); }
    .svc-table tfoot td {
      padding: 5px 6px; font-weight: 700; font-size: 8px;
      border-top: 2px solid var(--primary); color: var(--secondary);
    }
    .svc-table tfoot td.right { text-align: right; font-variant-numeric: tabular-nums; }

    /* NOTES */
    .notes-section {
      display: grid; grid-template-columns: 1fr 1fr; gap: 1px 10px;
      font-size: 7.5px; color: var(--text-light); line-height: 1.35;
      margin-bottom: 4px; padding: 4px 0; border-top: 1px solid var(--border);
    }
    .notes-section strong { color: var(--text); }
    .notes-full { grid-column: 1 / -1; margin-top: 2px; }

    /* CUSTOM WARRANTY */
    .custom-warranty {
      background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
      border: 1px solid #86efac;
      border-radius: 4px;
      padding: 6px 10px;
      margin-bottom: 6px;
      font-size: 8px;
      color: #166534;
      font-weight: 500;
    }
    .custom-warranty .cw-icon { font-size: 11px; margin-right: 3px; }
    .custom-warranty strong { color: #15803d; }

    /* WARRANTY */
    .warranty-box {
      background: var(--grey-bg); border-left: 4px solid var(--primary); border-radius: 4px;
      padding: 4px 8px; margin-bottom: 5px; font-size: 6.5px; color: #555; line-height: 1.2;
    }
    .warranty-box .title {
      font-weight: 800; font-size: 7.5px; color: var(--secondary);
      text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 2px;
    }
    .warranty-box ul { margin: 0; padding-left: 12px; }
    .warranty-box li { margin: 0 0 1px 0; }

    /* SIGNATURES */
    .sig-area { display: flex; justify-content: space-between; gap: 16px; margin: 6px 0 5px; }
    .sig-box { flex: 1; border-top: 1px solid #ccc; padding-top: 3px; font-size: 7.5px; color: var(--text-light); text-align: center; }

    /* FOOTER */
    .footer { border-top: 1px solid var(--border); padding-top: 4px; display: flex; justify-content: space-between; align-items: flex-end; }
    .thank-you { font-size: 9px; color: var(--primary); font-weight: 700; }
    .footer-right { text-align: right; font-size: 6.5px; color: var(--text-muted); line-height: 1.35; }

    @page { size: A4 portrait; margin: 0; }
    @media print {
      body { background: none; margin: 0; padding: 0; }
      .page { width: 100%; min-height: auto; margin: 0; padding: 12px 16px 10px; }
    }
  </style>
</head>
<body>
  <div class="page">
    <!-- TOP BAR -->
    <div class="top-bar">
      <div>
        <h1>${esc(companyName)}</h1>
        <div class="slogan">GO SMART WITH LASANTHA TYRE</div>
      </div>
      <span class="doc-badge">${includeVat ? 'TAX QUOTATION' : 'QUOTATION'}</span>
    </div>

    <!-- DOC META -->
    <div class="doc-meta">
      <span><strong>Ref:</strong> #${esc(details.quotationNo)}</span>
      <span><strong>Date:</strong> ${esc(today)}</span>
      <span><strong>Valid Until:</strong> ${esc(validUntil)}</span>
    </div>

    <!-- DUAL INFO STRIP -->
    <div class="info-strip">
      <div class="info-panel">
        <div class="panel-title">From</div>
        <div class="panel-name">${esc(companyName)}</div>
        <div>${esc(COMPANY.ADDRESS)}</div>
        <div>Tel: ${esc(COMPANY.PHONE)} | Mobile: ${esc(COMPANY.MOBILE)}</div>
        ${includeVat ? `<div style="margin-top:2px;"><strong style="color:var(--secondary);">VAT Reg No:</strong> ${esc(COMPANY.VAT_NO)}</div>` : ''}
        <div>Dealer for All Kind of Tyres, Tubes, Tapes and Alloy wheels.</div>
        <div>Nitrogen Air filling | Wheel Alignment &amp; Balancing Centre</div>
      </div>
      <div class="info-panel">
        <div class="panel-title">Bill To</div>
        <div class="panel-name">${esc(details.customerName || 'Cash Customer')}</div>
        <div class="info-row">
          <span><strong>Terms:</strong> ${esc(details.terms || 'Cash')}</span>
          ${details.vehicleNo ? `<span><strong>Vehicle:</strong> ${esc(details.vehicleNo)}</span>` : ''}
        </div>
        ${includeVat && customerVatNo ? `<div style="margin-top:2px;"><strong>Customer VAT No:</strong> ${esc(customerVatNo)}</div>` : ''}
      </div>
    </div>

    <!-- ITEMS TABLE -->
    ${itemsHtml}

    <!-- CUSTOM WARRANTY -->
    ${customWarrantyHtml}

    <!-- NOTES -->
    <div class="notes-section">
      <div><strong>Valid Period:</strong> 7 Days</div>
      <div><strong>Payment:</strong> ${esc(details.terms || 'Cash')}</div>
      <div class="notes-full"><strong>Cheque payable to:</strong> ${esc(chequePayableTo)}</div>
      <div class="notes-full" style="font-size:7px;">Above Quoted all Items (Tyres) are under Guarantee and Best Possible Rates. We Assured you the Best Service at all time (Open 365 days)</div>
    </div>

    <!-- WARRANTY -->
    <div class="warranty-box">
      <div class="title">Warranty &amp; Terms</div>
      <ul>
        ${defaultWarrantyBullets.map(b => `<li>${esc(b)}</li>`).join('\n        ')}
      </ul>
    </div>

    <!-- SIGNATURES -->
    <div class="sig-area">
      <div class="sig-box">Prepared By</div>
      <div class="sig-box">Sales Manager</div>
      <div class="sig-box">Customer Acceptance</div>
    </div>

    <!-- FOOTER -->
    <div class="footer">
      <div class="thank-you">Thank you for your business!</div>
      <div class="footer-right">
        Go smart with Lasantha Tyres<br>
        Generated by Lasantha Tyre Smart System<br>
        Powered by Snath Software Solution
      </div>
    </div>
  </div>
</body>
</html>`;
}
