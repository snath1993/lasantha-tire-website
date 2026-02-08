using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;


namespace UserAutherization
{
    public partial class frmViewerJobReturn : Form
    {
        DataSet ds = new DataSet();

        public frmViewerJobReturn(frmJobReturn frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void frmViewerJobReturn_Load(object sender, EventArgs e)
        {
            string Myfullpath;

            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRJobReturn.rpt";
            
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);

            crvInvoice.ReportSource = crp; 

        }
    }
}