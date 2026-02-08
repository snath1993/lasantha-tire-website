// PriceProvider.js
// Placeholder for future integration with SQL to fetch price/pattern/warranty
// Keep return shape stable so DMs auto-include details when ready.

async function getTyreOffer({ brand, size }) {
  // TODO: Integrate with MSSQL View_Item Master Whatsapp + pricing rules
  // Return null for now (DM will ask to continue on WhatsApp)
  return null;
  /* Example return shape later:
  return {
    brand: 'BRIDGESTONE',
    size: '195/65R15',
    pattern: 'ECOPIA EP150',
    itemCode: 'BS-EP150-1956515',
    cashPrice: 21500, // LKR (Do NOT post publicly)
    warranty: '3 years or 50,000 km',
    stockHint: 'In stock at Thalawathugoda'
  };
  */
}

module.exports = { getTyreOffer };
