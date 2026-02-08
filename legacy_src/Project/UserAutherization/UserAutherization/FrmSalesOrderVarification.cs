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
    public partial class FrmSalesOrderVarification : Form
    {
        public FrmSalesOrderVarification()
        {
            InitializeComponent();
            setConnectionString();
        }
        public DSSalesOrderLIst DSDispatch = new DSSalesOrderLIst();
       //public DSDeliveryNotes DSDispatch = new DSDeliveryNotes();
        public static string ConnectionString;


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
            catch { }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                DSDispatch.CustomerMaster2.Clear();
                String S1 = "Select * from tblCustomerMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSDispatch, "CustomerMaster2");
            }
            catch { }

            if (rbtDateRange.Checked == true)
            {
                try
                {
                    DSDispatch.DTSalesOrderList.Clear();
                    String S3 = "Select * from tblSalesOrderTemp  where Date  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  Date <='" + dtpToDate.Text.ToString().Trim() + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(DSDispatch, "DTSalesOrderList");

                    frmSalesVarificationPrint svp = new frmSalesVarificationPrint(this);
                    svp.Show();
                }
                catch { }
              //  rbtDateRange

            }

             if (rbtAll.Checked == true)
            {
                try
                {
                    DSDispatch.DTSalesOrderList.Clear();
                    String S3 = "Select * from tblSalesOrderTemp";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(DSDispatch, "DTSalesOrderList");

                    frmSalesVarificationPrint svp = new frmSalesVarificationPrint(this);
                    svp.Show();
                }
                catch { }

            
            }
        }

        private void FrmSalesOrderVarification_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}