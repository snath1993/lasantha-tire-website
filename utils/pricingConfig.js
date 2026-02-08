// utils/pricingConfig.js
// Centralizes pricing constants / environment overrides for tyre pricing logic.

const DEFAULTS = {
  BASE_ROUND_STEP: 50,
  PUBLIC_BASE_MARKUP: 1500,
  PUBLIC_EXTRA_BUFFER: 500,
  ALLOWED_DEFAULT_MARKUP: 1500,
  MOTORBIKE_EXTRA: 1000,
  MAXXIS_PUBLIC_MARKUP: 500
};

const { getJobConfig } = require('./jobsConfigReader');

function coerceNumber(value, fallback) {
  if (value === null || value === undefined) {
    return fallback;
  }
  const parsed = Number(value);
  if (Number.isNaN(parsed) || !Number.isFinite(parsed)) {
    return fallback;
  }
  return Math.round(parsed);
}

function loadPricingEnv() {
  const pricing = {
    baseRoundStep: parseInt(process.env.PRICE_ROUND_STEP || String(DEFAULTS.BASE_ROUND_STEP), 10),
    publicBaseMarkup: parseInt(process.env.PUBLIC_BASE_MARKUP || String(DEFAULTS.PUBLIC_BASE_MARKUP), 10),
    publicExtraBuffer: parseInt(process.env.PUBLIC_EXTRA_BUFFER || String(DEFAULTS.PUBLIC_EXTRA_BUFFER), 10),
    allowedDefaultMarkup: parseInt(process.env.ALLOWED_DEFAULT_MARKUP || String(DEFAULTS.ALLOWED_DEFAULT_MARKUP), 10),
    motorbikeExtra: parseInt(process.env.MOTORBIKE_EXTRA || String(DEFAULTS.MOTORBIKE_EXTRA), 10),
    maxxisPublicMarkup: parseInt(process.env.MAXXIS_PUBLIC_MARKUP || String(DEFAULTS.MAXXIS_PUBLIC_MARKUP), 10)
  };

  try {
    const config = getJobConfig('TyrePriceReplyJob');
    if (config && typeof config.settings === 'object' && config.settings !== null) {
      pricing.baseRoundStep = coerceNumber(config.settings.baseRoundStep, pricing.baseRoundStep);
      pricing.publicBaseMarkup = coerceNumber(config.settings.publicBaseMarkup, pricing.publicBaseMarkup);
      pricing.publicExtraBuffer = coerceNumber(config.settings.publicExtraBuffer, pricing.publicExtraBuffer);
      pricing.allowedDefaultMarkup = coerceNumber(config.settings.allowedDefaultMarkup, pricing.allowedDefaultMarkup);
      pricing.motorbikeExtra = coerceNumber(config.settings.motorbikeExtra, pricing.motorbikeExtra);
      pricing.maxxisPublicMarkup = coerceNumber(config.settings.maxxisPublicMarkup, pricing.maxxisPublicMarkup);
    }
  } catch (error) {
    console.error('pricingConfig: unable to apply job overrides', error.message);
  }

  return pricing;
}

function roundToStep(cost, step) {
  return Math.round(cost / step) * step;
}

module.exports = { loadPricingEnv, roundToStep, DEFAULTS };
