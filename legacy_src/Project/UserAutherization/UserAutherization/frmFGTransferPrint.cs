using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DataAccess;

namespace WarehouseTransfer
{
    public partial class frmFGTransferPrint : Form
    {
        DataSet ds;
        public static string ConnectionString;
        public frmFGTransferPrint(frmFGTransfer frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.ObjRetrnNoteDS;
        }
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        //private void frmReportViewerIssueNote_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        CRIssueNote issue_note = new CRIssueNote();
        //        issue_note.SetDataSource(ds);
        //        crystalReportViewer1.ReportSource = issue_note;
        //    }
        //    catch { }
        //}

        private void frmPrintReturnNote_Load(object sender, EventArgs e)
        {
            try
            {
                CRFGTransferForm ObjCRPrintReturn = new CRFGTransferForm();
                ObjCRPrintReturn.SetDataSource(ds);
                crvReturnNoteprint.ReportSource = ObjCRPrintReturn;
            }
            catch { }
        }
    }
}