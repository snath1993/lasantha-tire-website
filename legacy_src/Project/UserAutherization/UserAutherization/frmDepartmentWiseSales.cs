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
using System.Xml;

namespace UserAutherization
{
    public partial class frmDepartmentWiseSales : Form
    {
        public frmDepartmentWiseSales()
        {
            InitializeComponent();
            setConnectionString();
        }
        public DataSet dsledcode;
        public static string ConnectionString;
        Connector conn = new Connector();
        public DsDipWiseSales DsDipDaily = new DsDipWiseSales();
        public DsVapPay DsVatpay = new DsVapPay();
        clsCommon objclsCommon = new clsCommon();
        SqlConnection myConnection = new SqlConnection(ConnectionString);
        SqlTransaction myTrans;
        public int Reptype = 0;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.YesNo);

            if (reply == DialogResult.No)
            {
                return;
            }
                      
            DsDipDaily.Clear();
            DsVatpay.Clear();
            SetVatNbtControl();

            if (optdep.Checked == true) Reptype = 0;         
            if (optdate.Checked == true) Reptype = 1;            
            if (rdInv.Checked == true) Reptype = 2;           
            if (rdodipsum.Checked == true) Reptype = 3;        
            if (optind.Checked == true) Reptype = 4;       
            if (rdodetail.Checked == true) Reptype = 5;
            if (rdoVat.Checked == true)
            {
                if (cmbtype.Text == "Cash")
                {
                    Reptype = 6;
                }
                else
                {
                    Reptype = 7;
                }
            }
            if (rdovatclamed.Checked == true) Reptype = 8;           
            if (radioButton1.Checked == true) Reptype = 9;
            if (rdosale.Checked == true)  Reptype=10;
            if (optind.Checked == true)
            {
                 string StrSql = "SELECT * FROM dbo.View_tblimpInvoice_Chacc WHERE  ARAccountId ='" + cmbAR.Value  + "' and [date] >='" + dtfrom.Value.ToString("MM/dd/yyyy") + "' and [date] <= '" + dtto.Value.ToString("MM/dd/yyyy") + "' and  (AccountDescription not like '%VAT%' and AccountDescription not like  '%NBT%') ";
                 SqlCommand cmd = new SqlCommand(StrSql);
                 SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                 DataTable dt = new DataTable();
                 da.Fill(DsDipDaily.DtDipWise);

                 string StrSql1 = "SELECT * FROM dbo.tblChartofAcounts WHERE AcountID ='" + cmbAR.Value.ToString() + "'  and   (AccountDescription not like '%VAT%' and AccountDescription not like  '%NBT%') ";
                 SqlCommand cmd1 = new SqlCommand(StrSql1);
                 SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                 DataTable dt1 = new DataTable();
                 da1.Fill(DsDipDaily.ChartofAcc);
                 InDirectPrint();
            }
            else if (rdoVat.Checked == true)
            {
                string StrSql = "SELECT * FROM dbo.tbl_Import_invoice WHERE   type='" + cmbtype.Text.ToString()  + "' and [date] >='" + dtfrom.Value.ToString("MM/dd/yyyy") + "' and [date] <= '" + dtto.Value.ToString("MM/dd/yyyy") + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsDipDaily.DtDipWise);

                string StrSql1 = "SELECT * FROM dbo.tblCustomerMaster";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(DsDipDaily.DsCustomer);
                InDirectPrint();
            }
            else if (rdovatclamed.Checked == true)
            {                
                conn.ExportEnterBillsperiod(dtfrom.Value, dtto.Value);
                conn.Insert_SuplierInvoice(cmbvatcon.Value.ToString(), cmbnbtcon.Value.ToString());
                string StrSql = "SELECT * FROM dbo.View_VatClamed where left(Invoice_Number,3)='SUP' and amount <> 0 ";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsVatpay.DtEnterbill);
                InDirectPrint();
            }
            else if (radioButton1.Checked == true)
            {
                conn.ExportEnterBillsperiod(dtfrom.Value, dtto.Value);
                conn.Insert_SuplierInvoice(cmbvatcon.Value.ToString(), cmbnbtcon.Value.ToString());
                String S = "SELECT Invoice_Number FROM tbl_temp_Supllierinvoice where VAT='True' and  (Invoice_Number NOT LIKE '%SUP%')";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {   
                    String S1 = "update tbl_temp_Supllierinvoice SET VAT = 'True' WHERE Invoice_Number = '" +  dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt2 = new DataTable();
                    da1.Fill(dt2);
                }
                string StrSql = "SELECT * FROM dbo.View_VatClamed where left(Invoice_Number,3) <> 'SUP' and VAT='True' and amount <> 0";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da2.Fill(DsVatpay.DtEnterbill);
                InDirectPrint();
            }
            else if (rdosale.Checked == true)
            {
                string StrSql = "SELECT * FROM dbo.view_whse_wise_qty ";
                SqlCommand cmd2 = new SqlCommand(StrSql);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                da2.Fill(DsVatpay.DTqty);
                InDirectPrint();
            }
            else
            {
                string StrSql = "SELECT * FROM dbo.View_tblimpInvoice_Chacc WHERE  [date] >='" + dtfrom.Value.ToString("MM/dd/yyyy") + "' and [date] <= '" + dtto.Value.ToString("MM/dd/yyyy") + "' and  (AccountDescription not like '%VAT%' and AccountDescription not like  '%NBT%') ";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsDipDaily.DtDipWise);

                string StrSql1 = "SELECT * FROM dbo.tblChartofAcounts WHERE   (AccountDescription not like '%VAT%' and AccountDescription not like  '%NBT%') ";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(DsDipDaily.ChartofAcc);
                InDirectPrint();
            }
            
           
            
        }
        private void InDirectPrint()
        {
            try
            {
                frmListDepWise printax = new frmListDepWise(this);
                printax.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Import Date from Sage?", "Print", MessageBoxButtons.YesNo);

            if (reply == DialogResult.No)
            {
                return;
            }
            conn.ExportSalesJournal_By_Period(dtfrom.Value, dtto.Value);
            conn.Insert_Invoice_Details();
            MessageBox.Show("Data Successfuly Imported.");
        }
        private void loadglcode()
        {
            try
            {                             

                dsledcode = new DataSet();                
                try
                {
                    dsledcode.Clear();
                    string StrSql = "Select AcountID,AccountDescription from tblChartofAcounts";
                    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                    dAdapt.Fill(dsledcode, "DtWarehouse");
                    cmbAR.DataSource = dsledcode.Tables["DtWarehouse"];
                    cmbAR.DisplayMember = "AccountDescription";
                    cmbAR.ValueMember = "AcountID";

                    dsledcode.Clear();
                    string StrSql1 = "Select AcountID,AccountDescription from tblChartofAcounts";
                    SqlDataAdapter dAdapt1 = new SqlDataAdapter(StrSql1, ConnectionString);
                    dAdapt1.Fill(dsledcode, "DtWarehouse");
                    cmbvatcon.DataSource = dsledcode.Tables["DtWarehouse"];
                    cmbvatcon.DisplayMember = "AccountDescription";
                    cmbvatcon.ValueMember = "AcountID";

                    dsledcode.Clear();
                    string StrSql2 = "Select AcountID,AccountDescription from tblChartofAcounts";
                    SqlDataAdapter dAdapt2 = new SqlDataAdapter(StrSql2, ConnectionString);
                    dAdapt2.Fill(dsledcode, "DtWarehouse");
                    cmbnbtcon.DataSource = dsledcode.Tables["DtWarehouse"];
                    cmbnbtcon.DisplayMember = "AccountDescription";
                    cmbnbtcon.ValueMember = "AcountID";
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                return;
            }

        }
        private void frmDepartmentWiseSales_Load(object sender, EventArgs e)
        {
            loadglcode();
            loaddefault();
            dtfrom.Value = DateTime.Now;
            dtto.Value = DateTime.Now;
        }

        private void SetVatNbtControl()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            string sqlStatement = "DELETE FROM tbl_VatNbtControlDefaults";
            connection.Open();
            SqlCommand cmd = new SqlCommand(sqlStatement, connection);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            connection.Close();

            SqlConnection conn = new SqlConnection(ConnectionString);
            DataTable dtable = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("select * from tbl_VatNbtControlDefaults", conn);
            adp.Fill(dtable);
            SqlDataAdapter adp1 = new SqlDataAdapter("Insert into tbl_VatNbtControlDefaults (VatControl,NbtControl) values('" + cmbvatcon.Value + "','" + cmbnbtcon.Value  + "')", conn);
            conn.Open();
            adp1.Fill(dtable);
            conn.Close();                
        }
        private void loaddefault()
        {
            string StrSql = "Select VatControl,NbtControl from tbl_VatNbtControlDefaults ";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbvatcon.Value = dt.Rows[i]["VatControl"].ToString();
                    cmbnbtcon.Value = dt.Rows[i]["NbtControl"].ToString();
                }
            }
        }

        private void rdovatclamed_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
