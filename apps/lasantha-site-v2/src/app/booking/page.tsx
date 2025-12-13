'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { Calendar, User, Phone, CheckCircle2, XCircle, Loader2, ArrowLeft } from 'lucide-react'
import Link from 'next/link'

const timeSlots = [
  '08:00 AM', '09:00 AM', '10:00 AM', '11:00 AM',
  '12:00 PM', '01:00 PM', '02:00 PM', '03:00 PM',
  '04:00 PM', '05:00 PM'
]

const services = [
  'Tyre Installation',
  'Computerized Wheel Alignment',
  'Computerized Wheel Balancing',
  'Tyre Rotation',
  'Nitrogen Air Filling',
  'Puncture Repair',
  'Suspension Checkup'
]

export default function BookingPage() {
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    service: '',
    date: '',
    time: '',
    vehicleNo: '',
    message: ''
  })

  const [loading, setLoading] = useState(false)
  const [notification, setNotification] = useState<{
    type: 'success' | 'error'
    message: string
  } | null>(null)

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    
    // Format phone number: allow only digits, spaces, and hyphens
    if (name === 'phone') {
      const cleaned = value.replace(/[^\d\s-]/g, '')
      setFormData({ ...formData, [name]: cleaned })
    } else {
      setFormData({ ...formData, [name]: value })
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setNotification(null)

    try {
      const response = await fetch('/api/appointments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          customerName: formData.name,
          phoneNumber: formData.phone,
          vehicleNumber: formData.vehicleNo || null,
          serviceType: formData.service,
          appointmentDate: formData.date,
          timeSlot: formData.time,
          notes: formData.message || null
        })
      })

      const data = await response.json()

      if (data.success) {
        setNotification({
          type: 'success',
          message: `✅ Appointment booked! Reference: ${data.referenceNo}. Check your WhatsApp for confirmation.`
        })
        
        setFormData({
          name: '',
          phone: '',
          service: '',
          date: '',
          time: '',
          vehicleNo: '',
          message: ''
        })
      } else {
        setNotification({
          type: 'error',
          message: `❌ ${data.message || 'Failed to book appointment. Please try again.'}`
        })
      }

    } catch (error) {
      console.error('Appointment booking error:', error)
      setNotification({
        type: 'error',
        message: '❌ Connection error. Please check your internet and try again.'
      })
    } finally {
      setLoading(false)
    }
  }

  const today = new Date().toISOString().split('T')[0]

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-2xl mx-auto">
        <Link href="/" className="inline-flex items-center text-gray-600 hover:text-primary-600 mb-8 transition-colors">
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Home
        </Link>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-white rounded-2xl shadow-xl overflow-hidden"
        >
          <div className="p-6 md:p-8">
            <div className="text-center mb-8">
              <div className="inline-flex items-center justify-center p-3 bg-primary-50 rounded-2xl mb-4 ring-1 ring-primary-100">
                <Calendar className="w-8 h-8 text-primary-600" />
              </div>
              <h1 className="text-3xl font-bold text-gray-900">
                Book <span className="text-primary-600">VIP Service</span>
              </h1>
              <p className="text-gray-600 mt-2">Skip the queue. Book your appointment instantly.</p>
            </div>

            {notification && (
              <div className={`mb-6 p-4 rounded-xl flex items-center gap-3 ${
                notification.type === 'success'
                  ? 'bg-green-50 text-green-800 border border-green-200'
                  : 'bg-red-50 text-red-800 border border-red-200'
              }`}>
                {notification.type === 'success' ? (
                  <CheckCircle2 className="w-5 h-5 flex-shrink-0" />
                ) : (
                  <XCircle className="w-5 h-5 flex-shrink-0" />
                )}
                <p className="text-sm font-medium">{notification.message}</p>
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              <div className="grid md:grid-cols-2 gap-5">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Name</label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-4 h-4" />
                    <input
                      type="text"
                      name="name"
                      required
                      value={formData.name || ''}
                      onChange={handleChange}
                      className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                      placeholder="John Doe"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Phone</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-4 h-4" />
                    <input
                      type="tel"
                      name="phone"
                      required
                      value={formData.phone || ''}
                      onChange={handleChange}
                      pattern="[\d\s-]{10,15}"
                      title="Please enter a valid phone number (10-15 digits)"
                      className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                      placeholder="077 123 4567"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Service</label>
                  <select
                    name="service"
                    required
                    value={formData.service || ''}
                    onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                  >
                    <option value="">Select Service</option>
                    {services.map((s, i) => <option key={i} value={s}>{s}</option>)}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Vehicle No</label>
                  <input
                    type="text"
                    name="vehicleNo"
                    value={formData.vehicleNo || ''}
                    onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                    placeholder="CAB-1234"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Date</label>
                  <input
                    type="date"
                    name="date"
                    required
                    min={today}
                    value={formData.date || ''}
                    onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Time</label>
                  <select
                    name="time"
                    required
                    value={formData.time || ''}
                    onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                  >
                    <option value="">Select Time</option>
                    {timeSlots.map((t, i) => <option key={i} value={t}>{t}</option>)}
                  </select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Note (Optional)</label>
                <textarea
                  name="message"
                  rows={2}
                  value={formData.message || ''}
                  onChange={handleChange}
                  className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none resize-none"
                  placeholder="Any specific requirements?"
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-primary-600 hover:bg-primary-700 text-white py-3.5 rounded-xl font-bold transition-all flex items-center justify-center gap-2 shadow-lg disabled:opacity-70"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Booking Appointment...
                  </>
                ) : (
                  <>
                    <Calendar className="w-5 h-5" />
                    Book Appointment
                  </>
                )}
              </button>
            </form>
          </div>
        </motion.div>
      </div>
    </div>
  )
}
