// =====================================================
// Invoice Deletion Job with Backup & Counter Decrement
// Purpose: Delete invoice rows after item sale and update counters
// =====================================================

require('dotenv').config();
const sql = require('mssql');
const fs = require('fs').promises;
const path = require('path');

// SQL Server configuration for ERP database
const erpSqlConfig = {
    server: process.env.SQL_SERVER,
    database: process.env.SQL_DATABASE,
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    options: {
        encrypt: false,
        trustServerCertificate: true,
        enableArithAbort: true
    }
};

// SQL Server configuration for WhatsAppAI database (backup)
const backupSqlConfig = {
    server: process.env.SQL_SERVER,
    database: 'WhatsAppAI',
    user: process.env.SQL_USER,
    password: process.env.SQL_PASSWORD,
    options: {
        encrypt: false,
        trustServerCertificate: true,
        enableArithAbort: true
    }
};

class InvoiceDeletionJob {
    constructor() {
        this.jobName = 'Invoice Deletion Job';
        this.erpPool = null;
        this.backupPool = null;
        this.logFile = path.join(__dirname, '../logs/invoice-deletion.log');
    }

    // Initialize database connections
    async initialize() {
        try {
            console.log('🔌 Connecting to databases...');
            
            // Create separate connection pools (don't use global sql.connect)
            this.erpPool = new sql.ConnectionPool(erpSqlConfig);
            await this.erpPool.connect();
            
            this.backupPool = new sql.ConnectionPool(backupSqlConfig);
            await this.backupPool.connect();
            
            console.log('✅ Database connections established');
            return true;
        } catch (error) {
            console.error('❌ Database connection error:', error.message);
            throw error;
        }
    }

    // Log activity to file
    async log(message) {
        const timestamp = new Date().toISOString();
        const logMessage = `[${timestamp}] ${message}\n`;
        console.log(message);
        
        try {
            await fs.appendFile(this.logFile, logMessage);
        } catch (error) {
            console.error('Failed to write log:', error.message);
        }
    }

    // Backup invoice rows before deletion
    async backupInvoiceRows(invoiceNo, itemId, reason) {
        try {
            await this.log(`💾 Backing up rows for invoice: ${invoiceNo}`);

            // Get rows to backup
            const selectQuery = `
                SELECT * FROM ${erpSqlConfig.database}.dbo.tblSalesInvoices
                WHERE InvoiceNo = @invoiceNo
            `;

            const request = this.erpPool.request();
            request.input('invoiceNo', sql.VarChar, invoiceNo);
            const result = await request.query(selectQuery);

            if (result.recordset.length === 0) {
                await this.log(`⚠️  No rows found for invoice: ${invoiceNo}`);
                return {
                    success: false,
                    rowCount: 0,
                    reason: 'No rows found'
                };
            }

            // Insert into backup table
            for (const row of result.recordset) {
                const insertQuery = `
                    INSERT INTO WhatsAppAI.dbo.TblDelBack (
                        InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount,
                        NoofDistributions, DistributionNo, ItemID, Qty, Description,
                        GLAcount, UnitPrice, TotalDiscountPercen, Amount, TotalDiscountAmount,
                        Tax1Amount, Tax2Amount, GrossTotal, NetTotal, CurrentDate,
                        Time, Currentuser, IsExport, CustomerPO, UOM,
                        JobID, SONO, Location, TTType1, TTType2,
                        IsReturn, TTType3, Tax3Amount, RemainQty, SalesRep,
                        CostPrrice, PaymentM, ItemClass, ItemType, IsVoid,
                        VoidReson, VoidUser, Comments, Tax1Rate, Tax2Rate,
                        SubValue, Tax3Rate,
                        WHID, CusName, CustomerName, ContactNo, VehicleNo, JobDoneBy, Mileage,
                        DeletedByJob, DeleteReason, CounterType, OldCounterValue, WatchedItemID
                    ) VALUES (
                        @InvoiceNo, @CustomerID, @DeliveryNoteNos, @InvoiceDate, @ARAccount,
                        @NoofDistributions, @DistributionNo, @ItemID, @Qty, @Description,
                        @GLAcount, @UnitPrice, @TotalDiscountPercen, @Amount, @TotalDiscountAmount,
                        @Tax1Amount, @Tax2Amount, @GrossTotal, @NetTotal, @CurrentDate,
                        @Time, @Currentuser, @IsExport, @CustomerPO, @UOM,
                        @JobID, @SONO, @Location, @TTType1, @TTType2,
                        @IsReturn, @TTType3, @Tax3Amount, @RemainQty, @SalesRep,
                        @CostPrrice, @PaymentM, @ItemClass, @ItemType, @IsVoid,
                        @VoidReson, @VoidUser, @Comments, @Tax1Rate, @Tax2Rate,
                        @SubValue, @Tax3Rate,
                        @WHID, @CusName, @CustomerName, @ContactNo, @VehicleNo, @JobDoneBy, @Mileage,
                        @DeletedByJob, @DeleteReason, @CounterType, @OldCounterValue, @WatchedItemID
                    )
                `;

                const backupRequest = this.backupPool.request();
                
                // Add all parameters
                backupRequest.input('InvoiceNo', sql.VarChar, row.InvoiceNo);
                backupRequest.input('CustomerID', sql.VarChar, row.CustomerID);
                backupRequest.input('DeliveryNoteNos', sql.VarChar, row.DeliveryNoteNos);
                backupRequest.input('InvoiceDate', sql.DateTime, row.InvoiceDate);
                backupRequest.input('ARAccount', sql.VarChar, row.ARAccount);
                backupRequest.input('NoofDistributions', sql.Float, row.NoofDistributions);
                backupRequest.input('DistributionNo', sql.Float, row.DistributionNo);
                backupRequest.input('ItemID', sql.VarChar, row.ItemID);
                backupRequest.input('Qty', sql.Float, row.Qty);
                backupRequest.input('Description', sql.VarChar, row.Description);
                backupRequest.input('GLAcount', sql.VarChar, row.GLAcount);
                backupRequest.input('UnitPrice', sql.Float, row.UnitPrice);
                backupRequest.input('TotalDiscountPercen', sql.Float, row.TotalDiscountPercen);
                backupRequest.input('Amount', sql.Float, row.Amount);
                backupRequest.input('TotalDiscountAmount', sql.Float, row.TotalDiscountAmount);
                backupRequest.input('Tax1Amount', sql.Float, row.Tax1Amount);
                backupRequest.input('Tax2Amount', sql.Float, row.Tax2Amount);
                backupRequest.input('GrossTotal', sql.Float, row.GrossTotal);
                backupRequest.input('NetTotal', sql.Float, row.NetTotal);
                backupRequest.input('CurrentDate', sql.DateTime, row.CurrentDate);
                backupRequest.input('Time', sql.VarChar, row.Time);
                backupRequest.input('Currentuser', sql.VarChar, row.Currentuser);
                backupRequest.input('IsExport', sql.Bit, row.IsExport);
                backupRequest.input('CustomerPO', sql.VarChar, row.CustomerPO);
                backupRequest.input('UOM', sql.VarChar, row.UOM);
                backupRequest.input('JobID', sql.VarChar, row.JobID);
                backupRequest.input('SONO', sql.VarChar, row.SONO);
                backupRequest.input('Location', sql.VarChar, row.Location);
                backupRequest.input('TTType1', sql.VarChar, row.TTType1);
                backupRequest.input('TTType2', sql.VarChar, row.TTType2);
                backupRequest.input('IsReturn', sql.Bit, row.IsReturn);
                backupRequest.input('TTType3', sql.VarChar, row.TTType3);
                backupRequest.input('Tax3Amount', sql.Float, row.Tax3Amount);
                backupRequest.input('RemainQty', sql.Float, row.RemainQty);
                backupRequest.input('SalesRep', sql.VarChar, row.SalesRep);
                backupRequest.input('CostPrrice', sql.Float, row.CostPrrice);
                backupRequest.input('PaymentM', sql.VarChar, row.PaymentM);
                backupRequest.input('ItemClass', sql.VarChar, row.ItemClass);
                backupRequest.input('ItemType', sql.VarChar, row.ItemType);
                backupRequest.input('IsVoid', sql.Bit, row.IsVoid);
                backupRequest.input('VoidReson', sql.VarChar, row.VoidReson);
                backupRequest.input('VoidUser', sql.VarChar, row.VoidUser);
                backupRequest.input('Comments', sql.NVarChar, row.Comments);
                backupRequest.input('Tax1Rate', sql.Float, row.Tax1Rate);
                backupRequest.input('Tax2Rate', sql.Float, row.Tax2Rate);
                backupRequest.input('SubValue', sql.Float, row.SubValue);
                backupRequest.input('Tax3Rate', sql.Float, row.Tax3Rate);
                
                // Missing fields (CRITICAL FIX)
                backupRequest.input('WHID', sql.VarChar, row.WHID);
                backupRequest.input('CusName', sql.VarChar, row.CusName);
                backupRequest.input('CustomerName', sql.VarChar, row.CustomerName);
                backupRequest.input('ContactNo', sql.VarChar, row.ContactNo);
                backupRequest.input('VehicleNo', sql.VarChar, row.VehicleNo);
                backupRequest.input('JobDoneBy', sql.VarChar, row.JobDoneBy);
                backupRequest.input('Mileage', sql.VarChar, row.Mileage);
                
                // Tracking columns
                backupRequest.input('DeletedByJob', sql.VarChar, this.jobName);
                backupRequest.input('DeleteReason', sql.VarChar, reason);
                backupRequest.input('CounterType', sql.VarChar, null);
                backupRequest.input('OldCounterValue', sql.VarChar, null);
                backupRequest.input('WatchedItemID', sql.VarChar, itemId);

                await backupRequest.query(insertQuery);
            }

            await this.log(`✅ Backed up ${result.recordset.length} rows to TblDelBack`);

            return {
                success: true,
                rowCount: result.recordset.length,
                rows: result.recordset
            };
        } catch (error) {
            await this.log(`❌ Error backing up rows: ${error.message}`);
            throw error;
        }
    }

    // NEW METHOD: Delete invoice and create row from template
    // If transaction is provided, all ERP writes happen atomically.
    async deleteAndCreateFromTemplate(invoiceNo, transaction = null) {
        try {
            await this.log(`🗑️  Deleting invoice and creating template row: ${invoiceNo}`);

            // Step 1: Detect invoice prefix
            const prefix = invoiceNo.substring(0, 3).toUpperCase(); // 'INV' or 'NLT'
            await this.log(`📋 Detected prefix: ${prefix}`);

            // Step 2: Get template from WhatsAppAI database
            const templateResult = await this.backupPool.request()
                .input('prefix', sql.VarChar, prefix)
                .query(`
                    SELECT TOP 1 * 
                    FROM WhatsAppAI.dbo.TblInvoiceTemplates 
                    WHERE InvoicePrefix = @prefix
                `);

            if (templateResult.recordset.length === 0) {
                throw new Error(`No template found for prefix: ${prefix}`);
            }

            const template = templateResult.recordset[0];
            await this.log(`✅ Template loaded: ${template.Description} (${template.ItemType})`);

            // Step 3: Delete ALL rows from original invoice
            const deleteQuery = `
                DELETE FROM ${erpSqlConfig.database}.dbo.tblSalesInvoices
                WHERE InvoiceNo = @invoiceNo
            `;

            const deleteRequest = transaction ? new sql.Request(transaction) : this.erpPool.request();
            deleteRequest.input('invoiceNo', sql.VarChar, invoiceNo);
            const deleteResult = await deleteRequest.query(deleteQuery);

            await this.log(`✅ Deleted ${deleteResult.rowsAffected[0]} rows from tblSalesInvoices`);

            // Step 4: Insert NEW row with template data
            const now = new Date();
            const moment = require('moment');
            const timeStr = moment(now).format('h:mm A');

            await this.log(`📝 Creating new row with template data...`);
            await this.log(`   Invoice Number: ${invoiceNo} (from deleted invoice)`);
            await this.log(`   Date/Time: ${moment(now).format('YYYY-MM-DD')} ${timeStr} (job run time)`);
            await this.log(`   Template Data: ${template.Description}, Qty: ${template.Qty}, Amount: ${template.Amount}`);

            const insertRequest = transaction ? new sql.Request(transaction) : this.erpPool.request();
            
            // ONLY THESE 4 FIELDS CHANGE:
            insertRequest.input('InvoiceNo', sql.VarChar, invoiceNo);  // ← Delete වෙන invoice number
            insertRequest.input('CurrentDate', sql.DateTime, now);      // ← Job run date
            insertRequest.input('Time', sql.VarChar, timeStr);          // ← Job run time
            insertRequest.input('IsVoid', sql.Bit, 1);                  // ← Voided
            
            // ALL OTHER FIELDS FROM TEMPLATE:
            insertRequest.input('CustomerID', sql.VarChar, template.CustomerID);
            insertRequest.input('DeliveryNoteNos', sql.VarChar, template.DeliveryNoteNos);
            insertRequest.input('InvoiceDate', sql.DateTime, template.InvoiceDate || now);
            insertRequest.input('ARAccount', sql.VarChar, template.ARAccount);
            insertRequest.input('NoofDistributions', sql.Int, template.NoofDistributions);
            insertRequest.input('DistributionNo', sql.Int, template.DistributionNo);
            insertRequest.input('ItemID', sql.VarChar, template.ItemID);
            insertRequest.input('Qty', sql.Int, template.Qty);
            insertRequest.input('Description', sql.VarChar, template.Description);
            insertRequest.input('GLAcount', sql.VarChar, template.GLAcount);
            insertRequest.input('UnitPrice', sql.Decimal(18,4), template.UnitPrice);
            insertRequest.input('TotalDiscountPercen', sql.Decimal(18,4), template.TotalDiscountPercen);
            insertRequest.input('Amount', sql.Decimal(18,4), template.Amount);
            insertRequest.input('TotalDiscountAmount', sql.Decimal(18,4), template.TotalDiscountAmount);
            insertRequest.input('Tax1Amount', sql.Decimal(18,4), template.Tax1Amount);
            insertRequest.input('Tax2Amount', sql.Decimal(18,4), template.Tax2Amount);
            insertRequest.input('Tax3Amount', sql.Decimal(18,4), template.Tax3Amount);
            insertRequest.input('Tax1Rate', sql.Decimal(18,4), template.Tax1Rate);
            insertRequest.input('Tax2Rate', sql.Decimal(18,4), template.Tax2Rate);
            insertRequest.input('Tax3Rate', sql.Decimal(18,4), template.Tax3Rate);
            insertRequest.input('TTType1', sql.VarChar, template.TTType1);
            insertRequest.input('TTType2', sql.VarChar, template.TTType2);
            insertRequest.input('TTType3', sql.VarChar, template.TTType3);
            insertRequest.input('GrossTotal', sql.Decimal(18,4), template.GrossTotal);
            insertRequest.input('NetTotal', sql.Decimal(18,4), template.NetTotal);
            insertRequest.input('SubValue', sql.Decimal(18,4), template.SubValue);
            insertRequest.input('Currentuser', sql.VarChar, 'Srinath');
            insertRequest.input('IsExport', sql.Bit, template.IsExport);
            insertRequest.input('IsReturn', sql.Bit, template.IsReturn);
            insertRequest.input('CustomerPO', sql.VarChar, template.CustomerPO);
            insertRequest.input('CustomerName', sql.VarChar, template.CustomerName);
            insertRequest.input('CusName', sql.VarChar, template.CusName);
            insertRequest.input('ContactNo', sql.VarChar, template.ContactNo);
            insertRequest.input('UOM', sql.VarChar, template.UOM);
            insertRequest.input('JobID', sql.VarChar, template.JobID);
            insertRequest.input('SONO', sql.VarChar, template.SONO);
            insertRequest.input('Location', sql.VarChar, template.Location);
            insertRequest.input('WHID', sql.VarChar, template.WHID);
            insertRequest.input('RemainQty', sql.Int, template.RemainQty);
            insertRequest.input('SalesRep', sql.VarChar, template.SalesRep);
            insertRequest.input('CostPrrice', sql.Decimal(18,4), template.CostPrrice);
            insertRequest.input('PaymentM', sql.VarChar, template.PaymentM);
            insertRequest.input('ItemClass', sql.VarChar, template.ItemClass);
            insertRequest.input('ItemType', sql.VarChar, template.ItemType);
            insertRequest.input('VoidReson', sql.VarChar, 'System');
            insertRequest.input('VoidUser', sql.VarChar, 'Srinath');
            insertRequest.input('Comments', sql.VarChar, template.Comments);
            // Missing columns added
            insertRequest.input('ServiceCharge', sql.Decimal(18,4), template.ServiceCharge);
            insertRequest.input('LineDiscountAmount', sql.Decimal(18,4), template.LineDiscountAmount);
            insertRequest.input('LineDiscountPercentage', sql.VarChar, template.LineDiscountPercentage);
            insertRequest.input('IsInclusive', sql.Int, template.IsInclusive);
            insertRequest.input('IsDirect', sql.Bit, template.IsDirect);
            insertRequest.input('InclusivePrice', sql.Decimal(18,4), template.InclusivePrice);
            insertRequest.input('LineTax', sql.Decimal(18,4), template.LineTax);
            insertRequest.input('PaidAmount', sql.Decimal(18,4), template.PaidAmount);

            const insertQuery = `
                INSERT INTO ${erpSqlConfig.database}.dbo.tblSalesInvoices (
                    InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount,
                    NoofDistributions, DistributionNo, ItemID, Qty, Description,
                    GLAcount, UnitPrice, TotalDiscountPercen, Amount, TotalDiscountAmount,
                    Tax1Amount, Tax2Amount, GrossTotal, NetTotal, CurrentDate,
                    Time, Currentuser, IsExport, CustomerPO, UOM,
                    JobID, SONO, Location, TTType1, TTType2,
                    IsReturn, TTType3, Tax3Amount, RemainQty, SalesRep,
                    CostPrrice, PaymentM, ItemClass, ItemType, IsVoid,
                    VoidReson, VoidUser, Comments, Tax1Rate, Tax2Rate,
                    SubValue, Tax3Rate, WHID, CusName, CustomerName,
                    ContactNo,
                    ServiceCharge, LineDiscountAmount, LineDiscountPercentage, IsInclusive,
                    IsDirect, InclusivePrice, LineTax, PaidAmount
                ) VALUES (
                    @InvoiceNo, @CustomerID, @DeliveryNoteNos, @InvoiceDate, @ARAccount,
                    @NoofDistributions, @DistributionNo, @ItemID, @Qty, @Description,
                    @GLAcount, @UnitPrice, @TotalDiscountPercen, @Amount, @TotalDiscountAmount,
                    @Tax1Amount, @Tax2Amount, @GrossTotal, @NetTotal, @CurrentDate,
                    @Time, @Currentuser, @IsExport, @CustomerPO, @UOM,
                    @JobID, @SONO, @Location, @TTType1, @TTType2,
                    @IsReturn, @TTType3, @Tax3Amount, @RemainQty, @SalesRep,
                    @CostPrrice, @PaymentM, @ItemClass, @ItemType, @IsVoid,
                    @VoidReson, @VoidUser, @Comments, @Tax1Rate, @Tax2Rate,
                    @SubValue, @Tax3Rate, @WHID, @CusName, @CustomerName,
                    @ContactNo,
                    @ServiceCharge, @LineDiscountAmount, @LineDiscountPercentage, @IsInclusive,
                    @IsDirect, @InclusivePrice, @LineTax, @PaidAmount
                )
            `;

            await insertRequest.query(insertQuery);
            await this.log(`✅ Created template-based void row`);
            await this.log(`   InvoiceNo: ${invoiceNo}`);
            await this.log(`   ItemID: ${template.ItemID} (from template)`);
            await this.log(`   Description: ${template.Description} (from template)`);
            await this.log(`   Qty: ${template.Qty} (from template)`);
            await this.log(`   Amount: ${template.Amount} (from template)`);
            await this.log(`   IsVoid: true`);

            return {
                success: true,
                rowsDeleted: deleteResult.rowsAffected[0],
                templateUsed: prefix,
                voidRowCreated: true
            };

        } catch (error) {
            await this.log(`❌ Error in template-based deletion: ${error.message}`);
            throw error;
        }
    }

    // Delete invoice rows and create void placeholder
    async deleteAndVoidInvoiceRows(invoiceNo) {
        try {
            await this.log(`🗑️  Deleting and voiding invoice: ${invoiceNo}`);

            // Step 1: Get ALL original invoice data (complete row)
            const originalData = await this.erpPool.request()
                .input('invoiceNo', sql.VarChar, invoiceNo)
                .query(`
                    SELECT TOP 1 *
                    FROM ${erpSqlConfig.database}.dbo.tblSalesInvoices
                    WHERE InvoiceNo = @invoiceNo
                    ORDER BY DistributionNo
                `);

            if (originalData.recordset.length === 0) {
                throw new Error(`Invoice ${invoiceNo} not found`);
            }

            const original = originalData.recordset[0];

            // Validate required fields
            if (!original.InvoiceDate) {
                throw new Error(`Invoice ${invoiceNo} has no InvoiceDate - cannot create void placeholder`);
            }

            // Log original data for debugging
            await this.log(`📋 Original invoice data:`);
            await this.log(`   InvoiceNo: ${original.InvoiceNo}`);
            await this.log(`   ItemID: ${original.ItemID} (will be preserved)`);
            await this.log(`   Description: ${original.Description} (will be preserved)`);
            await this.log(`   Qty: ${original.Qty} (will be preserved)`);
            await this.log(`   Amount: ${original.Amount} (will be preserved)`);
            await this.log(`   CustomerID: ${original.CustomerID || 'NULL'}`);
            await this.log(`   InvoiceDate: ${original.InvoiceDate}`);
            await this.log(`   IsVoid: ${original.IsVoid} (current status)`);

            // Step 2: Delete all rows
            const deleteQuery = `
                DELETE FROM ${erpSqlConfig.database}.dbo.tblSalesInvoices
                WHERE InvoiceNo = @invoiceNo
            `;

            const deleteRequest = this.erpPool.request();
            deleteRequest.input('invoiceNo', sql.VarChar, invoiceNo);
            const deleteResult = await deleteRequest.query(deleteQuery);

            await this.log(`✅ Deleted ${deleteResult.rowsAffected[0]} rows from tblSalesInvoices`);

            // Step 3: Create void placeholder with ALL original data
            const now = new Date();
            const voidTime = now.toTimeString().substring(0, 8); // HH:mm:ss format
            
            const voidInsertQuery = `
                INSERT INTO ${erpSqlConfig.database}.dbo.tblSalesInvoices (
                    InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, ARAccount,
                    NoofDistributions, DistributionNo, ItemID, Qty, Description,
                    GLAcount, UnitPrice, TotalDiscountPercen, Amount, TotalDiscountAmount,
                    Tax1Amount, Tax2Amount, GrossTotal, NetTotal, CurrentDate,
                    Time, Currentuser, IsExport, CustomerPO, UOM,
                    JobID, SONO, Location, TTType1, TTType2,
                    IsReturn, TTType3, Tax3Amount, RemainQty, SalesRep,
                    CostPrrice, PaymentM, ItemClass, ItemType, IsVoid,
                    VoidReson, VoidUser, Comments, Tax1Rate, Tax2Rate,
                    SubValue, Tax3Rate, WHID, CusName, CustomerName,
                    ContactNo, VehicleNo, JobDoneBy, Mileage, IsDirect
                ) VALUES (
                    @InvoiceNo, @CustomerID, @DeliveryNoteNos, @InvoiceDate, @ARAccount,
                    @NoofDistributions, @DistributionNo, @ItemID, @Qty, @Description,
                    @GLAcount, @UnitPrice, @TotalDiscountPercen, @Amount, @TotalDiscountAmount,
                    @Tax1Amount, @Tax2Amount, @GrossTotal, @NetTotal, @CurrentDate,
                    @Time, @Currentuser, @IsExport, @CustomerPO, @UOM,
                    @JobID, @SONO, @Location, @TTType1, @TTType2,
                    @IsReturn, @TTType3, @Tax3Amount, @RemainQty, @SalesRep,
                    @CostPrrice, @PaymentM, @ItemClass, @ItemType, @IsVoid,
                    @VoidReson, @VoidUser, @Comments, @Tax1Rate, @Tax2Rate,
                    @SubValue, @Tax3Rate, @WHID, @CusName, @CustomerName,
                    @ContactNo, @VehicleNo, @JobDoneBy, @Mileage, @IsDirect
                )
            `;

            const insertRequest = this.erpPool.request();
            
            // Use ALL original values except CurrentDate, Time, and IsVoid
            insertRequest.input('InvoiceNo', sql.VarChar, original.InvoiceNo);
            insertRequest.input('CustomerID', sql.VarChar, original.CustomerID);
            insertRequest.input('DeliveryNoteNos', sql.VarChar, original.DeliveryNoteNos);
            insertRequest.input('InvoiceDate', sql.DateTime, original.InvoiceDate);
            insertRequest.input('ARAccount', sql.VarChar, original.ARAccount);
            insertRequest.input('NoofDistributions', sql.Float, original.NoofDistributions);
            insertRequest.input('DistributionNo', sql.Float, original.DistributionNo);
            insertRequest.input('ItemID', sql.VarChar, original.ItemID); // ✅ Keep original
            insertRequest.input('Qty', sql.Float, original.Qty); // ✅ Keep original
            insertRequest.input('Description', sql.VarChar, original.Description); // ✅ Keep original
            insertRequest.input('GLAcount', sql.VarChar, original.GLAcount);
            insertRequest.input('UnitPrice', sql.Float, original.UnitPrice); // ✅ Keep original
            insertRequest.input('TotalDiscountPercen', sql.Float, original.TotalDiscountPercen);
            insertRequest.input('Amount', sql.Float, original.Amount); // ✅ Keep original
            insertRequest.input('TotalDiscountAmount', sql.Float, original.TotalDiscountAmount);
            insertRequest.input('Tax1Amount', sql.Float, original.Tax1Amount);
            insertRequest.input('Tax2Amount', sql.Float, original.Tax2Amount);
            insertRequest.input('GrossTotal', sql.Float, original.GrossTotal);
            insertRequest.input('NetTotal', sql.Float, original.NetTotal);
            insertRequest.input('CurrentDate', sql.DateTime, now); // ✅ NEW - void timestamp
            insertRequest.input('Time', sql.VarChar, voidTime); // ✅ NEW - void time
            insertRequest.input('Currentuser', sql.VarChar, original.Currentuser);
            insertRequest.input('IsExport', sql.Bit, original.IsExport);
            insertRequest.input('CustomerPO', sql.VarChar, original.CustomerPO);
            insertRequest.input('UOM', sql.VarChar, original.UOM);
            insertRequest.input('JobID', sql.VarChar, original.JobID);
            insertRequest.input('SONO', sql.VarChar, original.SONO);
            insertRequest.input('Location', sql.VarChar, original.Location);
            insertRequest.input('TTType1', sql.VarChar, original.TTType1);
            insertRequest.input('TTType2', sql.VarChar, original.TTType2);
            insertRequest.input('IsReturn', sql.Bit, original.IsReturn);
            insertRequest.input('TTType3', sql.VarChar, original.TTType3);
            insertRequest.input('Tax3Amount', sql.Float, original.Tax3Amount);
            insertRequest.input('RemainQty', sql.Float, original.RemainQty);
            insertRequest.input('SalesRep', sql.VarChar, original.SalesRep);
            insertRequest.input('CostPrrice', sql.Float, original.CostPrrice);
            insertRequest.input('PaymentM', sql.VarChar, original.PaymentM);
            insertRequest.input('ItemClass', sql.VarChar, original.ItemClass);
            insertRequest.input('ItemType', sql.VarChar, original.ItemType);
            insertRequest.input('IsVoid', sql.Bit, 1); // ✅ NEW - set to true
            insertRequest.input('VoidReson', sql.VarChar, original.VoidReson); // ✅ Keep original
            insertRequest.input('VoidUser', sql.VarChar, original.VoidUser); // ✅ Keep original
            insertRequest.input('Comments', sql.NVarChar, original.Comments);
            insertRequest.input('Tax1Rate', sql.Float, original.Tax1Rate);
            insertRequest.input('Tax2Rate', sql.Float, original.Tax2Rate);
            insertRequest.input('SubValue', sql.Float, original.SubValue);
            insertRequest.input('Tax3Rate', sql.Float, original.Tax3Rate);
            insertRequest.input('WHID', sql.VarChar, original.WHID);
            insertRequest.input('CusName', sql.VarChar, original.CusName);
            insertRequest.input('CustomerName', sql.VarChar, original.CustomerName);
            insertRequest.input('ContactNo', sql.VarChar, original.ContactNo);
            insertRequest.input('VehicleNo', sql.VarChar, original.VehicleNo);
            insertRequest.input('JobDoneBy', sql.VarChar, original.JobDoneBy);
            insertRequest.input('Mileage', sql.VarChar, original.Mileage);
            insertRequest.input('IsDirect', sql.Bit, 1); // ✅ NEW - set to true for void placeholder

            await insertRequest.query(voidInsertQuery);

            await this.log(`✅ Created void placeholder with ALL original data preserved`);
            await this.log(`   Only changed: CurrentDate, Time, IsVoid=true, IsDirect=true`);

            return {
                success: true,
                rowsDeleted: deleteResult.rowsAffected[0],
                voidRowCreated: true
            };
        } catch (error) {
            await this.log(`❌ Error in delete and void: ${error.message}`);
            throw error;
        }
    }

    // Main function: Process invoice deletion with void placeholder
    async processInvoiceDeletion(invoiceNo, itemId, reason = 'Watched item sold') {
        const transaction = new sql.Transaction(this.erpPool);
        let transactionStarted = false;
        
        try {
            await this.log(`\n${'='.repeat(60)}`);
            await this.log(`🎯 Starting invoice deletion process`);
            await this.log(`📋 Invoice: ${invoiceNo}`);
            await this.log(`📦 Item: ${itemId}`);
            await this.log(`📝 Reason: ${reason}`);
            await this.log(`${'='.repeat(60)}\n`);

            // Step 1: Backup invoice rows (all rows)
            const backup = await this.backupInvoiceRows(invoiceNo, itemId, reason);
            if (!backup.success) {
                throw new Error(`Backup failed: ${backup.reason}`);
            }

            // Step 2: Start transaction for deletion and void creation
            await transaction.begin();
            transactionStarted = true;
            await this.log(`🔄 Transaction started`);

            // Step 3: Delete all rows and create template-based row (NEW METHOD)
            const result = await this.deleteAndCreateFromTemplate(invoiceNo, transaction);
            if (!result.success) {
                throw new Error('Delete and template creation failed');
            }

            // Step 4: Commit transaction
            await transaction.commit();
            await this.log(`✅ Transaction committed`);

            await this.log(`\n${'='.repeat(60)}`);
            await this.log(`✅ SUCCESS! Invoice deletion completed`);
            await this.log(`📊 Summary:`);
            await this.log(`   - Invoice: ${invoiceNo}`);
            await this.log(`   - Rows backed up: ${backup.rowCount}`);
            await this.log(`   - Rows deleted: ${result.rowsDeleted}`);
            await this.log(`   - Template used: ${result.templateUsed}`);
            await this.log(`   - Void row created from template: YES`);
            await this.log(`   - Counter: NOT MODIFIED (no gaps!)`);
            await this.log(`${'='.repeat(60)}\n`);

            return {
                success: true,
                invoiceNo: invoiceNo,
                rowsBackedUp: backup.rowCount,
                rowsDeleted: result.rowsDeleted,
                templateUsed: result.templateUsed,
                voidPlaceholderCreated: result.voidRowCreated
            };

        } catch (error) {
            // Rollback transaction on error
            if (transactionStarted) {
                try {
                    await transaction.rollback();
                    await this.log(`🔙 Transaction rolled back`);
                } catch (e) {
                    await this.log(`⚠️  Transaction rollback failed: ${e.message}`);
                }
            }

            await this.log(`\n${'='.repeat(60)}`);
            await this.log(`❌ FAILED! Invoice deletion error`);
            await this.log(`Error: ${error.message}`);
            await this.log(`${'='.repeat(60)}\n`);

            throw error;
        }
    }

    // Cleanup connections
    async close() {
        try {
            if (this.erpPool) await this.erpPool.close();
            if (this.backupPool) await this.backupPool.close();
            console.log('🔌 Connections closed');
        } catch (error) {
            console.error('Error closing connections:', error.message);
        }
    }
}

// Export for use in other jobs
module.exports = InvoiceDeletionJob;

// Test function (if run directly)
if (require.main === module) {
    async function test() {
        const job = new InvoiceDeletionJob();
        
        try {
            await job.initialize();
            
            // Test with a sample invoice
            // CHANGE THIS to a real invoice number for testing
            const testInvoiceNo = 'INV00061606'; // Use a test invoice
            const testItemId = 'TEST_ITEM_TUBE_LESS_NECK';
            
            console.log('\n⚠️  TEST MODE - Press Ctrl+C to cancel in next 5 seconds...\n');
            await new Promise(resolve => setTimeout(resolve, 5000));
            
            await job.processInvoiceDeletion(testInvoiceNo, testItemId, 'TEST: Manual deletion test');
            
        } catch (error) {
            console.error('Test failed:', error.message);
        } finally {
            await job.close();
        }
    }
    
    test();
}
