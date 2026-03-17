'use client';

import { useState, useEffect, useMemo, useCallback } from 'react';
import {
  X, Loader2, AlertTriangle, Package, CheckCircle2, Circle, Minus, Plus,
  Send, ChevronDown, Clock, Copy, CheckCheck,
  Search, Share2, XCircle, Filter
} from 'lucide-react';
import { authenticatedFetch } from '@/core/lib/client-auth';

// ─── Types ────────────────────────────────────────────────────────────
interface ReorderItem {
  itemId: string;
  description: string;
  brand: string;
  category: string;
  currentStock: number;
  targetStock: number;
  unitCost: number;
  isMotorbike: boolean;
  stockStatus: 'out' | 'critical' | 'low' | 'ok' | 'dead';
  velocity: 'hot' | 'rising' | 'steady' | 'slowing' | 'dead';
  suggestedQty: number;
  sales: {
    last7: number;
    last30: number;
    last90: number;
    monthlyRate: number;
    weeklyRate: number;
    lastSaleDate: string | null;
    daysSinceLastSale: number | null;
  };
  lastGRN: { no: string; date: string; qty: number } | null;
  grnAgeDays: number | null;
  pendingOrder: { orderDate: string; orderedQty: number; status: string } | null;
}

interface ReorderData {
  summary: {
    total: number;
    outOfStock: number;
    critical: number;
    lowStock: number;
    deadStock: number;
    motorbike: number;
    pendingOrders: number;
    totalSuggestedQty: number;
    estimatedCost: number;
  };
  brands: string[];
  items: ReorderItem[];
  analysisMonths: number;
  timestamp: string;
}

interface SelectedItem {
  itemId: string;
  orderQty: number;
}

interface Props {
  open: boolean;
  onClose: () => void;
}

// ═══════════════════════════════════════════════════════════════════════
// MAIN COMPONENT
// ═══════════════════════════════════════════════════════════════════════
export default function ReorderView({ open, onClose }: Props) {
  const [data, setData] = useState<ReorderData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Core state
  const [selectedBrand, setSelectedBrand] = useState<string>('');
  const [selected, setSelected] = useState<Map<string, SelectedItem>>(new Map());
  const [searchQ, setSearchQ] = useState('');
  const [showPreview, setShowPreview] = useState(false);
  const [soldLast30Only, setSoldLast30Only] = useState(true);

  // Auto-select urgent items when brand changes
  useEffect(() => {
    if (!data || !selectedBrand) return;
    const sel = new Map(selected);
    // Clear previous brand's selections
    data.items.forEach(item => {
      if (item.brand !== selectedBrand) return;
      sel.delete(item.itemId);
    });
    // Auto-select out-of-stock and critical items for this brand
    data.items.forEach(item => {
      if (item.brand !== selectedBrand) return;
      if (item.stockStatus === 'dead') return;
      if (soldLast30Only && item.sales.last30 === 0) return;
      if (item.stockStatus === 'out' || item.stockStatus === 'critical') {
        sel.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty || 1 });
      }
    });
    setSelected(sel);
  }, [selectedBrand, data]);

  // WhatsApp send state
  const [sending, setSending] = useState(false);
  const [sendResult, setSendResult] = useState<{ success: boolean; message: string } | null>(null);
  const [copied, setCopied] = useState(false);

  // ─── Fetch data ─────────────────────────────────────────────────────
  useEffect(() => {
    if (open && !data) fetchData();
  }, [open]);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError('');
      const res = await authenticatedFetch('/api/erp/reorder?months=3');
      if (!res.ok) throw new Error('Failed to load reorder data');
      const json = await res.json();
      if (!json.success) throw new Error(json.error || 'Error loading data');
      setData(json);
    } catch (e: any) {
      setError(e.message || 'Failed to fetch');
    } finally {
      setLoading(false);
    }
  };

  // ─── Brand stats ────────────────────────────────────────────────────
  const brandStats = useMemo(() => {
    if (!data) return new Map<string, { total: number; out: number; critical: number; low: number; dead: number; sold30: number }>();
    const stats = new Map<string, { total: number; out: number; critical: number; low: number; dead: number; sold30: number }>();
    data.items.forEach(item => {
      // Skip items with no sales in last 30 days if filter is on
      if (soldLast30Only && item.sales.last30 === 0) return;
      if (!stats.has(item.brand)) stats.set(item.brand, { total: 0, out: 0, critical: 0, low: 0, dead: 0, sold30: 0 });
      const s = stats.get(item.brand)!;
      s.total++;
      s.sold30 += item.sales.last30;
      if (item.stockStatus === 'out') s.out++;
      else if (item.stockStatus === 'critical') s.critical++;
      else if (item.stockStatus === 'low') s.low++;
      else if (item.stockStatus === 'dead') s.dead++;
    });
    return stats;
  }, [data, soldLast30Only]);

  // ─── Filtered items for selected brand ──────────────────────────────
  const brandItems = useMemo(() => {
    if (!data || !selectedBrand) return [];
    let items = data.items.filter(i => i.brand === selectedBrand && i.stockStatus !== 'dead');

    // Filter: only items sold in last 30 days
    if (soldLast30Only) {
      items = items.filter(i => i.sales.last30 > 0);
    }

    if (searchQ) {
      const q = searchQ.toLowerCase();
      items = items.filter(i =>
        i.description.toLowerCase().includes(q) ||
        i.itemId.toLowerCase().includes(q)
      );
    }

    // Sort: out of stock first, then critical, then low, then by stock qty
    const statusOrder: Record<string, number> = { out: 0, critical: 1, low: 2, ok: 3 };
    items.sort((a, b) => {
      const sa = statusOrder[a.stockStatus] ?? 3;
      const sb = statusOrder[b.stockStatus] ?? 3;
      if (sa !== sb) return sa - sb;
      return a.currentStock - b.currentStock;
    });

    return items;
  }, [data, selectedBrand, searchQ, soldLast30Only]);

  // ─── Toggle item ────────────────────────────────────────────────────
  const toggleItem = (item: ReorderItem) => {
    setSelected(prev => {
      const next = new Map(prev);
      if (next.has(item.itemId)) {
        next.delete(item.itemId);
      } else {
        next.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty || 1 });
      }
      return next;
    });
  };

  const updateQty = (itemId: string, delta: number) => {
    setSelected(prev => {
      const next = new Map(prev);
      const entry = next.get(itemId);
      if (entry) {
        entry.orderQty = Math.max(1, entry.orderQty + delta);
      }
      return next;
    });
  };

  const selectAllBrand = () => {
    const sel = new Map(selected);
    brandItems.forEach(item => {
      if (!sel.has(item.itemId)) {
        sel.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty || 1 });
      }
    });
    setSelected(sel);
  };

  const clearBrand = () => {
    const sel = new Map(selected);
    brandItems.forEach(item => sel.delete(item.itemId));
    setSelected(sel);
  };

  // ─── Selected count for current brand ───────────────────────────────
  const brandSelectedCount = useMemo(() => {
    return brandItems.filter(i => selected.has(i.itemId)).length;
  }, [brandItems, selected]);

  // Total selected across all brands
  const totalSelected = selected.size;

  // ─── Generate WhatsApp message ──────────────────────────────────────
  const generateMessage = useCallback((): string => {
    if (!data || selected.size === 0) return '';

    // Group selected items by brand
    const brandGroups = new Map<string, { description: string; qty: number }[]>();
    for (const [itemId, sel] of selected) {
      const item = data.items.find(i => i.itemId === itemId);
      if (!item) continue;
      if (!brandGroups.has(item.brand)) brandGroups.set(item.brand, []);
      brandGroups.get(item.brand)!.push({ description: item.description, qty: sel.orderQty });
    }

    const date = new Date().toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' });
    let msg = '';
    let globalIdx = 1;

    for (const [brand, items] of brandGroups) {
      msg += `🔄 *Re-Order — ${brand}*\n`;
      msg += `📅 ${date}\n\n`;
      items.forEach(it => {
        msg += `${globalIdx}. ${it.description} — *Qty: ${it.qty}*\n`;
        globalIdx++;
      });
      msg += `\n_${items.length} items_\n\n`;
    }

    msg += `━━━━━━━━━━━━━━━━━━\n`;
    msg += `📦 *Total: ${selected.size} items*`;

    return msg.trim();
  }, [data, selected]);

  // ─── Copy message ───────────────────────────────────────────────────
  const copyMessage = async () => {
    const msg = generateMessage();
    try {
      await navigator.clipboard.writeText(msg);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      const ta = document.createElement('textarea');
      ta.value = msg;
      document.body.appendChild(ta);
      ta.select();
      document.execCommand('copy');
      document.body.removeChild(ta);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  // ─── Send via WhatsApp Bot ──────────────────────────────────────────
  const sendViaWhatsApp = async () => {
    if (!data || selected.size === 0) return;

    // Group by brand for API
    const brandGroups = new Map<string, { description: string; qty: number }[]>();
    for (const [itemId, sel] of selected) {
      const item = data.items.find(i => i.itemId === itemId);
      if (!item) continue;
      if (!brandGroups.has(item.brand)) brandGroups.set(item.brand, []);
      brandGroups.get(item.brand)!.push({ description: item.description, qty: sel.orderQty });
    }

    const date = new Date().toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' });
    const messages = [...brandGroups].map(([brand, items]) => {
      let msg = `🔄 *Re-Order — ${brand}*\n📅 ${date}\n\n`;
      items.forEach((it, idx) => {
        msg += `${idx + 1}. ${it.description} — *Qty: ${it.qty}*\n`;
      });
      msg += `\n_${items.length} items_`;
      return { brand, message: msg };
    });

    try {
      setSending(true);
      setSendResult(null);

      const res = await authenticatedFetch('/api/erp/reorder/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ messages }),
      });
      const result = await res.json();

      if (result.success) {
        setSendResult({
          success: true,
          message: `✅ Sent ${result.sentCount} message${result.sentCount > 1 ? 's' : ''} to "${result.groupName}"`,
        });
        setShowPreview(false);
      } else {
        setSendResult({ success: false, message: result.error || 'Failed to send' });
      }
    } catch (e: any) {
      setSendResult({ success: false, message: e.message || 'Network error' });
    } finally {
      setSending(false);
      setTimeout(() => setSendResult(null), 5000);
    }
  };

  if (!open) return null;

  // ═══════════════════════════════════════════════════════════════════
  // RENDER
  // ═══════════════════════════════════════════════════════════════════
  return (
    <div className="fixed inset-0 z-50 bg-zinc-50 flex flex-col">

      {/* ─── Header ─────────────────────────────────────────────────── */}
      <div className="bg-white border-b border-zinc-200 px-4 py-3 flex items-center justify-between shrink-0 shadow-sm">
        <div>
          <h2 className="font-bold text-lg text-zinc-900 leading-tight">Re-Order</h2>
          {data && (
            <p className="text-zinc-400 text-[11px] mt-0.5">
              {data.summary.outOfStock} out · {data.summary.critical} critical · {data.summary.lowStock} low
            </p>
          )}
        </div>
        <button onClick={onClose} className="w-8 h-8 rounded-full bg-zinc-100 flex items-center justify-center hover:bg-zinc-200 transition-colors">
          <X size={16} className="text-zinc-500" />
        </button>
      </div>

      {/* ─── Loading ─────────────────────────────────────────────────── */}
      {loading && (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <Loader2 className="w-8 h-8 text-indigo-500 animate-spin mx-auto mb-3" />
            <p className="text-zinc-400 text-sm">Analyzing inventory…</p>
          </div>
        </div>
      )}

      {/* ─── Error ───────────────────────────────────────────────────── */}
      {error && (
        <div className="flex-1 flex items-center justify-center p-6">
          <div className="text-center">
            <AlertTriangle className="w-8 h-8 text-rose-400 mx-auto mb-3" />
            <p className="text-rose-600 font-medium text-sm">{error}</p>
            <button onClick={fetchData} className="mt-3 px-4 py-2 bg-indigo-600 text-white rounded-lg text-sm font-medium">
              Retry
            </button>
          </div>
        </div>
      )}

      {/* ─── Toast ───────────────────────────────────────────────────── */}
      {sendResult && (
        <div className={`mx-4 mt-2 p-3 rounded-xl flex items-center gap-2 text-sm font-medium ${
          sendResult.success ? 'bg-green-50 text-green-700 border border-green-200' : 'bg-rose-50 text-rose-700 border border-rose-200'
        }`}>
          {sendResult.success ? <CheckCircle2 size={16} /> : <XCircle size={16} />}
          {sendResult.message}
        </div>
      )}

      {/* ─── Main Content ────────────────────────────────────────────── */}
      {!loading && !error && data && (
        <div className={`flex-1 overflow-y-auto ${selectedBrand ? 'pb-24' : 'pb-4'}`}>

          {/* ─── Brand Dropdown ────────────────────────────────────── */}
          <div className="px-4 pt-4 pb-2">
            <label className="block text-[11px] font-bold text-zinc-500 uppercase tracking-wider mb-1.5">
              Select Brand
            </label>
            <div className="relative">
              <select
                value={selectedBrand}
                onChange={e => { setSelectedBrand(e.target.value); setSearchQ(''); }}
                className="w-full appearance-none bg-white border-2 border-zinc-200 rounded-xl px-4 py-3 pr-10 text-sm font-semibold text-zinc-900 focus:outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100 transition-colors"
              >
                <option value="">— Choose a brand —</option>
                {data.brands.filter(brand => brandStats.has(brand)).map(brand => {
                  const stats = brandStats.get(brand)!;
                  const urgent = stats.out + stats.critical;
                  const total = stats.total - stats.dead;
                  return (
                    <option key={brand} value={brand}>
                      {brand} ({total} items{urgent > 0 ? ` · ${urgent} urgent` : ''})
                    </option>
                  );
                })}
              </select>
              <ChevronDown size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 pointer-events-none" />
            </div>

            {/* Sold in last 30 days filter */}
            <button
              onClick={() => setSoldLast30Only(!soldLast30Only)}
              className={`mt-2 flex items-center gap-2 px-3 py-2 rounded-lg text-xs font-bold transition-all ${
                soldLast30Only
                  ? 'bg-indigo-50 text-indigo-700 border border-indigo-200'
                  : 'bg-zinc-50 text-zinc-400 border border-zinc-200'
              }`}
            >
              <Filter size={12} />
              Last 30 Days Sales Only
              {soldLast30Only && <span className="bg-indigo-600 text-white text-[9px] px-1.5 py-0.5 rounded-full">ON</span>}
            </button>
          </div>

          {/* ─── No brand selected ─────────────────────────────────── */}
          {!selectedBrand && (
            <div className="px-4 pt-6">
              {/* Summary Cards */}
              <div className="grid grid-cols-3 gap-2 mb-6">
                <div className="bg-rose-50 border border-rose-200 rounded-xl p-3 text-center">
                  <div className="text-2xl font-black text-rose-700">{data.summary.outOfStock}</div>
                  <div className="text-[10px] font-bold text-rose-500 uppercase">Out of Stock</div>
                </div>
                <div className="bg-orange-50 border border-orange-200 rounded-xl p-3 text-center">
                  <div className="text-2xl font-black text-orange-700">{data.summary.critical}</div>
                  <div className="text-[10px] font-bold text-orange-500 uppercase">Critical</div>
                </div>
                <div className="bg-amber-50 border border-amber-200 rounded-xl p-3 text-center">
                  <div className="text-2xl font-black text-amber-700">{data.summary.lowStock}</div>
                  <div className="text-[10px] font-bold text-amber-500 uppercase">Low Stock</div>
                </div>
              </div>

              {/* Brand Quick Cards */}
              <h3 className="text-xs font-bold text-zinc-500 uppercase tracking-wider mb-2">Brands needing orders</h3>
              <div className="space-y-2">
                {data.brands.map(brand => {
                  const stats = brandStats.get(brand);
                  if (!stats) return null;
                  const urgent = stats.out + stats.critical;
                  const active = stats.total - stats.dead;
                  if (active === 0) return null;
                  return (
                    <button
                      key={brand}
                      onClick={() => setSelectedBrand(brand)}
                      className="w-full bg-white border border-zinc-200 rounded-xl p-3 flex items-center justify-between hover:border-indigo-300 active:scale-[0.99] transition-all"
                    >
                      <div className="flex items-center gap-3">
                        <div className={`w-10 h-10 rounded-lg flex items-center justify-center text-sm font-black ${
                          urgent > 0 ? 'bg-rose-100 text-rose-600' : 'bg-indigo-50 text-indigo-600'
                        }`}>
                          {brand.substring(0, 2)}
                        </div>
                        <div className="text-left">
                          <div className="font-bold text-sm text-zinc-900">{brand}</div>
                          <div className="text-[11px] text-zinc-400">{active} items</div>
                        </div>
                      </div>
                      <div className="flex items-center gap-2">
                        {stats.out > 0 && (
                          <span className="bg-rose-600 text-white text-[10px] font-bold px-2 py-0.5 rounded-full">
                            {stats.out} out
                          </span>
                        )}
                        {stats.critical > 0 && (
                          <span className="bg-orange-500 text-white text-[10px] font-bold px-2 py-0.5 rounded-full">
                            {stats.critical} critical
                          </span>
                        )}
                        {stats.low > 0 && (
                          <span className="bg-amber-100 text-amber-700 text-[10px] font-bold px-2 py-0.5 rounded-full">
                            {stats.low} low
                          </span>
                        )}
                        <ChevronDown size={14} className="text-zinc-300 -rotate-90" />
                      </div>
                    </button>
                  );
                })}
              </div>
            </div>
          )}

          {/* ─── Brand Items ───────────────────────────────────────── */}
          {selectedBrand && (
            <div>
              {/* Brand Header */}
              <div className="px-4 py-2 flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <h3 className="font-bold text-sm text-zinc-900">{selectedBrand}</h3>
                  <span className="text-[11px] text-zinc-400">{brandItems.length} items</span>
                </div>
                <div className="flex items-center gap-2">
                  <button onClick={selectAllBrand} className="text-[11px] font-bold text-indigo-600 px-2 py-1 rounded-lg hover:bg-indigo-50">
                    Select All
                  </button>
                  {brandSelectedCount > 0 && (
                    <button onClick={clearBrand} className="text-[11px] font-bold text-zinc-400 px-2 py-1 rounded-lg hover:bg-zinc-50">
                      Clear
                    </button>
                  )}
                </div>
              </div>

              {/* Search within brand */}
              {brandItems.length > 5 && (
                <div className="px-4 mb-2">
                  <div className="relative">
                    <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-400" />
                    <input
                      type="text"
                      placeholder="Search items…"
                      value={searchQ}
                      onChange={e => setSearchQ(e.target.value)}
                      className="w-full pl-8 pr-3 py-2 bg-white border border-zinc-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    />
                  </div>
                </div>
              )}

              {/* Items List */}
              <div className="px-4 space-y-2">
                {brandItems.map(item => {
                  const isSelected = selected.has(item.itemId);
                  const orderQty = selected.get(item.itemId)?.orderQty || item.suggestedQty || 1;

                  return (
                    <div
                      key={item.itemId}
                      onClick={() => toggleItem(item)}
                      className={`bg-white border rounded-xl p-3 transition-all cursor-pointer active:scale-[0.99] ${
                        isSelected ? 'border-indigo-400 shadow-sm shadow-indigo-100' : 'border-zinc-200'
                      }`}
                    >
                      <div className="flex items-start gap-3">
                        {/* Checkbox */}
                        <div className="mt-0.5 shrink-0">
                          {isSelected ? (
                            <CheckCircle2 size={22} className="text-indigo-600" />
                          ) : (
                            <Circle size={22} className="text-zinc-300" />
                          )}
                        </div>

                        {/* Info */}
                        <div className="flex-1 min-w-0">
                          {/* Status row */}
                          <div className="flex items-center gap-1.5 mb-1 flex-wrap">
                            <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-[10px] font-bold ${
                              item.stockStatus === 'out' ? 'bg-rose-600 text-white' :
                              item.stockStatus === 'critical' ? 'bg-orange-100 text-orange-700' :
                              item.stockStatus === 'low' ? 'bg-amber-100 text-amber-700' :
                              'bg-emerald-100 text-emerald-700'
                            }`}>
                              {item.stockStatus === 'out' ? '⛔ OUT' : `📦 Stock: ${item.currentStock}`}
                            </span>
                            <span className="text-[10px] bg-blue-50 text-blue-700 px-2 py-0.5 rounded-full font-bold">
                              Sold 30d: {item.sales.last30}
                            </span>
                            {item.currentStock > 0 && item.sales.last30 > 0 && (
                              <span className={`text-[10px] px-2 py-0.5 rounded-full font-bold ${
                                Math.ceil(item.currentStock / (item.sales.last30 / 30)) <= 7
                                  ? 'bg-rose-50 text-rose-600'
                                  : Math.ceil(item.currentStock / (item.sales.last30 / 30)) <= 14
                                    ? 'bg-amber-50 text-amber-600'
                                    : 'bg-emerald-50 text-emerald-600'
                              }`}>
                                ~{Math.ceil(item.currentStock / (item.sales.last30 / 30))}d left
                              </span>
                            )}
                          </div>

                          {/* Description */}
                          <p className="text-[13px] font-medium text-zinc-900 leading-snug">
                            {item.description}
                          </p>

                          {/* Pending order warning */}
                          {item.pendingOrder && (
                            <div className="mt-1 flex items-center gap-1 text-[10px] text-blue-600">
                              <Clock size={10} />
                              <span className="font-medium">Pending: {item.pendingOrder.orderedQty} ordered</span>
                            </div>
                          )}
                        </div>

                        {/* Qty control */}
                        {isSelected && (
                          <div className="shrink-0 flex flex-col items-center gap-0.5" onClick={e => e.stopPropagation()}>
                            <span className="text-[8px] font-bold text-zinc-400 uppercase">Qty</span>
                            <div className="flex items-center bg-zinc-100 rounded-lg overflow-hidden">
                              <button
                                onClick={() => updateQty(item.itemId, -1)}
                                className="w-8 h-8 flex items-center justify-center text-zinc-600 hover:bg-zinc-200 active:bg-zinc-300"
                              >
                                <Minus size={14} />
                              </button>
                              <span className="w-8 text-center text-sm font-bold text-indigo-700">{orderQty}</span>
                              <button
                                onClick={() => updateQty(item.itemId, 1)}
                                className="w-8 h-8 flex items-center justify-center text-zinc-600 hover:bg-zinc-200 active:bg-zinc-300"
                              >
                                <Plus size={14} />
                              </button>
                            </div>
                          </div>
                        )}
                      </div>
                    </div>
                  );
                })}

                {brandItems.length === 0 && (
                  <div className="text-center py-8">
                    <Package className="w-8 h-8 text-zinc-300 mx-auto mb-2" />
                    <p className="text-zinc-400 text-sm">No items found</p>
                  </div>
                )}

                {/* ── SHARE BUTTON ── right here in the list ── */}
                <div className="pt-4 pb-8">
                  {totalSelected > 0 ? (
                    <button
                      onClick={(e) => { e.stopPropagation(); setShowPreview(true); }}
                      className="w-full bg-green-600 text-white py-4 rounded-2xl font-bold text-base flex items-center justify-center gap-2 shadow-lg active:bg-green-700"
                    >
                      <Share2 size={20} />
                      📤 Share Order ({totalSelected} items)
                    </button>
                  ) : (
                    <div className="w-full bg-zinc-200 text-zinc-500 py-4 rounded-2xl font-bold text-sm flex items-center justify-center gap-2">
                      <Share2 size={18} />
                      ☝️ Select items above to share
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      )}

      {/* ═══════════════════════════════════════════════════════════════ */}
      {/* MESSAGE PREVIEW & SEND PANEL                                   */}
      {/* ═══════════════════════════════════════════════════════════════ */}
      {showPreview && (
        <div className="fixed inset-0 z-[60] bg-black/40 flex items-end">
          <div className="bg-white w-full rounded-t-3xl max-h-[85vh] flex flex-col animate-slide-up">

            {/* Preview Header */}
            <div className="px-4 py-3 border-b border-zinc-100 flex items-center justify-between shrink-0">
              <h3 className="font-bold text-zinc-900">Order Preview</h3>
              <button
                onClick={() => setShowPreview(false)}
                className="w-8 h-8 rounded-full bg-zinc-100 flex items-center justify-center"
              >
                <X size={14} className="text-zinc-500" />
              </button>
            </div>

            {/* Message Preview */}
            <div className="flex-1 overflow-y-auto px-4 py-3">
              <div className="bg-[#e7fed6] rounded-2xl rounded-tr-sm p-4 shadow-sm">
                <pre className="text-[13px] text-zinc-800 whitespace-pre-wrap font-sans leading-relaxed">
                  {generateMessage()}
                </pre>
              </div>
            </div>

            {/* Action Buttons */}
            <div className="px-4 py-4 border-t border-zinc-100 space-y-2 shrink-0">
              {/* Send via WhatsApp Bot */}
              <button
                onClick={sendViaWhatsApp}
                disabled={sending}
                className="w-full bg-green-600 text-white py-3 rounded-xl font-bold text-sm flex items-center justify-center gap-2 disabled:opacity-50 active:bg-green-700 transition-colors"
              >
                {sending ? (
                  <><Loader2 size={16} className="animate-spin" /> Sending…</>
                ) : (
                  <><Send size={16} /> Send to &quot;Re Order&quot; Group</>
                )}
              </button>

              {/* Copy */}
              <button
                onClick={copyMessage}
                className={`w-full py-3 rounded-xl font-bold text-sm flex items-center justify-center gap-2 transition-colors ${
                  copied
                    ? 'bg-green-100 text-green-700'
                    : 'bg-zinc-100 text-zinc-700 active:bg-zinc-200'
                }`}
              >
                {copied ? (
                  <><CheckCheck size={16} /> Copied!</>
                ) : (
                  <><Copy size={16} /> Copy Message</>
                )}
              </button>
            </div>
          </div>
        </div>
      )}

      <style jsx>{`
        @keyframes slide-up {
          from { transform: translateY(100%); }
          to { transform: translateY(0); }
        }
        .animate-slide-up {
          animation: slide-up 0.3s ease-out;
        }
      `}</style>
    </div>
  );
}
