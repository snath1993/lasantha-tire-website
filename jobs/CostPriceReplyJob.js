// CostPriceReplyJob
// Handles 'size cost' requests and replies with item description (wrapped in ~~) and UnitCost
const { extractTyreSizeFlexible, extractVehicleNumber } = require('../utils/detect');
const { parsePriceAdjustments } = require('../utils/priceAdjust');

const ALLOWED_COST_CONTACTS = ['0777078700', '0777311770', '0771222509'];

module.exports = async function CostPriceReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    const rawText = msg.body.trim();
    const senderNumber = msg.from.replace('@c.us', '');
    const vehicleNo = extractVehicleNumber(rawText);
    const textForTyre = vehicleNo ? rawText.replace(vehicleNo, '') : rawText;
    const tyreSize = extractTyreSizeFlexible(textForTyre);
    const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
    const isAllowedCostContact = ALLOWED_COST_CONTACTS.includes(senderNumber) || ALLOWED_COST_CONTACTS.includes(senderNormalizedTo0);
    if (!tyreSize) return false;
    if (!/cost/i.test(rawText)) return false;
    if (!isAllowedCostContact) {
        // Not authorized to see cost
        return false;
    }

    try {
        await sql.connect(sqlConfig);
        const result = await sql.query`
            SELECT im.ItemDescription, im.UnitCost, im.Categoty, im.Custom3
            FROM View_ItemMasterS im
            JOIN View_ItemWhse iw ON im.ItemID = iw.ItemID
            WHERE im.Categoty = 'TYRES'
              AND iw.QTY > 0
              AND (
                  im.ItemDescription LIKE ${'%' + tyreSize + '%'}
                  OR im.ItemDescription LIKE ${'%' + tyreSize.replace(/\//g, '') + '%'}
              )
        `;

        const tyres = (result.recordset || []).filter(t =>
            (t.ItemDescription.includes(tyreSize) || t.ItemDescription.replace(/\//g, '').includes(tyreSize.replace(/\//g, '')))
        );

        if (!tyres || tyres.length === 0) {
            msg.reply('No cost price found.');
            logAndSave(`No cost price found for ${tyreSize} to ${senderNumber}`);
            return true;
        }

        const costList = tyres.map(t => `~~${t.ItemDescription}~~\n${t.UnitCost}`).join('\n\n');
        msg.reply(costList);
        logAndSave(`Cost prices sent for size ${tyreSize} to ${senderNumber} (${tyres.length} items)`);
        return true;
    } catch (err) {
        msg.reply('Error connecting to SQL Server.');
        logAndSave(`CostPriceReplyJob SQL error: ${err.message}`);
        return true;
    } finally {
        if (sql.connected) {
            try { await sql.close(); } catch {}
        }
    }
};
