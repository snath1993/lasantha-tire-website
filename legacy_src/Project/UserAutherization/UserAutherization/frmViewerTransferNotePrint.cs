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

    public partial class frmViewerTransferNotePrint : Form
    {

        DataSet ds = new DataSet();


        public frmViewerTransferNotePrint(frmWareHouseTrans frmParent)
        {
            InitializeComponent();
            ds = frmParent.DsItemWise;
        }

        private void frmViewerTransferNotePrint_Load(object sender, EventArgs e)
        {
            try
            {
                string Myfullpath;
                ReportDocument crp = new ReportDocument();
                Myfullpath = System.Windows.Forms.Application.StartupPath + "\\REPORTS\\rptTransferNote.rpt";
                crp.Load(Myfullpath);
                crp.SetDataSource(ds);
                crvTransferNote.ReportSource = crp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);
            }

        }
    }
}