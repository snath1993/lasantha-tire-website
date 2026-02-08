import React, { useState, useEffect } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';

interface CalendarViewProps {
  selectedDate: string;
  onSelectDate: (date: string) => void;
}

export const CalendarView: React.FC<CalendarViewProps> = ({ selectedDate, onSelectDate }) => {
  const [currentMonth, setCurrentMonth] = useState(new Date());

  // Initialize current month based on selectedDate or today
  useEffect(() => {
    if (selectedDate) {
      setCurrentMonth(new Date(selectedDate));
    }
  }, []); // Only run on mount to set initial view

  const getDaysInMonth = (date: Date) => {
    const year = date.getFullYear();
    const month = date.getMonth();
    const days = new Date(year, month + 1, 0).getDate();
    return days;
  };

  const getFirstDayOfMonth = (date: Date) => {
    const year = date.getFullYear();
    const month = date.getMonth();
    return new Date(year, month, 1).getDay();
  };

  const handlePrevMonth = () => {
    setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() - 1, 1));
  };

  const handleNextMonth = () => {
    setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1));
  };

  const daysInMonth = getDaysInMonth(currentMonth);
  const firstDay = getFirstDayOfMonth(currentMonth);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const days = [];
  for (let i = 0; i < firstDay; i++) {
    days.push(null);
  }
  for (let i = 1; i <= daysInMonth; i++) {
    days.push(new Date(currentMonth.getFullYear(), currentMonth.getMonth(), i));
  }

  const isSameDay = (d1: Date, d2: Date) => {
    return d1.getFullYear() === d2.getFullYear() &&
           d1.getMonth() === d2.getMonth() &&
           d1.getDate() === d2.getDate();
  };

  const formatDate = (date: Date) => {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  };

  return (
    <div className="bg-white/5 rounded-xl border border-white/10 p-4 select-none">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-bold text-white">
          {currentMonth.toLocaleString('default', { month: 'long', year: 'numeric' })}
        </h3>
        <div className="flex gap-2">
          <button 
            onClick={handlePrevMonth}
            disabled={currentMonth.getFullYear() === today.getFullYear() && currentMonth.getMonth() === today.getMonth()}
            className="p-1 hover:bg-white/10 rounded-lg disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
          >
            <ChevronLeft className="w-5 h-5 text-gray-300" />
          </button>
          <button 
            onClick={handleNextMonth}
            className="p-1 hover:bg-white/10 rounded-lg transition-colors"
          >
            <ChevronRight className="w-5 h-5 text-gray-300" />
          </button>
        </div>
      </div>
      
      <div className="grid grid-cols-7 gap-1 mb-2">
        {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map(day => (
          <div key={day} className="text-center text-xs font-medium text-gray-500 py-1">
            {day}
          </div>
        ))}
      </div>

      <div className="grid grid-cols-7 gap-1">
        {days.map((date, index) => {
          if (!date) return <div key={`empty-${index}`} />;
          
          const isPast = date < today;
          const isSelected = selectedDate === formatDate(date);
          const isToday = isSameDay(date, today);

          return (
            <button
              key={index}
              type="button"
              onClick={() => !isPast && onSelectDate(formatDate(date))}
              disabled={isPast}
              className={`
                h-9 w-full rounded-lg text-sm font-medium transition-all relative
                ${isSelected 
                  ? 'bg-primary-600 text-white shadow-md shadow-primary-600/20' 
                  : isPast 
                    ? 'text-gray-600 cursor-not-allowed' 
                    : 'text-gray-300 hover:bg-white/10 hover:text-white'
                }
                ${isToday && !isSelected ? 'border border-primary-500/50 text-primary-400' : ''}
              `}
            >
              {date.getDate()}
              {isToday && !isSelected && (
                <div className="absolute bottom-1 left-1/2 -translate-x-1/2 w-1 h-1 bg-primary-500 rounded-full" />
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
};
