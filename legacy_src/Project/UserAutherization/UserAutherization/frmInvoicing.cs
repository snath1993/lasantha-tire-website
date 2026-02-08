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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Text.RegularExpressions;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmInvoicing : Form
    {
        public static string ConnectionString;
        public string StrPaymmetM;
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon();
        public string sMsg = "Warehouse Module -Invoice";
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        private bool selectionSet;
        public bool PayValChecked;
        public Boolean IsGLok = false;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        public string StrAP = null;
        public string StrRep = null;
        public string StrSql;
        public string StrSql1;
        public string StrSqlA = null;        
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();        
        public static DateTime UserWiseDate = System.DateTime.Now;
        bool IsFind = false;
        bool Isok = false;
        bool ApplySO = true;
        public int Decimalpoint = 2;//validate for price
        public int DecimalpointQuantity = 2;//validate for quabtity
        public int InvoiceType = 1;
        public int TaxINCType = 1;
        public int PosType=0;
        bool IsExport = false;
        public bool isok;
        public int CurrentInvType;
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

        public frmInvoicing()
        {
            setConnectionString();
            InitializeComponent();
            IsFind = false;
            //this.txtPersntage.PreviewKeyDown

        }         

        public frmInvoicing(string InvoiceNo)
        {
            setConnectionString();
            InitializeComponent();
            IsFind = true;
            txtInvoiceNo.Text = InvoiceNo;
            viewinvpayhistory(txtInvoiceNo.Text.ToString());
            LoadCreditcardData();            
            viewCardHistory(txtInvoiceNo.Text.ToString());
            PaycardCalculation();
        }

        private void Load_salesOrder()
        {
            try
            {
                //asanga
                String SSo = "select SalesWithDiliveryOrder from tblDefualtSetting";
                SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
                DataTable dtSo = new DataTable();
                daSo.Fill(dtSo);                
                if (dtSo.Rows.Count > 0 && dtSo.Rows[0].ItemArray[0].ToString().Trim() != "")
                {   
                    Isok  = Convert.ToBoolean(dtSo.Rows[0].ItemArray[0].ToString().Trim());
                    if (Isok == true)
                    {
                        ApplySO = true;
                    }
                    else if (Isok == false)
                    {
                        ApplySO = false ;
                    }
                }
                if (ApplySO == true)
                {
                    bool IsfullDispatch = false;
                    String S = "Select distinct(SalesOrderNo) from tblSalesOrderTemp where CustomerID='" + cmbCustomer.Value.ToString().Trim() + "' and IsfullDispatch='" + IsfullDispatch + "' order by SalesOrderNo";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
                else
                {
                    bool Isinvoce = false;
                    String S = "Select distinct(DeliveryNoteNo),SONos from tblDispatchOrder where CustomerID='" + cmbCustomer.Value.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "' order by DeliveryNoteNo";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + " : " + dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim());
                    }
                }
                GetTaxDeails();
                
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

        public void GetCurrentUserDate()
        {
            try
            {
                dtpDispatchDate.Value = user.LoginDate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadData()
        {
            try
            {
                GetCurrentUserDate();
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
                cmbInvoiceType.Value = 1;
                cmbSalesRep.Text = "";
                rdoTax.Checked = true;
                //rbtInclusive.Checked = true;
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
                txtGridTotalExcl.Text = "0.00";
                txtNetTotal.Text = "0.00";
                txtPersntage.Text = "0";
                txtValueDiscount.Text = "0.00";
                txtPersntage.Text = "0.00";
                txtDiscLineTot.Text = "0.00";
                txtSubTot.Text = "0.00";
                txtServCharges.Text = "0.00";
                txtpaid.Text = "0.00";
                txtbalance.Text = "0.00";
                grdviewdata.Rows.Clear(); 
                clistbxSalesOrder.Enabled = true;
                btnSave.Enabled = true;
                txtcardamount.Text = "0.00";
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                grdviewdata.Rows.Clear();
                grpamounttypes.Enabled = false;
                optCredit.Checked = true;
                
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

        public void GetDeafultRep()
        {
            try
            {
                StrSql = "SELECT [Repcode] FROM tblSalesRep where Repcode='GEN'";
                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrRep = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbSalesRep.Value = StrRep.Trim();
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
                        //GetDeafultRep();//infragistics
                        LoadData();
                        CheckGL();
                        loadDefaltOption();
                        rdoNonVat.Checked = true;
                        //cmbInvoiceType.Text = "Exclusive";
                    }
                    if (user.IsCINVNoAutoGen) txtInvoiceNo.ReadOnly = false;
                    else txtInvoiceNo.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void LoadCreditcardData()
        {
            try
            {
                Card_Name.Items.Clear();
                String S = "Select CardType from tblCreditData order by CardType";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                dt.Clear();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    Card_Name.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
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
                    txtGridTotalExcl.Text = "0.00";
                    txtNetTotal.Text = "0.00";
                    txtCustomerPO.Text = "";
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
                        txtGridTotalExcl.Text = "0.00";
                        txtNetTotal.Text = "0.00";
                    }

                    if (SelectSONums.Length != 0)
                    {
                        SelectSONums = SelectSONums.Substring(0, SelectSONums.Length - 1);
                        DataSet ds = new DataSet();
                        ds = ReturnSOList(SelectSONums);

                        if (ApplySO == true)
                        {
                            String S = "Select WareHouseID,DisPer,nbtper,IsInclusive,SalesRep,Remarks from tblSalesOrderTemp where SalesOrderNo in (" + SelectSONums + ")";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                txtLocation.Text = dt.Rows[0].ItemArray[0].ToString();
                                txtPersntage.Text = double.Parse(dt.Rows[0]["DisPer"].ToString()).ToString("0.00"); 
                                txtNBTP.Text = double.Parse(dt.Rows[0]["nbtper"].ToString()).ToString("0.00");
                                CurrentInvType = Convert.ToInt16(dt.Rows[0].ItemArray[3].ToString());
                                cmbInvoiceType.Value=dt.Rows[0].ItemArray[3].ToString(); //IsInclusive 
                                cmbSalesRep.Value = dt.Rows[0].ItemArray[4].ToString();
                               // txtNBTP.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                                //txtVATP.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");
                                txtDescription.Text  = dt.Rows[0].ItemArray[5].ToString();
                            }

                        }
                        else if (ApplySO == false)
                        {
                            String S = "Select WareHouseID from tblDispatchOrder where DeliveryNoteNo in (" + SelectSONums + ")";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                txtLocation.Text = dt.Rows[0].ItemArray[0].ToString();
                            }
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
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString() ; //"0";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString();
                            }
                            if (Decimalpoint == 1)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N1");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N1");  //"0.0";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0.0";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0.0";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N1");
                            }
                            if (Decimalpoint == 2)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N2");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N2"); //"0.00";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0.00";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0.00";  
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N2");
                            }
                            if (Decimalpoint == 3)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N3");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N3"); //"0.000";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0.000";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0.000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N3");
                            }
                            if (Decimalpoint == 4)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N4");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N4"); //"0.0000";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0.0000";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0.0000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N4");
                            }
                            if (Decimalpoint == 5)
                            {
                                dgvdispactApplytoSales.Rows[j].Cells["colUnitPrice"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4]).ToString("N5");
                                dgvdispactApplytoSales.Rows[j].Cells["Discount"].Value = Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[6]).ToString("N5");// "0.00000";
                                dgvdispactApplytoSales.Rows[j].Cells["colLineTax"].Value = "0.00000";
                                dgvdispactApplytoSales.Rows[j].Cells["colDiscLineAmt"].Value = "0.00000";
                                dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value = (Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[2]) * Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray[4])).ToString("N5");
                            }

                            dgvdispactApplytoSales.Rows[j].Cells["SODisNo"].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();                           
                            AmountWD = AmountWD + Convert.ToDouble(dgvdispactApplytoSales.Rows[j].Cells["colAmount"].Value);
                        }
                        if (Decimalpoint == 0)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString();
                            txtNetTotal.Text = AmountWD.ToString();
                        }
                        if (Decimalpoint == 1)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString("N1");
                            txtNetTotal.Text = AmountWD.ToString("N1");
                        }
                        if (Decimalpoint == 2)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString("N2");
                            txtNetTotal.Text = AmountWD.ToString("N2");
                        }
                        if (Decimalpoint == 3)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString("N3");
                            txtNetTotal.Text = AmountWD.ToString("N3");
                        }
                        if (Decimalpoint == 4)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString("N4");
                            txtNetTotal.Text = AmountWD.ToString("N4");
                        }
                        if (Decimalpoint == 5)
                        {
                            txtGridTotalExcl.Text = AmountWD.ToString("N5");
                            txtNetTotal.Text = AmountWD.ToString("N5");
                        }
                    }
                    DatgridcolumnEnable();


                    cmbInvoiceType_ValueChanged(sender, e);

                    this.txtpaid.Focus();
                     
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ValidateInvoiceSetting()
        {
            String S1 = "Select Locked from tblTax_Default where flg ='PRL' and UserName='" + user.userName.ToString().Trim() + "' ";
            SqlCommand cmd = new SqlCommand(S1);
            SqlDataAdapter da = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Boolean Isok = false;
            if (dt.Rows.Count > 0)
            {
                Isok = bool.Parse(dt.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    dgvdispactApplytoSales.Columns[4].ReadOnly = true;
                }
            }
            String S2 = "Select Locked from tblTax_Default where flg ='QTY' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S2);
            SqlDataAdapter da1 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                Isok = bool.Parse(dt1.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    dgvdispactApplytoSales.Columns[2].ReadOnly = true;
                }
            }
            String S3 = "Select Locked from tblTax_Default where flg ='DST' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd2 = new SqlCommand(S3);
            SqlDataAdapter da2 = new SqlDataAdapter(S3, ConnectionString);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            if (dt2.Rows.Count > 0)
            {
                Isok = bool.Parse(dt2.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    dgvdispactApplytoSales.Columns[6].ReadOnly = true;
                }
            }
        }
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
                dgvdispactApplytoSales.Columns[9].ReadOnly = false;
                ValidateInvoiceSetting();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();
            // bool Isinvoice = false;
            try
            {
                if (ApplySO == true)
                {
                    String S = "Select ItemID,Description,SUM(Quantity) AS Expr1, GLAccount, UnitPrice, DisNumber,Linedisper from tblSalesOrderTemp where  (SalesOrderNo IN (" + SO_No + ")) GROUP BY ItemID,Description, GLAccount, UnitPrice, DisNumber,Linedisper ORDER BY DisNumber";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    da.Fill(ds, "SO");
                }
                else if (ApplySO == false)
                {
                    String S = "SELECT ItemID,Description,SUM(DispatchQty) AS Expr1, GL_Account, UnitPrice, SODistributionNO,Linedisper FROM tblDispatchOrder WHERE (DeliveryNoteNo IN (" + SO_No + ")) GROUP BY ItemID, Description, GL_Account, UnitPrice, SODistributionNO,Linedisper ORDER BY SODistributionNO";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    da.Fill(ds, "SO");
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
                if (ApplySO == false )
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
                else if (ApplySO == true  )
                {
                    String S = "Select CustomerPO from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            CusPO = dt.Rows[i].ItemArray[0].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }

        public string ReturnJobID(string SO_No)
        {
            string Job = "";

            try
            {
                if (ApplySO == false )
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
                else if (ApplySO == true  )
                {
                    String S = "Select JobID from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Job = dt.Rows[i].ItemArray[0].ToString();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Job;
        }

        public string ReturnCheckSO(string SO_No)
        {
            string CheckPO = "";
            try
            {
                if (ApplySO == true)
                {
                    String S = "Select CustomerPO from tblSalesOrderTemp where SalesOrderNo in (" + SO_No + ")";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            CheckPO = dt.Rows[i].ItemArray[0].ToString();
                        }
                    }
                }
                else if (ApplySO == false)
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
                txtGridTotalExcl.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
                CalculateTaxAmounts();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double NAmountDC = 0.0;//Net Amount for discount Calculation              

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
                if (ApplySO == true)
                {
                    for (int i = 0; i < clistbxSalesOrder.Items.Count; i++)
                    {
                        if (clistbxSalesOrder.GetItemCheckState(i) == CheckState.Checked)
                        {
                            string[] D_NoteNO = clistbxSalesOrder.Items[i].ToString().Split(':');

                            SqlCommand myCommand4 = new SqlCommand("select SalesOrderNo from tblSalesOrderTemp WHERE SalesOrderNo = '" + D_NoteNO[0].ToString().Trim() + "'", con, tr);
                            SqlDataAdapter da = new SqlDataAdapter(myCommand4);
                            DataTable dt1 = new DataTable();
                            da.Fill(dt1);
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                SqlCommand myCommand5 = new SqlCommand("update tblSalesOrderTemp SET IsfullDispatch = '" + IsInvoice + "' WHERE SalesOrderNo = '" + D_NoteNO[0].ToString().Trim() + "'", con, tr);
                                SqlDataAdapter da1 = new SqlDataAdapter(myCommand5);
                                DataTable dt2 = new DataTable();
                                da1.Fill(dt2);
                            }
                        }
                    }
                }
                else if (ApplySO == false)
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
        private void CheckGL()
        {
            string StrSql1 = "SELECT GL_True from  tblDefualtSetting";
            SqlCommand cmd1 = new SqlCommand(StrSql1);
            SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                IsGLok = Convert.ToBoolean(dt1.Rows[0].ItemArray[0].ToString().Trim());
            }
        }
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

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }

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

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }

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


                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }

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

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }

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
                double _Amount = 0;
                double _AmountTot = 0;
                double _DiscountTotLine = 0;

                _Discount = double.Parse(txtDiscLineTot.Text.Trim()) + double.Parse(txtValueDiscount.Text.Trim());

                foreach (object Items in clistbxSalesOrder.CheckedItems)
                {
                    _SalesOrderNo = Items.ToString().Substring(Items.ToString().IndexOf(':') + 1).Trim();
                }

                int _ItemsRowCount = GetFilledRows();
                int _TaxLines = 0;
                if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0)
                    _TaxLines = _TaxLines + 1;
               
               if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0)
                    _TaxLines = _TaxLines + 1;
               
                int NoDistributions = 0;

                if (txtServCharges.Text.Trim() != string.Empty && double.Parse(txtServCharges.Text.Trim()) > 0)
                {
                    NoDistributions = _ItemsRowCount + _TaxLines + 1;
                }
                else
                {
                    NoDistributions = _ItemsRowCount + _TaxLines;
                }

                //if (IntInvoicetype != 1)
                //{
                //    if (_Discount > 0)
                //    {
                //        NoDistributions = NoDistributions + 1;
                //    }
                //}

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

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Invoice_Number");
                Writer.WriteString(txtInvoiceNo.Text.ToString().Trim());
                Writer.WriteEndElement();


                string crtype = null;
                if (optCredit.Text.ToString() == "Credit" && combMode.Text.ToString() == "Credit")
                {
                    crtype = "Credit";
                }
                else
                {
                    crtype = "Cash";
                }

                Writer.WriteStartElement("Ship_Via");
                Writer.WriteString(crtype);
                Writer.WriteEndElement();

                //Writer.WriteStartElement("Ship_Via");
                //string crtype = null;
                //crtype = combMode.Text.ToString();
                //Writer.WriteString(crtype);
                //Writer.WriteEndElement();

                //Writer.WriteStartElement("Sales_Representative_ID");
                //Writer.WriteAttributeString("xsi:type", "paw:id");
                //Writer.WriteString(cmbSalesRep.Value.ToString().Trim());//Vendor ID should be here = Ptient No
                //Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                if (user.MergeAccUser)
                {
                    Writer.WriteString(user.userName.ToString().Trim());
                }
                else
                {
                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                }
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
                    if (IsThisItemSerial(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString()))
                    {
                        foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        {
                            if (dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString() == dr["colItem"].ToString())
                            {
                                Writer.WriteStartElement("SalesLine");

                                Writer.WriteStartElement("Quantity");
                                Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Item_ID");
                                Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Description");
                                Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colDescription"].Value.ToString());
                                Writer.WriteEndElement();

                                string GL_code = dgvdispactApplytoSales.Rows[i].Cells["colGL"].Value.ToString();
                                if (IsGLok == true)
                                {
                                    GL_code = StrSalesGLAccount;
                                }
                                Writer.WriteStartElement("GL_Account");
                                Writer.WriteString(GL_code);
                                Writer.WriteEndElement();

                                //if (IntInvoicetype == 1)
                                //{

                                //    _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                //        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) / Temp_getTaxRate();
                                //    _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                // double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                                //* double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                 

                                //}
                                //else
                                //{
                                //    _Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                //        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());

                                //    _DiscountTotLine = _DiscountTotLine + _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                //}

                                //_AmountTot = _AmountTot + _Amount;

                                _Amount = 0;
                                if (IntInvoicetype == 1)
                                {
                                    if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount < 0) //Line Discount
                                    {
                                        _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                        Temp_getTaxRate();

                                        if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                        }
                                        else
                                        {
                                            _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                        }
                                    }
                                    else if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) < 0) //Foter Discount
                                    {
                                        _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                        Temp_getTaxRate();

                                        if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0)
                                        {
                                            _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                        }
                                        else
                                        {
                                            _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                        }

                                        _DiscountTotLine = _DiscountTotLine +
                                        (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                                       * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                    }
                                    else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                                    {
                                        _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                        Temp_getTaxRate();
                                        _Amount = _Amount - _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                        _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                    }
                                    else
                                    {
                                        _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                        Temp_getTaxRate();
                                        _DiscountTotLine = _DiscountTotLine +
                                       (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                       double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                                      * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                    }

                                }
                                else if (IntInvoicetype == 2)
                                {
                                    if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0) //Foter Discount
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                    }
                                    else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount <= 0)
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                    }
                                    else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;
                                        _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                    }
                                    else
                                    {
                                        _Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                        //double NBTAM = _Amount * (double.Parse(txtNBTP.Text.Trim())/100);
                                        //double VATAM = _Amount * (double.Parse(txtVATP.Text.Trim())/100);
                                        //_Amount=_Amount+NBTAM+

                                        // _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                        //double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                        //Temp_getTaxRate();

                                        _DiscountTotLine = _DiscountTotLine +
                                       (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                       double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                                      * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                    }
                                }
                                else if (IntInvoicetype == 3)
                                {
                                    if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0) //Foter Discount
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                    }
                                    else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount <= 0)
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                    }
                                    else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                                    {
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                        _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;
                                        _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                        _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                    }
                                    else
                                    {
                                        _Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                            double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                    }

                                    //_Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                    //    double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                }
                                _AmountTot = _AmountTot + _Amount;
                                
                                //_DiscountTotLine = _DiscountTotLine + _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString())/100;
                                //(_Amount/Temp_getTaxRate())*100

                                Writer.WriteStartElement("Amount");
                                Writer.WriteString("-" + _Amount.ToString());
                                Writer.WriteEndElement();
                                //========================================================

                                Writer.WriteStartElement("Job_ID");
                                Writer.WriteString(cmbjob.Text.ToString().Trim());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Tax_Type");
                                Writer.WriteString("1");//Doctor Charge
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("SalesOrderDistributionNumber");
                                Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["SODisNo"].Value.ToString());
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Apply_To_Sales_Order");
                                Writer.WriteString("TRUE");
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("SalesOrderNumber");
                                Writer.WriteString(_SalesOrderNo);
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Serial_Number");
                                Writer.WriteString(dr["SerialNo"].ToString());
                                Writer.WriteEndElement();

                                Writer.WriteEndElement();// end of sales line 
                            }
                        }
                    }
                    else
                    {
                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["colDescription"].Value.ToString());
                        Writer.WriteEndElement();

                        string GL_code = dgvdispactApplytoSales.Rows[i].Cells["colGL"].Value.ToString();
                        if (IsGLok == true)
                        {
                            GL_code = StrSalesGLAccount;
                        }

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(GL_code);
                        Writer.WriteEndElement();

                        _Amount = 0;
                        if (IntInvoicetype == 1)
                        {
                            if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount <= 0) //Line Discount
                            {
                                _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                Temp_getTaxRate();

                                if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                                }
                                else
                                {
                                    _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                }      
                            }
                            else if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0 ) //Foter Discount
                            {
                                _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                Temp_getTaxRate();

                                if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0)
                                {
                                    _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                }
                                else
                                {
                                    _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                }                             

                                _DiscountTotLine = _DiscountTotLine + 
                                (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                               * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                            {
                                _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                Temp_getTaxRate();                                
                                _Amount = _Amount - _Amount * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;                                                               
                                _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;                                
                            }
                            else
                            {
                                _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                                Temp_getTaxRate();
                                _DiscountTotLine = _DiscountTotLine +
                               (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                               double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                              * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                            }                         

                        }
                        if (IntInvoicetype == 2)
                        {
                            if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0) //Foter Discount
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount <= 0)
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;
                                _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0 && _Discount <= 0) //Line Discount & footer
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) ;                                                               
                                _DiscountTotLine = 0.00 ;
                            }
                            else if (IntInvoicetype == 3)
                            {
                                _Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());
                                //double NBTAM = _Amount * (double.Parse(txtNBTP.Text.Trim())/100);
                                //double VATAM = _Amount * (double.Parse(txtVATP.Text.Trim())/100);
                                //_Amount=_Amount+NBTAM+

                               // _Amount = 100 * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                               //double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()) /
                               //Temp_getTaxRate();
                                
                                _DiscountTotLine = _DiscountTotLine +
                               (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                               double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString()))
                              * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100;
                            }
                        }
                        else if (IntInvoicetype == 3)
                        {
                            if (_Discount > 0 && double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) <= 0) //Foter Discount
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(txtPersntage.Text.ToString()) / 100);
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount <= 0)
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;

                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                            }
                            else if (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) > 0 && _Discount > 0) //Line Discount & footer
                            {
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                                _Amount = (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) - _Amount;
                                _Amount = _Amount - _Amount * double.Parse(txtPersntage.Text.ToString()) / 100;
                                _DiscountTotLine = _DiscountTotLine + (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) * double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString())) * (double.Parse(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString()) / 100);
                            }
                            else
                            {
                                _Amount = double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()) *
                                    double.Parse(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value.ToString());
                            }
                        }
                        _AmountTot = _AmountTot + _Amount;

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + _Amount.ToString()); //("-" + double.Parse(_Amount.ToString()).ToString("0.00"));
                        
                        
                        Writer.WriteEndElement();
                        //========================================================

                        Writer.WriteStartElement("Job_ID");
                        Writer.WriteString(cmbjob.Text.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesOrderDistributionNumber");
                        Writer.WriteString(dgvdispactApplytoSales.Rows[i].Cells["SODisNo"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Apply_To_Sales_Order");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesOrderNumber");
                        Writer.WriteString(_SalesOrderNo);
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();// end of sales line     
                    }
                }

                double _VAT = 0;
                double _NBT = 0;                

                

                if (cmbInvoiceType.Value.ToString() == "1")
                {
                    _NBT = _AmountTot * double.Parse(txtNBTP.Text.Trim()) / 100;
                    _VAT = (_AmountTot + _NBT) * double.Parse(txtVATP.Text.Trim()) / 100;
                   // _Discount = ((_AmountTot+_NBT+_VAT) - _DiscountTotLine) * double.Parse(txtPersntage.Text.Trim()) / 100;
                   // _Discount = _Discount + _DiscountTotLine;
                }
                else if (cmbInvoiceType.Value.ToString() == "2")
                {
                    _NBT = (_AmountTot) * double.Parse(txtNBTP.Text.Trim()) / 100;
                    _VAT =((_AmountTot) + _NBT) * double.Parse(txtVATP.Text.Trim()) / 100;
                }
                else
                {
                    _NBT = (_AmountTot - _Discount) * double.Parse(txtNBTP.Text.Trim()) / 100;
                    _VAT = ((_AmountTot - _Discount) + _NBT) * double.Parse(txtVATP.Text.Trim()) / 100;

                    //_Discount = (_AmountTot - _DiscountTotLine) * double.Parse(txtPersntage.Text.Trim()) / 100;
                    //_Discount = _Discount + _DiscountTotLine;
                }

                if (_TaxLines > 0)
                {

                    //if (rdoTax.Checked)
                    //{
                        if (double.Parse(txtVAT.Text.Trim()) > 0)
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
                            Writer.WriteString("-" + _VAT.ToString());//HospitalCharge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Apply_To_Sales_Order");
                            Writer.WriteString("FALSE");
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("SalesOrderDistributionNumber");
                            Writer.WriteString("0");
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                        }
                  //  }
                    //NBT Amount
                    if (double.Parse(txtNBT.Text.ToString()) > 0)
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
                        Writer.WriteString("-" + _NBT.ToString());//HospitalCharge
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
                //if (cmbInvoiceType.Value.ToString() == "1")
                //{
                //    if (_Discount > 0)
                //    {
                //        Writer.WriteStartElement("SalesLine");
                //        Writer.WriteStartElement("Quantity");
                //        Writer.WriteString("");
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("GL_Account");
                //        Writer.WriteAttributeString("xsi:type", "paw:id");
                //        Writer.WriteString(user.DiscountGL);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Item_ID");
                //        Writer.WriteString(user.DiscountItemID);
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Description");
                //        Writer.WriteString("Discount");
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Job_ID");
                //        Writer.WriteString(cmbjob.Text.ToString().Trim());
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Tax_Type");
                //        Writer.WriteString("1");
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Amount");
                //        Writer.WriteString(_Discount.ToString());
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("Apply_To_Sales_Order");
                //        Writer.WriteString("FALSE");
                //        Writer.WriteEndElement();

                //        Writer.WriteStartElement("SalesOrderDistributionNumber");
                //        Writer.WriteString("0");
                //        Writer.WriteEndElement();

                //        Writer.WriteEndElement();
                //    }
                //}
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
                    Writer.WriteString("-" + txtServCharges.Text.ToString());//HospitalCharge
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
                IsExport = true;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private void GetTaxDeails()
        {
            try
            {
                String S1 = "Select * from tblTaxApplicable order by Rank";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                txtNBTP.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                txtVATP.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");

                Tax1ID = dt.Rows[0]["TaxID"].ToString();
                Tax2ID = dt.Rows[1]["TaxID"].ToString();

                Tax1Name = dt.Rows[0]["TaxName"].ToString();
                Tax2Name = dt.Rows[1]["TaxName"].ToString();

                Tax1Rate = double.Parse(dt.Rows[0]["Rate"].ToString());
                Tax2Rate = double.Parse(dt.Rows[1]["Rate"].ToString());

                Tax1GLAccount = user.TaxPayGL1;
                Tax2GLAccount = user.TaxPayGL2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CheckRowsValidation()
        {
            try
            {
                PayValChecked = false;
                int intGrid = 0;
                for (intGrid = 0; intGrid < grdviewdata.Rows.Count - 1; intGrid++)
                {
                    string CRCardNo = "";

                    if (grdviewdata.Rows[intGrid].Cells["Card_Name"].Value == null || grdviewdata.Rows[intGrid].Cells["Card_Am"].Value == null || double.Parse(grdviewdata.Rows[intGrid].Cells["Card_Am"].Value.ToString()) < 0)
                    {
                        PayValChecked = true;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        int IntInvoicetype = 1;
        private void btnSave_Click(object sender, EventArgs e)
        {
            PosType = 0;
            if (dtpDispatchDate.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
           // return;
            if (dtpDispatchDate.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (CurrentInvType != Convert.ToInt16(cmbInvoiceType.Value))
            {
                MessageBox.Show("Invoice Type Missmatch", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            CheckRowsValidation();
            if (PayValChecked == true)
            {
                MessageBox.Show("Invalid Card Amount or Pay Type", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
            {
                if (chkpos.Checked == true)
                {
                    PosType = 1;
                    IntInvoicetype = 1;
                }
                else
                {
                    IntInvoicetype = 1;
                }
                
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//inclusive
            {
                IntInvoicetype = 2;
            }
            if (Convert.ToInt64(cmbInvoiceType.Value) == 3)//inclusive
            {
                IntInvoicetype = 3;
            }
            if (!objControlers.HeaderValidation_Customer(cmbCustomer.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_SaleRep(cmbSalesRep.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_Warehouse(txtLocation.Text, sMsg)) return;

            int INVTYPE = 1;
            if (rdoNonVat.Checked == true)
            {
                if (chkpos.Checked == true)
                {
                    PosType = 1;
                }
                else
                {
                    INVTYPE = 1;
                }                
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
            if (cmbCustomer.Value == null || cmbCustomer.Text.Trim() == string.Empty)
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
            if (optCash.Checked == true)
            {
                StrPaymmetM = "Cash";
            }
            else if (optCredit.Checked == true)
            {
                StrPaymmetM = "Credit";
            }
            else if (rdobtnCreditCard.Checked)
            {
                StrPaymmetM = "CreditCard";
            }

            double CreditCAm;
            double Netam;
            double PaidAmount;
            PaidAmount = 0;
            CreditCAm = 0;
            Netam = 0;
            if (txtpaid.Text != null)
            {
                PaidAmount = double.Parse(txtpaid.Text.ToString());
            }
            if (PaidAmount < 0)
            {
                MessageBox.Show("Paid Amount is Zero....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
                return;
            }
            if (rdobtnCreditCard.Checked == true)
            {
                if (txtNetTotal.Text != null)
                {
                    CreditCAm = double.Parse(txtcardamount.Text.ToString());
                }
                Netam = double.Parse(txtNetTotal.Text.ToString());
                if (CreditCAm != Netam)
                {
                    DialogResult reply1 = MessageBox.Show("You Have a Advance Rs. " +  double.Parse(txtcardamount.Text.ToString()) + " . You want to Proceed This..? ", "Information", MessageBoxButtons.OKCancel);
                    if (reply1 != DialogResult.Cancel)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Invalid Payment....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                        
                        return;
                    }

                }

            }

            ClassDriiDown.IsInvSerch = false;
            string InvNo = "";
            string CrInv = "";
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans;
            myConnection.Open();
            myTrans = myConnection.BeginTransaction();
            IsExport = false;
            SONO = "";
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(dtpDispatchDate.Value)))
                    return;

                if (user.IsCINVNoAutoGen)
                {
                    if (user.InvPrefex == true)
                    {
                        SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = InvoiceNo + 1 select InvoiceNo, InvoicePrefix ,InvoicePad from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                        SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        DataTable dt41 = new DataTable();
                        da41.Fill(dt41);

                        {
                            InvNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(int.Parse(dt41.Rows[0]["InvoicePad"].ToString()), '0');
                            InvNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + InvNo;
                        }
                        txtInvoiceNo.Text = InvNo;
                    }
                    else
                    {
                        if (combMode.Value.ToString() == "1")
                        {
                            SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET InvoiceNo = InvoiceNo + 1 select InvoiceNo, InvoicePrefix ,InvoicePad from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            {
                                InvNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(int.Parse(dt41.Rows[0]["InvoicePad"].ToString()), '0');
                                InvNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + InvNo;
                            }
                            txtInvoiceNo.Text = InvNo;
                        }
                        else if (combMode.Value.ToString() == "2")
                        {
                            SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET InvCreditNum = InvCreditNum + 1 select InvCreditNum, InvCreditPref ,InvCreditPad from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                            SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                            DataTable dt41 = new DataTable();
                            da41.Fill(dt41);

                            {
                                InvNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(int.Parse(dt41.Rows[0]["InvCreditPad"].ToString()), '0');
                                InvNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + InvNo;
                            }
                            txtInvoiceNo.Text = InvNo;
                        }
                    }
                    //if (optCredit.Checked == true)
                    //{
                    //    //This is Only Softwave
                    //    SqlCommand myCommand1 = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET CRINVNum = CRINVNum + 1 select CRINVNum, CRINVNPref ,CRINVPad from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                    //    SqlDataAdapter da411 = new SqlDataAdapter(myCommand1);
                    //    DataTable dt411 = new DataTable();
                    //    da411.Fill(dt411);
                    //    {
                    //        CrInv = dt411.Rows[0].ItemArray[0].ToString().Trim().PadLeft(int.Parse(dt411.Rows[0]["CRINVPad"].ToString()), '0');
                    //        CrInv = dt411.Rows[0].ItemArray[1].ToString().Trim() + CrInv;
                    //    }
                    //}
                    //else
                    //{
                    //    CrInv = InvNo;
                    //}
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
                //for (int i = 0; i < rowCount; i++)
                //{
                //    isok = false;
                //    CheckOnHandQty(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()), myConnection, myTrans);

                //    if (isok == true)
                //    {
                //        myTrans.Rollback();
                //        return;
                //    }
                //}

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
                    int rowCounttax = 0;
                    if (txtNBT.Text.Trim() != string.Empty && double.Parse(txtNBT.Text.Trim()) > 0) rowCounttax = rowCounttax + 1;
                    if (txtVAT.Text.Trim() != string.Empty && double.Parse(txtVAT.Text.Trim()) > 0) rowCounttax = rowCounttax + 1;

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

                    string RepCode = null;
                    if (user.MergeAccUser)
                    {
                        RepCode = user.userName.ToString().Trim();
                    }
                    else
                    {
                        RepCode = cmbSalesRep.Value.ToString().Trim();
                    }

                    StrSqlA = "insert into tblSalesInvoices(IsDirect,ServiceCharge,IsInclusive,InvoiceNo,CustomerID,DeliveryNoteNos,InvoiceDate,ARAccount,NoofDistributions, " +
                        " DistributionNo,ItemID,Qty,Description, " +
                        " GLAcount,UnitPrice,Amount,Tax1Amount,Tax2Amount,GrossTotal,NetTotal,CurrentDate,Time,Currentuser,IsExport, " +
                        " CustomerPO,Location,TTType1,TTType2,IsReturn,TTType3,Tax3Amount,RemainQty,JobID,Tax1Rate,Tax2Rate,SalesRep,CostPrrice,Tax3Rate,InvType,TotalDiscountPercen,TotalDiscountAmount,LineDiscountPercentage,LineDiscountAmount,InclusivePrice,LineTax,PaymentM,Comments,TempCrInvoiceNo,WHID,CusName) " +
                        " values ('False','" + txtServCharges.Text.Trim() + "','" + IntInvoicetype + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" +
                        cmbCustomer.Value.ToString().Trim() + "','" +
                        SelectSONums1 + "','" +
                        dtpDispatchDate.Value.ToString("MM/dd/yyyy").Trim() + "','" +
                        cmbAR.Value.ToString().Trim() + "','" +
                        NoOfDis + "','" + (i + 1).ToString().Trim() + "','" +
                        dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value + "','" + //ItemID
                        Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" //Qty
                        + dgvdispactApplytoSales[1, i].Value.ToString().Trim() + "','" + //Description
                        dgvdispactApplytoSales[3, i].Value.ToString().Trim() + "','" + //GLAcount
                        Convert.ToDouble(dgvdispactApplytoSales[4, i].Value) + "','" + //UnitPrice
                        Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colAmount"].Value) + "','" + //Amount
                        Convert.ToDouble(txtNBT.Text) + "','" + //Tax1Amount
                        Convert.ToDouble(txtVAT.Text) + "','" + //Tax2Amount
                        Convert.ToDouble(txtGridTotalExcl.Text) + "','" + //GrossTotal
                        Convert.ToDouble(txtNetTotal.Text) + "','" +//NetTotal
                        user.LoginDate.ToString("MM/dd/yyyy") + "','" + //CurrentDate
                        System.DateTime.Now.ToShortTimeString().Trim() + "','" + //Time
                        user.userName.ToString().Trim() + "','" + //Currentuser
                        IsExport + "','" + //IsExport
                        txtCustomerPO.Text + "','" +//CustomerPO 
                        txtLocation.Text.ToString().Trim() + "','" + //Location
                        Tax1Name + "','" +//TTType1
                        Tax2Name + "','" + //TTType2
                        isretrun + "','" + //IsReturn
                        Tax3Name + "','" + //TTType3
                        Tax3Amount + "','" + //Tax3Amount
                        Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" + //RemainQty
                        cmbjob.Text.ToString().Trim() + "','" +//JobID
                        Tax1Rate + "','" + //Tax1Rate
                        Tax2Rate + "','" + //Tax2Rate  
                        RepCode.ToString() + "','" +
                        ItemCost + "','" +
                        Tax3Rate + "','" +
                        INVTYPE + "','" +
                        txtPersntage.Text.Trim() + "','" +
                        txtValueDiscount.Text.Trim() + "','" +
                        dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value.ToString() + "','" +
                        (Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value) * Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value)) * Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value)/100 + "',"+
                         " '" + dgvdispactApplytoSales.Rows[i].Cells["colInclUnitPrice"].Value.ToString() + "','" + dgvdispactApplytoSales.Rows[i].Cells["colLineTax"].Value.ToString() + "','" + StrPaymmetM + "','" + txtDescription.Text.ToString() + "','" + CrInv + "','" + txtLocation.Text.ToString().Trim()  + "','" + txtcusName.Text.ToString() + "')";

                    SqlCommand cmd = new SqlCommand(StrSqlA, myConnection, myTrans);
                    cmd.ExecuteNonQuery();
                    //===========================================================

                    isok = false;
                    CheckOnHandQty(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value.ToString()), myConnection, myTrans);
                    if (isok == true)
                    {
                        myTrans.Rollback();
                        return;
                    }

                    UpdateItemWarehouse(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString(), txtLocation.Text.ToString().Trim(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value), myConnection, myTrans);

                    StrSql1 = "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + txtLocation.Text.ToString().Trim() + "' AND ItemId='" + dgvdispactApplytoSales[0, i].Value + "') " +
                             "Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) " +
                             " values (@OHQTY,'" + DocType + "','" + txtInvoiceNo.Text.ToString().Trim() + "','" + GetDateTime(dtpDispatchDate.Value) + "','" + TranType + "','" + QtyIN + "','" + dgvdispactApplytoSales[0, i].Value + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "','" + ItemCost + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) * ItemCost + "','" + txtLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvdispactApplytoSales[2, i].Value) + "')";
                    SqlCommand cmd11 = new SqlCommand(StrSql1, myConnection, myTrans);
                    cmd11.ExecuteNonQuery();
                    //=============================================================

                   
                        
                    //==================================================
                    //as
                }
                double Paid_Am = 0;
                double AM_bal = 0;
                double Netamount = 0;
                int intGrid = 0;
                if (txtpaid.Text != null)
                {
                    Paid_Am = double.Parse(txtpaid.Text.ToString());
                }
                if (txtbalance.Text != null)
                {
                    AM_bal = double.Parse(txtbalance.Text.ToString());
                }
                if (txtNetTotal.Text != null)
                {
                    Netamount = double.Parse(txtNetTotal.Text.ToString());
                }
                if (Paid_Am < 0)
                {
                    MessageBox.Show("Paid Amount is Not Valid....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    myTrans.Rollback();
                    return;
                }
                if (txtpaid.Text != null)
                {
                    SaveInvBalance(txtInvoiceNo.Text.ToString().Trim(), Netamount, Paid_Am, AM_bal,StrPaymmetM, myConnection, myTrans);
                }

                for (intGrid = 0; intGrid < grdviewdata.Rows.Count-1; intGrid++)
                {
                    string CRCardNo = "";
                    if (grdviewdata.Rows[intGrid].Cells["Card_Name"].Value != null && grdviewdata.Rows[intGrid].Cells["Card_Am"].Value != null)
                    {
                        if ((grdviewdata.Rows[intGrid].Cells["Card_No"].Value == null && grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString() != "Cash") && grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString() != "Advance")
                        {
                            MessageBox.Show("Check Payment Type....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            myTrans.Rollback();
                            return;
                        }
                         
                        if (grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString() == "Cash")
                        {
                            CRCardNo = "Cash";
                        }
                        else if (grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString().ToUpper() == "ADVANCE")
                        {
                            CRCardNo = "Advance";
                        }
                        else 
                        {
                            CRCardNo = grdviewdata.Rows[intGrid].Cells["Card_No"].Value.ToString();
                        }
                        SaveCardDetails(txtInvoiceNo.Text.ToString().Trim(), grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString(), CRCardNo, double.Parse(grdviewdata.Rows[intGrid].Cells["Card_Am"].Value.ToString()), myConnection, myTrans);
                    }
                }

                UpdateDeliveryNotes(myTrans, myConnection);

                DataTable dtblRefNo = new DataTable();
                dtblRefNo = GetCheckedGRNList();

                DataTable dt = new DataTable();
                clsSerializeItem.DtsSerialNoList.Rows.Clear();

                if (clsSerializeItem.DtsSerialNoList.Columns.Count == 0)
                {
                    clsSerializeItem.DtsSerialNoList.Columns.Add("ItemCode");
                    clsSerializeItem.DtsSerialNoList.Columns.Add("WHCode");
                    clsSerializeItem.DtsSerialNoList.Columns.Add("SerialNo");
                    clsSerializeItem.DtsSerialNoList.Columns.Add("Status");
                }

                foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                {
                    if (dgvr.Cells[0].Value != null)
                    {
                        string SSql = "SELECT 'True' as Selected, TRNNO,ItemID as ItemCode,SerialNO,TransactionType,Status,LocationID as WHCode,TransDate,IsOut,Status2 FROM tblSerialTransfer " +
                               " WHERE (tblSerialTransfer.ItemID = '" + dgvr.Cells[0].Value.ToString().Trim() + "') and Status<>'Invoiced' and   (";

                        for (int Index = 0; Index <= dtblRefNo.Rows.Count - 1; Index++)
                        {
                            SSql = SSql + " tblSerialTransfer.TRNNO='" + dtblRefNo.Rows[Index]["DNote"].ToString().Trim() + "' ";

                            if (Index != dtblRefNo.Rows.Count - 1 && dtblRefNo.Rows.Count != 1)
                                SSql = SSql + " or ";
                        }
                        SSql = SSql + ")";

                        SqlCommand cmd = new SqlCommand(SSql, myConnection, myTrans);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                clsSerializeItem.DtsSerialNoList = dt;

                if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                {
                    frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Invoice", txtLocation.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDispatchDate.Value, true, "Invoiced");
                }

                             
                
                if (CreateSalesJXML(myTrans, myConnection) == 1)
                    myTrans.Commit();
                else
                {
                     myTrans.Rollback();
                    return;
                }

                if (optCash.Checked == true)
                {
                    exporetReceipt_Cash(txtInvoiceNo.Text.Trim());                    
                }
                else if (rdobtnCreditCard.Checked == true)
                {
                    exporetReceipt_Credit(txtInvoiceNo.Text.Trim());                    
                }

                //if (IsExport == true)
                //{
                //    myTrans.Commit();
                //}
                //else
                //{
                //    myTrans.Rollback();                    
                //}

               //if IsExport==True
                //Commit
                //else
                //Rallbak and return
                MessageBox.Show("Customer Invoice Successfuly Saved");

                if (INVTYPE == 1) //chkpos
                {
                    if (chkpos.Checked == true)
                    {
                        PosType = 1;
                    }
                    else
                    {
                        PosType = 0;
                    }
                    btnPrint_Click(sender, e);
                }
                if (INVTYPE == 2)
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
        private void UpdateItemWarehouse(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Asanga
        private void SaveInvBalance(string InvoiceNo, double InvAmount, double InvPaid, double InvBalance,string STPayType, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Insert into  tblInvoicePaymentHistory(InvoiceNo,InvAmount,InvPaid,InvBalance,PayType)" +
                    "VALUES ('" + InvoiceNo + "','" + InvAmount + "','" + InvPaid + "'," + InvBalance + ",'" + STPayType + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Asanga
        private void SaveCardDetails(string InvoiceNo, string CardType, string CardNo, double Amount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Insert into  tblInvoicePayTypes(InvoiceNo,CardType,CardNo,Amount)" +
                    "VALUES ('" + InvoiceNo + "','" + CardType + "','" + CardNo + "'," + Amount + ")";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Asanga
        private void viewCardHistory(string StrInvoiceNo)
        {
            try
            {

                grdviewdata.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "SELECT CardType,CardNo,Amount from tblInvoicePayTypes  WHERE InvoiceNo='" + StrInvoiceNo + "'   ORDER BY CardType";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    grdviewdata.Rows.Add();                   
                    grdviewdata.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    grdviewdata.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    grdviewdata.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
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
                //GetDeafultRep();
                LoadCreditcardData();
                rdoNonVat.Checked = true;
                GetTaxDeails();
                DatgridcolumnEnable();
                loadDefaltOption();
                ClassDriiDown.IsInvSerch = false;

                if (user.IsCINVNoAutoGen) txtInvoiceNo.ReadOnly = true;
                else txtInvoiceNo.ReadOnly = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        private void loadDefaltOption()
        {
            try
            {
                StrSql = "Select Tid,TAXID,locked from tblTax_Default where Flg='PAY' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (bool.Parse(dt.Rows[0].ItemArray[2].ToString()) == true)
                        {
                            grpamounttypes.Enabled = false;
                        }
                        else
                        {
                            grpamounttypes.Enabled = true ;
                        }
                        if (dt.Rows[0]["TAXID"].ToString() == "Cash")
                        {
                            optCash.Checked = true;
                        }
                        else if (dt.Rows[0]["TAXID"].ToString() == "Credit")
                        {
                            optCredit.Checked = true;
                        }
                        else if (dt.Rows[0]["TAXID"].ToString() == "Other")
                        {
                            rdobtnCreditCard.Checked = true;
                        }
                    }
                }                

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
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
                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='REP' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql1);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    cmbSalesRep.Enabled = true ;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        cmbSalesRep.Value = dt2.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt2.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbSalesRep.Enabled = false;
                        }
                    }
                }

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='INV' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd3 = new SqlCommand(StrSql1);
                SqlDataAdapter da3 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt3 = new DataTable();
                da3.Fill(dt3);
                if (dt3.Rows.Count > 0)
                {
                    combMode.Enabled = true;
                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        combMode.Value = dt3.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt3.Rows[i]["locked"].ToString()) == true)
                        {
                            combMode.Enabled = false;
                        }
                        else
                        {
                            combMode.Enabled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void DirectPrintPOSInvoice()
        {
            try
            {
                ReportDocument crp = new ReportDocument();
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRPOSInvoice.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(DsItemWise);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }
            ClassDriiDown.IsInvSerch = false;
            DsItemWise.Clear();

            try
            {
                DsItemWise.Clear();
                StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsItemWise.dtSalesInvoice);

                StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsItem);

                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster where CutomerID='" + cmbCustomer.Text.ToString().Trim()  + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsCustomer);

                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsWarehouse);

                StrSql = "SELECT * FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() +  "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblInvoicePaymentHistory );

                StrSql = "SELECT * FROM tblInvoicePayTypes WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblpaytran);

                StrSql = "SELECT * FROM tblSalesRep where repcode='" + cmbSalesRep.Value.ToString()  + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblSalesRep);

                if (chkpos.Checked == true)
                {
                    if (txtInvoiceNo.ToString() != "")
                    {
                        DirectPrintPOSInvoice();                       
                    }                    
                }
                else
                {
                    InDirectPrint();
                }
                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void InDirectPrint()
        {
            try
            {

                if (chkpos.Checked == true)
                {
                    PosType = 1;
                }
                else
                {
                    PosType = 0;
                }

                if (rdoNonVat.Checked)
                {
                    if (chkpos.Checked == true)
                    {
                        PosType = 1;
                        frmInvoicePrint printax = new frmInvoicePrint(this);                         
                        printax.Show();
                    }
                    else
                    {
                        InvoiceType = 1;
                        frmInvoicePrint printax = new frmInvoicePrint(this);
                        printax.Show();
                    }
                }
                if (rdoTax.Checked)
                {
                    if (Convert.ToInt64(cmbInvoiceType.Value) == 1)
                        TaxINCType = 1;
                    else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)
                        TaxINCType = 2;
                    
                    InvoiceType = 2;
                    frmInvoicePrint printax = new frmInvoicePrint(this);
                    printax.Show();
                }
                if (rdoSVat.Checked)
                {
                    InvoiceType = 3;
                    frmInvoicePrint printax = new frmInvoicePrint(this);
                    printax.Show();
                }
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
                //FillDetails();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnReprint2_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

            ClassDriiDown.IsInvSerch = false;
            DsItemWise.Clear();
            try
            {
                //String S12 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                //SqlCommand cmd12 = new SqlCommand(S12);
                //SqlConnection con12 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                //da12.Fill(DsItemWise, "DTItemMaster");

                //String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                //SqlCommand cmd1 = new SqlCommand(S1);
                //SqlConnection con1 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                //da1.Fill(DsItemWise, "CustomerMaster1");

                //String S3 = "Select * from tblSalesInvoices where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                //SqlCommand cmd3 = new SqlCommand(S3);
                //SqlConnection con3 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                //da3.Fill(DsItemWise, "dtInvoiceData");

                //String S322 = "SELECT * FROM tblCompanyInformation";
                //SqlCommand cmd322 = new SqlCommand(S322);
                //SqlConnection con322 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da322 = new SqlDataAdapter(S322, con322);
                //da322.Fill(DsItemWise, "DtCompaniInfo");

                //InDirectPrint();
                ////frmInvoicePrint printax = new frmInvoicePrint(this);
                ////printax.Show();

                DsItemWise.Clear();
                StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsItemWise.dtSalesInvoice);

                StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsItem);

                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Custom1,Custom2,Custom3,Custom4 FROM tblCustomerMaster where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsCustomer);

                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.DsWarehouse);

                StrSql = "SELECT * FROM tblInvoicePaymentHistory WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblInvoicePaymentHistory);

                StrSql = "SELECT * FROM tblInvoicePayTypes WHERE InvoiceNo='" + txtInvoiceNo.Text.ToString().Trim() + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblpaytran);

                StrSql = "SELECT * FROM tblSalesRep where repcode='" + cmbSalesRep.Value.ToString() + "'";
                cmd = new SqlCommand(StrSql);
                da = new SqlDataAdapter(StrSql, ConnectionString);
                dt = new DataTable();
                da.Fill(DsItemWise.tblSalesRep);

                if (chkpos.Checked == true)
                {
                    if (txtInvoiceNo.ToString() != "")
                    {
                        DirectPrintPOSInvoice();
                    }
                }
                else
                {
                    InDirectPrint();
                }

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
                if (clistbxSalesOrder.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
                {
                    e.NewValue = e.CurrentValue;
                    MessageBox.Show("Sytem Does not allowed to Select more than one sales order");
                    txtNetTotal.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public static double DiscountAmount = 0.0;

        //view-as
        private void FillDetails(string StrInvNo)
        {

            if (ClassDriiDown.IsInvSerch == true)
            {
                
                clistbxSalesOrder.Enabled = false;
                string SerchText = StrInvNo;// ab.GetText1();
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
                    GetCustomer();
                    cmbCustomer.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    cmbjob.Text = dt.Rows[0].ItemArray[25].ToString().Trim();
                    string abc = dt.Rows[0].ItemArray[2].ToString().Trim();
                    dtpDispatchDate.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    cmbAR.Text = dt.Rows[0].ItemArray[4].ToString().Trim();
                    // txtGridTotalExcl.Text = dt.Rows[0].ItemArray[17].ToString().Trim();
                    //txtNetTotal.Text = dt.Rows[0]["netvalue"].ToString().Trim();
                    txtCustomerPO.Text = dt.Rows[0].ItemArray[23].ToString().Trim();
                    txtLocation.Text = dt.Rows[0].ItemArray[27].ToString().Trim();//Loacation
                    cmbSalesRep.Text = dt.Rows[0]["SalesRep"].ToString();
                    combMode.Value = dt.Rows[0]["PaymentM"].ToString();
                    if (Convert.ToDouble(dt.Rows[0]["IsInclusive"].ToString()) == 1)
                    {
                        cmbInvoiceType.Value = 1;
                    }
                    if (Convert.ToDouble(dt.Rows[0]["IsInclusive"].ToString()) == 2)
                    {
                        cmbInvoiceType.Value = 2;
                    }
                    if (Convert.ToDouble(dt.Rows[0]["IsInclusive"].ToString()) == 3)
                    {
                        cmbInvoiceType.Value = 3;
                    }

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

                    //if (bool.Parse(dt.Rows[0]["IsInclusive"].ToString()))
                    //    rbtInclusive.Checked = true;
                    //else
                    //    rbtExclusive.Checked = true;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                       
                        dgvdispactApplytoSales.Rows.Add();
                        dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value = dt.Rows[i]["ItemID"].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells["colDescription"].Value = dt.Rows[i]["Description"].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value = dt.Rows[i]["Qty"].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells["colGL"].Value = dt.Rows[i]["GLAcount"].ToString().Trim();
                        dgvdispactApplytoSales.Rows[i].Cells["colUnitPrice"].Value = Convert.ToDouble(dt.Rows[i]["UnitPrice"]).ToString("0.00");
                        dgvdispactApplytoSales.Rows[i].Cells["colInclUnitPrice"].Value = Convert.ToDouble(dt.Rows[i]["InclusivePrice"]).ToString("0.00");
                        dgvdispactApplytoSales.Rows[i].Cells["colLineTax"].Value = Convert.ToDouble(dt.Rows[i]["LineTax"]).ToString("0.00");
                        dgvdispactApplytoSales.Rows[i].Cells["colAmountIncl"].Value = (Convert.ToDouble(dt.Rows[i]["InclusivePrice"]) * Convert.ToDouble(dt.Rows[i]["Qty"].ToString())).ToString("0.00");
                        dgvdispactApplytoSales.Rows[i].Cells["colAmount"].Value = Convert.ToDouble(dt.Rows[i]["Amount"]).ToString("0.00");
                        dgvdispactApplytoSales.Rows[i].Cells["Discount"].Value = Convert.ToDouble(dt.Rows[i]["LineDiscountPercentage"]).ToString("0.00");
                    }

                    txtNBTP.Text = Convert.ToDouble(dt.Rows[0]["Tax1Rate"]).ToString("0.00");
                    txtVATP.Text = Convert.ToDouble(dt.Rows[0]["Tax2Rate"]).ToString("0.00");
                    txtPersntage.Text = Convert.ToDouble(dt.Rows[0]["TotalDiscountPercen"]).ToString("0.00");
                    txtValueDiscount.Text = Convert.ToDouble(dt.Rows[0]["TotalDiscountAmount"]).ToString("0.00");
                    txtServCharges.Text = Convert.ToDouble(dt.Rows[0]["ServiceCharge"]).ToString("0.00");
                    txtNetTotal.Text = Convert.ToDouble(dt.Rows[0]["NetTotal"]).ToString("0.00");
                    txtNBT.Text = Convert.ToDouble(dt.Rows[0]["Tax1Amount"]).ToString("0.00");
                    txtVAT.Text = Convert.ToDouble(dt.Rows[0]["Tax2Amount"]).ToString("0.00");
                    txtDescription.Text  = dt.Rows[0]["Comments"].ToString();
                }
                btnSave.Enabled = false;
                ValidateInvoiceSetting();
                
            }           
           
        }

        private void txtValueDiscount_Leave(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtGridTotalExcl.Text.Trim() != string.Empty && double.Parse(txtGridTotalExcl.Text.Trim())>0)
            //        txtPersntage.Text = (double.Parse(txtValueDiscount.Text.Trim()) * 100 / double.Parse(txtGridTotalExcl.Text.Trim())).ToString("0.00");
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
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
                if (ClassDriiDown.IsInvSerch == false)
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

                            dgvdispactApplytoSales.Rows.Clear();
                            txtNetTotal.Text = "0.00";
                            txtValueDiscount.Text = "0.00";
                            txtServCharges.Text = "0.00";
                            txtNBT.Text = "0";
                            txtVAT.Text = "0";

                            Load_salesOrder();
                        }
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

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }


            ClassDriiDown.IsInvSerch = false;
            DsItemWise.Clear();
            try
            {
                String S12 = "Select * from tblItemMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlConnection con12 = new SqlConnection(ConnectionString);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                da12.Fill(DsItemWise, "DTItemMaster");

                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DsItemWise, "CustomerMaster1");

                String S3 = "Select * from tblSalesInvoices where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(DsItemWise, "dtInvoiceData");

                String S322 = "SELECT * FROM tblCompanyInformation";
                SqlCommand cmd322 = new SqlCommand(S322);
                SqlConnection con322 = new SqlConnection(ConnectionString);
                SqlDataAdapter da322 = new SqlDataAdapter(S322, con322);
                da322.Fill(DsItemWise, "DtCompaniInfo");

                InDirectPrint();

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

                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                //int rowCount = GetFilledRows();
                //NAmountDC = 0.0;
                //double DispatchQty = 0.00;
                //double unitprice = 0.00;
                //double unitprice_Incl = 0.00;
                //double DiscountRate = 0.00;
                //double DiscountAmount = 0.00;
                //double Amount = 0.00;
                //double Amount1 = 0.00;
                //double TotalAmount = 0.00;
                //double TotalDiscAmount = 0.00;
                ////GetTaxDeails();

                //for (int a = 0; a < rowCount; a++)
                //{
                //    if (dgvdispactApplytoSales.Rows[a].Cells["colItem"].Value != null && dgvdispactApplytoSales.Rows[a].Cells["colInvQty"].Value != null)
                //    {
                //        if (rbtExclusive.Checked)
                //        {
                //            DispatchQty = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colInvQty"].Value);
                //            unitprice = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colUnitPrice"].Value);
                //            DiscountRate = Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["Discount"].Value) / 100;
                //            Amount = (DispatchQty * unitprice);
                //            TotalAmount = TotalAmount + Amount;
                //            DiscountAmount = Amount * DiscountRate;
                //            Amount1 = Amount - DiscountAmount;
                //            dgvdispactApplytoSales.Rows[a].Cells["colDiscLineAmt"].Value = DiscountAmount.ToString("0.00");
                //            dgvdispactApplytoSales.Rows[a].Cells["colAmount"].Value = Amount1.ToString("0.00");
                //            TotalDiscAmount = TotalDiscAmount + Convert.ToDouble(dgvdispactApplytoSales.Rows[a].Cells["colDiscLineAmt"].Value);
                //            unitprice_Incl = unitprice + (unitprice * Tax1Amount / 100);
                //            unitprice_Incl = unitprice_Incl + (unitprice_Incl * Tax2Amount / 100);
                //            //dgvdispactApplytoSales.Rows[a].Cells["colInclUnitPrice"].Value = unitprice_Incl;
                //        }
                //    }
                //}

                //txtDiscLineTot.Text = TotalDiscAmount.ToString();
                ////txtSubTot.Text = Amount1.ToString("N2");
                ////txtGridTotalExcl.Text = (Amount1+double.Parse(txtServCharges.Text.Trim())).ToString("N2");
                ////txtNetTotal.Text = TotalAmount.ToString("N2");

                ////CalculateTaxAmounts();

                //InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                //FooterCalculation();
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
                    StrSalesGLAccount = dt.Tables[0].Rows[0]["SalesGLAccount"].ToString();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void FillInvoiceType()
        {
            try
            {
                if (rdoNonVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTP.Text = "0.00";
                    txtVATP.Text = "0.00";
                    txtVAT.Text = "0.00";

                    txtNBT.ReadOnly = true;
                    txtNBTP.ReadOnly = true;
                    txtVAT.ReadOnly = true;
                    txtVATP.ReadOnly = true;
                }
                else if (rdoTax.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTP.Text = "0.00";
                    txtVATP.Text = "0.00";
                    txtVAT.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTP.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVATP.ReadOnly = false;
                    GetTaxDeails();
                }
                else if (rdoSVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtNBTP.Text = "0.00";
                    txtVATP.Text = "0.00";
                    txtVAT.Text = "0.00";

                    //txtNBT.ReadOnly = false;
                    txtNBTP.ReadOnly = false;
                    //txtVAT.ReadOnly = false;
                    txtVATP.ReadOnly = false;
                    GetTaxDeails();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void rdoNonVat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (rbtInclusive.Checked)
                //{
                //    if (rdoNonVat.Checked)
                //    {
                //        rdoNonVat.Checked = false;
                //        rdoTax.Checked = true;
                //        //MessageBox.Show("Invalid Price Entry....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }

                //}
                //if (rdoNonVat.Checked)
                //    FillInvoiceType();

                //if (rdoNonVat.Checked)
                //{
                //    txtVAT.Text = "0.00";
                //    txtNBT.Text = "0.00";
                //    CalculateTaxAmounts();
                //    rbtExclusive.Checked = true;
                //}
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoTax_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (rdoTax.Checked)
            //        FillInvoiceType();
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
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
                Tax1Rate = double.Parse(txtNBTP.Text.Trim());
                Tax2Rate = double.Parse(txtVATP.Text.Trim());
                _footerDiscP = double.Parse(txtPersntage.Text.Trim());

                if (rbtExclusive.Checked || (ClassDriiDown.IsInvSerch == true))
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

                            //Incl Unit Price without Discount
                            //_Amount = (_UnitPrice * _Qty);
                            _TempTax1 = _UnitPrice * Tax1Rate / 100;
                            _TempTax2 = (_UnitPrice + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;
                            dgvr.Cells["colInclUnitPrice"].Value = (_UnitPrice + _LineTax).ToString("0.00");


                            //Incl Unit Price with Discount
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            //dgvr.Cells["colInclUnitPrice"].Value = (_UnitPrice + _LineTax).ToString("0.00");
                            dgvr.Cells["colDiscLineAmt"].Value = _LineDiscount.ToString("0.00");
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

                else if (rbtInclusive.Checked)
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

                            _TempTaxRate = Temp_getTaxRate();

                            _Qty = double.Parse(dgvr.Cells["colInvQty"].Value.ToString());
                            _UnitPriceIncl = double.Parse(dgvr.Cells["colInclUnitPrice"].Value.ToString());
                            _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                            _UnitPrice = _UnitPriceIncl * 100 / (100 + _TempTaxRate);

                            _DiscountP = double.Parse(dgvr.Cells["Discount"].Value.ToString());
                            _LineDiscount = _UnitPrice * _Qty * _DiscountP / 100;
                            _Amount = (_UnitPrice * _Qty) - _LineDiscount;

                            _TempTax1 = _Amount * Tax1Rate / 100;
                            _TempTax2 = (_Amount + _TempTax1) * Tax2Rate / 100;
                            _LineTax = _TempTax1 + _TempTax2;

                            dgvr.Cells["colUnitPrice"].Value = (_UnitPrice).ToString("0.00");
                            dgvr.Cells["colDiscLineAmt"].Value = _LineDiscount.ToString("0.00");
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

                txtGridTotalExcl.Text = _TotalAmount.ToString("0.00");
                txtGridTotalIncl.Text = (_TotalAmount + _Tax1Total + _Tax2Total).ToString("0.00");

                if (txtServCharges.Text.Trim() == null || txtServCharges.Text.Trim() == string.Empty)
                    txtServCharges.Text = "0.00";

                _servCharg = double.Parse(txtServCharges.Text.Trim());
                _servChargTax1 = _servCharg * Tax1Rate / 100;
                _servChargTax2 = (_servChargTax1 + _servCharg) * Tax2Rate / 100;

                //txtNBT.Text = (_Tax1Total + _servChargTax1).ToString("0.00");
                //txtVAT.Text = (_Tax2Total + _servChargTax2).ToString("0.00");

                if (rdoNonVat.Checked)
                {
                    txtNBT.Text = "0.00";
                    txtVAT.Text = "0.00";
                }
                else
                {
                    txtNBT.Text = (_Tax1Total + _servChargTax1).ToString("0.00");
                    txtVAT.Text = (_Tax2Total + _servChargTax2).ToString("0.00");
                }

                txtNetTotal.Text = (_TotalAmount + _servCharg - double.Parse(txtValueDiscount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (double.Parse(txtNBT.Text.Trim()) + _servChargTax1) + (double.Parse(txtVAT.Text.Trim()) + _servChargTax2)).ToString("0.00");
                txtpaid.Text = (_TotalAmount + _servCharg - double.Parse(txtValueDiscount.Text.Trim()) - (double.Parse(txtDiscLineTot.Text.Trim())) + (double.Parse(txtNBT.Text.Trim()) + _servChargTax1) + (double.Parse(txtVAT.Text.Trim()) + _servChargTax2)).ToString("0.00");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CalculateTaxAmounts()
        {
            double _subTot = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            Tax1Rate = double.Parse(txtNBTP.Text.Trim());
            Tax2Rate = double.Parse(txtVATP.Text.Trim());

            if (txtValueDiscount.Text.Trim() == string.Empty || txtValueDiscount.Text == null) txtValueDiscount.Text = "0.00";

            _subTot = double.Parse(txtGridTotalExcl.Text.Trim()) + double.Parse(txtServCharges.Text.Trim());

            txtValueDiscount.Text = ((_subTot * double.Parse(txtPersntage.Text.Trim())) / 100).ToString("0.00");

            _subTot = _subTot - double.Parse(txtValueDiscount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());

            _Tax1 = _subTot * Tax1Rate / 100;
            _Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            if (rdoNonVat.Checked)
            {
                txtNBT.Text = "0.00";
                txtVAT.Text = "0.00";
            }
            else
            {
                txtNBT.Text = _Tax1.ToString("0.00");
                txtVAT.Text = _Tax2.ToString("0.00");
            }

            txtNetTotal.Text = (_subTot + double.Parse(txtNBT.Text) + double.Parse(txtVAT.Text)).ToString("0.00");
            txtpaid.Text = (_subTot + double.Parse(txtNBT.Text) + double.Parse(txtVAT.Text)).ToString("0.00");
        }

        private double Temp_getTaxRate()
        {
            double _TempRate = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            try
            {
                double Tax1Rate = double.Parse(txtNBTP.Text.Trim());
                double Tax2Rate = double.Parse(txtVATP.Text.Trim());

                //_Tax1 = Math.Round(Tax1Rate, 2);
                //_Tax2 = Math.Round((Tax1Rate + 100) * Tax2Rate / 100, 2);

                //_TempRate = Math.Round(_Tax2 + _Tax1, 2);

                _Tax1 = Tax1Rate;
                _Tax2 = (Tax1Rate + 100) * Tax2Rate / 100;

                _TempRate = _Tax2 + _Tax1;

                _TempRate = _TempRate + 100;
                //_Rate=100+(100*Tax1Rate/100)+(
                return _TempRate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void rdoSVat_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (rdoSVat.Checked)
            //        FillInvoiceType();
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void txtPersntage_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                if (!decimal.TryParse(txtPersntage.Text.Trim(), out amount))
                {
                    txtPersntage.Text = "0.00";
                    return;
                }

                //if (txtPersntage.Text == string.Empty) txtPersntage.Text = "0.00";
                //if (txtGridTotalExcl.Text == string.Empty) txtGridTotalExcl.Text = "0.00";

                //txtValueDiscount.Text = ((double.Parse(txtGridTotalExcl.Text.Trim()) + double.Parse(txtServCharges.Text.Trim())) * double.Parse(txtPersntage.Text.Trim()) / 100).ToString("0.00");
                //InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();

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
                //CalculateGridAmounts();
                //CalculateTaxAmounts();
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
                decimal amount = 0;
                if (!decimal.TryParse(txtServCharges.Text.Trim(), out amount))
                {
                    txtServCharges.Text = "0.00";
                    return;
                }

                txtPersntage_TextChanged(sender, e);
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //txtGridTotalExcl.Text = (double.Parse(txtSubTot.Text.Trim()) + double.Parse(txtServCharges.Text.Trim())).ToString("0.00");
                //CalculateGridAmounts();
                //CalculateTaxAmounts();

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
            //try
            //{
            //    if (rdoNonVat.Checked)
            //    {
            //        if (rbtInclusive.Checked)
            //        {
            //            rbtInclusive.Checked = false;
            //            rbtExclusive.Checked = true;
            //            //MessageBox.Show("Invalid Tax Type....", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }
            //        CalculateGridAmounts();
            //        CalculateTaxAmounts();
            //    }

            //    if (rbtInclusive.Checked)
            //    {
            //        dgvdispactApplytoSales.Columns["colUnitPrice"].Visible = false;
            //        dgvdispactApplytoSales.Columns["colInclUnitPrice"].Visible = true;
            //        dgvdispactApplytoSales.Columns["colAmount"].Visible = false;
            //        dgvdispactApplytoSales.Columns["colAmountIncl"].Visible = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void rbtExclusive_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{

            //    if (rbtExclusive.Checked)
            //    {
            //        CalculateGridAmounts();
            //        CalculateTaxAmounts();

            //        dgvdispactApplytoSales.Columns["colUnitPrice"].Visible = true;
            //        dgvdispactApplytoSales.Columns["colInclUnitPrice"].Visible = false;
            //        dgvdispactApplytoSales.Columns["colAmount"].Visible = true;
            //        dgvdispactApplytoSales.Columns["colAmountIncl"].Visible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void dgvdispactApplytoSales_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //CalculateGridAmounts();
                //CalculateTaxAmounts();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtNBTP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVATP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                //if (!decimal.TryParse(txtVATP.Text.Trim(), out amount))
                //{
                //    txtVATP.Text = "0.00";
                //    return;
                //}

                //CalculateGridAmounts();
                //CalculateTaxAmounts();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FillDetails(txtInvoiceNo.Text.ToString());
                                   
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmInvoicing_Activated(object sender, EventArgs e)
        {
            try
            {                
                //FillDetails();
                //viewinvpayhistory(txtInvoiceNo.Text.ToString());                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void cmbCustomer_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (cmbCustomer.Value == null) return;
            //    DataSet _dts=new DataSet ();
            //    StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster where CutomerID='" + cmbCustomer.Value.ToString() + "' order by CutomerID";

            //    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
            //    dAdapt.Fill(_dts);

            //    txtcusName.Text = _dts.Tables[0].Rows[0][1].ToString();
            //    txtCusAdd1.Text = _dts.Tables[0].Rows[0][2].ToString();
            //    txtCusAdd2.Text = _dts.Tables[0].Rows[0][3].ToString();                 
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void clistbxSalesOrder_MouseClick(object sender, MouseEventArgs e)
        {

            try
            {
                if (clistbxSalesOrder.CheckedItems.Count == 0)
                {
                    txtLocation.Text = "";
                    txtSONumber.Text = "";
                    txtCustomerPO.Text = "";
                }
                String S12 = "Select CustomePO from tblDispatchOrder where DeliveryNoteNo='" + clistbxSalesOrder.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd12 = new SqlCommand(S12);
                SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                DataTable dt12 = new DataTable();
                da12.Fill(dt12);

                if (dt12.Rows.Count > 0)
                {
                    txtCustomerPO.Text = dt12.Rows[0].ItemArray[0].ToString();
                }
                String S = "Select distinct SONos from tblDispatchOrder where DeliveryNoteNo ='" + clistbxSalesOrder.SelectedItem.ToString().Split(':')[0].Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (txtSONumber.Text != string.Empty)
                    {
                        if (txtSONumber.Text == dt.Rows[0].ItemArray[0].ToString())
                        {
                            txtSONumber.Text = dt.Rows[0].ItemArray[0].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Please Select Delivery Notes Belongs to the Same Sales Order Number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clistbxSalesOrder.SetItemChecked(clistbxSalesOrder.SelectedIndex, false);
                            clistbxSalesOrder.Refresh();
                            clistbxSalesOrder_SelectedIndexChanged(sender, e);
                            return;
                        }
                    }
                    else
                    {
                        txtSONumber.Text = dt.Rows[0].ItemArray[0].ToString();
                    }
                }

                //================================================

                // PoValidation = 0;
                String S1 = "Select distinct WareHouseID from tblDispatchOrder where DeliveryNoteNo ='" + clistbxSalesOrder.SelectedItem.ToString().Split(':')[0].Trim() + "'";
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
                            MessageBox.Show("Please Select Delivery Notes belongs to the same Warehouse", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clistbxSalesOrder.SetItemChecked(clistbxSalesOrder.SelectedIndex, false);
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
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int rowCount = GetFilledRows();
                if (rowCount == 0 || Convert.ToDouble(dgvdispactApplytoSales[2, dgvdispactApplytoSales.CurrentRow.Index].Value) == 0 || txtLocation.Text == "")
                {
                    if (rowCount == 0)
                    {
                        MessageBox.Show("please Select a line That contain Serialized Item");
                    }
                    if (Convert.ToDouble(dgvdispactApplytoSales[3, dgvdispactApplytoSales.CurrentRow.Index].Value) == 0)
                    {
                        MessageBox.Show("please Enter Despatch Quantity");
                    }
                    if (txtLocation.Text == "")
                    {
                        MessageBox.Show("please Select a Warehouse");
                    }
                }
                else
                {
                    string ItemID = dgvdispactApplytoSales[0, dgvdispactApplytoSales.CurrentRow.Index].Value.ToString().Trim();
                    //check wether this item is serial ior not  by classs
                    string ItemClass = "";
                    String S = "Select * from tblItemMaster where ItemID  = '" + ItemID + "'";
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
                        frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Invoice", txtLocation.Text.ToString().Trim(),
                       ItemID, Convert.ToDouble(dgvdispactApplytoSales[2, dgvdispactApplytoSales.CurrentRow.Index].Value),
                       txtInvoiceNo.Text.Trim(), IsFind, clsSerializeItem.DtsSerialNoList, GetCheckedGRNList(), false, false);
                        ObjfrmSerialSubCommon.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("You must select a Serialize Stock Item");
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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

        private DataTable GetCheckedGRNList()
        {
            DataTable _dtblGRN = new DataTable();
            _dtblGRN.Columns.Add("DNote");

            try
            {
                for (int i = 0; i < clistbxSalesOrder.Items.Count; i++)
                {
                    if (clistbxSalesOrder.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string[] GRNNo = clistbxSalesOrder.Items[i].ToString().Split(':');

                        DataRow dr = _dtblGRN.NewRow();
                        dr["DNote"] = GRNNo[0];
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

        private void optCash_CheckedChanged(object sender, EventArgs e)
        {    
            grpamounttypes.Enabled = false;
            //if (optCash.Checked==true)
            combMode.Value = 1;
        }

        private void optCredit_CheckedChanged(object sender, EventArgs e)
        {            
            grpamounttypes.Enabled = false;
            //if (optCredit.Checked == true)
            combMode.Value = 2;
        }

        private void rdobtnCreditCard_CheckedChanged(object sender, EventArgs e)
        {           
            grpamounttypes.Enabled = true;
            //if (rdobtnCreditCard.Checked == true)
            combMode.Value = 1;
        }

        private void grdviewdata_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grdviewdata_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            for (int j = 0; j < grdviewdata.Rows.Count; j++)
            {
                if (grdviewdata.Rows[j].Cells["Card_Name"].Value != null && grdviewdata.Rows[j].Cells["Card_Am"].Value == null)
                {
                    if (e.ColumnIndex == grdviewdata.Columns["Card_Am"].Index)
                    {
                        int i;
                        if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
                        {
                            e.Cancel = true;
                            MessageBox.Show("Amount Must be Numeric");
                        }
                    }
                }
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }



        //Asanga
        private void viewinvpayhistory(string StrInvoiceNo)
        {
            try
            {
                string StrSql = "SELECT InvPaid,InvBalance,PayType from  tblInvoicePaymentHistory  WHERE InvoiceNo='" + StrInvoiceNo + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtpaid.Text = dt.Rows[0]["InvPaid"].ToString().Trim();  
                    txtbalance.Text = dt.Rows[0]["InvBalance"].ToString().Trim();
                    if (dt.Rows[0]["PayType"].ToString().Trim() == "Cash")
                    {
                        optCash.Checked = true;
                    }
                    if (dt.Rows[0]["PayType"].ToString().Trim() == "Credit")
                    {
                        optCredit.Checked = true;
                    }
                    if (dt.Rows[0]["PayType"].ToString().Trim() == "CreditCard")
                    {
                        rdobtnCreditCard.Checked = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Asanga
        public void PaycardCalculation()
        {
            try
            {
                double MultyAmount = 0;
                double NetAmount = 0;
                double Total = 0;
                for (int i = 0; i < grdviewdata.Rows.Count; i++)
                {
                    //Amount   
                    if (grdviewdata.Rows[i].Cells["Card_Am"].Value == null || grdviewdata.Rows[i].Cells["Card_Am"].Value.ToString().Trim() == string.Empty)
                    {
                        return;
                    }
                    else 
                    {
                        Total += Convert.ToDouble(grdviewdata.Rows[i].Cells["Card_Am"].Value);
                    }

                    txtcardamount.Text = Total.ToString("0.00");
                }
                txtcardamount.Text = Total.ToString("0.00");


                if (txtcardamount.Text != null)
                {
                    MultyAmount = double.Parse(txtcardamount.Text.ToString());
                }
                if (txtNetTotal.Text != null)
                {
                    NetAmount = double.Parse(txtNetTotal.Text.ToString());
                }
                //Netam = double.Parse(txtNetTotal.Text.ToString());
                if (MultyAmount > NetAmount)
                {
                    MessageBox.Show("Invoice Amount & Multipayment Amount Not Match....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch { }

        }

        private void grdviewdata_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void txtpaid_TextChanged(object sender, EventArgs e)
        {            
            //Asanga
            float NetAm;
            float PaidAm;
            float BalAm;
            NetAm = 0;
            BalAm = 0;
            PaidAm=0;
            NetAm = float.Parse(txtNetTotal.Text.ToString());
            PaidAm = float.Parse(txtpaid.Text.ToString());
            BalAm = NetAm - PaidAm;            
            txtbalance.Text = BalAm.ToString("0.00");
            //txtpaid.Text = PaidAm.ToString("0.00");
        }
     
        public void exporetReceipt_Cash_old(string StrReference)
        {
            
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;

            for (int i = 0; i < rowCount1; i++)
            {
                if (double.Parse(txtNetTotal.Text.ToString()) > 0)
                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(StrReference + "R");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(InvDate);//Date 
                    Writer.WriteEndElement();
                    //=================
                    //if (optCash.Checked)
                    //{

                    //    Writer.WriteStartElement("Payment_Method");
                    //    Writer.WriteString("Cash");//PayMethod
                    //    Writer.WriteEndElement();


                    //    Writer.WriteStartElement("Cash_Account");
                    //    Writer.WriteAttributeString("xsi:type", "paw:id");
                    //    Writer.WriteString(StrCashAccount);//Cash Account
                    //    Writer.WriteEndElement();
                    //}

                    Writer.WriteStartElement("Payment_Method");
                    Writer.WriteString("Cash");//PayMethod
                    Writer.WriteEndElement();
                    string cardAccount = "";
                    String S = "Select GL_Account from tblCreditData where CardType='Cash'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);
                    cardAccount = dt.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    Writer.WriteStartElement("Cash_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cardAccount);//Cash Account
                    Writer.WriteEndElement();
                    
                    Writer.WriteStartElement("Total_Paid_On_Invoices");
                    Writer.WriteString("-" + txtNetTotal.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReceiptNumber");
                    Writer.WriteString("R" + StrReference);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Distributions");
                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("InvoicePaid");
                    Writer.WriteString(StrReference);//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtNetTotal.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                              
                }
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();
        }
        public void exporetReceipt_Cash(string StrReference)
        {

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;

            for (int i = 0; i < rowCount1; i++)
            {
                if (double.Parse(txtNetTotal.Text.ToString()) > 0)
                {
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(StrReference + "R");
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    if (user.MergeAccUser)
                    {
                        Writer.WriteString(user.userName.ToString().Trim());
                    }
                    else
                    {
                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    }
                    //Writer.WriteString(cmbSalesRep.Value.ToString().Trim()); //(MergeUser.ToString());//

                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date ");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(InvDate);//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Payment_Method");
                    Writer.WriteString("Cash");//PayMethod
                    Writer.WriteEndElement();

                    string cardAccount = "";
                    String S = "Select GL_Account from tblCreditData where CardType='Cash'";
                    SqlCommand cmd1 = new SqlCommand(S);
                    SqlDataAdapter da1 = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt1 = new DataSet();
                    da1.Fill(dt1);
                    cardAccount = dt1.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                    Writer.WriteStartElement("Cash_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cardAccount);//Cash Account
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Total_Paid_On_Invoices");
                    Writer.WriteString("-" + txtNetTotal.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("ReceiptNumber");
                    Writer.WriteString("R" + StrReference);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions ");
                    Writer.WriteString("1");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Distributions");
                    Writer.WriteStartElement("Distribution");

                    Writer.WriteStartElement("InvoicePaid");
                    Writer.WriteString(StrReference);//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + txtNetTotal.Text.ToString().Trim());//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();
                    // Writer.Close();
                }
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();
            IsExport = true ;

        }
        public void exporetReceipt_Credit(string StrReference)
        {
            int intGrid;
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;

            for (int i = 0; i < rowCount1; i++)
            {
                if (rdobtnCreditCard.Checked)
                {
                    if (double.Parse(txtcardamount.Text.ToString()) > 0)
                    {
                        for (intGrid = 0; intGrid < grdviewdata.Rows.Count - 1; intGrid++)
                        {
                            if (grdviewdata.Rows[intGrid].Cells[0].Value != null && grdviewdata.Rows[intGrid].Cells[0].Value.ToString().Trim() != string.Empty)
                            {
                                Writer.WriteStartElement("PAW_Receipt");
                                Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                                Writer.WriteStartElement("Customer_ID");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Reference");
                                //(grdviewdata.Rows[intGrid].Cells["cblcname"].Value.ToString() == "Cash")
                                if (grdviewdata.Rows[intGrid].Cells[0].Value.ToString().ToUpper()  == "CASH")
                                {
                                    Writer.WriteString(StrReference + "R".ToString());
                                    Writer.WriteEndElement();
                                }
                                else if (grdviewdata.Rows[intGrid].Cells[0].Value.ToString().ToUpper() == "CHECK")
                                {
                                    Writer.WriteString(grdviewdata.Rows[intGrid].Cells[1].Value.ToString());
                                    Writer.WriteEndElement();
                                }
                                else if (grdviewdata.Rows[intGrid].Cells[0].Value.ToString().ToUpper() == "ADVANCE")
                                {
                                    Writer.WriteString(StrReference + "ADV".ToString());
                                    Writer.WriteEndElement();
                                }
                                else
                                {
                                    Writer.WriteString(grdviewdata.Rows[intGrid].Cells[1].Value.ToString() + "R");
                                    Writer.WriteEndElement();
                                }

                                Writer.WriteStartElement("Date ");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(InvDate);//Date 
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Sales_Representative_ID");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                if (user.MergeAccUser)
                                {
                                    Writer.WriteString(user.userName.ToString().Trim());
                                }
                                else
                                {
                                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                                }
                                Writer.WriteEndElement();

                                Writer.WriteStartElement("Payment_Method");
                                Writer.WriteString(grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString());//PayMethod
                                Writer.WriteEndElement();
                                string cardAccount = "";
                                String S = "Select GL_Account from tblCreditData where CardType='" + grdviewdata.Rows[intGrid].Cells["Card_Name"].Value.ToString() + "'";
                                SqlCommand cmd = new SqlCommand(S);
                                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                DataSet dt = new DataSet();
                                da.Fill(dt);
                                cardAccount = dt.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                                Writer.WriteStartElement("Cash_Account");
                                Writer.WriteAttributeString("xsi:type", "paw:id");
                                Writer.WriteString(cardAccount);//Cash Account
                                Writer.WriteEndElement();

                               
                            }


                            Writer.WriteStartElement("Total_Paid_On_Invoices");
                            Writer.WriteString("-" + grdviewdata.Rows[intGrid].Cells["Card_Am"].Value.ToString().Trim());//PayMethod
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("ReceiptNumber");
                            Writer.WriteString("R" + StrReference);
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Number_of_Distributions");
                            Writer.WriteString("1");
                            Writer.WriteEndElement();


                            Writer.WriteStartElement("Distributions");
                            Writer.WriteStartElement("Distribution");

                            Writer.WriteStartElement("InvoicePaid");
                            Writer.WriteString(StrReference);//PayMethod
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" + grdviewdata.Rows[intGrid].Cells["Card_Am"].Value.ToString().Trim());//PayMethod
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();

                        }
                    }
                }
                // Writer.Close();
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();
            IsExport = true ;
            //Connector abc = new Connector();//export to peach tree
            //abc.i
            //abc.Import_Receipt_JournalOnline();



        }

        private void grdviewdata_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void grdviewdata_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            PaycardCalculation();
        }

        private void cmbCustomer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }
        public void InvoiceNewCalculation(Int64 iInvoiceType)
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
                    double netvalue = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVATP.Text.ToString());  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTP.Text.ToString());  //2%

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            //ug.ActiveCell.Row.Cells

                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                           // dgvr.Cells["colAmountIncl"].Value.ToString()
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value.ToString());//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value.ToString()) / 100;
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 3, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 3, MidpointRounding.AwayFromZero);

                           // dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 3, MidpointRounding.AwayFromZero);//price without vat=102
                            dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 3, MidpointRounding.AwayFromZero);//price without vat=102
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value.ToString());

                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 3, MidpointRounding.AwayFromZero);
                            dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 3, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colInclUnitPrice"].Value = dblExcluisvePrice;//100

                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;

                            //dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            //dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal;

                            dgvr.Cells["colAmount"].Value = Math.Round(dblLineQty * dblLinePrice, 3, MidpointRounding.AwayFromZero);

                            // dgvr.Cells["colAmountIncl"].Value = Math.Round(Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value) * 100 / (100 + dblVATPer), 3, MidpointRounding.AwayFromZero);
                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["colAmount"].Value) - Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineTax, 3, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 3, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);

                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);
                           // dgvr.Activated = true;
                           // ug.PerformAction(.CommitRow);
                           // ug.PerformAction(UltraGridAction.ExitEditMode);
                        }
                        txtSubTot.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString();
                        txtVAT.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = Convert.ToString(Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero));

                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 4, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;


                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;



                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 3, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 3, MidpointRounding.AwayFromZero);


                            //dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            //dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            //dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            //dblLineTax = Math.Round(Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value) - Convert.ToDouble(dgvr.Cells["colAmount"].Value), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineTax, 3, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 3, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);

                          //  dgvr.Activated = true;

                           // ug.PerformAction(UltraGridAction.CommitRow);
                           // ug.PerformAction(UltraGridAction.ExitEditMode);
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString("N2");
                        txtSubTot.Text = dblSubTotal.ToString("N2");
                        txtVAT.Text = dblTotalVAT.ToString("N2");
                        txtNBT.Text = dblTotalNBT.ToString("N2");
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString("N2"); 

                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            // dgvr.Cells["colAmountIncl"].Value = Math.Round(((dblLineQty * dblLinePrice)), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 3, MidpointRounding.AwayFromZero);
                            //VAT
                            // dblLineVAT = Math.Round(Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value) - Convert.ToDouble(dgvr.Cells["colAmount"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 3, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 3, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                          //  dgvr.Activated = true;
                           // ug.PerformAction(UltraGridAction.CommitRow);
                           // ug.PerformAction(UltraGridAction.ExitEditMode);
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString("N2");
                        txtSubTot.Text = dblSubTotal.ToString("N2");
                        txtVAT.Text = dblTotalVAT.ToString("N2");
                        txtNBT.Text = dblTotalNBT.ToString("N2");
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString("N2");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;

                    //double dblTotNBT = 0;
                    //double dblInvTotal = 0;

                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVATP.Text);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTP.Text);  //2%
                    double totexlprice = 0;
                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            //ug.ActiveCell.Row.Cells
                            
                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty)
                                break;

                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colDiscLineAmt"].Value = 0.00;
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                          
                            
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice =dblLinePrice - (dblLinePrice * dbdiscountPer);
                            dblLinediscountAmount = dblLinePrice - dblDiscountedPrice;
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value);
                            dblLineVAT = ((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty);
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                            dblLineNBT = ((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty);//, 2, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colInclUnitPrice"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;                           
                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal;
                            dgvr.Cells["colAmount"].Value = dblLineQty * dblLinePrice;                            
                            dgvr.Cells["colDiscLineAmt"].Value = dblLinediscountAmount;
                            //VAT
                            dblLineTax = (dblLineNBT + dblLineVAT);
                            dgvr.Cells["colLineTax"].Value = dblLineTax;                            
                            //Inv Sub Total
                            dblSubTotal = dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value);
                            dblTotalVAT = dblTotalVAT + dblLineVAT;
                            dblTotalNBT = dblTotalNBT + dblLineNBT;
                            dblTotalLinedDiscount = dblTotalLinedDiscount + dblLinediscountAmount;                           
                            //* double.Parse(dblLineQty.ToString()).ToString();// (double.Parse(txtGridTotalExcl.Text) + (dblDiscountedPrice * Convert.ToDouble(dgvr.Cells["colInvQty"].Value))).ToString();                            
                        }
                        totexlprice = dblDiscountedPrice * dblLineQty;
                        //txtGridTotalExcl.Text = totexlprice.ToString();
                        txtSubTot.Text = dblSubTotal.ToString();
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString();
                        txtVAT.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();

                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        txtpaid.Text = txtNetTotal.Text.ToString();
                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {

                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                            if (dgvr.Cells["colInclUnitPrice"].Value == null || dgvr.Cells["colInclUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInclUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colAmountIncl"].Value = 0.00;
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineTax"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colDiscLineAmt"].Value = 0.00;
                            

                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["colDiscLineAmt"].Value = dblLinediscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            //dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 3, MidpointRounding.AwayFromZero);
                            //dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            //dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            //ugR.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dgvr.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            //dblLineTax = Math.Round(Convert.ToDouble(ugR.Cells["colAmountIncl"].Value) - Convert.ToDouble(ugR.Cells["colAmount"].Value), 3, MidpointRounding.AwayFromZero);

                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString();
                        txtSubTot.Text = dblSubTotal.ToString();
                        txtVAT.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        txtpaid.Text = txtNetTotal.Text.ToString(); 
                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (DataGridViewRow dgvr in dgvdispactApplytoSales.Rows)
                        {
                            if (dgvr.Cells["colUnitPrice"].Value == null || dgvr.Cells["colUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["Discount"].Value = 0.00;
                            if (dgvr.Cells["colInvQty"].Value == null || dgvr.Cells["colInvQty"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInvQty"].Value = 0.00;
                            if (dgvr.Cells["colInclUnitPrice"].Value == null || dgvr.Cells["colInclUnitPrice"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colInclUnitPrice"].Value = 0.00;
                            if (dgvr.Cells["colAmountIncl"].Value == null || dgvr.Cells["colAmountIncl"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colAmountIncl"].Value = 0.00;
                            if (dgvr.Cells["colLineTax"].Value == null || dgvr.Cells["colLineTax"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colLineTax"].Value = 0.00;
                            if (dgvr.Cells["Discount"].Value == null || dgvr.Cells["Discount"].Value.ToString().Trim() == string.Empty) dgvr.Cells["colDiscLineAmt"].Value = 0.00;

                            if (dgvr.Cells["colItem"].Value == null || dgvr.Cells["colItem"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(dgvr.Cells["colUnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(dgvr.Cells["colInvQty"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(dgvr.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            dgvr.Cells["colAmountIncl"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            // ugR.Cells["colAmountIncl"].Value = Math.Round(((dblLineQty * dblLinePrice)), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colDiscLineAmt"].Value = dblLinediscountAmount;

                            dgvr.Cells["colAmount"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);
                            //VAT
                            // dblLineVAT = Math.Round(Convert.ToDouble(ugR.Cells["colAmountIncl"].Value) - Convert.ToDouble(ugR.Cells["colAmount"].Value), 3, MidpointRounding.AwayFromZero);
                            dgvr.Cells["colLineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(dgvr.Cells["colAmountIncl"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            //Inv TAX Total
                            // dblTotVAT = Math.Round(dblTotVAT + dblLineVAT, 3, MidpointRounding.AwayFromZero);
                            //ugR.Activated = true;
                            //ug.PerformAction(UltraGridAction.CommitRow);
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtValueDiscount.Text = dblTotalLinedDiscount.ToString();
                        txtSubTot.Text = dblSubTotal.ToString();
                        txtVAT.Text = dblTotalVAT.ToString();
                        txtNBT.Text = dblTotalNBT.ToString();
                        txtNetTotal.Text = (dblSubTotal + dblTotalNBT + dblTotalVAT).ToString();
                        txtpaid.Text = txtNetTotal.Text.ToString();
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

                    dblGrossTotalF = Convert.ToDouble(txtSubTot.Text.ToString().Trim());
                    dblServiceChgF = Convert.ToDouble(txtServCharges.Text.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtPersntage.Text.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    txtValueDiscount.Text = (dblTotalDiscount +
                        (double.Parse(txtServCharges.Text.ToString()) * double.Parse(txtPersntage.Text.ToString()) / 100)).ToString();

                    double _AmountWithoutDisc = double.Parse(txtSubTot.Text.ToString()) + 
                        double.Parse(txtDiscLineTot.Text) - 
                        double.Parse(txtValueDiscount.Text.ToString()) + 
                        double.Parse(txtServCharges.Text.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                        txtNBT.Text = (_AmountWithoutDisc * double.Parse(txtNBTP.Text.ToString()) / 100).ToString();
                        //txtVAT.Text = ((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                        //    double.Parse(txtVATP.Text.ToString()) / 100).ToString();

                        txtVAT.Text = double.Parse(((_AmountWithoutDisc + double.Parse(txtNBT.Text.ToString())) *
                            double.Parse(txtVATP.Text.ToString()) / 100).ToString()).ToString();

                    }

                    dblNBTF = Convert.ToDouble(txtNBT.Text.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVAT.Text.ToString().Trim());
                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF;// +double.Parse(txtDiscAmount.Value.ToString());
                    txtNetTotal.Text = dblNetTotalF.ToString("N2");
                    
                    //Asanga
                    txtpaid.Text = dblNetTotalF.ToString("N2");
                    if (cmbInvoiceType.Value.ToString() == "1")
                        txtValueDiscount.Text = ((Convert.ToDouble(txtGridTotalExcl.Text) -
                        Convert.ToDouble(txtDiscLineTot.Text)) *
                        Convert.ToDouble(txtPersntage.Text) / 100).ToString();
                    else
                    txtValueDiscount.Text = (Convert.ToDouble(txtSubTot.Text.Trim()) * Convert.ToDouble(txtPersntage.Text) / 100).ToString();
                    txtNBT.Text = Convert.ToDouble(txtNBT.Text.ToString().Trim()).ToString("0.00"); //txtNBT.Text.ToString("N2");
                    txtVAT.Text = Convert.ToDouble(txtVAT.Text.ToString().Trim()).ToString("0.00");  //txtVAT.Text.ToString("N2");
                }
            }
            catch { }
        }

        public void FooterCalculation_test()
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

                    dblGrossTotalF = Convert.ToDouble(txtSubTot.Text.ToString().Trim());
                   // dblGrossTotalF = Convert.ToDouble(txtGridTotalExcl.Text.ToString().Trim());
                    //txtValueDiscount

                    dblServiceChgF = Convert.ToDouble(txtServCharges.Text.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF + dblServiceChgF) * Convert.ToDouble(txtPersntage.Text.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;
                    txtValueDiscount.Text = dblTotalDiscount.ToString("N2");

                    dblNBTF = Convert.ToDouble(txtNBT.Text.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVAT.Text.ToString().Trim());
                    dblNetTotalF = (dblGrossTotalF + dblServiceChgF + dblNBTF + dblVATF) - (dblDiscountF);
                    txtNetTotal.Text = dblNetTotalF.ToString("N2");

                    //Asanga                                            
                    txtpaid.Text = dblNetTotalF.ToString("N2").Trim();

                }
            }
            catch { }
        }

        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                double Tax1Rate = double.Parse(txtNBTP.Text.Trim());
                double Tax2Rate = double.Parse(txtVATP.Text.Trim());
                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                {
                    dgvdispactApplytoSales.Columns["colUnitPrice"].Visible = true;
                    dgvdispactApplytoSales.Columns["colInclUnitPrice"].Visible = false;
                    dgvdispactApplytoSales.Columns["colAmount"].Visible = true;
                    dgvdispactApplytoSales.Columns["colAmountIncl"].Visible = false;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//Exclusive
                {
                    dgvdispactApplytoSales.Columns["colUnitPrice"].Visible = true;
                    dgvdispactApplytoSales.Columns["colInclUnitPrice"].Visible = false;
                    dgvdispactApplytoSales.Columns["colAmount"].Visible = true;
                    dgvdispactApplytoSales.Columns["colAmountIncl"].Visible = false;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                {
                    dgvdispactApplytoSales.Columns["colUnitPrice"].Visible = true;
                    dgvdispactApplytoSales.Columns["colInclUnitPrice"].Visible = false;
                    dgvdispactApplytoSales.Columns["colAmount"].Visible = true;
                    dgvdispactApplytoSales.Columns["colAmountIncl"].Visible = false;
                
                    Tax1Rate = 0.00;
                    Tax2Rate = 0.00;
                }
                //Invoice calculation
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbAR_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }
        private void CheckOnHandQty(string Itemcode, double ReqQty, SqlConnection con, SqlTransaction myTrans)
        {
            if (user.IsMinusAllow != true)
            {      
                SqlCommand myCommand2 = new SqlCommand("select ItemID,qty from View_ItemWise_ItemMaster where ItemID = '" + Itemcode.ToString().Trim() + "' and ItemClass <> '4'", con, myTrans);
                SqlDataAdapter da1 = new SqlDataAdapter(myCommand2);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    double stkqty = 0;
                    stkqty = Convert.ToDouble((dt1.Rows[0]["qty"].ToString()));
                    if (ReqQty > stkqty)
                    {
                        MessageBox.Show("Insufficiency Quantity....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        isok = true;
                        return;
                    }                    
                }

            }

        }
        private void cmbSalesRep_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtpaid_Click(object sender, EventArgs e)
        {
            txtpaid.Focus(); 
        }

        private void dgvdispactApplytoSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvdispactApplytoSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdispactApplytoSales.CurrentRow.Cells[0].Value != null)
            {
                if (IsThisItemSerial(dgvdispactApplytoSales.CurrentRow.Cells[0].Value.ToString()))
                    toolStripButton1.Enabled = true;
                else
                    toolStripButton1.Enabled = false;
            }   
        }

        private void ultraPanel1_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void txtGridTotalExcl_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void combMode_ValueChanged(object sender, EventArgs e)
        {
            

        }

        private void combMode_Validating(object sender, CancelEventArgs e)
        {
            if (Convert.ToInt32(combMode.Value) == 2)
            {
                optCredit.Checked = true;
            }
            else
            {
                optCash.Checked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            string SONo = "";
            myConnection.Open();
            myTrans = myConnection.BeginTransaction();

            int rowCount = GetFilledRows();
            for (int i = 0; i < rowCount; i++)
            {
                UpdateItemWarehouse(dgvdispactApplytoSales.Rows[i].Cells["colItem"].Value.ToString(), txtLocation.Text.ToString().Trim(), Convert.ToDouble(dgvdispactApplytoSales.Rows[i].Cells["colInvQty"].Value), myConnection, myTrans);
            }
            
            StrSql = "SELECT  DeliveryNoteNos FROM tblSalesInvoices WHERE  InvoiceNo='" + txtrefno.Text.Trim() + "'";
            SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                SONo = dt.Rows[0].ItemArray[0].ToString().Trim();
            }
            StrSql = "";
            StrSql = "DELETE FROM [tbItemlActivity] WHERE  TranNo='" + txtrefno.Text.Trim() + "'";
            SqlCommand sqcom1 = new SqlCommand(StrSql, myConnection, myTrans);
            sqcom1.CommandType = CommandType.Text;
            sqcom1.ExecuteNonQuery();

            StrSql = "";
            StrSql = "DELETE FROM [tblSalesInvoices] WHERE  InvoiceNo='" + txtrefno.Text.Trim() + "'";
            SqlCommand sqcom2 = new SqlCommand(StrSql, myConnection, myTrans);
            sqcom2.CommandType = CommandType.Text;
            sqcom2.ExecuteNonQuery();

            StrSql = "UPDATE tblSalesOrderTemp set IsfullDispatch ='False' where  SalesOrderNo='" + SONo.ToString().Trim() + "'";
            SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
            command1.CommandType = CommandType.Text;
            command1.ExecuteNonQuery();

            myTrans.Commit();
            MessageBox.Show("Invoice Successfuly Rollbacked.", "Information", MessageBoxButtons.OK);
        }
        
    }
}