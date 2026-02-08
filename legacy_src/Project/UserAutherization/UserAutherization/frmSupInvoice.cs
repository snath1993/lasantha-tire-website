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
using Infragistics.Win.UltraWinGrid;

namespace UserAutherization
{
    public partial class frmSupInvoice : Form
    {
        clsCommon objclsCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        DataTable dtUser = new DataTable();
        bool run = false;
        public static string ConnectionString;
        int flg = 0;
        public DsSupplierInvoice ds = new DsSupplierInvoice();

        public DTInvoiceData DSInvoicing2 = new DTInvoiceData();//

        public static DateTime UserWiseDate = System.DateTime.Now;
        public string StrAP = null;
        public string StrSql;
        string Po_No = "";

        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Warehouse - Module - Supllier Invoice";
        bool IsFind = false;
        public int Printype = 0;
        int flglist = 0; // this is to check whether list is loaded or not
         public  static string LineDisitemid, LineDisitemdescription, LineDisGLAccount, SpecialDisItemid, SpecialDisItemdescription, SpecialDisGLAccount, Cashitemid, cashitemdis, cashGL, NBitemid, NBTitemDis, NBTitemGL, VATitemid, VATitemDis, VATGL,FreeValitemid,FreeValitemDis,FreeValitemGL;

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

        public DSInvoice DSInvoicing = new DSInvoice();

        public frmSupInvoice()
        {
            setConnectionString();
            InitializeComponent();
            cmbVendorSelect.Focus();
            IsFind = false;
        }

        public frmSupInvoice(string SupInvNo)
        {
            setConnectionString();
            InitializeComponent();
            cmbVendorSelect.Focus();
            IsFind = true;
            flglist = 1;
            txtInvoiceNo.Text = SupInvNo;
        }

        //following code segment load the good received notes==============

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

        private void LoadGRN(string VendorID)
        {
            try
            {

                //sanjeewa aded this code segment to refresh data with vendor..........
                dgvSupInvoice.Rows.Clear();
                txtTotalAmount.Text = "0.00";
                txtNetTotal.Text = "0.00";
                txtLocation.Text = "";
                txtCustomerSO.Text = "";

                txtNBT.Text = "0.00";
                txtVAT.Text = "0.00";
                //......................

                clstGRN.Items.Clear();
                bool ISGRNFinished = false;
                String S = "Select distinct(GRN_NO), PONos from tblGRNTran where VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "' and ISGRNFinished='" + ISGRNFinished + "' and GRN_NO not in (select  substring(GRNNos,1,(select min(len(GRNNos)) from tblSupplierInvoices)) from tblSupplierInvoices) and GRN_NO not in (select  substring(GRNNos,(select min(len(GRNNos)) from tblSupplierInvoices)+2,(select max(len(GRNNos)) from tblSupplierInvoices))  from tblSupplierInvoices) and IsActive='" + false+"'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                // cmbSalesOrderNo.Items.Clear();

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    clstGRN.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + " : " + dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=======================      
        ClassDriiDown ab = new ClassDriiDown();
        public bool flag1 = false;
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
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //txtNBTP.Text = dt.Rows[0]["Rate"].ToString();
                    //txtVATP.Text = dt.Rows[1]["Rate"].ToString();

                    //txtNBTP.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                    //txtVATP.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

                    Tax1ID = dt.Rows[0]["TaxID"].ToString();
                    Tax2ID = dt.Rows[1]["TaxID"].ToString();

                    Tax1Name = dt.Rows[0]["TaxName"].ToString();
                    Tax2Name = dt.Rows[1]["TaxName"].ToString();

                    TaxRate = double.Parse(dt.Rows[0]["Rate"].ToString());
                    TaxRate1 = double.Parse(dt.Rows[1]["Rate"].ToString());

                    Tax1GLAccount = user.TaxPayGL1;
                    Tax2GLAccount = user.TaxPayGL2;

                    //dgvTaxApplicable.Rows.Add();

                    //dgvTaxApplicable.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                    //dgvTaxApplicable.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                    //dgvTaxApplicable.Rows[i].Cells[2].Value = false;
                    //dgvTaxApplicable.Rows[i].Cells[3].Value = "0";
                    //dgvTaxApplicable.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                    //dgvTaxApplicable.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[3].ToString().Trim();

                    //DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                    //dgvTaxApplicable.Rows[i].Cells[2].Value = abc.ToShortDateString();
                    //// dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                    //dgvTaxApplicable.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                    //dgvTaxApplicable.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //********************************************Date********************

        public void GetCurrentUserDate()
        {
            try
            {
                dtpDispatchDate.Value = user.LoginDate;
                dtpInvRecivedDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpDispatchDate.Value = UserWiseDate;
                //    //.ToString().Trim();
                //    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //*************************************************************

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
        //***********************************************************************
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

        //==================================================

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
        //===============================================

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

        //public void GetAPAccount()
        //{
        //    try
        //    {


        //        StrSql = "SELECT [APAccount] FROM tblDefualtSetting";



        //        SqlCommand command = new SqlCommand(StrSql);
        //        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);


        //        if (dt.Rows.Count > 0)
        //        {
        //            StrAP = dt.Rows[0].ItemArray[0].ToString().Trim();
        //            cmbAR.Text = StrAP.Trim();
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //}
        private void frmInvoicing_Load(object sender, EventArgs e)
        {
            try
            {
                btnNew_Click(sender, e);
                GetARAccount();//Infragistics
                GetAPAccount();
                GetVendorDataset();
                flglist = 0;
                // Disable();
                btnNew.Enabled = true;
                TaxValidation();
                Enable();
                GetCurrentUserDate();
                LoadTaxMethod();
                LoadtaxDetails();
                load_Decimal();//load the numbe rof deccimal point for field          
                TaxRateLoad();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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



        public ArrayList ArraySONO = new ArrayList();
        public string SelectSONums1 = "";

        private void clistbxSalesOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (DiferentPOValidate == true)
            {
                return;
            }

            String S111 = "Select *  from tblSupplierInvoices where CustomerID ='" + cmbVendorSelect.Text.Trim() + "' and IsActive ='" + true + "'";
            SqlCommand cmd111 = new SqlCommand(S111);
            SqlDataAdapter da111 = new SqlDataAdapter(S111, ConnectionString);
            DataTable dt111 = new DataTable();
            da111.Fill(dt111);

            if (dt111.Rows.Count > 0)
            {
                MessageBox.Show("There is unprocess Suplier Invoice :- " + dt111.Rows[0].ItemArray[1].ToString() + " available for this vendor, Please process in order to continue!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                clstGRN.SetItemChecked(clstGRN.SelectedIndex, false);

                return;

            }

            try
            {
                //txtLocation.Text = "";
                //txtCustomerSO.Text = "";
                if (flg == 0)
                {
                    //cmbAR.Text = "";
                    //txtTotalAmount.Text = "0.00";
                    //SelectSONums1 = "";
                    //txtNetTotal.Text = "0.00";
                    SelectSONums1 = "";
                    if (clstGRN.CheckedItems.Count == 0)
                    {
                        cmbtaxSys1.Text = "";
                        cmbtaxSys2.Text = "";
                        txtTax1Amount.Text = "0.00";
                        txtTax2.Text = "0.00";
                        txtLocation.Text = "";
                        cmbAR.Text = "";
                        txtTotalAmount.Text = "0.00";
                        SelectSONums1 = "";
                        txtNetTotal.Text = "0.00";
                        dgvSupInvoice.Rows.Clear();
                        txtCustomerSO.Text = "";

                    }
                    string SelectPONums = "";
                    //dgvSupInvoice.Rows.Clear();
                    int step = 0;
                    int i = 0;
                    ArraySONO.Clear();

                    string SupINvNo = null;

                    while (i < clstGRN.Items.Count)
                    {
                        if (clstGRN.GetItemChecked(i) == true)
                        {
                            step++;
                            string[] SOIDs = new string[2];
                            SOIDs = clstGRN.Items[i].ToString().Split(':');
                            //SOIDs = clstGRN.Items[i].ToString().Split('*');
                            string So_No = SOIDs[0].ToString().Trim();

                            string So_No1 = SOIDs[0].ToString().Trim();//saving code

                            ArraySONO.Add(So_No);
                            So_No = "'" + So_No + "'";

                            So_No = So_No + ",";
                            SelectPONums = SelectPONums + So_No;

                            So_No1 = So_No1 + " ";//savins purpose
                            SelectSONums1 = SelectSONums1 + So_No1;//saving purpose



                            ////  string GRNNo = GRN_No.Replace("'", "").Trim();
                            //String S12 = "Select CustomerSO from tblGRNTran where GRN_NO='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                            ////  String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                            //SqlCommand cmd12 = new SqlCommand(S12);
                            //SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                            //DataTable dt12 = new DataTable();
                            //da12.Fill(dt12);

                            //if (dt12.Rows.Count > 0)
                            //{
                            //    txtCustomerSO.Text = dt12.Rows[0].ItemArray[0].ToString();
                            //}

                            String S12 = "Select distinct(CustomerSO) from tblGRNTran where PONos='" + clstGRN.Items[i].ToString().Split(':')[1].Trim() + "' and GRN_NO ='" + clstGRN.Items[i].ToString().Split(':')[0].Trim() + "'";
                            //  String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                            SqlCommand cmd12 = new SqlCommand(S12);
                            SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                            DataTable dt12 = new DataTable();
                            da12.Fill(dt12);

                            if (dt12.Rows.Count > 0)
                            {
                                for (int x = 0; x < dt12.Rows.Count; x++)
                                {

                                    if (SupINvNo == null)
                                    {
                                        SupINvNo = dt12.Rows[x].ItemArray[0].ToString();

                                    }
                                    else
                                    {

                                        SupINvNo = SupINvNo + ':' + dt12.Rows[x].ItemArray[0].ToString();

                                    }



                                }

                                txtCustomerSO.Text = SupINvNo.ToString();

                            }



                        }
                        i++;
                    }


                 

                    if (SelectPONums.Length != 0)
                    {
                        SelectPONums = SelectPONums.Substring(0, SelectPONums.Length - 1);
                        ReturnCheckPONew();
                        DataSet ds = new DataSet();
                        // txtCustomerSO.Text = "";
                        ds = ReturnPOList(SelectPONums);
                        // string CusPO = ReturnCusPO(SelectPONums);//get customerSoO From grn
                        // string CheckPO = ReturnCheckPO(SelectPONums);
                        if (txtCheckPONo.Text.Length == 0)
                        {
                            txtCheckPONo.Text = ReturnCheckPO(SelectPONums);//get ponumber from grn
                        }
                        if (txtCustomerSO.Text.Length == 0)
                        {
                            // txtCustomerSO.Text = CusPO;


                            //String S = "Select distinct WareHouseID from tblGRNTran where GRN_NO ='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                            //SqlCommand cmd = new SqlCommand(S);
                            //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            //DataTable dt = new DataTable();
                            //da.Fill(dt);

                            //if (dt.Rows.Count > 0)
                            //{

                            //    if (txtLocation.Text != string.Empty)
                            //    {
                            //        if (txtLocation.Text == dt.Rows[0].ItemArray[0].ToString())
                            //        {
                            //            txtLocation.Text = dt.Rows[0].ItemArray[0].ToString();
                            //        }
                            //        else
                            //        {
                            //            MessageBox.Show("Please Select GRNs belongs to the same Warehouse", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //            clstGRN.SetItemChecked(clstGRN.SelectedIndex, false);
                            //            return;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        txtLocation.Text = dt.Rows[0].ItemArray[0].ToString();
                            //    }
                            //}
                        }

                        //=============================================
                       

                        // cmbAR.Text = "";
                        txtTotalAmount.Text = "0.00";
                        //SelectSONums1 = "";
                        txtNetTotal.Text = "0.00";
                        dgvSupInvoice.Rows.Clear();
                        for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                        {
                            dgvSupInvoice.Rows.Add();
                        }

                        double AmountWD = 0.0;
                        double dblLineAmount = 0.00;

                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            dgvSupInvoice.Rows[j].Cells[0].Value = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();

                            String S1 = "Select UnitCost,Discount,Profit,PriceLevel1 from tblItemMaster where ItemID='" + ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim() + "'";
                            SqlCommand cmd1 = new SqlCommand(S1);
                            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);
                            if (dt1.Rows.Count > 0)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = dt1.Rows[0].ItemArray[3].ToString().Trim();
                              //    dgvSupInvoice.Rows[j].Cells[5].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[2].ToString().Trim());
                           // dgvSupInvoice.Rows[j].Cells[6].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[2].ToString().Trim()).ToString("N2");
                           // dgvSupInvoice.Rows[j].Cells[7].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[2].ToString()).ToString("N2");
                            }

                          
                            //decimal validation
                            if (DecimalpointQuantity == 0)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N2");//Receved Qty
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N2");
                            }
                            else if (DecimalpointQuantity == 1)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N1");
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N1");
                            }
                            else if (DecimalpointQuantity == 2)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N2");
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N2");
                            }
                            else if (DecimalpointQuantity == 3)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N3");
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N3");
                            }
                            else if (DecimalpointQuantity == 4)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N4");
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N4");
                            }
                            else if (DecimalpointQuantity == 5)
                            {
                                dgvSupInvoice.Rows[j].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N5");
                                dgvSupInvoice.Rows[j].Cells[13].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[8]).ToString("N5");
                            }
                            // dgvSupInvoice.Rows[j].Cells[1].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString();
                            // dgvSupInvoice.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim();//Received Qty
                            dgvSupInvoice.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim();
                            dgvSupInvoice.Rows[j].Cells[3].Value = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();
                            //sanjeewa1234
                            dgvSupInvoice.Rows[j].Cells[9].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                            dgvSupInvoice.Rows[j].Cells[10].Value = ds.Tables[0].Rows[j].ItemArray[6].ToString().Trim();

                            if (Decimalpoint == 0)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString();//Unit Price
                             //   dgvSupInvoice.Rows[j].Cells[5].Value = "0";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[6].Value = dblLineAmount.ToString();
                            }
                            else if (Decimalpoint == 1)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString("N1");//Unit Price
                          //      dgvSupInvoice.Rows[j].Cells[5].Value = "0.0";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[8].Value = dblLineAmount.ToString("N1");
                            }
                            else if (Decimalpoint == 2)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString("N2");//Unit Price
                         //       dgvSupInvoice.Rows[j].Cells[5].Value = "0.00";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[8].Value = dblLineAmount.ToString("N2");
                            }
                            else if (Decimalpoint == 3)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString("N3");//Unit Price
                        //        dgvSupInvoice.Rows[j].Cells[5].Value = "0.000";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[8].Value = dblLineAmount.ToString("N3");
                            }
                            else if (Decimalpoint == 4)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString("N4");//Unit Price
                         //       dgvSupInvoice.Rows[j].Cells[5].Value = "0.0000";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[8].Value = dblLineAmount.ToString("N4");
                            }
                            else if (Decimalpoint == 5)
                            {
                                dgvSupInvoice.Rows[j].Cells[4].Value = Convert.ToDouble(dt1.Rows[0].ItemArray[3]).ToString("N5");//Unit Price
                          //      dgvSupInvoice.Rows[j].Cells[5].Value = "0.00000";//Discount
                                dblLineAmount = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]));
                                dgvSupInvoice.Rows[j].Cells[8].Value = dblLineAmount.ToString("N5");
                            }
                            // dgvSupInvoice.Rows[j].Cells[7].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                            dgvSupInvoice.Rows[j].Cells[9].Value = ds.Tables[0].Rows[j]["PODistributionNO"].ToString().Trim();
                            //dgvSupInvoice.Rows[j].Cells[4].Value = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();//Unit Price
                            //dgvSupInvoice.Rows[j].Cells[5].Value = "0";//Discount
                            //dgvSupInvoice.Rows[j].Cells[6].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]);//Amount
                            AmountWD = AmountWD + Convert.ToDouble(dgvSupInvoice.Rows[j].Cells[8].Value);
                        }

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


                    dgvSupInvoice.Columns[0].ReadOnly = true;
                    dgvSupInvoice.Columns[1].ReadOnly = true;
                    dgvSupInvoice.Columns[2].ReadOnly = true;
                    dgvSupInvoice.Columns[3].ReadOnly = true;
                    dgvSupInvoice.Columns[4].ReadOnly = false;
                    dgvSupInvoice.Columns[7].ReadOnly = false;
                    dgvSupInvoice.Columns[8].ReadOnly = true;
                }
                //=============================================
                //dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells[3];
                //dgvSupInvoice_CellValueChanged;
                TaxCalculation();
                //===================================================================
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //==========================
        public DataSet ReturnPOList(string GRN_No)
        {
            DataSet ds = new DataSet();

            try
            {
                ////SELECT ItemID, Description, SUM(ReceiveQty) AS Expr1, GlAccount, UnitPrice, DistributionNo FROM  tblGRNTran WHERE (GRN_NO IN ('GRV-10000051','GRV-10000052'))GROUP BY ItemID, Description, GlAccount, UnitPrice, DistributionNoORDER BY DistributionNo
                //SELECT ItemID, Description, SUM(ReceiveQty) AS Expr1, GlAccount, UnitPrice, DistributionNo FROM  tblGRNTran WHERE (GRN_NO IN ('GRV-10000051','GRV-10000052'))GROUP BY ItemID, Description, GlAccount, UnitPrice, DistributionNoORDER BY DistributionNo
                //string S = "Select ItemID,Description,Sum(ReceiveQty),GlAccount,UnitPrice from tblGRNTran where GRN_NO in (" + GRN_No + ") group by ItemID,Description,GlAccount,UnitPrice";//and IsInvoce='" + Isinvoice + "'
                string S = "SELECT ItemID, Description, SUM(ReceiveQty) AS Expr1,GlAccount, UnitPrice, PODistributionNO,SUM(ConvertedQty) AS ConvertedQty,UOM,SUM(FreeQty) AS FreeQty FROM  tblGRNTran WHERE (GRN_NO IN (" + GRN_No + "))and ReceiveQty != 0 GROUP BY ItemID, Description, GlAccount, UnitPrice, PODistributionNO,UOM ORDER BY PODistributionNO";

                // string S = "SELECT ItemID, Description, SUM(ReceiveQty),GlAccount, UnitPrice, DistributionNo FROM  tblGRNTran WHERE (GRN_NO IN (" + GRN_No + "))GROUP BY ItemID, Description, GlAccount, UnitPrice, DistributionNo";// ORDER BY DistributionNo";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                da.Fill(ds, "PO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public string ReturnCheckPONew()
        {

            try
            {
                String S1 = "Select PONos from tblGRNTran where GRN_NO='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    Po_No = dt1.Rows[0].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Po_No;
        }



        public string ReturnCheckPO(string GRN_No)
        {
            string CheckPO = "";

            try
            {
                string GRNNo = GRN_No.Replace("'", "").Trim();
                // String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                String S1 = "Select PONos from tblGRNTran where GRN_NO='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    CheckPO = dt1.Rows[0].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CheckPO;
        }
        //=====get Customr POs To Invoice
        public string ReturnCusPO(string GRN_No)
        {
            string CusPO = "";

            try
            {
                string GRNNo = GRN_No.Replace("'", "").Trim();
                String S1 = "Select CustomerSO from tblGRNTran where GRN_NO='" + clstGRN.CheckedItems[clstGRN.SelectedIndex].ToString().Split(':')[0].Trim() + "'";
                //  String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    CusPO = dt1.Rows[0].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }
        //====================================
        public string ReturnCusPOCheck(string GRN_No)
        {
            string CusPO = "";

            try
            {
                string GRNNo = GRN_No.Replace("'", "").Trim();
                // String S1 = "Select CustomerSO from tblGRNTran where GRN_NO='" + GRNNo + "'";
                String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    CusPO = dt1.Rows[0].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }
        //=================================================

        public double NAmountDC = 0.0;//Net Amount for discount Calculation
        private void dgvdispactApplytoSales_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
                    if (dgvSupInvoice.Rows[a].Cells[2].Value != null && dgvSupInvoice.Rows[a].Cells[4].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[2].Value);
                        unitprice = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[4].Value);
                        DiscountRate = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[5].Value) / 100;

                        Amount = (DispatchQty * unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        dgvSupInvoice.Rows[a].Cells[6].Value = Amount1.ToString("N4");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                    }
                }
                txtTotalAmount.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
                //=========================================
                // dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells[3];
                TaxCalculation();
                //===================================================================
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDisRate_TextChanged(object sender, EventArgs e)
        {

            if (btnProcessclick == false)
            {
                TaxCalculation();
            }
            //    if (txtDisRate.Text != "0"&& btnEditer.Enabled == false)
            //    {
            //        try
            //        {
            //            double DisRate = 0.0;
            //            double Grosstotal = 0.0;
            //            double DiscountAmount = 0.0;
            //            double NetTotal = 0.0;
            //            if (txtDisRate.Text == string.Empty)
            //            {
            //                DisRate = 0.0;
            //            }
            //            else
            //            {
            //                DisRate = Convert.ToDouble(txtDisRate.Text) / 100;

            //            }
            //            Grosstotal = Convert.ToDouble(txtTotalAmount.Text);
            //            DiscountAmount = Grosstotal * DisRate;

            //            txtDiscountAmount.Text = DiscountAmount.ToString("N2");
            //            if (txtDiscountAmount1.Text.ToString() != "" || txtDiscountAmount1.Text.ToString() != string.Empty)
            //            {
            //                double Dis1 = Convert.ToDouble(txtDiscountAmount1.Text.ToString());
            //                double nbt = Convert.ToDouble(txtNBT.Text.ToString());
            //                double vat = Convert.ToDouble(txtVAT.Text.ToString());
            //                NetTotal = (Grosstotal - DiscountAmount) - Dis1 + nbt + vat;
            //            }
            //            else
            //            {
            //                NetTotal = (Grosstotal - DiscountAmount + Convert.ToDouble(txtNBT.Text.ToString()) + Convert.ToDouble(txtVAT.Text.ToString()));
            //            }
            //            txtNetTotal.Text = NetTotal.ToString("N2");

            //        }
            //        catch (Exception ex)
            //    {

            //    }
            //}
        }

        //===================================get tax rate=================

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

        //============================================================

        public double TaxRate = 0;
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
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
                        txtTax1Amount.Text = TaxAmount.ToString("N2");
                        double NetTot = GrossTot + TaxAmount;
                        txtNetTotal.Text = NetTot.ToString("N2");
                    }
                    else
                    {
                        TaxRate = GetTaxRate(cmbtaxSys1.Text);
                        TaxRate = TaxRate / 100;
                        double GrossTot = Convert.ToDouble(txtTotalAmount.Text);
                        double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
                        double TaxAmt = (GrossTot - Did_Amt) * TaxRate;
                        txtTax1Amount.Text = TaxAmt.ToString("N2");
                        double NetTot = (GrossTot - Did_Amt) + TaxAmt;
                        txtNetTotal.Text = NetTot.ToString("N2");
                    }
                }
                else
                {
                    MessageBox.Show("Set the Invoice Values First", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public double TaxRate1 = 0;
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
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
                    txtTax2.Text = TaxAmount.ToString("N2");
                    double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
                    double NetTot = (GrossTot - Did_Amt) + Tax1Amt;
                    txtNetTotal.Text = (NetTot + TaxAmount).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //==============================================GetInvoiceNo
        public string NextInvoiceNo()//get the next number
        {
            string NextInvoiceNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select SupplierInvoiceNo from tblDefualtSetting";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() != "")
                {
                    NextInvoiceNo = getNextID(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[0].ToString());
                }
                else
                {
                    NextInvoiceNo = "SIN100000";
                    // set the supplier invoice number as SIN100000
                    string ConnString2 = ConnectionString;
                    string sql2 = "update tblDefualtSetting set SupplierInvoiceNo = '" + NextInvoiceNo + "'";
                    SqlConnection Conn2 = new SqlConnection(ConnString2);
                    SqlCommand cmd2 = new SqlCommand(sql2);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(sql2, ConnString2);
                    DataSet ds2 = new DataSet();
                    adapter2.Fill(ds2);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NextInvoiceNo;
        }

        //================================================
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
        //===============================================
        public void UpdateGRNs()
        {
            bool IsGRNFinished = true;
            try
            {
                for (int i = 0; i <= clstGRN.Items.Count; i++)
                {
                    if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                        string S = "select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataTable dt1 = new DataTable();
                        da.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            String S1 = "update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'";
                            SqlCommand cmd1 = new SqlCommand(S1);
                            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                            DataTable dt2 = new DataTable();
                            da1.Fill(dt2);
                        }
                    }
                    //=======================================
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=================================================get filled row count..........
        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvSupInvoice.Rows.Count; i++)
                {
                    if (dgvSupInvoice.Rows[i].Cells[1].Value != null) //change cell value by 1                   
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
        //=============================================



        //making xml for export invoice into peachtree================================

        public void exportSupplierInvoice()
        {
            //Create a Xmal File..................................................................................

            //try
            //{
            //Receipts2.xml
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("paw:purchase");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            //===================================================================================
            string ARAccount = "";
            string Tax1ID = "";
            string Tax1Name = "";
            string Tax1GL = "";
            string Tax2ID = "";
            string Tax2Name = "";
            string Tax2GL = "";
            string DiscountID = "";
            string DiscountName = "";
            string DiscountGL = "";

            //try
            //{
            String S = "Select * from tblDefualtSetting";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                ARAccount = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();

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
            //}
            //catch { }

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            //======================================================================================

            // string NoDistributions = Convert.ToString(dgvdispactApplytoSales.Rows.Count - 1);
            // double discountRate = Convert.ToDouble(txtdisRate.Text) / 100;
            int rowCount1 = GetFilledRows();
            string NoDistributions = Convert.ToString(rowCount1 + 2);

            for (int i = 0; i < rowCount1 + 2; i++)
            {
                if (i < rowCount1)
                {
                    Writer.WriteStartElement("PAW_Purchase");
                    Writer.WriteAttributeString("xsi:type", "paw:purchase");

                    Writer.WriteStartElement("Vendor_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");  
                    Writer.WriteString(dtpDispatchDate.Value.ToString("MM/dd/yyyy"));//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("CustomerSO");
                    //Writer.WriteString(txtCustomerSO.Text);
                    //Writer.WriteEndElement();


                    Writer.WriteStartElement("Accounts_Payable_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType



                    Writer.WriteStartElement("PurchaseLines");
                    Writer.WriteStartElement("PurchaseLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[1].Value.ToString());//Doctor Charge
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[0].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[2].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[3].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Unit_Price");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[4].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dgvSupInvoice.Rows[i].Cells[6].Value.ToString());
                    Writer.WriteEndElement();
                    //========================================================

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }


                //===============================================
                if (i == rowCount1)
                {

                    Writer.WriteStartElement("PAW_Purchase");
                    Writer.WriteAttributeString("xsi:type", "paw:purchase");

                    Writer.WriteStartElement("Vendor_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());//Vendor ID shoauld be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpDispatchDate.Value.ToString("MM/dd/yyyy"));//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("CustomerSO");
                    //Writer.WriteString(txtCustomerSO.Text);
                    //Writer.WriteEndElement();


                    Writer.WriteStartElement("Accounts_Payable_Account");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("PurchaseLines");

                    Writer.WriteStartElement("PurchaseLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Item_ID");
                    // Writer.WriteString("NBT001");
                    Writer.WriteString(Tax1ID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    //Writer.WriteString("NBT");
                    Writer.WriteString(Tax1Name);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("40000-MF");
                    Writer.WriteString(Tax1GL);
                    Writer.WriteEndElement();


                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    //  double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                    // double DiscountAmount = Amount * discountRate;
                    // double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtTax1Amount.Text.ToString().Trim());//HospitalCharge
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }

                //======================tax1============================
                if (i == rowCount1 + 1)
                {
                    Writer.WriteStartElement("PAW_Purchase");
                    Writer.WriteAttributeString("xsi:type", "paw:purchase");

                    Writer.WriteStartElement("Vendor_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    //Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpDispatchDate.Value.ToString("MM/dd/yyyy"));//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("CustomerSO");
                    //Writer.WriteString(txtCustomerSO.Text);
                    //Writer.WriteEndElement();


                    Writer.WriteStartElement("Accounts_Payable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType
                    Writer.WriteStartElement("PurchaseLines");

                    Writer.WriteStartElement("PurchaseLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Item_ID");
                    // Writer.WriteString("VAT001");
                    Writer.WriteString(Tax2ID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    // Writer.WriteString("VAT");
                    Writer.WriteString(Tax2Name);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("40000-MF");
                    Writer.WriteString(Tax2GL);
                    Writer.WriteEndElement();


                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    // double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                    // double DiscountAmount = Amount * discountRate;
                    // double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtTax2.Text.ToString().Trim());//tax amount1
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }
                //        //==============================================
            }

            Writer.Close();

            Connector abc = new Connector();//export to peach tree
            abc.ImportSupplierInvoice();//ImportSalesInvice()
            //}
            //catch { }
        }


        //=================================================================================
        public string PONO = "";

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


        public Boolean IsGridValidation()
        {
            try
            {
                double dilqty = 0;
                if (dgvSupInvoice.Rows.Count == 0)
                {
                    return false;
                }

                foreach (DataGridViewRow ugR in dgvSupInvoice.Rows)
                {
                    if (ugR.Cells["UnitPrice"].Value != null)
                    {
                        {
                            dilqty = dilqty + double.Parse(ugR.Cells["UnitPrice"].Value.ToString());
                        }

                        if (double.Parse(ugR.Cells["UnitPrice"].Value.ToString()) <= 0 && double.Parse(ugR.Cells["FreeQty"].Value.ToString())<=0)
                        {
                            MessageBox.Show("Enter Valid Unit Price of Item ID '" + ugR.Cells["ItemID"].Value.ToString() + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dtpDispatchDate.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dtpDispatchDate.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

            if (IsGridValidation() == false)
            {
                return;
            }

            string TranType = "Sup-Invoice";
            int DocType = 10;
            bool QtyIN = true;
            string InvRefNo = "";
            string DuplicateSupinv = "No";
            int rowCount = GetFilledRows();

            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

            String S = "Select distinct SupInvoiceNo from tblSupplierInvoices where CustomerID='" + cmbVendorSelect.Value.ToString().Trim() + "'";// where FieldType='" + FTybpe + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                if (dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() == txtInvoiceNo.Text.ToString().Trim())
                {
                    DuplicateSupinv = "Yes";
                }
            }

            Tax1Amount = double.Parse(txtNBT.Text.Trim());
            Tax2Amount = double.Parse(txtVAT.Text.Trim());

            //==============form level validations
            if (txtTotalAmount.Text == "" || txtNetTotal.Text == "" || DuplicateSupinv == "Yes" || rowCount == 0 || txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
            {
                if (DuplicateSupinv == "Yes")
                {
                    MessageBox.Show("Supplier invoice number already entered", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //btnSave.Focus();
                }

                else if (rowCount == 0)
                {
                    MessageBox.Show("Please select a GRN", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //btnSave.Focus();
                }
                else if (txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                {
                    MessageBox.Show("Please enter valid transaction", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                if (txtInvoiceNo.Text == "")
                {
                    if (!user.IsSINVNoAutoGen)
                    {
                        MessageBox.Show("Enter Supply Invoice Number", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //btnSave.Focus();
                }

                //if (!IsSerialNoCorrect())
                //    return;

                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string Dformat = "MM/dd/yyyy";
                string SupINVDate = DTP.ToString(Dformat);

                //setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                //SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                try
                {
                    Connector objConnector = new Connector();
                    if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                        return;

                    if (user.IsSINVNoAutoGen)
                    {
                        SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET SupplierInvoiceNo = SupplierInvoiceNo + 1 select SupplierInvoiceNo, SupInvoicePrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                        //myCommand.ExecuteNonQuery();

                        SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        DataTable dt41 = new DataTable();
                        da41.Fill(dt41);

                        if (dt41.Rows.Count > 0)
                        {
                            InvRefNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                            InvRefNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + InvRefNo;
                        }
                        txtInvoiceNo.Text = InvRefNo;
                    }
                    else
                    {
                        SqlCommand myCommand = new SqlCommand("select * from tblSupplierInvoices where SupInvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        DataTable dt41 = new DataTable();
                        da41.Fill(dt41);

                        if (dt41.Rows.Count > 0)
                        {
                            MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            myTrans.Rollback();
                            myConnection.Close();//
                            return;
                        }
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        bool Isreturn = false;
                        bool IsExport = false;
                        string AA = clstGRN.SelectedItems[0].ToString();
                        PONO = "";

                        SqlCommand myCommand2 = new SqlCommand("Select PONos from tblGRNTran where GRN_NO = '" + AA + "'", myConnection, myTrans);
                        SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            for (int k = 0; k < dt1.Rows.Count; k++)
                            {
                                PONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                            }
                        }

                        StrSql = "DELETE FROM [tblSupplierInvoices] WHERE SupInvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";

                        SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                        command1.CommandType = CommandType.Text;
                        command1.ExecuteNonQuery();

                        string NoOfDis = Convert.ToString(rowCount);

                        SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive) " +
                            " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + InvRefNo.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[5, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Tax1Amount + "','" + Tax2Amount + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + dgvSupInvoice[9, i].Value.ToString().Trim() + "','" + Po_No + "','"+ txtSupName .Text.ToString()+ "','"+ txtSupAdd1.Text.ToString() + "','"+ txtSupAdd2.Text.ToString() + "','"+ Convert.ToDouble(txtDisRate1.Text.ToString()) + "','"+ Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','"+ Convert.ToDouble(txtDisRate.Text.ToString()) + "','"+ Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','"+false+"')", myConnection, myTrans);
                        myCommand3.ExecuteNonQuery();

                        double SellingPRice = 0.00;
                        double ItemCost = 0.00;

                        SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                        DataTable dt34 = new DataTable();
                        da34.Fill(dt34);
                        if (dt34.Rows.Count > 0)
                        {
                            ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                        }

                        SqlCommand cmd11 = new SqlCommand("declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.Trim() + "' AND ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "') " +
                            "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + SupINVDate.Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) * Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + txtLocation.Text.ToString().Trim() + "','" + SellingPRice + "')", myConnection, myTrans);
                        cmd11.ExecuteNonQuery();


                        SqlCommand myCommand367 = new SqlCommand("update tblItemMaster set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        myCommand367.ExecuteNonQuery();

                        myCommand367 = new SqlCommand("update tblItemWhse set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "' and WhseId='" + txtLocation.Text.Trim() + "'", myConnection, myTrans);
                        myCommand367.ExecuteNonQuery();

                    }

                    bool IsGRNFinished = true;

                    for (int i = 0; i < clstGRN.Items.Count; i++)
                    {
                        if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                        {
                            string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                            SqlCommand myCommand4 = new SqlCommand("select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                            da = new SqlDataAdapter(myCommand4);
                            DataTable dt1 = new DataTable();
                            da.Fill(dt1);
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                SqlCommand myCommand5 = new SqlCommand("update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                                DataTable dt2 = new DataTable();
                                da1.Fill(dt2);
                            }
                        }
                    }

                    DataTable dtblRefNo = new DataTable();
                    dtblRefNo = GetCheckedGRNList();
                    clsSerializeItem.DtsSerialNoList.Rows.Clear();

                    foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
                    {
                        string SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                               " WHERE (tblSerialTransfer.ItemID = '" + dgvr.Cells[0].Value.ToString().Trim() + "') and Status<>'Invoiced' and   (";

                        for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        {
                            SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                            if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                SSql = SSql + " or ";
                        }
                        SSql = SSql + ")";

                        cmd = new SqlCommand(SSql, myConnection, myTrans);
                        da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Tables[0].Rows)
                        {
                            if (dr["SerialNo"].ToString() != string.Empty)
                            {
                                DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();

                                if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                                {
                                    clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                                    clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                                    clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                                    clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                                }
                                drow["SerialNo"] = dr["SerialNo"].ToString();
                                drow["Status"] = dr["Status"].ToString();
                                drow["ItemCode"] = dr["ItemCode"].ToString();
                                drow["WHCode"] = dr["WHCode"].ToString();
                                clsSerializeItem.DtsSerialNoList.Rows.Add(drow);
                            }
                        }
                        dt = new DataSet();
                    }

                    if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                    {
                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Invoice", txtLocation.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDispatchDate.Value, false, "Invoiced");
                    }

                    //the following code3 segment export items apply to po method
                    CreatePurchaseJXML(myTrans, myConnection);
                    //Following method export itemss tin to peachtree purchase closing stok method
                    Connector conn = new Connector();
                    conn.ImportSupplierInvoice();

                    //UpdateGRNs();
                    myTrans.Commit();
                    btnNew.Enabled = true;
                    btnProcesss.Enabled = false;
                    Disable();

                    MessageBox.Show(" Suppler Invoice is Successfuly Processed", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //btnReprint2_Click(sender, e);
                    btnPrint_Click(sender, e);
                    btnNew_Click(sender, e);
                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                }
                finally
                {
                    myConnection.Close();
                }
            }
        }

        private int GetFilledRowstax()
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

        public DataSet GetDistictPO(string GRNNO)//this is regarding the Scan Details
        {
            try
            {
                DataSet DistinctPO = new DataSet();

                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                String S2 = "Select Distinct(SupInvoiceNo) from tblSupplierInvoices where SupInvoiceNo='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(DistinctPO);

                return DistinctPO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        DataSet DistnctPO = new DataSet();
        DataSet POQuantity = new DataSet();
        DataSet ItemDetails = new DataSet();

        /***************new ***********************/
        //Create a XML File for import Purchase Journal=====================
        public void CreatePurchaseJXML(SqlTransaction tr, SqlConnection con)
        {

            LoadtaxDetails();
            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string Dformat = "MM/dd/yyyy";
                string GRNDate = DTP.ToString(Dformat);

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SupplierInvoice.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_Purchases");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                int DistributionNumber = 0;

                Writer.WriteStartElement("PAW_Purchase");
                Writer.WriteAttributeString("xsi:type", "paw:purchase");
                int rowCount1 = GetFilledRows();
                int rowCountFull = 0;
                int rowCounttaxImp = GetFilledRowstax();//get filled row count from the datagrid

                Writer.WriteStartElement("VendorID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbVendorSelect.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtCustomerSO.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(dtpInvRecivedDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("CustomerInvoiceNumber");
                Writer.WriteString(txtInvoiceNo.Text.Trim());//Supinv 
                Writer.WriteEndElement();

                Writer.WriteStartElement("AP_Account");
                Writer.WriteString(cmbAR.Text.Trim());//Cash Account
                Writer.WriteEndElement();//CreditMemoType

                string NoDistributions = Convert.ToString(rowCount1 + rowCounttaxImp);
            //if (txtDiscountAmount.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount.Text.Trim()) > 0)
            //{
            //     NoDistributions =Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}

            //if (txtDiscountAmount1.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount1.Text.Trim()) > 0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}

            double TotalLineDiscount = 0;
            double FreeQtyValue = 0;
            double lineAMT = 0;
            foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
            {
                try
                {
                    TotalLineDiscount = TotalLineDiscount + ((double.Parse(dgvr.Cells[2].Value.ToString()) * double.Parse(dgvr.Cells[4].Value.ToString())) - double.Parse(dgvr.Cells[8].Value.ToString()));
                    FreeQtyValue = FreeQtyValue+ double.Parse(dgvr.Cells[4].Value.ToString()) * double.Parse(dgvr.Cells[13].Value.ToString());
                }
                catch (Exception ex)
                {
                   
                }

            }

            //if(TotalLineDiscount>0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}

            //if (FreeQtyValue > 0)
            //{
            //    NoDistributions = Convert.ToString(Convert.ToInt32(NoDistributions) + 1);
            //}
           // double AMT = 0;
            Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoDistributions);
                Writer.WriteEndElement();

                Writer.WriteStartElement("PurchaseLines");
            double Qty = 0;
            for (int i = 0; i < rowCount1; i++)
                {
                bool ItemWrite = true;
                    if (i < rowCount1)
                    {
                        DistributionNumber = i + 1;
                        string[] AAA = new string[2];



                    if (IsThisItemSerial(dgvSupInvoice.Rows[i].Cells[0].Value.ToString(), tr, con))
                    {

                        for (int k = 0; k < clstGRN.CheckedItems.Count; k++)
                        {
                            AAA = clstGRN.CheckedItems[k].ToString().Split(':');

                            if (k == 0)
                            {
                         
                                for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                                {
                                    if (dgvSupInvoice.Rows[i].Cells[0].Value.ToString() == clsSerializeItem.DtsSerialNoList.Rows[j]["ItemCode"].ToString())
                                    {

                                        Writer.WriteStartElement("PurchaseLine");

                                         Qty = Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[10].Value.ToString());
                                        Writer.WriteStartElement("Quantity");
                                        Writer.WriteString(Qty.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Item_ID");
                                        Writer.WriteString(dgvSupInvoice.Rows[i].Cells[0].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Description");
                                        Writer.WriteString(dgvSupInvoice.Rows[i].Cells[1].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("GL_Account");
                                        Writer.WriteString(dgvSupInvoice.Rows[i].Cells[3].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Unit_Price");
                                        Writer.WriteString(dgvSupInvoice.Rows[i].Cells[4].Value.ToString());
                                        Writer.WriteEndElement();

                                        //  AMT = (Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[4].Value) * Qty) - (TotalLineDiscount + FreeQtyValue);
                                        lineAMT = (Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[8].Value) * (100 - (Convert.ToDouble(txtDisRate.Text.Trim()))) / 100) * (100 - (Convert.ToDouble(txtDisRate1.Text.Trim()))) / 100;

                                        Writer.WriteStartElement("Amount");
                                        Writer.WriteString(lineAMT.ToString());

                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("AppliedToPO");
                                        Writer.WriteString("TRUE");
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("PO_DistNumber");
                                        Writer.WriteString(dgvSupInvoice.Rows[i].Cells[9].Value.ToString());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("PO_Number");
                                        Writer.WriteString(AAA[1].ToString().Trim());
                                        Writer.WriteEndElement();

                                        Writer.WriteStartElement("Serial_Number");
                                        Writer.WriteString(clsSerializeItem.DtsSerialNoList.Rows[j]["SerialNo"].ToString());
                                        Writer.WriteEndElement();


                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        string ItemClass = "";
                        SqlCommand cmd345 = new SqlCommand("Select * from tblItemMaster where ItemID  = '" + dgvSupInvoice.Rows[i].Cells[0].Value.ToString() + "'", con, tr);
                        SqlDataAdapter da345 = new SqlDataAdapter(cmd345);
                        DataSet dt345 = new DataSet();
                        da345.Fill(dt345);

                        if (dt345.Tables[0].Rows.Count > 0)
                        {
                            ItemClass = dt345.Tables[0].Rows[0].ItemArray[2].ToString();
                        }
                        if (ItemClass == "10" || ItemClass == "11")
                        {

                        }
                        else
                        {
                            Qty = Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[10].Value.ToString());

                            //k = clstGRN.CheckedItems.Count;
                            if (dgvSupInvoice.Rows[i].Cells[8].Value == null)
                            {
                                Writer.WriteStartElement("PurchaseLine");
                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(Qty.ToString());
                                Writer.WriteEndElement();

                            }
                            else
                            {

                                Writer.WriteStartElement("PurchaseLine");
                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(Qty.ToString());
                                Writer.WriteEndElement();
                            }

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(dgvSupInvoice.Rows[i].Cells[0].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(dgvSupInvoice.Rows[i].Cells[1].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteString(dgvSupInvoice.Rows[i].Cells[3].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Unit_Price");
                            Writer.WriteString(dgvSupInvoice.Rows[i].Cells[4].Value.ToString());
                            Writer.WriteEndElement();

                            lineAMT = (Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[8].Value) * (100 - (Convert.ToDouble(txtDisRate.Text.Trim()))) / 100) * (100 - (Convert.ToDouble(txtDisRate1.Text.Trim()))) / 100;

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString(lineAMT.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("AppliedToPO");
                            Writer.WriteString("TRUE");
                            Writer.WriteEndElement();

                            if (dgvSupInvoice.Rows[i].Cells[9].Value == null)
                            {
                                Writer.WriteStartElement("PO_DistNumber");
                                Writer.WriteString("1");
                                Writer.WriteEndElement();
                            }
                            else
                            {
                                Writer.WriteStartElement("PO_DistNumber");
                                Writer.WriteString(dgvSupInvoice.Rows[i].Cells[9].Value.ToString());
                                Writer.WriteEndElement();
                            }

                            Writer.WriteStartElement("PO_Number");
                            Writer.WriteString(PONO.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                        }


                    }
                        
                    }

                }

            //if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
            //{

            //    Writer.WriteStartElement("PurchaseLine");
            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString("30520");//nbTAccount
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString(txtNBT.Text.ToString());//HospitalCharge
            //    Writer.WriteEndElement();

            //    //Writer.WriteStartElement("AppliedToPO");
            //    //Writer.WriteString("FALSE");
            //    //Writer.WriteEndElement();

            //    Writer.WriteStartElement("PO_DistNumber");
            //    Writer.WriteString("2");
            //    Writer.WriteEndElement();

            //    Writer.WriteEndElement();
            //}
            //if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0)
            //{
            //    Writer.WriteStartElement("PurchaseLine");

            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString(Tax2GLAccount);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString(txtVAT.Text.ToString());//tax amount1
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("AppliedToPO");
            //    Writer.WriteString("FALSE");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("PO_DistNumber");
            //    Writer.WriteString("");
            //    Writer.WriteEndElement();

            //    Writer.WriteEndElement();
            //}


            int disno = 0;
            //if (TotalLineDiscount > 0)
            //{
            //    Writer.WriteStartElement("PurchaseLine");

            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("1");
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
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString((-Convert.ToDouble(TotalLineDiscount)).ToString());//tax amount1
            //    Writer.WriteEndElement();

            //    //Writer.WriteStartElement("AppliedToPO");
            //    //Writer.WriteString("TRUE");
            //    //Writer.WriteEndElement();

            //    Writer.WriteStartElement("PO_DistNumber");
            //    Writer.WriteString((rowCount1+1).ToString());
            //    Writer.WriteEndElement();


            //    //Writer.WriteStartElement("PO_Number");
            //    //Writer.WriteString(PONO.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    Writer.WriteEndElement();

            //}

            //if (FreeQtyValue > 0)
            //{
            //    Writer.WriteStartElement("PurchaseLine");

            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("1");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Item_ID");
            //    Writer.WriteString(FreeValitemid);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(FreeValitemDis);
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString(FreeValitemGL);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString((-Convert.ToDouble(FreeQtyValue)).ToString());//tax amount1
            //    Writer.WriteEndElement();

            //    //Writer.WriteStartElement("AppliedToPO");
            //    //Writer.WriteString("TRUE");
            //    //Writer.WriteEndElement();
            //    disno = rowCount1;
            //    if (TotalLineDiscount > 0)
            //    {
            //        disno++;
            //    }
            //    Writer.WriteStartElement("PO_DistNumber");
            //    Writer.WriteString((disno+1).ToString());
            //    Writer.WriteEndElement();


            //    //Writer.WriteStartElement("PO_Number");
            //    //Writer.WriteString(PONO.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    Writer.WriteEndElement();

            //}


            //if (txtDiscountAmount.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount.Text.Trim()) > 0)
            //{
            //    Writer.WriteStartElement("PurchaseLine");

            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("1");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Item_ID");
            //    Writer.WriteString(SpecialDisItemid);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(SpecialDisItemdescription);
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString(SpecialDisGLAccount);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString((-Convert.ToDouble(txtDiscountAmount.Text)).ToString());//tax amount1
            //    Writer.WriteEndElement();

            //    //Writer.WriteStartElement("AppliedToPO");
            //    //Writer.WriteString("TRUE");
            //    //Writer.WriteEndElement();
            //    disno = rowCount1;
            //    //if (TotalLineDiscount>0)
            //    //{
            //    //    disno++;
            //    //}
            //    //if (FreeQtyValue > 0)
            //    //{
            //    //    disno++;
            //    //}
            //    Writer.WriteStartElement("PO_DistNumber");
            //    Writer.WriteString((disno+1).ToString());
            //    Writer.WriteEndElement();


            //    //Writer.WriteStartElement("PO_Number");
            //    //Writer.WriteString(PONO.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    Writer.WriteEndElement();

            //}

            //if (txtDiscountAmount1.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount1.Text.Trim()) > 0)
            //{
            //    Writer.WriteStartElement("PurchaseLine");

            //    Writer.WriteStartElement("Quantity");
            //    Writer.WriteString("1");
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Item_ID");
            //    Writer.WriteString(Cashitemid);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Description");
            //    Writer.WriteString(cashitemdis);
            //    Writer.WriteEndElement();


            //    Writer.WriteStartElement("GL_Account");
            //    Writer.WriteAttributeString("xsi:type", "paw:id");
            //    Writer.WriteString(cashGL);
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Tax_Type");
            //    Writer.WriteString("1");//Doctor Charge
            //    Writer.WriteEndElement();

            //    Writer.WriteStartElement("Amount");
            //    Writer.WriteString((-Convert.ToDouble(txtDiscountAmount1.Text)).ToString());//tax amount1
            //    Writer.WriteEndElement();

            //    //Writer.WriteStartElement("AppliedToPO");
            //    //Writer.WriteString("TRUE");
            //    //Writer.WriteEndElement();
            //    disno = rowCount1;
            //    //if (TotalLineDiscount > 0)
            //    //{
            //    //    disno++;
            //    //}
            //    //if (FreeQtyValue > 0)
            //    //{
            //    //    disno++;
            //    //}
            //    if (txtDiscountAmount.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount.Text.Trim()) > 0)
            //    {
            //        disno++;
            //    }
               
            //        Writer.WriteStartElement("PO_DistNumber");
            //        Writer.WriteString((disno+1).ToString());
            //        Writer.WriteEndElement();
                
              


            //    //Writer.WriteStartElement("PO_Number");
            //    //Writer.WriteString(PONO.ToString().Trim());
            //    //Writer.WriteEndElement();

            //    Writer.WriteEndElement();

            //}

           
            if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
            {
                Writer.WriteStartElement("PurchaseLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
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
                Writer.WriteString("1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString((Convert.ToDouble(txtNBT.Text)).ToString());//tax amount1
                Writer.WriteEndElement();

                //Writer.WriteStartElement("AppliedToPO");
                //Writer.WriteString("TRUE");
                //Writer.WriteEndElement();
                disno = rowCount1;
               
                //if (TotalLineDiscount > 0)
                //{
                //    disno++;
                //}
                //if (FreeQtyValue > 0)
                //{
                //    disno++;
                //}
                if (txtDiscountAmount.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount.Text.Trim()) > 0)
                {
                    disno++;
                }

                if (txtDiscountAmount1.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount1.Text.Trim()) > 0)
                {
                    disno++;
                }
                Writer.WriteStartElement("PO_DistNumber");
                Writer.WriteString((disno+1).ToString());
                Writer.WriteEndElement();

                //Writer.WriteStartElement("PO_Number");
                //Writer.WriteString(PONO.ToString().Trim());
                //Writer.WriteEndElement();

                Writer.WriteEndElement();

            }


            if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0)
            {
                Writer.WriteStartElement("PurchaseLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("1");
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
                Writer.WriteString("1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString((Convert.ToDouble(txtVAT.Text)).ToString());//tax amount1
                Writer.WriteEndElement();

                disno = rowCount1;

                //if (TotalLineDiscount > 0)
                //{
                //    disno++;
                //}
                //if (FreeQtyValue > 0)
                //{
                //    disno++;
                //}
                if (txtDiscountAmount.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount.Text.Trim()) > 0)
                {
                    disno++;
                }

                if (txtDiscountAmount1.Text.Trim() != string.Empty && double.Parse(txtDiscountAmount1.Text.Trim()) > 0)
                {
                    disno++;
                }

                if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                {
                    disno++;
                }
                Writer.WriteStartElement("PO_DistNumber");
                Writer.WriteString((disno+1).ToString());
                Writer.WriteEndElement();
                //Writer.WriteStartElement("AppliedToPO");
                //Writer.WriteString("TRUE");
                //Writer.WriteEndElement();



                //Writer.WriteStartElement("PO_Number");
                //Writer.WriteString(PONO.ToString().Trim());
                //Writer.WriteEndElement();

                Writer.WriteEndElement();

            }


            // Writer.WriteEndElement();
            Writer.WriteEndElement();//last line            
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            Writer.Close();
          
        }
       

        private bool IsThisItemSerial(string _ItemCode, SqlTransaction tr, SqlConnection con)
        {
            try
            {
                //if (dgvGRNTransaction.CurrentRow.Cells[0].Value == null) return false;
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + _ItemCode + "'";
                SqlCommand cmd = new SqlCommand(S, con, tr);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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

        /******************************************/

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void CleareData()
        {
            try
            {
                //dgvTaxApplicable.Rows.Clear();
                LoadtaxDetails();
                txtLocation.Text = "";
                txtSupName.Text = "";
                txtSupAdd1.Text = "";
                txtSupAdd2.Text = "";
                txtInvoiceNo.Text = "";
                txtTotalAmount.Text = "";
                //cmbAPAccount.Text = "";
                txtCustomerSO.Text = "";

                txtTax1Amount.Text = "0.00";
                txtTax2.Text = "0.00";
                txtNetTotal.Text = "0.00";
                txtTotalAmount.Text = "0.00";
                cmbtaxSys1.Text = "";
                cmbtaxSys2.Text = "";
                cmbVendorSelect.Text = "";

                dgvSupInvoice.Rows.Clear();
                clstGRN.Items.Clear();

                // txtCustomerSO.Text = "";
                txtCheckPONo.Text = "";
                GetAPAccount();
                GetCurrentUserDate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            cmbVendorSelect.Focus();
            btnProcessclick = false;
            btnProcesss.Enabled = false;
            btnEditer.Enabled = false;
            EditClick = false;
            try
            {
                txtDisRate.Text = "0.00";
                txtDisRate1.Text = "0.00";
                txtDiscountAmount.Text = "0.00";
                txtDiscountAmount1.Text = "0.00";
                txtNBTP.Text = "0.00";
                txtVATP.Text = "0.00";
                txtNBT.Text = "0.00";
                txtVAT.Text = "0.00";

                flglist = 0;
                run = false;
                dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmSupInvoice");
                if (dtUser.Rows.Count > 0)
                {
                    run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                }
                if (run)
                {
                    Enable();
                    CleareData();
                    //dgvTaxApplicable.Columns[2].Visible = true;
                    //dgvTaxApplicable.Columns[0].Visible = true;
                    // dgvTaxApplicable.Columns[3].Visible = false;
                    //dgvTaxApplicable.Columns[4].Visible = true;

                    TaxRateLoad();
                    cmbVendorSelect.Focus();

                    GetARAccount();//Infragistics
                    GetAPAccount();
                    GetVendorDataset();

                    if (user.IsSINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                    else txtInvoiceNo.ReadOnly = false;

                    cmbAR.Text = user.ApAccount;

                    clsSerializeItem.DtsSerialNoList.Rows.Clear();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this. Please contact your system administrator for assistance.", "Access Privilleges Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
            btnSave.Enabled = true;
            GetCRNNo();
        }

        public void Enable()
        {
            clstGRN.Enabled = true;
            dgvSupInvoice.ReadOnly = false;
            dgvSupInvoice.Enabled = true;
            dtpDispatchDate.Enabled = true;
            // txtLocation.Enabled = true;
            //txtSupName.Enabled = true;
            //txtSupAdd1.Enabled = true;
            //txtSupAdd2.Enabled = true;
            txtInvoiceNo.Enabled = true;
            //cmbAR.Enabled = true;
            // txtTotalAmount.Enabled = true;
            // cmbAPAccount.Enabled = true;
            //  txtCustomerSO.Enabled = true;

            // txtTax1Amount.Enabled = true;
            // txtTax2.Enabled = true;
            // txtNetTotal.Enabled = true;
            // txtTotalAmount.Enabled = true;
            cmbtaxSys1.Enabled = true;
            cmbtaxSys2.Enabled = true;
            cmbVendorSelect.Enabled = true;

            btnList.Enabled = true;
            btnclose.Enabled = true;
            btnRePrint2.Enabled = false;
            btnNew.Enabled = true;
        }

        public void Disable()
        {
            clstGRN.Enabled = false;
            dgvSupInvoice.Enabled = false;
            dtpDispatchDate.Enabled = false;
            txtLocation.Enabled = false;
            txtSupName.Enabled = false;
            txtSupAdd1.Enabled = false;
            txtSupAdd2.Enabled = false;
            txtInvoiceNo.Enabled = false;
            txtTotalAmount.Enabled = false;
            cmbAR.Enabled = false;

            txtCustomerSO.Enabled = false;
            txtTax1Amount.Enabled = false;
            txtTax2.Enabled = false;
            txtNetTotal.Enabled = false;
            txtTotalAmount.Enabled = false;
            cmbtaxSys1.Enabled = false;
            cmbtaxSys2.Enabled = false;
            cmbVendorSelect.Enabled = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            ds.Clear();

            try
            {
                String S3V = "Select * from tblVendorMaster";// where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3V = new SqlCommand(S3V);
                SqlConnection con3V = new SqlConnection(ConnectionString);
                SqlDataAdapter da3V = new SqlDataAdapter(S3V, con3V);
                da3V.Fill(ds, "DTVendor");

                String S1 = "Select * from tblSupplierInvoices where SupInvoiceNo = '" + txtInvoiceNo.Text.Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(ds, "DtSupInv");

                String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                SqlCommand cmd4 = new SqlCommand(S4);
                SqlConnection con4 = new SqlConnection(ConnectionString);
                SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                da4.Fill(ds, "dt_CompanyDetails");

                frmRepSupInvoice prininv = new frmRepSupInvoice(this);
                prininv.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                txtInvoiceNo.Text = "";
                load_Decimal();//load the numbe rof deccimal point for field
                if (frmMain.ObjSupInvoicelist == null || frmMain.ObjSupInvoicelist.IsDisposed)
                {
                    frmMain.ObjSupInvoicelist = new frmSupInvoiceList(1);
                }
                frmMain.ObjSupInvoice.TopMost = false;
                frmMain.ObjSupInvoicelist.ShowDialog();
                frmMain.ObjSupInvoicelist.TopMost = true;
              

                //txtInvoiceNo.Text = frmMain.ObjSupInvoicelist.RtnSupInvoiceNo();
                if (txtInvoiceNo.Text.Trim().Length > 0)
                {
                    flglist = 1;
                    //txtInvoiceNo.Text = "";
                    //txtInvoiceNo.Text = frmMain.ObjSupInvoicelist.RtnSupInvoiceNo();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
         string GRNNO;
        private void LoadFrimList()
        {
            try
            {
                if (flglist == 1)
                {
                    dgvSupInvoice.Rows.Clear();
                    //dgvTaxApplicable.Rows.Clear();
                    load_Decimal();
                    string ConnString = ConnectionString;
                    String S1 = "Select * from tblSupplierInvoices where SupInvoiceNo='" + txtInvoiceNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        //{
                        txtInvoiceNo.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        cmbVendorSelect.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                        string abc = dt.Rows[0].ItemArray[3].ToString().Trim();
                        dtpDispatchDate.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                        dtpInvRecivedDate.Text = dt.Rows[0].ItemArray[51].ToString().Trim();
                        cmbAR.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                        btnSave.Enabled = false;
                        IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                        if (IsActive == true)
                        {
                            btnEditer.Enabled = true;

                        }
                        else
                        {
                            btnProcesss.Enabled = false;
                            btnEditer.Enabled = false;
                        }

                        txtSupName.Text = dt.Rows[0].ItemArray[42].ToString().Trim();
                        txtSupAdd1.Text = dt.Rows[0].ItemArray[43].ToString().Trim();
                        txtSupAdd2.Text = dt.Rows[0].ItemArray[44].ToString().Trim();
                        txtDisRate1.Text = dt.Rows[0].ItemArray[45].ToString().Trim();
                        txtDiscountAmount1.Text = dt.Rows[0].ItemArray[46].ToString().Trim();
                        txtDisRate.Text = dt.Rows[0].ItemArray[47].ToString().Trim();

                        txtLocation.Text = dt.Rows[0].ItemArray[28].ToString().Trim();
                        txtCustomerSO.Text = dt.Rows[0].ItemArray[24].ToString().Trim();

                        txtNBTP.Text = dt.Rows[0]["Tax1Rate"].ToString();
                        txtVATP.Text = dt.Rows[0]["Tax2Rate"].ToString();
                        txtNBT.Text = dt.Rows[0]["Tax1Amount"].ToString();
                        txtVAT.Text = dt.Rows[0]["Tax2Amount"].ToString();
                        //for (int k = 0; k < 3; k++)
                        //{
                        //    dgvTaxApplicable.Rows.Add();
                        //    if (k == 0)
                        //    {
                        //        //TaxName 0
                        //        //Rate 1
                        //        //IsTax 2
                        //        //Amount 3
                        //        //GLAccouont 4
                        //        //TaxID 5

                        //        dgvTaxApplicable.Rows[0].Cells[0].Value = dt.Rows[0].ItemArray[29].ToString().Trim();
                        //        dgvTaxApplicable.Rows[0].Cells[3].Value = dt.Rows[0].ItemArray[16].ToString().Trim();
                        //    }

                        //    if (k == 1)
                        //    {
                        //        dgvTaxApplicable.Rows[1].Cells[0].Value = dt.Rows[0].ItemArray[30].ToString().Trim();
                        //        dgvTaxApplicable.Rows[1].Cells[3].Value = dt.Rows[0].ItemArray[17].ToString().Trim();
                        //    }
                        //    if (k == 2)
                        //    {
                        //        dgvTaxApplicable.Rows[2].Cells[0].Value = dt.Rows[0].ItemArray[32].ToString().Trim();
                        //        dgvTaxApplicable.Rows[2].Cells[3].Value = dt.Rows[0].ItemArray[33].ToString().Trim();
                        //    }
                        //    dgvTaxApplicable.Columns[2].Visible = false;
                        //    //dgvTaxApplicable.Columns[0].Visible = false;
                        //    // dgvTaxApplicable.Columns[3].Visible = false;
                        //    dgvTaxApplicable.Columns[4].Visible = false;
                        //}


                        //txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[i].ItemArray[15]).ToString();//disacount
                        //txtTax1Amount.Text = Convert.ToDouble(dt.Rows[i].ItemArray[16]).ToString();//Total amount
                        //txtTotalAmount.Text = Convert.ToDouble(dt.Rows[i].ItemArray[18]).ToString();//tax2amount
                        //txtNetTotal.Text = Convert.ToDouble(dt.Rows[i].ItemArray[19]).ToString();//Net Total
                        //txtTax2.Text = Convert.ToDouble(dt.Rows[i].ItemArray[17]).ToString();//tax2amount


                        //txtCustomerSO.Text = dt.Rows[0].ItemArray[24].ToString().Trim();
                        //txtLocation.Text = dt.Rows[0].ItemArray[28].ToString().Trim();
                        //cmbtaxSys1.Text = dt.Rows[i].ItemArray[29].ToString().Trim();
                        //cmbtaxSys2.Text = dt.Rows[i].ItemArray[30].ToString().Trim();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSupInvoice.Rows.Add();
                            dgvSupInvoice.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                            dgvSupInvoice.Rows[i].Cells[12].Value = dt.Rows[i].ItemArray[48].ToString().Trim();
                            dgvSupInvoice.Rows[i].Cells[11].Value = dt.Rows[i].ItemArray[25].ToString().Trim();
                            //dgvSupInvoice.Rows[i].Cells[1].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString();//1 Receved qty

                            if (DecimalpointQuantity == 0)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString();//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString();//1 Free qty
                            }
                            else if (DecimalpointQuantity == 1)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString("N1");//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString("N1");//1 Free qty
                            }
                            else if (DecimalpointQuantity == 2)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString("N2");//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString("N2");//1 Free qty
                            }
                            else if (DecimalpointQuantity == 3)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString("N3");//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString("N3");//1 Free qty
                            }
                            else if (DecimalpointQuantity == 4)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString("N4");//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString("N4");//1 Free qty
                            }
                            else if (DecimalpointQuantity == 7)
                            {
                                dgvSupInvoice.Rows[i].Cells[2].Value = Convert.ToDouble(dt.Rows[i].ItemArray[9]).ToString("N5");//1 Receved qty
                                dgvSupInvoice.Rows[i].Cells[13].Value = Convert.ToDouble(dt.Rows[i].ItemArray[50]).ToString("N5");//1 Free qty
                            }

                            //dgvSupInvoice.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();//1 Receved qty
                            dgvSupInvoice.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                            dgvSupInvoice.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[11].ToString().Trim();



                            if (Decimalpoint == 0)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString();//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString();//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString();
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])- Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString();
                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString();//6 amount
                            }
                            else if (Decimalpoint == 1)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N1");//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N1");//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString("N1");
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13]) - Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString("N1");

                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString("N1");//6 amount
                            }
                            else if (Decimalpoint == 2)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N2");//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N2");//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString("N2");
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13]) - Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString("N2");

                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString("N2");//6 amount
                            }
                            else if (Decimalpoint == 3)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N3");//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N3");//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString("N3");
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13]) - Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString("N3");

                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString("N3");//6 amount
                            }
                            else if (Decimalpoint == 4)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N4");//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N4");//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString("N4");
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13]) - Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString("N4");

                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString("N4");//6 amount
                            }
                            else if (Decimalpoint == 5)
                            {
                                dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N5");//4 unit price
                                dgvSupInvoice.Rows[i].Cells[7].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N5");//5 discount
                                dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[36]).ToString("N5");
                                dgvSupInvoice.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13]) - Convert.ToDouble(dt.Rows[i].ItemArray[36])).ToString("N5");

                                dgvSupInvoice.Rows[i].Cells[8].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString("N5");//6 amount
                            }

                            dgvSupInvoice.Rows[i].Cells[9].Value = Convert.ToDouble(dt.Rows[i].ItemArray[7]).ToString();
                            dgvSupInvoice.Rows[i].Cells[10].Value = Convert.ToDouble(dt.Rows[i].ItemArray[41]).ToString();
                            //dgvSupInvoice.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString();//4 unit price
                            //dgvSupInvoice.Rows[i].Cells[5].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString();//5 discount
                            //dgvSupInvoice.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[14]).ToString();//6 amount

                            //dgvSupInvoice.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[12].ToString().Trim();//4 unit price
                            //dgvSupInvoice.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[13].ToString().Trim();//5 discount
                            //dgvSupInvoice.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[14].ToString().Trim();//6 amount
                        }

                        string[] s = dt.Rows[0].ItemArray[3].ToString().Trim().Split(' ');
                        clstGRN.Items.Clear();
                        GRNNO = dt.Rows[0].ItemArray[3].ToString().Trim();
                        int a = s.Length;
                        for (int x = 0; x < a; x++)
                        {
                            clstGRN.Items.Add(s[x].ToString(), CheckState.Checked);
                        }

                        if (Decimalpoint == 0)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString();//disacount
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString();//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString();//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString();//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString();//tax2amount
                        }
                        else if (Decimalpoint == 1)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString("N1");//disacount
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString("N1");//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString("N1");//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N1");//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString("N1");//tax2amount
                        }
                        else if (Decimalpoint == 2)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString("N2");//disacount
                            txtDiscountAmount1.Text = Convert.ToDouble(dt.Rows[0]["DisCountAmount1"]).ToString("N2");
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString("N2");//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString("N2");//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N2");//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString("N2");//tax2amount
                        }
                        else if (Decimalpoint == 3)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString("N3");//disacount
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString("N3");//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString();//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N3");//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString("N3");//tax2amount
                        }
                        else if (Decimalpoint == 4)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString("N4");//disacount
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString("N4");//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString("N4");//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N4");//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString("N4");//tax2amount
                        }
                        else if (Decimalpoint == 5)
                        {
                            txtDiscountAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[15]).ToString("N5");//disacount
                            txtTax1Amount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[16]).ToString("N5");//Total amount
                            txtTotalAmount.Text = Convert.ToDouble(dt.Rows[0].ItemArray[18]).ToString("N5");//tax2amount
                            txtNetTotal.Text = Convert.ToDouble(dt.Rows[0].ItemArray[19]).ToString("N5");//Net Total
                            txtTax2.Text = Convert.ToDouble(dt.Rows[0].ItemArray[17]).ToString("N5");//tax2amount
                        }

                    }

                    TaxRateLoad();

                    btnRePrint2.Enabled = true;
                    btnclose.Enabled = true;
                    btnNew.Enabled = true;
                    clstGRN.Enabled = false;
                    dgvSupInvoice.ReadOnly = true;
                    cmbVendorSelect.Enabled = false;
                    txtInvoiceNo.Enabled = false;
                    dtpDispatchDate.Enabled = false;

                }
            }
            catch (Exception ex)
            {
              //  objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnReprint2_Click(object sender, EventArgs e)
        {

            //DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            //if (reply == DialogResult.Cancel)
            //{
            //    return;
            //}
            //if (txtInvoiceNo.Text.Trim().Length > 0)
            //{
            //    ds.Clear();

            //    try
            //    {
            //        String S1 = "Select * from tblSupplierInvoices WHERE SupInvoiceNo = '" + txtInvoiceNo.Text.Trim() + "'";
            //        SqlCommand cmd1 = new SqlCommand(S1);
            //        SqlConnection con1 = new SqlConnection(ConnectionString);
            //        SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
            //        da1.Fill(ds, "DtSupInv");

            //        String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
            //        SqlCommand cmd4 = new SqlCommand(S4);
            //        SqlConnection con4 = new SqlConnection(ConnectionString);
            //        SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
            //        da4.Fill(ds, "dt_CompanyDetails");


            //        frmRepSupInvoice prininv = new frmRepSupInvoice(this);
            //        prininv.Show();
            //    }
            //    catch (Exception ex)
            //    {
            //        objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please select an invoice and try again");
            //}
        }


        int PoValidation = 0;
        private void clstGRN_ItemCheck(object sender, ItemCheckEventArgs e)
        {
         
            //if (CheckState.Unchecked == e.NewValue)
            //{
            //    txtPONumber.Text = "";
            //    txtLocation.Text = "";
            //}



            //try
            //{
            //    if (CheckState.Unchecked == e.NewValue)
            //    {
            //        if (PoValidation == 1)
            //        {
            //        }
            //        else
            //        {

            //            txtCustomerSO.Text = "";
            //            txtCheckPONo.Text = "";
            //        }
            //    }
            //    // if (txtCustomerSO.Text.Length > 0)
            //    if (txtCheckPONo.Text.Length > 0)
            //    {
            //        //txtTotalAmount.Text = "0";
            //        int x = e.Index;
            //        string[] grnno = clstGRN.Items[x].ToString().Split(':');

            //        // string CusPO = ReturnCusPO(grnno[0].ToString());
            //        string PONo = ReturnCheckPO(grnno[0].ToString());
            //        // if (CusPO.Trim() != txtCustomerSO.Text.Trim())
            //        if (PONo.Trim() != txtCheckPONo.Text.Trim())
            //        {
            //            MessageBox.Show("You cannot select GRN's from different PO Numbers","Warning",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //            e.NewValue = CheckState.Unchecked;
            //            flg = 1;
            //            //clstGRN.Items.Clear();
            //            //Load_PurchaseOrder();
            //            //dgvSupInvoice.Rows.Clear();
            //            //txtCustomerSO.Text = "";
            //            //txtCheckPONo.Text = "";


            //            //for(int i=0;i<clstGRN.Items.Count;i++)
            //            //{

            //            //}
            //            // clstGRN.Items.Count
            //        }
            //        else
            //        {
            //            flg = 0;
            //        }                            //  }

            //    }
            //    else
            //    {
            //        flg = 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }
        Boolean IsActive = true;
        private void txtInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            LoadFrimList();
        }

        private void TaxCalculation()
        {
            try
            {

                if(btnSave.Enabled ==false)
                {
                    return;
                }
                double _SubAmt = 0;

                foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
                {
                    if (dgvr.Cells[0].Value != null && dgvr.Cells[0].Value.ToString().Trim() != string.Empty)
                    {
                        if (dgvr.Cells["colAmount"].Value == null) dgvr.Cells["colAmount"].Value = 0.00;
                        _SubAmt = _SubAmt + double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                    }
                    else if (dgvr.Cells[3].Value != null && dgvr.Cells[2].Value != null && dgvr.Cells[2].Value.ToString().Trim() != string.Empty)
                    {
                        if (dgvr.Cells["colAmount"].Value == null) dgvr.Cells["colAmount"].Value = 0.00;
                        _SubAmt = _SubAmt + double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                    }
                }

                //if (txtNBTP.Text.ToString() != "" && txtNBTP.Text.ToString() != String.Empty && txtVATP.Text.ToString() != "" && txtVATP.Text.ToString() != String.Empty && txtDiscountAmount1.Text.ToString() != "" && txtDiscountAmount1.Text.ToString() != String.Empty)
                //{
                //    _SubAmt = _SubAmt - double.Parse(txtDiscountAmount.Text.ToString()) - double.Parse(txtDiscountAmount1.Text.ToString()) - double.Parse(txtNBT.Text.ToString()) - double.Parse(txtVAT.Text.ToString());
                //}
                txtTotalAmount.Text = _SubAmt.ToString("N" + "" + Decimalpoint + "");
                if (txtNBTP.Text.Trim() == string.Empty) txtNBTP.Text = "0.00";
                if (txtVATP.Text.Trim() == string.Empty) txtVATP.Text = "0.00";
                if (txtDiscountAmount.Text.Trim() == string.Empty) txtDiscountAmount.Text = "0.00";
                if (txtDiscountAmount1.Text.Trim() == string.Empty) txtDiscountAmount1.Text = "0.00";

                if(txtDisRate.Text.ToString()!=""&&txtDisRate.Text.ToString()!=string.Empty)
                {
                    txtDiscountAmount.Text = (_SubAmt * double.Parse(txtDisRate.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }
                if (txtDisRate1.Text.ToString() != "" && txtDisRate1.Text.ToString() != string.Empty)
                {
                    txtDiscountAmount1.Text = ((_SubAmt - double.Parse(txtDiscountAmount.Text.Trim())) * double.Parse(txtDisRate1.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                if (txtNBT.Text.ToString() != "" && txtNBT.Text.ToString() != string.Empty)
                {
                    txtNBT.Text = ((_SubAmt - double.Parse(txtDiscountAmount.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim())) * double.Parse(txtNBTP.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                if (txtVAT.Text.ToString() != "" && txtVAT.Text.ToString() != string.Empty)
                {
                    txtVAT.Text = ((_SubAmt - double.Parse(txtDiscountAmount.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim()) + double.Parse(txtNBT.Text.Trim())) * double.Parse(txtVATP.Text.Trim()) / 100).ToString("N" + "" + Decimalpoint + "");
                }

                try
                {
                    txtNetTotal.Text = (double.Parse(txtNBT.Text.Trim()) + double.Parse(txtVAT.Text.Trim()) + _SubAmt - double.Parse(txtDiscountAmount.Text.Trim()) - double.Parse(txtDiscountAmount1.Text.Trim())).ToString("N" + "" + Decimalpoint + "");
                }
                catch(Exception ex)
                {
                    txtNetTotal.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbVendorSelect_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        LoadGRN(cmbVendorSelect.ActiveRow.Cells[0].Value.ToString());
                        txtSupName.Text = cmbVendorSelect.ActiveRow.Cells[1].Value.ToString();
                        txtSupAdd1.Text = cmbVendorSelect.ActiveRow.Cells[2].Value.ToString();
                        txtSupAdd2.Text = cmbVendorSelect.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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
        private void frmSupInvoice_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsFind)
                {
                    GetARAccount();//Infragistics
                    GetAPAccount();
                    GetVendorDataset();
                    flglist = 0;
                    // Disable();
                    btnNew.Enabled = true;
                    TaxValidation();
                    Enable();
                    GetCurrentUserDate();
                    LoadTaxMethod();
                    LoadtaxDetails();
                    load_Decimal();//load the numbe rof deccimal point for field
                    TaxRateLoad();
                    GetChargeItems();
                    cmbVendorSelect.Focus();
                    btnNew_Click(sender, e);
                    
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void GetCRNNo()
        {

            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT OpdPref, OpdPad, OpdNo FROM tblDefualtSetting";

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

                    txtInvoiceNo.Text = StrInvNo + StrInV.Substring(1, intX);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

                string StrSql6 = "SELECT ItemID,ItemDescription,SalesGLAccount FROM tblItemMaster where Custom8  ='6'";

                SqlCommand cmd6 = new SqlCommand(StrSql6);
                SqlDataAdapter da6 = new SqlDataAdapter(StrSql6, ConnectionString);
                DataTable dt6 = new DataTable();
                dt6.Clear();
                da6.Fill(dt6);
                {
                    FreeValitemid = dt6.Rows[0].ItemArray[0].ToString();
                    FreeValitemDis = dt6.Rows[0].ItemArray[1].ToString();
                    FreeValitemGL = dt6.Rows[0].ItemArray[2].ToString();
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

        private void cmbVendorSelect_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtInvoiceNo, cmbVendorSelect, e);

            if (e.KeyCode == Keys.Down)
            {
                if (cmbVendorSelect.Enabled == true)
                {
                    cmbVendorSelect.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void dgvSupInvoice_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
                    if (dgvSupInvoice.Rows[a].Cells[2].Value != null && dgvSupInvoice.Rows[a].Cells[4].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[2].Value);
                        unitprice = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[4].Value);

                        if (e.ColumnIndex == 5 || e.ColumnIndex == 6)
                        {
                            dgvSupInvoice.Rows[a].Cells[7].Value = (Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[5].Value) + Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[6].Value)).ToString("N2");
                        }
                        if (dgvSupInvoice.Rows[a].Cells[7].Value != (Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[5].Value) + Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[6].Value)).ToString("N2"))
                        {
                            if (Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[7].Value) >= Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[6].Value))
                            {
                                dgvSupInvoice.Rows[a].Cells[5].Value = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[7].Value) - Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[6].Value);
                            }
                        }


                        DiscountRate = Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[7].Value) / 100;

                        Amount = (DispatchQty * unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        dgvSupInvoice.Rows[a].Cells[8].Value = Amount1.ToString("N" + "" + Decimalpoint + "");
                        dgvSupInvoice.Rows[a].Cells[12].Value = (Amount1/ Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[10].Value)).ToString("N" + "" + Decimalpoint + "");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvSupInvoice.Rows[a].Cells[8].Value);// sanjeewa change cell value 7 into 8
                    }
                }
                txtTotalAmount.Text = TotalAmount.ToString("N" + "" + Decimalpoint + "");
                txtNetTotal.Text = TotalAmount.ToString("N" + "" + Decimalpoint + "");
                //=========================================
                // dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells[3];
                TaxCalculation();
                //===================================================================
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvSupInvoice_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvSupInvoice.IsCurrentCellDirty)
                {
                    dgvSupInvoice.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtLocation.Text.Trim() != string.Empty)
                {
                    String S = "Select * from tblWhseMaster where WhseId='" + txtLocation.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    cmbAR.Text = dt.Tables[0].Rows[0]["APAccount"].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        bool DiferentPOValidate = false;
      
        private void clstGRN_MouseClick(object sender, MouseEventArgs e)
        {
            DiferentPOValidate = false;

            try
            {
                if (clstGRN.CheckedItems.Count == 0)
                {
                    txtLocation.Text = "";
                    txtPONumber.Text = "";
                    txtCustomerSO.Text = "";
                }
                ////  string GRNNo = GRN_No.Replace("'", "").Trim();
                //String S12 = "Select CustomerSO from tblGRNTran where GRN_NO='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                ////  String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                //SqlCommand cmd12 = new SqlCommand(S12);
                //SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                //DataTable dt12 = new DataTable();
                //da12.Fill(dt12);

                //if (dt12.Rows.Count > 0)
                //{
                //    txtCustomerSO.Text = dt12.Rows[0].ItemArray[0].ToString();
                //}

              

                    String S = "Select distinct PONos from tblGRNTran where GRN_NO ='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    if (txtPONumber.Text != string.Empty)
                    {
                        if (txtPONumber.Text.ToString().Trim() == dt.Rows[0].ItemArray[0].ToString().Trim())
                        {
                            txtPONumber.Text = dt.Rows[0].ItemArray[0].ToString();
                            DiferentPOValidate = false;
                        }
                        else
                        {
                            DiferentPOValidate = true;

                            MessageBox.Show("Please Select GRNs Belongs to the Same Purchase Order Number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            clstGRN.SetItemChecked(clstGRN.SelectedIndex, false);
                            clstGRN.Refresh();
                            clistbxSalesOrder_SelectedIndexChanged(sender, e);
                            clstGRN_ItemCheck(sender, null);

                            return;
                        }
                    }
                    else
                    {
                        txtPONumber.Text = dt.Rows[0].ItemArray[0].ToString();
                       
                    }
                }

                //================================================

                // PoValidation = 0;
                String S1 = "Select distinct WareHouseID from tblGRNTran where GRN_NO ='" + clstGRN.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {

                    if (txtLocation.Text != string.Empty)
                    {
                        if (txtLocation.Text == dt1.Rows[0].ItemArray[0].ToString())
                        {
                            txtLocation.Text = dt1.Rows[0].ItemArray[0].ToString();
                       
                        }
                        else
                        {
                            MessageBox.Show("Please Select GRNs belongs to the same Warehouse", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            clstGRN.SetItemChecked(clstGRN.SelectedIndex, false);
                            clistbxSalesOrder_SelectedIndexChanged(sender, e);
                          
                            return;
                        }
                    }
                    else
                    {
                        txtLocation.Text = dt1.Rows[0].ItemArray[0].ToString();
                    }
                }
            }
            catch { }
            //End of the cell end edit event*********************************
            //=====================================
        }



        private void txtNBT_TextChanged(object sender, EventArgs e)
        {
            //TaxCalculation();
            try
            {
                int intGridRow = 0;
                double dblSubTot = 0;
                for (intGridRow = 0; intGridRow < dgvSupInvoice.Rows.Count; intGridRow++)
                {
                    if (dgvSupInvoice.Rows[intGridRow].Cells[8].Value != null)
                    {
                        dblSubTot += double.Parse(dgvSupInvoice.Rows[intGridRow].Cells[8].Value.ToString());
                    }
                }
                double TotalDis = Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtDiscountAmount1.Text.ToString());
                double TotalVat = Convert.ToDouble(txtVAT.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString());
                txtNetTotal.Text = ((dblSubTot - TotalDis) + TotalVat).ToString("N2");
            }
            catch
            {

            }
        }

        private void txtVATP_TextChanged(object sender, EventArgs e)
        {

            if (btnProcessclick == false)
            {
                TaxCalculation();
            }
            //if (txtVATP.Text != "0" && btnEditer.Enabled == false)
            //{
            //    // TaxCalculation();
            //    try
            //    {
            //        double DisRate = 0.0;
            //        double Grosstotal = 0.0;
            //        double DiscountAmount = 0.0;
            //        double NetTotal = 0.0;
            //        if (txtVATP.Text == string.Empty)
            //        {
            //            DisRate = 0.0;
            //        }
            //        else
            //        {
            //            DisRate = Convert.ToDouble(txtVATP.Text) / 100;
            //        }
            //        Grosstotal = Convert.ToDouble(txtTotalAmount.Text);
            //        DiscountAmount = Convert.ToDouble(txtNetTotal.Text) * DisRate;

            //        txtVAT.Text = DiscountAmount.ToString("N2");
            //        if (txtVATP.Text.ToString() != "" || txtVATP.Text.ToString() != string.Empty)
            //        {
            //            NetTotal = Grosstotal - Convert.ToDouble(txtDiscountAmount1.Text.ToString()) - Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString()) + DiscountAmount;
            //        }
            //        else
            //        {
            //            NetTotal = (Grosstotal - Convert.ToDouble(txtDiscountAmount1.Text.ToString()) - Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString()));
            //        }
            //        txtNetTotal.Text = NetTotal.ToString("N2");

            //    }
            //    catch (Exception ex)
            //    {
            //        objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //    }
            //}
        }

        private void txtNBTP_TextChanged(object sender, EventArgs e)
        {
            //if (txtNBTP.Text != "" && btnEditer.Enabled == false)
            //{

            if (btnProcessclick == false)
            {
                TaxCalculation();
            }
            //    try
            //    {
            //        double DisRate = 0.0;
            //        double Grosstotal = 0.0;
            //        double DiscountAmount = 0.0;
            //        double NetTotal = 0.0;
            //        if (txtNBTP.Text == string.Empty)
            //        {
            //            DisRate = 0.0;
            //        }
            //        else
            //        {
            //            DisRate = Convert.ToDouble(txtNBTP.Text) / 100;
            //        }
            //        Grosstotal = Convert.ToDouble(txtTotalAmount.Text);
            //        DiscountAmount = Convert.ToDouble(txtNetTotal.Text) * DisRate;

            //        txtNBT.Text = DiscountAmount.ToString("N2");
            //        if (txtNBTP.Text.ToString() != "" || txtNBTP.Text.ToString() != string.Empty)
            //        {
            //            NetTotal = Grosstotal - Convert.ToDouble(txtDiscountAmount1.Text.ToString()) - Convert.ToDouble(txtDiscountAmount.Text.ToString()) + DiscountAmount;
            //        }
            //        else
            //        {
            //            NetTotal = (Grosstotal - Convert.ToDouble(txtDiscountAmount1.Text.ToString()) - Convert.ToDouble(txtDiscountAmount.Text.ToString()));
            //        }
            //        txtNetTotal.Text = NetTotal.ToString("N2");

            //    }
            //    catch (Exception ex)
            //    {
            //        objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //    }
            //}
        }

        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtLocation.Text == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse First");
                    return;
                }

                if (Convert.ToDouble(dgvSupInvoice.CurrentRow.Cells[2].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + dgvSupInvoice.CurrentRow.Cells[2].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            dgvSupInvoice.CurrentRow.Cells[2].Selected = true;
                        }
                    }
                }
                else
                {
                    bool Find = false;
                    if (flglist == 1) Find = true;
                    DataSet dt = new DataSet();

                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Sup-Invoice", txtLocation.Text.ToString().Trim(),
                        dgvSupInvoice.CurrentRow.Cells[0].Value.ToString(),
                        Convert.ToDouble(dgvSupInvoice.CurrentRow.Cells[2].Value.ToString()),
                        txtInvoiceNo.Text.Trim(), Find, clsSerializeItem.DtsSerialNoList, GetCheckedGRNList(), false, false);

                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable GetCheckedGRNList()
        {
            DataTable _dtblGRN = new DataTable();
            _dtblGRN.Columns.Add("GRN");

            try
            {
                for (int i = 0; i < clstGRN.Items.Count; i++)
                {
                    if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

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

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                //if(dgvGRNTransaction.CurrentRow.Cells[0].Value==null) return false;
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

        private void dgvSupInvoice_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvSupInvoice.CurrentRow != null)
                {
                    if (dgvSupInvoice.CurrentRow.Cells[0].Value != null)
                    {
                        if (IsThisItemSerial(dgvSupInvoice.CurrentRow.Cells[0].Value.ToString()))
                            btnSNO.Enabled = true;
                        else
                            btnSNO.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvSupInvoice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dgvSupInvoice_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmSupInvoice_KeyDown(object sender, KeyEventArgs e)
        {
          


            if (btnSave.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
                {
                    btnSave_Click_1(null, null);

                }
            }

            if (btnList.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
                {
                    btnList_Click(null, null);
                }
            }


            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                btnNew_Click(null, null);
            }


            if (btnProcesss.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    btnProcesss_Click(null, null);
                }
            }

            if (btnRePrint2.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    btnPrint_Click(null, null);
                }
            }

            if (btnEditer.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    btnEditer_Click(null, null);
                }
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                this.Close();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void clstGRN_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                clstGRN.SetItemChecked(clstGRN.SelectedIndex, true);

            }
        }

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int intGridRow = 0;
                double dblSubTot = 0;
                for (intGridRow = 0; intGridRow < dgvSupInvoice.Rows.Count; intGridRow++)
                {
                    if (dgvSupInvoice.Rows[intGridRow].Cells[8].Value != null)
                    {
                        dblSubTot += double.Parse(dgvSupInvoice.Rows[intGridRow].Cells[8].Value.ToString());
                    }
                }
                double TotalDis = Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtDiscountAmount1.Text.ToString());
                double TotalVat = Convert.ToDouble(txtVAT.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString());
                txtNetTotal.Text = ((dblSubTot - TotalDis) + TotalVat).ToString("N2");
            }
            catch
            {

            }
        }

        private void txtDiscountAmount1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int intGridRow = 0;
                double dblSubTot = 0;
                for (intGridRow = 0; intGridRow < dgvSupInvoice.Rows.Count; intGridRow++)
                {
                    if (dgvSupInvoice.Rows[intGridRow].Cells[8].Value != null)
                    {
                        dblSubTot += double.Parse(dgvSupInvoice.Rows[intGridRow].Cells[8].Value.ToString());
                    }
                }
                double TotalDis = Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtDiscountAmount1.Text.ToString());
                double TotalVat = Convert.ToDouble(txtVAT.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString());
                txtNetTotal.Text = ((dblSubTot - TotalDis) + TotalVat).ToString("N2");
            }
            catch
            {

            }
        }

        private void txtVAT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int intGridRow = 0;
                double dblSubTot = 0;
                for (intGridRow = 0; intGridRow < dgvSupInvoice.Rows.Count; intGridRow++)
                {
                    if (dgvSupInvoice.Rows[intGridRow].Cells[8].Value != null)
                    {
                        dblSubTot += double.Parse(dgvSupInvoice.Rows[intGridRow].Cells[8].Value.ToString());
                    }
                }
                double TotalDis = Convert.ToDouble(txtDiscountAmount.Text.ToString()) + Convert.ToDouble(txtDiscountAmount1.Text.ToString());
                double TotalVat = Convert.ToDouble(txtVAT.Text.ToString()) + Convert.ToDouble(txtNBT.Text.ToString());
                txtNetTotal.Text = ((dblSubTot - TotalDis) + TotalVat).ToString("N2");
            }
            catch
            {

            }
        }

        private void cmbVendorSelect_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        public void DeleteTable(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE FROM Temp_Barcode";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                int rowCount = GetFilledRows();
                double QtyCount = 0;
                double QtyIN = 0;
                string Itemcode = "";
                string Taxable = "";
                double TaxablePrice = 0;
                double UnitPrice = 0;
                string ItemDescription;

                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                DeleteTable(myConnection, myTrans);
                for (int b = 0; b < rowCount; b++)
                {
                    if (Convert.ToDouble(dgvSupInvoice.Rows[b].Cells[2].Value) > 0)
                    {
                        QtyIN = Convert.ToDouble(dgvSupInvoice.Rows[b].Cells[2].Value);
                        Itemcode = dgvSupInvoice.Rows[b].Cells[0].Value.ToString();
                        ItemDescription = dgvSupInvoice.Rows[b].Cells[1].Value.ToString();
                        QtyCount = Convert.ToDouble(dgvSupInvoice.Rows[b].Cells[2].Value);

                        for (int c = 0; c < QtyCount; c++)
                        {
                            String S = "Select TaxType,UnitPrice from tblItemMaster where ItemID  = '" + Itemcode.ToString() + "'";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataSet dt = new DataSet();
                            da.Fill(dt);
                            if (dt.Tables[0].Rows.Count > 0)
                            {
                                Taxable = dt.Tables[0].Rows[0].ItemArray[0].ToString();
                                UnitPrice = Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString());   //Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString());
                            }
                            if (Taxable == "TAX")
                            {
                                TaxablePrice = (Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString()) * 112) / 100;
                            }
                            else
                            {
                                TaxablePrice = UnitPrice;
                            }
                            SqlCommand myCommand2 = new SqlCommand("insert into Temp_Barcode(Itemcode,Description,Taxable,UnitPrice,TaxablePrice) Values ('" + Itemcode.ToString().Trim() + "','" + ItemDescription.ToString().Trim() + "','" + Taxable + "','" + UnitPrice + "','" + TaxablePrice.ToString().Trim() + "')", myConnection, myTrans);
                            myCommand2.ExecuteNonQuery();
                        }
                    }
                }
                myTrans.Commit();
                printbarcode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }
        private void printbarcode()
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            ds.Clear();

            try
            {
                Printype = 1;
                String S3V = "Select * from dbo.Temp_Barcode";// where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3V = new SqlCommand(S3V);
                SqlConnection con3V = new SqlConnection(ConnectionString);
                SqlDataAdapter da3V = new SqlDataAdapter(S3V, con3V);
                da3V.Fill(ds, "DTBarCode");

                frmRepSupInvoice prininv = new frmRepSupInvoice(this);
                prininv.Show();
            }
            catch (Exception ex)
            {
                //objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                throw ex;
            }
        }

        private void txtDisRate1_TextChanged(object sender, EventArgs e)
        {

            if (btnProcessclick == false)
            {
                TaxCalculation();
            }
            //if (txtDisRate1.Text != "0" && btnEditer.Enabled == false)
            //{
            //    try
            //    {
            //        double DisRate = 0.0;
            //        double Grosstotal = 0.0;
            //        double DiscountAmount = 0.0;
            //        double NetTotal = 0.0;
            //        if (txtDisRate1.Text == string.Empty)
            //        {
            //            DisRate = 0.0;
            //        }
            //        else
            //        {
            //            DisRate = Convert.ToDouble(txtDisRate1.Text) / 100;
            //        }
            //        Grosstotal = Convert.ToDouble(txtTotalAmount.Text);
            //        DiscountAmount = Convert.ToDouble(txtNetTotal.Text) * DisRate;

            //        txtDiscountAmount1.Text = DiscountAmount.ToString("N2");
            //        if (txtDiscountAmount.Text.ToString() != "" || txtDiscountAmount.Text.ToString() != string.Empty)
            //        {
            //            double Dis1 = Convert.ToDouble(txtDiscountAmount1.Text.ToString());
            //            double nbt = Convert.ToDouble(txtNBT.Text.ToString());
            //            double vat = Convert.ToDouble(txtVAT.Text.ToString());
            //            NetTotal = (Grosstotal - DiscountAmount) - Dis1 + nbt + vat;
            //        }
            //        else
            //        {
            //            NetTotal = (Grosstotal - DiscountAmount + Convert.ToDouble(txtNBT.Text.ToString()) + Convert.ToDouble(txtVAT.Text.ToString()));
            //        }
            //        txtNetTotal.Text = NetTotal.ToString("N2");

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            dgvSupInvoice.CommitEdit(DataGridViewDataErrorContexts.Commit);
            if (SelectSONums1=="")
            {
                SelectSONums1 = GRNNO;
            }
            if (EditClick == true)
            {
                if (dtpDispatchDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDispatchDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

                if (IsGridValidation() == false)
                {
                    return;
                }

                string TranType = "Sup-Invoice";
                int DocType = 10;
                bool QtyIN = true;
                string InvRefNo = "";
                string DuplicateSupinv = "No";
                int rowCount = GetFilledRows();

                int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

                String S = "Select distinct SupInvoiceNo from tblSupplierInvoices where CustomerID='" + cmbVendorSelect.Value.ToString().Trim() + "'";// where FieldType='" + FTybpe + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    if (dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() == txtInvoiceNo.Text.ToString().Trim())
                //    {
                //        DuplicateSupinv = "Yes";
                //    }
                //}

                Tax1Amount = double.Parse(txtNBT.Text.Trim());
                Tax2Amount = double.Parse(txtVAT.Text.Trim());

                //==============form level validations
                if (txtTotalAmount.Text == "" || txtNetTotal.Text == "" || DuplicateSupinv == "Yes" || rowCount == 0 || txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                {
                    if (DuplicateSupinv == "Yes")
                    {
                        MessageBox.Show("Supplier invoice number already entered", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //btnSave.Focus();
                    }

                    else if (rowCount == 0)
                    {
                        MessageBox.Show("Please select a GRN", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //btnSave.Focus();
                    }
                    else if (txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                    {
                        MessageBox.Show("Please enter valid transaction", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        if (!user.IsSINVNoAutoGen)
                        {
                            MessageBox.Show("Enter Supply Invoice Number", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        //btnSave.Focus();
                    }

                    //if (!IsSerialNoCorrect())
                    //    return;

                    DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string SupINVDate = DTP.ToString(Dformat);
                    DateTime DTP2 = Convert.ToDateTime(dtpInvRecivedDate.Text);
                    string InvoiceRecvidDate = DTP2.ToString(Dformat);
                    //setConnectionString();
                    SqlConnection myConnection = new SqlConnection(ConnectionString);
                    //SqlCommand myCommand = new SqlCommand();
                    myConnection.Open();
                    SqlTransaction myTrans = myConnection.BeginTransaction();

                    try
                   {
                    //    Connector objConnector = new Connector();
                    //    if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                    //        return;

                        if (user.IsSINVNoAutoGen)
                        {
                            //SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET SupplierInvoiceNo = SupplierInvoiceNo + 1 select SupplierInvoiceNo, SupInvoicePrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                            ////myCommand.ExecuteNonQuery();

                            //SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            //DataTable dt41 = new DataTable();
                            //da41.Fill(dt41);

                            //if (dt41.Rows.Count > 0)
                            //{
                            //    InvRefNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                            //    InvRefNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + InvRefNo;
                            //}
                            // txtInvoiceNo.Text = InvRefNo;
                        }
                        else
                        {
                            SqlCommand myCommand = new SqlCommand("select * from tblSupplierInvoices where SupInvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            //if (dt41.Rows.Count > 0)
                            //{
                            //    MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    myTrans.Rollback();
                            //    myConnection.Close();//
                            //    return;
                            //}
                        }
                        string S1 = "DELETE FROM [dbo].[tblSupplierInvoices] where SupInvoiceNo ='" + txtInvoiceNo.Text.ToString().Trim() + "'";

                        SqlCommand cmd1 = new SqlCommand(S1, myConnection, myTrans);
                        cmd1.ExecuteNonQuery();

                        for (int i = 0; i < rowCount; i++)
                        {
                            bool Isreturn = false;
                            bool IsExport = false;
                            //string AA = clstGRN.SelectedItems[0].ToString();
                            //PONO = "";

                            SqlCommand myCommand2 = new SqlCommand("Select PONos from tblGRNTran where GRN_NO = '" + SelectSONums1 + "'", myConnection, myTrans);
                            SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);

                            if (dt1.Rows.Count > 0)
                            {
                                for (int k = 0; k < dt1.Rows.Count; k++)
                                {
                                    PONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                                }
                            }
                            string NoOfDis = Convert.ToString(rowCount);

                          

                            SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive,CurrencyRate,FreeQty,InvoiceRecivedDate) " +
                                " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[7, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Convert.ToDouble(txtNBT.Text) + "','" + Convert.ToDouble(txtVAT.Text) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','" + dgvSupInvoice[11, i].Value + "','" + PONO + "','" + txtSupName.Text.ToString() + "','" + txtSupAdd1.Text.ToString() + "','" + txtSupAdd2.Text.ToString() + "','" + Convert.ToDouble(txtDisRate1.Text.ToString()) + "','" + Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','" + Convert.ToDouble(txtDisRate.Text.ToString()) + "','" + Convert.ToDouble(dgvSupInvoice[12, i].Value) + "','" + true + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[13, i].Value) + "','"+ InvoiceRecvidDate.Trim()+ "')", myConnection, myTrans);
                            myCommand3.ExecuteNonQuery();

                            double SellingPRice = 0.00;
                            double ItemCost = 0.00;

                            //SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                            //DataTable dt34 = new DataTable();
                            //da34.Fill(dt34);
                            //if (dt34.Rows.Count > 0)
                            //{
                            //    ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                            //}

                            //SqlCommand cmd11 = new SqlCommand("declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.Trim() + "' AND ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "') " +
                            //    "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + SupINVDate.Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) * Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + txtLocation.Text.ToString().Trim() + "','" + SellingPRice + "')", myConnection, myTrans);
                            //cmd11.ExecuteNonQuery();


                            //SqlCommand myCommand367 = new SqlCommand("update tblItemMaster set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //myCommand367.ExecuteNonQuery();

                            //myCommand367 = new SqlCommand("update tblItemWhse set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "' and WhseId='" + txtLocation.Text.Trim() + "'", myConnection, myTrans);
                            //myCommand367.ExecuteNonQuery();

                        }

                        //bool IsGRNFinished = true;

                        //for (int i = 0; i < clstGRN.Items.Count; i++)
                        //{
                        //    if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                        //    {
                        //        string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                        //        SqlCommand myCommand4 = new SqlCommand("select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                        //        da = new SqlDataAdapter(myCommand4);
                        //        DataTable dt1 = new DataTable();
                        //        da.Fill(dt1);
                        //        for (int j = 0; j < dt1.Rows.Count; j++)
                        //        {
                        //            SqlCommand myCommand5 = new SqlCommand("update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                        //            SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                        //            DataTable dt2 = new DataTable();
                        //            da1.Fill(dt2);
                        //        }
                        //    }
                        //}

                        //DataTable dtblRefNo = new DataTable();
                        //dtblRefNo = GetCheckedGRNList();
                        //clsSerializeItem.DtsSerialNoList.Rows.Clear();

                        //foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
                        //{
                        //    string SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                        //           " WHERE (tblSerialTransfer.ItemID = '" + dgvr.Cells[0].Value.ToString().Trim() + "') and Status<>'Invoiced' and   (";

                        //    for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        //    {
                        //        SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                        //        if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                        //            SSql = SSql + " or ";
                        //    }
                        //    SSql = SSql + ")";

                        //    cmd = new SqlCommand(SSql, myConnection, myTrans);
                        //    da = new SqlDataAdapter(cmd);
                        //    da.Fill(dt);

                        //    foreach (DataRow dr in dt.Tables[0].Rows)
                        //    {
                        //        if (dr["SerialNo"].ToString() != string.Empty)
                        //        {
                        //            DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();

                        //            if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                        //            {
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                        //            }
                        //            drow["SerialNo"] = dr["SerialNo"].ToString();
                        //            drow["Status"] = dr["Status"].ToString();
                        //            drow["ItemCode"] = dr["ItemCode"].ToString();
                        //            drow["WHCode"] = dr["WHCode"].ToString();
                        //            clsSerializeItem.DtsSerialNoList.Rows.Add(drow);
                        //        }
                        //    }
                        //    dt = new DataSet();
                        //}

                        //if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                        //{
                        //    frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Invoice", txtLocation.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDispatchDate.Value, false, "Invoiced");
                        //}

                        ////the following code3 segment export items apply to po method
                     //   CreatePurchaseJXML(myTrans, myConnection);
                        //Following method export itemss tin to peachtree purchase closing stok method
                        //Connector conn = new Connector();
                        //conn.ImportSupplierInvoice();

                        //UpdateGRNs();
                        myTrans.Commit();
                        btnNew.Enabled = true;
                        btnSave.Enabled = false;
                        btnRePrint2.Enabled = true;
                        btnProcesss.Enabled = true;
                       // Disable();

                        MessageBox.Show(" Suppler Invoice is Successfuly Saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        EditClick = false;
                        //btnReprint2_Click(sender, e);
                        //  btnPrint_Click(sender, e);
                        //  btnNew_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                         objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }

            }
            else
            {
                if (dtpDispatchDate.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpDispatchDate.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
                if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

                if (IsGridValidation() == false)
                {
                    return;
                }

                string TranType = "Sup-Invoice";
                int DocType = 10;
                bool QtyIN = true;
                string InvRefNo = "";
                string DuplicateSupinv = "No";
                int rowCount = GetFilledRows();

                int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

                String S = "Select distinct SupInvoiceNo from tblSupplierInvoices where CustomerID='" + cmbVendorSelect.Value.ToString().Trim() + "' and IsActive='"+true+ "' and CustomerSO!= '"+ txtCustomerSO.Text+ "'";// where FieldType='" + FTybpe + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() == txtInvoiceNo.Text.ToString().Trim())
                    {
                        DuplicateSupinv = "Yes";
                    }
                }

                Tax1Amount = double.Parse(txtNBT.Text.Trim());
                Tax2Amount = double.Parse(txtVAT.Text.Trim());

                //==============form level validations
                if (txtTotalAmount.Text == "" || txtNetTotal.Text == "" || DuplicateSupinv == "Yes" || rowCount == 0 || txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                {
                    if (DuplicateSupinv == "Yes")
                    {
                        MessageBox.Show("This Supplier has unprocced invoice", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //btnSave.Focus();
                    }

                    if (rowCount == 0)
                    {
                        MessageBox.Show("Please select a GRN", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //btnSave.Focus();
                    }
                    else if (txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                    {
                        MessageBox.Show("Please enter valid transaction", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        if (!user.IsSINVNoAutoGen)
                        {
                            MessageBox.Show("Enter Supply Invoice Number", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        //btnSave.Focus();
                    }

                    //if (!IsSerialNoCorrect())
                    //    return;

                    DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string SupINVDate = DTP.ToString(Dformat);

                    //setConnectionString();
                    SqlConnection myConnection = new SqlConnection(ConnectionString);
                    //SqlCommand myCommand = new SqlCommand();
                    myConnection.Open();
                    SqlTransaction myTrans = myConnection.BeginTransaction();

                    try
                    {
                        //Connector objConnector = new Connector();
                        //if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                        //    return;

                        if (user.IsCRTNNoAutoGen)
                        {
                            InvRefNo = GetInvNoField(myConnection, myTrans);
                            UpdatePrefixNo(myConnection, myTrans);
                            txtInvoiceNo.Text = InvRefNo;
                        }
                        else
                        {
                            SqlCommand myCommand = new SqlCommand("select * from tblSupplierInvoices where SupInvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            if (dt41.Rows.Count > 0)
                            {
                                MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                myTrans.Rollback();
                                myConnection.Close();//
                                return;
                            }
                        }

                        for (int i = 0; i < rowCount; i++)
                        {
                            bool Isreturn = false;
                            bool IsExport = false;
                            string AA = clstGRN.SelectedItems[0].ToString();
                            PONO = "";

                            SqlCommand myCommand2 = new SqlCommand("Select PONos from tblGRNTran where GRN_NO = '" + SelectSONums1 + "'", myConnection, myTrans);
                            SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);

                            if (dt1.Rows.Count > 0)
                            {
                                for (int k = 0; k < dt1.Rows.Count; k++)
                                {
                                    PONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                                }
                            }
                            string NoOfDis = Convert.ToString(rowCount);

                            SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive,CurrencyRate,FreeQty) " +
                                 " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + InvRefNo.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[7, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Convert.ToDouble(txtNBT.Text) + "','" + Convert.ToDouble(txtVAT.Text) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + Convert.ToDouble(dgvSupInvoice[13, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','" + dgvSupInvoice[11,i].Value + "','" + PONO + "','" + txtSupName.Text.ToString() + "','" + txtSupAdd1.Text.ToString() + "','" + txtSupAdd2.Text.ToString() + "','" + Convert.ToDouble(txtDisRate1.Text.ToString()) + "','" + Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','" + Convert.ToDouble(txtDisRate.Text.ToString()) + "','" + Convert.ToDouble(dgvSupInvoice[12, i].Value) + "','" + true + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[13, i].Value) + "')", myConnection, myTrans);
                            myCommand3.ExecuteNonQuery();

                            double SellingPRice = 0.00;
                            double ItemCost = 0.00;

                            //SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                            //DataTable dt34 = new DataTable();
                            //da34.Fill(dt34);
                            //if (dt34.Rows.Count > 0)
                            //{
                            //    ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                            //}

                            //SqlCommand cmd11 = new SqlCommand("declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.Trim() + "' AND ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "') " +
                            //    "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + SupINVDate.Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) * Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + txtLocation.Text.ToString().Trim() + "','" + SellingPRice + "')", myConnection, myTrans);
                            //cmd11.ExecuteNonQuery();


                            //SqlCommand myCommand367 = new SqlCommand("update tblItemMaster set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //myCommand367.ExecuteNonQuery();

                            //myCommand367 = new SqlCommand("update tblItemWhse set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "' and WhseId='" + txtLocation.Text.Trim() + "'", myConnection, myTrans);
                            //myCommand367.ExecuteNonQuery();

                        }

                        //bool IsGRNFinished = true;

                        //for (int i = 0; i < clstGRN.Items.Count; i++)
                        //{
                        //    if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                        //    {
                        //        string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                        //        SqlCommand myCommand4 = new SqlCommand("select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                        //        da = new SqlDataAdapter(myCommand4);
                        //        DataTable dt1 = new DataTable();
                        //        da.Fill(dt1);
                        //        for (int j = 0; j < dt1.Rows.Count; j++)
                        //        {
                        //            SqlCommand myCommand5 = new SqlCommand("update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                        //            SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                        //            DataTable dt2 = new DataTable();
                        //            da1.Fill(dt2);
                        //        }
                        //    }
                        //}

                        //DataTable dtblRefNo = new DataTable();
                        //dtblRefNo = GetCheckedGRNList();
                        //clsSerializeItem.DtsSerialNoList.Rows.Clear();

                        //foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
                        //{
                        //    string SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                        //           " WHERE (tblSerialTransfer.ItemID = '" + dgvr.Cells[0].Value.ToString().Trim() + "') and Status<>'Invoiced' and   (";

                        //    for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        //    {
                        //        SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                        //        if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                        //            SSql = SSql + " or ";
                        //    }
                        //    SSql = SSql + ")";

                        //    cmd = new SqlCommand(SSql, myConnection, myTrans);
                        //    da = new SqlDataAdapter(cmd);
                        //    da.Fill(dt);

                        //    foreach (DataRow dr in dt.Tables[0].Rows)
                        //    {
                        //        if (dr["SerialNo"].ToString() != string.Empty)
                        //        {
                        //            DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();

                        //            if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                        //            {
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                        //                clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                        //            }
                        //            drow["SerialNo"] = dr["SerialNo"].ToString();
                        //            drow["Status"] = dr["Status"].ToString();
                        //            drow["ItemCode"] = dr["ItemCode"].ToString();
                        //            drow["WHCode"] = dr["WHCode"].ToString();
                        //            clsSerializeItem.DtsSerialNoList.Rows.Add(drow);
                        //        }
                        //    }
                        //    dt = new DataSet();
                        //}

                        //if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                        //{
                        //    frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Invoice", txtLocation.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDispatchDate.Value, false, "Invoiced");
                        //}

                        ////the following code3 segment export items apply to po method
                        //CreatePurchaseJXML(myTrans, myConnection);
                        //Following method export itemss tin to peachtree purchase closing stok method
                        //Connector conn = new Connector();
                        //conn.ImportSupplierInvoice();

                        //UpdateGRNs();
                        myTrans.Commit();
                        btnNew.Enabled = true;
                        btnSave.Enabled = false;
                        btnProcesss.Enabled = true;
                        btnRePrint2.Enabled = true;

                        MessageBox.Show(" Suppler Invoice is Successfuly Saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //btnReprint2_Click(sender, e);
                       // btnPrint_Click(sender, e);
                       // btnNew_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }

            }

        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intCRNNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(OpdNo) FROM tblDefualtSetting ORDER BY PoNo DESC";

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
                StrSql = "UPDATE tblDefualtSetting SET OpdNo='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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

                StrSql = "SELECT OpdPref, OpdPad, OpdNo FROM tblDefualtSetting";

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

        private void updateEvent()
        {
           
        }

        private void SaveEvent()
        {
           
        }

        private void UpdateEveent()
        {
            int rowCount = GetFilledRows();
            for (int i = 0; i < rowCount; i++)
            {
                bool Isreturn = false;
                bool IsExport = false;
                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string Dformat = "MM/dd/yyyy";
                string SupINVDate = DTP.ToString(Dformat);
       
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                //SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                StrSql = "DELETE FROM [tblSupplierInvoices] WHERE SupInvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";

                SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                command1.CommandType = CommandType.Text;
                command1.ExecuteNonQuery();

                string NoOfDis = Convert.ToString(rowCount);

                SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive) " +
                    " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + txtInvoiceNo.Text.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[5, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Tax1Amount + "','" + Tax2Amount + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + dgvSupInvoice[9, i].Value.ToString().Trim() + "','" + Po_No + "','" + txtSupName.Text.ToString() + "','" + txtSupAdd1.Text.ToString() + "','" + txtSupAdd2.Text.ToString() + "','" + Convert.ToDouble(txtDisRate1.Text.ToString()) + "','" + Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','" + Convert.ToDouble(txtDisRate.Text.ToString()) + "','" + Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','" + false + "')", myConnection, myTrans);
                myCommand3.ExecuteNonQuery();
                Disable();
                myConnection.Close();
                MessageBox.Show(" Suppler Invoice is Successfuly Updated", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        bool EditClick = false;
        
        private void btnEditer_Click(object sender, EventArgs e)
        {

            btnProcesss.Enabled = true;
            EditClick = true;
            btnEditer.Enabled = false;
            btnSave.Enabled = true;
         //   clstGRN.Enabled = true;
          //  dgvSupInvoice.ReadOnly = false;
            dgvSupInvoice.Enabled = true;
            dtpDispatchDate.Enabled = true;
            dgvSupInvoice.ReadOnly = false;
            dgvSupInvoice.Columns[0].ReadOnly = true;
            dgvSupInvoice.Columns[1].ReadOnly = true;
            dgvSupInvoice.Columns[2].ReadOnly = true;
            dgvSupInvoice.Columns[3].ReadOnly = true;
            dgvSupInvoice.Columns[8].ReadOnly = true;
            // txtLocation.Enabled = true;
            //txtSupName.Enabled = true;
            //txtSupAdd1.Enabled = true;
            //txtSupAdd2.Enabled = true;
            // txtInvoiceNo.Enabled = true;
            //cmbAR.Enabled = true;
            // txtTotalAmount.Enabled = true;
            // cmbAPAccount.Enabled = true;
            //  txtCustomerSO.Enabled = true;

            // txtTax1Amount.Enabled = true;
            // txtTax2.Enabled = true;
            // txtNetTotal.Enabled = true;
            // txtTotalAmount.Enabled = true;
            cmbtaxSys1.Enabled = true;
            cmbtaxSys2.Enabled = true;
           // cmbVendorSelect.Enabled = true;
        }

        bool btnProcessclick = false;
        private void btnProcesss_Click(object sender, EventArgs e)
        {

            if (SelectSONums1 == "")
            {
                SelectSONums1 = GRNNO;
            }
            dgvSupInvoice.CommitEdit(DataGridViewDataErrorContexts.Commit);
            btnProcessclick = true;
            //if (dtpDispatchDate.Value < user.Period_begin_Date)
            //{
            //    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //if (dtpDispatchDate.Value > user.Period_End_Date)
            //{
            //    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
            //if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

            if (IsGridValidation() == false)
            {
                return;
            }

            string TranType = "Sup-Invoice";
            int DocType = 10;
            bool QtyIN = true;
            string InvRefNo = "";
            string DuplicateSupinv = "No";
            int rowCount = GetFilledRows();

            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

            String S = "Select distinct SupInvoiceNo from tblSupplierInvoices where CustomerID='" + cmbVendorSelect.Value.ToString().Trim() + "'";// where FieldType='" + FTybpe + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                if (dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() == txtInvoiceNo.Text.ToString().Trim())
                {
                    DuplicateSupinv = "Yes";
                }
            }

            Tax1Amount = double.Parse(txtNBT.Text.Trim());
            Tax2Amount = double.Parse(txtVAT.Text.Trim());

            //==============form level validations
            if (txtTotalAmount.Text == "" || txtNetTotal.Text == "" || rowCount == 0 || txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
            {
                if (rowCount == 0)
                {
                    MessageBox.Show("Please select a GRN", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //btnSave.Focus();
                }
                else if (txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                {
                    MessageBox.Show("Please enter valid transaction", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {

                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string Dformat = "MM/dd/yyyy";
                string SupINVDate = DTP.ToString(Dformat);


                SqlConnection myConnection = new SqlConnection(ConnectionString);

                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();
               
                try
                {
                 //Connector objConnector = new Connector();
                 //if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                 //    return;

                    StrSql = "DELETE FROM [tblSupplierInvoices] WHERE SupInvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";

                    SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                    command1.CommandType = CommandType.Text;
                    command1.ExecuteNonQuery();



                    for (int i = 0; i < rowCount; i++)
                    {
                        bool Isreturn = false;
                        bool IsExport = true;
                        string[] item = clstGRN.CheckedItems[0].ToString().Split(':');
                        string AA = item[0];
                        PONO = "";

                        SqlCommand myCommand2 = new SqlCommand("Select PONos from tblGRNTran where GRN_NO = '" + AA + "'", myConnection, myTrans);
                        SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            for (int k = 0; k < dt1.Rows.Count; k++)
                            {
                                PONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                            }
                        }
                        string NoOfDis = Convert.ToString(rowCount);
                        double dblConverteQty = double.Parse(dgvSupInvoice.Rows[i].Cells[2].Value.ToString()) + double.Parse(dgvSupInvoice.Rows[i].Cells[13].Value.ToString());
                        double newCost = ((Convert.ToDouble(dgvSupInvoice.Rows[i].Cells[8].Value.ToString()) / dblConverteQty) * (100 - (Convert.ToDouble(txtDisRate.Text.Trim()))) / 100) * (100 - (Convert.ToDouble(txtDisRate1.Text.Trim()))) / 100;




                        SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive,CurrencyRate,FreeQty) " +
                                 " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[7, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Convert.ToDouble(txtNBT.Text) + "','" + Convert.ToDouble(txtVAT.Text) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + dblConverteQty + "','" + Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','" + dgvSupInvoice[11, i].Value + "','" + PONO + "','" + txtSupName.Text.ToString() + "','" + txtSupAdd1.Text.ToString() + "','" + txtSupAdd2.Text.ToString() + "','" + Convert.ToDouble(txtDisRate1.Text.ToString()) + "','" + Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','" + Convert.ToDouble(txtDisRate.Text.ToString()) + "','" + Convert.ToDouble(dgvSupInvoice[12, i].Value) + "','" + false + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[13, i].Value) + "')", myConnection, myTrans);
                        myCommand3.ExecuteNonQuery();

                        double SellingPRice = 0.00;
                        double ItemCost = 0.00;

                        SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                        DataTable dt34 = new DataTable();
                        da34.Fill(dt34);
                        if (dt34.Rows.Count > 0)
                        {
                            ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                        }


                        SqlCommand myCommand367 = new SqlCommand("update tblItemMaster set UnitCost = '" + newCost + "', Profit = '" + Convert.ToDouble(dgvSupInvoice[5, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        myCommand367.ExecuteNonQuery();


                        if (Convert.ToDouble(Convert.ToDouble(dgvSupInvoice[4, i].Value)) > 0)
                        {
                            SqlCommand myCommand368 = new SqlCommand("update tblItemMaster set PriceLevel1 = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            myCommand368.ExecuteNonQuery();
                        }

                        StrSql = "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values ('0','" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + SupINVDate.Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvSupInvoice[0, i].Value + "','" + dblConverteQty + "','" + newCost + "','" + dblConverteQty * newCost + "','" + txtLocation.Text.ToString().Trim() + "','" + SellingPRice + "')";

                        SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                    }

                    bool IsGRNFinished = true;

                    for (int i = 0; i < clstGRN.Items.Count; i++)
                    {
                        if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                        {
                            string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                            SqlCommand myCommand4 = new SqlCommand("select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                            da = new SqlDataAdapter(myCommand4);
                            DataTable dt1 = new DataTable();
                            da.Fill(dt1);
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                SqlCommand myCommand5 = new SqlCommand("update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                                DataTable dt2 = new DataTable();
                                da1.Fill(dt2);
                            }
                        }
                    }

                    DataTable dtblRefNo = new DataTable();
                    dtblRefNo = GetCheckedGRNList();



                    CreatePurchaseJXML(myTrans, myConnection);
                    //Following method export itemss tin to peachtree purchase closing stok method
                    Connector conn = new Connector();
                    conn.ImportSupplierInvoice();

                 

                    myTrans.Commit();
                    //btnNew.Enabled = true;
                    //btnSave.Enabled = false;
                    //Disable();

                    MessageBox.Show(" Suppler Invoice is Successfuly Processed", "Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  //  btnReprint2_Click(sender, e);
                   
                    btnPrint_Click(sender, e);
                    btnNew_Click(sender, e);
                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                }
                finally
                {
                    myConnection.Close();
                }
            }

        }

        private void ProcessEvent()
        {
            if (dtpDispatchDate.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dtpDispatchDate.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

            if (IsGridValidation() == false)
            {
                return;
            }

            string TranType = "Sup-Invoice";
            int DocType = 10;
            bool QtyIN = true;
            string InvRefNo = "";
            string DuplicateSupinv = "No";
            int rowCount = GetFilledRows();

            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid

            String S = "Select distinct SupInvoiceNo from tblSupplierInvoices where CustomerID='" + cmbVendorSelect.Value.ToString().Trim() + "'";// where FieldType='" + FTybpe + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                if (dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() == txtInvoiceNo.Text.ToString().Trim())
                {
                    DuplicateSupinv = "Yes";
                }
            }

            Tax1Amount = double.Parse(txtNBT.Text.Trim());
            Tax2Amount = double.Parse(txtVAT.Text.Trim());

            //==============form level validations
            if (txtTotalAmount.Text == "" || txtNetTotal.Text == "" || DuplicateSupinv == "Yes" || rowCount == 0 || txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
            {
                if (DuplicateSupinv == "Yes")
                {
                    MessageBox.Show("Supplier invoice number already entered", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //btnSave.Focus();
                }

                else if (rowCount == 0)
                {
                    MessageBox.Show("Please select a GRN", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //btnSave.Focus();
                }
                else if (txtNetTotal.Text == "0" || txtNetTotal.Text == "0.0" || txtNetTotal.Text == "0.00" || txtNetTotal.Text == "0.000" || txtNetTotal.Text == "0.0000" || txtNetTotal.Text == "0.00000")
                {
                    MessageBox.Show("Please enter valid transaction", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                if (txtInvoiceNo.Text == "")
                {
                    if (!user.IsSINVNoAutoGen)
                    {
                        MessageBox.Show("Enter Supply Invoice Number", "warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //btnSave.Focus();
                }

                //if (!IsSerialNoCorrect())
                //    return;

                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string Dformat = "MM/dd/yyyy";
                string SupINVDate = DTP.ToString(Dformat);

                //setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                //SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                try
                {
                    Connector objConnector = new Connector();
                    if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                        return;

                    if (user.IsSINVNoAutoGen)
                    {
                        SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET SupplierInvoiceNo = SupplierInvoiceNo + 1 select SupplierInvoiceNo, SupInvoicePrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                        myCommand.ExecuteNonQuery();


                        StrSql = "SELECT SupInvoicePrefix, OpdPad, SupplierInvoiceNo FROM tblDefualtSetting";

                        SqlCommand cmd25 = new SqlCommand(StrSql);
                        SqlDataAdapter da25 = new SqlDataAdapter(StrSql, ConnectionString);
                        DataTable dt25 = new DataTable();
                        da25.Fill(dt25);

                        Int32 intX;
                        Int32 intZ;
                        Int32 intP;
                        Int32 intI;
                        String StrInV;
                        if (dt25.Rows.Count > 0)
                        {
                            
                                InvRefNo = dt25.Rows[0].ItemArray[0].ToString().Trim();
                                intX = Int32.Parse(dt25.Rows[0].ItemArray[1].ToString().Trim());
                                intZ = Int32.Parse(dt25.Rows[0].ItemArray[2].ToString().Trim());

                                intP = 1;
                                for (intI = 1; intI <= intX; intI++)
                                {
                                    intP = intP * 10;
                                }
                                intP = intP + intZ;
                                StrInV = intP.ToString();

                                InvRefNo= InvRefNo + StrInV.Substring(1, intX);
                            }
                           



                        
                        txtInvoiceNo.Text = InvRefNo;
                    }
                    else
                    {
                        SqlCommand myCommand = new SqlCommand("select * from tblSupplierInvoices where SupInvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
                        SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        DataTable dt41 = new DataTable();
                        da41.Fill(dt41);

                        if (dt41.Rows.Count > 0)
                        {
                            MessageBox.Show("Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            myTrans.Rollback();
                            myConnection.Close();//
                            return;
                        }
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        bool Isreturn = false;
                        bool IsExport = false;
                        string AA = clstGRN.SelectedItems[0].ToString();
                        PONO = "";

                        SqlCommand myCommand2 = new SqlCommand("Select PONos from tblGRNTran where GRN_NO = '" + AA + "'", myConnection, myTrans);
                        SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            for (int k = 0; k < dt1.Rows.Count; k++)
                            {
                                PONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                            }
                        }
                        string NoOfDis = Convert.ToString(rowCount);

                        SqlCommand myCommand3 = new SqlCommand("insert into tblSupplierInvoices(Tax1Rate,Tax2Rate,InvReferenceNo,SupInvoiceNo,CustomerID,GRNNos,InvoiceDate,APAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Discount,Amount,DiscountAmount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerSO, Location, TTType1, TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,ConvertedQty,UOM,PONO,Ship1,Ship2,Ship3,DisCountRate1,DisCountAmount1,DisCountRate,LastPrice,IsActive) " +
                            " values ('" + txtNBTP.Text.Trim() + "','" + txtVATP.Text.Trim() + "','" + InvRefNo.Trim() + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + SupINVDate.Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + dgvSupInvoice[1, i].Value.ToString().Trim() + "','" + dgvSupInvoice[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[5, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[6, i].Value) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + Tax1Amount + "','" + Tax2Amount + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerSO.Text + "', '" + txtLocation.Text.Trim() + "', '" + Tax1Name + "', '" + Tax2Name + "','" + Isreturn + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[8, i].Value) + "','" + dgvSupInvoice[9, i].Value.ToString().Trim() + "','" + Po_No + "','" + txtSupName.Text.ToString() + "','" + txtSupAdd1.Text.ToString() + "','" + txtSupAdd2.Text.ToString() + "','" + Convert.ToDouble(txtDisRate1.Text.ToString()) + "','" + Convert.ToDouble(txtDiscountAmount1.Text.ToString()) + "','" + Convert.ToDouble(txtDisRate.Text.ToString()) + "','" + Convert.ToDouble(dgvSupInvoice[10, i].Value) + "','" + true + "')", myConnection, myTrans);
                        myCommand3.ExecuteNonQuery();

                        double SellingPRice = 0.00;
                        double ItemCost = 0.00;

                        //SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        //SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                        //DataTable dt34 = new DataTable();
                        //da34.Fill(dt34);
                        //if (dt34.Rows.Count > 0)
                        //{
                        //    ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                        //}

                        //SqlCommand cmd11 = new SqlCommand("declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.Trim() + "' AND ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "') " +
                        //    "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + SupINVDate.Trim() + "','" + TranType + "','" + QtyIN + "','" + dgvSupInvoice[0, i].Value + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + Convert.ToDouble(dgvSupInvoice[2, i].Value) * Convert.ToDouble(dgvSupInvoice[4, i].Value) + "','" + txtLocation.Text.ToString().Trim() + "','" + SellingPRice + "')", myConnection, myTrans);
                        //cmd11.ExecuteNonQuery();


                        //SqlCommand myCommand367 = new SqlCommand("update tblItemMaster set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemID='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                        //myCommand367.ExecuteNonQuery();

                        //myCommand367 = new SqlCommand("update tblItemWhse set UnitCost = '" + Convert.ToDouble(dgvSupInvoice[4, i].Value) + "' where ItemId='" + dgvSupInvoice[0, i].Value.ToString().Trim() + "' and WhseId='" + txtLocation.Text.Trim() + "'", myConnection, myTrans);
                        //myCommand367.ExecuteNonQuery();

                    }

                    //bool IsGRNFinished = true;

                    //for (int i = 0; i < clstGRN.Items.Count; i++)
                    //{
                    //    if (clstGRN.GetItemCheckState(i) == CheckState.Checked)
                    //    {
                    //        string[] GRNNo = clstGRN.Items[i].ToString().Split(':');

                    //        SqlCommand myCommand4 = new SqlCommand("select GRN_NO from tblGRNTran WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                    //        da = new SqlDataAdapter(myCommand4);
                    //        DataTable dt1 = new DataTable();
                    //        da.Fill(dt1);
                    //        for (int j = 0; j < dt1.Rows.Count; j++)
                    //        {
                    //            SqlCommand myCommand5 = new SqlCommand("update tblGRNTran SET IsGRNFinished = '" + IsGRNFinished + "' WHERE GRN_NO = '" + GRNNo[0].ToString().Trim() + "'", myConnection, myTrans);
                    //            SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                    //            DataTable dt2 = new DataTable();
                    //            da1.Fill(dt2);
                    //        }
                    //    }
                    //}

                    //DataTable dtblRefNo = new DataTable();
                    //dtblRefNo = GetCheckedGRNList();
                    //clsSerializeItem.DtsSerialNoList.Rows.Clear();

                    //foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
                    //{
                    //    string SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                    //           " WHERE (tblSerialTransfer.ItemID = '" + dgvr.Cells[0].Value.ToString().Trim() + "') and Status<>'Invoiced' and   (";

                    //    for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                    //    {
                    //        SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["GRN"].ToString().Trim() + "' ";

                    //        if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                    //            SSql = SSql + " or ";
                    //    }
                    //    SSql = SSql + ")";

                    //    cmd = new SqlCommand(SSql, myConnection, myTrans);
                    //    da = new SqlDataAdapter(cmd);
                    //    da.Fill(dt);

                    //    foreach (DataRow dr in dt.Tables[0].Rows)
                    //    {
                    //        if (dr["SerialNo"].ToString() != string.Empty)
                    //        {
                    //            DataRow drow = clsSerializeItem.DtsSerialNoList.NewRow();

                    //            if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                    //            {
                    //                clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                    //                clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                    //                clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                    //                clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                    //            }
                    //            drow["SerialNo"] = dr["SerialNo"].ToString();
                    //            drow["Status"] = dr["Status"].ToString();
                    //            drow["ItemCode"] = dr["ItemCode"].ToString();
                    //            drow["WHCode"] = dr["WHCode"].ToString();
                    //            clsSerializeItem.DtsSerialNoList.Rows.Add(drow);
                    //        }
                    //    }
                    //    dt = new DataSet();
                    //}

                    //if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                    //{
                    //    frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                    //    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Invoice", txtLocation.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDispatchDate.Value, false, "Invoiced");
                    //}

                    ////the following code3 segment export items apply to po method
                    CreatePurchaseJXML(myTrans, myConnection);
                    //Following method export itemss tin to peachtree purchase closing stok method
                    Connector conn = new Connector();
                    conn.ImportSupplierInvoice();

                    //UpdateGRNs();
                    myTrans.Commit();
                    btnNew.Enabled = true;
                    btnSave.Enabled = false;
                    Disable();

                    MessageBox.Show(" Suppler Invoice is Successfuly Saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //btnReprint2_Click(sender, e);
                    //btnPrint_Click(sender, e);
                    //btnNew_Click(sender, e);
                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    // objclsCommon.ErrorLog("Supplier Invoice", ex.Message, sender.ToString(), ex.StackTrace);
                }
                finally
                {
                    myConnection.Close();
                }
            }

        }


        //public bool IsSerialNoCorrect()
        //{
        //    try
        //    {
        //        //DataTable table = DataSet1.Tables["Orders"];
        //        int _Count = 0;
        //        // Presuming the DataTable has a column named Date. 
        //        string expression;
        //        foreach (DataGridViewRow dgvr in dgvSupInvoice.Rows)
        //        {
        //            if (dgvr.Cells[0].Value != null)
        //            {
        //                if (IsThisItemSerial(dgvr.Cells[0].Value.ToString().Trim()))
        //                {
        //                    if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
        //                    {
        //                        MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells[0].Value.ToString().Trim(), "Supplier Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        return false;
        //                    }

        //                    _Count = 0;
        //                    expression = "ItemCode = '" + dgvr.Cells[0].Value.ToString().Trim() + "'";
        //                    DataRow[] foundRows;

        //                    // Use the Select method to find all rows matching the filter.
        //                    foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

        //                    // Print column 0 of each returned row. 
        //                    for (int i = 0; i < foundRows.Length; i++)
        //                    {
        //                        _Count = i + 1;
        //                    }

        //                    if (_Count > 0 && double.Parse(dgvr.Cells[2].Value.ToString()) == 0)
        //                    {
        //                        for (int i = 0; i < foundRows.Length; i++)
        //                        {
        //                            clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
        //                        }
        //                    }

        //                    if (_Count != double.Parse(dgvr.Cells[2].Value.ToString()))
        //                    {
        //                        MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells[0].Value.ToString().Trim(), "Supplier Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        return false;
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}