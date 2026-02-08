/**
 * Email Service - Send quotations and notifications via email
 * Uses Nodemailer with Gmail SMTP
 */

const nodemailer = require('nodemailer');
require('dotenv').config();

class EmailService {
    constructor() {
        this.transporter = null;
        this.initialized = false;
    }

    async initialize() {
        if (this.initialized) return;

        try {
            const emailProvider = (process.env.EMAIL_PROVIDER || 'gmail').toLowerCase();
            
            let transportConfig;
            
            if (emailProvider === 'zoho') {
                // Zoho Mail Configuration
                transportConfig = {
                    host: 'smtp.zoho.com',
                    port: 465,
                    secure: true, // use SSL
                    auth: {
                        user: process.env.EMAIL_USER || '',
                        pass: process.env.EMAIL_PASSWORD || ''
                    }
                };
                console.log('[EmailService] Using Zoho Mail');
            } else {
                // Gmail Configuration (default)
                transportConfig = {
                    service: 'gmail',
                    auth: {
                        user: process.env.EMAIL_USER || '',
                        pass: process.env.EMAIL_PASSWORD || ''
                    }
                };
                console.log('[EmailService] Using Gmail');
            }

            // Create transporter
            this.transporter = nodemailer.createTransport(transportConfig);

            // Verify connection
            if (process.env.EMAIL_USER && process.env.EMAIL_PASSWORD) {
                await this.transporter.verify();
                this.initialized = true;
                console.log('[EmailService] Initialized successfully');
            } else {
                console.warn('[EmailService] Email credentials not configured');
            }
        } catch (error) {
            console.error('[EmailService] Initialization failed:', error.message);
        }
    }

    async sendQuotationEmail(quotation) {
        if (!this.initialized) {
            console.warn('[EmailService] Not initialized - skipping email');
            return { success: false, error: 'Email service not configured' };
        }

        try {
            const { customerName, customerEmail, quotationNumber, items, totalAmount, bookingUrl, expiryDate } = quotation;

            if (!customerEmail || !customerEmail.includes('@')) {
                return { success: false, error: 'Invalid email address' };
            }

            // Build HTML email
            let itemsHtml = '<table style="width: 100%; border-collapse: collapse;">';
            itemsHtml += '<tr style="background-color: #f4f4f4;"><th style="padding: 8px; text-align: left;">Item</th><th style="padding: 8px; text-align: right;">Qty</th><th style="padding: 8px; text-align: right;">Price</th><th style="padding: 8px; text-align: right;">Total</th></tr>';

            items.forEach(item => {
                const total = item.price * item.quantity;
                itemsHtml += `
                    <tr style="border-bottom: 1px solid #ddd;">
                        <td style="padding: 8px;">${item.description}<br/><small style="color: #666;">${item.brand}</small></td>
                        <td style="padding: 8px; text-align: right;">${item.quantity}</td>
                        <td style="padding: 8px; text-align: right;">Rs ${item.price.toLocaleString()}</td>
                        <td style="padding: 8px; text-align: right;">Rs ${total.toLocaleString()}</td>
                    </tr>
                `;
            });

            itemsHtml += `
                <tr style="font-weight: bold; background-color: #f4f4f4;">
                    <td colspan="3" style="padding: 12px; text-align: right;">Total:</td>
                    <td style="padding: 12px; text-align: right;">Rs ${totalAmount.toLocaleString()}</td>
                </tr>
            </table>`;

            const htmlContent = `
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }
                        .content { background: white; padding: 30px; border: 1px solid #e0e0e0; border-radius: 0 0 8px 8px; }
                        .quotation-number { font-size: 24px; font-weight: bold; color: #667eea; margin: 20px 0; }
                        .button { display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 6px; margin: 20px 0; }
                        .footer { text-align: center; margin-top: 30px; color: #666; font-size: 12px; }
                    </style>
                </head>
                <body>
                    <div class="container">
                        <div class="header">
                            <h1>🛞 Lasantha Tire Service</h1>
                            <p>Your Professional Tire & Service Center</p>
                        </div>
                        <div class="content">
                            <p>Dear ${customerName},</p>
                            <p>Thank you for your interest! Here's your quotation:</p>
                            
                            <div class="quotation-number">
                                📋 Quotation: ${quotationNumber}
                            </div>

                            ${itemsHtml}

                            ${expiryDate ? `<p style="margin-top: 20px; color: #e67e22;"><strong>⏰ Valid Until:</strong> ${new Date(expiryDate).toLocaleDateString()}</p>` : ''}

                            <div style="text-align: center; margin: 30px 0;">
                                <a href="${bookingUrl}" class="button">📅 Book Your Appointment</a>
                            </div>

                            <p><strong>Why Choose Lasantha Tire?</strong></p>
                            <ul>
                                <li>✅ Computerized Wheel Alignment</li>
                                <li>✅ Professional Installation</li>
                                <li>✅ Quality Guaranteed</li>
                                <li>✅ Competitive Prices</li>
                            </ul>

                            <p>If you have any questions, feel free to contact us!</p>
                        </div>
                        <div class="footer">
                            <p>Lasantha Tire Service | Homagama, Sri Lanka</p>
                            <p>This is an automated email. Please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>
            `;

            const mailOptions = {
                from: `"Lasantha Tire Service" <${process.env.EMAIL_USER}>`,
                to: customerEmail,
                subject: `Your Quotation ${quotationNumber} - Lasantha Tire`,
                html: htmlContent
            };

            const info = await this.transporter.sendMail(mailOptions);
            console.log(`[EmailService] Quotation email sent to ${customerEmail} - ID: ${info.messageId}`);

            return {
                success: true,
                messageId: info.messageId
            };

        } catch (error) {
            console.error('[EmailService] Send email error:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async sendInvoiceEmail({ customerEmail, customerName, invoiceNumber, docType, pdfPath, totalAmount, items }) {
        if (!this.initialized) {
            console.warn('[EmailService] Not initialized - skipping invoice email');
            return { success: false, error: 'Email service not configured' };
        }

        try {
            if (!customerEmail || !customerEmail.includes('@')) {
                return { success: false, error: 'Invalid email address' };
            }

            const documentTitle = docType || 'Document';
            const isQuotation = (documentTitle === 'QUOTATION' || documentTitle === 'TAX QUOTATION');
            const subject = `${documentTitle} ${invoiceNumber} - ${process.env.STORE_NAME || 'Lasantha Tyre Traders'}`;
            
            // Generate Royal Booking link for quotations
            const royalBookingUrl = process.env.ROYAL_BOOKING_URL || 'http://localhost:3005';
            const bookingLink = isQuotation ? `${royalBookingUrl}/?ref=${encodeURIComponent(invoiceNumber)}` : null;

            // Build items table if provided
            // Classic email: no item price chart in body, concise professional text
            const itemsHtml = '';
            
            // Format total with commas
            const formattedTotal = totalAmount ? totalAmount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0.00';
            const todayDate = new Date().toISOString().split('T')[0];

            const htmlContent = `
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap');
                        * { margin: 0; padding: 0; box-sizing: border-box; }
                        body { font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #374151; background: #f3f4f6; }
                        .email-wrapper { max-width: 640px; margin: 20px auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1); }
                        
                        /* Header */
                        .header { background: linear-gradient(135deg, #0f172a 0%, #1e40af 100%); color: #ffffff; padding: 48px 40px; text-align: center; position: relative; }
                        .header-logo { font-size: 32px; font-weight: 800; letter-spacing: -0.025em; margin-bottom: 8px; color: #ffffff; text-shadow: 0 2px 4px rgba(0,0,0,0.1); }
                        .header-subtitle { font-size: 14px; color: #bfdbfe; font-weight: 500; letter-spacing: 0.05em; text-transform: uppercase; }
                        
                        /* Content */
                        .content { padding: 48px 40px; }
                        .greeting { font-size: 18px; font-weight: 600; color: #111827; margin-bottom: 24px; }
                        
                        /* Text Highlighting */
                        .doc-highlight { color: #2563eb; font-weight: 700; }

                        /* Primary Message */
                        .message-box { margin-bottom: 32px; color: #4b5563; font-size: 15px; }
                        
                        /* CTA Section - Advanced */
                        .cta-container { text-align: center; margin: 40px 0; position: relative; }
                        .cta-card { background: linear-gradient(to right, #f8fafc, #f1f5f9); border: 2px dashed #cbd5e1; border-radius: 16px; padding: 32px; text-align: center; }
                        .cta-icon { font-size: 32px; margin-bottom: 16px; display: block; }
                        .cta-heading { font-size: 18px; font-weight: 700; color: #1e293b; margin-bottom: 8px; }
                        .cta-text { font-size: 14px; color: #64748b; margin-bottom: 24px; max-width: 400px; margin-left: auto; margin-right: auto; }
                        
                        .cta-button { display: inline-flex; align-items: center; justify-content: center; background: #2563eb; color: #ffffff !important; padding: 16px 40px; border-radius: 12px; text-decoration: none; font-weight: 600; font-size: 16px; transition: all 0.2s ease; box-shadow: 0 10px 15px -3px rgba(37, 99, 235, 0.3); }
                        .cta-button:hover { background: #1d4ed8; transform: translateY(-2px); box-shadow: 0 20px 25px -5px rgba(37, 99, 235, 0.4); }
                        
                        /* Attachment */
                        .attachment-pill { display: inline-flex; align-items: center; background: #fff7ed; color: #9a3412; padding: 10px 20px; border-radius: 9999px; font-size: 13px; font-weight: 600; border: 1px solid #ffedd5; margin-top: 10px; }
                        
                        /* Services Grid */
                        .services-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 16px; margin-top: 48px; padding-top: 48px; border-top: 1px solid #e5e7eb; }
                        .service-item { display: flex; align-items: flex-start; }
                        .service-icon { background: #eff6ff; color: #2563eb; width: 36px; height: 36px; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 18px; margin-right: 12px; flex-shrink: 0; }
                        .service-info h4 { font-size: 14px; font-weight: 700; color: #1e293b; margin-bottom: 2px; }
                        .service-info p { font-size: 12px; color: #64748b; line-height: 1.4; }

                        /* Footer */
                        .footer { background: #111827; padding: 48px 40px; text-align: center; border-radius: 0 0 16px 16px; }
                        .footer-brand { font-size: 16px; font-weight: 700; color: #ffffff; letter-spacing: -0.01em; margin-bottom: 24px; }
                        .footer-links { display: flex; justify-content: center; gap: 24px; margin-bottom: 32px; flex-wrap: wrap; }
                        .footer-link { color: #9ca3af; text-decoration: none; font-size: 13px; font-weight: 500; display: flex; align-items: center; }
                        .footer-link span { margin-left: 8px; }
                        .hotline-badge { background: #dc2626; color: white; padding: 4px 10px; border-radius: 4px; font-size: 12px; font-weight: 700; margin-left: 8px; }
                        .copyright { color: #4b5563; font-size: 12px; border-top: 1px solid #1f2937; padding-top: 24px; }
                        
                        @media (max-width: 600px) {
                            .email-wrapper { margin: 0; border-radius: 0; }
                            .header, .content, .footer { padding: 32px 24px; }
                            .services-grid { grid-template-columns: 1fr; }
                        }
                    </style>
                </head>
                <body>
                    <div class="email-wrapper">
                        <!-- Modern Header -->
                        <div class="header">
                            <div class="header-logo">Lasantha Tyre Traders</div>
                            <div class="header-subtitle">Premium Automotive Solutions</div>
                        </div>
                        
                        <div class="content">
                            <!-- Greeting -->
                            <div class="greeting">Hello ${customerName || 'Valued Customer'},</div>
                            
                            <!-- Primary Message -->
                            <div class="message-box">
                                <p>Thank you for choosing <strong>${process.env.STORE_NAME || 'Lasantha Tyre Traders'}</strong>.</p>
                                <p style="margin-top: 12px;">
                                    Please find attached the <strong>${documentTitle.toLowerCase()}</strong> (<span class="doc-highlight">#${invoiceNumber}</span>) for your reference.
                                    The document containing complete pricing and item specifications is available in the attached PDF file.
                                </p>
                                
                                <div style="margin-top: 24px;">
                                    <div class="attachment-pill">
                                        📄 Attached: ${invoiceNumber}.pdf
                                    </div>
                                </div>
                            </div>
                            
                            <!-- DYNAMIC CTA: Only for Quotations -->
                            ${isQuotation ? `
                            <div class="cta-container">
                                <div class="cta-card">
                                    <span class="cta-icon">📅</span>
                                    <div class="cta-heading">Ready to Proceed?</div>
                                    <p class="cta-text">Skip the manual work. Your quotation details are already connected to our booking system.</p>
                                    <a href="${bookingLink}" class="cta-button">
                                        Book Appointment Now
                                    </a>
                                </div>
                            </div>
                            ` : ''}
                            
                            <!-- Trust Indicators Grid -->
                            <div class="services-grid">
                                <div class="service-item">
                                    <div class="service-icon">🔧</div>
                                    <div class="service-info">
                                        <h4>Computerized Alignment</h4>
                                        <p>Precision 3D alignment technology</p>
                                    </div>
                                </div>
                                <div class="service-item">
                                    <div class="service-icon">🏆</div>
                                    <div class="service-info">
                                        <h4>Expert Installation</h4>
                                        <p>Certified professional technicians</p>
                                    </div>
                                </div>
                                <div class="service-item">
                                    <div class="service-icon">💎</div>
                                    <div class="service-info">
                                        <h4>Quality Guarantee</h4>
                                        <p>100% authentic products</p>
                                    </div>
                                </div>
                                <div class="service-item">
                                    <div class="service-icon">⚡</div>
                                    <div class="service-info">
                                        <h4>Fast Service</h4>
                                        <p>Quick turnaround time guaranteed</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Premium Footer -->
                        <div class="footer">
                            <div class="footer-brand">${process.env.STORE_NAME || 'Lasantha Tyre Traders'}</div>
                            <div class="footer-links">
                                <a href="#" class="footer-link">
                                    📍 Data: ${process.env.STORE_LOCATION || 'Thalawathugoda'}
                                </a>
                                <a href="tel:0773131883" class="footer-link" style="color: #ffffff;">
                                    📞 Hotline <span class="hotline-badge">0773131883</span>
                                </a>
                            </div>
                            <div class="footer-links">
                                <a href="mailto:${process.env.EMAIL_USER || 'info@lasanthatyre.com'}" class="footer-link">
                                    ✉️ ${process.env.EMAIL_USER || 'info@lasanthatyre.com'}
                                </a>
                            </div>
                            <div class="copyright">
                                © ${new Date().getFullYear()} ${process.env.STORE_NAME || 'Lasantha Tyre Traders'}. All rights reserved.<br>
                                This is an automated notification. Please do not reply directly to this email.
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            `;

            const mailOptions = {
                from: `"${process.env.EMAIL_FROM_NAME || 'Lasantha Tyre Traders'}" <${process.env.EMAIL_USER}>`,
                to: customerEmail,
                subject: subject,
                html: htmlContent,
                attachments: [{
                    filename: `${invoiceNumber}.pdf`,
                    path: pdfPath
                }]
            };

            const info = await this.transporter.sendMail(mailOptions);
            console.log(`[EmailService] ${documentTitle} email sent to ${customerEmail} - ID: ${info.messageId}`);

            return {
                success: true,
                messageId: info.messageId
            };

        } catch (error) {
            console.error('[EmailService] Send invoice email error:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async sendBookingConfirmation(booking) {
        if (!this.initialized) return { success: false };

        try {
            const { customerName, customerEmail, bookingRef, appointmentDate, timeSlot, serviceType } = booking;

            if (!customerEmail || !customerEmail.includes('@')) {
                return { success: false, error: 'Invalid email' };
            }

            const htmlContent = `
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }
                        .content { background: white; padding: 30px; border: 1px solid #e0e0e0; }
                        .booking-box { background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; }
                    </style>
                </head>
                <body>
                    <div class="container">
                        <div class="header">
                            <h1>✅ Booking Confirmed!</h1>
                        </div>
                        <div class="content">
                            <p>Dear ${customerName},</p>
                            <p>Your appointment has been confirmed!</p>
                            
                            <div class="booking-box">
                                <h3>📅 Booking Details</h3>
                                <p><strong>Reference:</strong> ${bookingRef}</p>
                                <p><strong>Service:</strong> ${serviceType}</p>
                                <p><strong>Date:</strong> ${appointmentDate}</p>
                                <p><strong>Time:</strong> ${timeSlot}</p>
                            </div>

                            <p>We look forward to serving you!</p>
                            <p><strong>Lasantha Tire Service</strong><br/>Homagama, Sri Lanka</p>
                        </div>
                    </div>
                </body>
                </html>
            `;

            const mailOptions = {
                from: `"Lasantha Tire Service" <${process.env.EMAIL_USER}>`,
                to: customerEmail,
                subject: `Booking Confirmed - ${bookingRef}`,
                html: htmlContent
            };

            const info = await this.transporter.sendMail(mailOptions);
            return { success: true, messageId: info.messageId };

        } catch (error) {
            console.error('[EmailService] Booking email error:', error);
            return { success: false, error: error.message };
        }
    }
}

// Export the class for instantiation
module.exports = EmailService;
