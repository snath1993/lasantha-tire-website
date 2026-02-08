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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;


namespace UserAutherization
{
    public partial class frmPriceInquiry : Form
    {
        public frmPriceInquiry()
        {
            InitializeComponent();
            setConnectionString();
        }


        public DataSet dsItem;

        public string StrSql;

        public DataSet dsItem1;
        public static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        SqlConnection Con;
        SqlCommand cmd;
        DataSet ds;
        string SQL = string.Empty;
        SqlDataAdapter da;
        SqlTransaction Trans;


        public void GetItemID()
        {
            dsItem1 = new DataSet();
            try
            {
                dsItem1.Clear();
                StrSql = " SELECT ItemID, ItemDescription FROM tblItemMaster order by ItemID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem1, "DtItem");

                cmbItem.DataSource = dsItem1.Tables["DtItem"];
                cmbItem.DisplayMember = "ItemID";
                cmbItem.ValueMember = "ItemID";
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemID"].Width = 125;
                cmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 200;

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void frmPriceInquiry_Load(object sender, EventArgs e)
        {
            GetItemID();
        }


        private void SearchItem()
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();
                Con = new SqlConnection(ConnectionString);
                SQL = "SELECT DISTINCT tblPriceMatrix.AutiIndex, tblPriceMatrix.ItemID, tblPriceMatrix.MinQty, " +
                   " tblPriceMatrix.MaxQty, tblPriceMatrix.IsDefault, viewItemPriceSelect.PriceLevel AS [Level], " +
                   " viewItemPriceSelect.Unitprice, cast (tblPriceMatrix.PriceLevel as numeric(18,0)) as PriceLevel " +
                   "FROM         tblPriceMatrix INNER JOIN " +
                   " viewItemPriceSelect ON tblPriceMatrix.ItemID = viewItemPriceSelect.ItemID AND " +
                   " tblPriceMatrix.PriceLevel = viewItemPriceSelect.PriceLevel " +
                   "  WHERE tblPriceMatrix.ItemID ='" + cmbItem.Value.ToString().Trim() + "' ORDER BY cast (tblPriceMatrix.PriceLevel as numeric(18,0))";
                cmd = new SqlCommand(SQL, Con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                Con.Open();
                da.Fill(ds);
                Con.Close();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dgvDeliveryNoteList.Rows.Add();
                        dgvDeliveryNoteList.Rows[i].Cells[0].Value = i + 1;
                        dgvDeliveryNoteList.Rows[i].Cells[1].Value = Convert.ToDouble(ds.Tables[0].Rows[i].ItemArray[2]).ToString("N2");
                        dgvDeliveryNoteList.Rows[i].Cells[2].Value = Convert.ToDouble(ds.Tables[0].Rows[i].ItemArray[3]).ToString("N2");
                        dgvDeliveryNoteList.Rows[i].Cells[4].Value = Convert.ToBoolean(ds.Tables[0].Rows[i].ItemArray[4]);
                        dgvDeliveryNoteList.Rows[i].Cells[3].Value = Convert.ToDouble(ds.Tables[0].Rows[i].ItemArray[6]).ToString("N2");
                      
                    }
                    //dgvDeliveryNoteList.Rows.RemoveAt(ds.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void cmbItem_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        SearchItem();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }
    }
}