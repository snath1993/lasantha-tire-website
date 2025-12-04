/**
 * Peachtree ERP Data API
 * Connect to Peachtree on WIN-JIAVRTFMA0N via bridge server
 */

import { NextResponse } from 'next/server';

// The Python ODBC bridge runs as a separate process (32-bit Python) on port 5000
// Default to localhost:5000 so requests are forwarded to the Flask bridge, not this Next server.
const PEACHTREE_BRIDGE = process.env.PEACHTREE_BRIDGE_URL || 'http://localhost:5000';

export async function GET(request: Request) {
  const url = new URL(request.url);
  const { searchParams } = url;
  
  if (searchParams.get('action') === 'info') {
    return getPeachtreeInfo();
  }

  const endpoint = searchParams.get('endpoint') || 'tables';

  // Prefer Direct ODBC for core endpoints; fall back to Python bridge for others
  const directEndpoints = new Set(['health', 'customers', 'vendors', 'tables']);

  try {
    if (directEndpoints.has(endpoint)) {
      // Forward to new Direct ODBC route within the same server
      const base = url.origin;
      const directUrl = `${base}/api/peachtree-direct?${searchParams.toString()}`;

      try {
        const resp = await fetch(directUrl, { headers: { 'Content-Type': 'application/json' } });
        if (resp.ok) {
          const data = await resp.json();
          return NextResponse.json(data, { status: resp.status });
        }

        console.warn(`[Peachtree API] Direct route "${endpoint}" failed with ${resp.status}. Falling back to bridge...`);
      } catch (directError) {
        console.warn(`[Peachtree API] Direct route "${endpoint}" error. Falling back to bridge...`, directError);
      }

      // Bridge fallback for direct endpoints (customers, vendors, tables, health)
      const fallbackUrl = endpoint === 'health'
        ? `${PEACHTREE_BRIDGE}/health`
        : `${PEACHTREE_BRIDGE}/api/peachtree/${endpoint}`;

      // Add query parameters to fallback URL
      const fallbackUrlWithParams = new URL(fallbackUrl);
      searchParams.forEach((value, key) => {
        if (key !== 'endpoint') {
          fallbackUrlWithParams.searchParams.append(key, value);
        }
      });

      const fallbackResp = await fetch(fallbackUrlWithParams.toString(), { headers: { 'Content-Type': 'application/json' } });
      const fallbackData = await fallbackResp.json();
      return NextResponse.json(
        { ...fallbackData, mode: 'Bridge Fallback', bridgeUrl: fallbackUrlWithParams.toString() },
        { status: fallbackResp.status }
      );
    }

    // Legacy/special endpoints still go through the Python bridge
    let bridgeUrl: string;
    if (endpoint === 'outstanding-report') {
      const reportType = searchParams.get('type') || 'all';
      bridgeUrl = `${PEACHTREE_BRIDGE}/api/peachtree/reports/outstanding?type=${reportType}`;
    } else if (endpoint.startsWith('business-status/')) {
      bridgeUrl = `${PEACHTREE_BRIDGE}/api/peachtree/${endpoint}`;
      // Append query params for business status endpoints (e.g. limit)
      const bridgeUrlObj = new URL(bridgeUrl);
      searchParams.forEach((value, key) => {
        if (key !== 'endpoint') {
          bridgeUrlObj.searchParams.append(key, value);
        }
      });
      bridgeUrl = bridgeUrlObj.toString();
    } else {
      // Generic passthrough
      bridgeUrl = endpoint === 'health' ? `${PEACHTREE_BRIDGE}/health` : `${PEACHTREE_BRIDGE}/api/peachtree/${endpoint}`;
      // Append query params
      const bridgeUrlObj = new URL(bridgeUrl);
      searchParams.forEach((value, key) => {
        if (key !== 'endpoint') {
          bridgeUrlObj.searchParams.append(key, value);
        }
      });
      bridgeUrl = bridgeUrlObj.toString();
    }

    const response = await fetch(bridgeUrl, { headers: { 'Content-Type': 'application/json' } });
    const data = await response.json();
    return NextResponse.json(data, { status: response.status });

  } catch (error: any) {
    return NextResponse.json({
      success: false,
      error: error.message || 'Connection failed',
      peachtreeServer: 'WIN-JIAVRTFMA0N',
      bridgeUrl: PEACHTREE_BRIDGE,
      bridgeStatus: 'unreachable'
    }, { status: 500 });
  }
}

export async function POST(request: Request) {
  let body;
  try {
    body = await request.json();
  } catch (e) {
    return NextResponse.json({ success: false, error: 'Invalid JSON body' }, { status: 400 });
  }

  try {
    // Prefer Direct ODBC POST for custom queries
    const base = new URL(request.url).origin;

    const resp = await fetch(`${base}/api/peachtree-direct`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });

    const data = await resp.json();
    return NextResponse.json(data, { status: resp.status });

  } catch (error: any) {
    // Fallback to bridge if direct fails
    try {
      // Use the body we already parsed
      const response = await fetch(`${PEACHTREE_BRIDGE}/api/peachtree/query`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });
      const data = await response.json();
      return NextResponse.json(data, { status: response.status });
    } catch (e2: any) {
      return NextResponse.json({ success: false, error: e2.message || 'Query failed' }, { status: 500 });
    }
  }
}

async function getPeachtreeInfo() {
  try {
    const [healthRes, tablesRes] = await Promise.all([
      fetch(`${PEACHTREE_BRIDGE}/health`),
      fetch(`${PEACHTREE_BRIDGE}/api/peachtree/tables`)
    ]);

    const healthData = await healthRes.json();
    const tablesData = await tablesRes.json();
    
    return NextResponse.json({
      success: true,
      peachtreeServer: 'WIN-JIAVRTFMA0N',
      database: healthData.odbc_dsn || 'Peachtree',
      connected: healthData.connected || false,
      tables: { count: tablesData.count || 0, list: tablesData.tables || [] },
      bridgeUrl: PEACHTREE_BRIDGE
    });
  } catch (error: any) {
    return NextResponse.json({
      success: false,
      error: error.message || 'Bridge unreachable',
      connected: false
    });
  }
}

async function getPeachtreeInventory() {
  // Try common Peachtree inventory table names
  const tableNames = ['Inventory', 'Item', 'ItemMaster', 'Product', 'InventoryItem'];
  
  for (const tableName of tableNames) {
    try {
  // Use the peachtree-prefixed query endpoint exposed by the Python bridge
  const response = await fetch(`${PEACHTREE_BRIDGE}/api/peachtree/query`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ query: `SELECT TOP 100 * FROM ${tableName}` })
      });
      
      if (response.ok) {
        const data = await response.json();
        return NextResponse.json({
          success: true,
          tableName,
          inventory: data.data || [],
          rowCount: data.count || 0,
          peachtreeServer: 'WIN-JIAVRTFMA0N'
        });
      }
    } catch {
      continue; // Try next table name
    }
  }
  
  return NextResponse.json({
    error: 'No inventory table found',
    triedTables: tableNames,
    peachtreeServer: 'WIN-JIAVRTFMA0N'
  }, { status: 404 });
}

async function getPeachtreeCustomers() {
  const tableNames = ['Customer', 'Customers', 'AR_Customer', 'ARCustomer'];
  
  for (const tableName of tableNames) {
    try {
  const response = await fetch(`${PEACHTREE_BRIDGE}/api/peachtree/query`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ query: `SELECT TOP 100 * FROM ${tableName}` })
      });
      
      if (response.ok) {
        const data = await response.json();
        return NextResponse.json({
          success: true,
          tableName,
          customers: data.data || [],
          rowCount: data.count || 0,
          peachtreeServer: 'WIN-JIAVRTFMA0N'
        });
      }
    } catch {
      continue;
    }
  }
  
  return NextResponse.json({
    error: 'No customer table found',
    triedTables: tableNames,
    peachtreeServer: 'WIN-JIAVRTFMA0N'
  }, { status: 404 });
}

async function getPeachtreeSales() {
  const tableNames = ['Sales', 'SalesOrder', 'Invoice', 'Transaction', 'SalesTransaction'];
  
  for (const tableName of tableNames) {
    try {
  const response = await fetch(`${PEACHTREE_BRIDGE}/api/peachtree/query`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ query: `SELECT TOP 100 * FROM ${tableName} ORDER BY 1 DESC` })
      });
      
      if (response.ok) {
        const data = await response.json();
        return NextResponse.json({
          success: true,
          tableName,
          sales: data.data || [],
          rowCount: data.count || 0,
          peachtreeServer: 'WIN-JIAVRTFMA0N'
        });
      }
    } catch {
      continue;
    }
  }
  
  return NextResponse.json({
    error: 'No sales table found',
    triedTables: tableNames,
    peachtreeServer: 'WIN-JIAVRTFMA0N'
  }, { status: 404 });
}

async function executePeachtreeQuery(sql: string) {
  const response = await fetch(`${PEACHTREE_BRIDGE}/api/peachtree/query`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ query: sql })
  });
  
  const data = await response.json();
  
  return NextResponse.json({
    success: data.success || false,
    data: data.data || [],
    rowCount: data.count || 0,
    error: data.error,
    peachtreeServer: 'WIN-JIAVRTFMA0N',
    query: sql
  });
}
