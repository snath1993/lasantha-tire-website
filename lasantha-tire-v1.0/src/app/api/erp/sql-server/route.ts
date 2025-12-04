import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';

const config = {
  user: process.env.DB_USER || 'sa',
  password: process.env.DB_PASSWORD || 'Sn@th@2024',
  server: process.env.DB_SERVER || 'WIN-JIAVRTFMA0N',
  database: process.env.DB_NAME || 'LasanthaTire',
  options: {
    encrypt: false,
    trustServerCertificate: true,
    enableArithAbort: true,
  },
  requestTimeout: 30000,
};

export async function GET() {
  let pool;
  
  try {
    // Connect to SQL Server
    pool = await sql.connect(config);
    
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
  } finally {
    if (pool) {
      await pool.close();
    }
  }
}
