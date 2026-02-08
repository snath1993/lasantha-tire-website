"use client";

import { Card, CardContent, CardHeader, CardTitle } from '@/views/shared/ui/card';
import { ScrollArea } from '@/views/shared/ui/scroll-area';
import { Avatar, AvatarFallback, AvatarImage } from '@/views/shared/ui/avatar';
import { Badge } from '@/views/shared/ui/badge';
import { Search, Bot, User, Calendar, MessageSquare, ArrowLeft } from "lucide-react";
import Link from "next/link";

const chatHistory = [
  {
    id: 1,
    customer: "0771234567",
    name: "Kamal Perera",
    timestamp: "10:30 AM",
    messages: [
      { role: "user", content: "Do you have 175/70R13 tyres?" },
      { role: "assistant", content: "Yes, we have CEAT and DSI brands available. CEAT is LKR 12,500 and DSI is LKR 11,000." },
      { role: "user", content: "Can I book an appointment for tomorrow?" },
      { role: "assistant", content: "Sure! What time would you like to come?" }
    ]
  },
  {
    id: 2,
    customer: "0719876543",
    name: "Nimal Silva",
    timestamp: "09:15 AM",
    messages: [
      { role: "user", content: "Shop open today?" },
      { role: "assistant", content: "Yes, we are open from 8:00 AM to 6:00 PM today." }
    ]
  }
];

export default function AIHistoryPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto space-y-6">
        <Link href="/dashboard" className="inline-flex items-center gap-2 text-slate-400 hover:text-white transition-colors">
          <ArrowLeft className="w-4 h-4" />
          <span className="font-medium">Back to Dashboard</span>
        </Link>
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-blue-500/20 rounded-2xl flex items-center justify-center border border-blue-500/30 backdrop-blur-xl">
              <Bot className="w-6 h-6 text-blue-400" />
            </div>
            <div>
              <h2 className="text-2xl md:text-3xl font-bold text-white tracking-tight">AI Chat History</h2>
              <p className="text-slate-400 text-xs md:text-sm">Review automated conversations and interactions</p>
            </div>
          </div>
          
          <div className="relative w-full md:w-72">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input 
              type="text" 
              placeholder="Search phone number..." 
              className="w-full pl-10 pr-4 py-2.5 bg-slate-800/50 border border-white/10 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all text-sm backdrop-blur-xl"
            />
          </div>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          {chatHistory.map((chat) => (
            <Card key={chat.id} className="h-[600px] flex flex-col bg-slate-800/50 border-white/10 backdrop-blur-xl shadow-xl">
              <CardHeader className="border-b border-white/10 pb-4 bg-slate-900/30">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-white font-bold shadow-lg shadow-blue-500/20">
                      {chat.name.charAt(0)}
                    </div>
                    <div>
                      <CardTitle className="text-base text-white font-bold">{chat.name}</CardTitle>
                      <p className="text-xs text-blue-400 font-medium flex items-center gap-1">
                        <User className="w-3 h-3" />
                        {chat.customer}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 px-3 py-1 rounded-full bg-slate-700/50 border border-white/5">
                    <Calendar className="w-3 h-3 text-slate-400" />
                    <span className="text-xs text-slate-300 font-medium">{chat.timestamp}</span>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="flex-1 overflow-y-auto p-4 space-y-4 custom-scrollbar">
                {chat.messages.map((msg, idx) => (
                  <div
                    key={idx}
                    className={`flex ${msg.role === 'user' ? 'justify-start' : 'justify-end'}`}
                  >
                    <div className={`flex flex-col max-w-[80%] ${msg.role === 'user' ? 'items-start' : 'items-end'}`}>
                      <div
                        className={`rounded-2xl px-4 py-3 text-sm shadow-md ${
                          msg.role === 'user'
                            ? 'bg-slate-700 text-slate-200 rounded-tl-none border border-white/5'
                            : 'bg-blue-600 text-white rounded-tr-none shadow-blue-600/20'
                        }`}
                      >
                        {msg.content}
                      </div>
                      <span className="text-[10px] text-slate-500 mt-1 px-1">
                        {msg.role === 'user' ? 'Customer' : 'AI Assistant'}
                      </span>
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
      
      <style jsx global>{`
        .custom-scrollbar::-webkit-scrollbar {
          width: 6px;
        }
        .custom-scrollbar::-webkit-scrollbar-track {
          background: rgba(30, 41, 59, 0.5);
        }
        .custom-scrollbar::-webkit-scrollbar-thumb {
          background: rgba(71, 85, 105, 0.8);
          border-radius: 10px;
        }
        .custom-scrollbar::-webkit-scrollbar-thumb:hover {
          background: rgba(96, 165, 250, 0.5);
        }
      `}</style>
    </div>
  );
}
