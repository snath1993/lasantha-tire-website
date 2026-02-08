// Smart data caching with TTL (Time To Live)
// Strategy: Always fetch fresh first, cache only as fallback

export interface CacheEntry<T> {
  data: T;
  timestamp: number;
  ttl: number; // milliseconds
}

export interface CacheOptions {
  ttl?: number; // milliseconds, default 1 hour
  key: string;
  storage?: 'localStorage' | 'sessionStorage';
}

const DEFAULT_TTL = 60 * 60 * 1000; // 1 hour

/**
 * Check if cached data is still valid
 */
function isValid<T>(entry: CacheEntry<T> | null): boolean {
  if (!entry) return false;
  const now = Date.now();
  return now - entry.timestamp < entry.ttl;
}

/**
 * Get cached data if valid
 */
export function getFromCache<T>(key: string, storage: 'localStorage' | 'sessionStorage' = 'localStorage'): T | null {
  try {
    const storageObj = storage === 'localStorage' ? localStorage : sessionStorage;
    const item = storageObj.getItem(`peachtree_cache_${key}`);
    
    if (!item) return null;
    
    const entry: CacheEntry<T> = JSON.parse(item);
    
    if (isValid(entry)) {
      return entry.data;
    } else {
      storageObj.removeItem(`peachtree_cache_${key}`);
      return null;
    }
  } catch (error) {
    console.error('[Cache] Error reading from cache:', error);
    return null;
  }
}

/**
 * Save data to cache
 */
export function saveToCache<T>(
  key: string, 
  data: T, 
  options: { ttl?: number; storage?: 'localStorage' | 'sessionStorage' } = {}
): void {
  try {
    const { ttl = DEFAULT_TTL, storage = 'localStorage' } = options;
    const storageObj = storage === 'localStorage' ? localStorage : sessionStorage;
    
    const entry: CacheEntry<T> = {
      data,
      timestamp: Date.now(),
      ttl
    };
    
    storageObj.setItem(`peachtree_cache_${key}`, JSON.stringify(entry));
  } catch (error) {
    console.error('[Cache] Error saving to cache:', error);
  }
}

/**
 * Get cache age in seconds
 */
export function getCacheAge(key: string, storage: 'localStorage' | 'sessionStorage' = 'localStorage'): number | null {
  try {
    const storageObj = storage === 'localStorage' ? localStorage : sessionStorage;
    const item = storageObj.getItem(`peachtree_cache_${key}`);
    
    if (!item) return null;
    
    const entry: CacheEntry<any> = JSON.parse(item);
    return Math.round((Date.now() - entry.timestamp) / 1000);
  } catch (error) {
    console.error('[Cache] Error getting cache age:', error);
    return null;
  }
}

/**
 * Clear specific cache entry
 */
export function clearCache(key: string, storage: 'localStorage' | 'sessionStorage' = 'localStorage'): void {
  try {
    const storageObj = storage === 'localStorage' ? localStorage : sessionStorage;
    storageObj.removeItem(`peachtree_cache_${key}`);
    console.log(`[Cache] Cleared key: ${key}`);
  } catch (error) {
    console.error('[Cache] Error clearing cache:', error);
  }
}

/**
 * Clear all Peachtree caches
 */
export function clearAllCaches(storage: 'localStorage' | 'sessionStorage' = 'localStorage'): void {
  try {
    const storageObj = storage === 'localStorage' ? localStorage : sessionStorage;
    const keys = Object.keys(storageObj);
    
    const cacheKeys = keys.filter(key => key.startsWith('peachtree_cache_'));
    cacheKeys.forEach(key => storageObj.removeItem(key));
    
    console.log(`[Cache] Cleared ${cacheKeys.length} cache entries`);
  } catch (error) {
    console.error('[Cache] Error clearing all caches:', error);
  }
}

/**
 * Fetch data with caching fallback
 * Strategy: Always try fresh first, use cache only on error
 */
export async function fetchWithCache<T>(
  key: string,
  fetchFn: () => Promise<T>,
  options: {
    ttl?: number;
    storage?: 'localStorage' | 'sessionStorage';
    onStale?: (cacheAge: number) => void;
  } = {}
): Promise<{ data: T; fromCache: boolean; cacheAge?: number }> {
  const { ttl = DEFAULT_TTL, storage = 'localStorage', onStale } = options;

  try {
    // 1. Always try to fetch fresh data first
    console.log(`[Cache] Fetching fresh data for key: ${key}`);
    const freshData = await fetchFn();
    
    // 2. Save to cache for future fallback
    saveToCache(key, freshData, { ttl, storage });
    
    return { data: freshData, fromCache: false };
  } catch (error) {
    console.error(`[Cache] Fetch failed for key: ${key}, trying cache...`, error);
    
    // 3. On error, try to use cached data as fallback
    const cachedData = getFromCache<T>(key, storage);
    
    if (cachedData) {
      const cacheAge = getCacheAge(key, storage);
      console.log(`[Cache] Using cached fallback for key: ${key}, age: ${cacheAge}s`);
      
      if (cacheAge && onStale) {
        onStale(cacheAge);
      }
      
      return { data: cachedData, fromCache: true, cacheAge: cacheAge || undefined };
    }
    
    // 4. No cache available, re-throw error
    console.error(`[Cache] No cache available for key: ${key}`);
    throw error;
  }
}

/**
 * Format cache age for display
 */
export function formatCacheAge(seconds: number): string {
  if (seconds < 60) {
    return `${seconds}s ago`;
  } else if (seconds < 3600) {
    return `${Math.round(seconds / 60)}m ago`;
  } else {
    return `${Math.round(seconds / 3600)}h ago`;
  }
}
