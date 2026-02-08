const EventEmitter = require('events');

/**
 * Advanced Job Execution Manager
 * Prevents job conflicts and manages concurrent execution with priority system
 * 
 * Features:
 * - Prevents multiple instances of the same job running simultaneously
 * - Priority-based queue for job execution
 * - Resource-based conflict detection (database, WhatsApp, Facebook API)
 * - Automatic retry with exponential backoff
 * - Execution history and monitoring
 */
class JobExecutionManager extends EventEmitter {
    constructor() {
        super();
        this.runningJobs = new Map(); // jobName -> { startTime, pid, resources }
        this.jobQueue = []; // Priority queue: { jobName, priority, fn, attempt, resources }
        this.executionHistory = []; // Last 100 executions
        this.maxHistorySize = 100;
        this.isProcessingQueue = false;
        
        // Resource locks (prevent conflicts)
        this.resourceLocks = {
            database: false,      // Heavy database operations (reports, deletion)
            whatsapp: false,      // WhatsApp client operations
            facebook: false,      // Facebook API calls
            fileSystem: false,    // File operations (image generation)
        };
        
        // Job priority levels (higher = more important)
        this.priorities = {
            CRITICAL: 100,    // Invoice deletion, system operations
            HIGH: 75,         // Real-time monitoring, customer replies
            NORMAL: 50,       // Scheduled reports, daily jobs
            LOW: 25,          // Background tasks, cleanup
        };
        
        // Job resource requirements
        this.jobResources = {
            // Critical jobs
            'InvoiceDeletionJob': ['database'],
            'DeletionQueueProcessorJob': ['database'],
            
            // High priority
            'CostPriceReplyJob': ['whatsapp', 'database'],
            'TyrePriceReplyJob': ['whatsapp', 'database'],
            'TyreQtyReplyJob': ['whatsapp', 'database'],
            'VehicleDetailsReplyJob': ['whatsapp', 'database'],
            'VehicleInvoiceReplyJob': ['whatsapp', 'database'],
            'TyreQuotationPDFJob': ['whatsapp', 'fileSystem', 'database'],
            'TyreQuotationPDFLibJob': ['whatsapp', 'fileSystem', 'database'],
            'TyreQuotationPDFLibJobPublic': ['whatsapp', 'fileSystem', 'database'],
            'WatchedItemRealtimeJob': ['whatsapp', 'database'],
            
            // Normal priority
            'DailyTyreSalesReportJob': ['whatsapp', 'database'],
            'DailyFacebookPostJob': ['facebook', 'fileSystem'],
            'FacebookCommentMonitorJob': ['facebook'],
            'FacebookCommentResponderJob': ['facebook', 'whatsapp'],
            'FacebookMessengerResponderJob': ['facebook', 'whatsapp'],
            
            // Low priority
            'WeeklyTyreSalesReportJob': ['database'],
            'MonthlyTyreSalesReportJob': ['database'],
            'MonthlySalesPDFReportJob': ['database', 'fileSystem'],
            'TokenRefreshJob': ['facebook'],
            'TyrePhotoReplyJob': ['whatsapp', 'fileSystem'],
        };
        
        // Job priorities
        this.jobPriorities = {
            // Critical
            'InvoiceDeletionJob': this.priorities.CRITICAL,
            'DeletionQueueProcessorJob': this.priorities.CRITICAL,
            
            // High
            'CostPriceReplyJob': this.priorities.HIGH,
            'TyrePriceReplyJob': this.priorities.HIGH,
            'TyreQtyReplyJob': this.priorities.HIGH,
            'VehicleDetailsReplyJob': this.priorities.HIGH,
            'VehicleInvoiceReplyJob': this.priorities.HIGH,
            'TyreQuotationPDFJob': this.priorities.HIGH,
            'TyreQuotationPDFLibJob': this.priorities.HIGH,
            'TyreQuotationPDFLibJobPublic': this.priorities.HIGH,
            'WatchedItemRealtimeJob': this.priorities.HIGH,
            
            // Normal
            'DailyTyreSalesReportJob': this.priorities.NORMAL,
            'DailyFacebookPostJob': this.priorities.NORMAL,
            'FacebookCommentMonitorJob': this.priorities.NORMAL,
            'FacebookCommentResponderJob': this.priorities.NORMAL,
            'FacebookMessengerResponderJob': this.priorities.NORMAL,
            
            // Low
            'WeeklyTyreSalesReportJob': this.priorities.LOW,
            'MonthlyTyreSalesReportJob': this.priorities.LOW,
            'MonthlySalesPDFReportJob': this.priorities.LOW,
            'TokenRefreshJob': this.priorities.LOW,
            'TyrePhotoReplyJob': this.priorities.LOW,
        };
        
        console.log('🔧 JobExecutionManager initialized - Advanced conflict prevention enabled');
    }
    
    /**
     * Check if job can run (not already running and resources available)
     */
    canJobRun(jobName, resources = []) {
        // Check if already running
        if (this.runningJobs.has(jobName)) {
            return { canRun: false, reason: 'already_running' };
        }
        
        // Check resource conflicts
        for (const resource of resources) {
            if (this.resourceLocks[resource]) {
                return { canRun: false, reason: `resource_locked_${resource}` };
            }
        }
        
        return { canRun: true };
    }
    
    /**
     * Lock resources for a job
     */
    lockResources(jobName, resources = []) {
        for (const resource of resources) {
            this.resourceLocks[resource] = jobName;
        }
    }
    
    /**
     * Unlock resources after job completion
     */
    unlockResources(jobName, resources = []) {
        for (const resource of resources) {
            if (this.resourceLocks[resource] === jobName) {
                this.resourceLocks[resource] = false;
            }
        }
    }
    
    /**
     * Execute a job with conflict prevention
     */
    async executeJob(jobName, jobFunction, options = {}) {
        const resources = options.resources || this.jobResources[jobName] || [];
        const priority = options.priority || this.jobPriorities[jobName] || this.priorities.NORMAL;
        const attempt = options.attempt || 1;
        const maxRetries = options.maxRetries || 3;
        
        // Check if can run immediately
        const canRun = this.canJobRun(jobName, resources);
        
        if (!canRun.canRun) {
            if (canRun.reason === 'already_running') {
                console.log(`⏸️  Job ${jobName} is already running. Skipping duplicate execution.`);
                return { status: 'skipped', reason: 'already_running' };
            } else {
                // Add to queue if resources are locked
                console.log(`⏳ Job ${jobName} waiting for resources: ${canRun.reason}`);
                return this.queueJob(jobName, jobFunction, { resources, priority, attempt, maxRetries });
            }
        }
        
        // Mark as running and lock resources
        const startTime = Date.now();
        this.runningJobs.set(jobName, { startTime, resources, priority });
        this.lockResources(jobName, resources);
        
        console.log(`🚀 Executing job: ${jobName} (Priority: ${priority}, Resources: ${resources.join(', ') || 'none'})`);
        
        let result = null;
        let error = null;
        
        try {
            // Execute the job
            result = await jobFunction();
            
            // Record success
            const duration = Date.now() - startTime;
            this.recordExecution(jobName, 'success', duration, attempt);
            
            console.log(`✅ Job completed: ${jobName} (${duration}ms)`);
            this.emit('jobComplete', { jobName, duration, result });
            
        } catch (err) {
            error = err;
            const duration = Date.now() - startTime;
            this.recordExecution(jobName, 'error', duration, attempt, err.message);
            
            console.error(`❌ Job failed: ${jobName} (${duration}ms) - ${err.message}`);
            this.emit('jobError', { jobName, duration, error: err });
            
            // Retry logic with exponential backoff
            if (attempt < maxRetries) {
                const retryDelay = Math.min(1000 * Math.pow(2, attempt), 30000); // Max 30s
                console.log(`🔄 Retrying ${jobName} in ${retryDelay}ms (Attempt ${attempt + 1}/${maxRetries})`);
                
                setTimeout(() => {
                    this.executeJob(jobName, jobFunction, {
                        resources,
                        priority,
                        attempt: attempt + 1,
                        maxRetries
                    });
                }, retryDelay);
            }
            
        } finally {
            // Cleanup - remove from running and unlock resources
            this.runningJobs.delete(jobName);
            this.unlockResources(jobName, resources);
            
            // Process queue to check if waiting jobs can now run
            this.processQueue();
        }
        
        return { status: error ? 'error' : 'success', result, error };
    }
    
    /**
     * Add job to priority queue
     */
    queueJob(jobName, jobFunction, options) {
        const queueItem = {
            jobName,
            jobFunction,
            priority: options.priority || this.priorities.NORMAL,
            resources: options.resources || [],
            attempt: options.attempt || 1,
            maxRetries: options.maxRetries || 3,
            queuedAt: Date.now()
        };
        
        // Insert into queue based on priority (higher priority first)
        let inserted = false;
        for (let i = 0; i < this.jobQueue.length; i++) {
            if (queueItem.priority > this.jobQueue[i].priority) {
                this.jobQueue.splice(i, 0, queueItem);
                inserted = true;
                break;
            }
        }
        
        if (!inserted) {
            this.jobQueue.push(queueItem);
        }
        
        console.log(`📋 Job queued: ${jobName} (Position: ${this.jobQueue.findIndex(j => j.jobName === jobName) + 1}, Queue size: ${this.jobQueue.length})`);
        
        // Try to process queue
        this.processQueue();
        
        return { status: 'queued', position: this.jobQueue.findIndex(j => j.jobName === jobName) + 1 };
    }
    
    /**
     * Process queued jobs
     */
    async processQueue() {
        if (this.isProcessingQueue || this.jobQueue.length === 0) {
            return;
        }
        
        this.isProcessingQueue = true;
        
        try {
            // Try to execute jobs from queue
            let i = 0;
            while (i < this.jobQueue.length) {
                const queueItem = this.jobQueue[i];
                const canRun = this.canJobRun(queueItem.jobName, queueItem.resources);
                
                if (canRun.canRun) {
                    // Remove from queue and execute
                    this.jobQueue.splice(i, 1);
                    
                    const waitTime = Date.now() - queueItem.queuedAt;
                    console.log(`▶️  Starting queued job: ${queueItem.jobName} (Waited: ${waitTime}ms)`);
                    
                    // Execute without awaiting (run in background)
                    this.executeJob(queueItem.jobName, queueItem.jobFunction, {
                        resources: queueItem.resources,
                        priority: queueItem.priority,
                        attempt: queueItem.attempt,
                        maxRetries: queueItem.maxRetries
                    });
                } else {
                    i++; // Check next job in queue
                }
            }
        } finally {
            this.isProcessingQueue = false;
        }
    }
    
    /**
     * Record job execution history
     */
    recordExecution(jobName, status, duration, attempt = 1, errorMessage = null) {
        const record = {
            jobName,
            status,
            duration,
            attempt,
            errorMessage,
            timestamp: new Date().toISOString()
        };
        
        this.executionHistory.unshift(record);
        
        // Keep only last 100 executions
        if (this.executionHistory.length > this.maxHistorySize) {
            this.executionHistory = this.executionHistory.slice(0, this.maxHistorySize);
        }
    }
    
    /**
     * Get current system status
     */
    getStatus() {
        return {
            runningJobs: Array.from(this.runningJobs.entries()).map(([name, info]) => ({
                name,
                duration: Date.now() - info.startTime,
                resources: info.resources,
                priority: info.priority
            })),
            queuedJobs: this.jobQueue.map(item => ({
                name: item.jobName,
                priority: item.priority,
                waitTime: Date.now() - item.queuedAt,
                resources: item.resources
            })),
            resourceLocks: Object.entries(this.resourceLocks)
                .filter(([_, locked]) => locked)
                .map(([resource, jobName]) => ({ resource, lockedBy: jobName })),
            recentExecutions: this.executionHistory.slice(0, 10)
        };
    }
    
    /**
     * Get statistics
     */
    getStatistics() {
        const stats = {
            totalExecutions: this.executionHistory.length,
            successCount: this.executionHistory.filter(e => e.status === 'success').length,
            errorCount: this.executionHistory.filter(e => e.status === 'error').length,
            averageDuration: 0,
            jobStats: {}
        };
        
        if (this.executionHistory.length > 0) {
            const totalDuration = this.executionHistory.reduce((sum, e) => sum + e.duration, 0);
            stats.averageDuration = Math.round(totalDuration / this.executionHistory.length);
            
            // Per-job statistics
            const jobGroups = {};
            for (const exec of this.executionHistory) {
                if (!jobGroups[exec.jobName]) {
                    jobGroups[exec.jobName] = { executions: 0, successes: 0, errors: 0, totalDuration: 0 };
                }
                jobGroups[exec.jobName].executions++;
                jobGroups[exec.jobName].totalDuration += exec.duration;
                if (exec.status === 'success') jobGroups[exec.jobName].successes++;
                if (exec.status === 'error') jobGroups[exec.jobName].errors++;
            }
            
            for (const [jobName, data] of Object.entries(jobGroups)) {
                stats.jobStats[jobName] = {
                    executions: data.executions,
                    successRate: `${Math.round((data.successes / data.executions) * 100)}%`,
                    averageDuration: Math.round(data.totalDuration / data.executions),
                    errors: data.errors
                };
            }
        }
        
        return stats;
    }
    
    /**
     * Force cancel a running job (emergency use only)
     */
    forceCancel(jobName) {
        if (!this.runningJobs.has(jobName)) {
            return { success: false, message: 'Job not running' };
        }
        
        const jobInfo = this.runningJobs.get(jobName);
        this.runningJobs.delete(jobName);
        this.unlockResources(jobName, jobInfo.resources);
        
        console.warn(`⚠️  Force cancelled job: ${jobName}`);
        this.emit('jobCancelled', { jobName, forced: true });
        
        return { success: true, message: 'Job cancelled and resources released' };
    }
    
    /**
     * Clear queue (emergency use only)
     */
    clearQueue() {
        const count = this.jobQueue.length;
        this.jobQueue = [];
        console.warn(`⚠️  Cleared job queue (${count} jobs removed)`);
        return { success: true, cleared: count };
    }
}

// Singleton instance
let instance = null;

module.exports = {
    JobExecutionManager,
    getInstance: () => {
        if (!instance) {
            instance = new JobExecutionManager();
        }
        return instance;
    }
};
