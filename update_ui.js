const fs = require('fs');
const path = require('path');

const filePath = path.join('apps', 'invoice', 'app', 'invoices', 'new', 'page.tsx');

const newContent = `'use client';

import { useState, useEffect, useRef } from 'react';
import Link from 'next/link';

interface Customer {
  CutomerID: string;
  CustomerName: string;
  Address1: string;
  Phone1: string;
  Balance: number;
}

interface Item {
  ItemID: string;
  ItemDescription: string;
  UnitPrice: number;
  UOM: string;
  stock: { WhseId: string; QTY: number }[];
}

interface Warehouse {
  LocationID: string;
  LocationDesc: string;
}

interface SalesRep {
  RepCode: string;
  RepName: string;
}

interface InvoiceLine {
  id: number;
  itemId: string;
  description: string;
  listPrice: number;
  qtyOnHand: number;
  quantity: number;
  focQty: number;
  totalQty: number;
  discountPercent: number;
  discountAmount: number;
  total: number;
}

export default function NewInvoice() {
  // --- Master Data State ---
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [items, setItems] = useState<Item[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [salesReps, setSalesReps] = useState<SalesRep[]>([]);

  // --- Form State (Header) ---
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [customerSearch, setCustomerSearch] = useState('');
  const [billTo, setBillTo] = useState('');
  const [vehicleNo, setVehicleNo] = useState('');
  const [mileage, setMileage] = useState('');
  const [contactNo, setContactNo] = useState('');
  const [patientName, setPatientName] = useState(''); // "Name" field
  const [jobDoneBy, setJobDoneBy] = useState('');
  
  const [paymentMethod, setPaymentMethod] = useState('Cash'); // Cash, Credit, Card
  const [taxType, setTaxType] = useState('NON-VAT'); // NON-VAT, VAT, SVAT
  const [customerPO, setCustomerPO] = useState('');
  
  const [printer, setPrinter] = useState('Dot Matrix');
  const [invoiceNo, setInvoiceNo] = useState('NEW');
  const [invoiceDate, setInvoiceDate] = useState(new Date().toISOString().split('T')[0]);
  const [selectedSalesRep, setSelectedSalesRep] = useState('');
  const [invoiceType, setInvoiceType] = useState('Non VAT');
  const [invoiceMode, setInvoiceMode] = useState('Cash');
  const [selectedWarehouse, setSelectedWarehouse] = useState('');
  const [withDiscount, setWithDiscount] = useState(false);

  // --- Form State (Lines) ---
  const [lines, setLines] = useState<InvoiceLine[]>([]);
  const [itemSearch, setItemSearch] = useState('');
  
  // --- Form State (Footer) ---
  const [useItems, setUseItems] = useState('');
  const [useItemsAmount, setUseItemsAmount] = useState(0);
  const [warrantyKm, setWarrantyKm] = useState('');
  const [warrantyYear, setWarrantyYear] = useState('');
  const [remarks, setRemarks] = useState('');
  
  const [nbtPercent, setNbtPercent] = useState(0);
  const [vatPercent, setVatPercent] = useState(0);
  const [paidAmount, setPaidAmount] = useState(0);
  const [cashTendered, setCashTendered] = useState(0);

  // --- UI State ---
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // --- Effects ---
  useEffect(() => {
    fetch('/api/master-data')
      .then(res => res.json())
      .then(data => {
        setWarehouses(data.warehouses);
        setSalesReps(data.salesReps);
        if (data.warehouses.length > 0) setSelectedWarehouse(data.warehouses[0].LocationID);
      });
  }, []);

  useEffect(() => {
    if (customerSearch.length > 2) {
      fetch(\`/api/customers?search=\${customerSearch}\`)
        .then(res => res.json())
        .then(setCustomers);
    }
  }, [customerSearch]);

  useEffect(() => {
    if (itemSearch.length > 2) {
      fetch(\`/api/items?search=\${itemSearch}\`)
        .then(res => res.json())
        .then(setItems);
    }
  }, [itemSearch]);

  // --- Logic ---

  const addLine = (item: Item) => {
    const stockItem = item.stock.find(s => s.WhseId === selectedWarehouse);
    const qtyOnHand = stockItem ? stockItem.QTY : 0;

    setLines([...lines, {
      id: Date.now(),
      itemId: item.ItemID,
      description: item.ItemDescription,
      listPrice: item.UnitPrice,
      qtyOnHand: qtyOnHand,
      quantity: 1,
      focQty: 0,
      totalQty: 1,
      discountPercent: 0,
      discountAmount: 0,
      total: item.UnitPrice
    }]);
    setItemSearch('');
    setItems([]);
  };

  const updateLine = (id: number, field: keyof InvoiceLine, value: number) => {
    setLines(lines.map(line => {
      if (line.id === id) {
        const updatedLine = { ...line, [field]: value };
        
        // Recalculate dependent fields
        if (field === 'quantity' || field === 'focQty') {
          updatedLine.totalQty = updatedLine.quantity + updatedLine.focQty;
        }

        if (field === 'quantity' || field === 'listPrice' || field === 'discountPercent') {
           const gross = updatedLine.quantity * updatedLine.listPrice;
           updatedLine.discountAmount = (gross * updatedLine.discountPercent) / 100;
           updatedLine.total = gross - updatedLine.discountAmount;
        }

        return updatedLine;
      }
      return line;
    }));
  };

  const removeLine = (id: number) => {
    setLines(lines.filter(line => line.id !== id));
  };

  // --- Totals Calculation ---
  const subTotal = lines.reduce((sum, line) => sum + line.total, 0);
  const nbtAmount = (subTotal * nbtPercent) / 100;
  const vatAmount = ((subTotal + nbtAmount) * vatPercent) / 100;
  const invoiceTotal = subTotal + nbtAmount + vatAmount;
  const balance = invoiceTotal - paidAmount;

  const saveInvoice = async () => {
    if (!selectedCustomer || !selectedWarehouse || !selectedSalesRep || lines.length === 0) {
      setError('Please fill in all required fields and add at least one item.');
      return;
    }

    setIsSaving(true);
    setError('');
    setSuccess('');

    try {
      const response = await fetch('/api/invoices', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          customer: selectedCustomer,
          date: invoiceDate,
          warehouse: selectedWarehouse,
          salesRep: selectedSalesRep,
          lines,
          
          // New Fields
          vehicleNo,
          mileage,
          contactNo,
          patientName,
          jobDoneBy,
          paymentMethod,
          taxType,
          customerPO,
          invoiceType,
          invoiceMode,
          warrantyKm,
          warrantyYear,
          remarks,
          
          // Totals
          subTotal,
          nbtAmount,
          vatAmount,
          invoiceTotal,
          paidAmount,
          balance
        })
      });

      const data = await response.json();

      if (response.ok) {
        setSuccess(\`Invoice saved successfully! Invoice No: \${data.invoiceNo}\`);
        setLines([]);
        setInvoiceNo(data.invoiceNo);
        // Optional: Reset other fields
      } else {
        setError(data.error || 'Failed to save invoice');
      }
    } catch (err) {
      setError('An error occurred while saving the invoice');
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 text-xs font-sans">
      {/* Toolbar */}
      <div className="bg-gradient-to-b from-gray-100 to-gray-300 border-b border-gray-400 p-1 flex space-x-2 items-center shadow-sm">
        <button className="flex items-center px-2 py-1 hover:bg-gray-200 rounded border border-transparent hover:border-gray-400 text-red-600 font-bold">
          <span className="mr-1">ⓧ</span> Close
        </button>
        <button className="flex items-center px-2 py-1 hover:bg-gray-200 rounded border border-transparent hover:border-gray-400 text-blue-600 font-bold" onClick={() => window.location.reload()}>
          <span className="mr-1">📄</span> New
        </button>
        <button className="flex items-center px-2 py-1 hover:bg-gray-200 rounded border border-transparent hover:border-gray-400">
          <span className="mr-1">📋</span> List
        </button>
        <button className="flex items-center px-2 py-1 hover:bg-gray-200 rounded border border-transparent hover:border-gray-400 text-blue-800 font-bold" onClick={saveInvoice} disabled={isSaving}>
          <span className="mr-1">💾</span> {isSaving ? 'Saving...' : 'Save/Update'}
        </button>
        <div className="border-l border-gray-400 h-6 mx-2"></div>
        <button className="px-2 py-1 text-gray-500">Process</button>
        <button className="px-2 py-1 text-gray-500">Edit</button>
        <div className="border-l border-gray-400 h-6 mx-2"></div>
        <button className="px-2 py-1">Direct-Print</button>
        <button className="px-2 py-1">Direct-POS Print</button>
        <button className="px-2 py-1">Print</button>
        <button className="px-2 py-1 text-gray-500">Void</button>
        
        <div className="flex-grow"></div>
        <div className="border border-gray-400 px-2 py-0.5 bg-white font-bold">Customer Invoice</div>
      </div>

      <div className="p-2">
        <div className="bg-white border border-gray-400 p-2 shadow-sm">
          <fieldset className="border border-gray-300 p-2 mb-2">
            <legend className="px-1 text-blue-800 font-bold">Sales Invoice</legend>
            
            <div className="grid grid-cols-12 gap-4">
              {/* Left Column */}
              <div className="col-span-4 space-y-1">
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">CustomerID</label>
                  <div className="flex-grow relative">
                    <input 
                      type="text" 
                      className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500"
                      value={customerSearch}
                      onChange={(e) => setCustomerSearch(e.target.value)}
                      placeholder="Search..."
                    />
                    {customers.length > 0 && !selectedCustomer && (
                      <ul className="absolute z-50 left-0 top-6 w-full bg-white border border-gray-400 shadow-lg max-h-40 overflow-auto">
                        {customers.map(c => (
                          <li key={c.CutomerID} className="px-2 py-1 hover:bg-blue-100 cursor-pointer" onClick={() => {
                            setSelectedCustomer(c);
                            setCustomerSearch(c.CutomerID);
                            setBillTo(c.CustomerName + '\\n' + c.Address1);
                            setContactNo(c.Phone1);
                            setCustomers([]);
                          }}>
                            {c.CutomerID} - {c.CustomerName}
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2"></label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 bg-gray-50" readOnly value={selectedCustomer?.CustomerName || ''} />
                </div>
                <div className="flex items-start">
                  <label className="w-24 text-right pr-2 mt-1">Bill to</label>
                  <textarea className="w-full border border-gray-300 px-1 h-16 resize-none focus:outline-none focus:border-blue-500" value={billTo} onChange={e => setBillTo(e.target.value)}></textarea>
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Vehicle No</label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={vehicleNo} onChange={e => setVehicleNo(e.target.value)} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Mileage</label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={mileage} onChange={e => setMileage(e.target.value)} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Contact No</label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={contactNo} onChange={e => setContactNo(e.target.value)} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Name</label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={patientName} onChange={e => setPatientName(e.target.value)} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Job done by</label>
                  <select className="w-full border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={jobDoneBy} onChange={e => setJobDoneBy(e.target.value)}>
                    <option value="">Select...</option>
                    {salesReps.map(r => <option key={r.RepCode} value={r.RepCode}>{r.RepName}</option>)}
                  </select>
                </div>
              </div>

              {/* Middle Column */}
              <div className="col-span-5 space-y-2">
                <fieldset className="border border-gray-300 p-1">
                  <legend className="text-gray-600 px-1">Payment Method</legend>
                  <div className="flex space-x-4">
                    {['Cash', 'Credit', 'Card'].map(m => (
                      <label key={m} className="flex items-center space-x-1 cursor-pointer font-bold">
                        <input type="radio" name="paymentMethod" checked={paymentMethod === m} onChange={() => setPaymentMethod(m)} />
                        <span>{m}</span>
                      </label>
                    ))}
                  </div>
                </fieldset>

                <div className="flex space-x-4 py-1">
                  {['NON-VAT', 'VAT', 'SVAT'].map(t => (
                    <label key={t} className="flex items-center space-x-1 cursor-pointer font-bold">
                      <input type="radio" name="taxType" checked={taxType === t} onChange={() => setTaxType(t)} />
                      <span>{t}</span>
                    </label>
                  ))}
                </div>

                <fieldset className="border border-gray-300 p-1 h-32 bg-orange-50">
                  <legend className="text-gray-600 px-1">Multiple Payment Mode</legend>
                  <table className="w-full text-xs bg-white border border-gray-300">
                    <thead className="bg-gray-100">
                      <tr>
                        <th className="border p-1">Card Name</th>
                        <th className="border p-1">CardNo</th>
                        <th className="border p-1">Amount</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td className="border p-1 h-6"></td>
                        <td className="border p-1"></td>
                        <td className="border p-1"></td>
                      </tr>
                    </tbody>
                  </table>
                  <div className="flex justify-end items-center mt-2">
                    <span className="mr-2 text-gray-600">Total Amount</span>
                    <input type="text" className="w-24 border border-gray-300 px-1 h-6 text-right bg-gray-100" readOnly value="0.00" />
                  </div>
                </fieldset>

                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Customer PO</label>
                  <input type="text" className="flex-grow border border-gray-300 px-1 h-6 focus:outline-none focus:border-blue-500" value={customerPO} onChange={e => setCustomerPO(e.target.value)} />
                  <button className="ml-2 px-2 py-0.5 border border-gray-400 bg-gray-100 hover:bg-gray-200 font-bold">Item Browser</button>
                  <a href="#" className="ml-2 text-blue-600 hover:underline">View Vehicle History</a>
                  <label className="ml-2 flex items-center space-x-1">
                    <input type="checkbox" checked={withDiscount} onChange={e => setWithDiscount(e.target.checked)} />
                    <span>With Discount</span>
                  </label>
                </div>
              </div>

              {/* Right Column */}
              <div className="col-span-3 space-y-1">
                <fieldset className="border border-gray-300 p-1 mb-2">
                  <legend className="text-gray-600 px-1">Default Printer</legend>
                  <div className="flex space-x-2">
                    <label className="flex items-center space-x-1 cursor-pointer">
                      <input type="radio" name="printer" checked={printer === 'Dot Matrix'} onChange={() => setPrinter('Dot Matrix')} />
                      <span>Dot Matrix</span>
                    </label>
                    <label className="flex items-center space-x-1 cursor-pointer">
                      <input type="radio" name="printer" checked={printer === 'Laser'} onChange={() => setPrinter('Laser')} />
                      <span>Laser</span>
                    </label>
                  </div>
                </fieldset>

                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Invoice No</label>
                  <input type="text" className="w-full border border-gray-300 px-1 h-6 bg-white" readOnly value={invoiceNo} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Date</label>
                  <input type="date" className="w-full border border-gray-300 px-1 h-6" value={invoiceDate} onChange={e => setInvoiceDate(e.target.value)} />
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Sales Rep</label>
                  <select className="w-full border border-gray-300 px-1 h-6" value={selectedSalesRep} onChange={e => setSelectedSalesRep(e.target.value)}>
                    <option value="">Select...</option>
                    {salesReps.map(r => <option key={r.RepCode} value={r.RepCode}>{r.RepName}</option>)}
                  </select>
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Invoice type</label>
                  <select className="w-full border border-gray-300 px-1 h-6" value={invoiceType} onChange={e => setInvoiceType(e.target.value)}>
                    <option>Non VAT</option>
                    <option>VAT</option>
                  </select>
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Invoice Mode</label>
                  <select className="w-full border border-gray-300 px-1 h-6" value={invoiceMode} onChange={e => setInvoiceMode(e.target.value)}>
                    <option>Cash</option>
                    <option>Credit</option>
                  </select>
                </div>
                <div className="flex items-center">
                  <label className="w-24 text-right pr-2">Warehouse</label>
                  <select className="w-full border border-gray-300 px-1 h-6" value={selectedWarehouse} onChange={e => setSelectedWarehouse(e.target.value)}>
                    {warehouses.map(w => <option key={w.LocationID} value={w.LocationID}>{w.LocationDesc}</option>)}
                  </select>
                </div>
              </div>
            </div>
          </fieldset>

          {/* Grid */}
          <div className="border border-gray-300 bg-orange-100 min-h-[300px] flex flex-col">
            <div className="overflow-x-auto">
              <table className="w-full text-xs border-collapse">
                <thead className="bg-white">
                  <tr>
                    <th className="border border-gray-300 px-2 py-1 w-8">Line</th>
                    <th className="border border-gray-300 px-2 py-1 text-left">Description</th>
                    <th className="border border-gray-300 px-2 py-1 w-20 text-right">List Price</th>
                    <th className="border border-gray-300 px-2 py-1 w-20 text-right">Qty On Han</th>
                    <th className="border border-gray-300 px-2 py-1 w-16 text-right">Quantity</th>
                    <th className="border border-gray-300 px-2 py-1 w-16 text-right">FOC Qty</th>
                    <th className="border border-gray-300 px-2 py-1 w-16 text-right">TotalQty</th>
                    <th className="border border-gray-300 px-2 py-1 w-16 text-right">Discount%</th>
                    <th className="border border-gray-300 px-2 py-1 w-20 text-right">LineDisc</th>
                    <th className="border border-gray-300 px-2 py-1 w-24 text-right">Line Total</th>
                    <th className="border border-gray-300 px-2 py-1 w-8"></th>
                  </tr>
                </thead>
                <tbody>
                  {lines.map((line, index) => (
                    <tr key={line.id} className="bg-white hover:bg-blue-50">
                      <td className="border border-gray-300 px-2 py-1 text-center">{index + 1}</td>
                      <td className="border border-gray-300 px-2 py-1">{line.description}</td>
                      <td className="border border-gray-300 px-2 py-1 text-right">{line.listPrice.toFixed(2)}</td>
                      <td className="border border-gray-300 px-2 py-1 text-right">{line.qtyOnHand}</td>
                      <td className="border border-gray-300 px-0 py-0">
                        <input 
                          type="number" 
                          className="w-full h-full text-right px-1 focus:outline-none focus:bg-yellow-50"
                          value={line.quantity}
                          onChange={e => updateLine(line.id, 'quantity', parseFloat(e.target.value) || 0)}
                        />
                      </td>
                      <td className="border border-gray-300 px-0 py-0">
                        <input 
                          type="number" 
                          className="w-full h-full text-right px-1 focus:outline-none focus:bg-yellow-50"
                          value={line.focQty}
                          onChange={e => updateLine(line.id, 'focQty', parseFloat(e.target.value) || 0)}
                        />
                      </td>
                      <td className="border border-gray-300 px-2 py-1 text-right">{line.totalQty}</td>
                      <td className="border border-gray-300 px-0 py-0">
                        <input 
                          type="number" 
                          className="w-full h-full text-right px-1 focus:outline-none focus:bg-yellow-50"
                          value={line.discountPercent}
                          onChange={e => updateLine(line.id, 'discountPercent', parseFloat(e.target.value) || 0)}
                        />
                      </td>
                      <td className="border border-gray-300 px-2 py-1 text-right">{line.discountAmount.toFixed(2)}</td>
                      <td className="border border-gray-300 px-2 py-1 text-right font-bold">{line.total.toFixed(2)}</td>
                      <td className="border border-gray-300 px-1 py-1 text-center">
                        <button onClick={() => removeLine(line.id)} className="text-red-600 font-bold">×</button>
                      </td>
                    </tr>
                  ))}
                  {/* Input Row */}
                  <tr className="bg-white">
                    <td className="border border-gray-300 px-2 py-1 text-center">+</td>
                    <td className="border border-gray-300 px-0 py-0 relative">
                      <input 
                        type="text" 
                        className="w-full h-full px-2 py-1 focus:outline-none focus:bg-yellow-50"
                        placeholder="Type to search item..."
                        value={itemSearch}
                        onChange={e => setItemSearch(e.target.value)}
                      />
                      {items.length > 0 && (
                        <ul className="absolute z-50 left-0 top-full w-full bg-white border border-gray-400 shadow-lg max-h-60 overflow-auto">
                          {items.map(item => (
                            <li key={item.ItemID} className="px-2 py-1 hover:bg-blue-100 cursor-pointer border-b" onClick={() => addLine(item)}>
                              <div className="flex justify-between font-bold">
                                <span>{item.ItemDescription}</span>
                                <span>{item.UnitPrice.toFixed(2)}</span>
                              </div>
                              <div className="text-gray-500 text-[10px] flex justify-between">
                                <span>{item.ItemID}</span>
                                <span>Stock: {item.stock.find(s => s.WhseId === selectedWarehouse)?.QTY || 0}</span>
                              </div>
                            </li>
                          ))}
                        </ul>
                      )}
                    </td>
                    <td colSpan={9} className="border border-gray-300 bg-gray-50"></td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          {/* Footer */}
          <div className="mt-2 grid grid-cols-12 gap-4">
            {/* Left Footer */}
            <div className="col-span-6 space-y-2">
              <div className="flex items-center bg-orange-100 p-1 border border-orange-200">
                <label className="w-32 font-bold">For the Use Items</label>
                <input type="text" className="flex-grow border border-gray-300 px-1 h-6 mx-2" value={useItems} onChange={e => setUseItems(e.target.value)} />
                <input type="text" className="w-24 border border-gray-300 px-1 h-6 text-right" value={useItemsAmount} onChange={e => setUseItemsAmount(parseFloat(e.target.value) || 0)} />
              </div>
              
              <div className="flex items-center">
                <label className="w-32 border border-gray-300 px-1 h-6 bg-gray-50 flex items-center">Warrenty Period Km</label>
                <input type="text" className="w-32 border border-gray-300 px-1 h-6 ml-2" value={warrantyKm} onChange={e => setWarrantyKm(e.target.value)} />
              </div>
              <div className="flex items-center">
                <label className="w-32 border border-gray-300 px-1 h-6 bg-gray-50 flex items-center">Warrenty Period Year</label>
                <input type="text" className="w-32 border border-gray-300 px-1 h-6 ml-2" value={warrantyYear} onChange={e => setWarrantyYear(e.target.value)} />
              </div>
              <div className="flex items-start">
                <label className="w-16 border border-gray-300 px-1 h-6 bg-gray-50 flex items-center">Remarks</label>
                <textarea className="flex-grow border border-gray-300 px-1 h-12 ml-2 resize-none" value={remarks} onChange={e => setRemarks(e.target.value)}></textarea>
              </div>
            </div>

            {/* Right Footer (Totals) */}
            <div className="col-span-6 grid grid-cols-2 gap-x-4 gap-y-1">
              <div className="flex items-center justify-end">
                <label className="border border-gray-300 px-1 h-6 bg-gray-50 flex items-center mr-1">NBT %</label>
                <input type="number" className="w-16 border border-gray-300 px-1 h-6 text-right" value={nbtPercent} onChange={e => setNbtPercent(parseFloat(e.target.value) || 0)} />
              </div>
              <div className="flex items-center">
                <label className="w-24 border border-gray-300 px-1 h-6 bg-white flex items-center">Invoice Total</label>
                <div className="flex-grow border border-gray-300 px-1 h-6 text-right font-bold text-lg">{invoiceTotal.toFixed(2)}</div>
              </div>

              <div className="flex items-center justify-end">
                <label className="border border-gray-300 px-1 h-6 bg-gray-50 flex items-center mr-1">VAT %</label>
                <input type="number" className="w-16 border border-gray-300 px-1 h-6 text-right" value={vatPercent} onChange={e => setVatPercent(parseFloat(e.target.value) || 0)} />
              </div>
              <div className="flex items-center">
                <label className="w-24 border border-gray-300 px-1 h-6 bg-white flex items-center">Paid Amount</label>
                <input type="number" className="flex-grow border border-gray-300 px-1 h-6 text-right text-red-600 font-bold" value={paidAmount} onChange={e => setPaidAmount(parseFloat(e.target.value) || 0)} />
              </div>

              <div className="flex items-center justify-end">
                <label className="border border-gray-300 px-1 h-6 bg-white flex items-center mr-1 w-24">NBT Amount</label>
                <div className="w-24 border border-gray-300 px-1 h-6 text-right bg-gray-50">{nbtAmount.toFixed(2)}</div>
              </div>
              <div className="flex items-center">
                <label className="w-24 border border-gray-300 px-1 h-6 bg-white flex items-center">Balance MPM</label>
                <div className="flex-grow border border-gray-300 px-1 h-6 text-right text-red-600 font-bold">{balance.toFixed(2)}</div>
              </div>

              <div className="flex items-center justify-end">
                <label className="border border-gray-300 px-1 h-6 bg-white flex items-center mr-1 w-24">VAT Amount</label>
                <div className="w-24 border border-gray-300 px-1 h-6 text-right bg-gray-50">{vatAmount.toFixed(2)}</div>
              </div>
              <div className="flex items-center">
                <label className="w-24 border border-gray-300 px-1 h-6 bg-white flex items-center">Cash</label>
                <input type="number" className="flex-grow border border-gray-300 px-1 h-6 text-right text-red-600 font-bold" value={cashTendered} onChange={e => setCashTendered(parseFloat(e.target.value) || 0)} />
              </div>

              <div className="col-span-2 flex justify-end mt-2">
                <label className="border border-gray-300 px-1 h-8 bg-gray-100 flex items-center mr-1 font-bold">BALANCE</label>
                <div className="w-40 border border-gray-300 px-1 h-8 text-right bg-yellow-200 text-red-600 font-bold text-xl flex items-center justify-end">
                  {(cashTendered - balance).toFixed(2)}
                </div>
              </div>
            </div>
          </div>
          
          {/* Status Bar */}
          <div className="mt-2 text-red-600 font-bold h-6">
            {error && <span>Error: {error}</span>}
            {success && <span className="text-green-600">{success}</span>}
          </div>
        </div>
      </div>
    </div>
  );
}
`;

fs.writeFileSync(filePath, newContent);
console.log('File updated successfully');
