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
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using Infragistics.Win.UltraWinGrid;

namespace UserAutherization
{   

    public partial class frmGRN : Form
    {
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon(); 
        // Connector conn = new Connector();
        public DSGRN objGRN = new DSGRN();
        public DsSupplierInvoice ds = new DsSupplierInvoice();
        // public DSDeliveryNotes  DSDispatch = new  DSDeliveryNotes();
        //int flglist = 0; // this is to check whether list is loaded or not
        public static string ConnectionString;
        public static DateTime UserWiseDate = System.DateTime.Now;

        public string StrSql;
        public string StrAP = null;

        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        bool IsFind = false;

        double dblUOMRatio = 1;
        double dblQuantity = 0;
        double dblConverteQty = 0;

        public int Printype = 0;

        public string sMsg = "Warehouse Module - Good Received Note";

        public frmGRN()
        {
            setConnectionString();
            InitializeComponent();
            cmbVendorSelect.Focus();
            IsFind = false;
        }

        public frmGRN(string GRNNo)
        {
            setConnectionString();
            InitializeComponent();
            cmbVendorSelect.Focus();
            CheckAcivated = true;
            IsFind = true;
            flag = true;
            clsSerializeItem.DtsSerialNoList.Rows.Clear();
        }

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
        public bool flag = false;

        public DataTable FillVendor()
        {
            DataTable dataTable = new DataTable("Vendor");
            string ConnString = ConnectionString;
            string sql = "Select VendorID,VendorName from tblVendorMaster";// where ItemClass!='8' and  ItemClass!='5'  and ItemClass!='3'";
            SqlConnection Conn = new SqlConnection(ConnString);
            SqlDataReader reader = null;

            try
            {                
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                dataTable.Columns.Add("VendorID", typeof(String));
                dataTable.Columns.Add("VendorName", typeof(String));
                // dataTable.Columns.Add("","");
                Conn.Open();
                reader = cmd.ExecuteReader();
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
                reader.Close();
                Conn.Close();
                throw ex;
            }
        }    

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



        // The follwing code segment load the warehouse data to combo box
        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
               // cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;  
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //the following code segment oad the vendor masterdata to table

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
        //==============Following Cod esegment load the AR Accounts=============
   
        //==========================================
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

        public int avoidloadvcmb = 0;
        bool savecomplete = false;
        private void frmInvoice_Load(object sender, EventArgs e)
        {                       
            
            try
            {
                if (!IsFind)
                {
                    GetARAccount();//Infragistics
                    GetAPAccount();
                    GetWareHouseDataSet();//Infragistics
                    GetVendorDataset();//Infragistics
                    //CheckAcivated = false;
                    LoadTaxMethod();
                    GetCurrentUserDate();
                    avoidloadvcmb = 1;
                    flag = false;
                    load_Decimal();
                    TaxRateLoad();
                    cmbVendorSelect.Focus();
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
            btnProcess.Enabled = false;
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
                //objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        
        
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
                    //UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    //dtpDispatchDate.Value = UserWiseDate;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void LoadPurchaseOrder(string VendorID)
        {
            try
            {
                //sanjeewa added this code segment to refresh deatails with vendor
                dgvGRNTransaction.Rows.Clear();
                txtTotalAmount.Text = "0.00";
              //  cmbAR.Text = "";
              //  cmbWarehouse.Text = user.StrDefaultWH;
                //----------------------

                clistbxSalesOrder.Items.Clear();
                bool ISSoClosed = false;
                bool dispatch = false;
                String S = "Select distinct(PONumber) from tblPurchaseOrder where VendorID='" + VendorID + "' and WHID ='"+cmbWarehouse.Text.ToString()+"' and IsFullGRN='" + dispatch + "' and IsPOClosed='" + ISSoClosed + "' and IsActive='"+false+"'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message,sMsg, ex.StackTrace);
            }
        }

        private void cmbSalesOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void AmountCalculation()
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

                for (int a = 0; a < dgvGRNTransaction.Rows.Count - 1; a++)
                {
                    if (dgvGRNTransaction.Rows[a].Cells[2].Value != null && dgvGRNTransaction.Rows[a].Cells[5].Value != null)
                    {
                        dgvGRNTransaction.Rows[0].Cells[6].Style.BackColor = R1;
                        dgvGRNTransaction.Rows[0].Cells[2].Style.BackColor = R1;

                        ReiveQty = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value);
                        //========================================================================
                        //for (int b = 0; b < dgvPOLIst.Rows.Count - 1; b++)
                        //{
                        //    POQty = Convert.ToDouble(dgvPOLIst.Rows[b].Cells[3].Value);
                        //    if (ReiveQty >= POQty)
                        //    {
                        //        dgvPOLIst.Rows[b].Cells[4].Value = ReiveQty;
                        //    }
                        //}

                        Unitprice = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                        DiscountRate = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value) / 100;

                        Amount = (ReiveQty * Unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8
                    }
                }
                txtTotalAmount.Text = TotalAmount.ToString("N2");
                txtNetTotal.Text = TotalAmount.ToString("N2");
            }
            catch(Exception ex)
            {
                System.Drawing.Color R1 = new Color();
                R1 = Color.Yellow;
                MessageBox.Show("Enter Numeric Value");
                txtTotalAmount.Text = "0";
                for (int a = 0; a < dgvGRNTransaction.Rows.Count - 1; a++)
                {
                    dgvGRNTransaction.Rows[0].Cells[6].Style.BackColor = R1;
                    dgvGRNTransaction.Rows[0].Cells[2].Style.BackColor = R1;
                } throw ex;
            }
        }



        private void DatgridCellEndEditEvent()
        {
            try
            {
                int rowCount = GetFilledRows();
                double ReiveQty = 0;
                double Unitprice = 0.00;
                double DiscountRate = 0.00;
                double DiscountAmount = 0.00;
                double Amount = 0.00;
                double Amount1 = 0.00;
                double TotalAmount = 0.00;

                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvGRNTransaction.Rows[a].Cells[3].Value != null && dgvGRNTransaction.Rows[a].Cells[5].Value != null)
                    {
                        dgvGRNTransaction.Rows[a].Cells[3].Value = dgvGRNTransaction.Rows[a].Cells[2].Value;
                        ReiveQty = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value);
                        Unitprice = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                        DiscountRate = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value) / 100;
                        Amount = (ReiveQty * Unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;
                        if (Decimalpoint == 0)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N5");
                        }
                        TotalAmount = TotalAmount + Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8
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
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void dgvdispactApplytoSales_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //System.Drawing.Color R1 = new Color();
                //R1 = Color.White;
                int rowCount = GetFilledRows();
                double ReiveQty = 0;
                double Unitprice = 0.00;
                double DiscountRate = 0.00;
                double DiscountAmount = 0.00;
                double Amount = 0.00;
                double Amount1 = 0.00;
                double TotalAmount = 0.00;

                // double POQty = 0;//this is a po wise quantity

                for (int a = 0; a < rowCount; a++)
                {
                    if (dgvGRNTransaction.Rows[a].Cells[3].Value != null && dgvGRNTransaction.Rows[a].Cells[5].Value != null)
                    {
                        //dgvGRNTransaction.Rows[0].Cells[6].Style.BackColor = R1;
                        //dgvGRNTransaction.Rows[0].Cells[2].Style.BackColor = R1;

                        ReiveQty = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value);
                        Unitprice = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                        DiscountRate = Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value) / 100;

                        Amount = (ReiveQty * Unitprice);
                        DiscountAmount = Amount * DiscountRate;
                        Amount1 = Amount - DiscountAmount;

                        if (Decimalpoint == 0)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString();
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N1");
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N5");
                        }
                        //  dgvGRNTransaction.Rows[a].Cells[7].Value = Amount1.ToString("N2");

                        TotalAmount = TotalAmount + Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[7].Value);// sanjeewa change cell value 7 into 8
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
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connector conn = new Connector();
                conn.IssueAdjustmentExportGetFromP();             
                MessageBox.Show("PO Loaded Successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
           
        }
        public void LoadSalesLines(string CheckItem)
        {
            try
            {                
                String S = "Select * from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                             
                String S1 = "Select ItemID,Quantity,Description,GLAccount,UnitPrice from tblSalesOrder where SalesOrderNo = '" + CheckItem + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt.Rows.Count > 0)
                {                
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {                 
                        dgvGRNTransaction.Rows.Add();
                        dgvGRNTransaction.Rows[i].Cells[0].Value = dt1.Rows[i].ItemArray[0].ToString();
                        dgvGRNTransaction.Rows[i].Cells[1].Value = dt1.Rows[i].ItemArray[1].ToString();
                        dgvGRNTransaction.Rows[i].Cells[2].Value = "0.00";
                        dgvGRNTransaction.Rows[i].Cells[3].Value = dt1.Rows[i].ItemArray[2].ToString();
                        dgvGRNTransaction.Rows[i].Cells[4].Value = dt1.Rows[i].ItemArray[3].ToString();
                        dgvGRNTransaction.Rows[i].Cells[5].Value = dt1.Rows[i].ItemArray[4].ToString();
                        dgvGRNTransaction.Rows[i].Cells[6].Value = "0.00";
                    }
                }
                else
                {                    
                }
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
        

        private void clistbxSalesOrder_Click(object sender, EventArgs e)
        {

        }
        public string ReturnCheckPO(string GRN_No)
        {
            string CheckPO = "";

            try
            {
                string GRNNo = GRN_No.Replace("'", "").Trim();
                String S1 = "Select PONos from tblGRNTran where GRN_NO='" + GRNNo + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    CheckPO = dt1.Rows[0].ItemArray[0].ToString();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return CheckPO;
        }

        bool MultiplePOSelect = false;
        private void clistbxSalesOrder_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (clistbxSalesOrder.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
                {
                    e.NewValue = e.CurrentValue;
                    MultiplePOSelect = true;
                    MessageBox.Show("Multiple PO Numbers can't be selected...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                  
                 //   txtTotalAmount.Text = "0.00";
                    return;
                }
                else
                {
                    MultiplePOSelect = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        public ArrayList ArraySONO = new ArrayList();//Sales orders related to a custmer

        public string SelectSONums1 = "";//saving purpose


        private void clistbxSalesOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MultiplePOSelect == true)
            {
                return;
            }

            try
            {
                txtTotalAmount.Text = "0.00";
                double Ordrqty = 0.00;
                double ReqQty = 0.00;
                double UnitPrice = 0.00;
                double Amount = 0.00;
                SelectSONums1 = "";
                string SelectSONums = "";
                dgvGRNTransaction.Rows.Clear();
                // dgvPOLIst.Rows.Clear();
                int step = 0;
                int i = 0;
                ArraySONO.Clear();


                while (i < clistbxSalesOrder.Items.Count)
                {
                    if (clistbxSalesOrder.GetItemChecked(i) == true)
                    {
                        step++;
                        string[] SOIDs = new string[2];
                        SOIDs = clistbxSalesOrder.Items[i].ToString().Split('*');
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

                if (SelectSONums != "" || SelectSONums != string.Empty)
                {

                    String S1 = "Select * from tblGRNTran where VendorID = '" + cmbVendorSelect.Text + "' and IsActive ='" + true + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da2 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {
                        MessageBox.Show("There is unprocess GRN :- "+dt2.Rows[0].ItemArray[0].ToString()+" available for this vendor, Please process in order to continue!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }
                }
                if (SelectSONums.Length != 0)
                {
                    SelectSONums = SelectSONums.Substring(0, SelectSONums.Length - 1);
                    DataSet ds = new DataSet();
                    //  DataSet ds1 = new DataSet();
                    ds = ReturnSOList(SelectSONums);
                    // ds1 = ReturnSOList1(SelectSONums);
                    try
                    {
                        string CusPO = ReturnCusPO(SelectSONums);
                        txtCustomerSO.Text = CusPO;
                    }
                    catch { }

                    for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                    {
                        dgvGRNTransaction.Rows.Add();
                    }
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        //ItemId,Description,RemainQty,GRNQty,UOM,UnitPrice,GL_Account,Amount,PODisNum
                        //dgvdispactApplytoSales.Rows.Add();
                        dgvGRNTransaction.Rows[j].Cells[0].Value = ds.Tables[0].Rows[j]["ItemId"].ToString().Trim();// Item ID
                        dgvGRNTransaction.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j]["Description"].ToString().Trim();// Item ID
                       // ReqQty =Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"].ToString().Trim());// Convert.ToInt64(ds.Tables[0].Rows[j].ItemArray["RemainQty"].ToString() );
                        ReqQty = 0;
                        Ordrqty = Convert.ToDouble(ds.Tables[0].Rows[j]["GRNQty"].ToString().Trim());// Convert.ToInt64(ds.Tables[0].Rows[j].ItemArray["GRNQty"].ToString());                      
                        double RecivedQty = Convert.ToDouble(ds.Tables[0].Rows[j]["RemainQty"].ToString().Trim());
                        UnitPrice = Convert.ToDouble(ds.Tables[0].Rows[j]["UnitPrice"].ToString().Trim()); // Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray["UnitPrice"]);
                        Amount = Convert.ToDouble(ds.Tables[0].Rows[j]["Amount"].ToString().Trim());  //Convert.ToDouble(ds.Tables[0].Rows[j].ItemArray["Amount"]);
                        Ordrqty = Amount / UnitPrice;
                        if (DecimalpointQuantity == 0)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N0");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N0");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0";

                        }
                        else if (DecimalpointQuantity == 1)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N1");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N1");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0.0";
                        }
                        else if (DecimalpointQuantity == 2)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N2");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N2");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0.00";
                        }
                        else if (DecimalpointQuantity == 3)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N3");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N3");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0.000";
                        }
                        else if (DecimalpointQuantity == 4)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N4");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N4");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0.0000";
                        }
                        else if (DecimalpointQuantity == 5)
                        {
                            dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N5");
                            dgvGRNTransaction.Rows[j].Cells[2].Value = RecivedQty.ToString("N5");
                            dgvGRNTransaction.Rows[j].Cells[10].Value = "0.00000";
                        }

                        //if (DecimalpointQuantity == 0)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("");
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = Ordrqty.ToString("");
                        //}
                        //else if (DecimalpointQuantity == 1)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N1");
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = Ordrqty.ToString("N1");
                        //}
                        //else if (DecimalpointQuantity == 2)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N2");
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = Ordrqty.ToString("N2");
                        //}
                        //else if (DecimalpointQuantity == 3)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N3");
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = Ordrqty.ToString("N3");
                        //}
                        //else if (DecimalpointQuantity == 4)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = ReqQty.ToString("N4");
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = Ordrqty.ToString("N4");
                        //}
                        //else if (DecimalpointQuantity == 5)
                        //{
                        //    dgvGRNTransaction.Rows[j].Cells[3].Value = ReqQty.ToString("N5");
                        //    dgvGRNTransaction.Rows[j].Cells[2].Value = Ordrqty.ToString("N5");
                        //}
                        //dgvGRNTransaction.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j]["Description"].ToString().Trim();//Description
                        dgvGRNTransaction.Rows[j].Cells[4].Value = ds.Tables[0].Rows[j]["UOM"].ToString().Trim();//UOM                        
                        // dgvGRNTransaction.Rows[j].Cells[5].Value = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();

                        if (Decimalpoint == 0)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N2");
                           // dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString(); 
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N2"); 
                        }
                        else if (Decimalpoint == 1)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N1");
                           // dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString("N1"); 
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N1"); 
                        }
                        else if (Decimalpoint == 2)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N2");
                          //  dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString("N2");
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N2");
                        }
                        else if (Decimalpoint == 3)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N3");
                           // dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString("N3");
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N3");
                        }
                        else if (Decimalpoint == 4)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N4");
                           // dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString("N4");
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N4");
                        }
                        else if (Decimalpoint == 5)
                        {
                            dgvGRNTransaction.Rows[j].Cells[5].Value = UnitPrice.ToString("N5");
                            //dgvGRNTransaction.Rows[j].Cells[6].Value = Amount.ToString("N5");
                            dgvGRNTransaction.Rows[j].Cells[7].Value = Amount.ToString("N5");
                        }
                        dgvGRNTransaction.Rows[j].Cells[8].Value = ds.Tables[0].Rows[j]["GL_Account"].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[9].Value = ds.Tables[0].Rows[j]["PODisNum"].ToString().Trim();                  
                    }
                }

                dgvGRNTransaction.Columns[0].ReadOnly = true;
                dgvGRNTransaction.Columns[1].ReadOnly = true;
                dgvGRNTransaction.Columns[2].ReadOnly = true;  
                dgvGRNTransaction.Columns[7].ReadOnly = true;

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();

            try
            {

           
                //String S = "Select ItemId,RemainQty,Description,GL_Account,UnitPrice,UOM,PODisNum from tblPurchaseOrder where PONumber =" + SO_No + " and VendorID='" + cmbVendorSelect.Text.ToString().Trim() + "' and IsFullGRN='false' order by PODisNum";//  group by ItemId,Description,GL_Account,UnitPrice,UOM";               
                String S = "Select ItemId,Description,RemainQty,GRNQty,UOM,UnitPrice,GL_Account,Amount,PODisNum from tblPurchaseOrder where PONumber =" + SO_No + " and VendorID='" + cmbVendorSelect.Text.ToString().Trim() + "' and IsFullGRN='false' order by PODisNum";//  group by ItemId,Description,GL_Account,UnitPrice,UOM";               
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

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select CustomerSoNo from tblPurchaseOrder where PONumber in (" + SO_No + ")";
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
            catch(Exception ex)
            {
                throw ex;
            }
            return CusPO;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < dgvGRNTransaction.Rows.Count; i++)
                {
                    if (dgvGRNTransaction.Rows[i].Cells[0].Value != null) //change cell value by 1                   
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
        //Createaopening balance double entry*********************

        private void CreateXmlToExportGRNAdjust(SqlTransaction tr, SqlConnection con)
        {
            try
            {            
                string ClosingStockAcc = "";
                SqlCommand myCommand4 = new SqlCommand("Select GRNCrAc from tblDefualtSetting", con, tr);                
                SqlDataAdapter da = new SqlDataAdapter(myCommand4);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    ClosingStockAcc = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                }


                DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                string reference = "GRN" + "-" + DTP.Day + DTP.Month + DTP.Year;
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
                    double ReceivedQty = Convert.ToDouble(dgvGRNTransaction[3, k].Value);
                    if (ReceivedQty != 0)
                    {
                        Writer.WriteStartElement("PAW_Item");
                        Writer.WriteAttributeString("xsi:type", "paw:item");


                        //crate a ID element (tag)=====================(1)
                        Writer.WriteStartElement("ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(dgvGRNTransaction[0, k].Value.ToString().Trim());//dgvItem[0, c].Value
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
                        Writer.WriteString(ClosingStockAcc);
                        Writer.WriteEndElement();


                        //Unit Cost========================================(8)
                        //Writer.WriteStartElement("UnitCost");
                        //Writer.WriteString(dgvGRNTransaction[5, k].Value.ToString().Trim());
                        //Writer.WriteEndElement();

                        //Quantity========================================(9)

                        Writer.WriteStartElement("Quantity");
                        // Writer.WriteString(dgvItem[4, k].Value.ToString().Trim());
                        Writer.WriteString(ReceivedQty.ToString().Trim());
                        //Adjust_qty
                        Writer.WriteEndElement();
                        //Amount===========================================(10)
                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + dgvGRNTransaction[7, k].Value.ToString().Trim());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("ReasonToAdjust");
                        Writer.WriteString("Good Received");
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //*********************************************************
        public int ChechDQty = 0;
        public bool CheckAcivated = false;
        public bool CheckMinusAmount = false;

        public int VendorValidation(string VendorID)
        {
            try
            {
                StrSql = "SELECT VendorID FROM tblVendorMaster where VendorID='" + VendorID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
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
                return 0;
                throw ex;
            }
        }

        public int AccountValidation(string AccountID)
        {
            try
            {
                StrSql = "SELECT AcountID FROM tblChartofAcounts where AcountID='" + AccountID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
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
                return 0;
                throw ex;
            }
        }
        public int WarehouseValidation(string WID)
        {
            try
            {
                StrSql = "SELECT WhseId FROM tblWhseMaster where WhseId='" + WID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
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
                return 0;
                throw ex;
            }
        }
        private void HeaderValidation()
        {
            if (cmbWarehouse.Value != null)
            {
                if (WarehouseValidation(cmbWarehouse.Value.ToString().Trim()) == 0)
                {
                    MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }
            else
            {

                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //====================================================================

            if (cmbVendorSelect.Value != null)
            {
                if (VendorValidation(cmbVendorSelect.Value.ToString().Trim()) == 0)
                {
                    MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }
            else
            {

                MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //============================
            if (cmbAR.Value != null)
            {
                if (AccountValidation(cmbAR.Value.ToString().Trim()) == 0)
                {
                    MessageBox.Show("Incorrect Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }
            else
            {

                MessageBox.Show("Incorrect Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        public Boolean IsGridValidation()
        {
            try
            {
                double dilqty = 0;
                if (dgvGRNTransaction.Rows.Count == 0)
                {
                    return false;
                }

                foreach (DataGridViewRow ugR in dgvGRNTransaction.Rows)
                {
                    if (ugR.Cells["RQty"].Value != null)
                    {
                        dilqty = dilqty + double.Parse(ugR.Cells["RQty"].Value.ToString());

                        if (double.Parse(ugR.Cells["RQty"].Value.ToString()) < 0)
                        {
                            MessageBox.Show("Enter Valid Quantity of Item ID '" + ugR.Cells["ItemID"].Value.ToString() + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
                if (dilqty <= 0)
                {
                    MessageBox.Show("Invalid Received Quantity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public bool IsSerialNoCorrect()
        {
            try
            {
                int _Count = 0;
                string expression;
                foreach (DataGridViewRow dgvr  in dgvGRNTransaction.Rows)
                {
                    if (dgvr.Cells["ItemID"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["ItemID"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["RQty"].Value.ToString()) > 0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemID"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["ItemID"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["RQty"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["RQty"].Value.ToString()))
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

        private void btnSave_Click(object sender, EventArgs e)
        {
           
                dgvGRNTransaction.EndEdit();
            DialogResult reply = MessageBox.Show("Are you sure, you want to Process this record ? ", "Information", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

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

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }
            if (!objControlers.HeaderValidation_Warehouse(cmbWarehouse.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }

            dgvGRNTransaction.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //dgvGRNTransaction.CurrentCell = dgvGRNTransaction.CurrentRow.Cells[7];


            if (IsGridValidation() == false)
            {
                return;
            }

            if (!user.IsGRNNoAutoGen)
            {
                if (txtGRn_NO.Text.Trim() == string.Empty)
                {
                    CheckAcivated = false;
                    MessageBox.Show("Enter GRN No...!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbAR.Value == null)
            {
                MessageBox.Show("Invalid AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!IsSerialNoCorrect())
                return;

            int rowCount = GetFilledRows();
            string TranType = "Grn-Tran";
            int DocType = 2;
            bool QtyIN = true;

            bool CheckZeroReceived = false;
            try
            {
                for (int b = 0; b < rowCount; b++)
                {
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[3].Value) > 0)
                    {
                        CheckZeroReceived = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            bool IsItemSerial = false;
            //check wether this item is serialized or not=======================
            try
            {
                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    ItemClass = dgvGRNTransaction.Rows[a].Cells[0].Value.ToString().Trim();
                    if (ItemClass != "" && dgvGRNTransaction.Rows[a].Cells[8].Value.ToString().Trim() != "")
                    {
                        String S = "Select * from tblItemMaster where ItemID  = '" + dgvGRNTransaction.Rows[a].Cells[0].Value.ToString().Trim() + "'";
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

                        }
                        else
                        {
                            if (ItemClass == "")
                            {
                                MessageBox.Show("The Item You are going to ship is not in the Item Master File, Please Import Item Master File", "Goods Received Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            CheckAcivated = false;
            bool ISExeedLimit = false;
            string GRNNo = "";
            bool IsMinusAllow = true;
            bool IsminusValidate = false;
            try
            {
                String S31 = "select GRN_NO from tblGRNTran";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                SqlDataAdapter da31 = new SqlDataAdapter(S31, ConnectionString);
                DataTable dt31 = new DataTable();
                da31.Fill(dt31);
                if (dt31.Rows.Count > 10000000)
                {
                    ISExeedLimit = true;
                }
            }
            catch { }

            try
            {
                //check the dispatch qty is wether a number or not
                for (int a = 0; a < rowCount; a++)
                {
                    ChechDQty = 0;
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value);

                    Convert.ToDouble(txtTax1Amount.Text);
                    Convert.ToDouble(txtTax2.Text);
                    Convert.ToDouble(txtDisRate.Text);
                    Convert.ToDouble(txtDiscountAmount.Text);
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value) > Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value))
                    {
                        String S3 = "select OverGRN from tblDefualtSetting";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt3.Rows[0].ItemArray[0]) == true)
                            {
                                IsMinusAllow = true;
                            }
                            else
                            {
                                IsMinusAllow = false;
                                IsminusValidate = true;
                            }
                        }

                    }
                    else
                    {
                        if (IsminusValidate == true)
                        {
                            IsMinusAllow = false;
                        }
                        else
                        {
                            IsMinusAllow = true;
                        }
                    }

                    //if (Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value) > 0)
                    //{
                    //    IsRecevedNull = true;
                    //}
                }

            }
            catch (Exception ex)
            {
                ChechDQty = 1;//if this flag is 1 the violate the number format
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            if (ChechDQty == 0)
            {
                if (txtTotalAmount.Text == "" || txtTotalAmount.Text == "0.00" || txtTotalAmount.Text == "0" ||
                    IsMinusAllow == false || ISExeedLimit == true || rowCount == 0 || CheckZeroReceived == false)
                {
                    if (txtTotalAmount.Text == "")
                    {
                        MessageBox.Show("Total Amount is Empty");
                    }

                    else if (IsMinusAllow == false)
                    {
                        if (!user.IsOverGRNQty)
                        {
                            MessageBox.Show("Received quantity can not be grater than the order quantity");
                            return;
                        }
                    }
                    else if (ISExeedLimit == true)
                    {
                        MessageBox.Show("Your Transaction Limit Over Please Register the Product");
                    }
                    else if (rowCount == 0)
                    {
                        MessageBox.Show("Please Select a Purchase Order");
                    }
                    else if (txtTotalAmount.Text == "" || txtTotalAmount.Text == "0.00" || txtTotalAmount.Text == "0")
                    {
                        MessageBox.Show("You have not entered Received Quantity ");
                    }
                    else if (CheckZeroReceived == false)
                    {
                        MessageBox.Show("You have not entered Received Quantity ");
                    }
                }
                else
                {
                    DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string GRNDate = DTP.ToString(Dformat);
                    SqlConnection myConnection = new SqlConnection(ConnectionString);
                    myConnection.Open();
                    SqlTransaction myTrans = myConnection.BeginTransaction();
                    try
                    {
                        StrSql = "DELETE FROM [tblGRNTran] WHERE GRN_NO='" + txtGRn_NO.Text.ToString().Trim() + "'";

                        SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                        command1.CommandType = CommandType.Text;
                        command1.ExecuteNonQuery();
                        //Get the current running number from the system
                        //if (user.IsGRNNoAutoGen)
                        //{
                        //    SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET GRNNo = GRNNo + 1 select GRNNo, GRNPrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                        //    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        //    DataTable dt41 = new DataTable();
                        //    da41.Fill(dt41);

                        //    if (dt41.Rows.Count > 0)
                        //    {
                        //        GRNNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                        //        GRNNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + GRNNo;
                        //    }
                        //    txtGRn_NO.Text = GRNNo;
                        //}
                        //else
                        //{
                        //    SqlCommand myCommand = new SqlCommand("select * from tblGRNTran where GRN_NO='" + txtGRn_NO.Text.Trim() + "'", myConnection, myTrans);
                        //    SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                        //    DataTable dt41 = new DataTable();
                        //    da41.Fill(dt41);

                        //    if (dt41.Rows.Count > 0)
                        //    {
                        //        MessageBox.Show("GRN No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //        myTrans.Rollback();
                        //        myConnection.Close();//
                        //        return;
                        //    }
                        //}

                        //============================================
                        for (int i = 0; i < rowCount; i++)
                        {
                            bool IsfullShipment = false;
                            bool Isinvoice = false;
                            // string TranType = "Tran-GRN";

                            bool Duplicate = true;
                            string NoOfDis = Convert.ToString(dgvGRNTransaction.Rows.Count - 1);
                            //==============UOM Details================


                            SqlCommand cmd34 = new SqlCommand("select Ratio from tblUOMMaster where UMID='" + dgvGRNTransaction.Rows[i].Cells["UOM"].Value.ToString().Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                            DataTable dt34 = new DataTable();
                            da34.Fill(dt34);
                            if (dt34.Rows.Count > 0)
                            {
                                dblUOMRatio = Convert.ToDouble(dt34.Rows[0]["Ratio"].ToString());
                            }
                            else
                            {
                                dblUOMRatio = 1;
                            }
                            dblQuantity = Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["RQty"].Value.ToString()) + Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["FreeQty"].Value.ToString());
                            dblConverteQty = dblUOMRatio * dblQuantity;


                            if (SelectSONums1 == "" || SelectSONums1 == null)
                            {
                                string item = clistbxSalesOrder.CheckedItems[0].ToString();
                                SelectSONums1 = item;
                            }

                            SqlCommand myCommand2 = new SqlCommand("insert into tblGRNTran(GRN_NO,VendorID,PONos,GRNDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,OrderQty,ReceiveQty,GlAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,Dupliate,ISGRNFinished,CustomerSO,WareHouseID,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1,PODistributionNO,ConvertedQty,IsActive,FreeQty) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[1, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[3, i].Value) + "','" + dgvGRNTransaction[8, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[7, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToDouble(dgvGRNTransaction[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "','" + Convert.ToDouble(dgvGRNTransaction[9, i].Value) + "','" + dblConverteQty + "','"+false+ "','" + Convert.ToDouble(dgvGRNTransaction[10, i].Value) + "')", myConnection, myTrans);
                            myCommand2.ExecuteNonQuery();

                            double SellingPrice = 0.00;

                            SqlCommand cmd3 = new SqlCommand("update tblItemMaster set UnitCost='" + Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[5].Value) + "', PriceLevel1='" + Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[5].Value) +"' where  ItemID='" + dgvGRNTransaction[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            SqlCommand myCommand6 = new SqlCommand("Select * from  tblItemWhse where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);

                            SqlDataAdapter da1 = new SqlDataAdapter(myCommand6);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);

                            if (dt1.Rows.Count > 0)
                            {
                                SqlCommand myCommand3 = new SqlCommand("update tblItemWhse set QTY = QTY + '" + dblConverteQty + "' where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);
                                myCommand3.ExecuteNonQuery();
                            }
                            else
                            {

                                SqlCommand myCommand4 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[1, i].Value) + "','" + dblConverteQty + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + GRNDate + "')", myConnection, myTrans);
                                myCommand4.ExecuteNonQuery();
                            }

                            if (Convert.ToDouble(dgvGRNTransaction[3, i].Value) != 0)
                            {
                                SqlCommand cmd11 = new SqlCommand(
                                    "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbWarehouse.Value.ToString().Trim() + "' AND ItemId='" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "') " +
                                    " Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY, '" + DocType + "','" + txtGRn_NO.Text.ToString().Trim() + "','" + GRNDate + "','" + TranType + "','" + QtyIN + "','" + dgvGRNTransaction[0, i].Value + "','" + dblConverteQty + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + dblConverteQty * Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + SellingPrice + "')", myConnection, myTrans);
                                cmd11.ExecuteNonQuery();
                            }

                            string Stataus = "Available";
                            bool IsGRNProcess = true;
                            ArraySONO.Add(abc);

                            //==========================================setfull dispath
                            if (Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[2].Value.ToString()) == Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[3].Value.ToString()))//changed cell value
                            {
                                IsfullShipment = true;
                                SetFullDispatch(ArraySONO, dgvGRNTransaction.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[3].Value.ToString()), IsfullShipment, int.Parse(dgvGRNTransaction.Rows[i].Cells["PODisNUmber"].Value.ToString()));
                            }
                            else
                            {
                                UpdateSoTemptbl(ArraySONO, dgvGRNTransaction.Rows[i].Cells["ItemId"].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["RQty"].Value.ToString()), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["dataGridViewTextBoxColumn6"].Value.ToString()), int.Parse(dgvGRNTransaction.Rows[i].Cells["PODisNUmber"].Value.ToString()));
                            }
                            //===============================================
                        }

                        foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        {
                            SqlCommand myCommandSe1 = new SqlCommand("insert into tblSerialItemTransaction  " +
                                " (TranType,Status,ItemID,WareHouseID,SerialNo)" +
                                " values('Grn-Tran','Available','" +
                                dr["ItemCode"].ToString() + "' ,'" + cmbWarehouse.Text.ToString().Trim() + "' ,'" +
                                dr["SerialNo"].ToString() + "')", myConnection, myTrans);
                            myCommandSe1.ExecuteNonQuery();
                        }

                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Grn-Tran", cmbWarehouse.Text.ToString(), txtGRn_NO.Text.ToString().Trim(), dtpDispatchDate.Value, false, "");

                        myTrans.Commit();

                        // Disable();
                        MessageBox.Show("Good Received Note Successfully Procesed", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnPrint_Click(sender, e);
                        Enable();
                        ClearData();
                        btnSave.Enabled = false;
                        btnNew.Enabled = true;
                        btnNew_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                }


            }
            else
            {
                MessageBox.Show(" You must enter numeric value");
            }


        }

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
            catch(Exception ex)
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
                String S2 = "Select ItemID,Qty,PONO,PODisNo from tblGRNMPO where PONO='" + PONO + "' and GRNNO='" + GRNNO + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(POQty);
            }
            catch(Exception ex)
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
            catch(Exception ex)
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
            catch (Exception ex) { throw ex; }

            DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
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

        public Double GetOrdQTY1(string poid, string ItemID, double UPrice, int DisNo)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                double OrdQty =0;
                string ConnString = ConnectionString;
                string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
                //  string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrdQty =Convert.ToDouble( reader[0]);
                }
                reader.Close();
                Conn.Close();
                return Convert.ToDouble( OrdQty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==============================================================

        //==============================================================
        public Double GetPrevOrdQTY(string poid, string ItemID, double UPrice,int DisNo)
        {
            try
            {
                string POid = poid.Trim();
                setConnectionString();
                Double OrdQty = 0;
                string ConnString = ConnectionString;
                //string sql = "select RemainQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
                string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "'  and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";// and UnitPrice=" + UPrice + "";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = Conn;
                Conn.Open();
               // SqlDataReader reader = cmd.ExecuteReader();
                SqlDataAdapter da2 = new SqlDataAdapter(sql, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                if (dt2.Rows.Count > 0)
                {   
                   OrdQty = Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString());   
                }
                           
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
                string sql = "select ItemId from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
                string sql = "select GRNQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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


        public void SetDispatchFOR_Partial(string SOID, string ItemID, bool FullShip, double DispatchQty,int DisNo)
        {
            //============================
            // bool fullshipment = false;
            bool fullshipment = FullShip;
            double OrginalQty = 0;
            double UpdateDispatchQty = 0;
            double RemainQty = 0;
            bool updatefull = false;
            try
            {
                //String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "';";
                String S = "Select Quantity,GRNQty,RemainQty from tblPurchaseOrder where PONumber = '" + SOID + "' and ItemId='" + ItemID + "' and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
                cmd.CommandText = "UPDATE tblPurchaseOrder SET IsFullGRN = '" + fullshipment + "',GRNQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE PONumber = '" + SOID + "' and ItemId='" + ItemID + "' and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
                cmd.ExecuteNonQuery();

                if (DispatchQty == 0)
                { }
                else
                {
                    // cmd.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "'";
                    cmd.CommandText = "update tblGRNMPO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updatefull + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";

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

        public void UpdateSOTemp(string PO_NO_update, string ItemID_update, double ReceivableQty_update, double DispatchQ, double UnitPrice_update,int DisNo)
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
            try
            {
                //String S = "Select DispatchQty,Quantity from tblSalesOrderTemp where SalesOrderNo = '" + PO_NO_update + "' and ItemID='" + ItemID_update + "';";
                String S = "Select GRNQty,Quantity from tblPurchaseOrder where PONumber = '" + PO_NO_update + "' and ItemId='" + ItemID_update + "'  and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
                Conn.Open();
                SqlCommand cmd1 = Conn.CreateCommand();
                // cmd1.CommandText = "UPDATE tblSalesOrderTemp SET RemainQty=" + RemainQty + ",DispatchQty='" + updateQty + "',IsfullDispatch='" + fullDispath + "' where SalesOrderNo='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemID='" + ItemID_update + "'";
               // cmd1.CommandText = "UPDATE tblPurchaseOrder SET RemainQty=" + RemainQty + ",GRNQty  ='" + updateQty + "',IsFullGRN='" + fullDispath + "' where PONumber='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemId='" + ItemID_update + "'  and PODisNum='" + DisNo + "'";

                cmd1.CommandText = "UPDATE tblPurchaseOrder SET RemainQty=" + RemainQty + ",GRNQty  ='" + updateQty + "',IsFullGRN='" + fullDispath + "' where PONumber='" + PO_NO_update + "'and ItemId='" + ItemID_update + "'  and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";         
                cmd1.ExecuteNonQuery();
                if (DispatchQ == 0)
                { }
                else
                {
                    cmd1.CommandText = "update tblGRNMPO set Qty ='" + DispatchQ + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + Ischeck + "' where PONO='" + PO_NO_update + "' and ItemID='" + ItemID_update + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
        public void UpdateSoTemptbl(ArrayList SOIDs, string ItemID, double dispatchQty, double UPrice,int DisNo)
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
            try
            {
                for (int i = 0; i < SOIDs.Count; i++)
                {

                   
                    string SOID = SOIDs[i].ToString();
                    string Item_ID = ItemID;
                    double orderdqty = GetOrdQTY1(SOID, ItemID, UPrice,DisNo);//order quantity mean Actual Remain Qty
                    if (orderdqty != 0)
                    {
                        //========================================================
                        if (orderdqty <= dispatchQty)
                        {
                            Extra_qty1 = ParialDispatchQty1 - orderdqty;
                            Extra_qty = dispatchQty - orderdqty;
                            dispatchQty = Extra_qty;
                            if (Extra_qty == 0)
                            {
                                if (ABC == false)
                                {
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty1,DisNo);
                                }
                            }
                            else if (Extra_qty > 0)
                            {
                                ParialDispatchQty = ParialDispatchQty1 - Extra_qty1;

                                if (i == 0)
                                {
                                    prevqtyTemp = Extra_qty1;
                                    //2
                                    SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty,DisNo);
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
                                    PrevOrderdqty = GetPrevOrdQTY(SOID2, ItemID, UPrice,DisNo);
                                    if (PrevOrderdqty == 0)
                                    {
                                        DispatchableQty = dispatchQty;
                                        FullDispatch = false;
                                        //3
                                        SetDispatchFOR_Partial(SOIDs[i].ToString(), ItemID, FullDispatch, ParialDispatchQty,DisNo);
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

                            if (i == 0)
                            {
                                ABC = true;
                                DispathQty = orderdqty - dispatchQty;
                                {
                                    //4
                                    UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice,DisNo);
                                    PQR = true;
                                }

                            }
                            else
                            {
                                string SOID1 = SOIDs[i - 1].ToString();
                                // string SOID2 = SOIDs[i].ToString();

                                // checkItem = GetIem(SOID2, ItemID);
                                PrevOrderdqty = GetPrevOrdQTY(SOID1, ItemID, UPrice,DisNo);
                                // if (checkItem == true)
                                //{
                                if (ABC == false)
                                {
                                    if (PrevOrderdqty == 0)
                                    {
                                        DispathQty = orderdqty - prevqtyTemp;
                                        {
                                            //5
                                            UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice,DisNo);
                                            ABC = true;
                                            PQR = true;
                                        }
                                    }
                                }
                            }
                        }
                    }//if(orderQty==0)
                    //=====================================================                
                }
                //========================================
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SetFullDispatch(ArrayList SOID, string ItemID, double DispatchQty, bool FullDispatch,int DisNo)
        {
            try
            {
                double OrginalQty = 0;
                double UpdateDispatchQty = 0;
                double RemainQty = 0;
                bool IsCheckFull = false;
                bool updateCheck = true;
                // bool   FullDispatch = true;
                //DataSet ds = new DataSet();

                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                SqlCommand myCommand = new SqlCommand();
                myConnection.Open();
                myCommand.Connection = myConnection;
                for (int i = 0; i < SOID.Count; i++)
                {
                    //================================================
                   
                    //String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    String S = "Select Quantity,GRNQty,RemainQty from tblPurchaseOrder where PONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "' and PODisNum='" + DisNo + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
                    String S1 = "Select ISFull from tblGRNMPO where PONO = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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

                    //=======================================================
                    //kjkjkjkfjfjf
                    //=====================================
                    //myCommand.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + FullDispatch + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    myCommand.CommandText = "UPDATE tblPurchaseOrder SET IsFullGRN = '" + FullDispatch + "',GRNQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE PONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "' and PODisNum='" + DisNo + "'and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
                    myCommand.ExecuteNonQuery();

                    // myCommand.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                    if (IsCheckFull == true)
                    {
                        if (DispatchQty == 0)
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + OrginalQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
                            myCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            myCommand.CommandText = "update tblGRNMPO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "' and VendorID='" + cmbVendorSelect.Value.ToString().Trim() + "'";
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
                    //  myCommand.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + SOID[i].ToString() + "','" + ItemID + "','" + DispatchQty + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                    // myCommand.ExecuteNonQuery();

                    //===================================get no of po line per purchaeorder
                    //try
                    //{
                    //    String S2 = "Select PONumber from tblPurchaseOrder where PONumber = '" + SOID[i].ToString() + "'";
                    //    SqlCommand cmd1 = new SqlCommand(S2);
                    //    SqlDataAdapter da1 = new SqlDataAdapter(S2, ConnectionString);
                    //    DataTable dt2 = new DataTable();
                    //    da1.Fill(dt2);
                    //    //if (dt1.Rows.Count > 0)
                    //    //{

                    //}
                    //}
                    //catch { }

                    //========================================
                    //if (i <= dt2.Rows.Count)
                    //{
                    //    myCommand.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + SOID[i].ToString() + "','" + ItemID + "','" + DispatchQty + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                    //    myCommand.ExecuteNonQuery();
                    ////}
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

        public string NextDeliveryNoteNo()//get the next dispatch link number
        {
            string NextDNoteNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select GRN_NO from tblGRNTran ORDER BY GRN_NO";
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand(sql);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnString);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // NextDNoteNo =ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    //   int p = ds.Tables[0].Rows.Count - 1;
                    NextDNoteNo = getNextID(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1].ItemArray[0].ToString());

                    //  txtReceiptNo.Text = NewID;
                }
                else
                {
                    //String S2 = "Select DeliveryNoteNo from tblDefaultSetting";
                    //SqlCommand cmd2 = new SqlCommand(S2);
                    //SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    //DataTable dt1 = new DataTable();
                    //da2.Fill(dt1);

                    //if (dt1.Rows.Count > 0)
                    //{
                    //  //  NextDNoteNo = dt1.Rows[0].ItemArray[0].ToString().Trim();
                    NextDNoteNo = "GRN-100000";
                    //}
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return NextDNoteNo;
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
        private void ClearData()
        {
            try
            {
                ClassDriiDown.Delivery_note_No = "";
                txtlocName.Text = "";
                txtLocAdd1.Text = "";
                txtLocAdd2.Text = "";
                txtCustomerSO.Text = "";

                cmbVendorSelect.Text = "";
                cmbWarehouse.Text = user.StrDefaultWH;

                GetAPAccount();

                cmbtaxSys1.Text = "";
                cmbtaxSys2.Text = "";

                txtTax1Amount.Text = "0";
                txtTax2.Text = "0";

                txtDiscountAmount.Text = "0";
                txtDisRate.Text = "0";
                txtNetTotal.Text = "";

                txtcusName.Text = "";
                txtcusCity.Text = "";
                txtCusAdd1.Text = "";
                txtCusAdd2.Text = "";
                txtGRn_NO.Text = "";
                txtTotalAmount.Text = "0.00";
                dgvGRNTransaction.Rows.Clear();
                clistbxSalesOrder.Items.Clear();
                GetCurrentUserDate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            //cmbARAccount.Items.Clear();
        }

     
        private void btnNew_Click(object sender, EventArgs e)
        {
            MultiplePOSelect = false;
            cmbVendorSelect.Focus();
            btnfullShip.Enabled = true;
            savecomplete = false;
           IsUpdate = false;
            btnSave.Enabled = true;
            btnEditer.Enabled = false;
            btnProcess.Enabled = false;
            btnPrint.Enabled = false;
            txtCustomerSO.ReadOnly = false;
            IsUpdate = false;
            IsActive = true;
            cmbVendorSelect.ReadOnly = false;
            cmbWarehouse.ReadOnly = false;
            try
            {
                CheckAcivated = false;
                GetARAccount();//Infragistics
                GetAPAccount();
                GetWareHouseDataSet();//Infragistics
                GetVendorDataset();//Infragistics


                flag = false;
                //flglist = 0;
                // cmbLocation.Items.Clear();
                chekSOK = false;
                Enable();
                ClearData();
                cmbVendorSelect.Focus();

                if (user.IsGRNNoAutoGen) txtGRn_NO.ReadOnly = true;
                else txtGRn_NO.ReadOnly = false;

                cmbAR.Text = user.ApAccount;

                clsSerializeItem.DtsSerialNoList.Rows.Clear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            btnProcess.Enabled = false;
            txtlocName.Text = "";
        }

        public void Enable()
        {
            clistbxSalesOrder.Enabled = true;
            dtpDispatchDate.Enabled = true;
            dgvGRNTransaction.Enabled = true;
            dgvGRNTransaction.ReadOnly = false;
            btnClose.Enabled = true;
            btnProcess.Enabled = true;
            btnList.Enabled = true;
            btnSNO.Enabled = false;
            btnGetPurchaseOrder.Enabled = true;
        }

        public void Disable()
        {
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
            txtcusName.Enabled = false;
            txtcusCity.Enabled = false;
            txtCusAdd1.Enabled = false;
            txtCusAdd2.Enabled = false;
            txtGRn_NO.Enabled = false;
            txtTotalAmount.Enabled = false;
            clistbxSalesOrder.Enabled = false;
           
            
            dtpDispatchDate.Enabled = false;
            dgvGRNTransaction.Enabled = false;
        }
        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptGoodsRecevedNote.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("rptGoodsRecevedNote.rpt");
                }
                else
                {
                    MessageBox.Show("rptGoodsRecevedNote.rpt not Exists.");
                    this.Close();
                    return;
                }

                crp.Load(Myfullpath);
                crp.SetDataSource(objGRN);
                //crp.PrintToPrinter(1, true, 1, 1);
                crp.PrintToPrinter(1, true, 0, 0);
                // crp.PrintOptions.PrinterName = "Generic / Text Only On Nazeer";

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                if (txtGRn_NO.Text == "")
                {
                    MessageBox.Show("Please Select a GRN Number");
                    //btnPrint.Focus();
                }
                else
                {
                    objGRN.Clear();

                    String S3V = "Select * from tblVendorMaster";// where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd3V = new SqlCommand(S3V);
                    SqlConnection con3V = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3V = new SqlDataAdapter(S3V, con3V);
                    da3V.Fill(objGRN, "DTVendor");

                    // objGRN.DtGRNTrans.Clear();
                    String S3 = "Select * from tblGRNTran where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(objGRN, "DtGRNTrans");

                    // objGRN.dt_CompanyDetails.Clear();
                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(objGRN, "dt_CompanyDetails");

                   // DirectPrint();
                    frmDeiveryNotePrint frm = new frmDeiveryNotePrint(this);
                    frm.Show();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                CheckAcivated = true;
                if (frmMain.ObjGRNList == null || frmMain.ObjGRNList.IsDisposed)
                {
                    frmMain.ObjGRNList = new frmDeliveryNoteList(1);
                }
                frmMain.ObjGRN.TopMost = false;
                frmMain.ObjGRNList.ShowDialog();                
                frmMain.ObjGRNList.TopMost = true;
              

                //fillDetails();
                //txtGRn_NO.Text = ClassDriiDown.Delivery_note_No;
                //frmGRN_Activated(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }           
        }
        ClassDriiDown ab = new ClassDriiDown();

        

        //====================Get Tax Rate Information====================
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
            catch(Exception ex)
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
                    MessageBox.Show("Set the Invoice Values First");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                    txtTax2.Text = TaxAmount.ToString("N2");
                    double Did_Amt = Convert.ToDouble(txtDiscountAmount.Text);
                    double NetTot = (GrossTot - Did_Amt) + Tax1Amt;
                    txtNetTotal.Text = (NetTot + TaxAmount).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                cmbtaxSys1.Enabled = true;
                cmbtaxSys2.Enabled = true;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }         
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
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        bool chekSOK = false;
        private void frmGRN_Activated(object sender, EventArgs e)
        {
            try
            {
                if (CheckAcivated == true)
                {
                    fillDetails();
                    btnfullShip.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
            CheckAcivated = false;
        }
        string abc;
        string SerchText;
        private void fillDetails()
        {
           
            try
            {
                
                if (ClassDriiDown.Delivery_note_No != "")
                {
                    SerchText = ClassDriiDown.Delivery_note_No;
                }
                else if(savecomplete ==false)
                {
                    return;
                }
                if (SerchText == String.Empty)
                {
                    CheckAcivated = false;
                    return;
                }
                txtCustomerSO.ReadOnly = true;
                dgvGRNTransaction.Rows.Clear();
                chekSOK = true;
                clistbxSalesOrder.Items.Clear();
                cmbVendorSelect.ReadOnly = true;
                cmbWarehouse.ReadOnly = true;

                string ConnString = ConnectionString;
                    String S1 = "Select * from tblGRNTran where GRN_NO='" + SerchText + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);
                   
                    if (dt.Rows.Count > 0)
                    {                   
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {                   
                            txtGRn_NO.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                            cmbVendorSelect.Text = dt.Rows[i].ItemArray[1].ToString().Trim();
                             abc = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dtpDispatchDate.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                            cmbAR.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                            cmbtaxSys1.Text = dt.Rows[i].ItemArray[27].ToString().Trim();
                            cmbtaxSys2.Text = dt.Rows[i].ItemArray[28].ToString().Trim();
                            txtTax1Amount.Text = dt.Rows[i].ItemArray[29].ToString().Trim();
                            txtTax2.Text = dt.Rows[i].ItemArray[30].ToString().Trim();
                            txtDiscountAmount.Text = dt.Rows[i].ItemArray[26].ToString().Trim();
                            txtDisRate.Text = dt.Rows[i].ItemArray[25].ToString().Trim();
                            txtNetTotal.Text = dt.Rows[i].ItemArray[24].ToString().Trim();
                            txtCustomerSO.Text = dt.Rows[i].ItemArray[21].ToString().Trim();
                            cmbWarehouse.Text = dt.Rows[i].ItemArray[22].ToString().Trim();
                            dgvGRNTransaction.Rows.Add();
                            dgvGRNTransaction.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();

                            if (DecimalpointQuantity == 0)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString();//ORDER QTY
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString();//Received Qty
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString();//Free Qty

                        }
                        else if (DecimalpointQuantity == 1)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString("N1");
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString("N1");
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString("N1");
                        }
                            else if (DecimalpointQuantity == 2)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString("N2");
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString("N2");
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString("N2");
                            }
                            else if (DecimalpointQuantity == 3)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString("N3");
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString("N3");
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString("N3");
                            }
                            else if (DecimalpointQuantity == 4)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString("N4");
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString("N4");
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString("N4");
                            }
                            else if (DecimalpointQuantity == 5)
                            {
                                dgvGRNTransaction.Rows[i].Cells[2].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[9])).ToString("N5");
                                dgvGRNTransaction.Rows[i].Cells[3].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[10])).ToString("N5");
                                dgvGRNTransaction.Rows[i].Cells[10].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[39])).ToString("N5");
                            }
                            dgvGRNTransaction.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                            dgvGRNTransaction.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString().Trim();
                            if (Decimalpoint == 0)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString();//unit price
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString();//discount
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString();//amount
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString();//Grand Total
                            }
                            else if (Decimalpoint == 1)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString("N1");
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString("N1");
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString("N1");
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString("N1");//Grand Total
                            }
                            else if (Decimalpoint == 2)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString("N2");
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString("N2");
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString("N2");
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString("N2");//Grand Total
                            }
                            else if (Decimalpoint == 3)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString("N3");
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString("N3");
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString("N3");
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString("N3");//Grand Total
                            }
                            else if (Decimalpoint == 4)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString("N4");
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString("N4");
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString("N4");
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString("N4");//Grand Total
                            }
                            else if (Decimalpoint == 5)
                            {
                                dgvGRNTransaction.Rows[i].Cells[5].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[12])).ToString("N5");
                                dgvGRNTransaction.Rows[i].Cells[6].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[23])).ToString("N5");
                                dgvGRNTransaction.Rows[i].Cells[7].Value = (Convert.ToDouble(dt.Rows[i].ItemArray[13])).ToString("N5");
                                txtTotalAmount.Text = (Convert.ToDouble(dt.Rows[i].ItemArray[15])).ToString("N5");//Grand Total
                            }
                            dgvGRNTransaction.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[11].ToString().Trim();
                            dgvGRNTransaction.Rows[i].Cells[9].Value = dt.Rows[i].ItemArray[36].ToString().Trim();

                        btnSave.Enabled = false;

                        IsActive = Convert.ToBoolean(dt.Rows[i].ItemArray[38].ToString().Trim());
                        if(IsActive == false)
                        {
                            btnEditer.Enabled = false;
                            btnProcess.Enabled = false;
                        }
                        else
                        {
                            btnEditer.Enabled = true;
                           
                        }
                    }
                        clistbxSalesOrder.Items.Add(dt.Rows[0].ItemArray[2].ToString().Trim(), CheckState.Checked);

                  
                }


            
                    StrSql = " SELECT VendorName,VContact,VAddress1,VAddress2 FROM tblVendorMaster where VendorID = '"+cmbVendorSelect.Text.Trim()+"'";

                    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt2 = new DataTable();
                dAdapt.Fill(dt2);
                if(dt2.Rows.Count>0)
                {
                    txtcusName.Text = dt2.Rows[0].ItemArray[0].ToString();
                    txtCusAdd1.Text = dt2.Rows[0].ItemArray[1].ToString();
                    txtCusAdd2.Text = dt2.Rows[0].ItemArray[2].ToString();
                    txtcusCity.Text = dt2.Rows[0].ItemArray[3].ToString();
                }

                 
                    //clistbxSalesOrder.Enabled = false;
                    dgvGRNTransaction.ReadOnly = true;
                    dgvGRNTransaction.Enabled = true;
                    btnSNO.Enabled = true;
                    btnSave.Enabled = false;
                    btnClose.Enabled = true;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //  int m = 0;
       

        //=============================================


        //private void ComboBox1_KeyUp(Object sender,System.Windows.Forms.KeyEventArgs e)
        //{
        //AutoCompleteCombo(ComboBox1,e);
        //}

        //public static void AutoCompleteCombo(mu cbo, KeyEventArgs e)
        //{

        //    String sTypedText;

        //    Int32 iFoundIndex;

        //    Object oFoundItem;

        //    String sFoundText;

        //    String sAppendText;

        //    //'Allow select keys without Autocompleting

        //    switch (e.KeyCode)
        //    {

        //        case Keys.Back:

        //            break;

        //        case Keys.Left:

        //            break;

        //        case Keys.Right:

        //            break;

        //        case Keys.Tab:

        //            break;

        //        case Keys.Up:

        //            break;

        //        case Keys.Delete:

        //            break;

        //        case Keys.Down:

        //            break;

        //    }
        //}




        //===============================================================    
        

        private void dtpDispatchDate_ValueChanged(object sender, EventArgs e)
        {
            //CheckAcivated = false;
        }
        ClassDriiDown a = new ClassDriiDown();
        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Warehouse First");
                    return;
                }

                if (Convert.ToDouble(dgvGRNTransaction.CurrentRow.Cells["RQty"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + dgvGRNTransaction.CurrentRow.Cells["RQty"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            dgvGRNTransaction.CurrentRow.Cells["RQty"].Selected = true;
                        }
                    }
                }
                else
                {
                    frmSerialAddCommon ObjfrmSerialAddCommon = new frmSerialAddCommon("Grn-Tran", cmbWarehouse.Text.ToString().Trim(),
                        dgvGRNTransaction.CurrentRow.Cells["ItemID"].Value.ToString().Trim(),
                        Convert.ToDouble(dgvGRNTransaction.CurrentRow.Cells["RQty"].Value.ToString()), txtGRn_NO.Text,
                        IsFind, dgvGRNTransaction.CurrentRow.Cells[1].Value.ToString(),clsSerializeItem.DtsSerialNoList);
                    ObjfrmSerialAddCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
                
        }
        private void cmbWarehouse_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            
           
            clistbxSalesOrder.Items.Clear();
            dgvGRNTransaction.Rows.Clear();
            try
            {
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        if(cmbVendorSelect.Text.ToString()!="" && savecomplete ==false)
                        {

                            LoadPurchaseOrder(cmbVendorSelect.ActiveRow.Cells[0].Value.ToString());

                        }
                        txtlocName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();

                        //txtCusAdd1 = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                        //txtCusAdd2 = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        //txtcusCity = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();

                        String S = "Select * from tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataSet dt = new DataSet();
                        DataTable datatable = new DataTable();
                        da.Fill(dt);
                        da.Fill(datatable);
                        if (datatable.Rows.Count > 0)
                        {
                            cmbAR.Text = dt.Tables[0].Rows[0]["APAccount"].ToString();
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
             
                 MessageBox.Show("Incorrect vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                       // LoadPurchaseOrder(cmbVendorSelect.ActiveRow.Cells[0].Value.ToString());


                        txtcusName.Text  = cmbVendorSelect.ActiveRow.Cells[1].Value.ToString();
                        txtCusAdd1.Text  = cmbVendorSelect.ActiveRow.Cells[2].Value.ToString();
                        txtCusAdd2.Text  = cmbVendorSelect.ActiveRow.Cells[3].Value.ToString();
                        txtcusCity.Text  = cmbVendorSelect.ActiveRow.Cells[4].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnfullShip_Click(object sender, EventArgs e)
        {
            try
            {
                DatgridCellEndEditEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbVendorSelect_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbWarehouse, cmbVendorSelect, e);
            if (e.KeyCode == Keys.Down)
            {
                if (cmbVendorSelect.Enabled == true)
                {
                    cmbVendorSelect.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            //objControlers.FocusControl(txtUnitCost, ugCmbItem, e);

            if (e.KeyCode == Keys.Down)
            {
                if (cmbWarehouse.Enabled == true)
                {
                    cmbWarehouse.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree()))
                    return;

                Connector conn = new Connector();
                conn.ImportPurchaseOrderList();
                conn.Insert_PurchaseOrderList();

                MessageBox.Show("Successfully Imported Purchase Orders...!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWarehouse_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

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

        private void dgvGRNTransaction_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvGRNTransaction.CurrentRow != null)
                {
                    if (dgvGRNTransaction.CurrentRow.Cells[0].Value != null)
                    {
                        if (IsThisItemSerial(dgvGRNTransaction.CurrentRow.Cells[0].Value.ToString()))
                            btnSNO.Enabled = true;
                        else
                            btnSNO.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbVendorSelect_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void cmbAR_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
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
        private void toolStripButton1_Click(object sender, EventArgs e)
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
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[2].Value) > 0)
                    {
                        QtyIN = Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[2].Value);
                        Itemcode = dgvGRNTransaction.Rows[b].Cells[0].Value.ToString();
                        ItemDescription = dgvGRNTransaction.Rows[b].Cells[1].Value.ToString();
                        QtyCount = Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[2].Value);

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
                                UnitPrice = Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[5].Value.ToString()); //Convert.ToDouble(dt.Tables[0].Rows[0].ItemArray[1].ToString());
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
                Printype = 3;
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

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (txtCustomerSO.Text == "")
            {
                MessageBox.Show("Please Enter Suplier Invoice No", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (IsUpdate == false)
            {
                String StrSql = "select * from tblGRNTran where CustomerSO='" + txtCustomerSO.Text.Trim() + "'";
                SqlCommand cmd50 = new SqlCommand(StrSql);
                SqlDataAdapter da50 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt50 = new DataTable();
                da50.Fill(dt50);
                if (dt50.Rows.Count > 0)
                {
                    MessageBox.Show("Suplier Invoice No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;

                }
            }

            dgvGRNTransaction.EndEdit();
            DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;
            }

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

            if (!objControlers.HeaderValidation_Vendor(cmbVendorSelect.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }
            if (!objControlers.HeaderValidation_AccountID(cmbAR.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }
            if (!objControlers.HeaderValidation_Warehouse(cmbWarehouse.Text, sMsg))
            {
                CheckAcivated = false;
                return;
            }

            dgvGRNTransaction.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //dgvGRNTransaction.CurrentCell = dgvGRNTransaction.CurrentRow.Cells[7];


            if (IsGridValidation() == false)
            {
                return;
            }

            if (!user.IsGRNNoAutoGen)
            {
                if (txtGRn_NO.Text.Trim() == string.Empty)
                {
                    CheckAcivated = false;
                    MessageBox.Show("Enter GRN No...!", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbAR.Value == null)
            {
                MessageBox.Show("Invalid AR Account", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!IsSerialNoCorrect())
                return;

            int rowCount = GetFilledRows();
            string TranType = "Grn-Tran";
            int DocType = 2;
            bool QtyIN = true;

            bool CheckZeroReceived = false;
            try
            {
                for (int b = 0; b < rowCount; b++)
                {
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[b].Cells[3].Value) > 0)
                    {
                        CheckZeroReceived = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            bool IsItemSerial = false;
            //check wether this item is serialized or not=======================
            try
            {
                for (int a = 0; a < rowCount; a++)
                {
                    string ItemClass = "";
                    ItemClass = dgvGRNTransaction.Rows[a].Cells[0].Value.ToString().Trim();
                    if (ItemClass != "" && dgvGRNTransaction.Rows[a].Cells[8].Value.ToString().Trim() != "")
                    {
                        String S = "Select * from tblItemMaster where ItemID  = '" + dgvGRNTransaction.Rows[a].Cells[0].Value.ToString().Trim() + "'";
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

                        }
                        else
                        {
                            if (ItemClass == "")
                            {
                                MessageBox.Show("The Item You are going to ship is not in the Item Master File, Please Import Item Master File", "Goods Received Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            CheckAcivated = false;
            bool ISExeedLimit = false;
            string GRNNo = "";
            bool IsMinusAllow = true;
            bool IsminusValidate = false;
            try
            {
                String S31 = "select GRN_NO from tblGRNTran";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                SqlDataAdapter da31 = new SqlDataAdapter(S31, ConnectionString);
                DataTable dt31 = new DataTable();
                da31.Fill(dt31);
                if (dt31.Rows.Count > 10000000)
                {
                    ISExeedLimit = true;
                }
            }
            catch { }

            try
            {
                //check the dispatch qty is wether a number or not
                for (int a = 0; a < rowCount; a++)
                {
                    ChechDQty = 0;
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value);

                    Convert.ToDouble(txtTax1Amount.Text);
                    Convert.ToDouble(txtTax2.Text);
                    Convert.ToDouble(txtDisRate.Text);
                    Convert.ToDouble(txtDiscountAmount.Text);
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[3].Value) > Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value))
                    {
                        String S3 = "select OverGRN from tblDefualtSetting";// where ItemID = '" + Search.ItemId.ToString().Trim() + "'";
                        SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                        DataTable dt3 = new DataTable();
                        da3.Fill(dt3);
                        if (dt3.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(dt3.Rows[0].ItemArray[0]) == true)
                            {
                                IsMinusAllow = true;
                            }
                            else
                            {
                                IsMinusAllow = false;
                                IsminusValidate = true;
                            }
                        }

                    }
                    else
                    {
                        if (IsminusValidate == true)
                        {
                            IsMinusAllow = false;
                        }
                        else
                        {
                            IsMinusAllow = true;
                        }
                    }

                    //if (Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value) > 0)
                    //{
                    //    IsRecevedNull = true;
                    //}
                }

            }
            catch (Exception ex)
            {
                ChechDQty = 1;//if this flag is 1 the violate the number format
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }

            if (ChechDQty == 0)
            {
                if (txtTotalAmount.Text == "" || txtTotalAmount.Text == "0.00" || txtTotalAmount.Text == "0" ||
                    IsMinusAllow == false || ISExeedLimit == true || rowCount == 0 || CheckZeroReceived == false)
                {
                    if (txtTotalAmount.Text == "")
                    {
                        MessageBox.Show("Total Amount is Empty");
                    }

                    else if (IsMinusAllow == false)
                    {
                        if (!user.IsOverGRNQty)
                        {
                            MessageBox.Show("Received quantity can not be grater than the order quantity",sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else if (ISExeedLimit == true)
                    {
                        MessageBox.Show("Your Transaction Limit Over Please Register the Product",sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (rowCount == 0)
                    {
                        MessageBox.Show("Please Select a Purchase Order");
                    }
                    else if (txtTotalAmount.Text == "" || txtTotalAmount.Text == "0.00" || txtTotalAmount.Text == "0")
                    {
                        MessageBox.Show("You have not entered Received Quantity ",sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (CheckZeroReceived == false)
                    {
                        MessageBox.Show("You have not entered Received Quantity ",sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string GRNDate = DTP.ToString(Dformat);
                    SqlConnection myConnection = new SqlConnection(ConnectionString);
                    myConnection.Open();
                    SqlTransaction myTrans = myConnection.BeginTransaction();
                    try
                    {

                        StrSql = "DELETE FROM [tblGRNTran] WHERE GRN_NO='" + txtGRn_NO.Text.ToString().Trim() + "'";

                        SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                        command1.CommandType = CommandType.Text;
                        command1.ExecuteNonQuery();

                        //Get the current running number from the system
                        if (IsUpdate == false)
                        {

                            if (user.IsGRNNoAutoGen)
                            {
                                SqlCommand myCommand = new SqlCommand("UPDATE tblDefualtSetting with (rowlock) SET GRNNo = GRNNo + 1 select GRNNo, GRNPrefix from tblDefualtSetting with (rowlock)", myConnection, myTrans);
                                SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                DataTable dt41 = new DataTable();
                                da41.Fill(dt41);

                                if (dt41.Rows.Count > 0)
                                {
                                    GRNNo = dt41.Rows[0].ItemArray[0].ToString().Trim().PadLeft(8, '0');
                                    GRNNo = dt41.Rows[0].ItemArray[1].ToString().Trim() + "-" + GRNNo;
                                }
                                txtGRn_NO.Text = GRNNo;
                            }
                            else
                            {
                                SqlCommand myCommand = new SqlCommand("select * from tblGRNTran where GRN_NO='" + txtGRn_NO.Text.Trim() + "'", myConnection, myTrans);
                                SqlDataAdapter da41 = new SqlDataAdapter(myCommand);
                                DataTable dt41 = new DataTable();
                                da41.Fill(dt41);

                                if (dt41.Rows.Count > 0)
                                {
                                    MessageBox.Show("GRN No already exists....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    myTrans.Rollback();
                                    myConnection.Close();//
                                    return;
                                }
                            }
                        }

                        //============================================
                        for (int i = 0; i < rowCount; i++)
                        {
                            bool IsfullShipment = false;
                            bool Isinvoice = false;
                            // string TranType = "Tran-GRN";

                            bool Duplicate = true;
                            string NoOfDis = Convert.ToString(dgvGRNTransaction.Rows.Count - 1);
                            //==============UOM Details================


                            SqlCommand cmd34 = new SqlCommand("select Ratio from tblUOMMaster where UMID='" + dgvGRNTransaction.Rows[i].Cells["UOM"].Value.ToString().Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da34 = new SqlDataAdapter(cmd34);
                            DataTable dt34 = new DataTable();
                            da34.Fill(dt34);
                            if (dt34.Rows.Count > 0)
                            {
                                dblUOMRatio = Convert.ToDouble(dt34.Rows[0]["Ratio"].ToString());
                            }
                            else
                            {
                                dblUOMRatio = 1;
                            }
                            dblQuantity = Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["RQty"].Value.ToString());
                            dblConverteQty = dblUOMRatio * dblQuantity;
                           
                            if(SelectSONums1==""|| SelectSONums1==null)
                            {
                                string item = clistbxSalesOrder.CheckedItems[0].ToString();
                                 SelectSONums1 = item;
                            }

                            SqlCommand myCommand2 = new SqlCommand("insert into tblGRNTran(GRN_NO,VendorID,PONos,GRNDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,OrderQty,ReceiveQty,GlAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,Dupliate,ISGRNFinished,CustomerSO,WareHouseID,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1,PODistributionNO,ConvertedQty,IsActive,FreeQty) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendorSelect.Value.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbAR.Value.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[1, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[3, i].Value) + "','" + dgvGRNTransaction[8, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[7, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToDouble(dgvGRNTransaction[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "','" + Convert.ToDouble(dgvGRNTransaction[9, i].Value) + "','" + dblConverteQty + "','" + true + "','"+ Convert.ToDouble(dgvGRNTransaction[10, i].Value) + "')", myConnection, myTrans);
                            myCommand2.ExecuteNonQuery();

                            double SellingPrice = 0.00;

                            //SqlCommand cmd3 = new SqlCommand("update tblItemMaster set UnitCost='" + Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[5].Value) + "'where  ItemID='" + dgvGRNTransaction[0, i].Value.ToString().Trim() + "'", myConnection, myTrans);
                            //SqlCommand myCommand6 = new SqlCommand("Select * from  tblItemWhse where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);

                            //SqlDataAdapter da1 = new SqlDataAdapter(myCommand6);
                            //DataTable dt1 = new DataTable();
                            //da1.Fill(dt1);

                            //if (dt1.Rows.Count > 0)
                            //{
                            //    SqlCommand myCommand3 = new SqlCommand("update tblItemWhse set QTY = QTY + '" + dblConverteQty + "' where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbWarehouse.Value.ToString().Trim() + "'", myConnection, myTrans);
                            //    myCommand3.ExecuteNonQuery();
                            //}
                            //else
                            //{

                            //    SqlCommand myCommand4 = new SqlCommand("insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbWarehouse.Value.ToString().Trim() + "','" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[1, i].Value) + "','" + dblConverteQty + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + GRNDate + "')", myConnection, myTrans);
                            //    myCommand4.ExecuteNonQuery();
                            //}

                            //if (Convert.ToDouble(dgvGRNTransaction[3, i].Value) != 0)
                            //{
                            //    SqlCommand cmd11 = new SqlCommand(
                            //        "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + cmbWarehouse.Value.ToString().Trim() + "' AND ItemId='" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "') " +
                            //        " Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values (@OHQTY, '" + DocType + "','" + txtGRn_NO.Text.ToString().Trim() + "','" + GRNDate + "','" + TranType + "','" + QtyIN + "','" + dgvGRNTransaction[0, i].Value + "','" + dblConverteQty + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + dblConverteQty * Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + cmbWarehouse.Value.ToString().Trim() + "','" + SellingPrice + "')", myConnection, myTrans);
                            //    cmd11.ExecuteNonQuery();
                            //}

                            //string Stataus = "Available";
                            //bool IsGRNProcess = true;

                            ////==========================================setfull dispath
                            //if (Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[2].Value.ToString()) == Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[3].Value.ToString()))//changed cell value
                            //{
                            //    IsfullShipment = true;
                              // SetFullDispatch(ArraySONO, dgvGRNTransaction.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[3].Value.ToString()), IsfullShipment, int.Parse(dgvGRNTransaction.Rows[i].Cells["PODisNUmber"].Value.ToString()));
                            //}
                            //else
                            //{
                            //    UpdateSoTemptbl(ArraySONO, dgvGRNTransaction.Rows[i].Cells["ItemId"].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["RQty"].Value.ToString()), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells["dataGridViewTextBoxColumn6"].Value.ToString()), int.Parse(dgvGRNTransaction.Rows[i].Cells["PODisNUmber"].Value.ToString()));
                            //}
                            ////===============================================
                        }

                        //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                        //{
                        //    SqlCommand myCommandSe1 = new SqlCommand("insert into tblSerialItemTransaction  " +
                        //        " (TranType,Status,ItemID,WareHouseID,SerialNo)" +
                        //        " values('Grn-Tran','Available','" +
                        //        dr["ItemCode"].ToString() + "' ,'" + cmbWarehouse.Text.ToString().Trim() + "' ,'" +
                        //        dr["SerialNo"].ToString() + "')", myConnection, myTrans);
                        //    myCommandSe1.ExecuteNonQuery();
                        //}

                        //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Grn-Tran", cmbWarehouse.Text.ToString(), txtGRn_NO.Text.ToString().Trim(), dtpDispatchDate.Value, false, "");

                        myTrans.Commit();

                        // Disable();
                        MessageBox.Show("Good Received Note Successfully Saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //  btnPrint_Click(sender, e);
                        //Enable();
                        //ClearData();
                     
                        //clistbxSalesOrder.Items.Clear();
                     //   clistbxSalesOrder.Items.Add(SelectSONums1, CheckState.Checked);
                        //btnNew.Enabled = true;
                        SerchText = txtGRn_NO.Text.ToString();
                        savecomplete = true;
                        fillDetails();
                        btnSave.Enabled = false;
                        btnEditer.Enabled = false;
                        btnProcess.Enabled = true;
                        btnNew.Enabled = true;
                        SerchText = "";
                        btnPrint.Enabled = true;


                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                }


            }
            else
            {
                MessageBox.Show(" You must enter numeric value",sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //btnSave.Enabled = true;
            //btnEditer.Enabled = false;
            //btnProcess.Enabled = false;
            //btnPrint.Enabled = false;

        }
            
                               
                       
        Boolean IsUpdate = false;
        Boolean IsActive = true;
        private void btnEditer_Click(object sender, EventArgs e)
        {
            IsUpdate = true;
            btnfullShip.Enabled = true;
            btnEditer.Enabled = false;
            if (IsActive == true)
            {
                IsUpdate = true;
                btnSave.Enabled = true;
                btnProcess.Enabled = false;
                btnPrint.Enabled = false;
                dgvGRNTransaction.Enabled = true;
                dgvGRNTransaction.ReadOnly = false;
                dgvGRNTransaction.Columns[0].ReadOnly = true;
                dgvGRNTransaction.Columns[1].ReadOnly = true;
                dgvGRNTransaction.Columns[2].ReadOnly = true;
                dgvGRNTransaction.Columns[4].ReadOnly = true;
                dgvGRNTransaction.Columns[7].ReadOnly = true;
                btnProcess.Enabled = true;
            }
            else if (IsActive == false)
            {
                btnSave.Enabled = false;
                btnProcess.Enabled = false;
                btnPrint.Enabled = true;
            }
        }

        private void txtlocName_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtGRn_NO_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbAR_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbAR.Enabled == true)
                {
                    cmbAR.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void frmGRN_KeyDown(object sender, KeyEventArgs e)
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


            if (btnEditer.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E)
                {
                    btnEditer_Click(null, null);
                }
            }


            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                btnNew_Click(null, null);
            }


            if (btnProcess.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    btnSave_Click(null, null);
                }
            }


            if (btnPrint.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    btnPrint_Click(null, null);
                }
            }


            if (btnfullShip.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
                {
                    btnfullShip_Click(null, null);
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

        private void clistbxSalesOrder_KeyDown(object sender, KeyEventArgs e)
        {

            //clistbxSalesOrder.SetItemChecked(clistbxSalesOrder.SelectedIndex, true);
            //clistbxSalesOrder_SelectedIndexChanged(null, null);
        }
    }
}