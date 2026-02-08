-- =============================================
-- Digital Invoice Queue Table
-- =============================================
-- මෙම Table එක පරණ System එකෙන් Invoice/Quotation යැවීමට අවශ්‍ය විස්තර Store කරයි
-- Node.js Bot එක මෙය නිරීක්ෂණය කර PDF ස්වයංක්‍රීයව WhatsApp/Email හරහා යවයි
-- =============================================

USE LasanthaTire;
GO

-- පවතින Table එක drop කරන්න (නැවත create කිරීමට - පළමු වතාවට මෙය අවශ්‍ය නැහැ)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblDigitalInvoiceQueue]') AND type in (N'U'))
BEGIN
    PRINT 'Dropping existing tblDigitalInvoiceQueue table...';
    DROP TABLE [dbo].[tblDigitalInvoiceQueue];
END
GO

-- Table එක නිර්මාණය කිරීම
CREATE TABLE [dbo].[tblDigitalInvoiceQueue]
(
    -- Primary Key
    ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    
    -- ============================================
    -- පරණ System එක පුරවන Fields (INSERT කරන කොටස)
    -- ============================================
    DocType NVARCHAR(20) NOT NULL,              -- 'INVOICE' හෝ 'QUOTATION'
    RefNumber NVARCHAR(50) NOT NULL,            -- Invoice No (උදා: NLT00006259) හෝ Quotation No
    ContactNumber NVARCHAR(50) NULL,            -- WhatsApp Phone Number (0771234567)
    EmailAddress NVARCHAR(150) NULL,            -- Email Address (customer@example.com)
    
    -- ============================================
    -- Bot එක කළමනාකරණය කරන Fields (Auto-managed)
    -- ============================================
    Status NVARCHAR(20) NOT NULL DEFAULT 'PENDING',  -- Status: PENDING, PROCESSING, SENT, FAILED
    StatusMessage NVARCHAR(MAX) NULL,           -- විස්තරාත්මක Log Message
    RetryCount INT NOT NULL DEFAULT 0,          -- කී පාරක් Retry කළාද
    
    -- ============================================
    -- Timestamps
    -- ============================================
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),    -- පරණ system එකෙන් INSERT කළ වේලාව
    ProcessedDate DATETIME NULL,                        -- Bot එක process කළ වේලාව
    
    -- Constraints
    CONSTRAINT CK_DocType CHECK (DocType IN ('INVOICE', 'QUOTATION')),
    CONSTRAINT CK_Status CHECK (Status IN ('PENDING', 'PROCESSING', 'SENT', 'FAILED'))
);
GO

-- Performance සඳහා Index එකක්
CREATE NONCLUSTERED INDEX IX_Status_CreatedDate 
ON [dbo].[tblDigitalInvoiceQueue] (Status, CreatedDate ASC);
GO

-- RefNumber සඳහා Index (පරණ system එකෙන් status check කරන්න පහසුවෙන්)
CREATE NONCLUSTERED INDEX IX_RefNumber 
ON [dbo].[tblDigitalInvoiceQueue] (RefNumber);
GO

PRINT '✅ tblDigitalInvoiceQueue table created successfully!';
GO

-- =============================================
-- පරණ System එකෙන් භාවිතා කරන Sample INSERT Queries
-- =============================================

-- උදාහරණය 1: Invoice එකක් Queue එකට දැමීම (Phone සහ Email දෙකම තිබේ)
/*
INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
VALUES ('INVOICE', 'NLT00006259', '0771234567', 'customer@gmail.com');
*/

-- උදාහරණය 2: Quotation එකක් Queue එකට දැමීම (Phone එකක් පමණයි)
/*
INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
VALUES ('QUOTATION', 'QT-2026-001', '0777311770', NULL);
*/

-- උදාහරණය 3: Email එකක් පමණයි (WhatsApp number නැහැ)
/*
INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
VALUES ('INVOICE', 'NLT00006260', NULL, 'business@example.com');
*/

-- =============================================
-- Status පරීක්ෂා කිරීම (පරණ System එකෙන්)
-- =============================================

-- විශේෂිත Invoice එකක Status බැලීම:
/*
SELECT ID, DocType, RefNumber, Status, StatusMessage, CreatedDate, ProcessedDate
FROM tblDigitalInvoiceQueue
WHERE RefNumber = 'NLT00006259';
*/

-- අද දවසේ යැවූ සියලුම ඒවා බැලීම:
/*
SELECT *
FROM tblDigitalInvoiceQueue
WHERE CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY CreatedDate DESC;
*/

-- අසාර්ථක වූ ඒවා බැලීම:
/*
SELECT ID, RefNumber, Status, StatusMessage
FROM tblDigitalInvoiceQueue
WHERE Status = 'FAILED'
ORDER BY CreatedDate DESC;
*/

PRINT '✅ Setup Complete! Table is ready for use.';
GO
