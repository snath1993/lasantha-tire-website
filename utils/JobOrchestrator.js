/**
 * Job Orchestrator - Advanced Workflow Management
 * Handles complex multi-step workflows and job chaining
 */

const { registry } = require('./JobRegistry');

class JobOrchestrator {
    constructor() {
        this.workflows = new Map();
        this.activeWorkflows = new Map();
    }

    /**
     * Define a workflow
     * @param {Object} workflowConfig 
     */
    defineWorkflow(workflowConfig) {
        const {
            name,
            description,
            trigger,
            steps,
            fallback = null,
            timeout = 30000
        } = workflowConfig;

        this.workflows.set(name, {
            name,
            description,
            trigger,
            steps,
            fallback,
            timeout,
            executions: 0,
            successes: 0,
            failures: 0
        });
    }

    /**
     * Check if message triggers a workflow
     * @param {string} text 
     * @param {Object} context 
     * @returns {string|null} - Workflow name or null
     */
    matchWorkflow(text, context = {}) {
        for (const [name, workflow] of this.workflows.entries()) {
            const { trigger } = workflow;

            if (typeof trigger === 'function') {
                if (trigger(text, context)) return name;
            } else if (trigger instanceof RegExp) {
                if (trigger.test(text)) return name;
            } else if (typeof trigger === 'string') {
                if (text.toLowerCase().includes(trigger.toLowerCase())) return name;
            }
        }

        return null;
    }

    /**
     * Execute a workflow
     * @param {string} workflowName 
     * @param {Object} msg 
     * @param {Object} deps 
     * @returns {Promise<Object>}
     */
    async executeWorkflow(workflowName, msg, deps = {}) {
        const workflow = this.workflows.get(workflowName);
        if (!workflow) {
            throw new Error(`Workflow ${workflowName} not found`);
        }

        const workflowId = `${workflowName}_${Date.now()}`;
        const startTime = Date.now();

        workflow.executions++;

        const state = {
            workflowId,
            workflowName,
            currentStep: 0,
            totalSteps: workflow.steps.length,
            data: {},
            errors: [],
            startTime,
            status: 'running'
        };

        this.activeWorkflows.set(workflowId, state);

        if (deps.logAndSave) {
            deps.logAndSave(`[Orchestrator] Starting workflow: ${workflowName} (${workflow.steps.length} steps)`);
        }

        try {
            // Execute each step
            for (let i = 0; i < workflow.steps.length; i++) {
                state.currentStep = i + 1;
                const step = workflow.steps[i];

                if (deps.logAndSave) {
                    deps.logAndSave(`[Orchestrator] Step ${i + 1}/${workflow.steps.length}: ${step.name || step.type}`);
                }

                const stepResult = await this.executeStep(step, msg, { ...deps, workflowState: state });

                // Store step result in state
                state.data[step.name || `step_${i}`] = stepResult;

                // Check if step failed and has a condition
                if (!stepResult.success && step.required !== false) {
                    throw new Error(`Required step ${step.name} failed: ${stepResult.error}`);
                }

                // Check timeout
                if (Date.now() - startTime > workflow.timeout) {
                    throw new Error(`Workflow timeout after ${workflow.timeout}ms`);
                }
            }

            state.status = 'completed';
            workflow.successes++;

            const elapsed = Date.now() - startTime;
            if (deps.logAndSave) {
                deps.logAndSave(`[Orchestrator] Workflow ${workflowName} completed in ${elapsed}ms`);
            }

            return {
                success: true,
                workflowId,
                workflowName,
                duration: elapsed,
                data: state.data
            };

        } catch (err) {
            state.status = 'failed';
            state.errors.push(err.message);
            workflow.failures++;

            if (deps.logAndSave) {
                deps.logAndSave(`[Orchestrator] Workflow ${workflowName} failed: ${err.message}`);
            }

            // Try fallback if available
            if (workflow.fallback) {
                if (deps.logAndSave) {
                    deps.logAndSave(`[Orchestrator] Attempting fallback for ${workflowName}`);
                }

                try {
                    const fallbackResult = await workflow.fallback(msg, deps, state);
                    return {
                        success: false,
                        workflowId,
                        workflowName,
                        error: err.message,
                        fallbackUsed: true,
                        fallbackResult
                    };
                } catch (fallbackErr) {
                    if (deps.logAndSave) {
                        deps.logAndSave(`[Orchestrator] Fallback also failed: ${fallbackErr.message}`);
                    }
                }
            }

            return {
                success: false,
                workflowId,
                workflowName,
                error: err.message,
                data: state.data
            };

        } finally {
            // Cleanup
            setTimeout(() => {
                this.activeWorkflows.delete(workflowId);
            }, 60000); // Keep for 1 minute for debugging
        }
    }

    /**
     * Execute a single workflow step
     * @param {Object} step 
     * @param {Object} msg 
     * @param {Object} deps 
     * @returns {Promise<Object>}
     */
    async executeStep(step, msg, deps = {}) {
        const { type, name, jobName, handler, params = {} } = step;

        try {
            if (type === 'job') {
                // Execute a registered job
                const handled = await registry.execute(jobName, msg, deps);
                return { success: handled, type: 'job', jobName };

            } else if (type === 'function') {
                // Execute a custom function
                const result = await handler(msg, deps, params);
                return { success: true, type: 'function', result };

            } else if (type === 'conditional') {
                // Execute based on condition
                const condition = await step.condition(msg, deps);
                if (condition) {
                    return await this.executeStep(step.ifTrue, msg, deps);
                } else if (step.ifFalse) {
                    return await this.executeStep(step.ifFalse, msg, deps);
                }
                return { success: true, type: 'conditional', branch: condition ? 'true' : 'false' };

            } else if (type === 'parallel') {
                // Execute multiple steps in parallel
                const results = await Promise.all(
                    step.steps.map(s => this.executeStep(s, msg, deps))
                );
                return { success: true, type: 'parallel', results };

            } else {
                throw new Error(`Unknown step type: ${type}`);
            }

        } catch (err) {
            return { success: false, type, error: err.message };
        }
    }

    /**
     * Get workflow statistics
     */
    getStats() {
        const workflows = [];

        for (const [name, workflow] of this.workflows.entries()) {
            workflows.push({
                name,
                description: workflow.description,
                steps: workflow.steps.length,
                executions: workflow.executions,
                successes: workflow.successes,
                failures: workflow.failures,
                successRate: workflow.executions > 0 
                    ? ((workflow.successes / workflow.executions) * 100).toFixed(2) + '%' 
                    : 'N/A'
            });
        }

        return {
            totalWorkflows: this.workflows.size,
            activeWorkflows: this.activeWorkflows.size,
            workflows
        };
    }

    /**
     * Get active workflow details
     * @param {string} workflowId 
     */
    getWorkflowStatus(workflowId) {
        return this.activeWorkflows.get(workflowId);
    }
}

// Singleton instance
const orchestrator = new JobOrchestrator();

// Define common workflows

// Workflow 1: Complete Tyre Inquiry (Price + Photo + Stock)
orchestrator.defineWorkflow({
    name: 'complete_tyre_inquiry',
    description: 'Provide price, photo, and stock information for a tyre',
    trigger: (text) => {
        const hasSize = /\d{3}\/\d{2}[R\/]\d{2}/i.test(text);
        const wantsComplete = /complete|full|හැම|සියල්ල/.test(text);
        return hasSize && wantsComplete;
    },
    steps: [
        {
            type: 'job',
            name: 'get_price',
            jobName: 'TyrePriceReply',
            required: true
        },
        {
            type: 'job',
            name: 'get_photo',
            jobName: 'TyrePhotoReply',
            required: false
        },
        {
            type: 'job',
            name: 'get_stock',
            jobName: 'TyreQtyReply',
            required: false
        }
    ],
    timeout: 10000
});

// Workflow 2: Vehicle Full Report (History + Recommendations)
orchestrator.defineWorkflow({
    name: 'vehicle_full_report',
    description: 'Get vehicle history and tyre recommendations',
    trigger: (text) => {
        const hasVehicle = /[A-Z]{2,3}[-\s]?\d{4}/i.test(text);
        const wantsReport = /report|history|විස්තර|සියල්ල/.test(text);
        return hasVehicle && wantsReport;
    },
    steps: [
        {
            type: 'job',
            name: 'get_vehicle_invoice',
            jobName: 'VehicleInvoiceReply',
            required: true
        },
        {
            type: 'function',
            name: 'recommend_tyres',
            handler: async (msg, deps) => {
                // Extract vehicle type from previous step and recommend tyres
                const vehicleData = deps.workflowState.data.get_vehicle_invoice;
                // Logic to recommend tyres based on vehicle
                return { recommended: true };
            }
        }
    ],
    timeout: 15000
});

// Workflow 3: Bulk Quotation (Multiple Sizes)
orchestrator.defineWorkflow({
    name: 'bulk_quotation',
    description: 'Generate quotation for multiple tyre sizes',
    trigger: (text) => {
        const sizeCount = (text.match(/\d{3}\/\d{2}[R\/]\d{2}/gi) || []).length;
        const hasQuote = /quotation|quote|ඇස්තමේන්තුව/.test(text);
        return sizeCount >= 2 && hasQuote;
    },
    steps: [
        {
            type: 'function',
            name: 'extract_all_sizes',
            handler: async (msg, deps) => {
                const text = msg.body;
                const sizes = text.match(/\d{3}\/\d{2}[R\/]\d{2}/gi);
                return { sizes };
            }
        },
        {
            type: 'parallel',
            name: 'get_prices_parallel',
            steps: [] // Dynamically populated based on sizes
        },
        {
            type: 'job',
            name: 'generate_pdf',
            jobName: 'TyreQuotationPDF',
            required: true
        }
    ],
    timeout: 20000
});

module.exports = {
    JobOrchestrator,
    orchestrator
};
