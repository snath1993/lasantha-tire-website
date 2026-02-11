'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import Image from 'next/image'
import { X, MapPin, Phone, Clock, ChevronLeft, ChevronRight } from 'lucide-react'

const shopPhotos = [
  { src: '/shop/shop-1.jpg', caption: 'Lasantha Tyre Traders - Main Showroom' },
  { src: '/shop/w (10).jpg', caption: 'Wide Range of Tyres' },
  { src: '/shop/w (11).jpg', caption: 'Spacious Service Area' },
  { src: '/shop/w (8).jpg', caption: 'Advanced Alignment Technology' },
  { src: '/shop/w (2).jpg', caption: 'Ready to Serve You' },
  { src: '/shop/w (12).jpg', caption: 'Premium Brands Stock' },
]

export default function ShopGallery() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  const [selectedPhoto, setSelectedPhoto] = useState<number | null>(null)

  const openLightbox = (index: number) => setSelectedPhoto(index)
  const closeLightbox = () => setSelectedPhoto(null)
  const nextPhoto = () => setSelectedPhoto(prev => prev !== null ? (prev + 1) % shopPhotos.length : 0)
  const prevPhoto = () => setSelectedPhoto(prev => prev !== null ? (prev - 1 + shopPhotos.length) % shopPhotos.length : 0)

  return (
    <section id="gallery" className="py-20 bg-gray-50" ref={ref}>
      <div className="container mx-auto px-4">
        {/* Section Header */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
          className="text-center mb-14"
        >
          <span className="inline-block px-4 py-1.5 bg-primary-100 text-primary-700 rounded-full text-sm font-semibold mb-4 tracking-wide">
            Visit Our Shop
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-gray-900 mb-4">
            Our <span className="text-primary-600">Showroom</span>
          </h2>
          <p className="text-lg text-gray-600 max-w-2xl mx-auto">
            Located at 1035 Pannipitiya Road, Battaramulla — serving Sri Lankan drivers since 2002
          </p>
        </motion.div>

        {/* Photo Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {/* Large featured photo */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={isInView ? { opacity: 1, scale: 1 } : {}}
            transition={{ duration: 0.5, delay: 0.1 }}
            className="md:col-span-2 lg:col-span-2 row-span-2 relative group cursor-pointer overflow-hidden rounded-2xl"
            onClick={() => openLightbox(0)}
          >
            <div className="relative w-full h-[300px] md:h-[500px]">
              <Image
                src={shopPhotos[0].src}
                alt={shopPhotos[0].caption}
                fill
                className="object-cover transition-transform duration-500 group-hover:scale-105"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
              <div className="absolute bottom-0 left-0 right-0 p-6 text-white translate-y-full group-hover:translate-y-0 transition-transform duration-300">
                <p className="text-lg font-bold">{shopPhotos[0].caption}</p>
                <div className="flex items-center gap-2 mt-2 text-sm text-white/80">
                  <MapPin className="w-4 h-4" />
                  <span>1035 Pannipitiya Rd, Battaramulla</span>
                </div>
              </div>
            </div>
          </motion.div>

          {/* Smaller photos */}
          {shopPhotos.slice(1).map((photo, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, scale: 0.95 }}
              animate={isInView ? { opacity: 1, scale: 1 } : {}}
              transition={{ duration: 0.5, delay: 0.2 + index * 0.1 }}
              className="relative group cursor-pointer overflow-hidden rounded-2xl"
              onClick={() => openLightbox(index + 1)}
            >
              <div className="relative w-full h-[200px] md:h-[240px]">
                <Image
                  src={photo.src}
                  alt={photo.caption}
                  fill
                  className="object-cover transition-transform duration-500 group-hover:scale-105"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                <div className="absolute bottom-0 left-0 right-0 p-4 text-white translate-y-full group-hover:translate-y-0 transition-transform duration-300">
                  <p className="text-sm font-semibold">{photo.caption}</p>
                </div>
              </div>
            </motion.div>
          ))}
        </div>

        {/* Quick Info Cards */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.5, delay: 0.5 }}
          className="grid grid-cols-1 sm:grid-cols-3 gap-4 mt-8"
        >
          <a href="https://maps.google.com/?q=Lasantha+Tyre+Traders+Battaramulla" target="_blank" rel="noopener noreferrer" className="flex items-center gap-4 p-5 bg-white rounded-xl border border-gray-200 hover:border-primary-300 hover:shadow-lg transition-all group">
            <div className="w-12 h-12 bg-primary-100 rounded-xl flex items-center justify-center group-hover:bg-primary-600 transition-colors">
              <MapPin className="w-6 h-6 text-primary-600 group-hover:text-white transition-colors" />
            </div>
            <div>
              <p className="text-sm text-gray-500">Location</p>
              <p className="font-bold text-gray-900">1035 Pannipitiya Rd, Battaramulla</p>
            </div>
          </a>
          <a href="tel:+94773131883" className="flex items-center gap-4 p-5 bg-white rounded-xl border border-gray-200 hover:border-primary-300 hover:shadow-lg transition-all group">
            <div className="w-12 h-12 bg-red-100 rounded-xl flex items-center justify-center group-hover:bg-red-600 transition-colors">
              <Phone className="w-6 h-6 text-red-600 group-hover:text-white transition-colors" />
            </div>
            <div>
              <p className="text-sm text-gray-500">Call Us</p>
              <p className="font-bold text-gray-900">077 313 1883</p>
            </div>
          </a>
          <div className="flex items-center gap-4 p-5 bg-white rounded-xl border border-gray-200">
            <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center">
              <Clock className="w-6 h-6 text-green-600" />
            </div>
            <div>
              <p className="text-sm text-gray-500">Open Daily</p>
              <p className="font-bold text-gray-900">6:30 AM - 9:00 PM</p>
            </div>
          </div>
        </motion.div>
      </div>

      {/* Lightbox */}
      {selectedPhoto !== null && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 bg-black/90 backdrop-blur-sm z-50 flex items-center justify-center p-4"
          onClick={closeLightbox}
        >
          <button
            onClick={closeLightbox}
            className="absolute top-4 right-4 w-12 h-12 bg-white/10 hover:bg-white/20 rounded-full flex items-center justify-center text-white transition-colors z-50"
          >
            <X className="w-6 h-6" />
          </button>

          <button
            onClick={(e) => { e.stopPropagation(); prevPhoto() }}
            className="absolute left-4 top-1/2 -translate-y-1/2 w-12 h-12 bg-white/10 hover:bg-white/20 rounded-full flex items-center justify-center text-white transition-colors z-50"
          >
            <ChevronLeft className="w-6 h-6" />
          </button>

          <button
            onClick={(e) => { e.stopPropagation(); nextPhoto() }}
            className="absolute right-4 top-1/2 -translate-y-1/2 w-12 h-12 bg-white/10 hover:bg-white/20 rounded-full flex items-center justify-center text-white transition-colors z-50"
          >
            <ChevronRight className="w-6 h-6" />
          </button>

          <div className="relative max-w-5xl max-h-[85vh] w-full" onClick={(e) => e.stopPropagation()}>
            <Image
              src={shopPhotos[selectedPhoto].src}
              alt={shopPhotos[selectedPhoto].caption}
              width={1600}
              height={1200}
              className="w-full h-auto max-h-[80vh] object-contain rounded-xl"
            />
            <p className="text-white text-center mt-4 text-lg font-medium">
              {shopPhotos[selectedPhoto].caption}
            </p>
            <p className="text-white/60 text-center text-sm mt-1">
              {selectedPhoto + 1} / {shopPhotos.length}
            </p>
          </div>
        </motion.div>
      )}
    </section>
  )
}
