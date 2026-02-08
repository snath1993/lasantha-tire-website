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
    public partial class frmGRNList : Form
    {
        public frmGRNList()
        {
            InitializeComponent();
            setConnectionString();
        }
        public DSGRNList GRNData = new DSGRNList();

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

        private void frmGRNList_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Rows.Add("All");
            dt.Rows.Add("Save");
            dt.Rows.Add("Process");

            cmbType.Text = "All";
            cmbType.DataSource = dt;
            cmbType2.Text = "All";
            cmbType2.DataSource = dt;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnView_Click(object sender, EventArgs e)
        {

            string status = "";
            string status2 = "";
            if (cmbType.Text == "All")
            {
                status = "";
            }
            else if (cmbType.Text == "Save")
            {
                status = "1";
            }
            else if (cmbType.Text == "Process")
            {
                status = "0";
            }

            if (cmbType2.Text == "All")
            {
                status2 = "";
            }
            else if (cmbType2.Text == "Save")
            {
                status2 = "1";
            }
            else if (cmbType2.Text == "Process")
            {
                status2 = "0";
            }

            try
                {
                    GRNData.Clear();
                    String S31 = "Select VendorID,VendorName,VContact,VAddress1,VAddress2 from tblVendorMaster";
                    SqlCommand cmd31 = new SqlCommand(S31);
                    SqlConnection con31 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da31 = new SqlDataAdapter(S31, con31);
                    da31.Fill(GRNData, "DTVendor");

             
                 if ((chbDirectGRN.Checked == false && chbGRN.Checked == false) || (chbDirectGRN.Checked == true && chbGRN.Checked == true))
                {
                    String S3 = "Select * from tblGRNTran  where GRNDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  GRNDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status + "%'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(GRNData, "DT_GRNLIst");

                    String S33 = "Select GRNNos as GRN_NO,CustomerID as VendorID,'',InvoiceDate as GRNDate,APAccount,NoofDistributions as NoOfDis,DistributionNo as DistributionNo,ItemID,Description,Qty as OrderQty,Qty as ReceiveQty,GLAcount as GlAccount,UnitPrice,Amount,UOM,NetTotal as TotalAmount,FreeQty from tblDirectSupplierInvoices  where InvoiceDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status2 + "%'";
                    SqlCommand cmd33 = new SqlCommand(S33);
                    SqlConnection con33 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                    da33.Fill(GRNData, "DT_GRNLIst");
                }
               else if (chbGRN.Checked == true)
                {
                    String S3 = "Select * from tblGRNTran  where GRNDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  GRNDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status + "%'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(GRNData, "DT_GRNLIst");
                }
                else if (chbDirectGRN.Checked == true)
                {
                    String S33 = "Select GRNNos as GRN_NO,CustomerID as VendorID,'',InvoiceDate as GRNDate,APAccount,NoofDistributions as NoOfDis,DistributionNo as DistributionNo,ItemID,Description,Qty as OrderQty,Qty as ReceiveQty,GLAcount as GlAccount,UnitPrice,Amount,UOM,NetTotal as TotalAmount,FreeQty from tblDirectSupplierInvoices  where InvoiceDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status2 + "%'";
                    SqlCommand cmd33 = new SqlCommand(S33);
                    SqlConnection con33 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                    da33.Fill(GRNData, "DT_GRNLIst");
                }

            
                String S312 = "Select * from tblCompanyInformation";
                    SqlCommand cmd312 = new SqlCommand(S312);
                    SqlConnection con312 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da312 = new SqlDataAdapter(S312, con312);
                    da312.Fill(GRNData, "DtCompaniInfo");

                    frmGRNListPrint grnlprint = new frmGRNListPrint(this);
                    grnlprint.Show();
                }
                catch { }
               
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void chbGRN_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}