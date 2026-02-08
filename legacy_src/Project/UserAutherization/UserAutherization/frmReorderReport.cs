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
    public partial class frmReorderReport : Form
    {
        public frmReorderReport()
        {
            setConnectionString();
            InitializeComponent();
            
        }
        public static string ConnectionString;
        public DSInventorymovement ds = new DSInventorymovement();
        public string StrSql;
        public DataSet dsItem;
        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Warehouse-Reorder Quantity";

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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                ds.Clear();
                string S = "exec QtyOnHand_Report '" + GetDateTime(dtpDate.Value) + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

               // S = "select * from ViewOnHandQtyReport where ItemID like '%" + ugCmbItem.Text + "' and WhseId ='" + ugCmbWarehouse.Text + "'";

                S = "select * from View_ItemReorderReport where ItemID like '%" + ugCmbItem.Text + "' and WhseId ='" + ugCmbWarehouse.Text + "'";
                //View_ItemReorderReport
                cmd = new SqlCommand(S);
                da = new SqlDataAdapter(S, ConnectionString);
                dt = new DataTable();
                da.Fill(ds, "DTWHQtyONHand");


                if (ds.DTWHQtyONHand.Rows.Count > 0)
                {
                    frmViewerReorderReport ObjReorderReport = new frmViewerReorderReport(this);
                    ObjReorderReport.Show();
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
        private void frmReorderReport_Load(object sender, EventArgs e)
        {
            try
            {
                GetItemDataset();
                GetWareHouseDataSet();
            }
            catch
            {
            }
        }
    }
}
