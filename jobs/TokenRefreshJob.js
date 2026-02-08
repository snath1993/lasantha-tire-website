// Daily Token Validity Check Job
const cron = require('node-cron');
const FacebookTokenManager = require('../utils/FacebookTokenManager');

class TokenRefreshJob {
  constructor() {
    this.task = null;
    this.tokenManager = new FacebookTokenManager();
    
    // Alert thresholds (days)
    this.WARNING_THRESHOLD = 7;  // Warn if < 7 days
    this.CRITICAL_THRESHOLD = 3; // Critical if < 3 days
  }
  
  start() {
    console.log('🔐 Starting Facebook Token Monitor Job...');
    
    // Check token validity every 6 hours
    this.task = cron.schedule('0 */6 * * *', () => {
      this.checkToken();
    }, {
      timezone: 'Asia/Colombo',
      recoverMissedExecutions: true
    });
    
    console.log('✅ Token monitor: Every 6 hours');
    
    // Run immediately on startup
    setTimeout(() => this.checkToken(), 5000);
  }
  
  stop() {
    if (this.task) {
      this.task.stop();
      console.log('⏹️ TokenRefreshJob stopped');
    }
  }
  
  async checkToken() {
    try {
      console.log('\n🔐 Running Facebook token validity check...');
      
      const validity = await this.tokenManager.checkTokenValidity();
      
      if (!validity.valid) {
        console.error('❌ TOKEN INVALID:', validity.reason);
        console.error('⚠️  ACTION REQUIRED: Please refresh Facebook token!');
        console.error('📖 Guide: See FACEBOOK_LONG_LIVED_TOKEN_GUIDE.md');
        
        // TODO: Send WhatsApp alert to admin
        // await this.sendAlertToAdmin('CRITICAL', 'Facebook token invalid!');
        
        return;
      }
      
      // Check expiry
      if (validity.daysRemaining !== null) {
        if (validity.daysRemaining <= this.CRITICAL_THRESHOLD) {
          console.error(`🚨 CRITICAL: Token expires in ${validity.daysRemaining} days!`);
          console.error('⚠️  ACTION REQUIRED: Refresh token immediately!');
          // TODO: Send critical WhatsApp alert
          
        } else if (validity.daysRemaining <= this.WARNING_THRESHOLD) {
          console.warn(`⚠️  WARNING: Token expires in ${validity.daysRemaining} days`);
          console.warn('📝 Reminder: Refresh token soon');
          // TODO: Send warning WhatsApp message
          
        } else {
          console.log(`✅ Token healthy: ${validity.daysRemaining} days remaining`);
        }
      } else {
        console.log('✅ Token healthy: Long-lived or permanent');
      }
      
      console.log('');
      
    } catch (err) {
      console.error('❌ Token check failed:', err.message);
    }
  }
  
  // Get token status for API/dashboard
  getStatus() {
    return this.tokenManager.getStatus();
  }
  
  // Manual refresh (admin triggered)
  async refreshToken(shortLivedToken) {
    try {
      console.log('🔄 Manual token refresh requested...');
      const result = await this.tokenManager.refreshToken(shortLivedToken);
      console.log('✅ Manual refresh successful!');
      return result;
    } catch (err) {
      console.error('❌ Manual refresh failed:', err.message);
      throw err;
    }
  }
}

module.exports = TokenRefreshJob;

// Test if run directly
if (require.main === module) {
  require('dotenv').config();
  const job = new TokenRefreshJob();
  job.checkToken()
    .then(() => {
      console.log('✅ Test complete!');
      process.exit(0);
    })
    .catch(err => {
      console.error('❌ Test failed:', err);
      process.exit(1);
    });
}
