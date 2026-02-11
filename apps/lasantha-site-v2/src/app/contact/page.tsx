import React from 'react'
import Header from '@/components/Header'
import Footer from '@/components/Footer'
import LocationMap from '@/components/LocationMap'
import QuoteForm from '@/components/QuoteForm'
import FAQ from '@/components/FAQ'
import { Phone, MapPin, Clock } from 'lucide-react'
import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Contact Us | Lasantha Tyre Traders',
  description: 'Visit Lasantha Tyre Traders at 1035 Pannipitiya Road, Battaramulla. Call 077 313 1883 for instant tyre quotes. Open 365 days, 06:30 AM - 9:00 PM.',
  keywords: ['tyre shop battaramulla', 'tyre shop pannipitiya road', 'contact lasantha tyre', 'tyre shop phone number sri lanka', 'tyre service near me'],
  openGraph: {
    title: 'Contact Lasantha Tyre Traders',
    description: 'Get in touch for quotes, appointments, or any questions. Located at 1035 Pannipitiya Road, Battaramulla.',
    url: 'https://lasanthatyre.com/contact',
  },
}

export default function ContactPage() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <main className="pt-24">
        <div className="bg-red-600 text-white py-16 mb-12">
          <div className="container mx-auto px-4 text-center">
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Contact Us</h1>
            <p className="text-xl text-red-100 max-w-2xl mx-auto">
              Get in touch with us for quotes, appointments, or any questions.
            </p>
          </div>
        </div>

        <div className="container mx-auto px-4 mb-16">
          <div className="grid md:grid-cols-3 gap-8 mb-16">
            <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 text-center hover:shadow-md transition-all">
              <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-6">
                <Phone className="w-8 h-8 text-red-600" />
              </div>
              <h3 className="text-xl font-bold mb-2">Call Us</h3>
              <p className="text-gray-600 mb-1">Hotline: 077 313 1883</p>
              <p className="text-gray-600">Office: 072 122 2509</p>
            </div>
            
            <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 text-center hover:shadow-md transition-all">
              <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-6">
                <MapPin className="w-8 h-8 text-red-600" />
              </div>
              <h3 className="text-xl font-bold mb-2">Visit Us</h3>
              <p className="text-gray-600">
                Lasantha Tyre Traders<br />
                1035 Pannipitiya Road, Battaramulla
              </p>
            </div>

            <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 text-center hover:shadow-md transition-all">
              <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-6">
                <Clock className="w-8 h-8 text-red-600" />
              </div>
              <h3 className="text-xl font-bold mb-2">Opening Hours</h3>
              <p className="text-gray-600 mb-1">Open 365 Days</p>
              <p className="text-gray-600">06:30 AM - 9:00 PM</p>
            </div>
          </div>

          <div className="grid lg:grid-cols-2 gap-12">
            <div>
              <h2 className="text-3xl font-bold mb-8">Send us a Message</h2>
              <QuoteForm />
            </div>
            <div>
              <h2 className="text-3xl font-bold mb-8">Find Us</h2>
              <div className="h-[400px] md:h-[600px] rounded-3xl overflow-hidden shadow-lg">
                <LocationMap />
              </div>
            </div>
          </div>
        </div>

        <FAQ />
      </main>
      <Footer />
    </div>
  )
}
