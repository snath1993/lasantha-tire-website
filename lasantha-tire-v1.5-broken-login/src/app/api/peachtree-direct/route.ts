import { NextRequest, NextResponse } from 'next/server';
import { validateSession } from '@/lib/session';

const BRIDGE_URL = 'http://localhost:5000';

async function checkBridgeHealth() {
  try {
    const res = await fetch(`${BRIDGE_URL}/health`, { signal: AbortSignal.timeout(2000) });
    return res.ok;
  } catch (e) {
    return false;
  }
}

export async function GET(request: NextRequest) {
  // 🔒 Security: Validate Session
  const sessionId = request.headers.get('x-session-id');
  const session = validateSession(sessionId);
  if (!session) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const { searchParams } = new URL(request.url);
  const endpoint = searchParams.get('endpoint');

  // Check if bridge is running
  const isBridgeUp = await checkBridgeHealth();
  if (!isBridgeUp) {
    return NextResponse.json({
      success: false,
      error: 'Peachtree Bridge (Python) is not running on port 5000.',
      details: 'The system will attempt to start it automatically via the Dashboard.',
      mode: 'Bridge Proxy'
    }, { status: 503 });
  }

  try {
    // Health check endpoint
    if (endpoint === 'health') {
      const res = await fetch(`${BRIDGE_URL}/health`);
      const data = await res.json();
      return NextResponse.json({
        ...data,
        mode: 'Bridge Proxy'
      });
    }

    // Tables list endpoint
    if (endpoint === 'tables') {
      const res = await fetch(`${BRIDGE_URL}/api/peachtree/tables`);
      const data = await res.json();
      return NextResponse.json({
        success: data.success,
        tables: data.tables || [],
        count: data.count || 0,
        mode: 'Bridge Proxy'
      });
    }

    // Customers/Vendors
    if (endpoint === 'customers' || endpoint === 'vendors') {
      // Use bridge's dedicated endpoints which handle table name fallback
      const res = await fetch(`${BRIDGE_URL}/api/peachtree/${endpoint}`);
      
      const data = await res.json();
      return NextResponse.json({
        success: data.success,
        table: data.table,
        count: data.count,
        data: data.data,
        mode: 'Bridge Proxy'
      });
    }

    return NextResponse.json({
      success: false,
      error: 'Unknown endpoint',
      available: ['health', 'customers', 'vendors', 'tables']
    }, { status: 400 });

  } catch (error: any) {
    console.error('❌ Bridge Proxy Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Bridge proxy failed',
      mode: 'Bridge Proxy'
    }, { status: 500 });
  }
}

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { query } = body;

    if (!query) {
      return NextResponse.json({
        success: false,
        error: 'Query parameter required'
      }, { status: 400 });
    }

    const isBridgeUp = await checkBridgeHealth();
    if (!isBridgeUp) {
      return NextResponse.json({
        success: false,
        error: 'Peachtree Bridge is not running',
        mode: 'Bridge Proxy'
      }, { status: 503 });
    }

    const res = await fetch(`${BRIDGE_URL}/api/peachtree/query`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ query: query })
    });

    const data = await res.json();
    return NextResponse.json({
      success: data.success,
      data: data.data,
      count: data.count,
      mode: 'Bridge Proxy'
    });

  } catch (error: any) {
    return NextResponse.json({
      success: false,
      error: error.message || 'Bridge proxy failed',
      mode: 'Bridge Proxy'
    }, { status: 500 });
  }
}
