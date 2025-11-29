'use client'

import { motion } from 'framer-motion'
import { FileText, Download, ArrowRight } from 'lucide-react'

export default function TyreCareBlueprint() {
  return (
    <section className="py-12 md:py-20 bg-gray-900 text-white relative overflow-hidden">
      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-10">
        <div className="absolute -top-24 -right-24 w-64 md:w-96 h-64 md:h-96 rounded-full bg-primary-500 blur-3xl"></div>
        <div className="absolute -bottom-24 -left-24 w-64 md:w-96 h-64 md:h-96 rounded-full bg-blue-500 blur-3xl"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <div className="flex flex-col lg:flex-row items-center gap-12 lg:gap-20">
          
          {/* Text Content */}
          <div className="flex-1 text-center lg:text-left">
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.6 }}
            >
              <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/10 border border-white/20 text-primary-300 text-sm font-semibold mb-6">
                <FileText className="w-4 h-4" />
                Featured Guide
              </div>
              
              <h2 className="text-3xl md:text-5xl font-bold mb-6 leading-tight">
                Tyre Care Blueprint <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-primary-200 block md:inline">(Sri Lanka)</span>
              </h2>
              
              <p className="text-gray-300 text-lg mb-8 max-w-xl mx-auto lg:mx-0">
                High-humidity, stop-go Colombo traffic එකට හොඳම weekly + monthly routine එක මෙන්න.
              </p>

              <div className="space-y-4 mb-10 text-left max-w-lg mx-auto lg:mx-0">
                <div className="flex items-start gap-3">
                  <div className="mt-1 bg-primary-500/20 p-1 rounded text-primary-400 font-bold text-xs uppercase tracking-wider w-20 text-center flex-shrink-0">
                    Weekly
                  </div>
                  <span className="text-gray-200">Cold PSI + Tread wear bar check</span>
                </div>
                <div className="flex items-start gap-3">
                  <div className="mt-1 bg-blue-500/20 p-1 rounded text-blue-400 font-bold text-xs uppercase tracking-wider w-20 text-center flex-shrink-0">
                    Monthly
                  </div>
                  <span className="text-gray-200">Wheel balancing + Rotation log update</span>
                </div>
                <div className="flex items-start gap-3">
                  <div className="mt-1 bg-purple-500/20 p-1 rounded text-purple-400 font-bold text-xs uppercase tracking-wider w-20 text-center flex-shrink-0">
                    Quarterly
                  </div>
                  <span className="text-gray-200">Roadside emergency kit + Spare inspection</span>
                </div>
              </div>

              <motion.a
                href="/downloads/Tyre_Care_Blueprint_Sri_Lanka.html"
                target="_blank"
                rel="noopener noreferrer"
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                className="inline-flex items-center gap-3 bg-primary-600 hover:bg-primary-700 text-white px-8 py-4 rounded-xl font-bold text-lg transition-all shadow-lg shadow-primary-600/30 group"
              >
                <Download className="w-5 h-5 group-hover:-translate-y-1 transition-transform" />
                Download Checklist
                <ArrowRight className="w-5 h-5 opacity-0 group-hover:opacity-100 -ml-4 group-hover:ml-0 transition-all" />
              </motion.a>
              
              <p className="mt-4 text-sm text-gray-400">
                *Instant download. No email required.
              </p>
            </motion.div>
          </div>

          {/* Visual Representation */}
          <div className="flex-1 relative w-full max-w-md lg:max-w-none">
            <motion.div
              initial={{ opacity: 0, x: 50 }}
              whileInView={{ opacity: 1, x: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.8 }}
              className="relative z-10"
            >
              {/* Document Preview Card */}
              <div className="bg-white text-gray-900 rounded-2xl shadow-2xl p-6 md:p-8 max-w-md mx-auto transform rotate-3 hover:rotate-0 transition-transform duration-500 scale-95 md:scale-100">
                <div className="border-b-2 border-primary-600 pb-4 mb-6">
                  <h3 className="text-xl md:text-2xl font-bold text-gray-900">Tyre Care Blueprint</h3>
                  <p className="text-primary-600 font-medium uppercase tracking-wider text-xs md:text-sm">Sri Lanka Edition</p>
                </div>
                
                <div className="space-y-6">
                  <div className="flex items-center gap-4 p-3 bg-gray-50 rounded-lg border border-gray-100">
                    <div className="w-8 h-8 rounded-full bg-primary-100 flex items-center justify-center text-primary-600 font-bold text-sm">W</div>
                    <div className="text-sm font-semibold text-gray-700">Cold PSI Check</div>
                  </div>
                  <div className="flex items-center gap-4 p-3 bg-gray-50 rounded-lg border border-gray-100">
                    <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-sm">M</div>
                    <div className="text-sm font-semibold text-gray-700">Rotation Log</div>
                  </div>
                  <div className="flex items-center gap-4 p-3 bg-gray-50 rounded-lg border border-gray-100">
                    <div className="w-8 h-8 rounded-full bg-purple-100 flex items-center justify-center text-purple-600 font-bold text-sm">Q</div>
                    <div className="text-sm font-semibold text-gray-700">Emergency Kit</div>
                  </div>
                </div>
              </div>

              {/* Decorative Elements */}
              <div className="absolute -top-10 -right-10 w-20 h-20 bg-yellow-400 rounded-full blur-xl opacity-20"></div>
              <div className="absolute -bottom-10 -left-10 w-32 h-32 bg-primary-600 rounded-full blur-xl opacity-20"></div>
            </motion.div>
          </div>

        </div>
      </div>
    </section>
  )
}
