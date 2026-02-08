// TyreQtyReplyJob
// Reply with tyre description and quantity for allowed contacts only (unless job marked as allowEveryone)
const { extractTyreSizeFlexible } = require('../utils/detect');
const { isJobAllowEveryone } = require('../utils/jobsConfigReader');
module.exports = async function TyreQtyReplyJob(msg, sql, sqlConfig, allowedContacts, logAndSave) {
    const text = msg.body.trim();
    const tyreSize = extractTyreSizeFlexible(text);
    const senderNumber = msg.from.replace('@c.us', '');
    const senderNormalizedTo0 = senderNumber.replace(/^\+?94/, '0');
    const allowEveryone = isJobAllowEveryone('TyreQtyReplyJob');
    const isAllowedContact = allowedContacts.includes(senderNumber) || allowedContacts.includes(senderNormalizedTo0);
    const canRespond = allowEveryone || isAllowedContact;
    // Match messages like '175/65/15 qty' or '175/65/15 quantity'
    if (tyreSize && canRespond && /(qty|quantity)/i.test(text)) {
        try {
            await sql.connect(sqlConfig);
            const result = await sql.query`
                SELECT m.ItemDescription, w.QTY
                FROM [View_Item Master Whatsapp] m
                INNER JOIN [View_Item Whse Whatsapp] w ON w.ItemID = m.ItemID
                WHERE (m.Categoty = 'TYRES')
                  AND w.QTY > 0
                  AND (
                      m.ItemDescription LIKE ${'%' + tyreSize + '%'}
                      OR REPLACE(m.ItemDescription, '/', '') LIKE ${'%' + tyreSize.replace(/\//g, '') + '%'}
                  )
            `;
            const filtered = result.recordset.filter(tyre =>
                (tyre.ItemDescription || '').includes(tyreSize)
                || (tyre.ItemDescription || '').replace(/\//g, '').includes(tyreSize.replace(/\//g, ''))
            );
            if (filtered.length > 0) {
                let reply = `*Tyre Description & Quantity*\nTyre Size: *${tyreSize}*\n\n`;
                filtered.forEach(tyre => {
                    reply += `Description: ${tyre.ItemDescription}\nQuantity: ${tyre.QTY}\n\n`;
                });
                msg.reply(reply.trim());
                logAndSave(`Qty reply for size ${tyreSize} to ${senderNumber}: ${reply.trim()}`);
            } else {
                msg.reply(`*${tyreSize}* is currently out of stock (QTY zero).`);
                logAndSave(`Out of stock qty reply for size ${tyreSize} to ${senderNumber}`);
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
