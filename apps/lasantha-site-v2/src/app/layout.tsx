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
  keywords: ['tyres sri lanka', 'tyre prices sri lanka', 'wheel alignment colombo', 'tyre shop near me', 'Lasantha Tyre Traders', 'car tyres', 'van tyres', 'suv tyres', 'tyre replacement', 'wheel balancing'],
  authors: [{ name: 'Lasantha Tyre Traders' }],
  creator: 'Lasantha Tyre Traders',
  publisher: 'Lasantha Tyre Traders',
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  manifest: '/manifest.json',
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
        url: '/images/lasantha-logo.png',
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
    description: 'Get instant tyre prices via WhatsApp. Premium brands, expert service.',
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
  verification: {
    google: 'your-google-verification-code',
  },
  appleWebApp: {
    capable: true,
    statusBarStyle: 'default',
    title: 'Lasantha Tyre',
  },
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
    "telephone": "+94721222509",
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
    "priceRange": "$$"
  }

  return (
    <html lang="en" suppressHydrationWarning>
      <head>
        <link rel="icon" href="/favicon.ico" sizes="any" />
        <link rel="apple-touch-icon" href="/apple-touch-icon.png" />
        <meta name="theme-color" content="#dc2626" />
        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{ __html: JSON.stringify(jsonLd) }}
        />
      </head>
      <body className={`${inter.className} ${notoSansSinhala.variable}`} suppressHydrationWarning>
        {children}
      </body>
    </html>
  )
}
