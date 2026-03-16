'use client';

import { useState, useEffect, useCallback } from 'react';

const STORAGE_KEY = 'lasantha-tire-search-history';
const MAX_HISTORY = 15;

export interface SearchHistoryEntry {
  size: string;        // e.g. "185/65R15"
  timestamp: number;   // Date.now()
  resultCount?: number; // how many results were found
}

/**
 * Persists recent tire size searches to localStorage.
 * Returns the last N unique searches, most recent first.
 */
export function useSearchHistory() {
  const [history, setHistory] = useState<SearchHistoryEntry[]>([]);

  // Load from localStorage on mount
  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) {
        const parsed = JSON.parse(raw) as SearchHistoryEntry[];
        setHistory(parsed);
      }
    } catch {
      // Corrupted data — start fresh
      localStorage.removeItem(STORAGE_KEY);
    }
  }, []);

  /** Add a search to history (deduplicates by size) */
  const addSearch = useCallback((size: string, resultCount?: number) => {
    if (!size || size.length < 3) return;

    setHistory((prev) => {
      // Remove existing entry for same size
      const filtered = prev.filter((e) => e.size !== size);
      const newEntry: SearchHistoryEntry = {
        size,
        timestamp: Date.now(),
        resultCount,
      };
      const updated = [newEntry, ...filtered].slice(0, MAX_HISTORY);

      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      } catch {
        // localStorage quota exceeded — silently fail
      }

      return updated;
    });
  }, []);

  /** Remove a specific entry */
  const removeSearch = useCallback((size: string) => {
    setHistory((prev) => {
      const updated = prev.filter((e) => e.size !== size);
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      } catch { /* noop */ }
      return updated;
    });
  }, []);

  /** Clear all history */
  const clearHistory = useCallback(() => {
    setHistory([]);
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch { /* noop */ }
  }, []);

  return { history, addSearch, removeSearch, clearHistory };
}
