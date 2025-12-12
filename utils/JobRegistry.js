/**
 * Job Registry
 * Manages and executes jobs based on message patterns
 */

class JobRegistry {
    constructor() {
        this.jobs = [];
    }
    
    /**
     * Register a job
     * @param {object} job - Job definition
     */
    register(job) {
        this.jobs.push(job);
        console.log(`[JobRegistry] Registered job: ${job.name}`);
    }
    
    /**
     * Try to handle a message with registered jobs
     * @param {object} msg - WhatsApp message
     * @param {object} context - Execution context
     * @returns {Promise<object>} - Result { handled: boolean, jobName: string, reason: string }
     */
    async tryHandle(msg, context) {
        const text = msg.body ? msg.body.trim().toLowerCase() : '';
        
        // Sort jobs by priority (higher priority first)
        const sortedJobs = [...this.jobs].sort((a, b) => (b.priority || 0) - (a.priority || 0));
        
        for (const job of sortedJobs) {
            try {
                // Check if job can handle this message
                if (job.canHandle && typeof job.canHandle === 'function') {
                    const canHandle = await job.canHandle(msg, context);
                    
                    if (canHandle) {
                        console.log(`[JobRegistry] Job ${job.name} can handle message`);
                        
                        // Execute the job
                        if (job.execute && typeof job.execute === 'function') {
                            await job.execute(msg, context);
                            return {
                                handled: true,
                                jobName: job.name,
                                score: 1.0
                            };
                        }
                    }
                }
            } catch (error) {
                console.error(`[JobRegistry] Error in job ${job.name}:`, error.message);
                // Continue to next job
            }
        }
        
        return {
            handled: false,
            reason: 'No matching job found'
        };
    }
    
    /**
     * Get all registered jobs
     * @returns {Array} - List of jobs
     */
    getJobs() {
        return this.jobs;
    }
}

// Create singleton instance
const registry = new JobRegistry();

module.exports = {
    JobRegistry,
    registry
};
