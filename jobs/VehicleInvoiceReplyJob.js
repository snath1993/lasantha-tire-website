// VehicleInvoiceReplyJob
// Reply invoice details for given vehicle number
const { extractVehicleNumber } = require('../utils/detect');
const { isJobAllowEveryone } = require('../utils/jobsConfigReader');
module.exports = async function VehicleInvoiceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    const text = msg.body.trim();
    const vehicleNumber = extractVehicleNumber(text);
    const senderNumber = msg.from.replace('@c.us', '');
        const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
    const allowEveryone = isJobAllowEveryone('VehicleInvoiceReplyJob');
    const isAllowedContact = allowedContacts.includes(senderNumber) || allowedContacts.includes(senderNormalizedTo0);
    if (vehicleNumber && (allowEveryone || isAllowedContact)) {
        try {
            await sql.connect(sqlConfig);
            // Get all invoices for the vehicle, sorted by InvoiceDate desc
            const result = await sql.query`
                SELECT Expr2 AS InvoiceNo, Expr9 AS Mileage, Expr8 AS VehicleNo, Expr1 AS InvoiceDate, Expr4 AS Description, Expr6 AS UnitPrice, Expr5 AS Qty, Expr12 AS Amount, Categoty, Expr13 AS LineDiscountPercentage
                FROM [View_Sales report whatsapp]
                WHERE Expr8 = ${vehicleNumber}
                ORDER BY Expr1 DESC, Expr2 DESC
            `;
            if (result.recordset.length > 0) {
                // Only detailed history (tyres & wheel alignment), oldest date at top
                let reply = `*Invoice History for ${vehicleNumber}*\n`;
                let lastDate = '';
                // Filter and sort ascending by date
                const filtered = result.recordset.filter(row => {
                    const cat = (row.Categoty || '').toString().toUpperCase();
                    // Some rows might have ItemType='KATTA TYRES' or similar
                    // So we check if it includes TYRE or is WHEEL ALIGNMENT
                    const desc = (row.Description || '').toString().toLowerCase();
                    return cat.includes('TYRE') || cat === 'WHEEL ALIGNMENT' || desc.includes('wheel alignment');
                }).sort((a, b) => new Date(a.InvoiceDate) - new Date(b.InvoiceDate));
                
                if (filtered.length === 0) {
                    // Found records but none match the filter (e.g. only services)
                    // We should probably still reply something or lax the filter?
                    // For now, let's behave as if no records found if nothing relevant to tyres
                    msg.reply(`No tyre/alignment history found for ${vehicleNumber}`);
                    logAndSave(`Vehicle ${vehicleNumber} found but no relevant history`);
                    return;
                }

                filtered.forEach(row => {
                    let dateObj = new Date(row.InvoiceDate);
                    let dateOnly = !isNaN(dateObj) ? dateObj.toISOString().slice(0, 10) : String(row.InvoiceDate).slice(0, 10);
                    if (dateOnly !== lastDate) {
                        if (lastDate !== '') reply += '\n';
                        reply += `🗓️ *${dateOnly}*\n`;
                        lastDate = dateOnly;
                    }
                    const disc = Number(row.LineDiscountPercentage) || 0;
                    const effUnit = (Number(row.UnitPrice) || 0) * (1 - disc/100);
                    reply += `Invoice No: ${row.InvoiceNo}\nMileage: ${row.Mileage}\nDescription: ${row.Description}\nQuantity: ${row.Qty}\nUnit Price: ${effUnit}/=\n---------------------\n`;
                });
                msg.reply(reply.trim());
                logAndSave(`Invoice history reply sent for ${vehicleNumber} to ${senderNumber} (lines=${filtered.length})`);
            } else {
                msg.reply(`No invoice found for vehicle number: ${vehicleNumber}`);
                logAndSave(`No invoice for vehicle ${vehicleNumber}`);
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
