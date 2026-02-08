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

namespace UserAutherization
{
    public partial class FrmPurchaseOrderVarification : Form
    {
        public FrmPurchaseOrderVarification()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;

        public DSPurchaseOrderList POList = new DSPurchaseOrderList();

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

        private void FrmPurchaseOrderVarification_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Rows.Add("All");
            dt.Rows.Add("Save");
            dt.Rows.Add("Process");
            
            cmbType.Text = "All";
            cmbType.DataSource = dt;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
                try
                {
                    POList.Clear();

                string status="";
                if(cmbType.Text=="All")
                {
                    status = "";
                }
                else if(cmbType.Text=="Save")
                {
                    status = "1";
                }
                else if (cmbType.Text == "Process")
                {
                    status = "0";
                }
                    String S3 = "Select * from tblPurchaseOrder  where Date  >= '" + dtpFromDate.Text.ToString().Trim() + "' and  Date <='" + dtpToDate.Text.ToString().Trim() + "' and Convert(varchar,IsActive) like  '%" + status + "%'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(POList, "PurchaseOrder");

                    String S31 = "Select * from tblCompanyInformation";
                    SqlCommand cmd31 = new SqlCommand(S31);
                    SqlConnection con31 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
                    da31.Fill(POList, "DtCompaniInfo");

                    frmPOrVariicationPrint povprint = new frmPOrVariicationPrint(this);
                    povprint.Show();
                }
                catch { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}