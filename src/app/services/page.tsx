import React from 'react'
import Header from '@/components/Header'
import Footer from '@/components/Footer'
import Services from '@/components/Services'
import FleetPortal from '@/components/FleetPortal'

export default function ServicesPage() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <main className="pt-24">
        <div className="bg-red-600 text-white py-16 mb-12">
          <div className="container mx-auto px-4 text-center">
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Our Services</h1>
            <p className="text-xl text-red-100 max-w-2xl mx-auto">
              Professional tyre services, alignment, balancing, and more. We ensure your safety on the road.
            </p>
          </div>
        </div>
        
        <Services />
        
        <FleetPortal />
      </main>
      <Footer />
    </div>
  )
}
