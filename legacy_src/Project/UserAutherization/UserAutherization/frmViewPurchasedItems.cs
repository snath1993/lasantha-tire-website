using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmViewPurchasedItems : Form
    {
        public static string ConnectionString;
        public DSPurchasedItems DSPurchasedItems = new DSPurchasedItems();

        public frmViewPurchasedItems()
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
            catch { }
        }
        private void frmViewPurchasedItems_Load(object sender, EventArgs e)
        {
            GetItemDataSet();
            GetVendorDataset();
            GetFromHouseDataSet();
        }

       
        public void GetItemDataSet()
        {
            try
            {
                string StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster order by ItemID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "ItemDescription";
                cmbItem.ValueMember = "ItemDescription";
                cmbItem.DisplayLayout.Bands[0].Columns["ItemID"].Width = 200;
                cmbItem.DisplayLayout.Bands[0].Columns["ItemDescription"].Width = 300;
                cmbItem.DisplayLayout.Bands[0].Columns["Categoty"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetFromHouseDataSet()
        {
            try
            {
               string StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbWH.DataSource = dt;
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";
                cmbWH.DisplayLayout.Bands[0].Columns["WhseId"].Width = 50;
                cmbWH.DisplayLayout.Bands[0].Columns["WhseName"].Width = 150;
                cmbWH.DisplayLayout.Bands[0].Columns["ArAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands[0].Columns["CashAccount"].Hidden = true;
                cmbWH.DisplayLayout.Bands[0].Columns["SalesGLAccount"].Hidden = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetVendorDataset()
        {
            try
            {
              string  StrSql = "SELECT VendorID,VendorName FROM tblVendorMaster";

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
            DSPurchasedItems.Clear();

            String S312 = "Select * from tblCompanyInformation";
            SqlCommand cmd312 = new SqlCommand(S312);
            SqlConnection con312 = new SqlConnection(ConnectionString);
            SqlDataAdapter da312 = new SqlDataAdapter(S312, con312);
            da312.Fill(DSPurchasedItems.DTCompany);

            String S1 = "Select * from tblVendorMaster";// where DeliveryNoteNo = '" + txtDeliveryNoteNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlConnection con1 = new SqlConnection(ConnectionString);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                da1.Fill(DSPurchasedItems.tblVendorMaster);

            string sSQL = "Select * from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                " and Description like '%'+'" + cmbItem.Text.ToString().Trim() + "'" +
                " and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'" +
                " and Location like '%" + cmbWH.Text.ToString().Trim() + "' and Location like '" + cmbWH.Text.ToString().Trim() + "%'";

                SqlCommand cmd3 = new SqlCommand(sSQL);
                SqlConnection con3 = new SqlConnection(ConnectionString);
                SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
                da3.Fill(DSPurchasedItems.SuplierInvoice);


            string sSQL2 = "Select * from tblSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
               " and Description like '%'+'" + cmbItem.Text.ToString().Trim() + "'" +
               " and CustomerID like '%'+'" + cmbCustomer.Text.ToString().Trim() + "'" +
               " and Location like '%" + cmbWH.Text.ToString().Trim() + "' and Location like '" + cmbWH.Text.ToString().Trim() + "%'";

            SqlCommand cmd33 = new SqlCommand(sSQL2);
            SqlConnection con33 = new SqlConnection(ConnectionString);
            SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
            da33.Fill(DSPurchasedItems.SuplierInvoice);


            frmPurchasedItemsPrint ObjViwerInvPrint = new frmPurchasedItemsPrint(this);
            ObjViwerInvPrint.Show();

        }

        private void chkItemAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkItemAll.Checked)
            {
                cmbItem.Text = "";
                cmbItem.Enabled = false;
            }
            else
            {
                cmbItem.Enabled = true;
            }
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

        private void chkWHAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWHAll.Checked)
            {
                cmbWH.Text = "";
                cmbWH.Enabled = false;
            }
            else
            {
                cmbWH.Enabled = true;
            }
        }
    }
}
