import Link from 'next/link';

export default function WinToolbar() {
  return (
    <div className="flex items-center gap-2 border-b bg-gray-100 px-2 py-1 text-sm">
      <Link className="rounded border bg-white px-2 py-1 hover:bg-gray-50" href="/invoice/new">
        New
      </Link>
      <Link className="rounded border bg-white px-2 py-1 hover:bg-gray-50" href="/invoice/list">
        List
      </Link>
      <span className="mx-1 h-5 w-px bg-gray-300" />
      <Link className="rounded border bg-white px-2 py-1 hover:bg-gray-50" href="/invoice/print-test">
        Print
      </Link>
    </div>
  );
}
