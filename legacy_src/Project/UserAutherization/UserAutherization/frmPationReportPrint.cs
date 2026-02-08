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
    public partial class frmPationReportPrint : Form
    {
        DataSet ds = new DataSet();
        public frmPationReportPrint(frmPationReport parent)
        {
            InitializeComponent();
            ds = parent.dt;
        }

        private void frmPationReportPrint_Load(object sender, EventArgs e)
        {

            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\crPationReport.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvInvoice.ReportSource = crp;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
