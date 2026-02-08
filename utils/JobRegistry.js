/**
 * Advanced Job Registry System
 * Centralized job management with priority, routing, and orchestration
 * 
 * Features:
 * - Priority-based job execution
 * - Pattern matching & entity extraction
 * - Job chaining & fallback
 * - Performance tracking
 * - Zero impact on existing jobs
 */

const { extractTyreSizeFlexible, extractVehicleNumber } = require('./detect');
const { analyzeMessage, extractIntent, extractBrand: extractBrandNLU } = require('./multilingualNLU');
const { loadJobsConfig } = require('./jobsConfigReader');

class JobRegistry {
    constructor() {
        this.jobs = new Map();
        this.stats = {
            totalExecutions: 0,
            successfulExecutions: 0,
            failedExecutions: 0,
            jobStats: {}
        };
    }

    /**
     * Register a job with metadata
     * @param {Object} jobConfig 
     */
    register(jobConfig) {
        const {
            name,
            handler,
            priority = 50,
            patterns = [],
            entityExtractors = {},
            requiresEntities = [],
            canHandlePartial = false,
            estimatedResponseTime = 1000,
            description = ''
        } = jobConfig;

        if (!name || !handler) {
            throw new Error('Job name and handler are required');
        }

        this.jobs.set(name, {
            name,
            handler,
            priority,
            patterns,
            entityExtractors,
            requiresEntities,
            canHandlePartial,
            estimatedResponseTime,
            description,
            registered: new Date(),
            executions: 0,
            successes: 0,
            failures: 0,
            totalTime: 0
        });

        // Initialize stats
        this.stats.jobStats[name] = {
            executions: 0,
            successes: 0,
            failures: 0,
            avgResponseTime: 0,
            lastExecuted: null
        };
    }

    /**
     * Find best job for message
     * @param {string} text - Message text
     * @param {Object} context - Additional context
     * @param {number} minScore - Minimum score threshold (default: 40, lowered to allow AI)
     * @returns {Object|null} - Best matching job with score
     */
    findBestJob(text, context = {}, minScore = 40) {
        const candidates = [];

        // 🧠 MULTILINGUAL NLU ANALYSIS
        const nluAnalysis = analyzeMessage(text);
        
        // Detect user intent using advanced NLU (supports Sinhala, Tamil, English)
        const detectedLanguage = nluAnalysis.language;
        const detectedIntent = nluAnalysis.intent; // String: 'photo', 'price', 'stock', etc.
        const detectedConfidence = nluAnalysis.confidence; // Number: 0-100
        const detectedBrand = nluAnalysis.entities?.brand || null;
        const allIntentScores = nluAnalysis.allIntentScores || {};
        
        if (context.logAndSave) {
            context.logAndSave(`[NLU] 🌍 Language: ${detectedLanguage}, Intent: ${detectedIntent || 'none'} (${detectedConfidence || 0}%), Brand: ${detectedBrand || 'none'}`);
            if (Object.keys(allIntentScores).length > 1) {
                context.logAndSave(`[NLU] 📊 All intents: ${JSON.stringify(allIntentScores)}`);
            }
        }

        // Check if message has tyre size and brand
        const extractedSize = (() => {
            try { return extractTyreSizeFlexible(text); } catch { return null; }
        })();
        const sizeLooseMatch = /\b(\d{2,3})\s*[\/_-]?\s*R?(\d{2})\b/i.test(text) || /\b(\d{2,3})\s*[\/_-]\s*(\d{2}(?:\.\d{1,2})?)\s*[\/_-]\s*(\d{2})\b/i.test(text);
        const hasTyreSize = !!extractedSize || sizeLooseMatch;
        const hasBrand = detectedBrand !== null; // Use NLU brand detection
        
        // Determine primary intent from NLU (more accurate than keyword matching)
        let primaryIntent = detectedIntent; // photo, price, stock, quotation, greeting, thanks, warranty
        
        if (context.logAndSave) {
            context.logAndSave(`[JobRegistry] 🔍 Detector: size=${extractedSize || (sizeLooseMatch ? 'loose-match' : 'none')}, brand=${hasBrand ? detectedBrand : 'none'}, intent=${primaryIntent || 'none'}`);
        }

        // Smart routing: If user has specific intent, allow that job to run
        const shouldSuppressJobs = hasTyreSize && hasBrand && !primaryIntent;
        
        if (shouldSuppressJobs && context.logAndSave) {
            context.logAndSave(`[JobRegistry] 🤖 Size + Brand without clear intent → Forcing AI flow`);
        }

        for (const [name, job] of this.jobs.entries()) {
            let score = this.scoreJob(job, text, context);
            
            // Smart suppression based on NLU intent
            if (shouldSuppressJobs) {
                // Only suppress if NO clear intent was detected by NLU
                if (name === 'TyrePriceReply' || name === 'TyrePhotoReply' || 
                    name === 'TyreCostReply' || name === 'TyreStockReply' || 
                    name === 'ContactInfo' || name === 'LocationInfo' || name === 'BusinessHours') {
                    if (context.logAndSave) {
                        context.logAndSave(`[JobRegistry] 🚫 Suppressing ${name} (score was ${score}, now 0)`);
                    }
                    score = 0;
                }
            } else if (primaryIntent) {
                // User has specific intent detected by NLU - boost matching job, suppress others
                if (primaryIntent === 'photo' && name === 'TyrePhotoReply') {
                    // Boost photo job when photo intent detected
                    score += 30;
                    if (context.logAndSave) {
                        context.logAndSave(`[NLU] 🎯 Photo intent detected (${detectedConfidence}%) → Boosting ${name} (+30)`);
                    }
                } else if (primaryIntent === 'photo' && name !== 'TyrePhotoReply') {
                    // Suppress all non-photo jobs when photo is requested
                    if (name === 'TyrePriceReply' || name === 'TyreQtyReply' || name === 'CostPriceReply') {
                        if (context.logAndSave && score > 0) {
                            context.logAndSave(`[NLU] 🎯 Photo intent detected → Suppressing ${name}`);
                        }
                        score = 0;
                    }
                } else if (primaryIntent === 'price' && name === 'TyrePhotoReply') {
                    // Don't send photos when asking for price
                    score = 0;
                } else if (primaryIntent === 'stock' && name === 'TyrePhotoReply') {
                    // Don't send photos when asking for stock
                    score = 0;
                } else if (primaryIntent === 'greeting' && name.includes('Tyre')) {
                    // Don't trigger tyre jobs for greetings
                    score = 0;
                } else if (primaryIntent === 'thanks' && name.includes('Tyre')) {
                    // Don't trigger tyre jobs for thank you messages
                    score = 0;
                }
                
                // Boost score for matching job
                if ((primaryIntent === 'photo' && name === 'TyrePhotoReply') ||
                    (primaryIntent === 'price' && name === 'TyrePriceReply') ||
                    (primaryIntent === 'stock' && name === 'TyreQtyReply') ||
                    (primaryIntent === 'quotation' && name === 'TyreQuotationPDF') ||
                    (primaryIntent === 'greeting' && name === 'GreetingResponse') ||
                    (primaryIntent === 'thanks' && name === 'ThankYouResponse') ||
                    (primaryIntent === 'warranty' && name === 'WarrantyInfo')) {
                    score += 30; // Significant boost for NLU-matched jobs
                    if (context.logAndSave) {
                        context.logAndSave(`[NLU] ⚡ Boosting ${name} score by 30 (intent match)`);
                    }
                }
            }
            
            if (score >= minScore) { // Only consider jobs above threshold
                candidates.push({ name, job, score });
            }
        }

        if (candidates.length === 0) return null;

        // Sort by score (desc), then priority (desc)
        candidates.sort((a, b) => {
            if (b.score !== a.score) return b.score - a.score;
            return b.job.priority - a.job.priority;
        });

        return candidates[0];
    }

    /**
     * Score a job's ability to handle a message
     * @param {Object} job 
     * @param {string} text 
     * @param {Object} context 
     * @returns {number} - Score (0-100)
     */
    scoreJob(job, text, context = {}) {
        let score = 0;

        // Pattern matching
        for (const pattern of job.patterns) {
            if (typeof pattern === 'string') {
                if (text.toLowerCase().includes(pattern.toLowerCase())) {
                    score += 30;
                }
            } else if (pattern instanceof RegExp) {
                if (pattern.test(text)) {
                    score += 40;
                }
            }
        }

        // Entity extraction
        const extractedEntities = this.extractEntities(job, text, context);
        const requiredFound = job.requiresEntities.every(
            entity => extractedEntities[entity] !== null && extractedEntities[entity] !== undefined
        );

        // Only award +50 points if job actually REQUIRES entities AND found them
        // Jobs with no requirements shouldn't get automatic +50 points
        if (job.requiresEntities.length > 0 && requiredFound) {
            score += 50;
        } else if (job.canHandlePartial && Object.keys(extractedEntities).length > 0) {
            score += 20;
        }

        return Math.min(score, 100);
    }

    /**
     * Extract entities for a job
     * @param {Object} job 
     * @param {string} text 
     * @param {Object} context 
     * @returns {Object} - Extracted entities
     */
    extractEntities(job, text, context = {}) {
        const entities = {};

        for (const [entityName, extractor] of Object.entries(job.entityExtractors)) {
            try {
                if (typeof extractor === 'function') {
                    entities[entityName] = extractor(text, context);
                } else if (typeof extractor === 'string') {
                    // Built-in extractor
                    entities[entityName] = this.builtInExtractors[extractor]?.(text, context) || null;
                }
            } catch (err) {
                entities[entityName] = null;
            }
        }

        return entities;
    }

    /**
     * Execute a job
     * @param {string} jobName 
     * @param {Object} msg 
     * @param {Object} deps - Dependencies (sql, sqlConfig, etc.)
     * @returns {Promise<boolean>} - Whether job handled the message
     */
    async execute(jobName, msg, deps = {}) {
        const job = this.jobs.get(jobName);
        if (!job) {
            throw new Error(`Job ${jobName} not found`);
        }

        // Check if job is enabled in config
        const config = loadJobsConfig();
        if (config) {
            // Check exact name or name + 'Job' (common convention)
            const jobConfig = config[jobName] || config[jobName + 'Job'];
            if (jobConfig && jobConfig.enabled === false) {
                if (deps.context && deps.context.logAndSave) {
                    deps.context.logAndSave(`[JobRegistry] Job ${jobName} is disabled in config. Skipping.`);
                }
                return false;
            }
        }

        const startTime = Date.now();
        job.executions++;
        this.stats.totalExecutions++;

        try {
            const text = msg.body?.trim() || '';
            const context = deps.context || {};
            
            // Extract entities
            const entities = this.extractEntities(job, text, context);
            
            // Execute handler
            const handled = await job.handler(msg, { ...deps, entities });

            const elapsed = Date.now() - startTime;
            job.successes++;
            job.totalTime += elapsed;
            this.stats.successfulExecutions++;
            this.stats.jobStats[jobName].executions++;
            this.stats.jobStats[jobName].successes++;
            this.stats.jobStats[jobName].avgResponseTime = job.totalTime / job.successes;
            this.stats.jobStats[jobName].lastExecuted = new Date();

            return handled;
        } catch (err) {
            const elapsed = Date.now() - startTime;
            job.failures++;
            job.totalTime += elapsed;
            this.stats.failedExecutions++;
            this.stats.jobStats[jobName].executions++;
            this.stats.jobStats[jobName].failures++;
            this.stats.jobStats[jobName].lastExecuted = new Date();

            if (deps.logAndSave) {
                deps.logAndSave(`[JobRegistry] Job ${jobName} failed: ${err.message}`);
            }

            throw err;
        }
    }

    /**
     * Try to handle message with registered jobs
     * @param {Object} msg 
     * @param {Object} deps 
     * @returns {Promise<Object>} - Result { handled, jobName, entities }
     */
    async tryHandle(msg, deps = {}) {
        const text = msg.body?.trim() || '';
        const context = deps.context || {};

        // Find best job
        const match = this.findBestJob(text, context);
        
        if (!match) {
            return { handled: false, jobName: null, entities: {}, reason: 'No matching job' };
        }

        const { name, job, score } = match;

        if (deps.logAndSave) {
            deps.logAndSave(`[JobRegistry] Selected job: ${name} (score: ${score}, priority: ${job.priority})`);
        }

        // Extract entities
        const entities = this.extractEntities(job, text, context);

        // Execute job
        try {
            const handled = await this.execute(name, msg, { ...deps, entities });
            return { handled, jobName: name, entities, score };
        } catch (err) {
            return { handled: false, jobName: name, entities, error: err.message };
        }
    }

    /**
     * Get job statistics
     */
    getStats() {
        const jobs = [];
        for (const [name, job] of this.jobs.entries()) {
            jobs.push({
                name,
                priority: job.priority,
                executions: job.executions,
                successes: job.successes,
                failures: job.failures,
                successRate: job.executions > 0 ? ((job.successes / job.executions) * 100).toFixed(2) + '%' : 'N/A',
                avgResponseTime: job.successes > 0 ? Math.round(job.totalTime / job.successes) + 'ms' : 'N/A',
                estimatedTime: job.estimatedResponseTime + 'ms'
            });
        }

        return {
            summary: {
                totalJobs: this.jobs.size,
                totalExecutions: this.stats.totalExecutions,
                successRate: this.stats.totalExecutions > 0 
                    ? ((this.stats.successfulExecutions / this.stats.totalExecutions) * 100).toFixed(2) + '%' 
                    : 'N/A'
            },
            jobs: jobs.sort((a, b) => b.executions - a.executions)
        };
    }

    /**
     * Built-in entity extractors
     */
    builtInExtractors = {
        tyreSize: (text) => extractTyreSizeFlexible(text),
        vehicleNumber: (text) => extractVehicleNumber(text),
        brand: (text) => {
            const brands = ['MAXXIS', 'DURATURN', 'BRIDGESTONE', 'MICHELIN', 'YOKOHAMA', 
                           'CONTINENTAL', 'GOODYEAR', 'PIRELLI', 'DUNLOP', 'HANKOOK'];
            const match = text.match(new RegExp(`\\b(${brands.join('|')})\\b`, 'i'));
            return match ? match[1].toUpperCase() : null;
        },
        quantity: (text) => {
            const match = text.match(/(\d+)\s*(tyre|tire|piece|pcs)/i);
            return match ? parseInt(match[1], 10) : null;
        },
        intent: (text) => {
            const intents = {
                price: /\b(price|ගාන|කියනවද|mil|cost)\b/i,
                stock: /\b(stock|තියෙනවද|available|ඇත්තද|qty|quantity)\b/i,
                photo: /\b(photo|image|පින්තූරය|එවන්න|pic)\b/i,
                quotation: /\b(quotation|estimate|ඇස්තමේන්තුව|quote)\b/i,
                cost: /\b(cost|costprice)\b/i
            };

            for (const [intent, pattern] of Object.entries(intents)) {
                if (pattern.test(text)) return intent;
            }
            return null;
        }
    };
}

// Singleton instance
const registry = new JobRegistry();

module.exports = {
    JobRegistry,
    registry
};
