-- Query to get structure and sample data from [View_Sales report whatsapp]

-- 1. Get Column Names and Types
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'View_Sales report whatsapp'
ORDER BY ORDINAL_POSITION;

-- 2. Get Sample Data (Top 5 rows)
SELECT TOP 5 * 
FROM [View_Sales report whatsapp]
ORDER BY InvoiceDate DESC;

-- 3. Get Row Count
SELECT COUNT(*) AS TotalRows
FROM [View_Sales report whatsapp];
