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
    public partial class frmInventotyAdjustment : Form
    {
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon();

        public frmInventotyAdjustment()
        {
            InitializeComponent();
            setConnectionString();
            cmbWarehouse.Focus();
        }

        public string StrSql;
        public double dblOnHandQty = 0.00;
        public double dblAdjustQty = 0.00;
        public double dblNewQuantity = 0.00;
        public double dblUnitCost = 0.00;
        public double dblTotalCost = 0.00;

        public DataSet dsCustomer;
        public DataSet dsItem;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        public static string ConnectionString;
        public string sMsg = "MultiWarehouse Module - Inventory Adjustments";
        public dsAPCommon dsObjAdjustments = new dsAPCommon();
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

        public void GetARAccount()
        {
            dsAR = new DataSet();
            try
            {
                dsAR.Clear();
                StrSql = " SELECT AcountID, AccountDescription FROM tblChartofAcounts order by AcountID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsAR, "DtAR");

                cmbARAccount.DataSource = dsAR.Tables["DtAR"];
                cmbARAccount.DisplayMember = "AcountID";
                cmbARAccount.ValueMember = "AcountID";
                cmbARAccount.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
                cmbARAccount.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 150;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmInventotyAdjustment_Load(object sender, EventArgs e)
        {
            try
            {
                ClearDetils();
                filJob();
                GetWareHouseDataSet();//Infragistics
                GetARAccount();//Infragistics
                cmbWarehouse.Focus();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void filJob()
        {
            try
            {
                clsCommonFunc objclsCommonFunc = new clsCommonFunc();
                objclsCommonFunc.fillSites(ucbJob);
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
                if (cmbWarehouse.Value == null) return;
                dsItem.Clear();
                //StrSql = "SELECT ItemId, ItemDis, isnull(QTY,0.00)as QTY, isnull(UnitCost,0.00) as UnitCost FROM tblItemWhse  where WhseId='" + cmbWarehouse.ActiveRow.Cells[0].Value.ToString() + "'order by ItemId";

                //SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                //dAdapt.Fill(dsItem, "DtItem");

                //ugCmbItem.DataSource = dsItem.Tables["DtItem"];
                //ugCmbItem.DisplayMember = "ItemId";
                //ugCmbItem.ValueMember = "ItemId";
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemId"].Width = 100;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDis"].Width = 200;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["QTY"].Width = 75;
                //ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["UnitCost"].Width = 75;


                //asanga
                StrSql = "SELECT ItemID, ItemDescription, isnull(QTY,0.00)as QTY, isnull(UnitCost,0.00) as UnitCost FROM dbo.View_StkAdjesment  where WarehouseID='" + cmbWarehouse.ActiveRow.Cells[0].Value.ToString() + "' AND ItemID IS NOT NULL  order by ItemDescription";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsItem, "DtItem");

                ugCmbItem.DataSource = dsItem.Tables["DtItem"];
                ugCmbItem.DisplayMember = "ItemDescription";
                ugCmbItem.ValueMember = "ItemId";
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemId"].Width = 100;
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["ItemDescription"].Width = 200;
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["QTY"].Width = 75;
                ugCmbItem.DisplayLayout.Bands["DtItem"].Columns["UnitCost"].Width = 75;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetWareHouseDataSet()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 150;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearDetils()
        {
            ClearDecimalField();
            cmbWarehouse.Text = user.StrDefaultWH;
            cmbARAccount.Text = user.AdjustGL;
            ugCmbItem.Text = "";
            txtReference.Text = "";
            txtWarehouseName.Text = "";
            cmbWarehouse.Focus();
            dtpInvAdjustment.Value = user.LoginDate;
            clsSerializeItem.DtsSerialNoList.Rows.Clear();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearDetils();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbehouseehouseID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ClearDecimalField()
        {
            txtDescription.Text = "";
            txtOnHandQty.Text = "0.00";
            txtAdustQty.Text = "0.00";
            txtNewQty.Text = "0.00";
            txtUnitCost.Text = "0.00";
            dblOnHandQty = 0.00;
            dblAdjustQty = 0.00;
            dblNewQuantity = 0.00;
            dblUnitCost = 0.00;
            dblTotalCost = 0.00;
        }

        private void cmbItemID_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void txtAdustQty_TextChanged(object sender, EventArgs e)
        {
            clsSerializeItem.DtsSerialNoList.Rows.Clear();
            txtAdustQty_Leave(sender, e);
        }

        private void CalculateNewQuantity()
        {
            try
            {
                dblOnHandQty = Convert.ToDouble(txtOnHandQty.Text);
                dblAdjustQty = Convert.ToDouble(txtAdustQty.Text);
                dblNewQuantity = dblOnHandQty + dblAdjustQty;
                txtNewQty.Text = dblNewQuantity.ToString("N2");

                if (txtUnitCost.Text == null || txtUnitCost.Text == string.Empty)
                    txtUnitCost.Text = "0.00";
                dblUnitCost = Convert.ToDouble(txtUnitCost.Text);
                dblTotalCost = dblUnitCost * dblAdjustQty;
            }
            catch (Exception ex)
            {
                txtAdustQty.Text = "0";
            }
        }

        private void txtAdustQty_Leave(object sender, EventArgs e)
        {
            try
            {
                CalculateNewQuantity();
            }
            catch (Exception ex)
            {
                //  objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
               
            }
        }

        private void txtAdustQty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CalculateNewQuantity();
                }
                objControlers.FocusControl(txtReasonToAdjust, txtAdustQty, e);
            }
            catch (Exception ex)
            {
                //  objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
              
            }
        }

        private void cmbehouseehouseID_Leave(object sender, EventArgs e)
        {

        }

        private void cmbWarehouse_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtWarehouseName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {
            try
            {     
                //((Control)sender).Handle.
                //((Control)sender).Name;
                GetItemDataset();//Infragistics
            }
            catch (Exception ex)
            {
                //ex.Source
                objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ugCmbItem_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        txtDescription.Text = ugCmbItem.ActiveRow.Cells[1].Value.ToString();
                        txtOnHandQty.Text = ugCmbItem.ActiveRow.Cells[2].Value.ToString();
                        txtNewQty.Text = ugCmbItem.ActiveRow.Cells[2].Value.ToString();
                        txtUnitCost.Text = ugCmbItem.ActiveRow.Cells[3].Value.ToString();
                        //txtAdustQty.Text = "0.00";
                    }

                    if (IsThisItemSerial())
                        btnSNO.Enabled = true;
                    else
                        btnSNO.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private bool IsThisItemSerial()
        {
            try
            {
                if (ugCmbItem.Text == null) return false;
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + ugCmbItem.Text.Trim() + "'";
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

        private bool IsSerialNoDuplicate()
        {
            try
            {
                foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                {
                    String S = " SELECT WareHouseID, ItemID, SerialNO, TranType, Status FROM tblSerialItemTransaction " +
                        " WHERE     (Status = 'Available' and ItemID='" + dr["ItemCode"].ToString() + "'  and SerialNO='" + dr["SerialNo"].ToString() + "') or " +
                        " (TranType='Del-Note' and Status='OutOfStock' and ItemID='" + dr["ItemCode"].ToString() + "' and SerialNO='" + dr["SerialNo"].ToString() + "') or  " +
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
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public string GetInvNoField(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT AdjustPref, AdjustPad, AdjustNum FROM tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }
                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    return StrInvNo + StrInV.Substring(1, intX);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;
                StrSql = "SELECT  TOP 1(AdjustNum) FROM tblDefualtSetting ORDER BY AdjustNum DESC";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intInvNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intInvNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET AdjustNum='" + intInvNo + "'";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void SaveEvent()
        {
            //txtAdustQty.e
            if (cmbWarehouse.Text == "")
            {
                MessageBox.Show("You must select a Warehouse", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ugCmbItem.Text == "")
            {
                MessageBox.Show("You must select an Item", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dblAdjustQty == 0)
            {
                MessageBox.Show("You must enter Adjust Quantity", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbARAccount.Value == null || cmbARAccount.Value.ToString().Trim() == string.Empty)
            {
                MessageBox.Show("Select a GL Account...!","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            if (!IsSerialNoCorrect())
                return;

            if (double.Parse(txtAdustQty.Text.Trim()) > 0)
            {
                if (IsSerialNoDuplicate())
                    return;
            }

            if (!objControlers.HeaderValidation_AccountID(cmbARAccount.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_Warehouse(cmbWarehouse.Text, sMsg)) return;
            if (!objControlers.HeaderValidation_ItemID(ugCmbItem.Value.ToString(), sMsg)) return;
           // if (!objControlers.HeaderValidation_JobID(ucbJob.Text, sMsg)) return;

            string StrReference = null;
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                StrReference = GetInvNoField(myConnection, myTrans);

                UpdatePrefixNo(myConnection, myTrans);
                txtReference.Text = StrReference;
               // dblAdjustQty = Math.Abs(dblAdjustQty);
                SaveDetails(StrReference, cmbWarehouse.Value.ToString(), txtWarehouseName.Text.ToString().Trim(), ugCmbItem.Value.ToString().Trim(), txtDescription.Text.ToString().Trim(), dblUnitCost, dblOnHandQty, dblAdjustQty, dblNewQuantity, txtReasonToAdjust.Text.ToString().Trim(), cmbARAccount.Text.ToString().Trim(), myConnection, myTrans);
                UpdateItemWarehouse(ugCmbItem.Value.ToString().Trim(), txtDescription.Text.ToString().Trim(), cmbWarehouse.Value.ToString(), dblAdjustQty, dblUnitCost, dblTotalCost, myConnection, myTrans);
                InvTransaction(StrReference, ugCmbItem.Value.ToString().Trim(), cmbWarehouse.Value.ToString(), cmbWarehouse.Value.ToString(), dblAdjustQty, dblUnitCost, dblTotalCost, myConnection, myTrans);

                for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                {
                    if (clsSerializeItem.DtsSerialNoList.Rows[j][0].ToString() == ugCmbItem.Text.Trim().ToString())
                    {
                        if (double.Parse(txtAdustQty.Text.Trim()) > 0)
                        {
                            //Insert item and serialNO into warehousewise Iitem Transaction Table=====================
                            SqlCommand myCommandSe12 = new SqlCommand("insert into tblSerialItemTransaction(WareHouseID,ItemID,SerialNO,TranType,Status)values ('" +
                                cmbWarehouse.Value.ToString().Trim() + "','" + ugCmbItem.Text.Trim().ToString() + "','" +
                                clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() + "','IN-InvAdjust','Available')", myConnection, myTrans);
                            myCommandSe12.ExecuteNonQuery();
                        }
                        else
                        {
                            //Insert item and serialNO into warehousewise Iitem Transaction Table=====================
                            SqlCommand myCommandSe12 = new SqlCommand("update tblSerialItemTransaction set TranType='OUT-InvAdjust',Status='OutOfStock' " +
                                "where WareHouseID='" + cmbWarehouse.Text.Trim() + "' and ItemID='" + ugCmbItem.Text.Trim() 
                                + "' and SerialNO='" + clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() + "' ", 
                                myConnection, myTrans);
                            myCommandSe12.ExecuteNonQuery();
                        }
                    }
                }

                if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                {
                    CreatAdjustmentXMLFileToPeachtree_MultipleSerialNos(clsSerializeItem.DtsSerialNoList, StrReference, ugCmbItem.Value.ToString(), cmbARAccount.Value.ToString().Trim(), dblAdjustQty, dblUnitCost, dblTotalCost, myConnection, myTrans);

                    if (double.Parse(txtAdustQty.Text.Trim()) > 0)
                    {
                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //objfrmSerialAddCommon.SaveSerialNos(myConnection, myTrans, _dataTable, "OpbBal", false, ugCmbWarehouse.Text.Trim());
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList,
                            "IN-InvAdjust", cmbWarehouse.Text.ToString(), txtReference.Text.ToString().Trim(), dtpInvAdjustment.Value, false, "");
                    }
                    else
                    {
                        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                        //objfrmSerialAddCommon.SaveSerialNos(myConnection, myTrans, _dataTable, "OpbBal", false, ugCmbWarehouse.Text.Trim());
                        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList,
                            "OUT-InvAdjust", cmbWarehouse.Text.ToString(), txtReference.Text.ToString().Trim(), dtpInvAdjustment.Value, true, "");
                    }


                }
                else
                {
                    CreatAdjustmentXMLFileToPeachtree(StrReference, ugCmbItem.Value.ToString(), cmbARAccount.Value.ToString().Trim(), dblAdjustQty, dblUnitCost, dblTotalCost, myConnection, myTrans);
                }
                //ImportSalesInvoice(intGrid, StrReference);
                myTrans.Commit();
                MessageBox.Show("Adjustment Successfuly Saved.", "Information", MessageBoxButtons.OK);
                ClearDetils();
                //Print(StrReference);
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);                
                throw ex;
                //throw;
            }
        }

        //following code segment updated the itemwise table ==========================
        private void UpdateItemWarehouse(string StrItemCode, string StrItemDesc, string StrWarehouse, double dblQty, double DblUnitCost, double DblTotalCost, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (dblQty < 0)
                {
                    StrSql = "UPDATE tblItemWhse SET QTY=QTY -" + Math.Abs(dblQty) + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                if (dblQty > 0)
                {
                    StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //following Code segment Insert to the item Activity table==============
        private void InvTransaction(string strInvoiceNo, String StrItemCode, string StrLocCode, string StrLocCodeTo, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                bool DocRef = false;
                // int DocRef = 0;
                if (dblQuantity > 0)
                {
                    DocRef = true;
                }
                if (dblQuantity != 0)
                {
                    StrSql = "declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + StrLocCode + "' AND ItemId='" + StrItemCode + "') " +
                        " INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(@OHQTY,6,'" + strInvoiceNo + "','" + GetDateTime(dtpInvAdjustment.Value) + "','InvAdjust','" + DocRef + "','" + StrItemCode + "'," + Math.Abs(dblQuantity) + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);
        }

        private void SaveDetails(string StrAdjusmentId, string StrWarehouseId, string StrWarehouseName, string StrItemID, string StrItemdescription, double dblUnitCost, double dblOnhandQty, double dblAdjustQty, double dblNewQty, string StrReasonToAdjust, string StrGLAccount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblInventoryAdjustment(AdjusmentId,Date,WarehouseId,WarehouseName,ItemID, " +
                         " Itemdescription,UnitCost,OnhandQty,AdjustQty, " +
                         " NewQty,ReasonToAdjust,GLAccount,JobNo) " +
                         " VALUES ('" + StrAdjusmentId + "','" + GetDateTime(dtpInvAdjustment.Value) + "','" + StrWarehouseId + "'," +
                         " '" + StrWarehouseName + "','" + StrItemID + "'," +
                         " '" + StrItemdescription + "','" + dblUnitCost + "','" + dblOnhandQty + "'," +
                         " '" + Math.Abs(dblAdjustQty) + "','" + dblNewQty + "','" + StrReasonToAdjust + "','" + StrGLAccount + "','"+ ucbJob.Text.ToString().Trim()+"')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
                //if (dgvGRNTransaction.CurrentRow.Cells[0].Value == null) return false;
                //mmm
                bool IsThisItemSerial = false;
                string ItemClass = "";
                //================================
                String S = "Select * from tblItemMaster where ItemID  = '" + _ItemCode + "'";
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

        public bool IsSerialNoCorrect()
        {
            try
            {
                //DataTable table = DataSet1.Tables["Orders"];
                int _Count = 0;
                // Presuming the DataTable has a column named Date. 
                string expression;
                //foreach (UltraGridRow dgvr in ug.Rows)
                {
                    if (ugCmbItem.Value != null)
                    {
                        if (IsThisItemSerial(ugCmbItem.Value.ToString().Trim()))
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + ugCmbItem.Value.ToString().Trim(), "Inventory Adjustment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + ugCmbItem.Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(txtAdustQty.Text.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count !=Math.Abs(double.Parse(txtAdustQty.Text.ToString())))
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + ugCmbItem.Value.ToString().Trim(), "Inventory Adjustment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            txtDescription.Focus();
            
            //txtAdustQty.lo
            try
            {
                if (dtpInvAdjustment.Value < user.Period_begin_Date)
                {
                    MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (dtpInvAdjustment.Value > user.Period_End_Date)
                {
                    MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


                btnSave.Select();
               // txtAdustQty.Focused = false;
                if (txtReference.Text.Trim().Length > 0)
                {
                    MessageBox.Show("Not allowed to edit a Adjustment...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SaveEvent();
            }
            catch 
            {
               // objclsCommon.ErrorLog("Inventory Adjustment", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //the following code segment impoert Adjustment data to peachtree==============

        private void CreatAdjustmentXMLFileToPeachtree(string StrReference, string StrItemID, string StrGLAccount, double dblQuantity, double dblUnitCost, double dblTotalCost, SqlConnection myConnection, SqlTransaction myTrans)
        {
            //Connector Conn = new Connector();

            //XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\Adjustment.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;

            Connector Conn = new Connector();
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Adjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Item");
            Writer.WriteAttributeString("xsi:type", "paw:item");
            Conn.ImportItemUnitCost(StrItemID);
            // Writer.WriteEndElement();//get Last Unit Cost

            //crate a ID element (tag)=====================(1)
            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrItemID);
            Writer.WriteEndElement();

            //this sis crating tag for reference============(2)
            Writer.WriteStartElement("Reference");
            Writer.WriteString(StrReference);
            Writer.WriteEndElement();

            Writer.WriteStartElement("JobID");
            Writer.WriteString(ucbJob.Text.ToString().Trim());
            Writer.WriteEndElement();

            //This is a Tag for Adjusment Date==============(3)
            Writer.WriteStartElement("Date ");
            Writer.WriteAttributeString("xsi:type", "paw:date");
            Writer.WriteString(GetDateTime(dtpInvAdjustment.Value));//Date format must be (MM/dd/yyyy)
            Writer.WriteEndElement();//Serial_Number

            //if (SerialNo != string.Empty)
            //{
            //    Writer.WriteStartElement("Serial_Number");
            //    Writer.WriteString(SerialNo);
            //    Writer.WriteEndElement();
            //}
            //This is a Tag for numberof dsistribution=======(4)

            Writer.WriteStartElement("Number_of_Distributions ");
            Writer.WriteString("1");
            Writer.WriteEndElement();

            //Adjustmne Lines=================================(5)
            Writer.WriteStartElement("AdjustmentItems");
            //Adjustmne Lines=================================(6)
            Writer.WriteStartElement("AdjustmentItem");

            //Gl ASccount======================================(7)
            Writer.WriteStartElement("GLSourceAccount ");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrGLAccount);
            Writer.WriteEndElement();

            string LastUnitCost = "0";
            //String S1 = "Select LastUnitCost from tblLastUnitCost where ItemID='" + dgvItemList[0, k].Value.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand("Select LastUnitCost from tblLastUnitCost where ItemID='" + StrItemID + "'", myConnection, myTrans);
            SqlDataAdapter da12 = new SqlDataAdapter(cmd1);
            DataSet dt12 = new DataSet();
            DataTable dt = new DataTable();
            da12.Fill(dt);
            da12.Fill(dt12);
            if (dt.Rows.Count > 0)
            {
                LastUnitCost = dt12.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
            }

            double Totalcost = 0;
            if (Convert.ToDouble(LastUnitCost) == dblUnitCost || dblUnitCost == 0)
            {
                Writer.WriteStartElement("UnitCost");
                Writer.WriteString(LastUnitCost);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Quantity");
                Writer.WriteString(dblQuantity.ToString());
                // Writer.WriteString(Math.Abs(dblQuantity).ToString());
                // Math.Abs(dblQuantity)

                Writer.WriteEndElement();
                Totalcost = Convert.ToDouble(LastUnitCost) * Math.Abs(dblQuantity);

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + Totalcost.ToString());
                Writer.WriteEndElement();

            }
            else
            {
                Writer.WriteStartElement("UnitCost");
                Writer.WriteString(dblUnitCost.ToString());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Quantity");
                Writer.WriteString(dblQuantity.ToString());
                // Writer.WriteString(Math.Abs(dblQuantity).ToString());
                Writer.WriteEndElement();

                Totalcost = dblUnitCost * Math.Abs(dblQuantity);

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + Totalcost.ToString());
                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();//Adjustment Line
            Writer.WriteEndElement();//Adjustment lines

            Writer.WriteEndElement();//Item Closed Tag
            Writer.Close();//finishing writing xml file
            Conn.IssueAdjustmentExport("Adjustment.xml");
        }

        private void CreatAdjustmentXMLFileToPeachtree_MultipleSerialNos(DataTable _dtblSerialNos, string StrReference, string StrItemID, string StrGLAccount, double dblQuantity, double dblUnitCost, double dblTotalCost, SqlConnection myConnection, SqlTransaction myTrans)
        {
            //Connector Conn = new Connector();

            //XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\Adjustment.xml", System.Text.Encoding.UTF8);
            //Writer.Formatting = Formatting.Indented;

            Connector Conn = new Connector();
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Adjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            foreach (DataRow dr in _dtblSerialNos.Rows)
            {
                Writer.WriteStartElement("PAW_Item");
                Writer.WriteAttributeString("xsi:type", "paw:item");
                Conn.ImportItemUnitCost(StrItemID);
                // Writer.WriteEndElement();//get Last Unit Cost

                //crate a ID element (tag)=====================(1)
                Writer.WriteStartElement("ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(StrItemID);
                Writer.WriteEndElement();

                //this sis crating tag for reference============(2)
                Writer.WriteStartElement("Reference");
                Writer.WriteString(StrReference);
                Writer.WriteEndElement();

                Writer.WriteStartElement("JobID");
                Writer.WriteString(ucbJob.Text.ToString().Trim());
                Writer.WriteEndElement();

                //This is a Tag for Adjusment Date==============(3)
                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(GetDateTime(dtpInvAdjustment.Value));//Date format must be (MM/dd/yyyy)
                Writer.WriteEndElement();//Serial_Number

                if (dr["SerialNo"].ToString() != string.Empty)
                {
                    Writer.WriteStartElement("Serial_Number");
                    Writer.WriteString(dr["SerialNo"].ToString());
                    Writer.WriteEndElement();
                }
                //This is a Tag for numberof dsistribution=======(4)

                Writer.WriteStartElement("Number_of_Distributions ");
                Writer.WriteString("1");
                Writer.WriteEndElement();

                //Adjustmne Lines=================================(5)
                Writer.WriteStartElement("AdjustmentItems");
                //Adjustmne Lines=================================(6)
                Writer.WriteStartElement("AdjustmentItem");

                //Gl ASccount======================================(7)
                Writer.WriteStartElement("GLSourceAccount ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(StrGLAccount);
                Writer.WriteEndElement();

                string LastUnitCost = "0";
                //String S1 = "Select LastUnitCost from tblLastUnitCost where ItemID='" + dgvItemList[0, k].Value.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand("Select LastUnitCost from tblLastUnitCost where ItemID='" + StrItemID + "'", myConnection, myTrans);
                SqlDataAdapter da12 = new SqlDataAdapter(cmd1);
                DataSet dt12 = new DataSet();
                da12.Fill(dt12);
                LastUnitCost = dt12.Tables[0].Rows[0].ItemArray[0].ToString().Trim();

                double Totalcost = 0;
                if (Convert.ToDouble(LastUnitCost) == dblUnitCost || dblUnitCost == 0)
                {
                    Writer.WriteStartElement("UnitCost");
                    Writer.WriteString(LastUnitCost);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(dblQuantity.ToString());
                    // Writer.WriteString(Math.Abs(dblQuantity).ToString());
                    // Math.Abs(dblQuantity)

                    Writer.WriteEndElement();
                    Totalcost = Convert.ToDouble(LastUnitCost) * Math.Abs(dblQuantity);

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Totalcost.ToString());
                    Writer.WriteEndElement();

                }
                else
                {
                    Writer.WriteStartElement("UnitCost");
                    Writer.WriteString(dblUnitCost.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Quantity");
                    Writer.WriteString(dblQuantity.ToString());
                    // Writer.WriteString(Math.Abs(dblQuantity).ToString());
                    Writer.WriteEndElement();

                    Totalcost = dblUnitCost * Math.Abs(dblQuantity);

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + Totalcost.ToString());
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();//Adjustment Line
                Writer.WriteEndElement();//Adjustment lines


                Writer.WriteEndElement();//Item Closed Tag
            }
            Writer.Close();//finishing writing xml file
            Conn.IssueAdjustmentExport("Adjustment.xml");
        }

        //Conn.IssueAdjustmentExport("Adjustment.xml");

        //==============================================================================

        private void btnList_Click(object sender, EventArgs e)
        {
            if (frmMain.objfrmInventoryAdjustmentList == null || frmMain.objfrmInventoryAdjustmentList.IsDisposed)
            {
                frmMain.objfrmInventoryAdjustmentList = new frmInventoryAdjustmentList();
            }
            frmMain.objfrmInventotyAdjustment.TopMost = false;
            frmMain.objfrmInventoryAdjustmentList.ShowDialog();
            frmMain.objfrmInventoryAdjustmentList.TopMost = true; 


            string s = " Select * from tblInventoryAdjustment where AdjusmentId ='" + Search.AdjstID + "'";

            SqlCommand cmd1 = new SqlCommand(s);
            //cmd1.CommandType = CommandType.Text;
            SqlDataAdapter da1 = new SqlDataAdapter(s, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbARAccount.Value = dt.Rows[0]["GLAccount"].ToString();           
                dtpInvAdjustment.Value = DateTime.Parse(dt.Rows[0]["Date"].ToString());
                txtAdustQty.Text = dt.Rows[0]["AdjustQty"].ToString();
                txtDescription.Text = dt.Rows[0]["Itemdescription"].ToString();
                txtNewQty.Text = dt.Rows[0]["NewQty"].ToString();
                txtOnHandQty.Text = dt.Rows[0]["OnhandQty"].ToString();
                txtReasonToAdjust.Text = dt.Rows[0]["ReasonToAdjust"].ToString();
                txtReference.Text = dt.Rows[0]["AdjusmentId"].ToString();
                txtUnitCost.Text = double.Parse(dt.Rows[0]["UnitCost"].ToString()).ToString("0.00");
                txtWarehouseName.Text = dt.Rows[0]["WarehouseName"].ToString();
                cmbWarehouse.Text=dt.Rows[0]["WarehouseId"].ToString();
                ugCmbItem.Text=dt.Rows[0]["ItemID"].ToString();
                ucbJob.Value = dt.Rows[0]["JobNo"].ToString();
            }            
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(ugCmbItem, cmbWarehouse, e);
        }

        private void ugCmbItem_KeyDown(object sender, KeyEventArgs e)
        {
          //  objControlers.FocusControl(cmbARAccount, cmbWarehouse, e);
        }

        private void cmbARAccount_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtUnitCost, ugCmbItem, e);
        }

        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtAdustQty, cmbARAccount, e);
        }

        private void txtReasonToAdjust_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.PerformClick();
            }
        }

        private void btnSNO_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Warehouse First");
                    return;
                }

                if (Convert.ToDouble(txtAdustQty.Text.Trim()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" +
                        txtAdustQty.Text.Trim() + "'", "", 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (double.Parse(txtAdustQty.Text.Trim()) > 0)
                    {
                        frmSerialAddCommon ObjfrmSerialAddCommon = new frmSerialAddCommon("IN-InvAdjust",
                            cmbWarehouse.Text.ToString().Trim(),
                            ugCmbItem.Text.Trim().ToString(),
                            Convert.ToDouble(txtAdustQty.Text.Trim()), txtReference.Text.Trim(),
                            false, cmbWarehouse.Text.ToString().Trim(), clsSerializeItem.DtsSerialNoList);
                        ObjfrmSerialAddCommon.ShowDialog();
                    }
                    else
                    {
                        frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("OUT-InvAdjust",
                            cmbWarehouse.Text.ToString().Trim(),
                       ugCmbItem.Text.Trim().ToString(),
                       Convert.ToDouble(txtAdustQty.Text.Trim()),
                       txtReference.Text.Trim(), false, clsSerializeItem.DtsSerialNoList, null, false,true);
                        ObjfrmSerialSubCommon.ShowDialog();

                        //frmSerialAddCommon ObjfrmSerialAddCommon = new frmSerialAddCommon("OUT-InvAdjust",
                        //    cmbWarehouse.Text.ToString().Trim(),
                        //    ugCmbItem.Text.Trim().ToString(),
                        //    Convert.ToDouble(txtAdustQty.Text.Trim()), txtReference.Text.Trim(),
                        //    false, cmbWarehouse.Text.ToString().Trim(), clsSerializeItem.DtsSerialNoList);
                        //ObjfrmSerialAddCommon.ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
           

            dsObjAdjustments.Clear();

            String StrSql = "SELECT * FROM tblCompanyInformation";
            SqlDataAdapter da251 = new SqlDataAdapter(StrSql, ConnectionString);
            da251.Fill(dsObjAdjustments, "DtCompaniInfo");




            String S25;
           
            
                S25 = "Select * from tblInventoryAdjustment where   AdjusmentId ='" + txtReference.Text.ToString().Trim() + "'";//

            

            SqlDataAdapter da25 = new SqlDataAdapter(S25, ConnectionString);
            da25.Fill(dsObjAdjustments, "dt_Adjustments");

            if (dsObjAdjustments.dt_Adjustments.Rows.Count > 0)
            {
                frmViwerAdjustmetnsListPrint objAdjustmentList = new frmViwerAdjustmetnsListPrint(this);
                objAdjustmentList.Show();
            }
            else
            {
                MessageBox.Show("No Data Found");

            }

        }
    }
}