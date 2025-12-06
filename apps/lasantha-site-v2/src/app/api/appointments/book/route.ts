import { NextRequest, NextResponse } from 'next/server';

export async function POST(req: NextRequest) {
    try {
        const body = await req.json();
        const { refId, selectedItemIndex, name, phone, date, time, notes } = body;

        // Validate required fields
        if (!name || !phone || !date || !time) {
            return NextResponse.json({ 
                success: false, 
                error: 'Name, phone, date, and time are required' 
            }, { status: 400 });
        }

        // Use the bot API URL from environment variable
        const botUrl = process.env.WHATSAPP_BOT_URL || 'https://bot.lasanthatyre.com';

        // Forward the request to bot API
        const response = await fetch(`${botUrl}/api/appointments/book`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                refId,
                selectedItemIndex,
                name,
                phone,
                date,
                time,
                notes
            })
        });

        const result = await response.json();

        if (!response.ok) {
            return NextResponse.json({ 
                success: false, 
                error: result.error || 'Failed to book appointment'
            }, { status: response.status });
        }

        return NextResponse.json(result);

    } catch (error: unknown) {
        console.error('Error booking appointment:', error);
        const errorMessage = error instanceof Error ? error.message : 'Internal server error';
        return NextResponse.json({ 
            success: false, 
            error: errorMessage
        }, { status: 500 });
    }
}
