'use client'

import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Calendar, X, User, Phone, CheckCircle2, XCircle, Loader2, Package, Crown, Clock } from 'lucide-react'
import { getBotApiUrl } from '@/utils/getBotApiUrl'

interface QuotationItem {
  itemId: string
  description: string
  brand: string
  price: number
  quantity: number
  isService: boolean
}

interface Quotation {
  Id: number
  RefCode: string
  TyreSize: string
  Items: QuotationItem[]
  CustomerPhone: string
  CustomerName: string
  TotalAmount: number
}

interface BookingModalProps {
  isOpen: boolean
  onClose: () => void
  refCode?: string // Optional quotation reference
}

// Generate all time slots from 7:00 AM to 8:00 PM with 30-minute intervals
const generateTimeSlots = () => {
  const slots: { time: string; display: string; hour: number; minute: number }[] = []
  for (let hour = 7; hour <= 20; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
      // Skip 8:30 PM (we only go up to 8:00 PM)
      if (hour === 20 && minute > 0) continue
      
      const displayHour = hour > 12 ? hour - 12 : hour === 0 ? 12 : hour
      const period = hour >= 12 ? 'PM' : 'AM'
      const display = `${displayHour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')} ${period}`
      const time24 = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`
      
      slots.push({ time: time24, display, hour, minute })
    }
  }
  return slots
}

const allTimeSlots = generateTimeSlots()

// Check if a time slot is within wheel alignment hours (7:30 AM - 5:30 PM)
const isWheelAlignmentAvailable = (hour: number, minute: number): boolean => {
  const timeInMinutes = hour * 60 + minute
  const startTime = 7 * 60 + 30  // 7:30 AM
  const endTime = 17 * 60 + 30   // 5:30 PM
  return timeInMinutes >= startTime && timeInMinutes <= endTime
}

const services = [
  'Tyre Installation',
  'Computerized Wheel Alignment',
  'Computerized Wheel Balancing',
  'Tyre Rotation',
  'Nitrogen Air Filling',
  'Puncture Repair',
  'Suspension Checkup'
]

const BOT_API_URL = getBotApiUrl()

export default function BookingModal({ isOpen, onClose, refCode }: BookingModalProps) {
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
  const [loadingQuotation, setLoadingQuotation] = useState(false)
  const [quotation, setQuotation] = useState<Quotation | null>(null)
  const [selectedQuotationItems, setSelectedQuotationItems] = useState<string[]>([])
  const [notification, setNotification] = useState<{
    type: 'success' | 'error'
    message: string
  } | null>(null)

  // Fetch quotation data when refCode is provided
  useEffect(() => {
    if (refCode && isOpen) {
      fetchQuotation(refCode)
    }
  }, [refCode, isOpen])

  const fetchQuotation = async (code: string) => {
    setLoadingQuotation(true)
    try {
      const response = await fetch(`${BOT_API_URL}/api/quotations/${code}`)
      const data = await response.json()
      
      if (data.ok && data.quotation) {
        setQuotation(data.quotation)
        // Auto-fill customer info
        setFormData(prev => ({
          ...prev,
          name: data.quotation.CustomerName || '',
          phone: data.quotation.CustomerPhone || ''
        }))
        // Select all quotation items by default
        if (data.quotation.Items && Array.isArray(data.quotation.Items)) {
          setSelectedQuotationItems(data.quotation.Items.map((_: QuotationItem, i: number) => String(i)))
        }
      }
    } catch (error) {
      console.error('Failed to fetch quotation:', error)
    } finally {
      setLoadingQuotation(false)
    }
  }

  // Check if Wheel Alignment is selected
  const isWheelAlignment = formData.service === 'Computerized Wheel Alignment'

  // Reset time when service changes if current time is restricted for wheel alignment
  useEffect(() => {
    if (formData.time && isWheelAlignment) {
      const [hours, minutes] = formData.time.split(':').map(Number)
      if (!isWheelAlignmentAvailable(hours, minutes)) {
        setFormData(prev => ({ ...prev, time: '' }))
      }
    }
  }, [formData.service, isWheelAlignment])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    })
  }

  const toggleQuotationItem = (idx: string) => {
    setSelectedQuotationItems(prev => 
      prev.includes(idx) 
        ? prev.filter(i => i !== idx)
        : [...prev, idx]
    )
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setNotification(null)

    // Build notes with selected quotation items
    let notes = formData.message || ''
    if (quotation && selectedQuotationItems.length > 0) {
      const selectedItems = selectedQuotationItems
        .map(idx => quotation.Items[parseInt(idx)])
        .filter(Boolean)
        .map(item => `${item.description} (${item.brand}) x${item.quantity}`)
        .join(', ')
      notes = `📦 Items from Quotation [${quotation.RefCode}]:\n${selectedItems}\n\n${notes}`
    }

    try {
      // Use Next.js API route (same as price request) - Vercel server routes to bot
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
          notes: notes || null,
          quotationRefCode: quotation?.RefCode || null,
          quotationItems: quotation ? selectedQuotationItems.map(idx => quotation.Items[parseInt(idx)]) : null
        })
      })

      const data = await response.json()

      if (data.success) {
        // Mark quotation as booked
        if (quotation?.RefCode) {
          try {
            await fetch(`${BOT_API_URL}/api/quotations/${quotation.RefCode}/booked`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ bookingRef: data.referenceNo })
            })
          } catch (err) {
            console.error('Failed to update quotation status:', err)
          }
        }

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
        setQuotation(null)
        setSelectedQuotationItems([])
        
        setTimeout(() => {
          onClose()
          setNotification(null)
        }, 5000)
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
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          onClick={onClose}
          className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4 overflow-y-auto"
        >
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            onClick={(e) => e.stopPropagation()}
            className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl my-8 relative"
          >
            <button
              onClick={onClose}
              className="absolute top-4 right-4 p-2 rounded-full hover:bg-gray-100 transition-colors z-10"
            >
              <X className="w-6 h-6 text-gray-500" />
            </button>

            <div className="p-6 md:p-8">
              <div className="text-center mb-8">
                {quotation ? (
                  <>
                    <div className="inline-flex items-center justify-center p-3 bg-gradient-to-br from-amber-100 to-yellow-100 rounded-2xl mb-4 ring-1 ring-amber-300">
                      <Crown className="w-8 h-8 text-amber-600" />
                    </div>
                    <h2 className="text-3xl font-bold text-gray-900">
                      <span className="text-amber-600">Royal</span> Booking
                    </h2>
                    <p className="text-gray-600 mt-2">
                      Your quotation is ready! Complete your VIP appointment.
                    </p>
                    <div className="mt-3 inline-flex items-center gap-2 px-3 py-1 bg-amber-50 border border-amber-200 rounded-full text-amber-700 text-sm font-medium">
                      <Package className="w-4 h-4" />
                      Quotation: {quotation.RefCode}
                    </div>
                  </>
                ) : (
                  <>
                    <div className="inline-flex items-center justify-center p-3 bg-primary-50 rounded-2xl mb-4 ring-1 ring-primary-100">
                      <Calendar className="w-8 h-8 text-primary-600" />
                    </div>
                    <h2 className="text-3xl font-bold text-gray-900">
                      Book <span className="text-primary-600">VIP Service</span>
                    </h2>
                    <p className="text-gray-600 mt-2">Skip the queue. Book your appointment instantly.</p>
                  </>
                )}
              </div>

              {loadingQuotation && (
                <div className="mb-6 p-4 rounded-xl flex items-center justify-center gap-3 bg-blue-50 text-blue-800 border border-blue-200">
                  <Loader2 className="w-5 h-5 animate-spin" />
                  <p className="text-sm font-medium">Loading your quotation...</p>
                </div>
              )}

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

              {/* Quotation Items Section */}
              {quotation && quotation.Items && quotation.Items.length > 0 && (
                <div className="mb-6 p-4 bg-gradient-to-br from-amber-50 to-yellow-50 rounded-xl border border-amber-200">
                  <h3 className="font-bold text-amber-800 mb-3 flex items-center gap-2">
                    <Package className="w-5 h-5" />
                    Select Items for Installation
                  </h3>
                  <div className="space-y-2">
                    {quotation.Items.map((item, idx) => (
                      <label 
                        key={idx}
                        className={`flex items-center gap-3 p-3 rounded-lg cursor-pointer transition-all ${
                          selectedQuotationItems.includes(String(idx))
                            ? 'bg-amber-100 border-2 border-amber-400'
                            : 'bg-white border border-amber-200 hover:border-amber-300'
                        }`}
                      >
                        <input
                          type="checkbox"
                          checked={selectedQuotationItems.includes(String(idx))}
                          onChange={() => toggleQuotationItem(String(idx))}
                          className="w-5 h-5 text-amber-600 rounded focus:ring-amber-500"
                        />
                        <div className="flex-1">
                          <p className="font-medium text-gray-900">{item.description}</p>
                          <p className="text-sm text-gray-600">{item.brand} × {item.quantity}</p>
                        </div>
                        <span className="font-bold text-amber-700">
                          Rs {(item.price * item.quantity).toLocaleString()}
                        </span>
                      </label>
                    ))}
                  </div>
                  <div className="mt-3 pt-3 border-t border-amber-200 flex justify-between items-center">
                    <span className="font-medium text-amber-800">Total:</span>
                    <span className="text-xl font-bold text-amber-700">
                      Rs {quotation.TotalAmount?.toLocaleString() || 0}
                    </span>
                  </div>
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
                        value={formData.name}
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
                        value={formData.phone}
                        onChange={handleChange}
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
                      value={formData.service}
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
                      value={formData.vehicleNo}
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
                      value={formData.date}
                      onChange={handleChange}
                      className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 outline-none"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                      Time (7:00 AM - 8:00 PM)
                      {isWheelAlignment && (
                        <span className="text-xs text-amber-600 bg-amber-50 px-2 py-0.5 rounded-full flex items-center gap-1">
                          <Clock className="w-3 h-3" />
                          7:30 AM - 5:30 PM only
                        </span>
                      )}
                    </label>
                    
                    {/* Visual Time Grid */}
                    <div className="grid grid-cols-4 sm:grid-cols-5 gap-1.5">
                      {allTimeSlots.map((slot) => {
                        const isRestricted = isWheelAlignment && !isWheelAlignmentAvailable(slot.hour, slot.minute)
                        const isSelected = formData.time === slot.time
                        
                        return (
                          <button
                            key={slot.time}
                            type="button"
                            onClick={() => !isRestricted && setFormData(prev => ({ ...prev, time: slot.time }))}
                            disabled={isRestricted}
                            className={`relative py-2 px-1 rounded-lg text-xs font-medium transition-all ${
                              isSelected
                                ? 'bg-primary-600 text-white shadow-md ring-2 ring-primary-400'
                                : isRestricted
                                  ? 'bg-gray-100 text-gray-400 cursor-not-allowed border border-dashed border-red-300'
                                  : 'bg-gray-50 text-gray-700 hover:bg-primary-50 hover:text-primary-600 border border-gray-200'
                            }`}
                          >
                            {isRestricted && (
                              <div className="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full flex items-center justify-center">
                                <X className="w-2 h-2 text-white" />
                              </div>
                            )}
                            {slot.display}
                          </button>
                        )
                      })}
                    </div>
                    
                    {isWheelAlignment && (
                      <p className="text-xs text-amber-600 mt-2 flex items-center gap-1">
                        <Clock className="w-3 h-3" />
                        ⚙️ Wheel Alignment available only during technician hours (7:30 AM - 5:30 PM)
                      </p>
                    )}
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Note (Optional)</label>
                  <textarea
                    name="message"
                    rows={2}
                    value={formData.message}
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
        </motion.div>
      )}
    </AnimatePresence>
  )
}
