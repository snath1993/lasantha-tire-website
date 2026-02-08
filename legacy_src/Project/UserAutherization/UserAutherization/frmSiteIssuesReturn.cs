using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBLL;
using PCMBeans;
using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using ComFunction;
using System.Data.SqlClient;

namespace UserAutherization
{
    public partial class frmSiteIssuesReturn : Form
    {
        public static string ConnectionString;
        public dsRptIssueNote objdsRptIssueNote = new dsRptIssueNote();
        clsCommon objclsCommon = new clsCommon();
        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "Site Issues Return";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        DataTable _DocumentList;
        clsCommonFunc objclsCommonFunc = new clsCommonFunc();

        public frmSiteIssuesReturn()
        {
            InitializeComponent();
            ucmbReturnNo.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();
        }

        public frmSiteIssuesReturn(string RetNoteNo)
        {
            InitializeComponent();
            ucmbReturnNo.Value = RetNoteNo;
            fillControls();
            FormMode = enmFormMode.Find;
            SetFormMode();
        }

        public void Discard()
        {
            foreach (UltraGridRow ugR in udgvSubcontractors.Rows.All)
            {
                ugR.Delete(false);
            }
        }

        private void ClearControlers()
        {
            ucmbReturnNo.Text = "";
            ucmbBOMID.Text = "";
            ucmbIssueID.Text = "";
            ucmbPhaseID.Text = "";
            txtDescription.Text = "";
            ucmbSiteID.Text = "";
            txtQtyBOMTemp.Value = 0.00;
            txtQtyOHTemp.Value = 0.00;
            txtQtyIssueTemp.Value = 0.00;
            ucmbCustomerID.Text = "";
            txtCustomerName.Text = "";

            dtpDate.Value = user.LoginDate;         

            Discard();
            objclsCommonFunc.fillReturns(ucmbReturnNo);
            objclsCommonFunc.fillBOMs_Active(ucmbBOMID);
            objclsCommonFunc.fillCustomer(ucmbCustomerID);
            objclsCommonFunc.fillIssues(ucmbIssueID);
            objclsCommonFunc.fillItems(ucmbItems);
            objclsCommonFunc.fillPhases(ucmbPhaseID);
            objclsCommonFunc.fillSites(ucmbSiteID);
            objclsCommonFunc.fillSubPhases(ucmbSubPhaseID);

            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

            _DocumentList = null;          
        }

        private void SetFormMode()
        {
            switch (FormMode)
            {
                case enmFormMode.Initialize:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Save:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Delete:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
                case enmFormMode.Find:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = true;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    break;
                case enmFormMode.Clear:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
            }
        }

        private bool IsValidControls()
        {
            if (!user.IsJOBReturnNOAutoGen)
            {
                if (string.IsNullOrEmpty(ucmbReturnNo.Text.Trim()))
                {
                    erpMaster.SetError(ucmbReturnNo, "Return No Can't be Empty");
                    ucmbReturnNo.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(ucmbIssueID.Text.Trim()))
            {
                erpMaster.SetError(ucmbIssueID, "Issue Note No Can't be Empty");
                ucmbIssueID.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(ucmbSiteID.Text.Trim()))
            {
                erpMaster.SetError(ucmbSiteID, "Site ID Can't be Empty");
                ucmbSiteID.Focus();
                return false;
            }
            if (!IsValidQtyReferingIssue())
            {
                return false;
            }
            if (ucmbIssueID.Value != null && ucmbIssueID.Text.Trim() != string.Empty)
            {
                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0
                        && dgvr.Cells["colReturnQty"].Value != null && dgvr.Cells["colReturnQty"].Value.ToString().Trim().Length > 0)
                    {
                        if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                        {
                            if (double.Parse(dgvr.Cells["colReturnQty"].Value.ToString()) > 0)
                            {
                                if (double.Parse(dgvr.Cells["colReturnQty"].Value.ToString()) > double.Parse(dgvr.Cells["colIssueQty"].Value.ToString()))
                                {
                                    MessageBox.Show("You are going to Return More than Issue Qty....!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            else
                return true;
        }

        private bool IsValidQtyReferingIssue()
        {
            try
            {
                DataSet _dts = new DataSet();
                _dts = objclsBLLPhases.GetToBeIssuedQty(ucmbBOMID.Value.ToString().Trim());

                foreach (DataRow dr in _dts.Tables[0].Rows)
                {
                    double _IssQty = 0;
                    double _IssuedQTY = double.Parse(dr["IssuedQty"].ToString()) - double.Parse(dr["ReturnedQty"].ToString());

                    foreach (UltraGridRow udr in udgvSubcontractors.Rows)
                    {
                        if (dr["PhaseID"].ToString() == udr.Cells["colPhase"].Value.ToString() &&
                            dr["SubPhaseID"].ToString() == udr.Cells["colSubPhase"].Value.ToString() &&
                            dr["ItemID"].ToString() == udr.Cells["colItem"].Value.ToString())
                        {
                            _IssQty = _IssQty + double.Parse(udr.Cells["colReturnQty"].Value.ToString().Trim());
                        }
                        if (_IssuedQTY < _IssQty)
                        {
                            MessageBox.Show("You are going to Issue more than Issued Qty for " + dr["PhaseID"].ToString() + ", " + dr["SubPhaseID"].ToString() + ", " + dr["ItemID"].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        //
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void frmBOQ_Load(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbPhaseID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                //ucmbSubPhaseID.DataSource = null;

                if (ucmbPhaseID.Value != null)
                {
                    ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_ByPhaseID(ucmbPhaseID.Value.ToString());
                    ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                    ucmbSubPhaseID.ValueMember = "SubPhaseID";
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            SetFormMode();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                //if(

                if (IsValidControls())
                {
                    if (ucmbReturnNo.Text.Trim().Length > 0)
                    {
                        MessageBox.Show("Not Allowed to Save again....!");
                        return;
                    }

                    objclsBeansPhases.CustomerID = ucmbCustomerID.Text.Trim();
                    objclsBeansPhases.ReturnNo = ucmbReturnNo.Text.Trim();
                    objclsBeansPhases.IssueNo = ucmbIssueID.Text.Trim();
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    //objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    if (objclsBeansPhases.Dtbl.Rows.Count == 0)
                    {
                        MessageBox.Show("No Items To Return....!");
                        return;
                    }
                    objclsBeansPhases.DtblPT = getDatasource_InGridPT();
                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;

                    string _ReturnsID = objclsBLLPhases.SaveReturns(objclsBeansPhases,true);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                    ucmbReturnNo.Text = _ReturnsID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private double GetTotals(double CostOfSAlesAmt,string SubPhase,string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + CostOfSAlesAmt;
                }
            }
            return _Amt;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetReturns_All();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Site Return");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbReturnNo.Value = clsBeansFind.ReturnValue;
                    fillControls();
                    FormMode = enmFormMode.Find;
                    SetFormMode();
                }
                else
                {
                    FormMode = enmFormMode.Initialize;
                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void fillControls()
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();
            try
            {                
                objclsBeansPhases = objclsBLLPhases.GetReturns_ByID(ucmbReturnNo.Text.Trim());
                if (objclsBeansPhases.ReturnNo != null)
                {
                    ucmbReturnNo.Value = objclsBeansPhases.ReturnNo;
                    //txtActualAmount.Value = objclsBeansPhases.ActualAmt;
                    txtDescription.Text = objclsBeansPhases.Description;
                    dtpDate.Value = objclsBeansPhases.Date;
                    ucmbSiteID.Value = objclsBeansPhases.SiteID;
                    ucmbIssueID.Value = objclsBeansPhases.IssueNo;
                    ucmbBOMID.Value = objclsBeansPhases.BOMID;

                    chkActive.Checked = objclsBeansPhases.Inactive;

                    fillGrid(objclsBeansPhases.Dtbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillControls_Issue()
        {
            objclsBeansPhases = new clsBeansPhases();
            try
            {
                objclsBeansPhases = objclsBLLPhases.GetIssues_ByID(ucmbIssueID.Text.Trim());
                //ucmbReturnNo.Value = objclsBeansPhases.ReturnNo;
                //txtActualAmount.Value = objclsBeansPhases.ActualAmt;
                //txtDescription.Text = objclsBeansPhases.Description;
                //dtpDate.Value = objclsBeansPhases.Date;
                ucmbSiteID.Value = objclsBeansPhases.SiteID;
                ucmbIssueID.Value = objclsBeansPhases.IssueNo;
                ucmbBOMID.Value = objclsBeansPhases.BOMID;

                //chkActive.Checked = objclsBeansPhases.Inactive;

                fillIssuesGrid(objclsBeansPhases.Dtbl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillIssuesGrid(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    if (double.Parse(dr["IssuedQty"].ToString()) > 0)
                    {
                        ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                        udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                        udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                        udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemID"].ToString();
                        udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                        //udgvSubcontractors.Rows[_Index].Cells["colReturnQty"].Value = dr["ReturnQty"].ToString();
                        udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                        udgvSubcontractors.Rows[_Index].Cells["colOnHandQty"].Value = double.Parse(dr["OnHandAtIssue"].ToString()).ToString("0.00");
                        udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["BOMQty"].ToString()).ToString("0.00");
                        udgvSubcontractors.Rows[_Index].Cells["colIssueQty"].Value = (double.Parse(dr["IssuedQty"].ToString()) - double.Parse(dr["ReturnQty"].ToString())).ToString("0.00");
                        udgvSubcontractors.Rows[_Index].Cells["colBalance"].Value = double.Parse(dr["BOMBalanceQty"].ToString()).ToString("0.00");
                        //udgvSubcontractors.Rows[_Index].Cells["colDelete"].Value = "Delete";
                        _Index = _Index + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillGrid(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colReturnQty"].Value = dr["ReturnedQty"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colOnHandQty"].Value = double.Parse(dr["OnHandAtReturn"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["BOMQty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colIssueQty"].Value = (double.Parse(dr["IssuedQty"].ToString()) - double.Parse(dr["ReturnedQty"].ToString())).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colBalance"].Value = double.Parse(dr["BOMBalanceQty"].ToString()).ToString("0.00");
                    //udgvSubcontractors.Rows[_Index].Cells["colDelete"].Value = "Delete";
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Delete;
                objclsBLLPhases = new clsBLLPhases();
                objclsBeansPhases = new clsBeansPhases();

                if (IsValidControls())
                {
                    objclsBeansPhases.ReturnNo = ucmbReturnNo.Text.Trim();

                    objclsBLLPhases.DeleteReturns(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable getDatasource_InGrid()
        {
            DataTable _dataTable = new DataTable();
            clsBLLPhases obj=new clsBLLPhases ();
            clsBeansPhases _objclsBeansPhases=new clsBeansPhases ();
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            double Qty = 0;
            double Amt = 0;

            try
            {
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("PhaseID");
                    _dataTable.Columns.Add("SubPhaseID");
                    _dataTable.Columns.Add("Balance");
                    _dataTable.Columns.Add("LocationID");
                    _dataTable.Columns.Add("Estimated");
                    _dataTable.Columns.Add("Units");
                    _dataTable.Columns.Add("Qty");
                    _dataTable.Columns.Add("Issued");
                    _dataTable.Columns.Add("Item");
                    _dataTable.Columns.Add("OH");
                    _dataTable.Columns.Add("GL");
                    _dataTable.Columns.Add("CostofSales");
                    _dataTable.Columns.Add("ItemClass");
                    _dataTable.Columns.Add("ItemDesc");
                    _dataTable.Columns.Add("LastUnitCost");
                    _dataTable.Columns.Add("Returned");

                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0
                            && dgvr.Cells["colReturnQty"].Value != null && dgvr.Cells["colReturnQty"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                            {
                                if (double.Parse(dgvr.Cells["colReturnQty"].Value.ToString()) > 0)
                                {
                                    DataRow drow = _dataTable.NewRow();
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Text;

                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Text;
                                    drow["Item"] = dgvr.Cells["colItem"].Text;
                                    DataSet _dtsitem = obj.GetItem_By_ItemID(dgvr.Cells["colItem"].Text);

                                    drow["LastUnitCost"] = objclsBBLPTImport.ImportItemUnitCost(dgvr.Cells["colItem"].Text);

                                    drow["ItemDesc"] = _dtsitem.Tables[0].Rows[0]["ItemDis"].ToString();
                                    drow["ItemClass"] = _dtsitem.Tables[0].Rows[0]["ItemClass"].ToString();
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Text;

                                    //if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                        drow["Estimated"] = "0";
                                    //else
                                    //    drow["Estimated"] = dgvr.Cells["colEstQty"].Text;

                                    drow["Units"] = dgvr.Cells["colUnits"].Text;

                                    //if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                    //    dgvr.Cells["colIssueQty"].Text = 0;
                                    DataSet _dstIssued = objclsBLLPhases.getIssuedetails_BySiteID_Phase_SubPhase(ucmbSiteID.Text, dgvr.Cells["colPhase"].Text, dgvr.Cells["colSubPhase"].Text);

                                    //if (_dstIssued.Tables.Count > 0 && _dstIssued.Tables[0].Rows.Count > 0)
                                    //{
                                    //    Amt = double.Parse(_dstIssued.Tables[0].Rows[0]["Amount"].ToString());
                                    //    Qty = double.Parse(_dstIssued.Tables[0].Rows[0]["Qty"].ToString());
                                    //}

                                    drow["Issued"] = double.Parse(dgvr.Cells["colIssueQty"].Text);

                                    drow["Returned"] = double.Parse(dgvr.Cells["colReturnQty"].Text);

                                    //if (dgvr.Cells["colBalance"].Text == null || dgvr.Cells["colBalance"].Text.Trim().Length == 0)
                                        drow["Balance"] = 0;
                                    //else
                                    //    drow["Balance"] = dgvr.Cells["colBalance"].Text;

                                    //if (dgvr.Cells["colOnHandQty"].Text.Trim().Length > 0)
                                    //    drow["OH"] = dgvr.Cells["colOnHandQty"].Text;
                                    //else
                                        drow["OH"] = 0;
                                    //DataSet _dts=obj.GetWereHouse_By_ID(dgvr.Cells["colLocation"].Text);
                                    DataSet _dts = obj.GetItemMaster_By_ItemID(dgvr.Cells["colItem"].Text);
                                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                                        drow["CostofSales"] = double.Parse(_dts.Tables[0].Rows[0]["UnitCost"].ToString()) + Amt;
                                    else
                                        drow["CostofSales"] = 0 + Amt;
                                    drow["GL"] = _dts.Tables[0].Rows[0]["SalesGLAccount"].ToString();
                                    _dataTable.Rows.Add(drow);

                                    //_TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        private DataTable getDatasource_InGridPT()
        {
            DataTable _dataTable = new DataTable();
            clsBLLPhases obj = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            double Qty = 0;
            double Amt = 0;

            try
            {
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("PhaseID");
                    _dataTable.Columns.Add("SubPhaseID");
                    _dataTable.Columns.Add("Balance");
                    _dataTable.Columns.Add("LocationID");
                    _dataTable.Columns.Add("Estimated");
                    _dataTable.Columns.Add("Units");
                    _dataTable.Columns.Add("Qty");
                    _dataTable.Columns.Add("Issued");
                    _dataTable.Columns.Add("Item");
                    _dataTable.Columns.Add("OH");
                    _dataTable.Columns.Add("GL");
                    _dataTable.Columns.Add("CostofSales");
                    _dataTable.Columns.Add("ItemClass");
                    _dataTable.Columns.Add("ItemDesc");
                    _dataTable.Columns.Add("LastUnitCost");
                    _dataTable.Columns.Add("Returned");

                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0
                            && dgvr.Cells["colReturnQty"].Value != null && dgvr.Cells["colReturnQty"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                            {
                                if (double.Parse(dgvr.Cells["colReturnQty"].Value.ToString()) > 0)
                                {                                  
                                    DataRow drow = _dataTable.NewRow();
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Text;

                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Text;
                                    drow["Item"] = dgvr.Cells["colItem"].Text;
                                    DataSet _dtsitem = obj.GetItem_By_ItemID(dgvr.Cells["colItem"].Text);

                                    drow["LastUnitCost"] = objclsBBLPTImport.ImportItemUnitCost(dgvr.Cells["colItem"].Text);

                                    drow["ItemDesc"] = _dtsitem.Tables[0].Rows[0]["ItemDis"].ToString();
                                    drow["ItemClass"] = _dtsitem.Tables[0].Rows[0]["ItemClass"].ToString();
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Text;

                                    //if (dgvr.Cells["colEstQty"].Text == null || dgvr.Cells["colEstQty"].Text.Trim().Length == 0)
                                        drow["Estimated"] = "0";
                                    //else
                                    //    drow["Estimated"] = dgvr.Cells["colEstQty"].Text;

                                    drow["Units"] = dgvr.Cells["colUnits"].Text;

                                    DataSet _dstIssued = objclsBLLPhases.getIssuedetails_BySiteID_Phase_SubPhase(ucmbSiteID.Text, dgvr.Cells["colPhase"].Text, dgvr.Cells["colSubPhase"].Text);

                                    double _exstQty = GetTotals_Qty(dgvr.Cells["colSubPhase"].Value.ToString(), dgvr.Cells["colPhase"].Value.ToString());
                                    if (_dstIssued.Tables.Count > 0 && _dstIssued.Tables[0].Rows.Count > 0)
                                    {
                                        Amt = double.Parse(_dstIssued.Tables[0].Rows[0]["Amount"].ToString());
                                        Qty = double.Parse(_dstIssued.Tables[0].Rows[0]["Qty"].ToString());
                                    }

                                    drow["Returned"] = double.Parse(dgvr.Cells["colReturnQty"].Text);
                                    drow["Issued"] = double.Parse(dgvr.Cells["colIssueQty"].Text);

                                    //if (dgvr.Cells["colBalance"].Text == null || dgvr.Cells["colBalance"].Text.Trim().Length == 0)
                                        drow["Balance"] = 0;
                                    //else
                                    //    drow["Balance"] = dgvr.Cells["colBalance"].Text;

                                    //if (dgvr.Cells["colOnHandQty"].Text.Trim().Length > 0)
                                    //    drow["OH"] = dgvr.Cells["colOnHandQty"].Text;
                                    //else
                                        drow["OH"] = 0;
                                    //DataSet _dts=obj.GetWereHouse_By_ID(dgvr.Cells["colLocation"].Text);

                                    DataSet _dts = obj.GetItemMaster_By_ItemID(dgvr.Cells["colItem"].Text);
                                    if (_dts.Tables.Count > 0 && _dts.Tables[0].Rows.Count > 0)
                                        drow["CostofSales"] = double.Parse(_dts.Tables[0].Rows[0]["UnitCost"].ToString()) + Amt;
                                    else
                                        drow["CostofSales"] = 0 + Amt;
                                    drow["GL"] = _dts.Tables[0].Rows[0]["CostOfSalesAcc"].ToString(); _dataTable.Rows.Add(drow);

                                    //_TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        private double GetTotals_Amt(string SubPhase, string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + double.Parse(udr.Cells["colAmount"].Value.ToString());
                }
            }
            return _Amt;
        }

        private double GetTotals_Qty(string SubPhase, string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvSubcontractors.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + double.Parse(udr.Cells["colReturnQty"].Value.ToString());
                }
            }
            return _Amt;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                frmBOQ_Load(sender, e);
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvSubcontractors_AfterCellUpdate(object sender, CellEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                if (e.Cell == null) return;
                ugR = udgvSubcontractors.ActiveRow;
                DataTable _dtblItemOH = new DataTable();

                if (e.Cell.Column.Key == "colItem")
                {
                    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                    {
                        objDataSet = objclsBLLPhases.GetItem_By_ItemID(ugR.Cells["colItem"].Value.ToString());

                        if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                        {
                            ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["UOM"].ToString();
                            //ugR.Cells["colRate"].Value = objDataSet.Tables[0].Rows[0]["UnitCost"].ToString();
                        }

                        ucmbLocCode.DataSource = null;
                        ucmbLocCode.DataSource = objclsBLLPhases.GetWarehouses_By_ItemID(ugR.Cells["colItem"].Value.ToString());
                        ucmbLocCode.DisplayMember = "WhseId";
                        ucmbLocCode.ValueMember = "WhseId";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Width = 100;
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
                        ucmbLocCode.DisplayLayout.Bands[0].Columns[2].Header.Caption = "OnHand Qty.";
                    }
                }


                //if (e.Cell.Column.Key == "colItem" || e.Cell.Column.Key == "colLocation")
                //{
                //    if (ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                //    {
                //        _dtblItemOH = new DataTable();
                //        _dtblItemOH = objclsBLLPhases.GetItems_By_WH_ItemCode(ugR.Cells["colLocation"].Value.ToString().Trim(), ugR.Cells["colItem"].Value.ToString().Trim()).Tables[0];
                //        if(_dtblItemOH.Rows.Count > 0)
                //            ugR.Cells["colOnHandQty"].Value = double.Parse(_dtblItemOH.Rows[0]["QTY"].ToString()).ToString("0.00");
                //    }
                //}
                if (FormMode != enmFormMode.Find)
                {
                    if (e.Cell.Column.Key == "colItem" || e.Cell.Column.Key == "colLocation")
                    {
                        if (ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0
                            && ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                        {
                            _dtblItemOH = new DataTable();
                            _dtblItemOH = objclsBLLPhases.GetItems_By_WH_ItemCode(ugR.Cells["colLocation"].Value.ToString().Trim(), ugR.Cells["colItem"].Value.ToString().Trim()).Tables[0];
                            if (_dtblItemOH.Rows.Count > 0)
                            {
                                ugR.Cells["colOnHandQty"].Value = double.Parse(_dtblItemOH.Rows[0]["QTY"].ToString()).ToString("0.00");
                            }
                        }
                    }

                    //if (e.Cell.Column.Key == "colReturnQty")
                    //{
                    //    if (ugR.Cells["colEstQty"].Value != null && ugR.Cells["colEstQty"].Value.ToString().Trim().Length > 0
                    //        && ugR.Cells["colReturnQty"].Value != null && ugR.Cells["colReturnQty"].Value.ToString().Trim().Length > 0)
                    //    {
                    //        double _TempBalance = 0.00;

                    //        _TempBalance = double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colReturnQty"].Value.ToString().Trim());

                    //        if (_TempBalance < 0)
                    //        {
                    //            ugR.Cells["colReturnQty"].Value = 0.00;
                    //            MessageBox.Show("Issuing Quantity is Invalid.....!");
                    //            return;
                    //        }

                    //        ugR.Cells["colBalance"].Value = (double.Parse(ugR.Cells["colEstQty"].Value.ToString().Trim()) - double.Parse(ugR.Cells["colIssueQty"].Value.ToString().Trim())).ToString("0.00");
                    //    }
                    //}

                }

                //if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                //{
                //    if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                //        ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                //    {
                //        ugR.Cells["colAmount"].Value = double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString());
                //    }
                //}
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbLocCode_Enter(object sender, EventArgs e)
        {
            ucmbLocCode.PerformAction(UltraComboAction.Dropdown);
            //ucmbLocCode.ToggleDropdown(
        }

        private void ucmbLocCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ucmbLocCode.PerformAction(UltraComboAction.Dropdown);

        }

        private void udgvSubcontractors_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int _currntIndex = 0;
                _currntIndex = udgvSubcontractors.ActiveRow.Index;

                if (udgvSubcontractors.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvSubcontractors.ActiveCell.Column.Key == "colReturnQty")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        if (GetNoOfEmptyRows() < 2)
                        {
                            ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                            udgvSubcontractors.ActiveCell = udgvSubcontractors.Rows[_currntIndex + 1].Cells[0];
                            udgvSubcontractors.PerformAction(UltraGridAction.PrevCell, true, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            erpMaster.Dispose();
        }

        private void ucmbSiteID_KeyPress(object sender, KeyPressEventArgs e)
        {
            erpMaster.Dispose();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void ucmbSiteID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbSiteID.Value != null)
                {
                    _objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    ucmbCustomerID.Value = _objclsBeansPhases.CustomerID;
                    //objclsCommonFunc.fillBOMs_Active(
                    objclsCommonFunc.fillCustomer(ucmbCustomerID, ucmbSiteID);
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            //clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void fillBOMGrid_ON_BOQ_SELECTION(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colActivity"].Value = dr["Activity"].ToString();
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ucmbBOMID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            clsBLLPhases objclsBLLPhases = new clsBLLPhases();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (FormMode != enmFormMode.Find)
                {
                    if (ucmbBOMID.Value != null && ucmbBOMID.Value.ToString().Trim().Length > 0)
                    {
                        _objclsBeansPhases = objclsBLLPhases.GetBOMs_ByID_Max(ucmbBOMID.Text.Trim(), 0);
                        ////fillGrid_On_BOMID(_objclsBeansPhases.Dtbl);
                        ucmbSiteID.Text = _objclsBeansPhases.SiteID;
                    }
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void fillGrid_On_BOMID(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            DataTable _dtblItemOH = new DataTable();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["Qty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colEstQty"].Value = double.Parse(dr["Qty"].ToString()).ToString("0.00"); 
                    udgvSubcontractors.Rows[_Index].Cells["colBalance"].Value = (double.Parse(dr["Qty"].ToString()) - double.Parse(dr["IssuedQty"].ToString())).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colOnHandQty"].Value = double.Parse(dr["OHQty"].ToString()).ToString("0.00");
                    udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    _Index = _Index + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ucmbIssueID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (ucmbIssueID.Value != null && ucmbIssueID.Value.ToString().Trim().Length > 0)
                {
                    if(FormMode != enmFormMode.Find)
                        fillControls_Issue();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbBOMID_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (ucmbBOMID.Value != null && ucmbBOMID.Value.ToString().Trim().Length > 0)
                {
                    frmBOM objfrmBOM = new frmBOM(ucmbBOMID.Value.ToString().Trim());
                    objfrmBOM.Show();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSiteID_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (ucmbSiteID.Value != null && ucmbSiteID.Value.ToString().Trim().Length > 0)
                {
                    frmJob objfrmJob = new frmJob(ucmbSiteID.Value.ToString().Trim());
                    objfrmJob.Show();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnLast_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "RR");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void tsbtnFirst_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "LL");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnPrevious_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "L");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsbtnNext_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbIssueID.Text = objclsBLLPhases.GetCodeforArrow("Issue", ucmbIssueID.Text.Trim(), "R");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                clsBeansPhases.DtblAttachment = _DocumentList;
                frmUpload objfrmUpload = new frmUpload();
                objfrmUpload.ShowDialog();

                if (clsBeansPhases.DtblAttachment != null)
                {
                    if (clsBeansPhases.DtblAttachment.Rows.Count > 0)
                    {
                        _DocumentList = clsBeansPhases.DtblAttachment;
                        clsBeansPhases.DtblAttachment = null;
                    }
                    else
                        _DocumentList = null;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSiteID_ValueChanged(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            //clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_ValueChanged(object sender, EventArgs e)
        {
            clsBLLPhases _oobjclsBLLPhases = new clsBLLPhases();
            //objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    txtCustomerName.Text = _oobjclsBLLPhases.GetCustomer_ByID(ucmbCustomerID.Text.Trim().ToString()).Description;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                if (IsValidControls())
                {
                    if (ucmbReturnNo.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("Save this first...!", "Return Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    objclsBeansPhases.ReturnNo = ucmbReturnNo.Text.Trim();
                    objclsBeansPhases.IssueNo = ucmbIssueID.Text.Trim();
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    //objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;

                    string _ReturnsID = objclsBLLPhases.SaveReturns(objclsBeansPhases, false);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK);

                    MessageBox.Show("Edit this record in Peachtree....!","Reminder",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                    SetFormMode();
                    ucmbReturnNo.Text = _ReturnsID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private int GetNoOfEmptyRows()
        {
            int _NoOfEmptyRows = 0;

            try
            {
                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if ((dgvr.Cells["colPhase"].Value == null || dgvr.Cells["colPhase"].Value.ToString() == string.Empty)
                        && (dgvr.Cells["colSubPhase"].Value == null || dgvr.Cells["colSubPhase"].Value.ToString() == string.Empty))
                    {

                        _NoOfEmptyRows = _NoOfEmptyRows + 1;
                        //DataRow drow = _dataTable.NewRow();
                        //drow["Item"] = "";
                        //drow["PhaseID"] = dgvr.Cells["colPhase"].Value;
                        //drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Value;
                        //drow["Activity"] = dgvr.Cells["colActivity"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _NoOfEmptyRows;
        }

        private void udgvSubcontractors_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            try
            {
                if (GetNoOfEmptyRows() >= 2)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbReturnNo_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (ucmbReturnNo.Value != null && ucmbReturnNo.Value.ToString().Trim().Length > 0)
                {
                    FormMode = enmFormMode.Find;
                    fillControls();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            objdsRptIssueNote.Clear();

            try
            {
                if (ucmbIssueID.Text.Trim() != string.Empty)
                {
                    String S12 = "Select * from viewRptReturnNote where ReturnNo='" + ucmbReturnNo.Text.Trim() + "'";
                    SqlCommand cmd12 = new SqlCommand(S12);
                    SqlConnection con12 = new SqlConnection(ConnectionString);
                    SqlDataAdapter da12 = new SqlDataAdapter(S12, con12);
                    da12.Fill(objdsRptIssueNote, "dtblReturnNote");

                    frmViewerJobActual printax = new frmViewerJobActual(objdsRptIssueNote, "rptSiteReturnNote");
                    printax.Show();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Site Issue", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
       

    }
}