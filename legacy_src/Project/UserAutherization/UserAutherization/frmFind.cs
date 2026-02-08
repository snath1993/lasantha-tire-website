using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PCMBeans;
//using TJaySmBeans.Master;
//using TJaySmBLL.Master;
//using TJaySmGUI.Master;
using ComFunction;

namespace UserAutherization
{
    public partial class frmFind : Form
    {
        clsCommon objclsCommon = new clsCommon();
        //Common objCommon = new Common();     
        string msgTitle = "Find";
        private string _CurrentColumn;
        private string _ReturnValue;
        enmFormMode FormMode;
        private string CurrentForm = "";

        public frmFind()
        {
            InitializeComponent();
        }

        public frmFind(DataSet _DataSet,string _CurrentForm)
        {
            InitializeComponent();
            dgvFind.DataSource = _DataSet.Tables[0];
            CurrentForm = _CurrentForm;
        }

        public frmFind(string _CurrentForm)
        {
            InitializeComponent();
            CurrentForm = _CurrentForm;
        }

        private void frmFind_Load(object sender, EventArgs e)
        {
            try
            {
                dgvFind.DataSource = clsBeansFind.DataTable;
            }
            catch (Exception ex)
            {
               objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }       

        private void txtFindWord_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SearchKeyword();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }      

            

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                SearchKeyword();
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void SearchKeyword()
        {
            try
            {

                Common _objCommon = new Common();
                _objCommon.SearchKeyword(txtFindWord.Text.Trim(), dgvFind, clsBeansFind.DataTable, _CurrentColumn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Save;
                clsBeansFind.ReturnValue = dgvFind.CurrentRow.Cells[0].Value.ToString();
                this.Close();
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

       

        private void btnExit_Click(object sender, EventArgs e)
        {
            FormMode = enmFormMode.Clear;
            clsBeansFind.ReturnValue = "";
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtFindWord.Text = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFindWord.Text = "";
        }

        private void dgvFind_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                _CurrentColumn = dgvFind.Columns[e.ColumnIndex].HeaderText;
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvFind_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                FormMode = enmFormMode.Save;
                clsBeansFind.ReturnValue = dgvFind.CurrentRow.Cells[0].Value.ToString();
                this.Close();

                if (CurrentForm == "Location")
                {
                    if (frmMain.objfrmLocation == null || frmMain.objfrmLocation.IsDisposed)
                    {
                        frmMain.objfrmLocation = new frmLocation(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmLocation.Show();
                    frmMain.objfrmLocation.TopMost = true;
                    frmMain.objfrmLocation.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "Phases")
                {
                    if (frmMain.objfrmPhases == null || frmMain.objfrmPhases.IsDisposed)
                    {
                        frmMain.objfrmPhases = new frmPhases(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmPhases.Show();
                    frmMain.objfrmPhases.TopMost = true;
                    frmMain.objfrmLocation.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "Sub Phases")
                {
                    if (frmMain.objfrmSubPhases == null || frmMain.objfrmSubPhases.IsDisposed)
                    {
                        frmMain.objfrmSubPhases = new frmSubPhases(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmSubPhases.Show();
                    frmMain.objfrmSubPhases.TopMost = true;
                    frmMain.objfrmSubPhases.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "Job")
                {
                    if (frmMain.objfrmJob == null || frmMain.objfrmJob.IsDisposed)
                    {
                        frmMain.objfrmJob = new frmJob(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmJob.Show();
                    frmMain.objfrmJob.TopMost = true;
                    frmMain.objfrmJob.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "BOQ")
                {
                    if (frmMain.objfrmBOQ == null || frmMain.objfrmBOQ.IsDisposed)
                    {
                        frmMain.objfrmBOQ = new frmBOQ(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmBOQ.Show();
                    frmMain.objfrmBOQ.TopMost = true;
                    frmMain.objfrmBOQ.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "BOM")
                {
                    if (frmMain.objfrmBOM == null || frmMain.objfrmBOM.IsDisposed)
                    {
                        frmMain.objfrmBOM = new frmBOM(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmBOM.Show();
                    frmMain.objfrmBOM.TopMost = true;
                    frmMain.objfrmBOM.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "Site Issue")
                {
                    if (frmMain.objfrmSiteIssues == null || frmMain.objfrmSiteIssues.IsDisposed)
                    {
                        frmMain.objfrmSiteIssues = new frmSiteIssues(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmSiteIssues.Show();
                    frmMain.objfrmSiteIssues.TopMost = true;
                    frmMain.objfrmSiteIssues.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
                if (CurrentForm == "Site Return")
                {
                    if (frmMain.objfrmSiteIssuesReturn == null || frmMain.objfrmSiteIssuesReturn.IsDisposed)
                    {
                        frmMain.objfrmSiteIssuesReturn = new frmSiteIssuesReturn(clsBeansFind.ReturnValue);
                    }

                    frmMain.objfrmSiteIssuesReturn.Show();
                    frmMain.objfrmSiteIssuesReturn.TopMost = true;
                    frmMain.objfrmSiteIssuesReturn.WindowState = FormWindowState.Normal;
                    frmMain.objfrmFind.TopMost = false;
                }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmFind_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(FormMode != enmFormMode.Save)
                clsBeansFind.ReturnValue = "";
        }

        

        private void dgvFind_KeyDown(object sender, KeyEventArgs e)
        {          
            try
            {
                 if (e.KeyCode == Keys.Enter)
                 {
                        FormMode = enmFormMode.Save;
                        clsBeansFind.ReturnValue = dgvFind.CurrentRow.Cells[0].Value.ToString();
                        //frmDesignation objfrmDesignation = new frmDesignation();
                        // objfrmDesignation.txtCode.Text = dgvFind.CurrentRow.Cells[0].Value.ToString();
                        this.Close();
                 }
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void tsBtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                frmFind_Load(sender, e);
                txtFindWord.Text = "";
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }       

        private void dgvFind_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                _CurrentColumn = dgvFind.Columns[e.ColumnIndex].HeaderText;
            }
            catch (Exception ex)
            {
                                objclsCommon.ErrorLog(msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        

        

       
       
    }
}
