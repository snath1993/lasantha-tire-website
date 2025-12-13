'use client';

import { createContext, useContext, useState, ReactNode } from 'react';

interface BookingItem {
  ItemId: string;
  Description: string;
  Brand: string;
  Price?: number;
  SellingPrice?: number;
  Quantity: number;
}

interface BookingContextType {
  isBookingPopupOpen: boolean;
  setIsBookingPopupOpen: (isOpen: boolean) => void;
  bookingItems: BookingItem[];
  setBookingItems: (items: BookingItem[]) => void;
  bookingRefCode: string | null;
  setBookingRefCode: (refCode: string | null) => void;
  customerInfo: {
    name: string;
    phone: string;
    vehicleNumber?: string;
  };
  setCustomerInfo: (info: { name: string; phone: string; vehicleNumber?: string }) => void;
  openBookingPopup: (items: BookingItem[], refCode?: string, customerInfo?: any) => void;
  closeBookingPopup: () => void;
}

const BookingContext = createContext<BookingContextType | undefined>(undefined);

export function BookingProvider({ children }: { children: ReactNode }) {
  const [isBookingPopupOpen, setIsBookingPopupOpen] = useState(false);
  const [bookingItems, setBookingItems] = useState<BookingItem[]>([]);
  const [bookingRefCode, setBookingRefCode] = useState<string | null>(null);
  const [customerInfo, setCustomerInfo] = useState({
    name: '',
    phone: '',
    vehicleNumber: ''
  });

  const openBookingPopup = (items: BookingItem[], refCode?: string, info?: any) => {
    setBookingItems(items);
    if (refCode) setBookingRefCode(refCode);
    if (info) setCustomerInfo(info);
    setIsBookingPopupOpen(true);
  };

  const closeBookingPopup = () => {
    setIsBookingPopupOpen(false);
  };

  return (
    <BookingContext.Provider
      value={{
        isBookingPopupOpen,
        setIsBookingPopupOpen,
        bookingItems,
        setBookingItems,
        bookingRefCode,
        setBookingRefCode,
        customerInfo,
        setCustomerInfo,
        openBookingPopup,
        closeBookingPopup,
      }}
    >
      {children}
    </BookingContext.Provider>
  );
}

export function useBooking() {
  const context = useContext(BookingContext);
  if (context === undefined) {
    throw new Error('useBooking must be used within a BookingProvider');
  }
  return context;
}
