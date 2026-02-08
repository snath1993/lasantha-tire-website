import { NextRequest, NextResponse } from 'next/server';
import fs from 'fs';
import path from 'path';

const WATCHED_ITEMS_CONFIG_PATH = path.join(process.cwd(), '..', 'watched-item-config.json');

// Read watched items configuration
export async function GET() {
  try {
    const configData = fs.readFileSync(WATCHED_ITEMS_CONFIG_PATH, 'utf8');
    const config = JSON.parse(configData);
    
    return NextResponse.json(config);
  } catch (error) {
    console.error('Failed to read watched items config:', error);
    return NextResponse.json(
      { error: 'Failed to load watched items' },
      { status: 500 }
    );
  }
}

// Update watched items configuration
export async function POST(request: NextRequest) {
  try {
    const { patterns } = await request.json();

    if (!Array.isArray(patterns)) {
      return NextResponse.json(
        { error: 'Invalid patterns array' },
        { status: 400 }
      );
    }

    // Read current configuration
    const configData = fs.readFileSync(WATCHED_ITEMS_CONFIG_PATH, 'utf8');
    const config = JSON.parse(configData);

    // Update patterns
    config.patterns = patterns;
    
    // Also update legacy 'pattern' field for backward compatibility
    if (patterns.length > 0) {
      config.pattern = patterns[0];
    }

    // Write back to file
    fs.writeFileSync(
      WATCHED_ITEMS_CONFIG_PATH,
      JSON.stringify(config, null, 2),
      'utf8'
    );

    console.log(`✅ Updated watched items: ${patterns.length} patterns`);

    return NextResponse.json({ success: true, patterns });
  } catch (error) {
    console.error('Failed to update watched items:', error);
    return NextResponse.json(
      { error: 'Failed to save watched items' },
      { status: 500 }
    );
  }
}
