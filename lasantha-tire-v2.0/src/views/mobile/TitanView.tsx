'use client';

import { useState, useRef, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Send, Bot, Loader2, ArrowLeft, ShieldCheck } from 'lucide-react';
import { useRouter } from 'next/navigation';

interface Message {
  id: string;
  text: string;
  sender: 'user' | 'bot';
  timestamp: Date;
}

export default function TitanView() {
  const router = useRouter();
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const [userPhone, setUserPhone] = useState(''); 

  useEffect(() => {
    const stored = sessionStorage.getItem('titan_user_phone');
    if (stored) setUserPhone(stored);
  }, []);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const handleSend = async () => {
    if (!input.trim()) return;

    let currentPhone = userPhone;
    if (!currentPhone) {
        const phone = prompt("🔒 Security Check: Enter Admin Phone Number (e.g., 0777078700)");
        if (!phone) return;
        currentPhone = phone;
        setUserPhone(phone);
        sessionStorage.setItem('titan_user_phone', phone);
    }

    const userMsg: Message = {
      id: Date.now().toString(),
      text: input,
      sender: 'user',
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMsg]);
    setInput('');
    setIsLoading(true);

    try {
      // Use our new local proxy route
      const response = await fetch('/api/titan', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ 
          message: userMsg.text,
          sender: currentPhone 
        }),
      });

      const data = await response.json();
      
      const botMsg: Message = {
        id: (Date.now() + 1).toString(),
        text: data.response || "No response.",
        sender: 'bot',
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, botMsg]);
    } catch (error) {
      console.error('Chat Error:', error);
      const errorMsg: Message = {
        id: (Date.now() + 1).toString(),
        text: "Error communicating with Titan.",
        sender: 'bot',
        timestamp: new Date()
      };
      setMessages(prev => [...prev, errorMsg]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex flex-col h-[calc(100vh-80px)] bg-slate-50">
      {/* Header */}
      <div className="bg-white px-4 py-3 flex items-center justify-between border-b border-slate-200 sticky top-0 z-10">
        <div className="flex items-center gap-3">
          <button onClick={() => router.back()} className="p-1 -ml-1 text-slate-500">
            <ArrowLeft size={20} />
          </button>
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-full bg-indigo-600 flex items-center justify-center text-white">
               <Bot size={18} />
            </div>
            <div>
              <h1 className="font-bold text-slate-800 text-sm">Titan AI Manager</h1>
              <div className="flex items-center gap-1">
                 <ShieldCheck size={10} className="text-emerald-500" />
                 <span className="text-[10px] text-slate-500">Secure Environment</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
          {messages.length === 0 && (
            <div className="flex flex-col items-center justify-center h-full text-zinc-400 space-y-3 opacity-60">
              <Bot size={48} />
              <p className="text-sm">Ready to assist, Admin.</p>
            </div>
          )}

          {messages.map((msg) => (
            <div
              key={msg.id}
              className={`flex ${msg.sender === 'user' ? 'justify-end' : 'justify-start'}`}
            >
              <div
                className={`max-w-[85%] p-3 rounded-2xl text-sm ${
                  msg.sender === 'user'
                    ? 'bg-indigo-600 text-white rounded-br-none'
                    : 'bg-white border border-gray-200 text-gray-800 rounded-bl-none shadow-sm'
                }`}
              >
                <p className="whitespace-pre-wrap">{msg.text}</p>
                <span className={`text-[10px] block mt-1 text-right ${msg.sender === 'user' ? 'text-indigo-200' : 'text-gray-400'}`}>
                  {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>
            </div>
          ))}
          
          {isLoading && (
            <div className="flex justify-start">
              <div className="bg-white border border-gray-200 p-3 rounded-2xl rounded-bl-none shadow-sm flex items-center gap-2">
                <Loader2 className="w-4 h-4 animate-spin text-indigo-600" />
                <span className="text-xs text-gray-500">Thinking...</span>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div className="p-4 bg-white border-t border-slate-200">
        <div className="flex items-center gap-2">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSend()}
            placeholder="Ask Titan..."
            className="flex-1 px-4 py-2 bg-slate-100 border-none rounded-full focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm text-slate-800 placeholder:text-slate-400"
          />
          <button
            onClick={handleSend}
            disabled={isLoading || !input.trim()}
            className="p-2 bg-indigo-600 text-white rounded-full hover:bg-indigo-700 disabled:opacity-50 transition-colors"
          >
            <Send size={18} />
          </button>
        </div>
      </div>
    </div>
  );
}
