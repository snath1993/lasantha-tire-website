import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Our Services | Tyre Sales, Alignment & More',
  description: 'Premium tyre sales, Italian CCD wheel alignment, expert installation, nitrogen inflation, warranty claims and hybrid battery services at Lasantha Tyre Traders, Battaramulla.',
  keywords: ['tyre services sri lanka', 'wheel alignment battaramulla', 'CCD wheel alignment', 'italian wheel alignment colombo', 'tyre installation', 'nitrogen inflation', 'hybrid battery service sri lanka', 'tyre shop pannipitiya road'],
  openGraph: {
    title: 'Services | Lasantha Tyre Traders',
    description: 'Complete tyre services: sales, alignment, installation, nitrogen inflation, warranty claims. Book your appointment today.',
    url: 'https://lasanthatyre.com/services',
  },
}

export default function ServicesLayout({ children }: { children: React.ReactNode }) {
  return <>{children}</>
}
