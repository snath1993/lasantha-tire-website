import Link from "next/link";

export default function Home() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto max-w-4xl p-6">
        <div className="rounded border">
          <div className="border-b px-4 py-3">
            <div className="text-sm font-semibold">Invoice V2 (Desktop)</div>
            <div className="text-xs opacity-75">WinForms-style remake workspace</div>
          </div>

          <div className="p-4">
            <div className="grid grid-cols-1 gap-3 text-sm sm:grid-cols-2">
              <Link className="rounded border px-3 py-2 hover:bg-black/5" href="/invoice/new">
                Invoice Entry (New)
              </Link>
              <Link className="rounded border px-3 py-2 hover:bg-black/5" href="/invoice/list">
                Invoice List
              </Link>
              <Link className="rounded border px-3 py-2 hover:bg-black/5" href="/invoice/print-test">
                Print Test (Crystal bridge)
              </Link>
            </div>

            <div className="mt-4 text-xs opacity-75">
              Note: UI + flows will be rebuilt page-by-page to match the legacy system.
            </div>
          </div>
        </div>
      </div>
    </div>
  );
