import { NextRequest } from 'next/server';

export const runtime = 'nodejs';
export const dynamic = 'force-dynamic';

export async function GET(request: NextRequest) {
  try {
    // Proxy to the Bot's SSE endpoint
    const botSseUrl = 'http://localhost:3100/sse';
    
    const response = await fetch(botSseUrl, {
      headers: {
        'Accept': 'text/event-stream',
      },
      cache: 'no-store',
      // Important: keep connection open
      signal: request.signal, 
    });

    if (!response.ok || !response.body) {
      throw new Error(`Failed to connect to bot SSE: ${response.status}`);
    }

    // Return the stream directly
    return new Response(response.body, {
      headers: {
        'Content-Type': 'text/event-stream',
        'Cache-Control': 'no-cache',
        'Connection': 'keep-alive',
      },
    });
  } catch (error: any) {
    console.error('SSE Proxy Error:', error.message);
    
    // Return a stream that sends an error event and closes
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      start(controller) {
        controller.enqueue(encoder.encode(`event: error\ndata: ${JSON.stringify({ error: 'Failed to connect to bot SSE' })}\n\n`));
        controller.close();
      }
    });

    return new Response(stream, {
      headers: {
        'Content-Type': 'text/event-stream',
        'Cache-Control': 'no-cache',
        'Connection': 'keep-alive',
      },
    });
  }
}
