'use client';

import { useState, useEffect, Suspense } from 'react';
import { createPortal } from 'react-dom';
import { useRouter, useSearchParams } from 'next/navigation';
import { checkAuth } from '@/core/lib/client-auth';
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
  Filter,
  Smartphone,
  ChevronRight,
  MoreVertical,
  ArrowLeft
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
    enabled: true,
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
    enabled: true,
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
    enabled: true,
    allowEveryone: true,
    contactNumbers: [],
    settings: {}
  },
  TyrePhotoReplyJob: {
    name: 'Tyre Photo Reply',
    description: 'Send tyre product images to customers',
    schedule: 'Real-time',
    type: 'auto-reply',
    enabled: true,
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
    enabled: true,
    allowEveryone: false,
    contactNumbers: [],
    settings: {}
  },
  VehicleInvoiceReplyJob: {
    name: 'Vehicle Invoice Reply',
    description: 'Reply with detailed vehicle invoice data',
    schedule: 'Real-time',
    type: 'auto-reply',
    enabled: true,
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
    enabled: true,
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
    enabled: true,
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
    enabled: true,
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
    enabled: false,
    allowEveryone: false,
    contactNumbers: ['0777078700', '0777311770'],
    settings: {
      includeProfit: true,
      useSQLite: true,
      reportIncludeProfit: true
    }
  },
  DailyTyreSalesPdfSendJob: {
    name: 'Daily Sales PDF Report',
    description: 'Sends previous day sales PDF at 7:00 AM',
    schedule: 'Daily at 7:00 AM',
    type: 'reporting',
    enabled: true,
    allowEveryone: false,
    contactNumbers: ['0777311770', '0771222509'],
    settings: {
      reportTime: '07:00',
      includeProfit: true
    }
  },
  WeeklyTyreSalesReportJob: {
    name: 'Weekly Sales Report',
    description: 'Weekly sales summary report every Sunday',
    schedule: 'Sunday 8:00 AM',
    type: 'reporting',
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
    enabled: false,
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
  const category = searchParams.get('category');
  const [mounted, setMounted] = useState(false);
  // Initialize with defaults to ensure UI is populated even if API fails
  const [jobs, setJobs] = useState<AllJobsConfig>(JOB_DEFAULTS as unknown as AllJobsConfig);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [editingJob, setEditingJob] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<JobConfig | null>(null);
  const [saving, setSaving] = useState(false);
  const [runningJob, setRunningJob] = useState<string | null>(null);
  const [showItemList, setShowItemList] = useState(false);
  const [watchedItems, setWatchedItems] = useState<string[]>([]);
  const [newItemName, setNewItemName] = useState('');
  const [editingItemIndex, setEditingItemIndex] = useState<number | null>(null);

  useEffect(() => {
    setMounted(true);
    fetchJobs();

    let eventSource: EventSource | null = null;

    const connectSSE = () => {
      if (eventSource) {
        (eventSource as EventSource).close();
      }
      
      console.log('Frontend: Connecting SSE...');
      eventSource = new EventSource('/api/sse');
      
      eventSource.onmessage = (event) => {
        try {
          const data = JSON.parse(event.data);
          if (data.type === 'job_update') {
            setJobs(prev => ({
              ...prev,
              [data.jobId]: {
                ...prev[data.jobId],
                ...data.config
              }
            }));
          }
        } catch (e) {
          console.error('SSE Parse Error:', e);
        }
      };
    };

    connectSSE();

    const handleWakeUp = () => {
      console.log('JobPage: Wake up signal received');
      fetchJobs();
      // Reconnect SSE to ensure fresh connection
      connectSSE();
    };

    window.addEventListener('app-wake-up', handleWakeUp);

    return () => {
      if (eventSource) {
        (eventSource as EventSource).close();
      }
      window.removeEventListener('app-wake-up', handleWakeUp);
    };
  }, []);

  const fetchJobs = async () => {
    try {
      setError(null);
      console.log('Frontend: Fetching jobs...');
      const response = await fetch('/api/jobs/config', { cache: 'no-store' });
      console.log('Frontend: Response status:', response.status, 'OK:', response.ok);
      if (response.ok) {
        const data = await response.json();
        console.log('Frontend: Data received:', Object.keys(data).length, 'keys');
        
        // Merge API data with defaults to ensure we don't lose definitions
        // and to fix "unknown" types from legacy config
        const merged: AllJobsConfig = { ...JOB_DEFAULTS } as AllJobsConfig;
        
        Object.entries(data).forEach(([key, value]: [string, any]) => {
          if (merged[key]) {
            console.log(`Frontend: Merging ${key}, enabled: ${value.enabled}`);
            merged[key] = {
              ...merged[key],
              ...value,
              // Ensure enabled is boolean and defaults to false if missing
              enabled: value.enabled !== undefined ? !!value.enabled : (merged[key].enabled || false),
              // Preserve type if API has it as unknown but we know it
              type: (value.type === 'unknown' && merged[key]?.type) 
                    ? merged[key].type 
                    : (value.type || merged[key]?.type || 'system')
            };
          } else {
            merged[key] = {
              ...value,
              enabled: !!value.enabled
            };
          }
        });
        
        setJobs(merged);
      } else {
        const statusLabel = response.statusText || 'Server error';
        console.error('Frontend: Fetch failed', statusLabel);
        setError(`Failed to load jobs: ${response.status} ${statusLabel}`);
      }
    } catch (error) {
      console.error('Failed to fetch jobs:', error);
      setError(`Connection error: ${(error as Error).message}`);
    } finally {
      setLoading(false);
    }
  };

  const handleToggleJob = async (jobId: string, currentStatus: boolean) => {
    try {
      // Optimistic update
      setJobs(prev => ({
        ...prev,
        [jobId]: { ...prev[jobId], enabled: !currentStatus }
      }));

      const response = await fetch('/api/jobs/config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          jobId,
          config: { ...jobs[jobId], enabled: !currentStatus }
        })
      });

      if (!response.ok) {
        // Revert on failure
        setJobs(prev => ({
          ...prev,
          [jobId]: { ...prev[jobId], enabled: currentStatus }
        }));
      }
    } catch (error) {
      console.error('Failed to toggle job:', error);
      // Revert on error
      setJobs(prev => ({
        ...prev,
        [jobId]: { ...prev[jobId], enabled: currentStatus }
      }));
    }
  };

  const handleRunJob = async (jobId: string) => {
    setRunningJob(jobId);
    try {
      await fetch('/api/jobs/run', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ jobId })
      });
    } catch (error) {
      console.error('Failed to run job:', error);
    } finally {
      setTimeout(() => setRunningJob(null), 2000);
    }
  };

  const handleEditJob = (jobId: string) => {
    const job = jobs[jobId];
    if (!job) return;
    
    setEditingJob(jobId);
    setEditForm(JSON.parse(JSON.stringify(job))); // Deep copy
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

      if (response.ok) {
        setJobs(prev => ({
          ...prev,
          [editingJob]: editForm
        }));
        setEditingJob(null);
        setEditForm(null);
      }
    } catch (error) {
      console.error('Failed to save job:', error);
    } finally {
      setSaving(false);
    }
  };

  const handleCancelEdit = () => {
    setEditingJob(null);
    setEditForm(null);
  };

  const handleResetJob = (jobId: string) => {
    const defaults = JOB_DEFAULTS[jobId];
    if (defaults && editForm) {
      setEditForm({
        ...editForm,
        ...defaults,
        settings: defaults.settings || {}
      });
    }
  };

  const updateSettingsField = (key: string, value: any) => {
    if (!editForm) return;
    setEditForm({
      ...editForm,
      settings: {
        ...editForm.settings,
        [key]: value
      }
    });
  };

  const updateContactNumber = (index: number, value: string) => {
    if (!editForm) return;
    const newContacts = [...editForm.contactNumbers];
    newContacts[index] = value;
    setEditForm({ ...editForm, contactNumbers: newContacts });
  };

  const addContactNumber = () => {
    if (!editForm) return;
    setEditForm({
      ...editForm,
      contactNumbers: [...editForm.contactNumbers, '']
    });
  };

  const removeContactNumber = (index: number) => {
    if (!editForm) return;
    const newContacts = editForm.contactNumbers.filter((_, i) => i !== index);
    setEditForm({ ...editForm, contactNumbers: newContacts });
  };

  // Watcher List Management
  const openWatcherList = () => {
    const watcherJob = jobs['WatchedItemRealtimeJob'];
    if (watcherJob && watcherJob.settings?.patterns) {
      setWatchedItems([...watcherJob.settings.patterns]);
      setShowItemList(true);
    }
  };

  const handleAddItem = () => {
    if (newItemName.trim()) {
      setWatchedItems([...watchedItems, newItemName.trim()]);
      setNewItemName('');
    }
  };

  const handleUpdateItem = () => {
    if (editingItemIndex !== null && newItemName.trim()) {
      const newItems = [...watchedItems];
      newItems[editingItemIndex] = newItemName.trim();
      setWatchedItems(newItems);
      setEditingItemIndex(null);
      setNewItemName('');
    }
  };

  const handleEditItem = (index: number) => {
    setNewItemName(watchedItems[index]);
    setEditingItemIndex(index);
  };

  const handleRemoveItem = (index: number) => {
    setWatchedItems(watchedItems.filter((_, i) => i !== index));
  };

  const handleCancelItemEdit = () => {
    setEditingItemIndex(null);
    setNewItemName('');
  };

  const handleCloseItemList = async () => {
    // Save changes to the watcher job
    const watcherJob = jobs['WatchedItemRealtimeJob'];
    if (watcherJob) {
      const updatedJob = {
        ...watcherJob,
        settings: {
          ...watcherJob.settings,
          patterns: watchedItems
        }
      };

      try {
        await fetch('/api/jobs/config', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            jobId: 'WatchedItemRealtimeJob',
            config: updatedJob
          })
        });
        
        setJobs(prev => ({
          ...prev,
          WatchedItemRealtimeJob: updatedJob
        }));
      } catch (error) {
        console.error('Failed to save watched items:', error);
      }
    }
    setShowItemList(false);
  };

  const filteredJobs = Object.entries(jobs).filter(([id, job]) => {
    if (!job) return false;
    const matchesFilter = filter === 'all' || job.type === filter;
    const matchesSearch = (job.name?.toLowerCase() || '').includes(searchQuery.toLowerCase()) ||
                         (job.description?.toLowerCase() || '').includes(searchQuery.toLowerCase());
    return matchesFilter && matchesSearch;
  });

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading Job Manager...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 pb-20 md:pb-10 w-full max-w-[100vw] overflow-x-hidden">
      {error && (
        <div className="fixed top-4 right-4 z-50 bg-red-500/90 text-white px-4 py-2 rounded-lg shadow-lg backdrop-blur-sm border border-red-400 text-sm font-medium animate-in fade-in slide-in-from-top-2">
          {error}
          <button onClick={() => setError(null)} className="ml-2 opacity-75 hover:opacity-100">✕</button>
        </div>
      )}
      {/* Mobile Header - Card Style */}
      <div className="sticky top-0 z-30 w-full px-3 py-3 md:px-6 md:py-6">
        <div className="bg-slate-900/90 backdrop-blur-md border border-white/10 rounded-2xl p-3 md:p-5 shadow-xl w-full md:max-w-7xl md:mx-auto">
          <div className="flex flex-col md:flex-row md:items-center justify-between gap-3 md:gap-4">
            <div className="flex flex-col">
              <Link href="/dashboard" className="flex items-center gap-2 text-slate-400 hover:text-white mb-2 w-fit transition-colors">
                <ArrowLeft className="w-4 h-4" />
                <span className="text-xs font-medium">Back to Dashboard</span>
              </Link>
              <h1 className="text-lg md:text-2xl font-bold text-white flex items-center gap-2 md:gap-3">
                <Smartphone className="w-5 h-5 md:w-7 md:h-7 text-blue-500" />
                Job Manager
              </h1>
              <p className="text-slate-400 text-[10px] md:text-sm mt-0.5 md:mt-1">
                {Object.keys(jobs).length} active jobs
              </p>
            </div>

            <div className="flex flex-col gap-2 md:gap-3 w-full md:w-auto">
              <div className="relative w-full md:w-64">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 md:w-4 md:h-4 text-slate-500" />
                <input
                  type="text"
                  placeholder="Search jobs..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="w-full pl-9 pr-4 py-1.5 md:py-2.5 bg-slate-800 border border-white/10 rounded-xl text-xs md:text-base text-white placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500/50"
                />
              </div>
              
              <div className="flex gap-1.5 md:gap-2 overflow-x-auto pb-1 md:pb-0 no-scrollbar w-full">
                {['all', 'auto-reply', 'reporting', 'system', 'monitoring', 'social-media'].map((f) => (
                  <button
                    key={f}
                    onClick={() => setFilter(f)}
                    className={`px-2.5 md:px-4 py-1.5 md:py-2.5 rounded-lg md:rounded-xl text-[10px] md:text-sm font-medium whitespace-nowrap transition-all flex-shrink-0 ${
                      filter === f
                        ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20'
                        : 'bg-slate-800 text-slate-400 hover:bg-slate-700 hover:text-white'
                    }`}
                  >
                    {f.charAt(0).toUpperCase() + f.slice(1)}
                  </button>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Content Grid */}
      <div className="w-full md:max-w-7xl md:mx-auto px-0 md:px-8 py-4 md:py-6">
        {filteredJobs.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-center mx-4">
            <div className="w-20 h-20 bg-slate-800/50 rounded-full flex items-center justify-center mb-4">
              <Search className="w-10 h-10 text-slate-500" />
            </div>
            <h3 className="text-xl font-bold text-white mb-2">No jobs found</h3>
            <p className="text-slate-400 max-w-md mx-auto mb-6">
              We couldn't find any jobs matching your current filters.
            </p>
            <button 
              onClick={() => { setFilter('all'); setSearchQuery(''); }}
              className="px-6 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-full font-medium transition-colors"
            >
              Clear Filters
            </button>
          </div>
        ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 md:gap-6 w-full max-w-md sm:max-w-none mx-auto">
          {filteredJobs.map(([id, job]) => (
            <div 
              key={id}
              className={`group relative bg-slate-900 rounded-2xl border transition-all duration-300 w-full overflow-hidden flex flex-col ${
                job.enabled 
                  ? 'border-blue-500/20 hover:border-blue-500/40 shadow-lg shadow-blue-900/5' 
                  : 'border-white/5 hover:border-white/10 opacity-75 hover:opacity-100'
              }`}
            >
              {/* Card Header */}
              <div className="p-4 md:p-5 flex-1">
                <div className="flex items-start justify-between mb-3 md:mb-4 gap-3">
                  <div className={`p-2.5 md:p-3 rounded-xl flex-shrink-0 ${
                    job.enabled ? 'bg-blue-500/10 text-blue-400' : 'bg-slate-800 text-slate-500'
                  }`}>
                    {job.type === 'reporting' ? <FileText className="w-5 h-5 md:w-6 md:h-6" /> :
                     job.type === 'system' ? <Settings className="w-5 h-5 md:w-6 md:h-6" /> :
                     job.type === 'monitoring' ? <Search className="w-5 h-5 md:w-6 md:h-6" /> :
                     <CheckCircle className="w-5 h-5 md:w-6 md:h-6" />}
                  </div>
                  <div className="flex items-center gap-2 flex-shrink-0">
                    <span className={`text-[10px] font-bold uppercase tracking-wider ${job.enabled ? 'text-blue-400' : 'text-slate-600'}`}>
                      {job.enabled ? 'ON' : 'OFF'}
                    </span>
                    <button
                      onClick={() => handleToggleJob(id, !!job.enabled)}
                      className={`relative inline-flex h-5 w-9 md:h-6 md:w-11 items-center rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 focus:ring-offset-slate-900 ${
                        job.enabled ? 'bg-blue-600' : 'bg-slate-700'
                      }`}
                    >
                      <span
                        className={`inline-block h-3.5 w-3.5 md:h-4 md:w-4 transform rounded-full bg-white transition-transform ${
                          job.enabled ? 'translate-x-5 md:translate-x-6' : 'translate-x-0.5 md:translate-x-1'
                        }`}
                      />
                    </button>
                  </div>
                </div>

                <h3 className="text-sm md:text-lg font-bold text-white mb-0.5 md:mb-1 line-clamp-1 break-all">{job.name || id}</h3>
                <p className="text-slate-400 text-xs md:text-sm line-clamp-1 md:line-clamp-2 mb-2 md:mb-4 break-words">
                  {job.description || 'No description available'}
                </p>

                <div className="flex items-center gap-1.5 md:gap-2 text-xs text-slate-500 bg-slate-800/50 p-1.5 md:p-2 rounded-lg w-full overflow-hidden">
                  <Clock className="w-3 h-3 md:w-3.5 md:h-3.5 flex-shrink-0" />
                  <span className="truncate text-[10px] md:text-xs">{job.schedule}</span>
                </div>
              </div>

              {/* Card Actions */}
              <div className="grid grid-cols-2 gap-px bg-slate-800/50 border-t border-white/5 rounded-b-2xl overflow-hidden w-full">
                <button
                  onClick={() => handleEditJob(id)}
                  className="p-2 md:p-3 flex items-center justify-center gap-1.5 md:gap-2 text-xs md:text-sm font-medium text-slate-300 hover:bg-slate-800 hover:text-white transition-colors min-h-[40px] md:min-h-[44px] w-full"
                >
                  <Settings className="w-4 h-4 flex-shrink-0" />
                  <span className="hidden sm:inline truncate">Configure</span>
                </button>
                <button
                  onClick={() => handleRunJob(id)}
                  disabled={runningJob === id}
                  className={`p-2 md:p-3 flex items-center justify-center gap-1.5 md:gap-2 text-xs md:text-sm font-medium transition-colors min-h-[40px] md:min-h-[44px] w-full ${
                    runningJob === id
                      ? 'bg-blue-500/10 text-blue-400'
                      : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                  }`}
                >
                  {runningJob === id ? (
                    <RefreshCw className="w-4 h-4 animate-spin flex-shrink-0" />
                  ) : (
                    <Play className="w-4 h-4 flex-shrink-0" />
                  )}
                  <span className="hidden sm:inline truncate">{runningJob === id ? 'Running' : 'Run Now'}</span>
                </button>
              </div>
              
              {/* Special Action for Watcher */}
              {id === 'WatchedItemRealtimeJob' && (
                <button
                  onClick={openWatcherList}
                  className="absolute top-4 right-14 p-2 text-slate-400 hover:text-white hover:bg-slate-800 rounded-lg transition-colors"
                  title="Manage Watched Items"
                >
                  <List className="w-5 h-5" />
                </button>
              )}
            </div>
          ))}
        </div>
        )}
      </div>

      {/* Edit Modal - Full Screen on Mobile */}
      {editingJob && editForm && createPortal(
        <div className="fixed inset-0 z-50 bg-slate-950 flex flex-col md:flex-row md:items-center md:justify-center md:bg-black/80 md:backdrop-blur-sm md:p-6">
          <div className="flex-1 flex flex-col bg-slate-900 w-full h-full md:h-auto md:max-h-[90vh] md:max-w-3xl md:rounded-3xl md:border md:border-white/10 shadow-2xl overflow-hidden">
            
            {/* Modal Header */}
            <div className="flex items-center justify-between p-4 md:p-6 border-b border-white/10 bg-slate-900">
              <div>
                <h2 className="text-xl md:text-2xl font-bold text-white">Configure Job</h2>
                <p className="text-slate-400 text-sm">{editForm.name}</p>
              </div>
              <button 
                onClick={handleCancelEdit}
                className="p-2 hover:bg-slate-800 rounded-full text-slate-400 hover:text-white transition-colors"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {/* Modal Content - Scrollable */}
            <div className="flex-1 overflow-y-auto p-4 md:p-6 space-y-6 md:space-y-8">
              
              {/* Basic Settings */}
              <div className="space-y-4">
                <h3 className="text-sm font-bold text-blue-400 uppercase tracking-wider">Basic Configuration</h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-300">Job Name</label>
                    <input
                      type="text"
                      value={editForm.name}
                      onChange={(e) => setEditForm({...editForm, name: e.target.value})}
                      className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                    />
                  </div>
                  
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-300">Schedule</label>
                    <input
                      type="text"
                      value={editForm.schedule}
                      onChange={(e) => setEditForm({...editForm, schedule: e.target.value})}
                      className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-300">Description</label>
                  <textarea
                    value={editForm.description}
                    onChange={(e) => setEditForm({...editForm, description: e.target.value})}
                    rows={2}
                    className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all resize-none"
                  />
                </div>

                <div className="flex items-center justify-between p-4 bg-slate-800/30 rounded-xl border border-white/5">
                  <div>
                    <p className="text-white font-medium">Public Access</p>
                    <p className="text-xs text-slate-500">Allow anyone to trigger this job</p>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input
                      type="checkbox"
                      checked={editForm.allowEveryone}
                      onChange={(e) => setEditForm({...editForm, allowEveryone: e.target.checked})}
                      className="sr-only peer"
                    />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-800 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                  </label>
                </div>
              </div>

              {/* Contacts */}
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <h3 className="text-sm font-bold text-blue-400 uppercase tracking-wider">Authorized Contacts</h3>
                  <button
                    onClick={addContactNumber}
                    className="text-xs bg-blue-500/10 text-blue-400 px-3 py-1.5 rounded-lg hover:bg-blue-500/20 transition-colors flex items-center gap-1"
                  >
                    <Plus size={14} />
                    Add
                  </button>
                </div>

                {editForm.contactNumbers.length === 0 ? (
                  <div className="text-center py-6 bg-slate-800/30 rounded-xl border border-white/10 border-dashed">
                    <Users className="w-6 h-6 text-slate-500 mx-auto mb-2" />
                    <p className="text-slate-400 text-sm">No contacts configured</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {editForm.contactNumbers.map((contact, index) => (
                      <div key={index} className="flex gap-2">
                        <input
                          type="tel"
                          value={contact}
                          onChange={(e) => updateContactNumber(index, e.target.value)}
                          placeholder="e.g., 0777078700"
                          className="flex-1 px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                        />
                        <button
                          onClick={() => removeContactNumber(index)}
                          className="px-4 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-xl transition-colors border border-rose-500/20"
                        >
                          <Trash2 className="w-5 h-5" />
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Advanced Settings */}
              {editForm.settings && Object.keys(editForm.settings).length > 0 && (
                <div className="space-y-4">
                  <h3 className="text-sm font-bold text-blue-400 uppercase tracking-wider">Advanced Parameters</h3>
                  <div className="grid grid-cols-1 gap-4 p-4 bg-slate-800/30 rounded-xl border border-white/5">
                    {Object.entries(editForm.settings).map(([key, value]) => (
                      <div key={key} className="space-y-2">
                        <label className="text-sm font-medium text-slate-300 capitalize">
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
                            <span className="text-sm text-slate-400 group-hover:text-white transition-colors">
                              {value ? 'Enabled' : 'Disabled'}
                            </span>
                          </label>
                        ) : typeof value === 'number' ? (
                          <input
                            type="number"
                            value={value}
                            onChange={(e) => updateSettingsField(key, Number(e.target.value))}
                            className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                          />
                        ) : Array.isArray(value) ? (
                          <textarea
                            value={value.join(', ')}
                            onChange={(e) => updateSettingsField(key, e.target.value.split(',').map(v => v.trim()))}
                            rows={2}
                            className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                            placeholder="Comma-separated values"
                          />
                        ) : (
                          <input
                            type="text"
                            value={String(value)}
                            onChange={(e) => updateSettingsField(key, e.target.value)}
                            className="w-full px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                          />
                        )}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>

            {/* Modal Footer */}
            <div className="p-4 md:p-6 border-t border-white/10 bg-slate-900 flex items-center justify-between gap-4">
              <button
                onClick={() => handleResetJob(editingJob)}
                className="px-4 py-3 bg-amber-500/10 hover:bg-amber-500/20 text-amber-400 rounded-xl transition-colors flex items-center gap-2 text-sm font-medium border border-amber-500/20"
              >
                <RotateCcw className="w-4 h-4" />
                <span className="hidden sm:inline">Reset</span>
              </button>

              <div className="flex gap-3 flex-1 justify-end">
                <button
                  onClick={handleCancelEdit}
                  className="px-6 py-3 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl transition-colors text-sm font-medium border border-white/10"
                >
                  Cancel
                </button>
                <button
                  onClick={handleSaveJob}
                  disabled={saving}
                  className="px-6 py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-xl transition-colors flex items-center gap-2 text-sm font-medium shadow-lg shadow-blue-600/20 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {saving ? (
                    <>
                      <RefreshCw className="w-4 h-4 animate-spin" />
                      Saving...
                    </>
                  ) : (
                    <>
                      <Save className="w-4 h-4" />
                      Save
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
        <div className="fixed inset-0 z-[100] bg-slate-950 flex flex-col md:flex-row md:items-center md:justify-center md:bg-black/80 md:backdrop-blur-sm md:p-6">
          <div className="flex-1 flex flex-col bg-slate-900 w-full h-full md:h-auto md:max-h-[80vh] md:max-w-2xl md:rounded-3xl md:border md:border-white/10 shadow-2xl overflow-hidden">
            {/* Modal Header */}
            <div className="bg-slate-900 border-b border-white/10 p-4 md:p-6 flex items-center justify-between">
              <div>
                <h2 className="text-xl md:text-2xl font-bold text-white mb-1">
                  Watched Items
                </h2>
                <p className="text-slate-400 text-sm">
                  Real-time monitoring list
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
            <div className="flex-1 overflow-y-auto p-4 md:p-6">
              {/* Add New Item */}
              <div className="mb-6 p-4 bg-slate-800/30 border border-white/5 rounded-2xl">
                <label className="block text-sm font-medium text-slate-400 mb-2">
                  {editingItemIndex !== null ? 'Edit Item' : 'Add New Item'}
                </label>
                <div className="flex flex-col sm:flex-row gap-2">
                  <input
                    type="text"
                    value={newItemName}
                    onChange={(e) => setNewItemName(e.target.value)}
                    onKeyPress={(e) => {
                      if (e.key === 'Enter') {
                        editingItemIndex !== null ? handleUpdateItem() : handleAddItem();
                      }
                    }}
                    placeholder="e.g., 195/65R15 MICHELIN"
                    className="flex-1 px-4 py-3 bg-slate-800/50 border border-white/10 rounded-xl text-white focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 focus:outline-none transition-all"
                  />
                  <div className="flex gap-2">
                    {editingItemIndex !== null ? (
                      <>
                        <button
                          onClick={handleUpdateItem}
                          disabled={!newItemName.trim()}
                          className="flex-1 sm:flex-none px-4 py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg shadow-blue-600/20"
                        >
                          <Save className="w-4 h-4" />
                          Update
                        </button>
                        <button
                          onClick={handleCancelItemEdit}
                          className="flex-1 sm:flex-none px-4 py-3 bg-slate-700 hover:bg-slate-600 text-white rounded-xl transition-colors"
                        >
                          Cancel
                        </button>
                      </>
                    ) : (
                      <button
                        onClick={handleAddItem}
                        disabled={!newItemName.trim()}
                        className="flex-1 sm:flex-none px-6 py-3 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg shadow-emerald-600/20"
                      >
                        <Plus className="w-4 h-4" />
                        Add
                      </button>
                    )}
                  </div>
                </div>
              </div>

              {/* Items List */}
              <div className="space-y-3">
                {watchedItems.length === 0 ? (
                  <div className="text-center py-12 bg-slate-800/30 rounded-2xl border border-white/10 border-dashed">
                    <List className="w-12 h-12 text-slate-500 mx-auto mb-3" />
                    <p className="text-slate-400 font-medium">No items being monitored</p>
                  </div>
                ) : (
                  watchedItems.map((item, index) => (
                    <div
                      key={index}
                      className={`p-4 rounded-xl border transition-all ${
                        editingItemIndex === index
                          ? 'bg-blue-500/10 border-blue-500/30'
                          : 'bg-slate-800/50 border-white/5 hover:border-white/10'
                      }`}
                    >
                      <div className="flex items-center justify-between gap-3">
                        <div className="flex items-center gap-3 min-w-0">
                          <span className="w-8 h-8 bg-amber-500/10 rounded-lg flex-shrink-0 flex items-center justify-center text-amber-500 font-bold text-sm">
                            {index + 1}
                          </span>
                          <p className="text-white font-medium truncate">
                            {item}
                          </p>
                        </div>
                        
                        <div className="flex items-center gap-2 flex-shrink-0">
                          <button
                            onClick={() => handleEditItem(index)}
                            disabled={editingItemIndex !== null && editingItemIndex !== index}
                            className="p-2 bg-slate-700/50 hover:bg-slate-600/50 text-slate-300 rounded-lg transition-colors"
                          >
                            <Edit2 className="w-4 h-4" />
                          </button>
                          <button
                            onClick={() => handleRemoveItem(index)}
                            disabled={editingItemIndex !== null}
                            className="p-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-lg transition-colors"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>

            {/* Modal Footer */}
            <div className="bg-slate-900 border-t border-white/10 p-4 md:p-6">
              <button
                onClick={handleCloseItemList}
                className="w-full px-6 py-3 bg-slate-800 hover:bg-slate-700 text-white rounded-xl transition-colors font-medium shadow-lg shadow-black/20 border border-white/10"
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
      <div className="min-h-screen bg-slate-950 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading Jobs...</p>
        </div>
      </div>
    }>
      <JobsPage />
    </Suspense>
  );
}
