'use client';

import { useState } from 'react';
import { X, Calendar, Clock, MapPin, Phone, User, Car, ChevronRight, Check, Wrench, Disc, MessageSquare } from 'lucide-react';
import { useBooking } from '@/contexts/BookingContext';

const SERVICE_IDS = ['120', '121', '161', '144', '122', '114'];

const SERVICES = [
  { id: 'tire-fitting', name: 'Tire Fitting', icon: Disc, duration: '30 min', color: 'blue' },
  { id: 'wheel-alignment', name: 'Wheel Alignment', icon: Wrench, duration: '45 min', color: 'indigo' },
  { id: 'balancing', name: 'Wheel Balancing', icon: Disc, duration: '30 min', color: 'purple' },
  { id: 'nitrogen', name: 'Nitrogen Filling', icon: Wrench, duration: '15 min', color: 'cyan' },
];

const TIME_SLOTS = [
  '07:00 AM', '08:00 AM', '09:00 AM', '10:00 AM', '11:00 AM',
  '12:00 PM', '01:00 PM', '02:00 PM', '03:00 PM', '04:00 PM',
  '05:00 PM', '06:00 PM', '07:00 PM', '08:00 PM'
];

export default function SmartBookingPopup() {
  const { 
    isBookingPopupOpen, 
    closeBookingPopup, 
    bookingItems, 
    bookingRefCode,
    customerInfo,
    setCustomerInfo
  } = useBooking();

  const [step, setStep] = useState(1); // 1: Info, 2: Service, 3: Date/Time, 4: Confirm
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTime, setSelectedTime] = useState('');
  const [selectedServices, setSelectedServices] = useState<string[]>([]);
  const [notes, setNotes] = useState('');
  const [bookingSuccess, setBookingSuccess] = useState(false);

  if (!isBookingPopupOpen) return null;

  const getPrice = (item: any) => item.Price || item.SellingPrice || 0;
  
  const total = bookingItems.reduce((sum, item) => sum + (getPrice(item) * item.Quantity), 0);

  const tyreItems = bookingItems.filter(i => !SERVICE_IDS.includes(i.ItemId));
  const serviceItems = bookingItems.filter(i => SERVICE_IDS.includes(i.ItemId));

  const handleSubmitBooking = async () => {
    try {
      const botApiUrl = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';
      
      const response = await fetch(`${botApiUrl}/api/appointments/book`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          customerName: customerInfo.name,
          customerPhone: customerInfo.phone,
          vehicleNumber: customerInfo.vehicleNumber,
          serviceType: selectedServices.join(', '),
          appointmentDate: selectedDate,
          appointmentTime: selectedTime,
          notes: notes,
          refCode: bookingRefCode,
          items: bookingItems.map(item => ({
            description: item.Description,
            brand: item.Brand,
            quantity: item.Quantity,
            price: getPrice(item)
          }))
        })
      });

      const data = await response.json();

      if (response.ok && data.ok) {
        setBookingSuccess(true);
        setTimeout(() => {
          closeBookingPopup();
          setBookingSuccess(false);
          setStep(1);
        }, 3000);
      } else {
        alert('Booking failed. Please try again or call us directly.');
      }
    } catch (error) {
      console.error('Booking error:', error);
      alert('Booking failed. Please try again or call us directly.');
    }
  };

  const nextStep = () => {
    if (step === 1 && (!customerInfo.name || !customerInfo.phone)) {
      alert('Please fill in your name and phone number');
      return;
    }
    if (step === 2 && selectedServices.length === 0) {
      alert('Please select at least one service');
      return;
    }
    if (step === 3 && (!selectedDate || !selectedTime)) {
      alert('Please select date and time');
      return;
    }
    if (step === 4) {
      handleSubmitBooking();
      return;
    }
    setStep(step + 1);
  };

  const prevStep = () => setStep(Math.max(1, step - 1));

  // Generate next 14 days for date selection
  const getNextDays = (count: number) => {
    const days = [];
    const today = new Date();
    for (let i = 0; i < count; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + i);
      days.push(date);
    }
    return days;
  };

  const dates = getNextDays(14);

  if (bookingSuccess) {
    return (
      <div className="fixed inset-0 bg-gradient-to-br from-green-600 to-emerald-700 z-[9999] flex items-center justify-center p-4">
        <div className="text-center text-white">
          <div className="w-24 h-24 bg-white rounded-full flex items-center justify-center mx-auto mb-6 animate-bounce">
            <Check className="w-16 h-16 text-green-600" />
          </div>
          <h2 className="text-3xl font-bold mb-4">Booking Confirmed! 🎉</h2>
          <p className="text-xl opacity-90">We'll send you a WhatsApp confirmation shortly</p>
        </div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 bg-black z-[9999] overflow-hidden">
      {/* Header */}
      <div className="absolute top-0 left-0 right-0 bg-gradient-to-br from-blue-600 to-indigo-700 p-6 pb-24">
        <div className="flex items-center justify-between mb-6">
          <button 
            onClick={closeBookingPopup}
            className="w-10 h-10 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center text-white hover:bg-white/30 transition-colors"
          >
            <X className="w-6 h-6" />
          </button>
          <div className="text-white text-sm font-medium">
            Step {step} of 4
          </div>
        </div>

        <h1 className="text-3xl font-bold text-white mb-2">
          {step === 1 && 'Your Information'}
          {step === 2 && 'Select Services'}
          {step === 3 && 'Pick Date & Time'}
          {step === 4 && 'Confirm Booking'}
        </h1>
        <p className="text-blue-100 text-sm">
          {step === 1 && 'Let us know who you are'}
          {step === 2 && 'Choose the services you need'}
          {step === 3 && 'When would you like to visit?'}
          {step === 4 && 'Review and confirm your booking'}
        </p>

        {/* Progress Bar */}
        <div className="flex gap-2 mt-6">
          {[1, 2, 3, 4].map((s) => (
            <div
              key={s}
              className={`h-1 flex-1 rounded-full transition-all ${
                s <= step ? 'bg-white' : 'bg-white/30'
              }`}
            />
          ))}
        </div>
      </div>

      {/* Content */}
      <div className="absolute top-44 left-0 right-0 bottom-24 overflow-y-auto bg-slate-900 rounded-t-3xl p-6 shadow-2xl">
        
        {/* Step 1: Customer Info */}
        {step === 1 && (
          <div className="space-y-4">
            <div>
              <label className="text-slate-400 text-sm font-medium mb-2 block flex items-center gap-2">
                <User className="w-4 h-4" />
                Full Name *
              </label>
              <input
                type="text"
                value={customerInfo.name}
                onChange={(e) => setCustomerInfo({ ...customerInfo, name: e.target.value })}
                placeholder="Enter your name"
                className="w-full bg-slate-800 border border-slate-700 rounded-xl px-4 py-4 text-white focus:border-blue-500 outline-none text-lg"
              />
            </div>

            <div>
              <label className="text-slate-400 text-sm font-medium mb-2 block flex items-center gap-2">
                <Phone className="w-4 h-4" />
                Phone Number *
              </label>
              <input
                type="tel"
                value={customerInfo.phone}
                onChange={(e) => setCustomerInfo({ ...customerInfo, phone: e.target.value })}
                placeholder="07X XXX XXXX"
                className="w-full bg-slate-800 border border-slate-700 rounded-xl px-4 py-4 text-white focus:border-blue-500 outline-none text-lg"
              />
            </div>

            <div>
              <label className="text-slate-400 text-sm font-medium mb-2 block flex items-center gap-2">
                <Car className="w-4 h-4" />
                Vehicle Number (Optional)
              </label>
              <input
                type="text"
                value={customerInfo.vehicleNumber}
                onChange={(e) => setCustomerInfo({ ...customerInfo, vehicleNumber: e.target.value })}
                placeholder="ABC-1234"
                className="w-full bg-slate-800 border border-slate-700 rounded-xl px-4 py-4 text-white focus:border-blue-500 outline-none text-lg"
              />
            </div>

            {/* Order Summary */}
            {tyreItems.length > 0 && (
              <div className="mt-8 bg-slate-800 rounded-2xl p-5 border border-slate-700">
                <h3 className="text-white font-bold mb-4 flex items-center gap-2">
                  <Disc className="w-5 h-5 text-blue-400" />
                  Your Items
                </h3>
                <div className="space-y-3">
                  {tyreItems.map((item, idx) => (
                    <div key={idx} className="flex justify-between items-start">
                      <div className="flex-1">
                        <div className="text-white font-medium text-sm">{item.Brand}</div>
                        <div className="text-slate-400 text-xs">{item.Description}</div>
                      </div>
                      <div className="text-right">
                        <div className="text-white font-bold">Rs {(getPrice(item) * item.Quantity).toLocaleString()}</div>
                        <div className="text-slate-400 text-xs">Qty: {item.Quantity}</div>
                      </div>
                    </div>
                  ))}
                </div>
                <div className="mt-4 pt-4 border-t border-slate-700 flex justify-between items-center">
                  <span className="text-slate-400 font-medium">Total</span>
                  <span className="text-white font-bold text-xl">Rs {total.toLocaleString()}</span>
                </div>
              </div>
            )}
          </div>
        )}

        {/* Step 2: Select Services */}
        {step === 2 && (
          <div className="space-y-4">
            {SERVICES.map((service) => {
              const isSelected = selectedServices.includes(service.id);
              const Icon = service.icon;
              return (
                <button
                  key={service.id}
                  onClick={() => {
                    if (isSelected) {
                      setSelectedServices(selectedServices.filter(s => s !== service.id));
                    } else {
                      setSelectedServices([...selectedServices, service.id]);
                    }
                  }}
                  className={`w-full bg-slate-800 border-2 rounded-2xl p-5 transition-all ${
                    isSelected 
                      ? `border-${service.color}-500 bg-${service.color}-500/10` 
                      : 'border-slate-700 hover:border-slate-600'
                  }`}
                >
                  <div className="flex items-center gap-4">
                    <div className={`w-14 h-14 rounded-xl bg-${service.color}-500/20 flex items-center justify-center`}>
                      <Icon className={`w-7 h-7 text-${service.color}-400`} />
                    </div>
                    <div className="flex-1 text-left">
                      <div className="text-white font-bold text-lg">{service.name}</div>
                      <div className="text-slate-400 text-sm flex items-center gap-2 mt-1">
                        <Clock className="w-4 h-4" />
                        {service.duration}
                      </div>
                    </div>
                    <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center transition-all ${
                      isSelected 
                        ? `border-${service.color}-500 bg-${service.color}-500` 
                        : 'border-slate-600'
                    }`}>
                      {isSelected && <Check className="w-5 h-5 text-white" />}
                    </div>
                  </div>
                </button>
              );
            })}
          </div>
        )}

        {/* Step 3: Date & Time */}
        {step === 3 && (
          <div className="space-y-6">
            {/* Date Selection */}
            <div>
              <label className="text-slate-400 text-sm font-medium mb-3 block flex items-center gap-2">
                <Calendar className="w-4 h-4" />
                Select Date
              </label>
              <div className="grid grid-cols-3 gap-3">
                {dates.map((date) => {
                  const dateStr = date.toISOString().split('T')[0];
                  const isSelected = selectedDate === dateStr;
                  const isToday = date.toDateString() === new Date().toDateString();
                  return (
                    <button
                      key={dateStr}
                      onClick={() => setSelectedDate(dateStr)}
                      className={`p-4 rounded-xl border-2 transition-all ${
                        isSelected
                          ? 'border-blue-500 bg-blue-500/20'
                          : 'border-slate-700 bg-slate-800 hover:border-slate-600'
                      }`}
                    >
                      <div className="text-center">
                        <div className="text-white font-bold text-lg">{date.getDate()}</div>
                        <div className="text-slate-400 text-xs">
                          {date.toLocaleDateString('en-US', { weekday: 'short' })}
                        </div>
                        {isToday && <div className="text-blue-400 text-[10px] font-bold mt-1">TODAY</div>}
                      </div>
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Time Selection */}
            <div>
              <label className="text-slate-400 text-sm font-medium mb-3 block flex items-center gap-2">
                <Clock className="w-4 h-4" />
                Select Time
              </label>
              <div className="grid grid-cols-3 gap-3">
                {TIME_SLOTS.map((time) => {
                  const isSelected = selectedTime === time;
                  return (
                    <button
                      key={time}
                      onClick={() => setSelectedTime(time)}
                      className={`p-3 rounded-xl border-2 transition-all ${
                        isSelected
                          ? 'border-blue-500 bg-blue-500/20'
                          : 'border-slate-700 bg-slate-800 hover:border-slate-600'
                      }`}
                    >
                      <div className="text-white font-bold text-sm">{time}</div>
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Notes */}
            <div>
              <label className="text-slate-400 text-sm font-medium mb-2 block flex items-center gap-2">
                <MessageSquare className="w-4 h-4" />
                Additional Notes (Optional)
              </label>
              <textarea
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                placeholder="Any special requests or information..."
                rows={3}
                className="w-full bg-slate-800 border border-slate-700 rounded-xl px-4 py-3 text-white focus:border-blue-500 outline-none resize-none"
              />
            </div>
          </div>
        )}

        {/* Step 4: Confirmation */}
        {step === 4 && (
          <div className="space-y-6">
            {/* Customer Info Summary */}
            <div className="bg-slate-800 rounded-2xl p-5 border border-slate-700">
              <h3 className="text-white font-bold mb-4 flex items-center gap-2">
                <User className="w-5 h-5 text-blue-400" />
                Your Information
              </h3>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-slate-400">Name:</span>
                  <span className="text-white font-medium">{customerInfo.name}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-slate-400">Phone:</span>
                  <span className="text-white font-medium">{customerInfo.phone}</span>
                </div>
                {customerInfo.vehicleNumber && (
                  <div className="flex justify-between">
                    <span className="text-slate-400">Vehicle:</span>
                    <span className="text-white font-medium">{customerInfo.vehicleNumber}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Services Summary */}
            <div className="bg-slate-800 rounded-2xl p-5 border border-slate-700">
              <h3 className="text-white font-bold mb-4 flex items-center gap-2">
                <Wrench className="w-5 h-5 text-indigo-400" />
                Selected Services
              </h3>
              <div className="space-y-2">
                {selectedServices.map((serviceId) => {
                  const service = SERVICES.find(s => s.id === serviceId);
                  return (
                    <div key={serviceId} className="flex items-center gap-3">
                      <Check className="w-4 h-4 text-green-400" />
                      <span className="text-white text-sm">{service?.name}</span>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Appointment Summary */}
            <div className="bg-slate-800 rounded-2xl p-5 border border-slate-700">
              <h3 className="text-white font-bold mb-4 flex items-center gap-2">
                <Calendar className="w-5 h-5 text-purple-400" />
                Appointment Details
              </h3>
              <div className="space-y-3">
                <div className="flex items-center gap-3">
                  <Calendar className="w-5 h-5 text-slate-400" />
                  <span className="text-white">
                    {new Date(selectedDate).toLocaleDateString('en-US', { 
                      weekday: 'long', 
                      year: 'numeric', 
                      month: 'long', 
                      day: 'numeric' 
                    })}
                  </span>
                </div>
                <div className="flex items-center gap-3">
                  <Clock className="w-5 h-5 text-slate-400" />
                  <span className="text-white">{selectedTime}</span>
                </div>
                <div className="flex items-center gap-3">
                  <MapPin className="w-5 h-5 text-slate-400" />
                  <span className="text-white text-sm">Lasantha Tyre Traders, Thalawathugoda</span>
                </div>
              </div>
            </div>

            {notes && (
              <div className="bg-slate-800 rounded-2xl p-5 border border-slate-700">
                <h3 className="text-white font-bold mb-3 flex items-center gap-2">
                  <MessageSquare className="w-5 h-5 text-cyan-400" />
                  Notes
                </h3>
                <p className="text-slate-300 text-sm">{notes}</p>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Footer Navigation */}
      <div className="absolute bottom-0 left-0 right-0 bg-slate-900 border-t border-slate-800 p-6 shadow-2xl">
        <div className="flex gap-3">
          {step > 1 && (
            <button
              onClick={prevStep}
              className="px-6 py-4 bg-slate-800 text-white rounded-xl font-bold transition-all hover:bg-slate-700"
            >
              Back
            </button>
          )}
          <button
            onClick={nextStep}
            className="flex-1 px-6 py-4 bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-xl font-bold transition-all hover:scale-[1.02] active:scale-[0.98] shadow-lg shadow-blue-500/20 flex items-center justify-center gap-2"
          >
            {step === 4 ? 'Confirm Booking' : 'Continue'}
            <ChevronRight className="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
  );
}
