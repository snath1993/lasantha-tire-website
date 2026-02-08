using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using Interop.PeachwServer;
using System.IO;
using System.Data.OleDb;
//using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;


namespace UserAutherization
{
    public partial class frmInsertToHeadoffice : Form
    {

        SqlCommand CMD;
        SqlConnection Con;
        SqlDataAdapter DA;
        DataSet DS;
        SqlTransaction Trans;
        string SQL;
        public static string ConnectionString;

        public Array coa;
        string StrInvoiceNo;
        string StrCustomerID;
        string StrDeliveryNoteNos;
        DateTime DtInvoiceDate;
        string StrARAccount;
        double dblNoofDistributions;
        double dblDistributionNo;
        string StrItemID;
        double dblQty;
        string StrDescription;
        string StrGLAcount;
        double dblUnitPrice;
        double dblDiscount;
        double dblAmount;
        double dblDiscountAmount;
        double dblTax1Amount;
        double dblTax2Amount;
        double dblGrossTotal;
        double dblNetTotal;
        DateTime dtCurrentDate;
        string StrTime;
        string StrCurrentuser;
        bool blIsExport;
        string StrCustomerPO;
        string StrUOM;
        string StrJobID;
        string StrSONO;
        string StrLocation;
        string StrTTType1;
        string StrTTType2;
        bool blIsReturn;
        string StrTTType3;
        double dblTax3Amount;
        double dblRemainQty;
        string StrSalesRep;
        double dblCostPrrice;
        string StrPaymentM;
        string StrItemClass;
        string StrItemType;
        bool blIsVoid;
        string StrVoidReson;
        string StrVoidUser;
        string StrComments;
        double dblTax1Rate;
        double dblTax2Rate;
        double dblSubValue;
        bool blIsApplyToSO;


        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        public frmInsertToHeadoffice()
        {
            InitializeComponent();
            setConnectionString();

        }

        private void gbCreateXML_Enter(object sender, EventArgs e)
        {

        }
        public int UpdateOnHandQtyINV(string ItemID, double Qty, string WH)
        {
            try
            {
                string SQL = "UPDATE tblItemWhse SET QTY=QTY-" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }
        private int ExportInventoryAdjustments(DataRow Dr)
        {
            try
            {
                //SQL = "SELECT AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,AdjustQty,ReasonToAdjust,GLAccount from tblInventoryAdjustment";
                //CMD = new SqlCommand(SQL, Con, Trans);
                //CMD.CommandType = CommandType.Text;
                //DA = new SqlDataAdapter(CMD);
                //DS = new DataSet();
                //DA.Fill(DS);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustmentGRN.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Items");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


                //foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:item");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["ItemID"].ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(Dr["AdjusmentId"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(Dr["Date"].ToString());//Date format must be (MM/dd/yyyy)
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("AdjustmentItems");
                    Writer.WriteStartElement("AdjustmentItem");

                    Writer.WriteStartElement("GLSourceAccount ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["GLAccount"].ToString().Trim());
                    Writer.WriteEndElement();



                    Writer.WriteStartElement("UnitCost");
                    Writer.WriteString(Dr["UnitCost"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(Dr["AdjustQty"].ToString().Trim());
                    Writer.WriteEndElement();

                    double TotalAmount = 0.00;
                    TotalAmount = Convert.ToDouble(Dr["AdjustQty"]) * Convert.ToDouble(Dr["UnitCost"]);

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(TotalAmount.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();//Adjustment Line
                    Writer.WriteEndElement();//Adjustment lines

                }
                Writer.WriteEndElement();
                Writer.Close();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int UpdateOnHandAdjustment(string ItemID, double Qty, string WH)
        {
            try
            {
                if (Qty < 0)
                {
                    string SQL = "UPDATE tblItemWhse SET QTY=QTY -" + Math.Abs(Qty) + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    CMD.ExecuteNonQuery();
                    return 1;
                }

                if (Qty > 0)
                {
                    string SQL = "UPDATE tblItemWhse SET QTY=QTY+" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    CMD.ExecuteNonQuery();
                    return 1;

                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SaveItemActivityAdjustment(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH)
        {
            try
            {


                bool DocRef = false;
                // int DocRef = 0;
                if (Qty > 0)
                {
                    DocRef = true;
                }
                string SQL = "INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('6', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'InvAdjust','" + DocRef + "','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "','0')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SaveAdjustments(string StrAdjusmentId, DateTime DTInvDate, string StrWarehouseId, string StrWarehouseName, string StrItemID, string StrItemdescription, double dblUnitCost, double dblOnhandQty, double dblAdjustQty, double dblNewQty, string StrReasonToAdjust, string StrGLAccount)
        {
            try
            {
                string SQL = " INSERT INTO tblInventoryAdjustment(AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount) VALUES('" + StrAdjusmentId + "','" + DTInvDate + "','" + StrWarehouseId + "','" + StrWarehouseName + "','" + StrItemID + "','" + StrItemdescription + "','" + dblUnitCost + "','" + dblOnhandQty + "','" + dblAdjustQty + "','" + dblNewQty + "','" + StrReasonToAdjust + "','" + StrGLAccount + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }


        private DataSet Get_Adjustments_From_CSV_To_Dataset(string FileName, string FilePath)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                // FillGL(dsExcel);
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT AdjusmentId,Date,WarehouseId as WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,OnhandQty,AdjustQty,NewQty,ReasonToAdjust,GLAccount FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return dsExcel;
            }
        }

        private DataSet GetReceipts_From_CSV_To_Dataset(string FileName, string FilePath)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                // FillGL(dsExcel);
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return dsExcel;
            }
        }




        public void DeleteItemTemp()
        {
            try
            {
                string SQL = "DELETE FROM [tblTempReceiptUpload]";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();

                //Con = new SqlConnection(ConnectionString);
                //Con.Open();
                //SQL = "DELETE FROM [tblTempReceiptUpload]";
                //CMD = new SqlCommand(SQL, Con, Trans);
                //CMD.CommandType = CommandType.Text;
                //DA = new SqlDataAdapter(CMD);
                //DS = new DataSet();
                //DA.Fill(DS);
                // Con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnInserFiles_Click(object sender, EventArgs e)
        {
            DataSet dsCustomer = new DataSet();
            DataSet dsInvoiceList = new DataSet();
            DataSet dsCRNList = new DataSet();
            DataSet dsAdjustments = new DataSet();
            DataSet dsTransfers = new DataSet();
            DataSet dsReceipts = new DataSet();
            // DeleteItemTemp();
            try
            {
                if (chkCustomerMasters.Checked == true)
                {
                    if (txtCustomer.Text.ToString().Trim().Length > 0 && txtCustomerPath.Text.ToString().Trim().Length > 0)
                    {
                        dsCustomer = Get_CustomerMaster_From_CSV_To_Dataset(txtCustomer.Text.ToString().Trim(), txtCustomerPath.Text.ToString().Trim());

                        if (dsCustomer.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Customer data", "No data in csv file");
                            return;
                        }
                    }
                }

                if (chkInvoices.Checked == true)
                {
                    if (txtInvoices.Text.ToString().Trim() != string.Empty && txtInvoicesPath.Text.ToString().Trim() != string.Empty)
                    {
                        dsInvoiceList = Get_Invoices_From_CSV_To_Dataset(txtInvoices.Text.ToString().Trim(),
                            txtInvoicesPath.Text.ToString().Trim());

                        if (dsInvoiceList.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Invoice data", "No Data in csv file");
                            return;
                        }
                    }
                }

                if (chkCreditNote.Checked)
                {
                    if (txtCrteditNotes.Text.ToString().Trim() != string.Empty && txtCreditPath.Text.ToString().Trim() != string.Empty)
                    {
                        dsCRNList = Get_CreditNotes_From_CSV_To_Dataset(txtCrteditNotes.Text.ToString().Trim(), txtCreditPath.Text.ToString().Trim());

                        if (dsCRNList.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Credit Note Data", "No data in csv file");
                            return;
                        }
                    }
                }

                if (chkInvAdjustments.Checked)
                {
                    if (txtInvAdjustment.Text.ToString().Trim() != string.Empty && txtAdjustPath.Text.ToString().Trim() != string.Empty)
                    {
                        dsAdjustments = Get_Adjustments_From_CSV_To_Dataset(txtInvAdjustment.Text.ToString().Trim(), txtAdjustPath.Text.ToString().Trim());

                        if (dsAdjustments.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Credit Note Data", "No data in csv file");
                            return;
                        }
                    }
                }

                if (chkTransfers.Checked)
                {
                    if (txtTransferNotes.Text.ToString().Trim() != string.Empty && txtTransPath.Text.ToString().Trim() != string.Empty)
                    {
                        dsTransfers = Get_Adjustments_From_CSV_To_Dataset(txtTransferNotes.Text.ToString().Trim(), txtTransPath.Text.ToString().Trim());

                        if (dsTransfers.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Transfer Note Data", "No data in csv file");
                            return;
                        }
                    }
                }


                if (chkReceipts.Checked)
                {
                    if (txtReceipts.Text.ToString().Trim() != string.Empty && txtReceiptPath.Text.ToString().Trim() != string.Empty)
                    {
                        //ddss
                        dsReceipts = GetReceipts_From_CSV_To_Dataset(txtReceipts.Text.ToString().Trim(), txtReceiptPath.Text.ToString().Trim());

                        if (dsReceipts.Tables.Count == 0)
                        {
                            MessageBox.Show("Can not file the Receips Note Data", "No data in csv file");
                            return;
                        }
                    }
                }
                InsertIntoLocalDB(dsInvoiceList, dsCustomer, dsCRNList, dsTransfers, dsAdjustments, dsAdjustments, dsReceipts);
                SendPeachtree_Customer();
                SendPeachtree_Invoice();
                SendPeachtree_CreditNote();
                SendPeachtree_Adjustment();
                //  SendPeachtree_Receipts();
                SendPeachtree_ReceiptsJournal();

                MessageBox.Show("Uploaded Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertIntoLocalDB(DataSet _dsInvoice, DataSet _dsCustomer, DataSet _dsCrNote, DataSet _dsTrans, DataSet _dsAdjustment, DataSet _dsReceipt, DataSet _dsReceipts)
        {
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Trans = Con.BeginTransaction();

                //customer

                if (chkCustomerMasters.Checked)
                {
                    if (_dsCustomer.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsCustomer.Tables[0].Rows)
                        {
                            if (IsCustomerExists(Dr["CutomerID"].ToString().Replace("'", "").Trim()) == 0)
                            {
                                if (SaveCustomerMaster(Dr["CutomerID"].ToString().Replace("'", "").Trim(), Dr["CustomerName"].ToString().Replace("'", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating Customer Master local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                            }
                        }
                    }
                }

                //invoice
                if (chkInvoices.Checked)
                {
                    if (_dsInvoice.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsInvoice.Tables[0].Rows)
                        {
                            if (IsInvoiceExists(Dr["InvoiceNo"].ToString(), Convert.ToInt32(Dr["DistributionNo"])) == 0)
                            {
                                if (SaveSalesInvoice(Dr["InvoiceNo"].ToString(), Dr["CustomerID"].ToString().Replace("'", "").Trim(), Dr["DeliveryNoteNos"].ToString(),
                                    Convert.ToDateTime(Dr["InvoiceDate"]), Dr["ARAccount"].ToString(), Convert.ToInt32(Dr["NoofDistributions"]), Convert.ToInt32(Dr["DistributionNo"]),
                                    Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Qty"]), Dr["Description"].ToString(), Dr["GLAcount"].ToString(), Convert.ToDouble(Dr["UnitPrice"]), Convert.ToDouble(Dr["Discount"]),
                                    Convert.ToDouble(Dr["Amount"]), Convert.ToDouble(Dr["DiscountAmount"]), Convert.ToDouble(Dr["Tax1Amount"]), Convert.ToDouble(Dr["Tax2Amount"]), Convert.ToDouble(Dr["GrossTotal"]), Convert.ToDouble(Dr["NetTotal"]), DateTime.Now,
                                 "Import", '0', Dr["CustomerPO"].ToString(), Dr["UOM"].ToString(), Dr["JobID"].ToString(), Dr["SONO"].ToString(), Dr["Location"].ToString().Replace("_", "").Trim(), Dr["TTType1"].ToString(), Dr["TTType2"].ToString(), '0', Convert.ToDouble(Dr["Tax1Rate"]), Convert.ToDouble(Dr["Tax2Rate"]), Dr["SalesRep"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["CostPrrice"]), Dr["PaymentM"].ToString().Replace("'", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating Invoice local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (SaveItemActivity(Dr["InvoiceNo"].ToString(), Convert.ToDateTime(Dr["InvoiceDate"]),
                                    Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Qty"]), Convert.ToDouble(Dr["CostPrrice"]), Dr["Location"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["UnitPrice"])) == 0)
                                {
                                    MessageBox.Show("Error found while updating Item Activity local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (UpdateOnHandQtyINV(Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Qty"]), Dr["Location"].ToString().Replace("_", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating OnHand Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                            }
                        }
                    }
                }

                //crn
                if (chkCreditNote.Checked)
                {
                    if (_dsCrNote.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsCrNote.Tables[0].Rows)
                        {
                            if (IsCreditNoteExists(Dr["CreditNo"].ToString().Replace("'", "").Trim(), Dr["DistributionNo"].ToString()) == 0)
                            {

                                if (SaveCreditNotes(Dr["CustomerID"].ToString().Replace("'", "").Trim(), Dr["CreditNo"].ToString().Replace("'", "").Trim(),
                                    Convert.ToDateTime(Dr["ReturnDate"]), Dr["LocationID"].ToString().Replace("_", "").Trim(), Convert.ToBoolean(Dr["IsApplyToInvoice"]), Dr["InvoiceNO"].ToString(),
                                    Dr["ARAccount"].ToString().Replace("'", "").Trim(), Dr["NoofDistribution"].ToString(), Dr["DistributionNo"].ToString(), Dr["ItemID"].ToString(), Convert.ToDouble(Dr["InvoiceQty"]), Convert.ToDouble(Dr["ReturnQty"]),
                                    Dr["Description"].ToString(), Dr["UOM"].ToString(), Convert.ToDouble(Dr["UnitPrice"]), Convert.ToDouble(Dr["Discount"]), Convert.ToDouble(Dr["Amount"]), Dr["GL_Account"].ToString(),
                                   Convert.ToDouble(Dr["NBT"]), Convert.ToDouble(Dr["VAT"]), Convert.ToDouble(Dr["GrossTotal"]), Convert.ToDouble(Dr["GrandTotal"]), Convert.ToBoolean(Dr["ISExport"]), Dr["CurrenUser"].ToString().Replace("'", "").Trim(), Convert.ToBoolean(Dr["IsFullInvReturn"]), Dr["JobID"].ToString().Replace("'", "").Trim(), Dr["Tax1ID"].ToString().Replace("'", "").Trim(), Dr["Tax2ID"].ToString().Replace("'", "").Trim(), Dr["Type"].ToString().Replace("'", "").Trim(), Dr["SalesRep"].ToString().Replace("_", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating Credit Note local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (SaveItemActivityCreditNote(Dr["CreditNo"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["ReturnDate"]),
                                    Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["ReturnQty"]), Convert.ToDouble(Dr["UnitPrice"]), Dr["LocationID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["UnitPrice"])) == 0)
                                {
                                    MessageBox.Show("Error found while updating Item Activity For Credit Note local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (UpdateOnHandQtyCreditNote(Dr["ItemID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["ReturnQty"]), Dr["LocationID"].ToString().Replace("_", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating OnHand for Credit Note Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                            }
                        }
                    }
                }
                //Adjustments

                if (chkInvAdjustments.Checked)
                {
                    if (_dsAdjustment.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsAdjustment.Tables[0].Rows)
                        {
                            if (IsAdjustmentExists(Dr["AdjusmentId"].ToString().Replace("'", "").Trim()) == 0)
                            {
                                if (SaveAdjustments(Dr["AdjusmentId"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["Date"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim(), Dr["WarehouseName"].ToString().Replace("'", "").Trim(), Dr["ItemID"].ToString().Replace("_", "").Trim(), Dr["Itemdescription"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["UnitCost"]), Convert.ToDouble(Dr["OnhandQty"]), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["NewQty"]), Dr["ReasonToAdjust"].ToString().Replace("'", "").Trim(), Dr["GLAccount"].ToString().Replace("'", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating Inventory Adjustments local database", "Inventory Adjustments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (SaveItemActivityAdjustment(Dr["AdjusmentId"].ToString(), Convert.ToDateTime(Dr["Date"]),
                                       Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Convert.ToDouble(Dr["UnitCost"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating Item Activity Inventory Adjustments local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                                if (UpdateOnHandAdjustment(Dr["ItemID"].ToString().Replace("_", "").Trim(), Convert.ToDouble(Dr["AdjustQty"]), Dr["WarehouseId"].ToString().Replace("_", "").Trim()) == 0)
                                {
                                    MessageBox.Show("Error found while updating OnHand for inventory Adjustments Table", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Trans.Rollback();
                                    return;
                                }
                            }
                        }
                    }
                }
                if (chkReceipts.Checked)
                {
                    DeleteItemTemp();
                    //Insert Data into tempory table
                    if (_dsReceipts.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsReceipts.Tables[0].Rows)
                        {

                            if (SaveReceiptInTemporyTable(Dr["Customer ID"].ToString().Replace("'", "").Trim(), Dr["Reference"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["Date"]),
                                Dr["Payment Method"].ToString().Replace("'", "").Trim(), Dr["Cash Account"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Number of Distributions"]), '1',
                                Dr["Invoice Paid"].ToString().Replace("'", "").Trim(), Dr["Item ID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Quantity"]), Dr["Description"].ToString().Replace("'", "").Trim(),
                                Dr["G/L Account"].ToString().Replace("'", "").Trim(), Dr["Tax Type"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Amount"]), Dr["Receipt Number"].ToString().Replace("'", "").Trim(), Dr["Sales Representative ID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Total Paid on Invoice(s)"])) == 0)
                            {
                                MessageBox.Show("Error found while updating ReceiptData to receipt temp local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Trans.Rollback();
                                return;
                            }
                        }
                    }


                    if (_dsReceipts.Tables.Count > 0)
                    {
                        foreach (DataRow Dr in _dsReceipts.Tables[0].Rows)
                        {
                            //polo
                            if (IsReceiptsExsits(Dr["Reference"].ToString()) == 0)
                            {
                                //if (IsReceiptsExsitsinInvoiceTrans(Dr["Receipt Number"].ToString()) == 0)
                                //{
                                SQL = "SELECT Reference,[Receipt Number] FROM tblTempReceiptUpload WHERE Reference='" + Dr["Reference"].ToString() + "'";
                                CMD = new SqlCommand(SQL, Con, Trans);
                                CMD.CommandType = CommandType.Text;
                                DA = new SqlDataAdapter(CMD);
                                DS = new DataSet();
                                DA.Fill(DS);
                                foreach (DataRow dr2 in DS.Tables[0].Rows)
                                {
                                    //cono
                                    if (IsReceiptsExsitsinInvoiceTrans(dr2["Receipt Number"].ToString()) == 0)
                                    {

                                        if (SaveReceiptUpload(Dr["Customer ID"].ToString().Replace("'", "").Trim(), Dr["Reference"].ToString().Replace("'", "").Trim(), Convert.ToDateTime(Dr["Date"]),
                                            Dr["Payment Method"].ToString().Replace("'", "").Trim(), Dr["Cash Account"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Number of Distributions"]), '1',
                                            Dr["Invoice Paid"].ToString().Replace("'", "").Trim(), Dr["Item ID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Quantity"]), Dr["Description"].ToString().Replace("'", "").Trim(),
                                            Dr["G/L Account"].ToString().Replace("'", "").Trim(), Dr["Tax Type"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Amount"]), Dr["Receipt Number"].ToString().Replace("'", "").Trim(), Dr["Sales Representative ID"].ToString().Replace("'", "").Trim(), Convert.ToDouble(Dr["Total Paid on Invoice(s)"])) == 0)
                                        {
                                            MessageBox.Show("Error found while updating Receipt Journal to Original Receipt local database", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            Trans.Rollback();
                                            return;
                                        }
                                    }
                                }
                                // }
                            }
                            //  }
                        }
                    }
                }
                Trans.Commit();
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                throw ex;
            }
        }

        private void SendPeachtree_Customer()
        {
            try
            {
                if (chkCustomerMasters.Checked)
                {
                    Connector ImportsInvoices = new Connector();
                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Trans = Con.BeginTransaction();
                    //Customer
                    SQL = "SELECT CutomerID,CustomerName from tblCustomerMaster where IsExport=1";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);
                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {
                        if (ExportCustomerMaster(dr) > 0)
                        {
                            if (ImportsInvoices.ImportCustomerMaster_return() > 0)
                                UpdateStatusCustomer(dr["CutomerID"].ToString());
                        }
                    }
                    //UpdateStatus();              
                    Trans.Commit();
                    //MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show("Error in Customer Import.....!");
            }
        }

        private void SendPeachtree_Invoice()
        {
            try
            {

                if (chkInvoices.Checked)
                {
                    Connector ImportsInvoices = new Connector();

                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Trans = Con.BeginTransaction();

                    //invoice
                    SQL = "SELECT DISTINCT InvoiceNo, CustomerID, InvoiceDate, ARAccount, NoofDistributions, IsExport, CustomerPO, JobID, SONO, Location,Tax1Amount, Tax2Amount,PaymentM,SalesRep FROM tblSalesInvoices WHERE IsExport=0 and Status=1";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {

                        if (dr["PaymentM"].ToString() == "Credit")
                        {

                            if (ExportInvoice(dr) > 0)
                            {
                                if (ImportsInvoices.ImportCustomerInvoices_Return() > 0)
                                    UpdateStatus(dr["InvoiceNo"].ToString());
                            }
                        }
                        if (dr["PaymentM"].ToString() == "Cash")
                        {
                            if (ExportReceipts(dr) > 0)
                            {
                                if (ImportsInvoices.ImportReceiptJ_BatchMode() > 0)
                                    UpdateStatus(dr["InvoiceNo"].ToString());
                            }
                        }
                    }

                    Trans.Commit();
                    //MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Con.Close();
                }
            }
            catch  (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show("Error in Invoice Import.....!");
            }
        }


        private void SendPeachtree_ReceiptsJournal()
        {
            try
            {

                if (chkReceipts.Checked)
                {
                    Connector ImportsInvoices = new Connector();

                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Trans = Con.BeginTransaction();

                    //Receipts
                    SQL = "SELECT DISTINCT [Customer ID],Reference,RDate,[Payment Method],[Cash Account],[Number of Distributions],[Invoice Paid],[Sales Representative ID],[Receipt Number],[Total Paid on Invoice(s)] FROM tblReceiptUpload WHERE Status=1";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow Dr in DS.Tables[0].Rows)
                    {

                        //if (dr["Invoice Paid"].ToString() != string.Empty)
                        //{
                        //mono
                        if (ExportReceiptsJornal(Dr) > 0)
                        {
                            if (ImportsInvoices.ImportReceiptJ_Imp() > 0)
                                UpdateStatusReceipts(Dr["Reference"].ToString());
                        }
                        //}
                        //if (dr["PaymentM"].ToString() == "Cash")
                        //{
                        //    if (ExportReceipts(dr) > 0)
                        //    {
                        //        if (ImportsInvoices.ImportReceiptJ_BatchMode() > 0)
                        //            UpdateStatus(dr["InvoiceNo"].ToString());
                        //    }
                        //}
                    }

                    Trans.Commit();
                    //MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Con.Close();
                }
            }
            catch  (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show("Error in Invoice Import.....!");
            }
        }

        private void SendPeachtree_Adjustment()
        {
            try
            {
                if (chkInvAdjustments.Checked)
                {
                    Connector ImportsInvoices = new Connector();

                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Trans = Con.BeginTransaction();

                    //Ajstment
                    SQL = "SELECT AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,AdjustQty,ReasonToAdjust,GLAccount from tblInventoryAdjustment where Status=1";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {
                        if (ExportInventoryAdjustments(dr) > 0)
                        {
                            ImportsInvoices.InventoryAdjustmentExport();
                            UpdateStatusAdjustment(dr["AdjusmentId"].ToString());
                        }

                    }
                    //UpdateStatusCredit();                 

                    Trans.Commit();
                    //MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void SendPeachtree_CreditNote()
        {
            try
            {
                if (chkCreditNote.Checked)
                {
                    Connector ImportsInvoices = new Connector();

                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Trans = Con.BeginTransaction();


                    //crn
                    SQL = "SELECT DISTINCT CustomerID, CreditNo, ReturnDate, InvoiceNO, ARAccount, NoofDistribution,JobID FROM tblCutomerReturn WHERE Status =1 ";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow dr in DS.Tables[0].Rows)
                    {
                        if (ExportCreditNote(dr) == 1)
                        {
                            ImportsInvoices.ImportCustomerReturn();
                            UpdateStatusCredit(dr["CreditNo"].ToString());
                        }
                    }

                    Trans.Commit();
                    //MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Con.Close();
                }
            }
            catch  (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show("Error in Customer Return Import.....!");
            }
        }


        private void SendPeachtree_Receipts()
        {
            try
            {
                if (chkReceipts.Checked)
                {
                    //Connector ImportsInvoices = new Connector();
                    //ImportsInvoices.Upload_ReceiptJ_BatchMode(txtReceiptPath.Text.ToString().Trim() + "\\" + txtReceipts.Text.ToString());

                    //Con = new SqlConnection(ConnectionString);
                    //Con.Open();
                    //Trans = Con.BeginTransaction();


                    ////crn
                    //SQL = "SELECT DISTINCT CustomerID, CreditNo, ReturnDate, InvoiceNO, ARAccount, NoofDistribution,JobID FROM tblCutomerReturn WHERE Status =1 ";
                    //CMD = new SqlCommand(SQL, Con, Trans);
                    //CMD.CommandType = CommandType.Text;
                    //DA = new SqlDataAdapter(CMD);
                    //DS = new DataSet();
                    //DA.Fill(DS);

                    //foreach (DataRow dr in DS.Tables[0].Rows)
                    //{
                    //    if (ExportCreditNote(dr) == 1)
                    //    {
                    //        ImportsInvoices.ImportCustomerReturn();
                    //        UpdateStatusCredit(dr["CreditNo"].ToString());
                    //    }
                    //}

                    //Trans.Commit();
                    ////MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Con.Close();
                }
            }
            catch  (Exception ex)
            {
                Trans.Rollback();
                Con.Close();
                MessageBox.Show("Error in Customer Return Import.....!");
            }
        }


        //  conn.Upload_ReceiptJ_BatchMode();



        //private void SendPeachtree()
        //{
        //    try
        //    {
        //        Connector ImportsInvoices = new Connector();

        //        Con = new SqlConnection(ConnectionString);
        //        Con.Open();
        //        Trans = Con.BeginTransaction();

        //        //Customer
        //        SQL = "SELECT CutomerID,CustomerName from tblCustomerMaster";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        DA = new SqlDataAdapter(CMD);
        //        DS = new DataSet();
        //        DA.Fill(DS);

        //        if (ExportCustomerMaster(DS.Tables[0].Rows[0]) > 0)
        //        {
        //            ImportsInvoices.ImportCustomerMaster();
        //        }
        //        //UpdateStatus(); 

        //        //invoice
        //        SQL = "SELECT DISTINCT InvoiceNo, CustomerID, InvoiceDate, ARAccount, NoofDistributions, IsExport, CustomerPO, JobID, SONO, Location,Tax1Amount, Tax2Amount FROM tblSalesInvoices WHERE IsExport = 0 AND Status=1";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        DA = new SqlDataAdapter(CMD);
        //        DS = new DataSet();
        //        DA.Fill(DS);

        //        if (ExportInvoice(DS.Tables[0].Rows[0]) > 0)
        //        {
        //            if(ImportsInvoices.ImportCustomerInvoices_Return()>0)
        //                UpdateStatus(DS.Tables[0].Rows[0]["InvoiceNo"].ToString());
        //        }

        //        //crn
        //        SQL = "SELECT DISTINCT CustomerID, CreditNo, ReturnDate, InvoiceNO, ARAccount, NoofDistribution,JobID FROM tblCutomerReturn WHERE ISExport = 0 order by CreditNo";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        DA = new SqlDataAdapter(CMD);
        //        DS = new DataSet();
        //        DA.Fill(DS);

        //        if (ExportCreditNote(DS.Tables[0].Rows[0]) > 0)
        //        {
        //            if(ImportsInvoices.ImportCustomerReturn() > 0)
        //                UpdateStatusCredit(DS.Tables[0].Rows[0]["CreditNo"].ToString());
        //        }

        //        //Ajstment
        //        SQL = "SELECT AdjusmentId,Date,WarehouseId,WarehouseName,ItemID,Itemdescription,UnitCost,AdjustQty,ReasonToAdjust,GLAccount from tblInventoryAdjustment";
        //        CMD = new SqlCommand(SQL, Con, Trans);
        //        CMD.CommandType = CommandType.Text;
        //        DA = new SqlDataAdapter(CMD);
        //        DS = new DataSet();
        //        DA.Fill(DS);

        //        if(ExportInventoryAdjustments(DS.Tables[0].Rows[0])>0)
        //            ImportsInvoices.InventoryAdjustmentExport();
        //        //UpdateStatusCredit();                 

        //        Trans.Commit();
        //        MessageBox.Show("Files Uploaded Successfully", "Peachtree", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        Con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Trans.Rollback();
        //        Con.Close();
        //        throw ex;
        //    }
        //}        

        private DataSet Get_CustomerMaster_From_CSV_To_Dataset(string FileName, string FilePath)
        {
            DataSet dsExcel = new DataSet();
            try
            {
                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }

        public int SaveCustomerMaster(string StrCusid, string StrCusname)
        {
            try
            {
                string SQL = "  INSERT INTO tblCustomerMaster(CutomerID,CustomerName,IsExport) VALUES('" + StrCusid + "','" + StrCusname + "',0)";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }


        public int SaveReceiptUpload(string StrCusid, string StrReference, DateTime DTDate, string StrPayM, string StrCashAccnt, double dblNoDis,
                                     int dblDisNo, string StrInvoicePaid, string StrItemID, double dblQty, string StrDescription, string StrGL,
                                     string StrTaxType, double dblAmount, string StrReceiptNO, string StrSalesRep, double dblTotalPaidONInvoices)
        {
            try
            {

                string SQL = "INSERT INTO [tblReceiptUpload]([Customer ID],[Reference],[RDate],[Payment Method],[Cash Account],[Number of Distributions]" +
              ",[DistributionNO],[Invoice Paid],[Quantity],[Item ID],[Description],[G/L Account],[Tax Type]" +
             ",[Amount],[Receipt Number],[Sales Representative ID],[Total Paid on Invoice(s)],Status)" +
               " VALUES('" + StrCusid + "','" + StrReference + "','" + DTDate.ToString("MM/dd/yyyy") + "','" + StrPayM + "','" + StrCashAccnt + "','" + dblNoDis + "','" + dblDisNo + "'," +
                 "'" + StrInvoicePaid + "','" + dblQty + "','" + StrItemID + "','" + StrDescription + "','" + StrGL + "','" + StrTaxType + "','" + dblAmount + "'," +
                "'" + StrReceiptNO + "','" + StrSalesRep + "','" + dblTotalPaidONInvoices + "','1')";


                //  string SQL = "  INSERT INTO tblCustomerMaster(CutomerID,CustomerName,IsExport) VALUES('" + StrCusid + "','" + StrCusname + "',0)";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }


        public int SaveReceiptInTemporyTable(string StrCusid, string StrReference, DateTime DTDate, string StrPayM, string StrCashAccnt, double dblNoDis,
                                     int dblDisNo, string StrInvoicePaid, string StrItemID, double dblQty, string StrDescription, string StrGL,
                                     string StrTaxType, double dblAmount, string StrReceiptNO, string StrSalesRep, double dblTotalPaidONInvoices)
        {
            try
            {

                string SQL = "INSERT INTO [tblTempReceiptUpload]([Customer ID],[Reference],[RDate],[Payment Method],[Cash Account],[Number of Distributions]" +
              ",[DistributionNO],[Invoice Paid],[Quantity],[Item ID],[Description],[G/L Account],[Tax Type]" +
             ",[Amount],[Receipt Number],[Sales Representative ID],[Total Paid on Invoice(s)],Status)" +
               " VALUES('" + StrCusid + "','" + StrReference + "','" + DTDate.ToString("MM/dd/yyyy") + "','" + StrPayM + "','" + StrCashAccnt + "','" + dblNoDis + "','" + dblDisNo + "'," +
                 "'" + StrInvoicePaid + "','" + dblQty + "','" + StrItemID + "','" + StrDescription + "','" + StrGL + "','" + StrTaxType + "','" + dblAmount + "'," +
                "'" + StrReceiptNO + "','" + StrSalesRep + "','" + dblTotalPaidONInvoices + "','1')";


                //  string SQL = "  INSERT INTO tblCustomerMaster(CutomerID,CustomerName,IsExport) VALUES('" + StrCusid + "','" + StrCusname + "',0)";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }




        private int ExportCustomerMaster(DataRow Dr)
        {
            try
            {
                ////SQL = "SELECT CutomerID CustomerName from tblCustomerMaster where Satus=1";
                //SQL = "SELECT CutomerID,CustomerName from tblCustomerMaster";
                //CMD = new SqlCommand(SQL, Con, Trans);
                //CMD.CommandType = CommandType.Text;
                //DA = new SqlDataAdapter(CMD);
                //DS = new DataSet();
                //DA.Fill(DS);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerMaster.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Customers");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                //foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Customer");
                    Writer.WriteAttributeString("xsi:type", "paw:customer");

                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CutomerID"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Name");
                    Writer.WriteString(Dr["CustomerName"].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Customer_Type");
                    Writer.WriteString("Branch");
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();
                    // Writer.WriteEndElement();
                }
                Writer.WriteEndElement();
                Writer.Close();

                return 1;
            }
            catch  (Exception ex)
            {
                return 0;
            }
        }


        public int IsReceiptsExsitsinInvoiceTrans(string ReceiptsNO)
        {
            try
            {
                SQL = "Select InvoiceNo from tblSalesInvoices where InvoiceNo='" + ReceiptsNO + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }




        public int IsReceiptsExsits(string ReceiptsNO)
        {
            try
            {
                SQL = "Select Reference from tblReceiptUpload where Reference='" + ReceiptsNO + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }
        public int IsCustomerExists(string CustomerID)
        {
            try
            {
                SQL = "Select CutomerID from tblCustomerMaster where CutomerID='" + CustomerID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }

        public int SaveCreditNotes(string CustomerID, string CreditNo, DateTime ReturnDate,
           string LocationID, bool IsApplyToInvoice, string InvoiceNO, string ARAccount, string NoofDistribution, string DistributionNo, string ItemID, double InvoiceQty,
            double ReturnQty, string Description, string UOM, double UnitPrice, double Discount, double Amount, string GL_Account,
            double NBT, double VAT, double GrossTotal, double GrandTotal, bool ISExport, string CurrenUser, bool IsFullInvReturn, string JobID,
            string Tax1ID, string Tax2ID, string Type, string SalesRep)
        {
            try
            {
                string SQL = " INSERT INTO tblCutomerReturn(CustomerID, CreditNo, ReturnDate, LocationID, " +
                    " IsApplyToInvoice, InvoiceNO, ARAccount, NoofDistribution, DistributionNo, ItemID, InvoiceQty, ReturnQty, " +
                    " Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT," +
                    " GrossTotal, GrandTotal, ISExport, CurrenUser, IsFullInvReturn, JobID, Tax1ID, Tax2ID, Type,SalesRep,Status)VALUES('" + CustomerID + "'," +
                    " '" + CreditNo + "','" + Convert.ToDateTime(ReturnDate).ToString("MM/dd/yyyy") + "','" + LocationID + "','" + IsApplyToInvoice + "'," +
                    " '" + InvoiceNO + "','" + ARAccount + "','" + NoofDistribution + "','" + DistributionNo + "'," +
                    " '" + ItemID + "','" + InvoiceQty + "','" + ReturnQty + "','" + Description + "','" + UOM + "'," +
                    " '" + UnitPrice + "','" + Discount + "','" + Amount + "'," +
                    " '" + GL_Account + "','" + NBT + "','" + VAT + "','" + GrossTotal + "'," +
                    " '" + GrandTotal + "','" + ISExport + "','" + CurrenUser + "','" + IsFullInvReturn + "','" + JobID + "','" + Tax1ID + "'," +
                    " '" + Tax2ID + "','" + Type + "','" + SalesRep + "','1')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }

        public int SaveItemActivityCreditNote(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH, double SellingPrice)
        {
            try
            {
                string SQL = "INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('5', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'CreditNote','1','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "'," + SellingPrice + ")";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int UpdateOnHandQtyCreditNote(string ItemID, double Qty, string WH)
        {
            try
            {
                string SQL = "UPDATE tblItemWhse SET QTY=QTY+" + Qty + " WHERE WhseId='" + WH + "' AND ItemId='" + ItemID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int ExportCreditNote(DataRow Dr)
        {
            try
            {
                string InvCreditAcc = string.Empty;
                string InvDebitAcc = string.Empty;

                SQL = "Select CusretnDrAc,CusretnCrAc from tblDefualtSetting";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();

                foreach (DataRow _Dr in DS.Tables[0].Rows)
                {
                    InvCreditAcc = _Dr["CusretnDrAc"].ToString();
                    InvDebitAcc = _Dr["CusretnCrAc"].ToString();
                }




                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                //Writer.WriteStartElement("PAW_Invoice");
                //Writer.WriteAttributeString("xsi:type", "paw:invoice");

                //foreach (DataRow Dr in DS.Tables[0].Rows)
                {

                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:invoice");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CustomerID"].ToString());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(Dr["CreditNo"].ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Date");
                    Writer.WriteString(Convert.ToDateTime(Dr["ReturnDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteString(Dr["ARAccount"].ToString());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("TRUE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(Dr["NoofDistribution"].ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesLines");

                    SQL = "SELECT DistributionNo, ItemID, InvoiceQty,ReturnQty, Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT,ISExport, Tax1ID,Tax2ID,JobID FROM tblCutomerReturn WHERE CreditNo ='" + Dr["CreditNo"].ToString().Trim() + "' AND Status=1";
                    // SQL = "SELECT DistributionNo, ItemID, InvoiceQty,ReturnQty, Description, UOM, UnitPrice, Discount, Amount, GL_Account, NBT, VAT,ISExport, Tax1ID,Tax2ID,JobID FROM tblCutomerReturn WHERE ISExport = 0 order by CreditNo";

                    //WHERE CreditNo ='" + Dr["CreditNo"].ToString().Trim() + "' AND Status=1
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow Dr1 in DS.Tables[0].Rows)
                    {
                        //if (Convert.ToDouble(Dr1["ReturnQty"]) > 0)
                        //{
                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(Dr1["DistributionNo"].ToString());
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("-" + Dr1["ReturnQty"].ToString());//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Dr1["ItemID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Dr1["Description"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(Dr1["GL_Account"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(Dr1["UnitPrice"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Dr1["Amount"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Job_ID");
                        Writer.WriteString(Dr1["JobID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToSO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                        //  }
                    }
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();//last line
                Writer.Close();



                return 1;
            }
            catch  (Exception ex)
            {
                return 0;
            }

        }

        private void UpdateStatusCredit(string _CrnNo)
        {
            try
            {
                SQL = "UPDATE    tblCutomerReturn SET Status = 2 WHERE Status = 1 and CreditNo='" + _CrnNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int IsInvoiceExists(string InvoiceNo, int DistributionNo)
        {
            try
            {

                SQL = "Select InvoiceNo from tblSalesInvoices where InvoiceNo='" + InvoiceNo + "' and DistributionNo='" + DistributionNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                // Con.Open();
                DA.Fill(DS);
                // Con.Close();
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }

        public int IsAdjustmentExists(string Adjustmnet)
        {
            try
            {
                SQL = "Select AdjusmentId from tblInventoryAdjustment where AdjusmentId='" + Adjustmnet + "' ";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int IsCreditNoteExists(string CreditNoteNo, string DistributionNo)
        {
            try
            {
                SQL = "Select CreditNo from tblCutomerReturn where CreditNo='" + CreditNoteNo + "' and DistributionNo='" + DistributionNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                DA = new SqlDataAdapter(CMD);
                DS = new DataSet();
                DA.Fill(DS);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int SaveSalesInvoice(string InvoiceNo, string CustomerId, string DeliveryNoteNo, DateTime Date,
        string ARAccount, Int32 NoOfDis, Int32 DisNo, string ItemId, double Quantity, string SODescription,
       string GLAccount, double Price, double Discount, double Amount, double DisAmount, double Tax1AmountNBT,
        double Tax2AmountVAT, double GrossTotal, double NetTotal, DateTime CurrentDate, string CurrentUser, Int64 IsExport,
        string CustomerPO, string UM, string JoBID, string SONO, string Loc, string TTType1, string TTType2, Int64 IsReturn, double TaxRate1, double TaxRate2, string SalesRep, double CostPrice, string PaymentM)
        {
            try
            {
                //string SQL = "INSERT INTO tblSalesInvoices(InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, " +
                //    " ARAccount, NoofDistributions, DistributionNo, ItemID, Qty, Description, GLAcount, UnitPrice, " +
                //    " Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, CurrentDate," +
                //    " Currentuser, IsExport, CustomerPO, UOM, JobID, SONO, Location, TTType1, TTType2, IsReturn," +
                //    " Tax1Rate, Tax2Rate, SalesRep, CostPrrice, Status) VALUES('" + InvoiceNo + "'," +
                //    " '" + CustomerId + "','" + DeliveryNoteNo + "','" + Convert.ToDateTime(Date).ToString("MM/dd/yyyy") + "','" + ARAccount + "'," +
                //    " " + NoOfDis + "," + DisNo + ",'" + ItemId + "'," + Quantity + "," +
                //    " '" + SODescription + "','" + GLAccount + "'," + Price + "," + Discount + "," + Amount + "," +
                //    " " + DisAmount + "," + Tax1AmountNBT + "," + Tax2AmountVAT + "," +
                //    " " + GrossTotal + "," + NetTotal + ",'" + Convert.ToDateTime(CurrentDate).ToString("MM/dd/yyyy") + "','" + CurrentUser + "'," +
                //    " 0,'" + CustomerPO + "','" + UM + "','" + JoBID + "','" + SONO + "','" + Loc + "'," +
                //    " '" + TTType1 + "','" + TTType2 + "','0'," + TaxRate1 + "," + TaxRate2 + ",'" + SalesRep + "'," + CostPrice + ",'1')";
                //CMD = new SqlCommand(SQL, Con, Trans);
                //CMD.CommandType = CommandType.Text;
                //CMD.ExecuteNonQuery();
                //return 1;


                string SQL = "INSERT INTO tblSalesInvoices(InvoiceNo, CustomerID, DeliveryNoteNos, InvoiceDate, " +
                   " ARAccount, NoofDistributions, DistributionNo, ItemID, Qty, Description, GLAcount, UnitPrice, " +
                   " Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, CurrentDate," +
                   " Currentuser, IsExport, CustomerPO, UOM, JobID, SONO, Location, TTType1, TTType2, IsReturn," +
                   " Tax1Rate, Tax2Rate, SalesRep, CostPrrice, Status,SubValue,RemainQty,PaymentM) VALUES('" + InvoiceNo + "'," +
                   " '" + CustomerId + "','Import','" + Convert.ToDateTime(Date).ToString("MM/dd/yyyy") + "','" + ARAccount + "'," +
                   " " + NoOfDis + "," + DisNo + ",'" + ItemId + "'," + Quantity + "," +
                   " '" + SODescription + "','" + GLAccount + "'," + Price + "," + Discount + "," + Amount + "," +
                   " " + DisAmount + "," + Tax1AmountNBT + "," + Tax2AmountVAT + "," +
                   " " + GrossTotal + "," + NetTotal + ",'" + Convert.ToDateTime(CurrentDate).ToString("MM/dd/yyyy") + "','" + CurrentUser + "'," +
                   " 0,'" + CustomerPO + "','" + UM + "','" + JoBID + "','" + SONO + "','" + Loc + "'," +
                   " '" + TTType1 + "','" + TTType2 + "','0'," + TaxRate1 + "," + TaxRate2 + ",'" + SalesRep + "'," + CostPrice + ",'1','" + GrossTotal + "','" + Quantity + "','" + PaymentM + "')";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }

        public int SaveItemActivity(string TranNo, DateTime TransDate, string ItemID, double Qty, double Cost, string WH, double SellingPrice)
        {
            try
            {
                string SQL = "  INSERT INTO tbItemlActivity(DocType, TranNo, TransDate, TranType, DocReference, ItemID, Qty, UnitCost," +
                    " TotalCost, WarehouseID, SellingPrice) VALUES('4', '" + TranNo + "','" + Convert.ToDateTime(TransDate).ToString("MM/dd/yyyy") + "'," +
                    " 'Invoice','0','" + ItemID + "'," + Qty + ", " + Cost + "," + Qty * Cost + ",'" + WH + "'," + SellingPrice + ")";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
                return 1;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        //invoice setoff with peachtree

        private int ExportInvoice(DataRow Dr)
        {
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                //Writer.WriteStartElement("PAW_Invoice");
                //Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                //int NoofDis = 0;


                //foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");
                    int NoofDis = 0;


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CustomerID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(Dr["InvoiceNo"].ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Date");
                    Writer.WriteString(Convert.ToDateTime(Dr["InvoiceDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                    Writer.WriteEndElement();

                    NoofDis = Convert.ToInt32(Dr["NoofDistributions"]);
                    if (Convert.ToDouble(Dr["Tax1Amount"]) > 0)
                    {
                        NoofDis = NoofDis + 1;
                    }
                    if (Convert.ToDouble(Dr["Tax2Amount"]) > 0)
                    {
                        NoofDis = NoofDis + 1;
                    }
                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoofDis.ToString().Trim());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteString(Dr["ARAccount"].ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");//Cash Account
                    Writer.WriteEndElement();//CreditMemoType


                    Writer.WriteStartElement("SalesLines");

                    SQL = "SELECT DistributionNo, ItemID, Qty, Description, GLAcount, UnitPrice, Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, UOM, JobID, SONO, Location, TTType1, TTType2, IsReturn, TTType3, Tax3Amount, RemainQty, InvoiceNo FROM tblSalesInvoices WHERE InvoiceNo ='" + Dr["InvoiceNo"].ToString().Trim() + "' AND Status=1 ";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow Dr1 in DS.Tables[0].Rows)
                    {
                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(Dr1["Qty"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Dr1["ItemID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Dr1["Description"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(Dr1["GLAcount"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr1["UnitPrice"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Dr1["Amount"].ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Job_ID");
                        //Writer.WriteString(Dr1["JobID"].ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                        Writer.WriteString(Dr1["DistributionNo"].ToString());
                        Writer.WriteEndElement();


                        Writer.WriteEndElement();// end of sales line

                    }
                    //Check for NBT Amount 

                    if (Convert.ToDouble(Dr["Tax1Amount"]) > 0)
                    {
                        SQL = "SELECT Account FROM tblTaxApplicable WHERE TaxID ='NBT'";
                        CMD = new SqlCommand(SQL, Con, Trans);
                        CMD.CommandType = CommandType.Text;
                        string Tax1GLAccount = string.Empty;
                        Tax1GLAccount = (string)CMD.ExecuteScalar();



                        if (Tax1GLAccount != string.Empty)
                        {
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Apply_To_Sales_Order");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(Tax1GLAccount);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + Dr["Tax1Amount"].ToString());
                            Writer.WriteEndElement();
                            Writer.WriteEndElement();

                        }

                    }
                    if (Convert.ToDouble(Dr["Tax2Amount"]) > 0)
                    {
                        //Check for NBT Amount 
                        SQL = "SELECT Account FROM tblTaxApplicable WHERE TaxID ='VAT'";
                        CMD = new SqlCommand(SQL, Con, Trans);
                        CMD.CommandType = CommandType.Text;
                        string Tax2GLAccount = string.Empty;
                        Tax2GLAccount = (string)CMD.ExecuteScalar();


                        if (Tax2GLAccount != string.Empty)
                        {
                            Writer.WriteStartElement("SalesLine");
                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Apply_To_Sales_Order");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(Tax2GLAccount);
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + Dr["Tax2Amount"].ToString());
                            Writer.WriteEndElement();
                            Writer.WriteEndElement();
                        }

                    }
                    Writer.WriteEndElement();//End oF the Sales lines
                    Writer.WriteEndElement();//End oF the Sales lines
                }
                Writer.Close();
                //  }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        private int ExportReceipts(DataRow Dr)
        {
            try
            {
                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Receipts");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:receipt");
                    //int NoofDis = 0;

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["CustomerID"].ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(Dr["InvoiceNo"].ToString().Trim() + "R");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteString(Convert.ToDateTime(Dr["InvoiceDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Payment_Method");
                    Writer.WriteString("Cash");//PayMethod
                    Writer.WriteEndElement();


                    SQL = "SELECT CashAccount FROM tblWhseMaster WHERE WhseId ='" + Dr["Location"].ToString().Trim() + "'";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);
                    foreach (DataRow Dr2 in DS.Tables[0].Rows)
                    {
                        Writer.WriteStartElement("Cash_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr2["CashAccount"].ToString().Trim());//Cash Account
                        Writer.WriteEndElement();
                    }

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Dr["SalesRep"].ToString().Trim());//Cash Account
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReceiptNumber");
                    Writer.WriteString(Dr["InvoiceNo"].ToString().Trim());
                    Writer.WriteEndElement();
                    //NoofDistributions

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(Dr["NoofDistributions"].ToString().Trim());
                    Writer.WriteEndElement();


                    //Writer.WriteStartElement("Accounts_Receivable_Account");
                    //Writer.WriteString(Dr["ARAccount"].ToString().Trim());//Cash Account
                    //Writer.WriteEndElement();//CreditMemoType

                    //Writer.WriteStartElement("CreditMemoType");
                    //Writer.WriteString("FALSE");//Cash Account
                    //Writer.WriteEndElement();//CreditMemoType


                    //Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("Distributions");


                    SQL = "SELECT DistributionNo, ItemID, Qty, Description, GLAcount, UnitPrice, Discount, Amount, DiscountAmount, Tax1Amount, Tax2Amount, GrossTotal, NetTotal, UOM, JobID, SONO, Location, TTType1, TTType2, IsReturn, TTType3, Tax3Amount, RemainQty, InvoiceNo FROM tblSalesInvoices WHERE InvoiceNo ='" + Dr["InvoiceNo"].ToString().Trim() + "' AND Status=1 ";
                    CMD = new SqlCommand(SQL, Con, Trans);
                    CMD.CommandType = CommandType.Text;
                    DA = new SqlDataAdapter(CMD);
                    DS = new DataSet();
                    DA.Fill(DS);

                    foreach (DataRow Dr1 in DS.Tables[0].Rows)
                    {
                        // Writer.WriteStartElement("SalesLine");
                        Writer.WriteStartElement("Distribution");

                        Writer.WriteStartElement("InvoicePaid");
                        Writer.WriteString("");//PayMethod
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(Dr1["Qty"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Dr1["ItemID"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Dr1["Description"].ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(Dr1["GLAcount"].ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Unit_Price");
                        ////Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(Dr1["UnitPrice"].ToString());
                        //Writer.WriteEndElement();


                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Dr1["Amount"].ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Job_ID");
                        //Writer.WriteString(Dr1["JobID"].ToString());
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Tax_Type");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Apply_To_Sales_Order");
                        //Writer.WriteString("FALSE");
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("SalesOrderDistributionNumber");
                        //Writer.WriteString(Dr1["DistributionNo"].ToString());
                        //Writer.WriteEndElement();


                        Writer.WriteEndElement();// end of sales line

                    }
                    Writer.WriteEndElement();//End oF the Sales lines
                    Writer.WriteEndElement();//End oF the Sales lines
                }
                Writer.Close();
                //  }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }



        private string ValidateCustomer(string Cusid)
        {
            if (Cusid.Length == 1)
            {
                return "0000" + Cusid;
            }
            else  if (Cusid.Length == 2)
            {
                return "000" + Cusid;
            }
            else if (Cusid.Length == 3)
            {
                return "00" + Cusid;
            }
            else if (Cusid.Length == 4)
            {
                return "0" + Cusid;
            }
            else if (Cusid.Length > 4)
            {
                return Cusid;
            }
            else
            {
                return Cusid;
            }
        }

        private int ExportReceiptsJornal(DataRow Dr)
        {
            if (Dr["Invoice Paid"].ToString().Trim() == string.Empty)
            {
                try
                {
                    XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Receipts");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                    {


                        Writer.WriteStartElement("PAW_Receipt");
                        Writer.WriteAttributeString("xsi:type", "paw:receipt");
                        //int NoofDis = 0;

                        string CustomerID=ValidateCustomer(Dr["Customer ID"].ToString().Trim());

                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(CustomerID);//Vendor ID should be here = Ptient No
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Reference");
                        Writer.WriteString(Dr["Reference"].ToString().Trim()+"IMP");
                        Writer.WriteEndElement();

                        //DateTime DTP = Convert.ToDateTime(Dr["RDate"].ToString());
                        //string Dformat = "MM/dd/yyyy";
                        //string GRNDate = DTP.ToString(Dformat);


                        Writer.WriteStartElement("Date");
                        Writer.WriteString(Convert.ToDateTime(Dr["RDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                       // Writer.WriteString(GRNDate.Trim());//Date 

                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Payment_Method");
                        Writer.WriteString(Dr["Payment Method"].ToString().Trim());//PayMethod
                        Writer.WriteEndElement();


                        //SQL = "SELECT CashAccount FROM tblWhseMaster WHERE WhseId ='" + Dr["Location"].ToString().Trim() + "'";
                        //CMD = new SqlCommand(SQL, Con, Trans);
                        //CMD.CommandType = CommandType.Text;
                        //DA = new SqlDataAdapter(CMD);
                        //DS = new DataSet();
                        //DA.Fill(DS);
                        //foreach (DataRow Dr2 in DS.Tables[0].Rows)
                        //{
                        Writer.WriteStartElement("Cash_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr["Cash Account"].ToString().Trim());//Cash Account
                        Writer.WriteEndElement();
                        //}

                        Writer.WriteStartElement("Sales_Representative_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr["Sales Representative ID"].ToString().Trim());//Cash Account
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("ReceiptNumber");
                        Writer.WriteString(Dr["Receipt Number"].ToString().Trim());
                        Writer.WriteEndElement();
                        //NoofDistributions

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(Dr["Number of Distributions"].ToString().Trim());
                        Writer.WriteEndElement();


                        //Writer.WriteStartElement("Accounts_Receivable_Account");
                        //Writer.WriteString(Dr["ARAccount"].ToString().Trim());//Cash Account
                        //Writer.WriteEndElement();//CreditMemoType

                        //Writer.WriteStartElement("CreditMemoType");
                        //Writer.WriteString("FALSE");//Cash Account
                        //Writer.WriteEndElement();//CreditMemoType


                        //Writer.WriteStartElement("SalesLines");

                        Writer.WriteStartElement("Distributions");


                        SQL = "SELECT [Item ID], Quantity, Description, [G/L Account], Amount,[Invoice Paid] FROM tblReceiptUpload WHERE Reference ='" + Dr["Reference"].ToString().Trim() + "' AND Status=1 ";
                        CMD = new SqlCommand(SQL, Con, Trans);
                        CMD.CommandType = CommandType.Text;
                        DA = new SqlDataAdapter(CMD);
                        DS = new DataSet();
                        DA.Fill(DS);

                        foreach (DataRow Dr1 in DS.Tables[0].Rows)
                        {
                            // Writer.WriteStartElement("SalesLine");
                            Writer.WriteStartElement("Distribution");

                            Writer.WriteStartElement("InvoicePaid");
                            Writer.WriteString(Dr1["Invoice Paid"].ToString());//PayMethod
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString(Dr1["Quantity"].ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(Dr1["Item ID"].ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(Dr1["Description"].ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteString(Dr1["G/L Account"].ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Unit_Price");
                            ////Writer.WriteAttributeString("xsi:type", "paw:id");
                            //Writer.WriteString(Dr1["UnitPrice"].ToString());
                            //Writer.WriteEndElement();


                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(Dr1["Amount"].ToString());
                            Writer.WriteEndElement();


                            Writer.WriteEndElement();// end of sales line

                        }
                        Writer.WriteEndElement();//End oF the Sales lines
                        Writer.WriteEndElement();//End oF the Sales lines
                    }
                    Writer.Close();
                    //  }
                    return 1;
                }
                catch (Exception)
                {

                    return 0;
                }
            }
            else
            {
                //Set off with invoices--------------------------


                try
                {
                    XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml", System.Text.Encoding.UTF8);
                    Writer.Formatting = Formatting.Indented;
                    Writer.WriteStartElement("PAW_Receipts");
                    Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                    Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                    Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                    {
                        Writer.WriteStartElement("PAW_Receipt");
                        Writer.WriteAttributeString("xsi:type", "paw:receipt");
                        //int NoofDis = 0;

                        string CustomerID = ValidateCustomer(Dr["Customer ID"].ToString().Trim());


                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                      //  Writer.WriteString(Dr["Customer ID"].ToString().Trim());//Vendor ID should be here = Ptient No
                        Writer.WriteString(CustomerID);//Vendor ID should be here = Ptient No
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Reference");
                        Writer.WriteString(Dr["Reference"].ToString().Trim() + "IMP");
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Date");
                        //Writer.WriteString(Convert.ToDateTime(Dr["RDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                        //Writer.WriteEndElement();


                        //DateTime DTP = Convert.ToDateTime(Dr["RDate"].ToString());
                        //string Dformat = "MM/dd/yyyy";
                        //string GRNDate = DTP.ToString(Dformat);


                        Writer.WriteStartElement("Date");
                        Writer.WriteString(Convert.ToDateTime(Dr["RDate"]).ToString("MM/dd/yyyy").Trim());//Date 
                      // Writer.WriteString(GRNDate.Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Payment_Method");
                        Writer.WriteString("Payment Method");//PayMethod
                        Writer.WriteEndElement();


                        //SQL = "SELECT CashAccount FROM tblWhseMaster WHERE WhseId ='" + Dr["Location"].ToString().Trim() + "'";
                        //CMD = new SqlCommand(SQL, Con, Trans);
                        //CMD.CommandType = CommandType.Text;
                        //DA = new SqlDataAdapter(CMD);
                        //DS = new DataSet();
                        //DA.Fill(DS);
                        //foreach (DataRow Dr2 in DS.Tables[0].Rows)
                        //{
                        Writer.WriteStartElement("Cash_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr["Cash Account"].ToString().Trim());//Cash Account
                        Writer.WriteEndElement();
                        //}

                        Writer.WriteStartElement("Total_Paid_On_Invoices");
                        Writer.WriteString(Dr["Total Paid on Invoice(s)"].ToString().Trim());//PayMethod
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Sales_Representative_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Dr["Sales Representative ID"].ToString().Trim());//Cash Account
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("ReceiptNumber");
                        Writer.WriteString(Dr["Receipt Number"].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(Dr["Number of Distributions"].ToString().Trim());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("InvoicePaid");
                        //Writer.WriteString(Dr["Invoice Paid"].ToString());//PayMethod
                        //Writer.WriteEndElement();


                        Writer.WriteStartElement("Distributions");

                        Writer.WriteStartElement("Distribution");
                        SQL = "SELECT [Item ID], Quantity, Description, [G/L Account], Amount,[Invoice Paid] FROM tblReceiptUpload WHERE Reference ='" + Dr["Reference"].ToString().Trim() + "' AND Status=1 ";
                        CMD = new SqlCommand(SQL, Con, Trans);
                        CMD.CommandType = CommandType.Text;
                        DA = new SqlDataAdapter(CMD);
                        DS = new DataSet();
                        DA.Fill(DS);

                        foreach (DataRow Dr1 in DS.Tables[0].Rows)
                        {


                            Writer.WriteStartElement("InvoicePaid");
                            Writer.WriteString(Dr1["Invoice Paid"].ToString());//PayMethod
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(Dr1["Amount"].ToString());
                            Writer.WriteEndElement();


                        }


                        Writer.WriteEndElement();// end of sales line
                        Writer.WriteEndElement();//End oF the Sales lines
                        Writer.WriteEndElement();//End oF the Sales lines
                    }
                    Writer.Close();
                    //  }
                    return 1;
                }
                catch (Exception)
                {

                    return 0;
                }
                //----------------------------------------------
            }

        }

        private void UpdateStatusReceipts(string _ReceiptNo)
        {
            try
            {
                SQL = "UPDATE tblReceiptUpload SET Status = 2 WHERE Status = 1 and Reference='" + _ReceiptNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
            }
            catch  (Exception ex)
            {
            }
        }

        private void UpdateStatus(string _InvNo)
        {
            try
            {
                SQL = "UPDATE tblSalesInvoices SET Status = 2 WHERE Status = 1 and InvoiceNo='" + _InvNo + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
            }
            catch  (Exception ex)
            {
            }
        }
        private void UpdateStatusAdjustment(string _adustID)
        {
            try
            {
                SQL = "UPDATE tblInventoryAdjustment SET Status = 2 WHERE Status = 1 and AdjusmentId='" + _adustID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
            }
            catch  (Exception ex)
            {
            }
        }
        private void UpdateStatusCustomer(string _CustID)
        {
            try
            {
                SQL = "UPDATE tblCustomerMaster SET IsExport = 2 WHERE CutomerID='" + _CustID + "'";
                CMD = new SqlCommand(SQL, Con, Trans);
                CMD.CommandType = CommandType.Text;
                CMD.ExecuteNonQuery();
            }
            catch  (Exception ex)
            {
            }
        }

        private DataSet Get_CreditNotes_From_CSV_To_Dataset(string FileName, string FilePath)
        {
            DataSet dsExcel = new DataSet();
            try
            {

                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }

        private DataSet Get_Invoices_From_CSV_To_Dataset(string FileName, string FilePath)
        {
            DataSet dsExcel = new DataSet();
            try
            {

                string sConExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='text;HDR=YES;FMT=Delimited'";
                OleDbConnection ConExcel = new OleDbConnection(sConExcel);
                OleDbCommand cmdExcel = new OleDbCommand("SELECT * FROM " + FileName, ConExcel);
                OleDbDataAdapter daExcel = new OleDbDataAdapter(cmdExcel);
                ConExcel.Open();
                daExcel.Fill(dsExcel);
                ConExcel.Close();
                return dsExcel;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return dsExcel;

            }

        }

        private void btnloadInvAdjust_Click(object sender, EventArgs e)
        {

            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectory = Path.GetDirectoryName(FilePath);

                txtInvAdjustment.Text = FileName;
                txtAdjustPath.Text = FDirectory;

            }
        }

        private void btnTransload_Click(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                txtTransferNotes.Text = FileName;

                FDirectory = Path.GetDirectoryName(FilePath);
                txtTransPath.Text = FDirectory;
            }
        }

        private void btnCusLoad_Click(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectory = Path.GetDirectoryName(FilePath);
                txtCustomerPath.Text = FDirectory;
                txtCustomer.Text = FileName;
            }
        }

        private void btnInvoiceLoad_Click(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectoty = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectoty = Path.GetDirectoryName(FilePath);
                txtInvoices.Text = FileName;
                txtInvoicesPath.Text = FDirectoty;
            }
        }

        private void btnCreditLoad_Click(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectory = Path.GetDirectoryName(FilePath);

                txtCreditPath.Text = FDirectory;
                txtCrteditNotes.Text = FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FDirectory = string.Empty;

            OFD.InitialDirectory = "C:\\PBSS";
            OFD.Filter = "csv files (*.csv)|*.csv";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FilePath = OFD.FileName;
                FileName = Path.GetFileName(FilePath);
                FDirectory = Path.GetDirectoryName(FilePath);

                txtReceiptPath.Text = FDirectory;
                txtReceipts.Text = FileName;
            }
        }

        private void frmInsertToHeadoffice_Load(object sender, EventArgs e)
        {

        }
    }
}