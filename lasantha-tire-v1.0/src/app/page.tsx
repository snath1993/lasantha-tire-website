'use client';
import { useEffect, useMemo, useState } from 'react';
import { useRouter } from 'next/navigation';
import { ArrowRight, Lock, User } from 'lucide-react';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [checkingAuth, setCheckingAuth] = useState(true);
  const [remember, setRemember] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const sessionId = sessionStorage.getItem('sessionId');
        if (!sessionId) {
          setCheckingAuth(false);
          return;
        }
        const res = await fetch('/api/auth/verify', { 
          cache: 'no-store',
          headers: { 'x-session-id': sessionId }
        });
        if (res.ok) {
          const json = await res.json();
          if (json.authenticated) {
            router.replace('/dashboard');
            return;
          }
        }
        sessionStorage.removeItem('sessionId');
      } catch {
        sessionStorage.removeItem('sessionId');
      } finally {
        setCheckingAuth(false);
      }
    };
    checkAuth();
  }, [router]);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });
      const json = await res.json();
      
      if (res.ok && json?.success && json?.sessionId) {
        sessionStorage.setItem('sessionId', json.sessionId);
        if (remember) localStorage.setItem('dash_last_user', username);
        await new Promise(resolve => setTimeout(resolve, 100));
        router.replace('/dashboard');
        return;
      }
      setError(json?.error || 'Invalid credentials');
    } catch (e: any) {
      setError(e?.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  if (checkingAuth) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-[#f3f4f6]">
        <div className="w-8 h-8 border-2 border-indigo-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#f3f4f6] p-4">
      <div className="w-full max-w-md bg-white rounded-[2rem] shadow-[0_20px_40px_-10px_rgba(0,0,0,0.05)] p-8 md:p-12 border border-zinc-100">
        <div className="text-center mb-10">
          <div className="w-16 h-16 bg-indigo-600 rounded-2xl mx-auto flex items-center justify-center mb-6 shadow-lg shadow-indigo-600/20 text-white">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
              <path d="M2 17L12 22L22 17" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
              <path d="M2 12L12 17L22 12" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-zinc-900 tracking-tight mb-2">Welcome Back</h1>
          <p className="text-zinc-500 text-sm">Sign in to access your dashboard</p>
        </div>

        <form onSubmit={handleLogin} className="space-y-5">
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-zinc-500 uppercase tracking-wider ml-1">Username</label>
            <div className="relative group">
              <User className="absolute left-4 top-1/2 -translate-y-1/2 text-zinc-400 group-focus-within:text-indigo-600 transition-colors" size={18} />
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full pl-11 pr-4 py-3.5 bg-zinc-50 border border-zinc-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-600/20 focus:border-indigo-600 transition-all text-zinc-900 placeholder:text-zinc-400 font-medium"
                placeholder="Enter username"
                required
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-zinc-500 uppercase tracking-wider ml-1">Password</label>
            <div className="relative group">
              <Lock className="absolute left-4 top-1/2 -translate-y-1/2 text-zinc-400 group-focus-within:text-indigo-600 transition-colors" size={18} />
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full pl-11 pr-4 py-3.5 bg-zinc-50 border border-zinc-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-600/20 focus:border-indigo-600 transition-all text-zinc-900 placeholder:text-zinc-400 font-medium"
                placeholder="Enter password"
                required
              />
            </div>
          </div>

          <div className="flex items-center justify-between pt-2">
            <label className="flex items-center gap-2 cursor-pointer">
              <input 
                type="checkbox" 
                checked={remember} 
                onChange={e => setRemember(e.target.checked)}
                className="w-4 h-4 rounded border-zinc-300 text-indigo-600 focus:ring-indigo-600/20"
              />
              <span className="text-sm text-zinc-600 font-medium">Remember me</span>
            </label>
            <a href="#" className="text-sm font-medium text-indigo-600 hover:text-indigo-700">Forgot password?</a>
          </div>

          {error && (
            <div className="p-3 rounded-xl bg-rose-50 border border-rose-100 text-rose-600 text-sm font-medium flex items-center gap-2">
              <div className="w-1.5 h-1.5 rounded-full bg-rose-600" />
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl font-semibold shadow-lg shadow-indigo-600/20 transition-all hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-70 disabled:cursor-not-allowed"
          >
            {loading ? (
              <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            ) : (
              <>
                Sign In <ArrowRight size={18} />
              </>
            )}
          </button>
        </form>
      </div>
    </div>
  );
}
