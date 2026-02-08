/**
 * Quotation Share Helper
 * Handles saving quotations to the bot API and generating booking links
 */

// Bot API URL from environment variable
// const BOT_API_URL = process.env.NEXT_PUBLIC_BOT_API_URL || 'http://localhost:8585';

export interface QuotationItemPayload {
  itemId: string;
  description: string;
  brand?: string;
  price: number;
  quantity: number;
  isService?: boolean;
}

export interface SaveQuotationPayload {
  customerPhone?: string;
  customerName?: string;
  tyreSize?: string;
  items: QuotationItemPayload[];
  totalAmount: number;
  messageContent: string;
  source?: string;
}

export interface SaveQuotationResult {
  refCode: string;
  quotationNumber: string;
  bookingUrl: string;
}

const CANONICAL_BOOKING_ORIGIN = 'https://book.lasanthatyre.com';

const buildCanonicalBookingUrl = (input: {
  bookingUrl?: unknown;
  quotationNumber?: unknown;
  refCode?: unknown;
}): string => {
  const reference = (input.quotationNumber || input.refCode || '').toString().trim();
  if (reference) {
    return `${CANONICAL_BOOKING_ORIGIN}?ref=${encodeURIComponent(reference)}`;
  }
  const raw = (input.bookingUrl || '').toString().trim();
  return raw || CANONICAL_BOOKING_ORIGIN;
};

export const normalizePhoneNumber = (input?: string) => {
  if (!input) return '';
  return input.replace(/\D/g, '');
};

export const formatPhoneForWhatsApp = (input?: string) => {
  const digits = normalizePhoneNumber(input);
  if (!digits) return '';
  if (digits.startsWith('94')) return digits;
  if (digits.startsWith('0')) return `94${digits.slice(1)}`;
  return digits.length === 9 ? `94${digits}` : digits;
};

export const saveQuotationToDb = async (
  payload: SaveQuotationPayload
): Promise<SaveQuotationResult | null> => {
  try {
    // Use the Next.js proxy route to avoid CORS/Network issues on mobile
    // The proxy is at /api/bot-proxy and it forwards to the bot's /api/quotations
    const endpoint = '/api/bot-proxy';
    console.log('📤 Saving quotation to proxy:', endpoint);
    
    const response = await fetch(endpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...payload,
        customerPhone: normalizePhoneNumber(payload.customerPhone),
        source: payload.source || 'Mobile App',
      }),
    });

    if (!response.ok) {
      console.error('❌ Failed to save quotation:', response.status);
      return null;
    }

    const data = await response.json();
    console.log('✅ Quotation API response:', data);
    
    if (data?.ok) {
      return {
        refCode: data.refCode,
        quotationNumber: data.quotationNumber,
        bookingUrl: buildCanonicalBookingUrl({
          bookingUrl: data.bookingUrl,
          quotationNumber: data.quotationNumber,
          refCode: data.refCode,
        }),
      };
    }

    return null;
  } catch (error) {
    console.error('❌ Error saving quotation:', error);
    return null;
  }
};
