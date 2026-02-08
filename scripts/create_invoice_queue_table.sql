-- Digital Invoice Queue Table
-- මෙම Table එකට පරණ System එකෙන් Invoice/Quotation යැවීමට අවශ්‍ය විස්තර INSERT කරන්න
-- Node.js Bot එක මෙය නිරීක්ෂණය කර PDF යවයි

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'tblDigitalInvoiceQueue') AND type in (N'U'))
BEGIN
    CREATE TABLE tblDigitalInvoiceQueue (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- පරණ System එක පුරවන කොටස (Required Fields)
        DocType NVARCHAR(20) NOT NULL,           -- 'INVOICE' හෝ 'QUOTATION'
        RefNumber NVARCHAR(50) NOT NULL,         -- Invoice No හෝ Quotation No
        ContactNumber NVARCHAR(50),              -- WhatsApp Number (0771234567 හෝ 94771234567)
        EmailAddress NVARCHAR(150),              -- Email Address
        
        -- Bot එක කළමනාකරණය කරන කොටස (Auto-managed by Bot)
        Status NVARCHAR(20) DEFAULT 'PENDING',   -- 'PENDING', 'PROCESSING', 'SENT', 'FAILED'
        StatusMessage NVARCHAR(MAX),             -- විස්තරාත්මක log ("Sent to WA", "Invalid Email", etc.)
        RetryCount INT DEFAULT 0,                -- කී වතාවක් උත්සාහ කළාද
        
        -- Timestamps
        CreatedDate DATETIME DEFAULT GETDATE(),  -- පරණ system එකෙන් INSERT කළ වේලාව
        ProcessedDate DATETIME NULL,             -- Bot එක process කළ වේලාව
        
        -- Index for fast lookup
        INDEX IX_Status_CreatedDate (Status, CreatedDate)
    );
    
    PRINT 'Table tblDigitalInvoiceQueue created successfully.';
END
ELSE
BEGIN
    PRINT 'Table tblDigitalInvoiceQueue already exists.';
END
GO

-- පරණ System එකෙන් භාවිතා කරන Sample INSERT Query:
-- 
-- INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
-- VALUES ('INVOICE', 'NLT00006259', '0771234567', 'customer@example.com');
--
-- INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
-- VALUES ('QUOTATION', 'QT-2026-001', '0777311770', NULL);  -- Email නැත්නම් NULL
