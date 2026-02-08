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

namespace UserAutherization
{
    public partial class frmBOM : Form
    {
        clsCommon objclsCommon = new clsCommon();

        clsCommonFunc objclsCommonFunc = new clsCommonFunc();

        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "BOM";
        DataSet objDataSet;

        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        DataTable _DocumentList;
        string _TempPhaseID = "";
        string _TempSubPhaseID = "";

        public frmBOM()
        {
            InitializeComponent();
            ucmbBOMID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();  
        }

        public frmBOM(string _BOMID)
        {
            InitializeComponent();

            try
            {
                if (_BOMID.ToString().Trim().Length > 0)
                {
                    ucmbBOMID.Value = _BOMID;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message,"Load", ex.StackTrace);
            }
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
            erpMaster.Dispose();
            ucmbBOMID.Text = "";
            ucmbBOQID.Text = "";
            ucmbPhaseID.Text = "";
            txtDescription.Text = "";
            txtActualAmount.Value = 0.00;
            txtEstimatedAmount.Value = 0.00;
            txtRevisionNo.Value = 0;
            ucmbSiteID.Text = "";
            ucmbBOQID.Text = "";
            ucmbCustomerID.Text = "";
            txtCustomerName.Text = "";
            chkAdd.Checked = false;

            Discard();

            dtpDate.Value = user.LoginDate;

            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

            objclsCommonFunc.fillBOMs(ucmbBOMID);
            objclsCommonFunc.fillBOQs_Active(ucmbBOQID);
            objclsCommonFunc.fillCustomer(ucmbCustomerID);
            objclsCommonFunc.fillItems(ucmbItems);
            objclsCommonFunc.fillPhases(ucmbPhaseID);
            objclsCommonFunc.fillSubPhases(ucmbSubPhaseID);
            objclsCommonFunc.fillSites(ucmbSiteID);

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

        private bool IsValidGrid()
        {
            try
            {
                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if (dgvr.Cells["colLocation"].Value != null && dgvr.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                    {
                        if (dgvr.Cells["colLocation"].Value == null || dgvr.Cells["colLocation"].Value.ToString().Trim().Length == 0)
                        {
                            MessageBox.Show("Select the Werehouse for " + dgvr.Cells["colItem"].Value.ToString() + "......!");
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

        private bool IsValidControls()
        {
            if (FormMode == enmFormMode.Save)
            {
                if (!user.IsBOMNOAutoGen)
                {
                    if (string.IsNullOrEmpty(ucmbBOMID.Text.Trim()))
                    {
                        erpMaster.SetError(ucmbBOMID, "BOM ID Can't be Empty");
                        ucmbBOMID.Focus();
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(ucmbSiteID.Text.Trim()))
                {
                    erpMaster.SetError(ucmbSiteID, "Site ID Can't be Empty");
                    ucmbSiteID.Focus();
                    return false;
                }
                if (txtEstimatedAmount.Value == null || txtEstimatedAmount.Value.ToString() == string.Empty || double.Parse(txtEstimatedAmount.Value.ToString()) == 0)
                {
                    erpMaster.SetError(txtEstimatedAmount, "Estimated Amount Can't be Zero");
                    txtEstimatedAmount.Focus();
                    return false;
                }
                if (!IsValidGrid())
                {
                    return false;
                }
                else
                    return true;
            }
            else if (FormMode == enmFormMode.Delete)
            {
                if (ucmbBOMID.Value == null || ucmbBOMID.Value.ToString().Trim() == string.Empty)
                {
                    erpMaster.SetError(ucmbBOMID, "Select a BOQ No....!");
                    ucmbBOMID.Focus();
                    return false;
                }
                else
                    return true;
            }
            return true;
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
                if (ucmbPhaseID.Value != null)
                {
                    ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_ByPhaseID(ucmbPhaseID.Value.ToString());
                    ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                    ucmbSubPhaseID.ValueMember = "SubPhaseID";

                    //objclsCommonFunc.fillLocations_By_PhaseID(ucmbLocCode, ucmbPhaseID.Value.ToString().Trim());
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
                ArrangeDataGrid();

                if (IsValidControls())
                {                    
                    clsBeansPhases obj = new clsBeansPhases();
                    obj = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Text.Trim());
                    objclsBeansPhases.StartDate = obj.StartDate;
                    objclsBeansPhases.EndDate = obj.EndDate;

                    objclsBeansPhases.UseCostCode = obj.UseCostCode;
                    objclsBeansPhases.JobDescription = obj.Description;

                    objclsBeansPhases.BOQID = ucmbBOQID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    objclsBeansPhases.RevisionNo =int.Parse(txtRevisionNo.Value.ToString());
                    objclsBeansPhases.EstimatedAmt = double.Parse(txtEstimatedAmount.Value.ToString());
                    objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();

                    if (objclsBeansPhases.Dtbl.Rows.Count > 0)
                    {
                        if (!objclsBeansPhases.UseCostCode)
                        {
                            MessageBox.Show("Change the Site 'Use Phase' Status as True....!");
                            return;
                        }
                    }

                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;

                    //string _BOMID =ucmbBOQID.Text

                    string _BOMID = objclsBLLPhases.SaveBOMs(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SetFormMode();

                    ucmbBOMID.Text = _BOMID;
                    //ArrangeDataGrid();
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetBOMs_AllActive_Dropdown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("BOM");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbBOQID.Value = clsBeansFind.ReturnValue;
                    fillControls(objclsBLLPhases.GetBOMs_ByID_Max(ucmbBOMID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));
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

        private void fillControls(clsBeansPhases objclsBeansPhases)
        {
            //objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (objclsBeansPhases.BOMID != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetBOMs_ByID(ucmbBOMID.Text.Trim(),int.Parse(txtRevisionNo.Value.ToString()));
                    txtRevisionNo.Value = objclsBeansPhases.RevisionNo;
                    txtActualAmount.Value = objclsBeansPhases.ActualAmt;
                    txtDescription.Text = objclsBeansPhases.Description;
                    //txtEstimatedAmount.Value = objclsBeansPhases.EstimatedAmt;
                    dtpDate.Value = objclsBeansPhases.Date;
                    ucmbSiteID.Value = objclsBeansPhases.SiteID;
                    ucmbBOQID.Value = objclsBeansPhases.BOQID;
                    chkActive.Checked = objclsBeansPhases.Inactive;

                    fillBOMGrid(objclsBeansPhases.Dtbl);
                    txtEstimatedAmount.Value = objclsBeansPhases.EstimatedAmt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillBOMGrid(DataTable _dataTable)
        {
            double _Gridtotal = 0.00;
            Discard();
            int _Index=0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                double Total = 0.00;
                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    udgvSubcontractors.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colActivity"].Value = dr["Activity"].ToString();

                    if (_dataTable.Columns.Contains("ItemId"))
                        udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["ItemId"].ToString();
                    else if (_dataTable.Columns.Contains("Item"))
                        udgvSubcontractors.Rows[_Index].Cells["colItem"].Value = dr["Item"].ToString();

                    if(_dataTable.Columns.Contains("WareHouseID"))
                        udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["WareHouseID"].ToString();
                    else if (_dataTable.Columns.Contains("LocCode"))
                        udgvSubcontractors.Rows[_Index].Cells["colLocation"].Value = dr["LocCode"].ToString();

                    udgvSubcontractors.Rows[_Index].Cells["colRate"].Value = double.Parse(dr["Rate"].ToString());
                    udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    udgvSubcontractors.Rows[_Index].Cells["colQty"].Value = dr["Qty"].ToString();

                    if (_dataTable.Columns.Contains("Amount"))
                        udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value = double.Parse(dr["Amount"].ToString());
                    else if (_dataTable.Columns.Contains("Amt"))
                        udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value = double.Parse(dr["Amt"].ToString());



                    //if (udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value != null && udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value.ToString().Trim().Length > 0)
                    {
                        //if (udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value.ToString() == "Sub Phase Total")
                        //{
                        //    udgvSubcontractors.Rows[_Index].Appearance.BackColor = Color.LightGray;
                        //}
                        //else if (udgvSubcontractors.Rows[_Index].Cells["colUnits"].Value.ToString() == "Phase Total")
                        //{
                        //    _Gridtotal = _Gridtotal + double.Parse(udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value.ToString());
                        //    udgvSubcontractors.Rows[_Index].Appearance.BackColor = Color.Aquamarine;
                        //}

                        

                        if (udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value != null && udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value.ToString().Trim().Length > 0)
                        {
                            Total = Total + double.Parse(udgvSubcontractors.Rows[_Index].Cells["colAmount"].Value.ToString().Trim());
                        }
                       
                    }

                    _Index = _Index + 1;
                }
                txtEstimatedAmount.Value = Total.ToString("0.00");
                //ArrangeDataGrid();

                if (_dataTable.Rows.Count == 0)
                {
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
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
                    objclsBeansPhases.BOMID = ucmbBOMID.Text.Trim();
                    objclsBeansPhases.RevisionNo = int.Parse(txtRevisionNo.Value.ToString());

                    objclsBLLPhases.DeleteBOMs(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            try
            {
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("PhaseID");
                    _dataTable.Columns.Add("SubPhaseID");
                    _dataTable.Columns.Add("Activity");
                    _dataTable.Columns.Add("LocationID");
                    _dataTable.Columns.Add("Rate");
                    _dataTable.Columns.Add("Units");
                    _dataTable.Columns.Add("Qty");
                    _dataTable.Columns.Add("Amount");
                    _dataTable.Columns.Add("Item");
                    //_dataTable.

                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && dgvr.Cells["colUnits"].Value.ToString() != "Phase Total")
                            {
                                if (dgvr.Cells["colQty"].Value != null && dgvr.Cells["colQty"].Value.ToString().Trim().Length > 0)
                                {
                                    DataRow drow = _dataTable.NewRow();
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Value;
                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Value;
                                    drow["Activity"] = dgvr.Cells["colActivity"].Value;
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Text;
                                    drow["Rate"] = dgvr.Cells["colRate"].Value;
                                    drow["Units"] = dgvr.Cells["colUnits"].Value;
                                    drow["Amount"] = dgvr.Cells["colAmount"].Value;
                                    drow["Qty"] = dgvr.Cells["colQty"].Value;
                                    drow["Item"] = dgvr.Cells["colItem"].Value;
                                    _dataTable.Rows.Add(drow);
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
            erpMaster.Dispose();
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                if (e.Cell == null) return;
                ugR = udgvSubcontractors.ActiveRow;

                if (e.Cell.Column.Key == "colItem")
                {                   
                    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                    {
                        objDataSet = objclsBLLPhases.GetItem_By_ItemID(ugR.Cells["colItem"].Value.ToString());

                        if (FormMode != enmFormMode.Find)
                        //if (ugR.Cells["colRate"].Value == null || ugR.Cells["colRate"].Value.ToString() == string.Empty || double.Parse(ugR.Cells["colRate"].Value.ToString().Trim())==0)
                        {
                            if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                            {
                                ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["UOM"].ToString();
                                ugR.Cells["colRate"].Value = double.Parse(objDataSet.Tables[0].Rows[0]["UnitCost"].ToString()).ToString("0.00");
                            }
                        }
                        else //if (ugR.Cells["colRate"].Value == null || ugR.Cells["colRate"].Value.ToString().Trim().Length == 0)
                        {
                            if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                            {
                                ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["UOM"].ToString();
                                ugR.Cells["colRate"].Value = double.Parse(objDataSet.Tables[0].Rows[0]["UnitCost"].ToString()).ToString("0.00");
                            }                            
                        }

                        objclsCommonFunc.fillWarehouse_By_Item(ucmbLocCode,ugR.Cells["colItem"].Value.ToString());                         
                    }                    
                }
               
                if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                {
                    if (FormMode != enmFormMode.Find)
                    {
                        if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                        {
                            if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                                ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                            {
                                ugR.Cells["colAmount"].Value = double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString());
                            }
                        }
                    }
                    if (FormMode == enmFormMode.Find)
                        if (e.Cell.Column.Key == "colRate" || e.Cell.Column.Key == "colQty")
                            if (ugR.Cells["colQty"].Value != null && ugR.Cells["colRate"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0 && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0)
                                ugR.Cells["colAmount"].Value = double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString());
                }
               
                if (FormMode != enmFormMode.Find)
                {
                    if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                    {
                        if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                            ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                        {
                            ugR.Cells["colAmount"].Value = double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString());
                        }
                    }
                }

                if (e.Cell.Column.Key == "colSubPhase")
                {
                    if (ugR.Cells["colPhase"].Value != null || ugR.Cells["colPhase"].Value.ToString().Trim().Length == 0)
                        ugR.Cells["colPhase"].Value = _TempPhaseID;
                }
                if (e.Cell.Column.Key == "colPhase")
                {
                    _TempPhaseID = ugR.Cells["colPhase"].Text;
                }

                if (e.Cell.Column.Key == "colActivity")
                {
                    if (ugR.Cells["colSubPhase"].Value != null || ugR.Cells["colSubPhase"].Value.ToString().Trim().Length == 0)
                        ugR.Cells["colSubPhase"].Value = _TempSubPhaseID;
                }
                if (e.Cell.Column.Key == "colSubPhase")
                {
                    _TempSubPhaseID = ugR.Cells["colSubPhase"].Text;
                }

                //if (e.Cell.Column.Key == "colAmount")
                {
                    double Total = 0.00;
                    foreach (UltraGridRow _ugr in udgvSubcontractors.Rows)
                    {
                        if (_ugr.Cells["colAmount"].Value != null && _ugr.Cells["colAmount"].Value.ToString().Trim().Length > 0)
                        {
                            Total = Total + double.Parse(_ugr.Cells["colAmount"].Value.ToString().Trim());
                        }
                    }
                    txtEstimatedAmount.Value = Total.ToString("0.00");
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
                        && (dgvr.Cells["colSubPhase"].Value == null || dgvr.Cells["colSubPhase"].Value.ToString() == string.Empty)
                        && (dgvr.Cells["colActivity"].Value == null || dgvr.Cells["colActivity"].Value.ToString() == string.Empty))
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


        private void ucmbLocCode_Enter(object sender, EventArgs e)
        {
            ucmbLocCode.PerformAction(UltraComboAction.Dropdown);
        }

        private void ucmbLocCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
                ucmbLocCode.PerformAction(UltraComboAction.Dropdown);
        }

        private void udgvSubcontractors_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int _currntIndex = 0;
                _currntIndex = udgvSubcontractors.ActiveRow.Index;
                
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvSubcontractors.ActiveCell.Column.Key == "colAmount")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

                        if (GetNoOfEmptyRows() < 2)
                        {
                            udgvSubcontractors.Rows[_currntIndex + 1].Selected = true;
                            udgvSubcontractors.ActiveCell = udgvSubcontractors.Rows[_currntIndex + 1].Cells[0];                            
                            udgvSubcontractors.PerformAction(UltraGridAction.PrevCell, true, false);
                            //udgvSubcontractors.Rows[_currntIndex + 1].Cells[0].TabStop = 0;
                            //udgvSubcontractors.Rows[_currntIndex + 1].Cells[0].Activated = true;
                            //udgvSubcontractors.Rows[_currntIndex + 1].Cells[0].Activation = Activation.AllowEdit;
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
            objclsBLLPhases=new clsBLLPhases ();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbSiteID.Value != null)
                {
                    _objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    ucmbCustomerID.Value = _objclsBeansPhases.CustomerID;

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

            try
            {
                if (ucmbCustomerID.Value != null)
                {
                    txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                    objclsCommonFunc.fillJobs_Active(ucmbSiteID, ucmbCustomerID);
                    
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbBOQID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (FormMode != enmFormMode.Find)
                {
                    if (ucmbBOQID.Value != null && ucmbBOQID.Value.ToString().Trim().Length > 0)
                    {
                        _objclsBeansPhases = objclsBLLPhases.GetBOQs_ByID_Max(ucmbBOQID.Text.Trim(), 0);
                        ucmbSiteID.Text = _objclsBeansPhases.SiteID;
                    }
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
           
        }

        private void chkAdd_CheckedChanged(object sender, EventArgs e)
        {
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (chkAdd.Checked)
                {
                    if (ucmbBOMID.Value == null || ucmbBOMID.Value.ToString().Trim().Length == 0)
                    {
                        if (ucmbBOQID.Value != null && ucmbBOQID.Value.ToString().Trim().Length > 0)
                        {
                            _objclsBeansPhases = objclsBLLPhases.GetBOQs_ByID_Max(ucmbBOQID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString()));

                            ucmbSiteID.Text = _objclsBeansPhases.SiteID;

                            fillBOMGrid_ON_BOQ_SELECTION(_objclsBeansPhases.Dtbl);
                        }
                    }
                }
                else
                {
                    Discard();

                    Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                    ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ArrangeDataGrid1()
        {
            DataTable _dataTable = new DataTable();
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR0;

            double _Gridtotal = 0.00;
            try
            {
                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows.All)
                {
                    if (dgvr.Cells["colAmount"].Value == null || dgvr.Cells["colAmount"].Value.ToString().Trim().Length == 0)
                        dgvr.Delete(false);

                    if (dgvr.Cells["colUnits"].Value != null && dgvr.Cells["colUnits"].Value.ToString() == "Sub Phase Total")
                        dgvr.Delete(false);

                    if (dgvr.Cells["colUnits"].Value != null && dgvr.Cells["colUnits"].Value.ToString() == "Phase Total")
                        dgvr.Delete(false);
                }

                string _PhaseID = ""; string _SubPhaseID = ""; string _Activity = ""; string _LocationID = "";
                double _Rate = 0.00; string _Units = ""; double _Qty = 0; double _Amount = 0.00; double _Total = 0; double _SubTotal = 0;

                _dataTable.Columns.Add("colPhase");
                _dataTable.Columns.Add("colSubPhase");
                _dataTable.Columns.Add("colActivity");
                _dataTable.Columns.Add("colLocation");
                _dataTable.Columns.Add("colRate");
                _dataTable.Columns.Add("colUnits");
                _dataTable.Columns.Add("colQty");
                _dataTable.Columns.Add("colAmount");

                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0 &&
                            dgvr.Cells["colSubPhase"].Value != null && dgvr.Cells["colSubPhase"].Value.ToString().Trim().Length > 0)
                    {
                        {
                            if (_SubPhaseID.Length > 0 && _SubPhaseID != dgvr.Cells["colSubPhase"].Value.ToString().Trim())
                            {
                                DataRow drowTotal = _dataTable.NewRow();
                                drowTotal["colPhase"] = "";
                                drowTotal["colSubPhase"] = "";
                                drowTotal["colActivity"] = "";
                                drowTotal["colLocation"] = "";
                                drowTotal["colRate"] = 0.00;
                                drowTotal["colUnits"] = "Sub Phase Total";
                                drowTotal["colAmount"] = _SubTotal;
                                drowTotal["colQty"] = 0;
                                _dataTable.Rows.Add(drowTotal);
                                _SubTotal = 0;
                            }
                        }
                        _PhaseID = dgvr.Cells["colPhase"].Value.ToString();
                    }

                    if (_PhaseID.Length > 0 && _PhaseID != dgvr.Cells["colPhase"].Value.ToString().Trim())
                    {
                        DataRow drowTotal = _dataTable.NewRow();
                        drowTotal["colPhase"] = "";
                        drowTotal["colSubPhase"] = "";
                        drowTotal["colActivity"] = "";
                        drowTotal["colLocation"] = "";
                        drowTotal["colRate"] = "0";
                        drowTotal["colUnits"] = "Phase Total";
                        drowTotal["colAmount"] = _Total;
                        drowTotal["colQty"] = "0";
                        _dataTable.Rows.Add(drowTotal);
                        _Total = 0;
                    }
                    _PhaseID = dgvr.Cells["colPhase"].Value.ToString();

                    if (dgvr.Cells["colSubPhase"].Value != null && dgvr.Cells["colSubPhase"].Value.ToString().Trim().Length > 0)
                        _SubPhaseID = dgvr.Cells["colSubPhase"].Value.ToString();

                    if (dgvr.Cells["colActivity"].Value != null && dgvr.Cells["colActivity"].Value.ToString().Trim().Length > 0)
                        _Activity = dgvr.Cells["colActivity"].Value.ToString();
                    else _Activity = "";

                    if (dgvr.Cells["colLocation"].Value != null && dgvr.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                        _LocationID = dgvr.Cells["colLocation"].Value.ToString();
                    else _LocationID = "";

                    if (dgvr.Cells["colRate"].Value != null && dgvr.Cells["colRate"].Value.ToString().Trim().Length > 0)
                        _Rate = double.Parse(dgvr.Cells["colRate"].Value.ToString());
                    else _Rate = 0.00;

                    if (dgvr.Cells["colUnits"].Value != null && dgvr.Cells["colUnits"].Value.ToString().Trim().Length > 0)
                        _Units = dgvr.Cells["colUnits"].Value.ToString();

                    if (dgvr.Cells["colAmount"].Value != null && dgvr.Cells["colAmount"].Value.ToString().Trim().Length > 0)
                    {
                        _Amount = double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                        _Total = _Total + _Amount;
                        _SubTotal = _SubTotal + _Amount;
                    }
                    else _Amount = 0.00;

                    if (dgvr.Cells["colQty"].Value != null && dgvr.Cells["colQty"].Value.ToString().Trim().Length > 0)
                        _Qty = double.Parse(dgvr.Cells["colQty"].Value.ToString());
                    else _Qty = 0;

                    DataRow drow = _dataTable.NewRow();
                    drow["colPhase"] = _PhaseID;
                    drow["colSubPhase"] = _SubPhaseID;
                    drow["colActivity"] = _Activity;
                    drow["colLocation"] = _LocationID;
                    drow["colRate"] = _Rate;
                    drow["colUnits"] = _Units;
                    drow["colAmount"] = _Amount;
                    drow["colQty"] = _Qty;
                    _dataTable.Rows.Add(drow);
                }

                DataRow drowTotalSub;

                drowTotalSub = _dataTable.NewRow();
                drowTotalSub["colPhase"] = "";
                drowTotalSub["colSubPhase"] = "";
                drowTotalSub["colActivity"] = "";
                drowTotalSub["colLocation"] = "";
                drowTotalSub["colRate"] = 0.00;
                drowTotalSub["colUnits"] = "Sub Phase Total";
                drowTotalSub["colAmount"] = _SubTotal;
                drowTotalSub["colQty"] = 0;
                _dataTable.Rows.Add(drowTotalSub);
                _SubTotal = 0;

                drowTotalSub = _dataTable.NewRow();
                drowTotalSub["colPhase"] = "";
                drowTotalSub["colSubPhase"] = "";
                drowTotalSub["colActivity"] = "";
                drowTotalSub["colLocation"] = "";
                drowTotalSub["colRate"] = "0";
                drowTotalSub["colUnits"] = "Phase Total";
                drowTotalSub["colAmount"] = _Total;
                drowTotalSub["colQty"] = "0";
                _dataTable.Rows.Add(drowTotalSub);
                _Total = 0;

                Discard();

                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR0 = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
                    ugR0.Cells["colPhase"].Value = dr["colPhase"];
                    ugR0.Cells["colSubPhase"].Value = dr["colSubPhase"];
                    ugR0.Cells["colActivity"].Value = dr["colActivity"];
                    ugR0.Cells["colLocation"].Value = dr["colLocation"];
                    ugR0.Cells["colRate"].Value = dr["colRate"];
                    ugR0.Cells["colUnits"].Value = dr["colUnits"];
                    ugR0.Cells["colAmount"].Value = dr["colAmount"];
                    ugR0.Cells["colQty"].Value = dr["colQty"];
                }

                foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                {
                    if (dgvr.Cells["colUnits"].Value != null && dgvr.Cells["colUnits"].Value.ToString().Trim().Length > 0)
                    {
                        if (dgvr.Cells["colUnits"].Value.ToString() == "Sub Phase Total")
                        {
                            dgvr.Cells["colUnits"].Appearance.BackColor = Color.SkyBlue;
                            dgvr.Cells["colAmount"].Appearance.BackColor = Color.SkyBlue;
                        }
                        else if (dgvr.Cells["colUnits"].Value.ToString() == "Phase Total")
                        {
                            _Gridtotal = _Gridtotal + double.Parse(dgvr.Cells["colAmount"].Value.ToString());
                            dgvr.Cells["colUnits"].Appearance.BackColor = Color.RoyalBlue;
                            dgvr.Cells["colAmount"].Appearance.BackColor = Color.RoyalBlue;
                        }
                    }
                }

                txtEstimatedAmount.Value = _Gridtotal.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            try
            {
                if (udgvSubcontractors.Rows.Count > 1)
                {
                    FormMode = enmFormMode.Find;
                    ArrangeDataGrid();
                    FormMode = enmFormMode.Clear;
                }
                else if (udgvSubcontractors.Rows.Count == 1)
                {
                    txtEstimatedAmount.Value = udgvSubcontractors.Rows[0].Cells["colAmount"].Value.ToString();
                    //foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    //{

                    //}
                }
            }
            catch (Exception ex)
            {
                        objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }

        }

        private void udgvSubcontractors_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
        }

        private void ArrangeDataGrid()
        {
            DataTable _dataTable = new DataTable();
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR0;

            double _Gridtotal = 0.00;
            try
            {
                objclsBeansPhases = new clsBeansPhases();
                objclsBeansPhases.Dtbl = getDatasource_InGrid();
                objclsBLLPhases.SaveTemp(objclsBeansPhases);

                fillBOMGrid(objclsBLLPhases.GetTemp_All().Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void tsbtnLast_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbBOMID.Text = objclsBLLPhases.GetCodeforArrow("BOM", ucmbBOMID.Text.Trim(), "RR");
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
                ucmbBOMID.Text = objclsBLLPhases.GetCodeforArrow("BOM", ucmbBOMID.Text.Trim(), "LL");
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
                ucmbBOMID.Text = objclsBLLPhases.GetCodeforArrow("BOM", ucmbBOMID.Text.Trim(), "L");
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
                ucmbBOMID.Text = objclsBLLPhases.GetCodeforArrow("BOM", ucmbBOMID.Text.Trim(), "R");
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvSubcontractors_Click(object sender, EventArgs e)
        {
            DataSet _objDataset = new DataSet();
            try
            {
                if (udgvSubcontractors.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvSubcontractors.ActiveCell.Column.Key == "colLocation")
                {
                    ugR = udgvSubcontractors.ActiveRow;

                    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                    {
                        objclsCommonFunc.fillWarehouse_By_Item(ucmbLocCode,ugR.Cells["colItem"].Value.ToString().Trim());
                    }                   
                }

                //ugR = udgvSubcontractors.ActiveRow;

                //if (ugR.Cells["colSubPhase"].Value != null && ugR.Cells["colSubPhase"].Value.ToString().Trim().Length > 0)
                //{
                //    _objDataset = objclsCommonFunc.fillLocations_By_SubPhaseID(ucmbLocCode, ugR.Cells["colSubPhase"].Value.ToString().Trim());
                //    if (_objDataset.Tables.Count > 0 && _objDataset.Tables[0].Rows.Count == 0)
                //    {
                //        clsBeansPhases tempobjclsBeansPhases = objclsBLLPhases.GetSubPhases_ByID(ugR.Cells["colSubPhase"].Value.ToString());

                //        ugR.Cells["colRate"].Value = tempobjclsBeansPhases.Rate;
                //        ugR.Cells["colUnits"].Value = tempobjclsBeansPhases.Units;
                //    }
                //}
                //else if (ugR.Cells["colPhase"].Value != null && ugR.Cells["colPhase"].Value.ToString().Trim().Length > 0)
                //{
                //    objclsCommonFunc.fillLocations_By_PhaseID(ucmbLocCode, ugR.Cells["colPhase"].Value.ToString().Trim());
                //    if (_objDataset.Tables.Count > 0 && _objDataset.Tables[0].Rows.Count == 0)
                //    {
                //        clsBeansPhases tempobjclsBeansPhases = objclsBLLPhases.GetSubPhases_ByID(ugR.Cells["colPhase"].Value.ToString());
                //        ugR.Cells["colRate"].Value = tempobjclsBeansPhases.Rate;
                //        ugR.Cells["colUnits"].Value = tempobjclsBeansPhases.Units;
                //    }
                //}

                //if (udgvSubcontractors.ActiveCell.Column.Key == "colQty")
                //{
                //    ugR = udgvSubcontractors.ActiveRow;
                //    if (ugR.Cells["colItem"].Value != null && ugR.Cells["colItem"].Value.ToString().Trim().Length > 0)
                //    {
                //        MessageBox.Show("Select the Werehouse for " + ugR.Cells["colItem"].Value.ToString() + "......!");
                //        return;
                //    }
                //}
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSubPhaseID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                //if (ucmbSubPhaseID.Value != null && ucmbSubPhaseID.Value.ToString().Trim().Length > 0)
                //{
                //    objclsCommonFunc.fillLocations_By_SubPhaseID(ucmbLocCode, ucmbSubPhaseID.Value.ToString().Trim());
                //}
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvSubcontractors_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            try
            {
                if (udgvSubcontractors.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;                

                if (udgvSubcontractors.ActiveCell.Column.Key == "colQty")
                {
                    ugR = udgvSubcontractors.ActiveRow;
                    if (ugR.Cells["colLocation"].Value == null || ugR.Cells["colLocation"].Value.ToString().Trim().Length == 0)
                    {
                        //ugR.Cells["colItem"].Value = "0.00";
                        //MessageBox.Show("Select the Werehouse for " + ugR.Cells["colItem"].Value.ToString() + "......!");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_ValueChanged(object sender, EventArgs e)
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

        private void lnkHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            DataSet _dataset = new DataSet();
            try
            {
                if (ucmbBOQID.Text.Trim() != "")
                {
                    _dataset = objclsBLLPhases.GetBOMs_ByID(ucmbBOMID.Text.Trim());

                    frmRevisionHistory objfrmRevisionHistory = new frmRevisionHistory(_dataset.Tables[0], "BOM", txtRevisionNo.Text.Trim());
                    objfrmRevisionHistory.ShowDialog();

                    if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue2))
                    {
                        //lblEstIndex.Text = clsBeansFind.ReturnValue2;
                        //txtEstimateNo.Text = clsBeansFind.ReturnValue;
                        FormMode = enmFormMode.Find;
                        objclsBeansPhases = objclsBLLPhases.GetBOMs_ByID(clsBeansFind.ReturnValue, int.Parse(clsBeansFind.ReturnValue2));

                        ucmbBOQID.Value = clsBeansFind.ReturnValue;
                        txtRevisionNo.Value = clsBeansFind.ReturnValue2;

                        fillControls(objclsBLLPhases.GetBOMs_ByID(ucmbBOMID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));
                        ArrangeDataGrid();
                        //objclsBeansEstimate = objclsBLLEstimate.GetEstIndex_ByRevNo_EstNo(clsBeansFind.ReturnValue2, clsBeansFind.ReturnValue);

                        //if (objclsBeansPhases.BOQID != null)
                        //{
                        //    fillControls(

                        //    SetFormMode();
                        //}
                    }
                }
                else
                    MessageBox.Show("Select an BOM to view History");
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
                    frmJob objfrmBOM = new frmJob(ucmbSiteID.Value.ToString().Trim());
                    objfrmBOM.Show();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

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

        private void ucmbBOMID_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtEstimatedAmount_ValueChanged(object sender, EventArgs e)
        {
            erpMaster.Dispose();
        }

        private void ucmbBOMID_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                objclsBLLPhases = new clsBLLPhases();
                if (ucmbBOMID.Value != null && ucmbBOMID.Value.ToString().Trim().Length > 0)
                {
                    FormMode = enmFormMode.Find;
                    fillControls(objclsBLLPhases.GetBOMs_ByID_Max(ucmbBOMID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));
                    // ArrangeDataGrid();
                }
                else
                {
                    FormMode = enmFormMode.Clear;
                }
                SetFormMode();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        

    }
}