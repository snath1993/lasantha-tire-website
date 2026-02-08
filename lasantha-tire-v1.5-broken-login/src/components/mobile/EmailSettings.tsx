'use client';

import { useState, useEffect } from 'react';
import { Mail, Save, Eye, EyeOff, CheckCircle, XCircle, Loader2, RefreshCw } from 'lucide-react';

const BOT_API_URL = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';

interface EmailConfig {
  provider: 'gmail' | 'zoho';
  user: string;
  password: string;
  fromName: string;
  isConfigured: boolean;
}

export default function EmailSettings() {
  const [config, setConfig] = useState<EmailConfig>({
    provider: 'gmail',
    user: '',
    password: '',
    fromName: 'Lasantha Tire Service',
    isConfigured: false
  });

  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [testing, setTesting] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  useEffect(() => {
    fetchConfig();
  }, []);

  const fetchConfig = async () => {
    try {
      const response = await fetch(`${BOT_API_URL}/api/config/email`);
      const data = await response.json();
      
      if (data.ok) {
        setConfig({
          provider: data.config.provider || 'gmail',
          user: data.config.user || '',
          password: '', // Never show password
          fromName: data.config.fromName || 'Lasantha Tire Service',
          isConfigured: data.config.isConfigured || false
        });
      }
    } catch (error) {
      console.error('Failed to fetch email config:', error);
    }
  };

  const handleSave = async () => {
    if (!config.user || !config.password) {
      setMessage({ type: 'error', text: 'Email and password are required' });
      return;
    }

    setLoading(true);
    setMessage(null);

    try {
      const response = await fetch(`${BOT_API_URL}/api/config/email`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          provider: config.provider,
          user: config.user,
          password: config.password,
          fromName: config.fromName
        })
      });

      const data = await response.json();

      if (data.ok) {
        setMessage({ type: 'success', text: 'Email configuration saved successfully!' });
        setConfig(prev => ({ ...prev, isConfigured: true }));
      } else {
        setMessage({ type: 'error', text: data.error || 'Failed to save configuration' });
      }
    } catch (error) {
      setMessage({ type: 'error', text: 'Connection error. Please try again.' });
    } finally {
      setLoading(false);
    }
  };

  const handleTest = async () => {
    if (!config.user) {
      setMessage({ type: 'error', text: 'Please configure email first' });
      return;
    }

    setTesting(true);
    setMessage(null);

    try {
      const response = await fetch(`${BOT_API_URL}/api/config/email/test`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          testEmail: config.user
        })
      });

      const data = await response.json();

      if (data.ok) {
        setMessage({ type: 'success', text: 'Test email sent successfully! Check your inbox.' });
      } else {
        setMessage({ type: 'error', text: data.error || 'Failed to send test email' });
      }
    } catch (error) {
      setMessage({ type: 'error', text: 'Connection error. Please try again.' });
    } finally {
      setTesting(false);
    }
  };

  return (
    <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="p-3 bg-blue-500/20 rounded-xl">
            <Mail className="w-6 h-6 text-blue-400" />
          </div>
          <div>
            <h3 className="text-lg font-bold text-white">Email Configuration</h3>
            <p className="text-sm text-slate-400">Setup email service for quotations</p>
          </div>
        </div>
        {config.isConfigured && (
          <div className="flex items-center gap-2 px-3 py-1 bg-green-500/20 text-green-400 rounded-full text-sm">
            <CheckCircle className="w-4 h-4" />
            <span>Configured</span>
          </div>
        )}
      </div>

      {/* Provider Selection */}
      <div className="mb-4">
        <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
          Email Provider
        </label>
        <div className="grid grid-cols-2 gap-3">
          <button
            onClick={() => setConfig({ ...config, provider: 'gmail' })}
            className={`py-3 px-4 rounded-xl font-semibold transition-all ${
              config.provider === 'gmail'
                ? 'bg-blue-600 text-white'
                : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
            }`}
          >
            Gmail
          </button>
          <button
            onClick={() => setConfig({ ...config, provider: 'zoho' })}
            className={`py-3 px-4 rounded-xl font-semibold transition-all ${
              config.provider === 'zoho'
                ? 'bg-blue-600 text-white'
                : 'bg-slate-700/50 text-slate-400 hover:bg-slate-700'
            }`}
          >
            Zoho Mail
          </button>
        </div>
      </div>

      {/* Setup Instructions */}
      <div className="mb-4 p-4 bg-blue-500/10 border border-blue-500/30 rounded-xl">
        <p className="text-sm text-blue-300 font-semibold mb-2">
          {config.provider === 'gmail' ? '📧 Gmail Setup:' : '📧 Zoho Mail Setup:'}
        </p>
        <ol className="text-xs text-slate-300 space-y-1 list-decimal list-inside">
          {config.provider === 'gmail' ? (
            <>
              <li>Go to Google Account Settings → Security</li>
              <li>Enable 2-Step Verification</li>
              <li>Generate App Password (Select "Mail" and "Other")</li>
              <li>Copy the 16-character password</li>
              <li>Enter your Gmail and the App Password below</li>
            </>
          ) : (
            <>
              <li>Login to Zoho Mail account</li>
              <li>Go to Settings → Security → App Passwords</li>
              <li>Click "Generate New Password"</li>
              <li>Give it a name (e.g., "Lasantha Tire Bot")</li>
              <li>Copy the generated password</li>
              <li>Enter your Zoho email and the App Password below</li>
            </>
          )}
        </ol>
      </div>

      {/* Email Input */}
      <div className="mb-4">
        <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
          Email Address
        </label>
        <input
          type="email"
          value={config.user}
          onChange={(e) => setConfig({ ...config, user: e.target.value })}
          placeholder={config.provider === 'gmail' ? 'your-email@gmail.com' : 'your-email@zohomail.com'}
          className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 px-4 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none"
        />
      </div>

      {/* Password Input */}
      <div className="mb-4">
        <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
          App Password
        </label>
        <div className="relative">
          <input
            type={showPassword ? 'text' : 'password'}
            value={config.password}
            onChange={(e) => setConfig({ ...config, password: e.target.value })}
            placeholder="Enter app password (16 characters)"
            className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 px-4 pr-12 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none font-mono"
          />
          <button
            onClick={() => setShowPassword(!showPassword)}
            className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 hover:text-white transition-all"
          >
            {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
          </button>
        </div>
      </div>

      {/* From Name Input */}
      <div className="mb-6">
        <label className="text-xs text-slate-400 uppercase font-semibold mb-2 block">
          From Name (Optional)
        </label>
        <input
          type="text"
          value={config.fromName}
          onChange={(e) => setConfig({ ...config, fromName: e.target.value })}
          placeholder="Lasantha Tire Service"
          className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 px-4 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none"
        />
      </div>

      {/* Message Display */}
      {message && (
        <div className={`mb-4 p-3 rounded-xl border ${
          message.type === 'success'
            ? 'bg-green-500/10 border-green-500/30 text-green-400'
            : 'bg-red-500/10 border-red-500/30 text-red-400'
        }`}>
          <div className="flex items-center gap-2">
            {message.type === 'success' ? (
              <CheckCircle className="w-5 h-5" />
            ) : (
              <XCircle className="w-5 h-5" />
            )}
            <span className="text-sm">{message.text}</span>
          </div>
        </div>
      )}

      {/* Action Buttons */}
      <div className="flex gap-3">
        <button
          onClick={handleSave}
          disabled={loading || !config.user || !config.password}
          className="flex-1 py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-700 text-white rounded-xl font-bold flex items-center justify-center gap-2 transition-all active:scale-95 disabled:scale-100"
        >
          {loading ? (
            <>
              <Loader2 className="w-5 h-5 animate-spin" />
              Saving...
            </>
          ) : (
            <>
              <Save className="w-5 h-5" />
              Save Configuration
            </>
          )}
        </button>

        <button
          onClick={handleTest}
          disabled={testing || !config.isConfigured}
          className="px-6 py-3 bg-green-600 hover:bg-green-700 disabled:bg-slate-700 text-white rounded-xl font-bold flex items-center gap-2 transition-all active:scale-95 disabled:scale-100"
        >
          {testing ? (
            <>
              <Loader2 className="w-5 h-5 animate-spin" />
              Testing...
            </>
          ) : (
            <>
              <RefreshCw className="w-5 h-5" />
              Test
            </>
          )}
        </button>
      </div>

      {/* Security Notice */}
      <div className="mt-4 p-3 bg-slate-700/30 rounded-lg">
        <p className="text-xs text-slate-400">
          🔒 Your credentials are securely stored in environment variables and never exposed to the client.
        </p>
      </div>
    </div>
  );
}
