/**
 * Utility functions for phone number handling in the booking system
 */

/**
 * Format and clean phone number input
 * Allows only digits, spaces, and hyphens for cleaner user input
 * @param value - The raw phone number input
 * @returns Cleaned phone number string containing only digits, spaces, and hyphens
 */
export function formatPhoneNumber(value: string): string {
  return value.replace(/[^\d\s-]/g, '')
}

/**
 * Validate phone number format
 * Accepts phone numbers with 10-15 digits (general validation suitable for most phone numbers)
 * Sri Lankan mobile: typically 10 digits (0771234567) or 11 with country code (94771234567)
 * @param phone - The phone number to validate
 * @returns true if phone number has 10-15 digits (after removing spaces and hyphens), false otherwise
 */
export function isValidPhoneNumber(phone: string): boolean {
  const cleaned = phone.replace(/[\s-]/g, '')
  return /^\d{10,15}$/.test(cleaned)
}
