import { NextRequest, NextResponse } from 'next/server'

// WhatsApp API configuration - adjust to your actual WhatsApp bot API
const WHATSAPP_BOT_URL = process.env.WHATSAPP_BOT_URL || 'https://bot.lasanthatyre.com'

export async function POST(request: NextRequest) {
  try {
    const body = await request.json()
    const { name, phone, tireSize, quantity, includeVehicle, vehicle } = body

    // Validate required fields
    if (!name || !phone || !tireSize) {
      return NextResponse.json(
        { message: 'Name, phone number, and tire size are required' },
        { status: 400 }
      )
    }

    // Clean phone number (remove spaces, dashes, etc.)
    const cleanPhone = phone.replace(/\D/g, '')

    // Ensure phone starts with country code
    let formattedPhone = cleanPhone
    if (!cleanPhone.startsWith('94') && cleanPhone.startsWith('0')) {
      formattedPhone = '94' + cleanPhone.substring(1)
    }

    // Price request prepared

    try {
      // Check if WhatsApp number is registered
      const checkResponse = await fetch(`${WHATSAPP_BOT_URL}/api/check-whatsapp`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ phone: formattedPhone })
      })

      if (!checkResponse.ok) {
        throw new Error('Failed to check WhatsApp registration')
      }

      const checkData = await checkResponse.json()

      if (!checkData.registered) {
        return NextResponse.json(
          {
            message: `⚠️ The number ${phone} is not registered on WhatsApp. Please use a registered WhatsApp number. You can also call us at 0112773231.`,
            isWarning: true
          },
          { status: 400 }
        )
      }

      // Send price request via WhatsApp bot
      const sendResponse = await fetch(`${WHATSAPP_BOT_URL}/api/send-price-request`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          to: formattedPhone,
          tireSize,
          quantity: quantity || '1',
          customerName: name,
          vehicle: includeVehicle ? vehicle : null
        })
      })

      if (!sendResponse.ok) {
        throw new Error('Failed to send WhatsApp message')
      }

      const sendData = await sendResponse.json()

      return NextResponse.json({
        success: true,
        message: `✅ Price request sent successfully! You'll receive the tire price on WhatsApp shortly.`,
        data: sendData
      })

    } catch (whatsappError) {
      console.error('WhatsApp API Error:', whatsappError)
      
      // Fallback: request received but bot offline

      return NextResponse.json({
        success: true,
        message: `✅ Your request has been received! We'll send the price to ${phone} via WhatsApp shortly.`,
        offline: true
      })
    }

  } catch (error) {
    console.error('API Error:', error)
    return NextResponse.json(
      { message: 'An error occurred while processing your request. Please try again.' },
      { status: 500 }
    )
  }
}
