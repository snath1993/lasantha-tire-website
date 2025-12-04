import { NextResponse } from 'next/server';

export async function GET() {
  try {
    const odbc = require('odbc');
    const CONNECTION_STRING = 'DSN=Peachtree;UID=Peachtree;PWD=#Snath1';
    
    console.log('🔵 Attempting ODBC connection...');
    const connection = await odbc.connect(CONNECTION_STRING);
    console.log('✅ ODBC connection successful!');
    
    console.log('🔵 Attempting SELECT 1 test query...');
    const result = await connection.query('SELECT 1 AS test');
    console.log('✅ Query successful:', result);
    
    await connection.close();
    console.log('✅ Connection closed');
    
    return NextResponse.json({
      success: true,
      message: 'Direct ODBC connection and query successful',
      testResult: result
    });
  } catch (error: any) {
    console.error('❌ ODBC Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message,
      stack: error.stack
    }, { status: 500 });
  }
}
