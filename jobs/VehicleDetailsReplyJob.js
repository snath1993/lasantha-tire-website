// VehicleDetailsReplyJob
// Reply invoice details for given vehicle number (CBH-6483 format)
const { extractVehicleNumber } = require('../utils/detect');
const { isJobAllowEveryone } = require('../utils/jobsConfigReader');
module.exports = async function VehicleDetailsReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    const text = msg.body.trim();
    const vehicleNumber = extractVehicleNumber(text);
    // Only allowed contacts can get invoice details
    const senderNumber = msg.from.replace('@c.us', '');
    const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
    const allowEveryone = isJobAllowEveryone('VehicleDetailsReplyJob');
    const isAllowedContact = allowedContacts.includes(senderNumber) || allowedContacts.includes(senderNormalizedTo0);
    if (vehicleNumber && (allowEveryone || isAllowedContact)) {
        try {
            await sql.connect(sqlConfig);
            // Get all invoices for the vehicle, sorted by InvoiceDate desc
            const result = await sql.query`
                SELECT InvoiceNo, Mileage, VehicleNo, InvoiceDate, Description, UnitPrice, Qty, Categoty, LineDiscountPercentage
                FROM [View_Sales report whatsapp]
                WHERE VehicleNo = ${vehicleNumber}
                ORDER BY InvoiceDate DESC, InvoiceNo DESC
            `;
            // Filter only Wheel Alignment & Tyres
            const filtered = result.recordset.filter(row => {
                const cat = (row.Categoty || '').toString().toUpperCase();
                const desc = (row.Description || '').toString().toLowerCase();
                return cat === 'TYRES' || desc.includes('wheel alignment');
            });
            if (filtered.length > 0) {
                let reply = `*Invoice Details for ${vehicleNumber}*\n\n`;
                let lastDate = '';
                filtered.forEach(row => {
                    // Format date as YYYY-MM-DD regardless of input
                    let dateObj = new Date(row.InvoiceDate);
                    let dateOnly = !isNaN(dateObj) ? dateObj.toISOString().slice(0, 10) : String(row.InvoiceDate).slice(0, 10);
                    if (dateOnly !== lastDate) {
                        if (lastDate !== '') reply += '\n'; // Add space before new date section
                        reply += `🗓️ *${dateOnly}*\n`;
                        lastDate = dateOnly;
                    }
                    const disc = Number(row.LineDiscountPercentage) || 0;
                    const effUnit = (Number(row.UnitPrice) || 0) * (1 - disc/100);
                    reply += `Invoice No: ${row.InvoiceNo}\nMileage: ${row.Mileage}\nDescription: ${row.Description}\nQuantity: ${row.Qty}\nUnit Price: ${effUnit}/=\n---------------------\n`;
                });
                msg.reply(reply.trim());
                logAndSave(`Vehicle invoice reply for ${vehicleNumber}: ${reply.trim()}`);
            } else {
                msg.reply(`No Wheel Alignment or Tyre invoices found for vehicle: ${vehicleNumber}`);
                logAndSave(`No relevant invoices for vehicle ${vehicleNumber}`);
            }
        } catch (err) {
            msg.reply('Error connecting to SQL Server.');
            logAndSave(`SQL error: ${err.message}`);
        } finally {
            await sql.close();
        }
        return true;
    }
    return false;
}
