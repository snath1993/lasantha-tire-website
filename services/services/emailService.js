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

// Export singleton instance
const emailService = new EmailService();
module.exports = emailService;
