'use client';

import { useState, useEffect, Suspense } from 'react';
import { createPortal } from 'react-dom';
import { useRouter, useSearchParams } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import Link from 'next/link';
import { 
  Play, 
  Pause, 
  RefreshCw, 
  Settings, 
  Save, 
  X, 
  RotateCcw,
  Clock,
  Users,
  FileText,
  CheckCircle,
  AlertCircle,
  XCircle,
  List,
  Plus,
  Trash2,
  Edit2,
  Search,
  Filter
} from 'lucide-react';

// Force dynamic rendering to avoid prerendering issues with useSearchParams
export const dynamic = 'force-dynamic';

// Job configuration interface matching backend structure
interface JobConfig {
  name: string;
  description: string;
  schedule: string;
  fullDayReportTime: string | null;
  contactNumbers: string[];
  enabled: boolean;
  type: string;
  icon: {
    path: string;
    color: string;
  };
  updatedAt: string;
  allowEveryone: boolean;
  settings: Record<string, any> | null;
}

interface AllJobsConfig {
  [key: string]: JobConfig;
}

// Job metadata with default configurations
const JOB_DEFAULTS: Record<string, Partial<JobConfig>> = {
  // ============ AUTO-REPLY JOBS ============
  TyrePriceReplyJob: {
    name: 'Tyre Price Reply',
    description: 'Auto-reply with tyre prices to customer queries',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: true,
    contactNumbers: [],
    settings: {
      baseRoundStep: 100,
      publicBaseMarkup: 1500,
      publicExtraBuffer: 500,
      allowedDefaultMarkup: 500,
      motorbikeExtra: 500,
      maxxisPublicMarkup: 2500,
      minimumUnitCost: 3000
    }
  },
  CostPriceReplyJob: {
    name: 'Cost Price Reply',
    description: 'Reply with cost prices (authorized contacts only)',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      allowedCostContacts: ['0777078700', '0777311770', '0771222509']
    }
  },
  TyreQtyReplyJob: {
    name: 'Tyre Qty Reply',
    description: 'Reply with available stock quantities',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: true,
    contactNumbers: [],
    settings: {}
  },
  TyrePhotoReplyJob: {
    name: 'Tyre Photo Reply',
    description: 'Send tyre product images to customers',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: true,
    contactNumbers: [],
    settings: {
      imageDirectory: 'images/tyres',
      supportedFormats: ['jpg', 'jpeg', 'png', 'webp']
    }
  },
  VehicleDetailsReplyJob: {
    name: 'Vehicle Details Reply',
    description: 'Reply with vehicle invoice history',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: false,
    contactNumbers: [],
    settings: {}
  },
  VehicleInvoiceReplyJob: {
    name: 'Vehicle Invoice Reply',
    description: 'Reply with detailed vehicle invoice data',
    schedule: 'Real-time',
    type: 'auto-reply',
    allowEveryone: false,
    contactNumbers: [],
    settings: {}
  },

  // ============ PDF GENERATION JOBS ============
  TyreQuotationPDFJob: {
    name: 'Tyre Quotation PDF (Basic)',
    description: 'Generate quotation PDFs (basic version)',
    schedule: 'Real-time',
    type: 'pdf-generation',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      baseRoundStep: 50,
      allowedDefaultMarkup: 1500,
      minimumUnitCost: 3000
    }
  },
  TyreQuotationPDFLibJob: {
    name: 'Tyre Quotation PDF (Advanced)',
    description: 'Generate professional quotation PDFs with branding',
    schedule: 'Real-time',
    type: 'pdf-generation',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      baseRoundStep: 50,
      allowedDefaultMarkup: 1500,
      creditMarkup: 500,
      minimumUnitCost: 3000,
      alignmentRoundStep: 50,
      balancingPrice: 600
    }
  },
  TyreQuotationPDFLibJobPublic: {
    name: 'Tyre Quotation PDF (Public)',
    description: 'Generate quotation PDFs for public customers',
    schedule: 'Real-time',
    type: 'pdf-generation',
    allowEveryone: true,
    contactNumbers: [],
    settings: {
      baseRoundStep: 50,
      publicBaseMarkup: 1500,
      publicExtraBuffer: 500,
      maxxisPublicMarkup: 500,
      minimumUnitCost: 3000,
      alignmentRoundStep: 50,
      balancingPrice: 600
    }
  },

  // ============ SALES REPORTING JOBS ============
  DailyTyreSalesReportJob: {
    name: 'Daily Sales Report',
    description: 'Incremental sales reports every 2 hours + full summary',
    schedule: 'Every 2 hours',
    fullDayReportTime: '23:37',
    type: 'reporting',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      includeProfit: true,
      useSQLite: true,
      reportIncludeProfit: true
    }
  },
  WeeklyTyreSalesReportJob: {
    name: 'Weekly Sales Report',
    description: 'Weekly sales summary report every Sunday',
    schedule: 'Sunday 8:00 AM',
    type: 'reporting',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      includeProfit: true,
      includeBrandBreakdown: true
    }
  },
  MonthlyTyreSalesReportJob: {
    name: 'Monthly Sales Report',
    description: 'Monthly sales summary on 1st of each month',
    schedule: '1st of month, 8:00 AM',
    type: 'reporting',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      includeProfit: true,
      includeCategoryBreakdown: true
    }
  },
  MonthlySalesPDFReportJob: {
    name: 'Monthly Sales PDF Report',
    description: 'Generate professional PDF monthly sales reports',
    schedule: '1st of month, 9:00 AM',
    type: 'reporting',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      includeCharts: true,
      includeCategoryBreakdown: true,
      includeProfitAnalysis: true
    }
  },

  // ============ MONITORING JOBS ============
  WatchedItemRealtimeJob: {
    name: 'Watched Item Monitor',
    description: 'Real-time alerts for specific tyre pattern sales',
    schedule: 'Every 60 seconds',
    type: 'monitoring',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      watchIntervalSeconds: 60,
      patterns: ['MICHELIN', 'YOKOHAMA', 'DURATURN'],
      minQty: 1
    }
  },

  // ============ SYSTEM/BACKGROUND JOBS ============
  DeletionQueueProcessorJob: {
    name: 'Deletion Queue Processor',
    description: 'Process pending invoice deletions from queue',
    schedule: 'Every 1 minute',
    type: 'system',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770', '0771222509'],
    settings: {
      processingIntervalSeconds: 60,
      maxRetries: 3,
      notifyOnFailure: true,
      notifyAdmins: ['0777078700', '0777311770', '0771222509']
    }
  },
  InvoiceDeletionJob: {
    name: 'Invoice Deletion Handler',
    description: 'Delete invoice rows and create void placeholders with backup',
    schedule: 'Triggered by Queue',
    type: 'system',
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770', '0771222509'],
    settings: {
      deletionDelaySeconds: 120,
      backupEnabled: true,
      createVoidPlaceholder: true,
      preserveAllColumns: true,
      notifyOnCompletion: true
    }
  },

  // ============ FACEBOOK AUTOMATION JOBS ============
  DailyFacebookPostJob: {
    name: 'Facebook Daily Post',
    description: 'AI-generated daily Facebook posts (Claude/Gemini) or fallback captions',
    schedule: '8:30 AM daily',
    type: 'social-media',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      useAI: true,
      aiProvider: 'gemini',
      useFallbackOnly: false,
      approvalMode: 'auto',
      publishMode: 'draft',
      localPreviewOnly: false
    }
  },
  FacebookCommentResponderJob: {
    name: 'Facebook Comment Responder',
    description: 'AI-powered smart replies to Facebook comments',
    schedule: 'Every 45 seconds',
    type: 'social-media',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      scanIntervalSeconds: 45,
      replyLanguage: 'si',
      dmWhatsAppFallback: true,
      enablePhoneExtraction: true
    }
  },
  FacebookCommentMonitorJob: {
    name: 'Facebook Comment Monitor (Deprecated)',
    description: 'Monitor Facebook comments (old version - use Responder instead)',
    schedule: 'Every 30 minutes',
    type: 'social-media',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      scanIntervalMinutes: 30,
      autoReplyEnabled: false
    }
  },
  FacebookMessengerResponderJob: {
    name: 'Facebook Messenger Responder',
    description: 'AI-powered replies to Facebook Messenger conversations',
    schedule: 'Every 60 seconds',
    type: 'social-media',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      scanIntervalSeconds: 60,
      replyLanguage: 'si',
      enablePriceQuotes: true
    }
  },
  TokenRefreshJob: {
    name: 'Facebook Token Refresh',
    description: 'Monitor and refresh Facebook access tokens',
    schedule: 'Every 6 hours',
    type: 'maintenance',
    allowEveryone: false,
    contactNumbers: [],
    settings: {
      warningThresholdDays: 7,
      criticalThresholdDays: 3,
      autoRefreshEnabled: false
    }
  },
  
  // ============ SCHEDULED MEDIA PUBLISHER ============
  ScheduledMediaPublisherJob: {
    name: 'Scheduled Media Publisher',
    description: 'Daily 5:15 PM auto-post with WhatsApp reports & history tracking',
    schedule: '5:15 PM daily (17:15)',
    type: 'social-media',
    allowEveryone: false,
    contactNumbers: ['0771222509'],
    settings: {
      enabled: true,
      publishTime: '17:15',
      whatsappLink: 'https://wa.me/94771222509',
      adminWhatsAppNumber: '94771222509',
      publishMode: 'draft',
      postFolder: 'C:/whatsapp-sql-api/post',
      captionsFolder: 'C:/whatsapp-sql-api/post/captions',
      processedFolder: 'C:/whatsapp-sql-api/post/processed',
      historyFile: 'publish-history.json',
      addWhatsAppLink: true,
      trackHistory: true,
      preventDuplicates: true,
      sendAdminReports: true
    }
  }
};

function JobsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const category = searchParams.get('category'); // 'whatsapp' or 'facebook'
  const [mounted, setMounted] = useState(false);
  const [isChecking, setIsChecking] = useState(true);
  const [jobs, setJobs] = useState<AllJobsConfig>({});
  const [loading, setLoading] = useState(true);
  const [editingJob, setEditingJob] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<JobConfig | null>(null);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
  const [showItemList, setShowItemList] = useState(false);
  const [watchedItems, setWatchedItems] = useState<string[]>([]);
  const [editingItemIndex, setEditingItemIndex] = useState<number | null>(null);
  const [newItemName, setNewItemName] = useState('');

  // Filter jobs based on category
  const getFilteredJobs = () => {
    if (!category) return jobs; // Show all if no category
    
    const filtered: AllJobsConfig = {};
    Object.entries(jobs).forEach(([jobId, job]) => {
      const jobName = job.name.toLowerCase();
      const jobIdLower = jobId.toLowerCase();
      
      if (category === 'facebook') {
        // Facebook jobs: DailyFacebookPostJob, FacebookCommentMonitorJob, FacebookCommentResponderJob, FacebookMessengerResponderJob, FacebookTokenRefreshJob
        if (jobIdLower.includes('facebook') || jobName.includes('facebook')) {
          filtered[jobId] = job;
        }
      } else if (category === 'whatsapp') {
        // WhatsApp jobs: Everything except Facebook jobs
        if (!jobIdLower.includes('facebook') && !jobName.includes('facebook')) {
          filtered[jobId] = job;
        }
      }
    });
    return filtered;
  };

  const filteredJobs = getFilteredJobs();
  const jobEntries = Object.entries(filteredJobs);

  useEffect(() => {
    setMounted(true);
  }, []);

  // Authentication check
  useEffect(() => {
    const verifyAuth = async () => {
      const auth = await checkAuth();
      if (!auth.authenticated) {
        router.push('/');
        return;
      }
      setIsChecking(false);
    };
    verifyAuth();
  }, [router]);

  // Load jobs configuration
  useEffect(() => {
    if (!isChecking) {
      loadJobs();
    }
  }, [isChecking]);

  const loadJobs = async () => {
    try {
      const response = await fetch('/api/jobs/config');
      const data = await response.json();
      setJobs(data);
    } catch (error) {
      console.error('Failed to load jobs:', error);
      showMessage('error', 'Failed to load jobs configuration');
    } finally {
      setLoading(false);
    }
  };

  const showMessage = (type: 'success' | 'error', text: string) => {
    setMessage({ type, text });
    setTimeout(() => setMessage(null), 5000);
  };

  const handleEditJob = (jobId: string) => {
    const job = jobs[jobId];
    if (!job) return;
    
    setEditingJob(jobId);
    setEditForm({ ...job });
  };

  const handleCancelEdit = () => {
    setEditingJob(null);
    setEditForm(null);
  };

  const handleSaveJob = async () => {
    if (!editingJob || !editForm) return;

    setSaving(true);
    try {
      const response = await fetch('/api/jobs/config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          jobId: editingJob,
          config: editForm
        })
      });

      if (!response.ok) throw new Error('Failed to save');

      const updatedJobs = await response.json();
      setJobs(updatedJobs);
      setEditingJob(null);
      setEditForm(null);
      showMessage('success', 'Configuration saved successfully!');
    } catch (error) {
      console.error('Failed to save job:', error);
      showMessage('error', 'Failed to save configuration');
    } finally {
      setSaving(false);
    }
  };

  const handleResetJob = (jobId: string) => {
    const defaults = JOB_DEFAULTS[jobId];
    if (!defaults || !editForm) return;

    setEditForm({
      ...editForm,
      ...defaults,
      icon: editForm.icon, // Preserve icon
      updatedAt: new Date().toISOString()
    });
    showMessage('success', 'Reset to default values');
  };

  const handleOpenItemList = async (jobId: string) => {
    if (jobId !== 'WatchedItemRealtimeJob') return;
    
    try {
      const response = await fetch('/api/watched-items');
      const data = await response.json();
      setWatchedItems(data.patterns || []);
      setShowItemList(true);
    } catch (error) {
      console.error('Failed to load watched items:', error);
      showMessage('error', 'Failed to load item list');
    }
  };

  const handleAddItem = () => {
    if (!newItemName.trim()) return;
    
    const updated = [...watchedItems, newItemName.trim()];
    setWatchedItems(updated);
    setNewItemName('');
    saveWatchedItems(updated);
  };

  const handleEditItem = (index: number) => {
    setEditingItemIndex(index);
    setNewItemName(watchedItems[index]);
  };

  const handleUpdateItem = () => {
    if (editingItemIndex === null || !newItemName.trim()) return;
    
    const updated = [...watchedItems];
    updated[editingItemIndex] = newItemName.trim();
    setWatchedItems(updated);
    setEditingItemIndex(null);
    setNewItemName('');
    saveWatchedItems(updated);
  };

  const handleRemoveItem = (index: number) => {
    const updated = watchedItems.filter((_, i) => i !== index);
    setWatchedItems(updated);
    saveWatchedItems(updated);
  };

  const saveWatchedItems = async (patterns: string[]) => {
    try {
      const response = await fetch('/api/watched-items', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ patterns })
      });

      if (!response.ok) throw new Error('Failed to save');
      
      showMessage('success', 'Item list saved successfully!');
    } catch (error) {
      console.error('Failed to save watched items:', error);
      showMessage('error', 'Failed to save item list');
    }
  };

  const handleCancelItemEdit = () => {
    setEditingItemIndex(null);
    setNewItemName('');
  };

  const handleCloseItemList = () => {
    setShowItemList(false);
    setEditingItemIndex(null);
    setNewItemName('');
  };

  const handleToggleEnabled = async (jobId: string) => {
    const job = jobs[jobId];
    if (!job) return;

    try {
      const response = await fetch('/api/jobs/config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          jobId,
          config: { ...job, enabled: !job.enabled }
        })
      });

      if (!response.ok) throw new Error('Failed to toggle');

      const updatedJobs = await response.json();
      setJobs(updatedJobs);
      showMessage('success', job.enabled ? 'Job paused' : 'Job enabled');
    } catch (error) {
      console.error('Failed to toggle job:', error);
      showMessage('error', 'Failed to toggle job');
    }
  };

  const updateFormField = (field: string, value: any) => {
    if (!editForm) return;
    setEditForm({ ...editForm, [field]: value });
  };

  const updateSettingsField = (field: string, value: any) => {
    if (!editForm) return;
    setEditForm({
      ...editForm,
      settings: { ...editForm.settings, [field]: value }
    });
  };

  const addContactNumber = () => {
    if (!editForm) return;
    setEditForm({
      ...editForm,
      contactNumbers: [...editForm.contactNumbers, '']
    });
  };

  const updateContactNumber = (index: number, value: string) => {
    if (!editForm) return;
    const updated = [...editForm.contactNumbers];
    updated[index] = value;
    setEditForm({ ...editForm, contactNumbers: updated });
  };

  const removeContactNumber = (index: number) => {
    if (!editForm) return;
    setEditForm({
      ...editForm,
      contactNumbers: editForm.contactNumbers.filter((_, i) => i !== index)
    });
  };

  const formatSchedule = (job: JobConfig): string => {
    if (job.schedule === 'Real-time' || !job.schedule) return 'Real-time';
    
    // Human-readable cron translations
    const translations: Record<string, string> = {
      '0 */2 * * *': 'Every 2 hours',
      '*/5 * * * *': 'Every 5 minutes',
      '*/10 * * * *': 'Every 10 minutes',
      '*/15 * * * *': 'Every 15 minutes',
      '30 8 * * *': '8:30 AM daily',
      '0 */6 * * *': 'Every 6 hours',
      '0 8 1 * *': '8:00 AM on 1st of month',
      '0 9 1 * *': '9:00 AM on 1st of month',
      '0 0 * * 0': '12:00 AM on Sunday'
    };

    return translations[job.schedule] || job.schedule;
  };

  const getJobTypeColor = (type: string): string => {
    const colors: Record<string, string> = {
      'auto-reply': 'bg-blue-50 text-blue-600',
      'reporting': 'bg-emerald-50 text-emerald-600',
      'monitoring': 'bg-amber-50 text-amber-600',
      'social-media': 'bg-violet-50 text-violet-600',
      'maintenance': 'bg-zinc-100 text-zinc-600',
      'pdf-generation': 'bg-pink-50 text-pink-600',
      'system': 'bg-rose-50 text-rose-600',
      'unknown': 'bg-zinc-100 text-zinc-500'
    };
    return colors[type] || colors.unknown;
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading Jobs...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-white tracking-tight">
            {category === 'facebook' && 'Facebook Jobs'}
            {category === 'whatsapp' && 'WhatsApp Jobs'}
            {!category && 'Job Management'}
          </h1>
          <p className="text-slate-400 mt-1">
            {category === 'facebook' && `Manage ${jobEntries.length} Facebook automation tasks`}
            {category === 'whatsapp' && `Manage ${jobEntries.length} WhatsApp automation tasks`}
            {!category && `Configure and monitor ${Object.keys(jobs).length} active automation jobs`}
          </p>
        </div>
        <div className="flex items-center gap-3">
          <Link
            href="/dashboard"
            className="px-4 py-2 bg-slate-800/50 hover:bg-slate-700/50 text-white rounded-xl transition-colors border border-white/10 text-sm font-medium shadow-sm backdrop-blur-sm"
          >
            Back to Home
          </Link>
        </div>
      </div>

      {/* Category Filter Tabs */}
      {!editingJob && (
        <div className="flex gap-3 p-1 bg-slate-800/50 rounded-2xl w-fit border border-white/10 backdrop-blur-sm">
          <Link
            href="/dashboard/jobs"
            className={`px-6 py-2.5 rounded-xl text-sm font-medium transition-all flex items-center gap-2 ${
              !category 
                ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20' 
                : 'text-slate-400 hover:text-white hover:bg-white/5'
            }`}
          >
            <List size={16} />
            All Jobs
          </Link>
          <Link
            href="/dashboard/jobs?category=whatsapp"
            className={`px-6 py-2.5 rounded-xl text-sm font-medium transition-all flex items-center gap-2 ${
              category === 'whatsapp'
                ? 'bg-emerald-600 text-white shadow-lg shadow-emerald-600/20' 
                : 'text-slate-400 hover:text-emerald-400 hover:bg-white/5'
            }`}
          >
            <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
              <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.890-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z"/>
            </svg>
            WhatsApp
          </Link>
          <Link
            href="/dashboard/jobs?category=facebook"
            className={`px-6 py-2.5 rounded-xl text-sm font-medium transition-all flex items-center gap-2 ${
              category === 'facebook'
                ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20' 
                : 'text-slate-400 hover:text-blue-400 hover:bg-white/5'
            }`}
          >
            <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
              <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
            </svg>
            Facebook
          </Link>
        </div>
      )}

      {/* Success/Error Message */}
      {message && (
        <div className={`flex items-center gap-3 px-4 py-3 rounded-xl border ${
          message.type === 'success' 
            ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400' 
            : 'bg-rose-500/10 border-rose-500/20 text-rose-400'
        }`}>
          {message.type === 'success' ? <CheckCircle size={18} /> : <XCircle size={18} />}
          <span className="text-sm font-medium">{message.text}</span>
        </div>
      )}

      {/* Job Cards Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {jobEntries.length === 0 ? (
          <div className="col-span-full text-center py-12 bg-slate-800/50 backdrop-blur-xl rounded-[2rem] border border-white/10 shadow-sm">
            <div className="w-16 h-16 bg-slate-700/50 rounded-full flex items-center justify-center mx-auto mb-4">
              <Search className="w-8 h-8 text-slate-400" />
            </div>
            <p className="text-slate-400 text-lg font-medium">No jobs found in this category</p>
            <p className="text-slate-500 text-sm mt-1">Try selecting a different category or check back later.</p>
          </div>
        ) : (
          jobEntries.map(([jobId, job]) => (
          <div
            key={jobId}
            className="bg-slate-800/50 backdrop-blur-xl rounded-[2rem] p-6 shadow-lg shadow-black/20 border border-white/10 hover:border-blue-500/30 hover:shadow-blue-500/10 transition-all group"
          >
            {/* Job Header */}
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <div 
                  className="w-12 h-12 rounded-2xl flex items-center justify-center shadow-sm"
                  style={{ backgroundColor: job.icon?.color + '15', color: job.icon?.color }}
                >
                  <svg
                    viewBox="0 0 24 24"
                    className="w-6 h-6 fill-current"
                  >
                    <path d={job.icon?.path} />
                  </svg>
                </div>
                <div>
                  <h3 className="text-lg font-bold text-white leading-tight">{job.name}</h3>
                  <span className={`inline-block px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider mt-1 ${getJobTypeColor(job.type)}`}>
                    {job.type}
                  </span>
                </div>
              </div>
              
              {/* Enable/Disable Toggle */}
              <button
                onClick={() => handleToggleEnabled(jobId)}
                className={`p-2 rounded-xl transition-colors ${
                  job.enabled
                    ? 'bg-emerald-500/10 text-emerald-400 hover:bg-emerald-500/20'
                    : 'bg-slate-700/50 text-slate-400 hover:bg-slate-600/50'
                }`}
                title={job.enabled ? 'Disable job' : 'Enable job'}
              >
                {job.enabled ? <CheckCircle className="w-5 h-5" /> : <AlertCircle className="w-5 h-5" />}
              </button>
            </div>

            {/* Job Description */}
            <p className="text-slate-400 text-sm mb-6 line-clamp-2 h-10">
              {job.description || 'No description available'}
            </p>

            {/* Job Info */}
            <div className="space-y-3 mb-6">
              <div className="flex items-center gap-3 text-sm p-2 bg-slate-900/50 rounded-xl border border-white/5">
                <Clock className="w-4 h-4 text-blue-400" />
                <span className="text-slate-300 font-medium">{formatSchedule(job)}</span>
              </div>
              
              {job.contactNumbers && job.contactNumbers.length > 0 && (
                <div className="flex items-center gap-3 text-sm p-2 bg-slate-900/50 rounded-xl border border-white/5">
                  <Users className="w-4 h-4 text-violet-400" />
                  <span className="text-slate-300 font-medium">{job.contactNumbers.length} contacts</span>
                </div>
              )}
              
              {job.fullDayReportTime && (
                <div className="flex items-center gap-3 text-sm p-2 bg-slate-900/50 rounded-xl border border-white/5">
                  <FileText className="w-4 h-4 text-emerald-400" />
                  <span className="text-slate-300 font-medium">Full report: {job.fullDayReportTime}</span>
                </div>
              )}
            </div>

            {/* Actions */}
            <div className="flex gap-2">
              <button
                onClick={() => handleEditJob(jobId)}
                className="flex-1 px-4 py-2.5 bg-slate-900 hover:bg-black text-white rounded-xl transition-colors flex items-center justify-center gap-2 text-sm font-medium shadow-lg shadow-black/20 border border-white/10"
              >
                <Settings className="w-4 h-4" />
                Configure
              </button>
              
              {jobId === 'WatchedItemRealtimeJob' && (
                <button
                  onClick={() => handleOpenItemList(jobId)}
                  className="px-4 py-2.5 bg-amber-500/10 hover:bg-amber-500/20 text-amber-400 rounded-xl transition-colors flex items-center justify-center gap-2 border border-amber-500/20"
                  title="Manage watched items"
                >
                  <List className="w-4 h-4" />
                </button>
              )}
            </div>
          </div>
        )))}
      </div>

      {/* Edit Modal */}
      {mounted && editingJob && editForm && createPortal(
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-6 z-[100]">
          <div className="bg-slate-900 rounded-[2rem] max-w-4xl w-full max-h-[90vh] overflow-y-auto shadow-2xl border border-white/10">
            {/* Modal Header */}
            <div className="sticky top-0 bg-slate-900/80 backdrop-blur-md border-b border-white/10 p-6 flex items-center justify-between z-10">
              <div>
                <h2 className="text-2xl font-bold text-white mb-1">
                  Configure {editForm.name}
                </h2>
                <p className="text-slate-400 text-sm font-mono bg-slate-800 px-2 py-0.5 rounded-md inline-block">{editingJob}</p>
              </div>
              <button
                onClick={handleCancelEdit}
                className="p-2 hover:bg-slate-800 rounded-full transition-colors text-slate-400 hover:text-white"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {/* Modal Content */}
            <div className="p-8 space-y-8">
              {/* Basic Info */}
              <div className="space-y-6">
                <h3 className="text-lg font-bold text-white flex items-center gap-2">
                  <div className="w-1 h-6 bg-blue-500 rounded-full"></div>
                  Basic Information
                </h3>
                
                <div className="grid grid-cols-1 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-slate-400 mb-2">
                      Job Name
                    </label>
                    <input
                      type="text"
                      value={editForm.name}
                      onChange={(e) => updateFormField('name', e.target.value)}
                      className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-slate-400 mb-2">
                      Description
                    </label>
                    <textarea
                      value={editForm.description}
                      onChange={(e) => updateFormField('description', e.target.value)}
                      rows={3}
                      className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all resize-none"
                    />
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div>
                      <label className="block text-sm font-medium text-slate-400 mb-2">
                        Schedule
                      </label>
                      <input
                        type="text"
                        value={editForm.schedule}
                        onChange={(e) => updateFormField('schedule', e.target.value)}
                        placeholder="e.g., 0 */2 * * * or Real-time"
                        className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                      />
                      <p className="text-xs text-slate-500 mt-2 flex items-center gap-1">
                        <Clock size={12} />
                        Examples: "Every 2 hours", "8:30 AM daily", "Real-time"
                      </p>
                    </div>

                    {editForm.fullDayReportTime !== null && (
                      <div>
                        <label className="block text-sm font-medium text-slate-400 mb-2">
                          Full Report Time
                        </label>
                        <input
                          type="time"
                          value={editForm.fullDayReportTime || ''}
                          onChange={(e) => updateFormField('fullDayReportTime', e.target.value)}
                          className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                        />
                      </div>
                    )}
                  </div>

                  <div className="flex flex-col gap-3 p-4 bg-slate-800/30 rounded-xl border border-white/5">
                    <label className="flex items-center gap-3 cursor-pointer group">
                      <div className="relative flex items-center">
                        <input
                          type="checkbox"
                          checked={editForm.enabled}
                          onChange={(e) => updateFormField('enabled', e.target.checked)}
                          className="peer sr-only"
                        />
                        <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-500/20 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </div>
                      <span className="text-sm font-medium text-slate-300 group-hover:text-white">Job Enabled</span>
                    </label>

                    <label className="flex items-center gap-3 cursor-pointer group">
                      <div className="relative flex items-center">
                        <input
                          type="checkbox"
                          checked={editForm.allowEveryone}
                          onChange={(e) => updateFormField('allowEveryone', e.target.checked)}
                          className="peer sr-only"
                        />
                        <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-500/20 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                      </div>
                      <span className="text-sm font-medium text-slate-300 group-hover:text-white">Allow Everyone (Public Access)</span>
                    </label>
                  </div>
                </div>
              </div>

              {/* Contact Numbers */}
              <div className="space-y-6">
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-bold text-white flex items-center gap-2">
                    <div className="w-1 h-6 bg-violet-500 rounded-full"></div>
                    Contact Numbers
                  </h3>
                  <button
                    onClick={addContactNumber}
                    className="px-4 py-2 bg-violet-500/10 hover:bg-violet-500/20 text-violet-400 rounded-xl text-sm font-medium transition-colors flex items-center gap-2 border border-violet-500/20"
                  >
                    <Plus size={16} />
                    Add Contact
                  </button>
                </div>

                {editForm.contactNumbers.length === 0 ? (
                  <div className="text-center py-8 bg-slate-800/30 rounded-xl border border-white/10 border-dashed">
                    <Users className="w-8 h-8 text-slate-500 mx-auto mb-2" />
                    <p className="text-slate-400 text-sm">No contacts configured</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {editForm.contactNumbers.map((contact, index) => (
                      <div key={index} className="flex gap-3">
                        <input
                          type="tel"
                          value={contact}
                          onChange={(e) => updateContactNumber(index, e.target.value)}
                          placeholder="e.g., 0777078700"
                          className="flex-1 px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                        />
                        <button
                          onClick={() => removeContactNumber(index)}
                          className="px-4 py-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-xl transition-colors border border-rose-500/20"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Settings */}
              {editForm.settings && Object.keys(editForm.settings).length > 0 && (
                <div className="space-y-6">
                  <h3 className="text-lg font-bold text-white flex items-center gap-2">
                    <div className="w-1 h-6 bg-emerald-500 rounded-full"></div>
                    Advanced Settings
                  </h3>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6 p-6 bg-slate-800/30 rounded-2xl border border-white/5">
                    {Object.entries(editForm.settings).map(([key, value]) => (
                      <div key={key}>
                        <label className="block text-sm font-medium text-slate-400 mb-2 capitalize">
                          {key.replace(/([A-Z])/g, ' $1').trim()}
                        </label>
                        
                        {typeof value === 'boolean' ? (
                          <label className="flex items-center gap-3 cursor-pointer group">
                            <div className="relative flex items-center">
                              <input
                                type="checkbox"
                                checked={value}
                                onChange={(e) => updateSettingsField(key, e.target.checked)}
                                className="peer sr-only"
                              />
                              <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-500/20 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                            </div>
                            <span className="text-sm font-medium text-slate-300 group-hover:text-white">
                              {value ? 'Enabled' : 'Disabled'}
                            </span>
                          </label>
                        ) : typeof value === 'number' ? (
                          <input
                            type="number"
                            value={value}
                            onChange={(e) => updateSettingsField(key, Number(e.target.value))}
                            className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                          />
                        ) : Array.isArray(value) ? (
                          <textarea
                            value={value.join(', ')}
                            onChange={(e) => updateSettingsField(key, e.target.value.split(',').map(v => v.trim()))}
                            rows={2}
                            className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                            placeholder="Comma-separated values"
                          />
                        ) : (
                          <input
                            type="text"
                            value={String(value)}
                            onChange={(e) => updateSettingsField(key, e.target.value)}
                            className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                          />
                        )}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>

            {/* Modal Footer */}
            <div className="sticky bottom-0 bg-slate-900 border-t border-white/10 p-6 flex items-center justify-between rounded-b-[2rem]">
              <button
                onClick={() => handleResetJob(editingJob)}
                className="px-4 py-2.5 bg-amber-500/10 hover:bg-amber-500/20 text-amber-400 rounded-xl transition-colors flex items-center gap-2 text-sm font-medium border border-amber-500/20"
              >
                <RotateCcw className="w-4 h-4" />
                Reset to Default
              </button>

              <div className="flex gap-3">
                <button
                  onClick={handleCancelEdit}
                  className="px-6 py-2.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl transition-colors text-sm font-medium border border-white/10"
                >
                  Cancel
                </button>
                <button
                  onClick={handleSaveJob}
                  disabled={saving}
                  className="px-6 py-2.5 bg-blue-600 hover:bg-blue-500 text-white rounded-xl transition-colors flex items-center gap-2 text-sm font-medium shadow-lg shadow-blue-600/20 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {saving ? (
                    <>
                      <RefreshCw className="w-4 h-4 animate-spin" />
                      Saving...
                    </>
                  ) : (
                    <>
                      <Save className="w-4 h-4" />
                      Save Changes
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      , document.body)}

      {/* Watched Items List Modal */}
      {mounted && showItemList && createPortal(
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-6 z-[100]">
          <div className="bg-slate-900 rounded-[2rem] max-w-2xl w-full max-h-[80vh] overflow-hidden flex flex-col shadow-2xl border border-white/10">
            {/* Modal Header */}
            <div className="bg-slate-900 border-b border-white/10 p-6 flex items-center justify-between">
              <div>
                <h2 className="text-2xl font-bold text-white mb-1">
                  Watched Items List
                </h2>
                <p className="text-slate-400 text-sm">
                  Manage tyre patterns to monitor in real-time
                </p>
              </div>
              <button
                onClick={handleCloseItemList}
                className="p-2 hover:bg-slate-800 rounded-full transition-colors text-slate-400 hover:text-white"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {/* Modal Content */}
            <div className="flex-1 overflow-y-auto p-6">
              {/* Add New Item */}
              <div className="mb-6 p-4 bg-slate-800/30 border border-white/5 rounded-2xl">
                <label className="block text-sm font-medium text-slate-400 mb-2">
                  {editingItemIndex !== null ? 'Edit Item' : 'Add New Item'}
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={newItemName}
                    onChange={(e) => setNewItemName(e.target.value)}
                    onKeyPress={(e) => {
                      if (e.key === 'Enter') {
                        editingItemIndex !== null ? handleUpdateItem() : handleAddItem();
                      }
                    }}
                    placeholder="e.g., 195/65R15 MICHELIN PRIMACY 4"
                    className="flex-1 px-4 py-2.5 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                  />
                  {editingItemIndex !== null ? (
                    <>
                      <button
                        onClick={handleUpdateItem}
                        disabled={!newItemName.trim()}
                        className="px-4 py-2.5 bg-blue-600 hover:bg-blue-500 text-white rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2 shadow-lg shadow-blue-600/20"
                      >
                        <Save className="w-4 h-4" />
                        Update
                      </button>
                      <button
                        onClick={handleCancelItemEdit}
                        className="px-4 py-2.5 bg-slate-700 hover:bg-slate-600 text-white rounded-xl transition-colors"
                      >
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      onClick={handleAddItem}
                      disabled={!newItemName.trim()}
                      className="px-4 py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2 shadow-lg shadow-emerald-600/20"
                    >
                      <Plus className="w-4 h-4" />
                      Add
                    </button>
                  )}
                </div>
                <p className="text-xs text-slate-500 mt-2">
                  Enter full item description (e.g., size + brand + pattern)
                </p>
              </div>

              {/* Items List */}
              <div className="space-y-3">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-lg font-bold text-white">
                    Monitored Items ({watchedItems.length})
                  </h3>
                  {watchedItems.length > 0 && (
                    <span className="text-xs font-medium bg-emerald-500/10 text-emerald-400 px-2 py-1 rounded-full">
                      Active
                    </span>
                  )}
                </div>

                {watchedItems.length === 0 ? (
                  <div className="text-center py-12 bg-slate-800/30 rounded-2xl border border-white/10 border-dashed">
                    <List className="w-12 h-12 text-slate-500 mx-auto mb-3" />
                    <p className="text-slate-400 font-medium mb-1">No items being monitored</p>
                    <p className="text-sm text-slate-500">
                      Add items above to start receiving real-time alerts
                    </p>
                  </div>
                ) : (
                  <div className="space-y-2">
                    {watchedItems.map((item, index) => (
                      <div
                        key={index}
                        className={`p-4 rounded-xl border transition-all ${
                          editingItemIndex === index
                            ? 'bg-blue-500/10 border-blue-500/30'
                            : 'bg-slate-800/50 border-white/5 hover:border-white/10 shadow-sm'
                        }`}
                      >
                        <div className="flex items-center justify-between">
                          <div className="flex-1">
                            <div className="flex items-center gap-3">
                              <span className="w-8 h-8 bg-amber-500/10 rounded-lg flex items-center justify-center text-amber-500 font-bold text-sm">
                                {index + 1}
                              </span>
                              <div>
                                <p className="text-white font-medium">
                                  {item}
                                </p>
                                <p className="text-xs text-slate-500 mt-0.5">
                                  Monitored in real-time • Alerts on sale
                                </p>
                              </div>
                            </div>
                          </div>
                          
                          <div className="flex items-center gap-2">
                            <button
                              onClick={() => handleEditItem(index)}
                              disabled={editingItemIndex !== null && editingItemIndex !== index}
                              className="p-2 bg-slate-700/50 hover:bg-slate-600/50 text-slate-300 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                              title="Edit item"
                            >
                              <Edit2 className="w-4 h-4" />
                            </button>
                            <button
                              onClick={() => handleRemoveItem(index)}
                              disabled={editingItemIndex !== null}
                              className="p-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                              title="Remove item"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>

            {/* Modal Footer */}
            <div className="bg-slate-900 border-t border-white/10 p-6 flex items-center justify-between">
              <div className="text-sm text-slate-500">
                <p>Changes are saved automatically</p>
              </div>
              <button
                onClick={handleCloseItemList}
                className="px-6 py-2.5 bg-slate-800 hover:bg-slate-700 text-white rounded-xl transition-colors font-medium shadow-lg shadow-black/20 border border-white/10"
              >
                Done
              </button>
            </div>
          </div>
        </div>
      , document.body)}
    </div>
  );
}

// Wrap in Suspense for useSearchParams
export default function JobsPageWrapper() {
  return (
    <Suspense fallback={
      <div className="min-h-screen bg-[#f3f4f6] flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto mb-4"></div>
          <p className="text-zinc-500 text-sm font-medium">Loading Jobs...</p>
        </div>
      </div>
    }>
      <JobsPage />
    </Suspense>
  );
}
