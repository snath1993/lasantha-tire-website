'use client'

import { motion, AnimatePresence } from 'framer-motion'
import { Phone, Mail, Clock, Menu, X, ChevronRight } from 'lucide-react'
import Image from 'next/image'
import Link from 'next/link'
import { useState, useEffect } from 'react'

export default function Header() {
  const [isScrolled, setIsScrolled] = useState(false)
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 20)
    }
    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])

  const navLinks = [
    { name: 'Home', href: '/' },
    { name: 'Services', href: '/services' },
    { name: 'Gallery', href: '/gallery' },
    { name: 'Contact', href: '/contact' },
  ]

  return (
    <>
      <header 
        className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${
          isScrolled ? 'bg-white/95 backdrop-blur-md shadow-md py-2' : 'bg-white/80 backdrop-blur-sm py-4'
        }`}
      >
        <div className="container mx-auto px-4">
          {/* Top Bar (Hidden on Scroll) */}
          <motion.div 
            initial={{ height: 'auto', opacity: 1 }}
            animate={{ height: isScrolled ? 0 : 'auto', opacity: isScrolled ? 0 : 1 }}
            className="hidden md:flex items-center justify-between border-b border-gray-200/50 pb-2 mb-2 overflow-hidden"
          >
            <div className="flex items-center gap-6 text-sm text-gray-600 font-medium">
              <a href="tel:+94773131883" className="flex items-center gap-2 hover:text-primary-600 transition-colors group">
                <div className="w-6 h-6 rounded-full bg-red-50 flex items-center justify-center group-hover:bg-red-100 transition-colors">
                  <Phone className="w-3 h-3 text-red-600" />
                </div>
                <span className="font-bold text-red-600">Hotline: 077 313 1883</span>
              </a>
              <a href="tel:+94721222509" className="flex items-center gap-2 hover:text-primary-600 transition-colors group">
                <div className="w-6 h-6 rounded-full bg-primary-50 flex items-center justify-center group-hover:bg-primary-100 transition-colors">
                  <Phone className="w-3 h-3 text-primary-600" />
                </div>
                <span>072 122 2509</span>
              </a>
              <a href="mailto:info@lasanthatyre.com" className="flex items-center gap-2 hover:text-primary-600 transition-colors group">
                <div className="w-6 h-6 rounded-full bg-primary-50 flex items-center justify-center group-hover:bg-primary-100 transition-colors">
                  <Mail className="w-3 h-3 text-primary-600" />
                </div>
                <span>info@lasanthatyre.com</span>
              </a>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-600 font-medium bg-gray-50 px-3 py-1 rounded-full">
              <Clock className="w-3 h-3 text-primary-600" />
              <span>Open 365 Days: 06:30 AM - 9:00 PM</span>
            </div>
          </motion.div>

          {/* Main Navigation */}
          <nav className="flex items-center justify-between">
            <Link href="/">
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                className="flex items-center gap-3 cursor-pointer group"
              >
                <div className="w-10 h-10 md:w-12 md:h-12 rounded-xl overflow-hidden shadow-lg bg-white ring-2 ring-gray-100 group-hover:ring-primary-100 transition-all">
                  <Image
                    src="/images/lasantha-logo.png"
                    alt="Lasantha Tyre Traders logo"
                    width={48}
                    height={48}
                    className="object-cover w-full h-full"
                    priority
                  />
                </div>
                <div className="flex flex-col">
                  <h1 className="text-lg md:text-xl font-bold text-gray-900 leading-none tracking-tight group-hover:text-primary-700 transition-colors">
                    LASANTHA
                  </h1>
                  <span className="text-xs md:text-sm font-semibold text-primary-600 tracking-widest">TYRE TRADERS</span>
                </div>
              </motion.div>
            </Link>

            {/* Desktop Menu */}
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              className="hidden lg:flex items-center gap-1"
            >
              {navLinks.map((link) => (
                <Link 
                  key={link.name}
                  href={link.href} 
                  className="px-4 py-2 text-gray-600 hover:text-primary-600 font-semibold rounded-lg hover:bg-primary-50 transition-all text-sm"
                >
                  {link.name}
                </Link>
              ))}
              
              <motion.a
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                href="https://wa.me/94721222509?text=Hi,%20I%20need%20a%20tyre%20quote"
                target="_blank"
                rel="noopener noreferrer"
                className="ml-4 bg-gradient-to-r from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 text-white px-6 py-2.5 rounded-xl font-bold transition-all flex items-center gap-2 shadow-lg shadow-green-500/20"
              >
                <Phone className="w-4 h-4" />
                <span>WhatsApp</span>
              </motion.a>
            </motion.div>

            {/* Mobile Menu Button */}
            <button 
              className="lg:hidden p-2 text-gray-600 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </nav>
        </div>
      </header>

      {/* Mobile Menu Overlay */}
      <AnimatePresence>
        {isMobileMenuOpen && (
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            className="fixed inset-0 z-40 bg-white pt-24 px-4 lg:hidden"
          >
            <div className="flex flex-col gap-2">
              {navLinks.map((link) => (
                <Link
                  key={link.name}
                  href={link.href}
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="flex items-center justify-between p-4 rounded-xl hover:bg-gray-50 text-lg font-semibold text-gray-800 border-b border-gray-100 last:border-0"
                >
                  {link.name}
                  <ChevronRight className="w-5 h-5 text-gray-400" />
                </Link>
              ))}
              
              <a
                href="https://wa.me/94721222509?text=Hi,%20I%20need%20a%20tyre%20quote"
                target="_blank"
                rel="noopener noreferrer"
                className="mt-4 w-full bg-green-500 text-white p-4 rounded-xl font-bold flex items-center justify-center gap-2"
              >
                <Phone className="w-5 h-5" />
                Chat on WhatsApp
              </a>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  )
}
