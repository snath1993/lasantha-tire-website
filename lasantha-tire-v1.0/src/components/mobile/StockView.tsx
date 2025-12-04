'use client';

import { useState, useEffect } from 'react';
import { Package, Loader2, AlertCircle, ArrowDown, Filter, ChevronRight, ArrowLeft, Layers } from 'lucide-react';

interface TireProduct {
  ItemId: string;
  Description: string;
  Brand: string;
  Quantity: number;
  Price?: number;
  SellingPrice?: number;
  UnitCost?: number;
}

interface BrandInfo {
  Brand: string;
  TotalQty: number;
}

export default function StockView() {
  const [items, setItems] = useState<TireProduct[]>([]);
  const [brands, setBrands] = useState<BrandInfo[]>([]);
  const [selectedBrand, setSelectedBrand] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [error, setError] = useState('');
  const [touchStart, setTouchStart] = useState<number | null>(null);
  const [touchStartY, setTouchStartY] = useState<number | null>(null);
  const [translateX, setTranslateX] = useState(0);
  const [isDragging, setIsDragging] = useState(false);

  // Fetch available brands on mount
  useEffect(() => {
    const fetchBrands = async () => {
      try {
        const res = await fetch('/api/erp/inventory?type=brands');
        if (res.ok) {
          const data = await res.json();
          if (data.success) {
            setBrands(data.data);
          }
        }
      } catch (e) {
        console.error('Failed to fetch brands', e);
      }
    };
    fetchBrands();
  }, []);

  const fetchStock = async (pageNum: number, brand: string) => {
    if (!brand) return;
    
    try {
      setLoading(true);
      const res = await fetch(`/api/erp/inventory?type=list&page=${pageNum}&limit=50&brand=${encodeURIComponent(brand)}`);
      if (!res.ok) throw new Error('Failed to fetch stock');
      
      const data = await res.json();
      if (data.success) {
        if (pageNum === 1) {
          setItems(data.data);
        } else {
          setItems(prev => [...prev, ...data.data]);
        }
        setHasMore(data.data.length === 50);
      }
    } catch (err) {
      console.error(err);
      setError('Could not load stock data');
    } finally {
      setLoading(false);
    }
  };

  // When brand changes, reset and fetch
  useEffect(() => {
    if (selectedBrand) {
      setItems([]);
      setPage(1);
      setHasMore(true);
      setError('');
      fetchStock(1, selectedBrand);
    } else {
        setItems([]);
    }
  }, [selectedBrand]);

  const loadMore = () => {
    const nextPage = page + 1;
    setPage(nextPage);
    fetchStock(nextPage, selectedBrand);
  };

  const handleTouchStart = (e: React.TouchEvent) => {
    setTouchStart(e.targetTouches[0].clientX);
    setTouchStartY(e.targetTouches[0].clientY);
    setIsDragging(true);
  };

  const handleTouchMove = (e: React.TouchEvent) => {
    if (touchStart === null || touchStartY === null) return;
    const currentX = e.targetTouches[0].clientX;
    const currentY = e.targetTouches[0].clientY;
    const diffX = currentX - touchStart;
    const diffY = currentY - touchStartY;
    
    // Only slide if moving right AND horizontal movement is greater than vertical
    if (diffX > 0 && Math.abs(diffX) > Math.abs(diffY)) {
      setTranslateX(diffX);
    }
  };

  const handleTouchEnd = () => {
    setIsDragging(false);
    if (translateX > 100) {
      setSelectedBrand('');
    }
    setTranslateX(0);
    setTouchStart(null);
    setTouchStartY(null);
  };

  if (!selectedBrand) {
    return (
      <div className="pb-24 space-y-4">
        <div className="px-1">
          <h1 className="text-2xl font-bold text-zinc-900 mb-4">Select Brand</h1>
          <div className="grid grid-cols-1 gap-3">
            {brands.map((brand, index) => (
              <button
                key={brand.Brand}
                onClick={() => setSelectedBrand(brand.Brand)}
                className="group relative bg-white p-0 rounded-2xl border border-zinc-100 shadow-sm hover:shadow-md transition-all active:scale-[0.98] overflow-hidden"
              >
                <div className="absolute inset-0 bg-gradient-to-br from-transparent via-transparent to-zinc-50/50 opacity-0 group-hover:opacity-100 transition-opacity" />
                
                <div className="p-4 flex items-center justify-between relative z-10">
                    <div className="flex items-center gap-4">
                        <div className={`w-12 h-12 rounded-xl flex items-center justify-center shadow-sm border border-zinc-100 ${
                            index % 3 === 0 ? 'bg-blue-50 text-blue-600' :
                            index % 3 === 1 ? 'bg-emerald-50 text-emerald-600' :
                            'bg-amber-50 text-amber-600'
                        }`}>
                            <Layers size={24} strokeWidth={2.5} />
                        </div>
                        
                        <div className="text-left">
                            <h3 className="text-lg font-bold text-zinc-900 leading-tight group-hover:text-indigo-600 transition-colors">
                                {brand.Brand}
                            </h3>
                            <p className="text-xs font-medium text-zinc-500 mt-1">
                                {brand.TotalQty} Items Available
                            </p>
                        </div>
                    </div>

                    <div className="w-8 h-8 rounded-full bg-zinc-50 flex items-center justify-center group-hover:bg-indigo-50 transition-colors">
                        <ChevronRight className="text-zinc-300 group-hover:text-indigo-500" size={18} />
                    </div>
                </div>
              </button>
            ))}
          </div>
          {brands.length === 0 && (
             <div className="text-center py-12">
                <Loader2 className="w-8 h-8 text-zinc-300 mx-auto mb-3 animate-spin" />
                <p className="text-zinc-400 text-sm">Loading brands...</p>
             </div>
          )}
        </div>
      </div>
    );
  }

  return (
    <div 
      className="pb-24 space-y-4 min-h-[80vh]"
      onTouchStart={handleTouchStart}
      onTouchMove={handleTouchMove}
      onTouchEnd={handleTouchEnd}
      style={{
        transform: `translateX(${translateX}px)`,
        transition: isDragging ? 'none' : 'transform 0.3s ease-out',
        opacity: Math.max(0.5, 1 - (translateX / 400))
      }}
    >
      <div className="px-1">
        <div className="flex items-center gap-3 mb-4">
            <button 
              onClick={() => setSelectedBrand('')}
              className="p-2 -ml-2 rounded-full hover:bg-zinc-100 text-zinc-600 active:bg-zinc-200 transition-colors"
            >
                <ArrowLeft size={24} />
            </button>
            <div>
                <h1 className="text-xl font-bold text-zinc-900 leading-none">{selectedBrand}</h1>
                <span className="text-xs text-zinc-500 font-medium">{items.length} Items Loaded</span>
            </div>
        </div>
      </div>

      {error && (
        <div className="p-4 bg-rose-50 text-rose-600 rounded-xl flex items-center gap-2 text-sm">
          <AlertCircle size={16} />
          {error}
        </div>
      )}

      <div className="space-y-3">
        {items.map((item, index) => (
          <div 
            key={`${item.ItemId}-${index}`}
            className="bg-white p-4 rounded-xl border border-zinc-100 shadow-sm flex justify-between items-start"
          >
            <div>
              <div className="flex items-center gap-2 mb-1">
                <span className="px-2 py-0.5 rounded text-[10px] font-bold uppercase tracking-wider bg-zinc-100 text-zinc-500 border border-zinc-200">
                  {item.Brand}
                </span>
                <span className={`text-xs font-bold flex items-center gap-1 ${
                    item.Quantity >= 4 ? 'text-emerald-600' : 
                    item.Quantity >= 2 ? 'text-yellow-600' : 
                    'text-rose-600'
                }`}>
                    In Stock ({item.Quantity})
                </span>
              </div>
              <h4 className="text-zinc-900 font-medium text-sm leading-snug">{item.Description}</h4>
              <p className="text-xs text-zinc-400 mt-1 font-mono">{item.ItemId}</p>
            </div>
            
            <div className="text-right">
                <span className="text-[10px] text-zinc-400 font-medium uppercase tracking-wider block">
                    Selling
                </span>
                <span className="text-lg font-bold text-zinc-900">
                    Rs {(item.SellingPrice || 0).toLocaleString()}
                </span>
            </div>
          </div>
        ))}
      </div>

      {loading && (
        <div className="flex justify-center py-8">
          <Loader2 className="w-6 h-6 text-indigo-600 animate-spin" />
        </div>
      )}

      {!loading && hasMore && items.length > 0 && (
        <button 
          onClick={loadMore}
          className="w-full py-3 bg-white border border-zinc-200 text-zinc-600 font-medium rounded-xl shadow-sm hover:bg-zinc-50 transition-colors flex items-center justify-center gap-2"
        >
          Load More <ArrowDown size={16} />
        </button>
      )}
      
      {!loading && !hasMore && items.length > 0 && (
        <p className="text-center text-zinc-400 text-sm py-4">End of list</p>
      )}
      
      {!loading && items.length === 0 && !error && (
        <div className="text-center py-12">
            <Package className="w-12 h-12 text-zinc-300 mx-auto mb-3" />
            <p className="text-zinc-500">No stock items found for {selectedBrand}.</p>
        </div>
      )}
    </div>
  );
}
