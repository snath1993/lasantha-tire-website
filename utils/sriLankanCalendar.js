/**
 * Sri Lankan Calendar Event Detection Utility
 * Detects festivals, holidays, and seasonal events based on current date
 */

/**
 * Check if current date falls within a date range
 * @param {Date} date - Current date
 * @param {string} startMMDD - Start date (MM-DD format)
 * @param {string} endMMDD - End date (MM-DD format)
 * @returns {boolean}
 */
function isWithinDateRange(date, startMMDD, endMMDD) {
  const currentYear = date.getFullYear();
  const [startMonth, startDay] = startMMDD.split('-').map(Number);
  const [endMonth, endDay] = endMMDD.split('-').map(Number);

  const current = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const start = new Date(currentYear, startMonth - 1, startDay);
  let end = new Date(currentYear, endMonth - 1, endDay);

  // Handle year wrap-around (e.g., Dec 20 to Jan 5)
  if (end < start) {
    if (current >= start) {
      // After start date, extend end to next year
      end = new Date(currentYear + 1, endMonth - 1, endDay);
    } else {
      // Before end date, extend start from previous year
      start.setFullYear(currentYear - 1);
    }
  }

  return current >= start && current <= end;
}

/**
 * Detect current Sri Lankan seasonal event
 * @param {Date} date - Date to check (defaults to today)
 * @returns {Object|null} - Event object with name and category, or null
 */
function detectSeasonalEvent(date = new Date()) {
  const fs = require('fs');
  const path = require('path');

  try {
    // Load seasonal captions configuration
    const seasonalPath = path.join(__dirname, '..', 'seasonal-captions.json');
    const seasonalData = JSON.parse(fs.readFileSync(seasonalPath, 'utf-8'));

    // Check each seasonal event
    for (const [eventKey, eventData] of Object.entries(seasonalData.seasonal_captions)) {
      if (isWithinDateRange(date, eventData.date_range.start, eventData.date_range.end)) {
        return {
          key: eventKey,
          name: eventKey.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase()),
          captions: eventData.captions,
          dateRange: eventData.date_range
        };
      }
    }

    return null; // No seasonal event active
  } catch (error) {
    console.error('[SeasonalCalendar] Error detecting event:', error.message);
    return null;
  }
}

/**
 * Get appropriate caption based on current season
 * Falls back to regular captions if no seasonal event
 * @returns {string} - Selected caption
 */
function getSeasonalCaption() {
  const fs = require('fs');
  const path = require('path');

  try {
    // Check for seasonal event
    const currentEvent = detectSeasonalEvent();

    if (currentEvent && currentEvent.captions.length > 0) {
      // Select random seasonal caption
      const randomIndex = Math.floor(Math.random() * currentEvent.captions.length);
      const selectedCaption = currentEvent.captions[randomIndex];
      
      console.log(`[SeasonalCalendar] 🎉 Using ${currentEvent.name} caption`);
      return selectedCaption;
    }

    // No seasonal event - use regular fallback captions
    const fallbackPath = path.join(__dirname, '..', 'fallback-captions.json');
    const fallbackData = JSON.parse(fs.readFileSync(fallbackPath, 'utf-8'));
    const captions = fallbackData.captions || [];

    if (captions.length === 0) {
      throw new Error('No fallback captions available');
    }

    const randomIndex = Math.floor(Math.random() * captions.length);
    const selectedCaption = captions[randomIndex];
    
    console.log('[SeasonalCalendar] ℹ️  Using regular fallback caption');
    return selectedCaption;

  } catch (error) {
    console.error('[SeasonalCalendar] Error getting caption:', error.message);
    
    // Emergency ultra-basic caption
    return `🚗 ප්‍රමිත තත්ත්වයේ ටයර් ලබා ගන්න! 🚗

වාහනයේ ආරක්ෂාව ප්‍රථමයි!

✨ N2 පිරවීම සහ Tyre Fixing නොමිලේ

වැඩි විස්තර සඳහා:
👉 WhatsApp: https://wa.me/94771222509
📞 Call: 0771222509
📍 Visit Us: Thalawathugoda

#LasanthaTyreTraders #QualityTyres`;
  }
}

/**
 * Get information about current seasonal event (for logging/debugging)
 * @returns {Object|null}
 */
function getCurrentEventInfo() {
  return detectSeasonalEvent();
}

module.exports = {
  detectSeasonalEvent,
  getSeasonalCaption,
  getCurrentEventInfo,
  isWithinDateRange
};
