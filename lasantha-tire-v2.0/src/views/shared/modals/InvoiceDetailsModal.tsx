'use client';

import { X, Calendar, DollarSign, FileText, Loader2 } from 'lucide-react';
import { useEffect, useState } from 'react';

interface InvoiceDetailsModalProps {
  entity: {
    id: string;
    name: string;
    type: 'customer' | 'vendor';
    balance: number;
  };
  onClose: () => void;
}

interface Invoice {
  InvoiceID: string;
  Date: string;
  Type: string;
  Description: string;
  Amount: number;
  Status: string;
  DueDate?: string;
}

export function InvoiceDetailsModal({ entity, onClose }: InvoiceDetailsModalProps) {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchInvoices = async () => {
      try {
        setLoading(true);
        const param = entity.type === 'customer' ? 'customer_id' : 'vendor_id';
        const res = await fetch(`/api/erp/peachtree?endpoint=transactions&${param}=${entity.id}`);
        const data = await res.json();
        
        if (data.success) {
          setInvoices(data.data || []);
        } else {
          setError(data.error || 'Failed to load invoices');
        }
      } catch (err: any) {
        setError(err.message || 'Network error');
      } finally {
        setLoading(false);
      }
    };

    fetchInvoices();
  }, [entity.id, entity.type]);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-LK', {
      style: 'currency',
      currency: 'LKR',
      minimumFractionDigits: 0
    }).format(amount);
  };

  const formatDate = (dateStr: string) => {
    if (!dateStr || dateStr === '-') return '-';
    try {
      return new Date(dateStr).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch {
      return dateStr;
    }
  };

  return (
    <div className="fixed inset-0 z-[999] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-slate-700">
          <div>
            <h2 className="text-2xl font-bold text-white flex items-center gap-2">
              <FileText className="w-6 h-6 text-blue-400" />
              Invoice Details
            </h2>
            <p className="text-slate-400 mt-1">
              {entity.name} • {formatCurrency(entity.balance)} Outstanding
            </p>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-slate-800 rounded-lg transition-colors"
            aria-label="Close"
          >
            <X className="w-6 h-6 text-slate-400" />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          {loading && (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="w-8 h-8 text-blue-400 animate-spin" />
            </div>
          )}

          {error && (
            <div className="bg-red-500/10 border border-red-500/30 rounded-xl p-4 text-red-400">
              {error}
            </div>
          )}

          {!loading && !error && invoices.length === 0 && (
            <div className="text-center py-12">
              <FileText className="w-16 h-16 text-slate-600 mx-auto mb-4" />
              <p className="text-slate-400">No invoices found</p>
            </div>
          )}

          {!loading && !error && invoices.length > 0 && (
            <div className="space-y-3">
              {invoices.map((invoice, idx) => (
                <div
                  key={idx}
                  className="bg-slate-800/50 border border-slate-700 rounded-xl p-4 hover:border-slate-600 transition-colors"
                >
                  <div className="flex items-start justify-between mb-3">
                    <div>
                      <p className="text-white font-semibold">{invoice.InvoiceID}</p>
                      <p className="text-sm text-slate-400 mt-1">{invoice.Description}</p>
                    </div>
                    <div className="text-right">
                      <p className={`text-lg font-bold ${invoice.Amount < 0 ? 'text-green-400' : 'text-blue-400'}`}>
                        {formatCurrency(Math.abs(invoice.Amount))}
                      </p>
                      <span className={`text-xs px-2 py-1 rounded-full ${
                        invoice.Status === 'Posted' 
                          ? 'bg-green-500/20 text-green-400'
                          : 'bg-amber-500/20 text-amber-400'
                      }`}>
                        {invoice.Status}
                      </span>
                    </div>
                  </div>

                  <div className="flex items-center gap-4 text-sm text-slate-400">
                    <div className="flex items-center gap-1">
                      <Calendar className="w-4 h-4" />
                      {formatDate(invoice.Date)}
                    </div>
                    {invoice.DueDate && invoice.DueDate !== '-' && (
                      <div className="flex items-center gap-1">
                        <span>Due:</span>
                        {formatDate(invoice.DueDate)}
                      </div>
                    )}
                    <div className="ml-auto">
                      <span className="px-2 py-1 bg-slate-700 rounded text-xs">{invoice.Type}</span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Footer Summary */}
        {!loading && !error && invoices.length > 0 && (
          <div className="border-t border-slate-700 p-6 bg-slate-800/30">
            <div className="flex items-center justify-between">
              <span className="text-slate-400">Total Transactions</span>
              <span className="text-white font-bold">{invoices.length}</span>
            </div>
            <div className="flex items-center justify-between mt-2">
              <span className="text-slate-400">Outstanding Balance</span>
              <span className="text-2xl font-bold text-blue-400">{formatCurrency(entity.balance)}</span>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
