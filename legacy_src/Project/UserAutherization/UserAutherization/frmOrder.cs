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
    public partial class frmOrder : Form
    {
        public DSEstimate DsEst = new DSEstimate();
        clsCommon objclsCommon = new clsCommon();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();

        public string sMsg = "Peachtree - Direct Return";
        public string Tax1ID = "";
        public double Tax1Rate = 0.0;
        public string Tax1Name = "";
        public double Tax1Amount = 0.0;
        public string Tax1GLAccount = "";
        public string Tax2ID = "";
        public double Tax2Rate = 0.0;
        public string Tax2Name = "";
        public double Tax2Amount = 0.0;
        public string Tax2GLAccount = "";
        public string StrSql;
        public double dblLineTot;
        public double dblinedis ;
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
        public string StrPaymmetM;
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public string StrAP = null;
        public string StrRep = null;

        public DataSet dsAR;
        string StrSql1=null ;
        string StrSql2 = null;
        public DataSet dsWarehouse;
        public DataSet dsReturn;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public DSCustomerReturn DsCustomerReturn = new DSCustomerReturn();
        public DSetSalesOrder DsSalesOrder = new DSetSalesOrder();
        public DSPosSale DsPos = new DSPosSale();
        public DSItemRequest DsSalesItemRequest = new DSItemRequest();
        public static DateTime UserWiseDate = System.DateTime.Now;
        DataSet dsCustomer;
        DataSet dsSalesRep;
        DataSet ds;
        bool IsFind = false;

        public int  dblCusPriceLevel = 0;

        public frmOrder()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmOrder(int intNo)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = true;
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

                    EnableHeader(true);
                    EnableFoter(true);
                    txtCreditNo.Text = strNo;
                    ViewHeader(strNo);
                    ViewDetails(strNo);
                    ViewDetailsTempIDs(strNo);
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

                StrSql = "SELECT  TOP 1(SONum) FROM tblDefualtSetting ORDER BY SONum DESC";

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
                StrSql = "UPDATE tblDefualtSetting SET SONum='" + intCRNNo + "'";

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
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

                StrSql = "SELECT SOPref, SOPad, SONum FROM tblDefualtSetting";

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
        public void GetSONum()
        {
            try
            {
                Int32 intX;
                Int32 intZ;
                string StrInvNo;
                Int32 intP;
                Int32 intI;
                String StrInV;

                StrSql = "SELECT SOPref, SOPad, SONum FROM tblDefualtSetting";
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
                StrSql = " SELECT WhseId, WhseName,ArAccount,CashAccount,SalesGLAccount FROM tblWhseMaster order by IsDefault";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 75;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 125;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["ArAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["CashAccount"].Hidden = true;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["SalesGLAccount"].Hidden = true;

            }
            catch (Exception)
            {

                throw;
            }
        }



        //sanjeewa added following code segements to make modificatioj\n on filering options
        public void GetItemDataSetForDescription()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }
                StrSql = "SELECT tblItemWhse.ItemId,tblItemWhse.ItemDis," +
                " tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2," +
               "  tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5," +
                "tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM," +
               " tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM" +
               "  tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID " +
               "  WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Value.ToString() + "' UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster where ItemClass in(5)order by tblItemWhse.ItemDis";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt1 = new DataTable();
                dt1.Clear();
                da.Fill(dt1);

                if (dt1.Rows.Count > 0)
                {

                    ultraCombo3.DataSource = dt1;
                    ultraCombo3.ValueMember = "ItemDis";
                    ultraCombo3.DisplayMember = "ItemDis";

                    ultraCombo3.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[2].Width = 100;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[2].Hidden = false;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo3.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo3.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                }
                else
                {
                    ultraCombo3.DataSource = dt1;
                    ultraCombo3.ValueMember = "ItemDis";
                    ultraCombo3.DisplayMember = "ItemDis";
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



                StrSql="SELECT tblItemWhse.ItemId,tblItemWhse.ItemDis,"+
                " tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,"+
               "  tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,"+
                "tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,"+
               " tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM"+
               "  tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID "+
               "  WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Value.ToString() + "' UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster where ItemClass in(5)order by tblItemWhse.ItemId";
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

                    ultraCombo1.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Width = 100;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = false;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Hidden = true;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;

                    //ultraCombo3.DataSource = dt;
                    //ultraCombo3.ValueMember = "ItemDis";
                    //ultraCombo3.DisplayMember = "ItemDis";

                    //ultraCombo3.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[2].Width = 100;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[2].Hidden = false;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    //ultraCombo3.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    //ultraCombo3.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

                    //ultraCombo3.DataSource = dt;
                    //ultraCombo3.ValueMember = "ItemDis";
                    //ultraCombo3.DisplayMember = "ItemDis";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetItemDataSet1()
        {
            try
            {        
                StrSql = "SELECT tblItemWhse.ItemId,tblItemWhse.ItemDis," +
                " tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2," +
               "  tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5," +
                "tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM," +
               " tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM" +
               "  tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID " +
               "  WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text.ToString() + "' UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster where ItemClass in(5)";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemID";
                    ultraCombo2.DisplayMember = "ItemID";

                    ultraCombo2.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[2].Width = 100;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[2].Hidden = false;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo2.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo2.DisplayLayout.Bands[0].Columns[13].Hidden = true;

                    ultraCombo4.DataSource = dt;
                    ultraCombo4.ValueMember = "ItemDis";
                    ultraCombo4.DisplayMember = "ItemDis";

                    ultraCombo4.DisplayLayout.Bands[0].Columns[0].Width = 75;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[2].Width = 100;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[2].Hidden = false;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[3].Hidden = true;

                    ultraCombo4.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[5].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[6].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[7].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[8].Hidden = true;

                    ultraCombo4.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo4.DisplayLayout.Bands[0].Columns[13].Hidden = true;
                }
                else
                {
                    ultraCombo2.DataSource = dt;
                    ultraCombo2.ValueMember = "ItemID";
                    ultraCombo2.DisplayMember = "ItemID";

                    ultraCombo4.DataSource = dt;
                    ultraCombo4.ValueMember = "ItemDis";
                    ultraCombo4.DisplayMember = "ItemDis";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetCustomer()
        {
            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2,Pricing_Level FROM tblCustomerMaster order by CutomerID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Pricing_Level"].Hidden = true;
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
                //GrandTotal();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                foreach (UltraGridRow ugR in ugItem2.Rows.All)
                {
                    ugR.Delete(false);
                }
                //GrandTotal();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void DeleteEmpGrid()
        {
            try
            {
                ug.PerformAction(UltraGridAction.CommitRow);
                foreach (UltraGridRow ugR in ug.Rows.All)
                {
                    if (ugR.Cells["ItemCode"].Value.ToString().Trim().Length == 0) //&& (ugR.Cells["GL"].Value.ToString().Trim().Length == 0)
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
                    dblSubTot += double.Parse(ug.Rows[intGridRow].Cells["TotalPrice"].Value.ToString());
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


        private void REQItemUnitTotal()
        {
            try
            {                
                double dblreqittot = 0;
                double dblreqitLinetot = 0;
                int intGridRow;
                for (intGridRow = 0; intGridRow < ugItem2.Rows.Count; intGridRow++)
                {
                    dblreqittot += double.Parse(ugItem2.Rows[intGridRow].Cells[3].Value.ToString());
                    dblreqitLinetot += double.Parse(ugItem2.Rows[intGridRow].Cells[5].Value.ToString());
                }
                txtunittot.Value = dblreqittot;
                txtlinetotam.Value = dblreqitLinetot;
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
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            if (ug.ActiveCell.Column.Key == "ItemCode" || ug.ActiveCell.Column.Key == "Description" || ug.ActiveCell.Column.Key == "UnitPrice" || ug.ActiveCell.Column.Key == "OnHand" || ug.ActiveCell.Column.Key == "Quantity" || ug.ActiveCell.Column.Key == "Discount" || ug.ActiveCell.Column.Key == "TotalPrice" )                            
                                ug.PerformAction(UltraGridAction.PrevCell);
                                break;             
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            //ug.PerformAction(UltraGridAction.PrevCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            
                        }
                    case 38:
                        {
                            if (ug.DisplayLayout.ActiveRow.Index == 0)

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
                            if (ug.ActiveCell.Row.Index != 0)
                                //ug.PerformAction(UltraGridAction.ExitEditMode);
                                ug.PerformAction(UltraGridAction.BelowCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    //case 46:
                    //    {
                    //        if (ug.ActiveCell.Row.Index != 0)
                    //        //InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                    //        //FooterCalculation();
                    //        break;
                    //    }
                    case 9:
                        {
                            //if (ug.ActiveCell.Column.Key == "Quantity")
                            //{
                            //    if (ug.ActiveRow.HasNextSibling() == false)
                            //    {
                            //        if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                            //        {
                            //            UltraGridRow ugR;
                            //            ugR = ug.DisplayLayout.Bands[0].AddNew();
                            //            ugR.Cells["LineNo"].Value = ugR.Index + 1;
                            //            ugR.Cells["LineNo"].Selected = true;
                            //            ugR.Cells["LineNo"].Activated = true;
                            //        }
                            //    }
                            //}
                            
                                if (ug.ActiveCell.Column.Key == "TotalPrice")
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
                                {
                                    //if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                    //if (ug.ActiveCell.Row.Cells["GL"].Value.ToString().Trim() != string.Empty &&
                                    //    ug.ActiveCell.Row.Cells["Quantity"].Value.ToString().Trim() != string.Empty &&
                                    //    double.Parse(ug.ActiveCell.Row.Cells["Quantity"].Value.ToString().Trim()) > 0)
                                    //{
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
                    case 123:
                        {
                            if (ugItem2.Rows.Count == 0)
                            {
                                UltraGridRow ugItR;
                                ugItem2.Select();
                                ugItR = ugItem2.DisplayLayout.Bands[0].AddNew();
                                ugItR.Cells["LineNo"].Value = ugItR.Index + 1;
                                ugItR.Cells["Itemcode"].Activate();
                                ugItem2.ActiveCell = ugItR.Cells["Itemcode"];
                                ugItR.Cells["Itemcode"].Selected = true;
                                ugItR.Cells["Itemcode"].Activated = true;                                
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    if (ugR.Cells["ItemCode"].Value != null && ugR.Cells["ItemCode"].Text.Trim() != string.Empty)
                    {
                        if (IsGridExitCode(ugR.Cells["ItemCode"].Text,"Item") == false)
                        {
                            MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    //if (ugR.Cells["GL"].Value != null && ugR.Cells["GL"].Text.Trim() != string.Empty)
                    //{
                    //    if (IsGridExitCode(ugR.Cells["GL"].Text, "GL") == false)
                    //    {
                    //        MessageBox.Show("Invalid GL Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (IsGridExitCode(ug.ActiveCell.Row.Cells["GL"].Text, "GL") == false)
                    //    {
                    //        e.Cancel = true;
                    //    }
                    //}
                    if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        public void ViewDetailsTempIDs(string StrInvoiceNo)
        {
            try
            {
                StrSql = "SELECT [SalesOrderNo],[ItemID],[Description],[UOM],[GLAccount],[Quantity],[UnitPrice],[Amount] FROM [tblSOTemItem] WHERE [SalesOrderNo]='" + StrInvoiceNo + "' ORDER BY [DisNumber]";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ugItem2.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = "1";
                        ugR.Cells["ItemCode"].Value = Dr["ItemID"];
                        ugR.Cells["Description"].Value = Dr["Description"];
                        ugR.Cells["OnHand"].Value = Dr["Quantity"];
                        ugR.Cells["ItemClass"].Value = "1";
                        ugR.Cells["GL"].Value = Dr["GLAccount"];
                        ugR.Cells["CostPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["Quantity"].Value = Dr["Quantity"];
                        ugR.Cells["UOM"].Value = Dr["UOM"];
                        ugR.Cells["Categoty"].Value = "";
                        ugR.Cells["PriceLevel1"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel2"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel3"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel4"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel5"].Value = Dr["UnitPrice"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["TotalPrice"].Value = Dr["Amount"];

                    }
                }
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
                StrSql = "SELECT [Quantity],[ItemID],[Description],[GLAccount],[UnitPrice],[Amount],[DispatchQty]" +
                       " ,[RemainQty],[IsfullDispatch],[IsSoClosed],[UOM],[JobID],[UserID],[DisNumber],[Linedisper] FROM [tblSalesOrderTemp] [tblSalesOrderTemp] WHERE [SalesOrderNo]='" + StrInvoiceNo + "'ORDER BY [DisNumber]";

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
                        ugR.Cells["LineNo"].Value = Dr["DisNumber"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemID"];
                        ugR.Cells["Description"].Value = Dr["Description"];
                        ugR.Cells["OnHand"].Value = Dr["Quantity"];
                        ugR.Cells["ItemClass"].Value = "1";
                        ugR.Cells["GL"].Value = Dr["GLAccount"];
                        ugR.Cells["CostPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["Quantity"].Value = Dr["Quantity"];
                        ugR.Cells["UOM"].Value = Dr["UOM"];
                        ugR.Cells["Categoty"].Value = "";
                        ugR.Cells["PriceLevel1"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel2"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel3"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel4"].Value = Dr["UnitPrice"];
                        ugR.Cells["PriceLevel5"].Value = Dr["UnitPrice"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["TotalPrice"].Value = Dr["Amount"];
                        ugR.Cells["Discount"].Value = Dr["Linedisper"];






                        //ugR = ug.DisplayLayout.Bands[0].AddNew();
                        //ugR.Cells["LineNo"].Value = Dr["ItemLine"];
                        //ugR.Cells["ItemCode"].Value = Dr["ItemId"];
                        //ugR.Cells["Description"].Value = Dr["ItemDis"];
                        //ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        //ugR.Cells["Quantity"].Value = Dr["Qty"];
                        //ugR.Cells["TotalPrice"].Value = Dr["LineTotal"];
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
                StrSql = "SELECT [SalesOrderNo],[CustomerID],[Date],[WarehouseID],[NBTPer],[NBTAmount],[VATPer]," +
                 " [VATAmount],[DisPer],[DisAmount],[SubAmount],[GrossAmount],[TotalAmount],[SalesRep],[CustomerPO],[Remarks],[CusName] FROM [tblSalesOrderTemp] WHERE [SalesOrderNo]='" + StrInvoiceNo + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbCustomer.Text = dt.Rows[0]["CustomerID"].ToString().Trim();
                    txtCreditNo.Text = dt.Rows[0]["SalesOrderNo"].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0]["WarehouseID"].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["Date"].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0]["TotalAmount"].ToString().Trim());
                    txtCustomerPO.Text = dt.Rows[0]["CustomerPO"].ToString().Trim();
                    cmbSalesRep.Text = dt.Rows[0]["SalesRep"].ToString().Trim();

                    txtNBT.Value = dt.Rows[0]["NBTAmount"].ToString().Trim();
                    txtNBTPer.Value = dt.Rows[0]["NBTPer"].ToString().Trim();

                    txtVatPer.Value = dt.Rows[0]["VATPer"].ToString().Trim();
                    txtVat.Value = dt.Rows[0]["VATAmount"].ToString().Trim();

                    txtDiscPer.Value = dt.Rows[0]["DisPer"].ToString().Trim();
                    txtDiscAmount.Value = dt.Rows[0]["DisAmount"].ToString().Trim();

                    txtGrossValue.Value = dt.Rows[0]["GrossAmount"].ToString().Trim();
                    txtSubValue.Value = dt.Rows[0]["SubAmount"].ToString().Trim();
                    txtNetValue.Value = dt.Rows[0]["TotalAmount"].ToString().Trim();
                    txtDescription.Text = dt.Rows[0]["Remarks"].ToString().Trim();
                    txtCustomer.Text = dt.Rows[0]["CusName"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmWareHouseTrans_Load(object sender, EventArgs e)
        {
            try
            {
                //----------------user----------
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
                    //---------------------------------
                    GetCurrentUserDate();

                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                    btnSearch.Enabled = true;
                    btnReset.Enabled = true;
                    btnNew.Enabled = true;
                    //btnEdit.Enabled = false;
                    dtpDate.Enabled = false;
                    ClearHeader();                    
                    GetWareHouseDataSet();
                    GetCustomer();
                    GetSalesRep();
                    loadChartofAcount();
                    LoadDefualtAccount();
                    LoadDefualtWH();
                    LoadDefualSalesRep();
                    GetItemDataSet();
                    GetItemDataSetForDescription();
                    GetItemDataSet1();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);                   
                    GetSONum();
                    loadDefaltOption();
                    GetTaxDeails();
                    ValidateInvoiceSetting();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ValidateInvoiceSetting()
        {

            String S1 = "Select Locked from tblTax_Default where flg ='PRL' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(S1);
            SqlDataAdapter da = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Boolean Isok = false;
            if (dt.Rows.Count > 0)
            {
                Isok = bool.Parse(dt.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.AllowEdit;
                }
            }
            String S2 = "Select Locked from tblTax_Default where flg ='QTY' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd1 = new SqlCommand(S2);
            SqlDataAdapter da1 = new SqlDataAdapter(S2, ConnectionString);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                Isok = bool.Parse(dt1.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["Quantity"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["Quantity"].CellActivation = Activation.AllowEdit;
                }
            }
            String S3 = "Select Locked from tblTax_Default where flg ='DST' and UserName='" + user.userName.ToString().Trim() + "'";
            SqlCommand cmd2 = new SqlCommand(S3);
            SqlDataAdapter da2 = new SqlDataAdapter(S3, ConnectionString);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            if (dt2.Rows.Count > 0)
            {
                Isok = bool.Parse(dt2.Rows[0].ItemArray[0].ToString());
                if (Isok == true)
                {
                    ug.DisplayLayout.Bands[0].Columns["Discount"].CellActivation = Activation.NoEdit;
                }
                else
                {
                    ug.DisplayLayout.Bands[0].Columns["Discount"].CellActivation = Activation.AllowEdit;
                }
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
                    UserWiseDate = user.LoginDate;// Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    dtpDate.Value = UserWiseDate;
                }
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
                    StrARAccount = dt2.Rows[0].ItemArray[3].ToString();
                    StrCashAccount = dt2.Rows[0].ItemArray[4].ToString();
                    StrSalesGLAccount = dt2.Rows[0].ItemArray[5].ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (add)
            {
                btnSave.Enabled = true;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
                // btnSNO.Enabled = false;
                btnSearch.Enabled = false;
                btnReset.Enabled = true;
                // btnEdit.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);

                ClearHeader();
                DeleteRows();
                GetSONum();
                GetCustomer(); 
                GetWareHouseDataSet();
                GetCustomer();
                GetSalesRep();
                loadChartofAcount();
                LoadDefualtAccount();
                LoadDefualtWH();
                LoadDefualSalesRep();
                GetItemDataSet();
                GetItemDataSetForDescription();
                GetItemDataSet1();
                cmbCustomer.Focus();
                loadDefaltOption();
                GetGLAccountDataset();
                GetTaxDeails();
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void GetGLAccountDataset()
        {
            try
            {
                if (cmbWarehouse.Text == "")
                {
                    return;
                }

                StrSql = "SELECT AcountID,AccountDescription from tblChartofAcounts ";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo5.DataSource = dt;
                    ultraCombo5.ValueMember = "AcountID";
                    ultraCombo5.DisplayMember = "AcountID";

                    ultraCombo5.DisplayLayout.Bands[0].Columns[0].Width = 120;
                    ultraCombo5.DisplayLayout.Bands[0].Columns[1].Width = 200;                    
                }
                else
                {
                    ultraCombo5.DataSource = dt;
                    ultraCombo5.ValueMember = "AcountID";
                    ultraCombo5.DisplayMember = "AcountID";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void loadDefaltOption()
        {
            try
            {

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='TAX' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(StrSql1);
                SqlDataAdapter da1 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        cmbInvoiceType.Value = dt1.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt1.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbInvoiceType.Enabled = false;
                        }
                    }
                }

                StrSql1 = "Select Tid,TAXID,locked from tblTax_Default where Flg='REP' and UserName='" + user.userName.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(StrSql1);
                SqlDataAdapter da2 = new SqlDataAdapter(StrSql1, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    cmbSalesRep.Enabled = true;
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        cmbSalesRep.Value = dt2.Rows[i]["Tid"].ToString();
                        if (bool.Parse(dt2.Rows[i]["locked"].ToString()) == true)
                        {
                            cmbSalesRep.Enabled = false;
                        }
                    }
                }
                            

            }
            catch (Exception ex)
            {
                throw ex;
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

        private void SaveEvent()
        {

            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cmbCustomer.Value == null)
            {
                MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int intGrid;
            int intAutoIndex;
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
                if (HeaderValidation() == false)
                {
                    return;
                }
               

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                GetSONum();
                StrReference = GetInvNoField(myConnection, myTrans);
                UpdatePrefixNo(myConnection, myTrans);
                int _LineCount = CalculateLines();
                //chamila
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(_LineCount, Int16.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), StrPaymmetM,
                        ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                        ug.Rows[intGrid].Cells["Description"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(),
                        double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                        ug.Rows[intGrid].Cells["GL"].Value.ToString(), int.Parse(cmbInvoiceType.Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["Discount"].Value.ToString()), myConnection, myTrans);

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), myConnection, myTrans);
                    }
                }

                if (ugItem2.Rows.Count > 0)
                {
                    for (intGrid = 0; intGrid < ugItem2.Rows.Count-1; intGrid++)
                    {
                        SaveExternalItem(ugItem2.Rows[intGrid].Cells["ItemCode"].Value.ToString(), double.Parse(ugItem2.Rows[intGrid].Cells["Quantity"].Value.ToString()),
                           ugItem2.Rows[intGrid].Cells["Description"].Value.ToString(), ugItem2.Rows[intGrid].Cells["UOM"].Value.ToString(),
                           double.Parse(ugItem2.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ugItem2.Rows[intGrid].Cells["TotalPrice"].Value.ToString()),
                           ugItem2.Rows[intGrid].Cells["GL"].Value.ToString(), Int16.Parse(ugItem2.Rows[intGrid].Cells["LineNo"].Value.ToString()), myConnection, myTrans);
                    }
                }
                else
                {
                    DialogResult reply1 = MessageBox.Show("You have not selected request items for this transaction, Are You Sure ", "Information", MessageBoxButtons.OKCancel);
                    if (reply1 == DialogResult.OK)
                    {

                    }
                    if (reply1 == DialogResult.Cancel)
                    {
                        myTrans.Rollback();
                        return;
                    }
                }
                //CreateSOXML(myTrans, myConnection);
                myTrans.Commit();
                MessageBox.Show("Sales Order Successfuly Saved.", "Information", MessageBoxButtons.OK);
                groupBox1.Visible = true;
                button1.Focus();
                buttonlock();
                
               // ButtonClear();
                //Print(StrReference);
                
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
            }
        }

        public void CreateSOXML(SqlTransaction tr, SqlConnection con)
        {

            try
            {
                DateTime DTP = Convert.ToDateTime(dtpDate.Text);
                string SalesOrderDate = DTP.ToString("MM/dd/yyyy");

                XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesOrderJournal.xml", System.Text.Encoding.UTF8);
                Writer.Formatting = Formatting.Indented;
                Writer.WriteStartElement("PAW_SalesOrders");
                Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
                Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
                Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

                int DistributionNumber = 0;
                int rowCount1 = CalculateLines();
                string NoDistributions = Convert.ToString(rowCount1);


                Writer.WriteStartElement("PAW_SalesOrder");
                Writer.WriteAttributeString("xsi:type", "paw:sales_order");

                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Vendor ID should be here = Ptient No
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Order_Number");
                Writer.WriteString(txtCreditNo.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Date");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(SalesOrderDate);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Ship_By");
                Writer.WriteAttributeString("xsi:type", "paw:date");
                Writer.WriteString(SalesOrderDate);
                Writer.WriteEndElement();

                //Ship_By

                Writer.WriteStartElement("Closed");
                Writer.WriteString("FALSE");
                Writer.WriteEndElement();


                Writer.WriteStartElement("Customer_PO");
                Writer.WriteString(txtCustomerPO.Text.ToString().Trim());
                Writer.WriteEndElement();

                Writer.WriteStartElement("Sales_Representative_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                if (user.MergeAccUser)
                {
                    Writer.WriteString(user.userName.ToString().Trim());
                }
                else
                {
                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                }
                Writer.WriteEndElement();

                Writer.WriteStartElement("Accounts_Receivable_Account");
                Writer.WriteString(StrARAccount);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions");
                Writer.WriteString((NoDistributions).ToString());
                Writer.WriteEndElement();

                for (int intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    if (ug.Rows[intGrid].Cells["Quantity"].Value != null && ug.Rows[intGrid].Cells["Quantity"].Value.ToString().Trim().Length > 0)
                    {
                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > 0)
                        {
                            Writer.WriteStartElement("SOLines");
                            Writer.WriteStartElement("SOLine");

                            Writer.WriteStartElement("DistributionNumber");
                            Writer.WriteString(DistributionNumber.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Quantity");
                            Writer.WriteString(ug.Rows[intGrid].Cells["Quantity"].Value.ToString());//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Item_ID");
                            Writer.WriteString(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("SO_Description");
                            Writer.WriteString(ug.Rows[intGrid].Cells["Description"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("GL_Account");
                            Writer.WriteAttributeString("xsi:type", "paw:id");
                            Writer.WriteString(StrSalesGLAccount); //(ug.Rows[intGrid].Cells["GL"].Value.ToString());//
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Tax_Type");
                            Writer.WriteString("1");//Doctor Charge
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("Unit_Price");
                            Writer.WriteString("-" + ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString());
                            Writer.WriteEndElement();

                            //Writer.WriteStartElement("UM_ID");
                            //if (ug.Rows[intGrid].Cells["UOM"].Value.ToString() == string.Empty)
                            //{
                            //    Writer.WriteString("<Each>");
                            //}
                            //else
                            //{
                            //    Writer.WriteString(ug.Rows[intGrid].Cells["UOM"].Value.ToString());
                            //}
                            //Writer.WriteEndElement();

                            Writer.WriteStartElement("Amount");
                            Writer.WriteString("-" +ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteEndElement();
                            Writer.WriteEndElement();

                        }
                    }
                }
                //********************
                Writer.WriteEndElement();//last line
                Writer.Close();

                Connector ObjImportP = new Connector();
                ObjImportP.ImportSalesOrder();

            }
            catch (Exception ex)
            {
                throw ex;

            }
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
                StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //SaveExternalItem

        private void SaveExternalItem(string StrItemCode, double dblQuantity, string StrItemDescription, string StrUOM, double dblPrice, double dblLineNetAmt, string StrGLAccount, Int32 LineNo, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tblSOTemItem]([SalesOrderNo],[ItemID],[Description], " +
                    " [UOM],[GLAccount],[Quantity],[UnitPrice],[Amount],DisNumber) VALUES ('" + txtCreditNo.Text.Trim() + "','" + StrItemCode + "','" + StrItemDescription + "','" + StrUOM + "','" + StrGLAccount + "','" + dblQuantity + "','" + dblPrice + "','" + dblLineNetAmt + "','" + LineNo + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDetails(int intLineCount, int intLineNo, string StrPayMethod, string StrItemCode, double dblQuantity, string StrItemDescription, string StrUOM, double dblPrice, double dblLineNetAmt, string StrGLAccount,int IsInclusive,double LineDisPer  , SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                string Cuser = "";
                if (user.MergeAccUser)
                {
                    Cuser = user.userName.ToString().Trim();
                }
                else
                {
                    Cuser = cmbSalesRep.Value.ToString().Trim();
                }

                //if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                //{
                //    StrItemCode = "";
                //    StrItemDescription = "";
                //}                    

                StrSql = "INSERT INTO tblSalesOrderTemp([SalesOrderLink],[SalesOrderNo],[CustomerID],[Date],[ARAccount]," +
                " [NoOfDis],[DisNumber],[Quantity],[ItemID],[Description],[GLAccount],[UnitPrice]" +
                " ,[Amount],[DispatchQty],[RemainQty],[IsfullDispatch],[IsSoClosed],[CustomerPO],[UOM]" +
                " ,[JobID],[WarehouseID],[NBTPer],[NBTAmount],[VATPer],[VATAmount],[DisPer],[DisAmount],[SubAmount],[GrossAmount],[TotalAmount],[SalesRep],UserID,Remarks,IsInclusive,Linedisper,CusName) VALUES ('1','" + txtCreditNo.Text.Trim() + "','" + cmbCustomer.Value.ToString() + "','" + GetDateTime(dtpDate.Value) + "'," +
                " '" + cmbARAccount.Text.Trim() + "','" + intLineCount + "','" + intLineNo + "','" + dblQuantity + "','" + StrItemCode + "'," +
                " '" + StrItemDescription + "','" + StrGLAccount + "','" + dblPrice + "','" + dblLineNetAmt + "','0','0'," +
                " 'False','False','" + txtCustomerPO.Text.ToString().Trim() + "','" + StrUOM + "','','" + cmbWarehouse.Text.ToString() + "','" + txtNBTPer.Value.ToString().Trim() + "'," +
                " '" + txtNBT.Value.ToString() + "','" + txtVatPer.Value.ToString() + "','" + txtVat.Value.ToString() + "','" + txtDiscPer.Value.ToString() + "'," +
                " '" + txtDiscAmount.Value.ToString() + "','" + txtSubValue.Value.ToString() + "','" + txtGrossValue.Value.ToString() + "','" + txtNetValue.Value.ToString() + "','" + Cuser.ToString() + "','" + user.userName.ToString().Trim() + "','" + txtDescription.Text.ToString().Trim() + "','" + IsInclusive.ToString() + "','" + LineDisPer.ToString() + "','" + txtCustomer.Text.ToString().Trim()  + "')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CalculateTaxAmounts()
        {
            double _subTot = 0;
            double _Tax1 = 0;
            double _Tax2 = 0;

            Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
            Tax2Rate = double.Parse(txtVatPer.Text.Trim());

            if (txtDiscAmount.Text.Trim() == string.Empty || txtDiscAmount.Text == null) txtDiscAmount.Text = "0.00";

            _subTot = double.Parse(txtSubValue.Text.Trim());

            txtDiscAmount.Text = ((_subTot * double.Parse(txtDiscPer.Text.Trim())) / 100).ToString("0.00");

            _subTot = _subTot - double.Parse(txtDiscAmount.Text.Trim());
            //_subTot = _subTot - double.Parse(txtValueDiscount.Text.Trim()) - double.Parse(txtDiscLineTot.Text.Trim());

            _Tax1 = _subTot * Tax1Rate / 100;
            _Tax2 = (_Tax1 + _subTot) * Tax2Rate / 100;

            txtNBT.Text = _Tax1.ToString("0.00");
            txtVat.Text = _Tax2.ToString("0.00");

            txtNetValue.Text = (_subTot + _Tax1 + _Tax2).ToString("0.00");
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveEvent();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice]) " +
                    " VALUES('15','" + strCRNNo + "','" + GetDateTime(dtDate) + "','SalesOrder','False','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + SellingPrice + "')";

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
                btnPrint.Enabled = true;
                frmOrderSearch ObjOrderSearch = new frmOrderSearch();
                ObjOrderSearch.ShowDialog();
                setValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void PrintPOS(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print Job Order?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    SalesRequestPrint(txtCreditNo.Text);
                }
                else
                {
                    DsSalesOrder.Clear();
                    DsPos.Clear();
                    if (strCRNNo != "")
                    {
                        String S1 = "Select * from View_salesOrder_customer WHERE SalesOrderNo = '" + txtCreditNo.Text.Trim() + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlConnection con1 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                        da1.Fill(DsPos.DtSalesOrder);
                        DirectPrintPOS();
                        SalesRequestPrint(txtCreditNo.Text);
                    }
                }
                
            }
            catch (Exception ex) { throw ex; }
        }
        private void Print(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print Job Order?", "Print", MessageBoxButtons.OKCancel);

                //if (reply == DialogResult.Cancel)
                //{
                //    SalesRequestPrint(txtCreditNo.Text);
                //}
                if (reply == DialogResult.OK)
                {
                    DsSalesOrder.Clear();
                    if (strCRNNo != "")
                    {
                        String S1 = "Select * from View_salesOrder_customer WHERE SalesOrderNo = '" + txtCreditNo.Text.Trim() + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlConnection con1 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                        da1.Fill(DsSalesOrder.DserSOrder);
                        DirectPrint();
                      //  SalesRequestPrint(txtCreditNo.Text);
                    }
                }
                
            }
            catch (Exception ex) { throw ex; }
        }
        private void PrintPos(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print Job Order?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    SalesRequestPrint(txtCreditNo.Text);
                }
                else
                {
                    DsSalesOrder.Clear();
                    if (strCRNNo != "")
                    {
                        String S1 = "Select * from View_salesOrder_customer WHERE SalesOrderNo = '" + txtCreditNo.Text.Trim() + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlConnection con1 = new SqlConnection(ConnectionString);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                        da1.Fill(DsSalesOrder.DserSOrder);
                        DirectPrintSO();
                        //SalesRequestPrint(txtCreditNo.Text);
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }
        private void SalesRequestPrint(string strCRNNo)
        {
            try
            {
                ds = new DataSet();
                DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print Request Note?", "Print", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                DsSalesItemRequest.Clear();
                DsSalesOrder.Clear();
                if (strCRNNo != "")
                {
                    String S1 = "Select * from View_sales_sub_Order WHERE SalesOrderNo = '" + txtCreditNo.Text.Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlConnection con1 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
                    da1.Fill(DsSalesItemRequest.DtITRequest);
                    //DirectPrint1();//Comment by sanjeewa on 01/09/2013  to make it direct print
                    DirectPrintSO();//Add this mathod by sanjeewa to activate direct print
                    ButtonClear();
                }
            }
            catch (Exception ex) { throw ex; }
        }
        private void DirectPrintSO()
        {
            try
            {
                ReportDocument crp = new ReportDocument();
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptItemsRequest.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(DsSalesItemRequest);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void DirectPrintPOS()
        {
            try
            {
                ReportDocument crp = new ReportDocument();
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CrtPosSalesOrder.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(DsPos);
                crp.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void DirectPrint1()
        {
            try
            {
                frmPrintSalesItemRequest printax = new frmPrintSalesItemRequest(this);
                printax.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void DirectPrint()
        {
            try
            {
                frmsalesorderprint printax = new frmsalesorderprint(this);
                printax.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
               // groupBox1.Visible = true;
                button1.Focus();
                groupBox1.Visible = true;
                //Print(txtCreditNo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            cmbWarehouse.Text = "";
            txtDescription.Text = "";
            dtpDate.Value = DateTime.Now;
            txtWarehouseName.Text = "";
            cmbCustomer.Text = "";
            txtCustomer.Text = "";
            cmbSalesRep.Text = "";
            txtSalesRep.Text = "";
            txtAddress1.Text = "";
            txtCustomerPO.Text = "";
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

        private void EnableFoter(Boolean blnEnable)
        {
            txtVatPer.Enabled = blnEnable;
            txtNBTPer.Enabled = blnEnable;
            txtDescription.Enabled = blnEnable;
            txtDiscPer.Enabled = blnEnable;
            ug.Enabled = blnEnable;
        }

        private void EnableHeader(Boolean blnEnable)
        {
            cmbWarehouse.Enabled = blnEnable;
            dtpDate.Enabled = blnEnable;
            txtWarehouseName.Enabled = false;

            cmbCustomer.Enabled = blnEnable;
            txtCustomer.Enabled = true ;

            cmbSalesRep.Enabled = blnEnable;
            txtSalesRep.Enabled = false;

            cmbARAccount.Enabled = blnEnable;
            optCash.Enabled = blnEnable;
            optCredit.Enabled = blnEnable;
        }

        private void ButtonClear()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            //btnEdit.Enabled = false;

            ClearHeader();
            EnableHeader(false);
            EnableFoter(false);
            DeleteRows();
            GetSONum();
            ug.Enabled = false;
            intEstomateProcode = 0;
        }
        private void buttonlock()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            btnSearch.Enabled = true;
            btnReset.Enabled = true;
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
            try
            {
                e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private double ChangePrice2(double dblPriceList2)
        {
            return dblPriceList2;
        }
        private void GetTaxDeails()
        {
            try
            {
                String S1 = "Select * from tblTaxApplicable order by Rank";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtNBTPer.Text = double.Parse(dt.Rows[0]["Rate"].ToString()).ToString("0.00");
                    txtVatPer.Text = double.Parse(dt.Rows[1]["Rate"].ToString()).ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private double Discalculation(double UnitCost, double Quantity, double _Discount)
        {
            try
            {
                dblinedis = 0;
                double LineDisAmount = 0;
                dblinedis = (UnitCost * Quantity) * (_Discount / 100);
                LineDisAmount = dblinedis;
                return dblinedis;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                //sss
                if (e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity" || e.Cell.Column.Key == "Discount")
                {
                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                   // e.Cell.Row.Cells["LineDisc"].Value = Discalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value), Convert.ToDouble(e.Cell.Row.Cells["Discount"].Value));
                    //GrandTotal();

                    if (e.Cell.Column.Key != "LineNo")
                        InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                        FooterCalculation();   

                    //CalculateTaxAmounts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
        private void ug_AfterCellUpdate_ooo(object sender, CellEventArgs e)
        {
            try
            {
                if (ug.ActiveCell == null)
                    return;

                if (e.Cell.Column.Key == "ItemCode")
                {
                    StrSql = " SELECT   tblItemWhse.ItemID,  tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemWhse.UOM, tblItemMaster.Categoty, " +
                                " tblItemMaster.PriceLevel1, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, tblItemMaster.PriceLevel6, tblItemMaster.PriceLevel5, " +
                                " tblItemMaster.PriceLevel7, tblItemMaster.PriceLevel8, tblItemMaster.PriceLevel9, tblItemMaster.PriceLevel10, isnull(tblItemWhse.UnitCost,0) as UnitCost " +
                                " FROM         tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where  tblItemWhse.ItemID='" + ug.Rows[e.Cell.Row.Index].Cells["ItemCode"].Value.ToString() + "' and tblItemWhse.WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";

                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ug.ActiveCell.Value.ToString() == dr["ItemId"].ToString())
                        {
                            ug.ActiveCell.Row.Cells["Description"].Value = dr["ItemDis"].ToString();
                            ug.ActiveCell.Row.Cells["OnHand"].Value = dr["QTY"].ToString();
                            ug.ActiveCell.Row.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                            ug.ActiveCell.Row.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                            ug.ActiveCell.Row.Cells["Quantity"].Value = "1.00";
                            ug.ActiveCell.Row.Cells["UOM"].Value = dr["UOM"].ToString();
                            ug.ActiveCell.Row.Cells["Categoty"].Value = dr["Categoty"].ToString();
                            ug.ActiveCell.Row.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                            ug.ActiveCell.Row.Cells["UnitPrice(Incl)"].Value = dr["PriceLevel1"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel1"].Value = dr["PriceLevel1"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                            ug.ActiveCell.Row.Cells["TotalPrice"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["TotalPrice(Incl)"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineTax"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = "0.00";

                            if (dblCusPriceLevel == 0)
                            {
                                // dblCusPriceLevel = 1;
                                string PriceLevel = "PriceLevel" + "1";
                                // string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            }
                            else
                            {
                                string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            }

                            //if (dblCusPriceLevel == 0)
                            //{
                            //    dblCusPriceLevel = 1;
                            //    string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            //}
                            //else
                            //{
                            //    string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            //}

                        }
                    }
                }

                if (e.Cell.Column.Key == "Description")
                {
                    StrSql = " SELECT   tblItemWhse.ItemID,  tblItemWhse.ItemDis, tblItemWhse.QTY, tblItemMaster.ItemClass, tblItemMaster.SalesGLAccount, tblItemWhse.UOM, tblItemMaster.Categoty, " +
                                " tblItemMaster.PriceLevel1, tblItemMaster.PriceLevel2, tblItemMaster.PriceLevel3, tblItemMaster.PriceLevel4, tblItemMaster.PriceLevel6, tblItemMaster.PriceLevel5, " +
                                " tblItemMaster.PriceLevel7, tblItemMaster.PriceLevel8, tblItemMaster.PriceLevel9, tblItemMaster.PriceLevel10, isnull(tblItemWhse.UnitCost,0) as UnitCost " +
                                " FROM         tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID where  tblItemWhse.ItemDis='" + ug.Rows[e.Cell.Row.Index].Cells["Description"].Value.ToString() + "' and tblItemWhse.WhseId='" + cmbWarehouse.Text.ToString().Trim() + "'";

                    SqlCommand cmd = new SqlCommand(StrSql);
                    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (ug.ActiveCell.Value.ToString() == dr["ItemDis"].ToString())
                        {
                            ug.ActiveCell.Row.Cells["ItemCode"].Value = dr["ItemID"].ToString();
                            ug.ActiveCell.Row.Cells["OnHand"].Value = dr["QTY"].ToString();
                            ug.ActiveCell.Row.Cells["ItemClass"].Value = dr["ItemClass"].ToString();
                            ug.ActiveCell.Row.Cells["GL"].Value = dr["SalesGLAccount"].ToString();
                            ug.ActiveCell.Row.Cells["Quantity"].Value = "1.00";
                            ug.ActiveCell.Row.Cells["UOM"].Value = dr["UOM"].ToString();
                            ug.ActiveCell.Row.Cells["Categoty"].Value = dr["Categoty"].ToString();
                            ug.ActiveCell.Row.Cells["CostPrice"].Value = dr["UnitCost"].ToString();
                            ug.ActiveCell.Row.Cells["UnitPrice(Incl)"].Value = dr["PriceLevel1"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel1"].Value = dr["PriceLevel1"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel2"].Value = dr["PriceLevel2"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel3"].Value = dr["PriceLevel3"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel4"].Value = dr["PriceLevel4"].ToString();
                            ug.ActiveCell.Row.Cells["PriceLevel5"].Value = dr["PriceLevel5"].ToString();
                            ug.ActiveCell.Row.Cells["TotalPrice"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["TotalPrice(Incl)"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["Discount"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineTax"].Value = "0.00";
                            ug.ActiveCell.Row.Cells["LineDisc"].Value = "0.00";
                            //if (dblCusPriceLevel == 0)
                            //{
                            //    dblCusPriceLevel = 1;
                            //    string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            //}
                            //else
                            //{
                            //    string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                            //    ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            //}
                            if (dblCusPriceLevel == 0)
                            {
                                // dblCusPriceLevel = 1;
                                string PriceLevel = "PriceLevel" + "1";
                                // string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            }
                            else
                            {
                                string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = dr[PriceLevel].ToString();
                            }
                        }
                    }
                }

                if (e.Cell.Column.Key != "LineNo")
                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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
                MessageBox.Show(ex.Message);
            }
        }

        

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //GrandTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
                //GrandTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        public Boolean IsGridExitCode(String StrCode, string _Type)
        {
            try
            {
                if (_Type == "Item")
                {
                    foreach (UltraGridRow ugR in ultraCombo1.Rows)
                    {
                        if (ugR.Cells["ItemID"].Text == StrCode)
                        {
                            return true;
                        }
                    }
                }
              
                if (_Type == "Desc")
                {
                    foreach (UltraGridRow ugR in ultraCombo3.Rows)
                    {
                        if (ugR.Cells["Description"].Text == StrCode)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (UltraGridRow ugR in ultraCombo5.Rows)
                    {
                        if (ugR.Cells[0].Text == StrCode)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                GetItemDataSetForDescription();
                GetItemDataSet1();
                GetTaxDeails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (ug.ActiveCell.Column.Key == "ItemCode" || ug.ActiveCell.Column.Key == "GL")
                {
                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ug.ActiveCell.Value = ug.ActiveCell.Text;
                        if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != null 
                            && ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim()!=string.Empty)
                        {
                            if (IsGridExitCode(ug.ActiveCell.Row.Cells["ItemCode"].Text, "Item") == false)
                            {
                                e.Cancel = true;
                            }
                        }
                        else
                        {
                            if (IsGridExitCode(ug.ActiveCell.Row.Cells["GL"].Text, "GL") == false)
                            {
                                e.Cancel = true;
                            }
                        }
                        //if (IsGridExitCode(ug.ActiveCell.Row.Cells["ItemCode"].Text,"Item") == false)
                        //{
                        //    e.Cancel = true;
                        //}
                        //test
                        foreach (UltraGridRow ugR in ultraCombo1.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
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
                                
                                if(dblCusPriceLevel==0)
                                {
                                    dblCusPriceLevel = 1;
                                }
                                string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                               // ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel5"].Value;
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;
                               
                                //ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitPrice"].Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void optSerialTwo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetSONum();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void optSerialOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GetSONum();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                        StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();
                        cmbARAccount.Text = StrARAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }

        public void GetSalesRep()
        {
            dsSalesRep = new DataSet();
            try
            {
                dsSalesRep.Clear();
                StrSql = " SELECT RepCode, RepName FROM tblSalesRep order by RepCode";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsSalesRep, "DtSalesRep");
                cmbSalesRep.DataSource = dsSalesRep.Tables["DtSalesRep"];
                cmbSalesRep.DisplayMember = "RepCode";
                cmbSalesRep.ValueMember = "RepCode";
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnclose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {
            try
            {
               //GrandTotal();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        txtCustomer.Text = cmbCustomer.ActiveRow.Cells[1].Value.ToString();
                        txtAddress1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString() + ", " + cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                        dblCusPriceLevel = int.Parse(cmbCustomer.ActiveRow.Cells[4].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void GetDefaultAPAccount()
        {
            try
            {
                StrSql = "SELECT [ARAccount] FROM tblDefualtSetting";
                SqlCommand command = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrAP = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbARAccount.Text = StrAP.Trim();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void GetARAccount()
        //{
        //    dsAR = new DataSet();
        //    try
        //    {
        //        dsAR.Clear();
        //        StrSql = " SELECT AcountID, AccountDescription FROM tblChartofAcounts order by AcountID";
        //        SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
        //        dAdapt.Fill(dsAR, "DtAR");
        //        cmbARAccount.DataSource = dsAR.Tables["DtAR"];
        //        cmbARAccount.DisplayMember = "AcountID";
        //        cmbARAccount.ValueMember = "AcountID";
        //        cmbARAccount.DisplayLayout.Bands["DtAR"].Columns["AcountID"].Width = 100;
        //        cmbARAccount.DisplayLayout.Bands["DtAR"].Columns["AccountDescription"].Width = 150;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public void LoadDefualtWH()
        {
            try
            {
                String S = "Select WhseId from tblWhseMaster where IsDefault='1'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbWarehouse.Text = dt.Tables[0].Rows[i]["WhseId"].ToString().Trim();
                }
            }
            catch (Exception ex) { throw ex; }
        }


        public void LoadDefualSalesRep()
        {
            try
            {
                String S = "Select RepCode from tblSalesRep";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbSalesRep.Text = dt.Tables[0].Rows[i]["RepCode"].ToString().Trim();
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public void LoadDefualtAccount()
        {
            try
            {
                String S = "Select CusretnDrAc from tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbARAccount.Text = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
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
                    cmbARAccount.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void cmbSalesRep_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void cmbSalesRep_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtSalesRep.Text = cmbSalesRep.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
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
                    " tblItemMaster.PriceLevel5, tblItemMaster.UnitCost,tblItemWhse.QTY " +
                    " FROM            tblCutomerReturn LEFT OUTER JOIN " +
                    " tblItemMaster ON tblCutomerReturn.ItemID = tblItemMaster.ItemID LEFT OUTER JOIN " +
                    " tblItemWhse ON tblCutomerReturn.ItemID = tblItemWhse.ItemId AND tblCutomerReturn.LocationID = tblItemWhse.WhseId where CreditNo='" + txtCreditNo.Text.Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cmbCustomer.Text = dt.Rows[0]["CustomerID"].ToString();
                    //cmbSalesRep.Text = dt.Rows[0][""].ToString();
                    cmbWarehouse.Text = dt.Rows[0]["LocationID"].ToString();
                    cmbARAccount.Text = dt.Rows[0]["ARAccount"].ToString();
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
                        ugR.Cells["OnHand"].Value = dr["QTY"].ToString();
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
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbCustomer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void ugItem2_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "UnitPrice" || e.Cell.Column.Key == "Quantity")
                {
                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    //GrandTotal();
                    InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                    FooterCalculation();
                    REQItemUnitTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void ugItem2_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                if (ugItem2.ActiveCell.Column.Key == "ItemCode" || ugItem2.ActiveCell.Column.Key == "GL")
                {
                    if (ugItem2.ActiveCell.Value.ToString() == ugItem2.ActiveCell.Text)
                    {
                        return;
                    }                    
                    //else
                    {
                        ugItem2.ActiveCell.Value = ugItem2.ActiveCell.Text;
                       
                        if (IsGridExitCode(ugItem2.ActiveCell.Row.Cells[1].Text,"Item") == false)
                        {
                            e.Cancel = true;
                        }
                        
                        foreach (UltraGridRow ugR in ultraCombo2.Rows)
                        {
                            if (ugItem2.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ugItem2.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDis"].Value;
                                ugItem2.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ugItem2.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ugItem2.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ugItem2.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ugItem2.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ugItem2.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ugItem2.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ugItem2.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ugItem2.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel1"].Value;
                            }
                        }
                    }
                }

                if (ugItem2.ActiveCell.Column.Key == "Description")
                {
                    if (ugItem2.ActiveCell.Value.ToString() == ugItem2.ActiveCell.Text)
                    {
                        return;
                    }
                    else
                    {
                        ugItem2.ActiveCell.Value = ugItem2.ActiveCell.Text;
                        if (IsGridExitCode(ugItem2.ActiveCell.Row.Cells[2].Text,"Item") == false)
                        {
                            e.Cancel = true;
                        }
                        foreach (UltraGridRow ugR in ultraCombo4.Rows)
                        {
                            if (ugItem2.ActiveCell.Value.ToString() == ugR.Cells["ItemDis"].Value.ToString())
                            {
                                ugItem2.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value;
                                ugItem2.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ugItem2.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ugItem2.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ugItem2.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                                ugItem2.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ugItem2.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ugItem2.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;


                                ugItem2.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ugItem2.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                                ugItem2.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel1"].Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ugItem2_Click(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;

                if (HeaderValidation() == false)
                {
                    return;
                }
                if (ugItem2.Rows.Count == 0)
                {
                    ugR = ugItem2.DisplayLayout.Bands[0].AddNew();
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

        private void ugItem2_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ugItem2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ugItem2.PerformAction(UltraGridAction.PrevCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 38:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            //   ugItem2.PerformAction(UltraGridAction.AboveCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 39:
                        {
                            //ug.PerformAction(UltraGridAction.ExitEditMode);
                            ugItem2.PerformAction(UltraGridAction.NextCell);
                            //ug.PerformAction(UltraGridAction.EnterEditMode);
                            break;
                        }
                    case 40:
                        {
                            if (ug.ActiveRow.Index != 0)
                                ug.PerformAction(UltraGridAction.BelowCell);
                            break;
                        }

                    case 9:
                        {
                            if (ugItem2.ActiveCell.Column.Key == "Quantity")
                            {
                                if (ugItem2.ActiveRow.HasNextSibling() == false)
                                {
                                    if (ugItem2.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                                    {
                                        UltraGridRow ugR;
                                        ugR = ugItem2.DisplayLayout.Bands[0].AddNew();
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
                MessageBox.Show(ex.Message);
            }
        }

        private void ug_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyValue)
                {
                    case 37:
                        {
                            if (ug.ActiveCell.Column.Key == "ItemCode" || ug.ActiveCell.Column.Key == "Description" || ug.ActiveCell.Column.Key == "UnitPrice" || ug.ActiveCell.Column.Key == "OnHand" || ug.ActiveCell.Column.Key == "Quantity" || ug.ActiveCell.Column.Key == "Discount" || ug.ActiveCell.Column.Key == "TotalPrice")
                                ug.PerformAction(UltraGridAction.PrevCell);
                            break; 

                            //ug.PerformAction(UltraGridAction.PrevCell);
                            //break;
                        }
                    case 38:
                        {
                            //ug.PerformAction(UltraGridAction.AboveCell);
                            break;
                        }
                    case 39:
                        {
                            ug.PerformAction(UltraGridAction.NextCell);
                            break;
                        }
                    case 40:
                        {
                            if (ug.ActiveRow.Index != 0)
                                ug.PerformAction(UltraGridAction.BelowCell);
                            break;
                        }

                    case 9:
                        {                          
                            if (ug.ActiveCell.Column.Key == "Totalprice") //if (ug.ActiveCell.Column.Key == "Quantity")
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
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtNBTPer_ValueChanged_1(object sender, EventArgs e)
        {
            //CalculateTaxAmounts();
            try
            {

                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtVatPer_ValueChanged_1(object sender, EventArgs e)
        {
            //CalculateTaxAmounts();
            try
            {
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Direct Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

      

        private void btnsprint_Click(object sender, EventArgs e)
        {
            try
            {
                SalesRequestPrint(txtCreditNo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraCombo1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void ug_BeforeEnterEditMode(object sender, CancelEventArgs e)
        {

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
                        if (ug.ActiveCell.Row.Cells["ItemCode"].Text != string.Empty)
                        {
                            if (IsGridExitCode(ug.ActiveCell.Row.Cells["ItemCode"].Text, "Item") == false)
                            {
                                e.Cancel = true;
                            }
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
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                                ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                                ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;

                                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;
                                
                                if (dblCusPriceLevel == 0)
                                {
                                   // dblCusPriceLevel = 1;
                                    string PriceLevel = "PriceLevel" + "1";
                                   // string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;
                                }
                                else
                                {
                                    string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;
                                }
                                // ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel5"].Value;
                               // ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;

                                //ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["UnitPrice"].Value;
                            }
                        }
                    }
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
                        if (IsGridExitCode(ug.ActiveCell.Row.Cells["Description"].Text, "Description") == false)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (IsGridExitCode(ug.ActiveCell.Row.Cells["GL"].Text,"GL") == false)
                            {
                                e.Cancel = true;
                            } 
                        }
                        //test
                        foreach (UltraGridRow ugR in ultraCombo3.Rows)
                        {
                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemDis"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["ItemCode"].Value = ugR.Cells["ItemId"].Value;
                                ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
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
                                if (dblCusPriceLevel == 0)
                                {
                                    // dblCusPriceLevel = 1;
                                    string PriceLevel = "PriceLevel" + "1";
                                    // string PriceLevel = "PriceLevel" + (dblCusPriceLevel).ToString().Trim();
                                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;
                                }
                                else
                                {
                                    string PriceLevel = "PriceLevel" + (dblCusPriceLevel + 1).ToString().Trim();
                                    ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells[PriceLevel].Value;
                                }                                
                            }
                        }
                    }
                }

                //if (ug.ActiveCell.Column.Key == "GL")
                //{
                //    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        ug.ActiveCell.Value = ug.ActiveCell.Text;
                //        if (IsGridExitCode(ug.ActiveCell.Row.Cells["GL"].Text, "GL") == false)
                //        {
                //            e.Cancel = true;
                //        }
                //        foreach (UltraGridRow ugR in ultraCombo5.Rows)
                //        {
                //            if (ug.ActiveCell.Value.ToString() == ugR.Cells[0].Value.ToString())
                //            {
                //                ug.ActiveCell.Row.Cells["ItemCode"].Value = string.Empty;
                //                ug.ActiveCell.Row.Cells["OnHand"].Value = "1";
                //                ug.ActiveCell.Row.Cells["ItemClass"].Value = "2";
                //                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["AcountID"].Value;
                //                ug.ActiveCell.Row.Cells["CostPrice"].Value = "0.00";
                //                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                //                ug.ActiveCell.Row.Cells["UOM"].Value = string.Empty;
                //                ug.ActiveCell.Row.Cells["Categoty"].Value = string.Empty;
                //                ug.ActiveCell.Row.Cells["PriceLevel1"].Value = 0.00;
                //                ug.ActiveCell.Row.Cells["PriceLevel2"].Value = 0.00;
                //                ug.ActiveCell.Row.Cells["PriceLevel3"].Value = 0.00;
                //                ug.ActiveCell.Row.Cells["PriceLevel4"].Value = 0.00;
                //                ug.ActiveCell.Row.Cells["PriceLevel5"].Value = 0.00;
                //            }
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCustomerPO_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerPO_Leave(object sender, EventArgs e)
        {
            try
            {
                UltraGridRow ugR;
                ugR = ug.DisplayLayout.Bands[0].AddNew();
                ugR.Cells["LineNo"].Value = ugR.Index + 1;
                ugR.Cells["Itemcode"].Selected = true;
                ugR.Cells["Itemcode"].Activated = true;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    Print(txtCreditNo.Text);
                    groupBox1.Visible = false;
                    ButtonClear();
                    NewRecord();
                }
                else
                {
                    PrintPOS(txtCreditNo.Text);
                    groupBox1.Visible = false;
                    ButtonClear();
                    NewRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ugItem2_BeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
          
        }
        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void NewRecord()
        {
            btnSave.Enabled = true;
            btnNew.Enabled = false;
            btnPrint.Enabled = false;
            // btnSNO.Enabled = false;
            btnSearch.Enabled = false;
            btnReset.Enabled = true;
            // btnEdit.Enabled = false;
            EnableHeader(true);
            EnableFoter(true);

            ClearHeader();
            DeleteRows();
            GetSONum();
            GetCustomer();
            GetWareHouseDataSet();
            GetCustomer();
            GetSalesRep();
            loadDefaltOption();
            loadChartofAcount();
            LoadDefualtAccount();
            LoadDefualtWH();
            LoadDefualSalesRep();
            GetItemDataSet();
            GetItemDataSetForDescription();
            GetItemDataSet1();
            cmbCustomer.Focus();
            GetTaxDeails();
        }
                
        private void cmbInvoiceType_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                double Tax1Rate = double.Parse(txtNBTPer.Text.Trim());
                double Tax2Rate = double.Parse(txtVatPer.Text.Trim());
                if (Convert.ToInt64(cmbInvoiceType.Value) == 1)//inclusive
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 2)//Exclusive
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                }
                else if (Convert.ToInt64(cmbInvoiceType.Value) == 3)
                {
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["UnitPrice(Incl)"].Hidden = true;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice"].Hidden = false;
                    ug.DisplayLayout.Bands[0].Columns["TotalPrice(Incl)"].Hidden = true;
                    Tax1Rate = 0.00;
                    Tax2Rate = 0.00;
                    txtNBTPer.Value="0.00";
                    txtVatPer.Value = "0.00";
                }
                //Invoice calculation
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice (Inclusive/Exclusive)", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
        public void FooterCalculation_Asss()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;
                    double dblTotalDiscountTemp = 0;

                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Value.ToString().Trim());
                    //dblServiceChgF = Convert.ToDouble(txtServicecharge.Value.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Value.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    dblTotalDiscountTemp = dblTotalDiscount +
                    (double.Parse(txtServicecharge.Value.ToString()) * double.Parse(txtDiscPer.Value.ToString()) / 100);
                    double _AmountWithoutDisc = double.Parse(txtSubValue.Value.ToString()) + double.Parse(txtDiscLineTot.Text) - dblTotalDiscountTemp + double.Parse(txtServicecharge.Value.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                        txtNBT.Value = _AmountWithoutDisc * double.Parse(txtNBTPer.Value.ToString()) / 100;

                        txtVat.Value = Convert.ToDouble((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                        double.Parse(txtVatPer.Value.ToString()) / 100).ToString("0.00");

                        //txtVat.Value = Math.Round(((_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) *
                        //double.Parse(txtVatPer.Value.ToString()) / 100),1, MidpointRounding.AwayFromZero);

                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Value.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Value.ToString().Trim());
                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF;
                    txtNetValue.Value = dblNetTotalF.ToString("N2");
                    //Asanga
                    //.Text = dblNetTotalF.ToString("N2");

                    if (cmbInvoiceType.Value.ToString() == "1")
                         txtDiscAmount.Text = ((Convert.ToDouble(txtGridTotalExcl.Text) -
                            Convert.ToDouble(txtDiscLineTot.Text)) *
                            Convert.ToDouble(txtDiscPer.Text) / 100).ToString();

                        //txtDiscAmount.Text = (Convert.ToDouble(txtGridTotalExcl.Text) *
                          //  Convert.ToDouble(txtDiscPer.Text) / 100).ToString();
                    else
                        txtDiscAmount.Text = (Convert.ToDouble(txtSubValue.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();


                }
            }
            catch { }
        }
        public void InvoiceCalculation_TEstss(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {


                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;

                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    txtGridTotalExcl.Text = "0";

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {

                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            //ug.ActiveCell.Row.Cells
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Math.Round(Convert.ToDouble(ugR.Cells["Discount"].Value) / 100, 2, MidpointRounding.AwayFromZero);
                            dblDiscountedPrice = Math.Round(dblLinePrice - (dblLinePrice * dbdiscountPer), 2, MidpointRounding.AwayFromZero);
                            dblLinediscountAmount = Math.Round(dblLinePrice - dblDiscountedPrice, 2, MidpointRounding.AwayFromZero);
                            
                            //dblPriceWithoutVat = Math.Round(((dblDiscountedPrice * 100) / (100 + dblVATPer)), 2, MidpointRounding.AwayFromZero);//price without vat=102
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);
                            txtGridTotalExcl.Text = (double.Parse(txtGridTotalExcl.Text) + (dblDiscountedPrice*dblLineQty)).ToString();

                            dblLineVAT = Math.Round(((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                           // dblExcluisvePrice = Math.Round(((dblPriceWithoutVat * 100) / (100 + dblNBTPer)), 2, MidpointRounding.AwayFromZero);//exclusive price here eg 100
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                            dblLineNBT = Math.Round(((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            ugR.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount; //dblInclusiveLineTotal;
                            ugR.Cells["TotalPrice"].Value = Math.Round(dblLineQty * dblLinePrice, 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = Math.Round((dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
//                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblSubTotal = dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            //dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = dblTotalLinedDiscount + dblLinediscountAmount;

                        }
                        txtSubValue.Value = dblSubTotal;
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero);
                        //.Text = txtNetValue.Value.ToString();
                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty)
                                dblLinePrice = 0;
                            else
                                dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty)
                                dblLineQty = 0;
                            else
                                dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            //dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblSubTotal = dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);


                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal + dblTotalNBT + dblTotalVAT);
                        //.Text = txtNetValue.Value.ToString();
                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            //error
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            //error
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT                            
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            //dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblSubTotal = dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal + dblTotalNBT + dblTotalVAT);
                        //.Text = txtNetValue.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void InvoiceCalculation(Int64 iInvoiceType)
        {
            try
            {
                if (IsFind != true)
                {

                    double dblSubTotal = 0;
                    double dblTotVAT = 0;
                    double dblDiscountedPrice = 0;
                   
                    double dblPriceWithoutVat = 0;
                    double dblExcluisvePrice = 0;

                    double dbdiscountPer = 0;
                    double dblLinediscountAmount = 0;

                    double dblInclusiveLineTotal = 0;
                    double dblDiscountedLineTotal = 0;

                    double dblLineVAT = 0;
                    double dblLineNBT = 0;

                    double dblTotalVAT = 0;
                    double dblTotalNBT = 0;
                    double dblTotalLinedDiscount = 0;

                    double dblLineTax = 0;
                    double dblLineQty = 0;
                    double dblLinePrice = 0;
                    double dblVATPer = Convert.ToDouble(txtVatPer.Value);  //12%
                    double dblNBTPer = Convert.ToDouble(txtNBTPer.Value);  //2%

                    txtGridTotalExcl.Text = "0";

                    if (iInvoiceType == 1) // Inclusive
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {

                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            //ug.ActiveCell.Row.Cells
                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);//here price in inclusive eg-114.24
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblDiscountedPrice = dblLinePrice - (dblLinePrice * dbdiscountPer);
                            dblLinediscountAmount = dblLinePrice - dblDiscountedPrice;
                            txtGridTotalExcl.Text = (double.Parse(txtGridTotalExcl.Text) + (dblDiscountedPrice * Convert.ToDouble(ugR.Cells["Quantity"].Value))).ToString();
                            dblPriceWithoutVat = ((dblDiscountedPrice * 100) / (100 + dblVATPer));//price without vat=102
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);
                            dblLineVAT = ((dblDiscountedPrice - dblPriceWithoutVat) * dblLineQty);
                            dblExcluisvePrice = ((dblPriceWithoutVat * 100) / (100 + dblNBTPer));//exclusive price here eg 100
                            dblLineNBT = ((dblPriceWithoutVat - dblExcluisvePrice) * dblLineQty);
                            ugR.Cells["UnitPrice(Incl)"].Value = dblExcluisvePrice;//100
                            dblInclusiveLineTotal = dblExcluisvePrice * dblLineQty;
                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal;
                            ugR.Cells["TotalPrice"].Value = dblLineQty * dblLinePrice;

                            //VAT
                            dblLineTax = (dblLineNBT + dblLineVAT);
                            ugR.Cells["LineTax"].Value = dblLineTax;

                            //Inv Sub Total
                            dblSubTotal = dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value);

                            //Inv TAX Total
                            dblTotalVAT = dblTotalVAT + dblLineVAT;
                            dblTotalNBT = dblTotalNBT + dblLineNBT;
                            dblTotalLinedDiscount = dblTotalLinedDiscount + dblLinediscountAmount;

                        }
                        txtSubValue.Value = dblSubTotal;
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = Math.Round((dblSubTotal + dblTotalVAT + dblTotalNBT), 2, MidpointRounding.AwayFromZero);
                        //.Text = txtNetValue.Value.ToString();
                    }
                    else if (iInvoiceType == 2)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty)
                                dblLinePrice = 0;
                            else
                                dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty)
                                dblLineQty = 0;
                            else
                                dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLinePrice * dblLineQty), 2, MidpointRounding.AwayFromZero);
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty)
                                dbdiscountPer = 0;
                            else
                                dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;
                            dblDiscountedLineTotal = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            dblLineNBT = Math.Round(((dblDiscountedLineTotal) * dblNBTPer / 100), 2, MidpointRounding.AwayFromZero);
                            dblLineVAT = Math.Round((((dblDiscountedLineTotal) + dblLineNBT) * dblVATPer / 100), 2, MidpointRounding.AwayFromZero);

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT
                            dblLineTax = dblLineNBT + dblLineVAT;
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineTax, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);

                            //Inv TAX Total
                            dblTotalVAT = Math.Round(dblTotalVAT + dblLineVAT, 2, MidpointRounding.AwayFromZero);
                            dblTotalNBT = Math.Round(dblTotalNBT + dblLineNBT, 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);


                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal + dblTotalNBT + dblTotalVAT);
                        //.Text = txtNetValue.Value.ToString();
                    }

                    else if (iInvoiceType == 3)
                    {
                        foreach (UltraGridRow ugR in ug.Rows)
                        {
                            //error
                            if (ugR.Cells["UnitPrice"].Value == null || ugR.Cells["UnitPrice"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice"].Value = 0.00;
                            if (ugR.Cells["Discount"].Value == null || ugR.Cells["Discount"].Value.ToString().Trim() == string.Empty) ugR.Cells["Discount"].Value = 0.00;
                            if (ugR.Cells["Quantity"].Value == null || ugR.Cells["Quantity"].Value.ToString().Trim() == string.Empty) ugR.Cells["Quantity"].Value = 0.00;
                            if (ugR.Cells["UnitPrice(Incl)"].Value == null || ugR.Cells["UnitPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["UnitPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["TotalPrice(Incl)"].Value == null || ugR.Cells["TotalPrice(Incl)"].Value.ToString().Trim() == string.Empty) ugR.Cells["TotalPrice(Incl)"].Value = 0.00;
                            if (ugR.Cells["LineTax"].Value == null || ugR.Cells["LineTax"].Value.ToString().Trim() == string.Empty) ugR.Cells["LineTax"].Value = 0.00;

                            if (ugR.Cells["ItemCode"].Value == null || ugR.Cells["ItemCode"].Value.ToString().Trim() == string.Empty)
                                break;
                            //error
                            dblLinePrice = Convert.ToDouble(ugR.Cells["UnitPrice"].Value);
                            dblLineQty = Convert.ToDouble(ugR.Cells["Quantity"].Value);

                            dblInclusiveLineTotal = Math.Round((dblLineQty * dblLinePrice) + (dblLineNBT + dblLineVAT), 2, MidpointRounding.AwayFromZero);
                            dbdiscountPer = Convert.ToDouble(ugR.Cells["Discount"].Value) / 100;
                            dblLinediscountAmount = dblInclusiveLineTotal * dbdiscountPer;

                            ugR.Cells["TotalPrice(Incl)"].Value = dblInclusiveLineTotal - dblLinediscountAmount;

                            ugR.Cells["TotalPrice"].Value = Math.Round((dblLineQty * dblLinePrice), 2, MidpointRounding.AwayFromZero);

                            //VAT                            
                            ugR.Cells["LineTax"].Value = Math.Round(dblLineVAT, 2, MidpointRounding.AwayFromZero);

                            //Inv Sub Total
                            dblSubTotal = Math.Round(dblSubTotal + Convert.ToDouble(ugR.Cells["TotalPrice(Incl)"].Value), 2, MidpointRounding.AwayFromZero);
                            dblTotalLinedDiscount = Math.Round(dblTotalLinedDiscount + dblLinediscountAmount, 2, MidpointRounding.AwayFromZero);

                        }
                        txtDiscLineTot.Text = dblTotalLinedDiscount.ToString("N2");
                        txtDiscAmount.Value = dblTotalLinedDiscount;
                        txtSubValue.Value = dblSubTotal;
                        txtVat.Value = dblTotalVAT;
                        txtNBT.Value = dblTotalNBT;
                        txtNetValue.Value = (dblSubTotal + dblTotalNBT + dblTotalVAT);
                        //.Text = txtNetValue.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FooterCalculation()
        {
            try
            {
                if (IsFind != true)
                {
                    double dblGrossTotalF = 0;
                    double dblNetTotalF = 0;
                    double dblNBTF = 0;
                    double dblVATF = 0;
                    double dblDiscountF = 0;
                    double dblServiceChgF = 0;
                    double dblDiscountLineTot = 0;
                    double dblTotalDiscount = 0;
                    double dblTotalDiscountTemp = 0;
                    double Fdiscount = 0;
                    dblGrossTotalF = Convert.ToDouble(txtSubValue.Value.ToString().Trim()); //4.01785714285714
                    //dblServiceChgF = Convert.ToDouble(txtServicecharge.Value.ToString().Trim());
                    dblDiscountF = (dblGrossTotalF) * Convert.ToDouble(txtDiscPer.Value.ToString().Trim()) / 100;
                    dblDiscountLineTot = Convert.ToDouble(txtDiscLineTot.Text.ToString().Trim());
                    dblTotalDiscount = dblDiscountLineTot + dblDiscountF;

                    dblTotalDiscountTemp = dblTotalDiscount +
                    (double.Parse(txtServicecharge.Value.ToString()) * double.Parse(txtDiscPer.Value.ToString()) / 100);
                    double _AmountWithoutDisc = double.Parse(txtSubValue.Value.ToString()) + double.Parse(txtDiscLineTot.Text) - dblTotalDiscountTemp + double.Parse(txtServicecharge.Value.ToString());

                    if (cmbInvoiceType.Value.ToString() != "3")
                    {
                    //    txtNBT.Value = _AmountWithoutDisc * double.Parse(txtNBTPer.Value.ToString()) / 100;
                    //    txtVat.Value = (_AmountWithoutDisc + double.Parse(txtNBT.Value.ToString())) * //0.48214285714285682
                    //    double.Parse(txtVatPer.Value.ToString()) / 100;
                    }
                    dblNBTF = Convert.ToDouble(txtNBT.Value.ToString().Trim());
                    dblVATF = Convert.ToDouble(txtVat.Value.ToString().Trim());
                    dblNetTotalF = _AmountWithoutDisc + dblNBTF + dblVATF; //4.4999999999999964
                    //28.125+3.375
                    txtNetValue.Value = dblNetTotalF.ToString("N2"); //
                    //Asanga
                    //.Text = dblNetTotalF.ToString("N2");
                    Fdiscount = Convert.ToDouble(txtDiscPer.Value.ToString().Trim());
                    if (cmbInvoiceType.Value.ToString() == "1")                     
                        txtDiscAmount.Text = (Convert.ToDouble(txtGridTotalExcl.Text) *
                                Convert.ToDouble(txtDiscPer.Text) / 100).ToString();                      
                        else
                            txtDiscAmount.Text = (Convert.ToDouble(txtSubValue.Text.Trim()) * Convert.ToDouble(txtDiscPer.Text) / 100).ToString();


                }
            }
            catch { }
        }
        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //GrandTotal();
                InvoiceCalculation(Convert.ToInt64(cmbInvoiceType.Value));
                FooterCalculation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SalesRequestPrint(txtCreditNo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSubValue_ValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ug_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        

    }
}