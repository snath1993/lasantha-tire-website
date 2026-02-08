using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Timers;;

namespace UserAutherization
{
    public partial class FrmUserwiseSummery : Form
    {
        public FrmUserwiseSummery()
        {
            InitializeComponent();
            setConnectionString();
        }
        public static string ConnectionString;
        Connector conn = new Connector();

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


        private void FrmUserwiseSummery_Load(object sender, EventArgs e)
        {

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
                string StrSql = "SELECT * FROM tbl_Daily_Sales_Summary WHERE UserName='" + cmbUser.Text + "' and [DateFrom]='" + GetDateTime(dateTimePicker1.Value) + "'";
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
    }
}
