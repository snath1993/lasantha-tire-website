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
    public partial class frmInvoiceWiseSales : Form
    {
        public DSMainReports ds = new DSMainReports();
        public static string ConnectionString;

        public frmInvoiceWiseSales()
        {
            setConnectionString();
            InitializeComponent();
        }

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
       

        private void chkDateFrom_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void chkDateTo_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void frmInvoiceWiseSales_Load(object sender, EventArgs e)
        {
            chkDateFrom.Checked = true;
            chkDateTo.Checked = true;
            chkInvFrom.Checked = true;
            chkInvTo.Checked = true;

            try
            {
                string S = "Select distinct InvoiceNo from tblSalesInvoices order by InvoiceNo";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbInvFrom.Items.Clear();
                    cmbInvTo.Items.Clear();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cmbInvFrom.Items.Add(dt.Rows[i].ItemArray[0].ToString().Trim());
                        cmbInvTo.Items.Add(dt.Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbInvFrom.Enabled == true && cmbInvTo.Enabled == true)
                {
                    ds.Clear();
                    string S = "Select * from tblSalesInvoices where InvoiceNo between '" + cmbInvFrom.Text.ToString().Trim() + "' and '" + cmbInvTo.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_locationwise_sales");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_locationwise_sales.Rows.Count > 0)
                    {
                        frmViewerInvoiceWiseSales InvSales = new frmViewerInvoiceWiseSales(this);
                        InvSales.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else if (cmbInvFrom.Enabled == true && cmbInvTo.Enabled == false)
                {
                    ds.Clear();
                    string S = "Select * from tblSalesInvoices where InvoiceNo >= '" + cmbInvFrom.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_locationwise_sales");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_locationwise_sales.Rows.Count > 0)
                    {
                        frmViewerInvoiceWiseSales locationSales = new frmViewerInvoiceWiseSales(this);
                        locationSales.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else if (cmbInvFrom.Enabled == false && cmbInvTo.Enabled == true)
                {
                    ds.Clear();
                    string S = "Select * from tblSalesInvoices where InvoiceNo <= '" + cmbInvTo.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_locationwise_sales");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_locationwise_sales.Rows.Count > 0)
                    {
                        frmViewerInvoiceWiseSales InvSales = new frmViewerInvoiceWiseSales(this);
                        InvSales.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else
                {
                    ds.Clear();
                    string S = "Select * from tblSalesInvoices";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_locationwise_sales");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_locationwise_sales.Rows.Count > 0)
                    {
                        frmViewerInvoiceWiseSales InvSales = new frmViewerInvoiceWiseSales(this);
                        InvSales.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
            }
            catch { }
        }

        private void chkInvFrom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkInvFrom.Checked == true)
                {
                    cmbInvFrom.Enabled = true;
                }
                else if (chkInvFrom.Checked == false)
                {
                    cmbInvFrom.Enabled = false;
                }
            }
            catch { }
        }

        private void chkInvTo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkInvTo.Checked == true)
                {
                    cmbInvTo.Enabled = true;
                }
                else if (chkInvTo.Checked == false)
                {
                    cmbInvTo.Enabled = false;
                }
            }
            catch { }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}