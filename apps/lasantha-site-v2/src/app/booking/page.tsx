'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'

export default function BookingRedirect() {
  const router = useRouter()

  useEffect(() => {
    router.replace('/book')
  }, [router])

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <p className="text-gray-500 animate-pulse">Redirecting to booking...</p>
    </div>
  )
}
