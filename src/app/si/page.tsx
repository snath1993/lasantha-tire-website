import Header from '@/components/Header'
import Hero from '@/components/Hero'
import WhyChooseUs from '@/components/WhyChooseUs'
import Services from '@/components/Services'
import Brands from '@/components/Brands'
import ShopByVehicle from '@/components/ShopByVehicle'
import BlogSection from '@/components/BlogSection'
import Gallery from '@/components/Gallery'
import RealReviews from '@/components/RealReviews'
import Testimonials from '@/components/Testimonials'
import FAQ from '@/components/FAQ'
import QuoteForm from '@/components/QuoteForm'
import FleetPortal from '@/components/FleetPortal'
import LocationMap from '@/components/LocationMap'
import Footer from '@/components/Footer'
import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Lasantha Tyre Traders | ශ්‍රී ලංකාවේ හොඳම ටයර් මිල ගණන්',
  description: 'WhatsApp හරහා ක්ෂණික ටයර් මිල ගණන් ලබාගන්න. ප්‍රමුඛ පෙළේ ටයර් සන්නාම, විශේෂඥ සේවාව, රෝද අනුකූලතාව (Wheel Alignment) සහ සියලුම ටයර් සේවා.',
  alternates: {
    languages: {
      'en-US': '/',
      'si-LK': '/si',
    },
  },
  openGraph: {
    locale: 'si_LK',
    title: 'Lasantha Tyre Traders | ක්ෂණික ටයර් මිල ගණන්',
    description: 'ශ්‍රී ලංකාවේ හොඳම ටයර් මිල ගණන් WhatsApp හරහා ලබාගන්න.',
  }
}

export default function SinhalaHome() {
  return (
    <main>
      {/* Note: Ideally components should accept a 'lang' prop or content dictionary. 
          For now, we are reusing the English components but setting the page metadata to Sinhala.
          To fully localize, we would need to refactor components to accept translations. 
          Given the current scope, we are setting up the route structure first. */}
      <Header />
      <Hero />
      <WhyChooseUs />
      <Services />
      <Brands />
      <ShopByVehicle />
      <BlogSection />
      <Gallery />
      <RealReviews />
      <Testimonials />
      <FAQ />
      <QuoteForm />
      <FleetPortal />
      <LocationMap />
      <Footer />
    </main>
  )
}
