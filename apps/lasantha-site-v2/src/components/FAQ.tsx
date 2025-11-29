'use client'

import { motion } from 'framer-motion'
import { useInView } from 'framer-motion'
import { useRef, useState } from 'react'
import { ChevronDown, HelpCircle } from 'lucide-react'

const faqs = [
  {
    question: 'What types of tyres do you sell?',
    answer: 'We stock a wide range of tyres including passenger car tyres, SUV tyres, van tyres, and light truck tyres from premium brands like Michelin, Bridgestone, Continental, Goodyear, and many more.'
  },
  {
    question: 'Do you provide tyre installation service?',
    answer: 'Yes! We offer professional tyre installation by certified technicians. The service includes mounting, balancing, and valve replacement. Installation is typically completed within 30-60 minutes.'
  },
  {
    question: 'How often should I replace my tyres?',
    answer: 'Tyres should be replaced when the tread depth reaches 1.6mm (legal minimum) or after 5-6 years regardless of wear. However, if you notice uneven wear, cracks, bulges, or frequent air loss, replace them immediately.'
  },
  {
    question: 'Do you offer wheel alignment services?',
    answer: 'Yes, we provide precision wheel alignment using advanced equipment. Proper alignment improves tyre life, fuel efficiency, and vehicle handling. We recommend alignment every 10,000 km or if you notice uneven tyre wear.'
  },
  {
    question: 'What warranty do you provide on tyres?',
    answer: 'All our tyres come with manufacturer warranty covering manufacturing defects. Premium brands typically offer 3-5 year warranties. We also provide installation warranty for workmanship.'
  },
  {
    question: 'How can I check my tyre size?',
    answer: 'Your tyre size is printed on the sidewall (e.g., 195/65R15). You can also find it in your vehicle manual, inside the driver door jamb, or fuel filler cap. Use our tyre size calculator on this website for comparisons.'
  },
  {
    question: 'Do you offer home delivery or mobile service?',
    answer: 'Currently, we recommend visiting our shop for professional installation. However, for bulk orders or special requirements, please contact us to discuss delivery options.'
  },
  {
    question: 'What payment methods do you accept?',
    answer: 'We accept cash, bank transfers, and all major credit/debit cards. For corporate clients, we also offer credit payment terms subject to approval.'
  },
  {
    question: 'How do I know if I need new tyres?',
    answer: 'Check for: 1) Tread depth below 3mm, 2) Visible cracks or cuts, 3) Bulges or blisters, 4) Uneven wear patterns, 5) Age over 5 years, 6) Frequent air pressure loss. If unsure, visit us for a free inspection.'
  },
  {
    question: 'Can I mix different tyre brands on my vehicle?',
    answer: 'While possible, it\'s not recommended. Different brands have different performance characteristics. Ideally, all four tyres should be the same brand and model. At minimum, front and rear pairs should match.'
  }
]

function FAQItem({ faq, index }: { faq: typeof faqs[0], index: number }) {
  const [isOpen, setIsOpen] = useState(false)
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <motion.div
      ref={ref}
      initial={{ opacity: 0, y: 20 }}
      animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
      transition={{ duration: 0.5, delay: index * 0.05 }}
      className="bg-white rounded-xl shadow-md hover:shadow-lg transition-all overflow-hidden"
    >
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="w-full px-6 py-5 flex items-center justify-between text-left hover:bg-gray-50 transition-colors"
      >
        <span className="font-semibold text-gray-900 pr-4">{faq.question}</span>
        <motion.div
          animate={{ rotate: isOpen ? 180 : 0 }}
          transition={{ duration: 0.3 }}
          className="flex-shrink-0"
        >
          <ChevronDown className="w-5 h-5 text-primary-600" />
        </motion.div>
      </button>
      <motion.div
        initial={false}
        animate={{
          height: isOpen ? 'auto' : 0,
          opacity: isOpen ? 1 : 0
        }}
        transition={{ duration: 0.3 }}
        className="overflow-hidden"
      >
        <div className="px-6 pb-5 pt-2">
          <p className="text-gray-600 leading-relaxed">{faq.answer}</p>
        </div>
      </motion.div>
    </motion.div>
  )
}

export default function FAQ() {
  const ref = useRef(null)
  const isInView = useInView(ref, { once: true })

  return (
    <section id="faq" className="py-20 bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6 }}
          className="text-center mb-16"
        >
          <div className="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-full mb-4">
            <HelpCircle className="w-8 h-8 text-primary-600" />
          </div>
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Frequently Asked <span className="text-primary-600">Questions</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Find answers to common questions about our tyres and services
          </p>
        </motion.div>

        <div className="max-w-4xl mx-auto space-y-4">
          {faqs.map((faq, index) => (
            <FAQItem key={index} faq={faq} index={index} />
          ))}
        </div>

        {/* Still have questions? */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 30 }}
          transition={{ duration: 0.6, delay: 0.5 }}
          className="max-w-4xl mx-auto mt-12 bg-gradient-to-r from-primary-600 to-primary-700 rounded-2xl p-8 text-center text-white shadow-xl"
        >
          <h3 className="text-2xl font-bold mb-3">Still Have Questions?</h3>
          <p className="text-primary-100 mb-6">
            Can&apos;t find what you&apos;re looking for? Contact us directly and we&apos;ll be happy to help!
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            <a
              href="https://wa.me/94721222509"
              target="_blank"
              rel="noopener noreferrer"
              className="bg-white text-primary-600 px-8 py-3 rounded-lg font-semibold hover:bg-primary-50 transition-colors inline-flex items-center gap-2"
            >
              📱 WhatsApp Us
            </a>
            <a
              href="tel:+94721222509"
              className="bg-primary-500 text-white px-8 py-3 rounded-lg font-semibold hover:bg-primary-400 transition-colors inline-flex items-center gap-2 border-2 border-white"
            >
              📞 Call Us
            </a>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
