'use client';

import { useState, useEffect, useRef } from 'react';
import { 
  FileText, Plus, Trash2, Share2, Download, X, Search, 
  User, Truck, Calendar, Check, ChevronRight, Loader2, Tag, Edit2,
  Car, Bus, Bike, MoreHorizontal, Wrench, CircleDashed, CircleDot
} from 'lucide-react';
import { exportQuotationPDF } from '@/utils/pdfExports';
import NumericKeypad from './NumericKeypad';
import { useToast } from '@/contexts/ToastContext';
import { authenticatedFetch } from '@/lib/client-auth';

interface QuotationItem {
  ItemId: string;
  Description: string;
  Brand: string;
  Size: string;
  Quantity: number;
  UnitPrice: number;
  UnitCost: number;
  DiscountPercent?: number;
  Category?: string;
  isFOC?: boolean;
  SellingPrice?: number;
}

type PricingMode = 'cost_plus' | 'wholesale' | 'cash' | 'selling' | 'custom';

// Hardcoded Service IDs
const SERVICE_IDS = {
    ALIGNMENT_CAR: '120', // WHEEL ALIGNMENT- COMPUTERIZED CARS
    ALIGNMENT_JEEP: '121', // WHEEL ALIGNMENT- COMPUTERIZED VANS/JEEPS (1500)
    ALIGNMENT_LORRY: '161', // WHEEL ALIGNMENT LORRY (6 WHEEL ) (0) - Wait, 0 price? Maybe it's custom.
    ALIGNMENT_BUS: '144', // WHEEL ALIGNMENT- COMPUTERIZED BUS/TIPPER (0)
    BALANCING: '122', // WHEEL BALANCING
    TUBELESS_NECK: '114', // TUBE LESS NECK
    
    // Let's stick to the IDs found. If price is 0, user can edit.
};

const QuantityPicker = ({ value, onChange }: { value: number, onChange: (val: number) => void }) => {
  const scrollRef = useRef<HTMLDivElement>(null);
  const ITEM_HEIGHT = 40; // Height of each number item

  // Sync scroll with value
  useEffect(() => {
    if (scrollRef.current) {
      const targetScroll = (value - 1) * ITEM_HEIGHT;
      // Only scroll if significantly different (avoids fighting with scroll loop)
      if (Math.abs(scrollRef.current.scrollTop - targetScroll) > 1) {
          scrollRef.current.scrollTop = targetScroll;
      }
    }
  }, [value]);

  const handleScroll = () => {
    if (!scrollRef.current) return;
    const scrollTop = scrollRef.current.scrollTop;
    const index = Math.round(scrollTop / ITEM_HEIGHT);
    const newValue = Math.max(1, Math.min(10, index + 1));
    
    if (newValue !== value) {
       onChange(newValue);
    }
  };

  return (
    <div className="relative h-10 bg-slate-900/50 rounded-xl border border-slate-700 overflow-hidden w-full shadow-inner">
      <div 
        ref={scrollRef}
        onScroll={handleScroll}
        className="h-full overflow-y-auto snap-y snap-mandatory no-scrollbar"
      >
        {Array.from({ length: 10 }, (_, i) => i + 1).map((num) => (
          <div 
            key={num}
            onClick={() => onChange(num)}
            className={`h-[40px] flex items-center justify-center snap-start transition-all cursor-pointer ${
              num === value ? 'text-xl font-bold text-blue-400' : 'text-sm text-slate-600'
            }`}
          >
            {num}
          </div>
        ))}
      </div>
    </div>
  );
};

export default function QuotationView() {
  // Quotation Details
  const [vehicleNo, setVehicleNo] = useState('');
  const [customerName, setCustomerName] = useState('');
  const [terms, setTerms] = useState('Cash');
  const [defaultQuantity, setDefaultQuantity] = useState(1); // Default to 1
  const [items, setItems] = useState<QuotationItem[]>([]);
  
  // Pricing State
  const [pricingMode, setPricingMode] = useState<PricingMode>('selling');
  const [customMarkup, setCustomMarkup] = useState<string>('');
  
  // Item Editing State
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [editPrice, setEditPrice] = useState<string>('');
  
  // Selection State
  const [selectedIndices, setSelectedIndices] = useState<Set<number>>(new Set());

  // UI State
  const [showAddPopup, setShowAddPopup] = useState(false);
  const [showItemSelectPopup, setShowItemSelectPopup] = useState(false);
  const [showServicePopup, setShowServicePopup] = useState(false); // New Service Popup
  const [searchSize, setSearchSize] = useState('');
  const [searchResults, setSearchResults] = useState<any[]>([]);
  const [serviceResults, setServiceResults] = useState<any[]>([]);
  const [serviceCategories, setServiceCategories] = useState<string[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [serviceSearchQuery, setServiceSearchQuery] = useState('');
  const [selectedPopupItems, setSelectedPopupItems] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(false);
  
  const { showToast } = useToast();

  // Handle Global Quantity Change
  const handleGlobalQuantityChange = (newQty: number) => {
    setDefaultQuantity(newQty);
    setItems(prev => prev.map(item => {
        // Skip Wheel Alignment
        if (item.Category === 'WHEEL ALIGNMENT') return item;
        
        // Update others
        return { ...item, Quantity: newQty };
    }));
  };

  // Helper to calculate price based on current mode
  const calculatePrice = (item: any, mode: PricingMode = pricingMode, customVal: string = customMarkup) => {
    const cost = Number(item.UnitCost) || 0;
    let finalPrice = cost;

    // Maxxis Exception: Fixed price (Cost = Selling), no markups allowed
    const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
    if (brand === 'MAXXIS' || brand === 'MAXXIES') {
        return Math.ceil(cost / 50) * 50; 
    }
    
    // Services usually don't follow tyre pricing logic (Cost+500 etc). 
    // They usually have a fixed SellingPrice or UnitCost.
    // If Category is NOT TYRES, we should probably just return the UnitCost or SellingPrice if available.
    // However, the API returns SellingPrice = UnitCost for services currently.
    // Let's assume for Services, we just use the cost/price as is, maybe rounded.
    if (item.Category && item.Category !== 'TYRES') {
        // If item has a SellingPrice (from API), use it. Otherwise use UnitCost.
        // Note: For services, API maps PriceLevel1 to SellingPrice.
        const price = Number(item.SellingPrice) || cost;
        return Math.ceil(price / 50) * 50;
    }

    switch (mode) {
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
            if (!customVal) {
                finalPrice = cost;
            } else if (customVal.endsWith('%')) {
                const pct = parseFloat(customVal.replace('%', ''));
                finalPrice = isNaN(pct) ? cost : cost + (cost * pct / 100);
            } else {
                const val = parseFloat(customVal);
                finalPrice = isNaN(val) ? cost : cost + val;
            }
            break;
        default:
            finalPrice = Number(item.SellingPrice) || (cost + 2000);
    }
    
    return Math.ceil(finalPrice / 50) * 50;
  };

  // Fetch specific item by ID and add it
  const addServiceById = async (itemId: string) => {
    // Check if already added
    if (items.some(i => i.ItemId === itemId)) {
        showToast('error', 'Service already added');
        return;
    }

    setLoading(true);
    try {
        // Fetch the specific service item to get its latest price from DB
        const res = await authenticatedFetch(`/api/erp/inventory?type=services&itemId=${itemId}`);
        const data = await res.json();
        
        if (data.success && data.data && data.data.length > 0) {
            addItem(data.data[0]);
        } else {
            showToast('error', 'Service item not found');
        }
    } catch (e) {
        showToast('error', 'Failed to add service');
    } finally {
        setLoading(false);
    }
  };

  // Fetch Service Categories
  const fetchServiceCategories = async () => {
      try {
          const res = await authenticatedFetch('/api/erp/inventory?type=service_categories');
          const data = await res.json();
          if (data.success) {
              setServiceCategories(data.data);
              if (data.data.length > 0) setSelectedCategory(data.data[0]);
              fetchServices(data.data[0]);
          }
      } catch (e) {
          console.error(e);
      }
  };

  // Fetch Services by Category
  const fetchServices = async (category?: string, query?: string) => {
      setLoading(true);
      try {
          let url = `/api/erp/inventory?type=services`;
          if (category) url += `&category=${encodeURIComponent(category)}`;
          if (query) url += `&query=${encodeURIComponent(query)}`;
          
          const res = await authenticatedFetch(url);
          const data = await res.json();
          if (data.success) {
              setServiceResults(data.data);
          }
      } catch (e) {
          console.error(e);
      } finally {
          setLoading(false);
      }
  };

  // Open Service Popup
  const openServicePopup = () => {
      setShowServicePopup(true);
      setServiceSearchQuery(''); // Reset search
      fetchServiceCategories();
  };

  // Apply Pricing Mode
  const applyPricingMode = (mode: PricingMode, customVal: string = '') => {
    setPricingMode(mode);
    if (customVal) setCustomMarkup(customVal);

    setItems(prev => prev.map((item, index) => {
        // Check if item is Maxxis
        const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
        const isMaxxis = brand === 'MAXXIS' || brand === 'MAXXIES';
        
        if (isMaxxis) return item; // Never change Maxxis price via global mode

        // Check if item is NOT a Tyre (e.g. Service)
        if (item.Category && item.Category !== 'TYRES') {
            return item; // Never change non-tyre price via global mode
        }

        // If selection exists, only update selected items
        if (selectedIndices.size > 0 && !selectedIndices.has(index)) {
            return item;
        }

        // Calculate new price
        const newPrice = calculatePrice(item, mode, customVal);
        return { ...item, UnitPrice: newPrice };
    }));

    if (selectedIndices.size > 0) {
        showToast('success', `Updated ${selectedIndices.size} items`);
        setSelectedIndices(new Set()); // Clear selection after update
    } else {
        // showToast('success', 'Updated all items');
    }
  };

  // Search Items
  const searchItems = async (size: string) => {
    console.log('Searching for:', size);
    if (!size) return;
    setLoading(true);
    setSelectedPopupItems(new Set()); // Clear selection on new search
    try {
      console.log('Calling API...');
      const res = await authenticatedFetch(`/api/erp/inventory?type=search&query=${encodeURIComponent(size)}`);
      console.log('API Response Status:', res.status);
      
      const data = await res.json();
      console.log('API Data:', data);

      if (data.data) {
        if (data.data.length === 0) {
            showToast('error', 'No items found');
        } else {
            setSearchResults(data.data);
            setShowItemSelectPopup(true);
        }
      } else {
        showToast('error', 'Invalid response from server');
      }
    } catch (error) {
      console.error('Search Error:', error);
      showToast('error', 'Failed to search items');
    } finally {
      setLoading(false);
    }
  };

  // Keypad Handlers
  const handleKeyPress = (key: string) => {
    setSearchSize(prev => prev + key);
  };

  const handleBackspace = () => {
    setSearchSize(prev => prev.slice(0, -1));
  };

  const handleClear = () => {
    setSearchSize('');
  };

  const handleDone = () => {
    if (searchSize) {
      searchItems(searchSize);
    }
  };

  // Add Item to Quote
  const addItem = (item: any) => {
    const price = calculatePrice(item);
    
    // Use defaultQuantity for everything EXCEPT Wheel Alignment
    // User request: "wheel alignment kiyana catagory card walata nathuwa hama item card ekakatama apply wenna hadanna"
    const isAlignment = item.Category === 'WHEEL ALIGNMENT';
    const qty = isAlignment ? 1 : defaultQuantity;

    const newItem: QuotationItem = {
      ItemId: item.ItemId,
      Description: item.Description,
      Brand: item.Brand,
      Size: item.Size || searchSize,
      Quantity: qty,
      UnitPrice: price,
      UnitCost: Number(item.UnitCost) || 0,
      Category: item.Category,
      isFOC: false,
      SellingPrice: Number(item.SellingPrice) || 0
    };
    
    setItems(prev => [...prev, newItem]);
    showToast('success', 'Item added to quotation');
    setShowItemSelectPopup(false);
    setShowAddPopup(false);
    setSearchSize('');
  };

  // Toggle Popup Item Selection
  const togglePopupItem = (itemId: string) => {
    const newSelected = new Set(selectedPopupItems);
    if (newSelected.has(itemId)) {
      newSelected.delete(itemId);
    } else {
      newSelected.add(itemId);
    }
    setSelectedPopupItems(newSelected);
  };

  // Add Selected Items
  const addSelectedItems = () => {
    const itemsToAdd = searchResults.filter(item => selectedPopupItems.has(item.ItemId));
    
    if (itemsToAdd.length === 0) {
      showToast('error', 'Select items to add');
      return;
    }

    const newItems = itemsToAdd.map(item => {
        // Apply default quantity logic
        const isAlignment = item.Category === 'WHEEL ALIGNMENT';
        const qty = isAlignment ? 1 : defaultQuantity;

        return {
            ItemId: item.ItemId,
            Description: item.Description,
            Brand: item.Brand,
            Size: item.Size || searchSize,
            Quantity: qty,
            UnitPrice: calculatePrice(item),
            UnitCost: Number(item.UnitCost) || 0,
            Category: item.Category,
            isFOC: false,
            SellingPrice: Number(item.SellingPrice) || 0
        };
    });

    setItems(prev => [...prev, ...newItems]);
    showToast('success', `${itemsToAdd.length} items added`);
    setShowItemSelectPopup(false);
    setShowAddPopup(false);
    setSearchSize('');
    setSelectedPopupItems(new Set());
  };

  // Remove Item
  const removeItem = (index: number) => {
    setItems(prev => prev.filter((_, i) => i !== index));
    setSelectedIndices(prev => {
        const newSet = new Set<number>();
        prev.forEach(i => {
            if (i < index) newSet.add(i);
            if (i > index) newSet.add(i - 1);
        });
        return newSet;
    });
  };

  // Toggle Selection
  const toggleSelection = (index: number) => {
    setSelectedIndices(prev => {
        const newSet = new Set(prev);
        if (newSet.has(index)) {
            newSet.delete(index);
        } else {
            newSet.add(index);
        }
        return newSet;
    });
  };

  // Update Quantity
  const updateQuantity = (index: number, delta: number) => {
    setItems(prev => prev.map((item, i) => {
      if (i === index) {
        const newQty = Math.max(1, item.Quantity + delta);
        return { ...item, Quantity: newQty };
      }
      return item;
    }));
  };

  // Maxxis Discount Logic
  const toggleMaxxisDiscount = (index: number, percent: number) => {
    setItems(prev => prev.map((item, i) => {
        if (i === index) {
            // Toggle off if same percent clicked
            if (item.DiscountPercent === percent) {
                return { 
                    ...item, 
                    UnitPrice: item.UnitCost, // Reset to cost (Fixed Price)
                    DiscountPercent: undefined 
                };
            }

            // Apply new discount
            const discountedPrice = item.UnitCost * (1 - percent / 100);
            const roundedPrice = Math.ceil(discountedPrice / 50) * 50;
            return { 
                ...item, 
                UnitPrice: roundedPrice,
                DiscountPercent: percent
            };
        }
        return item;
    }));
  };

  // Toggle FOC
  const toggleFOC = (index: number) => {
    setItems(prev => prev.map((item, i) => {
        if (i === index) {
            const newFOC = !item.isFOC;
            return {
                ...item,
                isFOC: newFOC,
                UnitPrice: newFOC ? 0 : calculatePrice(item) // Reset to calculated price if unchecked
            };
        }
        return item;
    }));
  };

  // Manual Price Edit
  const startEditing = (index: number, currentPrice: number) => {
    setEditingIndex(index);
    setEditPrice(currentPrice.toString());
  };

  const savePrice = (index: number) => {
    const newPrice = parseFloat(editPrice);
    if (!isNaN(newPrice) && newPrice >= 0) {
        setItems(prev => prev.map((item, i) => 
            i === index ? { ...item, UnitPrice: newPrice } : item
        ));
        setEditingIndex(null);
        setEditPrice('');
    } else {
        showToast('error', 'Invalid price');
    }
  };

  // Generate PDF
  const handleGeneratePDF = async () => {
    if (items.length === 0) {
      showToast('error', 'Add items to generate quotation');
      return;
    }

    const quotationDetails = {
      vehicleNo,
      customerName,
      terms,
      date: new Date().toLocaleDateString(),
      quotationNo: `QT-${Date.now().toString().slice(-6)}`
    };

    // 1. Generate PDF
    exportQuotationPDF(quotationDetails, items);
    showToast('success', 'Quotation PDF generated');

    // 2. Save to Database & Get Short Link
    try {
        const response = await authenticatedFetch('/api/quotations/create-link', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                items: items.map(i => ({
                    description: i.Description,
                    brand: i.Brand,
                    size: i.Size,
                    price: i.UnitPrice,
                    quantity: i.Quantity,
                    category: i.Category // Pass category to help with logic
                })),
                customerName,
                customerPhone: '' 
            })
        });

        if (response.ok) {
            const data = await response.json();
            const refId = data.refId;
            
            // 3. Prepare WhatsApp Message with Smart Logic
            // Logic: Show Total if (Tyre Count <= 1) OR (Only Services)
            // Tyre Count = Items that are NOT services (assuming Category or ID check)
            // For simplicity, let's assume if Brand is present, it's a product. Services usually have empty Brand or specific category.
            
            const tyreItems = items.filter(i => i.Brand && i.Brand !== 'SERVICE'); // Adjust filter based on your data
            const serviceItems = items.filter(i => !i.Brand || i.Brand === 'SERVICE');
            
            const showTotal = tyreItems.length <= 1;
            const totalAmount = items.reduce((sum, item) => sum + (item.Quantity * item.UnitPrice), 0);

            let message = `*Lasantha Tyre Traders - Quotation*\n\n`;
            
            // Add Items
            message += items.map(i => {
                let line = `🛠️ ${i.Description}`;
                if (i.Brand) line += `\n🏷️ ${i.Brand}`;
                line += `\n💰 Rs. ${i.UnitPrice.toLocaleString()}/=`;
                if (i.Quantity > 1) line += `\nQty: ${i.Quantity} | Total: Rs. ${(i.Quantity * i.UnitPrice).toLocaleString()}`;
                return line;
            }).join('\n\n');

            // Add Grand Total if applicable
            if (showTotal) {
                message += `\n\n------------------------\n*Total: Rs. ${totalAmount.toLocaleString()}*`;
            }

            // Add Smart Link
            message += `\n\n📅 *Book your appointment now:*\nhttps://lasanthatyre.com/book?ref=${refId}\n\nRef: #${refId}`;

            // Copy to clipboard or open WhatsApp
            if (navigator.share) {
                await navigator.share({
                    title: 'Quotation',
                    text: message
                });
            } else {
                // Fallback for desktop
                await navigator.clipboard.writeText(message);
                showToast('success', 'Message copied to clipboard!');
                window.open(`https://wa.me/?text=${encodeURIComponent(message)}`, '_blank');
            }
        }
    } catch (error) {
        console.error('Error creating smart link:', error);
        // Fallback: Just share without link if API fails
        const message = `*Lasantha Tyre Traders - Quotation*\n\n` +
            items.map(i => `🛠️ ${i.Description}\n🏷️ ${i.Brand}\n💰 Rs. ${i.UnitPrice.toLocaleString()}/=`).join('\n\n');
        window.open(`https://wa.me/?text=${encodeURIComponent(message)}`, '_blank');
    }
  };

  const totalAmount = items.reduce((sum, item) => sum + (item.Quantity * item.UnitPrice), 0);

  return (
    <div className="pb-24 space-y-4 p-4">
      {/* Header */}
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-bold text-white">New Quotation</h1>
        <div className="bg-blue-500/20 text-blue-400 px-3 py-1 rounded-full text-xs font-mono">
          {new Date().toLocaleDateString()}
        </div>
      </div>

      {/* Top Card: Details */}
      <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 space-y-4">
        <div className="space-y-1">
          <label className="text-xs text-slate-400 uppercase font-semibold ml-1">Vehicle Number</label>
          <div className="relative">
            <Truck className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
            <input
              type="text"
              value={vehicleNo}
              onChange={(e) => setVehicleNo(e.target.value.toUpperCase())}
              placeholder="WP CAX-1234"
              className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 pl-10 pr-4 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none font-mono uppercase"
            />
          </div>
        </div>

        <div className="space-y-1">
          <label className="text-xs text-slate-400 uppercase font-semibold ml-1">Customer Name</label>
          <div className="relative">
            <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-500" />
            <input
              type="text"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              placeholder="Select or type name"
              className="w-full bg-slate-900/50 border border-slate-700 rounded-xl py-3 pl-10 pr-4 text-white placeholder:text-slate-600 focus:border-blue-500 focus:outline-none"
            />
          </div>
        </div>

        <div className="flex gap-4">
            <div className="flex-1 space-y-1">
              <label className="text-xs text-slate-400 uppercase font-semibold ml-1">Terms</label>
              <div className="flex gap-2">
                {['Cash', 'Credit', 'Cheque'].map((t) => (
                  <button
                    key={t}
                    onClick={() => setTerms(t)}
                    className={`flex-1 py-2 rounded-xl text-sm font-medium transition-colors ${
                      terms === t 
                        ? 'bg-blue-600 text-white shadow-lg shadow-blue-900/20' 
                        : 'bg-slate-900/50 text-slate-400 border border-slate-700'
                    }`}
                  >
                    {t}
                  </button>
                ))}
              </div>
            </div>
            
            <div className="w-16 space-y-1">
                <label className="text-xs text-slate-400 uppercase font-semibold ml-1 text-center block">Qty</label>
                <QuantityPicker value={defaultQuantity} onChange={handleGlobalQuantityChange} />
            </div>
        </div>
      </div>

      {/* Pricing Mode Card */}
      <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-5 space-y-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Tag className="w-4 h-4 text-emerald-400" />
            <h3 className="text-sm font-bold text-white">Pricing Mode</h3>
          </div>
          <span className="text-xs text-slate-400 bg-slate-900/50 px-2 py-1 rounded-lg border border-slate-700">
            {pricingMode === 'custom' ? (customMarkup || 'Custom') : pricingMode.replace('_', ' ').toUpperCase()}
          </span>
        </div>

        <div className="grid grid-cols-3 gap-2">
          {[
            { id: 'selling', label: 'Selling', sub: '+2000' },
            { id: 'cash', label: 'Cash', sub: '+1500' },
            { id: 'wholesale', label: 'Wholesale', sub: '+1000' },
            { id: 'cost_plus', label: 'Cost+', sub: '+500' },
            { id: 'custom', label: 'Custom', sub: 'Manual' }
          ].map((mode) => (
            <button
              key={mode.id}
              onClick={() => applyPricingMode(mode.id as PricingMode)}
              className={`p-2 rounded-xl border transition-all ${
                pricingMode === mode.id
                  ? 'bg-emerald-600/20 border-emerald-500 text-emerald-400'
                  : 'bg-slate-900/50 border-slate-700 text-slate-400 hover:bg-slate-800'
              }`}
            >
              <div className="text-xs font-bold">{mode.label}</div>
              <div className="text-[10px] opacity-70">{mode.sub}</div>
            </button>
          ))}
        </div>

        {pricingMode === 'custom' && (
          <div className="animate-in slide-in-from-top-2 duration-200">
            <label className="text-xs text-slate-400 uppercase font-semibold ml-1 mb-1 block">Custom Markup</label>
            <div className="flex gap-2">
              <input
                type="text"
                value={customMarkup}
                onChange={(e) => {
                    setCustomMarkup(e.target.value);
                    // Optional: Apply immediately or wait for button? 
                    // Let's wait for user to finish typing or press a button, 
                    // but for now we just update state. 
                    // To apply, user can click "Custom" button again or we add an "Apply" button.
                }}
                placeholder="Amount (500) or % (10%)"
                className="flex-1 bg-slate-900/50 border border-slate-700 rounded-xl py-2 px-4 text-white placeholder:text-slate-600 focus:border-emerald-500 focus:outline-none text-sm"
              />
              <button 
                onClick={() => applyPricingMode('custom', customMarkup)}
                className="px-4 bg-emerald-600 text-white rounded-xl font-bold text-xs"
              >
                Apply
              </button>
            </div>
            <p className="text-[10px] text-slate-500 mt-1 ml-1">
              Enter fixed amount to add (e.g. 500) or percentage (e.g. 10%)
            </p>
          </div>
        )}
      </div>

      {/* Items List */}
      <div className="space-y-3">
        <div className="flex items-center justify-between px-1">
          <h2 className="text-slate-400 text-sm font-semibold uppercase tracking-wider">Items ({items.length})</h2>
          {items.length > 0 && (
            <span className="text-emerald-400 font-bold font-mono">
              Rs. {totalAmount.toLocaleString()}
            </span>
          )}
        </div>

        {items.length === 0 ? (
          <div className="bg-slate-800/30 border border-slate-700/50 border-dashed rounded-2xl p-8 text-center">
            <div className="w-16 h-16 bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
              <FileText className="w-8 h-8 text-slate-600" />
            </div>
            <p className="text-slate-400 font-medium">No items added</p>
            <p className="text-slate-500 text-sm mt-1">Tap + to add items to quotation</p>
          </div>
        ) : (
          items.map((item, index) => {
            const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
            const isMaxxis = brand === 'MAXXIS' || brand === 'MAXXIES';
            const isEditing = editingIndex === index;
            const isSelected = selectedIndices.has(index);
            const isTyre = item.Category === 'TYRES';

            return (
            <div 
                key={index} 
                onClick={() => toggleSelection(index)}
                className={`bg-slate-800/50 border rounded-xl p-4 relative overflow-hidden group transition-all cursor-pointer ${
                    isSelected 
                    ? 'border-blue-500 shadow-[0_0_15px_rgba(59,130,246,0.15)] bg-blue-900/10' 
                    : 'border-slate-700 hover:border-slate-600'
                }`}
            >
              <div className="flex justify-between items-start mb-2">
                <div className="flex items-start gap-3">
                    {/* Selection Checkbox */}
                    <div className={`mt-1 w-5 h-5 rounded border flex items-center justify-center transition-colors shrink-0 ${
                        isSelected ? 'bg-blue-500 border-blue-500' : 'border-slate-600'
                    }`}>
                        {isSelected && <Check className="w-3 h-3 text-white" />}
                    </div>
                    <div>
                        <h3 className="text-white font-bold">{item.Description}</h3>
                        <div className="flex items-center gap-2 mt-1">
                            <span className="text-xs bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded">{item.Brand}</span>
                            <span className="text-xs bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded">{item.Size}</span>
                            {isMaxxis && (
                                <span className="text-[10px] bg-emerald-900/30 text-emerald-400 border border-emerald-800 px-1.5 py-0.5 rounded">
                                    {item.DiscountPercent ? `-${item.DiscountPercent}%` : 'Fixed'}
                                </span>
                            )}
                        </div>
                    </div>
                </div>
                <div className="flex items-center gap-1">
                    {!isEditing && (
                        <button 
                            onClick={(e) => {
                                e.stopPropagation();
                                startEditing(index, item.UnitPrice);
                            }}
                            className="p-2 text-slate-500 hover:text-blue-400 transition-colors"
                        >
                            <Edit2 className="w-4 h-4" />
                        </button>
                    )}
                    <button 
                        onClick={(e) => {
                            e.stopPropagation();
                            removeItem(index);
                        }}
                        className="p-2 text-slate-500 hover:text-red-400 transition-colors"
                    >
                        <Trash2 className="w-5 h-5" />
                    </button>
                </div>
              </div>
              
              <div className="flex items-center justify-between mt-3 pt-3 border-t border-slate-700/50">
                <div className="flex items-center gap-3 bg-slate-900/50 rounded-lg p-1">
                  <button 
                    onClick={(e) => {
                        e.stopPropagation();
                        updateQuantity(index, -1);
                    }}
                    className="w-8 h-8 flex items-center justify-center text-slate-400 hover:text-white hover:bg-slate-700 rounded-md transition-colors"
                  >
                    -
                  </button>
                  <span className="text-white font-mono font-bold w-4 text-center">{item.Quantity}</span>
                  <button 
                    onClick={(e) => {
                        e.stopPropagation();
                        updateQuantity(index, 1);
                    }}
                    className="w-8 h-8 flex items-center justify-center text-slate-400 hover:text-white hover:bg-slate-700 rounded-md transition-colors"
                  >
                    +
                  </button>
                </div>
                
                <div className="flex items-center gap-3">
                  {!isTyre && !isEditing && (
                      <label className="flex items-center gap-2 cursor-pointer bg-slate-900/50 px-2 py-1 rounded-lg border border-slate-700/50" onClick={(e) => e.stopPropagation()}>
                          <span className={`text-xs font-bold ${item.isFOC ? 'text-red-400' : 'text-slate-400'}`}>FOC</span>
                          <input 
                              type="checkbox" 
                              checked={item.isFOC || false} 
                              onChange={() => toggleFOC(index)}
                              className="w-5 h-5 rounded border-slate-600 bg-slate-800 text-red-500 focus:ring-0 focus:ring-offset-0"
                          />
                      </label>
                  )}
                  <div className="text-right flex flex-col items-end gap-1">
                  {isEditing ? (
                      <div className="flex items-center gap-2" onClick={(e) => e.stopPropagation()}>
                          <input 
                              type="number" 
                              value={editPrice}
                              onChange={(e) => setEditPrice(e.target.value)}
                              className="w-24 bg-slate-900 border border-slate-600 rounded px-2 py-1 text-right text-white font-mono text-sm focus:border-blue-500 outline-none"
                              autoFocus
                          />
                          <button onClick={() => savePrice(index)} className="p-1 bg-emerald-600 rounded text-white">
                              <Check className="w-4 h-4" />
                          </button>
                          <button onClick={() => setEditingIndex(null)} className="p-1 bg-slate-700 rounded text-slate-300">
                              <X className="w-4 h-4" />
                          </button>
                      </div>
                  ) : (
                      <>
                        <p className="text-xs text-slate-500">Unit: {item.UnitPrice.toLocaleString()}</p>
                        <p className={`font-bold font-mono ${item.UnitPrice === 0 ? 'text-red-500' : 'text-emerald-400'}`}>
                            {(item.Quantity * item.UnitPrice).toLocaleString()}
                        </p>
                      </>
                  )}

                  {/* Maxxis Discount Controls */}
                  {isMaxxis && (
                      <div className="flex gap-1 mt-1">
                          <button
                              onClick={(e) => {
                                  e.stopPropagation();
                                  toggleMaxxisDiscount(index, 2.5);
                              }}
                              className={`px-1.5 py-0.5 rounded text-[10px] font-bold border transition-all ${
                                  item.DiscountPercent === 2.5
                                  ? 'bg-emerald-500 border-emerald-400 text-white'
                                  : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                              }`}
                          >
                              2.5%
                          </button>
                          <button
                              onClick={(e) => {
                                  e.stopPropagation();
                                  toggleMaxxisDiscount(index, 5);
                              }}
                              className={`px-1.5 py-0.5 rounded text-[10px] font-bold border transition-all ${
                                  item.DiscountPercent === 5
                                  ? 'bg-emerald-500 border-emerald-400 text-white'
                                  : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                              }`}
                          >
                              5%
                          </button>
                      </div>
                  )}
                </div>
                </div>
              </div>
            </div>
            );
          })
        )}
      </div>

      {/* Add Button */}
      <button
        onClick={() => setShowAddPopup(true)}
        className="w-full py-4 bg-slate-800 hover:bg-slate-700 border border-slate-700 border-dashed rounded-2xl text-slate-400 hover:text-white transition-all flex items-center justify-center gap-2 group"
      >
        <div className="w-8 h-8 rounded-full bg-slate-700 group-hover:bg-slate-600 flex items-center justify-center transition-colors">
          <Plus className="w-5 h-5" />
        </div>
        <span className="font-medium">Add Item</span>
      </button>

      {/* Quick Service Card */}
      <div className="bg-slate-800/50 backdrop-blur-xl border border-slate-700 rounded-2xl p-4">
          <h3 className="text-xs font-bold text-slate-400 uppercase tracking-wider mb-3">Quick Services</h3>
          <div className="grid grid-cols-4 gap-2 mb-3">
              <button 
                onClick={() => addServiceById(SERVICE_IDS.ALIGNMENT_CAR)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <Car className="w-6 h-6 text-slate-400 group-hover:text-indigo-400" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">Car</span>
              </button>
              <button 
                onClick={() => addServiceById(SERVICE_IDS.ALIGNMENT_JEEP)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <Truck className="w-6 h-6 text-slate-400 group-hover:text-indigo-400" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">Jeep/Van</span>
              </button>
              <button 
                onClick={() => addServiceById(SERVICE_IDS.ALIGNMENT_LORRY)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <Truck className="w-6 h-6 text-slate-400 group-hover:text-indigo-400 scale-x-[-1]" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">Lorry</span>
              </button>
              <button 
                onClick={() => addServiceById(SERVICE_IDS.ALIGNMENT_BUS)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <Bus className="w-6 h-6 text-slate-400 group-hover:text-indigo-400" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">Bus</span>
              </button>
              <button 
                onClick={() => addServiceById(SERVICE_IDS.BALANCING)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <CircleDashed className="w-6 h-6 text-slate-400 group-hover:text-indigo-400" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">Balancing</span>
              </button>
              <button 
                onClick={() => addServiceById(SERVICE_IDS.TUBELESS_NECK)}
                className="flex flex-col items-center justify-center gap-2 p-3 bg-slate-900/50 hover:bg-indigo-900/20 border border-slate-700 hover:border-indigo-500 rounded-xl transition-all group"
              >
                  <CircleDot className="w-6 h-6 text-slate-400 group-hover:text-indigo-400" />
                  <span className="text-[10px] font-medium text-slate-400 group-hover:text-white">TL Neck</span>
              </button>
          </div>
          
          <button 
            onClick={openServicePopup}
            className="w-full py-2 bg-slate-900/50 hover:bg-slate-800 border border-slate-700 rounded-xl text-xs font-bold text-slate-400 hover:text-white transition-all flex items-center justify-center gap-2"
          >
              <MoreHorizontal className="w-4 h-4" />
              More Services
          </button>
      </div>

      {/* Generate Button */}
      {items.length > 0 && (
        <button
          onClick={handleGeneratePDF}
          className="w-full py-4 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-500 hover:to-blue-600 text-white rounded-2xl shadow-lg shadow-blue-900/20 font-bold text-lg flex items-center justify-center gap-3 active:scale-95 transition-all"
        >
          <FileText className="w-6 h-6" />
          Generate Quotation PDF
        </button>
      )}

      {/* Add Item Popup (Keypad) */}
      {showAddPopup && (
        <div className="fixed inset-0 bg-slate-950 z-[60] flex flex-col animate-in slide-in-from-bottom-10 duration-200">
          <div className="p-4 border-b border-slate-800 flex items-center justify-between bg-slate-900">
            <h2 className="text-lg font-bold text-white">Search Item</h2>
            <button onClick={() => setShowAddPopup(false)} className="p-2 text-slate-400 hover:text-white">
              <X className="w-6 h-6" />
            </button>
          </div>
          
          <div className="flex-1 flex flex-col p-4">
            <div className="bg-slate-800 rounded-2xl p-6 mb-6 text-center border border-slate-700">
              <p className="text-slate-400 text-sm mb-2">Enter Size</p>
              <div className="text-4xl font-bold text-white font-mono tracking-wider h-12">
                {searchSize || <span className="text-slate-600">_</span>}
              </div>
            </div>

            <div className="mt-auto">
              <NumericKeypad
                onKeyPress={handleKeyPress}
                onBackspace={handleBackspace}
                onClear={handleClear}
                onDone={handleDone}
              />
            </div>
          </div>
        </div>
      )}

      {/* Item Selection Popup */}
      {showItemSelectPopup && (
        <div className="fixed inset-0 bg-slate-950 z-[70] flex flex-col animate-in slide-in-from-right duration-200">
          <div className="p-4 border-b border-slate-800 flex items-center justify-between bg-slate-900">
            <div>
              <h2 className="text-lg font-bold text-white">Select Item</h2>
              <p className="text-xs text-slate-400">Results for "{searchSize}"</p>
            </div>
            <button onClick={() => setShowItemSelectPopup(false)} className="p-2 text-slate-400 hover:text-white">
              <X className="w-6 h-6" />
            </button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-3 pb-24">
            {searchResults.map((item, i) => {
              const isSelected = selectedPopupItems.has(item.ItemId);
              return (
                <button
                  key={i}
                  onClick={() => togglePopupItem(item.ItemId)}
                  className={`w-full border rounded-xl p-4 text-left transition-all ${
                    isSelected 
                      ? 'bg-blue-600/20 border-blue-500 shadow-lg shadow-blue-900/20' 
                      : 'bg-slate-800/50 border-slate-700 active:bg-slate-800'
                  }`}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex items-start gap-3">
                      <div className={`mt-1 w-5 h-5 rounded-full border flex items-center justify-center transition-colors ${
                        isSelected ? 'bg-blue-500 border-blue-500' : 'border-slate-500'
                      }`}>
                        {isSelected && <Check className="w-3 h-3 text-white" />}
                      </div>
                      <div>
                        <h3 className="text-white font-bold">{item.Description}</h3>
                        <div className="flex items-center gap-2 mt-1">
                          <span className="text-xs bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded">{item.Brand}</span>
                          <span className="text-xs bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded">{item.Size}</span>
                        </div>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="text-emerald-400 font-bold">
                        Rs. {calculatePrice(item).toLocaleString()}
                      </p>
                      <p className="text-xs text-slate-500 mt-1">Stock: {item.Quantity}</p>
                    </div>
                  </div>
                </button>
              );
            })}
            {searchResults.length === 0 && (
              <div className="text-center py-12 text-slate-500">
                No items found
              </div>
            )}
          </div>

          {/* Add Selected Button */}
          {selectedPopupItems.size > 0 && (
            <div className="p-4 border-t border-slate-800 bg-slate-900/95 backdrop-blur-xl absolute bottom-0 left-0 right-0 z-10">
              <button
                onClick={addSelectedItems}
                className="w-full py-4 bg-blue-600 hover:bg-blue-500 text-white rounded-2xl font-bold text-lg shadow-lg shadow-blue-900/20 active:scale-95 transition-all flex items-center justify-center gap-2"
              >
                <Plus className="w-5 h-5" />
                Add {selectedPopupItems.size} Item{selectedPopupItems.size !== 1 ? 's' : ''}
              </button>
            </div>
          )}
        </div>
      )}

      {/* Service Selection Popup */}
      {showServicePopup && (
        <div className="fixed inset-0 bg-slate-950 z-[80] flex flex-col animate-in slide-in-from-bottom duration-200">
            <div className="p-4 border-b border-slate-800 flex items-center justify-between bg-slate-900">
                <h2 className="text-lg font-bold text-white flex items-center gap-2">
                    <Wrench className="w-5 h-5 text-indigo-400" />
                    Select Service
                </h2>
                <button onClick={() => setShowServicePopup(false)} className="p-2 text-slate-400 hover:text-white">
                    <X className="w-6 h-6" />
                </button>
            </div>

            {/* Search Bar */}
            <div className="p-4 bg-slate-900 border-b border-slate-800">
                <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-500" />
                    <input
                        type="text"
                        value={serviceSearchQuery}
                        onChange={(e) => {
                            const val = e.target.value;
                            setServiceSearchQuery(val);
                            // If typing, clear category to search all, or keep category?
                            // Let's keep category if selected, but if user wants global search they can select 'All'
                            fetchServices(selectedCategory, val);
                        }}
                        placeholder="Search services..."
                        className="w-full bg-slate-800 border border-slate-700 rounded-xl py-2 pl-9 pr-4 text-white placeholder:text-slate-600 focus:border-indigo-500 focus:outline-none text-sm"
                    />
                    {serviceSearchQuery && (
                        <button 
                            onClick={() => {
                                setServiceSearchQuery('');
                                fetchServices(selectedCategory, '');
                            }}
                            className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 hover:text-white"
                        >
                            <X className="w-4 h-4" />
                        </button>
                    )}
                </div>
            </div>

            {/* Categories (Horizontal Scroll) */}
            <div className="p-2 bg-slate-900 border-b border-slate-800 overflow-x-auto flex gap-2 no-scrollbar">
                <button
                    onClick={() => {
                        setSelectedCategory('');
                        fetchServices('', serviceSearchQuery);
                    }}
                    className={`px-4 py-2 rounded-full text-xs font-bold whitespace-nowrap transition-all border ${
                        selectedCategory === ''
                        ? 'bg-indigo-600 border-indigo-500 text-white shadow-lg shadow-indigo-900/20'
                        : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                    }`}
                >
                    All
                </button>
                {serviceCategories.map(cat => (
                    <button
                        key={cat}
                        onClick={() => {
                            setSelectedCategory(cat);
                            fetchServices(cat, serviceSearchQuery);
                        }}
                        className={`px-4 py-2 rounded-full text-xs font-bold whitespace-nowrap transition-all border ${
                            selectedCategory === cat
                            ? 'bg-indigo-600 border-indigo-500 text-white shadow-lg shadow-indigo-900/20'
                            : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                        }`}
                    >
                        {cat}
                    </button>
                ))}
            </div>

            {/* Service List */}
            <div className="flex-1 overflow-y-auto p-4 space-y-2 bg-slate-950">
                {loading ? (
                    <div className="flex items-center justify-center py-12">
                        <Loader2 className="w-8 h-8 text-indigo-500 animate-spin" />
                    </div>
                ) : serviceResults.length === 0 ? (
                    <div className="text-center py-12 text-slate-500">
                        No services found in this category
                    </div>
                ) : (
                    serviceResults.map((item) => {
                        const isAdded = items.some(i => i.ItemId === item.ItemId);
                        return (
                            <button
                                key={item.ItemId}
                                onClick={() => {
                                    if (!isAdded) {
                                        addItem(item);
                                        setShowServicePopup(false);
                                    }
                                }}
                                disabled={isAdded}
                                className={`w-full p-4 rounded-xl border text-left transition-all flex justify-between items-center ${
                                    isAdded 
                                    ? 'bg-slate-900 border-slate-800 opacity-50 cursor-not-allowed' 
                                    : 'bg-slate-800/50 border-slate-700 hover:border-indigo-500 hover:bg-slate-800'
                                }`}
                            >
                                <div>
                                    <h4 className="text-white font-bold text-sm">{item.Description}</h4>
                                    <p className="text-xs text-slate-500 mt-1">{item.Category}</p>
                                </div>
                                <div className="text-right">
                                    <span className="text-emerald-400 font-bold font-mono">
                                        Rs. {item.SellingPrice ? item.SellingPrice.toLocaleString() : '0'}
                                    </span>
                                    {isAdded && <span className="block text-[10px] text-indigo-400 mt-1">Added</span>}
                                </div>
                            </button>
                        );
                    })
                )}
            </div>
        </div>
      )}
    </div>
  );
}
