'use client';

declare global {
  interface Window {
    invoiceV2?: {
      crystalPrintInvoice?: (payload: any) => Promise<any>;
    };
  }
}

export default function PrintButton() {
  async function onClick() {
    const res = await window.invoiceV2?.crystalPrintInvoice?.({
      invoiceNo: 'INV00063586',
      reportPath: '',
    });
    alert(JSON.stringify(res ?? { ok: false, error: 'IPC not available' }, null, 2));
  }

  return (
    <button className="rounded border bg-white px-3 py-2 text-sm hover:bg-gray-50" onClick={onClick}>
      Crystal Print Test
    </button>
  );
}
