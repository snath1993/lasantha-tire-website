'use client';

import { createContext, useContext, useState, ReactNode } from 'react';

interface ModalContextType {
  isResultsModalOpen: boolean;
  setIsResultsModalOpen: (isOpen: boolean) => void;
}

const ModalContext = createContext<ModalContextType | undefined>(undefined);

export function ModalProvider({ children }: { children: ReactNode }) {
  const [isResultsModalOpen, setIsResultsModalOpen] = useState(false);

  return (
    <ModalContext.Provider value={{ isResultsModalOpen, setIsResultsModalOpen }}>
      {children}
    </ModalContext.Provider>
  );
}

export function useModal() {
  const context = useContext(ModalContext);
  if (context === undefined) {
    throw new Error('useModal must be used within a ModalProvider');
  }
  return context;
}
