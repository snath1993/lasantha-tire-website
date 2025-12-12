# SQL Injection Prevention Guide

## Overview

This document provides comprehensive guidelines for preventing SQL injection vulnerabilities in the Lasantha Tire WhatsApp Bot system.

**Current Status:** ⚠️ 42 potential SQL query concatenations detected

## What is SQL Injection?

SQL injection is a code injection technique that exploits security vulnerabilities in database layer of an application. Attackers can inject malicious SQL statements into query fields to:
- Access unauthorized data
- Modify or delete data
- Execute administrative operations
- Bypass authentication
- In some cases, issue commands to the operating system

## Risk Assessment

### High-Risk Patterns Found

The following patterns were detected in the codebase:

```javascript
// ❌ UNSAFE: String concatenation in SQL queries
const query = `SELECT * FROM Items WHERE Size = '${tyreSize}'`;
await request.query(query);

// ❌ UNSAFE: Template literals with user input
await request.query(`
    SELECT * FROM Customers 
    WHERE Phone = '${customerPhone}'
`);

// ❌ UNSAFE: Dynamic table names without validation
const table = req.params.tableName;
await request.query(`SELECT * FROM ${table}`);
```

### Attack Examples

**Example 1: Data Extraction**
```javascript
// User input: tyreSize = "195/65R15' OR '1'='1"
const query = `SELECT * FROM Items WHERE Size = '${tyreSize}'`;
// Resulting query: SELECT * FROM Items WHERE Size = '195/65R15' OR '1'='1'
// Result: Returns ALL items (authentication bypass)
```

**Example 2: Data Deletion**
```javascript
// User input: customerPhone = "0771234567'; DROP TABLE Customers; --"
const query = `DELETE FROM Cart WHERE Phone = '${customerPhone}'`;
// Resulting query: DELETE FROM Cart WHERE Phone = '0771234567'; DROP TABLE Customers; --'
// Result: Cart deleted AND Customers table dropped!
```

**Example 3: Unauthorized Access**
```javascript
// User input: quotationNumber = "QT-001' UNION SELECT Password FROM Users --"
const query = `SELECT * FROM Quotations WHERE Number = '${quotationNumber}'`;
// Result: Exposes password hashes from Users table
```

## Prevention Strategies

### 1. Parameterized Queries (Primary Defense)

**✅ SAFE: Using mssql prepared statements**

```javascript
const sql = require('mssql');

// Safe pattern for single parameter
async function getTyreBySize(tyreSize) {
    try {
        const pool = await sql.connect(sqlConfig);
        const request = pool.request();
        
        // Use input() to parameterize the query
        request.input('size', sql.VarChar(20), tyreSize);
        
        const result = await request.query(`
            SELECT * FROM Items 
            WHERE Size = @size
        `);
        
        await pool.close();
        return result.recordset;
    } catch (error) {
        console.error('Database error:', error);
        throw error;
    }
}

// Safe pattern for multiple parameters
async function getQuotation(quotationNumber, customerPhone) {
    const pool = await sql.connect(sqlConfig);
    const request = pool.request();
    
    request.input('quotationNumber', sql.VarChar(50), quotationNumber);
    request.input('customerPhone', sql.VarChar(20), customerPhone);
    
    const result = await request.query(`
        SELECT * FROM Quotations 
        WHERE QuotationNumber = @quotationNumber 
          AND CustomerPhone = @customerPhone
    `);
    
    await pool.close();
    return result.recordset[0];
}

// Safe pattern for LIKE queries
async function searchItems(searchTerm) {
    const pool = await sql.connect(sqlConfig);
    const request = pool.request();
    
    // Escape special characters in LIKE pattern
    const safeTerm = searchTerm.replace(/[%_\[\]]/g, '[$&]');
    request.input('searchTerm', sql.VarChar(100), `%${safeTerm}%`);
    
    const result = await request.query(`
        SELECT * FROM Items 
        WHERE Description LIKE @searchTerm
    `);
    
    await pool.close();
    return result.recordset;
}

// Safe pattern for IN clause
async function getMultipleItems(sizeArray) {
    const pool = await sql.connect(sqlConfig);
    const request = pool.request();
    
    // Create table-valued parameter
    const table = new sql.Table();
    table.columns.add('Size', sql.VarChar(20));
    
    sizeArray.forEach(size => {
        table.rows.add(size);
    });
    
    request.input('sizes', table);
    
    const result = await request.query(`
        SELECT * FROM Items 
        WHERE Size IN (SELECT Size FROM @sizes)
    `);
    
    await pool.close();
    return result.recordset;
}
```

### 2. Input Validation (Secondary Defense)

**Validation Middleware:**

```javascript
const { body, param, validationResult } = require('express-validator');

// Tire size validator
const validateTyreSize = body('tyreSize')
    .matches(/^\d{3}\/\d{2}R\d{2}$/)
    .withMessage('Invalid tire size format')
    .customSanitizer(value => value.toUpperCase());

// Phone number validator
const validatePhone = body('customerPhone')
    .matches(/^0[0-9]{9}$/)
    .withMessage('Invalid phone number')
    .customSanitizer(value => value.replace(/[^0-9]/g, ''));

// Quotation number validator
const validateQuotationNumber = param('quotationNumber')
    .matches(/^QT-\d{8}-\d{4}$/)
    .withMessage('Invalid quotation number format');

// Email validator
const validateEmail = body('email')
    .isEmail()
    .normalizeEmail()
    .withMessage('Invalid email address');

// Vehicle number validator
const validateVehicleNumber = body('vehicleNumber')
    .optional()
    .matches(/^[A-Z]{2,3}-\d{4}$/)
    .withMessage('Invalid vehicle number format')
    .customSanitizer(value => value.toUpperCase());

// Integer validator
const validateQuantity = body('quantity')
    .isInt({ min: 1, max: 10 })
    .withMessage('Quantity must be between 1 and 10')
    .toInt();

// Date validator
const validateDate = body('date')
    .isISO8601()
    .withMessage('Invalid date format')
    .toDate();

// Error handler
function handleValidationErrors(req, res, next) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({
            success: false,
            errors: errors.array().map(err => ({
                field: err.param,
                message: err.msg
            }))
        });
    }
    next();
}

// Usage in routes
app.post('/api/quotations',
    validateTyreSize,
    validatePhone,
    validateQuantity,
    handleValidationErrors,
    async (req, res) => {
        // Input is now validated and sanitized
        const { tyreSize, customerPhone, quantity } = req.body;
        // ... safe to use in parameterized query
    }
);
```

### 3. Whitelist Validation for Dynamic Queries

**For dynamic table/column names:**

```javascript
// ❌ UNSAFE: User-controlled table name
async function getDataFromTable(tableName) {
    const query = `SELECT * FROM ${tableName}`;
    return await request.query(query);
}

// ✅ SAFE: Whitelisted table names
const ALLOWED_TABLES = {
    'items': 'Items',
    'quotations': 'Quotations',
    'customers': 'Customers',
    'invoices': 'Invoices'
};

async function getDataFromTable(tableKey) {
    // Validate against whitelist
    const tableName = ALLOWED_TABLES[tableKey.toLowerCase()];
    
    if (!tableName) {
        throw new Error('Invalid table name');
    }
    
    // Now safe to use
    const query = `SELECT * FROM ${tableName}`;
    return await request.query(query);
}

// ✅ SAFE: Whitelisted column names
const ALLOWED_COLUMNS = {
    'size': 'Size',
    'brand': 'Brand',
    'pattern': 'Pattern',
    'price': 'Price'
};

async function sortItems(sortBy, sortOrder) {
    const column = ALLOWED_COLUMNS[sortBy.toLowerCase()];
    
    if (!column) {
        throw new Error('Invalid sort column');
    }
    
    // Validate sort order
    const order = sortOrder.toUpperCase() === 'DESC' ? 'DESC' : 'ASC';
    
    const pool = await sql.connect(sqlConfig);
    
    // Use string interpolation only for whitelisted values
    const query = `SELECT * FROM Items ORDER BY ${column} ${order}`;
    const result = await pool.request().query(query);
    
    await pool.close();
    return result.recordset;
}
```

### 4. Stored Procedures (Best Practice)

**Create stored procedures for complex queries:**

```sql
-- Create stored procedure
CREATE PROCEDURE sp_CreateQuotation
    @QuotationNumber VARCHAR(50),
    @TyreSize VARCHAR(20),
    @CustomerPhone VARCHAR(20),
    @CustomerName NVARCHAR(100),
    @TotalAmount DECIMAL(10,2),
    @Items NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO Quotations (
            QuotationNumber,
            TyreSize,
            CustomerPhone,
            CustomerName,
            TotalAmount,
            Items,
            CreatedAt
        )
        VALUES (
            @QuotationNumber,
            @TyreSize,
            @CustomerPhone,
            @CustomerName,
            @TotalAmount,
            @Items,
            GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        SELECT SCOPE_IDENTITY() AS QuotationId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
```

```javascript
// Call stored procedure from Node.js
async function createQuotation(quotationData) {
    const pool = await sql.connect(sqlConfig);
    const request = pool.request();
    
    // All parameters are automatically escaped
    request.input('QuotationNumber', sql.VarChar(50), quotationData.quotationNumber);
    request.input('TyreSize', sql.VarChar(20), quotationData.tyreSize);
    request.input('CustomerPhone', sql.VarChar(20), quotationData.customerPhone);
    request.input('CustomerName', sql.NVarChar(100), quotationData.customerName);
    request.input('TotalAmount', sql.Decimal(10, 2), quotationData.totalAmount);
    request.input('Items', sql.NVarChar(sql.MAX), JSON.stringify(quotationData.items));
    
    const result = await request.execute('sp_CreateQuotation');
    
    await pool.close();
    return result.recordset[0];
}
```

## Audit Checklist

Use this checklist to review all SQL queries in the codebase:

### Phase 1: Identify Vulnerable Queries

- [ ] Search for string concatenation in SQL queries: `+ variable +`, `${variable}`
- [ ] Search for `.query(` calls
- [ ] Search for `.execute(` calls
- [ ] List all files containing SQL queries

### Phase 2: Review Each Query

For each query, check:

- [ ] Uses parameterized queries (`.input()` method)
- [ ] No string concatenation with user input
- [ ] Dynamic table/column names are whitelisted
- [ ] LIKE queries properly escape special characters
- [ ] IN clauses use table-valued parameters or arrays
- [ ] ORDER BY uses whitelisted columns
- [ ] WHERE clauses use parameters

### Phase 3: Test for Vulnerabilities

Test each endpoint with:

```javascript
// Test inputs
const testInputs = [
    "'; DROP TABLE Items; --",
    "' OR '1'='1",
    "' UNION SELECT * FROM Users --",
    "'; DELETE FROM Customers WHERE '1'='1",
    "admin'--",
    "' OR 1=1--",
    "1'; EXEC sp_executesql N'SELECT @@version'--"
];

// Automated testing
testInputs.forEach(input => {
    test(`SQL Injection test: ${input}`, async () => {
        const response = await request(app)
            .post('/api/quotations')
            .send({ tyreSize: input })
            .expect(400); // Should fail validation
    });
});
```

## Code Review Guidelines

### Files to Review

Based on the detection of 42 potential issues, prioritize review of:

1. **index.js** (4,378 lines) - Main bot file
2. **jobs/** directory - Background job files
3. **utils/** directory - Utility functions
4. API route handlers
5. Database query functions

### Review Process

For each file:

1. **Search Pattern:** Look for these patterns
```bash
# Search for potential SQL injection
grep -n "query.*\+" index.js
grep -n "query.*\${" index.js
grep -n "execute.*\+" index.js
```

2. **Document Findings:** Create a spreadsheet with:
   - File name
   - Line number
   - Code snippet
   - Risk level (High/Medium/Low)
   - Fix required (Yes/No)
   - Status (Fixed/Pending)

3. **Fix Priority:**
   - **Critical:** Authentication/Authorization queries
   - **High:** Data modification queries (INSERT, UPDATE, DELETE)
   - **Medium:** Data retrieval queries (SELECT)
   - **Low:** Read-only views, reports

## Safe Query Patterns Reference

### SELECT Queries

```javascript
// ✅ SAFE: Basic SELECT
async function getItem(itemId) {
    const request = pool.request();
    request.input('itemId', sql.Int, itemId);
    return await request.query('SELECT * FROM Items WHERE Id = @itemId');
}

// ✅ SAFE: Multiple conditions
async function searchItems(size, brand, minPrice, maxPrice) {
    const request = pool.request();
    request.input('size', sql.VarChar(20), size);
    request.input('brand', sql.VarChar(50), brand);
    request.input('minPrice', sql.Decimal(10, 2), minPrice);
    request.input('maxPrice', sql.Decimal(10, 2), maxPrice);
    
    return await request.query(`
        SELECT * FROM Items 
        WHERE Size = @size 
          AND Brand = @brand
          AND Price BETWEEN @minPrice AND @maxPrice
    `);
}

// ✅ SAFE: LIKE query with escaping
async function searchByDescription(searchTerm) {
    const safeTerm = searchTerm.replace(/[%_\[\]]/g, '[$&]');
    const request = pool.request();
    request.input('searchTerm', sql.VarChar(100), `%${safeTerm}%`);
    
    return await request.query(`
        SELECT * FROM Items 
        WHERE Description LIKE @searchTerm
    `);
}
```

### INSERT Queries

```javascript
// ✅ SAFE: Single INSERT
async function createQuotation(data) {
    const request = pool.request();
    request.input('quotationNumber', sql.VarChar(50), data.quotationNumber);
    request.input('customerPhone', sql.VarChar(20), data.customerPhone);
    request.input('totalAmount', sql.Decimal(10, 2), data.totalAmount);
    
    return await request.query(`
        INSERT INTO Quotations (QuotationNumber, CustomerPhone, TotalAmount, CreatedAt)
        VALUES (@quotationNumber, @customerPhone, @totalAmount, GETDATE())
    `);
}

// ✅ SAFE: Bulk INSERT using table-valued parameter
async function createMultipleItems(items) {
    const table = new sql.Table();
    table.columns.add('Size', sql.VarChar(20));
    table.columns.add('Brand', sql.VarChar(50));
    table.columns.add('Price', sql.Decimal(10, 2));
    
    items.forEach(item => {
        table.rows.add(item.size, item.brand, item.price);
    });
    
    const request = pool.request();
    request.input('items', table);
    
    return await request.query(`
        INSERT INTO Items (Size, Brand, Price)
        SELECT Size, Brand, Price FROM @items
    `);
}
```

### UPDATE Queries

```javascript
// ✅ SAFE: UPDATE with WHERE clause
async function updateQuotationStatus(quotationNumber, status) {
    const request = pool.request();
    request.input('quotationNumber', sql.VarChar(50), quotationNumber);
    request.input('status', sql.VarChar(20), status);
    
    return await request.query(`
        UPDATE Quotations 
        SET Status = @status, UpdatedAt = GETDATE()
        WHERE QuotationNumber = @quotationNumber
    `);
}

// ✅ SAFE: UPDATE with multiple conditions
async function markQuotationsExpired() {
    return await pool.request().query(`
        UPDATE Quotations
        SET IsExpired = 1, Status = 'Expired'
        WHERE IsExpired = 0 
          AND IsBooked = 0
          AND ExpiryDate < GETDATE()
    `);
}
```

### DELETE Queries

```javascript
// ✅ SAFE: DELETE with WHERE clause
async function deleteQuotation(quotationNumber) {
    const request = pool.request();
    request.input('quotationNumber', sql.VarChar(50), quotationNumber);
    
    return await request.query(`
        DELETE FROM Quotations 
        WHERE QuotationNumber = @quotationNumber
    `);
}

// ✅ SAFE: DELETE with multiple conditions
async function cleanupOldData(daysOld) {
    const request = pool.request();
    request.input('daysOld', sql.Int, daysOld);
    
    return await request.query(`
        DELETE FROM TempData
        WHERE CreatedAt < DATEADD(DAY, -@daysOld, GETDATE())
    `);
}
```

## Tools for Detection

### 1. Static Analysis

```bash
# Install SQLMap for testing
pip install sqlmap

# Test an endpoint
sqlmap -u "http://localhost:8585/api/items?size=195/65R15" --batch --risk=3 --level=5

# Install SonarQube for code analysis
docker run -d -p 9000:9000 sonarqube
# Analyze code
sonar-scanner -Dsonar.projectKey=lasantha-tire -Dsonar.sources=.
```

### 2. Runtime Monitoring

```javascript
// Add SQL query logger
const originalQuery = sql.Request.prototype.query;

sql.Request.prototype.query = function(...args) {
    const query = args[0];
    
    // Log all queries
    console.log('[SQL]', query);
    
    // Check for dangerous patterns
    const dangerousPatterns = [
        /\$\{.*\}/,  // Template literals
        /\+.*\+/,    // String concatenation
        /';/,        // SQL injection attempt
        /DROP\s+TABLE/i,
        /DELETE\s+FROM.*WHERE.*1=1/i
    ];
    
    dangerousPatterns.forEach(pattern => {
        if (pattern.test(query)) {
            console.error('[SQL INJECTION WARNING]', query);
            // Send alert
        }
    });
    
    return originalQuery.apply(this, args);
};
```

## Emergency Response

If SQL injection is detected:

### 1. Immediate Actions
- [ ] Disable affected endpoint
- [ ] Review database logs for suspicious activity
- [ ] Check for unauthorized data access
- [ ] Backup database immediately

### 2. Investigation
- [ ] Identify injection point
- [ ] Trace attack origin (IP, user)
- [ ] Document compromised data
- [ ] Review similar code patterns

### 3. Remediation
- [ ] Fix vulnerable code with parameterized queries
- [ ] Deploy security patch
- [ ] Reset passwords if user table accessed
- [ ] Notify affected users if required

### 4. Prevention
- [ ] Add automated SQL injection testing
- [ ] Implement WAF (Web Application Firewall)
- [ ] Enable database audit logging
- [ ] Train developers on secure coding

## Success Criteria

After implementing fixes:

- [ ] **0 SQL queries use string concatenation with user input**
- [ ] **100% of queries use parameterized statements**
- [ ] **All dynamic table/column names are whitelisted**
- [ ] **Input validation on all endpoints**
- [ ] **Automated SQL injection tests pass**
- [ ] **No warnings from static analysis tools**
- [ ] **Penetration testing shows no SQL injection vulnerabilities**

---

**Document Version:** 1.0  
**Last Updated:** December 12, 2025  
**Next Review:** After code fixes are implemented  
**Related Documents:**
- PRODUCTION_READINESS_FINAL_REPORT.md
- IMPROVEMENT_RECOMMENDATIONS.md
