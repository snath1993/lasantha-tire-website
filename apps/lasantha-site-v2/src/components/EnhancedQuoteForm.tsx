'use client'

import { useRef, useState } from 'react'
import { motion, useInView } from 'framer-motion'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Phone, User, CheckCircle2, XCircle, Loader2, Package } from 'lucide-react'

// Enhanced validation schema
const quoteFormSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters').max(100),
  phone: z
    .string()
    .regex(/^(?:\+94|0)?7[0-9]{8}$/, 'Invalid Sri Lankan phone number (e.g., 0771234567)'),
  tireSize: z
    .string()
    .regex(
      /^\d{3}\/\d{2}[A-Z]?\d{2}$/,
      'Invalid tire size format (e.g., 195/65R15)'
    ),
  quantity: z.string().optional(),
  includeVehicle: z.boolean().default(false),
  vehicle: z.string().max(100).optional(),
})

interface Notification {
  type: 'success' | 'error' | 'warning'
  message: string
}

export default function EnhancedQuoteForm() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  const [notification, setNotification] = useState<Notification | null>(null)

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(quoteFormSchema),
    defaultValues: {
      name: '',
      phone: '',
      tireSize: '',
      quantity: '',
      includeVehicle: false,
      vehicle: '',
    },
  })

  const includeVehicle = watch('includeVehicle')

  const onSubmit = async (data: unknown) => {
    setNotification(null)

    try {
      const response = await fetch('/api/quote', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })

      const result = await response.json()

      if (response.ok) {
        setNotification({
          type: 'success',
          message: result.message || 'Price request sent successfully via WhatsApp!',
        })
        reset()
      } else {
        setNotification({
          type: result.isWarning ? 'warning' : 'error',
          message: result.message || 'Failed to send request. Please try again.',
        })
      }
    } catch {
      setNotification({
        type: 'error',
        message: 'Network error. Please check your connection and try again.',
      })
    }
  }

  return (
    <section ref={ref} className="py-16 bg-gradient-to-br from-gray-50 to-white">
      <div className="container mx-auto px-4">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.6 }}
          className="max-w-2xl mx-auto"
        >
          <div className="text-center mb-8">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4">
              Get Instant Tire Price Quote
            </h2>
            <p className="text-gray-600 text-lg">
              Enter your details and receive the best prices directly on WhatsApp
            </p>
          </div>

          <motion.form
            initial={{ opacity: 0, scale: 0.95 }}
            animate={isInView ? { opacity: 1, scale: 1 } : {}}
            transition={{ delay: 0.2, duration: 0.5 }}
            onSubmit={handleSubmit(onSubmit)}
            className="bg-white rounded-2xl shadow-xl p-6 md:p-8 border border-gray-100"
          >
            {/* Name Field */}
            <div className="mb-6">
              <label className="block text-gray-700 font-semibold mb-2" htmlFor="name">
                <User className="inline w-4 h-4 mr-2" />
                Full Name *
              </label>
              <input
                {...register('name')}
                id="name"
                type="text"
                placeholder="Enter your name"
                className={`w-full px-4 py-3 border-2 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors ${
                  errors.name ? 'border-red-500' : 'border-gray-300'
                }`}
                disabled={isSubmitting}
              />
              {errors.name && (
                <p className="mt-1 text-sm text-red-600">{errors.name.message}</p>
              )}
            </div>

            {/* Phone Field */}
            <div className="mb-6">
              <label className="block text-gray-700 font-semibold mb-2" htmlFor="phone">
                <Phone className="inline w-4 h-4 mr-2" />
                WhatsApp Number *
              </label>
              <input
                {...register('phone')}
                id="phone"
                type="tel"
                placeholder="0771234567"
                className={`w-full px-4 py-3 border-2 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors ${
                  errors.phone ? 'border-red-500' : 'border-gray-300'
                }`}
                disabled={isSubmitting}
              />
              {errors.phone && (
                <p className="mt-1 text-sm text-red-600">{errors.phone.message}</p>
              )}
            </div>

            {/* Tire Size Field */}
            <div className="mb-6">
              <label className="block text-gray-700 font-semibold mb-2" htmlFor="tireSize">
                <Package className="inline w-4 h-4 mr-2" />
                Tire Size *
              </label>
              <input
                {...register('tireSize')}
                id="tireSize"
                type="text"
                placeholder="e.g., 195/65R15"
                className={`w-full px-4 py-3 border-2 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors ${
                  errors.tireSize ? 'border-red-500' : 'border-gray-300'
                }`}
                disabled={isSubmitting}
              />
              {errors.tireSize && (
                <p className="mt-1 text-sm text-red-600">{errors.tireSize.message}</p>
              )}
              <p className="mt-1 text-xs text-gray-500">
                Format: Width/Profile + Type + Rim (e.g., 195/65R15)
              </p>
            </div>

            {/* Quantity Field */}
            <div className="mb-6">
              <label className="block text-gray-700 font-semibold mb-2" htmlFor="quantity">
                Quantity (Optional)
              </label>
              <select
                {...register('quantity')}
                id="quantity"
                className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors"
                disabled={isSubmitting}
              >
                <option value="">Select quantity</option>
                {[1, 2, 3, 4, 5, 6].map((num) => (
                  <option key={num} value={num}>
                    {num} {num === 1 ? 'Tire' : 'Tires'}
                  </option>
                ))}
              </select>
            </div>

            {/* Vehicle Details Checkbox */}
            <div className="mb-6">
              <label className="flex items-center cursor-pointer">
                <input
                  {...register('includeVehicle')}
                  type="checkbox"
                  className="w-5 h-5 text-primary-600 border-gray-300 rounded focus:ring-2 focus:ring-primary-500"
                  disabled={isSubmitting}
                />
                <span className="ml-2 text-gray-700">
                  Include vehicle information (optional)
                </span>
              </label>
            </div>

            {/* Vehicle Field (conditional) */}
            {includeVehicle && (
              <motion.div
                initial={{ opacity: 0, height: 0 }}
                animate={{ opacity: 1, height: 'auto' }}
                exit={{ opacity: 0, height: 0 }}
                className="mb-6"
              >
                <label className="block text-gray-700 font-semibold mb-2" htmlFor="vehicle">
                  Vehicle Model
                </label>
                <input
                  {...register('vehicle')}
                  id="vehicle"
                  type="text"
                  placeholder="e.g., Toyota Aqua 2015"
                  className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors"
                  disabled={isSubmitting}
                />
              </motion.div>
            )}

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isSubmitting}
              className="w-full bg-gradient-to-r from-primary-600 to-primary-700 hover:from-primary-700 hover:to-primary-800 text-white font-bold py-4 px-6 rounded-lg transition-all duration-300 transform hover:scale-[1.02] disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-primary-500/30"
            >
              {isSubmitting ? (
                <span className="flex items-center justify-center">
                  <Loader2 className="animate-spin mr-2" />
                  Sending Request...
                </span>
              ) : (
                'Get Price on WhatsApp'
              )}
            </button>

            {/* Notification */}
            {notification && (
              <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                className={`mt-6 p-4 rounded-lg flex items-start gap-3 ${
                  notification.type === 'success'
                    ? 'bg-green-50 text-green-800 border border-green-200'
                    : notification.type === 'warning'
                      ? 'bg-yellow-50 text-yellow-800 border border-yellow-200'
                      : 'bg-red-50 text-red-800 border border-red-200'
                }`}
              >
                {notification.type === 'success' ? (
                  <CheckCircle2 className="w-5 h-5 flex-shrink-0 mt-0.5" />
                ) : (
                  <XCircle className="w-5 h-5 flex-shrink-0 mt-0.5" />
                )}
                <p className="text-sm font-medium">{notification.message}</p>
              </motion.div>
            )}

            <p className="text-xs text-gray-500 text-center mt-6">
              By submitting this form, you agree to receive price quotes via WhatsApp
            </p>
          </motion.form>
        </motion.div>
      </div>
    </section>
  )
}
