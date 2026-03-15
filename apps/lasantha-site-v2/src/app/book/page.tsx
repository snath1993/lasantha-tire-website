'use client'

import { useState, useEffect, Suspense } from 'react'
import { useSearchParams } from 'next/navigation'
import { motion, AnimatePresence } from 'framer-motion'
import {
  Calendar, User, Phone, CheckCircle2, Loader2,
  Clock, Car, ChevronRight, ArrowLeft, ShieldCheck, Package
} from 'lucide-react'
import { CalendarView } from '@/components/CalendarView'

// ─── Time Slots (7:00 AM – 8:00 PM, 30-min intervals) ─────────────
const generateTimeSlots = () => {
  const slots: { time: string; display: string; hour: number; minute: number }[] = []
  for (let hour = 7; hour <= 20; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
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

// ─── Services ──────────────────────────────────────────────────────
const services = [
  { id: 'tyre-install', name: 'Tyre Installation', icon: '🛞', time: '30-45 min' },
  { id: 'alignment', name: 'Wheel Alignment', icon: '⚙️', time: '45 min' },
  { id: 'balancing', name: 'Wheel Balancing', icon: '⚖️', time: '30 min' },
  { id: 'rotation', name: 'Tyre Rotation', icon: '🔄', time: '20 min' },
  { id: 'nitrogen', name: 'Nitrogen Air Filling', icon: '💨', time: '15 min' },
  { id: 'puncture', name: 'Puncture Repair', icon: '🔧', time: '20-30 min' },
  { id: 'suspension', name: 'Suspension Checkup', icon: '🔍', time: '30 min' },
]

// ─── Interfaces ────────────────────────────────────────────────────
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

// ─── Alignment / Suspension restriction window (7:30 AM – 5:30 PM) ─
const RESTRICTED_START = 7 * 60 + 30
const RESTRICTED_END = 17 * 60 + 30

// ─── BookingWizard ─────────────────────────────────────────────────
function BookingWizard() {
  const searchParams = useSearchParams()
  const refCode = searchParams.get('ref')

  // Wizard state
  const [step, setStep] = useState(1)
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    services: [] as string[],
    date: '',
    time: '',
    vehicleNo: '',
    message: '',
  })

  // Client-side "now" (avoids hydration mismatch)
  const [now, setNow] = useState<Date | null>(null)

  useEffect(() => {
    setNow(new Date())
    if (!formData.date) {
      const today = new Date()
      const localDate =
        today.getFullYear() +
        '-' +
        String(today.getMonth() + 1).padStart(2, '0') +
        '-' +
        String(today.getDate()).padStart(2, '0')
      setFormData((prev) => ({ ...prev, date: localDate }))
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const [loading, setLoading] = useState(false)
  const [loadingQuotation, setLoadingQuotation] = useState(false)
  const [quotation, setQuotation] = useState<Quotation | null>(null)
  const [bookingComplete, setBookingComplete] = useState(false)
  const [bookingRef, setBookingRef] = useState('')
  const [error, setError] = useState('')

  // Slot management
  const [bookedSlots, setBookedSlots] = useState<string[]>([])
  const [loadingSlots, setLoadingSlots] = useState(false)

  // Item selection (quotation flow)
  const [selectedItems, setSelectedItems] = useState<Record<string, number>>({})
  const [quantityModalOpen, setQuantityModalOpen] = useState(false)
  const [currentItem, setCurrentItem] = useState<QuotationItem | null>(null)
  const [customQty, setCustomQty] = useState('')

  // ── Data fetching ────────────────────────────────────────────────
  useEffect(() => {
    if (refCode) fetchQuotation(refCode)
  }, [refCode])

  useEffect(() => {
    if (formData.date) fetchBookedSlots(formData.date)
  }, [formData.date])

  const fetchBookedSlots = async (date: string) => {
    setLoadingSlots(true)
    try {
      const res = await fetch(`/api/appointments/availability?date=${date}`)
      if (res.ok) {
        const data = await res.json()
        if (data.ok) setBookedSlots(Array.isArray(data.bookedSlots) ? data.bookedSlots : [])
      }
    } catch (err) {
      console.error('Error fetching booked slots:', err)
    } finally {
      setLoadingSlots(false)
    }
  }

  const fetchQuotation = async (code: string) => {
    setLoadingQuotation(true)
    try {
      const res = await fetch(`/api/quotation/${code}`)
      if (res.ok) {
        const data = await res.json()
        if (data.ok && data.quotation) {
          setQuotation(data.quotation)
          setFormData((prev) => ({
            ...prev,
            name: data.quotation.CustomerName || '',
            phone: data.quotation.CustomerPhone || '',
            vehicleNo: data.quotation.VehicleNumber || '',
          }))
          setSelectedItems({})
          setStep(1)
        }
      }
    } catch (err) {
      console.error('Error fetching quotation:', err)
    } finally {
      setLoadingQuotation(false)
    }
  }

  // ── Helpers ──────────────────────────────────────────────────────
  const isAlignmentSelected = () => {
    if (formData.services.includes('Wheel Alignment')) return true
    if (quotation) {
      return quotation.Items.some(
        (item) => selectedItems[item.itemId] && item.description.toLowerCase().includes('alignment')
      )
    }
    return false
  }

  const isSuspensionSelected = () => {
    if (formData.services.includes('Suspension Checkup')) return true
    if (quotation) {
      return quotation.Items.some(
        (item) => selectedItems[item.itemId] && item.description.toLowerCase().includes('suspension')
      )
    }
    return false
  }

  const isRestricted = isAlignmentSelected() || isSuspensionSelected()

  const isSlotDisabled = (slot: { time: string; hour: number; minute: number }) => {
    if (bookedSlots.includes(slot.time)) return true

    if (formData.date && now) {
      const todayStr = now.toDateString()
      const selectedStr = new Date(formData.date).toDateString()
      if (todayStr === selectedStr) {
        const currentMin = now.getHours() * 60 + now.getMinutes()
        const slotMin = slot.hour * 60 + slot.minute
        const buffer = isAlignmentSelected() ? 90 : 0
        if (slotMin < currentMin + buffer) return true
      }
    }

    return false
  }

  // ── Item click (quotation) ───────────────────────────────────────
  const handleItemClick = (item: QuotationItem) => {
    if (selectedItems[item.itemId]) {
      const copy = { ...selectedItems }
      delete copy[item.itemId]
      setSelectedItems(copy)
      return
    }
    if (!item.isService) {
      setCurrentItem(item)
      setCustomQty('')
      setQuantityModalOpen(true)
    } else {
      setSelectedItems((prev) => ({ ...prev, [item.itemId]: 1 }))
    }
  }

  const confirmQuantity = (qty: number) => {
    if (currentItem) {
      setSelectedItems((prev) => ({ ...prev, [currentItem.itemId]: qty }))
      setQuantityModalOpen(false)
      setCurrentItem(null)
    }
  }

  // ── Navigation ───────────────────────────────────────────────────
  const handleNext = () => {
    if (step === 1) {
      if (quotation) {
        if (Object.keys(selectedItems).length === 0) {
          setError('Please select at least one item')
          return
        }
      } else if (formData.services.length === 0) {
        setError('Please select at least one service')
        return
      }
    }
    if (step === 2 && (!formData.date || !formData.time)) {
      setError('Please select date and time')
      return
    }
    setError('')
    setStep((s) => s + 1)
  }

  const handleBack = () => {
    setStep((s) => s - 1)
    setError('')
  }

  // ── Submit ───────────────────────────────────────────────────────
  const handleSubmit = async () => {
    if (!formData.name || !formData.phone || !formData.vehicleNo) {
      setError('Please fill in all required fields')
      return
    }
    setLoading(true)
    try {
      const quotationItems =
        quotation?.Items.filter((i) => selectedItems[i.itemId]).map((item) => ({
          ...item,
          quantity: selectedItems[item.itemId],
        })) || []

      const serviceType =
        formData.services.length > 0
          ? formData.services.join(', ')
          : quotation
            ? 'Quotation Booking'
            : ''

      const res = await fetch('/api/appointments', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          customerName: formData.name,
          phoneNumber: formData.phone,
          vehicleNumber: formData.vehicleNo,
          serviceType,
          appointmentDate: formData.date,
          timeSlot: formData.time,
          quotationRefCode: quotation?.RefCode || null,
          quotationItems: quotationItems.length > 0 ? quotationItems : null,
          notes: formData.message || null,
        }),
      })

      const data = await res.json()
      if (data.success) {
        setBookingComplete(true)
        setBookingRef(data.referenceNo || 'CONFIRMED')
      } else {
        setError(data.message || 'Booking failed. Please try again.')
      }
    } catch {
      setError('Failed to submit booking. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  // ── Computed ─────────────────────────────────────────────────────
  const selectedTotal = quotation
    ? quotation.Items.reduce((sum, item) => sum + item.price * (selectedItems[item.itemId] || 0), 0)
    : 0

  // ── Booking Complete Screen ──────────────────────────────────────
  if (bookingComplete) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <motion.div
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="bg-white rounded-3xl shadow-2xl p-8 text-center max-w-md w-full"
        >
          <div className="w-24 h-24 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
            <CheckCircle2 className="w-12 h-12 text-green-600" />
          </div>
          <h2 className="text-3xl font-bold text-gray-900 mb-3">Booking Confirmed!</h2>
          <p className="text-gray-600 mb-8 text-lg">
            Your appointment has been scheduled successfully.
          </p>
          <div className="bg-gray-50 rounded-2xl p-6 mb-6 border border-gray-100">
            <p className="text-sm text-gray-500 mb-2 uppercase tracking-wide font-semibold">Reference Number</p>
            <p className="text-3xl font-mono font-bold text-primary-600 tracking-wider">{bookingRef}</p>
          </div>
          <div className="space-y-2 text-sm text-gray-500 mb-8">
            <div className="flex items-center justify-center gap-2">
              <Calendar className="w-4 h-4 text-primary-500" />
              <span>{formData.date} at {formData.time}</span>
            </div>
            <div className="flex items-center justify-center gap-2">
              <Car className="w-4 h-4 text-primary-500" />
              <span>{formData.vehicleNo}</span>
            </div>
          </div>
          <p className="text-xs text-gray-400 mb-6">You will receive a WhatsApp confirmation shortly.</p>
          <button
            onClick={() => window.location.reload()}
            className="w-full bg-primary-600 hover:bg-primary-700 text-white px-8 py-4 rounded-xl font-bold text-lg transition-all shadow-lg active:scale-95"
          >
            Book Another
          </button>
        </motion.div>
      </div>
    )
  }

  // ── Loading Quotation Screen ─────────────────────────────────────
  if (loadingQuotation) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center space-y-4">
          <Loader2 className="w-12 h-12 text-primary-500 animate-spin mx-auto" />
          <p className="text-gray-500 animate-pulse">Loading your quotation...</p>
        </div>
      </div>
    )
  }

  // ── Main Wizard ──────────────────────────────────────────────────
  return (
    <div className="min-h-screen bg-gray-50 font-sans">
      {/* Header */}
      <div className="bg-white border-b border-gray-100 shadow-sm">
        <div className="max-w-2xl mx-auto px-4 py-5">
          <h1 className="text-2xl font-bold text-gray-900">
            {quotation ? '👑 Royal Booking' : 'Book a Service'}
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            {quotation
              ? `Quotation ${quotation.QuotationNumber || quotation.RefCode}`
              : 'Schedule your appointment at Lasantha Tyre Traders'}
          </p>
        </div>
      </div>

      {/* Progress Bar */}
      <div className="bg-white border-b border-gray-100">
        <div className="max-w-2xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-2">
            {[1, 2, 3].map((s) => (
              <div key={s} className="flex items-center flex-1 last:flex-none">
                <div
                  className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold transition-all duration-300 shadow-sm ${
                    step >= s
                      ? 'bg-primary-600 text-white scale-110 ring-2 ring-primary-200'
                      : 'bg-gray-100 text-gray-400'
                  }`}
                >
                  {s}
                </div>
                {s < 3 && (
                  <div className="flex-1 mx-2 h-1 bg-gray-100 rounded-full overflow-hidden">
                    <div
                      className={`h-full bg-primary-600 transition-all duration-500 ease-out ${step > s ? 'w-full' : 'w-0'}`}
                    />
                  </div>
                )}
              </div>
            ))}
          </div>
          <div className="flex justify-between text-xs font-semibold text-gray-500 px-1">
            <span className={step >= 1 ? 'text-primary-600' : ''}>{quotation ? 'Review' : 'Service'}</span>
            <span className={step >= 2 ? 'text-primary-600' : ''}>Date & Time</span>
            <span className={step >= 3 ? 'text-primary-600' : ''}>Details</span>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-2xl mx-auto px-4 py-6 pb-32">
        <AnimatePresence mode="wait">
          {/* ─── Step 1: Service / Quotation Review ─────────────── */}
          {step === 1 && (
            <motion.div
              key="step1"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-4"
            >
              {quotation ? (
                <>
                  <h2 className="text-2xl font-bold text-gray-900 mb-2">Review Quotation</h2>
                  <p className="text-gray-500 mb-6">Select the items you want to proceed with.</p>

                  {/* Quotation Items */}
                  <div className="bg-yellow-50/80 border border-yellow-200 rounded-2xl p-5 backdrop-blur-sm">
                    <div className="flex items-start justify-between mb-4">
                      <div>
                        <h3 className="font-bold text-yellow-900 flex items-center gap-2 text-lg">
                          <Package className="w-5 h-5" />
                          Quotation Details
                        </h3>
                        <p className="text-xs text-yellow-700 mt-1 font-medium">Ref: {quotation.RefCode}</p>
                      </div>
                      {selectedTotal > 0 && (
                        <div className="text-right">
                          <p className="text-xl font-bold text-yellow-900">LKR {selectedTotal.toLocaleString()}</p>
                          <p className="text-xs text-yellow-700 font-medium">Total Selected</p>
                        </div>
                      )}
                    </div>

                    <div className="space-y-3">
                      {quotation.Items.map((item, idx) => {
                        const isSelected = !!selectedItems[item.itemId]
                        const qty = selectedItems[item.itemId] || 0
                        const isAlignmentItem = item.description.toLowerCase().includes('alignment')

                        return (
                          <div key={idx}>
                            <button
                              type="button"
                              onClick={() => handleItemClick(item)}
                              className={`w-full text-left transition-all rounded-xl p-4 text-sm flex justify-between items-center border-2 shadow-sm active:scale-[0.98] ${
                                isSelected
                                  ? 'bg-white border-yellow-500 ring-1 ring-yellow-500/20 shadow-md'
                                  : 'bg-white border-gray-200 hover:border-yellow-300 hover:shadow-md'
                              }`}
                            >
                              <div className="flex items-center gap-4">
                                <div
                                  className={`w-6 h-6 rounded-full border-2 flex items-center justify-center transition-colors flex-shrink-0 ${
                                    isSelected ? 'border-yellow-500 bg-yellow-500 text-white' : 'border-gray-300 bg-gray-50'
                                  }`}
                                >
                                  {isSelected && <CheckCircle2 className="w-3.5 h-3.5" />}
                                </div>
                                <div>
                                  <p className={`font-bold text-base ${isSelected ? 'text-gray-900' : 'text-gray-700'}`}>
                                    {item.description}
                                  </p>
                                  {isSelected && qty > 0 && (
                                    <span className="text-xs font-bold text-yellow-700 bg-yellow-100 px-2 py-0.5 rounded-md mt-1 inline-block">
                                      Qty: {qty}
                                    </span>
                                  )}
                                </div>
                              </div>
                              <p className={`font-bold whitespace-nowrap text-base ${isSelected ? 'text-yellow-700' : 'text-gray-600'}`}>
                                LKR {(item.price * (qty || item.quantity)).toLocaleString()}
                              </p>
                            </button>
                            {isSelected && isAlignmentItem && (
                              <div className="mt-2 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-600 font-medium flex items-start gap-2">
                                <Clock className="w-4 h-4 flex-shrink-0 mt-0.5" />
                                <span>Wheel Alignment available only <strong>7:30 AM – 5:30 PM</strong>.</span>
                              </div>
                            )}
                          </div>
                        )
                      })}
                    </div>
                  </div>
                </>
              ) : (
                <>
                  <h2 className="text-2xl font-bold text-gray-900 mb-6">Select Service</h2>
                  <div className="grid grid-cols-1 gap-3">
                    {services.map((s) => (
                      <button
                        key={s.id}
                        type="button"
                        onClick={() => {
                          const newServices = formData.services.includes(s.name)
                            ? formData.services.filter((n) => n !== s.name)
                            : [...formData.services, s.name]
                          setFormData({ ...formData, services: newServices })
                        }}
                        className={`
                          relative p-5 rounded-2xl border-2 text-left transition-all duration-200 active:scale-[0.98]
                          ${formData.services.includes(s.name)
                            ? 'border-primary-500 bg-primary-50 shadow-md ring-1 ring-primary-200'
                            : 'border-white bg-white shadow-sm hover:border-primary-200 hover:shadow-md'
                          }
                        `}
                      >
                        <div className="flex items-center gap-4">
                          <div
                            className={`w-12 h-12 rounded-xl flex items-center justify-center text-2xl transition-colors ${
                              formData.services.includes(s.name) ? 'bg-primary-100' : 'bg-gray-50'
                            }`}
                          >
                            {s.icon}
                          </div>
                          <div>
                            <div className="font-bold text-gray-900 text-lg">{s.name}</div>
                            <div className="text-sm text-gray-500 mt-1 flex items-center gap-1.5 font-medium">
                              <Clock className="w-3.5 h-3.5" /> {s.time}
                            </div>
                          </div>
                          {formData.services.includes(s.name) && (
                            <div className="absolute top-5 right-5 text-primary-600">
                              <CheckCircle2 className="w-6 h-6" />
                            </div>
                          )}
                        </div>
                      </button>
                    ))}
                  </div>

                  {formData.services.includes('Wheel Alignment') && (
                    <div className="mt-4 px-4 py-3 bg-red-50 border border-red-100 rounded-xl text-sm text-red-600 font-medium flex items-start gap-2">
                      <Clock className="w-5 h-5 flex-shrink-0 mt-0.5" />
                      <span>Wheel Alignment available only between <strong>7:30 AM – 5:30 PM</strong>.</span>
                    </div>
                  )}
                  {formData.services.includes('Suspension Checkup') && (
                    <div className="mt-3 px-4 py-3 bg-red-50 border border-red-100 rounded-xl text-sm text-red-600 font-medium flex items-start gap-2">
                      <Clock className="w-5 h-5 flex-shrink-0 mt-0.5" />
                      <span>Suspension Checkup available only between <strong>7:30 AM – 5:30 PM</strong>.</span>
                    </div>
                  )}
                </>
              )}
            </motion.div>
          )}

          {/* ─── Step 2: Date & Time ────────────────────────────── */}
          {step === 2 && (
            <motion.div
              key="step2"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-6"
            >
              <h2 className="text-2xl font-bold text-gray-900">Select Date & Time</h2>

              {/* Calendar */}
              <div className="bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
                <label className="block text-sm font-bold text-gray-700 mb-3 flex items-center gap-2">
                  <Calendar className="w-4 h-4 text-primary-600" /> Select Date
                </label>
                <CalendarView
                  selectedDate={formData.date}
                  onSelectDate={(date) => setFormData({ ...formData, date, time: '' })}
                />
              </div>

              {/* Time Slots */}
              <div className="bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
                <label className="block text-sm font-bold text-gray-700 mb-3 flex items-center gap-2">
                  <Clock className="w-4 h-4 text-primary-600" /> Select Time Slot
                </label>

                {isRestricted && (
                  <div className="mb-3 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-600 font-medium flex items-start gap-2">
                    <Clock className="w-4 h-4 flex-shrink-0 mt-0.5" />
                    <span>
                      {isAlignmentSelected() ? 'Alignment' : 'Suspension'} available: <strong>7:30 AM – 5:30 PM</strong>
                    </span>
                  </div>
                )}

                <div className="grid grid-cols-3 sm:grid-cols-4 gap-2 max-h-[40vh] overflow-y-auto pr-1">
                  {loadingSlots ? (
                    <div className="col-span-3 sm:col-span-4 py-12 text-center text-gray-500 flex flex-col items-center gap-3">
                      <Loader2 className="w-8 h-8 animate-spin text-primary-400" />
                      <span className="text-sm font-medium">Checking availability...</span>
                    </div>
                  ) : (
                    allTimeSlots
                      .filter((slot) => {
                        if (!isRestricted) return true
                        const m = slot.hour * 60 + slot.minute
                        return m >= RESTRICTED_START && m <= RESTRICTED_END
                      })
                      .map((slot) => {
                        const disabled = isSlotDisabled(slot)
                        const isBooked = bookedSlots.includes(slot.time)

                        return (
                          <button
                            key={slot.time}
                            type="button"
                            onClick={() => setFormData({ ...formData, time: slot.time })}
                            disabled={disabled}
                            className={`
                              py-3 px-2 text-sm font-medium rounded-xl border transition-all relative active:scale-95
                              ${formData.time === slot.time
                                ? 'bg-primary-600 text-white border-primary-600 shadow-md'
                                : disabled
                                  ? 'bg-gray-50 text-gray-300 border-gray-100 cursor-not-allowed'
                                  : 'bg-white border-gray-200 text-gray-700 hover:border-primary-300 hover:bg-gray-50'
                              }
                            `}
                          >
                            {slot.display}
                            {isBooked && (
                              <span className="absolute inset-0 flex items-center justify-center bg-red-50/90 backdrop-blur-[1px] rounded-xl border border-red-100">
                                <span className="text-[10px] font-bold text-red-600 uppercase tracking-wider">Reserved</span>
                              </span>
                            )}
                          </button>
                        )
                      })
                  )}
                </div>
              </div>

              {/* Special Requests */}
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">Special Requests (Optional)</label>
                <textarea
                  value={formData.message}
                  onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                  placeholder="Any specific requirements or notes?"
                  rows={2}
                  className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-primary-200 focus:border-primary-500 outline-none transition-all resize-none bg-white"
                />
              </div>
            </motion.div>
          )}

          {/* ─── Step 3: Confirm & Customer Details ─────────────── */}
          {step === 3 && (
            <motion.div
              key="step3"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-6"
            >
              <h2 className="text-2xl font-bold text-gray-900">Confirm Booking</h2>

              {/* Summary */}
              <div className="bg-white border border-gray-200 rounded-2xl overflow-hidden shadow-sm">
                <div className="p-4 bg-gray-50/80 border-b border-gray-100">
                  <div className="flex items-center gap-2">
                    <Package className="w-5 h-5 text-primary-600" />
                    <span className="font-bold text-gray-900">Booking Summary</span>
                  </div>
                </div>

                <div className="p-5 space-y-4 text-sm">
                  <div className="flex justify-between items-start">
                    <span className="text-gray-500 font-medium">Service</span>
                    <div className="text-right">
                      {formData.services.map((s, i) => (
                        <div key={i} className="font-bold text-gray-900">{s}</div>
                      ))}
                      {quotation && !formData.services.length && (
                        <div className="font-bold text-gray-900">Quotation Booking</div>
                      )}
                    </div>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-gray-500 font-medium">Date & Time</span>
                    <span className="font-bold text-gray-900 bg-primary-50 text-primary-700 px-2 py-1 rounded-lg">
                      {formData.date} @ {formData.time}
                    </span>
                  </div>
                  {formData.message && (
                    <div className="flex justify-between">
                      <span className="text-gray-500 font-medium">Note</span>
                      <span className="font-medium text-gray-900 text-right max-w-[200px] truncate" title={formData.message}>
                        {formData.message}
                      </span>
                    </div>
                  )}

                  {/* Quotation Items Summary */}
                  {quotation && Object.keys(selectedItems).length > 0 && (
                    <div className="pt-4 mt-2 border-t border-dashed border-gray-200">
                      <p className="text-xs font-bold text-gray-500 mb-3 uppercase tracking-wide">Selected Items</p>
                      <div className="space-y-2">
                        {quotation.Items.filter((i) => selectedItems[i.itemId]).map((item, idx) => (
                          <div key={idx} className="flex justify-between text-xs">
                            <span className="text-gray-700 font-medium">
                              {item.brand} {item.description}{' '}
                              <span className="text-gray-400">x{selectedItems[item.itemId]}</span>
                            </span>
                            <span className="font-bold text-gray-900">
                              LKR {(item.price * selectedItems[item.itemId]).toLocaleString()}
                            </span>
                          </div>
                        ))}
                      </div>
                      <div className="flex justify-between mt-3 pt-3 border-t border-gray-100 font-bold text-lg text-primary-600">
                        <span>Total</span>
                        <span>LKR {selectedTotal.toLocaleString()}</span>
                      </div>

                      <div className="mt-4 p-3 bg-blue-50 border border-blue-100 rounded-xl text-[11px] text-blue-800 leading-relaxed">
                        <p className="font-bold mb-1 flex items-center gap-1.5">
                          <ShieldCheck className="w-3.5 h-3.5" /> Note:
                        </p>
                        The total above covers your selected products. Installation, alignment, and additional service charges
                        will be calculated separately based on the services performed.
                      </div>
                    </div>
                  )}
                </div>
              </div>

              {/* Customer Details Form */}
              <div className="space-y-5">
                <p className="text-sm text-gray-500 font-bold uppercase tracking-wide">Enter your details</p>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Full Name *</label>
                  <div className="relative group">
                    <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary-600 transition-colors" />
                    <input
                      type="text"
                      value={formData.name}
                      onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                      onFocus={() => {
                        if (['Customer', 'Cash Customer', 'Walk-in Customer'].includes(formData.name)) {
                          setFormData({ ...formData, name: '' })
                        }
                      }}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary-200 focus:border-primary-500 outline-none transition-all font-medium"
                      placeholder="John Doe"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Phone Number *</label>
                  <div className="relative group">
                    <Phone className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary-600 transition-colors" />
                    <input
                      type="tel"
                      value={formData.phone}
                      onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary-200 focus:border-primary-500 outline-none transition-all font-medium"
                      placeholder="077 123 4567"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Vehicle Number *</label>
                  <div className="relative group">
                    <Car className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary-600 transition-colors" />
                    <input
                      type="text"
                      value={formData.vehicleNo}
                      onChange={(e) => {
                        const value = e.target.value.toUpperCase().replace(/[^A-Z0-9- ]/g, '')
                        setFormData({ ...formData, vehicleNo: value })
                      }}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary-200 focus:border-primary-500 outline-none transition-all font-medium"
                      placeholder="CAB-1234"
                    />
                  </div>
                </div>
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      </div>

      {/* Quantity Modal */}
      {quantityModalOpen && currentItem && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <motion.div
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className="bg-white rounded-2xl p-6 max-w-sm w-full shadow-xl"
          >
            <h3 className="text-lg font-bold text-gray-900 mb-2">Select Quantity</h3>
            <p className="text-sm text-gray-600 mb-6">
              How many <strong>{currentItem.brand} {currentItem.description}</strong> do you need?
            </p>

            <div className="grid grid-cols-4 gap-2 mb-4">
              {[1, 2, 3, 4].map((num) => (
                <button
                  key={num}
                  type="button"
                  onClick={() => confirmQuantity(num)}
                  className="aspect-square rounded-xl border-2 border-gray-100 hover:border-primary-500 hover:bg-primary-50 text-lg font-bold text-gray-700 hover:text-primary-600 transition-all"
                >
                  {num}
                </button>
              ))}
            </div>

            <div className="mb-6">
              <label className="block text-xs font-medium text-gray-500 mb-1">Custom Quantity</label>
              <div className="flex gap-2">
                <input
                  type="number"
                  value={customQty}
                  onChange={(e) => setCustomQty(e.target.value)}
                  className="flex-1 px-3 py-2 rounded-lg border border-gray-200 focus:ring-2 focus:ring-primary-200 focus:border-primary-500 outline-none"
                  placeholder="Enter amount..."
                />
                <button
                  type="button"
                  onClick={() => customQty && confirmQuantity(parseInt(customQty))}
                  disabled={!customQty}
                  className="bg-gray-900 text-white px-4 rounded-lg font-medium disabled:opacity-50"
                >
                  Set
                </button>
              </div>
            </div>

            <button
              type="button"
              onClick={() => setQuantityModalOpen(false)}
              className="w-full py-3 text-gray-500 font-medium hover:bg-gray-50 rounded-xl transition-colors"
            >
              Cancel
            </button>
          </motion.div>
        </div>
      )}

      {/* Sticky Footer Actions */}
      <div className="fixed bottom-0 left-0 right-0 p-4 bg-white/90 backdrop-blur-xl border-t border-gray-200 z-30">
        <div className="max-w-2xl mx-auto flex justify-between items-center gap-4">
          {step > 1 ? (
            <button
              type="button"
              onClick={handleBack}
              className="flex items-center justify-center gap-2 text-gray-600 hover:text-gray-900 font-bold px-6 py-3.5 rounded-xl hover:bg-gray-100 transition-colors active:scale-95"
            >
              <ArrowLeft className="w-5 h-5" /> <span className="hidden sm:inline">Back</span>
            </button>
          ) : (
            <div />
          )}

          {error && (
            <div className="absolute -top-12 left-0 right-0 text-center">
              <span className="bg-red-500 text-white text-sm font-medium px-4 py-2 rounded-full shadow-lg">
                {error}
              </span>
            </div>
          )}

          <button
            type="button"
            onClick={step === 3 ? handleSubmit : handleNext}
            disabled={loading}
            className={`bg-primary-600 hover:bg-primary-700 text-white px-8 py-3.5 rounded-xl font-bold text-lg transition-all flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg active:scale-95 ${
              step === 1 ? 'w-full sm:w-auto' : 'flex-1 sm:flex-none sm:min-w-[200px]'
            }`}
          >
            {loading ? (
              <Loader2 className="w-6 h-6 animate-spin" />
            ) : step === 3 ? (
              'Confirm Booking'
            ) : (
              <>
                Next <ChevronRight className="w-5 h-5" />
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  )
}

// ─── Page Export ────────────────────────────────────────────────────
export default function BookingPage() {
  return (
    <Suspense
      fallback={
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <Loader2 className="w-12 h-12 text-primary-500 animate-spin" />
        </div>
      }
    >
      <BookingWizard />
    </Suspense>
  )
}
