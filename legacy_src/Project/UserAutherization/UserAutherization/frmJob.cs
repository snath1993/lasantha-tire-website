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
    public partial class frmJob : Form
    {
        clsCommon objclsCommon = new clsCommon();
        clsBLLPhases objclsBLLPhases;
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;
        private string _msgTitle = "Job";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        clsCommonFunc objclsCommonFunc = new clsCommonFunc();
        DataTable _DocumentList;

        public frmJob()
        {
            InitializeComponent();
            ucmbSiteID.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode();
        }

        public frmJob(string _SiteID)
        {
            InitializeComponent();

            try
            {
                if (_SiteID.Trim().Length > 0)
                    ucmbSiteID.Text = _SiteID;
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, "Load", ex.StackTrace);
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
            chkActive.Checked = false;
            txtSupervisor.Text = "";
            ucmbBillingMethod.Text = "";
            ucmbContractorID.Text = "";
            ucmbCustomerID.Text = "";
            ucmbSiteID.Text = "";
            ucmbStatus.Text = "";
            ucmbSubConID.Text = "";

            txtActualAmount.Text = "0.00";
            txtArchName.Text = "";
            txtCustomerName.Text = "";
            txtDescription.Text = "";
            txtEstimatedAmount.Text = "0.00";
            txtProjManName.Text = "";
            txtRetainPers.Text = "";
            txtStructEngName.Text = "";

            dtpEndActual.Value = user.LoginDate;
            dtpEndEst.Value = user.LoginDate;
            dtpStartActual.Value = user.LoginDate;
            dtpStartEst.Value = user.LoginDate;

            txtCompleted.Text = "0";
            txtLabBrdn.Text = "0";
            ucmbCategory.Value = "";
            txtContractorName.Text = "";
            chkUsePhases.Checked = false;

            objclsCommonFunc.fillSubContractors(ucmbSubConID);
            objclsCommonFunc.fillCustomer(ucmbCustomerID);
            objclsCommonFunc.fillJobs(ucmbSiteID);
            objclsCommonFunc.fillBillingMethod(ucmbBillingMethod);
            objclsCommonFunc.fillJobStatus(ucmbStatus);
            objclsCommonFunc.fillContractr(ucmbContractorID);
            objclsCommonFunc.fillJobCategory(ucmbCategory);

            Discard(udgvSubcontractors);
            Discard(udgvIssues);
            Discard(udgvExpences);
            Discard(udgvBOQ);
            Discard(udgvBOM);
           
            //udgvSubcontractors.DataSource = null;
            //Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            //ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

            //ugR.Cells["colID"].Value = "";
            //ugR.Cells["colName"].Value = "";
            //ugR.Cells["colAddress"].Value = "";
            //ugR.Cells["colContact"].Value = "";


            //udgvSubcontractors.DataSource = null;
            //Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            //ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
            //Discard(

            //Infragistics.Win.UltraWinGrid.UltraGridRow ugR;
            //ugR = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();
            _DocumentList = null;
        }

        public void Discard(UltraGrid ug)
        {
            foreach (UltraGridRow ugR in ug.Rows.All)
            {
                ugR.Delete(false);
            }
        }


        private void frmPhases_Load(object sender, EventArgs e)
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


        private void btnClear_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            SetFormMode();
        }

        private void lnkAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridRow ur;
            objclsBeansPhases = new clsBeansPhases();

            try
            {
                //Discard(udgvSubcontractors);

                objclsBeansPhases = objclsBLLPhases.GetSubcontractors_ByID(ucmbSubConID.Text.Trim());

                if (IsGridExitCode(objclsBeansPhases.Code))
                {
                    MessageBox.Show("Sub-Contractor already exists.....!");
                    return;
                }

                ur = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

                ur.Cells["colID"].Value = objclsBeansPhases.Code;
                ur.Cells["colName"].Value = objclsBeansPhases.Description;
                ur.Cells["colAddress"].Value = objclsBeansPhases.Address1 + "," + objclsBeansPhases.Address2;
                ur.Cells["colContact"].Value = objclsBeansPhases.ContactNo;
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        public bool IsGridExitCode(String StrCode)
        {
            try
            {
                foreach (UltraGridRow ugR in udgvSubcontractors.Rows)
                {
                    if (ugR.Cells["colID"].Text == StrCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Save;
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();
            string _JobID = "";

            try
            {
                if (IsValidControls())
                {
                    //objclsBeansPhases.ActualStartDate =DateTime.MinValue;
                    objclsBeansPhases.ActualAmt = double.Parse(txtActualAmount.Text.Trim());
                    objclsBeansPhases.EstimatedAmt = double.Parse(txtEstimatedAmount.Text.Trim());
                    objclsBeansPhases.ActualEndDate = dtpEndActual.Value;
                    objclsBeansPhases.IsActEndDate = dtpEndActual.Checked;
                    objclsBeansPhases.ActualStartDate = dtpStartActual.Value;
                    objclsBeansPhases.IsActStartDate = dtpStartActual.Checked;
                    objclsBeansPhases.EstimateEndDate = dtpEndEst.Value;
                    objclsBeansPhases.IsProjEndDate = dtpEndEst.Checked;
                    objclsBeansPhases.EstimateStartDate = dtpStartEst.Value;
                    objclsBeansPhases.IsProjStartDate = dtpStartEst.Checked;

                    
                    objclsBeansPhases.Architecture = txtArchName.Text.Trim();
                    objclsBeansPhases.BillingMethodID = ucmbBillingMethod.Value.ToString();
                    objclsBeansPhases.Code = ucmbSiteID.Text.ToString().Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Contractor = ucmbContractorID.Text.Trim();
                    objclsBeansPhases.Inactive = chkActive.Checked;
                    objclsBeansPhases.RetainingPersentage = double.Parse(txtRetainPers.Text.Trim());
                    objclsBeansPhases.StatusID = ucmbStatus.Value.ToString();
                    objclsBeansPhases.StructuralEngineer = txtStructEngName.Text.Trim();
                    objclsBeansPhases.Supervisor = txtSupervisor.Text.Trim();
                    objclsBeansPhases.Dtbl = getDatagridSource_Subcontractor();
                    objclsBeansPhases.DtblDocList = _DocumentList;
                    objclsBeansPhases.CustomerID = ucmbCustomerID.Text.Trim();
                    objclsBeansPhases.ProjectManager = txtProjManName.Text.Trim();
                    objclsBeansPhases.Category = ucmbCategory.Text.ToString().Trim();
                    objclsBeansPhases.CompletedPercentage = double.Parse(txtCompleted.Text.Trim());
                    objclsBeansPhases.IsUsePhases = chkUsePhases.Checked;
                    objclsBeansPhases.Labor_Burden_Percent = double.Parse(txtLabBrdn.Text.Trim());
                    _JobID = ucmbSiteID.Text.Trim();
                    //objclsBeansPhases.
                    _DocumentList = null;

                    objclsBLLPhases.SaveJobs(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);

                    SetFormMode();
                    ucmbSiteID.Text = _JobID;
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private bool IsValidControls()
        {
            if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                erpMaster.SetError(txtDescription, "Description Can't be Empty");
                txtDescription.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(ucmbSiteID.Text.Trim()))
            {
                erpMaster.SetError(ucmbSiteID, "Site ID Can't be Empty");
                ucmbSiteID.Focus();
                return false;
            }
            else
                return true;
        }

        private DataTable getDatagridSource_Subcontractor()
        {
            DataTable _dataTable = new DataTable();
            try
            {
                if (udgvSubcontractors.Rows.Count > 0)
                {
                    _dataTable.Columns.Add("ID");
                    _dataTable.Columns.Add("Name");
                    _dataTable.Columns.Add("Address");
                    _dataTable.Columns.Add("Contact");
                    _dataTable.Columns.Add("Remarks");

                    foreach (UltraGridRow dgvr in udgvSubcontractors.Rows)
                    {
                        if (dgvr.Cells["colID"].Value != null && dgvr.Cells["colID"].Value.ToString().Trim().Length > 0)
                        {
                            DataRow drow = _dataTable.NewRow();
                            drow["ID"] = dgvr.Cells["colID"].Value;
                            drow["Name"] = dgvr.Cells["colName"].Value;
                            drow["Address"] = dgvr.Cells["colAddress"].Value;
                            drow["Contact"] = dgvr.Cells["colContact"].Value;
                            drow["Remarks"] = dgvr.Cells["colRemarks"].Value;
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

        private void ucmbSiteID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            //try
            //{
            //    if (ucmbSiteID.Text != null && ucmbSiteID.Text.Trim().Length  > 0)
            //    {
            //        //txtDescription.Text = ucmbSiteID.Value.ToString();
            //        fillControls();
            //    }
            //}
            //catch (Exception ex)
            //{
            //     objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            //}
        }

        private void fillControls()
        {
            objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();
            try
            {
                objclsBeansPhases = objclsBLLPhases.GetJob_ByID(ucmbSiteID.Text.Trim());

                if (objclsBeansPhases.Code != null)
                {
                    txtActualAmount.Text = objclsBeansPhases.ActualAmt.ToString("0.00");
                    txtArchName.Text = objclsBeansPhases.Architecture;
                    txtDescription.Text = objclsBeansPhases.Description;
                    txtEstimatedAmount.Text = objclsBeansPhases.EstimatedAmt.ToString("0.00");
                    txtProjManName.Text = objclsBeansPhases.ProjectManager;
                    txtRetainPers.Text = objclsBeansPhases.RetainingPersentage.ToString("0.00");
                    txtStructEngName.Text = objclsBeansPhases.StructuralEngineer;
                    txtSupervisor.Text = objclsBeansPhases.Supervisor;

                    ucmbBillingMethod.Value = objclsBeansPhases.BillingMethodID;
                    ucmbContractorID.Value = objclsBeansPhases.Contractor;
                    ucmbCustomerID.Value = objclsBeansPhases.CustomerID;
                    //ucmbSiteID.Value = objclsBeansPhases.Code;
                    ucmbStatus.Value = objclsBeansPhases.StatusID;

                    
                    chkActive.Checked = objclsBeansPhases.Inactive;

                    if (objclsBeansPhases.ActualEndDate.Year != 1)
                    {
                        dtpEndActual.Value = objclsBeansPhases.ActualEndDate;
                        if (objclsBeansPhases.ActualEndDate.Year > 1900)
                            dtpEndActual.Checked = true;
                        else dtpEndActual.Checked = false;
                    }
                    if (objclsBeansPhases.ActualStartDate.Year != 1)
                    {
                        dtpStartActual.Value = objclsBeansPhases.ActualStartDate;
                        if (objclsBeansPhases.ActualStartDate.Year > 1900)
                            dtpStartActual.Checked = true;
                        else dtpStartActual.Checked = false;
                    }

                    if (objclsBeansPhases.EstimateEndDate.Year != 1)
                    {
                        dtpEndEst.Value = objclsBeansPhases.EstimateEndDate;
                        if (objclsBeansPhases.EstimateEndDate.Year > 1900)
                            dtpEndEst.Checked = true;
                        else dtpEndEst.Checked = false;
                    }

                    if (objclsBeansPhases.EstimateStartDate.Year != 1)
                    {
                        dtpStartEst.Value = objclsBeansPhases.EstimateStartDate;
                        if (objclsBeansPhases.EstimateStartDate.Year > 1900)
                            dtpStartEst.Checked = true;
                        else dtpStartEst.Checked = false;
                    }

                    ucmbCategory.Value = objclsBeansPhases.Category;
                    txtLabBrdn.Text = objclsBeansPhases.Labor_Burden_Percent.ToString("0.00");
                    txtCompleted.Text = objclsBeansPhases.CompletedPercentage.ToString("0.00");
                    chkUsePhases.Checked = objclsBeansPhases.IsUsePhases;

                    fillSubcontractorGrid(objclsBeansPhases.Dtbl);

                    fillBOQGrid(objclsBLLPhases.GetBOQs_BySiteID(ucmbSiteID.Text.Trim()).Tables[0]);
                    fillBOMGrid(objclsBLLPhases.GetBOMs_BySiteID(ucmbSiteID.Text.Trim()).Tables[0]);
                    fillIssuesGrid(objclsBLLPhases.GetIssues_BySiteID(ucmbSiteID.Text.Trim()).Tables[0]);
                    fillExpencesGrid(objclsBLLPhases.GetExpences_By_SiteID(ucmbSiteID.Text.Trim()).Tables[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillIssuesGrid(DataTable dataTable)
        {
            try
            {
                Discard(udgvIssues);

                Infragistics.Win.UltraWinGrid.UltraGridRow ur;
                if (objclsBeansPhases.Dtbl == null) return;
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        ur = udgvIssues.DisplayLayout.Bands[0].AddNew();
                        ur.Cells["colBOQ"].Value = dr["IssueNo"].ToString();
                        ur.Cells["colPhase"].Value = dr["PhaseID"].ToString();
                        ur.Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                        ur.Cells["colQty"].Value = dr["IssuedQty"].ToString();
                        ur.Cells["colItem"].Value = dr["ItemID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillBOQGrid(DataTable dataTable)
        {
            try
            {
                Discard(udgvBOQ);

                Infragistics.Win.UltraWinGrid.UltraGridRow ur;
                if (objclsBeansPhases.Dtbl == null) return;
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        ur = udgvBOQ.DisplayLayout.Bands[0].AddNew();
                        ur.Cells["colBOQ"].Value = dr["BOQID"].ToString();
                        ur.Cells["colPhase"].Value = dr["PhaseID"].ToString();
                        ur.Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                        ur.Cells["colQty"].Value = dr["Qty"].ToString();
                        ur.Cells["colAmount"].Value = dr["Amount"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillExpencesGrid(DataTable dataTable)
        {
            try
            {
                Discard(udgvExpences);

                Infragistics.Win.UltraWinGrid.UltraGridRow ur;
                if (objclsBeansPhases.Dtbl == null) return;
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        ur = udgvExpences.DisplayLayout.Bands[0].AddNew();
                        ur.Cells["colPhaseID"].Value = dr["PhaseID"].ToString();
                        ur.Cells["colSubPhaseID"].Value = dr["SubPhaseID"].ToString();
                        ur.Cells["colUnits"].Value = dr["Units"].ToString();
                        ur.Cells["colEstQty"].Value = dr["BOQQty"].ToString();
                        ur.Cells["colActQty"].Value = dr["IssuedQty"].ToString();
                        ur.Cells["colEstAmt"].Value = dr["Amount"].ToString();
                        ur.Cells["colActAmt"].Value =  dr["LastUnitCost"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillBOMGrid(DataTable dataTable)
        {
            try
            {
                Discard(udgvBOM);

                Infragistics.Win.UltraWinGrid.UltraGridRow ur;
                if (objclsBeansPhases.Dtbl == null) return;
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        ur = udgvBOM.DisplayLayout.Bands[0].AddNew();
                        ur.Cells["colBOQ"].Value = dr["BOMID"].ToString();
                        ur.Cells["colPhase"].Value = dr["PhaseID"].ToString();
                        ur.Cells["colSubPhase"].Value = dr["SubPhaseID"].ToString();
                        ur.Cells["colQty"].Value = dr["Qty"].ToString();
                        ur.Cells["colAmount"].Value = dr["Amount"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void fillSubcontractorGrid(DataTable dataTable)
        {
            try
            {
                Discard(udgvSubcontractors);

                Infragistics.Win.UltraWinGrid.UltraGridRow ur;
                if (objclsBeansPhases.Dtbl == null) return;
                if (objclsBeansPhases.Dtbl.Rows.Count > 0)
                {
                    foreach (DataRow dr in objclsBeansPhases.Dtbl.Rows)
                    {
                        ur = udgvSubcontractors.DisplayLayout.Bands[0].AddNew();

                        ur.Cells["colID"].Value = dr["SubContractor"].ToString();
                        ur.Cells["colName"].Value = dr["Name"].ToString();
                        ur.Cells["colRemarks"].Value = dr["Remarks"].ToString();
                        ur.Cells["colContact"].Value = dr["Contact"].ToString();
                        ur.Cells["colAddress"].Value = dr["Address"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                objDataSet = new DataSet();
                objDataSet = objclsBLLPhases.GetJob_AllActive_ForDropDown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Job");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbSiteID.Text = clsBeansFind.ReturnValue;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Delete;
                objclsBLLPhases = new clsBLLPhases();
                objclsBeansPhases = new clsBeansPhases();

                if (IsValidControls())
                {
                    objclsBeansPhases.Code = ucmbSiteID.Text.Trim();

                    objclsBLLPhases.DeleteJob(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbCustomerID_RowSelected(object sender, RowSelectedEventArgs e)
        {
            
        }

        private void tsbtnFirst_Click(object sender, EventArgs e)
        {
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "LL");
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
                ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "RR");
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
                if(ucmbSiteID.Text.Trim().Length > 0)
                    ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "L");
                if (ucmbSiteID.Text.Trim().Length == 0)
                    ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "LL");
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
                if (ucmbSiteID.Text.Trim().Length > 0)
                    ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "R");
                if (ucmbSiteID.Text.Trim().Length == 0)
                    ucmbSiteID.Text = objclsBLLPhases.GetCodeforArrow("Job", ucmbSiteID.Text.Trim(), "RR");
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

        private void ucmbStatus_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (ucmbStatus.Text.Trim() == "In progress")
                    ucmbStatus.Appearance.BackColor = Color.Lime;
                else if (ucmbStatus.Text.Trim() == "Pending")
                    ucmbStatus.Appearance.BackColor = Color.Yellow;
                else if (ucmbStatus.Text.Trim() == "Completed")
                    ucmbStatus.Appearance.BackColor = Color.Cyan;
                else if (ucmbStatus.Text.Trim() == "On hold")
                    ucmbStatus.Appearance.BackColor = Color.Red;
                else
                    ucmbStatus.Appearance.BackColor = Color.White;
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

        private void ucmbContractorID_ValueChanged(object sender, EventArgs e)
        {
            clsBLLPhases _oobjclsBLLPhases = new clsBLLPhases();
            //objclsBeansPhases = new clsBeansPhases();
            try
            {
                if (ucmbContractorID.Value != null)
                {
                    txtContractorName.Text = _oobjclsBLLPhases.GetCustomer_ByID(ucmbContractorID.Text.Trim().ToString()).Description;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            //     objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbSiteID_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (ucmbSiteID.Text != null && ucmbSiteID.Text.Trim().Length > 0)
                {
                    //txtDescription.Text = ucmbSiteID.Value.ToString();
                    fillControls();
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            clsBBLPTImport objclsBBLPTImport = new clsBBLPTImport();
            try
            {
                objclsBBLPTImport.ImportJob_List(ultraProgressBar1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}