import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';

const sqlConfig: sql.config = {
  user: process.env.SQL_USER || process.env.DB_USER || 'sa',
  password: process.env.SQL_PASSWORD || process.env.DB_PASSWORD || '',
  database: process.env.SQL_DATABASE || process.env.DB_NAME || 'TYREDB',
  server: process.env.SQL_SERVER || process.env.DB_SERVER || 'localhost',
  port: parseInt(process.env.SQL_PORT || process.env.DB_PORT || '1433', 10),
  pool: { max: 10, min: 0, idleTimeoutMillis: 30000 },
  options: {
    encrypt: false,
    trustServerCertificate: true,
    enableArithAbort: true
  }
};

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const type = searchParams.get('type') || 'summary';
    
    const pool = await sql.connect(sqlConfig);
    let result;
    
    switch(type) {
      case 'summary':
        result = await pool.request().query(`
          SELECT 
            COUNT(DISTINCT ItemId) as TotalProducts,
            COUNT(DISTINCT 
              CASE 
                WHEN ItemDis LIKE '%/%' AND LEN(ItemDis) > 10
                THEN RTRIM(LTRIM(
                  SUBSTRING(ItemDis, 
                    CHARINDEX(' ', ItemDis) + 1,
                    CASE 
                      WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                      THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                      ELSE 50
                    END
                  )
                ))
                ELSE NULL
              END
            ) as TotalBrands,
            SUM(CAST(QTY as INT)) as TotalQuantity,
            SUM(CASE WHEN CAST(QTY as INT) < 5 AND CAST(QTY as INT) > 0 THEN 1 ELSE 0 END) as LowStockItems,
            SUM(CASE WHEN CAST(QTY as INT) = 0 THEN 1 ELSE 0 END) as OutOfStockItems
          FROM View_ItemWhse
          WHERE ItemDis IS NOT NULL
        `);
        break;
        
      case 'by-brand':
        result = await pool.request().query(`
          SELECT TOP 20
            RTRIM(LTRIM(
              SUBSTRING(ItemDis, 
                CHARINDEX(' ', ItemDis) + 1,
                CASE 
                  WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                  THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                  ELSE 50
                END
              )
            )) as Brand,
            COUNT(DISTINCT ItemId) as ProductCount,
            SUM(CAST(QTY as INT)) as TotalQuantity,
            AVG(CAST(QTY as INT)) as AvgQuantity,
            SUM(CASE WHEN CAST(QTY as INT) < 5 AND CAST(QTY as INT) > 0 THEN 1 ELSE 0 END) as LowStockCount
          FROM View_ItemWhse
          WHERE ItemDis IS NOT NULL
            AND ItemDis LIKE '%/%'
            AND LEN(ItemDis) > 10
            AND ItemDis NOT LIKE '%TUBE%'
            AND ItemDis NOT LIKE '%KATTA%'
            AND ItemDis NOT LIKE '%/17%'
            AND ItemDis NOT LIKE '%/18%'
            AND ItemDis NOT LIKE '%TIMSUN%'
          GROUP BY RTRIM(LTRIM(
            SUBSTRING(ItemDis, 
              CHARINDEX(' ', ItemDis) + 1,
              CASE 
                WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                ELSE 50
              END
            )
          ))
          HAVING SUM(CAST(QTY as INT)) > 0
          ORDER BY SUM(CAST(QTY as INT)) DESC
        `);
        break;
        
      case 'low-stock':
        const threshold = parseInt(searchParams.get('threshold') || '5', 10);
        result = await pool.request()
          .input('threshold', sql.Int, threshold)
          .query(`
            SELECT TOP 100
              ItemId,
              ItemDis as Description,
              CAST(QTY as INT) as Quantity,
              RTRIM(LTRIM(
                SUBSTRING(ItemDis, 
                  CHARINDEX(' ', ItemDis) + 1,
                  CASE 
                    WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                    THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                    ELSE 50
                  END
                )
              )) as Brand
            FROM View_ItemWhse
            WHERE CAST(QTY as INT) < @threshold
              AND CAST(QTY as INT) > 0
              AND ItemDis IS NOT NULL
              AND ItemDis LIKE '%/%'
              AND ItemDis NOT LIKE '%TUBE%'
              AND ItemDis NOT LIKE '%KATTA%'
            ORDER BY CAST(QTY as INT) ASC
          `);
        break;
        
      case 'search':
        const query = searchParams.get('query') || '';
        const rim = searchParams.get('rim') || '';
        
        // Safe search pattern
        const searchPattern = `%${query}%`;
        
        result = await pool.request()
          .input('query', sql.NVarChar, searchPattern)
          .query(`
            SELECT TOP 50
              ItemId,
              ItemDis as Description,
              CAST(QTY as INT) as Quantity,
              ISNULL(SalesPrice, 0) as Price,
              RTRIM(LTRIM(
                SUBSTRING(ItemDis, 
                  CHARINDEX(' ', ItemDis) + 1,
                  CASE 
                    WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                    THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                    ELSE 50
                  END
                )
              )) as Brand
            FROM View_ItemWhse
            WHERE ItemDis LIKE @query
              AND ItemDis IS NOT NULL
            ORDER BY CAST(QTY as INT) DESC
          `);
        break;

      case 'top-stock':
        const limit = parseInt(searchParams.get('limit') || '50', 10);
        result = await pool.request()
          .input('limit', sql.Int, limit)
          .query(`
            SELECT TOP (@limit)
              ItemId,
              ItemDis as Description,
              CAST(QTY as INT) as Quantity,
              RTRIM(LTRIM(
                SUBSTRING(ItemDis, 
                  CHARINDEX(' ', ItemDis) + 1,
                  CASE 
                    WHEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) > 0
                    THEN CHARINDEX(' ', ItemDis, CHARINDEX(' ', ItemDis) + 1) - CHARINDEX(' ', ItemDis) - 1
                    ELSE 50
                  END
                )
              )) as Brand
            FROM View_ItemWhse
            WHERE CAST(QTY as INT) > 0
              AND ItemDis IS NOT NULL
              AND ItemDis LIKE '%/%'
              AND ItemDis NOT LIKE '%TUBE%'
              AND ItemDis NOT LIKE '%KATTA%'
            ORDER BY CAST(QTY as INT) DESC
          `);
        break;
        
      default:
        await pool.close();
        return NextResponse.json({ error: 'Invalid type parameter' }, { status: 400 });
    }

    await pool.close();
    
    return NextResponse.json({
      success: true,
      type,
      data: result.recordset,
      timestamp: new Date().toISOString()
    });
    
  } catch (error: any) {
    console.error('[ERP/Inventory API] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to fetch inventory data',
      timestamp: new Date().toISOString()
    }, { status: 500 });
  }
}
