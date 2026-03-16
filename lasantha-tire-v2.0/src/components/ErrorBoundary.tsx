'use client';

import React, { Component, type ReactNode } from 'react';
import { AlertTriangle, RotateCcw, Home } from 'lucide-react';

interface ErrorBoundaryProps {
  children: ReactNode;
  /** Optional fallback component — if omitted, the default error UI is shown */
  fallback?: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
}

/**
 * App-wide error boundary that prevents full white-screen crashes.
 * Wraps the main layout to catch render-time exceptions.
 */
export default class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    // Log to console in dev, could be sent to a monitoring service in prod
    console.error('[ErrorBoundary] Caught:', error, info.componentStack);
  }

  handleReset = () => {
    this.setState({ hasError: false, error: null });
  };

  handleGoHome = () => {
    this.setState({ hasError: false, error: null });
    window.location.href = '/';
  };

  render() {
    if (this.state.hasError) {
      if (this.props.fallback) return this.props.fallback;

      return (
        <div className="min-h-screen bg-slate-950 flex items-center justify-center p-6">
          <div className="max-w-md w-full bg-slate-900/80 backdrop-blur-xl border border-red-800/40 rounded-2xl p-8 text-center space-y-5">
            {/* Icon */}
            <div className="mx-auto w-16 h-16 rounded-full bg-red-500/20 flex items-center justify-center">
              <AlertTriangle className="w-8 h-8 text-red-400" />
            </div>

            {/* Title */}
            <h2 className="text-xl font-bold text-white">Something Went Wrong</h2>
            <p className="text-sm text-slate-400 leading-relaxed">
              An unexpected error occurred. Your data is safe — try refreshing or going back to the home screen.
            </p>

            {/* Error details (dev only) */}
            {process.env.NODE_ENV !== 'production' && this.state.error && (
              <details className="text-left bg-slate-800/50 rounded-xl p-3 border border-slate-700">
                <summary className="text-xs text-red-400 cursor-pointer font-mono">
                  Error Details
                </summary>
                <pre className="mt-2 text-[10px] text-slate-500 overflow-auto max-h-40 whitespace-pre-wrap font-mono">
                  {this.state.error.message}
                  {'\n\n'}
                  {this.state.error.stack}
                </pre>
              </details>
            )}

            {/* Action Buttons */}
            <div className="flex gap-3 pt-2">
              <button
                onClick={this.handleReset}
                className="flex-1 flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-xl transition-colors active:scale-95"
                aria-label="Try again"
              >
                <RotateCcw className="w-4 h-4" />
                Try Again
              </button>
              <button
                onClick={this.handleGoHome}
                className="flex-1 flex items-center justify-center gap-2 bg-slate-800 hover:bg-slate-700 text-slate-300 font-semibold py-3 px-4 rounded-xl transition-colors active:scale-95 border border-slate-700"
                aria-label="Go to home"
              >
                <Home className="w-4 h-4" />
                Home
              </button>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
