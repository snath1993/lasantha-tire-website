import { NextResponse } from 'next/server';
import axios from 'axios';

export async function POST(request: Request) {
  try {
    const body = await request.json();
    const { jobId } = body;

    // Proxy to the backend bot running on port 8585
    const response = await axios.post('http://localhost:8585/api/jobs/run', {
      jobId
    });

    return NextResponse.json(response.data);
  } catch (error) {
    console.error('Error running job:', error);
    return NextResponse.json(
      { error: 'Failed to run job' },
      { status: 500 }
    );
  }
}
