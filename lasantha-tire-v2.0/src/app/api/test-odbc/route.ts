import { NextResponse } from 'next/server';

export async function GET() {
  if (process.env.NODE_ENV === 'production') {
    return NextResponse.json({ success: false, error: 'Not Found' }, { status: 404 });
  }

  try {
    // Try to load odbc
    const odbc = require('odbc');
    
    return NextResponse.json({
      success: true,
      message: 'ODBC module loaded successfully',
      odbcType: typeof odbc,
      hasConnect: typeof odbc.connect === 'function'
    });
  } catch (error: any) {
    return NextResponse.json({
      success: false,
      error: error.message,
      stack: error.stack
    }, { status: 500 });
  }
}
