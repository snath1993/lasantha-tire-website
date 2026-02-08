// utils/JobScheduler.js
const cron = require('node-cron');
const EventEmitter = require('events');
const path = require('path');
const fs = require('fs');

class JobScheduler extends EventEmitter {
  constructor() {
    super();
    this.jobs = new Map();
    this.schedules = new Map();
    this.configFile = path.join(__dirname, '..', 'jobs-config.json');
    this.statusFile = path.join(__dirname, '..', 'job-status.json');
  }

  async initialize() {
    await this.loadConfig();
    this.setupWatchers();
    return this;
  }

  async loadConfig() {
    try {
      const data = await fs.promises.readFile(this.configFile, 'utf8');
      const config = JSON.parse(data);
      
      // Clear existing schedules
      this.clearAllSchedules();
      
      // Set up new schedules
      Object.entries(config).forEach(([id, job]) => {
        if (job.enabled && job.schedule) {
          this.scheduleJob(id, job);
        }
      });
      
      this.emit('config-loaded', config);
    } catch (error) {
      this.emit('error', { type: 'config-load', error });
      throw error;
    }
  }

  setupWatchers() {
    // Watch config file for changes
    fs.watch(this.configFile, async (eventType) => {
      if (eventType === 'change') {
        try {
          await this.loadConfig();
        } catch (error) {
          this.emit('error', { type: 'config-watch', error });
        }
      }
    });
  }

  scheduleJob(id, job) {
    // Clean up existing schedule if any
    this.clearSchedule(id);
    
    if (!job.enabled || !job.schedule) return;

    try {
      // Validate cron expression
      if (!cron.validate(job.schedule)) {
        throw new Error(`Invalid cron expression: ${job.schedule}`);
      }

      // Create new schedule
      const schedule = cron.schedule(job.schedule, async () => {
        await this.executeJob(id, job);
      }, {
        scheduled: true,
        timezone: job.timezone || 'Asia/Colombo',
        recoverMissedExecutions: true
      });

      this.schedules.set(id, schedule);
      this.jobs.set(id, job);

      // Calculate and store next run time
      const nextRun = this.calculateNextRun(job.schedule);
      this.updateJobStatus(id, { nextRun });

      this.emit('job-scheduled', { id, job, nextRun });
    } catch (error) {
      this.emit('error', { type: 'job-schedule', jobId: id, error });
    }
  }

  clearSchedule(id) {
    const schedule = this.schedules.get(id);
    if (schedule) {
      schedule.stop();
      this.schedules.delete(id);
      this.emit('job-unscheduled', { id });
    }
  }

  clearAllSchedules() {
    this.schedules.forEach((schedule, id) => {
      schedule.stop();
      this.emit('job-unscheduled', { id });
    });
    this.schedules.clear();
    this.jobs.clear();
  }

  calculateNextRun(cronExpression) {
    try {
  const interval = cron.schedule(cronExpression, () => {}, { recoverMissedExecutions: true });
      const next = interval.nextDate();
      interval.stop();
      return next.toISOString();
    } catch (error) {
      this.emit('error', { type: 'next-run-calculation', error });
      return null;
    }
  }

  async executeJob(id, job) {
    const startTime = Date.now();
    const startStatus = {
      status: 'running',
      startedAt: new Date().toISOString(),
      lastRun: new Date().toISOString()
    };

    try {
      await this.updateJobStatus(id, startStatus);
      this.emit('job-started', { id, job });

      // Execute the job
      const result = await job.execute();

      // Update status on success
      const endStatus = {
        status: 'success',
        lastSuccess: new Date().toISOString(),
        lastRunDuration: Date.now() - startTime,
        error: null,
        nextRun: this.calculateNextRun(job.schedule)
      };

      await this.updateJobStatus(id, endStatus);
      this.emit('job-completed', { id, job, result });

    } catch (error) {
      // Update status on failure
      const failStatus = {
        status: 'failed',
        error: error.message,
        lastRunDuration: Date.now() - startTime,
        nextRun: this.calculateNextRun(job.schedule)
      };

      await this.updateJobStatus(id, failStatus);
      this.emit('job-failed', { id, job, error });
    }
  }

  async updateJobStatus(id, updates) {
    try {
      // Read current status
      let status = {};
      try {
        const data = await fs.promises.readFile(this.statusFile, 'utf8');
        status = JSON.parse(data);
      } catch (error) {
        // File might not exist yet
      }

      // Update status
      status[id] = {
        ...(status[id] || {}),
        ...updates,
        updatedAt: new Date().toISOString()
      };

      // Write updated status
      await fs.promises.writeFile(
        this.statusFile,
        JSON.stringify(status, null, 2)
      );

      this.emit('status-updated', { id, updates });

    } catch (error) {
      this.emit('error', { type: 'status-update', jobId: id, error });
    }
  }

  async triggerJob(id) {
    const job = this.jobs.get(id);
    if (!job) {
      throw new Error(`Job ${id} not found`);
    }

    await this.executeJob(id, job);
  }

  async enableJob(id) {
    const job = this.jobs.get(id);
    if (!job) {
      throw new Error(`Job ${id} not found`);
    }

    job.enabled = true;
    this.scheduleJob(id, job);
    await this.updateJobStatus(id, { enabled: true });
  }

  async disableJob(id) {
    const job = this.jobs.get(id);
    if (!job) {
      throw new Error(`Job ${id} not found`);
    }

    job.enabled = false;
    this.clearSchedule(id);
    await this.updateJobStatus(id, { enabled: false });
  }

  getJobStatus(id) {
    try {
      const status = JSON.parse(fs.readFileSync(this.statusFile, 'utf8'));
      return status[id] || null;
    } catch (error) {
      return null;
    }
  }

  getAllJobStatus() {
    try {
      return JSON.parse(fs.readFileSync(this.statusFile, 'utf8'));
    } catch (error) {
      return {};
    }
  }
}

module.exports = new JobScheduler();