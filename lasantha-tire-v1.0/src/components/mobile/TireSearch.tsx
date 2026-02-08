'use client';

import { useState, useEffect } from 'react';
import { Search, Mic, ShoppingCart, Loader2, AlertCircle, X } from 'lucide-react';

interface TireProduct {
  ItemId: string;
  Description: string;
  Brand: string;
  Quantity: number;
  Price?: number;
}

export default function TireSearch({ onAddToQuote }: { onAddToQuote: (item: TireProduct) => void }) {
  const [width, setWidth] = useState('');
  const [profile, setProfile] = useState('');
  const [rim, setRim] = useState('');
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<TireProduct[]>([]);
  const [isListening, setIsListening] = useState(false);
  const [transcript, setTranscript] = useState('');
  const [micError, setMicError] = useState('');
  const [searchInput, setSearchInput] = useState('');

  // Common tire sizes for quick selection
  const widths = ['145', '155', '165', '175', '185', '195', '205', '215', '225', '235', '245', '255', '265'];
  const profiles = ['40', '45', '50', '55', '60', '65', '70', '75', '80', '85'];
  const rims = ['12', '13', '14', '15', '16', '17', '18', '19', '20', '22.5'];

  const handleSearch = async () => {
    if (!width || !profile || !rim) return;
    
    setLoading(true);
    try {
      const query = `${width}/${profile}`;
      const res = await fetch(`/api/erp/inventory?type=search&query=${encodeURIComponent(query)}&rim=${rim}`);
      if (res.ok) {
        const data = await res.json();
        const filtered = data.data.filter((item: any) => 
          item.Description.includes(`R${rim}`) || item.Description.includes(`R ${rim}`) || item.Description.includes(`-${rim}`)
        );
        setResults(filtered);
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
    if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
      try {
        const SpeechRecognition = (window as any).webkitSpeechRecognition || (window as any).SpeechRecognition;
        const recognition = new SpeechRecognition();
        recognition.lang = 'si-LK';
        recognition.continuous = false;
        recognition.interimResults = true;

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
            parseInput(currentTranscript);
            setIsListening(false);
            setTranscript('');
          }
        };

        recognition.onerror = (event: any) => {
          console.error('Speech recognition error', event.error);
          setIsListening(false);
          if (event.error === 'not-allowed' || event.error === 'permission-denied') {
            setMicError('Microphone access denied. Please enable it in settings.');
          } else if (event.error === 'no-speech') {
            setTranscript('');
          } else {
            setTranscript('Error: ' + event.error);
          }
        };

        recognition.onend = () => {
          setIsListening(false);
        };

        recognition.start();
      } catch (e) {
        console.error(e);
        setMicError('Voice search failed to start.');
      }
    } else {
      setMicError('Voice search not supported in this browser.');
    }
  };

  const parseInput = (text: string) => {
    // Update search input visual
    setSearchInput(text);

    // Normalize text
    let normalized = text.toLowerCase()
      .replace(/by|into|x|\/|-|dash|slash/g, ' ')
      .replace(/one/g, '1')
      .replace(/two/g, '2')
      .replace(/three/g, '3')
      .replace(/four/g, '4')
      .replace(/five/g, '5')
      .replace(/six/g, '6')
      .replace(/seven/g, '7')
      .replace(/eight/g, '8')
      .replace(/nine/g, '9')
      .replace(/zero/g, '0');

    const numbers = normalized.match(/\d+/g);
    
    if (numbers && numbers.length >= 3) {
      const w = numbers.find(n => parseInt(n) > 100 && parseInt(n) < 400);
      const p = numbers.find(n => parseInt(n) > 20 && parseInt(n) < 100 && n !== w);
      const r = numbers.find(n => parseInt(n) >= 10 && parseInt(n) <= 24 && n !== w && n !== p);

      if (w && p && r) {
        setWidth(w);
        setProfile(p);
        setRim(r);
      } else if (numbers.length >= 3) {
        setWidth(numbers[0]);
        setProfile(numbers[1]);
        setRim(numbers[2]);
      }
    }
  };

  const handleManualInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setSearchInput(val);
    
    // Debounce parsing
    const timeoutId = setTimeout(() => {
      parseInput(val);
    }, 500);
    return () => clearTimeout(timeoutId);
  };

  useEffect(() => {
    if (width && profile && rim) {
      handleSearch();
    }
  }, [width, profile, rim]);

  return (
    <div className="space-y-6">
      {/* Hero Search Section */}
      <div className="relative bg-gradient-to-br from-indigo-600 to-violet-700 rounded-[2rem] p-6 shadow-xl shadow-indigo-500/20 overflow-hidden">
        {/* Background Decoration */}
        <div className="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full blur-3xl -mr-16 -mt-16" />
        <div className="absolute bottom-0 left-0 w-48 h-48 bg-black/10 rounded-full blur-2xl -ml-12 -mb-12" />

        <div className="relative z-10">
          <h2 className="text-white font-bold text-xl mb-4 flex items-center gap-2">
            <Search className="w-6 h-6 text-indigo-200" />
            Find Your Tires
          </h2>

          {/* Main Search Input */}
          <div className="relative group">
            <input
              type="text"
              value={searchInput}
              onChange={handleManualInput}
              placeholder="e.g. 175/70R13"
              className="w-full bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl py-4 pl-5 pr-14 text-white placeholder:text-indigo-200 text-lg font-medium focus:outline-none focus:bg-white/20 focus:border-white/40 transition-all shadow-inner"
            />
            
            {/* Clear Button */}
            {searchInput && (
              <button
                onClick={() => {
                  setSearchInput('');
                  setWidth(''); setProfile(''); setRim(''); setResults([]);
                }}
                className="absolute right-12 top-1/2 -translate-y-1/2 p-2 text-indigo-200 hover:text-white transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            )}

            {/* Mic Button inside Input */}
            <button
              onClick={startListening}
              className={`absolute right-2 top-1/2 -translate-y-1/2 p-2.5 rounded-xl transition-all ${
                isListening 
                  ? 'bg-red-500 text-white animate-pulse shadow-lg shadow-red-500/40' 
                  : 'bg-white/10 text-white hover:bg-white/20'
              }`}
            >
              <Mic className="w-5 h-5" />
            </button>
          </div>

          {/* Live Transcript / Status */}
          <div className="h-6 mt-2 flex items-center">
            {isListening ? (
              <span className="text-xs text-indigo-200 animate-pulse font-mono flex items-center gap-2">
                <span className="w-2 h-2 bg-red-400 rounded-full animate-ping" />
                {transcript || 'Listening...'}
              </span>
            ) : micError ? (
              <span className="text-xs text-red-300 flex items-center gap-1">
                <AlertCircle className="w-3 h-3" /> {micError}
              </span>
            ) : (
              <span className="text-xs text-indigo-300/60">
                Type size or tap mic to speak
              </span>
            )}
          </div>
        </div>
      </div>



      {/* Results List */}
      <div className="space-y-3">
        {loading ? (
          <div className="flex flex-col items-center justify-center py-12 text-zinc-400">
            <Loader2 className="w-8 h-8 animate-spin mb-2 text-indigo-500" />
            <span className="text-sm font-medium">Searching inventory...</span>
          </div>
        ) : results.length > 0 ? (
          <>
            <div className="flex items-center justify-between px-2">
              <span className="text-sm font-bold text-zinc-900">{results.length} Results Found</span>
            </div>
            {results.map((item, idx) => (
              <div key={idx} className="bg-white rounded-2xl p-4 shadow-sm border border-zinc-100 flex items-center justify-between group active:scale-[0.98] transition-all">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <span className="px-2 py-0.5 rounded-md bg-zinc-100 text-zinc-600 text-[10px] font-bold uppercase tracking-wider">
                      {item.Brand}
                    </span>
                    {item.Quantity > 0 ? (
                      <span className="text-[10px] font-bold text-emerald-600 flex items-center gap-1">
                        <span className="w-1.5 h-1.5 rounded-full bg-emerald-500" />
                        In Stock ({item.Quantity})
                      </span>
                    ) : (
                      <span className="text-[10px] font-bold text-rose-500 flex items-center gap-1">
                        <span className="w-1.5 h-1.5 rounded-full bg-rose-500" />
                        Out of Stock
                      </span>
                    )}
                  </div>
                  <div className="text-zinc-900 font-medium text-sm">{item.Description}</div>
                </div>
                
                <div className="flex flex-col items-end gap-2">
                  <div className="text-lg font-bold text-indigo-600">
                    Rs {item.Price ? item.Price.toLocaleString() : 'N/A'}
                  </div>
                  <button
                    onClick={() => onAddToQuote(item)}
                    className="px-3 py-1.5 bg-zinc-900 text-white text-xs font-bold rounded-lg flex items-center gap-1.5 hover:bg-zinc-800 transition-colors shadow-lg shadow-zinc-900/20"
                  >
                    <ShoppingCart className="w-3 h-3" />
                    Add
                  </button>
                </div>
              </div>
            ))}
          </>
        ) : (width && profile && rim) ? (
          <div className="bg-white rounded-2xl p-8 text-center border border-zinc-100 shadow-sm">
            <div className="w-12 h-12 bg-zinc-50 rounded-full flex items-center justify-center mx-auto mb-3">
              <Search className="w-6 h-6 text-zinc-300" />
            </div>
            <h3 className="text-zinc-900 font-bold mb-1">No Tires Found</h3>
            <p className="text-zinc-500 text-sm">Try checking the size or brand again.</p>
          </div>
        ) : null}
      </div>
    </div>
  );
}
