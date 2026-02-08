using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;

namespace UserAutherization
{
    public partial class frmLotNoEntry : Form
    {
        Controlers objControlers = new Controlers();
        public static DataTable _dataTable = new DataTable();
        public DataTable dblDatatable;
        public string TranType;
        public string strRefNo;

        public frmLotNoEntry()
        {
            InitializeComponent();
            setConnectionString();            
        }

        public frmLotNoEntry(string _ItemCode, string _WHCode, double _Qty, string _TransType, string _RefNo, DataTable _Datatable)
        {
            InitializeComponent();
            setConnectionString();

            txtItemCode.Text = _ItemCode;
            txtWHCode.Text = _WHCode;
            txtQty.Text = _Qty.ToString();
            TranType = _TransType;
            strRefNo = _TransType;
            dblDatatable = _Datatable;
            
        }

        //Golbal variable define here======================
        private double unitCost = 0.00;
        private double quantity = 0.00;
        private double totalCost = 0.00;
        public static string ConnectionString;
        private bool IsItemSerial = false;
        CSInvoiceSerial ObjSerialInvoice = new CSInvoiceSerial();
        clsCommon objclsCommon = new clsCommon();
        public string StrSql;
        public DataSet dsItem;
        public DataSet dsWarehouse;
        public DataSet dsVendor;
        public DataSet dsAR;
        public string sMsg = "Lot Items";

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

        

        private void innerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }       

        private void frmBeginingBalances_Load(object sender, EventArgs e)
        {
            try
            {
                string SSql = string.Empty;

                //txtItemCode.Text = StrItemID;
                //txt.Text = StrDescription;
                dgvInventoryBeginBal.Rows.Clear();
                DataSet dt = new DataSet();
                DataTable dtbl = new DataTable();

                if (TranType == "OpbBal")
                {
                    SSql = " SELECT ItemCode, Qty, LotNo, ExpiryDate, WH, Status " +
                        " FROM tblItemWhse_LotItems " +
                    " Where ItemCode='" + txtItemCode.Text + "' and WH='" + txtWHCode.Text + "' and Status='" + TranType + "' ";

                    SqlCommand cmd = new SqlCommand(SSql);
                    SqlDataAdapter da = new SqlDataAdapter(SSql, ConnectionString);
                    da.Fill(dt);
                }
                else
                {
                    if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                    {
                        dtbl = clsSerializeItem.DtsSerialNoList;
                        if (dt.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Tables[0].Rows)
                            {
                                dtbl.Rows.Add(dr["ItemID"].ToString(), dr["LocationID"].ToString(), dr["SerialNO"].ToString(), dr["Status"].ToString());
                            }
                        }
                    }
                }

                if (clsSerializeItem.TblLotNos.Rows.Count > 0)
                {
                    for (int k = 0; k < clsSerializeItem.TblLotNos.Rows.Count; k++)
                    {
                        if (clsSerializeItem.TblLotNos.Rows[k][0].ToString() == txtItemCode.Text.Trim())
                        {
                            dgvInventoryBeginBal.Rows.Add(txtItemCode.Text, txtWHCode.Text, clsSerializeItem.TblLotNos.Rows[k]["LotNo"].ToString(),
                                clsSerializeItem.TblLotNos.Rows[k]["Qty"].ToString(),
                                clsSerializeItem.TblLotNos.Rows[k]["ExpDate"].ToString(), "Delete");                            
                        }
                    }
                }
                if (dt.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < dt.Tables[0].Rows.Count; k++)
                    {
                        if (dt.Tables[0].Rows[k][0].ToString() == txtItemCode.Text.Trim())
                        {
                            //DataRow dr = dt.Tables[0].Rows[i];
                            DataRow[] foundRows = new DataRow[1];
                            //if (dr["ItemCode"].ToString() == txtItemCode.Text.Trim() && dr["WHCode"].ToString().Trim() == txtWHCode.Text.Trim())
                            foundRows = dt.Tables[0].Select("ItemCode ='" + txtItemCode.Text.Trim() + "'");
                            if (foundRows.Length == 0)
                            {
                                dgvInventoryBeginBal.Rows.Add(txtItemCode.Text, txtWHCode.Text, dt.Tables[0].Rows[k]["LotNo"].ToString(),
                                dt.Tables[0].Rows[k]["ExpiryDate"].ToString(), "Delete");
                            }                            
                        }
                    }
                }
                //if(
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Lot No Entry", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
  
        

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int RowCount = GetFilledRows();
                double _TotQty = 0;

                for (int i = dblDatatable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dblDatatable.Rows[i];
                    if (
                        dr["ItemCode"].ToString() == txtItemCode.Text &&
                    txtWHCode.Text == dr["WH"].ToString().Trim())
                        dr.Delete();
                }

                for (int i = 0; i < dgvInventoryBeginBal.Rows.Count; i++)
                {
                    if (dgvInventoryBeginBal.Rows[i].Cells[0].Value != null && 
                        dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString() != string.Empty &&
                        dgvInventoryBeginBal.Rows[i].Cells[2].Value != null && 
                        dgvInventoryBeginBal.Rows[i].Cells[2].Value.ToString() != string.Empty &&
                        dgvInventoryBeginBal.Rows[i].Cells[3].Value != null &&
                        dgvInventoryBeginBal.Rows[i].Cells[3].Value.ToString() != string.Empty)
                    {
                        DataRow drow = dblDatatable.NewRow();

                        if (dblDatatable.Columns.Count == 0)
                        {
                            dblDatatable.Columns.Add("ItemCode");
                            dblDatatable.Columns.Add("WH");
                            dblDatatable.Columns.Add("LotNo");
                            dblDatatable.Columns.Add("Status");
                            dblDatatable.Columns.Add("ExpDate");
                            dblDatatable.Columns.Add("Qty");
                        }

                        drow["ItemCode"] = dgvInventoryBeginBal.Rows[i].Cells[0].Value;
                        drow["WH"] = dgvInventoryBeginBal.Rows[i].Cells[1].Value;
                        drow["LotNo"] = dgvInventoryBeginBal.Rows[i].Cells[2].Value;
                        drow["Qty"] = dgvInventoryBeginBal.Rows[i].Cells[3].Value;
                        drow["Status"] = "OpbBal";
                        drow["ExpDate"] = dgvInventoryBeginBal.Rows[i].Cells[4].Value;
                        _TotQty = _TotQty + double.Parse(dgvInventoryBeginBal.Rows[i].Cells[3].Value.ToString());
                        dblDatatable.Rows.Add(drow);
                    }
                }
                if (double.Parse(txtQty.Text) == _TotQty)
                {
                    clsSerializeItem.TblLotNos = dblDatatable;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Quantity...!", "Lot No Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Lot No Entry", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private int GetFilledRows()
        {
            try
            {
                int RowCount = 0;
                for (int i = 0; i < dgvInventoryBeginBal.Rows.Count; i++)
                {
                    if (dgvInventoryBeginBal.Rows[i].Cells[0].Value != null) //change cell value by 1                   
                    {
                        RowCount++;
                    }
                }
                return RowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvInventoryBeginBal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvInventoryBeginBal_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvInventoryBeginBal.IsCurrentCellDirty)
            {
                dgvInventoryBeginBal.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                dgvInventoryBeginBal.Rows.Add(txtItemCode.Text, txtWHCode.Text, "",txtQty.Text, "", "Delete");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Lot No Entry", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvInventoryBeginBal_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvInventoryBeginBal.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim() == "Delete")
                {
                    dgvInventoryBeginBal.Rows.RemoveAt(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Lot No Entry", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

       
        

       

        
        


        
      
    }
}