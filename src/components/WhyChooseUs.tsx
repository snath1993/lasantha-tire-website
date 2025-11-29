'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef } from 'react'
import { Award, Shield, Users, Wrench, TrendingUp, Heart, CheckCircle2 } from 'lucide-react'

const features = [
  {
    icon: Shield,
    title: 'Genuine Products',
    description: 'We guarantee 100% authenticity on all our tyres, sourced directly from authorized global distributors.',
    color: 'from-blue-500 to-blue-600',
    stat: '100%',
    statLabel: 'Authentic'
  },
  {
    icon: Award,
    title: 'Industry Leaders',
    description: 'With over 22 years of excellence, we have established ourselves as the most trusted tyre center in Colombo.',
    color: 'from-purple-500 to-purple-600',
    stat: '22+',
    statLabel: 'Years'
  },
  {
    icon: Users,
    title: 'Expert Team',
    description: 'Our technicians undergo rigorous training and certification to handle the latest vehicle models with care.',
    color: 'from-green-500 to-green-600',
    stat: '50+',
    statLabel: 'Experts'
  },
  {
    icon: Wrench,
    title: 'Advanced Tech',
    description: 'We invest in the latest 3D alignment and balancing technology to ensure precision and safety.',
    color: 'from-orange-500 to-orange-600',
    stat: 'Latest',
    statLabel: 'Equipment'
  },
  {
    icon: TrendingUp,
    title: 'Best Value',
    description: 'Get premium products and services at competitive market rates with no hidden charges.',
    color: 'from-red-500 to-red-600',
    stat: 'Best',
    statLabel: 'Prices'
  },
  {
    icon: Heart,
    title: 'Customer First',
    description: 'Your safety is our priority. We provide honest advice and transparent service every step of the way.',
    color: 'from-pink-500 to-pink-600',
    stat: '5k+',
    statLabel: 'Happy Clients'
  }
]

export default function WhyChooseUs() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <section className="py-24 bg-dark-950 text-white relative overflow-hidden">
      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-20 pointer-events-none">
        <div className="absolute top-0 left-0 w-[800px] h-[800px] bg-primary-900/30 rounded-full blur-[120px] mix-blend-screen"></div>
        <div className="absolute bottom-0 right-0 w-[800px] h-[800px] bg-blue-900/30 rounded-full blur-[120px] mix-blend-screen"></div>
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-10"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <div className="text-center max-w-3xl mx-auto mb-20">
          <motion.div
            ref={ref}
            initial={{ opacity: 0, y: 30 }}
            animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
            transition={{ duration: 0.6 }}
          >
            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/5 border border-white/10 mb-6 backdrop-blur-sm">
              <CheckCircle2 className="w-4 h-4 text-primary-400" />
              <span className="text-sm font-medium text-gray-300">The Lasantha Difference</span>
            </div>
            <h2 className="text-4xl md:text-5xl font-bold mb-6 leading-tight">
              Why Sri Lanka Trusts <br />
              <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-primary-200">
                Lasantha Tyre Traders
              </span>
            </h2>
            <p className="text-xl text-gray-400 leading-relaxed">
              We don&apos;t just sell tyres; we deliver safety, performance, and peace of mind with every service.
            </p>
          </motion.div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 lg:gap-8">
          {features.map((feature, index) => {
            const Icon = feature.icon
            return (
              <motion.div
                key={index}
                initial={{ opacity: 0, y: 50 }}
                animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 50 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
                whileHover={{ y: -10 }}
                className="group relative"
              >
                <div className="absolute inset-0 bg-gradient-to-br from-white/10 to-white/5 rounded-3xl transform rotate-1 group-hover:rotate-2 transition-transform duration-300 opacity-0 group-hover:opacity-100"></div>
                <div className="relative bg-white/5 backdrop-blur-sm rounded-3xl p-8 border border-white/10 hover:border-primary-500/50 transition-all duration-300 h-full flex flex-col">
                  <div className="flex justify-between items-start mb-6">
                    <div className={`w-14 h-14 rounded-2xl bg-gradient-to-br ${feature.color} flex items-center justify-center shadow-lg group-hover:scale-110 transition-transform duration-300`}>
                      <Icon className="w-7 h-7 text-white" />
                    </div>
                    <div className="text-right">
                      <p className="text-2xl font-bold text-white">{feature.stat}</p>
                      <p className="text-xs text-gray-400 uppercase tracking-wider">{feature.statLabel}</p>
                    </div>
                  </div>
                  
                  <h3 className="text-xl font-bold mb-3 text-white group-hover:text-primary-400 transition-colors">{feature.title}</h3>
                  <p className="text-gray-400 leading-relaxed text-sm flex-grow">{feature.description}</p>
                </div>
              </motion.div>
            )
          })}
        </div>
      </div>
    </section>
  )
}
