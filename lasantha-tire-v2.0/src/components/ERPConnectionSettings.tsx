'use client';

import { useState, useEffect, useCallback } from 'react';
import { Database, Server, HardDrive, CheckCircle, XCircle, RefreshCw } from 'lucide-react';

type ConnectionStatus = {
  connected: boolean;
  message: string;
  tables?: number;
};

export default function ERPConnectionSettings() {
  const [mode, setMode] = useState<'sqlserver' | 'peachtree'>('sqlserver');
  const [sqlStatus, setSqlStatus] = useState<ConnectionStatus | null>(null);
  const [peachtreeStatus, setPeachtreeStatus] = useState<ConnectionStatus | null>(null);
  const [loading, setLoading] = useState(false);

  const checkConnections = useCallback(async () => {
    setLoading(true);
    
    // Check SQL Server
    try {
      const sqlRes = await fetch('/api/erp/inventory?type=summary');
      if (sqlRes.ok) {
        setSqlStatus({ connected: true, message: 'Connected to SQL Server' });
      } else {
        setSqlStatus({ connected: false, message: 'SQL Server unavailable' });
      }
    } catch {
      setSqlStatus({ connected: false, message: 'SQL Server unreachable' });
    }

    // Check Peachtree
    try {
      const ptRes = await fetch('/api/erp/peachtree?action=info');
      const ptData = await ptRes.json();
      if (ptData.connected) {
        setPeachtreeStatus({ 
          connected: true, 
          message: `Connected to ${ptData.database} on ${ptData.peachtreeServer}`,
          tables: ptData.tables?.count || 0
        });
      } else {
        setPeachtreeStatus({ 
          connected: false, 
          message: ptData.error || 'Peachtree bridge not running'
        });
      }
    } catch {
      setPeachtreeStatus({ 
        connected: false, 
        message: 'Peachtree bridge server not responding'
      });
    }

    setLoading(false);
  }, []);

  useEffect(() => {
    const timeout = setTimeout(() => {
      void checkConnections();
    }, 0);

    return () => clearTimeout(timeout);
  }, [checkConnections]);

  return (
    <div className="p-6 bg-slate-800/50 backdrop-blur-xl rounded-[2rem] border border-white/10 shadow-xl">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-white flex items-center gap-2">
          <Database className="w-6 h-6 text-blue-400" />
          ERP Connection Mode
        </h2>
        <button 
          onClick={checkConnections}
          disabled={loading}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold shadow-lg shadow-blue-600/20 transition-all disabled:bg-slate-600 disabled:shadow-none">
          <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
          Refresh
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        {/* SQL Server Status */}
        <div 
          onClick={() => setMode('sqlserver')}
          className={`p-6 rounded-2xl border-2 cursor-pointer transition-all ${
            mode === 'sqlserver' 
              ? 'border-blue-500 bg-blue-500/10 shadow-lg shadow-blue-500/10' 
              : 'border-white/5 bg-slate-800/50 hover:border-white/20'
          }`}>
          <div className="flex items-start justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className={`p-3 rounded-xl ${mode === 'sqlserver' ? 'bg-blue-500/20' : 'bg-slate-700/50'}`}>
                <Server className="w-6 h-6 text-blue-400" />
              </div>
              <div>
                <h3 className="text-lg font-bold text-white">SQL Server</h3>
                <p className="text-sm text-slate-400">WIN-JIAVRTFMA0N\SQLEXPRESS</p>
              </div>
            </div>
            {mode === 'sqlserver' && (
              <CheckCircle className="w-6 h-6 text-emerald-400" />
            )}
          </div>
          
          {sqlStatus && (
            <div className={`flex items-center gap-2 px-3 py-2 rounded-xl text-sm font-medium ${
              sqlStatus.connected ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' : 'bg-rose-500/10 text-rose-400 border border-rose-500/20'
            }`}>
              {sqlStatus.connected ? (
                <CheckCircle className="w-4 h-4" />
              ) : (
                <XCircle className="w-4 h-4" />
              )}
              <span className="text-sm">{sqlStatus.message}</span>
            </div>
          )}
          
          <div className="mt-4 space-y-2 text-sm">
            <div className="flex justify-between text-slate-400">
              <span>Database:</span>
              <span className="text-white">LasanthaTire</span>
            </div>
            <div className="flex justify-between text-slate-400">
              <span>Mode:</span>
              <span className="text-white">Direct SQL Connection</span>
            </div>
          </div>
        </div>

        {/* Peachtree Status */}
        <div 
          onClick={() => setMode('peachtree')}
          className={`p-6 rounded-2xl border-2 cursor-pointer transition-all ${
            mode === 'peachtree' 
              ? 'border-emerald-500 bg-emerald-500/10 shadow-lg shadow-emerald-500/10' 
              : 'border-white/5 bg-slate-800/50 hover:border-white/20'
          }`}>
          <div className="flex items-start justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className={`p-3 rounded-xl ${mode === 'peachtree' ? 'bg-emerald-500/20' : 'bg-slate-700/50'}`}>
                <HardDrive className="w-6 h-6 text-emerald-400" />
              </div>
              <div>
                <h3 className="text-lg font-bold text-white">Peachtree</h3>
                <p className="text-sm text-slate-400">WIN-JIAVRTFMA0N (ODBC)</p>
              </div>
            </div>
            {mode === 'peachtree' && (
              <CheckCircle className="w-6 h-6 text-emerald-400" />
            )}
          </div>
          
          {peachtreeStatus && (
            <div className={`flex items-center gap-2 px-3 py-2 rounded-xl text-sm font-medium ${
              peachtreeStatus.connected ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' : 'bg-rose-500/10 text-rose-400 border border-rose-500/20'
            }`}>
              {peachtreeStatus.connected ? (
                <CheckCircle className="w-4 h-4" />
              ) : (
                <XCircle className="w-4 h-4" />
              )}
              <span className="text-sm">{peachtreeStatus.message}</span>
            </div>
          )}
          
          <div className="mt-4 space-y-2 text-sm">
            <div className="flex justify-between text-slate-400">
              <span>Tables:</span>
              <span className="text-white">{peachtreeStatus?.tables || 0}</span>
            </div>
            <div className="flex justify-between text-slate-400">
              <span>Mode:</span>
              <span className="text-white">ODBC Bridge</span>
            </div>
          </div>
        </div>
      </div>

      {/* Instructions */}
      <div className="bg-blue-500/10 border border-blue-500/30 rounded-2xl p-6">
        <h3 className="font-bold text-white mb-4 flex items-center gap-2">
          <Database className="w-5 h-5 text-blue-400" />
          Connection Instructions
        </h3>
        {mode === 'sqlserver' ? (
          <div className="text-sm text-slate-300 space-y-2">
            <p className="flex items-center gap-2"><CheckCircle className="w-4 h-4 text-emerald-500" /> Currently using SQL Server (LasanthaTire database)</p>
            <p className="pl-6">• Direct connection to WIN-JIAVRTFMA0N\SQLEXPRESS</p>
            <p className="pl-6">• Uses custom views for WhatsApp data</p>
          </div>
        ) : (
          <div className="text-sm text-slate-300 space-y-3">
            <p>🔧 To connect to Peachtree:</p>
            <ol className="list-decimal ml-4 space-y-2 text-slate-400">
              <li>Install 32-bit Node.js if not installed</li>
              <li>Run: <code className="bg-slate-900 px-2 py-1 rounded text-white font-mono">start-peachtree-bridge-32bit.bat</code></li>
              <li>Bridge server will start automatically</li>
              <li>Dashboard will connect via ODBC to Peachtree on WIN-JIAVRTFMA0N</li>
            </ol>
            {!peachtreeStatus?.connected && (
              <div className="flex items-center gap-2 text-amber-400 bg-amber-500/10 p-3 rounded-xl border border-amber-500/20 mt-4">
                <RefreshCw className="w-4 h-4" />
                <p className="font-medium">Bridge server not running. Start it with the batch file above.</p>
              </div>
            )}
          </div>
        )}
      </div>

      <div className="mt-6 flex justify-end">
        <button 
          onClick={() => {
            localStorage.setItem('erp_mode', mode);
            window.location.href = '/dashboard/erp';
          }}
          className="px-8 py-3 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white rounded-xl font-bold shadow-lg shadow-blue-600/20 transition-all transform hover:scale-105">
          Apply & Go to ERP
        </button>
      </div>
    </div>
  );
}
