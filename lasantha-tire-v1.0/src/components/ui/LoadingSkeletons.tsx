export function TableLoadingSkeleton({ rows = 5, columns = 4 }: { rows?: number; columns?: number }) {
  return (
    <div className="space-y-3 animate-pulse">
      {/* Header row */}
      <div className="flex items-center gap-4 pb-3 border-b border-slate-700">
        {Array.from({ length: columns }).map((_, i) => (
          <div 
            key={`header-${i}`} 
            className={`h-4 bg-slate-700 rounded ${i === 0 ? 'flex-1' : 'w-24'}`}
          />
        ))}
      </div>

      {/* Data rows */}
      {Array.from({ length: rows }).map((_, rowIndex) => (
        <div key={`row-${rowIndex}`} className="flex items-center gap-4 py-3 border-b border-slate-800/50">
          {Array.from({ length: columns }).map((_, colIndex) => (
            <div 
              key={`cell-${rowIndex}-${colIndex}`}
              className={`h-5 bg-slate-800 rounded ${
                colIndex === 0 ? 'flex-1' : colIndex === columns - 1 ? 'w-20' : 'w-24'
              }`}
              style={{ 
                opacity: 0.3 + (Math.random() * 0.4) // Vary opacity for visual interest
              }}
            />
          ))}
        </div>
      ))}
    </div>
  );
}

export function CardLoadingSkeleton({ count = 3 }: { count?: number }) {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {Array.from({ length: count }).map((_, i) => (
        <div 
          key={`card-${i}`} 
          className="bg-slate-800/50 rounded-xl p-6 animate-pulse"
        >
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-slate-700 rounded-full" />
            <div className="w-16 h-8 bg-slate-700 rounded" />
          </div>
          <div className="space-y-3">
            <div className="h-4 bg-slate-700 rounded w-3/4" />
            <div className="h-6 bg-slate-700 rounded w-1/2" />
            <div className="h-3 bg-slate-700 rounded w-2/3" />
          </div>
        </div>
      ))}
    </div>
  );
}

export function StatLoadingSkeleton() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      {Array.from({ length: 4 }).map((_, i) => (
        <div 
          key={`stat-${i}`}
          className="bg-slate-800/50 rounded-xl p-6 animate-pulse"
        >
          <div className="flex items-center justify-between mb-3">
            <div className="w-10 h-10 bg-slate-700 rounded-lg" />
            <div className="w-12 h-6 bg-slate-700 rounded" />
          </div>
          <div className="h-8 bg-slate-700 rounded w-1/2 mb-2" />
          <div className="h-4 bg-slate-700 rounded w-3/4" />
        </div>
      ))}
    </div>
  );
}

export function ChartLoadingSkeleton({ height = 300 }: { height?: number }) {
  return (
    <div 
      className="bg-slate-800/50 rounded-xl p-6 animate-pulse flex items-end justify-around gap-2"
      style={{ height: `${height}px` }}
    >
      {Array.from({ length: 8 }).map((_, i) => (
        <div 
          key={`bar-${i}`}
          className="bg-slate-700 rounded-t-lg w-full"
          style={{ 
            height: `${30 + Math.random() * 70}%`,
            opacity: 0.3 + (Math.random() * 0.4)
          }}
        />
      ))}
    </div>
  );
}

export function ListLoadingSkeleton({ rows = 5 }: { rows?: number }) {
  return (
    <div className="space-y-2">
      {Array.from({ length: rows }).map((_, i) => (
        <div 
          key={`list-${i}`}
          className="flex items-center gap-4 p-4 bg-slate-800/30 rounded-lg animate-pulse"
        >
          <div className="w-10 h-10 bg-slate-700 rounded-full flex-shrink-0" />
          <div className="flex-1 space-y-2">
            <div className="h-4 bg-slate-700 rounded w-2/3" />
            <div className="h-3 bg-slate-700 rounded w-1/2" />
          </div>
          <div className="w-20 h-6 bg-slate-700 rounded" />
        </div>
      ))}
    </div>
  );
}

export function MobileCardLoadingSkeleton({ count = 3 }: { count?: number }) {
  return (
    <div className="space-y-4">
      {Array.from({ length: count }).map((_, i) => (
        <div 
          key={`mobile-card-${i}`}
          className="bg-slate-800/50 rounded-2xl p-5 animate-pulse"
        >
          <div className="flex items-start justify-between mb-4">
            <div className="flex-1 space-y-2">
              <div className="h-5 bg-slate-700 rounded w-3/4" />
              <div className="h-4 bg-slate-700 rounded w-1/2" />
            </div>
            <div className="w-16 h-8 bg-slate-700 rounded-lg" />
          </div>
          <div className="space-y-2 mb-4">
            <div className="h-3 bg-slate-700 rounded w-full" />
            <div className="h-3 bg-slate-700 rounded w-2/3" />
          </div>
          <div className="flex gap-2">
            <div className="h-8 bg-slate-700 rounded-lg flex-1" />
            <div className="h-8 bg-slate-700 rounded-lg w-24" />
          </div>
        </div>
      ))}
    </div>
  );
}

export function DashboardLoadingSkeleton() {
  return (
    <div className="space-y-6">
      <StatLoadingSkeleton />
      
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <ChartLoadingSkeleton />
        <ChartLoadingSkeleton />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <div className="bg-slate-800/50 rounded-xl p-6">
            <div className="h-6 bg-slate-700 rounded w-1/3 mb-4 animate-pulse" />
            <TableLoadingSkeleton rows={6} columns={5} />
          </div>
        </div>
        
        <div className="bg-slate-800/50 rounded-xl p-6">
          <div className="h-6 bg-slate-700 rounded w-1/2 mb-4 animate-pulse" />
          <ListLoadingSkeleton rows={8} />
        </div>
      </div>
    </div>
  );
}
