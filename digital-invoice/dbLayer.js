const sql = require('mssql');
const config = require('./config');
const moment = require('moment');

class InvoiceDbLayer {
    constructor(pool) {
        this.pool = pool;
    }

    normalizeDocType(docType) {
        const raw = (docType === null || docType === undefined) ? '' : String(docType).trim();
        const upper = raw.toUpperCase();
        if (!upper) return 'AUTO';
        if (['AUTO', 'AUTODETECT', 'AUTO-DETECT', 'DETECT', 'UNKNOWN', 'ANY'].includes(upper)) return 'AUTO';
        if (['INV', 'INVOICE', 'TAXINVOICE', 'TAX INVOICE'].includes(upper)) return 'INVOICE';
        if (['QUO', 'QUOTE', 'QUOTATION', 'ESTIMATE'].includes(upper)) return 'QUOTATION';
        return upper;
    }

    quoteIdentifier(name) {
        const s = String(name || '').trim();
        if (!s) return s;
        if (s.startsWith('[') && s.endsWith(']')) return s;
        return `[${s.replace(/]/g, '')}]`;
    }

    async existsInView(viewName, refCol, refNumber) {
        const col = this.quoteIdentifier(refCol);
        const view = String(viewName);
        const r = await this.pool.request()
            .input('RefNo', sql.VarChar, refNumber)
            .query(`SELECT TOP (1) 1 AS Found FROM ${view} WHERE ${col} = @RefNo`);
        return !!(r.recordset && r.recordset.length);
    }

    async resolveDocumentType(refNumber) {
        const invRefCol = config.DB.COLS.INVOICE.REF_NO;
        const quoRefCol = config.DB.COLS.QUOTATION.REF_NO;

        const [inInvoice, inQuotation] = await Promise.all([
            this.existsInView(config.DB.VIEW_INVOICE, invRefCol, refNumber),
            this.existsInView(config.DB.VIEW_QUOTATION, quoRefCol, refNumber)
        ]);

        if (inInvoice && !inQuotation) return 'INVOICE';
        if (!inInvoice && inQuotation) return 'QUOTATION';
        if (inInvoice && inQuotation) {
            console.warn(`⚠️ [DigitalInvoice] Ref '${refNumber}' found in BOTH views; defaulting to INVOICE.`);
            return 'INVOICE';
        }

        throw new Error(`Ref '${refNumber}' not found in invoice/quotation views.`);
    }

    /**
     * 🚀 ATOMIC FETCH: Status = PENDING එකක් අරගෙන PROCESSING කරනවා
     */
    async fetchNextJob() {
        try {
            const result = await this.pool.request().query(`
                WITH CTE AS (
                    SELECT TOP (1) *
                    FROM ${config.DB.QUEUE_TABLE}
                    WHERE Status = 'PENDING'
                    ORDER BY ID ASC
                )
                UPDATE CTE
                SET 
                    Status = 'PROCESSING',
                    ProcessedDate = GETDATE()
                OUTPUT INSERTED.*
            `);

            return result.recordset[0] || null;
        } catch (error) {
            console.error('[DigitalInvoice] DB Fetch Error:', error.message);
            return null;
        }
    }

    /**
     * View එකෙන් දත්ත අරගෙන PDF Template එකට ගැලපෙන object එකක් හදනවා
     */
    async fetchDocumentData(docType, refNumber) {
        const normalizedDocType = this.normalizeDocType(docType);
        const resolvedDocType = (normalizedDocType === 'AUTO')
            ? await this.resolveDocumentType(refNumber)
            : normalizedDocType;

        const isInvoice = resolvedDocType === 'INVOICE';
        const viewName = isInvoice ? config.DB.VIEW_INVOICE : config.DB.VIEW_QUOTATION;
        const mapping = isInvoice ? config.DB.COLS.INVOICE : config.DB.COLS.QUOTATION;
        const refCol = this.quoteIdentifier(mapping.REF_NO);

        // Fetch Raw Data
        console.log(`🔍 [DigitalInvoice] Fetching ${resolvedDocType} data from ${viewName} for ${refNumber}`);
        const result = await this.pool.request()
            .input('RefNo', sql.VarChar, refNumber)
            .query(`SELECT * FROM ${viewName} WHERE ${refCol} = @RefNo`);

        if (!result.recordset.length) {
            throw new Error(`Data not found in view ${viewName} for Ref: ${refNumber}`);
        }

        const rows = result.recordset;
        const header = rows[0];

        const addDays = (dateObj, days) => {
            const d = new Date(dateObj.getTime());
            d.setDate(d.getDate() + days);
            return d;
        };

        const parseTyreSize = (value) => {
            const s = (value === null || value === undefined) ? '' : String(value);
            // Examples: 155/65/14/GDR/ZUPECO, 195/65R15 ...
            const m = s.match(/(\d{3}\/\d{2}R?\d{2}|\d{3}\/\d{2}\/\d{2})/i);
            return m ? m[1].toUpperCase() : '';
        };

        const safeText = (v, fallback = '') => {
            if (v === null || v === undefined) return fallback;
            const s = String(v).trim();
            return s ? s : fallback;
        };

        const toNum = (v) => {
            const n = Number(v);
            return Number.isFinite(n) ? n : 0;
        };

        const normalizeCompanyName = (rawName) => {
            const s = safeText(rawName, '');
            if (!s) return config.COMPANY.NAME;
            if (/\bNEW\b/i.test(s)) return 'NEW Lasantha Tyre Traders';
            return 'Lasantha Tyre Traders';
        };

        const companyDisplayName = isInvoice
            ? normalizeCompanyName(header.Location || header.WHID || '')
            : normalizeCompanyName(header.WarehouseID || '');

        // 🏗️ Construct Standardized Data Object
        const headerDate = header[mapping.DATE] ? new Date(header[mapping.DATE]) : new Date();
        const invoiceDateStr = moment(headerDate).format('YYYY-MM-DD');
        const quotationValidDays = (config.LEGAL && Number(config.LEGAL.QUOTATION_VALID_DAYS)) ? Number(config.LEGAL.QUOTATION_VALID_DAYS) : 7;
        const quotationValidUntilStr = moment(addDays(headerDate, quotationValidDays)).format('YYYY-MM-DD');

        const data = {
            metadata: {
                type: isInvoice ? 'INVOICE' : 'QUOTATION',
                color: config.COMPANY.COLOR_PRIMARY,
                legalNote: isInvoice
                    ? (config.LEGAL && config.LEGAL.INVOICE_NOTE) ? config.LEGAL.INVOICE_NOTE : ''
                    : (config.LEGAL && config.LEGAL.QUOTATION_NOTE) ? config.LEGAL.QUOTATION_NOTE : '',
                currencyLabel: (config.LEGAL && config.LEGAL.CURRENCY_LABEL) ? config.LEGAL.CURRENCY_LABEL : 'Rs.',
                vatRate: null,
                isVatInvoice: null,
                companyDisplayName
            },
            invoice: {
                number: header[mapping.REF_NO],
                date: invoiceDateStr,
                validUntil: isInvoice ? '' : quotationValidUntilStr,
                customerName: safeText(header.Expr10, safeText(header[mapping.CUSTOMER], 'Cash Customer')),
                customerAddress: isInvoice 
                    ? `${header[mapping.ADDRESS_1] || ''} ${header[mapping.ADDRESS_2] || ''}`.trim()
                    : `${header[mapping.ADDRESS_1] || ''} ${header[mapping.ADDRESS_2] || ''}`.trim(),
                customerTel: isInvoice ? '' : safeText(mapping.TEL ? header[mapping.TEL] : '', ''),
                customerFax: isInvoice ? '' : safeText(mapping.FAX ? header[mapping.FAX] : '', ''),
                vehicleNo: header[mapping.VEHICLE] || '-',
                mileage: isInvoice ? (header[mapping.MILEAGE] || '-') : '-',
                rep: header[mapping.REP] || 'General',
                paymentMethod: mapping.PAYMENT ? (header[mapping.PAYMENT] || '-') : '-',
                quotationValidPeriod: isInvoice ? '' : safeText(mapping.VALID_PERIOD ? header[mapping.VALID_PERIOD] : '', ''),
                deliveryPeriod: isInvoice ? '' : safeText(mapping.DELIVERY_PERIOD ? header[mapping.DELIVERY_PERIOD] : '', ''),
                paymentTerms: isInvoice ? '' : safeText(mapping.PAYMENT_TERMS ? header[mapping.PAYMENT_TERMS] : '', ''),
                availability: isInvoice ? '' : safeText(mapping.AVAILABILITY ? header[mapping.AVAILABILITY] : '', '')
            },
            items: rows.map((row, index) => {
                const qty = row[mapping.QTY] || 0;
                const priceIncl = row[mapping.PRICE] || 0;
                const totalIncl = row[mapping.AMOUNT] || (qty * priceIncl);

                // Quotation view provides ex-tax numbers (despite the names)
                const priceEx = (!isInvoice && mapping.PRICE_EX) ? (row[mapping.PRICE_EX] || 0) : 0;
                const totalEx = (!isInvoice && mapping.AMOUNT_EX) ? (row[mapping.AMOUNT_EX] || 0) : 0;
                const vatLine = (!isInvoice && totalEx) ? (toNum(totalIncl) - toNum(totalEx)) : 0;
                const vatRate = (!isInvoice && totalEx) ? ((vatLine / toNum(totalEx)) * 100) : 0;

                // Invoice view: try to compute ex-vat from InclusivePrice when present
                const invUnitEx = (isInvoice && row.InclusivePrice != null) ? toNum(row.InclusivePrice) : 0;
                const invLineEx = (isInvoice && invUnitEx) ? (invUnitEx * toNum(qty)) : 0;
                const invVatLine = (isInvoice && invLineEx) ? (toNum(totalIncl) - invLineEx) : 0;
                
                // Keep description clean (no redundant (Brand/Origin/Warranty) block)
                const desc = safeText(row[mapping.ITEM_DESC], '-');

                const itemCode = row[mapping.ITEM_CODE];
                const size = parseTyreSize(itemCode) || parseTyreSize(desc);
                const brandOrCategory = mapping.BRAND ? safeText(row[mapping.BRAND], '') : '';

                return {
                    index: index + 1,
                    code: safeText(itemCode, ''),
                    description: desc,
                    // Template expects 'quantity'; keep 'qty' for backward compatibility
                    quantity: qty,
                    qty: qty,
                    brand: brandOrCategory || '-',
                    size: size || '',
                    // Backward compatible fields (old templates)
                    price: this.formatCurrency(priceIncl),
                    total: this.formatCurrency(totalIncl),
                    rawTotal: toNum(totalIncl),

                    // Tax-quotation fields (match old Crystal layout)
                    unitPriceExVat: this.formatCurrency(isInvoice ? invUnitEx : priceEx),
                    lineTotalExVat: this.formatCurrency(isInvoice ? invLineEx : totalEx),
                    vatAmount: this.formatCurrency(isInvoice ? invVatLine : vatLine),
                    lineTotalInclVat: this.formatCurrency(totalIncl),
                    rawLineExVat: isInvoice ? invLineEx : toNum(totalEx),
                    rawVatAmount: isInvoice ? invVatLine : toNum(vatLine),
                    warranty: mapping.WARRANTY ? safeText(row[mapping.WARRANTY], '') : '' ,
                    _vatRate: isInvoice ? 0 : vatRate
                };
            }),
            totals: {
                // සරල එකතුව (View එකේ හැටියට discount වෙනස් වෙන්න පුළුවන්, දැනට සරල එකතුව ගනිමු)
                subtotal: '', // Will calculate below
                grandTotal: '',
                subtotalExVat: '',
                vatTotal: ''
            },
            company: config.COMPANY
        };

        // Printed timestamp (for footer)
        try {
            if (!isInvoice) {
                const d = headerDate;
                data.invoice.printedAt = moment(d).format('M/D/YYYY');
            }
        } catch {}

        // Calculate Totals
        if (!isInvoice) {
            const subEx = mapping.SUBTOTAL_EX ? toNum(header[mapping.SUBTOTAL_EX]) : 0;
            const vatTot = mapping.VAT_TOTAL ? toNum(header[mapping.VAT_TOTAL]) : 0;
            const grand = mapping.GRAND_TOTAL ? toNum(header[mapping.GRAND_TOTAL]) : 0;

            data.totals.subtotalExVat = this.formatCurrency(subEx);
            data.totals.vatTotal = this.formatCurrency(vatTot);
            data.totals.grandTotal = this.formatCurrency(grand);
            data.totals.subtotal = this.formatCurrency(grand);

            // Pick VAT rate from first non-zero line, fallback null
            const rate = data.items.map((i) => i._vatRate).find((r) => Number(r) > 0);
            data.metadata.vatRate = rate ? Math.round(rate) : null;
        } else {
            const isVatInvoice = (header.IsVATInvoice === true)
                || ((mapping.VAT_TOTAL && toNum(header[mapping.VAT_TOTAL]) > 0))
                || ((mapping.TAX_INVOICE_NO && safeText(header[mapping.TAX_INVOICE_NO], '') !== ''));

            // Invoice totals: prefer header-level tax totals if provided; fallback to sum of line totals.
            const baseTotal = mapping.BASE_TOTAL ? toNum(header[mapping.BASE_TOTAL]) : 0;
            const vatTotal = mapping.VAT_TOTAL ? toNum(header[mapping.VAT_TOTAL]) : 0;
            const vatRate = mapping.VAT_RATE ? toNum(header[mapping.VAT_RATE]) : 0;
            const vatRateFallback = mapping.VAT_RATE_FALLBACK ? toNum(header[mapping.VAT_RATE_FALLBACK]) : 0;
            const netTotal = toNum(header.NetTotal) || toNum(header.NetTotalAmount) || 0;

            const sumLines = data.items.reduce((sum, item) => sum + item.rawTotal, 0);
            const grand = netTotal || (baseTotal + vatTotal) || sumLines;

            // If header totals are missing, compute from line Ex/VAT fields
            const sumEx = data.items.reduce((sum, item) => sum + toNum(item.rawLineExVat), 0);
            const sumVat = data.items.reduce((sum, item) => sum + toNum(item.rawVatAmount), 0);

            data.totals.grandTotal = this.formatCurrency(grand);
            data.totals.subtotal = this.formatCurrency(grand);
            data.metadata.isVatInvoice = isVatInvoice;
            if (isVatInvoice) {
                data.totals.subtotalExVat = baseTotal ? this.formatCurrency(baseTotal) : (sumEx ? this.formatCurrency(sumEx) : '');
                data.totals.vatTotal = vatTotal ? this.formatCurrency(vatTotal) : (sumVat ? this.formatCurrency(sumVat) : '');
                data.metadata.vatRate = (vatRate || vatRateFallback) ? Math.round(vatRate || vatRateFallback) : null;
            } else {
                data.totals.subtotalExVat = '';
                data.totals.vatTotal = '';
                data.metadata.vatRate = null;
            }

            // Customer mobile from invoice view (if present)
            try {
                if (mapping.CUSTOMER_MOBILE && header[mapping.CUSTOMER_MOBILE]) {
                    data.invoice.customerMobile = safeText(header[mapping.CUSTOMER_MOBILE], '');
                }
            } catch {}

            // Invoice-side header fields (best-effort mapping)
            try {
                const poNo = safeText(header.CusPoNum || header.CustomerPO || '', '');
                data.invoice.poNo = poNo;
            } catch {}

            try {
                // Vehicle often comes through Expr8 in current view
                const vno = safeText(header.VehicleNo || header.Expr8 || '', '');
                data.invoice.vehicleNo = vno || '-';
            } catch {}

            try {
                const mr = safeText(header.Mileage || header.Expr9 || '', '');
                data.invoice.mileage = mr || '-';
            } catch {}

            try {
                data.invoice.jobDoneBy = safeText(header.JobDoneBy || '', '');
            } catch {}

            try {
                data.invoice.issueBy = safeText(header.Currentuser || header.UserID || header.Expr11 || '', '');
            } catch {}

            // Printed timestamp (for footer parity with Crystal)
            try {
                const d = header.CurrentDate ? new Date(header.CurrentDate) : headerDate;
                const datePart = moment(d).format('M/D/YYYY');
                const timePart = safeText(header.Time || '', '');
                data.invoice.printedAt = timePart ? `${datePart} ${timePart}` : datePart;
            } catch {}

            // Paid/balance
            try {
                const paid = toNum(header.PaidAmount);
                data.invoice.paidAmount = this.formatCurrency(paid);
                const bal = Math.max(0, grand - paid);
                data.invoice.balance = this.formatCurrency(bal);
            } catch {}

            // Customer VAT/TIN (if present)
            try {
                const tin = mapping.PURCHASER_TIN ? safeText(header[mapping.PURCHASER_TIN], '') : '';
                const fallbackTin = safeText(header.Comments || '', '');
                data.invoice.customerVat = tin || fallbackTin;
            } catch {}

            // Extra tax invoice identifiers (if any)
            try {
                data.invoice.taxInvoiceNo = mapping.TAX_INVOICE_NO ? safeText(header[mapping.TAX_INVOICE_NO], '') : '';
                data.invoice.supplierTin = mapping.SUPPLIER_TIN ? safeText(header[mapping.SUPPLIER_TIN], '') : '';
                data.invoice.purchaserTin = mapping.PURCHASER_TIN ? safeText(header[mapping.PURCHASER_TIN], '') : '';
                data.invoice.amountInWords = mapping.AMOUNT_IN_WORDS ? safeText(header[mapping.AMOUNT_IN_WORDS], '') : '';
            } catch {}
        }

        // Cleanup internal fields
        try {
            data.items.forEach((i) => {
                delete i._vatRate;
                delete i.rawLineExVat;
                delete i.rawVatAmount;
            });
        } catch {}

        return data;
    }

    async updateStatus(id, status, log) {
        await this.pool.request()
            .input('ID', sql.Int, id)
            .input('Status', sql.VarChar, status)
            .input('Log', sql.NVarChar, log)
            .query(`UPDATE ${config.DB.QUEUE_TABLE} SET Status = @Status, StatusMessage = @Log WHERE ID = @ID`);
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat('en-LK', { style: 'decimal', minimumFractionDigits: 2 }).format(amount);
    }
}

module.exports = InvoiceDbLayer;
