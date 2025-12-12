import { z } from 'zod'

// Phone number validation (Sri Lankan format)
export const phoneSchema = z
  .string()
  .regex(/^(?:\+94|0)?7[0-9]{8}$/, 'Invalid Sri Lankan phone number')
  .transform((val) => {
    // Normalize to +94 format
    if (val.startsWith('0')) {
      return '+94' + val.slice(1)
    }
    if (!val.startsWith('+')) {
      return '+94' + val
    }
    return val
  })

// Tire size validation
export const tireSizeSchema = z
  .string()
  .regex(/^\d{3}\/\d{2}[A-Z]?\d{2}$/, 'Invalid tire size format (e.g., 195/65R15)')

// Quote request schema
export const quoteRequestSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters').max(100),
  phone: phoneSchema,
  tireSize: tireSizeSchema,
  quantity: z.number().min(1).max(10).optional(),
  vehicleModel: z.string().max(100).optional(),
})

// Appointment booking schema
export const appointmentSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters').max(100),
  phone: phoneSchema,
  email: z.string().email('Invalid email address').optional(),
  service: z.string().min(1, 'Please select a service'),
  date: z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Invalid date format'),
  time: z.string().regex(/^\d{2}:\d{2}$/, 'Invalid time format'),
  vehicleNumber: z.string().max(20).optional(),
  vehicleModel: z.string().max(100).optional(),
  notes: z.string().max(500).optional(),
})

// Contact form schema
export const contactSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters').max(100),
  email: z.string().email('Invalid email address'),
  phone: phoneSchema.optional(),
  subject: z.string().min(5, 'Subject must be at least 5 characters').max(200),
  message: z.string().min(10, 'Message must be at least 10 characters').max(1000),
})

// Sanitize HTML to prevent XSS
export function sanitizeInput(input: string): string {
  return input
    .replace(/[<>]/g, '') // Remove < and >
    .replace(/javascript:/gi, '') // Remove javascript: protocol
    .replace(/on\w+\s*=/gi, '') // Remove event handlers
    .trim()
}

// Validate and sanitize object
export function sanitizeObject<T extends Record<string, unknown>>(obj: T): T {
  const sanitized = {} as T
  for (const [key, value] of Object.entries(obj)) {
    if (typeof value === 'string') {
      sanitized[key as keyof T] = sanitizeInput(value) as T[keyof T]
    } else {
      sanitized[key as keyof T] = value as T[keyof T]
    }
  }
  return sanitized
}

// Export types
export type QuoteRequest = z.infer<typeof quoteRequestSchema>
export type AppointmentRequest = z.infer<typeof appointmentSchema>
export type ContactRequest = z.infer<typeof contactSchema>
