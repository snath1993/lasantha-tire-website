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
using Interop.PeachwServer;


namespace UserAutherization
{
    public partial class frmSupplierReturn : Form
    {
        clsCommon objclsCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        public DsSupplierInvoice ds = new DsSupplierInvoice();
        public static DateTime UserWiseDate = System.DateTime.Now;
        DataTable dtUser = new DataTable();
        bool run = false;
        int flglist = 0; // this is to check whether list is loaded or not
        public string StrSql;
        public string StrAP = null;
        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Peachtree - Supllier Return";
        bool IsFind = false;
        public static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL;

        public frmSupplierReturn()
        {
            setConnectionString();
            InitializeComponent();
            IsFind = false;
        }

        public frmSupplierReturn(string SupRetNo)
        {
            setConnectionString();
            InitializeComponent();
            IsFind = true;
            flglist = 1;
            txtGRn_NO.Text = SupRetNo;
        }

        //bool IsMinusAllow = false;
        public DSGRN objGRN = new DSGRN();
        public static string ConnectionString;

        public void setConnectionString()
        {
            try
            {
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Enable()
        {
            try
            {
                cmbVendorSelect.Enabled = true;
                dtpReturnDate.Enabled = true;
                cmbAR.Enabled = true;
                chkReferingSI.Enabled = true;
                //cmbLocation.Enabled = true;
                //txtlocName.Enabled = true;
                //txtLocAdd1.Enabled = true;
                //txtLocAdd2.Enabled = true;
                txtCustomerSO.Enabled = true;

                cmbtaxSys1.Enabled = true;
                cmbtaxSys2.Enabled = true;

                txtTax1Amount.Enabled = true;
                txtTax2.Enabled = true;

                txtDiscountAmount.Enabled = true;
                txtDisRate.Enabled = true;
                // txtNetTotal.Enabled = true;

                //txtSupName.Enabled = true;
                //txtSupCity.Enabled = true;
                //txtSupAdd1.Enabled = true;
                //txtSupAdd2.Enabled = true;
                // txtGRn_NO.Enabled = true;
                // txtTotalAmount.Enabled = true;
                dgvSupplierReturn.Enabled = true;

                btnSave.Enabled = true;
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
                cmbVendorSelect.Enabled = false;
                dtpReturnDate.Enabled = false;
                cmbAR.Enabled = false;
                chkReferingSI.Enabled = false;
                cmbLocation.Enabled = false;
                txtlocName.Enabled = false;
                txtLocAdd1.Enabled = false;
                txtLocAdd2.Enabled = false;
                txtCustomerSO.Enabled = false;

                cmbtaxSys1.Enabled = false;
                cmbtaxSys2.Enabled = false;

                txtTax1Amount.Enabled = false;
                txtTax2.Enabled = false;

                txtDiscountAmount.Enabled = false;
                txtDisRate.Enabled = false;
                txtNetTotal.Enabled = false;

                txtSupName.Enabled = false;
                txtSupCity.Enabled = false;
                txtSupAdd1.Enabled = false;
                txtSupAdd2.Enabled = false;
                //txtGRn_NO.Enabled = false;
                txtTotalAmount.Enabled = false;
                dgvSupplierReturn.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //get data to fill the vendor=========================

        public DataTable FillVendor()
        {
            try
            {
                DataTable dataTable = new DataTable("Vendor");
                string ConnString = ConnectionString;
                string sql = "Select VendorID,VendorName from tblVendorMaster";// where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
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
        //==========================================================
        //following code segment fill the vendor


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
        //***************Load Tax Details*******************************************
        private void LoadtaxDetails()
        {
            try
            {
                bool Active = true;
                string ConnString = ConnectionString;
                String S1 = "Select TaxName,Rate,Account,TaxID from tblTaxApplicable where IsActive='" + Active + "' order by Rank"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //txtNBTP.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.000");
                    //txtVATP.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.000");

                    //Tax1ID = dt.Rows[0]["TaxID"].ToString();
                    //Tax2ID = dt.Rows[1]["TaxID"].ToString();

                    //Tax1Name = dt.Rows[0]["TaxName"].ToString();
                    //Tax2Name = dt.Rows[1]["TaxName"].ToString();

                    //TaxRate = double.Parse(dt.Rows[0]["Rate"].ToString());
                    //TaxRate1 = double.Parse(dt.Rows[1]["Rate"].ToString());

                    Tax1GLAccount = user.TaxPayGL1;
                    Tax2GLAccount = user.TaxPayGL2;
                }

                //if (dt.Rows.Count > 0)
                //{
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{

                //    dgvTaxApplicable.Rows.Add();

                //    dgvTaxApplicable.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                //    dgvTaxApplicable.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                //    dgvTaxApplicable.Rows[i].Cells[2].Value = false;
                //    dgvTaxApplicable.Rows[i].Cells[3].Value = "0";
                //    dgvTaxApplicable.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                //    dgvTaxApplicable.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[3].ToString().Trim();

                //}
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //***************************************

        public void GetCurrentUserDate()
        {
            try
            {
                dtpReturnDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpReturnDate.Value = UserWiseDate;
                //    //.ToString().Trim();
                //    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //***************************************************
        public static bool TaxMethod = false;
        //private void LoadtaxDetails()
        //{
        //    try
        //    {
        //        bool Active = true;
        //        string ConnString = ConnectionString;
        //        String S1 = "Select TaxName,Rate,Account,TaxID from tblTaxApplicable where IsActive='" + Active + "' order by Rank"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
        //        // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
        //        SqlCommand cmd1 = new SqlCommand(S1);
        //        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da1.Fill(dt);

        //        if (dt.Rows.Count > 0)
        //        {

        //            txtNBTP.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
        //            txtVATP.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

        //            Tax1ID = dt.Rows[0]["TaxID"].ToString();
        //            Tax2ID = dt.Rows[1]["TaxID"].ToString();

        //            Tax1Name = dt.Rows[0]["TaxName"].ToString();
        //            Tax2Name = dt.Rows[1]["TaxName"].ToString();

        //            TaxRate = double.Parse(dt.Rows[0]["Rate"].ToString());
        //            TaxRate1 = double.Parse(dt.Rows[1]["Rate"].ToString());

        //            Tax1GLAccount = user.TaxPayGL1;
        //            Tax2GLAccount = user.TaxPayGL2;


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
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
        //*****************************************************
        public bool flag = false;
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
                if (TaxApplicable == false)
                {
                    txtNBTP.Enabled = false;
                    txtVATP.Enabled = false;
                    txtNBT.Enabled = false;
                    txtVAT.Enabled = false;
                }
                else
                {
                    txtNBTP.Enabled = true;
                    txtVATP.Enabled = true;
                    txtNBT.Enabled = true;
                    txtVAT.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================================

        public void GetVendorDataset()
        {
            dsVendor = new DataSet();
            try
            {
                dsVendor.Clear();
                StrSql = " SELECT VendorID, VendorName,VContact,VAddress1,VAddress2 FROM tblVendorMaster order by VendorID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsVendor, "DtVendor");

                cmbVendorSelect.DataSource = dsVendor.Tables["DtVendor"];
                cmbVendorSelect.DisplayMember = "VendorID";
                cmbVendorSelect.ValueMember = "VendorID";
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VendorID"].Width = 75;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VendorName"].Width = 150;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VContact"].Hidden = true;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VAddress1"].Hidden = true;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VAddress2"].Hidden = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==========================================
        /// <summary>
        ///
        /// </summary>
        public CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();
        public void GetARAccount()
        {
            dsAR = new DataSet();
            try
            {
                dsAR.Clear();
                StrSql = " SELECT AcountID, AccountDescription FROM tblChartofAcounts order by AcountID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsAR, "DtAR");

                cmbAR.DataSource = dsAR.Tables["DtAR"];
                cmbAR.DisplayMember = "AcountID";
                cmbAR.ValueMember = "AcountID";
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                // cmbAR.DisplayLayout.Bands["DtAR"].Columns["AcountID"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 200;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAPAccount()
        {
            try
            {
                StrSql = "SELECT [SalesInvDrAc] FROM tblDefualtSetting";

                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrAP = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbAR.Text = StrAP.Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                GetARAccount();
                GetAPAccount();
                GetVendorDataset();
                TaxValidation();
                flglist = 0;
                Disable();

                SqlConnection con1 = new SqlConnection(ConnectionString);
                string s1 = "select IsMinusAllow from tblDefualtSetting";
                SqlDataAdapter da1 = new SqlDataAdapter(s1, con1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows[0].ItemArray[0].ToString().Trim() != "")
                {
                    if (Convert.ToBoolean(dt1.Rows[0].ItemArray[0].ToString()) == true)
                    {
                        //IsMinusAllow = true;
                    }
                    else
                    {
                        //IsMinusAllow = false;
                    }
                }

                //if flag==true it is a serch option
                if (flag == true)
                {
                    chkSupplierInvoices.Items.Clear();
                    // string SupplierReturnNo = ab.GetText2();
                    string SerchText = ab.GetText2();

                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblGRNTran where GRN_NO='" + SerchText + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da2 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da2.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            txtGRn_NO.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                            cmbVendorSelect.Text = dt.Rows[i].ItemArray[1].ToString().Trim();

                            string abc = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dtpReturnDate.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                            cmbAR.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                            txtTotalAmount.Text = dt.Rows[i].ItemArray[15].ToString().Trim();

                            cmbtaxSys1.Text = dt.Rows[i].ItemArray[27].ToString().Trim();
                            cmbtaxSys2.Text = dt.Rows[i].ItemArray[28].ToString().Trim();

                            txtTax1Amount.Text = dt.Rows[i].ItemArray[29].ToString().Trim();
                            txtTax2.Text = dt.Rows[i].ItemArray[30].ToString().Trim();

                            txtDiscountAmount.Text = dt.Rows[i].ItemArray[26].ToString().Trim();
                            txtDisRate.Text = dt.Rows[i].ItemArray[25].ToString().Trim();
                            txtNetTotal.Text = dt.Rows[i].ItemArray[24].ToString().Trim();
                            txtCustomerSO.Text = dt.Rows[i].ItemArray[21].ToString().Trim();
                            cmbLocation.Text = dt.Rows[i].ItemArray[22].ToString().Trim();
                            cmbLocation_SelectedIndexChanged(sender, e);

                            dgvSupplierReturn.Rows.Add();

                            dgvSupplierReturn.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();

                            // DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvSupplierReturn.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                            dgvSupplierReturn.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[12].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[23].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[11].ToString().Trim();
                        }
                    }
                    btnPrint.Enabled = true;
                    btnClose.Enabled = true;

                    btnNew.Enabled = true;
                }
                else
                {
                    LoadTaxMethod();
                    GetCurrentUserDate();
                    LoadtaxDetails();
                    flag = false;
                    load_Decimal();
                    WarehouseDataLoad();
                    TaxRateLoad();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void TaxRateLoad()
        {
            try
            {
                cmbtaxSys1.Items.Clear();
                cmbtaxSys2.Items.Clear();
                String S = "Select TaxName from tblTax";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbtaxSys1.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                    cmbtaxSys2.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WarehouseDataLoad()
        {
            try
            {
                String S = "Select * from tblWhseMaster";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbLocation.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
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

        //=============following code segment load the suppllier invoices====
        private void Load_SupInvList(string VendorID)
        {
            try
            {
                dgvSupplierReturn.Rows.Clear();
                txtTotalAmount.Text = "0.00";
                txtNetTotal.Text = "0.00";
                txtNBT.Text = "0.000";
                txtVAT.Text = "0.000";
                chkSupplierInvoices.Items.Clear();


                bool IsReturn = false;
                //get the supplier invoioce numbers
                String S = "Select distinct(SupInvoiceNo) from tblSupplierInvoices where CustomerID='" + VendorID + "' and IsReturn = '" + IsReturn + "' and IsActive = '"+false+ "' and RemainQty!=0";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        chkSupplierInvoices.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                    }
                }

                String S2 = "Select distinct(SupInvoiceNo) from tblDirectSupplierInvoices where CustomerID='" + VendorID + "' and IsReturn = '" + IsReturn + "' and IsActive = '" + false + "' and RemainQty!=0";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        chkSupplierInvoices.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=======================================================================      

        private void dgvSupplierReturn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSupplierReturn.CurrentRow.Cells[0].Value != null)
            {
                if (IsThisItemSerial(dgvSupplierReturn.CurrentRow.Cells[0].Value.ToString()))
                    btnSNO.Enabled = true;
                else
                    btnSNO.Enabled = false;
            }
        }

        private void dgvSupplierReturn_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
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
                for (int a = 0; a <= rows; a++)
                {
                    if (dgvSupplierReturn.Rows[a].Cells[2].Value != null && dgvSupplierReturn.Rows[a].Cells[5].Value != null)
                    {
                        dgvSupplierReturn.Rows[0].Cells[6].Style.BackColor = R1;
                        dgvSupplierReturn.Rows[0].Cells[2].Style.BackColor = R1;

                        ReiveQty = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[2].Value);
                        //========================================================================                       

                        Unitprice = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);
                        DiscountRate = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value) / 100;


                        Amount = (ReiveQty * Unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8

                    }
                }
                txtTotalAmount.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        //private void dgvdispactApplytoSales_CellLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ColumnIndex == 3)
        //        {
        //            if (chkReferingSI.CheckState == CheckState.Checked)
        //            {
        //                if (Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value))
        //                {
        //                    MessageBox.Show("You cannot Exceed the Invoice Quantity......!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value = "0";
        //                }
        //            }
        //            else
        //            {
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                string s = "select QTY from tblItemWhse where ItemID = '" + dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + "'";
        //                SqlDataAdapter da = new SqlDataAdapter(s, con);
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);

        //                if (dt.Rows.Count > 0)
        //                {
        //                    if (Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) < Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value))
        //                    {
        //                        if (IsMinusAllow == false)
        //                        {
        //                            MessageBox.Show("You are not allowed to enter minus qty");
        //                            dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value = "";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + " item is not in this warehouse");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // conn.ImportPurchaseOrderList();
                Connector conn = new Connector();
                conn.ImportPurchaseOrderList();
                conn.Insert_PurchaseOrderList();
                // conn.Insert_SalesOrder();
                MessageBox.Show("Purchase orders Loaded");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void LoadSalesLines(string CheckItem)
        {
            try
            {
                //  String S = "Select * from tblSalesOrder where SalesOrderNo = '" + clistbxSalesOrder.SelectedItem.ToString().Trim() + "'";
                String S = "Select * from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // String S1 = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo = '" + clistbxSalesOrder.SelectedItem.ToString().Trim() + "'";
                String S1 = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt.Rows.Count > 0)
                {
                    // txtCusPO.Text = dt.Rows[0].ItemArray[3].ToString();
                    //dgvdispactApplytoSales.Rows.Clear();

                    //dgvdispactApplytoSales.DataSource = dt1;
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        // dgvdispactApplytoSales[0, i].Value = dt.Rows[i].ItemArray[3].ToString();
                        dgvSupplierReturn.Rows.Add();
                        dgvSupplierReturn.Rows[i].Cells[0].Value = dt1.Rows[i].ItemArray[0].ToString();
                        dgvSupplierReturn.Rows[i].Cells[1].Value = dt1.Rows[i].ItemArray[1].ToString();
                        dgvSupplierReturn.Rows[i].Cells[2].Value = "0.00";
                        dgvSupplierReturn.Rows[i].Cells[3].Value = dt1.Rows[i].ItemArray[2].ToString();
                        dgvSupplierReturn.Rows[i].Cells[4].Value = dt1.Rows[i].ItemArray[3].ToString();
                        dgvSupplierReturn.Rows[i].Cells[5].Value = dt1.Rows[i].ItemArray[4].ToString();
                        dgvSupplierReturn.Rows[i].Cells[6].Value = "0.00";
                    }
                }
                else
                {
                    //dgvdispactApplytoSales.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool multiselect = false;
        private void clistbxSalesOrder_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                // chkSupplierInvoices.Enabled = false;
                if (chkSupplierInvoices.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
                {
                    e.NewValue = e.CurrentValue;
                    multiselect = true;
                    MessageBox.Show("You can only check one item");
                }
                else
                {
                    multiselect = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public ArrayList ArraySONO = new ArrayList();//Sales orders related to a custmer

        public string SelectSONums1 = "";//saving purpose

        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();

            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
               
                String S = "Select ItemId,Sum(RemainQty),Description,GLAcount,UnitPrice,UOM, sum(Amount), Location,Tax1Amount,Tax2Amount,Discount,DisCountRate,DisCountRate1,Tax1Rate,Tax2Rate from tblSupplierInvoices where SupInvoiceNo in ('" + GRNNo + "') and RemainQty !=0 group by ItemId,Description,GLAcount,UnitPrice,UOM, Location,Tax1Amount,Tax2Amount,Discount,DisCountRate,DisCountRate1,Tax1Rate,Tax2Rate";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    da.Fill(ds, "SO");
                }
                else
                {

                    String S2 = "Select ItemId,Sum(RemainQty),Description,GLAcount,UnitPrice,UOM, sum(Amount), Location,Tax1Amount,Tax2Amount,Discount,DisCountRate,DisCountRate1,Tax1Rate,Tax2Rate from tblDirectSupplierInvoices where SupInvoiceNo in ('" + GRNNo + "') and RemainQty !=0 group by ItemId,Description,GLAcount,UnitPrice,UOM, Location,Tax1Amount,Tax2Amount,Discount,DisCountRate,DisCountRate1,Tax1Rate,Tax2Rate";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    da2.Fill(ds, "SO");
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                string GRNNo = SO_No.Replace("'", "").Trim();
                String S1 = "Select PONO from tblSupplierInvoices where SupInvoiceNo in ('" + GRNNo + "')";
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

        //============================================================================

        //==========================================================================
        private void CreateXmlToExportGRNAdjust(SqlTransaction tr, SqlConnection con)
        {
            //double PQty = 0;
            //double SysQty = 0;
            //double Adjust_qty = 0;
            // string StockOnHand = "";
            string ClosingStock = "";
            SqlCommand myCommand4 = new SqlCommand("Select GRNCrAc from tblDefualtSetting", con, tr);
            // SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            DataSet dt = new DataSet();
            da.Fill(dt);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                // StockOnHand = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                ClosingStock = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            }


            DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
            // string reference = "RTN" + "-" + DTP.Day + DTP.Month + DTP.Year;
            string reference = txtGRn_NO.Text.ToString().Trim();
            string Dformat = "MM/dd/yyyy";
            string AdjustmentDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\InvAdjustmentGRN.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            int GridrowCount = GetFilledRows();//get row count from data grid

            //create a start elemet=========================

            for (int k = 0; k < GridrowCount; k++)
            {
                //if (Convert.ToDouble(dgvItem[4, k].Value) != 0)
                //{
                // PQty = Convert.ToDouble(dgvItem[4, k].Value);
                // SysQty = Convert.ToDouble(dgvItem[3, k].Value);
                // Adjust_qty = PQty - SysQty;
                double ReceivedQty = Convert.ToDouble(dgvSupplierReturn[2, k].Value);
                if (ReceivedQty != 0)
                {
                    Writer.WriteStartElement("PAW_Item");
                    Writer.WriteAttributeString("xsi:type", "paw:item");


                    //crate a ID element (tag)=====================(1)
                    Writer.WriteStartElement("ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvSupplierReturn[0, k].Value.ToString().Trim());//dgvItem[0, c].Value
                    Writer.WriteEndElement();

                    //this sis crating tag for reference============(2)
                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(reference);
                    Writer.WriteEndElement();

                    //This is a Tag for Adjusment Date==============(3)
                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:date");
                    Writer.WriteString(AdjustmentDate);//Date format must be (MM/dd/yyyy)
                    Writer.WriteEndElement();

                    //This is a Tag for numberof dsistribution=======(4)

                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    //Adjustmne Lines=================================(5)
                    Writer.WriteStartElement("AdjustmentItems");
                    //Adjustmne Lines=================================(6)
                    Writer.WriteStartElement("AdjustmentItem");


                    //Gl ASccount======================================(7)
                    Writer.WriteStartElement("GLSourceAccount ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(ClosingStock);
                    Writer.WriteEndElement();


                    //Unit Cost========================================(8)
                    //Writer.WriteStartElement("UnitCost");
                    //Writer.WriteString(dgvSupplierReturn[5, k].Value.ToString().Trim());
                    //Writer.WriteEndElement();

                    //Quantity========================================(9)

                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                    Writer.WriteString("-" + ReceivedQty.ToString().Trim());
                    //Adjust_qty
                    Writer.WriteEndElement();
                    //Amount===========================================(10)
                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(dgvSupplierReturn[7, k].Value.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReasonToAdjust");
                    Writer.WriteString("Good Return");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//Adjustment Line
                    Writer.WriteEndElement();//Adjustment lines


                    Writer.WriteEndElement();//Item Closed Tag

                    // Writer.WriteEndElement();//Items Closed Tag

                    //   Writer.Close();//finishing writing xml file
                }

            }

            Writer.Close();//finishing writing xml file
        }
        //**************************************************************************

        public int ChechDQty = 0;
        public int checkRetrn = 0;
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

        //********************************************************
        public int GetFilledRowstax()
        {
            int _count = 0;
            try
            {
                if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                    _count = _count + 1;

                if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0)
                    _count = _count + 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _count;
        }

        //**************************************
        //making a xmlfile =======================================
        public void CreateSupplierReturnJXML(SqlTransaction tr, SqlConnection con)
        {
            DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierReturnApply.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Purchases");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int DistributionNumber = 0;
            int rowCount1 = GetFilledRows();
            int TemprowCount1=rowCount1;
            string NoDistributions=string.Empty;
            if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                TemprowCount1 = TemprowCount1 + 1;
            if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0)
                TemprowCount1 = TemprowCount1 + 1;

            NoDistributions = TemprowCount1.ToString();
            double TotalLineDiscount = 0;

            foreach (DataGridViewRow dgvr in dgvSupplierReturn.Rows)
            {
                try
                {
                    TotalLineDiscount = TotalLineDiscount + ((double.Parse(dgvr.Cells[3].Value.ToString()) * double.Parse(dgvr.Cells[5].Value.ToString())) - double.Parse(dgvr.Cells[7].Value.ToString()));
                }
                catch (Exception ex)
                {

                }

            }

            //if (TotalLineDiscount > 0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}

            //if(Convert.ToDouble(txtSpecialDiscount.Text)>0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}

            //if (Convert.ToDouble(txtCashDiscount.Text) > 0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}
            //for (int x = 0; x < dt.Tables[0].Rows.Count; x++)
            //{
            //Writer.WriteStartElement("PAW_Purchase");
            //Writer.WriteAttributeString("xsi:type", "paw:purchase");

            Writer.WriteStartElement("PAW_Purchase");
            Writer.WriteAttributeString("xsi:type", "paw:purchase");

            Writer.WriteStartElement("VendorID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbVendorSelect.Value.ToString());//Vendor ID should be here = Ptient No
            Writer.WriteEndElement();

            //if (i == 0)
            //{
            Writer.WriteStartElement("Invoice_Number");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(txtGRn_NO.Text.ToString().Trim());
            Writer.WriteEndElement();
            //}                       

            Writer.WriteStartElement("Date");
            //Writer.WriteAttributeString("xsi:type", "paw:id");  
            Writer.WriteString(dtpReturnDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("AP_Account");
            // Writer.WriteStartElement("Accounts_Payable_Account");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("TRUE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(NoDistributions);
            Writer.WriteEndElement();


            Writer.WriteStartElement("PurchaseLines");
            double lineAMT = 0;
            for (int i = 0; i < rowCount1 + 2; i++)
            {
                if (i < rowCount1)
                {
                    DistributionNumber = i + 1;

                    if (IsThisItemSerial(dgvSupplierReturn.Rows[i].Cells[0].Value.ToString()))
                    {
                        if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                        {
                            foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                            {
                                if (dr["ItemCode"].ToString() == dgvSupplierReturn.Rows[i].Cells[0].Value.ToString())
                                {
                                    Writer.WriteStartElement("PurchaseLine");
                                    Writer.WriteStartElement("Quantity");
                                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[3].Value.ToString());//Doctor Charge
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Item_ID");
                                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[0].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Description");
                                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[1].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("GL_Account");
                                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[8].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Tax_Type");
                                    Writer.WriteString("1");//Doctor Charge
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Unit_Price");
                                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[5].Value.ToString());
                                    Writer.WriteEndElement();

                                    Writer.WriteStartElement("Serial_Number");
                                    Writer.WriteAttributeString("xsi:type", "paw:id");
                                    Writer.WriteString(dr["SerialNo"].ToString());
                                    Writer.WriteEndElement();


                                    lineAMT = (Convert.ToDouble(dgvSupplierReturn.Rows[i].Cells["colAmount"].Value) * (100 - (Convert.ToDouble(textBox2.Text))) / 100) * (100 - (Convert.ToDouble(txtDiscountAmount1.Text))) / 100;
                                    Writer.WriteStartElement("Amount");
                                  //  Writer.WriteString("-"+lineAMT.ToString());
                                    Writer.WriteString(lineAMT.ToString());

                                    Writer.WriteEndElement();
                                    Writer.WriteEndElement();//LINE
                                }
                            }
                        }
                    }
                    else
                    {
                        Writer.WriteStartElement("PurchaseLine");
                        Writer.WriteStartElement("Quantity");
                       // Writer.WriteString("-" + dgvSupplierReturn.Rows[i].Cells[3].Value.ToString());//Doctor Charge
                        Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[3].Value.ToString());//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[0].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[1].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[8].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[5].Value.ToString());
                        Writer.WriteEndElement();

                        lineAMT = (Convert.ToDouble(dgvSupplierReturn.Rows[i].Cells["colAmount"].Value) * (100 - (Convert.ToDouble(textBox2.Text))) / 100) * (100 - (Convert.ToDouble(txtDiscountAmount1.Text))) / 100;
                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(lineAMT.ToString());
                        Writer.WriteEndElement();//LINE
                    }

                }
                //==================================================================================
                if (i == rowCount1)
                {
                    if (double.Parse(txtNBT.Text.Trim()) > 0)
                    {
                        //  Writer.WriteStartElement("PurchaseLines");
                        Writer.WriteStartElement("PurchaseLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        // Writer.WriteString("NBT001");
                        Writer.WriteString(NBitemid);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        //Writer.WriteString("NBT");
                        Writer.WriteString(NBTitemDis);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("GL_Account");
                        // Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(NBTitemGL);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Tax1Amount.ToString());//HospitalCharge
                        Writer.WriteEndElement();

                        // Writer.WriteEndElement();
                        Writer.WriteEndElement();
                    }
                }

                if (i == rowCount1 + 1)
                {
                    if (double.Parse(txtVAT.Text.Trim()) > 0)
                    {
                        // Writer.WriteStartElement("PurchaseLines");
                        Writer.WriteStartElement("PurchaseLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        // Writer.WriteString("VAT001");
                        Writer.WriteString(VATitemid);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        // Writer.WriteString("VAT");
                        Writer.WriteString(VATitemDis);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        // Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(VATGL);
                        Writer.WriteEndElement();

                        //========================================================
                        //Writer.WriteStartElement("Tax_Type");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Tax2Amount.ToString());//tax amount1
                        Writer.WriteEndElement();


                        //Writer.WriteStartElement("AppliedToSO");
                        //Writer.WriteString("FALSE");
                        //Writer.WriteEndElement();

                        //  Writer.WriteEndElement();
                        Writer.WriteEndElement();
                    }
                }


                //if (i == rowCount1 + 1)
                //{
                //    if (TotalLineDiscount > 0)
                //    {
                //        // Writer.WriteStartElement("PurchaseLines");
                //        Writer.WriteStartElement("PurchaseLine");

                //        Writer.WriteStartElement("Quantity");
                //        Writer.WriteString("-1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Tax_Type");
                //        Writer.WriteString("1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Item_ID");
                //        // Writer.WriteString("VAT001");
                //        Writer.WriteString(LineDisitemid);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Description");
                //        // Writer.WriteString("VAT");
                //        Writer.WriteString(LineDisitemdescription);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("GL_Account");
                //        // Writer.WriteAttributeString("xsi:type", "paw:id");
                //        //Writer.WriteString("40000-MF");
                //        Writer.WriteString(LineDisGLAccount);
                //        Writer.WriteEndElement();

                //        //========================================================
                //        //Writer.WriteStartElement("Tax_Type");
                //        //Writer.WriteString("1");//Doctor Charge
                //        //Writer.WriteEndElement();

                //        Writer.WriteStartElement("Amount");
                //        Writer.WriteString(TotalLineDiscount.ToString());//tax amount1
                //        Writer.WriteEndElement();


                //        //Writer.WriteStartElement("AppliedToSO");
                //        //Writer.WriteString("FALSE");
                //        //Writer.WriteEndElement();

                //        //  Writer.WriteEndElement();
                //        Writer.WriteEndElement();
                //    }
                //}

                //if (i == rowCount1 + 1)
                //{
                //    if (Convert.ToDouble(txtDiscountAmount1.Text) > 0)
                //    {
                //        // Writer.WriteStartElement("PurchaseLines");
                //        Writer.WriteStartElement("PurchaseLine");

                //        Writer.WriteStartElement("Quantity");
                //        Writer.WriteString("-1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Tax_Type");
                //        Writer.WriteString("1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Item_ID");
                //        // Writer.WriteString("VAT001");
                //        Writer.WriteString(Cashitemid);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Description");
                //        // Writer.WriteString("VAT");
                //        Writer.WriteString(cashitemdis);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("GL_Account");
                //        // Writer.WriteAttributeString("xsi:type", "paw:id");
                //        //Writer.WriteString("40000-MF");
                //        Writer.WriteString(cashGL);
                //        Writer.WriteEndElement();

                //        //========================================================
                //        //Writer.WriteStartElement("Tax_Type");
                //        //Writer.WriteString("1");//Doctor Charge
                //        //Writer.WriteEndElement();

                //        Writer.WriteStartElement("Amount");
                //        Writer.WriteString(txtDiscountAmount1.Text.ToString());//tax amount1
                //        Writer.WriteEndElement();


                //        //Writer.WriteStartElement("AppliedToSO");
                //        //Writer.WriteString("FALSE");
                //        //Writer.WriteEndElement();

                //        //  Writer.WriteEndElement();
                //        Writer.WriteEndElement();
                //    }
                //}


                //if (i == rowCount1 + 1)
                //{
                //    if (Convert.ToDouble(textBox2.Text) > 0)
                //    {
                //        // Writer.WriteStartElement("PurchaseLines");
                //        Writer.WriteStartElement("PurchaseLine");

                //        Writer.WriteStartElement("Quantity");
                //        Writer.WriteString("-1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Tax_Type");
                //        Writer.WriteString("1");//Doctor Charge
                //        Writer.WriteEndElement();


                //        Writer.WriteStartElement("Item_ID");
                //        // Writer.WriteString("VAT001");
                //        Writer.WriteString(SpecialDisItemid);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Description");
                //        // Writer.WriteString("VAT");
                //        Writer.WriteString(SpecialDisItemdescription);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("GL_Account");
                //        // Writer.WriteAttributeString("xsi:type", "paw:id");
                //        //Writer.WriteString("40000-MF");
                //        Writer.WriteString(SpecialDisGLAccount);
                //        Writer.WriteEndElement();

                //        //========================================================
                //        //Writer.WriteStartElement("Tax_Type");
                //        //Writer.WriteString("1");//Doctor Charge
                //        //Writer.WriteEndElement();

                //        Writer.WriteStartElement("Amount");
                //        Writer.WriteString(textBox2.Text.ToString());//tax amount1
                //        Writer.WriteEndElement();


                //        //Writer.WriteStartElement("AppliedToSO");
                //        //Writer.WriteString("FALSE");
                //        //Writer.WriteEndElement();

                //        //  Writer.WriteEndElement();
                //        Writer.WriteEndElement();
                //    }
                //}

            }
            //********************
            Writer.WriteEndElement();//last line
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            //  }
            Writer.Close();
        }

        private bool IsWarehouseHaveStock(double _RtnStock)
        {
            try
            {
                double _IssQty = 0;
                //DataSet _dts = new DataSet();
                //_dts = objclsBLLPhases.GetToBeIssuedQty(ucmbBOMID.Value.ToString().Trim());

                foreach (DataGridViewRow dr in dgvSupplierReturn.Rows)
                {
                    _IssQty = 0;
                    foreach (DataGridViewRow udr in dgvSupplierReturn.Rows)
                    {
                        if (udr.Cells[0].Value != null)
                        {
                            if (dr.Cells[0].ToString() == udr.Cells[0].Value.ToString())
                            {
                                _IssQty = _IssQty + double.Parse(udr.Cells[3].Value.ToString().Trim());
                            }
                            if (_RtnStock < _IssQty)
                            {
                                MessageBox.Show("Not Enough Qty for " + dr.Cells[0].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        //
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=======================================================


        public Boolean IsGridValidation()
        {
            try
            {
                double dilqty = 0;
                double RQty = 0;
                if (dgvSupplierReturn.Rows.Count == 0)
                {
                    return false;
                }
                foreach (DataGridViewRow ugR in dgvSupplierReturn.Rows)
                {
                    if (ugR.Cells["colUP"].Value != null)
                    {
                        {
                            dilqty = dilqty + double.Parse(ugR.Cells["colUP"].Value.ToString());
                        }

                        if (double.Parse(ugR.Cells["colUP"].Value.ToString()) <= 0)
                        {
                            MessageBox.Show("Enter Valid Unit Price of Item ID '" + ugR.Cells["ItemID"].Value.ToString() + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }

                    if (ugR.Cells["colRQty"].Value != null)
                    {
                        {
                            RQty = RQty + double.Parse(ugR.Cells["colRQty"].Value.ToString());
                        }

                        if (double.Parse(ugR.Cells["colRQty"].Value.ToString()) < 0)
                        {
                            MessageBox.Show("Enter Valid Quantity of Item ID '" + ugR.Cells["ItemID"].Value.ToString() + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }

                if (RQty <= 0)
                {
                    MessageBox.Show("Enter Return Quantity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
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

        public bool IsSerialNoCorrect()
        {
            try
            {
                //DataTable table = DataSet1.Tables["Orders"];
                int _Count = 0;
                // Presuming the DataTable has a column named Date. 
                string expression;
                foreach (DataGridViewRow dgvr in dgvSupplierReturn.Rows)
                {
                    if (dgvr.Cells["ItemID"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["ItemID"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["colRQty"].Value.ToString()) > 0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemID"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["ItemID"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["colRQty"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["colRQty"].Value.ToString()))
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemID"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        string StrReference = null;
        public string GetInvNoField(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT SupReturnNoPref, SupReturnAppInvPad, SupReturnAppInvNo FROM tblDefualtSetting";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }
                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intCRNNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(SupReturnAppInvNo) FROM tblDefualtSetting ORDER BY SupReturnAppInvNo DESC";

                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intCRNNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intCRNNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET SupReturnAppInvNo='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
         

            dgvSupplierReturn.EndEdit();
            if (btnEditClick == false)
            {
                if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_Warehouse(cmbLocation.Text, sMsg)) return;

                if (IsGridValidation() == false)
                {
                    return;
                }
                string TranType = "Sup-Return";
                int DocType = 7;
                bool QtyIN = false;
                dgvSupplierReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);

                if (cmbVendorSelect.Value == null)
                {
                    MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbAR.Value == null)
                {
                    MessageBox.Show("Invalid AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (!IsValidQuantity())
                    return;

                if (!IsSerialNoCorrect())
                    return;

                //bool IsminusAllow = false;

                int rowCount = GetFilledRows();
                int rows = GetFilledRows();
                bool MynusValue = false;

                bool IsItemSerial = false;
                //check wether this item is serialized or not=======================

                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
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
                        IsItemSerial = true;
                        String S1 = "Select SerialNO from tblSerialSupReturnTemp where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataSet dt1 = new DataSet();
                        da1.Fill(dt1);
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value) == dt1.Tables[0].Rows.Count)
                        {
                            IsItemSerial = false;
                        }
                    }
                }

                int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

                Tax1Amount = double.Parse(txtNBT.Text.Trim());
                Tax2Amount = double.Parse(txtVAT.Text.Trim());

                try
                {
                    //Connector objConnector = new Connector();
                    //if (!(objConnector.IsOpenPeachtree(dtpReturnDate.Value)))
                    //    return;

                    for (int b = 0; b < rows; b++)
                    {
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[2].Value))
                        {
                            //IsMinusAllow = true;
                        }
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) < 0)
                        {
                            ////MynusValue = true;
                        }
                    }

                    //check the qty is whether a number or not
                    for (int a = 0; a < rows; a++)
                    {
                        ChechDQty = 0;
                        checkRetrn = 0;
                        if ((dgvSupplierReturn.Rows[a].Cells[2].Value == null) || (dgvSupplierReturn.Rows[a].Cells[2].Value.ToString() == ""))
                        {
                            dgvSupplierReturn.Rows[a].Cells[2].Value = 0;
                        }
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value);
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value);
                        if(txtTax1Amount.Text=="")
                        {
                            txtTax1Amount.Text = "0";
                        }
                        Convert.ToDouble(txtTax1Amount.Text);
                        if (txtTax2.Text == "")
                        {
                            txtTax2.Text = "0";
                        }
                        Convert.ToDouble(txtTax2.Text);
                        Convert.ToDouble(txtDisRate.Text);
                        if (txtDiscountAmount.Text == "")
                        {
                            txtDiscountAmount.Text = "0";
                        }
                        Convert.ToDouble(txtDiscountAmount.Text);
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[2].Value) == 0)
                        {
                            checkRetrn = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChechDQty = 1;//if this flag is 1 the violate the number format
                    objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                }

                if (ChechDQty == 0)
                {
                    string SupplierReturnNo = "";

                    //==============form level validations
                    if (txtTotalAmount.Text == "" || Convert.ToDouble(txtTotalAmount.Text) == 0 || cmbLocation.Text == "" ||
                        checkRetrn == 1 || MynusValue == true || rows == 0)
                    {
                        //if (user.IsMinusAllow == false)
                        //{
                        //    MessageBox.Show("You can not Return more than Invoice Qty");
                        //    //IsMinusAllow = false;
                        //    return;
                        //    //btnSave.Focus();
                        //}
                        //if (IsItemSerial == true)
                        //{
                        //    MessageBox.Show("You have not entered serial numbers for this items");
                        //    return;
                        //    //btnSave.Focus();
                        //}
                        if (MynusValue == true)
                        {
                            MessageBox.Show("Quantity Field must be Positive Value");
                            return;
                            //btnSave.Focus();
                        }
                        if (rows == 0)
                        {
                            MessageBox.Show("Select an invoice");
                            return;
                            //btnSave.Focus();
                        }
                        if (Convert.ToDouble(txtTotalAmount.Text) == 0)
                        {
                            MessageBox.Show("Enter Return Quantiy");
                            return;
                            //btnSave.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Fill All Details");
                            return;
                            //btnSave.Focus();
                        }
                    }
                    else
                    {
                        DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
                        string Dformat = "MM/dd/yyyy";
                        string GRNDate = DTP.ToString(Dformat);

                        SqlConnection myConnection = new SqlConnection(ConnectionString);
                        myConnection.Open();
                        SqlTransaction myTrans = myConnection.BeginTransaction();
                        try
                        {
                            SqlCommand myCommand = null;
                            if (user.IsSRTNNoAutoGen)
                            {
                                //myCommand = new SqlCommand("UPDATE tblDefualtSetting SET SupplierReturnNo = SupplierReturnNo + 1 select SupplierReturnNo, SupReturnPrefix from tblDefualtSetting", myConnection, myTrans);
                                //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                //DataTable dt41 = new DataTable();
                                //da41.Fill(dt41);

                                //if (dt41.Rows.Count > 0)
                                //{
                                //    SupplierReturnNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                                //    SupplierReturnNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + SupplierReturnNo;
                                //}
                                //txtGRn_NO.Text = SupplierReturnNo;

                            

                                if (btnEditClick == false)
                                {
                                    StrReference = GetInvNoField(myConnection, myTrans);
                                    UpdatePrefixNo(myConnection, myTrans);
                                    txtGRn_NO.Text = StrReference;
                                    // txtCreditNo.Text = StrReference;
                                }
                            }
                            else
                            {
                                myCommand = new SqlCommand("select * from tblSupplierReturn where SupReturnNo='" + txtGRn_NO.Text.Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                DataTable dt41 = new DataTable();
                                da41.Fill(dt41);

                                if (dt41.Rows.Count > 0)
                                {
                                    MessageBox.Show("Supplier Return No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    myTrans.Rollback();
                                    myConnection.Close();//
                                    return;
                                }
                            }
                            // int rows = GetFilledRows();
                            for (int i = 0; i < rows; i++)
                            {
                                // bool IsfullShipment = false;
                                bool Isinvoice = false;
                                // string TranType = "SupReturn";

                                if (Convert.ToDouble(dgvSupplierReturn[2, i].Value) != 0)
                                {
                                    // bool Duplicate = true;
                                    string NoOfDis = Convert.ToString(rows);

                                    bool isRefereSI = true;

                                    if (chkReferingSI.CheckState == CheckState.Checked)
                                    {
                                        isRefereSI = true;
                                    }
                                    else
                                    {
                                        isRefereSI = false;
                                    }
                                    if (SelectSONums1 == "" || SelectSONums1 == null)
                                    {
                                        SelectSONums1 = chkSupplierInvoices.SelectedItem.ToString();
                                    }
                                    if (StrReference == "" || StrReference == null)
                                    {
                                        StrReference = txtGRn_NO.Text.ToString();
                                    }
                                  
                                    string ToLocation = "SupReurn";
                                    if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                    {
                                        //SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAPAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[3, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "', '" + isRefereSI + "')", myConnection, myTrans);
                                        SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal, " +
                                            " TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI,Tax3Name,Tax3Amount,IsActive,Discount,DiscountAmount) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[1, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtSpecialDiscount.Text) + "','" + Convert.ToDouble(textBox2.Text) + "','" +
                                            Tax1Name + "','" +
                                            Tax2Name + "','" + Tax1Amount + "','" + Tax2Amount +
                                            "','" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() +
                                            "', '" + isRefereSI + "','" + Tax3Name + "','" +
                                            Tax3Amount + "','"+true+"','"+Convert.ToDouble(txtCashDiscount.Text) + "','" + Convert.ToDouble(txtDiscountAmount1.Text) + "')", myConnection, myTrans);
                                        myCommand2.ExecuteNonQuery();
                                    }

                                    //SqlCommand myCommand51 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + " where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                    //SqlDataAdapter da1 = new SqlDataAdapter(myCommand51);
                                    //DataTable dt1 = new DataTable();
                                    //da1.Fill(dt1);

                                    //double ss = 0.00;

                                    //if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                    //{
                                    //    SqlCommand cmd11 = new SqlCommand(
                                    //        "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbLocation.Text.ToString().Trim() + "' AND ItemId='" + dgvSupplierReturn[0, i].Value + "') " +
                                    //        "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtGRn_NO.Text.ToString().Trim() + "','" + GRNDate + "','" + TranType + "','" + QtyIN + "','" + dgvSupplierReturn[0, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) * Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + cmbLocation.Text.ToString().Trim() + "','" + ss + "')", myConnection, myTrans);
                                    //    cmd11.ExecuteNonQuery();
                                    //}

                                    ////=============================
                                    //SqlCommand myCommand5 = new SqlCommand("insert into tblInvTransaction(TDate,ItemID,FrmWhseId,ToWhseId,QTY,TransType) values ('" + GRNDate + "','" + dgvSupplierReturn[0, i].Value + "','" + cmbLocation.Text.ToString().Trim() + "','" + ToLocation + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + TranType + "')", myConnection, myTrans);
                                    //myCommand5.ExecuteNonQuery();
                                    //=======================================================================
                                    //===================================================================
                                    //SqlCommand myCommandSe = new SqlCommand("Select * from  tblSerialSupReturnTemp where ItemID='" + dgvSupplierReturn[0, i].Value + "'", myConnection, myTrans);
                                    ////myCommand2.ExecuteNonQuery();
                                    //SqlDataAdapter daSe = new SqlDataAdapter(myCommandSe);
                                    //DataTable dtSe = new DataTable();
                                    //daSe.Fill(dtSe);
                                    //Insert to serial numbers table to serial numbers whic are taken from the serialisetemp table============

                                    //string TranType = "GRN";
                                    bool IsGRNProcess = true;
                                    string Status = "SupReturn";

                                    //for (int j = 0; j < dtSe.Rows.Count; j++)
                                    //{
                                    //    SqlCommand myCommandSe1 = new SqlCommand("insert into tblSerialSupReturn(RTNO,ItemID,Description,SerialNO,TransactionType,IsRTNProcess,WLocation)values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value.ToString() + "','" + dgvSupplierReturn[1, i].Value.ToString() + "','" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "','" + IsGRNProcess + "','" + cmbLocation.Text.ToString().Trim() + "')", myConnection, myTrans);
                                    //    myCommandSe1.ExecuteNonQuery();
                                    //    //  }
                                    //    //=======================Update the grn table of==================================================
                                    //    myCommand.CommandText = "Update tblSerializeItem SET IsInvoice = '" + IsGRNProcess + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WLocation='" + cmbLocation.Text.ToString().Trim() + "'";
                                    //    myCommand.ExecuteNonQuery();

                                    //    myCommand.CommandText = "Update tblSerialItemTransaction SET Status = '" + Status + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "'";
                                    //    myCommand.ExecuteNonQuery();
                                    //    //================================================================================
                                    //}

                                    //==========================================================================================

                                    //SqlCommand myCommand7 = new SqlCommand("update tblSupplierInvoices set RemainQty = RemainQty- '" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                                    ////myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                                    //myCommand7.ExecuteNonQuery();
                                    //==========================================
                                    // SqlCommand myCommand5 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "  Select * from  tblItemWhse where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                    //if (dt1.Rows.Count > 0)
                                    //{ }
                                    //else
                                    //{
                                    //    //SqlCommand myCommand3 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')", myConnection, myTrans);
                                    //    //// myCommand.CommandText = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')";
                                    //    //myCommand3.ExecuteNonQuery();
                                    //}

                                    if (IsItemSerial)
                                    {
                                        //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                                        //{
                                        //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                                        //        " TranType='Sup-Return',Status='OutOfStock' " +
                                        //        " where ItemID='" +
                                        //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                                        //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                                        //    myCommandSe1.ExecuteNonQuery();
                                        //}
                                    }
                                }
                            }

                            /***********************/

                            //for (int i = 0; i < rows; i++)
                            //{
                            //    SqlCommand myCommand6 = new SqlCommand("select sum(ReturnQty) from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //    //string s = "select qty from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "'";
                            //    SqlDataAdapter daz = new SqlDataAdapter(myCommand6);
                            //    DataTable dtz = new DataTable();
                            //    daz.Fill(dtz);

                            //    SqlCommand myCommand7 = new SqlCommand("select sum(Qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //    //  string s2 = "select sum(qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "'";
                            //    SqlDataAdapter da2 = new SqlDataAdapter(myCommand7);
                            //    DataTable dt2 = new DataTable();
                            //    da2.Fill(dt2);

                            //    if (dtz.Rows.Count > 0)
                            //    {
                            //        if (dtz.Rows[0].ItemArray[0].ToString().Trim() != "")
                            //        {
                            //            if (dt2.Rows.Count > 0)
                            //            {
                            //                if (dt2.Rows[0].ItemArray[0].ToString().Trim() != "")
                            //                {
                            //                    if (Convert.ToDouble(dtz.Rows[0].ItemArray[0].ToString()) == Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString()))
                            //                    {
                            //                        bool isreturn = true;
                            //                        SqlCommand myCommand4 = new SqlCommand("update tblSupplierInvoices set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //                        //myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                            //                        myCommand4.ExecuteNonQuery();
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                            //{
                            //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                            //        " TranType='Sup-Return',Status='OutOfStock' " +
                            //        " where ItemID='" +
                            //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                            //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                            //    myCommandSe1.ExecuteNonQuery();
                            //}

                            //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                            //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbLocation.Text.ToString(), txtGRn_NO.Text.ToString().Trim(), dtpReturnDate.Value, false, "");

                            //// CreateXmlToExportGRNAdjust(myTrans, myConnection);
                            //CreateSupplierReturnJXML(myTrans, myConnection);
                            //// CreateSupplierReturnJXML(SqlTransaction tr, SqlConnection con)
                            //// CreatePurchaseJXML(myTrans, myConnection);
                            ////exportSupplierInvoice();
                            //Connector conn = new Connector();
                            //conn.ImportSupplierReturnApply();//Export to supply return
                            //                                 //conn.InventoryAdjustmentExport();//Export to adjustment 

                            //SqlCommand myCommandDe = new SqlCommand("delete tblSerialSupReturnTemp", myConnection, myTrans);
                            //myCommandDe.ExecuteNonQuery();
                            myTrans.Commit();
                            /***********************/
                            MessageBox.Show("Supplier Return Note Successfuly Saved", "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            btnSave.Enabled = false;
                            btnProcess.Enabled = true;
                            btnNew.Enabled = true;
                           
                            //  Disable();
                            //btnSave.Enabled = false;
                            //btnNew.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            myTrans.Rollback();
                            objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                        }
                        finally
                        {
                            myConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Numeric Convertion Error");
                    //btnSave.Focus();
                }
            }

            else
            {
                if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_Warehouse(cmbLocation.Text, sMsg)) return;

                if (IsGridValidation() == false)
                {
                    return;
                }
                string TranType = "Sup-Return";
                int DocType = 7;
                bool QtyIN = false;
                dgvSupplierReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);

                if (cmbVendorSelect.Value == null)
                {
                    MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cmbAR.Value == null)
                {
                    MessageBox.Show("Invalid AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (!IsValidQuantity())
                    return;

                if (!IsSerialNoCorrect())
                    return;

                //bool IsminusAllow = false;

                int rowCount = GetFilledRows();
                int rows = GetFilledRows();
                bool MynusValue = false;

                bool IsItemSerial = false;

                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                SelectSONums1 = SupInv;

                if (SelectSONums1 == "" || SelectSONums1 == null)
                {
                    SelectSONums1 = chkSupplierInvoices.SelectedItem.ToString();
                }
                if (StrReference == "" || StrReference == null)
                {
                    StrReference = txtGRn_NO.Text.ToString();
                }
                string strsql2 = "delete from tblSupplierReturn where SupReturnNo='" + StrReference + "'";
                SqlCommand command2 = new SqlCommand(strsql2, myConnection, myTrans);
                command2.CommandType = CommandType.Text;
                command2.ExecuteNonQuery();

                //check wether this item is serialized or not=======================

                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
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
                        IsItemSerial = true;
                        String S1 = "Select SerialNO from tblSerialSupReturnTemp where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataSet dt1 = new DataSet();
                        da1.Fill(dt1);
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value) == dt1.Tables[0].Rows.Count)
                        {
                            IsItemSerial = false;
                        }
                    }
                }

                int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

                Tax1Amount = double.Parse(txtNBT.Text.Trim());
                Tax2Amount = double.Parse(txtVAT.Text.Trim());

                try
                {
                    //Connector objConnector = new Connector();
                    //if (!(objConnector.IsOpenPeachtree(dtpReturnDate.Value)))
                    //    return;

                    for (int b = 0; b < rows; b++)
                    {
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[2].Value))
                        {
                            //IsMinusAllow = true;
                        }
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) < 0)
                        {
                            ////MynusValue = true;
                        }
                    }

                    //check the qty is whether a number or not
                    for (int a = 0; a < rows; a++)
                    {
                        ChechDQty = 0;
                        checkRetrn = 0;
                        if ((dgvSupplierReturn.Rows[a].Cells[2].Value == null) || (dgvSupplierReturn.Rows[a].Cells[2].Value.ToString() == ""))
                        {
                            dgvSupplierReturn.Rows[a].Cells[2].Value = 0;
                        }
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value);
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);
                        Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value);
                        if (txtTax1Amount.Text == "")
                        {
                            txtTax1Amount.Text = "0";
                        }
                        Convert.ToDouble(txtTax1Amount.Text);
                        if (txtTax2.Text == "")
                        {
                            txtTax2.Text = "0";
                        }
                        Convert.ToDouble(txtTax2.Text);
                        Convert.ToDouble(txtDisRate.Text);
                        if (txtDiscountAmount.Text == "")
                        {
                            txtDiscountAmount.Text = "0";
                        }
                        if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[2].Value) == 0)
                        {
                            checkRetrn = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChechDQty = 1;//if this flag is 1 the violate the number format
                    objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                }

                if (ChechDQty == 0)
                {
                    string SupplierReturnNo = "";

                    //==============form level validations
                    if (txtTotalAmount.Text == "" || Convert.ToDouble(txtTotalAmount.Text) == 0 || cmbLocation.Text == "" ||
                        checkRetrn == 1 || MynusValue == true || rows == 0)
                    {
                        //if (user.IsMinusAllow == false)
                        //{
                        //    MessageBox.Show("You can not Return more than Invoice Qty");
                        //    //IsMinusAllow = false;
                        //    return;
                        //    //btnSave.Focus();
                        //}
                        //if (IsItemSerial == true)
                        //{
                        //    MessageBox.Show("You have not entered serial numbers for this items");
                        //    return;
                        //    //btnSave.Focus();
                        //}
                        if (MynusValue == true)
                        {
                            MessageBox.Show("Quantity Field must be Positive Value");
                            return;
                            //btnSave.Focus();
                        }
                        if (rows == 0)
                        {
                            MessageBox.Show("Select an invoice");
                            return;
                            //btnSave.Focus();
                        }
                        if (Convert.ToDouble(txtTotalAmount.Text) == 0)
                        {
                            MessageBox.Show("Enter Return Quantiy");
                            return;
                            //btnSave.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Fill All Details");
                            return;
                            //btnSave.Focus();
                        }
                    }
                    else
                    {
                        DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
                        string Dformat = "MM/dd/yyyy";
                        string GRNDate = DTP.ToString(Dformat);

                       
                        try
                        {
                            SqlCommand myCommand = null;
                            if (user.IsSRTNNoAutoGen)
                            {
                                //myCommand = new SqlCommand("UPDATE tblDefualtSetting SET SupplierReturnNo = SupplierReturnNo + 1 select SupplierReturnNo, SupReturnPrefix from tblDefualtSetting", myConnection, myTrans);
                                //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                //DataTable dt41 = new DataTable();
                                //da41.Fill(dt41);

                                //if (dt41.Rows.Count > 0)
                                //{
                                //    SupplierReturnNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                                //    SupplierReturnNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + SupplierReturnNo;
                                //}
                                //txtGRn_NO.Text = SupplierReturnNo;

                                //StrReference = GetInvNoField(myConnection, myTrans);
                                //UpdatePrefixNo(myConnection, myTrans);
                                //txtGRn_NO.Text = StrReference;
                                // txtCreditNo.Text = StrReference;
                            }
                            else
                            {
                                myCommand = new SqlCommand("select * from tblSupplierReturn where SupReturnNo='" + txtGRn_NO.Text.Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                DataTable dt41 = new DataTable();
                                da41.Fill(dt41);

                                if (dt41.Rows.Count > 0)
                                {
                                    MessageBox.Show("Supplier Return No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    myTrans.Rollback();
                                    myConnection.Close();//
                                    return;
                                }
                            }
                            // int rows = GetFilledRows();
                            for (int i = 0; i < rows; i++)
                            {
                                // bool IsfullShipment = false;
                                bool Isinvoice = false;
                                // string TranType = "SupReturn";

                                if (Convert.ToDouble(dgvSupplierReturn[2, i].Value) != 0)
                                {
                                    // bool Duplicate = true;
                                    string NoOfDis = Convert.ToString(rows);

                                    bool isRefereSI = true;

                                    if (chkReferingSI.CheckState == CheckState.Checked)
                                    {
                                        isRefereSI = true;
                                    }
                                    else
                                    {
                                        isRefereSI = false;
                                    }
                                 

                                    string ToLocation = "SupReurn";
                                    if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                    {
                                        //SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAPAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[3, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "', '" + isRefereSI + "')", myConnection, myTrans);
                                        SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal, " +
                                            " TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI,Tax3Name,Tax3Amount,IsActive,Discount,DiscountAmount) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[1, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtSpecialDiscount.Text) + "','" + Convert.ToDouble(textBox2.Text) + "','" +
                                            Tax1Name + "','" +
                                            Tax2Name + "','" + Tax1Amount + "','" + Tax2Amount +
                                            "','" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() +
                                            "', '" + isRefereSI + "','" + Tax3Name + "','" +
                                            Tax3Amount + "','" + true + "','" + Convert.ToDouble(txtCashDiscount.Text) + "','" + Convert.ToDouble(txtDiscountAmount1.Text) + "')", myConnection, myTrans);
                                        myCommand2.ExecuteNonQuery();
                                    }


                                    //SqlCommand myCommand51 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + " where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                    //SqlDataAdapter da1 = new SqlDataAdapter(myCommand51);
                                    //DataTable dt1 = new DataTable();
                                    //da1.Fill(dt1);

                                    //double ss = 0.00;

                                    //if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                    //{
                                    //    SqlCommand cmd11 = new SqlCommand(
                                    //        "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbLocation.Text.ToString().Trim() + "' AND ItemId='" + dgvSupplierReturn[0, i].Value + "') " +
                                    //        "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtGRn_NO.Text.ToString().Trim() + "','" + GRNDate + "','" + TranType + "','" + QtyIN + "','" + dgvSupplierReturn[0, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) * Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + cmbLocation.Text.ToString().Trim() + "','" + ss + "')", myConnection, myTrans);
                                    //    cmd11.ExecuteNonQuery();
                                    //}

                                    ////=============================
                                    //SqlCommand myCommand5 = new SqlCommand("insert into tblInvTransaction(TDate,ItemID,FrmWhseId,ToWhseId,QTY,TransType) values ('" + GRNDate + "','" + dgvSupplierReturn[0, i].Value + "','" + cmbLocation.Text.ToString().Trim() + "','" + ToLocation + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + TranType + "')", myConnection, myTrans);
                                    //myCommand5.ExecuteNonQuery();
                                    //=======================================================================
                                    //===================================================================
                                    //SqlCommand myCommandSe = new SqlCommand("Select * from  tblSerialSupReturnTemp where ItemID='" + dgvSupplierReturn[0, i].Value + "'", myConnection, myTrans);
                                    ////myCommand2.ExecuteNonQuery();
                                    //SqlDataAdapter daSe = new SqlDataAdapter(myCommandSe);
                                    //DataTable dtSe = new DataTable();
                                    //daSe.Fill(dtSe);
                                    //Insert to serial numbers table to serial numbers whic are taken from the serialisetemp table============

                                    //string TranType = "GRN";
                                    bool IsGRNProcess = true;
                                    string Status = "SupReturn";

                                    //for (int j = 0; j < dtSe.Rows.Count; j++)
                                    //{
                                    //    SqlCommand myCommandSe1 = new SqlCommand("insert into tblSerialSupReturn(RTNO,ItemID,Description,SerialNO,TransactionType,IsRTNProcess,WLocation)values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value.ToString() + "','" + dgvSupplierReturn[1, i].Value.ToString() + "','" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "','" + IsGRNProcess + "','" + cmbLocation.Text.ToString().Trim() + "')", myConnection, myTrans);
                                    //    myCommandSe1.ExecuteNonQuery();
                                    //    //  }
                                    //    //=======================Update the grn table of==================================================
                                    //    myCommand.CommandText = "Update tblSerializeItem SET IsInvoice = '" + IsGRNProcess + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WLocation='" + cmbLocation.Text.ToString().Trim() + "'";
                                    //    myCommand.ExecuteNonQuery();

                                    //    myCommand.CommandText = "Update tblSerialItemTransaction SET Status = '" + Status + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "'";
                                    //    myCommand.ExecuteNonQuery();
                                    //    //================================================================================
                                    //}

                                    //==========================================================================================

                                    //SqlCommand myCommand7 = new SqlCommand("update tblSupplierInvoices set RemainQty = RemainQty- '" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                                    ////myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                                    //myCommand7.ExecuteNonQuery();
                                    //==========================================
                                    // SqlCommand myCommand5 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "  Select * from  tblItemWhse where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                    //if (dt1.Rows.Count > 0)
                                    //{ }
                                    //else
                                    //{
                                    //    //SqlCommand myCommand3 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')", myConnection, myTrans);
                                    //    //// myCommand.CommandText = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')";
                                    //    //myCommand3.ExecuteNonQuery();
                                    //}

                                    if (IsItemSerial)
                                    {
                                        //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                                        //{
                                        //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                                        //        " TranType='Sup-Return',Status='OutOfStock' " +
                                        //        " where ItemID='" +
                                        //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                                        //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                                        //    myCommandSe1.ExecuteNonQuery();
                                        //}
                                    }
                                }
                            }

                            /***********************/

                            //for (int i = 0; i < rows; i++)
                            //{
                            //    SqlCommand myCommand6 = new SqlCommand("select sum(ReturnQty) from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //    //string s = "select qty from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "'";
                            //    SqlDataAdapter daz = new SqlDataAdapter(myCommand6);
                            //    DataTable dtz = new DataTable();
                            //    daz.Fill(dtz);

                            //    SqlCommand myCommand7 = new SqlCommand("select sum(Qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //    //  string s2 = "select sum(qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "'";
                            //    SqlDataAdapter da2 = new SqlDataAdapter(myCommand7);
                            //    DataTable dt2 = new DataTable();
                            //    da2.Fill(dt2);

                            //    if (dtz.Rows.Count > 0)
                            //    {
                            //        if (dtz.Rows[0].ItemArray[0].ToString().Trim() != "")
                            //        {
                            //            if (dt2.Rows.Count > 0)
                            //            {
                            //                if (dt2.Rows[0].ItemArray[0].ToString().Trim() != "")
                            //                {
                            //                    if (Convert.ToDouble(dtz.Rows[0].ItemArray[0].ToString()) == Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString()))
                            //                    {
                            //                        bool isreturn = true;
                            //                        SqlCommand myCommand4 = new SqlCommand("update tblSupplierInvoices set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //                        //myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                            //                        myCommand4.ExecuteNonQuery();
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                            //{
                            //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                            //        " TranType='Sup-Return',Status='OutOfStock' " +
                            //        " where ItemID='" +
                            //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                            //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                            //    myCommandSe1.ExecuteNonQuery();
                            //}

                            //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                            //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbLocation.Text.ToString(), txtGRn_NO.Text.ToString().Trim(), dtpReturnDate.Value, false, "");

                            //// CreateXmlToExportGRNAdjust(myTrans, myConnection);
                            //CreateSupplierReturnJXML(myTrans, myConnection);
                            //// CreateSupplierReturnJXML(SqlTransaction tr, SqlConnection con)
                            //// CreatePurchaseJXML(myTrans, myConnection);
                            ////exportSupplierInvoice();
                            //Connector conn = new Connector();
                            //conn.ImportSupplierReturnApply();//Export to supply return
                            //                                 //conn.InventoryAdjustmentExport();//Export to adjustment 

                            //SqlCommand myCommandDe = new SqlCommand("delete tblSerialSupReturnTemp", myConnection, myTrans);
                            //myCommandDe.ExecuteNonQuery();
                            myTrans.Commit();
                            /***********************/
                            MessageBox.Show("Supplier Return Note Successfuly Saved", "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            btnSave.Enabled = false;
                            btnProcess.Enabled = true;
                            btnNew.Enabled = true;
                            btnEditClick = false;
                            //  Disable();
                            //btnSave.Enabled = false;
                            //btnNew.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            myTrans.Rollback();
                            objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                        }
                        finally
                        {
                            myConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Numeric Convertion Error");
                    //btnSave.Focus();
                }
            }
        }

        //Create a XML File for import Purchase Journal=====================
        public void CreatePurchaseJXML(SqlTransaction tr, SqlConnection con)
        {
            //string APAccount = "";
            //string Tax1ID = "";
            //string Tax1Name = "";
            //string Tax1GL = "";
            //string Tax2ID = "";
            //string Tax2Name = "";
            //string Tax2GL = "";
            //string DiscountID = "";
            //string DiscountName = "";
            //string DiscountGL = "";

            string DrAccount = "";
            string CrAccount = "";

            SqlCommand myCommand4 = new SqlCommand("Select SalesInvDrAc,SalesInvCrAc from tblDefualtSetting", con, tr);
            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                //  APAccount = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();

                DrAccount = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                CrAccount = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();


                //Tax1ID = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                //Tax1Name = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
                //Tax1GL = dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim();

                //Tax2ID = dt.Tables[0].Rows[i].ItemArray[5].ToString().Trim();
                //Tax2Name = dt.Tables[0].Rows[i].ItemArray[6].ToString().Trim();
                //Tax2GL = dt.Tables[0].Rows[i].ItemArray[7].ToString().Trim();

                //DiscountID = dt.Tables[0].Rows[i].ItemArray[8].ToString().Trim();
                //DiscountName = dt.Tables[0].Rows[i].ItemArray[9].ToString().Trim();
                //DiscountGL = dt.Tables[0].Rows[i].ItemArray[10].ToString().Trim();
            }

            DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Good_Return_List.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Purchases");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int DistributionNumber = 0;
            int rowCount1 = GetFilledRows();
            int taxCount = GetFilledRowsTax();
            string NoDistributions = Convert.ToString(rowCount1 + taxCount);

            //for (int x = 0; x < dt.Tables[0].Rows.Count; x++)
            //{
            //Writer.WriteStartElement("PAW_Purchase");
            //Writer.WriteAttributeString("xsi:type", "paw:purchase");

            for (int i = 0; i < rowCount1 + taxCount; i++)
            {
                if (i < rowCount1)
                {
                    DistributionNumber = i + (taxCount - 1);
                    //abcdefgh
                    //*********
                    Writer.WriteStartElement("PAW_Purchase");
                    Writer.WriteAttributeString("xsi:type", "paw:purchase");

                    Writer.WriteStartElement("VendorID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());
                    Writer.WriteEndElement();



                    //Writer.WriteStartElement("ID");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString(cmbVendor.Text.ToString().Trim());//Vendor ID should be here = Ptient No
                    //Writer.WriteEndElement();

                    //if (i == 0)
                    //{
                    Writer.WriteStartElement("Invoice_Number");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtGRn_NO.Text.ToString().Trim());
                    Writer.WriteEndElement();
                    //}                       

                    Writer.WriteStartElement("Date");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");  
                    Writer.WriteString(dtpReturnDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("AP_Account");
                    // Writer.WriteStartElement("Accounts_Payable_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType ");
                    Writer.WriteString("TRUE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("InvoiceDistNum");
                    Writer.WriteString(DistributionNumber.ToString());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("CustomerSO");
                    //Writer.WriteString(txtCustomerSO.Text);
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("PurchaseLines");//Sales lies
                    Writer.WriteStartElement("PurchaseLine");

                    //Writer.WriteStartElement("PurchaseLines");
                    //Writer.WriteStartElement("PurchaseLine");

                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                    Writer.WriteString("-" + dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[0].Value.ToString());
                    Writer.WriteEndElement();

                    // Writer.WriteStartElement("Description");
                    // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[3].Value.ToString());
                    // Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                    Writer.WriteString("0.00");
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("GL_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(DrAccount);
                    // Writer.WriteString(cmbAPAccount.Text);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Unit_Price");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[5].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString("-" + dgvSupplierReturn.Rows[i].Cells[7].Value.ToString());
                    Writer.WriteEndElement();
                    //========================================================

                    Writer.WriteStartElement("AppliedToPO");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("PO_Number");
                    // Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                    //Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[2].ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    //**************
                    Writer.WriteEndElement();


                }
                //==================================================================================
                if (i == rowCount1)
                {
                    if (Tax1GLAccount != "")
                    {
                        //**********
                        Writer.WriteStartElement("PAW_Purchase");
                        Writer.WriteAttributeString("xsi:type", "paw:purchase");

                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Invoice_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtGRn_NO.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(dtpReturnDate.Text.ToString().Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(NoDistributions);
                        Writer.WriteEndElement();
                        //DistributionNumber

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(Convert.ToString(DistributionNumber + 1));
                        Writer.WriteEndElement();
                        //Writer.WriteStartElement("CustomerSO");
                        //Writer.WriteString(txtCustomerSO.Text);
                        //Writer.WriteEndElement();


                        Writer.WriteStartElement("CreditMemoType ");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        //Writer.WriteStartElement("Accounts_Payable_Account");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                        Writer.WriteEndElement();//CreditMemoType

                        Writer.WriteStartElement("PurchaseLines");
                        Writer.WriteStartElement("PurchaseLine");


                        Writer.WriteStartElement("Quantity");
                        // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                        Writer.WriteString("0.00");
                        Writer.WriteEndElement();
                        //Writer.WriteStartElement("Quantity");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();


                        //Writer.WriteStartElement("Item_ID");
                        //// Writer.WriteString("NBT001");
                        //Writer.WriteString(Tax1ID);
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Description");
                        ////Writer.WriteString("NBT");
                        //Writer.WriteString(Tax1Name);
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(Tax1GLAccount);
                        Writer.WriteEndElement();


                        //========================================================
                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        //  double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                        // double DiscountAmount = Amount * discountRate;
                        // double ItemAmount = Amount - DiscountAmount;

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Tax1Amount.ToString());//HospitalCharge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();//LINE
                        Writer.WriteEndElement();//LINES

                        //**************
                        Writer.WriteEndElement();
                    }
                }

                if (i == rowCount1 + 1)
                {

                    //===========NBT==============NBT=======================NBT=============================
                    //********************
                    if (Tax2GLAccount != "")
                    {
                        Writer.WriteStartElement("PAW_Purchase");
                        Writer.WriteAttributeString("xsi:type", "paw:purchase");

                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Invoice_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtGRn_NO.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(dtpReturnDate.Text.ToString().Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(NoDistributions);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(Convert.ToString(DistributionNumber + 2));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("CreditMemoType ");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        // Writer.WriteStartElement("Accounts_Payable_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                        Writer.WriteEndElement();//CreditMemoType

                        Writer.WriteStartElement("PurchaseLines");
                        Writer.WriteStartElement("PurchaseLine");

                        Writer.WriteStartElement("Quantity");
                        // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                        Writer.WriteString("0.00");
                        Writer.WriteEndElement();
                        //Writer.WriteStartElement("Quantity");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();


                        //Writer.WriteStartElement("Item_ID");
                        //// Writer.WriteString("VAT001");
                        //Writer.WriteString(Tax2ID);
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Description");
                        //// Writer.WriteString("VAT");
                        //Writer.WriteString(Tax2Name);
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(Tax2GLAccount);
                        Writer.WriteEndElement();

                        //========================================================
                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Tax2Amount.ToString());//tax amount1
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();//LINE
                        Writer.WriteEndElement();//LINES

                        //**********
                        Writer.WriteEndElement();
                    }
                }

                //===================================================

                if (i == rowCount1 + 2)
                {

                    //===========NBT==============NBT=======================NBT=============================
                    //********************
                    if (Tax3GLAccount != "")
                    {
                        Writer.WriteStartElement("PAW_Purchase");
                        Writer.WriteAttributeString("xsi:type", "paw:purchase");

                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Invoice_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtGRn_NO.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(dtpReturnDate.Text.ToString().Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(NoDistributions);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(Convert.ToString(DistributionNumber + 2));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("CreditMemoType ");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        // Writer.WriteStartElement("Accounts_Payable_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                        Writer.WriteEndElement();//CreditMemoType

                        Writer.WriteStartElement("PurchaseLines");
                        Writer.WriteStartElement("PurchaseLine");

                        Writer.WriteStartElement("Quantity");
                        // Writer.WriteString(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString());
                        Writer.WriteString("0.00");
                        Writer.WriteEndElement();
                        //Writer.WriteStartElement("Quantity");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();


                        //Writer.WriteStartElement("Item_ID");
                        //// Writer.WriteString("VAT001");
                        //Writer.WriteString(Tax2ID);
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Description");
                        //// Writer.WriteString("VAT");
                        //Writer.WriteString(Tax2Name);
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(Tax3GLAccount);
                        Writer.WriteEndElement();

                        //========================================================
                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Tax3Amount.ToString());//tax amount1
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();//LINE
                        Writer.WriteEndElement();//LINES

                        //**********
                        Writer.WriteEndElement();
                    }
                }
                //====================================================
            }
            //********************
            Writer.WriteEndElement();//last line
            // }
            Writer.Close();
        }
        /******************************************/

        //=============================================

        public DataSet GetDistictPO(string GRNNO)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet DistinctPO = new DataSet();
            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                String S2 = "Select Distinct(PONO) from tblGRNMPO where GRNNO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(DistinctPO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DistinctPO;
        }

        //==========================================================

        public DataSet GetPOQty(string PONO, string GRNNO)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet POQty = new DataSet();
            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                String S2 = "Select ItemID,Qty,PONO,PODisNo from tblGRNMPO where PONO='" + PONO + "' and GRNNO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(POQty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return POQty;
        }
        //=====================================================

        public DataSet ItemData(string ItemID, string GRNNO)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet ItemData = new DataSet();
            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                String S2 = "Select VendorID,GRNDate,APAccount,ItemID,Description,GlAccount,UnitPrice,Amount,UOM,CustomerSO,DistributionNo,LineDiscountRate,TaxRate,TaxRate1 from tblGRNTran where ItemID='" + ItemID + "' and GRN_NO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(ItemData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ItemData;
        }

        //=======================================================


        DataSet DistnctPO = new DataSet();
        DataSet POQuantity = new DataSet();
        DataSet ItemDetails = new DataSet();

        //Create a XML File for import Purchase Journal=====================
        public void CreatePurchaseJXML()
        {
            string APAccount = "";
            string Tax1ID = "";
            string Tax1Name = "";
            string Tax1GL = "";
            string Tax2ID = "";
            string Tax2Name = "";
            string Tax2GL = "";
            string DiscountID = "";
            string DiscountName = "";
            string DiscountGL = "";

            try
            {
                String S = "Select * from tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    APAccount = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();

                    Tax1ID = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                    Tax1Name = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
                    Tax1GL = dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim();

                    Tax2ID = dt.Tables[0].Rows[i].ItemArray[5].ToString().Trim();
                    Tax2Name = dt.Tables[0].Rows[i].ItemArray[6].ToString().Trim();
                    Tax2GL = dt.Tables[0].Rows[i].ItemArray[7].ToString().Trim();

                    DiscountID = dt.Tables[0].Rows[i].ItemArray[8].ToString().Trim();
                    DiscountName = dt.Tables[0].Rows[i].ItemArray[9].ToString().Trim();
                    DiscountGL = dt.Tables[0].Rows[i].ItemArray[10].ToString().Trim();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\GRNExport.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Purchases");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            DistnctPO = GetDistictPO(txtGRn_NO.Text.ToString().Trim());
            for (int i = 0; i < DistnctPO.Tables[0].Rows.Count; i++)
            {
                int NODistribution = 0;
                string GRnNo = "";

                double LineDisCount = 0;
                double TotAmount = 0;
                double TOT_D = 0;//Total amount-Linediscount

                double Tax1Amount = 0;//TOT_D * taxRate
                double Net_Tax1 = 0;//TOT_D +Net_Tax1

                double Tax2Amount = 0;//Net_Tax1* taxRate1


                Writer.WriteStartElement("PAW_Purchase");
                Writer.WriteAttributeString("xsi:type", "paw:purchase");

                POQuantity = GetPOQty(DistnctPO.Tables[0].Rows[i].ItemArray[0].ToString().Trim(), txtGRn_NO.Text.ToString().Trim());
                for (int j = 0; j < POQuantity.Tables[0].Rows.Count + 3; j++)
                {
                    NODistribution = POQuantity.Tables[0].Rows.Count + 3;
                    if (j < POQuantity.Tables[0].Rows.Count)
                    {
                        ItemDetails = ItemData(POQuantity.Tables[0].Rows[j].ItemArray[0].ToString().Trim(), txtGRn_NO.Text.ToString().Trim());

                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Invoice_Number");
                        // Writer.WriteString(txtGRn_NO.Text.ToString().Trim()  + "-" + POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                        Writer.WriteString(txtGRn_NO.Text.ToString().Trim() + "-" + POQuantity.Tables[0].Rows[j].ItemArray[2].ToString().Trim());
                        Writer.WriteEndElement();

                        GRnNo = txtGRn_NO.Text.ToString().Trim() + "-" + POQuantity.Tables[0].Rows[j].ItemArray[2].ToString().Trim();

                        Writer.WriteStartElement("Date");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("3/15/10");
                        Writer.WriteString(GRNDate);
                        // Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[1].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[2].ToString().Trim());//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        // Writer.WriteString(Convert.ToString(POQuantity.Tables[0].Rows.Count));
                        Writer.WriteString(Convert.ToString(NODistribution));
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Invoice_Distribution");
                        //Writer.WriteString(Convert.ToString(j));
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("PurchaseLines");

                        //Writer.WriteStartElement("PurchaseLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[1].ToString().Trim());//Doctor Charge
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Stocking_Quantity");
                        //Writer.WriteString("5");//Doctor Charge
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[0].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[4].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[5].ToString().Trim());
                        Writer.WriteEndElement();

                        double Qty = 0;
                        double UntiPrice = 0;
                        double disRate = 0;
                        double disAmount = 0;
                        double Amount = 0;

                        double TaxRate = 0;
                        double TaxRate1 = 0;


                        TaxRate = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[12]) / 100;
                        TaxRate1 = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[13]) / 100;
                        // double NetAmount = 0;

                        Qty = Convert.ToDouble(POQuantity.Tables[0].Rows[j].ItemArray[1]);
                        UntiPrice = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[6]);
                        disRate = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[11]) / 100;
                        Amount = (Qty * UntiPrice);
                        disAmount = Amount * disRate;
                        // NetAmount = Amount - disAmount;
                        LineDisCount = LineDisCount + disAmount;
                        TotAmount = TotAmount + Amount;


                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[6].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        // Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[7].ToString().Trim());
                        Writer.WriteString(Amount.ToString("N2"));
                        Writer.WriteEndElement();

                        // Amount = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[7]);
                        //NetAmount = NetAmount + Amount;

                        Writer.WriteStartElement("PO_DistNumber");
                        // Writer.WriteString(Convert.ToString(j + 1)); //prev one
                        Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[3].ToString().Trim());
                        //Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[10].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("PO_Number");
                        // Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                        Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[2].ToString().Trim());
                        Writer.WriteEndElement();


                    }
                    //==================================================================================
                    if (j == POQuantity.Tables[0].Rows.Count)
                    {

                        //============VAT============VAT==================VAT===========================

                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Invoice_Number");
                        Writer.WriteString(GRnNo);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("3/15/10");
                        Writer.WriteString(GRNDate);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("20000-00");//Date 
                        Writer.WriteString(APAccount);//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(Convert.ToString(NODistribution));
                        Writer.WriteEndElement();

                        //Invoice/CM Distribution
                        //Writer.WriteStartElement("Invoice_Distribution");
                        //Writer.WriteString(Convert.ToString(j));
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("VAT001");
                        Writer.WriteString(Tax1ID);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        //Writer.WriteString("VAT");
                        Writer.WriteString(Tax1Name);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(Tax1GL);
                        Writer.WriteEndElement();

                        TOT_D = TotAmount - LineDisCount;//Totalamount-LineDiscount
                        Tax1Amount = TOT_D * TaxRate;//(Totalamount-LineDiscount)*TaxRate
                        Net_Tax1 = TOT_D + Tax1Amount;//TOT_D+Tax1Amount
                        Tax2Amount = Net_Tax1 * TaxRate1;

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Tax1Amount.ToString("N2"));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("PO_DistNumber");
                        Writer.WriteString("0");
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();
                    }

                    if (j == POQuantity.Tables[0].Rows.Count + 1)
                    {

                        //===========NBT==============NBT=======================NBT=============================
                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Invoice_Number");
                        Writer.WriteString(GRnNo);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("3/15/10");
                        Writer.WriteString(GRNDate);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("20000-00");//Date 
                        Writer.WriteString(APAccount);//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(Convert.ToString(NODistribution));
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Invoice_Distribution");
                        //Writer.WriteString(Convert.ToString(j));
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("NBT001");
                        Writer.WriteString(Tax2ID);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        //Writer.WriteString("NBT");
                        Writer.WriteString(Tax2Name);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("40000-MF");
                        Writer.WriteString(Tax2GL);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(Tax2Amount.ToString("N2"));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("PO_DistNumber");
                        Writer.WriteString("0");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();
                    }
                    if (j == POQuantity.Tables[0].Rows.Count + 2)
                    {

                        //=========DISCOUNT=========DISCOUNT==================DISCOUNT==========================
                        Writer.WriteStartElement("VendorID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Invoice_Number");
                        Writer.WriteString(GRnNo);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Date");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString("3/15/10");
                        Writer.WriteString(GRNDate);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AP_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("20000-00");//Date 
                        Writer.WriteString(APAccount);//Date 
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString(Convert.ToString(NODistribution));
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Invoice_Distribution");
                        //Writer.WriteString(Convert.ToString(j));
                        //Writer.WriteEndElement();


                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("DISCOUNT001");
                        Writer.WriteString(DiscountID);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        //Writer.WriteString("Discount");
                        Writer.WriteString(DiscountName);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        // Writer.WriteString("23300-00");
                        Writer.WriteString(DiscountGL);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + LineDisCount.ToString("N2"));
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("PO_DistNumber");
                        Writer.WriteString("0");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToPO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();
                    }

                    //======================================================================================
                    // }


                    //===================================================
                }
                Writer.WriteEndElement();//last line

            }
            //  Writer.WriteEndElement();//last line
            Writer.Close();

            // Writer.WriteStartElement("PAW_Purchase");
            // Writer.WriteAttributeString("xsi:type", "paw:purchase");

            // Writer.WriteStartElement("VendorID");
            // Writer.WriteAttributeString("xsi:type", "paw:id");
            // Writer.WriteString("DOWNEY");//Customer ID should be here = Ptient No
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Invoice_Number");
            // Writer.WriteString("A0001");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Date");
            // Writer.WriteAttributeString("xsi:type", "paw:id");
            // // Writer.WriteString("1/7/2007");//Date 
            // Writer.WriteString("03/15/2010");//Date 
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("AP_Account");
            // Writer.WriteAttributeString("xsi:type", "paw:id");
            // Writer.WriteString("20000-00");//Date 
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Number_of_Distributions");
            // Writer.WriteString("1");
            // Writer.WriteEndElement();

            // //Writer.WriteStartElement("PurchaseLines");

            // //Writer.WriteStartElement("PurchaseLine");

            // Writer.WriteStartElement("Quantity");
            // Writer.WriteString("5");//Doctor Charge
            // Writer.WriteEndElement();

            // //Writer.WriteStartElement("Stocking_Quantity");
            // //Writer.WriteString("5");//Doctor Charge
            // //Writer.WriteEndElement();

            // Writer.WriteStartElement("Item_ID");
            // Writer.WriteString("EQFF-13120");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Description");
            // Writer.WriteString("Catalog # F00600:  Hand Spayer/Mister");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("GL_Account");
            // Writer.WriteAttributeString("xsi:type", "paw:id");
            // Writer.WriteString("12150-00");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Unit_Price");
            // Writer.WriteString("3.95");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("Amount");
            // Writer.WriteString("19.75");
            // Writer.WriteEndElement();

            //// Writer.WriteStartElement("U/M_No_of_Stocking_Units");
            ////// Writer.WriteAttributeString("xsi:type", "paw:id");
            //// Writer.WriteString("1");
            //// Writer.WriteEndElement();

            // Writer.WriteStartElement("UM_ID");
            // Writer.WriteAttributeString("xsi:type", "paw:id");
            // Writer.WriteString("Each");
            // Writer.WriteEndElement();

            //// U/M No. of Stocking Units

            // Writer.WriteStartElement("Applied_To_PO");
            // Writer.WriteString("TRUE");
            // Writer.WriteEndElement();

            // Writer.WriteStartElement("PO_Number");
            // Writer.WriteString("PO_10001");
            // Writer.WriteEndElement();

            // //Writer.WriteEndElement();//LINE
            // //Writer.WriteEndElement();//LINES

            // Writer.WriteEndElement();//last line

            // Writer.WriteEndElement();//last line
            // Writer.Close();

        }

        //==================================================================


        //get order qtry fro so temp table=======================

        public Double GetOrdQTY1(string poid, string ItemID, double UPrice)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "'";
                //  string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrdQty = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return OrdQty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==============================================================

        //==============================================================
        public Double GetPrevOrdQTY(string poid, string ItemID, double UPrice)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                //string sql = "select RemainQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrdQty = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return OrdQty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================================================================
        //======================================================

        public bool GetIem(string poid, string ItemID)
        {
            try
            {
                string POid = poid.Trim();
                bool IsAvalble = false;
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                //string sql = "select ItemID from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID  + "'";
                string sql = "select ItemId from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "'";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd2 = new SqlCommand(sql);
                SqlDataAdapter da2 = new SqlDataAdapter(sql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    IsAvalble = true;
                }
                return IsAvalble;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=========================================================

        public Double GetPrevOrginalQty(string poid, string ItemID, double UPrice)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double DispatchQ = 0;
                string ConnString = ConnectionString;
                //string sql = "select DispatchQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                string sql = "select GRNQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and Amount=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DispatchQ = reader.GetDouble(0);
                }
                reader.Close();
                Conn.Close();
                return DispatchQ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=================================================================

        public void SetDispatchFOR_Partial(string SOID, string ItemID, bool FullShip, double DispatchQty)
        {
            try
            {
                //============================
                // bool fullshipment = false;
                bool fullshipment = FullShip;
                double OrginalQty = 0;
                double UpdateDispatchQty = 0;
                double RemainQty = 0;
                bool updatefull = false;

                //String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "';";
                String S = "Select Quantity,GRNQty,RemainQty from tblPurchaseOrder where PONumber = '" + SOID + "' and ItemId='" + ItemID + "';";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int k = 0; k < dt1.Rows.Count; k++)
                    {
                        OrginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                        UpdateDispatchQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                        RemainQty = Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                    }
                }

                UpdateDispatchQty = UpdateDispatchQty + DispatchQty;
                RemainQty = OrginalQty - UpdateDispatchQty;
                if (RemainQty <= 0)
                {
                    // UpdateDispatchQty
                    RemainQty = 0;
                    UpdateDispatchQty = OrginalQty;
                    fullshipment = true;

                }
                //=============================================================

                string ConnString = ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnString);
                cmd = Conn.CreateCommand();
                Conn.Open();
                //cmd.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + fullshipment + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "';";
                cmd.CommandText = "UPDATE tblPurchaseOrder SET IsFullGRN = '" + fullshipment + "',GRNQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE PONumber = '" + SOID + "' and ItemId='" + ItemID + "';";
                cmd.ExecuteNonQuery();

                if (DispatchQty == 0)
                { }
                else
                {
                    // cmd.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "'";
                    cmd.CommandText = "update tblGRNMPO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updatefull + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "'";

                    // cmd.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + SOID + "','" + ItemID + "','" + DispatchQty + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                    cmd.ExecuteNonQuery();
                }
                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=========================================================

        public void UpdateSOTemp(string PO_NO_update, string ItemID_update, double ReceivableQty_update, double DispatchQ, double UnitPrice_update)
        {
            try
            {
                bool fullDispath = false;
                double updateQty = 0;
                double OriginalQty = 0;
                double RemainQty = 0;
                bool Ischeck = false;

                if (ReceivableQty_update == 0)
                {
                    fullDispath = true;
                }
                else
                {
                    fullDispath = false;
                }
                //========================================

                //String S = "Select DispatchQty,Quantity from tblSalesOrderTemp where SalesOrderNo = '" + PO_NO_update + "' and ItemID='" + ItemID_update + "';";
                String S = "Select GRNQty,Quantity from tblPurchaseOrder where PONumber = '" + PO_NO_update + "' and ItemId='" + ItemID_update + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int k = 0; k < dt1.Rows.Count; k++)
                    {
                        updateQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                        OriginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                    }
                }


                updateQty = updateQty + DispatchQ;
                RemainQty = OriginalQty - updateQty;

                //==================================================================

                string ConnString = ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd1 = Conn.CreateCommand();
                // cmd1.CommandText = "UPDATE tblSalesOrderTemp SET RemainQty=" + RemainQty + ",DispatchQty='" + updateQty + "',IsfullDispatch='" + fullDispath + "' where SalesOrderNo='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemID='" + ItemID_update + "'";
                cmd1.CommandText = "UPDATE tblPurchaseOrder SET RemainQty=" + RemainQty + ",GRNQty  ='" + updateQty + "',IsFullGRN='" + fullDispath + "' where PONumber='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemId='" + ItemID_update + "'";
                Conn.Open();
                cmd1.ExecuteNonQuery();
                if (DispatchQ == 0)
                { }
                else
                {
                    cmd1.CommandText = "update tblGRNMPO set Qty ='" + DispatchQ + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + Ischeck + "' where PONO='" + PO_NO_update + "' and ItemID='" + ItemID_update + "'";
                    // cmd1.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQ + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + PO_NO_update + "' and ItemID='" + ItemID_update + "'";

                    //cmd1.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + PO_NO_update + "','" + ItemID_update + "','" + DispatchQ + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                    cmd1.ExecuteNonQuery();
                }

                Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=========================================================
        public void UpdateSoTemptbl(ArrayList SOIDs, string ItemID, double dispatchQty, double UPrice)
        {
            //==================================
            bool FullDispatch = true;
            double DispathQty = 0;
            double Extra_qty = 0;
            double Extra_qty1 = 0;
            double ParialDispatchQty = dispatchQty;
            double ParialDispatchQty1 = dispatchQty;
            double DispatchableQty = 0;
            double PrevOrderdqty = 0;//quantity of the pevious line
            double prevOriginalQty = 0;
            double prevqtyTemp = 0;
            double prevqtyTemp1 = 0;
            bool checkItem = false;
            bool ABC = false;
            bool PQR = false;
            for (int i = 0; i < SOIDs.Count; i++)
            {
                //  bool ABC = false;
                try
                {
                    string SOID = SOIDs[i].ToString();
                    string Item_ID = ItemID;
                    double orderdqty = GetOrdQTY1(SOID, ItemID, UPrice);//order quantity mean Actual Remain Qty
                    if (orderdqty != 0)
                    {
                        //========================================================
                        if (orderdqty <= dispatchQty)
                        {
                            //ABC = false;
                            Extra_qty1 = ParialDispatchQty1 - orderdqty;
                            Extra_qty = dispatchQty - orderdqty;
                            dispatchQty = Extra_qty;
                            if (Extra_qty == 0)
                            {
                                if (ABC == false)
                                {
                                    //1
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty1);
                                }
                            }
                            else if (Extra_qty > 0)
                            {
                                ParialDispatchQty = ParialDispatchQty1 - Extra_qty1;

                                if (i == 0)
                                {
                                    prevqtyTemp = Extra_qty1;
                                    //2
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty);
                                    string SOID3 = SOIDs[i].ToString();
                                    checkItem = GetIem(SOID3, ItemID);
                                    if (checkItem == true)
                                    {
                                        // SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty);
                                        ABC = true;

                                    }
                                }
                                else
                                {
                                    string SOID2 = SOIDs[i - 1].ToString();
                                    PrevOrderdqty = GetPrevOrdQTY(SOID2, ItemID, UPrice);
                                    if (PrevOrderdqty == 0)
                                    {
                                        DispatchableQty = dispatchQty;
                                        FullDispatch = false;
                                        //3
                                        SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty);
                                        // SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, DispatchableQty);
                                        // ABC = true;

                                    }
                                }

                            }
                        }
                        //=========================================================
                        else
                        {

                            if (PQR == true)
                            {
                                ABC = true;
                            }
                            else
                            {
                                ABC = false;
                            }
                            //string SOID3 = SOIDs[i].ToString();
                            // checkItem = GetIem(SOID3, ItemID);
                            //if (checkItem == true)
                            //{
                            //    ABC = true;

                            //}
                            //else
                            //{
                            //    ABC = false;
                            //}

                            if (i == 0)
                            {
                                ABC = true;
                                //string SOID3 = SOIDs[i].ToString();
                                //checkItem = GetIem(SOID3, ItemID);
                                //if (checkItem == true)
                                //{

                                //}
                                //else
                                //{
                                //    ABC = false;
                                //}
                                DispathQty = orderdqty - dispatchQty;

                                {
                                    //4
                                    UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice);
                                    PQR = true;
                                }

                            }
                            else
                            {
                                string SOID1 = SOIDs[i - 1].ToString();
                                // string SOID2 = SOIDs[i].ToString();

                                // checkItem = GetIem(SOID2, ItemID);
                                PrevOrderdqty = GetPrevOrdQTY(SOID1, ItemID, UPrice);
                                // if (checkItem == true)
                                //{
                                if (ABC == false)
                                {
                                    if (PrevOrderdqty == 0)
                                    {

                                        DispathQty = orderdqty - prevqtyTemp;
                                        //if (DispathQty == 0)
                                        //{
                                        //    DispathQty = prevqtyTemp;
                                        //    dispatchQty = prevqtyTemp;
                                        //}

                                        //5
                                        UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice);
                                        ABC = true;
                                        PQR = true;

                                    }
                                }
                                // }

                            }

                        }
                    }//if(orderQty==0)
                    //=====================================================
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //========================================
        }

        public void SetFullDispatch(ArrayList SOID, string ItemID, double DispatchQty, bool FullDispatch)
        {
            try
            {
                double OrginalQty = 0;
                double UpdateDispatchQty = 0;
                double RemainQty = 0;
                bool IsCheckFull = false;
                bool updateCheck = true;

                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                myCommand.Connection = myConnection;
                for (int i = 0; i < SOID.Count; i++)
                {
                    String S = "Select Quantity,GRNQty,RemainQty from tblPurchaseOrder where PONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "';";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        for (int k = 0; k < dt1.Rows.Count; k++)
                        {
                            OrginalQty = Convert.ToDouble(dt1.Rows[k].ItemArray[0].ToString());
                            UpdateDispatchQty = Convert.ToDouble(dt1.Rows[k].ItemArray[1].ToString());
                            RemainQty = Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                        }
                    }

                    UpdateDispatchQty = UpdateDispatchQty + DispatchQty;
                    RemainQty = OrginalQty - UpdateDispatchQty;
                    if (RemainQty <= 0)
                    {
                        RemainQty = 0;
                        UpdateDispatchQty = OrginalQty;
                        DispatchQty = UpdateDispatchQty;//update only po qty
                    }
                    //==========================================
                    String S1 = "Select ISFull from tblGRNMPO where PONO = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da1.Fill(dt2);
                    if (dt2.Rows.Count > 0)
                    {
                        for (int k = 0; k < dt2.Rows.Count; k++)
                        {
                            IsCheckFull = Convert.ToBoolean(dt2.Rows[k].ItemArray[0].ToString());
                        }
                    }

                    myCommand.CommandText = "UPDATE tblPurchaseOrder SET IsFullGRN = '" + FullDispatch + "',GRNQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE PONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "';";
                    myCommand.ExecuteNonQuery();

                    if (IsCheckFull == true)
                    {
                        if (DispatchQty == 0)
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + OrginalQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (DispatchQty == 0)
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + OrginalQty + "'-Qty,GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + DispatchQty + "'-Qty,GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                    }
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=============================================================

        public int getNexNo()//get the next dispatch link number
        {
            int NDispachLink = 0;
            try
            {
                String S1 = "Select max(DispachLink) from tblDispatchHeader";// where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                // int CDispachLink = 0;

                if (dt.Rows.Count > 0)
                {
                    NDispachLink = Convert.ToInt32(dt.Rows[0].ItemArray[0]) + 1;
                }
                else
                {
                    NDispachLink = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NDispachLink;
        }

        //======================================================================

        public int getNextDisTrans()//get the next dispatch transaction number link number
        {
            int NDispachTranLink = 0;

            try
            {
                String S1 = "Select max(DispatchTranLink) from tblDispatchTransaction";// where (Consultant = '" + txtConsultant.Text.ToString().Trim() + "') AND (Date = '" + dtpDate.Text.ToString().Trim() + "')";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                // int CDispachLink = 0;

                if (dt.Rows.Count > 0)
                {
                    NDispachTranLink = Convert.ToInt32(dt.Rows[0].ItemArray[0]) + 1;
                }
                else
                {
                    NDispachTranLink = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NDispachTranLink;
        }

        //======================================================================

        public string NextSupplierReturnNo()//get the next dispatch link number
        {
            string NextReturnNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select SupplierReturnNo from tblDefualtSetting";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() != "")
                {
                    // NextReturnNo =ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    //   int p = ds.Tables[0].Rows.Count - 1;
                    NextReturnNo = getNextID(ds.Tables[0].Rows[0].ItemArray[0].ToString());

                    //  txtReceiptNo.Text = NewID;
                }
                else
                {
                    NextReturnNo = "RTN-100000";
                    string ConnString2 = ConnectionString;
                    string sql2 = "update tblDefualtSetting set SupplierReturnNo = '" + NextReturnNo + "'";
                    SqlConnection Conn2 = new SqlConnection(ConnString2);
                    SqlCommand cmd2 = new SqlCommand(sql2);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(sql2, ConnString2);
                    DataSet ds2 = new DataSet();
                    adapter2.Fill(ds2);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NextReturnNo;
        }

        //=====================================GEt next ID===================

        public string getNextID(string s)
        {
            try
            {
                int i = 0;
                string nextID = "";
                while (i < s.Length - 1)
                {
                    if ((Char.IsDigit(s[i]) && Char.IsLetter(s[i + 1])) || (Char.IsLetter(s[i]) && Char.IsDigit(s[i + 1]) || ((s[i] == '-')) || ((s[i] == ' '))))
                    {
                        s = s.Insert(i + 1, "*");
                    }
                    i++;
                }
                bool Islarge = false;
                string[] arr = s.Split('*');
                i = arr.Length - 1;
                for (int no = i; no >= 0; no--)
                {
                    if (arr[i].Length > 19)
                    {
                        Islarge = true;
                    }
                    else
                    {
                        Islarge = false;
                    }
                }
                if (Islarge == false)
                {
                    ///'''''''''''''''''''''''''''''''''
                    while (i >= 0)
                    {
                        try
                        {
                            //if (arr[i].Length<=19)
                            //{
                            long no = long.Parse(arr[i]);
                            i = 0;
                            while (i < arr.Length)
                            {
                                if (arr[i] == no.ToString())
                                {
                                    no++;
                                    arr[i] = no.ToString();
                                }
                                nextID = nextID + arr[i];
                                i++;
                            }
                            return nextID;

                        }
                        catch { }


                        if (i != 0)
                        {
                            i--;
                        }
                    }
                    return s + "1";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearControls()
        {
            try
            {
                //dgvTaxApplicable.Rows.Clear();
                //dgvTaxApplicable.Columns[0].Visible = true;
                //dgvTaxApplicable.Columns[1].Visible = true;
                //dgvTaxApplicable.Columns[2].Visible = true;
                //dgvTaxApplicable.Columns[3].Visible = true;
                //dgvTaxApplicable.Columns[4].Visible = false;
                //dgvTaxApplicable.Columns[5].Visible = false;
                flag = false;
                flglist = 0;
                Enable();
                //dgvTaxApplicable.Rows.Clear();
                LoadtaxDetails();
                cmbVendorSelect.Text = "";

                txtSpecialDiscount.Text = "0.00";
                txtCashDiscount.Text = "0.00";
                textBox2.Text = "0.00";
                txtDiscountAmount1.Text = "0.00";

                txtNBTP.Text = "0.00";
                txtVATP.Text = "0.00";

                // cmbAPAccount.Text = "";
                cmbLocation.Text = "";
                txtlocName.Text = "";
                txtLocAdd1.Text = "";
                txtLocAdd2.Text = "";
                txtCustomerSO.Text = "";
                cmbtaxSys1.Text = "";
                cmbtaxSys2.Text = "";
                txtTax1Amount.Text = "0";
                txtTax2.Text = "0";
                txtDiscountAmount.Text = "0";
                txtDisRate.Text = "0";
                txtNetTotal.Text = "";
                txtSupName.Text = "";
                txtSupCity.Text = "";
                txtSupAdd1.Text = "";
                txtSupAdd2.Text = "";
                txtGRn_NO.Text = "";
                txtTotalAmount.Text = "";
                dgvSupplierReturn.Rows.Clear();
                chkSupplierInvoices.Items.Clear();
                //cmbVendor.Items.Clear();
                cmbVendorSelect.Text = "";
                GetAPAccount();
                btnClose.Enabled = true;
                btnSave.Enabled = true;
                btnList.Enabled = true;
                btnPrint.Enabled = false;
                GetCurrentUserDate();
                txtVAT.Text = "0.00";
                txtNBT.Text = "0.00";
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
                btnEditClick = false;
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                ClearTaxGrid();
                ClearControls();
                if (user.IsSRTNNoAutoGen) txtGRn_NO.ReadOnly = true;
                else txtGRn_NO.ReadOnly = false;
                GetARAccount();
                GetAPAccount();
                GetVendorDataset();
                dgvSupplierReturn.Enabled = true;
                btnProcess.Enabled = false;
                btnEditer.Enabled = false;

                EnableFields();
                //cmbAR.Text = user.ApAccount;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            if (txtGRn_NO.Text.Trim().Length > 0)
            {
                ds.Clear();
                try
                {
                    String S1 = "Select * from tblSupplierReturn WHERE SupReturnNo = '" + txtGRn_NO.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(ds, "DtSupReturn");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    String S5 = "Select VendorID,VendorName from tblVendorMaster where VendorID='" + cmbVendorSelect.Value.ToString() +"'   ";
                    SqlCommand cmd5 = new SqlCommand(S5);
                    SqlConnection con5 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da5 = new SqlDataAdapter(S5, con5);
                    da5.Fill(ds, "DTVendor");

                    frmRepSupReturn prininv = new frmRepSupReturn(this);
                    prininv.Show();
                }
                catch (Exception ex)
                {
                    objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier return and try again");
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                flag = true;
                txtGRn_NO.Text = "";
                if (frmMain.ObjSupReturnlist == null || frmMain.ObjSupReturnlist.IsDisposed)
                {
                    frmMain.ObjSupReturnlist = new frmSupReturnList(1);
                }
                frmMain.ObjSupplierRetern.TopMost = false;
                frmMain.ObjSupReturnlist.ShowDialog();
                frmMain.ObjSupReturnlist.TopMost = true;

                //if (frmMain.ObjSupInvoicelist == null || frmMain.ObjSupInvoicelist.IsDisposed)
                //{
                //    frmMain.ObjSupInvoicelist = new frmSupInvoiceList(1);
                //}
                //frmMain.ObjSupInvoice.TopMost = false;
                //frmMain.ObjSupInvoicelist.ShowDialog();
                //frmMain.ObjSupInvoicelist.TopMost = true;  

                //txtGRn_NO.Text = frmMain.ObjSupReturnlist.RtnSupReturnNo();
                if (txtGRn_NO.Text.Trim().Length > 0)
                {
                    txtGRn_NO.Text = "";
                    flglist = 1;
                    //txtGRn_NO.Text = frmMain.ObjSupReturnlist.RtnSupReturnNo();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        ClassDriiDown ab = new ClassDriiDown();

        private void cmbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                String S = "Select * from tblWhseMaster where WhseId='" + cmbLocation.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    // cmbCustomer.Text = cmbCustomer.Text;
                    txtlocName.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
                    txtLocAdd1.Text = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                    cmbAR.Text = dt.Tables[0].Rows[0]["APAccount"].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //====================Get Tax Rate Information====================
        public double GetTaxRate(string TaxCode)
        {
            double TaxRate = 0.00;
            try
            {
                DataSet _dts = new DataSet();

                String S4 = "select Rate from tblTaxApplicable where Rank='" + TaxCode + "'";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(_dts);

                TaxRate = double.Parse(_dts.Tables[0].Rows[0][0].ToString());
                //string ConnString = ConnectionString;
                //string sql = ;
                //SqlConnection Conn = new SqlConnection(ConnString);
                //SqlCommand cmd = new SqlCommand(sql);
                //cmd.Connection = Conn;
                //Conn.Close(); Conn.Open();
                //SqlDataReader reader = cmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    TaxRate = Convert.ToDouble(reader.GetValue(0));
                //}

                //reader.Close();
                //Conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return TaxRate;
        }
        //====================================================================

        public double TaxRate = 0;

        private void cmbtaxSys1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDouble(txtTotalAmount.Text) != 0)
                {
                    if (txtDiscountAmount.Text == "" || txtDiscountAmount.Text == "0")
                    {
                        TaxRate = GetTaxRate(cmbtaxSys1.Text);
                        TaxRate = TaxRate / 100;
                        double GrossTot = Convert.ToDouble(txtTotalAmount.Text);
                        double TaxAmount = GrossTot * TaxRate;
                        double NetTot = GrossTot + TaxAmount;


                        if (Decimalpoint == 0)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString();
                            txtNetTotal.Text = NetTot.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString("N1");
                            txtNetTotal.Text = NetTot.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString("N2");
                            txtNetTotal.Text = NetTot.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString("N3");
                            txtNetTotal.Text = NetTot.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString("N4");
                            txtNetTotal.Text = NetTot.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            txtTax1Amount.Text = TaxAmount.ToString("N5");
                            txtNetTotal.Text = NetTot.ToString("N5");
                        }
                        //txtTax1Amount.Text = TaxAmount.ToString("N2");
                        //txtNetTotal.Text = NetTot.ToString("N2");
                    }
                    else
                    {
                        TaxRate = GetTaxRate(cmbtaxSys1.Text);
                        TaxRate = TaxRate / 100;
                        double GrossTot = Convert.ToDouble(txtTotalAmount.Text);
                        double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
                        double TaxAmt = (GrossTot - Did_Amt) * TaxRate;
                        double NetTot = (GrossTot - Did_Amt) + TaxAmt;


                        if (Decimalpoint == 0)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString();
                            txtNetTotal.Text = NetTot.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString("N1");
                            txtNetTotal.Text = NetTot.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString("N2");
                            txtNetTotal.Text = NetTot.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString("N3");
                            txtNetTotal.Text = NetTot.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString("N4");
                            txtNetTotal.Text = NetTot.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            txtTax1Amount.Text = TaxAmt.ToString("N5");
                            txtNetTotal.Text = NetTot.ToString("N5");
                        }

                        //txtTax1Amount.Text = TaxAmt.ToString("N2");
                        //txtNetTotal.Text = NetTot.ToString("N2");
                    }
                }
                else
                {
                    // MessageBox.Show("Set the Invoice Values First");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public double TaxRate1 = 0;

        private void cmbtaxSys2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TaxRate1 = GetTaxRate(cmbtaxSys2.Text);
                TaxRate1 = TaxRate1 / 100;

                if (txtTax1Amount.Text == "0" || txtTax1Amount.Text == "" || txtTax1Amount.Text == "0.00")
                {
                    txtTax1Amount.Text = "0";
                }
                if (txtDiscountAmount.Text == "" || txtDiscountAmount.Text == "0" || txtDiscountAmount.Text == "0.00")
                {
                    txtDiscountAmount.Text = "0";
                }
                double DiscountAmt = Convert.ToDouble(txtDiscountAmount.Text);
                if (txtDiscountAmount.Text != "" || txtDiscountAmount.Text != "0")
                {
                    double GrossTot = Convert.ToDouble(txtTotalAmount.Text);

                    double Tax1Amt = Convert.ToDouble(txtTax1Amount.Text);
                    double TaxAmount = ((GrossTot - DiscountAmt) + Tax1Amt) * TaxRate1;
                    double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
                    double NetTot = (GrossTot - Did_Amt) + Tax1Amt;

                    if (Decimalpoint == 0)
                    {
                        txtTax2.Text = TaxAmount.ToString();
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString();
                    }
                    else if (Decimalpoint == 1)
                    {
                        txtTax2.Text = TaxAmount.ToString("N1");
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N1");
                    }
                    else if (Decimalpoint == 2)
                    {
                        txtTax2.Text = TaxAmount.ToString("N2");
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N2");
                    }
                    else if (Decimalpoint == 3)
                    {
                        txtTax2.Text = TaxAmount.ToString("N3");
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N3");
                    }
                    else if (Decimalpoint == 4)
                    {
                        txtTax2.Text = TaxAmount.ToString("N4");
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N4");
                    }
                    else if (Decimalpoint == 5)
                    {
                        txtTax2.Text = TaxAmount.ToString("N5");
                        txtNetTotal.Text = (NetTot + TaxAmount).ToString("N5");
                    }
                    //txtTax2.Text = TaxAmount.ToString("N2");
                    //txtNetTotal.Text = (NetTot + TaxAmount).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDisRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double DisRate = 0.0;
                double Grosstotal = 0.0;
                double DiscountAmount = 0.0;
                double NetTotal = 0.0;

                DisRate = Convert.ToDouble(txtDisRate.Text) / 100;
                Grosstotal = Convert.ToDouble(txtTotalAmount.Text);
                DiscountAmount = Grosstotal * DisRate;

                txtDiscountAmount.Text = DiscountAmount.ToString("N2");
                NetTotal = Grosstotal - DiscountAmount;
                txtNetTotal.Text = NetTotal.ToString("N2");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
            cmbtaxSys1.Enabled = true;
            cmbtaxSys2.Enabled = true;
        }

        private void btnFormSetting_Click(object sender, EventArgs e)
        {
            try
            {
                frmGRNSetting gset = new frmGRNSetting();
                gset.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvSupplierReturn_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            //dgvSupplierReturn_CellEndEdit(sender, e);
            try
            {
                if (flglist == 0)
                {
                    if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
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
                        for (int a = 0; a < rows; a++)
                        {
                            if (dgvSupplierReturn.Rows[a].Cells[3].Value != null && dgvSupplierReturn.Rows[a].Cells[5].Value != null)
                            {
                                dgvSupplierReturn.Rows[0].Cells[6].Style.BackColor = R1;
                                dgvSupplierReturn.Rows[0].Cells[2].Style.BackColor = R1;

                                //if (chkReferingSI.Checked == false)
                                //{
                                //    ReiveQty = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[1].Value);
                                //}
                                //else
                                //{
                                ReiveQty = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value);
                                //}
                                //========================================================================
                                //for (int b = 0; b < dgvPOLIst.Rows.Count - 1; b++)
                                //{
                                //    POQty = Convert.ToDouble(dgvPOLIst.Rows[b].Cells[3].Value);
                                //    if (ReiveQty >= POQty)
                                //    {
                                //        dgvPOLIst.Rows[b].Cells[4].Value = ReiveQty;
                                //    }
                                //}

                                Unitprice = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);
                                if (dgvSupplierReturn.Rows[a].Cells[6].Value != null)
                                {
                                    if (dgvSupplierReturn.Rows[a].Cells[6].Value.ToString().Trim() != "")
                                    {
                                        DiscountRate = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value) / 100;
                                    }
                                }
                                Amount = (ReiveQty * Unitprice);
                                DiscountAmount = Amount * DiscountRate;
                                Amount1 = Amount - DiscountAmount;

                                if (Decimalpoint == 0)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString();
                                }
                                else if (Decimalpoint == 1)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N1");
                                }
                                else if (Decimalpoint == 2)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N2");
                                }
                                else if (Decimalpoint == 3)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N3");
                                }
                                else if (Decimalpoint == 4)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N4");
                                }
                                else if (Decimalpoint == 5)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N5");
                                }
                                //  dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N2");

                                TotalAmount = TotalAmount + Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8

                            }
                        }

                        if (Decimalpoint == 0)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString();
                            txtNetTotal.Text = TotalAmount.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N1");
                            txtNetTotal.Text = TotalAmount.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N2");
                            txtNetTotal.Text = TotalAmount.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N3");
                            txtNetTotal.Text = TotalAmount.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N4");
                            txtNetTotal.Text = TotalAmount.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N5");
                            txtNetTotal.Text = TotalAmount.ToString("N5");
                        }
                        //txtTotalAmount.Text = TotalAmount.ToString("N2");
                        //txtNetTotal.Text = TotalAmount.ToString("N2");
                    }
                }
                //TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ApplyInvocieTaxAmount(DataTable _dtbl)
        {
            try
            {
                TaxCalculation();
                // LoadTaxMethod();
                LoadtaxDetails();
                //ClearTaxGrid();
                //foreach(DataGridViewRow dgvr in dgvTaxApplicable.Rows)
                //{
                if (_dtbl.Rows.Count > 0)
                {
                    if (double.Parse(_dtbl.Rows[0]["Tax1Amount"].ToString()) > 0)
                    {
                        txtNBT.Text = double.Parse(_dtbl.Rows[0]["Tax1Amount"].ToString()).ToString();
                    //    txtNBTP.Text = GetTaxRate("1").ToString("0.000");
                        //dgvTaxApplicable.Rows[0].Cells["Rate"].Value = GetTaxRate("NBT");
                        //dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = true;                    
                    }
                    else
                    {
                        //txtNBTP.Text = "0.000";
                        //txtNBT.Text = "0.00";
                        //dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = false;
                        //dgvTaxApplicable.Rows[0].Cells["Rate"].Value = "0.00";
                    }
                    if (double.Parse(_dtbl.Rows[0]["Tax2Amount"].ToString()) > 0)
                    {
                        //dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = true;
                        //dgvTaxApplicable.Rows[1].Cells["Rate"].Value = GetTaxRate("VAT");
                        txtVAT.Text = double.Parse(_dtbl.Rows[0]["Tax2Amount"].ToString()).ToString();
                       // txtVATP.Text = GetTaxRate("2").ToString("0.000");
                    }
                    else
                    {
                        //txtVAT.Text = "0.00";
                        //txtVATP.Text = "0.000";
                        //dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = false;
                        //dgvTaxApplicable.Rows[1].Cells["Rate"].Value = "0.00";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool unprocess = false;
        private void clstSupInvoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectSONums1 = "";
                string SelectSONums = "";
                dgvSupplierReturn.Rows.Clear();
                int step = 0;
                int i = 0;
                ArraySONO.Clear();

                while (i < chkSupplierInvoices.Items.Count)
                {
                    if (chkSupplierInvoices.GetItemChecked(i) == true)
                    {
                        step++;
                        string[] SOIDs = new string[2];
                        SOIDs = chkSupplierInvoices.Items[i].ToString().Split('*');
                        string So_No = SOIDs[0].ToString();

                        string So_No1 = SOIDs[0].ToString();//saving code

                        ArraySONO.Add(So_No);
                        So_No = "'" + So_No + "'";

                        So_No = So_No;
                        SelectSONums = SelectSONums + So_No;

                        So_No1 = So_No1 + " ";//savins purpose
                        SelectSONums1 = SelectSONums1 + So_No1;//saving purpose

                    }
                    i++;
                }

             

                if (SelectSONums.Length != 0)
                {
                    DataSet ds = new DataSet();
                    ds = ReturnSOList(SelectSONums);
                    string CusPO = ReturnCusPO(SelectSONums);
                    txtCustomerSO.Text = CusPO;

                    if (SelectSONums != "" || SelectSONums != string.Empty)
                    {

                        String S1 = "Select * from tblSupplierReturn where VendorID = '" + cmbVendorSelect.Text + "' and IsActive ='" + true + "' and SupInvoiceNos = '" + chkSupplierInvoices.SelectedItem.ToString()+ "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da2 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);

                  
                        if (dt2.Rows.Count > 0)
                        {
                            MessageBox.Show("There is unprocess Suplier Return available for this vendor and Suplier Invoice, Please process in order to continue!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            unprocess = true;
                            return;
                        }
                        else
                        {
                            unprocess = false;
                        }
                    }

                    for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                    {
                        dgvSupplierReturn.Rows.Add();
                    }

                    double AmountWD = 0.0;
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {

                        dgvSupplierReturn.Rows[j].Cells[0].Value = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();

                        //Validation for quantity

                        if (DecimalpointQuantity == 0)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString();//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0";//ReturnQty
                        }
                        else if (DecimalpointQuantity == 1)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N1");//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0.0";//ReturnQty
                        }
                        else if (DecimalpointQuantity == 2)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N2");//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0.00";//ReturnQty
                        }
                        else if (DecimalpointQuantity == 3)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N3");//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0.000";//ReturnQty
                        }
                        else if (DecimalpointQuantity == 4)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N4");//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0.0000";//ReturnQty
                        }
                        else if (DecimalpointQuantity == 5)
                        {
                            dgvSupplierReturn.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[1]).ToString("N5");//inv Quantity
                            dgvSupplierReturn.Rows[j].Cells[3].Value = "0.00000";//ReturnQty
                        }

                        //dgvSupplierReturn.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim();//inv Quantity
                        //dgvSupplierReturn.Rows[j].Cells[2].Value = "0";//ReturnQty


                        dgvSupplierReturn.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim();

                        string S = "Select UOM from tblItemMaster where ItemID = '" + ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            dgvSupplierReturn.Rows[j].Cells[4].Value = dt.Rows[0].ItemArray[0].ToString().Trim();
                        }

                        //validation for price

                        if (Decimalpoint == 0)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString();//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString();//Discount
                            //dgvSupplierReturn.Rows[j].Cells[7].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString();//Amount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0";
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N1");//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString("N1");//Discount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0.0";
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N2");//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString("N2");//Discount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0.00";
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N3");//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString("N3");//Discount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0.000";
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N4");//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString("N4");//Discount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0.0000";
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvSupplierReturn.Rows[j].Cells[5].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N5");//unit price
                            dgvSupplierReturn.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[10]).ToString("N5");//Discount
                            dgvSupplierReturn.Rows[j].Cells[7].Value = "0.00000";
                        }

                        //dgvSupplierReturn.Rows[j].Cells[5].Value = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();//unit price
                        //dgvSupplierReturn.Rows[j].Cells[6].Value = "0";//Discount
                        //dgvSupplierReturn.Rows[j].Cells[7].Value = ds.Tables[0].Rows[j].ItemArray[6].ToString().Trim();//Amount

                        cmbLocation.Text = ds.Tables[0].Rows[j].ItemArray[7].ToString().Trim();
                        dgvSupplierReturn.Rows[j].Cells[8].Value = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();
                        AmountWD = AmountWD + Convert.ToDouble(dgvSupplierReturn.Rows[j].Cells[7].Value);
                    }

                    txtSpecialDiscount.Text = ds.Tables[0].Rows[0].ItemArray[11].ToString().Trim();
                    txtCashDiscount.Text = ds.Tables[0].Rows[0].ItemArray[12].ToString().Trim();
                    txtNBTP.Text = ds.Tables[0].Rows[0].ItemArray[13].ToString().Trim();
                    txtVATP.Text = ds.Tables[0].Rows[0].ItemArray[14].ToString().Trim();

                    ApplyInvocieTaxAmount(ds.Tables[0]);

                    if (Decimalpoint == 0)
                    {
                        txtTotalAmount.Text = AmountWD.ToString();
                        txtNetTotal.Text = AmountWD.ToString();
                    }
                    else if (Decimalpoint == 1)
                    {
                        txtTotalAmount.Text = AmountWD.ToString("N1");
                        txtNetTotal.Text = AmountWD.ToString("N1");
                    }
                    else if (Decimalpoint == 2)
                    {
                        txtTotalAmount.Text = AmountWD.ToString("N2");
                        txtNetTotal.Text = AmountWD.ToString("N2");
                    }
                    else if (Decimalpoint == 3)
                    {
                        txtTotalAmount.Text = AmountWD.ToString("N3");
                        txtNetTotal.Text = AmountWD.ToString("N3");
                    }
                    else if (Decimalpoint == 4)
                    {
                        txtTotalAmount.Text = AmountWD.ToString("N4");
                        txtNetTotal.Text = AmountWD.ToString("N4");
                    }
                    else if (Decimalpoint == 5)
                    {
                        txtTotalAmount.Text = AmountWD.ToString("N5");
                        txtNetTotal.Text = AmountWD.ToString("N5");
                    }
                    //txtTotalAmount.Text = AmountWD.ToString("N2");
                    //txtNetTotal.Text = AmountWD.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
            finally
            {
                //   chkSupplierInvoices.Enabled = true;
            }
        }

        private void chkReferingSI_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkReferingSI.CheckState == CheckState.Checked)
                {
                    chkSupplierInvoices.Visible = true;
                    lblSupplierInvoices.Visible = true;
                    dgvSupplierReturn.Columns[1].Visible = true;
                    dgvSupplierReturn.Columns[1].Width = 75;
                    dgvSupplierReturn.Columns[0].Width = 100;
                    //lblInvoicedQty.Visible = true;
                }
                else
                {
                    chkSupplierInvoices.Visible = false;
                    lblSupplierInvoices.Visible = false;
                    dgvSupplierReturn.Columns[1].Visible = false;
                    dgvSupplierReturn.Columns[0].Width = 175;
                    // lblInvoicedQty.Visible = false;
                    dgvSupplierReturn.Rows.Clear();
                    for (int i = 0; i < chkSupplierInvoices.Items.Count; i++)
                    {
                        chkSupplierInvoices.SetItemCheckState(i, CheckState.Unchecked);
                    }

                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ClearTaxGrid()
        {
            try
            {
                //dgvTaxApplicable.Rows[0].Cells["Rate"].Value = "0.00";
                //dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = false;
                //dgvTaxApplicable.Rows[0].Cells["Amount"].Value = "0.00";

                //dgvTaxApplicable.Rows[1].Cells["Rate"].Value = "0.00";
                //dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = false;
                //dgvTaxApplicable.Rows[1].Cells["Amount"].Value = "0.00";

                //foreach (DataGridViewRow dgvr in dgvTaxApplicable.Rows)
                //{
                //    Rate
                //        istax
                //    amount
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void chkSupplierInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            

                if (multiselect ==true)
            {
                return;
            }

            try
            {
               
                //ClearTaxGrid();
                txtTotalAmount.Text = "0.00";
                txtNetTotal.Text = "0.00";
                clstSupInvoice_SelectedIndexChanged(sender, e);
                dgvSupplierReturn.Columns[0].ReadOnly = true;
                dgvSupplierReturn.Columns[1].ReadOnly = true;
                dgvSupplierReturn.Columns[2].ReadOnly = true;
                dgvSupplierReturn.Columns[3].ReadOnly = false;
                dgvSupplierReturn.Columns[4].ReadOnly = true;
                dgvSupplierReturn.Columns[5].ReadOnly = false;
                dgvSupplierReturn.Columns[6].ReadOnly = false;
                TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void chkSupplierInvoices_ItemCheck(object sender, ItemCheckEventArgs e)
        {

           
            try
            {
                
              
                


                clistbxSalesOrder_ItemCheck(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        int newVal1 = 0;
        Point defaultLocation1;

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvSupplierReturn.Rows.Count; i++)
                {
                    if (dgvSupplierReturn.Rows[i].Cells[2].Value != null) //change cell value by 1                   
                    {
                        if (dgvSupplierReturn.Rows[i].Cells[2].Value.ToString().Trim() != string.Empty)
                        {
                            if (double.Parse(dgvSupplierReturn.Rows[i].Cells[2].Value.ToString().Trim())>0)
                            {
                                RowCount++;
                            }
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

        public int GetFilledRowsTax()
        {
            try
            {
                int RowCountTax = 0;

                for (int i = 0; i < dgvSupplierReturn.Rows.Count; i++)
                {
                    if (dgvSupplierReturn.Rows[i].Cells[0].Value != null) //change cell value by 1                   
                    {
                        RowCountTax++;
                    }
                }
                return RowCountTax;
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
                //dataTable.Columns.Add("UnitPrice", typeof(String));

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

        public bool ISSAVE = false;//if save this variable ge true
        bool formload = false;
        // bool formload = true;
        int currentRowIndex1;
        int k = 0;

        private int calculateRowsHeightForPO()
        {
            try
            {
                int rowsHeight = 0;

                DataGridViewRow row;
                for (int i = newVal1; i < dgvSupplierReturn.Rows.Count; i++)
                {
                    row = dgvSupplierReturn.Rows[i];

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

        int clickCount = 0;

        //private void dgvSupplierReturn_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ColumnIndex == 3)
        //        {
        //            if (chkReferingSI.CheckState == CheckState.Checked)
        //            {
        //                if (Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value))
        //                {
        //                    MessageBox.Show("You cannot Exceed the Invoice Quantity......!","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        //                    dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value = "0";
        //                }
        //            }
        //            else
        //            {
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                string s = "select QTY from tblItemWhse where ItemID = '" + dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + "'";
        //                SqlDataAdapter da = new SqlDataAdapter(s, con);
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);

        //                if (dt.Rows.Count > 0)
        //                {
        //                    if (Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) < Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value))
        //                    {
        //                        if (IsMinusAllow == false)
        //                        {
        //                            MessageBox.Show("You are not allowed to enter minus qty");
        //                            dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value = "";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + " item is not in this warehouse");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
        //    }
        //}

        private bool IsValidQuantity()
        {
            user.IsReturnOverSupInv = false;
            try
            {
                if (cmbLocation.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Select a Warehouse....!");
                    return false;
                }

                foreach (DataGridViewRow dgvr in dgvSupplierReturn.Rows)
                {
                    if (dgvr.Cells[0].Value != null)
                    {
                        if (!user.IsReturnOverSupInv)
                        {
                            if (Convert.ToDouble(dgvr.Cells[3].Value) > Convert.ToDouble(dgvr.Cells[2].Value))
                            {
                                MessageBox.Show("You cannot Exceed the Invoice Quantity......!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //dgvr.Cells[3].Value = "0";
                                return false;
                            }
                        }
                        SqlConnection con = new SqlConnection(ConnectionString);
                        string s = "select QTY from tblItemWhse where ItemID = '" + dgvr.Cells[0].Value + "' and WhseId='" + cmbLocation.Text.Trim() + "'";
                        SqlDataAdapter da = new SqlDataAdapter(s, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show(dgvr.Cells[0].Value + ": Item is not in Warehouse :" + cmbLocation.Text);
                            return false;
                        }

                        if (!IsWarehouseHaveStock(double.Parse(dt.Rows[0][0].ToString())))
                            return false;

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private void dgvSupplierReturn_CellEnter(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ColumnIndex == 3)
        //        {
        //            if (chkReferingSI.CheckState == CheckState.Checked)
        //            {
        //                if (Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value))
        //                {
        //                    MessageBox.Show("You cannot Exceed the Invoice Quantity......!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[3].Value = "0";
        //                }
        //            }
        //            else
        //            {
        //                SqlConnection con = new SqlConnection(ConnectionString);
        //                string s = "select QTY from tblItemWhse where ItemID = '" + dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + "'";
        //                SqlDataAdapter da = new SqlDataAdapter(s, con);
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);

        //                if (dt.Rows.Count > 0)
        //                {
        //                    if (Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) < Convert.ToDouble(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value))
        //                    {
        //                        if (IsMinusAllow == false)
        //                        {
        //                            MessageBox.Show("You are not allowed to enter minus qty");
        //                            dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[2].Value = "";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // MessageBox.Show(dgvSupplierReturn.Rows[dgvSupplierReturn.CurrentCell.RowIndex].Cells[0].Value + " item is not in this warehouse");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
        //    }
        //}       
        string SupInv;
        private void txtGRn_NO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (flglist == 1)
                {
                    dgvSupplierReturn.Rows.Clear();
                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblSupplierReturn where SupReturnNo='" + txtGRn_NO.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {

                        if (Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString().Trim()) == true)
                        {
                            btnEditer.Enabled = true;
                            btnSave.Enabled = false;
                            btnProcess.Enabled = false;
                        }
                        else
                        {
                            btnEditer.Enabled = false;
                            btnSave.Enabled = false;
                            btnProcess.Enabled = false;

                        }

                        txtSpecialDiscount.Text = double.Parse(dt.Rows[0].ItemArray[33].ToString()).ToString("0.00");
                        textBox2.Text = double.Parse(dt.Rows[0].ItemArray[35].ToString()).ToString("0.00");

                        txtCashDiscount.Text = double.Parse(dt.Rows[0].ItemArray[12].ToString()).ToString("0.00");
                        txtDiscountAmount1.Text = double.Parse(dt.Rows[0].ItemArray[14].ToString()).ToString("0.00");

                        txtNBTP.Text = double.Parse(dt.Rows[0]["TaxRate"].ToString()).ToString("0.00");
                        txtVATP.Text = double.Parse(dt.Rows[0]["TaxRate1"].ToString()).ToString("0.00");
                        txtNBT.Text = double.Parse(dt.Rows[0]["Tax1Amount"].ToString()).ToString("0.00");
                        txtVAT.Text = double.Parse(dt.Rows[0]["Tax2Amount"].ToString()).ToString("0.00");

                     //   txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString();//disacount
                        txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString();//Total amount
                        txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString();//tax2amount
                        txtNetTotal.Text = dt.Rows[0]["NetTotal"].ToString();//Net Total
                        txtTax2.Text = dt.Rows[0]["Tax2Amount1"].ToString();//tax2amount

                        //--------------------------------------------------
                        //for (int k = 0; k < 3; k++)
                        //{
                        //    dgvTaxApplicable.Rows.Add();
                        //    if (k == 0)
                        //    {
                        //        dgvTaxApplicable.Rows[0].Cells[1].Value = dt.Rows[0].ItemArray[36].ToString().Trim();
                        //        dgvTaxApplicable.Rows[0].Cells[3].Value = dt.Rows[0].ItemArray[15].ToString().Trim();
                        //    }

                        //    if (k == 1)
                        //    {
                        //        dgvTaxApplicable.Rows[1].Cells[1].Value = dt.Rows[0].ItemArray[37].ToString().Trim();
                        //        dgvTaxApplicable.Rows[1].Cells[3].Value = dt.Rows[0].ItemArray[16].ToString().Trim();
                        //    }
                        //    if (k == 2)
                        //    {
                        //        dgvTaxApplicable.Rows[2].Cells[1].Value = dt.Rows[0].ItemArray[43].ToString().Trim();
                        //        dgvTaxApplicable.Rows[2].Cells[3].Value = dt.Rows[0].ItemArray[44].ToString().Trim();
                        //    }
                        //    dgvTaxApplicable.Columns[2].Visible = false;
                        //    dgvTaxApplicable.Columns[0].Visible = false;
                        //    // dgvTaxApplicable.Columns[3].Visible = false;
                        //    dgvTaxApplicable.Columns[4].Visible = false;
                        //}

                        //---------------------------------------------------
                        txtGRn_NO.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        cmbVendorSelect.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        string abc = dt.Rows[0].ItemArray[2].ToString().Trim();
                        SupInv = abc;
                        dtpReturnDate.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                        cmbAR.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                     ///   txtDiscountAmount.Text = dt.Rows[0].ItemArray[14].ToString().Trim();
                        txtTax1Amount.Text = dt.Rows[0].ItemArray[38].ToString().Trim();//15

                        txtTax2.Text = dt.Rows[0].ItemArray[39].ToString().Trim();//16
                        double tot = Convert.ToDouble(dt.Rows[0].ItemArray[29].ToString().Trim());
                        txtTotalAmount.Text = tot.ToString("N2");//17
                        txtNetTotal.Text = dt.Rows[0].ItemArray[18].ToString().Trim();
                        txtCustomerSO.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                        cmbLocation.Text = dt.Rows[0].ItemArray[34].ToString().Trim();
                        cmbtaxSys1.Text = dt.Rows[0].ItemArray[36].ToString().Trim();
                        cmbtaxSys2.Text = dt.Rows[0].ItemArray[37].ToString().Trim();
                        if (Convert.ToBoolean(dt.Rows[0].ItemArray[42].ToString().Trim()) == true)
                        {
                            chkReferingSI.Checked = true;
                        }
                        else
                        {
                            chkReferingSI.Checked = false;
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSupplierReturn.Rows.Add();
                            dgvSupplierReturn.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[27].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[24].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[11].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[32].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                            dgvSupplierReturn.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                        }

                        string[] s = dt.Rows[0].ItemArray[2].ToString().Trim().Split(' ');
                        chkSupplierInvoices.Items.Clear();

                        int a = s.Length;
                        for (int x = 0; x < a; x++)
                        {
                            chkSupplierInvoices.Items.Add(s[x].ToString(), CheckState.Checked);
                        }
                    }

                    TaxRateLoad();
                    DisableField();
                    btnPrint.Enabled = true;
                    btnClose.Enabled = true;
                    btnNew.Enabled = true;
                  
                }

                StrSql = " SELECT VendorName,VContact,VAddress1,VAddress2 FROM tblVendorMaster where VendorID = '" + cmbVendorSelect.Text.Trim() + "'";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt2 = new DataTable();
                dAdapt.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    txtSupName.Text = dt2.Rows[0].ItemArray[0].ToString();
                    txtSupAdd1.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtSupAdd2.Text = dt2.Rows[0].ItemArray[2].ToString();
                    txtSupCity.Text = dt2.Rows[0].ItemArray[3].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void DisableField()
        {
            dgvSupplierReturn.Enabled = false;
            cmbVendorSelect.Enabled = false;
            chkSupplierInvoices.Enabled = false;
            txtGRn_NO.Enabled = false;
            dtpReturnDate.Enabled = false;
            cmbLocation.Enabled = false;
            cmbAR.Enabled = false;
            txtCustomerSO.Enabled = false;
            txtNBTP.Enabled = false;
            txtVATP.Enabled = false;
            txtSpecialDiscount.Enabled = false;
            txtCashDiscount.Enabled = false;

        }

        private void TaxCalculation()
        {
            try
            {
                double _SubAmt = 0;

                foreach (DataGridViewRow dgvr in dgvSupplierReturn.Rows)
                {
                    if (dgvr.Cells[0].Value != null && dgvr.Cells[0].Value.ToString().Trim() != string.Empty)
                    {
                        if (dgvr.Cells["colAmount"].Value == null) dgvr.Cells["colAmount"].Value = 0.00;
                        _SubAmt = _SubAmt + double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                    }
                    else if (dgvr.Cells[2].Value != null && dgvr.Cells[2].Value.ToString().Trim() != string.Empty && dgvr.Cells[8].Value != null && dgvr.Cells[8].Value.ToString().Trim() != string.Empty)
                    {
                        if (dgvr.Cells["colAmount"].Value == null) dgvr.Cells["colAmount"].Value = 0.00;
                        _SubAmt = _SubAmt + double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                    }
                }
                txtTotalAmount.Text = _SubAmt.ToString("0.00");

                if (txtNBTP.Text.Trim() == string.Empty) txtNBTP.Text = "0.000";
                if (txtVATP.Text.Trim() == string.Empty) txtVATP.Text = "0.000";

                if (txtDiscountAmount.Text.Trim() == string.Empty) textBox2.Text = "0.00";
                if (txtDiscountAmount1.Text.Trim() == string.Empty) txtDiscountAmount1.Text = "0.00";

                if (txtSpecialDiscount.Text.ToString() != "" && txtSpecialDiscount.Text.ToString() != string.Empty&&_SubAmt>0)
                {
                    textBox2.Text = (_SubAmt * double.Parse(txtSpecialDiscount.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }
                if (txtCashDiscount.Text.ToString() != "" && txtCashDiscount.Text.ToString() != string.Empty && _SubAmt > 0)
                {
                    txtDiscountAmount1.Text = ((_SubAmt - double.Parse(textBox2.Text.Trim())) * double.Parse(txtCashDiscount.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                if (txtNBT.Text.ToString() != "" && txtNBT.Text.ToString() != string.Empty)
                {
                    txtNBT.Text = ((_SubAmt - double.Parse(textBox2.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim())) * double.Parse(txtNBTP.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                if (txtVAT.Text.ToString() != "" && txtVAT.Text.ToString() != string.Empty)
                {
                    txtVAT.Text = ((_SubAmt - double.Parse(textBox2.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim()) + double.Parse(txtNBT.Text.Trim())) * double.Parse(txtVATP.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                try
                {
                    txtNetTotal.Text = (double.Parse(txtNBT.Text.Trim()) + double.Parse(txtVAT.Text.Trim()) + _SubAmt - double.Parse(textBox2.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim())).ToString("N" + "" + Decimalpoint + "");
                }
                catch (Exception ex)
                {
                    txtNetTotal.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //**********************************************************


        private void GrandTotal()
        {
            try
            {
                double dblGrandTot = 0;
                double dblGrocessAmt = 0;
                double dblSubTot = 0;
                double dblVatAmount = 0;
                double dblNbtAmount = 0;
                double dblDiscAmt = 0;
                double dblNetAmount = 0;

                int intGridRow;

                for (intGridRow = 0; intGridRow < dgvSupplierReturn.Rows.Count; intGridRow++)
                {
                    dblSubTot += double.Parse(dgvSupplierReturn.Rows[intGridRow].Cells[5].Value.ToString());
                }
                double dblDiscPer = double.Parse(txtDiscPer.Value.ToString());
                if (double.Parse(txtDiscPer.Value.ToString()) > 0)
                {
                    dblDiscAmt = (dblSubTot * dblDiscPer) / 100;
                }
                else
                {
                    dblDiscAmt = 0;
                }

                dblGrocessAmt = dblSubTot - dblDiscAmt;

                if (double.Parse(txtNBTP.Text.ToString()) > 0)
                {
                    dblNbtAmount = ((dblGrocessAmt * double.Parse(txtNBTP.Text.ToString())) / 100);
                }
                else
                {
                    dblNbtAmount = 0;
                }
                if (double.Parse(txtVATP.Text.ToString()) > 0)
                {
                    dblVatAmount = (((dblGrocessAmt + dblNbtAmount) * double.Parse(txtVATP.Text.ToString())) / 100);
                }
                else
                {
                    dblVatAmount = 0;
                }

                dblNetAmount = dblGrocessAmt + dblNbtAmount + dblVatAmount;

                //txtSubValue.Value = dblSubTot;
                txtDiscAmount2.Text = dblDiscAmt.ToString("0.00");
                txtNetTotal.Text = dblGrocessAmt.ToString("0.00");
                txtNBT.Text = dblNbtAmount.ToString("0.000");
                txtVAT.Text = dblVatAmount.ToString("0.000");
                txtTotalAmount.Text = dblNetAmount.ToString("0.00");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbLocation.Text == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse First");
                    return;
                }

                if (Convert.ToDouble(dgvSupplierReturn.CurrentRow.Cells["colRQty"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + dgvSupplierReturn.CurrentRow.Cells["colRQty"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            dgvSupplierReturn.CurrentRow.Cells["colRQty"].Selected = true;
                        }
                    }
                }
                else
                {
                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Sup-Return", cmbLocation.Text.ToString().Trim(),
                        dgvSupplierReturn.CurrentRow.Cells["ItemID"].Value.ToString(),
                        Convert.ToDouble(dgvSupplierReturn.CurrentRow.Cells["colRQty"].Value.ToString()),
                        txtGRn_NO.Text.Trim(), IsFind, clsSerializeItem.DtsSerialNoList, GetCheckedGRNList(), false,true);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable GetCheckedGRNList()
        {
            DataTable _dtblGRN = new DataTable();
            _dtblGRN.Columns.Add("GRN");

            try
            {
                for (int i = 0; i < chkSupplierInvoices.Items.Count; i++)
                {
                    if (chkSupplierInvoices.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] GRNNo = chkSupplierInvoices.Items[i].ToString().Split(':');

                        DataRow dr = _dtblGRN.NewRow();
                        dr["GRN"] = GRNNo[0];
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

        private void cmbVendorSelect_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        Load_SupInvList(cmbVendorSelect.ActiveRow.Cells[0].Value.ToString());
                        txtSupName.Text = cmbVendorSelect.ActiveRow.Cells[1].Value.ToString();
                        txtSupAdd1.Text = cmbVendorSelect.ActiveRow.Cells[2].Value.ToString();
                        txtSupAdd2.Text = cmbVendorSelect.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmSupplierReturn_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsFind)
                {
                    LoadtaxDetails();
                    GetARAccount();
                    GetAPAccount();
                    GetVendorDataset();
                    TaxValidation();
                    flglist = 0;
                    Disable();

                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    string s1 = "select IsMinusAllow from tblDefualtSetting";
                    SqlDataAdapter da1 = new SqlDataAdapter(s1, con1);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);

                    if (dt1.Rows[0].ItemArray[0].ToString().Trim() != "")
                    {
                        if (Convert.ToBoolean(dt1.Rows[0].ItemArray[0].ToString()) == true)
                        {
                            //IsMinusAllow = true;
                        }
                        else
                        {
                            //IsMinusAllow = false;
                        }
                    }

                    //if flag==true it is a serch option
                    if (flag == true)
                    {
                        chkSupplierInvoices.Items.Clear();
                        // string SupplierReturnNo = ab.GetText2();
                        string SerchText = ab.GetText2();

                        string ConnString = ConnectionString;
                        String S1 = "Select * from tblGRNTran where GRN_NO='" + SerchText + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da2 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da2.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                txtGRn_NO.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                                cmbVendorSelect.Text = dt.Rows[i].ItemArray[1].ToString().Trim();

                                string abc = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dtpReturnDate.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                                cmbAR.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                                txtTotalAmount.Text = dt.Rows[i].ItemArray[15].ToString().Trim();

                                cmbtaxSys1.Text = dt.Rows[i].ItemArray[27].ToString().Trim();
                                cmbtaxSys2.Text = dt.Rows[i].ItemArray[28].ToString().Trim();

                                txtTax1Amount.Text = dt.Rows[i].ItemArray[29].ToString().Trim();
                                txtTax2.Text = dt.Rows[i].ItemArray[30].ToString().Trim();

                                txtDiscountAmount.Text = dt.Rows[i].ItemArray[26].ToString().Trim();
                                txtDisRate.Text = dt.Rows[i].ItemArray[25].ToString().Trim();
                                txtNetTotal.Text = dt.Rows[i].ItemArray[24].ToString().Trim();
                                txtCustomerSO.Text = dt.Rows[i].ItemArray[21].ToString().Trim();
                                cmbLocation.Text = dt.Rows[i].ItemArray[22].ToString().Trim();
                                cmbLocation_SelectedIndexChanged(sender, e);


                                dgvSupplierReturn.Rows.Add();

                                dgvSupplierReturn.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();

                                // DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvSupplierReturn.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                                dgvSupplierReturn.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[12].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[23].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                                dgvSupplierReturn.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[11].ToString().Trim();

                            }
                        }
                        btnPrint.Enabled = true;
                        btnClose.Enabled = true;

                        btnNew.Enabled = true;
                    }
                    else
                    {
                        LoadTaxMethod();
                        GetCurrentUserDate();
                        LoadtaxDetails();
                        flag = false;
                        load_Decimal();
                        WarehouseDataLoad();
                        TaxRateLoad();
                    }
                    ClearControls();
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
            GetChargeItems();
        }

        private void txtSpecialDiscount_TextChanged(object sender, EventArgs e)
        {
            try
            {
               // TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Suplier Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtCashDiscount_TextChanged(object sender, EventArgs e)
        {
            try
            {
              //  TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Suplier Return", ex.Message, sender.ToString(), ex.StackTrace);
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


                string StrSql5 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='2'";

                SqlCommand cmd5 = new SqlCommand(StrSql5);
                SqlDataAdapter da5 = new SqlDataAdapter(StrSql5, ConnectionString);
                DataTable dt5 = new DataTable();
                dt5.Clear();
                da5.Fill(dt5);
                {
                    SpecialDisItemid = dt5.Rows[0].ItemArray[0].ToString();
                    SpecialDisItemdescription = dt5.Rows[0].ItemArray[1].ToString();
                    SpecialDisGLAccount = dt5.Rows[0].ItemArray[2].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void dgvSupplierReturn_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvSupplierReturn.IsCurrentCellDirty)
                {
                    dgvSupplierReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvSupplierReturn_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
          
        }



        private void cmbVendorSelect_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void txtNBTP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Suplier Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVATP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Suplier Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Suplier Return", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dgvSupplierReturn_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvSupplierReturn_CellEndEdit_2(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (flglist == 0)
                {
                    if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
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
                        for (int a = 0; a < rows; a++)
                        {
                            if (dgvSupplierReturn.Rows[a].Cells[3].Value != null && dgvSupplierReturn.Rows[a].Cells[5].Value != null)
                            {
                                dgvSupplierReturn.Rows[0].Cells[6].Style.BackColor = R1;
                                dgvSupplierReturn.Rows[0].Cells[2].Style.BackColor = R1;

                                //if (chkReferingSI.Checked == false)
                                //{
                                //    ReiveQty = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[1].Value);
                                //}
                                //else
                                //{
                                    ReiveQty = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value);// recived qty= qty that we enter
                                //}
                                //========================================================================
                                //for (int b = 0; b < dgvPOLIst.Rows.Count - 1; b++)
                                //{
                                //    POQty = Convert.ToDouble(dgvPOLIst.Rows[b].Cells[3].Value);
                                //    if (ReiveQty >= POQty)
                                //    {
                                //        dgvPOLIst.Rows[b].Cells[4].Value = ReiveQty;
                                //    }
                                //}

                                Unitprice = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);// item unit price

                                if (dgvSupplierReturn.Rows[a].Cells[6].Value != null)
                                {
                                    if (dgvSupplierReturn.Rows[a].Cells[6].Value.ToString().Trim() != "")
                                    {
                                        DiscountRate = Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value) / 100;
                                    }
                                }
                                Amount = (ReiveQty * Unitprice);
                                DiscountAmount = Amount * DiscountRate;
                                Amount1 = Amount - DiscountAmount;

                                if (Decimalpoint == 0)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString();
                                }
                                else if (Decimalpoint == 1)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N1");
                                }
                                else if (Decimalpoint == 2)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N2");
                                }
                                else if (Decimalpoint == 3)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N3");
                                }
                                else if (Decimalpoint == 4)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N4");
                                }
                                else if (Decimalpoint == 5)
                                {
                                    dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N5");
                                }
                                //  dgvSupplierReturn.Rows[a].Cells[7].Value = Amount1.ToString("N2");

                                TotalAmount = TotalAmount + Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8

                            }
                        }

                        if (Decimalpoint == 0)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString();
                            txtNetTotal.Text = TotalAmount.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N1");
                            txtNetTotal.Text = TotalAmount.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N2");
                            txtNetTotal.Text = TotalAmount.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N3");
                            txtNetTotal.Text = TotalAmount.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N4");
                            txtNetTotal.Text = TotalAmount.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            txtTotalAmount.Text = TotalAmount.ToString("N5");
                            txtNetTotal.Text = TotalAmount.ToString("N5");
                        }
                        //txtTotalAmount.Text = TotalAmount.ToString("N2");
                        //txtNetTotal.Text = TotalAmount.ToString("N2");
                    }
                }
                //TaxCalculation();//coment by sanjeewa
                TaxCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_Warehouse(cmbLocation.Text, sMsg)) return;

            if (IsGridValidation() == false)
            {
                return;
            }
            string TranType = "Sup-Return";
            int DocType = 7;
            bool QtyIN = false;
            dgvSupplierReturn.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbAR.Value == null)
            {
                MessageBox.Show("Invalid AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!IsValidQuantity())
                return;

            if (!IsSerialNoCorrect())
                return;

            //bool IsminusAllow = false;

            int rowCount = GetFilledRows();
            int rows = GetFilledRows();
            bool MynusValue = false;

            bool IsItemSerial = false;

            SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();

            SelectSONums1 = SupInv;
            if (SelectSONums1 == "" || SelectSONums1 == null)
            {
                SelectSONums1 = chkSupplierInvoices.SelectedItem.ToString();
            }

            if (StrReference == "" || StrReference == null)
            {
                StrReference = txtGRn_NO.Text.ToString();
            }

            string strsql2 = "delete from tblSupplierReturn where SupReturnNo='" + StrReference + "'";
            SqlCommand command2 = new SqlCommand(strsql2, myConnection, myTrans);
            command2.CommandType = CommandType.Text;
            command2.ExecuteNonQuery();

            //check wether this item is serialized or not=======================

            for (int a = 0; a < rowCount; a++)
            {
                string ItemClass = "";
                String S = "Select * from tblItemMaster where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
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
                    IsItemSerial = true;
                    String S1 = "Select SerialNO from tblSerialSupReturnTemp where ItemID  = '" + dgvSupplierReturn.Rows[a].Cells[0].Value.ToString().Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet dt1 = new DataSet();
                    da1.Fill(dt1);
                    if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value) == dt1.Tables[0].Rows.Count)
                    {
                        IsItemSerial = false;
                    }
                }
            }

            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

            Tax1Amount = double.Parse(txtNBT.Text.Trim());
            Tax2Amount = double.Parse(txtVAT.Text.Trim());

            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpReturnDate.Value)))
                    return;

                for (int b = 0; b < rows; b++)
                {
                    if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) > Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[2].Value))
                    {
                        //IsMinusAllow = true;
                    }
                    if (Convert.ToDouble(dgvSupplierReturn.Rows[b].Cells[3].Value) < 0)
                    {
                        ////MynusValue = true;
                    }
                }

                //check the qty is whether a number or not
                for (int a = 0; a < rows; a++)
                {
                    ChechDQty = 0;
                    checkRetrn = 0;
                    if ((dgvSupplierReturn.Rows[a].Cells[2].Value == null) || (dgvSupplierReturn.Rows[a].Cells[2].Value.ToString() == ""))
                    {
                        dgvSupplierReturn.Rows[a].Cells[2].Value = 0;
                    }
                    Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[3].Value);
                    Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[5].Value);
                    Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[6].Value);
                    Convert.ToDouble(txtNBT.Text);
                    Convert.ToDouble(txtVAT.Text);
                    Convert.ToDouble(txtDisRate.Text);
                    if(txtDiscountAmount.Text =="")
                    {
                        txtDiscountAmount.Text = "0";
                    }
                    Convert.ToDouble(txtDiscountAmount.Text);
                    if (Convert.ToDouble(dgvSupplierReturn.Rows[a].Cells[2].Value) == 0)
                    {
                        checkRetrn = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                ChechDQty = 1;//if this flag is 1 the violate the number format
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            if (ChechDQty == 0)
            {
                string SupplierReturnNo = "";

                //==============form level validations
                if (txtTotalAmount.Text == "" || Convert.ToDouble(txtTotalAmount.Text) == 0 || cmbLocation.Text == "" ||
                    checkRetrn == 1 || MynusValue == true || rows == 0)
                {
                    //if (user.IsMinusAllow == false)
                    //{
                    //    MessageBox.Show("You can not Return more than Invoice Qty");
                    //    //IsMinusAllow = false;
                    //    return;
                    //    //btnSave.Focus();
                    //}
                    //if (IsItemSerial == true)
                    //{
                    //    MessageBox.Show("You have not entered serial numbers for this items");
                    //    return;
                    //    //btnSave.Focus();
                    //}
                    if (MynusValue == true)
                    {
                        MessageBox.Show("Quantity Field must be Positive Value");
                        return;
                        //btnSave.Focus();
                    }
                    if (rows == 0)
                    {
                        MessageBox.Show("Select an invoice");
                        return;
                        //btnSave.Focus();
                    }
                    if (Convert.ToDouble(txtTotalAmount.Text) == 0)
                    {
                        MessageBox.Show("Enter Return Quantiy");
                        return;
                        //btnSave.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Fill All Details");
                        return;
                        //btnSave.Focus();
                    }
                }
                else
                {
                    DateTime DTP = Convert.ToDateTime(dtpReturnDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string GRNDate = DTP.ToString(Dformat);

                   
                    try
                    {
                        SqlCommand myCommand = null;
                        if (user.IsSRTNNoAutoGen)
                        {
                            //myCommand = new SqlCommand("UPDATE tblDefualtSetting SET SupplierReturnNo = SupplierReturnNo + 1 select SupplierReturnNo, SupReturnPrefix from tblDefualtSetting", myConnection, myTrans);
                            //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            //DataTable dt41 = new DataTable();
                            //da41.Fill(dt41);

                            //if (dt41.Rows.Count > 0)
                            //{
                            //    SupplierReturnNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                            //    SupplierReturnNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + SupplierReturnNo;
                            //}
                            //txtGRn_NO.Text = SupplierReturnNo;

                            StrReference = GetInvNoField(myConnection, myTrans);
                            //UpdatePrefixNo(myConnection, myTrans);
                            StrReference = txtGRn_NO.Text;
                            txtGRn_NO.Text = StrReference;

                            // txtCreditNo.Text = StrReference;
                        }
                        else
                        {
                            //myCommand = new SqlCommand("select * from tblSupplierReturn where SupReturnNo='" + txtGRn_NO.Text.Trim() + "'", myConnection, myTrans);
                            //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            //DataTable dt41 = new DataTable();
                            //da41.Fill(dt41);

                            //if (dt41.Rows.Count > 0)
                            //{
                            //    MessageBox.Show("Supplier Return No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    myTrans.Rollback();
                            //    myConnection.Close();//
                            //    return;
                            //}
                        }
                        // int rows = GetFilledRows();
                        for (int i = 0; i < rows; i++)
                        {
                            // bool IsfullShipment = false;
                            bool Isinvoice = false;
                            // string TranType = "SupReturn";

                            if (Convert.ToDouble(dgvSupplierReturn[2, i].Value) != 0)
                            {
                                // bool Duplicate = true;
                                string NoOfDis = Convert.ToString(rows);

                                bool isRefereSI = true;

                                if (chkReferingSI.CheckState == CheckState.Checked)
                                {
                                    isRefereSI = true;
                                }
                                else
                                {
                                    isRefereSI = false;
                                }

                               
                            
                               
                                string ToLocation = "SupReurn";
                                if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                {
                                    //SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAPAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[3, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "', '" + isRefereSI + "')", myConnection, myTrans);
                                    SqlCommand myCommand2 = new SqlCommand("insert into tblSupplierReturn(SupReturnNo,VendorID,SupInvoiceNos,ReturnDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,Qty,ReturnQty,GLAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,ISGRNFinished,CustomerSO,Location,LineDiscountRate,NetTotal, " +
                                        " TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1, IsReferingSI,Tax3Name,Tax3Amount,IsActive,Discount,DiscountAmount) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value + "','" + dgvSupplierReturn[1, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + dgvSupplierReturn[8, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[7, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvSupplierReturn[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtSpecialDiscount.Text) + "','" + Convert.ToDouble(textBox2.Text) + "','" +
                                        Tax1Name + "','" +
                                        Tax2Name + "','" + Tax1Amount + "','" + Tax2Amount +
                                        "','" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() +
                                        "', '" + isRefereSI + "','" + Tax3Name + "','" +
                                        Tax3Amount + "','"+false+"','"+Convert.ToDouble(txtCashDiscount.Text) + "','" + Convert.ToDouble(txtDiscountAmount1.Text) + "')", myConnection, myTrans);
                                    myCommand2.ExecuteNonQuery();
                                }


                                
                                   double  dblAvailableQty = CheckWarehouseItem(dgvSupplierReturn.Rows[i].Cells["ItemID"].Value.ToString(), cmbLocation.Text.Trim(), myConnection, myTrans);

                                    if (double.Parse(dgvSupplierReturn.Rows[i].Cells["colRQty"].Value.ToString()) > dblAvailableQty)
                                    {
                                        MessageBox.Show("Line No :" + i+1.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                                        myTrans.Rollback();
                                        return;
                                    }

                                

                                SqlCommand myCommand51 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + " where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da1 = new SqlDataAdapter(myCommand51);
                                DataTable dt1 = new DataTable();
                                da1.Fill(dt1);

                                double ss = 0.00;

                                if (Convert.ToDouble(dgvSupplierReturn[3, i].Value) != 0)
                                {
                                    SqlCommand cmd11 = new SqlCommand(
                                        "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbLocation.Text.ToString().Trim() + "' AND ItemId='" + dgvSupplierReturn[0, i].Value + "') " +
                                        "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtGRn_NO.Text.ToString().Trim() + "','" + GRNDate + "','" + TranType + "','" + QtyIN + "','" + dgvSupplierReturn[0, i].Value + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) * Convert.ToDouble(dgvSupplierReturn[5, i].Value) + "','" + cmbLocation.Text.ToString().Trim() + "','" + ss + "')", myConnection, myTrans);
                                    cmd11.ExecuteNonQuery();
                                }

                                ////=============================
                                //SqlCommand myCommand5 = new SqlCommand("insert into tblInvTransaction(TDate,ItemID,FrmWhseId,ToWhseId,QTY,TransType) values ('" + GRNDate + "','" + dgvSupplierReturn[0, i].Value + "','" + cmbLocation.Text.ToString().Trim() + "','" + ToLocation + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + TranType + "')", myConnection, myTrans);
                                //myCommand5.ExecuteNonQuery();
                                //=======================================================================
                                //===================================================================
                                //SqlCommand myCommandSe = new SqlCommand("Select * from  tblSerialSupReturnTemp where ItemID='" + dgvSupplierReturn[0, i].Value + "'", myConnection, myTrans);
                                ////myCommand2.ExecuteNonQuery();
                                //SqlDataAdapter daSe = new SqlDataAdapter(myCommandSe);
                                //DataTable dtSe = new DataTable();
                                //daSe.Fill(dtSe);
                                //Insert to serial numbers table to serial numbers whic are taken from the serialisetemp table============

                                //string TranType = "GRN";
                                bool IsGRNProcess = true;
                                string Status = "SupReturn";

                                //for (int j = 0; j < dtSe.Rows.Count; j++)
                                //{
                                //    SqlCommand myCommandSe1 = new SqlCommand("insert into tblSerialSupReturn(RTNO,ItemID,Description,SerialNO,TransactionType,IsRTNProcess,WLocation)values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + dgvSupplierReturn[0, i].Value.ToString() + "','" + dgvSupplierReturn[1, i].Value.ToString() + "','" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "','" + IsGRNProcess + "','" + cmbLocation.Text.ToString().Trim() + "')", myConnection, myTrans);
                                //    myCommandSe1.ExecuteNonQuery();
                                //    //  }
                                //    //=======================Update the grn table of==================================================
                                //    myCommand.CommandText = "Update tblSerializeItem SET IsInvoice = '" + IsGRNProcess + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WLocation='" + cmbLocation.Text.ToString().Trim() + "'";
                                //    myCommand.ExecuteNonQuery();

                                //    myCommand.CommandText = "Update tblSerialItemTransaction SET Status = '" + Status + "' where ItemID = '" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "' and SerialNO='" + dtSe.Rows[j].ItemArray[2].ToString().Trim() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "'";
                                //    myCommand.ExecuteNonQuery();
                                //    //================================================================================
                                //}

                                //==========================================================================================

                               
                                SqlCommand myCommand7 = new SqlCommand("update tblSupplierInvoices set RemainQty = RemainQty- '" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                                //myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                                myCommand7.ExecuteNonQuery();



                                SqlCommand myCommand77 = new SqlCommand("update tblDirectSupplierInvoices set RemainQty = RemainQty- '" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                                //myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
                                myCommand77.ExecuteNonQuery();

                                //==========================================
                                // SqlCommand myCommand5 = new SqlCommand("update tblItemWhse set QTY = QTY - " + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "  Select * from  tblItemWhse where ItemId='" + dgvSupplierReturn[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'", myConnection, myTrans);
                                //if (dt1.Rows.Count > 0)
                                //{ }
                                //else
                                //{
                                //    //SqlCommand myCommand3 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[1, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')", myConnection, myTrans);
                                //    //// myCommand.CommandText = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvSupplierReturn[0, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[3, i].Value) + "','" + Convert.ToDouble(dgvSupplierReturn[2, i].Value) + "','" + Convert.ToString(dgvSupplierReturn[4, i].Value) + "','" + GRNDate + "')";
                                //    //myCommand3.ExecuteNonQuery();
                                //}

                                if (IsItemSerial)
                                {
                                    //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                                    //{
                                    //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                                    //        " TranType='Sup-Return',Status='OutOfStock' " +
                                    //        " where ItemID='" +
                                    //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                                    //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                                    //    myCommandSe1.ExecuteNonQuery();
                                    //}
                                }
                            }
                        }

                        /***********************/

                        for (int i = 0; i < rows; i++)
                        {
                            SqlCommand myCommand6 = new SqlCommand("select sum(ReturnQty) from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //string s = "select qty from tblSupplierReturn where SupInvoiceNos =  '" + SelectSONums1 + "'";
                            SqlDataAdapter daz = new SqlDataAdapter(myCommand6);
                            DataTable dtz = new DataTable();
                            daz.Fill(dtz);

                            SqlCommand myCommand7 = new SqlCommand("select sum(Qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                            //  string s2 = "select sum(qty) from tblSupplierInvoices where SupInvoiceNo =  '" + SelectSONums1 + "'";
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
                                                SqlCommand myCommand4 = new SqlCommand("update tblSupplierInvoices set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "' and ItemID='" + dgvSupplierReturn[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);

                                                //myCommand2.CommandText = "update tblSupplierInvoices with (rowlock) set IsReturn = '" + isreturn + "'  where SupInvoiceNo =  '" + SelectSONums1 + "'";
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
                                " TranType='Sup-Return',Status='OutOfStock' " +
                                " where ItemID='" +
                                dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbLocation.Text.ToString().Trim() + "' and SerialNo='" +
                                dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                            myCommandSe1.ExecuteNonQuery();
                        }

                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbLocation.Text.ToString(), txtGRn_NO.Text.ToString().Trim(), dtpReturnDate.Value, false, "");

                        // CreateXmlToExportGRNAdjust(myTrans, myConnection);
                        CreateSupplierReturnJXML(myTrans, myConnection);
                        // CreateSupplierReturnJXML(SqlTransaction tr, SqlConnection con)
                        // CreatePurchaseJXML(myTrans, myConnection);
                        //exportSupplierInvoice();
                        Connector conn = new Connector();
                        conn.ImportSupplierReturnApply();//Export to supply return
                        //conn.InventoryAdjustmentExport();//Export to adjustment 

                        //SqlCommand myCommandDe = new SqlCommand("delete tblSerialSupReturnTemp", myConnection, myTrans);
                        //myCommandDe.ExecuteNonQuery();
                        myTrans.Commit();
                        /***********************/
                        MessageBox.Show("Supplier Return Note Successfuly Processed", "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnPrint_Click(sender, e);
                        btnNew_Click(sender, e);

                        //  Disable();
                        //btnSave.Enabled = false;
                        //btnNew.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Numeric Convertion Error");
                //btnSave.Focus();
            }
        }

        private double CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse WHERE ItemId='" + StrItemCode + "' AND WhseId='" + StrWarehouseCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return double.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }

        bool btnEditClick = false;
        private void btnEditer_Click(object sender, EventArgs e)
        {
            btnEditClick = true;
            btnProcess.Enabled = true;
            dgvSupplierReturn.Enabled = true;
            flglist = 0;
            EnableFields();
            btnSave.Enabled = true;
        }

        private void EnableFields()
        {
            dgvSupplierReturn.Enabled = true;
            cmbVendorSelect.Enabled = true;
            chkSupplierInvoices.Enabled = true;
            txtGRn_NO.Enabled = true;
            dtpReturnDate.Enabled = true;
            cmbLocation.Enabled = true;
            cmbAR.Enabled = true;
            txtCustomerSO.Enabled = true;
            txtNBTP.Enabled = true;
            txtVATP.Enabled = true;

            txtSpecialDiscount.Enabled = true;
            txtCashDiscount.Enabled = true;
        }
    }
}