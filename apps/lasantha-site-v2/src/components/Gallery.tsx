'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import { Camera, Award, Users, Wrench, Building2, X } from 'lucide-react'

const galleryCategories = [
  { id: 'all', name: 'All', icon: Camera },
  { id: 'shop', name: 'Our Shop', icon: Building2 },
  { id: 'equipment', name: 'Equipment', icon: Wrench },
  { id: 'team', name: 'Our Team', icon: Users },
  { id: 'work', name: 'At Work', icon: Award }
]

const galleryItems = [
  // Shop Exterior & Interior
  {
    id: 1,
    category: 'shop',
    title: 'Modern Shop Front',
    titleSi: 'නවීන අංගෝපාංග සහිත ප්‍රදර්ශනාගාරය',
    description: 'Our spacious, well-organized retail space in Battaramulla',
    image: '/images/gallery/shop-front.jpg',
    placeholder: 'from-blue-400 to-blue-600'
  },
  {
    id: 2,
    category: 'shop',
    title: 'Service Bay Area',
    titleSi: 'සේවා අංගනය',
    description: 'State-of-the-art service bays with modern equipment',
    image: '/images/gallery/service-bay.jpg',
    placeholder: 'from-primary-400 to-primary-600'
  },
  {
    id: 3,
    category: 'shop',
    title: 'Customer Lounge',
    titleSi: 'පාරිභෝගික විවේක අංශය',
    description: 'Comfortable waiting area with refreshments',
    image: '/images/gallery/lounge.jpg',
    placeholder: 'from-purple-400 to-purple-600'
  },
  
  // Equipment
  {
    id: 4,
    category: 'equipment',
    title: 'TRIGON 725 PRO CCD Aligner',
    titleSi: 'TRIGON 725 PRO CCD Aligner',
    description: '8 CCD cameras for precision wheel alignment',
    image: '/images/gallery/alignment-machine.jpg',
    placeholder: 'from-green-400 to-green-600'
  },
  {
    id: 5,
    category: 'equipment',
    title: 'Laser Wheel Balancer',
    titleSi: 'Laser Wheel Balancer',
    description: 'Advanced computerized wheel balancing system',
    image: '/images/gallery/balancer.jpg',
    placeholder: 'from-orange-400 to-orange-600'
  },
  {
    id: 6,
    category: 'equipment',
    title: 'Touchless Tyre Changer',
    titleSi: 'Touchless Tyre Changer',
    description: 'Protecting your expensive rims during installation',
    image: '/images/gallery/tyre-changer.jpg',
    placeholder: 'from-red-400 to-red-600'
  },
  {
    id: 7,
    category: 'equipment',
    title: 'Nitrogen Inflation Station',
    titleSi: 'නයිට්‍රජන් වායු පිරවුම් ඒකකය',
    description: 'Pure nitrogen filling for better tyre performance',
    image: '/images/gallery/nitrogen.jpg',
    placeholder: 'from-teal-400 to-teal-600'
  },
  
  // Team
  {
    id: 8,
    category: 'team',
    title: 'Certified Technicians',
    titleSi: 'පුහුණුව ලත් වෘත්තීය කාර්මික ශිල්පීන්',
    description: 'Our expert team with years of experience',
    image: '/images/gallery/team-1.jpg',
    placeholder: 'from-indigo-400 to-indigo-600'
  },
  {
    id: 9,
    category: 'team',
    title: 'Customer Service Team',
    titleSi: 'පාරිභෝගික සේවා අංශය',
    description: 'Friendly staff ready to assist you',
    image: '/images/gallery/team-2.jpg',
    placeholder: 'from-pink-400 to-pink-600'
  },
  {
    id: 10,
    category: 'team',
    title: 'Management Team',
    titleSi: 'කළමනාකාරීත්වය',
    description: 'Leadership committed to quality service',
    image: '/images/gallery/team-3.jpg',
    placeholder: 'from-cyan-400 to-cyan-600'
  },
  
  // Work in Progress
  {
    id: 11,
    category: 'work',
    title: 'Professional Installation',
    titleSi: 'වෘත්තීය මට්ටමේ ටයර් සවි කිරීම',
    description: 'Precision tyre mounting and balancing',
    image: '/images/gallery/work-1.jpg',
    placeholder: 'from-yellow-400 to-yellow-600'
  },
  {
    id: 12,
    category: 'work',
    title: 'Wheel Alignment in Action',
    titleSi: 'Wheel Alignment පරීක්ෂාවක් අතරතුර',
    description: 'Using advanced CCD technology',
    image: '/images/gallery/work-2.jpg',
    placeholder: 'from-lime-400 to-lime-600'
  },
  {
    id: 13,
    category: 'work',
    title: 'Quality Inspection',
    titleSi: 'තත්ත්ව පරීක්ෂාව',
    description: 'Every job is thoroughly checked before delivery',
    image: '/images/gallery/work-3.jpg',
    placeholder: 'from-emerald-400 to-emerald-600'
  },
  {
    id: 14,
    category: 'work',
    title: 'Premium Tyre Showcase',
    titleSi: 'ප්‍රමුඛ පෙළේ ටයර් එකතුවක්',
    description: 'Wide selection of top brands in stock',
    image: '/images/gallery/work-4.jpg',
    placeholder: 'from-rose-400 to-rose-600'
  }
]

export default function Gallery() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })
  
  const [activeCategory, setActiveCategory] = useState('all')
  const [selectedImage, setSelectedImage] = useState<typeof galleryItems[0] | null>(null)

  const filteredItems = activeCategory === 'all' 
    ? galleryItems 
    : galleryItems.filter(item => item.category === activeCategory)

  return (
    <section id="gallery" className="py-20 bg-gradient-to-b from-gray-50 to-white">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <div className="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-full mb-4">
            <Camera className="w-8 h-8 text-primary-600" />
          </div>
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Our <span className="text-primary-600">Gallery</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Take a virtual tour of our modern facilities, advanced equipment, and professional team
          </p>
        </motion.div>

        {/* Category Filter */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
          transition={{ duration: 0.6, delay: 0.2 }}
          className="flex flex-wrap justify-center gap-4 mb-12"
        >
          {galleryCategories.map((category) => {
            const Icon = category.icon
            return (
              <button
                key={category.id}
                onClick={() => setActiveCategory(category.id)}
                className={`flex items-center gap-2 px-6 py-3 rounded-xl font-semibold transition-all ${
                  activeCategory === category.id
                    ? 'bg-primary-600 text-white shadow-lg'
                    : 'bg-white text-gray-700 hover:bg-gray-100 shadow-md'
                }`}
              >
                <Icon className="w-5 h-5" />
                {category.name}
              </button>
            )
          })}
        </motion.div>

        {/* Gallery Grid */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {filteredItems.map((item, index) => (
            <motion.div
              key={item.id}
              initial={{ opacity: 0, scale: 0.9 }}
              animate={isInView ? { opacity: 1, scale: 1 } : { opacity: 0, scale: 0.9 }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
              onClick={() => setSelectedImage(item)}
              className="group relative bg-white rounded-2xl shadow-lg overflow-hidden cursor-pointer hover:shadow-2xl transition-all duration-300"
            >
              {/* Image Placeholder with Gradient */}
              <div className={`relative h-64 bg-gradient-to-br ${item.placeholder} overflow-hidden`}>
                <div className="absolute inset-0 flex items-center justify-center">
                  <Camera className="w-16 h-16 text-white/30" />
                </div>
                {/* Overlay on Hover */}
                <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-all duration-300 flex items-center justify-center">
                  <div className="opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                    <div className="bg-white/90 backdrop-blur-sm px-4 py-2 rounded-lg font-semibold text-gray-900">
                      View Full Image
                    </div>
                  </div>
                </div>
              </div>

              {/* Content */}
              <div className="p-6">
                <h3 className="text-xl font-bold text-gray-900 mb-1 group-hover:text-primary-600 transition-colors">
                  {item.title}
                </h3>
                <p className="text-sm font-medium text-gray-500 mb-2">{item.titleSi}</p>
                <p className="text-gray-600 text-sm">{item.description}</p>
              </div>
            </motion.div>
          ))}
        </div>

        {/* Lightbox Modal */}
        {selectedImage && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 z-50 bg-black/90 backdrop-blur-sm flex items-center justify-center p-4"
            onClick={() => setSelectedImage(null)}
          >
            <button
              onClick={() => setSelectedImage(null)}
              className="absolute top-4 right-4 w-12 h-12 bg-white/10 hover:bg-white/20 rounded-full flex items-center justify-center text-white transition-colors"
            >
              <X className="w-6 h-6" />
            </button>

            <motion.div
              initial={{ scale: 0.9, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              transition={{ duration: 0.3 }}
              className="max-w-4xl w-full bg-white rounded-2xl overflow-hidden"
              onClick={(e) => e.stopPropagation()}
            >
              <div className={`relative h-96 bg-gradient-to-br ${selectedImage.placeholder}`}>
                <div className="absolute inset-0 flex items-center justify-center">
                  <Camera className="w-24 h-24 text-white/30" />
                </div>
              </div>
              <div className="p-8">
                <h3 className="text-3xl font-bold text-gray-900 mb-2">{selectedImage.title}</h3>
                <p className="text-lg font-medium text-gray-500 mb-4">{selectedImage.titleSi}</p>
                <p className="text-gray-600 text-lg">{selectedImage.description}</p>
              </div>
            </motion.div>
          </motion.div>
        )}

        {/* Visit CTA */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.8 }}
          className="mt-16 text-center"
        >
          <div className="inline-flex flex-col items-center gap-6 bg-gradient-to-br from-primary-50 to-blue-50 p-8 rounded-2xl border border-primary-100">
            <div>
              <h3 className="text-2xl font-bold text-gray-900 mb-2">Come Visit Us!</h3>
              <p className="text-gray-600">
                See our facilities in person and experience our quality service firsthand
              </p>
            </div>
            <div className="flex flex-wrap gap-4 justify-center">
              <a
                href="#contact"
                className="bg-primary-600 hover:bg-primary-700 text-white px-8 py-4 rounded-xl font-bold transition-all shadow-lg"
              >
                Get Directions
              </a>
              <a
                href="https://wa.me/94721222509?text=Hi,%20I'd%20like%20to%20visit%20your%20shop"
                target="_blank"
                rel="noopener noreferrer"
                className="bg-green-500 hover:bg-green-600 text-white px-8 py-4 rounded-xl font-bold transition-all shadow-lg"
              >
                Schedule Visit
              </a>
            </div>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
