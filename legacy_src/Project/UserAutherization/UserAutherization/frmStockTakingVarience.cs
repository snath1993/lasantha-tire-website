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
    public partial class frmStockTakingVarience : Form
    {
        public DSMainReports ds = new DSMainReports();
        public static string ConnectionString;
        public frmStockTakingVarience()
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

        private void frmStockTakingVarience_Load(object sender, EventArgs e)
        {
            string S = "Select distinct WhseId from tblItemWhse order by WhseId";
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

            chkDateFrom.Checked = true;
            chkDateTo.Checked = true;
        }

        private void chkDateFrom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDateFrom.Checked == true)
                {
                    dtpDateFrom.Enabled = true;
                }
                if (chkDateFrom.Checked == false)
                {
                    dtpDateFrom.Enabled = false;
                }
                if (chkDateTo.Checked == true)
                {
                    dtpDateTo.Enabled = true;
                }
                if (chkDateTo.Checked == false)
                {
                    dtpDateTo.Enabled = false;
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
                if (chkDateTo.Checked == false)
                {
                    dtpDateTo.Enabled = false;
                }
                if (chkDateTo.Checked == true)
                {
                    dtpDateTo.Enabled = true;
                }
                if (chkDateTo.Checked == false)
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
                if (dtpDateFrom.Enabled == true && dtpDateTo.Enabled == true && cmbWarehouseCode.SelectedItem != null)
                {
                    ds.Clear();
                    string S = "Select * from tblOpeningBal where Odate between '" + dtpDateFrom.Text.ToString().Trim() + "' and '" + dtpDateTo.Text.ToString().Trim() + "' and WhseId = '" + cmbWarehouseCode.Text.ToString().Trim() + "' and TransType = 'StkTake'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_Stocktaking");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_Stocktaking.Rows.Count > 0)
                    {
                        frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                        stockTaking.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else if(dtpDateFrom.Enabled == true && dtpDateTo.Enabled == true)
                {
                    ds.Clear();
                    string S = "Select * from tblOpeningBal where Odate between '" + dtpDateFrom.Text.ToString().Trim() + "' and '" + dtpDateTo.Text.ToString().Trim() + "' and TransType = 'StkTake'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_Stocktaking");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_Stocktaking.Rows.Count > 0)
                    {
                        frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                        stockTaking.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else if (dtpDateFrom.Enabled == true && cmbWarehouseCode.SelectedItem != null)
                {
                    ds.Clear();
                    string S = "Select * from tblOpeningBal where Odate >= '" + dtpDateFrom.Text.ToString().Trim() + "' and WhseId = '" + cmbWarehouseCode.Text.ToString().Trim() + "' and TransType = 'StkTake'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_Stocktaking");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_Stocktaking.Rows.Count > 0)
                    {
                        frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                        stockTaking.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else if (dtpDateTo.Enabled == true && cmbWarehouseCode.SelectedItem != null)
                {
                    ds.Clear();
                    string S = "Select * from tblOpeningBal where Odate <= '" + dtpDateTo.Text.ToString().Trim() + "' and WhseId = '" + cmbWarehouseCode.Text.ToString().Trim() + "' and TransType = 'StkTake'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_Stocktaking");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_Stocktaking.Rows.Count > 0)
                    {
                        frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                        stockTaking.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                }
                else
                {
                    if(cmbWarehouseCode.SelectedItem == null)
                    {
                        ds.Clear();
                        string S = "Select * from tblOpeningBal where TransType = 'StkTake'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(ds, "dt_Stocktaking");

                        String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                        SqlCommand cmd4 = new SqlCommand(S4);
                        SqlConnection con4 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                        da4.Fill(ds, "dt_CompanyDetails");

                        if (ds.dt_Stocktaking.Rows.Count > 0)
                        {
                            frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                            stockTaking.Show();
                        }
                        else
                        {
                            MessageBox.Show("There is no data to display");
                        }
                    }
                    else
                    {
                    ds.Clear();
                    string S = "Select * from tblOpeningBal where WhseId = '" + cmbWarehouseCode.Text.ToString().Trim() + "' and TransType = 'StkTake'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_Stocktaking");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_Stocktaking.Rows.Count > 0)
                    {
                        frmViewerStockTaking stockTaking = new frmViewerStockTaking(this);
                        stockTaking.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }
                    }
                }
            }
            catch { }
        }
    }
}