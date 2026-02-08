using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Timers;

namespace UserAutherization
{
    public partial class frmDailySales : Form
    {
        public frmDailySales()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        Connector conn = new Connector();
        public DSDailySalesSummary DSDAILY = new DSDailySalesSummary();
        clsCommon objclsCommon = new clsCommon();

        public double TotAmount  ;
        public double AM5000=0;
        public double AM2000=0;
        public double AM1000=0;
        public double AM500=0;
        public double AM200=0;
        public double AM100=0;
        public double  AM50=0;
        public double AM20=0;
        public double AM10=0;
        public double AM5=0;
        public double AM2=0;
        public double AM1=0;
        public double AM5cen = 0;
       
        

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
       // Connector conn = new Connector();

        private void frmImportCustomer_Load(object sender, EventArgs e)
        {
            TotAmount = 0;
            setvalue();
            label36.Text = user.userName;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            CheckUser();
            loadSign();
        }
        private void loadSign()
        {
            try
            {
                string StrSql = "Select CSH5000QTY, CSH5000AM, CSH2000QTY, CSH2000AM, CSH1000QTY, CSH1000AM, " +
                         " CSH500QTY, CSH500AM,  CSH200QTY, CSH200AM,  CSH100QTY, CSH100AM,  CSH50QTY, CSH50AM,  CSH20QTY, CSH20AM, " +
                         " CSH10QTY, CSH10AM, CSH5QTY, CSH5AM, CSH2QTY, CSH2AM, CSH1QTY, CSH1AM,  CSHC50QTY, CSHC50AM,Gross_Sale,Return_Sale,Dinomination  from tbl_Detamination where UserName='" + user.userName + "' and LoginDate ='" + GetDateTime(dateTimePicker1.Value) + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        txt5000.Text = dt.Rows[i]["CSH5000QTY"].ToString();
                        txt5000tot.Text = dt.Rows[i]["CSH5000AM"].ToString();
                        txt2000.Text = dt.Rows[i]["CSH2000QTY"].ToString();
                        txt2000tot.Text = dt.Rows[i]["CSH2000AM"].ToString();
                        txt1000.Text = dt.Rows[i]["CSH1000QTY"].ToString();
                        txt1000tot.Text = dt.Rows[i]["CSH1000AM"].ToString();
                        txt500.Text = dt.Rows[i]["CSH500QTY"].ToString();
                        txt500tot.Text = dt.Rows[i]["CSH500AM"].ToString();
                        txt200.Text = dt.Rows[i]["CSH200QTY"].ToString();
                        txt200tot.Text = dt.Rows[i]["CSH200AM"].ToString();
                        txt100.Text = dt.Rows[i]["CSH100QTY"].ToString();
                        txt100tot.Text = dt.Rows[i]["CSH100AM"].ToString();
                        txt50.Text = dt.Rows[i]["CSH50QTY"].ToString();
                        txt50tot.Text = dt.Rows[i]["CSH50AM"].ToString();
                        txt20.Text = dt.Rows[i]["CSH20QTY"].ToString();
                        txt20tot.Text = dt.Rows[i]["CSH20AM"].ToString();
                        txt10.Text = dt.Rows[i]["CSH10QTY"].ToString();
                        txt10tot.Text = dt.Rows[i]["CSH10AM"].ToString();
                        txt5.Text = dt.Rows[i]["CSH5QTY"].ToString();
                        txt5tot.Text = dt.Rows[i]["CSH5AM"].ToString();
                        txt2.Text = dt.Rows[i]["CSH2QTY"].ToString();
                        txt2tot.Text = dt.Rows[i]["CSH2AM"].ToString();
                        txt1.Text = dt.Rows[i]["CSH1QTY"].ToString();
                        txt1tot.Text = dt.Rows[i]["CSH1AM"].ToString();
                        txt5cen.Text = dt.Rows[i]["CSHC50QTY"].ToString();
                        txt50centot.Text = dt.Rows[i]["CSHC50AM"].ToString();
                        txttotsale.Text = double.Parse(dt.Rows[i]["Gross_Sale"].ToString()).ToString("0.00");
                        txtreturn.Text = double.Parse(dt.Rows[i]["Return_Sale"].ToString()).ToString("0.00");
                        txtnetam.Text = double.Parse(dt.Rows[i]["Dinomination"].ToString()).ToString("0.00");                     
                        
                    }
                }

                string StrSql1 = "Select  UserName, StartDate, Opening_Balance, flag, CasherAmount, SystemAmount, Diffrence,SignOff_Flag,CreditSale from  tbl_Dayly_OpBal where Flag='Y' and UserName='" + user.userName.ToString() + "' and Today ='" + GetDateTime(dateTimePicker1.Value) + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (dt1.Rows[i]["Opening_Balance"].ToString() == null || dt1.Rows[i]["Opening_Balance"].ToString() == string.Empty) 
                            txtopbal.Text = "0.00";
                        else
                            txtopbal.Text = double.Parse(dt1.Rows[i]["Opening_Balance"].ToString()).ToString("0.00");

                        if (dt1.Rows[i]["CasherAmount"].ToString() == null || dt1.Rows[i]["CasherAmount"].ToString() == string.Empty) 
                            txtcashinhand.Text = "0.00";
                        else
                            txtcashinhand.Text = double.Parse(dt1.Rows[i]["CasherAmount"].ToString()).ToString("0.00");
                        
                        if (dt1.Rows[i]["SystemAmount"].ToString() == null || dt1.Rows[i]["SystemAmount"].ToString() == string.Empty)
                            txtnetsale.Text = "0.00";
                        else
                            txtnetsale.Text = double.Parse(dt1.Rows[i]["SystemAmount"].ToString()).ToString("0.00");

                        if (dt1.Rows[i]["CreditSale"].ToString() == null || dt1.Rows[i]["CreditSale"].ToString() == string.Empty) 
                            txtbalance.Text = "0.00";
                        else
                            txtcreditsale.Text = double.Parse(dt1.Rows[i]["CreditSale"].ToString()).ToString("0.00");

                        if (dt1.Rows[i]["Diffrence"].ToString() == null || dt1.Rows[i]["Diffrence"].ToString() == string.Empty)
                            txtbalance.Text = "0.00";
                        else
                            txtbalance.Text = double.Parse(dt1.Rows[i]["Diffrence"].ToString()).ToString("0.00");


                        if (dt1.Rows[i]["SignOff_Flag"].ToString() == "Y")
                        {
                            button2.Enabled = false;
                            button5.Enabled = true ;
                        }
                        else if (dt1.Rows[i]["flag"].ToString() == "Y")
                        {
                            button6.Enabled = false;
                            button5.Enabled = false;
                        }
                        else
                        {
                            button6.Enabled = true ;
                            button5.Enabled = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void CheckUser()
        {
            try
            {
                String SSo = "select * from tbl_Dayly_OpBal where UserName='" + user.userName.ToString() + "' and Today ='" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and flag='Y'";
                SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
                DataTable dtSo = new DataTable();
                daSo.Fill(dtSo);
                if (dtSo.Rows.Count > 0)
                {                    
                    button6.Enabled = false;
                    button2.Enabled = true;
                    button7.Enabled = false ;
                    button5.Enabled = false;
                }
                else
                {                    
                    button6.Enabled = true;
                    button2.Enabled = false;
                    button7.Enabled = false;
                    button5.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
                return; 
            }
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            //===================================
            
            //====================================

            //try
            //{
            //   // Connector conn = new Connector();
            //   // conn.ImportCustomerInvoicesApply();
            //  // conn.ImportCustomer_Master();
            //    //conn.Insert_Customer();
            //    conn.ImportCustomer_MasterCSV();
            //    conn.Insert_CustomerCSV();
            //   MessageBox.Show("Customer Master file Successfully imported from Peachtree");
            //   this.Close();
            //}
            //catch { }
        }

        private void setvalue()
        {
            txttotsale.Text = "0.00";
            txtreturn.Text = "0.00";
            txtnetsale.Text = "0.00";
            txtnetam.Text = "0.00";
            txtopbal.Text = "0.00";
            txtnetsale.Text = "0.00";
            txtbalance.Text = "0.00";
            txtcreditsale.Text = "0.00";
            txtcashsale.Text = "0.00";
            txtadvance.Text = "0.00";

            txt5000tot.Text="0.00";
            txt2000tot.Text = "0.00";
            txt1000tot.Text = "0.00";
            txt500tot.Text = "0.00";
            txt200tot.Text = "0.00";
            txt100tot.Text = "0.00";
            txt50tot.Text = "0.00";
            txt20tot.Text = "0.00";
            txt10tot.Text = "0.00";
            txt5tot.Text = "0.00";
            txt2tot.Text = "0.00";
            txt1tot.Text = "0.00";
            txt50centot.Text = "0.00";

            txt5000.Text = "0";
            txt2000.Text = "0";
            txt1000.Text = "0";
            txt500.Text = "0";
            txt200.Text = "0";
            txt100.Text = "0";
            txt50.Text = "0";
            txt20.Text = "0";
            txt10.Text = "0";
            txt5.Text = "0";
            txt2.Text = "0";
            txt1.Text = "0";
            txt5cen.Text = "0";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
     
       

        private void txt5000_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtshowtime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
           // SaveCardGLAccount();
        }
        private void SaveCardGLAccount()
        {
            
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.YesNo);
                if (reply == DialogResult.No)
                {
                    return;
                }

                if (Convert.ToDouble(txtnetam.Text) <= 0  )
                {    
                    MessageBox.Show("Total Dinomination is Empty");                   
                    return;  
                }
                if (Convert.ToDouble(txtopbal.Text) < 0)
                {
                    MessageBox.Show("Opening Amount is Empty");
                    return;
                }
                //if (Convert.ToDouble(txtnetsale.Text) <= 0)
                //{
                //    MessageBox.Show("Total Amount is Empty");
                //    return;
                //}     
                String SSo = "select * from tbl_Daily_Sales_Summary where UserName='" + user.userName.ToString() + "' and DateFrom='" + dateTimePicker1.Value.ToString("MM/dd/yyyy")  + "'";
                SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
                DataTable dtSo = new DataTable();
                daSo.Fill(dtSo);
                if (dtSo.Rows.Count > 0 )
                {
                    MessageBox.Show("You Cannot Save the Another Record. Because this user have  allready Recorded Today ...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    myTrans.Rollback();
                    return;
                }
                try
                {
                    SqlConnection conn = new SqlConnection(ConnectionString);
                    DataTable dtable = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter("select * from tbl_Daily_Sales_Summary", conn);
                    adp.Fill(dtable);
                    for (int intGrid = 0; intGrid < dataGridView1.Rows.Count - 1; intGrid++)
                    {
                        if (dataGridView1.Rows[intGrid].Cells["Type"].Value.ToString() != null || dataGridView1.Rows[intGrid].Cells["Type"].Value.ToString() != string.Empty)
                        {
                            SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tbl_Daily_Sales_Summary (UserName,OpeningBalance,DateFrom,DateTo,LoginDate,Type,Amount,GrossAmount,ReturnAmount,NetAmount) values('" + user.userName + "','" + Convert.ToDouble(txtopbal.Text) + "','" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "','" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "', '" + dataGridView1.Rows[intGrid].Cells["Type"].Value.ToString() + "','" + double.Parse(dataGridView1.Rows[intGrid].Cells["Amount"].Value.ToString()) + "','" + Convert.ToDouble(txttotsale.Text) + "','" + Convert.ToDouble(txtreturn.Text) + "','" + Convert.ToDouble(txtnetsale.Text) + "')", conn);
                            conn.Open();
                            adp1.Fill(dtable);
                            conn.Close();
                        }
                        
                    }
                    MessageBox.Show("Process Compleated.. ", "Processing..", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button2.Enabled = false;
                    button6.Enabled = false;
                    button7.Enabled = true;
                    button7.Focus();
                }
                
                catch (Exception ex)
                {                       
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Process Not Compleated.. ", "Processing..", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    myTrans.Rollback();
                    return;
                }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value;
        }

        private void txt5000_TextChanged(object sender, EventArgs e)
        {
            int number2;
            if (int.TryParse(txt5000.Text, out number2))
            {
                AM5000 = 0;
                AM5000 = Convert.ToDouble(txt5000.Text.ToString()) * 5000;
                txt5000tot.Text = double.Parse(AM5000.ToString()).ToString("0.00");
            }
            else
            {
                MessageBox.Show("Enter Valid Count", "Validatting");
                txt5000.Text = "";
                return;
            }
        }

        private void txt2000_TextChanged(object sender, EventArgs e)
        {
            int number3;
            if (int.TryParse(txt2000.Text, out number3))
            {
                AM2000 = 0;
                AM2000 = Convert.ToDouble(txt2000.Text.ToString()) * 2000;
                txt2000tot.Text = double.Parse(AM2000.ToString()).ToString("0.00");
            }
            else
            {
                MessageBox.Show("Enter Valid Count", "Validatting");
                txt2000.Text = "";
                return;
            }
        }

        private void txt1000_TextChanged(object sender, EventArgs e)
        {
            int number4;
            if (int.TryParse(txt1000.Text, out number4))
            {
                AM1000 = 0;
                AM1000 = Convert.ToDouble(txt1000.Text.ToString()) * 1000;
                txt1000tot.Text = double.Parse(AM1000.ToString()).ToString("0.00");
            }
            else
            {
                MessageBox.Show("Enter Valid Count", "Validatting");
                txt1000.Text = "";
                return;
            }
        }

        private void txt500_TextChanged(object sender, EventArgs e)
        {
            int number5;
            if (int.TryParse(txt500.Text, out number5))
            {
                AM500 = 0;
                AM500 = Convert.ToDouble(txt500.Text.ToString()) * 500;
                txt500tot.Text = double.Parse(AM500.ToString()).ToString("0.00");
            }
            else
            {
                MessageBox.Show("Enter Valid Count", "Validatting");
                txt500.Text = "";
                return;
            }
        }

        private void txt200_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM200 = 0;
                AM200 = Convert.ToDouble(txt200.Text.ToString()) * 200;
                txt200tot.Text = double.Parse(AM200.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt100_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM100 = 0;
                AM100 = Convert.ToDouble(txt100.Text.ToString()) * 100;
                txt100tot.Text = double.Parse(AM100.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt50_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM50 = 0;
                AM50 = Convert.ToDouble(txt50.Text.ToString()) * 50;
                txt50tot.Text = double.Parse(AM50.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt20_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM20 = 0;
                AM20 = Convert.ToDouble(txt20.Text.ToString()) * 20;
                txt20tot.Text = double.Parse(AM20.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM10 = 0;
                AM10 = Convert.ToDouble(txt10.Text.ToString()) * 10;
                txt10tot.Text = double.Parse(AM10.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM5 = 0;
                AM5 = Convert.ToDouble(txt5.Text.ToString()) * 5;
                txt5tot.Text = double.Parse(AM5.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM2 = 0;
                AM2 = Convert.ToDouble(txt2.Text.ToString()) * 2;
                txt2tot.Text = double.Parse(AM2.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM1 = 0;
                AM1 = Convert.ToDouble(txt1.Text.ToString()) * 1;
                txt1tot.Text = double.Parse(AM1.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt5cen_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AM5cen = 0;
                AM5cen = Convert.ToDouble(txt5cen.Text.ToString()) * .5;
                txt50centot.Text = double.Parse(AM5cen.ToString()).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txt5000_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt2000.Focus();
            }
        }

        private void txt2000_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt1000.Focus();
            }
        }

        private void txt1000_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt500.Focus();
            }
        }

        private void txt500_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt200.Focus();
            }
        }

        private void txt200_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt100.Focus();
            }
        }

        private void txt100_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt50.Focus();
            }
        }

        private void txt50_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt20.Focus();
            }
        }

        private void txt20_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt10.Focus();
            }
        }

        private void txt10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt5.Focus();
            }
        }

        private void txt5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt2.Focus();
            }
        }

        private void txt2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt1.Focus();
            }
        }

        private void txt1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt5cen.Focus();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TotAmount = AM5000 + AM2000 + AM1000 + AM500 + AM200 + AM100 + AM50 + AM20 + AM10 + AM5 + AM2 + AM1 + AM5cen;
            txtnetam.Text = double.Parse(TotAmount.ToString()).ToString("0.00");   
        }

        private void button5_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.YesNo);

            if (reply == DialogResult.No )
            {
                return;
            }
            ClassDriiDown.IsInvSerch = false;
            DSDAILY.Clear();
            try
            {
                DSDAILY.Clear();
                //string StrSql = "SELECT * FROM tbl_Daily_Sales_Summary WHERE UserName='" + user.userName.ToString().Trim() + "' and DateFrom='" + Convert.ToDateTime(dateTimePicker1.Value.ToString("MM/dd/yyyy")) + "'";
                //SqlCommand cmd = new SqlCommand(StrSql);
                //SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(DSDAILY.DsDailySales);

                string StrSql1 = "SELECT * FROM tbl_Dayly_OpBal WHERE UserName='" + user.userName.ToString().Trim() + "' and [today]='" + GetDateTime(dateTimePicker1.Value) + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(DSDAILY.DsOpening);  

                InDirectPrint();
                
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

                frmPrintDailyReports printax = new frmPrintDailyReports(this);
                printax.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Connector objConnector = new Connector();
                if (!(objConnector.IsOpenPeachtree(user.LoginDate)))
                    return;

                DialogResult reply = MessageBox.Show("Are you sure, you want Process? ", "Information", MessageBoxButtons.YesNo);
                if (reply == DialogResult.No)
                {
                    return;
                }
                try
                {
                    Cursor = Cursors.WaitCursor;
                                       
                    conn.ExportSalesJournal_By_Period(dateTimePicker1.Value,dateTimePicker2.Value);
                    conn.Insert_Invoice();

                    conn.Export_Receipt_Journal(); //Receipt Import
                    conn.Insert_Receipt();//Insert Receipt

                    Cursor = Cursors.Default;
                
                    double NetAm =0;
                    double OBAL =0;
                    OBAL = Convert.ToDouble(txtopbal.Text);
                    NetAm= Convert.ToDouble(txtnetam.Text);
                    if (NetAm < 0)
                    {
                        MessageBox.Show("Total Dinomination is Empty");
                        return;  
                    }
                    if (OBAL < 0)
                    {
                        MessageBox.Show("Total Dinomination is Empty");
                        return;
                    }

                    double TotalSale = 0;
                    string ConnString = ConnectionString;
                    String S1 = "SELECT   PayMethod, SalesRepId, SUM(TotalPaidOnInvoices) AS totpaidam FROM tbl_Import_Receipt  WHERE     (SalesRepId <> '') and SalesRepId='" + user.userName + "' and [Rec_Date] >= '" + GetDateTime(dateTimePicker1.Value)  + "' and  [Rec_Date] <='" + GetDateTime(dateTimePicker1.Value)  + "' and PayMethod <> 'Cash' and PayMethod <> 'Advance' GROUP BY PayMethod, SalesRepId ORDER BY PayMethod, SalesRepId";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);                    
                    dataGridView1.Rows.Clear();
                    progressBar1.Value = 0;
                    progressBar1.Maximum = ds1.Tables[0].Rows.Count;
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                        {
                            dataGridView1.Rows.Add();
                            dataGridView1.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                            dataGridView1.Rows[i].Cells[1].Value = double.Parse(ds1.Tables[0].Rows[i].ItemArray[2].ToString()).ToString("0.00");
                            TotalSale = TotalSale + Convert.ToDouble(ds1.Tables[0].Rows[i].ItemArray[2].ToString());
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[0].Cells[0].Value = "MasterCard";
                        dataGridView1.Rows[0].Cells[1].Value = "0.00";
                    }
                    txttotsale.Text = double.Parse(TotalSale.ToString()).ToString("0.00");

                    double CashSale = 0;
                    String S5 = "SELECT   PayMethod, SalesRepId, SUM(TotalPaidOnInvoices) AS totpaidam FROM tbl_Import_Receipt  WHERE     (SalesRepId <> '') and SalesRepId='" + user.userName + "' and [Rec_Date] >= '" + GetDateTime(dateTimePicker1.Value) + "' and  [Rec_Date] <='" + GetDateTime(dateTimePicker1.Value) + "' and PayMethod='Cash' GROUP BY PayMethod, SalesRepId ORDER BY PayMethod, SalesRepId";
                    SqlCommand cmd5 = new SqlCommand(S5);
                    SqlDataAdapter da5 = new SqlDataAdapter(S5, ConnectionString);
                    DataSet ds5 = new DataSet();
                    da5.Fill(ds5);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = ds5.Tables[0].Rows.Count;
                    if (ds5.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds5.Tables[0].Rows.Count; i++)
                        {
                            CashSale = CashSale + Convert.ToDouble(ds5.Tables[0].Rows[i].ItemArray[2].ToString());
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
                    txtcashsale.Text = double.Parse(CashSale.ToString()).ToString("0.00");

                    double Advance = 0;
                    String S6 = "SELECT   PayMethod, SalesRepId, SUM(TotalPaidOnInvoices) AS totpaidam FROM tbl_Import_Receipt  WHERE     (SalesRepId <> '') and SalesRepId='" + user.userName + "' and [Rec_Date] >= '" + GetDateTime(dateTimePicker1.Value) + "' and  [Rec_Date] <='" + GetDateTime(dateTimePicker1.Value) + "' and PayMethod='Advance' GROUP BY PayMethod, SalesRepId ORDER BY PayMethod, SalesRepId";
                    SqlCommand cmd6 = new SqlCommand(S6);
                    SqlDataAdapter da6 = new SqlDataAdapter(S6, ConnectionString);
                    DataSet ds6 = new DataSet();
                    da6.Fill(ds6);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = ds6.Tables[0].Rows.Count;
                    if (ds6.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds6.Tables[0].Rows.Count; i++)
                        {
                            Advance = Advance + Convert.ToDouble(ds6.Tables[0].Rows[i].ItemArray[2].ToString());
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
                    txtadvance.Text = double.Parse(Advance.ToString()).ToString("0.00");

                    
                    double ReturnSale = 0;
                    double Rts=0;
                    //string ConnString = ConnectionString;
                    String S2 = "SELECT   SalesRepId, SUM(Amount) AS TotReturn FROM tbl_Import_invoice  WHERE  [Date] >= '" + GetDateTime(dateTimePicker1.Value) + "' and  [Date] <='" + GetDateTime(dateTimePicker1.Value) + "'  and  (SalesRepId <> '') and SalesRepId='" + user.userName + "' and CreditMemo='True' GROUP BY SalesRepId ";
                    SqlCommand cmd2 = new SqlCommand(S2);
                    SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = ds2.Tables[0].Rows.Count;
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                        {
                            Rts = 0;
                            if (ds2.Tables[0].Rows[i].ItemArray[1].ToString() == null || ds2.Tables[0].Rows[i].ItemArray[1].ToString() == string.Empty)
                                Rts = 0;
                            else 
                                Rts=Convert.ToDouble(ds2.Tables[0].Rows[i].ItemArray[1].ToString());
                            ReturnSale = ReturnSale + Rts;
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
                    txtreturn.Text = double.Parse(ReturnSale.ToString()).ToString("0.00");


                    double CreditSale = 0;
                    //string ConnString = ConnectionString;
                    //String S3 = "SELECT   SalesRepId, SUM(Amount) AS TotReturn FROM tbl_Import_invoice  WHERE  Date >= '" + Convert.ToDateTime(dateTimePicker1.Value.ToString("MM/dd/yyyy")) + "' and  Date <='" + Convert.ToDateTime(dateTimePicker2.Value.ToString("MM/dd/yyyy")) + "'  and  (SalesRepId <> '') and SalesRepId='" + user.userName + "' and CreditMemo='False' and Type='Credit' GROUP BY SalesRepId ";
                    //SqlCommand cmd3 = new SqlCommand(S3);
                    //SqlDataAdapter da3 = new SqlDataAdapter(S3, ConnectionString);
                    //DataSet ds3 = new DataSet();
                    //da3.Fill(ds3);
                    //progressBar1.Value = 0;
                    //progressBar1.Maximum = ds3.Tables[0].Rows.Count;
                    //for (int i = 0; i < ds3.Tables[0].Rows.Count; i++)
                    //{
                    //    CreditSale = CreditSale + Convert.ToDouble(ds3.Tables[0].Rows[i].ItemArray[1].ToString());
                    //    progressBar1.Value = progressBar1.Value + 1;
                    //}

                    double Crebal = 0;
                    double totcredit = 0;
                    double Recipaid = 0;
                    //String S4 = "SELECT  InvoiceNumber, Amount, Date,SalesRepId,SUM(RecPaid) AS RecPaid FROM View_Impinv_impRec WHERE  Date >= '" + Convert.ToDateTime(dateTimePicker1.Value.ToString("MM/dd/yyyy")) + "' and  Date <='" + Convert.ToDateTime(dateTimePicker2.Value.ToString("MM/dd/yyyy")) + "'  and  (SalesRepId <> '') and SalesRepId='" + user.userName + "' and Amount - RecPaid > 0  and CreditMemo='False' GROUP BY InvoiceNumber, Amount, Date,SalesRepId";
                    String S4 = "SELECT     InvoiceNumber, Amount, Date, SalesRepId, SUM(RecPaid) AS RecPaid,CreditMemo  FROM         View_Impinv_impRec  WHERE Date >= '" + GetDateTime(dateTimePicker1.Value) + "' and  Date <='" + GetDateTime(dateTimePicker1.Value)  + "' and SalesRepId='" + user.userName + "' and CreditMemo='False'  GROUP BY InvoiceNumber, Amount, Date, SalesRepId,CreditMemo ORDER BY InvoiceNumber";
                    //String S3 = "SELECT   SalesRepId, SUM(Amount) AS TotReturn FROM tbl_Import_invoice  WHERE  Date >= '" + Convert.ToDateTime(dateTimePicker1.Value.ToString("MM/dd/yyyy")) + "' and  Date <='" + Convert.ToDateTime(dateTimePicker2.Value.ToString("MM/dd/yyyy")) + "'  and  (SalesRepId <> '') and SalesRepId='" + user.userName + "' and CreditMemo='False' and Type='Credit' GROUP BY SalesRepId ";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, ConnectionString);
                    DataSet ds4 = new DataSet();
                    da4.Fill(ds4);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = ds4.Tables[0].Rows.Count;
                    if (ds4.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds4.Tables[0].Rows.Count; i++)
                        {
                            Recipaid = 0;
                            if (ds4.Tables[0].Rows[i].ItemArray[4].ToString() == null || ds4.Tables[0].Rows[i].ItemArray[4].ToString() == string.Empty)
                                Recipaid = 0;
                            else 
                                Recipaid=Convert.ToDouble((ds4.Tables[0].Rows[i].ItemArray[4].ToString()));
                            
                            Crebal = Crebal + (Convert.ToDouble(ds4.Tables[0].Rows[i].ItemArray[1].ToString()) - Recipaid);
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
                    totcredit = Crebal + CreditSale;
                    txtcreditsale.Text = double.Parse(totcredit.ToString()).ToString("0.00");

                    double netsale = 0;
                    netsale = TotalSale - (ReturnSale * -1);
                    txtnetsale.Text = double.Parse(netsale.ToString()).ToString("0.00");
                    double Balance = 0;
                   // Balance   = Convert.ToDouble(txtnetsale.Text) - Convert.ToDouble(txtcasheram.Text);
                    txtbalance.Text = double.Parse(Balance.ToString()).ToString("0.00");

                    SaveCardGLAccount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }              

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private void txtcasheram_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void txtcasheram_Leave(object sender, EventArgs e)
        {
            
            button2.Enabled = true;
        }

        private void txtcasheram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //if (Convert.ToDouble(txtcasheram.Text) > 0)
                //{
                //    txtcasheram.Text = double.Parse(txtcasheram.Text).ToString("0.00");
                //    button2.Enabled = true;
                //}
                //else
                //{
                //    txtcasheram.Text = double.Parse(txtcasheram.Text).ToString("0.00");
                //    button2.Enabled = false;
                //}
            }
        }
   
        public static string GetDateTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetTime(DateTime Dttime)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(Dttime);
                string Dformat = "MM/dd/yyyy hh:mm";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            try
            {                

                DialogResult reply = MessageBox.Show("Are you sure, you want to Sign In This System? ", "Information", MessageBoxButtons.YesNo);
                if (reply == DialogResult.No)
                {
                    return;
                }
                
                if (Convert.ToDouble(txtopbal.Text) <= 0)
                {
                    MessageBox.Show("Opening Amount is Empty. You Cannot Sign in...", "Wanning");
                    return;
                }

                String SSo = "select * from tbl_Dayly_OpBal where UserName='" + user.userName.ToString() + "' and [Today]='" +  GetDateTime(dateTimePicker1.Value)  + "'";
                SqlDataAdapter daSo = new SqlDataAdapter(SSo, ConnectionString);
                DataTable dtSo = new DataTable();
                daSo.Fill(dtSo);
                if (dtSo.Rows.Count > 0)
                {
                    MessageBox.Show("You Cannot Save the Another Record. Because this user have  allready Recorded Today ...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    myTrans.Rollback();
                    return;
                }
                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("select * from  tbl_Dayly_OpBal", conn);
                adp.Fill(dtable);

                SqlDataAdapter adp1 = new SqlDataAdapter("Insert into  tbl_Dayly_OpBal (UserName,StartDate,Opening_Balance,Today) values('" + user.userName + "','" + GetTime(dateTimePicker1.Value)  + "','" + Convert.ToDouble(txtopbal.Text) + "','" + GetDateTime(dateTimePicker1.Value) + "')", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();
                CheckUser();     
                button6.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
                return;
            }
        }

        private void txtopbal_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtopbal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt5000.Focus();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();
            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Sign Off This System? ", "Information", MessageBoxButtons.YesNo);
                if (reply == DialogResult.No )
                {
                    return;
                }
                if (Convert.ToDouble(txtnetam.Text) <= 0)
                {
                    MessageBox.Show("Total Dinomination is Empty.You Cannot Sign Off...", "Wanning");
                    return;
                }
                if (Convert.ToDouble(txtopbal.Text) <= 0)
                {
                    MessageBox.Show("Opening Amount is Empty. You Cannot Sign Off...", "Wanning");
                    return;
                }
                //if (Convert.ToDouble(txtnetsale.Text) <= 0)
                //{
                //    MessageBox.Show("Total Amount is Empty. You Cannot Sign Off...", "Wanning");
                //    return;
                //}
                

                SqlConnection conn = new SqlConnection(ConnectionString);
                DataTable dtable = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("select * from  tbl_Dayly_OpBal", conn);
                adp.Fill(dtable);

                SqlDataAdapter adp1 = new SqlDataAdapter("Update  tbl_Dayly_OpBal  set Advance='" + Convert.ToDouble(txtadvance.Text) + "', CashSale='" + Convert.ToDouble(txtcashsale.Text) + "' ,SignOff_Flag='Y',CasherAmount='" + Convert.ToDouble(txtcashinhand.Text) + "',SystemAmount='" + Convert.ToDouble(txtnetsale.Text) + " ',Diffrence='" + Convert.ToDouble(txtbalance.Text) + "',SignOff='" + GetTime(dateTimePicker1.Value) + "',CreditSale='" + Convert.ToDouble(txtcreditsale.Text) + "' where UserName= '" + user.userName + "' and [Today]='" + GetDateTime(dateTimePicker1.Value) + "' and flag='Y'", conn);
                conn.Open();
                adp1.Fill(dtable);
                conn.Close();

                SqlDataAdapter adp3 = new SqlDataAdapter("Insert into tbl_Detamination " +
                        "(UserName,LoginDate,CSH5000,CSH5000QTY,CSH5000AM,CSH2000,CSH2000QTY,CSH2000AM,CSH1000,CSH1000QTY,CSH1000AM,CSH500,CSH500QTY,CSH500AM,CSH200,CSH200QTY,CSH200AM,CSH100,CSH100QTY,CSH100AM,CSH50,CSH50QTY,CSH50AM,CSH20,CSH20QTY,CSH20AM,CSH10,CSH10QTY,CSH10AM,CSH5,CSH5QTY,CSH5AM,CSH2,CSH2QTY,CSH2AM,CSH1,CSH1QTY,CSH1AM,CSHC50,CSHC50QTY,CSHC50AM,Gross_Sale,Return_Sale,Net_Sale,Dinomination) " +
                        " values('" + user.userName + "','" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "'" +
                        ",'5000','" + Convert.ToInt64(txt5000.Text) + "','" + Convert.ToDouble(txt5000tot.Text) + "'" +
                        ",'2000','" + Convert.ToInt64(txt2000.Text) + "','" + Convert.ToDouble(txt2000tot.Text) + "'" +
                        ",'1000','" + Convert.ToInt64(txt1000.Text) + "','" + Convert.ToDouble(txt1000tot.Text) + "'" +
                        ",'500','" + Convert.ToInt64(txt500.Text) + "','" + Convert.ToDouble(txt500tot.Text) + "'" +
                        ",'200','" + Convert.ToInt64(txt200.Text) + "','" + Convert.ToDouble(txt200tot.Text) + "'" +
                        ",'100','" + Convert.ToInt64(txt100.Text) + "','" + Convert.ToDouble(txt100tot.Text) + "'" +
                        ",'50','" + Convert.ToInt64(txt50.Text) + "','" + Convert.ToDouble(txt50tot.Text) + "'" +
                        ",'20','" + Convert.ToInt64(txt20.Text) + "','" + Convert.ToDouble(txt20tot.Text) + "'" +
                        ",'10','" + Convert.ToInt64(txt10.Text) + "','" + Convert.ToDouble(txt10tot.Text) + "'" +
                        ",'5','" + Convert.ToInt64(txt5.Text) + "','" + Convert.ToDouble(txt5tot.Text) + "'" +
                        ",'2','" + Convert.ToInt64(txt2.Text) + "','" + Convert.ToDouble(txt2tot.Text) + "'" +
                        ",'1','" + Convert.ToInt64(txt1.Text) + "','" + Convert.ToDouble(txt1tot.Text) + "'" +
                        ",'.50','" + Convert.ToInt64(txt5cen.Text) + "','" + Convert.ToDouble(txt50centot.Text) + "'" +
                        ",'" + Convert.ToDouble(txttotsale.Text) + "','" + Convert.ToDouble(txtreturn.Text) + "','" + Convert.ToDouble(txtnetsale.Text) + "','" + Convert.ToDouble(txtnetam.Text) + "')", conn);
                conn.Open();
                adp3.Fill(dtable);
                conn.Close();
                button7.Enabled = false;
                button5.Enabled = true ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
                return;
            }
        }
      

        
    }
}