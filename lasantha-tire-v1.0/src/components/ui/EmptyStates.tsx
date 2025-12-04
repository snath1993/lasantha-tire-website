import { Users, Building2, Wallet, Receipt, FileText, TrendingUp, AlertCircle, RefreshCw, Database } from 'lucide-react';

interface EmptyStateProps {
  icon?: React.ReactNode;
  title: string;
  description: string;
  actionLabel?: string;
  onAction?: () => void;
  actionLoading?: boolean;
  variant?: 'default' | 'error' | 'info';
}

export function EmptyState({
  icon,
  title,
  description,
  actionLabel,
  onAction,
  actionLoading = false,
  variant = 'default'
}: EmptyStateProps) {
  const variantStyles = {
    default: 'bg-slate-800/30 border-slate-700/50',
    error: 'bg-red-900/20 border-red-500/30',
    info: 'bg-blue-900/20 border-blue-500/30'
  };

  const iconColors = {
    default: 'text-slate-600',
    error: 'text-red-500/50',
    info: 'text-blue-500/50'
  };

  return (
    <div className={`flex flex-col items-center justify-center py-20 px-6 text-center space-y-4 rounded-xl border ${variantStyles[variant]}`}>
      {icon && (
        <div className={`w-20 h-20 rounded-full flex items-center justify-center ${iconColors[variant]}`}>
          {icon}
        </div>
      )}
      <div className="space-y-2 max-w-md">
        <h3 className="text-lg font-semibold text-slate-300">{title}</h3>
        <p className="text-sm text-slate-500">{description}</p>
      </div>
      {actionLabel && onAction && (
        <button
          onClick={onAction}
          disabled={actionLoading}
          className="inline-flex items-center gap-2 px-6 py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-700 text-white text-sm font-semibold rounded-lg shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <RefreshCw className={`w-4 h-4 ${actionLoading ? 'animate-spin' : ''}`} />
          {actionLabel}
        </button>
      )}
    </div>
  );
}

export function NoCustomersState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<Users className="w-12 h-12" />}
      title="No Customers Found"
      description="There are no customer records available. Make sure you're connected to Peachtree and try reloading the data."
      actionLabel="Reload from Peachtree"
      onAction={onReload}
      actionLoading={loading}
    />
  );
}

export function NoVendorsState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<Building2 className="w-12 h-12" />}
      title="No Vendors Found"
      description="There are no vendor records available. Make sure you're connected to Peachtree and try reloading the data."
      actionLabel="Reload from Peachtree"
      onAction={onReload}
      actionLoading={loading}
    />
  );
}

export function NoAccountsState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<Wallet className="w-12 h-12" />}
      title="No Accounts Found"
      description="No general ledger accounts are available. Connect to Peachtree to access your chart of accounts."
      actionLabel="Reload from Peachtree"
      onAction={onReload}
      actionLoading={loading}
    />
  );
}

export function NoTransactionsState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<Receipt className="w-12 h-12" />}
      title="No Transactions Found"
      description="No transaction records are available for the selected filters. Try adjusting your filters or reload from Peachtree."
      actionLabel="Reload from Peachtree"
      onAction={onReload}
      actionLoading={loading}
    />
  );
}

export function NoReportsState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<FileText className="w-12 h-12" />}
      title="No Reports Available"
      description="Select a report type from the menu above to generate financial reports and analytics."
      variant="info"
    />
  );
}

export function NoDataState({ onReload, loading }: { onReload?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<Database className="w-12 h-12" />}
      title="No Data Available"
      description="Unable to load data. Check your Peachtree connection and try again."
      actionLabel="Retry Connection"
      onAction={onReload}
      actionLoading={loading}
      variant="error"
    />
  );
}

export function BridgeDisconnectedState({ onConnect, loading }: { onConnect?: () => void; loading?: boolean }) {
  return (
    <EmptyState
      icon={<AlertCircle className="w-12 h-12" />}
      title="Peachtree Bridge Disconnected"
      description="The connection to Peachtree is not active. Click below to start the bridge and connect to your ERP data."
      actionLabel="Connect Now"
      onAction={onConnect}
      actionLoading={loading}
      variant="error"
    />
  );
}

export function SearchNoResultsState({ searchTerm, onClear }: { searchTerm: string; onClear?: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center py-16 px-6 text-center space-y-4">
      <div className="w-16 h-16 rounded-full bg-slate-800/50 flex items-center justify-center">
        <AlertCircle className="w-8 h-8 text-slate-600" />
      </div>
      <div className="space-y-2 max-w-md">
        <h3 className="text-lg font-semibold text-slate-300">No Results Found</h3>
        <p className="text-sm text-slate-500">
          No records match <span className="font-mono text-blue-400">"{searchTerm}"</span>. Try different search terms or clear the filter.
        </p>
      </div>
      {onClear && (
        <button
          onClick={onClear}
          className="inline-flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-slate-300 text-sm font-medium rounded-lg transition-all"
        >
          Clear Search
        </button>
      )}
    </div>
  );
}

export function FilterNoResultsState({ onClear }: { onClear?: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center py-16 px-6 text-center space-y-4">
      <div className="w-16 h-16 rounded-full bg-slate-800/50 flex items-center justify-center">
        <TrendingUp className="w-8 h-8 text-slate-600" />
      </div>
      <div className="space-y-2 max-w-md">
        <h3 className="text-lg font-semibold text-slate-300">No Matching Records</h3>
        <p className="text-sm text-slate-500">
          No records match your current filter criteria. Try adjusting the filters or reset them to view all records.
        </p>
      </div>
      {onClear && (
        <button
          onClick={onClear}
          className="inline-flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-slate-300 text-sm font-medium rounded-lg transition-all"
        >
          Clear Filters
        </button>
      )}
    </div>
  );
}
