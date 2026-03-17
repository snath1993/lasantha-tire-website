'use client';

import { useState, useEffect, useMemo, useCallback, useRef } from 'react';
import {
  X, Loader2, AlertTriangle, Package, TrendingUp, TrendingDown, Flame,
  CheckCircle2, Circle, Minus, Plus, Send, Brain, Copy, ChevronDown,
  ChevronUp, AlertCircle, Clock, BarChart3, Sparkles, Filter,
  ArrowUpDown, MessageSquare, CheckCheck, Search, Eye, EyeOff,
  Skull, XCircle
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

// ─── Velocity Badge ───────────────────────────────────────────────────
function VelocityBadge({ velocity }: { velocity: string }) {
  const config: Record<string, { icon: React.ReactNode; label: string; cls: string }> = {
    hot:     { icon: <Flame size={10} />, label: 'Hot',     cls: 'bg-red-50 text-red-700 border-red-200' },
    rising:  { icon: <TrendingUp size={10} />, label: 'Rising',  cls: 'bg-orange-50 text-orange-700 border-orange-200' },
    steady:  { icon: <BarChart3 size={10} />, label: 'Steady',  cls: 'bg-blue-50 text-blue-700 border-blue-200' },
    slowing: { icon: <TrendingDown size={10} />, label: 'Slowing', cls: 'bg-yellow-50 text-yellow-700 border-yellow-200' },
    dead:    { icon: <AlertCircle size={10} />, label: 'Dead',    cls: 'bg-zinc-100 text-zinc-500 border-zinc-200' },
  };
  const c = config[velocity] || config.steady;
  return (
    <span className={`inline-flex items-center gap-0.5 px-1.5 py-0.5 rounded text-[9px] font-bold uppercase tracking-wider border ${c.cls}`}>
      {c.icon} {c.label}
    </span>
  );
}

// ─── Stock Status Badge ───────────────────────────────────────────────
function StockBadge({ status, qty }: { status: string; qty: number }) {
  const config: Record<string, string> = {
    out:      'bg-rose-600 text-white',
    critical: 'bg-rose-100 text-rose-700',
    low:      'bg-amber-100 text-amber-700',
    ok:       'bg-emerald-100 text-emerald-700',
    dead:     'bg-zinc-200 text-zinc-600',
  };
  return (
    <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-bold ${config[status] || config.low}`}>
      {status === 'out' ? '⛔ OUT' : status === 'dead' ? '💀 DEAD' : `📦 ${qty}`}
    </span>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// MAIN COMPONENT
// ═══════════════════════════════════════════════════════════════════════
export default function ReorderView({ open, onClose }: Props) {
  const [activeTab, setActiveTab] = useState<'basic' | 'smart'>('basic');
  const [data, setData] = useState<ReorderData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  
  // Shared state
  const [activeBrands, setActiveBrands] = useState<string[]>([]);
  const [selected, setSelected] = useState<Map<string, SelectedItem>>(new Map());
  const [sortBy, setSortBy] = useState<'stock' | 'velocity' | 'brand'>('stock');
  const [searchQ, setSearchQ] = useState('');
  const [copiedBrand, setCopiedBrand] = useState<string | null>(null);
  const [showAiPrompt, setShowAiPrompt] = useState(false);
  const [aiPromptCopied, setAiPromptCopied] = useState(false);
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc');
  const [hideDead, setHideDead] = useState(true);  // Hide dead stock by default
  const promptRef = useRef<HTMLTextAreaElement>(null);

  // WhatsApp send state
  const [sending, setSending] = useState(false);
  const [sendResult, setSendResult] = useState<{ success: boolean; message: string } | null>(null);

  // Fetch data on open
  useEffect(() => {
    if (open && !data) {
      fetchData();
    }
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
      
      // Auto-select critical items in Smart mode
      if (activeTab === 'smart') {
        autoSelectSmartItems(json.items);
      }
    } catch (e: any) {
      setError(e.message || 'Failed to fetch');
    } finally {
      setLoading(false);
    }
  };

  const toggleBrand = (brand: string) => {
    if (brand === 'all') {
      setActiveBrands([]);
      return;
    }
    setActiveBrands(prev => {
      if (prev.includes(brand)) return prev.filter(b => b !== brand);
      return [...prev, brand];
    });
  };

  const autoSelectSmartItems = (items: ReorderItem[]) => {
    const sel = new Map<string, SelectedItem>();
    items.forEach(item => {
      // Skip dead stock from auto-select
      if (item.stockStatus === 'dead') return;
      if (item.stockStatus === 'out' || item.stockStatus === 'critical') {
        if (!item.pendingOrder) {
          sel.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty });
        }
      }
      if (item.velocity === 'hot' && item.stockStatus === 'low') {
        sel.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty });
      }
    });
    setSelected(sel);
  };

  // Toggle selection
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

  // Update qty
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

  // Select all filtered
  const selectAll = () => {
    const sel = new Map(selected);
    filteredItems.forEach(item => {
      if (!sel.has(item.itemId) && item.stockStatus !== 'dead') {
        sel.set(item.itemId, { itemId: item.itemId, orderQty: item.suggestedQty || 1 });
      }
    });
    setSelected(sel);
  };

  // Clear all
  const clearAll = () => setSelected(new Map());

  // ─── Filter & Sort ──────────────────────────────────────────────────
  const filteredItems = useMemo(() => {
    if (!data) return [];
    let items = [...data.items];

    if (activeBrands.length > 0) {
      items = items.filter(i => activeBrands.includes(i.brand));
    }
    // Dead stock filter
    if (hideDead) {
      items = items.filter(i => i.stockStatus !== 'dead');
    }
    if (searchQ) {
      const q = searchQ.toLowerCase();
      items = items.filter(i => 
        i.description.toLowerCase().includes(q) || 
        i.itemId.toLowerCase().includes(q) ||
        i.brand.toLowerCase().includes(q)
      );
    }
    
    items.sort((a, b) => {
      let cmp = 0;
      if (sortBy === 'stock') cmp = a.currentStock - b.currentStock;
      else if (sortBy === 'velocity') {
        const order: Record<string, number> = { hot: 0, rising: 1, steady: 2, slowing: 3, dead: 4 };
        cmp = (order[a.velocity] ?? 2) - (order[b.velocity] ?? 2);
      } else {
        cmp = a.brand.localeCompare(b.brand);
      }
      return sortDir === 'asc' ? cmp : -cmp;
    });
    
    return items;
  }, [data, activeBrands, sortBy, sortDir, searchQ, hideDead]);

  // Brand stats for filter
  const brandStats = useMemo(() => {
    if (!data) return [];
    const stats = new Map<string, { total: number; out: number; critical: number; low: number; dead: number }>();
    data.items.forEach(item => {
      if (!stats.has(item.brand)) {
        stats.set(item.brand, { total: 0, out: 0, critical: 0, low: 0, dead: 0 });
      }
      const s = stats.get(item.brand)!;
      s.total++;
      if (item.stockStatus === 'out') s.out++;
      if (item.stockStatus === 'critical') s.critical++;
      if (item.stockStatus === 'low') s.low++;
      if (item.stockStatus === 'dead') s.dead++;
    });
    return [...stats.entries()].sort((a, b) => {
      const urgA = a[1].out + a[1].critical;
      const urgB = b[1].out + b[1].critical;
      return urgB - urgA;
    });
  }, [data]);

  // ─── Brand-wise WhatsApp Messages ────────────────────────────────────
  const generateWhatsAppMessages = useCallback((): Map<string, string> => {
    if (!data) return new Map();

    const brandGroups = new Map<string, { description: string; qty: number }[]>();

    for (const [itemId, sel] of selected) {
      const item = data.items.find(i => i.itemId === itemId);
      if (!item) continue;

      if (!brandGroups.has(item.brand)) {
        brandGroups.set(item.brand, []);
      }
      brandGroups.get(item.brand)!.push({
        description: item.description,
        qty: sel.orderQty,
      });
    }

    const messages = new Map<string, string>();
    for (const [brand, items] of brandGroups) {
      let msg = `🔄 *Re-Order Request — ${brand}*\n`;
      msg += `📅 ${new Date().toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}\n\n`;
      items.forEach((it, idx) => {
        msg += `${idx + 1}. ${it.description} — *${it.qty}*\n`;
      });
      msg += `\n_Total: ${items.length} items_`;
      messages.set(brand, msg);
    }
    return messages;
  }, [data, selected]);

  // Copy individual brand message
  const copyBrandMessage = async (brand: string, msg: string) => {
    try {
      await navigator.clipboard.writeText(msg);
      setCopiedBrand(brand);
      setTimeout(() => setCopiedBrand(null), 2000);
    } catch {
      // fallback
      const ta = document.createElement('textarea');
      ta.value = msg;
      document.body.appendChild(ta);
      ta.select();
      document.execCommand('copy');
      document.body.removeChild(ta);
      setCopiedBrand(brand);
      setTimeout(() => setCopiedBrand(null), 2000);
    }
  };

  // ─── AI Prompt Generator ────────────────────────────────────────────
  const generateAIPrompt = useCallback((): string => {
    if (!data) return '';

    const lines: string[] = [];
    lines.push('=== LASANTHA TIRE — INTELLIGENT RE-ORDER ANALYSIS ===');
    lines.push(`Generated: ${new Date().toLocaleString()}`);
    lines.push(`Analysis Period: Last ${data.analysisMonths} months`);
    lines.push('');
    
    lines.push('## INSTRUCTIONS FOR AI');
    lines.push('You are a tire shop inventory analyst. Analyze the data below and:');
    lines.push('1. Identify items that MUST be re-ordered immediately (out of stock + recent sales)');
    lines.push('2. Identify items to re-order within this week (low stock + moderate sales velocity)');
    lines.push('3. Flag dead stock items (have stock but no sales >6 months) — DO NOT order these');
    lines.push('4. For items with pending orders, skip unless stock is 0 and high demand');
    lines.push('5. Consider brand diversity — suggest order quantities that balance brand coverage');
    lines.push('6. Generate a final brand-wise order list with: Item Description | Order Qty');
    lines.push('7. Explain your reasoning for top 5 most important items');
    lines.push('8. Give a stock health score (0-100) and actionable recommendations');
    lines.push('');

    lines.push('## SUMMARY');
    lines.push(`Total items needing attention: ${data.summary.total}`);
    lines.push(`Out of stock: ${data.summary.outOfStock}`);
    lines.push(`Critical (1 left): ${data.summary.critical}`);
    lines.push(`Low stock: ${data.summary.lowStock}`);
    lines.push(`Dead stock: ${data.summary.deadStock}`);
    lines.push(`Pending orders: ${data.summary.pendingOrders}`);
    lines.push(`Estimated cost for suggested orders: Rs ${data.summary.estimatedCost.toLocaleString()}`);
    lines.push('');

    // Group by brand
    const brandGroups = new Map<string, ReorderItem[]>();
    data.items.forEach(item => {
      if (!brandGroups.has(item.brand)) brandGroups.set(item.brand, []);
      brandGroups.get(item.brand)!.push(item);
    });

    lines.push('## COMPLETE INVENTORY DATA (Brand-wise)');
    lines.push('');

    for (const [brand, items] of brandGroups) {
      lines.push(`### ${brand} (${items.length} items)`);
      lines.push(`${'Item'.padEnd(52)} | Stock | Target | 7d | 30d | 90d | Velocity | Status   | GRN Age  | Suggested`);
      lines.push(`${'─'.repeat(52)}-|-------|--------|----|----|-----|----------|----------|----------|----------`);
      
      items.forEach(item => {
        const desc = item.description.substring(0, 50).padEnd(52);
        const stock = String(item.currentStock).padStart(5);
        const target = String(item.targetStock).padStart(6);
        const s7 = String(item.sales.last7).padStart(2);
        const s30 = String(item.sales.last30).padStart(3);
        const s90 = String(item.sales.last90).padStart(3);
        const vel = item.velocity.padEnd(8);
        const status = item.stockStatus.padEnd(8);
        const grnAge = item.grnAgeDays !== null ? `${item.grnAgeDays}d`.padEnd(8) : 'N/A     ';
        const suggested = String(item.suggestedQty).padStart(8);
        
        lines.push(`${desc} | ${stock} | ${target} | ${s7} | ${s30} | ${s90} | ${vel} | ${status} | ${grnAge} | ${suggested}`);
        
        if (item.pendingOrder) {
          lines.push(`  ⚠️ PENDING ORDER: ${item.pendingOrder.orderedQty} units ordered on ${new Date(item.pendingOrder.orderDate).toLocaleDateString()}`);
        }
        if (item.lastGRN) {
          lines.push(`  📦 Last GRN: ${item.lastGRN.no} on ${new Date(item.lastGRN.date).toLocaleDateString()} (${item.lastGRN.qty} units)`);
        }
      });
      lines.push('');
    }

    lines.push('## KEY METRICS');
    lines.push('- TARGET_STOCK for general tyres: 4');
    lines.push('- TARGET_STOCK for motorbike tyres: 2');
    lines.push('- Dead stock threshold: 180 days without sale');
    lines.push('- Velocity "hot": 7-day avg ≥ 2x 30-day avg');
    lines.push('- Velocity "rising": 7-day avg ≥ 1.2x 30-day avg');
    lines.push('');
    lines.push('## EXPECTED OUTPUT FORMAT');
    lines.push('Please provide:');
    lines.push('1. Stock Health Score: X/100');
    lines.push('2. Priority 1 (Order TODAY):');
    lines.push('   Brand → Item Description → Qty');
    lines.push('3. Priority 2 (Order this week):');
    lines.push('   Brand → Item Description → Qty');  
    lines.push('4. DO NOT ORDER (dead stock / overstocked):');
    lines.push('   Brand → Item Description → Reason');
    lines.push('5. Brand-wise WhatsApp messages ready to copy');
    lines.push('6. Recommendations for improving stock management');

    return lines.join('\n');
  }, [data]);

  const copyAIPrompt = async () => {
    const prompt = generateAIPrompt();
    try {
      await navigator.clipboard.writeText(prompt);
      setAiPromptCopied(true);
      setTimeout(() => setAiPromptCopied(false), 3000);
    } catch {
      if (promptRef.current) {
        promptRef.current.value = prompt;
        promptRef.current.select();
        document.execCommand('copy');
        setAiPromptCopied(true);
        setTimeout(() => setAiPromptCopied(false), 3000);
      }
    }
  };

  // ─── Computed metrics for Smart tab ──────────────────────────────────
  const healthScore = useMemo(() => {
    if (!data) return 0;
    const total = data.summary.total;
    if (total === 0) return 100;
    const penalty = 
      data.summary.outOfStock * 10 + 
      data.summary.critical * 5 + 
      data.summary.lowStock * 2 + 
      data.summary.deadStock * 1;
    return Math.max(0, Math.min(100, 100 - Math.round(penalty / total * 20)));
  }, [data]);

  const whatsappMessages = useMemo(() => generateWhatsAppMessages(), [generateWhatsAppMessages]);

  // Count active items (excl dead) for display
  const activeCount = useMemo(() => {
    if (!data) return 0;
    return data.items.filter(i => i.stockStatus !== 'dead').length;
  }, [data]);

  // ─── Send via WhatsApp Bot ──────────────────────────────────────────
  const sendViaWhatsApp = async (brandToSend?: string) => {
    const msgs = generateWhatsAppMessages();
    if (msgs.size === 0) return;

    const toSend = brandToSend
      ? [{ brand: brandToSend, message: msgs.get(brandToSend)! }].filter(m => m.message)
      : [...msgs].map(([brand, message]) => ({ brand, message }));

    if (toSend.length === 0) return;

    try {
      setSending(true);
      setSendResult(null);

      const res = await authenticatedFetch('/api/erp/reorder/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ messages: toSend }),
      });

      const result = await res.json();

      if (result.success) {
        setSendResult({
          success: true,
          message: `✅ ${result.sentCount}/${result.totalCount} messages sent to "${result.groupName}" group`,
        });
      } else {
        setSendResult({
          success: false,
          message: result.error || 'Failed to send messages',
        });
      }
    } catch (e: any) {
      setSendResult({
        success: false,
        message: e.message || 'Network error',
      });
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
    <div className="fixed inset-0 z-50 bg-white flex flex-col">
      {/* ─── Header ─────────────────────────────────────────────────── */}
      <div className="bg-gradient-to-r from-indigo-600 to-violet-600 text-white px-4 py-3 flex items-center justify-between shrink-0">
        <div>
          <h2 className="font-bold text-lg leading-tight">Re-Order Manager</h2>
          <p className="text-indigo-200 text-[10px] mt-0.5">
            {data ? `${activeCount} items need ordering${hideDead ? ` · ${data.summary.deadStock} dead hidden` : ''}` : 'Loading...'}
          </p>
        </div>
        <button onClick={onClose} className="p-2 rounded-full hover:bg-white/10 transition-colors">
          <X size={20} />
        </button>
      </div>

      {/* ─── Tabs ───────────────────────────────────────────────────── */}
      <div className="flex border-b border-zinc-200 shrink-0">
        <button
          onClick={() => setActiveTab('basic')}
          className={`flex-1 py-3 text-sm font-bold text-center transition-colors relative ${
            activeTab === 'basic'
              ? 'text-indigo-600'
              : 'text-zinc-400 hover:text-zinc-600'
          }`}
        >
          <Package size={14} className="inline mr-1.5 -mt-0.5" />
          Basic
          {activeTab === 'basic' && <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-indigo-600" />}
        </button>
        <button
          onClick={() => {
            setActiveTab('smart');
            if (data) autoSelectSmartItems(data.items);
          }}
          className={`flex-1 py-3 text-sm font-bold text-center transition-colors relative ${
            activeTab === 'smart'
              ? 'text-violet-600'
              : 'text-zinc-400 hover:text-zinc-600'
          }`}
        >
          <Brain size={14} className="inline mr-1.5 -mt-0.5" />
          Smart AI
          {activeTab === 'smart' && <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-violet-600" />}
        </button>
      </div>

      {/* ─── Loading / Error ─────────────────────────────────────────── */}
      {loading && (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <Loader2 className="w-10 h-10 text-indigo-500 animate-spin mx-auto mb-3" />
            <p className="text-zinc-500 text-sm">Analyzing inventory & sales…</p>
          </div>
        </div>
      )}

      {error && (
        <div className="flex-1 flex items-center justify-center p-6">
          <div className="text-center">
            <AlertTriangle className="w-10 h-10 text-rose-400 mx-auto mb-3" />
            <p className="text-rose-600 font-medium">{error}</p>
            <button 
              onClick={fetchData}
              className="mt-3 px-4 py-2 bg-indigo-600 text-white rounded-lg text-sm font-medium"
            >
              Retry
            </button>
          </div>
        </div>
      )}

      {/* ─── Send Result Toast ────────────────────────────────────── */}
      {sendResult && (
        <div className={`mx-4 mt-2 p-3 rounded-xl flex items-center gap-2 text-sm font-medium ${
          sendResult.success ? 'bg-green-50 text-green-700 border border-green-200' : 'bg-rose-50 text-rose-700 border border-rose-200'
        }`}>
          {sendResult.success ? <CheckCircle2 size={16} /> : <XCircle size={16} />}
          {sendResult.message}
        </div>
      )}

      {/* ─── Content ─────────────────────────────────────────────────── */}
      {!loading && !error && data && (
        <div className="flex-1 overflow-y-auto">
          {activeTab === 'basic' ? (
            <BasicTab
              data={data}
              items={filteredItems}
              selected={selected}
              activeBrands={activeBrands}
              toggleBrand={toggleBrand}
              brandStats={brandStats}
              sortBy={sortBy}
              setSortBy={setSortBy}
              sortDir={sortDir}
              setSortDir={setSortDir}
              searchQ={searchQ}
              setSearchQ={setSearchQ}
              hideDead={hideDead}
              setHideDead={setHideDead}
              toggleItem={toggleItem}
              updateQty={updateQty}
              selectAll={selectAll}
              clearAll={clearAll}
              whatsappMessages={whatsappMessages}
              copiedBrand={copiedBrand}
              copyBrandMessage={copyBrandMessage}
              sendViaWhatsApp={sendViaWhatsApp}
              sending={sending}
            />
          ) : (
            <SmartTab
              data={data}
              items={filteredItems}
              selected={selected}
              healthScore={healthScore}
              activeBrands={activeBrands}
              toggleBrand={toggleBrand}
              brandStats={brandStats}
              hideDead={hideDead}
              setHideDead={setHideDead}
              toggleItem={toggleItem}
              updateQty={updateQty}
              showAiPrompt={showAiPrompt}
              setShowAiPrompt={setShowAiPrompt}
              generateAIPrompt={generateAIPrompt}
              copyAIPrompt={copyAIPrompt}
              aiPromptCopied={aiPromptCopied}
              promptRef={promptRef}
              whatsappMessages={whatsappMessages}
              copiedBrand={copiedBrand}
              copyBrandMessage={copyBrandMessage}
              sendViaWhatsApp={sendViaWhatsApp}
              sending={sending}
            />
          )}
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// BASIC TAB
// ═══════════════════════════════════════════════════════════════════════
function BasicTab({
  data, items, selected, activeBrands, toggleBrand, brandStats,
  sortBy, setSortBy, sortDir, setSortDir, searchQ, setSearchQ,
  hideDead, setHideDead,
  toggleItem, updateQty, selectAll, clearAll,
  whatsappMessages, copiedBrand, copyBrandMessage,
  sendViaWhatsApp, sending,
}: {
  data: ReorderData;
  items: ReorderItem[];
  selected: Map<string, SelectedItem>;
  activeBrands: string[];
  toggleBrand: (v: string) => void;
  brandStats: [string, { total: number; out: number; critical: number; low: number; dead: number }][];
  sortBy: string;
  setSortBy: (v: 'stock' | 'velocity' | 'brand') => void;
  sortDir: string;
  setSortDir: (v: 'asc' | 'desc') => void;
  searchQ: string;
  setSearchQ: (v: string) => void;
  hideDead: boolean;
  setHideDead: (v: boolean) => void;
  toggleItem: (item: ReorderItem) => void;
  updateQty: (itemId: string, delta: number) => void;
  selectAll: () => void;
  clearAll: () => void;
  whatsappMessages: Map<string, string>;
  copiedBrand: string | null;
  copyBrandMessage: (brand: string, msg: string) => void;
  sendViaWhatsApp: (brand?: string) => void;
  sending: boolean;
}) {
  const [showSendPanel, setShowSendPanel] = useState(false);
  
  return (
    <div className="pb-32">
      {/* Summary Cards */}
      <div className="grid grid-cols-4 gap-2 p-3">
        <SummaryCard label="Out" value={data.summary.outOfStock} color="rose" />
        <SummaryCard label="Critical" value={data.summary.critical} color="orange" />
        <SummaryCard label="Low" value={data.summary.lowStock} color="amber" />
        <SummaryCard label="Dead" value={data.summary.deadStock} color="zinc" icon={<Skull size={12} />} />
      </div>

      {/* Search */}
      <div className="px-3 mb-2">
        <div className="relative">
          <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-400" />
          <input
            type="text"
            placeholder="Search items..."
            value={searchQ}
            onChange={e => setSearchQ(e.target.value)}
            className="w-full pl-8 pr-3 py-2 bg-zinc-50 border border-zinc-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>
      </div>

      {/* Advanced Brand Filter */}
      <BrandFilterBar
        brandStats={brandStats}
        activeBrands={activeBrands}
        toggleBrand={toggleBrand}
        totalCount={data.items.length}
        hideDead={hideDead}
        setHideDead={setHideDead}
      />

      {/* Sort & Select Bar */}
      <div className="px-3 mb-2 flex items-center justify-between gap-2">
        <div className="flex items-center gap-1.5">
          <button
            onClick={() => setSortBy('stock')}
            className={`px-2 py-1 rounded text-[10px] font-bold ${sortBy === 'stock' ? 'bg-indigo-100 text-indigo-700' : 'text-zinc-500'}`}
          >
            Stock
          </button>
          <button
            onClick={() => setSortBy('velocity')}
            className={`px-2 py-1 rounded text-[10px] font-bold ${sortBy === 'velocity' ? 'bg-indigo-100 text-indigo-700' : 'text-zinc-500'}`}
          >
            Velocity
          </button>
          <button
            onClick={() => setSortBy('brand')}
            className={`px-2 py-1 rounded text-[10px] font-bold ${sortBy === 'brand' ? 'bg-indigo-100 text-indigo-700' : 'text-zinc-500'}`}
          >
            Brand
          </button>
          <button 
            onClick={() => setSortDir(sortDir === 'asc' ? 'desc' : 'asc')}
            className="p-1 text-zinc-400"
          >
            <ArrowUpDown size={12} />
          </button>
        </div>
        <div className="flex items-center gap-2">
          <button onClick={selectAll} className="text-[10px] font-bold text-indigo-600">Select All</button>
          <span className="text-zinc-300">|</span>
          <button onClick={clearAll} className="text-[10px] font-bold text-zinc-400">Clear</button>
          <span className="text-[10px] font-bold text-zinc-600 bg-zinc-100 px-2 py-0.5 rounded-full">
            {selected.size} selected
          </span>
        </div>
      </div>

      {/* Item List */}
      <div className="px-3 space-y-2">
        {items.map(item => (
          <ItemCard
            key={item.itemId}
            item={item}
            isSelected={selected.has(item.itemId)}
            orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
            onToggle={() => toggleItem(item)}
            onUpdateQty={(d) => updateQty(item.itemId, d)}
            compact
          />
        ))}
        {items.length === 0 && (
          <div className="text-center py-8">
            <Package className="w-10 h-10 text-zinc-300 mx-auto mb-2" />
            <p className="text-zinc-400 text-sm">No items match your filters</p>
          </div>
        )}
      </div>

      {/* WhatsApp Send Panel (bottom fixed) */}
      {selected.size > 0 && (
        <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-zinc-200 shadow-2xl z-50">
          {!showSendPanel ? (
            <div className="p-3 flex items-center justify-between">
              <div>
                <span className="text-sm font-bold text-zinc-900">{selected.size} items selected</span>
                <span className="text-xs text-zinc-400 ml-2">
                  {whatsappMessages.size} brand{whatsappMessages.size !== 1 ? 's' : ''}
                </span>
              </div>
              <button
                onClick={() => setShowSendPanel(true)}
                className="flex items-center gap-2 bg-green-600 text-white px-4 py-2 rounded-xl font-bold text-sm shadow-lg active:scale-95 transition-transform"
              >
                <CheckCheck size={16} />
                Generate Order Options
              </button>
            </div>
          ) : (
            <div className="max-h-[65vh] overflow-y-auto">
              <div className="p-3 flex items-center justify-between border-b border-zinc-100 sticky top-0 bg-white z-10">
                <h3 className="font-bold text-zinc-900">WhatsApp — Brand Messages</h3>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => sendViaWhatsApp()}
                    disabled={sending}
                    className="flex items-center gap-1.5 bg-green-600 text-white px-3 py-1.5 rounded-lg text-xs font-bold disabled:opacity-50"
                  >
                    {sending ? <Loader2 size={12} className="animate-spin" /> : <Send size={12} />}
                    Send All
                  </button>
                  <button onClick={() => setShowSendPanel(false)} className="text-zinc-400 p-1">
                    <ChevronDown size={18} />
                  </button>
                </div>
              </div>
              <div className="p-3 space-y-3">
                {[...whatsappMessages].map(([brand, msg]) => (
                  <div key={brand} className="bg-zinc-50 rounded-xl overflow-hidden border border-zinc-100">
                    <div className="p-3 flex items-center justify-between gap-2">
                      <span className="font-bold text-sm text-zinc-900">{brand}</span>
                      <div className="flex items-center gap-1.5">
                        <button
                          onClick={() => copyBrandMessage(brand, msg)}
                          className={`flex items-center gap-1 px-2.5 py-1.5 rounded-lg text-[11px] font-bold transition-colors ${
                            copiedBrand === brand
                              ? 'bg-green-100 text-green-700'
                              : 'bg-zinc-200 text-zinc-700 active:bg-zinc-300'
                          }`}
                        >
                          {copiedBrand === brand ? <><CheckCheck size={10} /> Copied</> : <><Copy size={10} /> Copy</>}
                        </button>
                        <button
                          onClick={() => sendViaWhatsApp(brand)}
                          disabled={sending}
                          className="flex items-center gap-1 bg-green-600 text-white px-2.5 py-1.5 rounded-lg text-[11px] font-bold disabled:opacity-50 active:bg-green-700"
                        >
                          {sending ? <Loader2 size={10} className="animate-spin" /> : <Send size={10} />}
                          Send
                        </button>
                      </div>
                    </div>
                    <pre className="px-3 pb-3 text-[11px] text-zinc-600 whitespace-pre-wrap font-sans leading-relaxed">
                      {msg}
                    </pre>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// SMART TAB
// ═══════════════════════════════════════════════════════════════════════
function SmartTab({
  data, items, selected, healthScore, activeBrands, toggleBrand, brandStats,
  hideDead, setHideDead,
  toggleItem, updateQty,
  showAiPrompt, setShowAiPrompt, generateAIPrompt, copyAIPrompt,
  aiPromptCopied, promptRef,
  whatsappMessages, copiedBrand, copyBrandMessage,
  sendViaWhatsApp, sending,
}: {
  data: ReorderData;
  items: ReorderItem[];
  selected: Map<string, SelectedItem>;
  healthScore: number;
  activeBrands: string[];
  toggleBrand: (v: string) => void;
  brandStats: [string, { total: number; out: number; critical: number; low: number; dead: number }][];
  hideDead: boolean;
  setHideDead: (v: boolean) => void;
  toggleItem: (item: ReorderItem) => void;
  updateQty: (itemId: string, delta: number) => void;
  showAiPrompt: boolean;
  setShowAiPrompt: (v: boolean) => void;
  generateAIPrompt: () => string;
  copyAIPrompt: () => void;
  aiPromptCopied: boolean;
  promptRef: React.RefObject<HTMLTextAreaElement | null>;
  whatsappMessages: Map<string, string>;
  copiedBrand: string | null;
  copyBrandMessage: (brand: string, msg: string) => void;
  sendViaWhatsApp: (brand?: string) => void;
  sending: boolean;
}) {
  const [expandedSection, setExpandedSection] = useState<string | null>('critical');
  const [showWhatsApp, setShowWhatsApp] = useState(false);

  // Group items by status
  const outOfStock = items.filter(i => i.stockStatus === 'out');
  const critical = items.filter(i => i.stockStatus === 'critical');
  const lowStock = items.filter(i => i.stockStatus === 'low');
  const deadStock = items.filter(i => i.stockStatus === 'dead');
  const hotItems = items.filter(i => i.velocity === 'hot' && i.stockStatus !== 'dead');
  const pendingItems = items.filter(i => i.pendingOrder !== null);

  return (
    <div className="pb-32">
      {/* Health Score */}
      <div className="p-4">
        <div className="bg-gradient-to-br from-violet-50 to-indigo-50 rounded-2xl p-4 border border-violet-100">
          <div className="flex items-center justify-between mb-3">
            <div>
              <h3 className="text-sm font-bold text-violet-900">Stock Health Score</h3>
              <p className="text-[10px] text-violet-500 mt-0.5">{data.analysisMonths}-month analysis</p>
            </div>
            <div className={`w-16 h-16 rounded-full flex items-center justify-center text-xl font-black border-4 ${
              healthScore >= 80 ? 'border-emerald-300 text-emerald-700 bg-emerald-50' :
              healthScore >= 60 ? 'border-amber-300 text-amber-700 bg-amber-50' :
              healthScore >= 40 ? 'border-orange-300 text-orange-700 bg-orange-50' :
              'border-rose-300 text-rose-700 bg-rose-50'
            }`}>
              {healthScore}
            </div>
          </div>
          
          {/* Progress Bar */}
          <div className="w-full bg-white/60 rounded-full h-2 mb-3">
            <div
              className={`h-2 rounded-full transition-all ${
                healthScore >= 80 ? 'bg-emerald-500' :
                healthScore >= 60 ? 'bg-amber-500' :
                healthScore >= 40 ? 'bg-orange-500' :
                'bg-rose-500'
              }`}
              style={{ width: `${healthScore}%` }}
            />
          </div>

          {/* Quick Stats */}
          <div className="grid grid-cols-3 gap-2">
            <div className="bg-white/60 rounded-lg px-2 py-1.5 text-center">
              <div className="text-lg font-black text-rose-600">{data.summary.outOfStock + data.summary.critical}</div>
              <div className="text-[9px] font-bold text-zinc-500 uppercase">Urgent</div>
            </div>
            <div className="bg-white/60 rounded-lg px-2 py-1.5 text-center">
              <div className="text-lg font-black text-amber-600">{data.summary.lowStock}</div>
              <div className="text-[9px] font-bold text-zinc-500 uppercase">Low Stock</div>
            </div>
            <div className="bg-white/60 rounded-lg px-2 py-1.5 text-center">
              <div className="text-lg font-black text-zinc-500">{data.summary.deadStock}</div>
              <div className="text-[9px] font-bold text-zinc-500 uppercase">Dead Stock</div>
            </div>
          </div>
        </div>
      </div>

      {/* Advanced Brand Filter */}
      <div className="px-1">
        <BrandFilterBar
          brandStats={brandStats}
          activeBrands={activeBrands}
          toggleBrand={toggleBrand}
          totalCount={data.items.length}
          hideDead={hideDead}
          setHideDead={setHideDead}
          variant="violet"
        />
      </div>

      {/* AI Prompt Generator Button */}
      <div className="px-4 mb-4">
        <button
          onClick={() => setShowAiPrompt(!showAiPrompt)}
          className="w-full bg-gradient-to-r from-violet-600 to-purple-600 text-white rounded-xl p-3 flex items-center justify-between shadow-lg active:scale-[0.98] transition-transform"
        >
          <div className="flex items-center gap-2">
            <Sparkles size={18} />
            <div className="text-left">
              <div className="font-bold text-sm">AI Analysis Prompt</div>
              <div className="text-violet-200 text-[10px]">Generate data for Claude / ChatGPT</div>
            </div>
          </div>
          {showAiPrompt ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
        </button>

        {showAiPrompt && (
          <div className="mt-2 bg-zinc-900 rounded-xl overflow-hidden border border-zinc-700">
            <div className="p-3 flex items-center justify-between border-b border-zinc-700">
              <span className="text-zinc-300 text-xs font-medium">
                📋 Complete inventory data prompt — copy & paste to any AI
              </span>
              <button
                onClick={copyAIPrompt}
                className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-bold transition-colors ${
                  aiPromptCopied
                    ? 'bg-green-600 text-white'
                    : 'bg-violet-600 text-white hover:bg-violet-700'
                }`}
              >
                {aiPromptCopied ? <><CheckCheck size={12} /> Copied!</> : <><Copy size={12} /> Copy Prompt</>}
              </button>
            </div>
            <div className="p-3 max-h-64 overflow-y-auto">
              <pre className="text-green-400 text-[10px] font-mono whitespace-pre-wrap leading-relaxed">
                {generateAIPrompt().substring(0, 3000)}
                {generateAIPrompt().length > 3000 && (
                  <span className="text-zinc-500">{'\n\n'}... [{generateAIPrompt().length.toLocaleString()} chars total — click Copy to get full prompt]</span>
                )}
              </pre>
            </div>
            <textarea ref={promptRef} className="sr-only" readOnly tabIndex={-1} />
          </div>
        )}
      </div>

      {/* Collapsible Sections */}
      
      {/* 🔴 Out of Stock */}
      {outOfStock.length > 0 && (
        <SmartSection
          title="Out of Stock"
          icon={<AlertTriangle size={14} />}
          count={outOfStock.length}
          color="rose"
          expanded={expandedSection === 'oos'}
          onToggle={() => setExpandedSection(expandedSection === 'oos' ? null : 'oos')}
        >
          {outOfStock.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
            />
          ))}
        </SmartSection>
      )}

      {/* 🟠 Critical */}
      {critical.length > 0 && (
        <SmartSection
          title="Critical (1 Left)"
          icon={<AlertCircle size={14} />}
          count={critical.length}
          color="orange"
          expanded={expandedSection === 'critical'}
          onToggle={() => setExpandedSection(expandedSection === 'critical' ? null : 'critical')}
        >
          {critical.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
            />
          ))}
        </SmartSection>
      )}

      {/* 🔥 Hot Items */}
      {hotItems.length > 0 && (
        <SmartSection
          title="Hot Sellers"
          icon={<Flame size={14} />}
          count={hotItems.length}
          color="red"
          expanded={expandedSection === 'hot'}
          onToggle={() => setExpandedSection(expandedSection === 'hot' ? null : 'hot')}
        >
          {hotItems.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
            />
          ))}
        </SmartSection>
      )}

      {/* 🟡 Low Stock */}
      {lowStock.length > 0 && (
        <SmartSection
          title="Low Stock"
          icon={<Package size={14} />}
          count={lowStock.length}
          color="amber"
          expanded={expandedSection === 'low'}
          onToggle={() => setExpandedSection(expandedSection === 'low' ? null : 'low')}
        >
          {lowStock.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
            />
          ))}
        </SmartSection>
      )}

      {/* ⏳ Pending Orders */}
      {pendingItems.length > 0 && (
        <SmartSection
          title="Pending Orders"
          icon={<Clock size={14} />}
          count={pendingItems.length}
          color="blue"
          expanded={expandedSection === 'pending'}
          onToggle={() => setExpandedSection(expandedSection === 'pending' ? null : 'pending')}
        >
          {pendingItems.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
              showPending
            />
          ))}
        </SmartSection>
      )}

      {/* 💀 Dead Stock */}
      {deadStock.length > 0 && (
        <SmartSection
          title="Dead Stock"
          icon={<Skull size={14} />}
          count={deadStock.length}
          color="zinc"
          expanded={expandedSection === 'dead'}
          onToggle={() => setExpandedSection(expandedSection === 'dead' ? null : 'dead')}
        >
          <div className="px-4 py-2 bg-zinc-50 border-b border-zinc-200">
            <p className="text-[10px] text-zinc-500 font-medium">
              ⚠️ Items with stock but no sales in {DEAD_STOCK_DAYS_DISPLAY}+ days. Consider promotions or returns.
            </p>
          </div>
          {deadStock.map(item => (
            <ItemCard
              key={item.itemId}
              item={item}
              isSelected={selected.has(item.itemId)}
              orderQty={selected.get(item.itemId)?.orderQty || item.suggestedQty}
              onToggle={() => toggleItem(item)}
              onUpdateQty={(d) => updateQty(item.itemId, d)}
            />
          ))}
        </SmartSection>
      )}

      {/* WhatsApp Send Panel for Smart Tab */}
      {selected.size > 0 && (
        <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-zinc-200 shadow-2xl z-50">
          {!showWhatsApp ? (
            <div className="p-3 flex items-center justify-between">
              <div>
                <span className="text-sm font-bold text-zinc-900">{selected.size} items to order</span>
                <span className="text-xs text-zinc-400 ml-2">
                  {whatsappMessages.size} brand{whatsappMessages.size !== 1 ? 's' : ''}
                </span>
              </div>
              <button
                onClick={() => setShowWhatsApp(true)}
                className="flex items-center gap-2 bg-green-600 text-white px-4 py-2 rounded-xl font-bold text-sm shadow-lg active:scale-95 transition-transform"
              >
                <CheckCheck size={16} />
                Generate Order Options
              </button>
            </div>
          ) : (
            <div className="max-h-[65vh] overflow-y-auto">
              <div className="p-3 flex items-center justify-between border-b border-zinc-100 sticky top-0 bg-white z-10">
                <h3 className="font-bold text-zinc-900">WhatsApp — Brand Messages</h3>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => sendViaWhatsApp()}
                    disabled={sending}
                    className="flex items-center gap-1.5 bg-green-600 text-white px-3 py-1.5 rounded-lg text-xs font-bold disabled:opacity-50"
                  >
                    {sending ? <Loader2 size={12} className="animate-spin" /> : <Send size={12} />}
                    Send All
                  </button>
                  <button onClick={() => setShowWhatsApp(false)} className="text-zinc-400 p-1">
                    <ChevronDown size={18} />
                  </button>
                </div>
              </div>
              <div className="p-3 space-y-3">
                {[...whatsappMessages].map(([brand, msg]) => (
                  <div key={brand} className="bg-zinc-50 rounded-xl overflow-hidden border border-zinc-100">
                    <div className="p-3 flex items-center justify-between gap-2">
                      <span className="font-bold text-sm text-zinc-900">{brand}</span>
                      <div className="flex items-center gap-1.5">
                        <button
                          onClick={() => copyBrandMessage(brand, msg)}
                          className={`flex items-center gap-1 px-2.5 py-1.5 rounded-lg text-[11px] font-bold transition-colors ${
                            copiedBrand === brand
                              ? 'bg-green-100 text-green-700'
                              : 'bg-zinc-200 text-zinc-700 active:bg-zinc-300'
                          }`}
                        >
                          {copiedBrand === brand ? <><CheckCheck size={10} /> Copied</> : <><Copy size={10} /> Copy</>}
                        </button>
                        <button
                          onClick={() => sendViaWhatsApp(brand)}
                          disabled={sending}
                          className="flex items-center gap-1 bg-green-600 text-white px-2.5 py-1.5 rounded-lg text-[11px] font-bold disabled:opacity-50 active:bg-green-700"
                        >
                          {sending ? <Loader2 size={10} className="animate-spin" /> : <Send size={10} />}
                          Send
                        </button>
                      </div>
                    </div>
                    <pre className="px-3 pb-3 text-[11px] text-zinc-600 whitespace-pre-wrap font-sans leading-relaxed">{msg}</pre>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

const DEAD_STOCK_DAYS_DISPLAY = 180;

// ═══════════════════════════════════════════════════════════════════════
// ITEM CARD
// ═══════════════════════════════════════════════════════════════════════
function ItemCard({
  item, isSelected, orderQty, onToggle, onUpdateQty, compact, showPending,
}: {
  item: ReorderItem;
  isSelected: boolean;
  orderQty: number;
  onToggle: () => void;
  onUpdateQty: (delta: number) => void;
  compact?: boolean;
  showPending?: boolean;
}) {
  return (
    <div
      onClick={onToggle}
      className={`bg-white border rounded-xl overflow-hidden transition-colors cursor-pointer active:scale-[0.99] ${
        isSelected ? 'border-indigo-300 bg-indigo-50/30' : 'border-zinc-100'
      }`}
    >
      <div className="p-3">
        <div className="flex items-start gap-2">
          {/* Checkbox */}
          <div className="mt-0.5 shrink-0">
            {isSelected ? (
              <CheckCircle2 size={20} className="text-indigo-600" />
            ) : (
              <Circle size={20} className="text-zinc-300" />
            )}
          </div>

          {/* Content */}
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-1.5 mb-0.5 flex-wrap">
              <StockBadge status={item.stockStatus} qty={item.currentStock} />
              <VelocityBadge velocity={item.velocity} />
              <span className="text-[9px] font-bold text-zinc-400 bg-zinc-50 px-1.5 py-0.5 rounded uppercase">
                {item.brand}
              </span>
            </div>
            
            <h4 className="text-sm font-medium text-zinc-900 leading-snug mt-1">
              {item.description}
            </h4>

            {/* Sales Data */}
            {!compact && (
              <div className="flex items-center gap-3 mt-1.5">
                <span className="text-[10px] text-zinc-500">
                  7d: <span className="font-bold text-zinc-700">{item.sales.last7}</span>
                </span>
                <span className="text-[10px] text-zinc-500">
                  30d: <span className="font-bold text-zinc-700">{item.sales.last30}</span>
                </span>
                <span className="text-[10px] text-zinc-500">
                  90d: <span className="font-bold text-zinc-700">{item.sales.last90}</span>
                </span>
                {item.sales.lastSaleDate && (
                  <span className="text-[10px] text-zinc-400">
                    Last: {new Date(item.sales.lastSaleDate).toLocaleDateString('en-US', { day: 'numeric', month: 'short' })}
                  </span>
                )}
              </div>
            )}

            {compact && (
              <div className="flex items-center gap-2 mt-1">
                <span className="text-[10px] text-zinc-500">
                  Sold 30d: <span className="font-bold">{item.sales.last30}</span>
                </span>
                <span className="text-[10px] text-zinc-400">
                  Stock: {item.currentStock}/{item.targetStock}
                </span>
              </div>
            )}

            {/* Pending Order Warning */}
            {(showPending || !compact) && item.pendingOrder && (
              <div className="mt-1.5 flex items-center gap-1.5 text-[10px] bg-blue-50 text-blue-700 px-2 py-1 rounded-lg w-fit">
                <Clock size={10} />
                <span className="font-bold">
                  Pending: {item.pendingOrder.orderedQty} ordered {new Date(item.pendingOrder.orderDate).toLocaleDateString('en-US', { day: 'numeric', month: 'short' })}
                </span>
              </div>
            )}

            {/* Last GRN */}
            {!compact && item.lastGRN && (
              <div className="mt-1 text-[10px] text-zinc-400">
                GRN: {item.lastGRN.no} — {new Date(item.lastGRN.date).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}
                {item.grnAgeDays !== null && (
                  <span className={`ml-1 font-bold ${
                    item.grnAgeDays > 365 ? 'text-rose-500' : item.grnAgeDays > 90 ? 'text-amber-500' : 'text-zinc-500'
                  }`}>
                    ({item.grnAgeDays}d ago)
                  </span>
                )}
              </div>
            )}
          </div>

          {/* Qty Controls */}
          {isSelected && (
            <div className="flex flex-col items-center gap-0.5 shrink-0" onClick={e => e.stopPropagation()}>
              <span className="text-[8px] font-bold text-zinc-400 uppercase">Order</span>
              <div className="flex items-center bg-zinc-100 rounded-lg overflow-hidden">
                <button
                  onClick={(e) => { e.stopPropagation(); onUpdateQty(-1); }}
                  className="w-7 h-7 flex items-center justify-center text-zinc-600 hover:bg-zinc-200 active:bg-zinc-300 transition-colors"
                >
                  <Minus size={12} />
                </button>
                <span className="w-7 text-center text-sm font-bold text-indigo-700">{orderQty}</span>
                <button
                  onClick={(e) => { e.stopPropagation(); onUpdateQty(1); }}
                  className="w-7 h-7 flex items-center justify-center text-zinc-600 hover:bg-zinc-200 active:bg-zinc-300 transition-colors"
                >
                  <Plus size={12} />
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// ADVANCED BRAND FILTER
// ═══════════════════════════════════════════════════════════════════════
function BrandFilterBar({
  brandStats,
  activeBrands,
  toggleBrand,
  totalCount,
  hideDead,
  setHideDead,
  variant = 'indigo',
}: {
  brandStats: [string, { total: number; out: number; critical: number; low: number; dead: number }][];
  activeBrands: string[];
  toggleBrand: (v: string) => void;
  totalCount: number;
  hideDead: boolean;
  setHideDead: (v: boolean) => void;
  variant?: 'indigo' | 'violet';
}) {
  const activeColor = variant === 'violet' ? 'bg-violet-600 text-white' : 'bg-indigo-600 text-white';
  const deadCount = brandStats.reduce((s, [, v]) => s + v.dead, 0);

  return (
    <div className="px-3 mb-2">
      {/* Dead Stock Toggle */}
      <div className="flex items-center justify-between mb-2">
        <div className="flex items-center gap-2">
          <Filter size={12} className="text-zinc-400" />
          <span className="text-[10px] font-bold text-zinc-500 uppercase tracking-wider">Brand Filter</span>
        </div>
        <button
          onClick={() => setHideDead(!hideDead)}
          className={`flex items-center gap-1.5 px-2.5 py-1 rounded-full text-[10px] font-bold transition-colors ${
            hideDead
              ? 'bg-zinc-800 text-white'
              : 'bg-zinc-100 text-zinc-500 border border-zinc-200'
          }`}
        >
          {hideDead ? <EyeOff size={10} /> : <Eye size={10} />}
          Dead Stock ({deadCount})
          {hideDead && <span className="text-zinc-400">hidden</span>}
        </button>
      </div>

      {/* Brand Chips */}
      <div className="overflow-x-auto pb-1">
        <div className="flex gap-1.5 min-w-max">
          <button
            onClick={() => toggleBrand('all')}
            className={`px-3 py-1.5 rounded-full text-xs font-bold transition-colors ${
              activeBrands.length === 0 ? activeColor : 'bg-zinc-100 text-zinc-600 hover:bg-zinc-200'
            }`}
          >
            All ({totalCount})
          </button>
          {brandStats.map(([brand, stats]) => {
            const urgentDots = stats.out + stats.critical;
            const isSelected = activeBrands.includes(brand);
            return (
              <button
                key={brand}
                onClick={() => toggleBrand(brand)}
                className={`px-3 py-1.5 rounded-full text-xs font-bold whitespace-nowrap transition-colors flex items-center gap-1 ${
                  isSelected ? activeColor : 'bg-zinc-100 text-zinc-600 hover:bg-zinc-200'
                }`}
              >
                {brand}
                <span className="opacity-75">({stats.total - (hideDead ? stats.dead : 0)})</span>
                {urgentDots > 0 && (
                  <span className={`w-4 h-4 rounded-full text-[8px] font-black flex items-center justify-center ${
                    isSelected ? 'bg-white/30 text-white' : 'bg-rose-500 text-white'
                  }`}>
                    {urgentDots}
                  </span>
                )}
              </button>
            );
          })}
        </div>
      </div>
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// SMART SECTION (collapsible)
// ═══════════════════════════════════════════════════════════════════════
function SmartSection({
  title, icon, count, color, expanded, onToggle, children,
}: {
  title: string;
  icon: React.ReactNode;
  count: number;
  color: string;
  expanded: boolean;
  onToggle: () => void;
  children: React.ReactNode;
}) {
  const colorMap: Record<string, string> = {
    rose: 'bg-rose-50 text-rose-700 border-rose-200',
    orange: 'bg-orange-50 text-orange-700 border-orange-200',
    amber: 'bg-amber-50 text-amber-700 border-amber-200',
    red: 'bg-red-50 text-red-700 border-red-200',
    blue: 'bg-blue-50 text-blue-700 border-blue-200',
    zinc: 'bg-zinc-100 text-zinc-600 border-zinc-200',
  };
  const cls = colorMap[color] || colorMap.zinc;

  return (
    <div className="mx-4 mb-3 rounded-xl border overflow-hidden border-zinc-100">
      <button
        onClick={onToggle}
        className={`w-full p-3 flex items-center justify-between ${cls}`}
      >
        <div className="flex items-center gap-2">
          {icon}
          <span className="font-bold text-sm">{title}</span>
          <span className="text-xs opacity-75">({count})</span>
        </div>
        {expanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
      </button>
      {expanded && (
        <div className="p-2 space-y-2 bg-white">
          {children}
        </div>
      )}
    </div>
  );
}

// ═══════════════════════════════════════════════════════════════════════
// SUMMARY CARD
// ═══════════════════════════════════════════════════════════════════════
function SummaryCard({ label, value, color, icon }: { label: string; value: number; color: string; icon?: React.ReactNode }) {
  const colorMap: Record<string, string> = {
    rose: 'text-rose-700 bg-rose-50 border-rose-200',
    orange: 'text-orange-700 bg-orange-50 border-orange-200',
    amber: 'text-amber-700 bg-amber-50 border-amber-200',
    zinc: 'text-zinc-600 bg-zinc-50 border-zinc-200',
  };
  const cls = colorMap[color] || colorMap.zinc;

  return (
    <div className={`rounded-xl border p-2 text-center ${cls}`}>
      <div className="text-xl font-black flex items-center justify-center gap-1">{icon}{value}</div>
      <div className="text-[9px] font-bold uppercase tracking-wider">{label}</div>
    </div>
  );
}
