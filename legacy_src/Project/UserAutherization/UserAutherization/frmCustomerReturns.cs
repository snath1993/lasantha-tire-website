using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


namespace UserAutherization
{
    public partial class frmCustomerReturns : Form
    {

//        colItemCode - 0
//1 "colDesc"
//2 colInvQty
//3 colRetQty
//4 UOM
//5 colUnitPrice
//colUnitPriceIncl
//6 Discount
//7 colAmount
//colAmountIncl
//colLineTax
//colLineDisc
//8 colGL

        public frmCustomerReturns()
        {
            InitializeComponent();
            setConnectionString();
            IsFind = false;
        }

        public frmCustomerReturns(string RetNo)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = true;
            flglist = 1;
            txtCreditNoteNo.Text = RetNo;            
        }

        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL, SERID, SERDIS, SERGL;

        public DSCustomerReturn ds = new DSCustomerReturn();
        public static DateTime UserWiseDate = System.DateTime.Now;
        public static string ConnectionString;
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        clsCommon objclsCommon = new clsCommon();
        bool IsFind = false;

        public string StrSql;

        public string sMsg = "MultiWarehouse Module - Delivery Note";

        int flglist = 0;
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
        double Tax1Rate = 0;
        double Tax2Rate = 0;

        //  bool formload = false;
        private void LoadtaxDetails()
        {
            try
            {
                //                SELECT     TaxID, TaxName, Rate, Rank, Account, IsActive, IsTaxOnTax
                //FROM         tblTaxApplicable

                String S1 = "Select * from tblTaxApplicable order by Rank";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                    txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

                    Tax1ID = dt.Rows[0]["TaxID"].ToString();
                    Tax2ID = dt.Rows[1]["TaxID"].ToString();

                    Tax1Name = dt.Rows[0]["TaxName"].ToString();
                    Tax2Name = dt.Rows[1]["TaxName"].ToString();

                    Tax1Rate = double.Parse(dt.Rows[0]["Rate"].ToString());
                    Tax2Rate = double.Parse(dt.Rows[1]["Rate"].ToString());

                    Tax1GLAccount = user.TaxPayGL1;
                    Tax2GLAccount = user.TaxPayGL2;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //********************************************************

        //*******************************************************
        public DataTable FillVendor()
        {
            try
            {
                DataTable dataTable = new DataTable("Vendor");
                string ConnString = ConnectionString;
                string sql = "Select CutomerID,CustomerName from tblCustomerMaster order by CutomerID";// where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                dataTable.Columns.Add("VendorID", typeof(String));
                dataTable.Columns.Add("VendorName", typeof(String));
                // dataTable.Columns.Add("","");
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //  dataTable.Rows.Add(new String[] { "", " " });//, null,""  });//, null
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        dataTable.Rows.Add(new String[] { reader.GetString(0).Trim(), reader.GetString(1).Trim() });//, reader.GetString(3).ToString().Trim(),reader.GetString(4).ToString().Trim()});//, reader.GetValue(3).ToString(), reader.GetValue(4).ToString()
                    }
                }
                reader.Close();
                Conn.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //********************************************************
        public int Decimalpoint = 2;//validate for price
        public int DecimalpointQuantity = 2;//validate for quabtity
        public void load_Decimal()
        {
            try
            {
                string FType = "Price";
                String S = "Select CurentDecimal from tblDecimal where FieldType='" + FType + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                Decimalpoint = Convert.ToInt16(dt.Tables[0].Rows[0].ItemArray[0]);
                //==========================================================================
                string FType1 = "Quantity";
                String S1 = "Select CurentDecimal from tblDecimal where FieldType='" + FType1 + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet dt1 = new DataSet();
                da1.Fill(dt1);
                DecimalpointQuantity = Convert.ToInt16(dt1.Tables[0].Rows[0].ItemArray[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //****************************************
        public void GetCurrentUserDate()
        {
            try
            {
                dtpCreditDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpCreditDate.Value = UserWiseDate;
                //    //.ToString().Trim();
                //    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //*****************************************************
        public static bool TaxMethod = false;
        public void LoadTaxMethod()
        {
            try
            {
                String S = "Select IsTaxOnTax from tblDefualtSetting";// where UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    TaxMethod = Convert.ToBoolean(dt.Tables[0].Rows[i].ItemArray[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //***********************************************
        bool TaxApplicable = false;
        private void TaxValidation()
        {
            try
            {
                //check the systemis tax appplicable********************
                // bool TaxApplicable = false;
                String S3 = "select IsTaxApplicable from tblDefualtSetting";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt3.Rows[0].ItemArray[0]) == true)
                    {
                        //if this variable is false tax table is not visible
                        TaxApplicable = true;
                    }
                    else
                    {
                        TaxApplicable = false;
                    }
                }
                //*******************************************
                //if (TaxApplicable == false)
                //{
                //    dgvTaxApplicable.Visible = false;
                //}
                //else
                //{
                //    //commented by tharaka
                //    //dgvTaxApplicable.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //================================================
        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster order by IsDefault";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 125;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=================================================
        public bool flag = false;

        public void GetCustomer()
        {

            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster order by CutomerID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    return base.ProcessDialogKey(Keys.Tab);
            }
            return base.ProcessDialogKey(keyData);
        }
        private void frmCustomerReturns_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsFind)
                {
                    //aaaa
                    ClearValues();
                    GetCustomer();
                    GetWareHouseDataSet();
                    SetDefaultWarehouse();
                    TaxValidation();
                    Enable();
                    LoadTaxMethod();
                    GetCurrentUserDate();
                    Disable();
                    // WarehouseDataLoad();
                    LoadtaxDetails();
                    load_Decimal();//load the numbe rof deccimal point for field
                    // CusDataLoad();

                    GetChargeItems();

                    loadChartofAcount();
                    LoadDefualtAccount();
                    btnRePrint.Enabled = true;
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void GetChargeItems()
        {
            try
            {

                StrSql = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='1'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    LineDisitemid = dt.Rows[0].ItemArray[0].ToString();
                    LineDisitemdescription = dt.Rows[0].ItemArray[1].ToString();
                    LineDisGLAccount = dt.Rows[0].ItemArray[2].ToString();
                }

                string StrSql2 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='3'";

                SqlCommand cmd2 = new SqlCommand(StrSql2);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql2, ConnectionString);
                DataTable dt2 = new DataTable();
                dt2.Clear();
                da2.Fill(dt2);
                {
                    Cashitemid = dt2.Rows[0].ItemArray[0].ToString();
                    cashitemdis = dt2.Rows[0].ItemArray[1].ToString();
                    cashGL = dt2.Rows[0].ItemArray[2].ToString();
                }
                string StrSql3 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='4'";

                SqlCommand cmd3 = new SqlCommand(StrSql3);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql3, ConnectionString);
                DataTable dt3 = new DataTable();
                dt3.Clear();
                da3.Fill(dt3);
                {
                    NBitemid = dt3.Rows[0].ItemArray[0].ToString();
                    NBTitemDis = dt3.Rows[0].ItemArray[1].ToString();
                    NBTitemGL = dt3.Rows[0].ItemArray[2].ToString();
                }

                string StrSql4 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='5'";

                SqlCommand cmd4 = new SqlCommand(StrSql4);
                SqlDataAdapter da4 = new SqlDataAdapter(StrSql4, ConnectionString);
                DataTable dt4 = new DataTable();
                dt4.Clear();
                da4.Fill(dt4);
                {
                    VATitemid = dt4.Rows[0].ItemArray[0].ToString();
                    VATitemDis = dt4.Rows[0].ItemArray[1].ToString();
                    VATGL = dt4.Rows[0].ItemArray[2].ToString();
                }


                string StrSql5 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='6'";

                SqlCommand cmd5 = new SqlCommand(StrSql5);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql5, ConnectionString);
                DataTable dt5 = new DataTable();
                dt5.Clear();
                da5.Fill(dt5);
                {
                    SERID = dt5.Rows[0].ItemArray[0].ToString();
                    SERDIS = dt5.Rows[0].ItemArray[1].ToString();
                    SERGL = dt5.Rows[0].ItemArray[2].ToString();
                }






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }  //this metod get all details from warehousetable=================

        //====================================================

        public void Enable()
        {
            try
            {
                // mltcmbVendor.Enabled = true;
                dtpCreditDate.Enabled = true;
                clistbxInvoices.Enabled = true;

                dgvCustomerReturn.Enabled = true;
                dgvCustomerReturn.ReadOnly = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Disable()
        {
            try
            {
                // mltcmbVendor.Enabled = false;
                dtpCreditDate.Enabled = false;
                cmbARAccount.Enabled = false;
                //  chkReferingSI.Enabled = false;
                //cmbLocation.Enabled = false;
                //txtlocName.Enabled = false;
                //txtLocAdd1.Enabled = false;
                // txtLocAdd2.Enabled = false;
                txtCustomerPO.Enabled = false;

                // cmbtaxSys1.Enabled = false;
                //  cmbtaxSys2.Enabled = false;

                // txtTax1Amount.Enabled = false;
                //txtTax2.Enabled = false;

                //txtDiscountAmount.Enabled = false;
                // txtDisRate.Enabled = false;
                txtNetTotal.Enabled = false;

                txtcusName.Enabled = false;
                //  txtSupCity.Enabled = false;
                txtCusAdd1.Enabled = false;
                txtCusAdd2.Enabled = false;
                txtCreditNoteNo.Enabled = true;
                txtTotalAmount.Enabled = false;
                dgvCustomerReturn.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //========================================================
        public DataTable filldata()
        {
            try
            {
                DataTable dataTable = new DataTable("Item");
                string ConnString = ConnectionString;
                string sql = "select ItemID,ItemDescription,UnitPrice,SalesGLAccount,UOM from tblItemMaster";// where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
                //string sql = "select ItemID,Item_Description,ItemClass,PartNumber,LastUnitCost,PriceLevel from Item where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                dataTable.Columns.Add("ItemID", typeof(String));
                dataTable.Columns.Add("Description", typeof(String));
                // dataTable.Columns.Add("UOM", typeof(String));
                //dataTable.Columns.Add("colUnitPrice", typeof(String));

                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                // dataTable.Rows.Add(new String[] { "Item ID","Description"});//, null,""  });//, null
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        dataTable.Rows.Add(new String[] { reader.GetString(0).Trim(), reader.GetString(1).Trim() });//, reader.GetString(3).ToString().Trim(),reader.GetString(4).ToString().Trim()});//, reader.GetValue(3).ToString(), reader.GetValue(4).ToString()
                    }
                }
                reader.Close();
                Conn.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // fillCombobox fil = new fillCombobox();  //Class to load the data to system
        //=================================================================
        //public void databindCombobox_Purchase()
        //{
        //    DataTable S_data2 = new DataTable();
        //    try
        //    {
        //        S_data2 = this.filldata();
        //        if (S_data2.Rows.Count > 0)
        //        {
        //            mltcmbboxItemSelect.DataSource = S_data2;
        //            mltcmbboxItemSelect.DisplayMember = "ItemID";
        //            mltcmbboxItemSelect.ValueMember = "Description";
        //            mltcmbboxItemSelect.Text = "";
        //        }

        //    }
        //    catch { }
        //}


        //public void TaxRateLoad()
        //{
        //    cmbtaxSys1.Items.Clear();
        //    cmbtaxSys2.Items.Clear();
        //    String S = "Select TaxName from tblTax";
        //    SqlCommand cmd = new SqlCommand(S);
        //    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //    DataSet dt = new DataSet();
        //    da.Fill(dt);

        //    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //    {
        //        cmbtaxSys1.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //        cmbtaxSys2.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //    }

        //}
        public void LoadDefualtAccount()
        {
            try
            {
                String S = "Select CusretnDrAc from tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbARAccount.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void loadChartofAcount()
        {
            try
            {
                String S = "Select * from tblChartofAcounts";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void CusDataLoad()
        //{
        //    String S = "Select * from tblCustomerMaster";
        //    SqlCommand cmd = new SqlCommand(S);
        //    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //    DataSet dt = new DataSet();
        //    da.Fill(dt);

        //    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //    {
        //        cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //    }


        //}

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //clistbxInvoices.Items.Clear();
            //loadAddress();
            //// dgvCustomerReturn 
            //Load_Invoices();//this mehod load al sales orders into to the cobobox regard the selected customer
            // dgvCustomerReturn.Rows.Clear();
        }

        private void Load_Invoices(string CustomerID)
        {
            try
            {
                dgvCustomerReturn.Rows.Clear();
                txtNetTotal.Text = "0.00";
                clistbxInvoices.Items.Clear();

                bool Isinvoce = false;
                String S = "Select distinct(InvoiceNo),InvoiceDate from tblSalesInvoices where CustomerID='" + CustomerID + "' and IsReturn='" + Isinvoce + "' and IsConfirm = '"+true+"' and IsVoid = '"+false+"' order by InvoiceNo";//and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    clistbxInvoices.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                   // clistbxInvoices.Items.Add(dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkReferingSI_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkReferingSI.CheckState == CheckState.Checked)
            //{
            //    lblcheckinvoice.Visible = true;
            //    clistbxInvoices.Visible = true;
            //    dgvCustomerReturn.Columns[1].Visible = true;
            //    dgvCustomerReturn.Columns[1].Width = 75;
            //    dgvCustomerReturn.Columns[0].Width = 100;
            //    // lblInvoicedQty.Visible = true;
            //}
            //else
            //{
            //    lblcheckinvoice.Visible = false;
            //    clistbxInvoices.Visible = false;
            //    dgvCustomerReturn.Columns[1].Visible = false;
            //    dgvCustomerReturn.Columns[0].Width = 175;
            //    //lblInvoicedQty.Visible = false;
            //    dgvCustomerReturn.Rows.Clear();
            //    for (int i = 0; i < clistbxInvoices.Items.Count; i++)
            //    {
            //        clistbxInvoices.SetItemCheckState(i, CheckState.Unchecked);
            //    }

            //    // btnNew_Click(sender, e);
            //}
        }
        // int currentRowIndex1;
        Point defaultLocation1;
        int clickCount = 0;
        private void dgvCustomerReturn_CellClick(object sender, DataGridViewCellEventArgs e)
        {          
            if (dgvCustomerReturn.CurrentRow.Cells[0].Value != null)
            {
                if (IsThisItemSerial(dgvCustomerReturn.CurrentRow.Cells[0].Value.ToString()))
                    btnSNO.Enabled = true;
                else
                    btnSNO.Enabled = false;
            }       
            
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                //if (dgvGRNTransaction.CurrentRow.Cells[0].Value == null) return false;
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + _ItemCode + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ItemClass == "10" || ItemClass == "11")
                {
                    IsThisItemSerial = true;
                }
                return IsThisItemSerial;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int calculateRowsHeightForPO()
        {
            try
            {
                int rowsHeight = 0;

                DataGridViewRow row;
                for (int i = newVal1; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    row = dgvCustomerReturn.Rows[i];

                    if (row.Visible)
                        rowsHeight += row.Height;

                    if (currentRowIndex1 == i)
                        break;
                }
                return rowsHeight + 5;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public bool ISSAVE = false;//if save this variable ge true
        // int k = 0;


        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    if (dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value != null && dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString() != string.Empty) //change cell value by 1                   
                    {
                        RowCount++;
                    }

                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetFilledRowsRetQtyAvailble()
        {
            try
            {
                int RetRowCount = 0;

                for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    if (dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value != null && dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString() != string.Empty) //change cell value by 1                   
                    {
                        
                        if (dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value != null &&
                            dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString().Trim() != string.Empty &&
                            double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString().Trim()) > 0)
                        {
                            RetRowCount++;
                        }

                    }

                }
                return RetRowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetFilledRows_new()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    if (dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value != null && dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString() != string.Empty) //change cell value by 1                   
                    {
                        if (dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value != null &&
                            dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString().Trim() != string.Empty 
                            )
                        {
                            RowCount++;
                        }
                    }

                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public int GetFilledRowsForXMLExport()
        //{
        //    try
        //    {
        //        int RowCount = 0;

        //        for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
        //        {
        //            if (Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value) > 0) //change cell value by 1                   
        //            {
        //                RowCount++;
        //            }

        //        }
        //        return RowCount;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public int ChechDQty = 0;

        public int GetFilledRowstax()
        {
            try
            {
                int RowCounttax = 0;
                if (double.Parse(txtNBT.Value.ToString().Trim()) > 0)
                    RowCounttax = 1;

                if (double.Parse(txtVat.Value.ToString().Trim()) > 0)
                    RowCounttax = RowCounttax + 1;

                return RowCounttax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //******************************************
        public string Tax1ID = "";
        public string Tax1Name = "";
        public double Tax1Amount = 0.0;
        public string Tax1GLAccount = "";

        public string Tax2ID = "";
        public string Tax2Name = "";
        public double Tax2Amount = 0.0;
        public string Tax2GLAccount = "";

        public string Tax3ID = "";
        public string Tax3Name = "";
        public double Tax3Amount = 0.0;
        public string Tax3GLAccount = "";
        public int INVTYPE = 1;
        public int INVEXINC = 5;  

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvCustomerReturn.EndEdit();
            dgvCustomerReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (dtpCreditDate.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dtpCreditDate.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //if (dgvCustomerReturn.CurrentCell == null) return;
            //dgvCustomerReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //dgvCustomerReturn.CurrentCell = dgvCustomerReturn.CurrentRow.Cells["colAmount"];
            // int INVTYPE = 1;

            if (rdoNonVat.Checked == true)
            {
                INVTYPE = 1;
            }
            if (rdoTax.Checked == true)
            {
                INVTYPE = 2;
                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)
                {
                    INVEXINC = 5;
                }
                if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                {
                    INVEXINC = 6;
                }
            }
            if (rdoSVat.Checked == true)
            {
                INVTYPE = 3;
            }

            bool IsItemSerial = false;
            int rowCount = GetFilledRows();
           
            if (!IsSerialNoCorrect())
                return;

            if (Convert.ToDouble(txtNetTotal.Text) == 0)
            {
                MessageBox.Show("Enter Return Quantity");
                return;
            }

             if (!user.IsCRTNNoAutoGen)
            {
                if (txtCreditNoteNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter CRN No....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            //check wether this item is serialized or not=======================
            try
            {
                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgvCustomerReturn.Rows[a].Cells["colItemCode"].Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (!user.IsReturnOverCustInv)
                    {
                        if (double.Parse(dgvCustomerReturn.Rows[a].Cells["colInvQty"].Value.ToString()) < double.Parse(dgvCustomerReturn.Rows[a].Cells["colRetQty"].Value.ToString()))
                        {
                            MessageBox.Show("Not allowed more than Invoiced Qty for " + dgvCustomerReturn.Rows[a].Cells["colItemCode"].Value.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    }
                    if (ItemClass == "10" || ItemClass == "11")
                    {
                        //IsItemSerial = true;
                        //String S1 = "Select SerialNO from tblSerialCusReturnTemp where ItemID  = '" + dgvCustomerReturn.Rows[a].Cells["colItemCode"].Value.ToString().Trim() + "'";
                        //SqlCommand cmd1 = new SqlCommand(S1);
                        //SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        //DataSet dt1 = new DataSet();
                        //da1.Fill(dt1);
                        //if (Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colRetQty"].Value) == dt1.Tables[0].Rows.Count)
                        //{
                        //    IsItemSerial = false;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }

            ISSAVE = true;
            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid
            for (int a = 0; a < rowCounttax; a++)
            {
                //if (a == 0)
                //{
                //    Tax1ID = Tax1ID;
                //    Tax1Name = dgvTaxApplicable.Rows[a].Cells[0].Value.ToString();
                Tax1Amount = Convert.ToDouble(txtNBT.Value.ToString());
                //    Tax1GLAccount = dgvTaxApplicable.Rows[a].Cells["UOM"].Value.ToString();
                //}
                //if (a == 1)
                //{
                //    Tax2ID = dgvTaxApplicable.Rows[a].Cells["colUnitPrice"].Value.ToString();
                //    Tax2Name = dgvTaxApplicable.Rows[a].Cells[0].Value.ToString();
                Tax2Amount = Convert.ToDouble(txtVat.Value.ToString());
                //    Tax2GLAccount = dgvTaxApplicable.Rows[a].Cells["UOM"].Value.ToString();
                //}
                //if (a == 2)
                //{
                //    Tax3ID = dgvTaxApplicable.Rows[a].Cells["colUnitPrice"].Value.ToString();
                //    Tax3Name = dgvTaxApplicable.Rows[a].Cells[0].Value.ToString();
                //    Tax3Amount = Convert.ToDouble(dgvTaxApplicable.Rows[a].Cells["colRetQty"].Value);
                //    Tax3GLAccount = dgvTaxApplicable.Rows[a].Cells["UOM"].Value.ToString();
                //}
            }
            try
            {
                //check the qty is whether a number or not
                for (int a = 0; a < dgvCustomerReturn.Rows.Count - 1; a++)
                {
                    ChechDQty = 0;
                    Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colInvQty"].Value);
                    Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colUnitPrice"].Value);
               //     Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["Discount"].Value);
                    //Convert.ToDouble(txtTax1Amount.Text);
                    //Convert.ToDouble(txtTax2.Text);
                    //Convert.ToDouble(txtDisRate.Text);
                    //Convert.ToDouble(txtDiscountAmount.Text);
                }
            }
            catch (Exception ex)
            {
                ChechDQty = 1;//if this flag is 1 the violate the number format
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }

            //here check numeric convertion======================================
            if (ChechDQty == 0)
            {
                //here if checkDQty is 1 thre is aninvaid numberformat or charactor 
                //now proceed to save

                //declare a Cutomer Retun No
                string CustomerReturnNo = "";
                //here check blank field
                if (txtTotalAmount.Text == "" || cmbWarehouse.Value.ToString().Trim() == "" || IsItemSerial == true || rowCount == 0)
                {
                    if (rowCount == 0)
                    {
                        MessageBox.Show("Pleease select an Invoice");
                    }
                    else if (IsItemSerial == true)
                    {
                        MessageBox.Show("There are Serialize Stock Items you have not selected in this transaction");
                        // MessageBox.Show("You have not entered serial numbers for this items");
                        //btnSave.Focus();

                    }
                    else
                    {
                        //here user has missed mandotory field
                        MessageBox.Show("Enter Mandotory field");
                        //btnSave.Focus();
                    }
                }
                else
                {
                    //here proceed save process
                    //take the filled rowcount of the db
                    //int rowCount = GetFilledRows();

                    //before save Convert System Date into our real datetime type=========
                    //dtpCreditDate is a DateTimePicker and get it text then convert into our format
                    DateTime DTP = Convert.ToDateTime(dtpCreditDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string CreditDate = DTP.ToString(Dformat);
                    //CreditDate is use to save in the database

                    setConnectionString();
                    SqlConnection myConnection = new SqlConnection(ConnectionString);
                    //SqlCommand myCommand = new SqlCommand();
                    //SqlCommand myCommand1= new SqlCommand();
                    // SqlCommand myCommand2 = new SqlCommand();
                    myConnection.Open();

                    SqlTransaction myTrans = myConnection.BeginTransaction(); ;
                    //myConnection.Open();
                    //myCommand.Connection = myConnection;
                    // myCommand1.Connection = myConnection;
                    //myCommand2.Connection = myConnection;

                    //myTrans = myConnection.BeginTransaction();
                    //myCommand.Transaction = myTrans;
                    // myCommand1.Transaction = myTrans;
                    //  myCommand2.Transaction = myTrans;

                    try
                    {
                        SqlCommand myCommand = null;
                        if (user.IsCRTNNoAutoGen)
                        {
                            //start to write code here

                            // select a defualt Credit number from Defualt setting table while keeping lock untill get updated
                            // myCommand.CommandText = "UPDATE tblDefualtSetting SET CustomerReturnNo = CustomerReturnNo + 1 ";//select CustomerReturnNo, CusReturnPrefix from tblDefualtSetting with (rowlock)";
                            //myCommand.ExecuteNonQuery();
                            myCommand = new SqlCommand("UPDATE tblDefualtSetting SET CusReturnNo = CusReturnNo + 1 select CusReturnNo, CusReturnPrefix from tblDefualtSetting", myConnection, myTrans);

                            //  SqlCommand myCommand = new SqlCommand("select CusReturnNo, CusReturnPrefix from tblDefualtSetting",myConnection, myTrans);
                            // myCommand.CommandText = "select CustomerReturnNo, CusReturnPrefix from tblDefualtSetting ";//with (holdlock)";
                            // myCommand.ExecuteNonQuery();

                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            if (dt41.Rows.Count > 0)
                            {
                                CustomerReturnNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                                CustomerReturnNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + CustomerReturnNo;
                            }
                            flglist = 0;
                            //Assiging a new number to the textbox 
                            txtCreditNoteNo.Text = CustomerReturnNo;
                            
                        }
                        else
                        {
                             //tblCutomerReturn(CustomerID,CreditNo
                            myCommand = new SqlCommand("select * from tblCutomerReturn where CreditNo='" + txtCreditNoteNo.Text.Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            if (dt41.Rows.Count > 0)
                            {
                                MessageBox.Show("CRN No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                myTrans.Rollback();
                                myConnection.Close();//
                                return;
                            }
                        }

                        //satar for loop for saving datagridrecord untill reach datagrid row count
                        //row count is a filled count
                        for (int i = 0; i < rowCount; i++)
                        {
                            //declare boolinyan variables defuaalt is false

                            string TranType = "CreditNote";
                            bool IsApplyToInvoice = false;//Check this invoice is apply to invoice or  not
                            bool ISExport = false;//check wehter this transaction exported to peachtree
                            bool IsFullInvReturn = false;//check wether it setof the invoice qty or not

                            //here validating Receive quantity it canot be 0, must be greater than 0
                            if (Convert.ToDouble(dgvCustomerReturn[2, i].Value) != 0)
                            {
                                bool Duplicate = true;


                                if (Convert.ToDouble(dgvCustomerReturn[3, i].Value) > Convert.ToDouble(dgvCustomerReturn[2, i].Value))
                                {
                                    MessageBox.Show("You cannot return more than invoice Quantity");
                                    myTrans.Rollback();
                                    return;
                                }
                                //insert data into Cutomer return table
                                if (Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value) > 0)
                                {
                                    SqlCommand myCommand1 = new SqlCommand("insert into tblCutomerReturn " +
                                        " (CustomerID,CreditNo,ReturnDate,LocationID,IsApplyToInvoice,InvoiceNO,ARAccount, " +
                                        " NoofDistribution,DistributionNo,ItemID,InvoiceQty,ReturnQty,Description,UOM,UnitPrice, " +
                                        " LineDiscP,Amount,GL_Account,NBT,VAT,GrossTotal,GrandTotal,ISExport,CurrenUser,IsFullInvReturn, " +
                                        " JobID,Tax1ID,Tax2ID,Type,SalesRep,InvType,Discount,ServCharg,NBTP,VATP) values ('" +
                                        cmbCustomer.Value.ToString().Trim() + "','" + txtCreditNoteNo.Text.ToString().Trim() + "','" +
                                        CreditDate + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + IsApplyToInvoice + "','" +
                                        SelectInvNO1 + "','" + cmbARAccount.Text.ToString().Trim() + "','" + rowCount.ToString().Trim()
                                        + "','" + (i + 1).ToString().Trim() + "','" + dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString().Trim() +
                                        "','" + Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colInvQty"].Value) + "','" +
                                        Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value) + "','" +
                                        dgvCustomerReturn.Rows[i].Cells["colDesc"].Value.ToString().Trim() + "','" +
                                        dgvCustomerReturn.Rows[i].Cells["UOM"].Value.ToString().Trim() + "','" +
                                        Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value) + "','" +
                                        dgvCustomerReturn.Rows[i].Cells["Discount"].Value + "','" +
                                        Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colAmount"].Value) + "','" +
                                        dgvCustomerReturn.Rows[i].Cells["colGL"].Value.ToString().Trim() + "','" +
                                        Convert.ToDouble(Tax1Amount) + "','" + Convert.ToDouble(Tax2Amount) + "','" +
                                        Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text)
                                        + "','" + ISExport + "','" + user.userName.ToString().Trim() + "','" +
                                        IsFullInvReturn + "','" + cmbjob.Text.ToString().Trim() + "','" + Tax1Name
                                        + "','" + Tax2Name + "','ApplyToInvoice','" + cmbSalesRepID.Text.ToString().Trim()
                                        + "','" + INVTYPE + "','" + txtDiscPer.Value.ToString().Trim() + "','" + txtServCharges.Text.Trim() + "','" + txtNBTPer.Text.Trim() + "','" + txtVatPer.Text.Trim() + "')", myConnection, myTrans);
                                    myCommand1.ExecuteNonQuery();
                                }
                                SqlCommand myCommand2 = new SqlCommand("Select * from  tblItemWhse where ItemId='" + dgvCustomerReturn[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);
                                //  myCommand2.ExecuteNonQuery();

                                //check weher the item is altready in the warehouse table
                                SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                                DataTable dt1 = new DataTable();
                                da1.Fill(dt1);
                                if (dt1.Rows.Count > 0)
                                {
                                    //if exsist update
                                    SqlCommand myCommand3 = new SqlCommand("update tblItemWhse set QTY = QTY + '" + Convert.ToDouble(dgvCustomerReturn[3, i].Value) + "' where ItemId='" + dgvCustomerReturn[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);
                                    myCommand3.ExecuteNonQuery();
                                }
                                else
                                {
                                    //else insert
                                    SqlCommand myCommand4 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToString(dgvCustomerReturn[0, i].Value) + "','" + dgvCustomerReturn[1, i].Value.ToString() + "','" + Convert.ToDouble(dgvCustomerReturn[3, i].Value) + "','" + Convert.ToString(dgvCustomerReturn[4, i].Value) + "','" + CreditDate + "')", myConnection, myTrans);
                                    myCommand4.ExecuteNonQuery();
                                }                                

                                if (Convert.ToDouble(dgvCustomerReturn[3, i].Value) != 0)
                                {
                                    SqlCommand myCommand123 = new SqlCommand(
                                        "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbWarehouse.Text.ToString().Trim() + "' AND ItemId='" + dgvCustomerReturn[0, i].Value + "') " +
                                        " INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(@OHQTY,5,'" +
                                        txtCreditNoteNo.Text.ToString().Trim() + "','" + CreditDate + "','CreditNote','true','" + 
                                        dgvCustomerReturn[0, i].Value.ToString().Trim() + "'," + 
                                        Convert.ToDouble(dgvCustomerReturn[3, i].Value) + "," + 
                                        Convert.ToDouble(dgvCustomerReturn[5, i].Value) + "," + 
                                        Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colAmount"].Value) + ",'" +
                                        cmbWarehouse.Value.ToString().Trim() + "', '" + 
                                        Convert.ToDouble(dgvCustomerReturn[3, i].Value) + "')", myConnection, myTrans);
                                    myCommand123.ExecuteNonQuery();
                                }                   
                              
                                //string TranType = "GRN";
                                bool IsGRNProcess = true;
                                string Status = "Available";                               

                                //update he customer table==================================
                                SqlCommand myCommand7 = new SqlCommand("update tblSalesInvoices set RemainQty = RemainQty- '" + Convert.ToDouble(dgvCustomerReturn[3, i].Value) + "'  where InvoiceNo =  '" + SelectInvNO1 + "' and ItemID='" + dgvCustomerReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                                myCommand7.ExecuteNonQuery();                              
                            }
                        }
                        //========================================================================
                        int rows = GetFilledRows1();
                        for (int i = 0; i < rows; i++)
                        {
                            SqlCommand myCommand6 = new SqlCommand("select sum(ReturnQty) as ReturnQty from tblCutomerReturn where InvoiceNO =  '" + SelectInvNO1 + "' and ItemID='" + dgvCustomerReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            SqlDataAdapter daz = new SqlDataAdapter(myCommand6);
                            DataTable dtz = new DataTable();
                            daz.Fill(dtz);

                            SqlCommand myCommand7 = new SqlCommand("select Qty from tblSalesInvoices where InvoiceNo =  '" + SelectInvNO1 + "' and ItemID='" + dgvCustomerReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            SqlDataAdapter da2 = new SqlDataAdapter(myCommand7);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);

                            if (dtz.Rows.Count > 0)
                            {
                                if (dtz.Rows[0].ItemArray[0].ToString().Trim() != "")
                                {
                                    if (dt2.Rows.Count > 0)
                                    {
                                        if (dt2.Rows[0].ItemArray[0].ToString().Trim() != "")
                                        {
                                            if (Convert.ToDouble(dtz.Rows[0].ItemArray[0].ToString()) == Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString()))
                                            {
                                                bool isreturn = true;
                                                SqlCommand myCommand4 = new SqlCommand("update tblSalesInvoices set IsReturn = '" + isreturn + "'  where InvoiceNo =  '" + SelectInvNO1 + "' and ItemID='" + dgvCustomerReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                                                myCommand4.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        {
                            SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                                " TranType='CreditNote',Status='Available' " +
                                " where ItemID='" +
                                dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                                dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                            myCommandSe1.ExecuteNonQuery();
                        }

                        if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                        {
                            frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                            objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "CreditNote", cmbWarehouse.Text.ToString(), txtCreditNoteNo.Text.ToString().Trim(), dtpCreditDate.Value ,false, string.Empty);
                        }

                        CreatePurchaseJXML(myTrans, myConnection);
                        Connector conn = new Connector();
                        if (conn.ImportCustomerReturnAR() == 0)
                        {
                            myTrans.Rollback();
                            return;
                        }                       

                        myTrans.Commit();
                        //  myCommand.CommandText = "UPDATE tblDefualtSetting SET CustomerReturnNo = CustomerReturnNo + 1";// select CustomerReturnNo, CusReturnPrefix from tblDefualtSetting with (rowlock)";
                        MessageBox.Show("Customer Return Successfully saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnReprint_Click(sender, e);
                        btnNew_Click(sender, e);

                        Disable();
                        btnSave.Enabled = false;
                        btnNew.Enabled = true;
                        // btnNew_Click(sender, e);
                    }
                    
                    catch (Exception ex)
                    {
                        objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                        myTrans.Rollback();
                    }
                }
            }
            else
            {
                //user has enter invalid input to numober field
                MessageBox.Show("Numeric Convertion Error");
            }
            btnNew_Click(sender, e);
        }

        public int GetFilledRows1()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    if (dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value != null) //change cell value by 1                   
                    {
                        RowCount++;
                    }
                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Create a XML File for import Purchase Journal=====================
        public void CreatePurchaseJXML(SqlTransaction tr, SqlConnection con)
        {
            try
            {
                double _Doscount = 0;
                double _Amount = 0;
                _Doscount = double.Parse(txtDiscAmount.Value.ToString());

                DateTime DTP = Convert.ToDateTime(dtpCreditDate.Text);
                string Dformat = "MM/dd/yyyy";
                string GRNDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CustomerReturn.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Invoices");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                int DistributionNumber = 0;

                int rowCount1 = GetFilledRows_new();
                int rowCount2 = GetFilledRowstax();
                int ActRow = GetFilledRowsRetQtyAvailble();

                //discount
                //if (double.Parse(txtDiscAmount.Value.ToString().Trim()) + double.Parse(txtDiscLineTot.Text.Trim()) > 0)
                //   rowCount2 = rowCount2 + 1;

                if (double.Parse(txtServCharges.Text.ToString().Trim()) > 0)
                    rowCount2 = rowCount2 + 1;

                // int rowCount1 = GetFilledRowsForXMLExport();
                //  string NoDistributions = Convert.ToString(rowCount1 + rowCount2);
                string NoDistributions = Convert.ToString(ActRow + rowCount2);



                if (txtDiscAmount.Text.Trim() != string.Empty && double.Parse(txtDiscAmount.Text.Trim()) > 0)
                {
                    NoDistributions = (Convert.ToInt32(NoDistributions) + 1).ToString();
                }



                double TotalLineDiscount = 0;

                for (int i = 0; i < dgvCustomerReturn.Rows.Count; i++)
                {
                    try
                    {
                        TotalLineDiscount = TotalLineDiscount + (double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString())* double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()))- double.Parse(dgvCustomerReturn.Rows[i].Cells["colAmount"].Value.ToString());
                    }
                    catch (Exception ex)
                    {

                    }

                }

                //if (TotalLineDiscount > 0)
                //{
                //    NoDistributions = (Convert.ToInt32(NoDistributions) + 1).ToString();
                //}

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:invoice");

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtCreditNoteNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(dtpCreditDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbSalesRepID.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteString(StrARAccount);//Cash Account
                Writer.WriteEndElement();//CreditMemoType

                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("TRUE");
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoDistributions);
                Writer.WriteEndElement();

                Writer.WriteStartElement("SalesLines");//Sales lies

                for (int i = 0; i < rowCount1; i++)
                {
                    if (i < rowCount1)
                    {
                        if (Convert.ToDouble(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()) > 0)
                        {
                            if (IsThisItemSerial(dgvCustomerReturn.Rows[i].Cells[0].Value.ToString()))
                            {
                                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                                {
                                    if (dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString() == dr["ItemCode"].ToString())
                                    {
                                        double _GRAM = 0;
                                        double dblcolUnitPriceExport = 0;
                                        double Disam = 0.00;
                                        double vatper = double.Parse(txtVatPer.Value.ToString());
                                        double Taxam = 0.00;
                                        if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                                        {
                                            _Amount = 0;
                                            _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                            if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                                _Amount = (_Amount / (vatper + 100)) * 100;
                                            }


                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                                _Amount = (_Amount / (vatper + 100)) * 100;
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                Taxam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineTax"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                                Disam = (double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                                                _Amount = (double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()) - Taxam) - Disam;
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineTax"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                //Discount 1
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                                _Amount = (_Amount / (vatper + 100)) * 100;
                                                //Discount 2
                                                _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                                            }
                                            dblcolUnitPriceExport = _Amount;
                                        }
                                        if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                                        {
                                            _Amount = 0;
                                            _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                            if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                                _Amount = (_Amount / (vatper + 100)) * 100;
                                            }

                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                Disam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                                _Amount = _Amount - Disam;
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                                _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                            }
                                            dblcolUnitPriceExport = _Amount;
                                        }
                                        else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                                        {
                                            _Amount = 0;
                                            _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                            if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                                _Amount = (_Amount / (vatper + 100)) * 100;
                                            }

                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                Disam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                                _Amount = _Amount - Disam;
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                            {
                                                _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                            }
                                            else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                            {
                                                _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                                _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                            }
                                            dblcolUnitPriceExport = _Amount;
                                        }


                                        dblcolUnitPriceExport = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString());

                                        Writer.WriteStartElement("SalesLine");

                                        Writer.WriteStartElement("Quantity");
                                        //Writer.WriteString("1" );
                                        Writer.WriteString("-" + dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Item_ID");
                                        Writer.WriteString(dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Description");
                                        Writer.WriteString(dgvCustomerReturn.Rows[i].Cells["colDesc"].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("GL_Account");
                                        Writer.WriteString(StrSalesGLAccount);
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Job_ID");
                                        Writer.WriteString(cmbjob.Text.ToString().Trim());
                                        Writer.WriteEndElement();

                                        dblcolUnitPriceExport = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                        Writer.WriteStartElement("Unit_Price");
                                        Writer.WriteString(dblcolUnitPriceExport.ToString());
                                        Writer.WriteEndElement();

                                        _GRAM = dblcolUnitPriceExport * double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());

                                        _GRAM = double.Parse(dgvCustomerReturn.Rows[i].Cells["colAmount"].Value.ToString());
                                        Writer.WriteStartElement("Amount");
                                        Writer.WriteString(_GRAM.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Tax_Type");
                                        Writer.WriteString("1");
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Serial_Number");
                                        Writer.WriteString(dr["SerialNo"].ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("AppliedToSO");
                                        Writer.WriteString("FALSE");
                                        Writer.WriteEndElement();
                                    }
                                }
                            }
                            else
                            {
                                double _GRAM = 0;
                                double dblcolUnitPriceExport = 0;
                                double Disam = 0.00;
                                double vatper = double.Parse(txtVatPer.Value.ToString());
                                double Taxam = 0.00;
                                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                    if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                    }

                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        Taxam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineTax"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                        Disam = (double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()) - Taxam) * double.Parse(txtDiscPer.Text.ToString()) / 100;
                                        _Amount = (double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()) - Taxam) - Disam;
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineTax"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        //Discount 1
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                        //Discount 2
                                        _Amount = _Amount - (_Amount * double.Parse(txtDiscPer.Text.ToString()) / 100);

                                    }
                                    dblcolUnitPriceExport = _Amount;
                                }
                                if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                    if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                    }

                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        Disam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                        _Amount = _Amount - Disam;
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    dblcolUnitPriceExport = _Amount;
                                }
                                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                                {
                                    _Amount = 0;
                                    _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                    if (dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString() == "40+4")
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()));
                                        _Amount = (_Amount / (vatper + 100)) * 100;
                                    }

                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        Disam = double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                        _Amount = _Amount - Disam;
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && double.Parse(txtDiscAmount.Value.ToString()) <= 0)
                                    {
                                        _Amount = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                    }
                                    else if (double.Parse(dgvCustomerReturn.Rows[i].Cells["Discount"].Value.ToString()) > 0 && double.Parse(txtDiscAmount.Value.ToString()) > 0)
                                    {
                                        _Amount = _Amount - (double.Parse(dgvCustomerReturn.Rows[i].Cells["colLineDisc"].Value.ToString()) / double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()));
                                        _Amount = _Amount - ((_Amount * double.Parse(txtDiscPer.Value.ToString())) / 100);
                                    }
                                    dblcolUnitPriceExport = _Amount;
                                }
                                dblcolUnitPriceExport = (double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString()) * double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString()));

                                Writer.WriteStartElement("SalesLine");

                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString("-" + dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteString(dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(dgvCustomerReturn.Rows[i].Cells["colDesc"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteString(dgvCustomerReturn.Rows[i].Cells["colGL"].Value.ToString()); //StrSalesGLAccount
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Job_ID");
                                Writer.WriteString(cmbjob.Text.ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");
                                Writer.WriteEndElement();

                                dblcolUnitPriceExport = double.Parse(dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                _GRAM = dblcolUnitPriceExport * double.Parse(dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value.ToString());

                                _GRAM = double.Parse(dgvCustomerReturn.Rows[i].Cells["colAmount"].Value.ToString());
                                Writer.WriteStartElement("Amount");
                                Writer.WriteString(_GRAM.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();
                            }
                        }
                    }
                }
                Writer.WriteStartElement("AppliedToSO");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();
                if (double.Parse(txtNBT.Value.ToString().Trim()) > 0)
                {

                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("-1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(NBitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(NBTitemDis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(NBTitemGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(txtNBT.Text.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("AppliedToSO");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                if (double.Parse(txtVat.Value.ToString().Trim()) > 0)
                {

                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("-1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(VATitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(VATitemDis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(VATGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(txtVat.Text.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("AppliedToSO");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                //if (double.Parse(TotalLineDiscount.ToString().Trim()) > 0)
                //{
                //    Writer.WriteStartElement("SalesLine");
                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("-1");
                //    Writer.WriteEndElement();


                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString(LineDisitemid);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString(LineDisitemdescription);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    Writer.WriteString(LineDisGLAccount);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Job_ID");
                //    Writer.WriteString(cmbjob.Text.ToString().Trim());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    Writer.WriteString(TotalLineDiscount.ToString());
                //    Writer.WriteEndElement();



                //    Writer.WriteEndElement();
                //}

                if (double.Parse(txtDiscAmount.Text.ToString().Trim()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("-1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(Cashitemid);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(cashitemdis);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cashGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-"+txtDiscAmount.Text.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("AppliedToSO");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                if (double.Parse(txtServCharges.Text.ToString().Trim()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("-1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(SERID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(SERDIS);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(SERGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(txtServCharges.Text.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("AppliedToSO");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                //if ((TotalLineDiscount) > 0)
                //{
                //    Writer.WriteStartElement("SalesLine");

                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("-1");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString(LineDisitemid);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString(LineDisitemdescription);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(LineDisGLAccount);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    Writer.WriteString("-"+TotalLineDiscount.ToString());
                //    Writer.WriteEndElement();
                

                //    Writer.WriteStartElement("AppliedToSO");
                //    Writer.WriteString("FALSE");
                //    Writer.WriteEndElement();

                //    Writer.WriteEndElement();
                //}

                // Writer.WriteEndElement();//LINEs

                //Writer.WriteStartElement("AppliedToSO");
                //Writer.WriteString("FALSE");
                //Writer.WriteEndElement();

                //********************
                Writer.WriteEndElement();//last line

                Writer.WriteEndElement();//last line

                Writer.WriteEndElement();//last line
                Writer.Close();
            }
            catch(Exception ex)
            {

            }
        }
        /******************************************/

        private void dgvCustomerReturn_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //    DataGridViewColumn col = e.Column;

            //    if (col.Index == 0)
            //    {
            //        mltcmbboxItemSelect.Width = col.Width;
            //    }
        }
        int newVal1 = 0;
        private void dgvCustomerReturn_Scroll(object sender, ScrollEventArgs e)
        {
            //newVal1 = e.NewValue;

            //this.mltcmbboxItemSelect.Location = new System.Drawing.Point(defaultLocation1.X, defaultLocation1.Y);
            //mltcmbboxItemSelect.Visible = false;
        }

        private void dgvCustomerReturn_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //mltcmbboxItemSelect.Visible = true;
        }
        private void SetDefaultWarehouse()
        {
            try
            {
                if (dsWarehouse.Tables[0].Rows.Count > 0)
                {
                    cmbWarehouse.Value = cmbWarehouse.GetRow(ChildRow.First).Cells["WhseId"].Value;
                    txtWarehouseName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();
                    StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                    StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                    StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {

                rdoNonVat.Checked = true;
                flag = false;
                cmbSalesRepID.Text = "";
                txtCreditNoteNo.Text = "";
                // Enable();
                ClearValues();
                formload = false;
                btnSave.Enabled = true;
                Enable();
                SetDefaultWarehouse();
                loadDefaltOption();
                if (user.IsCRTNNoAutoGen) txtCreditNoteNo.ReadOnly = true;
                else txtCreditNoteNo.ReadOnly = false;

                txtNBTPer.Text = "0.00";
                txtVatPer.Text = "0.00";

                clsSerializeItem.DtsSerialNoList.Rows.Clear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void loadDefaltOption()
        {
            try
            {
                //StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PAY' and UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(StrSql);
                //SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        if (bool.Parse(dt.Rows[0].ItemArray[2].ToString()) == true)
                //        {
                //            grpamounttypes.Enabled = false;
                //        }
                //        if (dt.Rows[0]["TAXID"].ToString() == "Cash")
                //        {
                //            optCash.Checked = true;
                //        }
                //        else if (dt.Rows[0]["TAXID"].ToString() == "Credit")
                //        {
                //            optCredit.Checked = true;
                //        }
                //        else if (dt.Rows[0]["TAXID"].ToString() == "Other")
                //        {
                //            rdobtnCreditCard.Checked = true;
                //        }
                //    }
                //}

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbInvoiceType.Value = dt1.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbInvoiceType.Enabled = false;
                        }
                    }
                }
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='REP' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    cmbSalesRepID.Enabled = true;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        cmbSalesRepID.Text  = dt2.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt2.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbSalesRepID.Enabled = false;
                        }
                    }
                }

                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='WEH' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(StrSql);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt2.Rows.Count > 0)
                {
                    cmbWarehouse.Enabled = true;
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        cmbWarehouse.Value = dt3.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt3.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbWarehouse.Enabled = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void ClearValues()
        {
            cmbCustomer.Text = "";

            txtcusName.Text = "";
            txtCusAdd1.Text = "";
            txtCusAdd2.Text = "";
            dgvCustomerReturn.Rows.Clear();
            txtTotalAmount.Text = "0";
            txtNetTotal.Text = "0";
            clistbxInvoices.Items.Clear();
            cmbInvoiceType.Value = 1;
            txtDiscPer.Value = "0.00";
            txtServCharges.Text = "0.00";
        }

        public bool ISSAVE = false;//if save this variable ge true
        bool formload = false;
        // bool formload = true;
        int currentRowIndex1;
        int k = 0;

        private void mltcmbboxItemSelect_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (formload == false)
            //    {
            //        k++;
            //        try
            //        {
            //            if (ISSAVE != true)
            //            {
            //                if (k >= 1)
            //                {
            //                    if (currentRowIndex1 - 1 != -1)
            //                    {
            //                        if (dgvCustomerReturn[0, currentRowIndex1 - 1].Value != null)
            //                        {
            //                            dgvCustomerReturn.Rows.Add();
            //                            dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                            dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

            //                            String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                            SqlCommand cmd = new SqlCommand(S);
            //                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                            DataSet dt = new DataSet();
            //                            da.Fill(dt);

            //                            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                            {
            //                                dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                                dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                                dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                            }

            //                            mltcmbboxItemSelect.Visible = false;
            //                            if (k > 4)
            //                            {
            //                                dgvCustomerReturn.Rows.Add();
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        dgvCustomerReturn.Rows.Add();
            //                        dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                        dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

            //                        String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID ='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                        SqlCommand cmd = new SqlCommand(S);
            //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                        DataSet dt = new DataSet();
            //                        da.Fill(dt);
            //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                        {
            //                            dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                            dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                            dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                        }
            //                        mltcmbboxItemSelect.Visible = false;
            //                        if (k > 5)// old value is 4 change in to  as 5
            //                        {
            //                            dgvCustomerReturn.Rows.Add();
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            { }
            //            dgvCustomerReturn.Focus();

            //        }
            //        catch
            //        { }
            //    }
            //    //}
            //    //if (e.KeyCode == Keys.Enter)
            //    //{
            //    //    if (formload == false)
            //    //    {
            //    //        k++;
            //    //        try
            //    //        {
            //    //            if (ISSAVE != true)
            //    //            {
            //    //                if (k >= 1)
            //    //                {
            //    //                    if (currentRowIndex1 - 1 != -1)
            //    //                    {
            //    //                        if (dgvCustomerReturn[0, currentRowIndex1 - 1].Value != null)
            //    //                        {
            //    //                            dgvCustomerReturn.Rows.Add();
            //    //                            dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //    //                            dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();


            //    //                            String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //    //                            SqlCommand cmd = new SqlCommand(S);
            //    //                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //    //                            DataSet dt = new DataSet();
            //    //                            da.Fill(dt);
            //    //                            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //    //                            {
            //    //                                dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    //                                dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //    //                                dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //    //                            }
            //    //                            mltcmbboxItemSelect.Visible = false;
            //    //                            if (k > 4)
            //    //                            {
            //    //                                dgvCustomerReturn.Rows.Add();
            //    //                            }
            //    //                        }
            //    //                    }
            //    //                    else
            //    //                    {
            //    //                        dgvCustomerReturn.Rows.Add();
            //    //                        dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //    //                        dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

            //    //                        String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID ='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //    //                        SqlCommand cmd = new SqlCommand(S);
            //    //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //    //                        DataSet dt = new DataSet();
            //    //                        da.Fill(dt);
            //    //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //    //                        {
            //    //                            dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    //                            dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //    //                            dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //    //                        }
            //    //                        mltcmbboxItemSelect.Visible = false;
            //    //                        if (k > 5)// old value is 4 change in to  as 5
            //    //                        {
            //    //                            dgvCustomerReturn.Rows.Add();
            //    //                        }
            //    //                    }
            //    //                }
            //    //            }
            //    //            else
            //    //            { }
            //    //            dgvCustomerReturn.Focus();

            //    //        }
            //    //        catch
            //    //        { }
            //    //    }
            //    //}
            //}
        }



        //public void GetItemDetails()//this mehod get items details to fill tyhe datagrid
        //{
        //    String S = "Select * from tblCustomerMaster";
        //    SqlCommand cmd = new SqlCommand(S);
        //    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //    DataSet dt = new DataSet();
        //    da.Fill(dt);

        //    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //    {
        //        cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
        //    }
        //}

        private void mltcmbboxItemSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (formload == false)
            //{
            //    // string aa = dataGridView4[0, currentRowIndex1 - 1].Value.ToString();
            //    k++;
            //    try
            //    {
            //        if (ISSAVE != true)
            //        {
            //            if (k >= 1)
            //            {
            //                if (currentRowIndex1 - 1 != -1)
            //                {
            //                    if (dgvCustomerReturn[0, currentRowIndex1 - 1].Value != null)
            //                    {

            //                        //if (currentRowIndex1 == dgvCustomerReturn.SelectedRows.Count - 1)
            //                        //{
            //                        //    //dgvCustomerReturn.Rows.Add();
            //                        //}
            //                        //else
            //                        //{
            //                        //     dgvCustomerReturn.Rows.Add();
            //                        //}



            //                        dgvCustomerReturn.Rows.Add();
            //                        // mlt_P_Items.Visible = false;
            //                        // string aa = dataGridView4[0, currentRowIndex1 - 1].Value.ToString();
            //                        dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                        dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();


            //                        String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                        SqlCommand cmd = new SqlCommand(S);
            //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                        DataSet dt = new DataSet();
            //                        da.Fill(dt);
            //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                        {
            //                            dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                            dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                            dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                            //cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            //                        }

            //                        //acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                        //dataGridView4[2, currentRowIndex1].Value = acc[1].ToString().Trim();//enter byu sanjeewa
            //                        //mlt_P_Items.Visible = false;
            //                        //try
            //                        //{
            //                        //    acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                        //    dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();// change by sanjeewa
            //                        //    ArrayList stockdata = new ArrayList();
            //                        //    stockdata = fil.ReturnStockDetails(mlt_P_Items.Text.Trim());
            //                        //    double UPrice = Convert.ToDouble(stockdata[0].ToString().Trim());
            //                        //    dataGridView4[5, currentRowIndex1].Value = UPrice.ToString("N2");// change cell  value by1
            //                        //}
            //                        //catch
            //                        //{ }

            //                        // 
            //                        mltcmbboxItemSelect.Visible = false;
            //                        if (k > 4)
            //                        {
            //                            dgvCustomerReturn.Rows.Add();
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // A = 1;
            //                   dgvCustomerReturn.Rows.Add();

            //                   //if (currentRowIndex1 == dgvCustomerReturn.SelectedRows.Count - 1)
            //                   //{
            //                   //    //dgvCustomerReturn.Rows.Add();
            //                   //}
            //                   //else
            //                   //{
            //                   //   dgvCustomerReturn.Rows.Add();
            //                   //}

            //                    dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                    // dataGridView4[0, currentRowIndex1].Value = mlt_P_Items.Text.Trim();
            //                    dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

            //                    String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID ='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                    SqlCommand cmd = new SqlCommand(S);
            //                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                    DataSet dt = new DataSet();
            //                    da.Fill(dt);
            //                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                    {
            //                        dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                        dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                        dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                        //cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            //                    }
            //                    //acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                    //dataGridView4[2, currentRowIndex1].Value = acc[1].ToString().Trim(); // add by sanjeewa
            //                    //mlt_P_Items.Visible = false;
            //                    //try
            //                    //{
            //                    //    acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                    //    // dataGridView4[2, currentRowIndex1].Value = mlt_P_Items.SelectedValue.ToString().Trim();
            //                    //    dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();// comment by sanjeewa
            //                    //    // dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();
            //                    //    ArrayList stockdata = new ArrayList();
            //                    //    stockdata = fil.ReturnStockDetails(mlt_P_Items.Text.Trim());
            //                    //    double UPrice = Convert.ToDouble(stockdata[0].ToString().Trim());

            //                    //    dataGridView4[5, currentRowIndex1].Value = UPrice.ToString("N2");

            //                    //}
            //                    //catch
            //                    //{ }

            //                    mltcmbboxItemSelect.Visible = false;
            //                    if (k > 5)// old value is 4 change in to  as 5
            //                    {
            //                        dgvCustomerReturn.Rows.Add();
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        { }
            //        dgvCustomerReturn.Focus();

            //    }
            //    catch
            //    { }
            //}

            //if (formload == false)
            //{
            //    // string aa = dataGridView4[0, currentRowIndex1 - 1].Value.ToString();
            //    k++;
            //    try
            //    {
            //        if (ISSAVE != true)
            //        {
            //            if (k >= 1)
            //            {
            //                if (currentRowIndex1 - 1 != -1)
            //                {
            //                    if (dgvCustomerReturn[0, currentRowIndex1 - 1].Value != null)
            //                    {
            //                        dgvCustomerReturn.Rows.Add();
            //                        // mlt_P_Items.Visible = false;
            //                        // string aa = dataGridView4[0, currentRowIndex1 - 1].Value.ToString();
            //                        dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                        dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();


            //                        String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                        SqlCommand cmd = new SqlCommand(S);
            //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                        DataSet dt = new DataSet();
            //                        da.Fill(dt);
            //                        for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                        {
            //                            dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                            dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                            dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                            //cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            //                        }

            //                        //acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                        //dataGridView4[2, currentRowIndex1].Value = acc[1].ToString().Trim();//enter byu sanjeewa
            //                        //mlt_P_Items.Visible = false;
            //                        //try
            //                        //{
            //                        //    acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                        //    dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();// change by sanjeewa
            //                        //    ArrayList stockdata = new ArrayList();
            //                        //    stockdata = fil.ReturnStockDetails(mlt_P_Items.Text.Trim());
            //                        //    double UPrice = Convert.ToDouble(stockdata[0].ToString().Trim());
            //                        //    dataGridView4[5, currentRowIndex1].Value = UPrice.ToString("N2");// change cell  value by1
            //                        //}
            //                        //catch
            //                        //{ }

            //                        // 
            //                        mltcmbboxItemSelect.Visible = false;
            //                        if (k > 4)
            //                        {
            //                            dgvCustomerReturn.Rows.Add();
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // A = 1;
            //                    dgvCustomerReturn.Rows.Add();
            //                    dgvCustomerReturn[0, currentRowIndex1].Value = mltcmbboxItemSelect.Text.Trim();
            //                    // dataGridView4[0, currentRowIndex1].Value = mlt_P_Items.Text.Trim();
            //                    dgvCustomerReturn[3, currentRowIndex1].Value = mltcmbboxItemSelect.SelectedValue.ToString().Trim();

            //                    String S = "Select UOM,UnitPrice,SalesGLAccount from tblItemMaster where ItemID ='" + mltcmbboxItemSelect.Text.Trim() + "'";
            //                    SqlCommand cmd = new SqlCommand(S);
            //                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //                    DataSet dt = new DataSet();
            //                    da.Fill(dt);
            //                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //                    {
            //                        dgvCustomerReturn[4, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //                        dgvCustomerReturn[5, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //                        dgvCustomerReturn[8, currentRowIndex1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
            //                        //cmbCustomer.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            //                    }
            //                    //acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                    //dataGridView4[2, currentRowIndex1].Value = acc[1].ToString().Trim(); // add by sanjeewa
            //                    //mlt_P_Items.Visible = false;
            //                    //try
            //                    //{
            //                    //    acc = lqty.LoadInventory_COSAcc(mlt_P_Items.Text.Trim());
            //                    //    // dataGridView4[2, currentRowIndex1].Value = mlt_P_Items.SelectedValue.ToString().Trim();
            //                    //    dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();// comment by sanjeewa
            //                    //    // dataGridView4[3, currentRowIndex1].Value = acc[0].ToString().Trim();
            //                    //    ArrayList stockdata = new ArrayList();
            //                    //    stockdata = fil.ReturnStockDetails(mlt_P_Items.Text.Trim());
            //                    //    double UPrice = Convert.ToDouble(stockdata[0].ToString().Trim());

            //                    //    dataGridView4[5, currentRowIndex1].Value = UPrice.ToString("N2");

            //                    //}
            //                    //catch
            //                    //{ }

            //                    mltcmbboxItemSelect.Visible = false;
            //                    if (k > 5)// old value is 4 change in to  as 5
            //                    {
            //                        dgvCustomerReturn.Rows.Add();
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        { }
            //        dgvCustomerReturn.Focus();

            //    }
            //    catch
            //    { }
            //}
        }


        public ArrayList ArryInvoiceNo = new ArrayList();//Sales orders related to a custmer
        public string SelectInvNO1 = "";
        private void clistbxInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //string  SelectSONums1 = "";
                // string SelectSONums = "";

                SelectInvNO1 = "";
                string SelectInvNO = "";
                dgvCustomerReturn.Rows.Clear();
                int step = 0;
                int i = 0;
                ArryInvoiceNo.Clear();
                //  ArraySONO.Clear();

                while (i < clistbxInvoices.Items.Count)
                {
                    if (clistbxInvoices.GetItemChecked(i) == true)
                    {
                        step++;
                        string[] SOIDs = new string[2];
                        SOIDs = clistbxInvoices.Items[i].ToString().Split('*');
                        string So_No = SOIDs[0].ToString();

                        string So_No1 = SOIDs[0].ToString();//saving code
                        ArryInvoiceNo.Add(So_No);
                        // ArraySONO.Add(So_No);
                        So_No = "'" + So_No + "'";

                        // So_No = So_No;
                        SelectInvNO = SelectInvNO + So_No;

                        So_No1 = So_No1 + " ";//savins purpose
                        SelectInvNO1 = SelectInvNO1 + So_No1;//saving purpose

                    }
                    i++;
                }

                if (SelectInvNO.Length != 0)
                {
                    DataSet ds = new DataSet();
                    ds = ReturnInvNoList(SelectInvNO);
                    // ds = ReturnSOList(SelectInvNO);

                    string CusPO = ReturnCusPO(SelectInvNO);
                    txtCustomerPO.Text = CusPO;

                    //===========================jobjob

                    string Job = ReturnJob(SelectInvNO);
                    cmbSalesRepID.Text = Job;
                    // txtCustomerPO.Text = job;



                    string Job1 = ReturnJob1(SelectInvNO);
                    cmbjob.Text = Job1;
                    // txtCustomerPO.Text = job;

                    //==================================



                    for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                    {

                        dgvCustomerReturn.Rows.Add();
                    }

                    double AmountWD = 0.0;

                    cmbWarehouse.Value = ds.Tables[0].Rows[0]["Location"].ToString();
                    //String S = "Select ItemID,Sum(RemainQty),Description,GLAcount,UnitPrice,UOM, sum(Amount),Location,IsInclusive,InvType,ServiceCharge,Discount,TotalDiscountPercen,Tax1Rate,Tax2Rate from tblSalesInvoices where InvoiceNo in ('" + GRNNo + "') group by   ItemID,Description,GLAcount,UnitPrice,UOM,Location,IsInclusive,InvType,ServiceCharge,Discount,TotalDiscountPercen,Tax1Rate,Tax2Rate  having Sum(RemainQty)>0";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        if (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]) > 0)
                        {
                            dgvCustomerReturn.Rows[j].Cells["colItemCode"].Value = ds.Tables[0].Rows[j]["ItemId"].ToString().Trim();// Item ID ds.Tables[0].Rows[j].ItemArray["ItemID"].ToString(); //ds.Tables[0].Rows[j].ItemArray["ItemID"].ToString().Trim();//itemid
                            dgvCustomerReturn.Rows[j].Cells["colDesc"].Value = ds.Tables[0].Rows[j]["Description"].ToString().Trim();//Quantity

                            if (DecimalpointQuantity == 0)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString();//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0";
                                //dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString();//Description
                            }
                            if (DecimalpointQuantity == 1)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString("N1");//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0.0";
                                // dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N1");//Description
                            }
                            if (DecimalpointQuantity == 2)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString("N2");//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0.00";
                                // dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N2");//Description
                            }
                            if (DecimalpointQuantity == 3)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString("N3");//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0.000";
                                // dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N3");//Description
                            }
                            if (DecimalpointQuantity == 4)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString("N4");//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0.0000";
                                // dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N4");//Description
                            }
                            if (DecimalpointQuantity == 5)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"]).ToString("N5");//Description
                                dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = "0.00000";
                                //dgvCustomerReturn.Rows[j].Cells["colRetQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N5");//Description
                            }
                            dgvCustomerReturn.Rows[j].Cells["UOM"].Value = ds.Tables[0].Rows[j]["UOM"].ToString().Trim();//UnitPrice
                            if (Decimalpoint == 0)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = ds.Tables[0].Rows[j]["UnitPrice"].ToString().Trim();
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = ds.Tables[0].Rows[j]["InclusivePrice"].ToString().Trim();
                                dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.00");
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0";
                                //dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = ds.Tables[0].Rows[j].ItemArray[6].ToString().Trim();
                            }
                            if (Decimalpoint == 1)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"]).ToString("N1");
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["InclusivePrice"]).ToString("N1");
                                dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.00");
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0.0";
                                // dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N1");
                            }
                            if (Decimalpoint == 2)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"]).ToString("N2");
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["InclusivePrice"]).ToString("N2");

                                if(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()=="40+4")
                                {
                                    dgvCustomerReturn.Rows[j].Cells["Discount"].Value = "40+4";
                                }
                                else
                                {
                                    dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.00");

                                }
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0.00";
                                // dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N2");
                            }
                            if (Decimalpoint == 3)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"]).ToString("N3");
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["InclusivePrice"]).ToString("N3");
                                dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.000");
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0.000";                                
                            }
                            if (Decimalpoint == 4)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"]).ToString("N4");
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["InclusivePrice"]).ToString("N4");
                                dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.0000");
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0.0000";
                            }
                            if (Decimalpoint == 5)
                            {
                                dgvCustomerReturn.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"]).ToString("N5");
                                dgvCustomerReturn.Rows[j].Cells["colUnitPriceIncl"].Value = Convert.ToDouble(ds.Tables[0].Rows[j]["InclusivePrice"]).ToString("N5");
                                dgvCustomerReturn.Rows[j].Cells["Discount"].Value = double.Parse(ds.Tables[0].Rows[j]["LineDiscountPercentage"].ToString()).ToString("0.00000");
                                dgvCustomerReturn.Rows[j].Cells["colAmount"].Value = "0.00000";
                            }
                            dgvCustomerReturn.Rows[j].Cells["colGL"].Value = ds.Tables[0].Rows[j]["GLAcount"].ToString().Trim();
                            AmountWD = AmountWD + Convert.ToDouble(dgvCustomerReturn.Rows[j].Cells["colAmount"].Value);
                        }
                    }

                    if (ds.Tables[0].Rows[0]["IsInclusive"].ToString() == "1") cmbInvoiceType.Value = 1;
                    else if (ds.Tables[0].Rows[0]["IsInclusive"].ToString() == "3") cmbInvoiceType.Value = 3;
                    else if (ds.Tables[0].Rows[0]["IsInclusive"].ToString() == "2") cmbInvoiceType.Value = 2;

                    if (int.Parse(ds.Tables[0].Rows[0]["InvType"].ToString()) == 1) rdoNonVat.Checked = true;
                    if (int.Parse(ds.Tables[0].Rows[0]["InvType"].ToString()) == 2) rdoTax.Checked = true;
                    if (int.Parse(ds.Tables[0].Rows[0]["InvType"].ToString()) == 3) rdoSVat.Checked = true;

                    txtDiscPer.Value = double.Parse(ds.Tables[0].Rows[0]["TotalDiscountPercen"].ToString()).ToString("0.00");
                    
                    txtNBTPer.Value = double.Parse(ds.Tables[0].Rows[0]["Tax1Rate"].ToString()).ToString("0.00");
                    txtVatPer.Value = double.Parse(ds.Tables[0].Rows[0]["Tax2Rate"].ToString()).ToString("0.00");
                    

                    //if (Decimalpoint == 0)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString();
                    //    txtNetTotal.Text = AmountWD.ToString();
                    //}
                    //if (Decimalpoint == 1)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString("N1");
                    //    txtNetTotal.Text = AmountWD.ToString("N1");
                    //}
                    //if (Decimalpoint == 2)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString("N2");
                    //    txtNetTotal.Text = AmountWD.ToString("N2");
                    //}
                    //if (Decimalpoint == 3)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString("N3");
                    //    txtNetTotal.Text = AmountWD.ToString("N3");
                    //}
                    //if (Decimalpoint == 4)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString("N4");
                    //    txtNetTotal.Text = AmountWD.ToString("N4");
                    //}
                    //if (Decimalpoint == 5)
                    //{
                    //    txtTotalAmount.Text = AmountWD.ToString("N5");
                    //    txtNetTotal.Text = AmountWD.ToString("N5");
                    //}
                    cmbInvoiceType_ValueChanged(sender, e);
                    txtServCharges.Text = double.Parse(ds.Tables[0].Rows[0]["ServiceCharge"].ToString()).ToString("0.00");
                    txtDiscPer.Text = double.Parse(ds.Tables[0].Rows[0]["TotalDiscountPercen"].ToString()).ToString("0.00"); 
                }

                
                //===========================
                // dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells["colRetQty"];
                //CalculateGridAmounts();
                //TaxCalculation();
                //=============================

            //    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
            //    FooterCalculation();

                dgvCustomerReturn.Columns[0].ReadOnly = true;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        //GetCustomerPO=================================

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
                String S1 = "Select CustomerPO from tblSalesInvoices where InvoiceNo in ('" + GRNNo + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        CusPO = dt1.Rows[i].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }
        //==========Return a job ==================================
        public string ReturnJob(string SO_No)
        {
            string Job = "";
            //jobid is change as salesRepID
            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
                String S1 = "Select SalesRep from tblSalesInvoices where InvoiceNo in ('" + GRNNo + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    //for (int i = 0; i < dt1.Rows.Count; i++)
                    //{
                    Job = dt1.Rows[0].ItemArray[0].ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Job;
        }

        public string ReturnJob1(string SO_No)
        {
            string Job1 = "";
            //jobid is change as salesRepID
            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
                String S1 = "Select JobID from tblSalesInvoices where InvoiceNo in ('" + GRNNo + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    //for (int i = 0; i < dt1.Rows.Count; i++)
                    //{
                    Job1 = dt1.Rows[0].ItemArray[0].ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Job1;
        }
        //============================================

        //get invoice NoList======================

        public DataSet ReturnInvNoList(string SO_No)
        {
            DataSet ds = new DataSet();

            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
                String S = "Select ItemID,Sum(RemainQty)as RemainQty,Description,GLAcount,UnitPrice,UOM, sum(Amount) as Amount,Location,IsInclusive,InvType,ServiceCharge,TotalDiscountPercen,Tax1Rate,Tax2Rate,InclusivePrice,LineDiscountPercentage from tblSalesInvoices where InvoiceNo in ('" + GRNNo + "') group by   ItemID,Description,GLAcount,UnitPrice,UOM,Location,IsInclusive,InvType,ServiceCharge,TotalDiscountPercen,Tax1Rate,Tax2Rate,InclusivePrice,LineDiscountPercentage  having Sum(RemainQty)>0";
                //String S = "Select ItemId,Sum(Qty),Description,GLAcount,UnitPrice,UOM, sum(Amount), Location from tblSupplierInvoices where SupInvoiceNo in ('" + GRNNo + "') group by ItemId,Description,GLAcount,UnitPrice,UOM, Location";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                // da.Fill(ds, "SO");
                da.Fill(ds, "INV");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return ds;
        }
        //=====================gettax rate==================
        public double GetTaxRate(string TaxCode)
        {
            double TaxRate = 0.00;
            try
            {
                string ConnString = ConnectionString;
                string sql = "select TaxRate from tblTax where TaxName='" + TaxCode + "'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Close(); Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TaxRate = Convert.ToDouble(reader.GetValue(0));
                }

                reader.Close();
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return TaxRate;
        }

        //========================================================
        public double TaxRate = 0;

        private void cmbtaxSys1_SelectedIndexChanged(object sender, EventArgs e)
        {

            //try
            //{
            //    if (Convert.ToDouble(txtTotalAmount.Text) != 0)
            //    {
            //        if (txtDiscountAmount.Text == "" || txtDiscountAmount.Text == "0")
            //        {
            //            TaxRate = GetTaxRate(cmbtaxSys1.Text);
            //            TaxRate = TaxRate / 100;
            //            double GrossTot = Convert.ToDouble(txtTotalAmount.Text);
            //            double TaxAmount = GrossTot * TaxRate;
            //            txtTax1Amount.Text = TaxAmount.ToString("N2");
            //            double NetTot = GrossTot + TaxAmount;
            //            txtNetTotal.Text = NetTot.ToString("N2");
            //        }
            //        else
            //        {
            //            TaxRate = GetTaxRate(cmbtaxSys1.Text);
            //            TaxRate = TaxRate / 100;
            //            double GrossTot = Convert.ToDouble(txtTotalAmount.Text);
            //            double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
            //            double TaxAmt = (GrossTot - Did_Amt) * TaxRate;
            //            txtTax1Amount.Text = TaxAmt.ToString("N2");
            //            double NetTot = (GrossTot - Did_Amt) + TaxAmt;
            //            txtNetTotal.Text = NetTot.ToString("N2");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Set the Invoice Values First");
            //    }
            //}
            //catch
            //{ }
        }

        public double TaxRate1 = 0;

        private void cmbtaxSys2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    TaxRate1 = GetTaxRate(cmbtaxSys2.Text);
            //    TaxRate1 = TaxRate1 / 100;

            //    if (txtTax1Amount.Text == "0" || txtTax1Amount.Text == "" || txtTax1Amount.Text == "0.00")
            //    {
            //        txtTax1Amount.Text = "0";
            //    }
            //    if (txtDiscountAmount.Text == "" || txtDiscountAmount.Text == "0" || txtDiscountAmount.Text == "0.00")
            //    {
            //        txtDiscountAmount.Text = "0";
            //    }
            //    double DiscountAmt = Convert.ToDouble(txtDiscountAmount.Text);
            //    if (txtDiscountAmount.Text != "" || txtDiscountAmount.Text != "0")
            //    {
            //        double GrossTot = Convert.ToDouble(txtTotalAmount.Text);

            //        double Tax1Amt = Convert.ToDouble(txtTax1Amount.Text);
            //        double TaxAmount = ((GrossTot - DiscountAmt) + Tax1Amt) * TaxRate1;
            //        txtTax2.Text = TaxAmount.ToString("N2");
            //        double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
            //        double NetTot = (GrossTot - Did_Amt) + Tax1Amt;
            //        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N2");
            //    }
            //}
            //catch
            //{ }
        }

        private void dgvCustomerReturn_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomerReturn.CurrentCell == null) return;

            if (flag == true)
            {
            }
            else
            {

                try
                {
                    //if (flglist == 0)
                    //{
                    if (e.ColumnIndex == 2)
                    {
                        System.Drawing.Color R1 = new Color();
                        R1 = Color.White;

                        double ReiveQty = 0;
                        double Unitprice = 0.00;
                        double DiscountRate = 0.00;
                        double DiscountAmount = 0.00;
                        double Amount = 0.00;
                        double Amount1 = 0.00;
                        double TotalAmount = 0.00;

                        // double POQty = 0;//this is a po wise quantity

                        int rows = GetFilledRows();
                        for (int a = 0; a < dgvCustomerReturn.Rows.Count; a++)
                        {
                            if (dgvCustomerReturn.Rows[a].Cells["colInvQty"].Value != null && dgvCustomerReturn.Rows[a].Cells["colUnitPrice"].Value != null)
                            {
                                dgvCustomerReturn.Rows[0].Cells["Discount"].Style.BackColor = R1;
                                dgvCustomerReturn.Rows[0].Cells["colInvQty"].Style.BackColor = R1;

                                ReiveQty = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colInvQty"].Value);

                                Unitprice = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colUnitPrice"].Value);
                                if (dgvCustomerReturn.Rows[a].Cells["Discount"].Value != null)
                                {
                                    if(dgvCustomerReturn.Rows[a].Cells["Discount"].Value.ToString().Trim() == "40+4")
                                    {
                                        double DisPrice1 = ((ReiveQty * Unitprice) * 60 / 100);
                                        Amount1 = DisPrice1 * 96 / 100;
                                    }
                                    else if (dgvCustomerReturn.Rows[a].Cells["Discount"].Value.ToString().Trim() != "")
                                    {
                                        DiscountRate = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["Discount"].Value) / 100;
                                    }
                                }
                                if (dgvCustomerReturn.Rows[a].Cells["Discount"].Value.ToString().Trim() != "40+4")
                                {
                                    Amount = (ReiveQty * Unitprice);
                                    DiscountAmount = Amount * DiscountRate;
                                    Amount1 = Amount - DiscountAmount;
                                }
                            //    dgvCustomerReturn.Rows[a].Cells["colAmount"].Value = Amount1.ToString("N2");

                                TotalAmount = TotalAmount + Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colAmount"].Value);// sanjeewa change cell value 7 into 8

                            }
                        }
                        //txtTotalAmount.Text = TotalAmount.ToString("N2");
                        txtNetTotal.Text = TotalAmount.ToString("N2");
                    }

                   // this.dgvCustomerReturn.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);
                    //}
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void clistbxInvoices_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (clistbxInvoices.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
                {
                    e.NewValue = e.CurrentValue;
                    MessageBox.Show("You can Select only one Invoice");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TaxCalculation()
        {
            double _subTot = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
            Tax2Rate = double.Parse(txtVatPer.Text.Trim());

            if (txtDiscAmount.Text.Trim() == string.Empty || txtDiscAmount.Text == null) txtDiscAmount.Text = "0.00";

            _subTot = double.Parse(txtTotalAmount.Text.Trim()) + double.Parse(txtServCharges.Text.Trim());

            txtDiscAmount.Text = ((_subTot * double.Parse(txtDiscPer.Text.Trim())) / 100).ToString("0.00");

            _subTot = _subTot - double.Parse(txtDiscAmount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());

            _Tax1 = _subTot * Tax1Rate / 100;
            _Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            txtNBT.Text = _Tax1.ToString("0.00");
            txtVat.Text = _Tax2.ToString("0.00");

            txtNetTotal.Text = (_subTot + _Tax1 + _Tax2).ToString("0.00");

            //double _subTot = 0;
            //double _Tax1 = 0;
            //double _Tax2 = 0;

            //Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
            //Tax2Rate = double.Parse(txtVatPer.Text.Trim());

            //if (txtDiscAmount.Text.Trim() == string.Empty || txtDiscAmount.Text == null) txtDiscAmount.Text = "0.00";

            //_subTot = double.Parse(txtTotalAmount.Text.Trim()) + double.Parse(txtServCharges.Text.Trim());

            //txtDiscAmount.Text = ((_subTot * double.Parse(txtDiscPer.Text.Trim())) / 100).ToString("0.00");

            //_subTot = _subTot - double.Parse(txtDiscAmount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());
            ////_subTot = _subTot - double.Parse(txtValueDiscount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());

            //_Tax1 = _subTot * Tax1Rate / 100;
            //_Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            //txtNBT.Text = _Tax1.ToString("0.00");
            //txtVat.Text = _Tax2.ToString("0.00");

            //txtNetTotal.Text = (_subTot + _Tax1 + _Tax2).ToString("0.00");

        }

     

        private void CalculateGridAmounts()
        {
            double _UnitPrice = 0;
            double _UnitPriceIncl = 0;
            double _LineTax = 0;
            double _DiscountP = 0;
            double _LineDiscount = 0;
            double _AmountIncl = 0;
            double _Amount = 0;
            double _AmountAfterDisc = 0;
            double _TotalAmountIncl = 0;
            double _TotalAmount = 0;
            double _Qty = 0;
            double _TempTax1 = 0;
            double _TempTax2 = 0;
            double _DiscountTotal = 0;
            double _Tax1Total = 0;
            double _Tax2Total = 0;
            double _servCharg = 0;
            double _servChargTax1 = 0;
            double _servChargTax2 = 0;
            double _TempTaxRate = 0;
            double _footerDiscP = 0;

            try
            {
                Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
                Tax2Rate = double.Parse(txtVatPer.Text.Trim());
                _footerDiscP = double.Parse(txtDiscPer.Text.Trim());

                if (cmbInvoiceType.Value.ToString()=="2" || (ClassDriiDown.IsInvSerch == true))
                {
                    foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)
                    {
                        if (dgvr.Cells["colItemCode"].Value != null && dgvr.Cells["colItemCode"].Value.ToString() != string.Empty)
                        {
                            if (dgvr.Cells["colRetQty"].Value == null || dgvr.Cells["colRetQty"].Value.ToString() == string.Empty) dgvr.Cells["colRetQty"].Value = "0";
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["colUnitPrice"].Value = "0";
                            if (dgvr.Cells["colUnitPriceIncl"].Value == null || dgvr.Cells["colUnitPriceIncl"].Value.ToString() == string.Empty) dgvr.Cells["colUnitPriceIncl"].Value = "0";
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString() == string.Empty) dgvr.Cells["Discount"].Value = "0";
                            if (dgvr.Cells["colAmount"].Value == null || dgvr.Cells["colAmount"].Value.ToString() == string.Empty) dgvr.Cells["colAmount"].Value = "0";
                            if (dgvr.Cells["colLineDisc"].Value == null || dgvr.Cells["colLineDisc"].Value.ToString() == string.Empty) dgvr.Cells["colLineDisc"].Value = "0";
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString() == string.Empty) dgvr.Cells["colAmountIncl"].Value = "0";
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString() == string.Empty) dgvr.Cells["colLineTax"].Value = "0";

                            _Qty = double.Parse(dgvr.Cells["colRetQty"].Value.ToString());
                            _UnitPrice = double.Parse(dgvr.Cells["colUnitPrice"].Value.ToString());
                            _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                            _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;

                            //Incl Unit Price without Discount
                            //_Amount = (_UnitPrice * _Qty);
                            _TempTax1 = _UnitPrice * Tax1Rate / 100;
                            _TempTax2 = (_UnitPrice + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;
                            dgvr.Cells["colUnitPriceIncl"].Value = (_UnitPrice + _LineTax).ToString("0.00");


                            //Incl Unit Price with Discount
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            //dgvr.Cells["colUnitPriceIncl"].Value = (_UnitPrice + _LineTax).ToString("0.00");
                            dgvr.Cells["colLineDisc"].Value = _LineDiscount.ToString("0.00");
                            dgvr.Cells["colAmount"].Value = (_Amount).ToString("0.00");
                            dgvr.Cells["colLineTax"].Value = (_LineTax).ToString("0.00");
                            dgvr.Cells["colAmountIncl"].Value = (_Amount + _LineTax).ToString("0.00");

                            //after total discount
                            //_LineDiscount = _UnitPrice * _Qty * (_DiscountP+double.Parse(txtPersntage.Text.Trim())) / 100;
                            //_Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            //_TempTax1 = _Amount * Tax1Rate / 100;
                            //_TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            //_LineTax = _TempTax1 + _TempTax2;

                            _Tax1Total = _Tax1Total + _TempTax1;
                            _Tax2Total = _Tax2Total + _TempTax2;

                            _TotalAmount = _TotalAmount + _Amount;


                            //_TotalAmountIncl = _TotalAmountIncl + _Amount + _TempTax1 + _TempTax1;
                        }
                    }
                }

                else if (cmbInvoiceType.Value.ToString() == "1")
                {
                    foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)
                    {
                        if (dgvr.Cells["colItemCode"].Value != null && dgvr.Cells["colItemCode"].Value.ToString() != string.Empty)
                        {
                            if (dgvr.Cells["colRetQty"].Value == null || dgvr.Cells["colRetQty"].Value.ToString() == string.Empty) dgvr.Cells["colRetQty"].Value = "0";
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["colUnitPrice"].Value = "0";
                            if (dgvr.Cells["colUnitPriceIncl"].Value == null || dgvr.Cells["colUnitPriceIncl"].Value.ToString() == string.Empty) dgvr.Cells["colUnitPriceIncl"].Value = "0";
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString() == string.Empty) dgvr.Cells["Discount"].Value = "0";
                            if (dgvr.Cells["colAmount"].Value == null || dgvr.Cells["colAmount"].Value.ToString() == string.Empty) dgvr.Cells["colAmount"].Value = "0";
                            if (dgvr.Cells["colLineDisc"].Value == null || dgvr.Cells["colLineDisc"].Value.ToString() == string.Empty) dgvr.Cells["colLineDisc"].Value = "0";
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString() == string.Empty) dgvr.Cells["colAmountIncl"].Value = "0";
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString() == string.Empty) dgvr.Cells["colLineTax"].Value = "0";

                            _TempTaxRate = Temp_getTaxRate();

                            _Qty = double.Parse(dgvr.Cells["colRetQty"].Value.ToString());
                            _UnitPriceIncl = double.Parse(dgvr.Cells["colUnitPriceIncl"].Value.ToString());
                            _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                            _UnitPrice = _UnitPriceIncl * 100 / (100 + _TempTaxRate);

                            _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                            _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            dgvr.Cells["colUnitPrice"].Value = (_UnitPrice).ToString("0.00");
                            dgvr.Cells["colLineDisc"].Value = _LineDiscount.ToString("0.00");
                            dgvr.Cells["colAmount"].Value = (_Amount).ToString("0.00");
                            dgvr.Cells["colLineTax"].Value = (_LineTax).ToString("0.00");
                            dgvr.Cells["colAmountIncl"].Value = (_Amount + _LineTax).ToString("0.00");

                            //after total discount
                            //_LineDiscount = _UnitPrice * _Qty * (_DiscountP + double.Parse(txtPersntage.Text.Trim())) / 100;
                            //_Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            //_TempTax1 = _Amount * Tax1Rate / 100;
                            //_TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            //_LineTax = _TempTax1 + _TempTax2;

                            _Tax1Total = _Tax1Total + _TempTax1;
                            _Tax2Total = _Tax2Total + _TempTax2;

                            _TotalAmount = _TotalAmount + _Amount;
                            //_TotalAmountIncl = _Amount + _TotalAmountIncl + _Tax1Total + _Tax2Total;
                        }
                    }
                }

                txtTotalAmount.Text = _TotalAmount.ToString("0.00");
                txtGridTotalIncl.Text = (_TotalAmount + _Tax1Total + _Tax2Total).ToString("0.00");

                if (txtServCharges.Text.Trim() == null || txtServCharges.Text.Trim() == string.Empty)
                    txtServCharges.Text = "0.00";

                _servCharg = double.Parse(txtServCharges.Text.Trim());
                _servChargTax1 = _servCharg * Tax1Rate / 100;
                _servChargTax2 = (_servChargTax1 + _servCharg) * Tax2Rate / 100;

                txtNBT.Text = (_Tax1Total + _servChargTax1).ToString("0.00");
                txtVat.Text = (_Tax2Total + _servChargTax2).ToString("0.00");

                txtNetTotal.Text = (_TotalAmount + _servCharg - double.Parse(txtDiscAmount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (_Tax1Total + _servChargTax1) + (_Tax2Total + _servChargTax2)).ToString("0.00");
          
                ///////////////////////////////////

                ////txtTotalAmount.Text = _TotalAmount.ToString("0.00");
                //txtGridTotalIncl.Text = (_TotalAmount + _Tax1Total + _Tax2Total).ToString("0.00");

                //if (txtServCharges.Text.Trim() == null || txtServCharges.Text.Trim() == string.Empty)
                //    txtServCharges.Text = "0.00";

                //_servCharg = double.Parse(txtServCharges.Text.Trim());
                //_servCharg = _servCharg - (_servCharg * double.Parse(txtDiscPer.Value.ToString()) / 100);
                //_servChargTax1 = _servCharg * Tax1Rate / 100;
                //_servChargTax2 = (_servChargTax1 + _servCharg) * Tax2Rate / 100;

                ////txtNBT.Text = (_Tax1Total + _servChargTax1).ToString("0.00");
                ////txtVat.Text = (_Tax2Total + _servChargTax2).ToString("0.00");
                ////txtTotalAmount.Text = (_TotalAmount).ToString("0.00");
                ////txtNetTotal.Text = (_TotalAmount + _servCharg - double.Parse(txtValueDiscount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (_Tax1Total + _servChargTax1) + (_Tax2Total + _servChargTax2)).ToString("0.00");

                //txtNetTotal.Text = (_TotalAmount + double.Parse(txtServCharges.Text.Trim()) - 
                //    double.Parse(txtDiscAmount.Text.Trim()) 
                //    - (double.Parse(txtDiscLineTot.Text.Trim())) + (_Tax1Total + _servChargTax1) 
                //    + (_Tax2Total + _servChargTax2)).ToString("0.00");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {           
            try
            {
                flag = true;
                flglist = 1;
               // txtCreditNoteNo.Text = "";
                Search.searchIssueNoteNo = "";

                if (frmMain.objfrmCusReturnList == null || frmMain.objfrmCusReturnList.IsDisposed)
                {
                    frmMain.objfrmCusReturnList = new frmCusReturnList(1);
                }
                frmMain.ObjCusRetern.TopMost = false;
                frmMain.objfrmCusReturnList.ShowDialog();
                frmMain.objfrmCusReturnList.TopMost = true;

                txtCreditNoteNo.Text = Search.searchIssueNoteNo;
                //txtCreditNoteNo.Text = frmMain.objfrmCusReturnList.RtnSupReturnNo();
                //if (txtCreditNoteNo.Text.Trim().Length > 0)
                //{
                //    txtCreditNoteNo.Text = "";
                //    flglist = 1;
                //    txtCreditNoteNo.Text = frmMain.objfrmCusReturnList.RtnSupReturnNo();
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double Temp_getTaxRate()
        {
            double _TempRate = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            try
            {
                _Tax1 = Tax1Rate;
                _Tax2 = (Tax1Rate + 100) * Tax2Rate / 100;

                _TempRate = _Tax2 + _Tax1;

                //_Rate=100+(100*Tax1Rate/100)+(
                return _TempRate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // int flglist = 0; // this is to check whether list is loaded or not
        private void txtCreditNoteNo_TextChanged(object sender, EventArgs e)
        {
            if (flglist == 1)
            {
                try
                {
                    GetWareHouseDataSet();
                    clistbxInvoices.Items.Clear();
                    dgvCustomerReturn.Rows.Clear();
                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblCutomerReturn where CreditNo='" + txtCreditNoteNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0].ItemArray[30]) == 1)
                        {
                            rdoNonVat.Checked = true;
                        }
                        if (Convert.ToInt32(dt.Rows[0].ItemArray[30]) == 2)
                        {
                            rdoTax.Checked = true;
                        }
                        if (Convert.ToInt32(dt.Rows[0].ItemArray[30]) == 3)
                        {
                            rdoSVat.Checked = true;
                        }
                        //========================================================

                        //for (int k = 0; k < 3; k++)
                        //{
                            //dgvTaxApplicable.Rows.Add();
                            //if (k == 0)
                            //{
                            //    dgvTaxApplicable.Rows[0].Cells[1].Value = dt.Rows[0].ItemArray[26].ToString().Trim();
                            //    dgvTaxApplicable.Rows[0].Cells["colRetQty"].Value = dt.Rows[0].ItemArray[18].ToString().Trim();
                            //}

                            //if (k == 1)
                            //{
                            //    dgvTaxApplicable.Rows[1].Cells[1].Value = dt.Rows[0].ItemArray[27].ToString().Trim();
                            //    dgvTaxApplicable.Rows[1].Cells["colRetQty"].Value = dt.Rows[0].ItemArray[19].ToString().Trim();
                            //}
                            //if (k == 2)
                            //{
                            //    dgvTaxApplicable.Rows[2].Cells[1].Value = dt.Rows[0].ItemArray[31].ToString().Trim();
                            //    dgvTaxApplicable.Rows[2].Cells["colRetQty"].Value = dt.Rows[0].ItemArray[32].ToString().Trim();
                            //}
                            //dgvTaxApplicable.Columns[2].Visible = false;
                            //dgvTaxApplicable.Columns[0].Visible = false;
                            //// dgvTaxApplicable.Columns[3].Visible = false;
                            //dgvTaxApplicable.Columns[4].Visible = false;
                        //}
                        //==================================================================
                        cmbCustomer.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        txtCreditNoteNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        dtpCreditDate.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                        cmbSalesRepID.Text = dt.Rows[0]["SalesRep"].ToString().Trim();
                        cmbjob.Text = dt.Rows[0]["JobID"].ToString().Trim();
                        cmbWarehouse.Value = dt.Rows[0]["LocationID"].ToString().Trim();
                        cmbInvoiceType.Value = dt.Rows[0]["InvType"].ToString().Trim();
                        //cmbWarehouse_RowSelected(sender, e);
                        clistbxInvoices.Items.Clear();
                        // ListBox1.Items.Add(Convert.ToDouble(dt.Tables[0].Rows[i].ItemArray[3]), CheckState.Checked);
                        clistbxInvoices.Items.Add(dt.Rows[0].ItemArray[5].ToString().Trim(), CheckState.Checked);


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvCustomerReturn.Rows.Add();
                            dgvCustomerReturn.Rows[i].Cells["colItemCode"].Value = dt.Rows[i].ItemArray[9].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["colDesc"].Value = dt.Rows[i].ItemArray[12].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["colInvQty"].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["colRetQty"].Value = dt.Rows[i].ItemArray[11].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["UOM"].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["colUnitPrice"].Value = dt.Rows[i].ItemArray[14].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["Discount"].Value = dt.Rows[i]["LineDiscP"].ToString().Trim();
                            dgvCustomerReturn.Rows[i].Cells["colAmount"].Value = Convert.ToDouble(dt.Rows[i].ItemArray[16]).ToString("N2");

                        }

                        txtServCharges.Text = Convert.ToDouble(dt.Rows[0]["ServCharg"]).ToString("0.00");


                        string x = dt.Rows[0]["Discount"].ToString();
                            txtDiscPer.Value = Convert.ToDouble(dt.Rows[0]["Discount"]).ToString("0.00");
                      
                          
                        
                       
                        txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[20]).ToString("N2");
                        txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[21]).ToString("N2");

                        txtNBT.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString("N2");
                        txtVat.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N2");

                        txtNBTPer.Text = Convert.ToDouble(dt.Rows[0].ItemArray[35]).ToString("N2");
                        txtVatPer.Text = Convert.ToDouble(dt.Rows[0].ItemArray[36]).ToString("N2");

                        btnSave.Enabled = false;
                        dgvCustomerReturn.Enabled = true;
                        dgvCustomerReturn.ReadOnly = true;
                        clistbxInvoices.Enabled = false;

                        //CalculateGridAmounts();
                       // TaxCalculation();
                    }
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
        }

        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();

                if (File.Exists(Application.StartupPath + "\\CRCustomerReturn.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("CRCustomerReturn.rpt");
                }
                else
                {
                    MessageBox.Show("CRCustomerReturn.rpt not Exists.");
                    this.Close();
                    return;
                }
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crp.PrintToPrinter(1, true, 0, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            //************************


            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }
            if (txtCreditNoteNo.Text.Trim().Length > 0)
            {
                ds.Clear();

                try
                {
                    String S12 = "Select * from tblCustomerMaster where CutomerID='" + cmbCustomer.Text.ToString() + "'";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd12 = new SqlCommand(S12);
                    SqlConnection con12 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                    da12.Fill(ds, "CustomerMaster1");

                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + txtCreditNoteNo.Text.Trim() + "' and ReturnQty >0";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DTReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    //DirectPrint();

                    frmViewerCustomerReturn cusReturn = new frmViewerCustomerReturn(this);
                    cusReturn.Show();
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer return and try again");
            }
        }

        public double NAmountDC = 0.0;//Net Amount for discount Calculation

        private void dgvCustomerReturn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowCount = GetFilledRows();
                NAmountDC = 0.0;
                double DispatchQty = 0.00;
                double unitprice = 0.00;
                double DiscountRate = 0.00;
                double DiscountAmount = 0.00;
                double Amount = 0.00;
                double Amount1 = 0.00;
                double TotalAmount = 0.00;


                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvCustomerReturn.Rows[a].Cells["colRetQty"].Value != null && dgvCustomerReturn.Rows[a].Cells["colUnitPrice"].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colRetQty"].Value);
                        unitprice = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colUnitPrice"].Value);
                        if (dgvCustomerReturn.Rows[a].Cells["Discount"].Value.ToString() == "40+4")
                        {
                            double DisPrice1 = ((DispatchQty * unitprice) * 60 / 100);
                            Amount1 = DisPrice1 * 96 / 100;
                        }
                        else if (dgvCustomerReturn.Rows[a].Cells["Discount"].Value != null)
                        {
                            DiscountRate = Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["Discount"].Value) / 100;
                            Amount = (DispatchQty * unitprice);
                            DiscountAmount = Amount * DiscountRate;
                            Amount1 = Amount - DiscountAmount;
                        }

                    
                        dgvCustomerReturn.Rows[a].Cells["colAmount"].Value = Amount1.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvCustomerReturn.Rows[a].Cells["colAmount"].Value);// sanjeewa change cell value 7 into 8
                    }
                }
                //txtTotalAmount.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
                //=========================================
                // dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells["colRetQty"];
                
                //CalculateGridAmounts();
                //TaxCalculation();
                //===================================================================


                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();

        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse First");
                    return;
                }

                if (Convert.ToDouble(dgvCustomerReturn.CurrentRow.Cells["colRetQty"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + dgvCustomerReturn.CurrentRow.Cells["colRetQty"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            dgvCustomerReturn.CurrentRow.Cells["colRetQty"].Selected = true;
                        }
                    }
                }
                else
                {
                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("CreditNote", cmbWarehouse.Text.ToString().Trim(),
                        dgvCustomerReturn.CurrentRow.Cells["colItemCode"].Value.ToString(),
                        Convert.ToDouble(dgvCustomerReturn.CurrentRow.Cells["colRetQty"].Value.ToString()),
                        txtCreditNoteNo.Text.Trim(), flag, clsSerializeItem.DtsSerialNoList, GetCheckedGRNList(), false, true);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable GetCheckedGRNList()
        {
            DataTable _dtblGRN = new DataTable();
            _dtblGRN.Columns.Add("RefNo");

            try
            {
                for (int i = 0; i < clistbxInvoices.Items.Count; i++)
                {
                    if (clistbxInvoices.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] GRNNo = clistbxInvoices.Items[i].ToString().Split(':');

                        DataRow dr = _dtblGRN.NewRow();
                        dr["RefNo"] = GRNNo[0];
                        _dtblGRN.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dtblGRN;
        }

        private void cmbWarehouse_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    //if(flglist!=1)
                    if (e.Row.Activated == true)
                    {
                        txtWarehouseName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();

                        StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                        StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                        cmbARAccount.Text = StrARAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbCustomer_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        Load_Invoices(cmbCustomer.ActiveRow.Cells[0].Value.ToString());
                        txtcusName.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtCusAdd1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                        txtCusAdd2.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnVATPrint_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }
            if (txtCreditNoteNo.Text.Trim().Length > 0)
            {
                INVTYPE = 2;
                ds.Clear();

                try
                {

                    String S12 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd12 = new SqlCommand(S12);
                    SqlConnection con12 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                    da12.Fill(ds, "CustomerMaster1");

                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + txtCreditNoteNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DTReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    // DirectPrint();

                    frmViewerCustomerReturn cusReturn = new frmViewerCustomerReturn(this);
                    cusReturn.Show();
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer return and try again");
            }
        }

        private void btnSVATPrint_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }
            if (txtCreditNoteNo.Text.Trim().Length > 0)
            {
                INVTYPE = 3;
                ds.Clear();

                try
                {
                    String S12 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd12 = new SqlCommand(S12);
                    SqlConnection con12 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                    da12.Fill(ds, "CustomerMaster1");


                    String S1 = "Select * from tblCutomerReturn WHERE CreditNo = '" + txtCreditNoteNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DTReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    // DirectPrint();

                    frmViewerCustomerReturn cusReturn = new frmViewerCustomerReturn(this);
                    cusReturn.Show();
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer return and try again");
            }
        }

        private void dgvCustomerReturn_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCustomerReturn.IsCurrentCellDirty)
                {
                    dgvCustomerReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtServCharges_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtServCharges.Text.Trim(), out amount))
                {
                    txtServCharges.Text = "0.00";
                    return;
                }

               // CalculateGridAmounts();
                //TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtDiscPer.Text.Trim(), out amount))
                {
                    txtDiscPer.Text = "0.00";
                    return;
                }
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //CalculateGridAmounts();
                //TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtNBTPer.Text.Trim(), out amount))
                {
                    txtNBTPer.Text = "0.00";
                    return;
                }
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //CalculateGridAmounts();
                //TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtVatPer.Text.Trim(), out amount))
                {
                    txtVatPer.Text = "0.00";
                    return;
                }
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //CalculateGridAmounts();
                //TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoNonVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                   // FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoTax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTax.Checked)
                {
                   // FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoSVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoSVat.Checked)
                {
                  //  FillInvoiceType();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Credit Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void FillInvoiceType()
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVatPer.Text = "0.00";
                    txtVat.Text = "0.00";

                    txtNBT.ReadOnly = true;
                    txtNBTPer.ReadOnly = true;
                    txtVat.ReadOnly = true;
                    txtVatPer.ReadOnly = true;
                }
                else if (rdoTax.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVatPer.Text = "0.00";
                    txtVat.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTPer.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                    LoadtaxDetails();
                }
                else if (rdoSVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTPer.Text = "0.00";
                    txtVatPer.Text = "0.00";
                    txtVat.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTPer.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVatPer.ReadOnly = false;
                    LoadtaxDetails();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if ((dgvCustomerReturn.CurrentCell.ColumnIndex == 3 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 5 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 6 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 7)
                    && dgvCustomerReturn.EditingControl.Text != null &&
                    dgvCustomerReturn.CurrentCell.Value.ToString().Trim() != string.Empty)
                {
                    //dgvItems.EditingControl.Text = dgvItems.CurrentCell.Value.ToString();
                    TextBox innerTextBox;
                    if (e.Control is TextBox)
                    {
                        innerTextBox = e.Control as TextBox;
                        innerTextBox.KeyPress += new KeyPressEventHandler(innerTextBox_KeyPress);
                    }
                }
            }
            catch (Exception ex)
            {
                //objCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void innerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if ((dgvCustomerReturn.CurrentCell.ColumnIndex == 3 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 5 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 6 ||
                    dgvCustomerReturn.CurrentCell.ColumnIndex == 7)
                    && e.Value != null && e.Value.ToString().Trim() != string.Empty)
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                //objCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        public void InvoiceCalculation(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {
                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblcolLineDiscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblcolLineTax = 0;

                    //double dblTotNBT = 0;
                    //double dblInvTotal = 0;

                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = 0;
                    dblVATPer = Convert.ToDouble(txtVatPer.Text);  //12%
                    double dblNBTPer = 0;
                    dblNBTPer = Convert.ToDouble(txtNBTPer.Text);  //2%
                    double TotalAmount = 0;


                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)
                        {
                            //ug.ActiveCell.Row.Cells

                            if (dgvr.Cells["colItemCode"].Value == null || dgvr.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineDisc"].Value = 0.00;
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                            if (dgvr.Cells["colUnitPriceIncl"].Value == null || dgvr.Cells["colUnitPriceIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPriceIncl"].Value = 0.00;
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colAmountIncl"].Value = 0.00;
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineTax"].Value = 0.00;

                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);//here price in inclusive eg-114.24
                         //   dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;

                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                double Totalprice = DisPrice1 * 96 / 100;
                                dblcolLineDiscountAmount = (dblLinePrice * dblLineQty) - Totalprice;
                            }
                            else
                            {
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                                dblcolLineDiscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            }

                            //dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            //dblcolLineDiscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 2, MidpointRounding.AwayFromZero);

                            //dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colRetQty"].Value);

                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                               // dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;



                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            //dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colUnitPriceIncl"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal;
                            dgvr.Cells["LineTotal"].Value = Math.Round(dblLineQty * dblLinePrice, 2, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colLineDisc"].Value = dblcolLineDiscountAmount;
                            //VAT
                            dblcolLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);

                            //dblcolLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["LineTotal"].Value) - Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineTax"].Value = Math.Round(dblcolLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblcolLineDiscountAmount, 2, MidpointRounding.AwayFromZero);
                            //txtGridTotalExcl.Text = TotalAmount.ToString();

                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["LineTotal"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = (Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero)).ToString();
                        //txtpaid.Text = txtNetTotal.Text.ToString();

                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)
                        
                        {

                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                            if (dgvr.Cells["colUnitPriceIncl"].Value == null || dgvr.Cells["colUnitPriceIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPriceIncl"].Value = 0.00;
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colAmountIncl"].Value = 0.00;
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineTax"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineDisc"].Value = 0.00;


                            if (dgvr.Cells["colItemCode"].Value == null || dgvr.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colRetQty"].Value);

                            //dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblInclusiveLineTotal = (dblLinePrice * dblLineQty);
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                            {
                                dbdiscountPer = 0;
                            }
                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                double Totalprice = DisPrice1 * 96 / 100;
                                dblcolLineDiscountAmount = (dblLinePrice * dblLineQty) - Totalprice;
                            }
                            else
                            {
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                                dblcolLineDiscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            }

                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblcolLineDiscountAmount;

                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblcolLineDiscountAmount;

                            dgvr.Cells["colLineDisc"].Value = dblcolLineDiscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            //dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            //dblcolLineDiscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            //dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblcolLineDiscountAmount;

                            dgvr.Cells["LineTotal"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblcolLineTax = dblLineNBT + dblLineVAT;
                            //dblcolLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value) - Convert.ToDouble(dgvr.Cells["LineTotal"].Value), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colLineTax"].Value = Math.Round(dblcolLineTax, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblcolLineDiscountAmount, 2, MidpointRounding.AwayFromZero);
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["LineTotal"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        //txtpaid.Text = txtNetTotal.Text.ToString();

                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)
                        
                        {
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                            if (dgvr.Cells["colUnitPriceIncl"].Value == null || dgvr.Cells["colUnitPriceIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPriceIncl"].Value = 0.00;
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colAmountIncl"].Value = 0.00;
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineTax"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineDisc"].Value = 0.00;

                            if (dgvr.Cells["colItemCode"].Value == null || dgvr.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colRetQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);

                            if (dgvr.Cells["Discount"].Value.ToString() == "40+4")
                            {
                                double DisPrice1 = ((dblLinePrice * dblLineQty) * 60 / 100);
                                double Totalprice = DisPrice1 * 96 / 100;
                                dblcolLineDiscountAmount = (dblLinePrice * dblLineQty) - Totalprice;
                            }
                            else
                            {
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                                dblcolLineDiscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            }

                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblcolLineDiscountAmount;

                            // dgvr.Cells["colAmountIncl"].Value = Math.Round(((dblLineQty * dblLinePrice)), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineDisc"].Value = dblcolLineDiscountAmount;

                            dgvr.Cells["LineTotal"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);
                            //VAT
                            // dblLineVAT = Math.Round(Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value) - Convert.ToDouble(dgvr.Cells["LineTotal"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblcolLineDiscountAmount, 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            //dgvr.Activated = true;
                            //ug.PerformAction(UltraGridAction.CommitRow);
                            //ug.PerformAction(UltraGridAction.ExitEditMode);  
                            TotalAmount = TotalAmount + Convert.ToDouble(dgvr.Cells["LineTotal"].Value);
                            txtGridTotalExcl.Text = TotalAmount.ToString();
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Text = dblTotalLinedDiscount.ToString();
                        txtSubValue.Text = dblSubTotal.ToString();
                        txtVat.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        //txtpaid.Text = txtNetTotal.Text.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FooterCalculation()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;



                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Text.ToString().Trim());
                    dblServiceChgF = Convert.ToDouble(txtServCharges.Text.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Text.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    txtDiscAmount.Text = (dblTotalDiscount +
                        (double.Parse(txtServCharges.Text.ToString()) * double.Parse(txtDiscPer.Text.ToString()) / 100)).ToString();

                    double _AmountWithoutDisc = double.Parse(txtSubValue.Text.ToString()) + //102
                        double.Parse(txtDiscLineTot.Text) -
                        double.Parse(txtDiscAmount.Text.ToString()) +
                        double.Parse(txtServCharges.Text.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                        if (cmbInvoiceType.Value.ToString() == "2")
                        {
                            txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                            txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                                double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                        }
                        else
                        {
                            txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                            txtVat.Value = Convert.ToDouble((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                            double.Parse(txtVatPer.Value.ToString()) / 100).ToString("0.00");
                        }

                    }
                    if (cmbInvoiceType.Value.ToString() == "3")
                    {
                        txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTPer.Text.ToString()) / 100).ToString();
                        txtVat.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                            double.Parse(txtVatPer.Text.ToString()) / 100).ToString();
                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Text.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Text.ToString().Trim());

                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF;// +double.Parse(txtDiscAmount.Value.ToString());

                    txtNetTotal.Text = dblNetTotalF.ToString("N2"); //127.99
                    //Asanga
                    //txtpaid.Text = dblNetTotalF.ToString("N2");

                    if (cmbInvoiceType.Value.ToString() == "1")
                        txtDiscAmount.Text = ((Convert.ToDouble(txtGridTotalExcl.Text) -
                            Convert.ToDouble(txtDiscLineTot.Text)) *
                            Convert.ToDouble(txtDiscPer.Text) / 100).ToString();

                    else
                        txtDiscAmount.Text = (Convert.ToDouble(txtSubValue.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();
                    txtNBT.Text = Convert.ToDouble(txtNBT.Text.ToString().Trim()).ToString("0.00"); //txtNBT.Text.ToString("N2");
                    txtVat.Text = Convert.ToDouble(txtVat.Text.ToString().Trim()).ToString("0.00");  //txtVat.Text.ToString("N2");
                }
            }
            catch { }
        }


        public void FooterCalculation_temp()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;
                    double dblTotalDiscountTemp = 0;

                    dblGrossTotalF = Convert.ToDouble(txtTotalAmount.Text.ToString().Trim()); 
                    dblServiceChgF = Convert.ToDouble(txtServCharges.Text.ToString().Trim()); //0
                    
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Value.ToString().Trim()) / 100; //152
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());//0
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF; //152

                    dblTotalDiscountTemp = dblTotalDiscount +
                    (double.Parse(txtServCharges.Text.ToString()) * double.Parse(txtDiscPer.Value.ToString()) / 100);
                    double _AmountWithoutDisc = double.Parse(txtTotalAmount.Text.ToString()) + double.Parse(txtDiscLineTot.Text) - dblTotalDiscountTemp + double.Parse(txtServCharges.Text.ToString()); //152

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                        txtNBT.Value = _AmountWithoutDisc * double.Parse(txtNBTPer.Value.ToString()) / 100;
                        txtVat.Value = (_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                        double.Parse(txtVatPer.Value.ToString()) / 100; //164.16
                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Value.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Value.ToString().Trim());
                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF;
                    txtNetTotal.Text = dblNetTotalF.ToString("N2");
                    //Asanga
                    //txtpaid.Text = dblNetTotalF.ToString("N2");

                    if (cmbInvoiceType.Value.ToString() == "1")
                        txtDiscAmount.Text = (Convert.ToDouble(txtGridTotalExcl.Text) *
                            Convert.ToDouble(txtDiscPer.Text) / 100).ToString();
                    else
                        txtDiscAmount.Text = (Convert.ToDouble(txtTotalAmount.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();


                }
            }
            catch { }
        }

        public void InvoiceCalculation_temp(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {

                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    txtGridTotalExcl.Text = "0";

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (DataGridViewRow ugR in dgvCustomerReturn.Rows)
                        {
                            if (ugR.Cells["colUnitPrice"].Value == null || ugR.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["colUnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["colRetQty"].Value == null || ugR.Cells["colRetQty"].Value.ToString().Trim() == string.Empty) ugR.Cells["colRetQty"].Value = 0.00;
                            if (ugR.Cells["colUnitPriceIncl"].Value == null || ugR.Cells["colUnitPriceIncl"].Value.ToString().Trim() == string.Empty) ugR.Cells["colUnitPriceIncl"].Value = 0.00;
                            if (ugR.Cells["colAmountIncl"].Value == null || ugR.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) ugR.Cells["colAmountIncl"].Value = 0.00;
                            if (ugR.Cells["colLineTax"].Value == null || ugR.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["colLineTax"].Value = 0.00;

                            //ug.ActiveCell.Row.Cells
                            if (ugR.Cells["colItemCode"].Value == null || ugR.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["colUnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 2, MidpointRounding.AwayFromZero);

                            txtGridTotalExcl.Text = (double.Parse(txtGridTotalExcl.Text) + dblDiscountedPrice).ToString();

                            dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblLineQty = Convert.ToDouble(ugR.Cells["colRetQty"].Value);

                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                          
                            ugR.Cells["colUnitPriceIncl"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            ugR.Cells["colAmountIncl"].Value = dblInclusiveLineTotal;
                            ugR.Cells["colAmount"].Value = Math.Round(dblLineQty * dblLinePrice, 2, MidpointRounding.AwayFromZero);
                         
                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            ugR.Cells["colLineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtTotalAmount.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetTotal.Text = ( Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero)).ToString();
                        //txtpaid.Text = txtNetValue.Value.ToString();
                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (DataGridViewRow ugR in dgvCustomerReturn.Rows)
                        {
                            if (ugR.Cells["colItemCode"].Value == null || ugR.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["colRetQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                            
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal;// -dblLinediscountAmount;

                            txtDiscAmount.Text = (Convert.ToDouble(txtTotalAmount.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();

                            if (txtDiscPer.Text.ToString() != null && Convert.ToDouble(txtDiscPer.Text) != 0)
                            {
                                dbdiscountPer = Convert.ToDouble(txtDiscPer.Text);
                                dblDiscountedPrice = Math.Round(dblLinePrice - ((dblLinePrice * dbdiscountPer)/100), 2, MidpointRounding.AwayFromZero);
                                dblExcluisvePrice = dblDiscountedPrice;// Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                                ugR.Cells["colUnitPriceIncl"].Value = dblExcluisvePrice;
                                ugR.Cells["colAmountIncl"].Value = dblDiscountedPrice * dblLineQty;
                                //dblLineNBT = Math.Round(((dblLinePrice * dblLineQty) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                                //dblLineVAT = Math.Round((((dblLinePrice * dblLineQty) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dblDiscountedPrice =dblLinePrice - ((dblLinePrice * dbdiscountPer));
                                dblExcluisvePrice = dblDiscountedPrice;// Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                                ugR.Cells["colUnitPriceIncl"].Value = dblExcluisvePrice;
                                ugR.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;
        
                            }
                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                           
                            ugR.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            ugR.Cells["colLineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["colAmount"].Value), 2, MidpointRounding.AwayFromZero);
                            
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);


                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtTotalAmount.Text = dblSubTotal.ToString();
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        //txtpaid.Text = txtNetValue.Value.ToString();
                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (DataGridViewRow ugR in dgvCustomerReturn.Rows)
                        {
                            if (ugR.Cells["colUnitPrice"].Value == null || ugR.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["colUnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["colRetQty"].Value == null || ugR.Cells["colRetQty"].Value.ToString().Trim() == string.Empty) ugR.Cells["colRetQty"].Value = 0.00;
                            if (ugR.Cells["colUnitPriceIncl"].Value == null || ugR.Cells["colUnitPriceIncl"].Value.ToString().Trim() == string.Empty) ugR.Cells["colUnitPriceIncl"].Value = 0.00;
                            if (ugR.Cells["colAmountIncl"].Value == null || ugR.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) ugR.Cells["colAmountIncl"].Value = 0.00;
                            if (ugR.Cells["colLineTax"].Value == null || ugR.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["colLineTax"].Value = 0.00;

                            if (ugR.Cells["colItemCode"].Value == null || ugR.Cells["colItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["colRetQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            ugR.Cells["colUnitPriceIncl"].Value = dblExcluisvePrice;
                            ugR.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT                            
                            ugR.Cells["colLineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtTotalAmount.Text = dblSubTotal.ToString();
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        //txtpaid.Text = txtNetValue.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvCustomerReturn_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            double Tax1Rate = double.Parse(txtNBTPer.Value.ToString().Trim());
            double Tax2Rate = double.Parse(txtVatPer.Value.ToString().Trim());
            InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));

            //if (Convert.ToInt64(cmbInvoiceType.Value) == 1 )
            //{
            //    rdoNonVat.Checked=true;
            //}
            //else if (Convert.ToInt64(cmbInvoiceType.Value) == 2 )
            //{
            //    rdoTax.Checked=true;
            //}
            //else 
            //{
            //    rdoNonVat.Checked = true;
            //}
                
            FooterCalculation();          

        }

        private void dgvCustomerReturn_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsSerialNoCorrect()
        {
            try
            {

                int _Count = 0;
                // Presuming the DataTable has a column named Date. 
                string expression;
                foreach (DataGridViewRow dgvr in dgvCustomerReturn.Rows)  //foreach (UltraGridRow dgvr in ug.Rows)
                {
                    if (dgvr.Cells["colItemCode"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["colItemCode"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["colRetQty"].Value.ToString()) > 0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for colItemCode=" + dgvr.Cells["colItemCode"].Value.ToString().Trim(),
                                    "Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["colItemCode"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["colRetQty"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["colRetQty"].Value.ToString()))
                            {
                                MessageBox.Show("Enter Serial Numbers for colItemCode=" + dgvr.Cells["colItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}