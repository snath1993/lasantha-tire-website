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
    public partial class frmBOQ : Form
    {
        clsCommon objclsCommon = new clsCommon();
        clsCommonFunc objclsCommonFunc = new clsCommonFunc();
        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "BOQ";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        string _TempPhaseID = "";
        string _TempSubPhaseID = "";
        //string _TempSubPhaseID = "";

        DataTable _DocumentList;

        public frmBOQ()
        {
            InitializeComponent();

            

            ucmbBOQID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();
        }

        public frmBOQ(string _BOQID)
        {
            InitializeComponent();

         

            try
            {
                if (_BOQID.Trim().Length > 0)
                {
                    ucmbBOQID.Value = _BOQID;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, "Load", ex.StackTrace);
            }
        }

        public void Discard()
        {
            foreach (UltraGridRow ugR in udgvBOQ.Rows.All)
            {
                ugR.Delete(false);
            }
        }

        private void ClearControlers()
        {            
            ucmbPhaseID.Text = "";
            txtDescription.Text = "";
            txtActualAmount.Value = 0.00;
            txtEstimatedAmount.Value = 0.00;
            txtRevisionNo.Value = 0;
            ucmbSiteID.Text = "";
            ucmbBOQID.Text = "";
            ucmbCustomerID.Text = "";
            txtCustomerName.Text = "";
            lblPosted.Text = "";

            dtpDate.Value = user.LoginDate;
            dtpEnd.Value = user.LoginDate;
            dtpStart.Value = user.LoginDate;

            Discard();
            objclsCommonFunc.fillBOQs(ucmbBOQID);
                              

            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            ugR = udgvBOQ.DisplayLayout.Bands[0].AddNew();
            _DocumentList = null;

            _TempPhaseID = "";
            //_TempSubPhaseID = "";
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
            //uc
            ArrangeDataGrid();

            if (!user.IsBOQNOAutoGen)
            {
                if (string.IsNullOrEmpty(ucmbBOQID.Text.Trim()))
                {
                    erpMaster.SetError(ucmbBOQID, "BOQ ID Can't be Empty");
                    ucmbBOQID.Focus();
                    return false;
                }
            }

            if (txtEstimatedAmount.Value == null || txtEstimatedAmount.Value.ToString() == string.Empty || double.Parse(txtEstimatedAmount.Value.ToString()) == 0)
            {
                erpMaster.SetError(txtEstimatedAmount, "Estimated Amount Can't be Zero");
                txtEstimatedAmount.Focus();
                return false;
            }          
            //if (txtEstimatedAmount.Value==null || txtEstimatedAmount.Value.ToString().Trim().Length == 0)
            //{
            //    if(double.Parse(txtEstimatedAmount.Value.ToString()) == 0)
            //    {
            //        erpMaster.SetError(txtDescription, "Amount Can't be Zero");
            //        txtDescription.Focus();
            //        return false;
            //    }
            //}
            if (string.IsNullOrEmpty(ucmbSiteID.Text.Trim()))
            {
                erpMaster.SetError(ucmbSiteID, "Site ID Can't be Empty");
                ucmbSiteID.Focus();
                return false;
            }
            else
                return true;
        }

        private void frmBOQ_Load(object sender, EventArgs e)
        {
            try
            {
                objclsCommonFunc.fillCustomer(ucmbCustomerID);
                objclsCommonFunc.fillLocations(ucmbLocCode);
                objclsCommonFunc.fillPhases(ucmbPhaseID);
                objclsCommonFunc.fillSites(ucmbSiteID);
                objclsCommonFunc.fillSubPhases(ucmbSubPhaseID); 
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbPhaseID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                if (ucmbPhaseID.Value != null)
                {
                    ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_ByPhaseID(ucmbPhaseID.Value.ToString());
                    ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                    ucmbSubPhaseID.ValueMember = "SubPhaseID";

                    objclsCommonFunc.fillLocations_By_PhaseID(ucmbLocCode, ucmbPhaseID.Value.ToString().Trim());
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
                if (IsValidControls())
                {                   
                    
                    objclsBeansPhases.IsPosted = false;
                    objclsBeansPhases.BOQID = ucmbBOQID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    objclsBeansPhases.RevisionNo =int.Parse(txtRevisionNo.Value.ToString());
                    objclsBeansPhases.EstimatedAmt = double.Parse(txtEstimatedAmount.Value.ToString());
                    objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.EstimateStartDate = dtpStart.Value;
                    objclsBeansPhases.EstimateEndDate = dtpEnd.Value;
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    objclsBeansPhases.DtblPT = getDatasource_InGrid_PT();
                    if (objclsBeansPhases.Dtbl.Rows.Count > 0)
                        objclsBeansPhases.IsUsePhases = true;
                    else
                        objclsBeansPhases.IsUsePhases = false;
                    objclsBeansPhases.Inactive = chkActive.Checked;
                    objclsBeansPhases.CustomerID = ucmbCustomerID.Value.ToString();
                    //objclsBeansPhases.CustomerName=txts
                    clsBeansPhases _obj=new clsBeansPhases ();
                    _obj=objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString().Trim());

                    objclsBeansPhases.SiteDescription = _obj.Description;
                    objclsBeansPhases.EstimateStartDate = _obj.EstimateStartDate;
                    objclsBeansPhases.EstimateEndDate = _obj.EstimateEndDate;


                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;
                    clsBeansPhases.DtblAttachment = null;

                    string _BOQID = objclsBLLPhases.SaveBOQs(objclsBeansPhases,false);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SetFormMode();

                    ucmbBOQID.Text = _BOQID;
                    ////ArrangeDataGrid();

                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                objclsBLLPhases = new clsBLLPhases();
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetBOQs_All();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                //if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("BOQ");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbBOQID.Value = clsBeansFind.ReturnValue;
                    fillControls(objclsBLLPhases.GetBOQs_ByID_Max(ucmbBOQID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));
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
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                if (objclsBeansPhases.BOQID != null)
                {
                    //objclsBeansPhases = objclsBLLPhases.GetBOQs_ByID_Max(ucmbBOQID.Text.Trim(),int.Parse(txtRevisionNo.Value.ToString()));
                    txtRevisionNo.Value = objclsBeansPhases.RevisionNo;
                    txtActualAmount.Value = objclsBeansPhases.ActualAmt;
                    txtDescription.Text = objclsBeansPhases.Description;

                    dtpDate.Value = objclsBeansPhases.Date;
                    dtpEnd.Value = objclsBeansPhases.EstimateEndDate;
                    dtpStart.Value = objclsBeansPhases.EstimateStartDate;
                    if (objclsBeansPhases.IsPosted)
                        lblPosted.Text = "POSTED";
                    else
                        lblPosted.Text = "PENDING";
                    //lblPosted.Text = 
                    //    ;

                    ucmbSiteID.Value = objclsBeansPhases.SiteID;

                    chkActive.Checked = objclsBeansPhases.Inactive;
                    Discard();
                    fillBOQGrid(objclsBeansPhases.Dtbl);
                    _DocumentList = objclsBeansPhases.DtblDocList;
                    txtEstimatedAmount.Value = objclsBeansPhases.EstimatedAmt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillBOQGrid(DataTable _dataTable)
        {
            int _Index = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            double _Gridtotal = 0.00;
            try
            {
                foreach (UltraGridRow ugr in udgvBOQ.Rows)
                {
                    if(ugr.Cells["colAmount"].Value!=null && ugr.Cells["colAmount"].Value.ToString().Trim()!=string.Empty)
                        _Gridtotal = _Gridtotal + double.Parse(ugr.Cells["colAmount"].Value.ToString());
                }


                foreach (DataRow dr in _dataTable.Rows)
                {
                    ugR = udgvBOQ.DisplayLayout.Bands[0].AddNew();
                    udgvBOQ.Rows[_Index].Cells["colPhase"].Value = dr["PhaseID"].ToString();
                    udgvBOQ.Rows[_Index].Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                    udgvBOQ.Rows[_Index].Cells["colActivity"].Value = dr["Activity"].ToString();
                    if (_dataTable.Columns.Contains("Location"))
                        udgvBOQ.Rows[_Index].Cells["colLocation"].Value = dr["Location"].ToString();
                    else if (_dataTable.Columns.Contains("LocCode"))
                        udgvBOQ.Rows[_Index].Cells["colLocation"].Value = dr["LocCode"].ToString();

                    udgvBOQ.Rows[_Index].Cells["colRate"].Value = double.Parse(dr["Rate"].ToString()).ToString("0.00");
                    udgvBOQ.Rows[_Index].Cells["colUnits"].Value = dr["Units"].ToString();
                    udgvBOQ.Rows[_Index].Cells["colQty"].Value = double.Parse(dr["Qty"].ToString()).ToString("0.00");
                    if (_dataTable.Columns.Contains("Amount"))
                        udgvBOQ.Rows[_Index].Cells["colAmount"].Value = double.Parse(dr["Amount"].ToString()).ToString("0.00");
                    else if (_dataTable.Columns.Contains("Amt"))
                        udgvBOQ.Rows[_Index].Cells["colAmount"].Value = double.Parse(dr["Amt"].ToString()).ToString("0.00");


                    if (udgvBOQ.Rows[_Index].Cells["colUnits"].Value != null && udgvBOQ.Rows[_Index].Cells["colUnits"].Value.ToString().Trim().Length > 0)
                    {
                        if (udgvBOQ.Rows[_Index].Cells["colUnits"].Value.ToString() == "Sub Phase Total")
                        {
                                //udgvBOQ.Rows[_Index].Appearance.BackColor = Color.LightGray;
                        }
                        else if (udgvBOQ.Rows[_Index].Cells["colUnits"].Value.ToString() == "Phase Total")
                        {
                            //_Gridtotal = _Gridtotal + double.Parse(udgvBOQ.Rows[_Index].Cells["colAmount"].Value.ToString());
                            //udgvBOQ.Rows[_Index].Appearance.BackColor = Color.Aquamarine;
                        }
                    }
                    _Index = _Index + 1;
                }
                txtEstimatedAmount.Value = _Gridtotal.ToString();
                
                //ArrangeDataGrid();

                //if (_dataTable.Rows.Count == 0)
                //{
                //    ugR = udgvBOQ.DisplayLayout.Bands[0].AddNew();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetNoOfEmptyRows()
        {
            int _NoOfEmptyRows=0;

            try
            {                
                foreach (UltraGridRow dgvr in udgvBOQ.Rows)
                {
                    if ((dgvr.Cells["colPhase"].Value == null || dgvr.Cells["colPhase"].Value.ToString()==string.Empty)
                        && (dgvr.Cells["colSubPhase"].Value == null || dgvr.Cells["colSubPhase"].Value.ToString()==string.Empty)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Delete;
                objclsBLLPhases = new clsBLLPhases();
                objclsBeansPhases = new clsBeansPhases();

                if (IsValidControls())
                {
                    objclsBeansPhases.BOQID = ucmbBOQID.Text.Trim();
                    objclsBeansPhases.RevisionNo = int.Parse(txtRevisionNo.Value.ToString());

                    objclsBLLPhases.DeleteBOQs(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private DataTable getDatasource_InGrid_PT()
        {
            DataTable _dataTable = new DataTable();
            clsCommonFunc objclsCommonFunc = new clsCommonFunc();

            try
            {
                if (udgvBOQ.Rows.Count > 0)
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

                    foreach (UltraGridRow dgvr in udgvBOQ.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && dgvr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                            {
                                if (dgvr.Cells["colQty"].Value != null && dgvr.Cells["colQty"].Value.ToString().Trim().Length > 0)
                                {
                                    
                                    DataRow drow = _dataTable.NewRow();
                                    drow["Item"] = "";
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Value;
                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Value;
                                    drow["Activity"] = dgvr.Cells["colActivity"].Value;
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Value;
                                    drow["Rate"] = dgvr.Cells["colRate"].Value;
                                    drow["Units"] = dgvr.Cells["colUnits"].Value;
                                    if (dgvr.Cells["colSubPhase"].Value == null) dgvr.Cells["colSubPhase"].Value = "";
                                    drow["Amount"] = GetTotals(dgvr.Cells["colSubPhase"].Value.ToString(), dgvr.Cells["colPhase"].Value.ToString());
                                    drow["Qty"] = dgvr.Cells["colQty"].Value;
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

        private double GetTotals(string SubPhase,string Phase)
        {
            double _Amt = 0;

            foreach (UltraGridRow udr in udgvBOQ.Rows)
            {
                if (udr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && udr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                {
                    if (udr.Cells["colPhase"].Value.ToString().Trim() == Phase && udr.Cells["colSubPhase"].Value.ToString().Trim() == SubPhase)
                        _Amt = _Amt + double.Parse(udr.Cells["colAmount"].Value.ToString());
                }
            }
            return _Amt;
        }

        private DataTable getDatasource_InGrid()
        {
            DataTable _dataTable = new DataTable();
            try
            {
                if (udgvBOQ.Rows.Count > 0)
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

                    foreach (UltraGridRow dgvr in udgvBOQ.Rows)
                    {
                        if (dgvr.Cells["colPhase"].Value != null && dgvr.Cells["colPhase"].Value.ToString().Trim().Length > 0)
                        {
                            if (dgvr.Cells["colUnits"].Value.ToString().Trim() != "Phase Total" && dgvr.Cells["colUnits"].Value.ToString().Trim() != "Sub Phase Total")
                            {
                                if (dgvr.Cells["colQty"].Value != null && dgvr.Cells["colQty"].Value.ToString().Trim().Length > 0)
                                {
                                    DataRow drow = _dataTable.NewRow();
                                    drow["Item"] = "";
                                    drow["PhaseID"] = dgvr.Cells["colPhase"].Value;
                                    drow["SubPhaseID"] = dgvr.Cells["colSubPhase"].Value;
                                    drow["Activity"] = dgvr.Cells["colActivity"].Value;
                                    drow["LocationID"] = dgvr.Cells["colLocation"].Value;
                                    drow["Rate"] = dgvr.Cells["colRate"].Value;
                                    drow["Units"] = dgvr.Cells["colUnits"].Value;
                                    drow["Amount"] = dgvr.Cells["colAmount"].Value;
                                    drow["Qty"] = dgvr.Cells["colQty"].Value;
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

        private void udgvBOQ_AfterCellUpdate(object sender, CellEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            try
            {
                if (e.Cell == null) return;
                if (e.Cell.Column.Key == "colActivity" || e.Cell.Column.Key == "colPhase" || e.Cell.Column.Key == "colSubPhase" || e.Cell.Column.Key == "colLocation" || e.Cell.Column.Key == "colQty")
                {
                    ugR = udgvBOQ.ActiveRow;                   

                    if (e.Cell.Column.Key == "colPhase" || e.Cell.Column.Key == "colSubPhase")
                    {
                        if (ugR.Cells["colUnits"].Value.ToString() == "Sub Phase Total" || ugR.Cells["colUnits"].Value.ToString() == "Phase Total")
                        {
                            ugR.Cells["colUnits"].Value = "";
                            ugR.Cells["colAmount"].Value = "";
                            ugR.Cells["colUnits"].Appearance.BackColor = DefaultBackColor;
                            ugR.Cells["colAmount"].Appearance.BackColor = DefaultBackColor;
                        }
                    }

                    if (ugR.Cells["colSubPhase"].Value != null && ugR.Cells["colSubPhase"].Value.ToString().Trim().Length > 0 &&
                        ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                    {                        
                        objDataSet = objclsBLLPhases.GetSubPhases_ByID_Location(ugR.Cells["colSubPhase"].Value.ToString(), ugR.Cells["colLocation"].Value.ToString());

                        if (objDataSet.Tables[0].Rows.Count > 0)
                        {
                            ugR.Cells["colRate"].Value = double.Parse(objDataSet.Tables[0].Rows[0]["Rate"].ToString()).ToString("0.00");
                            ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["Units"].ToString();
                        }
                    }

                    else if (ugR.Cells["colPhase"].Value != null && ugR.Cells["colPhase"].Value.ToString().Trim().Length > 0 &&
                        ugR.Cells["colLocation"].Value != null && ugR.Cells["colLocation"].Value.ToString().Trim().Length > 0)
                    {
                        objDataSet = objclsBLLPhases.GetPhases_ByID_Location(ugR.Cells["colPhase"].Value.ToString(), ugR.Cells["colLocation"].Value.ToString());

                        if (objDataSet.Tables[0].Rows.Count > 0)
                        {
                            ugR.Cells["colRate"].Value = double.Parse(objDataSet.Tables[0].Rows[0]["Rate"].ToString()).ToString("0.00");
                            ugR.Cells["colUnits"].Value = objDataSet.Tables[0].Rows[0]["Units"].ToString();
                        }
                    }

                    else if (ugR.Cells["colPhase"].Value != null && ugR.Cells["colPhase"].Value.ToString().Trim().Length > 0 &&(
                   ugR.Cells["colLocation"].Value == null && ugR.Cells["colLocation"].Value.ToString().Trim().Length == 0))
                    {
                        //if (FormMode != enmFormMode.Find)
                        if (ugR.Cells["colRate"].Value == null || ugR.Cells["colRate"].Value.ToString() == string.Empty || double.Parse(ugR.Cells["colRate"].Value.ToString().Trim()) == 0)
                        {
                            clsBeansPhases obj = objclsBLLPhases.GetPhases_ByID(ugR.Cells["colPhase"].Value.ToString());

                            //if (objDataSet.Tables[0].Rows.Count > 0)
                            {
                                ugR.Cells["colRate"].Value = obj.Rate.ToString("0.00");
                                ugR.Cells["colUnits"].Value = obj.Units;
                            }
                        }
                    }

                    else if (ugR.Cells["colSubPhase"].Value != null && ugR.Cells["colSubPhase"].Value.ToString().Trim().Length > 0 &&(
                        ugR.Cells["colLocation"].Value == null || ugR.Cells["colLocation"].Value.ToString().Trim().Length== 0))
                    {
                        if (FormMode != enmFormMode.Find)
                        {
                            clsBeansPhases _obj = objclsBLLPhases.GetSubPhases_ByID(ugR.Cells["colSubPhase"].Value.ToString());

                            //if (objDataSet.Tables[0].Rows.Count > 0)
                            {
                                ugR.Cells["colRate"].Value = _obj.Rate.ToString("0.00");
                                ugR.Cells["colUnits"].Value = _obj.Units;
                            }
                        }
                    }

                    if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                        ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                    {
                        if (ugR.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && ugR.Cells["colUnits"].Value.ToString() != "Phase Total")
                            ugR.Cells["colAmount"].Value =(double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString())).ToString("0.00");
                    }

                    if (e.Cell.Column.Key == "colSubPhase")
                    {
                        if (ugR.Cells["colPhase"].Value == null || ugR.Cells["colPhase"].Value.ToString().Trim().Length == 0)
                            if (ugR.Cells["colSubPhase"].Value != null && ugR.Cells["colSubPhase"].Value.ToString().Trim().Length > 0)
                                ugR.Cells["colPhase"].Value = _TempPhaseID;
                    }
                    if (e.Cell.Column.Key == "colPhase")
                    {
                        _TempPhaseID = ugR.Cells["colPhase"].Text;
                    }

                    if (e.Cell.Column.Key == "colActivity")
                    {
                        if ((ugR.Cells["colSubPhase"].Value == null || ugR.Cells["colSubPhase"].Value.ToString().Trim().Length == 0) &&
                            (ugR.Cells["colPhase"].Value == null || ugR.Cells["colPhase"].Value.ToString().Trim().Length == 0))
                            if (ugR.Cells["colActivity"].Value != null && ugR.Cells["colActivity"].Value.ToString().Trim().Length > 0)
                                ugR.Cells["colSubPhase"].Value = _TempSubPhaseID;
                    }
                    if (e.Cell.Column.Key == "colSubPhase")
                    {
                        _TempSubPhaseID = ugR.Cells["colSubPhase"].Text;
                    }
                }

                if (e.Cell.Column.Key == "colQty" || e.Cell.Column.Key == "colRate")
                {
                    ugR = udgvBOQ.ActiveRow;    

                    if (ugR.Cells["colRate"].Value != null && ugR.Cells["colRate"].Value.ToString().Trim().Length > 0 &&
                        ugR.Cells["colQty"].Value != null && ugR.Cells["colQty"].Value.ToString().Trim().Length > 0)
                    {
                        if (ugR.Cells["colUnits"].Value.ToString() != "Sub Phase Total" && ugR.Cells["colUnits"].Value.ToString() != "Phase Total")
                            ugR.Cells["colAmount"].Value = (double.Parse(ugR.Cells["colQty"].Value.ToString()) * double.Parse(ugR.Cells["colRate"].Value.ToString())).ToString("0.00");
                    }
                }

                if (e.Cell.Column.Key == "colAmount")
                {                    
                    double Total = 0.00;
                    foreach (UltraGridRow _ugr in udgvBOQ.Rows)
                    {
                        if (_ugr.Cells["colAmount"].Value != null && _ugr.Cells["colAmount"].Value.ToString().Trim()!=string.Empty && double.Parse(_ugr.Cells["colAmount"].Value.ToString().Trim()) > 0)
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

        private void ucmbLocCode_Enter(object sender, EventArgs e)
        {
            ucmbLocCode.PerformAction(UltraComboAction.Dropdown);
        }

        private void ucmbLocCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
                ucmbLocCode.PerformAction(UltraComboAction.Dropdown);

        }

        private void udgvBOQ_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int _currntIndex = 0;
                _currntIndex = udgvBOQ.ActiveRow.Index;

                if (udgvBOQ.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvBOQ.ActiveCell.Column.Key == "colAmount")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        //udgvBOQ.DisplayLayout.Override.
                        ugR = udgvBOQ.DisplayLayout.Bands[0].AddNew();
                        //ugR.Cells[0].Activated = true;
                        //ugR.Cells[0].Activation = Activation.AllowEdit;

                        if (GetNoOfEmptyRows() < 2)
                        {
                            udgvBOQ.ActiveCell = udgvBOQ.Rows[_currntIndex + 1].Cells[0];
                            udgvBOQ.PerformAction(UltraGridAction.PrevCell, true, false);
                        }
                        
                        //                    ucmbLocCode.PerformAction(UltraComboAction.Dropdown);//UltraGridAction.EnterEditModeAndDropdown);
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
            erpMaster.Dispose();
            objclsBLLPhases=new clsBLLPhases ();
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();

            try
            {
                if (ucmbSiteID.Value != null)
                {
                    _objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                    ucmbCustomerID.Text = _objclsBeansPhases.CustomerID;
                    txtCustomerName.Text = _objclsBeansPhases.CustomerName;

                    //objclsCommonFunc.fillCustomer(ucmbCustomerID, ucmbSiteID);
                    //_objclsBeansPhases.cus

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
                
                //if (ucmbSiteID.Text.Trim().Length == 0)
                {
                    ucmbSiteID.DataSource = null;
                    objclsCommonFunc.fillJobs_Active(ucmbSiteID, ucmbCustomerID);
                }
                //obj
                //if (ucmbCustomerID.Value != null)
                //{
                //    objclsBLLPhases.getc
                //    //objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString());
                //txtCustomerName.Text = ucmbCustomerID.Value.ToString();
                //}

                if(e.Row!=null)
                    txtCustomerName.Text = e.Row.Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbBOQID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (udgvBOQ.Rows.Count > 1)
                {
                    ArrangeDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }       

        private void ArrangeDataGrid()
        {
            DataTable _dataTable = new DataTable();
            Infragistics.Win.UltraWinGrid.UltraGridRow ugR0;
            objclsBLLPhases = new clsBLLPhases();

            double _Gridtotal = 0.00;
            try
            {
                objclsBeansPhases=new clsBeansPhases ();
                objclsBeansPhases.Dtbl=getDatasource_InGrid();
                objclsBLLPhases.SaveTemp(objclsBeansPhases);

                fillBOQGrid(objclsBLLPhases.GetTemp_All().Tables[0]);

                //DeleteEmptyRows();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void tsbtnLast_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbBOQID.Text = objclsBLLPhases.GetCodeforArrow("BOQ", ucmbBOQID.Text.Trim(), "RR");
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
                ucmbBOQID.Text = objclsBLLPhases.GetCodeforArrow("BOQ", ucmbBOQID.Text.Trim(), "LL");
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
                ucmbBOQID.Text = objclsBLLPhases.GetCodeforArrow("BOQ", ucmbBOQID.Text.Trim(), "L");
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
                ucmbBOQID.Text = objclsBLLPhases.GetCodeforArrow("BOQ", ucmbBOQID.Text.Trim(), "R");
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbLocCode_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
               
            }
            catch(Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSubPhaseID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (ucmbSubPhaseID.Value != null && ucmbSubPhaseID.Value.ToString().Trim().Length > 0)
                {
                    objclsCommonFunc.fillLocations_By_SubPhaseID(ucmbLocCode, ucmbSubPhaseID.Value.ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvBOQ_Click(object sender, EventArgs e)
        {
            DataSet _objDataset = new DataSet();
            try
            {
                if (udgvBOQ.ActiveCell == null) return;
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvBOQ.ActiveCell.Column.Key == "colLocation")
                {
                    ugR = udgvBOQ.ActiveRow;

                    if (ugR.Cells["colSubPhase"].Value != null && ugR.Cells["colSubPhase"].Value.ToString().Trim().Length > 0)
                    {
                        _objDataset=objclsCommonFunc.fillLocations_By_SubPhaseID(ucmbLocCode, ugR.Cells["colSubPhase"].Value.ToString().Trim());
                        if (_objDataset.Tables.Count > 0 && _objDataset.Tables[0].Rows.Count == 0)
                        {
                            clsBeansPhases tempobjclsBeansPhases = objclsBLLPhases.GetSubPhases_ByID(ugR.Cells["colSubPhase"].Value.ToString());

                            ugR.Cells["colRate"].Value = tempobjclsBeansPhases.Rate;
                            ugR.Cells["colUnits"].Value = tempobjclsBeansPhases.Units;
                        }
                    }
                    else if (ugR.Cells["colPhase"].Value != null && ugR.Cells["colPhase"].Value.ToString().Trim().Length > 0)
                    {
                        objclsCommonFunc.fillLocations_By_PhaseID(ucmbLocCode, ugR.Cells["colPhase"].Value.ToString().Trim());
                        if (_objDataset.Tables.Count > 0 && _objDataset.Tables[0].Rows.Count == 0)
                        {
                            clsBeansPhases tempobjclsBeansPhases = objclsBLLPhases.GetSubPhases_ByID(ugR.Cells["colPhase"].Value.ToString());
                            ugR.Cells["colRate"].Value = tempobjclsBeansPhases.Rate;
                            ugR.Cells["colUnits"].Value = tempobjclsBeansPhases.Units;
                        }
                    }
                    else
                        ucmbLocCode.DataSource = null;                    
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            clsBeansPhases.DtblAttachment = _DocumentList;
            frmUpload objfrmUpload = new frmUpload();
            objfrmUpload.ShowDialog();

            try
            {
                if (clsBeansPhases.DtblAttachment != null)
                {
                    if (clsBeansPhases.DtblAttachment.Rows.Count > 0)
                    {
                        _DocumentList = clsBeansPhases.DtblAttachment;
                    }
                    else
                        _DocumentList = null;

                    clsBeansPhases.DtblAttachment = null;
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();
            bool IsSave=true;

            try
            {
                if (IsValidControls())
                {
                    //DataSet _dts = objclsBLLPhases.GetIssues_BySiteID(ucmbSiteID.Text.Trim());
                    if (lblPosted.Text=="POSTED")
                    {
                        MessageBox.Show("Unable to Post this Again...!");
                        return;
                    }

                    ArrangeDataGrid();
                    objclsBeansPhases.IsPosted = true;
                    objclsBeansPhases.BOQID = ucmbBOQID.Text.Trim();
                    objclsBeansPhases.SiteID = ucmbSiteID.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Date = dtpDate.Value;
                    objclsBeansPhases.RevisionNo = int.Parse(txtRevisionNo.Value.ToString());
                    objclsBeansPhases.EstimatedAmt = double.Parse(txtEstimatedAmount.Value.ToString());
                    objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Value.ToString());
                    objclsBeansPhases.EstimateStartDate = dtpStart.Value;
                    objclsBeansPhases.EstimateEndDate = dtpEnd.Value;
                    objclsBeansPhases.Dtbl = getDatasource_InGrid();
                    objclsBeansPhases.DtblPT = getDatasource_InGrid_PT();
                    if (objclsBeansPhases.Dtbl.Rows.Count > 0)
                        objclsBeansPhases.IsUsePhases = true;
                    else
                        objclsBeansPhases.IsUsePhases = false;
                    objclsBeansPhases.Inactive = chkActive.Checked;
                    objclsBeansPhases.CustomerID = ucmbCustomerID.Text.ToString();
                    //objclsBeansPhases.CustomerName=txts
                    clsBeansPhases _obj = new clsBeansPhases();
                    _obj = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Value.ToString().Trim());

                    objclsBeansPhases.SiteDescription = _obj.Description;
                    objclsBeansPhases.EstimateStartDate = _obj.EstimateStartDate;
                    objclsBeansPhases.EstimateEndDate = _obj.EstimateEndDate;
                    objclsBeansPhases.IsPosted = true;

                    objclsBeansPhases.DtblDocList = _DocumentList;
                    _DocumentList = null;
                    clsBeansPhases.DtblAttachment = null;

                    string _BOQID = objclsBLLPhases.SaveBOQs(objclsBeansPhases, IsSave);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //if(!IsSave)
                    //    MessageBox.Show("Edit This Job Manually in Peachtree...!");

                    SetFormMode();

                    ucmbBOQID.Text = _BOQID;
                    ////ArrangeDataGrid();

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
                    _dataset = objclsBLLPhases.GetBOQs_ByID(ucmbBOQID.Text.Trim());

                    frmRevisionHistory objfrmRevisionHistory = new frmRevisionHistory(_dataset.Tables[0], "BOQ", txtRevisionNo.Text.Trim());
                    objfrmRevisionHistory.ShowDialog();

                    if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue2))
                    {
                        //lblEstIndex.Text = clsBeansFind.ReturnValue2;
                        //txtEstimateNo.Text = clsBeansFind.ReturnValue;
                        FormMode = enmFormMode.Find;
                        objclsBeansPhases = objclsBLLPhases.GetBOQs_ByID(clsBeansFind.ReturnValue,int.Parse(clsBeansFind.ReturnValue2));

                        ucmbBOQID.Value = clsBeansFind.ReturnValue;
                        txtRevisionNo.Value = clsBeansFind.ReturnValue2;

                        fillControls(objclsBLLPhases.GetBOQs_ByID(ucmbBOQID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));
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
                    MessageBox.Show("Select an BOQ to view History");
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

        private void dtpEnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                udgvBOQ.ActiveCell = udgvBOQ.Rows[0].Cells[0];


                //udgvBOQ.ActiveCell = udgvBOQ.Rows[_currntIndex + 1].Cells[0];
                udgvBOQ.PerformAction(UltraGridAction.EnterEditModeAndDropdown, false, false);
                //ucmbPhaseID.PerformAction(UltraComboAction.Dropdown);
            }
            // 
        }

        private void udgvBOQ_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
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

        private void ucmbBOQID_ValueChanged(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                if (ucmbBOQID.Value != null)
                {
                    FormMode = enmFormMode.Find;
                    fillControls(objclsBLLPhases.GetBOQs_ByID_Max(ucmbBOQID.Text.Trim(), int.Parse(txtRevisionNo.Value.ToString())));

                    //ArrangeDataGrid();
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