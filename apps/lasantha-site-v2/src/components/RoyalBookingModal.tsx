'use client'

import { useState, useEffect, useRef } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Calendar, X, User, Phone, CheckCircle2, XCircle, Loader2, 
  Package, Crown, Clock, Car, Sparkles, ChevronRight, 
  Shield, Star, MapPin, MessageSquare, Check, ArrowLeft, ShieldCheck
} from 'lucide-react'
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
  QuotationNumber?: string
  TyreSize: string
  Items: QuotationItem[]
  CustomerPhone: string
  CustomerName: string
  VehicleNumber?: string
  TotalAmount: number
}

interface RoyalBookingModalProps {
  isOpen: boolean
  onClose: () => void
  refCode?: string
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
  { id: 'tyre-install', name: 'Tyre Installation', icon: '🛞', time: '30-45 min' },
  { id: 'alignment', name: 'Computerized Wheel Alignment', icon: '⚙️', time: '45 min' },
  { id: 'balancing', name: 'Computerized Wheel Balancing', icon: '⚖️', time: '30 min' },
  { id: 'rotation', name: 'Tyre Rotation', icon: '🔄', time: '20 min' },
  { id: 'nitrogen', name: 'Nitrogen Air Filling', icon: '💨', time: '15 min' },
  { id: 'puncture', name: 'Puncture Repair', icon: '🔧', time: '20-30 min' },
  { id: 'suspension', name: 'Suspension Checkup', icon: '🔍', time: '30 min' }
]

const BOT_API_URL = getBotApiUrl()

export default function RoyalBookingModal({ isOpen, onClose, refCode }: RoyalBookingModalProps) {
  const [step, setStep] = useState(1)
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
  const [bookingComplete, setBookingComplete] = useState(false)
  const [bookingRef, setBookingRef] = useState('')
  const [notification, setNotification] = useState<{
    type: 'success' | 'error'
    message: string
  } | null>(null)

  const modalRef = useRef<HTMLDivElement>(null)

  // Fetch quotation data
  useEffect(() => {
    if (refCode && isOpen) {
      fetchQuotation(refCode)
    }
  }, [refCode, isOpen])

  // Reset on close
  useEffect(() => {
    if (!isOpen) {
      setTimeout(() => {
        setStep(1)
        setBookingComplete(false)
        setNotification(null)
      }, 300)
    }
  }, [isOpen])

  // Lock body scroll when modal is open
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden'
    } else {
      document.body.style.overflow = ''
    }
    return () => {
      document.body.style.overflow = ''
    }
  }, [isOpen])

  const fetchQuotation = async (code: string) => {
    setLoadingQuotation(true)
    setNotification(null)
    try {
      console.log(`[RoyalBooking] Fetching quotation: ${BOT_API_URL}/api/quotations/${code}`)
      
      const controller = new AbortController()
      const timeoutId = setTimeout(() => controller.abort(), 10000) // 10 second timeout
      
      const response = await fetch(`${BOT_API_URL}/api/quotations/${code}`, {
        signal: controller.signal
      })
      clearTimeout(timeoutId)
      
      console.log(`[RoyalBooking] Response status: ${response.status}`)
      
      if (!response.ok) {
        throw new Error(`API returned ${response.status}: ${response.statusText}`)
      }
      
      const data = await response.json()
      console.log('[RoyalBooking] API Response:', data)
      
      if (data.ok && data.quotation) {
        setQuotation(data.quotation)
        // Safely set form data with fallback to empty strings
        setFormData(prev => ({
          ...prev,
          name: data.quotation.CustomerName || '',
          phone: data.quotation.CustomerPhone || '',
          vehicleNo: data.quotation.VehicleNumber || ''
        }))
        if (data.quotation.Items && Array.isArray(data.quotation.Items)) {
          setSelectedQuotationItems(data.quotation.Items.map((_: QuotationItem, i: number) => String(i)))
        }
      } else {
        console.error('[RoyalBooking] Invalid response structure:', data)
        setNotification({
          type: 'error',
          message: data.error || 'Failed to load quotation details. Please fill manually.'
        })
      }
    } catch (error: any) {
      console.error('[RoyalBooking] Fetch error:', error)
      setNotification({
        type: 'error',
        message: error.name === 'AbortError'
          ? 'Request timeout. Please check your connection and try again.'
          : `Failed to load quotation: ${error.message || 'Network error'}`
      })
    } finally {
      setLoadingQuotation(false)
    }
  }

  const isWheelAlignment = formData.service === 'Computerized Wheel Alignment'

  // Check if selected time is valid for wheel alignment
  useEffect(() => {
    if (formData.time && isWheelAlignment) {
      const [hours, minutes] = formData.time.split(':').map(Number)
      if (!isWheelAlignmentAvailable(hours, minutes)) {
        setFormData(prev => ({ ...prev, time: '' }))
      }
    }
  }, [formData.service, isWheelAlignment])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const toggleQuotationItem = (idx: string) => {
    setSelectedQuotationItems(prev => 
      prev.includes(idx) ? prev.filter(i => i !== idx) : [...prev, idx]
    )
  }

  const selectService = (serviceName: string) => {
    setFormData(prev => ({ ...prev, service: serviceName }))
  }

  const handleSubmit = async () => {
    setLoading(true)
    setNotification(null)

    let notes = formData.message || ''
    if (quotation && selectedQuotationItems.length > 0) {
      const selectedItems = selectedQuotationItems
        .map(idx => quotation.Items[parseInt(idx)])
        .filter(Boolean)
        .map(item => `${item.description} (${item.brand}) x${item.quantity}`)
        .join(', ')
      const quotationRef = quotation.QuotationNumber || quotation.RefCode;
      notes = `📦 Items from Quotation [${quotationRef}]:\n${selectedItems}\n\n${notes}`
    }

    try {
      const response = await fetch('/api/appointments', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
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
        setBookingRef(data.referenceNo)
        setBookingComplete(true)
      } else {
        setNotification({
          type: 'error',
          message: data.message || 'Failed to book appointment. Please try again.'
        })
      }
    } catch (error) {
      console.error('Appointment booking error:', error)
      setNotification({
        type: 'error',
        message: 'Connection error. Please check your internet and try again.'
      })
    } finally {
      setLoading(false)
    }
  }

  const today = new Date().toISOString().split('T')[0]
  const maxDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]

  const canProceedStep1 = formData.name && formData.phone
  const canProceedStep2 = formData.service
  const canProceedStep3 = formData.date && formData.time

  const totalSteps = quotation ? 4 : 3

  // Get selected items total
  const selectedTotal = quotation && selectedQuotationItems.length > 0
    ? selectedQuotationItems.reduce((sum, idx) => {
        const item = quotation.Items[parseInt(idx)]
        return sum + (item ? item.price * item.quantity : 0)
      }, 0)
    : 0

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 z-[100] flex items-end sm:items-center justify-center"
        >
          {/* Backdrop with blur */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="absolute inset-0 bg-black/70 backdrop-blur-md"
          />

          {/* Modal Container - Full screen on mobile, centered on desktop */}
          <motion.div
            ref={modalRef}
            initial={{ opacity: 0, y: '100%' }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: '100%' }}
            transition={{ type: 'spring', damping: 30, stiffness: 300 }}
            className="relative w-full h-[100dvh] sm:h-auto sm:max-h-[90vh] sm:max-w-lg sm:rounded-3xl overflow-hidden bg-gradient-to-b from-slate-900 via-slate-900 to-slate-950 shadow-2xl"
          >
            {/* Decorative gradient orbs */}
            <div className="absolute top-0 left-1/4 w-64 h-64 bg-amber-500/20 rounded-full blur-3xl pointer-events-none" />
            <div className="absolute bottom-0 right-1/4 w-64 h-64 bg-primary-500/20 rounded-full blur-3xl pointer-events-none" />

            {/* Header */}
            <div className="relative z-10 px-4 pt-4 pb-3 sm:px-6 sm:pt-6">
              <div className="flex items-center justify-between">
                {step > 1 && !bookingComplete ? (
                  <button 
                    onClick={() => setStep(step - 1)}
                    className="p-2 -ml-2 rounded-full hover:bg-white/10 transition-colors"
                  >
                    <ArrowLeft className="w-5 h-5 text-white" />
                  </button>
                ) : (
                  <div className="w-9" />
                )}
                
                {/* Royal Badge */}
                <div className="flex items-center gap-2">
                  {quotation ? (
                    <div className="flex items-center gap-1.5 px-3 py-1 bg-gradient-to-r from-amber-500/20 to-yellow-500/20 border border-amber-500/30 rounded-full">
                      <Crown className="w-4 h-4 text-amber-400" />
                      <span className="text-xs font-semibold text-amber-300">ROYAL</span>
                    </div>
                  ) : (
                    <div className="flex items-center gap-1.5 px-3 py-1 bg-primary-500/20 border border-primary-500/30 rounded-full">
                      <Sparkles className="w-4 h-4 text-primary-400" />
                      <span className="text-xs font-semibold text-primary-300">VIP</span>
                    </div>
                  )}
                </div>

                <button 
                  onClick={onClose}
                  className="p-2 -mr-2 rounded-full hover:bg-white/10 transition-colors"
                >
                  <X className="w-5 h-5 text-white/70" />
                </button>
              </div>

              {/* Progress Steps */}
              {!bookingComplete && (
                <div className="mt-4 flex items-center justify-center gap-2">
                  {[...Array(totalSteps)].map((_, i) => (
                    <div
                      key={i}
                      className={`h-1 rounded-full transition-all duration-300 ${
                        i + 1 <= step 
                          ? quotation ? 'bg-amber-400' : 'bg-primary-400'
                          : 'bg-white/20'
                      } ${i + 1 === step ? 'w-8' : 'w-4'}`}
                    />
                  ))}
                </div>
              )}
            </div>

            {/* Scrollable Content */}
            <div className="relative z-10 h-[calc(100dvh-140px)] sm:h-auto sm:max-h-[calc(90vh-180px)] overflow-y-auto overscroll-contain px-4 pb-4 sm:px-6 sm:pb-6">
              
              {/* Loading Quotation */}
              {loadingQuotation && (
                <div className="flex flex-col items-center justify-center py-20">
                  <div className="relative">
                    <div className="w-16 h-16 border-4 border-amber-500/30 rounded-full animate-pulse" />
                    <Crown className="absolute inset-0 m-auto w-8 h-8 text-amber-400 animate-bounce" />
                  </div>
                  <p className="mt-4 text-amber-300 font-medium">Loading your royal quotation...</p>
                </div>
              )}

              {/* Booking Complete */}
              {bookingComplete && !loadingQuotation && (
                <motion.div
                  initial={{ opacity: 0, scale: 0.9 }}
                  animate={{ opacity: 1, scale: 1 }}
                  className="flex flex-col items-center justify-center py-12 text-center"
                >
                  <div className="relative mb-6">
                    <div className="w-24 h-24 bg-gradient-to-br from-green-400 to-emerald-500 rounded-full flex items-center justify-center shadow-lg shadow-green-500/30">
                      <CheckCircle2 className="w-12 h-12 text-white" />
                    </div>
                    <motion.div
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      transition={{ delay: 0.3, type: 'spring' }}
                      className="absolute -top-2 -right-2 w-10 h-10 bg-amber-400 rounded-full flex items-center justify-center"
                    >
                      <Star className="w-5 h-5 text-amber-900" />
                    </motion.div>
                  </div>
                  
                  <h2 className="text-2xl font-bold text-white mb-2">Booking Confirmed! 🎉</h2>
                  <p className="text-gray-400 mb-4">Your VIP appointment is reserved</p>
                  
                  <div className="bg-white/5 border border-white/10 rounded-2xl p-4 mb-6 w-full max-w-xs">
                    <p className="text-xs text-gray-500 uppercase tracking-wider mb-1">Reference Number</p>
                    <p className="text-xl font-mono font-bold text-white">{bookingRef}</p>
                  </div>

                  <div className="space-y-2 text-sm text-gray-400 mb-8">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-primary-400" />
                      <span>{formData.date} at {formData.time}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <MapPin className="w-4 h-4 text-primary-400" />
                      <span>Lasantha Tyre, Homagama</span>
                    </div>
                  </div>

                  <div className="flex items-center gap-2 text-green-400 bg-green-500/10 px-4 py-2 rounded-full">
                    <MessageSquare className="w-4 h-4" />
                    <span className="text-sm font-medium">WhatsApp confirmation sent!</span>
                  </div>

                  <button
                    onClick={onClose}
                    className="mt-8 w-full max-w-xs py-3 bg-white text-slate-900 rounded-xl font-bold hover:bg-gray-100 transition-colors"
                  >
                    Done
                  </button>
                </motion.div>
              )}

              {/* Step 1: Customer Info */}
              {step === 1 && !loadingQuotation && !bookingComplete && (
                <motion.div
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="space-y-6"
                >
                  <div className="text-center mb-8">
                    <h2 className="text-2xl font-bold text-white mb-2">
                      {quotation ? '👑 Welcome Back!' : 'Book Your Service'}
                    </h2>
                    <p className="text-gray-400">
                      {quotation 
                        ? `Your quotation ${quotation.QuotationNumber || quotation.RefCode} is ready`
                        : 'Skip the queue with VIP booking'
                      }
                    </p>
                  </div>

                  {/* Quotation Items Preview */}
                  {quotation && quotation.Items && quotation.Items.length > 0 && (
                    <div className="bg-gradient-to-br from-amber-500/10 to-yellow-500/5 border border-amber-500/20 rounded-2xl p-4 mb-6">
                      <div className="flex items-center justify-between mb-3">
                        <h3 className="font-semibold text-amber-300 flex items-center gap-2">
                          <Package className="w-4 h-4" />
                          Your Items
                        </h3>
                        <span className="text-xs text-amber-400/70">{quotation.Items.length} items</span>
                      </div>
                      <div className="space-y-2 max-h-32 overflow-y-auto">
                        {quotation.Items.map((item, idx) => (
                          <label
                            key={idx}
                            className={`flex items-center gap-3 p-2 rounded-lg cursor-pointer transition-all ${
                              selectedQuotationItems.includes(String(idx))
                                ? 'bg-amber-500/20'
                                : 'bg-white/5 hover:bg-white/10'
                            }`}
                          >
                            <div className={`w-5 h-5 rounded border-2 flex items-center justify-center transition-all ${
                              selectedQuotationItems.includes(String(idx))
                                ? 'bg-amber-400 border-amber-400'
                                : 'border-white/30'
                            }`}>
                              {selectedQuotationItems.includes(String(idx)) && (
                                <Check className="w-3 h-3 text-slate-900" />
                              )}
                            </div>
                            <input
                              type="checkbox"
                              checked={selectedQuotationItems.includes(String(idx))}
                              onChange={() => toggleQuotationItem(String(idx))}
                              className="sr-only"
                            />
                            <div className="flex-1 min-w-0">
                              <p className="text-sm text-white truncate">{item.description}</p>
                              <p className="text-xs text-gray-500">{item.brand} × {item.quantity}</p>
                            </div>
                            <span className="text-sm font-semibold text-amber-300">
                              Rs {(item.price * item.quantity).toLocaleString()}
                            </span>
                          </label>
                        ))}
                      </div>
                      {selectedTotal > 0 && (
                        <div className="mt-3 pt-3 border-t border-amber-500/20 flex justify-between">
                          <span className="text-sm text-amber-300">Selected Total:</span>
                          <span className="font-bold text-amber-300">Rs {selectedTotal.toLocaleString()}</span>
                        </div>
                      )}
                    </div>
                  )}

                  {/* Customer Info Form */}
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-400 mb-2">Your Name</label>
                      <div className="relative">
                        <User className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 w-5 h-5" />
                        <input
                          type="text"
                          name="name"
                          value={formData.name || ''}
                          onChange={handleChange}
                          placeholder="Enter your name"
                          className="w-full pl-12 pr-4 py-4 bg-white/5 border border-white/10 rounded-xl text-white placeholder-gray-500 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 outline-none transition-all"
                        />
                      </div>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-400 mb-2">Phone Number</label>
                      <div className="relative">
                        <Phone className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 w-5 h-5" />
                        <input
                          type="tel"
                          name="phone"
                          value={formData.phone || ''}
                          onChange={handleChange}
                          placeholder="077 123 4567"
                          className="w-full pl-12 pr-4 py-4 bg-white/5 border border-white/10 rounded-xl text-white placeholder-gray-500 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 outline-none transition-all"
                        />
                      </div>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-400 mb-2">Vehicle Number (Optional)</label>
                      <div className="relative">
                        <Car className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 w-5 h-5" />
                        <input
                          type="text"
                          name="vehicleNo"
                          value={formData.vehicleNo || ''}
                          onChange={handleChange}
                          placeholder="CAB-1234"
                          className="w-full pl-12 pr-4 py-4 bg-white/5 border border-white/10 rounded-xl text-white placeholder-gray-500 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 outline-none transition-all"
                        />
                      </div>
                    </div>
                  </div>
                </motion.div>
              )}

              {/* Step 2: Select Service */}
              {step === 2 && !bookingComplete && (
                <motion.div
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="space-y-4"
                >
                  <div className="text-center mb-6">
                    <h2 className="text-2xl font-bold text-white mb-2">Select Service</h2>
                    <p className="text-gray-400">What do you need today?</p>
                  </div>

                  <div className="space-y-3">
                    {services.map((service) => (
                      <button
                        key={service.id}
                        onClick={() => selectService(service.name)}
                        className={`w-full flex items-center gap-4 p-4 rounded-2xl border transition-all ${
                          formData.service === service.name
                            ? 'bg-primary-500/20 border-primary-500'
                            : 'bg-white/5 border-white/10 hover:bg-white/10 hover:border-white/20'
                        }`}
                      >
                        <div className="text-2xl">{service.icon}</div>
                        <div className="flex-1 text-left">
                          <p className={`font-medium ${formData.service === service.name ? 'text-white' : 'text-gray-300'}`}>
                            {service.name}
                          </p>
                          <p className="text-xs text-gray-500">⏱️ {service.time}</p>
                        </div>
                        <div className={`w-6 h-6 rounded-full border-2 flex items-center justify-center transition-all ${
                          formData.service === service.name
                            ? 'bg-primary-500 border-primary-500'
                            : 'border-white/30'
                        }`}>
                          {formData.service === service.name && (
                            <Check className="w-4 h-4 text-white" />
                          )}
                        </div>
                      </button>
                    ))}
                  </div>

                  {isWheelAlignment && (
                    <motion.div 
                      initial={{ opacity: 0, y: -10 }}
                      animate={{ opacity: 1, y: 0 }}
                      className="mt-4 p-4 bg-gradient-to-r from-amber-500/15 to-yellow-500/10 border border-amber-500/30 rounded-xl"
                    >
                      <div className="flex items-start gap-3">
                        <div className="w-10 h-10 rounded-full bg-amber-500/20 flex items-center justify-center flex-shrink-0">
                          <Clock className="w-5 h-5 text-amber-400" />
                        </div>
                        <div>
                          <p className="font-semibold text-amber-300">⚙️ Wheel Alignment - Special Hours</p>
                          <p className="text-sm text-amber-400/80 mt-1">
                            Our computerized wheel alignment technician is available only during:
                          </p>
                          <div className="mt-2 inline-flex items-center gap-2 bg-amber-500/20 px-3 py-1.5 rounded-lg">
                            <Clock className="w-4 h-4 text-amber-300" />
                            <span className="font-bold text-amber-200">7:30 AM - 5:30 PM</span>
                          </div>
                          <p className="text-xs text-amber-400/60 mt-2">
                            Time slots outside these hours will be disabled in the next step.
                          </p>
                        </div>
                      </div>
                    </motion.div>
                  )}
                </motion.div>
              )}

              {/* Step 3: Date & Time */}
              {step === 3 && !bookingComplete && (
                <motion.div
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="space-y-6"
                >
                  <div className="text-center mb-6">
                    <h2 className="text-2xl font-bold text-white mb-2">Pick Date & Time</h2>
                    <p className="text-gray-400">When would you like to visit?</p>
                  </div>

                  {/* Date Picker */}
                  <div>
                    <label className="block text-sm font-medium text-gray-400 mb-2">Select Date</label>
                    <input
                      type="date"
                      name="date"
                      min={today}
                      max={maxDate}
                      value={formData.date || ''}
                      onChange={handleChange}
                      className="w-full px-4 py-4 bg-white/5 border border-white/10 rounded-xl text-white focus:border-primary-500 focus:ring-1 focus:ring-primary-500 outline-none transition-all [color-scheme:dark]"
                    />
                  </div>

                  {/* Time Slots Grid */}
                  <div>
                    <label className="block text-sm font-medium text-gray-400 mb-3">
                      Select Time (7:00 AM - 8:00 PM)
                    </label>
                    
                    {/* Wheel Alignment Restriction Notice */}
                    {isWheelAlignment && (
                      <div className="mb-4 p-3 bg-amber-500/10 border border-amber-500/30 rounded-xl flex items-start gap-3">
                        <Clock className="w-5 h-5 text-amber-400 flex-shrink-0 mt-0.5" />
                        <div>
                          <p className="text-sm font-medium text-amber-300">⚙️ Wheel Alignment - Technician Hours Only</p>
                          <p className="text-xs text-amber-400/70 mt-1">
                            Available only between <strong>7:30 AM - 5:30 PM</strong> when our alignment technician is present. 
                            Restricted time slots are shown as disabled below.
                          </p>
                        </div>
                      </div>
                    )}
                    
                    <div className="grid grid-cols-4 sm:grid-cols-5 gap-2">
                      {allTimeSlots.map((slot) => {
                        const isRestricted = isWheelAlignment && !isWheelAlignmentAvailable(slot.hour, slot.minute)
                        const isSelected = formData.time === slot.time
                        
                        return (
                          <button
                            key={slot.time}
                            type="button"
                            onClick={() => !isRestricted && setFormData(prev => ({ ...prev, time: slot.time }))}
                            disabled={isRestricted}
                            className={`relative py-2.5 px-1 rounded-xl text-xs sm:text-sm font-medium transition-all ${
                              isSelected
                                ? 'bg-gradient-to-br from-primary-500 to-primary-600 text-white shadow-lg shadow-primary-500/30 ring-2 ring-primary-400 scale-105'
                                : isRestricted
                                  ? 'bg-slate-800/50 text-slate-600 cursor-not-allowed border-2 border-dashed border-red-500/30'
                                  : 'bg-white/5 text-gray-400 hover:bg-white/15 hover:text-white border border-white/10 hover:border-primary-500/50'
                            }`}
                          >
                            {/* Restricted indicator */}
                            {isRestricted && (
                              <div className="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center">
                                <X className="w-2.5 h-2.5 text-white" />
                              </div>
                            )}
                            {slot.display}
                          </button>
                        )
                      })}
                    </div>
                    
                    {/* Legend for restricted slots */}
                    {isWheelAlignment && (
                      <div className="mt-4 flex items-center justify-center gap-6 text-xs">
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-4 rounded bg-white/5 border border-white/10" />
                          <span className="text-gray-400">Available</span>
                        </div>
                        <div className="flex items-center gap-2">
                          <div className="relative w-4 h-4 rounded bg-slate-800/50 border-2 border-dashed border-red-500/30">
                            <div className="absolute -top-1 -right-1 w-2 h-2 bg-red-500 rounded-full" />
                          </div>
                          <span className="text-gray-400">Alignment N/A</span>
                        </div>
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-4 rounded bg-primary-500" />
                          <span className="text-gray-400">Selected</span>
                        </div>
                      </div>
                    )}
                    
                    {/* Selected time display */}
                    {formData.time && (
                      <div className="mt-4 text-center">
                        <p className="text-primary-400 font-medium">
                          ⏰ Selected: {allTimeSlots.find(s => s.time === formData.time)?.display}
                        </p>
                      </div>
                    )}
                  </div>

                  {/* Notes */}
                  <div>
                    <label className="block text-sm font-medium text-gray-400 mb-2">Special Requests (Optional)</label>
                    <textarea
                      name="message"
                      rows={2}
                      value={formData.message || ''}
                      onChange={handleChange}
                      placeholder="Any specific requirements?"
                      className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white placeholder-gray-500 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 outline-none transition-all resize-none"
                    />
                  </div>
                </motion.div>
              )}

              {/* Step 4: Confirm (for quotation bookings) */}
              {step === 4 && quotation && !bookingComplete && (
                <motion.div
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="space-y-6"
                >
                  <div className="text-center mb-6">
                    <h2 className="text-2xl font-bold text-white mb-2">Confirm Booking</h2>
                    <p className="text-gray-400">Review your appointment details</p>
                  </div>

                  {/* Summary Card */}
                  <div className="bg-white/5 border border-white/10 rounded-2xl overflow-hidden">
                    <div className="p-4 bg-gradient-to-r from-amber-500/20 to-yellow-500/10 border-b border-white/10">
                      <div className="flex items-center gap-2">
                        <Crown className="w-5 h-5 text-amber-400" />
                        <span className="font-semibold text-amber-300">Royal Booking Summary</span>
                      </div>
                    </div>
                    
                    <div className="p-4 space-y-3">
                      <div className="flex justify-between">
                        <span className="text-gray-400">Customer</span>
                        <span className="text-white font-medium">{formData.name}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-400">Phone</span>
                        <span className="text-white font-medium">{formData.phone}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-400">Service</span>
                        <span className="text-white font-medium">{formData.service}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-400">Date & Time</span>
                        <span className="text-white font-medium">{formData.date} @ {formData.time}</span>
                      </div>
                      {formData.vehicleNo && (
                        <div className="flex justify-between">
                          <span className="text-gray-400">Vehicle</span>
                          <span className="text-white font-medium">{formData.vehicleNo}</span>
                        </div>
                      )}
                    </div>

                    {selectedTotal > 0 && (
                      <div className="p-4 bg-amber-500/10 border-t border-amber-500/20">
                        <div className="flex justify-between items-center">
                          <span className="text-amber-300 font-medium">Items Total</span>
                          <span className="text-xl font-bold text-amber-300">Rs {selectedTotal.toLocaleString()}</span>
                        </div>
                      </div>
                    )}
                  </div>

                  {/* Trust Badges */}
                  <div className="flex items-center justify-center gap-4 text-xs text-gray-500">
                    <div className="flex items-center gap-1">
                      <Shield className="w-3 h-3" />
                      <span>Secure</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Star className="w-3 h-3" />
                      <span>Premium Service</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Check className="w-3 h-3" />
                      <span>Guaranteed</span>
                    </div>
                  </div>
                </motion.div>
              )}

              {/* Error Notification */}
              {notification && notification.type === 'error' && (
                <div className="mt-4 p-4 bg-red-500/10 border border-red-500/20 rounded-xl flex items-start gap-3">
                  <XCircle className="w-5 h-5 text-red-400 flex-shrink-0 mt-0.5" />
                  <p className="text-sm text-red-300">{notification.message}</p>
                </div>
              )}
            </div>

            {/* Footer - Fixed at bottom */}
            {!loadingQuotation && !bookingComplete && (
              <div className="relative z-10 px-4 py-4 sm:px-6 border-t border-white/10 bg-slate-900/80 backdrop-blur-sm">
                {step === 1 && (
                  <button
                    onClick={() => setStep(2)}
                    disabled={!canProceedStep1}
                    className={`w-full py-4 rounded-xl font-bold text-lg flex items-center justify-center gap-2 transition-all ${
                      canProceedStep1
                        ? quotation
                          ? 'bg-gradient-to-r from-amber-500 to-yellow-500 text-slate-900 shadow-lg shadow-amber-500/30'
                          : 'bg-primary-500 text-white shadow-lg shadow-primary-500/30'
                        : 'bg-white/10 text-gray-500 cursor-not-allowed'
                    }`}
                  >
                    Continue
                    <ChevronRight className="w-5 h-5" />
                  </button>
                )}

                {step === 2 && (
                  <button
                    onClick={() => setStep(3)}
                    disabled={!canProceedStep2}
                    className={`w-full py-4 rounded-xl font-bold text-lg flex items-center justify-center gap-2 transition-all ${
                      canProceedStep2
                        ? 'bg-primary-500 text-white shadow-lg shadow-primary-500/30'
                        : 'bg-white/10 text-gray-500 cursor-not-allowed'
                    }`}
                  >
                    Continue
                    <ChevronRight className="w-5 h-5" />
                  </button>
                )}

                {step === 3 && !quotation && (
                  <button
                    onClick={handleSubmit}
                    disabled={!canProceedStep3 || loading}
                    className={`w-full py-4 rounded-xl font-bold text-lg flex items-center justify-center gap-2 transition-all ${
                      canProceedStep3 && !loading
                        ? 'bg-gradient-to-r from-green-500 to-emerald-500 text-white shadow-lg shadow-green-500/30'
                        : 'bg-white/10 text-gray-500 cursor-not-allowed'
                    }`}
                  >
                    {loading ? (
                      <>
                        <Loader2 className="w-5 h-5 animate-spin" />
                        Booking...
                      </>
                    ) : (
                      <>
                        <Calendar className="w-5 h-5" />
                        Confirm Booking
                      </>
                    )}
                  </button>
                )}

                {step === 3 && quotation && (
                  <button
                    onClick={() => setStep(4)}
                    disabled={!canProceedStep3}
                    className={`w-full py-4 rounded-xl font-bold text-lg flex items-center justify-center gap-2 transition-all ${
                      canProceedStep3
                        ? 'bg-gradient-to-r from-amber-500 to-yellow-500 text-slate-900 shadow-lg shadow-amber-500/30'
                        : 'bg-white/10 text-gray-500 cursor-not-allowed'
                    }`}
                  >
                    Review Booking
                    <ChevronRight className="w-5 h-5" />
                  </button>
                )}

                {step === 4 && quotation && (
                  <button
                    onClick={handleSubmit}
                    disabled={loading}
                    className={`w-full py-4 rounded-xl font-bold text-lg flex items-center justify-center gap-2 transition-all ${
                      !loading
                        ? 'bg-gradient-to-r from-green-500 to-emerald-500 text-white shadow-lg shadow-green-500/30'
                        : 'bg-white/10 text-gray-500 cursor-not-allowed'
                    }`}
                  >
                    {loading ? (
                      <>
                        <Loader2 className="w-5 h-5 animate-spin" />
                        Confirming...
                      </>
                    ) : (
                      <>
                        <Crown className="w-5 h-5" />
                        Confirm Royal Booking
                      </>
                    )}
                  </button>
                )}
              </div>
            )}
          </motion.div>

          {/* Footer - Debug Info */}
          <div className="p-4 border-t border-slate-700/50 bg-slate-900/50 rounded-b-2xl">
            <div className="flex justify-between items-center text-xs text-slate-500">
              <div className="flex items-center gap-1">
                <ShieldCheck className="w-3 h-3" />
                <span>Protected by Royal Security</span>
              </div>
              <div className="flex gap-2 opacity-50 hover:opacity-100 transition-opacity">
                <span>v2.2</span>
                <span title={BOT_API_URL}>{BOT_API_URL.includes('localhost') ? '⚠️ Local' : '✅ Prod'}</span>
              </div>
            </div>
          </div>

        </motion.div>
      )}
    </AnimatePresence>
  )
}
