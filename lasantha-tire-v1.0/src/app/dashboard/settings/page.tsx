'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import { Settings, Database, Key, Facebook, MessageSquare, Image, Store, Globe, Save, RotateCcw, CheckCircle, XCircle, Home, Download, Upload, Eye, EyeOff, Search, Clock, Shield, Lock, History, FileText, Copy, Users, UserPlus, Trash2, Edit } from 'lucide-react';
import Link from 'next/link';
import { format } from 'date-fns';
import ERPConnectionSettings from '@/components/ERPConnectionSettings';

interface SystemConfig {
  [key: string]: string;
}

interface User {
  id: string;
  username: string;
  role: 'admin' | 'user' | 'viewer';
  createdAt: string;
  lastLogin?: string;
  active: boolean;
}

const SETTINGS_CATEGORIES = [
  { id: 'users', name: 'User Management', icon: Users, color: 'cyan', description: 'Manage users, passwords and permissions', fields: [] },
  { id: 'erp-connection', name: 'ERP Connection', icon: Database, color: 'cyan', description: 'Choose between SQL Server or Peachtree connection', fields: [] },
  { id: 'database', name: 'Database Configuration', icon: Database, color: 'blue', description: 'SQL Server connection and security settings', fields: ['SQL_SERVER', 'SQL_USER', 'SQL_PASSWORD', 'SQL_DATABASE', 'SQL_PORT', 'DB_ENCRYPT', 'DB_TRUST_CERT'] },
  { id: 'bot', name: 'Bot & WhatsApp', icon: MessageSquare, color: 'green', description: 'WhatsApp bot configuration', fields: ['BOT_API_PORT', 'ADMIN_NUMBERS', 'ADMIN_WHATSAPP_NUMBER', 'DAILY_REPORT_NUMBERS', 'ALLOWED_NUMBERS', 'SCHEDULER_ENABLED'] },
  { id: 'ai', name: 'AI & API Keys', icon: Key, color: 'purple', description: 'AI and API credentials', fields: ['ENABLE_AI_COPILOT', 'AI_CONTEXT_MESSAGES', 'ANTHROPIC_API_KEY', 'GEMINI_API_KEY', 'FB_POST_AI_PROVIDER'] },
  { id: 'facebook', name: 'Facebook Integration', icon: Facebook, color: 'indigo', description: 'Facebook automation settings', fields: ['FACEBOOK_PAGE_ID', 'FACEBOOK_PAGE_ACCESS_TOKEN', 'FB_PUBLISH_MODE', 'FB_APPROVAL_MODE', 'AUTO_REANALYZE_POST_STYLE', 'ENABLE_AB_DRAFTS', 'AB_VARIANT_COUNT', 'LOCAL_PREVIEW_ONLY'] },
  { id: 'store', name: 'Store Information', icon: Store, color: 'orange', description: 'Business details', fields: ['STORE_NAME', 'STORE_PHONE', 'STORE_LOCATION', 'STORE_HOURS', 'STORE_WHEEL_ALIGNMENT_HOURS', 'AUTH_BADGE_TEXT', 'TAGLINE_SI', 'TAGLINE_EN', 'LOGO_PATH'] },
  { id: 'images', name: 'Image & Posters', icon: Image, color: 'pink', description: 'Image generation', fields: ['POST_IMAGE_MODE', 'EXTERNAL_IMAGE_DIR', 'POSTER_TEMPLATE_DIR', 'FORCE_LAYOUT', 'FORCE_PALETTE', 'FORCE_TEMPLATE_ID'] },
  { id: 'pricing', name: 'Pricing Rules', icon: Globe, color: 'yellow', description: 'Cost calculation', fields: ['COST_ADD_BASE', 'COST_ADD_CREDIT', 'MIN_TYRE_COST', 'QUOTE_DIR'] },
  { id: 'system', name: 'System & Logging', icon: Settings, color: 'gray', description: 'System configuration', fields: ['LOG_LEVEL', 'LOG_MAX_BYTES', 'BOT_PORT_MAX_TRIES'] }
];

const FIELD_DESCRIPTIONS: Record<string, any> = {
  SQL_SERVER: { label: 'SQL Server', description: 'Database server', type: 'text', default: 'localhost', helpText: 'Use localhost for local' },
  SQL_USER: { label: 'SQL User', description: 'Username', type: 'text', default: 'sa' },
  SQL_PASSWORD: { label: 'SQL Password', description: 'Password', type: 'password', default: '', sensitive: true },
  SQL_DATABASE: { label: 'Database', description: 'Database name', type: 'text', default: 'TYREDB' },
  SQL_PORT: { label: 'Port', description: 'Port number', type: 'number', default: '1433' },
  DB_ENCRYPT: { label: 'Encrypt', description: 'Enable SSL', type: 'boolean', default: 'false', helpText: 'Recommended' },
  DB_TRUST_CERT: { label: 'Trust Cert', description: 'Trust certificate', type: 'boolean', default: 'true' },
  BOT_API_PORT: { label: 'Bot Port', description: 'API port', type: 'number', default: '3100' },
  ADMIN_NUMBERS: { label: 'Admins', description: 'Admin numbers', type: 'textarea', default: '0771222509' },
  ADMIN_WHATSAPP_NUMBER: { label: 'Primary Admin', description: 'Main admin', type: 'text', default: '0771222509' },
  DAILY_REPORT_NUMBERS: { label: 'Report To', description: 'Report recipients', type: 'textarea', default: '0771222509' },
  ALLOWED_NUMBERS: { label: 'Allowed', description: 'Whitelist', type: 'textarea', default: '' },
  SCHEDULER_ENABLED: { label: 'Scheduler', description: 'Enable scheduler', type: 'boolean', default: '1' },
  ENABLE_AI_COPILOT: { label: 'AI Copilot', description: 'Enable AI', type: 'boolean', default: 'true' },
  AI_CONTEXT_MESSAGES: { label: 'AI Context', description: 'Context messages', type: 'number', default: '5' },
  ANTHROPIC_API_KEY: { label: 'Claude Key', description: 'Anthropic API', type: 'password', default: '', sensitive: true },
  GEMINI_API_KEY: { label: 'Gemini Key', description: 'Google API', type: 'password', default: '', sensitive: true },
  FB_POST_AI_PROVIDER: { label: 'Provider', description: 'AI provider', type: 'select', default: 'claude', options: ['claude', 'gemini', 'openai'] },
  FACEBOOK_PAGE_ID: { label: 'Page ID', description: 'FB Page ID', type: 'text', default: '' },
  FACEBOOK_PAGE_ACCESS_TOKEN: { label: 'Token', description: 'Access token', type: 'password', default: '', sensitive: true },
  FB_PUBLISH_MODE: { label: 'Mode', description: 'Publish mode', type: 'select', default: 'draft', options: ['draft', 'publish'] },
  FB_APPROVAL_MODE: { label: 'Approval', description: 'How to approve', type: 'select', default: 'whatsapp', options: ['whatsapp', 'auto', 'manual'] },
  AUTO_REANALYZE_POST_STYLE: { label: 'Reanalyze', description: 'Auto reanalyze', type: 'boolean', default: 'false' },
  ENABLE_AB_DRAFTS: { label: 'A/B Testing', description: 'Multiple variants', type: 'boolean', default: 'true' },
  AB_VARIANT_COUNT: { label: 'Variants', description: 'Variant count', type: 'number', default: '2' },
  LOCAL_PREVIEW_ONLY: { label: 'Test Mode', description: 'Preview only', type: 'boolean', default: 'false' },
  STORE_NAME: { label: 'Name', description: 'Store name', type: 'text', default: 'Lasantha Tyre' },
  STORE_PHONE: { label: 'Phone', description: 'Contact', type: 'text', default: '0721222509' },
  STORE_LOCATION: { label: 'Location', description: 'City', type: 'text', default: 'Thalawathugoda' },
  STORE_HOURS: { label: 'Hours', description: 'Business hours', type: 'text', default: '06:30-21:00' },
  STORE_WHEEL_ALIGNMENT_HOURS: { label: 'Alignment', description: 'Service hours', type: 'text', default: '07:30-18:00' },
  AUTH_BADGE_TEXT: { label: 'Badge', description: 'Badge text', type: 'text', default: 'Authorised' },
  TAGLINE_SI: { label: 'Tagline SI', description: 'Sinhala', type: 'text', default: '' },
  TAGLINE_EN: { label: 'Tagline EN', description: 'English', type: 'text', default: '' },
  LOGO_PATH: { label: 'Logo', description: 'Logo path', type: 'text', default: '' },
  POST_IMAGE_MODE: { label: 'Mode', description: 'Image mode', type: 'select', default: 'generate', options: ['generate', 'upload', 'external'] },
  EXTERNAL_IMAGE_DIR: { label: 'Image Dir', description: 'External dir', type: 'text', default: '' },
  POSTER_TEMPLATE_DIR: { label: 'Templates', description: 'Template dir', type: 'text', default: 'C:/Users/Cashi/Music' },
  FORCE_LAYOUT: { label: 'Layout', description: 'Force layout', type: 'text', default: '' },
  FORCE_PALETTE: { label: 'Palette', description: 'Force palette', type: 'text', default: '' },
  FORCE_TEMPLATE_ID: { label: 'Template', description: 'Force template', type: 'text', default: '' },
  COST_ADD_BASE: { label: 'Base Cost', description: 'Base markup', type: 'number', default: '1500' },
  COST_ADD_CREDIT: { label: 'Credit', description: 'Credit markup', type: 'number', default: '500' },
  MIN_TYRE_COST: { label: 'Min Cost', description: 'Floor price', type: 'number', default: '3000' },
  QUOTE_DIR: { label: 'Quote Dir', description: 'PDF location', type: 'text', default: 'C:/QUOTE' },
  LOG_LEVEL: { label: 'Log Level', description: 'Verbosity', type: 'select', default: 'info', options: ['error', 'warn', 'info', 'debug', 'trace'] },
  LOG_MAX_BYTES: { label: 'Log Size', description: 'Max size', type: 'number', default: '2000000' },
  BOT_PORT_MAX_TRIES: { label: 'Port Tries', description: 'Max attempts', type: 'number', default: '10' }
};

export default function AdvancedSettings() {
  const router = useRouter();
  const [config, setConfig] = useState<SystemConfig>({});
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<any>(null);
  const [activeCategory, setActiveCategory] = useState('users');
  const [changedFields, setChangedFields] = useState<Set<string>>(new Set());
  const [searchQuery, setSearchQuery] = useState('');
  const [showSensitive, setShowSensitive] = useState<Record<string, boolean>>({});
  const [lastSaved, setLastSaved] = useState<Date | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [users, setUsers] = useState<User[]>([]);
  const [showUserModal, setShowUserModal] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [newUser, setNewUser] = useState({ username: '', password: '', role: 'user' as 'admin' | 'user' | 'viewer' });

  useEffect(() => {
    const verifyAuth = async () => {
      const auth = await checkAuth();
      if (!auth.authenticated) {
        router.push('/');
        return false;
      }
      setIsAuthenticated(true);
      return true;
    };

    verifyAuth().then((authenticated) => {
      if (authenticated) {
        loadConfig();
        loadUsers();
      }
    });
  }, [router]);

  const loadConfig = async () => {
    try {
      const res = await fetch('/api/settings/config');
      if (res.ok) {
        const data = await res.json();
        setConfig(data.config || {});
        if (data.lastModified) setLastSaved(new Date(data.lastModified));
      }
    } catch (error) {
      showMsg('error', 'Failed to load');
    } finally {
      setLoading(false);
    }
  };

  const loadUsers = async () => {
    try {
      const res = await fetch('/api/settings/users');
      if (res.ok) {
        const data = await res.json();
        setUsers(data.users || []);
      }
    } catch (error) {
      console.error('Failed to load users:', error);
    }
  };

  const createUser = async () => {
    if (!newUser.username || !newUser.password) {
      showMsg('error', 'Username and password required');
      return;
    }
    if (newUser.password.length < 8) {
      showMsg('error', 'Password must be 8+ characters');
      return;
    }

    try {
      const res = await fetch('/api/settings/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action: 'create', user: newUser })
      });
      
      const data = await res.json();
      if (res.ok) {
        showMsg('success', '✅ User created');
        setNewUser({ username: '', password: '', role: 'user' });
        setShowUserModal(false);
        loadUsers();
      } else {
        showMsg('error', data.error || 'Failed to create user');
      }
    } catch (error) {
      showMsg('error', 'Error creating user');
    }
  };

  const updateUser = async (userId: string, updates: Partial<User>) => {
    try {
      const res = await fetch('/api/settings/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action: 'update', user: { id: userId, ...updates } })
      });
      
      const data = await res.json();
      if (res.ok) {
        showMsg('success', '✅ User updated');
        loadUsers();
        setEditingUser(null);
      } else {
        showMsg('error', data.error || 'Failed to update user');
      }
    } catch (error) {
      showMsg('error', 'Error updating user');
    }
  };

  const deleteUser = async (userId: string, username: string) => {
    if (!confirm(`🗑️ Delete user "${username}"?`)) return;

    try {
      const res = await fetch('/api/settings/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ action: 'delete', user: { id: userId } })
      });
      
      const data = await res.json();
      if (res.ok) {
        showMsg('success', '✅ User deleted');
        loadUsers();
      } else {
        showMsg('error', data.error || 'Failed to delete user');
      }
    } catch (error) {
      showMsg('error', 'Error deleting user');
    }
  };

  const saveConfig = async () => {
    setSaving(true);
    try {
      const res = await fetch('/api/settings/config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ config })
      });
      if (res.ok) {
        showMsg('success', ' Saved! Restart bot to apply.');
        setChangedFields(new Set());
        setLastSaved(new Date());
      } else {
        showMsg('error', 'Failed to save');
      }
    } catch (error) {
      showMsg('error', 'Save error');
    } finally {
      setSaving(false);
    }
  };

  const exportConfig = () => {
    const blob = new Blob([JSON.stringify(config, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `settings-${format(new Date(), 'yyyy-MM-dd-HHmm')}.json`;
    link.click();
    showMsg('success', ' Exported');
  };

  const importConfig = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      const text = await file.text();
      const imported = JSON.parse(text);
      setConfig(imported);
      setChangedFields(new Set(Object.keys(imported)));
      showMsg('success', ' Imported. Save to apply.');
    } catch {
      showMsg('error', 'Invalid file');
    }
  };

  const resetDefaults = () => {
    if (!confirm(' Reset to defaults?')) return;
    const defaults: SystemConfig = {};
    Object.entries(FIELD_DESCRIPTIONS).forEach(([key, meta]) => {
      defaults[key] = meta.default || '';
    });
    setConfig(defaults);
    setChangedFields(new Set(Object.keys(defaults)));
    showMsg('warning', 'Reset. Save to apply.');
  };

  const handleChange = (field: string, value: string) => {
    setConfig(prev => ({ ...prev, [field]: value }));
    setChangedFields(prev => new Set(prev).add(field));
  };

  const showMsg = (type: string, text: string) => {
    setMessage({ type, text });
    setTimeout(() => setMessage(null), 5000);
  };

  const maskStr = (str: string) => {
    if (!str || str.length <= 4) return '****';
    return str.substring(0, 4) + '*'.repeat(Math.max(0, str.length - 4));
  };

  const getColor = (color: string) => {
    const colors: Record<string, string> = {
      cyan: 'bg-cyan-500/20', blue: 'bg-blue-500/20', green: 'bg-green-500/20', purple: 'bg-purple-500/20',
      indigo: 'bg-indigo-500/20', orange: 'bg-orange-500/20', pink: 'bg-pink-500/20',
      yellow: 'bg-yellow-500/20', gray: 'bg-slate-500/20'
    };
    return colors[color] || 'bg-slate-500/20';
  };

  const getTextColor = (color: string) => {
    const textColors: Record<string, string> = {
      cyan: 'text-cyan-400', blue: 'text-blue-400', green: 'text-green-400', purple: 'text-purple-400',
      indigo: 'text-indigo-400', orange: 'text-orange-400', pink: 'text-pink-400',
      yellow: 'text-yellow-400', gray: 'text-slate-400'
    };
    return textColors[color] || 'text-slate-400';
  };

  const filterFields = (fields: string[]) => {
    if (!searchQuery) return fields;
    return fields.filter(f => {
      const m = FIELD_DESCRIPTIONS[f];
      return m && (m.label.toLowerCase().includes(searchQuery.toLowerCase()) || f.toLowerCase().includes(searchQuery.toLowerCase()));
    });
  };

  const renderField = (field: string) => {
    const meta = FIELD_DESCRIPTIONS[field];
    if (!meta) return null;
    const value = config[field] || '';
    const changed = changedFields.has(field);
    const sensitive = meta.sensitive;
    const show = !sensitive || showSensitive[field];

    if (meta.type === 'boolean') {
      return (
        <div key={field} className={`p-4 rounded-2xl border ${changed ? 'border-amber-500/30 bg-amber-500/10' : 'border-white/10 bg-slate-800/50 shadow-sm'}`}>
          <div className="flex items-start justify-between gap-4">
            <div className="flex-1">
              <label className="text-sm font-bold text-white flex items-center gap-2">
                {meta.label}
                {sensitive && <Lock className="w-3 h-3 text-amber-500" />}
              </label>
              <p className="text-xs text-slate-400 mt-1">{meta.description}</p>
              {meta.helpText && <p className="text-xs text-blue-400 mt-1 italic"> {meta.helpText}</p>}
            </div>
            <button
              onClick={() => handleChange(field, value === 'true' || value === '1' ? 'false' : 'true')}
              className={`relative inline-flex h-7 w-14 items-center rounded-full shadow-inner transition-colors ${
                value === 'true' || value === '1' ? 'bg-emerald-500' : 'bg-slate-600'
              }`}
            >
              <span className={`inline-block h-5 w-5 rounded-full bg-white shadow-md transition-transform ${
                value === 'true' || value === '1' ? 'translate-x-8' : 'translate-x-1'
              }`} />
            </button>
          </div>
        </div>
      );
    }

    if (meta.type === 'select') {
      return (
        <div key={field} className={`p-4 rounded-2xl border ${changed ? 'border-amber-500/30 bg-amber-500/10' : 'border-white/10 bg-slate-800/50 shadow-sm'}`}>
          <label className="text-sm font-bold text-white block mb-2">{meta.label}</label>
          <p className="text-xs text-slate-400 mb-3">{meta.description}</p>
          <select value={value} onChange={(e) => handleChange(field, e.target.value)}
            className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all">
            {meta.options?.map((opt: string) => <option key={opt} value={opt} className="bg-slate-800 text-white">{opt}</option>)}
          </select>
        </div>
      );
    }

    if (meta.type === 'textarea') {
      return (
        <div key={field} className={`p-4 rounded-2xl border ${changed ? 'border-amber-500/30 bg-amber-500/10' : 'border-white/10 bg-slate-800/50 shadow-sm'}`}>
          <label className="text-sm font-bold text-white block mb-2">{meta.label}</label>
          <p className="text-xs text-slate-400 mb-3">{meta.description}</p>
          <textarea value={value} onChange={(e) => handleChange(field, e.target.value)} rows={3}
            className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 font-mono transition-all placeholder-slate-600"
            placeholder={meta.default} />
        </div>
      );
    }

    return (
      <div key={field} className={`p-4 rounded-2xl border ${changed ? 'border-amber-500/30 bg-amber-500/10' : 'border-white/10 bg-slate-800/50 shadow-sm'}`}>
        <label className="text-sm font-bold text-white block mb-2 flex items-center gap-2">
          {meta.label}
          {sensitive && <Lock className="w-3 h-3 text-amber-500" />}
        </label>
        <p className="text-xs text-slate-400 mb-3">{meta.description}</p>
        <div className="flex gap-2">
          <input
            type={meta.type === 'password' && !show ? 'password' : 'text'}
            value={show ? value : (sensitive ? maskStr(value) : value)}
            onChange={(e) => handleChange(field, e.target.value)}
            readOnly={sensitive && !show}
            className="flex-1 px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-600"
            placeholder={meta.default}
          />
          {sensitive && (
            <button onClick={() => setShowSensitive(p => ({ ...p, [field]: !p[field] }))}
              className="px-3 py-2 bg-slate-700 hover:bg-slate-600 text-slate-300 rounded-xl transition-colors">
              {show ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
            </button>
          )}
          {value && (
            <button onClick={() => { navigator.clipboard.writeText(value); showMsg('success', 'Copied'); }}
              className="px-3 py-2 bg-slate-700 hover:bg-slate-600 text-slate-300 rounded-xl transition-colors">
              <Copy className="w-4 h-4" />
            </button>
          )}
        </div>
        {meta.helpText && <p className="text-xs text-blue-400 mt-2 italic"> {meta.helpText}</p>}
      </div>
    );
  };

  if (!isAuthenticated || loading) {
    return (
      <div className="min-h-screen bg-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">{!isAuthenticated ? 'Redirecting...' : 'Loading Settings...'}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto mb-6">
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 bg-slate-800/50 rounded-2xl shadow-sm flex items-center justify-center border border-white/10 backdrop-blur-xl">
              <Shield className="w-8 h-8 text-blue-500" />
            </div>
            <div>
              <h1 className="text-3xl font-bold text-white mb-1 tracking-tight">
                Advanced Settings
              </h1>
              <p className="text-slate-400 text-sm font-medium">Secure configuration with advanced features</p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            {lastSaved && (
              <div className="flex items-center gap-2 px-4 py-2 bg-slate-800/50 rounded-xl border border-white/10 shadow-sm backdrop-blur-xl">
                <Clock className="w-4 h-4 text-emerald-500" />
                <span className="text-sm text-slate-300 font-medium">Saved: {format(lastSaved, 'MMM d, HH:mm')}</span>
              </div>
            )}
            <Link href="/dashboard" className="flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl border border-white/10 shadow-sm transition-colors text-sm font-medium">
              <Home className="w-4 h-4" />
              Dashboard
            </Link>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mb-6">
          <div className="flex items-center gap-3 bg-slate-800/50 border border-white/10 rounded-[2rem] p-4 shadow-sm backdrop-blur-xl">
            <button onClick={saveConfig} disabled={saving || changedFields.size === 0}
              className="flex items-center gap-2 px-6 py-2.5 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400/50 disabled:cursor-not-allowed text-white rounded-xl font-bold shadow-lg shadow-blue-600/20 transition-all text-sm">
              <Save className="w-4 h-4" />
              {saving ? 'Saving...' : changedFields.size > 0 ? `Save (${changedFields.size})` : 'Save Changes'}
            </button>
            <button onClick={resetDefaults} className="flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-slate-200 rounded-xl font-medium transition-colors text-sm">
              <RotateCcw className="w-4 h-4" />
              Reset
            </button>
          </div>

          <div className="flex items-center gap-3 bg-slate-800/50 border border-white/10 rounded-[2rem] p-4 shadow-sm backdrop-blur-xl">
            <button onClick={exportConfig} className="flex items-center gap-2 px-4 py-2 bg-violet-600 hover:bg-violet-700 text-white rounded-xl font-medium shadow-lg shadow-violet-600/20 transition-all text-sm">
              <Download className="w-4 h-4" />
              Export
            </button>
            <label className="flex items-center gap-2 px-4 py-2 bg-fuchsia-600 hover:bg-fuchsia-700 text-white rounded-xl cursor-pointer font-medium shadow-lg shadow-fuchsia-600/20 transition-all text-sm">
              <Upload className="w-4 h-4" />
              Import
              <input type="file" accept=".json" onChange={importConfig} className="hidden" />
            </label>
            <div className="flex-1 relative">
              <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input type="text" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search settings..." className="w-full pl-10 pr-4 py-2 bg-slate-900/50 border border-white/10 rounded-xl text-white text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-500" />
            </div>
          </div>
        </div>

        {message && (
          <div className={`mb-6 flex items-center gap-3 px-6 py-4 rounded-2xl shadow-sm border ${
            message.type === 'success' ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400' :
            message.type === 'warning' ? 'bg-amber-500/10 border-amber-500/20 text-amber-400' :
            'bg-rose-500/10 border-rose-500/20 text-rose-400'
          }`}>
            {message.type === 'success' && <CheckCircle className="w-6 h-6" />}
            {message.type === 'error' && <XCircle className="w-6 h-6" />}
            <span className="font-bold text-sm">{message.text}</span>
          </div>
        )}
      </div>

      <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-4 gap-6">
        <div className="lg:col-span-1">
          <div className="bg-slate-800/50 border border-white/10 rounded-[2rem] p-6 sticky top-6 shadow-xl backdrop-blur-xl">
            <h3 className="text-lg font-bold text-white mb-4 flex items-center gap-2">
              <FileText className="w-5 h-5 text-blue-500" />
              Categories
            </h3>
            <div className="space-y-2">
              {SETTINGS_CATEGORIES.map((cat) => {
                const Icon = cat.icon;
                const active = activeCategory === cat.id;
                const changed = cat.fields.filter(f => changedFields.has(f)).length;
                return (
                  <button key={cat.id} onClick={() => setActiveCategory(cat.id)}
                    className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${
                      active ? 'bg-slate-700/50 text-white font-bold shadow-sm border border-white/5' : 'text-slate-400 hover:bg-slate-800/50 hover:text-slate-200'
                    }`}>
                    <div className={`w-8 h-8 ${active ? 'bg-slate-600' : 'bg-slate-800'} rounded-lg flex items-center justify-center transition-colors`}>
                      <Icon className={`w-4 h-4 ${active ? 'text-blue-400' : 'text-slate-500'}`} />
                    </div>
                    <div className="flex-1 text-left">
                      <span className="text-sm block">{cat.name}</span>
                      {changed > 0 && <span className="text-[10px] font-bold text-amber-400 bg-amber-500/10 px-2 py-0.5 rounded-full mt-1 inline-block border border-amber-500/20">{changed} changed</span>}
                    </div>
                  </button>
                );
              })}
            </div>
          </div>
        </div>

        <div className="lg:col-span-3">
          <div className="bg-slate-800/50 border border-white/10 rounded-[2rem] p-8 shadow-xl backdrop-blur-xl">
            {SETTINGS_CATEGORIES.filter(c => c.id === activeCategory).map(cat => {
              const Icon = cat.icon;
              const fields = filterFields(cat.fields);
              return (
                <div key={cat.id}>
                  <div className="flex items-center gap-4 mb-8 pb-6 border-b border-white/10">
                    <div className={`w-16 h-16 ${getColor(cat.color)} rounded-2xl flex items-center justify-center border border-white/5`}>
                      <Icon className={`w-8 h-8 ${getTextColor(cat.color)}`} />
                    </div>
                    <div className="flex-1">
                      <h2 className="text-2xl font-bold text-white">{cat.name}</h2>
                      <p className="text-sm text-slate-400 font-medium mt-1">{cat.description}</p>
                    </div>
                  </div>
                  
                  {cat.id === 'users' ? (
                    <div className="space-y-6">
                      <div className="flex items-center justify-between mb-4">
                        <h3 className="text-lg font-bold text-white">Users ({users.length})</h3>
                        <button onClick={() => setShowUserModal(true)}
                          className="flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-xl font-bold shadow-lg shadow-emerald-600/20 transition-all text-sm">
                          <UserPlus className="w-4 h-4" />
                          Add User
                        </button>
                      </div>

                      <div className="grid gap-4">
                        {users.map(user => (
                          <div key={user.id} className="p-6 rounded-2xl border border-white/10 bg-slate-800/50 shadow-sm hover:shadow-md transition-shadow">
                            <div className="flex items-center justify-between">
                              <div className="flex-1">
                                <div className="flex items-center gap-3 mb-2">
                                  <h4 className="text-lg font-bold text-white">{user.username}</h4>
                                  <span className={`px-3 py-1 rounded-full text-xs font-bold ${
                                    user.role === 'admin' ? 'bg-rose-500/10 text-rose-400 border border-rose-500/20' :
                                    user.role === 'user' ? 'bg-blue-500/10 text-blue-400 border border-blue-500/20' :
                                    'bg-slate-500/10 text-slate-400 border border-slate-500/20'
                                  }`}>
                                    {user.role.toUpperCase()}
                                  </span>
                                  {!user.active && (
                                    <span className="px-3 py-1 rounded-full text-xs font-bold bg-amber-500/10 text-amber-400 border border-amber-500/20">
                                      INACTIVE
                                    </span>
                                  )}
                                </div>
                                <p className="text-sm text-slate-400 font-medium">Created: {format(new Date(user.createdAt), 'MMM d, yyyy HH:mm')}</p>
                                {user.lastLogin && (
                                  <p className="text-sm text-slate-500 mt-1">Last login: {format(new Date(user.lastLogin), 'MMM d, yyyy HH:mm')}</p>
                                )}
                              </div>

                              <div className="flex items-center gap-2">
                                <button onClick={() => setEditingUser(user)}
                                  className="px-3 py-2 bg-slate-700 hover:bg-slate-600 text-blue-400 rounded-xl transition-colors">
                                  <Edit className="w-4 h-4" />
                                </button>
                                <button onClick={() => updateUser(user.id, { active: !user.active })}
                                  className={`px-4 py-2 rounded-xl font-bold text-sm transition-colors ${
                                    user.active ? 'bg-amber-500/10 hover:bg-amber-500/20 text-amber-400 border border-amber-500/20' : 'bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 border border-emerald-500/20'
                                  }`}>
                                  {user.active ? 'Disable' : 'Enable'}
                                </button>
                                <button onClick={() => deleteUser(user.id, user.username)}
                                  className="px-3 py-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-xl transition-colors border border-rose-500/20">
                                  <Trash2 className="w-4 h-4" />
                                </button>
                              </div>
                            </div>
                          </div>
                        ))}

                        {users.length === 0 && (
                          <div className="text-center py-12 text-slate-500">
                            <Users className="w-12 h-12 mx-auto mb-3 opacity-50" />
                            <p className="font-medium">No users found</p>
                          </div>
                        )}
                      </div>
                    </div>
                  ) : cat.id === 'erp-connection' ? (
                    <ERPConnectionSettings />
                  ) : fields.length === 0 ? (
                    <div className="text-center py-12 text-slate-500">
                      <Search className="w-12 h-12 mx-auto mb-3 opacity-50" />
                      <p className="font-medium">No matching settings</p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 gap-4">
                      {fields.map(f => renderField(f))}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      </div>

      {/* Add User Modal */}
      {showUserModal && (
        <div className="fixed inset-0 bg-slate-900/80 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-slate-800 rounded-[2rem] p-8 max-w-md w-full shadow-2xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-white flex items-center gap-2">
                <UserPlus className="w-6 h-6 text-emerald-500" />
                Add New User
              </h3>
              <button onClick={() => setShowUserModal(false)} className="text-slate-400 hover:text-white transition-colors">
                <XCircle className="w-6 h-6" />
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="text-sm font-bold text-white block mb-2">Username</label>
                <input type="text" value={newUser.username} onChange={(e) => setNewUser({...newUser, username: e.target.value})}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-600"
                  placeholder="Enter username" />
              </div>

              <div>
                <label className="text-sm font-bold text-white block mb-2">Password (8+ characters)</label>
                <input type="password" value={newUser.password} onChange={(e) => setNewUser({...newUser, password: e.target.value})}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-600"
                  placeholder="Enter password" />
              </div>

              <div>
                <label className="text-sm font-bold text-white block mb-2">Role</label>
                <select value={newUser.role} onChange={(e) => setNewUser({...newUser, role: e.target.value as any})}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all">
                  <option value="viewer" className="bg-slate-800">Viewer (Read Only)</option>
                  <option value="user" className="bg-slate-800">User (Can Edit)</option>
                  <option value="admin" className="bg-slate-800">Admin (Full Access)</option>
                </select>
              </div>

              <div className="flex gap-3 pt-4">
                <button onClick={createUser}
                  className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 bg-emerald-600 hover:bg-emerald-700 text-white rounded-xl font-bold shadow-lg shadow-emerald-600/20 transition-all">
                  <UserPlus className="w-4 h-4" />
                  Create User
                </button>
                <button onClick={() => setShowUserModal(false)}
                  className="px-6 py-2.5 bg-slate-700 hover:bg-slate-600 text-white rounded-xl font-medium transition-colors">
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Edit User Modal */}
      {editingUser && (
        <div className="fixed inset-0 bg-slate-900/80 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-slate-800 rounded-[2rem] p-8 max-w-md w-full shadow-2xl border border-white/10">
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-white flex items-center gap-2">
                <Edit className="w-6 h-6 text-blue-500" />
                Edit User: {editingUser.username}
              </h3>
              <button onClick={() => setEditingUser(null)} className="text-slate-400 hover:text-white transition-colors">
                <XCircle className="w-6 h-6" />
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="text-sm font-bold text-white block mb-2">Username</label>
                <input type="text" value={editingUser.username} 
                  onChange={(e) => setEditingUser({...editingUser, username: e.target.value})}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-600"
                  placeholder="Enter username" />
              </div>

              <div>
                <label className="text-sm font-bold text-white block mb-2">New Password (leave blank to keep current)</label>
                <input type="password" 
                  onChange={(e) => setEditingUser({...editingUser, password: e.target.value} as any)}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all placeholder-slate-600"
                  placeholder="Enter new password" />
              </div>

              <div>
                <label className="text-sm font-bold text-white block mb-2">Role</label>
                <select value={editingUser.role} 
                  onChange={(e) => setEditingUser({...editingUser, role: e.target.value as any})}
                  className="w-full px-4 py-2.5 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all">
                  <option value="viewer" className="bg-slate-800">Viewer (Read Only)</option>
                  <option value="user" className="bg-slate-800">User (Can Edit)</option>
                  <option value="admin" className="bg-slate-800">Admin (Full Access)</option>
                </select>
              </div>

              <div className="flex gap-3 pt-4">
                <button onClick={() => {
                  const updates: any = { 
                    username: editingUser.username, 
                    role: editingUser.role 
                  };
                  if ((editingUser as any).password) {
                    updates.password = (editingUser as any).password;
                  }
                  updateUser(editingUser.id, updates);
                }}
                  className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold shadow-lg shadow-blue-600/20 transition-all">
                  <Save className="w-4 h-4" />
                  Save Changes
                </button>
                <button onClick={() => setEditingUser(null)}
                  className="px-6 py-2.5 bg-slate-700 hover:bg-slate-600 text-white rounded-xl font-medium transition-colors">
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
