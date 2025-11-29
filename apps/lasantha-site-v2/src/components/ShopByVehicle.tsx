'use client'

import { motion, AnimatePresence } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState, useEffect } from 'react'
import { Car, Search, ArrowRight, CheckCircle2, ChevronRight, RotateCcw, AlertCircle } from 'lucide-react'
import { vehicleDatabase } from '@/lib/vehicleData'
import QuoteModal from './QuoteModal'

export default function ShopByVehicle() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  const [step, setStep] = useState(1)
  const [selectedMake, setSelectedMake] = useState('')
  const [selectedModel, setSelectedModel] = useState('')
  const [selectedYear, setSelectedYear] = useState('')
  const [result, setResult] = useState<{ size: string, category: string } | null>(null)
  const [loading, setLoading] = useState(false)
  const [isQuoteModalOpen, setIsQuoteModalOpen] = useState(false)

  const makes = Object.keys(vehicleDatabase).sort()
  const models = selectedMake ? Object.keys(vehicleDatabase[selectedMake]).sort() : []
  const years = selectedMake && selectedModel 
    ? vehicleDatabase[selectedMake][selectedModel].years 
    : []

  // Reset subsequent selections when a parent selection changes
  useEffect(() => {
    if (step === 1) {
      setSelectedModel('')
      setSelectedYear('')
      setResult(null)
    } else if (step === 2) {
      setSelectedYear('')
      setResult(null)
    }
  }, [step, selectedMake, selectedModel])

  const handleSearch = async () => {
    setLoading(true)
    // Simulate database lookup delay
    await new Promise(resolve => setTimeout(resolve, 800))
    
    if (selectedMake && selectedModel && selectedYear) {
      const vehicleData = vehicleDatabase[selectedMake]?.[selectedModel]
      if (vehicleData) {
        setResult({ 
          size: vehicleData.size,
          category: vehicleData.category
        })
        setStep(4) // Move to result step
      }
    }
    setLoading(false)
  }

  const handleReset = () => {
    setStep(1)
    setSelectedMake('')
    setSelectedModel('')
    setSelectedYear('')
    setResult(null)
  }

  const handleGetQuote = () => {
    setIsQuoteModalOpen(true)
  }

  return (
    <section id="shop-by-vehicle" className="py-16 md:py-24 bg-gradient-to-b from-white to-gray-50 relative overflow-hidden">
      {/* Background Elements */}
      <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
        <div className="absolute -top-[20%] -right-[10%] w-[300px] md:w-[600px] h-[300px] md:h-[600px] bg-primary-50 rounded-full blur-[60px] md:blur-[100px] opacity-60"></div>
        <div className="absolute top-[40%] -left-[10%] w-[250px] md:w-[500px] h-[250px] md:h-[500px] bg-blue-50 rounded-full blur-[60px] md:blur-[100px] opacity-60"></div>
      </div>

      <div className="container mx-auto px-4 relative z-10">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-10 md:mb-16"
        >
          <span className="text-primary-600 font-bold tracking-wider uppercase text-xs md:text-sm mb-3 block">
            Smart Tyre Finder
          </span>
          <h2 className="text-3xl md:text-5xl font-bold text-gray-900 mb-4 md:mb-6">
            Shop by <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-600 to-primary-800">Vehicle</span>
          </h2>
          <p className="text-lg md:text-xl text-gray-600 max-w-2xl mx-auto">
            Our advanced database matches your vehicle with the perfect tyre size. 
            Select your vehicle details below.
          </p>
        </motion.div>

        <div className="max-w-5xl mx-auto">
          {/* Progress Steps */}
          <div className="flex justify-center mb-12">
            <div className="flex items-center gap-4">
              {[1, 2, 3].map((s) => (
                <div key={s} className="flex items-center">
                  <div 
                    className={`w-10 h-10 rounded-full flex items-center justify-center font-bold transition-all duration-300 ${
                      step >= s 
                        ? 'bg-primary-600 text-white shadow-lg shadow-primary-200' 
                        : 'bg-gray-200 text-gray-500'
                    }`}
                  >
                    {step > s ? <CheckCircle2 className="w-6 h-6" /> : s}
                  </div>
                  {s < 3 && (
                    <div className={`w-12 h-1 rounded-full mx-2 transition-all duration-300 ${
                      step > s ? 'bg-primary-600' : 'bg-gray-200'
                    }`} />
                  )}
                </div>
              ))}
            </div>
          </div>

          <motion.div
            layout
            className="bg-white rounded-3xl shadow-xl border border-gray-100 overflow-hidden min-h-[400px] relative"
          >
            <AnimatePresence mode="wait">
              {step === 1 && (
                <motion.div
                  key="step1"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="p-8 md:p-12"
                >
                  <h3 className="text-2xl font-bold text-gray-900 mb-8 text-center">Select Your Vehicle Make</h3>
                  <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                    {makes.map((make) => (
                      <button
                        key={make}
                        onClick={() => {
                          setSelectedMake(make)
                          setTimeout(() => setStep(2), 200)
                        }}
                        className={`p-6 rounded-2xl border-2 transition-all hover:shadow-md flex flex-col items-center gap-3 group ${
                          selectedMake === make
                            ? 'border-primary-600 bg-primary-50'
                            : 'border-gray-100 hover:border-primary-200 hover:bg-gray-50'
                        }`}
                      >
                        <Car className={`w-8 h-8 ${selectedMake === make ? 'text-primary-600' : 'text-gray-400 group-hover:text-primary-500'}`} />
                        <span className={`font-semibold ${selectedMake === make ? 'text-primary-900' : 'text-gray-700'}`}>
                          {make}
                        </span>
                      </button>
                    ))}
                  </div>
                </motion.div>
              )}

              {step === 2 && (
                <motion.div
                  key="step2"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="p-8 md:p-12"
                >
                  <div className="flex items-center justify-between mb-8">
                    <button 
                      onClick={() => setStep(1)}
                      className="text-gray-500 hover:text-primary-600 flex items-center gap-2 font-medium transition-colors"
                    >
                      <RotateCcw className="w-4 h-4" /> Back to Makes
                    </button>
                    <h3 className="text-2xl font-bold text-gray-900">Select Model</h3>
                    <div className="w-24"></div> {/* Spacer for centering */}
                  </div>
                  
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                    {models.map((model) => (
                      <button
                        key={model}
                        onClick={() => {
                          setSelectedModel(model)
                          setTimeout(() => setStep(3), 200)
                        }}
                        className="p-4 rounded-xl border border-gray-200 hover:border-primary-300 hover:bg-primary-50 hover:shadow-md transition-all text-left flex items-center justify-between group"
                      >
                        <span className="font-semibold text-gray-700 group-hover:text-primary-900">{model}</span>
                        <ChevronRight className="w-5 h-5 text-gray-300 group-hover:text-primary-500" />
                      </button>
                    ))}
                  </div>
                </motion.div>
              )}

              {step === 3 && (
                <motion.div
                  key="step3"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  className="p-8 md:p-12"
                >
                  <div className="flex items-center justify-between mb-8">
                    <button 
                      onClick={() => setStep(2)}
                      className="text-gray-500 hover:text-primary-600 flex items-center gap-2 font-medium transition-colors"
                    >
                      <RotateCcw className="w-4 h-4" /> Back to Models
                    </button>
                    <h3 className="text-2xl font-bold text-gray-900">Select Year</h3>
                    <div className="w-24"></div>
                  </div>

                  <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4 mb-8">
                    {years.map((year) => (
                      <button
                        key={year}
                        onClick={() => setSelectedYear(year)}
                        className={`p-4 rounded-xl border-2 transition-all font-bold text-lg ${
                          selectedYear === year
                            ? 'border-primary-600 bg-primary-600 text-white shadow-lg'
                            : 'border-gray-100 hover:border-primary-300 hover:bg-gray-50 text-gray-700'
                        }`}
                      >
                        {year}
                      </button>
                    ))}
                  </div>

                  <div className="flex justify-center mt-8">
                    <motion.button
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                      onClick={handleSearch}
                      disabled={!selectedYear || loading}
                      className="bg-gradient-to-r from-primary-600 to-primary-800 hover:from-primary-700 hover:to-primary-900 text-white px-12 py-4 rounded-xl font-bold text-lg transition-all shadow-lg hover:shadow-primary-500/30 flex items-center gap-3 disabled:opacity-70 disabled:cursor-not-allowed min-w-[200px] justify-center"
                    >
                      {loading ? (
                        <>Searching...</>
                      ) : (
                        <>
                          Find Tyre Size <Search className="w-5 h-5" />
                        </>
                      )}
                    </motion.button>
                  </div>
                </motion.div>
              )}

              {step === 4 && result && (
                <motion.div
                  key="result"
                  initial={{ opacity: 0, scale: 0.95 }}
                  animate={{ opacity: 1, scale: 1 }}
                  className="p-8 md:p-12 text-center"
                >
                  <div className="inline-flex items-center justify-center w-20 h-20 bg-green-100 rounded-full mb-6 animate-bounce-slow">
                    <CheckCircle2 className="w-10 h-10 text-green-600" />
                  </div>
                  
                  <h3 className="text-3xl font-bold text-gray-900 mb-2">Perfect Match Found!</h3>
                  <p className="text-xl text-gray-600 mb-8">
                    {selectedYear} {selectedMake} {selectedModel} <span className="text-gray-400">|</span> {result.category}
                  </p>

                  <div className="max-w-md mx-auto bg-gradient-to-br from-gray-900 to-gray-800 rounded-2xl p-8 mb-8 text-white shadow-2xl relative overflow-hidden group">
                    <div className="absolute top-0 right-0 w-32 h-32 bg-white/5 rounded-full blur-3xl group-hover:bg-white/10 transition-colors"></div>
                    
                    <p className="text-gray-400 text-sm font-medium uppercase tracking-wider mb-2">Recommended Size</p>
                    <p className="text-5xl md:text-6xl font-bold text-white mb-4 tracking-tight">{result.size}</p>
                    <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-white/10 border border-white/20 text-sm text-gray-300">
                      <CheckCircle2 className="w-3 h-3" /> OEM Specification
                    </div>
                  </div>

                  <div className="flex flex-col sm:flex-row gap-4 justify-center max-w-lg mx-auto">
                    <motion.button
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                      onClick={handleGetQuote}
                      className="flex-1 bg-green-500 hover:bg-green-600 text-white py-4 rounded-xl font-bold text-lg transition-all shadow-lg shadow-green-500/20 flex items-center justify-center gap-2"
                    >
                      <ArrowRight className="w-5 h-5" />
                      Get Price via WhatsApp
                    </motion.button>
                    
                    <motion.button
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                      onClick={handleReset}
                      className="px-8 bg-gray-100 hover:bg-gray-200 text-gray-700 py-4 rounded-xl font-semibold transition-colors flex items-center justify-center gap-2"
                    >
                      <RotateCcw className="w-4 h-4" />
                      Search Again
                    </motion.button>
                  </div>
                </motion.div>
              )}
            </AnimatePresence>
          </motion.div>

          {/* Help Text */}
          <div className="mt-8 text-center">
            <p className="text-gray-500 flex items-center justify-center gap-2">
              <AlertCircle className="w-4 h-4" />
              Not sure about your vehicle details? 
              <a href="#contact" className="text-primary-600 font-semibold hover:underline">Contact our experts</a>
            </p>
          </div>
        </div>
      </div>

      {/* Quote Modal */}
      <QuoteModal 
        isOpen={isQuoteModalOpen} 
        onClose={() => setIsQuoteModalOpen(false)}
        initialData={result ? {
          tireSize: result.size,
          vehicle: `${selectedYear} ${selectedMake} ${selectedModel}`
        } : undefined}
      />
    </section>
  )
}
