import { NextRequest, NextResponse } from 'next/server';
import { configService } from '@/lib/config';

export async function GET() {
  try {
    const config = await configService.load();
    return NextResponse.json(config);
  } catch (error) {
    return NextResponse.json(
      { error: 'Failed to load configuration' },
      { status: 500 }
    );
  }
}

export async function PUT(request: NextRequest) {
  try {
    const body = await request.json();
    await configService.save(body);
    return NextResponse.json({ success: true, config: body });
  } catch (error) {
    return NextResponse.json(
      { error: 'Failed to save configuration' },
      { status: 500 }
    );
  }
}
