'use client';

import { useRef, useEffect } from 'react';
import { Delete, X, Check } from 'lucide-react';

interface NumericKeypadProps {
  onKeyPress: (key: string) => void;
  onBackspace: () => void;
  onClear: () => void;
  onDone: () => void;
}

// Helper component for highly responsive buttons
const KeypadButton = ({ 
  onClick, 
  children, 
  className, 
  variant = 'default' 
}: { 
  onClick?: () => void, 
  children: React.ReactNode, 
  className?: string,
  variant?: 'default' | 'action' | 'danger' | 'success'
}) => {
  const handlePointerDown = (e: React.PointerEvent) => {
    e.preventDefault(); // Prevent default touch actions like scrolling/selection
    if (navigator.vibrate) {
        try { navigator.vibrate(10); } catch {}
    }
    onClick?.();
  };

  return (
    <button
      onPointerDown={handlePointerDown}
      className={`h-16 rounded-xl text-2xl font-bold active:scale-95 active:translate-y-1 transition-all touch-none select-none ${className}`}
    >
      {children}
    </button>
  );
};

export default function NumericKeypad({ onKeyPress, onBackspace, onClear, onDone }: NumericKeypadProps) {
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const onBackspaceRef = useRef(onBackspace);

  useEffect(() => {
    onBackspaceRef.current = onBackspace;
  }, [onBackspace]);

  const startBackspace = (e: React.PointerEvent) => {
    e.preventDefault();
    if (navigator.vibrate) try { navigator.vibrate(10); } catch {}
    
    if (intervalRef.current) return;
    onBackspaceRef.current();
    intervalRef.current = setInterval(() => {
      if (navigator.vibrate) try { navigator.vibrate(5); } catch {}
      onBackspaceRef.current();
    }, 150); // Faster repeat
  };

  const stopBackspace = () => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
  };

  // Base styles
  const numStyle = "bg-slate-800 text-white shadow-sm border-b-4 border-slate-950 active:border-b-0";
  const actionStyle = "bg-slate-700 text-indigo-300 border-b-4 border-slate-900 active:border-b-0";
  const dangerStyle = "bg-red-900/30 text-red-400 border-b-4 border-red-900/50 active:border-b-0 text-sm";
  const successStyle = "bg-emerald-600 text-white border-b-4 border-emerald-800 active:border-b-0 flex items-center justify-center";

  return (
    <div className="w-full select-none">
      <div className="grid grid-cols-4 gap-2">
        {/* Row 1: 7 8 9 Backspace */}
        <KeypadButton onClick={() => onKeyPress('7')} className={numStyle}>7</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('8')} className={numStyle}>8</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('9')} className={numStyle}>9</KeypadButton>
        
        <button 
          onPointerDown={startBackspace}
          onPointerUp={stopBackspace}
          onPointerLeave={stopBackspace}
          className="h-16 rounded-xl bg-slate-700 text-red-400 active:scale-95 flex items-center justify-center border-b-4 border-slate-900 active:border-b-0 active:translate-y-1 touch-none select-none transition-all"
        >
          <Delete className="w-6 h-6" />
        </button>

        {/* Row 2: 4 5 6 / */}
        <KeypadButton onClick={() => onKeyPress('4')} className={numStyle}>4</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('5')} className={numStyle}>5</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('6')} className={numStyle}>6</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('/')} className={actionStyle}>/</KeypadButton>

        {/* Row 3: 1 2 3 R */}
        <KeypadButton onClick={() => onKeyPress('1')} className={numStyle}>1</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('2')} className={numStyle}>2</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('3')} className={numStyle}>3</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('R')} className={actionStyle}>R</KeypadButton>

        {/* Row 4: CLR 0 . OK */}
        <KeypadButton onClick={onClear} className={dangerStyle}>CLR</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('0')} className={numStyle}>0</KeypadButton>
        <KeypadButton onClick={() => onKeyPress('.')} className={numStyle}>.</KeypadButton>
        <KeypadButton onClick={onDone} className={successStyle}><Check className="w-8 h-8" /></KeypadButton>
      </div>
    </div>
  );
}
