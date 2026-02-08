using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBLL;
using ComFunction;
using PCMBeans;

namespace UserAutherization
{
    public partial class frmLocation : Form
    {
        clsCommon objclsCommon = new clsCommon();

        clsBLLPhases objclsBLLPhases;
        private string _msgTitle = "Locations";
        DataSet objDataSet;
        clsCommon objCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        clsBeansPhases objclsBeansPhases;
        enmFormMode FormMode;


        public frmLocation()
        {
            InitializeComponent();
            ucmbLocCode.Focus();
            FormMode = enmFormMode.Initialize;
            SetFormMode(); 
        }

        public frmLocation(string LocCode)
        {
            InitializeComponent();

            ucmbLocCode.Focus();
            //FormMode = enmFormMode.Initialize;
            //SetFormMode(); 

            ucmbLocCode.Text = LocCode;
            fillControls();
            FormMode = enmFormMode.Find;
            SetFormMode();
        }

         

        private void frmLocation_Load(object sender, EventArgs e)
        {            
            try
            {
                               
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbLocCode_RowSelected(object sender, Infragistics.Win.UltraWinGrid.RowSelectedEventArgs e)
        {
            try
            {
                if (ucmbLocCode.Value != null)
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Save;
                objclsBeansPhases = new clsBeansPhases();
                objclsBLLPhases = new clsBLLPhases();

                if (IsValidControls())
                {
                    objclsBeansPhases.Code = ucmbLocCode.Text.Trim();
                    objclsBeansPhases.Description = txtDescription.Text.Trim();
                    objclsBeansPhases.Inactive = chkActive.Checked;

                    objclsBLLPhases.SaveLocations(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Saved Successfully", _msgTitle, MessageBoxButtons.OK,MessageBoxIcon.Information);

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
                case enmFormMode.New:
                    btnSave.Enabled = true;
                    btnDelete.Enabled = true;
                    btnFind.Enabled = true;
                    btnClear.Enabled = true;
                    ClearControlers();
                    break;
            }
        }

        private void ClearControlers()
        {
            if (FormMode != enmFormMode.New)
            {
                ucmbLocCode.Text = "";
                ucmbLocCode.Focus();
            }
            txtDescription.Text = "";
            chkActive.Checked = false;
            
            FillCombo();           
        }

        private void FillCombo()
        {
            try
            {
                objclsBLLPhases = new clsBLLPhases();
                ucmbLocCode.DataSource = null;
                ucmbLocCode.DataSource = objclsBLLPhases.GetLocations_All();
                ucmbLocCode.DisplayMember = "ID";
                ucmbLocCode.ValueMember = "Name";
            }
            catch (Exception ex)
            {
                throw ex;
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
            if (string.IsNullOrEmpty(ucmbLocCode.Text.Trim()))
            {
                erpMaster.SetError(ucmbLocCode, "Code Can't be Empty");
                ucmbLocCode.Focus();
                return false;
            }
            else
                return true;
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
                objDataSet = objclsBLLPhases.GetLocations_AllActive_Dropdown();
                clsBeansFind.DataTable = objDataSet.Tables[0];

                if (frmMain.objfrmFind == null || frmMain.objfrmFind.IsDisposed)
                {
                    frmMain.objfrmFind = new frmFind("Location");
                }
                this.TopMost = false;
                frmMain.objfrmFind.ShowDialog();
                frmMain.objfrmFind.TopMost = true; 

                if (!string.IsNullOrEmpty(clsBeansFind.ReturnValue))
                {
                    ucmbLocCode.Text = clsBeansFind.ReturnValue;
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

        private void fillControls()
        {
            clsBeansPhases _objclsBeansPhases = new clsBeansPhases();
            objclsBLLPhases = new clsBLLPhases();

            try
            {
                _objclsBeansPhases = objclsBLLPhases.GetLocations_ByID(ucmbLocCode.Text.Trim());

                if (_objclsBeansPhases.Code != null)
                {
                    FormMode = enmFormMode.Find;
                    ucmbLocCode.Text = _objclsBeansPhases.Code;
                    txtDescription.Text = _objclsBeansPhases.Description;
                    chkActive.Checked = _objclsBeansPhases.Inactive;
                }
                else
                {
                    FormMode = enmFormMode.New;
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
                    objclsBeansPhases.Code = ucmbLocCode.Text.Trim();

                    objclsBLLPhases.DeleteLocations(objclsBeansPhases);
                    MessageBox.Show(_msgTitle + " Deleted Successfully", _msgTitle, MessageBoxButtons.OK);

                    SetFormMode();
                }
            }
            catch (Exception ex)
            {
                 objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void ucmbLocCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                txtDescription.Focus();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                //    btnSave.(); 
                toolStrip1.Focus();
                btnSave.Select();

            }
        }

        
    }
}