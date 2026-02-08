import WinToolbar from '../_components/WinToolbar';

export default function InvoiceNewPage() {
  return (
    <div className="min-h-screen bg-gray-100 p-3 text-gray-900">
      <div className="mx-auto max-w-6xl border bg-white">
        <div className="border-b bg-gray-50 px-3 py-2 text-sm font-semibold">Invoice Entry</div>
        <WinToolbar />

        <div className="p-3 text-sm">
          <div className="grid grid-cols-4 gap-2">
            <label className="col-span-1 text-xs text-gray-600">Invoice No</label>
            <input className="col-span-1 border px-2 py-1" placeholder="INV00000000" />
            <label className="col-span-1 text-xs text-gray-600">Date</label>
            <input className="col-span-1 border px-2 py-1" placeholder="MM/DD/YYYY" />

            <label className="col-span-1 text-xs text-gray-600">Customer</label>
            <input className="col-span-3 border px-2 py-1" placeholder="CASH / Customer Name" />

            <label className="col-span-1 text-xs text-gray-600">Vehicle No</label>
            <input className="col-span-1 border px-2 py-1" />
            <label className="col-span-1 text-xs text-gray-600">Terms</label>
            <input className="col-span-1 border px-2 py-1" />
          </div>

          <div className="mt-3 border">
            <div className="border-b bg-gray-50 px-2 py-1 text-xs font-semibold">Items</div>
            <div className="p-2 text-xs text-gray-600">
              Placeholder: legacy-style grid will be rebuilt to match frmInvoices (UltraGrid/DataGridView).
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
