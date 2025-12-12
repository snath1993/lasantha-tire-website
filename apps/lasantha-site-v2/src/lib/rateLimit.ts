import { NextRequest, NextResponse } from 'next/server'

interface RateLimitConfig {
  interval: number // Time window in milliseconds
  uniqueTokenPerInterval: number // Max requests per interval
}

export class RateLimiter {
  private tokenCache: Map<string, number[]>

  constructor() {
    this.tokenCache = new Map()
  }

  check(identifier: string, config: RateLimitConfig): boolean {
    const now = Date.now()
    const tokenKey = identifier

    if (!this.tokenCache.has(tokenKey)) {
      this.tokenCache.set(tokenKey, [])
    }

    const timestamps = this.tokenCache.get(tokenKey)!
    const validTimestamps = timestamps.filter((time) => now - time < config.interval)

    if (validTimestamps.length >= config.uniqueTokenPerInterval) {
      return false
    }

    validTimestamps.push(now)
    this.tokenCache.set(tokenKey, validTimestamps)

    return true
  }

  reset(identifier: string): void {
    this.tokenCache.delete(identifier)
  }
}

// Create a singleton instance
const limiter = new RateLimiter()

export async function rateLimit(
  request: NextRequest,
  config: RateLimitConfig = {
    interval: 60 * 1000, // 1 minute
    uniqueTokenPerInterval: 10, // 10 requests per minute
  }
): Promise<NextResponse | null> {
  const identifier =
    request.headers.get('x-forwarded-for') ||
    request.headers.get('x-real-ip') ||
    'anonymous'

  const allowed = limiter.check(identifier, config)

  if (!allowed) {
    return NextResponse.json(
      {
        error: 'Too many requests',
        message: 'Please slow down and try again later',
      },
      {
        status: 429,
        headers: {
          'Retry-After': String(Math.ceil(config.interval / 1000)),
          'X-RateLimit-Limit': String(config.uniqueTokenPerInterval),
          'X-RateLimit-Remaining': '0',
        },
      }
    )
  }

  return null
}

export { limiter }
