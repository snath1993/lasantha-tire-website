import type { Metadata, Viewport } from 'next'
import { Inter, Noto_Sans_Sinhala } from 'next/font/google'
import './globals.css'

const inter = Inter({ subsets: ['latin'] })
const notoSansSinhala = Noto_Sans_Sinhala({ 
  subsets: ['sinhala'],
  variable: '--font-sinhala',
  display: 'swap',
})

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  maximumScale: 5,
}

export const metadata: Metadata = {
  metadataBase: new URL('https://lasanthatyre.com'),
  title: {
    default: 'Lasantha Tyre Traders | Best Tyre Prices in Sri Lanka',
    template: '%s | Lasantha Tyre Traders'
  },
  description: 'Get instant tyre prices via WhatsApp. Sri Lanka\'s trusted tyre center offering premium brands, expert installation, wheel alignment, and comprehensive tyre services.',
  keywords: ['tyres sri lanka', 'tyre prices sri lanka', 'wheel alignment colombo', 'wheel alignment battaramulla', 'CCD wheel alignment sri lanka', 'tyre shop near me', 'tyre shop battaramulla', 'tyre shop pannipitiya road', 'Lasantha Tyre Traders', 'car tyres', 'van tyres', 'suv tyres', 'tyre replacement', 'wheel balancing', 'nitrogen inflation sri lanka', 'best tyre prices colombo', 'tyre dealers sri lanka', 'bridgestone sri lanka', 'michelin sri lanka', 'maxxis tyres sri lanka'],
  icons: {
    icon: [
      { url: '/images/lasantha-logo.png', type: 'image/png' },
    ],
    apple: [
      { url: '/images/lasantha-logo.png', type: 'image/png' },
    ],
  },
  alternates: {
    canonical: '/',
    languages: {
      'en-US': '/',
      'si-LK': '/si',
    },
  },
  openGraph: {
    title: 'Lasantha Tyre Traders | Instant Tyre Quotes',
    description: 'Get the best tyre prices in Sri Lanka sent directly to your WhatsApp. Premium brands, expert service.',
    url: 'https://lasanthatyre.com',
    siteName: 'Lasantha Tyre Traders',
    images: [
      {
        url: '/images/lasantha-logo.png', // Using logo as fallback
        width: 1200,
        height: 630,
        alt: 'Lasantha Tyre Traders',
      },
    ],
    locale: 'en_US',
    type: 'website',
  },
  twitter: {
    card: 'summary_large_image',
    title: 'Lasantha Tyre Traders | Best Tyre Prices in Sri Lanka',
    description: 'Get instant tyre prices via WhatsApp. Premium brands, expert installation, CCD wheel alignment.',
    images: ['/images/lasantha-logo.png'],
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      'max-video-preview': -1,
      'max-image-preview': 'large',
      'max-snippet': -1,
    },
  },
  // verification: {
  //   google: 'ADD_YOUR_GOOGLE_VERIFICATION_CODE_HERE',
  // },
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  const jsonLd = {
    "@context": "https://schema.org",
    "@type": "AutoPartsStore",
    "name": "Lasantha Tyre Traders",
    "image": "https://lasanthatyre.com/images/lasantha-logo.png",
    "description": "Sri Lanka's trusted tyre center offering premium brands, expert installation, wheel alignment, and comprehensive tyre services.",
    "address": {
      "@type": "PostalAddress",
      "streetAddress": "1035 Pannipitiya Road",
      "addressLocality": "Battaramulla",
      "postalCode": "10230",
      "addressCountry": "LK"
    },
    "geo": {
      "@type": "GeoCoordinates",
      "latitude": 6.883467,
      "longitude": 79.9330325
    },
    "url": "https://lasanthatyre.com",
    "telephone": "+94773131883",
    "contactPoint": [
      {
        "@type": "ContactPoint",
        "telephone": "+94773131883",
        "contactType": "customer service",
        "areaServed": "LK",
        "availableLanguage": ["English", "Sinhala"]
      },
      {
        "@type": "ContactPoint",
        "telephone": "+94721222509",
        "contactType": "sales",
        "areaServed": "LK"
      }
    ],
    "sameAs": [
      "https://facebook.com/lasanthatyretraders"
    ],
    "openingHoursSpecification": [
      {
        "@type": "OpeningHoursSpecification",
        "dayOfWeek": [
          "Monday",
          "Tuesday",
          "Wednesday",
          "Thursday",
          "Friday",
          "Saturday",
          "Sunday"
        ],
        "opens": "06:30",
        "closes": "21:00"
      }
    ],
    "hasOfferCatalog": {
      "@type": "OfferCatalog",
      "name": "Tyre Services",
      "itemListElement": [
        {
          "@type": "Offer",
          "itemOffered": {
            "@type": "Service",
            "name": "Premium Tyre Sales",
            "description": "Authorized dealer for Michelin, Bridgestone, Maxxis, and more."
          }
        },
        {
          "@type": "Offer",
          "itemOffered": {
            "@type": "Service",
            "name": "Italian CCD Wheel Alignment",
            "description": "TRIGON 725 PRO with 8 CCD cameras for precision alignment."
          }
        },
        {
          "@type": "Offer",
          "itemOffered": {
            "@type": "Service",
            "name": "Expert Tyre Installation",
            "description": "Touchless mounting and laser balancing by certified technicians."
          }
        },
        {
          "@type": "Offer",
          "itemOffered": {
            "@type": "Service",
            "name": "Nitrogen Inflation",
            "description": "Better pressure retention and improved fuel efficiency."
          }
        }
      ]
    },
    "priceRange": "$$",
    "aggregateRating": {
      "@type": "AggregateRating",
      "ratingValue": "4.8",
      "reviewCount": "500",
      "bestRating": "5"
    }
  }

  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{ __html: JSON.stringify(jsonLd) }}
        />
      </head>
      <body className={`${inter.className} ${notoSansSinhala.variable}`} suppressHydrationWarning>{children}</body>
    </html>
  )
}
