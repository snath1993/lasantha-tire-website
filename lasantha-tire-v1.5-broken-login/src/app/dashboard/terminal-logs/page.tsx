'use client';
import { useEffect, useState, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { Terminal, AlertCircle, CheckCircle, Info, Clock, RefreshCw, Play, Pause, Settings } from 'lucide-react';

interface LogEntry {
  timestamp: string;
  level: 'info' | 'error' | 'warn';
  message: string;
  source: string;
}

export default function TerminalLogsPage() {
  const router = useRouter();
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [selectedTab, setSelectedTab] = useState<'all' | 'out' | 'error' | 'pm2'>('all');
  const [autoRefresh, setAutoRefresh] = useState(true);
  const [refreshInterval, setRefreshInterval] = useState(3000); // 3 seconds
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const logsEndRef = useRef<HTMLDivElement>(null);
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  // Auto-scroll to bottom when new logs arrive
  const scrollToBottom = () => {
    logsEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [logs]);

  const fetchLogs = async () => {
    if (isLoading) return;
    
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`/api/terminal-logs?type=${selectedTab}&lines=200`, {
        cache: 'no-store',
      });
      
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }
      
      const data = await response.json();
      
      if (data.success) {
        setLogs(data.logs || []);
      } else {
        setError(data.error || 'Failed to fetch logs');
      }
    } catch (err: any) {
      console.error('Error fetching logs:', err);
      setError(err.message || 'Network error');
    } finally {
      setIsLoading(false);
    }
  };

  // Initial load
  useEffect(() => {
    if (!isMounted) return;
    fetchLogs();
  }, [selectedTab, isMounted]);

  // Auto-refresh timer
  useEffect(() => {
    if (!autoRefresh || !isMounted) return;

    const timer = setInterval(() => {
      fetchLogs();
    }, refreshInterval);

    return () => clearInterval(timer);
  }, [autoRefresh, refreshInterval, selectedTab, isMounted]);

  const formatTimestamp = (timestamp: string) => {
    try {
      const date = new Date(timestamp);
      return date.toLocaleString('en-US', {
        month: '2-digit',
        day: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false
      });
    } catch {
      return timestamp;
    }
  };

  const getLogColor = (level: string) => {
    switch (level) {
      case 'error':
        return 'text-rose-400 bg-rose-500/10 border-rose-500/20 hover:bg-rose-500/20';
      case 'warn':
        return 'text-amber-400 bg-amber-500/10 border-amber-500/20 hover:bg-amber-500/20';
      default:
        return 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20 hover:bg-emerald-500/20';
    }
  };

  const getMessageColor = (message: string, level: string) => {
    if (level === 'error' || message.toLowerCase().includes('error')) {
      return 'text-rose-300';
    }
    if (message.toLowerCase().includes('warn')) {
      return 'text-amber-300';
    }
    if (message.toLowerCase().includes('✅') || message.toLowerCase().includes('success')) {
      return 'text-emerald-300';
    }
    return 'text-slate-300';
  };

  if (!isMounted) {
    return null;
  }

  return (
    <main className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 text-white p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-slate-800/50 rounded-2xl flex items-center justify-center border border-white/10 backdrop-blur-xl shadow-lg">
              <Terminal className="w-6 h-6 text-blue-400" />
            </div>
            <div>
              <h1 className="text-3xl font-bold text-white tracking-tight">
                Terminal Logs
              </h1>
              <p className="text-slate-400 text-sm font-medium">
                Live PM2 process monitoring with timestamps
              </p>
            </div>
          </div>

          {/* Controls */}
          <div className="flex items-center gap-3 bg-slate-800/50 p-2 rounded-2xl border border-white/10 backdrop-blur-xl">
            <button
              onClick={() => setAutoRefresh(!autoRefresh)}
              className={`px-4 py-2 rounded-xl transition-all flex items-center gap-2 text-sm font-bold ${
                autoRefresh 
                  ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30 shadow-lg shadow-emerald-500/10' 
                  : 'bg-slate-700/50 text-slate-400 border border-white/5 hover:bg-slate-700'
              }`}
            >
              {autoRefresh ? <Pause className="w-4 h-4" /> : <Play className="w-4 h-4" />}
              {autoRefresh ? 'Auto-refresh ON' : 'Auto-refresh OFF'}
            </button>

            <div className="h-8 w-px bg-white/10 mx-1"></div>

            <div className="flex items-center gap-2 px-3">
              <Clock className="w-4 h-4 text-slate-400" />
              <select
                value={refreshInterval}
                onChange={(e) => setRefreshInterval(Number(e.target.value))}
                className="bg-transparent text-slate-300 text-sm font-medium focus:outline-none cursor-pointer"
              >
                <option value={1000} className="bg-slate-800">1s</option>
                <option value={3000} className="bg-slate-800">3s</option>
                <option value={5000} className="bg-slate-800">5s</option>
                <option value={10000} className="bg-slate-800">10s</option>
              </select>
            </div>

            <button
              onClick={fetchLogs}
              disabled={isLoading}
              className="p-2 bg-blue-600 hover:bg-blue-700 text-white rounded-xl transition-all shadow-lg shadow-blue-600/20 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <RefreshCw className={`w-4 h-4 ${isLoading ? 'animate-spin' : ''}`} />
            </button>
          </div>
        </div>

        {/* Tabs */}
        <div className="flex gap-3 mb-6 overflow-x-auto pb-2">
          {[
            { id: 'all', label: 'All Logs', icon: Terminal },
            { id: 'out', label: 'Output', icon: Info },
            { id: 'error', label: 'Errors', icon: AlertCircle },
            { id: 'pm2', label: 'PM2 Status', icon: Settings }
          ].map((tab) => {
            const Icon = tab.icon;
            const isActive = selectedTab === tab.id;
            return (
              <button
                key={tab.id}
                onClick={() => setSelectedTab(tab.id as any)}
                className={`flex items-center gap-2 px-6 py-3 rounded-xl font-bold transition-all text-sm ${
                  isActive
                    ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20 scale-105'
                    : 'bg-slate-800/50 text-slate-400 border border-white/5 hover:bg-slate-700/50 hover:text-slate-200'
                }`}
              >
                <Icon className={`w-4 h-4 ${isActive ? 'text-white' : 'text-slate-500'}`} />
                {tab.label}
              </button>
            );
          })}
        </div>

        {/* Error Display */}
        {error && (
          <div className="mb-6 p-4 bg-rose-500/10 border border-rose-500/20 rounded-2xl text-rose-400 flex items-center gap-3 shadow-lg shadow-rose-500/5">
            <AlertCircle className="w-5 h-5" />
            <span className="font-bold">Error: {error}</span>
          </div>
        )}

        {/* Logs Display */}
        <div className="bg-slate-800/50 backdrop-blur-xl border border-white/10 rounded-[2rem] p-6 shadow-2xl">
          <div className="flex items-center justify-between mb-4 px-2">
            <h2 className="text-sm font-bold text-slate-400 uppercase tracking-wider flex items-center gap-2">
              <div className={`w-2 h-2 rounded-full ${isLoading ? 'bg-blue-500 animate-pulse' : 'bg-emerald-500'}`}></div>
              {logs.length} log entries
            </h2>
            {isLoading && (
              <div className="flex items-center gap-2 text-blue-400 text-xs font-bold bg-blue-500/10 px-3 py-1 rounded-full border border-blue-500/20">
                <RefreshCw className="w-3 h-3 animate-spin" />
                SYNCING...
              </div>
            )}
          </div>

          <div className="bg-slate-950/50 rounded-xl p-4 h-[calc(100vh-380px)] overflow-y-auto font-mono text-sm custom-scrollbar border border-white/5 shadow-inner">
            {logs.length === 0 ? (
              <div className="h-full flex flex-col items-center justify-center text-slate-500 gap-4">
                <Terminal className="w-12 h-12 opacity-20" />
                <p className="font-medium">No logs available for this filter</p>
              </div>
            ) : (
              <div className="space-y-2">
                {logs.map((log, index) => (
                  <div
                    key={index}
                    className={`p-3 rounded-lg border ${getLogColor(log.level)} transition-all`}
                  >
                    <div className="flex items-start gap-3">
                      <span className="text-[10px] text-slate-500 whitespace-nowrap min-w-[140px] font-medium pt-0.5">
                        {formatTimestamp(log.timestamp)}
                      </span>
                      <span className={`text-[10px] uppercase font-bold min-w-[50px] pt-0.5 ${
                        log.level === 'error' ? 'text-rose-400' : 
                        log.level === 'warn' ? 'text-amber-400' : 
                        'text-emerald-400'
                      }`}>
                        {log.level}
                      </span>
                      <span className="text-[10px] text-slate-400 min-w-[60px] font-bold pt-0.5">
                        {log.source}
                      </span>
                      <span className={`flex-1 break-words text-xs leading-relaxed ${getMessageColor(log.message, log.level)}`}>
                        {log.message}
                      </span>
                    </div>
                  </div>
                ))}
                <div ref={logsEndRef} />
              </div>
            )}
          </div>

          {/* Legend */}
          <div className="mt-6 flex items-center gap-6 text-xs font-medium text-slate-500 px-2">
            <div className="flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-emerald-500 shadow-[0_0_8px_rgba(16,185,129,0.5)]"></div>
              Info
            </div>
            <div className="flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.5)]"></div>
              Warning
            </div>
            <div className="flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-rose-500 shadow-[0_0_8px_rgba(244,63,94,0.5)]"></div>
              Error
            </div>
          </div>
        </div>
      </div>

      <style jsx global>{`
        .custom-scrollbar::-webkit-scrollbar {
          width: 6px;
        }
        .custom-scrollbar::-webkit-scrollbar-track {
          background: rgba(15, 23, 42, 0.5);
          border-radius: 4px;
        }
        .custom-scrollbar::-webkit-scrollbar-thumb {
          background: rgba(59, 130, 246, 0.3);
          border-radius: 4px;
        }
        .custom-scrollbar::-webkit-scrollbar-thumb:hover {
          background: rgba(59, 130, 246, 0.5);
        }
      `}</style>
    </main>
  );
}
