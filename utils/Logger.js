// utils/Logger.js
const winston = require('winston');
const path = require('path');
const fs = require('fs');
const DailyRotateFile = require('winston-daily-rotate-file');

class Logger {
  constructor() {
    this.logDir = path.join(__dirname, '..', 'logs');
    this.setupLogDirectory();
    this.createLoggers();
    this.setupErrorHandling();
  }

  setupLogDirectory() {
    // Create logs directory if it doesn't exist
    ['', 'jobs', 'system', 'api', 'whatsapp'].forEach(subDir => {
      const dir = path.join(this.logDir, subDir);
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
      }
    });
  }

  createLoggers() {
    // Create format for console and file outputs
    const consoleFormat = winston.format.combine(
      winston.format.colorize(),
      winston.format.timestamp(),
      winston.format.printf(({ timestamp, level, message, ...meta }) => {
        return `[${timestamp}] ${level}: ${message} ${
          Object.keys(meta).length ? JSON.stringify(meta, null, 2) : ''
        }`;
      })
    );

    const fileFormat = winston.format.combine(
      winston.format.timestamp(),
      winston.format.json()
    );

    // Create main application logger
    this.mainLogger = winston.createLogger({
      level: 'info',
      format: fileFormat,
      defaultMeta: { service: 'whatsapp-sql-api' },
      transports: [
        new winston.transports.Console({ format: consoleFormat }),
        new DailyRotateFile({
          filename: path.join(this.logDir, 'system', 'system-%DATE%.log'),
          datePattern: 'YYYY-MM-DD',
          zippedArchive: true,
          maxSize: '20m',
          maxFiles: '14d'
        })
      ]
    });

    // Create job-specific logger
    this.jobLogger = winston.createLogger({
      level: 'info',
      format: fileFormat,
      defaultMeta: { service: 'job-execution' },
      transports: [
        new winston.transports.Console({ format: consoleFormat }),
        new DailyRotateFile({
          filename: path.join(this.logDir, 'jobs', 'jobs-%DATE%.log'),
          datePattern: 'YYYY-MM-DD',
          zippedArchive: true,
          maxSize: '20m',
          maxFiles: '14d'
        })
      ]
    });

    // Create WhatsApp-specific logger
    this.whatsappLogger = winston.createLogger({
      level: 'info',
      format: fileFormat,
      defaultMeta: { service: 'whatsapp' },
      transports: [
        new winston.transports.Console({ format: consoleFormat }),
        new DailyRotateFile({
          filename: path.join(this.logDir, 'whatsapp', 'whatsapp-%DATE%.log'),
          datePattern: 'YYYY-MM-DD',
          zippedArchive: true,
          maxSize: '20m',
          maxFiles: '14d'
        })
      ]
    });

    // Create API logger
    this.apiLogger = winston.createLogger({
      level: 'info',
      format: fileFormat,
      defaultMeta: { service: 'api' },
      transports: [
        new winston.transports.Console({ format: consoleFormat }),
        new DailyRotateFile({
          filename: path.join(this.logDir, 'api', 'api-%DATE%.log'),
          datePattern: 'YYYY-MM-DD',
          zippedArchive: true,
          maxSize: '20m',
          maxFiles: '14d'
        })
      ]
    });
  }

  setupErrorHandling() {
    // Handle uncaught exceptions and unhandled rejections
    process.on('uncaughtException', (error) => {
      this.mainLogger.error('Uncaught Exception:', error);
    });

    process.on('unhandledRejection', (reason, promise) => {
      this.mainLogger.error('Unhandled Rejection:', { reason, promise });
    });
  }

  // Main logging methods
  log(level, message, meta = {}) {
    this.mainLogger[level](message, meta);
  }

  logJob(jobId, level, message, meta = {}) {
    this.jobLogger[level](message, { jobId, ...meta });
  }

  logWhatsApp(level, message, meta = {}) {
    this.whatsappLogger[level](message, meta);
  }

  logAPI(level, message, meta = {}) {
    this.apiLogger[level](message, meta);
  }

  // Utility methods
  async getTodaysLogs(type = 'system') {
    const today = new Date().toISOString().split('T')[0];
    const logFile = path.join(this.logDir, type, `${type}-${today}.log`);
    
    try {
      if (fs.existsSync(logFile)) {
        const content = await fs.promises.readFile(logFile, 'utf8');
        return content.split('\n').filter(line => line.trim());
      }
      return [];
    } catch (error) {
      this.mainLogger.error('Error reading logs:', error);
      return [];
    }
  }

  async getJobLogs(jobId, days = 1) {
    const logs = [];
    const now = new Date();
    
    for (let i = 0; i < days; i++) {
      const date = new Date(now - i * 24 * 60 * 60 * 1000);
      const dateStr = date.toISOString().split('T')[0];
      const logFile = path.join(this.logDir, 'jobs', `jobs-${dateStr}.log`);
      
      try {
        if (fs.existsSync(logFile)) {
          const content = await fs.promises.readFile(logFile, 'utf8');
          const jobLogs = content
            .split('\n')
            .filter(line => line.trim())
            .map(line => JSON.parse(line))
            .filter(log => log.jobId === jobId);
          logs.push(...jobLogs);
        }
      } catch (error) {
        this.mainLogger.error('Error reading job logs:', error);
      }
    }
    
    return logs;
  }

  async cleanOldLogs(maxAgeDays = 30) {
    const types = ['system', 'jobs', 'whatsapp', 'api'];
    const cutoffDate = new Date();
    cutoffDate.setDate(cutoffDate.getDate() - maxAgeDays);

    for (const type of types) {
      const dir = path.join(this.logDir, type);
      try {
        const files = await fs.promises.readdir(dir);
        for (const file of files) {
          const filePath = path.join(dir, file);
          const stats = await fs.promises.stat(filePath);
          
          if (stats.mtime < cutoffDate) {
            await fs.promises.unlink(filePath);
            this.mainLogger.info(`Deleted old log file: ${file}`);
          }
        }
      } catch (error) {
        this.mainLogger.error(`Error cleaning old logs in ${type}:`, error);
      }
    }
  }
}

// Export singleton instance
module.exports = new Logger();