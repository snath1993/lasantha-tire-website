'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef } from 'react'
import { MapPin, Phone, Clock, Navigation } from 'lucide-react'

export default function LocationMap() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  // Real Lasantha Tyre Traders location (pulled from Google Maps)
  const shopLocation = {
    lat: 6.883467,
    lng: 79.9330325,
    name: 'Lasantha Tyre Traders',
    addressLine1: '1035 Pannipitiya Road',
    addressLine2: 'Battaramulla 10230, Sri Lanka',
    googleMapsUrl: 'https://maps.app.goo.gl/tv2DuXtkkDFu5nv76'
  }

  return (
    <section id="contact" className="py-20 bg-gray-50">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-12"
        >
          <div className="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-full mb-4">
            <MapPin className="w-8 h-8 text-primary-600" />
          </div>
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Visit Our <span className="text-primary-600">Shop</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Find us easily with our location map and get directions
          </p>
        </motion.div>

        <div className="grid lg:grid-cols-2 gap-8">
          {/* Map Section */}
          <motion.div
            initial={{ opacity: 0, x: -30 }}
            animate={isInView ? { opacity: 1, x: 0 } : { opacity: 0, x: -30 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="bg-white rounded-2xl shadow-xl overflow-hidden"
          >
            <div className="relative w-full h-96">
              <iframe
                title={`${shopLocation.name} location map`}
                src={`https://maps.google.com/maps?q=${shopLocation.lat},${shopLocation.lng}&z=17&hl=en&output=embed`}
                width="100%"
                height="100%"
                style={{ border: 0 }}
                allowFullScreen
                loading="lazy"
                referrerPolicy="no-referrer-when-downgrade"
                className="absolute inset-0"
                suppressHydrationWarning
              />
            </div>
            <div className="p-6 bg-gradient-to-r from-primary-600 to-primary-700 text-white">
              <a
                href={shopLocation.googleMapsUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-center gap-2 font-semibold hover:underline"
              >
                <Navigation className="w-5 h-5" />
                Open in Google Maps
              </a>
            </div>
          </motion.div>

          {/* Contact Information */}
          <motion.div
            initial={{ opacity: 0, x: 30 }}
            animate={isInView ? { opacity: 1, x: 0 } : { opacity: 0, x: 30 }}
            transition={{ duration: 0.6, delay: 0.4 }}
            className="space-y-6"
          >
            {/* Address Card */}
            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all">
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <MapPin className="w-6 h-6 text-primary-600" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-gray-900 mb-2">Our Location</h3>
                  <p className="text-gray-600 leading-relaxed">
                    {shopLocation.addressLine1}
                    <br />
                    {shopLocation.addressLine2}
                  </p>
                </div>
              </div>
            </div>

            {/* Phone Card */}
            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all">
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Phone className="w-6 h-6 text-green-600" />
                </div>
                <div>
                  <h3 className="text-xl font-bold text-gray-900 mb-2">Phone Numbers</h3>
                  <div className="space-y-2">
                    <a href="tel:+94773131883" className="block text-red-600 font-bold hover:text-red-700 transition-colors">
                      🔥 Hotline: 077 313 1883
                    </a>
                    <a href="tel:+94721222509" className="block text-gray-600 hover:text-primary-600 transition-colors">
                      📱 072 122 2509
                    </a>
                    <a href="tel:+94112773231" className="block text-gray-600 hover:text-primary-600 transition-colors">
                      ☎️ 011 277 3231
                    </a>
                  </div>
                </div>
              </div>
            </div>

            {/* Hours Card */}
            <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-all">
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Clock className="w-6 h-6 text-blue-600" />
                </div>
                <div className="flex-1">
                  <h3 className="text-xl font-bold text-gray-900 mb-3">Opening Hours</h3>
                  <div className="space-y-2 text-gray-600">
                    {[
                      { label: 'General Hours', hours: '06:30 AM - 9:00 PM' },
                      { label: 'Wheel Alignment', hours: '07:30 AM - 6:00 PM' }
                    ].map(({ label, hours }) => (
                      <div key={label} className="flex justify-between">
                        <span className="font-medium">{label}:</span>
                        <span>{hours}</span>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>

            {/* Directions Button */}
            <motion.a
              href={shopLocation.googleMapsUrl}
              target="_blank"
              rel="noopener noreferrer"
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
              className="block w-full bg-primary-600 hover:bg-primary-700 text-white py-4 rounded-xl font-semibold text-center transition-colors shadow-lg hover:shadow-xl"
            >
              Get Directions
            </motion.a>
          </motion.div>
        </div>
      </div>
    </section>
  )
}
