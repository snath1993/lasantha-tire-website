/**
 * Shared ERP Types
 * Central type definitions for inventory, pricing, and quotation entities.
 */

// ─── Product Types ───────────────────────────────────────────────────────────

export interface GRNHistory {
  InvReferenceNo: string;
  InvoiceDate: string;
  Qty: number;
}

export interface LastGRN {
  No: string;
  Date: string;
  Qty: number;
  History?: GRNHistory[];
}

export interface TireProduct {
  ItemId: string;
  Description: string;
  Brand: string;
  Quantity: number;
  Price?: number;
  SellingPrice?: number;
  UnitCost?: number;
  LastGRN?: LastGRN | null;
}

export interface QuotationItem extends TireProduct {
  Size: string;
  UnitPrice: number;
  DiscountPercent?: number;
  Category?: string;
  isFOC?: boolean;
}

// ─── Pricing Types ───────────────────────────────────────────────────────────

export type PricingMode = 'cost_plus' | 'wholesale' | 'cash' | 'selling' | 'custom';

// ─── Service IDs ─────────────────────────────────────────────────────────────

export const SERVICE_IDS = {
  ALIGNMENT_CAR: '120',
  ALIGNMENT_JEEP: '121',
  ALIGNMENT_LORRY: '161',
  ALIGNMENT_BUS: '144',
  BALANCING: '122',
  TUBELESS_NECK: '114',
} as const;

/** All service ID values as a flat array (for quick .includes() checks) */
export const SERVICE_ID_LIST: string[] = Object.values(SERVICE_IDS);

/** Check if an ItemId is a service (not a tyre) */
export const isServiceItem = (itemId: string): boolean =>
  SERVICE_ID_LIST.includes(itemId);

/** All alignment service IDs */
export const ALIGNMENT_IDS: string[] = [
  SERVICE_IDS.ALIGNMENT_CAR,
  SERVICE_IDS.ALIGNMENT_JEEP,
  SERVICE_IDS.ALIGNMENT_LORRY,
  SERVICE_IDS.ALIGNMENT_BUS,
];

// ─── Pricing Calculator ──────────────────────────────────────────────────────

export interface CalculatePriceOptions {
  mode: PricingMode;
  customMarkup?: string;
  /** Maxxis-specific discount percentage (only for TireSearch) */
  maxxisDiscount?: number;
  /** Custom override price for this item */
  customPrice?: number;
  /** Whether the item is FOC (free of charge) */
  isFOC?: boolean;
}

/**
 * Universal price calculator for all components.
 * Rounds up to nearest Rs.50.
 */
export function calculateItemPrice(
  item: { UnitCost?: number; SellingPrice?: number; Brand?: string; Category?: string; ItemId?: string },
  options: CalculatePriceOptions
): number {
  // FOC items
  if (options.isFOC) return 0;

  // Custom override
  if (options.customPrice !== undefined) return options.customPrice;

  const cost = Number(item.UnitCost) || 0;

  // Service items: use SellingPrice
  if (item.ItemId && isServiceItem(item.ItemId)) {
    return Number(item.SellingPrice) || cost;
  }

  // Non-tyre category items: use SellingPrice
  if (item.Category && item.Category !== 'TYRES') {
    const price = Number(item.SellingPrice) || cost;
    return Math.ceil(price / 50) * 50;
  }

  // MAXXIS brand: apply discount or use cost
  const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
  if (brand === 'MAXXIS' || brand === 'MAXXIES') {
    if (options.maxxisDiscount) {
      const discounted = cost * (1 - options.maxxisDiscount / 100);
      return Math.ceil(discounted / 50) * 50;
    }
    return Math.ceil(cost / 50) * 50;
  }

  // Standard pricing modes
  let finalPrice = cost;
  switch (options.mode) {
    case 'cost_plus':
      finalPrice = cost + 500;
      break;
    case 'wholesale':
      finalPrice = cost + 1000;
      break;
    case 'cash':
      finalPrice = cost + 1500;
      break;
    case 'selling':
      finalPrice = cost + 2000;
      break;
    case 'custom':
      if (!options.customMarkup) {
        finalPrice = cost;
      } else if (options.customMarkup.endsWith('%')) {
        const pct = parseFloat(options.customMarkup.replace('%', ''));
        finalPrice = isNaN(pct) ? cost : cost + (cost * pct / 100);
      } else {
        const val = parseFloat(options.customMarkup);
        finalPrice = isNaN(val) ? cost : cost + val;
      }
      break;
    default:
      finalPrice = Number(item.SellingPrice) || (cost + 2000);
  }

  return Math.ceil(finalPrice / 50) * 50;
}

// ─── Common Tire Sizes ───────────────────────────────────────────────────────

export const COMMON_SIZES = [
  '145/70R12', '155/65R13', '155/70R13', '155/80R13', '165/65R13', '165/70R13', '175/70R13',
  '165/65R14', '165/70R14', '175/65R14', '175/70R14', '185/65R14', '185/70R14', '195/70R14',
  '175/65R15', '185/60R15', '185/65R15', '195/55R15', '195/60R15', '195/65R15', '205/65R15',
  '185/55R16', '195/50R16', '195/55R16', '205/55R16', '205/60R16', '215/60R16', '215/65R16',
  '215/70R16', '225/70R16', '235/70R16', '245/70R16', '265/70R16',
  '215/55R17', '225/60R17', '225/65R17', '265/65R17',
  '225/45R18', '235/55R18', '265/60R18',
  '195R15', '195R14', '185R14', '175R14', '165R13LT', '155R12',
  '4.00-8', '4.00-10', '4.00-12', '4.50-10', '5.00-10',
  '90/90-17', '100/90-17', '120/80-17', '140/70-17',
  '2.75-17', '3.00-17', '3.00-18', '2.75-18', '90/90-18',
  '3.50-10', '90/90-10', '90/100-10', '100/90-10', '120/70-12', '130/70-12',
] as const;
