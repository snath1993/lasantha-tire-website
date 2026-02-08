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
    public partial class frmViewerFinishgoodsTransfer : Form
    {
        DataSet ds = new DataSet();

        public frmViewerFinishgoodsTransfer(frmFinishGoodTransfer frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsEst;

        }

        

        private void frmViewerFinishgoodsTransfer_Load(object sender, EventArgs e)
        {
            string Myfullpath;

            ReportDocument crp = new ReportDocument();
            Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\CRFinishedGoodsTransfer.rpt";
            crp.Load(Myfullpath);
            crp.SetDataSource(ds);

            crvInvoice.ReportSource = crp; 

        }

        private void crvInvoice_Load(object sender, EventArgs e)
        {

        }

        //private void frmViewerFinishgoodsTransfer_Load_1(object sender, EventArgs e)
        //{

        //}
    }
}