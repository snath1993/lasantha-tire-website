# 🏢 SANJEEWA SYSTEM - සම්පූර්ණ පද්ධති ලේඛනය
## Lasantha Tyre Traders - Inventory & Accounting Management System

---

## 📋 පොදු දළ විසිතුරු | Overview

**Sanjeewa System** යනු **Lasantha Tyre Traders** ව්‍යාපාරය සඳහා නිර්මාණය කරන ලද සම්පූර්ණ ERP (Enterprise Resource Planning) Software System එකකි. මෙම පද්ධතිය Windows Desktop Application එකක් ලෙස C# .NET Framework භාවිතයෙන් සංවර්ධනය කර ඇත.

### 🎯 ප්‍රධාන අරමුණු
- Inventory Management (භාණ්ඩ කළමනාකරණය)
- Sales & Quotation Management (විකුණුම් සහ මිල ගණන්)
- Purchase & GRN Management (මිලදී ගැනීම්)
- VAT/Non-VAT Invoice System (බදු ඉන්වොයිස් පද්ධතිය)
- Peachtree Accounting Integration (ගිණුම්කරණ ඒකාබද්ධතාවය)
- Multi-Warehouse Support (බහු ගබඩා සහාය)
- Crystal Reports Integration (වාර්තා)
- SMS Gateway Integration (SMS දැනුම්දීම්)

---

## 🏗️ Project Architecture | ව්‍යාපෘති ව්‍යුහය

### Solution Structure (Lasantha.sln)

```
📁 Lasantha Solution
│
├── 📦 MultiWearHouse (Main Application - WinForms)
│   ├── Forms (400+ Windows Forms)
│   ├── Crystal Reports (100+ .rpt files)
│   ├── DataSets (.xsd files)
│   └── Resources
│
├── 📦 DataAccess (Data Access Layer)
│   └── Database connection & queries
│
├── 📦 DBUtil (Database Utilities)
│   └── Helper functions
│
├── 📦 PCMBeans (Business Objects)
│   └── Entity classes
│
├── 📦 PCMBLL (Business Logic Layer)
│   └── Business rules
│
└── 📦 PCMDBL (Database Layer)
    └── Database operations
```

### 🛠️ Technical Stack

| Component | Technology |
|-----------|------------|
| **Language** | C# (.NET Framework 4.5.2) |
| **IDE** | Visual Studio 2015 |
| **Database** | Microsoft SQL Server Express |
| **Reporting** | SAP Crystal Reports for VS |
| **UI Framework** | Windows Forms (WinForms) |
| **Third-Party** | Infragistics Controls v9.2 |
| **Accounting** | Peachtree (Sage 50) Integration |
| **SMS** | Custom SMS Gateway API |

---

## 📊 Database Information | දත්ත සමුදාය

### Server Details
```
Server: WIN-JIAVRTFMA0N\SQLEXPRESS
Database: LasanthaTire
User: sa
Password: Admin1234
```

### ප්‍රධාන Database Tables

#### 🏭 Master Tables
| Table Name | Description |
|------------|-------------|
| `tblCompanyInformation` | Company details (Name, Address, VAT TIN) |
| `tblItemMaster` | Product/Item master data |
| `tblCustomerMaster` | Customer information |
| `tblVendorMaster` | Supplier/Vendor information |
| `tblWhseMaster` | Warehouse master |
| `tblUserMaster` | System users |
| `tblLocation` | Location/Branch details |

#### 📦 Inventory Tables
| Table Name | Description |
|------------|-------------|
| `tblStock` | Current stock levels |
| `tblInventoryAdjustment` | Stock adjustments |
| `tblWarehouseTransfer` | Inter-warehouse transfers |
| `tblBeginingBalances` | Opening balances |

#### 🛒 Sales Tables
| Table Name | Description |
|------------|-------------|
| `tblSalesInvoices` | Sales invoice header |
| `tblSalesInvoiceDetails` | Sales invoice line items |
| `tblSalesInvoicesVAT` | VAT invoices (IRD compliant) |
| `tblSOrder` | Sales orders |
| `tblP_Order` | Quotations/Purchase orders |
| `tblDeliveryNote` | Delivery notes |
| `tblCustomerReturn` | Customer returns |

#### 🛍️ Purchase Tables
| Table Name | Description |
|------------|-------------|
| `tblDirectSupInvoice` | Supplier invoices (GRN) |
| `tblDirectSupInvoiceDetails` | GRN line items |
| `tblPurchaseOrder` | Purchase orders |
| `tblSupplierReturn` | Supplier returns |

#### 💰 Accounting Tables
| Table Name | Description |
|------------|-------------|
| `tblAccountLink` | Peachtree account mapping |
| `tblVATSettings` | VAT configuration |
| `tblTransactions` | Financial transactions |

---

## 🖥️ ප්‍රධාන Windows Forms | Main Application Forms

### 📌 Core Forms

#### 1. Login & Main
| Form | File | Description |
|------|------|-------------|
| Login | `frmLogin.cs` | User authentication |
| Main Menu | `frmMain.cs` | Main application window with menu |
| Main Screen | `frmMainScreen.cs` | Dashboard/Home screen |

#### 2. Master Data Forms
| Form | File | Description |
|------|------|-------------|
| Item Master | `frmItemMaster.cs` | Product setup (Name, Price, VAT) |
| Customer Master | `frmCustomerMaster.cs` | Customer information |
| Vendor Master | `frmVendorMaster.cs` | Supplier information |
| Warehouse Master | `frmWareHouse.cs` | Warehouse setup |
| User Management | `frmAddUser.cs` | User creation |
| User Authorization | `frmUserAuthentication.cs` | Access rights |

#### 3. Sales & Invoicing Forms
| Form | File | Description |
|------|------|-------------|
| **Sales Invoice** | `frmInvoices.cs` | Main invoice form (VAT/Non-VAT) |
| Sales Order | `frmpurchesorder.cs` | Quotations & Sales Orders |
| Delivery Note | `frmDeliveryNote.cs` | Delivery documentation |
| Customer Returns | `frmCustomerReturns.cs` | Return processing |
| Invoice List | `frmInvoiceList.cs` | Invoice search/view |
| Invoice Print | `frmInvoicePrint.cs` | Print invoices |

#### 4. Purchase & GRN Forms
| Form | File | Description |
|------|------|-------------|
| **GRN Entry** | `frmDirectSupInvoice.cs` | Goods Received Note |
| Purchase Order | `frmPurchaseOder.cs` | PO creation |
| Supplier Invoice | `frmSupInvoice.cs` | Supplier billing |
| Supplier Returns | `frmSupplierReturn.cs` | Return to supplier |
| GRN List | `frmDirectSupInvoiceList.cs` | GRN search/view |

#### 5. Inventory Forms
| Form | File | Description |
|------|------|-------------|
| Stock Adjustment | `frmInventotyAdjustment.cs` | Quantity adjustments |
| Stock Transfer | `frmWareHouseTrans.cs` | Inter-warehouse |
| Qty on Hand | `frmQtyOnHand.cs` | Stock inquiry |
| Issue Note | `frmIssueNote.cs` | Stock issues |
| Beginning Balance | `frmBeginingBalances.cs` | Opening stock |

#### 6. Report Forms
| Form | File | Description |
|------|------|-------------|
| Sales Summary | `frmSalesSummary.cs` | Sales reports |
| Daily Sales | `frmDailySales.cs` | Daily collection |
| Inventory Movement | `frmInventoryMovement.cs` | Stock movement |
| Invoice Wise Sales | `frmInvoiceWiseSales.cs` | Per-invoice reports |
| Item Wise Sales | `frmViewerItemWiseSales.cs` | Per-item reports |
| Location Sales | `frmLocationWiseSales.cs` | Branch-wise reports |
| Valuation Report | `frmValuation.cs` | Stock valuation |

#### 7. Accounting Integration
| Form | File | Description |
|------|------|-------------|
| Account Link | `frmAccountLink.cs` | Peachtree mapping |
| Import Peachtree | `frmImportFromPeachTree.cs` | Data import |
| Chart of Accounts | `frmChartofAccount.cs` | Account master |
| Settings - Accounts | `frmSettingsAccounts.cs` | Account config |
| VAT/NBT Report | `frmNBTVATReport.cs` | Tax reports |

---

## 📑 Crystal Reports | වාර්තා

### 📊 Report Categories

#### Sales Reports
| Report | File | Description |
|--------|------|-------------|
| Tax Invoice | `CRTaxInvoice.rpt` | VAT Invoice print |
| Invoice | `CRInvoice.rpt` | Standard invoice |
| Quotation | `rptQuotation.rpt` | Customer quotation |
| Sales Order | `rptSalesOrder.rpt` | Sales order print |
| Credit Note | `CRCreditNote.rpt` | Customer credit |
| Delivery Note | `CRDeliveryNote.rpt` | Delivery slip |

#### Purchase Reports
| Report | File | Description |
|--------|------|-------------|
| GRN Report | `rptSupplierInvoice.rpt` | GRN print |
| Purchase Order | `CRPurchaseOrder.rpt` | PO print |
| Supplier Return | `CRSupplierReturn.rpt` | Return slip |

#### Inventory Reports
| Report | File | Description |
|--------|------|-------------|
| Stock on Hand | `rptwherehousewiseqty.rpt` | Current stock |
| Stock Movement | `CRInvMovement.rpt` | Movement report |
| Valuation | `CRValuation.rpt` | Stock value |
| Reorder Report | `CRReorderReport.rpt` | Low stock |

#### Summary Reports
| Report | File | Description |
|--------|------|-------------|
| Daily Collection | `Rpt_DaillyCollectionSummary_new.rpt` | Daily cash |
| Sales Summary | `CRDailySalesSummary.rpt` | Sales total |
| Item Wise Sales | `CRItemWiseSales.rpt` | Per-product |
| Customer Wise | `CRCustomerWiseSales.rpt` | Per-customer |

---

## 💼 VAT/Non-VAT System | බදු පද්ධතිය

### 🏢 Two Company Separation (Peachtree)

මෙම පද්ධතිය VAT සහ Non-VAT ව්‍යාපාර වෙන්කර පවත්වාගෙන යයි:

#### VAT Business (Lasantha TYRE TRADERS)
```
Warehouse: Lasantha TYRE TRADERS
Peachtree AR Account: 6500-00
Peachtree AP Account: 8000-00
VAT Rate: 18%
```

#### Non-VAT Business (NEW Lasantha TYRE TRADERS)
```
Warehouse: NEW Lasantha TYRE TRADERS
Peachtree AR Account: 6500-01
Peachtree AP Account: 8000-01
VAT Rate: 0%
```

### 📋 IRD Gazette Compliant VAT Invoice

IRD Gazette (2025.11.17) අනුව VAT Invoice Format:

```
Format: YYMMM_QQQQ_XXXXX
Example: 26JAN_LT_1

YY    = Year (26 for 2026)
MMM   = Month (JAN, FEB, etc.)
QQQQ  = Business Prefix (LT)
XXXXX = Sequential number (resets monthly)
```

#### VAT Invoice Required Fields
- Tax Invoice Number (IRD Format)
- Supplier TIN: `743321219-7000`
- Purchaser TIN
- Base Amount
- VAT Amount (18%)
- Total Amount
- Amount in Words
- Place of Supply

---

## 🔗 Peachtree Integration | Peachtree ඒකාබද්ධතාවය

### Integration Method
Peachtree SDK (Interop.PeachwServer) භාවිතයෙන් සෘජු ඒකාබද්ධතාවය:

```csharp
using Interop.PeachwServer;

Interop.PeachwServer.Application app;
Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
app = login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
```

### 📤 Export to Peachtree
- Sales Invoices → Sales Journal
- Customer Returns → Credit Memos
- Supplier Invoices → Purchase Journal
- Supplier Returns → Vendor Credits
- Inventory Adjustments → Inventory Adjustment

### 📥 Import from Peachtree
- Chart of Accounts
- Customer List
- Vendor List
- Item Master
- Employee List
- Job List

### XML Integration Files (XMLFILES folder)
| File | Purpose |
|------|---------|
| `SalesInvice.xml` | Sales invoice export |
| `Receipts.xml` | Payment receipts |
| `PurchaseJournal.xml` | Purchases export |
| `Vendor.xml` | Vendor import |
| `CustomerMaster.xml` | Customer import |
| `Jobs.xml` | Job import |

---

## 📱 SMS Integration | SMS පද්ධතිය

### SMS Manager (`SmsManager.cs`)

```csharp
public class SmsManager
{
    public async Task SendSms(SendSmsDto sms)
    {
        // HTTP POST to SMS Gateway
        var response = await httpclient.PostAsync(SmsConfig.SmsUrl, data);
    }
}
```

### SMS Configuration (`SmsGatewayConfig.txt`)
- API URL
- Authentication Token
- Sender ID

### Use Cases
- Invoice notification to customer
- Payment reminder
- Stock alert
- Delivery notification

---

## ⚙️ System Settings | පද්ධති සැකසුම්

### Settings Forms
| Form | Purpose |
|------|---------|
| `frmSettings.cs` | General settings |
| `frmSettingsAccounts.cs` | Account mappings |
| `frmSettingsTax.cs` | VAT/NBT rates |
| `frmSettingsOther.cs` | Other configurations |
| `frmDefaultSettings.cs` | Default values |
| `frmSystem.cs` | System parameters |

### Configurable Items
- Company Information
- Tax Rates (VAT, NBT)
- Default Warehouse
- Invoice Numbering
- Report Paths
- Printer Settings
- Decimal Points
- User Permissions

---

## 👤 User Management | පරිශීලක කළමනාකරණය

### User Authorization System

```
frmUserAuthentication.cs - Form-level access control
```

Each user can be assigned:
- **Add** - Create new records
- **Edit** - Modify existing records
- **Delete** - Remove records
- **Print** - Print reports
- **View** - View-only access

### Permission Copy Feature
`frmCopyUserAuthentication.cs` - Copy permissions from one user to another

---

## 🔄 Key Business Processes | ප්‍රධාන ව්‍යාපාර ක්‍රියාවලි

### 1. 📦 Purchase Process
```
Purchase Order → GRN Entry → Supplier Invoice → Payment
       ↓              ↓              ↓
    frmPurchaseOder  frmDirectSupInvoice  frmSupInvoice
```

### 2. 🛒 Sales Process
```
Quotation → Sales Order → Invoice → Delivery Note → Receipt
     ↓           ↓           ↓            ↓
frmpurchesorder  frmInvoices  frmDeliveryNote  frmFinalRecept
```

### 3. 📊 Inventory Process
```
Beginning Balance → GRN/Issues → Adjustments → Stock Reports
        ↓              ↓              ↓
frmBeginingBalances  frmIssueNote  frmInventotyAdjustment
```

### 4. 💰 Accounting Process
```
Transactions → Account Link → Export to Peachtree
      ↓              ↓              ↓
  Forms data   frmAccountLink  frmImportFromPeachTree
```

---

## 🗂️ File Structure | ගොනු ව්‍යුහය

```
📁 Debug SMS/
│
├── 📁 Project/
│   ├── 📁 UserAutherization/      (Main Solution)
│   │   ├── 📁 UserAutherization/  (WinForms Project)
│   │   │   ├── *.cs               (Form code files)
│   │   │   ├── *.designer.cs      (Form designer)
│   │   │   ├── *.resx             (Resources)
│   │   │   ├── *.rpt              (Crystal Reports)
│   │   │   └── *.xsd              (DataSets)
│   │   └── 📁 DBUtil/             (Database Utility)
│   ├── 📁 DataAccess/             (Data Layer)
│   ├── 📁 PCMBeans/               (Entity Objects)
│   ├── 📁 PCMBLL/                 (Business Logic)
│   └── 📁 PCMDBL/                 (Database Logic)
│
├── 📁 REPORTS/                    (Crystal Reports Collection)
│   ├── CRInvoice.rpt
│   ├── CRTaxInvoice.rpt
│   └── ... (100+ reports)
│
├── 📁 XMLFILES/                   (Peachtree XML Exports)
│   ├── SalesInvice.xml
│   ├── PurchaseJournal.xml
│   └── ...
│
├── 📄 UserAutherization.exe       (Main Application)
├── 📄 DataAccess.dll              (Data Layer DLL)
├── 📄 DBUtil.dll                  (Utility DLL)
└── 📄 *.dll                       (Supporting Libraries)
```

---

## 🔧 Development & Build | සංවර්ධනය

### Build Requirements
- Visual Studio 2015 or later
- .NET Framework 4.5.2
- SAP Crystal Reports Runtime
- SQL Server 2012 Express or later
- Infragistics WinForms v9.2

### Build Steps
```powershell
# 1. Open Solution
# Open Lasantha.sln in Visual Studio

# 2. Restore NuGet Packages
# Build → Restore NuGet Packages

# 3. Build Solution
# Build → Build Solution (Ctrl+Shift+B)

# 4. Output Location
# bin\Debug\UserAutherization.exe
```

### Deployment
```powershell
# Copy these files to deployment folder:
# - UserAutherization.exe
# - DataAccess.dll
# - DBUtil.dll
# - All Crystal Reports DLLs
# - All Infragistics DLLs
# - REPORTS folder
# - config.txt
```

---

## 📞 Support Information | සහාය තොරතුරු

### Business Information
```
Company: Lasantha Tyre Traders
Address: 1035 Pannipitiya Rd, Kumaragewatta, Battaramulla
Tel: 0112773232
Fax: 0112773231
TIN: 743321219-7000
```

### Database Backup
```sql
-- Backup Command
BACKUP DATABASE LasanthaTire 
TO DISK = 'D:\Backup\LasanthaTire_backup.bak'
WITH FORMAT, MEDIANAME = 'LasanthaTireBackup'
```

---

## 📝 Recent Updates | මෑත යාවත්කාලීන

### January 2026
1. ✅ **VAT/Non-VAT Separation** - Peachtree account separation (6500-01, 8000-01)
2. ✅ **GRN Report Header Fix** - Company name shows correctly for Non-VAT
3. ✅ **IRD VAT Invoice Format** - YYMMM_QQQQ_XXXXX format implementation
4. ✅ **Crystal Report Formulas** - VAT Amount and Price calculations fixed

### Pending Features
- 📧 Email Invoice PDF
- 📱 WhatsApp Integration
- 📊 Dashboard Analytics

---

## 📖 Glossary | පාරිභාෂික ශබ්ද

| Term | Sinhala | Description |
|------|---------|-------------|
| GRN | භාණ්ඩ ලදු සටහන | Goods Received Note |
| PO | මිලදී ගැනීම් ඇණවුම | Purchase Order |
| SO | විකුණුම් ඇණවුම | Sales Order |
| VAT | වට් බද්ද | Value Added Tax |
| NBT | ජා.ස.බ. | Nation Building Tax |
| TIN | බදු හැඳුනුම් අංකය | Tax Identification Number |
| AR | ගෙවිය යුතු ගිණුම් | Accounts Receivable |
| AP | ගෙවිය යුතු ගිණුම් | Accounts Payable |
| FG | නිමි භාණ්ඩ | Finished Goods |
| BOM | ද්‍රව්‍ය බිල්පත | Bill of Materials |
| BOQ | ප්‍රමාණ බිල්පත | Bill of Quantities |

---

## 📄 License & Copyright

```
© 2008-2026 Lasantha Tyre Traders
All Rights Reserved

Developed by: Sanjeewa System Development Team
For Internal Use Only
```

---

*Last Updated: January 14, 2026*
*Document Version: 1.0*
