'use client'

import { useReportWebVitals } from 'next/web-vitals'

export function WebVitals() {
  useReportWebVitals((metric) => {
    // Log Web Vitals to console in development
    if (process.env.NODE_ENV === 'development') {
      console.log(metric)
    }

    // Send to analytics in production
    if (typeof window !== 'undefined' && window.gtag) {
      window.gtag('event', metric.name, {
        value: Math.round(metric.name === 'CLS' ? metric.value * 1000 : metric.value),
        event_label: metric.id,
        non_interaction: true,
      })
    }

    // You can also send to your own analytics endpoint
    // fetch('/api/analytics', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'application/json' },
    //   body: JSON.stringify(metric)
    // })
  })

  return null
}

// Add TypeScript type for gtag
declare global {
  interface Window {
    gtag?: (command: string, event: string, params: Record<string, unknown>) => void
  }
}
