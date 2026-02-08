using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComFunction;
using PCMBLL;
using PCMBeans;

namespace UserAutherization
{
    public partial class frmRevisionHistory : Form
    {
        string _msgTitle = "Revision History";
        clsCommon objclsCommon = new clsCommon();
        Controlers objControlers = new Controlers();
        clsBLLPhases objclsBLLQuotation = new clsBLLPhases();
        private string _DocumentType = "";
        private string _RefNo = "";
        DataTable _Datatable;

        public frmRevisionHistory()
        {
            InitializeComponent();
            _Datatable = new DataTable();
            try
            {
                //_Datatable = objclsBLLQuotation.GetRevisionHistory_All().Tables[0];
                fillDataGrid(_Datatable);
            }
            catch (Exception ex)
            {
                //objclsCommon.ErrorLog("Add Warehouse", ex.Message, sender.ToString(), ex.StackTrace);                

                objclsCommon.ErrorLog(_msgTitle, ex.Message, "Load", ex.StackTrace);
            }
        }

        public frmRevisionHistory(DataTable RevisionList,string Document,string RevisionNo)
        {
            //fill the form accourding to the revision no and the Document(Quotation,SA,Estimate);
            InitializeComponent();
            try
            {
                fillDataGrid(RevisionList);
                cmbRevNo.SelectedText = RevisionNo;
                _DocumentType = Document;
                cmbDocType.Text = _DocumentType;               
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

        private void fillDataGrid(DataTable RevisionList)
        {
            try
            {
                dgvRevisionHistry.DataSource = RevisionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {  
            try
            {
                cmbRevNo.SelectedIndex = cmbRevNo.SelectedIndex + 1;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbRevNo.SelectedIndex != -1)
                    cmbRevNo.SelectedIndex = cmbRevNo.SelectedIndex - 1;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbRevNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(_DocumentType=="Quotation")
                    //_Datatable = objclsBLLQuotation.GetRevisionHistory_ByRevNo(cmbRevNo.Text.Trim(), _RefNo).Tables[0];

                dgvRevisionHistry.DataSource = _Datatable;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (_DocumentType == "Quotation")
                    //_Datatable = objclsBLLQuotation.GetRevisionHistory_ByRevNo(cmbRevNo.Text.Trim(), _RefNo).Tables[0];

                dgvRevisionHistry.DataSource = _Datatable;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void txtRefCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_DocumentType == "Quotation")
                    //_Datatable = objclsBLLQuotation.GetRevisionHistory_ByRevNo(cmbRevNo.Text.Trim(), _RefNo).Tables[0];

                dgvRevisionHistry.DataSource = _Datatable;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvRevisionHistry_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //FormMode = enmFormMode.Save;
                clsBeansFind.ReturnValue = dgvRevisionHistry.CurrentRow.Cells[0].Value.ToString();
                clsBeansFind.ReturnValue2 = dgvRevisionHistry.CurrentRow.Cells[1].Value.ToString();

                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog(_msgTitle, ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        
    }
}