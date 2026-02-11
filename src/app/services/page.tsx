'use client'

import React from 'react'
import { motion } from 'framer-motion'
import { Package, Wrench, Gauge, Shield, Award, CheckCircle2, Calendar, ArrowRight, Star } from 'lucide-react'
import Header from '@/components/Header'
import Footer from '@/components/Footer'
import FleetPortal from '@/components/FleetPortal'

const services = [
  {
    icon: Package,
    title: 'Premium Tyre Sales',
    description: 'We are authorized dealers for world-renowned tyre brands. Our experts help you find the perfect match for your vehicle\'s needs and your budget.',
    features: ['All Major Brands (Michelin, Bridgestone, etc.)', 'Manufacturer Warranty Included', 'Competitive Market Rates', 'Expert Recommendations'],
    color: 'from-blue-500 to-blue-600',
    bg: 'bg-blue-50',
    link: 'https://book.lasanthatyre.com',
    popular: true
  },
  {
    icon: Wrench,
    title: 'Expert Installation',
    description: 'Our certified technicians use state-of-the-art mounting equipment to ensure your tyres are installed safely without damaging your rims.',
    features: ['Touchless Tyre Mounting', 'Advanced Laser Balancing', 'Rim Protection Guarantee', 'Quick Turnaround Time'],
    color: 'from-green-500 to-green-600',
    bg: 'bg-green-50',
    link: 'https://book.lasanthatyre.com'
  },
  {
    icon: Gauge,
    title: 'Italian CCD Wheel Alignment',
    description: 'Precision alignment using advanced Italian Technology. The TRIGON 725 PRO system with 8 CCD cameras ensures your vehicle handles perfectly.',
    features: ['Italian Technology', 'Detailed Computerized Reports', 'Suspension Checkup Included', 'Extended Tyre Life'],
    color: 'from-purple-500 to-purple-600',
    bg: 'bg-purple-50',
    link: 'https://book.lasanthatyre.com',
    popular: true
  },
  {
    icon: Shield,
    title: 'Warranty Claims',
    description: 'We handle all manufacturer warranty claims directly, saving you time and hassle. We stand behind every product we sell.',
    features: ['Direct Manufacturer Links', 'Hassle-free Processing', 'Replacement Assistance', 'Transparent Policies'],
    color: 'from-red-500 to-red-600',
    bg: 'bg-red-50',
    link: 'https://book.lasanthatyre.com'
  },
  {
    icon: Award,
    title: 'Nitrogen Inflation',
    description: 'Keep your tyre pressure stable for longer with Nitrogen inflation. It improves fuel efficiency and reduces tyre wear.',
    features: ['Maintains Pressure Context', 'Runs Cooler', 'Prevents Rim Oxidation', 'Free Top-ups for Customers'],
    color: 'from-orange-500 to-orange-600',
    bg: 'bg-orange-50',
    link: 'https://book.lasanthatyre.com'
  }
]

export default function ServicesPage() {
  const handleBooking = async () => {
    try {
      // Try to reach the booking subdomain
      await fetch('https://book.lasanthatyre.com', { 
        mode: 'no-cors',
        method: 'HEAD',
        cache: 'no-store'
      })
      // If successful (no network error), navigate
      window.location.href = 'https://book.lasanthatyre.com'
    } catch {
      // If network error (site unreachable), show fallback
      alert('Appointments system is currently unavailable. Please call 011 2 77 32 32 to book your time.\n\nඇපොයින්ට්මන්ට් පද්ධතිය තාවකාලිකව අක්‍රියයි. කරුණාකර 011 2 77 32 32 අමතා වේලාවක් වෙන් කරවා ගන්න.')
    }
  }

  return (
    <div className="min-h-screen bg-gray-50 font-sans selection:bg-primary-100">
      <Header />
      
      <main className="pt-20">
        {/* Hero Section */}
        <section className="relative bg-dark-950 text-white py-24 overflow-hidden">
          {/* Background Gradients */}
          <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
            <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-primary-600/20 rounded-full blur-[120px]"></div>
            <div className="absolute bottom-0 left-0 w-[500px] h-[500px] bg-blue-600/20 rounded-full blur-[120px]"></div>
          </div>

          <div className="container mx-auto px-4 relative z-10 text-center">
            <motion.div
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6 }}
            >
              <h2 className="text-primary-500 font-bold tracking-widest uppercase text-sm mb-4">World-Class Automotive Care</h2>
              <h1 className="text-4xl md:text-6xl font-extrabold mb-6 leading-tight">
                Premium Services for <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-primary-600">Your Vehicle</span>
              </h1>
              <p className="text-xl text-gray-300 max-w-2xl mx-auto mb-10 leading-relaxed">
                Experience the difference with our state-of-the-art equipment and certified technicians. We treat every car as if it were our own.
              </p>
              
              <div className="flex flex-wrap justify-center gap-4">
                <button 
                  onClick={handleBooking}
                  className="bg-primary-600 hover:bg-primary-500 text-white px-8 py-4 rounded-xl font-bold flex items-center gap-2 transition-all shadow-lg shadow-primary-900/50 hover:scale-105"
                >
                  <Calendar className="w-5 h-5" />
                  Book an Appointment
                </button>
                <a 
                  href="#services-grid"
                  className="bg-white/10 hover:bg-white/20 text-white border border-white/10 px-8 py-4 rounded-xl font-bold flex items-center gap-2 transition-all backdrop-blur-sm"
                >
                  Explore Services
                  <ArrowRight className="w-5 h-5" />
                </a>
              </div>
            </motion.div>
          </div>
        </section>

        {/* Services Grid */}
        <section id="services-grid" className="py-24 bg-gray-50">
          <div className="container mx-auto px-4">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {services.map((service, index) => {
                const Icon = service.icon
                return (
                  <motion.div
                    key={index}
                    initial={{ opacity: 0, y: 20 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.5, delay: index * 0.1 }}
                    className="group relative bg-white rounded-3xl p-8 shadow-xl hover:shadow-2xl transition-all duration-300 border border-gray-100 flex flex-col h-full"
                  >
                    {service.popular && (
                      <div className="absolute top-6 right-6 bg-gradient-to-r from-yellow-400 to-orange-500 text-white text-xs font-bold px-3 py-1 rounded-full shadow-lg flex items-center gap-1">
                        <Star className="w-3 h-3 fill-current" />
                        POPULAR
                      </div>
                    )}

                    <div className={`w-16 h-16 rounded-2xl bg-gradient-to-br ${service.color} flex items-center justify-center mb-6 shadow-md group-hover:scale-110 transition-transform duration-300`}>
                      <Icon className="w-8 h-8 text-white" />
                    </div>

                    <h3 className="text-2xl font-bold text-gray-900 mb-3 group-hover:text-primary-600 transition-colors">
                      {service.title}
                    </h3>
                    
                    <p className="text-gray-600 mb-6 flex-grow leading-relaxed">
                      {service.description}
                    </p>

                    <div className="space-y-3 mb-8 bg-gray-50 p-4 rounded-xl">
                      {service.features.map((feature, idx) => (
                        <div key={idx} className="flex items-start gap-2 text-sm text-gray-700">
                          <CheckCircle2 className="w-4 h-4 text-green-500 mt-0.5 flex-shrink-0" />
                          <span>{feature}</span>
                        </div>
                      ))}
                    </div>

                    <button
                      onClick={handleBooking} 
                      className="w-full py-3 rounded-xl border-2 border-primary-100 text-primary-700 font-bold hover:bg-primary-600 hover:text-white hover:border-primary-600 transition-all flex items-center justify-center gap-2 group/btn"
                    >
                      Book Now
                      <ArrowRight className="w-4 h-4 transform group-hover/btn:translate-x-1 transition-transform" />
                    </button>
                  </motion.div>
                )
              })}
            </div>
          </div>
        </section>

        {/* Why Choose Us Mini Section */}
        <section className="py-20 bg-white border-y border-gray-100">
           <div className="container mx-auto px-4">
             <div className="text-center max-w-3xl mx-auto mb-16">
               <h2 className="text-3xl font-bold text-gray-900 mb-4">Why Trust Lasantha Tyre Traders?</h2>
               <p className="text-gray-600">More than just a tyre shop. We are your partners in road safety.</p>
             </div>

             <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
               {[
                 { title: 'Certified Experts', desc: 'Technicians trained to handle modern vehicles.' },
                 { title: 'Genuine Products', desc: '100% authentic tyres with manufacturer warranty.' },
                 { title: 'Advanced Tech', desc: 'Laser alignment and touchless mounting systems.' },
                 { title: 'Customer Lounge', desc: 'Relax in AC comfort while we work on your car.' }
               ].map((item, i) => (
                 <div key={i} className="text-center p-6 rounded-2xl bg-gray-50 hover:bg-primary-50 transition-colors">
                   <div className="w-12 h-12 bg-white rounded-full shadow-sm mx-auto flex items-center justify-center mb-4 text-primary-600 font-bold text-xl">
                     {i + 1}
                   </div>
                   <h4 className="font-bold text-gray-900 mb-2">{item.title}</h4>
                   <p className="text-sm text-gray-500">{item.desc}</p>
                 </div>
               ))}
             </div>
           </div>
        </section>

        {/* Portal Section */}
        <div className="bg-gray-900">
          <FleetPortal />
        </div>
      </main>

      <Footer />
    </div>
  )
}
