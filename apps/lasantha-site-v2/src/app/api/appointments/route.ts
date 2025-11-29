import { NextRequest, NextResponse } from 'next/server'

// Bot API configuration
const BOT_API_URL = process.env.WHATSAPP_BOT_URL || 'http://localhost:8585'

export async function POST(request: NextRequest) {
  try {
    const body = await request.json()
    const { customerName, phoneNumber, serviceType, appointmentDate, timeSlot, vehicleNumber, notes } = body

    // Validate required fields
    if (!customerName || !phoneNumber || !serviceType || !appointmentDate || !timeSlot) {
      return NextResponse.json(
        { message: 'All required fields must be provided' },
        { status: 400 }
      )
    }

    // Forward the request to the local bot API
    const botResponse = await fetch(`${BOT_API_URL}/api/appointments/book`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        customerName,
        phoneNumber,
        serviceType,
        appointmentDate,
        timeSlot,
        vehicleNumber: vehicleNumber || '',
        notes: notes || ''
      })
    })

    if (!botResponse.ok) {
      const errorData = await botResponse.json().catch(() => ({ message: 'Bot API error' }))
      throw new Error(errorData.message || 'Failed to book appointment')
    }

    const data = await botResponse.json()

    return NextResponse.json({
      success: true,
      referenceNo: data.referenceNo,
      message: data.message || 'Appointment booked successfully!'
    })

  } catch (error) {
    console.error('Appointment API Error:', error)
    return NextResponse.json(
      { 
        success: false, 
        message: error instanceof Error ? error.message : 'An error occurred while booking the appointment. Please try again.' 
      },
      { status: 500 }
    )
  }
}
