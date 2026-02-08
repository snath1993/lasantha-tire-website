'use client';

import { useState } from 'react';
import { Send, Trash2, FileText, X, Share2 } from 'lucide-react';

interface QuoteItem {
  ItemId: string;
  Description: string;
  Brand: string;
  Price: number;
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

  const total = items.reduce((sum, item) => sum + (item.Price * item.Quantity), 0);

  const generateMessage = () => {
    let message = `*Lasantha Tire Service*\n`;
    message += `Quotation for ${customerName || 'Customer'}\n`;
    message += `------------------------\n`;
    
    items.forEach(item => {
      message += `${item.Brand} ${item.Description}\n`;
      message += `Qty: ${item.Quantity} x Rs ${item.Price.toLocaleString()}\n`;
      message += `Amount: Rs ${(item.Price * item.Quantity).toLocaleString()}\n\n`;
    });
    
    message += `------------------------\n`;
    message += `*Total: Rs ${total.toLocaleString()}*\n`;
    message += `\nThank you!`;
    return message;
  };

  const handleShare = async () => {
    const message = generateMessage();
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

  const handleSendWhatsApp = () => {
    if (items.length === 0) return;

    const message = generateMessage();

    // Format phone number (remove leading 0, add 94)
    let formattedPhone = phone.replace(/\D/g, '');
    if (formattedPhone.startsWith('0')) {
      formattedPhone = '94' + formattedPhone.substring(1);
    }

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
        className="fixed bottom-20 right-4 bg-blue-600 text-white p-4 rounded-full shadow-lg shadow-blue-600/40 z-50 flex items-center gap-2 animate-bounce"
      >
        <FileText className="w-6 h-6" />
        <span className="font-bold">{items.length}</span>
      </button>
    );
  }

  return (
    <div className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-end sm:items-center justify-center p-0 sm:p-4">
      <div className="bg-slate-900 w-full sm:max-w-md rounded-t-2xl sm:rounded-2xl border border-slate-700 shadow-2xl max-h-[90vh] flex flex-col">
        {/* Header */}
        <div className="p-4 border-b border-slate-800 flex items-center justify-between bg-slate-800/50 rounded-t-2xl">
          <h3 className="text-lg font-bold text-white flex items-center gap-2">
            <FileText className="w-5 h-5 text-blue-400" />
            Quick Quotation
          </h3>
          <button onClick={() => setIsOpen(false)} className="text-slate-400 hover:text-white">
            <X className="w-6 h-6" />
          </button>
        </div>

        {/* Items List */}
        <div className="flex-1 overflow-y-auto p-4 space-y-3">
          {items.length === 0 ? (
            <div className="text-center text-slate-500 py-8">
              No items in quotation
            </div>
          ) : (
            items.map((item, idx) => (
              <div key={idx} className="bg-slate-800 rounded-xl p-3 flex justify-between items-start border border-slate-700">
                <div>
                  <div className="text-white font-medium text-sm">{item.Brand} {item.Description}</div>
                  <div className="text-slate-400 text-xs mt-1">
                    {item.Quantity} x Rs {item.Price.toLocaleString()}
                  </div>
                </div>
                <div className="flex flex-col items-end gap-2">
                  <div className="text-white font-bold text-sm">
                    Rs {(item.Price * item.Quantity).toLocaleString()}
                  </div>
                  <button 
                    onClick={() => onRemoveItem(idx)}
                    className="text-red-400 hover:text-red-300 p-1"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            ))
          )}
        </div>

        {/* Footer / Actions */}
        <div className="p-4 bg-slate-800/50 border-t border-slate-800 space-y-4 rounded-b-2xl">
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
            
            <div className="grid grid-cols-2 gap-3">
              <button
                onClick={onClear}
                className="px-4 py-3 bg-slate-700 hover:bg-slate-600 text-white rounded-xl font-medium transition-colors"
              >
                Clear
              </button>
              <button
                onClick={handleShare}
                className="px-4 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold transition-colors flex items-center justify-center gap-2"
              >
                <Share2 className="w-5 h-5" />
                Share
              </button>
            </div>
            
            <button
              onClick={handleSendWhatsApp}
              disabled={items.length === 0 || !phone}
              className="w-full px-4 py-3 bg-green-600 hover:bg-green-700 disabled:bg-slate-700 disabled:text-slate-500 text-white rounded-xl font-bold transition-colors flex items-center justify-center gap-2"
            >
              <Send className="w-5 h-5" />
              Send WhatsApp
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
