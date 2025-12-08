const LOCAL_FALLBACK = 'http://localhost:8585'
const PROD_FALLBACK = 'https://bot.lasanthatyre.com'

/**
 * Resolve the Bot API base URL with sensible fallbacks.
 * - Uses NEXT_PUBLIC_BOT_API_URL when available
 * - Falls back to localhost only when running on localhost
 * - Defaults to production URL otherwise
 */
export function getBotApiUrl(): string {
  const envUrl = process.env.NEXT_PUBLIC_BOT_API_URL?.trim()
  if (envUrl) {
    return envUrl
  }

  if (typeof window !== 'undefined') {
    const hostname = window.location.hostname
    if (hostname === 'localhost' || hostname === '127.0.0.1') {
      return LOCAL_FALLBACK
    }
  }

  return PROD_FALLBACK
}
