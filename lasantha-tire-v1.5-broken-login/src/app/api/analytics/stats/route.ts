import { NextResponse } from 'next/server';

// Mock data - Replace with actual database queries
export async function GET() {
  try {
    // Simulate bot statistics
    const stats = {
      totalMessages: 15847,
      todayMessages: 234,
      activeChats: 42,
      responseTime: 2.3,
      uptime: process.uptime()
    };

    return NextResponse.json(stats);
  } catch (error) {
    console.error('Failed to get stats:', error);
    return NextResponse.json(
      { error: 'Failed to fetch statistics' },
      { status: 500 }
    );
  }
}
