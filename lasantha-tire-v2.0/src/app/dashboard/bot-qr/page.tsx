"use client";

import { useEffect, useRef, useState } from "react";
import Link from "next/link";
import { Check, X, Bot, RefreshCw, LogOut, QrCode, AlertCircle } from "lucide-react";

interface Status {
  isConnected: boolean;
  phoneNumber: string | null;
  status: string;
}

export default function BotQrPage() {
  const [status, setStatus] = useState<Status>({ isConnected: false, phoneNumber: null, status: "offline" });
  const [qr, setQr] = useState<string | null>(null);
  const [connecting, setConnecting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const eventSrcRef = useRef<EventSource | null>(null);

  const fetchStatus = async () => {
    try {
      const res = await fetch("/api/whatsapp/status");
      const data = await res.json();
      if (!res.ok) {
        setError((data && (data.error || data.message)) || 'Failed to fetch status');
      }

      // Normalize response shape (older/newer APIs may return different keys)
      const isConnected = !!(data?.isConnected ?? data?.isReady ?? data?.details?.isConnected);
      const phoneNumber = (data?.phoneNumber ?? data?.details?.phoneNumber ?? null) as string | null;
      const normalizedStatus = (data?.status ?? data?.details?.status ?? (isConnected ? 'connected' : 'offline')) as string;

      setStatus({ isConnected, phoneNumber, status: normalizedStatus });
    } catch (e: any) {
      setError(e?.message || "Failed to fetch status");
    }
  };

  const fetchQrFallback = async () => {
    try {
      const res = await fetch("/api/whatsapp/qr");
      const data = await res.json();
      const code = data?.qr || data?.qrCode;
      if (data?.ok && code) setQr(code);
    } catch {}
  };

  useEffect(() => {
    fetchStatus();
    // Open SSE stream
    const es = new EventSource("/api/sse");
    eventSrcRef.current = es;

    es.addEventListener("qr", (evt: MessageEvent) => {
      try {
        const payload = JSON.parse(evt.data);
        setQr(payload.qr || null);
      } catch {}
    });

    es.addEventListener("authenticated", () => {
      setQr(null);
      fetchStatus();
    });

    es.addEventListener("ready", () => {
      setQr(null);
      fetchStatus();
    });

    es.addEventListener("disconnected", () => {
      fetchStatus();
      // Try to get a fresh QR
      setTimeout(fetchQrFallback, 300);
    });

    es.onerror = () => {
      // Fallback polling when SSE breaks
      setTimeout(fetchQrFallback, 500);
    };

    return () => {
      es.close();
    };
  }, []);

  const ADMIN_TOKEN = process.env.NEXT_PUBLIC_ADMIN_TOKEN || '';

  const startLogin = async () => {
    setConnecting(true);
    setError(null);
    try {
      const res = await fetch("/api/whatsapp/login", { 
        method: "POST",
        headers: ADMIN_TOKEN ? { 'x-admin-token': ADMIN_TOKEN } : undefined
      });
      const data = await res.json();
      if (!data?.success) {
        setError(data?.error || "Login request failed");
      } else {
        // QR will arrive via SSE; fallback fetch
        setTimeout(fetchQrFallback, 800);
      }
    } catch (e: any) {
      setError(e?.message || "Login failed");
    } finally {
      setConnecting(false);
    }
  };

  const doLogout = async () => {
    setError(null);
    try {
      const res = await fetch("/api/whatsapp/logout", { 
        method: "POST",
        headers: ADMIN_TOKEN ? { 'x-admin-token': ADMIN_TOKEN } : undefined
      });
      const data = await res.json();
      if (!data?.success) {
        setError(data?.error || "Logout failed");
      }
      setTimeout(() => { fetchStatus(); fetchQrFallback(); }, 500);
    } catch (e: any) {
      setError(e?.message || "Logout failed");
    }
  };

  return (
    <div className="min-h-[calc(100vh-80px)] p-4">
      <div className="max-w-md mx-auto bg-slate-800/50 border border-slate-700 rounded-2xl p-5 space-y-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="w-10 h-10 rounded-xl bg-slate-700 flex items-center justify-center">
              <Bot className="w-6 h-6 text-slate-300" />
            </div>
            <div>
              <h2 className="text-lg font-bold text-white">Bot QR Login</h2>
              <p className="text-xs text-slate-400">WhatsApp SQL API</p>
            </div>
          </div>
          <Link href="/dashboard" className="text-slate-400 hover:text-white">
            <X className="w-5 h-5" />
          </Link>
        </div>

        {/* Status */}
        <div className="flex items-center justify-between bg-slate-900/50 border border-slate-700 rounded-xl p-3">
          <div className="flex items-center gap-2">
            <div className={`w-2 h-2 rounded-full ${status.isConnected ? "bg-emerald-400" : "bg-red-400"}`} />
            <span className="text-sm text-slate-300">
              {status.isConnected ? "Connected" : "Disconnected"}
            </span>
          </div>
          {status.isConnected && (
            <span className="text-xs text-slate-400 font-mono">{status.phoneNumber || "Unknown"}</span>
          )}
        </div>

        {/* Actions */}
        <div className="flex items-center gap-2">
          {!status.isConnected ? (
            <button
              onClick={startLogin}
              disabled={connecting}
              className="flex-1 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl flex items-center justify-center gap-2"
            >
              <QrCode className="w-4 h-4" />
              {connecting ? "Generating…" : "Login (Generate QR)"}
            </button>
          ) : (
            <button
              onClick={doLogout}
              className="flex-1 py-2 bg-red-600 hover:bg-red-500 text-white rounded-xl flex items-center justify-center gap-2"
            >
              <LogOut className="w-4 h-4" />
              Logout
            </button>
          )}
          <button
            onClick={() => { fetchStatus(); fetchQrFallback(); }}
            className="px-3 py-2 bg-slate-800 hover:bg-slate-700 border border-slate-700 rounded-xl text-slate-300"
          >
            <RefreshCw className="w-4 h-4" />
          </button>
        </div>

        {/* QR Display */}
        {!status.isConnected && (
          <div className="mt-2 bg-slate-900/50 border border-slate-700 rounded-xl p-4 flex flex-col items-center gap-2">
            {qr ? (
              <img
                alt="WhatsApp QR"
                src={`https://api.qrserver.com/v1/create-qr-code/?size=240x240&data=${encodeURIComponent(qr)}`}
                className="w-48 h-48 rounded"
              />
            ) : (
              <div className="flex items-center gap-2 text-slate-400 text-sm">
                <AlertCircle className="w-4 h-4" />
                QR not available yet. Click Login to generate.
              </div>
            )}
            <p className="text-[11px] text-slate-500">Scan with WhatsApp to login the bot.</p>
          </div>
        )}

        {/* Error */}
        {error && (
          <div className="text-red-400 text-sm flex items-center gap-2">
            <AlertCircle className="w-4 h-4" />
            {error}
          </div>
        )}
      </div>
    </div>
  );
}
