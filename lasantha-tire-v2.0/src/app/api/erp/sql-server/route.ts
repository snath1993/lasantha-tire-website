import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';
import { getPool } from '@/core/lib/db';
import { validateSession } from '@/core/lib/session';

export async function GET(request: NextRequest) {
  // 🔒 Security: Validate Session
  const sessionId = request.headers.get('x-session-id');
  const session = validateSession(sessionId);
  if (!session) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  try {
    // Connect to SQL Server
    const pool = await getPool();
    
    // Get table count
    const tablesResult = await pool.request().query(`
      SELECT COUNT(*) as tableCount 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_TYPE = 'BASE TABLE'
    `);
    
    // Get sample customer data
    const customersResult = await pool.request().query(`
      SELECT TOP 10 * FROM customers ORDER BY customer_id DESC
    `);
    
    // Get sample inventory data
    const inventoryResult = await pool.request().query(`
      SELECT TOP 10 * FROM inventory ORDER BY item_id DESC
    `);
    
    return NextResponse.json({
      success: true,
      server: 'WIN-JIAVRTFMA0N',
      database: 'LasanthaTire',
      connected: true,
      tableCount: tablesResult.recordset[0].tableCount,
      customers: customersResult.recordset,
      inventory: inventoryResult.recordset,
    });
    
  } catch (error: any) {
    console.error('SQL Server connection error:', error);
    return NextResponse.json(
      {
        success: false,
        error: error.message,
        server: 'WIN-JIAVRTFMA0N',
        database: 'LasanthaTire',
        connected: false,
      },
      { status: 500 }
    );
  }
}
