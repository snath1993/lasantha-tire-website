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
    public partial class frmValuation : Form
    {
        public DSMainReports ds = new DSMainReports();
        public static string ConnectionString;
        public frmValuation()
        {
            setConnectionString();
            InitializeComponent();
        }
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

        public string StrSql;
        public DataSet dsItem;
        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Peachtree-On Hand Quantity";

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkItemAll.Checked == true && chkWarehouseAll.Checked == true)
                {
                    ds.Clear();
                    string S = "Select * from tblItemWhse";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_valuation");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_valuation.Rows.Count > 0)
                    {
                        frmViewerValuations valuation = new frmViewerValuations(this);
                        valuation.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }

                }

                //-----------------------------------------------------------------
                if (chkWarehouseAll.Checked == true && chkItemAll.Checked == false)
                {
                    if (ugCmbItem.Value == null)
                    {
                        MessageBox.Show("Please Select a valid Item");
                        return;
                    }

                    ds.Clear();
                    string S = "Select * from tblItemWhse where ItemId = '" + ugCmbItem.Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_valuation");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_valuation.Rows.Count > 0)
                    {
                        frmViewerValuations valuation = new frmViewerValuations(this);
                        valuation.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }

                }
                //------------------------------------------------------------------------

                if (chkWarehouseAll.Checked == false && chkItemAll.Checked == true)
                {

                    if (ugCmbWarehouse.Value == null)
                    {
                        MessageBox.Show("Please Select a valid Warehouse");
                        return;
                    }
                    ds.Clear();
                    string S = "Select * from tblItemWhse where WhseId = '" + ugCmbWarehouse.Value.ToString().Trim() + "'";// and ItemId = '" + cmbItems.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_valuation");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_valuation.Rows.Count > 0)
                    {
                        frmViewerValuations valuation = new frmViewerValuations(this);
                        valuation.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }

                }

                if (chkWarehouseAll.Checked == false && chkItemAll.Checked == false)
                {

                    if (ugCmbWarehouse.Value == null)
                    {
                        MessageBox.Show("Please Select a valid Warehouse");
                        return;
                    }

                    if (ugCmbItem.Value == null)
                    {
                        MessageBox.Show("Please Select a valid Item");
                        return;
                    }
                    ds.Clear();
                    string S = "Select * from tblItemWhse where WhseId = '" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemId = '" + ugCmbItem.Value.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(ds, "dt_valuation");

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    da4.Fill(ds, "dt_CompanyDetails");

                    if (ds.dt_valuation.Rows.Count > 0)
                    {
                        frmViewerValuations valuation = new frmViewerValuations(this);
                        valuation.Show();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to display");
                    }


                }
            }
            catch { }
        }
        public void GetItemDataset()//Infragistics
        {
            dsItem = new DataSet();
            try
            {

                dsItem.Clear();
                StrSql = "SELECT ItemID, ItemDescription FROM tblItemMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                ugCmbItem.DataSource = dsItem.Tables["DtItem"];
                ugCmbItem.DisplayMember = "ItemId";
                ugCmbItem.ValueMember = "ItemId";
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Width = 100;
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 200;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["QTY"].Width = 75;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["UnitCost"].Width = 75;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                ugCmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                ugCmbWarehouse.DisplayMember = "WhseId";
                ugCmbWarehouse.ValueMember = "WhseId";
                ugCmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                ugCmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                // cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;  
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void frmValuation_Load(object sender, EventArgs e)
        {
            GetItemDataset();
            GetWareHouseDataSet();
        }

        private void chkWarehouseAll_Click(object sender, EventArgs e)
        {
            if (chkWarehouseAll.Checked == true)
            {
                ugCmbWarehouse.Enabled = false;
                ugCmbWarehouse.Text = "";
            }
            if (chkWarehouseAll.Checked == false)
            {
                ugCmbWarehouse.Enabled = true;
                ugCmbWarehouse.Text = "";
            }
        }

        private void chkItemAll_Click(object sender, EventArgs e)
        {
            if (chkItemAll.Checked == true)
            {
                ugCmbItem.Enabled = false;
                ugCmbItem.Text = "";
            }

            if (chkItemAll.Checked == false)
            {
                ugCmbItem.Enabled = true;
                ugCmbItem.Text = "";
            }

        }

        private void chkWarehouseAll_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}