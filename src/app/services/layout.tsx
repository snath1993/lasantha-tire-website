import { Metadata } from 'next'

export const metadata: Metadata = {
  title: 'Services | Lasantha Tyre Traders - Expert Tyre Services in Battaramulla',
  description: 'Premium tyre sales, expert installation, Italian CCD wheel alignment (TRIGON 725 PRO), warranty claims, and nitrogen inflation. Visit us at 1035 Pannipitiya Road, Battaramulla.',
  keywords: ['tyre services battaramulla', 'wheel alignment sri lanka', 'tyre installation', 'nitrogen inflation', 'TRIGON 725 PRO', 'CCD wheel alignment', 'tyre warranty claims'],
}

export default function ServicesLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return <>{children}</>
}
