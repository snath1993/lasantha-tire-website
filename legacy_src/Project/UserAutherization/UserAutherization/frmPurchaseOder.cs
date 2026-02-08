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
using System.Collections;
using System.Threading;


namespace UserAutherization
{
    public partial class frmPurchaseOder : Form
    {
        clsCommon objclsCommon = new clsCommon();
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();
        public DataSet dsVendor;
        public DataSet dsAR;


        public bool IsUpdate;

        public string sMsg = "Direct Supllier Return";
        public double TaxRate = 0;
        public double TaxRate1 = 0;
        public string StrSql;
        public double dblLineTot = 0;
        public double dblUnitCost = 0;
        public double dblSubTot;
        public double dblGrocessAmt = 0;
        public double dblGrandTot = 0;
        public double dblDiscAmt = 0;
        public double dblDiscPer = 0;
        public double dblVatAmount = 0;
        public double dblNbtAmount = 0;
        public double dblNetAmount = 0;
        public double dblVat = 0;
        public double dblNbt = 0;
        public static string ConnectionString;
        int intEstomateProcode;
        public Boolean blnEdit;
        public int intProcess;
        public string StrPaymmetM;
        public string StrARAccount;
        public string StrAPAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;

        public string Tax1ID = "";
        public string Tax1Name = "";
        public double Tax1Amount = 0.0;
        public string Tax1GLAccount = "";
        public string Tax2ID = "";
        public string Tax2Name = "";
        public double Tax2Amount = 0.0;
        public string Tax2GLAccount = "";
        public string Tax3ID = "";
        public string Tax3Name = "";
        public double Tax3Amount = 0.0;
        public string Tax3GLAccount = "";

        public DataSet dsWarehouse;
        public DataSet dsReturn;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public DSCustomerReturn DsCustomerReturn = new DSCustomerReturn();
        public DsPurchaseOder DSPurchaseOder = new DsPurchaseOder();
        public static DateTime UserWiseDate = System.DateTime.Now;
        DataSet dsCustomer;
        DataSet dsSalesRep;
        public DataSet ds;

        public Boolean IsActive = true;

        public frmPurchaseOder()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmPurchaseOder(int intNo)
        {
            try
            {
                InitializeComponent();
                setConnectionString();

                if (intNo == 0)
                {
                    intEstomateProcode = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void setValue()
        {
            try
            {
                string strNo = (Search.searchPONo);

                if (strNo == "")
                {
                    strNo = "";
                }
                else
                {
                    btnProcess.Enabled = false;
                    specialedit = false;
                    ClearHeader();
                    DeleteRows();

                    EnableHeader(true);
                    EnableFoter(true);
                    txtCreditNo.Text = strNo;
                    SetSerchValues();
                    Search.searchPONo = "";
                    //ViewHeader(strNo);
                    //ViewDetails(strNo);
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

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intCRNNo;
                SqlCommand command;

                StrSql = "SELECT  TOP 1(PoNo) FROM tblDefualtSetting ORDER BY PoNo DESC";

                command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    intCRNNo = (int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim())) + 1;
                }
                else
                {
                    intCRNNo = 1;
                }
                StrSql = "UPDATE tblDefualtSetting SET PoNo='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Int32 GetPOLink(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int IntPOLInk = 0;

                StrSql = "SELECT COUNT  (DISTINCT [PONumber]) FROM[dbo].[tblPurchaseOrder]";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    IntPOLInk = int.Parse(dt.Rows[0].ItemArray[0].ToString().Trim());
                    return IntPOLInk + 1;
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

                StrSql = "SELECT PoNoPref, PoNoPad, PoNo FROM tblDefualtSetting";

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

        public void GetCRNNo()
        {

            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT PoNoPref, PoNoPad, PoNo FROM tblDefualtSetting";

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

                    txtCreditNo.Text = StrInvNo + StrInV.Substring(1, intX);
                }
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
                StrSql = " SELECT WhseId, WhseName,APAccount,InventoryAccount,SalesGLAccount FROM tblWhseMaster order by IsDefault";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 200;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["APAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["InventoryAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;
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
                //StrSql = "SELECT tblItemWhse.ItemId,tblItemWhse.ItemDis, tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY as QOH,tblItemMaster.ItemClass,tblItemMaster.InventoryAcc as SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                //StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5) order by tblItemWhse.ItemDis";

                StrSql = "SELECT [MasterID] as ItemId,[MasterDesc] as ItemDis,[UnitPrice],[PriceLevel1],[PriceLevel2],[PriceLevel3],[PriceLevel4],[PriceLevel5]" +
                         " ,isnull([QTY],0) as QOH,[ItemClass],[InventoryAcc] as SalesGLAccount,[UOM],[Categoty],[UnitCost],[AVGQty],isnull([LastPurDate], '" + System.DateTime.Now.ToShortDateString() + "') as LastPurDate FROM [dbo].[View_loadToItemMaster] where WhseId ='"+cmbWarehouse.Text.ToString()+"' order by MasterDesc";
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

                    ultraCombo2.DisplayLayout.Bands[0].Columns[8].CellAppearance.TextHAlign = HAlign.Right;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[13].CellAppearance.TextHAlign = HAlign.Right;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[14].CellAppearance.TextHAlign = HAlign.Right;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[3].Hidden = true;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    //ultraCombo1.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[12].Hidden = true;
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

                //StrSql = "SELECT tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY as QOH,tblItemMaster.ItemClass,tblItemMaster.InventoryAcc as SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                //StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster order by tblItemWhse.ItemDis";


                StrSql = "SELECT [MasterID] as ItemId,[MasterDesc] as ItemDis,[UnitPrice],[PriceLevel1],[PriceLevel2],[PriceLevel3],[PriceLevel4],[PriceLevel5]" +
                          " ,isnull([QTY],0) as QOH,[ItemClass],[InventoryAcc] as SalesGLAccount,[UOM],[Categoty],[UnitCost],[AVGQty],isnull([LastPurDate],'" + System.DateTime.Now.ToShortDateString() + "') as LastPurDate FROM [dbo].[View_loadToItemMaster] where WhseId ='" + cmbWarehouse.Text.ToString() + "' order by MasterDesc";

                //StrSql = "SELECT tblItemWhse.ItemId,tblItemWhse.ItemDis, tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY as QOH,tblItemMaster.ItemClass,tblItemMaster.InventoryAcc as SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                //StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5) order by tblItemWhse.ItemDis";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemId";
                    ultraCombo1.DisplayMember = "ItemId";

                    ultraCombo1.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 300;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Width = 100;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 100;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].CellAppearance.TextHAlign = HAlign.Right;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].CellAppearance.TextHAlign = HAlign.Right;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[14].CellAppearance.TextHAlign = HAlign.Right;

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


                    //Alignment

                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemId";
                    ultraCombo1.DisplayMember = "ItemId";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void GetCustomer()
        //{
        //    dsCustomer = new DataSet();
        //    try
        //    {
        //        dsCustomer.Clear();
        //        StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster order by CutomerID";

        //        SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
        //        dAdapt.Fill(dsCustomer, "DtClient");

        //        cmbVendorSelect.DataSource = dsCustomer.Tables["DtClient"];
        //        cmbVendorSelect.DisplayMember = "CutomerID";
        //        cmbVendorSelect.ValueMember = "CutomerID";

        //        cmbVendorSelect.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
        //        cmbVendorSelect.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
        //        cmbVendorSelect.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
        //        cmbVendorSelect.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
            catch
            {
               
            }
        }

        public void DeleteEmpGrid()
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
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


            if (e.KeyCode == Keys.Alt)
            {
                cmbVendorSelect.Focus();
            }

            try
            {
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.PrevCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 38:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.AboveCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 39:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.NextCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 40:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ug.PerformAction(UltraGridAction.BelowCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }

                    case 9:
                        {
                            if (ug.ActiveCell.Column.Key == "LastPODate")
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                    if (ug.ActiveCell.Row.Cells["Description"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                }
                            }
                            break;
                        }


                    case 13:
                        {
                            if (ug.ActiveCell.Column.Key == "LastPODate")
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                    if (ug.ActiveCell.Row.Cells["Description"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                                        ugR.Cells["LineNo"].Selected = true;
                                        ugR.Cells["LineNo"].Activated = true;
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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

        private double LineCalculationUnitPrice(double TotalPrice, double Quantity)
        {
            try
            {
                dblUnitCost = 0;
                double unitcost = 0;
                dblUnitCost = TotalPrice / Quantity;
                unitcost = dblUnitCost;
                return unitcost;
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
            }
            catch (Exception ex)
            {
                throw ex;
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
                    
                        string b = ugR.Cells["ItemCode"].Text;
                        if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
                        {
                            MessageBox.Show("Invalid Item Code In Line No :- "+ ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                    if (IsGridExitCodeDes(ugR.Cells["Description"].Text) == false)
                    {
                        MessageBox.Show("Invalid Item Description In Line No :- " + ugR.Cells["LineNo"].Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                        {
                            MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (double.Parse(ugR.Cells["UnitPrice"].Value.ToString()) <= 0)
                        {
                            MessageBox.Show("Unit Price Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool IsGridExitCodeDes(string StrCode)
        {
            try
            {
                GetItemDescriptionDataSet();
                int c = ultraCombo1.Rows.Count;
                foreach (UltraGridRow ugR in ultraCombo2.Rows)
                {
                    if (ugR.Cells["ItemDis"].Text == StrCode)
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

        public Boolean HeaderValidation()
        {
            try
            {
                if (cmbWarehouse.Text.Trim() == "")
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ViewDetails(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT ItemLine,ItemId,ItemDis,Qty,UnitCost,LineTotal FROM tblWhseTransLine   WHERE WhseTransId='" + StrInvoiceNo + "' ORDER BY ItemLine";

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
                    txtCreditNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
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


        public void GetVendorDataset()
        {
            dsVendor = new DataSet();
            try
            {
                dsVendor.Clear();
                StrSql = " SELECT VendorID, VendorName,VContact,VAddress1,VAddress2 FROM tblVendorMaster order by VendorID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsVendor, "DtVendor");

                cmbVendorSelect.DataSource = dsVendor.Tables["DtVendor"];
                cmbVendorSelect.DisplayMember = "VendorID";
                cmbVendorSelect.ValueMember = "VendorID";
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VendorID"].Width = 75;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VendorName"].Width = 200;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VContact"].Hidden = true;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VAddress1"].Hidden = true;
                cmbVendorSelect.DisplayLayout.Bands["DtVendor"].Columns["VAddress2"].Hidden = true;

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

            GridKeyActionMapping m = new GridKeyActionMapping(Keys.Enter, UltraGridAction.NextCellByTab, (UltraGridState)0, UltraGridState.Cell, SpecialKeys.All, (SpecialKeys)0);
            this.ug.KeyActionMappings.Add(m);

            try
            {
                //----------------user----------
                intEstomateProcode = 0;
                cmbVendorSelect.Focus();
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
                    //---------------------------------
                    if (user.IsCostHide)
                    {
                        ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = true;
                        ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = true;
                        txtNetValue.Visible = false;
                        label5.Visible = false;
                    }
                    else
                    {
                        ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                        ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                        txtNetValue.Visible = true; ;
                        label5.Visible = true; ;
                    }

                    GetCurrentUserDate();

                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                    btnSearch.Enabled = true;
                    btnReset.Enabled = true;
                    btnNew.Enabled = true;
                    btnEdit.Enabled = false;
                    dtpDate.Enabled = false;

                    GetItemDataSet();
                    GetItemDescriptionDataSet();
                    GetWareHouseDataSet();
                    GetVendorDataset();
                    //abcd
                    // GetCustomer();
                    // GetSalesRep();
                    loadChartofAcount();
                    LoadDefualtAccount();

                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);
                    GetCRNNo();
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void GetCurrentUserDate()
        {
            try
            {
                dtpDate.Value = user.LoginDate;
                //String S = "Select CurrentDate from tblUserWiseDate where UserName='" + UserAutherization.user.userName.ToString() + "'";
                //SqlCommand cmd = new SqlCommand(S);
                //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                //DataSet dt = new DataSet();
                //da.Fill(dt);

                //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                //{
                //    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                //    dtpDate.Value = UserWiseDate;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    StrARAccount = dt2.Rows[0].ItemArray[3].ToString();  //Account payable Account
                    StrCashAccount = dt2.Rows[0].ItemArray[4].ToString();
                    StrSalesGLAccount = dt2.Rows[0].ItemArray[5].ToString();
                    StrAPAccount = dt2.Rows[0].ItemArray[7].ToString();  //Account payable Account
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {


            cmbVendorSelect.Focus();
            IsUpdate = false;
            btnEditer.Enabled = false;
            btnProcess.Enabled = false;
            btnPrint.Enabled = false;
            specialedit = false;
            txtWarehouseName.Text = "";
            processFinished = false;
            DeleteRows();
            try
            {
                if (add)
                {
                    Search.searchPONo = "";
                    btnSave.Enabled = true;
                    btnNew.Enabled = true;
                    btnPrint.Enabled = false;
                    btnSNO.Enabled = false;
                    btnSearch.Enabled = true;
                    btnReset.Enabled = true;
                    btnEdit.Enabled = false;
                    EnableHeader(true);
                    EnableFoter(true);

                    ClearHeader();
                 
                    GetCRNNo();
                    // GetCustomer();
                    // GetSalesRep();
                    LoadtaxDetails();
                    loadChartofAcount();
                    LoadDefualtAccount();
                    cmbVendorSelect.Focus();

                    if (user.IsCRTNNoAutoGen) txtCreditNo.ReadOnly = true;
                    else txtCreditNo.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                                MessageBox.Show("Enter Serial Numbers for ItemCode=" + dgvr.Cells["ItemCode"].Value.ToString().Trim(), "Supplier Return", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
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

            string TranType = "Purchase-Order";
            int DocType = 7;
            bool QtyIN = false;

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Please Select Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Please Select vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

          
            int intGrid;
            int intAutoIndex;
            double dblAvailableQty;
            string StrReference = null;
            int intPOLInk = 0;
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

                if (ug.Rows.Count==0)
                {
                    MessageBox.Show("Items is empty", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (VendorValidation() == false)
                {
                    MessageBox.Show("Incorrect Vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


                if (WHValidation() == false)
                {
                    MessageBox.Show("Incorrect WareHouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (IsGridValidation() == false)
                {
                    return;
                }
                if (HeaderValidation() == false)
                {
                    return;
                }

               

                if (!IsSerialNoCorrect())
                    return;

                //if (optCash.Checked == true)
                //{
                //    StrPaymmetM = "Cash";
                //}
                //else if (optCredit.Checked == true)
                //{
                //    StrPaymmetM = "Credit";
                //}

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                intPOLInk = GetPOLink(myConnection, myTrans);

                if (user.IsCRTNNoAutoGen)
                {
                    StrReference = GetInvNoField(myConnection, myTrans);
                    UpdatePrefixNo(myConnection, myTrans);
                    txtCreditNo.Text = StrReference;
                }
                bool Active = true;
                StrReference = txtCreditNo.Text.Trim();
                int _LineCount = CalculateLines();
                //chamila
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {

                    SaveDetails(intPOLInk, _LineCount, Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), StrPaymmetM,
                        ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                       double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["AVGC"].Value.ToString()), Convert.ToDateTime(ug.Rows[intGrid].Cells["LastPODate"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), Active, myConnection, myTrans);

                    //--------Check Stock Item-------------
                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                //    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                //    {
                //        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                //        //if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                //        //{
                //        //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                //        //    myTrans.Rollback();
                //        //    return;
                //        //}

                //        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                //        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), myConnection, myTrans);
                //    }
                //    //---------------------------------------
                }

                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                //{
                //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                //        " TranType='Sup-Return',Status='OutOfStock' " +
                //        " where ItemID='" +
                //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                //    myCommandSe1.ExecuteNonQuery();
                //}

                //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbWarehouse.Text.ToString(), txtCreditNo.Text.ToString().Trim(), dtpDate.Value, true, "");


                //--End PH3 Posting--------------------
                //CreatePurchaseOrderJXML(myTrans, myConnection);
                //Connector ObjImportP = new Connector();
                ////ObjImportP.ImportSupplierReturn();
                //ObjImportP.ExportToPeachtreePurchaseOrder();
               
                myTrans.Commit();
                MessageBox.Show("Purchase Order Successfuly Saved.", "Information", MessageBoxButtons.OK,MessageBoxIcon.Information);
               
                // Createfile();
                //  Print(StrReference);
                btnSave.Enabled = false;
                btnProcess.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
              //  ButtonClearAfterSave();
                //  btnNew_Click(sender, e);
                // ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                //btnSave.Focus();
                throw ex;
            }
        }

        private bool WHValidation()
        {
            String S = "Select * from tblWhseMaster where WhseId  = '" + cmbWarehouse.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool VendorValidation()
        {
            String S = "Select * from tblVendorMaster where VendorID  = '" + cmbVendorSelect.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if(dt.Rows.Count >0)
            {
                return true;
            }
            return false;
        }

        private bool IsThisItemSerial(string _ItemCode)
        {
            try
            {
               // if (ug.ActiveRow.Cells[0].Value == null) return false;
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

        public void CreatePurchaseJXMLOld(SqlTransaction tr, SqlConnection con)
        {

            //string INvCreditAcc = "";
            //string InvDebitAcc = "";

            //SqlCommand myCommand4 = new SqlCommand("Select CusretnDrAc,CusretnCrAc from tblDefualtSetting", con, tr);
            //// SqlCommand cmd = new SqlCommand(S);
            //SqlDataAdapter da = new SqlDataAdapter(myCommand4);
            //DataSet dt = new DataSet();
            //da.Fill(dt);

            //for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //{
            //    // APAccount    = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    INvCreditAcc = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
            //    InvDebitAcc = dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim();
            //}

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\CustomerReturn.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            int DistributionNumber = 0;
            int rowCount1 = CalculateLines();
            string NoDistributions = Convert.ToString(rowCount1);



            for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                {
                    if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                    {
                        Writer.WriteStartElement("PAW_Invoice");
                        Writer.WriteAttributeString("xsi:type", "paw:invoice");

                        Writer.WriteStartElement("Customer_ID");
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(cmbVendorSelect.Text.ToString().Trim());//Vendor ID should be here = Ptient No
                        Writer.WriteEndElement();

                        //if (i == 0)
                        //{
                        Writer.WriteStartElement("Invoice_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(txtCreditNo.Text.ToString().Trim());
                        Writer.WriteEndElement();
                        //}                       

                        Writer.WriteStartElement("Date");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");  
                        Writer.WriteString(dtpDate.Value.ToString("MM/dd/yyyy").Trim());//Date 
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Sales_Representative_ID");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(cmbSalesRep.Text.ToString().Trim());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Accounts_Receivable_Account");
                        Writer.WriteString(StrAPAccount);//Cash Account
                        Writer.WriteEndElement();//CreditMemoType

                        Writer.WriteStartElement("CreditMemoType");
                        Writer.WriteString("TRUE");
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Number_of_Distributions");
                        Writer.WriteString((intGrid + 1).ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("SalesLines");

                        Writer.WriteStartElement("SalesLine");

                        Writer.WriteStartElement("InvoiceDistNum");
                        Writer.WriteString(DistributionNumber.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString("-" + ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                        Writer.WriteEndElement();


                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(StrSalesGLAccount);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Unit_Price");
                        ////Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(dgvCustomerReturn.Rows[i].Cells[5].Value.ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                        Writer.WriteEndElement();
                        //========================================================                        
                        Writer.WriteEndElement();
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("AppliedToSO");
                        Writer.WriteString("FALSE");
                        Writer.WriteEndElement();
                    }
                }
            }
            //********************
            Writer.WriteEndElement();//last line
            Writer.Close();


        }

        public void CreatePurchaseOrderJXML(SqlTransaction tr, SqlConnection con)
        {
            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string GRNDate = DTP.ToString(Dformat);


            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\POJournal.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_PurchaseOrders");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            int DistributionNumber = 0;
            int rowCount1 = CalculateLines();
            string NoDistributions = Convert.ToString(rowCount1);

            int rowCount2 = 0;

            if (double.Parse(txtNBT.Value.ToString().Trim()) > 0) rowCount2 = 1;
            if (double.Parse(txtVat.Value.ToString().Trim()) > 0) rowCount2 = 1 + rowCount2;


            Writer.WriteStartElement("PAW_PurchaseOrder");
            Writer.WriteAttributeString("xsi:type", "paw:purchase_orders");

            Writer.WriteStartElement("VendorID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(cmbVendorSelect.Text.ToString().Trim());//Vendor ID should be here = Ptient No
            Writer.WriteEndElement();

            Writer.WriteStartElement("PO_Number");
            Writer.WriteString(txtCreditNo.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:date");
            Writer.WriteString(GRNDate);//Date 
            Writer.WriteEndElement();

            //<PO_Closed>TRUE</PO_Closed>

            Writer.WriteStartElement("PO_Closed");
            Writer.WriteString("FALSE");//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            //	<AP_Account xsi:type="paw:id">85000</AP_Account>

            Writer.WriteStartElement("AP_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrAPAccount);//Cash Account
            Writer.WriteEndElement();//CreditMemoType

            //Writer.WriteStartElement("CreditMemoType");
            //Writer.WriteString("TRUE");
            //Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString((rowCount1 + rowCount2).ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("PurchaseLines");
            for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                {
                    if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                    {
                        Writer.WriteStartElement("PurchaseLine");

                        // < DistributionNumber > 1 </ DistributionNumber >
                        Writer.WriteStartElement("DistributionNumber");
                        Writer.WriteString((intGrid + 1).ToString().Trim());//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Quantity");
                        Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Item_ID");
                        Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Description");
                        Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("GL_Account");
                        Writer.WriteString(ug.Rows[intGrid].Cells["GL"].Value.ToString());
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Unit_Price");
                        Writer.WriteString(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("Tax_Type");
                        //Writer.WriteString("1");//Doctor Charge
                        //Writer.WriteEndElement();

                        //Writer.WriteStartElement("Serial_Number");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(dr["SerialNo"].ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                        Writer.WriteEndElement();

                        //Writer.WriteStartElement("UM_ID");
                        //Writer.WriteAttributeString("xsi:type", "paw:id");
                        //Writer.WriteString(ug.Rows[intGrid].Cells["UOM"].Value.ToString());
                        //Writer.WriteEndElement();

                        Writer.WriteEndElement();
                    }
                }
            }

            if ((double.Parse(txtNBT.Value.ToString().Trim()) > 0))
            {
                Writer.WriteStartElement("PurchaseLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(Tax1ID);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(Tax1Name);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteString(Tax1GLAccount);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtNBT.Value.ToString());//HospitalCharge
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            if ((double.Parse(txtVat.Value.ToString().Trim()) > 0))
            {
                Writer.WriteStartElement("PurchaseLine");

                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("Tax_Type");
                Writer.WriteString("1");//Doctor Charge
                Writer.WriteEndElement();

                Writer.WriteStartElement("Item_ID");
                Writer.WriteString(Tax2ID);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Description");
                Writer.WriteString(Tax2Name);
                Writer.WriteEndElement();

                Writer.WriteStartElement("GL_Account");
                Writer.WriteString(Tax2GLAccount);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtVat.Value.ToString());//tax amount1
                Writer.WriteEndElement();

                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();//last line
            Writer.Close();
        }

        //private void ImportCreditNote()
        //{
        //    try
        //    {
        //        jkhjkhj



        //        XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\CreditNote.xml", System.Text.Encoding.UTF8);
        //        Writer.Formatting = Formatting.Indented;
        //        Writer.WriteStartElement("PAW_Invoices");
        //        Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
        //        Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
        //        Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
        //        // --PH3 Posting--------------------
        //        for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
        //        {
        //            if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
        //            {
        //                if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
        //                {
        //                    DateTime DTP = Convert.ToDateTime(dtpDate.Text);
        //                    string Dformat = "MM/dd/yyyy";
        //                    string InvDate = DTP.ToString(Dformat);

        //                    if (intGrid < ug.Rows.Count)
        //                    {
        //                        Writer.WriteStartElement("PAW_Invoice");
        //                        Writer.WriteAttributeString("xsi:type", "paw:Receipt");

        //                        Writer.WriteStartElement("Customer_ID");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Date");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(GetDateTime(dtpDate.Value));//Date 
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Invoice_Number");
        //                        Writer.WriteString(txtCreditNo.Text.Trim());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Sales_Representative_ID");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Accounts_Receivable_Account");
        //                        Writer.WriteAttributeString("xsi:type", "paw:id");
        //                        Writer.WriteString(cmbARAccount.Text.ToString().Trim());//Cash Account
        //                        Writer.WriteEndElement();//CreditMemoType

        //                        Writer.WriteStartElement("CreditMemoType");
        //                        Writer.WriteString("TRUE");
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("Number_of_Distributions");
        //                        Writer.WriteString((ug.Rows.Count).ToString());
        //                        Writer.WriteEndElement();

        //                        Writer.WriteStartElement("SalesLines");

        //                            Writer.WriteStartElement("SalesLine");

        //                            Writer.WriteStartElement("Quantity");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Item_ID");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Description");
        //                            Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("GL_Account");
        //                            Writer.WriteAttributeString("xsi:type", "paw:id");
        //                            Writer.WriteString(StrSalesGLAccount);
        //                            Writer.WriteEndElement();
        //                            //========================================================
        //                            Writer.WriteStartElement("Tax_Type");
        //                            Writer.WriteString("1");//Doctor Charge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteStartElement("Amount");
        //                            Writer.WriteString("-" + ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());//HospitalCharge
        //                            Writer.WriteEndElement();

        //                            Writer.WriteEndElement();//LINE
        //                            Writer.WriteEndElement();//LINES

        //                        Writer.WriteEndElement();
        //                    }
        //                }
        //            }
        //        }
        //        Writer.Close();

        //        Connector ObjImportP = new Connector();
        //        ObjImportP.ImportDirectSalesInvice();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private int CalculateLines()
        {
            int _Count = 0;
            try
            {
                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                    {
                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                            _Count = _Count + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Count;
            //SaveDetails(ug.Rows.Count, intGrid + 1, StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()),
            //ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(),
            //double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
        }

        private void UpdateItemWarehouse(string StrItemCode, string StrWarehouse, double dblQty, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDetails(int POLINK, int intLineCount, int intLineNo, string StrPayMethod, string StrItemCode, double dblQuantity, string StrItemDescription, string StrUOM, double dblPrice, double dblLineNetAmt, double dblQOH, double dblAVGC, DateTime strLastPODate, string StrGLAccount, bool Active, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                DateTime Date1 = System.DateTime.Now;
                string Dformat = "MM/dd/yyyy";
                string GRNDate = Date1.ToString(Dformat);

                string CurretDate = GRNDate;
                string CurrentTime = System.DateTime.Now.ToShortTimeString();
                StrSql = "INSERT INTO tblPurchaseOrder([POlink],[VendorID],[PONumber],[Date], " +
                    " [WHID],[IsPOClosed],[CustomerSoNo], " +
                    " [AccountPayID],NoOfDistibution,PODisNum, " +
                    " ItemId,Quantity, " +
                    " Description,UOM,[UnitPrice], " +
                    " Discount,Amount,GL_Account, " +
                    " NBT,VAT,GrossTotal, " +
                    " GrandTotal,ISExport,CurretUser, " +
                    " Type ,JobId,GRNQty,RemainQty,IsFullGRN,CurrentDate,CurrentTime,CurrencyName,CurrencyRate,ConvertedAmount,ConvertedTotal,DefaultTotal,QOH,AVGC,LastPurDate,IsActive) " +
                         " VALUES ('" + POLINK + "','" + cmbVendorSelect.Text.ToString().Trim() + "','" + txtCreditNo.Text.Trim() + "','" + (dtpDate.Value.ToString("MM/dd/yyyy")) + "','" +
                         cmbWarehouse.Value.ToString() + "','0','" + txtDescription.Text.ToString().Trim() + "','" +
                         cmbAR.Text.Trim() + "','" + intLineCount + "','" + intLineNo + "','" +
                         StrItemCode + "','" + dblQuantity + "','" +
                         StrItemDescription + "','" + StrUOM + "','" + dblPrice + "','" +
                         txtDiscAmount.Value.ToString().Trim() + "','" + dblLineNetAmt + "','" + StrGLAccount + "','" +
                         txtNBT.Value.ToString().Trim() + "','" + txtVat.Value.ToString().Trim() + "','" + txtSubValue.Value.ToString() + "','" +
                         txtGrossValue.Value.ToString().Trim() + "','False','" + user.userName.ToString().Trim() + "','PURCHASE ORDER','','0','" + dblQuantity + "','0','" + CurretDate + "','" + CurrentTime + "','LKR','1','0','0','0','" + dblQOH + "','" + dblAVGC + "','" + strLastPODate.ToShortDateString().Trim() + "','"+ Active + "')";




                //StrSql = "INSERT INTO tblDirectSupReturn([VendorID],[SupReturnNo],[ReturnDate], " +
                //    " [WarehouseID],[IsApplyToSupInvoice],[SupInvoiceNO], " +
                //    " [APAccount],NoofDistribution,DistributionNo, " +
                //    " ItemID,ReturnQty, " +
                //    " Description,UOM,[UnitCost], " +
                //    " Discount,Amount,GL_Account, " +
                //    " NBT,VAT,GrossTotal, " +
                //    " GrandTotal,ISExport,CurrenUser, " +
                //    " Type ,SalesRep) " +
                //         " VALUES ('" + cmbVendorSelect.Value.ToString() + "','" + txtCreditNo.Text.Trim() + "','" + (dtpDate.Value.ToString("MM/dd/yyyy")) + "','" +
                //         cmbWarehouse.Value.ToString() + "','0','DirectReturn','" +
                //         cmbAR.Text.Trim() + "','" + intLineCount + "','" + intLineNo + "','" +
                //         StrItemCode + "','" + dblQuantity + "','" +
                //         StrItemDescription + "','" + StrUOM + "','" + dblPrice + "','" +
                //         txtDiscAmount.Value.ToString().Trim() + "','" + dblLineNetAmt + "','" + StrGLAccount + "','" +
                //         txtNBT.Value.ToString().Trim() + "','" + txtVat.Value.ToString().Trim() + "','" + txtSubValue.Value.ToString() + "','" +
                //         txtGrossValue.Value.ToString().Trim() + "','False','" + user.userName.ToString().Trim() + "','DirectReurn','')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnEditer.Enabled = false;
            try
            {
                //chamila
                if (IsUpdate == false)
                {
                    GetCRNNo();
                    SaveEvent();
                }
                else if (IsUpdate == true)
                {

                    UpdateEvent();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void UpdateEvent()
        {
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

            string TranType = "Purchase-Order";
            int DocType = 7;
            bool QtyIN = false;

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Incorrect vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int intGrid;
            int intAutoIndex;
            double dblAvailableQty;
            string StrReference = null;
            int intPOLInk = 0;
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
                if (ug.Rows.Count == 0)
                {
                    MessageBox.Show("Items is empty", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (IsGridValidation() == false)
                {
                    return;
                }
                if (HeaderValidation() == false)
                {
                    return;
                }

                if (!IsSerialNoCorrect())
                    return;

                //if (optCash.Checked == true)
                //{
                //    StrPaymmetM = "Cash";
                //}
                //else if (optCredit.Checked == true)
                //{
                //    StrPaymmetM = "Credit";
                //}

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                intPOLInk = GetPOLink(myConnection, myTrans);

                if (user.IsCRTNNoAutoGen)
                {
                    StrReference = GetInvNoField(myConnection, myTrans);
                  //  UpdatePrefixNo(myConnection, myTrans);
                   // txtCreditNo.Text = StrReference;
                }

                StrSql = "DELETE FROM [tblPurchaseOrder] WHERE PONumber='" + txtCreditNo.Text.ToString().Trim() + "'";
                SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                command1.CommandType = CommandType.Text;
                command1.ExecuteNonQuery();

                StrReference = txtCreditNo.Text.Trim();
                int _LineCount = CalculateLines();
                //chamila

                bool Active = true;
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(intPOLInk, _LineCount, Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), StrPaymmetM,
                        ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                       double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["AVGC"].Value.ToString()), Convert.ToDateTime(ug.Rows[intGrid].Cells["LastPODate"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), Active, myConnection, myTrans);

                    //--------Check Stock Item-------------
                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    //    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    //    {
                    //        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                    //        //if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                    //        //{
                    //        //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                    //        //    myTrans.Rollback();
                    //        //    return;
                    //        //}

                    //        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                    //        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), myConnection, myTrans);
                    //    }
                    //    //---------------------------------------
                }

                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                //{
                //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                //        " TranType='Sup-Return',Status='OutOfStock' " +
                //        " where ItemID='" +
                //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                //    myCommandSe1.ExecuteNonQuery();
                //}

                //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbWarehouse.Text.ToString(), txtCreditNo.Text.ToString().Trim(), dtpDate.Value, true, "");


                //--End PH3 Posting--------------------
                //CreatePurchaseOrderJXML(myTrans, myConnection);
                //Connector ObjImportP = new Connector();
                ////ObjImportP.ImportSupplierReturn();
                //ObjImportP.ExportToPeachtreePurchaseOrder();

                myTrans.Commit();
                MessageBox.Show("Purchase Order Successfuly Saved.", "Information", MessageBoxButtons.OK,MessageBoxIcon.Information);
              
                // Createfile();
                //Print(StrReference);
                btnSave.Enabled = false;
                btnProcess.Enabled = true;
                btnNew.Enabled = true;
                //  ButtonClearAfterSave();
                //  btnNew_Click(sender, e);
                // ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                //btnSave.Focus();
                throw ex;
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

        private void InvTransaction(string strCRNNo, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, double SellingPrice, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],                [UnitCost],         [TotalCost],        [WarehouseID],[SellingPrice]) " +
                    " VALUES(7,'" + strCRNNo + "','" + GetDateTime(dtDate) + "','Sup-Return','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + SellingPrice + "')";

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
                frmPurchaseOrderList ObjfrmPurchaseOderSearch = new frmPurchaseOrderList();
                ObjfrmPurchaseOderSearch.ShowDialog();

                setValue();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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

        private void Print(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                if (strCRNNo != "")
                {
                    DSPurchaseOder.Clear();
                    DsCustomerReturn.Clear();
                    String S1 = "Select * from tblPurchaseOrder WHERE PONumber = '" + txtCreditNo.Text.ToString().Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    //da1.Fill(ds, "DTReturn");
                    da1.Fill(DSPurchaseOder.dtPurchaseOrder);

                    String S4 = "Select CompanyName,Address1,Address2,City,State,Zip,Country,Telephone,Fax,WebSite,Email from tblCompanyInformation";
                    SqlCommand cmd4 = new SqlCommand(S4);
                    SqlConnection con4 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da4 = new SqlDataAdapter(S4, con4);
                    //da4.Fill(ds, "dt_CompanyDetails");
                    da4.Fill(DSPurchaseOder.dt_CompanyDetails);

                    String S5 = "Select [VendorID],[VendorName],[VContact],[VAddress1],[VAddress2] from tblVendorMaster  WHERE VendorID='" + cmbVendorSelect.Text.ToString().Trim() + "'";
                    SqlCommand cmd5 = new SqlCommand(S5);
                    SqlConnection con5 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da5 = new SqlDataAdapter(S5, con5);
                    //da4.Fill(ds, "dt_CompanyDetails");
                    da5.Fill(DSPurchaseOder.DTVendor);

                    // DirectPrint();

                    frmViewePurchaseOder objfrmViewerDirectSupReturen = new frmViewePurchaseOder(this);
                    objfrmViewerDirectSupReturen.Show();

                    //frmViewerCreditNote cusReturn = new frmViewerCreditNote(this);
                    //cusReturn.Show();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void DirectPrint()
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();

                if (File.Exists(Application.StartupPath + "\\rptDirectSupReturn.rpt") == true)
                {
                    Myfullpath = Path.GetFullPath("rptDirectSupReturn.rpt");
                }
                else
                {
                    MessageBox.Show("rptDirectSupReturn.rpt not Exists.");
                    this.Close();
                    return;
                }
                crp.Load(Myfullpath);
                crp.SetDataSource(DsCustomerReturn);
                crp.PrintToPrinter(1, true, 0, 0);

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
                Print(txtCreditNo.Text);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ClearFooter()
        {
            try
            {
                //txtNBTPer.Value = 0;
                //txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearHeader()
        {
            try
            {
                cmbWarehouse.Value = user.StrDefaultWH;
                GetItemDataSet();
                GetItemDescriptionDataSet();
                txtDescription.Text = "";
                dtpDate.Value = DateTime.Now;
                // txtWarehouseName.Text = "";
                cmbVendorSelect.Text = "";
                txtCustomer.Text = "";
                cmbSalesRep.Text = "";
                txtSalesRep.Text = "";
                txtAddress1.Text = "";

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
                // cmbWarehouse_Leave(se);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableFoter(Boolean blnEnable)
        {
            try
            {
                txtVatPer.Enabled = blnEnable;
                txtNBTPer.Enabled = blnEnable;
                txtDescription.Enabled = blnEnable;
                txtDiscPer.Enabled = blnEnable;
                ug.Enabled = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EnableHeader(Boolean blnEnable)
        {
            try
            {
                cmbWarehouse.Enabled = blnEnable;
                dtpDate.Enabled = blnEnable;
                txtWarehouseName.Enabled = false;

                cmbVendorSelect.Enabled = blnEnable;
                txtCustomer.Enabled = false;

                cmbSalesRep.Enabled = blnEnable;
                txtSalesRep.Enabled = false;

                cmbAR.Enabled = blnEnable;
                optCash.Enabled = blnEnable;
                optCredit.Enabled = blnEnable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void ButtonClearAfterSave()
        {
            try
            {
                btnSave.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSNO.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);
                btnProcess.Enabled = false;
                ClearHeader();
                DeleteRows();
                GetCRNNo();
                // GetCustomer();
                // GetSalesRep();
                LoadtaxDetails();
                loadChartofAcount();
                LoadDefualtAccount();
                cmbVendorSelect.Focus();

                if (user.IsCRTNNoAutoGen) txtCreditNo.ReadOnly = true;
                else txtCreditNo.ReadOnly = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ButtonClear()
        {
            try
            {
                btnSave.Enabled = false;
                btnNew.Enabled = true;
                btnPrint.Enabled = true;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;

                ClearHeader();
                EnableHeader(false);
                EnableFoter(false);
                DeleteRows();
                GetCRNNo();
                ug.Enabled = false;
                intEstomateProcode = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonClear();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
            try
            {
                e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double ChangePrice2(double dblPriceList2)
        {
            return dblPriceList2;
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {

                if (Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value) == 0.00)
                {
                    e.Cell.Row.Cells["TotalPrice"].Value = 0.00;
                    GrandTotal();
                    return;
                }
                if(e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity")
                {
                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    GrandTotal();
                }

                if (e.Cell.Column.Key == "TotalPrice" || e.Cell.Column.Key == "Quantity")
                {
                    e.Cell.Row.Cells["UnitPrice"].Value = LineCalculationUnitPrice(Convert.ToDouble(e.Cell.Row.Cells["TotalPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    GrandTotal();
                }

              
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Edit()
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean IsGridExitCode(String StrCode)
        {
            try
            {
                GetItemDataSet();
                int c = ultraCombo1.Rows.Count;
                foreach (UltraGridRow ugR in ultraCombo1.Rows)
                {
                    if (ugR.Cells["ItemId"].Text == StrCode)
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
                DeleteRows();
                ClearFooter();
                GetItemDataSet();
                GetItemDescriptionDataSet();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QOH"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["AVGC"].Value = ugR.Cells["AVGQty"].Value;

                                ug.ActiveCell.Row.Cells["LastPODate"].Value = Convert.ToDateTime(ugR.Cells["LastPurDate"].Value.ToString()).ToShortDateString();


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
                                //  ug.ActiveCell.Row.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemID"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QOH"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitCost"].Value;

                                ug.ActiveCell.Row.Cells["AVGC"].Value = ugR.Cells["AVGQty"].Value;

                                ug.ActiveCell.Row.Cells["LastPODate"].Value = Convert.ToDateTime(ugR.Cells["LastPurDate"].Value.ToString()).ToShortDateString();
                            }
                        }
                    }

                    HideSelectedRow();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                GetCRNNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void optSerialOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetCRNNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
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
                        StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                        StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        StrCashAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                        StrAPAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //public void GetSalesRep()
        //{
        //    dsSalesRep = new DataSet();
        //    try
        //    {
        //        dsSalesRep.Clear();
        //        StrSql = " SELECT RepCode, RepName FROM tblSalesRep order by RepCode";

        //        SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
        //        dAdapt.Fill(dsSalesRep, "DtSalesRep");

        //        cmbSalesRep.DataSource = dsSalesRep.Tables["DtSalesRep"];
        //        cmbSalesRep.DisplayMember = "RepCode";
        //        cmbSalesRep.ValueMember = "RepCode";
        //        cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
        //        cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void btnclose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
                HideSelectedRow();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbCustomer_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtCustomer.Text = cmbVendorSelect.ActiveRow.Cells[1].Value.ToString();
                        txtAddress1.Text = cmbVendorSelect.ActiveRow.Cells[2].Value.ToString() + ", " + cmbVendorSelect.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public void LoadDefualtAccount()
        {
            try
            {
                String S = "Select APAccount from tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbAR.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                    // cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void loadChartofAcount()
        {
            try
            {
                String S = "Select * from tblChartofAcounts";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbAR.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void cmbSalesRep_RowSelected(object sender, RowSelectedEventArgs e)
        {
            //try
            //{
            //    if (e.Row != null)
            //    {
            //        if (e.Row.Activated == true)
            //        {
            //            txtSalesRep.Text = cmbSalesRep.ActiveRow.Cells[1].Value.ToString();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void txtDiscPer_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            //btnSave.Focus();
        }

        private void txtCreditNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //ug.DataSource = null;
                string ConnString = ConnectionString;
                String S1 = "SELECT tblCutomerReturn.*, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemMaster.Categoty, " +
                    " tblItemMaster.PriceLevel1, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, " +
                    " tblItemMaster.PriceLevel5, tblItemMaster.UnitCost,isnull(tblItemWhse.QTY,0)as QTY" +
                    " FROM            tblCutomerReturn LEFT OUTER JOIN " +
                    " tblItemMaster ON tblCutomerReturn.ItemID = tblItemMaster.ItemID LEFT OUTER JOIN " +
                    " tblItemWhse ON tblCutomerReturn.ItemID = tblItemWhse.ItemId AND tblCutomerReturn.LocationID = tblItemWhse.WhseId where CreditNo='" + txtCreditNo.Text.Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbVendorSelect.Text = dt.Rows[0]["CustomerID"].ToString();
                    //cmbSalesRep.Text = dt.Rows[0][""].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["LocationID"].ToString();
                    cmbAR.Text = dt.Rows[0]["ARAccount"].ToString();
                    txtDiscAmount.Value = dt.Rows[0]["Discount"].ToString();

                    txtDiscPer.Value = ((double.Parse(dt.Rows[0]["Discount"].ToString())) * 100 / (double.Parse(dt.Rows[0]["GrossTotal"].ToString()))).ToString("0.00");
                    txtSubValue.Value = dt.Rows[0]["GrossTotal"].ToString();
                    txtGrossValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    txtNetValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["ReturnDate"].ToString());

                    foreach (DataRow dr in dt.Rows)
                    {
                        UltraGridRow ugR;
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                        ugR.Cells["Description"].Value = dr["Description"].ToString();
                        ugR.Cells["UnitPrice"].Value = dr["UnitPrice"].ToString();
                        ugR.Cells["Quantity"].Value = dr["ReturnQty"].ToString();
                        ugR.Cells["TotalPrice"].Value = dr["Amount"].ToString();
                        ugR.Cells["OnHand"].Value = Convert.ToDouble(dr["QTY"]);
                        ugR.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                        ugR.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                        ugR.Cells["UOM"].Value = dr["UOM"].ToString();
                        ugR.Cells["Categoty"].Value = dr["Categoty"].ToString();
                        ugR.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                        ugR.Cells["PriceLevel1"].Value = dr["PriceLevel1"].ToString();
                        ugR.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                        ugR.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                        ugR.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                        ugR.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public int GetItemClass(string Item_ID)
        {

            string ConnString = ConnectionString;
            String S1 = "SELECT ItemClass from tblItemMaster where ItemID='" + Item_ID + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["ItemClass"].ToString());
            }
            else
            {
                return 5;
            }

        }

        public string GetItemCategory(string Item_ID)
        {

            string ConnString = ConnectionString;
            String S1 = "SELECT Categoty from tblItemMaster where ItemID='" + Item_ID + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Categoty"].ToString();
            }
            else
            {
                return "Pharmay";
            }

        }


        //Categoty

        private void SetSerchValues()
        {


            try
            {
                //ug.DataSource = null;
                string ConnString = ConnectionString;
                String S1 = "SELECT * from tblPurchaseOrder where PONumber='" + txtCreditNo.Text.Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbVendorSelect.Text = dt.Rows[0]["VendorID"].ToString();
                    //cmbSalesRep.Text = dt.Rows[0][""].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["WHID"].ToString();
                    // cmbAR.Text = dt.Rows[0]["APAccount"].ToString();
                    txtDiscAmount.Value = dt.Rows[0]["Discount"].ToString();
                    IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                    if(IsActive ==true)
                    {
                        btnEditer.Enabled = true;
                        btnSave.Enabled = false;
                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        btnProcess.Enabled = false;
                        btnEditer.Enabled = false;
                        btnSave.Enabled = false;
                        btnPrint.Enabled = true;
                    }

                    //if (double.Parse(dt.Rows[0]["GrossTotal"].ToString()) != 0 || double.Parse(dt.Rows[0]["GrandTotal"].ToString()) != 0)

                    //{
                    txtDiscPer.Value = ((double.Parse(dt.Rows[0]["Discount"].ToString())) * 100 / (double.Parse(dt.Rows[0]["GrossTotal"].ToString()))).ToString("0.00");
                    txtSubValue.Value = dt.Rows[0]["GrossTotal"].ToString();
                    txtGrossValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    txtNetValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["Date"].ToString());

                    txtNBTPer.Value = (double.Parse(dt.Rows[0]["NBT"].ToString())).ToString("0.000");
                    txtVatPer.Value = (double.Parse(dt.Rows[0]["VAT"].ToString())).ToString("0.000");

                    foreach (DataRow dr in dt.Rows)
                    {
                        UltraGridRow ugR;
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                        ugR.Cells["Description"].Value = dr["Description"].ToString();
                        ugR.Cells["UnitPrice"].Value = dr["UnitPrice"].ToString();
                        ugR.Cells["Quantity"].Value = dr["Quantity"].ToString();
                        ugR.Cells["TotalPrice"].Value = dr["Amount"].ToString();
                        ugR.Cells["OnHand"].Value = Convert.ToDouble(dr["QOH"]);
                        ugR.Cells["ItemClass"].Value = GetItemClass(dr["ItemID"].ToString());
                        ugR.Cells["GL"].Value = dr["GL_Account"].ToString();
                        ugR.Cells["UOM"].Value = dr["UOM"].ToString();
                        ugR.Cells["Categoty"].Value = GetItemCategory(dr["ItemID"].ToString());
                        ugR.Cells["CostPrice"].Value = dr["UnitPrice"].ToString();
                        ugR.Cells["PriceLevel1"].Value = 0.00;
                        ugR.Cells["PriceLevel2"].Value = 0.00;
                        ugR.Cells["PriceLevel3"].Value = 0.00;
                        ugR.Cells["PriceLevel4"].Value = 0.00;
                        ugR.Cells["PriceLevel5"].Value = 0.00;
                       
                        ugR.Cells["AVGC"].Value = dr["AVGC"].ToString();
                        ugR.Cells["LastPODate"].Value = Convert.ToDateTime(dr["LastPurDate"].ToString()).ToShortDateString();



                    }

                }

                if(CheckSpecailEditOption()==true && IsActive == false && CheckPoHaveSupInvoice(txtCreditNo.Text)==false)
                {
                    btnEditer.Enabled = true;
                    specialedit = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckPoHaveSupInvoice(string pono)
        {
            String S1 = "SELECT * from tblSupplierInvoices where PONO='" + pono + "' and IsActive ='"+true+"'";

            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        bool specialedit = false;
        private bool CheckSpecailEditOption()
        {
            dtUser = DataAccess.Access.setUserAuthentication(user.userName, "frmPurchaseOder");
            if (dtUser.Rows.Count > 0)
            {
                if(Convert.ToBoolean(dtUser.Rows[0].ItemArray[7].ToString())==true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
             
            }
            return false;
        }

        private void txtCreditNo_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                //ug.DataSource = null;
                string ConnString = ConnectionString;
                String S1 = "SELECT * FROM tblPurchaseOrder where PONumber='" + txtCreditNo.Text.ToString().Trim() + "'";



                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbVendorSelect.Text = dt.Rows[0]["VendorID"].ToString();
                    //cmbSalesRep.Text = dt.Rows[0][""].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["WHID"].ToString();
                    //  cmbAR.Text = dt.Rows[0]["APAccount"].ToString();
                    txtDiscAmount.Value = dt.Rows[0]["Discount"].ToString();

                    txtDiscPer.Value = ((double.Parse(dt.Rows[0]["Discount"].ToString())) * 100 / (double.Parse(dt.Rows[0]["GrossTotal"].ToString()))).ToString("0.00");
                    txtSubValue.Value = dt.Rows[0]["GrossTotal"].ToString();
                    txtGrossValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    txtNetValue.Value = dt.Rows[0]["GrandTotal"].ToString();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["Date"].ToString());

                    txtNBTPer.Value = (double.Parse(dt.Rows[0]["NBT"].ToString())).ToString("0.000");
                    txtVatPer.Value = (double.Parse(dt.Rows[0]["VAT"].ToString())).ToString("0.000");

                    foreach (DataRow dr in dt.Rows)
                    {
                        UltraGridRow ugR;
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = ugR.Index + 1;
                        ugR.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                        ugR.Cells["Description"].Value = dr["Description"].ToString();
                        ugR.Cells["UnitPrice"].Value = dr["UnitPrice"].ToString();
                        ugR.Cells["Quantity"].Value = dr["Quantity"].ToString();
                        ugR.Cells["TotalPrice"].Value = dr["Amount"].ToString();
                        // ugR.Cells["OnHand"].Value = Convert.ToDouble(dr["QOH"]);
                        // ugR.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                        // ugR.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                        // ugR.Cells["UOM"].Value = dr["UOM"].ToString();
                        // ugR.Cells["Categoty"].Value = dr["Categoty"].ToString();
                        // ugR.Cells["CostPrice"].Value = dr["UnitPrice"].ToString();
                        /// ugR.Cells["PriceLevel1"].Value = dr["PriceLevel1"].ToString();
                        // ugR.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                        // ugR.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                        // ugR.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                        // ugR.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        //private void Createfile()
        //{
        //    DialogResult reply1 = MessageBox.Show("Are you sure, you want to Create a Transfer File Now ? ", "Information", MessageBoxButtons.OKCancel);

        //    if (reply1 == DialogResult.Cancel)
        //    {
        //        //return;
        //    }
        //    else if (reply1 == DialogResult.OK)
        //    {
        //        ClassDriiDown.strSupplierReturnDirectNo = txtCreditNo.Text.ToString().Trim();
        //       // frmCreateDirectReturnFile ObjfrmCreateDirectReturnFile = new frmCreateDirectReturnFile();
        //      //  ObjfrmCreateDirectReturnFile.ShowDialog();
        //    }
        //}
        private void btnCreateFile_Click(object sender, EventArgs e)
        {
            //  Createfile();
        }

        private void LoadtaxDetails()
        {
            try
            {
                bool Active = true;
                string ConnString = ConnectionString;
                String S1 = "Select TaxName,Rate,Account,TaxID from tblTaxApplicable where IsActive='" + Active + "' order by Rank"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.000");
                    txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.000");

                    Tax1ID = dt.Rows[0]["TaxID"].ToString();
                    Tax2ID = dt.Rows[1]["TaxID"].ToString();

                    Tax1Name = dt.Rows[0]["TaxName"].ToString();
                    Tax2Name = dt.Rows[1]["TaxName"].ToString();

                    TaxRate = double.Parse(dt.Rows[0]["Rate"].ToString());
                    TaxRate1 = double.Parse(dt.Rows[1]["Rate"].ToString());

                    Tax1GLAccount = user.TaxPayGL1;
                    Tax2GLAccount = user.TaxPayGL2;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
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
                    frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Sup-Return",
                        cmbWarehouse.Text.ToString().Trim(),
                       ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                       Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                       txtCreditNo.Text.Trim(), blnEdit,
                       clsSerializeItem.DtsSerialNoList, null, true, true);
                    ObjfrmSerialSubCommon.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_ClickCell(object sender, ClickCellEventArgs e)
        {
            try
            {
               
                if (IsThisItemSerial(ug.ActiveRow.Cells[1].Value.ToString().Trim()))
                    btnSNO.Enabled = true;
                else
                    btnSNO.Enabled = false;
            }
            catch(Exception)
            {
               // MessageBox.Show(ex.Message);
            }
        }

        private void frmPurchaseOder_Activated(object sender, EventArgs e)
        {
            if (Search.searchPONo != "")
            {

            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            btnEditer.Enabled = false;
            DeleteEmpGrid();
            try
            {

                if (specialedit == true)
                {
                   
                    if (CheckActiveStatus())
                    {
                        if(CheckAllGrnItemsisInclude())
                        {
                            if (GrideValidationForSpecialEdit())
                            {
                                ProcessEvent();
                                UpdatePOStatus();
                              
                            }
                        }
                      
                        

                    }
                    else
                    {
                        ProcessEvent();
                    }
                   
                }
                else
                {
                    //chamila
                    //GetCRNNo();
                    ProcessEvent();
                }
            }
            catch (Exception ex)
            {
               objclsCommon.ErrorLog("Purchase Order", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void UpdatePOStatus()
        {
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            myConnection.Open();
            SqlTransaction myTrans = myConnection.BeginTransaction();

            try
            {
                string sql = "update tblPurchaseOrder set IsFullGRN='" + false + "' , IsPOClosed='" + false + "' where PONumber ='"+txtCreditNo.Text.ToString()+"'";
                SqlCommand myCommand2 = new SqlCommand(sql, myConnection, myTrans);
                myCommand2.ExecuteNonQuery();
                myTrans.Commit();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                myTrans.Rollback();
            }
        }

        private bool CheckAllGrnItemsisInclude()
        {
            String S1 = "Select ItemID,Description,GRN_NO from [tblGRNTran] where [PONos]='" + txtCreditNo.Text.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);
            int z = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
             {
                for (int x = 0; x < ug.Rows.Count; x++)
                {
                    if(dt.Rows[i].ItemArray[0].ToString()== ug.Rows[x].Cells[1].Value.ToString()|| dt.Rows[i].ItemArray[1].ToString() == ug.Rows[x].Cells[2].Value.ToString())
                    {
                         z = 1;
                    }
                }
                
                if(z==0)
                {
                    MessageBox.Show("Please Enter  Item :"+ dt.Rows[i].ItemArray[0].ToString()+" To This PO Because it Already used in Good Recive Note :"+ dt.Rows[i].ItemArray[2].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                z = 0;
            }
           
            return true;
        }
        private bool GrideValidationForSpecialEdit()
        {
           for(int x=0;x<ug.Rows.Count;x++)
            {
                String S1 = "Select GRNQty from [tblPurchaseOrder] where [PONumber]='" + txtCreditNo.Text.ToString().Trim() + "' and ItemId ='" + ug.Rows[x].Cells[1].Value.ToString() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    
                   
                   if( Convert.ToDouble(ug.Rows[x].Cells[4].Value) < Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()))
                    {
                        int m1 = x + 1;
                        string m2 = (Convert.ToDouble(dt.Rows[0].ItemArray[0])).ToString("N2");
                        MessageBox.Show("Line No :"+m1+" Invalid Qty Your Already Have Good Recived Note for This Item with Qty :"+m2 , "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                }
            }
            return true;
        }

        private  bool CheckActiveStatus()
        {
            String S1 = "Select * from [tblGRNTran] where [PONos]='" + txtCreditNo.Text.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;

        }
        bool processFinished = false;
        private void ProcessEvent()
        {
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

            string TranType = "Purchase-Order";
            int DocType = 7;
            bool QtyIN = false;

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbVendorSelect.Value == null)
            {
                MessageBox.Show("Incorrect vendor", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int intGrid;
            int intAutoIndex;
            double dblAvailableQty;
            string StrReference = null;
            int intPOLInk = 0;
            int intItemClass;
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;
            try
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to Process this record ? ", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                DeleteEmpGrid();
                if (ug.Rows.Count == 0)
                {
                    MessageBox.Show("Items is empty", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (IsGridValidation() == false)
                {
                    return;
                }
                if (HeaderValidation() == false)
                {
                    return;
                }

                if (!IsSerialNoCorrect())
                    return;

                //if (optCash.Checked == true)
                //{
                //    StrPaymmetM = "Cash";
                //}
                //else if (optCredit.Checked == true)
                //{
                //    StrPaymmetM = "Credit";
                //}

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

               // intPOLInk = GetPOLink(myConnection, myTrans);

                if (user.IsCRTNNoAutoGen)
                {
                    StrReference = GetInvNoField(myConnection, myTrans);
                   // UpdatePrefixNo(myConnection, myTrans);
                  //  txtCreditNo.Text = StrReference;
                }

                if (specialedit == false)
                {
                    StrSql = "DELETE FROM [tblPurchaseOrder] WHERE PONumber='" + txtCreditNo.Text.ToString().Trim() + "'";
                    SqlCommand command1 = new SqlCommand(StrSql, myConnection, myTrans);
                    command1.CommandType = CommandType.Text;
                    command1.ExecuteNonQuery();
                }

                StrReference = txtCreditNo.Text.Trim();
                int _LineCount = CalculateLines();

                bool Active = false;
                //chamila
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (specialedit == false)
                    {
                        SaveDetails(intPOLInk, _LineCount, Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), StrPaymmetM,
                            ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                            ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                            double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                           double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["AVGC"].Value.ToString()), Convert.ToDateTime(ug.Rows[intGrid].Cells["LastPODate"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), Active, myConnection, myTrans);
                    }
                   
                    if(specialedit ==true)
                    {

                        DateTime Date1 = System.DateTime.Now;
                        string Dformat = "MM/dd/yyyy";
                        string GRNDate = Date1.ToString(Dformat);

                        string CurretDate = GRNDate;
                        string CurrentTime = System.DateTime.Now.ToShortTimeString();

                        string S = "exec POSpecialEdit '" + intPOLInk + "','" + cmbVendorSelect.Text.ToString().Trim() + "','" + txtCreditNo.Text.Trim() + "','" + (dtpDate.Value.ToString("MM / dd / yyyy")) + "','" +
       cmbWarehouse.Value.ToString() + "','" + txtDescription.Text.ToString().Trim() + "','" + cmbAR.Text.Trim() + "','" + _LineCount + "','" + Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()) + "','" +
        ug.Rows[intGrid].Cells["ItemCode"].Value.ToString() + "','" + double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) + "','" +
              ug.Rows[intGrid].Cells["Description"].Value.ToString() + "','" + ug.Rows[intGrid].Cells["UOM"].Value.ToString() + "','" + double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()) + "','" +
       txtDiscAmount.Value.ToString().Trim() + "','" + double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()) + "','" + ug.Rows[intGrid].Cells["GL"].Value.ToString() + "','" +
       txtNBT.Value.ToString().Trim() + "','" + txtVat.Value.ToString().Trim() + "','" + txtSubValue.Value.ToString() + "','" +
       txtGrossValue.Value.ToString().Trim() + "','" + user.userName.ToString().Trim() + "','" + CurretDate + "','" + CurrentTime + "','" + double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()) + "','" + double.Parse(ug.Rows[intGrid].Cells["AVGC"].Value.ToString()) + "','" + Convert.ToDateTime(ug.Rows[intGrid].Cells["LastPODate"].Value).ToShortDateString().Trim() + "'";

                        SqlCommand command = new SqlCommand(S, myConnection, myTrans);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }


                    //--------Check Stock Item-------------
                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    //    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    //    {
                    //        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                    //        //if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                    //        //{
                    //        //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                    //        //    myTrans.Rollback();
                    //        //    return;
                    //        //}

                    //        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                            //InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), myConnection, myTrans);
                    //    }
                    //    //---------------------------------------
                }

                //foreach (DataRow dr in clsSerializeItem.DtsSerialNoList.Rows)
                //{
                //    SqlCommand myCommandSe1 = new SqlCommand("update  tblSerialItemTransaction set " +
                //        " TranType='Sup-Return',Status='OutOfStock' " +
                //        " where ItemID='" +
                //        dr["ItemCode"].ToString() + "' and WareHouseID='" + cmbWarehouse.Text.ToString().Trim() + "' and SerialNo='" +
                //        dr["SerialNo"].ToString() + "'", myConnection, myTrans);
                //    myCommandSe1.ExecuteNonQuery();
                //}

                //frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList, "Sup-Return", cmbWarehouse.Text.ToString(), txtCreditNo.Text.ToString().Trim(), dtpDate.Value, true, "");


                //--End PH3 Posting--------------------
               CreatePurchaseOrderJXML(myTrans, myConnection);
               Connector ObjImportP = new Connector();
                //ObjImportP.ImportSupplierReturn();


                string[] TransToDelete = new string[2];
                TransToDelete[0] = cmbVendorSelect.Text.ToString();
                TransToDelete[1] = txtCreditNo.Text.ToString();


                ObjImportP.ExportToPeachtreePurchaseOrder(specialedit, TransToDelete);

                myTrans.Commit();
                MessageBox.Show("Purchase Order Successfuly Processed.", "Information", MessageBoxButtons.OK,MessageBoxIcon.Information);
                // Createfile();
                Print(StrReference);
                ButtonClearAfterSave();
                specialedit = false;
                processFinished = true;
                  btnNew_Click(null, null);
                // ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
            //    btnSave.Focus();
                throw ex;
            }
        }

        private void SaveDetailsAfterSpecialEdit(int POLINK, int intLineCount, int intLineNo, string StrPayMethod, string StrItemCode, double dblQuantity, string StrItemDescription, string StrUOM, double dblPrice, double dblLineNetAmt, double dblQOH, double dblAVGC, DateTime strLastPODate, string StrGLAccount, bool Active,double grnqty,double remainqty, SqlConnection con, SqlTransaction Trans)
        {

            SqlCommand cmd = con.CreateCommand();
            cmd.Connection = con;
            cmd.Transaction = Trans;
          

            dblQuantity = dblQuantity - grnqty;
            StrSql = "DELETE FROM [tblPurchaseOrder] WHERE PONumber='" + txtCreditNo.Text.ToString().Trim() + "' and ItemId='" + StrItemCode + "'";
            cmd.CommandText = StrSql;
            cmd.ExecuteNonQuery();


            DateTime Date1 = System.DateTime.Now;
            string Dformat = "MM/dd/yyyy";
            string GRNDate = Date1.ToString(Dformat);

            string CurretDate = GRNDate;
            string CurrentTime = System.DateTime.Now.ToShortTimeString();
           string StrSql2 = "INSERT INTO tblPurchaseOrder([POlink],[VendorID],[PONumber],[Date], " +
                " [WHID],[IsPOClosed],[CustomerSoNo], " +
                " [AccountPayID],NoOfDistibution,PODisNum, " +
                " ItemId,Quantity, " +
                " Description,UOM,[UnitPrice], " +
                " Discount,Amount,GL_Account, " +
                " NBT,VAT,GrossTotal, " +
                " GrandTotal,ISExport,CurretUser, " +
                " Type ,JobId,GRNQty,RemainQty,IsFullGRN,CurrentDate,CurrentTime,CurrencyName,CurrencyRate,ConvertedAmount,ConvertedTotal,DefaultTotal,QOH,AVGC,LastPurDate,IsActive) " +
                     " VALUES ('" + POLINK + "','" + cmbVendorSelect.Text.ToString().Trim() + "','" + txtCreditNo.Text.Trim() + "','" + (dtpDate.Value.ToString("MM/dd/yyyy")) + "','" +
                     cmbWarehouse.Value.ToString() + "','0','" + txtDescription.Text.ToString().Trim() + "','" +
                     cmbAR.Text.Trim() + "','" + intLineCount + "','" + intLineNo + "','" +
                     StrItemCode + "','" + dblQuantity + "','" +
                     StrItemDescription + "','" + StrUOM + "','" + dblPrice + "','" +
                     txtDiscAmount.Value.ToString().Trim() + "','" + dblLineNetAmt + "','" + StrGLAccount + "','" +
                     txtNBT.Value.ToString().Trim() + "','" + txtVat.Value.ToString().Trim() + "','" + txtSubValue.Value.ToString() + "','" +
                     txtGrossValue.Value.ToString().Trim() + "','False','" + user.userName.ToString().Trim() + "','PURCHASE ORDER','','"+grnqty+"','" + remainqty+ "','0','" + CurretDate + "','" + CurrentTime + "','LKR','1','0','0','0','" + dblQOH + "','" + dblAVGC + "','" + strLastPODate.ToShortDateString().Trim() + "','" + Active + "')";
            cmd.CommandText = StrSql2;
            cmd.ExecuteNonQuery();

         
        }

        private void btnEditer_Click(object sender, EventArgs e)
        {

            GetItemDataSet();
            GetItemDescriptionDataSet();
            HideSelectedRow();
            btnEditer.Enabled = false;
            dtpDate.Enabled = true;
            if (IsActive == true && specialedit ==false)
            {
                IsUpdate = true;
             
                btnSave.Enabled = true;
                btnProcess.Enabled = false;
                btnProcess.Enabled = true;
            }


            else if (IsActive == false)
            {
                if (specialedit == true)
                {
                    btnProcess.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnProcess.Enabled = false;
                }
             

            }
        }

        private void txtCreditNo_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void ultraCombo2_RowSelected(object sender, RowSelectedEventArgs e)
        {

            ////  ug.ActiveCell.Row.Cells["ItemCode"].Value = dr["ItemID"].ToString();
            //try
            //{
            //    ug.ActiveCell.Row.Cells["Description"].Value = ultraCombo2.SelectedRow.Cells["ItemDis"].Value;
            //    ug.ActiveCell.Row.Cells["ItemCode"].Value = ultraCombo2.SelectedRow.Cells["ItemID"].Value;
            //    ug.ActiveCell.Row.Cells["OnHand"].Value = ultraCombo2.SelectedRow.Cells["QOH"].Value;
            //    ug.ActiveCell.Row.Cells["ItemClass"].Value = ultraCombo2.SelectedRow.Cells["ItemClass"].Value;
            //    ug.ActiveCell.Row.Cells["GL"].Value = ultraCombo2.SelectedRow.Cells["SalesGLAccount"].Value;
            //    ug.ActiveCell.Row.Cells["CostPrice"].Value = ultraCombo2.SelectedRow.Cells["UnitCost"].Value;
            //    ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
            //    ug.ActiveCell.Row.Cells["UOM"].Value = ultraCombo2.SelectedRow.Cells["UOM"].Value;
            //    ug.ActiveCell.Row.Cells["Categoty"].Value = ultraCombo2.SelectedRow.Cells["Categoty"].Value;


            //    ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ultraCombo2.SelectedRow.Cells["PriceLevel1"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ultraCombo2.SelectedRow.Cells["PriceLevel2"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ultraCombo2.SelectedRow.Cells["PriceLevel3"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ultraCombo2.SelectedRow.Cells["PriceLevel4"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ultraCombo2.SelectedRow.Cells["PriceLevel5"].Value;

            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ultraCombo2.SelectedRow.Cells["UnitCost"].Value;

            //    ug.ActiveCell.Row.Cells["AVGC"].Value = ultraCombo2.SelectedRow.Cells["AVGQty"].Value;

            //    ug.ActiveCell.Row.Cells["LastPODate"].Value = Convert.ToDateTime(ultraCombo2.SelectedRow.Cells["LastPurDate"].Value.ToString()).ToShortDateString();
            //}
            //catch
            //{

            //}
                
            
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void ultraCombo1_RowSelected(object sender, RowSelectedEventArgs e)
        {
            //try
            //{
            //    ug.ActiveCell.Row.Cells["Description"].Value = ultraCombo1.SelectedRow.Cells["ItemDis"].Value;
            //    ug.ActiveCell.Row.Cells["ItemCode"].Value = ultraCombo1.SelectedRow.Cells["ItemID"].Value;
            //    ug.ActiveCell.Row.Cells["OnHand"].Value = ultraCombo1.SelectedRow.Cells["QOH"].Value;
            //    ug.ActiveCell.Row.Cells["ItemClass"].Value = ultraCombo1.SelectedRow.Cells["ItemClass"].Value;
            //    ug.ActiveCell.Row.Cells["GL"].Value = ultraCombo1.SelectedRow.Cells["SalesGLAccount"].Value;
            //    ug.ActiveCell.Row.Cells["CostPrice"].Value = ultraCombo1.SelectedRow.Cells["UnitCost"].Value;
            //    ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
            //    ug.ActiveCell.Row.Cells["UOM"].Value = ultraCombo1.SelectedRow.Cells["UOM"].Value;
            //    ug.ActiveCell.Row.Cells["Categoty"].Value = ultraCombo1.SelectedRow.Cells["Categoty"].Value;


            //    ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ultraCombo1.SelectedRow.Cells["PriceLevel1"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ultraCombo1.SelectedRow.Cells["PriceLevel2"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ultraCombo1.SelectedRow.Cells["PriceLevel3"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ultraCombo1.SelectedRow.Cells["PriceLevel4"].Value;
            //    ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ultraCombo1.SelectedRow.Cells["PriceLevel5"].Value;

            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ultraCombo1.SelectedRow.Cells["UnitCost"].Value;

            //    ug.ActiveCell.Row.Cells["AVGC"].Value = ultraCombo1.SelectedRow.Cells["AVGQty"].Value;

            //    ug.ActiveCell.Row.Cells["LastPODate"].Value = Convert.ToDateTime(ultraCombo1.SelectedRow.Cells["LastPurDate"].Value.ToString()).ToShortDateString();
            //}
            //catch
            //{

            //}

        }

        private void cmbVendorSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbVendorSelect.Enabled == true)
                {
                    cmbVendorSelect.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void cmbWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (cmbWarehouse.Enabled == true)
                {
                    cmbWarehouse.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void ultraCombo1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (ultraCombo1.Enabled == true)
                {
                    ultraCombo1.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void ultraCombo2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (ultraCombo2.Enabled == true)
                {
                    ultraCombo2.PerformAction(UltraComboAction.Dropdown, true, true);
                }
            }
        }

        private void txtDescription_Leave(object sender, EventArgs e)
        {
            ug.Focus();
            if(ug.Enabled ==true)
            {
                ug_Click(null, null);
            }
        }

        private void frmPurchaseOder_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnSave.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
                {
                    btnSave_Click(null, null);

                }
            }

            if (btnSearch.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
                {
                    btnSearch_Click(null, null);
                }
            }


            if (btnEditer.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E)
                {
                    btnEditer_Click(null, null);
                }
            }


            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                btnNew_Click(null, null);
            }


            if (btnProcess.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                {
                    toolStripButton1_Click_1(null, null);
                }
            }


            if (btnPrint.Enabled == true)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    btnPrint_Click(null, null);
                }
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                this.Close();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}