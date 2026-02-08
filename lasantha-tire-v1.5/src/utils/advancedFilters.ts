export interface DateRangeFilter {
  from: string;
  to: string;
}

export interface AmountRangeFilter {
  min: number;
  max: number;
}

export interface AdvancedFilters {
  dateRange?: DateRangeFilter;
  amountRange?: AmountRangeFilter;
  status?: 'all' | 'active' | 'inactive';
  hasBalance?: boolean;
  searchTerm?: string;
}

export function applyAdvancedFilters<T extends Record<string, any>>(
  data: T[],
  filters: AdvancedFilters,
  config: {
    dateField?: keyof T;
    amountField?: keyof T;
    statusField?: keyof T;
    searchFields?: (keyof T)[];
  }
): T[] {
  let filtered = [...data];

  // Date range filter
  if (filters.dateRange && config.dateField) {
    const { from, to } = filters.dateRange;
    if (from || to) {
      filtered = filtered.filter((item) => {
        const itemDate = new Date(item[config.dateField!] as string);
        if (from && itemDate < new Date(from)) return false;
        if (to && itemDate > new Date(to)) return false;
        return true;
      });
    }
  }

  // Amount range filter
  if (filters.amountRange && config.amountField) {
    const { min, max } = filters.amountRange;
    filtered = filtered.filter((item) => {
      const amount = Number(item[config.amountField!]) || 0;
      if (min !== undefined && amount < min) return false;
      if (max !== undefined && amount > max) return false;
      return true;
    });
  }

  // Status filter
  if (filters.status && filters.status !== 'all' && config.statusField) {
    filtered = filtered.filter((item) => {
      const status = String(item[config.statusField!]).toLowerCase();
      return status === filters.status;
    });
  }

  // Has balance filter
  if (filters.hasBalance !== undefined && config.amountField) {
    filtered = filtered.filter((item) => {
      const amount = Number(item[config.amountField!]) || 0;
      return filters.hasBalance ? amount > 0 : amount === 0;
    });
  }

  // Search term filter
  if (filters.searchTerm && config.searchFields) {
    const searchLower = filters.searchTerm.toLowerCase();
    filtered = filtered.filter((item) => {
      return config.searchFields!.some((field) => {
        const value = String(item[field] || '').toLowerCase();
        return value.includes(searchLower);
      });
    });
  }

  return filtered;
}
