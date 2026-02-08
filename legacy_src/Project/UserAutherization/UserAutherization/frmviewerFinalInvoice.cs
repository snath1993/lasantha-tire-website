using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmviewerFinalInvoice : Form
    {
        DataSet ds = new DataSet();
        string StrReportName = String.Empty;

        public frmviewerFinalInvoice(frmFinalInvoice frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsFinalInvoice;
            StrReportName = frmParent.StrReportName;
        }
        private void frmviewerFinalInvoice_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\"+ StrReportName +".rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoice.ReportSource = crp;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }
        }
    }
}
