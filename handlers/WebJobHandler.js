/**
 * Web Job Handler
 * Handles web-triggered jobs
 */

class WebJobHandler {
    constructor() {
        this.jobs = new Map();
    }
    
    registerJob(name, handler) {
        this.jobs.set(name, handler);
        console.log(`[WebJobHandler] Registered job: ${name}`);
    }
    
    async executeJob(name, params) {
        const handler = this.jobs.get(name);
        
        if (!handler) {
            throw new Error(`Job not found: ${name}`);
        }
        
        return await handler(params);
    }
}

module.exports = WebJobHandler;
