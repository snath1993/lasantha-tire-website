import Header from '@/components/Header'
import Hero from '@/components/Hero'
import WhyChooseUs from '@/components/WhyChooseUs'
import Brands from '@/components/Brands'
import ShopByVehicle from '@/components/ShopByVehicle'
import TyreCareBlueprint from '@/components/TyreCareBlueprint'
import BlogSection from '@/components/BlogSection'
import RealReviews from '@/components/RealReviews'
import Testimonials from '@/components/Testimonials'
import Footer from '@/components/Footer'

export default function Home() {
  return (
    <main>
      <Header />
      <Hero />
      <WhyChooseUs />
      <Brands />
      <ShopByVehicle />
      <TyreCareBlueprint />
      <RealReviews />
      <Testimonials />
      <BlogSection />
      <Footer />
    </main>
  )
}
