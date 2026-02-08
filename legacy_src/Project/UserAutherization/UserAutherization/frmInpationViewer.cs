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
    public partial class frmInpationViewer : Form
    {
        private DataSet ds;

        public frmInpationViewer(InpatientMaster frmParent)
        {
            InitializeComponent();
            ds = frmParent.ds;
        }

        private void frmInpationViewer_Load(object sender, EventArgs e)
        {

            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\crInpation.rpt";// Path.GetFullPath("CRMaxInvoiceList.rpt");
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
