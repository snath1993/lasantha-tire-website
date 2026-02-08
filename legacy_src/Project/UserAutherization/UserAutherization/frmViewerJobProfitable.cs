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
    public partial class frmViewerJobProfitable : Form
    {
        DataSet ds = new DataSet();

        public frmViewerJobProfitable(frmJobReturn frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void frmViewerJobProfitable_Load(object sender, EventArgs e)
        {
            string Myfullpath;

            ReportDocument crp = new ReportDocument();

            Myfullpath = Path.GetFullPath(clsPara.StrRepFolder + "\\CRJobActual.rpt");
            
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);

            crvInvoice.ReportSource = crp; 

        }
    }
}