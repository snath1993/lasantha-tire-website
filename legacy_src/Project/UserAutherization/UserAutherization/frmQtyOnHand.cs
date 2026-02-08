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
    public partial class frmQtyOnHand : Form
    {
        public DSInventorymovement ds = new DSInventorymovement();
        public static string ConnectionString;
        public frmQtyOnHand()
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
        public static string GetDateTime(DateTime DtGetDate)
        {
            try
            {
                DateTime DTP = Convert.ToDateTime(DtGetDate);
                string Dformat = "MM/dd/yyyy";
                return DTP.ToString(Dformat);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
//     QtyOnHand_Report]
//@Date datetime

                ds.Clear();

                //string S = "exec updateQty_Final";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(dt);

                string S = "exec QtyOnHand_Report '" + GetDateTime(dtpDate.Value) + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                //string S = "exec QtyOnHand_Report '" + dtpDate.Value + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(ds, "DTWHQtyONHand");
                //ViewOnHandQtyReport
               // S = "select * from TempOnHandQty where ItemID like '%" + ugCmbItem.Text + "' and WhseId ='" + ugCmbWarehouse.Text + "'";
                S = "SELECT        dbo.TempOnHandQty.WhseId, dbo.TempOnHandQty.WhseName, dbo.TempOnHandQty.ItemId, dbo.TempOnHandQty.ItemDis, dbo.TempOnHandQty.QTY, dbo.TempOnHandQty.TransDate, dbo.tblItemMaster.UnitPrice, "
                       +"  dbo.tblItemMaster.UnitCost, dbo.tblItemMaster.Categoty, dbo.tblItemMaster.Custom3  "+
"FROM            dbo.TempOnHandQty LEFT OUTER JOIN"+
                      "   dbo.tblItemMaster ON dbo.TempOnHandQty.ItemId = dbo.tblItemMaster.ItemID where dbo.tblItemMaster.ItemDescription like '%" + ugCmbItem.Text.ToString().Trim() + "' and dbo.TempOnHandQty.WhseId like '" + ugCmbWarehouse.Text + "%' and dbo.TempOnHandQty.WhseId like '%" + ugCmbWarehouse.Text + "' AND  Custom3 like '%" + ugItemType.Text + "'";
                cmd = new SqlCommand(S);
                da = new SqlDataAdapter(S, ConnectionString);
                dt = new DataTable();
                da.Fill(ds, "DTWHQtyONHand");

                //String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                //SqlCommand cmd4 = new SqlCommand(S4);
                //SqlConnection con4 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                //da4.Fill(ds, "dt_CompanyDetails");

                if (ds.DTWHQtyONHand.Rows.Count > 0)
                {
                    frmViewerQtyOnHand valuation = new frmViewerQtyOnHand(this);
                    valuation.Show();
                }
                else
                {
                    MessageBox.Show("There is no data to display");
                }


                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void GetItemDataset()//Infragistics
        {
            dsItem = new DataSet();
            try
            {

                dsItem.Clear();
                StrSql = "SELECT ItemID, ItemDescription FROM tblItemMaster order by ItemDescription";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                ugCmbItem.DataSource = dsItem.Tables["DtItem"];
                ugCmbItem.DisplayMember = "ItemDescription";
                ugCmbItem.ValueMember = "ItemDescription";
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
            GetItemType();
        }
        DataSet dsItemType = new DataSet();
        private void GetItemType()
        {
            dsItemType = new DataSet();
            try
            {

                dsItemType.Clear();
                StrSql = "SELECT  DISTINCT  Description from tbl_ItemCustom3";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItemType, "DtItem");

                ugItemType.DataSource = dsItemType.Tables["DtItem"];
                ugItemType.DisplayMember = "Description";
                ugItemType.ValueMember = "Description";
                ugItemType.DisplayLayout.Bands["DtItem"].Columns["Description"].Width = 100;
               
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["QTY"].Width = 75;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["UnitCost"].Width = 75;

            }
            catch (Exception)
            {
                throw;
            }
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ckeckItemType_Click(object sender, EventArgs e)
        {
            if (ckeckItemType.Checked == true)
            {
                ugItemType.Enabled = false;
                ugItemType.Text = "";
            }

            if (ckeckItemType.Checked == false)
            {
                ugItemType.Enabled = true;
                ugItemType.Text = "";
            }
        }
    }
}