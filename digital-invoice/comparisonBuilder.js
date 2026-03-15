/**
 * 🔀 Comparison Table Builder for Quotation PDFs
 * 
 * Converts a flat array of quotation line items into a side-by-side
 * comparison HTML table (like the PowerShell QuotationPdfGenerator).
 * 
 * Each item = one "option column" in the comparison table.
 * Rows show Description, Brand, Country, Warranty, Qty, and pricing.
 */

const money = (n) => {
    if (!Number.isFinite(n)) return '';
    return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n);
};

const esc = (s) => String(s || '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');

const toNum = (v) => {
    if (v === null || v === undefined) return 0;
    const s = String(v).replace(/,/g, '').trim();
    const n = parseFloat(s);
    return Number.isFinite(n) ? n : 0;
};

/**
 * Test if any item in the array has a non-empty value for a given field
 */
function hasAnyValue(items, field) {
    return items.some(it => {
        const v = it[field];
        return v !== null && v !== undefined && String(v).trim() !== '' && String(v).trim() !== '-';
    });
}

/**
 * Test if any item has a numeric value > 0 for a given field
 */
function hasAnyNumericValue(items, field) {
    return items.some(it => toNum(it[field]) > 0);
}

/**
 * Build a comparison row (label + one cell per option)
 * @param {string} label - Row label
 * @param {Array} items - Array of option items
 * @param {string} field - Field name to pick from each item
 * @param {boolean} isDesc - If true, add desc-row class
 * @param {string} cssClass - CSS class for value cells (default: 'val')
 */
function buildComparisonRow(label, items, field, isDesc = false) {
    const trClass = isDesc ? ' class="desc-row"' : '';
    let html = `                <tr${trClass}>\n`;
    html += `                    <td class="lbl">${esc(label)}</td>\n`;
    for (const item of items) {
        const val = item[field];
        const display = (val !== null && val !== undefined && String(val).trim() !== '') 
            ? esc(String(val).trim()) 
            : '-';
        html += `                    <td class="val">${display}</td>\n`;
    }
    html += `                </tr>\n`;
    return html;
}

/**
 * Build a pricing row (label + monetary values per option)
 */
function buildPricingRow(label, items, field, extraClass = '') {
    const trClass = extraClass ? ` class="${extraClass}"` : '';
    let html = `                <tr${trClass}>\n`;
    html += `                    <td class="lbl">${esc(label)}</td>\n`;
    for (const item of items) {
        const val = toNum(item[field]);
        html += `                    <td class="prc">${val > 0 ? money(val) : '-'}</td>\n`;
    }
    html += `                </tr>\n`;
    return html;
}

/**
 * Build a subtotal pricing row (with subtotal class)
 */
function buildSubtotalRow(label, items, field) {
    return buildPricingRow(label, items, field, 'subtotal');
}

/**
 * Build discount row (only if any discounts exist)
 */
function buildDiscountRow(items) {
    let html = `                <tr class="disc">\n`;
    html += `                    <td class="lbl">Discount</td>\n`;
    for (const item of items) {
        const disc = toNum(item._rawDiscount);
        html += `                    <td class="prc">${disc > 0 ? '-' + money(disc) : '-'}</td>\n`;
    }
    html += `                </tr>\n`;
    return html;
}

/**
 * Build computed unit price row (Amount / Qty)
 */
function buildCalculatedUnitPriceRow(label, items) {
    let html = `                <tr>\n`;
    html += `                    <td class="lbl">${esc(label)}</td>\n`;
    for (const item of items) {
        const amount = toNum(item._rawTotal);
        const qty = toNum(item.quantity || item.qty);
        const unit = (qty > 0) ? amount / qty : amount;
        html += `                    <td class="prc">${unit > 0 ? money(unit) : '-'}</td>\n`;
    }
    html += `                </tr>\n`;
    return html;
}

/**
 * Build inclusive unit price row for VAT mode
 */
function buildInclUnitPriceRow(label, items) {
    let html = `                <tr>\n`;
    html += `                    <td class="lbl">${esc(label)}</td>\n`;
    for (const item of items) {
        const totalIncl = toNum(item._rawTotalIncl);
        const qty = toNum(item.quantity || item.qty);
        const unit = (qty > 0) ? totalIncl / qty : totalIncl;
        html += `                    <td class="prc">${unit > 0 ? money(unit) : '-'}</td>\n`;
    }
    html += `                </tr>\n`;
    return html;
}

/**
 * Build service HTML section
 * Handles: WAC/WBC/N2Rate from tyre rows + ItemClass=7 service rows
 * 
 * @param {Array} tyreItems - Tyre items (to extract WAC/WBC/N2Rate)
 * @param {Array} serviceItems - ItemClass=7 service rows
 * @param {boolean} hasVat - Whether VAT mode
 * @param {number} vatRate - VAT rate (e.g. 18)
 * @returns {string} HTML for services section
 */
function buildServicesSection(tyreItems, serviceItems, hasVat, vatRate = 18) {
    const services = [];
    const freeBadge = '<span style="background:#27ae60;color:#fff;padding:2px 8px;border-radius:3px;font-weight:700;font-size:9px;letter-spacing:0.5px;">FREE</span>';

    // Total tyre qty across all options
    const totalTyreQty = tyreItems.reduce((sum, it) => sum + toNum(it.quantity || it.qty), 0);
    const totalFocQty = tyreItems.reduce((sum, it) => sum + toNum(it.foc), 0);

    // Check WAC/WBC/N2Rate from first tyre row (header-level fields)
    if (tyreItems.length > 0) {
        const first = tyreItems[0];

        // Wheel Alignment
        const wac = String(first.wac || '').trim();
        if (wac) {
            const wacAmt = toNum(wac);
            if (wacAmt > 0) {
                services.push({ description: 'Wheel Alignment - Computerized', qty: 1, unitPrice: wacAmt, isFree: false });
            } else if (totalFocQty > 0) {
                services.push({ description: 'Wheel Alignment - Computerized', qty: 1, unitPrice: 0, isFree: true });
            }
        }

        // Wheel Balancing
        const wbc = String(first.wbc || '').trim();
        if (wbc) {
            const wbcAmt = toNum(wbc);
            if (wbcAmt > 0) {
                services.push({ description: 'Wheel Balancing', qty: totalTyreQty, unitPrice: wbcAmt, isFree: false });
            } else if (totalFocQty > 0) {
                services.push({ description: 'Wheel Balancing', qty: totalTyreQty, unitPrice: 0, isFree: true });
            }
        }

        // Nitrogen Air Filling
        const n2 = String(first.n2Rate || '').trim();
        if (n2) {
            const n2Amt = toNum(n2);
            if (n2Amt > 0) {
                services.push({ description: 'Nitrogen Air Filling', qty: totalTyreQty, unitPrice: n2Amt, isFree: false });
            } else if (totalFocQty > 0) {
                services.push({ description: 'Nitrogen Air Filling', qty: totalTyreQty, unitPrice: 0, isFree: true });
            }
        }
    }

    // Add ItemClass=7 service rows from the DB
    for (const svc of serviceItems) {
        const desc = String(svc.description || '').trim();
        if (!desc) continue;

        const qty = toNum(svc.quantity || svc.qty) || 1;
        const amount = toNum(svc.rawTotal || svc.total);
        const focQty = toNum(svc.foc);
        const isFree = (amount === 0 && focQty > 0);
        const unitPrice = (amount > 0 && qty > 0) ? Math.round((amount / qty) * 100) / 100 : 0;

        // Avoid duplicating services we already added from WAC/WBC/N2Rate
        const descLower = desc.toLowerCase();
        const isDuplicate = services.some(s => {
            const sLower = s.description.toLowerCase();
            return (descLower.includes('alignment') && sLower.includes('alignment'))
                || (descLower.includes('balancing') && sLower.includes('balancing'))
                || (descLower.includes('nitrogen') && sLower.includes('nitrogen'));
        });
        if (isDuplicate) continue;

        services.push({ description: desc, qty, unitPrice, isFree });
    }

    if (services.length === 0) return '';

    // Build HTML
    let html = '';
    html += '        <div class="svc-section-title">Additional Services</div>\n';
    html += '        <table class="svc-table">\n';
    html += '            <thead>\n';
    html += '                <tr>\n';
    html += '                    <th style="width:40px">#</th>\n';
    html += '                    <th>Service Description</th>\n';
    html += '                    <th class="center" style="width:60px">Qty</th>\n';

    if (hasVat) {
        html += '                    <th class="right" style="width:100px">Unit Price (Excl. VAT)</th>\n';
        html += `                    <th class="right" style="width:80px">VAT (${vatRate}%)</th>\n`;
    } else {
        html += '                    <th class="right" style="width:100px">Unit Price</th>\n';
    }

    html += '                    <th class="right" style="width:100px">Amount</th>\n';
    html += '                </tr>\n';
    html += '            </thead>\n';
    html += '            <tbody>\n';

    let svcTotal = 0;
    let svcVatTotal = 0;

    services.forEach((svc, i) => {
        let lineTotal = svc.qty * svc.unitPrice;
        let lineVat = 0;
        let exclUnitPrice = svc.unitPrice;

        if (svc.isFree) {
            lineTotal = 0;
            lineVat = 0;
            exclUnitPrice = 0;
        } else if (hasVat) {
            exclUnitPrice = Math.round(svc.unitPrice * 100 / (100 + vatRate) * 100) / 100;
            lineVat = Math.round((svc.unitPrice - exclUnitPrice) * svc.qty * 100) / 100;
        }

        svcTotal += lineTotal;
        svcVatTotal += lineVat;

        html += '                <tr>\n';
        html += `                    <td class="center">${i + 1}</td>\n`;
        html += `                    <td>${esc(svc.description)}</td>\n`;
        html += `                    <td class="center">${svc.qty}</td>\n`;

        if (hasVat) {
            const exclCell = svc.isFree ? freeBadge : money(exclUnitPrice);
            const vatCell = svc.isFree ? '-' : money(lineVat);
            html += `                    <td class="right">${exclCell}</td>\n`;
            html += `                    <td class="right">${vatCell}</td>\n`;
        } else {
            const upCell = svc.isFree ? freeBadge : money(svc.unitPrice);
            html += `                    <td class="right">${upCell}</td>\n`;
        }

        const amtCell = svc.isFree ? freeBadge : money(lineTotal);
        html += `                    <td class="right">${amtCell}</td>\n`;
        html += '                </tr>\n';
    });

    html += '            </tbody>\n';
    html += '            <tfoot>\n';
    html += '                <tr>\n';
    const footerColSpan = hasVat ? 5 : 4;
    html += `                    <td colspan="${footerColSpan}">Services Total</td>\n`;
    html += `                    <td class="right">${money(svcTotal)}</td>\n`;
    html += '                </tr>\n';
    html += '            </tfoot>\n';
    html += '        </table>\n';

    return html;
}

/**
 * 🏗️ Main: Build the full comparison table HTML from items array
 * 
 * @param {Array} items - Array of quotation items (each = one option column)
 * @param {boolean} hasVat - Whether this is a VAT quotation
 * @param {number|null} vatRate - VAT rate percentage (e.g. 18)
 * @returns {string} - Full comparison table HTML
 */
function buildComparisonTableHtml(items, hasVat, vatRate) {
    if (!Array.isArray(items) || items.length === 0) {
        return '<p style="text-align:center; color:#999; padding:20px;">No items found</p>';
    }

    const optionCount = items.length;
    const totalCols = optionCount + 1; // label col + option cols

    // Enrich items with raw numeric fields for pricing calculations
    const enriched = items.map(it => {
        const rawPrice = toNum(it.price);
        const rawTotal = toNum(it.total);
        const qty = toNum(it.quantity || it.qty);
        const rawExVat = toNum(it.unitPriceExVat);
        const rawLineExVat = toNum(it.lineTotalExVat || it.rawLineExVat);
        const rawVat = toNum(it.vatAmount || it.rawVatAmount);
        const rawTotalIncl = toNum(it.lineTotalInclVat) || rawTotal;
        const rawDiscount = toNum(it.discount || it._rawDiscount);

        return {
            ...it,
            _rawPrice: rawPrice,
            _rawTotal: rawTotal,
            _rawExVat: rawExVat,
            _rawLineExVat: rawLineExVat,
            _rawVat: rawVat,
            _rawTotalIncl: rawTotalIncl,
            _rawDiscount: rawDiscount,
            _rawGrossAmount: rawLineExVat || (rawExVat * qty) || 0
        };
    });

    // Column tags and headers
    let colTags = '';
    let optHeaders = '';
    for (let i = 0; i < optionCount; i++) {
        colTags += `                <col>\n`;
        optHeaders += `                    <th class="opt-head"><span class="opt-pill">${i + 1}</span> Option ${i + 1}</th>\n`;
    }

    // === PRODUCT DETAIL ROWS ===
    let productRows = '';
    productRows += buildComparisonRow('Description', enriched, 'description', true);
    productRows += buildComparisonRow('Brand', enriched, 'brand', false);

    if (hasAnyValue(enriched, 'country')) {
        productRows += buildComparisonRow('Country of Origin', enriched, 'country', false);
    }
    if (hasAnyValue(enriched, 'plyRate')) {
        productRows += buildComparisonRow('Ply Rate', enriched, 'plyRate', false);
    }
    if (hasAnyValue(enriched, 'warranty')) {
        productRows += buildComparisonRow('Warranty', enriched, 'warranty', false);
    }
    if (hasAnyValue(enriched, 'yom')) {
        productRows += buildComparisonRow('YOM', enriched, 'yom', false);
    }
    if (hasAnyValue(enriched, 'lsSymbol')) {
        productRows += buildComparisonRow('L&S Symbol', enriched, 'lsSymbol', false);
    }
    if (hasAnyValue(enriched, 'size')) {
        productRows += buildComparisonRow('Size', enriched, 'size', false);
    }
    productRows += buildComparisonRow('Quantity', enriched, 'quantity', false);

    // === PRICING ROWS ===
    let pricingRows = '';
    let pricingTitle, grandTotalLabel;

    if (hasVat) {
        pricingTitle = 'Pricing Breakdown (VAT Inclusive)';
        grandTotalLabel = 'GRAND TOTAL (INCL. VAT)';

        // Unit Price (Excl. VAT)
        pricingRows += buildPricingRow('Unit Price (Excl. VAT)', enriched, '_rawExVat');
        // Subtotal (Excl. VAT)
        pricingRows += buildPricingRow('Subtotal (Excl. VAT)', enriched, '_rawGrossAmount');
        // VAT
        pricingRows += buildPricingRow(`VAT (${vatRate || 18}%)`, enriched, '_rawVat');
        // Unit Price (Incl. VAT)
        pricingRows += buildInclUnitPriceRow('Unit Price (Incl. VAT)', enriched);

        // Discount (if any)
        if (hasAnyNumericValue(enriched, '_rawDiscount')) {
            pricingRows += buildDiscountRow(enriched);
        }

        // Total (Incl. VAT)
        pricingRows += buildSubtotalRow('Total (Incl. VAT)', enriched, '_rawTotalIncl');
    } else {
        pricingTitle = 'Pricing';
        grandTotalLabel = 'GRAND TOTAL';

        // Unit Price
        pricingRows += buildCalculatedUnitPriceRow('Unit Price', enriched);

        // Discount (if any)
        if (hasAnyNumericValue(enriched, '_rawDiscount')) {
            pricingRows += buildDiscountRow(enriched);
        }

        // Total
        pricingRows += buildSubtotalRow('Total', enriched, '_rawTotal');
    }

    // === GRAND TOTAL CELLS ===
    let grandTotalCells = '';
    for (const item of enriched) {
        let grandTotal = item._rawTotalIncl || item._rawTotal;
        if (item._rawDiscount > 0) {
            grandTotal -= item._rawDiscount;
        }
        grandTotalCells += `                    <td class="prc">${money(grandTotal)}</td>\n`;
    }

    // === ASSEMBLE TABLE ===
    let html = '';
    html += `        <table class="cmp-table">\n`;
    html += `            <colgroup>\n`;
    html += `                <col class="col-label">\n`;
    html += colTags;
    html += `            </colgroup>\n`;
    html += `            <thead>\n`;
    html += `                <tr>\n`;
    html += `                    <th class="corner">Specification</th>\n`;
    html += optHeaders;
    html += `                </tr>\n`;
    html += `            </thead>\n`;
    html += `            <tbody>\n`;

    // Product Details section
    html += `                <!-- Product Details -->\n`;
    html += `                <tr class="sec-div">\n`;
    html += `                    <td colspan="${totalCols}">&#9654; Product Details</td>\n`;
    html += `                </tr>\n`;
    html += productRows;

    // Pricing Breakdown section
    html += `                <!-- Pricing Breakdown -->\n`;
    html += `                <tr class="sec-div">\n`;
    html += `                    <td colspan="${totalCols}">&#9654; ${esc(pricingTitle)}</td>\n`;
    html += `                </tr>\n`;
    html += pricingRows;

    // Grand Total row
    html += `                <!-- Grand Total -->\n`;
    html += `                <tr class="grand">\n`;
    html += `                    <td class="lbl">${esc(grandTotalLabel)}</td>\n`;
    html += grandTotalCells;
    html += `                </tr>\n`;

    html += `            </tbody>\n`;
    html += `        </table>\n`;

    return html;
}

module.exports = { buildComparisonTableHtml, buildServicesSection };
