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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    
    public partial class frmWareHouseTrans : Form
    {

        public static DataTable _dataTable = new DataTable();
        bool _IsFind = false;
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon=new clsCommon ();
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();
        public string sMsg = "Multi Warehouse Module - Warehouse Transfer";
       // public static bool IsCostHide;

        public string StrSql;
        public double dblLineTot;
        public double dblSubTot;
        public double dblGrocessAmt;
        public double dblGrandTot;
        public double dblDiscAmt;
        public double dblDiscPer;
        public double dblVatAmount;
        public double dblNbtAmount;
        public double dblNetAmount;
        public double dblVat;
        public double dblNbt;
        public static string ConnectionString;
        int intEstomateProcode;
        public Boolean blnEdit;
        public int intProcess;
       
        public DataSet dsWarehouse;
        public DataSet dsToWarehouse;

        public DsItemWiseSales DsItemWise = new DsItemWiseSales();

        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmWareHouseTrans(int intNo)
        {
            InitializeComponent();
            setConnectionString();

            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }
        }

        private void setValue()
        {
            try
            {
                string strNo = (Search.searchIssueNoteNo);

                if (strNo == "")
                {
                    strNo = "";
                }
                else
                {
                    ClearHeader();
                    DeleteRows();
                    //jkl
                    EnableHeader(true);
                    EnableFoter(true);
                    ViewHeader(strNo);
                    ViewDetails(strNo);
                    EnableHeader(false);
                    EnableFoter(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }
        }

        public void UpdateInvoiceNo(Int32 intInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {
            SqlCommand command;
            Int32 intX;
            Int32 intZ;
            string StrInvNo;
            Int32 intP;
            Int32 intI;
            String StrInV;
            string StrUpdateInvNo;

            try
            {
                StrSql = "SELECT TrnPref, TrnPad, TrnNum FROM tblDefualtSetting";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = (int.Parse(dt.Rows[0].ItemArray[1].ToString().Trim()));
                    intZ = (int.Parse(dt.Rows[0].ItemArray[2].ToString().Trim()));

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();
                    StrUpdateInvNo = StrInvNo + StrInV.Substring(1, intX);              
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int WarehouseValidation(string WID)
        {
            try
            {
                StrSql = "SELECT WhseId FROM tblWhseMaster where WhseId='" +WID + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }
        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;
                
                StrSql = "SELECT  TOP 1(TrnNum) FROM tblDefualtSetting ORDER BY InvNum DESC";               

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
            
                StrSql = "UPDATE tblDefualtSetting SET TrnNum='" + intInvNo + "'";            

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void GetInvNo()
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;
             
                StrSql = "SELECT TrnPref, TrnPad, TrnNum FROM tblDefualtSetting";       


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

                    txtInvoiceNo.Text = StrInvNo + StrInV.Substring(1, intX);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                StrSql = "SELECT TrnPref, TrnPad, TrnNum FROM tblDefualtSetting";          

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
            }
        }

        public void GetFromWarehouse()
        {
            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 180;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 360;
                       
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void GetToWarehouseDataSet()
        {
            dsToWarehouse = new DataSet();
            try
            {
                dsToWarehouse.Clear();
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsToWarehouse, "DtWarehouse");
                cmbToWarehouse.DataSource = dsToWarehouse.Tables["DtWarehouse"];
                cmbToWarehouse.DisplayMember = "WhseId";
                cmbToWarehouse.ValueMember = "WhseId";
                cmbToWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 180;
                cmbToWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 360;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetItemDescriptionDataSet()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }

                StrSql = "SELECT  tblItemWhse.ItemId,tblItemWhse.ItemDis ,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5) order by tblItemWhse.ItemDis";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDis";
                    ultraCombo2.DisplayMember = "ItemDis";

                    ultraCombo2.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[1].Width = 300;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[8].Width = 100;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[13].Width = 100;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[3].Hidden = true;


                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    //ultraCombo2.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    //ultraCombo2.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                    // ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 200;
                }
                else
                {
                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemDis";
                    ultraCombo2.DisplayMember = "ItemDis";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetItemDataSet()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }

                StrSql = "SELECT  tblItemWhse.ItemId,tblItemWhse.ItemDis ,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                    ultraCombo1.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 300;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Width = 100;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 100;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Hidden = true;
               

                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    //ultraCombo1.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    //ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                   // ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 200;
                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteRows()
        {
            try
            {
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    ugR.Delete(false);
                }
                GrandTotal();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteEmpGrid()
        {
            ug.PerformAction(UltraGridAction.CommitRow);
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                if (ugR.Cells[1].Value.ToString().Trim().Length == 0)
                {
                    ugR.Delete(false);
                }
            }
        }

        private void GrandTotal()
        {
            try
            {
                dblGrandTot = 0;
                dblGrocessAmt = 0;
                dblSubTot = 0;
                dblVatAmount = 0;
                dblNbtAmount = 0;

                int intGridRow;

                for (intGridRow = 0; intGridRow < ug.Rows.Count; intGridRow++)
                {
                    dblSubTot += double.Parse(ug.Rows[intGridRow].Cells[5].Value.ToString());

                }
                dblDiscPer = double.Parse(txtDiscPer.Value.ToString());
                if (double.Parse(txtDiscPer.Value.ToString()) > 0)
                {
                    dblDiscAmt = (dblSubTot * dblDiscPer) / 100;
                }
                else
                {
                    dblDiscAmt = 0;
                }

                dblGrocessAmt = dblSubTot - dblDiscAmt;


                if (double.Parse(txtNBTPer.Value.ToString()) > 0)
                {
                    dblNbtAmount = ((dblGrocessAmt * double.Parse(txtNBTPer.Value.ToString())) / 100);
                }
                else
                {
                    dblNbtAmount = 0;
                }

                if (double.Parse(txtVatPer.Value.ToString()) > 0)
                {
                    dblVatAmount = (((dblGrocessAmt + dblNbtAmount) * double.Parse(txtVatPer.Value.ToString())) / 100);
                }
                else
                {
                    dblVatAmount = 0;
                }

                dblNetAmount = dblGrocessAmt + dblNbtAmount + dblVatAmount;

                txtSubValue.Value = dblSubTot;
                txtDiscAmount.Value = dblDiscAmt;
                txtGrossValue.Value = dblGrocessAmt;
                txtNBT.Value = dblNbtAmount;
                txtVat.Value = dblVatAmount;
                txtNetValue.Value = dblNetAmount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (ug.ActiveCell == null) return;
                if (ug.ActiveCell.Column == null) return;
                if (ug.ActiveCell.Column.Key == "Quantity")
                {
                    if (ug.ActiveRow.HasNextSibling() == false)
                    {
                        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                        {
                            if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                            {
                                UltraGridRow ugR;
                                ugR = ug.DisplayLayout.Bands[0].AddNew();
                                ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                ug.DisplayLayout.Rows[ugR.Index].Cells[0].Activate();
                            }

                            if (ug.ActiveCell.Row.Cells["Description"].Value.ToString().Trim() != string.Empty)
                            {
                                UltraGridRow ugR;
                                ugR = ug.DisplayLayout.Bands[0].AddNew();
                                ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                ug.DisplayLayout.Rows[ugR.Index].Cells[0].Activate();
                            }
                        }
                    }
                }
                else if (ug.ActiveCell.Column.Key == "TotalPrice")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        //ug.ActiveCell = ug.Rows[ug.Rows.Count].Cells[1];
                        ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown,false,true);
                    }
                }

                //switch (e.KeyValue)
                //{
                //    case 37:
                //        {
                //            ug.PerformAction(UltraGridAction.PrevCell);
                //            break;
                //        }
                //    case 38:
                //        {
                //            ug.PerformAction(UltraGridAction.AboveCell);
                //            break;
                //        }
                //    case 39:
                //        {
                //            ug.PerformAction(UltraGridAction.NextCell);
                //            break;
                //        }
                //    case 40:
                //        {
                //            ug.PerformAction(UltraGridAction.BelowCell);
                //            break;
                //        }
                //    case 9:
                //        {
                //            if (ug.ActiveCell.Column.Key == "Quantity")
                //            {
                //                if (ug.ActiveRow.HasNextSibling() == false)
                //                {
                //                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                //                    {
                //                        UltraGridRow ugR;
                //                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                //                        ugR.Cells["LineNo"].Value = ugR.Index + 1;                                    
                //                    }
                //                }
                //            }
                //            else if (ug.ActiveCell.Column.Key == "Line Total")
                //            {                          
                //                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                //                {
                //                    ug.ActiveCell = ug.Rows[ug.Rows.Count].Cells[1];
                //                    ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown, false, false);
                //                }                              
                //            }
                //            break;
                //        }
                //}               
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double LineCalculation(double UnitCost, double Quantity)
        {
            try
            {
                dblLineTot = 0;
                double lineTotal = 0;
                dblLineTot = UnitCost * Quantity;
                lineTotal = dblLineTot;
                return lineTotal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;

                if (HeaderValidation() == false)
                {
                    return;
                }

                if (ug.Rows.Count == 0)
                {
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                    ugR.Cells["LineNo"].Selected = true;
                    ugR.Cells["LineNo"].Activated = true;
                }

                if (ug.ActiveRow.Cells["ItemCode"].Value != null)
                {
                    if (IsThisItemSerial(ug.ActiveRow.Cells["ItemCode"].Value.ToString()))
                        btnSerialNo.Enabled = true;
                    else
                        btnSerialNo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public Boolean IsGridValidation()
        {
            try
            {
                if (ug.Rows.Count == 0)
                {
                    return false;
                }
                foreach (UltraGridRow ugR in ug.Rows)
                {
                    if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
                    {
                        MessageBox.Show("Invalid Item Code In Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (IsGridExitCodeDesc(ugR.Cells["Description"].Text) == false)
                    {
                        MessageBox.Show("Invalid Item Description In Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (int.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Transfer Quantity must be Greater than Zero", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsGridExitCodeDes(string text)
        {
            throw new NotImplementedException();
        }

        public Boolean HeaderValidation()
        {
            if (cmbWarehouse.Text.Trim() == "")
            {
                MessageBox.Show("Select warehouse.....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbToWarehouse.Text.Trim() == "")
            {
                MessageBox.Show("Select warehouse.....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbWarehouse.Text.Trim() == cmbToWarehouse.Text.Trim())
            {
                MessageBox.Show("You cant Transfer Items To the Same warehouse you have been selected", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        public void ViewDetails(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT ItemLine,ItemId,ItemDis,Qty,UnitCost,LineTotal,OnHandQty FROM tblWhseTransLine   WHERE WhseTransId='" + StrInvoiceNo + "' ORDER BY ItemLine";

            //StrSql= "SELECT tblWhseTransLine.ItemLine,tblWhseTransLine.ItemId,tblWhseTransLine.ItemDis,tblWhseTransLine.QTY," +
            //        "tblWhseTransLine.UnitCost,tblWhseTransLine.LineTotal,tblItemWhse.QTY AS OHQTY " +
            //        "FROM tblItemWhse RIGHT OUTER JOIN tblWhseTransLine ON tblItemWhse.ItemId = tblWhseTransLine.ItemId " +
            //        "WHERE(tblWhseTransLine.WhseTransId = '"+StrInvoiceNo+"')ORDER BY tblWhseTransLine.ItemLine";


                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = Dr["ItemLine"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemId"];
                        ugR.Cells["Description"].Value = Dr["ItemDis"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["Quantity"].Value = Dr["Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["LineTotal"];
                        ugR.Cells["OnHand"].Value = Dr["OnHandQty"];     
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ViewHeader(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT WhseTransId,FrmWhseId,ToWhseId,TDate,NetValue,Description FROM tblWhseTransfer WHERE WhseTransId='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtInvoiceNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    cmbToWarehouse.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0].ItemArray[3].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0].ItemArray[4].ToString().Trim());
                    txtDescription.Text = (dt.Rows[0].ItemArray[5].ToString().Trim());                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    return base.ProcessDialogKey(Keys.Tab);
            }
            return base.ProcessDialogKey(keyData);
        }
        private void frmWareHouseTrans_Load(object sender, EventArgs e)
        {
            try
            {
                //----------------user----------
                GridKeyActionMapping m = new GridKeyActionMapping(Keys.Enter, UltraGridAction.NextCellByTab, (UltraGridState)0, UltraGridState.Cell, SpecialKeys.All, (SpecialKeys)0);
                this.ug.KeyActionMappings.Add(m);

                intEstomateProcode = 0;

                if (intEstomateProcode == 0)
                {
                    run = false;
                    add = false;
                    edit = false;
                    delete = false;

                    dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmInvoices");
                    if (dtUser.Rows.Count > 0)
                    {
                        run = Convert.ToBoolean(dtUser.Rows[0].ItemArray[2].ToString());
                        add = Convert.ToBoolean(dtUser.Rows[0].ItemArray[3].ToString());
                        edit = Convert.ToBoolean(dtUser.Rows[0].ItemArray[4].ToString());
                        delete = Convert.ToBoolean(dtUser.Rows[0].ItemArray[5].ToString());
                    }

                    run = true;
                    add = true;
                    edit = true;
                    delete = true;

                    if (user.IsCostHide)
                    {
                        ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = true;
                        ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = true;
                    }
                    else
                    {
                        ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                        ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    }
                    //---------------------------------
                   // GetCurrentUserDate();
                    GetFromWarehouse();
                    GetToWarehouseDataSet();


                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;
                    btnSearch.Enabled = true;
                    btnReset.Enabled = true;
                    btnNew.Enabled = true;
                    btnEdit.Enabled = false;
                    dtpDate.Enabled = false;

                    //btnSave.Enabled = true;
                    //btnDelete.Enabled = false;
                    //btnFind.Enabled = true;
                    //btnClear.Enabled = true;

                    GetItemDataSet();
                    GetItemDescriptionDataSet();
                  //  GetWareHouseDataSet();
                    //GetToWarehouseDataSet();
                    //GetFromWarehouse();
                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);
                    GetInvNo();

                    _dataTable.Columns.Clear();
                    if (_dataTable.Columns.Count == 0)
                    {
                        _dataTable.Columns.Add("ItemCode");
                        _dataTable.Columns.Add("WHCode");
                        _dataTable.Columns.Add("SerialNo");
                        _dataTable.Columns.Add("Status");
                        _dataTable.Columns.Add("Selected");
                    }
                }
                //btnReset_Click(sender, e);
                btnNew_Click(sender, e);
                
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void GetCurrentUserDate()
        {
            try
            {
                String S = "Select CurrentDate from tblUserWiseDate where UserName='" + UserAutherization.user.userName.ToString() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    //ultraCalendarCombo1.Text=dt.Tables[0].Rows[i].ItemArray[0].ToString();
                    //UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    //dtpDate.Value = DateTime.Parse(dt.Tables[0].Rows[i].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void cmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String S2 = "select * from tblWhseMaster where WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    txtWarehouseName.Text = dt2.Rows[0].ItemArray[1].ToString();                 
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

            txtWarehouseName.Text = "";
            txtToWarehouseName.Text = "";
            if (add)
            {
                btnSave.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSerialNo.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);
                ug.Enabled = true;
                ClearHeader();
                DeleteRows();
                GetInvNo();
                cmbWarehouse.Focus();

                _dataTable.Rows.Clear();

                blnEdit = false;
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetEstimateCode(string strJobID, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT AutoIndex FROM tblJobMaster WHERE JobID='" + strJobID + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetFilledRows()
        {
            try
            {
                int RowCount = 0;

                for (int i = 0; i < ug.Rows.Count; i++)
                {
                    if (ug.Rows[i].Cells[1].Value != null) //change cell value by 1                   
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
                foreach (UltraGridRow dgvr in ug.Rows)
                {
                    if (dgvr.Cells["ItemCode"].Value != null)
                    {
                        if (IsThisItemSerial(dgvr.Cells["ItemCode"].Value.ToString().Trim()) && double.Parse(dgvr.Cells["Quantity"].Value.ToString()) > 0)
                        {
                            if (clsSerializeItem.DtsSerialNoList.Rows.Count == 0)
                            {
                                if (double.Parse(dgvr.Cells["Quantity"].Value.ToString()) > 0)
                                {
                                    MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }

                            _Count = 0;
                            expression = "ItemCode = '" + dgvr.Cells["ItemCode"].Value.ToString().Trim() + "'";
                            DataRow[] foundRows;

                            // Use the Select method to find all rows matching the filter.
                            foundRows = clsSerializeItem.DtsSerialNoList.Select(expression);

                            // Print column 0 of each returned row. 
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                _Count = i + 1;
                            }

                            if (_Count > 0 && double.Parse(dgvr.Cells["Quantity"].Value.ToString()) == 0)
                            {
                                for (int i = 0; i < foundRows.Length; i++)
                                {
                                    clsSerializeItem.DtsSerialNoList.Rows.Remove(foundRows[i]);
                                }
                            }

                            if (_Count != double.Parse(dgvr.Cells["Quantity"].Value.ToString()))
                            {
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void SaveEvent()
        {
            int rowCount = GetFilledRows();
            if (dtpDate.Value < user.Period_begin_Date)
            {
                MessageBox.Show("Transaction Date is Prior to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dtpDate.Value > user.Period_End_Date)
            {
                MessageBox.Show("Transaction Date is Exceed to Financial Period", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbWarehouse.Value != null)
            {
                if (WarehouseValidation(cmbWarehouse.Value.ToString().Trim()) == 0)
                {
                    MessageBox.Show("Incorrect From Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Incorrect From  Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbToWarehouse.Value != null)
            {
                if (WarehouseValidation(cmbToWarehouse.Value.ToString().Trim()) == 0)
                {
                    MessageBox.Show("Incorrect To Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Incorrect To  Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            bool IsItemSerial = objControlers.CheckSerialNo(rowCount, ug, "ItemCode", "Quantity", _dataTable);           

            if (HeaderValidation() == false)
            {
                return;
            }

            if (!IsItemSerial)
            {
                MessageBox.Show("Enter Serial Numbers...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsSerialNoCorrect())
                return;

            int intGrid;
            double dblAvailableQty;
            string StrReference = null;
            int intItemClass;

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
                DeleteEmpGrid();

                if (IsGridValidation() == false)
                {
                    return;
                }
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                StrReference = GetInvNoField(myConnection, myTrans);
                UpdatePrefixNo(myConnection, myTrans);
                SaveHeader(StrReference, myConnection, myTrans);

                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(StrReference, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), myConnection, myTrans);

                    //--------Check Stock Item-------------

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), cmbWarehouse.Text.Trim(), cmbToWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), cmbToWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                    }
                }

                if (IsItemSerial)
                {
                    foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                    {
                        SqlCommand myCommandSe1 = new SqlCommand("delete  tblSerialItemTransaction where ItemID='" + 
                            dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Value.ToString().Trim() + "' and SerialNo='" + 
                            dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                        myCommandSe1.ExecuteNonQuery();                       

                        //for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                        {                            
                            SqlCommand myCommandSe12 = new SqlCommand("insert into tblSerialItemTransaction(WareHouseID,ItemID,SerialNO,TranType,Status)values ('" +
                                cmbToWarehouse.Value.ToString().Trim() + "','" + dr["ItemCode"].ToString() + "','" + dr["SerialNo"].ToString() + "','Trans','" + dr["Status"].ToString() + "')", myConnection, myTrans);
                            myCommandSe12.ExecuteNonQuery();
                        }
                    }
                    frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Tran", cmbWarehouse.Value.ToString(), StrReference, dtpDate.Value, true,"");
                    objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Tran", cmbToWarehouse.Value.ToString(), StrReference, dtpDate.Value, false,"");
                   
                }

                myTrans.Commit();
                MessageBox.Show("Tranfer Note Successfuly Saved.", "Information", MessageBoxButtons.OK);
                if (cmbToWarehouse.Text == "K2" || cmbToWarehouse.Text == "MT" || cmbToWarehouse.Text == "K1")
                {
                    DialogResult reply1 = MessageBox.Show("Are you sure, you want to Create a Transfer File Now ? ", "Information", MessageBoxButtons.OKCancel);

                    if (reply1 == DialogResult.Cancel)
                    {
                        //return;
                    }
                    else if (reply == DialogResult.OK)
                    {
                        ClassDriiDown.StrTrnasferNoteNO = StrReference;
                        frmImportHeadToBranch ObjIMEXP = new frmImportHeadToBranch();
                        ObjIMEXP.ShowDialog();
                    }
                }
                Print(StrReference);
                //frmViewerTransferNotePrint ObjTransPrint = new frmViewerTransferNotePrint(this);
                //ObjTransPrint.Show();
                ButtonClear();

            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                //btnSave.Focus();


            }
        }      
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveEvent();
                _dataTable.Rows.Clear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }             
        }

        private double CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT QTY FROM tblItemWhse WHERE ItemId='" + StrItemCode + "' AND WhseId='" + StrWarehouseCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return double.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }

        public static string GetDateTime(DateTime DtGetDate)
        {
            DateTime DTP = Convert.ToDateTime(DtGetDate);
            string Dformat = "MM/dd/yyyy";
            return DTP.ToString(Dformat);

        }

        private void DeleteDetails(int intEstomateProcode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "DELETE FROM [EST_DETAILS] WHERE AutoIndex=" + intEstomateProcode + "";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateItemWarehouse(string StrItemCode,string StrItemDesc, string StrWarehouse,string StrWarehouseTo ,double dblQty,string StrUOM,double DblUnitCost,double DblTotalCost, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                StrSql = "SELECT ItemId from tblItemWhse  WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "' ";
                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                if (dt.Rows.Count > 0)
                {               
                    StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "'";
                }
                else 
                {
                    StrSql = "INSERT INTO [tblItemWhse] ([WhseId],[ItemId],[ItemDis],[QTY],[UOM],[TraDate],[UnitCost],[TranType],[TotalCost],[OPBQtry]) VALUES('" + cmbToWarehouse.Value + "','" + StrItemCode + "','" + StrItemDesc + "'," + dblQty + ",'" + StrUOM + "','" + GetDateTime(DateTime.Now) + "'," + DblUnitCost + ",'Trans'," + DblTotalCost + ",0)";
                }               

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {               
                throw ex;
            }
        }

        private int GetLastTransactionNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT TOP 1 [AutoIndex] FROM EST_HEADER ORDER BY [AutoIndex] DESC";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InvTransaction(string strInvoiceNo, DateTime dtDate, String StrItemCode, string StrLocCode, string StrLocCodeTo, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            double OnHandQty = 0;
            try
            {
                StrSql = "select isnull(Qty,0) from tblItemWhse where WhseId='" + StrLocCode + "' and ItemId='" + StrItemCode + "'";//INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(3,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Tran','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0) OnHandQty = double.Parse(dt.Rows[0][0].ToString());

                StrSql = "INSERT INTO [tbItemlActivity](OHQTY, [DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES("+OnHandQty+", 3,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Tran','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                StrSql = "select isnull(Qty,0) from tblItemWhse where WhseId='" + StrLocCodeTo + "' and ItemId='" + StrItemCode + "'";//INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(3,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Tran','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                command = new SqlCommand(StrSql, con, Trans);
                da = new SqlDataAdapter(command);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0) OnHandQty = double.Parse(dt.Rows[0][0].ToString());

                StrSql = "INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(" + OnHandQty + ", 3,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Tran','true','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCodeTo + "',0)";
                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveHeader(string StrInvoiceNo, SqlConnection con, SqlTransaction Trans)
        {            
            try
            {
                StrSql = "INSERT INTO tblWhseTransfer (WhseTransId,FrmWhseId,ToWhseId,TDate,NetValue,Description) VALUES('" + StrInvoiceNo + "','" + cmbWarehouse.Value.ToString() + "','" + cmbToWarehouse.Value + "','" + GetDateTime(dtpDate.Value) + "'," + txtNetValue.Value + ",'"+ txtDescription.Text +"')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        private void SaveDetails(string StrInvoiceNo, int intLineNo, String StrItemCode, string StrItemDescription, double dblQuantity, string StrUOM, double dblCostPrice, double dblLineTotal,double Qty_OnHand, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblWhseTransLine (WhseTransId,ItemLine,ItemId,ItemDis,QTY,UOM,FrmWhseQTY,ToWhseQTY,UnitCost,LineTotal,OnHandQty) VALUES('" + StrInvoiceNo + "'," + intLineNo + ",'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + ",'" + StrUOM + "'," + dblQuantity + "," + dblQuantity + "," + dblCostPrice + "," + dblLineTotal + ",'" + Qty_OnHand + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                blnEdit = true;
                btnEdit.Enabled = true;
                btnPrint.Enabled = true;
                btnSave.Enabled = false;
                frmSeachTrans issueSearch = new frmSeachTrans();
                issueSearch.ShowDialog();
                setValue();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void DataSetHeader(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtEstimateHeader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DataSetDetails(int intEstimateNo)
        {
            try
            {
                StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtEstimateDETAILS);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\rptTransferNote.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("rptTransferNote.rpt");
                }
                else
                {
                    MessageBox.Show("rptTransferNote.rpt not Exists.");
                    this.Close();
                    return;
                }

                crp.Load(Myfullpath);
                crp.SetDataSource(DsItemWise);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Print(string StrTransNo)
        {
            try
            {
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);
                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                if (StrTransNo != "")
                {
                    DsItemWise.Clear();

                    StrSql = "SELECT * FROM tblWhseTransfer WHERE WhseTransId='" + StrTransNo + "'";
                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(DsItemWise.tblWhseTransfer);

                    StrSql = "SELECT * FROM tblWhseTransLine WHERE WhseTransId='" + StrTransNo + "'";
                    cmd = new SqlCommand(StrSql);
                    da = new SqlDataAdapter(StrSql, ConnectionString);
                    dt = new DataTable();
                    da.Fill(DsItemWise.tblWhseTransLine);

                    StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                    cmd = new SqlCommand(StrSql);
                    da = new SqlDataAdapter(StrSql, ConnectionString);
                    dt = new DataTable();
                    da.Fill(DsItemWise.DsItem);

                    StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                    cmd = new SqlCommand(StrSql);
                    da = new SqlDataAdapter(StrSql, ConnectionString);
                    dt = new DataTable();
                    da.Fill(DsItemWise.DsWarehouse);

                    StrSql = "SELECT * FROM tblCompanyInformation";
                    cmd = new SqlCommand(StrSql);
                    da = new SqlDataAdapter(StrSql, ConnectionString);
                    dt = new DataTable();
                    da.Fill(DsItemWise.DtCompaniInfo);

                    frmViewerTransferNotePrint ObjTransPrint = new frmViewerTransferNotePrint(this);
                    ObjTransPrint.Show();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(txtInvoiceNo.Text);
           // DirectPrint();
        }

        private void ClearFooter()
        {
            txtNBTPer.Value = 0;
            txtVatPer.Value = 0;
            txtDiscPer.Value = 0;
            txtSubValue.Value = 0;
            txtDiscAmount.Value = 0;
            txtGrossValue.Value = 0;
            txtNBT.Value = 0;
            txtVat.Value = 0;
            txtNetValue.Value = 0;
        }

        private void ClearHeader()
        {
            try
            {
                cmbWarehouse.Value = user.StrDefaultWH;
                cmbToWarehouse.Value = user.StrDefaultWH;

                txtDescription.Text = "";
                dtpDate.Value = user.LoginDate;

                txtNBTPer.Value = 0;
                txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
                clsSerializeItem.DtsSerialNoList.Rows.Clear();

                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableFoter(Boolean blnEnable)
        {
            txtVatPer.Enabled = blnEnable;
            txtNBTPer.Enabled = blnEnable;
            txtDescription.Enabled = blnEnable;
            txtDiscPer.Enabled = blnEnable;
           // ug.Enabled = blnEnable;
        }

        private void EnableHeader(Boolean blnEnable)
        {
            cmbWarehouse.Enabled = blnEnable;
            cmbToWarehouse.Enabled = blnEnable;
            dtpDate.Enabled = blnEnable;
            txtWarehouseName.Enabled = false;
            txtToWarehouseName.Enabled = false;
        }

        private void ButtonClear()
        {
            btnSave.Enabled = true;
            btnNew.Enabled = true;
            btnPrint.Enabled = false;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            btnEdit.Enabled = false;
            //GetItemDataSet();
            //GetItemDescriptionDataSet();

            //btnSave.Enabled = true;
            //btnDelete.Enabled = false;
            //btnFind.Enabled = true;
            //btnClear.Enabled = true;
          
            ClearHeader();
            EnableHeader(false);
            EnableFoter(false);
            DeleteRows();
            GetInvNo();
            ug.Enabled = true;
            intEstomateProcode = 0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ButtonClear();
        }

        private void DatasetWhse()
        {
            try
            {
                StrSql = "SELECT * FROM tblWhseMaster";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(DsEst.DtWhseMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ug_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
        }       

        private double ChangePrice2(double dblPriceList2)
        {
            return dblPriceList2;
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity")
                {
                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    GrandTotal();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (double.Parse(txtDiscPer.Value.ToString()) > 100)
                {
                    MessageBox.Show("Invalid Discount Percentage", "Information", MessageBoxButtons.OK);
                    txtDiscPer.Focus();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Edit()
        {
            EnableHeader(false);
            EnableFoter(false);
            dtpDate.Enabled = true;
            txtDescription.Enabled = true;
            txtDiscPer.Enabled = true;
            txtNBTPer.Enabled = true;
            txtVatPer.Enabled = true;
            btnReset.Enabled = true;
            btnSave.Enabled = true;
            ug.Enabled = true;
        }

        public Boolean IsGridExitCode(String StrCode)
        {
            foreach (UltraGridRow ugR in ultraCombo1.Rows)
            {
                if (ugR.Cells["ItemID"].Text == StrCode)
                {
                    return true;
                }
            }
            return false;
        }
            public Boolean IsGridExitCodeDesc(String StrDescription)
        {
            try
                        {
                foreach (UltraGridRow ugR in ultraCombo2.Rows)
                {
                    if (ugR.Cells["ItemDis"].Text == StrDescription)
                    {
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

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {
            try
            {
                //if (ug.Rows.Count > 0)
                //{
                //    DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OK);

                //    if (reply == DialogResult.OK)
                //    {
                //        DeleteRows();
                //        ClearFooter();
                //    }
                //}
                //GetItemDataSet();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                if (ug.ActiveCell.Column.Key == "ItemCode")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (IsGridExitCode(ug.ActiveCell.Row.Cells[1].Text) == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo1.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 0;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitCost"].Value;
                            }
                        }
                    }
                    HideSelectedRow();
                }

                if (ug.ActiveCell.Column.Key == "Description")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (IsGridExitCodeDesc(ug.ActiveCell.Row.Cells[2].Text) == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo2.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemDis"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemID"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 0;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitCost"].Value;
                            }
                        }
                    }

                    HideSelectedRow();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void HideSelectedRow()
        {
            for (int x = 0; x < ultraCombo1.Rows.Count; x++)
            {
                ultraCombo1.Rows[x].Hidden = false;
                ultraCombo2.Rows[x].Hidden = false;
            }
            for (int i = 0; i < ultraCombo1.Rows.Count; i++)
            {
                for (int v = 0; v < ug.Rows.Count; v++)
                {
                    if (ultraCombo1.Rows[i].Cells[0].Value.ToString() == ug.Rows[v].Cells["ItemCode"].Value.ToString())
                    {
                        ultraCombo1.Rows[i].Hidden = true;
                        ultraCombo2.Rows[i].Hidden = true;
                    }

                }
            }
        }

        private void optSerialTwo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetInvNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void optSerialOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetInvNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
                    if (ug.Rows.Count > 0)
                    {
                        //DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OK);

                        //if (reply == DialogResult.OK)
                        //{
                            DeleteRows();
                            ClearFooter();
                        //}
                    }
                    GetItemDataSet();
                    GetItemDescriptionDataSet();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }      

       

        private void cmbToWarehouse_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtToWarehouseName.Text = cmbToWarehouse.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {
            HideSelectedRow();
            try
            {
                GrandTotal();
                SetRowLineNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnIndirectPrint_Click(object sender, EventArgs e)
        {
            //Print(txtInvoiceNo.Text);
            //frmViewerTransferNotePrint ObjTransPrint = new frmViewerTransferNotePrint(this);
            //ObjTransPrint.Show();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                UltraGridRow ugR;

                if (ug.Rows.Count == 0)
                {
                    ugR = ug.DisplayLayout.Bands[0].AddNew();
                    ugR.Cells["LineNo"].Value = ugR.Index + 1;
                }

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    ug.ActiveCell = ug.Rows[0].Cells[1];
                    ug.PerformAction(UltraGridAction.EnterEditModeAndDropdown, false, false);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(cmbToWarehouse, cmbWarehouse, e);
        }

        private void cmbToWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            objControlers.FocusControl(txtDescription, cmbWarehouse, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmWareHouseTrans_Load(sender, e);
        }

        private void SetRowLineNo()
        {
            try
            {
                UltraGridRow ugR;
                int _Index=1;
                foreach (UltraGridRow dr in ug.Rows )
                {
                    //ugR = ug.DisplayLayout.Bands[0].AddNew();
                    dr.Cells["LineNo"].Value = _Index;
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;                                                                                       
            }
        }        

        private void btnCreateFile_Click(object sender, EventArgs e)
        {
            //if (cmbToWarehouse.Text == "K2" || cmbToWarehouse.Text == "MT")
            //{
            DialogResult reply1 = MessageBox.Show("Are you sure, you want to Create a Transfer File Now ? ", "Information", MessageBoxButtons.OKCancel);

            if (reply1 == DialogResult.Cancel)
            {
                return;
            }
            else if (reply1 == DialogResult.OK)
            {
                ClassDriiDown.StrTrnasferNoteNO = txtInvoiceNo.Text.ToString().Trim();
                frmImportHeadToBranch ObjIMEXP = new frmImportHeadToBranch();
                ObjIMEXP.ShowDialog();
            }
            //}
            Print(txtInvoiceNo.Text.ToString().Trim());
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
                            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ug.PerformAction(UltraGridAction.CommitRow);
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select  From Warehouse First");
                    return;
                }

                if (cmbToWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select To Warehouse First");
                    return;
                }

                if (Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + ug.ActiveRow.Cells["Quantity"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            ug.ActiveRow.Cells["Quantity"].Selected = true;
                        }
                    }
                }
                else
                {
                    //clsSerializeItem.TranScreen = "WarehouseTransfer";
                    //clsSerializeItem.StrSerialItemID = ug.ActiveRow.Cells["ItemCode"].Value.ToString();
                    //clsSerializeItem.StrDescription = ug.ActiveRow.Cells["Description"].Value.ToString();
                    //clsSerializeItem.IntEnterdQty = Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString());
                    //clsSerializeItem.StrWarehouseID = cmbWarehouse.Text.ToString().Trim();
                    //clsSerializeItem.StrWarehouseName = txtWarehouseName.Text.ToString().Trim();

                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Tran", cmbWarehouse.Text.ToString().Trim(),
                        ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                        Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                        txtInvoiceNo.Text.Trim(), blnEdit, clsSerializeItem.DtsSerialNoList, null, false,true);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Begining Balance", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnSNO_Click(object sender, EventArgs e)
        {
                    }

  

   




   

     

        

        
   

       

       
        





    }
}