'use client'

import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Calculator, Info, X, RotateCcw } from 'lucide-react'
import TyreDiagram from './TyreDiagram'

interface CalculatorModalProps {
  isOpen: boolean
  onClose: () => void
}

interface TyreSize {
  width: number
  aspectRatio: number
  rimDiameter: number
}

interface CalculationResult {
  diameter: number
  circumference: number
  sidewall: number
  revPerKm: number
}

export default function CalculatorModal({ isOpen, onClose }: CalculatorModalProps) {
  const [size1, setSize1] = useState('')
  const [size2, setSize2] = useState('')
  const [result, setResult] = useState<{
    size1: CalculationResult | null
    size2: CalculationResult | null
    differences: {
      diameter: number
      circumference: number
      sidewall: number
      speedometerError: number
    } | null
  }>({
    size1: null,
    size2: null,
    differences: null
  })
  const [error, setError] = useState('')

  // Reset when modal opens
  useEffect(() => {
    if (isOpen) {
      setSize1('')
      setSize2('')
      setResult({ size1: null, size2: null, differences: null })
      setError('')
    }
  }, [isOpen])

  const parseTyreSize = (sizeStr: string): TyreSize | null => {
    const cleaned = sizeStr.replace(/[rR\s-]/g, '/')
    const parts = cleaned.split('/').map(p => parseInt(p.trim()))
    
    if (parts.length === 3 && parts.every(p => !isNaN(p))) {
      return {
        width: parts[0],
        aspectRatio: parts[1],
        rimDiameter: parts[2]
      }
    }
    return null
  }

  const calculateTyreSpecs = (size: TyreSize): CalculationResult => {
    const sidewall = (size.width * size.aspectRatio) / 100
    const diameter = (size.rimDiameter * 25.4) + (2 * sidewall)
    const circumference = Math.PI * diameter
    const revPerKm = 1000000 / circumference
    
    return {
      diameter: Math.round(diameter * 10) / 10,
      circumference: Math.round(circumference * 10) / 10,
      sidewall: Math.round(sidewall * 10) / 10,
      revPerKm: Math.round(revPerKm)
    }
  }

  const handleCompare = () => {
    setError('')
    
    if (!size1 || !size2) {
      setError('Please enter both tyre sizes')
      return
    }

    const parsed1 = parseTyreSize(size1)
    const parsed2 = parseTyreSize(size2)

    if (!parsed1 || !parsed2) {
      setError('Invalid tyre size format. Use format: 195/65R15')
      return
    }

    const calc1 = calculateTyreSpecs(parsed1)
    const calc2 = calculateTyreSpecs(parsed2)

    const diameterDiff = ((calc2.diameter - calc1.diameter) / calc1.diameter) * 100
    const circumferenceDiff = ((calc2.circumference - calc1.circumference) / calc1.circumference) * 100
    const sidewallDiff = calc2.sidewall - calc1.sidewall
    const speedometerError = diameterDiff

    setResult({
      size1: calc1,
      size2: calc2,
      differences: {
        diameter: Math.round(diameterDiff * 100) / 100,
        circumference: Math.round(circumferenceDiff * 100) / 100,
        sidewall: Math.round(sidewallDiff * 10) / 10,
        speedometerError: Math.round(speedometerError * 100) / 100
      }
    })
  }

  const handleClear = () => {
    setSize1('')
    setSize2('')
    setResult({ size1: null, size2: null, differences: null })
    setError('')
  }

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
            className="bg-white rounded-2xl shadow-2xl w-full max-w-4xl my-8 relative"
          >
            <button
              onClick={onClose}
              className="absolute top-4 right-4 p-2 rounded-full hover:bg-gray-100 transition-colors z-10"
            >
              <X className="w-6 h-6 text-gray-500" />
            </button>

            <div className="p-6 md:p-8">
              <div className="text-center mb-8">
                <div className="inline-flex items-center justify-center w-12 h-12 bg-primary-100 rounded-full mb-4">
                  <Calculator className="w-6 h-6 text-primary-600" />
                </div>
                <h2 className="text-3xl font-bold text-gray-900">
                  Tyre Size <span className="text-primary-600">Calculator</span>
                </h2>
              </div>

              <div className="grid md:grid-cols-2 gap-6 mb-8">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Original Tyre Size
                  </label>
                  <input
                    type="text"
                    value={size1}
                    onChange={(e) => setSize1(e.target.value)}
                    placeholder="e.g., 195/65R15"
                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent outline-none"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    New Tyre Size
                  </label>
                  <input
                    type="text"
                    value={size2}
                    onChange={(e) => setSize2(e.target.value)}
                    placeholder="e.g., 205/60R15"
                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-600 focus:border-transparent outline-none"
                  />
                </div>
              </div>

              {error && (
                <div className="mb-6 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                  {error}
                </div>
              )}

              <div className="flex gap-4 mb-8">
                <button
                  onClick={handleCompare}
                  className="flex-1 bg-primary-600 hover:bg-primary-700 text-white py-3 rounded-lg font-semibold transition-colors flex items-center justify-center gap-2"
                >
                  <Calculator className="w-5 h-5" />
                  Compare
                </button>
                <button
                  onClick={handleClear}
                  className="px-6 bg-gray-100 hover:bg-gray-200 text-gray-700 py-3 rounded-lg font-semibold transition-colors"
                >
                  <RotateCcw className="w-5 h-5" />
                </button>
              </div>

              {result.differences && (
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  className="space-y-6"
                >
                  <div className="bg-primary-50 rounded-xl p-5">
                    <h3 className="font-bold text-gray-900 mb-4 flex items-center gap-2">
                      <Info className="w-5 h-5 text-primary-600" />
                      Key Differences
                    </h3>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                      <div className="bg-white p-3 rounded-lg text-center">
                        <p className="text-xs text-gray-500 mb-1">Diameter</p>
                        <p className={`font-bold ${Math.abs(result.differences.diameter) > 3 ? 'text-red-600' : 'text-green-600'}`}>
                          {result.differences.diameter > 0 ? '+' : ''}{result.differences.diameter}%
                        </p>
                      </div>
                      <div className="bg-white p-3 rounded-lg text-center">
                        <p className="text-xs text-gray-500 mb-1">Speedometer</p>
                        <p className={`font-bold ${Math.abs(result.differences.speedometerError) > 3 ? 'text-red-600' : 'text-green-600'}`}>
                          {result.differences.speedometerError > 0 ? '+' : ''}{result.differences.speedometerError}%
                        </p>
                      </div>
                      <div className="bg-white p-3 rounded-lg text-center">
                        <p className="text-xs text-gray-500 mb-1">Sidewall</p>
                        <p className="font-bold text-gray-900">
                          {result.differences.sidewall > 0 ? '+' : ''}{result.differences.sidewall}mm
                        </p>
                      </div>
                      <div className="bg-white p-3 rounded-lg text-center">
                        <p className="text-xs text-gray-500 mb-1">Circumference</p>
                        <p className="font-bold text-gray-900">
                          {result.differences.circumference > 0 ? '+' : ''}{result.differences.circumference}%
                        </p>
                      </div>
                    </div>
                  </div>

                  {/* Detailed Visual Analysis with Tyre Diagrams */}
                  <div className="bg-gradient-to-br from-gray-50 to-gray-100 rounded-xl p-6 border border-gray-200">
                    <h3 className="font-bold text-gray-900 mb-6 text-center text-lg">
                      📊 Detailed Analysis with Visual Representation
                    </h3>
                    
                    <div className="grid md:grid-cols-2 gap-8">
                      {/* Original Tyre Diagram */}
                      <div className="bg-white rounded-lg p-6 shadow-sm border border-gray-200">
                        <h4 className="font-bold text-gray-900 mb-4 text-center">Original: {size1}</h4>
                        
                        {/* SVG Tyre Cross Section */}
                        <div className="mb-6 flex justify-center">
                          <TyreDiagram
                            width={parseTyreSize(size1)?.width || 0}
                            aspectRatio={parseTyreSize(size1)?.aspectRatio || 0}
                            rimDiameter={parseTyreSize(size1)?.rimDiameter || 0}
                            sidewall={result.size1?.sidewall || 0}
                            patternId="treadPattern1"
                          />
                        </div>
                        
                        {/* Specs Table */}
                        <div className="space-y-2 text-sm">
                          <div className="flex justify-between items-center p-2 bg-red-50 rounded">
                            <span className="text-gray-600">Width:</span>
                            <span className="font-bold text-red-700">{parseTyreSize(size1)?.width} mm</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-blue-50 rounded">
                            <span className="text-gray-600">Aspect Ratio:</span>
                            <span className="font-bold text-blue-700">{parseTyreSize(size1)?.aspectRatio}%</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-green-50 rounded">
                            <span className="text-gray-600">Rim Diameter:</span>
                            <span className="font-bold text-green-700">{parseTyreSize(size1)?.rimDiameter}"</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-gray-50 rounded">
                            <span className="text-gray-600">Sidewall Height:</span>
                            <span className="font-bold text-gray-900">{result.size1?.sidewall} mm</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-gray-50 rounded">
                            <span className="text-gray-600">Overall Diameter:</span>
                            <span className="font-bold text-gray-900">{result.size1?.diameter} mm</span>
                          </div>
                        </div>
                      </div>
                      
                      {/* New Tyre Diagram */}
                      <div className="bg-white rounded-lg p-6 shadow-sm border border-gray-200">
                        <h4 className="font-bold text-gray-900 mb-4 text-center">New: {size2}</h4>
                        
                        {/* SVG Tyre Cross Section */}
                        <div className="mb-6 flex justify-center">
                          <TyreDiagram
                            width={parseTyreSize(size2)?.width || 0}
                            aspectRatio={parseTyreSize(size2)?.aspectRatio || 0}
                            rimDiameter={parseTyreSize(size2)?.rimDiameter || 0}
                            sidewall={result.size2?.sidewall || 0}
                            patternId="treadPattern2"
                          />
                        </div>
                        
                        {/* Specs Table */}
                        <div className="space-y-2 text-sm">
                          <div className="flex justify-between items-center p-2 bg-red-50 rounded">
                            <span className="text-gray-600">Width:</span>
                            <span className="font-bold text-red-700">{parseTyreSize(size2)?.width} mm</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-blue-50 rounded">
                            <span className="text-gray-600">Aspect Ratio:</span>
                            <span className="font-bold text-blue-700">{parseTyreSize(size2)?.aspectRatio}%</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-green-50 rounded">
                            <span className="text-gray-600">Rim Diameter:</span>
                            <span className="font-bold text-green-700">{parseTyreSize(size2)?.rimDiameter}"</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-gray-50 rounded">
                            <span className="text-gray-600">Sidewall Height:</span>
                            <span className="font-bold text-gray-900">{result.size2?.sidewall} mm</span>
                          </div>
                          <div className="flex justify-between items-center p-2 bg-gray-50 rounded">
                            <span className="text-gray-600">Overall Diameter:</span>
                            <span className="font-bold text-gray-900">{result.size2?.diameter} mm</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    {/* Legend */}
                    <div className="mt-6 pt-4 border-t border-gray-300">
                      <p className="text-sm text-gray-600 text-center mb-3 font-semibold">Measurement Guide:</p>
                      <div className="flex flex-wrap justify-center gap-4 text-xs">
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-1 bg-red-600"></div>
                          <span className="text-gray-700">Width (Section Width)</span>
                        </div>
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-1 bg-blue-600"></div>
                          <span className="text-gray-700">Sidewall Height</span>
                        </div>
                        <div className="flex items-center gap-2">
                          <div className="w-4 h-1 bg-green-600"></div>
                          <span className="text-gray-700">Rim Diameter</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </motion.div>
              )}
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  )
}
