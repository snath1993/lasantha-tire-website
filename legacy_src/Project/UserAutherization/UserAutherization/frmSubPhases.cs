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
    public partial class frmSubPhases : Form
    {
        clsCommon objclsCommon = new clsCommon();

        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "Sub Phases";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();

        public frmSubPhases()
        {
            InitializeComponent();
            ucmbSubPhaseID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();
        }

        public frmSubPhases(string SUbPhaseID)
        {
            InitializeComponent();
            ucmbSubPhaseID.Text = SUbPhaseID;
            fillControls();
            FormMode = enmFormMode.Find;
            SetFormMode();
        }

        public void Discard()
        {
            foreach (UltraGridRow ugR in udgvLocWiseRates.Rows.All)
            {
                ugR.Delete(false);
            }
        }

        private void FillPhases()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbPhaseID.DataSource = objclsBLLPhases.GetPhases_All_Dropdown_WithBreakdown();
                ucmbPhaseID.DisplayMember = "PhaseID";
                ucmbPhaseID.ValueMember = "PhaseDesc";
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbPhaseID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillSubPhases()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbSubPhaseID.DataSource = objclsBLLPhases.GetSubPhases_All();
                ucmbSubPhaseID.DisplayMember = "SubPhaseID";
                ucmbSubPhaseID.ValueMember = "SubPhaseDesc";
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbSubPhaseID.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillLocation()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbLocCode.DataSource = objclsBLLPhases.GetLocations_AllActive_Dropdown();
                ucmbLocCode.DisplayMember = "ID";
                ucmbLocCode.ValueMember = "ID";
                ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbLocCode.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbLocCode.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillCostTypes()
        {
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                ucmbCostType.DataSource = objclsBLLPhases.GetCostTypes_AllActive();
                ucmbCostType.DisplayMember = "CostTypeDesc";
                ucmbCostType.ValueMember = "CostTypeID";
                ucmbCostType.DisplayLayout.Bands[0].Columns[0].Width = 40;
                ucmbCostType.DisplayLayout.Bands[0].Columns[1].Width = 100;
                ucmbCostType.DisplayLayout.Bands[0].Columns[0].Header.Caption = "ID";
                ucmbCostType.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmPhases_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }     
       

        private void fillControls()
        {
            try
            {
                if (ucmbSubPhaseID.Text == null && ucmbSubPhaseID.Text.Trim().Length == 0) return;

                objclsBeansPhases = new clsBeansPhases();
                objclsBLLPhases=new clsBLLPhases ();

                objclsBeansPhases = objclsBLLPhases.GetSubPhases_ByID(ucmbSubPhaseID.Text.Trim());

                ucmbSubPhaseID.Text = objclsBeansPhases.Code;
                ucmbPhaseID.Value = objclsBeansPhases.PhaseID;
                txtDescription.Text = objclsBeansPhases.Description;
                txtMargin.Text = objclsBeansPhases.Margine.ToString("0.00");
                txtRate.Text = objclsBeansPhases.Rate.ToString("0.00");
                txtUnits.Text = objclsBeansPhases.Units;
                if (objclsBeansPhases.CostTypeID != "0") ucmbCostType.Value = objclsBeansPhases.CostTypeID;
                else ucmbCostType.Value = "";

                Discard();

                if (objclsBeansPhases.Dtbl.Rows.Count > 0)
                {                   
                    Infragistics.Win.UltraWinGrid.UltraGridRow ur;

                    foreach (DataRow dr in objclsBeansPhases.Dtbl.Rows)
                    {
                        ur = udgvLocWiseRates.DisplayLayout.Bands[0].AddNew();

                        ur.Cells["colLocation"].Value = dr["LocationCode"].ToString();
                        ur.Cells["colRate"].Value = dr["Rate"].ToString();
                        ur.Cells["colMargine"].Value = dr["Margine"].ToString();
                    }
                }
                else
                {
                    Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                    ugR = udgvLocWiseRates.DisplayLayout.Bands[0].AddNew();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      

        private void ultraCombo1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColumnFilters[0].FilterConditions.Add(FilterComparisionOperator.Like, ucmbPhaseID.Text);
        }

        private void lnkLocwiseRate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Save;
                objclsBeansPhases = new clsBeansPhases();
                objclsBLLPhases = new clsBLLPhases();
                string _TempSubPhaseID = "";

                if (IsValidControls())
                {
                    if (txtRateIncrease.Text.Trim().Length > 0 && double.Parse(txtRateIncrease.Text) > 0)
                    {
                        objclsBeansPhases.Code = ucmbSubPhaseID.Text.Trim();
                        objclsBeansPhases.RateIncrease = double.Parse(txtRateIncrease.Text.Trim());
                        objclsBeansPhases.IsApplyToAll = chkApply.Checked;
                        objclsBeansPhases.Date = dtpDate.Value;

                        objclsBLLPhases.UpdateSubPhasesRates(objclsBeansPhases);
                        MessageBox.Show(_msgTitle + " Rate Incremented Successfully", _msgTitle, MessageBoxButtons.OK);
                    }
                    else
                    {
                        objclsBeansPhases.Code = ucmbSubPhaseID.Text.Trim();
                        objclsBeansPhases.PhaseID = ucmbPhaseID.Text.Trim();
                        objclsBeansPhases.Description = txtDescription.Text.Trim();
                        objclsBeansPhases.Inactive = chkActive.Checked;
                        objclsBeansPhases.UseCostCode = chkApply.Checked;
                        objclsBeansPhases.Units = txtUnits.Text.Trim();
                        objclsBeansPhases.Rate = double.Parse(txtRate.Text.Trim());
                        objclsBeansPhases.Margine = double.Parse(txtMargin.Text.Trim());
                        if (ucmbCostType.Value != null) objclsBeansPhases.CostTypeID = ucmbCostType.Value.ToString();
                        else objclsBeansPhases.CostTypeID = "0";

                        objclsBeansPhases.Dtbl = getDatagridSource_LocationWise();
                        _TempSubPhaseID = ucmbSubPhaseID.Text.Trim();

                        objclsBLLPhases.SaveSubPhases(objclsBeansPhases);
                        MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK);

                        
                    }

                    SetFormMode();
                    ucmbSubPhaseID.Text = _TempSubPhaseID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
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

        private void ClearControlers()
        {
            ucmbPhaseID.Text = "";
            txtDescription.Text = "";
            txtMargin.Text = "0.00";
            txtRate.Text = "0.00";
            txtRateIncrease.Text = "0.00";
            txtUnits.Text = "";
            chkActive.Checked = false;
            chkApply.Checked = false;
            ucmbCostType.Text = "";
            ucmbSubPhaseID.Text = "";

            Discard();
            FillCostTypes();
            FillLocation();
            FillSubPhases();
            FillPhases();

            Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            ugR = udgvLocWiseRates.DisplayLayout.Bands[0].AddNew();
        }

        private bool IsValidControls()
        {
            if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                erpMaster.SetError(txtDescription, "Description Can't be Empty");
                txtDescription.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(ucmbPhaseID.Text.Trim()))
            {
                erpMaster.SetError(ucmbPhaseID, "Code Can't be Empty");
                ucmbPhaseID.Focus();
                return false;
            }
            //if (ucmbSubPhaseID.Value == null)
            //    return;
            if (ucmbSubPhaseID.Value == null)
            {
                erpMaster.SetError(ucmbSubPhaseID, "Code Can't be Empty");
                ucmbSubPhaseID.Focus();
                return false;
            }
            else
                return true;
        }

        private DataTable getDatagridSource_LocationWise()
        {
            DataTable _dataTable = new DataTable();
            try
            {
                if (udgvLocWiseRates.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("LocCode");
                    _dataTable.Columns.Add("Rate");
                    _dataTable.Columns.Add("Margine");

                    foreach (UltraGridRow dgvr in udgvLocWiseRates.Rows)
                    {
                        if (dgvr.Cells["colLocation"].Value != null && dgvr.Cells["colLocation"].Value.ToString().Trim().Length > 0 &&
                            dgvr.Cells["colRate"].Value != null && dgvr.Cells["colRate"].Value.ToString().Trim().Length > 0)
                        {                            
                            DataRow drow = _dataTable.NewRow();
                            drow["LocCode"] = dgvr.Cells["colLocation"].Value;
                            drow["Rate"] = dgvr.Cells["colRate"].Value;
                            drow["Margine"] = dgvr.Cells["colMargine"].Value;                                
                            _dataTable.Rows.Add(drow);
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
       
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                tabControl1.SelectedTab = tabControl1.TabPages[2];
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

      

        private void ucmbSubPhase_RowSelected(object sender, RowSelectedEventArgs e)
        {
            try
            {
                if (ucmbSubPhaseID.Text != null)
                {
                    FormMode = enmFormMode.Find;
                    fillControls();
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

        private void ucmbSubPhaseID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                ucmbPhaseID.Focus();
        }

        private void ucmbPhaseID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtDescription.Focus();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                ucmbCostType.Focus();
        }

        private void ucmbCostType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {               
                tabControl1.SelectedTab = tabControl1.TabPages[2];
                txtUnits.Focus();
            }
        }

        private void txtUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtRate.Focus();
        }

        private void txtRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtMargin.Focus();
        }

        private void txtMargin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                toolStrip1.Focus();
                btnSave.Select();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            SetFormMode();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetSubPhases_AllActive_ForDropDown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Sub Phases");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbSubPhaseID.Text = clsBeansFind.ReturnValue;
                    fillControls();
                    FormMode = enmFormMode.Find;
                    SetFormMode();
                }
                else
                {
                    FormMode = enmFormMode.Save;
                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
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
                    objclsBeansPhases.Code = ucmbSubPhaseID.Text.Trim();

                    objclsBLLPhases.DeleteSubPhases(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void udgvLocWiseRates_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvLocWiseRates.ActiveCell.Column.Key == "colMargine")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        ugR = udgvLocWiseRates.DisplayLayout.Bands[0].AddNew();
                        //ugR.Cells["colLocation"].Activated = true;
                        //ugR.Cells["colLocation"].Activation = Activation.AllowEdit;
                        //                    ucmbLocCode.PerformAction(UltraComboAction.Dropdown);//UltraGridAction.EnterEditModeAndDropdown);
                    }
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
                ucmbSubPhaseID.Text = objclsBLLPhases.GetCodeforArrow("SubPhase", ucmbSubPhaseID.Text.Trim(), "RR");
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
                ucmbSubPhaseID.Text = objclsBLLPhases.GetCodeforArrow("SubPhase", ucmbSubPhaseID.Text.Trim(), "LL");
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
                ucmbSubPhaseID.Text = objclsBLLPhases.GetCodeforArrow("SubPhase", ucmbSubPhaseID.Text.Trim(), "L");
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
                ucmbSubPhaseID.Text = objclsBLLPhases.GetCodeforArrow("SubPhase", ucmbSubPhaseID.Text.Trim(), "R");
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbPhaseID_VisibleChanged(object sender, EventArgs e)
        {
            erpMaster.Dispose();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            erpMaster.Dispose();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                objclsBBLPTImport.ImportSubPhases_List(ultraProgressBar1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

      

        
        
        

        

       

       
    }
}