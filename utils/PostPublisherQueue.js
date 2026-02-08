const fs = require('fs');
const path = require('path');
const EventEmitter = require('events');

// Simple singleton queue to serialize Facebook publish operations
class PostPublisherQueue extends EventEmitter {
  constructor() {
    super();
    this.queue = [];
    this.running = false;
    this.delayMs = Math.max(0, Number(process.env.WA_POSTER_PUBLISH_DELAY_MS || 2500));
  }

  static getInstance() {
    if (!global.__POST_PUBLISHER_QUEUE__) {
      global.__POST_PUBLISHER_QUEUE__ = new PostPublisherQueue();
    }
    return global.__POST_PUBLISHER_QUEUE__;
  }

  enqueue(task) {
    return new Promise((resolve, reject) => {
      this.queue.push({ task, resolve, reject });
      this._drain();
    });
  }

  async _drain() {
    if (this.running) return;
    this.running = true;
    while (this.queue.length) {
      const { task, resolve, reject } = this.queue.shift();
      try {
        const result = await this._runTask(task);
        resolve(result);
      } catch (e) {
        reject(e);
      }
      if (this.delayMs) await new Promise(r => setTimeout(r, this.delayMs));
    }
    this.running = false;
  }

  async _runTask(task) {
    // Task shape: { caption, imagePath, publishMode }
    const DailyFacebookPostJob = require('../jobs/DailyFacebookPostJob');
    const job = new DailyFacebookPostJob();
    // Ensure job respects publish mode env
    if (task.publishMode) process.env.FB_PUBLISH_MODE = task.publishMode;
    const res = await job.postToFacebook(task.caption, task.imagePath || null);
    return res;
  }
}

module.exports = { PostPublisherQueue };
