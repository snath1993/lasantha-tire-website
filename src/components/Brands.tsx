'use client'

import { motion, useInView, AnimatePresence } from 'framer-motion'
import Image from 'next/image'
import { useRef, useState } from 'react'
import { X } from 'lucide-react'

const featuredBrands = [
  { name: 'Michelin', logo: '/brands/michelin.png' },
  { name: 'Bridgestone', logo: '/brands/bridgestone.png' },
  { name: 'Goodyear', logo: '/brands/goodyear.png' },
  { name: 'Continental', logo: '/brands/continental.svg' },
  { name: 'Dunlop', logo: '/brands/dunlop.jpg' },
  { name: 'Yokohama', logo: '/brands/yokohama.jpg' },
  { name: 'Kumho', logo: '/brands/kumho.png' },
  { name: 'Maxxis', logo: '/brands/maxxis.jpg' },
  { name: 'Giti', logo: '/brands/giti.png' },
  { name: 'Marshal', logo: '/brands/marshal.png' },
  { name: 'Duraturn', logo: '/brands/duraturn.png' },
]

const allBrands = [
  { name: 'Bridgestone', logo: '/brands/bridgestone.png' },
  { name: 'Michelin', logo: '/brands/michelin.png' },
  { name: 'Goodyear', logo: '/brands/goodyear.png' },
  { name: 'Continental', logo: '/brands/continental.png' },
  { name: 'Dunlop', logo: '/brands/dunlop.png' },
  { name: 'Pirelli', logo: '/brands/pirelli.png' },
  { name: 'Yokohama', logo: '/brands/yokohama.png' },
  { name: 'Hankook', logo: '/brands/hankook.png' },
  { name: 'Maxxis', logo: '/brands/maxxis.png' },
  { name: 'Toyo', logo: '/brands/toyo.png' },
  { name: 'Kumho', logo: '/brands/kumho.png' },
  { name: 'BFGoodrich', logo: '/brands/bfgoodrich.png' },
  { name: 'GT Radial', logo: '/brands/gtradial.png' },
  { name: 'CEAT', logo: '/brands/ceat.png' },
  { name: 'Westlake', logo: '/brands/westlake.png' },
  { name: 'Sailun', logo: '/brands/sailun.png' },
  { name: 'Linglong', logo: '/brands/linglong.png' },
  { name: 'Nexen', logo: '/brands/nexen.png' },
  { name: 'Federal', logo: '/brands/federal.png' },
  { name: 'Nankang', logo: '/brands/nankang.png' },
  { name: 'Firestone', logo: '/brands/firestone.png' },
  { name: 'Giti', logo: '/brands/giti.png' },
  { name: 'Marshal', logo: '/brands/marshal.png' },
  { name: 'Duraturn', logo: '/brands/duraturn.png' },
  { name: 'Falken', logo: null },
  { name: 'Triangle', logo: null },
  { name: 'DSI', logo: null },
  { name: 'Radar', logo: null },
  { name: 'Achilles', logo: null },
]

export default function Brands() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  const [showAllBrands, setShowAllBrands] = useState(false)

  return (
    <>
      <section id="brands" className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <motion.div
            ref={ref}
            initial={{ opacity: 0, y: 30 }}
            animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
            transition={{ duration: 0.6 }}
            className="text-center mb-16"
          >
            <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
              Premium <span className="text-primary-600">Brands</span>
            </h2>
            <p className="text-xl text-gray-600 max-w-2xl mx-auto">
              We stock tyres from the world&apos;s leading manufacturers
            </p>
          </motion.div>

          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4 sm:gap-6 md:gap-8">
            {featuredBrands.map((brand, index) => (
              <motion.div
                key={index}
                initial={{ opacity: 0, scale: 0.8 }}
                animate={isInView ? { opacity: 1, scale: 1 } : { opacity: 0, scale: 0.8 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
                whileHover={{ scale: 1.05, transition: { duration: 0.2 } }}
                className="rounded-xl p-4 sm:p-6 md:p-8 flex items-center justify-center hover:shadow-xl transition-all group cursor-pointer border h-32 sm:h-36 md:h-40 shadow-sm bg-white border-gray-200"
              >
                <div className="relative w-full h-full flex items-center justify-center p-4">
                  <div className="relative w-full h-full">
                    <Image
                      src={brand.logo}
                      alt={`${brand.name} logo`}
                      fill
                      className="object-contain transition-all duration-300"
                      sizes="(max-width: 768px) 50vw, 25vw"
                    />
                  </div>
                </div>
              </motion.div>
            ))}
            
            {/* View All Brands Button */}
            <motion.button
              initial={{ opacity: 0, scale: 0.8 }}
              animate={isInView ? { opacity: 1, scale: 1 } : { opacity: 0, scale: 0.8 }}
              transition={{ duration: 0.5, delay: featuredBrands.length * 0.1 }}
              whileHover={{ scale: 1.05, transition: { duration: 0.2 } }}
              onClick={() => setShowAllBrands(true)}
              className="rounded-xl p-4 sm:p-6 md:p-8 flex items-center justify-center hover:shadow-xl transition-all group cursor-pointer border h-32 sm:h-36 md:h-40 shadow-sm bg-primary-600 border-primary-600"
            >
              <div className="text-center text-white">
                <span className="block text-2xl font-bold leading-tight">View All</span>
                <span className="block text-xl font-medium opacity-90">{allBrands.length}+ Brands</span>
              </div>
            </motion.button>
          </div>
        </div>
      </section>

      {/* All Brands Modal */}
      <AnimatePresence>
        {showAllBrands && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
            onClick={() => setShowAllBrands(false)}
          >
            <motion.div
              initial={{ scale: 0.9, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              exit={{ scale: 0.9, opacity: 0 }}
              transition={{ type: 'spring', damping: 25, stiffness: 300 }}
              className="bg-white rounded-2xl shadow-2xl max-w-5xl w-full max-h-[85vh] overflow-hidden"
              onClick={(e) => e.stopPropagation()}
            >
              {/* Header */}
              <div className="px-6 py-5 border-b border-gray-200 flex items-center justify-between bg-gradient-to-r from-primary-600 to-primary-700">
                <div>
                  <h3 className="text-2xl font-bold text-white">All Tyre Brands</h3>
                  <p className="text-sm text-white/80 mt-1">{allBrands.length} trusted manufacturers</p>
                </div>
                <button
                  onClick={() => setShowAllBrands(false)}
                  className="w-10 h-10 rounded-full bg-white/10 hover:bg-white/20 flex items-center justify-center transition-colors text-white"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Brands Grid */}
              <div className="p-6 overflow-y-auto max-h-[calc(85vh-100px)]">
                <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
                  {allBrands.map((brand, index) => (
                    <motion.div
                      key={index}
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ delay: index * 0.02 }}
                      className="group"
                    >
                      <div className="rounded-xl p-4 bg-gray-50 hover:bg-white border border-gray-200 hover:border-primary-300 hover:shadow-lg transition-all h-28 flex items-center justify-center relative overflow-hidden">
                        {/* Brand Logo or Initial */}
                        {brand.logo ? (
                          <div className="relative w-full h-full p-2">
                            <Image
                              src={brand.logo}
                              alt={`${brand.name} logo`}
                              fill
                              className="object-contain"
                              sizes="(max-width: 640px) 50vw, (max-width: 768px) 33vw, 20vw"
                            />
                          </div>
                        ) : (
                          <div className="w-12 h-12 rounded-full bg-gradient-to-br from-primary-500 to-primary-600 flex items-center justify-center text-white text-xl font-bold shadow-lg">
                            {brand.name[0]}
                          </div>
                        )}
                        
                        {/* Brand Name Tooltip on Hover */}
                        <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/80 to-transparent opacity-0 group-hover:opacity-100 transition-opacity py-2 px-2">
                          <p className="text-white text-xs font-semibold text-center truncate">{brand.name}</p>
                        </div>
                      </div>
                    </motion.div>
                  ))}
                </div>

                {/* Footer Note */}
                <div className="mt-8 p-4 bg-primary-50 rounded-xl border border-primary-100">
                  <p className="text-sm text-gray-600 text-center">
                    <span className="font-semibold text-primary-600">Can&apos;t find your brand?</span> Call us at{' '}
                    <a href="tel:+94773131883" className="text-primary-600 hover:text-primary-700 font-bold">
                      077 313 1883
                    </a>{' '}
                    — we stock many more brands!
                  </p>
                </div>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  )
}
