import WinToolbar from '../_components/WinToolbar';
import PrintButton from './PrintButton';

export default function PrintTestPage() {
  return (
    <div className="min-h-screen bg-gray-100 p-3 text-gray-900">
      <div className="mx-auto max-w-4xl border bg-white">
        <div className="border-b bg-gray-50 px-3 py-2 text-sm font-semibold">Crystal Print Bridge Test</div>
        <WinToolbar />

        <div className="p-3 text-sm">
          <div className="text-xs text-gray-600">
            This will call the desktop (Electron) main-process IPC and attempt to run `electron/bridge/CrystalPrintBridge.exe`.
          </div>
          <div className="mt-3">
            <PrintButton />
          </div>
        </div>
      </div>
    </div>
  );
}
