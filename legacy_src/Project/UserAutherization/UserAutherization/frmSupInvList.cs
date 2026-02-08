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
    public partial class frmSupInvList : Form
    {
        public frmSupInvList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public DsSupplierInvoice SupInvList = new DsSupplierInvoice();
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

        private void frmSupInvList_Load(object sender, EventArgs e)
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


            GetVendorDataset();
        }


        public void GetVendorDataset()
        {
            try
            {
                string StrSql = "SELECT VendorID,VendorName FROM tblVendorMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbCustomer.DataSource = dt;
                cmbCustomer.DisplayMember = "VendorID";
                cmbCustomer.ValueMember = "VendorID";
                cmbCustomer.DisplayLayout.Bands[0].Columns["VendorID"].Width = 150;
                cmbCustomer.DisplayLayout.Bands[0].Columns["VendorName"].Width = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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


                String S3V = "Select * from tblVendorMaster";// where GRN_NO = '" + txtGRn_NO.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd3V = new SqlCommand(S3V);
                SqlConnection con3V = new SqlConnection(ConnectionString);
                SqlDataAdapter da3V = new SqlDataAdapter(S3V, con3V);
                da3V.Fill(SupInvList, "DTVendor");

                SupInvList.DtSupInv.Clear();

                if ((chbGRN.Checked == true && chbDirectGRN.Checked == true)|| (chbGRN.Checked == false && chbDirectGRN.Checked == false))
                {
                    String S3 = "Select * from tblSupplierInvoices  where InvoiceRecivedDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceRecivedDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status + "%' and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(SupInvList, "DtSupInv");


                    String S33 = "Select * from tblDirectSupplierInvoices  where InvoiceRecivedDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceRecivedDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'  and Convert(varchar,IsActive) like  '%" + status2 + "%' and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'";
                    SqlCommand cmd33 = new SqlCommand(S33);
                    SqlConnection con33 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                    da33.Fill(SupInvList, "DtSupInv");
                }

                else if(chbGRN.Checked ==true)
                {
                    String S3 = "Select * from tblSupplierInvoices  where InvoiceRecivedDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceRecivedDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status + "%' and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'";
                    SqlCommand cmd3 = new SqlCommand(S3);
                    SqlConnection con3 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                    da3.Fill(SupInvList, "DtSupInv");
                }

                else
                {

                    String S33 = "Select * from tblDirectSupplierInvoices  where InvoiceRecivedDate  >= '" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and  InvoiceRecivedDate <='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "' and Convert(varchar,IsActive) like  '%" + status2 + "%' and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'";
                    SqlCommand cmd33 = new SqlCommand(S33);
                    SqlConnection con33 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da33 = new SqlDataAdapter(S33, con33);
                    da33.Fill(SupInvList, "DtSupInv");
                }
                    

                    String S312 = "Select * from tblCompanyInformation";
                    SqlCommand cmd312 = new SqlCommand(S312);
                    SqlConnection con312 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da312 = new SqlDataAdapter(S312, con312);
                    da312.Fill(SupInvList, "DtCompaniInfo");


                    frmSupplyInvoiceListPrint supprint = new frmSupplyInvoiceListPrint(this);
                    supprint.Show();

                  
                }
                catch { }
             
        }

        private void chkcusAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkcusAll.Checked)
            {
                cmbCustomer.Text = "";
                cmbCustomer.Enabled = false;
            }
            else
            {
                cmbCustomer.Enabled = true;
            }
        }
    }
}