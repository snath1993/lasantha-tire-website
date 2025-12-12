/**
 * Configuration Service
 * Provides centralized access to environment variables
 */
class ConfigService {
    /**
     * Get configuration value
     * @param {string} key - Configuration key
     * @param {any} defaultValue - Default value if not found
     * @returns {any} Configuration value
     */
    static get(key, defaultValue = null) {
        const value = process.env[key];
        return value !== undefined ? value : defaultValue;
    }
    
    /**
     * Get configuration value as boolean
     * @param {string} key - Configuration key
     * @param {boolean} defaultValue - Default value if not found
     * @returns {boolean} Configuration value as boolean
     */
    static getBoolean(key, defaultValue = false) {
        const value = process.env[key];
        if (value === undefined) return defaultValue;
        return String(value).toLowerCase() === 'true' || value === '1';
    }
    
    /**
     * Get configuration value as number
     * @param {string} key - Configuration key
     * @param {number} defaultValue - Default value if not found
     * @returns {number} Configuration value as number
     */
    static getNumber(key, defaultValue = 0) {
        const value = process.env[key];
        if (value === undefined) return defaultValue;
        const num = Number(value);
        return isNaN(num) ? defaultValue : num;
    }
}

module.exports = { ConfigService };
