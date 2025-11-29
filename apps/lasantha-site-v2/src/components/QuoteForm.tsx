'use client'

import { useState, useRef } from 'react'
import { motion, useInView } from 'framer-motion'
import { Phone, User, CheckCircle2, XCircle, Loader2 } from 'lucide-react'

export default function QuoteForm() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    tireSize: '',
    quantity: '',
    includeVehicle: false,
    vehicle: ''
  })

  const [loading, setLoading] = useState(false)
  const [notification, setNotification] = useState<{
    type: 'success' | 'error' | 'warning'
    message: string
  } | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setNotification(null)

    // Basic Phone Validation (Sri Lanka)
    const phoneRegex = /^(?:0|94|\+94)?(7[0-9]{8})$/
    const cleanPhone = formData.phone.replace(/\D/g, '')
    // Check if it matches 9 digits (without leading 0) or 10 digits (with leading 0) or 11 (with 94)
    // Simplified check: must contain at least 9 digits and look like a mobile number
    if (cleanPhone.length < 9 || !phoneRegex.test(cleanPhone.replace(/^0/, '94'))) {
       setNotification({
         type: 'error',
         message: 'Please enter a valid Sri Lankan mobile number (e.g., 0771234567)'
       })
       setLoading(false)
       return
    }

    try {
      const response = await fetch('/api/quote', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData)
      })

      const data = await response.json()

      if (response.ok) {
        setNotification({
          type: 'success',
          message: data.message || 'Price request sent successfully via WhatsApp!'
        })
        // Reset form
        setFormData({
          name: '',
          phone: '',
          tireSize: '',
          quantity: '',
          includeVehicle: false,
          vehicle: ''
        })
      } else {
        setNotification({
          type: data.isWarning ? 'warning' : 'error',
          message: data.message || 'Failed to send request. Please try again.'
        })
      }
    } catch {
      setNotification({
        type: 'error',
        message: 'Network error. Please check your connection and try again.'
      })
    } finally {
      setLoading(false)
    }
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target
    const checked = (e.target as HTMLInputElement).checked
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value
    })
  }

  return (
    <section id="quote" className="py-20 bg-gradient-to-br from-dark-950 via-dark-900 to-primary-900">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="max-w-4xl mx-auto"
        >
          <div className="text-center mb-12">
            <h2 className="text-4xl md:text-5xl font-bold text-white mb-4">
              Get Your <span className="text-primary-400">Free Quote</span>
            </h2>
            <p className="text-xl text-gray-300">
              Send us your tyre requirements via WhatsApp and get an instant price response
            </p>
          </div>

          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={isInView ? { opacity: 1, scale: 1 } : { opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="bg-white rounded-2xl shadow-2xl p-8 md:p-12"
          >
            {/* Notification */}
            {notification && (
              <motion.div
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                className={`mb-6 p-4 rounded-lg flex items-center gap-3 ${
                  notification.type === 'success'
                    ? 'bg-green-50 text-green-800 border border-green-200'
                    : notification.type === 'warning'
                    ? 'bg-yellow-50 text-yellow-800 border border-yellow-200'
                    : 'bg-red-50 text-red-800 border border-red-200'
                }`}
              >
                {notification.type === 'success' ? (
                  <CheckCircle2 className="w-5 h-5 flex-shrink-0" />
                ) : (
                  <XCircle className="w-5 h-5 flex-shrink-0" />
                )}
                <p className="text-sm font-medium">{notification.message}</p>
              </motion.div>
            )}

            <form onSubmit={handleSubmit} className="space-y-6" suppressHydrationWarning>
              <div className="grid md:grid-cols-2 gap-6">
                {/* Name Input */}
                <div>
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                    Your Name *
                  </label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                    <input
                      type="text"
                      id="name"
                      name="name"
                      required
                      disabled={loading}
                      value={formData.name}
                      onChange={handleChange}
                      className="w-full pl-11 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100 disabled:cursor-not-allowed"
                      placeholder="John Doe"
                      suppressHydrationWarning
                    />
                  </div>
                </div>

                {/* Phone Input */}
                <div>
                  <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-2">
                    WhatsApp Number *
                  </label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                    <input
                      type="tel"
                      id="phone"
                      name="phone"
                      required
                      disabled={loading}
                      value={formData.phone}
                      onChange={handleChange}
                      className="w-full pl-11 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100 disabled:cursor-not-allowed"
                      placeholder="94721222509"
                      suppressHydrationWarning
                    />
                  </div>
                  <p className="text-xs text-gray-500 mt-1">Enter number with country code (e.g., 94721222509)</p>
                </div>

                {/* Tyre Size Input */}
                <div>
                  <label htmlFor="tireSize" className="block text-sm font-medium text-gray-700 mb-2">
                    Tyre Size *
                  </label>
                  <input
                    type="text"
                    id="tireSize"
                    suppressHydrationWarning
                    name="tireSize"
                    required
                    disabled={loading}
                    value={formData.tireSize}
                    onChange={handleChange}
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100 disabled:cursor-not-allowed"
                    placeholder="e.g., 195/65R15"
                  />
                </div>

                {/* Quantity Select */}
                <div>
                  <label htmlFor="quantity" className="block text-sm font-medium text-gray-700 mb-2">
                    Quantity (Optional)
                  </label>
                  <select
                    id="quantity"
                    name="quantity"
                    disabled={loading}
                    value={formData.quantity}
                    onChange={handleChange}
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100 disabled:cursor-not-allowed"
                    suppressHydrationWarning
                  >
                    <option value="">Select quantity</option>
                    <option value="1">1 Tyre</option>
                    <option value="2">2 Tyres</option>
                    <option value="3">3 Tyres</option>
                    <option value="4">4 Tyres</option>
                    <option value="5">5 Tyres</option>
                    <option value="6">6 Tyres</option>
                  </select>
                </div>
              </div>

              {/* Vehicle Model - Optional Checkbox */}
              <div className="space-y-3">
                <label className="flex items-center gap-3 cursor-pointer">
                  <input
                    type="checkbox"
                    name="includeVehicle"
                    checked={formData.includeVehicle}
                    onChange={handleChange}
                    disabled={loading}
                    className="w-4 h-4 text-primary-600 border-gray-300 rounded focus:ring-primary-600 disabled:cursor-not-allowed"
                    suppressHydrationWarning
                  />
                  <span className="text-sm font-medium text-gray-700">
                    Include vehicle model (optional)
                  </span>
                </label>

                {formData.includeVehicle && (
                  <motion.div
                    initial={{ opacity: 0, height: 0 }}
                    animate={{ opacity: 1, height: 'auto' }}
                    exit={{ opacity: 0, height: 0 }}
                  >
                    <input
                      type="text"
                      id="vehicle"
                      name="vehicle"
                      disabled={loading}
                      value={formData.vehicle}
                      onChange={handleChange}
                      suppressHydrationWarning
                      className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100 disabled:cursor-not-allowed"
                      placeholder="e.g., Toyota Aqua 2020"
                    />
                  </motion.div>
                )}
              </div>

              {/* Submit Button */}
              <motion.button
                type="submit"
                disabled={loading}
                whileHover={!loading ? { scale: 1.02 } : {}}
                whileTap={!loading ? { scale: 0.98 } : {}}
                className="w-full bg-green-500 hover:bg-green-600 text-white py-4 rounded-lg font-semibold text-lg transition-colors flex items-center justify-center gap-3 shadow-lg hover:shadow-xl disabled:bg-gray-400 disabled:cursor-not-allowed"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Sending Request...
                  </>
                ) : (
                  <>
                    <Phone className="w-5 h-5" />
                    Get Price via WhatsApp
                  </>
                )}
              </motion.button>

              <p className="text-center text-sm text-gray-500">
                We&apos;ll send the price directly to your WhatsApp number.
                <br />
                <span className="text-xs text-gray-400">Your privacy is important to us. We do not share your number.</span>
              </p>
            </form>
          </motion.div>
        </motion.div>
      </div>
    </section>
  )
}
