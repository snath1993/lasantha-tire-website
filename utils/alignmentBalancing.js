// Utility to extract alignment/balancing info from message text
// Returns: { alignment: { type: 'car'|'jeep'|'bus'|'van'|'lorry'|'prime'|null }, balancing: boolean }
function parseAlignmentBalancing(text) {
    let alignment = null;
    let balancing = false;
    // Detect balancing
    if (/balanc(ing)?/i.test(text)) {
        balancing = true;
    }
    // Detect alignment and type (support typos like 'aligment')
    const alignMatch = text.match(/ali[g]?n?m?e?n?t?\s*(car|jeep|bus|van|lorry|prime)?/i);
    if (alignMatch) {
        alignment = alignMatch[1] ? alignMatch[1].toLowerCase() : 'car';
    }
    return { alignment, balancing };
}

module.exports = { parseAlignmentBalancing };