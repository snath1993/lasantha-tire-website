using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;

namespace UserAutherization
{
    public partial class frmVieverCreditNotelList : Form
    {
        public frmVieverCreditNotelList(frmCreditnotelist frmParent)
        {
            InitializeComponent();
            setConnectionString();
            ds = frmParent.dsCreditNote;
        }

        public static string ConnectionString;
        DataSet ds;

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmVieverCreditNotelList_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CrCreditNoteDetails.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                ReportDocument crp = new ReportDocument();
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                CRVCreditNoteList.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}