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
    public partial class frmInvoiceAR : Form
    {
        clsCommon objclsCommon = new clsCommon();
        //Used to set SQL Statments
        string sSQL = string.Empty;
        string sMsg = "Peachtree - Customer Invoice";
        UltraGridRow ugR;
        //Common Sql Database Parameters
        SqlConnection sqlCon;
        SqlTransaction sqlTrans;
        SqlCommand sqlCMD;
        SqlDataAdapter sqlDA;
        DataSet sqlDS;
        bool bFilterCus = false;  //False - Code,True - Name
        bool bFilterSalesRep = false; //False - Code,True - Name

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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public frmInvoiceAR()
        {
            setConnectionString();
            InitializeComponent();
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
                throw ex;
                return 0;
            }
        }
        public int GetFieldLoopUp()
        {
            try
            {
                if (Directory.Exists(Application.StartupPath + "\\Lookup"))
                {
                    if (File.Exists(Application.StartupPath + "\\Lookup\\InvoiceLup.txt"))
                    {
                        StreamReader sr = new StreamReader(Application.StartupPath + "\\Lookup\\InvoiceLup.txt");
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
                throw ex;  
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
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;  
                return 0;
                throw ex;
            }
        }
        private void GetDeliveryNotes()
        {
            try
            {
                sSQL = "Select distinct(DeliveryNoteNo),SONos from tblDispatchOrder where CustomerID='" + cmbCustomerCode.Value.ToString().Trim() + "' and IsInvoce='0'";
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

                //Fill Delivery Note Details for particular customer
                if (sqlDS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                    {
                        ugR = UGDeliveryNote.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["DeliveryNoteNo"].Value = Dr["DeliveryNoteNo"].ToString();
                        ugR.Cells["SalesOrderNo"].Value = Dr["SONos"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GetDeliveryNote(string DeliveryNoteNo)
        {
            try
            {
                //Fill Header Details
                sSQL = " SELECT DISTINCT(DeliveryNoteNo) , CustomerID, SONos, DispatchDate, ArAccount, NoOfDistributions, CustomePO," +
                    " JobID, WareHouseID FROM tblDispatchOrder WHERE IsInvoce = 0 AND DeliveryNoteNo ='" + DeliveryNoteNo + "'";
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
                        txtCustomerPO.Text = Dr["CustomePO"].ToString();
                        cmbWH.Value = Dr["WareHouseID"].ToString();
                    }
                }

                //Fill Detail
                sSQL = "SELECT DistributionNo, ItemID, Description, OrderQty, DispatchQty, GL_Account, UnitPrice, " +
                    " Amount, TotalAmount, SODistributionNO FROM tblDispatchOrder WHERE IsInvoce = 0 AND DeliveryNoteNo ='" + DeliveryNoteNo + "'";
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
                        ugR.Cells["Description"].Value = Dr["Description"].ToString();
                        ugR.Cells["Qty"].Value = Dr["DispatchQty"].ToString();
                        ugR.Cells["GL_Account"].Value = Dr["GL_Account"].ToString();
                        ugR.Cells["SODistributionNO"].Value = Dr["SODistributionNO"].ToString();
                        ugR.Cells["UnitPrice"].Value = Convert.ToDouble(Dr["UnitPrice"]);
                    }
                }
                //Calculation Invoice Totals
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void codeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bFilterCus = false;
                SetFieldLoopUp(); //Saving Field Loopup data
                GetInvoiceMasterData(); //Refresh All Master data
                cmbCustomerCode_Leave(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bFilterCus = true;
                SetFieldLoopUp();//Saving Field Loopup data
                GetInvoiceMasterData();//Refresh All Master data
                cmbCustomerCode_Leave(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }            
        }

        private void cmbCustomerCode_Leave(object sender, EventArgs e)
        {
            //Will changes Lookup Field Property do not delete this event
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
                //Fill Delivery Notes Details
                GetDeliveryNotes(); 
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
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
                txtInvoiveNo.Text = "<Auto Number>";
                txtInvoiveNo.Enabled = false;
                dtpInvDate.Value = DateTime.Now;
                cmbInvoiceType.Value = 3; //1-Inclusive,2-Exclusive,3-non VAT 
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
                            GetDeliveryNote(e.Cell.Row.Cells["DeliveryNoteNo"].Value.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
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
                cmbCustomerCode.Text = "";

                txtBillAdd1.ResetText();
                txtBillAdd2.ResetText();
                txtBillAdd3.ResetText();
                txtBillAdd4.ResetText();
                txtShipAdd1.ResetText();
                txtShipAdd2.ResetText();
                txtShipAdd3.ResetText();
                txtShipAdd4.ResetText();
                txtInvoiveNo.Text = "<Auto Number>";
                txtInvoiveNo.Enabled = false;
                dtpInvDate.Value = DateTime.Now;
                cmbInvoiceType.Value = 3; //1-Inclusive,2-Exclusive,3-non VAT 
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

               // ugR = UGItems.DisplayLayout.Bands[0].AddNew();
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
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            } 
        }

        private void cbManualNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (cbManualNumber.Checked == true)
            {
                txtInvoiveNo.ResetText();
                txtInvoiveNo.Enabled = true;
            }
            else
            {
                txtInvoiveNo.ResetText();
                txtInvoiveNo.Text = "<Auto Number>";
                txtInvoiveNo.Enabled = false ;
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
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
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
                        dblLineQty =Convert.ToDouble( mUgR.Cells["Qty"].Value);
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
                        dblLineQty = Convert.ToDouble(mUgR.Cells["Qty"].Value);
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
                        dblLineQty = Convert.ToDouble(mUgR.Cells["Qty"].Value);
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

        private void frmInvoiceAR_Load(object sender, EventArgs e)
        {
            try
            {
                GetInvoiceMasterData();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            } 
        }
        private void  InvoicePrint(string InvNo)
        {
            try
            {
                DSInvoicing.Clear();
                sqlCon = new SqlConnection(ConnectionString);
                if (Convert.ToInt64(cmbInvoiceType.Value) == 1 || Convert.ToInt64(cmbInvoiceType.Value) == 3)
                {
                    sSQL = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "CustomerMaster1");

                    //Added by Chathura on 20/04/2011
                    //Added New Table into DSInvoice : tblItemMaster

                    sSQL = "SELECT    * FROM         tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "tblItemMaster");

                    //---------------------               
                    sSQL = "Select * from tblSalesInvoices where InvoiceNo = '" + InvNo + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dtInvoiceData");

                    frmInvPrint1 prininv = new frmInvPrint1(this);
                    prininv.Show();

                }
                else
                {
                    sSQL = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "CustomerMaster1");

                    //Added by Chathura on 20/04/2011
                    //Added New Table into DSInvoice : tblItemMaster

                    sSQL = "SELECT    * FROM         tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "tblItemMaster");

                    //---------------------

                    sSQL = "Select * from tblSalesInvoices where InvoiceNo = '" + InvNo + "'";// AND Refund <> '" + Ref + "'";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dtInvoiceData");

                    sSQL = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    sqlCMD = new SqlCommand(sSQL, sqlCon);
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDA.Fill(DSInvoicing, "dt_CompanyDetails");

                    frmInvPrint2 printax = new frmInvPrint2(this);
                    printax.Show();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (txtCustomerPO.Text.ToString().Trim().Length == 0)
            {
                txtCustomerPO.Text = "-";
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
                MessageBox.Show("Select Delivery Notes", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cbManualNumber.Checked == true)
            {
                if (txtInvoiveNo.Text.ToString().Trim().Length == 0)
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
            //if (Convert.ToDouble( txtInvoiceTotal.Value) == 0.00)
            //{
            //    MessageBox.Show("Invoice Total can not be 0.00", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            foreach (UltraGridRow mUgR in UGItems.Rows)
            {
                if (Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) == 0.00)
                {
                    MessageBox.Show("Unit Price can not be 0.00", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                   
                }
                if (Convert.ToDouble( mUgR.Cells["Qty"].Value )== 0.00)
                {
                    MessageBox.Show("Qty can not be 0.00", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpInvDate.Value)))
                    return;

                sqlCon = new SqlConnection(ConnectionString);
                sqlCon.Open();
                sqlTrans = sqlCon.BeginTransaction();               

                string sInvoiceNumber = string.Empty;
                string sDeliNoteNo = string.Empty;
                string sSONo = string.Empty;
                bool bIsExport = false;
                bool bIsReturn=false ;
                int DocType = 7;
                string TranType = "Invoice";
                bool QtyIN = false;

                //Get No Of Distribution 
                //NoOfDistrbution = NoOfItemRow + NBT + VAT
                //------------------
                int iNoOfDis = 0;
                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                    if(mUgR.Cells["ItemCode"].Value.ToString()!= string.Empty )
                    {
                        iNoOfDis = iNoOfDis + 1;
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
                        sDeliNoteNo = mUgR.Cells["DeliveryNoteNo"].Value.ToString().Trim();
                        sSONo = mUgR.Cells["SalesOrderNo"].Value.ToString().Trim();
                    }

                }

                if (cbManualNumber.Checked == true)
                {
                    sInvoiceNumber = txtInvoiveNo.Text.ToString().Trim();
                }
                else
                {
                    sSQL = "UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = InvoiceNo + 1 select InvoiceNo, InvoicePrefix from tblDefualtSetting with (rowlock)";
                    sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                    sqlCMD.CommandType = CommandType.Text;
                    sqlDA = new SqlDataAdapter(sqlCMD);
                    sqlDS = new DataSet();
                    sqlDA.Fill(sqlDS);

                    if (sqlDS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                        {
                            sInvoiceNumber  = Dr["InvoiceNo"].ToString().Trim().PadLeft(8, '0');

                            sInvoiceNumber  = Dr["InvoicePrefix"].ToString().Trim() + "-" + sInvoiceNumber ;
                        }
                    }
                    txtInvoiveNo.Text = sInvoiceNumber;
                }


                foreach (UltraGridRow mUgR in UGItems.Rows)
                {
                        double dblUnitCost = 0;
                        double dblAmount=0;

                        //sSQL = "select UnitCost from tblItemMaster where ItemID='" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'";
                        //sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        //sqlCMD.CommandType = CommandType.Text;
                        //dblUnitCost =(double)  sqlCMD.ExecuteScalar();

                        //if (dblUnitCost > Convert.ToDouble(mUgR.Cells["UnitPrice"].Value))
                        //{
                        //    MessageBox.Show("Item Selling Price less than item cost price", "Sales Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);     
                        //    sqlTrans.Rollback();
                        //    sqlCon.Close();
                        //    return;
                        //}
                        if (Convert.ToInt64( cmbInvoiceType.Value) == 1)
                        {
                            dblAmount =Convert.ToDouble( mUgR.Cells["AmountIncl"].Value );
                        }
                        else  if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                        {
                            dblAmount =Convert.ToDouble( mUgR.Cells["AmountExcl"].Value );
                        }
                        else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                        {
                            dblAmount =Convert.ToDouble( mUgR.Cells["AmountExcl"].Value );
                        }

                        sSQL = "Insert InTo tblSalesInvoices(InvoiceNo,CustomerID,DeliveryNoteNos,InvoiceDate," +
                            " ARAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice," +
                            " Discount,Amount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser," +
                            " IsExport,CustomerPO,Location,TTType1,TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,JobID," +
                            " Tax1Rate,Tax2Rate,SalesRep,CostPrrice,Tax3Rate, InvType) values ('" + sInvoiceNumber + "'," +
                            " '" + cmbCustomerCode.Value.ToString().Trim() + "'," +
                            " '" + sDeliNoteNo + "','" + dtpInvDate.Value.ToString("MM/dd/yyyy").Trim()  + "'," +
                            " '" + cmbARAccount.Value.ToString().Trim() + "'," + iNoOfDis + "," + mUgR.Index + 1 + "," +
                            " '" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'," +
                            " " + Convert.ToDouble(mUgR.Cells["Qty"].Value) + ",'" + mUgR.Cells["Description"].Value.ToString().Trim() + "'," +
                            " '" + mUgR.Cells["GL_Account"].Value.ToString().Trim() + "'," + Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) + "," +
                            " " + Convert.ToDouble(txtDiscount.Value) + "," + dblAmount + ",'" + txtNBT.Value + "','" + txtVAT.Value + "'," +
                            " " + Convert.ToDouble(txtInvoiceTotal.Value) + "," + Convert.ToDouble(txtInvoiceTotal.Value) + "," +
                            " '" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "'," +
                            " '" + user.userName.ToString().Trim() + "','" + bIsExport + "','" + txtCustomerPO.Text.ToString().Trim() + "'," +
                            " '" + cmbWH.Value.ToString().Trim() + "','" + cmbNBTGL.Value.ToString().Trim() + "','" + cmbVATGL.Value.ToString().Trim() + "'," +
                            " '" + bIsReturn + "','','0'," + Convert.ToDouble(mUgR.Cells["Qty"].Value) + ",''," +
                            " " + Convert.ToDouble(txtNBTPer.Value) + "," + Convert.ToDouble(txtVATPer.Value) + "," +
                            " '" + cmbSalesRep.Value.ToString().Trim() + "'," + dblUnitCost + ",'0'," + Convert.ToInt64( cmbInvoiceType.Value)   + ")";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();

                        sSQL = "Insert into tbItemlActivity (DocType,TranNo,TransDate,TranType,DocReference," +
                            " ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (" + DocType + "," +
                            " '" + sInvoiceNumber + "','" + dtpInvDate.Value.ToString("MM/dd/yyyy").Trim() + "'," +
                            " '" + TranType + "','" + QtyIN + "','" + mUgR.Cells["ItemCode"].Value.ToString().Trim() + "'," +
                            " " + Convert.ToDouble(mUgR.Cells["Qty"].Value) + "," + dblUnitCost + "," + Convert.ToDouble(dblUnitCost * Convert.ToDouble(mUgR.Cells["Qty"].Value)) + "," +
                            " '" + cmbWH.Value.ToString().Trim() + "'," + Convert.ToDouble(mUgR.Cells["UnitPrice"].Value) + ")";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();
                }
                //Update Peachtree
                if (CreateSalesInvXML(iNoOfDis, sDeliNoteNo, sSONo) == 0)
                {
                    sqlTrans.Rollback();
                    sqlCon.Close();
                    return;
                }
                //Update Delivery Note Status
                bool bIsInvoice=true;
                foreach (UltraGridRow mUgr in UGDeliveryNote.Rows)
                {
                    if (Convert.ToBoolean(mUgr.Cells["Select"].Value) == true)
                    {
                        sSQL = "update tblDispatchOrder SET IsInvoce = '" + bIsInvoice + "' WHERE DeliveryNoteNo = '" + mUgr.Cells ["DeliveryNoteNo"].Value .ToString().Trim() + "'";
                        sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                        sqlCMD.CommandType = CommandType.Text;
                        sqlCMD.ExecuteNonQuery();
                    }
                }
                sqlTrans.Commit();
                sqlCon.Close();  
                MessageBox.Show("Customer Invoice Successfuly Saved");
                if (MessageBox.Show("Do you want to print invoice now ?", "Print Invoice", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    InvoicePrint(sInvoiceNumber); 
                }
                NewInvoice(); 
            }           
            catch (Exception ex)
            {
                sqlTrans.Rollback();
                sqlCon.Close();
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }
            finally 
            {
                sqlCon.Close();  
            }
        }

        public int CreateSalesInvXML(int NoOfDis,string DeliNoteNo,string SONo)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(dtpInvDate.Value);
                string Dformat = "MM/dd/yyyy";
                string GRNDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerInvoice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;

                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                //=============Header of XML=========================
                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomerCode.Value.ToString().Trim());//Customer ID
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtInvoiveNo.Text.ToString().Trim()); //Inv No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbSalesRep.Value.ToString().Trim());//Sales Rep
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(dtpInvDate.Value.ToString("MM/dd/yyyy").Trim() );// Inv Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoOfDis.ToString());
                Writer.WriteEndElement();


                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteString(cmbARAccount.Value.ToString().Trim());   // AR Account
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Amount");
                Writer.WriteString(txtInvoiceTotal.Value .ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Customer_PO");
                Writer.WriteString(txtCustomerPO.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();

                //------------- 0 START
                Writer.WriteStartElement("SalesLines");
                foreach (UltraGridRow mUgr1 in UGItems.Rows)
                {
                    foreach (UltraGridRow mUgr2 in UGDeliveryNote.Rows)
                    {
                        if (Convert.ToBoolean(mUgr2.Cells["Select"].Value) == true)
                        {
                            sSQL = "Select SerialNO from tblInvoiceSerializeItem where INVNO='" + DeliNoteNo + "' and ItemID='" + mUgr1.Cells["ItemCode"].Value.ToString() + "'";
                            sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                            sqlCMD.CommandType = CommandType.Text;
                            sqlDA = new SqlDataAdapter(sqlCMD);
                            sqlDS = new DataSet();
                            sqlDA.Fill(sqlDS);
                            if (sqlDS.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                                {
                                    //----------1 START
                                    Writer.WriteStartElement("SalesLine");

                                    Writer.WriteStartElement("SalesOrderDistributionNumber");
                                    Writer.WriteString(mUgr1.Cells["SODistributionNO"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Apply_To_Sales_Order");
                                    Writer.WriteString("TRUE");
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("SalesOrderNumber");
                                    Writer.WriteString(SONo.ToString() );
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Quantity");
                                    Writer.WriteString(mUgr1.Cells["Qty"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Item_ID");
                                    Writer.WriteString(mUgr1.Cells["ItemCode"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Description");
                                    Writer.WriteString(mUgr1.Cells["Description"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("GL_Account");
                                    Writer.WriteString(mUgr1.Cells["GL_Account"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Amount");
                                    Writer.WriteString("-" + mUgr1.Cells["AmountExcl"].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Tax_Type");
                                    Writer.WriteString("1");
                                    Writer.WriteEndElement();

                                                                        

                                    Writer.WriteEndElement();
                                    //-----------1 END

                                    Writer.WriteStartElement("Serial_Number");
                                    Writer.WriteString(Dr["SerialNO"].ToString());
                                    Writer.WriteEndElement();

                                }
                            }
                            else  //if sqlDS rows count =0 then
                            {
                                string ItemClass = string.Empty;
                                sSQL = "Select * from tblItemMaster where ItemID  = '" + mUgr1.Cells["ItemCode"].Value.ToString() + "'";
                                sqlCMD = new SqlCommand(sSQL, sqlCon, sqlTrans);
                                sqlCMD.CommandType = CommandType.Text;
                                sqlDA = new SqlDataAdapter(sqlCMD);
                                sqlDS = new DataSet();
                                sqlDA.Fill(sqlDS);
                                if (sqlDS.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow Dr in sqlDS.Tables[0].Rows)
                                    {
                                        ItemClass = Dr["ItemClass"].ToString();
                                    }
                                    if (ItemClass == "10" || ItemClass == "11")
                                    {

                                    }
                                    else
                                    {
                                        //----------2 Start
                                        Writer.WriteStartElement("SalesLine");

                                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                                        Writer.WriteString(mUgr1.Cells["SODistributionNO"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Apply_To_Sales_Order");
                                        Writer.WriteString("TRUE");
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("SalesOrderNumber");
                                        Writer.WriteString(SONo.ToString() );
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Quantity");
                                        Writer.WriteString(mUgr1.Cells["Qty"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Item_ID");
                                        Writer.WriteString(mUgr1.Cells["ItemCode"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Description");
                                        Writer.WriteString(mUgr1.Cells["Description"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("GL_Account");
                                        Writer.WriteString(mUgr1.Cells["GL_Account"].Value.ToString());
                                        Writer.WriteEndElement();


                                        Writer.WriteStartElement("Amount");
                                        Writer.WriteString("-" + mUgr1.Cells["AmountExcl"].Value.ToString());
                                        Writer.WriteEndElement();


                                        Writer.WriteStartElement("Tax_Type");
                                        Writer.WriteString("1");
                                        Writer.WriteEndElement();

                                                                      

                                        Writer.WriteEndElement();
                                        //---------- 2 END
                                    }

                                }
                            }

                        }
                    }
                }
                //-----------START NBT
                if (Convert.ToDouble(txtNBT.Value) > 0)
                {

                    if (cmbNBTGL.Value.ToString().Trim() != string.Empty)
                    {
                        //----------3
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
                        Writer.WriteString(cmbNBTGL.Text .ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + txtNBT.Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

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

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbVATGL.Text .ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + txtVAT.Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                        //------------4
                    }
                } //-----END VAT


                Writer.WriteEndElement();
                //------------- 0 END
                Writer.WriteEndElement();
                //-----PAW_Invoice
                Writer.WriteEndElement();
                //-----PAW_Invoices END

                Writer.Close();

                Connector abc = new Connector();//export to peach tree
                if (abc.ImportCustomerInvoice () == 0)
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
                throw ex;
            }            
        }

        private void tsbReprint_Click(object sender, EventArgs e)
        {

        }

        private void tsbList_Click(object sender, EventArgs e)
        {
            frmInvoiceARList ObjInvoiceList = new frmInvoiceARList();
            ObjInvoiceList.Show();  
        }

        private void txtInvoiveNo_ValueChanged(object sender, EventArgs e)
        {

        }

       
        

       

        

        
       

       
    }
}