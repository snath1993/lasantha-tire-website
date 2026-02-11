import React from 'react'
import Header from '@/components/Header'
import Footer from '@/components/Footer'
import Gallery from '@/components/Gallery'
import RealReviews from '@/components/RealReviews'
import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Gallery & Customer Reviews | Lasantha Tyre Traders',
  description: 'See our modern tyre service facility in Battaramulla, state-of-the-art equipment, and read what our customers say about Lasantha Tyre Traders.',
  openGraph: {
    title: 'Gallery | Lasantha Tyre Traders',
    description: 'Tour our state-of-the-art tyre service facility and see customer reviews.',
    url: 'https://lasanthatyre.com/gallery',
  },
}

export default function GalleryPage() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <main className="pt-24">
        <div className="bg-gray-900 text-white py-16 mb-12">
          <div className="container mx-auto px-4 text-center">
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Our Gallery</h1>
            <p className="text-xl text-gray-400 max-w-2xl mx-auto">
              See our state-of-the-art facility and happy customers.
            </p>
          </div>
        </div>

        <Gallery />
        <RealReviews />
      </main>
      <Footer />
    </div>
  )
}
