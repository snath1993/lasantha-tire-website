# Legacy System Analysis Report

## 1. System Overview
- **Technology Stack:** C# .NET Framework (Windows Forms)
- **Database:** SQL Server (`LasanthaTire` DB)
- **Reporting:** Crystal Reports
- **External Integration:** Peachtree Accounting (Sage 50) via COM Interop (`Interop.PeachwServer`)

## 2. Key Modules Analyzed

### A. Sales & Invoicing (`frmInvoices.cs`)
- **Complexity:** High (9,500+ lines of code).
- **Core Logic:**
  - **Invoice Numbering:** Custom generation logic (`GetInvNoField_CashOR_Credit`).
  - **Data Storage:** 
    - `tblSalesInvoices`: Stores line items. Header information (Customer, Date, etc.) is repeated for every line item (Denormalized).
    - `tblInvoicePaymentHistory`: Tracks payments and balances.
    - `tblInvoicePayTypes`: Tracks payment methods (Cash, Card, Cheque).
  - **Inventory:** Real-time updates to `tblItemMaster` and `tblSerialItemTransaction`.
  - **Validation:** Extensive checks for credit limits, stock availability, and financial periods.

### B. Peachtree Integration (`clsDBLPTImport.cs`)
- **Mechanism:** Uses legacy COM Interop (`Interop.PeachwServer`).
- **Process:** 
  1. Connects to Peachtree using hardcoded credentials.
  2. Exports data (Phases, Jobs, Vendors) to XML files in the application startup path.
  3. Parses XML files to update the local SQL database.
- **Risk:** This method is fragile and relies on the Peachtree client being installed and accessible via COM. It is not suitable for a modern web application.

### C. Database Structure
- **Tables Identified:**
  - `tblSalesInvoices`: Main transaction table.
  - `tblItemMaster`: Product catalog.
  - `tblCustomerMaster`: Customer data.
  - `tblSerialItemTransaction`: Serial number tracking (crucial for tires).

## 3. Modernization Strategy (Next.js ERP)

### A. Database Migration
- **Recommendation:** Keep the existing SQL Server database but create a new Prisma Schema that maps to it.
- **Improvement:** We can create "Views" or a new normalized schema for the Web App while maintaining backward compatibility for the legacy app if needed.

### B. Peachtree Bridge
- **Problem:** The COM Interop code cannot run in a Node.js/Next.js environment.
- **Solution:** Use the Python ODBC Bridge (`peachtree-odbc-bridge-32bit.py`) we identified earlier. This allows the Web App to talk to Peachtree via HTTP/JSON, bypassing the need for COM on the web server.

### C. Sales Module Re-implementation
- **Frontend:** Rebuild `frmInvoices` as a React/Next.js page with a dynamic grid for line items.
- **Backend:** Implement the `SaveEvents` logic (Validation -> Inventory Check -> DB Insert) in a Next.js API Route (Transaction-safe).

## 4. Conclusion
The legacy system is a robust but aging WinForms application. The source code is now safely backed up in `c:\whatsapp-sql-api\legacy_src`. We have all the necessary logic to replicate the functionality in the new Web ERP.
