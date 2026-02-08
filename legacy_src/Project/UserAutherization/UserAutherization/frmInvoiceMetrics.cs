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
using UserAutherization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmInvoiceMetrics : Form
    {
        //public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();

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
        public string StrPaymmetM;
        public string StrRetailCustomer;
        public Boolean blnRetailsCustomer;
        //Following Code Segment Define GL Accounts
        public string sMsg = "Peachtree - Invoice";
        public string StrARAccount;
        public string StrCashAccount;
        public string StrSalesGLAccount;
        public double dblQtyLevel1;
        public double dblQtyLevel2;
        public double dblQtyLevel3;
        public DataSet dsCustomer;
        public DataSet dsWarehouse;
        public DataSet dsSalesRep;
        public DataSet dsAR;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        clsCommon objclsCommon = new clsCommon();

        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmInvoiceMetrics()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmInvoiceMetrics(int intNo)
        {
            InitializeComponent();
            setConnectionString();

            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }
        }

        public frmInvoiceMetrics(string intNo)
        {
            InitializeComponent();
            setConnectionString();           
        }

        private void setValue()
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
                ViewHeader(strNo);
                ViewDetails(strNo);
                EnableHeader(false);
                EnableFoter(false);
                btnSave.Enabled = false;
            }
        }

        public void GetCustomer()
        {

            dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster order by CutomerID";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbCustomer.DataSource = dsCustomer.Tables["DtClient"];
                cmbCustomer.DisplayMember = "CutomerID";
                cmbCustomer.ValueMember = "CutomerID";

                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address1"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["Address2"].Hidden = true;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CutomerID"].Width = 100;
                cmbCustomer.DisplayLayout.Bands["DtClient"].Columns["CustomerName"].Width = 150;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;

            }
            catch { }
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
                StrSql = "SELECT JobActPref, JobActPad, JobActNum FROM tblDefualtSetting";
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

                    StrSql = "UPDATE EST_HEADER SET  EstimateNo='" + StrUpdateInvNo + "' WHERE AutoIndex=" + intInvoiceNo + "";
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void GetQuantityMatrix()
        {
            try
            {
                StrSql = "SELECT  QTY1,QTY2,QTY3 FROM tblQuantityMatrix";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dblQtyLevel1 = double.Parse(dt.Rows[0].ItemArray[0].ToString().Trim());
                    dblQtyLevel2 = double.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    dblQtyLevel3 = double.Parse(dt.Rows[0].ItemArray[2].ToString().Trim());
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetRetailCustomer()
        {
            try
            {
                StrSql = "SELECT  CutomerID FROM tblDefualtSetting";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrRetailCustomer = dt.Rows[0].ItemArray[0].ToString().Trim();
                }
                else
                {
                    StrRetailCustomer = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;
                SqlCommand command;

                if (optSerialOne.Checked == true)
                {
                    StrSql = "SELECT  TOP 1(InvNum) FROM tblDefualtSetting ORDER BY InvNum DESC";
                }
                else
                {
                    StrSql = "SELECT  TOP 1(InvNum1) FROM tblDefualtSetting ORDER BY InvNum1 DESC";
                }

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

                if (optSerialOne.Checked == true)
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvNum='" + intInvNo + "'";
                }
                else
                {
                    StrSql = "UPDATE tblDefualtSetting SET InvNum1='" + intInvNo + "'";
                }

                command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
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

                if (optSerialOne.Checked == true)
                {
                    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                }
                else
                {
                    StrSql = "SELECT InvPref1, InvPad1, InvNum1 FROM tblDefualtSetting";
                }


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

                MessageBox.Show("Error :" + ex.Message);
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

                if (optSerialOne.Checked == true)
                {
                    StrSql = "SELECT InvPref, InvPad, InvNum FROM tblDefualtSetting";
                }
                else
                {
                    StrSql = "SELECT InvPref1, InvPad1, InvNum1 FROM tblDefualtSetting";
                }

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

            catch (Exception)
            {
                return null;
                throw;
            }



        }

        public void GetAR()
        {
            //dsAR = new DataSet();
            //try
            //{
            //    dsAR.Clear();
            //    StrSql = " SELECT ArAccount FROM tblDefualtSetting";

            //    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
            //    dAdapt.Fill(dsAR, "DtAR");

            //    cmbAR.DataSource = dsAR.Tables["DtAR"];
            //    cmbAR.DisplayMember = "ArAccount";
            //    cmbAR.ValueMember = "ArAccount";
            //    cmbAR.DisplayLayout.Bands["DtAR"].Columns["ArAccount"].Width = 180;



            //}
            //catch (Exception)
            //{

            //    throw;
            //}


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
                cmbSalesRep.DisplayMember = "RepName";
                cmbSalesRep.ValueMember = "RepCode";
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepCode"].Width = 75;
                cmbSalesRep.DisplayLayout.Bands["DtSalesRep"].Columns["RepName"].Width = 125;

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
                //if (dsWarehouse.Tables[0].Rows.Count > 0)
                //{
                //    cmbWarehouse.Value =  cmbWarehouse.GetRow(ChildRow.First).Cells["WhseId"].Value   ;   
                //}

            }
            catch (Exception)
            {

                throw;
            }
        }

        //===================================

        //=======================================



        public void GetItemDataSet()
        {
            try
            {

                if (cmbWarehouse.Text == "")
                {
                    return;
                }

                StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";

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
                    ultraCombo1.DisplayLayout.Bands[0].Columns[1].Width = 200;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Width = 60;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[3].Width = 100;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[4].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[5].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[6].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[7].Width = 70;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[8].Width = 70;

                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;

                }
                else
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";


                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[9].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[10].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[11].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[12].Hidden = true;
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Hidden = true;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private void DeleteRows()
        {
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                ugR.Delete(false);
            }
            GrandTotal();
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

        private void ug_KeyDown(object sender, KeyEventArgs e)
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

                        if (ug.ActiveCell.Column.Key == "Quantity")
                        {
                            if (ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString().Trim() != string.Empty)
                            {
                                if (ug.ActiveRow.HasNextSibling() == false)
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

        private double LineCalculation(double UnitPrice, double Quantity)
        {
            dblLineTot = 0;
            double lineTotal = 0;
            dblLineTot = UnitPrice * Quantity;
            lineTotal = dblLineTot;
            return lineTotal;

        }


        private void ug_Click(object sender, EventArgs e)
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

        public Boolean IsGridValidation()
        {
            if (ug.Rows.Count == 0)
            {
                MessageBox.Show("No Items In the Gird....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            foreach (UltraGridRow ugR in ug.Rows)
            {
                if (IsGridExitCode(ugR.Cells["ItemCode"].Text) == false)
                {
                    MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                {
                    MessageBox.Show("Quantity Should be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            if (!IsWarehouseHaveStock())
            {
                return false;
            }
            return true;
        }

        private bool IsWarehouseHaveStock()
        {
            try
            {
                double _RtnStock = 0;
                double _IssQty = 0;
                //DataSet _dts = new DataSet();
                //_dts = objclsBLLPhases.GetToBeIssuedQty(ucmbBOMID.Value.ToString().Trim());

                foreach (UltraGridRow dr in ug.Rows)
                {
                    SqlConnection con = new SqlConnection(ConnectionString);
                    string s = "select QTY from tblItemWhse where ItemID = '" + dr.Cells[1].Value.ToString().Trim() + "' and WhseId='" + cmbWarehouse.Text.Trim() + "'";
                    SqlDataAdapter da = new SqlDataAdapter(s, con);
                    DataTable _dtbl = new DataTable();
                    da.Fill(_dtbl);

                    _RtnStock = double.Parse(_dtbl.Rows[0][0].ToString());

                    _IssQty = 0;
                    foreach (UltraGridRow udr in ug.Rows)
                    {
                        if (dr.Cells[1].ToString() == udr.Cells[1].Value.ToString())
                        {
                            _IssQty = _IssQty + double.Parse(udr.Cells[4].Value.ToString().Trim());
                        }
                        if (_RtnStock < _IssQty)
                        {
                            MessageBox.Show("Not Enough Qty for " + dr.Cells[1].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
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

        public Boolean HeaderValidation()
        {
            if (cmbCustomer.Text.Trim() == "")
            {
                MessageBox.Show("Select the Customer...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbWarehouse.Text.Trim() == "")
            {
                MessageBox.Show("Select the Warehouse...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbSalesRep.Text.Trim() == "")
            {
                MessageBox.Show("Select the SalesRep...!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        public void ViewDetails(string StrInvoiceNo)
        {
            try
            {

                StrSql = "SELECT tblSalesInvoices.DistributionNo,tblSalesInvoices.ItemID,tblSalesInvoices.Description,tblSalesInvoices.Qty,tblSalesInvoices.UnitPrice,tblSalesInvoices.Amount,tblItemWhse.QTY as WHQTY,tblSalesInvoices.ItemClass,tblSalesInvoices.ItemType,tblSalesInvoices.GLAcount,tblSalesInvoices.UOM,tblSalesInvoices.CostPrrice FROM tblSalesInvoices INNER JOIN tblItemWhse ON tblItemWhse.ItemID=tblSalesInvoices.ItemID AND tblItemWhse.WhseId=tblSalesInvoices.Location  WHERE tblSalesInvoices.InvoiceNo='" + StrInvoiceNo + "' ORDER BY tblSalesInvoices.DistributionNo";

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
                        ugR.Cells["LineNo"].Value = Dr["DistributionNo"];
                        ugR.Cells["ItemCode"].Value = Dr["ItemID"];
                        ugR.Cells["Description"].Value = Dr["Description"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitPrice"];
                        ugR.Cells["Quantity"].Value = Dr["Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["Amount"];

                        ugR.Cells["OnHand"].Value = Dr["WHQTY"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["GLAcount"];

                        ugR.Cells["UOM"].Value = Dr["UOM"];
                        ugR.Cells["CostPrice"].Value = Dr["CostPrrice"];


                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }

        public void ViewHeader(string StrInvoiceNo)
        {
            try
            {

                StrSql = "SELECT tblSalesInvoices.InvoiceNo,tblSalesInvoices.InvoiceDate,tblSalesInvoices.CustomerID,tblSalesInvoices.Location,tblSalesInvoices.Comments,tblSalesInvoices.ARAccount,tblSalesInvoices.SalesRep,tblSalesInvoices.PaymentM,tblSalesInvoices.Tax1Rate,tblSalesInvoices.Tax2Rate,tblSalesInvoices.Tax1Amount,tblSalesInvoices.Tax2Amount,tblSalesInvoices.Discount,tblSalesInvoices.DiscountAmount,tblSalesInvoices.SubValue,tblSalesInvoices.GrossTotal,tblSalesInvoices.NetTotal FROM tblSalesInvoices WHERE tblSalesInvoices.InvoiceNo='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtInvoiceNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    cmbCustomer.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    txtDescription.Text = (dt.Rows[0].ItemArray[4].ToString().Trim());
                    //cmbAR.Text = dt.Rows[0].ItemArray[5].ToString().Trim();
                    cmbSalesRep.Text = dt.Rows[0].ItemArray[6].ToString().Trim();

                    if (dt.Rows[0].ItemArray[7].ToString().Trim() == "Cash")
                    {
                        optCash.Checked = true;
                    }
                    else
                    {
                        optCredit.Checked = true;
                    }

                    txtNBTPer.Value = double.Parse(dt.Rows[0].ItemArray[8].ToString().Trim());
                    txtVatPer.Value = double.Parse(dt.Rows[0].ItemArray[9].ToString().Trim());
                    txtNBT.Value = double.Parse(dt.Rows[0].ItemArray[10].ToString().Trim());
                    txtVat.Value = double.Parse(dt.Rows[0].ItemArray[11].ToString().Trim());
                    txtDiscPer.Value = double.Parse(dt.Rows[0].ItemArray[12].ToString().Trim());
                    txtDiscAmount.Value = double.Parse(dt.Rows[0].ItemArray[13].ToString().Trim());


                    txtSubValue.Value = double.Parse(dt.Rows[0].ItemArray[14].ToString().Trim());
                    txtGrossValue.Value = double.Parse(dt.Rows[0].ItemArray[15].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0].ItemArray[16].ToString().Trim());

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }



        private void frmInvoices_Load(object sender, EventArgs e)
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
                    btnEdit.Enabled = false;
                    dtpDate.Enabled = false;

                    GetItemDataSet();
                    GetWareHouseDataSet();
                    GetCustomer();
                    GetSalesRep();
                    GetAR();
                    GetRetailCustomer();
                    GetQuantityMatrix();


                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);
                    GetInvNo();

                    SetDefaultWarehouse();
                }
                btnNew_Click(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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
                    UserWiseDate = Convert.ToDateTime(dt.Tables[0].Rows[i].ItemArray[0]);
                    dtpDate.Value = UserWiseDate;

                }
            }
            catch { }

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
                    txtWarehouseAddress.Text = dt2.Rows[0].ItemArray[2].ToString();
                    StrARAccount = dt2.Rows[0].ItemArray[3].ToString();
                    StrCashAccount = dt2.Rows[0].ItemArray[4].ToString();
                    StrSalesGLAccount = dt2.Rows[0].ItemArray[5].ToString();
                    //sss
                }

            }
            catch { }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {

            this.Close();
        }


        private void SetDefaultWarehouse()
        {
            if (dsWarehouse.Tables[0].Rows.Count > 0)
            {
                cmbWarehouse.Value = cmbWarehouse.GetRow(ChildRow.First).Cells["WhseId"].Value;
                txtWarehouseName.Text = cmbWarehouse.ActiveRow.Cells[1].Value.ToString();
                StrARAccount = cmbWarehouse.ActiveRow.Cells[2].Value.ToString();
                StrCashAccount = cmbWarehouse.ActiveRow.Cells[3].Value.ToString();
                StrSalesGLAccount = cmbWarehouse.ActiveRow.Cells[4].Value.ToString();

            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (add)
            {
                //chkOverLimit.Checked = false;
                //chkOverLimit.Enabled = true;
                btnSave.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSNO.Enabled = false;
                btnSearch.Enabled = true;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;
                EnableHeader(true);
                EnableFoter(true);
                optCash.Checked = true;

                ClearHeader();
                DeleteRows();
                GetInvNo();
                cmbCustomer.Focus();
                SetDefaultWarehouse();
                GetItemDataSet();
            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private int GetEstimateCode(string strJobID, SqlConnection con, SqlTransaction Trans)
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
        //peahtree update=========================
        private void CreateXmlToExportInvAdjust(string StrItemCode, string strIssueNoteNo, DateTime dtDate, string StrJobId, double dblUnitcost, double dblQty, double dblLineTotal, SqlTransaction myTrans, SqlConnection myConnection)
        {

        }
        //============================================

        private void ImportSalesInvoice(int intGrid, string StrReference)
        {

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
            // --PH3 Posting--------------------
            for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                DateTime DTP = Convert.ToDateTime(dtpDate.Text);
                string Dformat = "MM/dd/yyyy";
                string InvDate = DTP.ToString(Dformat);


                if (intGrid < ug.Rows.Count)
                {

                    Writer.WriteStartElement("PAW_Invoice");
                    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(InvDate);//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Invoice_Number");
                    Writer.WriteString(StrReference);
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                    // Writer.WriteString(cmbSalesRep.Text.ToString().Trim());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Accounts_Receivable_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(StrARAccount);//Cash Account
                    Writer.WriteEndElement();//CreditMemoType

                    Writer.WriteStartElement("CreditMemoType");
                    Writer.WriteString("FALSE");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString((ug.Rows.Count).ToString());
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("SalesLines");

                    Writer.WriteStartElement("SalesLine");

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
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    // Writer.WriteString(ug.Rows[intGrid].Cells["GL"].Value.ToString());
                    Writer.WriteString(StrSalesGLAccount);
                    //StrSalesGLAccount
                    Writer.WriteEndElement();
                    //========================================================
                    Writer.WriteStartElement("Tax_Type");
                    Writer.WriteString("1");//Doctor Charge
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Amount");
                    Writer.WriteString("-" + ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());//HospitalCharge
                    Writer.WriteEndElement();


                    Writer.WriteEndElement();//LINE
                    Writer.WriteEndElement();//LINES

                    Writer.WriteEndElement();
                }

                //if (intGrid == ug.Rows.Count)
                //{
                //    Writer.WriteStartElement("PAW_Invoice");
                //    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                //    Writer.WriteStartElement("Customer_ID");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Date");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(InvDate);//Date 
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Invoice_Number");
                //    Writer.WriteString(StrReference);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Sales_Representative_ID");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Accounts_Receivable_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(StrARAccount);//Cash Account
                //    Writer.WriteEndElement();//CreditMemoType

                //    Writer.WriteStartElement("CreditMemoType");
                //    Writer.WriteString("FALSE");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Number_of_Distributions");
                //    Writer.WriteString((ug.Rows.Count + 2).ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("SalesLines");

                //    Writer.WriteStartElement("SalesLine");

                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("1");//Doctor Charge
                //    Writer.WriteEndElement();

                //    //"+ txtNBTPer.Value.ToString() +","+ txtVatPer.Value.ToString() +","+ txtNBT.Value.ToString() +","+ txtVat.Value.ToString() +",'NBT','VAT'
                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString("NBT");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString("NBT");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString("8300-00");
                //    Writer.WriteEndElement();

                //    //========================================================
                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");//Doctor Charge
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    Writer.WriteString("-" + txtNBT.Value.ToString().Trim());//HospitalCharge
                //    Writer.WriteEndElement();


                //    Writer.WriteEndElement();//LINE
                //    Writer.WriteEndElement();//LINES

                //    Writer.WriteEndElement();

                //}

                //if (intGrid == ug.Rows.Count + 1)
                //{
                //    Writer.WriteStartElement("PAW_Invoice");
                //    Writer.WriteAttributeString("xsi:type", "paw:Receipt");

                //    Writer.WriteStartElement("Customer_ID");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Date");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(InvDate);//Date 
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Invoice_Number");
                //    Writer.WriteString(StrReference);
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Sales_Representative_ID");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Accounts_Receivable_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString(StrARAccount);//Cash Account
                //    Writer.WriteEndElement();//CreditMemoType

                //    Writer.WriteStartElement("CreditMemoType");
                //    Writer.WriteString("FALSE");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Number_of_Distributions");
                //    Writer.WriteString((ug.Rows.Count + 2).ToString());
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("SalesLines");

                //    Writer.WriteStartElement("SalesLine");

                //    Writer.WriteStartElement("Quantity");
                //    Writer.WriteString("1");//Doctor Charge
                //    Writer.WriteEndElement();


                //    Writer.WriteStartElement("Item_ID");
                //    Writer.WriteString("VAT");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Description");
                //    Writer.WriteString("VAT");
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("GL_Account");
                //    Writer.WriteAttributeString("xsi:type", "paw:id");
                //    Writer.WriteString("8200-00");
                //    Writer.WriteEndElement();

                //    //========================================================
                //    Writer.WriteStartElement("Tax_Type");
                //    Writer.WriteString("1");//Doctor Charge
                //    Writer.WriteEndElement();

                //    Writer.WriteStartElement("Amount");
                //    Writer.WriteString("-" + txtVat.Value.ToString().Trim());//HospitalCharge
                //    Writer.WriteEndElement();


                //    Writer.WriteEndElement();//LINE
                //    Writer.WriteEndElement();//LINES

                //    Writer.WriteEndElement();


                //}
                //============================================================
            }
            Writer.Close();

            Connector ObjImportP = new Connector();
            ObjImportP.ImportDirectSalesInvice();

        }

        //following code segment export receipts to Peachtree=========


        public void exporetReceiptCash(int intGrid, string StrReference)
        {
            //try
            //{
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\CashReceipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);
            //sanjeewa

            //for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            //{

                //if (intGrid < ug.Rows.Count)
                //{
                    Writer.WriteStartElement("PAW_Receipt");
                    Writer.WriteAttributeString("xsi:type", "paw:receipt");


                    Writer.WriteStartElement("Customer_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Reference");
                    Writer.WriteString(StrReference + "R");
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Date");
                    Writer.WriteAttributeString("xsi:type", "paw:Date");
                    Writer.WriteString(InvDate);//Date 
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Payment_Method");
                    Writer.WriteString("Cash");//PayMethod
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Cash_Account");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(StrCashAccount);//Cash Account
                    Writer.WriteEndElement();

                    Writer.WriteStartElement("Sales_Representative_ID");
                    Writer.WriteAttributeString("xsi:type", "paw:id");
                    Writer.WriteString(cmbSalesRep.Value.ToString());//Cash Account
                    Writer.WriteEndElement();



                    // <Sales_Representative_ID xsi:type="paw:id">00001</Sales_Representative
                    Writer.WriteStartElement("ReceiptNumber");
                    Writer.WriteString(StrReference);
                    Writer.WriteEndElement();


                    Writer.WriteStartElement("Number_of_Distributions");
                    Writer.WriteString((ug.Rows.Count).ToString());
                    Writer.WriteEndElement();

                    //Writer.WriteStartElement("InvoicePaid");
                    //Writer.WriteString("");//PayMethod
                    //Writer.WriteEndElement();

                    Writer.WriteStartElement("Distributions");

                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {

                        Writer.WriteStartElement("Distribution");

                        Writer.WriteStartElement("InvoicePaid");
                        Writer.WriteString("");//PayMethod
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
                        Writer.WriteAttributeString("xsi:type", "paw:id");
                        Writer.WriteString(StrSalesGLAccount);
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString());//HospitalCharge
                        Writer.WriteEndElement();

                        Writer.WriteEndElement();
                    }
                    Writer.WriteEndElement();
                    Writer.WriteEndElement();

              //  }
         //   }

            Writer.Close();

            // Connector abc = new Connector();//export to peach tree

            Connector ObjImportP = new Connector();
            ObjImportP.Import_Receipt_JournalOnline();

            //}

            //catch { }


        }




        public void exporetReceipt(string StrReference)
        {

            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\Receipts.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Receipts");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");


            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            int rowCount1 = 1;
            //asb
            for (int i = 0; i < rowCount1; i++)
            {
                Writer.WriteStartElement("PAW_Receipt");
                Writer.WriteAttributeString("xsi:type", "paw:Receipt");


                Writer.WriteStartElement("Customer_ID");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(cmbCustomer.Text.ToString().Trim());//Customer ID should be here = Ptient No
                Writer.WriteEndElement();


                Writer.WriteStartElement("Reference");
                Writer.WriteString(StrReference + "R");
                Writer.WriteEndElement();


                Writer.WriteStartElement("Date ");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(InvDate);//Date 
                Writer.WriteEndElement();

                Writer.WriteStartElement("Payment_Method");
                Writer.WriteString("Cash");//PayMethod
                Writer.WriteEndElement();


                Writer.WriteStartElement("Cash_Account");
                Writer.WriteAttributeString("xsi:type", "paw:id");
                Writer.WriteString(StrCashAccount);//Cash Account
                Writer.WriteEndElement();


                Writer.WriteStartElement("Total_Paid_On_Invoices");
                Writer.WriteString("-" + txtNetValue.Value.ToString().Trim());//PayMethod
                Writer.WriteEndElement();


                Writer.WriteStartElement("ReceiptNumber");
                Writer.WriteString("R" + StrReference);
                Writer.WriteEndElement();

                Writer.WriteStartElement("Number_of_Distributions ");
                Writer.WriteString("1");
                Writer.WriteEndElement();


                //Writer.WriteStartElement("Prepayment");//Prepayment
                //Writer.WriteString("True");//PayMethod
                //Writer.WriteEndElement();

                Writer.WriteStartElement("InvoicePaid");
                Writer.WriteString(StrReference);//PayMethod
                // Writer.WriteString("");//PayMethod
                Writer.WriteEndElement();

                Writer.WriteStartElement("Distributions");
                Writer.WriteStartElement("Distribution");

                //Writer.WriteStartElement("InvoicePaid");
                //// Writer.WriteString(StrReference);//PayMethod
                //Writer.WriteString("");//PayMethod
                //Writer.WriteEndElement();

                Writer.WriteStartElement("Amount");
                Writer.WriteString("-" + txtNetValue.Value.ToString().Trim());//PayMethod
                Writer.WriteEndElement();


                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteEndElement();
                // Writer.Close();
            }

            Writer.Close();
            Connector ObjReceiptP = new Connector();
            ObjReceiptP.Import_Receipt_Journal();

            //Connector abc = new Connector();//export to peach tree
            //abc.i
            //abc.Import_Receipt_JournalOnline();



        }
        //============================================================


        private bool SaveEvent()
        {
            if (cmbWarehouse.Value == null)
            {
                MessageBox.Show("Incorrect Warehouse", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (cmbSalesRep.Value == null)
            {
                MessageBox.Show("Incorrect SalesRep", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (cmbCustomer.Value == null)
            {
                MessageBox.Show("Incorrect Customer", sMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
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
                DeleteEmpGrid();
                if (IsGridValidation() == false)
                {
                    return false;
                }
                if (HeaderValidation() == false)
                {
                    return false;
                }
                if (optCash.Checked == true)
                {
                    StrPaymmetM = "Cash";

                }
                else if (optCredit.Checked == true)
                {
                    StrPaymmetM = "Credit";

                }

                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record ? ", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return false;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();
                StrReference = GetInvNoField(myConnection, myTrans);
                UpdatePrefixNo(myConnection, myTrans);

                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(StrReference, ug.Rows.Count, intGrid + 1, StrPaymmetM, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), ug.Rows[intGrid].Cells["Categoty"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), myConnection, myTrans);

                    //--------Check Stock Item-------------

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {
                        dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);

                        if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            myTrans.Rollback();
                            return false;
                        }

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);

                    }
                    //---------------------------------------

                }
                if (optCash.Checked == true)
                {
                    exporetReceiptCash(intGrid, StrReference);
                }
                if (optCredit.Checked == true)
                {
                    ImportSalesInvoice(intGrid, StrReference);
                }
                //--End PH3 Posting--------------------
                myTrans.Commit();
                MessageBox.Show("Invoice Successfuly Saved.", "Information", MessageBoxButtons.OK);

                Print(StrReference);

                ButtonClear();
                return true;



            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                return false;
                // btnSave.Focus();
                //throw;

            }
        }
        //above code segment import invoice to Pechtree
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveEvent())
                    btnNew_Click(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
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
            catch (Exception)
            {
                return 0;
                throw;
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
            StrSql = "DELETE FROM [EST_DETAILS] WHERE AutoIndex=" + intEstomateProcode + "";

            SqlCommand command = new SqlCommand(StrSql, con, Trans);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
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
            catch (Exception)
            {

                throw;
            }


        }


        private void UpdateEstimateVsActualHeader(int intAutoIndex, double dblActualAmount, SqlConnection con, SqlTransaction Trans)
        {

            StrSql = "UPDATE EST_HEADER SET ActHed_NetAmt=ActHed_NetAmt +  " + dblActualAmount + " WHERE AutoIndex=" + intAutoIndex + " ";

            SqlCommand command = new SqlCommand(StrSql, con, Trans);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

        }






        private int GetLastTransactionNo(SqlConnection con, SqlTransaction Trans)
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


        private void InvTransaction(string strInvoiceNo, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(4,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Invoice','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "','" + dblPrice + "')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }


        }

        private void SaveDetails(string StrInvoiceNo, int intLineCount, int intLineNo, String StrPayMethod, int intLineId, String StrItemCode, string StrItemDescription, double dblQuantity, double dblPrice, double dblLineNetAmt, string StrItemClass, string StrGLAccount, string StrUOM, string StrItemType, double dblCostPrice, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblSalesInvoices(InvoiceNo,CustomerID,DeliveryNoteNos,InvoiceDate,PaymentM, " +
                         " SalesRep,ARAccount,NoofDistributions,DistributionNo, " +
                         " ItemID,Description,Qty,UnitPrice,Amount,GLAcount,UOM,Discount,DiscountAmount,GrossTotal,NetTotal, " +
                         " CurrentUser,CurrentDate,Time,ItemClass,ItemType,IsVoid,VoidReson,VoidUser,CostPrrice,RemainQty,CustomerPO,SONO,IsExport,Location,IsReturn,Tax1Rate,Tax2Rate,Tax1Amount,Tax2Amount,TTTYPE1,TTTYPE2,JobID,SubValue,Comments,Status) " +
                         " VALUES ('" + StrInvoiceNo + "','" + cmbCustomer.Value.ToString() + "','','" + GetDateTime(dtpDate.Value) + "','" + StrPayMethod + "', " +
                         " '" + cmbSalesRep.Value.ToString() + "','" + StrARAccount + "','" + intLineCount.ToString() + "','" + intLineNo.ToString() + "' " +
                         " ,'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + "," +
                         " '" + StrGLAccount + "','" + StrUOM + "'," + double.Parse(txtDiscPer.Value.ToString()) + "," + double.Parse(txtDiscAmount.Value.ToString()) + "," +
                         " " + double.Parse(txtGrossValue.Value.ToString()) + "," + double.Parse(txtNetValue.Value.ToString()) + "," +
                         " '" + user.userName.ToString().Trim() + "','" + GetDateTime(DateTime.Now) + "','" + System.DateTime.Now.ToShortTimeString().Trim() + "'," +
                         " '" + StrItemClass + "','" + StrItemType + "',0,'','" + user.userName.ToString().Trim() + "'," + dblCostPrice + "," + dblQuantity + ",'','',0,'" + cmbWarehouse.Value.ToString() + "',0," + txtNBTPer.Value.ToString() + "," + txtVatPer.Value.ToString() + "," + txtNBT.Value.ToString() + "," + txtVat.Value.ToString() + ",'NBT','VAT',''," + txtSubValue.Value.ToString() + ",'" + txtDescription.Text.Trim() + "','2')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }

        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                blnEdit = true;
                btnEdit.Enabled = true;
                btnPrint.Enabled = true;

                if (frmMain.objfrmInvoiceSearch == null || frmMain.objfrmInvoiceSearch.IsDisposed)
                {
                    frmMain.objfrmInvoiceSearch = new frmInvoiceSearch(1);
                }
                frmMain.objfrmInvoiceSearch.TopMost = false;
                frmMain.objfrmInvoiceSearch.ShowDialog();
                frmMain.objfrmInvoiceSearch.TopMost = true; 

                setValue();

            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        //private void DataSetHeader(string StrInvoiceNo)
        //{


        //    StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + StrInvoiceNo + "'";

        //    SqlCommand cmd = new SqlCommand(StrSql);
        //    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //    DataTable dt = new DataTable();
        //    da.Fill(DsEst.DtEstimateHeader);

        //}


        //private void DataSetDetails(int intEstimateNo)
        //{
        //    StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

        //    SqlCommand cmd = new SqlCommand(StrSql);
        //    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //    DataTable dt = new DataTable();
        //    da.Fill(DsEst.DtEstimateDETAILS);

        //}


        private void Print(string strInvoiceNo)
        {
            try
            {

                try
                {
                    DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                    if (reply == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (strInvoiceNo != "")
                    {
                        DsItemWise.Clear();

                        StrSql = "SELECT * FROM tblSalesInvoices WHERE InvoiceNo='" + strInvoiceNo + "'";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        DataTable dt = new DataTable();

                        da.Fill(DsItemWise.DSSalesInvoices);

                        StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsItem);

                        StrSql = "SELECT CutomerID,CustomerName,Address1,Address2 FROM tblCustomerMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsCustomer);


                        StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(DsItemWise.DsWarehouse);

                        DirectPrint();

                        //frmViewerItemWiseSales frmviewer = new frmViewerItemWiseSales(this);
                        //frmviewer.Show();

                    }

                }
                catch
                {
                    // MessageBox.Show("Error :" + ex.Message);
                }

            }
            catch { }
        }

        private void DirectPrint()
        {
            try
            {
               
                ReportDocument crp = new ReportDocument();
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptInvoices.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(DsItemWise);
                //crp.PrintToPrinter(1, true, 1, 1);
                crp.PrintToPrinter(1, true, 0, 0);
                // crp.PrintOptions.PrinterName = "Generic / Text Only On Nazeer";

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(txtInvoiceNo.Text);
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

                cmbWarehouse.Text = "";
                txtDescription.Text = "";
                dtpDate.Value = DateTime.Now;
                txtWarehouseName.Text = "";
                txtCustomer.Text = "";
                cmbCustomer.Text = "";
                cmbSalesRep.Text = "";
                txtWarehouseAddress.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtNBTPer.Value = 0;
                txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
                optSerialOne.Checked = true;


            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }


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
            //dtpDate.Enabled = blnEnable;
            txtWarehouseName.Enabled = false;
            cmbCustomer.Enabled = blnEnable;
            cmbSalesRep.Enabled = blnEnable;



        }

        private void ButtonClear()
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
            GetInvNo();
            ug.Enabled = false;
            intEstomateProcode = 0;
            optCash.Checked = true;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnNew_Click(sender, e);
            ButtonClear();

        }

        //private void DatasetWhse()
        //{
        //    StrSql = "SELECT * FROM tblWhseMaster";

        //    SqlCommand cmd = new SqlCommand(StrSql);
        //    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //    DataTable dt = new DataTable();
        //    da.Fill(DsEst.DtWhseMaster);

        //}



        private void ug_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
        }

        private double ChangePrice(string ItemCode, double dblGridQty)
        {

            double _Price = 0.00;
            string sSQL = " select isnull((select isnull((SELECT (viewItemPriceSelect.Unitprice) as Unitprice " +
                " FROM tblPriceMatrix INNER JOIN viewItemPriceSelect ON " +
                " tblPriceMatrix.ItemID = viewItemPriceSelect.ItemID AND tblPriceMatrix.PriceLevel = viewItemPriceSelect.PriceLevel " +
                " WHERE     (tblPriceMatrix.MinQty < " + dblGridQty + ") AND (tblPriceMatrix.MaxQty >= " + dblGridQty + ") AND (tblPriceMatrix.ItemID = '" + ItemCode + "')), " +
                " (SELECT     viewItemPriceSelect.Unitprice " +
                " FROM         viewItemPriceSelect INNER JOIN " +
                " viewItemDefault ON viewItemPriceSelect.PriceLevel = viewItemDefault.PriceLevel AND " +
                " viewItemPriceSelect.ItemID = viewItemDefault.ItemID " +
                " WHERE (viewItemDefault.ItemID = '" + ItemCode + "'))) as UnitPrice),0) as UnitPrice";

            SqlCommand cmd = new SqlCommand(sSQL);
            SqlDataAdapter da = new SqlDataAdapter(sSQL, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt != null)
            {
                _Price = double.Parse(dt.Rows[0]["UnitPrice"].ToString());
            }

            return _Price;

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

                    if (blnRetailsCustomer != true)
                    {

                        // e.Cell.Row.Cells["UnitPrice"].Value = ChangePrice(e.Cell.Row.Cells["ItemCode"].Value.ToString().Trim(), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value), Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble((e.Cell.Row.Cells["PriceLevel1"].Value)), Convert.ToDouble((e.Cell.Row.Cells["PriceLevel2"].Value)), Convert.ToDouble((e.Cell.Row.Cells["PriceLevel3"].Value)), Convert.ToDouble((e.Cell.Row.Cells["PriceLevel4"].Value)), Convert.ToDouble((e.Cell.Row.Cells["PriceLevel5"].Value)));
                        //   e.Cell.Row.Cells["UnitPrice"].Value = ChangePrice(e.Cell.Row.Cells["ItemCode"].Value.ToString().Trim(), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));

                    }


                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    GrandTotal();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }


        private void txtDiscPer_Leave(object sender, EventArgs e)
        {
            if (double.Parse(txtDiscPer.Value.ToString()) > 100)
            {
                MessageBox.Show("Invalid Discount Percentage", "Information", MessageBoxButtons.OK);
                txtDiscPer.Focus();

            }
        }

        private void txtDiscPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void txtNBTPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void txtVatPer_ValueChanged(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void Edit()
        {
            EnableHeader(false);
            EnableFoter(false);
           // dtpDate.Enabled = true;
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





        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {
            try
            {
                string Code;
                GetItemDataSet();
                if (ug.Rows.Count > 0)
                {
                    DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OK);

                    if (reply == DialogResult.OK)
                    {
                        DeleteRows();
                        ClearFooter();
                    }

                }
            }
            catch (Exception)
            {

            }

        }



        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
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
                            ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
                            ug.ActiveCell.Row.Cells["UOM"].Value = ugR.Cells["UOM"].Value;
                            ug.ActiveCell.Row.Cells["Categoty"].Value = ugR.Cells["Categoty"].Value;
                            ug.ActiveCell.Row.Cells["CostPrice"].Value = ugR.Cells["UnitCost"].Value;
                            ug.ActiveCell.Row.Cells["PriceLevel1"].Value = ugR.Cells["PriceLevel1"].Value;
                            ug.ActiveCell.Row.Cells["PriceLevel2"].Value = ugR.Cells["PriceLevel2"].Value;
                            ug.ActiveCell.Row.Cells["PriceLevel3"].Value = ugR.Cells["PriceLevel3"].Value;
                            ug.ActiveCell.Row.Cells["PriceLevel4"].Value = ugR.Cells["PriceLevel4"].Value;
                            ug.ActiveCell.Row.Cells["PriceLevel5"].Value = ugR.Cells["PriceLevel5"].Value;

                            if (blnRetailsCustomer == true)
                            {
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel3"].Value;
                            }
                            else
                            {
                                ug.ActiveCell.Row.Cells["UnitPrice"].Value = ugR.Cells["PriceLevel2"].Value;
                            }
                        }
                    }
                }
            }
        }


        private void cmbCustomer_Leave(object sender, EventArgs e)
        {

            try
            {
                if (StrRetailCustomer == cmbCustomer.Value.ToString())
                {
                    blnRetailsCustomer = true;
                }
                else
                {
                    blnRetailsCustomer = false;
                }
                // cmbCustomer.Enabled = false;
            }

            catch (Exception)
            {
                //MessageBox.Show("Invalid Customer", "Information", MessageBoxButtons.OK);
                //cmbCustomer.Focus();
            }

        }



        private void optSerialTwo_CheckedChanged(object sender, EventArgs e)
        {
            GetInvNo();
        }

        private void optSerialOne_CheckedChanged(object sender, EventArgs e)
        {
            GetInvNo();
        }

        private void cmbSalesRep_Leave(object sender, EventArgs e)
        {

            //try
            //{
            //    string Code;
            //    Code = cmbSalesRep.ActiveRow.Cells[1].Value.ToString();

            //    foreach (DataRow dr in dsSalesRep.Tables[0].Rows)
            //    {
            //        if (Code == dr[0].ToString())
            //        {

            //           // MessageBox.Show("Invalid Sales Rep", "Information", MessageBoxButtons.OK);
            //            //cmbSalesRep.Focus();

            //        }
            //        else
            //        {

            //        }
            //    }

            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Invalid Sales Rep", "Information", MessageBoxButtons.OK);
            //    cmbSalesRep.Focus();


            //}
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
                        txtAddress1.Text = cmbCustomer.ActiveRow.Cells[2].Value.ToString();
                        txtAddress2.Text = cmbCustomer.ActiveRow.Cells[3].Value.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }


        private void frmInvoices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                optSerialTwo.Checked = true;
                //  SaveEvent();
            }
        }

        private void frmInvoices_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //if (e.KeyCode == Keys.F12)
            //{
            //    optSerialTwo.Checked = true;
            //}
        }

        private void ug_AfterRowsDeleted(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void optCredit_CheckedChanged(object sender, EventArgs e)
        {
            //chkOverLimit.Checked = false;
            //chkOverLimit.Enabled = false;
        }

        private void optCash_CheckedChanged(object sender, EventArgs e)
        {
            //chkOverLimit.Checked = false;
            //chkOverLimit.Enabled = true;
        }






    }
}