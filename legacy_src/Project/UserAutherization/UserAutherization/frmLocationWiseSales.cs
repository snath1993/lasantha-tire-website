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
    public partial class frmLocationWiseSales : Form
    {
        public DSMainReports ds = new DSMainReports();
        public static string ConnectionString;

        public frmLocationWiseSales()
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

        private void frmLocationWiseSales_Load(object sender, EventArgs e)
        {
            try
            {
                chkDateFrom.Checked = true;
                chkDateTo.Checked = true;
                //dtpDateFrom.Enabled = true;
                //dtpDateTo.Enabled = true;
                cmbWarehouseCode.Enabled = true;

                string S = "Select Location from tblSalesInvoices order by Location";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbWarehouseCode.Items.Clear();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cmbWarehouseCode.Items.Add(dt.Rows[i].ItemArray[0].ToString().Trim());
                    }
                }
            }
            catch {}
        }

        private void chkDateFrom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDateFrom.Checked == true)
                {
                    dtpDateFrom.Enabled = true;
                }
                else if(chkDateFrom.Checked == false)
                {
                    dtpDateFrom.Enabled = false;
                }
            }
            catch { }
        }

        private void chkDateTo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDateTo.Checked == true)
                {
                    dtpDateTo.Enabled = true;
                }
                else if (chkDateTo.Checked == false)
                {
                    dtpDateTo.Enabled = false;
                }
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpDateFrom.Enabled == true && dtpDateTo.Enabled == true)
                {
                    if (cmbWarehouseCode.SelectedItem == null)
                    {
                        ds.Clear();
                        string S = "Select * from tblSalesInvoices where InvoiceDate between '" + dtpDateFrom.Text.ToString().Trim() + "' and '" + dtpDateTo.Text.ToString().Trim() + "'";
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
                            frmViewerLocationSales locationSales = new frmViewerLocationSales(this);
                            locationSales.Show();
                        }
                        else
                        {
                            MessageBox.Show("There is no data to display");
                        }
                    }
                    else
                    {
                        ds.Clear();
                        string S = "Select * from tblSalesInvoices where Location = '" + cmbWarehouseCode.Text.ToString().Trim() + "' and InvoiceDate between '" + dtpDateFrom.Text.ToString().Trim() + "' and '" + dtpDateTo.Text.ToString().Trim() + "'";
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
                            frmViewerLocationSales locationSales = new frmViewerLocationSales(this);
                            locationSales.Show();
                        }
                        else
                        {
                            MessageBox.Show("There is no data to display");
                        }
                    }
                }
                else
                {
                        ds.Clear();
                        string S = "Select * from tblSalesInvoices where Location = '" + cmbWarehouseCode.Text.ToString().Trim() + "'";
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
                            frmViewerLocationSales locationSales = new frmViewerLocationSales(this);
                            locationSales.Show();
                        }
                        else
                        {
                            MessageBox.Show("There is no data to display");
                        }
                }
            }
            catch { }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}