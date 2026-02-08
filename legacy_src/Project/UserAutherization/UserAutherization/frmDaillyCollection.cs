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
    public partial class frmDaillyCollection : Form
    {
        public frmDaillyCollection()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        Connector conn = new Connector();
        public DSDailySalesSummary DSDAILY = new DSDailySalesSummary();
        clsCommon objclsCommon = new clsCommon();

        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;           
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
        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.YesNo);

            if (reply == DialogResult.No)
            {
                return;
            }
            ClassDriiDown.IsInvSerch = false;
            DSDAILY.Clear();
            try
            {
                DSDAILY.Clear();
                string StrSql = "SELECT * FROM tbl_Daily_Sales_Summary WHERE UserName='" + cmbUser.Text + "' and [DateFrom]='" + GetDateTime(dateTimePicker1.Value)  + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DSDAILY.DsDailySales);

                string StrSql1 = "SELECT * FROM tbl_Dayly_OpBal WHERE UserName='" + cmbUser.Text + "' and [today]='" + GetDateTime(dateTimePicker1.Value) + "'";
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

                frmPrintTotalDailyCollection printax = new frmPrintTotalDailyCollection(this);
                printax.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmDaillyCollection_Load(object sender, EventArgs e)
        {
            String S = "Select UserID from Login";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);            
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbUser.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString());
            }

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }
    }
}