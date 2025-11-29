'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import { BookOpen, Clock, ArrowRight, Sparkles, Lightbulb } from 'lucide-react'
import Link from 'next/link'

import { blogPosts } from '@/lib/blogData'

const knowledgeCategories = ['All', 'Maintenance', 'Education', 'Tips', 'Services', 'Seasonal', 'Safety', 'Fuel Economy']

const quickTips = [
  {
    title: 'කලින් පීඩනය බලන්න',
    description: 'මාසිකව හෝ දිගු ගමනකට පෙර සියලු ටයර් 32-35 PSI අතරදැයි බලන්න.',
  },
  {
    title: 'Rotation Routine',
    description: '10,000km ට වරක් cross-rotation එකක් කරන්න tread unevenness අවම කරගන්න.',
  },
  {
    title: 'Alignment Reminder',
    description: 'වාහනය එක පැත්තකට ඇදෙනවාද? වහාම CCD wheel alignment කරගන්න.',
  },
  {
    title: 'බර සහ ගැලීම්',
    description: 'අතිරික්ත බර හෝ pothole impact වලින් වහාම visual crack check එකක් කරන්න.',
  },
]

const featuredGuide = {
  title: 'Tyre Care Blueprint (Sri Lanka)',
  description: 'High-humidity, stop-go Colombo traffic එකට හොඳම weekly + monthly routine එක මෙන්න.',
  bullets: [
    'Weekly: cold PSI + tread wear bar check',
    'Monthly: wheel balancing + rotation log update',
    'Quarterly: roadside emergency kit + spare inspection',
  ],
}

export default function BlogSection() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  const [selectedCategory, setSelectedCategory] = useState('All')
  const [showSinhala, setShowSinhala] = useState(false)

  const filteredPosts = blogPosts.filter((post) =>
    selectedCategory === 'All' ? true : post.category === selectedCategory
  )
  const articleCount = filteredPosts.length

  return (
    <section id="blog" className="py-20 bg-gray-50">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <div className="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-full mb-4">
            <BookOpen className="w-8 h-8 text-primary-600" />
          </div>
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Tyre Knowledge <span className="text-primary-600">Center</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Expert advice and guides to help you make informed decisions about your tyres
          </p>
        </motion.div>

        <div className="flex flex-col gap-6 mb-10">
          <div className="flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4">
            <div className="flex flex-wrap gap-3">
              {knowledgeCategories.map((category) => {
                const isActive = selectedCategory === category
                return (
                  <button
                    key={category}
                    onClick={() => setSelectedCategory(category)}
                    className={`px-4 py-2 rounded-full text-sm font-semibold transition-all border ${
                      isActive
                        ? 'bg-primary-600 text-white border-primary-600 shadow-lg'
                        : 'bg-white text-gray-600 border-gray-200 hover:border-primary-300'
                    }`}
                  >
                    {category}
                  </button>
                )
              })}
            </div>

            <div className="flex items-center gap-2 bg-white border border-gray-200 rounded-full px-2 py-1">
              <button
                onClick={() => setShowSinhala(false)}
                className={`px-3 py-1 text-sm font-semibold rounded-full transition ${
                  !showSinhala ? 'bg-primary-600 text-white shadow' : 'text-gray-600'
                }`}
              >
                EN
              </button>
              <button
                onClick={() => setShowSinhala(true)}
                className={`px-3 py-1 text-sm font-semibold rounded-full transition ${
                  showSinhala ? 'bg-primary-600 text-white shadow' : 'text-gray-600'
                }`}
              >
                සිං
              </button>
            </div>
          </div>

          <div className="flex items-center justify-between text-sm text-gray-500">
            <span>{articleCount} curated resources</span>
            <div className="inline-flex items-center gap-2 text-primary-600 font-semibold">
              <Sparkles className="w-4 h-4" /> Updated weekly with local data
            </div>
          </div>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {filteredPosts.map((post, index) => {
            const displayTitle = showSinhala ? post.titleSi : post.title
            const secondaryTitle = showSinhala ? post.title : post.titleSi
            const displayExcerpt = showSinhala ? post.excerptSi : post.excerpt
            const readMoreLabel = showSinhala ? 'තවත් කියවන්න' : 'Read More'

            return (
              <motion.article
                key={index}
                initial={{ opacity: 0, y: 50 }}
                animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 50 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
                className="bg-white rounded-2xl shadow-lg overflow-hidden hover:shadow-2xl transition-all duration-300 group cursor-pointer"
              >
                {/* Image Placeholder */}
                <div className="relative h-48 bg-gradient-to-br from-primary-100 to-blue-100 overflow-hidden">
                  <div className="absolute inset-0 flex items-center justify-center">
                    <BookOpen className="w-16 h-16 text-primary-300" />
                  </div>
                  <div className="absolute top-4 left-4 bg-white/90 backdrop-blur-sm px-3 py-1 rounded-full text-xs font-semibold text-primary-600">
                    {post.category}
                  </div>
                </div>

                <div className="p-6">
                  <div className="flex items-center gap-2 text-sm text-gray-500 mb-3">
                    <Clock className="w-4 h-4" />
                    <span>{post.readTime}</span>
                  </div>

                  <h3 className="text-xl font-bold text-gray-900 mb-2 group-hover:text-primary-600 transition-colors line-clamp-2">
                    {displayTitle}
                  </h3>
                  <p className="text-sm font-medium text-gray-500 mb-3 line-clamp-1">
                    {secondaryTitle}
                  </p>

                  <p className="text-gray-600 mb-4 line-clamp-3">
                    {displayExcerpt}
                  </p>

                  <Link
                    href={`/blog/${post.slug}`}
                    className="inline-flex items-center gap-2 text-primary-600 font-semibold hover:gap-3 transition-all group"
                  >
                    {readMoreLabel}
                    <ArrowRight className="w-4 h-4" />
                  </Link>
                </div>
              </motion.article>
            )
          })}
        </div>

        <div className="grid lg:grid-cols-3 gap-8 mt-12">
          <div className="lg:col-span-2 bg-white rounded-2xl shadow-lg border border-primary-100 p-8">
            <div className="flex items-center gap-3 mb-6">
              <Lightbulb className="w-6 h-6 text-primary-600" />
              <div>
                <p className="text-sm uppercase tracking-wide text-primary-600 font-semibold">Quick wins</p>
                <h3 className="text-2xl font-bold text-gray-900">Weekly Tyre Care Tips</h3>
              </div>
            </div>

            <div className="grid md:grid-cols-2 gap-4">
              {quickTips.map((tip, index) => (
                <div key={index} className="p-4 rounded-xl bg-primary-50/60 border border-primary-100">
                  <p className="text-sm font-semibold text-primary-600 mb-1">
                    Tip {index + 1}
                  </p>
                  <h4 className="text-lg font-bold text-gray-900 mb-1">{tip.title}</h4>
                  <p className="text-sm text-gray-600">{tip.description}</p>
                </div>
              ))}
            </div>
          </div>

          <div className="bg-gradient-to-br from-gray-900 via-gray-800 to-primary-900 rounded-2xl text-white p-8 shadow-xl relative overflow-hidden">
            <div className="absolute inset-0 opacity-20" style={{ backgroundImage: 'radial-gradient(circle at top, #fff, transparent 60%)' }} />
            <div className="relative">
              <p className="text-sm uppercase tracking-[0.3em] text-primary-200 mb-2">Featured Guide</p>
              <h3 className="text-2xl font-bold mb-3">{featuredGuide.title}</h3>
              <p className="text-sm text-white/80 mb-6">{featuredGuide.description}</p>
              <ul className="space-y-3 mb-6">
                {featuredGuide.bullets.map((item, index) => (
                  <li key={index} className="flex items-start gap-3 text-sm">
                    <span className="mt-1 h-2 w-2 rounded-full bg-primary-300" />
                    {item}
                  </li>
                ))}
              </ul>

              <Link
                href="https://wa.me/94721222509?text=Hi,%20send%20me%20the%20Tyre%20Care%20Blueprint"
                target="_blank"
                rel="noopener noreferrer"
                className="inline-flex items-center gap-2 bg-white text-gray-900 px-5 py-3 rounded-xl font-semibold"
              >
                Download Checklist
                <ArrowRight className="w-4 h-4" />
              </Link>
            </div>
          </div>
        </div>

        {/* CTA */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.6 }}
          className="mt-16 text-center"
        >
          <p className="text-gray-600 mb-4">Have questions about tyres?</p>
          <Link
            href="https://wa.me/94721222509?text=Hi,%20I%20have%20a%20question%20about%20tyres"
            target="_blank"
            rel="noopener noreferrer"
            className="inline-flex items-center gap-2 bg-green-500 hover:bg-green-600 text-white px-8 py-4 rounded-xl font-bold text-lg transition-all shadow-lg"
          >
            Ask Our Experts on WhatsApp
            <ArrowRight className="w-5 h-5" />
          </Link>
        </motion.div>
      </div>
    </section>
  )
}
