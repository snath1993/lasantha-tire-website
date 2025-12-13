/**
 * Utility functions for phone number handling
 */

/**
 * Format and clean phone number input
 * Allows only digits, spaces, and hyphens
 * @param value - The raw phone number input
 * @returns Cleaned phone number string
 */
export function formatPhoneNumber(value: string): string {
  return value.replace(/[^\d\s-]/g, '')
}

/**
 * Validate phone number format for Sri Lankan numbers
 * @param phone - The phone number to validate
 * @returns true if valid, false otherwise
 */
export function isValidPhoneNumber(phone: string): boolean {
  const cleaned = phone.replace(/[\s-]/g, '')
  return /^\d{10,15}$/.test(cleaned)
}
