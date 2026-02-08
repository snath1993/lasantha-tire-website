import WinToolbar from '../_components/WinToolbar';

export default function InvoiceListPage() {
  return (
    <div className="min-h-screen bg-gray-100 p-3 text-gray-900">
      <div className="mx-auto max-w-6xl border bg-white">
        <div className="border-b bg-gray-50 px-3 py-2 text-sm font-semibold">Invoice List</div>
        <WinToolbar />

        <div className="p-3 text-sm">
          <div className="grid grid-cols-6 gap-2">
            <label className="col-span-1 text-xs text-gray-600">From</label>
            <input className="col-span-2 border px-2 py-1" placeholder="YYYY-MM-DD" />
            <label className="col-span-1 text-xs text-gray-600">To</label>
            <input className="col-span-2 border px-2 py-1" placeholder="YYYY-MM-DD" />
          </div>

          <div className="mt-3 border">
            <div className="border-b bg-gray-50 px-2 py-1 text-xs font-semibold">Results</div>
            <div className="p-2 text-xs text-gray-600">
              Placeholder: will match frmInvoiceList / frmInvoiceARList behavior.
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
