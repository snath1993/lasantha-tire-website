// =================================================================
// Tyre Specifications Fetcher (fetchTyreSpecs.js)
// Fetches detailed tyre specifications from the WhatsAppAI database
// =================================================================

const sql = require('mssql');

/**
 * Normalizes brand name for comparison
 * - Converts to uppercase
 * - Trims whitespace
 * - Removes extra spaces
 */
function normalizeBrandName(brand) {
    if (!brand) return '';
    return brand.toString().trim().toUpperCase().replace(/\s+/g, ' ');
}

/**
 * Brand name mapping to handle inconsistencies between databases
 * Maps production DB brand names to correct names in tyre_specs DB
 * Keys should be normalized (uppercase, trimmed)
 */
const BRAND_NAME_MAP = {
    'MAXXIES': 'MAXXIS',
    'GOOD YEAR': 'GOODYEAR',
    'YOKOHAMA T/H': 'YOKOHAMA',
    'YOKOHAMA TOY': 'YOKOHAMA'
};

/**
 * Fetches tyre specifications from the tyre_specs table
 * @param {string} brand - The tyre brand name
 * @param {string} pattern - The tyre pattern name (optional)
 * @returns {Promise<object|null>} - Tyre specifications or null if not found
 */
async function fetchTyreSpecs(brand, pattern = null) {
    let pool;
    try {
        // Normalize brand name (uppercase, trim, remove extra spaces)
        const normalizedBrand = normalizeBrandName(brand);
        
        // Map brand name if it's in the mapping (handles production DB inconsistencies)
        const mappedBrand = BRAND_NAME_MAP[normalizedBrand] || normalizedBrand;
        
        // Normalize pattern name too
        const normalizedPattern = pattern ? normalizeBrandName(pattern) : null;
        
        // Log if brand name was mapped
        if (mappedBrand !== normalizedBrand) {
            console.log(`[Brand Mapping] "${brand}" → "${mappedBrand}"`);
        }

        // Connect to WhatsAppAI database (environment-driven)
        const config = {
            user: process.env.DB_USER,
            password: process.env.DB_PASSWORD,
            server: process.env.DB_SERVER,
            database: process.env.DB_SPECS_DATABASE || 'WhatsAppAI', // Specs default DB
            options: {
                encrypt: false,
                trustServerCertificate: true,
                enableArithAbort: true,
            },
        };

        // If server not configured, skip DB fetch silently
        if (!config.server || typeof config.server !== 'string' || config.server.trim().length === 0) {
            if (!fetchTyreSpecs._warnedNoServer) {
                console.warn('[fetchTyreSpecs] Skipping DB lookup: DB_SERVER not configured');
                fetchTyreSpecs._warnedNoServer = true;
            }
            return null;
        }

        pool = await sql.connect(config);

        let query;
        let params;

        if (normalizedPattern && normalizedPattern !== '-') {
            // Search by both brand and pattern
            query = `
                SELECT TOP 1
                    brand,
                    pattern,
                    fuel_efficiency,
                    wet_grip,
                    noise_level_db,
                    noise_class,
                    dry_grip_rating,
                    comfort_rating,
                    durability_rating,
                    handling_rating,
                    tyre_category,
                    terrain_type,
                    season_type,
                    speed_rating,
                    load_index,
                    max_speed_kmh,
                    treadwear_rating,
                    traction_rating,
                    temperature_rating,
                    key_features,
                    best_for,
                    warranty_km,
                    is_runflat,
                    is_reinforced,
                    special_technology,
                    data_source,
                    confidence_score
                FROM tyre_specs
                WHERE UPPER(LTRIM(RTRIM(brand))) = @brand 
                  AND UPPER(LTRIM(RTRIM(pattern))) = @pattern
            `;
            params = { brand: mappedBrand, pattern: normalizedPattern };
        } else {
            // Search by brand only (get any one pattern for this brand)
            query = `
                SELECT TOP 1
                    brand,
                    pattern,
                    fuel_efficiency,
                    wet_grip,
                    noise_level_db,
                    noise_class,
                    dry_grip_rating,
                    comfort_rating,
                    durability_rating,
                    handling_rating,
                    tyre_category,
                    terrain_type,
                    season_type,
                    speed_rating,
                    load_index,
                    max_speed_kmh,
                    treadwear_rating,
                    traction_rating,
                    temperature_rating,
                    key_features,
                    best_for,
                    warranty_km,
                    is_runflat,
                    is_reinforced,
                    special_technology,
                    data_source,
                    confidence_score
                FROM tyre_specs
                WHERE UPPER(LTRIM(RTRIM(brand))) = @brand
                ORDER BY confidence_score DESC
            `;
            params = { brand: mappedBrand };
        }

        const request = pool.request();
        Object.entries(params).forEach(([key, value]) => {
            request.input(key, sql.NVarChar, value);
        });

        const result = await request.query(query);

        if (result.recordset && result.recordset.length > 0) {
            const spec = result.recordset[0];
            
            // Format the data nicely
            return {
                brand: spec.brand,
                pattern: spec.pattern,
                euLabel: {
                    fuelEfficiency: spec.fuel_efficiency,
                    wetGrip: spec.wet_grip,
                    noiseLevel: spec.noise_level_db,
                    noiseClass: spec.noise_class
                },
                performance: {
                    dryGrip: spec.dry_grip_rating,
                    comfort: spec.comfort_rating,
                    durability: spec.durability_rating,
                    handling: spec.handling_rating
                },
                classification: {
                    category: spec.tyre_category,
                    terrain: spec.terrain_type,
                    season: spec.season_type
                },
                technical: {
                    speedRating: spec.speed_rating,
                    loadIndex: spec.load_index,
                    maxSpeed: spec.max_speed_kmh,
                    treadwear: spec.treadwear_rating,
                    traction: spec.traction_rating,
                    temperature: spec.temperature_rating
                },
                features: {
                    keyFeatures: spec.key_features,
                    bestFor: spec.best_for,
                    warranty: spec.warranty_km
                },
                technology: {
                    runFlat: spec.is_runflat,
                    reinforced: spec.is_reinforced,
                    special: spec.special_technology
                },
                dataQuality: {
                    source: spec.data_source,
                    confidence: spec.confidence_score
                }
            };
        }

        return null; // No specs found

    } catch (error) {
        console.error('[fetchTyreSpecs Error]', error);
        return null;
    } finally {
        if (pool) {
            await pool.close();
        }
    }
}

module.exports = { fetchTyreSpecs };
