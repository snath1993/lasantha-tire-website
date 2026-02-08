import { NextResponse } from 'next/server';

// Mock data - Replace with actual SQL Server queries
export async function GET() {
  try {
    // Simulate sales data
    const salesData = {
      today: 125000,
      week: 847500,
      month: 3250000,
      topProducts: [
        { name: '195/65R15 Maxxis MA-P3', count: 24, revenue: 384000 },
        { name: '185/65R14 Duraturn Mozzo', count: 18, revenue: 216000 },
        { name: '175/70R13 Yokohama Bluearth', count: 16, revenue: 192000 },
        { name: '205/55R16 Bridgestone Turanza', count: 12, revenue: 264000 },
        { name: '215/60R16 Michelin Primacy', count: 10, revenue: 280000 }
      ]
    };

    return NextResponse.json(salesData);
  } catch (error) {
    console.error('Failed to get sales data:', error);
    return NextResponse.json(
      { error: 'Failed to fetch sales data' },
      { status: 500 }
    );
  }
}
