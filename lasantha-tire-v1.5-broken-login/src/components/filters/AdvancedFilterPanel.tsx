'use client';

import { useState } from 'react';
import { X, Calendar, DollarSign } from 'lucide-react';

interface AdvancedFilterPanelProps {
  onApply: (filters: {
    dateFrom?: string;
    dateTo?: string;
    amountMin?: number;
    amountMax?: number;
    status?: 'all' | 'active' | 'inactive';
    hasBalance?: boolean;
  }) => void;
  onClear: () => void;
  onClose: () => void;
}

export function AdvancedFilterPanel({ onApply, onClear, onClose }: AdvancedFilterPanelProps) {
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');
  const [amountMin, setAmountMin] = useState('');
  const [amountMax, setAmountMax] = useState('');
  const [status, setStatus] = useState<'all' | 'active' | 'inactive'>('all');
  const [hasBalance, setHasBalance] = useState<boolean | undefined>(undefined);

  const handleApply = () => {
    onApply({
      dateFrom: dateFrom || undefined,
      dateTo: dateTo || undefined,
      amountMin: amountMin ? Number(amountMin) : undefined,
      amountMax: amountMax ? Number(amountMax) : undefined,
      status,
      hasBalance
    });
  };

  const handleClear = () => {
    setDateFrom('');
    setDateTo('');
    setAmountMin('');
    setAmountMax('');
    setStatus('all');
    setHasBalance(undefined);
    onClear();
  };

  return (
    <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-bold text-white">Advanced Filters</h3>
        <button
          onClick={onClose}
          className="p-2 hover:bg-slate-700 rounded-lg transition-colors"
          aria-label="Close"
        >
          <X className="w-5 h-5 text-slate-400" />
        </button>
      </div>

      {/* Date Range */}
      <div className="space-y-3">
        <label className="text-sm font-semibold text-slate-300 flex items-center gap-2">
          <Calendar className="w-4 h-4" />
          Date Range
        </label>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-xs text-slate-400 mb-1 block">From</label>
            <input
              type="date"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              className="w-full bg-slate-700/50 text-white text-sm px-3 py-2 rounded-lg border border-slate-600 focus:outline-none focus:border-blue-500"
            />
          </div>
          <div>
            <label className="text-xs text-slate-400 mb-1 block">To</label>
            <input
              type="date"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
              className="w-full bg-slate-700/50 text-white text-sm px-3 py-2 rounded-lg border border-slate-600 focus:outline-none focus:border-blue-500"
            />
          </div>
        </div>
      </div>

      {/* Amount Range */}
      <div className="space-y-3">
        <label className="text-sm font-semibold text-slate-300 flex items-center gap-2">
          <DollarSign className="w-4 h-4" />
          Amount Range (LKR)
        </label>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-xs text-slate-400 mb-1 block">Min</label>
            <input
              type="number"
              value={amountMin}
              onChange={(e) => setAmountMin(e.target.value)}
              placeholder="0"
              className="w-full bg-slate-700/50 text-white text-sm px-3 py-2 rounded-lg border border-slate-600 focus:outline-none focus:border-blue-500"
            />
          </div>
          <div>
            <label className="text-xs text-slate-400 mb-1 block">Max</label>
            <input
              type="number"
              value={amountMax}
              onChange={(e) => setAmountMax(e.target.value)}
              placeholder="∞"
              className="w-full bg-slate-700/50 text-white text-sm px-3 py-2 rounded-lg border border-slate-600 focus:outline-none focus:border-blue-500"
            />
          </div>
        </div>
      </div>

      {/* Status */}
      <div className="space-y-3">
        <label className="text-sm font-semibold text-slate-300">Status</label>
        <div className="flex gap-2">
          {(['all', 'active', 'inactive'] as const).map((s) => (
            <button
              key={s}
              onClick={() => setStatus(s)}
              className={`flex-1 px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                status === s
                  ? 'bg-blue-500 text-white'
                  : 'bg-slate-700/50 text-slate-300 hover:bg-slate-700'
              }`}
            >
              {s === 'all' ? 'All' : s.charAt(0).toUpperCase() + s.slice(1)}
            </button>
          ))}
        </div>
      </div>

      {/* Has Balance */}
      <div className="space-y-3">
        <label className="text-sm font-semibold text-slate-300">Balance Filter</label>
        <div className="flex gap-2">
          {[
            { label: 'All', value: undefined },
            { label: 'With Balance', value: true },
            { label: 'Zero Balance', value: false }
          ].map((option) => (
            <button
              key={String(option.value)}
              onClick={() => setHasBalance(option.value)}
              className={`flex-1 px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                hasBalance === option.value
                  ? 'bg-blue-500 text-white'
                  : 'bg-slate-700/50 text-slate-300 hover:bg-slate-700'
              }`}
            >
              {option.label}
            </button>
          ))}
        </div>
      </div>

      {/* Actions */}
      <div className="flex gap-3 pt-4 border-t border-slate-700">
        <button
          onClick={handleClear}
          className="flex-1 px-4 py-2.5 bg-slate-700/50 hover:bg-slate-700 text-white rounded-xl transition-colors font-medium"
        >
          Clear All
        </button>
        <button
          onClick={handleApply}
          className="flex-1 px-4 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-xl transition-colors font-medium shadow-lg shadow-blue-600/30"
        >
          Apply Filters
        </button>
      </div>
    </div>
  );
}
