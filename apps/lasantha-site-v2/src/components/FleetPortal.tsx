'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import { Building2, Truck, Phone, Mail, User, FileText, CheckCircle2, XCircle, Loader2, Briefcase } from 'lucide-react'

export default function FleetPortal() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  const [formData, setFormData] = useState({
    companyName: '',
    contactPerson: '',
    phone: '',
    email: '',
    fleetSize: '',
    vehicleTypes: '',
    message: ''
  })

  const [loading, setLoading] = useState(false)
  const [notification, setNotification] = useState<{
    type: 'success' | 'error'
    message: string
  } | null>(null)

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    })
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setNotification(null)

    try {
      // WhatsApp Message for B2B inquiry
      const message = `🏢 *FLEET / CORPORATE INQUIRY*
──────────────────
🏭 *Company:* ${formData.companyName}
👤 *Contact:* ${formData.contactPerson}
📱 *Phone:* ${formData.phone}
📧 *Email:* ${formData.email}
──────────────────
🚗 *Fleet Size:* ${formData.fleetSize} vehicles
🚙 *Vehicle Types:* ${formData.vehicleTypes}
──────────────────
📝 *Requirements:*
${formData.message || 'Not specified'}

_Sent via Fleet Portal - Lasantha Tyre Website_`

      const encodedMessage = encodeURIComponent(message)
      const whatsappUrl = `https://wa.me/94721222509?text=${encodedMessage}`
      
      window.open(whatsappUrl, '_blank')
      
      setNotification({
        type: 'success',
        message: '✅ Your fleet inquiry has been sent! Our B2B team will contact you shortly.'
      })
      
      // Reset form
      setFormData({
        companyName: '',
        contactPerson: '',
        phone: '',
        email: '',
        fleetSize: '',
        vehicleTypes: '',
        message: ''
      })
    } catch {
      setNotification({
        type: 'error',
        message: '❌ Something went wrong. Please try again or call us directly.'
      })
    } finally {
      setLoading(false)
    }
  }

  return (
    <section id="fleet" className="py-24 bg-gradient-to-br from-dark-950 via-dark-900 to-primary-900 text-white relative overflow-hidden">
      {/* Background Elements */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none opacity-20">
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-primary-600 rounded-full blur-[100px]"></div>
        <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-blue-600 rounded-full blur-[100px]"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <div className="inline-flex items-center justify-center p-3 bg-white/10 backdrop-blur-md rounded-2xl mb-6 ring-1 ring-white/20">
            <Building2 className="w-8 h-8 text-white" />
          </div>
          <h2 className="text-4xl md:text-5xl font-bold mb-6 tracking-tight">
            Fleet & <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-400 to-primary-200">Corporate Services</span>
          </h2>
          <p className="text-xl text-gray-300 max-w-2xl mx-auto leading-relaxed">
            Special wholesale rates, credit terms, and priority service for fleet operators and corporate clients
          </p>
        </motion.div>

        <div className="grid lg:grid-cols-5 gap-8 max-w-6xl mx-auto">
          {/* Benefits */}
          <motion.div
            initial={{ opacity: 0, x: -30 }}
            animate={isInView ? { opacity: 1, x: 0 } : { opacity: 0, x: -30 }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="lg:col-span-2 space-y-6"
          >
            <div className="bg-white/5 backdrop-blur-sm p-6 rounded-2xl border border-white/10 hover:bg-white/10 transition-all">
              <div className="w-12 h-12 bg-green-500/20 rounded-xl flex items-center justify-center mb-4">
                <Truck className="w-6 h-6 text-green-400" />
              </div>
              <h3 className="text-lg font-bold mb-2">Wholesale Pricing</h3>
              <p className="text-gray-300 text-sm">Volume discounts for fleets of 5+ vehicles. Save up to 25% on bulk orders.</p>
            </div>

            <div className="bg-white/5 backdrop-blur-sm p-6 rounded-2xl border border-white/10 hover:bg-white/10 transition-all">
              <div className="w-12 h-12 bg-blue-500/20 rounded-xl flex items-center justify-center mb-4">
                <FileText className="w-6 h-6 text-blue-400" />
              </div>
              <h3 className="text-lg font-bold mb-2">Credit Terms Available</h3>
              <p className="text-gray-300 text-sm">30-60 day payment terms for approved corporate accounts. Simplified invoicing.</p>
            </div>

            <div className="bg-white/5 backdrop-blur-sm p-6 rounded-2xl border border-white/10 hover:bg-white/10 transition-all">
              <div className="w-12 h-12 bg-purple-500/20 rounded-xl flex items-center justify-center mb-4">
                <Briefcase className="w-6 h-6 text-purple-400" />
              </div>
              <h3 className="text-lg font-bold mb-2">Dedicated Account Manager</h3>
              <p className="text-gray-300 text-sm">Single point of contact for all your fleet needs. Priority scheduling and support.</p>
            </div>
          </motion.div>

          {/* Form */}
          <motion.div
            initial={{ opacity: 0, x: 30 }}
            animate={isInView ? { opacity: 1, x: 0 } : { opacity: 0, x: 30 }}
            transition={{ duration: 0.6, delay: 0.4 }}
            className="lg:col-span-3"
          >
            <div className="bg-white rounded-3xl shadow-2xl p-8 md:p-10 relative overflow-hidden">
              <div className="absolute top-0 left-0 w-full h-2 bg-gradient-to-r from-primary-500 via-primary-600 to-primary-800"></div>

              {notification && (
                <motion.div
                  initial={{ opacity: 0, y: -20 }}
                  animate={{ opacity: 1, y: 0 }}
                  className={`mb-8 p-4 rounded-xl flex items-center gap-3 ${
                    notification.type === 'success'
                      ? 'bg-green-50 text-green-800 border border-green-200'
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

              <h3 className="text-2xl font-bold text-gray-900 mb-6">Request Fleet Quotation</h3>

              <form onSubmit={handleSubmit} className="space-y-6" suppressHydrationWarning>
                <div className="grid md:grid-cols-2 gap-6">
                  {/* Company Name */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Company Name *</label>
                    <div className="relative group">
                      <Building2 className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 group-focus-within:text-primary-600 transition-colors w-5 h-5" />
                      <input
                        type="text"
                        name="companyName"
                        required
                        value={formData.companyName}
                        onChange={handleChange}
                        className="w-full pl-12 pr-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none text-gray-900"
                        placeholder="ABC Logistics (Pvt) Ltd"
                        suppressHydrationWarning
                      />
                    </div>
                  </div>

                  {/* Contact Person */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Contact Person *</label>
                    <div className="relative group">
                      <User className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 group-focus-within:text-primary-600 transition-colors w-5 h-5" />
                      <input
                        type="text"
                        name="contactPerson"
                        required
                        value={formData.contactPerson}
                        onChange={handleChange}
                        className="w-full pl-12 pr-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none text-gray-900"
                        placeholder="Mr. John Silva"
                        suppressHydrationWarning
                      />
                    </div>
                  </div>

                  {/* Phone */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Phone Number *</label>
                    <div className="relative group">
                      <Phone className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 group-focus-within:text-primary-600 transition-colors w-5 h-5" />
                      <input
                        type="tel"
                        name="phone"
                        required
                        value={formData.phone}
                        onChange={handleChange}
                        className="w-full pl-12 pr-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none text-gray-900"
                        placeholder="077 123 4567"
                        suppressHydrationWarning
                      />
                    </div>
                  </div>

                  {/* Email */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Email Address *</label>
                    <div className="relative group">
                      <Mail className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 group-focus-within:text-primary-600 transition-colors w-5 h-5" />
                      <input
                        type="email"
                        name="email"
                        required
                        value={formData.email}
                        onChange={handleChange}
                        className="w-full pl-12 pr-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none text-gray-900"
                        placeholder="fleet@company.lk"
                        suppressHydrationWarning
                      />
                    </div>
                  </div>

                  {/* Fleet Size */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Fleet Size *</label>
                    <select
                      name="fleetSize"
                      required
                      value={formData.fleetSize}
                      onChange={handleChange}
                      className="w-full px-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none appearance-none text-gray-900"
                      suppressHydrationWarning
                    >
                      <option value="">Select fleet size</option>
                      <option value="5-10">5-10 vehicles</option>
                      <option value="11-25">11-25 vehicles</option>
                      <option value="26-50">26-50 vehicles</option>
                      <option value="51-100">51-100 vehicles</option>
                      <option value="100+">100+ vehicles</option>
                    </select>
                  </div>

                  {/* Vehicle Types */}
                  <div className="space-y-2">
                    <label className="text-sm font-semibold text-gray-700">Vehicle Types *</label>
                    <input
                      type="text"
                      name="vehicleTypes"
                      required
                      value={formData.vehicleTypes}
                      onChange={handleChange}
                      className="w-full px-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none text-gray-900"
                      placeholder="e.g., Vans, Trucks, Cars"
                      suppressHydrationWarning
                    />
                  </div>
                </div>

                {/* Message */}
                <div className="space-y-2">
                  <label className="text-sm font-semibold text-gray-700">Requirements (Optional)</label>
                  <textarea
                    name="message"
                    rows={4}
                    value={formData.message}
                    onChange={handleChange}
                    className="w-full px-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl focus:bg-white focus:ring-2 focus:ring-primary-100 focus:border-primary-500 transition-all outline-none resize-none text-gray-900"
                    placeholder="Specific tyre brands, sizes, or any special requirements..."
                    suppressHydrationWarning
                  />
                </div>

                {/* Submit Button */}
                <motion.button
                  type="submit"
                  disabled={loading}
                  whileHover={!loading ? { scale: 1.01, translateY: -2 } : {}}
                  whileTap={!loading ? { scale: 0.99 } : {}}
                  className="w-full bg-gradient-to-r from-primary-600 to-primary-800 hover:from-primary-700 hover:to-primary-900 text-white py-4 rounded-xl font-bold text-lg transition-all shadow-lg hover:shadow-primary-500/30 flex items-center justify-center gap-3 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {loading ? (
                    <>
                      <Loader2 className="w-5 h-5 animate-spin" />
                      Sending...
                    </>
                  ) : (
                    <>
                      <Phone className="w-5 h-5" />
                      Request Quotation via WhatsApp
                    </>
                  )}
                </motion.button>

                <p className="text-center text-xs text-gray-500">
                  Our B2B team will review your requirements and contact you within 24 hours with a custom quote.
                </p>
              </form>
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  )
}
