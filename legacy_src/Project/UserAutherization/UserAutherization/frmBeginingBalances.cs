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
    public partial class frmBeginingBalances : Form
    {
        Controlers objControlers = new Controlers();
        public static DataTable _dataTable = new DataTable();
        public static DataTable _dataTableLotNo = new DataTable();

        public frmBeginingBalances()
        {
            InitializeComponent();
            setConnectionString();
            cmbWH.Focus();
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
        public string sMsg = "Peachtree -Opening Balances";
        //==========================================================

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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //------------------------------------
        //private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    try
        //    {
        //        if ((dgvItems.CurrentCell.ColumnIndex == 5 || dgvItems.CurrentCell.ColumnIndex == 7 || dgvItems.CurrentCell.ColumnIndex == 8 ||
        //            dgvItems.CurrentCell.ColumnIndex == 10 || dgvItems.CurrentCell.ColumnIndex == 11 || dgvItems.CurrentCell.ColumnIndex == 12 ||
        //            dgvItems.CurrentCell.ColumnIndex == 13 || dgvItems.CurrentCell.ColumnIndex == 14 || dgvItems.CurrentCell.ColumnIndex == 15)
        //            && dgvItems.EditingControl.Text != null && dgvItems.CurrentCell.Value.ToString().Trim() != string.Empty)
        //        {
        //            //dgvItems.EditingControl.Text = dgvItems.CurrentCell.Value.ToString();
        //            TextBox innerTextBox;
        //            if (e.Control is TextBox)
        //            {
        //                innerTextBox = e.Control as TextBox;
        //                innerTextBox.KeyPress += new KeyPressEventHandler(innerTextBox_KeyPress);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
        //    }
        //}


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
        //---------------------------------------------





        private void frmBeginingBalances_Load(object sender, EventArgs e)
        {
            try
            {
                clsSerializeItem.DtsSerialNoList.Rows.Clear();
                clsSerializeItem.TblLotNos.Rows.Clear();
                dtpOPbBalnce.Value = user.LoginDate;
                GetItemDataset();
                GetWareHouseDataSet();
                cmbWH.Focus();

                //DialogResult reply = MessageBox.Show("Do you want to Import ItemMasterfile now ?", "Information", MessageBoxButtons.YesNo);
                //if (reply == DialogResult.Yes)
                //{
                //    Connector ObjConnector = new Connector();
                //    ObjConnector.Get_Peachtree_Onhand();
                //    ObjConnector.Insert_ItemOnhand();
                //}
                _dataTable.Columns.Clear();
                if (_dataTable.Columns.Count == 0)
                {
                    _dataTable.Columns.Add("ItemCode");
                    _dataTable.Columns.Add("WHCode");
                    _dataTable.Columns.Add("SerialNo");
                    _dataTable.Columns.Add("Status");

                }

                _dataTableLotNo.Columns.Clear();
                if (_dataTableLotNo.Columns.Count == 0)
                {
                    _dataTableLotNo.Columns.Add("ItemCode");
                    _dataTableLotNo.Columns.Add("WH");
                    _dataTableLotNo.Columns.Add("Qty");
                    _dataTableLotNo.Columns.Add("Status");
                    _dataTableLotNo.Columns.Add("LotNo");
                    _dataTableLotNo.Columns.Add("ExpDate");

                }

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GeTotalBalances()
        {
            try
            {
                double TotalValue = 0.0;
                int rowcount = GetFilledRows();
                for (int i = 0; i < rowcount; i++)
                {
                    TotalValue = TotalValue + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[4].Value);
                }
                txtTotalBeginBalance.Text = TotalValue.ToString("N2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //load the warehouse details from the tbl warehouse master



        //=======================================================
        private void LoadAddresses()
        {
            try
            {
                String S1 = "Select WhseName from tblWhseMaster where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    txtwarehouseName.Text = dt1.Rows[0].ItemArray[0].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=======================================================

        private void Load_InvetoryList()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ItemID,ItemDescription,UnitCost from tblItemMaster"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInventoryBeginBal.Rows.Add();
                        dgvInventoryBeginBal.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInventoryBeginBal.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvInventoryBeginBal.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //load item from warehousewiseitem table 
        //============================================
        private void Load_InvetoryListFromWarehouse()
        {
            try
            {
                string trantype = "OpbBal";
                dgvInventoryBeginBal.Rows.Clear();
                string ConnString = ConnectionString;
                // String S1 = "Select ItemId,ItemDis,QTY,UnitCost from tblItemWhse where WhseId='" + cmbWarehouse.Text.ToString().Trim() +"'"; 
                String S1 = "Select ItemID,ItemDescription,UnitCost,SalesGLAccount,ItemClass from tblItemMaster";// where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //add anew row to dtagrid
                        dgvInventoryBeginBal.Rows.Add();
                        dgvInventoryBeginBal.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();//itemid
                        dgvInventoryBeginBal.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();//description
                        //===========================
                        // string WH = "CR001";
                        // String S2 = "Select QTY from tblItemWhse Where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "'and ItemId='" + dt.Rows[i].ItemArray[0].ToString().Trim() + "'";// and TranType='" + trantype + "'";// where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                        String S2 = "Select OPBQtry from tblItemWhse Where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "'and ItemId='" + dt.Rows[i].ItemArray[0].ToString().Trim().Replace("'", "''") + "' and TranType='" + trantype + "'";// where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);
                        if (dt2.Rows.Count > 0)
                        {
                            dgvInventoryBeginBal.Rows[i].Cells[2].Value = dt2.Rows[0].ItemArray[0].ToString().Trim();//Quantity
                            dgvInventoryBeginBal.Rows[i].Cells[5].Value = true;//apply is true mean assisig is done 
                        }
                        else
                        {
                            dgvInventoryBeginBal.Rows[i].Cells[2].Value = "0";//Quantity if item is not in the warehouse
                        }
                        dgvInventoryBeginBal.Rows[i].Cells[3].Value = Convert.ToDouble(dt.Rows[i].ItemArray[2]).ToString("N2");//unit cost
                        dgvInventoryBeginBal.Rows[i].Cells[4].Value = (Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) * Convert.ToDouble(dt.Rows[i].ItemArray[2])).ToString("N2");//toal cost
                        dgvInventoryBeginBal.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[3].ToString().Trim();//GL Account
                        dgvInventoryBeginBal.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[4].ToString().Trim();//ItemClass
                    }
                }

                S1 = "SELECT     ItemCode, Qty, LotNo, ExpiryDate, WH, Status FROM         tblItemWhse_LotItems where Status='OpbBal' ";// where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                cmd1 = new SqlCommand(S1);
                da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dtLot = new DataTable();
                da1.Fill(dtLot);

                _dataTableLotNo = new DataTable();
                

                if (_dataTableLotNo.Columns.Count == 0)
                {
                    _dataTableLotNo.Columns.Add("ItemCode");
                    _dataTableLotNo.Columns.Add("WH");
                    _dataTableLotNo.Columns.Add("LotNo");
                    _dataTableLotNo.Columns.Add("Status");
                    _dataTableLotNo.Columns.Add("ExpDate");
                    _dataTableLotNo.Columns.Add("Qty");
                }

                foreach (DataRow drLot in dtLot.Rows)
                {
                    DataRow drow = _dataTableLotNo.NewRow();
                    drow["ItemCode"] = drLot["ItemCode"].ToString();
                    drow["WH"] = drLot["WH"].ToString();
                    drow["LotNo"] = drLot["LotNo"].ToString();
                    drow["Qty"] = drLot["Qty"].ToString();
                    drow["Status"] = drLot["Status"].ToString();
                    drow["ExpDate"] = drLot["ExpiryDate"].ToString();
                    _dataTableLotNo.Rows.Add(drow);
                }

                clsSerializeItem.TblLotNos = _dataTableLotNo;

                //dgvInventoryBeginBal.Rows.RemoveAt(dgvInventoryBeginBal.Rows.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //===========================================
        private void UpdatethetextBox()
        {
            try
            {
                txtItemID.Text = dgvInventoryBeginBal[0, dgvInventoryBeginBal.CurrentRow.Index].Value.ToString().Trim();
                txtDescription.Text = dgvInventoryBeginBal[1, dgvInventoryBeginBal.CurrentRow.Index].Value.ToString().Trim();
                txtQuantity.Text = dgvInventoryBeginBal[2, dgvInventoryBeginBal.CurrentRow.Index].Value.ToString().Trim();
                if (Convert.ToBoolean(dgvInventoryBeginBal[5, dgvInventoryBeginBal.CurrentRow.Index].Value) == true)
                {
                    chkApply.Checked = true;
                }
                else
                {
                    chkApply.Checked = false;
                }
                txtUnitCost.Text = Convert.ToDouble(dgvInventoryBeginBal[3, dgvInventoryBeginBal.CurrentRow.Index].Value).ToString("N2");
                txtTotalCost.Text = Convert.ToDouble(dgvInventoryBeginBal[4, dgvInventoryBeginBal.CurrentRow.Index].Value).ToString("N2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //====================================================

        private void UpdatetheDatagridfromTex()
        {
            try
            {
                dgvInventoryBeginBal[0, dgvInventoryBeginBal.CurrentRow.Index].Value = txtItemID.Text.ToString();
                dgvInventoryBeginBal[1, dgvInventoryBeginBal.CurrentRow.Index].Value = txtDescription.Text.ToString();
                dgvInventoryBeginBal[2, dgvInventoryBeginBal.CurrentRow.Index].Value = txtQuantity.Text.ToString();
                dgvInventoryBeginBal[3, dgvInventoryBeginBal.CurrentRow.Index].Value = txtUnitCost.Text.ToString();
                dgvInventoryBeginBal[4, dgvInventoryBeginBal.CurrentRow.Index].Value = txtTotalCost.Text.ToString();
                if (chkApply.Checked == true)
                {
                    dgvInventoryBeginBal[5, dgvInventoryBeginBal.CurrentRow.Index].Value = true;
                }
                else
                {
                    dgvInventoryBeginBal[5, dgvInventoryBeginBal.CurrentRow.Index].Value = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearEnteringTexBoxes()
        {
            txtItemID.Text = "";
            txtDescription.Text = "";
            txtQuantity.Text = "0";
            txtUnitCost.Text = "0.00";
            txtTotalCost.Text = "0.00";
            chkApply.Checked = false;
        }

        public bool WarehoiseSelect = false;

        private void cmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearEnteringTexBoxes();
                WarehoiseSelect = true;
                LoadAddresses();
                Load_InvetoryListFromWarehouse();
                int rowCount = GetFilledRows();
                for (int a = rowCount; a < dgvInventoryBeginBal.Rows.Count; a++)
                {
                    dgvInventoryBeginBal.Rows.RemoveAt(a);
                }
                GeTotalBalances();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //get datagrid rowcount which are filled===========================================
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

        private bool IsSerialNoDuplicate()
        {
            try
            {
                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                {
                    if (dr["Status"].ToString() == "New")
                    {
                        String S = " SELECT WareHouseID, ItemID, SerialNO, TranType, Status FROM tblSerialItemTransaction " +
                            " WHERE     (Status = 'Available' and ItemID='" + dr["ItemCode"].ToString() + "'  and SerialNO='" + dr["SerialNo"].ToString() + "') or " +
                            " (TranType='Del-Note' and Status='OutOfStock' and ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "') or  " +
                            ////" (TranType='De' and Status='OutOfStock' and ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "') or  " +
                            " (TranType='Invoice' and Status='OutOfStock' and ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "') ";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                        DataSet dt = new DataSet();
                        da.Fill(dt);

                        if (dt.Tables[0].Rows.Count > 0)
                        {
                            MessageBox.Show("The Serial Number " + dr["SerialNo"].ToString() + " already Exists for the Item " + dr["ItemCode"].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dtpOPbBalnce.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dtpOPbBalnce.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!objControlers.HeaderValidation_Warehouse(ugCmbWarehouse.Text, sMsg))
                return;

            if (IsSerialNoDuplicate())
                return;

            double SellingPrice = 0.00;
            int rowCount = GetFilledRows();//get row count from data grid
            int chkNumber = 0;
            string TranType = "OpbBal";
            int DocType = 1;
            bool QtyIN = true;
            string Stataus = "Available";
            bool IsGRNProcess = true;
            bool IsItemSerial = false;
            int EffectedRows = 0;

            try
            {
                chkApply_Leave(sender, e);
                for (int a = 0; a < rowCount; a++)
                {
                    chkNumber = 0;//this variable check for numaric validation
                    Convert.ToDouble(dgvInventoryBeginBal.Rows[a].Cells[4].Value);
                    Convert.ToDouble(dgvInventoryBeginBal.Rows[a].Cells[5].Value);
                }
            }
            catch (Exception ex)
            {
                chkNumber = 1;//if this is ==1 then invalid input for numer field unitcost and Quantity
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
            if (chkNumber == 1 || IsItemSerial == true)
            {
                if (chkNumber == 1)
                {
                    MessageBox.Show("Enter valid Data");
                }
                if (IsItemSerial == true)
                {
                    MessageBox.Show("There are Serialize Stock Items you have not entered begining balances");
                }
            }
            else
            {
                setConnectionString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                myConnection.Open();
                SqlTransaction myTrans = myConnection.BeginTransaction();

                try
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        if (Convert.ToBoolean(dgvInventoryBeginBal.Rows[i].Cells[5].Value) == true)
                        {
                            DateTime DTP = Convert.ToDateTime(dtpOPbBalnce.Text);
                            string Dformat = "MM/dd/yyyy";
                            string OPBDate = DTP.ToString(Dformat);

                            double PeachtreeQty = 0.00;
                            double SumWarehouseQty = 0.00;                          

                            SqlCommand cmd2 = new SqlCommand("SELECT SUM(QTY) FROM tblItemWhse GROUP BY ItemId HAVING (ItemId = '" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "')", myConnection, myTrans);
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                            DataTable dt2 = new DataTable();
                            da2.Fill(dt2);
                            if (dt2.Rows.Count > 0)
                            {
                                SumWarehouseQty = Convert.ToDouble(dt2.Rows[0].ItemArray[0]);
                            }
                            SqlCommand cmd3 = new SqlCommand("SELECT dblQtyonhand FROM tblPeachtreeQtyOnhand where (StrItemID = '" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "')", myConnection, myTrans);
                            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                            DataTable dt3 = new DataTable();
                            da3.Fill(dt3);
                            if (dt3.Rows.Count > 0)
                            {
                                PeachtreeQty = Convert.ToDouble(dt3.Rows[0].ItemArray[0]);
                            }
                            //=================================================

                            SqlCommand cmd = new SqlCommand("select WhseId,ItemId,QTY, isnull(OPBQtry,0) as OPBQtry  from tblItemWhse where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemId='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "'", myConnection, myTrans);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {                              
                                if (Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString()) != Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value))
                                {
                                    double  dblUpQty = 0;
                                    if (Convert.ToDouble(dt.Rows[0].ItemArray[3]) < Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value))
                                    {
                                        dblUpQty = Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) - Convert.ToDouble(dt.Rows[0].ItemArray[3]);
                                        SqlCommand cmd1 = new SqlCommand("update tblItemWhse set QTY=QTY+'" + dblUpQty + "',OPBQtry='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "',UnitCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "',TotalCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[4].Value) + "' where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemId='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "'", myConnection, myTrans);

                                        SqlCommand cmd11 = new SqlCommand("update tbItemlActivity set Qty=Qty+'" + dblUpQty + "' where WarehouseID='" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "' and TranType='OpbBal'", myConnection, myTrans);

                                        //if (PeachtreeQty < SumWarehouseQty + dblUpQty)
                                        //{
                                        //    MessageBox.Show("Stock Differece Between Peachtree and Warehouse Module for Item ID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "' > Peachtree Quantity Available='" + PeachtreeQty + "' Warehouse Available Quantity='" + SumWarehouseQty + "' updated new Quantity='" + dblUpQty + "'", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        //    return;
                                        //}
                                        EffectedRows =cmd11.ExecuteNonQuery();
                                        EffectedRows = cmd1.ExecuteNonQuery();
                                    }
                                   if (Convert.ToDouble(dt.Rows[0].ItemArray[3]) > Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value))
                                    {
                                        dblUpQty = Convert.ToDouble(dt.Rows[0].ItemArray[3])-Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value);
                                        SqlCommand cmd1 = new SqlCommand("update tblItemWhse set QTY=QTY-'" + dblUpQty + "',OPBQtry='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "',UnitCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "',TotalCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[4].Value) + "' where WhseId='" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemId='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "'", myConnection, myTrans);

                                        SqlCommand cmd11 = new SqlCommand("update tbItemlActivity set Qty=Qty-'" + dblUpQty + "' where WarehouseID='" + ugCmbWarehouse.Value.ToString().Trim() + "' and ItemID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "' and TranType='OpbBal'", myConnection, myTrans);

                                        if (PeachtreeQty < SumWarehouseQty - dblUpQty)
                                        {
                                            //MessageBox.Show("Stock Differece Between Peachtree and Warehouse Module for Item ID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "' > Peachtree Quantity Available='" + PeachtreeQty + "' Warehouse Available Quantity='" + SumWarehouseQty + "' updated new Quantity='" + dblUpQty + "'", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            //return;
                                        }
                                        EffectedRows = cmd11.ExecuteNonQuery();
                                        EffectedRows = cmd1.ExecuteNonQuery();
                                    }  
                                }
                            }
                            else
                            {
                                if (PeachtreeQty < SumWarehouseQty + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value))
                                {
                                    //MessageBox.Show("Stock Differece Between Peachtree and Warehouse Module for Item ID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "' > Peachtree Quantity Available='" + PeachtreeQty + "' Warehouse Available Quantity='" + SumWarehouseQty + "' Entered Quantity='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "'", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //return;
                                }

                                SqlCommand cmd21 = new SqlCommand("Insert into tblItemWhse (WhseId,WhseName,ItemId,ItemDis,QTY,TraDate,UnitCost,TranType,TotalCost,OPBQtry) values ('" + ugCmbWarehouse.Value.ToString().Trim() + "','" + txtwarehouseName.Text.ToString().Trim() + "','" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "','" + dgvInventoryBeginBal.Rows[i].Cells[1].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "','" + OPBDate + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "','" + TranType + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[4].Value) + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + " ')", myConnection, myTrans);
                                EffectedRows = cmd21.ExecuteNonQuery();
                                
                                SqlCommand cmd11 = new SqlCommand("Insert into tbItemlActivity (OHQTY,DocType,TranNo,TransDate,TranType,DocReference,ItemID,Qty,UnitCost,TotalCost,WarehouseID,SellingPrice) values ('" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "','" + DocType + "','" + TranType + "','" + OPBDate + "','" + TranType + "','" + QtyIN + "','" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[2].Value) + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "','" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[4].Value) + "','" + ugCmbWarehouse.Value.ToString().Trim() + "','" + SellingPrice + "')", myConnection, myTrans);
                                cmd11.ExecuteNonQuery();                                

                                //update the Unit Cost from Irtem Master file
                                SqlCommand cmd31 = new SqlCommand("update tblItemMaster set UnitCost='" + Convert.ToDouble(dgvInventoryBeginBal.Rows[i].Cells[3].Value) + "'where  ItemID='" + dgvInventoryBeginBal.Rows[i].Cells[0].Value.ToString().Trim() + "'", myConnection, myTrans);
                                cmd31.ExecuteNonQuery();                  
                            }

                            if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                            {
                                for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                                {
                                    if (clsSerializeItem.DtsSerialNoList.Rows[j][0].ToString() == dgvInventoryBeginBal[0, i].Value.ToString())
                                    {
                                        SqlCommand myCommandSerial = new SqlCommand("delete  tblSerialItemTransaction where ItemID='" + 
                                            dgvInventoryBeginBal[0, i].Value + "' and WareHouseID='" + 
                                            ugCmbWarehouse.Value.ToString().Trim() + "' and TranType='" + 
                                            TranType + "'", myConnection, myTrans);
                                        SqlDataAdapter daSerial = new SqlDataAdapter(myCommandSerial);
                                        DataTable dtSerial = new DataTable();
                                        daSerial.Fill(dtSerial);

                                        break;
                                    }
                                }
                            }

                            SqlCommand myCommandSe1 = new SqlCommand("delete  tblItemWhse_LotItems where ItemCode='" + dgvInventoryBeginBal[0, i].Value + "' and WH='" + ugCmbWarehouse.Value.ToString().Trim() + "' and Status='" + TranType + "'", myConnection, myTrans);
                            SqlDataAdapter daSe1 = new SqlDataAdapter(myCommandSe1);
                            DataTable dtLot = new DataTable();
                            daSe1.Fill(dtLot);

                            myCommandSe1 = new SqlCommand("delete from tblLotItemTransactions where ItemCode='" + dgvInventoryBeginBal[0, i].Value.ToString() + "' and " +
                                       " WH='" + ugCmbWarehouse.Value.ToString().Trim() + "' and Status='OpbBal' ", myConnection, myTrans);
                            daSe1 = new SqlDataAdapter(myCommandSe1);
                            dtLot = new DataTable();
                            daSe1.Fill(dtLot);

                            for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                            {
                                if (clsSerializeItem.DtsSerialNoList.Rows[j][0].ToString() == dgvInventoryBeginBal[0, i].Value.ToString())
                                {
                                    //Insert item and serialNO into warehousewise Iitem Transaction Table=====================
                                    SqlCommand myCommandSe12 = new SqlCommand("insert into tblSerialItemTransaction(WareHouseID,ItemID,SerialNO,TranType,Status)values ('" + 
                                        ugCmbWarehouse.Value.ToString().Trim() + "','" + dgvInventoryBeginBal[0, i].Value.ToString() + "','" + 
                                        clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() + "','" + TranType + "','" + 
                                        Stataus + "')", myConnection, myTrans);
                                    myCommandSe12.ExecuteNonQuery();
                                }
                            }

                            for (int j = 0; j < clsSerializeItem.TblLotNos.Rows.Count; j++)
                            {
                                if (clsSerializeItem.TblLotNos.Rows[j][0].ToString() == dgvInventoryBeginBal[0, i].Value.ToString())
                                {
                                    SqlCommand myCommandLot = new SqlCommand("insert into tblItemWhse_LotItems(ItemCode, Qty, LotNo, ExpiryDate, WH, Status)values ('"
                                        + dgvInventoryBeginBal[0, i].Value.ToString() + "','" + clsSerializeItem.TblLotNos.Rows[j]["Qty"].ToString() + "','" +
                                        clsSerializeItem.TblLotNos.Rows[j]["LotNo"].ToString() + "','" + clsSerializeItem.TblLotNos.Rows[j]["ExpDate"].ToString() + "','" +
                                        ugCmbWarehouse.Value.ToString().Trim() + "','OpbBal')", myConnection, myTrans);
                                    myCommandLot.ExecuteNonQuery();                                   

                                    myCommandLot = new SqlCommand("insert into tblLotItemTransactions(RefNo, ItemCode,WH, TransDate, LotNo, Qty, ExpiryDate, Status)values ('OpbBal','" +
                                         dgvInventoryBeginBal[0, i].Value.ToString() + "','" + ugCmbWarehouse.Text.Trim() + "','" + dtpOPbBalnce.Value.ToString("MM/dd/yyyy")
                                        + "','" + clsSerializeItem.TblLotNos.Rows[j]["LotNo"].ToString() + "','" +                                        
                                        clsSerializeItem.TblLotNos.Rows[j]["Qty"].ToString() + "','" +
                                        clsSerializeItem.TblLotNos.Rows[j]["ExpDate"].ToString() + "','OpbBal')", myConnection, myTrans);
                                    myCommandLot.ExecuteNonQuery();
                                }
                            } 
                        }
                    }

                    if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                    {
                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //objfrmSerialAddCommon.SaveSerialNos(myConnection, myTrans, _dataTable, "OpbBal", false, ugCmbWarehouse.Text.Trim());
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "OpbBal", ugCmbWarehouse.Text.ToString(), string.Empty, dtpInvDate.Value, false,"");
                    }
                    myTrans.Commit();
                    _dataTable.Rows.Clear();
                    _dataTableLotNo.Rows.Clear();
                    clsSerializeItem.DtsSerialNoList.Rows.Clear();
                    clsSerializeItem.TblLotNos.Rows.Clear();
                    if (EffectedRows > 0)
                        MessageBox.Show("Opening Balances Entered Successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    myTrans.Rollback();
                    objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
                }
                finally
                {
                    myConnection.Close();
                }
            }
        }

        private void TextBoxTotalCalculation()
        {
            try
            {
                quantity = 0.0;
                unitCost = 0.0;
                totalCost = 0.0;
                if (txtUnitCost.Text.Trim() == string.Empty) txtUnitCost.Text = "0.00";
                if (txtQuantity.Text.Trim() == string.Empty) txtQuantity.Text = "0";

                quantity = Convert.ToDouble(txtQuantity.Text);
                unitCost = Convert.ToDouble(txtUnitCost.Text);
                totalCost = quantity * unitCost;

                txtUnitCost.Text = unitCost.ToString("N2");
                txtTotalCost.Text = totalCost.ToString("N2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            // TextBoxTotalCalculation();
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            // TextBoxTotalCalculation();
        }

        private void txtTotalCost_TextChanged(object sender, EventArgs e)
        {
            ///TextBoxTotalCalculation();
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
                RowLeaveEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
                RowLeaveEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtTotalCost_Leave(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
                RowLeaveEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvInventoryBeginBal_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtQuantity_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (ugCmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Warehouse First");
                    return;
                }

                if (Convert.ToDouble(txtQuantity.Text) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + txtItemID.Text.ToString().Trim() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            txtQuantity.Focus();
                        }
                    }
                }
                else
                {                   
                    frmSerialAddCommon ObjfrmSerialAddCommon = new frmSerialAddCommon("OpbBal", ugCmbWarehouse.Text.ToString().Trim(),
                        txtItemID.Text.ToString().Trim(),
                        Convert.ToDouble(txtQuantity.Text),"OpbBal",
                        false, txtDescription.Text.Trim(), _dataTable);
                    ObjfrmSerialAddCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void RowLeaveEvent()
        {
            try
            {
                if (WarehoiseSelect == false)
                {
                    if (txtItemID.Text != "" || txtDescription.Text != "" || txtQuantity.Text != "")
                    {
                        CheckSerialNumberEntering();
                        if (IsItemSerial == true)
                        {
                            DialogResult reply1 = MessageBox.Show("The number of serial numbers entered for Item ID '" + txtItemID.Text.ToString() + "' must match the begining Balance Quantity. Click OK to Enter Serial Numbers now", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            {
                                if (reply1 == DialogResult.OK)
                                {
                                    //ClassDriiDown.SerialItemID = ObjSerialInvoice.GetItemId(txtItemID.Text.ToString().Trim());
                                    //ClassDriiDown.SerialDescription = ObjSerialInvoice.GetItemDescription(txtDescription.Text.ToString().Trim());
                                    //ClassDriiDown.ReceivedQty = ObjSerialInvoice.GetRewceivedQty(Convert.ToDouble(txtQuantity.Text));
                                    //ClassDriiDown.SerialLocation = ObjSerialInvoice.GetLocation(ugCmbWarehouse.Value.ToString().Trim());

                                    frmSerialAddCommon objSerialB = new frmSerialAddCommon("OpbBal", ugCmbWarehouse.Text.ToString().Trim(),
                        txtItemID.Text.ToString().Trim(),
                        Convert.ToDouble(txtQuantity.Text),
                        string.Empty,
                        false, txtDescription.Text.Trim(), _dataTable);
                                    objSerialB.Show();
                                }
                            }
                        }
                        else
                        {
                            UpdatetheDatagridfromTex();
                            GeTotalBalances();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvInventoryBeginBal_RowLeave(object sender, DataGridViewCellEventArgs e)
        {

            //if (WarehoiseSelect == false)
            //{
            //    if (txtItemID.Text != "" || txtDescription.Text != "" || txtQuantity.Text != "")
            //    {
            //        CheckSerialNumberEntering();
            //        if (IsItemSerial == true)
            //        {
            //            DialogResult reply1 = MessageBox.Show("The number of serial numbers entered for Item ID '" + txtItemID.Text.ToString() + "' must match the begining Balance Quantity. Click OK to Enter Serial Numbers now", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //            {
            //                if (reply1 == DialogResult.OK)
            //                {
            //                    ClassDriiDown.SerialItemID = ObjSerialInvoice.GetItemId(txtItemID.Text.ToString().Trim());
            //                    ClassDriiDown.SerialDescription = ObjSerialInvoice.GetItemDescription(txtDescription.Text.ToString().Trim());
            //                    ClassDriiDown.ReceivedQty = ObjSerialInvoice.GetRewceivedQty(Convert.ToDouble(txtQuantity.Text));
            //                    ClassDriiDown.SerialLocation = ObjSerialInvoice.GetLocation(cmbWarehouse.Text.ToString().Trim());

            //                    frmSerialBeginingBal objSerialB = new frmSerialBeginingBal();
            //                    objSerialB.Show();
            //                }

            //            }
            //        }
            //        else
            //        {
            //            UpdatetheDatagridfromTex();
            //            GeTotalBalances();
            //        }
            //    }
            //}
        }

        private void SetSelectdRow()
        {
            try
            {
                WarehoiseSelect = false;
                UpdatethetextBox();
                if (IsThisItemSerial() == true)
                {
                    btnSNO.Enabled = true;
                }
                else
                {
                    btnSNO.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvInventoryBeginBal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SetSelectdRow();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private bool IsThisItemSerial()
        {
            try
            {
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + txtItemID.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ItemClass == "10" || ItemClass == "11")
                {
                    IsThisItemSerial = true;
                }
                return IsThisItemSerial;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //=====================================================================
        private bool DatagridCheckItemSerial(string ItemID)
        {
            try
            {
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + ItemID + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ItemClass == "10" || ItemClass == "11")
                {
                    IsThisItemSerial = true;
                }
                return IsThisItemSerial;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CheckSerialNumberEntering()
        {
            int rowCount = GetFilledRows();//get row count from data grid
            IsItemSerial = false;
            //check wether this item is serialized or not=======================
            try
            {
                string ItemClass = "";
                String S = "Select * from tblItemMaster where ItemID  = '" + txtItemID.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {
                    ItemClass = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ItemClass == "10" || ItemClass == "11")
                {
                    IsItemSerial = true;
                    if (Convert.ToDouble(txtQuantity.Text) > 0)
                    {
                        //=======================================
                        string TranType = "OpbBal";
                        String S12 = "Select SerialNO from tblSerialItemTransaction where ItemID  = '" + txtItemID.Text.ToString().Trim() + "' and WareHouseID='" + ugCmbWarehouse.Value.ToString().Trim() + "' and TranType='" + TranType + "'";
                        SqlCommand cmd12 = new SqlCommand(S12);
                        SqlDataAdapter da12 = new SqlDataAdapter(S12, ConnectionString);
                        DataSet dt12 = new DataSet();
                        da12.Fill(dt12);

                        if (Convert.ToDouble(txtQuantity.Text) == dt12.Tables[0].Rows.Count)
                        {
                            IsItemSerial = false;
                        }
                        else
                        {               
                             DataView dv1 = clsSerializeItem.DtsSerialNoList.DefaultView;
                             if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                             {
                                 dv1.RowFilter = "ItemCode='" + txtItemID.Text.Trim() + "'";
                                 DataTable dtbl = dv1.ToTable();
                                 if (Convert.ToDouble(txtQuantity.Text) == dtbl.Rows.Count)
                                 {
                                     IsItemSerial = false;
                                 }
                             }
                        }
                    }
                    else
                    {
                        IsItemSerial = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvInventoryBeginBal_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //if (WarehoiseSelect == false)
            //{
            //    if (txtItemID.Text != "" || txtDescription.Text != "" || txtQuantity.Text != "")
            //    {
            //        CheckSerialNumberEntering();
            //        if (IsItemSerial == true)
            //        {
            //            DialogResult reply1 = MessageBox.Show("The number of serial numbers entered for Item ID '" + txtItemID.Text.ToString() + "' must match the begining Balance Quantity. Click OK to Enter Serial Numbers now", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //            {
            //                if (reply1 == DialogResult.OK)
            //                {
            //                    ClassDriiDown.SerialItemID = ObjSerialInvoice.GetItemId(txtItemID.Text.ToString().Trim());
            //                    ClassDriiDown.SerialDescription = ObjSerialInvoice.GetItemDescription(txtDescription.Text.ToString().Trim());
            //                    ClassDriiDown.ReceivedQty = ObjSerialInvoice.GetRewceivedQty(Convert.ToDouble(txtQuantity.Text));
            //                    ClassDriiDown.SerialLocation = ObjSerialInvoice.GetLocation(cmbWarehouse.Text.ToString().Trim());

            //                    frmSerialBeginingBal objSerialB = new frmSerialBeginingBal();
            //                    objSerialB.Show();
            //                }

            //            }
            //        }
            //        else
            //        {
            //            UpdatetheDatagridfromTex();
            //            GeTotalBalances();
            //        }
            //    }
            //}
        }

        private void dgvInventoryBeginBal_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                UpdatethetextBox();
                if (IsThisItemSerial() == true)
                {
                    btnSNO.Enabled = true;
                }
                else
                {
                    btnSNO.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvInventoryBeginBal_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                UpdatethetextBox();
                if (IsThisItemSerial() == true)
                {
                    btnSNO.Enabled = true;
                }
                else
                {
                    btnSNO.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvInventoryBeginBal_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    WarehoiseSelect = false;
            //    UpdatethetextBox();
            //    if (IsThisItemSerial() == true)
            //    {
            //        btnSNO.Enabled = true;
            //    }
            //    else
            //    {
            //        btnSNO.Enabled = false;
            //    }
            //}
            //catch { }
        }

        private void txtQuantity_TabIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtUnitCost_TabIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtTotalCost_TabIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                
                frmBeginingBalReport ObjBegBalRep = new frmBeginingBalReport();
                ObjBegBalRep.Show();

                //String S3 = "Select * from tblItemWhse";// where InvoiceNo = '" + txtInvoiceNo.Text.ToString().Trim() + "'";// AND Refund <> '" + Ref + "'";
                //SqlCommand cmd3 = new SqlCommand(S3);
                //SqlConnection con3 = new SqlConnection(ConnectionString);
                //SqlDataAdapter da3 = new SqlDataAdapter(S3, con3);
                //da3.Fill(DSInvoicing, "dtInvoiceData");


                //frmInvoicePrint prininv = new frmInvoicePrint(this);
                //prininv.Show();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void chkApply_Leave(object sender, EventArgs e)
        {
            try
            {
                WarehoiseSelect = false;
                TextBoxTotalCalculation();
                RowLeaveEvent();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ugCmbWarehouse_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (ugCmbWarehouse.Value != null)
                {
                    //chs
                    ClearEnteringTexBoxes();
                    WarehoiseSelect = true;
                    LoadAddresses();
                    Load_InvetoryListFromWarehouse();
                    int rowCount = GetFilledRows();
                    for (int a = rowCount; a < dgvInventoryBeginBal.Rows.Count; a++)
                    {
                        dgvInventoryBeginBal.Rows.RemoveAt(a);
                    }
                    GeTotalBalances();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ugCmbItem_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (ugCmbItem.Value == null) return;

                if (ugCmbWarehouse.Value == null || ugCmbWarehouse.Value.ToString().Trim().Length == 0)
                {
                    MessageBox.Show("Select a Warehouse..!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int rowCount = GetFilledRows();//get row count from data grid

                for (int i = 0; i < rowCount; i++)
                {
                    if (dgvInventoryBeginBal[0, i].Value.ToString().Contains(ugCmbItem.Value.ToString().Trim()))
                    {
                        dgvInventoryBeginBal.CurrentCell = dgvInventoryBeginBal[0, i];
                        break;
                    }
                }
                SetSelectdRow();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ugCmbItem_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtQuantity, ugCmbItem, e);
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtUnitCost, ugCmbItem, e);
        }

        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(chkApply, txtQuantity, e);
        }

        private void ugCmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(ugCmbItem, ugCmbWarehouse, e);
        }
        


        
        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
              
                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
                {
                    e.Handled = true;
                    return;
                }

             
                if (e.KeyChar == 46)
                {
                    if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            frmLotNoEntry objfrmLotNoEntry = new frmLotNoEntry(txtItemID.Text.Trim().ToString(), ugCmbWarehouse.Value.ToString(), double.Parse(txtQuantity.Text.Trim()), "OpbBal", "OpbBal",_dataTableLotNo);
            objfrmLotNoEntry.ShowDialog();
        }

        private void txtItemID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtItemID.Text.Trim() != string.Empty)
                {
                    String S = "Select isnull(IsLotItem,'False') as IsLotItem from tblItemMaster where ItemID  = '" + 
                        txtItemID.Text.ToString().Trim() + "'";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataSet dt = new DataSet();
                    da.Fill(dt);

                    if (bool.Parse(dt.Tables[0].Rows[0]["IsLotItem"].ToString()))
                        btnLotItem.Enabled = true;
                    else
                        btnLotItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }



        //===================================================
    }
}