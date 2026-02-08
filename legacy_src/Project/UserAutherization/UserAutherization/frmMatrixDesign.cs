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
    public partial class frmMatrixDesign : Form
    {
        SqlConnection Con;
        SqlCommand cmd;
        DataSet ds;
        string SQL = string.Empty;
        SqlDataAdapter da;
        SqlTransaction Trans;
        public frmMatrixDesign()
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
        public void GetItem()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Con = new SqlConnection(ConnectionString);
                SQL = "SELECT     ItemID, ItemDescription, ItemClass FROM         tblItemMaster";
                cmd = new SqlCommand(SQL, Con);
                cmd.CommandType = CommandType.Text;
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                Con.Open();
                da.Fill(ds);
                Con.Close();
                int _index = 0;

                foreach (DataRow Dr in ds.Tables[0].Rows)
                {
                    _index = _index + 1;

                    //if (_index == 100)
                    //    MessageBox.Show("100");

                    UltraGridRow ugR;
                    ugR = UG.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["ItemID"].Value = Dr["ItemID"];
                    ugR.Cells["ItemDescription"].Value = Dr["ItemDescription"];
                    ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                    ugR.Activated = true;

                    if (Convert.ToInt64(ugR.ChildBands[0].Rows.Count) == 0)
                    {
                        UltraGridRow ugChild;
                        int iPrintList = 0;
                        while (iPrintList < 10)
                        {
                            iPrintList += 1;
                            ugChild = ugR.ChildBands[0].Band.AddNew();
                            ugChild.Cells["PriceListID"].Value = iPrintList;
                            ugChild.Cells["PriceListName"].Value = "Price List " + iPrintList.ToString();
                            ugChild.Cells["Max"].Value = 0.0;
                            ugChild.Cells["Min"].Value = 0.0;
                            ugChild.Cells["DEfault"].Value = false;
                        }
                    }

                    SQL = "SELECT DISTINCT tblPriceMatrix.AutiIndex, tblPriceMatrix.ItemID, tblPriceMatrix.MinQty, " +
                        " tblPriceMatrix.MaxQty, tblPriceMatrix.IsDefault, viewItemPriceSelect.PriceLevel AS [Level], " +
                        " viewItemPriceSelect.Unitprice, cast (tblPriceMatrix.PriceLevel as numeric(18,0)) as PriceLevel " +
                        "FROM         tblPriceMatrix INNER JOIN "+
                        " viewItemPriceSelect ON tblPriceMatrix.ItemID = viewItemPriceSelect.ItemID AND "+
                        " tblPriceMatrix.PriceLevel = viewItemPriceSelect.PriceLevel " +
                        "  WHERE tblPriceMatrix.ItemID ='" + Dr["ItemID"].ToString().Trim() + "' ORDER BY cast (tblPriceMatrix.PriceLevel as numeric(18,0))";
                    
                    cmd = new SqlCommand(SQL, Con);
                    cmd.CommandType = CommandType.Text;
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    Con.Open();
                    da.Fill(ds);
                    Con.Close();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ugR.Activated = true;
                        foreach (UltraGridRow ugM in ugR.ChildBands[0].Rows)
                        {
                            foreach (DataRow Dr1 in ds.Tables[0].Rows)
                            {
                                if (ugR.Cells["ItemID"].Value.ToString().Trim() == Dr1["ItemID"].ToString().Trim() && Convert.ToInt64(ugM.Cells["PriceListID"].Value) == Convert.ToInt64(Dr1["PriceLevel"]))
                                {
                                    ugM.Cells["Max"].Value = Convert.ToDouble(Dr1["MaxQty"]);
                                    ugM.Cells["Min"].Value = Convert.ToDouble(Dr1["MinQty"]);
                                    ugM.Cells["DEfault"].Value = Convert.ToBoolean(Dr1["IsDefault"]);
                                    ugM.Cells["Price"].Value = Convert.ToDouble(Dr1["Unitprice"]);
                                }
                            }
                        }
                    }
                    ugR.Expanded = false;
                }
                UG.GetRow(ChildRow.First).Activated = true;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


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
            catch (Exception ex)            {

                throw ex;
            }
        }
        private  void frmMatrixDesign_Load(object sender, EventArgs e)
        {
            try
            {
                //GetItemDataset()//Infragistics
                GetItemID();
                GetItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Con = new SqlConnection(ConnectionString);
            Con.Open();
            Trans = Con.BeginTransaction();
            
            SQL = "DELETE FROM tblPriceMatrix";
            cmd = new SqlCommand(SQL, Con, Trans);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            try
            {
                foreach (UltraGridRow ugR in UG.Rows)
                {
                    ugR.Activated = true;
                    if (ugR.ChildBands[0].Rows.Count > 0)
                    {
                        foreach (UltraGridRow ugM in ugR.ChildBands[0].Rows)
                        {
                            SQL = "INSERT INTO tblPriceMatrix (ItemID, MinQty, MaxQty, " +
                            " PriceLevel) VALUES ('" + ugR.Cells["ItemID"].Value.ToString().Trim() + "'," + Convert.ToDouble(ugM.Cells["Min"].Value) + "," +
                                " " + Convert.ToDouble(ugM.Cells["Max"].Value) + "," + Convert.ToInt64(ugM.Cells["PriceListID"].Value) + ")";
                            cmd = new SqlCommand(SQL, Con, Trans);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Trans.Commit();
                Con.Close();
                MessageBox.Show("Updated Successfully", "Price Metrics", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "Price Metrics", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Trans.Rollback();
                Con.Close();
            }
        }

        private void SaveEvent()
        {

            Cursor.Current = Cursors.WaitCursor;
            Con = new SqlConnection(ConnectionString);
            Con.Open();
            Trans = Con.BeginTransaction();

            SQL = "DELETE FROM tblPriceMatrix";
            cmd = new SqlCommand(SQL, Con, Trans);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            try
            {
                foreach (UltraGridRow ugR in UG.Rows)
                {
                   // ugR.Activated = true;
                    if (ugR.ChildBands[0].Rows.Count > 0)
                    {
                        foreach (UltraGridRow ugM in ugR.ChildBands[0].Rows)
                        {
                            SQL = "INSERT INTO tblPriceMatrix (ItemID, MinQty, MaxQty, " +
                            " PriceLevel,IsDefault) VALUES ('" + ugR.Cells["ItemID"].Value.ToString().Trim() + "'," + Convert.ToDouble(ugM.Cells["Min"].Value) + "," +
                                " " + Convert.ToDouble(ugM.Cells["Max"].Value) + "," + Convert.ToInt64(ugM.Cells["PriceListID"].Value) + ",'" + Convert.ToBoolean(ugM.Cells["DEfault"].Value) + "')";
                            cmd = new SqlCommand(SQL, Con, Trans);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Trans.Commit();
                Con.Close();
                MessageBox.Show("Updated Successfully", "Price Metrics", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "Price Metrics", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Trans.Rollback();
                Con.Close();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveEvent();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UG_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0)
            {
                e.Row.Cells["LN"].Value = e.Row.Index + 1;
            }
        }

        private void SearchItem()
        {

            for (int i = 0; i < UG.Rows.Count; i++)
            {
                if (UG.Rows[i].Cells["ItemID"].Value.ToString().Trim() == cmbItem.Value.ToString().Trim())
                {
                    UG.Rows[i].Activated = true;
                    UG.Rows[i].Expanded = true;
                    UG.Rows[i].Appearance.BackColor = System.Drawing.Color.SkyBlue;

                }
                else
                {
                   // UG.Rows[i].Appearance.BackColor = System.Drawing.Color.White;
                    UG.Rows[i].Expanded = false;
                  //  UG.Rows[i].Appearance.BackColor = System.Drawing.Color.de
                }
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


                       // LoadPurchaseOrder(cmbItem.ActiveRow.Cells[0].Value.ToString());
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