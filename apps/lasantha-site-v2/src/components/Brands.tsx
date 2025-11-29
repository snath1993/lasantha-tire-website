'use client'

import { motion, useInView } from 'framer-motion'
import Image from 'next/image'
import { useRef } from 'react'

const brands = [
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
  { name: 'And Many More', logo: '', special: true },
]

export default function Brands() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
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
          {brands.map((brand, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, scale: 0.8 }}
              animate={isInView ? { opacity: 1, scale: 1 } : { opacity: 0, scale: 0.8 }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
              whileHover={{ scale: 1.05, transition: { duration: 0.2 } }}
              className={`rounded-xl p-4 sm:p-6 md:p-8 flex items-center justify-center hover:shadow-xl transition-all group cursor-pointer border h-32 sm:h-36 md:h-40 shadow-sm ${
                brand.special 
                  ? 'bg-primary-600 border-primary-600' 
                  : 'bg-white border-gray-200'
              }`}
            >
              <div className="relative w-full h-full flex items-center justify-center p-4">
                {brand.special ? (
                  <div className="text-center text-white">
                    <span className="block text-2xl font-bold leading-tight">And Many</span>
                    <span className="block text-xl font-medium opacity-90">More...</span>
                  </div>
                ) : (
                  <div className="relative w-full h-full">
                    <Image
                      src={brand.logo}
                      alt={`${brand.name} logo`}
                      fill
                      className="object-contain transition-all duration-300"
                      sizes="(max-width: 768px) 50vw, 25vw"
                    />
                  </div>
                )}
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  )
}
