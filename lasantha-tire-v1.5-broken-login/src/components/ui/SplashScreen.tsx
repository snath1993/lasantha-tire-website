'use client';

import { motion } from 'framer-motion';
import Image from 'next/image';

export default function SplashScreen() {
  return (
    <motion.div
      className="fixed inset-0 z-[100] flex flex-col items-center justify-center bg-white"
      initial={{ opacity: 1 }}
      exit={{ opacity: 0, filter: 'blur(10px)' }}
      transition={{ duration: 0.8, ease: "easeInOut" }}
    >
      {/* Animated Background Elements */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <motion.div 
          animate={{ 
            scale: [1, 1.2, 1],
            opacity: [0.2, 0.4, 0.2],
            rotate: [0, 90, 0]
          }}
          transition={{ duration: 8, repeat: Infinity, ease: "linear" }}
          className="absolute -top-[30%] -left-[10%] w-[80%] h-[80%] bg-blue-100 rounded-full blur-[120px]" 
        />
        <motion.div 
          animate={{ 
            scale: [1, 1.3, 1],
            opacity: [0.1, 0.3, 0.1],
            rotate: [0, -60, 0]
          }}
          transition={{ duration: 10, repeat: Infinity, ease: "linear", delay: 1 }}
          className="absolute -bottom-[30%] -right-[10%] w-[80%] h-[80%] bg-emerald-100 rounded-full blur-[120px]" 
        />
      </div>

      <div className="relative flex flex-col items-center z-10">
        {/* Logo Container */}
        <motion.div
          initial={{ scale: 0.5, opacity: 0, y: 20 }}
          animate={{ scale: 1, opacity: 1, y: 0 }}
          transition={{ duration: 0.8, type: "spring", bounce: 0.5 }}
          className="mb-10 relative"
        >
          {/* Glowing Effect behind logo */}
          <motion.div
            animate={{ 
              boxShadow: [
                "0 0 20px rgba(37, 99, 235, 0.1)",
                "0 0 60px rgba(37, 99, 235, 0.3)",
                "0 0 20px rgba(37, 99, 235, 0.1)"
              ]
            }}
            transition={{ duration: 2, repeat: Infinity }}
            className="w-28 h-28 bg-white rounded-3xl flex items-center justify-center relative z-10 overflow-hidden shadow-xl shadow-slate-200"
          >
            <Image 
              src="/shop-logo.jpg" 
              alt="Lasantha Tire Service" 
              width={112} 
              height={112} 
              className="w-full h-full object-cover"
            />
          </motion.div>

          {/* Ripple/Pulse Rings */}
          {[1, 2, 3].map((i) => (
            <motion.div
              key={i}
              className="absolute inset-0 bg-blue-500/10 rounded-3xl -z-10"
              initial={{ scale: 1, opacity: 0.5 }}
              animate={{ scale: 1.8, opacity: 0 }}
              transition={{ 
                duration: 2, 
                repeat: Infinity, 
                delay: i * 0.4,
                ease: "easeOut"
              }}
            />
          ))}
        </motion.div>

        {/* Text Animation */}
        <div className="text-center space-y-3">
          <motion.h1 
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3, duration: 0.6 }}
            className="text-4xl font-bold text-slate-900 tracking-tight"
          >
            Lasantha <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-emerald-600">Tire</span>
          </motion.h1>
          
          <motion.div 
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.6, duration: 0.6 }}
            className="flex items-center justify-center gap-3"
          >
            <div className="h-[1px] w-8 bg-slate-200" />
            <p className="text-slate-500 text-xs font-bold tracking-[0.3em] uppercase">
              Smart Dashboard
            </p>
            <div className="h-[1px] w-8 bg-slate-200" />
          </motion.div>
        </div>

        {/* Loading Indicator */}
        <motion.div 
          initial={{ opacity: 0, width: 0 }}
          animate={{ opacity: 1, width: "12rem" }}
          transition={{ delay: 0.8, duration: 0.5 }}
          className="mt-16 h-1 bg-slate-100 rounded-full overflow-hidden relative"
        >
          <motion.div
            className="absolute inset-y-0 left-0 bg-gradient-to-r from-blue-500 to-emerald-500"
            initial={{ width: "0%" }}
            animate={{ width: "100%" }}
            transition={{ 
              duration: 2,
              ease: "easeInOut",
              repeat: Infinity,
              repeatType: "reverse"
            }}
          />
        </motion.div>
      </div>
    </motion.div>
  );
}
