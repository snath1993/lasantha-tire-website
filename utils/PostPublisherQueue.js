/**
 * Post Publisher Queue
 * Serialized queue for Facebook post publishing
 */

class PostPublisherQueue {
    constructor() {
        this.queue = [];
        this.processing = false;
    }
    
    static getInstance() {
        if (!PostPublisherQueue.instance) {
            PostPublisherQueue.instance = new PostPublisherQueue();
        }
        return PostPublisherQueue.instance;
    }
    
    async enqueue(postData) {
        return new Promise((resolve, reject) => {
            this.queue.push({ postData, resolve, reject });
            this.processQueue();
        });
    }
    
    async processQueue() {
        if (this.processing || this.queue.length === 0) {
            return;
        }
        
        this.processing = true;
        const { postData, resolve, reject } = this.queue.shift();
        
        try {
            // Mock publish - in production this would publish to Facebook
            const result = {
                id: `post_${Date.now()}`,
                caption: postData.caption,
                imagePath: postData.imagePath
            };
            
            console.log('[PostPublisher] Published post:', result.id);
            resolve(result);
        } catch (error) {
            console.error('[PostPublisher] Error:', error.message);
            reject(error);
        } finally {
            this.processing = false;
            this.processQueue();
        }
    }
}

module.exports = {
    PostPublisherQueue
};
