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
    public partial class frmNewInvoice : Form
    {
        public frmNewInvoice()
        {
            setConnectionString();
            InitializeComponent();
        }
       
        Connector conn = new Connector();
       // public DSGRN objGRN = new DSGRN();
        public DSINV objGRN = new DSINV();
       // public DSDeliveryNotes  DSDispatch = new  DSDeliveryNotes();

        public static string ConnectionString;

       
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
            }
            catch { }
        }
        public bool flag1 = false;
        private void frmInvoice_Load(object sender, EventArgs e)
        {
            if (flag1 == true)
            {
                clistbxSalesOrder.Items.Clear();
               // string DeliveryNoteNo = ab.GetText2();
                string SerchText = ab.GetText2();

                string ConnString = ConnectionString;
                String S1 = "Select * from tblInvoiceTransaction where GRN_NO='" + SerchText + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        txtGRn_NO.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                        cmbVendor.Text = dt.Rows[i].ItemArray[1].ToString().Trim();
                        LoadVendor();
                        string abc = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dtpDispatchDate.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                        cmbARAccount.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
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
                       

                        dgvGRNTransaction.Rows.Add();

                        dgvGRNTransaction.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[9].ToString().Trim();

                       // DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvGRNTransaction.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[8].ToString().Trim();

                        dgvGRNTransaction.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[14].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[12].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[23].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[13].ToString().Trim();
                        dgvGRNTransaction.Rows[i].Cells[8].Value = dt.Rows[i].ItemArray[11].ToString().Trim();

                    }
                }
            }
            else
            {
                flag1 = false;
                CusDataLoad();
                WarehouseDataLoad();
                loadChartofAcount();
                TaxRateLoad();
            }

           
        }

        public void TaxRateLoad()
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

        public void CusDataLoad()
        {
            try
            {
                String S = "Select * from tblCustomerMaster";// where CutomerID='" + cmbCustomer.Text.ToString().Trim() + "'";
              //  String S = "Select * from tblVendorMaster";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbVendor.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch { }
        
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
            catch { }

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
            catch { }

        }
        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDispacthTransaction_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void tabDispach_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cmbCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                try
                {
                    String S = "Select * from tblCustomerMaster where CutomerID='" + cmbVendor.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        txtcusName.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
                        txtCusAdd1.Text = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                        txtCusAdd2.Text = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
                    }
                }
                catch { }
                txtcusName.Focus();
            }
        }

        private void cmbCustomer_Click(object sender, EventArgs e)
        {

            
        }

        private void cmbCustomer_Enter(object sender, EventArgs e)
        {
            
        }

        private void cmbCustomer_Click_1(object sender, EventArgs e)
        {
           
        
        }


        private void LoadVendor()
        {

            String S = "Select * from tblVendorMaster where VendorID='" + cmbVendor.Text.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                // cmbCustomer.Text = cmbCustomer.Text;
                txtcusName.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
                txtCusAdd1.Text = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                txtCusAdd2.Text = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
                txtcusCity.Text = dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim();
            }
        }
        private void cmbCustomer_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {

                String S = "Select * from tblCustomerMaster where CutomerID='" + cmbVendor.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                   // cmbCustomer.Text = cmbCustomer.Text;
                    txtcusName.Text = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
                    txtCusAdd1.Text = dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim();
                    txtCusAdd2.Text = dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim();
                    txtcusCity.Text = dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim();
                }
            }
            catch { }
            clistbxSalesOrder.Items.Clear();
            Load_salesOrder();//this mehod load al sales orders into to the cobobox regard the selected customer
            dgvGRNTransaction.Rows.Clear();
        }
        private void Load_salesOrder()
        {
            try
            {
                bool ISSoClosed = false; //sales order closed
                bool dispatch = false;
                //PONumber
                String S = "Select distinct(SONumber) from tblSalesOrder where CustomerID='" + cmbVendor.Text.ToString().Trim() + "' and IsFullInvoice='" + dispatch + "' and IsSOClosed='" + ISSoClosed + "'";
                //String S = "Select distinct(SalesOrderNo) from tblSalesOrderTemp where CustomerID='" + cmbVendor.Text.ToString().Trim() + "' and IsfullDispatch='" + dispatch + "' and IsSoClosed='" + ISSoClosed + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
               // cmbSalesOrderNo.Items.Clear();

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                       // cmbSalesOrderNo.Visible = false;
                    }
                    else
                    {
                       // cmbSalesOrderNo.Visible = true;
                       // cmbSalesOrderNo.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                        clistbxSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
                if (dt.Tables[0].Rows.Count == 0)
                {
                   // cmbSalesOrderNo.Visible = false;
                }
              
            }
            catch { }
        }
       

        private void cmbSalesOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void dgvdispactApplytoSales_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
            catch 
            {
                System.Drawing.Color R1 = new Color();
                R1 = Color.Yellow;
                MessageBox.Show("Enter Numeric Value");
                txtTotalAmount.Text = "0";
                for (int a = 0; a < dgvGRNTransaction.Rows.Count - 1; a++)
                {
                    dgvGRNTransaction.Rows[0].Cells[6].Style.BackColor = R1;
                    dgvGRNTransaction.Rows[0].Cells[2].Style.BackColor = R1;
                }
            }

        }

        private void dgvdispactApplytoSales_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connector conn = new Connector();
            conn.ImportSalesOrderList();
            conn.Insert_SalesOrder();
            MessageBox.Show("Sales orders Loaded");
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
                    //dgvdispactApplytoSales.Rows.Clear();
                }

            }
            catch { }
        
        
        }

        private void clistbxSalesOrder_Click(object sender, EventArgs e)
        {
            
        }

        private void clistbxSalesOrder_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
        }


        public ArrayList ArraySONO = new ArrayList();//Sales orders related to a custmer
        
        public string SelectSONums1 = "";//saving purpose


        private void clistbxSalesOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
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

                        So_No1= So_No1 + " ";//savins purpose
                        SelectSONums1 = SelectSONums1 + So_No1;//saving purpose

                    }
                    i++;
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

                    for (int k = 0; k < ds.Tables[0].Rows.Count-1; k++)
                    {
                        dgvGRNTransaction.Rows.Add();
                    }


                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {

                        //dgvdispactApplytoSales.Rows.Add();
                        dgvGRNTransaction.Rows[j].Cells[0].Value = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[1].Value = ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim();

                      //  dgvGRNTransaction.Rows[j].Cells[2].Value = ds1.Tables[0].Rows[j].ItemArray[0].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[2].Value = "0";
                        dgvGRNTransaction.Rows[j].Cells[3].Value = ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[4].Value = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[5].Value = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();
                        dgvGRNTransaction.Rows[j].Cells[6].Value = "0";
                        dgvGRNTransaction.Rows[j].Cells[7].Value = "0";
                        dgvGRNTransaction.Rows[j].Cells[8].Value = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();
                       // txtCustomerPO.Text = ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim();
                    }
                }

                //dgvGRNTransaction.Columns[0].ReadOnly = true;
                //dgvGRNTransaction.Columns[1].ReadOnly = true;
                //dgvGRNTransaction.Columns[3].ReadOnly = true;
                //dgvGRNTransaction.Columns[4].ReadOnly = true;
                //dgvGRNTransaction.Columns[5].ReadOnly = true;
                //dgvGRNTransaction.Columns[6].ReadOnly = true;

            }
            catch { }

       

        }



        public DataSet ReturnSOList(string SO_No)
        {
            DataSet ds = new DataSet();
            
            try
            {
                String S = "Select ItemId,Sum(RemainQty),Description,GL_Account,UnitPrice,UOM from tblSalesOrder where SONumber in (" + SO_No + ")and IsFullInvoice='false'  group by ItemId,Description,GL_Account,UnitPrice,UOM";
                //String S = "Select ItemId,Sum(RemainQty),Description,GL_Account,UnitPrice,UOM from tblPurchaseOrder where PONumber in (" + SO_No + ")and IsFullGRN='false'  group by ItemId,Description,GL_Account,UnitPrice,UOM";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                da.Fill(ds, "SO");
            }
            catch 
            {

            }
            return ds;
        }

        public string ReturnCusPO(string SO_No)
        {
            string CusPO = "";

            try
            {
                String S1 = "Select CustomerPO from tblSalesOrder where SONumber in (" + SO_No + ")";   
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        CusPO = dt1.Rows[i].ItemArray[1].ToString();
                        //dgvPOLIst.Rows.Add();
                        //dgvPOLIst.Rows[i].Cells[0].Value = dt1.Rows[i].ItemArray[1].ToString();
                        //dgvPOLIst.Rows[i].Cells[1].Value = dt1.Rows[i].ItemArray[0].ToString();
                        //dgvPOLIst.Rows[i].Cells[2].Value = dt1.Rows[i].ItemArray[2].ToString();
                        //dgvPOLIst.Rows[i].Cells[3].Value = dt1.Rows[i].ItemArray[3].ToString();
                        //dgvPOLIst.Rows[i].Cells[4].Value = "0";
                    }
                }
            }
            catch
            {
            }
            return CusPO;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public int ChechDQty = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool IsMinusAllow = false;

            try
            {
                //check the dispatch qty is wether a number or not
                for (int a = 0; a < dgvGRNTransaction.Rows.Count - 1; a++)
                {
                    ChechDQty = 0;
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[5].Value);
                    Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[6].Value);

                    Convert.ToDouble(txtTax1Amount.Text);
                    Convert.ToDouble(txtTax2.Text);
                    Convert.ToDouble(txtDisRate.Text);
                    Convert.ToDouble(txtDiscountAmount.Text);
                    if (Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[2].Value) > Convert.ToDouble(dgvGRNTransaction.Rows[a].Cells[1].Value))
                    {
                        IsMinusAllow = true;
                    }
                }

            }
            catch
            {
                ChechDQty = 1;//if this flag is 1 the violate the number format
            }
            //if (dgvGRNTransaction.Rows[a].Cells[2].Value() > dgvGRNTransaction.Rows[a].Cells[1].Value())
            //{
            //    IsMinusAllow = true;
            //}
            if (ChechDQty == 0)
            {
                string DeliveryNoteNo = NextDeliveryNoteNo();
                txtGRn_NO.Text = DeliveryNoteNo;
                //==============form level validations
                if (cmbVendor.Text == "" || cmbARAccount.Text == "" || txtTotalAmount.Text == "" || txtGRn_NO.Text == "" || cmbLocation.Text == "" || IsMinusAllow==true)
                {
                    if (cmbVendor.Text == "")
                    {
                        MessageBox.Show("Select Vendor");
                        btnSave.Focus();
                    }
                    else if (cmbARAccount.Text == "")
                    {
                        MessageBox.Show("Select AR Account");
                        btnSave.Focus();
                    }
                    else if (txtTotalAmount.Text == "")
                    {
                        MessageBox.Show("Total Amount is Empty");
                        btnSave.Focus();
                    }
                    else if (txtGRn_NO.Text == "")
                    {
                        MessageBox.Show("Database Error");
                        btnSave.Focus();
                    }
                    else if (cmbLocation.Text == "")
                    {
                        MessageBox.Show("Select a Location");
                        btnSave.Focus();
                    }
                    else if (IsMinusAllow  == true)
                    {
                        MessageBox.Show("System Doesnot Allow Minus Quantity");
                        btnSave.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Fill All Details");
                        btnSave.Focus();
                    }
                }
                else
                {

                    DateTime DTP = Convert.ToDateTime(dtpDispatchDate.Text);
                    string Dformat = "MM/dd/yyyy";
                    string GRNDate = DTP.ToString(Dformat);

                    for (int i = 0; i < dgvGRNTransaction.Rows.Count - 1; i++)
                    {

                        setConnectionString();
                        SqlConnection myConnection = new SqlConnection(ConnectionString);
                        SqlCommand myCommand = new SqlCommand();
                        SqlTransaction myTrans;
                        myConnection.Open();
                        myCommand.Connection = myConnection;

                        myTrans = myConnection.BeginTransaction();
                        myCommand.Transaction = myTrans;

                        bool IsfullShipment = false;
                        bool Isinvoice = false;
                        string TranType = "Tran-";
                        try
                        {
                            bool Duplicate = true;
                            string NoOfDis = Convert.ToString(dgvGRNTransaction.Rows.Count - 1);
                            //myCommand.CommandText = "insert into tblDispatchOrder(DeliveryNoteNo,CustomerID,SONos,DispatchDate,ArAccount,NoOfDistributions,DistributionNo,ItemID,Description,OrderQty,DispatchQty,GL_Account,UnitPrice,Amount,TotalAmount,CurrentDate,Time,CurrentUser,Duplicate,IsInvoce,CustomePO) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + dtpDispatchDate.Text.ToString().Trim() + "','" + cmbARAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[3, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[1, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + dgvGRNTransaction[4, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[6, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerPO.Text.ToString().Trim() + "')";
                           // myCommand.CommandText = "insert into tblGRNTran(GRN_NO,VendorID,PONos,GRNDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,OrderQty,ReceiveQty,GlAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,Dupliate,ISGRNFinished,CustomerSO,WareHouseID) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbARAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[3, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[1, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + dgvGRNTransaction[8, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[7, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "')";
                            //tblInvoiceTransaction
                            //myCommand.CommandText = "insert into tblGRNTran(GRN_NO,VendorID,PONos,GRNDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,OrderQty,ReceiveQty,GlAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,Dupliate,ISGRNFinished,CustomerSO,WareHouseID,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbARAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[3, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[1, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + dgvGRNTransaction[8, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[7, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvGRNTransaction[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "')";
                            myCommand.CommandText = "insert into tblInvoiceTransaction(GRN_NO,VendorID,PONos,GRNDate,APAccount,NoOfDis,DistributionNo,ItemID,Description,OrderQty,ReceiveQty,GlAccount,UnitPrice,Amount,UOM,TotalAmount,CurrentDate,Time,CurrentUser,Dupliate,ISGRNFinished,CustomerSO,WareHouseID,LineDiscountRate,NetTotal,TotalDisRate,TotalDisAmount,Tax1,Tax2,Tax1Amount,Tax2Amount,TaxRate,TaxRate1) values ('" + txtGRn_NO.Text.ToString().Trim() + "','" + cmbVendor.Text.ToString().Trim() + "','" + SelectSONums1 + "','" + GRNDate + "','" + cmbARAccount.Text.ToString().Trim() + "','" + NoOfDis + "','" + (i + 1).ToString().Trim() + "','" + dgvGRNTransaction[0, i].Value + "','" + dgvGRNTransaction[3, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[1, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + dgvGRNTransaction[8, i].Value + "','" + Convert.ToDouble(dgvGRNTransaction[5, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[7, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + Convert.ToDouble(txtTotalAmount.Text) + "','" + System.DateTime.Now.ToShortDateString().Trim() + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "','" + user.userName.ToString().Trim() + "','" + Duplicate + "','" + Isinvoice + "','" + txtCustomerSO.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToDouble(dgvGRNTransaction[6, i].Value) + "','" + Convert.ToDouble(txtNetTotal.Text) + "','" + Convert.ToDouble(txtDisRate.Text) + "','" + Convert.ToDouble(txtDiscountAmount.Text) + "','" + cmbtaxSys1.Text.ToString().Trim() + "','" + cmbtaxSys2.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTax1Amount.Text) + "','" + Convert.ToDouble(txtTax2.Text) + "','" + TaxRate + "','" + TaxRate1 + "')";
                            myCommand.ExecuteNonQuery();

                            myCommand.CommandText = "Select * from  tblItemWhse where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'";
                            myCommand.ExecuteNonQuery();
                            SqlDataAdapter da1 = new SqlDataAdapter(myCommand.CommandText, ConnectionString);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);
                            if (dt1.Rows.Count > 0)
                            {
                                //myCommand.CommandText = "update tblItemWhse set QTY = QTY + '" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "' where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'";
                                myCommand.CommandText = "update tblItemWhse set QTY = QTY - '" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "' where ItemId='" + dgvGRNTransaction[0, i].Value + "' and WhseId='" + cmbLocation.Text.ToString().Trim() + "'";
                                //UPDATE Videos SET TimesViewed = TimesViewed + 1 WHERE (VideoId = @VideoId)
                                //update tblItemWhse set QTY = QTY + -10 where ItemId='AA'
                                myCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                myCommand.CommandText = "insert into  tblItemWhse(WhseId,ItemId,ItemDis,QTY,UOM,TraDate) values('" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[3, i].Value) + "','" + Convert.ToDouble(dgvGRNTransaction[2, i].Value) + "','" + Convert.ToString(dgvGRNTransaction[4, i].Value) + "','" + GRNDate + "')";
                                myCommand.ExecuteNonQuery();
                            }

                            myCommand.CommandText = "insert into tblInvTransaction(TDate,ItemID,FrmWhseId,ToWhseId,QTY,Type) values ('" + GRNDate + "','" + Convert.ToString(dgvGRNTransaction[0, i].Value) + "','" + cmbLocation.Text.ToString().Trim() + "','" + cmbLocation.Text.ToString().Trim() + "','" + Convert.ToString(dgvGRNTransaction[2, i].Value) + "','" + TranType + "')";
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

                        //==========================================setfull dispath
                        if (Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[1].Value.ToString()) == Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[2].Value.ToString()))//changed cell value
                        {
                            IsfullShipment = true;
                            SetFullDispatch(ArraySONO, dgvGRNTransaction.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[2].Value.ToString()), IsfullShipment);
                        }
                        else
                        {
                            try
                            {
                                UpdateSoTemptbl(ArraySONO, dgvGRNTransaction.Rows[i].Cells[0].Value.ToString(), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[2].Value.ToString()), Convert.ToDouble(dgvGRNTransaction.Rows[i].Cells[5].Value.ToString()));
                            }
                            catch
                            {

                            }

                        }
                        //===============================================
                    }


                    //CreatePurchaseJXML();
                    //Connector conn = new Connector();
                    //conn.ImportMSalesInv();



                   // conn.Upload_GRN();

                    //ImportMSalesInv


                    //MessageBox.Show("Successfuly Saved");
                   btnPrint_Click(sender, e);
                   btnNew_Click(sender, e);

                }


            }
            else 
            {
                MessageBox.Show("Numeric Convertion Error");
                btnSave.Focus();
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
                String S2 = "Select Distinct(PONO) from tblINVMSO where GRNNO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(DistinctPO);
            }
            catch
            {

            }
            return DistinctPO;
        }

        //==========================================================

        public DataSet GetPOQty(string PONO,string GRNNO)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet POQty = new DataSet();
            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                String S2 = "Select ItemID,Qty,PONO,PODisNo from tblINVMSO where PONO='" + PONO + "' and GRNNO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(POQty);
            }
            catch
            {

            }
            return POQty;
        }
        //=====================================================

        public DataSet ItemData(string ItemID,string GRNNO)//this is regarding the Scan Details
        {
            setConnectionString();
            DataSet ItemData = new DataSet();
            try
            {
                string ConnString = ConnectionString;//ReceiptsNo ConsultantName
                //String S2 = "Select VendorID,GRNDate,APAccount,ItemID,Description,GlAccount,UnitPrice,Amount,UOM,CustomerSO,DistributionNo,LineDiscountRate,TaxRate,TaxRate1 from tblGRNTran where ItemID='" + ItemID + "' and GRN_NO='" + GRNNO + "'";
                String S2 = "Select VendorID,GRNDate,APAccount,ItemID,Description,GlAccount,UnitPrice,Amount,UOM,CustomerSO,DistributionNo,LineDiscountRate,TaxRate,TaxRate1 from tblInvoiceTransaction where ItemID='" + ItemID + "' and GRN_NO='" + GRNNO + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                da2.Fill(ItemData);
            }
            catch
            {

            }
            return ItemData;
        }

        //=======================================================



        DataSet DistnctPO = new DataSet();
        DataSet POQuantity = new DataSet();
        DataSet ItemDetails = new DataSet();

        //Create a XML File for import Purchase Journal=====================
        public void  CreatePurchaseJXML()
        {

            
            //XmlTextWriter Writer = new XmlTextWriter(@"c:\\GRNExport.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;
            //Writer.WriteStartElement("PAW_Purchases");
            //Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            //Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            //Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
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

                Writer.WriteStartElement("PAW_Invoice");
                Writer.WriteAttributeString("xsi:type", "paw:Invoice");
               // Writer.WriteAttributeString("xsi:type", "paw:Receipt");


               // Writer.WriteStartElement("PAW_Purchase");
               // Writer.WriteAttributeString("xsi:type", "paw:purchase");

               POQuantity = GetPOQty(DistnctPO.Tables[0].Rows[i].ItemArray[0].ToString().Trim(),txtGRn_NO.Text.ToString().Trim());
               for (int j = 0; j < POQuantity.Tables[0].Rows.Count + 3; j++)
               {
                   NODistribution = POQuantity.Tables[0].Rows.Count + 3;
                   if (j < POQuantity.Tables[0].Rows.Count)
                   {
                       ItemDetails = ItemData(POQuantity.Tables[0].Rows[j].ItemArray[0].ToString().Trim(), txtGRn_NO.Text.ToString().Trim());

                       Writer.WriteStartElement("Customer_ID");
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
                       Writer.WriteString("3/15/10");
                       // Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[1].ToString().Trim());
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Accounts_Receivable_Account");
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

                       Writer.WriteStartElement("Tax_Type");
                       Writer.WriteString("1");//Doctor Charge
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Amount");
                      // Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[7].ToString().Trim());
                       Writer.WriteString(Amount.ToString("N2"));
                       Writer.WriteEndElement();

                      // Amount = Convert.ToDouble(ItemDetails.Tables[0].Rows[0].ItemArray[7]);
                       //NetAmount = NetAmount + Amount;

                       Writer.WriteStartElement("SO_DistNumber");
                       // Writer.WriteString(Convert.ToString(j + 1)); //prev one
                       Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[3].ToString().Trim());
                       //Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[10].ToString().Trim());
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Applied_To_SO");
                       Writer.WriteString("TRUE");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("SO_Number");
                       // Writer.WriteString(POQuantity.Tables[0].Rows[i].ItemArray[2].ToString().Trim());
                       Writer.WriteString(POQuantity.Tables[0].Rows[j].ItemArray[2].ToString().Trim());
                       Writer.WriteEndElement();


                   }
                   //==================================================================================
                   if (j == POQuantity.Tables[0].Rows.Count)
                   {

                       //============VAT============VAT==================VAT===========================

                       Writer.WriteStartElement("Customer_ID");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Invoice_Number");
                       Writer.WriteString(GRnNo);
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Date");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("3/15/10");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Accounts_Receivable_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("11000-00");//Date 
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
                       Writer.WriteString("VAT001");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Description");
                       Writer.WriteString("VAT");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("GL_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("40000-00");
                       Writer.WriteEndElement();

                       TOT_D = TotAmount - LineDisCount;//Totalamount-LineDiscount
                       Tax1Amount = TOT_D * TaxRate;//(Totalamount-LineDiscount)*TaxRate
                       Net_Tax1 = TOT_D + Tax1Amount;//TOT_D+Tax1Amount
                       Tax2Amount = Net_Tax1 * TaxRate1;

                       Writer.WriteStartElement("Tax_Type");
                       Writer.WriteString("1");//Doctor Charge
                       Writer.WriteEndElement();
                      
                       Writer.WriteStartElement("Amount");
                       Writer.WriteString(Tax1Amount.ToString("N2"));
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("SO_DistNumber");
                       Writer.WriteString("0");
                       Writer.WriteEndElement();


                       Writer.WriteStartElement("Applied_To_SO");
                       Writer.WriteString("FALSE");
                       Writer.WriteEndElement();
                   }

                   if (j == POQuantity.Tables[0].Rows.Count + 1 )
                   {

                       //===========NBT==============NBT=======================NBT=============================
                       Writer.WriteStartElement("Customer_ID");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Invoice_Number");
                       Writer.WriteString(GRnNo);
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Date");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("3/15/10");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Accounts_Receivable_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("11000-00");//Date 
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
                       Writer.WriteString("NBT001");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Description");
                       Writer.WriteString("NBT");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("GL_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("41000-00");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Tax_Type");
                       Writer.WriteString("1");//Doctor Charge
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Amount");
                       Writer.WriteString(Tax2Amount.ToString("N2"));
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("SO_DistNumber");
                       Writer.WriteString("0");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Applied_To_SO");
                       Writer.WriteString("FALSE");
                       Writer.WriteEndElement();
                   }
                   if (j == POQuantity.Tables[0].Rows.Count + 2)
                   {

                       //=========DISCOUNT=========DISCOUNT==================DISCOUNT==========================
                       Writer.WriteStartElement("Customer_ID");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString(ItemDetails.Tables[0].Rows[0].ItemArray[0].ToString().Trim());
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Invoice_Number");
                       Writer.WriteString(GRnNo);
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Date");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("3/15/10");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Accounts_Receivable_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("11000-00");//Date 
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
                       Writer.WriteString("DISCOUNT001");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Description");
                       Writer.WriteString("Discount");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("GL_Account");
                       Writer.WriteAttributeString("xsi:type", "paw:id");
                       Writer.WriteString("58000-00");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Tax_Type");
                       Writer.WriteString("1");//Doctor Charge
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Amount");
                       Writer.WriteString("-" + LineDisCount.ToString("N2"));
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("SO_DistNumber");
                       Writer.WriteString("0");
                       Writer.WriteEndElement();

                       Writer.WriteStartElement("Applied_To_SO");
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
            string POid = poid.Trim();
            setConnectionString();
            Double OrdQty = 0;
            string ConnString = ConnectionString;

            string sql = "select RemainQty from tblSalesOrder where SONumber='" + POid + "' and ItemId='" + ItemID + "'";
            //string sql = "select RemainQty from tblPurchaseOrder where PONumber='" + POid + "' and ItemId='" + ItemID + "'";
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
        //==============================================================

        //==============================================================
        public Double GetPrevOrdQTY(string poid, string ItemID, double UPrice)
        {
            string POid = poid.Trim();
            setConnectionString();
            Double OrdQty = 0;
            string ConnString = ConnectionString;
            //string sql = "select RemainQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
            string sql = "select RemainQty from tblSalesOrder where SONumber='" + POid + "' and ItemId='" + ItemID + "' and UnitPrice=" + UPrice + "";
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

        //==================================================================
        //======================================================

        public bool GetIem(string poid, string ItemID)
        {
            string POid = poid.Trim();
            bool IsAvalble = false;
            setConnectionString();
            Double OrdQty = 0;
            string ConnString = ConnectionString;
            //string sql = "select ItemID from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID  + "'";
            string sql = "select ItemId from tblSalesOrder where SONumber='" + POid + "' and ItemId='" + ItemID + "'";
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
        //=========================================================



        public Double GetPrevOrginalQty(string poid, string ItemID, double UPrice)
        {
            string POid = poid.Trim();
            setConnectionString();
            Double DispatchQ = 0;
            string ConnString = ConnectionString;
            //string sql = "select DispatchQty from tblSalesOrderTemp where SalesOrderNo='" + POid + "' and ItemID='" + ItemID + "' and UnitPrice=" + UPrice + "";
            string sql = "select DespatchQty from tblSalesOrder where SONumber='" + POid + "' and ItemId='" + ItemID + "' and Amount=" + UPrice + "";
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

        //=================================================================


        public void SetDispatchFOR_Partial(string SOID, string ItemID, bool FullShip,double DispatchQty)
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
                String S = "Select Quantity,DespatchQty,RemainQty from tblSalesOrder where SONumber = '" + SOID + "' and ItemId='" + ItemID + "'";
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

            }
            catch
            {

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
            try
            {
                string ConnString = ConnectionString;
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand cmd = Conn.CreateCommand();
                Conn.Open();
                //cmd.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + fullshipment + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID + "' and ItemID='" + ItemID + "';";
                cmd.CommandText = "UPDATE tblSalesOrder SET IsFullInvoice = '" + fullshipment + "',DespatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SONumber = '" + SOID + "' and ItemId='" + ItemID + "';";
                cmd.ExecuteNonQuery();

                if (DispatchQty == 0)
                { }
                else
                {
                        // cmd.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "'";
                    cmd.CommandText = "update tblINVMSO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updatefull + "' where PONO='" + SOID + "' and ItemID='" + ItemID + "'";

                        // cmd.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + SOID + "','" + ItemID + "','" + DispatchQty + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                        cmd.ExecuteNonQuery();
                   
                }


                Conn.Close();
            }
            catch { }
        }
        //=========================================================

        public void UpdateSOTemp(string PO_NO_update, string ItemID_update, double ReceivableQty_update,double DispatchQ, double UnitPrice_update)
        {
            bool fullDispath = false;
            double updateQty = 0;
            double OriginalQty= 0;
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
                String S = "Select DespatchQty,Quantity from tblSalesOrder where SONumber = '" + PO_NO_update + "' and ItemId='" + ItemID_update + "'";
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

            }
            catch
            {

            }
            updateQty = updateQty + DispatchQ;
            RemainQty = OriginalQty - updateQty;


            //==================================================================

            string ConnString = ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnString);
            SqlCommand cmd1 = Conn.CreateCommand();
           // cmd1.CommandText = "UPDATE tblSalesOrderTemp SET RemainQty=" + RemainQty + ",DispatchQty='" + updateQty + "',IsfullDispatch='" + fullDispath + "' where SalesOrderNo='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemID='" + ItemID_update + "'";
            cmd1.CommandText = "UPDATE tblSalesOrder SET RemainQty=" + RemainQty + ",DespatchQty  ='" + updateQty + "',IsFullInvoice='" + fullDispath + "' where SONumber='" + PO_NO_update + "'and UnitPrice=" + UnitPrice_update + " and ItemId='" + ItemID_update + "'";
            Conn.Open();
            cmd1.ExecuteNonQuery();
            if (DispatchQ == 0)
            { }
            else
            {
                cmd1.CommandText = "update tblINVMSO set Qty ='" + DispatchQ + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + Ischeck + "' where PONO='" + PO_NO_update + "' and ItemID='" + ItemID_update + "'";
                    // cmd1.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQ + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + PO_NO_update + "' and ItemID='" + ItemID_update + "'";

                    //cmd1.CommandText = "insert into tblGRNMPO(PONO,ItemID,Qty,GRNNO) values ('" + PO_NO_update + "','" + ItemID_update + "','" + DispatchQ + "','" + txtGRn_NO.Text.ToString().Trim() + "')";
                    cmd1.ExecuteNonQuery();
             }

            Conn.Close();
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
             double prevqtyTemp1=0;
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
                     double orderdqty = GetOrdQTY1(SOID, ItemID, UPrice);//order quantity mean Actual Remain Qty-done
                     if(orderdqty != 0)
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
                                     //Sanjeewa
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
                                 try
                                 {
                                     {
                                         //4
                                         UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice);
                                         PQR = true;
                                     }
                                 }
                                 catch
                                 {

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
                                         try
                                         {
                                             {
                                                 //5
                                                 UpdateSOTemp(SOIDs[i].ToString(), ItemID, DispathQty, dispatchQty, UPrice);
                                                 ABC = true;
                                                 PQR = true;
                                             }
                                         }
                                         catch
                                         {

                                         }
                                     }
                                 }
                                // }

                             }

                         }
                     }//if(orderQty==0)
                     //=====================================================
                 }
                 catch
                 { }
             }

        //========================================


        }


        public void SetFullDispatch(ArrayList SOID, string ItemID,double DispatchQty, bool FullDispatch)
        {
            try
            {
                double  OrginalQty = 0;
                double  UpdateDispatchQty = 0;
                double  RemainQty = 0;
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


                    try
                    {
                        //String S = "Select Quantity,DispatchQty,RemainQty from tblSalesOrderTemp where SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                        //String S = "Select Quantity,GRNQty,RemainQty from tblPurchaseOrder where PONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "'";
                        String S = "Select Quantity,DespatchQty,RemainQty from tblSalesOrder where SONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "'";
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
                                RemainQty=Convert.ToDouble(dt1.Rows[k].ItemArray[2].ToString());
                            }
                        }

                    }
                    catch
                    {

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
                    String S1 = "Select ISFull from tblINVMSO where PONO = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da1.Fill(dt2);
                    if (dt2.Rows.Count > 0)
                    {
                        for (int k = 0; k < dt2.Rows.Count; k++)
                        {
                            IsCheckFull = Convert.ToBoolean (dt2.Rows[k].ItemArray[0].ToString());
                           
                        }
                    }

                    //=======================================================
                    //kjkjkjkfjfjf
                    //=====================================
                    //myCommand.CommandText = "UPDATE tblSalesOrderTemp SET IsfullDispatch = '" + FullDispatch + "',DispatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SalesOrderNo = '" + SOID[i].ToString() + "' and ItemID='" + ItemID + "';";
                    myCommand.CommandText = "UPDATE tblSalesOrder SET IsFullInvoice = '" + FullDispatch + "',DespatchQty='" + UpdateDispatchQty + "',RemainQty='" + RemainQty + "' WHERE SONumber = '" + SOID[i].ToString() + "' and ItemId='" + ItemID + "';";
                    myCommand.ExecuteNonQuery();

                   // myCommand.CommandText = "update tblGRNMPO set Qty = Qty + '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                    if (IsCheckFull == true)
                    {
                        if (DispatchQty == 0)
                        {
                            myCommand.CommandText = "update tblINVMSO set Qty = '" + OrginalQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                             myCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            myCommand.CommandText = "update tblINVMSO set Qty = '" + DispatchQty + "',GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (DispatchQty == 0)
                        {
                            myCommand.CommandText = "update tblINVMSO set Qty = '" + OrginalQty + "'-Qty,GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
                            myCommand.ExecuteNonQuery();
                        }
                        else 
                        {
                            myCommand.CommandText = "update tblINVMSO set Qty = '" + DispatchQty + "'-Qty,GRNNO='" + txtGRn_NO.Text.ToString().Trim() + "',ISFull='" + updateCheck + "' where PONO='" + SOID[i].ToString() + "' and ItemID='" + ItemID + "'";
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
            catch { }
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
            catch { }
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
            catch { }
            return NDispachTranLink;
        }


        //======================================================================

        public string NextDeliveryNoteNo()//get the next dispatch link number
        {
            string NextDNoteNo = "";

            try
            {
                string ConnString = ConnectionString;
                string sql = "Select GRN_NO from tblInvoiceTransaction ORDER BY GRN_NO";
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
                        NextDNoteNo = "INV-100000";
                    //}
                }


            }
            
            catch { }
            return NextDNoteNo;
        }

        //=====================================GEt next ID===================


        public string getNextID(string s)
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

        private void btnNew_Click(object sender, EventArgs e)
        {
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

            txtcusName.Text = "";
            txtcusCity.Text = "";
            txtCusAdd1.Text = "";
            txtCusAdd2.Text = "";
            txtGRn_NO.Text = "";
            txtTotalAmount.Text = ""; dgvGRNTransaction.Rows.Clear();
            clistbxSalesOrder.Items.Clear();
            cmbVendor.Items.Clear();
            cmbVendor.Text = "";
            cmbARAccount.Items.Clear();
            CusDataLoad();
            loadChartofAcount();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            try
            {
                objGRN.DtINVTran.Clear();
                String S3 = "Select * from tblInvoiceTransaction where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3 = new SqlCommand(S3);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                da3.Fill(objGRN, "DtINVTran");

                frmNewInvoicePrint invprint = new frmNewInvoicePrint(this);
                invprint.Show();
               // frmDeiveryNotePrint frm = new frmDeiveryNotePrint(this);
                //frm.Show();
            }
            catch { }

        }

        private void btnList_Click(object sender, EventArgs e)
        {
            frmInvoiceList invList = new frmInvoiceList();
            invList.Show();
            this.Close();

           // frmDeliveryNoteList dList = new frmDeliveryNoteList();
            //dList.Show();
            //this.Close();
        }
        ClassDriiDown ab = new ClassDriiDown();

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

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
                }
            }
            catch { }
        }

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
            catch
            { }
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
            catch
            { }
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

            catch
            { }
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
            catch { }
            cmbtaxSys1.Enabled = true;
            cmbtaxSys2.Enabled = true;


        }

        private void btnFormSetting_Click(object sender, EventArgs e)
        {
            frmGRNSetting gset = new frmGRNSetting();
            gset.Show();
            this.Close();
        }

    }
}