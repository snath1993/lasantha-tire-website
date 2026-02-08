'use client';

import { useState } from 'react';
import { Send, Trash2, FileText, X, Share2, Wrench, Disc, Loader2 } from 'lucide-react';
import { saveQuotationToDb, normalizePhoneNumber, formatPhoneForWhatsApp } from '@/lib/quotationShare';

const SERVICE_IDS = ['120', '121', '161', '144', '122', '114'];

interface QuoteItem {
  ItemId: string;
  Description: string;
  Brand: string;
  Price?: number;
  SellingPrice?: number;
  Quantity: number;
}

export default function QuickQuote({ 
  items, 
  onRemoveItem, 
  onClear 
}: { 
  items: QuoteItem[], 
  onRemoveItem: (index: number) => void,
  onClear: () => void 
}) {
  const [isOpen, setIsOpen] = useState(false);
  const [phone, setPhone] = useState('');
  const [customerName, setCustomerName] = useState('');
  const [showSaveSuccess, setShowSaveSuccess] = useState(false);
  const [quantities, setQuantities] = useState<{[key: number]: number}>({});
  const [isSaving, setIsSaving] = useState(false);

  const getPrice = (item: QuoteItem) => {
    return item.Price || item.SellingPrice || 0;
  };

  const updateQuantity = (index: number, delta: number) => {
    const currentQty = quantities[index] || items[index]?.Quantity || 1;
    const newQty = Math.max(1, currentQty + delta);
    setQuantities({...quantities, [index]: newQty});
  };

  const getItemQuantity = (index: number) => {
    return quantities[index] || items[index]?.Quantity || 1;
  };

  const saveQuote = () => {
    const quote = {
      date: new Date().toISOString(),
      customer: customerName,
      items: items.map((item, idx) => ({...item, Quantity: getItemQuantity(idx)})),
      total: total
    };
    const saved = JSON.parse(localStorage.getItem('savedQuotes') || '[]');
    saved.unshift(quote);
    localStorage.setItem('savedQuotes', JSON.stringify(saved.slice(0, 10)));
    setShowSaveSuccess(true);
    setTimeout(() => setShowSaveSuccess(false), 2000);
  };

  const total = items.reduce((sum, item, idx) => {
    const qty = getItemQuantity(idx);
    return sum + (getPrice(item) * qty);
  }, 0);

  const buildPayloadItems = () => items.map((item, idx) => ({
    itemId: item.ItemId,
    description: item.Description,
    brand: item.Brand,
    price: getPrice(item),
    quantity: getItemQuantity(idx),
    isService: SERVICE_IDS.includes(item.ItemId)
  }));

  const deriveTyreSize = () => {
    return items.find(i => !SERVICE_IDS.includes(i.ItemId))?.Description?.match(/\d+\/?\d*R?\d+/)?.[0] || '';
  };

  const generateMessage = (quotationNumber?: string, bookingUrl?: string) => {
    let message = `*Lasantha Tire Service*\n`;
    message += `Quotation for ${customerName || 'Customer'}\n`;
    
    if (quotationNumber) {
      message += `📋 *Ref: ${quotationNumber}*\n`;
    }
    
    message += `------------------------\n`;
    
    const tyreItems = items.map((item, idx) => ({...item, originalIdx: idx})).filter(i => !SERVICE_IDS.includes(i.ItemId));
    const serviceItems = items.map((item, idx) => ({...item, originalIdx: idx})).filter(i => SERVICE_IDS.includes(i.ItemId));

    if (tyreItems.length > 0) {
        message += `*Tyres*\n`;
        tyreItems.forEach((item) => {
            const qty = getItemQuantity(item.originalIdx);
            const price = getPrice(item);
            
            message += `🛠️ ${item.Description}\n`;
            message += `🏷️ ${item.Brand}\n`;
            message += `💰 Rs. ${price.toLocaleString()}/=\n`;
            
            if (qty > 1) {
                message += `Qty: ${qty} | Total: Rs ${(price * qty).toLocaleString()}\n`;
            }
            message += `\n`;
        });
    }

    if (serviceItems.length > 0) {
        message += `*Services*\n`;
        serviceItems.forEach((item) => {
            const qty = getItemQuantity(item.originalIdx);
            const price = getPrice(item);
            message += `${item.Description}\n`;
            message += `Qty: ${qty} x Rs ${price.toLocaleString()}\n`;
            message += `Amount: Rs ${(price * qty).toLocaleString()}\n\n`;
        });
    }
    
    // Only show total if there is 1 or fewer tyre items
    if (tyreItems.length <= 1) {
        message += `------------------------\n`;
        message += `*Total: Rs ${total.toLocaleString()}*\n`;
    }
    
    if (bookingUrl) {
      message += `\n📅 *Book Your Appointment:*\n${bookingUrl}\n`;
    }
    
    message += `\nThank you!`;
    return message;
  };

  const handleShare = async () => {
    if (items.length === 0) return;
    
    setIsSaving(true);
    
    const baseMessage = generateMessage();
    const result = await saveQuotationToDb({
      customerPhone: normalizePhoneNumber(phone),
      customerName: customerName || 'Customer',
      tyreSize: deriveTyreSize(),
      items: buildPayloadItems(),
      totalAmount: total,
      messageContent: baseMessage,
      source: 'Mobile App'
    });
    const quotationNumber = result?.quotationNumber || '';
    const bookingUrl = result?.bookingUrl || '';
    const message = generateMessage(quotationNumber, bookingUrl);
    
    setIsSaving(false);
    
    if (navigator.share) {
      try {
        await navigator.share({
          title: 'Lasantha Tire Quotation',
          text: message,
        });
      } catch (err) {
        console.error('Error sharing:', err);
      }
    } else {
      // Fallback for desktop or unsupported browsers
      try {
        await navigator.clipboard.writeText(message);
        alert('Quotation copied to clipboard!');
      } catch (err) {
        alert('Sharing not supported');
      }
    }
  };

  const handleSendWhatsApp = async () => {
    if (items.length === 0) return;

    setIsSaving(true);
    
    const baseMessage = generateMessage();
    const result = await saveQuotationToDb({
      customerPhone: normalizePhoneNumber(phone),
      customerName: customerName || 'Customer',
      tyreSize: deriveTyreSize(),
      items: buildPayloadItems(),
      totalAmount: total,
      messageContent: baseMessage,
      source: 'Mobile App'
    });
    const quotationNumber = result?.quotationNumber || '';
    const bookingUrl = result?.bookingUrl || '';
    const message = generateMessage(quotationNumber, bookingUrl);
    
    setIsSaving(false);

    const formattedPhone = formatPhoneForWhatsApp(phone);
    const url = `https://wa.me/${formattedPhone}?text=${encodeURIComponent(message)}`;
    window.open(url, '_blank');
  };

  if (items.length === 0 && !isOpen) {
    return null;
  }

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="fixed bottom-20 right-4 bg-gradient-to-br from-blue-600 to-indigo-700 text-white p-4 rounded-full shadow-2xl shadow-blue-600/40 z-50 flex items-center gap-2 hover:scale-110 active:scale-95 transition-transform"
      >
        <div className="relative">
          <FileText className="w-6 h-6" />
          <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center animate-pulse">
            {items.length}
          </span>
        </div>
      </button>
    );
  }

  return (
    <div className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex flex-col sm:items-center sm:justify-center p-0 sm:p-4">
      <div className="bg-slate-900 w-full h-full sm:h-auto sm:max-h-[90vh] sm:max-w-md sm:rounded-2xl border-x-0 sm:border border-slate-700 shadow-2xl flex flex-col animate-in slide-in-from-bottom duration-200">
        {/* Header */}
        <div className="p-4 border-b border-slate-800 flex items-center justify-between bg-slate-800/50 sm:rounded-t-2xl shrink-0">
          <h3 className="text-lg font-bold text-white flex items-center gap-2">
            <FileText className="w-5 h-5 text-blue-400" />
            Quick Quotation
          </h3>
          <button onClick={() => setIsOpen(false)} className="text-slate-400 hover:text-white">
            <X className="w-6 h-6" />
          </button>
        </div>

        {/* Items List */}
        <div className="flex-1 overflow-y-auto p-4 space-y-6">
          {items.length === 0 ? (
            <div className="text-center text-slate-500 py-8">
              No items in quotation
            </div>
          ) : (
            <>
                {/* Tyres Section */}
                {items.some(i => !SERVICE_IDS.includes(i.ItemId)) && (
                    <div className="space-y-3">
                        <div className="flex items-center gap-2 text-slate-400 px-1">
                            <Disc className="w-4 h-4" />
                            <h4 className="text-xs font-bold uppercase tracking-wider">Tyres</h4>
                        </div>
                        {items.map((item, idx) => ({ item, idx }))
                             .filter(({ item }) => !SERVICE_IDS.includes(item.ItemId))
                             .map(({ item, idx }) => {
                                const qty = getItemQuantity(idx);
                                const price = getPrice(item);
                                return (
                                    <div key={idx} className="bg-gradient-to-br from-slate-800 to-slate-900 rounded-2xl p-4 border border-slate-700 hover:border-blue-500/50 transition-all">
                                        <div className="flex justify-between items-start mb-3">
                                        <div className="flex-1">
                                            <div className="text-white font-bold text-sm mb-1">{item.Brand}</div>
                                            <div className="text-slate-400 text-xs">{item.Description}</div>
                                        </div>
                                        <button 
                                            onClick={() => onRemoveItem(idx)}
                                            className="text-red-400 hover:text-red-300 hover:bg-red-500/10 p-2 rounded-lg transition-colors"
                                        >
                                            <Trash2 className="w-4 h-4" />
                                        </button>
                                        </div>
                                        
                                        <div className="flex items-center justify-between">
                                        <div className="flex items-center gap-2 bg-slate-700/50 rounded-lg p-1">
                                            <button
                                            onClick={() => updateQuantity(idx, -1)}
                                            className="w-7 h-7 bg-slate-600 hover:bg-slate-500 text-white rounded flex items-center justify-center font-bold transition-colors"
                                            >
                                            -
                                            </button>
                                            <span className="text-white font-bold text-sm w-8 text-center">{qty}</span>
                                            <button
                                            onClick={() => updateQuantity(idx, 1)}
                                            className="w-7 h-7 bg-blue-600 hover:bg-blue-500 text-white rounded flex items-center justify-center font-bold transition-colors"
                                            >
                                            +
                                            </button>
                                        </div>
                                        
                                        <div className="text-right">
                                            <div className="text-xs text-slate-400">{qty} x Rs {price.toLocaleString()}</div>
                                            <div className="text-white font-bold text-lg">
                                            Rs {(price * qty).toLocaleString()}
                                            </div>
                                        </div>
                                        </div>
                                    </div>
                                );
                             })}
                    </div>
                )}

                {/* Services Section */}
                {items.some(i => SERVICE_IDS.includes(i.ItemId)) && (
                    <div className="space-y-3">
                        <div className="flex items-center gap-2 text-indigo-400 px-1">
                            <Wrench className="w-4 h-4" />
                            <h4 className="text-xs font-bold uppercase tracking-wider">Services</h4>
                        </div>
                        {items.map((item, idx) => ({ item, idx }))
                             .filter(({ item }) => SERVICE_IDS.includes(item.ItemId))
                             .map(({ item, idx }) => {
                                const qty = getItemQuantity(idx);
                                const price = getPrice(item);
                                return (
                                    <div key={idx} className="bg-indigo-900/20 rounded-2xl p-4 border border-indigo-500/30 transition-all">
                                        <div className="flex justify-between items-start mb-3">
                                        <div className="flex-1">
                                            <div className="text-white font-bold text-sm mb-1">{item.Description}</div>
                                            {item.ItemId === '122' && (
                                                <div className="text-[10px] text-indigo-400 font-bold mt-1">Rs 500 / Tyre</div>
                                            )}
                                        </div>
                                        <button 
                                            onClick={() => onRemoveItem(idx)}
                                            className="text-red-400 hover:text-red-300 hover:bg-red-500/10 p-2 rounded-lg transition-colors"
                                        >
                                            <Trash2 className="w-4 h-4" />
                                        </button>
                                        </div>
                                        
                                        <div className="flex items-center justify-between">
                                        <div className="flex items-center gap-2 bg-slate-700/50 rounded-lg p-1">
                                            <button
                                            onClick={() => updateQuantity(idx, -1)}
                                            className="w-7 h-7 bg-slate-600 hover:bg-slate-500 text-white rounded flex items-center justify-center font-bold transition-colors"
                                            >
                                            -
                                            </button>
                                            <span className="text-white font-bold text-sm w-8 text-center">{qty}</span>
                                            <button
                                            onClick={() => updateQuantity(idx, 1)}
                                            className="w-7 h-7 bg-indigo-600 hover:bg-indigo-500 text-white rounded flex items-center justify-center font-bold transition-colors"
                                            >
                                            +
                                            </button>
                                        </div>
                                        
                                        <div className="text-right">
                                            <div className="text-xs text-slate-400">{qty} x Rs {price.toLocaleString()}</div>
                                            <div className="text-white font-bold text-lg">
                                            Rs {(price * qty).toLocaleString()}
                                            </div>
                                        </div>
                                        </div>
                                    </div>
                                );
                             })}
                    </div>
                )}
            </>
          )}
        </div>

        {/* Footer / Actions */}
        <div className="p-4 bg-slate-800/50 border-t border-slate-800 space-y-4 rounded-b-2xl pb-24">
          <div className="flex justify-between items-center text-lg font-bold text-white">
            <span>Total</span>
            <span>Rs {total.toLocaleString()}</span>
          </div>

          <div className="space-y-3">
            <input
              type="text"
              placeholder="Customer Name (Optional)"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:border-blue-500 outline-none"
            />
            <input
              type="tel"
              placeholder="WhatsApp Number (e.g. 0771234567)"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:border-blue-500 outline-none"
            />
            
            <div className="grid grid-cols-3 gap-2">
              <button
                onClick={saveQuote}
                disabled={items.length === 0}
                className="px-3 py-3 bg-indigo-600 hover:bg-indigo-700 disabled:bg-slate-700 text-white rounded-xl font-medium transition-colors text-sm"
              >
                {showSaveSuccess ? '✓ Saved' : 'Save'}
              </button>
              <button
                onClick={onClear}
                className="px-3 py-3 bg-slate-700 hover:bg-slate-600 text-white rounded-xl font-medium transition-colors text-sm"
              >
                Clear
              </button>
              <button
                onClick={handleShare}
                disabled={items.length === 0 || isSaving}
                className="px-3 py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-700 disabled:text-slate-500 text-white rounded-xl font-bold transition-colors flex items-center justify-center gap-2 text-sm"
              >
                {isSaving ? <Loader2 className="w-4 h-4 animate-spin" /> : <Share2 className="w-4 h-4" />}
                {isSaving ? 'Saving...' : 'Share'}
              </button>
            </div>
            
            <button
              onClick={handleSendWhatsApp}
              disabled={items.length === 0 || !phone || isSaving}
              className="w-full px-4 py-3 bg-green-600 hover:bg-green-700 disabled:bg-slate-700 disabled:text-slate-500 text-white rounded-xl font-bold transition-colors flex items-center justify-center gap-2"
            >
              {isSaving ? <Loader2 className="w-5 h-5 animate-spin" /> : <Send className="w-5 h-5" />}
              {isSaving ? 'Saving...' : 'Send WhatsApp'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
