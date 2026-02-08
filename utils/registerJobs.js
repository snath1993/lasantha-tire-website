/**
 * Job Registration - Register all existing jobs with the new system
 * This file bridges the legacy job system with the new advanced job handling
 */

const { registry } = require('./JobRegistry');

// Import existing jobs
const TyrePriceReplyJob = require('../jobs/TyrePriceReplyJob');
const TyreQtyReplyJob = require('../jobs/TyreQtyReplyJob');
const VehicleInvoiceReplyJob = require('../jobs/VehicleInvoiceReplyJob');
const CostPriceReplyJob = require('../jobs/CostPriceReplyJob');
const TyreQuotationPDFLibJob = require('../jobs/TyreQuotationPDFLibJob');
const TyrePhotoReplyJob = require('../jobs/TyrePhotoReplyJob');
const safeReply = require('./safeReply');

/**
 * Register all existing jobs
 */
function registerAllJobs() {
    // Job 0: Tyre Photo Reply (handle before price to catch explicit photo asks)
    registry.register({
        name: 'TyrePhotoReply',
        description: 'Send tyre photo for requested size/brand',
        priority: 85, // HIGH priority - handle photo requests before qty/price
        patterns: [
            /\d{3}\/\d{2}[R\/]\d{2}/i,           // tyre size
            /(photo|image|පින්තූර|පින්තූරය|ඡායාරූප|pic|එවන්න)/i // photo keywords
        ],
        entityExtractors: {
            tyreSize: 'tyreSize',
            brand: 'brand',
            intent: 'intent'
        },
        // Require tyreSize and photo intent
        requiresEntities: ['tyreSize', 'intent'],
        canHandlePartial: true,
        estimatedResponseTime: 900,
        handler: async (msg, deps) => {
            return await TyrePhotoReplyJob(msg, deps);
        }
    });
    // Job 1: Tyre Price Reply
    registry.register({
        name: 'TyrePriceReply',
        description: 'Provide tyre pricing information with business logic',
        priority: 50, // Lower priority - let AI handle with better context
        patterns: [
            /\d{3}\/\d{2}[R\/]\d{2}/i, // Tyre size pattern
            /price|ගාන|කියනවද/i
        ],
        entityExtractors: {
            tyreSize: 'tyreSize',
            brand: 'brand',
            quantity: 'quantity',
            intent: 'intent'
        },
        requiresEntities: ['tyreSize'],
        canHandlePartial: true,
        estimatedResponseTime: 800,
        handler: async (msg, deps) => {
            const { sql, sqlConfig, allowedContacts, logAndSave } = deps;
            return await TyrePriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
        }
    });

    // Job 2: Tyre Quantity Reply
    registry.register({
        name: 'TyreQtyReply',
        description: 'Provide stock quantity information',
        priority: 75,
        patterns: [
            /\d{3}\/\d{2}[R\/]\d{2}/i,
            /qty|quantity|stock|තියෙනවද|ඇත්තද/i
        ],
        entityExtractors: {
            tyreSize: 'tyreSize',
            brand: 'brand',
            intent: 'intent'
        },
        requiresEntities: ['tyreSize'],
        canHandlePartial: false,
        estimatedResponseTime: 600,
        handler: async (msg, deps) => {
            const { sql, sqlConfig, allowedContacts, logAndSave } = deps;
            return await TyreQtyReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
        }
    });

    // Job 3: Vehicle Invoice Reply
    registry.register({
        name: 'VehicleInvoiceReply',
        description: 'Provide vehicle purchase history and invoice details',
        priority: 90, // Very high priority for vehicle numbers
        patterns: [
            /[A-Z]{2,3}[-\s]?\d{4}/i, // Vehicle number pattern
            /vehicle|වාහන|invoice/i
        ],
        entityExtractors: {
            vehicleNumber: 'vehicleNumber'
        },
        requiresEntities: ['vehicleNumber'],
        canHandlePartial: false,
        estimatedResponseTime: 700,
        handler: async (msg, deps) => {
            const { sql, sqlConfig, allowedContacts, logAndSave } = deps;
            return await VehicleInvoiceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
        }
    });

    // Job 4: Cost Price Reply (Admin Only)
    registry.register({
        name: 'CostPriceReply',
        description: 'Provide cost pricing for admin/allowed contacts',
        priority: 75, // Lower than TyrePriceReply (80)
        patterns: [
            /\d{3}\/\d{2}[R\/]\d{2}/i,
            /\bcost\b|\bcostprice\b/i  // Only match "cost" as whole word
        ],
        entityExtractors: {
            tyreSize: 'tyreSize',
            brand: 'brand',
            intent: 'intent'
        },
        requiresEntities: ['tyreSize'],
        canHandlePartial: false,
        estimatedResponseTime: 600,
        handler: async (msg, deps) => {
            const { sql, sqlConfig, allowedContacts, logAndSave, entities } = deps;
            
            // Only handle if explicitly asking for cost
            if (!entities.intent || entities.intent !== 'cost') {
                return false; // Let TyrePriceReply handle it
            }
            
            return await CostPriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave);
        }
    });

    // Job 5: Quotation PDF Generation
    registry.register({
        name: 'TyreQuotationPDF',
        description: 'Generate and send quotation PDF',
        priority: 70,
        patterns: [
            /\d{3}\/\d{2}[R\/]\d{2}/i,
            /quotation|quote|ඇස්තමේන්තුව|estimate/i
        ],
        entityExtractors: {
            tyreSize: 'tyreSize',
            quantity: 'quantity'
        },
        requiresEntities: ['tyreSize'],
        canHandlePartial: true,
        estimatedResponseTime: 2000,
        handler: async (msg, deps) => {
            const { sql, sqlConfig, logAndSave } = deps;
            return await TyreQuotationPDFLibJob(msg, sql, sqlConfig, logAndSave);
        }
    });

    // Job 6: Contact Information (Template)
    registry.register({
        name: 'ContactInfo',
        description: 'Provide contact information',
        priority: 50,
        patterns: [
            /\b(contact|phone|call|අමතන්න|දුරකථන|number|නම්බර්)\b/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: false, // Changed to false - require explicit match
        estimatedResponseTime: 100,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            const contactMsg = `📞 *Lasantha Tyre Contact Information*\n\n` +
                `*Main Office:* 077 731 1770\n` +
                `*Sales:* 077 707 8700\n` +
                `*WhatsApp:* Same numbers\n\n` +
                `*Address:* [Your Address]\n` +
                `*Hours:* Mon-Sat: 8:30 AM - 6:00 PM`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, contactMsg);
            if (logAndSave) {
                logAndSave(`[ContactInfo] Sent contact info to ${msg.from}`);
            }
            return true;
        }
    });

    // Job 7: Location Information (Template)
    registry.register({
        name: 'LocationInfo',
        description: 'Provide location and directions',
        priority: 50,
        patterns: [
            /location|address|directions|පිහිටීම|ස්ථානය/i,
            /where|කොහෙද|කොහෙන්ද/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: true,
        estimatedResponseTime: 150,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            const locationMsg = `📍 *Lasantha Tyre Traders - Location*\n\n` +
                `*Address:*\n1035 Pannipitiya Road,\nBattaramulla 10230,\nSri Lanka\n\n` +
                `*Google Maps:*\nhttps://maps.app.goo.gl/frJHWEo4oRYYiJqw9\n\n` +
                `*Landmarks:*\n- Opposite Glomark Supermarket (Thalawathugoda branch)\n- Near Hiru TV Studio\n- On Pannipitiya Road, Battaramulla side\n\n` +
                `*Contact:*\n📞 0112 773 232\n📱 077 731 1770\n\n` +
                `*Business Hours:*\nMonday - Saturday: 8:00 AM - 9:00 PM\nSunday: Closed\n\n` +
                `⭐ Rated 4.3 stars (589+ reviews)\n\n` +
                `_We're open now! Call for quick service._`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, locationMsg);
            if (logAndSave) {
                logAndSave(`[LocationInfo] Sent location info to ${msg.from}`);
            }
            return true;
        }
    });

    // Job 8: Warranty Information
    registry.register({
        name: 'WarrantyInfo',
        description: 'Provide warranty terms and conditions',
        priority: 60,
        patterns: [
            /warranty|වොරන්ටි|වරන්ටි|ගැරන්ටි/i,
            /guarantee|ගැකැවීම/i,
            /defect|දෝෂ/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: false, // Changed from true to false - only trigger on explicit warranty keywords
        estimatedResponseTime: 150,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            const warrantyMsg = `🛡️ *Lasantha Tyre Traders - Warranty Policy*\n\n` +
                `*Coverage:*\n` +
                `✅ 40,000 - 50,000 km OR 4 years from billing date\n` +
                `   (Whichever comes first)\n\n` +
                `*What's Covered:*\n` +
                `✅ Manufacturing defects only\n` +
                `   - Tread separation\n` +
                `   - Sidewall cracks (factory fault)\n` +
                `   - Material/workmanship issues\n\n` +
                `*What's NOT Covered:*\n` +
                `❌ Road damage (potholes, curbs)\n` +
                `❌ Punctures or cuts\n` +
                `❌ Accidents or misuse\n` +
                `❌ Normal wear and tear\n` +
                `❌ Uneven wear (due to misalignment)\n\n` +
                `*Important Notes:*\n` +
                `⚠️ Warranty is handled by the tyre importer\n` +
                `📄 Original invoice/bill is REQUIRED for claims\n` +
                `🔧 Regular maintenance helps maximize tyre life:\n` +
                `   • Check air pressure weekly\n` +
                `   • Rotate tyres every 10,000 km\n` +
                `   • Get wheel alignment checked\n` +
                `   • Avoid overloading\n\n` +
                `_කරුණාකර බිල්පත නිසි ලෙස තබා ගන්න!_\n\n` +
                `📞 For warranty claims: 0112 773 232`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, warrantyMsg);
            if (logAndSave) {
                logAndSave(`[WarrantyInfo] Sent warranty info to ${msg.from}`);
            }
            return true;
        }
    });

    // Job 10: Business Hours (Template)
    registry.register({
        name: 'BusinessHours',
        description: 'Provide business hours information',
        priority: 50,
        patterns: [
            /hours|time|open|close|වෙලාව|විවෘත/i,
            /available|ඇත්තද/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: true,
        estimatedResponseTime: 100,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            const hoursMsg = `🕐 *Lasantha Tyre Business Hours*\n\n` +
                `*Weekdays:* Monday - Friday\n` +
                `⏰ 8:30 AM - 6:00 PM\n\n` +
                `*Saturday:*\n` +
                `⏰ 8:30 AM - 5:00 PM\n\n` +
                `*Sunday:* Closed\n\n` +
                `*Public Holidays:* Closed\n\n` +
                `_For urgent inquiries, call: 077 731 1770_`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, hoursMsg);
            if (logAndSave) {
                logAndSave(`[BusinessHours] Sent hours info to ${msg.from}`);
            }
            return true;
        }
    });

    // Job 11: Greeting Response (Template)
    registry.register({
        name: 'GreetingResponse',
        description: 'Respond to greetings',
        priority: 40,
        patterns: [
            /\b(hi|hello|hey|greetings|හලෝ|ආයුබෝවන්|හායි|කොහොමද)\b/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: true,
        estimatedResponseTime: 100,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            console.log(`[GreetingResponse] Triggered for ${msg.from}`);
            const greetingMsg = `හලෝ! 👋 Welcome to *Lasantha Tyre*!\n\n` +
                `මම ඔබට උදව් කරන්න සූදානම්! I can help you with:\n\n` +
                `🔹 Tyre prices (Send size like: 195/65R15)\n` +
                `🔹 Stock availability\n` +
                `🔹 Quotations\n` +
                `🔹 Vehicle history (Send vehicle no.)\n` +
                `🔹 Contact & location info\n\n` +
                `කරුණාකර ඔබට අවශ්‍ය දෙය කියන්න! 😊`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, greetingMsg);
            if (logAndSave) {
                logAndSave(`[GreetingResponse] Sent greeting to ${msg.from}`);
            }
            return true;
        }
    });

    // Job 10: Thank You Response (Template)
    registry.register({
        name: 'ThankYouResponse',
        description: 'Respond to thank you messages',
        priority: 40,
        patterns: [
            /^(thank|thanks|ස්තූතියි|ස්තූති|බොහොම).*$/i
        ],
        entityExtractors: {},
        requiresEntities: [],
        canHandlePartial: true,
        estimatedResponseTime: 100,
        handler: async (msg, deps) => {
            const { logAndSave, client } = deps;
            const thankYouMsg = `ඔබට ස්තූතියි! 😊\n\n` +
                `You're welcome! වෙන දෙයක් අවශ්‍යද?\n\n` +
                `_Feel free to ask anything about tyres!_`;
            
            await (deps.safeReply || safeReply)(msg, client, msg.from, thankYouMsg);
            if (logAndSave) {
                logAndSave(`[ThankYouResponse] Sent thank you reply to ${msg.from}`);
            }
            return true;
        }
    });
}

module.exports = {
    registerAllJobs
};
