const puppeteer = require('puppeteer-core');
const fs = require('fs');
const path = require('path');
const config = require('./config');

// Find Chrome executable path
const findChrome = () => {
    const possiblePaths = [
        'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',
        'C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe',
        process.env.CHROME_PATH
    ].filter(Boolean);
    
    for (const p of possiblePaths) {
        if (fs.existsSync(p)) return p;
    }
    return null;
};

/**
 * 🖌️ THE ARTIST: High-End PDF Generator
 * Uses Headless Chrome to render HTML/CSS perfectly.
 */
class PdfGenerator {
    constructor() {
        this.browser = null;
    }

    async init() {
        if (!this.browser) {
            const chromePath = findChrome();
            if (!chromePath) {
                throw new Error('Chrome not found. Install Chrome or set CHROME_PATH env var.');
            }
            
            // Launch browser once and keep it warm (Performance Trick)
            this.browser = await puppeteer.launch({
                executablePath: chromePath,
                headless: 'new', // New faster headless mode
                args: ['--no-sandbox', '--disable-setuid-sandbox']
            });
            console.log('🎨 [PdfGenerator] Advanced Rendering Engine Ready.');
        }
    }

    async generate(data) {
        if (!this.browser) await this.init();

        const loadLogoBase64 = () => {
            try {
                const candidates = ['logo.png', 'logo.jpg', 'logo.jpeg'];
                for (const name of candidates) {
                    const p = path.join(config.ASSETS_PATH, name);
                    if (!fs.existsSync(p)) continue;
                    const buf = fs.readFileSync(p);
                    const ext = path.extname(p).toLowerCase();
                    const mime = (ext === '.jpg' || ext === '.jpeg') ? 'image/jpeg' : 'image/png';
                    return `data:${mime};base64,${buf.toString('base64')}`;
                }
            } catch {}
            return '';
        };

        const logoBase64 = loadLogoBase64();

        const safe = (v, fallback = '') => {
            if (v === null || v === undefined) return fallback;
            const s = String(v);
            return s.includes('undefined') ? fallback : s;
        };

        const BLANK_QR = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';

        // Normalize input so template never sees undefined
        const normalized = (() => {
            // New standardized shape from dbLayer.fetchDocumentData
            if (data && data.invoice && data.metadata) {
                return {
                    docType: data.metadata.type || 'INVOICE',
                    isVatInvoice: data.metadata.isVatInvoice === true,
                    logoBase64,
                    companyName: data.metadata.companyDisplayName || config.COMPANY.NAME,
                    chequeTo: data.metadata.companyDisplayName || config.COMPANY.NAME,
                    legalNote: data.metadata.legalNote || '',
                    currencyLabel: data.metadata.currencyLabel || 'Rs.',
                    vatRate: (data.metadata.vatRate !== null && data.metadata.vatRate !== undefined) ? data.metadata.vatRate : '',
                    invoiceNo: data.invoice.number || '',
                    date: data.invoice.date || '',
                    validUntil: data.invoice.validUntil || '',
                    customerName: data.invoice.customerName || 'Cash Customer',
                    customerMobile: data.invoice.customerMobile || data.customerMobile || '',
                    customerAddress: data.invoice.customerAddress || '',
                    customerTel: data.invoice.customerTel || '',
                    customerFax: data.invoice.customerFax || '',
                    customerVat: data.invoice.customerVat || data.invoice.purchaserTin || '',
                    vehicleNo: (data.invoice.vehicleNo && data.invoice.vehicleNo !== '-') ? data.invoice.vehicleNo : '',
                    salesRep: data.invoice.rep || 'General',
                    mileage: (data.invoice.mileage && data.invoice.mileage !== '-') ? data.invoice.mileage : '',
                    paymentMethod: data.invoice.paymentMethod || '-',
                    poNo: data.invoice.poNo || '',
                    jobDoneBy: data.invoice.jobDoneBy || '',
                    issueBy: data.invoice.issueBy || '',
                    paidAmount: data.invoice.paidAmount || '',
                    balance: data.invoice.balance || '',
                    supplierTin: data.invoice.supplierTin || '',
                    printedAt: data.invoice.printedAt || '',
                    quotationValidPeriod: data.invoice.quotationValidPeriod || '',
                    deliveryPeriod: data.invoice.deliveryPeriod || '',
                    paymentTerms: data.invoice.paymentTerms || '',
                    availability: data.invoice.availability || '',
                    subtotal: (data.totals && data.totals.subtotal) ? data.totals.subtotal : '0.00',
                    subtotalExVat: (data.totals && data.totals.subtotalExVat) ? data.totals.subtotalExVat : '',
                    vatTotal: (data.totals && data.totals.vatTotal) ? data.totals.vatTotal : '',
                    discount: (data.totals && data.totals.discount) ? data.totals.discount : 0,
                    total: (data.totals && (data.totals.grandTotal || data.totals.total)) ? (data.totals.grandTotal || data.totals.total) : '0.00',
                    qrCode: data.qrCode || BLANK_QR,
                    items: Array.isArray(data.items) ? data.items.map((it) => ({
                        code: it.code,
                        description: it.description,
                        brand: it.brand,
                        size: it.size,
                        quantity: (it.quantity !== undefined ? it.quantity : it.qty),
                        price: it.price,
                        total: it.total,
                        unitPriceExVat: it.unitPriceExVat,
                        vatAmount: it.vatAmount,
                        lineTotalInclVat: it.lineTotalInclVat,
                        warranty: it.warranty
                    })) : []
                };
            }
            // Old shape (manager.js)
            return {
                docType: data.docType || 'INVOICE',
                isVatInvoice: data.isVatInvoice === true,
                logoBase64,
                companyName: data.companyName || config.COMPANY.NAME,
                chequeTo: data.companyName || config.COMPANY.NAME,
                legalNote: data.legalNote || '',
                currencyLabel: data.currencyLabel || 'Rs.',
                vatRate: data.vatRate || '',
                invoiceNo: data.invoiceNo || '',
                date: data.date || '',
                validUntil: data.validUntil || '',
                customerName: data.customerName || 'Cash Customer',
                customerMobile: data.customerMobile || '',
                customerAddress: data.customerAddress || '',
                customerTel: data.customerTel || '',
                customerFax: data.customerFax || '',
                customerVat: data.customerVat || '',
                vehicleNo: data.vehicleNo || '',
                salesRep: data.salesRep || 'General',
                mileage: (data.mileage && data.mileage !== '-') ? data.mileage : '',
                paymentMethod: data.paymentMethod || '-',
                poNo: data.poNo || '',
                jobDoneBy: data.jobDoneBy || '',
                issueBy: data.issueBy || '',
                paidAmount: data.paidAmount || '',
                balance: data.balance || '',
                supplierTin: data.supplierTin || '',
                printedAt: data.printedAt || '',
                quotationValidPeriod: data.quotationValidPeriod || '',
                deliveryPeriod: data.deliveryPeriod || '',
                paymentTerms: data.paymentTerms || '',
                availability: data.availability || '',
                subtotal: data.subtotal || '0.00',
                subtotalExVat: data.subtotalExVat || '',
                vatTotal: data.vatTotal || '',
                discount: data.discount || 0,
                total: data.total || '0.00',
                qrCode: data.qrCode || BLANK_QR,
                items: Array.isArray(data.items) ? data.items : []
            };
        })();

        const docTypeUpper = String(normalized.docType || 'INVOICE').toUpperCase();
        const isVatInvoice = normalized.isVatInvoice === true;

        const vatTotalNum = (() => {
            const raw = String(normalized.vatTotal ?? '').replace(/,/g, '').trim();
            const n = parseFloat(raw);
            return Number.isFinite(n) ? n : NaN;
        })();
        const subtotalExVatNum = (() => {
            const raw = String(normalized.subtotalExVat ?? '').replace(/,/g, '').trim();
            const n = parseFloat(raw);
            return Number.isFinite(n) ? n : NaN;
        })();
        const hasVatNumbers = Number.isFinite(vatTotalNum) && vatTotalNum > 0 && Number.isFinite(subtotalExVatNum);

        // Quotations can have VAT breakdown even though they are not "VAT invoices".
        const hasVatBreakdown = (docTypeUpper === 'QUOTATION')
            ? hasVatNumbers
            : (isVatInvoice && hasVatNumbers);
        const showQuotationTotals = (docTypeUpper === 'QUOTATION') && Array.isArray(normalized.items) && normalized.items.length <= 1;
        const displayDocType = (docTypeUpper === 'QUOTATION' && hasVatBreakdown)
            ? 'TAX QUOTATION'
            : ((docTypeUpper === 'INVOICE' && hasVatBreakdown)
                ? 'TAX INVOICE'
                : safe(normalized.docType, 'INVOICE'));

        // Use a single invoice layout for both VAT and non-VAT invoices.
        const templatePath = (docTypeUpper === 'QUOTATION' && config.TEMPLATE_PATH_QUOTATION)
            ? config.TEMPLATE_PATH_QUOTATION
            : ((docTypeUpper === 'INVOICE' && config.TEMPLATE_PATH_TAX_INVOICE)
                ? config.TEMPLATE_PATH_TAX_INVOICE
                : (config.TEMPLATE_PATH_INVOICE || config.TEMPLATE_PATH));

        try {
            // 1. Read Template
            let html = fs.readFileSync(templatePath, 'utf8');

            const parseNumber = (v) => {
                if (v === null || v === undefined) return NaN;
                const s = String(v).replace(/,/g, '').trim();
                if (!s) return NaN;
                const n = parseFloat(s);
                return Number.isFinite(n) ? n : NaN;
            };

            const escapeHtml = (s) => String(s)
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;')
                .replace(/"/g, '&quot;')
                .replace(/'/g, '&#39;');

            const toHtmlMultiline = (s) => {
                const v = safe(s, '').trim();
                if (!v) return '';
                return escapeHtml(v).replace(/\r\n|\r|\n/g, '<br/>');
            };

            const toHtmlBulletList = (arr) => {
                if (!Array.isArray(arr) || arr.length === 0) return '';
                const items = arr
                    .map((x) => safe(x, '').trim())
                    .filter(Boolean)
                    .map((x) => `<li>${escapeHtml(x)}</li>`)
                    .join('');
                return items ? `<ul>${items}</ul>` : '';
            };

            const money = (n) => {
                if (!Number.isFinite(n)) return '';
                return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n);
            };

            // 2. Inject Data (Manual Templating for speed - faster than EJS/Handlebars)
            // Replace simple keys
            const warrantyTermsHtml = toHtmlBulletList(config.COMPANY.WARRANTY_TERMS_BULLETS)
                || toHtmlMultiline(config.COMPANY.WARRANTY_TERMS || '');
            html = html
                .replace(/{{company_name}}/g, safe(normalized.companyName || config.COMPANY.NAME, config.COMPANY.NAME))
                .replace(/{{company_address}}/g, config.COMPANY.ADDRESS)
                .replace(/{{company_phone}}/g, config.COMPANY.PHONE)
                .replace(/{{company_fax}}/g, safe(config.COMPANY.FAX || '', ''))
                .replace(/{{company_email}}/g, config.COMPANY.EMAIL)
                .replace(/{{company_web}}/g, safe(config.COMPANY.WEB || '', ''))
                .replace(/{{company_br}}/g, safe(config.COMPANY.BR_NO || '', ''))
                .replace(/{{company_vat}}/g, safe(config.COMPANY.VAT_NO || normalized.supplierTin || '', ''))
                .replace(/{{company_svat}}/g, safe(config.COMPANY.SVAT_NO || '', ''))
                .replace(/{{color_primary}}/g, config.COMPANY.COLOR_PRIMARY)
                .replace(/{{color_secondary}}/g, config.COMPANY.COLOR_SECONDARY)
                .replace(/{{logo_base64}}/g, safe(normalized.logoBase64 || '', ''))
                
                .replace(/{{doc_type}}/g, displayDocType)
                .replace(/{{inv_number}}/g, safe(normalized.invoiceNo, ''))
                .replace(/{{inv_date}}/g, safe(normalized.date, ''))
                .replace(/{{valid_until}}/g, safe(normalized.validUntil, ''))
                
                .replace(/{{customer_name}}/g, safe(normalized.customerName, 'Cash Customer'))
                .replace(/{{customer_mobile}}/g, safe(normalized.customerMobile, ''))
                .replace(/{{customer_address}}/g, safe(normalized.customerAddress, ''))
                .replace(/{{customer_tel}}/g, safe(normalized.customerTel, ''))
                .replace(/{{customer_fax}}/g, safe(normalized.customerFax, ''))
                .replace(/{{customer_vat}}/g, safe(normalized.customerVat, ''))

                .replace(/{{sales_rep}}/g, safe(normalized.salesRep, 'General'))
                .replace(/{{mileage}}/g, safe(normalized.mileage, '-'))
                .replace(/{{payment_method}}/g, safe(normalized.paymentMethod, '-'))
                .replace(/{{po_no}}/g, safe(normalized.poNo, ''))
                .replace(/{{job_done_by}}/g, safe(normalized.jobDoneBy, ''))
                .replace(/{{issue_by}}/g, safe(normalized.issueBy, ''))
                .replace(/{{paid_amount}}/g, safe(normalized.paidAmount, ''))
                .replace(/{{balance}}/g, safe(normalized.balance, ''))
                .replace(/{{printed_at}}/g, safe(normalized.printedAt, ''))
                .replace(/{{cheque_to}}/g, safe(normalized.chequeTo, ''))
                .replace(/{{valid_period}}/g, safe(normalized.quotationValidPeriod, ''))
                .replace(/{{delivery_period}}/g, safe(normalized.deliveryPeriod, ''))
                .replace(/{{payment_terms}}/g, safe(normalized.paymentTerms, ''))
                .replace(/{{availability}}/g, safe(normalized.availability, ''))
                .replace(/{{vat_rate}}/g, safe(normalized.vatRate, ''))
                
                .replace(/{{subtotal}}/g, safe(normalized.subtotal, '0.00'))
                .replace(/{{subtotal_ex_vat}}/g, safe(normalized.subtotalExVat, ''))
                .replace(/{{vat_total}}/g, safe(normalized.vatTotal, ''))
                .replace(/{{final_total}}/g, safe(normalized.total, '0.00'))
                .replace(/{{currency_label}}/g, safe(normalized.currencyLabel, 'Rs.'))
                .replace(/{{legal_note}}/g, safe(normalized.legalNote, ''))
                .replace(/{{warranty_terms_html}}/g, safe(warrantyTermsHtml, ''))
                .replace(/{{qr_code_base64}}/g, safe(normalized.qrCode, BLANK_QR));

            // Doc-type conditional blocks
            if (docTypeUpper === 'INVOICE') {
                html = html.replace(/{{#invoice_only}}([\s\S]*?){{\/invoice_only}}/g, (m, p1) => p1);
                html = html.replace(/{{#quotation_only}}[\s\S]*?{{\/quotation_only}}/g, '');
            } else {
                html = html.replace(/{{#quotation_only}}([\s\S]*?){{\/quotation_only}}/g, (m, p1) => p1);
                html = html.replace(/{{#invoice_only}}[\s\S]*?{{\/invoice_only}}/g, '');
            }

            // Optional validity block (mostly for quotation)
            if (normalized.validUntil) {
                html = html.replace(/{{#valid_until}}([\s\S]*?){{\/valid_until}}/g, (m, p1) => p1.replace(/{{valid_until}}/g, safe(normalized.validUntil, '')));
            } else {
                html = html.replace(/{{#valid_until}}[\s\S]*?{{\/valid_until}}/g, '');
            }

            // VAT breakdown block (mostly for tax invoices/quotations)
            if (hasVatBreakdown) {
                html = html.replace(/{{#vat_breakdown}}([\s\S]*?){{\/vat_breakdown}}/g, (m, p1) => p1);
            } else {
                html = html.replace(/{{#vat_breakdown}}[\s\S]*?{{\/vat_breakdown}}/g, '');
            }

            // Inverse VAT block (handy for non-VAT invoices using same layout)
            if (!hasVatBreakdown) {
                html = html.replace(/{{#no_vat_breakdown}}([\s\S]*?){{\/no_vat_breakdown}}/g, (m, p1) => p1);
            } else {
                html = html.replace(/{{#no_vat_breakdown}}[\s\S]*?{{\/no_vat_breakdown}}/g, '');
            }

            // Quotation totals block (hide totals when multiple option items exist)
            if (showQuotationTotals) {
                html = html.replace(/{{#quotation_totals}}([\s\S]*?){{\/quotation_totals}}/g, (m, p1) => p1);
            } else {
                html = html.replace(/{{#quotation_totals}}[\s\S]*?{{\/quotation_totals}}/g, '');
            }

            // VAT rate block (inline)
            if (normalized.vatRate !== '' && normalized.vatRate !== null && normalized.vatRate !== undefined) {
                html = html.replace(/{{#vat_rate}}([\s\S]*?){{\/vat_rate}}/g, (m, p1) => p1.replace(/{{vat_rate}}/g, safe(normalized.vatRate, '')));
            } else {
                html = html.replace(/{{#vat_rate}}[\s\S]*?{{\/vat_rate}}/g, '');
            }

            // Optional fields: po/job/issue/customer vat
            const blocks = [
                ['po_no', normalized.poNo],
                ['job_done_by', normalized.jobDoneBy],
                ['issue_by', normalized.issueBy],
                ['customer_vat', normalized.customerVat],
                ['customer_fax', normalized.customerFax],
                ['company_fax', config.COMPANY.FAX || ''],
                ['mileage', normalized.mileage],
                ['logo', normalized.logoBase64],
                ['warranty_terms', warrantyTermsHtml]
            ];
            for (const [key, value] of blocks) {
                const re = new RegExp(`{{#${key}}}([\\s\\S]*?){{\\/${key}}}`, 'g');
                const reEmpty = new RegExp(`{{#${key}}}[\\s\\S]*?{{\\/${key}}}`, 'g');
                if (value) html = html.replace(re, (m, p1) => p1);
                else html = html.replace(reEmpty, '');
            }

            // Paid/balance block
            const hasPaid = !!(normalized.paidAmount || normalized.balance);
            if (hasPaid) {
                html = html.replace(/{{#paid_balance}}([\s\S]*?){{\/paid_balance}}/g, (m, p1) => p1);
            } else {
                html = html.replace(/{{#paid_balance}}[\s\S]*?{{\/paid_balance}}/g, '');
            }

            // Handle Vehicle No special logic
            if (normalized.vehicleNo) {
                html = html.replace(/{{#vehicle_no}}([\s\S]*?){{\/vehicle_no}}/g, (match, p1) => {
                    return p1.replace(/{{vehicle_no}}/g, safe(normalized.vehicleNo, ''));
                });
            } else {
                html = html.replace(/{{#vehicle_no}}[\s\S]*?{{\/vehicle_no}}/g, '');
            }
            
            // Handle Discount logic
            if (Number(normalized.discount) > 0) {
                 html = html.replace(/{{#discount}}([\s\S]*?){{\/discount}}/g, (match, p1) => {
                    return p1.replace(/{{discount}}/g, safe(normalized.discount, '0.00'));
                });
            } else {
                html = html.replace(/{{#discount}}[\s\S]*?{{\/discount}}/g, '');
            }

            // 3. Render Items Loop
            const itemsHtmlStart = html.indexOf('{{#items}}');
            const itemsHtmlEnd = html.indexOf('{{/items}}');
            
            if (itemsHtmlStart !== -1 && itemsHtmlEnd !== -1) {
                const itemTemplate = html.substring(itemsHtmlStart + 10, itemsHtmlEnd);
                let allItemsStr = '';
                
                normalized.items.forEach((item, index) => {
                    const computedUnitExVat = safe(item.unitPriceExVat, safe(item.price, ''));
                    const computedTotalExVat = (() => {
                        const existing = safe(item.lineTotalExVat, '').trim();
                        if (existing) return existing;
                        const qty = parseNumber(item.quantity);
                        const unit = parseNumber(computedUnitExVat);
                        if (Number.isFinite(qty) && Number.isFinite(unit)) return money(qty * unit);
                        const fallbackTotal = parseNumber(item.total);
                        if (Number.isFinite(fallbackTotal)) return money(fallbackTotal);
                        return '';
                    })();

                    let row = itemTemplate
                        .replace(/{{index}}/g, index + 1)
                        .replace(/{{item_code}}/g, safe(item.code, ''))
                        .replace(/{{description}}/g, safe(item.description, '-'))
                        .replace(/{{brand}}/g, safe(item.brand, '-'))
                        .replace(/{{quantity}}/g, safe(item.quantity, ''))
                        .replace(/{{price}}/g, safe(item.price, '0.00'))
                        .replace(/{{total}}/g, safe(item.total, '0.00'))
                        // Tax quotation fields (safe even if template doesn't use them)
                        .replace(/{{price_ex_vat}}/g, safe(computedTotalExVat, ''))  // Changed: Show total (qty × unit price) instead of unit price
                        .replace(/{{total_ex_vat}}/g, safe(computedTotalExVat, ''))
                        .replace(/{{vat_amount}}/g, safe(item.vatAmount, ''))
                        .replace(/{{total_incl_vat}}/g, safe(item.lineTotalInclVat, safe(item.total, '0.00')))
                        .replace(/{{warranty}}/g, safe(item.warranty, ''));
                        
                    // Size handling
                    if (item.size) {
                         row = row.replace(/{{#size}}([\s\S]*?){{\/size}}/g, (match, p1) => {
                             return p1.replace(/{{size}}/g, safe(item.size, ''));
                         });
                    } else {
                        row = row.replace(/{{#size}}[\s\S]*?{{\/size}}/g, '');
                    }
                    
                    allItemsStr += row;
                });
                
                html = html.substring(0, itemsHtmlStart) + allItemsStr + html.substring(itemsHtmlEnd + 10);
            }

            // 4. Create Page & Print
            const page = await this.browser.newPage();
            await page.setContent(html, { waitUntil: 'networkidle0' });
            
            const pdfBuffer = await page.pdf({
                format: 'A4',
                printBackground: true, // Important for colors
                margin: { top: '0px', bottom: '0px', left: '0px', right: '0px' } // Reset margins, handled by CSS
            });

            await page.close();
            return pdfBuffer;

        } catch (error) {
            console.error('🎨 PDF Generation Failed:', error);
            throw error;
        }
    }
}

module.exports = new PdfGenerator();
