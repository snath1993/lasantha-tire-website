using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.IO;
using System.Xml;  

namespace UserAutherization
{
    public partial class frmInvoiceARRtn : Form
    {
        clsCommon objclsCommon = new clsCommon();
        //Used to set SQL Statments
        string sSQL = string.Empty;
        string sMsg = "Peachtree - Credit Memo";
        UltraGridRow ugR;
        //Common Sql Database Parameters
        SqlConnection sqlCon;
        SqlTransaction sqlTrans;
        SqlCommand sqlCMD;
        SqlDataAdapter sqlDA;
        DataSet sqlDS;
        bool bFilterCus = false;  //False - Code,True - Name
        bool bFilterSalesRep = false; //False - Code,True - Name
        public DSCustomerReturn ds = new DSCustomerReturn();
        bool bMode = false;// False =New Invoice, True = Edit Invoice

        //Sql Connection String
        static string ConnectionString;
        public DSInvoice DSInvoicing = new DSInvoice();
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }
        public frmInvoiceARRtn()
        {
            try
            {
                setConnectionString();
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Invoice Type 1=Inclusive,2-Exclusive,3-NonVAT
        public enum InvoiceType
        {
            Inclusive = 1,
            Exclusive = 2,
            NonVAT = 3
        };
        public int SetFieldLoopUp()
        {
            try
            {
                if (Directory.Exists(Application.StartupPath + "\\Lookup"))
                {
                    if (File.Exists(Application.StartupPath + "\\Lookup\\InvoiceLup.txt"))
                    {
                        File.Delete(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        sw.WriteLine(bFilterCus.ToString());
                        sw.WriteLine(bFilterSalesRep.ToString());
                        sw.Close(); 
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        sw.WriteLine(bFilterCus.ToString());
                        sw.WriteLine(bFilterSalesRep.ToString());
                        sw.Close(); 
                    }
                    

                }
                else
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\Lookup");
                    if (File.Exists(Application.StartupPath + "\\Lookup\\InvoiceLup.txt"))
                    {
                        File.Delete(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        sw.WriteLine(bFilterCus.ToString());
                        sw.WriteLine(bFilterSalesRep.ToString());
                        sw.Close(); 
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
                        sw.WriteLine(bFilterCus.ToString());
                        sw.WriteLine(bFilterSalesRep.ToString());
                        sw.Close(); 
                    }
                }
                
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int GetFieldLoopUp()
        {
            try
            {
                if (Directory.Exists(Application.StartupPath + "\\Lookup"))
                {
                    if (File.Exists(Application.StartupPath + "\\Lookup\\InvoiceRtn.txt"))
                    {
                        StreamReader sr = new StreamReader(Application.StartupPath + "\\Lookup\\InvoiceRtn.txt");
                        bFilterCus = Convert.ToBoolean(sr.ReadLine());
                        bFilterSalesRep = Convert.ToBoolean(sr.ReadLine());
                        sr.Close();
                    }
                    else
                    {
                        bFilterCus = false;
                        bFilterSalesRep = false;
                    }
                }
                else
                {
                    bFilterCus = false;
                    bFilterSalesRep = false;
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
           
        }
        public int GetItems(string WH )
        {
            try
            {
                sSQL = " SELECT tblItemMaster.ItemClass, tblItemWhse.ItemId, tblItemWhse.ItemDis, tblItemWhse.QTY," +
                   " tblItemWhse.WhseId, tblItemMaster.SalesGLAccount FROM tblItemWhse INNER JOIN " +
                   " tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE tblItemWhse.WhseId ='" + WH + "'";
                sqlCon = new SqlConnection(ConnectionString);
                sqlDS = new DataSet();
                sqlCMD = new SqlCommand(sSQL, sqlCon);
                sqlDA = new SqlDataAdapter(sqlCMD);
                sqlCon.Open();
                sqlDA.Fill(sqlDS);
                sqlCon.Close();
                cmbItems.DataSource = sqlDS.Tables[0];
                cmbItems.ValueMember = "ItemId";
                cmbItems.DisplayMember = "ItemId";
                cmbItems.DisplayLayout.Bands[0].Columns["ItemClass"].Hidden = true;
                cmbItems.DisplayLayout.Bands[0].Columns["QTY"].Hidden = true;
                cmbItems.DisplayLayout.Bands[0].Columns["WhseId"].Hidden = true;
                cmbItems.DisplayLayout.Bands[0].Columns["SalesGLAccount"].Hidden = true;

                cmbItemDescription.DataSource = sqlDS.Tables[0];
                cmbItemDescription.ValueMember = "ItemDis";
                cmbItemDescription.DisplayMember = "ItemDis";
                cmbItemDescription.DisplayLayout.Bands[0].Columns["ItemClass"].Hidden = true;
                cmbItemDescription.DisplayLayout.Bands[0].Columns["QTY"].Hidden = true;
                cmbItemDescription.DisplayLayout.Bands[0].Columns["WhseId"].Hidden = true;
                cmbItemDescription.DisplayLayout.Bands[0].Columns["SalesGLAccount"].Hidden = true;
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                return 0;
            }
            
        }
        public int  GetInvoiceMasterData()
        {
            try
            {
               Cursor.Current = Cursors.WaitCursor;   
                //Get Field Loopup
                GetFieldLoopUp();

                sSQL = "SELECT CutomerID, CustomerName, Address1, Address2, ShipToAddress1, ShipToAddress2 FROM tblCustomerMaster";
                sSQL += " SELECT WhseId, WhseName FROM tblWhseMaster";
                sSQL += " SELECT RepCode, RepName FROM tblSalesRep";
                sSQL += " SELECT TaxID, Rate,  Account FROM tblTaxApplicable";
                sSQL += " SELECT AcountID, AccountDescription FROM tblChartofAcounts";
                sSQL += " Select CusretnDrAc from tblDefualtSetting";
               
                sqlCon =new SqlConnection(ConnectionString); 
                sqlDS = new DataSet();
                sqlCMD = new SqlCommand(sSQL, sqlCon);
                sqlDA = new SqlDataAdapter(sqlCMD);
                sqlCon.Open ();
                sqlDA.Fill(sqlDS);
                sqlCon.Close();

                cmbCustomerCode.DataSource = sqlDS.Tables[0]; //Customer Table
                cmbCustomerCode.ValueMember = "CutomerID";
                if (bFilterCus == false)
                {
                    cmbCustomerCode.DisplayMember = "CutomerID";
                }
                else
                {
                    cmbCustomerCode.DisplayMember = "CustomerName"; 
                }

                cmbCustomerCode.DisplayLayout.Bands[0].Columns["Address1"].Hidden = true;
                cmbCustomerCode.DisplayLayout.Bands[0].Columns["Address2"].Hidden = true;
                cmbCustomerCode.DisplayLayout.Bands[0].Columns["ShipToAddress1"].Hidden = true;
                cmbCustomerCode.DisplayLayout.Bands[0].Columns["ShipToAddress2"].Hidden = true;

                cmbWH.DataSource = sqlDS.Tables[1]; //WH Table
                cmbWH.ValueMember = "WhseId";
                cmbWH.DisplayMember = "WhseId"; 

                cmbSalesRep.DataSource = sqlDS.Tables[2]; //Sales Rep Table
                cmbSalesRep.ValueMember = "RepCode";
                if (bFilterSalesRep == false)
                {
                    cmbSalesRep.DisplayMember = "RepCode";
                }
                else
                {
                    cmbSalesRep.DisplayMember = "RepName";
                }

                cmbNBTGL.DataSource = sqlDS.Tables[3];  //NBT Tax
                cmbNBTGL.ValueMember  = "TaxID";
                cmbNBTGL.DisplayMember = "Account";
                if (cmbNBTGL.Rows.Count > 0)
                {
                    cmbNBTGL.Value = cmbNBTGL.Rows[0].Cells["TaxID"].Value.ToString().Trim();
                    txtNBTPer.Value = Convert.ToDouble(cmbNBTGL.Rows[0].Cells["Rate"].Value);

                }

                cmbVATGL.DataSource = sqlDS.Tables[3];  //VAT Tax
                cmbVATGL.ValueMember = "TaxID";
                cmbVATGL.DisplayMember = "Account";
                if (cmbVATGL.Rows.Count > 0)
                {
                    cmbVATGL.Value = cmbVATGL.Rows[1].Cells["TaxID"].Value.ToString().Trim();
                    txtVATPer.Value = Convert.ToDouble(cmbNBTGL.Rows[1].Cells["Rate"].Value);

                }

                cmbARAccount.DataSource = sqlDS.Tables[4];  //GL Account Table
                cmbARAccount.ValueMember = "AcountID";
                cmbARAccount.DisplayMember = "AcountID";
                if (sqlDS.Tables[5].Rows.Count > 0)
                {
                    foreach (DataRow Dr in sqlDS.Tables[5].Rows)
                    {
                        cmbARAccount.Value = Dr["CusretnDrAc"];
                    }
                }

            
 
                Cursor.Current = Cursors.Default;  
                return 1;
                    
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;  
                MessageBox.Show("Please check your master data", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }
        }
        private void GetInvoices()
        {
            if (Convert.ToInt64(cmbReturnType.Value) == 1 && cmbCustomerCode.Value != null  )  //Invoice Type 1- Invoice 
            {
                sSQL = "SELECT DISTINCT( InvoiceNo) , InvoiceDate,DeliveryNoteNos FROM tblSalesInvoices where CustomerID='" + cmbCustomerCode.Value.ToString().Trim() + "'";

                sqlCon = new SqlConnection(ConnectionString);
                sqlDS = new DataSet();
                sqlCMD = new SqlCommand(sSQL, sqlCon);
                sqlDA = new SqlDataAdapter(sqlCMD);
                sqlCon.Open();
                sqlDA.Fill(sqlDS);
                sqlCon.Close();

                //Delete All rows from delivery note grid
                //--------------------------------------
                foreach (UltraGridRow mUgR in UGDeliveryNote.Rows.All)
                {
                    mUgR.Delete(false);
                }
                //-------------------------------------

                //Fill Invoice Details for particular customer
                if (sqlDS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                    {
                        ugR = UGDeliveryNote.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["InvoiceNo"].Value = Dr["InvoiceNo"].ToString();
                        ugR.Cells["InvoiceDate"].Value = Convert.ToDateTime(Dr["InvoiceDate"]).ToString("dd/MM/yyyy");
                        ugR.Cells["DeliveryNoteNo"].Value = Dr["DeliveryNoteNos"].ToString();  

                    }
                }
            }
        }
        private void GetInvoice(string InvNo)
        {
            //Fill Header Details
            sSQL = " SELECT  DISTINCT(InvoiceNo), CustomerID, ARAccount, NoofDistributions, CustomerPO, JobID,Location, InvType,SalesRep " + 
                " FROM tblSalesInvoices WHERE InvoiceNo ='" + InvNo  +"'";
            sqlCon = new SqlConnection(ConnectionString);
            sqlDS = new DataSet();
            sqlCMD = new SqlCommand(sSQL, sqlCon);
            sqlDA = new SqlDataAdapter(sqlCMD);
            sqlCon.Open();
            sqlDA.Fill(sqlDS);
            sqlCon.Close();
            if (sqlDS.Tables[0].Rows.Count > 0)
            {
                if (sqlDS.Tables[0].Rows.Count > 1)
                {
                    MessageBox.Show("Duplicate Delivery Note has found,Check your existing data", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close(); 
                }
                foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                {
                    cmbCustomerCode.Value = Dr["CustomerID"].ToString();

                    txtCustomerPO.Text = Dr["CustomerPO"].ToString().Trim() ;
                    txtCustomerPO.Enabled = true; 
                    cmbWH.Value = Dr["Location"].ToString().Trim() ;
                    cmbWH.Enabled = true ;
                    cmbSalesRep.Value = Dr["SalesRep"].ToString().Trim();

                    if (Dr["InvType"] == DBNull.Value)
                    {
                        cmbInvoiceType.Value  = 3;
                        cmbInvoiceType.Enabled = false; 
                    }
                    else 
                    {
                        cmbInvoiceType.Value = Convert.ToInt64(Dr["InvType"]);
                        cmbInvoiceType.Enabled = false; 
                    }
                    
 
                }
            }

            //Fill Detail
            sSQL = "SELECT DistributionNo, ItemID, Description, RemainQty, GLAcount, UnitPrice, InvoiceNo" + 
                " FROM tblSalesInvoices WHERE InvoiceNo ='" + InvNo + "'";
            sqlCon = new SqlConnection(ConnectionString);
            sqlDS = new DataSet();
            sqlCMD = new SqlCommand(sSQL, sqlCon);
            sqlDA = new SqlDataAdapter(sqlCMD);
            sqlCon.Open();
            sqlDA.Fill(sqlDS);
            sqlCon.Close();


            //Delete existing data from item grid
            foreach (UltraGridRow mugR in UGItems.Rows.All)
            {
                mugR.Delete(false); 
            }
            //----------------------------------


            //Fill Item Grid 
            if (sqlDS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                {
                    ugR = UGItems.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["ItemCode"].Value = Dr["ItemID"].ToString();
                    foreach (UltraGridRow myugR in cmbItems.Rows)
                    {

                        if (myugR.Cells["ItemId"].Value.ToString().Trim() == ugR.Cells["ItemCode"].Value.ToString().Trim())
                        {
                            ugR.Cells["AvailableQty"].Value = Convert.ToDouble(myugR.Cells["QTY"].Value);
                            ugR.Cells["WH"].Value = cmbWH.Value.ToString ().Trim ()  ;
                            ugR.Cells["IsSerialItem"].Value = myugR.Cells["ItemClass"].Value.ToString();   
                        }

                    }
                    ugR.Cells["Description"].Value = Dr["Description"].ToString();
                    ugR.Cells["Qty"].Value = Convert.ToDouble(Dr["RemainQty"]);
                    ugR.Cells["GL_Account"].Value = Dr["GLAcount"].ToString();
                    ugR.Cells["UnitPrice"].Value = Convert.ToDouble(Dr["UnitPrice"]);
                }
            }
            //Calculation Invoice Totals
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));

        }
        private void codeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bFilterCus = false;
                SetFieldLoopUp(); //Saving Field Loopup data
                GetInvoiceMasterData(); //Refresh All Master data
                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bFilterCus = true;
                SetFieldLoopUp();//Saving Field Loopup data
                GetInvoiceMasterData();//Refresh All Master data
                //cmbCustomerCode_Leave(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

       

        private void cmbCustomerCode_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                //Discard
                DiscardCustomer();
                //Clear All Existing data
                txtCustomerName.ResetText();
                txtBillAdd1.ResetText();
                txtBillAdd2.ResetText();
                txtShipAdd1.ResetText();
                txtShipAdd2.ResetText();
                //get Selected row
                ugR = e.Row;

                if (ugR != null)
                {
                    txtCustomerName.Text = ugR.Cells["CustomerName"].Value.ToString();
                    txtBillAdd1.Text = ugR.Cells["Address1"].Value.ToString();
                    txtBillAdd2.Text = ugR.Cells["Address2"].Value.ToString();
                    txtShipAdd1.Text = ugR.Cells["ShipToAddress1"].Value.ToString();
                    txtShipAdd2.Text = ugR.Cells["ShipToAddress2"].Value.ToString();
                }
                else
                {
                    foreach (UltraGridRow myugR in cmbCustomerCode.Rows)
                    {
                        if (cmbCustomerCode.Value == null) return;
                            if (myugR.Cells["CutomerID"].Value.ToString().Trim() == cmbCustomerCode.Value.ToString().Trim())
                            {
                                txtCustomerName.Text = myugR.Cells["CustomerName"].Value.ToString();
                                txtBillAdd1.Text = myugR.Cells["Address1"].Value.ToString();
                                txtBillAdd2.Text = myugR.Cells["Address2"].Value.ToString();
                                txtShipAdd1.Text = myugR.Cells["ShipToAddress1"].Value.ToString();
                                txtShipAdd2.Text = myugR.Cells["ShipToAddress2"].Value.ToString();
                            }
                        
                    }
                }
                //Fill Customer Invoice Details
                GetInvoices();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }            
        }

        public void DiscardCustomer()
        {
            try
            {
                if (bMode == true)
                {
                    bMode = false; //New Invoice
                }

                txtBillAdd1.ResetText();
                txtBillAdd2.ResetText();
                txtBillAdd3.ResetText();
                txtBillAdd4.ResetText();
                txtShipAdd1.ResetText();
                txtShipAdd2.ResetText();
                txtShipAdd3.ResetText();
                txtShipAdd4.ResetText();
                txtReturnNo.Text = "<Auto Number>";
                txtReturnNo.Enabled = false;
                dtpRtnDate.Value = DateTime.Now;
                cmbInvoiceType.Value = 1; //1-Inclusive,2-Exclusive,3-non VAT 
                cmbSalesRep.ResetText();
                cmbWH.ResetText();
                txtCustomerPO.ResetText();
                foreach (UltraGridRow mUgR in UGItems.Rows.All)
                {
                    mUgR.Delete(false);
                }
                foreach (UltraGridRow mUgR in UGDeliveryNote.Rows.All)
                {
                    mUgR.Delete(false);
                }
                cbManualNumber.Checked = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        private void UGDeliveryNote_CellChange(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Select")
                {
                    bool bSelect = Convert.ToBoolean(e.Cell.Text);
                    e.Cell.Value = e.Cell.Text;

                    foreach (UltraGridRow mugR in UGItems.Rows.All)
                    {
                        mugR.Delete(false);
                    }

                    foreach (UltraGridRow mugR in UGDeliveryNote.Rows)
                    {
                        if (Convert.ToBoolean(mugR.Cells["Select"].Value) == true)
                        {
                            mugR.Cells["Select"].Value = false;
                        }
                    }
                    e.Cell.Value = bSelect;

                    foreach (UltraGridRow mugR in UGDeliveryNote.Rows)
                    {
                        if (Convert.ToBoolean(e.Cell.Value) == true)
                        {
                            GetInvoice(e.Cell.Row.Cells["InvoiceNo"].Value.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        public void NewInvoice()
        {
            try
            {
                if (bMode == true)
                {
                    bMode = false; //New Invoice
                }
                cmbCustomerCode.ResetText();

                txtBillAdd1.ResetText();
                txtBillAdd2.ResetText();
                txtBillAdd3.ResetText();
                txtBillAdd4.ResetText();
                txtShipAdd1.ResetText();
                txtShipAdd2.ResetText();
                txtShipAdd3.ResetText();
                txtShipAdd4.ResetText();
                txtReturnNo.Text = "<Auto Number>";
                txtReturnNo.Enabled = false;
                dtpRtnDate.Value = DateTime.Now;
                //
                cmbInvoiceType.Value = 1; //1-Inclusive,2-Exclusive,3-non VAT 
                cmbInvoiceType.Enabled = true;
                //
                cmbSalesRep.ResetText();
                cmbSalesRep.Enabled = true;
                //
                cmbWH.ResetText();
                cmbWH.Enabled = true;
                //
                txtCustomerPO.ResetText();
                txtCustomerPO.Enabled = true;
                //
                cmbReturnType.ResetText();
                cmbReturnType.Enabled = true;
                foreach (UltraGridRow mUgR in UGItems.Rows.All)
                {
                    mUgR.Delete(false);
                }
                foreach (UltraGridRow mUgR in UGDeliveryNote.Rows.All)
                {
                    mUgR.Delete(false);
                }
                foreach (UltraGridRow mUgR in UGSerial.Rows.All)
                {
                    mUgR.Delete(false);
                }
                cbManualNumber.Checked = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }              
        }
        private void tsbNew_Click(object sender, EventArgs e)
        {
            try
            {
                NewInvoice();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cbManualNumber_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbManualNumber.Checked == true)
                {
                    txtReturnNo.ResetText();
                    txtReturnNo.Enabled = true;
                }
                else
                {
                    txtReturnNo.ResetText();
                    txtReturnNo.Text = "<Auto Number>";
                    txtReturnNo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            try
            {
                NewInvoice();
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        
        private void UGItems_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["ID"].Value = e.Row.Index + 1; 
        }

        private void UGDeliveryNote_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["ID"].Value = e.Row.Index + 1; 
        }

        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt64( cmbInvoiceType.Value) == 1)
                {
                    UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].Header.Caption = "Unit Price(Incl)";
                    UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].Header.Caption = "Amount (Incl)";
                    UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].Hidden = true;
                    UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].Hidden = false;
                    if (cmbNBTGL.Rows.Count > 0)
                    {
                        cmbNBTGL.Value = cmbNBTGL.Rows[0].Cells["TaxID"].Value.ToString().Trim();
                        txtNBTPer.Value = Convert.ToDouble(cmbNBTGL.Rows[0].Cells["Rate"].Value);
                    }
                    if (cmbVATGL.Rows.Count > 0)
                    {
                        cmbVATGL.Value = cmbVATGL.Rows[1].Cells["TaxID"].Value.ToString().Trim();
                        txtVATPer.Value = Convert.ToDouble(cmbNBTGL.Rows[1].Cells["Rate"].Value);
                    }
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                {
                    UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].Header.Caption = "Unit Price(Excl)";
                    UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].Header.Caption = "Amount (Excl)";
                    UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].Hidden = true;
                    UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].Hidden = false;
                    if (cmbNBTGL.Rows.Count > 0)
                    {
                        cmbNBTGL.Value = cmbNBTGL.Rows[0].Cells["TaxID"].Value.ToString().Trim();
                        txtNBTPer.Value = Convert.ToDouble(cmbNBTGL.Rows[0].Cells["Rate"].Value);
                    }
                    if (cmbVATGL.Rows.Count > 0)
                    {
                        cmbVATGL.Value = cmbVATGL.Rows[1].Cells["TaxID"].Value.ToString().Trim();
                        txtVATPer.Value = Convert.ToDouble(cmbNBTGL.Rows[1].Cells["Rate"].Value);
                    }
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                {
                    UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].Header.Caption = "Unit Price";
                    UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].Hidden =true ;
                    UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].Hidden = false ;
                    txtVATPer.Value = 0;
                    txtNBTPer.Value = 0; 
                }
                //Invoice calculation
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));   
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
           
        }
        public void InvoiceCalculation(Int64 iInvoiceType)
        {
            try
            {

                double dblSubTotal = 0;
                double dblTotVAT = 0;
                //double dblTotNBT = 0;
                //double dblInvTotal = 0;
                double dblLineVAT = 0;
                double dblLineQty = 0;
                double dblLinePrice = 0;
                double dblVATPer =Convert.ToDouble(  txtVATPer.Value);  //12%
                double dblNBTPer =Convert.ToDouble(  txtNBTPer.Value);  //2%
     
                if (iInvoiceType == 1) // Inclusive
                {
                    foreach (UltraGridRow mUgR in UGItems.Rows)
                    {
                        dblLinePrice =Convert.ToDouble( mUgR.Cells["UnitPrice"].Value);
                        dblLineQty = Convert.ToDouble(mUgR.Cells["ReturnQty"].Value);
                        mUgR.Cells["AmountIncl"].Value = Math.Round(dblLineQty * dblLinePrice, 3, MidpointRounding.AwayFromZero);
                        mUgR.Cells["AmountExcl"].Value=Math.Round( Convert.ToDouble( mUgR.Cells["AmountIncl"].Value) * 100 / (100 + dblVATPer),3, MidpointRounding.AwayFromZero);  
                        //VAT
                        dblLineVAT =Math.Round( Convert .ToDouble(  mUgR.Cells["AmountIncl"].Value) - Convert.ToDouble( mUgR.Cells["AmountExcl"].Value),3, MidpointRounding.AwayFromZero ) ;
                        mUgR.Cells["VAT"].Value = Math.Round(dblLineVAT, 3, MidpointRounding.AwayFromZero);    
                        //Inv Sub Total
                        dblSubTotal =Math.Round(  dblSubTotal +Convert.ToDouble(  mUgR.Cells["AmountIncl"].Value),3, MidpointRounding.AwayFromZero)  ;
                        //Inv TAX Total
                        dblTotVAT =Math.Round(   dblTotVAT + dblLineVAT,3, MidpointRounding.AwayFromZero)  ;
                        mUgR.Activated = true; 
                        UGItems.PerformAction(UltraGridAction.CommitRow );
                        UGItems.PerformAction(UltraGridAction.ExitEditMode );    
                    }
                    txtSubTotal.Value = dblSubTotal;
                    txtVAT.Value = dblTotVAT;
                    txtInvoiceTotal.Value = dblSubTotal;   
                }
                else if (iInvoiceType == 2)
                {
                    foreach (UltraGridRow mUgR in UGItems.Rows)
                    {
                        dblLinePrice = Convert.ToDouble(mUgR.Cells["UnitPrice"].Value);
                        dblLineQty = Convert.ToDouble(mUgR.Cells["ReturnQty"].Value);
                        mUgR.Cells["AmountIncl"].Value = Math.Round((dblLineQty * dblLinePrice) + ((dblLineQty * dblLinePrice) * dblVATPer/100) , 3, MidpointRounding.AwayFromZero);
                        mUgR.Cells["AmountExcl"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);
                        //VAT
                        dblLineVAT = Math.Round(Convert.ToDouble(mUgR.Cells["AmountIncl"].Value) - Convert.ToDouble(mUgR.Cells["AmountExcl"].Value), 3, MidpointRounding.AwayFromZero);
                        mUgR.Cells["VAT"].Value = Math.Round(dblLineVAT, 3, MidpointRounding.AwayFromZero);
                        //Inv Sub Total
                        dblSubTotal =Math.Round(  dblSubTotal +Convert.ToDouble(  mUgR.Cells["AmountExcl"].Value),3, MidpointRounding.AwayFromZero)  ;
                        //Inv TAX Total
                        dblTotVAT =Math.Round(  dblTotVAT + dblLineVAT,3, MidpointRounding.AwayFromZero)  ;
                        mUgR.Activated = true; 

                        UGItems.PerformAction(UltraGridAction.CommitRow);
                        UGItems.PerformAction(UltraGridAction.ExitEditMode); 
                    }
                    txtSubTotal.Value = dblSubTotal;
                    txtVAT.Value = dblTotVAT;
                    txtInvoiceTotal.Value = dblSubTotal + dblTotVAT;  
                }

                else if (iInvoiceType == 3)
                {
                    foreach (UltraGridRow mUgR in UGItems.Rows)
                    {
                        dblLinePrice = Convert.ToDouble(mUgR.Cells["UnitPrice"].Value);
                        dblLineQty = Convert.ToDouble(mUgR.Cells["ReturnQty"].Value);
                        mUgR.Cells["AmountIncl"].Value = Math.Round((dblLineQty * dblLinePrice) + ((dblLineQty * dblLinePrice) * dblVATPer / 100), 3, MidpointRounding.AwayFromZero);
                        mUgR.Cells["AmountExcl"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);
                        //VAT
                        dblLineVAT = Math.Round(Convert.ToDouble(mUgR.Cells["AmountIncl"].Value) - Convert.ToDouble(mUgR.Cells["AmountExcl"].Value), 3, MidpointRounding.AwayFromZero);
                        mUgR.Cells["VAT"].Value = Math.Round(dblLineVAT, 3, MidpointRounding.AwayFromZero);
                        //Inv Sub Total
                        dblSubTotal =Math.Round(  dblSubTotal +Convert.ToDouble(  mUgR.Cells["AmountExcl"].Value),3, MidpointRounding.AwayFromZero ) ;
                        //Inv TAX Total
                        dblTotVAT =Math.Round(  dblTotVAT + dblLineVAT,3, MidpointRounding.AwayFromZero ) ;
                        mUgR.Activated = true; 
                        UGItems.PerformAction(UltraGridAction.CommitRow);
                        UGItems.PerformAction(UltraGridAction.ExitEditMode); 
                    }
                    txtSubTotal.Value = dblSubTotal;
                    txtVAT.Value = dblTotVAT;
                    txtInvoiceTotal.Value = dblSubTotal + dblTotVAT;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        private void txtVATPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Invoice calculation
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
       
        private void tsbSave_Click(object sender, EventArgs e)
        {
            
            foreach (UltraGridRow mugR in UGItems.Rows.All )
            {
                if (mugR.Cells["ItemCode"].Value.ToString().Trim().Length == 0)
                {
                    mugR.Delete(false);  
                }
                if (Convert.ToDouble(mugR.Cells["ReturnQty"].Value) == 0.00)
                {
                    mugR.Delete(false);   
                }

            }
            if (cmbCustomerCode.Value  == null)
            {
                MessageBox.Show("Customer Code can not be left blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (UGItems.Rows.Count == 0)
            {
                MessageBox.Show("Enter Items", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt64( cmbReturnType.Value )==1)
            {
                int iDispatchCount = 0;
                foreach (UltraGridRow mUgR in UGDeliveryNote.Rows)
                {
                    if (Convert.ToBoolean( mUgR.Cells["Select"].Value) == true)
                    {
                        iDispatchCount = iDispatchCount + 1;
                    }

                }
                if (iDispatchCount == 0)
                {
                    MessageBox.Show("Select Invoice", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (cmbInvoiceType.Value == null)
            {
                MessageBox.Show("Invoice Type can not be left blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbReturnType.Value == null)
            {
                MessageBox.Show("Return Type can not be left blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            if (cbManualNumber.Checked == true)
            {
                if (txtReturnNo.Text.ToString().Trim().Length == 0)
                {
                    MessageBox.Show("Invoice Number can not be left blank", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Select Sales Rep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbARAccount.Value == null)
            {
                MessageBox.Show("Select AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbNBTGL.Value  == null)
            {
                MessageBox.Show("Select NBT Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbVATGL.Value == null)
            {
                MessageBox.Show("Select NBT Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            foreach (UltraGridRow mUgR in UGItems.Rows)
            {
                if (Convert.ToInt64(mUgR.Cells["IsSerialItem"].Value) == 10)
                {
                    Int64  iQty = 0;
                    Int64  iRtnQty = 0;
                    iRtnQty = Convert.ToInt64(mUgR.Cells["ReturnQty"].Value);
                    foreach (UltraGridRow mUg in UGSerial.Rows)
                    {
                        if (mUg.Cells["ID"].Value == mUgR.Cells["ID"].Value)
                        {
                            iQty = iQty + 1;
                        }
                    }
                    if (iRtnQty != iQty)
                    {
                        MessageBox.Show("Return Quantity does not match with serial qty", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            // <Modified by: CHATHURA at 03/05/2011-10:24:43 AM on machine: SAGEERP>
            //if (Convert.ToDouble( txtInvoiceTotal.Value) == 0.00)
            //{
            //    MessageBox.Show("Invoice Total can not be 0.00", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            // </Modified by: CHATHURA at 03/05/2011-10:24:43 AM on machine: SAGEERP>

            int iZeroQtyCount = 0;
            int iReturnQtyCount = 0;
            foreach (UltraGridRow mUgR in UGItems.Rows)
            {
               
                if (Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) > 0.00)
                {
                    if (Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) == 0.00)
                    {
                        iZeroQtyCount = iZeroQtyCount + 1;
                    }
                }
                
            }

            if (iZeroQtyCount > 0)
            {
                MessageBox.Show("FOC", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            if (txtCustomerPO.Text.ToString().Trim().Length == 0)
            {
                txtCustomerPO.Text = "-"; 
            }

            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpRtnDate.Value)))
                    return;

                sqlCon = new SqlConnection(ConnectionString);
                sqlCon.Open();
                sqlTrans = sqlCon.BeginTransaction();


                string sReturnNumber = string.Empty;
                string sInvoiceNo = string.Empty;
                bool bIsApplyToInv = false;
                bool bIsExport =false;
                bool bIsFullyInvoice = false;
                int DocType = 8; //As Sanjeewa Advise put this number on 29-04-2011
                string TranType = "CusReturn";

                

                //Get No Of Distribution 
                //NoOfDistrbution = NoOfItemRow + NBT + VAT
                //------------------
                int iNoOfDis = 0;
                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                    if (Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) > 0.00)
                    {
                        if (mUgR.Cells["ItemCode"].Value.ToString() != string.Empty)
                        {
                            iNoOfDis = iNoOfDis + 1;
                        }
                    }
                }
                if (Convert.ToDouble( txtNBT.Value) > 0.0)
                {
                    iNoOfDis = iNoOfDis + 1;
                }
                if (Convert.ToDouble( txtVAT.Value )> 0.0)
                {
                    iNoOfDis = iNoOfDis + 1;
                }
                //-----------------


                foreach (UltraGridRow mUgR in UGDeliveryNote.Rows)
                {
                    if (Convert.ToBoolean(mUgR.Cells["Select"].Value) == true)
                    {
                        sInvoiceNo = mUgR.Cells["InvoiceNo"].Value.ToString().Trim();
                    }

                }

                if (cbManualNumber.Checked == true)
                {
                    sReturnNumber = txtReturnNo.Text.ToString().Trim();
                }
                else
                {
                    sSQL = "UPDATE tblDefualtSetting SET CusReturnNo = CusReturnNo + 1 select CusReturnNo, CusReturnPrefix from tblDefualtSetting";
                    sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                    sqlCMD.CommandType = CommandType.Text;
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDS = new DataSet();
                    sqlDA.Fill(sqlDS);

                    if (sqlDS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                        {
                            sReturnNumber = Dr["CusReturnNo"].ToString().Trim().PadLeft(8, '0');

                            sReturnNumber = Dr["CusReturnPrefix"].ToString().Trim() + "-" + sReturnNumber;
                        }
                    }
                    txtReturnNo.Text = sReturnNumber;
                }

                //Check for Return Type
                if (Convert.ToInt64(cmbReturnType.Value) ==1)
                {
                    bIsApplyToInv = true;  //Customer Invoice Return 
                }
                else if (Convert.ToInt64(cmbReturnType.Value) ==2)
                {
                    bIsApplyToInv =false; //Direct Return Or General Return
                }

                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                    if (Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) > 0.00)
                    {


                        double dblUnitCost = 0;
                        double dblAmount = 0;

                        sSQL = "select UnitCost from tblItemMaster where ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        dblUnitCost = (double)sqlCMD.ExecuteScalar();


                        if (Convert.ToInt64(cmbInvoiceType.Value) == 1)
                        {
                            dblAmount = Convert.ToDouble(mUgR.Cells["AmountIncl"].Value);
                        }
                        else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                        {
                            dblAmount = Convert.ToDouble(mUgR.Cells["AmountExcl"].Value);
                        }
                        else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                        {
                            dblAmount = Convert.ToDouble(mUgR.Cells["AmountExcl"].Value);
                        }

                        sSQL = "insert into tblCutomerReturn(CustomerID,CreditNo,ReturnDate,LocationID,IsApplyToInvoice," +
                            " InvoiceNO,ARAccount,NoofDistribution,DistributionNo,ItemID,InvoiceQty,ReturnQty,Description,UOM," +
                            " UnitPrice,Discount,Amount,GL_Account,NBT,VAT,GrossTotal,GrandTotal,ISExport,CurrenUser,IsFullInvReturn," +
                            " JobID,Tax1ID,Tax2ID,InvType,SalesRep,CustomerPO) values ('" + cmbCustomerCode.Value.ToString().Trim() + "'," +
                            " '" + sReturnNumber + "'," +
                            " '" + dtpRtnDate.Value.ToString("MM/dd/yyyy").Trim() + "','" + cmbWH.Value.ToString().Trim() + "'," +
                            " '" + bIsApplyToInv + "','" + sInvoiceNo + "','" + cmbARAccount.Value.ToString().Trim() + "'," +
                            " " + iNoOfDis + "," + mUgR.Index + 1 + ",'" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'," +
                            " " + Convert.ToDouble(mUgR.Cells["Qty"].Value) + "," + Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) + ",'" + mUgR.Cells["Description"].Value.ToString().Trim() + "'," +
                            " ''," + Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) + "," + Convert.ToDouble(txtDiscount.Value) + "," + dblAmount + "," +
                            " '" + mUgR.Cells["GL_Account"].Value.ToString().Trim() + "'," + Convert.ToDouble(txtNBT.Value) + "," + Convert.ToDouble(txtVAT.Value) + "," +
                            " " + Convert.ToDouble(txtInvoiceTotal.Value) + "," + Convert.ToDouble(txtInvoiceTotal.Value) + "," +
                            " '" + bIsExport + "','" + user.userName.ToString().Trim() + "','" + bIsFullyInvoice + "',''," +
                            " '" + cmbNBTGL.Value.ToString().Trim() + "','" + cmbVATGL.Value.ToString().Trim() + "'," + Convert.ToInt64(cmbReturnType.Value) + ",'" + cmbSalesRep.Value.ToString().Trim()     +"','" + txtCustomerPO.Text.ToString().Trim()     + "')";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();

                        sSQL = "insert into tblInvTransaction(TDate,ItemID,FrmWhseId,ToWhseId,QTY,TransType) values ('" + dtpRtnDate.Value.ToString("MM/dd/yyyy").Trim() + "'," +
                            " '" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "','" + cmbWH.Value.ToString().Trim() + "'," +
                            " '" + cmbWH.Value.ToString().Trim() + "','" + Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) + "','" + TranType + "')";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();


                        bool IsGRNProcess = true;
                        string Status = "Available";
                        foreach (UltraGridRow mSerialRow in UGSerial.Rows)
                        {
                            if (Convert.ToInt64(mUgR.Cells["ID"].Value) == Convert.ToInt64(mSerialRow.Cells["ID"].Value))
                            {
                                sSQL = "insert into tblSerialCusReturn(CRTNO,ItemID,Description,SerialNO,TransactionType,IsRTNProcess," +
                                    " WLocation)values ('" + sReturnNumber + "','" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'," +
                                    " '" + mUgR.Cells["Description"].Value.ToString().Trim() + "','" + mSerialRow.Cells["Serials"].Value.ToString().Trim() + "'," +
                                    " '" + TranType + "','" + IsGRNProcess + "','" + cmbWH.Value.ToString().Trim() + "')";
                                sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                                sqlCMD.CommandType = CommandType.Text;
                                sqlCMD.ExecuteNonQuery();

                                sSQL = "Update tblSerialItemTransaction SET Status = '" + Status + "' where ItemID = '" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "' " +
                                    " and SerialNO='" + mSerialRow.Cells["Serials"].Value.ToString().Trim() + "' " +
                                    " and WareHouseID='" + cmbWH.Value.ToString().Trim() + "'";
                                sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                                sqlCMD.CommandType = CommandType.Text;
                                sqlCMD.ExecuteNonQuery();

                                sSQL = "Update tblInvoiceSerializeItem SET IsINVProcess = '" + IsGRNProcess + "' where ItemID = '" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "' " +
                                    " and SerialNO='" + mSerialRow.Cells["Serials"].Value.ToString().Trim() + "' and WLocation='" + cmbWH.Value.ToString().Trim() + "'";
                                sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                                sqlCMD.CommandType = CommandType.Text;
                                sqlCMD.ExecuteNonQuery();
                             
                            }
                        }
                        if (Convert.ToInt64(cmbReturnType.Value) == 1)
                        {
                            sSQL = "update tblSalesInvoices set RemainQty = RemainQty- " + Convert.ToInt64(mUgR.Cells["ReturnQty"].Value) + " " +
                                " where InvoiceNo =  '" + sInvoiceNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            sqlCMD.ExecuteNonQuery();
                        }
                        sSQL = "Select * from  tblItemWhse where ItemId='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "' and WhseId='" + cmbWH.Value.ToString().Trim() + "'";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlDS = new DataSet();
                        sqlDA = new SqlDataAdapter(sqlCMD);
                        sqlDA.Fill(sqlDS);
                        if (sqlDS.Tables[0].Rows.Count > 0)
                        {
                            sSQL = "update tblItemWhse set QTY = QTY + " + Convert.ToInt64(mUgR.Cells["ReturnQty"].Value) + "  where " +
                                " ItemId='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "' and WhseId='" + cmbWH.Value.ToString().Trim() + "'";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            sqlCMD.ExecuteNonQuery();
                        }
                        else
                        {
                            sSQL = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbWH.Value.ToString().Trim() + "'," +
                                " '" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "','" + mUgR.Cells["Description"].Value.ToString().Trim() + "'," +
                                " " + Convert.ToInt64(mUgR.Cells["ReturnQty"].Value) + " ,'','" + dtpRtnDate.Value.ToString("MM/dd/yyyy").Trim() + "')";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            sqlCMD.ExecuteNonQuery();
                        }
                        if (Convert.ToInt64(cmbReturnType.Value) == 1)
                        {
                            double dTotRtnQty = 0;
                            double dTotInvQty = 0;
                            sSQL = "select Sum(ReturnQty) from tblCutomerReturn where InvoiceNO =  '" + sInvoiceNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            dTotRtnQty = (double)sqlCMD.ExecuteScalar();

                            sSQL = "select Qty from tblSalesInvoices where InvoiceNo =  '" + sInvoiceNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            dTotInvQty = (double)sqlCMD.ExecuteScalar();

                            if (dTotInvQty == dTotRtnQty)
                            {
                                bool IsReturn = true;
                                sSQL = "update tblSalesInvoices set IsReturn = '" + IsReturn + "'  where InvoiceNo =  '" + sInvoiceNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                                sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                                sqlCMD.CommandType = CommandType.Text;
                                sqlCMD.ExecuteNonQuery();
                            }
                        }

                        //Get Unit Cost 
                        bool bIsOut = true; //if true IN ,false OUT

                        sSQL = "Insert into tbItemlActivity (DocType,TranNo,TransDate,TranType,DocReference," +
                            " ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (" + DocType + "," +
                            " '" + sReturnNumber + "','" + dtpRtnDate.Value.ToString("MM/dd/yyyy").Trim() + "'," +
                            " '" + TranType + "','" + bIsOut + "','" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'," +
                            " " + Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) + "," + dblUnitCost + "," + Convert.ToDouble(dblUnitCost * Convert.ToDouble(mUgR.Cells["ReturnQty"].Value)) + "," +
                            " '" + cmbWH.Value.ToString().Trim() + "'," + Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) + ")";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();
                    }
                }
                //Update Peachtree
               
                if (CreateInvRtnXML(iNoOfDis, sReturnNumber , sInvoiceNo,bIsApplyToInv ) == 0)
                {
                    sqlTrans.Rollback();
                    sqlCon.Close();
                    return;

                }
                
                sqlTrans.Commit();
                sqlCon.Close();  
                MessageBox.Show("Customer Return Successfuly Saved");
                if (MessageBox.Show("Do you want to print invoice now ?", "Print Return Note", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    InvoicePrint(sReturnNumber); 
                }
                NewInvoice(); 
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                sqlTrans.Rollback();
                sqlCon.Close();
                return;
            }
            
            finally 
            {
                sqlCon.Close();  
            }
        }
        private void InvoicePrint(string InvNo)
        {
            if (InvNo.Length  > 0)
            {
                ds.Clear();

                try
                {
                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + InvNo + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DTReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    //----------------------- Added on 23/04/2011

                    String S2 = "Select * from tblCustomerMaster";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlConnection con2 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, con2);
                    da2.Fill(ds, "CustomerMaster");




                    String S3 = "Select * from tblSalesInvoices";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(ds, "dtInvoiceData");


                    //-----------------------

                    frmViewerCustomerReturn cusReturn = new frmViewerCustomerReturn(this);
                    cusReturn.Show();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                MessageBox.Show("Please select a customer return and try again");
            }
        }
        public int CreateInvRtnXML(int NoOfDis, string ReturnNo, string InvoiceNo, bool IsApplyToInv)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(dtpRtnDate.Value);
                string Dformat = "MM/dd/yyyy";
                string RtnDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:invoice");


                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomerCode.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtReturnNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(RtnDate);//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbARAccount.Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Customer_PO");
                Writer.WriteString(txtCustomerPO.Text.ToString().Trim());
                Writer.WriteEndElement();
                //if (IsApplyToInv == true)
                //{
                //    Writer.WriteStartElement("ApplyToInvoiceNumber");
                //    Writer.WriteString(InvoiceNo);
                //    Writer.WriteEndElement();
                //}
                //Apply to Invoice Distribution
                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("TRUE");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoOfDis.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("SalesLines");//----------Start SalesLines 
                Int64 iDisNo = 0;
                foreach (UltraGridRow mUgR in UGItems.Rows)  //----For Start
                {
                    if (Convert.ToDouble(mUgR.Cells["ReturnQty"].Value) > 0.00)
                    {
                    
                        sSQL = "Select SerialNO from tblSerialCusReturn where CRTNO='" + ReturnNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlDS = new DataSet();
                        sqlDA = new SqlDataAdapter(sqlCMD);
                        sqlDA.Fill(sqlDS);
                        if (sqlDS.Tables[0].Rows.Count > 0)  //Check For Serialize Item
                        {
                            foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                            {
                                Writer.WriteStartElement("SalesLine"); //-------------Start Sales Line

                                Writer.WriteStartElement("SalesOrderDistributionNumber");
                                Writer.WriteString("0");
                                Writer.WriteEndElement();

                                iDisNo = mUgR.Index + 1;
                                Writer.WriteStartElement("InvoiceCMDistribution");
                                Writer.WriteString(iDisNo.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString("-" + mUgR.Cells["ReturnQty"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(mUgR.Cells["ItemCode"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(mUgR.Cells["Description"].Value.ToString());
                                Writer.WriteEndElement();


                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(mUgR.Cells["GL_Account"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Unit_Price");
                                Writer.WriteString(mUgR.Cells["UnitPrice"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");//Doctor Charge
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString(mUgR.Cells["AmountExcl"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();// ------END Sales Lines

                                Writer.WriteStartElement("Serial_Number");
                                Writer.WriteString(Dr["SerialNO"].ToString().Trim());
                                Writer.WriteEndElement();

                            }

                        }
                        else
                        {
                            Writer.WriteStartElement("SalesLine");//---------- Start Line 

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();
                            iDisNo = mUgR.Index + 1;
                            Writer.WriteStartElement("InvoiceCMDistribution");
                            Writer.WriteString(iDisNo.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("-" + mUgR.Cells["ReturnQty"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(mUgR.Cells["ItemCode"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(mUgR.Cells["Description"].Value.ToString());
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(mUgR.Cells["GL_Account"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Unit_Price");
                            Writer.WriteString(mUgR.Cells["UnitPrice"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(mUgR.Cells["AmountExcl"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();//------END SALES LINE

                        }
                    }
                }
                if (Convert.ToDouble(txtNBT.Value) > 0)
                {

                   if (cmbNBTGL.Value.ToString().Trim() != string.Empty)
                   {
                            //----------3
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            iDisNo = iDisNo  + 1;
                            Writer.WriteStartElement("InvoiceCMDistribution");
                            Writer.WriteString(iDisNo.ToString());
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbNBTGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString( txtNBT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------3
                        }
                    } //-----END NBT
                    if (Convert.ToDouble(txtVAT.Value) > 0)
                    {

                        if (cmbVATGL.Value.ToString().Trim() != string.Empty)
                        {
                            //----------4
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            iDisNo = iDisNo + 1;
                            Writer.WriteStartElement("InvoiceCMDistribution");
                            Writer.WriteString(iDisNo.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbVATGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString( txtVAT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------4
                        }
                    } //-----END VAT
                Writer.WriteEndElement();//----------END SALES LINES
                Writer.WriteEndElement();//---------END PAW_Invoice
                Writer.WriteEndElement();//---------END PAW_Invoices
                Writer.Close();
                Connector abc = new Connector();//export to peach tree
                if (abc.ImportCustomerReturnAR() == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {                
                return 0;
            }
        }

        public int CreateInvRtnXMLCopy(int NoOfDis, string ReturnNo, string InvoiceNo ,bool IsApplyToInv)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(dtpRtnDate.Value);
                string Dformat = "MM/dd/yyyy";
                string RtnDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:invoice");


                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomerCode.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtReturnNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(RtnDate);//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbARAccount.Value.ToString().Trim());
                Writer.WriteEndElement();
                //if (IsApplyToInv == true)
                //{
                //    Writer.WriteStartElement("ApplyToInvoiceNumber");
                //    Writer.WriteString(InvoiceNo);
                //    Writer.WriteEndElement();
                //}
                //Apply to Invoice Distribution
                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("TRUE");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoOfDis.ToString() );
                Writer.WriteEndElement();

                Writer.WriteStartElement("SalesLines");//----------Start SalesLines 
                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                    sSQL = "Select SerialNO from tblSerialCusReturn where CRTNO='" + ReturnNo + "' and ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                    sqlCMD.CommandType = CommandType.Text;
                    sqlDS = new DataSet();
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(sqlDS);
                    if (sqlDS.Tables[0].Rows.Count > 0)  //Check For Serialize Item
                    {
                        foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                        {
                            Writer.WriteStartElement("SalesLine"); //-------------Start Sales Line

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Int64 iDisNo = mUgR.Index + 1; 
                            Writer.WriteStartElement("InvoiceCMDistribution");
                            Writer.WriteString(iDisNo.ToString ());
                            Writer.WriteEndElement();

                            //if (IsApplyToInv == true)
                            //{
                            //    Writer.WriteStartElement("Apply_To_Sales_Order");
                            //    Writer.WriteString("TRUE");
                            //    Writer.WriteEndElement();
                            //}
                            //else
                            //{
                            //    Writer.WriteStartElement("Apply_To_Sales_Order");
                            //    Writer.WriteString("FALSE");
                            //    Writer.WriteEndElement();
                            //}
                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("-" + mUgR.Cells["ReturnQty"].Value.ToString() );
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(mUgR.Cells["ItemCode"].Value.ToString()   );
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(mUgR.Cells["Description"].Value.ToString()   );
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(mUgR.Cells["GL_Account"].Value.ToString()  );
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Unit_Price");
                            Writer.WriteString(mUgR.Cells["UnitPrice"].Value.ToString()   );
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(mUgR.Cells["AmountExcl"].Value.ToString() );
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();// ------END Sales Lines

                            Writer.WriteStartElement("Serial_Number");
                            Writer.WriteString(Dr["SerialNO"].ToString().Trim() );
                            Writer.WriteEndElement();

                        }
 
                    }
                    else
                    {
                        Writer.WriteStartElement("SalesLine");//---------- Start Line 

                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                        Writer.WriteString("0");
                        Writer.WriteEndElement();
                        Int64 iDisNo = mUgR.Index + 1; 
                        Writer.WriteStartElement("InvoiceCMDistribution");
                        Writer.WriteString(iDisNo.ToString()   );
                        Writer.WriteEndElement();

                        //if (IsApplyToInv == true)
                        //{
                        //    Writer.WriteStartElement("Apply_To_Sales_Order");
                        //    Writer.WriteString("TRUE");
                        //    Writer.WriteEndElement();
                        //}
                        //else
                        //{
                        //    Writer.WriteStartElement("Apply_To_Sales_Order");
                        //    Writer.WriteString("FALSE");
                        //    Writer.WriteEndElement();
                        //}

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("-" + mUgR.Cells["ReturnQty"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(mUgR.Cells["ItemCode"].Value.ToString()   );
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(mUgR.Cells["Description"].Value.ToString()    );
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(mUgR.Cells["GL_Account"].Value.ToString()   );
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(mUgR.Cells["UnitPrice"].Value.ToString()   );
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(mUgR.Cells["AmountExcl"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();//------END SALES LINE

                    }
                }
                if (IsApplyToInv == true)
                {
                    Writer.WriteEndElement();//---------END PAW_Invoice

                    if (Convert.ToDouble(txtNBT.Value) > 0 || Convert.ToDouble(txtVAT.Value) > 0)
                    {
                        Writer.WriteStartElement("PAW_Invoice");
                        Writer.WriteAttributeString("xsi:type", "paw:invoice");

                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbCustomerCode.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Invoice_Number");
                        Writer.WriteString(txtReturnNo.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        Writer.WriteString(RtnDate);//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Sales_Representative_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Accounts_Receivable_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbARAccount.Value.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("CreditMemoType");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(NoOfDis.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesLines");
                        if (cmbNBTGL.Value.ToString().Trim() != string.Empty)
                        {
                            //----------3
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbNBTGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + txtNBT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------3
                        }
                        if (cmbVATGL.Value.ToString().Trim() != string.Empty)
                        {
                            //----------4
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbVATGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + txtVAT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------4
                        }
                        Writer.WriteEndElement();//----------END SALES LINES
                        Writer.WriteEndElement();//---------END PAW_Invoice
                       
                    }
                    
                }
                else
                {
                    if (Convert.ToDouble(txtNBT.Value) > 0)
                    {

                        if (cmbNBTGL.Value.ToString().Trim() != string.Empty)
                        {
                            //----------3
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbNBTGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + txtNBT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------3
                        }
                    } //-----END VAT
                    if (Convert.ToDouble(txtVAT.Value) > 0)
                    {

                        if (cmbVATGL.Value.ToString().Trim() != string.Empty)
                        {
                            //----------4
                            Writer.WriteStartElement("SalesLine");

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(cmbVATGL.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + txtVAT.Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("Apply_To_Sales_Order");
                            //Writer.WriteString("FALSE");
                            //Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            //------------4
                        }
                    } //-----END VAT
                    Writer.WriteEndElement();//----------END SALES LINES
                    Writer.WriteEndElement();//---------END PAW_Invoice
                    
                }
               
                // }
                Writer.WriteEndElement();//---------END PAW_Invoices
                Writer.Close();
                Connector abc = new Connector();//export to peach tree
                if (abc.ImportCustomerReturnAR() == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, sMsg, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            
        }

         private void tsbList_Click(object sender, EventArgs e)
        {
            frmInvoiceARRtnList  ObjInvoiceList = new frmInvoiceARRtnList ();
            ObjInvoiceList.Show();  
        }

         private void cmbItems_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                // tblItemMaster.ItemClass, tblItemWhse.ItemId, tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemWhse.WhseId, tblItemMaster.SalesGLAccount

                //Clear All Existing data
                if (UGItems.ActiveCell.Column.Key == "ItemCode" || UGItems.ActiveCell.Column.Key == "Description")
                {

                    //get Selected row
                    ugR = e.Row;

                    if (ugR != null)
                    {
                        UGItems.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value.ToString();
                        UGItems.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value.ToString();
                        UGItems.ActiveCell.Row.Cells["AvailableQty"].Value = Convert.ToDouble(ugR.Cells["QTY"].Value);
                        UGItems.ActiveCell.Row.Cells["WH"].Value = ugR.Cells["WhseId"].Value.ToString();
                        UGItems.ActiveCell.Row.Cells["GL_Account"].Value = ugR.Cells["SalesGLAccount"].Value.ToString();
                        UGItems.ActiveCell.Row.Cells["IsSerialItem"].Value    = ugR.Cells["ItemClass"].Value.ToString();   
                    }
                    else
                    {
                        foreach (UltraGridRow myugR in cmbItems.Rows)
                        {

                            if (myugR.Cells["CutomerID"].Value.ToString().Trim() == UGItems.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim())
                            {
                                UGItems.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value.ToString();
                                UGItems.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value.ToString();
                                UGItems.ActiveCell.Row.Cells["AvailableQty"].Value = Convert.ToDouble(ugR.Cells["QTY"].Value);
                                UGItems.ActiveCell.Row.Cells["WH"].Value = ugR.Cells["WhseId"].Value.ToString();
                                UGItems.ActiveCell.Row.Cells["GL_Account"].Value = ugR.Cells["SalesGLAccount"].Value.ToString();
                                UGItems.ActiveCell.Row.Cells["IsSerialItem"].Value = ugR.Cells["ItemClass"].Value.ToString();   
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbItemDescription_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {                             
                //get Selected row
                ugR = e.Row;

                if (ugR != null)
                {
                    UGItems.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value.ToString();
                    UGItems.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value.ToString();
                    UGItems.ActiveCell.Row.Cells["AvailableQty"].Value = Convert.ToDouble(ugR.Cells["TY"].Value);
                    UGItems.ActiveCell.Row.Cells["WH"].Value = ugR.Cells["WhseId"].Value.ToString();
                    UGItems.ActiveCell.Row.Cells["GL_Account"].Value = ugR.Cells["SalesGLAccount"].Value.ToString();
                    UGItems.ActiveCell.Row.Cells["IsSerialItem"].Value = ugR.Cells["ItemClass"].Value.ToString();   
                }
                else
                {
                    foreach (UltraGridRow myugR in cmbItems.Rows)
                    {

                        if (myugR.Cells["ItemId"].Value.ToString().Trim() == UGItems.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim())
                        {
                            UGItems.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value.ToString();
                            UGItems.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value.ToString();
                            UGItems.ActiveCell.Row.Cells["AvailableQty"].Value = Convert.ToDouble(ugR.Cells["QTY"].Value);
                            UGItems.ActiveCell.Row.Cells["WH"].Value = ugR.Cells["WhseId"].Value.ToString();
                            UGItems.ActiveCell.Row.Cells["GL_Account"].Value = ugR.Cells["SalesGLAccount"].Value.ToString();
                            UGItems.ActiveCell.Row.Cells["IsSerialItem"].Value = ugR.Cells["ItemClass"].Value.ToString();   
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWH_ValueChanged(object sender, EventArgs e)
        {
            if (cmbWH.Value != null)
            {
                GetItems(cmbWH.Value.ToString().Trim());     
            }
        }

        private void UGItems_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (UGItems.Rows.Count == 0)
                {
                    if (cmbCustomerCode.Value == null)
                    {
                        MessageBox.Show("Select Customer Code", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (cmbWH.Value == null)
                    {
                        MessageBox.Show("Select Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (cmbCustomerCode.Value != null && cmbWH.Value != null)
                    {
                        if (Convert.ToInt64(cmbReturnType.Value) == 2)//2- General Type
                        {
                            ugR = UGItems.DisplayLayout.Bands[0].AddNew();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void DeleteRows()
        {
            try
            {
                foreach (UltraGridRow myR in UGItems.Rows.All)
                {
                    myR.Delete(false);
                }
                foreach (UltraGridRow myR in UGSerial.Rows.All)
                {
                    myR.Delete(false);
                }
                foreach (UltraGridRow myR in UGDeliveryNote.Rows.All)
                {
                    myR.Delete(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void cmbReturnType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbReturnType.Value != null)
                {
                    if (Convert.ToInt64(cmbReturnType.Value) == 1)// Return Type From Invoice
                    {
                        UGItems.DisplayLayout.Bands[0].Columns["ItemCode"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["ItemCode"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["Description"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["Description"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["Qty"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["Qty"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["ReturnQty"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["ReturnQty"].CellActivation = Activation.AllowEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["GL_Account"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["GL_Account"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AvailableQty"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["AvailableQty"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["WH"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["WH"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].CellActivation = Activation.NoEdit;
                        cmbSalesRep.Enabled = true;
                        cmbWH.Enabled = true;
                        txtCustomerPO.Enabled = true;
                        cmbInvoiceType.Enabled = false;
                        DeleteRows();
                        GetInvoices();
                        GBInvoiceList.Visible = true;
                    }
                    else if (Convert.ToInt64(cmbReturnType.Value) == 2) //Return Type From General
                    {
                        UGItems.DisplayLayout.Bands[0].Columns["ItemCode"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["ItemCode"].CellActivation = Activation.AllowEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["Description"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["Description"].CellActivation = Activation.AllowEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["Qty"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["Qty"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["ReturnQty"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["ReturnQty"].CellActivation = Activation.AllowEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.AllowEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["GL_Account"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["GL_Account"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AvailableQty"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["AvailableQty"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["WH"].CellClickAction = CellClickAction.Edit;
                        UGItems.DisplayLayout.Bands[0].Columns["WH"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountIncl"].CellActivation = Activation.NoEdit;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].CellClickAction = CellClickAction.CellSelect;
                        UGItems.DisplayLayout.Bands[0].Columns["AmountExcl"].CellActivation = Activation.NoEdit;
                        cmbSalesRep.Enabled = true;
                        cmbSalesRep.ResetText();
                        cmbWH.Enabled = true;
                        cmbWH.ResetText();
                        txtCustomerPO.Enabled = true;
                        txtCustomerPO.ResetText();
                        cmbInvoiceType.Enabled = true;
                        cmbInvoiceType.ResetText();
                        //cmbCustomerCode.ResetText();  
                        DeleteRows();
                        GBInvoiceList.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void UGItems_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
            try
            {
                if (UGItems.ActiveCell.Column.Key == "ReturnQty" || UGItems.ActiveCell.Column.Key == "UnitPrice")
                {

                    UGItems.ActiveCell.Value = UGItems.ActiveCell.Text;
                    if (Convert.ToInt64(cmbReturnType.Value) == 1)  //If Return Type = Invoice Return 
                    {
                        if (Convert.ToInt64(UGItems.ActiveCell.Value) > Convert.ToInt64(UGItems.ActiveCell.Row.Cells["Qty"].Value))
                        {
                            MessageBox.Show("Return Quantity should be lessthen or equal to Invoice Quantity", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            UGItems.ActiveCell.Row.Cells["ReturnQty"].Value = Convert.ToInt64(UGItems.ActiveCell.Row.Cells["Qty"].Value);
                            e.Cancel = true;
                            return;
                        }
                    }
                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                }
                else if (UGItems.ActiveCell.Column.Key == "ItemCode" || UGItems.ActiveCell.Column.Key == "Description")
                {
                    UGItems.ActiveCell.Value = UGItems.ActiveCell.Text;
                    bool bHas = false;
                    if (UGItems.ActiveCell.Column.Key == "ItemCode")
                    {
                        foreach (UltraGridRow mUgR in cmbItems.Rows)
                        {
                            if (UGItems.ActiveCell.Value.ToString().Trim() == mUgR.Cells["ItemId"].Value.ToString().Trim())
                            {
                                bHas = true;
                            }
                        }


                    }
                    else if (UGItems.ActiveCell.Column.Key == "Description")
                    {
                        foreach (UltraGridRow mUgR in cmbItemDescription.Rows)
                        {
                            if (UGItems.ActiveCell.Value.ToString().Trim() == mUgR.Cells["ItemDis"].Value.ToString().Trim())
                            {
                                bHas = true;
                            }
                        }

                    }
                    if (UGItems.ActiveCell.Value.ToString().Trim().Length > 0)
                    {
                        if (bHas == false)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbSerial_Click(object sender, EventArgs e)
        {
            try
            {
                if (UGItems.Selected.Rows.Count > 0)
                {
                    if (UGItems.Selected.Rows.Count > 1)
                    {
                        MessageBox.Show("Multiple rows can not be seleted", "Return Serial No", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {

                        ugR = UGItems.Selected.Rows[0];
                        if (Convert.ToInt64(ugR.Cells["ReturnQty"].Value) == 0)
                        {
                            MessageBox.Show("Return Quantity can not be found", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        string sDeliNo = string.Empty;
                        if (Convert.ToInt64(cmbReturnType.Value) == 1)
                        {
                            foreach (UltraGridRow mUgR in UGDeliveryNote.Rows)
                            {
                                if (Convert.ToBoolean(mUgR.Cells["Select"].Value) == true)
                                {
                                    sDeliNo = mUgR.Cells["DeliveryNoteNo"].Value.ToString().Trim();
                                }
                            }
                        }
                        else if (Convert.ToInt64(cmbReturnType.Value) == 2)
                        {
                            sDeliNo = string.Empty;
                        }
                        if (Convert.ToInt64(ugR.Cells["IsSerialItem"].Value) == 10)    //Serial Item
                        {
                            frmInvoiceARRtnSerial ObjInvoiceSerial = new frmInvoiceARRtnSerial();
                            ObjInvoiceSerial.GetSerials(ugR.Cells["ItemCode"].Value.ToString().Trim(), cmbWH.Value.ToString().Trim(), "Sold", this, sDeliNo);
                            ObjInvoiceSerial.txtItemID.Text = ugR.Cells["ItemCode"].Value.ToString().Trim();
                            ObjInvoiceSerial.txtItemDescription.Text = ugR.Cells["Description"].Value.ToString().Trim();
                            ObjInvoiceSerial.txtWH.Text = cmbWH.Value.ToString().Trim();
                            ObjInvoiceSerial.txtQty.Value = ugR.Cells["ReturnQty"].Value;
                            ObjInvoiceSerial.Text = ugR.Cells["ItemCode"].Value.ToString().Trim();
                            ObjInvoiceSerial.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void UGItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (UGItems.ActiveCell.Column.Key == "UnitPrice")
                    {
                        if (Convert.ToInt64(cmbReturnType.Value) == 2)
                        {
                            if (UGItems.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                            {
                                if (UGItems.ActiveCell.Row.HasNextSibling() == false)
                                {
                                    ugR = UGItems.DisplayLayout.Bands[0].AddNew();
                                    ugR.Cells["ID"].Selected = true;
                                    ugR.Cells["ID"].Activated = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Select Item Code", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (UGItems.Selected.Rows.Count > 0)
                {
                    foreach (UltraGridRow mUgR in UGItems.Selected.Rows.All)
                    {
                        foreach (UltraGridRow ugSerial in UGSerial.Rows)
                        {
                            if (mUgR.Cells["ID"].Value == ugSerial.Cells["ID"].Value)
                            {
                                MessageBox.Show("You have already seleted serial number for Item " + mUgR.Cells["ItemCode"].Value.ToString() + " ,you can not delete serial number item after serial number entered", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        mUgR.Delete(false);
                    }
                }
                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                    mUgR.Cells["ID"].Value = mUgR.Index + 1;
                }
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmInvoiceARRtn_Load(object sender, EventArgs e)
        {
            try
            {
                GetInvoiceMasterData();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            } 
        } 
    }
}