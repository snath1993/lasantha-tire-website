'use client';

import { useEffect, useRef } from 'react';
import { AlertTriangle, Info } from 'lucide-react';

export interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  variant?: 'danger' | 'info';
  onConfirm: () => void;
  onCancel: () => void;
}

/**
 * Mobile-friendly confirmation dialog — replaces window.confirm().
 * Includes backdrop blur, proper touch targets, and accessibility.
 */
export default function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  variant = 'danger',
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  const confirmRef = useRef<HTMLButtonElement>(null);

  // Focus confirm button when opened
  useEffect(() => {
    if (open) {
      setTimeout(() => confirmRef.current?.focus(), 100);
    }
  }, [open]);

  // Block background scroll
  useEffect(() => {
    if (!open) return;
    const prev = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => { document.body.style.overflow = prev; };
  }, [open]);

  if (!open) return null;

  const isDanger = variant === 'danger';

  return (
    <div
      className="fixed inset-0 z-[9999] flex items-center justify-center p-4"
      role="dialog"
      aria-modal="true"
      aria-labelledby="confirm-title"
      aria-describedby="confirm-message"
    >
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onCancel}
        aria-hidden="true"
      />

      {/* Dialog */}
      <div className="relative bg-slate-900 border border-slate-700 rounded-2xl p-6 max-w-sm w-full shadow-2xl animate-in fade-in zoom-in-95 duration-200">
        {/* Icon */}
        <div className={`mx-auto mb-4 w-12 h-12 rounded-full flex items-center justify-center ${
          isDanger ? 'bg-red-500/20' : 'bg-blue-500/20'
        }`}>
          {isDanger
            ? <AlertTriangle className="w-6 h-6 text-red-400" />
            : <Info className="w-6 h-6 text-blue-400" />
          }
        </div>

        <h3 id="confirm-title" className="text-lg font-bold text-white text-center mb-2">
          {title}
        </h3>
        <p id="confirm-message" className="text-sm text-slate-400 text-center mb-6 leading-relaxed">
          {message}
        </p>

        {/* Buttons — min 48px touch target */}
        <div className="flex gap-3">
          <button
            onClick={onCancel}
            className="flex-1 min-h-[48px] bg-slate-800 hover:bg-slate-700 text-slate-300 font-semibold rounded-xl transition-colors active:scale-95 border border-slate-600"
            aria-label={cancelLabel}
          >
            {cancelLabel}
          </button>
          <button
            ref={confirmRef}
            onClick={onConfirm}
            className={`flex-1 min-h-[48px] font-semibold rounded-xl transition-colors active:scale-95 ${
              isDanger
                ? 'bg-red-600 hover:bg-red-700 text-white'
                : 'bg-blue-600 hover:bg-blue-700 text-white'
            }`}
            aria-label={confirmLabel}
          >
            {confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
