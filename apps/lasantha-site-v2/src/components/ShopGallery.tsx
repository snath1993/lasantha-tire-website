'use client'

import { motion, useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import { Store, Wrench, Gauge, ShieldCheck, ChevronLeft, ChevronRight, X } from 'lucide-react'
import Image from 'next/image'

const shopPhotos = [
  { src: '/shop/shop-1.jpg', alt: 'Lasantha Tyre Traders Showroom' },
  { src: '/shop/w (1).jpg', alt: 'Workshop - Tyre Installation' },
  { src: '/shop/w (2).jpg', alt: 'Workshop - Wheel Alignment' },
  { src: '/shop/w (3).jpg', alt: 'Workshop - Tyre Balancing' },
  { src: '/shop/w (4).jpg', alt: 'Workshop - Service Bay' },
  { src: '/shop/w (5).jpg', alt: 'Workshop - Equipment' },
  { src: '/shop/w (6).jpg', alt: 'Workshop - Professional Service' },
  { src: '/shop/w (7).jpg', alt: 'Workshop - Tyre Storage' },
  { src: '/shop/w (8).jpg', alt: 'Workshop - Customer Area' },
  { src: '/shop/w (9).jpg', alt: 'Workshop - Tools & Equipment' },
  { src: '/shop/w (10).jpg', alt: 'Workshop - Alignment Machine' },
  { src: '/shop/w (11).jpg', alt: 'Workshop - Tyre Display' },
  { src: '/shop/w (12).jpg', alt: 'Workshop - Service in Progress' },
]

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
  const [selectedPhoto, setSelectedPhoto] = useState<number | null>(null)

  const nextPhoto = () => {
    if (selectedPhoto !== null) {
      setSelectedPhoto((selectedPhoto + 1) % shopPhotos.length)
    }
  }
  
  const prevPhoto = () => {
    if (selectedPhoto !== null) {
      setSelectedPhoto((selectedPhoto - 1 + shopPhotos.length) % shopPhotos.length)
    }
  }

  return (
    <section ref={ref} className="py-20 bg-white">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
          className="text-center mb-12"
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

        {/* Photo Gallery */}
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3 mb-16">
          {shopPhotos.map((photo, index) => (
            <motion.div
              key={photo.src}
              initial={{ opacity: 0, scale: 0.9 }}
              animate={isInView ? { opacity: 1, scale: 1 } : {}}
              transition={{ duration: 0.4, delay: index * 0.05 }}
              className="relative aspect-square rounded-xl overflow-hidden cursor-pointer group"
              onClick={() => setSelectedPhoto(index)}
            >
              <Image
                src={photo.src}
                alt={photo.alt}
                fill
                className="object-cover group-hover:scale-110 transition-transform duration-300"
                sizes="(max-width: 640px) 50vw, (max-width: 768px) 33vw, 25vw"
              />
              <div className="absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors" />
            </motion.div>
          ))}
        </div>

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

      {/* Lightbox Modal */}
      {selectedPhoto !== null && (
        <div className="fixed inset-0 z-50 bg-black/90 flex items-center justify-center p-4" onClick={() => setSelectedPhoto(null)}>
          <button
            onClick={(e) => { e.stopPropagation(); setSelectedPhoto(null) }}
            className="absolute top-4 right-4 text-white hover:text-gray-300 z-50"
          >
            <X className="w-8 h-8" />
          </button>
          <button
            onClick={(e) => { e.stopPropagation(); prevPhoto() }}
            className="absolute left-4 text-white hover:text-gray-300 z-50"
          >
            <ChevronLeft className="w-10 h-10" />
          </button>
          <button
            onClick={(e) => { e.stopPropagation(); nextPhoto() }}
            className="absolute right-4 text-white hover:text-gray-300 z-50"
          >
            <ChevronRight className="w-10 h-10" />
          </button>
          <div className="relative w-full max-w-4xl aspect-video" onClick={(e) => e.stopPropagation()}>
            <Image
              src={shopPhotos[selectedPhoto].src}
              alt={shopPhotos[selectedPhoto].alt}
              fill
              className="object-contain"
              sizes="90vw"
              priority
            />
          </div>
          <p className="absolute bottom-6 text-white text-sm font-medium">
            {selectedPhoto + 1} / {shopPhotos.length} — {shopPhotos[selectedPhoto].alt}
          </p>
        </div>
      )}
    </section>
  )
}
