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

    public partial class frmIssueNote : Form
    {
        bool _IsFind = false;
        clsCommon objclsCommon = new clsCommon();
        public DSEstimate DsEst = new DSEstimate();
        bool run;
        bool add;
        bool edit;
        bool delete;
        DataTable dtUser = new DataTable();
        public string sMsg = "Peachtree - UserAutherization";

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
        public string StrGLCurrentAC;
        public string StrDocumentType;
        public DataSet dsWarehouse;
        public DataSet dsDeparment;
        public DataSet dsChartAcc;
        public DsItemWiseSales DsItemWise = new DsItemWiseSales();
        public DSIssueNote ObjDSIssueNote = new DSIssueNote();
        Controlers objControlers = new Controlers(); 

        public static DateTime UserWiseDate = System.DateTime.Now;

        public frmIssueNote(int intNo)
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
            catch (Exception ex)
            {
                throw ex;
            }
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
                int intInvNo;
                SqlCommand command;
                if (rdoOptIssue.Checked == true)
                {
                    StrSql = "SELECT  TOP 1(IssueNoteNum) FROM tblDefualtSetting ORDER BY IssueNoteNum DESC";
                }
                if (rdoOptReturn.Checked == true)
                {
                    StrSql = "SELECT  TOP 1(ReturnNoteNum) FROM tblDefualtSetting ORDER BY ReturnNoteNum DESC";
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
                if (rdoOptIssue.Checked == true)
                {
                    StrSql = "UPDATE tblDefualtSetting SET IssueNoteNum='" + intInvNo + "'";
                }
                if (rdoOptReturn.Checked == true)
                {
                    StrSql = "UPDATE tblDefualtSetting SET ReturnNoteNum='" + intInvNo + "'";
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

                if (rdoOptIssue.Checked == true)
                {
                    StrSql = "SELECT IssueNotePref, IssueNotePad, IssueNoteNum FROM tblDefualtSetting";
                    StrDocumentType = "IssueNote";
                }
                if (rdoOptReturn.Checked == true)
                {
                    StrSql = "SELECT ReturnNotePref, ReturnNotePad, ReturnNoteNum FROM tblDefualtSetting";
                    StrDocumentType = "ReturnNote";
                }

                // StrSql = "SELECT IssueNotePref, IssueNotePad, IssueNoteNum FROM tblDefualtSetting";

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

                if (rdoOptIssue.Checked == true)
                {
                    StrSql = "SELECT IssueNotePref, IssueNotePad, IssueNoteNum FROM tblDefualtSetting";
                }
                if (rdoOptReturn.Checked == true)
                {
                    StrSql = "SELECT ReturnNotePref, ReturnNotePad, ReturnNoteNum FROM tblDefualtSetting";
                }
                // StrSql = "SELECT IssueNotePref, IssueNotePad, IssueNoteNum FROM tblDefualtSetting";

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

        public void GetAccountData()
        {
            //sanju
            dsChartAcc = new DataSet();
            try
            {
                dsChartAcc.Clear();
                StrSql = " SELECT AcountID,AccountDescription FROM tblChartofAcounts order by AcountID";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsChartAcc, "DTChartAcc");
                cmbAccountType.DataSource = dsChartAcc.Tables["DTChartAcc"];
                cmbAccountType.DisplayMember = "AcountID";
                cmbAccountType.ValueMember = "AccountDescription";
                cmbAccountType.DisplayLayout.Bands["DTChartAcc"].Columns["AcountID"].Width = 100;
                cmbAccountType.DisplayLayout.Bands["DTChartAcc"].Columns["AccountDescription"].Width = 150;
                //cmbAccountType.DisplayLayout.Bands["DTChartAcc"].Columns["AountType"].Width = 75;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //GetDepatemtDataSet
        public void GetDepatemtDataSet()
        {
            dsDeparment = new DataSet();
            try
            {
                dsDeparment.Clear();
                StrSql = " SELECT DepartmentCode,DepartmentName,Type,ControlAccount FROM tblDeparmentMaster order by DepartmentCode";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsDeparment, "DtDepartment");

                ugcmbDepCode.DataSource = dsDeparment.Tables["DtDepartment"];
                ugcmbDepCode.DisplayMember = "DepartmentCode";
                ugcmbDepCode.ValueMember = "DepartmentCode";
                ugcmbDepCode.DisplayLayout.Bands["DtDepartment"].Columns["DepartmentCode"].Width = 100;
                ugcmbDepCode.DisplayLayout.Bands["DtDepartment"].Columns["DepartmentName"].Width = 250;
                ugcmbDepCode.DisplayLayout.Bands["DtDepartment"].Columns["Type"].Width = 100;
                ugcmbDepCode.DisplayLayout.Bands["DtDepartment"].Columns["ControlAccount"].Hidden = true;
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
                StrSql = " SELECT WhseId, WhseName FROM tblWhseMaster order by WhseId";

                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsWarehouse, "DtWarehouse");

                cmbWarehouse.DataSource = dsWarehouse.Tables["DtWarehouse"];
                cmbWarehouse.DisplayMember = "WhseId";
                cmbWarehouse.ValueMember = "WhseId";
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 180;
                cmbWarehouse.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 360;

                //cmbAccountType.DataSource = dsWarehouse.Tables["DtWarehouse"];
                //cmbAccountType.DisplayMember = "WhseId";
                //cmbAccountType.ValueMember = "WhseId";
                //cmbAccountType.DisplayLayout.Bands["DtWarehouse"].Columns["WhseId"].Width = 180;
                //cmbAccountType.DisplayLayout.Bands["DtWarehouse"].Columns["WhseName"].Width = 360;
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

                StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse INNER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";
                StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)";

                //StrSql = "SELECT  tblItemWhse.ItemId, tblItemWhse.ItemDis,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5, tblItemWhse.QTY,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemWhse RIGHT OUTER JOIN tblItemMaster ON tblItemWhse.ItemId = tblItemMaster.ItemID WHERE  tblItemWhse.WhseId='" + cmbWarehouse.Text + "'";

                //StrSql = StrSql + " UNION SELECT tblItemMaster.ItemID,tblItemMaster.ItemDescription,tblItemMaster.UnitPrice,tblItemMaster.PriceLevel1,tblItemMaster.PriceLevel2,tblItemMaster.PriceLevel3,tblItemMaster.PriceLevel4,tblItemMaster.PriceLevel5,0,tblItemMaster.ItemClass,tblItemMaster.SalesGLAccount,tblItemMaster.UOM,tblItemMaster.Categoty,tblItemMaster.UnitCost  FROM tblItemMaster WHERE tblItemMaster.ItemClass IN (5)";

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
                    ultraCombo1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
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
                    ultraCombo1.DisplayLayout.Bands[0].Columns[13].Width = 75;
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                        MessageBox.Show("Invalid Item Code.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (double.Parse(ugR.Cells["Quantity"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Issued Quantity Must be Greater than zero.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if (cmbWarehouse.Text.Trim() == "")
            {
                return false;
            }

            if (cmbAccountType.Text.Trim() == "")
            {
                return false;
            }

            if (cmbWarehouse.Text.Trim() == cmbAccountType.Text.Trim())
            {
                return false;
            }
            return true;
        }

        public void ViewDetails(string StrInvoiceNo)
        {
            try
            {

                StrSql = "SELECT ItemLine,ItemId,ItemDis,QTY,UnitCost,LineTotal,QtyOnHand FROM tblIssueNoteLine   WHERE IssueNoteNo='" + StrInvoiceNo + "' ";//ORDER BY ItemLine";

                //StrSql = "SELECT ItemLine,ItemId,ItemDis,isnull(QTY,0)as QTY,isnull(UnitCost,0)as UnitCost,LineTotal FROM tblIssueNoteLine   WHERE IssueNoteNo='" + StrInvoiceNo + "' ORDER BY ItemLine";
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
                        ugR.Cells["Quantity"].Value = Dr["QTY"];
                        ugR.Cells["UnitPrice"].Value = Dr["UnitCost"];
                        ugR.Cells["TotalPrice"].Value = Dr["LineTotal"];
                        ugR.Cells["OnHand"].Value = Dr["QtyOnHand"];
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
                txtInvoiceNo.Text = "";
                StrSql = "SELECT IssueNoteNo,FrmWhseId,AccountID,IssueDate,NetValue,Description,DocumentType,DepartmentCode FROM tblIssueNoteIC WHERE IssueNoteNo='" + StrInvoiceNo + "'";

                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    
                    txtInvoiceNo.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                    cmbWarehouse.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                    cmbAccountType.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                    dtpDate.Value = DateTime.Parse(dt.Rows[0].ItemArray[3].ToString().Trim());
                    txtNetValue.Value = double.Parse(dt.Rows[0].ItemArray[4].ToString().Trim());
                    txtDescription.Text = (dt.Rows[0].ItemArray[5].ToString().Trim());
                  //  ugcmbDepCode.Value = (dt.Rows[0].ItemArray[7].ToString().Trim());
                    if (dt.Rows[0].ItemArray[6].ToString().Trim() == "IssueNote")
                    {
                        rdoOptIssue.Checked = true;
                    }
                    if (dt.Rows[0].ItemArray[6].ToString().Trim() == "ReturnNote")
                    {
                        rdoOptReturn.Checked = true;
                    }
                    ugcmbDepCode.Value = (dt.Rows[0].ItemArray[7].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void GetCurrentAccountFromGL()
        {
            try
            {
                StrSql = "SELECT IssueNoteCurrentAC FROM tblDefualtSetting";
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    StrGLCurrentAC = dt.Rows[0].ItemArray[0].ToString().Trim();
                    // StrCustId = dt.Rows[0].ItemArray[1].ToString().Trim();
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
                    btnEdit.Enabled = false;
                    dtpDate.Enabled = false;


                    GetDepatemtDataSet();
                    GetItemDataSet();
                    GetWareHouseDataSet();
                    GetAccountData();

                    GetCurrentAccountFromGL();
                    ClearHeader();
                    DeleteRows();
                    EnableHeader(false);
                    EnableFoter(false);
                    GetInvNo();
                    btnNew_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
                    btnSave.Enabled = true;
                    btnNew.Enabled = true;
                    btnPrint.Enabled = false;
                    btnSNO.Enabled = false;
                    btnSearch.Enabled = false;
                    btnReset.Enabled = true;
                    btnEdit.Enabled = false;
                    EnableHeader(true);
                    EnableFoter(true);

                    ClearHeader();
                    DeleteRows();
                    GetInvNo();
                    cmbWarehouse.Focus();

                    //GetWareHouseDataSet();
                    //GetItemDataSet();

                    ug.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
                    //GetWareHouseDataSet();
                    //GetAccountData();
                }
                else
                {
                    MessageBox.Show("You don't have access privilleges for this.Please contact your system administrator for assistance.", "CMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                if (HeaderValidation() == false)
                {
                    return;
                }
                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                //for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                //{
                //    dblAvailableQty = CheckWarehouseItem(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), myConnection, myTrans);
                //    if (rdoOptIssue.Checked == true)
                //    {
                //        if (user.IsMinusAllow == false)
                //        {
                //            if (double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()) > dblAvailableQty)
                //            {
                //                MessageBox.Show("Line No :" + ug.Rows[intGrid].Cells["LineNo"].Value.ToString() + " " + "Insufficient quantity available.", "Message", MessageBoxButtons.OK);
                //                myTrans.Rollback();
                //                return;
                //            }
                //        }
                //    }
                //}
                
                StrReference = GetInvNoField(myConnection, myTrans);
                UpdatePrefixNo(myConnection, myTrans);
                SaveHeader(StrReference, myConnection, myTrans);
                for (intGrid = 0; intGrid < ug.Rows.Count; intGrid++)
                {
                    SaveDetails(StrReference, Int32.Parse(ug.Rows[intGrid].Cells["LineNo"].Value.ToString()), ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["OnHand"].Value.ToString()), myConnection, myTrans);

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

                        UpdateItemWarehouse(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), ug.Rows[intGrid].Cells["Description"].Value.ToString(), cmbWarehouse.Text.Trim(), cmbWarehouse.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), ug.Rows[intGrid].Cells["UOM"].Value.ToString(), double.Parse(ug.Rows[intGrid].Cells["CostPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);
                        InvTransaction(StrReference, dtpDate.Value, ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), cmbWarehouse.Text.Trim(), cmbAccountType.Text.Trim(), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), myConnection, myTrans);

                         for (int j = 0; j < clsSerializeItem.DtsSerialNoList.Rows.Count; j++)
                         {
                             if (clsSerializeItem.DtsSerialNoList.Rows[j][0].ToString() == ug.Rows[intGrid].Cells["ItemCode"].Value.ToString())
                            {
                                if (rdoOptIssue.Checked)
                                {
                                    //Insert item and serialNO into warehousewise Iitem Transaction Table=====================
                                    SqlCommand myCommandSe12 = new SqlCommand("insert into tblSerialItemTransaction(WareHouseID,ItemID,SerialNO,TranType,Status)values ('" +
                                        cmbWarehouse.Value.ToString().Trim() + "','" + ug.Rows[intGrid].Cells["ItemCode"].Value + "','" +
                                        clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() + "','Issue','OutOfStock')", myConnection, myTrans);
                                    myCommandSe12.ExecuteNonQuery();

                                    SqlCommand myCommand = new SqlCommand(
                                        " insert into tblSerialTransfer(TRNNO, ItemID, SerialNO, TransactionType, Status, LocationID, TransDate,IsOut,sTATUS2) " +
                                        " values('" + StrReference + "','" + ug.Rows[intGrid].Cells["ItemCode"].Value + "','" +
                                        clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() +
                                        "','Issue','OutOfStock','" + cmbWarehouse.Value.ToString().Trim() + "','" + dtpDate.Value + "','True','')  ",
                                        myConnection, myTrans);
                                    myCommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    SqlCommand myCommandSe12 = new SqlCommand("insert into tblSerialItemTransaction(WareHouseID,ItemID,SerialNO,TranType,Status)values ('" +
                                        cmbWarehouse.Value.ToString().Trim() + "','" + ug.Rows[intGrid].Cells["ItemCode"].Value + "','" +
                                        clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() + "','Return','Available')", myConnection, myTrans);
                                    myCommandSe12.ExecuteNonQuery();

                                    SqlCommand myCommand = new SqlCommand(
                                        " insert into tblSerialTransfer(TRNNO, ItemID, SerialNO, TransactionType, Status, LocationID, TransDate,IsOut,sTATUS2) " +
                                        " values('" + StrReference + "','" + ug.Rows[intGrid].Cells["ItemCode"].Value + "','" +
                                        clsSerializeItem.DtsSerialNoList.Rows[j].ItemArray[2].ToString().Trim() +
                                        "','Return','Available','" + cmbWarehouse.Value.ToString().Trim() + "','" + dtpDate.Value + "','False','')  ",
                                        myConnection, myTrans);
                                    myCommand.ExecuteNonQuery();
                                }
                            }
                         }
                                                 
                         CreateXmlToExportInvAdjust(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), StrReference, dtpDate.Value, double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), myTrans, myConnection);
                    }
                }

                //if (clsSerializeItem.DtsSerialNoList.Rows.Count > 0)
                //{
                //    if (rdoOptIssue.Checked)
                //    {
                //        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList,
                //            "Issue", cmbWarehouse.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDate.Value, false, "");
                //    }
                //    else
                //    {
                //        frmSerialAddCommon objfrmSerialAddCommon = new frmSerialAddCommon();
                //        objfrmSerialAddCommon.SaveSerialNos_Activity(myConnection, myTrans, clsSerializeItem.DtsSerialNoList,
                //            "Return", cmbWarehouse.Text.ToString(), txtInvoiceNo.Text.ToString().Trim(), dtpDate.Value, true, "");
                //    }
                //}

                //if ((intItemClass == 1) || (intItemClass == 3) || (intItemClass == 8) || (intItemClass == 9) || (intItemClass == 10) || (intItemClass == 11))
                //{
                //    CreateXmlToExportInvAdjust(ug.Rows[intGrid].Cells["ItemCode"].Value.ToString(), StrReference, dtpDate.Value, double.Parse(ug.Rows[intGrid].Cells["UnitPrice"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["Quantity"].Value.ToString()), double.Parse(ug.Rows[intGrid].Cells["TotalPrice"].Value.ToString()), ug.Rows[intGrid].Cells["GL"].Value.ToString(), myTrans, myConnection);
                //}

                myTrans.Commit();
                if (rdoOptIssue.Checked == true)
                {
                    MessageBox.Show("Issue Note Successfuly Saved.", "Information", MessageBoxButtons.OK);
                }
                if (rdoOptReturn.Checked == true)
                {
                    MessageBox.Show("Receive Note Successfuly Saved.", "Information", MessageBoxButtons.OK);
                }
                Print(StrReference);
                //frmViewerTransferNotePrint ObjTransPrint = new frmViewerTransferNotePrint(this);
                //ObjTransPrint.Show();
                ButtonClear();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                throw ex;
            }            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveEvent();
                btnNew_Click(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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

        private void UpdateItemWarehouse(string StrItemCode, string StrItemDesc, string StrWarehouse, string StrWarehouseTo, double dblQty, string StrUOM, double DblUnitCost, double DblTotalCost, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (rdoOptIssue.Checked == true)
                {
                    StrSql = "UPDATE tblItemWhse SET QTY=QTY-" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";
                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                if (rdoOptReturn.Checked == true)
                {
                    //StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouse + "' AND ItemId='" + StrItemCode + "'";

                    StrSql = "SELECT ItemId from tblItemWhse  WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "' ";
                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "'";
                    }
                    else
                    {
                        StrSql = "INSERT INTO [tblItemWhse] ([WhseId],[ItemId],[ItemDis],[QTY],[UOM],[TraDate],[UnitCost],[TranType],[TotalCost],[OPBQtry]) VALUES('" + StrWarehouseTo + "','" + StrItemCode + "','" + StrItemDesc + "'," + dblQty + ",'" + StrUOM + "','" + GetDateTime(DateTime.Now) + "'," + DblUnitCost + ",'Issue'," + DblTotalCost + ",0)";
                    }
                    command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }

                //SqlCommand command = new SqlCommand(StrSql, con, Trans);
                //command.CommandType = CommandType.Text;
                //command.ExecuteNonQuery();

                //StrSql = "SELECT ItemId from tblItemWhse  WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "' ";
                //command = new SqlCommand(StrSql, con, Trans);
                //SqlDataAdapter da = new SqlDataAdapter(command);
                //DataTable dt = new DataTable();
                //da.Fill(dt);

                //if (dt.Rows.Count > 0)
                //{

                //    StrSql = "UPDATE tblItemWhse SET QTY=QTY+" + dblQty + " WHERE WhseId='" + StrWarehouseTo + "' AND ItemId='" + StrItemCode + "'";
                //}
                //else
                //{
                //    StrSql = "INSERT INTO [tblItemWhse] ([WhseId],[ItemId],[ItemDis],[QTY],[UOM],[TraDate],[UnitCost],[TranType],[TotalCost],[OPBQtry]) VALUES('" + cmbAccountType.Value + "','" + StrItemCode + "','" + StrItemDesc + "'," + dblQty + ",'" + StrUOM + "','" + GetDateTime(DateTime.Now) + "'," + DblUnitCost + ",'Trans'," + DblTotalCost + ",0)";
                //}
                //command = new SqlCommand(StrSql, con, Trans);
                //command.CommandType = CommandType.Text;
                //command.ExecuteNonQuery();
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

        private void CreateXmlToExportInvAdjust(string StrItemCode, string strIssueNoteNo, DateTime dtDate,double dblUnitcost, double dblQty, double dblLineTotal, string StrGLCode, SqlTransaction myTrans, SqlConnection myConnection)
        {
            Connector Conn = new Connector();
            XmlTextWriter Writer = new XmlTextWriter(System.Windows.Forms.Application.StartupPath + "\\XMLFILES\\IssueNote.xml", System.Text.Encoding.UTF8);
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

            Writer.WriteStartElement("JobID");
            Writer.WriteAttributeString("xsi:type", "paw:id");
            Writer.WriteString("");
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
           // Writer.WriteString(StrGLCurrentAC);
            Writer.WriteString(cmbAccountType.Text.ToString().Trim());
            Writer.WriteEndElement();

            Writer.WriteStartElement("UnitCost");
            Writer.WriteString(dblUnitcost.ToString());
            Writer.WriteEndElement();

            if (rdoOptIssue.Checked == true)
            {
                Writer.WriteStartElement("Quantity");
                Writer.WriteString("-" + dblQty.ToString());
                Writer.WriteEndElement();
            }

            if (rdoOptReturn.Checked == true)
            {
                Writer.WriteStartElement("Quantity");
                Writer.WriteString(dblQty.ToString());
                Writer.WriteEndElement();
            }


            Writer.WriteStartElement("Amount");
            Writer.WriteString("-" + dblLineTotal.ToString().Trim());
            Writer.WriteEndElement();


            Writer.WriteEndElement();//Adjustment Line
            Writer.WriteEndElement();//Adjustment lines

            Writer.WriteEndElement();//Item Closed Tag
            Writer.Close();//finishing writing xml file

            Conn.IssueAdjustmentExport("IssueNote.xml");
        }
        private void InvTransaction(string strInvoiceNo, DateTime dtDate, String StrItemCode, string StrLocCode, string StrLocCodeTo, double dblQuantity, double dblPrice, double dblLineNetAmt, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                if (dblQuantity != 0)
                {
                    if (rdoOptIssue.Checked == true)
                    {
                        StrSql =
                            " declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + StrLocCode + "' AND ItemId='" + StrItemCode + "') " +
                            " INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(@OHQTY,10,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Issue','false','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                    }
                    if (rdoOptReturn.Checked == true)
                    {
                        StrSql =
                            " declare @OHQTY numeric(18,3) set @OHQTY=(select isnull(QTY,0) from tblItemWhse WHERE WhseId='" + StrLocCode + "' AND ItemId='" + StrItemCode + "') " +
                            "INSERT INTO [tbItemlActivity](OHQTY,[DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(@OHQTY,12,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Return','true','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCode + "',0)";
                    }

                    SqlCommand command = new SqlCommand(StrSql, con, Trans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    //StrSql = "INSERT INTO [tbItemlActivity]([DocType],[TranNo],[TransDate],[TranType],[DocReference],[ItemID],[QTY],[UnitCost],[TotalCost],[WarehouseID],[SellingPrice])VALUES(3,'" + strInvoiceNo + "','" + GetDateTime(dtDate) + "','Tran','true','" + StrItemCode + "'," + dblQuantity + "," + dblPrice + "," + dblLineNetAmt + ",'" + StrLocCodeTo + "',0)";

                    //command = new SqlCommand(StrSql, con, Trans);
                    //command.CommandType = CommandType.Text;
                    //command.ExecuteNonQuery();
                }
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
                StrSql = "INSERT INTO tblIssueNoteIC (IssueNoteNo,FrmWhseId,AccountID,IssueDate,NetValue,Description,DocumentType,DepartmentCode) VALUES('" + StrInvoiceNo + "','" + cmbWarehouse.Value.ToString() + "','" + cmbAccountType.Value + "','" + GetDateTime(dtpDate.Value) + "'," + txtNetValue.Value + ",'" + txtDescription.Text + "','" + StrDocumentType + "','" + ugcmbDepCode.Text.ToString().Trim() + "')";
                SqlCommand command = new SqlCommand(StrSql, con, Trans);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDetails(string StrInvoiceNo, int intLineNo, String StrItemCode, string StrItemDescription, double dblQuantity, string StrUOM, double dblCostPrice, double dblLineTotal,double ONHand, SqlConnection con, SqlTransaction Trans)
        {
            try
            {
                StrSql = "INSERT INTO tblIssueNoteLine (IssueNoteNo,ItemLine,ItemId,ItemDis,QTY,UOM,FrmWhseQTY,ToWhseQTY,UnitCost,LineTotal,QtyOnHand) VALUES('" + StrInvoiceNo + "'," + intLineNo + ",'" + StrItemCode + "','" + StrItemDescription + "'," + dblQuantity + ",'" + StrUOM + "'," + dblQuantity + "," + dblQuantity + "," + dblCostPrice + "," + dblLineTotal + ",'" + ONHand + "')";

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
                _IsFind = true;
                blnEdit = true;
                btnEdit.Enabled = true;
                btnPrint.Enabled = true;

                //frmSeachIssueNote ObjSeisuueN = new frmSeachIssueNote();
                //ObjSeisuueN.ShowDialog();

                if (frmMain.objfrmSeachIssueNote == null || frmMain.objfrmSeachIssueNote.IsDisposed)
                {
                    frmMain.objfrmSeachIssueNote = new frmSeachIssueNote();
                }
                frmMain.objfrmIssueNote.TopMost = false;
                frmMain.objfrmSeachIssueNote.ShowDialog();
                frmMain.objfrmSeachIssueNote.TopMost = true; 

               // setValue();
              //  ug.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
                setValue();
                _IsFind = false;
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptTransferNote.rpt") == true)
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
                        ObjDSIssueNote.Clear();

                       // StrSql = "SELECT * FROM tblIssueNoteIC WHERE IssueNoteNo='" + StrTransNo + "'";
                        StrSql = "select * from tblIssueNoteIC where IssueNoteNo='" + StrTransNo + "'";
                        SqlCommand cmd = new SqlCommand(StrSql);
                        SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(ObjDSIssueNote.dtIssueHeader);

                        StrSql = "SELECT * FROM tblIssueNoteLine WHERE IssueNoteNo='" + StrTransNo + "'";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(ObjDSIssueNote.dtIssueLine);

                        StrSql = "SELECT ItemID,ItemDescription FROM tblItemMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(ObjDSIssueNote.dtItemMaster);

                        StrSql = "SELECT WhseId,WhseName FROM tblWhseMaster";
                        cmd = new SqlCommand(StrSql);
                        da = new SqlDataAdapter(StrSql, ConnectionString);
                        dt = new DataTable();
                        da.Fill(ObjDSIssueNote.dtWarehouse);


                        frmViwerIssueNoteprint ObjPrintIssueNote = new frmViwerIssueNoteprint(this);
                        ObjPrintIssueNote.Show();
                        //frmViewerTransferNotePrint ObjTransPrint = new frmViewerTransferNotePrint(this);
                        //ObjTransPrint.Show();
                    }
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
                Print(txtInvoiceNo.Text);
                // DirectPrint();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }


        private void ClearFooter()
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearHeader()
        {
            try
            {
                cmbWarehouse.Text = user.StrDefaultWH;
                txtDescription.Text = "";
                txtDepName.Text = "";
                dtpDate.Value = user.LoginDate;
                //txtWarehouseName.Text = "";
                //txtToWarehouseName.Text = "";
                cmbAccountType.Text = user.RetrnIssuGL;

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
                cmbAccountType.Enabled = blnEnable;
                dtpDate.Enabled = blnEnable;
                txtWarehouseName.Enabled = false;
                txtToWarehouseName.Enabled = false;
                // ug.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
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
                GetInvNo();
                //ug.Enabled = true;
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                    //if (e.Cell.Row.Cells["UnitPrice"].Value == null || e.Cell.Row.Cells["UnitPrice"].Value.ToString().Trim().Length==0) e.Cell.Row.Cells["UnitPrice"].Value = "0";
                    //if (e.Cell.Row.Cells["Quantity"].Value == null || e.Cell.Row.Cells["Quantity"].Value.ToString().Trim().Length == 0) e.Cell.Row.Cells["Quantity"].Value = "0";
                    e.Cell.Row.Cells["TotalPrice"].Value = LineCalculation(Convert.ToDouble(e.Cell.Row.Cells["UnitPrice"].Value), Convert.ToDouble(e.Cell.Row.Cells["Quantity"].Value));
                    GrandTotal();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                //ug.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                if (ug.Rows.Count > 0)
                {
                    if (!_IsFind)
                    {
                        DialogResult reply = MessageBox.Show("Are you sure, you want to channge Warehouse?", "Information", MessageBoxButtons.OK);

                        if (reply == DialogResult.OK)
                        {
                            DeleteRows();
                            ClearFooter();
                        }
                    }
                }

                GetItemDataSet();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                                ug.ActiveCell.Row.Cells["Quantity"].Value = 1;
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
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                        txtToWarehouseName.Text = cmbAccountType.ActiveRow.Cells[1].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
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
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }     

        

        private void ug_KeyDown_1(object sender, KeyEventArgs e)
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

        private void ug_InitializeRow_1(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["LineNo"].Value = e.Row.Index + 1;
        }

        private void ug_Click_1(object sender, EventArgs e)
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_BeforeExitEditMode_1(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
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
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_AfterCellUpdate_1(object sender, CellEventArgs e)
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
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoOptIssue_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                GetInvNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void rdoOptReturn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                GetInvNo();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_AfterRowsDeleted_1(object sender, EventArgs e)
        {
            try
            {
                GrandTotal();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ugcmbDepCode_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    if (e.Row.Activated == true)
                    {
                        txtDepName.Text = ugcmbDepCode.ActiveRow.Cells[1].Value.ToString();
                        cmbAccountType.Text = ugcmbDepCode.ActiveRow.Cells[3].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Issue/Return Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbWarehouse.Text == string.Empty)
                {
                    MessageBox.Show("Please Select a Warehouse First");
                    return;
                }

                if (Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()) == 0)
                {
                    DialogResult reply1 = MessageBox.Show("Please enter a quantity before trying to add serial numbers for Item ID '" + 
                        ug.ActiveRow.Cells["Quantity"].Value.ToString() + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    {
                        if (reply1 == DialogResult.OK)
                        {
                            ug.ActiveRow.Cells["Quantity"].Selected = true;
                        }
                    }
                }
                else
                {
                    if (rdoOptIssue.Checked)
                    {
                        frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Issue",
                               cmbWarehouse.Text.ToString().Trim(),
                          ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                          Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                          txtInvoiceNo.Text.Trim(), false, clsSerializeItem.DtsSerialNoList, null, false, true);
                        ObjfrmSerialSubCommon.ShowDialog();
                    }
                    else
                    {
                        frmSerialSubCommon ObjfrmSerialSubCommon = new frmSerialSubCommon("Return",
                            cmbWarehouse.Text.ToString().Trim(),
                        ug.ActiveRow.Cells["ItemCode"].Value.ToString(),
                          Convert.ToDouble(ug.ActiveRow.Cells["Quantity"].Value.ToString()),
                       txtInvoiceNo.Text.Trim(), false, clsSerializeItem.DtsSerialNoList, null, false, true);
                        ObjfrmSerialSubCommon.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Good Receive Note", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ug_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void ultraCombo1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }




























    }
}