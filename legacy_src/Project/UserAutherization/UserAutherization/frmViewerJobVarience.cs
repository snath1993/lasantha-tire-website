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
    public partial class frmViewerJobVarience : Form
    {
        DataSet ds = new DataSet();

        public frmViewerJobVarience(frmJobReturn frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void frmViewerJobVarience_Load(object sender, EventArgs e)
        {
            string Myfullpath;

            ReportDocument crp = new ReportDocument();

            Myfullpath = Path.GetFullPath(clsPara.StrRepFolder + "\\CRJobVarience.rpt");
            
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);

            crvInvoice.ReportSource = crp; 

        }

       
    }
}