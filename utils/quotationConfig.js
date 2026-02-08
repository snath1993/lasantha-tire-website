// utils/quotationConfig.js
// Shared loader for Tyre quotation pricing knobs exposed via jobs-config.json.

const { getJobConfig } = require('./jobsConfigReader');

const DEFAULTS = {
    TyreQuotationPDFJob: {
        baseRoundStep: 50,
        allowedDefaultMarkup: 1500,
        minimumUnitCost: 3000
    },
    TyreQuotationPDFLibJob: {
        baseRoundStep: 50,
        allowedDefaultMarkup: 1500,
        creditMarkup: 500,
        minimumUnitCost: 3000,
        alignmentRoundStep: 50,
        balancingPrice: 600
    },
    TyreQuotationPDFLibJobPublic: {
        baseRoundStep: 50,
        publicBaseMarkup: 1500,
        publicExtraBuffer: 500,
        maxxisPublicMarkup: 500,
        minimumUnitCost: 3000,
        alignmentRoundStep: 50,
        balancingPrice: 600
    }
};

function coerceNumber(value, fallback) {
    const parsed = Number(value);
    if (!Number.isFinite(parsed)) {
        return fallback;
    }
    return Math.round(parsed);
}

function loadJobSettings(jobKey) {
    const defaults = DEFAULTS[jobKey] || {};
    const jobConfig = getJobConfig(jobKey);
    const settings = (jobConfig && typeof jobConfig.settings === 'object') ? jobConfig.settings : {};
    const merged = { ...defaults };
    for (const key of Object.keys(defaults)) {
        merged[key] = coerceNumber(settings?.[key], defaults[key]);
    }
    return merged;
}

function loadBasicQuotationConfig() {
    return loadJobSettings('TyreQuotationPDFJob');
}

function loadAllowedQuotationConfig() {
    return loadJobSettings('TyreQuotationPDFLibJob');
}

function loadPublicQuotationConfig() {
    return loadJobSettings('TyreQuotationPDFLibJobPublic');
}

function roundToStep(value, step) {
    if (!step || step <= 0) {
        return Math.round(value);
    }
    return Math.round(value / step) * step;
}

module.exports = {
    loadBasicQuotationConfig,
    loadAllowedQuotationConfig,
    loadPublicQuotationConfig,
    roundToStep
};
