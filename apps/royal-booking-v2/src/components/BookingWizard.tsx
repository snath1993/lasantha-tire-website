'use client'

import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Calendar, User, Phone, CheckCircle2, Loader2, 
  Clock, Car, ChevronRight, ArrowLeft, ShieldCheck, Package
} from 'lucide-react'
import { CalendarView } from './CalendarView'
import { useSearchParams } from 'next/navigation'

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

const services = [
  { id: 'tyre-install', name: 'Tyre Installation', icon: '🛞', time: '30-45 min' },
  { id: 'alignment', name: 'Wheel Alignment', icon: '⚙️', time: '45 min' },
  { id: 'balancing', name: 'Wheel Balancing', icon: '⚖️', time: '30 min' },
  { id: 'rotation', name: 'Tyre Rotation', icon: '🔄', time: '20 min' },
  { id: 'nitrogen', name: 'Nitrogen Air Filling', icon: '💨', time: '15 min' },
  { id: 'puncture', name: 'Puncture Repair', icon: '🔧', time: '20-30 min' },
  { id: 'suspension', name: 'Suspension Checkup', icon: '🔍', time: '30 min' }
]

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

export function BookingWizard() {
  const searchParams = useSearchParams()
  const refCode = searchParams.get('ref')

  const [step, setStep] = useState(1)
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    services: [] as string[],
    date: '', // Initialize empty to avoid hydration mismatch
    time: '',
    vehicleNo: '',
    message: ''
  })

  // State to handle client-side time checks
  const [now, setNow] = useState<Date | null>(null)

  useEffect(() => {
    // Set initial date and time on client side
    setNow(new Date())
    
    if (!formData.date) {
      // Get local date in YYYY-MM-DD format
      const today = new Date()
      const localDate = today.getFullYear() + '-' + 
        String(today.getMonth() + 1).padStart(2, '0') + '-' + 
        String(today.getDate()).padStart(2, '0')
      
      setFormData(prev => ({ ...prev, date: localDate }))
    }
  }, [])

  const [loading, setLoading] = useState(false)
  const [loadingQuotation, setLoadingQuotation] = useState(false)
  const [quotation, setQuotation] = useState<Quotation | null>(null)
  const [bookingComplete, setBookingComplete] = useState(false)
  const [bookingRef, setBookingRef] = useState('')
  const [error, setError] = useState('')

  // Slot Management
  const [bookedSlots, setBookedSlots] = useState<string[]>([])
  const [loadingSlots, setLoadingSlots] = useState(false)

  // Item Selection State
  const [selectedItems, setSelectedItems] = useState<Record<string, number>>({})
  const [quantityModalOpen, setQuantityModalOpen] = useState(false)
  const [currentItem, setCurrentItem] = useState<QuotationItem | null>(null)
  const [customQty, setCustomQty] = useState('')

  useEffect(() => {
    if (refCode) {
      fetchQuotation(refCode)
    }
  }, [refCode])

  useEffect(() => {
    if (formData.date) {
      fetchBookedSlots(formData.date)
    }
  }, [formData.date])

  const fetchBookedSlots = async (date: string) => {
    setLoadingSlots(true)
    try {
      // Compatibility endpoint (same shape used by the current production booking UI)
      const res = await fetch(`/api/appointments/availability?date=${date}`)
      if (res.ok) {
        const data = await res.json()
        if (data.ok) {
          setBookedSlots(Array.isArray(data.bookedSlots) ? data.bookedSlots : [])
        }
      }
    } catch (err) {
      console.error('Error fetching booked slots:', err)
    } finally {
      setLoadingSlots(false)
    }
  }

  // Business rule (keep unchanged): alignment+suspension can only be booked 07:30–17:30.
  // Minutes from midnight: 07:30 -> 450, 17:30 -> 1050
  const RESTRICTED_START_MINUTES = 7 * 60 + 30
  const RESTRICTED_END_MINUTES = 17 * 60 + 30

  const isSlotDisabled = (slot: { time: string, hour: number, minute: number }) => {
    // 1. Check if already booked
    if (bookedSlots.includes(slot.time)) return true

    // 2. Check if past time (only for today)
    // Only check if we have 'now' (client-side) to avoid hydration mismatch
    if (formData.date && now) {
      const today = now
      const selectedDate = new Date(formData.date)
      
      // Reset hours to compare dates only
      const todayStr = today.toDateString()
      const selectedStr = selectedDate.toDateString()

      if (todayStr === selectedStr) {
        const currentHour = today.getHours()
        const currentMinute = today.getMinutes()
        
        const currentTotalMinutes = currentHour * 60 + currentMinute
        const slotTotalMinutes = slot.hour * 60 + slot.minute
        
        // Business rule (keep unchanged): for same-day bookings,
        // require a 90-minute lead time only when alignment is selected.
        const bufferMinutes = isAlignmentSelected() ? 90 : 0
        
        if (slotTotalMinutes < currentTotalMinutes + bufferMinutes) {
          return true
        }
      }
    }
    
    return false
  }

  const fetchQuotation = async (code: string) => {
    setLoadingQuotation(true)
    try {
      const res = await fetch(`/api/quotation/${code}`)
      if (res.ok) {
        const data = await res.json()
        if (data.ok && data.quotation) {
          setQuotation(data.quotation)
          setFormData(prev => ({
            ...prev,
            name: data.quotation.CustomerName || '',
            phone: data.quotation.CustomerPhone || '',
            vehicleNo: data.quotation.VehicleNumber || '',
            service: 'Quotation Fulfillment'
          }))
          
          // Initialize selected items (start empty as requested)
          setSelectedItems({})

          // Start at step 1 (Review) if we have quotation
          setStep(1)
        }
      }
    } catch (err) {
      console.error('Error fetching quotation:', err)
    } finally {
      setLoadingQuotation(false)
    }
  }

  const handleItemClick = (item: QuotationItem) => {
    // If already selected, deselect
    if (selectedItems[item.itemId]) {
      const newSelection = { ...selectedItems }
      delete newSelection[item.itemId]
      setSelectedItems(newSelection)
      return
    }

    // If it's a Tyre (not a service), open quantity modal
    if (!item.isService) {
      setCurrentItem(item)
      setCustomQty('')
      setQuantityModalOpen(true)
    } else {
      // For services, just select with default quantity 1
      setSelectedItems(prev => ({ ...prev, [item.itemId]: 1 }))
    }
  }

  const confirmQuantity = (qty: number) => {
    if (currentItem) {
      setSelectedItems(prev => ({ ...prev, [currentItem.itemId]: qty }))
      setQuantityModalOpen(false)
      setCurrentItem(null)
    }
  }

  const QuantityModal = () => {
    if (!quantityModalOpen || !currentItem) return null

    return (
      <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
        <motion.div 
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="bg-white rounded-2xl p-6 max-w-sm w-full shadow-xl"
        >
          <h3 className="text-lg font-bold text-gray-900 mb-2">Select Quantity</h3>
          <p className="text-sm text-gray-600 mb-6">
            How many {currentItem.brand} {currentItem.description} do you need?
          </p>

          <div className="grid grid-cols-4 gap-2 mb-4">
            {[1, 2, 3, 4].map(num => (
              <button
                key={num}
                onClick={() => confirmQuantity(num)}
                className="aspect-square rounded-xl border-2 border-gray-100 hover:border-primary hover:bg-primary/5 text-lg font-bold text-gray-700 hover:text-primary transition-all"
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
                className="flex-1 px-3 py-2 rounded-lg border border-gray-200 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none"
                placeholder="Enter amount..."
              />
              <button
                onClick={() => customQty && confirmQuantity(parseInt(customQty))}
                disabled={!customQty}
                className="bg-gray-900 text-white px-4 rounded-lg font-medium disabled:opacity-50"
              >
                Set
              </button>
            </div>
          </div>

          <button
            onClick={() => setQuantityModalOpen(false)}
            className="w-full py-3 text-gray-500 font-medium hover:bg-gray-50 rounded-xl transition-colors"
          >
            Cancel
          </button>
        </motion.div>
      </div>
    )
  }

  const isAlignmentSelected = () => {
    if (formData.services.includes('Wheel Alignment')) return true
    if (quotation) {
      return quotation.Items.some(item => 
        selectedItems[item.itemId] && 
        item.description.toLowerCase().includes('alignment')
      )
    }
    return false
  }

  const isSuspensionSelected = () => {
    if (formData.services.includes('Suspension Checkup')) return true
    if (quotation) {
      return quotation.Items.some(item =>
        selectedItems[item.itemId] &&
        item.description.toLowerCase().includes('suspension')
      )
    }
    return false
  }

  const QuotationSummary = () => {
    if (!quotation) return null
    
    const selectedTotal = quotation.Items.reduce((sum, item) => {
      const qty = selectedItems[item.itemId] || 0
      return sum + (item.price * qty)
    }, 0)

    const hasAlignment = isAlignmentSelected()

    return (
      <div className="bg-yellow-50/80 border border-yellow-200 rounded-2xl p-5 mb-6 backdrop-blur-sm">
        <div className="flex items-start justify-between mb-4">
          <div>
            <h3 className="font-bold text-yellow-900 flex items-center gap-2 text-lg">
              <Package className="w-5 h-5" />
              Quotation Details
            </h3>
            <p className="text-xs text-yellow-700 mt-1 font-medium">Ref: {quotation.RefCode}</p>
          </div>
          <div className="text-right">
            <p className="text-xl font-bold text-yellow-900">
              LKR {selectedTotal.toLocaleString()}
            </p>
            <p className="text-xs text-yellow-700 font-medium">Total Selected</p>
          </div>
        </div>
        
        <div className="space-y-3">
          {quotation.Items.map((item, idx) => {
            const isSelected = !!selectedItems[item.itemId]
            const qty = selectedItems[item.itemId] || 0
            const isAlignmentItem = item.description.toLowerCase().includes('alignment')
            
            return (
              <div key={idx}>
                <button
                  onClick={() => handleItemClick(item)}
                  className={`w-full text-left transition-all rounded-xl p-4 text-sm flex justify-between items-center border-2 shadow-sm active:scale-[0.98] ${
                    isSelected 
                      ? 'bg-white border-yellow-500 ring-1 ring-yellow-500/20 shadow-md' 
                      : 'bg-white border-gray-200 hover:border-yellow-300 hover:shadow-md'
                  }`}
                >
                  <div className="flex items-center gap-4">
                    <div className={`w-6 h-6 rounded-full border-2 flex items-center justify-center transition-colors flex-shrink-0 ${
                      isSelected ? 'border-yellow-500 bg-yellow-500 text-white' : 'border-gray-300 bg-gray-50'
                    }`}>
                      {isSelected && <CheckCircle2 className="w-3.5 h-3.5" />}
                    </div>
                    <div>
                      <p className={`font-bold text-base ${isSelected ? 'text-gray-900' : 'text-gray-700'}`}>
                        {item.description}
                      </p>
                      <p className="text-xs text-gray-500 mt-1">
                        {isSelected && <span className="font-bold text-yellow-700 bg-yellow-100 px-2 py-0.5 rounded-md">Qty: {qty}</span>}
                      </p>
                    </div>
                  </div>
                  <p className={`font-bold whitespace-nowrap text-base ${isSelected ? 'text-yellow-700' : 'text-gray-600'}`}>
                    LKR {(item.price * (qty || item.quantity)).toLocaleString()}
                  </p>
                </button>
                {isSelected && isAlignmentItem && (
                  <div className="mt-2 mb-1 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-600 font-medium flex items-start gap-2 animate-in fade-in slide-in-from-top-1">
                    <Clock className="w-4 h-4 flex-shrink-0 mt-0.5" />
                    <span>
                      Wheel Alignment is available only between <strong>7:30 AM - 5:30 PM</strong>.
                    </span>
                  </div>
                )}
              </div>
            )
          })}
        </div>
      </div>
    )
  }

  const handleNext = () => {
    if (step === 1) {
      if (quotation) {
        // Check if at least one item is selected
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
    setStep(prev => prev + 1)
  }

  const handleBack = () => {
    setStep(prev => prev - 1)
    setError('')
  }

  const handleSubmit = async () => {
    if (!formData.name || !formData.phone || !formData.vehicleNo) {
      setError('Please fill in all required fields')
      return
    }

    setLoading(true)
    try {
      // Prepare quotation items array for the backend
      const quotationItems = quotation?.Items
        .filter(item => selectedItems[item.itemId])
        .map(item => ({
          ...item,
          quantity: selectedItems[item.itemId],
          // Ensure these fields match what backend expects
          description: item.description,
          brand: item.brand,
          price: item.price
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
          date: formData.date,
          timeSlot: formData.time,
          quotationRef: quotation?.RefCode,
          quotationItems: quotationItems // Send formatted items array
        })
      })

      const data = await res.json()
      if (data.ok) {
        setBookingComplete(true)
        setBookingRef(data.referenceNo || 'PENDING')
      } else {
        setError(data.error || 'Booking failed')
      }
    } catch (err) {
      setError('Failed to submit booking')
    } finally {
      setLoading(false)
    }
  }

  if (bookingComplete) {
    return (
      <div className="bg-white rounded-3xl shadow-2xl p-8 text-center max-w-md mx-auto mt-10 animate-in fade-in zoom-in duration-300">
        <div className="w-24 h-24 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6 shadow-inner">
          <CheckCircle2 className="w-12 h-12 text-green-600" />
        </div>
        <h2 className="text-3xl font-bold text-gray-900 mb-3">Booking Confirmed!</h2>
        <p className="text-gray-600 mb-8 text-lg">
          Your appointment has been scheduled successfully.
        </p>
        <div className="bg-gray-50 rounded-2xl p-6 mb-8 border border-gray-100">
          <p className="text-sm text-gray-500 mb-2 uppercase tracking-wide font-semibold">Reference Number</p>
          <p className="text-3xl font-mono font-bold text-primary-600 tracking-wider">{bookingRef}</p>
        </div>
        <button
          onClick={() => window.location.reload()}
          className="w-full bg-primary text-primary-foreground px-8 py-4 rounded-xl font-bold text-lg hover:bg-primary/90 transition-all shadow-lg shadow-primary/25 active:scale-95"
        >
          Book Another
        </button>
      </div>
    )
  }

  return (
    <div className="bg-gray-50 sm:bg-white sm:rounded-3xl sm:shadow-2xl overflow-hidden min-h-screen sm:min-h-[700px] flex flex-col relative font-sans">
      <QuantityModal />
      
      {/* Progress Bar - Sticky Header */}
      <div className="sticky top-0 z-20 bg-white/90 backdrop-blur-md border-b border-gray-100 shadow-sm">
        <div className="px-4 py-4">
          <div className="flex items-center justify-between max-w-md mx-auto mb-2">
            {[1, 2, 3].map((s) => (
              <div key={s} className="flex items-center flex-1 last:flex-none">
                <div className={`
                  w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold transition-all duration-300 shadow-sm
                  ${step >= s 
                    ? 'bg-primary text-primary-foreground scale-110 ring-2 ring-primary/20' 
                    : 'bg-gray-100 text-gray-400'
                  }
                `}>
                  {s}
                </div>
                {s < 3 && (
                  <div className="flex-1 mx-2 h-1 bg-gray-100 rounded-full overflow-hidden">
                    <div 
                      className={`h-full bg-primary transition-all duration-500 ease-out ${step > s ? 'w-full' : 'w-0'}`} 
                    />
                  </div>
                )}
              </div>
            ))}
          </div>
          <div className="flex justify-between max-w-md mx-auto text-xs font-semibold text-gray-500 px-1">
            <span className={step >= 1 ? 'text-primary' : ''}>{quotation ? 'Review' : 'Service'}</span>
            <span className={step >= 2 ? 'text-primary' : ''}>Time</span>
            <span className={step >= 3 ? 'text-primary' : ''}>Details</span>
          </div>
        </div>
      </div>

      <div className="flex-1 p-4 sm:p-8 overflow-y-auto pb-32 sm:pb-32 custom-scrollbar">
        <AnimatePresence mode="wait">
          {step === 1 && (
            <motion.div
              key="step1"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-4 max-w-lg mx-auto"
            >
              {quotation ? (
                <>
                  <h2 className="text-2xl font-bold text-gray-900 mb-2">Review Quotation</h2>
                  <p className="text-gray-500 mb-6">Select the items you want to proceed with.</p>
                  <QuotationSummary />
                </>
              ) : (
                <>
                  <h2 className="text-2xl font-bold text-gray-900 mb-6">Select Service</h2>
                  <div className="grid grid-cols-1 gap-4">
                    {services.map((s) => (
                      <button
                        key={s.id}
                        onClick={() => {
                          const newServices = formData.services.includes(s.name)
                            ? formData.services.filter(name => name !== s.name)
                            : [...formData.services, s.name]
                          setFormData({ ...formData, services: newServices })
                        }}
                        className={`
                          relative p-5 rounded-2xl border-2 text-left transition-all duration-200 active:scale-[0.98]
                          ${formData.services.includes(s.name)
                            ? 'border-primary bg-primary/5 shadow-md shadow-primary/10 ring-1 ring-primary/20' 
                            : 'border-white bg-white shadow-sm hover:border-primary/30 hover:shadow-md'
                          }
                        `}
                      >
                        <div className="flex items-center gap-4">
                          <div className={`
                            w-12 h-12 rounded-xl flex items-center justify-center text-2xl transition-colors
                            ${formData.services.includes(s.name) ? 'bg-primary/10' : 'bg-gray-50'}
                          `}>
                            {s.icon}
                          </div>
                          <div>
                            <div className="font-bold text-gray-900 text-lg">{s.name}</div>
                            <div className="text-sm text-gray-500 mt-1 flex items-center gap-1.5 font-medium">
                              <Clock className="w-3.5 h-3.5" /> {s.time}
                            </div>
                          </div>
                          {formData.services.includes(s.name) && (
                            <div className="absolute top-5 right-5 text-primary">
                              <CheckCircle2 className="w-6 h-6 fill-primary/10" />
                            </div>
                          )}
                        </div>
                      </button>
                    ))}
                  </div>
                  {formData.services.includes('Wheel Alignment') && (
                    <div className="mt-4 px-4 py-3 bg-red-50 border border-red-100 rounded-xl text-sm text-red-600 font-medium flex items-start gap-2 animate-in fade-in slide-in-from-top-2">
                      <Clock className="w-5 h-5 flex-shrink-0 mt-0.5" />
                      <span>
                        Note: Wheel Alignment services are only available between <strong>7:30 AM - 5:30 PM</strong>.
                      </span>
                    </div>
                  )}
                  {formData.services.includes('Suspension Checkup') && (
                    <div className="mt-3 px-4 py-3 bg-red-50 border border-red-100 rounded-xl text-sm text-red-600 font-medium flex items-start gap-2 animate-in fade-in slide-in-from-top-2">
                      <Clock className="w-5 h-5 flex-shrink-0 mt-0.5" />
                      <span>
                        Note: Suspension Checkup is only available between <strong>7:30 AM - 5:30 PM</strong>.
                      </span>
                    </div>
                  )}
                </>
              )}
            </motion.div>
          )}

          {step === 2 && (
            <motion.div
              key="step2"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-6 max-w-lg mx-auto"
            >
              <h2 className="text-2xl font-bold text-gray-900">Select Date & Time</h2>
              
              {!quotation && <QuotationSummary />}

              <div className="space-y-6">
                <div className="bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
                  <label className="block text-sm font-bold text-gray-700 mb-3 flex items-center gap-2">
                    <Calendar className="w-4 h-4 text-primary" /> Select Date
                  </label>
                  <CalendarView 
                    selectedDate={formData.date}
                    onSelectDate={(date) => setFormData({ ...formData, date })}
                  />
                </div>
                
                <div className="bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
                  <label className="block text-sm font-bold text-gray-700 mb-3 flex items-center gap-2">
                    <Clock className="w-4 h-4 text-primary" /> Select Time Slot
                  </label>
                  
                  {isAlignmentSelected() && (
                    <div className="mb-3 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-600 font-medium flex items-start gap-2">
                      <Clock className="w-4 h-4 flex-shrink-0 mt-0.5" />
                      <span>
                        Alignment available: <strong>7:30 AM - 5:30 PM</strong>
                      </span>
                    </div>
                  )}

                  {!isAlignmentSelected() && isSuspensionSelected() && (
                    <div className="mb-3 px-3 py-2 bg-red-50 border border-red-100 rounded-lg text-xs text-red-600 font-medium flex items-start gap-2">
                      <Clock className="w-4 h-4 flex-shrink-0 mt-0.5" />
                      <span>
                        Suspension available: <strong>7:30 AM - 5:30 PM</strong>
                      </span>
                    </div>
                  )}
                  
                  <div className="grid grid-cols-3 sm:grid-cols-4 gap-2 max-h-[40vh] overflow-y-auto pr-1 custom-scrollbar">
                    {loadingSlots ? (
                      <div className="col-span-3 sm:col-span-4 py-12 text-center text-gray-500 flex flex-col items-center gap-3">
                        <Loader2 className="w-8 h-8 animate-spin text-primary/50" />
                        <span className="text-sm font-medium">Checking availability...</span>
                      </div>
                    ) : (
                      allTimeSlots
                        .filter(slot => {
                          const restricted = isAlignmentSelected() || isSuspensionSelected()
                          if (!restricted) return true
                          const slotTotalMinutes = slot.hour * 60 + slot.minute
                          return (
                            slotTotalMinutes >= RESTRICTED_START_MINUTES &&
                            slotTotalMinutes <= RESTRICTED_END_MINUTES
                          )
                        })
                        .map((slot) => {
                        const disabled = isSlotDisabled(slot)
                        const isFullyBooked = bookedSlots.includes(slot.time)
                        
                        return (
                          <button
                            key={slot.time}
                            onClick={() => setFormData({ ...formData, time: slot.time })}
                            disabled={disabled}
                            className={`
                              py-3 px-2 text-sm font-medium rounded-xl border transition-all relative active:scale-95
                              ${formData.time === slot.time
                                ? 'bg-primary text-primary-foreground border-primary shadow-md shadow-primary/20'
                                : disabled
                                  ? 'bg-gray-50 text-gray-300 border-gray-100 cursor-not-allowed'
                                  : 'bg-white border-gray-200 text-gray-700 hover:border-primary/50 hover:bg-gray-50'
                              }
                            `}
                          >
                            {slot.display}
                            {isFullyBooked && (
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
              </div>

              {/* Special Requests (Notes) */}
              <div className="mt-6">
                <label className="block text-sm font-bold text-gray-700 mb-2">Special Requests (Optional)</label>
                <textarea
                  value={formData.message}
                  onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                  placeholder="Any specific requirements or notes?"
                  rows={2}
                  className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all resize-none bg-white"
                />
              </div>
            </motion.div>
          )}

          {step === 3 && (
            <motion.div
              key="step3"
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              className="space-y-6 max-w-lg mx-auto"
            >
              <h2 className="text-2xl font-bold text-gray-900">Confirm Booking</h2>
              
              {/* Summary Card (Legacy Style) */}
              <div className="bg-white border border-gray-200 rounded-2xl overflow-hidden shadow-sm">
                <div className="p-4 bg-gray-50/80 border-b border-gray-100 backdrop-blur-sm">
                  <div className="flex items-center gap-2">
                    <Package className="w-5 h-5 text-primary" />
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
                    </div>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-gray-500 font-medium">Date & Time</span>
                    <span className="font-bold text-gray-900 text-right bg-primary/10 text-primary px-2 py-1 rounded-lg">
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
                  {quotation && (
                    <div className="pt-4 mt-2 border-t border-dashed border-gray-200">
                      <p className="text-xs font-bold text-gray-500 mb-3 uppercase tracking-wide">Selected Items</p>
                      <div className="space-y-2">
                        {quotation.Items.filter(i => selectedItems[i.itemId]).map((item, idx) => (
                          <div key={idx} className="flex justify-between text-xs">
                            <span className="text-gray-700 font-medium">{item.brand} {item.description} <span className="text-gray-400">x{selectedItems[item.itemId]}</span></span>
                            <span className="font-bold text-gray-900">LKR {(item.price * selectedItems[item.itemId]).toLocaleString()}</span>
                          </div>
                        ))}
                      </div>
                      <div className="flex justify-between mt-3 pt-3 border-t border-gray-100 font-bold text-lg text-primary">
                        <span>Total</span>
                        <span>LKR {quotation.Items.reduce((sum, item) => sum + (item.price * (selectedItems[item.itemId] || 0)), 0).toLocaleString()}</span>
                      </div>
                      
                      {/* Professional Disclaimer Note */}
                      <div className="mt-4 p-3 bg-blue-50 border border-blue-100 rounded-xl text-[11px] text-blue-800 leading-relaxed">
                        <p className="font-bold mb-1 flex items-center gap-1.5">
                          <ShieldCheck className="w-3.5 h-3.5" /> Note:
                        </p>
                        The total above covers your selected products. Installation, alignment, and additional technical service charges will be calculated separately based on the services performed.
                      </div>
                    </div>
                  )}
                </div>
              </div>

              <div className="space-y-5">
                <p className="text-sm text-gray-500 font-bold uppercase tracking-wide">Enter your details</p>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Full Name</label>
                  <div className="relative group">
                    <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary transition-colors" />
                    <input
                      type="text"
                      value={formData.name}
                      onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                      onFocus={() => {
                        if (['Customer', 'Cash Customer', 'Walk-in Customer'].includes(formData.name)) {
                          setFormData({ ...formData, name: '' })
                        }
                      }}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all font-medium"
                      placeholder="John Doe"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Phone Number</label>
                  <div className="relative group">
                    <Phone className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary transition-colors" />
                    <input
                      type="tel"
                      value={formData.phone}
                      onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all font-medium"
                      placeholder="077 123 4567"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Vehicle Number</label>
                  <div className="relative group">
                    <Car className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 group-focus-within:text-primary transition-colors" />
                    <input
                      type="text"
                      value={formData.vehicleNo}
                      onChange={(e) => {
                        const value = e.target.value
                          .toUpperCase()
                          .replace(/[^A-Z0-9- ]/g, '')
                        setFormData({ ...formData, vehicleNo: value })
                      }}
                      className="w-full pl-12 pr-4 py-3.5 rounded-xl border border-gray-200 bg-white focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all font-medium"
                      placeholder="CAB-1234"
                    />
                  </div>
                </div>
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      </div>

      {/* Footer Actions - Sticky Bottom */}
      <div className="fixed bottom-0 left-0 right-0 p-4 bg-white/80 backdrop-blur-xl border-t border-gray-200 z-30 sm:absolute sm:bg-gray-50 sm:backdrop-blur-none">
        <div className="max-w-4xl mx-auto flex justify-between items-center gap-4">
          {step > 1 ? (
            <button
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
              <span className="bg-red-500 text-white text-sm font-medium px-4 py-2 rounded-full shadow-lg animate-in slide-in-from-bottom-2 fade-in">
                {error}
              </span>
            </div>
          )}

          <button
            onClick={step === 3 ? handleSubmit : handleNext}
            disabled={loading}
            className={`
              bg-primary text-primary-foreground px-8 py-3.5 rounded-xl font-bold text-lg hover:bg-primary/90 transition-all flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-primary/25 active:scale-95
              ${step === 1 ? 'w-full sm:w-auto' : 'flex-1 sm:flex-none sm:min-w-[200px]'}
            `}
          >
            {loading ? (
              <Loader2 className="w-6 h-6 animate-spin" />
            ) : step === 3 ? (
              'Confirm Booking'
            ) : (
              <>Next <ChevronRight className="w-5 h-5" /></>
            )}
          </button>
        </div>
      </div>
    </div>
  )
}
