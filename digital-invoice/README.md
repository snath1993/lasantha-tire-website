# Digital Invoice System - සංචිතය

## 📌 Overview

මෙම පද්ධතිය පරණ Desktop System (Sanjeewa System) සහ නවීන WhatsApp/Email Bot අතර පාලමක් ලෙස ක්‍රියා කරයි.

## 🔄 කෙසේ වැඩ කරන්නේද?

### පියවර 1: පරණ System එකෙන් Queue එකට දත්ත යැවීම

ඔබේ C# Desktop Application එකේ Invoice/Quotation Save කරන කොටසේ මෙම SQL Query එක execute කරන්න:

```csharp
string insertQuery = @"
    INSERT INTO tblDigitalInvoiceQueue (DocType, RefNumber, ContactNumber, EmailAddress)
    VALUES (@DocType, @RefNumber, @ContactNumber, @EmailAddress)
";

SqlCommand cmd = new SqlCommand(insertQuery, connection);
cmd.Parameters.AddWithValue("@DocType", "INVOICE"); // හෝ "QUOTATION"
cmd.Parameters.AddWithValue("@RefNumber", invoiceNo); // උදා: "NLT00006259"
cmd.Parameters.AddWithValue("@ContactNumber", customerPhone); // උදා: "0771234567"
cmd.Parameters.AddWithValue("@EmailAddress", customerEmail); // හෝ DBNull.Value
cmd.ExecuteNonQuery();
```

### පියවර 2: Node.js Bot එක ස්වයංක්‍රීයව Handle කරයි

Bot එක සෑම තත්පර 3කට වරක් `tblDigitalInvoiceQueue` table එක check කරයි:
1. `Status = 'PENDING'` jobs අල්ලා ගනී
2. Invoice/Quotation data [View_Sales report whatsapp] view එකෙන් ලබා ගනී
3. Professional PDF එකක් generate කරයි
4. WhatsApp/Email හරහා යවයි
5. Status එක `'SENT'` හෝ `'FAILED'` කරයි

### පියවර 3: Status පරීක්ෂා කිරීම

පරණ System එකෙන් Status බලන්න:

```sql
SELECT ID, RefNumber, Status, StatusMessage, ProcessedDate
FROM tblDigitalInvoiceQueue
WHERE RefNumber = 'NLT00006259'
```

## 📊 Database Setup

### අවශ්‍ය Tables/Views:

1. **tblDigitalInvoiceQueue** (Queue Table)
   ```sql
   -- Run: scripts/create_invoice_queue_table.sql
   ```

2. **[View_Sales report whatsapp]** (දැනටමත් තිබේ)
   - මෙතනට `Phone`, `Email` columns add කරන්න ඕන

3. **[View_Quotation_Data]** (TODO: Quotations සඳහා)

## 🚀 Bot එක Start කරන ආකාරය

`index.js` file එකේ:

```javascript
const QueueProcessor = require('./digital-invoice/queueProcessor');

// Bot initialize කළ පසු:
const queueProcessor = new QueueProcessor(sqlPool, client, emailService);
queueProcessor.start();
```

## 📁 File Structure

```
digital-invoice/
├── config.js           - Configuration
├── queueProcessor.js   - Main Queue Watcher
├── pdfGenerator.js     - PDF Generation (Puppeteer)
├── templates/
│   └── professional-invoice.html
└── README.md           - මෙම ගොනුව
```

## ⚙️ Status Values

| Status | විස්තරය |
|--------|---------|
| `PENDING` | Bot එක තවම process කරන්න නැහැ |
| `PROCESSING` | දැන් process වෙමින් පවතී |
| `SENT` | සාර්ථකව යැවිණි |
| `FAILED` | අසාර්ථකයි (StatusMessage බලන්න) |

## 🔍 Troubleshooting

**Q: PDF එකක් යැවුණේ නැහැ**
- tblDigitalInvoiceQueue table එකේ StatusMessage column එක බලන්න
- Node.js Bot එක running තිබේදැයි පරීක්ෂා කරන්න

**Q: Status එක PROCESSING එකේම තිරිසන්නෙ**
- Bot එක crash වුණා විය හැක
- Bot එක restart කරන්න

**Q: Email/WhatsApp number invalid කියලා පෙන්නනවා**
- ContactNumber format එක නිවැරදිද බලන්න (0771234567 හෝ 94771234567)
- Email address valid එකක්ද බලන්න
