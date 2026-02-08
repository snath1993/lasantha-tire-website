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
    public partial class frmLabPrint : Form
    {
        DataSet ds = new DataSet();
        public frmLabPrint(frmLab frmParent)
        {
            InitializeComponent();
            ds = frmParent.dsscanchanel;

        }

        private void frmLabPrint_Load(object sender, EventArgs e)
        {
            string Myfullpath;

            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\crLabTest.rpt";

            crp.Load(Myfullpath);
            crp.SetDataSource(ds);

            crvInvoice.ReportSource = crp;
        }
    }
}
