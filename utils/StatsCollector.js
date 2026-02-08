// utils/StatsCollector.js
const EventEmitter = require('events');
const path = require('path');
const fs = require('fs');
const os = require('os');
const sql = require('mssql');
const Logger = require('./Logger');

class StatsCollector extends EventEmitter {
  constructor() {
    super();
    this.stats = {
      system: {
        startTime: new Date().toISOString(),
        uptime: 0,
        memoryUsage: {},
        cpuUsage: [],
        nodeVersion: process.version
      },
      jobs: {
        total: 0,
        running: 0,
        completed: 0,
        failed: 0,
        queued: 0
      },
      whatsapp: {
        messagesReceived: 0,
        messagesSent: 0,
        errors: 0,
        lastMessageAt: null,
        connectionStatus: 'disconnected'
      },
      database: {
        connections: 0,
        queries: 0,
        errors: 0,
        avgResponseTime: 0,
        lastError: null
      },
      errors: {
        total: 0,
        lastError: null,
        byCategory: {}
      }
    };

    this.statsFile = path.join(__dirname, '..', 'stats', 'runtime-stats.json');
    this.historyDir = path.join(__dirname, '..', 'stats', 'history');
    
    // Ensure directories exist
    [
      path.join(__dirname, '..', 'stats'),
      this.historyDir
    ].forEach(dir => {
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
      }
    });

    // Initialize collectors
    this.initializeCollectors();
  }

  initializeCollectors() {
    // Collect system stats every minute
    setInterval(() => this.collectSystemStats(), 60000);

    // Update job stats every 30 seconds
    setInterval(() => this.collectJobStats(), 30000);

    // Save stats every 5 minutes
    setInterval(() => this.saveStats(), 300000);

    // Archive daily stats at midnight
    this.scheduleDailyArchive();

    // Initialize SQL query monitoring
    this.monitorSQLQueries();
  }

  async collectSystemStats() {
    try {
      // Update uptime
      this.stats.system.uptime = process.uptime();

      // Memory usage
      const used = process.memoryUsage();
      this.stats.system.memoryUsage = {
        heapTotal: Math.round(used.heapTotal / 1024 / 1024),
        heapUsed: Math.round(used.heapUsed / 1024 / 1024),
        external: Math.round(used.external / 1024 / 1024),
        rss: Math.round(used.rss / 1024 / 1024)
      };

      // CPU usage
      const cpus = os.cpus();
      const cpuUsage = cpus.map(cpu => {
        const total = Object.values(cpu.times).reduce((acc, tv) => acc + tv, 0);
        const idle = cpu.times.idle;
        return 100 - (idle / total * 100);
      });

      this.stats.system.cpuUsage = cpuUsage;

      this.emit('system-stats-updated', this.stats.system);
    } catch (error) {
      Logger.log('error', 'Error collecting system stats', error);
    }
  }

  async collectJobStats() {
    try {
      const statusFile = path.join(__dirname, '..', 'job-status.json');
      if (fs.existsSync(statusFile)) {
        const status = JSON.parse(await fs.promises.readFile(statusFile, 'utf8'));
        
        // Reset counters
        this.stats.jobs = {
          total: Object.keys(status).length,
          running: 0,
          completed: 0,
          failed: 0,
          queued: 0
        };

        // Count job states
        Object.values(status).forEach(job => {
          if (job.status === 'running') this.stats.jobs.running++;
          else if (job.status === 'completed') this.stats.jobs.completed++;
          else if (job.status === 'failed') this.stats.jobs.failed++;
          else if (job.status === 'queued') this.stats.jobs.queued++;
        });

        this.emit('job-stats-updated', this.stats.jobs);
      }
    } catch (error) {
      Logger.log('error', 'Error collecting job stats', error);
    }
  }

  monitorSQLQueries() {
    // Monitor SQL queries using event emitter
    const oldRequest = sql.Request;
    sql.Request = function() {
      const request = new oldRequest();
      const oldQuery = request.query;
      request.query = async function(...args) {
        const start = Date.now();
        try {
          const result = await oldQuery.apply(this, args);
          StatsCollector.instance.recordQuerySuccess(Date.now() - start);
          return result;
        } catch (error) {
          StatsCollector.instance.recordQueryError(error);
          throw error;
        }
      };
      return request;
    };
  }

  recordQuerySuccess(duration) {
    this.stats.database.queries++;
    this.stats.database.avgResponseTime = 
      (this.stats.database.avgResponseTime * (this.stats.database.queries - 1) + duration) 
      / this.stats.database.queries;
    
    this.emit('database-stats-updated', this.stats.database);
  }

  recordQueryError(error) {
    this.stats.database.errors++;
    this.stats.database.lastError = {
      message: error.message,
      timestamp: new Date().toISOString()
    };

    // Update error stats
    this.recordError('database', error);
    
    this.emit('database-stats-updated', this.stats.database);
  }

  updateWhatsAppStats(type, data = {}) {
    switch (type) {
      case 'message-received':
        this.stats.whatsapp.messagesReceived++;
        this.stats.whatsapp.lastMessageAt = new Date().toISOString();
        break;
      case 'message-sent':
        this.stats.whatsapp.messagesSent++;
        this.stats.whatsapp.lastMessageAt = new Date().toISOString();
        break;
      case 'error':
        this.stats.whatsapp.errors++;
        this.recordError('whatsapp', data.error);
        break;
      case 'connection-status':
        this.stats.whatsapp.connectionStatus = data.status;
        break;
    }

    this.emit('whatsapp-stats-updated', this.stats.whatsapp);
  }

  recordError(category, error) {
    this.stats.errors.total++;
    this.stats.errors.lastError = {
      category,
      message: error.message,
      timestamp: new Date().toISOString()
    };

    this.stats.errors.byCategory[category] = 
      (this.stats.errors.byCategory[category] || 0) + 1;

    this.emit('error-stats-updated', this.stats.errors);
  }

  async saveStats() {
    try {
      await fs.promises.writeFile(
        this.statsFile,
        JSON.stringify(this.stats, null, 2)
      );
    } catch (error) {
      Logger.log('error', 'Error saving stats', error);
    }
  }

  scheduleDailyArchive() {
    const now = new Date();
    const nextMidnight = new Date(
      now.getFullYear(),
      now.getMonth(),
      now.getDate() + 1
    );
    
    const msUntilMidnight = nextMidnight - now;
    
    setTimeout(() => {
      this.archiveStats();
      // Reschedule for next day
      this.scheduleDailyArchive();
    }, msUntilMidnight);
  }

  async archiveStats() {
    const date = new Date().toISOString().split('T')[0];
    const archiveFile = path.join(this.historyDir, `stats-${date}.json`);
    
    try {
      // Save current stats to archive
      await fs.promises.writeFile(
        archiveFile,
        JSON.stringify(this.stats, null, 2)
      );

      // Reset certain stats
      this.stats.jobs.completed = 0;
      this.stats.jobs.failed = 0;
      this.stats.whatsapp.messagesReceived = 0;
      this.stats.whatsapp.messagesSent = 0;
      this.stats.whatsapp.errors = 0;
      this.stats.database.queries = 0;
      this.stats.database.errors = 0;
      this.stats.database.avgResponseTime = 0;
      this.stats.errors = {
        total: 0,
        lastError: null,
        byCategory: {}
      };

      // Save reset stats
      await this.saveStats();
    } catch (error) {
      Logger.log('error', 'Error archiving stats', error);
    }
  }

  getStats() {
    return this.stats;
  }

  async getHistoricalStats(days = 7) {
    const history = [];
    const now = new Date();
    
    for (let i = 0; i < days; i++) {
      const date = new Date(now - i * 24 * 60 * 60 * 1000);
      const dateStr = date.toISOString().split('T')[0];
      const archiveFile = path.join(this.historyDir, `stats-${dateStr}.json`);
      
      try {
        if (fs.existsSync(archiveFile)) {
          const stats = JSON.parse(await fs.promises.readFile(archiveFile, 'utf8'));
          history.push({ date: dateStr, stats });
        }
      } catch (error) {
        Logger.log('error', `Error reading stats for ${dateStr}`, error);
      }
    }
    
    return history;
  }
}

// Create singleton instance
const instance = new StatsCollector();
StatsCollector.instance = instance;
module.exports = instance;