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
    public partial class ItemPurchaseAndSold : Form
    {
        public static string ConnectionString;
        public  DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public  DsSupplierInvoice ds = new DsSupplierInvoice();
        public ItemPurchaseAndSold()
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
        private void ItemPurchaseAndSold_Load(object sender, EventArgs e)
        {
            GetItemDataSet();
            GetItemBrandDataSet();
            GetSuplierDataSet();
        }

        private void GetSuplierDataSet()
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

        private void GetItemBrandDataSet()
        {
            string StrSql = "select  Description from tbl_ItemCustom3";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbBrand.DataSource = dt;
                cmbBrand.ValueMember = "Description";
                cmbBrand.DisplayMember = "Description";

                cmbBrand.DisplayLayout.Bands[0].Columns[0].Width = 120;
            }
        }

        public void GetItemDataSet()
        {
            try
            {
                string StrSql = "SELECT ItemID,ItemDescription,Categoty FROM tblItemMaster order by ItemDescription";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "ItemDescription";
                cmbItem.ValueMember = "ItemID";
                cmbItem.DisplayLayout.Bands[0].Columns["ItemID"].Width = 200;
                cmbItem.DisplayLayout.Bands[0].Columns["ItemDescription"].Width = 300;
                cmbItem.DisplayLayout.Bands[0].Columns["Categoty"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void cmbItem_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {

            LoadData();
        }

        private void LoadData()
        {
            double qty = 0;
            double qty2 = 0;
            string ItemId = "";

            string sSQL;
            if (cmbItem.Value!=null)
            {
                ItemId = cmbItem.Value.ToString().Trim();
            }

            if (cmbBrand.Text.ToString().Trim() == "")
            {
                 sSQL = "Select  sum(QTY)+sum(FreeQty)  from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'";
            }
            else
            {
                 sSQL = "Select  sum(QTY)+sum(FreeQty)  from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
              " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'" +
              " and ItemID in (select ItemID from tblItemMaster where Custom3='"+cmbBrand.Text.ToString()+"')";

            }


            SqlCommand cmd3 = new SqlCommand(sSQL);
            SqlConnection con3 = new SqlConnection(ConnectionString);
            SqlDataAdapter da3 = new SqlDataAdapter(sSQL, con3);
            DataTable dt = new DataTable();
            da3.Fill(dt);
            if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString() != "")
            {
                qty = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
            }
            string sSQL2;

            if (cmbBrand.Text.ToString().Trim() == "")
            {
                 sSQL2 = "Select sum(QTY)+sum(FreeQty) from tblSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
               " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'";

            }
            else
            {
                 sSQL2 = "Select sum(QTY)+sum(FreeQty) from tblSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
              " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'"+
               " and ItemID in (select ItemID from tblItemMaster where Custom3='" + cmbBrand.Text.ToString() + "')";

            }
            SqlCommand cmd33 = new SqlCommand(sSQL2);
            SqlConnection con33 = new SqlConnection(ConnectionString);
            SqlDataAdapter da33 = new SqlDataAdapter(sSQL2, con33);
            DataTable dt2 = new DataTable();
            da33.Fill(dt2);
            if (dt2.Rows.Count > 0 && dt2.Rows[0].ItemArray[0].ToString() != "")
            {
                qty = qty + Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString());
            }


            string sSQL3 = "Select sum(Qty)+sum(FOCQty) from tblSalesInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
             " and ItemID like '%'+'" + ItemId + "'";

            SqlCommand cmd66 = new SqlCommand(sSQL3);
            SqlConnection con66 = new SqlConnection(ConnectionString);
            SqlDataAdapter da66 = new SqlDataAdapter(sSQL3, con66);
            DataTable dt3 = new DataTable();
            da66.Fill(dt3);
            if (dt3.Rows.Count > 0 && dt3.Rows[0].ItemArray[0].ToString() != "")
            {
                qty2 = Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString());
            }

            label7.Text = qty2.ToString();

            label3.Text = qty.ToString();


            LoadGRN();
            LoadInoice();
        }

        private void LoadInoice()
        {
            try
            {
                string ItemId = "";

                if (cmbItem.Value != null)
                {
                    ItemId = cmbItem.Value.ToString().Trim();
                }

                string StrSql = "SELECT InvoiceNo,CustomerID,InvoiceDate FROM tblSalesInvoices WHERE   [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
              " and ItemID like '%'+'" + ItemId + "'";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbSales.DataSource = dt;
                cmbSales.DisplayMember = "InvoiceNo";
                cmbSales.ValueMember = "InvoiceNo";
                cmbSales.DisplayLayout.Bands[0].Columns["InvoiceNo"].Width = 200;
                cmbSales.DisplayLayout.Bands[0].Columns["CustomerID"].Width = 300;
                cmbSales.DisplayLayout.Bands[0].Columns["InvoiceDate"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadGRN()
        {
            try
            {
                string ItemId = "";

                if (cmbItem.Value != null)
                {
                    ItemId = cmbItem.Value.ToString().Trim();
                }
                string StrSql;
                if (cmbBrand.Text.ToString() == "")
                {
                     StrSql = "SELECT SupInvoiceNo,GRNNos,InvoiceDate FROM tblDirectSupplierInvoices WHERE   [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                  " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'";
                }
                else
                {
                    StrSql = "Select  SupInvoiceNo,GRNNos,InvoiceDate  from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
             " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'" +
             " and ItemID in (select ItemID from tblItemMaster where Custom3='" + cmbBrand.Text.ToString() + "')";
                }

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dAdapt.Fill(dt);

                cmbGRN.DataSource = dt;
                cmbGRN.DisplayMember = "SupInvoiceNo";
                cmbGRN.ValueMember = "SupInvoiceNo";
       
                cmbGRN.DisplayLayout.Bands[0].Columns["SupInvoiceNo"].Width = 300;
                cmbGRN.DisplayLayout.Bands[0].Columns["GRNNos"].Width = 200;
                cmbGRN.DisplayLayout.Bands[0].Columns["InvoiceDate"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        int RT = 0;
        private void btnView_Click(object sender, EventArgs e)
        {
            string ItemId = "";

            if (cmbItem.Value != null)
            {
                ItemId = cmbItem.Value.ToString().Trim();
            }

            DsItemWise.Clear();
            string StrSql = "SELECT * FROM tblSalesInvoices WHERE   [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
             " and ItemID like '%'+'" + ItemId + "' order by InvoiceDate";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsItemWise.dtSalesInvoice);
            RT = 1;
            frmPurchaseAndSoldPrint ps = new frmPurchaseAndSoldPrint(this,RT);
            ps.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ds.Clear();
            string ItemId = "";

            if (cmbItem.Value != null)
            {
                ItemId = cmbItem.Value.ToString().Trim();
            }
            String S1;
            if (cmbBrand.Text.ToString().Trim() == "")
            {
                 S1 = "Select * from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
                 " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "' order by InvoiceDate";
            }
            else
            {
                 S1 = "Select * from tblDirectSupplierInvoices where [InvoiceDate] >='" + dtpFromDate.Value.ToString("MM/dd/yyyy") + "' and [InvoiceDate]<='" + dtpToDate.Value.ToString("MM/dd/yyyy") + "'" +
            " and ItemID like '%'+'" + ItemId + "' and CustomerID like '%'+'" + cmbCustomer.Text.ToString() + "'"+
               " and ItemID in (select ItemID from tblItemMaster where Custom3='" + cmbBrand.Text.ToString() + "') order by InvoiceDate";
            }
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlConnection con1 = new SqlConnection(ConnectionString);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
            da1.Fill(ds, "DtSupInv");
            RT = 2;

            frmPurchaseAndSoldPrint ps = new frmPurchaseAndSoldPrint(this,RT);
            ps.Show();
        }

        private void chkItemAll_CheckedChanged(object sender, EventArgs e)
        {
            cmbItem.Text = "";
            LoadData();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cmbGRN_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            if (cmbGRN.Text != "")
            {
                Search.searchDirSupNo = cmbGRN.Text.ToString().Trim();
                frmDirectSupInvoice D = new frmDirectSupInvoice(this, cmbGRN.Text.ToString());
                D.Show();

            }
        }

        private void cmbGRN_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void cmbGRN_SizeChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ultraCombo1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void cmbItem_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void cmbBrand_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            LoadData();
        }

        private void cmbCustomer_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            LoadData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cmbBrand.Text = "";
            LoadData();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            cmbCustomer.Text = "";
            LoadData();
        }
    }
}
