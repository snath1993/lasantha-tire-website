// Facebook Token Manager (Automatic Refresh System)
const axios = require('axios');
const fs = require('fs');
const path = require('path');

class FacebookTokenManager {
  constructor() {
    this.appId = process.env.FACEBOOK_APP_ID;
    this.appSecret = process.env.FACEBOOK_APP_SECRET;
    this.pageId = process.env.FACEBOOK_PAGE_ID;
    this.currentToken = process.env.FACEBOOK_PAGE_ACCESS_TOKEN;
    
    // Token state file
    this.stateFile = path.join(__dirname, '../facebook-token-state.json');
    this.loadState();
  }
  
  loadState() {
    try {
      if (fs.existsSync(this.stateFile)) {
        this.state = JSON.parse(fs.readFileSync(this.stateFile, 'utf8'));
      } else {
        this.state = {
          lastCheck: null,
          lastRefresh: null,
          expiresAt: null,
          refreshCount: 0,
          errors: []
        };
      }
    } catch (err) {
      console.error('Error loading token state:', err.message);
      this.state = {};
    }
  }
  
  saveState() {
    try {
      fs.writeFileSync(this.stateFile, JSON.stringify(this.state, null, 2));
    } catch (err) {
      console.error('Error saving token state:', err.message);
    }
  }
  
  // Check if current token is valid
  async checkTokenValidity() {
    try {
      console.log('🔍 Checking Facebook token validity...');
      
      if (!this.currentToken) {
        console.log('❌ No token found in .env');
        return { valid: false, reason: 'No token configured' };
      }
      
      const response = await axios.get('https://graph.facebook.com/v18.0/debug_token', {
        params: {
          input_token: this.currentToken,
          access_token: `${this.appId}|${this.appSecret}` // App token
        }
      });
      
      const data = response.data.data;
      
      if (!data.is_valid) {
        console.log('❌ Token is invalid');
        return { valid: false, reason: data.error?.message || 'Invalid token' };
      }
      
      // Calculate days until expiry
      const expiresAt = data.expires_at;
      const now = Math.floor(Date.now() / 1000);
      const daysRemaining = expiresAt ? Math.floor((expiresAt - now) / 86400) : null;
      
      console.log(`✅ Token is valid`);
      if (daysRemaining !== null) {
        console.log(`📅 Expires in: ${daysRemaining} days`);
      } else {
        console.log(`📅 Token type: Long-lived (no expiry or very long)`);
      }
      
      this.state.lastCheck = new Date().toISOString();
      this.state.expiresAt = expiresAt ? new Date(expiresAt * 1000).toISOString() : null;
      this.saveState();
      
      return {
        valid: true,
        expiresAt: expiresAt,
        daysRemaining: daysRemaining,
        scopes: data.scopes,
        appId: data.app_id,
        userId: data.user_id
      };
      
    } catch (err) {
      console.error('❌ Token check failed:', err.message);
      
      if (err.response?.data) {
        console.error('Error details:', err.response.data);
      }
      
      return { 
        valid: false, 
        reason: err.message,
        error: err.response?.data
      };
    }
  }
  
  // Exchange short-lived token for long-lived token
  async exchangeToken(shortLivedToken) {
    try {
      console.log('🔄 Exchanging for long-lived token...');
      
      if (!this.appId || !this.appSecret) {
        throw new Error('FACEBOOK_APP_ID or FACEBOOK_APP_SECRET not set in .env');
      }
      
      const response = await axios.get('https://graph.facebook.com/v18.0/oauth/access_token', {
        params: {
          grant_type: 'fb_exchange_token',
          client_id: this.appId,
          client_secret: this.appSecret,
          fb_exchange_token: shortLivedToken
        }
      });
      
      const longLivedUserToken = response.data.access_token;
      const expiresIn = response.data.expires_in; // seconds
      
      console.log(`✅ Long-lived USER token obtained (expires in ${Math.floor(expiresIn / 86400)} days)`);
      
      return longLivedUserToken;
      
    } catch (err) {
      console.error('❌ Token exchange failed:', err.message);
      throw err;
    }
  }
  
  // Get Page Access Token from User Token
  async getPageAccessToken(userToken) {
    try {
      console.log('📄 Getting Page Access Token...');
      
      const response = await axios.get('https://graph.facebook.com/v18.0/me/accounts', {
        params: {
          access_token: userToken
        }
      });
      
      const pages = response.data.data;
      
      if (!pages || pages.length === 0) {
        throw new Error('No pages found for this user');
      }
      
      // Find our page
      let pageData = null;
      if (this.pageId) {
        pageData = pages.find(p => p.id === this.pageId);
      }
      
      if (!pageData) {
        pageData = pages[0]; // Use first page
      }
      
      console.log(`✅ Page Access Token obtained for: ${pageData.name}`);
      console.log(`   Page ID: ${pageData.id}`);
      
      return {
        accessToken: pageData.access_token,
        pageId: pageData.id,
        pageName: pageData.name,
        category: pageData.category
      };
      
    } catch (err) {
      console.error('❌ Failed to get Page Access Token:', err.message);
      throw err;
    }
  }
  
  // Complete refresh workflow
  async refreshToken(shortLivedToken) {
    try {
      console.log('\n🔄 Starting token refresh workflow...\n');
      
      // Step 1: Exchange for long-lived USER token
      const longLivedUserToken = await this.exchangeToken(shortLivedToken);
      
      // Step 2: Get Page Access Token
      const pageData = await this.getPageAccessToken(longLivedUserToken);
      
      // Step 3: Update .env file
      await this.updateEnvFile(pageData.pageId, pageData.accessToken);
      
      // Step 4: Verify new token
      this.currentToken = pageData.accessToken;
      this.pageId = pageData.pageId;
      
      const validity = await this.checkTokenValidity();
      
      if (!validity.valid) {
        throw new Error('New token validation failed');
      }
      
      // Step 5: Update state
      this.state.lastRefresh = new Date().toISOString();
      this.state.refreshCount = (this.state.refreshCount || 0) + 1;
      this.state.expiresAt = validity.expiresAt ? new Date(validity.expiresAt * 1000).toISOString() : null;
      this.saveState();
      
      console.log('\n✅ Token refresh complete!');
      console.log(`📋 Page: ${pageData.pageName}`);
      console.log(`📅 Valid for: ${validity.daysRemaining || 'permanent'} days`);
      console.log(`🔢 Total refreshes: ${this.state.refreshCount}\n`);
      
      return {
        success: true,
        pageData: pageData,
        validity: validity
      };
      
    } catch (err) {
      console.error('❌ Token refresh failed:', err.message);
      
      this.state.errors.push({
        timestamp: new Date().toISOString(),
        error: err.message
      });
      
      // Keep only last 10 errors
      if (this.state.errors.length > 10) {
        this.state.errors = this.state.errors.slice(-10);
      }
      
      this.saveState();
      
      throw err;
    }
  }
  
  // Update .env file with new token
  async updateEnvFile(pageId, accessToken) {
    try {
      const envPath = path.join(__dirname, '../.env');
      
      if (!fs.existsSync(envPath)) {
        throw new Error('.env file not found');
      }
      
      let envContent = fs.readFileSync(envPath, 'utf8');
      
      // Update or add FACEBOOK_PAGE_ID
      if (envContent.includes('FACEBOOK_PAGE_ID=')) {
        envContent = envContent.replace(
          /FACEBOOK_PAGE_ID=.*/,
          `FACEBOOK_PAGE_ID=${pageId}`
        );
      } else {
        envContent += `\nFACEBOOK_PAGE_ID=${pageId}`;
      }
      
      // Update or add FACEBOOK_PAGE_ACCESS_TOKEN
      if (envContent.includes('FACEBOOK_PAGE_ACCESS_TOKEN=')) {
        envContent = envContent.replace(
          /FACEBOOK_PAGE_ACCESS_TOKEN=.*/,
          `FACEBOOK_PAGE_ACCESS_TOKEN=${accessToken}`
        );
      } else {
        envContent += `\nFACEBOOK_PAGE_ACCESS_TOKEN=${accessToken}`;
      }
      
      fs.writeFileSync(envPath, envContent);
      
      console.log('✅ .env file updated');
      
    } catch (err) {
      console.error('❌ Failed to update .env:', err.message);
      throw err;
    }
  }
  
  // Get token status for dashboard/monitoring
  getStatus() {
    return {
      configured: !!this.currentToken,
      lastCheck: this.state.lastCheck,
      lastRefresh: this.state.lastRefresh,
      expiresAt: this.state.expiresAt,
      refreshCount: this.state.refreshCount,
      recentErrors: this.state.errors.slice(-3)
    };
  }
}

module.exports = FacebookTokenManager;
