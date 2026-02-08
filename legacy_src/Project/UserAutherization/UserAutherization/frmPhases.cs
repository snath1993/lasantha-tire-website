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
    public partial class frmPhases : Form
    {
        clsCommon objclsCommon = new clsCommon();

        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "Phases";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();

        public frmPhases()
        {
            InitializeComponent();

            ucmbPhaseID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode(); 
        }

        public frmPhases(string PhaseID)
        {
            InitializeComponent();           

            ucmbPhaseID.Text = clsBeansFind.ReturnValue;
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
                ucmbPhaseID.DataSource = objclsBLLPhases.GetPhases_AllActive_ForDropDown();
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

        private void FillLocations()
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

        private void FillCostCodes()
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

        private void ultraCombo1_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
           
        }

        private void fillControls()
        {
            try
            {
                if (ucmbPhaseID.Text == null && ucmbPhaseID.Text.Trim().Length == 0) return;

                objclsBeansPhases = new clsBeansPhases();
                objclsBLLPhases=new clsBLLPhases ();

                objclsBeansPhases = objclsBLLPhases.GetPhases_ByID(ucmbPhaseID.Text.Trim());

                ucmbPhaseID.Text = objclsBeansPhases.Code;
               // ucmbPhaseID.Value = objclsBeansPhases.Code;
                txtDescription.Text = objclsBeansPhases.Description;
                txtMargine.Text = objclsBeansPhases.Margine.ToString("0.00");
                txtRate.Text = objclsBeansPhases.Rate.ToString("0.00");
                txtUnits.Text = objclsBeansPhases.Units;
                if (objclsBeansPhases.CostTypeID != "0") ucmbCostType.Value = objclsBeansPhases.CostTypeID;
                else ucmbCostType.Value = "";
                rbtCostCode.Checked = objclsBeansPhases.UseCostCode;

                if (objclsBeansPhases.UseCostCode)
                {
                    tabControl1.Enabled = false;
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                if (IsValidControls())
                {
                    if (txtRateIncrease.Text.Trim().Length > 0 && double.Parse(txtRateIncrease.Text)>0)
                    {
                        objclsBeansPhases.Code = ucmbPhaseID.Text.Trim();
                        objclsBeansPhases.RateIncrease = double.Parse(txtRateIncrease.Text.Trim());
                        objclsBeansPhases.IsApplyToAll = chkApply.Checked;
                        objclsBeansPhases.Date = dtpDate.Value;

                        objclsBLLPhases.UpdatePhasesRates(objclsBeansPhases);
                        MessageBox.Show(_msgTitle + " Rate Incremented Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                    else
                    {
                        objclsBeansPhases.Code = ucmbPhaseID.Text.Trim();
                        objclsBeansPhases.Description = txtDescription.Text.Trim();
                        objclsBeansPhases.Inactive = chkActive.Checked;
                        objclsBeansPhases.UseCostCode = rbtCostCode.Checked;
                        objclsBeansPhases.Units = txtUnits.Text.Trim();
                        objclsBeansPhases.Rate = double.Parse(txtRate.Text.Trim());
                        objclsBeansPhases.Margine = double.Parse(txtMargine.Text.Trim());
                        if (ucmbCostType.Value != null) objclsBeansPhases.CostTypeID = ucmbCostType.Value.ToString();
                        else objclsBeansPhases.CostTypeID = "0";

                        objclsBeansPhases.Dtbl = getDatagridSource_LocationWise();

                        objclsBLLPhases.SavePhases(objclsBeansPhases);
                        MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }                  
                    SetFormMode();
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
            txtMargine.Text = "0.00";
            txtRate.Text = "0.00";
            txtRateIncrease.Text = "0.00";
            txtUnits.Text = "";
            chkActive.Checked = false;
            chkApply.Checked = false;
            ucmbCostType.Text = "";
            ucmbLocCode.Text = "";
            ucmbPhaseID.Focus();
            rbtCostCode.Checked = false;
            Discard();
            FillCostCodes();
            FillLocations();
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

        private void rbtCostCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbtCostCode.Checked)
                {
                    txtMargine.Text = "0.00";
                    txtMargine.ReadOnly = true;
                    txtRate.Text = "0.00";
                    txtRate.ReadOnly = true;
                    txtUnits.Text = "";
                    txtUnits.ReadOnly = true;
                    Discard();
                    udgvLocWiseRates.Enabled = false;
                }
                else
                {
                    txtMargine.ReadOnly = false;
                    txtRate.ReadOnly = false;
                    txtUnits.ReadOnly = false;
                    udgvLocWiseRates.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            SetFormMode();
        }

        private void ucmbPhaseID_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void ucmbPhaseID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtDescription.Focus();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                rbtCostCode.Focus();
        }

        private void rbtCostCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                ucmbCostType.Focus();
        }

        private void ucmbCostType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtUnits.Focus();
        }

        private void txtUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtRate.Focus();
        }

        private void txtRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtMargine.Focus();
        }

        private void txtMargin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                btnSave.Select();
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
                    objclsBeansPhases.Code = ucmbPhaseID.Text.Trim();

                    objclsBLLPhases.DeletePhases(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);

                    SetFormMode();
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
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetPhases_AllActive_ForDropDown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Phases");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbPhaseID.Text = clsBeansFind.ReturnValue;
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

        private void udgvLocWiseRates_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
                if (udgvLocWiseRates.ActiveCell.Column.Key == "colDelete")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                    {
                        ugR = udgvLocWiseRates.DisplayLayout.Bands[0].AddNew();
                        ugR.Cells[0].Activated = true;
                        ugR.Cells[0].Activation = Activation.AllowEdit;

                        //                    ucmbLocCode.PerformAction(UltraComboAction.Dropdown);//UltraGridAction.EnterEditModeAndDropdown);
                    }
                }
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
                ucmbPhaseID.Text = objclsBLLPhases.GetCodeforArrow("Phases", ucmbPhaseID.Text.Trim(), "LL");
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
                ucmbPhaseID.Text = objclsBLLPhases.GetCodeforArrow("Phases", ucmbPhaseID.Text.Trim(), "RR");
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
                ucmbPhaseID.Text = objclsBLLPhases.GetCodeforArrow("Phases", ucmbPhaseID.Text.Trim(), "R");
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
                ucmbPhaseID.Text = objclsBLLPhases.GetCodeforArrow("Phases", ucmbPhaseID.Text.Trim(), "L");
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
                if (ucmbPhaseID.Text != null)
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

        private void ucmbPhaseID_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].ColumnFilters[0].FilterConditions.Add(FilterComparisionOperator.Like, ucmbPhaseID.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                objclsBBLPTImport.ImportPhases_List(ultraProgressBar1);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbtCostCode_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                if(rbtCostCode.Checked)
                    tabControl1.Enabled = false;
                else
                    tabControl1.Enabled = true;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }





       

      

        
        
        

        

       

       
    }
}