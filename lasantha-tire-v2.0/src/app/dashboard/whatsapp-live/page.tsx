'use client';

import { useState, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { checkAuth } from '@/core/lib/client-auth';
import { Search, MoreVertical, MessageCircle, Phone, Video, Menu, X, SmilePlus, Mic, Send, Paperclip, ChevronDown, Lock, CheckCheck, Check, QrCode, RefreshCw, Power, Home, Smartphone, ArrowLeft, Camera, Image as ImageIcon, Users, CircleDashed } from 'lucide-react';
import Link from 'next/link';

const BOT_API_PROXY = '/api/bot';
const botUrl = (path: string) => `${BOT_API_PROXY}${path}`;

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

function toMsTimestamp(ts?: number) {
  if (!ts) return Date.now();
  // whatsapp-web.js timestamps are usually seconds; occasionally we may get ms
  return ts > 1_000_000_000_000 ? ts : ts * 1000;
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
  const [loadingMessages, setLoadingMessages] = useState(false);
  
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
              color: {
                dark: '#000000',
                light: '#ffffff'
              }
            });
            setQrDataUrl(url);
          }
        } catch (err) {
          console.error('Error generating QR:', err);
        }
      })();
    }
  }, [whatsappStatus.qr]);

  const connectToSSE = () => {
    if (eventSourceRef.current) {
      eventSourceRef.current.close();
    }

    const sse = new EventSource(botUrl('/events'));
    eventSourceRef.current = sse;

    const safeJson = (ev: MessageEvent) => {
      try {
        return JSON.parse(ev.data);
      } catch {
        return null;
      }
    };

    sse.addEventListener('qr', (ev) => {
      const payload: any = safeJson(ev as MessageEvent);
      const qr = payload?.qr;
      if (qr) setWhatsappStatus(prev => ({ ...prev, qr, ready: false }));
    });

    sse.addEventListener('ready', (ev) => {
      const payload: any = safeJson(ev as MessageEvent);
      if (payload?.ready) {
        setWhatsappStatus(prev => ({ ...prev, ready: true, qr: undefined }));
        loadChats();
      }
    });

    sse.addEventListener('disconnected', () => {
      setWhatsappStatus(prev => ({ ...prev, ready: false }));
    });

    sse.addEventListener('status', (ev) => {
      const payload: any = safeJson(ev as MessageEvent);
      // status payload is whatever bot writes to whatsapp-status.json
      if (payload && typeof payload === 'object') {
        setWhatsappStatus(prev => ({
          ...prev,
          ready: !!payload.ready,
          qr: payload.qr ?? prev.qr,
          phoneNumber: payload.phoneNumber ?? prev.phoneNumber,
          initializing: payload.initializing ?? prev.initializing,
        }));
      }
    });

    const handleIncoming = (payload: any) => {
      if (!payload) return;
      const newMsg = payload;
      const newMsgId = (newMsg.id && (newMsg.id._serialized || newMsg.id.id)) || undefined;

      // If the message belongs to the currently selected chat, add it
      if (selectedChat && (newMsg.from === selectedChat || newMsg.to === selectedChat)) {
        setMessages(prev => {
          if (newMsgId && prev.some(m => m.id === newMsgId)) return prev;
          return [...prev, {
            id: newMsgId || Date.now().toString(),
            text: newMsg.body || '',
            time: new Date(toMsTimestamp(newMsg.timestamp)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            sent: newMsg.fromMe || false,
            delivered: (newMsg.ack ?? 0) >= 2,
            read: (newMsg.ack ?? 0) >= 3,
            from: newMsg.from,
            timestamp: newMsg.timestamp
          }];
        });
      }

      // Refresh chat list to show latest message/unread count
      loadChats();
    };

    sse.addEventListener('message', (ev) => {
      handleIncoming(safeJson(ev as MessageEvent));
    });

    sse.addEventListener('message_create', (ev) => {
      handleIncoming(safeJson(ev as MessageEvent));
    });

    sse.onerror = () => {
      sse.close();
      // Reconnect after a delay
      setTimeout(connectToSSE, 5000);
    };
  };

  const loadWhatsAppData = async () => {
    try {
      // Prefer the dashboard-friendly status file (includes qr/ready)
      const statusRes = await fetch(botUrl('/status'), { cache: 'no-store' });
      if (statusRes.ok) {
        const st: any = await statusRes.json();
        const isReady = !!st.ready;
        setWhatsappStatus({
          ready: isReady,
          phoneNumber: st.phoneNumber || whatsappStatus.phoneNumber,
          qr: st.qr || undefined,
          initializing: !!st.initializing
        });
        if (isReady) {
          loadChats();
        } else {
          setLoading(false);
        }
        return;
      }

      // Fallback: api status
      const res = await fetch(botUrl('/api/whatsapp/status'), { cache: 'no-store' });
      const data = await res.json();
      const isConnected = !!data.isConnected || data.status === 'connected' || data.status === 'ready';
      setWhatsappStatus({ ready: isConnected, phoneNumber: data.phoneNumber });
      if (isConnected) loadChats(); else setLoading(false);
    } catch (error) {
      console.error('Error loading status:', error);
      setLoading(false);
    }
  };

  const handleConnect = async () => {
    try {
      setWhatsappStatus(prev => ({ ...prev, initializing: true }));
      await fetch(botUrl('/connect'), { method: 'POST' });
      // status/qr will arrive via SSE; also refresh status after a moment
      setTimeout(() => loadWhatsAppData(), 800);
    } catch (e) {
      console.error(e);
      setWhatsappStatus(prev => ({ ...prev, initializing: false }));
    }
  };

  const loadChats = async () => {
    try {
      const res = await fetch(botUrl('/whatsapp/chats'));
      if (!res.ok) throw new Error('Failed to fetch chats');
      const data = await res.json();
      
      const formattedChats: Chat[] = data.map((chat: any) => ({
        id: chat.id,
        name: chat.name || chat.id.replace('@c.us', ''),
        lastMessage: chat.lastMessage || '',
        time: new Date(toMsTimestamp(chat.timestamp)).toLocaleDateString(),
        unread: chat.unreadCount,
        avatar: chat.profilePicUrl || `https://ui-avatars.com/api/?name=${encodeURIComponent(chat.name || 'Unknown')}&background=random`,
        isOnline: false, // API doesn't provide this yet
        isGroup: chat.isGroup,
        timestamp: chat.timestamp
      }));

      setChats(formattedChats);
    } catch (error) {
      console.error('Error loading chats:', error);
      // Fallback to empty or error state, don't use fake data if we want "real" connection
      setChats([]); 
    } finally {
      setLoading(false);
    }
  };

  const handleChatSelect = async (chatId: string) => {
    setSelectedChat(chatId);
    setMessages([]); // Clear previous messages while loading
    setLoadingMessages(true);

    try {
      const res = await fetch(botUrl(`/whatsapp/messages/${encodeURIComponent(chatId)}?limit=50`));
      if (!res.ok) throw new Error('Failed to fetch messages');
      const data = await res.json();

      const formattedMessages: Message[] = data.map((msg: any) => ({
        id: msg.id,
        text: msg.body,
        time: new Date(toMsTimestamp(msg.timestamp)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
        sent: msg.fromMe,
        delivered: msg.ack >= 2,
        read: msg.ack >= 3,
        from: msg.from,
        timestamp: msg.timestamp,
        body: msg.body
      }));
      
      setMessages(formattedMessages);
    } catch (error) {
      console.error('Error loading messages:', error);
    } finally {
      setLoadingMessages(false);
    }
  };

  const handleSendMessage = async () => {
    if (!messageText.trim() || !selectedChat) return;

    const tempId = Date.now().toString();
    const newMessage: Message = {
      id: tempId,
      text: messageText,
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      sent: true,
      delivered: false,
      read: false
    };

    setMessages(prev => [...prev, newMessage]);
    setMessageText('');

    // Send to API
    try {
      const res = await fetch(botUrl('/whatsapp/messages/send'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          chatId: selectedChat,
          message: messageText
        })
      });
      
      if (!res.ok) {
         console.error('Failed to send');
         // Mark message as failed in UI?
      }
    } catch (error) {
      console.error('Error sending message:', error);
    }
  };

  const handleDisconnect = async () => {
    if (confirm('Are you sure you want to disconnect?')) {
      try {
        await fetch(botUrl('/api/whatsapp/logout'), { method: 'POST' });
        setWhatsappStatus({ ready: false });
        loadWhatsAppData();
      } catch (e) {
        console.error(e);
      }
    }
  };

  const filteredChats = chats.filter(chat => 
    chat.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    chat.lastMessage.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const selectedChatData = chats.find(c => c.id === selectedChat);

  if (isChecking) return null;

  return (
    <div className="fixed inset-0 z-[9999] flex h-full w-full bg-[#111b21] text-[#e9edef] overflow-hidden font-sans">
      
      {/* Background Pattern (Subtle) */}
      <div className="absolute inset-0 opacity-[0.06] pointer-events-none z-0" 
           style={{ backgroundImage: 'url("https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png")' }}>
      </div>

      {/* Left Sidebar / Mobile List View */}
      <div className={`
        w-full md:w-[400px] flex flex-col border-r border-[#2f3b43] bg-[#111b21] z-10
        ${selectedChat ? 'hidden md:flex' : 'flex'}
      `}>
        {/* Header */}
        <div className="h-[60px] bg-[#202c33] px-4 flex items-center justify-between shrink-0 border-b border-[#2f3b43]">
          <div className="flex items-center gap-3">
             <button 
               onClick={() => router.push('/dashboard')} 
               className="text-[#aebac1] hover:text-[#e9edef] transition-colors mr-1"
               title="Back to Dashboard"
             >
               <ArrowLeft className="w-6 h-6" />
             </button>
             <div className="w-10 h-10 rounded-full bg-slate-600 overflow-hidden cursor-pointer">
               <img src="https://ui-avatars.com/api/?name=Admin&background=00a884&color=fff" alt="Profile" className="w-full h-full object-cover" />
             </div>
             {whatsappStatus.ready && (
               <span className="text-xs text-[#00a884] font-medium bg-[#00a884]/10 px-2 py-0.5 rounded-full">
                 Online
               </span>
             )}
          </div>
          <div className="flex items-center gap-5 text-[#aebac1]">
            <button title="New Chat"><MessageCircle className="w-6 h-6" /></button>
            <button title="Menu" onClick={handleDisconnect}><MoreVertical className="w-6 h-6" /></button>
          </div>
        </div>
        
        {/* Search & Filter */}
        <div className="p-2 bg-[#111b21] border-b border-[#2f3b43]">
          <div className="relative flex items-center">
            <div className="absolute left-3 text-[#aebac1]">
              <Search className="w-5 h-5" />
            </div>
            <input
              type="text"
              placeholder="Search or start new chat"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full py-2 pl-12 pr-4 bg-[#202c33] text-[#d1d7db] text-sm rounded-lg focus:outline-none placeholder-[#8696a0]"
            />
            <button className="ml-2 text-[#aebac1]">
              <MoreVertical className="w-5 h-5 rotate-90" />
            </button>
          </div>
        </div>

        {/* Chat List */}
        <div className="flex-1 overflow-y-auto custom-scrollbar">
          {loading ? (
            <div className="flex items-center justify-center py-12">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#00a884]"></div>
            </div>
          ) : !whatsappStatus.ready ? (
            <div className="flex flex-col items-center justify-center h-full px-6 text-center">
              <div className="w-20 h-20 bg-[#202c33] rounded-full flex items-center justify-center mb-6">
                <QrCode className="w-10 h-10 text-[#8696a0]" />
              </div>
              <h3 className="text-[#e9edef] text-xl font-light mb-2">WhatsApp Web</h3>
              <p className="text-[#8696a0] text-sm mb-8">Connect your phone to sync messages.</p>
              
              {qrDataUrl ? (
                <div className="bg-white p-2 rounded-lg mb-4">
                  <img src={qrDataUrl} alt="QR Code" className="w-48 h-48" />
                </div>
              ) : (
                <button 
                  onClick={handleConnect}
                  className="px-6 py-2 bg-[#00a884] text-[#111b21] font-bold rounded-full hover:bg-[#008f6f] transition-colors"
                >
                  Generate QR Code
                </button>
              )}
              
              <Link href="/dashboard" className="mt-8 text-[#00a884] text-sm hover:underline">
                Back to Dashboard
              </Link>
            </div>
          ) : filteredChats.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-12 px-6 text-center text-[#8696a0]">
              <p className="text-sm">No chats found</p>
            </div>
          ) : (
            <div>
              {filteredChats.map((chat) => (
                <div
                  key={chat.id}
                  onClick={() => handleChatSelect(chat.id)}
                  className={`group flex items-center gap-3 px-3 py-3 cursor-pointer hover:bg-[#202c33] transition-colors border-b border-[#2f3b43] ${
                    selectedChat === chat.id ? 'bg-[#2a3942]' : ''
                  }`}
                >
                  <div className="relative shrink-0">
                    <img 
                      src={chat.avatar} 
                      alt={chat.name}
                      className="h-[49px] w-[49px] rounded-full object-cover"
                    />
                  </div>
                  <div className="flex-1 min-w-0 flex flex-col justify-center">
                    <div className="flex items-center justify-between mb-0.5">
                      <h3 className="font-normal text-[#e9edef] text-[17px] truncate">
                        {chat.name}
                      </h3>
                      <span className={`text-xs ${chat.unread ? 'text-[#00a884] font-medium' : 'text-[#8696a0]'}`}>
                        {chat.time}
                      </span>
                    </div>
                    <div className="flex items-center justify-between">
                      <p className="text-[14px] text-[#8696a0] truncate flex-1">
                        {chat.lastMessage}
                      </p>
                      {chat.unread ? (
                        <span className="bg-[#00a884] text-[#111b21] text-xs font-bold px-1.5 min-w-[20px] h-5 flex items-center justify-center rounded-full ml-2">
                          {chat.unread}
                        </span>
                      ) : (
                        <div className="hidden group-hover:block text-[#8696a0] ml-2">
                           <ChevronDown className="w-5 h-5" />
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Right Side / Chat View */}
      <div className={`
        flex-1 flex-col bg-[#0b141a] relative z-10
        ${selectedChat ? 'flex fixed inset-0 md:static' : 'hidden md:flex'}
      `}>
        {/* Chat Background Image */}
        <div className="absolute inset-0 opacity-[0.06] pointer-events-none z-0" 
             style={{ backgroundImage: 'url("https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png")' }}>
        </div>

        {selectedChat ? (
          <>
            {/* Chat Header */}
            <div className="h-[60px] bg-[#202c33] px-4 flex items-center justify-between shrink-0 border-b border-[#2f3b43] z-10">
              <div className="flex items-center gap-3">
                <button 
                  onClick={() => setSelectedChat(null)}
                  className="md:hidden -ml-2 p-2 text-[#aebac1]"
                >
                  <ArrowLeft className="w-6 h-6" />
                </button>
                <div className="flex items-center gap-3 cursor-pointer">
                  <img 
                    src={selectedChatData?.avatar} 
                    alt={selectedChatData?.name}
                    className="h-10 w-10 rounded-full object-cover"
                  />
                  <div>
                    <h2 className="text-[#e9edef] font-normal text-base">{selectedChatData?.name}</h2>
                    <p className="text-[#8696a0] text-xs">
                      {selectedChatData?.isOnline ? 'online' : 'click here for contact info'}
                    </p>
                  </div>
                </div>
              </div>
              <div className="flex items-center gap-6 text-[#aebac1]">
                <button title="Search"><Search className="w-5 h-5" /></button>
                <button title="Menu"><MoreVertical className="w-5 h-5" /></button>
              </div>
            </div>

            {/* Messages Area */}
            <div className="flex-1 overflow-y-auto p-4 space-y-2 z-10 custom-scrollbar bg-[#0b141a]">
              <div className="flex justify-center mb-6">
                <div className="bg-[#1f2c34] px-3 py-1.5 rounded-lg shadow-sm">
                  <p className="text-[#ffd279] text-xs text-center flex items-center gap-1.5">
                    <Lock className="w-2.5 h-2.5" />
                    Messages are end-to-end encrypted. No one outside of this chat, not even WhatsApp, can read or listen to them.
                  </p>
                </div>
              </div>

              {loadingMessages ? (
                <div className="flex justify-center py-8">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#00a884]"></div>
                </div>
              ) : (
                messages.map((message, index) => {
                  const prevMessage = messages[index - 1];
                  const showDate = !prevMessage || new Date(toMsTimestamp(message.timestamp)).toDateString() !== new Date(toMsTimestamp(prevMessage.timestamp)).toDateString();
                  
                  return (
                    <div key={message.id}>
                      {showDate && (
                        <div className="flex justify-center my-4 sticky top-2 z-20">
                          <div className="bg-[#1f2c34] px-3 py-1.5 rounded-lg shadow-sm">
                            <span className="text-[#8696a0] text-xs font-medium uppercase">
                              {new Date(toMsTimestamp(message.timestamp)).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                            </span>
                          </div>
                        </div>
                      )}
                      
                      <div className={`flex ${message.sent ? 'justify-end' : 'justify-start'} mb-1 group`}>
                        <div
                          className={`max-w-[85%] md:max-w-[65%] rounded-lg px-2 py-1 shadow-sm relative text-sm ${
                            message.sent
                              ? 'bg-[#005c4b] text-[#e9edef] rounded-tr-none'
                              : 'bg-[#202c33] text-[#e9edef] rounded-tl-none'
                          }`}
                        >
                          {/* Tail SVG */}
                          {message.sent ? (
                            <span className="absolute -right-2 top-0 text-[#005c4b]">
                              <svg viewBox="0 0 8 13" height="13" width="8" preserveAspectRatio="xMidYMid meet" version="1.1" x="0px" y="0px" enableBackground="new 0 0 8 13"><path opacity="0.13" fill="#00000000" d="M5.188,1H0v11.193l6.467-8.625 C7.526,2.156,6.958,1,5.188,1z"></path><path fill="currentColor" d="M5.188,0H0v11.193l6.467-8.625C7.526,1.156,6.958,0,5.188,0z"></path></svg>
                            </span>
                          ) : (
                            <span className="absolute -left-2 top-0 text-[#202c33]">
                              <svg viewBox="0 0 8 13" height="13" width="8" preserveAspectRatio="xMidYMid meet" version="1.1" x="0px" y="0px" enableBackground="new 0 0 8 13"><path opacity="0.13" fill="#00000000" d="M1.533,3.568L8,12.193V1H2.812 C1.042,1,0.474,2.156,1.533,3.568z"></path><path fill="currentColor" d="M1.533,2.568L8,11.193V0L2.812,0C1.042,0,0.474,1.156,1.533,2.568z"></path></svg>
                            </span>
                          )}

                          <div className="px-1 pt-1 pb-4">
                            <p className="whitespace-pre-wrap leading-relaxed">{message.text}</p>
                          </div>
                          
                          <div className="absolute bottom-1 right-2 flex items-center gap-1">
                            <span className="text-[11px] text-[#ffffff99] min-w-[45px] text-right">{message.time}</span>
                            {message.sent && (
                              <span className={message.read ? 'text-[#53bdeb]' : 'text-[#ffffff99]'}>
                                <CheckCheck className="w-4 h-4" />
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })
              )}
              <div ref={messagesEndRef} />
            </div>

            {/* Message Input */}
            <div className="min-h-[62px] bg-[#202c33] px-4 py-2 flex items-end gap-2 z-10">
              <div className="flex items-center gap-2 pb-2 text-[#8696a0]">
                <button className="p-1 hover:text-[#aebac1]"><SmilePlus className="w-6 h-6" /></button>
                <button className="p-1 hover:text-[#aebac1]"><Paperclip className="w-6 h-6" /></button>
              </div>
              
              <div className="flex-1 bg-[#2a3942] rounded-lg flex items-center min-h-[42px] mb-1">
                <input
                  type="text"
                  placeholder="Type a message"
                  value={messageText}
                  onChange={(e) => setMessageText(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
                  className="w-full px-4 py-2 bg-transparent text-[#d1d7db] text-sm focus:outline-none placeholder-[#8696a0]"
                />
              </div>

              <div className="pb-2 text-[#8696a0]">
                {messageText.trim() ? (
                  <button 
                    onClick={handleSendMessage}
                    className="p-2 text-[#8696a0] hover:text-[#aebac1]"
                  >
                    <Send className="w-6 h-6" />
                  </button>
                ) : (
                  <button className="p-2 text-[#8696a0] hover:text-[#aebac1]">
                    <Mic className="w-6 h-6" />
                  </button>
                )}
              </div>
            </div>
          </>
        ) : (
          // No Chat Selected State (Desktop)
          <div className="flex-1 flex flex-col items-center justify-center p-8 text-center border-b-[6px] border-[#00a884] z-10 bg-[#222e35]">
            <div className="max-w-md">
              <div className="mb-10">
                 <img src="https://static.whatsapp.net/rsrc.php/v3/y6/r/wa669ae.svg" alt="WhatsApp" className="w-80 opacity-30 mx-auto" />
                 {/* Fallback if image fails */}
                 <Smartphone className="w-32 h-32 text-[#41525d] mx-auto hidden" />
              </div>
              <h2 className="text-3xl font-light text-[#e9edef] mb-4">WhatsApp Web</h2>
              <p className="text-[#8696a0] text-sm leading-relaxed mb-8">
                Send and receive messages without keeping your phone online.<br />
                Use WhatsApp on up to 4 linked devices and 1 phone.
              </p>
              <div className="flex items-center justify-center gap-2 text-[#8696a0] text-xs mt-10">
                <Lock className="w-3 h-3" />
                <span>End-to-end encrypted</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
