'use client';

import { useState, useEffect } from 'react';
import { Database, Server, HardDrive, CheckCircle, XCircle, RefreshCw } from 'lucide-react';

export default function ERPConnectionSettings() {
  const [mode, setMode] = useState<'sqlserver' | 'peachtree'>('sqlserver');
  const [sqlStatus, setSqlStatus] = useState<any>(null);
  const [peachtreeStatus, setPeachtreeStatus] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    checkConnections();
  }, []);

  const checkConnections = async () => {
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
    } catch (error: any) {
      setPeachtreeStatus({ 
        connected: false, 
        message: 'Peachtree bridge server not responding (port 3001)'
      });
    }

    setLoading(false);
  };

  return (
    <div className="p-6 bg-gray-800 rounded-lg border border-gray-700">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-white flex items-center gap-2">
          <Database className="w-6 h-6 text-blue-400" />
          ERP Connection Mode
        </h2>
        <button 
          onClick={checkConnections}
          disabled={loading}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg disabled:bg-gray-600">
          <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
          Refresh
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        {/* SQL Server Status */}
        <div 
          onClick={() => setMode('sqlserver')}
          className={`p-6 rounded-lg border-2 cursor-pointer transition-all ${
            mode === 'sqlserver' 
              ? 'border-blue-500 bg-blue-500/10' 
              : 'border-gray-600 bg-gray-700/50 hover:border-gray-500'
          }`}>
          <div className="flex items-start justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className={`p-3 rounded-lg ${mode === 'sqlserver' ? 'bg-blue-500/20' : 'bg-gray-600/50'}`}>
                <Server className="w-6 h-6 text-blue-400" />
              </div>
              <div>
                <h3 className="text-lg font-bold text-white">SQL Server</h3>
                <p className="text-sm text-gray-400">WIN-JIAVRTFMA0N\SQLEXPRESS</p>
              </div>
            </div>
            {mode === 'sqlserver' && (
              <CheckCircle className="w-6 h-6 text-green-400" />
            )}
          </div>
          
          {sqlStatus && (
            <div className={`flex items-center gap-2 px-3 py-2 rounded-lg ${
              sqlStatus.connected ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
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
            <div className="flex justify-between text-gray-400">
              <span>Database:</span>
              <span className="text-white">LasanthaTire</span>
            </div>
            <div className="flex justify-between text-gray-400">
              <span>Mode:</span>
              <span className="text-white">Direct SQL Connection</span>
            </div>
          </div>
        </div>

        {/* Peachtree Status */}
        <div 
          onClick={() => setMode('peachtree')}
          className={`p-6 rounded-lg border-2 cursor-pointer transition-all ${
            mode === 'peachtree' 
              ? 'border-green-500 bg-green-500/10' 
              : 'border-gray-600 bg-gray-700/50 hover:border-gray-500'
          }`}>
          <div className="flex items-start justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className={`p-3 rounded-lg ${mode === 'peachtree' ? 'bg-green-500/20' : 'bg-gray-600/50'}`}>
                <HardDrive className="w-6 h-6 text-green-400" />
              </div>
              <div>
                <h3 className="text-lg font-bold text-white">Peachtree</h3>
                <p className="text-sm text-gray-400">WIN-JIAVRTFMA0N (ODBC)</p>
              </div>
            </div>
            {mode === 'peachtree' && (
              <CheckCircle className="w-6 h-6 text-green-400" />
            )}
          </div>
          
          {peachtreeStatus && (
            <div className={`flex items-center gap-2 px-3 py-2 rounded-lg ${
              peachtreeStatus.connected ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'
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
            <div className="flex justify-between text-gray-400">
              <span>Tables:</span>
              <span className="text-white">{peachtreeStatus?.tables || 0}</span>
            </div>
            <div className="flex justify-between text-gray-400">
              <span>Mode:</span>
              <span className="text-white">ODBC Bridge (Port 3001)</span>
            </div>
          </div>
        </div>
      </div>

      {/* Instructions */}
      <div className="bg-blue-500/10 border border-blue-500/30 rounded-lg p-4">
        <h3 className="font-bold text-white mb-2">📘 Connection Instructions</h3>
        {mode === 'sqlserver' ? (
          <div className="text-sm text-gray-300 space-y-1">
            <p>✅ Currently using SQL Server (LasanthaTire database)</p>
            <p>• Direct connection to WIN-JIAVRTFMA0N\SQLEXPRESS</p>
            <p>• Uses custom views for WhatsApp data</p>
          </div>
        ) : (
          <div className="text-sm text-gray-300 space-y-2">
            <p>🔧 To connect to Peachtree:</p>
            <ol className="list-decimal ml-4 space-y-1">
              <li>Install 32-bit Node.js if not installed</li>
              <li>Run: <code className="bg-gray-900 px-2 py-1 rounded">start-peachtree-bridge-32bit.bat</code></li>
              <li>Bridge server will start on port 3001</li>
              <li>Dashboard will connect via ODBC to Peachtree on WIN-JIAVRTFMA0N</li>
            </ol>
            {!peachtreeStatus?.connected && (
              <p className="text-yellow-400 mt-2">
                ⚠️ Bridge server not running. Start it with the batch file above.
              </p>
            )}
          </div>
        )}
      </div>

      <div className="mt-6 flex justify-end">
        <button 
          onClick={() => {
            localStorage.setItem('erp_mode', mode);
            window.location.href = '/erp';
          }}
          className="px-6 py-2.5 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white rounded-lg font-semibold">
          Apply & Go to ERP
        </button>
      </div>
    </div>
  );
}
