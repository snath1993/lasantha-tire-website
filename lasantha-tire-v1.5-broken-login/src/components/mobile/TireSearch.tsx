'use client';

import { useState, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { 
  Search, Mic, ShoppingCart, Loader2, AlertCircle, X, Check, Tag, Percent, Zap, Share,
  Car, Bus, Truck, CircleDashed, CircleDot, Edit, CheckSquare, Trash2, BarChart3
} from 'lucide-react';
import NumericKeypad from './NumericKeypad';
import { useModal } from '@/contexts/ModalContext';
import { useToast } from '@/contexts/ToastContext';
import { authenticatedFetch } from '@/lib/client-auth';
import { saveQuotationToDb } from '@/lib/quotationShare';

interface TireProduct {
  ItemId: string;
  Description: string;
  Brand: string;
  Quantity: number;
  Price?: number;
  SellingPrice?: number;
  UnitCost?: number;
  LastGRN?: {
    No: string;
    Date: string;
    Qty: number;
    History?: Array<{
        InvReferenceNo: string;
        InvoiceDate: string;
        Qty: number;
    }>;
  } | null;
}

export default function TireSearch({ onAddToQuote, quoteItems = [] }: { onAddToQuote: (item: TireProduct) => void, quoteItems?: any[] }) {
  const router = useRouter();
  const [width, setWidth] = useState('');
  const [profile, setProfile] = useState('');
  const [rim, setRim] = useState('');
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<TireProduct[]>([]);
  const [isListening, setIsListening] = useState(false);
  const [transcript, setTranscript] = useState('');
  const [micError, setMicError] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [selectedItems, setSelectedItems] = useState<Set<string>>(new Set());
  const [discountedItems, setDiscountedItems] = useState<Set<string>>(new Set());
  const [maxxisDiscounts, setMaxxisDiscounts] = useState<Record<string, number>>({}); // ItemId -> 2.5 or 5
  const [isSharing, setIsSharing] = useState(false);
  
  // New Pricing State
  type PricingMode = 'cost_plus' | 'wholesale' | 'cash' | 'selling' | 'custom';
  const [pricingMode, setPricingMode] = useState<PricingMode>('selling');
  const [customMarkup, setCustomMarkup] = useState<string>('');

  // Service Item Customization State
  const [focItems, setFocItems] = useState<Set<string>>(new Set());
  const [customServicePrices, setCustomServicePrices] = useState<Record<string, number>>({});
  const [editingPriceId, setEditingPriceId] = useState<string | null>(null);
  const [tempPriceInput, setTempPriceInput] = useState('');

  const [showResultsModal, setShowResultsModal] = useState(false);
  const { setIsResultsModalOpen } = useModal();
  const [voiceLang, setVoiceLang] = useState<'si-LK' | 'en-US'>('si-LK');
  const { showToast } = useToast();

  const [suggestions, setSuggestions] = useState<string[]>([]);

  // Common Tire Sizes for Suggestions
  const COMMON_SIZES = [
    // Cars (12-16 inch)
    '145/70R12', '155/65R13', '155/70R13', '155/80R13', '165/65R13', '165/70R13', '175/70R13',
    '165/65R14', '165/70R14', '175/65R14', '175/70R14', '185/65R14', '185/70R14', '195/70R14',
    '175/65R15', '185/60R15', '185/65R15', '195/55R15', '195/60R15', '195/65R15', '205/65R15',
    '185/55R16', '195/50R16', '195/55R16', '205/55R16', '205/60R16', '215/60R16', '215/65R16',
    
    // SUVs / Jeeps (16-18 inch)
    '215/70R16', '225/70R16', '235/70R16', '245/70R16', '265/70R16',
    '215/55R17', '225/60R17', '225/65R17', '265/65R17',
    '225/45R18', '235/55R18', '265/60R18',

    // Vans / Commercial
    '195R15', '195R14', '185R14', '175R14', '165R13LT', '155R12',

    // Three Wheelers
    '4.00-8', '4.00-10', '4.00-12', '4.50-10', '5.00-10',

    // Motorcycles
    '90/90-17', '100/90-17', '120/80-17', '140/70-17',
    '2.75-17', '3.00-17', '3.00-18', '2.75-18', '90/90-18',
    '3.50-10', '90/90-10', '90/100-10', '100/90-10', '120/70-12', '130/70-12'
  ];

  // Update suggestions when input changes
  useEffect(() => {
    if (!searchInput || searchInput.length < 2) {
        setSuggestions([]);
        return;
    }

    const normalizedInput = searchInput.replace(/[^0-9]/g, '');
    if (!normalizedInput) return;

    const matches = COMMON_SIZES.filter(size => {
        const normalizedSize = size.replace(/[^0-9]/g, '');
        return normalizedSize.startsWith(normalizedInput);
    }).slice(0, 6); // Limit to 6 suggestions

    setSuggestions(matches);
  }, [searchInput]);

  const handleSuggestionClick = (size: string) => {
    // Parse the selected size string into components
    // Format: 175/70R13 or 4.00-8
    
    // Try standard metric first: 175/70R13
    const metricMatch = size.match(/(\d+)\/(\d+)R(\d+)/);
    if (metricMatch) {
        const w = metricMatch[1];
        const p = metricMatch[2];
        const r = metricMatch[3];
        setWidth(w);
        setProfile(p);
        setRim(r);
        setSearchInput(size); // Show full formatted size
        setSuggestions([]);
        handleSearch(w, p, r).then(() => {
            setShowResultsModal(true);
            setIsResultsModalOpen(true);
        });
        return;
    }

    // Try commercial/van: 195R15
    const vanMatch = size.match(/(\d+)R(\d+)/);
    if (vanMatch) {
        const w = vanMatch[1];
        const r = vanMatch[2];
        setWidth(w);
        setProfile(''); // Often implied 80
        setRim(r);
        setSearchInput(size);
        setSuggestions([]);
        handleSearch(w, '', r).then(() => {
            setShowResultsModal(true);
            setIsResultsModalOpen(true);
        });
        return;
    }

    // Try bias/three-wheel: 4.00-8
    const biasMatch = size.match(/(\d+\.?\d*)-(\d+)/);
    if (biasMatch) {
        const w = biasMatch[1];
        const r = biasMatch[2];
        setWidth(w);
        setProfile('');
        setRim(r);
        setSearchInput(size);
        setSuggestions([]);
        handleSearch(w, '', r).then(() => {
            setShowResultsModal(true);
            setIsResultsModalOpen(true);
        });
        return;
    }
    
    // Fallback for motorcycle metric: 90/90-17
    const motoMatch = size.match(/(\d+)\/(\d+)-(\d+)/);
    if (motoMatch) {
        const w = motoMatch[1];
        const p = motoMatch[2];
        const r = motoMatch[3];
        setWidth(w);
        setProfile(p);
        setRim(r);
        setSearchInput(size);
        setSuggestions([]);
        handleSearch(w, p, r).then(() => {
            setShowResultsModal(true);
            setIsResultsModalOpen(true);
        });
        return;
    }
  };

  // Hardcoded Service IDs
  const SERVICE_IDS = {
      ALIGNMENT_CAR: '120',
      ALIGNMENT_JEEP: '121',
      ALIGNMENT_LORRY: '161',
      ALIGNMENT_BUS: '144',
      BALANCING: '122',
      TUBELESS_NECK: '114',
  };

  // Fetch specific item by ID and add it
  const addServiceById = async (itemId: string) => {
    const ALIGNMENT_IDS = [
        SERVICE_IDS.ALIGNMENT_CAR,
        SERVICE_IDS.ALIGNMENT_JEEP,
        SERVICE_IDS.ALIGNMENT_LORRY,
        SERVICE_IDS.ALIGNMENT_BUS
    ];

    // Check if already in results
    const isAlreadyInResults = results.some(r => r.ItemId === itemId);

    if (isAlreadyInResults) {
        // Remove from results
        setResults(prev => prev.filter(r => r.ItemId !== itemId));
        // Remove from selection
        setSelectedItems(prev => {
            const next = new Set(prev);
            next.delete(itemId);
            return next;
        });
        return;
    }

    // Handle Alignment Exclusivity
    if (ALIGNMENT_IDS.includes(itemId)) {
        // Remove other alignment items from results
        setResults(prev => prev.filter(r => !ALIGNMENT_IDS.includes(r.ItemId)));
        // Remove other alignment items from selection
        setSelectedItems(prev => {
            const next = new Set(prev);
            ALIGNMENT_IDS.forEach(id => next.delete(id));
            return next;
        });
    }

    setLoading(true);
    try {
        const res = await authenticatedFetch(`/api/erp/inventory?type=services&itemId=${itemId}`);
        const data = await res.json();
        
        if (data.success && data.data && data.data.length > 0) {
            const item = data.data[0];
            
            // Add to results
            setResults(prev => [...prev, item]);

            // Auto-select the item
            setSelectedItems(prev => {
                const newSet = new Set(prev);
                newSet.add(item.ItemId);
                return newSet;
            });

        } else {
            showToast('error', 'Service item not found');
        }
    } catch (e) {
        showToast('error', 'Failed to add service');
    } finally {
        setLoading(false);
    }
  };

  // Keep a persistent reference to the recognition instance to avoid GC
  const recognitionRef = useRef<any>(null);

  const touchStartX = useRef<number>(0);
  const touchEndX = useRef<number>(0);

  const toggleSelection = (itemId: string) => {
    const newSelected = new Set(selectedItems);
    if (newSelected.has(itemId)) {
      newSelected.delete(itemId);
    } else {
      newSelected.add(itemId);
    }
    setSelectedItems(newSelected);
  };

  // Helper to calculate price based on current mode
  const calculatePrice = (item: TireProduct) => {
    // If item is a service, return SellingPrice directly (no markup)
    if (Object.values(SERVICE_IDS).includes(item.ItemId)) {
        if (focItems.has(item.ItemId)) return 0;
        if (customServicePrices[item.ItemId] !== undefined) return customServicePrices[item.ItemId];
        return Number(item.SellingPrice) || Number(item.UnitCost) || 0;
    }

    const cost = Number(item.UnitCost) || 0;
    let finalPrice = cost;

    // Maxxis Exception: Fixed price (Cost = Selling), no markups allowed
    const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
    if (brand === 'MAXXIS' || brand === 'MAXXIES') {
        const discount = maxxisDiscounts[item.ItemId];
        if (discount) {
            finalPrice = cost * (1 - discount / 100);
        } else {
            finalPrice = cost;
        }
        return Math.ceil(finalPrice / 50) * 50;
    }
    
    switch (pricingMode) {
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
            if (!customMarkup) {
                finalPrice = cost;
            } else if (customMarkup.endsWith('%')) {
                const pct = parseFloat(customMarkup.replace('%', ''));
                finalPrice = isNaN(pct) ? cost : cost + (cost * pct / 100);
            } else {
                const val = parseFloat(customMarkup);
                finalPrice = isNaN(val) ? cost : cost + val;
            }
            break;
        default:
            finalPrice = Number(item.SellingPrice) || (cost + 2000);
    }
    
    return Math.ceil(finalPrice / 50) * 50;
  };

  const toggleDiscount = (itemId: string, e: React.MouseEvent) => {
    e.stopPropagation(); // Prevent selecting the item row
    const newDiscounted = new Set(discountedItems);
    if (newDiscounted.has(itemId)) {
      newDiscounted.delete(itemId);
    } else {
      newDiscounted.add(itemId);
    }
    setDiscountedItems(newDiscounted);
  };

  const toggleMaxxisDiscount = (itemId: string, percent: number, e: React.MouseEvent) => {
    e.stopPropagation();
    setMaxxisDiscounts(prev => {
        if (prev[itemId] === percent) {
            const next = { ...prev };
            delete next[itemId];
            return next;
        }
        return { ...prev, [itemId]: percent };
    });
  };

  const toggleSelectAll = () => {
    if (selectedItems.size === results.length) {
      setSelectedItems(new Set());
    } else {
      const allIds = new Set(results.map(r => r.ItemId));
      setSelectedItems(allIds);
    }
  };

  // Share message builder with booking link
  const DEFAULT_BOOKING_URL = 'https://book.lasanthatyre.com';

  const buildShareMessage = (
    itemsToShare: TireProduct[],
    sizeLabel: string,
    meta?: { quotationNumber?: string; bookingUrl?: string }
  ) => {
    const { quotationNumber, bookingUrl } = meta || {};
    const appointmentLink = bookingUrl || DEFAULT_BOOKING_URL;
    const headerSizeLabel = sizeLabel || 'Tyre';

    let text = `🏢 *LASANTHA TYRE TRADERS*\n`;
    text += `🚀 *Go Smart with Lasantha Tyre Traders*\n`;
    if (quotationNumber) {
      text += `📋 *Ref: ${quotationNumber}*\n`;
    }
    text += `\n`;

    text += itemsToShare.map((item) => {
      const price = calculatePrice(item);
      const isFoc = focItems.has(item.ItemId);
      const priceDisplay = isFoc ? 'FOC' : `Rs. ${price.toLocaleString()}`;

      let desc = item.Description;
      // Clean up size from description
      if (width && rim) {
        const sep = String.raw`[\/\\\s-]*`;
        const sizeFragment = profile
          ? `${width}${sep}${profile}${sep}R?${sep}${rim}`
          : `${width}${sep}R?${sep}${rim}`;
        const sizeRegex = new RegExp(sizeFragment, 'gi');
        desc = desc.replace(sizeRegex, '').trim();
      }
      desc = desc.replace(/^[-/]\s*/, '').replace(/\s*[-/]$/, '').trim();
      desc = desc.replace(/\s{2,}/g, ' ');

      return `🛠️ ${headerSizeLabel}\n🏷️ ${desc}\n💰 ${priceDisplay}/=`;
    }).join('\n\n');

    text += `\n\n------------------------\n\n`;
    text += `⏳ *Skip the Queue! Book Now:*\n${appointmentLink}\n\n`;
    text += `🕒 *Opening Hours:*\n`;
    text += `   Shop: 6:30 AM - 9:00 PM\n`;
    text += `   Alignment: 7:30 AM - 6:00 PM\n\n`;
    text += `📞 *Hotline:* 0773131883\n`;
    text += `🛡️ *Authorized Dealer with Genuine Warranty*\n\n`;
    text += `📍 *Location:*\n`;
    text += `Lasantha Tyre Traders\n`;
    text += `https://maps.app.goo.gl/7tEg3juSbtBqFgd98?g_st=ipc`;

    return text;
  };

  const shareTextViaDevice = async (text: string) => {
    if (navigator.share) {
      try {
        await navigator.share({ title: 'Tire Quotation', text });
        return;
      } catch (error) {
        console.error('Error sharing via Web Share API:', error);
      }
    }
    try {
      await navigator.clipboard.writeText(text);
      showToast('success', 'Copied to clipboard!');
    } catch (err) {
      console.error('Clipboard write failed:', err);
      showToast('error', 'Sharing not supported');
    }
  };

  const handleShareSelected = async () => {
    const itemsToShare = results.filter((r) => selectedItems.has(r.ItemId));
    if (itemsToShare.length === 0) return;

    const sizeStr = width && rim ? (profile ? `${width}/${profile}R${rim}` : `${width}R${rim}`) : '';
    const baseMessage = buildShareMessage(itemsToShare, sizeStr);
    const serviceIds = Object.values(SERVICE_IDS);

    const payloadItems = itemsToShare.map((item) => ({
      itemId: item.ItemId,
      description: item.Description,
      brand: item.Brand,
      price: focItems.has(item.ItemId) ? 0 : calculatePrice(item),
      quantity: 1,
      isService: serviceIds.includes(item.ItemId),
    }));

    const totalAmount = payloadItems.reduce((sum, item) => sum + item.price * item.quantity, 0);

    setIsSharing(true);
    try {
      const result = await saveQuotationToDb({
        customerName: 'Customer',
        customerPhone: '',
        tyreSize: sizeStr,
        items: payloadItems,
        totalAmount,
        messageContent: baseMessage,
        source: 'Mobile App - TireSearch',
      });

      console.log('🔍 TireSearch saveQuotationToDb result:', result);

      const finalMessage = buildShareMessage(itemsToShare, sizeStr, {
        quotationNumber: result?.quotationNumber,
        bookingUrl: result?.bookingUrl,
      });

      console.log('📤 Final message booking URL:', result?.bookingUrl || 'default');

      await shareTextViaDevice(finalMessage);
    } catch (error) {
      console.error('Error creating quotation before sharing:', error);
      showToast('error', 'Failed to save quotation');
      await shareTextViaDevice(baseMessage);
    } finally {
      setIsSharing(false);
    }
  };

  const handleSearch = async (w?: string, p?: string, r?: string) => {
    const qWidth = w !== undefined ? w : width;
    const qProfile = p !== undefined ? p : profile;
    const qRim = r !== undefined ? r : rim;

    if (!qWidth || !qRim) return;
    
    setLoading(true);
    try {
      // Construct query params
      const params = new URLSearchParams();
      params.append('type', 'search');
      if (qWidth) params.append('width', qWidth);
      if (qProfile) params.append('profile', qProfile);
      if (qRim) params.append('rim', qRim);

      const res = await authenticatedFetch(`/api/erp/inventory?${params.toString()}`);
      if (res.ok) {
        const data = await res.json();
        // Trust the backend search results
        setResults(data.data);
      }
    } catch (error) {
      console.error('Search failed', error);
    } finally {
      setLoading(false);
    }
  };

  // Voice Search Logic
  const startListening = () => {
    setMicError('');

    // Check for browser support first
    if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
      // If on mobile and not secure, it's likely hidden by the browser policy
      if (!window.isSecureContext) {
         const msg = 'Voice search requires a secure HTTPS connection. Please use the Cloudflare link or localhost.';
         setMicError(msg);
         alert(msg);
         return;
      }
      
      const msg = 'Voice search is not supported in this browser. Please use Chrome or Safari.';
      setMicError(msg);
      alert(msg);
      return;
    }

    // Secure context check (redundant but good for explicit messaging)
    if (!window.isSecureContext && !['localhost', '127.0.0.1'].includes(location.hostname)) {
      const msg = 'Voice requires HTTPS (or localhost) on mobile. Please check your connection.';
      setMicError(msg);
      alert(msg);
      return;
    }

    try {
      // Safari requires the window.SpeechRecognition prefix explicitly sometimes
      const SpeechRecognition = (window as any).webkitSpeechRecognition || (window as any).SpeechRecognition;
      const recognition = new SpeechRecognition();
      
      // Configure for best mobile results
      recognition.lang = voiceLang; // Use selected language
      recognition.continuous = false; // CRITICAL for mobile stability
      recognition.interimResults = true; // Show what's being heard
      recognition.maxAlternatives = 1;

      // iOS Safari specific fix: sometimes it needs a short delay or explicit start
      // inside a user gesture (which we are in).
      
      recognition.onstart = () => {
        setIsListening(true);
        setTranscript('Listening...');
      };

      recognition.onresult = (event: any) => {
        const currentTranscript = Array.from(event.results)
          .map((result: any) => result[0].transcript)
          .join('');
        
        setTranscript(currentTranscript);

        if (event.results[0].isFinal) {
          setSearchInput(currentTranscript);
          parseInput(currentTranscript);
          // Small delay to let user see the result before closing overlay
          setTimeout(() => {
            setIsListening(false);
            setTranscript('');
          }, 500);
        }
      };

      recognition.onerror = (event: any) => {
        console.error('Speech recognition error', event.error);
        
        // Handle specific iOS/Safari quirks
        if (event.error === 'service-not-allowed' || event.error === 'not-allowed') {
             // On iOS, this often means Dictation is disabled OR the site permission is blocked
             // We can try to fallback to English if Sinhala failed, or just show the help msg
             setIsListening(false);
             const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent);
             
             let msg = 'Voice service unavailable.';
             if (isIOS) {
                 if (voiceLang === 'si-LK') {
                     msg = 'Sinhala voice not active. Go to Settings > General > Keyboard > Dictation Languages and check "Sinhala".';
                 } else {
                     msg = 'Please Enable Dictation in Settings > General > Keyboard.';
                 }
             } else {
                 msg = 'Please check microphone permissions.';
             }
             
             setMicError(msg);
             alert(msg);
             return;
        }

        if (event.error === 'no-speech') {
            // Just close silently or show "Didn't hear anything"
            setIsListening(false);
            return;
        }
        
        setIsListening(false);
        
        let errorMsg = 'Voice error occurred.';
        if (event.error === 'not-allowed' || event.error === 'permission-denied') {
          errorMsg = 'Microphone access denied. Please enable permissions.';
          if (confirm('Microphone permission is required. Open settings?')) {
             // Try to open settings (works on some Androids)
             try { window.location.href = 'intent://#Intent;action=android.settings.APPLICATION_DETAILS_SETTINGS;package=com.android.chrome;end'; } catch {}
          }
        } else if (event.error === 'network') {
            errorMsg = 'Network error. Check your internet connection.';
        }
        
        // Only show alert if we haven't handled it above
        if (!micError) {
            setMicError(errorMsg);
            // alert(errorMsg); // Don't spam alerts for generic errors
        }
      };

      recognition.onend = () => {
        // Ensure we clean up state if it stops naturally
        if (isListening) {
            setIsListening(false);
        }
      };

      recognitionRef.current = recognition;
      
      // Small delay for iOS to register the gesture properly
      setTimeout(() => {
        try {
            recognition.start();
        } catch (e) {
            console.error('Start error:', e);
            setMicError('Please tap again.');
        }
      }, 100);
      
    } catch (e) {
      console.error('Voice start error:', e);
      setMicError('Failed to start voice search.');
      setIsListening(false);
    }
  };

  const stopListening = () => {
    try {
      recognitionRef.current?.stop?.();
    } catch {}
    setIsListening(false);
  };

  useEffect(() => {
    return () => {
      try { recognitionRef.current?.abort?.(); } catch {}
    };
  }, []);

  const parseInput = (text: string) => {
    // Normalize text: replace common separators with space, remove non-numeric chars except dot
    let normalized = text.toLowerCase()
      .replace(/by|into|x|\/|-|dash|slash|r/g, ' ') // Replace 'r' with space too (175r13)
      .replace(/one/g, '1').replace(/two/g, '2').replace(/three/g, '3')
      .replace(/four/g, '4').replace(/five/g, '5').replace(/six/g, '6')
      .replace(/seven/g, '7').replace(/eight/g, '8').replace(/nine/g, '9')
      .replace(/zero/g, '0');

    // Match numbers, including decimals (e.g. 10.5)
    const numbers = normalized.match(/[0-9]+(\.[0-9]+)?/g);
    
    if (!numbers) return;

    // Case 1: Continuous digits (e.g. 1757013 or 909017)
    if (numbers.length === 1) {
      const val = numbers[0];
      if (val.length === 7) {
        // 1757013 -> 175 70 13
        setWidth(val.substring(0, 3));
        setProfile(val.substring(3, 5));
        setRim(val.substring(5, 7));
        return;
      } else if (val.length === 6) {
        // 909017 -> 90 90 17
        setWidth(val.substring(0, 2));
        setProfile(val.substring(2, 4));
        setRim(val.substring(4, 6));
        return;
      }
    }
    
    // Case 2: 3 distinct numbers (Standard Metric: 175 70 13)
    if (numbers.length >= 3) {
      // Convert to floats for comparison
      const nums = numbers.slice(0, 3).map(n => parseFloat(n));
      
      // Heuristic:
      // Rim: Usually 10-24 (integers)
      // Width: Usually > 80 (mm) or < 15 (inches)
      // Profile: Usually 30-100
      
      // Try to identify Rim first (usually the last number or small integer)
      // But strictly speaking, input order is usually W P R.
      
      // Let's assume order W P R if they fit ranges
      const [n1, n2, n3] = nums;
      
      // Check if it matches W/P/R pattern
      // W: 20-400
      // P: 20-100
      // R: 8-30
      
      // Standard Car/Bike: 175 70 13 or 90 90 17
      if (n1 >= 20 && n2 >= 20 && n3 >= 8 && n3 <= 30) {
         setWidth(numbers[0]);
         setProfile(numbers[1]);
         setRim(numbers[2]);
         return;
      }
      
      // Fallback: Just take first 3
      setWidth(numbers[0]);
      setProfile(numbers[1]);
      setRim(numbers[2]);
    }
    
    // Case 3: 2 numbers (e.g. 3.00 17 or 7.50 16) - Tube type / Old style
    if (numbers.length === 2) {
       // Assume Width and Rim (Profile is often omitted/standard 80-100)
       // e.g. 7.50-16 -> Width 7.50, Rim 16
       setWidth(numbers[0]);
       setProfile(''); // Clear profile
       setRim(numbers[1]);
    }
  };

  const handleKeypadPress = (key: string) => {
    const newVal = searchInput + key;
    setSearchInput(newVal);
    parseInput(newVal);
  };

  const handleKeypadBackspace = () => {
    if (searchInput.length === 0) return;
    const newVal = searchInput.slice(0, -1);
    setSearchInput(newVal);
    if (newVal === '') {
      setWidth('');
      setProfile('');
      setRim('');
      setResults([]);
    } else {
      parseInput(newVal);
    }
  };

  const handleKeypadClear = () => {
    setSearchInput('');
    setWidth(''); setProfile(''); setRim(''); setResults([]);
  };

  const handleKeypadDone = () => {
    // If we have results, show the results modal
    if (results.length > 0) {
        setShowResultsModal(true);
        setIsResultsModalOpen(true);
    } else if (width && rim) {
        // If no results yet but we have input, try searching first
        handleSearch().then(() => {
            setShowResultsModal(true);
            setIsResultsModalOpen(true);
        });
    }
  };

  const handleAddSelected = () => {
    if (selectedItems.size > 0) {
      const itemsToAdd = results.filter(r => selectedItems.has(r.ItemId));
      itemsToAdd.forEach(item => {
        // Calculate final price based on pricing mode
        const finalPrice = calculatePrice(item);

        onAddToQuote({
            ...item,
            SellingPrice: finalPrice
        });
      });
      setSelectedItems(new Set());
      setDiscountedItems(new Set());
    }
    setShowResultsModal(false);
    setIsResultsModalOpen(false);
    // Clear search after adding? Maybe keep it for reference.
    // setSearchInput(''); setWidth(''); setProfile(''); setRim(''); setResults([]);
  };

  const handleCloseModal = () => {
    setShowResultsModal(false);
    setIsResultsModalOpen(false);
  };

  const handleTouchStart = (e: React.TouchEvent) => {
    touchStartX.current = e.touches[0].clientX;
    touchEndX.current = e.touches[0].clientX; // Reset end position to start position
  };

  const handleTouchMove = (e: React.TouchEvent) => {
    touchEndX.current = e.touches[0].clientX;
  };

  const handleTouchEnd = () => {
    const swipeDistance = touchEndX.current - touchStartX.current;
    const swipeThreshold = 150;

    // Swipe right (left to right)
    if (swipeDistance > swipeThreshold) {
      handleCloseModal();
    }
  };

  useEffect(() => {
    // Only search if we have all 3 components AND the user has stopped typing (debounced)
    // OR if the user explicitly pressed "Done" (which we can handle separately if needed)
    // For now, we'll just wait for all 3 parts to be present.
    // BUT, to prevent premature searching while typing (e.g. 175/70/1...), we can add a small delay
    // or check if the input length looks "complete".
    
    // Better approach: Only search when we have valid Width, Profile, AND Rim
    if (width && profile && rim) {
      const timer = setTimeout(() => {
        handleSearch();
      }, 800); // Wait 800ms after last change before searching
      return () => clearTimeout(timer);
    }
  }, [width, profile, rim]);

  return (
    <div className="fixed inset-0 flex flex-col bg-slate-950 overflow-hidden">
      {/* Unified Calculator Card */}
      {!showResultsModal && (
      <div className="w-full h-full bg-slate-900 flex flex-col">
        
        {/* Display Section */}
        <div className="flex flex-col justify-end p-6 pb-3 relative" style={{height: '35vh'}}>
            
            {/* Analytics Button */}
            <button
                onClick={() => router.push('/analytics')}
                className="absolute right-6 top-6 p-3 bg-slate-800/50 backdrop-blur-md rounded-xl border border-white/10 text-violet-400 hover:bg-violet-600 hover:text-white transition-all z-50"
            >
                <BarChart3 className="w-6 h-6" />
            </button>

            <div className="text-right mb-12">
                <span className="text-slate-500 text-xs font-bold uppercase tracking-wider block mb-2">Tyre Size</span>
                <input
                type="text"
                readOnly
                value={searchInput}
                placeholder="0"
                className="w-full bg-transparent border-none text-white placeholder:text-slate-800 text-6xl font-bold tracking-tighter text-right focus:outline-none"
                />
            </div>
            
            {/* Controls (Voice & Clear) */}
            <div className="absolute left-6 bottom-4 flex items-center gap-3 z-50">
                {/* Clear button removed as requested */}
                {micError && (
                  <span className="ml-2 text-xs text-rose-400">{micError}</span>
                )}
            </div>



        </div>



        {/* Divider */}
        <div className="h-px bg-gradient-to-r from-transparent via-slate-800 to-transparent w-full opacity-50" />

        {/* Keypad Section */}
        <div className="flex-1 px-3 py-4 bg-slate-900 flex items-center justify-center">
            <div className="w-full relative">
                {/* Floating Suggestions Island - Pinned to Keypad */}
                {suggestions.length > 0 && (
                    <div className="absolute bottom-full left-0 right-0 mb-3 z-50 flex justify-center items-center animate-in slide-in-from-bottom-2 duration-300 pointer-events-none">
                        <div className="bg-slate-800/95 backdrop-blur-xl border border-white/10 rounded-3xl p-2 shadow-2xl shadow-black/50 flex gap-3 overflow-x-auto no-scrollbar max-w-[100%] pointer-events-auto">
                            {suggestions.map((size, index) => (
                                <button
                                    key={size}
                                    onClick={() => handleSuggestionClick(size)}
                                    className="px-6 py-3 bg-slate-700/50 text-white text-lg font-bold rounded-2xl active:scale-95 transition-all whitespace-nowrap hover:bg-blue-600 hover:shadow-lg hover:shadow-blue-600/20 flex items-center gap-2"
                                >
                                    {size}
                                </button>
                            ))}
                        </div>
                    </div>
                )}

                <NumericKeypad
                onKeyPress={handleKeypadPress}
                onBackspace={handleKeypadBackspace}
                onClear={handleKeypadClear}
                onDone={handleKeypadDone}
                />
            </div>
        </div>
      </div>
      )}

      {/* Results Modal (Full Screen) */}
      {showResultsModal && (
        <div 
          className="fixed inset-0 z-[60] bg-slate-900 flex flex-col"
          onTouchStart={handleTouchStart}
          onTouchMove={handleTouchMove}
          onTouchEnd={handleTouchEnd}
        >
            {/* Header */}
            <div className="p-4 bg-gradient-to-r from-indigo-600 to-violet-700 flex justify-between items-center shrink-0 shadow-lg">
              <div>
                <h3 className="text-white font-bold text-lg flex items-center gap-2">
                    Select Tires
                    <span className="px-2 py-0.5 bg-white/20 rounded-full text-[10px] font-mono">
                        {results.length}
                    </span>
                </h3>
                <p className="text-indigo-200 text-xs">
                   {width}/{profile}R{rim}
                </p>
              </div>
              
              <div className="flex items-center gap-2">
                  {/* Select All Toggle */}
                  <button
                    onClick={toggleSelectAll}
                    title={selectedItems.size === results.length ? 'Deselect All' : 'Select All'}
                    className={`p-2 rounded-lg transition-all border ${
                        selectedItems.size === results.length && results.length > 0
                        ? 'bg-indigo-600 border-indigo-500 text-white shadow-[0_0_15px_rgba(99,102,241,0.4)]'
                        : 'bg-white/10 border-white/20 text-indigo-100 hover:bg-white/20'
                    }`}
                  >
                    <CheckSquare className={`w-5 h-5 ${selectedItems.size === results.length ? 'fill-current' : ''}`} />
                  </button>

                  <button 
                    onClick={() => {
                        setShowResultsModal(false);
                        setIsResultsModalOpen(false);
                    }}
                    className="p-2 bg-white/10 rounded-full text-white hover:bg-white/20 transition-colors"
                  >
                    <X className="w-5 h-5" />
                  </button>
              </div>
            </div>

            {/* List */}
            <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-slate-950">
               {results.map((item) => {
                const isDiscounted = discountedItems.has(item.ItemId);
                const standardPrice = item.SellingPrice || 0;
                const cashPrice = standardPrice - 500;
                
                const brand = (item.Brand || '').toUpperCase().replace(/[^A-Z0-9]/g, '');
                const isMaxxis = brand === 'MAXXIS' || brand === 'MAXXIES';
                const isService = Object.values(SERVICE_IDS).includes(item.ItemId);

                return (
                <div 
                  key={item.ItemId}
                  onClick={() => toggleSelection(item.ItemId)}
                  className={`w-full border rounded-xl p-4 text-left transition-all relative overflow-hidden ${
                    selectedItems.has(item.ItemId) 
                      ? 'bg-blue-600/20 border-blue-500 shadow-lg shadow-blue-900/20' 
                      : 'bg-slate-800/50 border-slate-700 active:bg-slate-800'
                  }`}
                >
                  <div className="flex justify-between items-start">
                    <div className="flex items-start gap-3 flex-1">
                      {/* Checkbox (Left) */}
                      <div className={`mt-1 w-5 h-5 rounded-full border flex items-center justify-center transition-colors shrink-0 ${
                        selectedItems.has(item.ItemId) ? 'bg-blue-500 border-blue-500' : 'border-slate-500'
                      }`}>
                        {selectedItems.has(item.ItemId) && <Check className="w-3 h-3 text-white" />}
                      </div>

                      {/* Content (Middle) */}
                      <div className="flex-1 min-w-0">
                        <h3 className="text-white font-bold leading-snug">{item.Description}</h3>
                        <div className="flex flex-wrap items-center gap-2 mt-1">
                          <span className="text-xs bg-slate-700 text-slate-300 px-1.5 py-0.5 rounded">{item.Brand}</span>
                          
                          {/* Stock Indicator */}
                          {!isService && (
                              item.Quantity > 0 ? (
                                  <span className={`text-xs font-bold flex items-center gap-1 ${
                                      item.Quantity >= 4 ? 'text-emerald-400' : 
                                      item.Quantity >= 2 ? 'text-yellow-400' : 
                                      'text-rose-400'
                                  }`}>
                                      <div className={`w-1.5 h-1.5 rounded-full ${
                                          item.Quantity >= 4 ? 'bg-emerald-500' : 
                                          item.Quantity >= 2 ? 'bg-yellow-500' : 
                                          'bg-rose-500'
                                      }`} />
                                      Stock: {item.Quantity}
                                  </span>
                              ) : (
                                  <span className="text-xs font-bold text-rose-400 flex items-center gap-1">
                                      <div className="w-1.5 h-1.5 rounded-full bg-rose-500" />
                                      Out
                                  </span>
                              )
                          )}
                        </div>
                      </div>
                    </div>

                    {/* Price (Right) */}
                    <div className="text-right pl-2">
                         <div className="flex flex-col items-end">
                            {editingPriceId === item.ItemId ? (
                                <div className="flex items-center gap-1" onClick={e => e.stopPropagation()}>
                                    <input 
                                        type="number" 
                                        autoFocus
                                        className="w-20 bg-slate-900 border border-indigo-500 rounded px-1 py-0.5 text-white font-bold text-sm"
                                        value={tempPriceInput}
                                        onChange={e => setTempPriceInput(e.target.value)}
                                        onKeyDown={e => {
                                            if (e.key === 'Enter') {
                                                const val = parseFloat(tempPriceInput);
                                                if (!isNaN(val)) {
                                                    setCustomServicePrices(prev => ({ ...prev, [item.ItemId]: val }));
                                                }
                                                setEditingPriceId(null);
                                            }
                                        }}
                                    />
                                    <button 
                                        onClick={(e) => {
                                            e.stopPropagation();
                                            const val = parseFloat(tempPriceInput);
                                            if (!isNaN(val)) {
                                                setCustomServicePrices(prev => ({ ...prev, [item.ItemId]: val }));
                                            }
                                            setEditingPriceId(null);
                                        }}
                                        className="p-1 bg-indigo-600 rounded text-white"
                                    >
                                        <Check className="w-3 h-3" />
                                    </button>
                                </div>
                            ) : (
                                <p className={`font-bold ${focItems.has(item.ItemId) ? 'text-emerald-400' : 'text-emerald-400'}`}>
                                    {focItems.has(item.ItemId) ? 'FOC' : `Rs. ${calculatePrice(item).toLocaleString()}`}
                                </p>
                            )}
                            
                            <p className="text-[10px] text-slate-500 mt-0.5">
                                {isService ? (
                                    focItems.has(item.ItemId) ? 'Free' : 
                                    customServicePrices[item.ItemId] !== undefined ? 'Custom' : 'Standard'
                                ) : isMaxxis ? (
                                    maxxisDiscounts[item.ItemId] ? `-${maxxisDiscounts[item.ItemId]}%` : 'Fixed'
                                ) : (
                                    pricingMode === 'cost_plus' ? 'Cost+' :
                                    pricingMode === 'wholesale' ? 'Wholesale' :
                                    pricingMode === 'cash' ? 'Cash' :
                                    pricingMode === 'selling' ? 'Selling' :
                                    'Custom'
                                )}
                            </p>
                         </div>
                    </div>
                  </div>

                  {/* Controls Section (Maxxis / Service) */}
                  {(isMaxxis || isService) && (
                      <div className="mt-3 pt-2 border-t border-slate-700/50 flex justify-end gap-2">
                        {/* Maxxis Discount Toggles */}
                        {isMaxxis && (
                            <div className="flex gap-2">
                                <button
                                    onClick={(e) => toggleMaxxisDiscount(item.ItemId, 2.5, e)}
                                    className={`px-2 py-1 rounded text-[10px] font-bold border transition-all ${
                                        maxxisDiscounts[item.ItemId] === 2.5
                                        ? 'bg-emerald-500 border-emerald-400 text-white'
                                        : 'bg-slate-700 border-slate-600 text-slate-400 hover:bg-slate-600'
                                    }`}
                                >
                                    2.5%
                                </button>
                                <button
                                    onClick={(e) => toggleMaxxisDiscount(item.ItemId, 5, e)}
                                    className={`px-2 py-1 rounded text-[10px] font-bold border transition-all ${
                                        maxxisDiscounts[item.ItemId] === 5
                                        ? 'bg-emerald-500 border-emerald-400 text-white'
                                        : 'bg-slate-700 border-slate-600 text-slate-400 hover:bg-slate-600'
                                    }`}
                                >
                                    5%
                                </button>
                            </div>
                        )}

                        {/* Service Controls */}
                        {isService && (
                            <div className="flex gap-2 items-center">
                                <button
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        setFocItems(prev => {
                                            const next = new Set(prev);
                                            if (next.has(item.ItemId)) next.delete(item.ItemId);
                                            else next.add(item.ItemId);
                                            return next;
                                        });
                                    }}
                                    className={`px-2 py-1 rounded text-[10px] font-bold border transition-all flex items-center gap-1 ${
                                        focItems.has(item.ItemId)
                                        ? 'bg-emerald-500 border-emerald-400 text-white'
                                        : 'bg-slate-700 border-slate-600 text-slate-400 hover:bg-slate-600'
                                    }`}
                                >
                                    {focItems.has(item.ItemId) && <Check className="w-3 h-3" />}
                                    FOC
                                </button>
                                
                                <button
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        setTempPriceInput(calculatePrice(item).toString());
                                        setEditingPriceId(item.ItemId);
                                    }}
                                    className="p-1.5 rounded text-[10px] font-bold border bg-slate-700 border-slate-600 text-slate-400 hover:bg-slate-600 hover:text-white"
                                >
                                    <Edit className="w-3 h-3" />
                                </button>
                            </div>
                        )}
                      </div>
                  )}

                  {/* Last GRN Info */}
                  {item.LastGRN && (
                    <div className="mt-3 pt-2 border-t border-slate-700/50 flex flex-col gap-2 text-[10px] pl-8">
                        <div className="flex items-center justify-between">
                            <span className="text-slate-500">Last GRN: <span className="font-medium text-slate-400">{item.LastGRN.No}</span></span>
                            <div className="flex gap-3 items-center">
                                {(() => {
                                    const grnDate = new Date(item.LastGRN.Date);
                                    const now = new Date();
                                    const ageInYears = (now.getTime() - grnDate.getTime()) / (1000 * 60 * 60 * 24 * 365);
                                    
                                    let dateColor = 'text-emerald-400';
                                    let showWarning = false;
                                    
                                    if (ageInYears > 2) {
                                        dateColor = 'text-rose-400';
                                        showWarning = true;
                                    } else if (ageInYears > 1) {
                                        dateColor = 'text-rose-400';
                                    }

                                    return (
                                        <span className={`${dateColor} font-bold text-xs flex items-center gap-1`}>
                                            {showWarning && <AlertCircle size={12} className="text-rose-400" />}
                                            {grnDate.toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}
                                        </span>
                                    );
                                })()}
                                <span className="font-medium text-slate-400 bg-slate-700/50 px-1.5 py-0.5 rounded">Qty: {item.LastGRN.Qty}</span>
                            </div>
                        </div>

                        {/* Older Stock Breakdown */}
                        {(() => {
                            if (!item.LastGRN?.History || item.LastGRN.History.length <= 1) return null;
                            
                            const history = item.LastGRN.History;
                            let remainingStock = Math.max(0, item.Quantity - history[0].Qty);
                            const olderBatches = [];

                            // Start from the second GRN (index 1)
                            for (let i = 1; i < history.length && remainingStock > 0; i++) {
                                const batch = history[i];
                                const batchQty = batch.Qty;
                                const allocated = Math.min(remainingStock, batchQty);
                                
                                if (allocated > 0) {
                                    const batchDate = new Date(batch.InvoiceDate);
                                    const now = new Date();
                                    const ageInYears = (now.getTime() - batchDate.getTime()) / (1000 * 60 * 60 * 24 * 365);
                                    const isVeryOld = ageInYears > 2;

                                    olderBatches.push({
                                        qty: allocated,
                                        date: batchDate,
                                        isVeryOld
                                    });
                                    remainingStock -= allocated;
                                }
                            }

                            // If there's still stock left after checking all history, show a generic "Older" badge
                            if (remainingStock > 0) {
                                olderBatches.push({
                                    qty: remainingStock,
                                    date: null,
                                    isVeryOld: true // Assume unknown older stock is very old
                                });
                            }

                            if (olderBatches.length === 0) return null;

                            return (
                                <div className="flex flex-wrap gap-2 justify-end">
                                    {olderBatches.map((batch, idx) => (
                                        <span 
                                            key={idx}
                                            className={`text-[9px] font-bold px-1.5 py-0.5 rounded border flex items-center gap-1 ${
                                                batch.isVeryOld 
                                                ? 'text-rose-400 bg-rose-900/30 border-rose-700/50' 
                                                : 'text-amber-400 bg-amber-900/30 border-amber-700/50'
                                            }`}
                                        >
                                            {batch.qty} Older 
                                            {batch.date && (
                                                <span className={batch.isVeryOld ? 'text-rose-500' : 'text-amber-500'}>
                                                    ({batch.date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' })})
                                                </span>
                                            )}
                                        </span>
                                    ))}
                                </div>
                            );
                        })()}
                    </div>
                  )}
                </div>
                );
               })}               {/* Pricing Control Card */}
               <div className="p-4 bg-slate-900 rounded-xl border border-slate-700 mt-4 mb-20">
                    <h4 className="text-slate-400 text-xs font-bold uppercase tracking-wider mb-3">Pricing Mode</h4>
                    <div className="grid grid-cols-2 gap-2 mb-3">
                        <button
                            onClick={() => setPricingMode('cost_plus')}
                            className={`p-3 rounded-lg text-xs font-bold border transition-all ${
                                pricingMode === 'cost_plus' 
                                ? 'bg-indigo-600 border-indigo-500 text-white' 
                                : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                            }`}
                        >
                            Cost Price (+500)
                        </button>
                        <button
                            onClick={() => setPricingMode('wholesale')}
                            className={`p-3 rounded-lg text-xs font-bold border transition-all ${
                                pricingMode === 'wholesale' 
                                ? 'bg-indigo-600 border-indigo-500 text-white' 
                                : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                            }`}
                        >
                            Wholesale (+1000)
                        </button>
                        <button
                            onClick={() => setPricingMode('cash')}
                            className={`p-3 rounded-lg text-xs font-bold border transition-all ${
                                pricingMode === 'cash' 
                                ? 'bg-indigo-600 border-indigo-500 text-white' 
                                : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                            }`}
                        >
                            Cash Price (+1500)
                        </button>
                        <button
                            onClick={() => setPricingMode('selling')}
                            className={`p-3 rounded-lg text-xs font-bold border transition-all ${
                                pricingMode === 'selling' 
                                ? 'bg-indigo-600 border-indigo-500 text-white' 
                                : 'bg-slate-800 border-slate-700 text-slate-400 hover:bg-slate-700'
                            }`}
                        >
                            Selling Price (+2000)
                        </button>
                    </div>
                    
                    {/* Custom Input */}
                    <div className="relative">
                        <div className={`absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none ${pricingMode === 'custom' ? 'text-indigo-400' : 'text-slate-500'}`}>
                            <Tag className="w-4 h-4" />
                        </div>
                        <input
                            type="text"
                            value={customMarkup}
                            onChange={(e) => {
                                setCustomMarkup(e.target.value);
                                setPricingMode('custom');
                            }}
                            placeholder="Custom Markup (e.g. 500 or 10%)"
                            className={`w-full bg-slate-800 border text-sm rounded-lg block pl-10 p-2.5 outline-none transition-all ${
                                pricingMode === 'custom'
                                ? 'border-indigo-500 text-white ring-1 ring-indigo-500'
                                : 'border-slate-700 text-slate-300 focus:border-slate-500'
                            }`}
                        />
                    </div>
               </div>

               {/* Quick Services Bar */}
               <div className="mb-20">
                   <h4 className="text-slate-400 text-xs font-bold uppercase tracking-wider mb-3 px-1">Quick Services</h4>
                   <div className="flex gap-2 overflow-x-auto pb-2 no-scrollbar">
                       {[
                           { id: SERVICE_IDS.ALIGNMENT_CAR, label: 'Car', icon: Car },
                           { id: SERVICE_IDS.ALIGNMENT_JEEP, label: 'Jeep/Van', icon: Truck },
                           { id: SERVICE_IDS.ALIGNMENT_LORRY, label: 'Lorry', icon: Truck, flip: true },
                           { id: SERVICE_IDS.ALIGNMENT_BUS, label: 'Bus', icon: Bus },
                           { id: SERVICE_IDS.BALANCING, label: 'Balancing', icon: CircleDashed },
                           { id: SERVICE_IDS.TUBELESS_NECK, label: 'TL Neck', icon: CircleDot },
                       ].map((service) => {
                           const isSelected = results.some(r => r.ItemId === service.id);
                           const Icon = service.icon;
                           
                           return (
                               <button 
                                   key={service.id}
                                   onClick={() => addServiceById(service.id)}
                                   className={`flex items-center gap-2 px-4 py-3 border rounded-xl whitespace-nowrap transition-all ${
                                       isSelected 
                                       ? 'bg-indigo-600 border-indigo-500 shadow-lg shadow-indigo-900/20' 
                                       : 'bg-slate-900 border-slate-700 hover:bg-slate-800 active:scale-95'
                                   }`}
                               >
                                   <Icon className={`w-4 h-4 ${isSelected ? 'text-white' : 'text-indigo-400'} ${service.flip ? 'scale-x-[-1]' : ''}`} />
                                   <span className={`text-xs font-bold ${isSelected ? 'text-white' : 'text-slate-300'}`}>
                                       {service.label}
                                   </span>
                                   {isSelected && <Check className="w-3 h-3 text-white ml-1" />}
                               </button>
                           );
                       })}
                   </div>
               </div>
            </div>

            {/* Footer Actions */}
            {selectedItems.size > 0 && (
            <div className="p-4 bg-slate-900 border-t border-slate-700 shrink-0 shadow-[0_-10px_30px_rgba(0,0,0,0.5)] animate-in slide-in-from-bottom-4 duration-200 z-50">
                {/* Summary Row */}
                <div className="flex items-center justify-between mb-4 px-1">
                    <div className="flex flex-col">
                        <span className="text-slate-400 text-[10px] font-bold uppercase tracking-wider">Selected</span>
                        <span className="text-white font-bold text-lg">{selectedItems.size} Items</span>
                    </div>
                    <div className="flex flex-col items-end">
                        <span className="text-slate-400 text-[10px] font-bold uppercase tracking-wider">Total Value</span>
                        <span className="text-emerald-400 font-bold text-lg">
                            Rs {results.filter(r => selectedItems.has(r.ItemId)).reduce((sum, item) => sum + calculatePrice(item), 0).toLocaleString()}
                        </span>
                    </div>
                </div>

                {/* Action Buttons */}
                <div className="flex gap-3">
                    <button 
                        onClick={() => setSelectedItems(new Set())}
                        className="p-3.5 bg-slate-800 hover:bg-rose-900/20 text-slate-400 hover:text-rose-400 rounded-xl font-bold border border-slate-700 transition-all active:scale-95"
                        title="Clear Selection"
                    >
                        <Trash2 className="w-5 h-5" />
                    </button>
                    
                    <button
                        onClick={handleShareSelected}
                        disabled={isSharing}
                        className={`flex-1 py-3.5 rounded-xl font-bold shadow-lg transition-all active:scale-95 flex items-center justify-center gap-2 ${
                            isSharing
                                ? 'bg-emerald-500/70 text-white cursor-not-allowed'
                                : 'bg-emerald-600 hover:bg-emerald-500 text-white shadow-emerald-600/20'
                        }`}
                    >
                        {isSharing ? (
                            <Loader2 className="w-5 h-5 animate-spin" />
                        ) : (
                            <Share className="w-5 h-5" />
                        )}
                        {isSharing ? 'Sharing...' : 'Share'}
                    </button>

                    <button 
                        onClick={handleAddSelected}
                        className="flex-1 py-3.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl font-bold shadow-lg shadow-indigo-600/20 transition-all active:scale-95 flex items-center justify-center gap-2"
                    >
                        <ShoppingCart className="w-5 h-5" />
                        Add
                    </button>
                </div>
            </div>
            )}
        </div>
      )}

      {/* Voice Listening Overlay */}
      {isListening && (
        <div className="fixed inset-0 z-[100] bg-slate-950/95 backdrop-blur-md flex flex-col items-center justify-center p-6 animate-in fade-in duration-200">
            <div className="relative mb-10">
                <div className="absolute inset-0 bg-indigo-500 rounded-full blur-2xl opacity-40 animate-pulse" />
                <div className="relative bg-gradient-to-br from-indigo-600 to-violet-600 p-8 rounded-full shadow-2xl shadow-indigo-500/30 border border-white/10">
                    <Mic className="w-12 h-12 text-white animate-bounce" />
                </div>
            </div>
            
            <h3 className="text-3xl font-bold text-white mb-3 text-center tracking-tight">Listening...</h3>
            <p className="text-indigo-200 text-center mb-10 max-w-xs text-lg leading-relaxed">
                Say the size clearly<br/>
                <span className="text-white font-mono font-bold bg-white/10 px-2 py-1 rounded mt-2 inline-block">"175 70 13"</span>
            </p>

            <div className="text-4xl font-bold text-white mb-12 min-h-[3rem] flex items-center justify-center text-center px-4 break-words w-full">
                {transcript === 'Listening...' ? <span className="opacity-30 animate-pulse">...</span> : transcript}
            </div>

            <button 
                onClick={stopListening}
                className="px-10 py-4 bg-slate-800 hover:bg-slate-700 text-white rounded-2xl font-bold transition-all active:scale-95 border border-slate-700 shadow-lg"
            >
                Cancel
            </button>
        </div>
      )}
    </div>
  );
}

