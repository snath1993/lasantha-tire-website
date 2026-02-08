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
using Interop.PeachwServer;


namespace UserAutherization
{
    public partial class frmJobReturn : Form
    {
        public DSEstimate DsEst = new DSEstimate();
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
        public string StrGLJob;
        public DataSet dsWarehouse;
        public DataSet dsJobCode;
        public DataSet dsCostCode;
        public Boolean blnIsPhase;
        public DataSet dsPhaseCode;

        public string StrSalesGLAccount;
        public string StrARAccount;
        public string StrCustId;


        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmJobReturn(int intNo)
        {
            InitializeComponent();
            setConnectionString();

            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }


        }
        public void GetGLJob()
        {
            try
            {
                StrSql = "SELECT GLJob,Customer_ID FROM tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrGLJob = dt.Rows[0].ItemArray[0].ToString().Trim();
                    StrCustId = dt.Rows[0].ItemArray[1].ToString().Trim();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GetGLAccount(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT [ArAccount],[SalesGLAccount] FROM tblDefualtSetting";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrARAccount = dt.Rows[0].ItemArray[0].ToString().Trim();
                    StrSalesGLAccount = dt.Rows[0].ItemArray[1].ToString().Trim();

                }

            }
            catch (Exception)
            {

                throw;
            }


        }


        //public void GetJobDataSet()
        //{
        //    dsJobCode = new DataSet();
        //    try
        //    {
        //        dsJobCode.Clear();
        //        StrSql = " Select JobID,JobDescription from tblJobMaster where Job_Status>=2";

        //        SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
        //        dAdapt.Fill(dsJobCode, "dtJobCode");

        //        cmbJobCode.DataSource = dsJobCode.Tables["dtJobCode"];
        //        cmbJobCode.DisplayMember = "JobID";
        //        cmbJobCode.ValueMember = "JobID";
        //        cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobID"].Width = 75;
        //        cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobDescription"].Width = 150;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void GetJobDataSet()
        {
            dsJobCode = new DataSet();
            dsCostCode = new DataSet();
            dsPhaseCode = new DataSet();

            try
            {
                dsJobCode.Clear();
                dsPhaseCode.Clear();
                dsJobCode.Clear();

                StrSql = " Select JobID,JobDescription,IsPhase from tblJobMaster where Job_Status>=2";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsJobCode, "dtJobCode");

                cmbJobCode.DataSource = dsJobCode.Tables["dtJobCode"];
                cmbJobCode.DisplayMember = "JobID";
                cmbJobCode.ValueMember = "JobID";
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobID"].Width = 75;
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobDescription"].Width = 150;
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["IsPhase"].Hidden = true;


                StrSql = "SELECT PhaseID,PhaseDescription,HasCostCodes FROM tblPhase ";
                SqlDataAdapter daPhase = new SqlDataAdapter(StrSql, ConnectionString);
                daPhase.Fill(dsPhaseCode, "dtPhaseCode");

                cmbPhase.DataSource = dsPhaseCode.Tables["dtPhaseCode"];
                cmbPhase.DisplayMember = "PhaseID";
                cmbPhase.ValueMember = "PhaseID";
                cmbPhase.DisplayLayout.Bands["dtPhaseCode"].Columns["PhaseID"].Width = 75;
                cmbPhase.DisplayLayout.Bands["dtPhaseCode"].Columns["PhaseDescription"].Width = 150;
                cmbPhase.DisplayLayout.Bands["dtPhaseCode"].Columns["HasCostCodes"].Width = 50;

                StrSql = "SELECT CostCode,CostCodeDescription FROM tblCostCode";
                SqlDataAdapter daCost = new SqlDataAdapter(StrSql, ConnectionString);
                daCost.Fill(dsCostCode, "dtCostCode");
                cmbCostCode.DataSource = dsCostCode.Tables["dtCostCode"];
                cmbCostCode.DisplayMember = "CostCode";
                cmbCostCode.ValueMember = "CostCode";
                cmbCostCode.DisplayLayout.Bands["dtCostCode"].Columns["CostCode"].Width = 75;
                cmbCostCode.DisplayLayout.Bands["dtCostCode"].Columns["CostCodeDescription"].Width = 150;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void setValue()
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            int intNo = int.Parse(Search.searchIssueNoteNo);

            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }
            else
            {
                ClearHeader();
                DeleteRows();
                intEstomateProcode = intNo;

                EnableHeader(true);
                EnableFoter(true);
                ViewHeader(intEstomateProcode, myConnection, myTrans);
                ViewDetails(intEstomateProcode, myConnection, myTrans);
                EnableHeader(false);
                EnableFoter(false);

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
                StrSql = "SELECT JobRetPref, JobRetPad, JobRetNum FROM tblDefualtSetting";
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

        public void UpdatePrefixNo(SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intInvNo;

                SqlCommand command;

                StrSql = "SELECT  TOP 1(JobRetNum) FROM tblDefualtSetting ORDER BY JobActNum DESC";
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

                StrSql = "UPDATE tblDefualtSetting SET JobRetNum='" + intInvNo + "'";
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

                StrSql = "SELECT JobRetPref, JobRetPad, JobRetNum FROM tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrInvNo = dt.Rows[0].ItemArray[0].ToString().Trim();
                    intX = Int32.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    intZ = Int32.Parse(dt.Rows[0].ItemArray[2].ToString().Trim()) + 1;

                    intP = 1;
                    for (intI = 1; intI <= intX; intI++)
                    {
                        intP = intP * 10;
                    }

                    intP = intP + intZ;
                    StrInV = intP.ToString();

                    txtJobId.Text = StrInvNo + StrInV.Substring(1, intX);

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

                StrSql = "SELECT JobRetPref, JobRetPad, JobRetNum FROM tblDefualtSetting";
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

                throw;
                return null;
            }

        }


        //public void GetWareHouseDataSet()
        //{
        //    StrSql = "Select WhseId,WhseName from tblWhseMaster";
        //    SqlCommand cmd = new SqlCommand(StrSql);
        //    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);
        //    if (dt.Rows.Count > 0)
        //    {
        //        cmbWarehouse.Items.Clear();
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            cmbWarehouse.Items.Add(dt.Rows[i].ItemArray[0].ToString().Trim());
        //        }
        //    }
        //}

        public void GetJobStatus()
        {
            cmbJobStatus.Items.Clear();
            cmbJobStatus.Items.Add("Quote");
            cmbJobStatus.Items.Add("Active");
            cmbJobStatus.Items.Add("Complete");
            cmbJobStatus.Items.Add("Finished");

            cmbJobStatus.SelectedIndex = 2;
        }

        public void GetItemDataSet()
        {
            try
            {
                StrSql = "SELECT ItemID,ItemDescription,UnitCost,0,ItemClass,SalesGLAccount FROM tblItemMaster";

                //StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitCost, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";

                //StrSql = StrSql + "UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitCost,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ultraCombo1.DataSource = dt;
                    ultraCombo1.ValueMember = "ItemID";
                    ultraCombo1.DisplayMember = "ItemID";

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

                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private void DeleteRows()
        {
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                ugR.Delete(false);
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

        private double LineCalculation(double UnitCost, double Quantity)
        {
            dblLineTot = 0;
            double lineTotal = 0;
            dblLineTot = UnitCost * Quantity;
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
            }
        }

        public Boolean IsGridValidation()
        {

            if (ug.Rows.Count == 0)
            {

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



            return true;
        }

        public Boolean HeaderValidation()
        {
            if (cmbJobCode.Value == null)
            {
                return false;

            }

            if (cmbJobStatus.Text.Trim() == "")
            {
                return false;
            }

            return true;
        }

        public void ViewDetails(int intAutoIndex, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "SELECT EST_DETAILS.EstDet_LineId,EST_DETAILS.Item_ID,EST_DETAILS.Item_Description,EST_DETAILS.EstDet_Return_Qty,EST_DETAILS.EstDet_Return_Price,EST_DETAILS.EstDet_Return_NetAmt,EST_DETAILS.ItemClass,EST_DETAILS.SalesGLAccount,EST_DETAILS.WarehouseCode,EST_DETAILS.IsPhaseID,EST_DETAILS.PhaseID,EST_DETAILS.IsCostCodeID,EST_DETAILS.CostCodeID FROM EST_DETAILS INNER JOIN EST_HEADER ON EST_HEADER.AutoIndex=EST_DETAILS.AutoIndex  WHERE EST_DETAILS.AutoIndex=" + intAutoIndex + " ";

                //StrSql = "SELECT EST_DETAILS.EstDet_LineId,EST_DETAILS.Item_ID,EST_DETAILS.Item_Description,EST_DETAILS.EstDet_Return_Qty,EST_DETAILS.EstDet_Return_Price,EST_DETAILS.EstDet_Return_NetAmt,EST_DETAILS.ItemClass,EST_DETAILS.SalesGLAccount,tblItemWhse.QTY FROM EST_DETAILS INNER JOIN EST_HEADER ON EST_HEADER.AutoIndex=EST_DETAILS.AutoIndex INNER JOIN tblItemWhse ON tblItemWhse.ItemID=EST_DETAILS.Item_ID AND tblItemWhse.WhseId=EST_HEADER.WarehouseCode  WHERE EST_DETAILS.AutoIndex=" + intAutoIndex + " ";

                //StrSql = StrSql + " UNION SELECT EST_DETAILS.EstDet_LineId,EST_DETAILS.Item_ID,EST_DETAILS.Item_Description,EST_DETAILS.EstDet_Return_Qty,EST_DETAILS.EstDet_Return_Price,EST_DETAILS.EstDet_Return_NetAmt,EST_DETAILS.ItemClass,EST_DETAILS.SalesGLAccount,0 FROM EST_DETAILS INNER JOIN EST_HEADER ON EST_HEADER.AutoIndex=EST_DETAILS.AutoIndex WHERE EST_DETAILS.AutoIndex=" + intAutoIndex + " AND EST_DETAILS.ItemClass IN (5) ORDER BY EST_DETAILS.EstDet_LineId";


                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                int intItemClass;
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    UltraGridRow ugR;
                    foreach (DataRow Dr in dt.Rows)
                    {
                        ugR = ug.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells["LineNo"].Value = Dr["EstDet_LineId"];
                        ugR.Cells["ItemCode"].Value = Dr["Item_ID"];
                        ugR.Cells["Description"].Value = Dr["Item_Description"];
                        ugR.Cells["UnitCost"].Value = Dr["EstDet_Return_Price"];
                        ugR.Cells["Quantity"].Value = Dr["EstDet_Return_Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["EstDet_Return_NetAmt"];

                        //ugR.Cells["OnHand"].Value = Dr["QTY"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["SalesGLAccount"];
                        ugR.Cells["WH"].Value = Dr["WarehouseCode"];

                        ugR.Cells["IsPhaseID"].Value = (Dr["IsPhaseID"]);
                        ugR.Cells["PhaseID"].Value = Dr["PhaseID"];
                        ugR.Cells["IsCostCodeID"].Value = (Dr["IsCostCodeID"]);
                        ugR.Cells["CostCodeID"].Value = Dr["CostCodeID"];


                        intItemClass = int.Parse(ugR.Cells["ItemClass"].Value.ToString());

                        if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                        {
                            ugR.Cells["OnHand"].Value = CheckWarehouseItem(ugR.Cells["ItemCode"].Value.ToString(), ugR.Cells["WH"].Value.ToString(), con, Trans);
                        }
                        else
                        {
                            ugR.Cells["OnHand"].Value = 0;
                        }



                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }

        public void ViewHeader(int intAutoIndex, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "SELECT [EstimateNo],[EstDate],[Description],[WarehouseCode],[JobCode],[Job_Status],EstHed_NetDiscPer,EstHed_NetDiscAmt,EstHed_NBTPer,EstHed_NBTAmt,EstHed_VATPer,EstHed_VATAmt,EstHed_SubAmt,EstHed_GrossAmt,EstHed_NetAmt FROM EST_HEADER WHERE AutoIndex=" + intAutoIndex + "";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {


                    txtJobId.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0].ItemArray[1].ToString().Trim());
                    txtDescription.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                    //cmbWarehouse.Text = dt.Rows[0].ItemArray[3].ToString().Trim();
                    cmbJobCode.Value = dt.Rows[0].ItemArray[4].ToString().Trim();
                    cmbJobStatus.SelectedIndex = int.Parse(dt.Rows[0].ItemArray[5].ToString().Trim());



                    txtDiscPer.Value = double.Parse(dt.Rows[0].ItemArray[6].ToString().Trim());
                    txtDiscAmount.Value = double.Parse(dt.Rows[0].ItemArray[7].ToString().Trim());
                    txtNBTPer.Value = double.Parse(dt.Rows[0].ItemArray[8].ToString().Trim());
                    txtNBT.Value = double.Parse(dt.Rows[0].ItemArray[9].ToString().Trim());
                    txtVatPer.Value = double.Parse(dt.Rows[0].ItemArray[10].ToString().Trim());

                    txtSubValue.Value = double.Parse(dt.Rows[0].ItemArray[11].ToString().Trim());
                    txtGrossValue.Value = double.Parse(dt.Rows[0].ItemArray[12].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0].ItemArray[13].ToString().Trim());

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
                throw;
            }
        }



        //private void Filljob()
        //{
        //    String S11 = "Select JobID from tblJobMaster where Job_Status>=2";
        //    SqlCommand cmd11 = new SqlCommand(S11);
        //    SqlDataAdapter da11 = new SqlDataAdapter(S11, ConnectionString);
        //    DataTable dt11 = new DataTable();
        //    da11.Fill(dt11);
        //    if (dt11.Rows.Count > 0)
        //    {
        //        cmbjob.Items.Clear();
        //        for (int i = 0; i < dt11.Rows.Count; i++)
        //        {
        //            cmbjob.Items.Add(dt11.Rows[i].ItemArray[0].ToString().Trim());
        //        }
        //    }
        //}


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



        private void btnclose_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {

                if (add)
                {
                    //Connector conn = new Connector();
                    //conn.ImportItem_List();
                    //conn.fillID_Item_list();

                    btnSave.Enabled = true;
                    btnNew.Enabled = false;
                    btnPrint.Enabled = false;
                    btnSNO.Enabled = false;
                    btnSearch.Enabled = false;
                    btnReset.Enabled = true;
                    btnEdit.Enabled = false;
                    EnableHeader(true);
                    EnableFoter(true);

                    //Connector conn = new Connector();
                    //conn.ImportItem_List();
                    //conn.fillID_Item_list();


                    ClearHeader();
                    DeleteRows();
                    GetInvNo();
                    cmbJobCode.Focus();

                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {

                throw;
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
            catch (Exception)
            {

                throw;
            }


        }




        //peahtree update=========================
        private void CreateXmlToExportInvAdjust(string StrItemCode, string strIssueNoteNo, DateTime dtDate, string StrJobId, double dblUnitcost, double dblQty, double dblLineTotal, string StrGLCode, Int32 intPhaseCode, string StrPhaseCode, Int32 intCostCode, string StrCostCode, SqlConnection con, SqlTransaction Trans)
        {

            Connector Conn = new Connector();
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueAdjustment.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("PAW_Items");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");

            Writer.WriteStartElement("PAW_Item");
            Writer.WriteAttributeString("xsi:type", "paw:item");

            //crate a ID element (tag)=====================(1)
            Writer.WriteStartElement("ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrItemCode);//dgvItem[0, c].Value
            Writer.WriteEndElement();

            //this sis crating tag for reference============(2)
            Writer.WriteStartElement("Reference");
            Writer.WriteString(strIssueNoteNo);
            Writer.WriteEndElement();

            //This is a Tag for Adjusment Date==============(3)
            Writer.WriteStartElement("Date ");
            Writer.WriteAttributeString("xsi:type", "paw:date");
            Writer.WriteString(GetDateTime(dtDate));//Date format must be (MM/dd/yyyy)
            Writer.WriteEndElement();


            if (intPhaseCode == 1)
            {
                StrJobId += "," + StrPhaseCode;
            }

            if (intCostCode == 1)
            {
                StrJobId += "," + StrCostCode;
            }

            Writer.WriteStartElement("JobID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrJobId);
            Writer.WriteEndElement();

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
            Writer.WriteString(StrGLJob);
            Writer.WriteEndElement();

            Writer.WriteStartElement("UnitCost");
            Writer.WriteString(dblUnitcost.ToString());
            Writer.WriteEndElement();

            Writer.WriteStartElement("Quantity");
            Writer.WriteString(dblQty.ToString());
            Writer.WriteEndElement();


            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + dblLineTotal.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteEndElement();//Adjustment Line
            Writer.WriteEndElement();//Adjustment lines

            Writer.WriteEndElement();//Item Closed Tag
            Writer.Close();//finishing writing xml file

            Conn.IssueAdjustmentExport("IssueAdjustment.xml");
        }
        //============================================

        private int CheckJobStatus(string strJobCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT Job_Status FROM tblJobMaster WHERE JOBID='" + strJobCode + "'";

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
            catch (Exception)
            {

                throw;
                return 0;
            }
        }

        private Boolean IsGridExitPhaseId(string StrPhase, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "Select PhaseID from tblPhase where PhaseID='" + StrPhase + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }

            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        private Boolean IsGridExitCostId(string StrCostCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "Select CostCode from tblCostCode where CostCode='" + StrCostCode + "' ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            int intGrid;
            int intAutoIndex;
            int intPreAutoIndex;
            int intProcess;
            double dblAvailableQty;
            string StrReference = null;

            int intItemClass;

            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

            try
            {


                DialogResult reply = MessageBox.Show("Are you sure, you want to Save this record?", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }

                reply = MessageBox.Show("Are You Sure, You Want To Process This?", "Information", MessageBoxButtons.YesNo);

                if (reply == DialogResult.Yes)
                {
                    intProcess = 1;
                }
                else
                {
                    intProcess = 0;
                }

                DeleteEmpGrid();

                if (IsGridValidation() == false)
                {
                    MessageBox.Show("No Data To Save", "Information", MessageBoxButtons.YesNo);
                    return;

                }
                if (HeaderValidation() == false)
                {
                    MessageBox.Show("Incomplete Transaction", "Information", MessageBoxButtons.YesNo);
                    return;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                if (CheckJobStatus(cmbJobCode.Value.ToString(), myConnection, myTrans) == 3)
                {
                    MessageBox.Show("This Project is Already Closed.", "Message", MessageBoxButtons.OK);
                    return;
                }

                intPreAutoIndex = GetEstimateCode(cmbJobCode.Value.ToString(), myConnection, myTrans);
                StrReference = GetInvNoField(myConnection, myTrans);
                GetGLAccount(myConnection, myTrans);

                if (intEstomateProcode != 0)
                {
                    DeleteDetails(intEstomateProcode, myConnection, myTrans);
                    intAutoIndex = intEstomateProcode;
                    ModifyHeader(intAutoIndex, intProcess, myConnection, myTrans);

                }
                else
                {
                    intAutoIndex = SaveHeader(intProcess, myConnection, myTrans);
                    //intAutoIndex = GetLastTransactionNo(myConnection, myTrans);

                }

                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {

                    if (blnIsPhase == true)
                    {
                        if (ug.Rows[intGrid].Cells["PhaseID"].Value.ToString() == string.Empty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Please Enter Phase Code", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }
                        if (IsGridExitPhaseId(ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), myConnection, myTrans) == false)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Invalid Phase Code", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }

                    }

                    if (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true)
                    {
                        if (ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString() == string.Empty)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Please Enter Cost Code", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;
                        }
                        if (IsGridExitCostId(ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), myConnection, myTrans) == false)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Invalid Cost Code.", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;

                        }
                    }


                    SaveDetails(intAutoIndex, ug.Rows[intGrid].Cells["WH"].Value.ToString(), Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsPhaseID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), myConnection, myTrans);

                    if (intProcess == 1)
                    {

                        //--------Check Stock Item-------------

                        intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());
                        UpdateEstimateVsActualDetails(intPreAutoIndex, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), (Boolean.Parse(ug.Rows[intGrid].Cells["IsPhaseID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), intItemClass, myConnection, myTrans);



                        if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                        {
                            if (IsWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans) == false)
                            {
                                MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Invalid Item Or Warehouse", "Message", MessageBoxButtons.OK);
                                myTrans.Rollback();
                                return;

                            }

                            dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans);

                            UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), myConnection, myTrans);

                            InvTransaction(intAutoIndex, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);


                        }

                        else if ((intItemClass == 5) || (intItemClass == 4))
                        {
                            InvTransaction(intAutoIndex, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                        }

                        //---------------------------------------



                    }


                }

                if (intEstomateProcode == 0)
                {
                    UpdatePrefixNo(myConnection, myTrans);
                    UpdateInvoiceNo(intAutoIndex, myConnection, myTrans);
                }


                //--PH3 Posting--------------------------------------------------------------------------------------------------------------------

                int intService = 0;

                if (intProcess == 1)
                {
                    UpdateEstimateVsActualHeader(intPreAutoIndex, double.Parse(txtNetValue.Value.ToString()), myConnection, myTrans);

                    for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                    {
                        intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                        if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                        {
                            CreateXmlToExportInvAdjust(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), StrReference, dtpDate.Value, cmbJobCode.Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsPhaseID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), myConnection, myTrans);
                        }
                        else if ((intItemClass == 5) || (intItemClass == 4))
                        {

                            intService++;
                        }
                    }

                    if (intService > 0)
                    {

                        ImportSalesInvoice(intGrid, StrReference, intService);
                    }
                }


                //-------------------------------------------------------------------------------------------------------------------------------------


                myTrans.Commit();
                MessageBox.Show("Job Issue Successfuly Saved.", "Information", MessageBoxButtons.OK);

                Print(intAutoIndex);

                ButtonClear();

            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);


            }
        }


        private void ImportSalesInvoice(int intGrid, string StrReference, int intRowCount)
        {

            int intItemClass;
            string StrJob = string.Empty;

            DateTime DTP = Convert.ToDateTime(dtpDate.Text);
            string Dformat = "MM/dd/yyyy";
            string InvDate = DTP.ToString(Dformat);

            XmlTextWriter Writer = new XmlTextWriter(@"c:\\PBSS\\SalesInvice.xml", System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartElement("PAW_Invoices");
            Writer.WriteAttributeString("xmlns:paw", "urn:schemas-peachtree-com/paw8.02-datatypes");
            Writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2000/10/XMLSchema-instance");
            Writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2000/10/XMLSchema-datatypes");
            // --PH3 Posting--------------------


            Writer.WriteStartElement("PAW_Invoice");
            Writer.WriteAttributeString("xsi:type", "paw:Receipt");

            Writer.WriteStartElement("Customer_ID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrCustId);//Customer ID should be here = Ptient No
            Writer.WriteEndElement();

            Writer.WriteStartElement("Date");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(InvDate);//Date 
            Writer.WriteEndElement();

            Writer.WriteStartElement("Invoice_Number");
            Writer.WriteString(StrReference);
            Writer.WriteEndElement();

            //Writer.WriteStartElement("Sales_Representative_ID");
            //Writer.WriteAttributeString("xsi:type", "paw:id");
            //Writer.WriteString(cmbSalesRep.Value.ToString().Trim());
            //Writer.WriteEndElement();

            Writer.WriteStartElement("Accounts_Receivable_Account");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString(StrARAccount);//Cash Account
            //Writer.WriteString("70000-00");
            Writer.WriteEndElement();//CreditMemoType

            Writer.WriteStartElement("CreditMemoType");
            Writer.WriteString("FALSE");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Number_of_Distributions");
            Writer.WriteString(intRowCount.ToString());
            Writer.WriteEndElement();

            for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
            {
                if (intGrid < ug.Rows.Count)
                {
                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 5) || (intItemClass == 4))
                    {

                        StrJob = string.Empty;
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
                        //Writer.WriteString("65201-00");
                        Writer.WriteString(StrSalesGLAccount);
                        //StrSalesGLAccount
                        Writer.WriteEndElement();
                        //========================================================
                        Writer.WriteStartElement("Tax_Type");
                        Writer.WriteString("1");//Doctor Charge
                        Writer.WriteEndElement();

                        Writer.WriteStartElement("Amount");
                        Writer.WriteString("-" + 0);//HospitalCharge
                        Writer.WriteEndElement();

                        StrJob = cmbJobCode.Value.ToString().Trim();

                        if (Boolean.Parse(ug.Rows[intGrid].Cells["IsPhaseID"].Value.ToString()) == true)
                        {
                            StrJob += "," + ug.Rows[intGrid].Cells["PhaseID"].Value.ToString().Trim();
                        }

                        if (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true)
                        {
                            StrJob += "," + ug.Rows[intGrid].Cells["PhaseID"].Value.ToString().Trim();
                        }

                        Writer.WriteStartElement("Job_ID");
                        Writer.WriteString(StrJob);
                        Writer.WriteEndElement();


                        Writer.WriteEndElement();//LINE
                        Writer.WriteEndElement();//LINES


                    }


                }
            }
            Writer.WriteEndElement();
            Writer.Close();

            Connector ObjImportP = new Connector();
            ObjImportP.ImportDirectSalesInvice();

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
                throw;
                return 0;

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
            catch (Exception)
            {

                throw;
            }

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
            catch (Exception)
            {

                throw;
            }


        }


        private void UpdateEstimateVsActualHeader(int intAutoIndex, double dblActualAmount, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE EST_HEADER SET RetHed_NetAmt=RetHed_NetAmt +  " + dblActualAmount + " WHERE AutoIndex=" + intAutoIndex + " ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }


        }



        private void ModifyHeader(int intEstomateProcode, int intProcess, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE [EST_HEADER] SET [DocType] =3,[EstDate] ='" + GetDateTime(dtpDate.Value) + "',[Description] = '" + txtDescription.Text.Trim() + "',[JobCode] = '" + cmbJobCode.Value.ToString() + "', " +
                " [Job_Status]=" + cmbJobStatus.SelectedIndex + ",[EstHed_SubAmt] =" + txtSubValue.Value + ",[EstHed_NetDiscPer]=" + txtDiscPer.Value + ",[EstHed_NetDiscAmt] =" + txtDiscAmount.Value + ",[EstHed_GrossAmt] =" + txtGrossValue.Value + ", " +
                " [EstHed_NBTPer] =" + txtNBTPer.Value + ",[EstHed_NBTAmt] =" + txtNBT.Value + ",[EstHed_VATPer] =" + txtNBTPer.Value + ",[EstHed_VATAmt] =" + txtVat.Value + ",[EstHed_NetAmt]=" + txtNetValue.Value + ",EstHed_Process=" + intProcess + " WHERE AutoIndex=" + intEstomateProcode + "";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }

        }
        private int SaveHeader(int intProcess, SqlConnection con, SqlTransaction Trans)
        {
            int index = 0;
            try
            {

                StrSql = "INSERT INTO [EST_HEADER]([DocType],[EstimateNo],[EstDate],[Description] " +
                        " ,[JobCode],[Job_Status],[EstHed_SubAmt],[EstHed_NetDiscPer],[EstHed_NetDiscAmt],[EstHed_GrossAmt] " +
                        ",[EstHed_NBTPer],[EstHed_NBTAmt],[EstHed_VATPer],[EstHed_VATAmt],[EstHed_NetAmt] " +
                        ",[EstHed_Process],[EstHed_UserCreated],[EstHed_DateCreated],[EstHed_UserModified],[EstHed_DateModified]) " +
                        " VALUES(3,'" + txtJobId.Text.Trim() + "','" + GetDateTime(dtpDate.Value) + "','" + txtDescription.Text.Trim() + "', " +
                        " '" + cmbJobCode.Value.ToString() + "'," + cmbJobStatus.SelectedIndex + "," + txtSubValue.Value + "," + txtDiscPer.Value + "," + txtDiscAmount.Value + "," + txtGrossValue.Value + ", " +
                        " " + txtNBTPer.Value + "," + txtNBT.Value + "," + txtVatPer.Value + "," + txtVat.Value + "," + txtNetValue.Value + ", " +
                        " " + intProcess + ",'" + user.userName + "','" + GetDateTime(DateTime.Now) + "','" + user.userName + "','" + GetDateTime(DateTime.Now) + "')" +
                        " select @@Identity ";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                index =int.Parse(command.ExecuteScalar().ToString());

            }
            catch (Exception)
            {

                throw;
            }
            return index;
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
            catch (Exception)
            {
                return 1;
                throw;
            }


        }

        private void UpdateEstimateVsActualDetails(int intPreAutoIndex, string StrItemCode, string StrWarehouseCode, string StrItemDescription, double dblQty, double dblPrice, double dblLineTotal, Int32 intPhaseCode, string StrPhaseCode, Int32 intCostCode, string StrCostCode, int intItemClass, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                int intLineNo;
                StrSql = "SELECT Item_ID FROM EST_DETAILS WHERE AutoIndex=" + intPreAutoIndex + " AND Item_ID='" + StrItemCode + "' AND PhaseID='" + StrPhaseCode + "' AND CostCodeID='" + StrCostCode + "' AND WarehouseCode='" + StrWarehouseCode + "'";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    StrSql = "UPDATE [EST_DETAILS] SET [EstDet_Return_Qty]=[EstDet_Return_Qty]+" + dblQty + ",EstDet_Return_Price=" + dblPrice + ",EstDet_Return_NetAmt=EstDet_Return_NetAmt + " + dblLineTotal + " WHERE AutoIndex=" + intPreAutoIndex + " AND Item_ID='" + StrItemCode + "' AND PhaseID='" + StrPhaseCode + "' AND CostCodeID='" + StrCostCode + "' AND WarehouseCode='" + StrWarehouseCode + "'";

                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                else
                {

                    intLineNo = 0;

                    StrSql = "SELECT MAX(isnull(EstDet_LineId,0)) AS EstDet_LineId FROM EST_DETAILS WHERE AutoIndex=" + intPreAutoIndex + "";

                    command = new SqlCommand(StrSql, con, Trans);
                    SqlDataAdapter da1 = new SqlDataAdapter(command);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);

                    if (dt1.Rows.Count > 0)
                    {
                        intLineNo = int.Parse(dt1.Rows[0].ItemArray[0].ToString()) + 1;
                    }
                    else
                    {
                        intLineNo = 1;
                    }


                    StrSql = "INSERT INTO [EST_DETAILS] (AutoIndex,EstDet_LineId,Item_ID,WarehouseCode,Item_Description,EstDet_Return_Qty,EstDet_Return_Price,EstDet_Return_NetAmt,Original_Doc,IsPhaseID,PhaseID,IsCostCodeID,CostCodeID,ItemClass) VALUES (" + intPreAutoIndex + "," + intLineNo + ",'" + StrItemCode + "','" + StrWarehouseCode + "','" + StrItemDescription + "'," + dblQty + "," + dblPrice + "," + dblLineTotal + ",0," + intPhaseCode + ",'" + StrPhaseCode + "'," + intCostCode + ",'" + StrCostCode + "'," + intItemClass + ")";

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

        private void InvTransaction(int intAutoIndex, DateTime dtDate, String StrItemCode, string StrLocCode, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(13," + intAutoIndex + ",'" + GetDateTime(dtDate) + "','JOBRE',1,'" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        private void SaveDetails(int intAutoIndex, string StrWHCode, int intLineId, String StrItemCode, string StrItemDescription, double dblQuantity, double dblPrice, double dblLineNetAmt, string StrItemClass, string StrGLAccount, Int32 intPhaseCode, string StrPhaseCode, Int32 intCostCode, string StrCostCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "INSERT INTO [EST_DETAILS]([AutoIndex],[EstDet_LineId],[Item_ID],[Item_Description],[EstDet_Return_Qty],[EstDet_Return_Price],[EstDet_Return_NetAmt],[ItemClass],[SalesGLAccount],[WarehouseCode],IsPhaseID,PhaseID,IsCostCodeID,CostCodeID) " +
                   " VALUES(" + intAutoIndex + "," + intLineId + ",'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrItemClass + "','" + StrGLAccount + "','" + StrWHCode + "'," + intPhaseCode + ",'" + StrPhaseCode + "'," + intCostCode + ",'" + StrCostCode + "')";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
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
                frmJobReturnSearch issueSearch = new frmJobReturnSearch(3);
                issueSearch.ShowDialog();

                setValue();

            }
            catch { }
        }


        private void DataSetHeader(int intEstimateNo)
        {


            StrSql = "SELECT * FROM EST_HEADER WHERE AutoIndex=" + intEstimateNo + "";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtEstimateHeader);

        }


        private void DataSetDetails(int intEstimateNo)
        {
            StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtEstimateDETAILS);

        }


        private void Print(int intAutoIndex)
        {
            try
            {

                try
                {
                    if (intAutoIndex == 0)
                    {
                        return;
                    }

                    DialogResult reply = MessageBox.Show("Are You Sure, You Want To Print This?", "Print", MessageBoxButtons.OKCancel);

                    if (reply == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (intAutoIndex > 0)
                    {
                        DsEst.Clear();
                        DataSetHeader(intAutoIndex);
                        DataSetDetails(intAutoIndex);
                        DatasetWhse();
                        DataGetItem();
                        DataGetPhase();
                        DataGetCostCode();
                        DatasetJob();

                        frmViewerJobReturn frmviewer = new frmViewerJobReturn(this);
                        frmviewer.Show();

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error :" + ex.Message);
                }

            }
            catch { }
        }
        private void DatasetJob()
        {
            StrSql = "SELECT * FROM tblJobMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtJob);

        }
        private void DataGetItem()
        {
            StrSql = "SELECT ItemID,UOM FROM tblItemMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtItem);

        }

        private void DataGetPhase()
        {
            StrSql = "SELECT PhaseID,PhaseDescription FROM tblPhase";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtPhase);

        }

        private void DataGetCostCode()
        {
            StrSql = "SELECT CostCode,CostCodeDescription FROM tblCostCode";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtCostCode);

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(intEstomateProcode);
        }



        private void ClearHeader()
        {
            try
            {


                txtDescription.Text = "";
                dtpDate.Value = user.LoginDate;
                cmbJobCode.Text = "";
                txtNBTPer.Value = 0;
                txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
                cmbJobStatus.SelectedItem = 0;
                blnIsPhase = false;

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

            dtpDate.Enabled = blnEnable;
            cmbJobCode.Enabled = blnEnable;



        }

        private void ButtonClear()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            //btnSNO.Enabled = false;
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

        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            ButtonClear();

        }

        private void DatasetWhse()
        {
            StrSql = "SELECT * FROM tblWhseMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtWhseMaster);

        }



        private void ug_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
        }

        private void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "UnitCost" || e.Cell.Column.Key == "Quantity")
            {
                e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitCost"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                GrandTotal();
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





        private int JobProcessed()
        {
            StrSql = "Select EstHed_Process from EST_HEADER where AutoIndex=" + intEstomateProcode + "";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {

                return (Boolean.Parse(dt.Rows[0].ItemArray[0].ToString()) == true ? 1 : 0);
            }
            else
            {
                return 0;

            }

        }

        private int ProjectStatus()
        {
            StrSql = "Select Job_Status from EST_HEADER where AutoIndex=" + intEstomateProcode + "";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
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

        private void Edit()
        {
            EnableHeader(false);
            EnableFoter(false);
            dtpDate.Enabled = true;
            //cmbJobStatus.Enabled = true;
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



        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (JobProcessed() == 0)
            {
                GetItemDataSet();
                Edit();

            }
            else
            {
                MessageBox.Show("This Transaction Already Processed. You can not modify this record.", "Information", MessageBoxButtons.OK);
            }
        }

        private void cmbWarehouse_Leave(object sender, EventArgs e)
        {


            if (ug.Rows.Count > 0)
            {
                DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OKCancel);

                if (reply == DialogResult.Cancel)
                {
                    return;
                }
                {
                    DeleteRows();
                }
            }

            GetItemDataSet();

        }



        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            try
            {
                int intItemClass = 0;
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
                                ug.ActiveCell.Row.Cells["Description"].Value = ugR.Cells["ItemDescription"].Value;
                                ug.ActiveCell.Row.Cells["UnitCost"].Value = ugR.Cells["UnitCost"].Value;
                                //ug.ActiveCell.Row.Cells["OnHand"].Value = ugR.Cells["QTY"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ugR.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ugR.Cells["SalesGLAccount"].Value;
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;

                                ug.ActiveCell.Row.Cells["IsPhaseID"].Value = blnIsPhase;

                                if (blnIsPhase == true)
                                {
                                    ug.ActiveCell.Row.Cells["PhaseID"].Column.CellActivation = Activation.AllowEdit;
                                    ug.ActiveCell.Row.Cells["CostCodeID"].Column.CellActivation = Activation.AllowEdit;
                                }
                                else
                                {
                                    ug.ActiveCell.Row.Cells["PhaseID"].Column.CellActivation = Activation.Disabled;
                                    ug.ActiveCell.Row.Cells["CostCodeID"].Column.CellActivation = Activation.Disabled;
                                }


                            }
                        }
                    }

                }

                else if (ug.ActiveCell.Column.Key == "WH")
                {
                    intItemClass = int.Parse(ug.ActiveCell.Row.Cells["ItemClass"].Value.ToString());

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {

                        if (ug.ActiveCell.Text == "")
                        {
                            e.Cancel = true;
                            return;
                        }
                        if (ug.ActiveCell.Value.ToString() != "")
                        {
                            ug.ActiveCell.Value = "";
                            e.Cancel = true;
                            return;
                        }

                        if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                        {
                            return;

                        }

                        ug.ActiveCell.Value = ug.ActiveCell.Text;

                        if (IsGridExitWHCode(ug.ActiveCell.Value.ToString()) == false)
                        {
                            e.Cancel = true;

                        }
                        if (IsGridExitWHItem(ug.ActiveCell.Value.ToString(), ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString()) == false)
                        {
                            DialogResult reply = MessageBox.Show("Invalid Warehouse.Do You Want To Delete This Record", "Information", MessageBoxButtons.YesNo);

                            if (reply == DialogResult.Yes)
                            {
                                ug.PerformAction(UltraGridAction.CommitRow);
                                ug.ActiveRow.Delete(false);

                            }
                            else
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                        else
                        {
                            ug.ActiveCell.Value = ug.ActiveCell.Text;

                            foreach (UltraGridRow ugR in cmbWH.Rows)
                            {

                                if (ug.ActiveCell.Value.ToString() == ugR.Cells["WhseId"].Value.ToString())
                                {

                                    ug.ActiveCell.Row.Cells["OnHand"].Value = DblGridExitWHItem(ug.ActiveCell.Row.Cells["WH"].Value.ToString(), ug.ActiveCell.Row.Cells["ItemCode"].Value.ToString());

                                }
                            }
                        }


                    }


                }
                else if (ug.ActiveCell.Column.Key == "PhaseID")
                {

                    if (ug.ActiveCell.Text == "")
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (ug.ActiveCell.Value.ToString() != "")
                    {
                        ug.ActiveCell.Value = "";
                        e.Cancel = true;
                        return;
                    }

                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        return;

                    }

                    ug.ActiveCell.Value = ug.ActiveCell.Text;

                    if (IsGridExitPhaseCode(ug.ActiveCell.Value.ToString()) == false)
                    {
                        e.Cancel = true;

                    }

                    else
                    {
                        if (IsGridExitCostCodeId(ug.ActiveCell.Value.ToString()) == true)
                        {
                            ug.ActiveCell.Row.Cells["IsCostCodeID"].Value = true;
                            ug.ActiveCell.Row.Cells["CostCodeID"].Column.CellActivation = Activation.AllowEdit;
                            ug.ActiveCell.Row.Cells["CostCodeID"].Selected = true;
                            ug.ActiveCell.Row.Cells["CostCodeID"].Activated = true;


                        }
                        else
                        {
                            ug.ActiveCell.Row.Cells["IsCostCodeID"].Value = false;
                            ug.ActiveCell.Row.Cells["CostCodeID"].Column.CellActivation = Activation.Disabled;
                        }
                    }



                }


                else if (ug.ActiveCell.Column.Key == "CostCodeID")
                {

                    if (ug.ActiveCell.Text == "")
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (ug.ActiveCell.Value.ToString() != "")
                    {
                        ug.ActiveCell.Value = "";
                        e.Cancel = true;
                        return;
                    }

                    if (ug.ActiveCell.Value.ToString() == ug.ActiveCell.Text)
                    {
                        e.Cancel = true;
                        return;

                    }

                    ug.ActiveCell.Value = ug.ActiveCell.Text;

                    if (IsGridExitCostCode(ug.ActiveCell.Value.ToString()) == false)
                    {
                        e.Cancel = true;
                        return;

                    }
                }


            }


            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }

        }

        private Boolean IsGridExitCostCodeId(string StrPhase)
        {
            try
            {
                StrSql = "Select HasCostCodes from tblPhase where PhaseID='" + StrPhase + "' ";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    if (Boolean.Parse(dt.Rows[0].ItemArray[0].ToString()) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;

                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        public Boolean IsGridExitPhaseCode(String StrCode)
        {
            foreach (UltraGridRow ugR in cmbPhase.Rows)
            {
                if (ugR.Cells["PhaseID"].Text == StrCode)
                {

                    return true;
                }
            }
            return false;

        }
        public Boolean IsGridExitCostCode(String StrCode)
        {
            foreach (UltraGridRow ugR in cmbCostCode.Rows)
            {
                if (ugR.Cells["CostCode"].Text == StrCode)
                {

                    return true;
                }
            }
            return false;

        }

        private double DblGridExitWHItem(string StrWH, string StrItem)
        {
            StrSql = "Select QTY from tblItemWhse where ItemId='" + StrItem + "' and WhseId='" + StrWH + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return double.Parse(dt.Rows[0].ItemArray[0].ToString().Trim());

            }
            else
            {
                return 0;

            }
        }

        private Boolean IsGridExitWHItem(string StrWH, string StrItem)
        {
            StrSql = "Select ItemId from tblItemWhse where ItemId='" + StrItem + "' and WhseId='" + StrWH + "'";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;

            }
        }


        public Boolean IsGridExitWHCode(String StrCode)
        {


            foreach (UltraGridRow ugR in cmbWH.Rows)
            {
                if (ugR.Cells["WhseId"].Text == StrCode)
                {

                    return true;
                }
            }
            return false;

        }

        //private Boolean IsGridExitWHItem(string StrWH, string StrItem)
        //{
        //    StrSql = "Select ItemId from tblItemWhse where ItemId='" + StrItem + "' and WhseId='" + StrWH + "'";
        //    SqlCommand cmd = new SqlCommand(StrSql);
        //    SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);

        //    if (dt.Rows.Count > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;

        //    }
        //}


        private Boolean IsWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
                throw;

            }
        }


        private void frmJobReturn_Load(object sender, EventArgs e)
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


                    dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmJobReturn");
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
                    GetJobDataSet();
                    GetWH();

                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);
                    GetJobStatus();
                    GetInvNo();
                    GetGLJob();

                    //intEstomateProcode = 0;

                }


            }
            catch { }
        }

        public void GetWH()
        {

            dsWarehouse = new DataSet();
            try
            {
                dsWarehouse.Clear();
                StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWH");

                cmbWH.DataSource = dsWarehouse.Tables["DtWH"];
                cmbWH.DisplayMember = "WhseId";
                cmbWH.ValueMember = "WhseId";

                cmbWH.DisplayLayout.Bands["DtWH"].Columns["WhseId"].Width = 70;
                cmbWH.DisplayLayout.Bands["DtWH"].Columns["WhseName"].Width = 150;

            }
            catch (Exception)
            {

                throw;
            }
        }



        private void cmbJobCode_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtDescription.Text = cmbJobCode.ActiveRow.Cells[1].Value.ToString();
                        blnIsPhase = Boolean.Parse((cmbJobCode.ActiveRow.Cells["IsPhase"].Value.ToString()));
                    }
                    else
                    {
                        blnIsPhase = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
        }

        private void cmbJobCode_Leave(object sender, EventArgs e)
        {
            if (cmbJobCode.Text != string.Empty)
            {
                cmbJobCode.Enabled = false;
            }
        }






    }
}