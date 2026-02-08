'use client';
import { useEffect, useMemo, useState } from 'react';
import { useRouter } from 'next/navigation';
import Image from 'next/image';
import { ArrowRight, Lock, User, ScanFace } from 'lucide-react';
import { startAuthentication } from '@simplewebauthn/browser';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [checkingAuth, setCheckingAuth] = useState(true);
  const [remember, setRemember] = useState(false);
  
  // Forgot Password State
  const [showForgot, setShowForgot] = useState(false);
  const [forgotStep, setForgotStep] = useState<'username' | 'otp'>('username');
  const [forgotUsername, setForgotUsername] = useState('');
  const [forgotOtp, setForgotOtp] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [forgotLoading, setForgotLoading] = useState(false);
  const [forgotError, setForgotError] = useState('');
  const [forgotSuccess, setForgotSuccess] = useState('');

  const router = useRouter();

  const handleSendOTP = async (e: React.FormEvent) => {
    e.preventDefault();
    setForgotError('');
    setForgotSuccess('');
    setForgotLoading(true);
    try {
      const res = await fetch('/api/auth/forgot-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username: forgotUsername })
      });
      const data = await res.json();
      if (res.ok && data.success) {
        setForgotSuccess(data.message);
        setForgotStep('otp');
      } else {
        setForgotError(data.error || data.message || 'Failed to send OTP');
      }
    } catch (err: any) {
      setForgotError(err.message || 'Error sending OTP');
    } finally {
      setForgotLoading(false);
    }
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setForgotError('');
    setForgotSuccess('');
    setForgotLoading(true);
    try {
      const res = await fetch('/api/auth/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username: forgotUsername, otp: forgotOtp, newPassword })
      });
      const data = await res.json();
      if (res.ok && data.success) {
        setForgotSuccess('Password reset successfully! You can now login.');
        setTimeout(() => {
          setShowForgot(false);
          setForgotStep('username');
          setForgotUsername('');
          setForgotOtp('');
          setNewPassword('');
          setForgotSuccess('');
        }, 2000);
      } else {
        setForgotError(data.error || 'Failed to reset password');
      }
    } catch (err: any) {
      setForgotError(err.message || 'Error resetting password');
    } finally {
      setForgotLoading(false);
    }
  };

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const sessionId = sessionStorage.getItem('sessionId') || localStorage.getItem('sessionId');
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
            // Ensure session is in the right place based on storage
            if (localStorage.getItem('sessionId')) {
              setRemember(true);
            }
            router.replace('/dashboard');
            return;
          }
        }
        // Clear invalid sessions
        sessionStorage.removeItem('sessionId');
        localStorage.removeItem('sessionId');
      } catch {
        sessionStorage.removeItem('sessionId');
        localStorage.removeItem('sessionId');
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
        if (remember) {
          localStorage.setItem('sessionId', json.sessionId);
          localStorage.setItem('dash_last_user', username);
        } else {
          sessionStorage.setItem('sessionId', json.sessionId);
          localStorage.removeItem('sessionId'); // Clear any old persistent session
        }
        
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
      <div className="min-h-screen flex items-center justify-center bg-slate-900">
        <div className="w-8 h-8 border-2 border-blue-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-4">
      <div className="w-full max-w-md bg-slate-900/50 backdrop-blur-xl rounded-[2rem] shadow-2xl shadow-black/20 p-8 md:p-12 border border-white/10">
        <div className="text-center mb-10">
          <div className="w-16 h-16 bg-white rounded-2xl mx-auto flex items-center justify-center mb-6 shadow-lg shadow-blue-600/20 overflow-hidden">
            <Image 
              src="/shop-logo.jpg" 
              alt="Lasantha Tire Service" 
              width={64} 
              height={64} 
              className="w-full h-full object-cover"
            />
          </div>
          <h1 className="text-3xl font-bold text-white mb-2 tracking-tight">Welcome Back</h1>
          <p className="text-slate-400 text-sm">Sign in to access your dashboard</p>
        </div>

        <form onSubmit={handleLogin} className="space-y-5">
          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-400 uppercase tracking-wider ml-1">Username</label>
            <div className="relative group">
              <User className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-blue-500 transition-colors" size={18} />
              <input
                type="text"
                name="username"
                autoComplete="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full pl-11 pr-4 py-3.5 bg-slate-800/50 border border-white/10 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-white placeholder:text-slate-500 font-medium"
                placeholder="Enter username"
                required
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-semibold text-slate-400 uppercase tracking-wider ml-1">Password</label>
            <div className="relative group">
              <Lock className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-blue-500 transition-colors" size={18} />
              <input
                type="password"
                name="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full pl-11 pr-4 py-3.5 bg-slate-800/50 border border-white/10 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-white placeholder:text-slate-500 font-medium"
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
                className="w-4 h-4 rounded border-slate-600 bg-slate-800 text-blue-600 focus:ring-blue-600/20"
              />
              <span className="text-sm text-slate-400 font-medium">Remember me</span>
            </label>
            <button type="button" onClick={() => setShowForgot(true)} className="text-sm font-medium text-blue-500 hover:text-blue-400">Forgot password?</button>
          </div>

          {error && (
            <div className="p-3 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-400 text-sm font-medium flex items-center gap-2">
              <div className="w-1.5 h-1.5 rounded-full bg-rose-500" />
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3.5 bg-blue-600 hover:bg-blue-500 text-white rounded-xl font-semibold shadow-lg shadow-blue-600/20 transition-all hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-70 disabled:cursor-not-allowed"
          >
            {loading ? (
              <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            ) : (
              <>
                Sign In <ArrowRight size={18} />
              </>
            )}
          </button>

          <div className="relative flex py-1 items-center">
            <div className="flex-grow border-t border-slate-700"></div>
            <span className="flex-shrink-0 mx-4 text-slate-500 text-[10px] uppercase font-bold tracking-wider">Or continue with</span>
            <div className="flex-grow border-t border-slate-700"></div>
          </div>

          <button
            type="button"
            onClick={async () => {
              try {
                setLoading(true);
                // 1. Get options from server
                const resp = await fetch('/api/auth/webauthn/authenticate/generate-options');
                const options = await resp.json();

                // 2. Pass options to browser authenticator
                const asseResp = await startAuthentication(options);

                // 3. Verify response with server
                const verificationResp = await fetch('/api/auth/webauthn/authenticate/verify', {
                  method: 'POST',
                  headers: { 'Content-Type': 'application/json' },
                  body: JSON.stringify(asseResp),
                });

                const verificationJSON = await verificationResp.json();

                if (verificationJSON && verificationJSON.verified) {
                  sessionStorage.setItem('sessionId', verificationJSON.sessionId);
                  router.replace('/dashboard');
                } else {
                  setError('Face ID authentication failed');
                }
              } catch (error: any) {
                console.error(error);
                if (error.name === 'NotAllowedError') {
                  setError('Login cancelled or not allowed. If using an in-app browser (WhatsApp/FB), please open in Safari/Chrome.');
                } else {
                  setError(error.message || 'Face ID failed');
                }
              } finally {
                setLoading(false);
              }
            }}
            className="w-full py-3.5 bg-slate-800 hover:bg-slate-700 text-white rounded-xl font-semibold border border-slate-700 transition-all hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 group"
          >
            <ScanFace size={20} className="text-blue-500 group-hover:text-blue-400 transition-colors" />
            <span>Login with Face ID</span>
          </button>
        </form>
      </div>

      {/* Forgot Password Modal */}
      {showForgot && (
        <div className="fixed inset-0 bg-slate-900/90 backdrop-blur-sm flex items-center justify-center z-50 p-4 animate-in fade-in duration-200">
          <div className="bg-slate-800 rounded-[2rem] p-8 max-w-md w-full shadow-2xl border border-white/10 relative">
            <button 
              onClick={() => {
                setShowForgot(false);
                setForgotStep('username');
                setForgotError('');
                setForgotSuccess('');
              }}
              className="absolute top-4 right-4 text-slate-400 hover:text-white transition-colors"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><path d="m15 9-6 6"/><path d="m9 9 6 6"/></svg>
            </button>

            <div className="text-center mb-8">
              <div className="w-12 h-12 bg-blue-500/10 rounded-2xl mx-auto flex items-center justify-center mb-4">
                <Lock className="w-6 h-6 text-blue-500" />
              </div>
              <h2 className="text-2xl font-bold text-white mb-2">Reset Password</h2>
              <p className="text-slate-400 text-sm">
                {forgotStep === 'username' 
                  ? 'Enter your username to receive a reset code via WhatsApp.' 
                  : 'Enter the code sent to your WhatsApp and your new password.'}
              </p>
            </div>

            {forgotStep === 'username' ? (
              <form onSubmit={handleSendOTP} className="space-y-4">
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-400 uppercase tracking-wider ml-1">Username</label>
                  <input
                    type="text"
                    value={forgotUsername}
                    onChange={(e) => setForgotUsername(e.target.value)}
                    className="w-full px-4 py-3 bg-slate-900/50 border border-white/10 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-white placeholder:text-slate-500"
                    placeholder="Enter your username"
                    required
                  />
                </div>

                {forgotError && (
                  <div className="p-3 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-400 text-sm font-medium text-center">
                    {forgotError}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={forgotLoading}
                  className="w-full py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-xl font-semibold shadow-lg shadow-blue-600/20 transition-all hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-70"
                >
                  {forgotLoading ? 'Sending...' : 'Send Reset Code'}
                </button>
              </form>
            ) : (
              <form onSubmit={handleResetPassword} className="space-y-4">
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-400 uppercase tracking-wider ml-1">WhatsApp Code</label>
                  <input
                    type="text"
                    value={forgotOtp}
                    onChange={(e) => setForgotOtp(e.target.value)}
                    className="w-full px-4 py-3 bg-slate-900/50 border border-white/10 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-white placeholder:text-slate-500 text-center tracking-widest text-lg font-mono"
                    placeholder="000000"
                    maxLength={6}
                    required
                  />
                </div>

                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-400 uppercase tracking-wider ml-1">New Password</label>
                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    className="w-full px-4 py-3 bg-slate-900/50 border border-white/10 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all text-white placeholder:text-slate-500"
                    placeholder="Min 8 characters"
                    minLength={8}
                    required
                  />
                </div>

                {forgotError && (
                  <div className="p-3 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-400 text-sm font-medium text-center">
                    {forgotError}
                  </div>
                )}
                
                {forgotSuccess && (
                  <div className="p-3 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-400 text-sm font-medium text-center">
                    {forgotSuccess}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={forgotLoading}
                  className="w-full py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-xl font-semibold shadow-lg shadow-blue-600/20 transition-all hover:scale-[1.02] active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-70"
                >
                  {forgotLoading ? 'Resetting...' : 'Reset Password'}
                </button>
              </form>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
