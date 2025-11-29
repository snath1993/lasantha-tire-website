'use client'

import { Phone, Mail, MapPin, Facebook, Clock, ArrowRight, Instagram, Linkedin } from 'lucide-react'
import Image from 'next/image'
import Link from 'next/link'

export default function Footer() {
  const currentYear = new Date().getFullYear()

  return (
    <footer id="contact" className="bg-dark-950 text-white pt-20 pb-10 relative overflow-hidden">
      {/* Background Elements */}
      <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none opacity-10">
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-primary-600 rounded-full blur-[100px]"></div>
        <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-blue-600 rounded-full blur-[100px]"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8 sm:gap-10 lg:gap-12 mb-12 sm:mb-16">
          {/* Company Info */}
          <div className="space-y-6">
            <div className="flex items-center gap-3">
              <div className="w-14 h-14 rounded-xl overflow-hidden shadow-lg bg-white p-1">
                <Image
                  src="/images/lasantha-logo.png"
                  alt="Lasantha Tyre Traders logo"
                  width={56}
                  height={56}
                  className="object-cover w-full h-full rounded-lg"
                />
              </div>
              <div>
                <h3 className="text-xl font-bold text-white leading-none mb-1">LASANTHA</h3>
                <p className="text-xs font-bold text-primary-500 tracking-widest">TYRE TRADERS</p>
              </div>
            </div>
            <p className="text-gray-400 leading-relaxed text-sm">
              Sri Lanka&apos;s premier tyre service center. We combine decades of experience with modern technology to ensure your safety on the road.
            </p>
            <div className="flex gap-4">
              {[
                { icon: Facebook, href: "https://facebook.com/lasanthatyretraders", color: "hover:bg-[#1877F2]" },
                { icon: Instagram, href: "#", color: "hover:bg-[#E4405F]" },
                { icon: Linkedin, href: "#", color: "hover:bg-[#0A66C2]" }
              ].map((social, idx) => (
                <a 
                  key={idx}
                  href={social.href}
                  target="_blank" 
                  rel="noopener noreferrer"
                  className={`w-10 h-10 bg-white/5 rounded-lg flex items-center justify-center transition-all duration-300 ${social.color} hover:text-white text-gray-400`}
                >
                  <social.icon className="w-5 h-5" />
                </a>
              ))}
            </div>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="text-lg font-bold text-white mb-6">Quick Links</h4>
            <ul className="space-y-3">
              {['Home', 'Services', 'Brands', 'Get Quote', 'Contact'].map((item) => (
                <li key={item}>
                  <Link 
                    href={item === 'Home' ? '/' : `#${item.toLowerCase().replace(' ', '-')}`}
                    className="text-gray-400 hover:text-primary-400 transition-colors flex items-center gap-2 group text-sm"
                  >
                    <ArrowRight className="w-3 h-3 opacity-0 group-hover:opacity-100 transition-opacity" />
                    <span className="group-hover:translate-x-1 transition-transform">{item}</span>
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Services */}
          <div>
            <h4 className="text-lg font-bold text-white mb-6">Our Services</h4>
            <ul className="space-y-3">
              {['Tyre Installation', 'Wheel Alignment', 'Wheel Balancing', 'Nitrogen Filling', 'Puncture Repair'].map((item) => (
                <li key={item}>
                  <Link 
                    href="#services"
                    className="text-gray-400 hover:text-primary-400 transition-colors flex items-center gap-2 group text-sm"
                  >
                    <ArrowRight className="w-3 h-3 opacity-0 group-hover:opacity-100 transition-opacity" />
                    <span className="group-hover:translate-x-1 transition-transform">{item}</span>
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Contact Info */}
          <div>
            <h4 className="text-lg font-bold text-white mb-6">Contact Us</h4>
            <ul className="space-y-4">
              <li>
                <a href="tel:+94773131883" className="flex items-start gap-4 text-gray-400 hover:text-primary-400 transition-colors group">
                  <div className="w-8 h-8 rounded-lg bg-red-500/10 flex items-center justify-center group-hover:bg-red-500/20 transition-colors flex-shrink-0">
                    <Phone className="w-4 h-4 text-red-500" />
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Hotline</p>
                    <p className="text-sm font-bold text-white">077 313 1883</p>
                  </div>
                </a>
              </li>
              <li>
                <a href="tel:+94721222509" className="flex items-start gap-4 text-gray-400 hover:text-primary-400 transition-colors group">
                  <div className="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center group-hover:bg-primary-500/20 transition-colors flex-shrink-0">
                    <Phone className="w-4 h-4" />
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Call Us</p>
                    <p className="text-sm font-medium text-white">072 122 2509</p>
                  </div>
                </a>
              </li>
              <li>
                <a href="mailto:info@lasanthatyre.com" className="flex items-start gap-4 text-gray-400 hover:text-primary-400 transition-colors group">
                  <div className="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center group-hover:bg-primary-500/20 transition-colors flex-shrink-0">
                    <Mail className="w-4 h-4" />
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Email Us</p>
                    <p className="text-sm font-medium text-white">info@lasanthatyre.com</p>
                  </div>
                </a>
              </li>
              <li className="flex items-start gap-4 text-gray-400 group">
                <div className="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center group-hover:bg-primary-500/20 transition-colors flex-shrink-0">
                  <MapPin className="w-4 h-4" />
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Visit Us</p>
                  <p className="text-sm font-medium text-white">1035 Pannipitiya Road<br />Battaramulla 10230</p>
                </div>
              </li>
              <li className="flex items-start gap-4 text-gray-400 group">
                <div className="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center group-hover:bg-primary-500/20 transition-colors flex-shrink-0">
                  <Clock className="w-4 h-4" />
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Opening Hours</p>
                  <p className="text-sm font-medium text-white">Daily: 06:30 AM - 9:00 PM</p>
                </div>
              </li>
            </ul>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="border-t border-white/10 pt-8 flex flex-col md:flex-row items-center justify-between gap-4">
          <p className="text-gray-500 text-sm text-center md:text-left">
            &copy; {currentYear} Lasantha Tyre Traders. All rights reserved.
          </p>
          <div className="flex items-center gap-6 text-sm text-gray-500">
            <a href="#" className="hover:text-white transition-colors">Privacy Policy</a>
            <a href="#" className="hover:text-white transition-colors">Terms of Service</a>
          </div>
        </div>
      </div>
    </footer>
  )
}
