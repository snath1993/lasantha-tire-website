// utils/BaseJob.js
const EventEmitter = require('events');
const path = require('path');
const fs = require('fs');

class BaseJob extends EventEmitter {
  constructor(id, options = {}) {
    super();
    this.id = id;
    this.name = options.name || id;
    this.description = options.description || '';
    this.schedule = options.schedule || null;
    this.enabled = options.enabled !== false;
    this.type = options.type || 'maintenance';
    this.icon = options.icon || '🔄';
    this.timezone = options.timezone || 'Asia/Colombo';
    this.retryAttempts = options.retryAttempts || 3;
    this.retryDelay = options.retryDelay || 5000;
    this.timeout = options.timeout || 30 * 60 * 1000; // 30 minutes
    this.contactNumbers = options.contactNumbers || [];
  }

  async execute() {
    let lastError = null;

    for (let attempt = 1; attempt <= this.retryAttempts; attempt++) {
      try {
        // Start execution timer
        const timeoutPromise = new Promise((_, reject) => {
          setTimeout(() => reject(new Error('Job execution timed out')), this.timeout);
        });

        // Run the job with timeout
        const result = await Promise.race([
          this.run(),
          timeoutPromise
        ]);

        // If successful, return result
        return result;

      } catch (error) {
        lastError = error;
        this.emit('attempt-failed', { 
          attempt, 
          error, 
          willRetry: attempt < this.retryAttempts 
        });

        if (attempt < this.retryAttempts) {
          await new Promise(resolve => setTimeout(resolve, this.retryDelay));
        }
      }
    }

    // If all attempts failed, throw the last error
    throw lastError;
  }

  async run() {
    throw new Error('run() method must be implemented by child class');
  }

  async validateConfig() {
    // Override this method to add custom validation
    return true;
  }

  async beforeRun() {
    // Hook for setup before running the job
  }

  async afterRun(result) {
    // Hook for cleanup after running the job
  }

  async onError(error) {
    // Hook for custom error handling
  }

  // Utility methods
  async updateStatus(status) {
    const statusFile = path.join(__dirname, '..', 'job-status.json');
    try {
      let currentStatus = {};
      try {
        currentStatus = JSON.parse(await fs.promises.readFile(statusFile, 'utf8'));
      } catch (error) {
        // File might not exist yet
      }

      currentStatus[this.id] = {
        ...(currentStatus[this.id] || {}),
        ...status,
        updatedAt: new Date().toISOString()
      };

      await fs.promises.writeFile(
        statusFile,
        JSON.stringify(currentStatus, null, 2)
      );

    } catch (error) {
      console.error(`Failed to update status for job ${this.id}:`, error);
    }
  }

  log(message, level = 'info') {
    const timestamp = new Date().toISOString();
    const logEntry = {
      timestamp,
      jobId: this.id,
      level,
      message
    };

    this.emit('log', logEntry);

    // Also write to job-specific log file
    const logDir = path.join(__dirname, '..', 'logs', 'jobs');
    const logFile = path.join(logDir, `${this.id}.log`);

    try {
      if (!fs.existsSync(logDir)) {
        fs.mkdirSync(logDir, { recursive: true });
      }

      fs.appendFileSync(
        logFile,
        `[${timestamp}] [${level.toUpperCase()}] ${message}\n`
      );

    } catch (error) {
      console.error(`Failed to write log for job ${this.id}:`, error);
    }
  }

  // Configuration management
  async loadConfig() {
    const configFile = path.join(__dirname, '..', 'jobs-config.json');
    try {
      const config = JSON.parse(await fs.promises.readFile(configFile, 'utf8'));
      return config[this.id] || {};
    } catch (error) {
      this.log(`Failed to load config: ${error.message}`, 'error');
      return {};
    }
  }

  async saveConfig(config) {
    const configFile = path.join(__dirname, '..', 'jobs-config.json');
    try {
      let fullConfig = {};
      try {
        fullConfig = JSON.parse(await fs.promises.readFile(configFile, 'utf8'));
      } catch (error) {
        // File might not exist yet
      }

      fullConfig[this.id] = {
        ...fullConfig[this.id],
        ...config,
        updatedAt: new Date().toISOString()
      };

      await fs.promises.writeFile(
        configFile,
        JSON.stringify(fullConfig, null, 2)
      );

    } catch (error) {
      this.log(`Failed to save config: ${error.message}`, 'error');
      throw error;
    }
  }

  // WhatsApp integration
  async sendWhatsAppMessage(message) {
    if (!this.contactNumbers || this.contactNumbers.length === 0) {
      this.log('No contact numbers configured for notifications', 'warn');
      return;
    }

    const { send } = require('./waClientRegistry');
    
    for (const number of this.contactNumbers) {
      try {
        const result = await send(number, message);
        if (!result.ok) {
          this.log(`Failed to send WhatsApp message to ${number}: ${result.error}`, 'error');
        }
      } catch (error) {
        this.log(`Error sending WhatsApp message to ${number}: ${error.message}`, 'error');
      }
    }
  }
}

module.exports = BaseJob;