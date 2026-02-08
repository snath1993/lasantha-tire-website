'use client';

import { useState, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/lib/client-auth';
import { Search, MoreVertical, MessageCircle, Phone, Video, Menu, X, SmilePlus, Mic, Send, Paperclip, ChevronDown, Lock, CheckCheck, Check, QrCode, RefreshCw, Power, Home, Smartphone } from 'lucide-react';
import Link from 'next/link';

const BOT_API_BASE_URL = (process.env.NEXT_PUBLIC_BOT_API_BASE_URL || 'http://localhost:8585').replace(/\/$/, '');
const botUrl = (path: string) => `${BOT_API_BASE_URL}${path}`;

interface WhatsAppStatus {
  ready: boolean;
  qr?: string;
  phoneNumber?: string;
  initializing?: boolean;
}

interface Chat {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
  unread?: number;
  avatar: string;
  isOnline?: boolean;
  isGroup?: boolean;
  timestamp?: number;
}

interface Message {
  id: string;
  text: string;
  time: string;
  sent: boolean;
  delivered?: boolean;
  read?: boolean;
  from?: string;
  timestamp?: number;
  body?: string;
}

export default function WhatsAppLivePage() {
  const router = useRouter();
  const [isChecking, setIsChecking] = useState(true);
  const [selectedChat, setSelectedChat] = useState<string | null>(null);
  const [messageText, setMessageText] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  
  // Real WhatsApp Data
  const [whatsappStatus, setWhatsappStatus] = useState<WhatsAppStatus>({ ready: false });
  const [chats, setChats] = useState<Chat[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);
  const [loading, setLoading] = useState(true);
  
  // Auto-scroll ref for messages container
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const [qrDataUrl, setQrDataUrl] = useState<string | null>(null);
  const eventSourceRef = useRef<EventSource | null>(null);

  useEffect(() => {
    const verifyAuth = async () => {
      const auth = await checkAuth();
      if (!auth.authenticated) {
        router.push('/');
        return;
      }
      setIsChecking(false);
    };
    verifyAuth();
  }, [router]);

  useEffect(() => {
    if (!isChecking) {
      loadWhatsAppData();
      connectToSSE();
    }
    return () => {
      if (eventSourceRef.current) {
        eventSourceRef.current.close();
      }
    };
  }, [isChecking]);

  // Auto-scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  // Generate QR code when needed
  useEffect(() => {
    if (whatsappStatus.qr && typeof window !== 'undefined') {
      (async () => {
        try {
          const mod: any = await import('qrcode');
          const QRCode = (mod && (mod.default || mod));
          if (QRCode && typeof QRCode.toDataURL === 'function') {
            const url = await QRCode.toDataURL(whatsappStatus.qr, {
              width: 512,
              margin: 2,
              errorCorrectionLevel: 'H',
              color: { dark: '#000000', light: '#FFFFFF' }
            });
            setQrDataUrl(url);
          }
        } catch (err) {
          console.error('QR generation error:', err);
        }
      })();
    } else {
      setQrDataUrl(null);
    }
  }, [whatsappStatus.qr]);

  const loadWhatsAppData = async () => {
    try {
      // Load WhatsApp status from bot API (localhost:8585)
      const statusRes = await fetch(botUrl('/status'), { 
        cache: 'no-cache',
      });
      if (statusRes.ok) {
        const status = await statusRes.json();
        setWhatsappStatus(status);
        console.log('[WhatsApp] Status loaded:', status);
      } else {
        console.warn('[WhatsApp] Status API not ok:', statusRes.status);
        setWhatsappStatus({ ready: false, initializing: false });
      }

      // Load chats if connected
      const chatsRes = await fetch(botUrl('/whatsapp/chats'), { 
        cache: 'no-cache',
      });
      if (chatsRes.ok) {
        const chatsData = await chatsRes.json();
        console.log('[WhatsApp] Chats loaded:', chatsData.length);
        
        if (Array.isArray(chatsData) && chatsData.length > 0) {
          const formattedChats = chatsData.map((chat: any) => ({
            id: chat.id,
            name: chat.name || 'Unknown',
            lastMessage: chat.lastMessage || 'No messages',
            time: formatTime(chat.timestamp),
            unread: chat.unreadCount || 0,
            avatar: chat.profilePicUrl || `https://ui-avatars.com/api/?name=${encodeURIComponent(chat.name || 'U')}&background=6366f1&color=fff`,
            isGroup: chat.isGroup,
            timestamp: chat.timestamp,
          }));
          setChats(formattedChats.sort((a: Chat, b: Chat) => (b.timestamp || 0) - (a.timestamp || 0)));
        }
      } else {
        console.warn('[WhatsApp] Chats API not available');
      }
    } catch (error) {
      console.error('[WhatsApp] Failed to load data:', error);
      setWhatsappStatus({ ready: false, initializing: false });
    } finally {
      setLoading(false);
    }
  };

  const connectToSSE = () => {
    try {
      console.log('[SSE] Connecting to bot SSE stream...');
      const eventSource = new EventSource(botUrl('/sse'));
      eventSourceRef.current = eventSource;

      eventSource.onopen = () => {
        console.log('[SSE] ✅ Connected to bot events');
      };

      eventSource.onmessage = (event) => {
        console.log('[SSE] Message received:', event);
      };

      eventSource.addEventListener('qr', (event: any) => {
        try {
          const data = JSON.parse(event.data);
          console.log('[SSE] QR code received');
          setWhatsappStatus({ ready: false, qr: data.qr, initializing: false });
        } catch (err) {
          console.error('[SSE] QR parse error:', err);
        }
      });

      eventSource.addEventListener('authenticated', (event: any) => {
        console.log('[SSE] Session authenticated');
        setWhatsappStatus(prev => ({ ...prev, initializing: true }));
      });

      eventSource.addEventListener('ready', (event: any) => {
        try {
          const data = JSON.parse(event.data);
          console.log('[SSE] WhatsApp ready!', data);
          setWhatsappStatus({ ready: true, phoneNumber: data.phoneNumber, initializing: false });
          // Reload chats when ready
          loadWhatsAppData();
        } catch (err) {
          console.error('[SSE] Ready parse error:', err);
        }
      });

      eventSource.addEventListener('disconnected', (event: any) => {
        try {
          const data = JSON.parse(event.data);
          console.log('[SSE] WhatsApp disconnected:', data.reason);
          setWhatsappStatus({ ready: false, initializing: false });
          setChats([]);
          setMessages([]);
        } catch (err) {
          console.error('[SSE] Disconnect parse error:', err);
        }
      });

      eventSource.addEventListener('auth_failure', (event: any) => {
        console.log('[SSE] Authentication failed');
        setWhatsappStatus({ ready: false, initializing: false });
      });

      eventSource.onerror = (error) => {
        console.error('[SSE] Connection error:', error);
        eventSource.close();
        // Retry connection after 5 seconds
        setTimeout(() => {
          if (!isChecking) {
            console.log('[SSE] Retrying connection...');
            connectToSSE();
          }
        }, 5000);
      };
    } catch (error) {
      console.error('[SSE] Setup error:', error);
    }
  };

  const loadChatMessages = async (chatId: string) => {
    try {
      console.log('[Messages] Loading for chat:', chatId);
      const res = await fetch(botUrl(`/whatsapp/messages/${encodeURIComponent(chatId)}`), {
        cache: 'no-cache'
      });
      if (res.ok) {
        const messagesData = await res.json();
        console.log('[Messages] Loaded:', messagesData.length);
        
        if (Array.isArray(messagesData) && messagesData.length > 0) {
          const formattedMessages = messagesData.map((msg: any) => ({
            id: msg.id || Date.now().toString(),
            text: msg.body || '',
            time: formatTime(msg.timestamp),
            sent: msg.fromMe,
            delivered: msg.ack >= 2,
            read: msg.ack >= 3,
            timestamp: msg.timestamp,
            body: msg.body,
          }));
          setMessages(formattedMessages.sort((a: Message, b: Message) => (a.timestamp || 0) - (b.timestamp || 0)));
        } else {
          setMessages([]);
        }
      }
    } catch (error) {
      console.error('[Messages] Load error:', error);
      setMessages([]);
    }
  };

  const formatTime = (timestamp: number) => {
    if (!timestamp) return '';
    const date = new Date(timestamp * 1000);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    
    if (days === 0) {
      return date.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true });
    } else if (days === 1) {
      return 'Yesterday';
    } else if (days < 7) {
      return date.toLocaleDateString('en-US', { weekday: 'short' });
    } else {
      return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    }
  };

  const handleChatSelect = (chatId: string) => {
    setSelectedChat(chatId);
    loadChatMessages(chatId);
  };

  const handleConnect = async () => {
    try {
      console.log('[Connect] Initiating WhatsApp login...');
      const res = await fetch(botUrl('/api/whatsapp/login'), { method: 'POST' });
      if (res.ok) {
        setWhatsappStatus({ ...whatsappStatus, initializing: true });
        console.log('[Connect] Login initiated, waiting for QR...');
        // QR will come via SSE
      } else {
        console.error('[Connect] Login API failed:', res.status);
      }
    } catch (error) {
      console.error('[Connect] Error:', error);
    }
  };

  const handleDisconnect = async () => {
    try {
      console.log('[Disconnect] Logging out...');
      const res = await fetch(botUrl('/api/whatsapp/logout'), { method: 'POST' });
      if (res.ok) {
        setWhatsappStatus({ ready: false });
        setChats([]);
        setMessages([]);
        setSelectedChat(null);
        console.log('[Disconnect] Logged out successfully');
      }
    } catch (error) {
      console.error('[Disconnect] Error:', error);
    }
  };

  const handleSendMessage = async () => {
    if (!messageText.trim() || !selectedChat) return;

    try {
      console.log('[Send] Sending message to:', selectedChat);
      const res = await fetch(botUrl('/whatsapp/messages/send'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          chatId: selectedChat,
          message: messageText
        })
      });

      if (res.ok) {
        console.log('[Send] Message sent successfully');
        // Add message optimistically
        const newMessage: Message = {
          id: Date.now().toString(),
          text: messageText,
          time: formatTime(Date.now() / 1000),
          sent: true,
          delivered: false,
          timestamp: Date.now() / 1000
        };
        setMessages([...messages, newMessage]);
        setMessageText('');
        
        // Reload messages after a short delay to get real ack status
        setTimeout(() => loadChatMessages(selectedChat), 1500);
      } else {
        console.error('[Send] Failed:', res.status);
      }
    } catch (error) {
      console.error('[Send] Error:', error);
    }
  };

  const filteredChats = chats.filter(chat =>
    chat.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  if (isChecking) {
    return (
      <div className="min-h-screen bg-slate-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-slate-400 text-sm font-medium">Loading WhatsApp...</p>
        </div>
      </div>
    );
  }

  const selectedChatData = chats.find(c => c.id === selectedChat);

  return (
    <div className="h-screen flex flex-col bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 overflow-hidden">
      
      {/* Top Header Bar */}
      <div className="bg-slate-900/50 backdrop-blur-xl px-6 py-3 flex items-center justify-between border-b border-white/10 shadow-sm z-10">
        <div className="flex items-center gap-4">
          <div className="w-10 h-10 bg-blue-500/20 rounded-xl flex items-center justify-center border border-blue-500/30">
            <MessageCircle className="w-6 h-6 text-blue-400" />
          </div>
          <div>
            <h1 className="text-white font-bold text-lg leading-tight">WhatsApp Live</h1>
            <div className="flex items-center gap-2">
              {whatsappStatus.ready ? (
                <>
                  <div className="w-2 h-2 bg-emerald-500 rounded-full animate-pulse"></div>
                  <span className="text-xs text-emerald-400 font-medium">Connected {whatsappStatus.phoneNumber && `(${whatsappStatus.phoneNumber})`}</span>
                </>
              ) : whatsappStatus.initializing ? (
                <>
                  <RefreshCw className="w-3 h-3 text-amber-500 animate-spin" />
                  <span className="text-xs text-amber-400 font-medium">Connecting...</span>
                </>
              ) : (
                <>
                  <div className="w-2 h-2 bg-rose-500 rounded-full"></div>
                  <span className="text-xs text-rose-400 font-medium">Disconnected</span>
                </>
              )}
            </div>
          </div>
        </div>
        <div className="flex items-center gap-3">
          {whatsappStatus.ready ? (
            <button
              onClick={handleDisconnect}
              className="px-4 py-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 rounded-xl text-sm font-medium transition-colors flex items-center gap-2 border border-rose-500/20"
            >
              <Power className="w-4 h-4" />
              Disconnect
            </button>
          ) : (
            <button
              onClick={handleConnect}
              className="px-4 py-2 bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 rounded-xl text-sm font-medium transition-colors flex items-center gap-2 border border-emerald-500/20"
            >
              <QrCode className="w-4 h-4" />
              Connect
            </button>
          )}
          <Link
            href="/dashboard"
            className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl text-sm font-medium transition-colors flex items-center gap-2 border border-white/10"
          >
            <Home className="w-4 h-4" />
            Dashboard
          </Link>
        </div>
      </div>

      {/* Main Container */}
      <div className="flex-1 flex overflow-hidden p-4 gap-4">
        
        {/* Left Sidebar - Chat List */}
        <div className="w-[380px] bg-slate-800/50 backdrop-blur-xl rounded-[2rem] border border-white/10 flex flex-col shadow-xl overflow-hidden">
          
          {/* Search Bar */}
          <div className="p-4 border-b border-white/10">
            <div className="relative">
              <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
              <input
                type="text"
                placeholder="Search chats..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-10 pr-4 py-2.5 bg-slate-900/50 text-white text-sm rounded-xl border border-white/10 focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all placeholder-slate-500"
              />
            </div>
          </div>

          {/* Chats List */}
          <div className="flex-1 overflow-y-auto custom-scrollbar">
            {loading ? (
              <div className="flex items-center justify-center py-12">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : !whatsappStatus.ready ? (
              <div className="flex flex-col items-center justify-center py-12 px-6 text-center">
                <div className="w-16 h-16 bg-slate-700/50 rounded-full flex items-center justify-center mb-4">
                  <QrCode className="w-8 h-8 text-slate-400" />
                </div>
                <p className="text-white font-medium text-sm mb-1">Not Connected</p>
                <p className="text-slate-500 text-xs">Connect to start chatting</p>
              </div>
            ) : filteredChats.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-12 px-6 text-center">
                <div className="w-16 h-16 bg-slate-700/50 rounded-full flex items-center justify-center mb-4">
                  <MessageCircle className="w-8 h-8 text-slate-400" />
                </div>
                <p className="text-white font-medium text-sm">No chats found</p>
              </div>
            ) : (
              <div className="p-2 space-y-1">
                {filteredChats.map((chat) => (
                  <div
                    key={chat.id}
                    onClick={() => handleChatSelect(chat.id)}
                    className={`flex items-center gap-3 p-3 rounded-xl cursor-pointer transition-all ${
                      selectedChat === chat.id 
                        ? 'bg-blue-600/20 border border-blue-500/30 shadow-sm' 
                        : 'hover:bg-slate-700/50 border border-transparent'
                    }`}
                  >
                    <div className="relative">
                      <img 
                        src={chat.avatar} 
                        alt={chat.name}
                        className="h-12 w-12 rounded-full object-cover ring-2 ring-slate-700"
                      />
                      {chat.isOnline && (
                        <div className="absolute bottom-0 right-0 w-3 h-3 bg-emerald-500 border-2 border-slate-800 rounded-full"></div>
                      )}
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center justify-between mb-1">
                        <h3 className={`font-bold text-sm truncate ${selectedChat === chat.id ? 'text-blue-100' : 'text-slate-200'}`}>
                          {chat.name}
                        </h3>
                        <span className={`text-xs ${selectedChat === chat.id ? 'text-blue-300' : 'text-slate-500'}`}>
                          {chat.time}
                        </span>
                      </div>
                      <div className="flex items-center justify-between">
                        <p className={`text-xs truncate ${selectedChat === chat.id ? 'text-blue-200' : 'text-slate-400'}`}>
                          {chat.lastMessage}
                        </p>
                        {chat.unread && (
                          <span className="bg-blue-500 text-white text-[10px] font-bold px-1.5 py-0.5 rounded-full min-w-[18px] text-center shadow-lg shadow-blue-500/30">
                            {chat.unread}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Right Side - Chat Window */}
        <div className="flex-1 bg-slate-800/50 backdrop-blur-xl rounded-[2rem] border border-white/10 flex flex-col shadow-xl overflow-hidden relative">
          {selectedChat ? (
            <>
              {/* Chat Header */}
              <div className="px-6 py-4 flex items-center justify-between border-b border-white/10 bg-slate-900/50 backdrop-blur-md sticky top-0 z-10">
                <div className="flex items-center gap-4">
                  <img 
                    src={selectedChatData?.avatar} 
                    alt={selectedChatData?.name}
                    className="h-10 w-10 rounded-full object-cover ring-2 ring-slate-700"
                  />
                  <div>
                    <h2 className="text-white font-bold text-base">{selectedChatData?.name}</h2>
                    <p className="text-slate-400 text-xs flex items-center gap-1">
                      {selectedChatData?.isOnline ? (
                        <span className="text-emerald-400 font-medium">Online</span>
                      ) : 'Offline'}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                    <Phone className="w-5 h-5" />
                  </button>
                  <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                    <Video className="w-5 h-5" />
                  </button>
                  <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                    <MoreVertical className="w-5 h-5" />
                  </button>
                </div>
              </div>

              {/* Messages Area */}
              <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-slate-900/30 custom-scrollbar">
                <div className="flex justify-center mb-6">
                  <div className="bg-slate-800/80 px-4 py-2 rounded-full flex items-center gap-2 shadow-sm border border-white/5">
                    <Lock className="w-3 h-3 text-slate-400" />
                    <p className="text-slate-400 text-xs font-medium">
                      Messages are end-to-end encrypted
                    </p>
                  </div>
                </div>

                {messages.map((message) => (
                  <div
                    key={message.id}
                    className={`flex ${message.sent ? 'justify-end' : 'justify-start'}`}
                  >
                    <div
                      className={`max-w-[70%] rounded-2xl px-5 py-3 shadow-sm ${
                        message.sent
                          ? 'bg-blue-600 text-white rounded-br-none shadow-lg shadow-blue-600/20'
                          : 'bg-slate-700 text-slate-100 border border-white/5 rounded-bl-none'
                      }`}
                    >
                      <p className="text-sm leading-relaxed whitespace-pre-wrap">{message.text}</p>
                      <div className={`flex items-center justify-end gap-1 mt-1 ${message.sent ? 'text-blue-200' : 'text-slate-400'}`}>
                        <span className="text-[10px] font-medium">{message.time}</span>
                        {message.sent && (
                          message.read ? (
                            <CheckCheck className="w-3 h-3 text-white" />
                          ) : message.delivered ? (
                            <CheckCheck className="w-3 h-3 opacity-70" />
                          ) : (
                            <Check className="w-3 h-3 opacity-70" />
                          )
                        )}
                      </div>
                    </div>
                  </div>
                ))}
                <div ref={messagesEndRef} />
              </div>

              {/* Message Input */}
              <div className="p-4 bg-slate-800/50 border-t border-white/10 backdrop-blur-md">
                <div className="flex items-center gap-3 bg-slate-900/50 p-2 rounded-2xl border border-white/10">
                  <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                    <SmilePlus className="w-5 h-5" />
                  </button>
                  <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                    <Paperclip className="w-5 h-5" />
                  </button>
                  <input
                    type="text"
                    placeholder="Type a message..."
                    value={messageText}
                    onChange={(e) => setMessageText(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
                    className="flex-1 bg-transparent text-white text-sm focus:outline-none placeholder-slate-500"
                  />
                  {messageText.trim() ? (
                    <button 
                      onClick={handleSendMessage}
                      className="p-2 bg-blue-600 hover:bg-blue-700 text-white rounded-xl transition-colors shadow-lg shadow-blue-600/20"
                    >
                      <Send className="w-5 h-5" />
                    </button>
                  ) : (
                    <button className="p-2 hover:bg-slate-700 rounded-xl transition-colors text-slate-400 hover:text-white">
                      <Mic className="w-5 h-5" />
                    </button>
                  )}
                </div>
              </div>
            </>
          ) : (
            // No Chat Selected State
            <div className="flex-1 flex flex-col items-center justify-center p-8 text-center bg-slate-900/20">
              {!whatsappStatus.ready && whatsappStatus.qr ? (
                <div className="max-w-md w-full bg-slate-800 p-8 rounded-[2rem] shadow-2xl border border-white/10">
                  <h2 className="text-2xl font-bold text-white mb-2">Link Device</h2>
                  <p className="text-slate-400 text-sm mb-6">Scan the QR code to connect WhatsApp</p>
                  
                  <div className="bg-white p-4 rounded-2xl mb-6 inline-block border border-white/10">
                    {qrDataUrl ? (
                      <img src={qrDataUrl} alt="QR Code" className="w-64 h-64" />
                    ) : (
                      <div className="w-64 h-64 flex items-center justify-center">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                      </div>
                    )}
                  </div>
                  
                  <div className="text-left space-y-4 mb-8 bg-slate-900/50 p-6 rounded-2xl border border-white/5">
                    <div className="flex gap-4 items-start">
                      <div className="w-6 h-6 bg-blue-500/20 text-blue-400 rounded-full flex items-center justify-center text-xs font-bold shrink-0 mt-0.5 border border-blue-500/30">1</div>
                      <p className="text-sm text-slate-300">Open WhatsApp on your phone</p>
                    </div>
                    <div className="flex gap-4 items-start">
                      <div className="w-6 h-6 bg-blue-500/20 text-blue-400 rounded-full flex items-center justify-center text-xs font-bold shrink-0 mt-0.5 border border-blue-500/30">2</div>
                      <p className="text-sm text-slate-300">Tap <strong>Menu</strong> or <strong>Settings</strong> and select <strong>Linked Devices</strong></p>
                    </div>
                    <div className="flex gap-4 items-start">
                      <div className="w-6 h-6 bg-blue-500/20 text-blue-400 rounded-full flex items-center justify-center text-xs font-bold shrink-0 mt-0.5 border border-blue-500/30">3</div>
                      <p className="text-sm text-slate-300">Point your phone to this screen to capture the QR code</p>
                    </div>
                  </div>

                  <button
                    onClick={() => loadWhatsAppData()}
                    className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-bold transition-colors flex items-center justify-center gap-2 shadow-lg shadow-blue-600/20"
                  >
                    <RefreshCw className="w-4 h-4" />
                    Refresh QR Code
                  </button>
                </div>
              ) : (
                <div className="max-w-md">
                  <div className="w-24 h-24 bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-6 border border-white/5 shadow-xl">
                    <Smartphone className="w-12 h-12 text-blue-500" />
                  </div>
                  <h2 className="text-3xl font-bold text-white mb-4">WhatsApp Live</h2>
                  <p className="text-slate-400 text-base leading-relaxed mb-8">
                    Send and receive messages directly from your dashboard.
                    <br />Connect your device to get started.
                  </p>
                  <div className="flex items-center justify-center gap-2 text-slate-400 text-xs font-medium bg-slate-800 px-4 py-2 rounded-full shadow-sm border border-white/10 inline-flex">
                    <Lock className="w-3 h-3" />
                    <span>End-to-end encrypted</span>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
