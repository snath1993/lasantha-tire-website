import React from 'react';
import BusinessStatusReports from '@/views/desktop/BusinessStatusReports';
import { Briefcase } from 'lucide-react';

export const metadata = {
  title: 'Business Status | Lasantha Tyre',
  description: 'Financial overview and business status reports',
};

export default function BusinessStatusPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900 p-6">
      <div className="max-w-7xl mx-auto mb-8">
        <div className="flex items-center gap-4 mb-2">
          <div className="w-12 h-12 bg-slate-800/50 rounded-2xl backdrop-blur-xl border border-white/10 flex items-center justify-center shadow-xl">
            <Briefcase className="w-6 h-6 text-blue-400" />
          </div>
          <div>
            <h1 className="text-3xl font-bold text-white tracking-tight">Business Status</h1>
            <p className="text-slate-400 font-medium">
              Real-time financial overview, aging reports, and cash position
            </p>
          </div>
        </div>
      </div>
      
      <div className="max-w-7xl mx-auto">
        <BusinessStatusReports />
      </div>
    </div>
  );
}
