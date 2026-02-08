import { NextResponse } from 'next/server';

const BOT_API_URL = process.env.WHATSAPP_BOT_URL || 'http://localhost:8585';

export async function POST(request: Request) {
  try {
    const body = await request.json();

    // Normalize fields (support multiple client payload shapes)
    let {
      customerName,
      name,
      phoneNumber,
      phone,
      vehicleNumber,
      vehicleNo,
      vehicle,
      serviceType,
      services,
      date,
      appointmentDate,
      timeSlot,
      time,
      quotationRef,
      quotationRefCode,
      refId,
      quotationItems,
      notes,
      message,
    } = body || {};

    customerName = customerName || name;
    phoneNumber = phoneNumber || phone;
    vehicleNumber = vehicleNumber || vehicleNo || vehicle;
    date = date || appointmentDate;
    timeSlot = timeSlot || time;

    if (!serviceType && Array.isArray(services)) {
      serviceType = services.join(', ');
    }

    // Validate required fields (be explicit to help diagnose real-world failures)
    if (!customerName) {
      console.warn('[royal-booking-v2] Missing customerName', { body });
      return NextResponse.json({ ok: false, error: 'Customer name is required' }, { status: 400 });
    }
    if (!phoneNumber) {
      console.warn('[royal-booking-v2] Missing phoneNumber', { body });
      return NextResponse.json({ ok: false, error: 'Phone number is required' }, { status: 400 });
    }
    if (!vehicleNumber) {
      console.warn('[royal-booking-v2] Missing vehicleNumber', { body });
      return NextResponse.json({ ok: false, error: 'Vehicle number is required' }, { status: 400 });
    }
    if (!serviceType) {
      console.warn('[royal-booking-v2] Missing serviceType', { body });
      return NextResponse.json({ ok: false, error: 'Service type is required' }, { status: 400 });
    }
    if (!date) {
      console.warn('[royal-booking-v2] Missing date', { body });
      return NextResponse.json({ ok: false, error: 'Date is required' }, { status: 400 });
    }
    if (!timeSlot) {
      console.warn('[royal-booking-v2] Missing timeSlot', { body });
      return NextResponse.json({ ok: false, error: 'Time slot is required' }, { status: 400 });
    }

    const forwardBody = {
      ...body,
      customerName,
      phoneNumber,
      vehicleNumber,
      serviceType,
      appointmentDate: date,
      timeSlot,
      quotationRefCode: quotationRefCode || quotationRef || refId,
      notes: notes || message,
    };

    console.log('[royal-booking-v2] Booking request (normalized)', {
      customerName,
      phoneNumber,
      vehicleNumber,
      serviceType,
      appointmentDate: date,
      timeSlot,
    });

    // Forward to Bot API
    const response = await fetch(`${BOT_API_URL}/api/appointments/book`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(forwardBody),
    });

    const data = await response.json();

    if (!response.ok) {
      return NextResponse.json(
        { ok: false, error: data.error || 'Failed to book appointment' },
        { status: response.status }
      );
    }

    return NextResponse.json(data);
  } catch (error) {
    console.error('Booking error:', error);
    return NextResponse.json(
      { success: false, error: 'Internal server error' },
      { status: 500 }
    );
  }
}
