using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Collections;
using System.Threading;
namespace UserAutherization
{
    public partial class frmtempPost : Form
    {
        public static string ConnectionString;
        Connector conn = new Connector();
        clsCommon objclsCommon = new clsCommon();
        public string StrSql;
        public frmtempPost()
        {

            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            string SONo = "";
            myConnection.Open();
            myTrans = myConnection.BeginTransaction();

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
            SqlCommand sqcom2= new SqlCommand(StrSql, myConnection, myTrans);
            sqcom2.CommandType = CommandType.Text;
            sqcom2.ExecuteNonQuery();
            
            StrSql = "UPDATE tblSalesOrderTemp set IsfullDispatch ='False' where  SalesOrderNo='" + SONo.ToString().Trim() + "'";
            SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
            command1.CommandType = CommandType.Text;
            command1.ExecuteNonQuery();

            myTrans.Commit();
            

        }
    }
        
}
