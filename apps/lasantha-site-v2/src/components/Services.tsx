'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef } from 'react'
import { Wrench, Shield, Gauge, Package, Award, Clock, ArrowRight, CheckCircle2 } from 'lucide-react'
import Link from 'next/link'

const services = [
  {
    icon: Package,
    title: 'Premium Tyre Sales',
    description: 'Authorized dealer for world-renowned brands. Find the perfect match for your vehicle from our extensive collection.',
    features: ['All Major Brands', 'Manufacturer Warranty', 'Competitive Pricing'],
    color: 'from-blue-500 to-blue-600',
    bg: 'bg-blue-50'
  },
  {
    icon: Wrench,
    title: 'Expert Installation',
    description: 'State-of-the-art mounting and balancing equipment operated by certified technicians for a smooth ride.',
    features: ['Touchless Mounting', 'Laser Balancing', 'Rim Protection'],
    color: 'from-green-500 to-green-600',
    bg: 'bg-green-50'
  },
  {
    icon: Gauge,
    title: 'CCD Wheel Alignment',
    description: 'TRIGON 725 PRO with 8 CCD cameras and wireless radio transmission. Precision alignment using advanced computerized technology to ensure vehicle safety and extend tyre life.',
    features: ['Camber/Caster Adjust', 'Printable Reports', 'Hours: 07:30 AM - 06:00 PM'],
    color: 'from-purple-500 to-purple-600',
    bg: 'bg-purple-50'
  },
  {
    icon: Shield,
    title: 'Warranty Claims',
    description: 'Hassle-free warranty processing for all tyres purchased from us. We handle the paperwork for you.',
    features: ['Direct Manufacturer Link', 'Quick Processing', 'Replacement Guarantee'],
    color: 'from-red-500 to-red-600',
    bg: 'bg-red-50'
  },
  {
    icon: Award,
    title: 'Quality Assurance',
    description: 'Every product and service is backed by our commitment to quality and customer satisfaction.',
    features: ['Genuine Products', 'Quality Checks', 'After-sales Support'],
    color: 'from-orange-500 to-orange-600',
    bg: 'bg-orange-50'
  },
  {
    icon: Clock,
    title: 'Express Service',
    description: 'Value your time? Our express bays ensure you get back on the road in record time.',
    features: ['Dedicated Bays', 'Pit-crew Style', 'Lounge Access'],
    color: 'from-teal-500 to-teal-600',
    bg: 'bg-teal-50'
  }
]

function ServiceCard({ service, index }: { service: typeof services[0], index: number }) {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  const Icon = service.icon

  return (
    <motion.div
      ref={ref}
      initial={{ opacity: 0, y: 50 }}
      animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 50 }}
      transition={{ duration: 0.5, delay: index * 0.1 }}
      whileHover={{ y: -10 }}
      className="group relative h-full"
    >
      <div className="absolute inset-0 bg-gradient-to-br from-gray-100 to-white rounded-3xl transform rotate-1 group-hover:rotate-2 transition-transform duration-300"></div>
      <div className="relative bg-white rounded-3xl p-8 shadow-xl hover:shadow-2xl transition-all duration-300 border border-gray-100 h-full flex flex-col">
        <div className={`w-16 h-16 rounded-2xl bg-gradient-to-br ${service.color} flex items-center justify-center mb-6 shadow-lg transform group-hover:scale-110 transition-transform duration-300`}>
          <Icon className="w-8 h-8 text-white" />
        </div>
        
        <h3 className="text-2xl font-bold text-gray-900 mb-3 group-hover:text-primary-600 transition-colors">
          {service.title}
        </h3>
        
        <p className="text-gray-600 leading-relaxed mb-6 flex-grow">
          {service.description}
        </p>

        <div className="space-y-3 mb-6">
          {service.features.map((feature, idx) => (
            <div key={idx} className="flex items-center gap-2 text-sm text-gray-500">
              <CheckCircle2 className="w-4 h-4 text-primary-500" />
              <span>{feature}</span>
            </div>
          ))}
        </div>
        
        <div className="pt-6 border-t border-gray-100">
          <span className="inline-flex items-center text-primary-600 font-semibold group-hover:gap-2 transition-all cursor-pointer">
            Learn More <ArrowRight className="w-4 h-4 ml-1" />
          </span>
        </div>
      </div>
    </motion.div>
  )
}

export default function Services() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <section id="services" className="py-24 bg-gray-50 relative overflow-hidden">
      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-[0.03] pointer-events-none">
        <div className="absolute top-0 left-0 w-full h-full bg-[url('/grid.svg')]"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <div className="text-center max-w-3xl mx-auto mb-20">
          <motion.div
            ref={ref}
            initial={{ opacity: 0, y: 30 }}
            animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
            transition={{ duration: 0.6 }}
          >
            <span className="text-primary-600 font-bold tracking-wider uppercase text-sm mb-4 block">
              World Class Standards
            </span>
            <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
              Comprehensive <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-800">Tyre Solutions</span>
            </h2>
            <p className="text-xl text-gray-600 leading-relaxed">
              We combine advanced technology with expert craftsmanship to deliver the best care for your vehicle.
            </p>
          </motion.div>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8 lg:gap-10">
          {services.map((service, index) => (
            <ServiceCard key={index} service={service} index={index} />
          ))}
        </div>

        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.6 }}
          className="mt-20 text-center"
        >
          <div className="inline-flex flex-col sm:flex-row items-center gap-6 bg-white p-2 pr-8 rounded-full shadow-xl border border-gray-100">
            <div className="bg-primary-600 text-white px-6 py-3 rounded-full font-bold">
              Ready to start?
            </div>
            <p className="text-gray-600 font-medium">
              Get a custom quote for your vehicle today.
            </p>
            <Link
              href="/contact"
              className="flex items-center gap-2 text-primary-600 font-bold hover:text-primary-700 transition-colors"
            >
              Contact Us <ArrowRight className="w-5 h-5" />
            </Link>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
