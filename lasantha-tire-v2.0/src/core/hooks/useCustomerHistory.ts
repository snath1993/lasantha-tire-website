'use client';

import { useState, useEffect, useCallback } from 'react';

const STORAGE_KEY = 'lasantha-tire-customers';
const MAX_CUSTOMERS = 50;

export interface CustomerEntry {
  name: string;
  phone?: string;
  vehicleNo?: string;
  lastUsed: number;
}

/**
 * Persist recently-used customer details to localStorage
 * for auto-complete in quotation forms.
 */
export function useCustomerHistory() {
  const [customers, setCustomers] = useState<CustomerEntry[]>([]);

  // Load on mount
  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) setCustomers(JSON.parse(raw));
    } catch {
      localStorage.removeItem(STORAGE_KEY);
    }
  }, []);

  /** Save/update a customer (deduplicates by name) */
  const saveCustomer = useCallback((entry: Omit<CustomerEntry, 'lastUsed'>) => {
    if (!entry.name || entry.name.trim().length < 2) return;
    const trimmedName = entry.name.trim();

    setCustomers((prev) => {
      // Find existing by name (case-insensitive)
      const filtered = prev.filter(
        (c) => c.name.toLowerCase() !== trimmedName.toLowerCase()
      );
      const updated: CustomerEntry[] = [
        {
          name: trimmedName,
          phone: entry.phone?.trim() || undefined,
          vehicleNo: entry.vehicleNo?.trim() || undefined,
          lastUsed: Date.now(),
        },
        ...filtered,
      ].slice(0, MAX_CUSTOMERS);

      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      } catch { /* noop */ }
      return updated;
    });
  }, []);

  /** Search customers by name prefix (case-insensitive) */
  const searchCustomers = useCallback(
    (query: string): CustomerEntry[] => {
      if (!query || query.length < 1) return [];
      const q = query.toLowerCase();
      return customers
        .filter((c) => c.name.toLowerCase().includes(q))
        .slice(0, 5);
    },
    [customers]
  );

  return { customers, saveCustomer, searchCustomers };
}
