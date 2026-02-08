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
    public partial class frmCustomerInvoice : Form
    {
        public static string ConnectionString;
        clsCommon objclsCommon = new clsCommon();
        public string sMsg = "Peachtree -Invoice";
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        public string StrAP = null;
        public string StrSql;
        public DSInvoice DSInvoicing = new DSInvoice();
        public static DateTime UserWiseDate = System.DateTime.Now;
        bool IsFind = false;
        public int Decimalpoint = 2;//validate for price
        public int DecimalpointQuantity = 2;//validate for quabtity

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

        public frmCustomerInvoice()
        {
            setConnectionString();
            InitializeComponent();
            IsFind = false;
        }

        public frmCustomerInvoice(string InvoiceNo)
        {
            setConnectionString();
            InitializeComponent();
            IsFind = true;
        }

        private void Load_salesOrder()
        {
            try
            {
                bool Isinvoce = false;
                String S = "Select distinct(DeliveryNoteNo),SONos from tblDispatchOrder where CustomerID='" + cmbCustomer.Value.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + " : " + dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

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
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvTaxApplicable.Rows.Add();
                        dgvTaxApplicable.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvTaxApplicable.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvTaxApplicable.Rows[i].Cells[2].Value = false;
                        dgvTaxApplicable.Rows[i].Cells[3].Value = "0";
                        dgvTaxApplicable.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvTaxApplicable.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[3].ToString().Trim();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        //***************************************************************
        public void GetCurrentUserDate()
        {
            try
            {
                dtpDispatchDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + user.userName.ToString().Trim() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpDispatchDate.Value = UserWiseDate;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        //*********************************************
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
        //====================================================================

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
                    dgvTaxApplicable.Visible = false;
                }
                else
                {
                    dgvTaxApplicable.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        //==============================================================
        private void LoadData()
        {
            try
            {
                TaxValidation();
                LoadTaxMethod();
                GetCurrentUserDate();
                dgvTaxApplicable.Rows.Clear();
                LoadtaxDetails();
                load_Decimal();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private void ClearForm()
        {
            try
            {
                cmbSalesRep.Text = "";
                rdoNonVat.Checked = true;
                rdoManulInvoice.Checked = false;
                txtInvoiceNo.Enabled = true;
                cmbCustomer.Text = "";
                txtcusName.Text = "";
                txtCusAdd1.Text = "";
                txtCusAdd2.Text = "";
                txtLocation.Text = "";
                clistbxSalesOrder.Items.Clear();
                txtInvoiceNo.Text = "";
                txtCheckSO.Text = "";
                cmbjob.Text = "";
                txtCustomerPO.Text = "";
                dgvdispactApplytoSales.Rows.Clear();
                txtTotalAmount.Text = "0.00";
                txtNetTotal.Text = "0.00";
                txtPersntage.Text = "0";
                txtValueDiscount.Text = "0.00";
                txtPersntage.Text = "0.00";
                txtDiscLineTot.Text = "0.00";
                txtSubTot.Text = "0.00";
                txtServCharges.Text="0.00";

                clistbxSalesOrder.Enabled = true;
                btnSave.Enabled = true;
                dgvTaxApplicable.Rows.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        //*********************************************************************

        public void GetCustomer()
        {
            dsCustomer = new DataSet();
            try
            {
                cmbCustomer.DataSource = null;
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

        public void GetSalesRep()
        {
            dsSalesRep = new DataSet();
            try
            {
                cmbSalesRep.DataSource = null;
                dsSalesRep.Clear();
                StrSql = " SELECT RepCode, RepName FROM tblSalesRep order by RepCode";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsSalesRep, "DtSalesRep");

                cmbSalesRep.DataSource = dsSalesRep.Tables["DtSalesRep"];
                cmbSalesRep.DisplayMember = "RepName";
                cmbSalesRep.ValueMember = "RepCode";
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
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
                cmbAR.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 150;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public void GetDefaultAPAccount()
        {
            try
            {
                StrSql = "SELECT [ARAccount] FROM tblDefualtSetting";
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
        ClassDriiDown ab = new ClassDriiDown();
        public bool flag1 = false;
        private void frmInvoicing_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsFind)
                {
                    if (flag1 == true)
                    {

                    }
                    else
                    {
                        ClearForm();
                        DatgridcolumnEnable();
                        GetCustomer();//infragistics
                        GetSalesRep();//infragistics
                        GetARAccount();//infragistics
                        GetDefaultAPAccount();//infragistics
                        LoadData();
                        rdoNonVat.Checked = true;
                    }
                    if (user.IsCINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                    else txtInvoiceNo.ReadOnly = false;                   
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
            try
            {
                if (flg == 0)
                {
                    txtTotalAmount.Text = "0.00";
                    txtNetTotal.Text = "0.00";
                    SelectSONums1 = "";
                    string SelectSONums = "";
                    dgvdispactApplytoSales.Rows.Clear();
                    int step = 0;
                    int i = 0;
                    ArraySONO.Clear();
                    

                    while (i < clistbxSalesOrder.Items.Count)
                    {
                        if (clistbxSalesOrder.GetItemChecked(i) == true)
                        {
                            step++;
                            string[] SOIDs = new string[2];
                            SOIDs = clistbxSalesOrder.Items[i].ToString().Split(':');
                            // SOIDs = clistbxSalesOrder.Items[i].ToString().Split('*');
                            string So_No = SOIDs[0].ToString();

                            string So_No1 = SOIDs[0].ToString();//saving code

                            ArraySONO.Add(So_No);
                            So_No = "'" + So_No + "'";                            

                            So_No = So_No + ",";
                            SelectSONums = SelectSONums + So_No;

                            So_No1 = So_No1 + " ";//savins purpose
                            SelectSONums1 = SelectSONums1 + So_No1;//saving purpose
                        }
                        i++;
                    }
                    if (step == 0)
                    {
                        dgvdispactApplytoSales.Rows.Clear();
                        txtSubTot.Text = "0.00";
                        txtServCharges.Text = "0.00";
                        txtPersntage.Text = "0.00";
                        txtValueDiscount.Text = "0.00";
                        txtTotalAmount.Text = "0.00";
                        txtNetTotal.Text="0.00";
                    }

                    if (SelectSONums.Length != 0)
                    {
                        SelectSONums = SelectSONums.Substring(0, SelectSONums.Length - 1);
                        DataSet ds = new DataSet();
                        ds = ReturnSOList(SelectSONums);

                        String S = "Select WareHouseID from tblDispatchOrder where DeliveryNoteNo in (" + SelectSONums + ")";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtLocation.Text = dt.Rows[0].ItemArray[0].ToString();
                        }
                        txtCustomerPO.Text = ReturnCusPO(SelectSONums);
                        cmbjob.Text = ReturnJobID(SelectSONums);
                        txtCheckSO.Text = ReturnCheckSO(SelectSONums);//load CustomerSo to Text Box                      

                        double AmountWD = 0.0;
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            dgvdispactApplytoSales.Rows.Add();
                            dgvdispactApplytoSales.Rows[j].Cells["colItem"].Value = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();
                            dgvdispactApplytoSales.Rows[j].Cells["colDescription"].Value = ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim();
                            if (DecimalpointQuantity == 0)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString();
                            }
                            if (DecimalpointQuantity == 1)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N1");
                            }
                            if (DecimalpointQuantity == 2)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N2");
                            }
                            if (DecimalpointQuantity == 3)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N3");
                            }
                            if (DecimalpointQuantity == 4)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N4");
                            }
                            if (DecimalpointQuantity == 5)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colInvQty"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]).ToString("N5");
                            }
                            dgvdispactApplytoSales.Rows[j].Cells["colGL"].Value = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();

                            if (Decimalpoint == 0)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString();
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString();
                            }
                            if (Decimalpoint == 1)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N1");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0.0";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N1");
                            }
                            if (Decimalpoint == 2)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N2");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0.00";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N2");
                            }
                            if (Decimalpoint == 3)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N3");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0.000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N3");
                            }
                            if (Decimalpoint == 4)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N4");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0.0000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N4");
                            }
                            if (Decimalpoint == 5)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N5");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = "0.00000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N5");
                            }

                            dgvdispactApplytoSales.Rows[j].Cells["SODisNo"].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                            //ssss
                            AmountWD = AmountWD + Convert.ToDouble(dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value);
                        }
                        if (Decimalpoint == 0)
                        {
                            txtTotalAmount.Text = AmountWD.ToString();
                            txtNetTotal.Text = AmountWD.ToString();
                        }
                        if (Decimalpoint == 1)
                        {
                            txtTotalAmount.Text = AmountWD.ToString("N1");
                            txtNetTotal.Text = AmountWD.ToString("N1");
                        }
                        if (Decimalpoint == 2)
                        {
                            txtTotalAmount.Text = AmountWD.ToString("N2");
                            txtNetTotal.Text = AmountWD.ToString("N2");
                        }
                        if (Decimalpoint == 3)
                        {
                            txtTotalAmount.Text = AmountWD.ToString("N3");
                            txtNetTotal.Text = AmountWD.ToString("N3");
                        }
                        if (Decimalpoint == 4)
                        {
                            txtTotalAmount.Text = AmountWD.ToString("N4");
                            txtNetTotal.Text = AmountWD.ToString("N4");
                        }
                        if (Decimalpoint == 5)
                        {
                            txtTotalAmount.Text = AmountWD.ToString("N5");
                            txtNetTotal.Text = AmountWD.ToString("N5");
                        }
                    }
                    DatgridcolumnEnable();
                    dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells[3];
                    //DgvCellEndEditEvent();
                    //CalculateTaxAmounts();
                    //CalculateDiscountforExclusive();
                    //InclusivetaxCalculation();
                    //InclusivetaxCalculation();
                    // TaxCalculation();              
                }
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        //===============================
        private void DatgridcolumnEnable()
        {
            try
            {
                dgvdispactApplytoSales.Columns[0].ReadOnly = true;
                dgvdispactApplytoSales.Columns[1].ReadOnly = true;
                dgvdispactApplytoSales.Columns[2].ReadOnly = true;
                dgvdispactApplytoSales.Columns[3].ReadOnly = true;
                dgvdispactApplytoSales.Columns[7].ReadOnly = false;
                dgvdispactApplytoSales.Columns[10].ReadOnly = false;
                dgvdispactApplytoSales.Columns[6].ReadOnly = false;

                dgvTaxApplicable.Columns[0].Visible = true;
                dgvTaxApplicable.Columns[1].Visible = true;
                dgvTaxApplicable.Columns[2].Visible = true;
                dgvTaxApplicable.Columns[3].Visible = true;
                dgvTaxApplicable.Columns[4].Visible = true;
                dgvTaxApplicable.Columns[5].Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        //========================================


        //==========================
        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();
            // bool Isinvoice = false;
            try
            {
                //SELECT ItemID, Description, SUM(DispatchQty) AS Expr1, GL_Account, UnitPrice, SODistributionNO FROM         tblDispatchOrder WHERE     (DeliveryNoteNo IN ('DRN-10000014 ')) GROUP BY ItemID, Description, GL_Account, UnitPrice, SODistributionNO, DeliveryNoteNo ORDER BY SODistributionNO
                // String S = "Select ItemID,Sum(RemainQty),Description,GLAccount,UnitPrice from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ") group by ItemID,Description,GLAccount,UnitPrice";
                //String S = "Select ItemID,Description,Sum(DispatchQty),GL_Account,UnitPrice from tblDispatchOrder where DeliveryNoteNo in (" + SO_No + ") group by ItemID,Description,GL_Account,UnitPrice";//and IsInvoce='" + Isinvoice + "'
                String S = "SELECT ItemID,Description,SUM(DispatchQty) AS Expr1, GL_Account, UnitPrice, SODistributionNO FROM tblDispatchOrder WHERE (DeliveryNoteNo IN (" + SO_No + ")) GROUP BY ItemID, Description, GL_Account, UnitPrice, SODistributionNO ORDER BY SODistributionNO";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                da.Fill(ds, "SO");
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return ds;
        }

        //=====get Customr POm To Invoice

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select CustomePO from tblDispatchOrder where DeliveryNoteNo in (" + SO_No + ")";
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
        //====================================================================
        public string ReturnJobID(string SO_No)
        {
            string Job = "";

            try
            {
                String S1 = "Select JobID from tblDispatchOrder where DeliveryNoteNo in (" + SO_No + ")";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        Job = dt1.Rows[i].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return Job;
        }
        //=====================================================================
        public string ReturnCheckSO(string SO_No)
        {
            string CheckPO = "";
            try
            {
                String S1 = "Select SONos from tblDispatchOrder where DeliveryNoteNo in (" + SO_No + ")";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        CheckPO = dt1.Rows[i].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return CheckPO;
        }       

        public string ReturnCusPO1(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select CustomePO from tblDispatchOrder where DeliveryNoteNo ='" + SO_No + "'";
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

        public string ReturnCusPO1Check(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select SONos from tblDispatchOrder where DeliveryNoteNo ='" + SO_No + "'";
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
        private void DgvCellEndEditEvent()
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
                    if (dgvdispactApplytoSales.Rows[a].Cells[1].Value != null && dgvdispactApplytoSales.Rows[a].Cells[4].Value != null)
                    {
                        DispatchQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[2].Value);
                        unitprice = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[4].Value);
                        DiscountRate = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[5].Value) / 100;

                        Amount = (DispatchQty * unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        dgvdispactApplytoSales.Rows[a].Cells[6].Value = Amount1.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells[6].Value);// sanjeewa change cell value 7 into 8
                    }
                }
                txtTotalAmount.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
                dgvTaxApplicable.CurrentCell = dgvTaxApplicable.CurrentRow.Cells[3];               
                CalculateTaxAmounts();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public double NAmountDC = 0.0;//Net Amount for discount Calculation

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
        public double TaxRate1 = 0;
        

        //==============================================GetInvoiceNo
        public string NextInvoiceNoteNo()//get the next dispatch link number
        {
            string NextInvoiceNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select InvoiceNo from tblSalesInvoices ORDER BY InvoiceNo";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    NextInvoiceNo = getNextID(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[0].ToString());
                }
                else
                {
                    NextInvoiceNo = "100000";
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

        public void UpdateDeliveryNotes(SqlTransaction tr, SqlConnection con)
        {
            bool IsInvoice = true;
            try
            {
                for (int i = 0; i < clistbxSalesOrder.Items.Count; i++)
                {                    
                    if (clistbxSalesOrder.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] D_NoteNO = clistbxSalesOrder.Items[i].ToString().Split(':');                       

                        SqlCommand myCommand4 = new SqlCommand("select DeliveryNoteNo from tblDispatchOrder WHERE DeliveryNoteNo = '" + D_NoteNO[0].ToString().Trim() + "'", con, tr);
                        SqlDataAdapter da = new SqlDataAdapter(myCommand4);
                        DataTable dt1 = new DataTable();
                        da.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            SqlCommand myCommand5 = new SqlCommand("update tblDispatchOrder SET IsInvoce = '" + IsInvoice + "' WHERE DeliveryNoteNo = '" + D_NoteNO[0].ToString().Trim() + "'", con, tr);
                            SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                            DataTable dt2 = new DataTable();
                            da1.Fill(dt2);
                        }
                    }
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

                for (int i = 0; i < dgvdispactApplytoSales.Rows.Count; i++)
                {
                    if (dgvdispactApplytoSales.Rows[i].Cells[0].Value != null) //change cell value by 1                   
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

        public void exporetSalesInvoice(SqlTransaction tr, SqlConnection con)
        {
            //Create a Xmal File..................................................................................
            string CusINvCreditAcc = "";
            string CusInvDebitAcc = "";

            SqlCommand myCommand4 = new SqlCommand("Select CusretnCrAc,CusretnDrAc from tblDefualtSetting", con, tr);
            // SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                // APAccount    = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                CusINvCreditAcc = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                CusInvDebitAcc = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            }
            //try
            //{
            //Receipts2.xml
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);
            int rowCount1 = GetFilledRows();
            string NoDistributions = Convert.ToString(rowCount1 + 3);

            for (int i = 0; i < rowCount1 + 3; i++)
            {
                if (i < rowCount1)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    // Writer.WriteString("1/7/2007");//Date 
                    Writer.WriteString(dtpDispatchDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("Apply_To_Sales_Order");
                    //Writer.WriteString("TRUE");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Sales_Order_Number");
                    //Writer.WriteString(SONO);
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("11000-00");//Cash Account
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("Accounts_Receivable_Amount");
                    Writer.WriteString("-" + txtNetTotal.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("InvoicePaid");
                    //Writer.WriteString("");//PayMethod
                    //Writer.WriteEndElement();//SalesLines

                    Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("SalesLine");

                    //Writer.WriteStartElement("Quantity");
                    //Writer.WriteString(dgvdispactApplytoSales[1, i].Value.ToString().Trim());//Doctor Charge
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Item_ID");
                    //Writer.WriteString(dgvdispactApplytoSales[0, i].Value.ToString().Trim());
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Description");
                    //Writer.WriteString(dgvdispactApplytoSales[2, i].Value.ToString().Trim());
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("0.00");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    // Writer.WriteString(dgvdispactApplytoSales[3, i].Value.ToString().Trim());
                    Writer.WriteString(CusINvCreditAcc);
                    Writer.WriteEndElement();

                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                    // double DiscountAmount = Amount * discountRate;
                    // double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Amount.ToString().Trim());//HospitalCharge
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }
                //===============================================
                if (i == rowCount1)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpDispatchDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("Apply_To_Sales_Order");
                    //Writer.WriteString("TRUE");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Sales_Order_Number");
                    //Writer.WriteString(SONO);
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("11000-00");//Cash Account
                    Writer.WriteString(cmbAR.Value.ToString().Trim());
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("InvoicePaid");
                    //Writer.WriteString("");//PayMethod
                    //Writer.WriteEndElement();//SalesLines

                    Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("SalesLine");

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

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("0.00");//Doctor Charge
                    Writer.WriteEndElement();

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

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }

                //======================tax1============================
                if (i == rowCount1 + 1)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpDispatchDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("Apply_To_Sales_Order");
                    //Writer.WriteString("TRUE");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Sales_Order_Number");
                    //Writer.WriteString(SONO);
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    // Writer.WriteString("11000-00");//Cash Account
                    Writer.WriteString(cmbAR.Value.ToString().Trim());
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();
                    
                    //Writer.WriteStartElement("InvoicePaid");
                    //Writer.WriteString("");//PayMethod
                    //Writer.WriteEndElement();//SalesLines

                    Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("SalesLine");

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

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("0.00");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //Writer.WriteString("40000-MF");
                    Writer.WriteString(Tax2GLAccount);
                    Writer.WriteEndElement();

                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    // double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                    // double DiscountAmount = Amount * discountRate;
                    // double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Tax2Amount.ToString());//tax amount1
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }

                //=======================tax2======================
                if (i == rowCount1 + 2)
                {
                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(dtpDispatchDate.Text.ToString().Trim());//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString(NoDistributions);
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("InvoicePaid");
                    //Writer.WriteString("");//PayMethod
                    //Writer.WriteEndElement();//SalesLines

                    Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("SalesLine");

                    //Writer.WriteStartElement("Quantity");
                    //Writer.WriteString("1");//Doctor Charge
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Item_ID");
                    //Writer.WriteString("MBT001");
                    //Writer.WriteEndElement();

                    //Writer.WriteStartElement("Description");
                    //Writer.WriteString("MBT");
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("0.00");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(Tax3GLAccount);
                    Writer.WriteEndElement();

                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    //  double Amount = Convert.ToDouble(dgvdispactApplytoSales[6, i].Value);
                    // double DiscountAmount = Amount * discountRate;
                    // double ItemAmount = Amount - DiscountAmount;

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Tax3Amount.ToString());//tax amount1
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }
                //==============================================
            }

            Writer.Close();

            Connector abc = new Connector();//export to peach tree
            abc.ImportSalesInvice();//ImportSalesInvice()            

        }
        //=================================================================================

        //public void exporetSalesOrderClosed()
        //{

        //    try
        //    {
        //        XmlTextWriter Writer = new XmlTextWriter(@"c:\\SalesInvice.xml", System.Text.Encoding.UTF8);
        //        Writer.Formatting = Formatting.Indented;
        //        Writer.WriteStartElement("PAW_SalesOrders");
        //        Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //        Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //        Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

        //        Writer.WriteStartElement("PAW_SalesOrder");
        //        Writer.WriteAttributeString("xsi:type", "paw:SalesOrder");

        //        Writer.WriteStartElement("Sales_Order_Number");
        //        Writer.WriteString(SONO);//Date 
        //        Writer.WriteEndElement();

        //        Writer.WriteStartElement("Is_SalesOrder_Closed");
        //        Writer.WriteString("TRUE");
        //        Writer.WriteEndElement();

        //        Writer.Close();

        //        Connector abc = new Connector();//export to peach tree
        //        abc.ExportSalesOrderClosed();
        //    }
        //    catch { }
        //}

        //=================================================================================
        public string SONO = "";

        public string Tax1ID = "";
        public double Tax1Rate = 0.0;
        public string Tax1Name = "";
        public double Tax1Amount = 0.0;
        public string Tax1GLAccount = "";

        //public string DiscountID = "";
        ////public double DiscountRate = 0.0;
        //public string DiscountName = "";
        ////public double DiscountAmount = 0.0;
        //public string DiscountGLAccount = "";


        public string Tax2ID = "";
        public double Tax2Rate = 0.0;
        public string Tax2Name = "";
        public double Tax2Amount = 0.0;
        public string Tax2GLAccount = "";

        public string Tax3ID = "";
        public double Tax3Rate = 0.0;
        public string Tax3Name = "";
        public double Tax3Amount = 0.0;
        public string Tax3GLAccount = "";

        //======Create Automatically setoff SO================================

        //Create a XML File for import Purchase Journal=====================
        public int CreateSalesJXML(SqlTransaction tr, SqlConnection con)
        {
            try
            {
                string _SalesOrderNo = "";
                double _Discount = 0;
                _Discount = double.Parse(txtDiscLineTot.Text.Trim()) + double.Parse(txtValueDiscount.Text.Trim());

                foreach (object Items in clistbxSalesOrder.CheckedItems)
                {
                    _SalesOrderNo = Items.ToString().Substring(Items.ToString().IndexOf(':')+1).Trim();
                }
              
                int _ItemsRowCount=GetFilledRows();
                int _TaxLines=0;
                if(rdoNonVat.Checked) _TaxLines=0;
                else if(rdoTax.Checked) _TaxLines=GetFilledRowstax();
                else if(rdoSVat.Checked) _TaxLines=GetFilledRowstax()-1;

                int NoDistributions = 0;

                if (txtServCharges.Text.Trim() != string.Empty && double.Parse(txtServCharges.Text.Trim()) > 0)
                {
                    NoDistributions = _ItemsRowCount + _TaxLines + 1;
                }
                else
                {
                    NoDistributions = _ItemsRowCount + _TaxLines;
                }

                if (_Discount > 0)
                {
                    NoDistributions = NoDistributions + 1;
                }

                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
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
                int rowCount1 = GetFilledRows();
                int rowCounttaxImp = GetFilledRowstax();//get filled row count from the datagrid

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbSalesRep.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteString(dtpDispatchDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString(NoDistributions.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteString(cmbAR.Value.ToString().Trim());//Cash Account
                Writer.WriteEndElement();//CreditMemoType              

                Writer.WriteStartElement("CreditMemoType");
                Writer.WriteString("FALSE");//Cash Account
                Writer.WriteEndElement();//CreditMemoType

                Writer.WriteStartElement("SalesLines");


                //write Items
                for (int i = 0; i < _ItemsRowCount; i++)               
                {
                    Writer.WriteStartElement("SalesLine");

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells[2].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells[0].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells[1].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells[3].Value.ToString());
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + dgvdispactApplytoSales.Rows[i].Cells[6].Value.ToString());
                    Writer.WriteEndElement();
                    //========================================================

                    Writer.WriteStartElement("Job_ID");
                    Writer.WriteString(cmbjob.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesOrderDistributionNumber");
                    Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells[7].Value.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Apply_To_Sales_Order");
                    Writer.WriteString("TRUE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesOrderNumber");
                    Writer.WriteString(_SalesOrderNo);
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();// end of sales line                            
                }

                if (_TaxLines > 0)
                {
                    if (rdoTax.Checked)
                    {
                        if (Tax2Amount > 0)
                        {
                            //VAT write
                            Writer.WriteStartElement("SalesLine");
                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString("");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(Tax2GLAccount);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(Tax2ID);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Description");
                            Writer.WriteString(Tax2Name);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Job_ID");
                            Writer.WriteString(cmbjob.Text.ToString().Trim());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + Tax2Amount.ToString());//HospitalCharge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Apply_To_Sales_Order");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                        }
                    }
                    //NBT Amount
                    if (Tax1Amount > 0)
                    {
                        Writer.WriteStartElement("SalesLine");
                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(Tax1GLAccount);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(Tax1ID);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(Tax1Name);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Job_ID");
                        Writer.WriteString(cmbjob.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + Tax1Amount.ToString());//HospitalCharge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                        Writer.WriteString("0");
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                    }
                }

                if (_Discount > 0)
                {
                    Writer.WriteStartElement("SalesLine");
                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(user.DiscountGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(user.DiscountItemID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString("Discount");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Job_ID");
                    Writer.WriteString(cmbjob.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();                     

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString(_Discount.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Apply_To_Sales_Order");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesOrderDistributionNumber");
                    Writer.WriteString("0");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }

                if (txtServCharges.Text.Trim() != string.Empty && double.Parse(txtServCharges.Text.Trim()) > 0)
                {
                    Writer.WriteStartElement("SalesLine");
                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString("");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("GL_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(user.ServiceChargesGL);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Item_ID");
                    Writer.WriteString(user.ServiceChargesItemID);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Description");
                    Writer.WriteString("Service Charges");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Job_ID");
                    Writer.WriteString(cmbjob.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-"+txtServCharges.Text.ToString());//HospitalCharge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Apply_To_Sales_Order");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesOrderDistributionNumber");
                    Writer.WriteString("0");
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                }
         
                Writer.WriteEndElement();//End oF the Sales lines
                Writer.WriteEndElement();//last line
                Writer.WriteEndElement();//last line
                Writer.Close();

                Connector abc = new Connector();//export to peach tree
                int ind = (abc.ImportSalesInvice());//ImportSalesInvice()
                return ind;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        //===================================================================
        private void GetTaxDeails(int rowCounttax)
        {
            try
            {
                for (int a = 0; a < rowCounttax; a++)
                {
                    if (a == 0)
                    {
                        Tax1ID = dgvTaxApplicable.Rows[a].Cells[5].Value.ToString();
                        Tax1Name = dgvTaxApplicable.Rows[a].Cells[0].Value.ToString();
                        Tax1Rate = Convert.ToDouble(dgvTaxApplicable.Rows[a].Cells[1].Value);//TaxRate
                        Tax1Amount = Convert.ToDouble(dgvTaxApplicable.Rows[a].Cells[3].Value);
                        Tax1GLAccount = dgvTaxApplicable.Rows[a].Cells[4].Value.ToString();
                    }
                    if (a == 1)
                    {
                        Tax2ID = dgvTaxApplicable.Rows[a].Cells[5].Value.ToString();
                        Tax2Rate = Convert.ToDouble(dgvTaxApplicable.Rows[a].Cells[1].Value);//TaxRate
                        Tax2Name = dgvTaxApplicable.Rows[a].Cells[0].Value.ToString();
                        Tax2Amount = Convert.ToDouble(dgvTaxApplicable.Rows[a].Cells[3].Value);
                        Tax2GLAccount = dgvTaxApplicable.Rows[a].Cells[4].Value.ToString();
                    }                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        //======================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            int INVTYPE = 1;
            if (rdoNonVat.Checked == true)
            {
                INVTYPE = 1;
            }
            if (rdoTax.Checked == true)
            {
                INVTYPE = 2;
            }
            if (rdoSVat.Checked == true)
            {
                INVTYPE = 3;
            }
            string TranType = "Invoice";
            int DocType = 10;
            bool QtyIN = false;
            int rowCount = GetFilledRows();
            if (txtLocation.Text == "")
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!user.IsCINVNoAutoGen)
            {
                if (txtInvoiceNo.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Enter Invoice No....!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            if (cmbCustomer.Value == null || cmbCustomer.Text.Trim()==string.Empty)
            {
                MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbAR.Value == null || cmbAR.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Incorrect AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbSalesRep.Value == null || cmbSalesRep.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Incorrect Sales Rep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }               
            if (rowCount == 0)
            {
                MessageBox.Show("Select a Delivery Note", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (txtNetTotal.Text == "0" || txtNetTotal.Text == "" || txtNetTotal.Text == "0.00")
            {
                MessageBox.Show("Invoice Value Can not be Zero", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ClassDriiDown.IsInvSerch = false;
            string InvNo = "";
            int rowCounttax = GetFilledRowstax();//get filled row count from the datagrid
            GetTaxDeails(rowCounttax);

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans;
            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
            bool IsExport = false;
            SONO = "";
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                    return;

                if (user.IsCINVNoAutoGen)
                {
                    SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = InvoiceNo + 1 select InvoiceNo, InvoicePrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                    DataTable dt41 = new DataTable();
                    da41.Fill(dt41);

                    if (dt41.Rows.Count > 0)
                    {
                        InvNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                        InvNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + InvNo;
                    }
                    txtInvoiceNo.Text = InvNo;
                }
                else
                {
                    SqlCommand myCommand = new SqlCommand("select * from tblSalesInvoices where InvoiceNo='" + txtInvoiceNo.Text.Trim() + "'", myConnection, myTrans);
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
                    string AA = clistbxSalesOrder.SelectedItems[0].ToString();
                    SqlCommand myCommand2 = new SqlCommand("Select SONos from tblDispatchOrder where DeliveryNoteNo = '" + AA + "'", myConnection, myTrans);
                    SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        for (int k = 0; k < dt1.Rows.Count; k++)
                        {
                            SONO = dt1.Rows[k].ItemArray[0].ToString().Trim();
                        }
                    }

                    bool isretrun = false;
                    string NoOfDis = rowCounttax.ToString();
                    //=========================================
                    double ItemCost = 0.00;
                    //============================
                    // SqlCommand cmd3 = new SqlCommand("update tblItemMaster set UnitCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "'where  ItemID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "'", myConnection, myTrans);
                    SqlCommand cmd34 = new SqlCommand("select UnitCost from tblItemMaster where ItemID='" + dgvdispactApplytoSales[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                    SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                    DataTable dt34 = new DataTable();
                    da34.Fill(dt34);
                    if (dt34.Rows.Count > 0)
                    {
                        ItemCost = Convert.ToDouble(dt34.Rows[0].ItemArray[0]);
                    }
                    //================================================

                    StrSql = "insert into tblSalesInvoices(InvoiceNo,CustomerID,DeliveryNoteNos,InvoiceDate,ARAccount,NoofDistributions,DistributionNo,ItemID,Qty,Description,GLAcount,UnitPrice,Amount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport,CustomerPO,Location,TTType1,TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,JobID,Tax1Rate,Tax2Rate,SalesRep,CostPrrice,Tax3Rate,InvType,TotalDiscountPercen,TotalDiscountAmount,LineDiscountPercentage,LineDiscountAmount) " +
                        " values ('" + txtInvoiceNo.Text.ToString().Trim() + "','" + cmbCustomer.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + dtpDispatchDate.Value.ToString("MM/dd/yyyy").Trim() + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvdispactApplytoSales[0, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" 
                        + dgvdispactApplytoSales[1, i].Value.ToString().Trim() + "','" + dgvdispactApplytoSales[3, i].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvdispactApplytoSales[4, i].Value) + "','" + Convert.ToDouble(dgvdispactApplytoSales[6, i].Value) + "','" + Tax1Amount + "','" + Tax2Amount + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" +
                        user.LoginDate.ToString("MM/dd/yyyy") + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + IsExport + "','" + txtCustomerPO.Text + "','" + txtLocation.Text.ToString().Trim() + "','" + Tax1Name + "','" + Tax2Name + "','" + isretrun + "','" + Tax3Name + "','" + Tax3Amount + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" + cmbjob.Text.ToString().Trim() + "','" +
                        Tax1Rate + "','" + Tax2Rate + "','" + cmbSalesRep.Value.ToString().Trim() + "','" + ItemCost + "','" + Tax3Rate + "','" + INVTYPE + "','" +
                        txtPersntage.Text.Trim() + "','" + txtValueDiscount.Text.Trim() + "','" + dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString() + "','" + dgvdispactApplytoSales.Rows[i].Cells["colDiscLineAmt"].Value.ToString() + "')";
                    SqlCommand cmd = new SqlCommand(StrSql,myConnection,myTrans);
                    cmd.ExecuteNonQuery();
                    //===========================================================
                    StrSql = "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.ToString().Trim() + "' AND ItemId='" + dgvdispactApplytoSales[0, i].Value + "') " +
                             "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) "+
                             " values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + GetDateTime(dtpDispatchDate.Value) + "','" + TranType + "','" + QtyIN + "','" + dgvdispactApplytoSales[0, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" + ItemCost + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) * ItemCost + "','" + txtLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "')";
                    SqlCommand cmd11 = new SqlCommand(StrSql, myConnection, myTrans);
                    cmd11.ExecuteNonQuery();
                    //=============================================================
                    //==================================================

                }
                UpdateDeliveryNotes(myTrans, myConnection);
                if (CreateSalesJXML(myTrans, myConnection) == 1)
                    myTrans.Commit();
                else
                {
                    myTrans.Rollback();
                    return;
                }

                MessageBox.Show("Customer Invoice Successfuly Saved");

                if (INVTYPE==1)
                {
                    btnPrint_Click(sender, e);
                }
                if (INVTYPE==2)
                {
                    btnReprint2_Click(sender, e);
                }
                if (INVTYPE == 3)
                {
                    btnReprintSVAT_Click(sender, e);
                }
                btnNew_Click(sender, e);
            }
            catch (Exception ex)
            {
                myTrans.Rollback();             
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //dgvdispactApplytoSales.Columns[0].Visible = false;

                ClearForm();
                LoadData();
                GetCustomer();
                GetSalesRep();

                rdoNonVat.Checked = true;

                DatgridcolumnEnable();
                ClassDriiDown.IsInvSerch = false;

                if (user.IsCINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                else txtInvoiceNo.ReadOnly = false;
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ClassDriiDown.IsInvSerch = false;
            DSInvoicing.Clear();

            try
            {
                String S12 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlConnection con12 = new SqlConnection(ConnectionString);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                da12.Fill(DSInvoicing, "DTItemMaster");
                         
                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSInvoicing, "CustomerMaster1");
                           
                String S3 = "Select * from tblSalesInvoices where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(DSInvoicing, "dtInvoiceData");

                //frmInvoicePrint prininv = new frmInvoicePrint(this);
                //prininv.Show();

                //frmDeiveryNotePrint frm = new frmDeiveryNotePrint(this);
                //frm.Show();
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                ClassDriiDown.IsInvSerch = false;
                flag1 = true;               

                if (frmMain.ObjInvoiceList == null || frmMain.ObjInvoiceList.IsDisposed)
                {
                    frmMain.ObjInvoiceList = new frmDInvoiceList(1);
                }
                frmMain.ObInvoicing.TopMost = false;
                frmMain.ObjInvoiceList.ShowDialog();
                frmMain.ObjInvoiceList.TopMost = true;
                

                //txtInvoiceNo.Text = ClassDriiDown.Invoice_No;
                //frmInvoicing_Activated(sender, e);
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnReprint2_Click(object sender, EventArgs e)
        {

            ClassDriiDown.IsInvSerch = false;
            DSInvoicing.Clear();

            try
            {
                String S12 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlConnection con12 = new SqlConnection(ConnectionString);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                da12.Fill(DSInvoicing, "DTItemMaster");

                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSInvoicing, "CustomerMaster1");

                String S3 = "Select * from tblSalesInvoices where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(DSInvoicing, "dtInvoiceData");

                //frmPrintTaxInvoice printax = new frmPrintTaxInvoice(this);
                //printax.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        
        int flg = 0;
        private void clistbxSalesOrder_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {

                //foreach (ListViewItem chkItem in clistbxSalesOrder.Items)
                //{
                //    if(chkItem.Checked)

                //}

                if (ClassDriiDown.IsInvSerch == false)
                {
                    //    //clistbxSalesOrder_SelectedIndexChanged(sender, e);
                    if (CheckState.Unchecked == e.NewValue)
                    {
                        txtCustomerPO.Text = "";
                        txtCheckSO.Text = "";
                    }
                    if (txtCheckSO.Text.Length > 0)
                    {
                        //txtTotalAmount.Text = "0";
                        int x = e.Index;
                        string[] grnno = clistbxSalesOrder.Items[x].ToString().Split(':');

                        // string CusPO = ReturnCusPO1(grnno[0].ToString());
                        string CusPO = ReturnCusPO1Check(grnno[0].ToString());
                        if (txtCheckSO.Text.Trim() == CusPO.Trim())
                        {
                            flg = 0;
                        }
                        else
                        {
                            MessageBox.Show("You cannot select GRN's from different Customer SO's");
                            e.NewValue = CheckState.Unchecked;
                            flg = 1;
                        }
                    }
                    else
                    {
                        flg = 0;
                    }
                }
                if (e.NewValue == CheckState.Checked)
                {
                    cmbSalesRep.Focus();
                    //cmbSalesRep.PerformAction(Infragistics.Win.UltraWinGrid.UltraComboAction.Dropdown, false, false);
                }
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        public int GetFilledRowstax()
        {
            try
            {
                int RowCounttax = 0;
                for (int i = 0; i < dgvTaxApplicable.Rows.Count; i++)
                {
                    if (dgvTaxApplicable.Rows[i].Cells[1].Value != null) //change cell value by 1                   
                    {
                        if (double.Parse(dgvTaxApplicable.Rows[i].Cells[1].Value.ToString())!=0)
                            RowCounttax++;
                    }
                }
                return RowCounttax;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        

        private void CalculateTaxAmounts()
        {
            double _Subotal = 0.00;
            double _Tax1Amount = 0;
            double _Tax2Amount = 0;
            try
            {
                if (dgvTaxApplicable.Rows.Count < 2) return;
                _Subotal = double.Parse(txtTotalAmount.Text.Trim()) - double.Parse(txtValueDiscount.Text.Trim());

                if (dgvTaxApplicable.Rows[0].Cells["Amount"].Value == null) dgvTaxApplicable.Rows[0].Cells["Amount"].Value = "0.00";
                if (dgvTaxApplicable.Rows[1].Cells["Amount"].Value == null) dgvTaxApplicable.Rows[1].Cells["Amount"].Value = "0.00";
                if (dgvTaxApplicable.Rows[0].Cells["Rate"].Value == null) dgvTaxApplicable.Rows[0].Cells["Rate"].Value = "0.00";
                if (dgvTaxApplicable.Rows[1].Cells["Rate"].Value == null) dgvTaxApplicable.Rows[1].Cells["Rate"].Value = "0.00";
                if (dgvTaxApplicable.Rows[0].Cells["IsTax"].Value == null) dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = false;
                if (dgvTaxApplicable.Rows[1].Cells["IsTax"].Value == null) dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = false;

                if (bool.Parse(dgvTaxApplicable.Rows[0].Cells["IsTax"].Value.ToString()))
                {
                    dgvTaxApplicable.Rows[0].Cells["Amount"].Value = double.Parse(dgvTaxApplicable.Rows[0].Cells["Rate"].Value.ToString()) * _Subotal/100;
                    _Tax1Amount = double.Parse(dgvTaxApplicable.Rows[0].Cells["Amount"].Value.ToString());
                }
                if (bool.Parse(dgvTaxApplicable.Rows[1].Cells["IsTax"].Value.ToString()))
                {
                    dgvTaxApplicable.Rows[1].Cells["Amount"].Value = double.Parse(dgvTaxApplicable.Rows[1].Cells["Rate"].Value.ToString()) * (_Subotal+_Tax1Amount)/100;
                    _Tax2Amount = double.Parse(dgvTaxApplicable.Rows[1].Cells["Amount"].Value.ToString());
                }

                if (rdoTax.Checked)
                {
                    txtNetTotal.Text = (_Subotal + _Tax1Amount + _Tax2Amount).ToString("0.00");
                }
                else if (rdoSVat.Checked)
                {
                    txtNetTotal.Text = (_Subotal + _Tax1Amount).ToString("0.00");
                }
                else
                {
                    txtNetTotal.Text = (_Subotal).ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvTaxApplicable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (bool.Parse(dgvTaxApplicable.Rows[0].Cells["IsTax"].Value.ToString()))
                    dgvTaxApplicable.Rows[0].Cells["Rate"].ReadOnly = false;
                else
                {
                    dgvTaxApplicable.Rows[0].Cells["Rate"].ReadOnly = true;
                    dgvTaxApplicable.Rows[0].Cells["Rate"].Value = "0.00";
                }

                if (bool.Parse(dgvTaxApplicable.Rows[1].Cells["IsTax"].Value.ToString()))
                    dgvTaxApplicable.Rows[1].Cells["Rate"].ReadOnly = false;
                else
                {
                    dgvTaxApplicable.Rows[1].Cells["Rate"].ReadOnly = true;
                    dgvTaxApplicable.Rows[1].Cells["Rate"].Value = "0.00";
                }

                //foreach (DataGridViewRow dgvr in dgvTaxApplicable.Rows)
                //{
                //    if (bool.Parse(dgvr.Cells["IsTax"].Value.ToString()))
                //        dgvr.Cells["Rate"].ReadOnly = false;
                //    else
                //        dgvr.Cells["Rate"].ReadOnly = true;
                //}
                CalculateGridAmounts();                
            }           
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        public static double DiscountAmount = 0.0;

        private void frmInvoicing_Activated(object sender, EventArgs e)
        {
            try
            {
                FillDetails();
            }            
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void FillDetails()
        {
            if (ClassDriiDown.IsInvSerch == true)
            {
                //clistbxSalesOrder.Items.Clear();
                clistbxSalesOrder.Enabled = false;

                string SerchText = ab.GetText1();
                string ConnString = ConnectionString;
                String S1 = "Select * from tblSalesInvoices where InvoiceNo='" + SerchText + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    clistbxSalesOrder.Items.Clear();
                    clistbxSalesOrder.Items.Add(dt.Rows[0].ItemArray[2].ToString().Trim(), CheckState.Checked);
                    txtInvoiceNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbCustomer.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    cmbjob.Text = dt.Rows[0].ItemArray[25].ToString().Trim();

                    string abc = dt.Rows[0].ItemArray[2].ToString().Trim();
                    dtpDispatchDate.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    cmbAR.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                    txtTotalAmount.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                    //txtTotalAmount.Text = dt.Rows[i].ItemArray[18].ToString().Trim();
                    txtNetTotal.Text = dt.Rows[0].ItemArray[18].ToString().Trim();
                    txtCustomerPO.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                    txtLocation.Text = dt.Rows[0].ItemArray[27].ToString().Trim();//Loacation

                    if (Convert.ToInt32(dt.Rows[0].ItemArray[47]) == 1)
                    {
                        rdoNonVat.Checked = true;
                    }
                    if (Convert.ToInt32(dt.Rows[0].ItemArray[47]) == 2)
                    {
                        rdoTax.Checked = true;
                    }
                    if (Convert.ToInt32(dt.Rows[0].ItemArray[47]) == 3)
                    {
                        rdoSVat.Checked = true;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        dgvTaxApplicable.Rows.Add();
                        if (k == 0)
                        {
                            dgvTaxApplicable.Rows[0].Cells[0].Value = dt.Rows[0]["TTType1"].ToString();
                            dgvTaxApplicable.Rows[0].Cells[1].Value = dt.Rows[0]["Tax1Rate"].ToString();
                            dgvTaxApplicable.Rows[0].Cells[2].Value = true;
                            dgvTaxApplicable.Rows[0].Cells[3].Value = dt.Rows[0]["Tax1Amount"].ToString();
                        }

                        if (k == 1)
                        {
                            dgvTaxApplicable.Rows[1].Cells[0].Value = dt.Rows[0]["TTType2"].ToString();
                            dgvTaxApplicable.Rows[1].Cells[1].Value = dt.Rows[0]["Tax2Rate"].ToString();
                            dgvTaxApplicable.Rows[1].Cells[2].Value = true;
                            dgvTaxApplicable.Rows[1].Cells[3].Value = dt.Rows[0]["Tax2Amount"].ToString();
                        }
                        if (k == 2)
                        {
                            dgvTaxApplicable.Rows[2].Cells[0].Value = dt.Rows[0]["TTType3"].ToString();
                            dgvTaxApplicable.Rows[2].Cells[1].Value = dt.Rows[0]["Tax3Rate"].ToString();
                            dgvTaxApplicable.Rows[2].Cells[2].Value = true;
                            dgvTaxApplicable.Rows[2].Cells[3].Value = dt.Rows[0]["Tax3Amount"].ToString();
                        }
                        //dgvTaxApplicable.Columns[2].Visible = false;
                        //dgvTaxApplicable.Columns[0].Visible = false;
                        dgvTaxApplicable.Columns[4].Visible = false;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvdispactApplytoSales.Rows.Add();
                        dgvdispactApplytoSales.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[11]).ToString("N2");
                        dgvdispactApplytoSales.Rows[i].Cells[5].Value = Convert.ToDouble(dt.Rows[i].ItemArray[12]).ToString("N2");
                        dgvdispactApplytoSales.Rows[i].Cells[6].Value = Convert.ToDouble(dt.Rows[i].ItemArray[13]).ToString("N2");
                    }
                }

                btnSave.Enabled = false;
                dgvTaxApplicable.Enabled = false;
            }
        }

        private void txtValueDiscount_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtTotalAmount.Text.Trim() != string.Empty && double.Parse(txtTotalAmount.Text.Trim())>0)
                    txtPersntage.Text = (double.Parse(txtValueDiscount.Text.Trim()) * 100 / double.Parse(txtTotalAmount.Text.Trim())).ToString("0.00");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoManulInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoManulInvoice.Checked == true)
            {
                txtInvoiceNo.Enabled = true;
            }
            else
            {
                txtInvoiceNo.Enabled = false;
            }
        }

        private void cmbCustomer_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtCheckSO.Text = "";
                        clistbxSalesOrder.Items.Clear();
                        txtcusName.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtCusAdd1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                        txtCusAdd2.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                        clistbxSalesOrder.Items.Clear();
                        Load_salesOrder();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnReprintSVAT_Click(object sender, EventArgs e)
        {
            ClassDriiDown.IsInvSerch = false;
            DSInvoicing.Clear();
            try
            {
                String S12 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlConnection con12 = new SqlConnection(ConnectionString);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                da12.Fill(DSInvoicing, "DTItemMaster");

                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSInvoicing, "CustomerMaster1");

                String S3 = "Select * from tblSalesInvoices where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(DSInvoicing, "dtInvoiceData");

                //frmSVATPrint ObjSVatP = new frmSVATPrint(this);
                //ObjSVatP.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvdispactApplytoSales_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvdispactApplytoSales.CurrentCell == null) return;

                int rowCount = GetFilledRows();
                NAmountDC = 0.0;
                double DispatchQty = 0.00;
                double unitprice = 0.00;
                double unitprice_Incl = 0.00;
                double DiscountRate = 0.00;
                double DiscountAmount = 0.00;
                double Amount = 0.00;
                double Amount1 = 0.00;
                double TotalAmount = 0.00;
                double TotalDiscAmount = 0.00;
                GetTaxDeails(GetFilledRowstax());

                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvdispactApplytoSales.Rows[a].Cells["colItem"].Value != null && dgvdispactApplytoSales.Rows[a].Cells["colInvQty"].Value != null)
                    {
                        if (rbtExclusive.Checked)
                        {
                            DispatchQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colInvQty"].Value);
                            unitprice = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colUnitPrice"].Value);
                            DiscountRate = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["Discount"].Value) / 100;
                            Amount = (DispatchQty * unitprice);
                            TotalAmount = TotalAmount + Amount;
                            DiscountAmount = Amount * DiscountRate;
                            Amount1 = Amount - DiscountAmount;
                            dgvdispactApplytoSales.Rows[a].Cells["colDiscLineAmt"].Value = DiscountAmount.ToString("0.00");
                            dgvdispactApplytoSales.Rows[a].Cells["colAmount"].Value = Amount1.ToString("0.00");
                            TotalDiscAmount = TotalDiscAmount + Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colDiscLineAmt"].Value);
                            unitprice_Incl = unitprice + (unitprice * Tax1Amount / 100);
                            unitprice_Incl = unitprice_Incl + (unitprice_Incl * Tax2Amount / 100);
                            dgvdispactApplytoSales.Rows[a].Cells["colInclUnitPrice"].Value = unitprice_Incl;
                        }
                    }
                }

                txtDiscLineTot.Text = TotalDiscAmount.ToString();
                txtSubTot.Text = Amount1.ToString("N2");
                txtTotalAmount.Text = (Amount1+double.Parse(txtServCharges.Text.Trim())).ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
               
                CalculateTaxAmounts();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvTaxApplicable_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvTaxApplicable.IsCurrentCellDirty)
                {
                    dgvTaxApplicable.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvdispactApplytoSales_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvdispactApplytoSales.IsCurrentCellDirty)
                {
                    dgvdispactApplytoSales.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtLocation.Text.ToString().Trim() != string.Empty)
                {
                    String S = "Select * from tblWhseMaster where WhseId='" + txtLocation.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    cmbAR.Text = dt.Tables[0].Rows[0]["ArAccount"].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoNonVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    dgvTaxApplicable.ReadOnly = true;
                    if (dgvTaxApplicable.Rows.Count > 1)
                    {
                        dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = false;
                        dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = false;
                    }
                }
                else dgvTaxApplicable.ReadOnly = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoTax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTax.Checked || rdoSVat.Checked)
                {
                    dgvTaxApplicable.ReadOnly = false;
                    if (dgvTaxApplicable.Rows.Count > 1)
                    {
                        dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = true;
                        dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = true;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void CalculateGridAmounts()
        {
            double _UnitPrice = 0;
            double _UnitPriceIncl = 0;
            double _LineTax = 0;
            double _DiscountP = 0;
            double _LineDiscount = 0;
            double _AmountIncl=0;
            double _Amount=0;
            double _Qty=0;
            double _TempTax1 = 0;
            double _TempTax2 = 0;
            double _DiscountTotal = 0;
            double _Tax1Total = 0;
            double _Tax2Total = 0;

            try
            {
                if (GetFilledRowstax() > 0)
                {
                    GetTaxDeails(GetFilledRowstax());

                    if (rbtExclusive.Checked)
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            if (dgvr.Cells["colItem"].Value != null && dgvr.Cells["colItem"].Value.ToString() != string.Empty)
                            {
                                if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString() == string.Empty) dgvr.Cells["colInvQty"].Value = "0";
                                if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["colUnitPrice"].Value = "0";
                                if (dgvr.Cells["colInclUnitPrice"].Value == null || dgvr.Cells["colInclUnitPrice"].Value.ToString() == string.Empty) dgvr.Cells["colInclUnitPrice"].Value = "0";
                                if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString() == string.Empty) dgvr.Cells["Discount"].Value = "0";
                                if (dgvr.Cells["colAmount"].Value == null || dgvr.Cells["colAmount"].Value.ToString() == string.Empty) dgvr.Cells["colAmount"].Value = "0";
                                if (dgvr.Cells["colDiscLineAmt"].Value == null || dgvr.Cells["colDiscLineAmt"].Value.ToString() == string.Empty) dgvr.Cells["colDiscLineAmt"].Value = "0";
                                if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString() == string.Empty) dgvr.Cells["colAmountIncl"].Value = "0";
                                if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString() == string.Empty) dgvr.Cells["colLineTax"].Value = "0";

                                _Qty = double.Parse(dgvr.Cells["colInvQty"].Value.ToString());
                                _UnitPrice = double.Parse(dgvr.Cells["colUnitPrice"].Value.ToString());
                                _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                                _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;
                                _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                                _TempTax1 = _Amount * Tax1Rate / 100;
                                _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                                _LineTax = _TempTax1 + _TempTax2;

                                dgvr.Cells["colInclUnitPrice"].Value = (_UnitPrice + _LineTax).ToString("0.00");
                                dgvr.Cells["colDiscLineAmt"].Value = _LineDiscount.ToString("0.00");
                                dgvr.Cells["colAmount"].Value = (_Amount).ToString("0.00");
                                dgvr.Cells["colLineTax"].Value = (_LineTax).ToString("0.00");
                                dgvr.Cells["colAmountIncl"].Value = (_Amount + _LineTax).ToString("0.00");

                                _Tax1Total = _Tax1Total + _TempTax1;
                                _Tax2Total = _Tax2Total + _TempTax2;
                            }
                        }
                        dgvTaxApplicable.Rows[0].Cells["Amount"].Value = _Tax1Total;
                        dgvTaxApplicable.Rows[1].Cells["Amount"].Value = _Tax2Total;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void rdoSVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTax.Checked || rdoSVat.Checked)
                {
                    if (dgvTaxApplicable.Rows.Count > 1)
                    {
                        dgvTaxApplicable.ReadOnly = false;
                        dgvTaxApplicable.Rows[0].Cells["IsTax"].Value = true;
                        dgvTaxApplicable.Rows[1].Cells["IsTax"].Value = true;
                        //dgvTaxApplicable.Rowsp
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtPersntage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPersntage.Text == string.Empty) txtPersntage.Text = "0.00";
                if (txtTotalAmount.Text == string.Empty) txtTotalAmount.Text = "0.00";

                txtValueDiscount.Text = (double.Parse(txtTotalAmount.Text.Trim()) * double.Parse(txtPersntage.Text.Trim()) / 100).ToString("0.00");

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtValueDiscount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtTotalAmount.Text.Trim() != string.Empty && double.Parse(txtTotalAmount.Text.Trim())>0)
                {
                    //txtPersntage.Text = (double.Parse(txtValueDiscount.Text.Trim()) * 100 / double.Parse(txtTotalAmount.Text.Trim())).ToString("0.00");
                    CalculateTaxAmounts();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtServCharges_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtTotalAmount.Text = (double.Parse(txtSubTot.Text.Trim()) + double.Parse(txtServCharges.Text.Trim())).ToString("0.00");
                CalculateTaxAmounts();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtServCharges_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtServCharges.Text.Trim() != string.Empty && double.Parse(txtServCharges.Text.Trim()) > 0)
                    txtServCharges.Text = double.Parse(txtServCharges.Text.Trim()).ToString("0.00");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtInclusive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rbtExclusive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvdispactApplytoSales_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CalculateGridAmounts();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvTaxApplicable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvTaxApplicable.CurrentCell == null) return;
                CalculateGridAmounts();              
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvTaxApplicable_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ArrangeTaxGrid()
        {
            try
            {
//TaxName
//Rate
//IsTax
//Amount
                //foreach(DataGridViewRow dgvr in dgvTaxApplicable.Rows)
                //{
                //    if(dgvr.Cells[].Value)
                //    {
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
    }
}