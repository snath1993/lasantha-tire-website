'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef } from 'react'
import { Star } from 'lucide-react'

const testimonials = [
  {
    name: 'Kasun Perera',
    location: 'Colombo',
    rating: 5,
    image: '/images/customer-1.jpg',
    text: 'Excellent service! Got my car tyres replaced quickly and professionally. The staff was very knowledgeable and helped me choose the right tyres for my vehicle.',
    date: 'October 2024'
  },
  {
    name: 'Nimal Silva',
    location: 'Kandy',
    rating: 5,
    image: '/images/customer-2.jpg',
    text: 'Best tyre shop in Sri Lanka! Fair prices, genuine products, and outstanding customer service. Highly recommend Lasantha Tyre Traders.',
    date: 'September 2024'
  },
  {
    name: 'Samantha Fernando',
    location: 'Galle',
    rating: 5,
    image: '/images/customer-3.jpg',
    text: 'Very satisfied with their wheel alignment service. My car drives much smoother now. Professional team and reasonable prices.',
    date: 'November 2024'
  },
  {
    name: 'Dilshan Jayawardena',
    location: 'Negombo',
    rating: 5,
    image: '/images/customer-4.jpg',
    text: 'Bought Michelin tyres for my SUV. Great quality products with warranty. The installation was done perfectly. Will definitely come back!',
    date: 'October 2024'
  }
]

function TestimonialCard({ testimonial, index }: { testimonial: typeof testimonials[0], index: number }) {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <motion.div
      ref={ref}
      initial={{ opacity: 0, y: 50 }}
      animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 50 }}
      transition={{ duration: 0.5, delay: index * 0.1 }}
      className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-2xl transition-all duration-300"
    >
      <div className="flex items-center gap-4 mb-6">
        <div className="w-16 h-16 rounded-full bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center text-white text-2xl font-bold">
          {testimonial.name.charAt(0)}
        </div>
        <div className="flex-1">
          <h4 className="font-bold text-gray-900 text-lg">{testimonial.name}</h4>
          <p className="text-sm text-gray-600">{testimonial.location}</p>
        </div>
        <div className="flex gap-1">
          {Array.from({ length: testimonial.rating }).map((_, i) => (
            <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
          ))}
        </div>
      </div>
      
      <p className="text-gray-700 leading-relaxed mb-4 italic">&ldquo;{testimonial.text}&rdquo;</p>
      
      <p className="text-sm text-gray-500">{testimonial.date}</p>
    </motion.div>
  )
}

export default function Testimonials() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <section id="testimonials" className="py-20 bg-white">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            What Our <span className="text-primary-600">Customers Say</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Don&apos;t just take our word for it - hear from our satisfied customers
          </p>
        </motion.div>

        <div className="grid md:grid-cols-2 gap-8 mb-12">
          {testimonials.map((testimonial, index) => (
            <TestimonialCard key={index} testimonial={testimonial} index={index} />
          ))}
        </div>

        {/* Stats Section */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.4 }}
          className="grid grid-cols-2 md:grid-cols-4 gap-6 mt-16"
        >
          <div className="text-center">
            <p className="text-4xl md:text-5xl font-bold text-primary-600 mb-2">22+</p>
            <p className="text-gray-600 font-medium">Years Experience</p>
          </div>
          <div className="text-center">
            <p className="text-4xl md:text-5xl font-bold text-primary-600 mb-2">5000+</p>
            <p className="text-gray-600 font-medium">Happy Customers</p>
          </div>
          <div className="text-center">
            <p className="text-4xl md:text-5xl font-bold text-primary-600 mb-2">50+</p>
            <p className="text-gray-600 font-medium">Tyre Brands</p>
          </div>
          <div className="text-center">
            <p className="text-4xl md:text-5xl font-bold text-primary-600 mb-2">100%</p>
            <p className="text-gray-600 font-medium">Satisfaction Rate</p>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
