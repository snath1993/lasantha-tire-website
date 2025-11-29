'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { ArrowRight, Shield, Wrench, Clock, Star, Calculator, Calendar } from 'lucide-react'
import Link from 'next/link'
import Image from 'next/image'
import QuoteModal from './QuoteModal'
import CalculatorModal from './CalculatorModal'
import BookingModal from './BookingModal'

export default function Hero() {
  const [isQuoteModalOpen, setIsQuoteModalOpen] = useState(false)
  const [isCalculatorOpen, setIsCalculatorOpen] = useState(false)
  const [isBookingOpen, setIsBookingOpen] = useState(false)

  return (
    <section className="relative min-h-[100svh] flex items-center justify-center overflow-hidden bg-dark-950 pt-28 pb-12 sm:pt-32 md:pt-24 md:pb-0">
      {/* Advanced Background */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        {/* Gradient Orbs */}
        <motion.div
          animate={{
            scale: [1, 1.2, 1],
            rotate: [0, 90, 0],
            opacity: [0.3, 0.5, 0.3]
          }}
          transition={{
            duration: 15,
            repeat: Infinity,
            ease: "linear"
          }}
          className="absolute -top-[20%] -left-[10%] w-[300px] md:w-[800px] h-[300px] md:h-[800px] bg-primary-900/20 rounded-full blur-[80px] md:blur-[120px]"
        />
        <motion.div
          animate={{
            scale: [1.2, 1, 1.2],
            rotate: [90, 0, 90],
            opacity: [0.2, 0.4, 0.2]
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: "linear"
          }}
          className="absolute -bottom-[20%] -right-[10%] w-[300px] md:w-[800px] h-[300px] md:h-[800px] bg-blue-900/20 rounded-full blur-[80px] md:blur-[120px]"
        />
        
        {/* Grid Pattern */}
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-[0.03]"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <div className="grid lg:grid-cols-2 gap-16 items-center">
          {/* Left Content */}
          <div className="text-white max-w-2xl">
            <motion.div
              initial={{ opacity: 0, x: -30 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.6 }}
              className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/5 border border-white/10 mb-8 backdrop-blur-sm"
            >
              <Star className="w-4 h-4 text-yellow-400 fill-yellow-400" />
              <span className="text-sm font-medium text-gray-300">#1 Rated Tyre Center in Colombo</span>
            </motion.div>

            <motion.h1
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.1 }}
              className="text-3xl sm:text-4xl md:text-5xl lg:text-7xl font-bold mb-6 leading-[1.1] tracking-tight"
            >
              Go Smart with <br />
              <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-primary-200">
                Lasantha Tyre Traders
              </span>
            </motion.h1>

            <motion.p
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.2 }}
              className="text-base sm:text-lg md:text-xl text-gray-400 mb-8 md:mb-10 leading-relaxed max-w-lg"
            >
              Experience the perfect blend of premium tyres, expert installation, and unmatched service quality at Lasantha Tyre Traders.
            </motion.p>

            <motion.div
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.3 }}
              className="flex flex-col gap-4"
            >
              <div className="flex flex-col sm:flex-row gap-4">
                <motion.button
                  whileHover={{ scale: 1.05 }}
                  whileTap={{ scale: 0.95 }}
                  onClick={() => setIsQuoteModalOpen(true)}
                  className="group relative bg-primary-600 hover:bg-primary-500 text-white px-8 py-4 rounded-xl font-bold transition-all flex items-center justify-center gap-2 shadow-lg shadow-primary-900/20 overflow-hidden flex-1 sm:flex-none"
                >
                  {/* Pulse Effect */}
                  <span className="absolute inset-0 w-full h-full bg-white/20 animate-pulse rounded-xl"></span>
                  
                  <div className="relative flex flex-col items-start text-left">
                    <span className="text-sm font-medium opacity-90">Get Your Free Quote</span>
                    <span className="text-xs font-normal opacity-80">නොමිලේ මිල ගණන් ලබාගන්න</span>
                  </div>
                  <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform ml-2" />
                </motion.button>
                
                <Link
                  href="/services"
                  className="group bg-white/5 hover:bg-white/10 text-white px-8 py-4 rounded-xl font-bold transition-all border border-white/10 hover:border-white/20 flex items-center justify-center backdrop-blur-sm flex-1 sm:flex-none"
                >
                  Explore Services
                </Link>
              </div>

              <div className="flex flex-col sm:flex-row gap-4">
                <button
                  onClick={() => setIsCalculatorOpen(true)}
                  className="group flex items-center justify-center gap-2 px-6 py-3 rounded-xl bg-gray-800/50 hover:bg-gray-800 text-gray-300 hover:text-white border border-gray-700 hover:border-gray-600 transition-all backdrop-blur-sm text-sm font-medium flex-1 sm:flex-none"
                >
                  <Calculator className="w-4 h-4 text-primary-400" />
                  Tyre Size Calculator
                </button>
                <button
                  onClick={() => setIsBookingOpen(true)}
                  className="group flex items-center justify-center gap-2 px-6 py-3 rounded-xl bg-gray-800/50 hover:bg-gray-800 text-gray-300 hover:text-white border border-gray-700 hover:border-gray-600 transition-all backdrop-blur-sm text-sm font-medium flex-1 sm:flex-none"
                >
                  <Calendar className="w-4 h-4 text-primary-400" />
                  Book VIP Service
                </button>
              </div>
            </motion.div>

            {/* Trust Indicators */}
            <motion.div
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, delay: 0.4 }}
              className="grid grid-cols-3 gap-8 mt-16 border-t border-white/10 pt-8"
            >
              <div>
                <h3 className="text-3xl font-bold text-white mb-1">22+</h3>
                <p className="text-sm text-gray-400">Years of Trust</p>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-white mb-1">50+</h3>
                <p className="text-sm text-gray-400">Global Brands</p>
              </div>
              <div>
                <h3 className="text-3xl font-bold text-white mb-1">10k+</h3>
                <p className="text-sm text-gray-400">Happy Clients</p>
              </div>
            </motion.div>
          </div>

          {/* Right Content - Visuals */}
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.8, delay: 0.2 }}
            className="relative hidden lg:block"
          >
            {/* Main Visual Container */}
            <div className="relative w-full aspect-square max-w-[800px] mx-auto flex items-center justify-center">
              
              {/* Real Tyre Image */}
              <motion.div
                animate={{ 
                  rotate: 360,
                  y: [-10, 10, -10] // Add floating effect
                }}
                transition={{ 
                  rotate: { duration: 60, repeat: Infinity, ease: "linear" },
                  y: { duration: 4, repeat: Infinity, ease: "easeInOut" }
                }}
                className="relative w-[400px] md:w-[900px] h-[400px] md:h-[900px] z-10 flex items-center justify-center"
              >
                {/* Local asset keeps the hero tyre reliable even when third-party CDNs fail */}
                <div className="absolute inset-0 bg-primary-500/30 blur-[60px] rounded-full z-0"></div>
                <Image
                  src="/images/new.png"
                  alt="Premium Tyre"
                  width={900}
                  height={900}
                  className="w-full h-full object-contain drop-shadow-2xl relative z-10"
                  priority
                />
              </motion.div>

              {/* Background Glow */}
              <div className="absolute inset-0 bg-primary-600/20 blur-[100px] rounded-full transform scale-75"></div>

              {/* Floating Feature Cards */}
              <motion.div
                animate={{ y: [-10, 10, -10] }}
                transition={{ duration: 4, repeat: Infinity, ease: "easeInOut" }}
                className="absolute top-10 right-0 bg-white/10 backdrop-blur-md border border-white/20 p-4 rounded-2xl flex items-center gap-4 shadow-xl z-20"
              >
                <div className="w-10 h-10 rounded-full bg-green-500/20 flex items-center justify-center">
                  <Shield className="w-5 h-5 text-green-400" />
                </div>
                <div>
                  <p className="text-white font-bold">Warranty</p>
                  <p className="text-xs text-gray-300">Guaranteed Protection</p>
                </div>
              </motion.div>

              <motion.div
                animate={{ y: [10, -10, 10] }}
                transition={{ duration: 5, repeat: Infinity, ease: "easeInOut", delay: 1 }}
                className="absolute bottom-20 -left-10 bg-white/10 backdrop-blur-md border border-white/20 p-4 rounded-2xl flex items-center gap-4 shadow-xl z-20"
              >
                <div className="w-10 h-10 rounded-full bg-blue-500/20 flex items-center justify-center">
                  <Wrench className="w-5 h-5 text-blue-400" />
                </div>
                <div>
                  <p className="text-white font-bold">Expert Care</p>
                  <p className="text-xs text-gray-300">Certified Technicians</p>
                </div>
              </motion.div>

              <motion.div
                animate={{ x: [-5, 5, -5] }}
                transition={{ duration: 6, repeat: Infinity, ease: "easeInOut", delay: 0.5 }}
                className="absolute top-1/2 -right-16 bg-white/10 backdrop-blur-md border border-white/20 p-4 rounded-2xl flex items-center gap-4 shadow-xl z-20"
              >
                <div className="w-10 h-10 rounded-full bg-purple-500/20 flex items-center justify-center">
                  <Clock className="w-5 h-5 text-purple-400" />
                </div>
                <div>
                  <p className="text-white font-bold">Fast Service</p>
                  <p className="text-xs text-gray-300">Quick Turnaround</p>
                </div>
              </motion.div>
            </div>
          </motion.div>
        </div>
      </div>

      {/* Quote Modal */}
      <QuoteModal isOpen={isQuoteModalOpen} onClose={() => setIsQuoteModalOpen(false)} />
      <CalculatorModal isOpen={isCalculatorOpen} onClose={() => setIsCalculatorOpen(false)} />
      <BookingModal isOpen={isBookingOpen} onClose={() => setIsBookingOpen(false)} />
    </section>
  )
}

