'use client'

import { motion, useInView } from 'framer-motion'
import { useRef } from 'react'
import { Store, Wrench, Gauge, ShieldCheck } from 'lucide-react'

const highlights = [
  {
    icon: Store,
    title: 'Modern Showroom',
    description: 'A clean, well-organized tyre showroom with a wide selection of premium brands on display.',
  },
  {
    icon: Wrench,
    title: 'Professional Workshop',
    description: 'Fully equipped service bay with modern tyre mounting, balancing, and alignment equipment.',
  },
  {
    icon: Gauge,
    title: 'Italian CCD Alignment',
    description: 'TRIGON 725 PRO computerized wheel alignment system for precision accuracy.',
  },
  {
    icon: ShieldCheck,
    title: 'Quality Assurance',
    description: 'Every tyre installation includes balancing, valve replacement, and quality inspection.',
  },
]

export default function ShopGallery() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true, margin: '-100px' })

  return (
    <section ref={ref} className="py-20 bg-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <span className="inline-block px-4 py-1.5 bg-blue-50 text-blue-700 rounded-full text-sm font-semibold mb-4">
            Our Facility
          </span>
          <h2 className="text-3xl md:text-4xl font-bold text-slate-900 mb-4">
            Visit Our Shop
          </h2>
          <p className="text-lg text-slate-600 max-w-2xl mx-auto">
            Located at 1035 Pannipitiya Road, Battaramulla — serving customers with quality tyres and expert service since 1998.
          </p>
        </motion.div>

        {/* Highlights Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {highlights.map((item, index) => (
            <motion.div
              key={item.title}
              initial={{ opacity: 0, y: 30 }}
              animate={isInView ? { opacity: 1, y: 0 } : {}}
              transition={{ duration: 0.5, delay: index * 0.1 }}
              className="bg-slate-50 rounded-2xl p-6 text-center hover:shadow-lg transition-shadow border border-slate-100"
            >
              <div className="w-14 h-14 bg-blue-100 rounded-xl flex items-center justify-center mx-auto mb-4">
                <item.icon className="w-7 h-7 text-blue-600" />
              </div>
              <h3 className="text-lg font-bold text-slate-900 mb-2">{item.title}</h3>
              <p className="text-sm text-slate-600 leading-relaxed">{item.description}</p>
            </motion.div>
          ))}
        </div>

        {/* CTA */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6, delay: 0.5 }}
          className="text-center mt-12"
        >
          <div className="inline-flex items-center gap-2 bg-green-50 text-green-700 px-6 py-3 rounded-full font-semibold">
            <Store className="w-5 h-5" />
            Open 365 Days — 6:30 AM to 9:00 PM
          </div>
        </motion.div>
      </div>
    </section>
  )
}
