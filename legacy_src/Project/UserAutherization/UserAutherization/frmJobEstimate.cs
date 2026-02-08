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

namespace UserAutherization
{
    public partial class frmJobEstimate : Form
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

        public DataSet dsWarehouse;
        public DataSet dsJobCode;
        public DataSet dsPhaseCode;
        public DataSet dsCostCode;
        public DataSet dsCustomer;

        public DataSet dsItem;
        public Boolean blnIsPhase;


        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmJobEstimate(int intNo)
        {
            InitializeComponent();
            setConnectionString();

            if (intNo == 0)
            {
                intEstomateProcode = 0;
            }


        }




        public void GetJobDataSet(int intType)
        {
            dsJobCode = new DataSet();
            dsCostCode = new DataSet();
            dsPhaseCode = new DataSet();
            try
            {
                dsJobCode.Clear();
                dsPhaseCode.Clear();
                dsJobCode.Clear();

                if (intType == 0)
                {
                    StrSql = " Select JobID,JobDescription,IsPhase,CustomerID from tblJobMaster";
                }
                else
                {
                    StrSql = " Select JobID,JobDescription,IsPhase,CustomerID from tblJobMaster where Job_Status<2";
                }

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsJobCode, "dtJobCode");

                cmbJobCode.DataSource = dsJobCode.Tables["dtJobCode"];
                cmbJobCode.DisplayMember = "JobID";
                cmbJobCode.ValueMember = "JobID";
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobID"].Width = 75;
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["JobDescription"].Width = 150;
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["IsPhase"].Hidden = true;
                cmbJobCode.DisplayLayout.Bands["dtJobCode"].Columns["CustomerID"].Hidden = true;

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void setValue()
        {
            try
            {
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

                    if (ProjectStatus() >= 2)
                    {
                        GetJobDataSet(0);

                    }

                    EnableHeader(true);
                    //  ViewHeaderforReport(intEstomateProcode);

                    DialogResult reply = MessageBox.Show("Do you want Create New Estimate Base On Selected Estimate", "Copy Estimate", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (reply == DialogResult.Yes)
                    {
                        ViewHeader(intEstomateProcode);
                        ViewDetails(intEstomateProcode);
                        IsPhaseJob(intEstomateProcode);
                        EnableHeader(true);
                        cmbJobCode.Text = string.Empty;
                        GetInvNo();
                        intEstomateProcode = 0;
                        Edit();
                    }
                    else
                    {
                        ViewHeader(intEstomateProcode);
                        ViewDetails(intEstomateProcode);
                        IsPhaseJob(intEstomateProcode);
                        EnableHeader(false);

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        private void IsPhaseJob(int intAutoIndex)
        {
            try
            {
                StrSql = "SELECT AutoIndex FROM tblJobMaster WHERE AutoIndex=" + intAutoIndex + " and IsPhase=1";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    blnIsPhase = true;
                }
                else
                {
                    blnIsPhase = false;
                }
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
                StrSql = "SELECT JobEstPref, JobEstPad, JobEstNum FROM tblDefualtSetting";
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

                StrSql = "SELECT  TOP 1(JobEstNum) FROM tblDefualtSetting ORDER BY JobEstNum DESC";
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

                StrSql = "UPDATE tblDefualtSetting SET JobEstNum='" + intInvNo + "'";
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

                StrSql = "SELECT JobEstPref, JobEstPad, JobEstNum FROM tblDefualtSetting";
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

                throw;
            }

        }

        public void GetModeOfDelivery()
        {
            try
            {
                cmbModeOfDelivery.Items.Clear();
                string[] StrArray ={ "Reells", "Bags", "Pouches", "Sheets", "Tubes" };

                int intArray = 0;

                while (intArray < 5)
                {
                    cmbModeOfDelivery.Items.Add(StrArray[intArray]);
                    intArray++;
                }

                cmbModeOfDelivery.SelectedIndex = 0;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GetJobStatus()
        {
            cmbJobStatus.Items.Clear();
            cmbJobStatus.Items.Add("Quote");
            cmbJobStatus.Items.Add("Active");
            cmbJobStatus.Items.Add("Complete");

            cmbJobStatus.SelectedIndex = 0;
        }

        public void GetPaymentMode()
        {
            try
            {

                cmdPaymentMethod.Items.Clear();
                string[] StrArray ={ "VAT", "SVAT", "ALL" };

                int intArray = 0;

                while (intArray < 3)
                {
                    cmdPaymentMethod.Items.Add(StrArray[intArray]);
                    intArray++;
                }

                cmdPaymentMethod.SelectedIndex = 0;


            }
            catch (Exception)
            {

                throw;
            }

        }

        public void GetItemDataSet()
        {
            try
            {


                StrSql = "SELECT ItemID,ItemDescription,UnitCost,0,ItemClass,SalesGLAccount FROM tblItemMaster order by ItemID";

                //StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitCost, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";

                //StrSql = StrSql + "UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitCost,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (4,5)";

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

            if (double.Parse(txtEstimateQty.Value.ToString()) != 0)
            {
                txtStandedCostPerValue.Value = dblNetAmount / double.Parse(txtEstimateQty.Value.ToString());
            }



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


        private Boolean CheckExitsItem(int intEstCode, int intLineNo, string StrItemCode, string StrWarehouseCode, string StrPhaseCode, string StrCostCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT Item_ID FROM EST_DETAILS WHERE AutoIndex=" + intEstCode + " AND EstDet_LineId!=" + intLineNo + " AND Item_Id='" + StrItemCode + "' AND WarehouseCode='" + StrWarehouseCode + "' AND PhaseID='" + StrPhaseCode + "' and CostCodeID='" + StrCostCode + "'";

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


        private Boolean CheckWarehouseItem(string StrItemCode, string StrWarehouseCode, SqlConnection con, SqlTransaction Trans)
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


        public Boolean IsGridValidation()
        {

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

                //if (CheckWarehouseItem(ugR.Cells["ItemCode"].Text, ugR.Cells["WH"].Text) == true)
                //{
                //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Invalid Warehouse.", "Message", MessageBoxButtons.OK);
                //    return false;
                //}



            }

            if (ug.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public Boolean HeaderValidation()
        {

            if (cmbJobCode.Text.Trim() == "")
            {
                MessageBox.Show("Please Select Job Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;


            }
            if (cmbCustomer.Text.Trim() == "")
            {
                MessageBox.Show("Please Select Customer", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //-----------------------------------------------------

            if (clsPara.BlnManufactureCompany == true)
            {
                if (ugCmbItem.Text.Trim() == "")
                {
                    MessageBox.Show("Please Select Manufacture Item.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if ((double.Parse(txtEstimateQty.Value.ToString()) == 0))
                {
                    MessageBox.Show("Manufacturing Quantity Can Not Be Zero", "Information", MessageBoxButtons.OK);
                    return false;
                }

            }


            //-----------------------------------------------------

            if (cmbJobStatus.Text.Trim() == "")
            {
                return false;
            }

            return true;
        }

        public void ViewDetails(int intAutoIndex)
        {
            try
            {

                StrSql = "SELECT EstDet_LineId,Item_ID,Item_Description,EstDet_Est_Qty,EstDet_Est_Price,EstDet_Est_NetAmt,ItemClass,SalesGLAccount,WarehouseCode,IsPhaseID,PhaseID,IsCostCodeID,CostCodeID,Comments FROM EST_DETAILS  WHERE AutoIndex=" + intAutoIndex + " AND EstDet_Est_Qty>0 ORDER BY EstDet_LineId";

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
                        ugR.Cells["LineNo"].Value = Dr["EstDet_LineId"];
                        ugR.Cells["ItemCode"].Value = Dr["Item_ID"];
                        ugR.Cells["Description"].Value = Dr["Item_Description"];
                        ugR.Cells["UnitCost"].Value = Dr["EstDet_Est_Price"];
                        ugR.Cells["Quantity"].Value = Dr["EstDet_Est_Qty"];
                        ugR.Cells["TotalPrice"].Value = Dr["EstDet_Est_NetAmt"];
                        ugR.Cells["ItemClass"].Value = Dr["ItemClass"];
                        ugR.Cells["GL"].Value = Dr["SalesGLAccount"];
                        ugR.Cells["WH"].Value = Dr["WarehouseCode"];
                        ugR.Cells["IsPhaseID"].Value = (Dr["IsPhaseID"]);
                        ugR.Cells["PhaseID"].Value = Dr["PhaseID"];
                        ugR.Cells["IsCostCodeID"].Value = (Dr["IsCostCodeID"]);
                        ugR.Cells["CostCodeID"].Value = Dr["CostCodeID"];
                        ugR.Cells["Comments"].Value = Dr["Comments"];



                    }



                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }


        public void ViewHeaderforReport(string EstimateNo)
        {
            try
            {
                StrSql = "SELECT [JobDeliveryDate],[PaymentMode]" +
               ",[CrtApprovedNo],[VatRegNo],[OrderNo],[RepeatNo],[CustomerID]" +
               ",[CustomerName],[CustomerAddress1],[CustomerAddress2],[Design],[TypeofDesign],[ProofApprovedby],[EndUse],[NoofColours]" +
               ",[NoofLaydowns],[TypeofPrinting],[ModeofDelivery],[Width],[Lenght],[CutoffLenght],[DeliveryEndUse],[WindingDirection],SkirtingWidth,TopL,Hidden,Bottom,Side,Center,TBG,TES" +
                " FROM [tblEstmateHeader] WHERE [EstimateNo]='" + EstimateNo + "'";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //customer instructions
                    txtOrderNo.Text = dt.Rows[0]["OrderNo"].ToString();
                    txtRepeatOrderNo.Text = dt.Rows[0]["RepeatNo"].ToString();
                    cmbCustomer.Text = dt.Rows[0]["CustomerID"].ToString();
                    txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                    txtAddress1.Text = dt.Rows[0]["CustomerAddress1"].ToString();
                    txtAddress2.Text = dt.Rows[0]["CustomerAddress2"].ToString();

                    txtTypeOfDesign.Text = dt.Rows[0]["TypeofDesign"].ToString();
                    txtproofapprovedby.Text = dt.Rows[0]["ProofApprovedby"].ToString();

                    //work instructions
                    //JobDeliveryDate


                    dtpDeliveryDate.Value = DateTime.Parse(dt.Rows[0]["JobDeliveryDate"].ToString().Trim());
                    cmdPaymentMethod.SelectedItem = dt.Rows[0]["PaymentMode"].ToString();
                    txtcrtApproved.Text = dt.Rows[0]["CrtApprovedNo"].ToString();
                    txtvatregno.Text = dt.Rows[0]["VatRegNo"].ToString();

                    //sales instructions

                    txtEndUse.Text = dt.Rows[0]["EndUse"].ToString();
                    txtNoOFColors.Text = dt.Rows[0]["NoofColours"].ToString();
                    txtNoOfLayDowns.Text = dt.Rows[0]["NoofLaydowns"].ToString();
                    txtTypeofprinting.Text = dt.Rows[0]["TypeofPrinting"].ToString();

                    //delivery instructions
                    cmbModeOfDelivery.SelectedItem = dt.Rows[0]["ModeofDelivery"].ToString();
                    txtWidth.Text = dt.Rows[0]["Width"].ToString();
                    txtLenght.Text = dt.Rows[0]["Lenght"].ToString();
                    txtCutofflenght.Text = dt.Rows[0]["CutoffLenght"].ToString();

                    txtwindingDirection.Text = dt.Rows[0]["WindingDirection"].ToString();
                    txtSkirtingWidth.Text = dt.Rows[0]["SkirtingWidth"].ToString();
                    txtTop.Text = dt.Rows[0]["TopL"].ToString();
                    txtHidden.Text = dt.Rows[0]["Hidden"].ToString();
                    txtBottom.Text = dt.Rows[0]["Bottom"].ToString();
                    txtSide.Text = dt.Rows[0]["Side"].ToString();
                    txtCenter.Text = dt.Rows[0]["Center"].ToString();
                    txtTBG.Text = dt.Rows[0]["TBG"].ToString();
                    txtTES.Text = dt.Rows[0]["TES"].ToString();




                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }
        public void ViewHeader(int intAutoIndex)
        {
            try
            {

                StrSql = "SELECT [EstimateNo],[EstDate],[Description],[JobCode],[Job_Status],[EstHed_NetDiscPer],[EstHed_NetDiscAmt],[EstHed_NBTPer],[EstHed_NBTAmt],[EstHed_VATPer],[EstHed_VATAmt],[EstHed_SubAmt],[EstHed_GrossAmt],[EstHed_NetAmt],[EstHed_Standed_Cost_Per_Unit],[EstHed_Finished_Qty],[ProductionItem_ID] FROM EST_HEADER WHERE AutoIndex=" + intAutoIndex + "";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtJobId.Text = dt.Rows[0]["EstimateNo"].ToString();
                    ViewHeaderforReport(dt.Rows[0]["EstimateNo"].ToString().Trim());
                    dtpDate.Value = DateTime.Parse(dt.Rows[0]["EstDate"].ToString().Trim());
                    txtDescription.Text = dt.Rows[0]["Description"].ToString().Trim();
                    //cmbWarehouse.Text  =dt.Rows[0].ItemArray[3].ToString().Trim();
                    cmbJobCode.Text = dt.Rows[0]["JobCode"].ToString().Trim();
                    cmbJobStatus.SelectedIndex = int.Parse(dt.Rows[0]["Job_Status"].ToString().Trim());

                    //cmbJobStatus.Items.IndexOf(cmbJobStatus.Items.
                    //(int.Parse((dt.Rows[0].ItemArray[5].ToString().Trim())));

                    txtDiscPer.Value = double.Parse(dt.Rows[0]["EstHed_NetDiscPer"].ToString().Trim());
                    txtDiscAmount.Value = double.Parse(dt.Rows[0]["EstHed_NetDiscAmt"].ToString().Trim());
                    txtNBTPer.Value = double.Parse(dt.Rows[0]["EstHed_NBTPer"].ToString().Trim());
                    txtNBT.Value = double.Parse(dt.Rows[0]["EstHed_NBTAmt"].ToString().Trim());
                    txtVatPer.Value = double.Parse(dt.Rows[0]["EstHed_VATPer"].ToString().Trim());
                    txtVat.Value = double.Parse(dt.Rows[0]["EstHed_VATAmt"].ToString().Trim());


                    txtSubValue.Value = double.Parse(dt.Rows[0]["EstHed_SubAmt"].ToString().Trim());
                    txtGrossValue.Value = double.Parse(dt.Rows[0]["EstHed_GrossAmt"].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0]["EstHed_NetAmt"].ToString().Trim());

                    txtStandedCostPerValue.Value = double.Parse(dt.Rows[0]["EstHed_Standed_Cost_Per_Unit"].ToString().Trim());
                    txtEstimateQty.Value = double.Parse(dt.Rows[0]["EstHed_Finished_Qty"].ToString().Trim());
                    ugCmbItem.Text = dt.Rows[0]["ProductionItem_ID"].ToString();

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }
        }


        public void GetItemDataset1()//Infragistics
        {
            dsItem = new DataSet();
            try
            {

                dsItem.Clear();
                StrSql = "SELECT ItemID, ItemDescription FROM tblItemMaster order by ItemID";

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
            catch (Exception)
            {
                throw;
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
        private void frmJobEstimate_Load(object sender, EventArgs e)
        {

            try
            {
                intEstomateProcode = 0;
                if (intEstomateProcode == 0)
                {
                    run = false;
                    add = false;
                    edit = false;
                    delete = false;

                    dtUser = DataAccess.Access.setUserAuthentication(UserAutherization.user.userName, "frmJobEstimate");
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

                    GetCurrentUserDate();

                    btnSave.Enabled = false;
                    btnPrint.Enabled = false;
                    btnSearch.Enabled = true;
                    btnReset.Enabled = true;
                    btnNew.Enabled = true;
                    btnEdit.Enabled = false;
                    dtpDate.Enabled = false;
                    btnMRN.Enabled = false;
                    btnWorkTicket.Enabled = false;

                    GetJobDataSet(1);
                    GetItemDataSet();
                    GetItemDataset1();
                    GetCustomer();

                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    GetJobStatus();
                    GetPaymentMode();
                    GetModeOfDelivery();
                    GetInvNo();
                    GetWH();


                    if (clsPara.BlnManufactureCompany == true)
                    {
                        grpIsManufactureCompany.Visible = true;
                    }
                    else
                    {
                        grpIsManufactureCompany.Visible = false;
                    }


                }


            }
            catch { }
        }


        private int CheckJobStatus(string strJobCode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "SELECT AutoIndex FROM tblJobMaster WHERE JOBID='" + strJobCode + "'";

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
            if (add)
            {
                btnSave.Enabled = true;
                btnNew.Enabled = false;
                btnPrint.Enabled = false;
                btnSNO.Enabled = false;
                btnSearch.Enabled = false;
                btnReset.Enabled = true;
                btnEdit.Enabled = false;
                EnableHeader(true);

                ClearHeader();
                DeleteRows();
                GetInvNo();
                GetJobDataSet(1);
                intEstomateProcode = 0;
                cmbJobCode.Focus();
                GetItemDataset1();

            }
            else
            {
                MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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


        private void btnSave_Click(object sender, EventArgs e)
        {
            int intGrid;
            int intAutoIndex;
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

                if (intEstomateProcode == 0)
                {
                    if (CheckJobStatus(cmbJobCode.Value.ToString(), myConnection, myTrans) != 0)
                    {
                        MessageBox.Show("This Job is already Entered.", "Information", MessageBoxButtons.OK);
                        ButtonClear();
                        return;
                    }
                }


                if (intEstomateProcode != 0)
                {
                    DeleteDetails(intEstomateProcode, myConnection, myTrans);
                    intAutoIndex = intEstomateProcode;
                    ModifyHeader(intAutoIndex, myConnection, myTrans);
                    ModifyHeaderforReport(intAutoIndex, myConnection, myTrans);

                }
                else
                {


                    intAutoIndex = SaveHeader(myConnection, myTrans);

                   // SaveHeader(myConnection, myTrans);
                   // intAutoIndex = GetLastTransactionNo(myConnection, myTrans);
                    SaveDataforMRNReport(intAutoIndex, myConnection, myTrans);

                }

                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {

                    intItemClass = int.Parse(ug.Rows[intGrid].Cells["ItemClass"].Value.ToString());

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

                    if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                    {

                        if (CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), myConnection, myTrans) == false)
                        {
                            MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Invalid Item Or Warehouse", "Message", MessageBoxButtons.OK);
                            myTrans.Rollback();
                            return;

                        }

                    }

                    //if (CheckWarehouseItem(ugR.Cells["LineNo"].Text, ugR.Cells["ItemCode"].Text, ugR.Cells["WH"].Text, ugR.Cells["PhaseID"].Text, ugR.Cells["CostCodeID"].Text) == true)
                    //{
                    //    MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Item Already Assign.", "Message", MessageBoxButtons.OK);
                    //    return false;
                    //}

                    if (CheckExitsItem(intAutoIndex, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), myConnection, myTrans) == true)
                    {
                        MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Item Already Assign.", "Message", MessageBoxButtons.OK);
                        myTrans.Rollback();
                        return;
                    }
                    SaveDetails(intAutoIndex, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitCost"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["ItemClass"].Value.ToString(), ug.Rows[intGrid].Cells["GL"].Value.ToString(), ug.Rows[intGrid].Cells["WH"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsPhaseID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["PhaseID"].Value.ToString(), (Boolean.Parse(ug.Rows[intGrid].Cells["IsCostCodeID"].Value.ToString()) == true ? 1 : 0), ug.Rows[intGrid].Cells["CostCodeID"].Value.ToString(), ug.Rows[intGrid].Cells["Comments"].Value.ToString(), myConnection, myTrans);

                }

                if (intEstomateProcode == 0)
                {
                    UpdatePrefixNo(myConnection, myTrans);
                    UpdateInvoiceNo(intAutoIndex, myConnection, myTrans);
                }


                UpdateJobMaster(intAutoIndex, cmbJobCode.Value.ToString(), cmbJobStatus.SelectedIndex, myConnection, myTrans);

                myTrans.Commit();
                MessageBox.Show("Job Estimate Successfuly Saved.", "Information", MessageBoxButtons.OK);

                Print(intAutoIndex);

                ButtonClear();

            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);

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

        private void UpdateJobMaster(int intAutoIndex, string strJobID, int intJobStatus, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE tblJobMaster SET AutoIndex=" + intAutoIndex + ",Job_Status=" + intJobStatus + " WHERE JobID='" + strJobID + "'";

                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }

        }





        private void ModifyHeaderforReport(int intEstAutoId, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "UPDATE [tblEstmateHeader] set " +
                  " [JobID]='" + cmbJobCode.Text.ToString() + "'," +
                  " [JobDescription]='" + txtDescription.Text.Trim() + "'," +
                  " [JobStatus]='" + cmbJobStatus.SelectedIndex + "'," +
                  " [JobStartDate]='" + GetDateTime(dtpDate.Value) + "'," +
                  " [JobDeliveryDate]='" + GetDateTime(dtpDeliveryDate.Value) + "'," +
                  " [PaymentMode]='" + cmdPaymentMethod.Text.Trim() + "'," +
                  " [CrtApprovedNo]='" + txtcrtApproved.Text.Trim() + "'," +
                  " [VatRegNo]='" + txtvatregno.Text.Trim() + "'," +
                  " [ManufacturingItem]='" + ugCmbItem.Text.ToString().Trim() + "'," +
                  " [ManufactureQty]='" + txtEstimateQty.Value.ToString().Trim() + "'," +
                  " [StaderdCost]='" + Convert.ToDouble(txtStandedCostPerValue.Value) + "'," +
                  " [OrderNo]='" + txtOrderNo.Text.Trim() + "'," +
                  " [RepeatNo]='" + txtRepeatOrderNo.Text.Trim() + "'," +
                  " [CustomerID]='" + cmbCustomer.Text.Trim() + "'," +
                  " [CustomerName]='" + txtCustomer.Text.Trim() + "'," +
                  " [CustomerAddress1]='" + txtAddress1.Text.Trim() + "'," +
                  " [CustomerAddress2]='" + txtAddress2.Text.Trim() + "'," +
                  " [TypeofDesign]='" + txtTypeOfDesign.Text.Trim() + "'," +
                  " [ProofApprovedby]='" + txtproofapprovedby.Text.Trim() + "'," +
                  " [EndUse]='" + txtEndUse.Text.Trim() + "'," +
                  " [NoofColours]='" + txtNoOFColors.Text.Trim() + "'," +
                  " [NoofLaydowns]='" + txtNoOfLayDowns.Text.Trim() + "'," +
                  " [TypeofPrinting]='" + txtTypeofprinting.Text.Trim() + "'," +
                  " [ModeofDelivery]='" + cmbModeOfDelivery.Text.Trim() + "'," +
                  " [Width]='" + txtWidth.Text.Trim() + "'," +
                  " [Lenght]='" + txtLenght.Text.Trim() + "'," +
                  " [CutoffLenght]='" + txtCutofflenght.Text.Trim() + "'," +
                  "  SkirtingWidth='" + txtSkirtingWidth.Text.Trim() + "'," +
                  "  TopL='" + txtTop.Text.Trim() + "'," +
                  "  Hidden='" + txtHidden.Text.Trim() + "'," +
                   " Bottom='" + txtBottom.Text.Trim() + "'," +
                   " Side='" + txtSide.Text.Trim() + "'," +
                   " Center='" + txtCenter.Text.Trim() + "'," +
                   " TBG='" + txtTBG.Text.Trim() + "'," +
                   " TES='" + txtTES.Text.Trim() + "'," +
                  " [WindingDirection]='" + txtwindingDirection.Text.Trim() + "' where EstAutoIndex=" + intEstAutoId + "";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ModifyHeader(int intEstomateProcode, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (clsPara.BlnManufactureCompany == true)
                {

                    StrSql = "UPDATE [EST_HEADER] SET [DocType] =1,[EstDate] ='" + GetDateTime(dtpDate.Value) + "',[Description] = '" + txtDescription.Text.Trim() + "',[JobCode] = '" + cmbJobCode.Value.ToString() + "', " +
                   " [Job_Status]=" + cmbJobStatus.SelectedIndex + ",[EstHed_SubAmt] =" + txtSubValue.Value + ",[EstHed_NetDiscPer]=" + txtDiscPer.Value + ",[EstHed_NetDiscAmt] =" + txtDiscAmount.Value + ",[EstHed_GrossAmt] =" + txtGrossValue.Value + ", " +
                   " [EstHed_NBTPer] =" + txtNBTPer.Value + ",[EstHed_NBTAmt] =" + txtNBT.Value + ",[EstHed_VATPer] =" + txtNBTPer.Value + ",[EstHed_VATAmt] =" + txtVat.Value + ",[EstHed_NetAmt]=" + txtNetValue.Value + ",[EstHed_Finished_Qty]=" + txtEstimateQty.Value + ",[EstHed_Standed_Cost_Per_Unit]=" + txtStandedCostPerValue.Value + " WHERE AutoIndex=" + intEstomateProcode + "";

                }
                else
                {

                    StrSql = "UPDATE [EST_HEADER] SET [DocType] =1,[EstDate] ='" + GetDateTime(dtpDate.Value) + "',[Description] = '" + txtDescription.Text.Trim() + "',[JobCode] = '" + cmbJobCode.Value.ToString() + "', " +
                            " [Job_Status]=" + cmbJobStatus.SelectedIndex + ",[EstHed_SubAmt] =" + txtSubValue.Value + ",[EstHed_NetDiscPer]=" + txtDiscPer.Value + ",[EstHed_NetDiscAmt] =" + txtDiscAmount.Value + ",[EstHed_GrossAmt] =" + txtGrossValue.Value + ", " +
                            " [EstHed_NBTPer] =" + txtNBTPer.Value + ",[EstHed_NBTAmt] =" + txtNBT.Value + ",[EstHed_VATPer] =" + txtNBTPer.Value + ",[EstHed_VATAmt] =" + txtVat.Value + ",[EstHed_NetAmt]=" + txtNetValue.Value + " WHERE AutoIndex=" + intEstomateProcode + "";

                }



                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }
        private void SaveDataforMRNReport(int intEstAutoId, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO [tblEstmateHeader]([EstAutoIndex],[EstimateNo],[JobID],[JobDescription]" +
              ",[JobStatus],[JobStartDate],[JobDeliveryDate],[PaymentMode],[CrtApprovedNo],[VatRegNo],[ManufacturingItem],[ManufactureQty]" +
              ",[StaderdCost],[OrderNo],[RepeatNo],[CustomerID],[CustomerName],[CustomerAddress1],[CustomerAddress2],[TypeofDesign]" +
              ",[ProofApprovedby],[EndUse],[NoofColours],[NoofLaydowns],[TypeofPrinting],[ModeofDelivery],[Width],[Lenght],[CutoffLenght]" +
              ",WindingDirection,SkirtingWidth,TopL,Hidden,Bottom,Side,Center,TBG,TES)VALUES(" + intEstAutoId + ",'" + txtJobId.Text.Trim() + "','" + cmbJobCode.Text.ToString() + "','" + txtDescription.Text.Trim() + "'" +
              ",'" + cmbJobStatus.SelectedIndex + "','" + GetDateTime(dtpDate.Value) + "','" + GetDateTime(dtpDeliveryDate.Value) + "','" + cmdPaymentMethod.Text.Trim() + "'" +
              ",'" + txtcrtApproved.Text.Trim() + "','" + txtvatregno.Text.Trim() + "','" + ugCmbItem.Text.ToString().Trim() + "','" + txtEstimateQty.Value.ToString().Trim() + "'" +
              ",'" + Convert.ToDouble(txtStandedCostPerValue.Value) + "','" + txtOrderNo.Text.Trim() + "','" + txtRepeatOrderNo.Text.Trim() + "','" + cmbCustomer.Text.Trim() + "'" +
              ",'" + txtCustomer.Text.Trim() + "','" + txtAddress1.Text.Trim() + "','" + txtAddress2.Text.Trim() + "','" + txtTypeOfDesign.Text.Trim() + "'" +
              ",'" + txtproofapprovedby.Text.Trim() + "','" + txtEndUse.Text.Trim() + "','" + txtNoOFColors.Text.Trim() + "','" + txtNoOfLayDowns.Text.Trim() + "'" +
              ",'" + txtTypeofprinting.Text.Trim() + "','" + cmbModeOfDelivery.Text.Trim() + "','" + txtWidth.Text.Trim() + "','" + txtLenght.Text.Trim() + "'" +
              ",'" + txtCutofflenght.Text.Trim() + "','" + txtwindingDirection.Text.Trim() + "','" + txtSkirtingWidth.Text.Trim() + "','" + txtTop.Text.Trim() + "','" + txtHidden.Text.Trim() + "','" + txtBottom.Text.Trim() + "','" + txtSide.Text.Trim() + "','" + txtCenter.Text.Trim() + "','" + txtTBG.Text.Trim() + "','" + txtTES.Text.Trim() + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }
        }



        private int  SaveHeader(SqlConnection con, SqlTransaction Trans)
        {
            int index = 0;
            try
            {
                if (clsPara.BlnManufactureCompany == true)
                {

                    StrSql = "INSERT INTO [EST_HEADER]([DocType],[EstimateNo],[EstDate],[Description] " +
                    " ,[JobCode],[Job_Status],[EstHed_SubAmt],[EstHed_NetDiscPer],[EstHed_NetDiscAmt],[EstHed_GrossAmt] " +
                    ",[EstHed_NBTPer],[EstHed_NBTAmt],[EstHed_VATPer],[EstHed_VATAmt],[EstHed_NetAmt] " +
                    ",[EstHed_Process],[EstHed_UserCreated],[EstHed_DateCreated],[EstHed_UserModified],[EstHed_DateModified],[EstHed_Standed_Cost_Per_Unit],[EstHed_Finished_Qty],[ProductionItem_ID]) " +
                    " VALUES(1,'" + txtJobId.Text.Trim() + "','" + GetDateTime(dtpDate.Value) + "','" + txtDescription.Text.Trim() + "', " +
                    " '" + cmbJobCode.Value.ToString() + "'," + cmbJobStatus.SelectedIndex + "," + txtSubValue.Value + "," + txtDiscPer.Value + "," + txtDiscAmount.Value + "," + txtGrossValue.Value + ", " +
                    " " + txtNBTPer.Value + "," + txtNBT.Value + "," + txtVatPer.Value + "," + txtVat.Value + "," + txtNetValue.Value + ", " +
                    " 0,'" + user.userName + "','" + GetDateTime(DateTime.Now) + "','" + user.userName + "','" + GetDateTime(DateTime.Now) + "'," + txtStandedCostPerValue.Value + "," + txtEstimateQty.Value + ",'" + ugCmbItem.Value.ToString().Trim() + "')" +
                     " select @@Identity ";
                }
                else
                {
                    StrSql = "INSERT INTO [EST_HEADER]([DocType],[EstimateNo],[EstDate],[Description] " +
                       " ,[JobCode],[Job_Status],[EstHed_SubAmt],[EstHed_NetDiscPer],[EstHed_NetDiscAmt],[EstHed_GrossAmt] " +
                       ",[EstHed_NBTPer],[EstHed_NBTAmt],[EstHed_VATPer],[EstHed_VATAmt],[EstHed_NetAmt] " +
                       ",[EstHed_Process],[EstHed_UserCreated],[EstHed_DateCreated],[EstHed_UserModified],[EstHed_DateModified],[EstHed_Standed_Cost_Per_Unit],[EstHed_Finished_Qty],[ProductionItem_ID]) " +
                       " VALUES(1,'" + txtJobId.Text.Trim() + "','" + GetDateTime(dtpDate.Value) + "','" + txtDescription.Text.Trim() + "', " +
                       " '" + cmbJobCode.Value.ToString() + "'," + cmbJobStatus.SelectedIndex + "," + txtSubValue.Value + "," + txtDiscPer.Value + "," + txtDiscAmount.Value + "," + txtGrossValue.Value + ", " +
                       " " + txtNBTPer.Value + "," + txtNBT.Value + "," + txtVatPer.Value + "," + txtVat.Value + "," + txtNetValue.Value + ", " +
                       " 0,'" + user.userName + "','" + GetDateTime(DateTime.Now) + "','" + user.userName + "','" + GetDateTime(DateTime.Now) + "'," + txtStandedCostPerValue.Value + "," + txtEstimateQty.Value + ",'')" +
                        " select @@Identity ";
                }
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

        private void SaveDetails(int intAutoIndex, int intLineId, String StrItemCode, string StrItemDescription, double dblQuantity, double dblPrice, double dblLineNetAmt, string intItemClass, string StrGLAccount, string StrWH, Int32 intPhaseCode, string StrPhaseCode, Int32 intCostCode, string StrCostCode, String StrComments, SqlConnection con, SqlTransaction Trans)
        {
            try
            {

                StrSql = "INSERT INTO [EST_DETAILS]([AutoIndex],[EstDet_LineId],[Item_ID],[Item_Description],[EstDet_Est_Qty],[EstDet_Est_Price],[EstDet_Est_NetAmt],[ItemClass],[SalesGLAccount],WarehouseCode,IsPhaseID,PhaseID,IsCostCodeID,CostCodeID,Comments) " +
                   " VALUES(" + intAutoIndex + "," + intLineId + ",'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + intItemClass + "','" + StrGLAccount + "','" + StrWH + "'," + intPhaseCode + ",'" + StrPhaseCode + "'," + intCostCode + ",'" + StrCostCode + "','" + StrComments + "')";



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
                btnMRN.Enabled = true;
                btnWorkTicket.Enabled = true;
                frmJobEstimateSearch issueSearch = new frmJobEstimateSearch();
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

        private void DatasetWhse()
        {
            StrSql = "SELECT * FROM tblWhseMaster";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtWhseMaster);



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


        private void GetDataSet()
        {
            StrSql = "SELECT CutomerID,CustomerName FROM tblCustomerMaster";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtCustomer);




        }

        private void GetJobTiket(int intEstNo)
        {
            StrSql = "SELECT * FROM tblEstmateHeader WHERE EstAutoIndex=" + intEstNo + "";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DTJobticketData);
        }




        private void DataSetDetails(int intEstimateNo)
        {
            StrSql = "SELECT * FROM EST_DETAILS WHERE AutoIndex=" + intEstimateNo + "";

            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(DsEst.DtEstimateDETAILS);

        }




        private void Print(int intRecordNo)
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
                    DsEst.Clear();
                    DataSetHeader(intRecordNo);
                    DataSetDetails(intRecordNo);
                    GetJobTiket(intRecordNo);
                    DatasetWhse();
                    DataGetItem();
                    DataGetPhase();
                    DataGetCostCode();



                    frmViewerJobEstimate frmviewer = new frmViewerJobEstimate(this);
                    frmviewer.Show();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error :" + ex.Message);
                }

            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            clsPara.intEstPrint = 1;
            Print(intEstomateProcode);
        }



        private void ClearHeader()
        {
            try
            {
                txtDescription.Text = "";
                txtOrderNo.Text = "";
                txtRepeatOrderNo.Text = "";
                cmbCustomer.Text = "";
                txtCustomer.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtTypeOfDesign.Text = "";
                txtproofapprovedby.Text = "";
                cmdPaymentMethod.SelectedItem = 0;
                txtcrtApproved.Text = "";
                txtvatregno.Text = "";
                txtEndUse.Text = "";
                txtNoOFColors.Text = "";
                txtNoOfLayDowns.Text = "";
                txtTypeofprinting.Text = "";
                cmbModeOfDelivery.SelectedItem = 0;
                txtWidth.Text = "";
                txtLenght.Text = "";
                txtCutofflenght.Text = "";
                txtwindingDirection.Text = "";


                dtpDate.Value = user.LoginDate;
                dtpDeliveryDate.Value = user.LoginDate;
                cmbJobCode.Text = "";
                txtNBTPer.Value = 0;
                txtVatPer.Value = 0;
                txtDiscPer.Value = 0;
                txtSubValue.Value = 0;
                txtDiscAmount.Value = 0;
                txtGrossValue.Value = 0;
                txtStandedCostPerValue.Value = 0;
                txtEstimateQty.Value = 0;
                txtNBT.Value = 0;
                txtVat.Value = 0;
                txtNetValue.Value = 0;
                cmbJobStatus.SelectedItem = 0;
                ugCmbItem.Text = "";

                blnIsPhase = false;

                txtSkirtingWidth.Text = "";
                txtTop.Text = "";
                txtHidden.Text = "";
                txtBottom.Text = "";
                txtSide.Text = "";
                txtCenter.Text = "";
                txtTBG.Text = "";
                txtTES.Text = "";

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error :" + ex.Message);
            }


        }

        private void EnableHeader(Boolean blnEnable)
        {

            dtpDate.Enabled = blnEnable;
            cmbJobCode.Enabled = blnEnable;
            txtVatPer.Enabled = blnEnable;
            txtNBTPer.Enabled = blnEnable;
            txtDescription.Enabled = blnEnable;
            cmbJobStatus.Enabled = blnEnable;
            txtDiscPer.Enabled = blnEnable;
            ugCmbItem.Enabled = blnEnable;


            txtOrderNo.Enabled = blnEnable;
            txtRepeatOrderNo.Enabled = blnEnable;
            //cmbCustomer.Enabled = blnEnable;
            txtCustomer.Enabled = blnEnable;
            txtAddress1.Enabled = blnEnable;
            txtAddress2.Enabled = blnEnable;

            txtTypeOfDesign.Enabled = blnEnable;
            txtproofapprovedby.Enabled = blnEnable;

            //work instructions

            dtpDeliveryDate.Enabled = blnEnable;
            cmdPaymentMethod.Enabled = blnEnable;
            txtcrtApproved.Enabled = blnEnable;
            txtvatregno.Enabled = blnEnable;

            //sales instructions

            txtEndUse.Enabled = blnEnable;
            txtNoOFColors.Enabled = blnEnable;
            txtNoOfLayDowns.Enabled = blnEnable;
            txtTypeofprinting.Enabled = blnEnable;

            //delivery instructions
            cmdPaymentMethod.Enabled = blnEnable;
            txtWidth.Enabled = blnEnable;
            txtLenght.Enabled = blnEnable;
            txtCutofflenght.Enabled = blnEnable;

            txtwindingDirection.Enabled = blnEnable;

            //txtStandedCostPerValue.Enabled = blnEnable;
            txtEstimateQty.Enabled = blnEnable;

            ug.Enabled = blnEnable;

            txtSkirtingWidth.Enabled = blnEnable;
            txtTop.Enabled = blnEnable;
            txtHidden.Enabled = blnEnable;
            txtBottom.Enabled = blnEnable;
            txtSide.Enabled = blnEnable;
            txtCenter.Enabled = blnEnable;
            txtTBG.Enabled = blnEnable;
            txtTES.Enabled = blnEnable;
            cmbModeOfDelivery.Enabled = blnEnable;

        }

        private void ButtonClear()
        {
            btnSave.Enabled = false;
            btnNew.Enabled = true;
            btnPrint.Enabled = true;
            btnMRN.Enabled = true;
            btnWorkTicket.Enabled = true;

            btnSearch.Enabled = true;
            btnReset.Enabled = true;
            btnEdit.Enabled = false;

            ClearHeader();
            EnableHeader(false);
            DeleteRows();
            GetInvNo();
            ug.Enabled = false;
            intEstomateProcode = 0;

            GetJobDataSet(1);

        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            ButtonClear();

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
            EnableHeader(true);
            dtpDate.Enabled = true;
            cmbJobStatus.Enabled = true;
            txtDescription.Enabled = true;
            txtEstimateQty.Enabled = true;

            txtDiscPer.Enabled = true;
            txtNBTPer.Enabled = true;
            txtVatPer.Enabled = true;

            btnReset.Enabled = true;
            btnSave.Enabled = true;
            ug.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {



            if (ProjectStatus() != 2)
            {
                GetItemDataSet();
                Edit();

            }
            else
            {
                MessageBox.Show("Job Status is complete. You can not modify this record.", "Information", MessageBoxButtons.OK);
            }
        }


        public Boolean IsDuplicateItem(string StrCode)
        {

            int intCount = 0;

            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                if (ugR.Cells["ItemCode"].Value.ToString() == StrCode)
                {
                    intCount++;

                }

                if (intCount > 1)
                {
                    MessageBox.Show("Item Already Exist.", "Information", MessageBoxButtons.OK);
                    return false;

                }

            }

            return true;
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

        //private Boolean IsGridExitCostCode(string StrPhase)
        //{
        //    try
        //    {
        //        StrSql = "Select HasCostCodes from tblPhase where PhaseID='" + StrPhase + "' ";
        //        SqlCommand cmd = new SqlCommand(StrSql);
        //        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        if (dt.Rows.Count > 0)
        //        {
        //            if (Boolean.Parse(dt.Rows[0].ItemArray[0].ToString()) == true)
        //            {
        //                return true ;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}


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

        public Boolean IsItemCode(String StrCode)
        {
            foreach (UltraGridRow ugR in ugCmbItem.Rows)
            {
                if (ugR.Cells["ItemID"].Text == StrCode)
                {

                    return true;
                }
            }
            return false;

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



        private void ug_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {

            int intItemClass = 0;

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

                        if (IsGridExitCode(ug.ActiveCell.Value.ToString()) == false)
                        {
                            e.Cancel = true;
                            return;

                        }


                        foreach (UltraGridRow ugR in ultraCombo1.Rows)
                        {

                            if (ug.ActiveCell.Value.ToString() == ugR.Cells["ItemId"].Value.ToString())
                            {
                                ug.ActiveCell.Row.Cells["Description"].Value = ultraCombo1.ActiveRow.Cells["ItemDescription"].Value;
                                ug.ActiveCell.Row.Cells["UnitCost"].Value = ultraCombo1.ActiveRow.Cells["UnitCost"].Value;
                                ug.ActiveCell.Row.Cells["ItemClass"].Value = ultraCombo1.ActiveRow.Cells["ItemClass"].Value;
                                ug.ActiveCell.Row.Cells["GL"].Value = ultraCombo1.ActiveRow.Cells["SalesGLAccount"].Value;
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
                                    //ug.ActiveCell.Row.Cells["Description"].Value = ultraCombo1.ActiveRow.Cells["ItemDescription"].Value;
                                    //ug.ActiveCell.Row.Cells["UnitCost"].Value = ultraCombo1.ActiveRow.Cells["UnitCost"].Value;
                                    //ug.ActiveCell.Row.Cells["ItemClass"].Value = ultraCombo1.ActiveRow.Cells["ItemClass"].Value;
                                    //ug.ActiveCell.Row.Cells["GL"].Value = ultraCombo1.ActiveRow.Cells["SalesGLAccount"].Value;
                                    //ug.ActiveCell.Row.Cells["Quantity"].Value = 1;


                                }
                            }
                        }


                    }


                }

                //-------------------------------------------------------------------
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

        private void txtEstimateQty_Leave(object sender, EventArgs e)
        {
            GrandTotal();
        }

        private void cmbJobCode_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {

                    if (e.Row.Activated == true)
                    {
                        txtDescription.Text = cmbJobCode.ActiveRow.Cells["JobDescription"].Value.ToString();
                        blnIsPhase = Boolean.Parse((cmbJobCode.ActiveRow.Cells["IsPhase"].Value.ToString()));
                        cmbCustomer.Text = cmbJobCode.ActiveRow.Cells["CustomerID"].Value.ToString();




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


        private void ugCmbItem_RowSelected(object sender, RowSelectedEventArgs e)
        {
            if (IsItemCode(ugCmbItem.Text.Trim()) == false)
            {
                ugCmbItem.Text = "";
                return;

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



        private void cmbJobCode_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cmbJobCode.Text != string.Empty)
                {
                    cmbJobCode.Enabled = false;

                    if (cmbCustomer.Text == string.Empty)
                    {
                        MessageBox.Show("Customer Shold be Link With Job.", "Information", MessageBoxButtons.OK);
                        ButtonClear();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbCustomer_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    txtCustomer.Text = string.Empty;
                    txtAddress1.Text = string.Empty;
                    txtAddress2.Text = string.Empty;

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

        private void btnMRN_Click(object sender, EventArgs e)
        {
            clsPara.intEstPrint = 2;
            Print(intEstomateProcode);
        }

        private void btnWorkTicket_Click(object sender, EventArgs e)
        {
            clsPara.intEstPrint = 3;
            Print(intEstomateProcode);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void cmdPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


















    }
}