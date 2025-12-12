interface TyreDiagramProps {
  width: number
  aspectRatio: number
  rimDiameter: number
  sidewall: number
  patternId: string
}

export default function TyreDiagram({ 
  width, 
  aspectRatio: _aspectRatio, 
  rimDiameter, 
  sidewall,
  patternId 
}: TyreDiagramProps) {
  return (
    <svg viewBox="0 0 200 200" className="w-48 h-48">
      {/* Outer Tyre */}
      <circle cx="100" cy="100" r="85" fill="#1e293b" stroke="#475569" strokeWidth="2"/>
      {/* Tread Pattern */}
      <circle cx="100" cy="100" r="85" fill={`url(#${patternId})`} opacity="0.3"/>
      {/* Inner Rim */}
      <circle cx="100" cy="100" r="50" fill="#94a3b8" stroke="#64748b" strokeWidth="2"/>
      <circle cx="100" cy="100" r="45" fill="#e2e8f0"/>
      {/* Center Hub */}
      <circle cx="100" cy="100" r="20" fill="#475569"/>
      <circle cx="100" cy="100" r="15" fill="#1e293b"/>
      
      {/* Measurement Lines */}
      {/* Width Line */}
      <line x1="15" y1="100" x2="185" y2="100" stroke="#dc2626" strokeWidth="2" strokeDasharray="5,5"/>
      <text x="100" y="95" textAnchor="middle" fill="#dc2626" fontSize="12" fontWeight="bold">
        {width}mm
      </text>
      
      {/* Sidewall Height */}
      <line x1="185" y1="100" x2="185" y2="15" stroke="#2563eb" strokeWidth="2" strokeDasharray="5,5"/>
      <text x="185" y="55" textAnchor="middle" fill="#2563eb" fontSize="11" fontWeight="bold">
        {sidewall}mm
      </text>
      
      {/* Rim Diameter */}
      <line x1="50" y1="100" x2="150" y2="100" stroke="#16a34a" strokeWidth="2"/>
      <text x="100" y="115" textAnchor="middle" fill="#16a34a" fontSize="11" fontWeight="bold">
        {rimDiameter}"
      </text>
      
      {/* Tread Pattern Definition */}
      <defs>
        <pattern id={patternId} x="0" y="0" width="20" height="20" patternUnits="userSpaceOnUse">
          <rect x="0" y="0" width="10" height="20" fill="#000"/>
        </pattern>
      </defs>
    </svg>
  )
}
