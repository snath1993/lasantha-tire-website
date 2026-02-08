using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Interop.PeachwServer;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace UserAutherization
{
    public class Connector
    {
        //............
        clsCommon objclsCommon = new clsCommon();
        public bool withGUID;
        public CompanyInfoList compList;
        public Interop.PeachwServer.Application app;

        public static string ConnectionString;
        public string StrSql;
        public string StrComName;



        public string ItemID = "";
        public string Description = "";
        public string ItemClass = "";
        public double UnitPrice = 0.00;
        public string SalesGLAccount = "";
        public string Categoty = "";//type
        public string UOM = "";
        public double UnitCost = 0.00;
        public double PriceLevel1 = 0.00;
        public double PriceLevel2 = 0.00;
        public double PriceLevel3 = 0.00;
        public double PriceLevel4 = 0.00;
        public double PriceLevel5 = 0.00;
        public double PriceLevel6 = 0.00;
        public double PriceLevel7 = 0.00;
        public double PriceLevel8 = 0.00;
        public double PriceLevel9 = 0.00;
        public double PriceLevel10 = 0.00;
        public string TaxType = "Regular";

        //BOM Import

        public string StrAssemblyID = "";
        public string StrAssemblyDesc = "";
        public Int32 IntNoOfComponent = 0;
        public string StrComponentID = "";
        public Int32 IntCompNo = 0;
        public string StrCompDescription = "";
        public double dblCompQtyNeed = 0.00;
        public Int32 IntRevisionNo = 0;
        public string CostOfSales = string.Empty;
        public string InventoryAcc = string.Empty;


        #region  Open Company
        public Connector()
        {
            try
            {

                if (IsOpenPeachtree() == false)
                {
                     System.Windows.Forms.Application.Exit();
                }

            }
            catch
            {
                app.ExecuteCommand("File|Exit", null);
                MessageBox.Show("Please Stop the Process peachw.exe from Task Manager First");

            }

        }


        #endregion

        #region  Open Company
        public void OpenCompany()
        {

        }
        #endregion

        public Boolean IsOpenPeachtree(DateTime Date)
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                StrComName = app.CurrentCompanyName;

                if (StrComName == string.Empty)
                {
                    MessageBox.Show("This Operation Required Open a Peachtree Company.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    StrSql = "SELECT CompanyName FROM tblCompanyInformation";
                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (StrComName != dt.Rows[0].ItemArray[0].ToString().Trim())
                        {
                            MessageBox.Show("Please Open a Peachtree Company Which is Compatible With Warehouse Module.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        public Boolean IsOpenPeachtree()
        {
            try
            {
                setConnectionString();
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                app = (Interop.PeachwServer.Application)login.GetApplication("Tom Aligood", "3M3336RJP111X7A");
                StrComName = app.CurrentCompanyName;

                if (StrComName == string.Empty)
                {
                    MessageBox.Show("This Operation Required Open a Peachtree Company.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    StrSql = "SELECT CompanyName FROM tblCompanyInformation";
                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (StrComName != dt.Rows[0].ItemArray[0].ToString().Trim())
                        {
                            MessageBox.Show("Please Open a Peachtree Company Which is Compatible With Warehouse Module.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void Connect()
        {
            try
            {

                if (IsOpenPeachtree() == false)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            catch
            {
                app.ExecuteCommand("File|Exit", null);
                MessageBox.Show("Please Stop the Process peachw.exe from Task Manager First");
            }


        }

        public Interop.PeachwServer.Export ImportFromPeactree;
        Interop.PeachwServer.Export exporter;
        Interop.PeachwServer.Import importer;
        Interop.PeachwServer.Import importer2;
        // Interop.PeachwServer.Export exporter;


        public void ImportGeneralJournaFromPeachtreeee()
        {
            //try
            //{

            if (IsOpenPeachtree() == false)
            {
                return;
            }
            //File.Delete(System.Windows.Forms.Application.StartupPath + "\\GenaralJournal.xml");
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjGeneralJournal);
            // exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
            importer.ClearImportFieldList();
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_TransactionDate);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_TransactionReference);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_GLAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_TransactionDescription);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjGeneralJournalField.peachwIEObjGeneralJournalField_TransactionAmount);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\Journal_Entrie.xml");
            importer.Import();
            //}
            //catch (System.Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}

        }

        public void ExportCustomerList()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                Interop.PeachwServer.Import importer;

                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCustomerList);
                importer.ClearImportFieldList();

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerName);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerCategory);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerFax);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerEmail);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField2);

                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

                importer.Import();
                // MessageBox.Show("Customer List has successfully Exported to Peachtree");

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void ExportVendorList()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                Interop.PeachwServer.Import importer;

                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjVendorList);
                importer.ClearImportFieldList();

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorName);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPhone1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorPhone2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorFax);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField3);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField4);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField5);

                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Vendor.xml");
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

                importer.Import();
                // MessageBox.Show("Customer List has successfully Exported to Peachtree");

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void ExportToPeachtreePurchaseOrder(bool SpecialEditOption, string[] POIDandVendorID)
        {     
           try
            {

                if (IsOpenPeachtree() == false)
                {
                    return;
                }

                //string[] TransToDelete = new string[2];
                //TransToDelete[0] = "TTT";
                //TransToDelete[1] = "PO0002";
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseOrderJournal);

                if (SpecialEditOption == true)
                {
                    app.DeleteRecord(PeachBusObjects.pboPurchaseOrderEntry, PeachObjectKey.pboKey_ByVendorIDByNumber, ref POIDandVendorID);
                }

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_VendorId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderClosed);
                // exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_enCustomerSONumber);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_APAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_ItemId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_enUMID);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Amount);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_JobId);
                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\POJournal.xml");
                importer.Import();
            }
            catch (System.Exception e)
            {
                throw e;
            }

        }
        public void ImportFinalSalesInvice()
        {
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            //Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
            //importer.AddToImportFieldList((short) Interop.PeachwServer.PeachwIEObjSalesJournalField.
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipVia);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesFInvice.xml");
            importer.Import();
        }
        public void ImportLabTestData()
        {

            //try
            //{
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);

            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            // importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerName);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine1);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine2);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCity);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToState);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToZip);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);

            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(@"c:\\PBSS\\LabTest.xml");
            importer.Import();
            // MessageBox.Show("Successfully  Saved BHT Details");
            //}PeachwServe
            //catch (System.Exception e)
            //{

            //    MessageBox.Show(e.Message);

            //}


        }
        public void ImportEmployeeMaster()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Employee.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjEmployeeList);
                exporter.ClearExportFieldList();

                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjEmployeeListField.peachwIEObjEmployeeListField_EmployeeID);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjEmployeeListField.peachwIEObjEmployeeListField_EmployeeFirstName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjEmployeeListField.peachwIEObjEmployeeListField_EmployeeName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjEmployeeListField.peachwIEObjEmployeeListField_Category);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Employee.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        //=============================================

        public void ImportScanData()
        {

            //try
            //{
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);

            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            // importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerName);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine1);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine2);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCity);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToState);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToZip);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);

            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(@"c:\\PBSS\\Scan.xml");
            importer.Import();
            //MessageBox.Show("Successfully  Saved BHT Details");
            //}
            //catch (System.Exception e)
            //{

            //    MessageBox.Show(e.Message);

            //}


        }
        //get itemID and Qty OnHand And Cost From Peachtree

        public void Get_Peachtree_Onhand()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PeachQtyOnHand.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_QuantityOnHand);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PeachQtyOnHand.xml");
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void StoreOnhandTble(string ItemId, double QtyOnHand, double UnitCost)
        {
            setConnectionString();
            try
            {

                string ConnString = ConnectionString;
                String S2 = "insert into [tblPeachtreeQtyOnhand]([StrItemID],[dblQtyonhand],[dblUnitCost]) values ('" + ItemId + "','" + QtyOnHand + "','" + UnitCost + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            catch
            {

            }
        }


        //Insert ItemOnhand In SQl Database
        public void Insert_ItemOnhand()
        {
            try
            {
                deleteItemWarehouse();
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PeachQtyOnHand.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Item");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            coa = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList Items = new ArrayList();

            for (int i = 0; i <= aLength - 1; i++)
            {
                string StrItemID = "";
                double dblOnHand = 0.00;
                double dblUnitCost = 0.00;
                node = reader[i];
                try
                {
                    StrItemID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {

                    dblUnitCost = Convert.ToDouble(node.ChildNodes[1].InnerText);
                }
                catch
                {

                }
                try
                {
                    dblOnHand = Convert.ToDouble(node.ChildNodes[2].InnerText);
                }
                catch
                {

                }
                StoreOnhandTble(StrItemID, dblOnHand, dblUnitCost);
            }

        }





        public void FillEmployee(string EmpID, string EmpName, string Type)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblSalesRep(RepCode,RepName,Type) values ('" + EmpID + "', '" + EmpName + "','" + Type + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch
            {
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }
        //==============================================
        public void deleteEmployee()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblSalesRep";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        //=============================================
        public void Insert_Employee()
        {
            try
            {
                deleteEmployee();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Employee.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Employee");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();


            for (int i = 0; i <= aLength - 1; i++)
            {

                string EmployeeID = "";
                string EmpName = "";
                string Category = "";


                node = reader[i];
                try
                {
                    EmployeeID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch { }
                try
                {
                    EmpName = Convert.ToString(node.ChildNodes[1].InnerText);
                }
                catch { }
                try
                {
                    Category = Convert.ToString(node.ChildNodes[2].InnerText);
                }
                catch { }

                FillEmployee(EmployeeID, EmpName, Category);
            }
        }



        public void AssebmlyAdjustmentExport()
        {
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAssembliesJournal);
            importer.ClearImportFieldList();
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_AssemblyItemId);
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_GLAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_QuantityBuilt);
            importer.SetWarnBeforeImportingDuplicates(1);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\AssembyBuild.xml");
            importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
            importer.Import();
        }



        //==============================================Import Item Master File =======================================
        public void IssueAdjustmentExport(string Path)
        {

            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
            importer.ClearImportFieldList();

            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\" + Path);
            importer.Import();

        }

        //====================================================================
        //ImportItemUnitCost
        public void ImportItemUnitCost(string ItemID)
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                exporter.SetFilterValue((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListFilter.peachwIEObjInventoryItemsListFilter_ItemId,
                PeachwIEFilterOperation.peachwIEFilterOperationRange, ItemID, ItemID);

                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);

                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
                exporter.Export();
                fillLastUnitCost();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        //================================================
        public Array coa1;
        public void fillLastUnitCost()
        {
            try
            {
                DeleteLastUnitCost();
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsUnitCost.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Item");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            coa1 = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList Items = new ArrayList();

            for (int i = 0; i <= aLength - 1; i++)
            {

                string ItemID = "";
                double LastUnitCost = 0.00;

                node = reader[i];
                try
                {
                    ItemID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {
                    LastUnitCost = Convert.ToDouble(node.ChildNodes[1].InnerText);
                    //  passLastunitCost();
                }
                catch
                {

                }
                StoreLastUnitCost(ItemID, LastUnitCost);
                //StoreItemWarehouse(ItemID);
            }

        }
        //============================================
        public void DeleteLastUnitCost()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblLastUnitCost";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        //============================================

        public void StoreLastUnitCost(string Item_ID, double LastUnitCost)//, string Inve_Acc, string CostOfSalesAcc, string PartNO)
        {
            setConnectionString();
            try
            {
                string ConnString = ConnectionString;
                String S2 = "insert into tblLastUnitCost(ItemID,LastUnitCost) values ('" + Item_ID + "','" + LastUnitCost + "')";//ItemType
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            catch
            {

            }

        }

        internal void ExportCustomer()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                Interop.PeachwServer.Import importer;

                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCustomerList);
                importer.ClearImportFieldList();

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerName);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerCategory);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField1);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerAccountNumber);

                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\exportCustomerMaster.xml");
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

                importer.Import();
                // MessageBox.Show("Customer List has successfully Exported to Peachtree");

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //=======================================
        public void Import_Receipt_Journal()
        {

            // loging to the Peachtree
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            Interop.PeachwServer.Import importer;
            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);
            //if our fields has currently data remove that data
            importer.ClearImportFieldList();

            //add fields one by  one
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TotalPaidOnInvoices);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);

            importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml");//C:\Copy of Receipts2.xml
            importer.Import();
        }

        public void Export_Receipt_Journal()
        {

            //Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
            //// connnect with the correct journal
            //exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);

            // loging to the Peachtree
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            //Interop.PeachwServer.Import Exportr;
            // connnect with the correct journal
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);
            //if our fields has currently data remove that data
            //Exportr.ClearImportFieldList();

            //add fields one by  one
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TotalPaidOnInvoices);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
            //exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_);
            // exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Prepayment);

            exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml");//C:\Copy of Receipts2.xml
            exporter.Export();
        }
        //===============================================
        public void IssueAdjustmentExportGetFromP()
        {
            //try
            //{
            //  exporter
            try
            {
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);

                // exporter = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
                exporter.ClearExportFieldList();

                // exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);

                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
                //importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_InventoryAccountId);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);
                exporter.AddToExportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);


                //importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);
                //importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);

                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueAdjustmentAA.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

                exporter.Export();
            }
            catch { }

        }


        public void ImportUnitsMeasureData()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    return;
                }

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\UOMMaster.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjUnitMeasure);
                //exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                exporter.ClearExportFieldList();

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjUnitMeasureField.peachwIEObjUnitMeasureField_Id);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjUnitMeasureField.peachwIEObjUnitMeasureField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjUnitMeasureField.peachwIEObjUnitMeasureField_ConversionFactor);

                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\UOMMaster.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public void ImportItemMaster()
        {

            if (IsOpenPeachtree() == false)
            {
                // System.Windows.Forms.Application.Exit();
                return;
            }

            //File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml");
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjInventoryItemsList);
            importer.ClearImportFieldList();
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemDescription);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Class);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Inactive);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesDesc);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Category);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InventoryAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InvChgAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitOfMeasure);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice1);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice2);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_QuantityDiscountID);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_TaxTypeName);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_CostMethod);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ReorderPoint);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field1);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field2);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field3);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field4);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field5);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml");
            importer.Import();


        }


        //=====================================================
        public void ImportItem_List()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }

                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Items.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Class);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Inactive);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesDesc);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Category);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InventoryAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InvChgAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitOfMeasure);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_QuantityDiscountID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_TaxTypeName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_CostMethod);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ReorderPoint);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field3);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field4);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Field5);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Items.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        //==================================================================
        public void ImportItem_ListFillWarehouse()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsFillWarehouse.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                // exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Class);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Category);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitOfMeasure);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_QuantityOnHand);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.



                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsFillWarehouse.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        //=========================
        public void ImportSalesOrderListD()//importso for delivcry note
        {
            try
            {

                AccountingPeriods ap = new AccountingPeriods();
                DateTime dt1 = DateTime.Parse("01/04/2008");
                DateTime dt2 = DateTime.Parse("01/04/2015");

                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesOrders);
                exporter.ClearExportFieldList();

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderClosed);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Description);
                //exporter.AddToExportFieldList((short)  Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_UnitPrice);
                //exporter.AddToExportFieldList((short)  Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_JobId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_TaxType);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_JobId);
                exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, dt1, dt2);

                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }




        public void ImportSupplierReturn()
        {

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            //Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);


            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            //importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DiscountAmount);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);
            //   importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAmount);
            //importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceDistNum);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            //importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);


            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);

            // importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);

            //importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceDistNum);

            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierReturn.xml");
            importer.Import();
            // MessageBox.Show("Imported Successfully");
        }

        public void ImportSupplierReturnApply()
        {

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            importer.AddToImportFieldList((short)PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierReturnApply.xml");
            importer.Import();
        }
        //SupplierReturnApply
        public int ImportCustomerInvoice()
        {

            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml");

                importer.Import();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import Customer Invoice Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }


        }

        //==============================================

        public void ImportSalesOrderList()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesOrders);
                exporter.ClearExportFieldList();

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderClosed);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Description);
                //exporter.AddToExportFieldList((short)  Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_UnitPrice);
                //exporter.AddToExportFieldList((short)  Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_JobId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_TaxType);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Amount);
                // exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_JobId);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        //============================


        //import joblist=========================================
        public void ImportJobList()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Jobs.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjJobList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_JobDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjJobListField.peachwIEObjJobListField_UsePhases);

                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Jobs.xml");
                exporter.Export();


            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void CostCodelist()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CostCodeList.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCostCodeList);
                exporter.ClearExportFieldList();

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_CostCodeId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_CostCodeDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCostCodeListField.peachwIEObjCostCodeListField_Inactive);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CostCodeList.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void InsertCostCodeList(SqlConnection con, SqlTransaction Trans)
        {

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CostCodeList.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Cost");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            JOB = Array.CreateInstance(typeof(String), 5, aLength);
            ArrayList Items = new ArrayList();

            for (int i = 0; i < aLength; i++)
            {
                node = reader[i];

                string StrCostID = string.Empty;
                string StrCostDescription = string.Empty;
                Boolean blnCostIsInactive = false;

                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    if (node.ChildNodes[j].Name == "ID")
                    {
                        StrCostID = Convert.ToString(node.ChildNodes[j].InnerText);
                    }
                    else if (node.ChildNodes[j].Name == "Description")
                    {
                        StrCostDescription = Convert.ToString(node.ChildNodes[j].InnerText);
                    }

                    else if (node.ChildNodes[j].Name == "IsInactive")
                    {
                        blnCostIsInactive = Convert.ToBoolean(node.ChildNodes[j].InnerText);
                    }

                }

                if (StrCostID != string.Empty)
                {
                    InsertUpdateCostCode(StrCostID, StrCostDescription, blnCostIsInactive, con, Trans);
                }

            }



        }


        public void InsertUpdateCostCode(string StrCostID, string StrCostDescription, Boolean blnCostIsInactive, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intCostIsInactive = (blnCostIsInactive == true ? 1 : 0);


                StrSql = " SELECT [CostCode] FROM tblCostCode where [CostCode]='" + StrCostID + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrSql = "UPDATE tblCostCode set CostCodeDescription='" + StrCostDescription + "',CostIsInactive=" + intCostIsInactive + " where CostCode='" + StrCostID + "'";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                else
                {
                    StrSql = "INSERT INTO tblCostCode (CostCode,CostCodeDescription,CostIsInactive) VALUES('" + StrCostID + "','" + StrCostDescription + "'," + intCostIsInactive + ")";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

            catch (Exception)
            {

                throw;
            }

        }


        public void PhaseList()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phase.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPhaseList);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_PhaseHasCost);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_NumFields);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPhaseListField.peachwIEObjPhaseListField_Inactive);


                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phase.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        //===============================================================

        public void deleteJob()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblJobMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        //insert job list
        public Array JOB;
        public bool InsertJoBData(SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Jobs.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Job");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                JOB = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();



                for (int i = 0; i <= aLength - 1; i++)
                {

                    string CustomerID = string.Empty;
                    string JobID = string.Empty;
                    string JobDescription = string.Empty;
                    Boolean blnPhase = false;
                    node = reader[i];
                    try
                    {
                        JobID = Convert.ToString(node.ChildNodes[0].InnerText);

                    }
                    catch
                    {
                    }
                    try
                    {
                        JobDescription = Convert.ToString(node.ChildNodes[1].InnerText);
                    }
                    catch
                    {
                    }


                    try
                    {
                        blnPhase = Convert.ToBoolean(node.ChildNodes[2].InnerText);
                    }
                    catch
                    {

                    }
                    try
                    {
                        CustomerID = Convert.ToString(node.ChildNodes[3].InnerText);
                    }
                    catch
                    {

                    }

                    if (CustomerID.Trim().Length == 0)
                    {
                        MessageBox.Show("Customer Should Be Link with Job :" + JobID, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    StrSql = "select JobID from tblJobMaster where  JobID='" + JobID + "'";
                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {

                        StrSql = "Update tblJobMaster set JobDescription  = '" + JobDescription + "', CustomerID = '" + CustomerID + "',IsPhase=" + (blnPhase == true ? 1 : 0) + " where JobID='" + JobID + "'";
                        command = new SqlCommand(StrSql, con, Trans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        StoreJob(CustomerID, JobID, JobDescription, blnPhase, con, Trans);
                    }

                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void InsertPhase(SqlConnection con, SqlTransaction Trans)
        {

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Phase.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Phase");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            JOB = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList Items = new ArrayList();

            for (int i = 0; i < aLength; i++)
            {
                node = reader[i];


                string StrPhseID = string.Empty;
                string StrPhaseDescription = string.Empty;
                string JobDescription = string.Empty;
                Boolean blnHascode = false;
                Boolean blnPhaseIsInactive = false;


                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    if (node.ChildNodes[j].Name == "ID")
                    {
                        StrPhseID = Convert.ToString(node.ChildNodes[j].InnerText);
                    }
                    else if (node.ChildNodes[j].Name == "Description")
                    {
                        StrPhaseDescription = Convert.ToString(node.ChildNodes[j].InnerText);
                    }
                    else if (node.ChildNodes[j].Name == "UseCostCodes")
                    {
                        blnHascode = true;
                    }
                    else if (node.ChildNodes[j].Name == "isInactive")
                    {
                        blnPhaseIsInactive = Convert.ToBoolean(node.ChildNodes[j].InnerText);
                    }

                }

                if (StrPhseID != string.Empty)
                {
                    InsertUpdatePhaseCode(StrPhseID, StrPhaseDescription, blnHascode, blnPhaseIsInactive, con, Trans);
                }

            }



        }

        public void InsertUpdatePhaseCode(string StrPhaseID, string StrPhasedescription, Boolean blnHasCostCodes, Boolean blnPhaseIsInactive, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intHasPhaseCode = (blnHasCostCodes == true ? 1 : 0);
                int intPhaseIsInactive = (blnPhaseIsInactive == true ? 1 : 0);

                StrSql = " SELECT [PhaseID] FROM tblPhase where [PhaseID]='" + StrPhaseID + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrSql = "UPDATE tblPhase set PhaseDescription='" + StrPhasedescription + "',HasCostCodes=" + intHasPhaseCode + ",PhaseIsInactive=" + intPhaseIsInactive + " where PhaseID='" + StrPhaseID + "'";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                else
                {
                    StrSql = "INSERT INTO tblPhase (PhaseID,PhaseDescription,HasCostCodes,PhaseIsInactive) VALUES('" + StrPhaseID + "','" + StrPhasedescription + "'," + intHasPhaseCode + "," + intPhaseIsInactive + ")";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

            catch (Exception)
            {

                throw;
            }

        }

        public void StoreJob(string CusID, string JobID, string JobDescription, Boolean blnPhase, SqlConnection con, SqlTransaction Trans)
        {

            try
            {
                int intPhase = (blnPhase == true ? 1 : 0);
                StrSql = "INSERT INTO tblJobMaster(CustomerID,JobID,JobDescription,IsPhase) VALUES ('" + CusID + "','" + JobID + "','" + JobDescription + "'," + intPhase + ")";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }

        }


        public void ImportChartofAccounts()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Acc.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjChartOfAccounts);
                //exporter = ( Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjChartOfAccounts)
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjChartOfAccountsField.peachwIEObjChartOfAccountsField_GeneralLedgerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjChartOfAccountsField.peachwIEObjChartOfAccountsField_GeneralLedgerDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjChartOfAccountsField.peachwIEObjChartOfAccountsField_Type);
                // exporter.AddToExportFieldList((short) Interop.PeachwServer.PeachwIEObjChartOfAccountsField.peachwIEObjChartOfAccountsField_Inactive);
                // exporter.AddToExportFieldList((short) Interop.PeachwServer.PeachwIEObjChartOfAccountsField.peachwIEObjChartOfAccountsField_TaxCode);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Acc.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        //==========================================================

        public void fillchartofAcc()
        {
            try
            {
                deleteChartofAccounts();
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
            //  setConnectionString();
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Acc.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Account");
            XmlNode node = reader[0];
            int aLength = reader.Count;

            coa = Array.CreateInstance(typeof(String), 4, aLength);

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                for (int i = 0; i <= aLength - 1; i++)
                {
                    try
                    {
                        node = reader[i];
                        StoreChartofAccount(node.ChildNodes[0].InnerText, node.ChildNodes[1].InnerText, node.ChildNodes[2].InnerText, myTrans, myConnection);

                    }
                    catch { }
                }

                myTrans.Commit();


            }
            catch (Exception ex)
            {

                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                throw;
            }









        }

        //====================================================================

        //=========================Import Purchase gernal=======================

        public void Export_GRN()
        {
            try
            {
                File.Delete("C:\\GRNList.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseJournal);
                exporter.ClearExportFieldList();
                //exporter.SetIncludeHeadersFlag(1);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DropShip);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enCustomerSONumber);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_WaitingOnBill);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DateDue);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DiscountDate);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DiscountAmount);


                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enCustomerSONumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ShipVia);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NotePrintsAfterLineItem);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_BeginningBalanceTransaction);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceDistNum);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enStockingQuantity);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMID);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMStockingUnits);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMStockingUnitPrice);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UPCSKU);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_TransactionPeriod);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_TransactionNumber);

                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                // exporter.SetFilename("D:\\PURCHASE.CSV");
                exporter.SetFilename("C:\\GRNList.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        //========================================================================
        //================================UPLOAD INVOICE================================
        public void ImportMSalesInv()
        {

            try
            {

                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                // Interop.PeachwServer.Import importer;

                // importer.ClearImportFieldList();
                //importer.SetFileIncludesHeadersFlag(1);


                // connnect with the correct journal
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);

                importer.ClearImportFieldList();
                //importer.SetFileIncludesHeadersFlag(1);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);




                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);

                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                ////importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);


                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml");

                //importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeCSV);
                //importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SALES.csv");

                importer.Import();
                //MessageBox.Show("Successfully Saved",");
                MessageBox.Show("Successfully Saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        //========================================================================

        //==================Upload GRN to Peachtree==========================
        public void Upload_GRN()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                //Interop.PeachwServer.Import importer;
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
                importer.ClearImportFieldList();
                // importer.SetFileIncludesHeadersFlag(1);
                //==========================================================================

                // importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DropShip);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enCustomerSONumber);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_WaitingOnBill);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DateDue);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DiscountDate);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DiscountAmount);


                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enCustomerSONumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ShipVia);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NotePrintsAfterLineItem);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_BeginningBalanceTransaction);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceDistNum);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enStockingQuantity);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMID);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMStockingUnits);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enUMStockingUnitPrice);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UPCSKU);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_TransactionPeriod);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_TransactionNumber);

                //===============================================================================



                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //importer.SetFilename("D:\\PURCHASE.CSV");
                // importer.SetFilename("c:\\GRNList.xml");
                //GRNExport.xml
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\GRNExport.xml");


                //GRNList.xml

                importer.Import();
                MessageBox.Show("Successfully Saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //====================================================================



        //===========Upload Sales Order Into Peachtree======================

        public void Upload_Sales_order()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                // Interop.PeachwServer.Import importer;
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesOrders);
                importer.ClearImportFieldList();


                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerPurchaseOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_NumberOfDistributions);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Amount);


                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");

                importer.Import();
                MessageBox.Show("Successfully Saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        //==================================================================
        //export inventory adjustment journal in to peachtree==============================
        public void InventoryAdjustmentExport()
        {
            //try
            //{
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
            importer.ClearImportFieldList();

            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_InventoryAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);

            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustmentGRN.xml");
            importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

            importer.Import();
            // MessageBox.Show("Successfully Exported Inventory adjustment to Peachtree");
            // MessageBox.Show("Transaction saved Successfully");
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }
        //==================================================================================
        public void InventoryAdjustmentExportOPB()
        {
            //try
            //{
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
            importer.ClearImportFieldList();

            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_InventoryAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);
            importer.AddToImportFieldList((short)PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);

            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustment.xml");
            importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);

            importer.Import();
            // MessageBox.Show("Successfully Exported Inventory adjustment to Peachtree");
            // MessageBox.Show("Transaction saved Successfully");
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }

        //=====================Import Purchase Order===========================


        public void ImportPurchaseOrderList()
        {
            try
            {

                AccountingPeriods ap = new AccountingPeriods();
                DateTime dt1 = DateTime.Parse("01/04/2018");
                DateTime dt2 = DateTime.Parse("01/04/2020");

                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseOrder.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseOrderJournal);
                exporter.ClearExportFieldList();

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_VendorId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderClosed);
                // exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_enCustomerSONumber);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_APAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_PurchaseOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_UnitPrice);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_Amount);
                exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, dt1, dt2);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseOrderJournalField.peachwIEObjPurchaseOrderJournalField_JobId);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseOrder.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                throw e;
            }

        }

        //=====================================================================================================
        //Import Vemdor Mater file=========================================================


        //import customer master as a csv file ==================


        public void ImportCustomer_MasterCSV()
        {
            try
            {
                File.Delete("c:\\PBSS\\Customer.CSV");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCustomerList);
                // exporter.ClearExportFieldList();
                // exporter.SetIncludeHeadersFlag(1);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerBillToAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerBillToAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerFax);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerEmail);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField3);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField4);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField5);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerDueDays);


                exporter.SetFilename(@"c:\\PBSS\\Customer.CSV");
                exporter.SetIncludeHeadersFlag(1);
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeCSV);
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        //====================================================
        //insert Customer Master as CSV ============================


        public void Insert_CustomerCSV()
        {
            try
            {
                deleteCustomer();
            }
            catch { }

            try
            {
                String S1 = "BULK INSERT tblCustomerMaster FROM 'c:\\PBSS\\Customer.csv' WITH(FIELDTERMINATOR = ',', ROWTERMINATOR = '\n')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        //=========================================================


        //=============================Import sales invoice aplly to sales ordeerws============

        public void ImportCustomerInvoicesApply()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvoiceApply.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
                exporter.ClearExportFieldList();

                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvoiceApply.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();
                MessageBox.Show("Invoice imported");

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        //===========================================================================================
        public void ImportBOMMaster()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOMMaster.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjBillOfMaterials);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_AssemblyID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_NumberOfComponents);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_ComponentNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_ComponentID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_ComponentQtyNeeded);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_ComponentDescription);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjBillOfMaterialsField.peachwIEObjBillOfMaterialsField_RevisionNumber);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOMMaster.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void ImportComponentMaster()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ComponentMaster.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_NumComp);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ComponentNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ComponentID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_QuantityNeeded);
                exporter.SetFilterValue((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListFilter.peachwIEObjInventoryItemsListFilter_ItemClass,
                PeachwIEFilterOperation.peachwIEFilterOperationEqualTo, "Assembly", "Assembly");
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ComponentMaster.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        public void AssemblyJournal()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Assembly.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjAssembliesJournal);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_AssemblyItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAssembliesJournalField.peachwIEObjAssembliesJournalField_ComponentId);
                exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Assembly.xml");
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }





        public void ImportCustomer_Master()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCustomerList);
                exporter.ClearExportFieldList();
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerBillToAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerBillToAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerShipTo1AddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerCategory);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPhone2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerFax);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerEmail);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField3);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField4);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerField5);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerDueDays);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerPriceLevel);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public void ExportInventoryAdjustment()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustment.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjAdjustmentsJournal);
                exporter.ClearExportFieldList();
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_TransactionNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_AmountAdjusted);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_InventoryAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);

                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustment.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void ExportPurchaseJournal()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseJournal.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseJournal);
                exporter.ClearExportFieldList();
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_IsCreditMemo);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_TransactionNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseJournal.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public void ExportSalesOrderJournal()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SO.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesOrders);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Date);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ShipByDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderClosed);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesRepId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_UnitPrice);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Amount);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SO.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                exporter.Export();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public void ExportSalesJournal()
        {
            try
            {
                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToInvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToInvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DisplayedTerms);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_QuoteNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesTaxCode);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipByDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCountry);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToState);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToZip);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipVia);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_StatementNote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TransactionNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TransactionPeriod);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Weight);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public void GET_Receipt_JournalOnline(string FilePath, DateTime FromDate, DateTime ToDate)
        {

            string FileName = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + "ReceiptsJournal.csv";

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            AccountingPeriods ap = new AccountingPeriods();

            DateTime dt1 = FromDate;
            DateTime dt2 = ToDate;

            //DateTime dt1 = DateTime.Parse(System.DateTime.Now.ToShortDateString());
            //DateTime dt2 = DateTime.Parse(System.DateTime.Now.ToShortDateString());

            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
            //exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Quantity);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ItemId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Description);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TaxType);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
            // exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TransactionNumber);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
            exporter.AddToExportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TotalPaidOnInvoices);
            //exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, ap.getFirstOpenDay(), ap.getLastOpenDay());
            exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, dt1, dt2);
            exporter.SetIncludeHeadersFlag(1);
            exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeCSV);
            exporter.SetFilename(FilePath + "\\" + FileName);//C:\Copy of Receipts2.xml
            exporter.Export();
        }
        public void ExportSalesJournal_By_Period(DateTime FromDate, DateTime ToDate)
        {
            try
            {

                if (IsOpenPeachtree() == false)
                {
                    // System.Windows.Forms.Application.Exit();
                    return;
                }

                DateTime dt1 = FromDate;
                DateTime dt2 = ToDate;


                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");

                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToInvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToInvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);

                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountAmount);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DiscountDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DisplayedTerms);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InventoryAccountId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_QuoteNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesTaxCode);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipByDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipDate);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCity);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToCountry);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToState);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipToZip);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipVia);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_StatementNote);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TransactionNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TransactionPeriod);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Weight);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);

                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.
                exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, dt1, dt2);
                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                //exporter.SetFileType(PeachwIEFileType.
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        //public void ExportInventoryAdjustment()
        //{
        //    try
        //    {
        //        if (IsOpenPeachtree() == false)
        //        {
        //            // System.Windows.Forms.Application.Exit();
        //            return;
        //        }
        //        File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustment.xml");

        //        exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPU);
        //        exporter.ClearExportFieldList();
        //        exporter.ClearExportFieldList();
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_TransactionNumber);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ItemId);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Quantity);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_SerialNumber);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Date);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_UnitCost);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Reference);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_Amount);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_AmountAdjusted);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_GLAccountId);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_InventoryAccountId);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_JobId);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_NumberOfDistributions);
        //        exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjAdjustmentsJournalField.peachwIEObjAdjustmentsJournalField_ReasonToAdjust);

        //        exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustment.xml");
        //        exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
        //        //exporter.SetFileType(PeachwIEFileType.
        //        exporter.Export();

        //    }
        //    catch (System.Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }

        //}

        //================================Import Customer Master========================

        public void ImportVendor_Master()
        {
            try
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Vendor.xml");
                exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjVendorList);
                exporter.ClearExportFieldList();

                exporter.ClearExportFieldList();
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorId);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorName);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorContact);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine1);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorAddressLine2);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCategory);
                exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorField1);

                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCity);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorState);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorZip);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorCountry);
                //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorOurAccountId);
                //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjVendorListField.peachwIEObjVendorListField_VendorUsualPurch);


                exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Vendor.xml");
                exporter.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                exporter.Export();

            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        //======================================================================================


        public Array SalesOrder;
        public void InsertSOD()//insert sales order for delivery
        {
            setConnectionString();
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_SalesOrder");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            SalesOrder = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList SalesOrderList = new ArrayList();

            int NoofDis1 = 0;

            for (int i = 0; i <= aLength - 1; i++)
            {
                int SoLink = 0;
                string SalesOrderNo = "";
                string CustomerID = "";
                string Date = "";
                bool IsSoClosed = false;
                string ARAccount = "";
                string NoofDis = "";
                string DisNumber = "";
                string ItemID = "";
                string Description = "";
                string GLAccount = "";
                double UnitPrice = 0.0;
                double Amount = 0.0;
                bool isfullDispatch = false;
                string CustomerPO = "";
                string UOM = "";
                string JobID = "";

                SoLink = i + 1;
                node = reader[i];

                //index of the sales order table
                //here start to read XML file
                try
                {
                    //read the customer iD
                    CustomerID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {
                    //Read the Salesorder NO
                    SalesOrderNo = Convert.ToString(node.ChildNodes[1].InnerText);//SalesOrderNo
                }
                catch
                {

                }

                try
                {
                    //Read the Date
                    Date = Convert.ToString(node.ChildNodes[2].InnerText);
                }
                catch
                {

                }
                try
                {
                    //Read the Wether SO Closed or Not
                    IsSoClosed = Convert.ToBoolean(node.ChildNodes[3].InnerText);
                }
                catch
                {

                }
                //=====================

                try
                {
                    //Read the CustomerPO


                    if (node.ChildNodes[4].Name != "Customer_PO")
                    {
                        CustomerPO = "";
                    }
                    else
                    {
                        CustomerPO = Convert.ToString(node.ChildNodes[4].InnerText);
                    }
                    // CustomerPO = Convert.ToString(node.ChildNodes[4].InnerText);
                    // IsSoClosed = Convert.ToBoolean(node.ChildNodes[3].InnerText);
                }
                catch
                {

                }

                //============================================
                try
                {
                    //Read the AR Account


                    if (node.ChildNodes[4].Name != "Customer_PO")
                    {
                        ARAccount = Convert.ToString(node.ChildNodes[4].InnerText);
                    }
                    else
                    {
                        ARAccount = Convert.ToString(node.ChildNodes[5].InnerText);

                    }
                    // ARAccount = Convert.ToString(node.ChildNodes[5].InnerText);
                }
                catch
                {

                }

                try
                {
                    //GEyt the No OF Distribution Per Sales Order
                    if (node.ChildNodes[4].Name != "Customer_PO")
                    {
                        NoofDis = Convert.ToString(node.ChildNodes[5].InnerText);
                        NoofDis1 = Convert.ToInt32(NoofDis);
                    }
                    else
                    {
                        NoofDis = Convert.ToString(node.ChildNodes[6].InnerText);
                        NoofDis1 = Convert.ToInt32(NoofDis);
                    }
                }
                catch
                {

                }
                //================================================================================================
                //Then Read the XMl Lines
                for (int j = 0; j < NoofDis1; j++)
                {


                    // double Quantity = 0.0;
                    // double RemainQty = 0.0;
                    //double DipachQty = 0.0;
                    // bool IsRefillDispatch = false;

                    double Quantity1 = 0.0;
                    double RemainQty1 = 0.0;
                    double Quantity2 = 0.0;
                    double RemainQty2 = 0.0;//kjkjkf

                    double DipachQty1 = 0.0;//sotemp
                    // bool IsRefillDispatch1 = false;



                    try
                    {
                        //Check wehter Customer PO has enterd or not
                        if (node.ChildNodes[4].Name != "Customer_PO")
                        {
                            //here customer po is not there
                            // SalesOrder.SetValue(node.ChildNodes[5].InnerText, 5, i);
                            //SalesOrderList.Add(node.ChildNodes[5].InnerText);
                            DisNumber = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[0].InnerText);
                        }
                        else
                        {
                            //Here customer PO exisis
                            DisNumber = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[0].InnerText);
                        }
                    }
                    catch
                    {

                    }


                    try
                    {
                        if (node.ChildNodes[4].Name != "Customer_PO")
                        {
                            //Customer PO get Blank
                            // SalesOrder.SetValue(node.ChildNodes[3].InnerText, 3, i);
                            // SalesOrderList.Add(node.ChildNodes[3].InnerText);
                            // Quantity = Convert.ToDouble(node.ChildNodes[5].ChildNodes[j].ChildNodes[1].InnerText);
                            Quantity1 = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[1].InnerText);//sotemp
                        }
                        else
                        {
                            //Customer PO Is there
                            Quantity1 = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[1].InnerText);//sotemp

                        }
                    }
                    catch
                    {

                    }
                    if (node.ChildNodes[4].Name != "Customer_PO")//Check Customer po is there
                    {
                        //here customer po is blank for this sales order
                        //then check item field is there
                        if (node.ChildNodes[6].ChildNodes[j].ChildNodes[2].Name != "Item_ID")
                        {
                            //cutomerpo blank
                            //here item field is blank for this sales order
                            //if item filed is blank exixting Quantity is going to be check with Description
                            try
                            {
                                //String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and Description ='" + node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText.ToString().Trim() + "')";
                                String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and Description ='" + node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText.ToString().Trim() + "'and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd2 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                DataTable dt2 = new DataTable();
                                da2.Fill(dt2);
                                if (dt2.Rows.Count > 0)
                                {

                                    for (int k = 0; k < dt2.Rows.Count; k++)
                                    {
                                        // Quantity = Quantity; // +Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                                        DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty1 = Quantity1 - DipachQty1;
                                        //To close so quantity
                                        Quantity2 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty2 = Quantity1 - DipachQty1;
                                        // RemainQty = Quantity - Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                                    }
                                }
                                else
                                {
                                    DipachQty1 = 0;
                                    RemainQty1 = Quantity1;

                                    Quantity2 = 0;
                                    RemainQty2 = Quantity1;

                                }
                            }
                            catch { }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[6].InnerText, 6, i);
                                // SalesOrderList.Add(node.ChildNodes[6].InnerText);
                                Description = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                // UOM = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[6].InnerText);
                                UOM = "";
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[7].InnerText, 7, i);
                                // SalesOrderList.Add(node.ChildNodes[7].InnerText);
                                GLAccount = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[8].InnerText, 8, i);
                                // SalesOrderList.Add(node.ChildNodes[8].InnerText);
                                UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[6].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                JobID = "";
                            }
                            catch
                            {

                            }
                            if (IsSoClosed == true)
                            {
                                DipachQty1 = RemainQty2;
                                RemainQty1 = Quantity2;
                            }


                        }
                        else
                        {
                            //here Item is there for paricular sales order and
                            //customer po is Blank

                            //}
                            try
                            {
                                // Customer.SetValue(node.ChildNodes[5].InnerText, 5, i);
                                // SalesOrderList.Add(node.ChildNodes[5].InnerText);
                                //if (node.ChildNodes[7].ChildNodes[j].ChildNodes[2].Name != "Item_ID")
                                //{


                                //}
                                //else
                                //{



                                //}
                                ItemID = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText);

                                // setConnectionString();

                                //=========================sotemp table
                                //String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "')";
                                String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "' and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd2 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                DataTable dt2 = new DataTable();
                                da2.Fill(dt2);
                                if (dt2.Rows.Count > 0)
                                {

                                    for (int k = 0; k < dt2.Rows.Count; k++)
                                    {
                                        // Quantity = Quantity; // +Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                                        DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty1 = Quantity1 - DipachQty1;
                                        //To close so quantity
                                        Quantity2 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty2 = Quantity1 - DipachQty1;
                                        // RemainQty = Quantity - Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                                    }
                                }
                                else
                                {
                                    DipachQty1 = 0;
                                    RemainQty1 = Quantity1;

                                    Quantity2 = 0;
                                    RemainQty2 = Quantity1;

                                }

                                //======================================


                            }
                            catch
                            {

                            }

                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[3].Name != "SO_Description")
                            {
                                //here Customer field is blank
                                //item field is there
                                //Item description is blank
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[6].InnerText, 6, i);
                                    // SalesOrderList.Add(node.ChildNodes[6].InnerText);
                                    Description = "";
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    UOM = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[6].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[7].InnerText, 7, i);
                                    // SalesOrderList.Add(node.ChildNodes[7].InnerText);
                                    GLAccount = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[8].InnerText, 8, i);
                                    // SalesOrderList.Add(node.ChildNodes[8].InnerText);
                                    UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[7].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    JobID = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[8].InnerText);
                                }
                                catch
                                {

                                }
                                if (IsSoClosed == true)
                                {
                                    DipachQty1 = RemainQty2;
                                    RemainQty1 = Quantity2;
                                }
                            }
                            else
                            {
                                //here Customer field is blank
                                //item field is there
                                //Item description is there
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[6].InnerText, 6, i);
                                    // SalesOrderList.Add(node.ChildNodes[6].InnerText);
                                    Description = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    UOM = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[7].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[7].InnerText, 7, i);
                                    // SalesOrderList.Add(node.ChildNodes[7].InnerText);
                                    GLAccount = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    // SalesOrder.SetValue(node.ChildNodes[8].InnerText, 8, i);
                                    // SalesOrderList.Add(node.ChildNodes[8].InnerText);
                                    UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[5].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[8].InnerText);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                    // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                    JobID = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[9].InnerText);
                                }
                                catch
                                {

                                }
                                if (IsSoClosed == true)
                                {
                                    DipachQty1 = RemainQty2;
                                    RemainQty1 = Quantity2;
                                }
                            }
                        }
                    }//end of the customer po check
                    else
                    {
                        //here customer po is there
                        //}
                        if (node.ChildNodes[7].ChildNodes[j].ChildNodes[2].Name != "Item_ID")
                        {
                            //customer po is there
                            //Item field is blank
                            try
                            {
                                //String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and Description ='" + node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText.ToString().Trim() + "')";
                                String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and Description ='" + node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText.ToString().Trim() + "' and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd2 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                DataTable dt2 = new DataTable();
                                da2.Fill(dt2);
                                if (dt2.Rows.Count > 0)
                                {

                                    for (int k = 0; k < dt2.Rows.Count; k++)
                                    {
                                        // Quantity = Quantity; // +Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                                        DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty1 = Quantity1 - DipachQty1;
                                        //To close so quantity
                                        Quantity2 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty2 = Quantity1 - DipachQty1;
                                        // RemainQty = Quantity - Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                                    }
                                }
                                else
                                {
                                    DipachQty1 = 0;
                                    RemainQty1 = Quantity1;

                                    Quantity2 = 0;
                                    RemainQty2 = Quantity1;

                                }
                            }
                            catch { }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[6].InnerText, 6, i);
                                // SalesOrderList.Add(node.ChildNodes[6].InnerText);
                                Description = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                // UOM = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[6].InnerText);
                                UOM = "";
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[7].InnerText, 7, i);
                                // SalesOrderList.Add(node.ChildNodes[7].InnerText);
                                GLAccount = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[3].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[8].InnerText, 8, i);
                                // SalesOrderList.Add(node.ChildNodes[8].InnerText);
                                UnitPrice = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[4].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                Amount = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[6].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                JobID = "";
                            }
                            catch
                            {

                            }
                            if (IsSoClosed == true)
                            {
                                DipachQty1 = RemainQty2;
                                RemainQty1 = Quantity2;
                            }


                        }
                        else
                        {
                            //herecustomer filed is there 
                            // item fiels is there
                            //}
                            try
                            {
                                // Customer.SetValue(node.ChildNodes[5].InnerText, 5, i);
                                // SalesOrderList.Add(node.ChildNodes[5].InnerText);
                                //if (node.ChildNodes[7].ChildNodes[j].ChildNodes[2].Name != "Item_ID")
                                //{


                                //}
                                //else
                                //{



                                //}
                                ItemID = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText);

                                // setConnectionString();

                                //=========================sotemp table

                                //String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "')";
                                String S2 = "select Quantity,DispatchQty from tblSalesOrderTemp where ( SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "' and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd2 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                DataTable dt2 = new DataTable();
                                da2.Fill(dt2);
                                if (dt2.Rows.Count > 0)
                                {

                                    for (int k = 0; k < dt2.Rows.Count; k++)
                                    {
                                        // Quantity = Quantity; // +Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                                        DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty1 = Quantity1 - DipachQty1;
                                        //To close so quantity
                                        Quantity2 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty2 = Quantity1 - DipachQty1;
                                        // RemainQty = Quantity - Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                                    }
                                }
                                else
                                {
                                    DipachQty1 = 0;
                                    RemainQty1 = Quantity1;

                                    Quantity2 = 0;
                                    RemainQty2 = Quantity1;

                                }

                                //======================================


                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[6].InnerText, 6, i);
                                // SalesOrderList.Add(node.ChildNodes[6].InnerText);
                                Description = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[3].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                UOM = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[7].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[7].InnerText, 7, i);
                                // SalesOrderList.Add(node.ChildNodes[7].InnerText);
                                GLAccount = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[4].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                // SalesOrder.SetValue(node.ChildNodes[8].InnerText, 8, i);
                                // SalesOrderList.Add(node.ChildNodes[8].InnerText);
                                UnitPrice = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[5].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                Amount = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[8].InnerText);
                            }
                            catch
                            {

                            }
                            try
                            {
                                //SalesOrder.SetValue(node.ChildNodes[9].InnerText, 9, i);
                                // SalesOrderList.Add(node.ChildNodes[9].InnerText);
                                JobID = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[9].InnerText);
                            }
                            catch
                            {

                            }
                            if (IsSoClosed == true)
                            {
                                DipachQty1 = RemainQty2;
                                RemainQty1 = Quantity2;
                            }
                        }
                    }
                    //============================

                    try
                    {
                        if (ItemID == "" && IsSoClosed == false)
                        {
                            if (SalesOrderNo == "PO215002")
                                MessageBox.Show("dfsdfsdfa");
                            //String S3 = "select SalesOrderNo from tblSalesOrderTemp where  SalesOrderNo='" + SalesOrderNo + "' and Description ='" + Description + "'";
                            String S3 = "select SalesOrderNo from tblSalesOrderTemp where  SalesOrderNo='" + SalesOrderNo + "' and Description ='" + Description + "' and DisNumber='" + DisNumber + "'";
                            SqlCommand cmd3 = new SqlCommand(S3);
                            SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                            DataTable dt3 = new DataTable();
                            da3.Fill(dt3);
                            if (dt3.Rows.Count > 0)
                            {
                                // String S = "Update tblSalesOrderTemp set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSoClosed='" + IsSoClosed + "',CustomerPO='" + CustomerPO + "',IsfullDispatch='" + isfullDispatch + "'  where (SalesOrderNo='" + SalesOrderNo + "' and Description ='" + Description  + "')";
                                //String S = "Update tblSalesOrderTemp set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSoClosed='" + IsSoClosed + "',CustomerPO='" + CustomerPO + "'where (SalesOrderNo='" + SalesOrderNo + "' and Description ='" + Description + "')";
                                String S = "Update tblSalesOrderTemp set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSoClosed='" + IsSoClosed + "',CustomerPO='" + CustomerPO + "'where (SalesOrderNo='" + SalesOrderNo + "' and Description ='" + Description + "' and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                            }
                            else
                            {

                                StoreSalesOreder1(SoLink, SalesOrderNo, CustomerID, Date, ARAccount, NoofDis, DisNumber, Quantity1, ItemID, Description, GLAccount, UnitPrice, Amount, DipachQty1, RemainQty1, isfullDispatch, IsSoClosed, CustomerPO, UOM, JobID);
                            }
                        }
                        else if (IsSoClosed == false)
                        {
                            //String S3 = "select SalesOrderNo from tblSalesOrderTemp where  SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "'";
                            String S3 = "select SalesOrderNo from tblSalesOrderTemp where  SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "' and DisNumber='" + DisNumber + "'";
                            SqlCommand cmd3 = new SqlCommand(S3);
                            SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                            DataTable dt3 = new DataTable();
                            da3.Fill(dt3);
                            if (dt3.Rows.Count > 0)
                            {
                                //String S = "Update tblSalesOrderTemp set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSoClosed='" + IsSoClosed + "',CustomerPO='" + CustomerPO + "' where (SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "')";
                                String S = "Update tblSalesOrderTemp set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSoClosed='" + IsSoClosed + "',CustomerPO='" + CustomerPO + "' where (SalesOrderNo='" + SalesOrderNo + "' and ItemID ='" + ItemID + "' and DisNumber='" + DisNumber + "')";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                            }
                            else
                            {

                                StoreSalesOreder1(SoLink, SalesOrderNo, CustomerID, Date, ARAccount, NoofDis, DisNumber, Quantity1, ItemID, Description, GLAccount, UnitPrice, Amount, DipachQty1, RemainQty1, isfullDispatch, IsSoClosed, CustomerPO, UOM, JobID);
                            }
                        }
                        //============================================


                    }
                    catch { }
                }

            }

        }


        public void StoreSalesOreder1(int SoLink, string SalesOrder_No, string Customer_ID, string Date, string AR_Account, string Noof_Dis, string Dis_Number, double Quantit_y, string Item_ID, string Descripti_on, string GL_Account, double Unit_Price, double A_mount, double DipachQty, double RemainQty, bool isfullDispatch, bool IsClosed, string CusPO, string UOM, string Jobid)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblSalesOrderTemp(SalesOrderLink,SalesOrderNo,CustomerID,Date,ARAccount,NoOfDis,DisNumber,Quantity,ItemID,Description,GLAccount,UnitPrice,Amount,DispatchQty,RemainQty,IsfullDispatch,IsSoClosed,CustomerPO,UOM,JobID) values ('" + SoLink + "','" + SalesOrder_No + "','" + Customer_ID + "','" + Date + "','" + AR_Account + "','" + Noof_Dis + "','" + Dis_Number + "','" + Quantit_y + "','" + Item_ID + "','" + Descripti_on + "','" + GL_Account + "','" + Unit_Price + "','" + A_mount + "','" + DipachQty + "','" + RemainQty + "','" + isfullDispatch + "','" + IsClosed + "','" + CusPO + "','" + UOM + "','" + Jobid + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }
            catch
            {
                myTrans.Rollback();
            }
            myConnection.Close();
            // finally
            // {

            // }


        }

        //=====================================================================================
        public void Insert_SalesOrder()
        {
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrder.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_SalesOrder");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            purhaseorder = Array.CreateInstance(typeof(String), 10, aLength);

            int NoofDis1 = 0;

            for (int i = 0; i <= aLength - 1; i++)
            {
                int SOlink = 0;
                string SONumber = "";
                string CustomerID = "";
                string SODate = "";
                bool IsSOClosed = false;
                string CustomerPO = "";
                string AccountPayID = "";
                string NoOfDistibution = "";
                string SODisNum = "";
                double Quantity = 0;
                string ItemId = "";
                string Description = "";
                string UOM = "";
                string GL_Account = "";
                double UnitPrice = 0;
                double Amount = 0;
                // string JobId = "";
                double DespatchQty = 0;
                double RemainQty = 0;
                bool IsFullInvoice = false;


                // DateTime 
                DateTime Date1 = System.DateTime.Now;
                string Dformat = "MM/dd/yyyy";
                string GRNDate = Date1.ToString(Dformat);

                string CurretDate = GRNDate;
                string CurrentTime = System.DateTime.Now.ToShortTimeString();

                string CurretUser = "";

                SOlink = i + 1;
                node = reader[i];
                // bool ISCusSO = false;

                try
                {
                    CustomerID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {
                    SONumber = Convert.ToString(node.ChildNodes[1].InnerText);//SalesOrderNo
                }
                catch
                {

                }

                try
                {
                    SODate = Convert.ToString(node.ChildNodes[2].InnerText);
                }
                catch
                {

                }
                try
                {
                    IsSOClosed = Convert.ToBoolean(node.ChildNodes[3].InnerText);
                }
                catch
                {

                }
                //=====================

                try
                {
                    CustomerPO = Convert.ToString(node.ChildNodes[4].InnerText);
                }
                catch
                {
                }
                try
                {
                    AccountPayID = Convert.ToString(node.ChildNodes[5].InnerText);

                }
                catch
                {

                }

                try
                {

                    NoOfDistibution = Convert.ToString(node.ChildNodes[6].InnerText);
                    NoofDis1 = Convert.ToInt32(NoOfDistibution);

                }
                catch
                {

                }
                //================================================================================================
                for (int j = 0; j < NoofDis1; j++)
                {

                    double Quantity1 = 0.0;
                    double RemainQty1 = 0.0;
                    double DipachQty1 = 0.0;//sotemp



                    try
                    {
                        SODisNum = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[0].InnerText);
                    }
                    catch
                    {

                    }


                    try
                    {
                        Quantity1 = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[1].InnerText);//sotemp
                    }
                    catch
                    {

                    }
                    try
                    {

                        ItemId = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText);

                        setConnectionString();
                        String S2 = "select Quantity,DespatchQty from tblSalesOrder where ( SONumber='" + SONumber + "' and ItemId ='" + ItemId + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);
                        if (dt2.Rows.Count > 0)
                        {

                            for (int k = 0; k < dt2.Rows.Count; k++)
                            {
                                DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                RemainQty1 = Quantity1 - DipachQty1;
                            }
                        }
                        else
                        {
                            DipachQty1 = 0;
                            RemainQty1 = Quantity1;
                        }
                        //======================================
                    }

                    catch
                    {

                    }

                    try
                    {
                        UOM = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[7].InnerText);
                    }
                    catch
                    {

                    }
                    try
                    {
                        Description = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[3].InnerText);
                    }
                    catch
                    {

                    }
                    try
                    {
                        GL_Account = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[4].InnerText);
                    }
                    catch
                    {

                    }
                    try
                    {
                        UnitPrice = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[5].InnerText);
                    }
                    catch
                    {

                    }
                    try
                    {
                        Amount = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[6].InnerText);
                    }
                    catch
                    {

                    }
                    //try
                    //{
                    //    JobId  = Convert.ToString (node.ChildNodes[7].ChildNodes[j].ChildNodes[7].InnerText);
                    //}
                    //catch
                    //{

                    //}
                    //============================

                    try
                    {
                        //=======================================
                        String S3 = "select SONumber from tblSalesOrder where  SONumber='" + SONumber + "' and ItemId ='" + ItemId + "'";
                        SqlCommand cmd3 = new SqlCommand(S3);
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            String S = "Update tblSalesOrder set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsSOClosed='" + IsSOClosed + "'  where (SONumber='" + SONumber + "' and ItemId ='" + ItemId + "')";

                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                        }
                        else
                        {
                            // StorePurchaseOrder(POLink, POnumber, VendorID, Date, IsPOClosed, CustomerSoNo, AccountPayID, NoOfDistibution, PODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, JobId, DipachQty1, RemainQty1, IsFullGRN, CurretDate, CurrentTime, CurretUser);
                            // StorePurchaseOrder(POLink, POnumber, VendorID, Date, IsPOClosed, CustomerSoNo, AccountPayID, NoOfDistibution, PODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, DipachQty1, RemainQty1, IsFullGRN, CurretDate, CurrentTime, CurretUser);
                            StoreSalesOrder(SOlink, SONumber, CustomerID, SODate, IsSOClosed, CustomerPO, AccountPayID, NoOfDistibution, SODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, DipachQty1, RemainQty1, IsFullInvoice);


                        }
                        //============================================
                    }
                    catch { }
                }

            }

        }

        //=====================================================================================



        public Array purhaseorder;
        public void Insert_PurchaseOrderList()
        {
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseOrder.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_PurchaseOrder");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            purhaseorder = Array.CreateInstance(typeof(String), 10, aLength);

            int NoofDis1 = 0;
            double a = 0;
            string dd = "";
            //frmmsg o = new frmmsg();
            //o.ShowDialog();

            for (int i = 0; i <= aLength - 1; i++)
            {

                int POLink = 0;
                string POnumber = "";
                string VendorID = "";
                string Date = "";
                bool IsPOClosed = false;
                string CustomerSoNo = "";
                string AccountPayID = "";
                string NoOfDistibution = "";
                string PODisNum = "";
                double Quantity = 0;
                string ItemId = "";
                string Description = "";
                string UOM = "";
                string GL_Account = "";
                double UnitPrice = 0;
                double Amount = 0;
                // string JobId = "";
                double GRNQty = 0;
                double RemainQty = 0;
                bool IsFullGRN = false;
                // DateTime 
                DateTime Date1 = System.DateTime.Now;
                string Dformat = "MM/dd/yyyy";
                string GRNDate = Date1.ToString(Dformat);

                string CurretDate = GRNDate;
                string CurrentTime = System.DateTime.Now.ToShortTimeString();

                string CurretUser = "";

                POLink = i + 1;
                node = reader[i];
                bool ISCusSO = false;

                try
                {
                    if (node.ChildNodes[0].Name == "VendorID")
                    {
                        VendorID = Convert.ToString(node.ChildNodes[0].InnerText);
                    }

                }
                catch
                {

                }
                try
                {
                    if (node.ChildNodes[1].Name == "PO_Number")
                    {
                        POnumber = Convert.ToString(node.ChildNodes[1].InnerText);
                        //if (POnumber == "PO0790")
                        //    MessageBox.Show ("");
                    }
                }
                catch
                {

                }

                try
                {
                    if (node.ChildNodes[2].Name == "Date")
                    {
                        Date = Convert.ToString(node.ChildNodes[2].InnerText);
                    }
                }
                catch
                {

                }
                try
                {
                    if (node.ChildNodes[3].Name == "PO_Closed")
                    {
                        IsPOClosed = Convert.ToBoolean(node.ChildNodes[3].InnerText);
                    }
                }
                catch
                {

                }

                try
                {
                    if (node.ChildNodes[4].Name == "AP_Account")
                    {
                        AccountPayID = Convert.ToString(node.ChildNodes[4].InnerText);
                    }

                }
                catch
                {

                }
                try
                {
                    if (node.ChildNodes[5].Name == "Number_of_Distributions")
                    {
                        NoOfDistibution = Convert.ToString(node.ChildNodes[5].InnerText);
                        NoofDis1 = Convert.ToInt32(NoOfDistibution);
                    }

                }
                catch
                {

                }

                for (int j = 0; j < NoofDis1; j++)
                {
                    double Quantity1 = 0.0;
                    double RemainQty1 = 0.0;
                    double DipachQty1 = 0.0;//sotemp

                    try
                    {
                        if (node.ChildNodes[6].ChildNodes[j].ChildNodes[0].Name == "DistributionNumber")
                        {
                            PODisNum = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[0].InnerText);
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (node.ChildNodes[6].ChildNodes[j].ChildNodes[1].Name == "Quantity")
                        {
                            Quantity1 = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[0].InnerText);//sotemp
                        }

                    }
                    catch
                    {

                    }

                    bool isnotItem = false;
                    if (node.ChildNodes[6].ChildNodes[j].ChildNodes[2].Name == "Item_ID")
                    {
                        isnotItem = true;
                        try
                        {

                            ItemId = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText);
                            if (ItemId == null || ItemId == "")
                            {
                                DipachQty1 = 0;
                                RemainQty1 = 0;
                            }
                            else
                            {
                                setConnectionString();
                                String S2 = "select Quantity,GRNQty from tblPurchaseOrder where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and VendorID='" + VendorID + "')";
                                SqlCommand cmd2 = new SqlCommand(S2);
                                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                                DataTable dt2 = new DataTable();
                                da2.Fill(dt2);
                                if (dt2.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dt2.Rows.Count; k++)
                                    {
                                        DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                        RemainQty1 = Quantity1 - DipachQty1;
                                    }
                                }
                                else
                                {
                                    DipachQty1 = 0;
                                    RemainQty1 = Quantity1;
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[3].Name == "Description")
                            {
                                Description = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);//sotemp
                            }

                        }
                        catch
                        {

                        }

                        try
                        {
                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[4].Name == "GL_Account")
                            {

                                GL_Account = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);//sotemp
                            }

                        }
                        catch
                        {

                        }

                        try
                        {
                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[5].Name == "Unit_Price")
                            {
                                UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[5].InnerText);//sotemp
                            }

                        }
                        catch
                        {

                        }

                        try
                        {
                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[6].Name == "Amount")
                            {
                                Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[6].InnerText);//sotemp
                            }

                        }
                        catch
                        {

                        }
                        try
                        {
                            if (node.ChildNodes[6].ChildNodes[j].ChildNodes[7].Name == "UM_ID")
                            {
                                UOM = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[7].InnerText);
                            }

                        }
                        catch
                        {

                        }
                    }


                    try
                    {
                        //if (POnumber == "PO0790")
                        // MessageBox.Show("");

                        if (isnotItem == false)
                        {
                            try
                            {
                                if (node.ChildNodes[6].ChildNodes[j].ChildNodes[2].Name == "Description")
                                {
                                    Description = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText);//sotemp                                    
                                }
                            }
                            catch
                            {

                            }

                            try
                            {
                                if (node.ChildNodes[6].ChildNodes[j].ChildNodes[4].Name == "Unit_Price")
                                {
                                    UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);//sotemp
                                }

                            }
                            catch
                            {

                            }

                            try
                            {
                                if (node.ChildNodes[6].ChildNodes[j].ChildNodes[5].Name == "Amount")
                                {
                                    Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[5].InnerText);//sotemp
                                }

                            }
                            catch
                            {

                            }
                            try
                            {
                                if (node.ChildNodes[6].ChildNodes[j].ChildNodes[3].Name == "GL_Account")
                                {
                                    GL_Account = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);//sotemp
                                }
                            }
                            catch
                            {

                            }
                            try
                            {
                                //if (node.ChildNodes[6].ChildNodes[j].ChildNodes[7].Name == "UM_ID")
                                //{
                                UOM = "<Each>";//Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[6].InnerText);
                                               // }

                            }
                            catch
                            {

                            }

                        }
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (node.ChildNodes[6].ChildNodes[j].ChildNodes[3].Name == "Unit_Price")
                        {
                            UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);//sotemp
                        }

                    }
                    catch
                    {

                    }
                    try
                    {
                        if (node.ChildNodes[6].ChildNodes[j].ChildNodes[4].Name == "Amount")
                        {
                            Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);//sotemp
                        }

                    }
                    catch
                    {

                    }

                    //try
                    //{
                    //    if (node.ChildNodes[6].ChildNodes[j].ChildNodes[4].Name == "UM_ID")
                    //    {
                    //        UOM = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[7].InnerText);
                    //    }

                    //}
                    //catch
                    //{

                    //}
                    try
                    {
                        String S3 = "select PONumber from tblPurchaseOrder where  PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and PODisNum='" + PODisNum + "' and VendorID='" + VendorID + "'";
                        SqlCommand cmd3 = new SqlCommand(S3);
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            String S = "Update tblPurchaseOrder set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsPOClosed='" + IsPOClosed + "'  where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and PODisNum='" + PODisNum + "' and VendorID='" + VendorID + "')";
                            // String S2 = "Update tblGRNMPO set Qty='" + Quantity1 + "' where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "')";

                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            String S2 = "Update tblGRNMPO set Qty='" + Quantity1 + "' where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and VendorID='" + VendorID + "')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                        }
                        else
                        {
                            StorePurchaseOrder(POLink, POnumber, VendorID, Date, IsPOClosed, CustomerSoNo, AccountPayID, NoOfDistibution, PODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, DipachQty1, RemainQty1, IsFullGRN, CurretDate, CurrentTime, CurretUser);
                        }
                        //============================================
                    }
                    catch { }
                }

            }

        }
        public void Insert_OLDPurchaseOrderList()
        {
            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\PurchaseOrder.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_PurchaseOrder");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            purhaseorder = Array.CreateInstance(typeof(String), 10, aLength);

            int NoofDis1 = 0;
            double a = 0;
            string dd = "";
            //frmmsg o = new frmmsg();
            //o.ShowDialog();

            for (int i = 0; i <= aLength - 1; i++)
            {

                int POLink = 0;
                string POnumber = "";
                string VendorID = "";
                string Date = "";
                bool IsPOClosed = false;
                string CustomerSoNo = "";
                string AccountPayID = "";
                string NoOfDistibution = "";
                string PODisNum = "";
                double Quantity = 0;
                string ItemId = "";
                string Description = "";
                string UOM = "";
                string GL_Account = "";
                double UnitPrice = 0;
                double Amount = 0;
                // string JobId = "";
                double GRNQty = 0;
                double RemainQty = 0;
                bool IsFullGRN = false;
                // DateTime 
                DateTime Date1 = System.DateTime.Now;
                string Dformat = "MM/dd/yyyy";
                string GRNDate = Date1.ToString(Dformat);

                string CurretDate = GRNDate;
                string CurrentTime = System.DateTime.Now.ToShortTimeString();

                string CurretUser = "";

                POLink = i + 1;
                node = reader[i];
                bool ISCusSO = false;


                try
                {
                    VendorID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {
                    POnumber = Convert.ToString(node.ChildNodes[1].InnerText);//SalesOrderNo
                }
                catch
                {

                }

                try
                {
                    Date = Convert.ToString(node.ChildNodes[2].InnerText);
                }
                catch
                {

                }
                try
                {
                    IsPOClosed = Convert.ToBoolean(node.ChildNodes[3].InnerText);
                }
                catch
                {

                }
                //=====================

                try
                {
                    if (node.ChildNodes[4].Name != "CustomerSONumber")
                    {
                        CustomerSoNo = "";
                    }
                    else
                    {
                        CustomerSoNo = Convert.ToString(node.ChildNodes[4].InnerText);
                    }
                }
                catch
                {
                }
                try
                {
                    if (node.ChildNodes[4].Name != "CustomerSONumber")
                    {
                        AccountPayID = Convert.ToString(node.ChildNodes[4].InnerText);
                    }
                    else
                    {
                        AccountPayID = Convert.ToString(node.ChildNodes[5].InnerText);

                    }

                }
                catch
                {

                }

                try
                {
                    if (node.ChildNodes[4].Name != "CustomerSONumber")
                    {
                        NoOfDistibution = Convert.ToString(node.ChildNodes[5].InnerText);
                        NoofDis1 = Convert.ToInt32(NoOfDistibution);
                    }
                    else
                    {
                        NoOfDistibution = Convert.ToString(node.ChildNodes[6].InnerText);
                        NoofDis1 = Convert.ToInt32(NoOfDistibution);
                    }

                    //NoOfDistibution = Convert.ToString(node.ChildNodes[6].InnerText);
                    //NoofDis1 = Convert.ToInt32(NoOfDistibution);

                }
                catch
                {

                }
                //================================================================================================
                for (int j = 0; j < NoofDis1; j++)
                {

                    double Quantity1 = 0.0;
                    double RemainQty1 = 0.0;
                    double DipachQty1 = 0.0;//sotemp



                    try
                    {

                        if (node.ChildNodes[4].Name != "CustomerSONumber")
                        {
                            PODisNum = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[0].InnerText);
                        }
                        else
                        {
                            PODisNum = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[0].InnerText);
                        }
                    }
                    catch
                    {

                    }


                    try
                    {
                        if (node.ChildNodes[4].Name != "CustomerSONumber")
                        {
                            Quantity1 = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[1].InnerText);//sotemp
                        }
                        else
                        {
                            Quantity1 = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[1].InnerText);//sotemp
                        }
                    }
                    catch
                    {

                    }


                    if (node.ChildNodes[4].Name != "CustomerSONumber")
                    {

                        try
                        {

                            //if (node.ChildNodes[4].Name != "CustomerSONumber")
                            //{

                            //}
                            //else{
                            ItemId = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[2].InnerText);

                            setConnectionString();
                            String S2 = "select Quantity,GRNQty from tblPurchaseOrder where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and VendorID='" + VendorID + "')";
                            //String S2 = "select Quantity,GRNQty from tblPurchaseOrder where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and PODisNum='" + PODisNum + "')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                            if (dt2.Rows.Count > 0)
                            {

                                for (int k = 0; k < dt2.Rows.Count; k++)
                                {
                                    DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                    RemainQty1 = Quantity1 - DipachQty1;
                                }
                            }
                            else
                            {
                                DipachQty1 = 0;
                                RemainQty1 = Quantity1;
                            }
                            //======================================
                        }

                        catch
                        {

                        }

                        try
                        {
                            UOM = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[7].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            Description = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[3].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            GL_Account = Convert.ToString(node.ChildNodes[6].ChildNodes[j].ChildNodes[4].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            UnitPrice = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[5].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            Amount = Convert.ToDouble(node.ChildNodes[6].ChildNodes[j].ChildNodes[6].InnerText);
                        }
                        catch
                        {

                        }

                    }
                    else
                    {
                        try
                        {

                            //if (node.ChildNodes[4].Name != "CustomerSONumber")
                            //{

                            //}
                            //else{
                            ItemId = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[2].InnerText);

                            setConnectionString();
                            String S2 = "select Quantity,GRNQty from tblPurchaseOrder where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and VendorID='" + VendorID + "')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                            if (dt2.Rows.Count > 0)
                            {

                                for (int k = 0; k < dt2.Rows.Count; k++)
                                {
                                    DipachQty1 = Convert.ToDouble(dt2.Rows[k].ItemArray[1].ToString());
                                    RemainQty1 = Quantity1 - DipachQty1;
                                }
                            }
                            else
                            {
                                DipachQty1 = 0;
                                RemainQty1 = Quantity1;
                            }
                            //======================================

                        }
                        catch
                        {

                        }

                        try
                        {
                            UOM = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[7].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            Description = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[3].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            GL_Account = Convert.ToString(node.ChildNodes[7].ChildNodes[j].ChildNodes[4].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            UnitPrice = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[5].InnerText);
                        }
                        catch
                        {

                        }
                        try
                        {
                            Amount = Convert.ToDouble(node.ChildNodes[7].ChildNodes[j].ChildNodes[6].InnerText);
                        }
                        catch
                        {

                        }
                    }
                    //try
                    //{
                    //    JobId  = Convert.ToString (node.ChildNodes[7].ChildNodes[j].ChildNodes[7].InnerText);
                    //}
                    //catch
                    //{

                    //}
                    //============================

                    try
                    {
                        //if (POnumber == "123")
                        //    MessageBox.Show("dsfsdfsd");
                        //=======================================
                        String S3 = "select PONumber from tblPurchaseOrder where  PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and PODisNum='" + PODisNum + "' and VendorID='" + VendorID + "'";
                        SqlCommand cmd3 = new SqlCommand(S3);
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            String S = "Update tblPurchaseOrder set Quantity  = '" + Quantity1 + "', RemainQty = '" + RemainQty1 + "',IsPOClosed='" + IsPOClosed + "'  where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and PODisNum='" + PODisNum + "' and VendorID='" + VendorID + "')";
                            // String S2 = "Update tblGRNMPO set Qty='" + Quantity1 + "' where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "')";

                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            String S2 = "Update tblGRNMPO set Qty='" + Quantity1 + "' where (PONumber='" + POnumber + "' and ItemId ='" + ItemId + "' and VendorID='" + VendorID + "')";
                            SqlCommand cmd2 = new SqlCommand(S2);
                            SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);



                            //  myCommand.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,PODisNo,ISFull) values ('" + PoNumber + "','" + ItemId + "','" + QTY + "','" + PODisNum + "','" + isfull + "')";
                        }
                        else
                        {
                            // StorePurchaseOrder(POLink, POnumber, VendorID, Date, IsPOClosed, CustomerSoNo, AccountPayID, NoOfDistibution, PODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, JobId, DipachQty1, RemainQty1, IsFullGRN, CurretDate, CurrentTime, CurretUser);
                            StorePurchaseOrder(POLink, POnumber, VendorID, Date, IsPOClosed, CustomerSoNo, AccountPayID, NoOfDistibution, PODisNum, Quantity1, ItemId, UOM, Description, GL_Account, UnitPrice, Amount, DipachQty1, RemainQty1, IsFullGRN, CurretDate, CurrentTime, CurretUser);
                        }
                        //============================================
                    }
                    catch { }
                }

            }

        }


        //=================================================================

        //====================store chartof Accounts=================================

        public void StoreChartofAccount(string AccountID, string AccountDesdription, string Type, SqlTransaction Trans, SqlConnection con)
        {
            //==========================================================


            try
            {

                StrSql = "insert into tblChartofAcounts(AcountID,AccountDescription,AountType) values ('" + AccountID + "','" + AccountDesdription + "','" + Type + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        //=======================================================================
        public void StoreSalesOrder(int SOlink, string SONumber, string CustomerID, string SODate, bool IsSOClosed, string CustomerPO, string AccountPayID, string NoOfDistibution, string SODisNum, double Quantity1, string ItemId, string UOM, string Description, string GL_Account, double UnitPrice, double Amount, double DipachQty1, double RemainQty1, bool IsFullInvoice)
        {
            double QTY = 0;
            bool isfull = true;
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                // myCommand.CommandText = "insert into tblPurchaseOrder(POlink,PONumber,VendorID,Date,IsPOClosed,CustomerSoNo,AccountPayID,NoOfDistibution,PODisNum,Quantity,ItemId,UOM,Description,GL_Account,UnitPrice,Amount,GRNQty,RemainQty,IsFullGRN,CurrentDate,CurrentTime,CurretUser) values ('" + POlink + "','" + PoNumber + "','" + VendorID + "','" + Date + "','" + IsPOClosed + "','" + CusSO + "','" + AccountPayID + "','" + NoOfDistibution + "','" + PODisNum + "','" + Quantity + "','" + ItemId + "','" + UOM + "','" + Description + "','" + GL_Account + "','" + UnitPrice + "','" + Amount + "','" + GRNQty + "','" + RemainQty + "','" + IsFullGRN + "','" + CurrentDate + "','" + CurrentTime + "','" + CurretUser + "')";
                myCommand.CommandText = "insert into tblSalesOrder(SOlink,SONumber,CustomerID,SODate,IsSOClosed,CustomerPO,AccountPayID,NoOfDistibution,SODisNum,Quantity,ItemId,UOM,Description,GL_Account,UnitPrice,Amount,DespatchQty,RemainQty,IsFullInvoice) values ('" + SOlink + "','" + SONumber + "','" + CustomerID + "','" + SODate + "','" + IsSOClosed + "','" + CustomerPO + "','" + AccountPayID + "','" + NoOfDistibution + "','" + SODisNum + "','" + Quantity1 + "','" + ItemId + "','" + UOM + "','" + Description + "','" + GL_Account + "','" + UnitPrice + "','" + Amount + "','" + DipachQty1 + "','" + RemainQty1 + "','" + IsFullInvoice + "')";
                //tblSalesOrder
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "insert into tblINVMSO(PONO,ItemID,Qty,PODisNo,ISFull) values ('" + SONumber + "','" + ItemId + "','" + QTY + "','" + SODisNum + "','" + isfull + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();


            }
            catch
            {
                myTrans.Rollback();
            }
            //  myConnection.Close();
            finally
            {
                myConnection.Close();
            }


        }


        //======================================================================================
        public void StorePurchaseOrder(int POlink, string PoNumber, string VendorID, string Date, bool IsPOClosed, string CusSO, string AccountPayID, string NoOfDistibution, string PODisNum, double Quantity, string ItemId, string UOM, string Description, string GL_Account, double UnitPrice, double Amount, double GRNQty, double RemainQty, bool IsFullGRN, string CurrentDate, string CurrentTime, string CurretUser)
        {
            double QTY = 0;
            bool isfull = true;
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblPurchaseOrder(POlink,PONumber,VendorID,Date,IsPOClosed,CustomerSoNo,AccountPayID,NoOfDistibution,PODisNum,Quantity,ItemId,UOM,Description,GL_Account,UnitPrice,Amount,GRNQty,RemainQty,IsFullGRN,CurrentDate,CurrentTime,CurretUser) values ('" + POlink + "','" + PoNumber + "','" + VendorID + "','" + Date + "','" + IsPOClosed + "','" + CusSO + "','" + AccountPayID + "','" + NoOfDistibution + "','" + PODisNum + "','" + Quantity + "','" + ItemId + "','" + UOM + "','" + Description + "','" + GL_Account + "','" + UnitPrice + "','" + Amount + "','" + GRNQty + "','" + RemainQty + "','" + IsFullGRN + "','" + CurrentDate + "','" + CurrentTime + "','" + CurretUser + "')";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,PODisNo,ISFull,VendorID) values ('" + PoNumber + "','" + ItemId + "','" + QTY + "','" + PODisNum + "','" + isfull + "','" + VendorID + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();


            }
            catch
            {
                myTrans.Rollback();
            }
            //  myConnection.Close();
            finally
            {
                myConnection.Close();
            }


        }

        //====================================================================
        //public void Insert_CustomerbillTo()
        //{
        //    try
        //    {
        //        deleteCustomerBillTo();
        //    }
        //    catch { }

        //    XmlImplementation imp = new XmlImplementation();
        //    XmlDocument doc = imp.CreateDocument();
        //    doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer1.xml");
        //    XmlNodeList reader = doc.GetElementsByTagName("PAW_Customer");
        //    XmlNode node = reader[0];
        //    int aLength = reader.Count;
        //    Customer = Array.CreateInstance(typeof(String), 10, aLength);
        //    ArrayList CustomerList = new ArrayList();


        //    for (int i = 0; i <= aLength - 1; i++)
        //    {

        //        string CutomerID = "";
        //        string CustomerName = "";
        //        string Address1 = "";
        //        string Address2 = "";
        //        string Phone1 = "";
        //        string Custom1 = "";
        //        string Custom2 = "";
        //        string Custom3 = "";
        //        string Custom4 = "";
        //        string Custom5 = "";


        //        node = reader[i];
        //        try
        //        {
        //            Customer.SetValue(node.ChildNodes[0].InnerText, 0, i);
        //            CustomerList.Add(node.ChildNodes[0].InnerText);
        //            CutomerID = Convert.ToString(node.ChildNodes[0].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            Customer.SetValue(node.ChildNodes[1].InnerText, 1, i);
        //            CustomerList.Add(node.ChildNodes[1].InnerText);
        //            CustomerName = Convert.ToString(node.ChildNodes[1].InnerText);
        //        }
        //        catch
        //        {

        //        }

        //        try
        //        {
        //            Customer.SetValue(node.ChildNodes[2].InnerText, 2, i);
        //            CustomerList.Add(node.ChildNodes[2].InnerText);
        //            Address1 = Convert.ToString(node.ChildNodes[2].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            Customer.SetValue(node.ChildNodes[3].InnerText, 3, i);
        //            CustomerList.Add(node.ChildNodes[3].InnerText);
        //            Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        //===============================================
        //        try
        //        {
        //            Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
        //            CustomerList.Add(node.ChildNodes[4].InnerText);
        //            Phone1 = Convert.ToString(node.ChildNodes[4].InnerText);
        //            //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            //Customer.SetValue(node.ChildNodes[5].InnerText, 5, i);
        //            // CustomerList.Add(node.ChildNodes[5].InnerText);
        //            // Custom1 = Convert.ToString(node.ChildNodes[5].InnerText);
        //            Custom1 = Convert.ToString(node.ChildNodes[5].ChildNodes[0].ChildNodes[1].InnerText);
        //            //  node.ChildNodes[5].ChildNodes[1].InnerText

        //            //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            //Customer.SetValue(node.ChildNodes[6].InnerText,6, i);
        //            //CustomerList.Add(node.ChildNodes[6].InnerText);
        //            Custom2 = Convert.ToString(node.ChildNodes[5].ChildNodes[1].ChildNodes[1].InnerText);
        //            // Custom2 = Convert.ToString(node.ChildNodes[6].InnerText);

        //            // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            // Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
        //            // CustomerList.Add(node.ChildNodes[7].InnerText);
        //            //Custom3 = Convert.ToString(node.ChildNodes[7].InnerText);
        //            Custom3 = Convert.ToString(node.ChildNodes[5].ChildNodes[2].ChildNodes[1].InnerText);

        //            // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            //Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
        //            // CustomerList.Add(node.ChildNodes[7].InnerText);
        //            // Custom4 = Convert.ToString(node.ChildNodes[7].InnerText);
        //            Custom4 = Convert.ToString(node.ChildNodes[5].ChildNodes[3].ChildNodes[1].InnerText);

        //            //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {
        //            //  Customer.SetValue(node.ChildNodes[8].InnerText, 8, i);
        //            // CustomerList.Add(node.ChildNodes[8].InnerText);
        //            //  Custom5 = Convert.ToString(node.ChildNodes[8].InnerText);
        //            Custom5 = Convert.ToString(node.ChildNodes[5].ChildNodes[4].ChildNodes[1].InnerText);

        //            //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
        //        }
        //        catch
        //        {

        //        }



        //        StoreCustomerBillTo(CutomerID, CustomerName, Address1, Address2, Phone1, Custom1, Custom2, Custom3, Custom4, Custom5);

        //    }

        //}
        //================================================================

        //Create an array to store cutomer Master Data

        //==============================insert vendor==============================
        public Array Customer;


        public void deleteReceipt()
        {
            try
            {
                setConnectionString();
                String S1 = "delete tbl_Import_Receipt";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        public void deleteInvoice()
        {
            try
            {
                setConnectionString();
                String S1 = "delete tbl_Import_invoice";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }

        public void Insert_SuplierInvoice(string VATAcc, string NBTAcc)
        {
            try
            {
                deleteSupInvoice();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Enterbills.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Purchase");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();
            for (int j = 0; j <= aLength - 1; j++)
            {
                string Invoice_Number = "";
                string VendorID = "";
                string AP_Account = "";
                string _Date = "";
                string Item_ID = "";
                string Description = "";
                double Unit_Price = 0.00;
                bool isVat = false;
                double nbtam = 0.00;
                double Amount = 0.00;
                string PO_Number = "";
                string AppliedToPO = "False";
                string GlAcc = "";
                node = reader[j];
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "VendorID")
                    {
                        VendorID = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Invoice_Number")
                    {
                        Invoice_Number = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Date")
                    {
                        _Date = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "AP_Account")
                    {
                        AP_Account = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "AppliedToPO")
                    {
                        AppliedToPO = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "CustomerInvoiceNumber")
                    {
                        PO_Number = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "PurchaseLines")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "PurchaseLine")
                            {
                                for (int t = 0; t <= node.ChildNodes[i].ChildNodes[k].ChildNodes.Count - 1; t++)
                                {
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "GL_Account")
                                    {
                                        GlAcc = node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText;
                                        if (GlAcc == VATAcc.ToString())
                                        {
                                            isVat = true;
                                        }
                                        else if (GlAcc == NBTAcc.ToString())
                                        {
                                            isVat = true;
                                        }
                                    }
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "Amount")
                                    {
                                        Amount = Convert.ToDouble(node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText);
                                    }
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "Item_ID")
                                    {
                                        Item_ID = node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText;
                                    }

                                }
                            }

                            StoreSupplierInvoice(VendorID, Invoice_Number, _Date, AP_Account, AppliedToPO, Amount, PO_Number, Item_ID, GlAcc, isVat);
                        }
                    }

                }

            }

        }
        public void StoreSupplierInvoice(string _VendorID, string _Invoice_Number, string _Date, string _AP_Account, string _AppliedToPO, double _Amount, string _PO_Number, string _Item_ID, string GLaccount, bool _IsVat)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tbl_temp_Supllierinvoice(VendorID,Invoice_Number,Date,AP_Account,AppliedToPO,Amount,PO_Number,Item_ID,GLACCCNO,VAT)  values  ('" + _VendorID + "', '" + _Invoice_Number + "','" + _Date + "','" + _AP_Account + "','" + _AppliedToPO + "','" + _Amount + "','" + _PO_Number + "','" + _Item_ID + "','" + GLaccount + "','" + _IsVat + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void deleteSupInvoice()
        {
            try
            {
                setConnectionString();
                String S1 = "delete tbl_temp_Supllierinvoice";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        public void deleteCustomer()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblCustomerMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }

        public void Insert_Customer()
        {
            try
            {
                deleteCustomer();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Customer");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();


            for (int i = 0; i <= aLength - 1; i++)
            {

                string CutomerID = "";
                string CustomerName = "";
                string Address1 = "";
                string Address2 = "";
                string Phone1 = "";
                string Custom1 = "";
                string Custom2 = "";
                string Custom3 = "";
                string Custom4 = "";
                string Custom5 = "";
                string CustomerType = "";
                string shipToAddress1 = "";
                string shipToAddress2 = "";
                string Phone2 = "";
                string fax = "";
                string Email = "";
                string dueDays = "";
                string PriceLevel = "";



                node = reader[i];
                try
                {
                    CutomerID = Convert.ToString(node.ChildNodes[0].InnerText);

                }
                catch
                {
                    // CutomerID = "";
                }
                try
                {
                    CustomerName = Convert.ToString(node.ChildNodes[1].InnerText);
                }
                catch
                {
                    // CustomerName = "";
                }

                try
                {
                    //Address1 = Convert.ToString(node.ChildNodes[2].InnerText);
                    Address1 = Convert.ToString(node.ChildNodes[2].ChildNodes[0].InnerText);

                }
                catch
                {
                    // Address1 = " -";
                }
                try
                {
                    Address2 = Convert.ToString(node.ChildNodes[2].ChildNodes[1].InnerText);
                }
                catch
                {
                    // Address2 = "-";
                }

                try
                {
                    //Address1 = Convert.ToString(node.ChildNodes[2].InnerText);
                    shipToAddress1 = Convert.ToString(node.ChildNodes[3].InnerText);

                }
                catch
                {
                    //shipToAddress1 = "-";
                }
                try
                {
                    shipToAddress2 = Convert.ToString(node.ChildNodes[4].InnerText);
                }
                catch
                {
                    // shipToAddress2 = "-";
                }
                //===============================================
                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    // CustomerList.Add(node.ChildNodes[4].InnerText);
                    Phone1 = Convert.ToString(node.ChildNodes[5].ChildNodes[0].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Phone1 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    Phone2 = Convert.ToString(node.ChildNodes[5].ChildNodes[1].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Phone2 = "-";
                }

                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    //fax = Convert.ToString(node.ChildNodes[6].InnerText);
                    fax = "";
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // fax = "-";
                }
                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    Email = Convert.ToString(node.ChildNodes[6].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Email = "-";
                }
                try
                {
                    PriceLevel = Convert.ToString(node.ChildNodes[7].InnerText);
                }
                catch
                {
                    PriceLevel = "0";
                }

                try
                {
                    dueDays = Convert.ToString(node.ChildNodes[8].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom5 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[5].InnerText, 5, i);
                    // CustomerList.Add(node.ChildNodes[5].InnerText);
                    // Custom1 = Convert.ToString(node.ChildNodes[5].InnerText);
                    Custom1 = Convert.ToString(node.ChildNodes[9].ChildNodes[0].ChildNodes[1].InnerText);
                    //  node.ChildNodes[5].ChildNodes[1].InnerText

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom1 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[6].InnerText,6, i);
                    //CustomerList.Add(node.ChildNodes[6].InnerText);
                    Custom2 = Convert.ToString(node.ChildNodes[10].ChildNodes[1].ChildNodes[1].InnerText);
                    // Custom2 = Convert.ToString(node.ChildNodes[6].InnerText);

                    // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom2 = "-";
                }
                try
                {
                    // Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
                    // CustomerList.Add(node.ChildNodes[7].InnerText);
                    //Custom3 = Convert.ToString(node.ChildNodes[7].InnerText);
                    Custom3 = Convert.ToString(node.ChildNodes[11].ChildNodes[2].ChildNodes[1].InnerText);

                    // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Custom3 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
                    // CustomerList.Add(node.ChildNodes[7].InnerText);
                    // Custom4 = Convert.ToString(node.ChildNodes[7].InnerText);
                    Custom4 = Convert.ToString(node.ChildNodes[12].ChildNodes[3].ChildNodes[1].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Custom4 = "-";
                }
                try
                {
                    //  Customer.SetValue(node.ChildNodes[8].InnerText, 8, i);
                    // CustomerList.Add(node.ChildNodes[8].InnerText);
                    //  Custom5 = Convert.ToString(node.ChildNodes[8].InnerText);
                    Custom5 = Convert.ToString(node.ChildNodes[13].ChildNodes[4].ChildNodes[1].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom5 = "-";
                }

                StoreCustomer(CutomerID, CustomerName, Address1, Address2, Phone1, Custom1, Custom2, Custom3, Custom4, Custom5, shipToAddress1, shipToAddress2, Phone2, fax, Email, dueDays, PriceLevel);
                //StoreCustomer(CutomerID, CustomerName, Address1, Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,shipToAddress1,shipToAddress2,Phone2,fax,Email);

            }

        }
        public void fillID_Customer_listAll()
        {
            try
            {
                deleteCustomer();
            }
            catch { }

            try
            {
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Customer");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                DataTable _dtblContact = new DataTable();
                _dtblContact.Columns.Add("Name");
                _dtblContact.Columns.Add("Line1");
                _dtblContact.Columns.Add("Line2");
                _dtblContact.Columns.Add("City");
                _dtblContact.Columns.Add("Country");
                _dtblContact.Columns.Add("Index");

                //for(int Index1=0;Index1<=al
                for (int j = 0; j <= aLength - 1; j++)
                {
                    _dtblContact.Rows.Clear();

                    string CutomerID = "";
                    string CustomerName = "";
                    string BillToAddress1 = "";
                    string BillToAddress2 = "";
                    string BillToCity = "";
                    string BillToCountry = "";
                    string Phone1 = "";
                    string Custom1 = "";
                    string Custom2 = "";
                    string Custom3 = "";
                    string Custom4 = "";
                    string Custom5 = "";
                    string CustomerType = "";
                    //string shipTo1Address1 = "";
                    //string shipTo1Address2 = "";
                    //string shipTo1City = "";
                    //string shipTo1Country = "";
                    //string shipTo1Tax = "";
                    //string shipTo1ContactPerson = "";
                    //string shipToCountry = "";
                    string Phone2 = "";
                    string fax = "";
                    string Email = "";
                    string dueDays = "";
                    string Tax = "";
                    //string shipToTax = "";                    
                    string ContactPerson = "";
                    double Balance = 0;

                    node = reader[j];
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "ID")
                        {
                            CutomerID = node.ChildNodes[i].InnerText;
                            //if (CutomerID == "WPKVPSA1011")
                            //    MessageBox.Show("sdfsdfs");
                        }
                        else if (node.ChildNodes[i].Name == "Name")
                        {
                            CustomerName = node.ChildNodes[i].InnerText;
                        }

                        else if (node.ChildNodes[i].Name == "Customer_Type")
                        {
                            CustomerType = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "BillToAddress")
                        {
                            for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                            {
                                if (node.ChildNodes[i].ChildNodes[k].Name == "Line1")
                                {
                                    BillToAddress1 = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                                else if (node.ChildNodes[i].ChildNodes[k].Name == "Line2")
                                {
                                    BillToAddress2 = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                                else if (node.ChildNodes[i].ChildNodes[k].Name == "City")
                                {
                                    BillToCity = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                                else if (node.ChildNodes[i].ChildNodes[k].Name == "Country")
                                {
                                    BillToCountry = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                                else if (node.ChildNodes[i].ChildNodes[k].Name == "Sales_Tax_Code")
                                {
                                    Tax = node.ChildNodes[i].ChildNodes[k].InnerText;
                                }
                            }

                            DataRow drow = _dtblContact.NewRow();
                            drow["Name"] = ContactPerson;
                            drow["Line1"] = BillToAddress1;
                            drow["Line2"] = BillToAddress2;
                            drow["City"] = BillToCity;
                            drow["Country"] = BillToCountry;
                            drow["Index"] = 0;

                            if (drow["Name"].ToString() != string.Empty || drow["Line1"].ToString() != string.Empty || drow["Line2"].ToString() != string.Empty)
                                _dtblContact.Rows.Add(drow);
                        }
                        else if (node.ChildNodes[i].Name == "PhoneNumbers")
                        {
                            for (int l = 0; l <= node.ChildNodes[i].ChildNodes.Count - 1; l++)
                            {
                                if (node.ChildNodes[i].ChildNodes[l].Name == "PhoneNumber")
                                {
                                    if (l == 0)
                                        Phone1 = node.ChildNodes[i].ChildNodes[l].InnerText;
                                    if (l == 1)
                                        Phone2 = node.ChildNodes[i].ChildNodes[l].InnerText;
                                }

                            }
                        }
                        else if (node.ChildNodes[i].Name.ToString() == "CustomFields")
                        {
                            for (int l = 0; l < 5; l++)
                            {
                                if (node.ChildNodes[i].ChildNodes[l].Name.ToString() == "CustomField")
                                {

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText == "AA")
                                        Custom1 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText == "VATNO")
                                        Custom2 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText == "Gender")
                                        Custom3 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText == "Company")
                                        Custom4 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText == "Doctor")
                                        Custom5 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                }
                            }
                        }
                        else if (node.ChildNodes[i].Name == "ContactName")
                        {
                            ContactPerson = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "FaxNumber")
                        {
                            fax = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "EMail_Address")
                        {
                            Email = node.ChildNodes[i].InnerText;
                        }
                        else if (node.ChildNodes[i].Name == "Customer_Balance")
                        {
                            Balance = double.Parse(node.ChildNodes[i].InnerText);
                        }

                        else if (node.ChildNodes[i].Name == "ShipToAddresses")
                        {
                            for (int l = 0; l <= node.ChildNodes[i].ChildNodes.Count - 1; l++)
                            {
                                if (node.ChildNodes[i].ChildNodes[l].Name == "ShipToAddress")
                                {
                                    DataRow drow = _dtblContact.NewRow();

                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].Name == "Name")
                                        drow["Name"] = node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText;
                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[1].Name == "Line1")
                                        drow["Line1"] = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;
                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[2].Name == "Line2")
                                        drow["Line2"] = node.ChildNodes[i].ChildNodes[l].ChildNodes[2].InnerText;
                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[3].Name == "City")
                                        drow["City"] = node.ChildNodes[i].ChildNodes[l].ChildNodes[3].InnerText;
                                    if (node.ChildNodes[i].ChildNodes[l].ChildNodes[4].Name == "Country")
                                        drow["Country"] = node.ChildNodes[i].ChildNodes[l].ChildNodes[4].InnerText;
                                    drow["Index"] = (l + 1).ToString();

                                    if (drow["Name"].ToString() != string.Empty || drow["Line1"].ToString() != string.Empty || drow["Line2"].ToString() != string.Empty)
                                        _dtblContact.Rows.Add(drow);

                                    //if (node.ChildNodes[i].ChildNodes[l].ChildNodes[0].Name == "Name")
                                    //    shipTo1ContactPerson = node.ChildNodes[i].ChildNodes[l].ChildNodes[0].InnerText;

                                    //if (node.ChildNodes[i].ChildNodes[l].ChildNodes[1].Name == "Line1")
                                    //    shipTo1Address1 = node.ChildNodes[i].ChildNodes[l].ChildNodes[1].InnerText;

                                    //if (node.ChildNodes[i].ChildNodes[l].ChildNodes[2].Name == "Line2")
                                    //    shipTo1Address2 = node.ChildNodes[i].ChildNodes[l].ChildNodes[2].InnerText;

                                    //if (node.ChildNodes[i].ChildNodes[l].ChildNodes[3].Name == "City")
                                    //    shipTo1City = node.ChildNodes[i].ChildNodes[l].ChildNodes[3].InnerText;

                                    //if (node.ChildNodes[i].ChildNodes[l].ChildNodes[4].Name == "Country")
                                    //    shipTo1Country = node.ChildNodes[i].ChildNodes[l].ChildNodes[4].InnerText;
                                    //objclsBeansCustomer.TPNo = node.ChildNodes[i].ChildNodes[l].InnerText;
                                    //objclsBeansCustomer.TPNo = node.ChildNodes[i].ChildNodes[l].InText;
                                }
                            }
                        }
                    }

                    StoreCustomer2(CutomerID, CustomerName, BillToAddress1, BillToAddress2, Phone1, Phone2, fax,
                        Email, BillToCity, BillToCountry, ContactPerson,
                        Custom1, Custom2, Custom3, Custom4, Custom5,
                        _dtblContact,
                        dueDays, Tax, Balance, CustomerType);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return objclsBeansCustomer;
        }
        public void Insert_Customer1()
        {
            try
            {
                deleteCustomer();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Customer.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Customer");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();


            for (int i = 0; i <= aLength - 1; i++)
            {

                string CutomerID = "";
                string CustomerName = "";
                string Address1 = "";
                string Address2 = "";
                string Phone1 = "";
                string Custom1 = "";
                string Custom2 = "";
                string Custom3 = "";
                string Custom4 = "";
                string Custom5 = "";
                string CustomerType = "";
                string shipToAddress1 = "";
                string shipToAddress2 = "";
                string Phone2 = "";
                string fax = "";
                string Email = "";
                string dueDays = "";
                string PriceLevel = "";



                node = reader[i];
                try
                {
                    CutomerID = Convert.ToString(node.ChildNodes[0].InnerText);

                }
                catch
                {
                    // CutomerID = "";
                }
                try
                {
                    CustomerName = Convert.ToString(node.ChildNodes[1].InnerText);
                }
                catch
                {
                    // CustomerName = "";
                }

                try
                {
                    //Address1 = Convert.ToString(node.ChildNodes[2].InnerText);
                    Address1 = Convert.ToString(node.ChildNodes[2].ChildNodes[0].InnerText);

                }
                catch
                {
                    // Address1 = " -";
                }
                try
                {
                    Address2 = Convert.ToString(node.ChildNodes[2].ChildNodes[1].InnerText);
                }
                catch
                {
                    // Address2 = "-";
                }

                try
                {
                    //Address1 = Convert.ToString(node.ChildNodes[2].InnerText);
                    shipToAddress1 = Convert.ToString(node.ChildNodes[3].InnerText);

                }
                catch
                {
                    //shipToAddress1 = "-";
                }
                try
                {
                    shipToAddress2 = Convert.ToString(node.ChildNodes[4].InnerText);
                }
                catch
                {
                    // shipToAddress2 = "-";
                }

                try
                {
                    CustomerType = Convert.ToString(node.ChildNodes[5].InnerText);
                }
                catch
                {
                    // shipToAddress2 = "-";
                }
                //===============================================
                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    // CustomerList.Add(node.ChildNodes[4].InnerText);
                    Phone1 = Convert.ToString(node.ChildNodes[6].ChildNodes[0].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Phone1 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    Phone2 = Convert.ToString(node.ChildNodes[6].ChildNodes[1].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Phone2 = "-";
                }

                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    //fax = Convert.ToString(node.ChildNodes[6].InnerText);
                    fax = "";
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // fax = "-";
                }
                try
                {
                    // Customer.SetValue(node.ChildNodes[4].InnerText, 4, i);
                    //CustomerList.Add(node.ChildNodes[4].InnerText);
                    Email = Convert.ToString(node.ChildNodes[7].InnerText);
                    //  Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Email = "-";
                }
                try
                {
                    PriceLevel = Convert.ToString(node.ChildNodes[8].InnerText);
                }
                catch
                {
                    PriceLevel = "0";
                }

                try
                {
                    dueDays = Convert.ToString(node.ChildNodes[8].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom5 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[5].InnerText, 5, i);
                    // CustomerList.Add(node.ChildNodes[5].InnerText);
                    // Custom1 = Convert.ToString(node.ChildNodes[5].InnerText);
                    Custom1 = Convert.ToString(node.ChildNodes[10].ChildNodes[0].ChildNodes[1].InnerText);
                    //  node.ChildNodes[5].ChildNodes[1].InnerText

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom1 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[6].InnerText,6, i);
                    //CustomerList.Add(node.ChildNodes[6].InnerText);
                    Custom2 = Convert.ToString(node.ChildNodes[12].ChildNodes[0].ChildNodes[1].InnerText);
                    // Custom2 = Convert.ToString(node.ChildNodes[6].InnerText);

                    // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom2 = "-";
                }
                try
                {
                    // Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
                    // CustomerList.Add(node.ChildNodes[7].InnerText);
                    //Custom3 = Convert.ToString(node.ChildNodes[7].InnerText);
                    Custom3 = Convert.ToString(node.ChildNodes[14].ChildNodes[0].ChildNodes[1].InnerText);

                    // Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Custom3 = "-";
                }
                try
                {
                    //Customer.SetValue(node.ChildNodes[7].InnerText, 7, i);
                    // CustomerList.Add(node.ChildNodes[7].InnerText);
                    // Custom4 = Convert.ToString(node.ChildNodes[7].InnerText);
                    Custom4 = Convert.ToString(node.ChildNodes[16].ChildNodes[0].ChildNodes[1].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    // Custom4 = "-";
                }
                try
                {
                    //  Customer.SetValue(node.ChildNodes[8].InnerText, 8, i);
                    // CustomerList.Add(node.ChildNodes[8].InnerText);
                    //  Custom5 = Convert.ToString(node.ChildNodes[8].InnerText);
                    Custom5 = Convert.ToString(node.ChildNodes[18].ChildNodes[0].ChildNodes[1].InnerText);

                    //Address2 = Convert.ToString(node.ChildNodes[3].InnerText);
                }
                catch
                {
                    //Custom5 = "-";
                }

                StoreCustomer1(CutomerID, CustomerName, Address1, Address2, Phone1, Custom1, Custom2, Custom3, Custom4, Custom5, shipToAddress1, shipToAddress2, Phone2, fax, Email, dueDays, PriceLevel, CustomerType);
                //StoreCustomer(CutomerID, CustomerName, Address1, Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,shipToAddress1,shipToAddress2,Phone2,fax,Email);

            }

        }


        public void Insert_Receipt()
        {
            try
            {
                deleteReceipt();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Receipt");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();


            for (int j = 0; j <= aLength - 1; j++)
            {
                string CutomerID = "";
                string Reference = "";
                string _Date = "";
                string Payment_Method = "";
                string Cash_Account = "";
                double Total_Paid_On_Invoices = 0.00;
                string ReceiptNumber = "";
                string Number_of_Distributions = "";
                string InvoicePaid = "";
                double Amount = 0.00;
                string SalesRepId = "";

                node = reader[j];
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "Customer_ID")
                    {
                        CutomerID = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Reference")
                    {
                        Reference = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Date")
                    {
                        _Date = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Payment_Method")
                    {
                        Payment_Method = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Cash_Account")
                    {
                        Cash_Account = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Total_Paid_On_Invoices")
                    {
                        Total_Paid_On_Invoices = Convert.ToDouble(node.ChildNodes[i].InnerText) * -1;
                    }
                    else if (node.ChildNodes[i].Name == "ReceiptNumber")
                    {
                        ReceiptNumber = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Number_of_Distributions")
                    {
                        Number_of_Distributions = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Sales_Representative_ID")
                    {
                        SalesRepId = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Distributions")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "Distribution")
                            {
                                for (int t = 0; t <= node.ChildNodes[i].ChildNodes[k].ChildNodes.Count - 1; t++)
                                {
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "InvoicePaid")
                                    {
                                        InvoicePaid = node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText;
                                    }
                                    else if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "Amount")
                                    {
                                        Amount = Convert.ToDouble(node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText) * -1;
                                    }
                                }
                            }
                        }
                    }
                }

                if (Total_Paid_On_Invoices <= 0)
                {
                    Total_Paid_On_Invoices = Amount;
                }

                StoreReceipt(CutomerID, Reference, _Date, Payment_Method, Cash_Account, Total_Paid_On_Invoices, ReceiptNumber, Number_of_Distributions, InvoicePaid, Amount, SalesRepId);


            }

        }
        public void Insert_Invoice()
        {
            try
            {
                deleteInvoice();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Invoice");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();
            for (int j = 0; j <= aLength - 1; j++)
            {
                string InvoiceNumber = "";
                string CustomerID = "";
                string ARAccountId = "";
                string _Date = "";
                double Amount = 0.00;
                string SalesRepId = "";
                string Type = "";
                string CreditMemo = "False";
                node = reader[j];
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "Customer_ID")
                    {
                        CustomerID = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Invoice_Number")
                    {
                        InvoiceNumber = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Date")
                    {
                        _Date = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Accounts_Receivable_Account")
                    {
                        ARAccountId = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Sales_Representative_ID")
                    {
                        SalesRepId = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Ship_Via")
                    {
                        Type = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "CreditMemoType")
                    {
                        CreditMemo = node.ChildNodes[i].InnerText;
                    }

                    else if (node.ChildNodes[i].Name == "SalesLines")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "SalesLine")
                            {
                                for (int t = 0; t <= node.ChildNodes[i].ChildNodes[k].ChildNodes.Count - 1; t++)
                                {
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "Amount")
                                    {
                                        Amount = Amount + Convert.ToDouble(node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText) * -1;
                                    }
                                }
                            }
                        }
                    }
                }
                StoreInvoice(InvoiceNumber, CustomerID, ARAccountId, _Date, Amount, SalesRepId, Type, CreditMemo);
            }

        }

        public void Insert_Invoice_Details()
        {
            try
            {
                deleteInvoice();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesJournal.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Invoice");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();
            for (int j = 0; j <= aLength - 1; j++)
            {
                string InvoiceNumber = "";
                string CustomerID = "";
                string ARAccountId = "";
                string _Date = "";
                double Amount = 0.00;
                string SalesRepId = "";
                string Type = "";
                string CreditMemo = "False";
                string GlAcount = "";
                string isQuote = "False";
                string Payment = "";
                node = reader[j];
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "Customer_ID")
                    {
                        CustomerID = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Invoice_Number")
                    {
                        InvoiceNumber = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Date")
                    {
                        _Date = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "isQuote")
                    {
                        isQuote = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Accounts_Receivable_Account")
                    {
                        ARAccountId = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Sales_Representative_ID")
                    {
                        SalesRepId = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Ship_Via")
                    {
                        Type = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "CreditMemoType")
                    {
                        CreditMemo = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Payment")
                    {
                        Payment = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "SalesLines")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "SalesLine")
                            {
                                for (int t = 0; t <= node.ChildNodes[i].ChildNodes[k].ChildNodes.Count - 1; t++)
                                {
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "Amount")
                                    {
                                        Amount = Convert.ToDouble(node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText) * -1;

                                    }
                                    if (node.ChildNodes[i].ChildNodes[k].ChildNodes[t].Name == "GL_Account")
                                    {
                                        GlAcount = node.ChildNodes[i].ChildNodes[k].ChildNodes[t].InnerText;
                                    }
                                }
                            }
                            if (isQuote == "FALSE")
                            {
                                StoreInvoice(InvoiceNumber, CustomerID, GlAcount, _Date, Amount, SalesRepId, Type, CreditMemo);
                            }
                        }
                    }
                }

            }

        }

        public void StoreInvoice(string InvoiceNumber, string CustomerID, string ARAccountId, string _Date, double Amount, string SalesRepId, string Type, string CreditMemo)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {

                //if (Type != "Credit" && Type != "Cash" ) // only for softwave Holdings
                //{
                //    Type="Cash";
                //}
                myCommand.CommandText = "insert into tbl_Import_invoice(InvoiceNumber,Customerid,ARAccountId,Date,Amount,SalesRepId,Type,CreditMemo)  values  ('" + InvoiceNumber + "', '" + CustomerID + "','" + ARAccountId + "','" + _Date + "','" + Amount + "','" + SalesRepId + "','" + Type + "','" + CreditMemo + "' )";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }

        public void StoreReceipt(string CutomerID, string Reference, string _Date, string Payment_Method, string Cash_Account, double Total_Paid_On_Invoices, string ReceiptNumber, string Number_of_Distributions, string InvoicePaid, double Amount, string SalesRepId)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tbl_Import_Receipt(CustomerId,Reference,Rec_Date,PayMethod,CashAccountId,TotalPaidOnInvoices,ReceiptNumber,NumberOfDistributions,InvoicePaid,Amount,SalesRepId) values  ('" + CutomerID + "', '" + Reference + "','" + _Date + "','" + Payment_Method + "','" + Cash_Account + "','" + Total_Paid_On_Invoices + "','" + ReceiptNumber + "','" + Number_of_Distributions + "','" + InvoicePaid + "','" + Amount + "','" + SalesRepId + "' )";

                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }

        public void StoreCustomer2(string Cutomer_ID, string Customer_Name, string Address_1, string Address_2, string Phone, string phone2,
         string fax, string email, string City, string Country, string ContactPerson,
         string Cus1, string cus2, string cus3, string cus4, string cus5, DataTable _dtbl,
         string DueDay, string VATNo, double Balance, string cusType)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblCustomerMaster(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4," +
                    " Custom5,Phone2,Fax,Email,DueDays,VATNo,Balance,City,Country,ContactPerson,Cus_Type) values ('" + Cutomer_ID + "', '" + Customer_Name
                    + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" +
                    cus3 + "','" + cus4 + "','" + cus5 + "','" + phone2 + "','" +
                    fax + "','" + email + "','" + DueDay + "','" + cus2 + "','" + Balance + "','" + City + "','" + Country + "','" + ContactPerson + "','" + cusType + "')";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "delete from tblCustomerContact2 where CustCode='" + Cutomer_ID + "'";
                myCommand.ExecuteNonQuery();

                foreach (DataRow dr in _dtbl.Rows)
                {
                    myCommand.CommandText = " insert into tblCustomerContact2(CustCode,Name,Line1,Line2,City,Country,[Index]) " +
                        " values('" + Cutomer_ID + "','" + dr["Name"].ToString() + "','" + dr["Line1"].ToString() + "','" + dr["Line2"].ToString() + "','" + dr["City"].ToString() + "','" + dr["Country"].ToString() + "','" + dr["Index"].ToString() + "')";
                    myCommand.ExecuteNonQuery();
                }

                myCommand.CommandText = "delete from tblCustomerContact where CustCode='" + Cutomer_ID + "'";
                myCommand.ExecuteNonQuery();

                myTrans.Commit();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void StoreCustomer(string Cutomer_ID, string Customer_Name, string Address_1, string Address_2, string Phone, string Cus1, string cus2, string cus3, string cus4, string cus5, string ship1, string ship2, string phone2, string fax, string email, string DueDay, string Pricing_Level)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblCustomerMaster(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,ShipToAddress1,ShipToAddress2,Phone2,Fax,Email,DueDays,Pricing_Level) values ('" + Cutomer_ID + "', '" + Customer_Name + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" + cus3 + "','" + cus4 + "','" + cus5 + "','" + ship1 + "','" + ship2 + "','" + phone2 + "','" + fax + "','" + email + "','" + DueDay + "','" + Pricing_Level + "' )";
                // myCommand.CommandText = "insert into tblCustomerMaster(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,ShipToAddress1,ShipToAddress2,Phone2,Fax,Email) values ('" + Cutomer_ID + "', '" + Customer_Name + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" + cus3 + "','" + cus4 + "','" + cus5 + "','" + ship1 + "','" + ship2  + "','" + phone2  + "','" + fax  + "','" + email  + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch
            {
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }
        public void StoreCustomer1(string Cutomer_ID, string Customer_Name, string Address_1, string Address_2, string Phone, string Cus1, string cus2, string cus3, string cus4, string cus5, string ship1, string ship2, string phone2, string fax, string email, string DueDay, string Pricing_Level, string cusType)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblCustomerMaster(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,ShipToAddress1,ShipToAddress2,Phone2,Fax,Email,DueDays,Pricing_Level,Cus_Type) values ('" + Cutomer_ID + "', '" + Customer_Name + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" + cus3 + "','" + cus4 + "','" + cus5 + "','" + ship1 + "','" + ship2 + "','" + phone2 + "','" + fax + "','" + email + "','" + DueDay + "','" + Pricing_Level + "','" + cusType + "' )";
                // myCommand.CommandText = "insert into tblCustomerMaster(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5,ShipToAddress1,ShipToAddress2,Phone2,Fax,Email) values ('" + Cutomer_ID + "', '" + Customer_Name + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" + cus3 + "','" + cus4 + "','" + cus5 + "','" + ship1 + "','" + ship2  + "','" + phone2  + "','" + fax  + "','" + email  + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch
            {
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }
        }

        //=======================================================================

        public Array Vendor;




        public void Insert_Vendor()
        {
            try
            {
                deleteVendor();
            }
            catch { }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Vendor.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Vendor");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            Customer = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList CustomerList = new ArrayList();

            //StoreVendor("VendorID", "VendorName", "VContact", "VAddress1", "VAddress2");
            for (int j = 0; j <= aLength - 1; j++)
            {
                node = reader[j];

                //  XmlNodeList reader = doc.GetElementsByTagName("PAW_Vendor");

                //reader.Item[1];

                string VendorID = "";
                string VendorName = "";
                string VContact = "";
                string VAddress1 = "";
                string VAddress2 = "";
                bool _boolInactive = false;
                string _strCategory = "";
                string CustomField1 = "";


                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "ID")
                    {
                        VendorID = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "Name")
                    {
                        VendorName = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "VendorType")
                    {
                        _strCategory = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "ContactName")
                    {
                        VContact = node.ChildNodes[i].InnerText;
                    }
                    else if (node.ChildNodes[i].Name == "RemitToAddress")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "Line1")
                            {
                                VAddress1 = node.ChildNodes[i].ChildNodes[k].InnerText;
                            }
                            else if (node.ChildNodes[i].ChildNodes[k].Name == "Line2")
                            {
                                VAddress2 = node.ChildNodes[i].ChildNodes[k].InnerText;
                            }
                        }
                    }

                    else if (node.ChildNodes[i].Name == "CustomFields")
                    {
                        for (int k = 0; k <= node.ChildNodes[i].ChildNodes.Count - 1; k++)
                        {
                            if (node.ChildNodes[i].ChildNodes[k].Name == "CustomField")
                            {
                                CustomField1 = node.ChildNodes[i].ChildNodes[k].ChildNodes[1].InnerText;
                            }
                        }
                    }
                    else if (node.ChildNodes[i].Name == "isInactive")
                    {
                        _boolInactive = Convert.ToBoolean(node.ChildNodes[i].InnerText);
                    }

                }

                StoreVendor(VendorID, VendorName, VContact, VAddress1, VAddress2, _strCategory, _boolInactive, CustomField1);

            }

        }


        public void StoreVendor(string VenID, string Vname, string Vcontact, string Vaddress1, string Vaddress2, string Category, bool Inactive, string CusF1)
        {
            //==========================================================
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlCommand myCommand = new SqlCommand();
            SqlTransaction myTrans;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myTrans = myConnection.BeginTransaction();
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into tblVendorMaster(VendorID,VendorName,VContact,VAddress1,VAddress2,Type,Inactive,CustomField1) values ('" + VenID + "', '" + Vname + "','" + Vcontact + "','" + Vaddress1 + "','" + Vaddress2 + "','" + Category + "','" + Inactive + "','" + CusF1 + "')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
            }

            catch
            {
                myTrans.Rollback();
            }
            finally
            {
                myConnection.Close();
            }


        }



        //public void StoreCustomerBillTo(string Cutomer_ID, string Customer_Name, string Address_1, string Address_2, string Phone, string Cus1, string cus2, string cus3, string cus4, string cus5)
        //{
        //    //==========================================================
        //    setConnectionString();
        //    SqlConnection myConnection = new SqlConnection(ConnectionString);
        //    SqlCommand myCommand = new SqlCommand();
        //    SqlTransaction myTrans;
        //    myConnection.Open();
        //    myCommand.Connection = myConnection;
        //    myTrans = myConnection.BeginTransaction();
        //    myCommand.Transaction = myTrans;

        //    try
        //    {
        //        myCommand.CommandText = "insert into tblCustomerMasterBIllTo(CutomerID,CustomerName,Address1,Address2,Phone1,Custom1,Custom2,Custom3,Custom4,Custom5) values ('" + Cutomer_ID + "', '" + Customer_Name + "','" + Address_1 + "','" + Address_2 + "','" + Phone + "','" + Cus1 + "','" + cus2 + "','" + cus3 + "','" + cus4 + "','" + cus5 + "')";
        //        myCommand.ExecuteNonQuery();
        //        myTrans.Commit();
        //    }

        //    catch
        //    {
        //        myTrans.Rollback();
        //    }
        //    finally
        //    {
        //        myConnection.Close();
        //    }


        //}

        //===========================================
        public Array coa;
        public void fillID_Item_listWarehouse(DateTime Tran_Date)
        {
            try
            {
                deleteItemWarehouse();
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }

            XmlImplementation imp = new XmlImplementation();
            XmlDocument doc = imp.CreateDocument();
            doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemsFillWarehouse.xml");
            XmlNodeList reader = doc.GetElementsByTagName("PAW_Item");
            XmlNode node = reader[0];
            int aLength = reader.Count;
            coa = Array.CreateInstance(typeof(String), 10, aLength);
            ArrayList Items = new ArrayList();

            for (int i = 0; i <= aLength - 1; i++)
            {

                string WarehouseID = "HO";
                string WhseName = "Head office";
                string ItemID = "";
                string Description = "";
                string ItemClass = "";
                double UnitPrice = 0.00;
                string SalesGLAccount = "";
                string Categoty = "";
                string UOM = "";
                double UnitCost = 0.00;
                double OnhandQty = 0.00;
                string TranType = "OpbBal";
                DateTime TranDate = Tran_Date;
                double TotalCost = 0.00;


                node = reader[i];
                try
                {
                    ItemID = Convert.ToString(node.ChildNodes[0].InnerText);
                }
                catch
                {

                }
                try
                {
                    Description = Convert.ToString(node.ChildNodes[1].InnerText);
                }
                catch
                {

                }

                try
                {
                    ItemClass = Convert.ToString(node.ChildNodes[2].InnerText);
                }
                catch
                {

                }
                try
                {
                    UnitPrice = Convert.ToDouble(node.ChildNodes[3].InnerText);
                }
                catch
                {


                }
                try
                {
                    UnitCost = Convert.ToDouble(node.ChildNodes[4].InnerText);
                }
                catch
                {

                }

                try
                {
                    SalesGLAccount = Convert.ToString(node.ChildNodes[5].InnerText);

                }
                catch
                {

                }

                //========================
                if (node.ChildNodes[6].Name == "Type")
                {
                    try
                    {
                        Categoty = Convert.ToString(node.ChildNodes[6].InnerText);

                    }
                    catch
                    {
                        Categoty = "";
                    }

                    try
                    {
                        UOM = "";
                    }
                    catch
                    {

                    }

                    try
                    {
                        OnhandQty = Convert.ToDouble(node.ChildNodes[7].InnerText);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    Categoty = "";

                    try
                    {
                        UOM = "";

                    }
                    catch
                    {

                    }

                    try
                    {
                        OnhandQty = Convert.ToDouble(node.ChildNodes[6].InnerText);
                    }
                    catch
                    {

                    }
                }
                //=======================


                TotalCost = OnhandQty * UnitCost;

                StoreItemWarehouse1(WarehouseID, WhseName, ItemID, Description, OnhandQty, UOM, TranDate, UnitCost, TranType, TotalCost, OnhandQty);
            }

        }

        //=================================================================================

        public Array coa11;
        public Array BOM;
        public void InserBOMData()
        {
            try
            {
                deleteBOM();
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\BOMMaster.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_BillOfMaterials_Entry");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                BOM = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                for (int i = 0; i <= aLength - 1; i++)
                {
                    node = reader[i];
                    StrAssemblyID = "";
                    StrAssemblyDesc = "";
                    IntNoOfComponent = 0;
                    StrComponentID = "";
                    IntCompNo = 0;
                    StrCompDescription = "";
                    dblCompQtyNeed = 0.00;
                    IntRevisionNo = 0;

                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {

                        if (node.ChildNodes[j].Name == "Assembly_ID")
                        {
                            StrAssemblyID = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        if (node.ChildNodes[j].Name == "Assembly_Description")
                        {
                            StrAssemblyDesc = Convert.ToString(node.ChildNodes[j].InnerText).Replace("'", "");
                        }
                        if (node.ChildNodes[j].Name == "Revision_Number")
                        {
                            IntRevisionNo = Convert.ToInt32(node.ChildNodes[j].InnerText);
                        }
                        //Revision_Number
                        else if (node.ChildNodes[j].Name == "Number_of_Components")
                        {
                            IntNoOfComponent = Convert.ToInt32(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Components")
                        {
                            int k = 0;
                            for (k = 0; k <= node.ChildNodes[j].ChildNodes.Count - 1; k++)
                            {

                                StrComponentID = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[0].InnerText);
                                IntCompNo = Convert.ToInt32(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                StrCompDescription = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[2].InnerText).Replace("'", "");
                                dblCompQtyNeed = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].ChildNodes[3].InnerText);

                                StoreAssemblyMasterData(StrAssemblyID, StrAssemblyDesc, IntNoOfComponent, StrComponentID, IntCompNo, StrCompDescription, dblCompQtyNeed, IntRevisionNo);

                                //if (k == 0)
                                //{
                                //    StrComponentID = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[k].InnerText);
                                //}
                                //if (k == 1)
                                //{
                                //    IntCompNo = Convert.ToInt32(node.ChildNodes[j].ChildNodes[k].ChildNodes[k].InnerText);
                                //}
                                //if (k == 2)
                                //{
                                //    StrCompDescription = Convert.ToString(node.ChildNodes[j].ChildNodes[k].InnerText);
                                //}
                                //if (k == 3)
                                //{
                                //    dblCompQtyNeed = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                //}
                            }
                        }
                    }
                    if (StrAssemblyID.Trim().Length > 0)
                    {
                        //StoreAssemblyMasterData(StrComponentID,StrAssemblyDesc,IntNoOfComponent,StrComponentID,IntCompNo,StrCompDescription,dblCompQtyNeed);

                        //StoreItem(ItemID, Description, ItemClass, UnitPrice, SalesGLAccount, Categoty, UOM, UnitCost, PriceLevel1, PriceLevel2, PriceLevel3, PriceLevel4, PriceLevel5, PriceLevel6, PriceLevel7, PriceLevel8, PriceLevel9, PriceLevel10);
                        //UpdateWarehouseItem(ItemID, Description, UOM, UnitCost);
                    }
                }
                //ClearWHItems_ByItemMaster();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //=================================================================================

        // public Array coa11;
        public void fillID_Item_list()
        {
            try
            {
                deleteItem();
                XmlImplementation imp = new XmlImplementation();
                XmlDocument doc = imp.CreateDocument();
                doc.Load(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Items.xml");
                XmlNodeList reader = doc.GetElementsByTagName("PAW_Item");
                XmlNode node = reader[0];
                int aLength = reader.Count;
                coa = Array.CreateInstance(typeof(String), 10, aLength);
                ArrayList Items = new ArrayList();

                for (int i = 0; i <= aLength-1; i++)
                {
                    var inactive = "false";
                    node = reader[i];
                    var salsDisc = "";
                    var Custom1 = "";
                    var Custom2 = "";
                    var Custom3 = "";
                    var Custom4 = "";
                    var Custom5 = "";

                    ItemID = "";
                    Description = "";
                    ItemClass = "";
                    UnitPrice = 0.00;
                    SalesGLAccount = "";
                    Categoty = "";
                    UOM = "";
                    UnitCost = 0.00;
                    CostOfSales = string.Empty;
                    InventoryAcc = string.Empty;
                    TaxType = "";
                    var Costing_Method = "";
                    PriceLevel1 = 0.00;
                    PriceLevel2 = 0.00;
                    PriceLevel3 = 0.00;
                    PriceLevel4 = 0.00;
                    PriceLevel5 = 0.00;
                    PriceLevel6 = 0.00;
                    PriceLevel7 = 0.00;
                    PriceLevel8 = 0.00;
                    PriceLevel9 = 0.00;
                    PriceLevel10 = 0.00;

                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {

                        if (node.ChildNodes[j].Name == "ID")
                        {
                            ItemID = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Description")
                        {
                            Description = Convert.ToString(node.ChildNodes[j].InnerText).Replace("'", "");
                        }
                        else if (node.ChildNodes[j].Name == "isInactive")
                        {
                            inactive = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Description_for_Sales")
                        {
                            salsDisc = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Costing_Method")
                        {
                            Costing_Method = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Class")
                        {
                            ItemClass = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Tax_Type_Name")
                        {
                            TaxType = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "CustomFields")
                        {
                            int k = 0;
                            for (k = 0; k <= node.ChildNodes[j].ChildNodes.Count - 1; k++)
                            {
                                if (node.ChildNodes[j].ChildNodes[k].Name == "CustomField")
                                {
                                    if (k == 0)
                                    {
                                        Custom1 = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                    }else if (k == 1)
                                    {
                                        Custom2 = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                    }
                                    else if (k == 2)
                                    {
                                        Custom3 = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                    }
                                    else if (k == 3)
                                    {
                                        Custom4 = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                    }
                                    else if (k == 4)
                                    {
                                        Custom5 = Convert.ToString(node.ChildNodes[j].ChildNodes[k].ChildNodes[1].InnerText);
                                    }

                                        

                                    
                                }
                            }
                            
                        }
                        else if (node.ChildNodes[j].Name == "Sales_Prices")
                        {
                            int k = 0;
                            for (k = 0; k <= node.ChildNodes[j].ChildNodes.Count - 1; k++)
                            {
                                if (k == 0)
                                {
                                    UnitPrice = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                    PriceLevel1 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 1)
                                {

                                    PriceLevel2 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 2)
                                {

                                    PriceLevel3 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 3)
                                {

                                    PriceLevel4 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 4)
                                {

                                    PriceLevel5 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 5)
                                {

                                    PriceLevel6 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 6)
                                {

                                    PriceLevel7 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 7)
                                {

                                    PriceLevel8 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 8)
                                {

                                    PriceLevel9 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }
                                if (k == 9)
                                {

                                    PriceLevel10 = Convert.ToDouble(node.ChildNodes[j].ChildNodes[k].InnerText);
                                }

                            }

                        }
                        //LastUnitCost

                        else if (node.ChildNodes[j].Name == "Last_Unit_Cost")
                        {
                            UnitCost = Convert.ToDouble(node.ChildNodes[j].InnerText);

                        }
                        //GL Account

                        else if (node.ChildNodes[j].Name == "GL_Sales_Account")
                        {
                            SalesGLAccount = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "GL_Inventory_Account")
                        {
                            InventoryAcc = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "GL_COGSSalary_Acct")
                        {
                            CostOfSales = Convert.ToString(node.ChildNodes[j].InnerText);
                        }

                        //Type
                        else if (node.ChildNodes[j].Name == "Type")
                        {

                            Categoty = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                        else if (node.ChildNodes[j].Name == "Stocking_UM")
                        {

                            UOM = Convert.ToString(node.ChildNodes[j].InnerText);
                        }
                    }

                    if (ItemID.Trim().Length > 0)
                    {
                        StoreItem(ItemID, Description, salsDisc, inactive, ItemClass, Costing_Method, UnitPrice, SalesGLAccount, InventoryAcc, CostOfSales, Categoty, UOM, UnitCost, PriceLevel1, PriceLevel2, PriceLevel3, PriceLevel4, PriceLevel5, PriceLevel6, PriceLevel7, PriceLevel8, PriceLevel9, PriceLevel10, TaxType, Custom1, Custom2, Custom3, Custom4, Custom5);
                        UpdateWarehouseItem(ItemID, Description, UOM, UnitCost);
                    }
                }
                //ClearWHItems_ByItemMaster();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //.............................


        //=====================================================================
        public void StoreItemWarehouse(string Item_ID)
        {
            try
            {
                String S14 = "SELECT DISTINCT WhseId FROM tblWhseMaster";
                SqlDataAdapter da14 = new SqlDataAdapter(S14, ConnectionString);
                DataSet ds14 = new DataSet();
                da14.Fill(ds14);
                for (int a = 0; a < ds14.Tables[0].Rows.Count; a++)
                {
                    //    String S161 = "select WhseId,ItemId from tblWhseWiseItem where WhseId ='" + ds14.Tables[0].Rows[a].ItemArray[0] + "' and ItemId='" + Item_ID + "'";
                    //    SqlCommand cmd161 = new SqlCommand(S161);
                    //    SqlDataAdapter da161 = new SqlDataAdapter(S161, ConnectionString);
                    //    DataTable dt161 = new DataTable();
                    //    da161.Fill(dt161);
                    //    if (dt161.Rows.Count > 0)
                    //    {
                    String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('" + ds14.Tables[0].Rows[a].ItemArray[0] + "','" + Item_ID + "')";
                    SqlCommand cmd16 = new SqlCommand(S16);
                    SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
                    DataTable dt16 = new DataTable();
                    da16.Fill(dt16);
                    //    }
                    //    else
                    //    {
                    //        //String S16 = "Insert into tblWhseWiseItem (WhseId,ItemId) values ('" + ds14.Tables[0].Rows[a].ItemArray[0] + "','" + Item_ID + "')";
                    //        //SqlCommand cmd16 = new SqlCommand(S16);
                    //        //SqlDataAdapter da16 = new SqlDataAdapter(S16, ConnectionString);
                    //        //DataTable dt16 = new DataTable();
                    //        //da16.Fill(dt16);
                    //    }
                }
            }
            catch { }
        }

        //======================================================================

        public void deleteItem()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblItemMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void deleteBOM()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblAssemblyMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ClearWHItems_ByItemMaster()
        {
            try
            {
                setConnectionString();

                String S1 = " declare @Item varchar(50) " +
                    " DECLARE db_cursorb CURSOR FOR " +
                    " SELECT  distinct tblItemWhse.ItemID FROM tblItemMaster RIGHT OUTER JOIN tblItemWhse ON tblItemMaster.ItemID = tblItemWhse.ItemId WHERE (tblItemMaster.ItemID IS NULL) " +
                    " OPEN db_cursorb FETCH NEXT FROM db_cursorb INTO @Item " +
                    " WHILE @@FETCH_STATUS = 0 " +
                    " BEGIN " +
                    " delete from dbo.tblItemWhse where ItemId=@Item " +
                    " FETCH NEXT FROM db_cursorb INTO @Item " +
                    " END    " +
                    " CLOSE db_cursorb " +
                    " DEALLOCATE db_cursorb ";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void deleteItemWarehouse()
        {
            try
            {
                setConnectionString();

                String S1 = "delete [tblPeachtreeQtyOnhand]";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

            }
            catch { }

        }
        public void deleteChartofAccounts()
        {
            try
            {
                setConnectionString();

                StrSql = "delete tblChartofAcounts";
                SqlCommand cmd1 = new SqlCommand(StrSql);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }

        public void deleteVendor()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblVendorMaster";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }
        //public void deleteCustomerBillTo()
        //{
        //    try
        //    {
        //        setConnectionString();

        //        String S1 = "delete tblCustomerMasterBIllTo";
        //        SqlCommand cmd1 = new SqlCommand(S1);
        //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
        //        DataTable dt1 = new DataTable();
        //        da1.Fill(dt1);
        //    }
        //    catch { }

        //}

        public void deleteSalesOrder()
        {
            try
            {
                setConnectionString();

                String S1 = "delete tblSalesOrder";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
            }
            catch { }

        }


        public void StoreAssemblyMasterData(string AssemID, string AssemDesc, Int32 NoOfComp, string CompID, Int32 CompNo, string CompDecs, double CompQtyNeed, Int32 RevisionNo)
        {
            setConnectionString();
            try
            {
                string ConnString = ConnectionString;
                String S2 = "insert into tblAssemblyMaster(AssemblyID,AssemblyDesc,NoOfComp,CompID,CompNo,CompDesc,CompQtyNeeded,RevisionNumber) values ('" + AssemID + "','" + AssemDesc + "','" + NoOfComp + "','" + CompID + "','" + CompNo + "','" + CompDecs + "','" + CompQtyNeed + "','" + RevisionNo + "')";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            catch
            {

            }

        }


        public void StoreItem(string Item_ID, string Descripti_on,string salsDisc, string inactive, string Item_class,string Costing_Method, double Unit_Price, string SalesGL_Account,
            string InventoryAcc, string CostOfSales,
            string Catego_ty, string UOM, double UnitCost, double P1, double P2, double P3, double P4, double P5, double P6, double P7, double P8, double P9, double P10, string TaxType,string Custom1, string Custom2, string Custom3, string Custom4, string Custom5)//, string Inve_Acc, string CostOfSalesAcc, string PartNO)
        {
            setConnectionString();
            try
            {

                string ConnString = ConnectionString;
                String S2 = "insert into tblItemMaster(InventoryAcc,CostOfSalesAcc,ItemID,ItemDescription,ItemClass,UnitPrice,SalesGLAccount, " +
                    " Categoty,UOM,UnitCost,PriceLevel1,PriceLevel2,PriceLevel3,PriceLevel4,PriceLevel5,PriceLevel6,PriceLevel7, " +
                    " PriceLevel8,PriceLevel9,PriceLevel10,TaxType,inactive,DescriptionForSale,CostMethod,Custom1,Custom2,Custom3,Custom4,Custom5) " +
                    "values ('" + InventoryAcc + "','" + CostOfSales + "','" + Item_ID + "','" + Descripti_on + "','" + Item_class + "','" + Unit_Price + "', '" + SalesGL_Account + "',"+
                    "'" + Catego_ty + "','" + UOM + "','" + UnitCost + "','" + P1 + "','" + P2 + "','" + P3 + "','" + P4 + "','" + P5 + "','" + P6 + "','" + P7 + "',"+
                    "'" + P8 + "','" + P9 + "','" + P10 + "','" + TaxType.ToString() + "','"+inactive+"','"+ salsDisc + "','"+ Costing_Method + "','"+ Custom1 + "','"+ Custom2+ "','"+ Custom3+ "','"+ Custom4+ "','"+ Custom5+ "')";//ItemType
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UpdateWarehouseItem(string Item_ID, string Descripti_on, string UOM, double UnitCost)
        {
            setConnectionString();
            try
            {

                string ConnString = ConnectionString;
                String S2 = "UPDATE tblItemWhse SET [ItemDis] = '" + Descripti_on + "',[UOM] = '" + UOM + "',[UnitCost] ='" + UnitCost + "'  WHERE [ItemId] = '" + Item_ID + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
            }
            catch
            {

            }

        }

        public void StoreItemWarehouse1(string WarehouseID, string WarehName, string ItemId, string Item_Description, double Qty, string UOM, DateTime Trandate, double Unitcost, string TranType, double Totalcost, double OPBQty)
        {

            // string TranType = "OpbBal";
            int DocType = 1;
            bool QtyIN = true;
            double SellingPrice = 0.00;

            setConnectionString();
            try
            {

                string ConnString = ConnectionString;

                String S2 = "insert into tblItemWhse(WhseId,WhseName,ItemId,ItemDis,QTY,UOM,TraDate,UnitCost,TranType,TotalCost,OPBQtry) values ('" + WarehouseID + "','" + WarehName + "','" + ItemId + "','" + Item_Description + "', '" + Qty + "','" + UOM + "','" + Trandate + "','" + Unitcost + "','" + TranType + "','" + Totalcost + "','" + OPBQty + "')";//ItemType
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);


                //=======================================
                //SqlCommand cmd11 = new SqlCommand("Insert into tbItemlActivity (DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values ('" + DocType + "','" + TranType + "','" + Trandate + "','" + TranType + "','" + QtyIN + "','" + ItemId  + "','" + Qty  + "','" + Unitcost  + "','" + Totalcost  + "','" + WarehouseID  + "','" + SellingPrice + "')", myConnection, myTrans);
                //cmd11.ExecuteNonQuery();

                //===================================
                String S21 = "Insert into tbItemlActivity (DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values ('" + DocType + "','" + TranType + "','" + Trandate + "','" + TranType + "','" + QtyIN + "','" + ItemId + "','" + Qty + "','" + Unitcost + "','" + Totalcost + "','" + WarehouseID + "','" + SellingPrice + "')";//ItemType
                SqlCommand cmd21 = new SqlCommand(S21);
                SqlDataAdapter da21 = new SqlDataAdapter(S21, ConnString);
                DataSet ds21 = new DataSet();
                da21.Fill(ds21);
                //=================================
            }
            catch
            {

            }

        }
        //==============================================================


        public void ExportSaleaOrderFrmPeachtree() // purchase invoice journal
        {

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\EnterbillsAAAA.xml");
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);
            //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);

            // exporter = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);


            // connnect with the correct journal
            //  importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            //importer.AddToImportFieldList((short) Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);










            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
            ////exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField

            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);

            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\EnterbillsAAAA.xml");
            exporter.Export();
        }
        //==============================GetSerializedGRN From Peachtree===================



        public void ExportEnterBillsperiod(DateTime FromDate, DateTime ToDate) // purchase invoice journal
        {
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Enterbills.xml");
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);

            // exporter = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DropShipInvoiceNumber);
            exporter.SetDateFilterValue(PeachwIEDateFilterOperation.peachwIEDateFilterOperationRange, FromDate, ToDate);
            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Enterbills.xml");
            exporter.Export();
        }
        public void ExportEnterBillsFromP() // purchase invoice journal
        {
            //DDDDD



            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Enterbills.xml");
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            //  exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);

            // exporter = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);

            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Enterbills.xml");
            exporter.Export();
        }
        //=================================================================================
        public void ImportSupplierInvoice() // purchase invoice journal
        {

            //try
            //{

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            //Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DateDue);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DropShipInvoiceNumber);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml");
            importer.Import();


            // MessageBox.Show("Successfully Saved");
            //}
            //catch (System.Exception e)
            //{

            //string a = e.Message.Substring(0, 40);
            //string b = "This transaction will cause the balance";
            //            This transaction will cause the balance    
            //// if (emassage == "This transaction will cause the balance")
            // if (String.ReferenceEquals(a, b))
            // {               
            //     MessageBox.Show("Successfully Saved");
            // }
            // else
            // {
            //   MessageBox.Show("kjkjk");
            //}

            // emassage.Substring(0, 37);
            //This transaction will cause the balance of customer [T000001] to go over the credit limit by $12,500.00.  Do you want to continue?
            //    MessageBox.Show(e.Message);
            //}
        }
        public void ExportSupplierInvoice() // purchase invoice journal
        {
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            File.Delete(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml");
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjPurchaseJournal);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_DropShipInvoiceNumber);
            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml");
            exporter.Export();
        }

        public int ImportGRNUpload()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjPurchaseJournal);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_VendorId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_APAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Amount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_GLAccountId);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_AppliedToPurchaseOrder);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderNum);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_PurchaseOrderDistNum);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjPurchaseJournalField.peachwIEObjPurchaseJournalField_enSerialNumber);

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\GRNExport.xml");
                importer.Import();

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //the folowing cod esement is used to import sales order date to peachtree
        //Date 01/09/2013
        //auther sanjeewa

        public void ImportSalesOrder()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesOrders);
                importer.ClearImportFieldList();

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ShipByDate);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderClosed);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesRepId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_CustomerPurchaseOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_SalesOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Description);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_enUMID);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesOrdersField.peachwIEObjSalesOrdersField_Amount);

                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrderJournal.xml");
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                importer.Import();

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }




        public int ImportCustomerReturnAR() // purchase invoice journal
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                // connnect with the correct journal
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
                importer.ClearImportFieldList();
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml");
                importer.Import();

                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import Customer Return Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            //MessageBox.Show("Imported Successfully");
        }



        public void ImportCustomerReturn() // purchase invoice journal
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                // connnect with the correct journal
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
                importer.ClearImportFieldList();
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml");
                importer.Import();

                //return 1;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Import Customer Return Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return 0;
                throw ex;
            }
            //MessageBox.Show("Imported Successfully");
        }

        public void ExportCustomerReturn() // purchase invoice journal
        {
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            // connnect with the correct journal
            exporter = (Interop.PeachwServer.Export)app.CreateExporter(PeachwIEObj.peachwIEObjSalesJournal);

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToInvoiceNumber);
            //exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);

            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);
            exporter.AddToExportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
            exporter.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            exporter.SetFilename(@System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturnAA.xml");
            exporter.Export();
            //MessageBox.Show("Imported Successfully");
        }

        //====================================================================================

        public void ImportDirectSalesInvice()
        {
            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            //Interop.PeachwServer.Import importer;

            // connnect with the correct journal
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_DateDue);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerPurchaseOrder);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_IsCreditMemo);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
            //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
            //importer.AddToImportFieldList((short) Interop.PeachwServer.PeachwIEObjSalesJournalField.
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAccountId);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CostOfSalesAmount);
            importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ShipVia);
            importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml");
            importer.Import();
        }




        public int ImportReceiptJ_Imp()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);

                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Quantity);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ItemId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Description);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_GLAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TaxType);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TotalPaidOnInvoices);
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(@System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml");//C:\Copy of Receipts2.xml
                importer.Import();
                return 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int Upload_ReceiptJ_BatchMode(string FilePath)
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);

                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Quantity);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ItemId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Description);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_GLAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TaxType);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TotalPaidOnInvoices);

                importer.SetWarnBeforeImportingDuplicates(1);
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeCSV);
                // importer.SetFilename(@System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceiptsTest.xml");//
                importer.SetFilename(FilePath);
                importer.Import();
                return 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int ImportReceiptJ_BatchMode()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);

                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Quantity);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ItemId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Description);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_GLAccountId);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TaxType);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
                importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
                importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(@System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml");//C:\Copy of Receipts2.xml
                importer.Import();
                return 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public void Import_Receipt_JournalOnline()
        {
            //Interop.PeachwServer.Login login = new Interop.PeachwServer.LoginClass();
            //importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);

            Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
            importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCashReceiptsJournal);

            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CustomerId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Reference);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Date);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_PayMethod);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_CashAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_NumberOfDistributions);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_InvoicePaid);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Quantity);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ItemId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Description);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_GLAccountId);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TaxType);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_Amount);
            importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_SalesRepId);
            //importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_TransactionNumber);
            // importer.AddToImportFieldList((short)PeachwIEObjCashReceiptsJournalField.peachwIEObjCashReceiptsJournalField_ReceiptNumber);

            importer.SetFileType(PeachwIEFileType.peachwIEFileTypeXML);
            importer.SetFilename(@System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml");//C:\Copy of Receipts2.xml
            importer.Import();
        }


        //========================================================================
        public int ImportSalesInvice()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesRepId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml");

                importer.Import();
                return 1;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, "Import", ex.StackTrace);
                return 0;
            }
        }

        public int ImportCustomerMaster_return()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjCustomerList);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerName);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjCustomerListField.peachwIEObjCustomerListField_CustomerCategory);

                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerMaster.xml");
                importer.Import();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int ImportCustomerInvoices_Return()
        {
            try
            {
                Interop.PeachwServer.Login login = new Interop.PeachwServer.Login();
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjSalesJournal);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_CustomerId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Date);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ARAmount);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_NumberOfDistributions);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_InvoiceDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Quantity);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_UnitPrice);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Description);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_GLAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_TaxType);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_Amount);
                //importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_JobId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_ApplyToSalesOrder);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderDistNum);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_SalesOrderNumber);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjSalesJournalField.peachwIEObjSalesJournalField_enSerialNumber);
                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml");
                importer.Import();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //System.Runtime.InteropServices.COMException d = new System.Runtime.InteropServices.COMException();
                //d.ErrorCode="";
                //if(ex.Source.
                //if (ex.Message.Contains("WARNING! The reference number has already been used."))
                //    return 1;                
                return 0;
            }
        }

        public int ImportItemMasterToPeactree()
        {
            try
            {
                importer = (Interop.PeachwServer.Import)app.CreateImporter(PeachwIEObj.peachwIEObjInventoryItemsList);
                importer.ClearImportFieldList();
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_ItemDescription);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Class);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_Category);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitOfMeasure);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_LaborCost);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_SalesDesc);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice1);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice2);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice3);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice4);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice5);

                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice6);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice7);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice8);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice9);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_UnitPrice10);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InventoryAccountId);
                importer.AddToImportFieldList((short)Interop.PeachwServer.PeachwIEObjInventoryItemsListField.peachwIEObjInventoryItemsListField_InvChgAccountId);



                importer.SetFileType(Interop.PeachwServer.PeachwIEFileType.peachwIEFileTypeXML);
                importer.SetFilename(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\ItemMaster.xml");
                importer.Import();

                return 1;
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }

        }
    }
}
