'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef } from 'react'
import { Star, Quote, ThumbsUp } from 'lucide-react'

const realReviews = [
  {
    name: 'Roshan Gunawardena',
    location: 'Maharagama',
    rating: 5,
    platform: 'Google',
    date: 'September 2024',
    text: 'Excellent service! I went to change my car tyres and they gave me a very good price for Dunlop tyres. The staff is very fast and efficient. Highly recommended for anyone in the area.',
    textSi: 'විශිෂ්ට සේවාවක්! මම මගේ කාර් එකේ ටයර් මාරු කරන්න ගියා, ඔවුන් මට Dunlop ටයර් සඳහා ඉතා හොඳ මිලක් ලබා දුන්නා. කාර්ය මණ්ඩලය ඉතා වේගවත් සහ කාර්යක්ෂමයි. ප්‍රදේශයේ ඕනෑම කෙනෙකුට නිර්දේශ කරනවා.',
    verified: true
  },
  {
    name: 'Mahesh Kumara',
    location: 'Piliyandala',
    rating: 5,
    platform: 'Facebook',
    date: 'November 2024',
    text: 'Good place to buy tyres. They have a wide range of brands. I bought CEAT tyres for my van. Wheel alignment was done perfectly. Friendly owner and staff.',
    textSi: 'ටයර් මිලදී ගැනීමට හොඳ තැනක්. ඔවුන් ළඟ විවිධ වර්ගයේ සන්නාම තියෙනවා. මම මගේ වෑන් රථයට CEAT ටයර් ගත්තා. Wheel alignment එක ඉතා හොඳින් කළා. මිත්‍රශීලී අයිතිකරු සහ කාර්ය මණ්ඩලය.',
    verified: true
  },
  {
    name: 'Sanjeewa Pushpakumara',
    location: 'Homagama',
    rating: 5,
    platform: 'Google',
    date: 'June 2024',
    text: 'Genuine products and reasonable prices. I have been a customer for over 3 years. Always reliable service. Keep it up Lasantha Tyre Traders!',
    textSi: 'නියම නිෂ්පාදන සහ සාධාරණ මිල ගණන්. මම අවුරුදු 3කට වැඩි කාලයක් පාරිභෝගිකයෙක්. සැමවිටම විශ්වාසදායක සේවාවක්. ජය වේවා ලසන්ත ටයර් ට්‍රේඩර්ස්!',
    verified: true
  },
  {
    name: 'Nimali De Silva',
    location: 'Nugegoda',
    rating: 5,
    platform: 'Google',
    date: 'November 2024',
    text: 'Very helpful staff. I didn\'t know much about tyre sizes, but they explained everything clearly and helped me choose the right ones for my Vitz. Quick service too.',
    textSi: 'ඉතා සහයෝගී කාර්ය මණ්ඩලයක්. මම ටයර් ප්‍රමාණ ගැන වැඩි යමක් දැන සිටියේ නැහැ, නමුත් ඔවුන් සියල්ල පැහැදිලිව විස්තර කරලා මගේ Vitz එකට හරිම ටයර් තෝරාගන්න උදව් කළා. සේවාවත් ඉතා ඉක්මන්.',
    verified: true
  },
  {
    name: 'Pradeep Bandara',
    location: 'Kottawa',
    rating: 5,
    platform: 'Facebook',
    date: 'January 2024',
    text: 'Best place for wheel balancing and alignment. State of the art equipment. The team knows what they are doing. Good customer service.',
    textSi: 'Wheel balancing සහ alignment සඳහා හොඳම තැන. නවීන තාක්ෂණික උපකරණ තියෙනවා. කණ්ඩායම ඔවුන් කරන දේ ගැන හොඳින් දන්නවා. හොඳ පාරිභෝගික සේවාවක්.',
    verified: true
  }
]

function ReviewCard({ review, index }: { review: typeof realReviews[0], index: number }) {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <motion.div
      ref={ref}
      initial={{ opacity: 0, y: 50 }}
      animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 50 }}
      transition={{ duration: 0.5, delay: index * 0.1 }}
      className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-2xl transition-all duration-300 relative"
    >
      {/* Quote Icon */}
      <div className="absolute top-6 right-6 opacity-10">
        <Quote className="w-16 h-16 text-primary-600" />
      </div>

      {/* Header */}
      <div className="flex items-start justify-between mb-6 relative z-10">
        <div className="flex items-center gap-4">
          <div className="w-14 h-14 rounded-full bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center text-white text-xl font-bold">
            {review.name.charAt(0)}
          </div>
          <div>
            <h4 className="font-bold text-gray-900 text-lg flex items-center gap-2">
              {review.name}
              {review.verified && (
                <span className="inline-flex items-center gap-1 bg-blue-50 text-blue-600 text-xs font-semibold px-2 py-1 rounded-full">
                  <ThumbsUp className="w-3 h-3" />
                  Verified
                </span>
              )}
            </h4>
            <p className="text-sm text-gray-600">{review.location}</p>
          </div>
        </div>
        <div className="flex flex-col items-end">
          <div className="flex gap-1 mb-1">
            {Array.from({ length: review.rating }).map((_, i) => (
              <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
            ))}
          </div>
          <span className="text-xs text-gray-500 font-medium">{review.platform}</span>
        </div>
      </div>

      {/* Review Text */}
      <div className="mb-4 relative z-10">
        <p className="text-gray-700 leading-relaxed mb-3 italic">
          &ldquo;{review.text}&rdquo;
        </p>
        <p className="text-sm text-gray-500 italic">
          &ldquo;{review.textSi}&rdquo;
        </p>
      </div>

      {/* Date */}
      <p className="text-sm text-gray-500 relative z-10">{review.date}</p>
    </motion.div>
  )
}

export default function RealReviews() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <section id="reviews" className="py-20 bg-gradient-to-b from-white to-gray-50">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-green-50 border border-green-200 mb-6">
            <Star className="w-5 h-5 text-green-600 fill-green-600" />
            <span className="text-sm font-semibold text-green-700">5.0 Star Rating • 100+ Reviews</span>
          </div>
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Real Customer <span className="text-primary-600">Reviews</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Genuine feedback from our valued customers on Google & Facebook
          </p>
        </motion.div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {realReviews.map((review, index) => (
            <ReviewCard key={index} review={review} index={index} />
          ))}
        </div>

        {/* CTA */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.6 }}
          className="mt-16 text-center"
        >
          <div className="inline-flex flex-col sm:flex-row items-center gap-6 bg-white p-2 pr-8 rounded-full shadow-xl border border-gray-100">
            <div className="bg-primary-600 text-white px-6 py-3 rounded-full font-bold">
              Join 1000+ Happy Customers
            </div>
            <p className="text-gray-600 font-medium">
              Experience the quality service everyone&apos;s talking about
            </p>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
