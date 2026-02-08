import { NextRequest, NextResponse } from 'next/server';

export const runtime = 'nodejs';
export const dynamic = 'force-dynamic';

const BOT_API_URL = process.env.BOT_API_URL || 'http://localhost:8585';

async function proxyRequest(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  const { path } = await params;
  const proxyPath = path.join('/');
  const searchParams = request.nextUrl.searchParams.toString();
  const url = `${BOT_API_URL}/${proxyPath}${searchParams ? `?${searchParams}` : ''}`;

  try {
    const headers = new Headers(request.headers);
    headers.delete('host');
    headers.delete('connection');

    const body = ['GET', 'HEAD'].includes(request.method)
      ? undefined
      : await request.arrayBuffer();

    const response = await fetch(url, {
      method: request.method,
      headers,
      body: body ? Buffer.from(body) : undefined,
      cache: 'no-store'
    });

    // IMPORTANT: do not buffer response bodies (breaks SSE/EventSource)
    const responseHeaders = new Headers(response.headers);
    responseHeaders.delete('content-encoding');
    responseHeaders.delete('content-length');

    return new NextResponse(response.body, {
      status: response.status,
      headers: responseHeaders
    });
  } catch (error) {
    console.error(`Proxy error for ${url}:`, error);
    return NextResponse.json({ ok: false, error: 'Bot API request failed' }, { status: 502 });
  }
}

export const GET = proxyRequest;
export const POST = proxyRequest;
export const PUT = proxyRequest;
export const DELETE = proxyRequest;
export const PATCH = proxyRequest;
