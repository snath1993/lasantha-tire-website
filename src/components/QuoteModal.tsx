'use client'

import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Phone, User, CheckCircle2, XCircle, Loader2, X } from 'lucide-react'

interface QuoteModalProps {
  isOpen: boolean
  onClose: () => void
  initialData?: {
    tireSize?: string
    vehicle?: string
  }
}

export default function QuoteModal({ isOpen, onClose, initialData }: QuoteModalProps) {
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

  // Reset form when modal opens or initialData changes
  useEffect(() => {
    if (isOpen) {
      setNotification(null)
      if (initialData) {
        setFormData(prev => ({
          ...prev,
          tireSize: initialData.tireSize || prev.tireSize,
          vehicle: initialData.vehicle || prev.vehicle,
          includeVehicle: !!initialData.vehicle
        }))
      }
    }
  }, [isOpen, initialData])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setNotification(null)

    // Basic Phone Validation (Sri Lanka)
    const phoneRegex = /^(?:0|94|\+94)?(7[0-9]{8})$/
    const cleanPhone = formData.phone.replace(/\D/g, '')
    
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
        // Close modal after success (optional delay)
        setTimeout(() => {
            onClose()
        }, 3000)
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
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4"
          >
            {/* Modal Content */}
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 20 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 20 }}
              onClick={(e) => e.stopPropagation()}
              className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto relative"
            >
              {/* Close Button */}
              <button
                onClick={onClose}
                className="absolute top-4 right-4 p-2 rounded-full hover:bg-gray-100 transition-colors z-10"
              >
                <X className="w-6 h-6 text-gray-500" />
              </button>

              <div className="p-6 md:p-8">
                <div className="text-center mb-8">
                  <h2 className="text-3xl font-bold text-gray-900 mb-2">
                    Get Your <span className="text-primary-600">Free Quote</span>
                  </h2>
                  <p className="text-gray-600">
                    නොමිලේ මිල ගණන් විමසන්න. Send us your requirements via WhatsApp.
                  </p>
                </div>

                {/* Notification */}
                {notification && (
                  <motion.div
                    initial={{ opacity: 0, y: -10 }}
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

                <form onSubmit={handleSubmit} className="space-y-5">
                  <div className="grid md:grid-cols-2 gap-5">
                    {/* Name Input */}
                    <div>
                      <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                        Your Name *
                      </label>
                      <div className="relative">
                        <User className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <input
                          type="text"
                          id="name"
                          name="name"
                          required
                          disabled={loading}
                          value={formData.name}
                          onChange={handleChange}
                          className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100"
                          placeholder="John Doe"
                        />
                      </div>
                    </div>

                    {/* Phone Input */}
                    <div>
                      <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                        WhatsApp Number *
                      </label>
                      <div className="relative">
                        <Phone className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <input
                          type="tel"
                          id="phone"
                          name="phone"
                          required
                          disabled={loading}
                          value={formData.phone}
                          onChange={handleChange}
                          className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100"
                          placeholder="0771234567"
                        />
                      </div>
                    </div>

                    {/* Tyre Size Input */}
                    <div>
                      <label htmlFor="tireSize" className="block text-sm font-medium text-gray-700 mb-1">
                        Tyre Size *
                      </label>
                      <input
                        type="text"
                        id="tireSize"
                        name="tireSize"
                        required
                        disabled={loading}
                        value={formData.tireSize}
                        onChange={handleChange}
                        className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100"
                        placeholder="e.g., 195/65R15"
                      />
                    </div>

                    {/* Quantity Select */}
                    <div>
                      <label htmlFor="quantity" className="block text-sm font-medium text-gray-700 mb-1">
                        Quantity
                      </label>
                      <select
                        id="quantity"
                        name="quantity"
                        disabled={loading}
                        value={formData.quantity}
                        onChange={handleChange}
                        className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100"
                      >
                        <option value="">Select quantity</option>
                        <option value="1">1 Tyre</option>
                        <option value="2">2 Tyres</option>
                        <option value="4">4 Tyres</option>
                        <option value="Other">Other</option>
                      </select>
                    </div>
                  </div>

                  {/* Vehicle Model - Optional Checkbox */}
                  <div className="space-y-2">
                    <label className="flex items-center gap-2 cursor-pointer">
                      <input
                        type="checkbox"
                        name="includeVehicle"
                        checked={formData.includeVehicle}
                        onChange={handleChange}
                        disabled={loading}
                        className="w-4 h-4 text-primary-600 border-gray-300 rounded focus:ring-primary-600"
                      />
                      <span className="text-sm text-gray-700">
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
                          className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent transition-all outline-none disabled:bg-gray-100"
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
                    className="w-full bg-green-600 hover:bg-green-700 text-white py-3.5 rounded-xl font-bold text-lg transition-all flex items-center justify-center gap-2 shadow-lg shadow-green-600/20 disabled:bg-gray-400 disabled:cursor-not-allowed mt-4"
                  >
                    {loading ? (
                      <>
                        <Loader2 className="w-5 h-5 animate-spin" />
                        Sending...
                      </>
                    ) : (
                      <>
                        <Phone className="w-5 h-5" />
                        Get Price via WhatsApp
                      </>
                    )}
                  </motion.button>
                  
                  <p className="text-center text-xs text-gray-500 mt-4">
                    By clicking above, you agree to receive a WhatsApp message with the price quotation.
                  </p>
                </form>
              </div>
            </motion.div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}
