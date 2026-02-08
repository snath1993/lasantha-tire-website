using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace UserAutherization
{
    public partial class frmInventoryAdjustmentList : Form
    {
        clsCommon objclsCommon = new clsCommon();

        public frmInventoryAdjustmentList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmInventoryAdjustmentList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAdjustmentData();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void LoadAdjustmentData()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost,OnhandQty,NewQty from tblInventoryAdjustment order by AdjusmentId DESC";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInvoiceList.Rows.Add();

                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();//Reference
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                        dgvInvoiceList.Rows[i].Cells[1].Value = abc.ToShortDateString();//Date

                        dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();

                        if (Convert.ToDouble(dt.Rows[i].ItemArray[6].ToString()) > Convert.ToDouble(dt.Rows[i].ItemArray[7].ToString()))
                        {
                            dgvInvoiceList.Rows[i].Cells[4].Value = "-" + Convert.ToDouble(dt.Rows[i].ItemArray[4]).ToString("N2");
                        }

                        else
                        {
                            dgvInvoiceList.Rows[i].Cells[4].Value = Convert.ToDouble(dt.Rows[i].ItemArray[4]).ToString("N2");
                        }
                        dgvInvoiceList.Rows[i].Cells[5].Value = Convert.ToDouble(dt.Rows[i].ItemArray[5]).ToString("N2");

                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void SeachOption()
        {
            try
            {
                dgvInvoiceList.Rows.Clear();

                string s = "";
               
                switch (cmbSearchby.Text)
                {
                    case "AdjustmentCode":
                        {
                            s = " Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost from tblInventoryAdjustment where AdjusmentId like '%" + txtSearch.Text.Trim() + "%' order by AdjusmentId DESC";
                            //StrCode = "AdjusmentId";
                            break;
                        }
                    case "Date":
                        {
                            s = " Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost from tblInventoryAdjustment where Date='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' order by AdjusmentId DESC";
                            //StrCode = "Date";
                            break;
                        }
                    case "WarehouseCode":
                        {
                            s = " Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost from tblInventoryAdjustment where WarehouseId like '%" + txtSearch.Text.Trim() + "%' order by AdjusmentId DESC";
                            //StrCode = "WarehouseId";
                            break;
                        }
                    case "Item ID":
                        {
                            s = " Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost from tblInventoryAdjustment where ItemID like '%" + txtSearch.Text.Trim() + "%' order by AdjusmentId DESC";
                            //StrCode = "ItemID";
                            break;
                        }                   
                }


                string ConnString = ConnectionString;
                
                SqlCommand cmd1 = new SqlCommand(s);
                //cmd1.CommandType = CommandType.Text;
                SqlDataAdapter da1 = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                //SqlCommand cmd = new SqlCommand(s);
                //SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                //DataTable dt = new DataTable();
                //da.Fill(dt);

                dgvInvoiceList.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInvoiceList.Rows.Add();
                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = DateTime.Parse(dt.Rows[i].ItemArray[1].ToString().Trim()).ToString("dd/MM/yyyy");
                        dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        //                            s = " Select AdjusmentId,Date,WarehouseId,ItemID,AdjustQty,UnitCost from tblInventoryAdjustment where ItemID like '%" + txtSearch.Text.Trim() + "%' order by AdjusmentId ";

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtSearch.Text = "";
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvInvoiceList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (dgvInvoiceList.CurrentCell!=null)
            //    {
            //        if (dgvInvoiceList.CurrentCell.Value != null && dgvInvoiceList.CurrentCell.Value.ToString().Trim().Length > 0)
            //         Search.AdjstID= dgvInvoiceList[0, dgvInvoiceList.CurrentRow.Index].Value.ToString().Trim();
            //    }
            //    this.Close();
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            dtpSearchDate.Value = user.LoginDate;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvInvoiceList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvInvoiceList.CurrentCell != null)
                {
                    if (dgvInvoiceList.CurrentCell.Value != null && dgvInvoiceList.CurrentCell.Value.ToString().Trim().Length > 0)
                        Search.AdjstID = dgvInvoiceList[0, dgvInvoiceList.CurrentRow.Index].Value.ToString().Trim();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
    }
}