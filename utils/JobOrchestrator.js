/**
 * Job Orchestrator
 * Manages workflow execution
 */

class JobOrchestrator {
    constructor() {
        this.workflows = new Map();
    }
    
    /**
     * Register a workflow
     * @param {string} name - Workflow name
     * @param {object} workflow - Workflow definition
     */
    registerWorkflow(name, workflow) {
        this.workflows.set(name, workflow);
        console.log(`[JobOrchestrator] Registered workflow: ${name}`);
    }
    
    /**
     * Match a workflow based on message text
     * @param {string} text - Message text
     * @param {object} context - Context
     * @returns {string|null} - Workflow name or null
     */
    matchWorkflow(text, context) {
        // For now, no workflows are defined
        return null;
    }
    
    /**
     * Execute a workflow
     * @param {string} workflowName - Workflow name
     * @param {object} msg - WhatsApp message
     * @param {object} context - Execution context
     * @returns {Promise<object>} - Result { success: boolean, duration: number, error?: string }
     */
    async executeWorkflow(workflowName, msg, context) {
        const startTime = Date.now();
        
        try {
            const workflow = this.workflows.get(workflowName);
            
            if (!workflow) {
                return {
                    success: false,
                    duration: Date.now() - startTime,
                    error: 'Workflow not found'
                };
            }
            
            // Execute workflow steps
            if (workflow.execute && typeof workflow.execute === 'function') {
                await workflow.execute(msg, context);
            }
            
            return {
                success: true,
                duration: Date.now() - startTime
            };
        } catch (error) {
            console.error(`[JobOrchestrator] Workflow ${workflowName} failed:`, error.message);
            return {
                success: false,
                duration: Date.now() - startTime,
                error: error.message
            };
        }
    }
}

// Create singleton instance
const orchestrator = new JobOrchestrator();

module.exports = {
    JobOrchestrator,
    orchestrator
};
