'use client'

import { useState, useRef, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Send, Bot, Loader2 } from 'lucide-react'
import { getBotApiUrl } from '@/utils/getBotApiUrl'

interface Message {
  id: string;
  text: string;
  sender: 'user' | 'bot';
  timestamp: Date;
}

interface AiChatModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AiChatModal({ isOpen, onClose }: AiChatModalProps) {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  
  // Simple session storage for admin phone to avoid asking every time
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
      const response = await fetch(`${getBotApiUrl()}/api/titan/message`, {
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
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm"
        >
          <motion.div
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.95, opacity: 0 }}
            className="w-full max-w-md bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[600px] h-[500px]"
          >
            {/* Header */}
            <div className="bg-primary-600 p-4 flex items-center justify-between text-white">
              <div className="flex items-center gap-2">
                <Bot className="w-6 h-6" />
                <h2 className="font-bold text-lg">Titan AI Manager</h2>
              </div>
              <button onClick={onClose} className="p-1 hover:bg-primary-700 rounded-full transition-colors">
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Messages */}
            <div className="flex-1 overflow-y-auto p-4 bg-gray-50 space-y-4">
               {messages.length === 0 && (
                 <div className="text-center text-gray-400 mt-10">
                   <Bot className="w-12 h-12 mx-auto mb-2 opacity-50" />
                   <p className="text-sm">Authorized usage only.</p>
                   <p className="text-xs mt-1">Try "List system jobs" or "Show sales".</p>
                 </div>
               )}
              {messages.map((msg) => (
                <div
                  key={msg.id}
                  className={`flex ${msg.sender === 'user' ? 'justify-end' : 'justify-start'}`}
                >
                  <div
                    className={`max-w-[85%] p-3 rounded-2xl ${
                      msg.sender === 'user'
                        ? 'bg-primary-600 text-white rounded-br-none'
                        : 'bg-white border border-gray-200 text-gray-800 rounded-bl-none shadow-sm'
                    }`}
                  >
                    <p className="whitespace-pre-wrap text-sm">{msg.text}</p>
                    <span className="text-[10px] opacity-70 block mt-1 text-right">
                      {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </span>
                  </div>
                </div>
              ))}
              {isLoading && (
                <div className="flex justify-start">
                  <div className="bg-white border border-gray-200 p-3 rounded-2xl rounded-bl-none shadow-sm flex items-center gap-2">
                    <Loader2 className="w-4 h-4 animate-spin text-primary-600" />
                    <span className="text-sm text-gray-500">Titan is processing...</span>
                  </div>
                </div>
              )}
              <div ref={messagesEndRef} />
            </div>

            {/* Input */}
            <div className="p-4 bg-white border-t border-gray-200">
              <div className="flex items-center gap-2">
                <input
                  type="text"
                  value={input}
                  onChange={(e) => setInput(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleSend()}
                  placeholder="Command Titan..."
                  className="flex-1 px-4 py-2 border border-gray-200 rounded-full focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent text-sm"
                />
                <button
                  onClick={handleSend}
                  disabled={isLoading || !input.trim()}
                  className="p-2 bg-primary-600 text-white rounded-full hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  <Send className="w-5 h-5" />
                </button>
              </div>
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  )
}
